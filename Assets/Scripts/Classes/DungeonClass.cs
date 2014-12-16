using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DungeonClass {

	private int dungeonLevel;
	private short[,] tiles;
	private List<RoomClass> spawnedRooms;
	private short[,] horizontalEdges;
	private short[,] verticalEdges;
	private List<MonsterClass> spawnedMonsters;
	private List<int> roomSizes;
	private int totalDungeonSize;


	
	public DungeonClass(int pDungeonLevel) {
		dungeonLevel = pDungeonLevel;
	}

	public void generateNewDungeon (int levelSize, int tilesToBeCreated, int minimumRoomSize, int maximumRoomSize,
	                                int maxNumberOfTries, int minimumCorridorLength, int maximumCorridorLength) {

		tiles = new short[levelSize,levelSize];
		horizontalEdges = new short[levelSize + 1,levelSize + 1]; //0-nothing, 1-wall, 2-door
		verticalEdges = new short[levelSize + 1,levelSize + 1];
		for (int i=0; i<levelSize; i++)
		for (int j=0; j<levelSize; j++) {
			tiles[i,j]=0;
			horizontalEdges[i,j] = verticalEdges[i,j] = 0;
		}
		for (int i=0; i<levelSize+1; i++)
			horizontalEdges [levelSize,i] = verticalEdges [levelSize,i] = 0;
		int created = 0;
		Random.seed = (int)System.DateTime.Now.Ticks;
		spawnedRooms = new List<RoomClass> ();
		roomSizes = new List<int> ();
		totalDungeonSize = 0;
		int numberOfTries = 0;
		//pokój startowy
		do 
		{
			RoomClass newRoom;
			if (numberOfTries == 0) //first room
			{
				int horizontalSize = Random.Range(minimumRoomSize, maximumRoomSize);
				int verticalSize = Random.Range(minimumRoomSize, maximumRoomSize);
				int xStart = Random.Range(0, levelSize - horizontalSize);
				int yStart = Random.Range(0, levelSize - verticalSize);
				newRoom = new RoomClass(xStart, yStart, horizontalSize, verticalSize, 
				                        RoomClass.RoomType.RECTANGULAR);
				created += horizontalSize*verticalSize;
			}
			else
			{
				int chosenRoom = Random.Range(0, spawnedRooms.Count);
				int chosenSide = Random.Range(0, 4);
				if (spawnedRooms[chosenRoom].getRoomType()==RoomClass.RoomType.OCTAGONAL && 
				    spawnedRooms[chosenRoom].getNumberOfRoomConnectors(chosenSide)>0)
					continue;
				int typeDraw = Random.Range(0,1000);
				RoomClass.RoomType newType;
				if (typeDraw<300)
					newType = RoomClass.RoomType.CORRIDOR;
				else if (typeDraw<950)
					newType = RoomClass.RoomType.RECTANGULAR;
				else
					newType = RoomClass.RoomType.OCTAGONAL;
				/*if (newType!=RoomClass.RoomType.CORRIDOR && 
				    spawnedRooms[chosenRoom].getRoomType()!=RoomClass.RoomType.CORRIDOR)
					continue;*/
				int horizontalSize=0, verticalSize=0;
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
				else if (newType == RoomClass.RoomType.OCTAGONAL) {
					horizontalSize=Random.Range(minimumRoomSize, maximumRoomSize);
					horizontalSize -= horizontalSize%3;
					if (horizontalSize%2==0) horizontalSize-=3;
					verticalSize = horizontalSize;
				}
				int xStart=0, yStart=0, xEnd=0, yEnd=0, newDoorX=0, newDoorY=0;
				switch (chosenSide)
				{
				case 0: //n
					yEnd = spawnedRooms[chosenRoom].getTopY()-1;
					newDoorY = yEnd+1;
					xStart = Random.Range(spawnedRooms[chosenRoom].getLeftX(), spawnedRooms[chosenRoom].getRightX()-1);
					newDoorX = xStart+1;
					xStart -= Random.Range(0, horizontalSize-2);
					yStart = yEnd-verticalSize+1;
					xEnd = xStart+horizontalSize-1;
					break;
				case 1: //e
					xStart=spawnedRooms[chosenRoom].getRightX()+1;
					newDoorX = xStart;
					yStart = Random.Range(spawnedRooms[chosenRoom].getTopY(), spawnedRooms[chosenRoom].getBottomY()-1);
					newDoorY = yStart+1;
					yStart -= Random.Range(0, verticalSize-2);
					xEnd = xStart+horizontalSize-1;
					yEnd = yStart+verticalSize-1;
					break;
				case 2: //w
					xEnd=spawnedRooms[chosenRoom].getLeftX()-1;
					newDoorX = xEnd+1;
					yStart = Random.Range(spawnedRooms[chosenRoom].getTopY(), spawnedRooms[chosenRoom].getBottomY()-1);
					newDoorY = yStart+1;
					yStart -= Random.Range(0, verticalSize-2);
					xStart = xEnd-horizontalSize+1;
					yEnd = yStart+verticalSize-1;
					break;
				case 3:
					yStart = spawnedRooms[chosenRoom].getBottomY()+1;
					newDoorY = yStart;
					xStart = Random.Range(spawnedRooms[chosenRoom].getLeftX(), spawnedRooms[chosenRoom].getRightX()-1);
					newDoorX = xStart+1;
					xStart -= Random.Range(0, horizontalSize-2);
					yEnd = yStart+verticalSize-1;
					xEnd = xStart+horizontalSize-1;
					break;
				}
				if (xStart<0||yStart<0||xEnd>=levelSize||yEnd>=levelSize)
					continue;
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
				if (newType == RoomClass.RoomType.CORRIDOR || newType == RoomClass.RoomType.RECTANGULAR)
					created += horizontalSize*verticalSize;
				else if (newType == RoomClass.RoomType.OCTAGONAL)
					created += horizontalSize*verticalSize - horizontalSize/3*4;
				if (chosenSide == 1 || chosenSide == 2)
					horizontalEdges[newDoorX,newDoorY] = 2;
				else
					verticalEdges[newDoorX,newDoorY] = 2;
			}
			spawnedRooms.Add(newRoom);
			if (newRoom.getRoomType() == RoomClass.RoomType.OCTAGONAL) {
				//todo: calculate exact
				roomSizes.Add((newRoom.getRightX()-newRoom.getLeftX()) * (newRoom.getBottomY() - newRoom.getTopY()));
			} else {
				roomSizes.Add((newRoom.getRightX()-newRoom.getLeftX()) * (newRoom.getBottomY() - newRoom.getTopY()));
			}
		} while (created<tilesToBeCreated && numberOfTries++<maxNumberOfTries);
		for (int i=0; i<spawnedRooms.Count; i++) {
			totalDungeonSize += roomSizes[i];
			for (int j=spawnedRooms[i].getLeftX(); j<=spawnedRooms[i].getRightX(); j++) {
				for (int k=spawnedRooms[i].getTopY(); k<=spawnedRooms[i].getBottomY(); k++) {
					tiles[j,k] = 1;
				}
			}
			if (spawnedRooms[i].getRoomType() == RoomClass.RoomType.OCTAGONAL) {
				//todo: ustaw kształt - usuń trójkąciki
			}
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
		int exitRoomNumber = Random.Range (0, spawnedRooms.Count);
		tiles[Random.Range(spawnedRooms[exitRoomNumber].getLeftX()+1,spawnedRooms[exitRoomNumber].getRightX()),
		      Random.Range(spawnedRooms[exitRoomNumber].getTopY()+1,spawnedRooms[exitRoomNumber].getBottomY())] = 2;
	}


	public void spawnMonsters (int numberOfMonsters, int numberOfTypes, int[] chanceToSpawn, string[] monsterNames,
	                           List<float[]> baseValues) {
		spawnedMonsters = new List<MonsterClass>();
		for (int i=0; i<numberOfMonsters; i++) {
			int totalChance = 0;
			for (int j=0; j<chanceToSpawn.Length; j++) {
				totalChance += chanceToSpawn[j];
			}
			int selectedChance = Random.Range(0,totalChance);
			int selectedType, startingRoom;
			for (selectedType = 0; selectedType<chanceToSpawn.Length-1; selectedType++) {
				if (selectedChance < chanceToSpawn[selectedType])
					break;
				else
					selectedChance -= chanceToSpawn[selectedType];
			}
			selectedChance = Random.Range(roomSizes[0], totalDungeonSize);
			for (startingRoom = 0; startingRoom<roomSizes.Count-1; startingRoom++) {
				if (selectedChance < roomSizes[startingRoom])
					break;
				else
					selectedChance -= roomSizes[startingRoom];
			}
			MonsterClass monster = new MonsterClass(startingRoom, selectedType, dungeonLevel, monsterNames[selectedType],
			                                        baseValues[selectedType]);
			spawnedMonsters.Add(monster);
		}
	}

	public short getTile (int pX, int pY) {
		return tiles[pX, pY];
	}

	public short getHorizontalEdge (int pX, int pY) {
		return horizontalEdges[pX, pY];
	}

	public short getVerticalEdge (int pX, int pY) {
		return verticalEdges [pX, pY];
	}

	public RoomClass getRoom (int pIndex) {
		return spawnedRooms [pIndex];
	}

	public MonsterClass getMonster (int pIndex) {
		return spawnedMonsters [pIndex];
	}

	public int getNumberOfMonsters () {
		return spawnedMonsters.Count;
	}

	public int getLevel () {
		return dungeonLevel;
	}
}
