using Game.Systems.PassTimeSystem;

using Zenject;

namespace Game.Systems.BuildingSystem
{
	public class PassTimeConstruction : ConstructionModel
	{
		private PassTimeHandler passTimeHandler;
		[Inject]
		private void Construct(PassTimeHandler passTimeHandler)
		{
			this.passTimeHandler = passTimeHandler;
		}

		public override void Interact()
		{
			passTimeHandler.SetConstruction(this);
		}
	}
}