/* 
 * Imię i nazwisko autora: Marcin Griszbacher
 * Temat pracy: Gra RPG stworzona w oparciu o silnik Unity
 * 
 * Klasa PlayerStatsClass
 * Zadanie: skrypt zawierający klasę przechowującą informacje o statystykach oraz zdobytym złocie i doświadczeniu
 * 			postaci gracza.
 */

using UnityEngine;
using System.Collections;

public class PlayerStatsClass {

	//Wartość złota posiadanego przez gracza
	private uint gold;
	//Wartość doświadczenia posiadanego przez gracza
	private uint experience;
	//Obecna wartość punktów życia
	private float currentHealthPoints;
	//Maksymalna wartość punktów życia
	private float maxHealthPoints;
	//Prędkość ataku gracza (mnożnik)
	private float attackSpeed;
	//Dolny próg obrażeń zadawanych przez gracza
	private float minAttackDamage;
	//Górny próg obrażeń zadawanych przez gracza
	private float maxAttackDamage;
	//Wartość procentowa wysysania życia
	private int lifeSteal;
	//Wartość odporności na ataki wroga
	private int damageReduction;
	//Szansa na uniknięcie ataku wroga
	private int dodgeChance;


	//Konstruktor
	public PlayerStatsClass() {
		gold = StatsClass.gold;
		experience = StatsClass.experience;
		currentHealthPoints = maxHealthPoints = StatsClass.maxHealthPoints;
		attackSpeed = StatsClass.attackSpeed;
		minAttackDamage = StatsClass.minAttackDamage;
		maxAttackDamage = StatsClass.maxAttackDamage;
		lifeSteal = StatsClass.lifeSteal;
		damageReduction = StatsClass.damageReduction;
		dodgeChance = StatsClass.dodgeChance;
	}

	//Dodanie podanej wartości złota do puli złota gracza
	public void addGold (float value) {
		if (value >= 0) 
			gold += (uint)value;
		else
			gold -= (uint)-value;
	}

	//Dodanie podanej wartości doświadczenia do puli doświadczenia gracza
	public void addExperience (float value) {
		if (value >= 0) 
			experience += (uint)value;
		else
			experience -= (uint)-value;
	}

	//Zwraca wartość złota
	public uint getGold () {
		return gold;
	}

	//Zwraca wartość doświadczenia
	public uint getExperience () {
		return experience;
	}

	//Zadaje obrażenia graczowi
	public void getHurt (float damage) {
		//próba uniknięcia ataku
		int isDodged = Random.Range (0, 100);
		if (isDodged >= dodgeChance) {
			//zmniejszenie zadanych graczowi obrażeń w zależności od wartości redukcji
			float reductedDamage = damage * 100f / (100f + damageReduction);
			currentHealthPoints -= reductedDamage;
		}
	}

	//Zwraca obecną wartość punktów życia
	public float getCurrentHP () {
		return currentHealthPoints;
	}

	//Zwraca maksymalną wartość punktów życia
	public float getMaxHP () {
		return maxHealthPoints;
	}

	//Zwraca prędkość ataku
	public float getAttackSpeed () {
		return attackSpeed;
	}

	//Zwraca dolny próg obrażeń zadawanych przez gracza
	public float getMinDamage () {
		return minAttackDamage;
	}
	
	//Zwraca górny próg obrażeń zadawanych przez gracza
	public float getMaxDamage () {
		return maxAttackDamage;
	}

	//Wykrada pewien procent zadanych obrażeń jako życie, lecząc postać
	public void stealLife (float damageDone) {
		currentHealthPoints += damageDone * (lifeSteal / 100f);
		//jeśli obecna wartość pktów życia jest większa od maksymalnej, to zostaje przycięta do tego progu 
		if (currentHealthPoints > maxHealthPoints)
			currentHealthPoints = maxHealthPoints;
	}
}
