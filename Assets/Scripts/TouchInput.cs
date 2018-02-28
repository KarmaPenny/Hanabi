using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInput : MonoBehaviour {
	public bool isPressed { get; set; }
	public bool wasPressed { get; set; }
	public bool wasLongPressed { get; set; }
	public int fingerId { get; set; }
	public float touchTime { get; set; }

	void Start() {
		fingerId = -1;
	}

	void Update () {
		wasPressed = false;
		wasLongPressed = false;
	}
}
