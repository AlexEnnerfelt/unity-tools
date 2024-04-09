using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


[Serializable]
public class KeyValuePairList<TKey, TValue> : KeyValuePairList, IDictionary<TKey, TValue> where TKey : IComparable, IConvertible {
	[Serializable]
	public class KeyValuePairRef<JKey, JValue> : KeyValuePairRef, IEquatable<KeyValuePair<TKey, TValue>> {
		public JKey key; public JValue value;
		public KeyValuePairRef(JKey key, JValue value) {
			this.key = key;
			this.value = value;
		}

		public bool Equals(KeyValuePair<TKey, TValue> other) {
			return key.Equals(other.Key) && value.Equals(other.Value);
		}
	}
	public List<KeyValuePairRef<TKey, TValue>> keyValuePairs;

	public ICollection<TKey> Keys => keyValuePairs.Select(key => key.key).ToList();
	public ICollection<TValue> Values => keyValuePairs.Select(key => key.value).ToList();

	public int Count => keyValuePairs.Count;
	public bool IsReadOnly => false;
	public TValue this[TKey key] {
		get => keyValuePairs.Find(kvPair => kvPair.key.Equals(key)).value;
		set => keyValuePairs.Find(kvPair => kvPair.key.Equals(key)).value = value;
	}

	public bool TryGetValue(TKey key, out TValue value) {
		value = default;
		var haskey = keyValuePairs.Any(val => val.key.Equals(key));
		if (haskey) {
			value = keyValuePairs.First(val => val.key.Equals(key)).value;
		}
		return haskey;
	}
	public void Add(TKey key, TValue value) {
		if (!Keys.Contains(key)) {
			keyValuePairs.Add(new(key, value));
		} else {
			throw new InvalidOperationException($"key: \"{key}\" already exists in KeyValuePairList");
		}
	}
	public bool ContainsKey(TKey key) => Keys.Contains(key);
	public bool Remove(TKey key) {
		var kvp = keyValuePairs.Find(kvPair => kvPair.key.Equals(key));
		if (kvp != null) {
			keyValuePairs.Remove(kvp);
			return true;
		} else {
			return false;
		}
	}

	public void Add(KeyValuePair<TKey, TValue> item) {
		Add(item.Key, item.Value);
	}
	public void Clear() {
		keyValuePairs.Clear();
	}
	public bool Contains(KeyValuePair<TKey, TValue> item) {
		foreach (var kvP in keyValuePairs) {
			if (kvP.Equals(item)) {
				return true;
			}
		}
		return false;
	}
	public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
		throw new NotImplementedException();
	}
	public bool Remove(KeyValuePair<TKey, TValue> item) {
		throw new NotImplementedException();
	}
	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
		throw new NotImplementedException();
	}
	IEnumerator IEnumerable.GetEnumerator() => keyValuePairs.GetEnumerator();
}

[Serializable]
public class KeyValuePairList {
	[Serializable]
	public class KeyValuePairRef { }
}
