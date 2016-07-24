using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CivilianAction : MonoBehaviour {
	//player information
	private Color 			playerColor;
	private float			playerPower;
	private GameObject		player;
	private PlayerAction 	playerScript;
	//checking within certain radius
	private float			civilianX;
	private float			civilianY;
	private float			civilianZ;
	private float			distance;
	private float 			triggerRadius;
	public Vector3 			civilianPos;
	private const float 	maxDist = 30f;
	private const float 	minDist = -30f;
	//civilian transformation/color info
	private bool			isHit;
	private bool			isStable;
	private Color			civilianColor;
	private Color			dormantColor;
	private Color			tempColor;
	private float			changeProbability;
	private float			civilianDamage;
	private float			colorPercentage;
	private float 			hitPauseDur;
	private float			prevHitTime;
	private const float 	colorTrnsfmSpeed = 0.08f;
	private const float 	changeMin = 0.7f;
	private const float 	transformSpeed = 0.008f;
	//not stable information
	private bool 			isStopped = true;
	private float 			prevStartWalkTime = 0f;
	private float			movingTime;
	private float			stopTime;
	private const float		minStopTime = 6f;
	private const float 	maxStopTime = 8f;
	private float 			attackProbability;
	//enemy weapon
	public GameObject			weaponPrefab;
	private List<GameObject>	weaponList;
	private int					numWeapons;
	private float				weaponScaleFactor = 1.05f;
	private List<GameObject> 	removeList;
	private bool 				isAttack;


	// Use this for initialization
	void Start () {
		//getting player information
		player = GameObject.Find ("Player");
		playerScript = player.GetComponent<PlayerAction> ();
		//initializing civilian info
		changeProbability = Random.Range (0f, 1f);
		civilianColor = new Color (1f, 1f, 1f);
		civilianDamage = 0f;
		colorPercentage = 0f;
		hitPauseDur = playerScript.attackSep;
		isHit = false;
		isStable = true;
		triggerRadius = Random.Range (5f, 25f);
		prevHitTime = 0f;
		this.GetComponent<Renderer> ().material.color = civilianColor;
		//putting civilian somewhere...
		this.transform.position = new Vector3 (Random.Range (minDist, maxDist), 0f, 
		                                      Random.Range (minDist, maxDist));
	}

	void CheckingPosition(){
		distance = Vector3.Distance (player.transform.position, this.transform.position);
		if ((distance <= triggerRadius)&& (changeProbability >= changeMin)) {
			isStable = false;
		}
	}

	void Transformation(){
		//keeps track of civilian color
		if ((!isHit) && (!isStable)) {
			//civilian will start tranforming
			if (civilianDamage < 1){
				//stops transforming after a while
				civilianDamage += transformSpeed;
			}
		} else if ((isHit)&&(!isStable)){
			//if civilian is hit change to a color
			prevHitTime = Time.time;
			civilianDamage -= ((playerPower/2)/(hitPauseDur+Time.time));
			if (civilianDamage <=0.1){
				//if we hit it above a certain threshold, it can't change again
				isStable = true;
				changeProbability = 0f;
				isHit = false;
			}
		} if (Time.time - hitPauseDur > prevHitTime) {
			isHit = false;
			//civilian can start transforming again
		} 
		tempColor = new Color	(civilianColor[0] - civilianDamage,
			                     civilianColor[1] - civilianDamage,
			                     civilianColor[2] - civilianDamage);
		this.GetComponent<Renderer> ().material.color = tempColor;
	}

	void CheckColor(){
		if (isHit){
			if (colorPercentage < 1){
				colorPercentage += 0.1f*colorTrnsfmSpeed;
			}
			tempColor = new Color 	(civilianColor[0] - (dormantColor[0]*colorPercentage),
			             			civilianColor[1] - (dormantColor[1]*colorPercentage),
			                        civilianColor[2] - (dormantColor[2]*colorPercentage));
			this.GetComponent<Renderer>().material.color = tempColor;
		}
	}

	void OnTriggerEnter(Collider other){
		//check for collision
		if (other.gameObject.name == "Fire") {
			//if the player attacked take damage
			playerPower = player.GetComponent<PlayerAction> ().currPower;
			playerColor = other.gameObject.GetComponent<Renderer>().material.color;
			isHit = true;
			//turns color of note it's hit with
			tempColor = new Color (0f, 0f, 0f, 0f);
			if (dormantColor == tempColor){
				dormantColor = new Color (1-playerColor[0], 1-playerColor[1], 1-playerColor[2]);
			}
		}
	}

	//not stable
	void Walk (){
		Vector3 relativePos = player.transform.position - transform.position;
		transform.rotation = Quaternion.LookRotation(relativePos);
		this.transform.Translate (Vector3.forward * Time.deltaTime);
		/*if (isStopped) {
			if (Time.time - prevStartWalkTime > stopTime) {
				stopTime = Random.Range (minStopTime, maxStopTime);
				prevStartWalkTime = Time.time;
				isStopped = false;
			} else {
				Attack ();
			}
		} else {
			if (Time.time - prevStartWalkTime > movingTime){
				movingTime = Random.Range (minStopTime, maxStopTime);
				prevStartWalkTime = Time.time;
				isStopped = true;
				//don't move
			} else{
				//move
				this.transform.Translate (Vector3.forward * Time.deltaTime);
			}
		}*/
	}

	void Attack(){
		attackProbability = Random.Range (0.0f, 1.0f);
		if (attackProbability >= 0.98f) {
			isAttack = true;
		} else {
			isAttack = false;
		}
		if (isAttack) {
			Weapon();
		}
	}
	
	void Weapon(){
		if (isAttack) {
			numWeapons++;//Add to number of notes coming form enemy
			GameObject wpnObj = Instantiate (weaponPrefab) as GameObject;
			wpnObj.name = "Attack";
			Color c = new Color (255f, 0f, 0f);
			wpnObj.GetComponent<Renderer> ().material.color = c;
			wpnObj.transform.position = this.transform.position;
			weaponList.Add (wpnObj);
		}
	}
	
	void RemoveWeapon(){
		//Remove notes after they've gone far enough
		foreach (GameObject weapon in weaponList) {
			//Makes the notes bigger
			float scale = weapon.transform.localScale.x;
			scale *= weaponScaleFactor;
			weapon.transform.localScale = Vector3.one * scale;
			if (scale >= 10f) {
				removeList.Add (weapon);
			}
		}
		foreach (GameObject weapon in removeList) {
			weaponList.Remove (weapon);
			Destroy (weapon);
		}
	}


	// Update is called once per frame
	void Update () {
		CheckingPosition ();
		if (isStable) {
			CheckColor ();
		} else {
			Walk();
			Transformation ();
		}
	}
}