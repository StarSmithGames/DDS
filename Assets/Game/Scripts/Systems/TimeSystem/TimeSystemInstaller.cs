using Funly.SkyStudio;

using Sirenix.OdinInspector;

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

			Container.Bind<TimeOfDayController>().FromInstance(FindObjectOfType<TimeOfDayController>()).AsSingle();
			Container.BindInterfacesAndSelfTo<TimeSystem>().AsSingle();

			/*TimeSystem timeSystem = Container.Resolve<TimeSystem>();

			timeSystem.AddEvent(
				new TimeEvent()
				{
					triggetTime = settings.frequenceCycle,
					isInfinity = true,
					onTrigger = Container.Resolve<TimeOfDayController>().UpdateSkyForCurrentTime
				});*/
		}
	}
}