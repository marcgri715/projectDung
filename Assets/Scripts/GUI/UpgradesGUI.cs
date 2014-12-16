using UnityEngine;
using System.Collections;

public class UpgradesGUI : MonoBehaviour {
	
	public Texture2D background;
	public GUISkin skin;

	
	// Use this for initialization
	void Start () {
		
	}
	
	private void OnGUI() {
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), background);
		GUI.skin = skin;
		//GUI.Window (0, new Rect (Screen.width / 2 - 160, Screen.height / 2 - 50, 320, 100),
		//            menuFunc, "Main menu");
		GUI.backgroundColor = Color.black;
	}

	// Update is called once per frame
	void Update () {
		
	}
}
