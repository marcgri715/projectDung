/* 
 * Imię i nazwisko autora: Marcin Griszbacher
 * Temat pracy: Gra RPG stworzona w oparciu o silnik Unity
 * 
 * Klasa DungeonCreator
 * Zadanie: skrypt, którego zadaniem jest uruchomienie klasy DungeonClass oraz na podstawie wyników jej działania
 * 			stworzenie na ekranie podziemi, w których będzie odbywać się gra.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class DungeonCreator : MonoBehaviour {


	//poziom podziemi
	public int dungeonLevel;
	//Parametr wielkości poziomu (n x n)
	public int levelSize;
	//Parametr z minimalną liczbą pól podłogi, które muszą zostać stworzone
	public int tilesToBeCreated;
	//Parametry minimalnej i maksymalnej wielkości pokoju
	public int minimumRoomSize;
	public int maximumRoomSize;
	//Parametr maksymalnej liczby prób stworzenia podziemi (patrz: DungeonClass)
	public int maxNumberOfTries;
	//Parametry minimalnej i maksymalnej długości korytarza 
	public int minimumCorridorLength;
	public int maximumCorridorLength;
	//Parametry prefabrykatów podłóg, ścian, drzwi, wyjścia na niższy poziom oraz obiektu gracza
	public GameObject floorTilePrefab = null;
	public GameObject wallPrefab = null;
	public GameObject player = null;
	public GameObject doorPrefab = null;
	public GameObject exitLadderPrefab = null;

	//Klasa opisująca parametry oraz prefabrykaty potworów wraz z szansą na wylosowanie tego rodzaju
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

	//Parametr określający liczbę rodzajów wrogów, którzy zostaną wylosowani spośród gatunków
	public int numberOfTypes;
	//Parametr określający liczbę wrogów, którzy pojawią się w podziemiach
	public int enemiesToSpawn;
	//Parametr  - lista z rodzajami poszczególnych gatunków (rodzajów) wrogów, którzy będą pojawiać się w podziemiach
	public List<Monsters> monsterList = new List<Monsters> (0);
	//Lista z wylosowanymi rodzajami wrogów
	private List<int> chosenTypes;

	//Obiekt klasy DungeonClass zawierający mapę lochów
	private DungeonClass dungeon;

	//Funkcja, która na podstawie map podłogi oraz ścian obiektu dungeon tworzy nowe obiekty w polu gry 
	void SpawnTerrain () {
		int elementNumber = 0;
		for (int i=0; i<levelSize; i++)
		for (int j=0; j<levelSize; j++) {		
			Vector3 spawnPoint = new Vector3(i,0,j);
			switch(dungeon.getTile(i,j)) {
			case 1:
				Instantiate(floorTilePrefab, spawnPoint, Quaternion.Euler(0,0,0));
				break;
			case 2:
				Instantiate(exitLadderPrefab,spawnPoint,Quaternion.Euler(0,0,0));
				break;
			}
			spawnPoint = new Vector3(i-0.5f,1.0f,j);
			switch(dungeon.getHorizontalEdge(i,j)) {
			case 1:
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
				Instantiate(wallPrefab,spawnPoint,Quaternion.Euler(0,90,0));
				break;
			case 2:
				GameObject door = (GameObject)Instantiate(doorPrefab,spawnPoint,Quaternion.Euler(0,90,0));
				door.name = "Door " + elementNumber++;
				break;
			}
		}
	}

	//Funkcja, która na podstawie listy wrogów znajdujących się w obiekcie dungeon tworzy wrogów
	//w odpowiednich pomieszczeniach
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

	//Funkcja ustawiająca postać gracza w pomieszczeniu centralnym podziemi
	void putPlayerToStartingPosition () {
		Vector3 startPos = new Vector3 ();
		startPos.x = (dungeon.getRoom(0).getRightX () - dungeon.getRoom(0).getLeftX ()) / 2 + dungeon.getRoom(0).getLeftX();
		startPos.z = (dungeon.getRoom(0).getBottomY () - dungeon.getRoom(0).getTopY ()) / 2 + dungeon.getRoom(0).getTopY();
		startPos.y = 0.35f;
		player.transform.Translate (startPos);
		//Dynamiczne przypisanie liczby wrogów do komponentu postaci gracza LookForMonsters
		LookForMonsters looker = (LookForMonsters)player.transform.gameObject.GetComponent (typeof(LookForMonsters));
		looker.numberOfEnemies = enemiesToSpawn;
	}
		
		
	void Start () {
		//pobranie poziomu podziemi z klasy statycznej
		dungeonLevel = StatsClass.dungeonLevel;
		Random.seed = (int)System.DateTime.Now.Ticks;
		//wygenerowanie podziemi
		dungeon = new DungeonClass (dungeonLevel);
		dungeon.generateNewDungeon (levelSize, tilesToBeCreated, minimumRoomSize, maximumRoomSize,
		                           maxNumberOfTries, minimumCorridorLength, maximumCorridorLength);
		//wybór typów (gatunków) wrogów, którzy muszą zostać wybrani
		chosenTypes = new List<int>();
		int i = 0;
		int[] chances = new int[numberOfTypes];
		List<float[]> baseValues = new List<float[]>();
		string[] monsterNames = new string[numberOfTypes]; 
		//Powtarzaj aż znajdziesz wystarczającą liczbę typów wrogów
		while (i<numberOfTypes) {
			//wylosuj wroga, który jeszcze nie znajduje się na liście
			int newNumber = Random.Range(0,monsterList.Count);
			bool duplicateFound = false;
			for (int j=0; j<chosenTypes.Count; j++) {
				if (newNumber == chosenTypes[j])
					duplicateFound = true;
			}
			if (duplicateFound) continue;
			//dodaj do odpowiednich list i tablic jego parametry
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
		//Wygeneruj przeciwników
		dungeon.spawnMonsters (enemiesToSpawn, numberOfTypes, chances, monsterNames, baseValues);
		//Wygeneruj obiekty terenu oraz przeciwników, a następnie ustaw gracza w pokoju początkowym
		SpawnTerrain ();
		SpawnMonsters ();
		putPlayerToStartingPosition ();

	}

	//Zwraca obiekt zawierający mapę podziemi
	public DungeonClass getDungeon() {
		return dungeon;
	}
}
