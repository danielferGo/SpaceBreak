using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpaceShip))] // Replace with your actual script name
public class SpaceshipControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SpaceShip controller = (SpaceShip)target;

        GUILayout.Space(10);
        GUILayout.Label("Test Gestures", EditorStyles.boldLabel);

        if (GUILayout.Button("Swipe Left"))
        {
            controller.ChangeDirection(0);
        }

        if (GUILayout.Button("Swipe Right"))
        {
            controller.ChangeDirection(1);
        }

        if (GUILayout.Button("Swipe Forward"))
        {
            controller.ChangeDirection(2);
        }

        if (GUILayout.Button("Swipe Backward"))
        {
            controller.ChangeDirection(3);
        }
    }
}