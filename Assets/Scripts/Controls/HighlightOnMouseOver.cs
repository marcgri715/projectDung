using UnityEngine;
using System.Collections;

public class HighlightOnMouseOver : MonoBehaviour {

	private Color startingColor;
	public float highlightMultiplier;

	void Start() {
		startingColor = renderer.material.color;
	}

	void OnMouseEnter() {
		Color newColor = renderer.material.color;
		newColor.r = startingColor.r * highlightMultiplier;
		newColor.g = startingColor.g * highlightMultiplier;
		newColor.b = startingColor.b * highlightMultiplier;
		renderer.material.color = newColor;
	}

	void OnMouseExit() {
		renderer.material.color = startingColor;
	}
}
