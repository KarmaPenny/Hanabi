using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckIndicator : MonoBehaviour {
	bool selected = false;
	public Text deckCount;
	public Image arrow;
	public Color lowDeckColor;

	TouchInput touchInput;

	void Start() {
		touchInput = GetComponent<TouchInput> ();
	}

	void Update() {
		if (touchInput.wasPressed) {
			selected = !selected;
		}

		Color targetColor = Color.white;
		if (Deck.remaining <= 5 && Deck.remaining > 0) {
			selected = true;
			targetColor = lowDeckColor;
		} else if (Deck.remaining == 0) {
			selected = false;
		}

		float alpha = (selected && Deck.turn >= 0) ? 1 : 0;
		GetComponent<CanvasGroup> ().alpha = Mathf.Lerp (GetComponent<CanvasGroup> ().alpha, alpha, 4 * Time.deltaTime);
		deckCount.text = Deck.remaining.ToString();

		deckCount.color = Color.Lerp (deckCount.color, targetColor, 4 * Time.deltaTime);
		arrow.color = Color.Lerp (arrow.color, targetColor, 4 * Time.deltaTime);
	}

	#if UNITY_STANDALONE
	void OnMouseEnter() {
		selected = true;
	}

	void OnMouseExit() {
		selected = false;
	}
	#endif
}
