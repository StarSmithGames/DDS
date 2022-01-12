using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items", fileName = "ItemData")]
public class ItemData : ScriptableObject
{
	[PreviewField]
	public Sprite itemSprite;

	public string itemName;
	[TextArea(5, 5)]
	public string itemDescription;

	[Space]
	public InteractableSettings interact;
	[Space]
	[AssetList]
	[InlineEditor(InlineEditorModes.GUIAndPreview)]
	public ItemModel prefab;

	public Vector3 prefabPossitionOffsetView;
	public Quaternion prefabRotationOffsetView;

	[Space]
	public bool isInfinityStack = false;
	[HideIf("isInfinityStack")]
	[Range(1, 999)]
	public int stackSize = 1;

	[Space]
	public bool isInfinityWeight = false;
	[HideIf("isInfinityWeight")]
	[SuffixLabel("kg", true)]
	[Range(0.01f, 99.99f)]
	public float weight = 0.01f;

	[Space]
	public bool isBreakable = false;
	[ShowIf("isBreakable")]
	public DecaySettings decay;
	
}
[InlineProperty]
[System.Serializable]
public class DecaySettings
{
	[Range(0, 100)]
	[SuffixLabel("%/day")]
	public float decayOverTime = 0f;
	[Range(0, 100)]
	[SuffixLabel("%/day")]
	public float decayInside = 0f;
	[Range(0, 100)]
	[SuffixLabel("%/day")]
	public float decayOutsie = 0f;
}

public enum ItemRarity
{
	Common,
	Rare,
	Epic,
	Legendary,
	Set,
}