using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization.Editor;
using Game.Systems.EnvironmentSystem;
using UnityEditor;
using System.Linq;

public class EnvironmentForecastWindow : OdinEditorWindow
{
	private ForecastAnimationCurve expectedCurve;
	private ForecastAnimationCurve dayCurve;
    private ForecastAnimationCurve morningCurve;
    private ForecastAnimationCurve nightCurve;

    private static object forecast;

    public static void OpenWindow()
	{
        EnvironmentForecastWindow window = GetWindow<EnvironmentForecastWindow>("Forecast", true);
        window.minSize = new Vector2(500, 100);

        window.ClearAll();
        window.Show();

        window.OnClose -= window.ClearAll;
        window.OnClose += window.ClearAll;
        window.SetData();
    }

    public static void SetForecast<T>(Forecast<T> forecast) where T : struct
	{
		EnvironmentForecastWindow.forecast = forecast;
    }

    public void SetData()
	{
		if (forecast is ForecastTemperature temperature)
		{
            expectedCurve.ReCreate(temperature.expectedCurve);
			dayCurve.ReCreate(temperature.airsOnDay.Select((x) => x.airTemperature).ToList());
			morningCurve.ReCreate(temperature.mornings.Select((x) => x.airTemperature).ToList());
			nightCurve.ReCreate(temperature.nights.Select((x) => x.airTemperature).ToList());
		}
		else if (forecast is ForecastWind wind)
		{
            expectedCurve.ReCreate(wind.expectedCurve);
            dayCurve.ReCreate(wind.winds.Select((x) => x.windchill).ToList());
			morningCurve.ReCreate(wind.mornings.Select((x) => x.windchill).ToList());
			nightCurve.ReCreate(wind.nights.Select((x) => x.windchill).ToList());
		}
		else if (forecast is ForecastHumidity humidity)
		{
            expectedCurve.ReCreate(humidity.expectedCurve);
            dayCurve.ReCreate(humidity.humidities.Select((x) => x.humidity).ToList());
			morningCurve.ReCreate(humidity.mornings.Select((x) => x.humidity).ToList());
			nightCurve.ReCreate(humidity.nights.Select((x) => x.humidity).ToList());
		}
		else if (forecast is ForecastPrecipitation precipitation)
		{
            expectedCurve.ReCreate(precipitation.expectedCurve);
            dayCurve.ReCreate(precipitation.precipitations.Select((x) => x.precipitation).ToList());
			morningCurve.ReCreate(precipitation.mornings.Select((x) => x.precipitation).ToList());
			nightCurve.ReCreate(precipitation.nights.Select((x) => x.precipitation).ToList());
		}
    }


    protected override void OnGUI()
	{
        EditorGUILayout.CurveField("Expected", expectedCurve.curve);
        EditorGUILayout.Space();
        EditorGUILayout.CurveField("Day", dayCurve.curve);
        EditorGUILayout.CurveField("Morning", morningCurve.curve);
        EditorGUILayout.CurveField("Night", nightCurve.curve);
    }

	public void ClearAll()
	{
        expectedCurve.Clear();
        dayCurve.Clear();
        morningCurve.Clear();
        nightCurve.Clear();
    }
}

[InlineProperty]
[System.Serializable]
public struct ForecastAnimationCurve
{
    [HideLabel]
    public AnimationCurve curve;

    public void ReCreate(AnimationCurve curve)
	{
        ReCreate(curve.keys.ToList());
    }

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