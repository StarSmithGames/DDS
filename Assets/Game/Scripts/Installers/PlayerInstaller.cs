using Game.Entities;
using Game.Systems.InventorySystem;

using System;
using System.ComponentModel;
using System.Linq;

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

			Container.BindInterfacesAndSelfTo<PlayerStates>().AsSingle();

			Container.BindInstance(settings.inventory).WhenInjectedInto<Inventory>();
			Container.BindInterfacesTo<Inventory>().AsSingle();

			Container.BindInstance(settings.stats).WhenInjectedInto<Stats>();
			Container.BindInterfacesTo<Stats>().AsSingle();

			Container.BindInstance(settings.resistances).WhenInjectedInto<Resistances>();
			Container.BindInterfacesAndSelfTo<Resistances>().AsSingle();

			Container.BindInterfacesTo<PlayerStatus>().WhenInjectedInto<Player>();
		}
	}
}