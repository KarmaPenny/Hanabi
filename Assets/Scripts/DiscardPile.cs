using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardPile : CardPile {
	static DiscardPile singleton;

	public static int top { get { return singleton.Count; } }
	public Vector3 sortLift = new Vector3(1.3f, 2f, 0.7f);

	bool selected;

	protected override void Start() {
		base.Start ();
		singleton = this;
	}

	void Update() {
		if (selected && Input.GetButtonDown ("Fire1") && transform.GetChild(1).childCount > 0) {
			Player.CycleDiscardPile ();
		}
	}

	void OnMouseEnter() {
		selected = true;
	}

	void OnMouseExit() {
		selected = false;
	}

	public static void Cycle() {
		Card card = null;
		Transform slot = singleton.transform.GetChild (top - 1);
		Vector3 lastPosition = slot.position;
		Quaternion lastRotation = slot.rotation;

		// move every slot up
		for(int i = 0; i < singleton.transform.childCount; i++) {
			slot = singleton.transform.GetChild (i);
			if (slot.childCount == 0) {
				break;
			}
			card = slot.GetComponentInChildren<Card>();
			card.pileSlot++;
			card.transform.parent = null;
			Vector3 tmpPosition = slot.position;
			Quaternion tmpRotation = slot.rotation;
			slot.position = new Vector3 (lastPosition.x, lastPosition.y, tmpPosition.z);
			slot.rotation = lastRotation;
			lastPosition = tmpPosition;
			lastRotation = tmpRotation;
			card.transform.parent = slot;
		}

		// move top slot to first slot
		if (card != null) {
			card.pileSlot = 0;
			card.liftOffset = singleton.sortLift;
			card.transform.parent = singleton.transform.GetChild (0);
			AudioSource.PlayClipAtPoint (card.moveSounds [Random.Range (0, card.moveSounds.Length)], Vector3.zero, 1);
		}
	}
}
