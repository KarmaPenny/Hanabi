using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIndicatorArrow : MonoBehaviour {
	public RectTransform playerName;
	public int sign = -1;

	void Update () {
		float x = sign * playerName.localScale.x * playerName.rect.width / 2;
		transform.localPosition = new Vector3 (x, transform.localPosition.y, transform.localPosition.z);
	}
}
