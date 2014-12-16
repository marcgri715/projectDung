using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomClass {

	public enum RoomType {CORRIDOR, RECTANGULAR, OCTAGONAL};

	int[] topLeftCorner;
	int[] bottomRightCorner;
	List<List<int>> roomConnector;  //0-N, 1-E, 2-W, 3-S [N-E-W-S]
	RoomType type;
	List<List<int[]>> wallTiles;


	public RoomClass(int pXCoordinate, int pYCoordinate, int pLength, int pHeight,
	          RoomType pType, int pNeighbourDirection=-1, int pNeighbourNumber=-1)
	{
		topLeftCorner = new int[2] {pXCoordinate, pYCoordinate};
		bottomRightCorner = new int[2] {pXCoordinate + pLength - 1, pYCoordinate + pHeight - 1};
		roomConnector = new List<List<int>>();
		for (int i=0; i<4; i++) {
			List<int> connectorSide = new List<int>();
			if (pNeighbourDirection==i) connectorSide.Add(pNeighbourNumber);
			roomConnector.Add(connectorSide);
		}
		type = pType;
		switch (type) {
		case RoomType.RECTANGULAR:
			int[] tile;
			List<int[]> topWall = new List<int[]> ();
			List<int[]> bottomWall = new List<int[]> ();
			List<int[]> leftWall = new List<int[]> ();
			List<int[]> rightWall = new List<int[]> ();
			for (int i=0; i<pHeight; i++) {
				tile = new int[2] {topLeftCorner [0], topLeftCorner [1] + i};
				leftWall.Add (tile);
				tile = new int[2] {bottomRightCorner [0], topLeftCorner [1] + i};
				rightWall.Add (tile);
			}
			for (int i=0; i<pLength; i++) {
				tile = new int[2] {topLeftCorner [0] + i, topLeftCorner [1]};
				topWall.Add (tile);
				tile = new int[2] {topLeftCorner [0] + i, bottomRightCorner [1]};
				bottomWall.Add (tile);
			}
			wallTiles = new List<List<int[]>> ();
			wallTiles.Add (topWall);
			wallTiles.Add (rightWall);
			wallTiles.Add (leftWall);
			wallTiles.Add (bottomWall);
			break;
		case RoomType.OCTAGONAL:
		//todo

			break;
		}
	}

	public int getLeftX() {
		return topLeftCorner [0];
	}

	public int getRightX() {
		return bottomRightCorner [0];
	}

	public int getTopY() {
		return topLeftCorner[1];
	}

	public int getBottomY() {
		return bottomRightCorner [1];
	}

	public int getNumberOfRoomConnectors(int pSide) {
		return roomConnector[pSide].Count;
	}

	public void addConnector(int pSide, int pRoomNumber) {
		roomConnector[pSide].Add (pRoomNumber);
	}

	public RoomType getRoomType() {
		return type;
	}
}
