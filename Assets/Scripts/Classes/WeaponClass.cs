using UnityEngine;
using System.Collections;

public class WeaponClass {

	private string weaponName;
	private int currentLevel;
	private float minDamage; //base values
	private float maxDamage;
	private float attackSpeed;

	public WeaponClass (string name, int level, int minDmg, int maxDmg, float AS) {
		weaponName = name;
		currentLevel = level;
		minDamage = minDmg;
		maxDamage = maxDmg;
		attackSpeed = AS;
	}

	public string getWeaponName() {
		return weaponName;
	}

	public int getCurrentLevel() {
		return currentLevel;
	}

	public void setCurrentLevel(int level) {
		currentLevel = level;
	}

	public float getCurrentMinDamage() {
		return minDamage + minDamage*(currentLevel-1)*1.25f; //do zmiany - wolna broń ma lepsze staty
	}

	public float getCurrentMaxDamage() {
		return maxDamage + maxDamage*(currentLevel-1)*1.25f;
	}

	public float getPredictedMinDamage() {
		return minDamage + minDamage*(currentLevel)*1.25f;
	}

	public float getPredictedMaxDamage() {
		return maxDamage + maxDamage*(currentLevel)*1.25f;
	}

	public float getAttackSpeed() {
		return attackSpeed;
	}

	public uint getNextLevelUpgradeCost() {
		if (currentLevel < 20) //max level
			return (uint)Mathf.Pow (currentLevel, 2) * 100;
		else
			return 0;
	}
}
