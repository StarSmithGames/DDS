using Game.Systems.EnvironmentSystem;

using Newtonsoft.Json.Linq;

using Sirenix.OdinInspector;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

public class Resistances : IModifiable
{
	private const int round = 1;

	//���� > 0 �������� �����
	//���� <= 0 ������ �����
	[ShowInInspector]
	public float FeelsLike => (float)Math.Round(AirFeels + WindFeels /*+ clothing*/ + ModifyValue, round);

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

	public float ModifyValue
		{
		get
		{
			float value = 0;
			for (int i = 0; i < Modifiers.Count; i++)
			{
				value += Modifiers[i].Value;
			}

			return (float)Math.Round(value, round);
		}
	}

	public float TemperatureChevrone0 => settings.temperatureChevrone0;
	public float TemperatureChevrone1 => settings.temperatureChevrone1;
	public float TemperatureChevrone2 => settings.temperatureChevrone2;

	public List<IModifier> Modifiers { get; private set; }

	private UIResistances uiResistances;
	private ResistancesSettings settings;

	public Resistances(SignalBus signalBus, ResistancesSettings settings, UIManager uiManager)
	{
		this.settings = settings;
		uiResistances = uiManager.Status.Resistances;

		Modifiers = new List<IModifier>();

		signalBus?.Subscribe<SignalWeatherChanged>(OnWeatherChanged);
		//signalBus?.Unsubscribe<SignalWeatherChanged>(OnWeatherChanged);
	}

	public void AddModifier(IModifier modifier)
	{
		if (!Modifiers.Contains(modifier))
		{
			Modifiers.Add(modifier);
		}
	}

	public void RemoveModifier(IModifier modifier)
	{
		if (Modifiers.Contains(modifier))
		{
			Modifiers.Remove(modifier);
		}
	}

	private void OnWeatherChanged(SignalWeatherChanged signal)
	{
		AirFeels =  signal.weather.air.airTemperature - (signal.weather.air.airTemperature * settings.airResistance / 100f);//Mathf.Clamp(, -100f, 50f);
		WindFeels = Mathf.Min(signal.weather.wind.windchill - (signal.weather.wind.windchill * settings.windResistance / 100f), 0);

		uiResistances.FeelsLike.text = FeelsLike + SymbolCollector.CELSIUS;
		uiResistances.AirFeels.text = AirFeels + SymbolCollector.CELSIUS;
		uiResistances.WindFeels.text = WindFeels + SymbolCollector.CELSIUS;
		uiResistances.BonusesFeels.text = ModifyValue == 0 ? "-" : ModifyValue + SymbolCollector.CELSIUS;
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
	[InfoBox("�����")]
	public float temperatureChevrone0 = -5f;
	public float temperatureChevrone1 = -15f;
	public float temperatureChevrone2 = -25f;
}