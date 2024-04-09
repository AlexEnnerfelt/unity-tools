using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TagAttribute))]
public class TagPropertyDrawer : PropertyDrawer {
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		if (property.propertyType == SerializedPropertyType.String) {
			EditorGUI.BeginProperty(position, label, property);

			var oldTag = property.stringValue;
			var newTag = EditorGUI.TagField(position, label, oldTag);

			if (newTag != oldTag) {
				property.stringValue = newTag;
			}

			EditorGUI.EndProperty();
		} else {
			EditorGUI.PropertyField(position, property, label);
		}
	}
}
