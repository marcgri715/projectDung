using UnityEngine;
using System.Collections;

public class ShowHPOnMouseOver : MonoBehaviour {

	MonsterClass stats;
	LookForMonsters visibilityCheck;
	int monsterIndex;

	void Start() {
		GameObject newObject = gameObject;
		DefaultBehavior monster = (DefaultBehavior)newObject.GetComponent (typeof(DefaultBehavior));
		stats = monster.myStats;
		newObject = GameObject.Find ("Player");
		visibilityCheck = (LookForMonsters)newObject.GetComponent (typeof(LookForMonsters));
		monsterIndex = int.Parse (gameObject.name);
	}

	void OnMouseEnter() {
		if (visibilityCheck.isVisible(monsterIndex)) {
			HighlightedEnemyHealthClass.isOverEnemy = true;
			HighlightedEnemyHealthClass.percentOfHP = stats.getHealthPoints () / stats.getMaxHealthPoints ();
			HighlightedEnemyHealthClass.enemyName = stats.getName();
		}
	}

	void OnMouseOver() {
		if (visibilityCheck.isVisible(monsterIndex)) {
			HighlightedEnemyHealthClass.percentOfHP = stats.getHealthPoints () / stats.getMaxHealthPoints ();
		}
	}
	
	void OnMouseExit() {
		HighlightedEnemyHealthClass.isOverEnemy = false;
	}
}
