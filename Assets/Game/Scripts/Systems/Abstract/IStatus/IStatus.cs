using Game.Systems.InventorySystem;

public interface IStatus
{
	bool IsAlive { get; }

	IInventory Inventory { get; }
	IStats Stats { get; }
	IResistances Resistances { get; }
}