/* 
 * Imię i nazwisko autora: Marcin Griszbacher
 * Temat pracy: Gra RPG stworzona w oparciu o silnik Unity
 * 
 * Klasa RoomClass
 * Zadanie: skrypt przechowujący klasę opisującą pomieszczenie w podziemiach - górny lewy oraz prawy dolny narożnik,
 * 			połączenia z innymi pokojami, rodzaj pokoju oraz ściany.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomClass {

	//Rodzaj pokoju - korytarz bądź pomieszczenie prostokątne
	public enum RoomType {CORRIDOR, RECTANGULAR};

	//Współrzędne górnego lewego rogu pokoju [x, y]
	int[] topLeftCorner;
	//Współrzędne dolnego prawego rogu pokoju
	int[] bottomRightCorner;
	//Lista połączeń z innymi pokojami (wartości listy wewnętrznej - numery poszczególnych pokojów;
	//									wartości listy zewnętrznej - ściana, przy której dany pokój się znajduje
	// 0 - ściana północna, 1 - ściana wschodnia, 2 - ściana zachodnia, 3 - ściana południowa
	List<List<int>> roomConnector;
	//Rodzaj pokoju
	RoomType type;


	//Konstruktor - tworzy pokój w zależności od koordynatów; domyślne wartości -1 zarezerwowane są dla pierwszego
	//pokoju, nie posiadającego jeszcze sąsiadów
	public RoomClass(int pXCoordinate, int pYCoordinate, int pLength, int pHeight,
	          RoomType pType, int pNeighbourDirection=-1, int pNeighbourNumber=-1)
	{
		topLeftCorner = new int[2] {pXCoordinate, pYCoordinate};
		bottomRightCorner = new int[2] {pXCoordinate + pLength - 1, pYCoordinate + pHeight - 1};
		type = pType;
		roomConnector = new List<List<int>>();
		for (int i=0; i<4; i++) {
			List<int> connectorSide = new List<int>();
			//jeśli pokój, przy którym zostaje stworzony nowy pokój znajduje się przy danej ścianie - dodaj go do listy
			if (pNeighbourDirection==i) connectorSide.Add(pNeighbourNumber);
				roomConnector.Add(connectorSide);
		}
	}

	//Zwróć najbardziej wysuniętą na zachód współrzędną
	public int getLeftX() {
		return topLeftCorner [0];
	}

	//Zwróć najbardziej wysuniętą na wschód współrzędną
	public int getRightX() {
		return bottomRightCorner [0];
	}

	//Zwróć najbardziej wysuniętą na północ współrzędną
	public int getTopY() {
		return topLeftCorner[1];
	}

	//Zwróć najbardziej wysuniętą na południe współrzędną
	public int getBottomY() {
		return bottomRightCorner [1];
	}

	//Dodaj połączenie z nowo utworzonym pokojem
	public void addConnector(int pSide, int pRoomNumber) {
		roomConnector[pSide].Add (pRoomNumber);
	}

	//Zwróć rodzaj pokoju
	public RoomType getRoomType() {
		return type;
	}
}
