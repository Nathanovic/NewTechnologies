using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class PhysicalButton : MonoBehaviour {

	public string[] validTags = { "Movable", "Player" };

	private Vector3 releasedPosition;
	private Vector3 pressedPosition;
	public float pressedYPosOffset = -0.1f;

	private List<Collider> onMeColliders = new List<Collider>();
	public float releaseWaitTime = 1f;

	private ButtonState state = ButtonState.up;
	public UnityEvent onPressed;
	public UnityEvent onRelease;

	void Start(){
		UnityAction onDown = PushDown;
		UnityAction onUp = Release;
		onPressed.AddListener (onDown);
		onRelease.AddListener (onUp);

		releasedPosition = transform.position;
		pressedPosition = releasedPosition + Vector3.up * pressedYPosOffset;
	}

	void OnCollisionEnter(Collision col){
		Collider otherColl = col.collider;
		if (!onMeColliders.Contains(otherColl) && ValidCollision (otherColl.tag)) {
			onMeColliders.Add (otherColl);
			if (state == ButtonState.up)
				onPressed.Invoke();
		}
	}
	void OnCollisionExit(Collision col){
		Collider otherColl = col.collider;
		if (onMeColliders.Contains(otherColl) && ValidCollision (otherColl.tag)) {
			onMeColliders.Remove (otherColl);
			if (onMeColliders.Count == 0 && state == ButtonState.down)
				StartCoroutine (ResetButton());
		}
	}

	bool ValidCollision(string colliderTag){
		foreach(string validTag in validTags){
			if(validTag == colliderTag){
				return true;
			}
		}
		return false;
	}

	IEnumerator ResetButton(){
		float t = 0;
		bool canRelease = true;

		while(t < releaseWaitTime && canRelease){
			t += Time.deltaTime;
			if (onMeColliders.Count > 0)
				canRelease = false;

			yield return 0;
		}

		if(canRelease)
			onRelease.Invoke ();
	}

	void PushDown(){
		state = ButtonState.down;
		transform.position = pressedPosition;
	}
	void Release(){
		state = ButtonState.up;
		transform.position = releasedPosition;
	}

	private enum ButtonState{
		down,
		up
	}
}
