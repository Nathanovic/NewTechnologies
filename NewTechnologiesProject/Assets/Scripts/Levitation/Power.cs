using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class Power {

	[SerializeField]private float maxPower = 20f;
	private float _power;
	public float RemainingPower{
		get{ 
			return _power;
		}
		private set{ 
			if (value > maxPower)
				_power = maxPower;
			else if (value < 0)
				_power = 0;
			else
				_power = value;
		}
	}

	[SerializeField]private float fullRegenTime = 10f;
	private float powerRegeneration;
	[SerializeField]private float weightPowerCostFactor = 0.2f;

	[SerializeField]private RectTransform powerBar;
	private Vector2 pbStartSizeDelta;

	public void Init () {
		RemainingPower = maxPower;
		powerRegeneration = maxPower / fullRegenTime;
		pbStartSizeDelta = powerBar.sizeDelta;
	}

	public void RegeneratePower(){
		RemainingPower += powerRegeneration * Time.deltaTime;
		UpdatePowerBar ();
	}

	//if Power runs out, the player will have to drop it
	public bool CarryItem(float itemWeight){
		if (RemainingPower == 0) {
			return false;
		}
		else {
			RemainingPower -= itemWeight * weightPowerCostFactor * Time.deltaTime;
			UpdatePowerBar ();
			return true;
		}
	}

	private void UpdatePowerBar () {
		Vector2 pbSizeDelta = pbStartSizeDelta;
		pbSizeDelta.y *= (RemainingPower / maxPower);
		powerBar.sizeDelta = pbSizeDelta;
	}
}
