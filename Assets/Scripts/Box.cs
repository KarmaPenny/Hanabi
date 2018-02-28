using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour {
	bool selected;

	TouchInput touchInput;

	void Start() {
		touchInput = GetComponent<TouchInput> ();
	}

	void Update() {
		if ((selected && Input.GetButtonDown ("Fire1")) || touchInput.wasPressed || touchInput.wasLongPressed) {
			MainMenu.opened = !MainMenu.opened;
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
}
