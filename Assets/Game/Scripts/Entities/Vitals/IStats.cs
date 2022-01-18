public interface IStats
{
	IStat Condtion { get; }
	IStat Stamina { get; }
	IStat Warmth { get; }
	IStat Fatigue { get; }
	IStat Hungred { get; }
	IStat Thrist { get; }

	//IStat GetStat<T>() where T : IStat;
}