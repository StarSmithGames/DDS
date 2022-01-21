using Sirenix.OdinInspector;
using System.Collections.Generic;

using UnityEngine;
using Zenject;

namespace Game.Systems.RadialMenu
{
	[CreateAssetMenu(fileName = "RadialMenuInstaller", menuName = "Installers/RadialMenuInstaller")]
    public class RadialMenuInstaller : ScriptableObjectInstaller<RadialMenuInstaller>
    {
		[SerializeField] private RadialMenuSettings radialMenuSettings;

		public override void InstallBindings()
		{
			Container.DeclareSignal<SignalRadialMenuOptionChanged>();


			Container.BindFactory<UIRadialMenuOption, UIRadialMenuOption.Factory>()
				.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(12)
				.FromComponentInNewPrefab(radialMenuSettings.optionPrefab));

			Container.BindInstance(radialMenuSettings).WhenInjectedInto<RadialMenuHandler>();

			Container.BindInterfacesAndSelfTo<RadialMenuHandler>().AsSingle();
		}
	}

	[System.Serializable]
	public class RadialMenuSettings
	{
		public float NormalCount => 1 / countOptions;

		[Range(2, 12)]
		public int countOptions;
		public bool isSnap = true;
		
		public float maxRadius = 200f;

		public UIRadialMenuOption optionPrefab;
	}
}