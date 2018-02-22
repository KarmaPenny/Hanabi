using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIndicator : MonoBehaviour {
	public Transform hand;
	public Text nameTag;

	void Update () {
		int handNumber = int.Parse (hand.tag.Substring (hand.tag.Length - 1, 1));
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		nameTag.text = "";
		foreach (GameObject playerObject in players) {
			Player player = playerObject.GetComponent<Player> ();
			if (player.assignedNumber == handNumber) {
				nameTag.text = player.userName;
			}
		}
			
		bool myTurn = (Deck.turn % Deck.numPlayers) + 1 == handNumber;
		GetComponent<Animator> ().SetBool ("myTurn", myTurn);
	}
}
