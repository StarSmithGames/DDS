using Game.Systems.TimeSystem;

using Sirenix.OdinInspector;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;


namespace Game.Systems.EnvironmentSystem
{
	[System.Serializable]
    public class WeatherForecast
	{
        [SerializeField] private int daysCount = 32;
        [ListDrawerSettings(NumberOfItemsPerPage = 5)]
        public List<ForecastDayWeather> daysForecasts = new List<ForecastDayWeather>();
        public ForecastDayWeather this[int index] => daysForecasts[index % daysCount];

        [HideInInspector] public List<Weather> weathers = new List<Weather>();

        [InlineProperty]
        [TabGroup("Temperature", AnimateVisibility = false)]
        [SerializeField] private ForecastTemperature temperature;
        [InlineProperty]
        [TabGroup("Wind", AnimateVisibility = false)]
        [SerializeField] private ForecastWind wind;
        [InlineProperty]
        [TabGroup("Humidity", AnimateVisibility = false)]
        [SerializeField] private ForecastHumidity humidity;
        [InlineProperty]
        [TabGroup("Precipitation", AnimateVisibility = false)]
        [SerializeField] private ForecastPrecipitation precipitation;

        public Weather GetWeather(float dayProgress)
        {
            int dayIndex = (int)dayProgress;
            float dayPercent = dayProgress - (float)dayIndex;

            ForecastDayWeather current = this[dayIndex];

            float progress = dayPercent * 24f;

            if (progress >= 0 && progress <= 6)
            {
                ForecastDayWeather prev = this[dayIndex - 1 < 0 ? 0 : dayIndex - 1];

                return Weather.Lerp(prev.night, current.morning, progress / 6f);
            }
            else if (progress > 6 && progress <= 12)
            {
                return Weather.Lerp(current.morning, current.afternoon, (progress - 6f) / 6f);
            }
            else if (progress > 12 && progress <= 18)
            {
                return Weather.Lerp(current.afternoon, current.evening, (progress - 12f) / 6f);
			}
			else
			{
                return Weather.Lerp(current.evening, current.night, (progress - 18f) / 6f);
            }
        }

        [Button]
        public void Generate()
        {
            ClearAll();

            temperature.Generate(daysCount);
            wind.Generate(daysCount, temperature.temperatureMinMax.x);
            humidity.Generate(daysCount);
            precipitation.Generate(daysCount);

            for (int i = 0; i < daysCount; i++)
			{
				daysForecasts.Add(CreateForecastDay(i));
			}

            for (int i = 0; i < daysForecasts.Count; i++)
            {
                weathers.AddRange(daysForecasts[i].GetAllWeathers());
            }
        }

        private ForecastDayWeather CreateForecastDay(int index)
		{
            ForecastDayWeather day = new ForecastDayWeather();

            day.morning = CreateWeather(temperature.mornings[index], wind.mornings[index], humidity.mornings[index], precipitation.mornings[index]);
            day.afternoon = CreateWeather(temperature.afternoons[index], wind.afternoons[index], humidity.afternoons[index], precipitation.afternoons[index]);
            day.evening = CreateWeather(temperature.evenings[index], wind.evenings[index], humidity.evenings[index], precipitation.evenings[index]);
            day.night = CreateWeather(temperature.nights[index], wind.nights[index], humidity.nights[index], precipitation.nights[index]);

            day.dayCurve.ReCreate(
                new List<Keyframe>()
                {
                    new Keyframe() { time = 0 },
                    new Keyframe() { time = 6 },
                    new Keyframe() { time = 12 },
                    new Keyframe() { time = 18 },
                    new Keyframe() { time = 24 },
                });

            return day;
        }

        private Weather CreateWeather(WeatherAir air, WeatherWind wind, WeatherHumidity humidity, WeatherPrecipitation precipitation)
        {
            return new Weather()
            {
                air = air,
                wind = wind,
                humidity = humidity,
                precipitation = precipitation,
            };
        }

        [Button]
        private void ClearAll()
        {
            daysForecasts.Clear();
            weathers.Clear();

            temperature.ClearAll();
            wind.ClearAll();
            humidity.ClearAll();
            precipitation.ClearAll();
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

    public class Forecast<T> where T : struct
	{
        [HideInInspector] public List<T> mornings = new List<T>();
        [HideInInspector] public List<T> afternoons = new List<T>();
        [HideInInspector] public List<T> evenings = new List<T>();
        [HideInInspector] public List<T> nights = new List<T>();

        public AnimationCurve expectedCurve;

        public virtual void ClearAll()
        {
            mornings.Clear();
            afternoons.Clear();
            evenings.Clear();
            nights.Clear();
        }

        [Button]
        private void ShowCurves()
		{
#if UNITY_EDITOR
            EnvironmentForecastWindow.SetForecast(this);
            EnvironmentForecastWindow.OpenWindow();
#endif
        }
    }

    [System.Serializable]
    public class ForecastTemperature : Forecast<WeatherAir>
    {
        [HideInInspector] public List<WeatherAir> airsOnDay = new List<WeatherAir>();

        public Vector2 temperatureMinMax = new Vector2(-60, 30);
        public Vector2 deviationMinMax = new Vector2(-6.66f, 6.66f);

        public void Generate(int daysCount)
        {
            ClearAll();

            float minTemp = temperatureMinMax.x;
            float maxTemp = temperatureMinMax.y;
            float curveTimeStepNormalized = (float)1f / daysCount;

            for (int i = 0; i < daysCount; i++)
            {
                float stepCurve = curveTimeStepNormalized * i;
                float curveTemperature = Mathf.Lerp(minTemp, maxTemp, expectedCurve.Evaluate(stepCurve));

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
        }

        public override void ClearAll()
        {
            airsOnDay.Clear();
            base.ClearAll();
        }
    }

    [System.Serializable]
    public class ForecastWind : Forecast<WeatherWind>
    {
        [HideInInspector] public List<WeatherWind> winds = new List<WeatherWind>();

        public void Generate(int daysCount, float maxTemperature)
        {
            ClearAll();

            float curveTimeStepNormalized = (float)1f / daysCount;

            for (int i = 0; i < daysCount; i++)
            {
                float stepCurve = curveTimeStepNormalized * i;
                float curveWind = expectedCurve.Evaluate(stepCurve) * 120f;//km/h

                mornings.Add(GetRandomWind(maxTemperature, curveWind));
                afternoons.Add(GetRandomWind(maxTemperature, curveWind));
                evenings.Add(GetRandomWind(maxTemperature, curveWind));
                nights.Add(GetRandomWind(maxTemperature, curveWind));

                winds.Add(mornings[mornings.Count - 1]);
                winds.Add(afternoons[afternoons.Count - 1]);
                winds.Add(evenings[evenings.Count - 1]);
                winds.Add(nights[nights.Count - 1]);
            }
        }

        public WeatherWind GetRandomWind(float maxTemperature, float maxWindStrength)
        {
            WeatherWind wind = new WeatherWind();

            wind.WindDirection = UnityEngine.Random.insideUnitSphere.normalized;
            wind.WindSpeed = UnityEngine.Random.Range(0, maxWindStrength);

            //formule
            //https://tehtab.ru/Guide/GuideTricks/WindChillingEffect/
            float constanta = Mathf.Pow(wind.WindSpeed, 0.16f);
            float teff = 13.12f + (0.6215f * maxTemperature) - (11.37f * constanta) + (0.3965f * maxTemperature * constanta);
            wind.windchill = Mathf.Min(0, teff - maxTemperature);

            return wind;
        }

        public override void ClearAll()
		{
            winds.Clear();
            base.ClearAll();
		}
	}

    [System.Serializable]
    public class ForecastHumidity : Forecast<WeatherHumidity>
	{
        [HideInInspector] public List<WeatherHumidity> humidities = new List<WeatherHumidity>();

        public void Generate(int daysCount)
        {
            ClearAll();

            float curveTimeStepNormalized = (float)1f / daysCount;

            for (int i = 0; i < daysCount; i++)
            {
                float stepCurve = curveTimeStepNormalized * i;
                //float curveWind = expectedCurve.Evaluate(stepCurve) * 100f;//%

				mornings.Add(new WeatherHumidity() { humidity = UnityEngine.Random.Range(0f, 100f) });
				afternoons.Add(new WeatherHumidity() { humidity = UnityEngine.Random.Range(0f, 0f) });
				evenings.Add(new WeatherHumidity() { humidity = UnityEngine.Random.Range(0f, 10f) });
				nights.Add(new WeatherHumidity() { humidity = UnityEngine.Random.Range(0f, 30f) });

				humidities.Add(mornings[mornings.Count - 1]);
				humidities.Add(afternoons[afternoons.Count - 1]);
				humidities.Add(evenings[evenings.Count - 1]);
				humidities.Add(nights[nights.Count - 1]);
			}
		}

        public override void ClearAll()
        {
            humidities.Clear();
            base.ClearAll();
        }
    }

    [System.Serializable]
    public class ForecastPrecipitation : Forecast<WeatherPrecipitation>
    {
        [HideInInspector] public List<WeatherPrecipitation> precipitations = new List<WeatherPrecipitation>();

        public void Generate(int daysCount)
		{
            ClearAll();

            float curveTimeStepNormalized = (float)1f / daysCount;

            for (int i = 0; i < daysCount; i++)
            {
                float stepCurve = curveTimeStepNormalized * i;
                float curveWind = expectedCurve.Evaluate(stepCurve) * 120f;//km/h

				mornings.Add(new WeatherPrecipitation().GetRandomPrecipitation());
				afternoons.Add(new WeatherPrecipitation().GetRandomPrecipitation());
				evenings.Add(new WeatherPrecipitation().GetRandomPrecipitation());
				nights.Add(new WeatherPrecipitation().GetRandomPrecipitation());

				precipitations.Add(mornings[mornings.Count - 1]);
                precipitations.Add(afternoons[afternoons.Count - 1]);
                precipitations.Add(evenings[evenings.Count - 1]);
                precipitations.Add(nights[nights.Count - 1]);
            }
		}

        public override void ClearAll()
		{
            precipitations.Clear();
			base.ClearAll();
		}
	}

    [System.Serializable]
    public class ForecastDayWeather
    {
        public Weather morning;
        public Weather afternoon;
        public Weather evening;
        public Weather night;

        public ForecastAnimationCurve dayCurve;

        /// <summary>
        /// https://answers.unity.com/questions/1252260/lerp-color-between-4-corners.html
        /// </summary>
        //public Weather GetWeatherByTime(float dayPercent)
        //{
        //    Weather weatherTop = Weather.Lerp(morning, afternoon, dayPercent);
        //    Weather weatherBottom = Weather.Lerp(evening, night, dayPercent);

        //    return Weather.Lerp(weatherBottom, weatherTop, dayPercent);
        //}

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
}