using DG.Tweening;

using Game.Entities;
using Game.Managers.InputManger;
using Game.Systems.BuildingSystem;

using System;

using Zenject;

namespace Game.Systems.PassTimeSystem
{
	public class PassTimeHandler : IInitializable, IDisposable
	{
		private UIPassTimeWindow passTimeWindow;

		private SignalBus signalBus;
		private UIManager uiManager;
		private Player player;
		

		public PassTimeHandler(SignalBus signalBus, UIManager uiManager, Player player, LocalizationSystem.LocalizationSystem localization)
		{
			this.signalBus = signalBus;
			this.uiManager = uiManager;
			this.player = player;
		}

		public void Initialize()
		{
			passTimeWindow = uiManager.WindowsManager.GetAs<UIPassTimeWindow>();
			passTimeWindow.onCanceled += OnCanceled;

			passTimeWindow.PassTab.onPassTimeClicked += OnPassTimeClicked;
			passTimeWindow.SleepTab.onPassTimeClicked += OnPassTimeClicked;

			signalBus?.Subscribe<SignalInputUnPressed>(OnInputUnPressed);
		}

		public void Dispose()
		{
			passTimeWindow.onCanceled -= OnCanceled;

			passTimeWindow.PassTab.onPassTimeClicked -= OnPassTimeClicked;
			passTimeWindow.SleepTab.onPassTimeClicked -= OnPassTimeClicked;

			signalBus?.Unsubscribe<SignalInputUnPressed>(OnInputUnPressed);
		}

		public void SetConstruction(PassTimeConstruction construction)
		{
			passTimeWindow.SetData((construction.ConstructionData as PassTimeConstructionData).warmthBonus);
			OpenPassTime();
		}

		public void OpenPassTime(PassTimeType type = PassTimeType.None)
		{
			passTimeWindow.SetType(type);

			passTimeWindow.PassTab.SetData();
			passTimeWindow.SleepTab.SetData();
			passTimeWindow.Enable(true);
			passTimeWindow.Show();

			Sequence sequence = DOTween.Sequence();//open animation
			sequence
				.AppendInterval(0.1f)
				.AppendCallback(() =>
				{
					player.Freeze();
					player.DisableVision();
				});
		}

		private void OnPassTimeClicked()
		{
			passTimeWindow.Enable(false);

			if (passTimeWindow.IsSleepTab)
			{

			}
			else
			{

			}
		}

		private void OnCanceled()
		{
			passTimeWindow.Hide();
			player.UnFreeze();
			player.EnableVision();
		}

		private void OnInputUnPressed(SignalInputUnPressed signal)
		{
			if (signal.input == InputType.Escape && passTimeWindow.IsShowing)
			{
				OnCanceled();
			}
		}
	}

	public enum PassTimeType 
	{
		None,
		OnlyPassTime,
		FirstPassTime,
	}
}