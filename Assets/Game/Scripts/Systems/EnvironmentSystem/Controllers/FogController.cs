using System.Collections;
using UnityEngine;

using Zenject;

namespace Game.Systems.EnvironmentSystem
{
    public class FogController
    {
        public bool IsTransitionProcess => transitionCoroutine != null;
        private Coroutine transitionCoroutine = null;

        public float CurrentFogRange
        {
            get => currentFogRange;
            set
            {
                currentFogRange = value;
                RenderSettings.fogDensity = Mathf.Lerp(fogDensityLimits.x, fogDensityLimits.y, currentFogRange);

                RenderSettings.fog = currentFogRange != 0;
            }
        }
        private float currentFogRange = 0f;

        public float CurrentNormalRange
        {
            get => currentNormalRange;
            set
            {
                currentNormalRange = value;

                normal.gameObject.SetActive(currentNormalRange != 0);
            }
        }
        private float currentNormalRange = 0f;

        public Color CurrentColor
        {
            get => currentColor;
            set
            {
                currentColor = value;

                RenderSettings.fogColor = currentColor;

                if (normal.Material != null)
                {
                    Color color = currentColor;
                    color.a = CurrentNormalRange;

                    normal.Material.color = color;
                }
            }
        }
        private Color currentColor;

        private float minBias = 0.7f;
        private Vector2 fogDensityLimits = new Vector2(0, 0.2f);

        private FogPresset currentFog;
        private FogNormal normal;
        private AsyncManager asyncManager;

        public FogController(FogNormal normal, AsyncManager asyncManager)
        {
            this.normal = normal;
            this.asyncManager = asyncManager;
        }

        public void SetFog(FogPresset fogPresset)
        {
            currentFog = fogPresset;
            UpdateFog();
        }

        public void StartTransition(FogPresset fogPresset, float time)
        {
            if (currentFog != fogPresset && fogPresset != null)
            {
                currentFog = fogPresset;

                StopTransition();
                asyncManager.StartCoroutine(Transition(currentFog, time));
            }
        }

        private IEnumerator Transition(FogPresset to, float time)
        {
            float oldFogRange = CurrentFogRange;
            float oldNormalRange = CurrentNormalRange;
            Color oldColor = CurrentColor;

            float t = Time.deltaTime;
            while (t < time)
            {
                t += Time.deltaTime;

                float progress = t / time;

                CurrentFogRange = Mathf.Lerp(oldFogRange, to.fogRange, progress);
                CurrentNormalRange = Mathf.Lerp(oldNormalRange, to.normalRange, progress);
                CurrentColor = Color.Lerp(oldColor, to.fogColor, progress);

                yield return null;
            }
            UpdateFog();
            StopTransition();
        }

        private void StopTransition()
        {
            if (IsTransitionProcess)
            {
                asyncManager.StopCoroutine(transitionCoroutine);
                transitionCoroutine = null;
            }
        }

        public void UpdateFog()
        {
            if (currentFog != null)
            {
                CurrentFogRange = currentFog.fogRange;
                CurrentNormalRange = currentFog.normalRange;
                CurrentColor = currentFog.fogColor;
            }
            else
            {
                CurrentFogRange = 0;
                CurrentNormalRange = 0;
            }
        }
    }
}