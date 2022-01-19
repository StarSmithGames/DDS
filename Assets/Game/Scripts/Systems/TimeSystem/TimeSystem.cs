using Funly.SkyStudio;

using Sirenix.OdinInspector;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

using Zenject;

namespace Game.Systems.TimeSystem
{
	public class TimeSystem : IInitializable, IDisposable
	{
        public Time GlobalTime => globalTime;
        private Time globalTime;
        private List<TimeEvent> timeEvents = new List<TimeEvent>();

        public bool IsTimeProcess => timeCoroutine != null;
        private Coroutine timeCoroutine = null;
        public bool IsPaused => isPaused;
        private bool isPaused = false;

        public TimeSettings Settings => settings;

        private WaitForSeconds seconds;

        private UIManager uiManger;
        private AsyncManager asyncManager;
        private TimeSettings settings;

        public TimeSystem(
            UIManager uiManger,
            AsyncManager asyncManager,
            TimeSettings settings)
		{
            this.uiManger = uiManger;
            this.asyncManager = asyncManager;
            this.settings = settings;

            uiManger.OnDrawGUI += OnGUI;

            globalTime = settings.useRandom? settings.random.GetRandomStart() : settings.startTime;
            settings.freaquanceTime.ConvertSeconds();

            seconds = new WaitForSeconds(1f / settings.timeScale);
        }

        public void Initialize()
        {
            StartCycle();
        }

        public void Dispose()
		{
            StopCycle();
        }

		#region TimeCycle
		public void StartCycle()
		{
			if (!IsTimeProcess)
			{
                timeCoroutine = asyncManager.StartCoroutine(TimeCycle());
			}
		}
        private IEnumerator TimeCycle()
		{
			while (true)
			{
				while (isPaused)
				{
                    yield return null;
                }

                globalTime.TotalSeconds += settings.freaquanceTime.TotalSeconds;

                TryInvokeEvents();
                yield return seconds;
            }

            StopCycle();
        }
        public void PauseCycle()
		{
            isPaused = true;

        }
        public void UnPauseCycle()
		{
            isPaused = false;
        }
        private void StopCycle()
		{
            if (IsTimeProcess)
            {
                asyncManager.StopCoroutine(timeCoroutine);
                timeCoroutine = null;
            }
        }
		#endregion

        public void AddEvent(TimeEvent timeEvent)
		{
            timeEvents.Add(timeEvent);
        }
        public void RemoveEvent(TimeEvent timeEvent)
		{
			if (timeEvents.Contains(timeEvent))
			{
                timeEvents.Remove(timeEvent);
            }
        }
        private void TryInvokeEvents()
		{
            for (int i = timeEvents.Count -1 ; i >= 0 ; i--)
			{
				if (timeEvents[i].TryInvoke(globalTime))
				{
                    if (!timeEvents[i].isInfinity)
                    {
                        RemoveEvent(timeEvents[i]);
                    }
                }
			}
		}

        private void OnGUI()
        {
            GUI.Box(new Rect(150, 150, 100, 30), globalTime.ConvertTime().ToString());
        }
    }

    [System.Serializable]
    public class TimeSettings
	{
        [Tooltip("Time Scale = 1f / timeScale - “ечение времени относительно реального.")]
        [Min(0.0f)]
        public float timeScale = 12f;

        public bool useRandom = false;

        [ShowIf("useRandom")]
        public RandomTimeSettings random;

        [HideIf("useRandom")]
        public Time startTime;
        [Tooltip("Ќасколько будет мен€тс€ врем€ за один тик.")]
        public Time freaquanceTime;
    }
    [System.Serializable]
    public class RandomTimeSettings
	{
        public Time GetRandomStart()
		{
            Time time = new Time();
            time.Seconds = UnityEngine.Random.Range(0, 60);
            time.Minutes = UnityEngine.Random.Range(0, 60);
            time.Hours = UnityEngine.Random.Range(0, 23);
            return time;
		}
	}

    [System.Serializable]
    public struct Time
    {
        public int Days
		{
            get => days;
			set
			{
                days = value;
                ConvertSeconds();
            }
		}
        [OnValueChanged("ConvertSeconds")]
        [SuffixLabel("d", true)]
        [Min(0), MaxValue(24500)]
        [SerializeField] private int days;
        public int Hours
		{
            get => days;
			set
			{
                days = value;
                ConvertSeconds();
            }
		}
        [OnValueChanged("ConvertSeconds")]
        [SuffixLabel("h", true)]
        [Range(0, 24)]
        [SerializeField] private int hours;
        public int Minutes
		{
            get => minutes;
			set
			{
                minutes = value;
                ConvertSeconds();
            }
		}
        [OnValueChanged("ConvertSeconds")]
        [SuffixLabel("m", true)]
        [Range(0, 60)]
        [SerializeField] private int minutes;
        public int Seconds
		{
            get => seconds;
			set
			{
                seconds = value;
                ConvertSeconds();//может быть не очень
            }
		}
        [OnValueChanged("ConvertSeconds")]
        [SuffixLabel("s", true)]
        [Range(0, 60)]
        [SerializeField] private int seconds;

        [Space]
        [ReadOnly] [SerializeField] private TimeState currentState;
        public TimeState CurrentState
        {
            get
            {
                if (hours >= 6 && hours <= 12)
                {
                    return TimeState.Morning;
                }
                else if (hours > 12 && hours <= 17)
                {
                    return TimeState.Afternoon;
                }
                else if (hours > 17 && hours <= 20)
                {
                    return TimeState.Evening;
                }

                return TimeState.Night;
            }
        }

        [ReadOnly] [SerializeField] private float totalSeconds;//max 2147483647 ~ 24855 дней
        /// <summary>
        /// ѕепед тем как использовать надо убедитс€ что выполнен ConvertSeconds()
        /// </summary>
        public float TotalSeconds
        {
            get => totalSeconds;
            set => totalSeconds = value;
        }

        public int TotalMinutes => (int)totalSeconds / 60;
        public int TotalHours => TotalMinutes / 60;
        public int TotalDays => TotalHours / 24;

        public float CurrentDayPercent => (hours * 3600) / 86400f;
        public float DaysPercent => totalSeconds / 86400f;

        public static Time operator +(Time currTime, Time addTime)
        {
            currTime.totalSeconds += addTime.totalSeconds;

            return currTime;
        }
        public static Time operator -(Time currTime, Time addTime)
        {
            float result = currTime.totalSeconds;
            result -= addTime.totalSeconds;
            result = Mathf.Abs(result);
            currTime.totalSeconds = result;
            return currTime;
        }

        public static bool operator !=(Time time0, Time time1) => time0.totalSeconds != time1.totalSeconds;
        public static bool operator ==(Time time0, Time time1) => time0.totalSeconds == time1.totalSeconds;

        public static bool operator <=(Time time0, Time time1) => time0.totalSeconds <= time1.totalSeconds;
        public static bool operator >=(Time time0, Time time1) => time0.totalSeconds >= time1.totalSeconds;

        public static bool operator >(Time time0, Time time1) => time0.totalSeconds > time1.totalSeconds;
        public static bool operator <(Time time0, Time time1) => time0.totalSeconds < time1.totalSeconds;

        public static int operator %(Time time0, Time time1) => (int)time0.TotalSeconds % (int)time1.TotalSeconds;

        public override string ToString()
        {
            return IntToString(days) + ":" + IntToString(hours) + ":" + IntToString(minutes) + ":" + IntToString(seconds);
        }

        public string ToStringSimplification(bool showSecs = false, bool isInfinity = false)
        {
            string result = "";
            if (isInfinity)
            {
                result = SymbolCollector.INFINITY.ToString();
                return result;
            }


            if (days != 0) result += days + "D ";
            if (hours != 0) result += hours + "H ";
            if (minutes != 0) result += minutes + "M ";
            if (showSecs && seconds != 0) result += seconds + "S";

            result = result == "" ? SymbolCollector.DASH.ToString() : result;

            return result;
        }

        private string IntToString(int value)
		{
            return value <= 9 ? "0" + value : value.ToString();
        }

        public Time ConvertSeconds()
		{
            TimeSpan span = new TimeSpan(days, hours, minutes, seconds);
            totalSeconds = (int)span.TotalSeconds;

            currentState = CurrentState;

            return this;
        }
        public Time ConvertTime()
		{
            TimeSpan span = TimeSpan.FromSeconds(TotalSeconds);

            days = span.Days;
            hours = span.Hours;
            minutes = span.Minutes;
            seconds = span.Seconds;

            return this;
        }
    }

    [System.Serializable]
    public class TimeEvent
	{
        public UnityAction onTrigger;
        public Time triggerTime;
        public bool isInfinity = false;

        public bool TryInvoke(Time global)
		{
            if(triggerTime.TotalSeconds == 1)
			{
                onTrigger?.Invoke();
                return true;
            }
            else if (global % triggerTime == 0)
			{
                onTrigger?.Invoke();
                return true;
            }

            return false;
        }
    }

    public enum TimeState
    {
        Morning,
        Noon,
        Afternoon,
        Evening,
        Night,
    }
}