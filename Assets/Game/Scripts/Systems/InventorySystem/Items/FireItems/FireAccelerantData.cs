using UnityEngine;

namespace Game.Systems.InventorySystem
{
    [CreateAssetMenu(menuName = "Game/Inventory/Items/Fire/Accelerant", fileName = "Accelerant")]
    public class FireAccelerantData : FireItemData
    {
        [Tooltip("Время отведённое на розжиг.")]
        public TimeSystem.Time ignitionTime = new TimeSystem.Time() { Seconds = 300 };
        [Min(1f)]
        [Tooltip("Сколько секунд розжигать.")]
        public float holdTime;
    }
}