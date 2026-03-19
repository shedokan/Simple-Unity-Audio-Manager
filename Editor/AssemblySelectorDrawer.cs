using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace JSAM.JSAMEditor
{
    [CustomPropertyDrawer(typeof(AssemblySelector))]
    public class AssemblySelectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty assetProp = property.FindPropertyRelative(nameof(AssemblySelector.assemblyAsset));
            SerializedProperty nameProp = property.FindPropertyRelative("assemblyName");

            // 1. Draw the Object Picker for the Assembly Definition Asset
            EditorGUI.BeginChangeCheck();
            Object selectedAsset = EditorGUI.ObjectField(position, label, assetProp.objectReferenceValue, typeof(AssemblyDefinitionAsset), false);
            
            if (EditorGUI.EndChangeCheck())
            {
                assetProp.objectReferenceValue = selectedAsset;
                
                if (selectedAsset is AssemblyDefinitionAsset asmDefAsset)
                {
                    // 2. Parse the .asmdef JSON to get the "name" field
                    string json = asmDefAsset.text;
                    var data = JsonUtility.FromJson<AsmdefData>(json);
                    nameProp.stringValue = data.name;
                }
                else
                {
                    nameProp.stringValue = "";
                }
            }

            EditorGUI.EndProperty();
        }

        // Small helper class to parse the name from the asmdef JSON
        [System.Serializable]
        private class AsmdefData { public string name; }
    }
}
