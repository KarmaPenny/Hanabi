using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSound : MonoBehaviour {
	public AudioClip[] sounds;

	public void PlaySound(int index) {
		AudioSource.PlayClipAtPoint (sounds [index], Vector3.zero, 1);
	}
}
