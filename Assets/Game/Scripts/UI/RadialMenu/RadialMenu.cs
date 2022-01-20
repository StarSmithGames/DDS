using Sirenix.OdinInspector;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

using UnityEngine;
using UnityEngine.Events;

using static UnityEditor.Progress;

public class RadialMenu : MonoBehaviour
{
    [SerializeField] private Transform parent;
    [Range(1, 12)]
    [OnValueChanged("UpdateMenu")]
    [SerializeField] private int count;
    [SerializeField] private GameObject prefab;

    private List<GameObject> options = new List<GameObject>();

    [OnValueChanged("UpdateRadius")]
    public float radius = 150f;

    public void OpenMenu()
    {
        gameObject.SetActive(true);
    }
    public void CloseMenu()
    {
        gameObject.SetActive(false);
    }

	private void Awake()
	{
        UpdateMenu();
        OpenMenu();
    }

	public void UpdateMenu()
    {
        ReSizeOptions();

        UpdateRadius();
    }

	private void UpdateRadius()
	{
        float fillRadius = (1f / (float)count) * 360f;
        float previousRotation = 0;
        //y=sin(angle)
        //x=cos(angle)
        for (int i = 0; i < options.Count; i++)
        {
            float bRot = previousRotation + fillRadius / 2;
            previousRotation = bRot + fillRadius / 2;

            options[i].transform.localPosition = new Vector2(radius * Mathf.Cos((bRot - 90) * Mathf.Deg2Rad), -radius * Mathf.Sin((bRot - 90) * Mathf.Deg2Rad));
        }
	}

    private void ReSizeOptions()
	{
        int diff = options.Count - count;

        if (diff > 0)
        {
			for (int i = diff - 1; i >= 0 ; i--)
			{
                var go = options[i];
                options.Remove(go);
                Destroy(go);
			}
        }
        else
        {
            for (int i = 0; i < -diff; i++)
            {
                Transform option = Instantiate(prefab).transform;
                option.SetParent(parent);
                option.position = Vector3.zero;
                option.localScale = Vector3.one;

                options.Add(option.gameObject);
            }
        }
    }
}