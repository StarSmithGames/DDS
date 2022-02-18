using UnityEngine;
using Zenject;

namespace Game.Systems.HarvestingSystem
{
    [CreateAssetMenu(menuName = "Installers/HarvestingSystemInstaller", fileName = "HarvestingSystemInstaller")]
    public class HarvestingSystemInstaller : ScriptableObjectInstaller<HarvestingSystemInstaller>
    {
		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<HarvestingHandler>().AsSingle();
		}
	}
}