using System.Collections.Generic;

using UnityEngine;
using Zenject;
using Sirenix.OdinInspector;

namespace Game.Systems.LocalizationSystem
{
	[CreateAssetMenu(fileName = "LocalizationInstaller", menuName = "Installers/LocalizationInstaller")]
	public class LocalizationInstaller : ScriptableObjectInstaller<LocalizationInstaller>
	{
		private const string Assets = "Game/Localization/";

		public SystemLanguage DefaultLanguage = SystemLanguage.English;
		[ReadOnly] public List<LocalizationData> localizations = new List<LocalizationData>();

		public override void InstallBindings()
        {
            Container.DeclareSignal<SignalLocalizationChanged>();

			Container.BindInstance(DefaultLanguage).WhenInjectedInto<LocalizationSystem>();
			Container.BindInstance(localizations).WhenInjectedInto<LocalizationSystem>();
            Container.BindInterfacesAndSelfTo<LocalizationSystem>().AsSingle();
        }


		[Button]
		private void SetLanguage(SystemLanguage language = SystemLanguage.English)
		{
			Container.Resolve<LocalizationSystem>().SelectLanguage(language);
		}
	}
}