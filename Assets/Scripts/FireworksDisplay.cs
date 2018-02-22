using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworksDisplay : MonoBehaviour {
	public Transform fireworkPrefab;
	public float xRange = 3f;
	public float minDelay = 0.3f;
	public float maxDelay = 0.5f;
	public float finaleMinDelay = 0.1f;
	public float finaleMaxDelay = 0.3f;
	public Color[] explosionColors;
	public bool test = false;

	public static bool playing = false;
	static int _totalScore = 0;
	public static int totalScore { get { return _totalScore; } set { _totalScore = Mathf.Clamp (value, 0, 25); } }

	void Update() {
		// tally the score
		int score = 0;
		for (int i = 1; i <= 5; i++) {
			GameObject playPileObject = GameObject.FindGameObjectWithTag ("Column" + i);
			if (playPileObject != null) {
				CardPile playPile = playPileObject.GetComponent<CardPile> ();
				score += playPile.Count;
			}
		}

		// if game is over
		if (score >= 25 || Bomb.fuse == 0 || Deck.isEmpty || test) {
			// play fireworks display if it is not already playing
			if (!playing) {
				playing = true;
				totalScore = 0;
				StartCoroutine (Play ());
			}
		} else {
			// stop playing fireworks display
			playing = false;
		}
	}

	IEnumerator Play() {
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

		// wait a bit before starting
		yield return new WaitForSeconds (0.5f);
		if (!playing) {
			yield break;
		}

		// launch a firework for each card played
		while(colors.Count > 0) {
			Vector3 position = transform.position + Vector3.right * Random.Range (-xRange, xRange);
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
				Vector3 position = transform.position + Vector3.right * Random.Range (-xRange, xRange);
				Transform fireworkTransform = Instantiate<Transform> (fireworkPrefab, position, Quaternion.identity);
				fireworkTransform.GetComponent<Firework> ().color = explosionColors[Random.Range(0, explosionColors.Length)];
				yield return new WaitForSeconds (Random.Range(finaleMinDelay, finaleMaxDelay));
			}
		}
	}
}
