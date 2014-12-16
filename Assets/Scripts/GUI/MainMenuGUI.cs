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
		           menuFunc, "Main menu");
	}


	private void menuFunc(int id) {
		if (GUILayout.Button("Graj")) {
			Application.LoadLevel(1);
		}
		if (GUILayout.Button("Wyjdź")) {
			Application.Quit();
		}
	}
	// Update is called once per frame
	void Update () {
	
	}
}
