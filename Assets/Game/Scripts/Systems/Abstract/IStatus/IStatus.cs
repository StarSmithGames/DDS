using Game.Systems.InventorySystem;

public interface IStatus
{
	IInventory Inventory { get; }
	IStats Stats { get; }
	Resistances Resistances { get; }
}