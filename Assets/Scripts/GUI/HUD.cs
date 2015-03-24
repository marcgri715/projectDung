/* 
 * Imię i nazwisko autora: Marcin Griszbacher
 * Temat pracy: Gra RPG stworzona w oparciu o silnik Unity
 * 
 * Klasa HUD
 * Zadanie: skrypt wyświetlający HUD (z ang. head-up display) na ekranie gry. HUD pokazuje pasek życia gracza, 
 * 			liczbę punktów życia, obecny poziom podziemi oraz zdobyte przez gracza doświadczenie i złoto.
 */

using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour {

	//Statystyki gracza
	private PlayerStatsClass playerStats;
	//Parametr z teksturą pustego paska życia
	public Texture2D emptyBar;
	//Parametr z teksturą pełnego paska życia
	public Texture2D fullBar;
	//Wypełnienie paska życia (ułamek w wartościach od 0 do 1)
	private float barDisplay;
	//Styl GUI do wyświetlania statystyk
	private GUIStyle statLabel;
	//Styl GUI do wyświetlania nazwy przeciwnika oraz punktów życia gracza
	private GUIStyle centeredLabel;
	//Styl GUI do wyświetlania napisu informującego nas o przegranej
	private GUIStyle gameOverLabel;
	//Licznik czasu
	private float timer;

	// Use this for initialization
	void Start () {
		//wczytanie statystyk
		GameObject newObject = GameObject.Find ("Player");
		MovePlayer player = (MovePlayer)newObject.GetComponent (typeof(MovePlayer));
		playerStats = player.stats;
		//ustawienie styli - kolor i wielkość czcionki, pogrubienie, ułożenie
		statLabel = new GUIStyle();
		statLabel.alignment = TextAnchor.UpperLeft;
		statLabel.fontSize = Screen.height / 50;
		statLabel.normal.textColor = Color.white;
		centeredLabel = new GUIStyle ();
		centeredLabel.fontSize = Screen.height / 33;
		centeredLabel.normal.textColor = Color.white;
		centeredLabel.fontStyle = FontStyle.Bold;
		centeredLabel.alignment = TextAnchor.MiddleCenter;
		gameOverLabel = new GUIStyle ();
		//wartość początkowa wielkości czcionki
		gameOverLabel.fontSize = Screen.height / 33;
		gameOverLabel.fontStyle = FontStyle.Bold;
		gameOverLabel.normal.textColor = Color.red;
		gameOverLabel.alignment = TextAnchor.MiddleCenter;
		timer = 1f;
	}

	private void OnGUI() {
		//wyświetlenie trzech statystyk
		GUI.Label (new Rect (Screen.width * 0.02f,
 		                     Screen.height * 0.01f,
 		                     Screen.width * 0.15f,
 		                     Screen.height * 0.02f),
		           "Poziom podziemi: " + StatsClass.dungeonLevel,
		           statLabel);
		GUI.Label (new Rect (Screen.width * 0.02f,
		                     Screen.height * 0.04f,
		                     Screen.width * 0.15f,
		                     Screen.height * 0.02f),
		           "Złoto: " + playerStats.getGold (),
		           statLabel);
		GUI.Label (new Rect (Screen.width * 0.02f,
		                     Screen.height * 0.07f,
		                     Screen.width * 0.15f,
		                     Screen.height * 0.02f),
		           "Doświadczenie: " + playerStats.getExperience (),
		           statLabel);
		//narysowanie pustego (pionowego) paska życia gracza
		GUI.DrawTexture (new Rect (Screen.width * 0.04f, 
		                           Screen.height * 0.12f, 
		                           Screen.width * 0.03f, 
		                           Screen.height * 0.82f), 
		                 emptyBar, 
		                 ScaleMode.StretchToFill);

		if (barDisplay >= 0){
			//jeśli ma więcej pkt życia niż 0, to ma pokazać pasek życia gracza oraz wyświetlić pkt życia
			GUI.DrawTexture (new Rect (Screen.width * 0.04f, 
			                           Screen.height * 0.12f + Screen.height * 0.82f * (1 - barDisplay),
			                           Screen.width * 0.03f,
			                           Screen.height * 0.82f * barDisplay),
			                 fullBar,
			                 ScaleMode.StretchToFill);
			GUI.Label(new Rect(Screen.width * 0.03f,
			                   Screen.height * 0.93f,
			                   Screen.width * 0.05f,
			                   Screen.height * 0.05f),
			          Mathf.CeilToInt(playerStats.getCurrentHP()) + " / " + (int)playerStats.getMaxHP(),
			          centeredLabel);
		}
		//jeśli gracz przegrał - wyświetl 
		if (barDisplay <= 0) {
			GUI.Label (new Rect(Screen.width * 0.4f,
			                    Screen.height * 0.48f,
			                    Screen.width * 0.2f,
			                    Screen.height * 0.04f),
			           "PRZEGRAŁEŚ", gameOverLabel);
			GUI.Label(new Rect(Screen.width * 0.03f,
			                   Screen.height * 0.93f,
			                   Screen.width * 0.05f,
			                   Screen.height * 0.05f),
			          "0 / " + (int)playerStats.getMaxHP(),
			          centeredLabel);
		}
		//jeśli przeciwnik jest martwy pasek życia ma być niewyświetlany
		if (HighlightedEnemyHealthClass.percentOfHP <= 0)
			HighlightedEnemyHealthClass.isOverEnemy = false;
		//jeśli można wyświetlić, to wyświetlamy pionowy pasek życia z nazwą przeciwnika
		if (HighlightedEnemyHealthClass.isOverEnemy) {
			GUI.DrawTexture (new Rect (Screen.width * 0.375f, 
			                           Screen.height * 0.04f, 
			                           Screen.width * 0.25f, 
			                           Screen.height * 0.03f), 
			                 emptyBar, 
			                 ScaleMode.StretchToFill);
			GUI.DrawTexture (new Rect (Screen.width * 0.375f,
			                           Screen.height * 0.04f,
			                           Screen.width * 0.25f * HighlightedEnemyHealthClass.percentOfHP,
			                           Screen.height * 0.03f),
			                 fullBar,
			                 ScaleMode.StretchToFill);
			GUI.Label (new Rect (Screen.width * 0.375f, Screen.height*0.04f, Screen.width * 0.25f, Screen.height*0.03f),
			           HighlightedEnemyHealthClass.enemyName, centeredLabel);
		}
	}

	// Update is called once per frame
	void Update () {
		//obliczenie ułamka z obecnym życiem gracza
		barDisplay = playerStats.getCurrentHP () / playerStats.getMaxHP();
		//zmiana wielkości czcionki ekranu zakończenia gry przez 2 sekundy po śmierci postaci gracza
		if (barDisplay < 0 && timer < 3f) {
			timer += Time.deltaTime;
			gameOverLabel.fontSize = (int)(Screen.height * timer) /33;
		}
	}
}
