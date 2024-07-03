using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AssetPreviewAttribute))]
public class SpritePreviewDrawer : PropertyDrawer {
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		// Get the sprite reference from the serialized property
		var sprite = property.objectReferenceValue as Sprite;
		var t = property.serializedObject.targetObject.GetType();

		//var field = t.GetProperty(property.name);
		//var attribute = field.GetCustomAttribute(typeof(AssetPreviewAttribute)) as AssetPreviewAttribute;


		if (sprite != null) {
			// Get the preview texture for the sprite
			Texture2D previewTexture = AssetPreview.GetAssetPreview(sprite);

			// Display the preview
			if (previewTexture != null) {
				GUILayout.Label(previewTexture, GUILayout.Width(60), GUILayout.Height(60));
			}
			else {
				GUILayout.Label("No preview available");
			}
		}
		else {
			GUILayout.Label("Select a sprite");
		}

		// Draw the default property field (optional)
		EditorGUI.PropertyField(position, property, label, true);
	}
}