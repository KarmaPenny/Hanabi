using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Hints : NetworkBehaviour {
	static Hints singleton;

	[SyncVar] int _remaining = 8;
	public static int remaining {
		get { return (singleton != null) ? singleton._remaining : 8; }
		set { singleton._remaining = Mathf.Clamp (value, 0, 8); }
	}

	TouchInput touchInput;

	void Start () {
		singleton = this;
		touchInput = GetComponent<TouchInput> ();
	}

	bool selected;

	void Update() {
		if ((selected && Input.GetButtonDown ("Fire1")) || touchInput.wasPressed || touchInput.wasLongPressed) {
			Card.showAllyCards = !Card.showAllyCards;
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
