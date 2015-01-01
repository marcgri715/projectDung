using UnityEngine;
using System.Collections;

public class MovePlayer : MonoBehaviour {
	private enum moveType {IDLE, RUN, ATTACK, DEAD, COMBAT_IDLE}

	private moveType currentAction;
	private Transform myTransform;				// this transform
	private Vector3 destinationPosition;		// The destination Point
	private float destinationDistance;			// The distance between myTransform and destinationPosition

	public float moveSpeed;						// The Speed the character will move
	private static LayerMask obstacleLayerMask = 1 << 10;
	private static LayerMask clickableLayerMask = 1 << 8;
	public float playerColliderDistance;
	private RaycastHit hit;
	private bool clickedOnDoor = false;
	private bool attacking = false;
	private bool clickedOnLadder = false;
	private float attackCooldown = 0;
	public PlayerStatsClass stats;
	private GameObject attackedMonster;
	private float timer;

	// Use this for initialization
	void Start () {
		stats = new PlayerStatsClass ();
		myTransform = transform;
		destinationPosition = myTransform.position;
		currentAction = moveType.IDLE;
	}

	void finishGame() {
		StatsClass.experience = stats.getExperience ();
		StatsClass.gold = stats.getGold ();
	}
	
	// Update is called once per frame
	void Update () {
		if (currentAction == moveType.DEAD) {
			timer -= Time.deltaTime;
			if (timer < 0) {
				finishGame();
				Application.LoadLevel(2);
			}
		} else {
			if (stats.getCurrentHP() < 0) {
				currentAction = moveType.DEAD;
				timer = 5.0f;
				//animation.Play ("death");
			} else {
				// Moves the Player if the Left Mouse Button was clicked
				if (currentAction != moveType.ATTACK) {
					if (Input.GetMouseButtonDown(0) && GUIUtility.hotControl == 0) {
						Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
						if (Physics.Raycast(ray, out hit, Mathf.Infinity, clickableLayerMask)) {
							Vector3 targetPoint = hit.point;
							targetPoint.y = 0.5f;
							Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
							myTransform.rotation = targetRotation;
							destinationPosition = targetPoint;
							if (hit.collider.tag == "Door") {
								clickedOnDoor = true;
							} else {
								clickedOnDoor = false;
							}
							if (hit.collider.tag == "Monster") {
								DefaultBehavior enemy = (DefaultBehavior)hit.transform.gameObject.GetComponent(typeof(DefaultBehavior));
								if (enemy.myStats.getHealthPoints() > 0) {
									attacking = true;
									attackedMonster = hit.transform.gameObject;
								} else {
									attacking = false;
									attackedMonster = null;
								}
							} else {
								attacking = false;
								attackedMonster = null;
							}
							if (hit.collider.tag == "ExitLadder") {
								clickedOnLadder = true;
							} else {
								clickedOnLadder = false;
							}
						}
					}
					// Moves the player if the mouse button is hold down
					else if (!attacking && Input.GetMouseButton(0) && GUIUtility.hotControl == 0) {
						Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
						if (Physics.Raycast(ray, out hit, Mathf.Infinity, clickableLayerMask)) {
							Vector3 targetPoint = hit.point;
							targetPoint.y = 0.5f;
							Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
							myTransform.rotation = targetRotation;
							destinationPosition = targetPoint;
						}
					}
				}

				destinationDistance = Vector3.Distance(destinationPosition, myTransform.position);
				float speed=0;

				if (currentAction == moveType.ATTACK) {
					if (attackCooldown <= 0) {
						DefaultBehavior enemy = (DefaultBehavior)attackedMonster.GetComponent(typeof(DefaultBehavior));
						float damageDone = Random.Range(stats.getMinDamage(), stats.getMaxDamage());
						enemy.myStats.damageMonster(damageDone);
						stats.stealLife(damageDone);
						currentAction = moveType.IDLE;
						if (enemy.myStats.getHealthPoints() < 0)
							attackedMonster = null;
					} else {
						attackCooldown -= Time.deltaTime;
					}
				}

				if(destinationDistance < .5f){
					speed = 0;
					if (currentAction == moveType.RUN) {
						if (attacking && attackCooldown <= 0) {
							currentAction = moveType.ATTACK;
							animation["attack"].speed = 1.0f * (1f/stats.getAttackSpeed()); //todo
							animation.Play("attack");
							attackCooldown = animation["attack"].length; //fix

						} else {
							currentAction = moveType.IDLE;
							animation.Play("idle");
						}
					}
				}
				else if(destinationDistance > .5f){			// To Reset Speed to default
					speed = moveSpeed;
					if (currentAction != moveType.RUN) {
						currentAction = moveType.RUN;
						animation.Play("run");
					}
				}

				Collider[] colliders = Physics.OverlapSphere (myTransform.position, playerColliderDistance);

				for (int i=0; i<colliders.Length; i++) {
					if (colliders[i].tag == "Wall" || colliders[i].tag == "Door" || colliders[i].tag == "Monster") {
						Ray ray = new Ray(myTransform.position, destinationPosition-myTransform.position);
						LayerMask mask = obstacleLayerMask | clickableLayerMask;
						if (Physics.Raycast(ray, out hit, destinationDistance*1.5f, mask)) {
							destinationPosition = myTransform.position;
							destinationDistance = 0;
							if (colliders[i].gameObject != attackedMonster) {
								attacking = false;
							}
						}
					}
				}

				if (clickedOnDoor) {
					Ray ray = new Ray(myTransform.position, destinationPosition-myTransform.position);
					if (Physics.Raycast(ray, out hit, playerColliderDistance*2.0f, clickableLayerMask)) {
						if (hit.collider.tag == "Door") {
							destinationPosition = myTransform.position;
							destinationDistance = 0;
							clickedOnDoor = false;
							GameObject door = hit.transform.gameObject;
							OpenDoor script = (OpenDoor)door.GetComponent(typeof(OpenDoor));
							script.openDoor();
						}
					}
				}

				if (clickedOnLadder && destinationDistance < 0.5f) {
					StatsClass.dungeonLevel++;
					if (StatsClass.maxDungeonLevel < StatsClass.dungeonLevel)
						StatsClass.maxDungeonLevel = StatsClass.dungeonLevel;
					StatsClass.gold = stats.getGold();
					StatsClass.experience = stats.getExperience();
					Application.LoadLevel(1);
				}

				// To prevent code from running if not needed
				if(destinationDistance > .5f){
					myTransform.position = Vector3.MoveTowards(myTransform.position, destinationPosition, speed * Time.deltaTime);
				}
			}
		}
	}
}
