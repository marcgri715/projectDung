/* 
 * Imię i nazwisko autora: Marcin Griszbacher
 * Temat pracy: Gra RPG stworzona w oparciu o silnik Unity
 * 
 * Klasa MainMenuGUI
 * Zadanie: skrypt wyświetlający menu główne gry. Skrypt wczytuje bądź resetuje stan gry (ilość złota, doświadczenia
 * 			oraz najdalszy poziom, na który dotarła postać).
 */

using UnityEngine;
using System.Collections;

public class MainMenuGUI : MonoBehaviour {

	public Texture2D background;
	private bool isSaved;

	void Start () {
		if (PlayerPrefs.GetInt ("isSaved") == 1)
			isSaved = true;
		else
			isSaved = false;
	}

	private void OnGUI() {
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), background);
		//Wyświetlenie okna z menu głównym gry
		GUI.Window (0, new Rect (Screen.width / 2 - 160, Screen.height / 2 - 50, 320, 100),
		           menuFunc, "Menu główne");
		GUI.backgroundColor = Color.black;
		GUI.Label (new Rect (Screen.width * 0.8f, Screen.height * 0.9f, Screen.width * 0.2f, Screen.height * 0.1f),
		           "© 2014, Marcin Griszbacher");
	}


	private void menuFunc(int id) {
		//Przycisk nowej gry resetuje zapis
		if (GUILayout.Button("Nowa gra")) {
			resetSave();
			loadGame();
			Application.LoadLevel(2);
		}
		if (!isSaved)
			GUI.enabled = false;
		//Przycisk kontynuacji tylko go wczytuje
		if (GUILayout.Button("Kontynuuj")) {
			loadGame();
			Application.LoadLevel(2);
		}
		GUI.enabled = true;
	}

	//Resetuje preferencję mówiącą o tym, czy gra była zapisana
	void resetSave() {
		PlayerPrefs.SetInt ("isSaved", 0);
		PlayerPrefs.Save ();
	}

	//Wczytanie zapisu gry
	void loadGame() {
		//Jeśli gra była zapisana, wczytywane są wszystkie wartości
		if (PlayerPrefs.GetInt("isSaved") == 1) {
			StatsClass.gold = (uint)PlayerPrefs.GetInt ("gold");
			StatsClass.experience = (uint)PlayerPrefs.GetInt("exp");
			StatsClass.maxDungeonLevel = PlayerPrefs.GetInt("maxLevel");
		//Jeśli gra nie była zapisana, przypisywane są wartości początkowe
		} else {
			StatsClass.gold = 0;
			StatsClass.experience = 0;
			StatsClass.maxDungeonLevel = 1;
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
