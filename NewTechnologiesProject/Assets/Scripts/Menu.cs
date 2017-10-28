using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

	public Text pauseText;
	public string pausedButtonText = "Unpause (start)";
	public string unpausedButtonText = "Pause (start)";
	public CanvasGroup gameOverPanel;
	public Text gameOverText;

	void Start(){
		pauseText.text = unpausedButtonText;
		GameManager.Instance.InitMenu (this);
	}

	void Update(){
		if(Input.GetButtonDown("Start")){
			if (!GameManager.Instance.gameOver)
				TogglePause ();
			else
				GameManager.Instance.Restart ();
		}
	}

	public void TogglePause(){
		GameManager.Instance.TogglePause ();
		pauseText.text = GameManager.Instance.paused ? pausedButtonText : unpausedButtonText;
	}

	public void ShowGameOver(string killer){
		pauseText.transform.parent.gameObject.SetActive (false);
		gameOverPanel.alpha = 1f;

		if (killer != "")
			gameOverText.text = "Game Over: \n" + killer + " killed you!";
	}
}
