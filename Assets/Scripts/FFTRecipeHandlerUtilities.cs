using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public static class FFTRecipeHandlerUtilities : System.Object {

    /// <summary>
    /// Saves a new FFT Recipe in XML format in the Assets directory under "Levels".
    /// </summary>
    /// <param name="RecipeMaker">Reference to the RecipeMaker.</param>
    /// <param name="Recipe">Reference to the Recipe data.</param>
    public static string SaveRecipeXML(ref FFTRecipeMaker RecipeMaker, ref FFTCounter Counter, ref FFTKitchen Kitchen)
    {
		if (!Application.isEditor)
		{
			Debug.Log("Saving is not supported from the main application, only the editor.");
			return "";
		}
		
        Debug.Log("Save Recipe");

        // update our dish list in case there have been manual changes
        Counter.RecipeCard.Dishes = new List<FFTDish>(RecipeMaker.gameObject.GetComponentsInChildren<FFTDish>());

        // formatting to prevent overwriting previously created recipes by accident
        string timeStampString;
        System.DateTime dtn = System.DateTime.Now;
        string dateString = dtn.Year.ToString().Substring(2) + "-" + dtn.Month.ToString("D2") + "-" + dtn.Day.ToString("D2");
        string currentTimeString = dtn.Hour.ToString("D2") + "." + dtn.Minute.ToString("D2") + "." + dtn.Second.ToString("D2");
        timeStampString = dateString + "_" + currentTimeString;

        string path = Application.dataPath + @"/Levels/" + Counter.RecipeCard.LevelTitle + "_" + timeStampString + "_" + Counter.RecipeCard.Author + @".FFTLevel" + @".xml";

        //Due to the timestamp, this should never actually delete the file, but just in case:
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }

        // Validate Recipe Data
        // Here we check to make sure all steps have a Destination, and that we do not have too many station slots 
        // (if we have 3 dishes, we should never have more than 3 of each slot). Also adds a slot if a station is referenced but no slot has been added.

        ValidateRecipeData(ref RecipeMaker, ref Counter, ref Kitchen);

        // Create XML Document

        XmlDocument levelXml = new XmlDocument();
        {
            XmlNode rootNode = levelXml.CreateElement("recipe");
            levelXml.AppendChild(rootNode);

            XmlNodeAppendAttribute(ref levelXml, ref rootNode, "title", Counter.RecipeCard.LevelTitle);

            XmlNode fileInfoNode = levelXml.CreateElement("info");
            rootNode.AppendChild(fileInfoNode);

            XmlNodeAppendAttribute(ref levelXml, ref fileInfoNode, "author", Counter.RecipeCard.Author);
            XmlNodeAppendAttribute(ref levelXml, ref fileInfoNode, "date", dateString);
            XmlNodeAppendAttribute(ref levelXml, ref fileInfoNode, "time", currentTimeString);

            XmlNode customerNode = levelXml.CreateElement("customer");
            rootNode.AppendChild(customerNode);
            XmlNodeAppendAttribute(ref levelXml, ref customerNode, "name", Counter.RecipeCard.Customer.ToString());

            XmlNode flavorTextNode = levelXml.CreateElement("flavorText");
            rootNode.AppendChild(flavorTextNode);
            XmlNodeAppendAttribute(ref levelXml, ref flavorTextNode, "text", Counter.RecipeCard.FlavorText);

            XmlNode freshnessNode = levelXml.CreateElement("freshness");
            rootNode.AppendChild(freshnessNode);
            XmlNodeAppendAttribute(ref levelXml, ref freshnessNode, "enabled", Counter.RecipeCard.FreshnessParameters.UseFreshness.ToString());
            XmlNodeAppendAttribute(ref levelXml, ref freshnessNode, "delay", Counter.RecipeCard.FreshnessParameters.DelayInSeconds.ToString());
            XmlNodeAppendAttribute(ref levelXml, ref freshnessNode, "decay", Counter.RecipeCard.FreshnessParameters.DecayTimeInSeconds.ToString());
            XmlNodeAppendAttribute(ref levelXml, ref freshnessNode, "starPenalty", Counter.RecipeCard.FreshnessParameters.StarPenaltyMaximum.ToString());

            XmlNode usesTimerNode = levelXml.CreateElement("timer");
            rootNode.AppendChild(usesTimerNode);
            XmlNodeAppendAttribute(ref levelXml, ref usesTimerNode, "usesTimer", RecipeMaker.TimedLevel.ToString());
            XmlNodeAppendAttribute(ref levelXml, ref usesTimerNode, "timeLimit", RecipeMaker.TimeLimit.ToString());

            XmlNode elapsedTimeNode = levelXml.CreateElement("elapsedTimeTweak");
            rootNode.AppendChild(elapsedTimeNode);
            XmlNodeAppendAttribute(ref levelXml, ref elapsedTimeNode, "multiplier", RecipeMaker.ElapsedTimeMultiplier.ToString());

            XmlNode kitchenStationCountNode = levelXml.CreateElement("stations");
            rootNode.AppendChild(kitchenStationCountNode);

            XmlNodeAppendAttribute(ref levelXml, ref kitchenStationCountNode, "chop", Kitchen.Stations[FFTStation.Type.Chop].SlotList.Count.ToString());
            XmlNodeAppendAttribute(ref levelXml, ref kitchenStationCountNode, "cook", Kitchen.Stations[FFTStation.Type.Cook].SlotList.Count.ToString());
            XmlNodeAppendAttribute(ref levelXml, ref kitchenStationCountNode, "prep", Kitchen.Stations[FFTStation.Type.Prep].SlotList.Count.ToString());
            XmlNodeAppendAttribute(ref levelXml, ref kitchenStationCountNode, "spice", Kitchen.Stations[FFTStation.Type.Spice].SlotList.Count.ToString());

            int currentDish = 1;

            foreach (FFTDish dish in Counter.RecipeCard.Dishes)
            {
                XmlNode dishNode = levelXml.CreateElement("dish");
                rootNode.AppendChild(dishNode);

                string dishAssetName = dish.FoodPrefab.name;

                XmlNodeAppendAttribute(ref levelXml, ref dishNode, "name", dishAssetName);
                string assetPath = dish.FoodPrefab.name + "_NoEditor";
#if UNITY_EDITOR
                
                if (Application.isEditor)
                    assetPath = UnityEditor.AssetDatabase.GetAssetPath(dish.FoodPrefab);
#endif

                XmlNodeAppendAttribute(ref levelXml, ref dishNode, "path", assetPath);
                XmlNodeAppendAttribute(ref levelXml, ref dishNode, "id", currentDish.ToString());

                int currentStep = 1;

                foreach (FFTStep step in dish.StepDataObjects)
                {
                    XmlNode stepNode = levelXml.CreateElement("step");
                    dishNode.AppendChild(stepNode);

                    // for backwards compatibility, id started with "1" instead of "0" in old format
                    XmlNodeAppendAttribute(ref levelXml, ref stepNode, "id", (step.Index + 1).ToString()); //currentStep.ToString() removed

                    XmlNodeAppendAttribute(ref levelXml, ref stepNode, "destination", step.Destination.ToString());
                    
                    XmlNodeAppendAttribute(ref levelXml, ref stepNode, "visualState", step.VisualStateIndex.ToString());

                    XmlNode gamePlayNode = levelXml.CreateElement("gameplay");
                    stepNode.AppendChild(gamePlayNode);
                    XmlNodeAppendAttribute(ref levelXml, ref gamePlayNode, "type", step.Gameplay.ToString());

                    XmlNode parametersNode = levelXml.CreateElement("parameters");
                    gamePlayNode.AppendChild(parametersNode);

                    //TODO: Change parameters based on the type of gameplay included
                    switch (step.Gameplay)
                    {
                        case FFTStation.GameplayType.ElapsedTime:
                            //XmlNodeAppendAttribute(ref levelXml, ref parametersNode, "position", "slot");
                            XmlNode timingParametersNode = levelXml.CreateElement("timing");
                            parametersNode.AppendChild(timingParametersNode);
                            XmlNode tweakableParametersNode = levelXml.CreateElement("tweakable");
                            timingParametersNode.AppendChild(tweakableParametersNode);
                            XmlNode readonlyParametersNode = levelXml.CreateElement("readonly");
                            timingParametersNode.AppendChild(readonlyParametersNode);

                            XmlNodeAppendAttribute(ref levelXml, ref tweakableParametersNode, "uncooked", step.Parameters.Uncooked.ToString());
                            if (step.Parameters.IsCookable)
                            {
                                XmlNodeAppendAttribute(ref levelXml, ref tweakableParametersNode, "cooked", step.Parameters.Cooked.ToString());
                            }
                            if (step.Parameters.IsBurnable)
                            {
                                XmlNodeAppendAttribute(ref levelXml, ref tweakableParametersNode, "burned", step.Parameters.Burned.ToString());
                            }
                            if (step.Parameters.UsesPeakFlavor)
                            {
                                XmlNodeAppendAttribute(ref levelXml, ref tweakableParametersNode, "peak", step.Parameters.PeakPercentage.ToString());
                            }

                            XmlNodeAppendAttribute(ref levelXml, ref readonlyParametersNode, "isCookable", step.Parameters.IsCookable.ToString());
                            XmlNodeAppendAttribute(ref levelXml, ref readonlyParametersNode, "isBurnable", step.Parameters.IsBurnable.ToString());
                            XmlNodeAppendAttribute(ref levelXml, ref readonlyParametersNode, "hasPeakFlavor", step.Parameters.UsesPeakFlavor.ToString());

                            break;
                        case FFTStation.GameplayType.MiniGame:
							//saveminigameparams
                            XmlNode mgTitleNode = levelXml.CreateElement(step.MinigameType.ToString());
                            parametersNode.AppendChild(mgTitleNode);
                            switch (step.MinigameType)
                            {
                                case MG_Minigame.Type.Placeholder:
                                    XmlNodeAppendAttribute(ref levelXml, ref mgTitleNode, "delayInSeconds", step.MinigameVariableContainer.Placeholder_DelayToWinInSeconds.ToString());
                                    XmlNodeAppendAttribute(ref levelXml, ref mgTitleNode, "useDelay", step.MinigameVariableContainer.Placeholder_UseDelay.ToString());
                                    break;
                                case MG_Minigame.Type.Sorting:
                                    XmlNodeAppendAttribute(ref levelXml, ref mgTitleNode, "numFoodItems", step.MinigameVariableContainer.Sorting_NumVegetables.ToString());
                                    break;
								case MG_Minigame.Type.Blending:
									XmlNodeAppendAttribute(ref levelXml, ref mgTitleNode, "numIngredients", step.MinigameVariableContainer.Blending_NumIngredients.ToString());
                                    break;
                                case MG_Minigame.Type.Stirring:
									XmlNodeAppendAttribute(ref levelXml, ref mgTitleNode, "numVegetables", step.MinigameVariableContainer.Stirring_NumberOfVegetables.ToString());
									XmlNodeAppendAttribute(ref levelXml, ref mgTitleNode, "totalTimeToFinish", step.MinigameVariableContainer.Stirring_TotalCookingTime.ToString());
									XmlNodeAppendAttribute(ref levelXml, ref mgTitleNode, "timeBeforeStir", step.MinigameVariableContainer.Stirring_TimeBeforeNeedingToStir.ToString());
									XmlNodeAppendAttribute(ref levelXml, ref mgTitleNode, "timeBeforeDanger", step.MinigameVariableContainer.Stirring_TimeBeforeEnteringDangerZone.ToString());
									XmlNodeAppendAttribute(ref levelXml, ref mgTitleNode, "timeBeforeBurnt", step.MinigameVariableContainer.Stirring_TimeBeforeFullyBurned.ToString());
									XmlNodeAppendAttribute(ref levelXml, ref mgTitleNode, "burnRecovery", step.MinigameVariableContainer.Stirring_BurnRecoveryMultiplier.ToString());
                                    break;
                                case MG_Minigame.Type.Spicing:
									XmlNodeAppendAttribute(ref levelXml, ref mgTitleNode, "numBottles", step.MinigameVariableContainer.Spicing_NumberOfBottles.ToString());
                                    break;
                                case MG_Minigame.Type.Chopping:
									XmlNodeAppendAttribute(ref levelXml, ref mgTitleNode, "gameMode", step.MinigameVariableContainer.Chopping_GameMode.ToString());
									XmlNodeAppendAttribute(ref levelXml, ref mgTitleNode, "numCuts", step.MinigameVariableContainer.Chopping_MaxCuts.ToString());
                                    break;
                                //TODO_INSERT
                            }
                            //XmlNodeAppendAttribute(ref levelXml, ref parametersNode, "position", "screen");
                            break;
                        case FFTStation.GameplayType.Empty:
                            //XmlNodeAppendAttribute(ref levelXml, ref parametersNode, "position", "nil");
                            break;
                    }

                    currentStep++;
                }

                currentDish++;
            }

        }

        levelXml.Save(path);
#if UNITY_EDITOR
        if (Application.isEditor)
            UnityEditor.AssetDatabase.Refresh();
#endif
        return path;

    }
	
    /// <summary>
    /// Loads a XML FFT Recipe and creates a new GameObject containing the Recipe data and food objects.
    /// </summary>
    /// <remarks>Does not create the RecipeMaker object for further editing. To do so, pass "true" into the second optional parameter.</remarks>
    /// <param name="levelPath">Path to the XML file containing the level data.</param>
    public static GameObject LoadRecipeXML(TextAsset levelPath)
    {
        return LoadRecipeXML(levelPath, false);
    }
	
	public static GameObject LoadRecipeXML(TextAsset levelPath, bool keepRecipeMakerInNewLoadedRecipe)
	{
		
        if (levelPath != null && levelPath.name.Contains("FFTLevel"))
        {
            Debug.Log("FFTRecipeHandlerUtilities :: Load Recipe");
			
			XmlDocument level = new XmlDocument();
            level.LoadXml(levelPath.text);
		
			return LoadRecipeXML(ref level, keepRecipeMakerInNewLoadedRecipe, levelPath);
		}

        Debug.Log("FFTRecipeHandlerUtilities :: Recipe Load Failed");
        return null;
	}

	public static GameObject LoadRecipeXML(string xmlLevelPath, bool keepRecipeMakerInNewLoadedRecipe)
	{
		if (xmlLevelPath.Contains("FFTLevel"))
        {

            Debug.Log("FFTRecipeHandlerUtilities :: Load Recipe :: Standalone");
			
			//TODO
			//load xml file to string here before passing to recipeLoader without levelPath TextAsset (null)
			
			XmlDocument level = new XmlDocument();
			level.Load(xmlLevelPath);
		
			return LoadRecipeXML(ref level, keepRecipeMakerInNewLoadedRecipe, null);
		}

        Debug.Log("FFTRecipeHandlerUtilities :: Recipe Load Failed :: Not an FFTLevel");
        return null;
		
	}
	

    /// <summary>
    /// Loads a XML FFT Recipe and creates a new GameObject containing the Recipe data and food objects.
    /// </summary>
    /// <param name="levelPath">Path to the XML file containing the level data.</param>
    /// <param name="keepRecipeMakerInNewLoadedRecipe">Optional flag to add the RecipeMaker for further editing and saving of the level.</param>
    public static GameObject LoadRecipeXML(ref XmlDocument level, bool keepRecipeMakerInNewLoadedRecipe, TextAsset levelPath)
    {
            // creating a new one from scratch first

            GameObject newRoot = GameObject.Instantiate(Resources.Load("MainGamePrefabs/CanvasBase", typeof(GameObject)) as GameObject) as GameObject;
            newRoot.name = "Canvas";

            GameObject newCounter = GameObject.Instantiate(Resources.Load("MainGamePrefabs/Counter", typeof(GameObject)) as GameObject) as GameObject;
            GameObject newKitchen = GameObject.Instantiate(Resources.Load("MainGamePrefabs/KitchenBase", typeof(GameObject)) as GameObject) as GameObject;
            newCounter.transform.parent = newRoot.transform;
            newKitchen.transform.parent = newRoot.transform;
            newCounter.tag = "Counter";
            newKitchen.tag = "Kitchen";
            
            FFTRecipeMaker recipeMakerScript = newRoot.AddComponent<FFTRecipeMaker>();
            FFTCounter counterScript = newCounter.AddComponent<FFTCounter>();
            counterScript.RecipeCard = new FFTRecipe();
            FFTKitchen kitchenScript = newKitchen.AddComponent<FFTKitchen>();
            kitchenScript.InitializeStations();

            if (level.HasChildNodes)
            {
                XmlNode rootNode = level.FirstChild;

                foreach (XmlAttribute attribute in rootNode.Attributes)
                {
                    switch (attribute.Name)
                    {
                        case "title":
                            counterScript.RecipeCard.LevelTitle = attribute.Value;
                            break;
                    }
                }

                if (rootNode.HasChildNodes)
                {
                    // clear the dish list, as we are populating it with new values.
                    counterScript.RecipeCard.Dishes = new List<FFTDish>();
                    foreach (XmlNode dishNode in rootNode.ChildNodes)
                    {
                        switch (dishNode.Name)
                        {
                            case "info":
                                foreach (XmlAttribute attribute in dishNode.Attributes)
                                {
                                    switch (attribute.Name)
                                    {
                                        case "author":
                                            counterScript.RecipeCard.Author = attribute.Value;
                                            break;
                                        case "date":
                                            //TODO: Date parse.
                                            break;
                                        case "time":
                                            //TODO: Time parse.
                                            break;
                                    }
                                }
                                break;
                            case "customer":
                                foreach (XmlAttribute attribute in dishNode.Attributes)
                                {
                                    switch (attribute.Name)
                                    {
                                        case "name":
                                            if (System.Enum.IsDefined(typeof(FFTCustomer.Name), attribute.Value))
                                            {
                                                Debug.Log("Got FFTCustomer Name on Load");
                                                counterScript.RecipeCard.Customer = (FFTCustomer.Name)System.Enum.Parse(typeof(FFTCustomer.Name), attribute.Value);
                                            }
                                            break;
                                    }
                                }
                                break;
                            case "flavorText":
                                foreach (XmlAttribute attribute in dishNode.Attributes)
                                {
                                    switch (attribute.Name)
                                    {
                                        case "text":
                                            counterScript.RecipeCard.FlavorText = attribute.Value;
                                            break;
                                    }
                                }
                                break;
                            case "freshness":
                                foreach (XmlAttribute attribute in dishNode.Attributes)
                                {
                                    switch (attribute.Name)
                                    {
                                        case "enabled":
                                            counterScript.RecipeCard.FreshnessParameters.UseFreshness = bool.Parse(attribute.Value);
                                            break;
                                        case "delay":
                                            counterScript.RecipeCard.FreshnessParameters.DelayInSeconds = float.Parse(attribute.Value);
                                            break;
                                        case "decay":
                                            counterScript.RecipeCard.FreshnessParameters.DecayTimeInSeconds = float.Parse(attribute.Value);
                                            break;
                                        case "starPenalty":
                                            counterScript.RecipeCard.FreshnessParameters.StarPenaltyMaximum = float.Parse(attribute.Value);
                                            break;
                                    }
                                }
                                break;
                            case "timer":
                                foreach (XmlAttribute attribute in dishNode.Attributes)
                                {
                                    switch (attribute.Name)
                                    {
                                        case "usesTimer":
                                            recipeMakerScript.TimedLevel = bool.Parse(attribute.Value);
                                            break;
                                        case "timeLimit":
                                            recipeMakerScript.TimeLimit = float.Parse(attribute.Value);
                                            break;
                                    }
                                }
                                break;
                            case "elapsedTimeTweak":
                                foreach (XmlAttribute attribute in dishNode.Attributes)
                                {
                                    switch (attribute.Name)
                                    {
                                        case "multiplier":
                                            recipeMakerScript.ElapsedTimeMultiplier = Mathf.Clamp(float.Parse(attribute.Value), 0.1f, 10f);
                                            break;
                                    }
                                }
                                break;

                            case "stations":
                                foreach (XmlAttribute attribute in dishNode.Attributes)
                                {
                                    int count;

                                    switch (attribute.Name)
                                    {
                                        case "chop":
                                            if (System.Int32.TryParse(attribute.Value, out count))
                                            {
                                                kitchenScript.Stations[FFTStation.Type.Chop].AddSlots(count);
                                            }
                                            break;
                                        case "cook":
                                            if (System.Int32.TryParse(attribute.Value, out count))
                                            {
                                                kitchenScript.Stations[FFTStation.Type.Cook].AddSlots(count);
                                            }
                                            break;
                                        case "prep":
                                            if (System.Int32.TryParse(attribute.Value, out count))
                                            {
                                                kitchenScript.Stations[FFTStation.Type.Prep].AddSlots(count);
                                            }
                                            break;
                                        case "spice":
                                            if (System.Int32.TryParse(attribute.Value, out count))
                                            {
                                                kitchenScript.Stations[FFTStation.Type.Spice].AddSlots(count);
                                            }
                                            break;
                                    }
                                }
                                kitchenScript.UpdateKitchenName();
                                break;
                            case "dish":
                                //Create FFTDishImporter objects, populate fields.
                                if (counterScript.AddSlot(false) == false)
                                {
                                    Debug.Log("Maximum Dishes Exceeded");
                                    break;
                                }

                                GameObject dishGameObject = new GameObject();
                                Vector3 slotPosition = counterScript.LastSlot.transform.position;
                                slotPosition.z -= FFTCounter.DishSpacing;
                                dishGameObject.transform.localPosition = slotPosition;
                                dishGameObject.transform.parent = newCounter.transform;
                                FFTDish dish = dishGameObject.AddComponent<FFTDish>();

                                foreach (XmlAttribute attribute in dishNode.Attributes)
                                {
                                    switch (attribute.Name)
                                    {
										// We don't use path or ID anymore.  We look up by name (in texture atlas).
                                        case "name":
                                            dishGameObject.name = attribute.Value + @" (Temp)";
                                            break;
                                        case "path":

                                            string path = attribute.Value.Substring(17); //remove "Assets/Resources/"
                                            if (path.Contains(".prefab"))
                                            {
                                                path = path.Substring(0, path.IndexOf(".prefab"));
                                                Debug.Log(path);
                                                dish.FoodPrefab = Resources.Load(path, typeof(GameObject)) as GameObject;
                                            }
								
											//rem levelPath
											if (levelPath != null)
											{
	                                            recipeMakerScript.LevelPath = levelPath;
	                                            counterScript.RecipeCard.LevelPath = levelPath;
											}

                                            //GameObject foodPrefab = Resources.Load(path, typeof(GameObject)) as GameObject;

                                            //dish.FoodPrefab = Resources.Load(path, typeof(GameObject)) as GameObject;
                                            break;
                                        case "id":
                                            dish.ID = System.Int32.Parse(attribute.Value);
                                            break;
                                    }
                                }

                                dish.LoadErrorMessages.AddRange(dish.LoadFoodPrefab());

                                if (dishNode.HasChildNodes)
                                {
                                    int i = 0;

                                    dish.gameObject.tag = "Dish";
                                    dish.Border.transform.localPosition = new Vector3(0, 0, 1);

                                    dish.StepDataObjects = new List<FFTStep>();
                                    dish.StepGameObjectReferences = new List<GameObject>();

                                    foreach (XmlNode stepNode in dishNode.ChildNodes)
                                    {
                                        //Add a new step every time, since we are altering the default values (to avoid errors when you use fewer steps than visual states from legacy cruft code
                                        FFTStep newStep = new FFTStep();
                                        dish.StepDataObjects.Add(newStep);
                                        dish.StepGameObjectReferences.Add(newStep.StepObject);

                                        /*
                                        //TODO: NOW: alter each step node

                                        if (i > dish.StepDataObjects.Count - 1) //add additional steps if needed (reusing visual states)
                                        {
                                            FFTStep newStep = new FFTStep();
                                            dish.StepDataObjects.Add(newStep);
                                            dish.StepGameObjectReferences.Add(newStep.StepObject);
                                        }
                                        */

                                        FFTStep currentStep = dish.StepDataObjects[i];

                                        if (stepNode.Attributes.GetNamedItem("destination") != null)
                                            currentStep.Destination = FFTStation.TypeFromString(stepNode.Attributes["destination"].Value);
                                        else
                                            Debug.Log("Error in parsing XML (No Step Destination Error), Reload XML");

                                        if (stepNode.Attributes.GetNamedItem("id") != null)
                                        {
                                            currentStep.Index = System.Int32.Parse(stepNode.Attributes["id"].Value) - 1; // for backwards compatibility, id started with "1" instead of "0" in old format
                                        }

                                        if (stepNode.Attributes.GetNamedItem("visualState") != null)
                                        {
                                            if (currentStep.Index == 0)
                                            {
                                                currentStep.AssignGameObject(dish.FoodVisualStates[0]); //assign a reference to the first visual state so we can turn it off if needed
                                                currentStep.SetVisibility(false); // by default we see the first step, so disable this view before changing it
                                            }
                                            
                                            currentStep.VisualStateIndex = System.Int32.Parse(stepNode.Attributes["visualState"].Value);
                                            currentStep.AssignGameObject(dish.FoodVisualStates[currentStep.VisualStateIndex]);

                                            if (currentStep.Index == 0)
                                                currentStep.SetVisibility(true); // now switch the first step's object back on
                                        }
                                        else
                                        {
                                            currentStep.VisualStateIndex = currentStep.Index;
                                            // no visibility tweaked as this is/was the default behavior

                                            if (currentStep.Index == 0)
                                                currentStep.SetVisibility(true); // now switch the first step's object back on
                                        }
                                        
                                        XmlNode gameplayNode = stepNode.FirstChild;
                                        currentStep.StepObject.transform.localPosition = new Vector3();

                                        switch (Station.GameplayTypeFromString(gameplayNode.Attributes["type"].Value))
                                        {
                                            case Station.GameplayType.ElapsedTime:
                                                currentStep.Gameplay = FFTStation.GameplayType.ElapsedTime;
                                                XmlNode timingParametersNode = gameplayNode.FirstChild;
                                                XmlNode timingNode = timingParametersNode.FirstChild;
                                                foreach (XmlNode node in timingNode.ChildNodes)
                                                {
                                                    switch (node.Name)
                                                    {
                                                        case "tweakable":
                                                            foreach (XmlAttribute attribute in node.Attributes)
                                                            {
                                                                switch (attribute.Name)
                                                                {
                                                                    case "uncooked":
                                                                        currentStep.Parameters.Uncooked = float.Parse(attribute.Value) * recipeMakerScript.ElapsedTimeMultiplier;
                                                                        break;
                                                                    case "cooked":
                                                                        currentStep.Parameters.Cooked = float.Parse(attribute.Value) * recipeMakerScript.ElapsedTimeMultiplier;
                                                                        //currentStep.Parameters.IsCookable = true;
                                                                        break;
                                                                    case "burned":
                                                                        currentStep.Parameters.Burned = float.Parse(attribute.Value) * recipeMakerScript.ElapsedTimeMultiplier;
                                                                        //currentStep.Parameters.IsBurnable = true;
                                                                        break;
                                                                    case "peak":
                                                                        currentStep.Parameters.PeakPercentage = System.Int32.Parse(attribute.Value);
                                                                        //currentStep.Parameters.UsesPeakFlavor = true;
                                                                        break;
                                                                }
                                                            }
                                                            break;
                                                        case "readonly":
                                                            foreach (XmlAttribute attribute in node.Attributes)
                                                            {
                                                                switch (attribute.Name)
                                                                {
                                                                    case "isCookable":
                                                                        currentStep.Parameters.IsCookable = bool.Parse(attribute.Value);
                                                                        break;
                                                                    case "isBurnable":
                                                                        currentStep.Parameters.IsBurnable = bool.Parse(attribute.Value);
                                                                        break;
                                                                    case "hasPeakFlavor":
                                                                        currentStep.Parameters.UsesPeakFlavor = bool.Parse(attribute.Value);
                                                                        break;
                                                                }
                                                            }
                                                            break;
                                                    }
                                                }

                                                break;
                                            case Station.GameplayType.MiniGame:
                                                currentStep.Gameplay = FFTStation.GameplayType.MiniGame;
                                                XmlNode minigameParametersNode = gameplayNode.FirstChild;
                                                XmlNode minigameNode = minigameParametersNode.FirstChild;
                                                MG_Minigame.Type gameType = MG_Minigame.MinigameTypeFromString(minigameNode.Name);
                                                currentStep.MinigameType = gameType;
                                                currentStep.MinigameVariableContainer.CurrentType = currentStep.MinigameType;

                                                switch (gameType)
                                                {
													//loadminigameparams
                                                    case MG_Minigame.Type.Placeholder:
                                                        foreach (XmlAttribute attribute in minigameNode.Attributes)
                                                        {
                                                            switch (attribute.Name)
                                                            {
                                                                case "delayInSeconds":
                                                                    currentStep.MinigameVariableContainer.Placeholder_DelayToWinInSeconds = float.Parse(attribute.Value);
                                                                    break;
                                                                case "useDelay":
                                                                    currentStep.MinigameVariableContainer.Placeholder_UseDelay = bool.Parse(attribute.Value);
                                                                    break;
                                                            }
                                                        }
                                                        break;
                                                    case MG_Minigame.Type.Sorting: //TODO: Visual object edit
                                                        foreach (XmlAttribute attribute in minigameNode.Attributes)
                                                        {
                                                            switch (attribute.Name)
                                                            {
                                                                case "numFoodItems":
                                                                    currentStep.MinigameVariableContainer.Sorting_NumVegetables = int.Parse(attribute.Value);
                                                                    break;
                                                            }
                                                        }
                                                        break;
                                                    case MG_Minigame.Type.Stirring: //TODO: Visual object edit
                                                        foreach (XmlAttribute attribute in minigameNode.Attributes)
                                                        {
															/*
															 *  dict.Add("NumVegetables", Stirring_NumberOfVegetables.ToString());
																dict.Add("TotalTimeToCook", Stirring_TotalCookingTime.ToString());
																dict.Add("TimeBeforeStir", Stirring_TimeBeforeNeedingToStir.ToString());
																dict.Add("TimeBeforeDanger", Stirring_TimeBeforeEnteringDangerZone.ToString());
																dict.Add("TimeBeforeBurn", Stirring_TimeBeforeFullyBurned.ToString());
																dict.Add("BurnRecoveryRate", Stirring_BurnRecoveryMultiplier.ToString());
															 */
                                                            switch (attribute.Name)
                                                            {
																case "numVegetables":
                                                                    currentStep.MinigameVariableContainer.Stirring_NumberOfVegetables = int.Parse(attribute.Value);
                                                                    break;
																case "totalTimeToFinish":
                                                                    currentStep.MinigameVariableContainer.Stirring_TotalCookingTime = float.Parse(attribute.Value);
                                                                    break;
																case "timeBeforeStir":
																	currentStep.MinigameVariableContainer.Stirring_TimeBeforeNeedingToStir = float.Parse(attribute.Value);
																	break;
																case "timeBeforeDanger":
																	currentStep.MinigameVariableContainer.Stirring_TimeBeforeEnteringDangerZone = float.Parse(attribute.Value);
																	break;
																case "timeBeforeBurnt":
																	currentStep.MinigameVariableContainer.Stirring_TimeBeforeFullyBurned = float.Parse(attribute.Value);
																	break;
																case "burnRecovery":
																	currentStep.MinigameVariableContainer.Stirring_BurnRecoveryMultiplier = float.Parse(attribute.Value);
																	break;
                                                                default:
                                                                    break;
                                                            }
                                                        }
                                                        break;
													case MG_Minigame.Type.Blending: //TODO: More params? Softness? Random ranges?
                                                        foreach (XmlAttribute attribute in minigameNode.Attributes)
                                                        {
                                                            switch (attribute.Name)
                                                            {
                                                                case "numIngredients":
                                                                    currentStep.MinigameVariableContainer.Blending_NumIngredients = int.Parse(attribute.Value);
                                                                    break;
                                                            }
                                                        }
                                                        break;
                                                    case MG_Minigame.Type.Spicing: 
                                                        foreach (XmlAttribute attribute in minigameNode.Attributes)
                                                        {
                                                            switch (attribute.Name)
                                                            {
																case "numBottles":
																	currentStep.MinigameVariableContainer.Spicing_NumberOfBottles = int.Parse(attribute.Value);
																	break;
                                                                default:
                                                                    break;
                                                            }
                                                        }
                                                        break;
                                                    case MG_Minigame.Type.Chopping: 
                                                        foreach (XmlAttribute attribute in minigameNode.Attributes)
                                                        {
                                                            switch (attribute.Name)
                                                            {
																case "numCuts":
																	currentStep.MinigameVariableContainer.Chopping_MaxCuts = int.Parse(attribute.Value);
																	break;
																case "gameMode":
																	currentStep.MinigameVariableContainer.Chopping_GameMode = MG2_RootScript.StringToGameMode(attribute.Value);
																	break;
                                                                default:
                                                                    break;
                                                            }
                                                        }
                                                        break;


                                                        //TODO_INSERT
                                                    //insert other cases for other MG_Minigame.Types
                                                }
                                                break;
                                            case Station.GameplayType.Empty:
                                                break;

                                        }

                                        dish.StepDataObjects[i] = currentStep;
                                        dish.StepGameObjectReferences[i] = currentStep.StepObject;

                                        i++;
									}
								
								if (i < dish.StepDataObjects.Count - 1)
								{
									dish.StepDataObjects.RemoveRange(i, dish.StepDataObjects.Count - i);
									dish.StepGameObjectReferences.RemoveRange(i, dish.StepGameObjectReferences.Count - i);
								}
								
                                }
                                dish.FreshnessMeterParameters = counterScript.RecipeCard.FreshnessParameters;
                                counterScript.RecipeCard.Dishes.Add(dish);
                                //Assign the dish we just created to the slot (since we are not creating a new empty dish here)
                                counterScript.SlotList[counterScript.SlotList.Count - 1].Dish = dish;
                                Debug.Log("Slot Dish added.");

                                break;
                        }

                    }
                }

            }
			//rem levelPath
            newCounter.name = "Counter - " + counterScript.RecipeCard.LevelTitle; //levelPath.name.Substring(0, levelPath.name.IndexOf('_'));
			
			//update freshness and time limit based on recipeMakerScript.ElapsedTimeMultiplier
		
			if (recipeMakerScript.TimedLevel)
			{
				recipeMakerScript.TimeLimit = recipeMakerScript.TimeLimit * recipeMakerScript.ElapsedTimeMultiplier;
			}
			if (counterScript.RecipeCard.FreshnessParameters.UseFreshness)
			{
				counterScript.RecipeCard.FreshnessParameters.DecayTimeInSeconds *= recipeMakerScript.ElapsedTimeMultiplier;
				counterScript.RecipeCard.FreshnessParameters.DelayInSeconds *= recipeMakerScript.ElapsedTimeMultiplier;
			}
			
            //counterScript.RecipeCard = recipeMakerScript.RecipeCard;

            if (!keepRecipeMakerInNewLoadedRecipe)
            {
                FFTUtilities.DestroySafe(recipeMakerScript); //newRecipe.GetComponent<FFTRecipeMaker>()
                Debug.Log("FFTRecipeHandlerUtilities :: RecipeMaker Discarded.");
            }

            Debug.Log("FFTRecipeHandlerUtilities :: Success");

            return newRoot;
    }

    /// <summary>
    /// Verifies that recipe data conforms to specs. 
    /// </summary>
    /// <param name="RecipeMaker"></param>
    /// <param name="Counter"></param>
    /// <param name="Kitchen"></param>
    public static void ValidateRecipeData(ref FFTRecipeMaker RecipeMaker, ref FFTCounter Counter, ref FFTKitchen Kitchen)
    {
        RecipeMaker.ElapsedTimeMultiplier = Mathf.Clamp(RecipeMaker.ElapsedTimeMultiplier, 0.1f, 10f);

        int chopSlots = 0;
        int cookSlots = 0;
        int prepSlots = 0;
        int spiceSlots = 0;

        List<FFTDish> emptyDishes = new List<FFTDish>();

        foreach (FFTDish dish in Counter.RecipeCard.Dishes)
        {
            if (dish.FoodPrefab == null)
            {
                Debug.Log("Validation Err: Dish " + dish.ID + " has no prefab assigned. Deleting...");
                emptyDishes.Add(dish);
                continue;
            }

            bool hasChop = false;
            bool hasCook = false;
            bool hasPrep = false;
            bool hasSpice = false;

            foreach (FFTStep step in dish.StepDataObjects)
            {
                switch (step.Destination)
                {
                    case FFTStation.Type.Chop:
                        hasChop = true;
                        break;
                    case FFTStation.Type.Cook:
                        hasCook = true;
                        break;
                    case FFTStation.Type.Prep:
                        hasPrep = true;
                        break;
                    case FFTStation.Type.Spice:
                        hasSpice = true;
                        break;
                    default:
                        step.Destination = FFTStation.Type.Chop;
                        Debug.Log("Validation Err: Dish " + dish.ID + " Step " + step.Index + "has no Destination defined. Assigning Chop.");
                        hasChop = true;
                        break;
                }
            }

            if (hasChop)
                chopSlots++;
            if (hasCook)
                cookSlots++;
            if (hasPrep)
                prepSlots++;
            if (hasSpice)
                spiceSlots++;
        }

        ValidateStationSlotCount(chopSlots, FFTStation.Type.Chop, ref Kitchen);
        ValidateStationSlotCount(cookSlots, FFTStation.Type.Cook, ref Kitchen);
        ValidateStationSlotCount(prepSlots, FFTStation.Type.Prep, ref Kitchen);
        ValidateStationSlotCount(spiceSlots, FFTStation.Type.Spice, ref Kitchen);

        foreach (FFTDish dish in emptyDishes)
        {
            Counter.RecipeCard.Dishes.Remove(dish);
            FFTUtilities.DestroySafe(dish.gameObject);
        }

    }

    static void ValidateStationSlotCount(int maxSlotsNeeded, FFTStation.Type stationType, ref FFTKitchen Kitchen)
    {
        // if we have more slots than dishes that use that slot, prune the slot list down to the max needed.
        if (maxSlotsNeeded < Kitchen.Stations[stationType].SlotList.Count)
        {
            Debug.Log("Validation Err: Recipe only needs " + maxSlotsNeeded + " slots of " + stationType.ToString() + " but currently has " + Kitchen.Stations[stationType].SlotList.Count + ".");
            while (maxSlotsNeeded < Kitchen.Stations[stationType].SlotList.Count)
            {
                Kitchen.Stations[stationType].RemoveSlot();
            }
        }
        // if we need at least one slot of a station type, but none exist, add one.
        if (maxSlotsNeeded > 0 && Kitchen.Stations[stationType].SlotList.Count == 0)
        {
            Debug.Log("Validation Err: Recipe needs at least one slot of " + stationType.ToString() + " but none exist. Adding one.");
            Kitchen.Stations[stationType].AddSlot();
        }

    }

    /// <summary>
    /// Creates a new XML child node with a passed in name and value.
    /// </summary>
    /// <param name="xmldoc">Parent XML Document</param>
    /// <param name="parentNode">Parent XML Node that will be given the new child.</param>
    /// <param name="name">Name/Label of the new XML Node.</param>
    /// <param name="value">Value of the new XML Node.</param>
    private static void XmlNodeNewChildWithInnerText(ref XmlDocument xmldoc, ref XmlNode parentNode, string name, string value)
    {
        XmlNode newNode = xmldoc.CreateElement(name);
        newNode.InnerText = value;
        parentNode.AppendChild(newNode);
    }

    /// <summary>
    /// Appends a new XML Attribute to an existing XML Node.
    /// </summary>
    /// <param name="xmldoc">Parent XML Document</param>
    /// <param name="node">XML Node to append the XML Attribute to.</param>
    /// <param name="name">Name/Label of the new XML Attribute.</param>
    /// <param name="value">Value of the new XML Attribute.</param>
    private static void XmlNodeAppendAttribute(ref XmlDocument xmldoc, ref XmlNode node, string name, string value)
    {
        XmlAttribute attribute = xmldoc.CreateAttribute(name);
        attribute.Value = value;
        node.Attributes.Append(attribute);
    }

    /// <summary>
    /// Saves a new participant log for the Levy 1st year study (LL1X). Should be called at the end of each played level.
    /// </summary>
    public static string SaveLevyParticipantLogXML()
    {
        Debug.Log("Save Log");

        // formatting to prevent overwriting previously created recipes by accident
        string timeStampString;
        System.DateTime dtn = System.DateTime.Now;
        string dateString = dtn.Year.ToString().Substring(2) + "-" + dtn.Month.ToString("D2") + "-" + dtn.Day.ToString("D2");
        string currentTimeString = dtn.Hour.ToString("D2") + "." + dtn.Minute.ToString("D2"); // +"." + dtn.Second.ToString("D2");
        
        timeStampString = dateString + "_" + currentTimeString;

        int participantID = FFTGameManager.Instance.CurrentLevel.ParticipantID;
        string experimentalCondition = FFTGameManager.Instance.CurrentLevel.Condition.ToString();
        int sessionID = FFTGameManager.Instance.CurrentLevel.SessionID; ;

        //prefix for logs from FFTLevel Information
        string participantString = "";
        participantString += participantID.ToString() + "-";
        participantString += experimentalCondition + "-";
        participantString += sessionID.ToString();

        string directory = Application.dataPath + @"/ParticipantLogs/";

        string path = directory + timeStampString + "_" + participantString + @".LL1X" + @".xml";

        if (!System.IO.Directory.Exists(directory))
        {
            System.IO.Directory.CreateDirectory(directory);
        }

        //Due to the timestamp, this should never actually delete the file, but just in case:
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }

        // Create XML Document

        XmlDocument levelXml = new XmlDocument();
        {
            XmlNode rootNode = levelXml.CreateElement("participant");
            levelXml.AppendChild(rootNode);

            XmlNodeAppendAttribute(ref levelXml, ref rootNode, "id", participantID.ToString());
            XmlNodeAppendAttribute(ref levelXml, ref rootNode, "session", sessionID.ToString());
            XmlNodeAppendAttribute(ref levelXml, ref rootNode, "condition", experimentalCondition);


            XmlNode timeNode = levelXml.CreateElement("time");
            rootNode.AppendChild(timeNode);

            XmlNodeAppendAttribute(ref levelXml, ref timeNode, "totalElapsedTime", FFTGameManager.Instance.LevelGameplayElapsedTime.ToString());
            XmlNodeAppendAttribute(ref levelXml, ref timeNode, "isTimedLevel", FFTGameManager.Instance.CurrentLevel.TimedLevel.ToString());
            XmlNodeAppendAttribute(ref levelXml, ref timeNode, "timeLimit", FFTGameManager.Instance.CurrentLevel.TimedLevel.ToString());

            XmlNode scoreNode = levelXml.CreateElement("score");
            rootNode.AppendChild(scoreNode);

            XmlNodeAppendAttribute(ref levelXml, ref scoreNode, "totalStars", FFTGameManager.Instance.CurrentScore.RecipeTotalStarRating().ToString());
            //XmlNodeAppendAttribute(ref levelXml, ref scoreNode, "levelName", FFTGameManager.Instance.CurrentLevel.LevelSource.name);
            XmlNodeAppendAttribute(ref levelXml, ref scoreNode, "levelName", FFTGameManager.Instance.CurrentLevel.GetCurrentLevelSource().name);

            XmlNode detailNode = levelXml.CreateElement("detail");
            scoreNode.AppendChild(detailNode);

            foreach (KeyValuePair<string, List<FFTStepReport>> pair in FFTGameManager.Instance.CurrentScore.Dict)
            {
                XmlNode dishNode = levelXml.CreateElement("dish");
                XmlNodeAppendAttribute(ref levelXml, ref dishNode, "name", pair.Key);
                detailNode.AppendChild(dishNode);
                int i = 1;
                List<FFTStepReport> reports = pair.Value;
                foreach (FFTStepReport report in reports)
                {
                    XmlNode stepNode = levelXml.CreateElement("step");
                    XmlNodeAppendAttribute(ref levelXml, ref stepNode, "id", i.ToString());
                    XmlNodeAppendAttribute(ref levelXml, ref stepNode, "stars", report.StarRating.ToString());
                    XmlNodeAppendAttribute(ref levelXml, ref stepNode, "feedback", report.Feedback.ToString());
                    dishNode.AppendChild(stepNode);
                    i++;
                }
            }
        }

        levelXml.Save(path);
#if UNITY_EDITOR        
        if (Application.isEditor) 
            UnityEditor.AssetDatabase.Refresh();
#endif
        return path;

    }
	

    /// <summary>
    /// Saves a new participant log for THE STUDY (NSFX). Should be called at the end of each played level.
    /// </summary>
    public static string SaveCogGameParticipantLog()
    {
        Debug.Log("Save Log");

        // formatting to prevent overwriting previously created recipes by accident
        string timeStampString;
        System.DateTime dtn = System.DateTime.Now;
        string dateString = dtn.Year.ToString().Substring(2) + "-" + dtn.Month.ToString("D2") + "-" + dtn.Day.ToString("D2");
        string currentTimeString = dtn.Hour.ToString("D2") + "." + dtn.Minute.ToString("D2") +"." + dtn.Second.ToString("D2");
		
		string levelProgressionID = FFTGameManager.Instance.LevelsAttempted.ToString("D3");
		string levelTitle = FFTGameManager.Instance.CurrentLevel.TitleofLevel;
        
        timeStampString = dateString + "_" + currentTimeString;

        int participantID = FFTGameManager.Instance.CurrentLevel.ParticipantID;
        string experimentalCondition = FFTGameManager.Instance.CurrentLevel.ParticipantGroup.ToString();
        int sessionID = FFTGameManager.Instance.CurrentLevel.SessionID;

        //prefix for logs from FFTLevel Information
        string participantString = "";
        participantString += participantID.ToString() + "-";
        participantString += experimentalCondition + "-";
        participantString += sessionID.ToString("D3");
		
		string directory, path;
		
		directory = Application.dataPath;
		
		//Debug.Log(System.IO.Directory.GetParent(Application.dataPath).ToString());
		
		if (!Application.isEditor)
			directory = System.IO.Directory.GetParent(Application.dataPath).ToString();
		else if (FFTGameManager.Instance.UseDebugLogDirectory)
		{
			directory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
		}
		
		
		directory += @"/ParticipantLogs/";
		
		directory += @"/" + participantID.ToString() + @"/";
		
		directory += @"/" + sessionID.ToString("D3") + "-" + experimentalCondition + @"/";		

        path = directory + timeStampString + "_" + levelProgressionID + "_" +levelTitle + "_" + participantString + @".NSFX" + @".xml";

        if (!System.IO.Directory.Exists(directory))
        {
            System.IO.Directory.CreateDirectory(directory);
        }

        //Due to the timestamp, this should never actually delete the file, but just in case:
		//ACTUALLY, we did, so add the milliseconds field just to be super safe and DON'T DELETE THE DATA, fool
        if (System.IO.File.Exists(path))
        {
			path = directory + timeStampString + "." + dtn.Millisecond.ToString() + "_" + levelProgressionID + "_" + levelTitle + "_" + participantString + @".NSFX" + @".xml";
            //System.IO.File.Delete(path);
        }

        // Create XML Document

        XmlDocument levelXml = new XmlDocument();
        {
            XmlNode rootNode = levelXml.CreateElement("participant");
            levelXml.AppendChild(rootNode);

            XmlNodeAppendAttribute(ref levelXml, ref rootNode, "id", participantID.ToString());
            XmlNodeAppendAttribute(ref levelXml, ref rootNode, "session", sessionID.ToString());
            XmlNodeAppendAttribute(ref levelXml, ref rootNode, "condition", experimentalCondition);
			XmlNodeAppendAttribute(ref levelXml, ref rootNode, "levelLoaded", FFTGameManager.Instance.LevelsAttempted.ToString());
			
			//Level Overview
			
			XmlNode levelOverviewNode = levelXml.CreateElement("overview");
			rootNode.AppendChild(levelOverviewNode);

            XmlNode timeNode = levelXml.CreateElement("time");
            levelOverviewNode.AppendChild(timeNode);

            XmlNodeAppendAttribute(ref levelXml, ref timeNode, "levelTime", FFTGameManager.Instance.LevelGameplayElapsedTime.ToString());
            XmlNodeAppendAttribute(ref levelXml, ref timeNode, "sessionTime", Time.timeSinceLevelLoad.ToString());
			XmlNodeAppendAttribute(ref levelXml, ref timeNode, "isTimedLevel", FFTGameManager.Instance.CurrentLevel.TimedLevel.ToString());
            XmlNodeAppendAttribute(ref levelXml, ref timeNode, "timeLimit", FFTGameManager.Instance.CurrentLevel.TimedLevel.ToString());

            XmlNode scoreNode = levelXml.CreateElement("score");
            levelOverviewNode.AppendChild(scoreNode);

            XmlNodeAppendAttribute(ref levelXml, ref scoreNode, "totalStars", FFTGameManager.Instance.CurrentScore.RecipeTotalStarRating().ToString());
            //XmlNodeAppendAttribute(ref levelXml, ref scoreNode, "levelName", FFTGameManager.Instance.CurrentLevel.LevelSource.name);
			string levelPath = FFTGameManager.Instance.CurrentLevel.GetCurrentLevelPathFromExternalList();
			levelPath = levelPath.Substring(levelPath.IndexOf("LevelData"));
            XmlNodeAppendAttribute(ref levelXml, ref scoreNode, "levelName", levelPath);

            XmlNode detailNode = levelXml.CreateElement("detail");
            scoreNode.AppendChild(detailNode);

            foreach (KeyValuePair<string, List<FFTStepReport>> pair in FFTGameManager.Instance.CurrentScore.Dict)
            {
                XmlNode dishNode = levelXml.CreateElement("dish");
                XmlNodeAppendAttribute(ref levelXml, ref dishNode, "name", pair.Key);
                detailNode.AppendChild(dishNode);
                int i = 1;
                List<FFTStepReport> reports = pair.Value;
                foreach (FFTStepReport report in reports)
                {
                    XmlNode stepNode = levelXml.CreateElement("step");
                    XmlNodeAppendAttribute(ref levelXml, ref stepNode, "id", i.ToString());
                    XmlNodeAppendAttribute(ref levelXml, ref stepNode, "stars", report.StarRating.ToString());
                    XmlNodeAppendAttribute(ref levelXml, ref stepNode, "feedback", report.Feedback.ToString());
                    dishNode.AppendChild(stepNode);
                    i++;
                }
            }
			
			// TODO: Detailed level data goes here
        }

        levelXml.Save(path);
#if UNITY_EDITOR        
        if (Application.isEditor && !FFTGameManager.Instance.UseDebugLogDirectory) 
            UnityEditor.AssetDatabase.Refresh();
#endif
        return path;

    }	


}
