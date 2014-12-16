using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(DungeonCreator))]

public class CustomMonstersEditor : Editor {

	DungeonCreator dungeonCreator;
	SerializedObject GetTarget;
	SerializedProperty ThisList;
	int listSize;
	List<bool> showMonsters;

	void OnEnable() {
		dungeonCreator = (DungeonCreator)target;
		GetTarget = new SerializedObject (dungeonCreator);
		ThisList = GetTarget.FindProperty ("monsterList");
		showMonsters = new List<bool> (ThisList.arraySize);
		for (int i=0; i<ThisList.arraySize; i++) {
			showMonsters.Add(false);
		}
	}

	public override void OnInspectorGUI() {
		GetTarget.Update ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.LabelField ("Define dungeon parameters",EditorStyles.boldLabel);
		dungeonCreator.dungeonLevel = EditorGUILayout.IntField ("Dungeon level: ", dungeonCreator.dungeonLevel);
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
		listSize = ThisList.arraySize;
		listSize = EditorGUILayout.IntField ("Number of monsters: ", listSize);
		if (listSize < 0)
			listSize = 0;
		if (listSize != ThisList.arraySize) {
			while (listSize > ThisList.arraySize) {
				ThisList.InsertArrayElementAtIndex (ThisList.arraySize);
				showMonsters.Add(false);
			}
			while (listSize < ThisList.arraySize) {
				ThisList.DeleteArrayElementAtIndex (ThisList.arraySize - 1);
				/*if (showMonsters.Count>0)*/ showMonsters.RemoveAt(showMonsters.Count - 1);
			}
		}
		dungeonCreator.numberOfTypes = EditorGUILayout.IntField ("Number of enemy types: ", dungeonCreator.numberOfTypes);
		if (ThisList.arraySize > 0 && dungeonCreator.numberOfTypes < 0)
			dungeonCreator.numberOfTypes = 1;
		if (dungeonCreator.numberOfTypes > ThisList.arraySize)
			dungeonCreator.numberOfTypes = ThisList.arraySize;
		dungeonCreator.enemiesToSpawn = EditorGUILayout.IntField ("Enemies to spawn: ", dungeonCreator.enemiesToSpawn);

		for (int i=0; i<ThisList.arraySize; i++) {
			showMonsters[i] = EditorGUILayout.Foldout(showMonsters[i], "Monster number " + i);
			if (showMonsters[i]) {
				SerializedProperty monsterListRef = ThisList.GetArrayElementAtIndex (i);
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

		GetTarget.ApplyModifiedProperties ();
	}

}
