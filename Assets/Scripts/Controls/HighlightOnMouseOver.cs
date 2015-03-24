/* 
 * Imię i nazwisko autora: Marcin Griszbacher
 * Temat pracy: Gra RPG stworzona w oparciu o silnik Unity
 * 
 * Klasa HighlightOnMouseOver
 * Zadanie: skrypt odpowiadający za podświetlenie gry po najechaniu na nie kursorem myszy.
 */

using UnityEngine;
using System.Collections;

public class HighlightOnMouseOver : MonoBehaviour {

	//Początkowy kolor drzwi
	private Color startingColor;
	//Parametr określający podświetlenie drzwi
	public float highlightMultiplier;

	void Start() {
		startingColor = renderer.material.color;
	}

	//Po najechaniu kursorem na drzwi - wymnażamy wartości poszczególnych kolorów przez parametr
	void OnMouseEnter() {
		Color newColor = renderer.material.color;
		newColor.r = startingColor.r * highlightMultiplier;
		newColor.g = startingColor.g * highlightMultiplier;
		newColor.b = startingColor.b * highlightMultiplier;
		renderer.material.color = newColor;
	}

	//Po zjechaniu kursorem z drzwi - przywrócenie poprzedniego koloru
	void OnMouseExit() {
		renderer.material.color = startingColor;
	}
}
