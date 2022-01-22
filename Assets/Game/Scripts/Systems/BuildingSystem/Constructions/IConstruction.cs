namespace Game.Systems.BuildingSystem
{
	public interface IConstruction : IEntity, IInteractable, IIntersectable
	{
		ConstructionData ConstructionData { get; }
	}
}