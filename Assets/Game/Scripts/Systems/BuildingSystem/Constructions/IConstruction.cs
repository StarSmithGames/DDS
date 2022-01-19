using UnityEngine;

public interface IConstruction : IEntity, IInteractable
{
	ConstructionData ConstructionData { get; }

	void SetMaterial(Material material);
}