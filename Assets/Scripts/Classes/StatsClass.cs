/* 
 * Imię i nazwisko autora: Marcin Griszbacher
 * Temat pracy: Gra RPG stworzona w oparciu o silnik Unity
 * 
 * Klasa StatsClass
 * Zadanie: skrypt zawierający klasę statyczną, która służy do przenoszenia statystyk oraz ich wartości między
 * 			scenami w grze.
 */

using UnityEngine;
using System.Collections;

public static class StatsClass {

	//CZĘŚĆ ZE STATYSTYKAMI

	//Obecny poziom podziemi
	public static int dungeonLevel;
	//Najdalszy osiągnięty poziom podziemi
	public static int maxDungeonLevel;
	//Złoto zdobyte przez gracza
	public static uint gold;
	//Doświadczenie zdobyte przez gracza
	public static uint experience;
	//Maksymalna liczba punktów życia gracza
	public static float maxHealthPoints;
	//Prędkość ataku gracza
	public static float attackSpeed;
	//Dolny próg obrażeń zadawanych przez gracza
	public static float minAttackDamage;
	//Górny próg obrażeń zadawanych przez gracza
	public static float maxAttackDamage;
	//Procent wysysania życia podczas ataków gracza
	public static int lifeSteal;
	//Wartość redukcji obrażeń
	public static int damageReduction;
	//Procentowa wartość szansy na uniknięcie ataku
	public static int dodgeChance;


	//CZĘŚĆ WARTOŚCI WYNIKAJĄCYCH Z UMIEJĘTNOŚCI PASYWNYCH

	//Procentowa wartość bonusowego złota zdobywanego przez gracza
	public static float goldBonus;
	//Procentowa wartość bonusowego doświadczenia zdobywanego przez gracza
	public static float expBonus;

}
