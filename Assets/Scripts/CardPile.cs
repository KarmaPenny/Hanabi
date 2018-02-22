using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CardPile : NetworkBehaviour {
	public Vector3 minOffset;
	public Vector3 maxOffset;
	public float minRotation;
	public float maxRotation;
	public bool resetRotation = false;
	public float xRotation = 0;
	public bool resetX = false;
	public bool resetY = false;
	public bool resetZ = false;
	public int maxSize;
	public GameObject slotPrefab;

	protected virtual void Start () {
		Vector3 lastPosition = Vector3.zero;
		float lastAngle = 0;
		for (int i = 0; i < maxSize; i++) {
			float x = Random.Range (minOffset.x, maxOffset.x);
			float y = Random.Range (minOffset.y, maxOffset.y);
			float z = Random.Range (minOffset.z, maxOffset.z);
			lastPosition += new Vector3 (x, y, z);
			lastAngle += Random.Range (minRotation, maxRotation);
			GameObject slot = (slotPrefab == null) ? new GameObject ("slot" + i) : Instantiate<GameObject> (slotPrefab);
			slot.transform.SetParent(transform);
			slot.transform.localPosition = lastPosition;
			slot.transform.localRotation = Quaternion.Euler (xRotation, 0, lastAngle);

			if (resetRotation) {
				lastAngle = 0;
			}

			x = (resetX) ? 0 : lastPosition.x;
			y = (resetY) ? 0 : lastPosition.y;
			z = (resetZ) ? 0 : lastPosition.z;
			lastPosition = new Vector3 (x, y, z);
		}
	}

	public List<Card> Cards {
		get {
			List<Card> cards = new List<Card> ();
			for (int i = 0; i < transform.childCount; i++) {
				Transform slot = transform.GetChild (i);
				if (slot.childCount == 0) {
					break;
				}
				cards.Add (slot.GetComponentInChildren<Card> ());
			}
			return cards;
		}
	}

	public int Count {
		get {
			return Cards.Count;
		}
	}
}
