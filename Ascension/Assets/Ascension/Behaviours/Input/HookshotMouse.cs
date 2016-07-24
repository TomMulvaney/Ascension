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

	public Vector2 GetHookshotVector(Vector3 characterPos) {
		if (Input.GetMouseButtonDown (0)) {
			Vector3 worldMousePos = _camera.ScreenToWorldPoint (Input.mousePosition);
            Vector2 delta = new Vector2(worldMousePos.x - characterPos.x, worldMousePos.y - characterPos.y);
			Debug.DrawRay (characterPos, delta, Color.green);
			Debug.DrawRay (characterPos, delta.normalized, Color.red);
            
            RaycastHit2D hit = Physics2D.Raycast (characterPos, delta);
            
            if (hit.collider != null) {
                Vector3 hitCameraPos = _camera.WorldToViewportPoint (hit.point);                
                if (hitCameraPos.x >= 0 && hitCameraPos.x <= 1 && hitCameraPos.y >= 0 && hitCameraPos.y <= 1) {
                    delta.Normalize ();
                    return delta;
                }
            }
		}
        
        return Vector2.zero;
	}
}