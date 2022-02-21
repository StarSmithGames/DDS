using UnityEngine;

namespace Game.Systems.InventorySystem
{
    [CreateAssetMenu(menuName = "Game/Inventory/Items/Fire/Starter", fileName = "Starter")]
    public class FireStarterData : FireItemData
    {
        [Tooltip("Время отведённое на розжиг.")]
        public TimeSystem.Time ignitionTime = new TimeSystem.Time() { Seconds = 300 };
        [Min(1f)]
        [Tooltip("Сколько секунд розжигать.")]
        public float holdTime;
        [Space]
        [Tooltip("Добавочное время к горению огня.")]
		public TimeSystem.Time addFireTime;
	}
}