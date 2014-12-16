using UnityEngine;
using System.Collections;

public class MonsterClass {

	private int startingRoom;
	private int type;
	private int level;
	private string name;
	private float healthPoints;
	private float maxHealthPoints;
	private float attackRange;
	private float damage;
	private float movementSpeed;
	private float experiencePerKill;
	private float goldPerKill;
	private float attackCooldown;
	private float visionRange;


	
	public MonsterClass(int pStartingRoom, int pType, int pLevel, string pName, float[] baseValues) {
		startingRoom = pStartingRoom;
		type = pType;
		level = pLevel;
		name = pName;
		maxHealthPoints = healthPoints = baseValues[0] * level;
		attackRange = baseValues[1];
		damage = baseValues[2] * level;
		movementSpeed = baseValues[3];
		experiencePerKill = baseValues[4] * level;
		goldPerKill = baseValues[5] * level;
		attackCooldown = baseValues[6];
		visionRange = baseValues[7];
	}

	public int getStartingRoom() {
		return startingRoom;
	}

	public void damageMonster(float damageDone) {
		healthPoints -= damageDone;
	}

	public string getName () {
		return name;
	}

	public float getMaxHealthPoints() {
		return maxHealthPoints;
	}

	public float getHealthPoints() {
		return healthPoints;
	}

	public float getAttackRange() {
		return attackRange;
	}

	public float getDamage() {
		return damage;
	}

	public float getMovementSpeed() {
		return movementSpeed;
	}

	public float getExpPerKill() {
		return experiencePerKill;
	}

	public float getGoldPerKill() {
		return goldPerKill;
	}

	public float getAttackCooldown() {
		return attackCooldown;
	}

	public float getVisionRange() {
		return visionRange;
	}

	public int getMonsterType() {
		return type;
	}
}
