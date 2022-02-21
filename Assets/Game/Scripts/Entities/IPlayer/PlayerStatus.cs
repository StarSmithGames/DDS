using Game.Systems.InventorySystem;
using Game.Systems.TimeSystem;

using UnityEngine;

using Zenject;
using Zenject.ReflectionBaking.Mono.Cecil;

using static PlayerStates;

public class PlayerStatus : IStatus
{
	public bool IsAlive => isAlive;
	private bool isAlive = true;

	public IInventory Inventory { get; private set; }
	public IStats Stats { get; private set; }
	public Resistances Resistances { get; private set; }
	public PlayerStates States { get; private set; }

	private float multiplier;
	private TimeEvent tick;

	private SignalBus signalBus;
	private UIManager uiManager;
	private TimeSystem timeSystem;

	public PlayerStatus(
		SignalBus signalBus,
		UIManager uiManager,
		TimeSystem timeSystem,
		IInventory inventory,
		IStats stats,
		Resistances resistances,
		PlayerStates states)
	{
		this.signalBus = signalBus;
		this.uiManager = uiManager;
		this.timeSystem = timeSystem;

		Inventory = inventory;
		Stats = stats;
		Resistances = resistances;
		States = states;

		timeSystem.AddEvent(new TimeEvent()
		{
			isInfinity = true,
			onTrigger = TimeTick,
			triggerTime = new Game.Systems.TimeSystem.Time() { TotalSeconds = 1 }
		});
	}

	public void RestoreCondition(float value)
	{
		Stats.Condition.CurrentValue += value;

		if(Stats.Condition.CurrentValue == 0)
		{
			isAlive = false;
			signalBus?.Fire<SignalPlayerDied>();
		}
	}

	private void TimeTick()
	{
		multiplier = timeSystem.Settings.freaquanceTime.TotalSeconds;
		State currentState = States.CurrentState;

		if (currentState == State.Standing)
		{
			StandingFormule();
		}
		else if (currentState == State.Walking)
		{
			WalkingFormule();
		}
		else if(currentState == State.Sprinting)//TODO
		{
			SprintingFormule();
		}
		else if(currentState == State.Climbing)//TODO
		{
			ClimbingFormule();
		}
		else if (currentState == State.Resting)//TODO
		{
			RestingFormule();
		}
		else if(currentState == State.Sleeping)//TODO
		{
			SleepingFormule();
		}

		TemperatureFormules();

		ConditionFormules();
	}

	private void StandingFormule()
	{
		Stats.Fatigue.CurrentValue -= multiplier * (Stats.Fatigue.MaxValue / (8f * 3600f));//100% / 8h or 12.5%/h
		Stats.Thirst.CurrentValue -= multiplier * (Stats.Thirst.MaxValue / (8f * 3600f));//100% / 8h
		Stats.Hungred.CurrentValue -= multiplier * (125f / (1f * 3600f));//125f kcal/h
	}
	private void WalkingFormule()
	{
		Stats.Fatigue.CurrentValue -= multiplier * (Stats.Fatigue.MaxValue / (7f * 3600f));//100% / 7h
		Stats.Thirst.CurrentValue -= multiplier * (Stats.Thirst.MaxValue / (8f * 3600f));//100% / 8h or 12.5%/h
		Stats.Hungred.CurrentValue -= multiplier * (200f / (1f * 3600f));//200 cal/h
	}
	private void SprintingFormule()
	{
		Stats.Fatigue.CurrentValue -= multiplier * (Stats.Fatigue.MaxValue / (1f * 3600f));//100% / 1h
		Stats.Thirst.CurrentValue -= multiplier * (Stats.Thirst.MaxValue / (8f * 3600f));//100% / 8h or 12.5%/h
		Stats.Hungred.CurrentValue -= multiplier * (400f / (1f * 3600f));//400 cal/h
	}
	private void ClimbingFormule()
	{
		Stats.Fatigue.CurrentValue -= multiplier * (0);//100% / 1h
		Stats.Thirst.CurrentValue -= multiplier * (0);//100% / 8h or 12.5%/h
		Stats.Hungred.CurrentValue -= multiplier * (0);//400 cal/h
	}
	private void SleepingFormule()
	{
		Stats.Fatigue.CurrentValue += multiplier * (Stats.Fatigue.MaxValue / (12f * 3600f));//100% / 12h or ~8.33%/h
		Stats.Thirst.CurrentValue -= multiplier * (Stats.Thirst.MaxValue / (12f * 3600f));//100% / 12h or ~8.33%/h
		Stats.Hungred.CurrentValue -= multiplier * (75f / 3600f);//75 kcal/h
	}
	private void RestingFormule()
	{
		Stats.Fatigue.CurrentValue += multiplier * (Stats.Fatigue.MaxValue / (36f * 3600f));//100% / 36h
		Stats.Thirst.CurrentValue -= multiplier * (Stats.Thirst.MaxValue / (8f * 3600f));//100% / 8h or 12.5%/h
		Stats.Hungred.CurrentValue -= multiplier * (100f / (1f * 3600f));//100 kcal/h
	}

	private void TemperatureFormules()
	{
		if (Resistances.FeelsLike >= 0)
		{
			Stats.Warmth.CurrentValue += multiplier * (Stats.Warmth.MaxValue / (5f * 3600f));
		}
		else
		{
			if (Resistances.TemperatureChevrone0 < Resistances.FeelsLike)
			{
				Stats.Warmth.CurrentValue -= multiplier * (Stats.Warmth.MaxValue / (5f * 3600f));//100% / 5h
			}
			else if (Resistances.TemperatureChevrone1 < Resistances.FeelsLike && Resistances.FeelsLike <= Resistances.TemperatureChevrone0)
			{
				Stats.Warmth.CurrentValue -= multiplier * (Stats.Warmth.MaxValue / (2f * 3600f));//100% / 1h
			}
			else if (Resistances.TemperatureChevrone2 < Resistances.FeelsLike && Resistances.FeelsLike <= Resistances.TemperatureChevrone1)
			{
				Stats.Warmth.CurrentValue -= multiplier * (Stats.Warmth.MaxValue / (0.5f * 3600f));//100% / 0.5h
			}
			else
			{
				Stats.Warmth.CurrentValue -= multiplier * (Stats.Warmth.MaxValue / (900f));//100% / 10m
			}
		}
	}

	private void ConditionFormules()
	{
		if (Stats.Warmth.CurrentValue == 0)
		{
			RestoreCondition(-multiplier * (Stats.Condition.MaxValue * 4.5f) / 86400f);//-450.0%/d or ~18.75%/h
		}
		if (Stats.Fatigue.CurrentValue == 0)
		{
			RestoreCondition(-multiplier * (Stats.Condition.MaxValue * 0.25f) / 86400f);//-25.0%/d or ~1.04%/h
		}
		if (Stats.Hungred.CurrentValue == 0)
		{
			RestoreCondition(-multiplier * (Stats.Condition.MaxValue * 0.25f) / 86400f);//-25.0%/d or ~1.04%/h
		}
		if (Stats.Thirst.CurrentValue == 0)
		{
			RestoreCondition(-multiplier * (Stats.Condition.MaxValue * 0.5f) / 86400f);//-50.0%/d or ~2.08%/h
		}
	}
}