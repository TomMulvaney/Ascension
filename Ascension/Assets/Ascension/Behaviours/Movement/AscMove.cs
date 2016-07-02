using UnityEngine;
using System.Collections;
using System;

public class AscMove : MonoBehaviour {
	[SerializeField]
	bool showGUI;
	[SerializeField]
	int walkAcceleration;
	[SerializeField]
	int walkMaxSpeed;
	[SerializeField]
	int wallSlideSpeed;
	[SerializeField]
	int jumpSpeed;
	[SerializeField]
	int gravity;
	[SerializeField]
	Rigidbody2D rb;

	enum States {
		Ground,
		Air,
		Wall,
		Ceiling
	}

	States state;
	States nextState;

	Vector2 force = Vector2.zero;
	Vector2 impulse = Vector2.zero;

	void Start () {
		if (rb == null) {
			rb = gameObject.GetComponent<Rigidbody2D> ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		force = Vector2.zero;
		impulse = Vector2.zero;

		switch (state) {
		case States.Ground:
			CalcWalk ();
			break;
		case States.Air:
			break;
		case States.Wall:
			break;
		case States.Ceiling:
			break;
		}

		if (Vector2.Distance (force, Vector2.zero) > 0.01) {
			rb.AddForce (force, ForceMode2D.Force);
		}

		if (Vector2.Distance (impulse, Vector2.zero) > 0.01) {
			rb.AddForce (impulse, ForceMode2D.Impulse);
		}
	}

	void CalcWalk() {
		if (Input.GetKey (KeyCode.A) && rb.velocity.x > -walkMaxSpeed) {
			force.x -= walkAcceleration;
		}


		if (Input.GetKey (KeyCode.D) && rb.velocity.x < walkMaxSpeed) {
			force.x += walkAcceleration;
		}

		/*
		if (Input.GetKeyUp (KeyCode.D) && rb.velocity.x > 0) {
			Debug.Log ("D Up");
			impulse.x -= rb.velocity.x;
		}
		if (Input.GetKeyUp (KeyCode.A) && rb.velocity.x < 0) {
			Debug.Log ("A Up");
			impulse.x += rb.velocity.x;
		}
		*/
	}

	void OnGUI () {
		if (showGUI) {
			GUILayout.Label (string.Format("Speed: {0}", rb.velocity.magnitude));
			GUILayout.Label (string.Format("Velocity: {0}", rb.velocity));
			GUILayout.Label (string.Format("Position: {0}", transform.position));
		}
	}
}
