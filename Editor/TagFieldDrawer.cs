using System;
using System.Collections.Generic;
using Packages.com.unpopular_opinion.tools.Runtime.Classes;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TagFieldAttribute))]
public class TagFieldDrawer : PropertyDrawer {
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		var tagFieldAttribute = (TagFieldAttribute)attribute;

		var tags = TryGetTagsFrom(tagFieldAttribute.tagName);
		if (tags != null) {
			var index = System.Array.IndexOf(tags, property.stringValue);
			index = EditorGUI.Popup(position, label.text, index, tags);
			if (index >= 0)
				property.stringValue = tags[index];
		} else {
			EditorGUI.PropertyField(position, property, label);
		}
	}

	private string[] TryGetTagsFrom(string tagName) {
		var manager = TagManager.Instance;
		var so = new SerializedObject(manager);
		var sp = so.GetIterator();
		while (sp.NextVisible(true)) {
			if (sp.propertyType == SerializedPropertyType.String && sp.isArray) {
				var defineAttribute = Attribute.GetCustomAttribute(manager.GetType().GetField(sp.name), typeof(TagDefineAttribute)) as TagDefineAttribute;
				if (defineAttribute != null && defineAttribute.tagName == tagName) {
					var tags = new List<string>();
					for (var i = 0; i < sp.arraySize; i++) {
						tags.Add(sp.GetArrayElementAtIndex(i).stringValue);
					}
					return tags.ToArray();
				}
			}
		}
		return null;

	}

	private string[] GetTags(string tagName) {
		var objs = Resources.FindObjectsOfTypeAll(typeof(MonoBehaviour));
		foreach (var obj in objs) {
			var so = new SerializedObject(obj);
			var sp = so.GetIterator();

			while (sp.NextVisible(true)) {
				if (sp.propertyType == SerializedPropertyType.String && sp.isArray) {
					var defineAttribute = Attribute.GetCustomAttribute(obj.GetType().GetField(sp.name), typeof(TagDefineAttribute)) as TagDefineAttribute;
					if (defineAttribute != null && defineAttribute.tagName == tagName) {
						var tags = new List<string>();
						for (var i = 0; i < sp.arraySize; i++) {
							tags.Add(sp.GetArrayElementAtIndex(i).stringValue);
						}
						return tags.ToArray();
					}
				}
			}
		}
		return null;
	}
}