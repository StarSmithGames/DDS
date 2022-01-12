using UnityEngine;

[CreateAssetMenu(menuName = "Game/Inventory/Items/Fire/Accelerant", fileName = "Accelerant")]
public class FireAccelerantData : FireItemData
{
    [Tooltip("Сколько секунд розжигать.")]
    [Min(1f)]
    public float holdTime;
}
