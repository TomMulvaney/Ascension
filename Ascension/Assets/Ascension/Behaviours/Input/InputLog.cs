using UnityEngine;
using System.Collections;

public class InputLog : MonoBehaviour {
    
    string[] _names = new string[] {"Fire1", "Fire2", "Fire3", "Jump", "Mouse X", "Mouse Y", "Mouse ScrollWheel", "Submit", "Cancel"};
	
	// Update is called once per frame
	void Update () {
        foreach (string name in _names) {
            if(Input.GetButton (name)) {
                Debug.Log (name);
            }
        }
	}
    
    void OnGUI() {
        GUILayout.Label (string.Format ("Vertical: {0}", Input.GetAxis ("Vertical")));
        GUILayout.Label (string.Format ("Horizontal: {0}", Input.GetAxis ("Horizontal")));
    }
}
