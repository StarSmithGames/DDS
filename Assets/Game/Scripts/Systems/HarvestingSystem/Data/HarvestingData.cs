using Game.Systems.BuildingSystem;
using Game.Systems.InventorySystem;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.HarvestingSystem
{
    [CreateAssetMenu(fileName = "HarvestingData", menuName = "Game/Harvestable Items/HarvestingData")]
    public class HarvestingData : ConstructionData
    {
        public TimeSystem.Time harvestingTime = new TimeSystem.Time() { Minutes = 5 };

        public List<Item> yields = new List<Item>();
        //add required tool
    }
}