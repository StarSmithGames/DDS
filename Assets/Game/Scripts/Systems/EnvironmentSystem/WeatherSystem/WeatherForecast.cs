using Sirenix.OdinInspector;

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
        public List<ForecastDay> daysForecasts = new List<ForecastDay>();
        public ForecastDay this[int index] => daysForecasts[index % daysCount];

        [TabGroup("Temperature", AnimateVisibility = false)]
        [SerializeField] private ForecastTemperature temperature;
        [TabGroup("Wind", AnimateVisibility = false)]
        [SerializeField] private ForecastWind wind;
        [TabGroup("Humidity", AnimateVisibility = false)]
        [SerializeField] private ForecastHumidity humidity;
        [TabGroup("Precipitation", AnimateVisibility = false)]
        [SerializeField] private ForecastPrecipitation precipitation;

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
            humidity.Generate(daysCount);
            precipitation.Generate(daysCount);

            for (int i = 0; i < daysCount; i++)
			{
				daysForecasts.Add(CreateForecastDay(i));
			}
        }

        private ForecastDay CreateForecastDay(int index)
		{
            ForecastDay day = new ForecastDay();

            day.morning = CreateWeather(temperature.mornings[index], wind.mornings[index], humidity.mornings[index], precipitation.mornings[index]);
            day.afternoon = CreateWeather(temperature.afternoons[index], wind.afternoons[index], humidity.afternoons[index], precipitation.afternoons[index]);
            day.evening = CreateWeather(temperature.evenings[index], wind.evenings[index], humidity.evenings[index], precipitation.evenings[index]);
            day.night = CreateWeather(temperature.nights[index], wind.nights[index], humidity.nights[index], precipitation.nights[index]);

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
        [Space]
        public ForecastAnimationCurve dayCurve;
        public ForecastAnimationCurve morningCurve;
        public ForecastAnimationCurve nightCurve;

        public virtual void ClearAll()
        {
            mornings.Clear();
            afternoons.Clear();
            evenings.Clear();
            nights.Clear();

            dayCurve.Clear();
            morningCurve.Clear();
            nightCurve.Clear();
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

            dayCurve.ReCreate(airsOnDay.Select((x) => x.airTemperature).ToList());
            morningCurve.ReCreate(mornings.Select((x) => x.airTemperature).ToList());
            nightCurve.ReCreate(nights.Select((x) => x.airTemperature).ToList());
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

                mornings.Add(new WeatherWind().GetRandomWind(maxTemperature, curveWind));
                afternoons.Add(new WeatherWind().GetRandomWind(maxTemperature, curveWind));
                evenings.Add(new WeatherWind().GetRandomWind(maxTemperature, curveWind));
                nights.Add(new WeatherWind().GetRandomWind(maxTemperature, curveWind));

                winds.Add(mornings[mornings.Count - 1]);
                winds.Add(afternoons[afternoons.Count - 1]);
                winds.Add(evenings[evenings.Count - 1]);
                winds.Add(nights[nights.Count - 1]);
            }

            dayCurve.ReCreate(winds.Select((x) => x.windchill).ToList());
            morningCurve.ReCreate(mornings.Select((x) => x.windchill).ToList());
            nightCurve.ReCreate(nights.Select((x) => x.windchill).ToList());
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
                float curveWind = expectedCurve.Evaluate(stepCurve) * 120f;//km/h

				mornings.Add(new WeatherHumidity().GetRandomHumidity());
				afternoons.Add(new WeatherHumidity().GetRandomHumidity());
				evenings.Add(new WeatherHumidity().GetRandomHumidity());
				nights.Add(new WeatherHumidity().GetRandomHumidity());

				humidities.Add(mornings[mornings.Count - 1]);
				humidities.Add(afternoons[afternoons.Count - 1]);
				humidities.Add(evenings[evenings.Count - 1]);
				humidities.Add(nights[nights.Count - 1]);
			}

			dayCurve.ReCreate(humidities.Select((x) => x.humidity).ToList());
			morningCurve.ReCreate(mornings.Select((x) => x.humidity).ToList());
			nightCurve.ReCreate(nights.Select((x) => x.humidity).ToList());
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

			dayCurve.ReCreate(precipitations.Select((x) => x.precipitation).ToList());
			morningCurve.ReCreate(mornings.Select((x) => x.precipitation).ToList());
			nightCurve.ReCreate(nights.Select((x) => x.precipitation).ToList());
		}

        public override void ClearAll()
		{
            precipitations.Clear();
			base.ClearAll();
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

        public void Clear()
        {
            curve = new AnimationCurve();
        }
    }
}