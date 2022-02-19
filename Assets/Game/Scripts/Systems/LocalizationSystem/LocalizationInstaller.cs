using System.Collections.Generic;

using UnityEngine;
using Zenject;

namespace Game.Systems.LocalizationSystem
{
	[CreateAssetMenu(fileName = "LocalizationInstaller", menuName = "Installers/LocalizationInstaller")]
	public class LocalizationInstaller : ScriptableObjectInstaller<LocalizationInstaller>
	{
		public SystemLanguage DefaultLanguage = SystemLanguage.English;
		public List<LocalizationData> localizations = new List<LocalizationData>();

		public override void InstallBindings()
        {
            Container.DeclareSignal<SignalLocalizationChanged>();

			Container.BindInstance(DefaultLanguage).WhenInjectedInto<LocalizationSystem>();
			Container.BindInstance(localizations).WhenInjectedInto<LocalizationSystem>();
            Container.BindInterfacesAndSelfTo<LocalizationSystem>().AsSingle();
        }
    }
}