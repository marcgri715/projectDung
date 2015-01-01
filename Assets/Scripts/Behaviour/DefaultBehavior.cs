/* imię nazwisko nazwa pracy */

using UnityEngine;
using System.Collections;

public class DefaultBehavior : MonoBehaviour {

	public enum MonsterState { IDLE, MOVING, ATTACKING, DEAD, COMBAT_IDLE };

	private Transform myTransform;				// this transform
	private Vector3 destinationPosition;		// The destination Point
	//private float destinationDistance;			// The distance between myTransform and destinationPosition

	private static LayerMask obstacleLayerMask = 1 << 10;
	private static LayerMask playerLayerMask = 1 << 9;
	private static LayerMask clickableLayerMask = 1 << 8;
	public float monsterColliderDistance;
	private float timer = 0;
	private int idNumber;
	private MonsterState currentState;
	public MonsterClass myStats;
	private bool attacking = false;
	public string idleAnimationString;
	public string runAnimationString;
	public string attackAnimationString;
	public float attackAnimationLength;
	public string battleIdleAnimationString;
	public string deathAnimationString;
	private PlayerStatsClass playerStats;

	// Use this for initialization
	void Start () {
		myTransform = transform;
		destinationPosition = myTransform.position;
		idNumber = int.Parse (gameObject.name);
		currentState = MonsterState.IDLE;
		GameObject newObject = GameObject.Find ("Level Spawner");
		DungeonCreator creator = (DungeonCreator) newObject.GetComponent(typeof(DungeonCreator));
		myStats = creator.getDungeon().getMonster(idNumber);
		newObject = GameObject.Find ("Player");
		MovePlayer player = (MovePlayer)newObject.GetComponent (typeof(MovePlayer));
		playerStats = player.stats;
		enabled = true;
		currentState = MonsterState.IDLE;
		Renderer[] renderers = GetComponentsInChildren<Renderer> ();
		foreach (Renderer renderer in renderers) {
			renderer.enabled = false;
		}
		animation.wrapMode = WrapMode.Loop;
	}

	void moveObject () {
		LayerMask mask = obstacleLayerMask | clickableLayerMask | playerLayerMask;
		while(true) {
			destinationPosition = new Vector3(myTransform.position.x + Random.Range(-0.5f, 0.5f),
			                                  myTransform.position.y,
			                                  myTransform.position.z + Random.Range(-0.5f, 0.5f));
			Ray ray = new Ray(myTransform.position, destinationPosition-myTransform.position);
			//Vector3.Distance(destinationPosition, myTransform.position)
			if (!Physics.Raycast(ray, monsterColliderDistance, mask)) {
				break;
			}
		}
		Quaternion targetRotation = Quaternion.LookRotation(destinationPosition - myTransform.position);
		myTransform.rotation = targetRotation;
	}

	void lookForPlayer () {
		Collider[] colliders = Physics.OverlapSphere (myTransform.position, myStats.getVisionRange());
		attacking = false;
		foreach (Collider collider in colliders) {
			if (collider.tag == "Player") {
				Vector3 rayLastPosition = myTransform.position;
				Ray ray = new Ray(rayLastPosition, collider.transform.position-rayLastPosition);
				LayerMask mask = obstacleLayerMask | clickableLayerMask;
				float distance = Vector3.Distance(myTransform.position, collider.transform.position);
				RaycastHit hit;
				while (true) {
					if (Physics.Raycast(ray, out hit, distance, mask)) {
						if (hit.collider.tag == "Monster") {
							distance -= Vector3.Distance(rayLastPosition, hit.transform.position);
							if (distance <= 0) 
								break;
							rayLastPosition = hit.transform.position;
							ray = new Ray(rayLastPosition, collider.transform.position - rayLastPosition);
						} else {
							break;
						}
					} else {
						destinationPosition = collider.transform.position;
						Quaternion targetRotation = Quaternion.LookRotation(destinationPosition - myTransform.position);
						myTransform.rotation = targetRotation;
						attacking = true;
						break;
					}
				}
				break;
			}
		}
	}

	void moveTowards() {
		LayerMask mask = obstacleLayerMask | clickableLayerMask;
		Ray ray = new Ray (myTransform.position, destinationPosition - myTransform.position);
		if (Physics.Raycast(ray, monsterColliderDistance, mask)) {
			destinationPosition = myTransform.position;
			attacking = false;
		} else {
			myTransform.position = Vector3.MoveTowards(myTransform.position, 
			                                           destinationPosition, 
			                                           myStats.getMovementSpeed() * Time.deltaTime);
		}
	}


	// Update is called once per frame
	void Update () {
		if (currentState != MonsterState.DEAD) {
			if (myStats.getHealthPoints() <= 0) {
				currentState = MonsterState.DEAD;
				animation.wrapMode = WrapMode.Once;
				animation.Play(deathAnimationString);
				timer = animation[deathAnimationString].length * 1.0f;
				playerStats.addExperience(myStats.getExpPerKill()*(1.0f+StatsClass.expBonus));
				playerStats.addGold(myStats.getGoldPerKill()*(1.0f+StatsClass.goldBonus));
				gameObject.tag = "DeadMonster";
			}
			lookForPlayer ();
			float distance = Vector3.Distance(myTransform.position, destinationPosition);
			if (currentState == MonsterState.MOVING) {
				if (attacking) {
					if (distance < myStats.getAttackRange()) {
						if (timer > 0) {
							currentState = MonsterState.COMBAT_IDLE;
							animation.Play(battleIdleAnimationString);
						} else {
							destinationPosition = myTransform.position;
							//destinationDistance = 0;
							currentState = MonsterState.ATTACKING;
							timer = attackAnimationLength;
							animation.Play(attackAnimationString);
						}
					} else {
							moveTowards();
					}
				} else if (distance > 0) {
						moveTowards();
				} else {
					currentState = MonsterState.IDLE;
					timer = Random.Range(1.0f, 3.0f);
					animation.Play(idleAnimationString);
				}
			} else if (currentState == MonsterState.ATTACKING) {
				if (timer <= 0) {
					playerStats.getHurt(myStats.getDamage());
					currentState = MonsterState.COMBAT_IDLE;
					timer = myStats.getAttackCooldown();
					animation.Play(battleIdleAnimationString);
				}
			} else if (currentState == MonsterState.COMBAT_IDLE) {
				if (timer <= 0) {
					if (attacking) {
						if (distance < myStats.getAttackRange()) {
							currentState = MonsterState.ATTACKING;
							timer = attackAnimationLength; //time of animation
							animation.Play(attackAnimationString);
						} else {
							currentState = MonsterState.MOVING;
							animation.Play(runAnimationString);
						}
					} else {
						currentState = MonsterState.IDLE;
						timer = Random.Range(1.0f, 3.0f);
						animation.Play(idleAnimationString);
					}
				}
			} else if (currentState == MonsterState.IDLE) {
				if (timer <=0 ) {
					if (!attacking) {
						moveObject();
					}
					currentState = MonsterState.MOVING;
					animation.Play(runAnimationString);
				}
			}

		} else {
			if (timer <= 0) {
				Vector3 newPosition = transform.position;
				newPosition.y -= transform.localScale.y * Time.deltaTime;
				transform.position = newPosition;
			}
			if (timer <= -1) {
				Destroy(transform.gameObject);
			}
		}
		timer -= Time.deltaTime;
	}
}
