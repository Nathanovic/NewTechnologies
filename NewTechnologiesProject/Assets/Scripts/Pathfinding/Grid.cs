using UnityEngine;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

	public bool drawGridGizmos;

	public LayerMask unwalkableMask;
	public LayerMask walkableMask;
	public Vector2 gridWorldSize;
	public float nodeRadius;
	private Node[,] grid;

	private float nodeDiameter;
	private int gridSizeX;
	private int gridSizeY;

	void Awake(){
		//how many nodes can we fit into our grid:
		nodeDiameter = nodeRadius * 2;
		gridSizeX = Mathf.RoundToInt (gridWorldSize.x / nodeDiameter);
		gridSizeY = Mathf.RoundToInt (gridWorldSize.y / nodeDiameter);

		CreateGrid ();
	}

	public int MaxSize{
		get{ 
			return gridSizeX * gridSizeY;
		}
	}

	void CreateGrid(){
		grid = new Node[gridSizeX, gridSizeY];
		//positie voor bottom left gridNode:
		Vector3 worldBottomLeft = new Vector3(-gridSizeX * nodeRadius, 0, -gridSizeY * nodeRadius);

		for(int x = 0; x < gridSizeX; x ++){
			for(int y = 0; y < gridSizeY; y ++){
				Vector3 worldPoint = worldBottomLeft 
					+ Vector3.right * (x * nodeDiameter + nodeRadius)
					+ Vector3.forward * (y * nodeDiameter + nodeRadius);
				bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
				if (walkable && !Physics.CheckSphere (worldPoint, nodeRadius, walkableMask))
					walkable = false;
				grid [x, y] = new Node (walkable, worldPoint, x, y);
			}
		}
	}

	public List<Node> GetNeighbours(Node node){
		List<Node> neighbours = new List<Node> ();

		for (int x = -1; x <= 1; x++) {
			for(int y = -1; y <= 1; y++){
				if (x == 0 && y == 0) {
					continue;
				}

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY){
					neighbours.Add(grid[checkX, checkY]);
				}
			}
		}

		return neighbours;
	}

	public Node NodeFromWorldPoint(Vector3 worldPosition){
		float percentX = (worldPosition.x + gridWorldSize.x * 0.5f) / gridWorldSize.x;
		float percentY = (worldPosition.z + gridWorldSize.y * 0.5f) / gridWorldSize.y;
		percentX = Mathf.Clamp01 (percentX);
		percentY = Mathf.Clamp01 (percentY);

		int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
		return(grid[x,y]);
	}

	void OnDrawGizmos(){
		Gizmos.DrawWireCube (transform.position, new Vector3(gridWorldSize.x, 1f, gridWorldSize.y));

		if (grid != null && drawGridGizmos) {
			foreach (Node n in grid) {
				Color gizmC = (n.walkable) ? Color.green : Color.red;
				gizmC.a = 0.5f;
				Gizmos.color = gizmC;
				Gizmos.DrawCube (n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
			}
		}
	}
}
