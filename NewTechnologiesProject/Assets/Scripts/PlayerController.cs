using UnityEngine;
using System.Collections;

//throwing only
[RequireComponent(typeof(LevitationController))]
public class PlayerController : MonoBehaviour {

	private LevitationController levitationScript;

	public float minThrowSpeed;
	public float deltaThrowSpeed;
	public float maxHoldTime;

	void Start(){
		levitationScript = GetComponent<LevitationController> ();
	}

	void Update(){
		if(Input.GetButtonDown("MOThrow") && levitationScript.IsLevitating){
			StartCoroutine (ThrowBehaviour());
		}
	}
	
	IEnumerator ThrowBehaviour(){
		float currentThrowSpeed = minThrowSpeed;
		float t = 0f;
		while(t < maxHoldTime){
			if(Input.GetButtonUp("MOThrow")){
				break;
			}

			yield return null;
			t += Time.deltaTime;
			currentThrowSpeed += deltaThrowSpeed * maxHoldTime;
		}

		levitationScript.ThrowObject (currentThrowSpeed);
	}
}
