using UnityEngine;
using System.Collections.Generic;

public class PathfindingTarget : MonoBehaviour {

	private Grid gridScript;
	private float distFromNode;
	private Vector3 currentNodePos;

	private List<AI> mySeekers = new List<AI>();

	private delegate void PathfindingDelegate();
	private event PathfindingDelegate onMovedToOtherNode;

	void Start () {
		gridScript = FindObjectOfType<Grid> ();
		distFromNode = gridScript.nodeRadius * 2;
	}

	void Update(){
		Vector3 newNodePos = gridScript.NodeFromWorldPoint (transform.position).worldPosition;
		if(newNodePos != currentNodePos){
			UpdateSeekerTargetPos (newNodePos);
		}
	}

	void UpdateSeekerTargetPos(Vector3 newTargetNodePos){
		currentNodePos = newTargetNodePos;

		if(onMovedToOtherNode != null)
			onMovedToOtherNode ();
	}
	
	public void AddSeeker(AI seeker){
		mySeekers.Add (seeker);
		onMovedToOtherNode += seeker.FindPathToTarget;
	}
	public void RemoveSeeker(AI seeker){
		mySeekers.Remove (seeker);
		onMovedToOtherNode -= seeker.FindPathToTarget;
	}
}
