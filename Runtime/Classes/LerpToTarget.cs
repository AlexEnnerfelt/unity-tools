using System.Threading;
using DG.Tweening;
using UnityEngine;

public class LerpToTarget : MonoBehaviour {

	public float lerpSpeed = 1f;
	public Transform moveTarget;
	private Transform lerpTarget;

	public float punchScale = 1.1f;
	public float punchDuration = 0.2f;

	public void Awake() {
		if (moveTarget == null) {
			moveTarget = transform;
		}
	}

	public void OnDisable() {
		ct.Cancel();
	}

	public void DoLerpToTarget(GameObject target) {
		lerpTarget = target.transform;
		Lerp();
	}

	private CancellationTokenSource ct = new();
	private async void Lerp() {
		ct.Cancel();
		ct = new();
		moveTarget.DOPunchScale(new(punchScale, punchScale), punchDuration);

		while (true) {
			moveTarget.position = Vector2.Lerp(moveTarget.position, lerpTarget.position, lerpSpeed * Time.deltaTime);
			try {
				await Awaitable.NextFrameAsync(ct.Token);
			}
			catch (System.Exception) {

				break;
			}
		}

	}


}
