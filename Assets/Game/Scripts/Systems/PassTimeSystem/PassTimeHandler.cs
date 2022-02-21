using DG.Tweening;

using Game.Entities;
using Game.Managers.InputManger;
using Game.Systems.BuildingSystem;
using Game.Systems.TimeSystem;

using System;

using UnityEngine;

using Zenject;

namespace Game.Systems.PassTimeSystem
{
	public class PassTimeHandler : IInitializable, IDisposable
	{
		private UIPassTimeWindow passTimeWindow;

		private SignalBus signalBus;
		private UIManager uiManager;
		private Player player;
		private TimeSystem.TimeSystem timeSystem;

		public PassTimeHandler(SignalBus signalBus, UIManager uiManager, Player player, TimeSystem.TimeSystem timeSystem)
		{
			this.signalBus = signalBus;
			this.uiManager = uiManager;
			this.player = player;
			this.timeSystem = timeSystem;
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

		public void OpenPassTime(PassTimeType type = PassTimeType.Both)
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

		public void ClosePassTime()
		{
			passTimeWindow.Hide();
			player.UnFreeze();
			player.EnableVision();
		}

		private void OnPassTimeClicked()
		{
			passTimeWindow.Enable(false);

			UIPassTimeTab tab = passTimeWindow.CurrentTab;

			timeSystem.Pause();

			var from = timeSystem.GlobalTime;
			var to = from + new TimeSystem.Time() { Hours = tab.Hours };

			int fromHours = tab.Hours;
			int toHours = 0;

			timeSystem.StartRewind(from, to, 3f,
				onProgress : (float progress)=> 
				{
					tab.Hours = (int)Mathf.Lerp(fromHours, toHours, progress);
				},
				onEnd : ClosePassTime);
		}

		private void OnCanceled()
		{
			ClosePassTime();
		}

		private void OnInputUnPressed(SignalInputUnPressed signal)
		{
			if (signal.input == InputType.Escape && passTimeWindow.IsShowing && passTimeWindow.IsEnable)
			{
				OnCanceled();
			}
		}
	}

	public enum PassTimeType 
	{
		Both,
		OnlySleep,
		OnlyPassTime,
	}
}