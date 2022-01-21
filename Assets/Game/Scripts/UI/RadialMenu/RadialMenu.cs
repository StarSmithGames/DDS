using Sirenix.OdinInspector;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using static UnityEditor.Progress;

public class RadialMenu : MonoBehaviour
{
    [SerializeField] private Transform parent;
    [SerializeField] private RadialMenuCursor cursor;
    [SerializeField] private bool isSnap = true;
    [Range(1, 12)]
    [OnValueChanged("UpdateMenu")]
    [SerializeField] private int count;
    [SerializeField] private RadialMenuOption prefab;

    private List<RadialMenuOption> options = new List<RadialMenuOption>();

    [OnValueChanged("UpdateRadius")]
    public float maxRadius = 200f;

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

    private void Update()
    {
        Vector3 screenBounds = new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f);
        Vector3 vector = Input.mousePosition - screenBounds;

        float mouseRotation = Mathf.Atan2(vector.x, vector.y) * 57.29578f;
        if (mouseRotation < 0f)
            mouseRotation += 360f;
        float cursorRotation = -(mouseRotation - cursor.FillAmount * 180f);

        float difference = 9999;
        RadialMenuOption nearest = null;
        for (int i = 0; i < options.Count; i++)
        {
            float rotation = options[i].Rotation;

            if (Mathf.Abs(rotation - mouseRotation) < difference)
            {
                nearest = options[i];
                difference = Mathf.Abs(rotation - mouseRotation);
            }
        }

		if (isSnap)
			cursorRotation = -(nearest.Rotation - cursor.FillAmount * 360f / 2f);
		cursor.SetRotation(Quaternion.Euler(0, 0, cursorRotation));//Quaternion.Slerp(cursor.transform.localRotation, , lerpAmount));
    }

	public void UpdateMenu()
    {
        ReSizeOptions();

        UpdateRadius();
    }

	private void UpdateRadius()
	{
        if (!Application.isPlaying) return;

        float normalCount = (1f / (float)count);

        float fillRadius = normalCount * 360f;
        cursor.FillAmount = normalCount;

        //y=sin(angle)
        //x=cos(angle)
        float prevRotation = 0;
        for (int i = 0; i < options.Count; i++)
        {
            float rotation = prevRotation + fillRadius / 2;
            prevRotation = rotation + fillRadius / 2;

            options[i].Rotation = rotation;
            options[i].SetPosition(new Vector2(maxRadius * Mathf.Cos((rotation - 90) * Mathf.Deg2Rad), -maxRadius * Mathf.Sin((rotation - 90) * Mathf.Deg2Rad)));
        }
	}

    private void ReSizeOptions()
	{
        if (!Application.isPlaying) return;

        int diff = options.Count - count;

        if (diff > 0)
        {
			for (int i = diff - 1; i >= 0 ; i--)
			{
                var go = options[i];
                options.Remove(go);
                Destroy(go.gameObject);
			}
        }
        else
        {
            for (int i = 0; i < -diff; i++)
            {
                RadialMenuOption option = Instantiate(prefab);
                option.transform.SetParent(parent);
                option.transform.position = Vector3.zero;
                option.transform.localScale = Vector3.one;

                options.Add(option);
            }
        }
    }
}