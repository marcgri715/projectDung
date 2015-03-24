/* 
 * Imię i nazwisko autora: Marcin Griszbacher
 * Temat pracy: Gra RPG stworzona w oparciu o silnik Unity
 * 
 * Klasa LookForMonsters
 * Zadanie: skrypt, którego celem jest ukrycie przeciwnika, jeśli znajduje się za ścianą lub drzwiami bądź pokazaniem 
 * 			go, jeśli znajduje się w zasięgu wzroku naszej postaci.
 */

using UnityEngine;
using System.Collections;

public class LookForMonsters : MonoBehaviour {

	//Maski warstw
	private LayerMask obstacleLayerMask;
	private LayerMask clickableLayerMask;
	//Zasięg wyszukiwania wrogów
	public float visionRange;
	//Tablica określająca widoczność każdego z wrogów
	private bool[] visibility;
	//Liczba wrogów
	public int numberOfEnemies;


	//Funkcja zwracająca informację, czy dany przeciwnik jest widoczny
	public bool isVisible(int index) {
		return visibility [index];
	}



	void Start () {
		obstacleLayerMask = 1 << 10;
		clickableLayerMask = 1 << 8;
		visibility = new bool[numberOfEnemies];
	}
	

	void Update () {
		//Wykrycie wszystkich obiektów znajdujących się w zasięgu wyszukiwania
		Collider[] colliders = Physics.OverlapSphere (transform.position, visionRange * 1.1f);
		for (int i=0; i<colliders.Length; i++) {
			//Zwracamy uwagę tylko na wrogów
			if (colliders[i].tag=="Monster") {
				//Określenie wartości początkowych do rzucenia promienia między postacią gracza a celem
				Ray ray = new Ray(transform.position, colliders[i].transform.position-transform.position);
				float distance = Vector3.Distance(transform.position, colliders[i].transform.position);
				LayerMask mask = obstacleLayerMask | clickableLayerMask;
				RaycastHit hit;
				float visionRangeLeft = visionRange;
				while (true) {
					//Jeśli promień między postacią gracza a poszukiwanym wrogiem napotka na przeszkodę
					if (distance > 0 && Physics.Raycast(ray, out hit, distance, mask)) {
						//jeśli przeszkodą nie jest żaden potwór - cel jest niewidoczny
						if ((hit.collider.tag != "Monster" && hit.collider.tag !="DeadMonster")
						    || distance > visionRangeLeft) {
							Renderer[] rends = colliders[i].GetComponentsInChildren<Renderer>();
							for (int j=0; j<rends.Length; j++) {
								rends[j].enabled = false;
							}
							visibility[int.Parse(colliders[i].name)] = false;
							break;
						//jeśli przeszkodą jest potwór, ale nie ten, który jest celem - kontynuuj od tego punktu
						} else if ((hit.collider.tag == "Monster" || hit.collider.tag == "DeadMonster")
						           && hit.collider != colliders[i]) {
							ray = new Ray(hit.transform.position, 
							              colliders[i].transform.position - hit.transform.position);
							float toDeduct = Vector3.Distance(hit.transform.position, colliders[i].transform.position);
							distance -= toDeduct;
							visionRangeLeft -= toDeduct;
						//jeśli przeszkodą jest cel - należy go ujawnić
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
