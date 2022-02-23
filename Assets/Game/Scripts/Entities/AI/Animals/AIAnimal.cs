using Zenject;

public class AIAnimal : AI
{
	private IBehavior behavior;

	[Inject]
	private void Construct(IBehavior behavior)
	{
		this.behavior = behavior;
	}

	private void Start()
	{
		behavior.Initialize();
	}

	protected override void Update()
	{
		base.Update();
		behavior.Tick();
	}
}