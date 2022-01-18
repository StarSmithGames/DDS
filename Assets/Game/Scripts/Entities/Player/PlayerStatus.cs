using UnityEngine;

using Zenject;

public class PlayerStatus : IStatus
{
	public IInventory Inventory { get; private set; }
	public IStats Stats { get; private set; }

	private UIManager uiManager;

	public PlayerStatus(UIManager uiManager, IInventory inventory, IStats stats)
	{
		this.uiManager = uiManager;

		Inventory = inventory;
		Stats = stats;

		var uistats = uiManager.Status.Stats;

		uistats.Condition.SetAttribute(Stats.Condtion);
		uistats.Stamina.SetAttribute(Stats.Stamina);
		uistats.Warmth.SetAttribute(Stats.Warmth);
		uistats.Fatigue.SetAttribute(Stats.Fatigue);
		uistats.Hungred.SetAttribute(Stats.Hungred);
		uistats.Thirst.SetAttribute(Stats.Thrist);
	}

	public void Tick()
	{
		Stats.Condtion.CurrentValue -= 0.01f;
	}
}