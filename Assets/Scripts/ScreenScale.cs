using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenScale : MonoBehaviour {
	void Update () {
		float y = Camera.main.orthographicSize * 2;
		float x = y * ((float)Screen.width) / ((float)Screen.height) + y / 2;
		transform.localScale = new Vector3 (x, y, 1);
	}
}
