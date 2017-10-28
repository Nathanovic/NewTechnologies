using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Pathfinding : MonoBehaviour {

	PathRequestManager requestManager;
	Grid grid;

	void Awake(){
		requestManager = GetComponent<PathRequestManager> ();
		grid = GetComponent<Grid> ();
	}

	public void StartFindPath(Vector3 startPos, Vector3 endPos){
		StartCoroutine (FindPath(startPos, endPos));
	}

	IEnumerator FindPath(Vector3 startPos, Vector3 endPos){ 
		Vector3[] waypoints = new Vector3[0];
		bool validPath = false;

		Node startNode = grid.NodeFromWorldPoint (startPos);
		Node targetNode = grid.NodeFromWorldPoint (endPos);

		if (targetNode.walkable) {
			Heap<Node> openSet = new Heap<Node> (grid.MaxSize);
			HashSet<Node> closedSet = new HashSet<Node> ();
			openSet.Add (startNode);

			while (openSet.Count > 0) {
				Node currentNode = openSet.RemoveFirst ();
				closedSet.Add (currentNode);

				if (currentNode == targetNode) {
					validPath = true;
					break;
				}


				foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
					if (!neighbour.walkable || closedSet.Contains (neighbour)) {
						continue;
					}

					int newMovementCostToNeighbour = currentNode.gCost + GetDistance (currentNode, neighbour);
					if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains (neighbour)) {
						neighbour.gCost = newMovementCostToNeighbour;
						neighbour.hCost = GetDistance (neighbour, targetNode);
						neighbour.parent = currentNode;

						if (!openSet.Contains (neighbour)) {
							openSet.Add (neighbour);
						}
						else {
							openSet.UpdateItem (neighbour);
						}
					}
				}
			}
		}

		yield return null;
		if(validPath){
			waypoints = RetracePath (startNode, targetNode);
		}
		requestManager.FinishedProcessingPath (waypoints, validPath);
	}

	Vector3[] RetracePath(Node startNode, Node endNode){
		List<Node> path = new List<Node> ();
		Node currentNode = endNode;

		while(currentNode != startNode){
			path.Add (currentNode);
			currentNode = currentNode.parent;
		}

		Vector3[] waypoints = SimplifyPath (path);
		Array.Reverse (waypoints);
		return waypoints;
	}

	Vector3[] SimplifyPath(List<Node> path){
		List<Vector3> waypoints = new List<Vector3> ();
		Vector2 directionOld = Vector2.zero;
		int pathCount = path.Count;

		for(int i = 1; i < pathCount; i ++){
			Vector2 directionNew = new Vector2 (path[i-1].gridX - path[i].gridX, path[i-1].gridY - path[i].gridY);
			if(directionNew != directionOld){
				waypoints.Add (path[i].worldPosition);
			}
			directionOld = directionNew;
		}

		return waypoints.ToArray ();
	}

	int GetDistance(Node nodeA, Node nodeB){
		int dstX = Mathf.Abs (nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs (nodeA.gridY - nodeB.gridY);

		if (dstX > dstY) {
			return 14 * dstY + 10 * (dstX - dstY);
		}
		return 14 * dstX + 10 * (dstY - dstX);
	}
}
