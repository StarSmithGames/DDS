using Game.Systems.LocalizationSystem.Signals;

using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game.Systems.LocalizationSystem
{
	[CreateAssetMenu(fileName = "LocalizationInstaller", menuName = "Installers/LocalizationInstaller")]
	public class LocalizationScriptableInstaller : ScriptableObjectInstaller<LocalizationScriptableInstaller>
	{
        //public string DefaultId;
        //public List<LocalizationHandler.LocalizationSettings> localizationSettings = new List<LocalizationHandler.LocalizationSettings>();

        public override void InstallBindings()
        {
            Container.DeclareSignal<SignalLocalizationChanged>();

            //Container.BindInstance(localizationSettings).WhenInjectedInto<LocalizationHandler>();
            //Container.BindInstance(DefaultId).WhenInjectedInto<LocalizationSystem>();
            Container.BindInterfacesAndSelfTo<LocalizationSystem>().AsSingle();
        }
    }
}