using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UpgradesGUI : MonoBehaviour {

	private enum Tab {INVENTORY, SKILLS};
	private Tab currentTab;
	private Vector2 scrollPositionUpper = Vector2.zero;
	private Vector2 scrollPositionLower = Vector2.zero;

	public Texture2D background;
	public GUISkin skin;
	private uint currentGold;
	private uint currentExp;
	private int level;
	private int maxAchievedLevel;

	[System.Serializable]
	public class WeaponClass {
		public int level;
		public Texture2D texture;
		public string name;
		public float minDamage;
		public float maxDamage;
		public float attackSpeed;
	}

	[System.Serializable]
	public class ArmorClass {
		public int level;
		public Texture2D texture;
		public string name;
		public float damageReduction;
		public float dodgeChance;
	}

	[System.Serializable]
	public class PassiveSkillsClass {
		public int level;
		public int boostedStat;
		public bool isPercent;
		public Texture2D texture;
		public string name;
		public float effect;
	}	

	public List<WeaponClass> weaponList = new List<WeaponClass>(0);
	public List<ArmorClass> armorList = new List<ArmorClass>(0);
	public List<PassiveSkillsClass> passiveList = new List<PassiveSkillsClass>(0);
	private int selectedWeapon;
	private int selectedArmor;


	public float calculateDamage(float pValue, int pLevel) {
		return pValue + pValue * pLevel * 1.25f; //do zbalansowania
	}

	public int calculateDamageReduction (float pValue, int pLevel) {
		return (int)(pValue + pValue * pLevel * 0.5f);
	}

	public int calculateDodgeChance(float pValue, int pLevel) {
		return (int)(pValue + pValue * pLevel * 0.2f);
	}

	public float calculatePassiveSkillEffect(float pValue, int pLevel) {
		return pValue * pLevel;
	}

	public uint calculateUpgradeCost (int pLevel) {
		if (pLevel < 20)
			return (uint)Mathf.Pow (pLevel+1, 2) * 100;
		else
			return 0;
	}


	
	// Use this for initialization
	void Start () {
		if (PlayerPrefs.GetInt("isSaved") == 1) {
			for (int i = 0; i<weaponList.Count; i++) {
				weaponList[i].level = PlayerPrefs.GetInt("weapon"+i);
			}
			for (int i=0; i<armorList.Count; i++) {
				armorList[i].level = PlayerPrefs.GetInt("armor"+i);
			}
			for (int i=0; i<passiveList.Count; i++) {
				passiveList[i].level = PlayerPrefs.GetInt("passive"+i);
			}
		}
		currentGold = StatsClass.gold;
		currentExp = StatsClass.experience;
		level = StatsClass.dungeonLevel;
		maxAchievedLevel = StatsClass.maxDungeonLevel;
		if (level > maxAchievedLevel) 
			maxAchievedLevel = level;
		if (maxAchievedLevel == 0)
						level = maxAchievedLevel = 1;
		selectedArmor = 0;
		selectedWeapon = 0;
		currentTab = Tab.INVENTORY;
	}
	
	private void OnGUI() {
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), background);
		GUI.skin = skin;
		GUI.Window (0, new Rect (Screen.width/20f, 
		                         Screen.height/20f, 
		                         Screen.width - Screen.width/10f, 
		                         Screen.height - Screen.height/10f),
		            upgradeWindow, GUIContent.none);
		GUI.backgroundColor = Color.black;

	}

	void upgradeWindow(int id) {

		GUIStyle upperLeftLabel = new GUIStyle (); //GUI.skin.GetStyle ("label");
		upperLeftLabel.alignment = TextAnchor.UpperLeft;
		upperLeftLabel.normal.textColor = Color.white;
		upperLeftLabel.fontStyle = FontStyle.Bold;
		upperLeftLabel.fontSize = Screen.height / 45;
		
		GUIStyle centeredLabel = new GUIStyle (); //GUI.skin.GetStyle ("label");
		centeredLabel.alignment = TextAnchor.MiddleCenter;
		centeredLabel.fontStyle = FontStyle.Bold;
		centeredLabel.fontSize = Screen.height / 45;
		centeredLabel.normal.textColor = Color.white;
		
		GUIStyle currencyLabel = new GUIStyle();
		currencyLabel.fontStyle = FontStyle.Bold;
		currencyLabel.fontSize = Screen.height / 33;
		currencyLabel.normal.textColor = Color.yellow;
		
		GUIStyle upgradeButton = new GUIStyle ();
		upgradeButton.normal.textColor = Color.yellow;
		upgradeButton.fontStyle = FontStyle.Bold;


		if (GUI.Button(new Rect (Screen.width/8f, Screen.height/20f, Screen.width/8f, Screen.height/20f), "Ekwipunek")) {
			if (currentTab != Tab.INVENTORY)
				currentTab = Tab.INVENTORY;
		}
		if (GUI.Button(new Rect (Screen.width*5.0f/8.0f, Screen.height/20f, Screen.width/8f, Screen.height/20f),"Umiejetnosci")) {
			if (currentTab != Tab.SKILLS)
				currentTab = Tab.SKILLS;
		}


			
		if (currentTab == Tab.INVENTORY) {

			GUI.Label(new Rect(Screen.width/8f, Screen.height*3f/20f,Screen.width, Screen.height),currentGold + " sztuk złota", currencyLabel);
			scrollPositionUpper = GUI.BeginScrollView(new Rect(Screen.width/20f,
			                                                   Screen.height/5f,
			                                                   Screen.width*4f/5f,
			                                                   Screen.height/5f),
			                                          scrollPositionUpper, 
			                                          new Rect(0,
			        											0,
			         											Screen.width*4f/5f*(weaponList.Count-1)/3f+(weaponList.Count-1)*Screen.width/10f,
			         											Screen.height/6f));
			//GUI.Box(new Rect(0,0,Screen.width, Screen.height), GUIContent.none);
			for (int i=0; i<weaponList.Count; i++) {
				GUI.Box(new Rect(i*(Screen.width/5f+Screen.width/10f),
				                 0, 
				                 Screen.width/5f, 
				                 Screen.height/6f),
				        GUIContent.none);
				GUI.DrawTexture(new Rect(i*(Screen.width/5f+Screen.width/10f),
				                            0,
				                            Screen.width/30f,
				                            Screen.width/30f),
				                weaponList[i].texture);
				GUI.Label(new Rect(i*(Screen.width/5f+Screen.width/10f)+Screen.width/25f,
				                   0, 
				                   Screen.width/5f, 
				                   Screen.height/20f),
				          weaponList[i].name, upperLeftLabel);
				GUI.Label(new Rect(i*(Screen.width/5f+Screen.width/10f)+Screen.width/25f,
				                   Screen.height/50f,
				                   Screen.width/5f,
				                   Screen.height/20f),
				          "Poziom " + weaponList[i].level, upperLeftLabel);
				GUI.Label(new Rect(i*(Screen.width/5f+Screen.width/10f)+Screen.width/200f,
				          		   Screen.height*7f/100f,
				          		   Screen.width/5f,
				          		   Screen.height/20f),
						  "Obrazenia: "+calculateDamage(weaponList[i].minDamage, weaponList[i].level)
									+"-"+calculateDamage(weaponList[i].maxDamage, weaponList[i].level),
						  upperLeftLabel);
				GUI.Label(new Rect(i*(Screen.width/5f+Screen.width/10f)+Screen.width/200f,
				                   Screen.height*9f/100f,
				                   Screen.width/5f,
				                   Screen.height/20f),
				          "Predkosc ataku: "+weaponList[i].attackSpeed,
				          upperLeftLabel);
				string selected;
				if (selectedWeapon==i) {
					GUI.enabled = false;
					selected = "Obecna";
				}
				else { 
					GUI.enabled = true;
					selected = "Ekwipuj";
				}
				if (GUI.Button(new Rect(i*(Screen.width/5f+Screen.width/10f)+Screen.width/200f,
				      					Screen.height*13f/100f,
				               			Screen.width/15f,
				               			Screen.height/30f),
				               selected)) {
					selectedWeapon = i;
				}
				uint cost = calculateUpgradeCost(weaponList[i].level);
				if (cost > currentGold) {
					GUI.enabled = false;
					upgradeButton.normal.textColor = Color.red;
					upgradeButton.fontStyle = FontStyle.Normal;
				}
				else {
					GUI.enabled = true;
					upgradeButton.normal.textColor = Color.yellow;
					upgradeButton.fontStyle = FontStyle.Bold;
				}
				if (GUI.Button(new Rect(i*(Screen.width/5f+Screen.width/10f)+Screen.width*5/40f,
				                        Screen.height*13f/100f,
				                        Screen.width/15f,
				                        Screen.height/30f),
				               cost.ToString() + " gold")) {
					currentGold -= cost;
					weaponList[i].level++;
				}
				GUI.enabled = true;
			}
			GUI.EndScrollView();
			scrollPositionLower = GUI.BeginScrollView(new Rect(Screen.width/20f,
			                                                   Screen.height*2.5f/5f,
			                                                   Screen.width*4f/5f,
			                                                   Screen.height/5f),
			                                          scrollPositionLower, 
			                                          new Rect(0,
			         0,
			         Screen.width*4f/5f*(armorList.Count-1)/3f+(armorList.Count-1)*Screen.width/10f,
			         Screen.height/6f));
			//GUI.Box(new Rect(0,0,Screen.width*4f/5f*(armorList.Count-1)/3f+(armorList.Count-1)*Screen.width/10f, Screen.height), GUIContent.none);
			for (int i=0; i<armorList.Count; i++) {
				GUI.Box(new Rect(i*(Screen.width/5f+Screen.width/10f),
				                 0, 
				                 Screen.width/5f, 
				                 Screen.height/6f),
				        GUIContent.none);
				GUI.DrawTexture(new Rect(i*(Screen.width/5f+Screen.width/10f),
				                         0,
				                         Screen.width/30f,
				                         Screen.width/30f),
				                armorList[i].texture);
				GUI.Label(new Rect(i*(Screen.width/5f+Screen.width/10f)+Screen.width/25f,
				                   0, 
				                   Screen.width/5f, 
				                   Screen.height/20f),
				          armorList[i].name, upperLeftLabel);
				GUI.Label(new Rect(i*(Screen.width/5f+Screen.width/10f)+Screen.width/25f,
				                   Screen.height/50f,
				                   Screen.width/5f,
				                   Screen.height/20f),
				          "Poziom " + armorList[i].level, upperLeftLabel);
				GUI.Label(new Rect(i*(Screen.width/5f+Screen.width/10f)+Screen.width/200f,
				                   Screen.height*7f/100f,
				                   Screen.width/5f,
				                   Screen.height/20f),
				          "Redukcja obrazen: "+calculateDamageReduction(armorList[i].damageReduction, armorList[i].level),
				          upperLeftLabel);
				GUI.Label(new Rect(i*(Screen.width/5f+Screen.width/10f)+Screen.width/200f,
				                   Screen.height*9f/100f,
				                   Screen.width/5f,
				                   Screen.height/20f),
				          "Szansa na unik (%): "+calculateDodgeChance(armorList[i].dodgeChance, armorList[i].level),
				          upperLeftLabel);
				string selected;
				if (selectedArmor==i) {
					GUI.enabled = false;
					selected = "Obecna";
				}
				else { 
					GUI.enabled = true;
					selected = "Ekwipuj";
				}
				if (GUI.Button(new Rect(i*(Screen.width/5f+Screen.width/10f)+Screen.width/200f,
				                        Screen.height*13f/100f,
				                        Screen.width/15f,
				                        Screen.height/30f),
				               selected)) {
					selectedArmor = i;
				}
				uint cost = calculateUpgradeCost(armorList[i].level);
				if (cost > currentGold) {
					GUI.enabled = false;
					upgradeButton.normal.textColor = Color.red;
					upgradeButton.fontStyle = FontStyle.Normal;
				}
				else {
					GUI.enabled = true;
					upgradeButton.normal.textColor = Color.yellow;
					upgradeButton.fontStyle = FontStyle.Bold;
				}
				if (GUI.Button(new Rect(i*(Screen.width/5f+Screen.width/10f)+Screen.width*5f/40f,
				                        Screen.height*13f/100f,
				                        Screen.width/15f,
				                        Screen.height/30f),
				               cost.ToString() + " gold")) {
					currentGold -= cost;
					armorList[i].level++;
				}
				GUI.enabled = true;
			}
			GUI.EndScrollView();
		}
		if (currentTab == Tab.SKILLS) {
			GUI.Label(new Rect(Screen.width/8f, Screen.height*3f/20f,Screen.width, Screen.height),currentExp + " doświadczenia", currencyLabel);
			scrollPositionUpper = GUI.BeginScrollView(new Rect(Screen.width/20f,
			                                                   Screen.height/5f,
			                                                   Screen.width*4f/5f,
			                                                   Screen.height/5f),
			                                          scrollPositionUpper, 
			                                          new Rect(0,
			         0,
			         Screen.width*4f/5f*(passiveList.Count-1)/3f+(passiveList.Count-1)*Screen.width/10f,
			         Screen.height/6f));
			//GUI.Box(new Rect(0,0,Screen.width, Screen.height), GUIContent.none);
			for (int i=0; i<passiveList.Count; i++) {
				GUI.Box(new Rect(i*(Screen.width/5f+Screen.width/10f),
				                 0, 
				                 Screen.width/5f, 
				                 Screen.height/6f),
				        GUIContent.none);
				GUI.DrawTexture(new Rect(i*(Screen.width/5f+Screen.width/10f),
				                         0,
				                         Screen.width/30f,
				                         Screen.width/30f),
				                weaponList[i].texture);
				GUI.Label(new Rect(i*(Screen.width/5f+Screen.width/10f)+Screen.width/25f,
				                   0, 
				                   Screen.width/5f, 
				                   Screen.height/20f),
				          passiveList[i].name, upperLeftLabel);
				GUI.Label(new Rect(i*(Screen.width/5f+Screen.width/10f)+Screen.width/25f,
				                   Screen.height/50f,
				                   Screen.width/5f,
				                   Screen.height/20f),
				          "Poziom " + passiveList[i].level, upperLeftLabel);
				string statName;
				switch(passiveList[i].boostedStat) {
				case 0:
					statName = "Gold";
					break;
				case 1:
					statName = "Experience";
					break;
				case 2:
					statName = "Health Points";
					break;
				case 3:
					statName = "Attack Speed";
					break;
				case 4:
					statName = "Attack Damage";
					break;
				case 5:
					statName = "Life Steal";
					break;
				case 6:
					statName = "Damage Reduction";
					break;
				case 7:
					statName = "Dodge Chance";
					break;
				default:
					statName = "ERROR";
					break;
				}
				GUI.Label(new Rect(i*(Screen.width/5f+Screen.width/10f)+Screen.width/200f,
				                   Screen.height*7f/100f,
				                   Screen.width/5f,
				                   Screen.height/20f),
				          "Ulepszana statystyka: " + statName, 
				          upperLeftLabel);
				string percent;
				if (passiveList[i].isPercent)
					percent = "%";
				else
					percent = "";
				GUI.Label(new Rect(i*(Screen.width/5f+Screen.width/10f)+Screen.width/200f,
				                   Screen.height*9f/100f,
				                   Screen.width/5f,
				                   Screen.height/20f),
				          "Wartość ulepszenia: "+passiveList[i].effect*passiveList[i].level + percent,
				          upperLeftLabel);
				uint cost = calculateUpgradeCost(passiveList[i].level);
				if (cost > currentExp) {
					GUI.enabled = false;
					upgradeButton.normal.textColor = Color.red;
					upgradeButton.fontStyle = FontStyle.Normal;
				}
				else {
					GUI.enabled = true;
					upgradeButton.normal.textColor = Color.yellow;
					upgradeButton.fontStyle = FontStyle.Bold;
				}
				if (GUI.Button(new Rect(i*(Screen.width/5f+Screen.width/10f)+Screen.width*5f/40f,
				                        Screen.height*13f/100f,
				                        Screen.width/15f,
				                        Screen.height/30f),
				               cost.ToString() + " exp")) {
					currentExp -= cost;
					passiveList[i].level++;
				}
				GUI.enabled = true;
			}
			GUI.EndScrollView();
		}
		if (level == 1) {
			GUI.enabled = false;
		}
		else {
			GUI.enabled = true;
		}
		if (GUI.Button(new Rect(Screen.width*3f/10f, Screen.height*15f/20f, Screen.width*1f/10f, Screen.height/20f), "<-")) {
			level--;
		}
		if (level == maxAchievedLevel) {
			GUI.enabled = false;
		}
		else {
			GUI.enabled = true;
		}
		if (GUI.Button(new Rect(Screen.width*5f/10f, Screen.height*15f/20f, Screen.width*1f/10f, Screen.height/20f), "->")) {
			level++;
		}
		GUI.enabled = true;
		GUI.Label (new Rect (Screen.width * 0.25f, Screen.height * 0.7f, Screen.width * 0.4f, Screen.height * 0.05f),
		           "Wybierz poziom od którego zaczniesz wędrówkę:", centeredLabel);
		GUI.Label (new Rect (Screen.width * 4 / 10f, Screen.height * 15f / 20f, Screen.width * 1f / 10f, Screen.height / 20f), 
		          level + " / " + maxAchievedLevel, centeredLabel);
		if (GUI.Button(new Rect(Screen.width*4f/10f, Screen.height*33f/40f, Screen.width*1f/10f, Screen.height/20f),
		               "ROZPOCZNIJ")) {
			startGame();
		}
	}

	void saveGame() {
		PlayerPrefs.SetInt ("isSaved", 1);
		PlayerPrefs.SetInt ("gold", (int)currentGold);
		PlayerPrefs.SetInt ("exp", (int)currentExp);
		PlayerPrefs.SetInt ("maxLevel", maxAchievedLevel);
		for (int i=0; i<weaponList.Count; i++) {
			PlayerPrefs.SetInt("weapon"+i, weaponList[i].level);
		}
		for (int i=0; i<armorList.Count; i++) {
			PlayerPrefs.SetInt("armor"+i, armorList[i].level);
		}
		for (int i=0; i<passiveList.Count; i++) {
			PlayerPrefs.SetInt("passive"+i, passiveList[i].level);
		}
		PlayerPrefs.Save ();
	}

	void startGame() {
		StatsClass.attackSpeed = weaponList [selectedWeapon].attackSpeed;
		StatsClass.damageReduction = calculateDamageReduction (armorList [selectedArmor].damageReduction,
		                                                      armorList [selectedArmor].level);
		StatsClass.dodgeChance = calculateDodgeChance (armorList [selectedArmor].dodgeChance, 
		                                              armorList [selectedArmor].level);
		StatsClass.dungeonLevel = level;
		StatsClass.experience = currentExp;
		StatsClass.gold = currentGold;
		StatsClass.lifeSteal = 0;
		StatsClass.maxAttackDamage = calculateDamage (weaponList [selectedWeapon].maxDamage,
		                                              weaponList [selectedWeapon].level);
		StatsClass.maxDungeonLevel = maxAchievedLevel;
		StatsClass.maxHealthPoints = 100; //base value
		StatsClass.minAttackDamage = calculateDamage (weaponList [selectedWeapon].minDamage,
		                                              weaponList [selectedWeapon].level);
		for (int i=0; i<passiveList.Count; i++) {
			switch (passiveList[i].boostedStat) {
			case 0:
				StatsClass.goldBonus = passiveList[i].effect; //always as percent
				break;
			case 1:
				StatsClass.expBonus = passiveList[i].effect; //always as percent
				break;
			case 2:
				if (!passiveList[i].isPercent)
					StatsClass.maxHealthPoints += passiveList[i].effect;
				else
					StatsClass.maxHealthPoints *= (passiveList[i].effect/100f)+1.0f; //effect is value in percent
				break;
			case 3:
				if (!passiveList[i].isPercent)
					StatsClass.attackSpeed += passiveList[i].effect;
				else
					StatsClass.attackSpeed *= (passiveList[i].effect/100f)+1.0f;
				break;
			case 4:
				if (!passiveList[i].isPercent) {
					StatsClass.minAttackDamage += passiveList[i].effect;
					StatsClass.maxAttackDamage += passiveList[i].effect;
				} else {
					StatsClass.minAttackDamage *= (passiveList[i].effect/100f)+1.0f;
					StatsClass.maxAttackDamage *= (passiveList[i].effect/100f)+1.0f;
				}
				break;
			case 5:
				StatsClass.lifeSteal += (int)passiveList[i].effect; //always as int
				break;
			case 6:
				if (passiveList[i].isPercent)
					StatsClass.damageReduction += (int)passiveList[i].effect;
				else
					StatsClass.damageReduction *= (int)((passiveList[i].effect/100f)+1.0f);
				break;
			case 7:
				StatsClass.dodgeChance += (int)passiveList[i].effect; //always as int
				break;
			}
		}
		saveGame ();
		Application.LoadLevel (1);
	}



	// Update is called once per frame
	void Update () {
		
	}
}
