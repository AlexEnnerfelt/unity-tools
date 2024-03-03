using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;


public static class JsonObjectConstructor {

	public static List<JsonConverter> converters = new();

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T">Any primitive type</typeparam>
	/// <param name="keyFoundAction">The action to take if the key is found in the Json 
	/// (recommended to use for applying found value)</param>
	/// <param name="key">The name of the variable that needs the data applied 
	/// (prefix '_' is automatically trimmed and disregards irregularties in Pascal and Camel case)</param>
	public static void ApplyValue<T>(this JObject jsonObject, Action<T> keyFoundAction, string key) {
		if (jsonObject.TryGetValueFromJson<T>(key, out var val)) {
			keyFoundAction.Invoke(val);
		}
	}
	public static T GetValueFromJson<T>(this string jsonString, string key) {
		var jsonObject = JObject.Parse(jsonString);
		return jsonObject.GetValueFromJson<T>(key);
	}
	public static T GetValueFromJson<T>(this JObject jsonObject, string key) {
		// Try to get a JsonToken from from the JsonObject and ignore wether it's camel or pascal case
		if (jsonObject.TryGetValue(key, StringComparison.OrdinalIgnoreCase, out var jToken)) {
			var settings = new JsonSerializerSettings();
			converters.ForEach(con => settings.Converters.Add(con));


			return jToken.ToObject<T>();
		} else {
			Debug.LogException(new JsonReaderException($"Could not find object of type {typeof(T).Name} with key: {key}"));
			return default;
		}
	}
	public static bool TryGetValueFromJson<T>(this JObject jsonObject, string key, out T result) {
		var trimmedKey = key.TrimStart('_');
		if (jsonObject.TryGetValue(trimmedKey, StringComparison.OrdinalIgnoreCase, out var jToken)) {
			var settings = new JsonSerializerSettings();
			converters.ForEach(con => settings.Converters.Add(con));

			try {
				result = jToken.ToObject<T>(JsonSerializer.Create(settings));
			} catch (JsonReaderException) {
				result = default;
				return false;
			}
			return true;
		} else {
			result = default;
			return false;
		}
	}
}


public class VectorConverter : JsonConverter {
	public override bool CanConvert(Type objectType) {
		return objectType == typeof(Vector3) || objectType == typeof(Vector2);
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
		var array = JArray.Load(reader);

		//If Vector3
		if (objectType == typeof(Vector3)) {
			var count = array.Count;
			return count switch {
				int n when n == 1 => new Vector3(array[0].Value<float>(), 0, 0),
				int n when n == 2 => new Vector3(array[0].Value<float>(), array[1].Value<float>(), 0),
				int n when n > 2 => new Vector3(array[0].Value<float>(), array[1].Value<float>(), array[2].Value<float>()),
				_ => Vector3.zero,
			};
			//Vector2
		} else {

			var count = array.Count;
			return count switch {
				int n when n == 1 => new Vector2(array[0].Value<float>(), 0),
				int n when n > 2 => new Vector2(array[0].Value<float>(), array[1].Value<float>()),
				_ => Vector2.zero,
			};
		}
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
		var vector = (Vector3)value;
		serializer.Serialize(writer, new float[] { vector.x, vector.y, vector.z });
	}
}
