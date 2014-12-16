using UnityEngine;
using System.Collections;

public class MovePlayer : MonoBehaviour {
	private enum moveType {IDLE, RUN, ATTACK}

	private moveType currentAction;
	private Transform myTransform;				// this transform
	private Vector3 destinationPosition;		// The destination Point
	private float destinationDistance;			// The distance between myTransform and destinationPosition

	public float moveSpeed;						// The Speed the character will move
	private static LayerMask obstacleLayerMask = 1 << 10;
	private static LayerMask clickableLayerMask = 1 << 8;
	public float playerColliderDistance;
	RaycastHit hit;
	private bool clickedOnDoor = false;
	private bool attacking = false;
	private float attackCooldown = 0;


	// Use this for initialization
	void Start () {
		myTransform = transform;
		destinationPosition = myTransform.position;
		currentAction = moveType.IDLE;
	}
	
	// Update is called once per frame
	void Update () {

		// keep track of the distance between this gameObject and destinationPosition
		destinationDistance = Vector3.Distance(destinationPosition, myTransform.position);
		float speed=0;



		if(destinationDistance < .5f){		// To prevent shakin behavior when near destination
			speed = 0;
			if (currentAction == moveType.RUN) {
				if (attacking) {
					currentAction = moveType.ATTACK;
					animation.Play("attack");
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
		
		
		// Moves the Player if the Left Mouse Button was clicked
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
					attacking = true;
				} else {
					attacking = false;
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

		Collider[] colliders = Physics.OverlapSphere (myTransform.position, playerColliderDistance);

		for (int i=0; i<colliders.Length; i++) {
			if (colliders[i].tag == "Wall" || colliders[i].tag == "Door" || colliders[i].tag == "Monster") {
				if (Physics.Raycast(myTransform.position, Vector3.forward, out hit, playerColliderDistance, obstacleLayerMask))
					Debug.Log("forward");
				if (Physics.Raycast(myTransform.position, Vector3.back, out hit, playerColliderDistance, obstacleLayerMask))
					Debug.Log("back");
				if (Physics.Raycast(myTransform.position, Vector3.left, out hit, playerColliderDistance, obstacleLayerMask))
					Debug.Log("left");
				if (Physics.Raycast(myTransform.position, Vector3.right, out hit, playerColliderDistance, obstacleLayerMask))
					Debug.Log("right");
				//todo
				Ray ray = new Ray(myTransform.position, destinationPosition-myTransform.position);
				LayerMask mask = obstacleLayerMask | clickableLayerMask;
				if (Physics.Raycast(ray, out hit, destinationDistance*1.5f, mask)) {
					destinationPosition = myTransform.position;
					destinationDistance = 0;
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

		if (attacking && attackCooldown <= 0 ) {
			Ray ray = new Ray(myTransform.position, destinationPosition-myTransform.position);
			if (Physics.Raycast(ray, out hit, /*attack range*/1.5f, clickableLayerMask)) {
				if (hit.collider.tag == "Monster") {
					destinationPosition = myTransform.position;
					destinationDistance = 0;
					currentAction = moveType.ATTACK;
					animation["attack"].speed = 2.0f; //todo
					animation.Play("attack");
					//attackCooldown = animation["attack"].length * speed;
					attackCooldown = 0.767f; //animation
				}
			}
		}

		if (attackCooldown > 0)
			attackCooldown -= Time.deltaTime;

		// To prevent code from running if not needed
		if(destinationDistance > .5f){
			myTransform.position = Vector3.MoveTowards(myTransform.position, destinationPosition, speed * Time.deltaTime);
		}
	}
}
