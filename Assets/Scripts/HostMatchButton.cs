using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class HostMatchButton : MonoBehaviour {
	NetworkManager manager;

	void Start() {
		manager = NetworkManager.singleton;
	}

	public void OnMatchCreate(bool success, string info, MatchInfo matchInfo) {
		manager.OnMatchCreate (success, info, matchInfo);
		MainMenu.connecting = false;
	}

	public void CreateMatch() {
		string matchName = "Default";
		matchName = System.Environment.UserName;
		manager.matchMaker.CreateMatch (matchName, manager.matchSize, true, "", "", "", 0, 0, manager.OnMatchCreate);
	}
}
