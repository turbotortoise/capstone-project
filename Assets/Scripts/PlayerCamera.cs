using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {
	private GameObject player;
	private Vector3 offset;

	void Start() {
		player = GameObject.Find ("ubee");
		offset = new Vector3(0, 2f, -10f);
	}

	void Move() {
		transform.position = player.transform.position + offset;
	}

	void Update() {
		Move();
	}
}