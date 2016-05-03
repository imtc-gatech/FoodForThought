using UnityEngine;
using System.Collections;

public class csTweenManager : MonoBehaviour {

	// Quickie for linear motion tweens without easing, position only.
	// If we need more than this, ITween is the obvious solution, but it can get into
	// licensing issues depending on how you use it.  GreenSock is another, which does
	// have cheap commercial licensing.

	public IEnumerator PositionTweenTo (GameObject objToTween, Vector3 destPos, float timeInSeconds) {
		yield return StartCoroutine (PositionTweenTo(objToTween.transform, objToTween.transform.position, destPos, timeInSeconds));
	}

	public IEnumerator PositionTweenTo (Transform transToTween, Vector3 destPos, float timeInSeconds) {
		yield return StartCoroutine (PositionTweenTo(transToTween, transToTween.position, destPos, timeInSeconds));
	}

	public IEnumerator PositionTweenTo (GameObject objToTween, Vector3 sourcePos, Vector3 destPos, float timeInSeconds) {
		yield return StartCoroutine (PositionTweenTo(objToTween.transform, sourcePos, destPos, timeInSeconds));
	}

	public IEnumerator PositionTweenTo (Transform transToTween, Vector3 sourcePos, Vector3 destPos, float timeInSeconds) {
		// Tween the transform from the start to the end position.  Linear path, no easing.
		float timeRemaining = timeInSeconds;
		while (timeRemaining > 0) {
			transToTween.position = Vector3.Lerp (sourcePos, destPos, 1.0f - (timeRemaining / timeInSeconds));
			timeRemaining -= Time.deltaTime;
			yield return null;
		}

		transToTween.position = destPos;
		yield return null;
	}


	// Quick transform "shake" for user feedback.
	public IEnumerator ShakePosition (Transform transToShake) {
		float SHAKE_MAGNITUDE = 3.0f;
		float SHAKE_TIME = 0.025f;
		Vector3 origPos = transToShake.position;

		yield return StartCoroutine (PositionTweenTo (transToShake, origPos + (Vector3.left * SHAKE_MAGNITUDE), SHAKE_TIME));
		yield return StartCoroutine (PositionTweenTo (transToShake, origPos + (Vector3.right * SHAKE_MAGNITUDE), SHAKE_TIME * 2));
		yield return StartCoroutine (PositionTweenTo (transToShake, origPos + (Vector3.left * SHAKE_MAGNITUDE), SHAKE_TIME));
		yield return StartCoroutine (PositionTweenTo (transToShake, origPos + (Vector3.right * SHAKE_MAGNITUDE), SHAKE_TIME * 2));
		yield return StartCoroutine (PositionTweenTo (transToShake, origPos, SHAKE_TIME));

		transToShake.position = origPos;

		yield return null;
	}

}
