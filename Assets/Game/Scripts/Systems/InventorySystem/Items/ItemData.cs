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
	public InteractionSettings interact = new InteractionSettings() { interactionType = InteractionType.Click };
	[Space]
	[AssetList]
	[InlineEditor(InlineEditorModes.GUIAndPreview)]
	public ItemModel prefab;

	public Vector3 prefabPossitionOffsetView;
	public Quaternion prefabRotationOffsetView;

	[Space]
	public bool isStackable = true;
	[ShowIf("isStackable")]
	public bool isInfinityStack = false;
	[ShowIf("@isStackable && !isInfinityStack")]
	[Range(1, 999)]
	public int stackSize = 1;

	[Space]
	public bool isInfinityWeight = false;
	public bool isFloatingWeight = false;
	[ShowIf("isFloatingWeight")]
	[SuffixLabel("kg", true)]
	[MinValue(0.01f)]
	public float baseWeight = 0.25f;
	[HideIf("isInfinityWeight")]
	[SuffixLabel("kg", true)]
	[MinValue("MinimumWeight"), MaxValue(99.99f)]
	public float weight = 0.01f;

	[Space]
	public bool isBreakable = false;
	[ShowIf("isBreakable")]
	public DecaySettings decay;


	private float MinimumWeight => isFloatingWeight ? baseWeight + 0.15f : 0.01f;
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