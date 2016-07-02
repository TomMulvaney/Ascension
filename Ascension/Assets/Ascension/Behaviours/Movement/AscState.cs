using UnityEngine;
using System.Collections;

public class AscState : MonoBehaviour 
{
	[SerializeField]
	IMove walke;
	[SerializeField]
	IMove jumpe;
	[SerializeField]
	IMove wallSlide;
	[SerializeField]
	IMove gravity;

	enum States
	{
		Ground,
		Air,
		Wall,
		Ceiling
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
