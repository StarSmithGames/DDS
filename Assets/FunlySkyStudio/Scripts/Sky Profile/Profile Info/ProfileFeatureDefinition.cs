using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Funly.SkyStudio
{
  [Serializable]
  public class ProfileFeatureDefinition : System.Object
  {
    public enum FeatureType
    {
      ShaderKeyword,
      BooleanValue
    }

    public string featureKey;
    public FeatureType featureType;
    public string shaderKeyword;
    public string name;
    public bool value;
    public string tooltip;
    public string dependsOnFeature;
    public bool dependsOnValue;
    public bool isShaderKeywordFeature;

    // Feature that uses a shader keyword.
    public static ProfileFeatureDefinition CreateShaderFeature(
      string featureKey, string shaderKeyword, bool value, string name, 
      string dependsOnFeature, bool dependsOnValue, string tooltip)
    {
      ProfileFeatureDefinition feature = new ProfileFeatureDefinition();
      feature.featureType = FeatureType.ShaderKeyword;
      feature.featureKey = featureKey;
      feature.shaderKeyword = shaderKeyword;
      feature.name = name;
      feature.value = value;
      feature.tooltip = tooltip;
      feature.dependsOnFeature = dependsOnFeature;
      feature.dependsOnValue = dependsOnValue;

      return feature;
    }

    // Feature that's just a boolean flag.
    public static ProfileFeatureDefinition CreateBooleanFeature(
      string featureKey, bool value, string name,
      string dependsOnFeature, bool dependsOnValue, string tooltip)
    {
      ProfileFeatureDefinition feature = new ProfileFeatureDefinition();
      feature.featureType = FeatureType.BooleanValue;
      feature.featureKey = featureKey;
      feature.name = name;
      feature.value = value;
      feature.tooltip = tooltip;

      return feature;
    }
  }
}

