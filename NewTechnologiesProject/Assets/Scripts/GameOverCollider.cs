using UnityEngine;
using System.Collections;

public class GameOverCollider : MonoBehaviour {

	void OnCollisionEnter (Collision coll) {
		string otherTag = coll.collider.name;
		if(otherTag == "Player" || otherTag == "Movable")
			GameManager.Instance.GameOver ();
	}
}
