/* 
 * Imię i nazwisko autora: Marcin Griszbacher
 * Temat pracy: Gra RPG stworzona w oparciu o silnik Unity
 * 
 * Klasa LookAtPlayer
 * Zadanie: skrypt odpowiadający za poruszanie się kamery. Umożliwia on sterowanie kamerą za pomocą klawiszy strzałek
 * 			na klawiaturze (oblicza pozycję kamery za pomocą funkcji sinus i cosinus).
 */

using UnityEngine;
using System.Collections;

public class LookAtPlayer : MonoBehaviour {

	//Punkt, w którym znajduje się postać gracza
	private Vector3 point;
	//Parametr, którym jest obiekt postaci gracza
	public Transform target;
	//Kąt na okręgu określający pozycję kamery względem gracza
	private float angle;
	//Wartość współrzędnej x kamery
	private float xValue;
	//Wartość współrzędnej z kamery
	private float zValue;
	//Parametr określający prędkość obracania się kamery
	public float rotateSpeed;
	//Parametr określający średnicę okręgu, po którym porusza się kamera
	public float cameraDistance;
	//Parametr, który określa o ile wyżej niż gracz znajduje się kamera
	public float cameraHeight;
	

	void Start () {
		//Ustalenie wartości początkowych
		angle = 45;
		point = target.transform.position;
		transform.LookAt (point);
		xValue = cameraDistance * Mathf.Cos (angle);
		zValue = cameraDistance * Mathf.Sin (angle);
	}


	void Update () {
		//Jeśli wciśnęty jest klawisz strzałki w lewo 
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			//zmniejszenie kąta i w razie potrzeby przekręcenie się licznika
			angle -= rotateSpeed * Time.deltaTime;
			if (angle < 0) angle += 360;
			//Obliczenie współrzędnych za pomocą funkcji cosinus i sinus
			xValue = cameraDistance * Mathf.Cos (angle);
			zValue = cameraDistance * Mathf.Sin (angle);
		}
		//Jeśli wciśnięty jest klawisz strzałki w prawo
		if (Input.GetKey(KeyCode.RightArrow))
		{
			angle += rotateSpeed * Time.deltaTime;
			if (angle > 360) angle -= 360;
			xValue = cameraDistance * Mathf.Cos (angle);
			zValue = cameraDistance * Mathf.Sin (angle);
		}
		//Ustawienie kamery w określonej pozycji względem postaci gracza
		point.x += xValue;
		point.y += cameraHeight;
		point.z += zValue;
		transform.position = point;
		//Obrócenie kamery w kierunku postaci gracza
		point = target.transform.position;
		transform.LookAt (point);
	}
}
