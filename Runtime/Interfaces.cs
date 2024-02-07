using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// Used to be able to reference scriptable objects in the project by a unique Id
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IStaticRefereable<T> where T : ScriptableObject {
	public ushort Id { get; set; }
	public string ApiName { get; set; }
	public static Dictionary<ushort, T> objects;

	public static T GetObject(ushort id) {
		objects.TryGetValue(id, out var val);
		return val;
	}
	public static T GetObject() {
		return objects.First().Value;
	}
	public static T GetObjectByName(string name) {
		return objects.FirstOrDefault(o => o.Value.name == name).Value;
	}
	public static T GetObjectByApiName(string name) {
		return objects.FirstOrDefault(o => {
			var r = o.Value as IStaticRefereable<T>;
			return r.ApiName == name;
		}).Value;
	}

	public static void TryRegisterValue(T value) {
		objects ??= new();
		//Does not contain value
		if (objects.ContainsValue(value))
			return;

		var keyChanged = false;
		var val = value as IStaticRefereable<T>;
		var id = val.Id;
		while (objects.ContainsKey(id)) {
			id = (ushort)UnityEngine.Random.Range(ushort.MinValue, ushort.MaxValue);
			keyChanged = true;
		}
		objects.Add(id, value);
		val.Id = id;

		if (keyChanged) {

#if UNITY_EDITOR
			UnityEditor.EditorUtility.SetDirty(value);
			UnityEditor.AssetDatabase.SaveAssetIfDirty(value);
			UnityEditor.AssetDatabase.Refresh();
#endif
		}
	}
}