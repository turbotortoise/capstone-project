using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NoteAttack : MonoBehaviour {

	public float power;

    void OnCollisionEnter(Collision collision) {
        if (collision.rigidbody && collision.rigidbody.tag!="Enemy") return;
        collision.rigidbody.GetComponent<Enemy>().Kill();
    }


}
