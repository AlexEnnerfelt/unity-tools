using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

public static class EnnerfeltExtensions {
	//Color Extensions
	public static string GetHexcode(this Color c) {
		var hex = "";
		byte[] bytes = {
			floatToByte(c.r),
			floatToByte(c.g),
			floatToByte(c.b),
			floatToByte(c.a),
		};
		hex = BitConverter.ToString(bytes);
		hex = Regex.Replace(hex, "-", "");
		return $"#{hex}";

		byte floatToByte(float input) {
			var i = (byte)Mathf.RoundToInt(input * 255);
			return i;
		}
	}
	public static string GetHexcode(this Color32 c) {
		var hex = "";
		byte[] bytes = {
			c.r,
			c.g,
			c.b,
			c.a,
		};
		hex = BitConverter.ToString(bytes);
		hex = Regex.Replace(hex, "-", "");
		return $"#{hex}";
	}
	public static Transform[] GetChildren(this Transform transform) {
		var count = transform.childCount;
		var children = new Transform[count];
		for (var i = 0; i < count; i++) {
			children[i] = transform.GetChild(i);
		}
		return children;
	}
	public static void CopyPropertiesTo<T>(this T source, T dest) {
		var plist = from prop in typeof(T).GetProperties() where prop.CanRead && prop.CanWrite select prop;

		foreach (var prop in plist) {
			prop.SetValue(dest, prop.GetValue(source, null), null);
		}
	}
	public static void CopyPublicProperties<T>(this T source, T destination) {
		var propertyInfos = typeof(T).GetProperties(
			BindingFlags.Public |
			BindingFlags.Instance);

		foreach (var propertyInfo in propertyInfos) {
			if (propertyInfo.CanRead && propertyInfo.CanWrite) {
				var value = propertyInfo.GetValue(source, null);
				propertyInfo.SetValue(destination, value, null);
			}
		}
	}
	public static void CopyPublicFields<T>(this T source, T destination) {
		var fieldInfos = typeof(T).GetFields(
			BindingFlags.Public |
			BindingFlags.Instance);

		foreach (var fieldInfo in fieldInfos) {
			var value = fieldInfo.GetValue(source);
			fieldInfo.SetValue(destination, value);
		}
	}
	public static bool TryGetComponentInParent<T>(this GameObject obj, out T component) where T : Component {
		if (obj.TryGetComponent(out component)) {
			return true;
		} else {
			component = obj.GetComponentInParent<T>();
		}
		return component != null;
	}
	public static bool TryGetComponentInParent<T>(this Behaviour obj, out T component) {
		if (obj.TryGetComponent(out component)) {
			return true;
		} else {
			component = obj.GetComponentInParent<T>();
		}
		return component != null;
	}
	public static bool TryGetComponentsInChildren<T>(this GameObject obj, out T[] components) {
		components = obj.GetComponentsInChildren<T>();
		if (components == null || components.Length < 1) {
			return false;
		} else {
			return true;
		}
	}
	public static bool TryGetComponentsInChildren<T>(this Behaviour obj, out T[] components) {
		return obj.gameObject.TryGetComponentsInChildren<T>(out components);
	}
}