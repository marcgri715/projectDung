using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour {

	private PlayerStatsClass playerStats;
	private float maxHP;
	public GUIStyle emptyStyle;
	public GUIStyle fullStyle;
	public Texture2D emptyBar;
	public Texture2D fullBar;
	private float barDisplay;

	// Use this for initialization
	void Start () {
		GameObject newObject = GameObject.Find ("Player");
		MovePlayer player = (MovePlayer)newObject.GetComponent (typeof(MovePlayer));
		playerStats = player.stats;
		maxHP = playerStats.getMaxHP ();
	}

	private void OnGUI() {
		GUIStyle statLabel = new GUIStyle();
		statLabel.alignment = TextAnchor.UpperLeft;
		statLabel.fontSize = Screen.height / 50;
		statLabel.normal.textColor = Color.white;
		GUIStyle centeredLabel = new GUIStyle ();
		centeredLabel.fontSize = Screen.height / 33;
		centeredLabel.normal.textColor = Color.white;
		centeredLabel.fontStyle = FontStyle.Bold;
		centeredLabel.alignment = TextAnchor.MiddleCenter;

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
		GUI.DrawTexture (new Rect (Screen.width * 0.04f, 
		                           Screen.height * 0.12f, 
		                           Screen.width * 0.03f, 
		                           Screen.height * 0.82f), 
		                 emptyBar, 
		                 ScaleMode.StretchToFill);

		if (barDisplay > 0){
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
			          (int)playerStats.getCurrentHP() + " / " + (int)playerStats.getMaxHP(),
			          centeredLabel);
		}
		if (barDisplay <= 0) {
			GUI.Label (new Rect(Screen.width * 2f / 5f,
			                    Screen.height *24f/50f,
			                    Screen.width/5f,
			                    Screen.height*2f/50f),
			           "PRZEGRAŁEŚ");
		}
		if (HighlightedEnemyHealthClass.percentOfHP < 0)
			HighlightedEnemyHealthClass.isOverEnemy = false;
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
		barDisplay = playerStats.getCurrentHP () / maxHP;
	}
}
