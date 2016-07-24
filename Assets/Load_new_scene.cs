using UnityEngine;
using System.Collections;

public class Load_new_scene : MonoBehaviour {

	private Player1 playerScript;

	void Start () 
	{
		playerScript = GameObject.Find ("ubee").GetComponent<Player1>();
	}

	void Update() {

		if (playerScript.transform.position.z <= -23f) {
			//Application.LoadLevel ("Test_Scene_2");
			print ("load next scene\n");
		}
		
	}
}
