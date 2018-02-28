using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireworksShow : MonoBehaviour {
	public Text scoreText;

	public Transform fireworkPrefab;
	public float xPosMin = -3f;
	public float xPosMax = 3f;
	public float yPos = -7f;
	public float zPos = -2f;
	public float minDelay = 0.3f;
	public float maxDelay = 0.5f;
	public float finaleMinDelay = 0.1f;
	public float finaleMaxDelay = 0.3f;
	public Color[] explosionColors;
	public bool test = false;

	public AudioClip cheerSound;
	public AudioClip sighSound;

	bool playing = false;
	static int _totalScore = 0;
	public static int totalScore { get { return _totalScore; } set { _totalScore = Mathf.Clamp (value, 0, 25); } }

	void Update () {
		// keep score display up to date
		scoreText.text = totalScore.ToString ();

		// show ending screen when game is over and hide it when it is not
		GetComponent<Animator> ().SetBool ("show", (Deck.gameOver || test));

		// stop playing when a new game starts
		if (!Deck.gameOver) {
			playing = false;
		}

		// let player click to restart game when the fireworks show is playing
		if ((Input.GetButtonDown ("Fire1") || Input.touches.Length > 0) && playing) {
			Player.NewGame ();
		}
	}

	public void PlayGameOverSound() {
		// tally the score
		int score = 0;
		for (int i = 1; i <= 5; i++) {
			GameObject playPileObject = GameObject.FindGameObjectWithTag ("Column" + i);
			if (playPileObject != null) {
				CardPile playPile = playPileObject.GetComponent<CardPile> ();
				score += playPile.Count;
			}
		}

		if (score < 20) {
			AudioSource.PlayClipAtPoint (sighSound, Vector3.zero, 1);
		} else {
			AudioSource.PlayClipAtPoint (cheerSound, Vector3.zero, 1);
		}
	}

	public void PlayFireworksShow () {
		if (!playing) {
			playing = true;
			totalScore = 0;
			StartCoroutine (FireworksShowAnimation ());
		}
	}

	IEnumerator FireworksShowAnimation() {
		// tally score and create list of colors based on the cards that were played
		List<Color> colors = new List<Color> ();
		int score = 0;
		if (test) {
			for (int i = 0; i < 25; i++) {
				colors.Add(explosionColors[Random.Range(0, explosionColors.Length)]);
				score++;
			}
		} else {
			for (int i = 1; i <= 5; i++) {
				GameObject playPileObject = GameObject.FindGameObjectWithTag ("Column" + i);
				CardPile playPile = playPileObject.GetComponent<CardPile> ();
				for (int j = 0; j < playPile.Count; j++) {
					colors.Add (explosionColors [i - 1]);
					score++;
				}
			}
		}

		// launch a firework for each card played
		while(colors.Count > 0) {
			Vector3 position = new Vector3(Random.Range (xPosMin, xPosMax), yPos, zPos);
			Transform fireworkTransform = Instantiate<Transform> (fireworkPrefab, position, Quaternion.identity);
			int colorIndex = Random.Range (0, colors.Count);
			Color color = colors [colorIndex];
			colors.RemoveAt (colorIndex);
			fireworkTransform.GetComponent<Firework> ().color = color;
			yield return new WaitForSeconds (Random.Range(minDelay, maxDelay));
			if (!playing) {
				yield break;
			}
		}

		yield return new WaitForSeconds (3f);

		if (score == 25) {
			while (playing) {
				Vector3 position = new Vector3(Random.Range (xPosMin, xPosMax), yPos, zPos);
				Transform fireworkTransform = Instantiate<Transform> (fireworkPrefab, position, Quaternion.identity);
				fireworkTransform.GetComponent<Firework> ().color = explosionColors[Random.Range(0, explosionColors.Length)];
				yield return new WaitForSeconds (Random.Range(finaleMinDelay, finaleMaxDelay));
			}
		}
	}
}
