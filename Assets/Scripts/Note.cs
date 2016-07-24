using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Note : MonoBehaviour {
	//note info
	public Color		noteColor;
	public GameObject	notePrefab;
	private float		noteSpeed = 2f;
	public float		power;
	public SphereCollider sphere;
	//trigger for player within a certain radius
	private float		noteRadius = 3f;
	public Vector3		origPos;
	//public AudioSource	audios;
	//player info
	private GameObject	player;
	private float		playerDistance;

	void Start() {
		this.transform.position = origPos;
		this.tag = "Note";
		player = GameObject.Find ("ubee");
		this.GetComponent<Renderer> ().material.color = noteColor;
	}

	void ApproachPlayer() {
		playerDistance = Vector3.Distance (player.transform.position, 
			this.transform.position);
		float step = noteSpeed * Time.deltaTime;
		//print ("playerDistance: " + playerDistance + "\nnoteRadius: " + noteRadius);//"\nplayerDistance <= noteRadius: " + (playerDistance <= noteRadius));
		if (playerDistance <= noteRadius) {
			this.transform.position = Vector3.MoveTowards(this.transform.position,
				player.transform.position, step);
		} else {
			this.transform.position = Vector3.MoveTowards (this.transform.position,
				origPos, step);
		}
	}

	void Update() {
		ApproachPlayer();
	}
}