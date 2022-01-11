using UnityEngine;

using Zenject;

public class MobileInput : IInput
{
	private MobileSettings settings;

	private UIPlayerLook playerLook;
	private UIPlayerMove playerMove;

	public MobileInput(InputSettings data, UIManager uiManager)
	{
		this.settings = data.mobile;

		playerLook = uiManager.Control.PlayerLook;
		playerMove = uiManager.Control.PlayerMove;
	}

	public float GetHorizontalCameraInput()
	{
		float input = playerLook.Direction.x;

		if (Time.timeScale > 0f && Time.deltaTime > 0f)
		{
			input /= Time.deltaTime;
			input *= Time.timeScale;
		}
		else
			input = 0f;

		//Apply sensitivity;
		input *= settings.inputMultiplier;

		//Invert input;
		if (settings.invertHorizontalInput)
			input *= -1f;

		return input;
	}

	public float GetVerticalCameraInput()
	{
		float input = playerLook.Direction.y;

		if (Time.timeScale > 0f && Time.deltaTime > 0f)
		{
			input /= Time.deltaTime;
			input *= Time.timeScale;
		}
		else
			input = 0f;

		//Apply sensitivity;
		input *= settings.inputMultiplier;

		//Invert input;
		if (settings.invertVerticalInput)
			input *= -1f;

		return input;
	}


	public float GetHorizontalMovementInput()
	{
		return playerMove.Direction.x;
	}

	public float GetVerticalMovementInput()
	{
		return playerMove.Direction.y;
	}

	public bool IsJumpKeyPressed()
	{
		return false;
	}
}

[System.Serializable]
public class MobileSettings 
{
	public bool invertHorizontalInput = false;
	public bool invertVerticalInput = false;

	public float inputMultiplier = 0.01f;
}