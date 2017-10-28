using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	private static GameManager _instance;
	public static GameManager Instance{
		get{ 
			if (_instance == null) {
				GameObject gm = new GameObject ("Game Manager");
				gm.AddComponent<GameManager> ();
			}
			return _instance;
		}
	}

	private Menu menuScript;

	public bool paused{ get; private set;}
	public bool gameOver{ get; private set; }

	void Awake () {
		_instance = this;
		StartCoroutine (Setup ());
	}

	IEnumerator Setup(){
		if (SceneManager.sceneCount == 1){
			yield return SceneManager.LoadSceneAsync (1, LoadSceneMode.Additive);
		}
	}

	public void InitMenu(Menu _menuScript){
		menuScript = _menuScript;
	}

	public void TogglePause(){
		paused = !paused;
		if (paused)
			Time.timeScale = 0f;
		else
			Time.timeScale = 1f;
	}

	public void GameOver(string gameOverText = ""){
		gameOver = true;
		menuScript.ShowGameOver (gameOverText);
		TogglePause ();
	}

	public void Restart(){
		TogglePause ();
		SceneManager.LoadScene (0);
	}
}
