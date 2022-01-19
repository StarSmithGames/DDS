public interface IStatus
{
	bool IsAlive { get; }

	IInventory Inventory { get; }
	IStats Stats { get; }
}