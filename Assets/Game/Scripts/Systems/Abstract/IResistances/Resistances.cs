using Game.Systems.EnvironmentSystem;

using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

public class Resistances : IResistances, IInitializable
{
	//если > 0 получаем тепло
	//если <= 0 теряет тепло
	[ShowInInspector]
	public float FeelsLike => AirFeels + WindFeels /*+ clothing*/ + BonusesFeels;

	public float AirFeels { get; set; }
	public float WindFeels { get; set; }
	public float BonusesFeels { get; set; }

	private SignalBus signalBus;

	public Resistances(SignalBus signalBus, ResistancesSettings settings)
	{
		this.signalBus = signalBus;

		signalBus?.Subscribe<SignalWeatherChanged>(OnWeatherChanged);
	}

	private void OnWeatherChanged(SignalWeatherChanged signal)
	{
		//Debug.LogError("Changed");
	}

	public void Initialize()
	{
		Debug.LogError("Initialize");
	}
}
[System.Serializable]
public class ResistancesSettings
{
	[Range(-100f, 100f)]
	public float airResistance = 0f;
	[Range(-100f, 100f)]
	public float windResistance = 0f;
}