using Game.Entities;

using Sirenix.OdinInspector;

using UnityEngine;

using Zenject;

public class PlayerVitals : Vitals
{
	public IStat Warmth { get; protected set; }
	public IStat Fatigue { get; protected set; }
	public IStat Hungred { get; protected set; }
	public IStat Thirst { get; protected set; }

	public PlayerVitals(UIManager uiManager, PlayerVitalsSettings settings)
	{
		Condition = new ConditionStat(settings.condtion, 100f);
		Stamina = new StaminaStat(settings.stamina, 100f);
		Warmth = new WarmthStat(settings.warmth, 100f);
		Fatigue = new FatigueStat(settings.fatigue, 100f);
		Hungred = new HungredStat(settings.hungred, 2500f);
		Thirst = new ThirstStat(settings.thrist, 100f);

		Debug.LogError(uiManager != null);
	}
}
[System.Serializable]
public class PlayerVitalsSettings
{
	[MinValue(0), MaxValue(100)]
	[SuffixLabel("%", true)]
	public float condtion = 100;
	[MinValue(0), MaxValue(100)]
	[SuffixLabel("%", true)]
	public float stamina = 100;
	[MinValue(0), MaxValue(100)]
	[SuffixLabel("%", true)]
	public float warmth = 100;
	[MinValue(0), MaxValue(100)]
	[SuffixLabel("%", true)]
	public float fatigue = 100;
	[MinValue(0), MaxValue(2500)]
	[SuffixLabel("kcal")]
	public int hungred = 2500;
	[MinValue(0), MaxValue(100)]
	[SuffixLabel("%", true)]
	public float thrist = 100;
}