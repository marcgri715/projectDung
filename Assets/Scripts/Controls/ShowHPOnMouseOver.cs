/* 
 * Imię i nazwisko autora: Marcin Griszbacher
 * Temat pracy: Gra RPG stworzona w oparciu o silnik Unity
 * 
 * Klasa ShowHPOnMouseOver
 * Zadanie: skrypt odpowiadający za pokazanie punktów życia żyjącego przeciwnika po najechaniu na niego kursorem myszy.
 * 			Działanie jest możliwe tylko pod warunkiem, że przeciwnik jest widziany przez naszą postać.
 */

using UnityEngine;
using System.Collections;

public class ShowHPOnMouseOver : MonoBehaviour {

	//Statystyki potwora
	private MonsterClass stats;
	//skrypt zawierający informacje o tym, czy wrogowie są widoczni
	private LookForMonsters visibilityCheck;
	//numer identyfikacyjny przeciwnika
	private int monsterIndex;

	void Start() {
		//ustalenie wartości początkowych dla danego przeciwnika
		GameObject newObject = gameObject;
		DefaultBehavior monster = (DefaultBehavior)newObject.GetComponent (typeof(DefaultBehavior));
		stats = monster.myStats;
		newObject = GameObject.Find ("Player");
		visibilityCheck = (LookForMonsters)newObject.GetComponent (typeof(LookForMonsters));
		monsterIndex = int.Parse (gameObject.name);
	}


	//jeśli kursor myszy najedzie na wroga
	void OnMouseEnter() {
		//i jest on widoczny
		if (visibilityCheck.isVisible(monsterIndex)) {
			//to przypisujemy wartości tego przeciwnika do klasy statycznej
			HighlightedEnemyHealthClass.isOverEnemy = true;
			HighlightedEnemyHealthClass.percentOfHP = stats.getHealthPoints () / stats.getMaxHealthPoints ();
			HighlightedEnemyHealthClass.enemyName = stats.getName();
		}
	}

	//dynamiczna zmiana wartości punktów życia wroga
	void OnMouseOver() {
		if (visibilityCheck.isVisible(monsterIndex)) {
			HighlightedEnemyHealthClass.percentOfHP = stats.getHealthPoints () / stats.getMaxHealthPoints ();
		}
	}

	//zakończenie wyświetlania wroga
	void OnMouseExit() {
		HighlightedEnemyHealthClass.isOverEnemy = false;
	}
}
