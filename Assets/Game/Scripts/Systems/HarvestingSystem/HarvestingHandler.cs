using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

namespace Game.Systems.HarvestingSystem
{
	public class HarvestingHandler : IInitializable, IDisposable
	{
		private UIManager uiManager;

		public HarvestingHandler(UIManager uiManager)
		{
			this.uiManager = uiManager;
		}

		public void Initialize()
		{
		}

		public void Dispose()
		{
		}

		public void SetHarveting(HarvestingConstruction construction)
		{
			GameObject.Destroy(construction.gameObject);
		}
	}
}