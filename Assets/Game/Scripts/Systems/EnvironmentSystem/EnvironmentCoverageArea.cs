using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.EnvironmentSystem
{
	public class EnvironmentCoverageArea : MonoBehaviour
	{
		public bool IsEnabled { get => isEnabled; set => isEnabled = value; }
		[SerializeField] private bool isEnabled = false;
		[Space]
		[SerializeField] private float radius;
		[SerializeField] private LayerMask targetMask;
		[SerializeField] private Weather coverage;

		public float FeelsLike => coverage.air.airTemperature + coverage.wind.windchill;

		public Collider[] Colliders { get; private set; }

		private Vector3 origin;

		private void Update()
		{
			if (!isEnabled) return;

			origin = transform.position;

			Colliders = Physics.OverlapSphere(origin, radius, targetMask);

			for (int i = 0; i < Colliders.Length; i++)
			{
				Debug.LogError(Colliders[i].transform.gameObject.name + "  " + (Colliders[i].GetComponent<IEntity>() != null));
			}
		}


		private void OnDrawGizmos()
		{
			Color color = Color.white;

			Gizmos.color = color;
			if(Colliders != null)
			{
				for (int i = 0; i < Colliders.Length; i++)
				{
					Gizmos.DrawLine(origin, Colliders[i].transform.position);
				}
			}

			if (FeelsLike < 0)
			{
				color = FeelsLike < -15 ? Color.blue : Color.cyan;
			}
			else
			{
				color = FeelsLike > 15 ? Color.red : Color.yellow;
			}

			color.a = isEnabled ? 1f : 0.35f;

			Gizmos.color = color;

			Gizmos.DrawWireSphere(transform.position, radius);
		}
	}
}