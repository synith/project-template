using UnityEditor;
using UnityEngine;

namespace Injection.Editor
{
    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(Injector))]
    public class InjectorPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property == null)
            {
                return;
            }
            
            var name = property.FindPropertyRelative("m_name");
            EditorGUILayout.LabelField($"{property.name}: {name?.stringValue}");
        }
    }
    #endif
}