using UnityEngine;
using System.Collections;

public class LookForMonsters : MonoBehaviour {

	private LayerMask obstacleLayerMask = 1 << 10;
	private LayerMask clickableLayerMask = 1 << 8;
	//private LayerMask enemyLayerMask = 1 << 11; 
	public float visionRange;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Collider[] colliders = Physics.OverlapSphere (transform.position, visionRange * 1.1f);
		for (int i=0; i<colliders.Length; i++) {
			if (colliders[i].tag=="Monster") {
				Ray ray = new Ray(transform.position, colliders[i].transform.position-transform.position);
				float distance = Vector3.Distance(transform.position, colliders[i].transform.position);
				LayerMask mask = obstacleLayerMask | clickableLayerMask; // | enemyLayerMask;
				RaycastHit hit;
				float visionRangeLeft = visionRange;
				while (true) {
					if (Physics.Raycast(ray, out hit, distance, mask)) {
						if (hit.collider.tag != "Monster" || distance > visionRangeLeft) {
							Renderer[] rends = colliders[i].GetComponentsInChildren<Renderer>();
							for (int j=0; j<rends.Length; j++) {
								rends[j].enabled = false;
							}
							break;
						} else if (hit.collider.tag == "Monster" && hit.collider != colliders[i]) {
							ray = new Ray(hit.transform.position, 
							              colliders[i].transform.position - hit.transform.position);
							float toDeduct = Vector3.Distance(hit.transform.position, colliders[i].transform.position);
							distance -= toDeduct;
							visionRangeLeft -= toDeduct;
						} else {
							Renderer[] rends = colliders[i].GetComponentsInChildren<Renderer>();
							for (int j=0; j<rends.Length; j++) {
								rends[j].enabled = true;
							}
							break;
						}
					} else break;
				}
			}
		}
	}
}
