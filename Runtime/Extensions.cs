using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

public static class Extensions {
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
	public static Color32 FadeAlpha(this Color32 color, float normalizedAlpha) {
		var newAlpha = (byte)(normalizedAlpha * 255);
		return new Color32(color.r, color.g, color.b, newAlpha);
	}
	public static Color FadeAlpha(this Color color, float normalizedAlpha) {
		return new Color(color.r, color.g, color.b, normalizedAlpha);
	}
	public static Transform[] GetChildren(this Transform transform) {
		var count = transform.childCount;
		var children = new Transform[count];
		for (var i = 0; i < count; i++) {
			children[i] = transform.GetChild(i);
		}
		return children;
	}
	public static Transform[] GetAllChildren(this Transform parent) {
		var allChildren = new List<Transform>();
		GetChildRecursive(parent, allChildren);
		return allChildren.ToArray();
	}

	private static void GetChildRecursive(Transform obj, List<Transform> childrenList) {
		if (obj == null)
			return;

		foreach (Transform child in obj) {
			if (child == null)
				continue;

			childrenList.Add(child);
			GetChildRecursive(child, childrenList);
		}
	}

	//Object model extensions
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

	//Component assignment extension
	public static bool TryGetComponentInParent<T>(this GameObject obj, out T component) where T : Component {
		return obj.transform.TryGetComponentInParent(out component);
	}
	public static bool TryGetComponentInParent<T>(this Component obj, out T component) where T : Component {
		if (obj.TryGetComponent(out component)) {
			return true;
		} else {
			component = obj.GetComponentInParent<T>();
		}
		return component != null;
	}
	public static bool TryGetComponentsInParent<T>(this GameObject obj, out T component) where T : Component {
		return obj.TryGetComponentsInParent(out component);
	}
	public static bool TryGetComponentsInParent<T>(this Component obj, out T component) where T : Component {
		if (obj.TryGetComponent(out component)) {
			return true;
		} else {
			component = obj.GetComponentInParent<T>();
		}
		return component != null;
	}
	public static bool TryGetComponentInChildren<T>(this GameObject obj, out T component) {
		return obj.transform.TryGetComponentInChildren(out component);
	}
	public static bool TryGetComponentInChildren<T>(this Component obj, out T component) {
		if (obj.TryGetComponent(out component)) {
			return true;
		} else {
			component = obj.GetComponentInChildren<T>();
		}
		return component != null;
	}
	public static bool TryGetComponentsInChildren<T>(this GameObject obj, out T[] components) {
		return obj.transform.TryGetComponentsInChildren(out components);
	}
	public static bool TryGetComponentsInChildren<T>(this Component obj, out T[] components) {
		components = obj.GetComponentsInChildren<T>();
		if (components == null || components.Length < 1) {
			return false;
		} else {
			return true;
		}
	}
	public static bool TryGetComponentAround<T>(this GameObject obj, out T components, bool parentFirst = true) where T : Component {
		return obj.transform.TryGetComponentAround(out components, parentFirst);
	}
	public static bool TryGetComponentAround<T>(this Component obj, out T components, bool parentFirst = true) where T : Component {
		components = parentFirst ? obj.GetComponentInParent<T>() : obj.GetComponentInChildren<T>();
		if (components == null) {
			components = parentFirst ? obj.GetComponentInChildren<T>() : obj.GetComponentInParent<T>();
		}
		return components != null;
	}

	//Layer extensions
	public static bool Contains(this LayerMask mask, int layer) {
		return (mask.value & (1 << layer)) != 0;
	}

	//Vector extensions
	public static Vector3 NormalizedToEuler(this Vector3 normalizedVector) {
		if (normalizedVector.x > 1 || normalizedVector.x < -1 || normalizedVector.y > 1 || normalizedVector.y < -1) {
			Debug.LogError("This vector is not normalized and will return zero");
			return Vector3.zero;
		}
		var angle = Mathf.Atan2(normalizedVector.y, normalizedVector.x) * Mathf.Rad2Deg;
		var eulerAngle = new Vector3(0, 0, angle);
		return eulerAngle;
	}
	public static Vector3 NormalizedToEuler(this Vector2 normalizedVector) {
		if (normalizedVector.x > 1 || normalizedVector.x < -1 || normalizedVector.y > 1 || normalizedVector.y < -1) {
			Debug.LogError("This vector is not normalized and will return zero");
			return Vector2.zero;
		}
		var angle = Mathf.Atan2(normalizedVector.y, normalizedVector.x) * Mathf.Rad2Deg;
		var eulerAngle = new Vector3(0, 0, angle);
		return eulerAngle;
	}

	//Audio extensions
	public static float DecibelToLinear(this float decibel) {
		var linear = Mathf.Pow(10f, decibel / 20f);
		return linear;
	}
}