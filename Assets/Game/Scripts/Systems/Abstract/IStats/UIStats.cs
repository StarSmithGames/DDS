using UnityEngine;

public class UIStats : MonoBehaviour
{
    public UIFillerAttribute Warmth => warmth;
    [SerializeField] private UIFillerAttribute warmth;
    public UIFillerAttribute Fatigue => fatigue;
    [SerializeField] private UIFillerAttribute fatigue;
    public UIFillerAttribute Hungred => hungred;
    [SerializeField] private UIFillerAttribute hungred;
    public UIFillerAttribute Thirst => thirst;
    [SerializeField] private UIFillerAttribute thirst;
    public UIFillerAttribute Condition => condition;
    [SerializeField] private UIFillerAttribute condition;
    public UIFillerAttribute Stamina => stamina;
    [SerializeField] private UIFillerAttribute stamina;
}
