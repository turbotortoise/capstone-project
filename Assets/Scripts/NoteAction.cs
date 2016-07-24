using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NoteAction : MonoBehaviour {
	//note info
	public GameObject			notePrefab;
	private int					currIndex;
	private int					numOfNotes;
	public Color				noteColor;
	private float				noteTpcy;
	private float				tpcyTime;
	private float				prevSoundTime;
	private bool				isHit;
	private const float			tpcyTimeDur = 10f;
	private const float			tpcyFactor = 0.1f;
	//note movement/position
	private Vector3				notePos;
	private const float 		noteRadius = 3f;
	private const float 		noteSpeed = 0.5f;
	private const float 		minDist = -5f;
	private const float 		maxDist = 5f;
	//note lists
	private List<Color> 		noteColorList;
	public List<Vector3>		notePosList;
	//player information
	private GameObject			player;
	//finding informaiton between player and notes
	private AudioClip			noteA;
	private AudioClip			noteAs;
	private float				playerDistance;
	//audios
	public AudioSource 			audios;
	private float 				soundfreq;

	// Use this for initialization
	void Start () {
		//initializing lists
		noteColorList = new List<Color> ();
		notePosList = new List<Vector3> ();
		//finding player
		player = GameObject.Find ("Player");
		//assigning elements to notes
		noteTpcy = 0f;
		tpcyTime = 0f;
		isHit = false;
		CreateColorList ();
		CreatePosList ();
		AssignNotes ();
		//sound info
		audios = GetComponent<AudioSource> ();
		soundfreq = 2f * currIndex;
		prevSoundTime = 0f;
		//LoadSounds ();
		this.GetComponent<Renderer> ().material.color = noteColor;
		this.transform.position = notePos;
	}
	
	void CreateColorList(){
		//creating colors for all the notes
		Color orange = new Color (1f, .65f, 0f, 1f);
		Color goldenrod = new Color (1f, .84f, 0f, 1f);
		Color greenYellow = new Color (.68f, 1f, .18f, 1f);
		Color teal = new Color (.24f, .70f, .44f, 1f);
		Color bluePurple = new Color (.54f, .17f, .88f, 1f);
		Color purple = new Color (.63f, .13f, .94f, 1f);
		noteColorList.Add (Color.magenta);//A
		noteColorList.Add (Color.red);//Bb
		noteColorList.Add (orange);//B
		noteColorList.Add (goldenrod);//C
		noteColorList.Add (Color.yellow);//C#
		noteColorList.Add (greenYellow);//D
		noteColorList.Add (Color.green);//Eb
		noteColorList.Add (teal);//E
		noteColorList.Add (Color.blue);//F
		noteColorList.Add (bluePurple);//F#
		noteColorList.Add (purple);//G
		noteColorList.Add (Color.grey);//G#
	}

	void CreatePosList (){
		for (int i=0; i < 12; i++) {
			Vector3 aPos = new Vector3 (Random.Range (minDist, maxDist), 1.5f, Random.Range (minDist, maxDist));
			notePosList.Add (aPos);
		}
	}

	void AssignNotes(){
		for (int i=0; i<12; i++){
			if (this.name == "A"){
				currIndex = 0;
			} else if (this.name == "A#"){
				currIndex = 1;
			} else if (this.name == "B") {
				currIndex = 2;
			} else if (this.name == "C") {
				currIndex = 3;
			} else if (this.name == "C#") {
				currIndex = 4;
			} else if (this.name == "D"){
				currIndex = 5;
			} else if (this.name == "D#") {
				currIndex = 6;
			} else if (this.name == "E") {
				currIndex = 7;
			} else if (this.name == "F") {
				currIndex = 8;
			} else if (this.name == "F#") {
				currIndex = 9;
			} else if (this.name == "G") {
				currIndex = 10;
			} else if (this.name == "G#") {
				currIndex = 11;
			}
		}
		noteColor = noteColorList [currIndex];
		notePos = notePosList [currIndex];
		//currSound = audios[currIndex];
	}


	//if the player is close to the note, move towards the player
	void MoveTowardsPlayer(){
		playerDistance = Vector3.Distance (player.transform.position, 
			                                   this.transform.position);
		float step = noteSpeed * Time.deltaTime;
		if ((playerDistance <= noteRadius) && (!isHit)){
		this.transform.position = Vector3.MoveTowards(this.transform.position,
		                                             player.transform.position, step);
		} else {
			//if the player is away from the note go back to original position
			this.transform.position = Vector3.MoveTowards (this.transform.position,
				                                              notePos, step);
			}
		}

	//if the player hit the note, go away for a bit
	void CheckTransparency(){
		if (isHit){
			if (tpcyTime + tpcyTimeDur <= Time.time) {
			//if there's a collision, make the note invisible
				tpcyTime = Time.time;
				noteTpcy = 0f;
				this.transform.position = notePos;
			} else if (Time.time < tpcyTime + tpcyTimeDur) {
				//make the note more visible
				noteTpcy += tpcyFactor;
				Color tempColor = new Color (noteColor [0], noteColor [1], 
				                             noteColor [2], noteTpcy);
				this.GetComponent<Renderer> ().material.color = tempColor;
				if (noteTpcy >= 1){
					//stop making the note more transparent
					noteTpcy = 1f;
					isHit = false;
					this.GetComponent<Renderer>().material.color = noteColor;
				}
			}
		}
	}


	//plays note sounds
	void PlaySounds(){
		if (prevSoundTime + soundfreq <= Time.time) {
		//if it's been long enough play the note sound
			prevSoundTime = Time.time;
			audios.Play ();
		}
	}

	//if note collides with player
	void OnTriggerEnter(Collider coll){
		if (coll.gameObject.name.Equals ("Player")) {
			isHit = true;
		}
	}

	// Update is called once per frame
	void Update () {
		MoveTowardsPlayer();
		CheckTransparency ();
		//PlaySounds ();
	}
}
