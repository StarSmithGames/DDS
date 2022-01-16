using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Zenject;

public class UIInventory : MonoBehaviour
{
	[SerializeField] private Transform content;

	public IInventory Inventory => inventory;
	private IInventory inventory;

	private List<UIInventorySlot> slots = new List<UIInventorySlot>();

	private SignalBus signalBus;
	private UIInventorySlot.Factory slotFactory;

	[Inject]
	private void Construct(SignalBus signalBus, UIInventorySlot.Factory slotFactory, int initialSlotCount)
	{
		this.signalBus = signalBus;
		this.slotFactory = slotFactory;

		for (int i = 0; i < initialSlotCount; i++)
		{
			SpawnSlot();
		}
	}

	private void OnDestroy()
	{
		if (inventory != null)
		{
			inventory.OnInventoryChanged -= UpdateInventory;
		}
	}

	public void SetInventory(IInventory inventory)
	{
		if (inventory != null)
		{
			inventory.OnInventoryChanged -= UpdateInventory;
		}

		this.inventory = inventory;
		
		if(inventory != null)
		{
			inventory.OnInventoryChanged += UpdateInventory;
		}

		UpdateInventory();
	}

	private void UpdateInventory()
	{
		ClearAllSlots();

		if(inventory != null)
		{
			for (int i = 0; i < inventory.Items.Count; i++)
			{
				AddItem(inventory.Items[i]);
			}
		}
	}

	private void ClearAllSlots()
	{
		for (int i = 0; i < slots.Count; i++)
		{
			slots[i].SetItem(null);
		}
	}

	private void AddItem(Item item)
	{
		UIInventorySlot slot = slots.Where((slot) => slot.IsEmpty).FirstOrDefault();
		
		if(slot != null)
		{
			slot.SetItem(item);
		}
		else
		{
			SpawnSlotRow();
			slot = slots.Where((slot) => slot.IsEmpty).FirstOrDefault();
			slot.SetItem(item);
		}
	}

	private void SpawnSlot()
	{
		UIInventorySlot slot = slotFactory.Create();

		slot.SetItem(null);
		slot.SetOwner(this);
		slot.transform.parent = content;

		slots.Add(slot);
	}

	private void SpawnSlotRow()
	{
		for (int i = 0; i < 5; i++)
		{
			SpawnSlot();
		}
	}

	private void DespawnSlot(UIInventorySlot slot)
	{
		if (slots.Contains(slot))
		{
			slots.Remove(slot);
			slot.SetItem(null);
			slot.SetOwner(null);
			slot.DespawnIt();
		}
	}

	private void DespawnSlotRow()
	{
		if (slots.Count <= 20) return;// min page

		for (int i = 0; i < 5; i++)
		{
			DespawnSlot(slots[slots.Count - 1]);
		}
	}
}