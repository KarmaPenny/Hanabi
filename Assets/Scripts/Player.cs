using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {
	[SyncVar] int _number = 1;
	public int assignedNumber { get { return _number; } }
	public static int number { get { return (player != null) ? player._number : 1; } }
	public static int maxPlayers = 2;
	public static GameObject[] players = new GameObject[4];
	public float rejectTime = 1f;

	public AudioClip hintSound;
	public AudioClip failSound;

	static Player player;

	[SyncVar] public string userName = "player";

	void Start() {
		if (hasAuthority) {
			player = this;
			CmdAssignPlayerNumber ();
			CmdSetPlayerName (System.Environment.UserName);
		}
	}

	[Command] void CmdSetPlayerName(string newName) {
		userName = newName;
	}

	[Command] void CmdAssignPlayerNumber() {
		for (int i = 0; i < players.Length; i++) {
			if (players [i] == null) {
				players [i] = gameObject;
				_number = i + 1;
				break;
			}
		}
	}

	public static void ReassignPlayerNumbers() {
		players = new GameObject[4];

		GameObject[] playerObjects = GameObject.FindGameObjectsWithTag ("Player");
		List<GameObject> tmp = new List<GameObject> ();
		foreach (GameObject playerObject in playerObjects) {
			tmp.Add (playerObject);
		}

		for (int i = 0; i < Deck.numPlayers; i++) {
			int index = Random.Range (0, tmp.Count);
			GameObject playerObject = tmp [index];
			players [i] = playerObject;
			playerObject.GetComponent<Player> ()._number = i + 1;
			tmp.RemoveAt (index);
		}
	}

	IEnumerator DrawCard(string pileTag, int slot) {
		yield return new WaitForSeconds (0.5f);
		Deck.DealCard (pileTag, slot);
	}

	[ClientRpc] void RpcDiscard(GameObject cardObject, bool playRejectSound) {
		Card card = cardObject.GetComponent<Card> ();
		card.pileTag = "Discard";
		card.pileSlot = 0;
		card.localPileTag = "Discard";
		int top = GameObject.FindGameObjectWithTag ("Discard").GetComponent<CardPile> ().Count;
		card.localPileSlot = top;
		if (playRejectSound) {
			AudioSource.PlayClipAtPoint (failSound, Vector3.zero, 0.667f);
		}
	}

	[Command] void CmdDiscard(GameObject cardObject) {
		if (Deck.gameOver || (Deck.turn % Deck.numPlayers) + 1 != assignedNumber) {
			// cheater! it isn't your turn
			return;
		}

		Deck.UpdateTurn (true);
		Card card = cardObject.GetComponent<Card> ();
		Deck.cardsInPlay [card.colorIndex - 1, card.number - 1]--;
		RpcDiscard (cardObject, false);
		StartCoroutine (DrawCard (card.pileTag, card.pileSlot));
		Hints.remaining++;
		Deck.CheckGameOver (false, card.colorIndex);
	}

	public static void Discard(GameObject cardObject) {
		player.CmdDiscard (cardObject);
	}

	IEnumerator RejectCard(GameObject cardObject) {
		yield return new WaitForSeconds (rejectTime);
		RpcDiscard (cardObject, true);
	}

	[Command] void CmdPlay(GameObject cardObject) {
		if (Deck.gameOver || (Deck.turn % Deck.numPlayers) + 1 != assignedNumber) {
			// cheater! it isn't your turn
			return;
		}

		Deck.UpdateTurn (true);
		Card card = cardObject.GetComponent<Card> ();
		Deck.cardsInPlay [card.colorIndex - 1, card.number - 1]--;
		CardPile playPile = (GameObject.FindGameObjectWithTag ("Column" + card.colorIndex)).GetComponent<CardPile> ();
		StartCoroutine (DrawCard (card.pileTag, card.pileSlot));
		if (playPile.Count + 1 == card.number) {
			card.pileTag = "Column" + card.colorIndex;
			card.pileSlot = card.number - 1;
			if (card.number == 5) {
				Hints.remaining++;
			}
			Deck.CheckGameOver (true, card.colorIndex);
		} else {
			card.pileTag = "RejectedPile";
			card.pileSlot = 0;
			Bomb.fuse--;
			StartCoroutine (RejectCard (cardObject));
			Deck.CheckGameOver (false, card.colorIndex);
		}
	}

	public static void Play(GameObject cardObject) {
		player.CmdPlay (cardObject);
	}

	[ClientRpc] void RpcRevealColor(GameObject cardObject, int color) {
		Transform hand = cardObject.transform.parent.parent;
		for (int i = 0; i < hand.childCount; i++) {
			Transform sibling = hand.GetChild (i);
			Card siblingCard = sibling.GetComponentInChildren<Card> ();
			if (siblingCard.colorIndex == color) {
				siblingCard.RevealColor ();
			}
		}
		AudioSource.PlayClipAtPoint (hintSound, Vector3.zero, 0.667f);
	}

	[Command] void CmdRevealColor(GameObject cardObject) {
		if (Deck.gameOver || (Deck.turn % Deck.numPlayers) + 1 != assignedNumber) {
			// cheater! it isn't your turn
			return;
		}

		if (Hints.remaining <= 0) {
			// cheater! there are no hints left
			return;
		}

		bool success = false;
		Card card = cardObject.GetComponent<Card> ();
		Transform hand = card.transform.parent.parent;
		for (int i = 0; i < hand.childCount; i++) {
			Transform sibling = hand.GetChild (i);
			Card siblingCard = sibling.GetComponentInChildren<Card> ();
			if (siblingCard.colorIndex == card.colorIndex) {
				if (!siblingCard.colorKnown) {
					success = true;
				}
				siblingCard.colorKnown = true;
			}
		}

		// prevent useless hints
		if (success) {
			Deck.UpdateTurn (false);
			Hints.remaining--;
			RpcRevealColor (cardObject, card.colorIndex);
		}
	}

	public static void RevealColor(GameObject cardObject) {
		player.CmdRevealColor (cardObject);
	}

	[ClientRpc] void RpcRevealNumber(GameObject cardObject, int cardNumber) {
		Transform hand = cardObject.transform.parent.parent;
		for (int i = 0; i < hand.childCount; i++) {
			Transform sibling = hand.GetChild (i);
			Card siblingCard = sibling.GetComponentInChildren<Card> ();
			if (siblingCard.number == cardNumber) {
				siblingCard.RevealNumber ();
			}
		}
		AudioSource.PlayClipAtPoint (hintSound, Vector3.zero, 0.667f);
	}

	[Command] void CmdRevealNumber(GameObject cardObject) {
		if (Deck.gameOver || (Deck.turn % Deck.numPlayers) + 1 != assignedNumber) {
			// cheater! it isn't your turn
			return;
		}

		if (Hints.remaining <= 0) {
			// cheater! there are no hints left
			return;
		}

		bool success = false;
		Card card = cardObject.GetComponent<Card> ();
		Transform hand = card.transform.parent.parent;
		for (int i = 0; i < hand.childCount; i++) {
			Transform sibling = hand.GetChild (i);
			Card siblingCard = sibling.GetComponentInChildren<Card> ();
			if (siblingCard.number == card.number) {
				if (!siblingCard.numberKnown) {
					success = true;
				}
				siblingCard.numberKnown = true;
			}
		}

		if (success) {
			Deck.UpdateTurn (false);
			Hints.remaining--;
			RpcRevealNumber (cardObject, card.number);
		}
	}

	public static void RevealNumber(GameObject cardObject) {
		player.CmdRevealNumber (cardObject);
	}

	[Command] void CmdNewGame() {
		Deck.NewGame ();
	}

	public static void NewGame() {
		player.CmdNewGame ();
	}
}
