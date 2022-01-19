public interface IStats
{
	IStat Condition { get; }
	IStat Stamina { get; }
	IStat Warmth { get; }
	IStat Fatigue { get; }
	IStat Hungred { get; }
	IStat Thirst { get; }

	//IStat GetStat<T>() where T : IStat;
}