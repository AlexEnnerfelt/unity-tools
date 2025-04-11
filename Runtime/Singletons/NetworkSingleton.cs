using Unity.Netcode;
using UnityEngine;

// public abstract class NetworkSingleton<T> : NetworkSingleton where T : NetworkBehaviour {
// 	#region  Fields

// 	private static T _instance;
// 	[SerializeField]
// 	protected bool persistent = true;

// 	// ReSharper disable once StaticMemberInGenericType
// 	private static readonly object _lock = new();

// 	#endregion

// 	#region  Properties

// 	public static T Instance {
// 		get {
// 			if (Quitting) {
// 				Debug.LogWarning($"[{nameof(NetworkSingleton)}<{typeof(T)}>] Instance will not be returned because the application is quitting.");
// 				// ReSharper disable once AssignNullToNotNullAttribute
// 				return null;
// 			}
// 			lock (_lock) {
// 				if (_instance != null)
// 					return _instance;
// 				var instances = FindObjectsByType<T>(FindObjectsSortMode.None);
// 				var count = instances.Length;
// 				if (count > 0) {
// 					if (count == 1)
// 						return _instance = instances[0];
// 					Debug.LogWarning($"[{nameof(NetworkSingleton)}<{typeof(T)}>] There should never be more than one {nameof(NetworkSingleton)} of type {typeof(T)} in the scene, but {count} were found. The first instance found will be used, and all others will be destroyed.");
// 					for (var i = 1; i < instances.Length; i++)
// 						Destroy(instances[i]);
// 					return _instance = instances[0];
// 				}

// 				return null;
// 			}
// 		}
// 	}
// 	#endregion

// 	#region  Methods
// 	protected virtual void Awake() {
// 		Quitting = false;
// 		if (persistent) {
// 			try {
// 				DontDestroyOnLoad(gameObject);
// 			}
// 			catch (System.Exception) {

// 			}
// 		}
// 	}

// 	#endregion
// }

// public abstract class NetworkSingleton : NetworkBehaviour {

// public static bool Quitting { get; protected set; }
// 	#endregion

// 	#region  Methods
// 	private void OnApplicationQuit() {

// 		Quitting = true;
// 	}
// 	#endregion
// }