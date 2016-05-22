using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player1 : MonoBehaviour {
	//for rigidbody
	//public Vector3 		eulerAngleVel;
	public Rigidbody 	rb;
	private float 		blueComponent = 1f;
	private float 		rotateSpeed = 200f;
	private float 		walkSpeed = 0.02f;
	private float		jumpHeight = 100f;
	private float		jumpMult; //for upgrades
	private bool		inAir = false;
	private Color 		playerColor;
	//for notes
	public float		notePower = 0.5f; //change depending on combinations
	private float		noteCollisionTime;
	private float		noteCollisionDur;
	private string		noteName1 = "";
	private string		noteName2 = "";
	private GameObject 	notePrefab1;
	private GameObject 	notePrefab2;
	private Color 		note1Color;
	private Color 		note2Color;
	//private AudioSource	noteSound1;
	//private AudioSource	noteSound2;
	//for attacking
	public NoteWeapon 	notePrefab;
	private NoteWeapon	note1;
	private NoteWeapon	note2;
	//public GameObject 	weaponPrefab;
	private float		prevAttTime;
	private float		attDuration = 1f;
	private float		noteSpeed = 3f;
	private List<NoteWeapon>	noteList;
	private List<NoteWeapon>	removeList;
	private float		powerMult; //for upgrades
	private float		defenseMult; //for upgrades
	//for being attacked
	private float 		damageTime;
	//private float 		damageDur = 1f;
	private float		damageTaken = 0f;
	private float		prevHitTime;
	private float		regenSpeed = 0.03f;
	private float		regenMult; //for upgrades
	private float		hitDur = 1f;
	private float		paralyzeDur = 0.8f;
	private bool		isStable = true;
	//private bool		isParalyzed = false; //if hit with long range attack


void Start () {
	rb = GetComponent<Rigidbody>();
	playerColor = new Color (0, 0, blueComponent, 1);
	noteList = new List<NoteWeapon> ();
	removeList = new List<NoteWeapon> ();
	this.GetComponent<Renderer> ().material.color = new Color (0, 0, 1, 1);
}

private void Move() {
	if (Input.GetKey ("up")) {
		rb.MovePosition (transform.position + transform.forward * 0.2f);
	} if (Input.GetKey ("right")) {
		rb.transform.Rotate (Vector3.up, rotateSpeed * Time.deltaTime);
	} if (Input.GetKey ("left")) {
		rb.transform.Rotate (Vector3.up, -rotateSpeed * Time.deltaTime);
	} if (Input.GetKey ("down")) {
		//need to check if closest to right or left
		rb.transform.Rotate (Vector3.up, rotateSpeed * Time.deltaTime);
	} if (Input.GetKey ("space") && (!inAir)) {
		inAir = true;
		rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
	}
}

void OnTriggerEnter(Collider other) {
	//checks for collisions
	print ("Collided with " + other.gameObject.tag);
	if ((other.gameObject.tag == "Note") && 
		(noteCollisionTime + noteCollisionDur <= Time.time)) {
		print ("Collided with note\n");
		noteCollisionTime = Time.time;
		if (noteName1 != "") {
			//if we collide with a new note we save the note we originally had
			notePrefab2 = notePrefab1;
			noteName2 = "note 2";
			note2Color = note1Color;
			//noteSound2 = noteSound1;
		}
		//saves note we just collided with
		notePrefab1 = other.gameObject;
		noteName1 = "note 1";
		note1Color = other.gameObject.GetComponent<Note>().noteColor;
		//noteSound1 = notePrefab1.GetComponent<Note>().audios;
	} else if (other.gameObject.name == "Enemy") {
		//collided with an enemy
	} else if (other.gameObject.tag == "Attack") {
		//take damage
		print ("Took Damage");
		isStable = false;
		damageTaken = other.gameObject.GetComponent<Weapon>().power;
	}
}

void OnCollisionEnter(Collision other) {
    if (other.gameObject.tag == "Ground" && inAir==true) {
        inAir = false;
    }
}
 
void OnCollisionStay(Collision other) {
    if (other.gameObject.tag == "Ground" && inAir == true) {
        inAir = false;
    }
}
 
void OnCollisionExit(Collision other) {
    inAir = true;
}

void ChangeColor() {
	//changes player's color based on health
	if ((blueComponent - damageTaken) >= 0f) {
		if (isStable) {
			if (blueComponent < 1) {
				//regenerate health
				blueComponent += regenSpeed * Time.deltaTime;
				playerColor = new Color (0, 0, blueComponent, 1);
				this.GetComponent<Renderer> ().material.color = playerColor;
			}
		}
		else {
			if (prevHitTime + hitDur <= Time.time) {
				//take damage
				prevHitTime = Time.time;
				blueComponent = blueComponent - damageTaken;
				print ("current health: " + blueComponent);
				playerColor = new Color (0, 0, blueComponent, 1);
				this.GetComponent<Renderer> ().material.color = playerColor;
				damageTaken = 0f;
			}
		}
	}
	if ((!isStable) && (prevHitTime + paralyzeDur <= Time.time)) isStable = true;
}

/*GameObject InstantiateWeapon(int i) {
	GameObject note = Instantiate (weaponPrefab) as GameObject;
	SphereCollider sphere = note.GetComponent<SphereCollider>();
	sphere.isTrigger = true;
	note.name = "Note";
	if (i == 1) {
		note.GetComponent<Renderer> ().material.color = note1Color;
		//noteSound1.Play();
	}
	else {
		note.GetComponent<Renderer> ().material.color = note2Color;
		//noteSound2.Play();
	}
	note.transform.position = this.transform.position;
	note.transform.forward = this.transform.forward;
	noteList.Add(note);
	return note;
}*/

void Attack() {
	if (Input.GetKey("a") && (prevAttTime + attDuration <= Time.time)) {
		print ("attacking");
		prevAttTime = Time.time;
		if (noteName1 != "") {
			//attacks using first note
			NoteWeapon note1 = Instantiate<NoteWeapon>(notePrefab);
			noteList.Add(note1);
		} if (noteName2 != "") {
			//attacks using second note
			NoteWeapon note2 = Instantiate<NoteWeapon>(notePrefab);
			noteList.Add(note2);
		}
	}
	foreach (var note in noteList) {
		//moves all notes and changes their color
		note.transform.position += note.transform.forward * Time.deltaTime * noteSpeed;
		Color noteColor = note.GetComponent<Renderer>().material.color;
		note.GetComponent<Renderer> ().material.color = noteColor;
		noteColor.a -= 0.1f * Time.deltaTime;
		note.GetComponent<Renderer> ().material.color = noteColor;
		if (noteColor.a <= 0f) {
			removeList.Add (note);
		}
	}
	foreach (var note in removeList) {
		noteList.Remove (note);
		Destroy (note);
	}
}

void Update() {
	if (isStable) Move();
	ChangeColor();
	//Attack();
}

void FixedUpdate() {
	//Move();
}

}