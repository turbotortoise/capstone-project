using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAction : MonoBehaviour {
	//camera info
	private Vector3				cameraPos;
	private float				angleOffset = 10f;
	//player information
	private float				playerX;
	private float				playerY;
	private float				playerZ;
	public Vector3				playerPos;
	private const float			rotateSpeed = 100f;
	private const float			runningSpeed = 3f;
	//for attacking
	public GameObject			weaponPrefab;
	private float				prevAttackTime;
	private float 				prevNoteColl;
	private float 				notCollDur = 1f;
	private bool				canAttack;
	public float				currPower;
	public Color				currColor;
	private int					numNotes;
	public float 				powerMultiplier = 1f;
	private List<GameObject>	noteList;
	private List<GameObject> 	removeList;
	//attacking information
	public float 				attackSep = 1f;
	private const float			noteForwardSpeed = 2f;
	private const float 		weakNote = 0.33f;
	private const float 		strongNote = 0.5f;
	private const float 		badNote = -0.15f;
	private const float 		noteSpeed = 5f;
	private const float 		noteScaleFactor = 1.05f;
	private const float 		noteTranspFactor = 1.05f;
	//for health
	public float				playerColor;
	private bool				isHit;
	private float				prevHitTime;
	private float 				playerDamage;
	private const float			hitTimeDur = 2f;
	//private AudioSource 		audio;
	private const float 		regen = 0.0002f;
	//for note information
	private AudioSource			noteSound1;
	private AudioSource			noteSound2;
	private GameObject			note1;
	private GameObject			note2;
	private float				noteAlpha = 20f;
	private float				noteCount;
	private NoteAction	 		noteScript1;
	private NoteAction			noteScript2;
	private Color 				noteColor1;
	private Color				noteColor2;
	private string				noteName1 = "";
	private string 				noteName2 = "";
	private Rigidbody  			rb;

	// Use this for initialization
	void Start () {
		//camera
		cameraPos = new Vector3(Camera.main.transform.position.x, 0, 0);
		//initializing hit/damage;
		isHit = false;
		prevHitTime	 = 0f;
		//initializing player rigidbody
		rb = GetComponent<Rigidbody> ();
		//initializing attack info
		canAttack = true;
		powerMultiplier = 1f;
		//initializing lists
		noteList = new List<GameObject> ();
		removeList = new List<GameObject> ();
		//initializing player pos/info
		this.transform.position = new Vector3 (0f, 0f, 0f);
		prevAttackTime = 0f;
		playerColor = 1f;
		playerDamage = 0f;
		Color c = new Color (0f, 0f, playerColor);
		this.GetComponent<Renderer> ().material.color = c;
	}

	private void CheckMovement(){
		//add in have directions (up right, up left, right up, right down, left up, left down, down right, down left)
		if (Input.GetKey ("up")) {
			//have player go forward
			rb.transform.Translate (Vector3.forward * runningSpeed * Time.deltaTime);
			if (Input.GetKey ("right")){
				rb.transform.Rotate (Vector3.up, rotateSpeed * Time.deltaTime);
			} else if (Input.GetKey ("left")){
				rb.transform.Rotate (Vector3.up, -rotateSpeed * Time.deltaTime);
			}
		}else if (Input.GetKey("right")){
			rb.transform.Rotate (Vector3.up, rotateSpeed * Time.deltaTime);
		} else if (Input.GetKey("left")){
			//have player face left
			rb.transform.Rotate (Vector3.up, -rotateSpeed * Time.deltaTime);
		} else if (Input.GetKey ("down")) {
			//have player face camera
				rb.transform.Rotate (Vector3.up, rotateSpeed * Time.deltaTime);
		}
	}

	private void CreateAttack(){
		if (Input.GetKey ("w")) {
			//this plays a note
			currPower = weakNote;
		} else {
			currPower = 0f;
		}
	}
	
	private void Attack(float power){
		if ((currPower != 0f)) {
			if (Time.time > prevAttackTime + attackSep) {
				prevAttackTime = Time.time;
				canAttack = true;
			} else {
				canAttack = false;
			}
			if (canAttack) {
				//plays both notes if both are occupied
				if (noteName1 != "" && noteName2 != ""){
					noteSound1.Play (); noteSound2.Play ();
					numNotes += 2;
					GameObject noteObj1 = Instantiate (weaponPrefab) as GameObject;
					GameObject noteObj2 = Instantiate (weaponPrefab) as GameObject;
					SphereCollider sphere1 = noteObj1.GetComponent<SphereCollider>();
					SphereCollider sphere2 = noteObj2.GetComponent<SphereCollider>();
					sphere1.isTrigger = true;
					sphere2.isTrigger = true;
					noteObj1.name = "Fire"; noteObj2.name = "Fire";
					noteObj1.GetComponent<Renderer> ().material.color = noteColor1;
					noteObj2.GetComponent<Renderer> ().material.color = noteColor2;
					noteObj1.transform.position = this.transform.position;
					noteObj2.transform.position = this.transform.position;
					noteList.Add (noteObj1); noteList.Add (noteObj2);
				} else {
					//don't have a second note, just play one
					noteSound1.Play ();
					numNotes++;
					GameObject noteObj = Instantiate(weaponPrefab) as GameObject;
					SphereCollider sphere = noteObj.GetComponent<SphereCollider>();
					sphere.isTrigger = true;
					noteObj.name = "Fire";
					noteObj.GetComponent<Renderer>().material.color = noteColor1;
					noteObj.transform.position = this.transform.position;
					noteList.Add (noteObj);
				}
			}
		}
		//Remove notes after they've gone far enough
		foreach (GameObject note in noteList) {
			//Makes the notes bigger
			noteCount++;
			float scale = note.transform.localScale.x;
			scale *= noteScaleFactor;
			note.transform.localScale = Vector3.one * scale;
			Vector3 temp = note.transform.position;
			if (noteCount % 2 == 0)
				temp.y += Time.deltaTime * noteForwardSpeed;
			else
				temp.y += Time.deltaTime * noteForwardSpeed/2;
			note.transform.position = temp;
			note.transform.Translate(this.transform.forward*Time.deltaTime*noteForwardSpeed);
			Color alphaColor = note.GetComponent<Renderer>().material.color;
			alphaColor.a -= noteAlpha * Time.deltaTime;
			//print ("alphacolor: " + alphaColor.a);
			note.GetComponent<Renderer> ().material.color = alphaColor;
			if (scale >= 10f) {
				removeList.Add (note);
			}
		}
		foreach (GameObject weapon in removeList) {
			noteList.Remove (weapon);
			Destroy (weapon);
		}
	}

	void CheckHealth(){
		//changes health of player
		if ((playerColor - playerDamage) >= 0f) {
			if (isHit) {
				if (Time.time - prevHitTime > hitTimeDur) {
					prevHitTime = Time.time;
					//get damage number from enemy
					playerDamage += 0.1f;
					Color c = new Color (playerDamage, playerDamage, playerColor - playerDamage);
					this.GetComponent<Renderer> ().material.color = c;
				} else if (Time.time - hitTimeDur > prevAttackTime) {
					isHit = false;
				}
			} else {
				//	//regenerate health after some time
				if (playerDamage > 0) {
					playerDamage -= regen;
				}
				Color c = new Color (playerDamage, playerDamage, playerColor - playerDamage);
				this.GetComponent<Renderer> ().material.color = c;
			}
		}
	}
	
	void OnTriggerEnter(Collider other){
		//check for collision
		if (other.gameObject.tag == "Note" && (prevNoteColl + notCollDur <= Time.time)) {
			prevNoteColl = Time.time;
			if (noteName1 != ""){
				//if we collide with another note save the old one
				noteName2 = noteName1;
				note2 = note1;
				noteColor2 = noteColor1;
				noteSound2 = noteSound1;
			}
			//saves new note
			noteName1 = other.gameObject.name;
			note1 = GameObject.Find (noteName1);
			noteColor1 = note1.GetComponent<NoteAction>().noteColor;
			noteSound1 = note1.GetComponent<NoteAction> ().audios;
		} else if (other.gameObject.name == "Attack"){
			//if we were attacked take damage
			isHit = true;
			//Destroy (gameObject);
		}
	}

	void Update() {
		CheckMovement ();
		CreateAttack();
		Attack (currPower);
		CheckHealth ();
	}
}