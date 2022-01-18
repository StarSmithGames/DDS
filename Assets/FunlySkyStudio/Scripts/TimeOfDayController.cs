using Game.Systems.TimeSystem;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using Zenject;

namespace Funly.SkyStudio
{
    // This controller manages time and updating the skybox material with the proper configuration
    // values for the current time of day. This loads sky data from your sky profile and sky timeline.
    public class TimeOfDayController : MonoBehaviour
    {
        [Tooltip("Sky profile defines the skyColors configuration for times of day. " +
          "This script will animate between those skyColors values based on the time of day.")]
        [SerializeField]
        private SkyProfile m_SkyProfile;
        public SkyProfile skyProfile
        {
            get { return m_SkyProfile; }
            set
            {
                if (value != null && copySkyProfile)
                {
                    m_SkyProfile = Instantiate(value);
                }
                else
                {
                    m_SkyProfile = value;
                }
                m_SkyMaterialController = null;
                UpdateSkyForCurrentTime();
                SynchronizeAllShaderKeywords();
            }
        }

        [Tooltip("Time is expressed in a fractional number of days that have completed.")]
        [SerializeField]
        private float m_SkyTime = 0;
        public float skyTime
        {
            get { return m_SkyTime; }
            set
            {
                m_SkyTime = Mathf.Abs(value);
                UpdateSkyForCurrentTime();
            }
        }

        [Tooltip("Create a copy of the sky profile at runtime, so modifications don't affect the original Sky Profile in your project.")]
        public bool copySkyProfile;

        // Use the Sky Material controller to directly manipulate the skybox values programatically.
        private SkyMaterialController m_SkyMaterialController;
        public SkyMaterialController SkyMaterial { get { return m_SkyMaterialController; } }

        [Tooltip("Sun orbit.")]
        public OrbitingBody sunOrbit;

        [Tooltip("Moon orbit.")]
        public OrbitingBody moonOrbit;

        [Tooltip("If true we'll invoke DynamicGI.UpdateEnvironment() when skybox changes. This is an expensive operation.")]
        public bool updateGlobalIllumination = false;

        // Callback invoked whenever the time of day changes.
        public delegate void TimeOfDayDidChange(TimeOfDayController tc, float timeOfDay);
        public event TimeOfDayDidChange timeChangedCallback;

        // Current progress value through a day cycle (value 0-1).
        public float timeOfDay
        {
            get { return m_SkyTime - ((int)m_SkyTime); }
        }

        public int daysElapsed
        {
            get { return (int)m_SkyTime; }
        }

        [Inject]
        private void Construct(TimeSystem timeSystem)
		{

		}

        private void OnEnabled()
        {
            skyTime = m_SkyTime;
        }

        private void OnValidate()
        {
            if (gameObject.activeInHierarchy == false)
            {
                return;
            }
            skyTime = m_SkyTime;
            skyProfile = m_SkyProfile;
        }

		private void Update()
		{
            skyTime += 0.5f * UnityEngine.Time.deltaTime;
		}

		public void UpdateGlobalIllumination()
        {
            DynamicGI.UpdateEnvironment();
        }

        private void SynchronizeAllShaderKeywords()
        {
            if (m_SkyProfile == null)
            {
                return;
            }

            foreach (ProfileFeatureSection section in m_SkyProfile.profileDefinition.features)
            {
                foreach (ProfileFeatureDefinition feature in section.featureDefinitions)
                {
                    if (feature.featureType == ProfileFeatureDefinition.FeatureType.ShaderKeyword)
                    {
                        SynchronizedShaderKeyword(feature.featureKey, feature.shaderKeyword);
                    }
                }
            }
        }

        private void SynchronizedShaderKeyword(string featureKey, string shaderKeyword)
        {
            if (skyProfile == null || skyProfile.skyboxMaterial == null)
            {
                return;
            }

            if (skyProfile.IsFeatureEnabled(featureKey))
            {
                if (!skyProfile.skyboxMaterial.IsKeywordEnabled(shaderKeyword))
                {
                    skyProfile.skyboxMaterial.EnableKeyword(shaderKeyword);
                }
            }
            else
            {
                if (skyProfile.skyboxMaterial.IsKeywordEnabled(shaderKeyword))
                {
                    skyProfile.skyboxMaterial.DisableKeyword(shaderKeyword);
                }
            }
        }

        public void UpdateSkyForCurrentTime()
        {
            if (skyProfile == null)
            {
                Debug.LogError("Your scene has a sky controller but no sky profile is assigned. " +
                  "Create a sky profile using one of the supplied templates in the presets directory, " +
                  "or create a new sky profile from 'Assets > Create > Sky Profile' and assign it to the sky controller.");
                return;
            }

            if (skyProfile.skyboxMaterial == null)
            {
                Debug.LogError("Your sky profile is missing a reference to the skybox material.");
                return;
            }

            if (m_SkyMaterialController == null)
            {
                m_SkyMaterialController = new SkyMaterialController();
            }

            m_SkyMaterialController.SkyboxMaterial = skyProfile.skyboxMaterial;

            if (RenderSettings.skybox == null || RenderSettings.skybox.GetInstanceID() != skyProfile.skyboxMaterial.GetInstanceID())
            {
                RenderSettings.skybox = skyProfile.skyboxMaterial;
            }

            SynchronizeAllShaderKeywords();

            // Sky.
            m_SkyMaterialController.BackgroundCubemap = skyProfile.GetTexturePropertyValue(ProfilePropertyKeys.SkyCubemapKey, timeOfDay) as Cubemap;
            m_SkyMaterialController.SkyColor = skyProfile.GetColorPropertyValue(ProfilePropertyKeys.SkyUpperColorKey, timeOfDay);
            m_SkyMaterialController.SkyMiddleColor = skyProfile.GetColorPropertyValue(ProfilePropertyKeys.SkyMiddleColorKey, timeOfDay);
            m_SkyMaterialController.HorizonColor = skyProfile.GetColorPropertyValue(ProfilePropertyKeys.SkyLowerColorKey, timeOfDay);
            m_SkyMaterialController.GradientFadeBegin = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.HorizonTrasitionStartKey, timeOfDay);
            m_SkyMaterialController.GradientFadeLength = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.HorizonTransitionLengthKey, timeOfDay);
            m_SkyMaterialController.SkyMiddlePosition = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.SkyMiddleColorPositionKey, timeOfDay);
            m_SkyMaterialController.StarFadeBegin = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.StarTransitionStartKey, timeOfDay);
            m_SkyMaterialController.StarFadeLength = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.StarTransitionLengthKey, timeOfDay);
            m_SkyMaterialController.HorizonDistanceScale = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.HorizonStarScaleKey, timeOfDay);

            // Clouds.
            if (skyProfile.IsFeatureEnabled(ProfileFeatureKeys.NoiseCloudFeature))
            {
                m_SkyMaterialController.CloudTexture = skyProfile.GetTexturePropertyValue(ProfilePropertyKeys.CloudNoiseTextureKey, timeOfDay);
                m_SkyMaterialController.CloudTextureTiling = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.CloudTextureTiling, timeOfDay);
                m_SkyMaterialController.CloudDensity = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.CloudDensityKey, timeOfDay);
                m_SkyMaterialController.CloudSpeed = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.CloudSpeedKey, timeOfDay);
                m_SkyMaterialController.CloudDirection = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.CloudDirectionKey, timeOfDay);
                m_SkyMaterialController.CloudHeight = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.CloudHeightKey, timeOfDay);
                m_SkyMaterialController.CloudColor1 = skyProfile.GetColorPropertyValue(ProfilePropertyKeys.CloudColor1Key, timeOfDay);
                m_SkyMaterialController.CloudColor2 = skyProfile.GetColorPropertyValue(ProfilePropertyKeys.CloudColor2Key, timeOfDay);
                m_SkyMaterialController.CloudFadePosition = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.CloudFadePositionKey, timeOfDay);
                m_SkyMaterialController.CloudFadeAmount = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.CloudFadeAmountKey, timeOfDay);
            }

            // Fog.
            if (skyProfile.IsFeatureEnabled(ProfileFeatureKeys.FogFeature))
            {
                Color fogColor = skyProfile.GetColorPropertyValue(ProfilePropertyKeys.FogColorKey, timeOfDay);
                m_SkyMaterialController.FogColor = fogColor;
                m_SkyMaterialController.FogDensity = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.FogDensityKey, timeOfDay);
                m_SkyMaterialController.FogHeight = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.FogLengthKey, timeOfDay);

                // Synchronize with Unity's global fog color so the rest of the scene uses this color fog.
                if (skyProfile.GetBoolPropertyValue(ProfilePropertyKeys.FogSyncWithGlobal, timeOfDay))
                {
                    RenderSettings.fogColor = fogColor;
                }
            }

            // Sun.
            if (skyProfile.IsFeatureEnabled(ProfileFeatureKeys.SunFeature) && sunOrbit)
            {
                sunOrbit.spherePoint = skyProfile.GetSpherePointPropertyValue(ProfilePropertyKeys.SunPositionKey, timeOfDay);

                m_SkyMaterialController.SunDirection = sunOrbit.BodyGlobalDirection;
                m_SkyMaterialController.SunColor = skyProfile.GetColorPropertyValue(ProfilePropertyKeys.SunColorKey, timeOfDay);
                m_SkyMaterialController.SunTexture = skyProfile.GetTexturePropertyValue(ProfilePropertyKeys.SunTextureKey, timeOfDay);
                m_SkyMaterialController.SunSize = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.SunSizeKey, timeOfDay);
                m_SkyMaterialController.SunEdgeFeathering = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.SunEdgeFeatheringKey, timeOfDay);
                m_SkyMaterialController.SunBloomFilterBoost = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.SunColorIntensityKey, timeOfDay);

                if (skyProfile.IsFeatureEnabled(ProfileFeatureKeys.SunRotationFeature))
                {
                    m_SkyMaterialController.SunRotationSpeed = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.SunRotationSpeedKey, timeOfDay); ;
                }

                if (skyProfile.IsFeatureEnabled(ProfileFeatureKeys.SunSpriteSheetFeature))
                {
                    m_SkyMaterialController.SetSunSpriteDimensions(
                      (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.SunSpriteColumnCountKey, timeOfDay),
                      (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.SunSpriteRowCountKey, timeOfDay));
                    m_SkyMaterialController.SunSpriteItemCount = (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.SunSpriteItemCountKey, timeOfDay);
                    m_SkyMaterialController.SunSpriteAnimationSpeed = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.SunSpriteAnimationSpeedKey, timeOfDay);
                }

                if (sunOrbit.BodyLight)
                {
                    if (!sunOrbit.BodyLight.enabled)
                    {
                        sunOrbit.BodyLight.enabled = true;
                    }
                    RenderSettings.sun = sunOrbit.BodyLight;
                    sunOrbit.BodyLight.color = skyProfile.GetColorPropertyValue(ProfilePropertyKeys.SunLightColorKey, timeOfDay);
                    sunOrbit.BodyLight.intensity = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.SunLightIntensityKey, timeOfDay);
                }
            }
            else if (sunOrbit && sunOrbit.BodyLight)
            {
                sunOrbit.BodyLight.enabled = false;
            }

            // Moon.
            if (skyProfile.IsFeatureEnabled(ProfileFeatureKeys.MoonFeature) && moonOrbit)
            {
                moonOrbit.spherePoint = skyProfile.GetSpherePointPropertyValue(ProfilePropertyKeys.MoonPositionKey, timeOfDay);

                m_SkyMaterialController.MoonDirection = moonOrbit.BodyGlobalDirection;
                m_SkyMaterialController.MoonColor = skyProfile.GetColorPropertyValue(ProfilePropertyKeys.MoonColorKey, timeOfDay);
                m_SkyMaterialController.MoonTexture = skyProfile.GetTexturePropertyValue(ProfilePropertyKeys.MoonTextureKey, timeOfDay);
                m_SkyMaterialController.MoonSize = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.MoonSizeKey, timeOfDay);
                m_SkyMaterialController.MoonEdgeFeathering = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.MoonEdgeFeatheringKey, timeOfDay);
                m_SkyMaterialController.MoonBloomFilterBoost = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.MoonColorIntensityKey, timeOfDay);

                if (skyProfile.IsFeatureEnabled(ProfileFeatureKeys.MoonRotationFeature))
                {
                    m_SkyMaterialController.MoonRotationSpeed = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.MoonRotationSpeedKey, timeOfDay);
                }

                if (skyProfile.IsFeatureEnabled(ProfileFeatureKeys.MoonSpriteSheetFeature))
                {
                    m_SkyMaterialController.SetMoonSpriteDimensions(
                      (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.MoonSpriteColumnCountKey, timeOfDay),
                      (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.MoonSpriteRowCountKey, timeOfDay));
                    m_SkyMaterialController.MoonSpriteItemCount = (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.MoonSpriteItemCountKey, timeOfDay);
                    m_SkyMaterialController.MoonSpriteAnimationSpeed = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.MoonSpriteAnimationSpeedKey, timeOfDay);
                }

                if (moonOrbit.BodyLight)
                {
                    if (!moonOrbit.BodyLight.enabled)
                    {
                        moonOrbit.BodyLight.enabled = true;
                    }
                    moonOrbit.BodyLight.color = skyProfile.GetColorPropertyValue(ProfilePropertyKeys.MoonLightColorKey, timeOfDay);
                    moonOrbit.BodyLight.intensity = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.MoonLightIntensityKey, timeOfDay);
                }
            }
            else if (moonOrbit && moonOrbit.BodyLight)
            {
                moonOrbit.BodyLight.enabled = false;
            }

            // Star Layer 1.
            if (skyProfile.IsFeatureEnabled(ProfileFeatureKeys.StarLayer1Feature))
            {
                m_SkyMaterialController.StarLayer1DataTexture = skyProfile.starLayer1DataTexture;
                m_SkyMaterialController.StarLayer1Color = skyProfile.GetColorPropertyValue(ProfilePropertyKeys.Star1ColorKey, timeOfDay);
                m_SkyMaterialController.StarLayer1MaxRadius = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star1SizeKey, timeOfDay);
                m_SkyMaterialController.StarLayer1Texture = skyProfile.GetTexturePropertyValue(ProfilePropertyKeys.Star1TextureKey, timeOfDay);
                m_SkyMaterialController.StarLayer1TwinkleAmount = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star1TwinkleAmountKey, timeOfDay);
                m_SkyMaterialController.StarLayer1TwinkleSpeed = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star1TwinkleSpeedKey, timeOfDay);
                m_SkyMaterialController.StarLayer1RotationSpeed = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star1RotationSpeedKey, timeOfDay);
                m_SkyMaterialController.StarLayer1EdgeFeathering = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star1EdgeFeatheringKey, timeOfDay);
                m_SkyMaterialController.StarLayer1BloomFilterBoost = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star1ColorIntensityKey, timeOfDay);

                if (skyProfile.IsFeatureEnabled(ProfileFeatureKeys.StarLayer1SpriteSheetFeature))
                {
                    m_SkyMaterialController.StarLayer1SpriteItemCount = (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star1SpriteItemCountKey, timeOfDay);
                    m_SkyMaterialController.StarLayer1SpriteAnimationSpeed = (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star1SpriteAnimationSpeedKey, timeOfDay);
                    m_SkyMaterialController.SetStarLayer1SpriteDimensions(
                      (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star1SpriteColumnCountKey, timeOfDay),
                      (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star1SpriteRowCountKey, timeOfDay));
                }
            }

            // Star Layer 2.
            if (skyProfile.IsFeatureEnabled(ProfileFeatureKeys.StarLayer2Feature))
            {
                m_SkyMaterialController.StarLayer2DataTexture = skyProfile.starLayer2DataTexture;
                m_SkyMaterialController.StarLayer2Color = skyProfile.GetColorPropertyValue(ProfilePropertyKeys.Star2ColorKey, timeOfDay);
                m_SkyMaterialController.StarLayer2MaxRadius = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star2SizeKey, timeOfDay); ;
                m_SkyMaterialController.StarLayer2Texture = skyProfile.GetTexturePropertyValue(ProfilePropertyKeys.Star2TextureKey, timeOfDay);
                m_SkyMaterialController.StarLayer2TwinkleAmount = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star2TwinkleAmountKey, timeOfDay);
                m_SkyMaterialController.StarLayer2TwinkleSpeed = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star2TwinkleSpeedKey, timeOfDay);
                m_SkyMaterialController.StarLayer2RotationSpeed = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star2RotationSpeedKey, timeOfDay);
                m_SkyMaterialController.StarLayer2EdgeFeathering = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star2EdgeFeatheringKey, timeOfDay);
                m_SkyMaterialController.StarLayer2BloomFilterBoost = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star2ColorIntensityKey, timeOfDay);

                if (skyProfile.IsFeatureEnabled(ProfileFeatureKeys.StarLayer2SpriteSheetFeature))
                {
                    m_SkyMaterialController.StarLayer2SpriteItemCount = (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star2SpriteItemCountKey, timeOfDay);
                    m_SkyMaterialController.StarLayer2SpriteAnimationSpeed = (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star2SpriteAnimationSpeedKey, timeOfDay);
                    m_SkyMaterialController.SetStarLayer2SpriteDimensions(
                      (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star2SpriteColumnCountKey, timeOfDay),
                      (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star2SpriteRowCountKey, timeOfDay));
                }
            }

            // Star Layer 3.
            if (skyProfile.IsFeatureEnabled(ProfileFeatureKeys.StarLayer3Feature))
            {
                m_SkyMaterialController.StarLayer3DataTexture = skyProfile.starLayer3DataTexture;
                m_SkyMaterialController.StarLayer3Color = skyProfile.GetColorPropertyValue(ProfilePropertyKeys.Star3ColorKey, timeOfDay);
                m_SkyMaterialController.StarLayer3MaxRadius = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star3SizeKey, timeOfDay);
                m_SkyMaterialController.StarLayer3Texture = skyProfile.GetTexturePropertyValue(ProfilePropertyKeys.Star3TextureKey, timeOfDay);
                m_SkyMaterialController.StarLayer3TwinkleAmount = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star3TwinkleAmountKey, timeOfDay);
                m_SkyMaterialController.StarLayer3TwinkleSpeed = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star3TwinkleSpeedKey, timeOfDay);
                m_SkyMaterialController.StarLayer3RotationSpeed = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star3RotationSpeedKey, timeOfDay);
                m_SkyMaterialController.StarLayer3EdgeFeathering = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star3EdgeFeatheringKey, timeOfDay);
                m_SkyMaterialController.StarLayer3BloomFilterBoost = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star3ColorIntensityKey, timeOfDay);

                if (skyProfile.IsFeatureEnabled(ProfileFeatureKeys.StarLayer3SpriteSheetFeature))
                {
                    m_SkyMaterialController.StarLayer3SpriteItemCount = (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star3SpriteItemCountKey, timeOfDay);
                    m_SkyMaterialController.StarLayer3SpriteAnimationSpeed = (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star3SpriteAnimationSpeedKey, timeOfDay);
                    m_SkyMaterialController.SetStarLayer3SpriteDimensions(
                      (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star3SpriteColumnCountKey, timeOfDay),
                      (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star3SpriteRowCountKey, timeOfDay));
                }
            }

            if (updateGlobalIllumination)
            {
                UpdateGlobalIllumination();
            }

            // Notify delegate after we've completed the sky modifications.
            if (timeChangedCallback != null)
            {
                timeChangedCallback(this, timeOfDay);
            }
        }
    }
}