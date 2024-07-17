using System;
using System.Threading;
using UnityEngine;
public class Counter : IDisposable {
	private readonly int _startValue;
	private readonly int _endValue;
	private readonly float _countSpeed;
	private readonly int _increment;
	public Action<int> countUpdated;

	private CancellationTokenSource _ct = new();

	public Counter(int oldVal, int newVal, int increment, float speed) {
		_startValue = oldVal;
		_endValue = newVal;
		_increment = increment;
		_countSpeed = speed;
	}
	public async void Start() {
		_ct.Cancel();
		_ct = new();

		try {
			await CountUp(_startValue, _endValue, _increment, _countSpeed);
		}
		catch (OperationCanceledException e) {
			Debug.LogException(e);
		}
	}
	public async Awaitable CountUp(int oldVal, int newVal, int increment, float speed) {
		var currentVal = oldVal;
		while (currentVal < newVal) {
			currentVal += increment;
			currentVal = Mathf.Clamp(currentVal, oldVal, newVal);
			countUpdated?.Invoke(currentVal);
			await Awaitable.WaitForSecondsAsync(speed, _ct.Token);
		}
	}
	public void Dispose() {
		countUpdated = null;
	}
}