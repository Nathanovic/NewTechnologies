using UnityEngine;
using System.Collections.Generic;

public class AIManager : MonoBehaviour {

	public static AIManager instance;
	private List<AI> myEnemies = new List<AI> ();

	void Awake(){
		instance = this;
	}

	public void AlertEnemies(){
		if (myEnemies.Count > 0) {
			foreach (AI enemy in myEnemies) {
				enemy.Attack ();
			}
		}
	}

	//subscribed to by enemies
	public void SubscribeEnemy(AI enemy){
		myEnemies.Add (enemy);
		enemy.onEnemyDie += OnEnemyKilled;
	}

	void OnEnemyKilled(AI sender){
		myEnemies.Remove (sender);
	}
}
