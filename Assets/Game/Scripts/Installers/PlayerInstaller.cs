using Game.Entities;
using Game.Systems.InventorySystem;

using UnityEngine;

using Zenject;

namespace Game.Installers
{
	[CreateAssetMenu(fileName = "PlayerInstaller", menuName = "Installers/PlayerInstaller")]
	public class PlayerInstaller : ScriptableObjectInstaller<PlayerInstaller>
	{
		[SerializeField] private PlayerSettings settings;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<InteractionHandler>().AsSingle();

			Player p = FindObjectOfType<Player>();
			Camera c = p.GetComponentInChildren<Camera>();

			BindPlayerStatus();

			Container.Bind<Camera>().FromInstance(c).AsSingle();
			Container.Bind<Player>().FromInstance(p).AsSingle();
		}

		private void BindPlayerStatus()
		{
			Container.DeclareSignal<SignalPlayerStateChanged>();

			Container.Bind<PlayerStates>().WhenInjectedInto<PlayerStatus>();

			Container.BindInstance(settings.inventory).WhenInjectedInto<Inventory>();
			Container.BindInterfacesTo<Inventory>().WhenInjectedInto<PlayerStatus>();

			Container.BindInstance(settings.stats).WhenInjectedInto<Stats>();
			Container.BindInterfacesTo<Stats>().WhenInjectedInto<PlayerStatus>();

			Container.BindInstance(settings.resistances).WhenInjectedInto<Resistances>();
			Container.BindInterfacesTo<Resistances>().AsSingle();

			Container.BindInterfacesTo<PlayerStatus>().WhenInjectedInto<Player>();
		}
	}
}