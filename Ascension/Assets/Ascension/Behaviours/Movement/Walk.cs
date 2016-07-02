using UnityEngine;
using System;

public class Walk : MonoBehaviour, IMove {
	[SerializeField]
	ILRInput input;
	[SerializeField]
	int walkSpeed;

	public Vector3 GetForce() {
		Vector3 walkForce = Vector3.zero;
		if (input.IsLeft ()) {
			walkForce += Vector3.left * walkSpeed;
		}
		if (input.IsRight ()) {
			walkForce += Vector3.right * walkSpeed;
		}
		return walkForce;
	}

	public Vector3 GetImpulse() {
		throw new NotImplementedException ();
	}
}
