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
			Container.DeclareSignal<SignalBuildingCancel>();
			Container.DeclareSignal<SignalBuildingBuild>();

			Container.BindFactory<ConstructionBlueprint, IConstruction, ConstructionModel.Factory>().FromFactory<BuildingSystem.Factory>();

			Container.BindInstance(settings);

			Container.BindInterfacesAndSelfTo<BuildingSystem>().AsSingle();
		}
    }
}