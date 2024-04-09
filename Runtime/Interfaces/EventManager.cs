using System;

public class EventManager<T> {
	public static T _event { get; protected set; }
	public static event Action<T> onraised;

	public static void AddListener(Action<T> listener) {
		onraised += listener;
	}
	public static void RemoveListener(Action<T> listener) {
		onraised -= listener;
	}
	public static void Trigger(T arg) {
		_event = arg;
		onraised?.Invoke(_event);
	}
}