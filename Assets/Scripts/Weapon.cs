using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : MonoBehaviour {

	public GameObject weaponPrefab;
	public float power;
	public Collider collider;
	float waitTime = 1f;

	void Start() {
		var weaponInstance = Instantiate(weaponPrefab) as GameObject;
		collider = GetComponent<Collider>();
		collider.isTrigger = true;
	}

}
