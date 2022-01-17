public class Modifier : IModifier
{
	public float Value { get; private set; }

	public Modifier(float value)
	{
		this.Value = value;
	}
}