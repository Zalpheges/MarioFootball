using System.Linq;
using System.Reflection;
using UnityEditor;

[CustomEditor(typeof(Team))]
public class TeamEditor : Editor
{
    private SerializedProperty teamType;
    private SerializedProperty goalType;

    int teamIndex = 0;
    int goalIndex = 0;

    private string[] brainTypes;

    public override void OnInspectorGUI()
    {
        System.Type[] types = Assembly.GetAssembly(typeof(PlayerBrain)).GetTypes();
        System.Type[] possible = (from System.Type type in types where type.IsSubclassOf(typeof(PlayerBrain)) select type).ToArray();

        brainTypes = possible.Where(type => !type.IsAbstract).Select(type => type.Name).ToArray();

        teamType = serializedObject.FindProperty("_ateamBrainType");
        goalType = serializedObject.FindProperty("_agoalBrainType");

        for (int i = 0; i < brainTypes.Length; ++i)
        {
            if (brainTypes[i] == teamType.stringValue)
                teamIndex = i;

            if (brainTypes[i] == goalType.stringValue)
                goalIndex = i;
        }

        serializedObject.Update();

        teamIndex = EditorGUILayout.Popup("Team Brain Type", teamIndex, brainTypes);
        goalIndex = EditorGUILayout.Popup("Goal Brain Type", goalIndex, brainTypes);

        teamType.stringValue = brainTypes[teamIndex];
        goalType.stringValue = brainTypes[goalIndex];

        serializedObject.ApplyModifiedProperties();
    }
}
