using Sirenix.OdinInspector;
using Sirenix.Utilities;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using UnityEngine;
using Zenject;


namespace Game.Systems.WeatherSystem
{
    public class WeatherSystem : IInitializable, ITickable, IDisposable
    {
        private WeatherSettings settings;
        private FogController fogController;

        public WeatherSystem(WeatherSettings settings, FogController fogController)
        {
            this.settings = settings;
            this.fogController = fogController;
        }

        public void Initialize()
        {
            SetWeather(settings.pressetClear);
        }

        public void Dispose()
        {
        }

        public void Tick()
        {
        }

        public void SetWeather(WeatherPresset weather)
		{
            fogController.SetFog(weather.fog);
        }

        public void StartTransition(WeatherPresset weather)
		{
            fogController.StartTransition(weather.fog, 1f);
        }
    }


    [System.Serializable]
    public class WeatherForecast
	{
        [SerializeField] private int daysCount = 32;
        [ListDrawerSettings(NumberOfItemsPerPage = 5)]
        [SerializeField] private List<ForecastDay> daysForecasts = new List<ForecastDay>();
        public ForecastDay this[int index] => daysForecasts[index % daysCount];

        [Title("Temperature & Wind")]
        [SerializeField] private ForecastTemperature temperature;
        [SerializeField] private ForecastWind wind;

        public Weather GetWeatherByTime(float percent)
        {
            int dayIndex = (int)percent;
            float dayPercent = percent - dayIndex;

            return this[dayIndex].GetWeatherByTime(dayPercent);
        }

        public List<Weather> GetAllWeathers()
        {
            List<Weather> weathers = new List<Weather>();

            for (int i = 0; i < daysForecasts.Count; i++)
            {
                weathers.AddRange(daysForecasts[i].GetAllWeathers());
            }

            return weathers;
        }


        [Button]
        public void Generate()
        {
            ClearAll();

            temperature.Generate(daysCount);
            wind.Generate(daysCount, temperature.temperatureMinMax.x);

			for (int i = 0; i < daysCount; i++)
			{
                ForecastDay day = new ForecastDay();

				day.morning = new Weather().GetRandomWeather(temperature.mornings[i], wind.mornings[i]);
				day.afternoon = new Weather().GetRandomWeather(temperature.afternoons[i], wind.afternoons[i]);
				day.evening = new Weather().GetRandomWeather(temperature.evenings[i], wind.evenings[i]);
				day.night = new Weather().GetRandomWeather(temperature.nights[i], wind.nights[i]);

				daysForecasts.Add(day);
			}
        }

        [Button]
        private void ClearAll()
        {
            daysForecasts.Clear();
        }

        /// <summary>
        ///  Three-sigma rule
        ///  https://en.wikipedia.org/wiki/68%E2%80%9395%E2%80%9399.7_rule
        ///  https://answers.unity.com/questions/421968/normal-distribution-random.html
        /// </summary>
        public static float GetNormalDistribution(float min, float max, bool randomSign = false)
        {
            float mean = (min + max) / 2;
            float sigma = (max - mean) / 3;

            float result = UnityEngine.Random.Range(mean, sigma);

            return randomSign ? (UnityEngine.Random.Range(0, 100) >= 50 ? -1 * result : result) : result;
		}
	}
    [InlineProperty]
    [System.Serializable]
	public struct ForecastAnimationCurve
    {
        [HideLabel]
        public AnimationCurve curve;

        public void ReCreate(List<float> values)
		{
            List<Keyframe> frames = new List<Keyframe>();

			for (int i = 0; i < values.Count; i++)
			{
                frames.Add(new Keyframe(i, values[i]));
            }

            ReCreate(frames);
        }

        public void ReCreate(List<Keyframe> frames)
		{
            curve = new AnimationCurve(frames.ToArray());
            curve.preWrapMode = WrapMode.Clamp;
            curve.postWrapMode = WrapMode.Clamp;
        }
    }


	[System.Serializable]
    public class ForecastTemperature
    {
        [HideInInspector] public List<WeatherAir> airsOnDay = new List<WeatherAir>();

        [HideInInspector] public List<WeatherAir> mornings = new List<WeatherAir>();
        [HideInInspector] public List<WeatherAir> afternoons = new List<WeatherAir>();
        [HideInInspector] public List<WeatherAir> evenings = new List<WeatherAir>();
        [HideInInspector] public List<WeatherAir> nights = new List<WeatherAir>();

        public Vector2 temperatureMinMax;
        public Vector2 deviationMinMax;

        [SerializeField] private AnimationCurve expectedTemperatureCurve;
        [Space]
        [SerializeField] private ForecastAnimationCurve temperatureDayCurve;
        [SerializeField] private ForecastAnimationCurve temperatureMorningCurve;
        [SerializeField] private ForecastAnimationCurve temperatureNightCurve;

        public void Generate(int daysCount)
		{
            ClearAll();

            float minTemp = temperatureMinMax.x;
            float maxTemp = temperatureMinMax.y;
            float curveTimeStepNormalized = (float)1f / daysCount;

            for (int i = 0; i < daysCount; i++)
            {
                float stepCurve = curveTimeStepNormalized * i;
                float curveTemperature = Mathf.Lerp(minTemp, maxTemp, expectedTemperatureCurve.Evaluate(stepCurve));

                //temperatures
                float minTemperature = Mathf.Clamp(curveTemperature + WeatherForecast.GetNormalDistribution(deviationMinMax.x, deviationMinMax.y, true), minTemp, maxTemp);
                float maxTemperature = Mathf.Clamp(curveTemperature + WeatherForecast.GetNormalDistribution(deviationMinMax.x, deviationMinMax.y, true), minTemp, maxTemp);

                mornings.Add(new WeatherAir() { airTemperature = maxTemperature });
                afternoons.Add(new WeatherAir() { airTemperature = WeatherForecast.GetNormalDistribution(minTemperature, maxTemperature) });
                evenings.Add(new WeatherAir() { airTemperature = WeatherForecast.GetNormalDistribution(minTemperature, maxTemperature) });
                nights.Add(new WeatherAir() { airTemperature = minTemperature });

                airsOnDay.Add(mornings[mornings.Count - 1]);
                airsOnDay.Add(afternoons[afternoons.Count - 1]);
                airsOnDay.Add(evenings[evenings.Count - 1]);
                airsOnDay.Add(nights[nights.Count - 1]);
            }

            temperatureDayCurve.ReCreate(airsOnDay.Select((x) => x.airTemperature).ToList());
            temperatureMorningCurve.ReCreate(mornings.Select((x) => x.airTemperature).ToList());
            temperatureNightCurve.ReCreate(nights.Select((x) => x.airTemperature).ToList());
        }

        private void ClearAll()
        {
            airsOnDay.Clear();
            mornings.Clear();
            afternoons.Clear();
            evenings.Clear();
            nights.Clear();
        }
    }

    [System.Serializable]
    public class ForecastWind
	{
        [HideInInspector] public List<WeatherWind> winds = new List<WeatherWind>();

        [HideInInspector] public List<WeatherWind> mornings = new List<WeatherWind>();
        [HideInInspector] public List<WeatherWind> afternoons = new List<WeatherWind>();
        [HideInInspector] public List<WeatherWind> evenings = new List<WeatherWind>();
        [HideInInspector] public List<WeatherWind> nights = new List<WeatherWind>();

        [SerializeField] private AnimationCurve expectedWindCurve;
        [Space]
        [SerializeField] private ForecastAnimationCurve temperatureWindDayCurve;
        [SerializeField] private ForecastAnimationCurve temperatureWindMorningCurve;
        [SerializeField] private ForecastAnimationCurve temperatureWindNightCurve;

        public void Generate(int daysCount, float maxTemperature)
        {
            ClearAll();

            float curveTimeStepNormalized = (float)1f / daysCount;

            for (int i = 0; i < daysCount; i++)
            {
                float stepCurve = curveTimeStepNormalized * i;
                float curveWind = expectedWindCurve.Evaluate(stepCurve) * 120f;//km/h

                mornings.Add(new WeatherWind().GetRandomWind(maxTemperature, curveWind));
                afternoons.Add(new WeatherWind().GetRandomWind(maxTemperature, curveWind));
                evenings.Add(new WeatherWind().GetRandomWind(maxTemperature, curveWind));
                nights.Add(new WeatherWind().GetRandomWind(maxTemperature, curveWind));

                winds.Add(mornings[mornings.Count - 1]);
                winds.Add(afternoons[afternoons.Count - 1]);
                winds.Add(evenings[evenings.Count - 1]);
                winds.Add(nights[nights.Count - 1]);
            }

            temperatureWindDayCurve.ReCreate(winds.Select((x) => x.windchill).ToList());
            temperatureWindMorningCurve.ReCreate(mornings.Select((x) => x.windchill).ToList());
            temperatureWindNightCurve.ReCreate(nights.Select((x) => x.windchill).ToList());
        }

        private void ClearAll()
        {
            winds.Clear();
            mornings.Clear();
            afternoons.Clear();
            evenings.Clear();
            nights.Clear();
        }
    }

    [System.Serializable]
    public class ForecastDay
	{
        public Weather morning;
        public Weather afternoon;
        public Weather evening;
        public Weather night;

		/// <summary>
		/// https://answers.unity.com/questions/1252260/lerp-color-between-4-corners.html
		/// </summary>
		public Weather GetWeatherByTime(float dayPercent)
        {
            Weather weatherTop = Weather.Lerp(morning, afternoon, dayPercent);
            Weather weatherBottom = Weather.Lerp(evening, night, dayPercent);

            return Weather.Lerp(weatherBottom, weatherTop, dayPercent);
        }

        public List<Weather> GetAllWeathers()
        {
            List<Weather> weathers = new List<Weather>();

            weathers.Add(morning);
            weathers.Add(afternoon);
            weathers.Add(evening);
            weathers.Add(night);

            return weathers;
        }
    }


    [System.Serializable]
    public struct Weather 
    {
        [Tooltip("Воздух")]
        public WeatherAir air;
        [Tooltip("Ветер")]
        public WeatherWind wind;

        [Range(0, 100f)]
        [Tooltip("Осадки")]
        public float precipitation;
        [Range(0, 100f)]
        [Tooltip("Влажность")]
        public float humidity;

        public float Temperature => air.airTemperature + wind.windchill;

        public static Weather Lerp(Weather from, Weather to, float progress)
        {
            Weather weather = new Weather();

            weather.air = weather.air.Lerp(from.air, to.air, progress);
            weather.wind = weather.wind.Lerp(from.wind, to.wind, progress);
            weather.humidity = Mathf.Lerp(from.humidity, to.humidity, progress);

            return weather;
        }

        public Weather GetRandomWeather(WeatherAir air, WeatherWind wind)
		{
            this.air = air;
            this.wind = wind;

            humidity = UnityEngine.Random.Range(0f, 100f);
            precipitation = UnityEngine.Random.Range(0f, 100f);

            return this;
        }
    }

	[InlineProperty]
    [System.Serializable]
    public struct WeatherAir
    {
        //Antarctida -90 , 20
        //Alaska -63 , 38
        [Range(-100f, 55f)]
        [Tooltip("Температура воздуха")]
        public float airTemperature;

        public WeatherAir Lerp(WeatherAir from, WeatherAir to, float progress)
        {
            airTemperature = Mathf.Lerp(from.airTemperature, to.airTemperature, progress);

            return this;
        }
    }

    [InlineProperty]
    [System.Serializable]
    public struct WeatherWind
    {
        [Space]
        [Range(-100f, 0)]
        [Tooltip("Ветер")]
        public float windchill;
        [Range(0, 120f)]
        [Tooltip("Скорость ветра")]
        [SerializeField] private float windSpeed;
        public float WindSpeed
        {
            get => windSpeed;
            set
            {
                windSpeed = Mathf.Clamp(value, 0, 120f);

                CheckWindSpeed();
            }
        }

        [Tooltip("Направление ветра")]
        [OnValueChanged("CheckWindDirection")]
        [MinValue(-1), MaxValue(1)]
        [SerializeField] private Vector3 windDirection;
        public Vector3 WindDirection
		{
            get => windDirection;
			set
			{
                windDirection = value;

                CheckWindDirection();
            }
		}

        [Space]
        [Sirenix.OdinInspector.ReadOnly] [SerializeField] private float windAngle;
        [Space]
        [Sirenix.OdinInspector.ReadOnly] [SerializeField] private WindSpeedType windSpeedType;
        [Sirenix.OdinInspector.ReadOnly] [SerializeField] private WindDirectionType windDirectionType;

        public WeatherWind Lerp(WeatherWind from, WeatherWind to, float progress)
        {
            windchill = Mathf.Lerp(from.windchill, to.windchill, progress);
            WindSpeed = Mathf.Lerp(from.windSpeed, to.windSpeed, progress);
            WindDirection = Vector3.Lerp(from.windDirection, to.windDirection, progress);

            return this;
        }

        public WeatherWind GetRandomWind(float maxTemperature, float maxWindStrength)
        {
            WindDirection = UnityEngine.Random.insideUnitSphere.normalized;
            WindSpeed = UnityEngine.Random.Range(0, maxWindStrength);

            //formule
            //https://tehtab.ru/Guide/GuideTricks/WindChillingEffect/
            float constanta = Mathf.Pow(WindSpeed, 0.16f);
            float teff = 13.12f + (0.6215f * maxTemperature) - (11.37f * constanta) + (0.3965f * maxTemperature * constanta);
            windchill = Mathf.Min(0, teff - maxTemperature);

            return this;
        }

        private void CheckWindDirection()
        {
            windAngle = Vector3.Angle(Vector3.forward, windDirection);

            if (windAngle >= 0 && windAngle <= 22.5f)
            {
                windDirectionType = WindDirectionType.N;
            }
            else if (windAngle > 22.5f && windAngle <= 45f)
            {
                windDirectionType = WindDirectionType.NNE;
            }
            else if (windAngle > 45f && windAngle <= 67.5f)
            {
                windDirectionType = WindDirectionType.NE;
            }
            else if (windAngle > 67.5f && windAngle <= 90f)
            {
                windDirectionType = WindDirectionType.ENE;
            }
            else if (windAngle > 90f && windAngle <= 112.5f)
            {
                windDirectionType = WindDirectionType.E;
            }
            else if (windAngle > 112.5f && windAngle <= 135f)
            {
                windDirectionType = WindDirectionType.ESE;
            }
            else if (windAngle > 135f && windAngle <= 157.5f)
            {
                windDirectionType = WindDirectionType.SE;
            }
            else if (windAngle > 157.5f && windAngle <= 180f)
            {
                windDirectionType = WindDirectionType.SSE;
            }
            else if (windAngle > 180f && windAngle <= 202.5f)
            {
                windDirectionType = WindDirectionType.S;
            }
            else if (windAngle > 202.5f && windAngle <= 225f)
            {
                windDirectionType = WindDirectionType.SSW;
            }
            else if (windAngle > 225f && windAngle <= 247.5f)
            {
                windDirectionType = WindDirectionType.SW;
            }
            else if (windAngle > 247.5f && windAngle <= 270f)
            {
                windDirectionType = WindDirectionType.WSW;
            }
            else if (windAngle > 270f && windAngle <= 292.5f)
            {
                windDirectionType = WindDirectionType.W;
            }
            else if (windAngle > 292.5f && windAngle <= 315f)
            {
                windDirectionType = WindDirectionType.WNW;
            }
            else if (windAngle > 315f && windAngle <= 337.5f)
            {
                windDirectionType = WindDirectionType.NW;
            }
            else if (windAngle > 337.5f && windAngle <= 360f)
            {
                windDirectionType = WindDirectionType.NNW;
            }
            else
            {
                Debug.LogError("ERROR WIND DIRECTION");
            }
        }
        private void CheckWindSpeed()
        {
            if (windSpeed <= 2f)
            {
                windSpeedType = WindSpeedType.Calm;
            }
            else if (windSpeed > 2f && windSpeed <= 5f)
            {
                windSpeedType = WindSpeedType.LightAir;
            }
            else if (windSpeed > 5f && windSpeed <= 11f)
            {
                windSpeedType = WindSpeedType.LightBreeze;
            }
            else if (windSpeed > 11f && windSpeed <= 19f)
            {
                windSpeedType = WindSpeedType.GentleBreeze;
            }
            else if (windSpeed > 19f && windSpeed <= 28f)
            {
                windSpeedType = WindSpeedType.ModerateBreeze;
            }
            else if (windSpeed > 28f && windSpeed <= 39f)
            {
                windSpeedType = WindSpeedType.FreshBreeze;
            }
            else if (windSpeed > 39f && windSpeed <= 49f)
            {
                windSpeedType = WindSpeedType.StrongBreeze;
            }
            else if (windSpeed > 49f && windSpeed <= 61f)
            {
                windSpeedType = WindSpeedType.NearGale;
            }
            else if (windSpeed > 61f && windSpeed <= 74f)
            {
                windSpeedType = WindSpeedType.Gale;
            }
            else if (windSpeed > 74f && windSpeed <= 88f)
            {
                windSpeedType = WindSpeedType.StrongGale;
            }
            else if (windSpeed > 88f && windSpeed <= 102f)
            {
                windSpeedType = WindSpeedType.Storm;
            }
            else if (windSpeed > 102f && windSpeed <= 117f)
            {
                windSpeedType = WindSpeedType.ViolentStorm;
            }
            else
            {
                windSpeedType = WindSpeedType.HurricaneForce;
            }
        }
    }

    public enum WeatherType
    {
        Clear,
        Aurora,
        Cloudy,
        Fog,
        Snowfall,
        Blizzard,
    }
    public enum FogType
    {
        None,
        Light,
        Medium,
        Heavy,
    }

    public enum WindDirectionType
    {
        N,
        NNE,
        NE,
        ENE,
        E,
        ESE,
        SE,
        SSE,
        S,
        SSW,
        SW,
        WSW,
        W,
        WNW,
        NW,
        NNW,
    }

    /// <summary>
    /// Beaufort number types 0-12
    /// </summary>
    public enum WindSpeedType : int
    {
        Calm = 0,
        LightAir = 1,
        LightBreeze = 2,
        GentleBreeze = 3,
        ModerateBreeze = 4,
        FreshBreeze = 5,
        StrongBreeze = 6,
        NearGale = 7,
        Gale = 8,
        StrongGale = 9,
        Storm = 10,
        ViolentStorm = 11,
        HurricaneForce = 12,
    }
}