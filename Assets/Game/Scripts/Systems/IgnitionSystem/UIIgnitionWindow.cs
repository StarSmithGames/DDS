using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Game.Systems.IgnitionSystem
{
    public class UIIgnitionWindow : WindowBase
    {
        private List<UIIgnitionSlot> ignitionSlots = null;
        public List<UIIgnitionSlot> IgnitionSlots {
			get
			{
                if(ignitionSlots == null) ignitionSlots = new List<UIIgnitionSlot> { starter, tinder, fuel, accelerant };
                return ignitionSlots;
			}
        }

        public UIIgnitionSlot Starter => starter;
        [SerializeField] private UIIgnitionSlot starter;
        public UIIgnitionSlot Tinder => tinder;
        [SerializeField] private UIIgnitionSlot tinder;
        public UIIgnitionSlot Fuel => fuel;
        [SerializeField] private UIIgnitionSlot fuel;
        public UIIgnitionSlot Accelerant => accelerant;
        [SerializeField] private UIIgnitionSlot accelerant;

        public Button StartButton => startButton;
        [SerializeField] private Button startButton;
        public Button BackButton => backButton;
        [SerializeField] private Button backButton;
    }
}