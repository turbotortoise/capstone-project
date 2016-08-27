using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NoteAttack : MonoBehaviour {

	public float power;
    public float speed = 3f;
    public Color color;


    void Update() {
        transform.position += transform.forward * Time.deltaTime * speed;
        color.a -= 0.1f * Time.deltaTime;
        GetComponent<Renderer>().material.color = color;
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.rigidbody && collision.rigidbody.tag!="Enemy") return;
        collision.rigidbody.GetComponent<Enemy>().Kill();
    }


}
