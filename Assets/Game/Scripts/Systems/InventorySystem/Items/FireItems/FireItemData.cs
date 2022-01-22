using Sirenix.OdinInspector;

using UnityEngine;

namespace Game.Systems.InventorySystem
{
    public abstract class FireItemData : ItemData
    {
        [Title("Benefit")]
        [SuffixLabel("%", true)]
        [Range(-100f, 100f)]
        [Tooltip("К шансу розжига.")]
        public float chance = 0f;
    }
}