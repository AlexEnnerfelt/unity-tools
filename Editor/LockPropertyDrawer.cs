using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(LockAttribute))]
public class ReadOnlyWhenToggledDrawer : PropertyDrawer {
	private bool isLocked = true;

	string currentInput;
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
			GUI.enabled = false; // Disable fields
			if (currentInput != null) {
				property.stringValue = currentInput;
				currentInput = null;
			}
			EditorGUI.TextField(fieldRect, property.stringValue);
			GUI.enabled = true; // Enable fields
		}
		else {
			if (currentInput == null) {
				currentInput = property.stringValue;
			}
			if (!string.IsNullOrEmpty(readOnlyWhenToggledAttribute.warningMessage)) {
				EditorGUILayout.HelpBox(readOnlyWhenToggledAttribute.warningMessage, MessageType.Warning);
			}
			currentInput = EditorGUI.TextField(fieldRect, currentInput);
		}

		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty();
	}
}