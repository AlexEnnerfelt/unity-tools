using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(LockAttribute))]
public class ReadOnlyWhenToggledDrawer : PropertyDrawer {
	private bool isLocked = true;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		var readOnlyWhenToggledAttribute = (LockAttribute)attribute;

		EditorGUI.BeginProperty(position, label, property);

		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		var toggleRect = new Rect(position.x, position.y, 30, position.height);
		isLocked = EditorGUI.Toggle(toggleRect, isLocked);

		var fieldRect = new Rect(position.x + 35, position.y, position.width - 35, position.height);
		if (isLocked) {
			EditorGUI.LabelField(fieldRect, property.stringValue);
		} else {
			if (!string.IsNullOrEmpty(readOnlyWhenToggledAttribute.warningMessage)) {
				EditorGUILayout.HelpBox(readOnlyWhenToggledAttribute.warningMessage, MessageType.Warning);
			}
			property.stringValue = EditorGUI.TextField(fieldRect, property.stringValue);
		}

		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty();
	}
}