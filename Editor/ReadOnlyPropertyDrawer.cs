using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(ReadOnlyFieldAttribute))]
public class ReadOnlyPropertyDrawer : PropertyDrawer {


	public override VisualElement CreatePropertyGUI(SerializedProperty property) {
		var field = new PropertyField(property);
		field.SetEnabled(false);
		return field;
	}
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		GUI.enabled = false; // Disable fields
		EditorGUI.PropertyField(position, property, label, true);
		GUI.enabled = true; // Enable fields
	}
}
