using Sirenix.OdinInspector;

using System.Collections.Generic;
using System.Linq;

public class Stats : IStats
{
	public IStat Condition { get; protected set; }
	public IStat Stamina { get; protected set; }
	public IStat Warmth { get; protected set; }
	public IStat Fatigue { get; protected set; }
	public IStat Hungred { get; protected set; }
	public IStat Thirst { get; protected set; }

	//private List<IStat> stats;

	public Stats(StatsSettings settings)
	{
		Condition = new ConditionStat(settings.condtion, 100f);
		Stamina = new StaminaStat(settings.stamina, 100f);
		Warmth = new WarmthStat(settings.warmth, 100f);
		Fatigue = new FatigueStat(settings.fatigue, 100f);
		Hungred = new HungredStat(settings.hungred, 2500f);
		Thirst = new ThirstStat(settings.thrist, 100f);

		//stats = new List<IStat>();
		//stats.Add(Condtion);
		//stats.Add(Stamina);
		//stats.Add(Warmth);
		//stats.Add(Fatigue);
		//stats.Add(Hungred);
		//stats.Add(Thrist);
	}

	//public IStat GetStat<T>() where T : IStat
	//{
	//	return stats.OfType<T>().FirstOrDefault();
	//}
}
[System.Serializable]
public class StatsSettings
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