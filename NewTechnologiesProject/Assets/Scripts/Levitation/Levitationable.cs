using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using System.Linq;

public class Levitationable : MonoBehaviour {

	private Rigidbody rb;
	public float itemWeight = 1f;

	//QQQ
	public ObjectState myState = ObjectState.standard;

	public Renderer myRenderer;
	private Material myMat;
	public Material frozenMaterial;
	public Renderer levitationFVXRenderer;

	private float radius;
	public LayerMask unMovableLM;

	private bool playerGazesAtMe;
	private GazeAware gazeAware;

	public delegate void SubScribeDelegate(Levitationable sender, LevitationOptions[] senderInfo = null);
	public delegate void DesubscribeDelegate (Levitationable sender);
	public delegate void ReleasedDelegate();

	public event SubScribeDelegate onGazeAtMe;
	public event DesubscribeDelegate onGazeFromMe;

	public event ReleasedDelegate attackOnRelease; 
	private LevitationOptions[] options;
	public LevitationInfo myInfo;
	public bool IsFloating{
		get{ 
			if (myState == ObjectState.floating)
				return true;
			return false;
		}
	}
	public bool CanControlSelf{//only relevant for ai.cs
		get{ 
			if (myState == ObjectState.standard)
				return true;
			return false;
		}
	}

	private InfoHUD infoHUD;

	private bool isAI;

	void Start () {
		rb = GetComponent<Rigidbody> ();
		gazeAware = GetComponent<GazeAware> ();

		myMat = myRenderer.material;
		levitationFVXRenderer.enabled = false;

		Vector3 renderExtents = myRenderer.bounds.extents;
		radius = Mathf.Max (renderExtents.x, renderExtents.y, renderExtents.z);

		List<LevitationOptions> _options = new List<LevitationOptions> ();
		if (myInfo.canLevitate)
			_options.Add (LevitationOptions.levitating);
		if (myInfo.canFreeze)
			_options.Add (LevitationOptions.freezing);
		if (myInfo.canThrow)
			_options.Add (LevitationOptions.throwing);
		options = _options.ToArray ();
		infoHUD = FindObjectOfType<InfoHUD> ();

		if (transform.tag == "AI")
			isAI = true;
	}

	void Update(){
		if(myState != ObjectState.floating){
			if (!playerGazesAtMe && gazeAware.HasGazeFocus && Mathf.Abs(rb.velocity.magnitude) < 0.5f) {
				onGazeAtMe (this, options);
				playerGazesAtMe = true;
			} 
			else if (playerGazesAtMe && !infoHUD.HasFocus && !gazeAware.HasGazeFocus) {
				onGazeFromMe (this);	
				playerGazesAtMe = false;	
			}
		}

		//QQQ:
		//TestBehaviour ();
	}

	void TestBehaviour(){
		if(transform.name == "Watermelon" && Input.GetKeyUp(KeyCode.X)){
			playerGazesAtMe = !playerGazesAtMe;
			if (playerGazesAtMe)
				onGazeAtMe (this, options);
			else {
				if (onGazeFromMe != null)
					onGazeFromMe (this);
			}
		}
	}

	public void Levitate(Vector3 newLoc, out bool validLevitation){
		if (myInfo.canLevitate) {
			Vector3 offset = newLoc - transform.position;
			Vector3 renderCenter = myRenderer.bounds.center + offset;
			Collider[] colls = Physics.OverlapSphere (renderCenter, radius, unMovableLM);
			if (colls.Length > 0) {	
				foreach (Collider c in colls) {
					if (!c.isTrigger) {
						validLevitation = false;
						return;
					}
				}
			}

			transform.position = newLoc;
		}

		validLevitation = true;
	}

	//start levitation control:
	public void LiftMeUp(){
		if(myState == ObjectState.frozen)
			Unfreeze ();
		rb.useGravity = false;
		myState = ObjectState.floating;
		levitationFVXRenderer.enabled = true;
	}
	//lifting:
	public void Lift(float upSpeed){
		if(myInfo.canLevitate)
			transform.Translate (Vector3.up * upSpeed * Time.deltaTime, Space.World);
	}
	public float AbsYVel(){
		return Mathf.Abs(rb.velocity.y);
	}

	public void DropMe(){
		print ("drop");
		if (attackOnRelease != null)
			attackOnRelease ();
		rb.useGravity = true;
		myState = ObjectState.standard;
		levitationFVXRenderer.enabled = false;
	}

	public void FreezeMe(){
		if (myInfo.canFreeze) {
			rb.isKinematic = true;
			myState = ObjectState.frozen;
			myRenderer.material = frozenMaterial;
			levitationFVXRenderer.enabled = false;

			if (isAI) {
				StopCoroutine ("UnFreezeOverTime");
				StartCoroutine ("UnFreezeOverTime");
			}
		}
		else {
			DropMe ();
		}
	}

	public void ThrowMe(Vector3 force){
		DropMe ();
		print ("try throwing");
		if (myInfo.canThrow) {
			print ("throw me");
			rb.isKinematic = false;
			rb.AddForce (force);
		}
	}

	IEnumerator UnFreezeOverTime(){
		yield return new WaitForSeconds (8);
		DropMe ();
		Unfreeze ();
	}

	void Unfreeze(){
		StopCoroutine ("UnFreezeOverTime");
		rb.isKinematic = false;
		myRenderer.material = myMat;
	}
}

public enum ObjectState{
	standard,
	floating,
	frozen
}

public enum LevitationOptions{
	levitating,
	freezing,
	throwing
}

[System.Serializable]
public class LevitationInfo{
	public bool canLevitate = true;
	public bool canFreeze;
	public bool canThrow;
}