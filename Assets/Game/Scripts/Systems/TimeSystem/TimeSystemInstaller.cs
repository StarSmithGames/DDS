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

            Container.BindInterfacesAndSelfTo<TimeSystem>().AsSingle();
		}
	}

    [System.Serializable]
    public class TimeSettings
    {
        [Tooltip("Time Scale = 1f / timeScale - Течение времени относительно реального.")]
        [Min(0.0f)]
        public float timeScale = 12f;

        public bool useRandom = false;

        [ShowIf("useRandom")]
        public RandomTimeSettings random;

        [HideIf("useRandom")]
        public Time startTime;
        [Tooltip("Насколько будет менятся время за один тик.")]
        public Time freaquanceTime;
    }

    [System.Serializable]
    public class RandomTimeSettings
    {
        public Time GetRandomStart()
        {
            Time time = new Time();
            time.Seconds = Random.Range(0, 60);
            time.Minutes = Random.Range(0, 60);
            time.Hours = Random.Range(0, 23);
            return time;
        }
    }
}