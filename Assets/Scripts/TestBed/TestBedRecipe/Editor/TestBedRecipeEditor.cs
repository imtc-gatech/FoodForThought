using UnityEngine;
using UnityEditor; // required for Editor scripts
using System.Collections;
using System.Collections.Generic; // you need to add this using in order to use List<> and Dictionary<>

[CustomEditor (typeof(TestBedRecipe))] // This is how the editor script knows which script to "replace" it's default Inspector 

    
// Note that we are deriving from "Editor" and not "MonoBehaviour" here.    
public class TestBedRecipeEditor : Editor {
    
    TestBedRecipe recipe;

    private int numberOfInstantiatedObjects = 0;

    public void OnEnable()
    {
        // Every Editor has a reference to it's host script. Here we cast it to our own variable for convenience.
        recipe = (TestBedRecipe)target;

        // Delegate stuff, see the tutorial link for details.
        SceneView.onSceneGUIDelegate += RecipeUpdate; //using = instead of += overrides all other callbacks
    }

    public void OnDisable()
    {
        // Delegate stuff, see the tutorial link for details.
        SceneView.onSceneGUIDelegate -= RecipeUpdate; //cleaning up our current callback
    }

    // To override the default Inspector GUI, you must overwrite this method.
    public override void OnInspectorGUI()
    {
        // We can style elements of the Inspector just as we would in GUIText (see GUIStyle docs for details)
        GUIStyle headerStyle = new GUIStyle();
        headerStyle.fontStyle = FontStyle.Bold;
        headerStyle.normal.textColor = Color.cyan;
        headerStyle.fontSize = 14;

        GUILayout.BeginHorizontal();
        GUILayout.Label("Default Inspector Variable Display", headerStyle, GUILayout.Height(20));
        GUILayout.EndHorizontal();

        // This call will show all of the things the Inspector would normally show (all public variables, formatted in the default (unhelpful) manner
        base.OnInspectorGUI();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Custom Inspector Variable Display", headerStyle, GUILayout.Height(20));
        GUILayout.EndHorizontal();

        // For custom layouts, we will only see what we explicitly define to be shown.

        // Layout is similar to Java GUI Swing layouts... for better or worse.
        GUILayout.BeginHorizontal();
        GUILayout.Label("  Size");
        EditorGUILayout.IntField(recipe.ParametersList.Count, GUILayout.Width(50));
        GUILayout.EndHorizontal();


        // We can also assign an EditorGUILayout Field to a variable. 
        // Note how changing this variable in our custom display changes the values in the default display.

        GUILayout.BeginHorizontal();
        recipe.Position = EditorGUILayout.Vector3Field("Pos:", recipe.Position);
        GUILayout.EndHorizontal();

        // If this button is clicked, add a new element to 
        if (GUILayout.Button("Add Element", GUILayout.Width(255)))
        {
            numberOfInstantiatedObjects++;
            recipe.AddParameterElement(numberOfInstantiatedObjects);
        }

        // Keeping track of our index in the ParametersList so we can remove
        // It starts over at zero each time the Component GUI is redrawn
        int currentIndex = 0;

        // For keeping track of all elements to remove if the user clicks "Destroy"
        List<int> removedElements = new List<int>();
        
        // For each created object (GaugeParameters), display a separate GUI box for each
        foreach (GaugeParameters gauge in recipe.ParametersList)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.IntField("Element", currentIndex);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("U:");
            EditorGUILayout.FloatField(gauge.Unfinished, GUILayout.Width(30));
            GUILayout.Label("F:");
            EditorGUILayout.FloatField(gauge.Finished, GUILayout.Width(30));
            GUILayout.Label("O:");
            EditorGUILayout.FloatField(gauge.Overdone, GUILayout.Width(30));

            // If this button is pressed, add the index of this element to our removal list:
            if (GUILayout.Button("Destroy", GUILayout.Width(60)))
            {
                removedElements.Add(currentIndex);
            }
            GUILayout.EndHorizontal();

            
            // Increase our index by one for each iteration through the foreach loop
            currentIndex++;
        }

        // Remove all destroyed elements
        // (done outside of the loop to keep enumerator from breaking)
        foreach (int indexToRemove in removedElements)
        {
            recipe.ParametersList.RemoveAt(indexToRemove);
        }


        // Whenever the GUI is changed, you should call SetDirty to tell Unity to update the variables and states within
        if (GUI.changed)
        {
            EditorUtility.SetDirty(recipe);
        }

    }

    // Delegate stuff, see the tutorial link for details.

    void RecipeUpdate(SceneView sceneView)
    {
        //Event e = Event.current;

    }
}
