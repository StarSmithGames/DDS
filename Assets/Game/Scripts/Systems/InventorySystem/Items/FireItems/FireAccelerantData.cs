using UnityEngine;

namespace Game.Systems.InventorySystem
{
    [CreateAssetMenu(menuName = "Game/Inventory/Items/Fire/Accelerant", fileName = "Accelerant")]
    public class FireAccelerantData : FireItemData
    {
        [Tooltip("������� ������ ���������.")]
        [Min(1f)]
        public float holdTime;
    }
}