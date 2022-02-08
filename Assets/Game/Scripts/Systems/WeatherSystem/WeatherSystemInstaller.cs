using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Sirenix.OdinInspector;

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

        [Button]
        private void Clear()
        {
            if (Application.isPlaying)
            {
                Container.Resolve<WeatherSystem>().StartTransition(settings.pressetClear);
            }
        }
        [Button]
        private void Fog()
        {
            if (Application.isPlaying)
            {
                Container.Resolve<WeatherSystem>().StartTransition(settings.pressetFog);
            }
        }
    }
}