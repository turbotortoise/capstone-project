using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Note : MonoBehaviour {

	bool hitPlayer;
	public Color noteColor;
	public AudioClip note;
	public float power;
	float
		delay = 5f,
		noteSpeed = 5f,
		radius = 3f,
		playerDistance;
	Vector3 origPos;

	Player player;

	IEnumerator Start() {
		origPos = transform.position;
		tag = "Note";
		player = GameObject.Find("Player").GetComponent<Player>();
		GetComponent<Renderer>().material.color = noteColor;
		GetComponent<AudioSource>().clip = note;
		var audio = GetComponent<AudioSource>();
		audio.loop = false;
		while (true) {
			yield return new WaitForSeconds(delay);
			audio.Play();
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.GetComponent<Rigidbody>()
		&& other.GetComponent<Rigidbody>().tag!="Player") return;
		player.AddNote(this);
		hitPlayer = true;

	}

	void ApproachPlayer() {
		playerDistance = Vector3.Distance(
			player.transform.position,
			transform.position);
		float step = noteSpeed * Time.deltaTime;
		if (playerDistance <= radius || hitPlayer) {
			transform.position = Vector3.MoveTowards(transform.position,
				player.transform.position, step);
		} else {
			transform.position = Vector3.MoveTowards(transform.position,
				origPos, step);
		}
	}

	void FixedUpdate() {
		ApproachPlayer();
	}
}
