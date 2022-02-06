using Sirenix.OdinInspector;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace Game.Systems.WeatherSystem
{
    public class WeatherSystem : IInitializable, ITickable, IDisposable
    {
        private WeatherSettings settings;

        public WeatherSystem(WeatherSettings settings)
        {
            this.settings = settings;
        }

        public void Initialize()
        {
        }

        public void Dispose()
        {
        }

        public void Tick()
        {
        }
    }

    [System.Serializable]
    public class WeatherSettings
    {
        public WeatherForecast forecast;

        public WeatherPresset pressetClear;
        public WeatherPresset pressetSnowFall;
        public WeatherPresset pressetFog;
        public WeatherPresset pressetBlizzard;
    }

    [System.Serializable]
    public class WeatherForecast
	{
        [SerializeField] private int daysCount = 32;
        [ListDrawerSettings(NumberOfItemsPerPage = 5)]
        [SerializeField] private List<WeatherDayForecast> daysForecasts = new List<WeatherDayForecast>();
        public WeatherDayForecast this[int index] => daysForecasts[index % daysCount];

        [Title("Temperature")]
        [SerializeField] private Vector2 expectedTemperature = Vector2.zero;
        [Min(0)]
        [SerializeField] private Vector2 deviation;

        [SerializeField] private AnimationCurve expectedTemperatureCurve;

        [Title("Wind")]
        [SerializeField] private AnimationCurve expectedWindCurve;

        [Space]
        [SerializeField] private AnimationCurve temperatureDayCurve;
        [SerializeField] private AnimationCurve temperatureNightCurve;


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

            float minTemp = expectedTemperature.x;
            float maxTemp = expectedTemperature.y;
            float curveTimeStepNormalized = (float)1f / daysCount;

            Keyframe[] framesDay = new Keyframe[daysCount];
            Keyframe[] framesNight = new Keyframe[daysCount];
            for (int i = 0; i < daysCount; i++)
            {
                float stepCurve = curveTimeStepNormalized * i;

                WeatherDayForecast dayForecast = new WeatherDayForecast();

                float curveTemperature = Mathf.Lerp(minTemp, maxTemp, expectedTemperatureCurve.Evaluate(stepCurve));

                float curveWind = expectedWindCurve.Evaluate(stepCurve) * 120f;//km/h

                //temperatures
                float minTemperature = Mathf.Clamp(curveTemperature - UnityEngine.Random.Range(deviation.x, deviation.y), minTemp, maxTemp);
                float maxTemperature = Mathf.Clamp(curveTemperature + UnityEngine.Random.Range(deviation.x, deviation.y), minTemp, maxTemp);

                dayForecast.morning.SetTemperature(maxTemperature, WeatherWind.GetRandomWind(maxTemperature, curveWind));
                dayForecast.afternoon.SetTemperature(UnityEngine.Random.Range(minTemperature, maxTemperature), WeatherWind.GetRandomWind(maxTemperature, curveWind));
                dayForecast.evening.SetTemperature(UnityEngine.Random.Range(minTemperature, maxTemperature), WeatherWind.GetRandomWind(maxTemperature, curveWind));
                dayForecast.night.SetTemperature(minTemperature, WeatherWind.GetRandomWind(maxTemperature, curveWind));

                //curves
                framesDay[i] = new Keyframe(i, maxTemperature);
                framesNight[i] = new Keyframe(i, minTemperature);

                daysForecasts.Add(dayForecast);
            }
            //curves
            temperatureDayCurve = new AnimationCurve(framesDay);
            temperatureDayCurve.preWrapMode = WrapMode.Clamp;
            temperatureDayCurve.postWrapMode = WrapMode.Clamp;

            temperatureNightCurve = new AnimationCurve(framesNight);
            temperatureNightCurve.preWrapMode = WrapMode.Clamp;
            temperatureNightCurve.postWrapMode = WrapMode.Clamp;
        }

        [Button]
        private void ClearAll()
        {
            daysForecasts.Clear();
        }
    }

    [System.Serializable]
    public class WeatherDayForecast
	{
        public Forecast morning;
        public Forecast afternoon;
        public Forecast evening;
        public Forecast night;

        public WeatherDayForecast()
		{
            morning = new Forecast();
            afternoon = new Forecast();
            evening = new Forecast();
            night = new Forecast();
        }

        /// <summary>
        /// https://answers.unity.com/questions/1252260/lerp-color-between-4-corners.html
        /// </summary>
        public Weather GetWeatherByTime(float dayPercent)
        {
            Weather weatherTop = Weather.Lerp(morning.weather, afternoon.weather, dayPercent);
            Weather weatherBottom = Weather.Lerp(evening.weather, night.weather, dayPercent);

            return Weather.Lerp(weatherBottom, weatherTop, dayPercent);
        }

        public List<Weather> GetAllWeathers()
        {
            List<Weather> weathers = new List<Weather>();

            weathers.Add(morning.weather);
            weathers.Add(afternoon.weather);
            weathers.Add(evening.weather);
            weathers.Add(night.weather);

            return weathers;
        }
    }

    [System.Serializable]
    public class Forecast
    {
        [HideLabel]
        public Weather weather;

        public Forecast()
        {
            weather = new Weather();
            weather.air = new WeatherAir();
            weather.wind = new WeatherWind();
        }

        public void SetTemperature(WeatherAir air, WeatherWind wind)
        {
            weather.air = air;
            weather.wind = wind;
        }
        public void SetTemperature(float air, WeatherWind wind)
        {
            weather.air.airTemperature = air;
            weather.wind = wind;

            weather.humidity = UnityEngine.Random.Range(0f, 100f);
            weather.precipitation = UnityEngine.Random.Range(0f, 100f);
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
        [ReadOnly] [SerializeField] private float windAngle;
        [Space]
        [ReadOnly] [SerializeField] private WindSpeedType windSpeedType;
        [ReadOnly] [SerializeField] private WindDirectionType windDirectionType;

        public WeatherWind Lerp(WeatherWind from, WeatherWind to, float progress)
        {
            windchill = Mathf.Lerp(from.windchill, to.windchill, progress);
            WindSpeed = Mathf.Lerp(from.windSpeed, to.windSpeed, progress);
            WindDirection = Vector3.Lerp(from.windDirection, to.windDirection, progress);

            return this;
        }

        public static WeatherWind GetRandomWind(float temperature, float maxWindStrength)
        {
            WeatherWind wind = new WeatherWind();
            wind.WindDirection = UnityEngine.Random.insideUnitSphere.normalized;
            wind.WindSpeed = UnityEngine.Random.Range(0, maxWindStrength);

            //formule
            //https://tehtab.ru/Guide/GuideTricks/WindChillingEffect/
            float constanta = Mathf.Pow(wind.WindSpeed, 0.16f);
            float teff = 13.12f + (0.6215f * temperature) - (11.37f * constanta) + (0.3965f * temperature * constanta);
            wind.windchill = Mathf.Min(0, teff - temperature);

            return wind;
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