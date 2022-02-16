using Game.Systems.EnvironmentSystem;

using Sirenix.OdinInspector;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

public class Resistances : IInitializable, IDisposable
{
	private const int round = 1;

	//если > 0 получаем тепло
	//если <= 0 тер€ет тепло
	[ShowInInspector]
	public float FeelsLike => (float)Math.Round(AirFeels + WindFeels /*+ clothing*/ + BonusesFeels, round);

	public float AirFeels
	{
		get => airFeels;
		set
		{
			airFeels = (float)Math.Round(value, round);
		}
	}
	private float airFeels;
	public float WindFeels
	{
		get => windFeels;
		set
		{
			windFeels = (float)Math.Round(value, round);
		}
	}
	private float windFeels;
	public float BonusesFeels
	{
		get => bonusesFeels;
		set
		{
			bonusesFeels = (float)Math.Round(value, round);
		}
	}
	private float bonusesFeels;

	public float TemperatureChevrone0 => settings.temperatureChevrone0;
	public float TemperatureChevrone1 => settings.temperatureChevrone1;
	public float TemperatureChevrone2 => settings.temperatureChevrone2;


	private UIResistances uiResistances;

	private SignalBus signalBus;
	private ResistancesSettings settings;

	public Resistances(SignalBus signalBus, ResistancesSettings settings, UIManager uiManager)
	{
		this.signalBus = signalBus;
		this.settings = settings;
		uiResistances = uiManager.Status.Resistances;
		Debug.LogError("Construct");
	}

	public void Initialize()
	{
		Debug.LogError("Initialize");
		signalBus?.Subscribe<SignalWeatherChanged>(OnWeatherChanged);
	}

	public void Dispose()
	{
		signalBus?.Unsubscribe<SignalWeatherChanged>(OnWeatherChanged);
		Debug.LogError("Dispose");
	}

	private void OnWeatherChanged(SignalWeatherChanged signal)
	{
		AirFeels =  signal.weather.air.airTemperature - (signal.weather.air.airTemperature * settings.airResistance / 100f);//Mathf.Clamp(, -100f, 50f);
		WindFeels = Mathf.Min(signal.weather.wind.windchill - (signal.weather.wind.windchill * settings.windResistance / 100f), 0);

		uiResistances.FeelsLike.text = FeelsLike + SymbolCollector.CELSIUS;
		uiResistances.AirFeels.text = AirFeels + SymbolCollector.CELSIUS;
		uiResistances.WindFeels.text = WindFeels + SymbolCollector.CELSIUS;
		uiResistances.BonusesFeels.text = BonusesFeels == 0 ? "-" : BonusesFeels + SymbolCollector.CELSIUS;
	}
}
[System.Serializable]
public class ResistancesSettings
{
	[SuffixLabel("%", true)]
	[Range(-100f, 100f)]
	public float airResistance = 0f;
	[SuffixLabel("%", true)]
	[Range(-100f, 100f)]
	public float windResistance = 0f;
	[InfoBox("ѕорог")]
	public float temperatureChevrone0 = -5f;
	public float temperatureChevrone1 = -15f;
	public float temperatureChevrone2 = -25f;
}