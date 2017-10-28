using UnityEngine;
using System.Collections;

public class AI : MonoBehaviour {

	//target is responsible for updating its own targetposition, and if he does, he calls the FindPathToTarget method with an event
	private PathfindingTarget target;
	public float foundWaypointDist = 0.1f;
	public float speed = 2.5f;
	private Vector3[] path;
	private int targetIndex;

	public delegate void EnemyDelegate (AI sender);
	public event EnemyDelegate onEnemyDie;

	private Levitationable levitationable;

	public bool attackOnStart;
	private bool attacking;

	private Animator anim;

	void Start(){
		target = FindObjectOfType<PathfindingTarget> ();
		AIManager.instance.SubscribeEnemy (this);
		levitationable = GetComponent<Levitationable> ();
		levitationable.attackOnRelease += Attack;

		anim = GetComponent<Animator> ();

		if (attackOnStart) {
			Attack ();
		}
	}

	//called by AIManager:
	public void Attack(){
		if (attacking == false) {
			attacking = true;
			target.AddSeeker (this);
			FindPathToTarget ();
		}
	}

	public void FindPathToTarget() {
		PathRequestManager.RequestPath(transform.position, target.transform.position, OnPathFound);
	}
	
	public void OnPathFound(Vector3[] newPath, bool validPath){
		StopCoroutine ("FollowPath");

		if (validPath && newPath.Length > 0) {
			path = newPath;
			StartCoroutine ("FollowPath");
		}
		else {
			if (!validPath) {
//				print ("stop attacking: unvalid path");
//				StopAttacking ();
				Invoke("FindPathToTarget", 0.5f);
			}
		}
	}

	IEnumerator FollowPath(){
		Vector3 currentWaypoint = path [0];

		anim.SetFloat ("moveSpeed", speed);
		while(true){
			if (levitationable == null || levitationable.CanControlSelf) {
				if (Vector3.Distance (transform.position, currentWaypoint) < foundWaypointDist) {
					targetIndex++;
					if (targetIndex >= path.Length) {
						break;
					}
					currentWaypoint = path [targetIndex];
				}

				currentWaypoint.y = transform.position.y;
				transform.position = Vector3.MoveTowards (transform.position, currentWaypoint, speed * Time.deltaTime);
			}
			else {
				StopAttacking ();
				break;
			}
			yield return null;
		}

		anim.SetFloat ("moveSpeed", 0f);
	}

	void StopAttacking(){
		target.RemoveSeeker (this);
		attacking = false;
	}

	void OnDrawGizmos(){
		if(path != null){
			for(int i = targetIndex; i < path.Length; i ++){
				Gizmos.color = Color.black;
				Gizmos.DrawCube (path[i], Vector3.one);

				if (i == targetIndex) {
					Gizmos.DrawLine (transform.position, path [i]);
				}
				else {
					Gizmos.DrawLine (path [i - 1], path [i]);
				}
			}
		}
	}
}
