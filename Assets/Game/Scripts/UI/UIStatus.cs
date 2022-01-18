using UnityEngine;

public class UIStatus : MonoBehaviour
{
    public UIStats Stats => vitals;
    [SerializeField] private UIStats vitals;
}