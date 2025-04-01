using UnityEditor;

namespace Timelesss.Editor
{
    [CustomEditor(typeof(WeaponDetails))]
    public class WeaponDetailsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("Icon"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Name"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("DropPrefab"));
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("LocalPosition"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("LocalRotation"));
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("EquipmentPrefab"));
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AttacksContainer"));
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OverrideController"));
            
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Description"));
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}