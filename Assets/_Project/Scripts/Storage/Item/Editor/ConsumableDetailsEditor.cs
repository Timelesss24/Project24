using UnityEditor;

namespace Timelesss.Editor
{
    [CustomEditor(typeof(ConsumableDetails))]
    public class ConsumableDetailsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("Icon"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Name"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("DropPrefab"));
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("MaxStack"));
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("RestoreValue"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("Description"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}