using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {
	GameObject player;
	Vector3 offset;

	void Start() {
		player = GameObject.Find("Player");
		offset = new Vector3(0,2f,-10f);
	}

	void Move() {
		transform.position = player.transform.position + offset;
	}

	void FixedUpdate() { Move(); }
}
