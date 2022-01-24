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

namespace Game.Systems.IgnitionSystem
{
	public class IgnitionHandler : IInitializable, IDisposable
	{
		public bool IsOpened => isOpened;
		private bool isOpened = false;

		private bool isReady = false;

		public bool IsIgnitionProcess => ignitionCoroutine != null;
		private Coroutine ignitionCoroutine = null;

		private UIIgnitionWindow window;
		private FireConstruction fireConstruction;

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

			signalBus?.Subscribe<SignalInputUnPressed>(OnInputUnPressed);
		}

		public void Dispose()
		{
			window.StartButton.onClick.RemoveAllListeners();
			window.BackButton.onClick.RemoveAllListeners();

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
			if (starter.CurrentStackSize - 1 == 0)
			{
				player.Status.Inventory.Remove(starter);
			}
			else
			{
				starter.CurrentStackSize -= 1;
			}

			if (!window.Accelerant.IsEmpty)
			{

			}
		}

		private void ExchangeOnComplete()
		{

		}


		private void OnBackButtonClicked()
		{
			if (isOpened && !IsIgnitionProcess)
			{
				CloseIgnition();
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