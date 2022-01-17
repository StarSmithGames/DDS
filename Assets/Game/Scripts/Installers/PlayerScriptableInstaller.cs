using Game.Entities;

using UnityEngine;

using Zenject;

namespace Game.Installers
{
	[CreateAssetMenu(fileName = "PlayerInstaller", menuName = "Installers/PlayerInstaller")]
	public class PlayerScriptableInstaller : ScriptableObjectInstaller
	{
		[SerializeField] private PlayerSettings settings;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<InteractionHandler>().AsSingle();

			Player p = FindObjectOfType<Player>();
			Camera c = p.GetComponentInChildren<Camera>();

			Container.Bind<InventorySettings>().FromInstance(settings.inventory).WhenInjectedInto<Inventory>();
			Container.Bind<IInventory>().To<Inventory>().WhenInjectedInto<PlayerStatus>();

			Container.Bind<PlayerVitalsSettings>().FromInstance(settings.vitals).WhenInjectedInto<PlayerVitals>();
			Container.Bind<IVitals>().To<PlayerVitals>().WhenInjectedInto<PlayerStatus>();

			Container.Bind<IStatus>().To<PlayerStatus>().WhenInjectedInto<Player>();


			Container.Bind<Camera>().FromInstance(c).AsSingle();
			Container.Bind<Player>().FromInstance(p).AsSingle();
		}
	}
}