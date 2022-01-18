public interface IStatus
{
	IInventory Inventory { get; }
	IStats Stats { get; }

	void Tick();
}