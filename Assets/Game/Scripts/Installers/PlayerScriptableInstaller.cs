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

			Container.Bind<PlayerSettings>().FromInstance(settings).WhenInjectedInto<Player>();

			Container.Bind<Camera>().FromInstance(c).AsSingle();
			Container.Bind<Player>().FromInstance(p).AsSingle();
		}
	}
}