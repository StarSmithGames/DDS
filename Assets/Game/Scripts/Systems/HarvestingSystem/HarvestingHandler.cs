using Game.Entities;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

namespace Game.Systems.HarvestingSystem
{
	public class HarvestingHandler : IInitializable, IDisposable
	{
		private UIHarvestingWindow harvestingWindow;

		private UIManager uiManager;
		private Player player;

		public HarvestingHandler(UIManager uiManager, Player player)
		{
			this.uiManager = uiManager;
			this.player = player;
		}

		public void Initialize()
		{
			harvestingWindow = uiManager.WindowsManager.GetAs<UIHarvestingWindow>();

			harvestingWindow.onClosed += OnClosed;
		}

		public void Dispose()
		{
			harvestingWindow.onClosed -= OnClosed;
		}

		public void SetHarveting(HarvestingConstruction construction)
		{
			ShowHarvesting();
			//GameObject.Destroy(construction.gameObject);
		}

		public void ShowHarvesting()
		{
			player.Freeze();
			player.DisableVision();

			harvestingWindow.Show();

		}

		public void HideHarvesting()
		{
			harvestingWindow.Hide();

			player.UnFreeze();
			player.EnableVision();
		}

		private void OnClosed()
		{
			HideHarvesting();
		}
	}
}