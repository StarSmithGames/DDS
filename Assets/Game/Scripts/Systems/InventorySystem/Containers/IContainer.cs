namespace Game.Systems.InventorySystem
{
	public interface IContainer : IEntity, IInteractable, ISearchable
	{
		public ContainerData ContainerData { get; }
		public IInventory Inventory { get; }
	}
}