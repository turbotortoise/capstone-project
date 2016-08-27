using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Player : MonoBehaviour {
	bool wait, inAir, isStable = true;
	float delay = 1f;

#pragma warning disable 0108
    Rigidbody rigidbody;
#pragma warning restore 0108

	float
		blueComponent = 1f,
		rotateSpeed = 200f,
		walkSpeed = 10f,
		jumpHeight = 50f,
		jumpMult = 1.1f;

	public float power = 1f;

	Color playerColor, noteColor;
	public Element element = Element.Water;

	Dictionary<Element,List<GameObject>> attacks =
		new Dictionary<Element,List<GameObject>>();

	Dictionary<Element,int> powerLevels = new Dictionary<Element,int> {
		{ Element.Water, 0 },
		{ Element.Plant, 0 },
		{ Element.Earth, 0 },
		{ Element.Air, 0 }};

	[SerializeField] List<GameObject> waterAttacks = new List<GameObject>();
	[SerializeField] List<GameObject> plantAttacks = new List<GameObject>();
	[SerializeField] List<GameObject> earthAttacks = new List<GameObject>();
	[SerializeField] List<GameObject> airAttacks = new List<GameObject>();

	private float noteSpeed = 3f;
	List<NoteAttack> noteList = new List<NoteAttack>();
	private float powerMult; //for upgrades
	private float defenseMult; //for upgrades
	//for being attacked
	private float damageTime;
	//private float 		damageDur = 1f;
	private float health = 1f;
	private float prevHitTime;
	private float regenSpeed = 0.03f;
	private float regenMult; //for upgrades
	private float hitDur = 1f;
	private float paralyzeDur = 0.8f;
	//private bool		isParalyzed = false; //if hit with long range attack
	//for communicating
	public bool canTalk = false;

	void Awake() {
		rigidbody = GetComponent<Rigidbody>();
		powerLevels[Element.Water] = 0;
		powerLevels[Element.Plant] = 0;
		powerLevels[Element.Earth] = 0;
		powerLevels[Element.Air] = 0;
		attacks[Element.Water] = new List<GameObject>();
		attacks[Element.Plant] = new List<GameObject>();
		attacks[Element.Earth] = new List<GameObject>();
		attacks[Element.Air] = new List<GameObject>();
		foreach (var attack in waterAttacks) attacks[Element.Water].Add(attack);
		foreach (var attack in plantAttacks) attacks[Element.Plant].Add(attack);
		foreach (var attack in earthAttacks) attacks[Element.Earth].Add(attack);
		foreach (var attack in airAttacks) attacks[Element.Air].Add(attack);
	}


	void Start() {
		playerColor = new Color(0,0,blueComponent,1);
		GetComponent<Renderer>().material.color = new Color (0, 0, 1, 1);
	}

	public void AddNote(Note note) {
		powerLevels[note.element]++;
		element = note.element;
	}

	void Move() {
		if (Input.GetKey("a")) Attack();
		if (Input.GetKey("up")) {
			rigidbody.MovePosition(transform.position +
				transform.forward * Time.deltaTime * walkSpeed);
		} if (Input.GetKey("right")) {
			rigidbody.transform.Rotate (Vector3.up, rotateSpeed * Time.deltaTime);
		} if (Input.GetKey ("left")) {
			rigidbody.transform.Rotate (Vector3.up, -rotateSpeed * Time.deltaTime);
		} if (Input.GetKey ("down")) {
			//need to check if closest to right or left
			//rigidbody.transform.Rotate (Vector3.up, rotateSpeed * Time.deltaTime);
			rigidbody.MovePosition (transform.position -
				transform.forward * Time.deltaTime * (0.5f * walkSpeed));
		} if (Input.GetKey ("space")) {
			if ((!inAir) && (!canTalk)) {
				inAir = true;
				rigidbody.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
			}
		}
		/*
		if (Input.GetKey("up") && (!waterAttack) && (waterList.Count != 0)) {
			//must have at least 1 note in list to use attack
			waterAttack = True;
			plantAttack = False;
			waterAttack = False;
			earthAttack = False;
			airAttack = False;
		}
		else if (Input.GetKey("right") && (!plantAttack) && (plantList.Count != 0)) {
			plantAttack = True;
			waterAttack = False;
			earthAttack = False;
			airAttack = False;

		}
		else if (Input.GetKey("down") && (!earthAttack) && (earthList.Count != 0)) {
			earthAttack = True;
			waterAttack = False;
			plantAttack = False;
			airAttack = False;
		}
		else if (Input.GetKey("left") && (!airAttack) && (airList.Count != 0)) {
			airAttack = True;
			waterAttack = False;
			plantAttack = False;
			earthAttack = False;
		}
		*/
	}

	public void ApplyDamage(float damage) {
		health -= damage;
		if (damage<0) isStable = false;
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Note") {
			print("Collided with note\n");
			/*if (other.gameObject.name == "Water") {
				waterList.Add(other.gameObject);
			}
			else if (other.gameObject.name == "Plant") {
				plantList.Add(other.gameObject);
			}
			else if (other.gameObject.name == "Earth") {
				earthList.Add(other.gameObject);
			}
			else if (other.gameObject.name == "Air") {
				airList.Add(other.gameObject);
			}
			*/
		}
	}

	void OnCollisionEnter(Collision other) {
	    if (other.gameObject.tag == "Ground" && inAir==true)
	    	inAir = false;
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
		if ((blueComponent - health) >= 0f) {
			if (isStable) {
				if (blueComponent < 1) {
					//regenerate health
					blueComponent += regenSpeed * Time.deltaTime;
					playerColor = new Color (0, 0, blueComponent, 1);
					GetComponent<Renderer>().material.color = playerColor;
				}
			}
			else {
				if (prevHitTime + hitDur <= Time.time) {
					//take damage
					prevHitTime = Time.time;
					blueComponent = blueComponent - health;
					print ("current health: " + blueComponent);
					playerColor = new Color (0, 0, blueComponent, 1);
					GetComponent<Renderer>().material.color = playerColor;
					health = 0f;
				}
			}
		}
		else if (blueComponent - health < 0f) {
			//player is ded
			print ("Player health depleted");
			//ded animation
			print ("load game over scene");
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

	void Attack() { if (!wait) StartCoroutine(Attacking()); }


	IEnumerator Attacking() {
		wait = true;
		print("attacking");
		var prefab = Instantiate(attacks[element][powerLevels[element]]) as GameObject;
		var attack = prefab.GetComponent<NoteAttack>();
		attack.transform.position = transform.position+transform.forward;
		//attack.transform.rotation = Quaternion.identity*transform.forward;
		attack.GetComponent<Rigidbody>().AddForce(
            (transform.forward)*20, ForceMode.Impulse);
		noteList.Add(attack);

		var removeList = new List<NoteAttack>();
		foreach (var note in noteList)
			if (note.color.a <= 0f) removeList.Add(note);
		foreach (var note in removeList) {
			noteList.Remove(note);
			Destroy(note);
		}
		yield return new WaitForSeconds(delay);
		wait = false;
	}

	void FixedUpdate() {
		if (isStable) Move();
		ChangeColor();
		//Attack();
	}

}
