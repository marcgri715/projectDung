/* 
 * Imię i nazwisko autora: Marcin Griszbacher
 * Temat pracy: Gra RPG stworzona w oparciu o silnik Unity
 * 
 * Klasa HighlightedEnemyHealthClass
 * Zadanie: Skrypt zawierający klasę statyczną przechowującą informacje o tym, czy kursor znajduje się nad przeciwnikiem
 * 			oraz procent jego punktów życia.
 */

using UnityEngine;
using System.Collections;

public static class HighlightedEnemyHealthClass {
		
	//Flaga określająca, czy kursor znajduje się nad wrogiem
	public static bool isOverEnemy;
	//Zmienna przechowująca procent punktów życia wroga
	public static float percentOfHP;
	//Zmienna przechowująca nazwę wroga
	public static string enemyName;

}

