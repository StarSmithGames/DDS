using UnityEngine;
using UnityEngine.Events;

public abstract class Stat : AttributeModifiable, IStat
{
	public event UnityAction onStatChanged;

	protected float currentValue;
	/// <summary>
	/// Текущее значение атрибутта.
	/// </summary>
	public float CurrentValue
	{
		get => currentValue;
		set
		{
			currentValue = Mathf.Clamp(value, 0, MaxValue);

			ValueChanged();
		}
	}

	/// <summary>
	/// Максимальное базовое значение атрибутта.
	/// </summary>
	public float MaxBaseValue { get; set; }

	/// <summary>
	/// Максимальное значение атрибутта со всеми плюсами и минусами.
	/// </summary>
	public float MaxValue => MaxBaseValue + ModifyValue;

	public float PercentValue => CurrentValue / MaxValue;

	public Stat(float currentValue, float maxBaseValue) : base()
	{
		this.currentValue = currentValue;
		MaxBaseValue = maxBaseValue;
	}

	//public Stat(Data data)
	//{
	//	this.currentValue = data.currentValue;
	//	MaxBaseValue = data.maxBaseValue;
	//}

	protected override void ValueChanged()
	{
		base.ValueChanged();

		onStatChanged?.Invoke();
	}
}

public class BaseStat : Stat
{
	public BaseStat(float currentValue, float maxBaseValue) : base(currentValue, maxBaseValue) { }

	public override string ToString()
	{
		return CurrentValue + "/" + MaxValue;
	}
}


public class StaminaStat : BaseStat
{
	public StaminaStat(float currentValue, float maxBaseValue) : base(currentValue, maxBaseValue) { }
	//public StaminaStat(Stat.Data data) : base(data) { }
}
public class ConditionStat : BaseStat
{
	public ConditionStat(float currentValue, float maxBaseValue) : base(currentValue, maxBaseValue) { }
	//public ConditionStat(Stat.Data data) : base(data) { }
}
public class WarmthStat : BaseStat
{
	public State state = State.Warm;
	public WarmthStat(float currentValue, float maxBaseValue) : base(currentValue, maxBaseValue) { }
	//public WarmthStat(Stat.Data data) : base(data) { }


	public enum State
	{
		Warm,
		Chilled,
		Cold,
		Numb,
		Freezing,
	}
}
public class FatigueStat : BaseStat
{
	public State state = State.Rested;
	public FatigueStat(float currentValue, float maxBaseValue) : base(currentValue, maxBaseValue) { }
	//public FatigueStat(Stat.Data data) : base(data) { }


	public enum State
	{
		Rested,
		Winded,
		Tired,
		Drained,
		Exhausted,
	}
}
public class HungredStat : BaseStat
{
	public State state = State.Full;
	public HungredStat(float currentValue, float maxBaseValue) : base(currentValue, maxBaseValue) { }
	//public HungredStat(Stat.Data data) : base(data) { }


	public enum State
	{
		Full,
		Peckish,
		Hungry,
		Ravenous,
		Starving,
	}
}
public class ThirstStat : BaseStat
{
	public State state = State.Slaked;
	public ThirstStat(float currentValue, float baseValue) : base(currentValue, baseValue) { }
	//public ThirstStat(Stat.Data data) : base(data) { }


	public enum State
	{
		Slaked,
		DryMouth,
		Thirsty,
		Parched,
		Dehydrated,
	}
}