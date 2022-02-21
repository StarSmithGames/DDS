using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Sirenix.OdinInspector;

public class UIToggleColorTransitionComponent : MonoBehaviour
{
    [SerializeField] private Toggle target;
    [InlineProperty]
    [SerializeField] private ColorBlock block = new ColorBlock()
    {
        normalColor = Color.white,
        highlightedColor = Color.white,
        pressedColor = Color.white,
        selectedColor = Color.white,
        disabledColor = Color.white,
    };

    private Image image;
    private Color startColor;

    private void Awake()
	{
		if(target == null)
		{
            target = GetComponent<Toggle>();
		}

        image = GetComponent<Image>();

        startColor = image.color;

        onValueChanged(target.isOn);

        target.onValueChanged.AddListener(onValueChanged);
    }

	private void OnDestroy()
	{
        target.onValueChanged.RemoveListener(onValueChanged);
    }

	private void onValueChanged(bool value)
	{
        image.color = value ? block.normalColor : startColor;
	}
}