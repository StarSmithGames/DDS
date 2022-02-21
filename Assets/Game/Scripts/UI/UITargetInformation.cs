using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITargetInformation : MonoBehaviour
{
	public bool IsActive => gameObject.activeSelf;

	[SerializeField] private GameObject topContent;
	[SerializeField] private TMPro.TextMeshProUGUI targetName;
	[SerializeField] private GameObject separator;
	[Space]
	[SerializeField] private GameObject bottomContent;
	[SerializeField] private TMPro.TextMeshProUGUI targetInformation;

	public void SetInformation(string name, string information)
	{
		targetName.text = name;
		targetInformation.text = information;

		separator.SetActive(!string.IsNullOrEmpty(information));
		bottomContent.SetActive(!string.IsNullOrEmpty(information));
	}
}