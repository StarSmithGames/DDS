using Funly.SkyStudio;

using UnityEngine;
using Zenject;

namespace Game.Systems.EnvironmentSystem
{
	[CreateAssetMenu(menuName = "Installers/EnvironmentSystemInstaller", fileName = "EnvironmentSystemInstaller")]
    public class EnvironmentSystemInstaller : ScriptableObjectInstaller<EnvironmentSystemInstaller>
    {
		public SkySettings skySettings;
		public WeatherSettings weatherSettings;

		public override void InstallBindings()
		{
			BindSky();

			BindWeather();
		}

		private void BindSky()
		{
			Container.BindInstance(FindObjectOfType<TimeOfDayController>()).WhenInjectedInto<SkyController>();
			Container.BindInstance(skySettings).WhenInjectedInto<SkyController>();
			Container.BindInterfacesAndSelfTo<SkyController>().AsSingle();
		}

		private void BindWeather()
		{
			Container.DeclareSignal<SignalWeatherChanged>();

			Container.BindInstance(FindObjectOfType<UI3DWindArrow>(true)).WhenInjectedInto<WindController>();
			Container.BindInterfacesAndSelfTo<WindController>().AsSingle();

			Container.BindInstance(FindObjectOfType<FogNormal>(true)).WhenInjectedInto<FogController>();
			Container.BindInterfacesAndSelfTo<FogController>().AsSingle();

			Container.BindInstance(weatherSettings).WhenInjectedInto<WeatherSystem>();
			Container.BindInterfacesAndSelfTo<WeatherSystem>().AsSingle();
		}

		public void SetWeather(WeatherPresset presset)
		{
			if (Application.isPlaying)
			{
				if (presset != null)
				{
					Container.Resolve<WeatherSystem>().StartTransition(presset);
				}
			}
		}
	}
}