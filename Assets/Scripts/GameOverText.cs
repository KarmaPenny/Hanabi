using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverText : MonoBehaviour {
	public Text score;

	void Update () {
		score.text = FireworksDisplay.totalScore.ToString ();
		GetComponent<Animator> ().SetBool ("show", FireworksDisplay.playing);

		if (Input.GetButtonDown ("Fire1") && FireworksDisplay.playing) {
			Player.NewGame ();
		}
	}
}
