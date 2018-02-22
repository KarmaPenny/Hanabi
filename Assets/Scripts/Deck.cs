using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Deck : CardPile {
	public Transform cardPrefab;

	public AudioClip[] shuffleSounds;

	static Deck singleton;

	[SyncVar] int _numPlayers = 2;
	public static int numPlayers { get { return (singleton != null) ? singleton._numPlayers : 2; } }

	bool selected;
	List<Card> spawnedCards = new List<Card>();

	[SyncVar] int _turn = -1;
	[SyncVar] int endTurn = -2;
	public static bool isEmpty { get { return (singleton != null) ? singleton._turn == singleton.endTurn : false; } }
	public static int turn {
		get {
			// end game if bomb fuse is gone
			if (singleton == null || Bomb.fuse == 0) {
				return -1;
			}

			// end game if all 25 points scored
			int score = 0;
			for (int i = 1; i <= 5; i++) {
				GameObject playPileObject = GameObject.FindGameObjectWithTag ("Column" + i);
				if (playPileObject != null) {
					CardPile playPile = playPileObject.GetComponent<CardPile> ();
					score += playPile.Count;
				}
			}
			if (score >= 25) {
				return -1;
			}

			// end game if deck is empty
			if (singleton._turn == singleton.endTurn) {
				return -1;
			}

			return singleton._turn;
		}
	}

	protected override void Start () {
		base.Start ();

		singleton = this;

		if (hasAuthority) {
			// get a list of available deck slots
			List<int> deckSlots = new List<int> ();
			for (int i = 0; i < transform.childCount; i++) {
				deckSlots.Add (i);
			}

			// place cards in the deck
			for (int color = 1; color < 6; color++) {
				// place the 5s
				int slot = Random.Range (0, deckSlots.Count);
				SpawnCard (color, 5, deckSlots [slot]);
				deckSlots.RemoveAt (slot);

				// place the 1s
				for (int i = 0; i < 3; i++) {
					slot = Random.Range (0, deckSlots.Count);
					SpawnCard (color, 1, deckSlots [slot]);
					deckSlots.RemoveAt (slot);
				}

				// place teh 2s, 3s and 4s
				for (int i = 0; i < 2; i++) {
					slot = Random.Range (0, deckSlots.Count);
					SpawnCard (color, 2, deckSlots [slot]);
					deckSlots.RemoveAt (slot);
					slot = Random.Range (0, deckSlots.Count);
					SpawnCard (color, 3, deckSlots [slot]);
					deckSlots.RemoveAt (slot);
					slot = Random.Range (0, deckSlots.Count);
					SpawnCard (color, 4, deckSlots [slot]);
					deckSlots.RemoveAt (slot);
				}
			}

			// Deal
			DealCards();
		}
	}

	[ClientRpc] void RpcNewGame() {
		AudioSource.PlayClipAtPoint (shuffleSounds [Random.Range (0, shuffleSounds.Length)], Vector3.zero, 1);
	}

	public static void NewGame() {
		Hints.remaining = 8;
		Bomb.fuse = 3;
		singleton._turn = -1;
		singleton.endTurn = -2;

		// get a list of available deck slots
		List<int> deckSlots = new List<int> ();
		for (int i = 0; i < singleton.transform.childCount; i++) {
			deckSlots.Add (i);
		}

		// place cards in the deck
		foreach (Card card in singleton.spawnedCards) {
			int slot = Random.Range (0, deckSlots.Count);
			card.pileTag = "Deck";
			card.pileSlot = deckSlots [slot];
			deckSlots.RemoveAt (slot);
		}

		// Deal
		singleton.DealCards();

		singleton.RpcNewGame ();
	}

	public static void UpdateTurn(bool drawing) {
		if (drawing && singleton.Count == 1) {
			singleton.endTurn = singleton._turn + numPlayers + 1;
		}

		singleton._turn++;
	}

	void SpawnCard(int color, int number, int slot) {
		Transform cardTransform = Instantiate (cardPrefab, transform.position, transform.rotation);
		Card card = cardTransform.GetComponent<Card> ();
		spawnedCards.Add (card);
		card.colorIndex = color;
		card.number = number;
		card.pileTag = "Deck";
		card.pileSlot = slot;
		NetworkServer.Spawn (cardTransform.gameObject);
	}

	public static void DealCard(string pileTag, int slot) {
		List<Card> cards = singleton.Cards;
		if (cards.Count > 0) {
			Card card = cards [cards.Count - 1];
			card.pileTag = pileTag;
			card.pileSlot = slot;
		}
	}

	public void DealCards() {
		StartCoroutine (Deal ());
	}

	IEnumerator Deal() {
		// at least 2 players are required to start
		while (NetworkManager.singleton.numPlayers < 2) {
			yield return null;
		}

		// wait a bit so newly joined players see the deal
		yield return new WaitForSeconds (1.5f);

		// set the number of players at the time of the deal
		_numPlayers = NetworkManager.singleton.numPlayers;

		// mix up player order
		Player.ReassignPlayerNumbers ();

		// deal the cards out
		for (int p = 1; p <= _numPlayers; p++) {
			for (int i = 0; i < 5; i++) {
				DealCard ("Hand" + p, i);
				yield return new WaitForSeconds (0.25f);
			}
		}

		// start the game
		_turn = 0;
	}
}