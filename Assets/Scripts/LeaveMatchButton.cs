using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LeaveMatchButton : MonoBehaviour {
	NetworkManager manager;

	void Start() {
		manager = NetworkManager.singleton;
	}

	public void LeaveMatch() {
		if (NetworkServer.active || NetworkClient.active) {
			manager.StopHost ();
		}
	}
}
