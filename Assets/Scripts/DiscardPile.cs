using UnityEngine;

public class DiscardPile : CardPile {
	public DiscardPile sortPile;

	bool selected;
	TouchInput touchInput;

	protected override void Start() {
		base.Start ();
		touchInput = GetComponent<TouchInput> ();
	}

	void Update() {
		if ((selected && Input.GetButtonDown ("Fire1")) || touchInput.wasPressed || touchInput.wasLongPressed) {
			Cycle ();
		}
	}

	#if UNITY_STANDALONE
	void OnMouseEnter() {
		selected = true;
	}

	void OnMouseExit() {
		selected = false;
	}
	#endif

	void Cycle() {
		int top = Count - 1;
		if (top < 0) {
			return;
		}

		Transform slot = transform.GetChild (top);
		Card card = slot.GetComponentInChildren<Card>();
		card.localPileTag = sortPile.tag;
		card.localPileSlot = sortPile.Count;
	}
}
