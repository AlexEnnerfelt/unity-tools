using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TagDefinedAttribute))]
public class TagDropdownPropertyDrawer : PropertyDrawer {
    public string[] options;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        var tagattribute = (TagDefinedAttribute)attribute;
        if (options == null) {
            var match = TagDefinitions.Instance.definitions.First(def => def.category.Equals(tagattribute.category));
            if (match != null) {
                options = match.tags;
            }
        }

        if (options != null) {
            var optionsList = options.ToList();

            // Check if the property is a string
            if (property.propertyType != SerializedPropertyType.String) {
                EditorGUI.PropertyField(position, property, label);
                return;
            }




            //selectedIndex = match.tags.ToList().IndexOf(property.stringValue);
            // Draw the options
            var selectedIndex = EditorGUI.Popup(position, property.displayName, optionsList.IndexOf(property.stringValue), options);

            // Update the property's value based on the selected option
            property.stringValue = options[selectedIndex];
        }
        else {
            base.OnGUI(position, property, label);
        }

    }
}