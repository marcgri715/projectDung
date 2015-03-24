/* 
 * Imię i nazwisko autora: Marcin Griszbacher
 * Temat pracy: Gra RPG stworzona w oparciu o silnik Unity
 * 
 * Klasa DefaultBehavior
 * Zadanie: skrypt opisujący zachowanie przeciwnika we wszystkich sytuacjach oraz przechowujący klasę ze statystykami
 *			 danego przeciwnika.
 */

using UnityEngine;
using System.Collections;

public class DefaultBehavior : MonoBehaviour {

	//Stany, w których może znajdować się przeciwnik
	public enum MonsterState { IDLE, MOVING, ATTACKING, DEAD, COMBAT_IDLE };

	//POLA PRYWATNE

	//transform obecnego przeciwnika
	private Transform myTransform;
	//punkt docelowy ruchu
	private Vector3 destinationPosition;
	//maski warstw
	private static LayerMask obstacleLayerMask;
	private static LayerMask playerLayerMask;
	private static LayerMask clickableLayerMask;
	//statystyki gracza
	private PlayerStatsClass playerStats;
	//licznik czasu
	private float timer;
	//numer identyfikacyjny przeciwnika
	private int idNumber;
	//obecny stan przeciwnika
	private MonsterState currentState;
	//flaga określająca, czy przeciwnik atakuje gracza
	private bool attacking;
	//skrypt zawierający informacje o widoczności przeciwników
	private LookForMonsters visibility;

	//POLA PUBLICZNE

	//statystyki stwora
	public MonsterClass myStats;
	//odległość wykrywania kolizji z przeszkodami i wrogami
	public float monsterColliderDistance;
	//nazwa animacji bezczynności
	public string idleAnimationString;
	//nazwa animacji biegu
	public string runAnimationString;
	//nazwa animacji ataku
	public string attackAnimationString;
	//nazwa animacji bezczynności podczas walki
	public string battleIdleAnimationString;
	//nazwa animacji śmierci
	public string deathAnimationString;

	// Use this for initialization
	void Start () {
		//ustawienie wartości początkowych
		obstacleLayerMask = 1 << 10;
		playerLayerMask = 1 << 9;
		clickableLayerMask = 1 << 8;
		timer = 0;
		attacking = false;
		myTransform = transform;
		destinationPosition = myTransform.position;
		//nazwa gameObjectu przeciwnika to kolejne numery od 0 do n-1 (n to liczba wrogów)
		idNumber = int.Parse (gameObject.name);
		currentState = MonsterState.IDLE;
		//dynamiczne wyszukanie obiektu tworzącego podziemia, by zdobyć statystyki przeciwnika o odpowiednim numerze ID
		GameObject newObject = GameObject.Find ("Level Spawner");
		DungeonCreator creator = (DungeonCreator) newObject.GetComponent(typeof(DungeonCreator));
		myStats = creator.getDungeon().getMonster(idNumber);
		//odnalezienie obiektu gracza i wczytanie jego statystyk
		newObject = GameObject.Find ("Player");
		MovePlayer player = (MovePlayer)newObject.GetComponent (typeof(MovePlayer));
		visibility = (LookForMonsters)newObject.GetComponent (typeof(LookForMonsters));
		playerStats = player.stats;
		//aktywowanie obiektu
		enabled = true;
		//wyłączenie rendererów - przeciwnik jest niewidoczny dla gracza
		Renderer[] renderers = GetComponentsInChildren<Renderer> ();
		foreach (Renderer renderer in renderers) {
			renderer.enabled = false;
		}
		//zapętlenie wyświetlanych animacji
		animation.wrapMode = WrapMode.Loop;
	}

	//Funkcja poruszająca przeciwnikiem znajdującym się w stanie bezczynności, gdy nie widzi postaci gracza
	void moveObject () {
		LayerMask mask = obstacleLayerMask | clickableLayerMask | playerLayerMask;
		while(true) {
			//wylosowanie nowej pozycji
			destinationPosition = new Vector3(myTransform.position.x + Random.Range(-0.5f, 0.5f),
			                                  myTransform.position.y,
			                                  myTransform.position.z + Random.Range(-0.5f, 0.5f));
			//wyjście z pętli dopiero, gdy nowa pozycja nie leży za przeszkodą
			Ray ray = new Ray(myTransform.position, destinationPosition-myTransform.position);
			if (!Physics.Raycast(ray, Vector3.Distance(myTransform.position, destinationPosition), mask)) {
				break;
			}
		}
		//obrócenie się w kierunku celu
		Quaternion targetRotation = Quaternion.LookRotation(destinationPosition - myTransform.position);
		myTransform.rotation = targetRotation;
	}

	//Funkcja poszukująca gracza i nakazująca poruszanie się w jego kierunku, by go zaatakować
	void lookForPlayer () {
		//przeszukiwanie obiektów w zasięgu wzroku
		Collider[] colliders = Physics.OverlapSphere (myTransform.position, myStats.getVisionRange());
		attacking = false;
		foreach (Collider collider in colliders) {
			//jeśli gracz znajduje się w zasięgu
			if (collider.tag == "Player") {
				//raycast w kierunku postaci gracza
				Vector3 rayLastPosition = myTransform.position;
				Ray ray = new Ray(rayLastPosition, collider.transform.position-rayLastPosition);
				LayerMask mask = obstacleLayerMask | clickableLayerMask;
				float distance = Vector3.Distance(myTransform.position, collider.transform.position);
				RaycastHit hit;
				while (true) {
					if (Physics.Raycast(ray, out hit, distance, mask)) {
						//jeśli na drodze znajduje się inny przeciwnik, ustal nowe wartości
						if (hit.collider.tag == "Monster" || hit.collider.tag == "DeadMonster") {
							distance -= Vector3.Distance(rayLastPosition, hit.transform.position);
							//jeśli odległość jest mniejsza od 0, przerwij poszukiwania
							if (distance <= 0) 
								break;
							rayLastPosition = hit.transform.position;
							ray = new Ray(rayLastPosition, collider.transform.position - rayLastPosition);
						} else {
							// jeśli na drodze znajduje się inna przeszkoda, przerwij
							break;
						}
					} else {
						//jeśli nie znaleziono przeszkód innych niż pozostali przeciwnicy, poruszaj się w stronę gracza
						//w celu zaatakowania go
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

	//Funkcja poruszająca postacią przeciwnika w danym kierunku po uprzednim sprawdzeniu kolizji
	void moveTowards() {
		LayerMask mask = obstacleLayerMask | clickableLayerMask;
		Ray ray = new Ray (myTransform.position, destinationPosition - myTransform.position);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, monsterColliderDistance, mask) && hit.collider.tag != "DeadMonster") {
			destinationPosition = myTransform.position;
			attacking = false;
		} else {
			myTransform.position = Vector3.MoveTowards(myTransform.position, 
			                                           destinationPosition, 
			                                           myStats.getMovementSpeed() * Time.deltaTime);
		}
	}

	
	void Update () {
		//jeśli stwór nie jest martwy
		if (currentState != MonsterState.DEAD) {
			//jeśli punkty życia spadły do zera lub poniżej, zabij wroga i dodaj doświadczenie oraz złoto postaci gracza
			if (myStats.getHealthPoints() <= 0) {
				currentState = MonsterState.DEAD;
				animation.wrapMode = WrapMode.Once;
				animation.Play(deathAnimationString);
				timer = animation[deathAnimationString].length * 1.0f;
				playerStats.addExperience(myStats.getExpPerKill()*(1.0f+StatsClass.expBonus));
				playerStats.addGold(myStats.getGoldPerKill()*(1.0f+StatsClass.goldBonus));
				//wyłączenie wykrywania kolizji między graczem a wrogiem
				gameObject.tag = "DeadMonster";
				return;
			}
			//poszukuj gracza, jeśli obiekt jest widoczny (jeśli nie jest, to nie ma szans zauważyć gracza)
			if (visibility.isVisible(idNumber)) {
				lookForPlayer ();
			}
			float distance = Vector3.Distance(myTransform.position, destinationPosition);
			//jeśli stwór się porusza
			if (currentState == MonsterState.MOVING) {
				//jeśli chce zaatakować
				if (attacking) {
					//jeśli jest w zasięgu ataku
					if (distance < myStats.getAttackRange()) {
						//jeśli nie upłynęło dość dużo czasu od ostatniego ataku, oczekuje
						if (timer > 0) {
							currentState = MonsterState.COMBAT_IDLE;
							animation.Play(battleIdleAnimationString);
						//jeśli zaś upłynęło, wchodzi w fazę ataku
						} else {
							destinationPosition = myTransform.position;
							currentState = MonsterState.ATTACKING;
							timer = animation[attackAnimationString].length;
							animation.Play(attackAnimationString);
						}
					//jeśli nie jest w zasięgu ataku - porusza się w kierunku gracza
					} else {
						moveTowards();
					}
				//jeśli nie atakuje - porusza się bądź stoi oczekując na zmianę
				} else if (distance > 0) {
						moveTowards();
				} else {
					currentState = MonsterState.IDLE;
					timer = Random.Range(1.0f, 3.0f);
					animation.Play(idleAnimationString);
				}
			//jeśli znajduje się w fazie ataku
			} else if (currentState == MonsterState.ATTACKING) {
				//po zakończeniu animacji - zadaje obrażenia postaci gracza i oczekuje na możliwość zaatakowania 
				//po raz kolejny
				if (timer <= 0) {
					playerStats.getHurt(myStats.getDamage());
					currentState = MonsterState.COMBAT_IDLE;
					timer = myStats.getAttackCooldown();
					animation.Play(battleIdleAnimationString);
				}
			//jeśli stoi bezczynnie podczas walki
			} else if (currentState == MonsterState.COMBAT_IDLE) {
				//jeśli skończył oczekiwanie
				if (timer <= 0) {
					if (attacking) {
						if (distance < myStats.getAttackRange()) {
							currentState = MonsterState.ATTACKING;
							timer = animation[attackAnimationString].length; //time of animation
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
			//jeśli stoi bezczynnie
			} else if (currentState == MonsterState.IDLE) {
				//jeśli stoi już dość długo
				if (timer <= 0) {
					//jeśli nie atakuje - porusza się w przypadkowym kierunku
					if (!attacking) {
						moveObject();
					}
					currentState = MonsterState.MOVING;
					animation.Play(runAnimationString);
				}
			}
		//jeśli stwór jest martwy
		} else {
			//w sekundę po zakończeniu animacji zapada się pod ziemię
			if (timer <= 0) {
				Vector3 newPosition = transform.position;
				newPosition.y -= transform.localScale.y * Time.deltaTime;
				transform.position = newPosition;
			}
			//sekundę później zostaje usunięty ze sceny
			if (timer <= -1) {
				Destroy(transform.gameObject);
			}
		}
		timer -= Time.deltaTime;
	}
}
