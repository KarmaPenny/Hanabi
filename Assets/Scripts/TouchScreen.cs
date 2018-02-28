using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchScreen : MonoBehaviour {
	#if UNITY_ANDROID || UNITY_IOS
	public float longPressTime = 0.5f;

	public Dictionary<int, TouchInput> trackedTouches = new Dictionary<int, TouchInput> ();

	void Update () {
		RaycastHit hit = new RaycastHit();
		foreach (Touch touch in Input.touches) {
			if (touch.phase == TouchPhase.Began) {
				Ray ray = Camera.main.ScreenPointToRay (touch.position);
				if (Physics.Raycast (ray, out hit)) {
					TouchInput touchInput = hit.transform.GetComponent<TouchInput> ();
					if (touchInput != null) {
						if (!touchInput.isPressed) {
							touchInput.fingerId = touch.fingerId;
							touchInput.isPressed = true;
							touchInput.touchTime = Time.time;
							trackedTouches.Add (touch.fingerId, touchInput);
						}
					}
				}
			} else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
				if (trackedTouches.ContainsKey (touch.fingerId)) {
					TouchInput touchInput = trackedTouches [touch.fingerId];
					if (touchInput != null) {
						touchInput.isPressed = false;
						touchInput.wasPressed = touch.phase == TouchPhase.Ended;
						touchInput.fingerId = -1;
					}
					trackedTouches.Remove (touch.fingerId);
				}
			}
		}

		List<int> removeKeys = new List<int> ();
		foreach (KeyValuePair<int, TouchInput> trackedTouch in trackedTouches) {
			int fingerId = trackedTouch.Key;
			TouchInput touchInput = trackedTouch.Value;
			if (touchInput != null) {
				if (Time.time > touchInput.touchTime + longPressTime) {
					touchInput.isPressed = false;
					touchInput.wasLongPressed = true;
					touchInput.fingerId = -1;
					removeKeys.Add (fingerId);
				}
			} else {
				removeKeys.Add (fingerId);
			}
		}

		foreach (int fingerId in removeKeys) {
			trackedTouches.Remove (fingerId);
		}
	}
	#endif
}
