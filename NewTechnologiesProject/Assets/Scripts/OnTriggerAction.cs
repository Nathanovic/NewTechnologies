using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class OnTriggerAction : MonoBehaviour {
	public UnityEvent onTriggerEvent;

	void OnTriggerEnter(Collider other){
		if (other.tag == "Player" && onTriggerEvent != null)
			onTriggerEvent.Invoke ();
	}
}
