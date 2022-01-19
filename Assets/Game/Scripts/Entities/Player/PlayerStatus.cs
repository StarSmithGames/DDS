using Game.Systems.TimeSystem;

using Zenject;

using static PlayerStates;

public class PlayerStatus : IStatus
{
	public bool IsAlive { get => isAlive; private set => isAlive = value; }
	private bool isAlive = true;

	public IInventory Inventory { get; private set; }
	public IStats Stats { get; private set; }
	public PlayerStates States { get; private set; }

	private UIManager uiManager;
	private TimeSystem timeSystem;

	public PlayerStatus(
		UIManager uiManager,
		TimeSystem timeSystem,
		IInventory inventory,
		IStats stats,
		PlayerStates states)
	{
		this.uiManager = uiManager;
		this.timeSystem = timeSystem;

		Inventory = inventory;
		Stats = stats;
		States = states;

		var uistats = uiManager.Status.Stats;

		uistats.Condition.SetAttribute(Stats.Condition);
		uistats.Stamina.SetAttribute(Stats.Stamina);
		uistats.Warmth.SetAttribute(Stats.Warmth);
		uistats.Fatigue.SetAttribute(Stats.Fatigue);
		uistats.Hungred.SetAttribute(Stats.Hungred);
		uistats.Thirst.SetAttribute(Stats.Thirst);

		timeSystem.AddEvent(new TimeEvent()
		{
			isInfinity = true,
			onTrigger = TimeTick,
			triggerTime = new Time() { TotalSeconds = 1}
		});
	}

	public void RestoreCondition(float value)
	{
		Stats.Condition.CurrentValue += value;
	}

	private void TimeTick()
	{
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

		ConditionFormules();
	}

	private void StandingFormule()
	{
		float mult = timeSystem.Settings.freaquanceTime.TotalSeconds;

		Stats.Fatigue.CurrentValue -= mult * (Stats.Fatigue.MaxValue / (8f * 3600f));//100% / 8h or 12.5%/h
		Stats.Thirst.CurrentValue -= mult * (Stats.Thirst.MaxValue / (8f * 3600f));//100% / 8h
		Stats.Hungred.CurrentValue -= mult * (125f / (1f * 3600f));//125f kcal/h
	}
	private void WalkingFormule()
	{
		float mult = timeSystem.Settings.freaquanceTime.TotalSeconds;

		Stats.Fatigue.CurrentValue -= mult * (Stats.Fatigue.MaxValue / (7f * 3600f));//100% / 7h
		Stats.Thirst.CurrentValue -= mult * (Stats.Thirst.MaxValue / (8f * 3600f));//100% / 8h or 12.5%/h
		Stats.Hungred.CurrentValue -= mult * (200f / (1f * 3600f));//200 cal/h
	}
	private void SprintingFormule()
	{
		float mult = timeSystem.Settings.freaquanceTime.TotalSeconds;

		Stats.Fatigue.CurrentValue -= mult * (Stats.Fatigue.MaxValue / (1f * 3600f));//100% / 1h
		Stats.Thirst.CurrentValue -= mult * (Stats.Thirst.MaxValue / (8f * 3600f));//100% / 8h or 12.5%/h
		Stats.Hungred.CurrentValue -= mult * (400f / (1f * 3600f));//400 cal/h
	}
	private void ClimbingFormule()
	{
		float mult = timeSystem.Settings.freaquanceTime.TotalSeconds;

		Stats.Fatigue.CurrentValue -= mult * (0);//100% / 1h
		Stats.Thirst.CurrentValue -= mult * (0);//100% / 8h or 12.5%/h
		Stats.Hungred.CurrentValue -= mult * (0);//400 cal/h
	}
	private void SleepingFormule()
	{
		float mult = timeSystem.Settings.freaquanceTime.TotalSeconds;

		Stats.Fatigue.CurrentValue += mult * (Stats.Fatigue.MaxValue / (12f * 3600f));//100% / 12h or ~8.33%/h
		Stats.Thirst.CurrentValue -= mult * (Stats.Thirst.MaxValue / (12f * 3600f));//100% / 12h or ~8.33%/h
		Stats.Hungred.CurrentValue -= mult * (75f / 3600f);//75 kcal/h
	}
	private void RestingFormule()
	{
		float mult = timeSystem.Settings.freaquanceTime.TotalSeconds;

		Stats.Fatigue.CurrentValue += mult * (Stats.Fatigue.MaxValue / (36f * 3600f));//100% / 36h
		Stats.Thirst.CurrentValue -= mult * (Stats.Thirst.MaxValue / (8f * 3600f));//100% / 8h or 12.5%/h
		Stats.Hungred.CurrentValue -= mult * (100f / (1f * 3600f));//100 kcal/h
	}

	private void ConditionFormules()
	{
		if (Stats.Warmth.CurrentValue == 0)
		{
			RestoreCondition(-(Stats.Condition.MaxValue * 4.5f) / 86400f);//-450.0%/d or ~18.75%/h
		}
		if (Stats.Fatigue.CurrentValue == 0)
		{
			RestoreCondition(-(Stats.Condition.MaxValue * 0.25f) / 86400f);//-25.0%/d or ~1.04%/h
		}
		if (Stats.Hungred.CurrentValue == 0)
		{
			RestoreCondition(-(Stats.Condition.MaxValue * 0.25f) / 86400f);//-25.0%/d or ~1.04%/h
		}
		if (Stats.Thirst.CurrentValue == 0)
		{
			RestoreCondition(-(Stats.Condition.MaxValue * 0.5f) / 86400f);//-50.0%/d or ~2.08%/h
		}
	}

	private void TemperatureFormules()
	{
		/*if (resistances.FeelsLike >= 0)
		{
			Stats.Warmth.CurrentValue += Stats.Warmth.MaxValue / (5f * 3600f);
		}
		else
		{
			if (resistances.TemperatureChevrone0 < resistances.FeelsLike)
			{
				Stats.Warmth.CurrentValue -= Stats.Warmth.MaxValue / (5f * 3600f);//100% / 5h
			}
			else if (resistances.TemperatureChevrone1 < resistances.FeelsLike && resistances.FeelsLike <= resistances.TemperatureChevrone0)
			{
				Stats.Warmth.CurrentValue -= Stats.Warmth.MaxValue / (1f * 3600f);//100% / 1h
			}
			else if (resistances.TemperatureChevrone2 < resistances.FeelsLike && resistances.FeelsLike <= resistances.TemperatureChevrone1)
			{
				Stats.Warmth.CurrentValue -= Stats.Warmth.MaxValue / (0.2f * 3600f);//100% / 0.2h
			}
			else
			{
				Stats.Warmth.CurrentValue -= Stats.Warmth.MaxValue / (900f);//100% / 10m
			}
		}*/
	}
}