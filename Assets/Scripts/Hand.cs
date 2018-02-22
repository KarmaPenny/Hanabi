using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Hand : CardPile {
	public int number = 1;
	
	void Update () {
		int tagNumber = Player.number;

		if (number == 2) {
			if (Deck.numPlayers > 2) {
				tagNumber = Player.number + 1;
			} else {
				tag = "Hand3";
				return;
			}
		} else if (number == 3) {
			if (Deck.numPlayers > 2) {
				tagNumber = Player.number + 2;
			} else {
				tagNumber = (Player.number == 1) ? 2 : 1;
			}
		} else if (number == 4) {
			if (Deck.numPlayers > 3) {
				tagNumber = Player.number + 3;
			} else {
				tag = "Hand4";
				return;
			}
		}

		if (tagNumber > Deck.numPlayers) {
			tagNumber -= Deck.numPlayers;
		}

		tag = "Hand" + tagNumber;
	}
}
