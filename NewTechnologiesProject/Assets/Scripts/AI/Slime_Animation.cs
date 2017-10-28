using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime_Animation : MonoBehaviour {
	Rigidbody rb;
	float MoveSpeed = 50.0f;
	float rotationspeed = 50.0f;
	Animator animator;		
	// Use this for initialization
	void Start () {
		rb = this.GetComponent<Rigidbody> ();
		animator = GetComponentInChildren<Animator> ();
		animator.SetBool ("Idling", true);
	}
	
	// Update is called once per frame
	void LateUpdate () {
		float translation = Input.GetAxis ("Vertical") * MoveSpeed;
		float rotation = Input.GetAxis ("Horizontal") * rotationspeed;
		translation *= Time.deltaTime;
		rotation *= Time.deltaTime;
		Quaternion turn = Quaternion.Euler (0f,rotation,0f);
		rb.MoveRotation (rb.rotation * turn);
		//animator.SetFloat ("speedMult", translation);

		if(translation !=0)
			animator.SetBool ("Idling", false);
		else
			animator.SetBool ("Idling", true);
	}
	}
	

