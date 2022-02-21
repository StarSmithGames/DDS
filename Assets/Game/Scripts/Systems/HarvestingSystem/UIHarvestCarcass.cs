using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Systems.HarvestingSystem
{
    public class UIHarvestCarcass : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI InfoTitle => infoTitle;
        public TMPro.TextMeshProUGUI ToolTitle => toolTitle;

        [SerializeField] private TMPro.TextMeshProUGUI itemName;
        [Space]
        [SerializeField] private TMPro.TextMeshProUGUI infoTitle;
        [SerializeField] private TMPro.TextMeshProUGUI hoursLabel;
        [SerializeField] private TMPro.TextMeshProUGUI caloriesLabel;
        [Space]
        [SerializeField] private GameObject toolRoot;
        [SerializeField] private TMPro.TextMeshProUGUI toolTitle;
        [SerializeField] private Image toolIcon;
        [SerializeField] private Button prevTool;
        [SerializeField] private Button nextTool;
    }
}