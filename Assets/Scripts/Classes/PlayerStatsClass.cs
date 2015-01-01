using UnityEngine;
using System.Collections;

public class PlayerStatsClass {

	private uint gold;
	private uint experience;
	private float currentHealthPoints;
	private float maxHealthPoints;
	private float attackSpeed;
	private float minAttackDamage;
	private float maxAttackDamage;
	private int lifeSteal;
	private int damageReduction;
	private int dodgeChance;


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

	public void addGold (float value) {
		if (value >= 0) 
			gold += (uint)value;
		else
			gold -= (uint)-value;
	}

	public void addExperience (float value) {
		if (value >= 0) 
			experience += (uint)value;
		else
			experience -= (uint)-value;
	}

	public uint getGold () {
		return gold;
	}

	public uint getExperience () {
		return experience;
	}

	public void getHurt (float damage) {
		int isDodged = Random.Range (0, 100);
		if (isDodged >= dodgeChance) {
			float reductedDamage = damage * 100f / (100f + damageReduction);
			currentHealthPoints -= reductedDamage;
		}
	}

	public float getCurrentHP () {
		return currentHealthPoints;
	}

	public float getMaxHP () {
		return maxHealthPoints;
	}

	public float getAttackSpeed () {
		return attackSpeed;
	}

	public float getMinDamage () {
		return minAttackDamage;
	}

	public float getMaxDamage () {
		return maxAttackDamage;
	}

	public int getLifeSteal () {
		return lifeSteal;
	}

	public int getDamageReduction () {
		return damageReduction;
	}

	public int getDodgeChance () {
		return dodgeChance;
	}

	public void stealLife (float damageDone) {
		currentHealthPoints += damageDone * (lifeSteal / 100f);
	}
}
