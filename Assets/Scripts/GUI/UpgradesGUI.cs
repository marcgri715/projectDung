/* 
 * Imię i nazwisko autora: Marcin Griszbacher
 * Temat pracy: Gra RPG stworzona w oparciu o silnik Unity
 * 
 * Klasa UpgradesGUI
 * Zadanie: skrypt wyświetlający okno z ulepszeniami do zakupienia za zdobyte złoto oraz doświadczenie. Możliwe jest
 * 			ulepszenie zbroi lub broni (oraz wybór tych, którymi walczyć będzie nasza postać) za pomocą złota oraz
 * 			ulepszenie umiejętności pasywnych (działających zawsze) za pomocą doświadczenia. Skrypt wczytuje oraz 
 * 			zapisuje stan gry (poziomy kolejnych ulepszeń). Umożliwia on też wybór poziomu, od którego rozpoczniemy
 * 			przygodę oraz przycisk przenoszący nas do samej gry.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UpgradesGUI : MonoBehaviour {

	//Tekstura tła
	public Texture2D background;
	//Zakładki okna ulepszeń
	private enum Tab {INVENTORY, SKILLS};
	//Obecna zakładka
	private Tab currentTab;
	//Pozycje paska przewijania w dwóch podoknach
	private Vector2 scrollPositionUpper = Vector2.zero;
	private Vector2 scrollPositionLower = Vector2.zero;

	//Obecna wartość złota
	private uint currentGold;
	//Obecna wartość doświadczenia
	private uint currentExp;
	//Obecnie wybrany poziom
	private int level;
	//Najwyższy osiągnięty poziom
	private int maxAchievedLevel;

	//Klasa broni
	[System.Serializable]
	public class WeaponClass {
		//poziom broni
		public int level;
		//tekstura
		public Texture2D texture;
		//nazwa broni
		public string name;
		//obrażenia minimalne, maksymalne oraz prędkość ataku
		public float minDamage;
		public float maxDamage;
		public float attackSpeed;
	}

	//Klasa zbroi
	[System.Serializable]
	public class ArmorClass {
		public int level;
		public Texture2D texture;
		public string name;
		//redukcja obrażeń oraz szansa na uniknięcie ataku
		public float damageReduction;
		public float dodgeChance;
	}

	//Klasa umiejętności pasywnych
	[System.Serializable]
	public class PassiveSkillsClass {
		public int level;
		//numer zwiększanej statystyki
		public int boostedStat;
		//flaga mówiąca nam o tym, czy statystyka jest zwiększana procentowo, czy też o stałą wartość
		public bool isPercent;
		public Texture2D texture;
		public string name;
		//wartość efektu
		public float effect;
	}	

	//Listy broni, zbroi oraz umiejętności pasywnych - parametry zmieniane w edytorze Unity
	public List<WeaponClass> weaponList = new List<WeaponClass>(0);
	public List<ArmorClass> armorList = new List<ArmorClass>(0);
	public List<PassiveSkillsClass> passiveList = new List<PassiveSkillsClass>(0);
	//Obecnie wybrana broń i zbroja
	private int selectedWeapon;
	private int selectedArmor;
	//Style GUI wykorzystywane przez różne elementy
	private GUIStyle upperLeftLabel;
	private GUIStyle centeredLabel;
	private GUIStyle currencyLabel;
	private GUIStyle upgradeButton;

	//Obliczenie obrażeń w zależności od poziomu
	public float calculateDamage(float pValue, int pLevel) {
		return pValue + pValue * pLevel * 1.25f;
	}

	//Obliczenie redukcji obrażeń w zależności od poziomu 
	public int calculateDamageReduction (float pValue, int pLevel) {
		return (int)(pValue + pValue * pLevel * 0.5f);
	}

	//Obliczenie szansy na unik w zależności od poziomu
	public int calculateDodgeChance(float pValue, int pLevel) {
		return (int)(pValue + pValue * pLevel * 0.2f);
	}

	//Obliczenie efektu umiejętności pasywnej w zależności od poziomu
	public float calculatePassiveSkillEffect(float pValue, int pLevel) {
		return pValue * pLevel;
	}

	//Obliczenie kosztu ulepszenia w zależności od poziomu - max. 20 poziom
	public uint calculateUpgradeCost (int pLevel) {
		if (pLevel < 20)
			return (uint)Mathf.Pow (pLevel+1, 2) * 50;
		else
			return 0;
	}


	
	// Use this for initialization
	void Start () {
		//jeśli gra była zapisana, wczytanie poziomów każdej z broni, zbroi oraz um. pasywnej
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
		//a następnie złota, doświadczenia oraz najdalszego osiągniętego poziomu
		currentGold = StatsClass.gold;
		currentExp = StatsClass.experience;
		level = StatsClass.dungeonLevel;
		maxAchievedLevel = StatsClass.maxDungeonLevel;
		if (level > maxAchievedLevel) 
			maxAchievedLevel = level;
		if (level < 1) 
			level = maxAchievedLevel;
		if (maxAchievedLevel <= 0)
						level = maxAchievedLevel = 1;
		selectedArmor = 0;
		selectedWeapon = 0;
		//Domyślna zakładka: ekwipunek
		currentTab = Tab.INVENTORY;
		//Ustawienie parametrów styli
		upperLeftLabel = new GUIStyle (); 
		upperLeftLabel.alignment = TextAnchor.UpperLeft;
		upperLeftLabel.normal.textColor = Color.white;
		upperLeftLabel.fontStyle = FontStyle.Bold;
		upperLeftLabel.fontSize = Screen.height / 45;
		
		centeredLabel = new GUIStyle ();
		centeredLabel.alignment = TextAnchor.MiddleCenter;
		centeredLabel.fontStyle = FontStyle.Bold;
		centeredLabel.fontSize = Screen.height / 45;
		centeredLabel.normal.textColor = Color.white;
		
		currencyLabel = new GUIStyle();
		currencyLabel.fontStyle = FontStyle.Bold;
		currencyLabel.fontSize = Screen.height / 33;
		currencyLabel.normal.textColor = Color.yellow;
		
		upgradeButton = new GUIStyle();
		upgradeButton.normal.textColor = Color.yellow;
		upgradeButton.fontStyle = FontStyle.Bold;
		upgradeButton.alignment = TextAnchor.MiddleCenter;
	}

	//wyświetlenie okna ulepszeń
	private void OnGUI() {
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height),
		                background,
		                ScaleMode.StretchToFill);
		GUI.Window (0, new Rect (Screen.width * 0.05f, 
		                         Screen.height * 0.05f, 
		                         Screen.width * 0.9f, 
		                         Screen.height * 0.9f),
		            upgradeWindow, GUIContent.none);
		GUI.backgroundColor = Color.black;
	}

	void upgradeWindow(int id) {
		//przyciski zakładek
		if (currentTab == Tab.INVENTORY)
			GUI.enabled = false;
		if (GUI.Button(new Rect (Screen.width*0.125f,
		                         Screen.height*0.05f,
		                         Screen.width*0.125f,
		                         Screen.height*0.05f),
		               "Ekwipunek")) {
			if (currentTab != Tab.INVENTORY)
				currentTab = Tab.INVENTORY;
		}
		GUI.enabled = true;
		if (currentTab == Tab.SKILLS)
			GUI.enabled = false;
		if (GUI.Button(new Rect (Screen.width*0.625f,
		                         Screen.height*0.05f,
		                         Screen.width*0.125f,
		                         Screen.height*0.05f),
		               "Umiejetnosci")) {
			if (currentTab != Tab.SKILLS)
				currentTab = Tab.SKILLS;
		}
		GUI.enabled = true;

		//Ekran ulepszenia ekwipunku
		if (currentTab == Tab.INVENTORY) {
			//Ilość złota
			GUI.Label(new Rect(Screen.width*0.125f,
			                   Screen.height*0.15f,
			                   Screen.width,
			                   Screen.height),currentGold + " sztuk złota", currencyLabel);
			//Okno wewnętrzne z listą broni do ulepszenia
			scrollPositionUpper = GUI.BeginScrollView(new Rect(Screen.width*0.05f,
			                                                   Screen.height*0.2f,
			                                                   Screen.width*0.8f,
			                                                   Screen.height*0.2f),
			                                          scrollPositionUpper, 
			                                          new Rect(0,
			        										   0,
			         										   Screen.width*(0.3f*weaponList.Count-0.1f),
			         										   Screen.height*0.166f));
			//Wyświetlenie pola dla każdej z ulepszanych broni
			for (int i=0; i<weaponList.Count; i++) {
				GUI.Box(new Rect(i*Screen.width*0.3f,
				                 0, 
				                 Screen.width*0.2f, 
				                 Screen.height*0.166f),
				        GUIContent.none);
				GUI.DrawTexture(new Rect(i*Screen.width*0.3f,
				                         0,
				                         Screen.width*0.033f,
				                         Screen.width*0.033f),
				                weaponList[i].texture);
				GUI.Label(new Rect(i*Screen.width*0.3f+Screen.width*0.04f,
				                   0, 
				                   Screen.width*0.2f, 
				                   Screen.height*0.05f),
				          weaponList[i].name, upperLeftLabel);
				GUI.Label(new Rect(i*Screen.width*0.3f+Screen.width*0.04f,
				                   Screen.height*0.02f,
				                   Screen.width*0.2f,
				                   Screen.height*0.05f),
				          "Poziom " + weaponList[i].level, upperLeftLabel);
				GUI.Label(new Rect(i*Screen.width*0.3f+Screen.width*0.005f,
				          		   Screen.height*0.07f,
				          		   Screen.width*0.2f,
				          		   Screen.height*0.05f),
						  "Obrazenia: "+calculateDamage(weaponList[i].minDamage, weaponList[i].level)
									+"-"+calculateDamage(weaponList[i].maxDamage, weaponList[i].level),
						  upperLeftLabel);
				GUI.Label(new Rect(i*Screen.width*0.3f+Screen.width*0.005f,
				                   Screen.height*0.09f,
				                   Screen.width*0.2f,
				                   Screen.height*0.05f),
				          "Predkosc ataku: "+weaponList[i].attackSpeed,
				          upperLeftLabel);
				//Przycisk do ekwipowania broni
				string selected;
				if (selectedWeapon==i) {
					GUI.enabled = false;
					selected = "Obecna";
				}
				else { 
					GUI.enabled = true;
					selected = "Ekwipuj";
				}
				//Przycisk do ulepszania broni
				if (GUI.Button(new Rect(i*Screen.width*0.3f+Screen.width*0.005f,
				      					Screen.height*0.13f,
				               			Screen.width*0.066f,
				               			Screen.height*0.033f),
				               selected)) {
					selectedWeapon = i;
				}
				//Zmiana wyświetlania kosztu w zależności od tego, czy mamy wystarczające fundusze, czy też nie
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
				if (GUI.Button(new Rect(i*Screen.width*0.3f+Screen.width*0.125f,
				                        Screen.height*0.13f,
				                        Screen.width*0.066f,
				                        Screen.height*0.033f),
				               GUIContent.none)) {
					currentGold -= cost;
					weaponList[i].level++;
				}
				GUI.Label (new Rect(i*Screen.width*0.3f+Screen.width*0.125f,
				                    Screen.height*0.13f,
				                    Screen.width*0.066f,
				                    Screen.height*0.033f),
				           cost.ToString() + " zł.",
				           upgradeButton);
				GUI.enabled = true;
			}
			GUI.EndScrollView();
			//Ulepszenia zbroi - analogicznie do ulepszeń broni
			scrollPositionLower = GUI.BeginScrollView(new Rect(Screen.width*0.05f,
			                                                   Screen.height*0.5f,
			                                                   Screen.width*0.8f,
			                                                   Screen.height*0.2f),
			                                          scrollPositionLower, 
			                                          new Rect(0,
													           0,
													           Screen.width*(0.3f*armorList.Count-0.1f),
													           Screen.height*0.166f));
			for (int i=0; i<armorList.Count; i++) {
				GUI.Box(new Rect(i*Screen.width*0.3f,
				                 0, 
				                 Screen.width*0.2f, 
				                 Screen.height*0.166f),
				        GUIContent.none);
				GUI.DrawTexture(new Rect(i*Screen.width*0.3f,
				                         0,
				                         Screen.width*0.033f,
				                         Screen.width*0.033f),
				                armorList[i].texture);
				GUI.Label(new Rect(i*Screen.width*0.3f+Screen.width*0.04f,
				                   0, 
				                   Screen.width*0.2f, 
				                   Screen.height*0.05f),
				          armorList[i].name, upperLeftLabel);
				GUI.Label(new Rect(i*Screen.width*0.3f+Screen.width*0.04f,
				                   Screen.height*0.02f,
				                   Screen.width*0.2f,
				                   Screen.height*0.05f),
				          "Poziom " + armorList[i].level, upperLeftLabel);
				GUI.Label(new Rect(i*Screen.width*0.3f+Screen.width*0.005f,
				                   Screen.height*0.07f,
				                   Screen.width*0.2f,
				                   Screen.height*0.05f),
				          "Redukcja obrazen: "+calculateDamageReduction(armorList[i].damageReduction, armorList[i].level),
				          upperLeftLabel);
				GUI.Label(new Rect(i*Screen.width*0.3f+Screen.width*0.005f,
				                   Screen.height*0.09f,
				                   Screen.width*0.2f,
				                   Screen.height*0.05f),
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
				if (GUI.Button(new Rect(i*Screen.width*0.3f+Screen.width*0.005f,
				                        Screen.height*0.13f,
				                        Screen.width*0.066f,
				                        Screen.height*0.033f),
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
				if (GUI.Button(new Rect(i*Screen.width*0.3f+Screen.width*0.125f,
				                        Screen.height*0.13f,
				                        Screen.width*0.066f,
				                        Screen.height*0.033f),
				               GUIContent.none)) {
					currentGold -= cost;
					armorList[i].level++;
				}
				GUI.Label(new Rect(i*Screen.width*0.3f+Screen.width*0.125f,
				          			Screen.height*0.13f,
				         			Screen.width*0.066f,
				          			Screen.height*0.033f),
				 			cost.ToString() + " zł.",
				 			upgradeButton);
				GUI.enabled = true;
			}
			GUI.EndScrollView();
		}
		//Zakładka umiejętności
		if (currentTab == Tab.SKILLS) {
			//Obecna wartość doświadczenia
			GUI.Label(new Rect(Screen.width*0.125f,
			                   Screen.height*0.15f,
			                   Screen.width,
			                   Screen.height),
			          currentExp + " doświadczenia",
			          currencyLabel);
			//Umiejętności pasywne
			scrollPositionUpper = GUI.BeginScrollView(new Rect(Screen.width*0.05f,
			                                                   Screen.height*0.2f,
			                                                   Screen.width*0.8f,
			                                                   Screen.height*0.2f),
			                                          scrollPositionUpper, 
			                                          new Rect(0,
													           0,
													           Screen.width*(0.3f*passiveList.Count-0.1f),
													           Screen.height*0.166f));
			for (int i=0; i<passiveList.Count; i++) {
				GUI.Box(new Rect(i*Screen.width*0.3f,
				                 0, 
				                 Screen.width*0.2f, 
				                 Screen.height*0.166f),
				        GUIContent.none);
				GUI.DrawTexture(new Rect(i*Screen.width*0.3f,
				                         0,
				                         Screen.width*0.033f,
				                         Screen.width*0.033f),
				                passiveList[i].texture);
				GUI.Label(new Rect(i*Screen.width*0.3f+Screen.width*0.04f,
				                   0, 
				                   Screen.width*0.2f, 
				                   Screen.height*0.05f),
				          passiveList[i].name, upperLeftLabel);
				GUI.Label(new Rect(i*Screen.width*0.3f+Screen.width*0.04f,
				                   Screen.height*0.02f,
				                   Screen.width*0.2f,
				                   Screen.height*0.05f),
				          "Poziom " + passiveList[i].level, upperLeftLabel);
				//Wybór odpowiedniej wzmacnianej statystyki
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
				GUI.Label(new Rect(i*Screen.width*0.3f+Screen.width*0.005f,
				                   Screen.height*0.07f,
				                   Screen.width*0.2f,
				                   Screen.height*0.05f),
				          "Ulepszana statystyka: ", 
				          upperLeftLabel);
				GUI.Label(new Rect(i*Screen.width*0.3f+Screen.width*0.075f,
				                   Screen.height*0.09f,
				                   Screen.width*0.2f,
				                   Screen.height*0.05f),
				          statName, 
				          upperLeftLabel);
				string percent;
				//Dodanie znaku procenta, jeśli umiejętność zwiększa wartość statystyki procentowo
				if (passiveList[i].isPercent)
					percent = "%";
				else
					percent = "";
				GUI.Label(new Rect(i*Screen.width*0.3f+Screen.width*0.005f,
				                   Screen.height*0.11f,
				                   Screen.width*0.2f,
				                   Screen.height*0.05f),
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
				if (GUI.Button(new Rect(i*Screen.width*0.3f+Screen.width*0.125f,
				                        Screen.height*0.13f,
				                        Screen.width*0.066f,
				                        Screen.height*0.033f),
				               GUIContent.none)) {
					currentExp -= cost;
					passiveList[i].level++;
				}
				GUI.Label(new Rect(i*Screen.width*0.3f+Screen.width*0.125f,
				          			Screen.height*0.13f,
				          			Screen.width*0.066f,
				          			Screen.height*0.033f),
				 			cost.ToString() + " xp",
				        	upgradeButton);
				GUI.enabled = true;
			}
			GUI.EndScrollView();
		}
		//Wyświetlenie przycisków wyboru poziomu, od którego zaczniemy
		if (level == 1) {
			GUI.enabled = false;
		}
		else {
			GUI.enabled = true;
		}
		if (GUI.Button(new Rect(Screen.width*0.3f,
		                        Screen.height*0.75f,
		                        Screen.width*0.1f,
		                        Screen.height*0.05f),
		               "<-")) {
			level--;
		}
		if (level == maxAchievedLevel) {
			GUI.enabled = false;
		}
		else {
			GUI.enabled = true;
		}
		if (GUI.Button(new Rect(Screen.width*0.5f,
		                        Screen.height*0.75f,
		                        Screen.width*0.1f,
		                        Screen.height*0.05f), "->")) {
			level++;
		}
		GUI.enabled = true;
		GUI.Label (new Rect (Screen.width * 0.25f,
		                     Screen.height * 0.7f,
		                     Screen.width * 0.4f,
		                     Screen.height * 0.05f),
		           "Wybierz poziom od którego zaczniesz wędrówkę:",
		           centeredLabel);
		GUI.Label (new Rect (Screen.width * 0.4f,
		                     Screen.height * 0.75f,
		                     Screen.width * 0.1f,
		                     Screen.height * 0.05f), 
		          level + " / " + maxAchievedLevel,
		           centeredLabel);
		//Wyświetlenie przycisku, który rozpoczyna grę
		if (GUI.Button(new Rect(Screen.width*0.4f,
		                        Screen.height*0.825f,
		                        Screen.width*0.1f,
		                        Screen.height*0.05f),
		               "ROZPOCZNIJ")) {
			startGame();
		}
	}

	//Autozapis przeprowadzany przy każdym rozpoczęciu gry oraz wejściu do ekranu ulepszeń
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

	//Rozpoczęcie rozgrywki
	void startGame() {
		//przypisanie do klasy statycznej parametrów zależnych od wybranej broni i zbroi, zwiększonych przez um. pasywne
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
		StatsClass.maxHealthPoints = 100; //wartość bazowa
		StatsClass.minAttackDamage = calculateDamage (weaponList [selectedWeapon].minDamage,
		                                              weaponList [selectedWeapon].level);
		PassiveSkillsClass passive = passiveList [0];
		for (int i=0; i<passiveList.Count; i++) {
			switch (passiveList[i].boostedStat) {
			case 0:
				StatsClass.goldBonus = passiveList[i].effect/100f*passiveList[i].level; //podawane zawsze jako procent, konwertowane do ułamka
				break;
			case 1:
				StatsClass.expBonus = passiveList[i].effect*0.01f*passiveList[i].level; //podawane zawsze jako procent, konwertowane do ułamka
				break;
			case 2:
				if (!passiveList[i].isPercent)
					StatsClass.maxHealthPoints += passiveList[i].effect*passiveList[i].level;
				else
					StatsClass.maxHealthPoints *= passiveList[i].effect*0.01f*passiveList[i].level+1.0f; //efekt to wartość procentowa
				break;
			case 3:
				if (!passiveList[i].isPercent)
					StatsClass.attackSpeed += passiveList[i].effect*passiveList[i].level;
				else
					StatsClass.attackSpeed *= passiveList[i].effect*0.01f*passiveList[i].level+1.0f;
				break;
			case 4:
				if (!passiveList[i].isPercent) {
					StatsClass.minAttackDamage += passiveList[i].effect*passiveList[i].level;
					StatsClass.maxAttackDamage += passiveList[i].effect*passiveList[i].level;
				} else {
					StatsClass.minAttackDamage *= passiveList[i].effect*0.01f*passiveList[i].level+1.0f;
					StatsClass.maxAttackDamage *= passiveList[i].effect*0.01f*passiveList[i].level+1.0f;
				}
				break;
			case 5:
				StatsClass.lifeSteal += (int)passiveList[i].effect*passiveList[i].level; //zawsze jako procent
				break;
			case 6:
				if (passiveList[i].isPercent)
					StatsClass.damageReduction += (int)passiveList[i].effect*passiveList[i].level;
				else
					StatsClass.damageReduction *= (int)(passiveList[i].effect*0.01f*passiveList[i].level+1.0f);
				break;
			case 7:
				StatsClass.dodgeChance += (int)passiveList[i].effect*passiveList[i].level; //zawsze jako procent
				break;
			}
		}
		//Zapis gry
		saveGame ();
		//Rozpoczęcie gry
		Application.LoadLevel (1);
	}
}
