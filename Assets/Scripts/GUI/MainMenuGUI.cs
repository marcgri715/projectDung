using UnityEngine;
using System.Collections;

public class MainMenuGUI : MonoBehaviour {

	public Texture2D background;
	public GUISkin skin;

	// Use this for initialization
	void Start () {
	
	}

	private void OnGUI() {
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), background);
		GUI.skin = skin;
		GUI.Window (0, new Rect (Screen.width / 2 - 160, Screen.height / 2 - 50, 320, 100),
		           menuFunc, "Menu główne");
		GUI.backgroundColor = Color.black;
	}


	private void menuFunc(int id) {
		if (GUILayout.Button("Nowa gra")) {
			resetSave();
			loadGame();
			Application.LoadLevel(2);
		}
		if (GUILayout.Button("Kontynuuj")) {
			loadGame();
			Application.LoadLevel(2);
		}
	}

	void resetSave() {
		PlayerPrefs.SetInt ("isSaved", 0);
	}

	void loadGame() {
		if (PlayerPrefs.GetInt("isSaved") == 1) {
			StatsClass.gold = (uint)PlayerPrefs.GetInt ("gold");
			StatsClass.experience = (uint)PlayerPrefs.GetInt("exp");
			StatsClass.maxDungeonLevel = PlayerPrefs.GetInt("maxLevel");
		} else {
			StatsClass.gold = 0;
			StatsClass.experience = 0;
			StatsClass.maxDungeonLevel = 0;
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
