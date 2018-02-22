using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour {
	bool selected;

	void Update() {
		if (selected) {
			if (Input.GetButtonDown ("Fire1")) {
				Player.NewGame ();
			}
		}
	}

	void OnMouseEnter() {
		selected = true;
	}

	void OnMouseExit() {
		selected = false;
	}
}
