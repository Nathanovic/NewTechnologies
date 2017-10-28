using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
	
	private Transform playerCam;

	[SerializeField]private CanvasGroup objectDescriptionCVG;
	[SerializeField]private Text objectName;
	[SerializeField]private Transform[] objectDescrTransforms;
	private Text[] objectDescription;
	[SerializeField]private int upperDescrYPos = 0;
	[SerializeField]private int objDescrYOffset = -17;

	void Start () {
		playerCam = Camera.main.transform;

		int objDescrLength = objectDescrTransforms.Length;
		objectDescription = new Text[objDescrLength];
		for(int i = 0; i < objDescrLength; i ++){
			objectDescription[i] = objectDescrTransforms [i].GetComponent<Text> ();
		}

		DisableHUD ();
	}
	
	void Update () {
		Vector3 lookDir = transform.position - playerCam.position;
		lookDir.y = 0;
		transform.rotation = Quaternion.LookRotation (lookDir);
	}

	public void EnableHUD(Vector3 newPos, string objName, LevitationOptions[] objectControlInfo){
		transform.position = newPos;

		objectDescriptionCVG.alpha = 1f;
		objectName.text = objName;

		int objInfoCount = objectControlInfo.Length;
		for(int i = 0; i < objInfoCount; i ++){
			Vector3 infoLocalPos = objectDescrTransforms [i].localPosition;
			infoLocalPos.y = upperDescrYPos + objDescrYOffset * i;

			objectDescrTransforms [i].localPosition = infoLocalPos;
			objectDescription [i].text = "- " + objectControlInfo [i].ToString();
			objectDescription [i].enabled = true;
		}
	}

	public void DisableHUD(){
		objectDescriptionCVG.alpha = 0f;

		foreach(Text hudInfoT in objectDescription){
			hudInfoT.enabled = false;
		}
	}
}
