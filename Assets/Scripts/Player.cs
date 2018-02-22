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

	[Server] void ServerDiscard(GameObject cardObject) {
		Card card = cardObject.GetComponent<Card> ();
		StartCoroutine (DrawCard (card.pileTag, card.pileSlot));
		card.pileTag = "Discard";
		card.pileSlot = DiscardPile.top;
	}

	[Command] void CmdDiscard(GameObject cardObject) {
		Deck.UpdateTurn (true);
		ServerDiscard (cardObject);
		Hints.remaining++;
	}

	public static void Discard(GameObject cardObject) {
		player.CmdDiscard (cardObject);
	}

	[Command] void CmdPlay(GameObject cardObject) {
		Deck.UpdateTurn (true);
		Card card = cardObject.GetComponent<Card> ();
		CardPile playPile = (GameObject.FindGameObjectWithTag ("Column" + card.colorIndex)).GetComponent<CardPile> ();
		if (playPile.Count + 1 == card.number) {
			StartCoroutine (DrawCard (card.pileTag, card.pileSlot));
			card.pileTag = "Column" + card.colorIndex;
			card.pileSlot = card.number - 1;
			if (card.number == 5) {
				Hints.remaining++;
			}
		} else {
			ServerDiscard (cardObject);
			Bomb.fuse--;
		}
	}

	public static void Play(GameObject cardObject) {
		player.CmdPlay (cardObject);
	}

	[ClientRpc] void RpcRevealColor(GameObject cardObject) {
		cardObject.GetComponent<Card> ().colorKnown = true;
	}

	[Command] void CmdRevealColor(GameObject cardObject) {
		Deck.UpdateTurn (false);
		Card card = cardObject.GetComponent<Card> ();
		Hints.remaining--;
		Transform hand = card.transform.parent.parent;
		for (int i = 0; i < hand.childCount; i++) {
			Transform sibling = hand.GetChild (i);
			Card siblingCard = sibling.GetComponentInChildren<Card> ();
			if (siblingCard.colorIndex == card.colorIndex) {
				RpcRevealColor (siblingCard.gameObject);
			}
		}
	}

	public static void RevealColor(GameObject cardObject) {
		player.CmdRevealColor (cardObject);
	}

	[ClientRpc] void RpcRevealNumber(GameObject cardObject) {
		cardObject.GetComponent<Card> ().numberKnown = true;
	}

	[Command] void CmdRevealNumber(GameObject cardObject) {
		Deck.UpdateTurn (false);
		Card card = cardObject.GetComponent<Card> ();
		Hints.remaining--;
		Transform hand = card.transform.parent.parent;
		for (int i = 0; i < hand.childCount; i++) {
			Transform sibling = hand.GetChild (i);
			Card siblingCard = sibling.GetComponentInChildren<Card> ();
			if (siblingCard.number == card.number) {
				RpcRevealNumber (siblingCard.gameObject);
			}
		}
	}

	public static void RevealNumber(GameObject cardObject) {
		player.CmdRevealNumber (cardObject);
	}

	[ClientRpc] void RpcCycleDiscardPile() {
		DiscardPile.Cycle ();
	}

	[Command] void CmdCycleDiscardPile() {
		RpcCycleDiscardPile ();
	}

	public static void CycleDiscardPile() {
		player.CmdCycleDiscardPile ();
	}

	[Command] void CmdNewGame() {
		Deck.NewGame ();
	}

	public static void NewGame() {
		player.CmdNewGame ();
	}
}
