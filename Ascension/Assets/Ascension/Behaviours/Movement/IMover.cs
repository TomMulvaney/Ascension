using UnityEngine;
using System.Collections;

public interface IMover {
	Vector3 GetForce();
	Vector3 GetImpulse();
}
