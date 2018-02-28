using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitButton : MonoBehaviour {
	void Update() {
		transform.SetSiblingIndex (transform.parent.childCount - 1);
	}

	public void QuitGame() {
		Application.Quit ();
	}
}
