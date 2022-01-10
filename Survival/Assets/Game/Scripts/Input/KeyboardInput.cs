using UnityEngine;

public class KeyboardInput : IInput
{
	private KeyboardSettings settings;

	public KeyboardInput(InputData data)
	{
		this.settings = data.keyboard;
	}

	public float GetHorizontalCameraInput()
	{
		//Get raw mouse input;
		float _input = Input.GetAxisRaw(settings.mouseHorizontalAxis);

		//Since raw mouse input is already time-based, we need to correct for this before passing the input to the camera controller;
		if (Time.timeScale > 0f && Time.deltaTime > 0f)
		{
			_input /= Time.deltaTime;
			_input *= Time.timeScale;
		}
		else
			_input = 0f;

		//Apply mouse sensitivity;
		_input *= settings.mouseInputMultiplier;

		//Invert input;
		if (settings.invertHorizontalInput)
			_input *= -1f;

		return _input;
	}
	public float GetVerticalCameraInput()
	{
		//Get raw mouse input;
		float _input = -Input.GetAxisRaw(settings.mouseVerticalAxis);

		//Since raw mouse input is already time-based, we need to correct for this before passing the input to the camera controller;
		if (Time.timeScale > 0f && Time.deltaTime > 0f)
		{
			_input /= Time.deltaTime;
			_input *= Time.timeScale;
		}
		else
			_input = 0f;

		//Apply mouse sensitivity;
		_input *= settings.mouseInputMultiplier;

		//Invert input;
		if (settings.invertVerticalInput)
			_input *= -1f;

		return _input;
	}


	public float GetHorizontalMovementInput()
	{
		if (settings.useRawInput)
			return Input.GetAxisRaw(settings.horizontalInputAxis);
		else
			return Input.GetAxis(settings.horizontalInputAxis);
	}

	public float GetVerticalMovementInput()
	{
		if (settings.useRawInput)
			return Input.GetAxisRaw(settings.verticalInputAxis);
		else
			return Input.GetAxis(settings.verticalInputAxis);
	}

	public bool IsJumpKeyPressed()
	{
		if (settings.useJump)
		{
			return Input.GetKey(settings.jumpKey);
		}
		return false;
	}
}

[System.Serializable]
public class KeyboardSettings
{
	public string mouseHorizontalAxis = "Mouse X";
	public string mouseVerticalAxis = "Mouse Y";

	public bool invertHorizontalInput = false;
	public bool invertVerticalInput = false;

	public float mouseInputMultiplier = 0.01f;

	[Space]
	public string horizontalInputAxis = "Horizontal";
	public string verticalInputAxis = "Vertical";

	public bool useJump = true;
	public KeyCode jumpKey = KeyCode.Space;

	//If this is enabled, Unity's internal input smoothing is bypassed;
	public bool useRawInput = true;
}