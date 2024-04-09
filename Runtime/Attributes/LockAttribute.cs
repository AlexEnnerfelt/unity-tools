using UnityEngine;

public class LockAttribute : PropertyAttribute {
	public readonly string warningMessage;

	public LockAttribute(string warningMessage = "") {
		this.warningMessage = warningMessage;
	}
}

