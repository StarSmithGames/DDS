using Sirenix.OdinInspector;

using UnityEngine;
using Zenject;

namespace Game.Systems.WeatherSystem
{
	[CreateAssetMenu(menuName = "Installers/WeatherSystemInstaller", fileName = "WeatherSystemInstaller")]
	public class WeatherSystemInstaller : ScriptableObjectInstaller<WeatherSystemInstaller>
	{
        public WeatherSettings settings;

		public override void InstallBindings()
		{
            Container.BindInstance(FindObjectOfType<NormalFog>(true)).WhenInjectedInto<FogController>();
			Container.BindInterfacesAndSelfTo<FogController>().AsSingle();

			Container.BindInstance(settings).WhenInjectedInto<WeatherSystem>();
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

    [System.Serializable]
    public class WeatherSettings
    {
        public WeatherForecast forecast;

        [InlineButton("Test")]
        public WeatherPresset pressetClear;
        [InlineButton("Test")]
        public WeatherPresset pressetSnowFall;
        [InlineButton("Test")]
        public WeatherPresset pressetFog;
        [InlineButton("Test")]
        public WeatherPresset pressetMilk;
        [InlineButton("Test")]
        public WeatherPresset pressetAurora;
        [InlineButton("Test")]
        public WeatherPresset pressetCloudy;
        [InlineButton("Test")]
        public WeatherPresset pressetBlizzard;

        private void Test(WeatherPresset presset)
        {
#if UNITY_EDITOR
            var asset = AssetDatabaseExtensions.GetAsset<WeatherSystemInstaller>("Installers/WeatherSystemInstaller/WeatherSystemInstaller.asset");
            asset.SetWeather(presset);
#endif
        }
    }
}