/* 
 * Imię i nazwisko autora: Marcin Griszbacher
 * Temat pracy: Gra RPG stworzona w oparciu o silnik Unity
 * 
 * Klasa OpenDoor
 * Zadanie: skrypt odpowiadający za otwieranie drzwi oraz usuwanie ich z pola gry.
 */

using UnityEngine;
using System.Collections;

public class OpenDoor : MonoBehaviour {

	//licznik czasu
	private float timer;
	//czas, po jakim drzwi zostaną otworzone (w sek.)
	public float timeToOpen;
	//flaga określająca, czy drzwi się otwierają
	private bool isOpening;


	//Funkcja, która zmienia flagę otwierania drzwi
	public void openDoor() {
		isOpening = true;
	}

	void Start() {
		isOpening = false;
		timer = 0;
	}

	void Update() {
		//jeśli drzwi się otwierają, są przesuwane w kierunku podłogi
		if (isOpening) {
			Vector3 newPosition = transform.position;
			newPosition.y -= transform.localScale.y / timeToOpen * Time.deltaTime;
			transform.position = newPosition;
			timer += Time.deltaTime;
			//po upłynięciu określonego czasu drzwi usuwane są ze sceny
			if (timer>timeToOpen) {
				Destroy(transform.gameObject);
			}
		}
	}
}
