/* 
 * Imię i nazwisko autora: Marcin Griszbacher
 * Temat pracy: Gra RPG stworzona w oparciu o silnik Unity
 * 
 * Klasa CustomUpgradesEditor
 * Zadanie: skrypt, który zmienia inspektora skryptu UpgradesGUI w edytorze silnika Unity w taki sposób, by możliwe
 * 			było wyświetlenie listy broni, zbroi oraz umiejętności pasywnych o dynamicznej liczbie elementów 
 * 			(domyślnie niemożliwe) oraz	uporządkowanie pozostałych modyfikowalnych parametrów skryptu. 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

//Własny inspector dla skryptu UpgradesGUI
[CustomEditor(typeof(UpgradesGUI))]

public class CustomUpgradesEditor : Editor { 

	//Skrypt
	UpgradesGUI upgrades;
	SerializedObject getTarget;
	//Listy do wyświetlenia
	SerializedProperty weaponList;
	SerializedProperty armorList;
	SerializedProperty passiveList;
	//Rozmiar listy - wykorzystywany po kolei dla każdej z list
	int listSize;
	//Wyświetlanie/chowanie elementów danego obiektu listy
	List<bool> showWeapons;
	List<bool> showArmors;
	List<bool> showPassives;
	//tablica z nazwami statystyk dla umiejętności pasywnych
	string[] statNames;

	void OnEnable() {
		//wczytanie skryptu oraz interesujących nas list
		upgrades = (UpgradesGUI)target;
		getTarget = new SerializedObject (upgrades);
		weaponList = getTarget.FindProperty ("weaponList");
		armorList = getTarget.FindProperty ("armorList");
		passiveList = getTarget.FindProperty ("passiveList");
		//ustawienie list wyświetlających/chowających obiekty w listach
		showWeapons = new List<bool> (0);
		for (int i=0; i<weaponList.arraySize; i++) {
			showWeapons.Add(false);
		}
		showArmors = new List<bool> (0);
		for (int i=0; i<armorList.arraySize; i++) {
			showArmors.Add(false);
		}
		showPassives = new List<bool> (0);
		for (int i=0; i<passiveList.arraySize; i++) {
			showPassives.Add(false);
		}
		//inicjalizacja tablicy z nazwami ulepszanych statystyk w um. pasywnych
		statNames = new string[] {"Gold", "Experience", "HP", "Attack Speed", "Attack damage", "Lifesteal", 
			"Damage reduction", "Dodge chance"};
	}


	public override void OnInspectorGUI ()
	{
		//Aktualizacja parametrów
		getTarget.Update ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		//Tekstura tła
		EditorGUILayout.LabelField ("Define textures", EditorStyles.boldLabel);
		upgrades.background = (Texture2D)EditorGUILayout.ObjectField ("Background: ", upgrades.background, typeof(Texture2D), true);
		EditorGUILayout.Space ();
		//Pytanie o liczbę broni
		EditorGUILayout.LabelField ("Define weapons", EditorStyles.boldLabel);
		listSize = weaponList.arraySize;
		listSize = EditorGUILayout.IntField ("Number of weapons: ", listSize);
		if (listSize < 0)
			listSize = 0;
		//Dodanie/usunięcie nowych/nadmiarowych broni po zmianie rozmiaru listy
		if (listSize != weaponList.arraySize) {
			while (listSize > weaponList.arraySize) {
				weaponList.InsertArrayElementAtIndex (weaponList.arraySize);
				showWeapons.Add(false);
			}
			while (listSize < weaponList.arraySize) {
				weaponList.DeleteArrayElementAtIndex (weaponList.arraySize - 1);
				showWeapons.RemoveAt(showWeapons.Count - 1);
			}
		}
		//Wyświetlenie każdego z obiektu z liście
		for (int i=0; i<weaponList.arraySize; i++) {
			showWeapons[i] = EditorGUILayout.Foldout(showWeapons[i], "Weapon " + i);
			if (showWeapons[i]) {
				//Powiązanie pól każdego obiektu z polami do wypełnienia przez użytkownika
				SerializedProperty weaponListRef = weaponList.GetArrayElementAtIndex(i);
				SerializedProperty texture = weaponListRef.FindPropertyRelative("texture");
				SerializedProperty name = weaponListRef.FindPropertyRelative("name");
				SerializedProperty minDamage = weaponListRef.FindPropertyRelative("minDamage");
				SerializedProperty maxDamage = weaponListRef.FindPropertyRelative("maxDamage");
				SerializedProperty attackSpeed = weaponListRef.FindPropertyRelative("attackSpeed");
				EditorGUILayout.PropertyField(texture);
				EditorGUILayout.PropertyField(name);
				EditorGUILayout.PropertyField(minDamage);
				EditorGUILayout.PropertyField(maxDamage);
				EditorGUILayout.PropertyField(attackSpeed);
				EditorGUILayout.Space();
			}
		}
		//Analogicznie do listy broni
		EditorGUILayout.LabelField ("Define armors", EditorStyles.boldLabel);
		listSize = armorList.arraySize;
		listSize = EditorGUILayout.IntField ("Number of armors: ", listSize);
		if (listSize < 0)
			listSize = 0;
		if (listSize != armorList.arraySize) {
			while (listSize > armorList.arraySize) {
				armorList.InsertArrayElementAtIndex (armorList.arraySize);
				showArmors.Add(false);
			}
			while (listSize < armorList.arraySize) {
				armorList.DeleteArrayElementAtIndex (armorList.arraySize - 1);
				showArmors.RemoveAt(showArmors.Count - 1);
			}
		}
		for (int i=0; i<armorList.arraySize; i++) {
			showArmors[i] = EditorGUILayout.Foldout(showArmors[i], "Armor " + i);
			if (showArmors[i]) {
				SerializedProperty armorListRef = armorList.GetArrayElementAtIndex(i);
				SerializedProperty texture = armorListRef.FindPropertyRelative("texture");
				SerializedProperty name = armorListRef.FindPropertyRelative("name");
				SerializedProperty damageReduction = armorListRef.FindPropertyRelative("damageReduction");
				SerializedProperty dodgeChance = armorListRef.FindPropertyRelative("dodgeChance");
				EditorGUILayout.PropertyField(texture);
				EditorGUILayout.PropertyField(name);
				EditorGUILayout.PropertyField(damageReduction);
				EditorGUILayout.PropertyField(dodgeChance);
				EditorGUILayout.Space();
			}
		}
		//Analogicznie do listy broni
		EditorGUILayout.LabelField ("Define passives", EditorStyles.boldLabel);
		listSize = passiveList.arraySize;
		listSize = EditorGUILayout.IntField ("Number of passives: ", listSize);
		if (passiveList.arraySize != listSize) {
			while (listSize > passiveList.arraySize) {
				passiveList.InsertArrayElementAtIndex (passiveList.arraySize);
				showPassives.Add(false);
			}
			while (listSize < passiveList.arraySize) {
				passiveList.DeleteArrayElementAtIndex (passiveList.arraySize - 1);
				showPassives.RemoveAt(showPassives.Count - 1);
			}
		}
		for (int i=0; i<passiveList.arraySize; i++) {
			showPassives[i] = EditorGUILayout.Foldout(showPassives[i], "Passive " + i);
			if (showPassives[i]) {
				SerializedProperty passiveListRef = passiveList.GetArrayElementAtIndex(i);
				SerializedProperty texture = passiveListRef.FindPropertyRelative("texture");
				SerializedProperty boostedStat = passiveListRef.FindPropertyRelative("boostedStat");
				SerializedProperty isPercent = passiveListRef.FindPropertyRelative("isPercent");
				SerializedProperty name = passiveListRef.FindPropertyRelative("name");
				SerializedProperty effect = passiveListRef.FindPropertyRelative("effect");
				EditorGUILayout.PropertyField(name);
				EditorGUILayout.PropertyField(texture);
				EditorGUILayout.PropertyField(effect);
				EditorGUILayout.PropertyField(isPercent);
				//Wyświetlenie listy wyboru z nazwami statystyk opisanymi wcześniej w tablicy
				boostedStat.intValue = EditorGUILayout.Popup("Boosted stat: ", boostedStat.intValue, statNames);
				EditorGUILayout.Space();
			}
		}
		//zapis zmian
		getTarget.ApplyModifiedProperties ();
	}

}