using Game.Systems.LocalizationSystem;

using System.Collections.Generic;

using UnityEngine;

using Zenject;

public class ConstructionModel : MonoBehaviour, IConstruction
{
	[SerializeField] private ConstructionData constructionData;
	public ConstructionData ConstructionData => constructionData;

	[SerializeField] private List<Renderer> renderers = new List<Renderer>();

	private UIManager uiManager;
	private LocalizationSystem localization;

	[Inject]
	private void Construct(UIManager uiManager, LocalizationSystem localization)
	{
		this.uiManager = uiManager;
		this.localization = localization;
	}

	public void Interact()
	{
		Debug.LogError("Interact");
	}

	public void StartObserve()
	{
		var text = constructionData.GetLocalization(localization.CurrentLanguage);
		uiManager.Targets.ShowTargetInformation(text.constructionName);
	}

	public virtual void Observe()
	{
	}

	public void EndObserve()
	{
		uiManager.Targets.HideTargetInformation();
	}

	public void SetMaterial(Material material)
	{
		for (int i = 0; i < renderers.Count; i++)
		{
			renderers[i].material = material;
		}
	}
}