using Game.Systems.EnvironmentSystem;

using Sirenix.OdinInspector;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Assertions;

using Zenject;

namespace Game.Systems.EnvironmentSystem
{
	public class WeatherSystem : IInitializable, IDisposable
    {
        private Weather currentWeather;

        private SignalBus signalBus;
        private WeatherSettings settings;
        private TimeSystem.TimeSystem timeSystem;
        private WindController windController;
        private FogController fogController;

        public WeatherSystem(SignalBus signalBus, WeatherSettings settings, TimeSystem.TimeSystem timeSystem, WindController windController, FogController fogController)
        {
            this.signalBus = signalBus;
            this.settings = settings;
            this.timeSystem = timeSystem;
            this.windController = windController;
            this.fogController = fogController;
        }

        public void Initialize()
        {
            SetWeather(settings.pressetClear);

            timeSystem.AddEvent(new TimeSystem.TimeEvent()
            {
                onTrigger = Tick,
                triggerTime = settings.freaquanceWeatherTime,
                isInfinity = true,
            });

            Assert.AreNotEqual(settings.forecast.daysForecasts.Count, 0);
        }

        public void Dispose() { }


        private void Tick()
		{
            currentWeather = settings.forecast.GetWeather(timeSystem.GlobalTime.DaysPercent);

            float fog = currentWeather.humidity.humidity / 100f;
            fogController.CurrentFogRange = fog;
            fogController.CurrentNormalRange = fog;
            windController.SetWindDirection(currentWeather.wind.WindDirection);

            signalBus?.Fire(new SignalWeatherChanged() { weather = currentWeather });
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
    public struct Weather 
    {
        [Tooltip("Воздух")]
        public WeatherAir air;
        [Tooltip("Ветер")]
        public WeatherWind wind;
        [Tooltip("Влажность")]
        public WeatherHumidity humidity;
        [Tooltip("Осадки")]
        public WeatherPrecipitation precipitation;

        public float Temperature => air.airTemperature + wind.windchill;

        public static Weather Lerp(Weather from, Weather to, float progress)
        {
            Weather weather = new Weather();

            weather.air = WeatherAir.Lerp(from.air, to.air, progress);
            weather.wind = WeatherWind.Lerp(from.wind, to.wind, progress);
            weather.humidity = WeatherHumidity.Lerp(from.humidity, to.humidity, progress);
            weather.precipitation = WeatherPrecipitation.Lerp(from.precipitation, to.precipitation, progress);

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

        public static WeatherAir Lerp(WeatherAir from, WeatherAir to, float progress)
        {
            WeatherAir weatherAir = new WeatherAir();

            weatherAir.airTemperature = Mathf.Lerp(from.airTemperature, to.airTemperature, progress);

            return weatherAir;
        }
    }

    [InlineProperty]
    [System.Serializable]
    public struct WeatherWind
    {
        [Range(-100f, 55f)]
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

        public static WeatherWind Lerp(WeatherWind from, WeatherWind to, float progress)
        {
            WeatherWind weatherWind = new WeatherWind();

            weatherWind.windchill = Mathf.Lerp(from.windchill, to.windchill, progress);
            weatherWind.WindSpeed = Mathf.Lerp(from.windSpeed, to.windSpeed, progress);
            weatherWind.WindDirection = Vector3.Lerp(from.windDirection, to.windDirection, progress);

            return weatherWind;
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

    [InlineProperty]
    [System.Serializable]
    public struct WeatherHumidity
	{
        [SuffixLabel("%")]
        [Range(0f, 100f)]
        [Tooltip("Влажность")]
        public float humidity;

        public static WeatherHumidity Lerp(WeatherHumidity from, WeatherHumidity to, float progress)
        {
            WeatherHumidity weatherHumidity = new WeatherHumidity();
            weatherHumidity.humidity = Mathf.Lerp(from.humidity, to.humidity, progress);
            return weatherHumidity;
        }
    }

    [InlineProperty]
    [System.Serializable]
    public struct WeatherPrecipitation
	{
        [SuffixLabel("%")]
        [Range(0, 100f)]
        [Tooltip("Осадки")]
        public float precipitation;

        public static WeatherPrecipitation Lerp(WeatherPrecipitation from, WeatherPrecipitation to, float progress)
        {
            WeatherPrecipitation weatherPrecipitation = new WeatherPrecipitation();
            weatherPrecipitation.precipitation = Mathf.Lerp(from.precipitation, to.precipitation, progress);

            return weatherPrecipitation;
        }

        public WeatherPrecipitation GetRandomPrecipitation()
        {
            precipitation = UnityEngine.Random.Range(0f, 100f);

            return this;
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


    [System.Serializable]
    public class WeatherSettings
    {
        public WeatherForecast forecast;

        [InlineButton("Test")]
        public WeatherPresset pressetClear;
        [InlineButton("Test")]
        public WeatherPresset pressetSnowFall;
        [InlineButton("Test")]
        public WeatherPresset pressetFog;
        [InlineButton("Test")]
        public WeatherPresset pressetMilk;
        [InlineButton("Test")]
        public WeatherPresset pressetAurora;
        [InlineButton("Test")]
        public WeatherPresset pressetCloudy;
        [InlineButton("Test")]
        public WeatherPresset pressetBlizzard;

        [Space]
        [Tooltip("Как часто будет обновляться погода за один тик.")]
        public TimeSystem.Time freaquanceWeatherTime = new TimeSystem.Time() { TotalSeconds = 1 };

        private void Test(WeatherPresset presset)
        {
#if UNITY_EDITOR
            var asset = AssetDatabaseExtensions.GetAsset<EnvironmentSystemInstaller>("Installers/EnvironmentSystemInstaller/EnvironmentSystemInstaller.asset");
            asset.SetWeather(presset);
#endif
        }
    }
}