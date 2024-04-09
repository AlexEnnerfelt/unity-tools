using System;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(validOn: AttributeTargets.Field | AttributeTargets.Property)]
public class TagDefineAttribute : PropertyAttribute {
	public readonly string tagName;
	public static Dictionary<string, string[]> taglists = new();


	public TagDefineAttribute(string tagName) {
		this.tagName = tagName;
	}
}