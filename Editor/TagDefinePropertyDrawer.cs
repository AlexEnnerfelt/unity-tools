using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Packages.com.unpopular_opinion.tools.Editor {
	//[CustomPropertyDrawer(typeof(TagDefineAttribute))]
	public class TagDefinePropertyDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			var tagDefine = (TagDefineAttribute)attribute;

			if (property.isArray) {
				List<string> tagArray = new();

				for (var i = 0; i < property.arraySize; i++) {
					tagArray.Add(property.GetArrayElementAtIndex(i).stringValue);
				}
				if (TagDefineAttribute.taglists.ContainsKey(tagDefine.tagName)) {
					TagDefineAttribute.taglists[tagDefine.tagName] = tagArray.ToArray();
				} else {
					TagDefineAttribute.taglists.Add(tagDefine.tagName, tagArray.ToArray());
				}
			}
			base.OnGUI(position, property, label);
		}
	}
}