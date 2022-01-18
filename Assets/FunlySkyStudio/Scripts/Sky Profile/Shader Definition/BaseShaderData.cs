using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Funly.SkyStudio
{
  [Serializable]
  public abstract class BaseShaderDefinition : IProfileDefinition
  {
    public string shaderName { get; protected set; }

    // Definition of shader parameters.
    private ProfileGroupSection[] m_ProfileDefinitions;
    public ProfileGroupSection[] groups
    {
      get { return m_ProfileDefinitions ?? (m_ProfileDefinitions = ProfileDefinitionTable()); }
    }

    // Shader features.
    [SerializeField]
    private ProfileFeatureSection[] m_ProfileFeatures;
    //private Dictionary<string, ProfileFeatureSection>
    public ProfileFeatureSection[] features
    {
      get { return m_ProfileFeatures ?? (m_ProfileFeatures = ProfileFeatureSection()); }
    }

    private Dictionary<string, ProfileFeatureDefinition> m_KeyToFeature;
    public ProfileFeatureDefinition GetFeatureDefinition(string featureKey)
    {
      // Build a table mapping on first access.
      if (m_KeyToFeature == null) {
        m_KeyToFeature = new Dictionary<string, ProfileFeatureDefinition>();
        foreach (ProfileFeatureSection section in features) {
          foreach (ProfileFeatureDefinition feature in section.featureDefinitions) {
            m_KeyToFeature[feature.featureKey] = feature;
          }
        }
      }

      if (featureKey == null) {
        return null;
      }

      if (m_KeyToFeature.ContainsKey(featureKey) == false) {
        return null;
      }

      return m_KeyToFeature[featureKey];
    }

    // Override and return shader keyword info.
    protected abstract ProfileFeatureSection[] ProfileFeatureSection();
    
    // Override and return shader property info.
    protected abstract ProfileGroupSection[] ProfileDefinitionTable();
  }
}

