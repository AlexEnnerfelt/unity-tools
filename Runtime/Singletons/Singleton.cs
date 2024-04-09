using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component {
	public static bool HasInstance => _instance != null;

	protected static T _instance;
	protected bool _enabled;
	public bool dontDestroyOnLoad = true;

	/// <summary>
	/// Singleton design pattern
	/// </summary>
	/// <value>The instance.</value>
	public static T Instance {
		get {
			if (_instance == null) {
				_instance = FindAnyObjectByType<T>(FindObjectsInactive.Include);
			}
			return _instance;
		}
	}

	/// <summary>
	/// On awake, we check if there's already a copy of the object in the scene. If there's one, we destroy it.
	/// </summary>
	public virtual void Awake() {
		InitializeSingleton();
	}

	/// <summary>
	/// Initializes the singleton.
	/// </summary>
	protected virtual void InitializeSingleton() {
		//if (!Application.isPlaying) {
		//	return;
		//}

		if (_instance == null) {
			//If I am the first instance, make me the Singleton
			_instance = this as T;
			if (dontDestroyOnLoad) {
				DontDestroyOnLoad(transform.gameObject);
			}
			_enabled = true;
		} else {
			//If a Singleton already exists and you find
			//another reference in scene, destroy it!
			if (this != _instance) {
				Destroy(gameObject);
			}
		}
	}
}