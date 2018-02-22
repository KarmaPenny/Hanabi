using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HideGui : MonoBehaviour {
	void Update () {
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		GetComponent<NetworkManagerHUD> ().showGUI = players.Length == 0;
	}
}
