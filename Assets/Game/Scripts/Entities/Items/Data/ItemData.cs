using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items", fileName = "ItemData")]
public class ItemData : ScriptableObject
{
	public InteractableSettings interact;

	[PreviewField]
	public Sprite itemSprite;
}
public enum ItemRarity
{
	Common,
	Rare,
	Epic,
	Legendary,
	Set,
}