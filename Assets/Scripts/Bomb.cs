using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bomb : NetworkBehaviour {
	static Bomb singleton;

	[SyncVar] int _fuse = 3;
	public static int fuse { get { return (singleton == null) ? 3 : singleton._fuse; } set { singleton._fuse = Mathf.Clamp (value, 0, 3); } }

	void Start () {
		singleton = this;
	}

	void Update () {
		GetComponent<Animator> ().SetInteger ("fuse", fuse);
	}
}
