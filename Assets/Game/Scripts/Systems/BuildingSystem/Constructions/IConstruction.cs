namespace Game.Systems.BuildingSystem
{
	public interface IConstruction : IEntity, IInteractable, IIntersectable
	{
		bool IsCreated { get; set; }
		bool IsPlaced { get; set; }
		ConstructionData ConstructionData { get; }
	}
}