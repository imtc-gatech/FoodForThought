using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(FFTRecipeMaker))]
public class FFTRecipeMakerEditor : Editor
{
    FFTRecipeMaker RecipeMaker;
    FFTCounter Counter;
    FFTKitchen Kitchen;

    public void OnEnable()
    {
        // Grab our Editor target so it is a bit more semantic (and to eliminate constant casts to RecipeMaker):
        RecipeMaker = (FFTRecipeMaker)target;

        RecipeMaker.gameObject.name = "__RecipeMaker__";

        Counter = RecipeMaker.gameObject.GetComponentInChildren<FFTCounter>();

        if (Counter == null)
        {
            GameObject CounterGO = GameObject.Instantiate(Resources.Load("MainGamePrefabs/Counter", typeof(GameObject)) as GameObject) as GameObject;
            CounterGO.name = "Counter (click to enable)";
            CounterGO.transform.parent = RecipeMaker.gameObject.transform;
            
            Counter = CounterGO.gameObject.AddComponent<FFTCounter>();
        }

        Kitchen = RecipeMaker.gameObject.GetComponentInChildren<FFTKitchen>();

        if (Kitchen == null)
        {
            GameObject KitchenGO = new GameObject();
            KitchenGO.name = "Kitchen (click to enable)";
            KitchenGO.transform.parent = RecipeMaker.gameObject.transform;

            Kitchen = KitchenGO.AddComponent<FFTKitchen>();
        }
        
    }

    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save"))
        {
            SaveRecipeXML();
        }
        if (GUILayout.Button("Load"))
        {
            
            LoadRecipeXML(); 
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        RecipeMaker.LevelPath = EditorGUILayout.ObjectField("Load Level from XML: ", RecipeMaker.LevelPath, typeof(TextAsset), false) as TextAsset;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        RecipeMaker.TimedLevel = EditorGUILayout.Toggle("Timed: ", RecipeMaker.TimedLevel);
        if (RecipeMaker.TimedLevel)
        {
            RecipeMaker.TimeLimit = EditorGUILayout.FloatField("Time Limit (sec):", RecipeMaker.TimeLimit);
        }
        else
        {
            EditorGUILayout.TextField("<-- UNTIMED (check to change)");
        }
        
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUIContent ttElapsedTimeMultiplier = new GUIContent("Elapsed Time Multiplier:", "Allows for global scaling of all time based steps in dishes. Does NOT scale the time limit.");
        RecipeMaker.ElapsedTimeMultiplier = EditorGUILayout.Slider(ttElapsedTimeMultiplier, RecipeMaker.ElapsedTimeMultiplier, 0.1f, 10f);
        GUILayout.EndHorizontal();

        //**********************COUNTER EDITOR CODE*****************************************************//
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Recipe Information:");
        EditorGUILayout.EndHorizontal();
        float recipeColWidth = 100.0f;
        if (Counter.RecipeCard != null)
        {
            GUILayout.BeginHorizontal();
            if (Counter.RecipeCard.LevelTitle == "") //experimental dynamic editor fields
                GUILayout.Label("Level Name:", GUILayout.Width(recipeColWidth));
                //GUILayout.Label("Level Name (EMPTY):", GUILayout.Width(recipeColWidth + 50));
            else
                GUILayout.Label("Level Name:", GUILayout.Width(recipeColWidth));
            Counter.RecipeCard.LevelTitle = GUILayout.TextArea(Counter.RecipeCard.LevelTitle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Author:", GUILayout.Width(recipeColWidth));
            Counter.RecipeCard.Author = GUILayout.TextArea(Counter.RecipeCard.Author);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Customer:", GUILayout.Width(recipeColWidth));
            RecipeMaker.Counter.RecipeCard.Customer = (FFTCustomer.Name)EditorGUILayout.EnumPopup(RecipeMaker.Counter.RecipeCard.Customer);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Flavor Text:", GUILayout.Width(recipeColWidth));
            RecipeMaker.Counter.RecipeCard.FlavorText = EditorGUILayout.TextField(RecipeMaker.Counter.RecipeCard.FlavorText);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("FRESHNESS:", GUILayout.Width(recipeColWidth));
            RecipeMaker.Counter.RecipeCard.FreshnessParameters.UseFreshness = GUILayout.Toggle(RecipeMaker.Counter.RecipeCard.FreshnessParameters.UseFreshness, "use freshness");
            GUILayout.EndHorizontal();
            if (RecipeMaker.Counter.RecipeCard.FreshnessParameters.UseFreshness)
            {
                GUILayout.BeginHorizontal();
                RecipeMaker.Counter.RecipeCard.FreshnessParameters.DelayInSeconds = EditorGUILayout.FloatField("Delay (sec):", RecipeMaker.Counter.RecipeCard.FreshnessParameters.DelayInSeconds);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                RecipeMaker.Counter.RecipeCard.FreshnessParameters.DecayTimeInSeconds = EditorGUILayout.FloatField("Total Range (sec):", RecipeMaker.Counter.RecipeCard.FreshnessParameters.DecayTimeInSeconds);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                RecipeMaker.Counter.RecipeCard.FreshnessParameters.StarPenaltyMaximum = EditorGUILayout.FloatField("Star Penalty (stars):", RecipeMaker.Counter.RecipeCard.FreshnessParameters.StarPenaltyMaximum);
                GUILayout.EndHorizontal();
            }
            if (RecipeMaker.TimedLevel)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("CALCULATIONS:", GUILayout.Width(recipeColWidth));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                RecipeMaker.RecalculateDifficultyAndComplexity = GUILayout.Button("[Recalculate Diff/Complex Values Below]");
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Difficulty:", GUILayout.Width(recipeColWidth));
                if (RecipeMaker.TimedLevel)
                    EditorGUILayout.FloatField(RecipeMaker.Difficulty);
                else
                    EditorGUILayout.TextField("(untimed)");
                GUILayout.Label("Complexity:", GUILayout.Width(recipeColWidth));
                if (RecipeMaker.TimedLevel)
                    EditorGUILayout.FloatField(RecipeMaker.Complexity);
                else
                    EditorGUILayout.TextField("(untimed)");
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("CALCULATIONS: UNTIMED (enable timed level to use)");
                GUILayout.EndHorizontal();
            }
            
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label("DISH / STATION EDITING:");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Dish"))
        {
            if (Counter.AddSlot())
            {
                Debug.Log("Added Slot to Counter.");
            }
            else
            {
                Debug.Log("Maximum Slots (" + Counter.MaxSlots + ") Reached.");
            }

        }
        if (GUILayout.Button("Remove Dish"))
        {
            if (Counter.RemoveSlot())
            {
                Debug.Log("Removed Slot from Counter.");


            }
            else
            {
                Debug.Log("No more slots to remove!");
            }
        }
        GUILayout.EndHorizontal();

        //**********************KITCHEN EDITOR CODE*****************************************************//

        float kitchenHeaderWidth = 150.0f;

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Kitchen Station Slots");
        EditorGUILayout.EndHorizontal();
        if (Kitchen.StationList == null || Kitchen.StationList.Count == 0)
            Kitchen.InitializeStations();
        foreach (KeyValuePair<FFTStation.Type, FFTStation> station in Kitchen.StationList)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextArea(station.Key.ToString() + " " + station.Value.SlotList.Count.ToString(), GUILayout.Width(kitchenHeaderWidth));
            if (GUILayout.Button("+"))
            {
                station.Value.AddSlot();
            }
            if (GUILayout.Button("-"))
            {
                station.Value.RemoveSlot();
            }
            EditorGUILayout.EndHorizontal();

        }

        Kitchen.UpdateKitchenName();

        //base.OnInspectorGUI();
    }

    void SaveRecipeXML()
    {
        FFTRecipeHandlerUtilities.SaveRecipeXML(ref RecipeMaker, ref Counter, ref Kitchen);
        RecipeMaker.LevelPath = null;
    }


    void LoadRecipeXML()
    {
        if (RecipeMaker.LevelPath != null &&
            RecipeMaker.LevelPath.name.Contains("FFTLevel"))
        {

            Debug.Log("Load Recipe");

            FFTRecipeHandlerUtilities.LoadRecipeXML(RecipeMaker.LevelPath, true);

            RecipeMaker.gameObject.tag = "Finish";
            RecipeMaker.gameObject.name = "__delete__" + RecipeMaker.gameObject.name;
            Vector3 shovePosition = RecipeMaker.gameObject.transform.position;
            shovePosition.z -= 100.0f;
            RecipeMaker.gameObject.transform.position = shovePosition;

            //newRecipe.transform.parent = Recipe.gameObject.transform;
        }


    }

}

