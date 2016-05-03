using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(FFTCounter))]
public class FFTCounterEditor : Editor
{
    FFTCounter Counter;

    public void OnEnable()
    {
        Counter = (FFTCounter)target;

        Counter.InitializeSlotHost();

    }

    public override void OnInspectorGUI()
    {
        float width = 100.0f;
        if (Counter.RecipeCard != null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Level Name:", GUILayout.Width(width));
            Counter.RecipeCard.LevelTitle = GUILayout.TextArea(Counter.RecipeCard.LevelTitle);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Author:", GUILayout.Width(width));
            Counter.RecipeCard.Author = GUILayout.TextArea(Counter.RecipeCard.Author);
            GUILayout.EndHorizontal();
            /*
			GUILayout.BeginHorizontal();
            GUILayout.Label("Revision:", GUILayout.Width(width));
            EditorGUILayout.IntField(Counter.RecipeCard.RevisionNumber);
            GUILayout.EndHorizontal();
			*/
			GUILayout.BeginHorizontal();
			GUILayout.Label("Flavor Text:", GUILayout.Width(width));
			EditorGUILayout.TextArea(Counter.RecipeCard.FlavorText);            
            GUILayout.EndHorizontal();
        }
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

        //base.OnInspectorGUI();
    }
	
}
