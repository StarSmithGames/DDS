using UnityEngine;

namespace Game.Systems.InventorySystem
{
    [CreateAssetMenu(menuName = "Game/Inventory/Items/Fire/Starter", fileName = "Starter")]
    public class FireStarterData : FireItemData
    {
        [Tooltip("Время отведённое на розжиг.")]
        public TimeSystem.Time ignitionTime = new TimeSystem.Time() { Seconds = 300 };

        [Tooltip("Дополнительное временя к горению.")]
		public TimeSystem.Time addFireTime;
	}
}