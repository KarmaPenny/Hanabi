using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedealButton : MonoBehaviour {
	public void NewGame() {
		Player.NewGame ();
		MainMenu.opened = false;
	}
}
