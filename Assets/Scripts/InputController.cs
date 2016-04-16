using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputController
{
	private Dictionary<Button, string> inputMap = new Dictionary<Button, string>()
	{
		{ Button.Up, "up" },
		{Button.Left, "left" },
		{ Button.Right, "right" },
		{ Button.Dodge, "space" },
		{ Button.Flip, "down" }
	};

	public bool GetButton(Button button)
	{
		return Input.GetButton(inputMap[button]);
	}

	public bool GetButtonDown(Button button)
	{
		return Input.GetButtonDown(inputMap[button]);
	}

	public bool GetButtonUp(Button button)
	{
		return Input.GetButtonUp(inputMap[button]);
	}

}

public enum Button
{
	Up,
	Left,
	Right,
	Dodge,
	Flip
}
