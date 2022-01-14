using System.Collections.Generic;

using UnityEngine.Events;

public interface IInventory
{
    event UnityAction OnInventoryChanged;

    List<Item> Items { get; }
    bool Add(Item item);
    bool Remove(Item item);
}