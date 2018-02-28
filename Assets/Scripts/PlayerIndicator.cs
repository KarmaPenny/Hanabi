using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIndicator : MonoBehaviour {
	public Transform hand;
	public Text nameTag;
	public Color highlightColor;
	public Color lastTurnColor;
	public Image leftArrow;
	public Image rightArrow;

	void Update () {
		int handNumber = int.Parse (hand.tag.Substring (hand.tag.Length - 1, 1));
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		Player player = null;
		foreach (GameObject playerObject in players) {
			Player tmp = playerObject.GetComponent<Player> ();
			if (tmp.assignedNumber == handNumber) {
				player = tmp;
			}
		}

		nameTag.text = "";
		if (player != null) {
			nameTag.text = player.userName;
		}

		bool myTurn = ((Deck.turn % Deck.numPlayers) + 1 == handNumber && player != null && !Deck.gameOver);

		Color targetColor = Color.white;
		if (Deck.lastTurn > 0 && (Deck.lastTurn % Deck.numPlayers) + 1 == handNumber) {
			targetColor = lastTurnColor;
		} else if (myTurn) {
			targetColor = highlightColor;
		}

		nameTag.color = Color.Lerp (nameTag.color, targetColor, 4 * Time.deltaTime);
		leftArrow.color = Color.Lerp (leftArrow.color, targetColor, 4 * Time.deltaTime);
		rightArrow.color = Color.Lerp (rightArrow.color, targetColor, 4 * Time.deltaTime);
			
		GetComponent<Animator> ().SetBool ("myTurn", myTurn);
	}
}
