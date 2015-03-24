/* 
 * Imię i nazwisko autora: Marcin Griszbacher
 * Temat pracy: Gra RPG stworzona w oparciu o silnik Unity
 * 
 * Klasa MovePlayer
 * Zadanie: skrypt odpowiadający poruszanie się postaci oraz za wszelkie działania wykonywane przez postać.
 * 			Poruszanie się postaci, atakowanie wrogów, otwieranie drzwi oraz przechodzenie na kolejne poziomy 
 * 			realizowane jest za pomocą myszy.
 */

using UnityEngine;
using System.Collections;

public class MovePlayer : MonoBehaviour {
	//Rodzaj akcji, którą obecnie wykonuje postać gracza
	private enum moveType {IDLE, RUN, ATTACK, DEAD}

	//obecna akcja
	private moveType currentAction;
	//transform gracza
	private Transform myTransform;
	//punkt docelowy ruchu
	private Vector3 destinationPosition;
	//odległość do punktu docelowego
	private float destinationDistance;
	//Parametr opisujący prędkość poruszania się gracza
	public float moveSpeed;
	//Maski warstw
	private static LayerMask obstacleLayerMask;
	private static LayerMask clickableLayerMask;
	//Odległość wykrywania kolizji
	public float playerColliderDistance;
	//obiekt trafiony przez promień przy sprawdzaniu kolizji
	private RaycastHit hit;
	//Flaga mówiąca o tym, czy gracz kliknął na drzwi
	private bool clickedOnDoor;
	//Flaga atakowania przeciwnika
	private bool attacking;
	//Flaga kliknięcia na drabinę przenoszącą gracza na kolejny poziom
	private bool clickedOnLadder;
	//Czas pomiędzy kolejnymi atakami (równy długości trwania animacji ataku)
	private float attackCooldown;
	//Statystyki postaci gracza
	public PlayerStatsClass stats;
	//Przeciwnik, którego chce zaatakować gracz
	private GameObject attackedMonster;
	//Licznik czasu
	private float timer;
	//kliknięte przez gracza drzwi
	private string clickedDoorName;


	//Ustalenie wartości początkowych
	void Start () {
		obstacleLayerMask = 1 << 10;
		clickableLayerMask = 1 << 8;
		clickedOnDoor = false;
		attacking = false;
		clickedOnLadder = false;
		attackCooldown = 0;
		stats = new PlayerStatsClass ();
		myTransform = transform;
		destinationPosition = myTransform.position;
		currentAction = moveType.IDLE;
		animation.Play ("idle");
	}

	//Przypisanie nowych wartości złota i doświadczenia po zakończeniu gry do klasy statycznej
	void finishGame() {
		StatsClass.experience = stats.getExperience ();
		StatsClass.gold = stats.getGold ();
		//Zapisujemy stan gry - złoto, doświadczenie oraz najdalszy osiągnięty poziom
		PlayerPrefs.SetInt("gold", (int)stats.getGold());
		PlayerPrefs.SetInt("exp", (int)stats.getExperience());
		PlayerPrefs.SetInt("maxLevel", StatsClass.maxDungeonLevel);
		PlayerPrefs.Save();
	}
	

	void Update () {
		//Jeśli gracz jest martwy - oczekiwanie na zakończenie gry (5 sekund)
		if (currentAction == moveType.DEAD) {
			timer -= Time.deltaTime;
			if (timer < 0) {
				finishGame();
				//Ładujemy ekran ulepszeń
				Application.LoadLevel(2);
			}
		} else {
			//jeśli punkty życia spadną do 0 lub poniżej tej wartości
			if (stats.getCurrentHP() <= 0) {
				currentAction = moveType.DEAD;
				timer = 5.0f;
				//poniższa linijka jest zakomentowana, ponieważ model postaci gracza nie posiada animacji śmierci
				//animation.Play ("death");
			} else {
				//jeśli gracz nie jest w trakcie ataku
				if (currentAction != moveType.ATTACK) {
					//Wykrycie kliknięcia lewym przyciskiem myszy
					if (Input.GetMouseButtonDown(0) && GUIUtility.hotControl == 0) {
						//Wykrycie miejsca gdzie kliknęła mysz - stworzenie promienia (półprostej) od kamery 
						//w kierunku kliknięcia
						Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
						if (Physics.Raycast(ray, out hit, Mathf.Infinity, clickableLayerMask)) {
							//poruszanie się w kierunku klikniętym przez gracza
							Vector3 targetPoint = hit.point;
							targetPoint.y = 0.5f;
							Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
							myTransform.rotation = targetRotation;
							destinationPosition = targetPoint;
							//jeśli kliknięto na drzwi - przestawienie flagi
							if (hit.collider.tag == "Door") {
								clickedOnDoor = true;
								clickedDoorName = hit.collider.gameObject.name;
							} else {
								clickedOnDoor = false;
							}
							//jeśli kliknięto na wroga - przestawienie flagi oraz zachowanie obiektu wroga, 
							//którego kliknięto
							if (hit.collider.tag == "Monster") {
								DefaultBehavior enemy = (DefaultBehavior)hit.transform.gameObject.GetComponent
																						(typeof(DefaultBehavior));
								//wykonywane tylko gdy przeciwnik jest żywy
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
							//Jeśli kliknięto na drabinę - przestawienie flagi
							if (hit.collider.tag == "ExitLadder") {
								clickedOnLadder = true;
							} else {
								clickedOnLadder = false;
							}
						}
					}
					//Wykrycie przytrzymanego klawisza myszy - okrojona o rozpoznanie klikniętego celu
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
				//obliczenie odległości między postacią gracza a celem
				destinationDistance = Vector3.Distance(destinationPosition, myTransform.position);
				float speed=0;

				//jeśli postać gracza jest w trakcie ataku
				if (currentAction == moveType.ATTACK) {
					//po zakończeniu animacji
					if (attackCooldown <= 0) {
						//Zadaj obrażenia oraz wylecz się o wartość wyssanego życia
						DefaultBehavior enemy = (DefaultBehavior)attackedMonster.GetComponent(typeof(DefaultBehavior));
						float damageDone = Random.Range(stats.getMinDamage(), stats.getMaxDamage());
						enemy.myStats.damageMonster(damageDone);
						stats.stealLife(damageDone);
						//przejdź w stan bezczynności
						currentAction = moveType.IDLE;
						//jeśli przeciwnik jest martwy, wyzeruj pole z klikniętym wrogiem
						if (enemy.myStats.getHealthPoints() < 0)
							attackedMonster = null;
					} else {
						attackCooldown -= Time.deltaTime;
					}
				}

				//jeśli postać doszła do celu
				if(destinationDistance < 0.5f){
					speed = 0;
					//jeśli poruszała się
					if (currentAction == moveType.RUN) {
						//jeśli zamierza zaatakować wroga - rozpoczyna atak
						if (attacking && attackCooldown <= 0) {
							currentAction = moveType.ATTACK;
							//przyspieszenie lub spowolnienie animacji ataku w zależności od prędkości ataku
							animation["attack"].speed = 1.0f * stats.getAttackSpeed();
							animation.Play("attack");
							attackCooldown = animation["attack"].length;
						//jeśli nie - staje w miejscu
						} else {
							currentAction = moveType.IDLE;
							animation.Play("idle");
						}
					}
				}
				//jeśli nie dotarła do celu - niech porusza się dalej
				else if(destinationDistance > 0.5f){
					speed = moveSpeed;
					if (currentAction != moveType.RUN) {
						currentAction = moveType.RUN;
						animation.Play("run");
					}
				}

				//wykrywanie kolizji między graczem a dowolnymi przeszkodami
				Collider[] colliders = Physics.OverlapSphere (myTransform.position, playerColliderDistance);

				for (int i=0; i<colliders.Length; i++) {
					//jeśli przeszkodą jest ściana, drzwi bądź żywy przeciwnik
					if (colliders[i].tag == "Wall" ||
					    colliders[i].tag == "Door" ||
					    colliders[i].tag == "Monster") {
						//jeśli przeszkodą są kliknięte przez gracza drzwi
						if (colliders[i].gameObject.name == clickedDoorName && clickedOnDoor) {
							//zatrzymujemy postać oraz otwieramy drzwi
							destinationPosition = myTransform.position;
							destinationDistance = 0;
							clickedOnDoor = false;
							GameObject door = colliders[i].gameObject;
							OpenDoor script = (OpenDoor)door.GetComponent(typeof(OpenDoor));
							script.openDoor();
						} else {
							Ray ray = new Ray(myTransform.position, destinationPosition-myTransform.position);
							LayerMask mask = obstacleLayerMask | clickableLayerMask;
							//Sprawdzenie, czy postać porusza się w kierunku przeszkody - jeśli tak, nie pozwalamy na ruch
							if (Physics.Raycast(ray, out hit, playerColliderDistance*1.5f, mask)) {
								destinationPosition = myTransform.position;
								destinationDistance = 0;
								//Zablokowanie ataku
								if (colliders[i].gameObject != attackedMonster) {
									attacking = false;
								}
							}
						}
					}
				}

				//Jeśli kliknięto na drabinę i jesteśmy wystarczająco blisko by wyjść na niższy poziom
				if (clickedOnLadder && destinationDistance < 0.5f) {
					//Zapisujemy wszystkie zmiany
					StatsClass.dungeonLevel++;
					if (StatsClass.maxDungeonLevel < StatsClass.dungeonLevel)
						StatsClass.maxDungeonLevel = StatsClass.dungeonLevel;
					StatsClass.gold = stats.getGold();
					StatsClass.experience = stats.getExperience();
					//Ładujemy poziom nowy poziom podziemi
					Application.LoadLevel(1);
				}

				//Jeśli cel nie znajduje się w pobliżu gracza, poruszamy się w jego kierunku
				if(destinationDistance > .5f){
					myTransform.position = Vector3.MoveTowards(myTransform.position, destinationPosition, speed * Time.deltaTime);
				}
			}
		}
	}
}
