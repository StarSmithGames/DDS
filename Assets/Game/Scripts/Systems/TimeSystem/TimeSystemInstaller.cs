using UnityEngine;
using Zenject;

namespace Game.Systems.TimeSystem
{
	[CreateAssetMenu(menuName = "Installers/TimeSystemInstaller", fileName = "TimeSystemInstaller")]
    public class TimeSystemInstaller : ScriptableObjectInstaller<TimeSystemInstaller>
    {
		public TimeSettings settings;

		public override void InstallBindings()
		{
			Container.BindInstance(settings).WhenInjectedInto<TimeSystem>();

			Container.BindInterfacesAndSelfTo<TimeSystem>().AsSingle();
		}
	}
}