using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(UpgradesGUI))]

public class CustomUpgradesEditor : Editor { 

	UpgradesGUI upgrades;
	SerializedObject getTarget;
	SerializedProperty weaponList;
	SerializedProperty armorList;
	SerializedProperty passiveList;
	int listSize;
	List<bool> showWeapons;
	List<bool> showArmors;
	List<bool> showPassives;
	string[] statNames;

	void OnEnable() {
		upgrades = (UpgradesGUI)target;
		getTarget = new SerializedObject (upgrades);
		weaponList = getTarget.FindProperty ("weaponList");
		armorList = getTarget.FindProperty ("armorList");
		passiveList = getTarget.FindProperty ("passiveList");
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
		statNames = new string[] {"Gold", "Experience", "HP", "Attack Speed", "Attack damage", "Lifesteal", 
			"Damage reduction", "Dodge chance"};
	}


	public override void OnInspectorGUI ()
	{
		getTarget.Update ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.LabelField ("Define base GUI parameters",EditorStyles.boldLabel);
		upgrades.background = (Texture2D)EditorGUILayout.ObjectField ("Background: ", upgrades.background, typeof(Texture2D), true);
		upgrades.skin = (GUISkin)EditorGUILayout.ObjectField ("Skin: ", upgrades.skin, typeof(GUISkin), true);
		EditorGUILayout.Space ();
		EditorGUILayout.LabelField ("Define weapons", EditorStyles.boldLabel);
		listSize = weaponList.arraySize;
		listSize = EditorGUILayout.IntField ("Number of weapons: ", listSize);
		if (listSize < 0)
			listSize = 0;
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
		for (int i=0; i<weaponList.arraySize; i++) {
			showWeapons[i] = EditorGUILayout.Foldout(showWeapons[i], "Weapon " + i);
			if (showWeapons[i]) {
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
		EditorGUILayout.LabelField ("Define passives", EditorStyles.boldLabel);
		listSize = passiveList.arraySize;
		listSize = EditorGUILayout.IntField ("Number of passives: ", listSize);
		if (passiveList.arraySize != listSize) {
			while (listSize < passiveList.arraySize) {
				passiveList.InsertArrayElementAtIndex (passiveList.arraySize);
				showPassives.Add(false);
			}
			while (listSize > passiveList.arraySize) {
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
				boostedStat.intValue = EditorGUILayout.Popup("Boosted stat: ", boostedStat.intValue, statNames);
				EditorGUILayout.Space();
			}
		}

		getTarget.ApplyModifiedProperties ();
	}

}