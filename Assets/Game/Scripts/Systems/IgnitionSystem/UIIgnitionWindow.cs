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
        public UIIgnitionSlot Tinder => tinder;
        public UIIgnitionSlot Fuel => fuel;
        public UIIgnitionSlot Accelerant => accelerant;

        public TMPro.TextMeshProUGUI BaseChance => baseChance;
        public TMPro.TextMeshProUGUI SuccessChance => successChance;
        public TMPro.TextMeshProUGUI Duration => duration;

        public Button StartButton => startButton;
        public Button BackButton => backButton;

        [SerializeField] private UIIgnitionSlot starter;
        [SerializeField] private UIIgnitionSlot tinder;
        [SerializeField] private UIIgnitionSlot fuel;
        [SerializeField] private UIIgnitionSlot accelerant;
        [Space]
        [SerializeField] private TMPro.TextMeshProUGUI baseChance;
        [SerializeField] private TMPro.TextMeshProUGUI successChance;
        [SerializeField] private TMPro.TextMeshProUGUI duration;
        [Space]
        [SerializeField] private Button startButton;
        [SerializeField] private Button backButton;

        public void Block()
		{
            starter.Block();
            tinder.Block();
            fuel.Block();
            accelerant.Block();

            startButton.enabled = false;
            backButton.enabled = false;
        }
        public void UnBlock()
		{
            starter.UnBlock();
            tinder.UnBlock();
            fuel.UnBlock();
            accelerant.UnBlock();

            startButton.enabled = true;
            backButton.enabled = true;
        }
	}
}