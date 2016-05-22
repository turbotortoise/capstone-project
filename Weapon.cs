using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : MonoBehaviour {
	public GameObject 	weaponPrefab;
	public float		power;
	public SphereCollider sphere;
	private float		waitTime = 1f;
	//public Tag			tag;

	void Start() {
		GameObject weaponInstance;
		weaponInstance = Instantiate(weaponPrefab) as GameObject;
		sphere = GetComponent<SphereCollider>();
		sphere.isTrigger = true;
	}

}