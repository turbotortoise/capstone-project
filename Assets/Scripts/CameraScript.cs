using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {
	//player information
	private GameObject		player;
	private float			rotateSpeed = 50f;
	private float			turnSpeed = 4.0f;
	private Vector3 		offset;

	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Player");
		offset = new Vector3(0, 2f, -10f);
	}

	void CheckMovement(){
		transform.position = player.transform.position + offset;
	}

	// Update is called once per frame
	void Update () {
		CheckMovement ();
	
	}
}
