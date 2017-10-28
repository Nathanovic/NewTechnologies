using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

	private Animator anim;

	void Start () {
		anim = GetComponent<Animator> ();
	}
	
	public void OpenMe(){
		anim.SetBool ("open", true);
	} 

	public void CloseMe(){
		anim.SetBool ("open", false);
	}
}
