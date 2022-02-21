using System;

using UnityEngine;

using Zenject;

namespace Game.Systems.EnvironmentSystem
{
	public class WindController : IInitializable, IDisposable
	{
		private UI3DWindArrow windArrow;

		public WindController(UI3DWindArrow windArrow)
		{
			this.windArrow = windArrow;
		}

		public void Initialize()
		{
		}

		public void Dispose()
		{
		}

		public void SetWindDirection(Vector3 forward)
		{
			windArrow.Arrow.forward = forward;
		}
	}
}