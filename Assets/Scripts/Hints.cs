using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Hints : NetworkBehaviour {
	static Hints singleton;

	[SyncVar] int _remaining = 8;
	public static int remaining { get { return (singleton != null) ? singleton._remaining : 8; } set { singleton._remaining = Mathf.Clamp (value, 0, 8); } }

	void Start () {
		singleton = this;
	}

	bool selected;

	void Update() {
		if (selected) {
			if (Input.GetButtonDown ("Fire1")) {
				Card.showAllyCards = !Card.showAllyCards;
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
