using Game.Entities;
using Game.Systems.BuildingSystem;

using System;

using Zenject;

namespace Game.Systems.PassTimeSystem
{
	public class PassTimeHandler : IInitializable, IDisposable
	{
		private UIPassTimeModalWindow passTimeWindow;

		private UIManager uiManager;
		private Player player;
		private LocalizationSystem.LocalizationSystem localization;

		public PassTimeHandler(UIManager uiManager, Player player, LocalizationSystem.LocalizationSystem localization)
		{
			this.uiManager = uiManager;
			this.player = player;
			this.localization = localization;
		}

		public void Initialize()
		{
			passTimeWindow = uiManager.WindowsManager.GetAs<UIPassTimeModalWindow>();
			passTimeWindow.onCanceled += OnCanceled;
		}

		public void Dispose()
		{
			passTimeWindow.onCanceled -= OnCanceled;
		}

		public void SetConstruction(PassTimeConstruction construction)
		{
			player.Freeze();
			player.DisableVision();

			passTimeWindow.SetData((construction.ConstructionData as PassTimeConstructionData).warmthBonus);

			passTimeWindow.Show();
		}

		private void Localize()
		{
			//localization.
		}


		private void OnCanceled()
		{
			passTimeWindow.Hide();
			player.UnFreeze();
			player.EnableVision();
		}
	}
}