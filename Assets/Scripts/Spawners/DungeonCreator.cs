using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class DungeonCreator : MonoBehaviour {


	public int dungeonLevel;
	public int levelSize;
	public int tilesToBeCreated;
	public int minimumRoomSize;
	public int maximumRoomSize;
	public int maxNumberOfTries;
	public int minimumCorridorLength;
	public int maximumCorridorLength;
	public GameObject floorTilePrefab = null;
	public GameObject wallPrefab = null;
	public GameObject player = null;
	public GameObject doorPrefab = null;
	public GameObject exitLadderPrefab = null;

	[System.Serializable]
	public class Monsters {
		public GameObject monsterModel = null;
		public int chanceToSpawn;
		public string name;
		public float healthPoints;
		public float attackRange;
		public float damage;
		public float movementSpeed;
		public float experiencePerKill;
		public float goldPerKill;
		public float attackCooldown;
		public float visionRange;
	}

	public int numberOfTypes;
	public int enemiesToSpawn;
	public List<Monsters> monsterList = new List<Monsters> (0);
	private List<int> chosenTypes;

	void AddNew() {
		monsterList.Add (new Monsters ());
	}

	void Remove(int index) {
		monsterList.RemoveAt (index);
	}

	private DungeonClass dungeon;

	void SpawnTerrain () {
		int elementNumber = 0;
		for (int i=0; i<levelSize; i++)
		for (int j=0; j<levelSize; j++) {		
			Vector3 spawnPoint = new Vector3(i,0,j);
			switch(dungeon.getTile(i,j)) {
			case 1:
				//GameObject floorTile = (GameObject)Instantiate(floorTilePrefab,spawnPoint,Quaternion.Euler(0,0,0));
				Instantiate(floorTilePrefab, spawnPoint, Quaternion.Euler(0,0,0));
				break;
			case 2:
				//GameObject exitLadder = (GameObject)Instantiate(exitLadderPrefab,spawnPoint,Quaternion.Euler(0,0,0));
				Instantiate(exitLadderPrefab,spawnPoint,Quaternion.Euler(0,0,0));
				break;
			}
			spawnPoint = new Vector3(i-0.5f,1.0f,j);
			switch(dungeon.getHorizontalEdge(i,j)) {
			case 1:
				//GameObject wall = (GameObject)Instantiate(wallPrefab,spawnPoint,Quaternion.Euler(0,0,0));
				Instantiate(wallPrefab,spawnPoint,Quaternion.Euler(0,0,0));
				break;
			case 2:
				GameObject door = (GameObject)Instantiate(doorPrefab,spawnPoint,Quaternion.Euler(0,0,0));
				door.name = "Door " + elementNumber++;
				break;
			}
			spawnPoint = new Vector3(i, 1.0f, j-0.5f);
			switch(dungeon.getVerticalEdge(i,j)) {
			case 1:
				//GameObject wall = (GameObject)Instantiate(wallPrefab,spawnPoint,Quaternion.Euler(0,90,0));
				Instantiate(wallPrefab,spawnPoint,Quaternion.Euler(0,90,0));
				break;
			case 2:
				GameObject door = (GameObject)Instantiate(doorPrefab,spawnPoint,Quaternion.Euler(0,90,0));
				door.name = "Door " + elementNumber++;
				break;
			}
		}
		for (int i=0; i<levelSize+1; i++) {
			Vector3 spawnPoint = new Vector3(levelSize-0.5f,1.0f,i);
			switch(dungeon.getHorizontalEdge(levelSize,i)) {
			case 1:
				//GameObject wall = (GameObject)Instantiate(wallPrefab,spawnPoint,Quaternion.Euler(0,0,0));
				Instantiate(wallPrefab,spawnPoint,Quaternion.Euler(0,0,0));
				break;
			case 2:
				GameObject door = (GameObject)Instantiate(wallPrefab,spawnPoint,Quaternion.Euler(0,0,0));
				door.name = "Door " + elementNumber++;
				break;
			}
			spawnPoint = new Vector3(levelSize, 1.0f, i-0.5f);
			switch(dungeon.getVerticalEdge(levelSize,i)) {
			case 1:
				//GameObject wall = (GameObject)Instantiate(wallPrefab,spawnPoint,Quaternion.Euler(0,90,0));
				Instantiate(wallPrefab,spawnPoint,Quaternion.Euler(0,90,0));
				break;
			case 2:
				GameObject door = (GameObject)Instantiate(doorPrefab,spawnPoint,Quaternion.Euler(0,90,0));
				door.name = "Door " + elementNumber++;
				break;
			}
		}
	}

	void SpawnMonsters() {
		for (int i=0; i<dungeon.getNumberOfMonsters(); i++) {
			int roomNumber = dungeon.getMonster(i).getStartingRoom();
			float startingX, startingY;
			startingX = Random.Range(dungeon.getRoom(roomNumber).getLeftX(), dungeon.getRoom(roomNumber).getRightX());
			startingY = Random.Range(dungeon.getRoom(roomNumber).getTopY(), dungeon.getRoom(roomNumber).getBottomY());
			Vector3 spawnPoint = new Vector3(startingX, 0.08f, startingY);
			int type = chosenTypes[dungeon.getMonster(i).getMonsterType()];
			GameObject monster = (GameObject)Instantiate(monsterList[type].monsterModel,
			                                             spawnPoint, Quaternion.Euler(0,0,0));
			monster.name = i.ToString();
		}
	}

	void putPlayerToStartingPosition () {
		Vector3 startPos = new Vector3 ();
		startPos.x = (dungeon.getRoom(0).getRightX () - dungeon.getRoom(0).getLeftX ()) / 2 + dungeon.getRoom(0).getLeftX();
		startPos.z = (dungeon.getRoom(0).getBottomY () - dungeon.getRoom(0).getTopY ()) / 2 + dungeon.getRoom(0).getTopY();
		startPos.y = 0.35f;
		player.transform.Translate (startPos);
		LookForMonsters looker = (LookForMonsters)player.transform.gameObject.GetComponent (typeof(LookForMonsters));
		looker.numberOfEnemies = enemiesToSpawn;
	}
		
		
		// Use this for initialization
	void Start () {
		dungeonLevel = StatsClass.dungeonLevel;
		Random.seed = (int)System.DateTime.Now.Ticks;
		dungeon = new DungeonClass (dungeonLevel);
		dungeon.generateNewDungeon (levelSize, tilesToBeCreated, minimumRoomSize, maximumRoomSize,
		                           maxNumberOfTries, minimumCorridorLength, maximumCorridorLength);
		chosenTypes = new List<int>();
		int i = 0;
		int[] chances = new int[numberOfTypes];
		List<float[]> baseValues = new List<float[]>();
		string[] monsterNames = new string[numberOfTypes]; 
		while (i<numberOfTypes) {
			int newNumber = Random.Range(0,monsterList.Count);
			bool duplicateFound = false;
			for (int j=0; j<chosenTypes.Count; j++) {
				if (newNumber == chosenTypes[j])
					duplicateFound = true;
			}
			if (duplicateFound) continue;
			chosenTypes.Add(newNumber);
			chances[i] = monsterList[newNumber].chanceToSpawn;
			float[] values = new float[8];
			values[0] = monsterList[newNumber].healthPoints;
			values[1] = monsterList[newNumber].attackRange;
			values[2] = monsterList[newNumber].damage;
			values[3] = monsterList[newNumber].movementSpeed;
			values[4] = monsterList[newNumber].experiencePerKill;
			values[5] = monsterList[newNumber].goldPerKill;
			values[6] = monsterList[newNumber].attackCooldown;
			values[7] = monsterList[newNumber].visionRange;
			monsterNames[i] = monsterList[newNumber].name;
			baseValues.Add(values);
			i++;
		}
		dungeon.spawnMonsters (enemiesToSpawn, numberOfTypes, chances, monsterNames, baseValues);
		SpawnTerrain ();
		SpawnMonsters ();
		//monsterSpawner
		putPlayerToStartingPosition ();

	}

	public DungeonClass getDungeon() {
		return dungeon;
	}
}
