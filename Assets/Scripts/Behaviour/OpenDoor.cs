using UnityEngine;
using System.Collections;

public class OpenDoor : MonoBehaviour {

	private float timer = 0;
	public float timeToOpen;
	private bool isOpening = false;

	public void openDoor() {
		isOpening = true;
	}

	void Update() {
		if (isOpening) {
			Vector3 newPosition = transform.position;
			newPosition.y -= transform.localScale.y / timeToOpen * Time.deltaTime;
			transform.position = newPosition;
			timer += Time.deltaTime;
			if (timer>timeToOpen) {
				Destroy(transform.gameObject);
			}
		}
	}
}
