using UnityEngine;

public class UIStatus : MonoBehaviour
{
    public UIStats Stats => vitals;
    public UIResistances Resistances => resistances;

    [SerializeField] private UIStats vitals;
    [SerializeField] private UIResistances resistances;
}