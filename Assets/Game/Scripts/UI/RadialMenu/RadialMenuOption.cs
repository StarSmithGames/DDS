using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMenuOption : MonoBehaviour
{
	public float Rotation { get; set; }

	public void SetPosition(Vector3 vector)
	{
		transform.localPosition = vector;
	}
}