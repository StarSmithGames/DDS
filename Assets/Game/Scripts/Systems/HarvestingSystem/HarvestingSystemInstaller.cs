using UnityEngine;
using Zenject;

namespace Game.Systems.HarvestingSystem
{
    [CreateAssetMenu(menuName = "Installers/HarvestingSystemInstaller", fileName = "HarvestingSystemInstaller")]
    public class HarvestingSystemInstaller : ScriptableObjectInstaller<HarvestingSystemInstaller>
    {
		[SerializeField] private UIYieldItem yieldItemPrefab;

		public override void InstallBindings()
		{
			Container.BindFactory<UIYieldItem, UIYieldItem.Factory>()
					.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(3)
					.FromComponentInNewPrefab(yieldItemPrefab));

			Container.BindInterfacesAndSelfTo<HarvestingHandler>().AsSingle();
		}
	}
}