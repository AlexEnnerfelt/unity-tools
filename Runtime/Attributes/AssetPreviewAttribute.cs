using UnityEngine;

public class AssetPreviewAttribute : PropertyAttribute {
	public int size = 60;
	public AssetPreviewAttribute(int size) {
		this.size = size;
	}
	public AssetPreviewAttribute() {
		this.size = 60;
	}
}