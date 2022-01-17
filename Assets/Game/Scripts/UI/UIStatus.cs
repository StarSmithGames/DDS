using UnityEngine;

public class UIStatus : MonoBehaviour
{
    public UIVitals Vitals => vitals;
    [SerializeField] private UIVitals vitals;
}