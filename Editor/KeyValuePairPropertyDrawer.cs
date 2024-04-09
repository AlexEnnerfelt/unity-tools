using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(KeyValuePairList))]
public class KeyValuePairListPropertyDrawer : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		base.OnGUI(position, property, label);

	}
}

[CustomPropertyDrawer(typeof(KeyValuePairList.KeyValuePairRef))]
public class KeyValuePairPropertyDrawer : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		base.OnGUI(position, property, label);

	}
}