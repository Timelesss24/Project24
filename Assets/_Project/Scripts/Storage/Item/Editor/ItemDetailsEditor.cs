using System.Linq;
using UnityEditor;

namespace Timelesss.Editor
{
    [CustomEditor(typeof(ItemDetails))]
    public class ItemDetailsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("Icon"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Name"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("ItemType"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("DropPrefab"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("MaxStack"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("Description"));

            serializedObject.ApplyModifiedProperties();
        }
    }


    [CustomEditor(typeof(EquipmentDetails))]
    public class EquipmentDetailsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("Icon"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Name"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("DropPrefab"));

            // EquipmentType 필드 직접 커스텀 표시
            var equipmentTypeProp = serializedObject.FindProperty("EquipmentType");

            // enum에서 Weapon 제외한 값만 리스트로 만들기
            var values = System.Enum.GetValues(typeof(EquipmentType))
                .Cast<EquipmentType>()
                .Where(e => e != EquipmentType.Weapon)
                .ToArray();

            // 현재 값이 Weapon이면 첫 번째 값으로 강제 설정
            if (!values.Contains((EquipmentType)equipmentTypeProp.enumValueIndex))
            {
                equipmentTypeProp.enumValueIndex = (int)values[0];
            }

            // 선택 UI
            int selected = System.Array.IndexOf(values, (EquipmentType)equipmentTypeProp.enumValueIndex);
            selected = EditorGUILayout.Popup("Equipment Type", selected, values.Select(v => v.ToString()).ToArray());
            equipmentTypeProp.enumValueIndex = (int)values[selected];

            EditorGUILayout.PropertyField(serializedObject.FindProperty("EquipmentPrefab"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Description"));

            serializedObject.ApplyModifiedProperties();
        }
    }

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