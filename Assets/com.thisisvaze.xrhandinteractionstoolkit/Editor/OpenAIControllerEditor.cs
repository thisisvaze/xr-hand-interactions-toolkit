using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(GenerateCustomPoseFunction))]
public class OpenAIControllerEditor : Editor
{ private string buttonText = "Generate";
   public override void OnInspectorGUI()
{
    GenerateCustomPoseFunction myScript = (GenerateCustomPoseFunction)target;

    // Manually draw the Behavior description field
    myScript.behaviorDescription = EditorGUILayout.TextField("Behavior description", myScript.behaviorDescription);

     // Draw the Generate button
        if(GUILayout.Button(buttonText))
        {
            buttonText = "Generating..";
            myScript.GenerateAndLogResponse();
            buttonText = "Generated";
        }

    // Draw the rest of the fields
    serializedObject.Update();
    SerializedProperty iterator = serializedObject.GetIterator();
    iterator.NextVisible(true); // Skip the first property (the script field)
    iterator.NextVisible(true); // Skip the second property (the behaviorDescription field)
     // Skip the behaviorDescription field
    while (iterator.name == "behaviorDescription")
    {
        iterator.NextVisible(false);
    }

    do
    {
        EditorGUILayout.PropertyField(iterator, true);
    }
    while (iterator.NextVisible(false));
    serializedObject.ApplyModifiedProperties();
}
}