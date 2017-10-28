using UnityEngine;
using System.Collections;
using Tobii.Gaming;

public class LevitationController : MonoBehaviour {

	private Camera mainCam;

	public bool usingEyeTracker;

	public Power powerScript = new Power();
	private Levitationable movableObject;
	private Levitationable frozenObject;
	public bool IsLevitating{
		get{ 
			if (movableObject == null)
				return false;
			else
				return true;
		}
	}

	public float moFromCamSpeed = 3f;
	private float moDistFromCam;
	public float levitationMaxReach = 10f;
	public float levitationMinReach = 1f;

	public float liftUpHeight = 0.8f;
	public float liftUpDuration = 0.75f;
	private float startLiftingTime;
	private float liftSpeed;

	private LayerMask levitationableLM;

	public GazeSettings gazeScript = new GazeSettings();
	private Levitationable gazedAtObject;

	public HUD hudScript;

	void Start(){
		mainCam = Camera.main;
		powerScript.Init ();

		levitationableLM = LayerMask.GetMask ("Levitationable");

		Levitationable[] allLevitationables = FindObjectsOfType<Levitationable> ();
		foreach(Levitationable l in allLevitationables){
			l.onGazeAtMe += RegisterGazedAtObject;
		}

		hudScript = FindObjectOfType<HUD> ();

		liftSpeed = liftUpHeight / liftUpDuration;
	}

	void Update(){
		if (GameManager.Instance.paused)
			return;

		if (IsLevitating) {
			bool release = ObjectReleaseInput ();
			bool carryOn = powerScript.CarryItem (movableObject.itemWeight);
			if (release || !carryOn) {
				DropMovableObject ();
			} 
			else {
				MoveObjectBehaviour ();
				FreezeObjectBehaviour ();
			}
		} else {
			SelectObjectBehaviour ();
			powerScript.RegeneratePower ();
		}
	}

	void SelectObjectBehaviour(){
		if (!usingEyeTracker && Input.GetMouseButtonDown (0)) {
			Ray mouseRay = mainCam.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (mouseRay, out hit, levitationMaxReach, levitationableLM) && powerScript.RemainingPower > 0f) {
				Transform hittedObject = hit.transform;
				LiftMovableObject (hittedObject);
			}
		}else if(usingEyeTracker){
			if (Input.GetButtonDown("MOHold") && gazedAtObject != null) {//&& de speler kijkt al x seconden naar MO
				LiftMovableObject (gazedAtObject.transform);
			}
		}
	}

	void RegisterGazedAtObject(Levitationable sender, LevitationOptions[] senderInfo = null){
		if (movableObject == null) {
			sender.onGazeAtMe -= RegisterGazedAtObject;
			sender.onGazeFromMe += DeregisterGazedAtObject;
			gazedAtObject = sender;
			hudScript.EnableHUD (sender.transform.position, sender.name, senderInfo);
		}
	}
	void DeregisterGazedAtObject(Levitationable sender){
		if(movableObject == null){
			sender.onGazeAtMe += RegisterGazedAtObject;
			sender.onGazeFromMe -= DeregisterGazedAtObject;
			gazedAtObject = null;
			hudScript.DisableHUD ();
		}
	}

	bool ObjectReleaseInput(){
		if(!usingEyeTracker){
			return Input.GetMouseButtonUp(0);
		}else{
			return Input.GetButtonUp("MOHold");
		}
	}
	void MoveObjectBehaviour(){
		//lift levitation:
		if ((Time.time - startLiftingTime) < liftUpDuration) {
			Vector3 moPos = movableObject.transform.position;
			Vector3 camPos = mainCam.transform.position;
			float fromCamDist = Vector3.Distance(moPos, camPos);

			movableObject.Lift (liftSpeed);
			moDistFromCam = fromCamDist;

			if(usingEyeTracker)
				gazeScript.UpdateGazePointSimple ();
		}
		else{//eye levitation:
			float triggerInput = Input.GetAxis ("MOTrigger");
			if(triggerInput != 0){
				Vector3 moPos = movableObject.transform.position;
				Vector3 camPos = mainCam.transform.position;
				float fromCamDist = Vector3.Distance(moPos, camPos);

				Vector3 fromCamDir = (moPos - camPos).normalized;
				if((triggerInput > 0f && fromCamDist < levitationMaxReach) ||
					(triggerInput < 0f && fromCamDist > levitationMinReach)){
					float fromCamDistModifier = triggerInput * moFromCamSpeed * Time.deltaTime;
					moDistFromCam += fromCamDistModifier;
				}
			}

			Ray controllingRay = new Ray ();
			if(!usingEyeTracker){
				controllingRay = mainCam.ScreenPointToRay (Input.mousePosition);
			}else{
				Vector2? gazePoint = gazeScript.ValidGazePoint ();
				if (gazePoint != null) {
					Vector2 currentPoint = (Vector2)gazePoint;
					controllingRay = mainCam.ScreenPointToRay (currentPoint);
				} else {
					DropMovableObject ();
					return;
				}
			}

			Vector3 newPos = controllingRay.origin + controllingRay.direction * moDistFromCam;
			bool validMovement = true;
			movableObject.Levitate (newPos, out validMovement);
			if(!validMovement)
				gazeScript.UnvalidObjectMovement ();
		}
	}

	void FreezeObjectBehaviour(){
		if (Input.GetButtonDown ("MOFreeze")) {
			if (frozenObject != null)
				frozenObject.DropMe ();
			
			movableObject.FreezeMe ();
			frozenObject = movableObject;
			movableObject = null;
		}
	}

	void LiftMovableObject(Transform mo){
		float _moDistFromCam = Vector3.Distance (mo.position, mainCam.transform.position);
		Levitationable target = mo.GetComponent<Levitationable> ();
		if (_moDistFromCam > levitationMinReach && _moDistFromCam < levitationMaxReach && !target.IsFloating) {	
			movableObject = target;
			if (frozenObject == movableObject) {
				frozenObject = null;
			}else if (target.AbsYVel() < 0.1f) {
				startLiftingTime = Time.time;
			}
			moDistFromCam = _moDistFromCam;
			movableObject.LiftMeUp ();

			if(usingEyeTracker)
				StartCoroutine(gazeScript.Init ());
		}

		hudScript.DisableHUD ();
	}
	void DropMovableObject(){
		DeregisterGazedAtObject (movableObject);
		movableObject.DropMe ();
		movableObject = null;
	}

	public void ThrowObject(float throwSpeed){
		if (IsLevitating) {
			Vector3 force = (movableObject.transform.position - mainCam.transform.position).normalized * throwSpeed;
			movableObject.ThrowMe (force);
			DropMovableObject ();
		}
	}
}

[System.Serializable]
public class GazeSettings{
	public float maxScreenDelta = 50;
	public float currentScreenDelta;

	private Vector2 backupPoint;
	private Vector2 lastPoint;
	public float filterPointValue = 0.5f;
	private bool initialized;

	public IEnumerator Init(){
		initialized = false;

		while(!TobiiAPI.GetUserPresence().IsUserPresent()){
			yield return 0;
		}

		initialized = true;
		lastPoint = TobiiAPI.GetGazePoint ().Screen;
		backupPoint = lastPoint;
	}

	public void UpdateGazePointSimple(){
		Vector2 filteredPoint = Vector2.Lerp(lastPoint, TobiiAPI.GetGazePoint ().Screen, filterPointValue);
		currentScreenDelta = Vector2.Distance (lastPoint, filteredPoint);
		lastPoint = filteredPoint;
	}

	public Vector2? ValidGazePoint(){
		if (!initialized) {
			return null;
		}

		Vector2 filteredPoint = Vector2.Lerp(lastPoint, TobiiAPI.GetGazePoint ().Screen, filterPointValue);
		currentScreenDelta = Vector2.Distance (lastPoint, filteredPoint);
		lastPoint = filteredPoint;

		if (currentScreenDelta < maxScreenDelta && TobiiAPI.GetUserPresence ().IsUserPresent ()) {
			return filteredPoint;
		} else {
			return null;
		}
	}

	//wordt aangeroepen als het object niet kan bewegen ivm collision: !ValidMovement in Levitationable
	public void UnvalidObjectMovement(){
		lastPoint = backupPoint;
	}
}
