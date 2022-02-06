using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game.Systems.WeatherSystem
{
	[CreateAssetMenu(menuName = "Installers/WeatherSystemInstaller", fileName = "WeatherSystemInstaller")]
	public class WeatherSystemInstaller : ScriptableObjectInstaller<WeatherSystemInstaller>
	{
		public WeatherSettings settings;

		public FogController fogController;

		public override void InstallBindings()
		{
			Container.BindInstance(settings).WhenInjectedInto<WeatherSystem>();

			Container.BindInterfacesAndSelfTo<WeatherSystem>().AsSingle();
		}
	}
}