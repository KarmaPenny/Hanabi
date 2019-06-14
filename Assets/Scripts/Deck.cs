using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Deck : CardPile {
	public Transform cardPrefab;

	public AudioClip[] shuffleSounds;

	static Deck singleton;
	static bool dealing = false;

	public static int[,] cardsInPlay = new int[5,5];
	static void ResetCardsInPlay() {
		for (int color = 0; color < 5; color++) {
			for (int number = 0; number < 5; number++) {
				int count = (number == 0) ? 3 : (number == 4) ? 1 : 2;
				cardsInPlay [color, number] = count;
			}
		}
	}

	[SyncVar] int _numPlayers = 2;
	public static int numPlayers { get { return (singleton != null) ? singleton._numPlayers : 2; } }

	bool selected;

	[SyncVar] int _turn = -1;
	[SyncVar] int endTurn = -2;
	public static int lastTurn { get { return (singleton != null) ? singleton.endTurn - 1 : -2; } }
	public static bool isEmpty { get { return (singleton != null) ? singleton._turn == singleton.endTurn : false; } }
	[SyncVar] bool _gameOver;
	public static bool gameOver { get { return (singleton == null) ? false : singleton._gameOver; } }
	public static void CheckGameOver(bool played, int color) {
		if (Bomb.fuse == 0 || singleton._turn == singleton.endTurn) {
			singleton._gameOver = true;
			return;
		}

		// end game if play piles are completed
		for (int i = 1; i <= 5; i++) {
			GameObject playPileObject = GameObject.FindGameObjectWithTag ("Column" + i);
			CardPile playPile = playPileObject.GetComponent<CardPile> ();
			int playPileCount = playPile.Count;
			if (played && color == i) {
				playPileCount++;
			}
			if (playPileCount < 5 && cardsInPlay [i - 1, playPileCount] > 0) {
				singleton._gameOver = false;
				return;
			}
		}

		// nothing left to play
		singleton._gameOver = true;
	}
	public static int turn { get { return (singleton != null) ? singleton._turn : -1; } }
	public static int remaining { get { return (singleton != null) ? singleton.Count : 50; } } 

	protected override void Start () {
		base.Start ();
		singleton = this;
	}

	void Update() {
		if (hasAuthority) {
			if (GameObject.FindGameObjectsWithTag("Card").Length == 0) {
				SpawnCards ();
			}
		}
	}

	[ClientRpc] void RpcNewGame() {
		AudioSource.PlayClipAtPoint (shuffleSounds [Random.Range (0, shuffleSounds.Length)], Vector3.zero, 1);
	}

	[Server] public static void NewGame() {
		if (!dealing) {
			dealing = true;
			Hints.remaining = 8;
			Bomb.fuse = 3;
			singleton._turn = -1;
			singleton.endTurn = -2;
			ResetCardsInPlay ();
			singleton._gameOver = false;
			FireworksShow.totalScore = 0;

			// get a list of available deck slots
			List<int> deckSlots = new List<int> ();
			for (int i = 0; i < singleton.transform.childCount; i++) {
				deckSlots.Add (i);
			}

			// place cards in the deck
			foreach (GameObject cardObject in GameObject.FindGameObjectsWithTag("Card")) {
				Card card = cardObject.GetComponent<Card> ();
				int slot = Random.Range (0, deckSlots.Count);
				card.pileTag = "Deck";
				card.pileSlot = deckSlots [slot];
				card.numberKnown = false;
				card.colorKnown = false;
				deckSlots.RemoveAt (slot);
			}

			// Deal
			singleton.DealCards ();

			singleton.RpcNewGame ();
		}
	}

	public static void UpdateTurn(bool drawing) {
		if (drawing && singleton.Count == 1) {
			singleton.endTurn = singleton._turn + numPlayers + 1;
		}

		singleton._turn++;
	}

	[Server] void SpawnCards() {
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

			// place the 2s, 3s and 4s
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

		ResetCardsInPlay ();

		// Deal
		DealCards();
	}

	void SpawnCard(int color, int number, int slot) {
		Transform cardTransform = Instantiate (cardPrefab, transform.position, transform.rotation);
		Card card = cardTransform.GetComponent<Card> ();
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

	[Server] public void DealCards() {
		StartCoroutine (Deal ());
	}

	IEnumerator Deal() {
		// at least 2 players are required to start
		while (NetworkManager.singleton.numPlayers < 2) {
			yield return null;
		}

		// set the number of players at the time of the deal
		_numPlayers = NetworkManager.singleton.numPlayers;

		// mix up player order
		Player.ReassignPlayerNumbers ();

		// wait a bit so newly joined players see the deal
		yield return new WaitForSeconds (1.5f);

		// deal the cards out
		for (int p = 1; p <= _numPlayers; p++) {
			for (int i = 0; i < 5; i++) {
				DealCard ("Hand" + p, i);
				yield return new WaitForSeconds (0.25f);
			}
		}

		// start the game
		_turn = 0;

		dealing = false;
	}
}