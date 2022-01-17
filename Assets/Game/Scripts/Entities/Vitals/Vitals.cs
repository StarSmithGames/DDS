public class Vitals : IVitals
{
	public IStat Condition { get; protected set; }
	public IStat Stamina { get; protected set; }
}