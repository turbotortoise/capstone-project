using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Attack : MonoBehaviour {

	float delay = 1f;
	public float power = 0.2f;

#pragma warning disable 0108
	Collider collider;
#pragma warning restore 0108

	IEnumerator Start() {
		collider = GetComponent<Collider>();
		yield return new WaitForSeconds(delay);
		Destroy(gameObject);
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.rigidbody.tag!="Player") return;
		collision.rigidbody.GetComponent<Player>().ApplyDamage(power);
	}
}
