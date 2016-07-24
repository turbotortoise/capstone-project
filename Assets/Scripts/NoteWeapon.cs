using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NoteWeapon : MonoBehaviour {
	public GameObject 	weaponPrefab;
	public float		power;
	public SphereCollider sphere;
	//public Tag			tag;

	void Start() {
		GameObject weaponInstance = Instantiate(weaponPrefab) as GameObject;
		sphere = GetComponent<SphereCollider>();
		sphere.isTrigger = true;
	}
}
