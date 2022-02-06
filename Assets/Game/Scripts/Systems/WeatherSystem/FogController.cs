using Game.Systems.WeatherSystem;

using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FogController
{
    public Material fogGlobal;
    private NormalFog normal;

    [InlineEditor]
    [OnValueChanged("UpdateFog", true)]
    public FogPresset data;

    private float currentFogRange = 0f;
    public float CurrentFogRange
    {
        get => currentFogRange;
        set
        {
            currentFogRange = value;
            RenderSettings.fogDensity = Mathf.Lerp(min, max, currentFogRange);

            RenderSettings.fog = currentFogRange != 0;
        }
    }

    private float currentNormalRange = 0f;
    public float CurrentNormalRange
    {
        get => currentNormalRange;
        set
        {
            currentNormalRange = value;

#if UNITY_EDITOR
            GameObject.FindObjectOfType<NormalFog>(true)?.gameObject.SetActive(currentNormalRange != 0);
#else
            normal.gameObject?.SetActive(currentNormalRange != 0);
#endif
        }
    }

    private Color currentColor;
    private Color CurrentColor
    {
        get => currentColor;
        set
        {
            currentColor = value;

            RenderSettings.fogColor = currentColor;

            Color color = currentColor;
            color.a = CurrentNormalRange;

            if(fogGlobal != null)
			{
                fogGlobal.color = color;
            }
        }
    }

    private float min = 0;
    private float max = 0.2f;

    private float minBias = 0.7f;

    private void UpdateFog()
    {
        CurrentFogRange = data?.fogRange ?? 0;
        CurrentNormalRange = data?.normalRange ?? 0;
        CurrentColor = data?.fogColor ?? Color.white;
    }
}