/* 
 * Imię i nazwisko autora: Marcin Griszbacher
 * Temat pracy: Gra RPG stworzona w oparciu o silnik Unity
 * 
 * Klasa CustomMonstersEditor
 * Zadanie: skrypt, który zmienia inspektora skryptu DungeonCreator w edytorze silnika Unity w taki sposób, by możliwe
 * 			było wyświetlenie listy przeciwników o dynamicznej liczbie elementów (domyślnie niemożliwe) oraz 
 * 			uporządkowanie pozostałych modyfikowalnych parametrów skryptu. 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

//Własny inspector dla skryptu DungeonCreator
[CustomEditor(typeof(DungeonCreator))]

public class CustomMonstersEditor : Editor {

	//skrypt
	DungeonCreator dungeonCreator;
	SerializedObject GetTarget;
	//Lista do wyświetlenia
	SerializedProperty monsterList;
	//rozmiar listy
	int listSize;
	//wyświetlanie/chowanie elementów danego obiektu listy
	List<bool> showMonsters;

	void OnEnable() {
		//wyszukanie listy w skrypcie
		dungeonCreator = (DungeonCreator)target;
		GetTarget = new SerializedObject (dungeonCreator);
		monsterList = GetTarget.FindProperty ("monsterList");
		//schowanie listy wrogów
		showMonsters = new List<bool> ();
		for (int i=0; i<monsterList.arraySize; i++) {
			showMonsters.Add(false);
		}
	}

	public override void OnInspectorGUI() {
		//Aktualizacja parametrów
		GetTarget.Update ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		//Wyświetlenie zwyczajnych parametrów w odpowiadających im polach
		EditorGUILayout.LabelField ("Define dungeon parameters",EditorStyles.boldLabel);
		dungeonCreator.levelSize = EditorGUILayout.IntField ("Level size: ", dungeonCreator.levelSize);
		dungeonCreator.tilesToBeCreated = EditorGUILayout.IntField ("Tiles to be created: ", dungeonCreator.tilesToBeCreated);
		dungeonCreator.minimumRoomSize = EditorGUILayout.IntField ("Minimum room size: ", dungeonCreator.minimumRoomSize);
		dungeonCreator.maximumRoomSize = EditorGUILayout.IntField ("Maximum room size: ", dungeonCreator.maximumRoomSize);
		dungeonCreator.maxNumberOfTries = EditorGUILayout.IntField ("Maximum number of tries: ", dungeonCreator.maxNumberOfTries);
		dungeonCreator.minimumCorridorLength = EditorGUILayout.IntField ("Minimum corridor length: ", dungeonCreator.minimumCorridorLength);
		dungeonCreator.maximumCorridorLength = EditorGUILayout.IntField ("Maximum corridor length: ", dungeonCreator.maximumCorridorLength);
		EditorGUILayout.Space ();
		EditorGUILayout.LabelField ("Define prefabs and objects", EditorStyles.boldLabel);
		dungeonCreator.floorTilePrefab = (GameObject)EditorGUILayout.ObjectField ("Floor tile prefab: ", dungeonCreator.floorTilePrefab, typeof(GameObject), true);
		dungeonCreator.wallPrefab = (GameObject)EditorGUILayout.ObjectField ("Wall prefab: ", dungeonCreator.wallPrefab, typeof(GameObject), true);
		dungeonCreator.player = (GameObject)EditorGUILayout.ObjectField ("Player: ", dungeonCreator.player, typeof(GameObject), true);
		dungeonCreator.doorPrefab = (GameObject)EditorGUILayout.ObjectField ("Door prefab: ", dungeonCreator.doorPrefab, typeof(GameObject), true);
		dungeonCreator.exitLadderPrefab = (GameObject)EditorGUILayout.ObjectField ("Exit ladder prefab: ", dungeonCreator.exitLadderPrefab, typeof(GameObject), true);
		EditorGUILayout.Space ();

		EditorGUILayout.LabelField ("Define number of monsters", EditorStyles.boldLabel);
		listSize = monsterList.arraySize;
		//Pobranie liczby rodzajów wrogów
		listSize = EditorGUILayout.IntField ("Number of monsters: ", listSize);
		if (listSize < 0)
			listSize = 0;
		//Dodanie lub usunięcie nowych elementów listy
		if (listSize != monsterList.arraySize) {
			while (listSize > monsterList.arraySize) {
				monsterList.InsertArrayElementAtIndex (monsterList.arraySize);
				showMonsters.Add(false);
			}
			while (listSize < monsterList.arraySize) {
				monsterList.DeleteArrayElementAtIndex (monsterList.arraySize - 1);
				if (showMonsters.Count>0) 
					showMonsters.RemoveAt(showMonsters.Count - 1);
			}
		}
		//Pytanie o liczbę typów oraz liczbę przeciwników w podziemiach
		dungeonCreator.numberOfTypes = EditorGUILayout.IntField ("Number of enemy types: ", dungeonCreator.numberOfTypes);
		if (monsterList.arraySize > 0 && dungeonCreator.numberOfTypes < 0)
			dungeonCreator.numberOfTypes = 1;
		if (dungeonCreator.numberOfTypes > monsterList.arraySize)
			dungeonCreator.numberOfTypes = monsterList.arraySize;
		dungeonCreator.enemiesToSpawn = EditorGUILayout.IntField ("Enemies to spawn: ", dungeonCreator.enemiesToSpawn);
		//wyświetlanie listy
		for (int i=0; i<monsterList.arraySize; i++) {
			showMonsters[i] = EditorGUILayout.Foldout(showMonsters[i], "Monster number " + i);
			if (showMonsters[i]) {
				//Wyświetlenie pól danego obiektu listy
				SerializedProperty monsterListRef = monsterList.GetArrayElementAtIndex (i);
				SerializedProperty name = monsterListRef.FindPropertyRelative ("name");
				SerializedProperty monsterModel = monsterListRef.FindPropertyRelative ("monsterModel");
				SerializedProperty chanceToSpawn = monsterListRef.FindPropertyRelative ("chanceToSpawn");
				SerializedProperty healthPoints = monsterListRef.FindPropertyRelative ("healthPoints");
				SerializedProperty attackRange = monsterListRef.FindPropertyRelative ("attackRange");
				SerializedProperty damage = monsterListRef.FindPropertyRelative ("damage");
				SerializedProperty movementSpeed = monsterListRef.FindPropertyRelative ("movementSpeed");
				SerializedProperty experiencePerKill = monsterListRef.FindPropertyRelative ("experiencePerKill");
				SerializedProperty goldPerKill = monsterListRef.FindPropertyRelative ("goldPerKill");
				SerializedProperty attackCooldown = monsterListRef.FindPropertyRelative ("attackCooldown");
				SerializedProperty visionRange = monsterListRef.FindPropertyRelative ("visionRange");

				EditorGUILayout.PropertyField (monsterModel);
				EditorGUILayout.PropertyField (name);
				EditorGUILayout.PropertyField (chanceToSpawn);
				EditorGUILayout.PropertyField (healthPoints);
				EditorGUILayout.PropertyField (attackRange);
				EditorGUILayout.PropertyField (damage);
				EditorGUILayout.PropertyField (movementSpeed);
				EditorGUILayout.PropertyField (experiencePerKill);
				EditorGUILayout.PropertyField (goldPerKill);
				EditorGUILayout.PropertyField (attackCooldown);
				EditorGUILayout.PropertyField (visionRange);
				EditorGUILayout.Space();
			}
		}
		//Zapis wszystkich zmian
		GetTarget.ApplyModifiedProperties ();
	}

}
