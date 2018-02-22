using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hint : MonoBehaviour {
	public int index = 0;

	void Update () {
		GetComponent<Animator> ().SetBool ("faceDown", index >= Hints.remaining);
	}
}
