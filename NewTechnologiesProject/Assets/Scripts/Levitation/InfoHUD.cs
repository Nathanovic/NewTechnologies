using UnityEngine;
using System.Collections;
using Tobii.Gaming;

public class InfoHUD : MonoBehaviour {

	private GazeAware gazeAware;
	public bool HasFocus{
		get{ 
			return gazeAware.HasGazeFocus;
		}
	}

	void Start () {
		gazeAware = GetComponent<GazeAware> ();
	}
}
