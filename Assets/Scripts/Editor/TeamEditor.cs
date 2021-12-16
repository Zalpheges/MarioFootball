using UnityEngine;
using System.Reflection;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(Team))]
public class TeamEditor : Editor
{
    private SerializedProperty teamType;
    private SerializedProperty goalType;

    int teamIndex = 0;
    int goalIndex = 0;

    private string[] brainTypes;

    private void OnEnable()
    {
        System.Type[] types = Assembly.GetAssembly(typeof(PlayerBrain)).GetTypes();
        System.Type[] possible = (from System.Type type in types where type.IsSubclassOf(typeof(PlayerBrain)) select type).ToArray();

        brainTypes = possible.Select(type => type.Name).ToArray();
    }

    public override void OnInspectorGUI()
    {
        teamType = serializedObject.FindProperty("_ateamBrainType");
        goalType = serializedObject.FindProperty("_agoalBrainType");

        teamIndex = EditorGUILayout.Popup("Team Brain Type", teamIndex, brainTypes);
        goalIndex = EditorGUILayout.Popup("Goal Brain Type", goalIndex, brainTypes);

        teamType.stringValue = brainTypes[teamIndex];
        goalType.stringValue = brainTypes[goalIndex];

        serializedObject.ApplyModifiedProperties();
    }
}
