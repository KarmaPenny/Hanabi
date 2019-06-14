using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class MainMenu : MonoBehaviour {
	public GameObject mainMenu;
	public GameObject inGameMenu;
	public Transform matchList;
	public Transform joinButtonPrefab;
	public float refreshTime = 1;
	public int listLimit = 7;

	public static bool opened = true;
	public static bool connecting = false;

	float nextRefresh = 0;
	NetworkManager manager;
	Animator animator;

	void Start() {
		manager = NetworkManager.singleton;
		manager.StartMatchMaker ();
		animator = GetComponent<Animator> ();
	}

	bool networkWasActive = true;
	bool networkIsActive { get { return NetworkServer.active || NetworkClient.active; } }

	void Update () {
		mainMenu.SetActive (!networkIsActive);
		inGameMenu.SetActive (networkIsActive);

		if (!networkIsActive) {
			opened = !connecting;
			if (manager.matchMaker != null) {
				// refresh matches periodically
				if (Time.time > nextRefresh) {
					nextRefresh = Time.time + refreshTime;

					manager.matchMaker.ListMatches (0, listLimit, "", true, 0, 0, manager.OnMatchList);
				}

				// spawn join buttons
				if (manager.matches != null) {
					foreach (MatchInfoSnapshot match in manager.matches) {
						bool alreadyExists = false;
						for (int i = 0; i < matchList.childCount; i++) {
							JoinMatchButton joinButton = matchList.GetChild(i).GetComponent<JoinMatchButton> ();
							if (joinButton != null) {
								if (joinButton.match.networkId == match.networkId) {
									alreadyExists = true;
									break;
								}
							}
						}

						if (!alreadyExists) {
							Transform joinButton = Instantiate (joinButtonPrefab, matchList);
							joinButton.GetComponent<JoinMatchButton> ().match = match;
						}
					}
				}
			} else {
				manager.StartMatchMaker ();
			}
		} else if (!networkWasActive) {
			opened = false;
		}

		animator.SetBool ("opened", opened);

		networkWasActive = networkIsActive;
	}
}
