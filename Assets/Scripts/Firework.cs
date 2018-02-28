using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firework : MonoBehaviour {
	public Color color;
	public Transform explosionPrefab;
	public AudioClip[] explosionSounds;
	public AudioClip[] launchSounds;
	public float fuse;

	public float minSpeed;
	public float maxSpeed;

	float explodeTime;
	bool exploded = false;

	void Start () {
		explodeTime = Time.time + fuse;
		float speed = Random.Range (minSpeed, maxSpeed);
		Vector3 dir = (new Vector3 (Random.Range (-0.5f, 0.5f), 1, 0)).normalized;
		GetComponent<Rigidbody> ().velocity = speed * dir;
		StartCoroutine (PlayLaunchSound ());
	}

	IEnumerator PlayLaunchSound() {
		yield return new WaitForSeconds (fuse / 4);
		AudioSource.PlayClipAtPoint (launchSounds [Random.Range (0, launchSounds.Length)], Vector3.zero, 0.05f);
	}

	void Update () {
		if (Time.time > explodeTime && !exploded) {
			exploded = true;

			Destroy (GetComponent<Rigidbody> ());

			// stop rocket smoke
			GetComponent<ParticleSystem> ().Stop (false, ParticleSystemStopBehavior.StopEmitting);

			// spawn an explosion
			Transform explosionTransform = Instantiate<Transform> (explosionPrefab, transform.position, Quaternion.identity, transform);
			ParticleSystem.MainModule main = explosionTransform.GetComponent<ParticleSystem> ().main;
			main.startColor = color;
			ParticleSystem.MinMaxCurve startSpeed = main.startSpeed;
			startSpeed.constantMax = Random.Range (30f, 50f);
			main.startSpeed = startSpeed;

			// play explosion sound
			AudioSource.PlayClipAtPoint(explosionSounds[Random.Range(0, explosionSounds.Length)], Vector3.zero, 1);

			// add to score
			FireworksShow.totalScore++;

			Destroy (gameObject, 10);
		}
	}
}
