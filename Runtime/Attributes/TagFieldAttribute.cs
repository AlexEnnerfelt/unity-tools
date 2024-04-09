using UnityEngine;

public class TagFieldAttribute : PropertyAttribute {
	public readonly string tagName;

	public TagFieldAttribute(string tagName) {
		this.tagName = tagName;
	}
}
