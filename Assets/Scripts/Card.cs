using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Card : NetworkBehaviour {
	public Color[] colors;
	public Text numberText;
	public MeshRenderer colorMeshRenderer;
	public float revealSpeed = 4f;
	public float liftHeight = 1.5f;
	public AudioClip[] moveSounds;
	public ParticleSystem revealColorEffect;
	public Animator revealNumberAnimator;
	public Text revealNumberText;

	[SyncVar] public int number;
	[SyncVar] public int colorIndex;
	[SyncVar] public string pileTag;
	[SyncVar] public int pileSlot;
	[SyncVar] public bool numberKnown;
	[SyncVar] public bool colorKnown;
	public string localPileTag;
	public int localPileSlot;

	public static bool showAllyCards = true;

	public Vector3 liftOffset { get; set; }

	Animator animator;

	void Start() {
		animator = GetComponent<Animator> ();

		liftOffset = Vector3.zero;

		// move card to its assigned pile
		GameObject pile = GameObject.FindGameObjectWithTag(pileTag);
		Transform slot = pile.transform.GetChild (pileSlot);
		transform.parent = slot;
		transform.position = slot.position;
		transform.rotation = slot.rotation;
	}

	void Update () {
		bool showNumber = false;
		bool showColor = false;

		if (pileTag == "Deck") {
			showNumber = false;
			showColor = false;
		} else if (pileTag == "Discard" || pileTag.StartsWith ("Column") || pileTag == "RejectedPile" || pileTag == "SortPile") {
			showNumber = true;
			showColor = true;
		} else if (pileTag == "Hand" + Player.number) {
			showNumber = numberKnown;
			showColor = colorKnown;
		} else {
			showNumber = showAllyCards || numberKnown;
			showColor = showAllyCards || colorKnown;
		}

		// update animator state
		animator.SetBool ("faceUp", showColor || showNumber);

		// set card color
		Color cardColor = (showColor) ? colors [colorIndex] : colors[0];
		colorMeshRenderer.material.color = Color.Lerp (colorMeshRenderer.material.color, cardColor, revealSpeed * Time.deltaTime);

		// set number color
		numberText.text = number.ToString();
		Color numberColor = (showNumber) ? Color.white : new Color (1, 1, 1, 0);
		Color outlineColor = (showNumber) ? new Color (0, 0, 0, 0.35f) : Color.clear;
		numberText.color = Color.Lerp (numberText.color, numberColor, revealSpeed * Time.deltaTime);
		Outline outline = numberText.GetComponent<Outline> ();
		outline.effectColor = Color.Lerp (outline.effectColor, outlineColor, revealSpeed * Time.deltaTime);

		// move to assigned pile
		string activePileTag = (pileTag == "Discard") ? localPileTag : pileTag;
		int activePileSlot = (pileTag == "Discard") ? localPileSlot : pileSlot;
		GameObject pile = GameObject.FindGameObjectWithTag(activePileTag);
		Transform slot = pile.transform.GetChild (activePileSlot);
		if (transform.parent != slot) {
			if (activePileTag != "Deck") {
				AudioSource.PlayClipAtPoint (moveSounds [Random.Range (0, moveSounds.Length)], Vector3.zero, 1);
			}
			liftOffset = new Vector3(liftOffset.x, liftOffset.y, -liftHeight);
		}
		transform.parent = slot;
		transform.position = Vector3.Lerp (transform.position, slot.position + liftOffset, revealSpeed * Time.deltaTime);
		transform.rotation = Quaternion.Lerp (transform.rotation, slot.rotation, revealSpeed * Time.deltaTime);

		// reduce lift height
		liftOffset = Vector3.Lerp(liftOffset, Vector3.zero, revealSpeed * revealSpeed * Time.deltaTime);

		// expand discard pile if joining mid game
		if (pileTag == "Discard" && pileSlot == 0 && transform.GetSiblingIndex() > 0) {
			localPileTag = "Discard";
			localPileSlot = transform.GetSiblingIndex ();
		}
	}

	public void RevealColor() {
		ParticleSystem.MainModule main = revealColorEffect.main;
		main.startColor = colors [colorIndex];
		revealColorEffect.Play ();
	}

	public void RevealNumber() {
		revealNumberText.text = number.ToString ();
		revealNumberAnimator.SetTrigger ("play");
	}
}
