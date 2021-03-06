﻿using UnityEngine;
using System.Collections;

public class Shell : MonoBehaviour {
	public GameObject target;
	public bool fired = false;
	public float speed = 22;
	private Vector3 oldVelocity;
	private float time = 0;
	private bool isColliderEnabled = false;
	private bool itemCollide = false;
    private GetInput input;

	// Use this for initialization
	void Start () {
        input = target.GetComponent<GetInput>();
	}

	public void Fire() {
		fired = true;
		isColliderEnabled = true;
		gameObject.GetComponent<Rigidbody> ().isKinematic = false;
		Vector3 kartDir = 1f*transform.forward;
		Vector3 spawnPos = transform.position + kartDir * 4.5f;
		transform.position = spawnPos;
		GetComponent<Rigidbody>().velocity = transform.TransformDirection( new Vector3( 0, 0, speed ) );
		isColliderEnabled = true;

		//GetComponent<BoxCollider> ().enabled = true;
	}
		
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.JoystickButton1)) {
			Fire ();
		}

		if (fired) {
			time += Time.deltaTime;
			if (time > 5f) {
				Destroy (gameObject);
				time = 0;
			}
			GetComponent<Rigidbody>().velocity = speed * (GetComponent<Rigidbody>().velocity.normalized);

			//print (GetComponent<Rigidbody> ().velocity);
			//GetComponent<Rigidbody>().AddForce(transform.forward * 20);
		} else if(target!=null){
			//gameObject.GetComponent<BoxCollider> ().enabled = false;
			Vector3 kartDir = -1f*target.transform.forward;
			Vector3 followPos = target.transform.position + kartDir * 1.8f;
			transform.position = followPos;
			transform.LookAt(target.transform);
		}
	}
		
	void FixedUpdate () {
		// because we want the velocity after physics, we put this in fixed update
		oldVelocity = GetComponent<Rigidbody>().velocity;
	}


	void OnCollisionEnter (Collision collision) {
		if (isColliderEnabled) {
			if (collision.gameObject.tag == "kart") {
				collision.gameObject.GetComponent<PowerUp> ().powerUp = "Fake";
				collision.gameObject.GetComponent<PowerUp> ().Activate ();
				Destroy (gameObject);
			} else if (collision.transform.GetComponent<Banana>() != null){
				Destroy (gameObject);
				Destroy (collision.gameObject);
			} else {
				// get the point of contact
				ContactPoint contact = collision.contacts[0];

				// reflect our old velocity off the contact point's normal vector
				Vector3 reflectedVelocity = Vector3.Reflect(oldVelocity, contact.normal);        

				// assign the reflected velocity back to the rigidbody
				GetComponent<Rigidbody>().velocity = reflectedVelocity;
			}
		}
	}
}
