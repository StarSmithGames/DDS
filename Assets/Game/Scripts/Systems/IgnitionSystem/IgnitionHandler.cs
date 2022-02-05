using Game.Entities;
using Game.Systems.BuildingSystem;

using System;
using System.Collections;
using UnityEngine;
using Zenject;
using UnityEngine.Assertions;
using DG.Tweening;
using System.Linq;
using Game.Systems.InventorySystem;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine.Events;

namespace Game.Systems.IgnitionSystem
{
	public class IgnitionHandler : IInitializable, IDisposable
	{
		public bool IsOpened => isOpened;
		private bool isOpened = false;

		public bool IsIgnitionProcess => ignitionCoroutine != null;
		private Coroutine ignitionCoroutine = null;

		private UIIgnitionWindow window;
		private FireConstruction fireConstruction;

		private float successChance = 0;

		private SignalBus signalBus;
		private UIManager uiManager;
		private AsyncManager asyncManager;
		private Player player;

		public IgnitionHandler(SignalBus signalBus,
			UIManager uiManager,
			AsyncManager asyncManager,
			Player player)
		{
			this.signalBus = signalBus;
			this.uiManager = uiManager;
			this.asyncManager = asyncManager;
			this.player = player;
		}

		public void Initialize()
		{
			window = uiManager.WindowsManager.GetAs<UIIgnitionWindow>();

			window.StartButton.onClick.AddListener(OnStartButtonClicked);
			window.BackButton.onClick.AddListener(OnBackButtonClicked);

			signalBus?.Subscribe<SignalUIIgnitionSlotItemChanged>(OnSlotChanged);

			signalBus?.Subscribe<SignalInputUnPressed>(OnInputUnPressed);
		}

		public void Dispose()
		{
			window.StartButton.onClick.RemoveAllListeners();
			window.BackButton.onClick.RemoveAllListeners();

			signalBus?.Unsubscribe<SignalUIIgnitionSlotItemChanged>(OnSlotChanged);

			signalBus?.Unsubscribe<SignalInputUnPressed>(OnInputUnPressed);
		}

		public void SetIgnition(FireConstruction fireConstruction)
		{
			this.fireConstruction = fireConstruction;

			OpenIgnition();
		}

		private void OpenIgnition()
		{
			player.DisableVision();
			uiManager.Targets.HideAll();
			player.Freeze();
			uiManager.Controls.DisableButtons();

			uiManager.WindowsManager.Show<UIIgnitionWindow>();

			var playerItems = player.Status.Inventory.Items;

			window.Starter.SetItems(playerItems.Where((x) => x.IsFireStarter).ToList());
			window.Tinder.SetItems(playerItems.Where((x) => x.IsFireTinder).ToList());
			window.Fuel.SetItems(playerItems.Where((x) => x.IsFireFuel).ToList());
			window.Accelerant.SetItems(playerItems.Where((x) => x.IsFireAccelerant).ToList());

			Sequence sequence = DOTween.Sequence();//open animation

			sequence
				.AppendInterval(0.5f)
				.AppendCallback(() => isOpened = true);
		}

		private void CloseIgnition()
		{
			uiManager.WindowsManager.Hide<UIIgnitionWindow>();

			player.EnableVision();
			player.UnFreeze();
			uiManager.Controls.EnableButtons();

			isOpened = false;
		}

		private void StartIgnition()
		{
			if (!window.Starter.IsEmpty && !window.Tinder.IsEmpty && !window.Fuel.IsEmpty)
			{
				ExchangeOnStart();

				if (!IsIgnitionProcess)
				{
					ignitionCoroutine = asyncManager.StartCoroutine(Ignition());
				}
			}
		}

		private IEnumerator Ignition()
		{
			uiManager.Targets.Filler.ShowFiller();

			FireStartingConstructionData data = null;
			if (fireConstruction.ConstructionData is FireStartingConstructionData fireStarting)
			{
				data = fireStarting;
			}
			Assert.IsNotNull(data, $"{fireConstruction.gameObject.name} FireStartingConstructionData is NULL");

			float t = 0;

			while(t < data.maxIgnitionTime)
			{
				float value = t / data.maxIgnitionTime;

				uiManager.Targets.Filler.SetFiller(value);

				t += Time.deltaTime;
				yield return null;
			}

			ExchangeOnComplete();
			fireConstruction.IsCompleted = true;

			uiManager.Targets.Filler.HideFiller();

			CloseIgnition();

			StopIgnition();
		}

		private void StopIgnition()
		{
			if (IsIgnitionProcess)
			{
				asyncManager.StopCoroutine(ignitionCoroutine);
				ignitionCoroutine = null;
			}
		}

		private void ExchangeOnStart()
		{
			var starter = window.Starter.CurrentItem;

			if (starter.ItemData.isStackable)
			{
				if (starter.CurrentStackSize - 1 == 0)
				{
					player.Status.Inventory.Remove(starter);
				}
				else
				{
					starter.CurrentStackSize -= 1;
				}
			}
			else
			{
				player.Status.Inventory.Remove(starter);
			}


			if (!window.Accelerant.IsEmpty)
			{

			}
		}

		private void ExchangeOnComplete()
		{
			var fuel = window.Fuel.CurrentItem;
			var tinder = window.Tinder.CurrentItem;
			var accelerant = window.Accelerant.CurrentItem;

			if (fuel.CurrentStackSize - 1 == 0)
			{
				player.Status.Inventory.Remove(fuel);
			}
			else
			{
				fuel.CurrentStackSize -= 1;
			}

			if (tinder.CurrentStackSize - 1 == 0)
			{
				player.Status.Inventory.Remove(tinder);
			}
			else
			{
				tinder.CurrentStackSize -= 1;
			}
		}

		private void ExchangeItem()
		{

		}

		private void OnSlotChanged(SignalUIIgnitionSlotItemChanged signal)
		{
			float playerBaseChance = 40;
			successChance = playerBaseChance;

			for (int i = 0; i < window.IgnitionSlots.Count; i++)
			{
				if (!window.IgnitionSlots[i].IsEmpty)
				{
					FireItemData fireData = window.IgnitionSlots[i].CurrentItem.ItemData as FireItemData;

					successChance += fireData.chance;
				}
			}

			successChance = successChance > 100 ? 100 : successChance;

			window.BaseChance.text = playerBaseChance + "%";
			window.SuccessChance.text = successChance + "%";
			window.Duration.text = "10D";
		}

		private void OnBackButtonClicked()
		{
			if (isOpened && !IsIgnitionProcess)
			{
				CloseIgnition();

				if (!fireConstruction.IsCompleted)
				{
					GameObject.Destroy(fireConstruction.gameObject);
					fireConstruction = null;
				}
			}
		}

		private void OnStartButtonClicked()
		{
			if (isOpened && !IsIgnitionProcess)
			{
				StartIgnition();
			}
		}

		private void OnInputUnPressed(SignalInputUnPressed signal)
		{
			if (IsOpened && !IsIgnitionProcess)
			{
				if(signal.input == InputType.Escape)
				{
					CloseIgnition();
				}
				else if (signal.input == InputType.IgnitionStartFire)
				{
					StartIgnition();
				}
			}
		}
	}
}