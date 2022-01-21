using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenuCursor : MonoBehaviour
{
    [SerializeField] private Image filler;
	public float FillAmount { get => filler.fillAmount; set => filler.fillAmount = value; }

	public void SetRotation(Quaternion quaternion)
	{
		transform.localRotation = quaternion;
	}
}