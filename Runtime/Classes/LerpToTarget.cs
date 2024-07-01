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

	public void DoLerpToTarget(GameObject target) {
		lerpTarget = target.transform;
		Lerp();
	}

	private async void Lerp() {

		moveTarget.DOPunchScale(new(punchScale, punchScale), punchDuration);

		while (true) {
			moveTarget.position = Vector2.Lerp(moveTarget.position, lerpTarget.position, lerpSpeed * Time.deltaTime);
			try {
				await Awaitable.NextFrameAsync(destroyCancellationToken);
			}
			catch (System.Exception) {
				break;
			}
		}

	}


}
