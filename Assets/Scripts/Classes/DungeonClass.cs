/* 
 * Imię i nazwisko autora: Marcin Griszbacher
 * Temat pracy: Gra RPG stworzona w oparciu o silnik Unity
 * 
 * Klasa DungeonClass
 * Zadanie: skrypt odpowiadający za stworzenie mapy danego poziomu podziemi na podstawie otrzymanych parametrów
 * 			oraz na umieszczeniu przeciwników w różnych pokojach.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DungeonClass {

	//obecny poziom podziemi
	private int dungeonLevel;
	//tablica dwuwymiarowa opisująca podłogę; wartości: 0 - puste pole, 1 - podłoga, 2 - zejście na kolejny poziom
	private short[,] tiles;
	//lista pomieszczeń znajdujących się na danym poziomie
	private List<RoomClass> spawnedRooms;
	//tablice dwuwymiarowe opisujące ściany; wartości - 0 - puste pole, 1 - ściana, 2 - drzwi
	private short[,] horizontalEdges;
	private short[,] verticalEdges;
	//lista stworów znajdujących się w podziemiach
	private List<MonsterClass> spawnedMonsters;
	//lista wielkości poszczególnych pokojów (indeksy pokojów odpowiadają indeksom z listy spawnedRooms)
	private List<int> roomSizes;
	//rozmiar wszystkich pokojów
	private int totalDungeonSize;


	//konstruktor
	public DungeonClass(int pDungeonLevel) {
		dungeonLevel = pDungeonLevel;
	}

	//Funkcja zawierająca algorytm losowo tworzący podziemia na podstawie określonych parametrów
	public void generateNewDungeon (int levelSize, int tilesToBeCreated, int minimumRoomSize, int maximumRoomSize,
	                                int maxNumberOfTries, int minimumCorridorLength, int maximumCorridorLength) {
		//stworzenie tablic, które będą zawierać informacje o podłodze oraz ścianach
		tiles = new short[levelSize,levelSize];
		horizontalEdges = new short[levelSize + 1,levelSize + 1];
		verticalEdges = new short[levelSize + 1,levelSize + 1];
		for (int i=0; i<levelSize; i++)
			for (int j=0; j<levelSize; j++) {
				tiles[i,j]=0;
				horizontalEdges[i,j] = verticalEdges[i,j] = 0;
			}
		for (int i=0; i<levelSize+1; i++)
			horizontalEdges [levelSize,i] = verticalEdges [levelSize,i] = 0;
		Random.seed = (int)System.DateTime.Now.Ticks;
		//stworzenie nowych list: pokojów oraz ich rozmiarów
		spawnedRooms = new List<RoomClass> ();
		roomSizes = new List<int> ();
		totalDungeonSize = 0;
		//wyzerowanie licznika prób
		int numberOfTries = 0;
		//pokój startowy
		do 
		{
			RoomClass newRoom;
			//tworzenie pierwszego pokoju
			if (numberOfTries == 0)
			{
				//losowy wybór rozmiarów oraz punktu startowego
				int horizontalSize = Random.Range(minimumRoomSize, maximumRoomSize);
				int verticalSize = Random.Range(minimumRoomSize, maximumRoomSize);
				int xStart = Random.Range(0, levelSize - horizontalSize);
				int yStart = Random.Range(0, levelSize - verticalSize);
				//stworzenie nowego prostokątnego pokoju
				newRoom = new RoomClass(xStart, yStart, horizontalSize, verticalSize, 
				                        RoomClass.RoomType.RECTANGULAR);
				//zwiększenie liczby zajętych pól 
				totalDungeonSize += horizontalSize*verticalSize;
			}
			//stworzenie kolejnego pokoju
			else
			{
				//wybór jednego z istniejących pokojów oraz jednej ze ścian wybranego pokoju
				int chosenRoom = Random.Range(0, spawnedRooms.Count);
				int chosenSide = Random.Range(0, 4);
				//wylosuj rodzaj nowego pomieszczenia - 30% korytarzy, 70% pomieszczeń prostokątnych
				int typeDraw = Random.Range(0,1000);
				RoomClass.RoomType newType;
				if (typeDraw<300)
					newType = RoomClass.RoomType.CORRIDOR;
				else 
					newType = RoomClass.RoomType.RECTANGULAR;
				int horizontalSize=0, verticalSize=0;
				//określenie rozmiarów nowego pomieszczenia
				//korytarze zawsze mają szerokość 3 pól
				if (newType == RoomClass.RoomType.CORRIDOR) {
					if (chosenSide==1 || chosenSide==2)
					{
						horizontalSize = Random.Range(minimumCorridorLength, maximumCorridorLength);
						verticalSize = 3;
					} else {
						horizontalSize = 3;
						verticalSize = Random.Range (minimumCorridorLength, maximumCorridorLength);
					}
				}
				else if (newType == RoomClass.RoomType.RECTANGULAR) {
					horizontalSize = Random.Range(minimumRoomSize, maximumRoomSize);
					verticalSize = Random.Range(minimumRoomSize, maximumRoomSize);
				}
				//losowanie miejsca na nowy pokój
				int xStart=0, yStart=0, xEnd=0, yEnd=0, newDoorX=0, newDoorY=0;
				switch (chosenSide)
				{
				case 0: //ściana północna
					yEnd = spawnedRooms[chosenRoom].getTopY()-1;
					newDoorY = yEnd+1;
					xStart = Random.Range(spawnedRooms[chosenRoom].getLeftX(), spawnedRooms[chosenRoom].getRightX()-1);
					newDoorX = xStart+1;
					xStart -= Random.Range(0, horizontalSize-2);
					yStart = yEnd-verticalSize+1;
					xEnd = xStart+horizontalSize-1;
					break;
				case 1: //ściana wschodnia
					xStart=spawnedRooms[chosenRoom].getRightX()+1;
					newDoorX = xStart;
					yStart = Random.Range(spawnedRooms[chosenRoom].getTopY(), spawnedRooms[chosenRoom].getBottomY()-1);
					newDoorY = yStart+1;
					yStart -= Random.Range(0, verticalSize-2);
					xEnd = xStart+horizontalSize-1;
					yEnd = yStart+verticalSize-1;
					break;
				case 2: //ściana zachodnia
					xEnd=spawnedRooms[chosenRoom].getLeftX()-1;
					newDoorX = xEnd+1;
					yStart = Random.Range(spawnedRooms[chosenRoom].getTopY(), spawnedRooms[chosenRoom].getBottomY()-1);
					newDoorY = yStart+1;
					yStart -= Random.Range(0, verticalSize-2);
					xStart = xEnd-horizontalSize+1;
					yEnd = yStart+verticalSize-1;
					break;
				case 3: //ściana południowa
					yStart = spawnedRooms[chosenRoom].getBottomY()+1;
					newDoorY = yStart;
					xStart = Random.Range(spawnedRooms[chosenRoom].getLeftX(), spawnedRooms[chosenRoom].getRightX()-1);
					newDoorX = xStart+1;
					xStart -= Random.Range(0, horizontalSize-2);
					yEnd = yStart+verticalSize-1;
					xEnd = xStart+horizontalSize-1;
					break;
				}
				//sprawdzenie czy pokój mieści się w podziemiach
				if (xStart<0||yStart<0||xEnd>=levelSize||yEnd>=levelSize)
					continue;
				//sprawdzenie czy nowy pokój nie koliduje z już istniejącym
				bool roomCollision = false;
				for (int i=0; i<spawnedRooms.Count; i++) {
					if (i==chosenRoom) continue;
					if ((spawnedRooms[i].getLeftX()<=xStart && spawnedRooms[i].getRightX()>=xStart)
					    ||(spawnedRooms[i].getLeftX()<=xEnd && spawnedRooms[i].getRightX()>=xEnd)
					    ||(spawnedRooms[i].getLeftX()>=xStart && spawnedRooms[i].getRightX()<=xEnd))
						if ((spawnedRooms[i].getTopY()<=yStart && spawnedRooms[i].getBottomY()>=yStart)
						    ||(spawnedRooms[i].getTopY()<=yEnd && spawnedRooms[i].getBottomY()>=yEnd)
						   ||(spawnedRooms[i].getTopY()>=yStart && spawnedRooms[i].getBottomY()<=yEnd)) {
						roomCollision = true;
						break;
					}
				}
				if (roomCollision) continue;
				//jeśli nie koliduje, stwórz pokój o podanych parametrach oraz dodaj połączenie z nim do pokoju leżącego
				//obok
				spawnedRooms[chosenRoom].addConnector(chosenSide,spawnedRooms.Count);
				switch (chosenSide) {
				case 0:
					chosenSide = 3;
					break;
				case 1:
					chosenSide = 2;
					break;
				case 2:
					chosenSide = 1;
					break;
				case 3:
					chosenSide = 0;
					break;
				}
				newRoom = new RoomClass(xStart, yStart, horizontalSize, verticalSize, newType, chosenSide, chosenRoom);
				totalDungeonSize += horizontalSize*verticalSize;
				//ustalenie miejsca, gdzie leżą drzwi
				if (chosenSide == 1 || chosenSide == 2)
					horizontalEdges[newDoorX,newDoorY] = 2;
				else
					verticalEdges[newDoorX,newDoorY] = 2;
			}
			//dodaj nowy pokój oraz jego rozmiar do odpowiednich list
			spawnedRooms.Add(newRoom);
			roomSizes.Add((newRoom.getRightX()-newRoom.getLeftX()) * (newRoom.getBottomY() - newRoom.getTopY()));
		} while (totalDungeonSize<tilesToBeCreated && numberOfTries++<maxNumberOfTries);
		//warunek zakończenia pętli: wystarczająco dużo pól zajętych bądź przekroczony limit poszukiwań pokojów

		//po stworzeniu pokojów - stworzenie mapy podłóg i ścian we wszystkich tablicach dwuwymiarowych
		for (int i=0; i<spawnedRooms.Count; i++) {
			//zwykła podłoga - 1
			for (int j=spawnedRooms[i].getLeftX(); j<=spawnedRooms[i].getRightX(); j++) {
				for (int k=spawnedRooms[i].getTopY(); k<=spawnedRooms[i].getBottomY(); k++) {
					tiles[j,k] = 1;
				}
			}
			//jeśli ściana nie jest już określona jako drzwi, to ma nią zostać
			for (int j=spawnedRooms[i].getLeftX(); j<=spawnedRooms[i].getRightX(); j++) {
				if(verticalEdges[j, spawnedRooms[i].getTopY()]<2)
					verticalEdges[j, spawnedRooms[i].getTopY()] = 1;
				if(verticalEdges[j, spawnedRooms[i].getBottomY()+1]<2)
					verticalEdges[j, spawnedRooms[i].getBottomY()+1] = 1;
			}
			for (int j=spawnedRooms[i].getTopY(); j<=spawnedRooms[i].getBottomY(); j++) {
				if(horizontalEdges[spawnedRooms[i].getLeftX(), j]<2)
					horizontalEdges[spawnedRooms[i].getLeftX(), j] = 1;
				if(horizontalEdges[spawnedRooms[i].getRightX()+1, j]<2)
					horizontalEdges[spawnedRooms[i].getRightX()+1, j] = 1;
			}
		}
		//wybór pokoju, w którym będzie znajdować się wyjście oraz zastąpienie jednej z podłóg w nim wyjściem
		int exitRoomNumber = Random.Range (0, spawnedRooms.Count);
		tiles[Random.Range(spawnedRooms[exitRoomNumber].getLeftX()+1,spawnedRooms[exitRoomNumber].getRightX()),
		      Random.Range(spawnedRooms[exitRoomNumber].getTopY()+1,spawnedRooms[exitRoomNumber].getBottomY())] = 2;
	}




	//Funkcja tworząca wrogów w różnych rodzajach oraz umieszczająca ich w losowo wybranych pomieszczeniach na podstawie
	//podanych parametrów
	public void spawnMonsters (int numberOfMonsters, int numberOfTypes, int[] chanceToSpawn, string[] monsterNames,
	                           List<float[]> baseValues) {
		spawnedMonsters = new List<MonsterClass>();
		for (int i=0; i<numberOfMonsters; i++) {
			int totalChance = 0;
			for (int j=0; j<chanceToSpawn.Length; j++) {
				totalChance += chanceToSpawn[j];
			}
			//losowanie jednego z typów wrogów na podstawie parametru opisującego szanse na wybór tego przeciwnika
			int selectedChance = Random.Range(0,totalChance);
			int selectedType, startingRoom;
			for (selectedType = 0; selectedType<chanceToSpawn.Length-1; selectedType++) {
				if (selectedChance < chanceToSpawn[selectedType])
					break;
				else
					selectedChance -= chanceToSpawn[selectedType];
			}
			//losowanie pokoju, w którym zostanie stworzony przeciwnik na podstawie wielkości pokojów 
			//(czym większy pokój, tym większa szansa na stworzenie w nim przeciwnika)
			//Pierwszy pokój wyłączony z losowania
			selectedChance = Random.Range(roomSizes[0], totalDungeonSize);
			for (startingRoom = 0; startingRoom<roomSizes.Count-1; startingRoom++) {
				if (selectedChance < roomSizes[startingRoom])
					break;
				else
					selectedChance -= roomSizes[startingRoom];
			}
			//stworzenie przeciwnika i dodanie go do listy
			MonsterClass monster = new MonsterClass(startingRoom, selectedType, dungeonLevel, monsterNames[selectedType],
			                                        baseValues[selectedType]);
			spawnedMonsters.Add(monster);
		}
	}

	//Funkcja zwracająca wartość podłogi znajdującej się na miejscu o współrzędnych (pX, pY)
	public short getTile (int pX, int pY) {
		return tiles[pX, pY];
	}

	//Funkcja zwracająca wartość ściany "poziomej" znajdującej się na miejscu o współrzędnych (pX, pY)
	public short getHorizontalEdge (int pX, int pY) {
		return horizontalEdges[pX, pY];
	}

	//Funkcja zwracająca wartość ściany "pionowej" znajdującej się na miejscu o współrzędnych (pX, pY)
	public short getVerticalEdge (int pX, int pY) {
		return verticalEdges [pX, pY];
	}

	//Funkcja zwracająca pokój o podanym indeksie
	public RoomClass getRoom (int pIndex) {
		return spawnedRooms [pIndex];
	}

	//Funkcja zwracająca przeciwnika o podanym indeksie
	public MonsterClass getMonster (int pIndex) {
		return spawnedMonsters [pIndex];
	}

	//funkcja zwracająca liczbę wrogów
	public int getNumberOfMonsters () {
		return spawnedMonsters.Count;
	}

	//funkcja zwracająca obecny poziom podziemi
	public int getLevel () {
		return dungeonLevel;
	}
}
