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

			Container.BindFactory<ConstructionBlueprint, IConstruction, ConstructionModel.Factory>().FromFactory<BuildingHandler.Factory>();

			Container.BindInstance(settings);

			Container.BindInterfacesAndSelfTo<BuildingHandler>().AsSingle();
		}
    }
}