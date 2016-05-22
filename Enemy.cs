using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {
	//for stability
	public Rigidbody rb;
	public Vector3	origPos;
	public bool		isStable = true;
	private bool	isHit = false;
	private bool	hasTransformed = false;
	private bool	isTransforming = false;
	//for moving
	private float	runRadius = 10f;
	private float	walkRadius = 5f;
	private float	runSpeed = 2.5f;
	private float	walkSpeed = 1.5f;
	//for health/color
	private float	health = 1f;
	private float	regenSpeed;
	private float	transformSpeed = 0.1f;
	private Color	dormantColor;
	private float	dormantPerc = 0;
	//for player
	private GameObject 	player;
	private float		playerDistance; //distance player currently is
	private float		playerPower = 0f; //power of player's attacks
	//for attacking
	public Weapon 	weaponPrefab;
	private float		laTime = 2.8f; //long ranged attack time
	private float		saTime = 1f; //short ranged attack time
	private float		waitTime = 0f; //uses last attack wait time (recharge)
	private float		prevAttTime = 0f;
	private bool		isAttacking = false;
	private float		addTime; //if the enemy doesn't feel like attacking
	private float		srPower = 0.2f;
	private float 		lrPower = 0.4f;
	public float		attPower = 0f;
	private Vector3		tempPosition;
	private List<Weapon>	attackList;
	private List<Weapon> 	removeList;

void Start() {
	this.transform.position = origPos;
	this.tag = "Enemy";
	player = GameObject.Find("ubee");
	attackList = new List<Weapon> ();
	removeList = new List<Weapon> ();
}

void ApproachPlayer() {
	float curSpeed = 0f;
	if (!isStable) {
		if (playerDistance <= walkRadius) {
			curSpeed = walkSpeed;
		}
		else if (playerDistance <= runRadius) {
			curSpeed = runSpeed;
		}
		if ((curSpeed != 0) && (!hasTransformed)) {
			hasTransformed = true;
			isTransforming = true;
		}
		//for rotating
		Vector3 relativePos = player.transform.position - transform.position;
		rb.transform.rotation = Quaternion.LookRotation(relativePos);
		rb.MovePosition (transform.position + transform.forward * (
			curSpeed * Time.deltaTime));
	}
	return;
}

void Transform() {
	//changes color of civilian to show health
	if (!isStable) {
		if (isHit) {
			isHit = false;
			health += playerPower;
			//do isHit animation yay
		}
		if (isTransforming) {
			health -= transformSpeed * Time.deltaTime;
			if (health <= 0f) {
				isTransforming = false;
			}
		}
		if (health >= 1f) {
			isTransforming = false;
			isStable = true;
			//do transformation animation
		}
		this.GetComponent<Renderer>().material.color = new Color (health, 
			health, health);
	}
	else {
		if (isHit) {
			if (dormantPerc < 1) {
				dormantPerc += transformSpeed * Time.deltaTime;
				Color tempColor = new Color (1f - (dormantColor.r * dormantPerc),
					1f - (dormantColor.g * dormantPerc), 
					1f - (dormantColor.b * dormantPerc));
				this.GetComponent<Renderer>().material.color = tempColor;
			}
		}
	}
	return;
}

/*void Attack() {
	if (playerDistance <= walkRadius) {
		addTime = Random.Range(0.0f, 2.0f);
		if (prevAttTime + waitTime + addTime <= Time.time) {
			prevAttTime = Time.time;
			isAttacking = true;
			addTime = Mathf.Floor(addTime);
			Weapon newAttack = Instantiate<Weapon>(weaponPrefab);
			if (addTime == 0.0) {
				newAttack.name = "Short";
				newAttack.GetComponent<Weapon>().power = srPower;
				waitTime = saTime;
			}
			else {
				print("in long attack");
				newAttack.name = "Long";
				newAttack.GetComponent<Weapon>().power = lrPower;
				waitTime = laTime;
			}
			newAttack.tag = "Attack";
			attackList.Add(newAttack);
			tempPosition = player.transform.position; //finds player
		}
		else if (isAttacking && (prevAttTime + waitTime <= Time.time)) {
			print("attacking");
			prevAttTime = Time.time;
			Weapon newAttack = attackList[0];
			newAttack.transform.position = tempPosition;
			isAttacking = false;
			attackList.Remove()
		}
	}
	return;
}*/

void OnTriggerEnter(Collider other) {
	if (!isTransforming) {
		//can't be hit during initial transformation (need to think about more)
		if (other.gameObject.name == "Note") {
			//if the player attacked take damage
			isHit = true;
			playerPower = player.GetComponent<Player1>().notePower;
			dormantColor = other.gameObject.GetComponent<Renderer>().material.color;
			dormantColor.r = 1f - dormantColor.r;
			dormantColor.g = 1f - dormantColor.g;
			dormantColor.b = 1f - dormantColor.b;
		}
	}
	return;
}

void Update() {
	playerDistance = Vector3.Distance (player.transform.position, 
			this.transform.position); //always check where the player is
	if (!isHit && !isAttacking) {
		ApproachPlayer();
	}
	if (isTransforming || isHit) {
		Transform();
	}
	if (isStable) {
		if (isHit) {
			Transform();
		}
	}
	else if (!isStable) {
	//	Attack();
	}
}

}