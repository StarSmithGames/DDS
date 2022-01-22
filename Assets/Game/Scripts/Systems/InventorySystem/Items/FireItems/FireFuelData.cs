using UnityEngine;

namespace Game.Systems.InventorySystem
{
    [CreateAssetMenu(menuName = "Game/Inventory/Items/Fire/Fuel", fileName = "Fuel")]
    public class FireFuelData : FireItemData
    {
        [Range(0, 21f)]
        public float addTemperature = 0;

        //[Tooltip("К времени горения.")]
        //public Times addFireTime;
    }
}