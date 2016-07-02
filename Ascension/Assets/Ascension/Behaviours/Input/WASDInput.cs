using UnityEngine;
using System.Collections;

public class WASDInput : MonoBehaviour, ILRInput, IUDInput {

	public bool IsUp()
	{
		return Input.GetKeyDown(KeyCode.W);
	}

	public bool IsDown()
	{
		return Input.GetKeyDown(KeyCode.W);
	}
	
	public bool IsLeft()
	{
		return Input.GetKeyDown(KeyCode.W);
	}

	public bool IsRight()
	{
		return Input.GetKeyDown(KeyCode.W);
	}
}
