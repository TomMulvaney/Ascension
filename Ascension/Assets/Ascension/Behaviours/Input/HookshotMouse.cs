using System;
using UnityEngine;
using System.Collections;

public class HookshotMouse : MonoBehaviour, IHookshot {
	Camera _camera;

	void Awake() {
		if (_camera == null) {
			_camera = UnityEngine.Object.FindObjectOfType <Camera>();
		}
	}

	public Vector2 GetHookshotVector() {
		throw new NotImplementedException ();
	}

	public Vector2 GetHookshotVector(Vector3 characterPos)
	{
		if (Input.GetMouseButtonDown (0)) {
			Vector3 worldMousePos = _camera.ScreenToWorldPoint (Input.mousePosition);
			Vector3 delta = worldMousePos - characterPos;
			Debug.DrawRay (characterPos, delta, Color.green);
			Debug.DrawRay (characterPos, delta.normalized, Color.red);
			return delta.normalized;
		}
		else {
			return Vector2.zero;
		}
	}
}
