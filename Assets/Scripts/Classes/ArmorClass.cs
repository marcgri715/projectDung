using UnityEngine;
using System.Collections;

public class ArmorClass {
	
	private string armorName;
	private int currentLevel;
	private float damageReduction;
	private float dodgeChance;
	
	public ArmorClass (string name, int level, int damageRed, int dodge) {
		armorName = name;
		currentLevel = level;
		damageReduction = damageRed;
		dodgeChance = dodge;
	}
	
	public string getArmorName() {
		return armorName;
	}
	
	public int getCurrentLevel() {
		return currentLevel;
	}
	
	public void setCurrentLevel(int level) {
		currentLevel = level;
	}
	
	public float getDamageReduction () {
		return damageReduction + damageReduction * (currentLevel - 1) * 0.5f;
	}

	public float getDodgeChance () {
		return dodgeChance + dodgeChance * (currentLevel - 1) * 0.2f;
	}

	public float getPredictedDamageReduction () {
		return damageReduction + damageReduction * currentLevel * 0.5f;
	}

	public float getPredictedDodgeChance () {
		return dodgeChance + dodgeChance * currentLevel * 0.2f;
	}
	
	public uint getNextLevelUpgradeCost() {
		if (currentLevel < 20) //max level
			return (uint)Mathf.Pow (currentLevel, 2) * 100;
		else
			return 0;
	}
}
