public class Modifier : IModifier
{
	public float Value { get; set; }

	public Modifier(float value)
	{
		this.Value = value;
	}
}