using UnityEngine;
using System.Collections;

public class InputLog : MonoBehaviour {
    
    string[] _names = new string[] {"Horizontal", "Vertical", "Fire1", "Fire2", "Fire3", "Jump", "Mouse X", "Mouse Y", "Mouse ScrollWheel"};
	
	// Update is called once per frame
	void Update () {
        foreach (string name in _names) {
            if(Input.GetButton (name)) {
                Debug.Log (name);
            }
        }
	}
}
