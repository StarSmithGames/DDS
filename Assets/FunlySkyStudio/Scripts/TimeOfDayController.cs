using Game.Systems.TimeSystem;

using Newtonsoft.Json.Linq;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

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
        [SerializeField] private SkyProfile m_SkyProfile;
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
                UpdateSkyForCurrentTime(timeOfDay);
                SynchronizeAllShaderKeywords();
            }
        }

        [Tooltip("Sun orbit.")]
        [SerializeField] private OrbitingBody sunOrbit;

        [Tooltip("Moon orbit.")]
        [SerializeField] private OrbitingBody moonOrbit;

        [Tooltip("Time is expressed in a fractional number of days that have completed.")]
        [Min(0)]
        [SerializeField] private float m_SkyTime = 0;
        public float SkyTime
        {
            get => m_SkyTime;
            set
            {
                m_SkyTime = Mathf.Abs(value);
                UpdateSkyForCurrentTime(timeOfDay);
            }
        }
        // Current progress value through a day cycle (value 0-1).
        public float timeOfDay => m_SkyTime - ((int)m_SkyTime);


        [Tooltip("Create a copy of the sky profile at runtime, so modifications don't affect the original Sky Profile in your project.")]
        public bool copySkyProfile;

        // Use the Sky Material controller to directly manipulate the skybox values programatically.
        private SkyMaterialController m_SkyMaterialController;
        public SkyMaterialController SkyMaterial => m_SkyMaterialController;


        [Tooltip("If true we'll invoke DynamicGI.UpdateEnvironment() when skybox changes. This is an expensive operation.")]
        public bool updateGlobalIllumination = false;

        private bool moonEnable;
        private bool sunEnable;

        private void OnEnabled()
        {
            UpdateSkyForCurrentTime(SkyTime);
        }

        private void OnValidate()
        {
            if (gameObject.activeInHierarchy)
            {
                UpdateSkyForCurrentTime(SkyTime);
            }
        }

        public void UpdateGlobalIllumination()
        {
            DynamicGI.UpdateEnvironment();
        }

        public void UpdateSkyForCurrentTime(float normalDayTime)
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
            m_SkyMaterialController.BackgroundCubemap = skyProfile.GetTexturePropertyValue(ProfilePropertyKeys.SkyCubemapKey, normalDayTime) as Cubemap;
            m_SkyMaterialController.SkyColor = skyProfile.GetColorPropertyValue(ProfilePropertyKeys.SkyUpperColorKey, normalDayTime);
            m_SkyMaterialController.SkyMiddleColor = skyProfile.GetColorPropertyValue(ProfilePropertyKeys.SkyMiddleColorKey, normalDayTime);
            m_SkyMaterialController.HorizonColor = skyProfile.GetColorPropertyValue(ProfilePropertyKeys.SkyLowerColorKey, normalDayTime);
            m_SkyMaterialController.GradientFadeBegin = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.HorizonTrasitionStartKey, normalDayTime);
            m_SkyMaterialController.GradientFadeLength = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.HorizonTransitionLengthKey, normalDayTime);
            m_SkyMaterialController.SkyMiddlePosition = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.SkyMiddleColorPositionKey, normalDayTime);
            m_SkyMaterialController.StarFadeBegin = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.StarTransitionStartKey, normalDayTime);
            m_SkyMaterialController.StarFadeLength = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.StarTransitionLengthKey, normalDayTime);
            m_SkyMaterialController.HorizonDistanceScale = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.HorizonStarScaleKey, normalDayTime);

            // Clouds.
            if (skyProfile.IsFeatureEnabled(ProfileFeatureKeys.NoiseCloudFeature))
            {
                m_SkyMaterialController.CloudTexture = skyProfile.GetTexturePropertyValue(ProfilePropertyKeys.CloudNoiseTextureKey, normalDayTime);
                m_SkyMaterialController.CloudTextureTiling = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.CloudTextureTiling, normalDayTime);
                m_SkyMaterialController.CloudDensity = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.CloudDensityKey, normalDayTime);
                m_SkyMaterialController.CloudSpeed = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.CloudSpeedKey, normalDayTime);
                m_SkyMaterialController.CloudDirection = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.CloudDirectionKey, normalDayTime);
                m_SkyMaterialController.CloudHeight = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.CloudHeightKey, normalDayTime);
                m_SkyMaterialController.CloudColor1 = skyProfile.GetColorPropertyValue(ProfilePropertyKeys.CloudColor1Key, normalDayTime);
                m_SkyMaterialController.CloudColor2 = skyProfile.GetColorPropertyValue(ProfilePropertyKeys.CloudColor2Key, normalDayTime);
                m_SkyMaterialController.CloudFadePosition = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.CloudFadePositionKey, normalDayTime);
                m_SkyMaterialController.CloudFadeAmount = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.CloudFadeAmountKey, normalDayTime);
            }

            // Fog.
            /*if (skyProfile.IsFeatureEnabled(ProfileFeatureKeys.FogFeature))
            {
                Color fogColor = skyProfile.GetColorPropertyValue(ProfilePropertyKeys.FogColorKey, normalDayTime);
                m_SkyMaterialController.FogColor = fogColor;
                m_SkyMaterialController.FogDensity = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.FogDensityKey, normalDayTime);
                m_SkyMaterialController.FogHeight = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.FogLengthKey, normalDayTime);

                // Synchronize with Unity's global fog color so the rest of the scene uses this color fog.
                if (skyProfile.GetBoolPropertyValue(ProfilePropertyKeys.FogSyncWithGlobal, normalDayTime))
                {
                    RenderSettings.fogColor = fogColor;
                }
            }*/

            // Sun.
            if (skyProfile.IsFeatureEnabled(ProfileFeatureKeys.SunFeature) && sunOrbit)
            {
                sunOrbit.spherePoint = skyProfile.GetSpherePointPropertyValue(ProfilePropertyKeys.SunPositionKey, normalDayTime);

                m_SkyMaterialController.SunDirection = sunOrbit.BodyGlobalDirection;
                m_SkyMaterialController.SunColor = skyProfile.GetColorPropertyValue(ProfilePropertyKeys.SunColorKey, normalDayTime);
                m_SkyMaterialController.SunTexture = skyProfile.GetTexturePropertyValue(ProfilePropertyKeys.SunTextureKey, normalDayTime);
                m_SkyMaterialController.SunSize = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.SunSizeKey, normalDayTime);
                m_SkyMaterialController.SunEdgeFeathering = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.SunEdgeFeatheringKey, normalDayTime);
                m_SkyMaterialController.SunBloomFilterBoost = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.SunColorIntensityKey, normalDayTime);

                if (skyProfile.IsFeatureEnabled(ProfileFeatureKeys.SunRotationFeature))
                {
                    m_SkyMaterialController.SunRotationSpeed = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.SunRotationSpeedKey, normalDayTime);
                }

                if (skyProfile.IsFeatureEnabled(ProfileFeatureKeys.SunSpriteSheetFeature))
                {
                    m_SkyMaterialController.SetSunSpriteDimensions(
                      (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.SunSpriteColumnCountKey, normalDayTime),
                      (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.SunSpriteRowCountKey, normalDayTime));
                    m_SkyMaterialController.SunSpriteItemCount = (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.SunSpriteItemCountKey, normalDayTime);
                    m_SkyMaterialController.SunSpriteAnimationSpeed = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.SunSpriteAnimationSpeedKey, normalDayTime);
                }

                if (sunOrbit.BodyLight)
                {
                    if (!sunOrbit.BodyLight.enabled)
                    {
                        sunOrbit.BodyLight.enabled = true;
                    }
                    RenderSettings.sun = sunOrbit.BodyLight;
                    sunOrbit.BodyLight.color = skyProfile.GetColorPropertyValue(ProfilePropertyKeys.SunLightColorKey, normalDayTime);
                    sunOrbit.BodyLight.intensity = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.SunLightIntensityKey, normalDayTime);
                }
            }
            else if (sunOrbit && sunOrbit.BodyLight)
            {
                sunOrbit.BodyLight.enabled = false;
            }

            // Moon.
            if (skyProfile.IsFeatureEnabled(ProfileFeatureKeys.MoonFeature) && moonOrbit)
            {
                moonOrbit.spherePoint = skyProfile.GetSpherePointPropertyValue(ProfilePropertyKeys.MoonPositionKey, normalDayTime);

                m_SkyMaterialController.MoonDirection = moonOrbit.BodyGlobalDirection;
                m_SkyMaterialController.MoonColor = skyProfile.GetColorPropertyValue(ProfilePropertyKeys.MoonColorKey, normalDayTime);
                m_SkyMaterialController.MoonTexture = skyProfile.GetTexturePropertyValue(ProfilePropertyKeys.MoonTextureKey, normalDayTime);
                m_SkyMaterialController.MoonSize = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.MoonSizeKey, normalDayTime);
                m_SkyMaterialController.MoonEdgeFeathering = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.MoonEdgeFeatheringKey, normalDayTime);
                m_SkyMaterialController.MoonBloomFilterBoost = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.MoonColorIntensityKey, normalDayTime);

                if (skyProfile.IsFeatureEnabled(ProfileFeatureKeys.MoonRotationFeature))
                {
                    m_SkyMaterialController.MoonRotationSpeed = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.MoonRotationSpeedKey, normalDayTime);
                }

                if (skyProfile.IsFeatureEnabled(ProfileFeatureKeys.MoonSpriteSheetFeature))
                {
                    m_SkyMaterialController.SetMoonSpriteDimensions(
                      (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.MoonSpriteColumnCountKey, normalDayTime),
                      (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.MoonSpriteRowCountKey, normalDayTime));
                    m_SkyMaterialController.MoonSpriteItemCount = (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.MoonSpriteItemCountKey, normalDayTime);
                    m_SkyMaterialController.MoonSpriteAnimationSpeed = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.MoonSpriteAnimationSpeedKey, normalDayTime);
                }

                if (moonOrbit.BodyLight)
                {
                    if (!moonOrbit.BodyLight.enabled)
                    {
                        moonOrbit.BodyLight.enabled = true;
                    }
                    moonOrbit.BodyLight.color = skyProfile.GetColorPropertyValue(ProfilePropertyKeys.MoonLightColorKey, normalDayTime);
                    moonOrbit.BodyLight.intensity = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.MoonLightIntensityKey, normalDayTime);
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
                m_SkyMaterialController.StarLayer1Color = skyProfile.GetColorPropertyValue(ProfilePropertyKeys.Star1ColorKey, normalDayTime);
                m_SkyMaterialController.StarLayer1MaxRadius = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star1SizeKey, normalDayTime);
                m_SkyMaterialController.StarLayer1Texture = skyProfile.GetTexturePropertyValue(ProfilePropertyKeys.Star1TextureKey, normalDayTime);
                m_SkyMaterialController.StarLayer1TwinkleAmount = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star1TwinkleAmountKey, normalDayTime);
                m_SkyMaterialController.StarLayer1TwinkleSpeed = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star1TwinkleSpeedKey, normalDayTime);
                m_SkyMaterialController.StarLayer1RotationSpeed = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star1RotationSpeedKey, normalDayTime);
                m_SkyMaterialController.StarLayer1EdgeFeathering = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star1EdgeFeatheringKey, normalDayTime);
                m_SkyMaterialController.StarLayer1BloomFilterBoost = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star1ColorIntensityKey, normalDayTime);

                if (skyProfile.IsFeatureEnabled(ProfileFeatureKeys.StarLayer1SpriteSheetFeature))
                {
                    m_SkyMaterialController.StarLayer1SpriteItemCount = (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star1SpriteItemCountKey, normalDayTime);
                    m_SkyMaterialController.StarLayer1SpriteAnimationSpeed = (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star1SpriteAnimationSpeedKey, normalDayTime);
                    m_SkyMaterialController.SetStarLayer1SpriteDimensions(
                      (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star1SpriteColumnCountKey, normalDayTime),
                      (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star1SpriteRowCountKey, normalDayTime));
                }
            }

            // Star Layer 2.
            if (skyProfile.IsFeatureEnabled(ProfileFeatureKeys.StarLayer2Feature))
            {
                m_SkyMaterialController.StarLayer2DataTexture = skyProfile.starLayer2DataTexture;
                m_SkyMaterialController.StarLayer2Color = skyProfile.GetColorPropertyValue(ProfilePropertyKeys.Star2ColorKey, normalDayTime);
                m_SkyMaterialController.StarLayer2MaxRadius = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star2SizeKey, normalDayTime);
                m_SkyMaterialController.StarLayer2Texture = skyProfile.GetTexturePropertyValue(ProfilePropertyKeys.Star2TextureKey, normalDayTime);
                m_SkyMaterialController.StarLayer2TwinkleAmount = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star2TwinkleAmountKey, normalDayTime);
                m_SkyMaterialController.StarLayer2TwinkleSpeed = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star2TwinkleSpeedKey, normalDayTime);
                m_SkyMaterialController.StarLayer2RotationSpeed = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star2RotationSpeedKey, normalDayTime);
                m_SkyMaterialController.StarLayer2EdgeFeathering = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star2EdgeFeatheringKey, normalDayTime);
                m_SkyMaterialController.StarLayer2BloomFilterBoost = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star2ColorIntensityKey, normalDayTime);

                if (skyProfile.IsFeatureEnabled(ProfileFeatureKeys.StarLayer2SpriteSheetFeature))
                {
                    m_SkyMaterialController.StarLayer2SpriteItemCount = (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star2SpriteItemCountKey, normalDayTime);
                    m_SkyMaterialController.StarLayer2SpriteAnimationSpeed = (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star2SpriteAnimationSpeedKey, normalDayTime);
                    m_SkyMaterialController.SetStarLayer2SpriteDimensions(
                      (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star2SpriteColumnCountKey, normalDayTime),
                      (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star2SpriteRowCountKey, normalDayTime));
                }
            }

            // Star Layer 3.
            if (skyProfile.IsFeatureEnabled(ProfileFeatureKeys.StarLayer3Feature))
            {
                m_SkyMaterialController.StarLayer3DataTexture = skyProfile.starLayer3DataTexture;
                m_SkyMaterialController.StarLayer3Color = skyProfile.GetColorPropertyValue(ProfilePropertyKeys.Star3ColorKey, normalDayTime);
                m_SkyMaterialController.StarLayer3MaxRadius = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star3SizeKey, normalDayTime);
                m_SkyMaterialController.StarLayer3Texture = skyProfile.GetTexturePropertyValue(ProfilePropertyKeys.Star3TextureKey, normalDayTime);
                m_SkyMaterialController.StarLayer3TwinkleAmount = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star3TwinkleAmountKey, normalDayTime);
                m_SkyMaterialController.StarLayer3TwinkleSpeed = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star3TwinkleSpeedKey, normalDayTime);
                m_SkyMaterialController.StarLayer3RotationSpeed = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star3RotationSpeedKey, normalDayTime);
                m_SkyMaterialController.StarLayer3EdgeFeathering = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star3EdgeFeatheringKey, normalDayTime);
                m_SkyMaterialController.StarLayer3BloomFilterBoost = skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star3ColorIntensityKey, normalDayTime);

                if (skyProfile.IsFeatureEnabled(ProfileFeatureKeys.StarLayer3SpriteSheetFeature))
                {
                    m_SkyMaterialController.StarLayer3SpriteItemCount = (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star3SpriteItemCountKey, normalDayTime);
                    m_SkyMaterialController.StarLayer3SpriteAnimationSpeed = (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star3SpriteAnimationSpeedKey, normalDayTime);
                    m_SkyMaterialController.SetStarLayer3SpriteDimensions(
                      (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star3SpriteColumnCountKey, normalDayTime),
                      (int)skyProfile.GetNumberPropertyValue(ProfilePropertyKeys.Star3SpriteRowCountKey, normalDayTime));
                }
            }

            if (updateGlobalIllumination)
            {
                UpdateGlobalIllumination();
            }

            // Notify delegate after we've completed the sky modifications.
            //timeChangedCallback?.Invoke(this, normalDayTime);
        }

        public void UpdateLunum(TimeState state)
        {
            if (state == TimeState.Night)
            {
                if (sunEnable)
                {
                    sunOrbit.gameObject.SetActive(false);

                    sunEnable = false;
                }
                if (moonEnable == false)
                {
                    moonOrbit.gameObject.SetActive(true);

                    moonEnable = true;
                }
            }
            else
            {
                if (sunEnable == false)
                {
                    sunOrbit.gameObject.SetActive(true);

                    sunEnable = true;

                }
                if (moonEnable)
                {
                    moonOrbit.gameObject.SetActive(false);

                    moonEnable = false;
                }
            }
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
    }
}
[System.Serializable]
public class EnvironmentSettings
{

}