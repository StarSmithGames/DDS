using System;
using UnityEngine;

using Zenject;

public class InputManager : IInitializable, ITickable, IDisposable
{
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
		if (globalSettings.projectSettings.platform == PlatformType.Mobile)
		{
			uiManager.BackpackButton.onClick.AddListener(() => Click(InputType.Inventory));
			//uiManager.BackpackButton.onClick.AddListener(() => Click(InputType.));

			uiManager.Controls.ButtonA.onClick.AddListener(() => { Click(InputType.Interaction); });
			uiManager.Controls.ButtonA.onPress.AddListener(() => { Press(InputType.Interaction); });
			uiManager.Controls.ButtonA.onUnPress.AddListener(() => { UnPress(InputType.Interaction); });
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
		if(globalSettings.projectSettings.platform == PlatformType.Desktop)
		{
			InputKey(inputSettings.keyboard.interactionKey, InputType.Interaction);
			
			InputKey(inputSettings.keyboard.inventoryKey, InputType.Inventory);

			InputKey(inputSettings.keyboard.buildingAcceptKey, InputType.BuildingAccept);
			InputKey(inputSettings.keyboard.buildingRejectKey, InputType.BuildingReject);
		}
	}

	private void InputKey(KeyCode key, InputType input)
	{
		if (Input.GetKey(key))
		{
			Click(input);
		}
		if (Input.GetKeyDown(key))
		{
			Press(input);
		}
		if (Input.GetKeyUp(key))
		{
			UnPress(input);
		}
	}

	private void Click(InputType input)
	{
		signalBus?.Fire(new SignalInputClicked() { input = input });
	}

	private void Press(InputType input)
	{
		signalBus?.Fire(new SignalInputPressed() { input = input });
	}
	private void UnPress(InputType input)
	{
		signalBus?.Fire(new SignalInputUnPressed() { input = input });
	}
}