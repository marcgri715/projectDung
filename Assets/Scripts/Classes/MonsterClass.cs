/* 
 * Imię i nazwisko autora: Marcin Griszbacher
 * Temat pracy: Gra RPG stworzona w oparciu o silnik Unity
 * 
 * Klasa MonsterClass
 * Zadanie: skrypt zawierający klasę potwora, która zawiera jego nazwę, statystyki, rodzaj oraz pokój, w którym
 * 			zostanie umieszczony podczas generowania podziemi.
 */

using UnityEngine;
using System.Collections;

public class MonsterClass {

	//Pokój, w którym zostaje stworzony przeciwnik
	private int startingRoom;
	//Typ (gatunek) wroga
	private int type;
	//Nazwa (gatunku) wroga
	private string name;
	//Obecna wartość punktów życia
	private float healthPoints;
	//Maksymalna wartość punktów życia
	private float maxHealthPoints;
	//Zasięg ataku
	private float attackRange;
	//Obrażenia
	private float damage;
	//Prędkość poruszania się
	private float movementSpeed;
	//Doświadczenie otrzymywane przez gracza po zabiciu przeciwnika
	private float experiencePerKill;
	//Złoto otrzymywane przez gracza po zabiciu przeciwnika
	private float goldPerKill;
	//Czas oczekiwania między kolejnymi atakami (w sek.)
	private float attackCooldown;
	//Zasięg wzroku
	private float visionRange;


	//Konstruktor - część parametrów zależna jest od poziomu podziemi, na którym zostaje stworzony przeciwnik
	public MonsterClass(int pStartingRoom, int pType, int pLevel, string pName, float[] baseValues) {
		startingRoom = pStartingRoom;
		type = pType;
		name = pName;
		maxHealthPoints = healthPoints = baseValues[0] * pLevel;
		attackRange = baseValues[1];
		damage = baseValues[2] * pLevel;
		movementSpeed = baseValues[3];
		experiencePerKill = baseValues[4] * pLevel;
		goldPerKill = baseValues[5] * pLevel;
		attackCooldown = baseValues[6];
		visionRange = baseValues[7];
	}

	//Funkcja zwracająca numer pokoju początkowego przeciwnika
	public int getStartingRoom() {
		return startingRoom;
	}
	
	//Funkcja zadająca obrażenia przeciwnikowi
	public void damageMonster(float damageDone) {
		healthPoints -= damageDone;
	}
	
	//Funkcja zwracająca nazwę przeciwnika
	public string getName () {
		return name;
	}
	
	//Funkcja zwracająca maksymalną wartość punktów życia
	public float getMaxHealthPoints() {
		return maxHealthPoints;
	}

	//Funkcja zwracająca obecną wartość punktów życia
	public float getHealthPoints() {
		return healthPoints;
	}

	//Funkcja zwracająca zasięg ataku
	public float getAttackRange() {
		return attackRange;
	}

	//Funkcja zwracająca obrażenia zadawane przez przeciwnika
	public float getDamage() {
		return damage;
	}

	//Funkcja zwracająca prędkość poruszania się
	public float getMovementSpeed() {
		return movementSpeed;
	}

	//Funkcja zwracająca doświadczenie przyznawane po zabiciu wroga
	public float getExpPerKill() {
		return experiencePerKill;
	}

	//Funkcja zwracająca złoto przyznawane po zabiciu wroga
	public float getGoldPerKill() {
		return goldPerKill;
	}

	//Funkcja zwracająca czas "odpoczynku" między atakami
	public float getAttackCooldown() {
		return attackCooldown;
	}

	//Funkcja zwracająca zasięg wzroku przeciwnika
	public float getVisionRange() {
		return visionRange;
	}

	//Funkcja zwracająca typ (gatunek) przeciwnika
	public int getMonsterType() {
		return type;
	}
}
