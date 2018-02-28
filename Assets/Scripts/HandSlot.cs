using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandSlot : MonoBehaviour {
	public float tabHeight = 0.35f;
	public float tabSpeed = 4;
	public AudioClip[] slideSounds;

	bool prevSelected = false;
	bool selected = false;

	Vector3 startPosition;
	Quaternion startRotation;

	Vector3 colliderCenter;
	Vector3 colliderSize;

	TouchInput touchInput;

	void Start() {
		startPosition = transform.position;
		startRotation = transform.localRotation;
		BoxCollider collider = GetComponent<BoxCollider> ();
		colliderCenter = collider.center;
		colliderSize = collider.size;
		touchInput = GetComponent<TouchInput> ();
	}

	void Update() {
		bool canInteract = true;

		// prevent interaction when not players turn
		if (Deck.gameOver || (Deck.turn % Deck.numPlayers) + 1 != Player.number) {
			selected = false;
			canInteract = false;
		}

		// prevent interaction with other players cards when out of hints
		if (transform.parent.tag != "Hand" + Player.number) {
			if (Hints.remaining <= 0) {
				selected = false;
				canInteract = false;
			}
		}

		if (selected != prevSelected) {
			string handTag = transform.parent.tag;
			int handNumber = 4;
			handNumber = int.Parse (handTag.Substring (handTag.Length - 1, 1));
			if (handNumber <= Deck.numPlayers) {
				AudioSource.PlayClipAtPoint (slideSounds [Random.Range (0, slideSounds.Length)], Vector3.zero, 0.75f);
			}
		}
		prevSelected = selected;

		tabHeight = (transform.parent.tag != "Hand" + Player.number) ? -Mathf.Abs (tabHeight) : Mathf.Abs (tabHeight);
		Vector3 position = (selected) ? startPosition + transform.parent.up * tabHeight : startPosition;
		Quaternion rotation = (selected) ? Quaternion.identity : startRotation;
		transform.position = Vector3.Lerp (transform.position, position, tabSpeed * Time.deltaTime);
		transform.localRotation = Quaternion.Lerp (transform.localRotation, rotation, tabSpeed * Time.deltaTime);

		BoxCollider collider = GetComponent<BoxCollider> ();
		collider.center = (selected) ? colliderCenter - Vector3.up * tabHeight / 2 : colliderCenter;
		collider.size = (selected) ? colliderSize + Vector3.up * Mathf.Abs (tabHeight) : colliderSize;

		if (canInteract) {
			if (transform.parent.tag == "Hand" + Player.number) {
				if ((selected && Input.GetButtonDown ("Fire2")) || touchInput.wasLongPressed) {
					GameObject card = transform.GetChild (0).gameObject;
					if (card != null) {
						Player.Discard (card);
					}
				} else if ((selected && Input.GetButtonDown ("Fire1")) || touchInput.wasPressed) {
					GameObject card = transform.GetChild (0).gameObject;
					if (card != null) {
						Player.Play (card);
					}
				}
			} else {
				if ((selected && Input.GetButtonDown ("Fire2")) || touchInput.wasLongPressed) {
					GameObject card = transform.GetChild (0).gameObject;
					if (card != null) {
						Player.RevealColor (card);
					}
				} else if ((selected && Input.GetButtonDown ("Fire1")) || touchInput.wasPressed) {
					GameObject card = transform.GetChild (0).gameObject;
					if (card != null) {
						Player.RevealNumber (card);
					}
				}
			}
		}
	}

	#if UNITY_STANDALONE
	void OnMouseOver() {
		selected = true;
	}

	void OnMouseExit() {
		selected = false;
	}
	#endif
}
