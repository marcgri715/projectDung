using UnityEngine;
using System.Collections;

public class LookForMonsters : MonoBehaviour {

	private LayerMask obstacleLayerMask = 1 << 10;
	private LayerMask clickableLayerMask = 1 << 8;
	public float visionRange;
	private bool[] visibility;
	public int numberOfEnemies;

	public bool isVisible(int index) {
		return visibility [index];
	}


	// Use this for initialization
	void Start () {
		visibility = new bool[numberOfEnemies];
	}
	
	// Update is called once per frame
	void Update () {
		Collider[] colliders = Physics.OverlapSphere (transform.position, visionRange * 1.1f);
		for (int i=0; i<colliders.Length; i++) {
			if (colliders[i].tag=="Monster") {
				Ray ray = new Ray(transform.position, colliders[i].transform.position-transform.position);
				float distance = Vector3.Distance(transform.position, colliders[i].transform.position);
				LayerMask mask = obstacleLayerMask | clickableLayerMask;
				RaycastHit hit;
				float visionRangeLeft = visionRange;
				while (true) {
					if (distance > 0 && Physics.Raycast(ray, out hit, distance, mask)) {
						if (hit.collider.tag != "Monster" || distance > visionRangeLeft) {
							Renderer[] rends = colliders[i].GetComponentsInChildren<Renderer>();
							for (int j=0; j<rends.Length; j++) {
								rends[j].enabled = false;
							}
							//if (colliders[i].tag == "Monster")
								visibility[int.Parse(colliders[i].name)] = false;
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
							visibility[int.Parse(hit.transform.gameObject.name)] = true;
							break;
						}
					} else break;
				}
			}
		}
	}
}
