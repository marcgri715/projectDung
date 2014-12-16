using UnityEngine;
using System.Collections;

public class PassiveSkillsClass {

	private string skillName;
	private int currentLevel;
	private float effect;

	public PassiveSkillsClass (string name, int level, float effect) {
		skillName = name;
		currentLevel = level;
		this.effect = effect;
	}

	public string getSkillName () {
		return skillName;
	}

	public int getCurrentLevel () {
		return currentLevel;
	}

	public void setCurrentLevel (int level) {
		currentLevel = level;
	}

	public float getSkillEffect () {
		return effect * currentLevel;
	}

	public uint getNextLevelUpgradeCost() {
		if (currentLevel<20)
			return (uint)Mathf.Pow (currentLevel, 2) * 100;
		else
			return 0;
	}

}