public class PlayerStatus : IStatus
{
	public IInventory Inventory { get; private set; }
	public IVitals Vitals { get; private set; }

	public PlayerStatus(IInventory inventory, IVitals vitals)
	{
		Inventory = inventory;
		Vitals = vitals;
	}
}