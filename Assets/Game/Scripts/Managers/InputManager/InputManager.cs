using Game.Managers.InputManger;

using System;
using UnityEngine;

using Zenject;

public class InputManager : IInitializable, ITickable, IDisposable
{
	public Vector3 InputPosition
	{
		get
		{
			if(platform == PlatformType.Mobile)
			{
				if(Input.touchCount > 0)
				{
					return Input.touches[0].position;
				}

				return Input.mousePosition;
			}

			return Input.mousePosition;
		}
	}

	private PlatformType platform;

	private SignalBus signalBus;
	private UIManager uiManager;
	private GlobalSettings globalSettings;
	private InputSettings inputSettings;

	public InputManager(SignalBus signalBus, GlobalSettings globalSettings, UIManager uiManager)
	{
		this.signalBus = signalBus;
		this.globalSettings = globalSettings;
		this.inputSettings = globalSettings.input;
		this.uiManager = uiManager;
	}

	public void Initialize()
	{
		platform = globalSettings.projectSettings.platform;

		if (platform == PlatformType.Mobile)
		{
			uiManager.BackpackButton.onClick.AddListener(() => Press(InputType.Inventory));
			uiManager.BackpackButton.onClick.AddListener(() => UnPress(InputType.Inventory));

			uiManager.Controls.ButtonA.onPress.AddListener(() => Press(InputType.Interaction));
			uiManager.Controls.ButtonA.onUnPress.AddListener(() => UnPress(InputType.Interaction));
		}
	}

	public void Dispose()
	{
		if (globalSettings.projectSettings.platform == PlatformType.Mobile)
		{
			uiManager.BackpackButton.onClick.RemoveAllListeners();

			uiManager.Controls.ButtonA.onClick.RemoveAllListeners();
			uiManager.Controls.ButtonA.onPress.RemoveAllListeners();
			uiManager.Controls.ButtonA.onUnPress.RemoveAllListeners();
		}
	}
	
	public void Tick()
	{
		if (InputDown())
		{
			signalBus?.Fire(new SignalInputDown());
		}
		if (InputUp())
		{
			signalBus?.Fire(new SignalInputUp());
		}


		//Проверка ввода KeyCode -> InputType
		if (globalSettings.projectSettings.platform == PlatformType.Desktop)
		{
			InputKey(KeyCode.Escape, InputType.Escape);

			InputKey(inputSettings.keyboard.interactionKey, InputType.Interaction);
			
			InputKey(inputSettings.keyboard.inventoryKey, InputType.Inventory);

			InputKey(inputSettings.keyboard.radialMenuKey, InputType.RadialMenu);

			InputKey(inputSettings.keyboard.buildingAcceptKey, InputType.BuildingAccept);
			InputKey(inputSettings.keyboard.buildingRejectKey, InputType.BuildingReject);
		}
	}

	public bool InputUp()
	{
		if(platform == PlatformType.Desktop)
		{
			return Input.GetKeyUp(KeyCode.Mouse0);
		}
		else if( platform == PlatformType.Mobile)
		{
			if(Input.touchCount > 0)
			{
				return Input.touches[0].phase == TouchPhase.Ended;
			}

			return false;
		}

		return false;
	}
	public bool InputDown()
	{
		if (platform == PlatformType.Desktop)
		{
			return Input.GetKeyDown(KeyCode.Mouse0);
		}
		else if (platform == PlatformType.Mobile)
		{
			if (Input.touchCount > 0)
			{
				return Input.touches[0].phase == TouchPhase.Began;
			}

			return false;
		}

		return false;
	}

	private void InputKey(KeyCode key, InputType input)
	{
		if (Input.GetKeyDown(key))
		{
			Down(key);
			Press(input);
		}
		if (Input.GetKeyUp(key))
		{
			Up(key);
			UnPress(input);
		}
	}

	private void Press(InputType input)
	{
		signalBus?.Fire(new SignalInputPressed() { input = input });
	}
	private void UnPress(InputType input)
	{
		signalBus?.Fire(new SignalInputUnPressed() { input = input });
	}

	private void Down(KeyCode key)
	{
		signalBus?.Fire(new SignalInputKeyDown() { key = key });
	}
	private void Up(KeyCode key)
	{
		signalBus?.Fire(new SignalInputKeyUp() { key = key });
	}
}