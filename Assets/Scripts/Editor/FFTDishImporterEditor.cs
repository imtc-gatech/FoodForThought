using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(FFTDish))]
public class FFTDishImporterEditor : Editor {

    public static string COUNTER_ASSET_PATH = "Assets/Resources/Counter/";

    bool canAlwaysBurn = true;

    FFTDish DI;

    List<string> stepLabels;

    bool debugMode = false;
    bool verboseMode = false;

    bool lockPrefab = false;

    bool duplicateStep = false;
    bool deleteStep = false;
    int stepActionIndex = 0;

    // Styles for different aspects of the GUI
    GUIStyle DefaultTextDisplay;
    GUIStyle InactiveTextDisplay;
    GUIStyle StepHeadlineDisplay;

    // Used with GUI.contentcolor to selectively change aspects of the text when active/inactive
    Color DefaultTextColor;
    Color InactiveTextColor;

    
    // Colors used to highlight different Station Destinations (not utilized yet)
    Color ChopColor;
    Color PrepColor;
    Color SpiceColor;
    Color CookColor;

    // Colors used to maintain GUI appearances
    Color GUIbackgroundColorDefault;
    Color GUIbackgroundColorActive;
    Color GUIbackgroundColorInactive;

    string[] counterAssetPaths;
    string[] counterAssetDisplay;

    int assetIndex = 0;

    Texture2D FlatWhiteTexture;

    public void OnEnable()
    {
        // Grab our Editor target so it is a bit more semantic (and to eliminate constant casts to DishImporter):
        DI = (FFTDish)target;

        GUIbackgroundColorDefault = GUI.color;
        GUIbackgroundColorActive = GUIbackgroundColorDefault;
        GUIbackgroundColorInactive = FFTVisualTools.ReturnColorFrom255Values(127, 127, 127);

        FlatWhiteTexture = Resources.Load("GUITextures/FlatWhite32") as Texture2D;

        // Assign colors:
        DefaultTextColor = new Color(0.9f, 0.9f, 0.9f);
        InactiveTextColor = new Color(0.4f, 0.4f, 0.4f);

        CookColor = FFTVisualTools.ReturnColorFrom255Values(230, 34, 32);
        ChopColor = FFTVisualTools.ReturnColorFrom255Values(86, 168, 219);
        SpiceColor = FFTVisualTools.ReturnColorFrom255Values(107, 230, 120);
        PrepColor = FFTVisualTools.ReturnColorFrom255Values(254, 213, 160);

        DefaultTextDisplay = new GUIStyle();
        //DefaultTextDisplay.fontStyle = FontStyle.Bold;
        DefaultTextDisplay.alignment = TextAnchor.MiddleLeft;
        DefaultTextDisplay.normal.textColor = Color.white;
        //DefaultTextDisplay.fontSize = 14;

        InactiveTextDisplay = new GUIStyle();
        InactiveTextDisplay.alignment = TextAnchor.MiddleLeft;
        InactiveTextDisplay.normal.textColor = Color.gray;

        StepHeadlineDisplay = new GUIStyle(); 
        StepHeadlineDisplay.alignment = TextAnchor.MiddleLeft;
        StepHeadlineDisplay.padding = new RectOffset(5, 0, 3, 2);
        StepHeadlineDisplay.normal.textColor = Color.white;

        CreateCounterAssetList();

    }

    public void OnDisable()
    {

    }

    public override void OnInspectorGUI()
    {
        // Make sure our GUI text color is reset before starting to draw the InspectorGUI
        GUI.contentColor = DefaultTextColor;

        // Selector for Food prefabs, which utilized by underlying logic
        DisplayFoodPrefabSelector();

        // Show our current Step, may be deprecated by new behavior in StepGUIs to display different layers to the image
        DisplayCurrentStep();

        // Show the Destination, GameplayType and subsequent GUI elements for each Step (longest block of code)
        DisplayStepGUIs();

        // Shows the Advanced and Debug buttons, should probably be removed before release
        DisplayDeveloperGUIs();

        DI.RefreshDisplayElements();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(DI);
        }

    }



    void DisplayStepGUIs()
    {
        if (DI.StepDataObjects != null && DI.StepDataObjects.Count > 0)
        {
            int currentStepInStepObjectLoop = 0;

            foreach (FFTStep step in DI.StepDataObjects)
            {
                //insert some space before the step button
                GUILayout.BeginHorizontal(GUILayout.Height(10)); EditorGUILayout.Space(); GUILayout.EndHorizontal();

                GUI.color = GUIbackgroundColorActive; // buttons are always the same color

                Color currentGUIColor = GUI.color;

                if (currentStepInStepObjectLoop == DI.CurrentStep) // highlight the current step if it is the "active" one
                {
                    GUI.contentColor = Color.white;
                    GUI.color = Color.cyan;
                }

                if (GUILayout.Button("Step " + (step.Index + 1))) // if we click the different step button, change the active one (CurrentStep) we are working on
                    DI.CurrentStep = currentStepInStepObjectLoop;

                if (currentStepInStepObjectLoop == DI.CurrentStep)
                {
                    //GUI.color = GUIbackgroundColorActive;
                    StepHeadlineDisplay.normal.textColor = Color.green;
                    GUI.contentColor = Color.white;
                    GUI.color = currentGUIColor;
                }
                else
                {
                    GUI.color = GUIbackgroundColorInactive; // make non-selected UI elements inactive
                }

                if (currentStepInStepObjectLoop == DI.CurrentStep)
                {
                    StepHeadlineDisplay.normal.textColor = Color.white;
                    GUI.contentColor = Color.white;
                }

                //insert some space after the step button
                GUILayout.BeginHorizontal(GUILayout.Height(5)); EditorGUILayout.Space(); GUILayout.EndHorizontal();

                //EditorGUILayout.TextArea(step.StepObject.name, StepHeadlineDisplay);

                //DRAWING the TEXTURE buttons
				
                if (DI.CurrentStepObject != null) //DI.FoodVisualStates[i].GetComponent<FFTStepVisualState>().Uncooked != null
                {
                    //DI.CurrentStepObject.Uncooked
                    GUILayout.BeginHorizontal();

                    Rect TextureSelect = GetRectSpaceForGUIDrawing(50, 1);
                    TextureSelect.width = 50;
                    /*
                     * // DEPRECATED: old code to draw the texture itself, for reference purposes only.
                    EditorGUI.DrawPreviewTexture(TextureSelect, DI.StepDataObjects[currentStepInStepObjectLoop].Uncooked.renderer.sharedMaterial.mainTexture);
                    TextureSelect.x += 60;
                    TextureSelect.width = 50;
                     */

                    int i = 0;

                    foreach (GameObject go in DI.FoodVisualStates)
                    {
                        currentGUIColor = GUI.color;
                        if (i == step.VisualStateIndex) //currentStepInStepObjectLoop
                        {
                            GUI.color = Color.cyan;
                        }
						
						GameObject buttonTextureObject;
						if (DI.FoodVisualStates[i].GetComponent<FFTStepVisualState>().Uncooked != null)
							buttonTextureObject = DI.FoodVisualStates[i].GetComponent<FFTStepVisualState>().Uncooked;
						else //(buttonTextureObject == null)
						{
							buttonTextureObject = DI.FoodVisualStates[i].GetComponent<FFTStepVisualState>().CookedNormal;
						}
						
                        if (GUI.Button(TextureSelect, buttonTextureObject.GetComponent<Renderer>().sharedMaterial.mainTexture))
                        {
                            if (currentStepInStepObjectLoop == DI.CurrentStep)
                            {
                                if (currentStepInStepObjectLoop == step.Index)
                                {
                                    step.SetVisibility(false);
                                    //Debug.Log(step.ToString() + " button pressed");
                                    step.VisualStateIndex = i;
                                    step.StepObject = DI.FoodVisualStates[i];
                                    step.SetVisibility(true);
                                }
                            }
                        }
                        
                        GUI.color = currentGUIColor;
                        TextureSelect.x += 60;

                        i++;
                    }
                    
                    GUILayout.EndHorizontal();
                }

                DisplayFoodDisplayControl(currentStepInStepObjectLoop);

                

                GUILayout.BeginHorizontal();
                step.Destination = (FFTStation.Type)EditorGUILayout.EnumPopup("Destination: ", step.Destination);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                step.Gameplay = (FFTStation.GameplayType)EditorGUILayout.EnumPopup("Gameplay: ", step.Gameplay);
                GUILayout.EndHorizontal();

                // ***********************************
                // ELAPSED TIME GAMEPLAYTYPE GUI BEGIN
                // ***********************************

                if (step.Gameplay == FFTStation.GameplayType.ElapsedTime)
                {
                    // Clear MiniGame if it is present (OLD)

                    /* OLD MINIGAME CODE
                    if (step.MiniGame != null)
                    {
                        DI.LoadMiniGame(null, currentStepInStepObjectLoop);
                    }
                     */

                    // display our GUIBox Here

                    GUILayout.BeginHorizontal();

                    if (step.Destination == FFTStation.Type.Cook || canAlwaysBurn)
                    {
                        if (step.AllowsCooking)
                            step.Parameters.IsCookable = true; //GUILayout.Toggle(step.Parameters.IsCookable, "Cookable");
                        else
                        {
                            GUI.contentColor = InactiveTextColor;
                            GUILayout.Toggle(step.Parameters.IsCookable, "Cookable");
                            GUI.contentColor = DefaultTextColor;
                        }
                    }
                    else
                    {
                        if (step.AllowsCooking)
                        {
                            step.Parameters.IsCookable = GUILayout.Toggle(step.Parameters.IsCookable, "Flavor Decay");
                            step.Parameters.IsBurnable = false;
                        }
                        else
                        {
                            GUI.contentColor = InactiveTextColor;
                            GUILayout.Toggle(step.Parameters.IsCookable, "Flavor Decay");
                            GUI.contentColor = DefaultTextColor;
                        }
                    }

                    if (step.Parameters.IsCookable)
                    {
                        if (step.Destination == FFTStation.Type.Cook || canAlwaysBurn)
                        {
                            step.Parameters.IsBurnable = GUILayout.Toggle(step.Parameters.IsBurnable, "Red Zone");
                        }
                        else
                        {
                            step.Parameters.IsBurnable = false;
                        }
                        step.Parameters.UsesPeakFlavor = GUILayout.Toggle(step.Parameters.UsesPeakFlavor, "Peak");
                    }

                    GUILayout.EndHorizontal();

                    if (step.Parameters.IsBurnable)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Uncooked:");
                        step.Parameters.Uncooked = EditorGUILayout.FloatField(step.Parameters.Uncooked, GUILayout.Width(40));
                        GUILayout.Label("Ready:");
                        step.Parameters.Cooked = EditorGUILayout.FloatField(step.Parameters.Cooked, GUILayout.Width(40));
                        GUILayout.Label("Overdone:");
                        step.Parameters.Burned = EditorGUILayout.FloatField(step.Parameters.Burned, GUILayout.Width(40));
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        if (step.Parameters.IsCookable)
                        {
                            GUILayout.BeginHorizontal();
                            if (step.Destination == FFTStation.Type.Cook || canAlwaysBurn)
                                GUILayout.Label("Uncooked:");
                            else
                                GUILayout.Label("Time to finish:");

                            step.Parameters.Uncooked = EditorGUILayout.FloatField(step.Parameters.Uncooked, GUILayout.Width(40));
                            if (step.Parameters.UsesPeakFlavor || step.Destination != FFTStation.Type.Cook)
                            {
                                GUILayout.Label("Ready:");
                                step.Parameters.Cooked = EditorGUILayout.FloatField(step.Parameters.Cooked, GUILayout.Width(40));
                            }
                            else // we don't need a range for the 'Ready' value, since it doesn't matter when they remove it from the "stove"
                            {
                                GUI.contentColor = InactiveTextColor;
                                GUILayout.Label("Ready:");
                                EditorGUILayout.FloatField(step.Parameters.Cooked, GUILayout.Width(40));
                                GUI.contentColor = DefaultTextColor;
                            }
                            if (step.Destination == FFTStation.Type.Cook || canAlwaysBurn)
                            {
                                GUI.contentColor = InactiveTextColor;
                                GUILayout.Label("Overdone:");
                                EditorGUILayout.TextArea("0", GUILayout.Width(40));
                                GUI.contentColor = DefaultTextColor;
                            }
                            GUILayout.EndHorizontal();
                        }
                        else
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Time to finish:");
                            step.Parameters.Uncooked = EditorGUILayout.FloatField(step.Parameters.Uncooked, GUILayout.Width(40));
                            GUI.contentColor = InactiveTextColor;
                            GUILayout.Label("Ready:");
                            EditorGUILayout.FloatField(step.Parameters.Cooked, GUILayout.Width(40));
                            GUI.contentColor = DefaultTextColor;
                            GUILayout.EndHorizontal();
                        }
                    }

                    // Display the "Peak Flavor" Variable.
                    if (step.Parameters.UsesPeakFlavor && step.Parameters.IsCookable)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Peak Flavor (% of 'Ready'):");
                        step.Parameters.PeakPercentage = EditorGUILayout.IntSlider(step.Parameters.PeakPercentage, FFTStepParameters.PEAK_FLOOR, FFTStepParameters.PEAK_CEILING);
                        /*
                        if (GUILayout.Button("-", GUILayout.Width(25)))
                        {
                            step.Parameters.Peak -= FFTTimingParameters.PEAK_STEP;
                        }
                        step.Parameters.Peak = EditorGUILayout.IntField(step.Parameters.Peak, GUILayout.Width(50));
                        if (GUILayout.Button("+", GUILayout.Width(25)))
                        {
                            step.Parameters.Peak += FFTTimingParameters.PEAK_STEP;
                        }
                         */
                        GUILayout.EndHorizontal();
                    }
                    else if (step.Parameters.IsCookable)
                    {
                        GUI.contentColor = InactiveTextColor;
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Peak Flavor (% of 'Ready'):");
                        EditorGUILayout.IntSlider(step.Parameters.PeakPercentage, FFTStepParameters.PEAK_FLOOR, FFTStepParameters.PEAK_CEILING);
                        GUILayout.EndHorizontal();
                        GUI.contentColor = DefaultTextColor;
                    }
                    if (verboseMode)
                    {
                        GUILayout.BeginHorizontal();
                        step.ElapsedTime = EditorGUILayout.FloatField("Elapsed Time:", step.ElapsedTime);
                        GUILayout.EndHorizontal();
                    }

                }

                // ************************************
                // ELAPSED TIME GAMEPLAYTYPE GUI FINISH
                // ************************************


                // *******************************
                // MINIGAME GAMEPLAYTYPE GUI BEGIN
                // *******************************

                if (step.Gameplay == FFTStation.GameplayType.MiniGame)
                {
                    GUILayout.BeginHorizontal();
                    GUIContent mgLabel = new GUIContent("Minigame:", "Selects minigame type.");
                    step.MinigameType = (MG_Minigame.Type)EditorGUILayout.EnumPopup(mgLabel, step.MinigameType);
                    GUILayout.EndHorizontal();

                    switch (step.MinigameType)
                    {
                        case MG_Minigame.Type.Placeholder:
                            GUILayout.BeginHorizontal();
                            GUIContent mgPlaceholderUsesDelayLabel = new GUIContent("Uses delay:", "Determines if delay should be used for this placeholder game (bool value test).");
                            step.MinigameVariableContainer.Placeholder_UseDelay = EditorGUILayout.Toggle(mgPlaceholderUsesDelayLabel, step.MinigameVariableContainer.Placeholder_UseDelay);
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            GUIContent mgPlaceholderDelayLabel = new GUIContent("Delay in seconds:", "Determines how long it takes for the check mark to appear in the placeholder minigame.");
                            step.MinigameVariableContainer.Placeholder_DelayToWinInSeconds = EditorGUILayout.FloatField(mgPlaceholderDelayLabel, step.MinigameVariableContainer.Placeholder_DelayToWinInSeconds);
                            GUILayout.EndHorizontal();
                            break;
                        case MG_Minigame.Type.Sorting:
                            GUILayout.BeginHorizontal();
                            GUIContent ttSortNumVeg = new GUIContent("Number of items:", "Number of food items to sort. Larger numbers are more difficult. default = 10");
                            step.MinigameVariableContainer.Sorting_NumVegetables = EditorGUILayout.IntField(ttSortNumVeg, step.MinigameVariableContainer.Sorting_NumVegetables);
                            GUILayout.EndHorizontal();
                            break;
						case MG_Minigame.Type.Chopping:
                            GUILayout.BeginHorizontal();
							GUIContent ttSpiceGameMode = new GUIContent("Game Mode:", "Cuts: Checks only number of cuts (width does not matter). Proportionality: Cuts must match an equal width.");
							step.MinigameVariableContainer.Chopping_GameMode = (MG2_GameMode)EditorGUILayout.EnumPopup(ttSpiceGameMode, step.MinigameVariableContainer.Chopping_GameMode);
							GUILayout.EndHorizontal();
							GUILayout.BeginHorizontal();
							string chopHelpText = "";
							if (step.MinigameVariableContainer.Chopping_GameMode == MG2_GameMode.cuts)
								chopHelpText = "Max 8. Number of cuts possible. Width does not matter. (default 8)";
							else
								chopHelpText = "Max 8. Number of cuts possible. Width DOES matter. (default 8)";
                            GUIContent ttChopNumCuts = new GUIContent("Number of cuts:", chopHelpText);
                            step.MinigameVariableContainer.Chopping_MaxCuts = EditorGUILayout.IntField(ttChopNumCuts, step.MinigameVariableContainer.Chopping_MaxCuts);
                            GUILayout.EndHorizontal();
                            break;
                        case MG_Minigame.Type.Stirring:
                            //none yet
							/*
							Stirring_NumberOfVegetables = 20;
							Stirring_TotalCookingTime = 45f;
							Stirring_TimeBeforeNeedingToStir = 0.4f;
							Stirring_TimeBeforeEnteringDangerZone = 10f;
							Stirring_TimeBeforeFullyBurned = 5f;
							Stirring_BurnRecoveryMultiplier = 1.0f;
							 */
							GUILayout.BeginHorizontal();
							GUIContent ttStirNumVeg = new GUIContent("Number of vegetables:", "Number of vegetables in pan. 20 is the default, and should not be strayed from much.");
							step.MinigameVariableContainer.Stirring_NumberOfVegetables = EditorGUILayout.IntField(ttStirNumVeg, step.MinigameVariableContainer.Stirring_NumberOfVegetables);
							GUILayout.EndHorizontal();
						
							GUILayout.BeginHorizontal();
							GUIContent ttStirTotalTime = new GUIContent("Total cooking time:", "Total time to finish cooking the vegetables. Values can range from 0-60 seconds. Default is 30.");
							step.MinigameVariableContainer.Stirring_TotalCookingTime = Mathf.Clamp(EditorGUILayout.FloatField(ttStirTotalTime, step.MinigameVariableContainer.Stirring_TotalCookingTime), 0, 60);
							GUILayout.EndHorizontal();
						
							float totalCookingTime = step.MinigameVariableContainer.Stirring_TotalCookingTime; //used for clamping the below values
						
							GUILayout.BeginHorizontal();
							GUIContent ttStirTimeBeforeStir = new GUIContent("Time before stir:", "The amount of time in seconds before the vegetable begins to burn. This can be very short, approximately 1% of the total time, or very large to make burning infrequent. Default is ~0.5 seconds.");
							step.MinigameVariableContainer.Stirring_TimeBeforeNeedingToStir = Mathf.Clamp(EditorGUILayout.FloatField(ttStirTimeBeforeStir, step.MinigameVariableContainer.Stirring_TimeBeforeNeedingToStir), 0, totalCookingTime);
							GUILayout.EndHorizontal();
						
							GUILayout.BeginHorizontal();
							GUIContent ttStirTimeBeforeDanger = new GUIContent("Time before danger:", "The amount of time in seconds before the vegetable enters the 'danger zone', where burning occurs that cannot be completely undone. The food is safe during this time until this amount of time elapses. Default is 10 seconds.");
							step.MinigameVariableContainer.Stirring_TimeBeforeEnteringDangerZone = Mathf.Clamp(EditorGUILayout.FloatField(ttStirTimeBeforeDanger, step.MinigameVariableContainer.Stirring_TimeBeforeEnteringDangerZone), 0, totalCookingTime);
							GUILayout.EndHorizontal();
						
							GUILayout.BeginHorizontal();
							GUIContent ttStirTimeBeforeBurnt = new GUIContent("Time before burnt:", "The amount of time in seconds before the vegetable is completely burned. At this point, some damage cannot be recovered by stirring the food item. Default is 5 seconds.");
							step.MinigameVariableContainer.Stirring_TimeBeforeFullyBurned = Mathf.Clamp(EditorGUILayout.FloatField(ttStirTimeBeforeBurnt, step.MinigameVariableContainer.Stirring_TimeBeforeFullyBurned), 0, totalCookingTime);
							GUILayout.EndHorizontal();
						
							GUILayout.BeginHorizontal();
							GUIContent ttStirBurnRecovery = new GUIContent("Burn Recovery Rate:", "A multiplier that affects the recovery of burned/danger vegetables as they are stirred. Default is 1.0. Increase this value if you want stirring to 'undo burning' at a faster rate, but only by a tenth of a point at a time or so (1.0 -> 1.1, etc).");
							step.MinigameVariableContainer.Stirring_BurnRecoveryMultiplier = Mathf.Clamp(EditorGUILayout.FloatField(ttStirBurnRecovery, step.MinigameVariableContainer.Stirring_BurnRecoveryMultiplier), 0, 5);
							GUILayout.EndHorizontal();
							
							break;
                        case MG_Minigame.Type.Spicing:
                            
                            //This works, AW YEEEEEAAAAAAH 
							//(This is Saturday before the study, forgive my goofy comment)
                            GUILayout.BeginHorizontal();
							GUIContent ttSpiceNumBottles = new GUIContent("Number of bottles:", "Number of spices to add (between 1-3).");
							step.MinigameVariableContainer.Spicing_NumberOfBottles = EditorGUILayout.IntField(ttSpiceNumBottles, step.MinigameVariableContainer.Spicing_NumberOfBottles);
							GUILayout.EndHorizontal();
						#region bottlecodeTest
							/*
							//Not currently working, will have to amend later
							GUILayout.BeginHorizontal();
                            GUIContent ttSpiceBottleNum = new GUIContent("Number of bottles:", "Number of bottles to distribute on 'food item'. Maximum allowed = 3");
                            EditorGUILayout.IntField(ttSpiceBottleNum, step.MinigameVariableContainer.Spicing_BottleList.Count);
                            if (GUILayout.Button("+"))
                            {
                                if (step.MinigameVariableContainer.Spicing_BottleList.Count < 3)
                                    step.MinigameVariableContainer.Spicing_BottleList.Add(MGSpiceGame.SpiceBottle.BlueBulge);
                            }
                            if (GUILayout.Button("-"))
                            {
                                if (step.MinigameVariableContainer.Spicing_BottleList.Count > 0)
                                    step.MinigameVariableContainer.Spicing_BottleList.RemoveAt(step.MinigameVariableContainer.Spicing_BottleList.Count - 1);
                            }

                            GUILayout.EndHorizontal();
                            if (step.MinigameVariableContainer.Spicing_BottleList.Count > 0)
                            {
                                foreach (MGSpiceGame.SpiceBottle bottle in step.MinigameVariableContainer.Spicing_BottleList)
                                {

                                    GUILayout.BeginHorizontal();
                                    bottle = (MGSpiceGame.SpiceBottle)EditorGUILayout.EnumPopup(bottle);
                                    GUILayout.EndHorizontal();
                                }
                            }
                            */
						#endregion
                            
                            break;
                        case MG_Minigame.Type.Blending:
                            GUILayout.BeginHorizontal();
							GUIContent ttBlendNumIngredients = new GUIContent("Number of ingredients:", "Number of ingredients to blend (between 0-3). Choosing zero will result in a random number of ingredients (2-3) being populated.");
							step.MinigameVariableContainer.Blending_NumIngredients = Mathf.Clamp(EditorGUILayout.IntField(ttBlendNumIngredients, step.MinigameVariableContainer.Blending_NumIngredients), 0, 3);
							GUILayout.EndHorizontal();
                            break;
                    }


                    /* OLD MINIGAME CODE
                    GUILayout.BeginHorizontal();
                    //GUILayout.Label("Food:");
                    step.MiniGamePrefab = EditorGUILayout.ObjectField(step.MiniGamePrefab, typeof(FFTMiniGameBase), true, GUILayout.Width(180)) as FFTMiniGameBase;
                    if (GUILayout.Button("Load", GUILayout.Width(75)))
                    {
                        if (step.MiniGamePrefab != null)
                        {
                            DI.LoadMiniGame(step.MiniGamePrefab.gameObject, currentStepInStepObjectLoop);
                        }
                    }
                    if (GUILayout.Button("Clear", GUILayout.Width(75)))
                    {
                        DI.LoadMiniGame(null, currentStepInStepObjectLoop);
                        //step.MiniGamePrefab = null;
                    }
                    
                    GUILayout.EndHorizontal();
                     */


                }

                // ************************************
                // STEP DUPLICATION / DELETION CODE
                // ************************************

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Duplicate"))
                {
                    duplicateStep = true;
                    stepActionIndex = step.Index;
                }
                if (GUILayout.Button("Delete"))
                {
                    deleteStep = true;
					stepActionIndex = step.Index;
                }
                GUILayout.EndHorizontal();

                // ************************************
                // BUFFER BETWEEN STEPS
                // ************************************

                GUILayout.BeginHorizontal();
                Rect buffer = GetRectSpaceForGUIDrawing(10, 0);
                currentGUIColor = GUI.color;
                if (currentStepInStepObjectLoop == DI.CurrentStep)
                {
                    GUI.color = FFTVisualTools.ReturnColorFrom255Values(0, 127, 127);
                }
                else
                {
                    GUI.color = Color.black;
                }
                GUI.DrawTexture(buffer, FlatWhiteTexture);
                GUI.color = currentGUIColor;
                GUILayout.EndHorizontal();

                currentStepInStepObjectLoop++;

            } //end foreach

            if (duplicateStep)
            {
                duplicateStep = false;
                FFTStep dupeStep = DI.StepDataObjects[stepActionIndex].Clone();
                
                dupeStep.Index++;
                int checkIndex = dupeStep.Index;
                foreach (FFTStep step in DI.StepDataObjects)
                {
                    if (checkIndex == step.Index)
                    {
                        step.Index++;
                        checkIndex++;
                    }
                }
                

                DI.StepDataObjects.Insert(stepActionIndex, dupeStep);
                // swapping them now that the list has been increased by one element
                DI.StepDataObjects[stepActionIndex] = DI.StepDataObjects[stepActionIndex + 1];
                DI.StepDataObjects[stepActionIndex + 1] = dupeStep;

                DI.StepGameObjectReferences.Insert(stepActionIndex, dupeStep.StepObject);
                //likewise:
                DI.StepGameObjectReferences[stepActionIndex] = DI.StepGameObjectReferences[stepActionIndex + 1];
                DI.StepGameObjectReferences[stepActionIndex + 1] = dupeStep.StepObject;
            }

            if (deleteStep)
            {
                Debug.Log(System.DateTime.Now +  "Step " + (stepActionIndex + 1) + " deleted.");
                deleteStep = false;
				
				DI.StepDataObjects[stepActionIndex].SetVisibility(false);
				DI.StepDataObjects.RemoveAt(stepActionIndex);
                DI.StepGameObjectReferences.RemoveAt(stepActionIndex);
				
				int i = 0;
                foreach (FFTStep step in DI.StepDataObjects)
                {
                    step.Index = i;
                    i++;
                }
                if (i == 0)
                {
                    DI.FoodPrefab = null;
                    DI.LoadFoodPrefab(DI.FoodPrefab);
                    DI.RefreshDisplayElements();
                }
            }
        }

    }


    // Simply displays an enumPopup with the Current Step
    void DisplayCurrentStep()
    {
        if (!DataPresent())
        {
            GUILayout.BeginHorizontal();
            string IntroText =  "Load a food prefab in the field above, then select 'Load'.\n";
            IntroText += "To clear the current food prefab, select 'Clear'.\n";
            IntroText += "\n";
            IntroText += "Questions? rob@imtc.gatech.edu\n";
            EditorGUILayout.TextArea(IntroText);
            GUILayout.EndHorizontal();
            return;
        }

        stepLabels = new List<string>();
        
        for (int i = 0; i < DI.StepGameObjectReferences.Count; i++)
        {
            stepLabels.Add("Step " + (i + 1).ToString());
        }

        string[] stepLabelsArray = stepLabels.ToArray();

        GUILayout.BeginHorizontal();
        GUILayout.Label("ID:", GUILayout.Width(40));
        DI.ID = EditorGUILayout.IntField(DI.ID, GUILayout.Width(40));
        GUILayout.Label("Current Step:", GUILayout.Width(100));
        DI.CurrentStep = EditorGUILayout.Popup(DI.CurrentStep, stepLabelsArray);
        GUILayout.EndHorizontal();  
        
    }

    void DisplayFoodDisplayControl(int currentStep)
    {
        float cookingDisplayHeight = 20.0f;

        if (currentStep != DI.CurrentStep)
        {
            GUILayout.BeginHorizontal();
            GUI.contentColor = InactiveTextColor;            
            EditorGUILayout.TextField("(Select button above to make this display active.", GUILayout.Height(cookingDisplayHeight));
            GUI.contentColor = DefaultTextColor;
            GUILayout.EndHorizontal();

            
            GUILayout.BeginHorizontal();
            GUI.contentColor = InactiveTextColor;
            Vector2 SliderPositionsChanged = new Vector2(0, 1);
            EditorGUILayout.MinMaxSlider(ref SliderPositionsChanged.x, ref SliderPositionsChanged.y, 0, 1);
            GUI.contentColor = DefaultTextColor;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.contentColor = InactiveTextColor;
            EditorGUILayout.TextField("Total seconds:", "--");
            GUI.contentColor = DefaultTextColor;
            GUILayout.EndHorizontal();

            return;
        }
        else
        {
            DisplayFoodDisplayControl();
        }
    }

    void DisplayFoodDisplayControl()
    {
        float cookingDisplayHeight = 20.0f;
        float outlineInPixels = 2.0f;

        if ((DI.CurrentStepObject.Gameplay != FFTStation.GameplayType.ElapsedTime))
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.TextField("(Area changes depending on the Step/Gameplay selected)", GUILayout.Height(cookingDisplayHeight));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.contentColor = InactiveTextColor;
            EditorGUILayout.TextField("Total seconds:", "--");
            GUI.contentColor = DefaultTextColor;

            GUILayout.EndHorizontal();

            return;
        }
        GUILayout.BeginHorizontal();

        Color displayBorder = Color.black;
        Color displayYellow = Color.yellow;
        Color displayGreen = Color.green;
        Color displayRed = Color.red;
        Color displayPeak = FFTVisualTools.ReturnColorFrom255Values(100, 200, 100);
        Color displayPeakBorder = FFTVisualTools.ReturnColorFrom255Values(0, 150, 0);

        FFTStep ActiveStep = DI.CurrentStepObject;

        // Using this to get our current screen position so we can draw our own GUI elements
        Rect CurrentLayoutPosition = GetRectSpaceForGUIDrawing(cookingDisplayHeight, 5);

        Color currentColor = GUI.color;
        GUI.color = displayBorder;
        GUI.DrawTexture(new Rect(CurrentLayoutPosition.x, CurrentLayoutPosition.y, CurrentLayoutPosition.width, CurrentLayoutPosition.height), FlatWhiteTexture);
        GUI.color = currentColor; //Reset content color

        Rect BaseCanvas = new Rect(CurrentLayoutPosition.x + outlineInPixels, CurrentLayoutPosition.y + outlineInPixels,
                                    CurrentLayoutPosition.width - outlineInPixels * 2, CurrentLayoutPosition.height - outlineInPixels * 2);

        GUI.color = displayYellow;
        GUI.DrawTexture(BaseCanvas, FlatWhiteTexture);

        float CookedOffsetInPixels = (ActiveStep.Parameters.PercentageUncooked * BaseCanvas.width);
        
        Rect CookedRect = BaseCanvas;
        CookedRect.x += CookedOffsetInPixels;
        CookedRect.width -= CookedOffsetInPixels;

        if (ActiveStep.Parameters.IsBurnable)
        {
            // draw cooked and burned rectangles

            float BurnedOffsetInPixels = CookedOffsetInPixels + (ActiveStep.Parameters.PercentageCooked * BaseCanvas.width);

            Rect BurnedRect = BaseCanvas;
            BurnedRect.x += BurnedOffsetInPixels;
            BurnedRect.width -= BurnedOffsetInPixels;

            GUI.color = displayBorder;
            GUI.DrawTexture(CookedRect, FlatWhiteTexture);

            CookedRect.x += outlineInPixels;
            CookedRect.width -= outlineInPixels * 2;

            GUI.color = displayGreen;
            GUI.DrawTexture(CookedRect, FlatWhiteTexture);

            GUI.color = displayBorder;
            GUI.DrawTexture(BurnedRect, FlatWhiteTexture);

            BurnedRect.x += outlineInPixels;
            BurnedRect.width -= outlineInPixels * 2;

            GUI.color = displayRed;
            GUI.DrawTexture(BurnedRect, FlatWhiteTexture);

            if (ActiveStep.Parameters.UsesPeakFlavor)
            {
                float PeakOffsetInPixels = (ActiveStep.Parameters.PeakPercentage / 100.0f * (CookedRect.width - BurnedRect.width));

                Rect PeakRect = CookedRect;
                PeakRect.x += PeakOffsetInPixels;
                PeakRect.width = outlineInPixels * 3;

                GUI.color = displayPeakBorder;
                GUI.DrawTexture(PeakRect, FlatWhiteTexture);

                PeakRect.x += outlineInPixels;
                PeakRect.width = outlineInPixels;

                GUI.color = displayPeak;
                GUI.DrawTexture(PeakRect, FlatWhiteTexture);
            }

        }
        else if (ActiveStep.Parameters.IsCookable)
        {
            // draw only the cooked rectangle for the display

            GUI.color = displayBorder;
            GUI.DrawTexture(CookedRect, FlatWhiteTexture);

            CookedRect.x += outlineInPixels;
            CookedRect.width -= outlineInPixels * 2;

            GUI.color = displayGreen;
            GUI.DrawTexture(CookedRect, FlatWhiteTexture);

            if (ActiveStep.Parameters.UsesPeakFlavor)
            {
                float PeakOffsetInPixels = (ActiveStep.Parameters.PeakPercentage / 100.0f * CookedRect.width);

                Rect PeakRect = CookedRect;
                PeakRect.x += PeakOffsetInPixels;
                PeakRect.width = outlineInPixels * 3;

                GUI.color = displayPeakBorder;
                GUI.DrawTexture(PeakRect, FlatWhiteTexture);

                PeakRect.x += outlineInPixels;
                PeakRect.width = outlineInPixels;

                GUI.color = displayPeak;
                GUI.DrawTexture(PeakRect, FlatWhiteTexture);
            }

        }

        GUI.color = currentColor;//Reset color to previous default

        GUILayout.EndHorizontal();

        // MIN MAX SLIDER FOR EDITING TIMING PARAMETERS

        GUILayout.BeginHorizontal();

        if (ActiveStep.Parameters.IsBurnable)
        {
            Vector2 SliderPositions = new Vector2(
                ActiveStep.Parameters.Uncooked,
                ActiveStep.Parameters.TotalSeconds - ActiveStep.Parameters.Burned);

            Vector2 SliderPositionsChanged = SliderPositions;

            EditorGUILayout.MinMaxSlider(ref SliderPositionsChanged.x, ref SliderPositionsChanged.y, 0, ActiveStep.Parameters.TotalSeconds);

            float UncookedDelta = SliderPositionsChanged.x - SliderPositions.x; // x = Uncooked, changing this changes the Uncooked Value
            // positive is increase of Uncooked variable, negative is decrease

            float BurnedDelta = SliderPositionsChanged.y - SliderPositions.y;   // y = Uncooked + Cooked, changing this changes the Burned value
            // negative is increase of Burned variable, positive is decrease

            float MinValue = 0.1f;

            if (UncookedDelta != 0 &&
                ActiveStep.Parameters.Cooked - UncookedDelta > 0 &&
                ActiveStep.Parameters.Uncooked + UncookedDelta > 0)
            {
                ActiveStep.Parameters.Uncooked += UncookedDelta;
                ActiveStep.Parameters.Cooked -= UncookedDelta;

                if (ActiveStep.Parameters.Uncooked < MinValue)
                {
                    ActiveStep.Parameters.Cooked -= MinValue;
                    ActiveStep.Parameters.Uncooked += MinValue;
                }

                if (ActiveStep.Parameters.Cooked < MinValue)
                {
                    ActiveStep.Parameters.Cooked += MinValue;
                    ActiveStep.Parameters.Uncooked -= MinValue;
                }
            }

            if (BurnedDelta != 0 &&
                ActiveStep.Parameters.Burned - BurnedDelta > 0 &&
                ActiveStep.Parameters.Cooked + BurnedDelta > 0)
            {
                ActiveStep.Parameters.Burned -= BurnedDelta;
                ActiveStep.Parameters.Cooked += BurnedDelta;

                if (ActiveStep.Parameters.Burned < MinValue)
                {
                    ActiveStep.Parameters.Cooked -= MinValue;
                    ActiveStep.Parameters.Burned += MinValue;
                }

                if (ActiveStep.Parameters.Cooked < MinValue)
                {
                    ActiveStep.Parameters.Cooked += MinValue;
                    ActiveStep.Parameters.Burned -= MinValue;
                }

            }

        }
        else if (ActiveStep.Parameters.IsCookable)
        {
            int TotalSliderRange = (int)((ActiveStep.Parameters.TotalSeconds));
            int SliderPoint = (int)((ActiveStep.Parameters.Uncooked / TotalSliderRange) * 100);
            

            int DeltaSliderPoint = EditorGUILayout.IntSlider(SliderPoint, 0, 100);

            if (DeltaSliderPoint != SliderPoint)
            {
                ActiveStep.Parameters.Uncooked = (float)(DeltaSliderPoint * TotalSliderRange / 100);
                ActiveStep.Parameters.Cooked -= (float)((DeltaSliderPoint - SliderPoint) / 100);
            }

        }
        else // nothing to alter, black it out.
        {
            GUI.color = InactiveTextColor;

            Vector2 SliderPositionsChanged = new Vector2(0, 1);

            EditorGUILayout.MinMaxSlider(ref SliderPositionsChanged.x, ref SliderPositionsChanged.y, 0, 1);

            GUI.color = DefaultTextColor;
        }
        
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        ActiveStep.Parameters.TotalSeconds = EditorGUILayout.FloatField("Total seconds:", ActiveStep.Parameters.TotalSeconds);

        GUILayout.EndHorizontal();

    }

    void DrawPeakGUI(float PeakOffsetInPixels)
    {

    }

    // Loads food prefabs that are dragged/picked by the Inspector. The DishImporter handles the underlying logic to minimize user errors
    void DisplayFoodPrefabSelector()
    {
        GUILayout.BeginHorizontal();
        if (lockPrefab)
        {
            EditorGUILayout.TextArea(counterAssetDisplay[assetIndex] + " (unlock to change)", GUILayout.Width(180));
        }
        else
        {
            int oldAssetIndex = assetIndex;
            assetIndex = EditorGUILayout.Popup(assetIndex, counterAssetDisplay, GUILayout.Width(180));
            if (oldAssetIndex != assetIndex)
            {
                LoadFoodPrefabFromCurrentAssetListPosition();
            }
        }
        
        if (GUILayout.Button("Load", GUILayout.Width(75)) && assetIndex != 0)
        {
            LoadFoodPrefabFromCurrentAssetListPosition();
        }
        if (GUILayout.Button("Clear", GUILayout.Width(75)))
        {
            DI.FoodPrefab = null;
            DI.LoadFoodPrefab(DI.FoodPrefab);
            DI.RefreshDisplayElements();
        }
        lockPrefab = GUILayout.Toggle(lockPrefab, " Lock", GUILayout.Width(100));

        GUILayout.EndHorizontal();

        if (DI.FoodPrefab != null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(DI.FoodPrefab.name.Substring(1), StepHeadlineDisplay);
            GUILayout.EndHorizontal();
        }
        

        /*
        GUILayout.BeginHorizontal();
        //GUILayout.Label("Food:");
        DI.FoodPrefab = EditorGUILayout.ObjectField(DI.FoodPrefab, typeof(GameObject), true, GUILayout.Width(180)) as GameObject;
        if (GUILayout.Button("Load", GUILayout.Width(75)))
        {
            DI.LoadFoodPrefab(DI.FoodPrefab);
        }
        if (GUILayout.Button("Clear", GUILayout.Width(75)))
        {
            DI.FoodPrefab = null;
            DI.LoadFoodPrefab(DI.FoodPrefab);
        }
        GUILayout.EndHorizontal();
        */
    }

    void LoadFoodPrefabFromCurrentAssetListPosition() //used above in DisplayFoodPrefabSelector() to avoid duplicate code
    {
        string counterAssetPath = counterAssetPaths[assetIndex].Substring(counterAssetPaths[assetIndex].IndexOf("Counter"));
        string prefixStrippedPath = counterAssetPath.Substring(0, counterAssetPath.IndexOf('.'));
        DI.FoodPrefab = Resources.Load(prefixStrippedPath) as GameObject;
        DI.LoadFoodPrefab(DI.FoodPrefab);
    }

    void CreateCounterAssetList()
    {
        //http://answers.unity3d.com/questions/234935/how-do-i-enumerate-the-contents-of-an-asset-folder.html

        string sDataPath = Application.dataPath;
        string sFolderPath = sDataPath.Substring(0, sDataPath.Length - 6) + COUNTER_ASSET_PATH;
        string[] aFilePaths = System.IO.Directory.GetFiles(sFolderPath);

        List<string> fullFilePaths = new List<string>();

        foreach (string path in aFilePaths)
        {
            if (path.Contains(".meta"))
            {
                //discard this result.
                continue;
            }
            if (path.Contains("#") && path.Contains(".prefab")) //'#' is used as a prefix to identify our food prefabs
            {
                fullFilePaths.Add(path);
            }
        }

        
        List<string> counterAssetDisplayList = new List<string>();
        foreach (string path in fullFilePaths)
        {
            int poundIndex = path.LastIndexOf('#') + 1;
            string poundString = path.Substring(poundIndex);
            string outputString = poundString.Substring(0, poundString.IndexOf('.'));
            counterAssetDisplayList.Add(outputString);
        }
        fullFilePaths.Insert(0, "");
        counterAssetDisplayList.Insert(0, "(select a food item)");

        counterAssetPaths = fullFilePaths.ToArray();
        counterAssetDisplay = counterAssetDisplayList.ToArray();



    }

    // Advanced = shows variables that are useful when monitoring state changes, but are not necessary for level editing
    // Debug    = shows the default Unity display for all public variables (except static variables and ones with manual getters and setters)
    void DisplayDeveloperGUIs()
    {
        if (!DataPresent())
        {
            return;
        }

        GUILayout.BeginHorizontal();
        if (verboseMode)
        {
            GUILayout.Label("  Advanced  ", GUILayout.Width(75));
        }
        else
        {
            if (GUILayout.Button("Advanced", GUILayout.Width(75)))
            {
                verboseMode = true;
            }
        }

        if (debugMode)
        {
            GUILayout.Label("  Debug  ", GUILayout.Width(75));
        }
        else
        {
            if (GUILayout.Button("Debug", GUILayout.Width(75)))
            {
                debugMode = true;
            }
        }
        GUILayout.EndHorizontal();

        if (verboseMode)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("ADVANCED OPTIONS");
            if (GUILayout.Button("[X]"))
            {
                verboseMode = false;
            }
            GUILayout.EndHorizontal();

            DisplayImageToggles();
        }

        //debugMode = EditorGUILayout.Toggle("Debug Mode", debugMode);

        if (debugMode)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("DEBUG PARAMETERS");
            if (GUILayout.Button("[X]"))
            {
                debugMode = false;
            }
            GUILayout.EndHorizontal();

            base.OnInspectorGUI();
        }

    }

    // Used by Advanced/Developer GUI setting
    void DisplayImageToggles()
    {
        if (!DI.IsEmpty)
        {
            GUILayout.BeginHorizontal();
            DI.DisplayBorder = EditorGUILayout.Toggle("Border: ", DI.DisplayBorder);
            DI.DisplayBackground = EditorGUILayout.Toggle("Background: ", DI.DisplayBackground);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            DI.DisplayFood = EditorGUILayout.Toggle("Food: ", DI.DisplayFood);
            GUILayout.EndHorizontal();
        }

    }

    bool DataPresent()
    {
        if (DI.StepGameObjectReferences != null)
        {
            return (DI.StepGameObjectReferences.Count > 0);
        }
        else
        {
            return false;
        }
    }

    Rect GetRectSpaceForGUIDrawing(float canvasHeight, float widthPadding)
    {
        // Using this to get our current screen position so we can draw our own GUI elements
        Color guiColor = GUI.color;
        GUI.color = Color.clear;
        GUILayout.Box(FlatWhiteTexture, GUILayout.Height(canvasHeight), GUILayout.Width(Screen.width));
        GUI.color = guiColor;
        Rect canvas = GUILayoutUtility.GetLastRect();
        canvas.x += widthPadding;
        //canvas.y += padding;
        canvas.width -= widthPadding * 2;
        //canvas.height -= padding;
        return canvas;
    }

}
