using UnityEngine;
using System.Collections;

public class Destroy : MonoBehaviour {

	void OnTriggerEnter(Collider other){
		print ("hello");
		Destroy (other.gameObject);
	}
}
