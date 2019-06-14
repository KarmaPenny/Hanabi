using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class JoinMatchButton : MonoBehaviour {
	public MatchInfoSnapshot match;
	public int maxDisplayCharacters = 13;
	public Text buttonText;

	NetworkManager manager;

	void Start() {
		manager = NetworkManager.singleton;
		string displayText = "Join: " + match.name;
		if (match.name.Length > maxDisplayCharacters) {
			displayText = "Join: " + match.name.Substring (0, maxDisplayCharacters - 1) + "...";
		}
		buttonText.text = displayText;
	}

	void Update() {
		if (manager.matches == null) {
			Destroy (gameObject);
		} else {
			bool listed = false;
			foreach (MatchInfoSnapshot listedMatch in manager.matches) {
				if (listedMatch.networkId == match.networkId) {
					listed = true;
					break;
				}
			}

			if (!listed) {
				Destroy (gameObject);
			}
		}
	}

	public void OnMatchJoined(bool success, string info, MatchInfo matchInfo) {
		MainMenu.connecting = false;
		manager.OnMatchJoined (success, info, matchInfo);
	}

	public void JoinMatch() {
		MainMenu.connecting = true;
		manager.matchMaker.JoinMatch (match.networkId, "", "", "", 0, 0, OnMatchJoined);
	}
}
