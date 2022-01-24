
using System.Collections.Generic;

using UnityEngine;

using Zenject;

namespace Game.Systems.BuildingSystem
{
	public class ConstructionModel : MonoBehaviour, IConstruction
	{
		[SerializeField] protected ConstructionData constructionData;
		public ConstructionData ConstructionData => constructionData;

		public Transform Transform => transform;

		public List<Collider> Intersections { get; private set; }
		public bool IsIntersectsColliders { get => Intersections.Count > 0; }
		[SerializeField] protected bool isPlaced = true;
		public bool IsPlaced
		{
			get => isPlaced;
			set
			{
				isPlaced = value;

				if (isPlaced == true)
				{
					Intersections.Clear();
				}
			}
		}

		[SerializeField] protected Collider coll;
		[SerializeField] private List<Renderer> renderers = new List<Renderer>();

		private List<Material> materials = new List<Material>();

		protected UIManager uiManager;
		protected LocalizationSystem.LocalizationSystem localization;
		protected LayerMask ignoringLayers;

		[Inject]
		private void Construct(UIManager uiManager, LocalizationSystem.LocalizationSystem localization, BuildingSystemSettings buildingSettings)
		{
			this.uiManager = uiManager;
			this.localization = localization;
			this.ignoringLayers = buildingSettings.groundLayers;

			for (int i = 0; i < renderers.Count; i++)
			{
				materials.Add(renderers[i].material);
			}

			Intersections = new List<Collider>();
		}

		public virtual void Interact()
		{
			Debug.LogError("Interact");
		}

		public void StartObserve()
		{
			if (IsPlaced)
			{
				var text = constructionData.GetLocalization(localization.CurrentLanguage);
				uiManager.Targets.ShowTargetInformation(text.constructionName);
			}
		}

		public virtual void Observe() { }

		public void EndObserve()
		{
			if (IsPlaced)
			{
				uiManager.Targets.HideTargetInformation();
			}
		}

		public void SetMaterial(Material material)
		{
			for (int i = 0; i < renderers.Count; i++)
			{
				renderers[i].material = material;
			}
		}

		public void ResetMaterial()
		{
			for (int i = 0; i < renderers.Count; i++)
			{
				renderers[i].material = materials[i];
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (IsPlaced) return;

			if (((1 << other.gameObject.layer) & ignoringLayers) != 0) return;

			if (!Intersections.Contains(other))
			{
				Intersections.Add(other);
			}
		}
		private void OnTriggerExit(Collider other)
		{
			if (IsPlaced) return;

			if (Intersections.Contains(other))
			{
				Intersections.Remove(other);
			}
		}

		public class Factory : PlaceholderFactory<ConstructionBlueprint, IConstruction> { }
	}
}