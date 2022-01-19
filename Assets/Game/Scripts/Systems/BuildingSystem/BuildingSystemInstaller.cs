using UnityEngine;
using Zenject;

namespace Game.Systems.BuildingSystem
{
    [CreateAssetMenu(menuName = "Installers/BuildingSystemInstaller", fileName = "BuildingSystemInstaller")]
    public class BuildingSystemInstaller : ScriptableObjectInstaller<BuildingSystemInstaller>
    {
		public BuildingSystemSettings settings;

		public override void InstallBindings()
		{
			Container.BindInstance(settings).WhenInjectedInto<BuildingSystem>();

			Container.BindInterfacesAndSelfTo<BuildingSystem>().AsSingle();
		}
    }
}