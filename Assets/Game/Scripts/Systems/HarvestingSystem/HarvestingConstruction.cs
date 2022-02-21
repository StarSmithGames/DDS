using Game.Systems.BuildingSystem;

using Zenject;

namespace Game.Systems.HarvestingSystem
{
	public class HarvestingConstruction : ConstructionModel, IHarvestable
	{
		private HarvestingHandler harvestingHandler;

		[Inject]
		private void Construct(HarvestingHandler harvestingHandler)
		{
			this.harvestingHandler = harvestingHandler;
		}

		public override void Interact()
		{
			harvestingHandler.SetHarveting(this);
		}
	}
}