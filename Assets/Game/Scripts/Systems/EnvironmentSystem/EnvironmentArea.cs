using System.Collections;
using System.Linq;

using UnityEngine;

namespace Game.Systems.EnvironmentSystem
{
	public class EnvironmentArea : MonoBehaviour
	{
		[SerializeField] private float radius;
		[SerializeField] private LayerMask targetMask;

		public IEntity[] Entities { get; private set; }
		private Collider[] colliders;

		public bool IsCoverageProcess => coverageCoroutine != null;
		private Coroutine coverageCoroutine = null;
		private WaitForFixedUpdate seconds = new WaitForFixedUpdate();

		public void StartCoverage()
		{
			if (!IsCoverageProcess)
			{
				coverageCoroutine = StartCoroutine(Coverage());
			}
		}

		private IEnumerator Coverage()
		{
			while (true)
			{
				colliders = Physics.OverlapSphere(transform.position, radius, targetMask);

				Entities = colliders.Select((x) =>
				{
					if (x.transform.parent == null)
					{
						return x.GetComponent<IEntity>();
					}

					return x.GetComponentInParent<IEntity>();
				}).ToArray();
				
				yield return seconds;
			}

			StopCoverage();
		}

		public void StopCoverage()
		{
			if (IsCoverageProcess)
			{
				StopCoroutine(coverageCoroutine);
				coverageCoroutine = null;
			}
		}


		private void OnDrawGizmos()
		{
			Color color = Color.white;

			Gizmos.color = color;
			if(colliders != null)
			{
				for (int i = 0; i < colliders.Length; i++)
				{
					Gizmos.DrawLine(transform.position, colliders[i].transform.position);
				}
			}

			color = Color.yellow;
			color.a = IsCoverageProcess ? 1f : 0.35f;

			Gizmos.color = color;

			Gizmos.DrawWireSphere(transform.position, radius);
		}
	}
}