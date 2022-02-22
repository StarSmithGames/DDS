using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour
{
	[SerializeField] private Settings settings;

	[SerializeField] private float delay = 0.2f;

	private Coroutine viewCoroutine = null;
	public bool IsViewProccess => viewCoroutine != null;
	private WaitForSeconds seconds;

	private List<Transform> visibleTargets = new List<Transform>();

	public void StartView()
	{
		if (!IsViewProccess)
		{
			seconds = new WaitForSeconds(delay);
			viewCoroutine = StartCoroutine(FindTargetsWithDelay());
		}
	}
	private IEnumerator FindTargetsWithDelay()
	{
		while (true)
		{
			yield return seconds;
			FindVisibleTargets();
		}

		StopView();
	}
	public void StopView()
	{
		if (IsViewProccess)
		{
			StopCoroutine(viewCoroutine);
			viewCoroutine = null;
		}
	}

	private void FindVisibleTargets()
	{
		visibleTargets.Clear();
		Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, settings.viewRadius, settings.targetMask);

		for (int i = 0; i < targetsInViewRadius.Length; i++)
		{
			Transform target = targetsInViewRadius[i].transform;
			Vector3 dirToTarget = (target.position - transform.position).normalized;
			if (Vector3.Angle(transform.forward, dirToTarget) < settings.viewAngle / 2)
			{
				float dstToTarget = Vector3.Distance(transform.position, target.position);

				if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, settings.obstacleMask))
				{
					visibleTargets.Add(target);
				}
			}
		}
	}


	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;

		for (int i = 0; i < visibleTargets.Count; i++)
		{
			Gizmos.DrawLine(transform.position, visibleTargets[i].position);
		}
	}

	[System.Serializable]
	public class Settings
	{
		public float viewRadius = 10f;
		public float farthestZone = 8f;
		public float nearestZone = 3f;
		[Space]
		[Range(0, 360f)]
		public float viewAngle = 90f;
		[Range(0, 360f)]
		public float farthestAngle = 90f;
		[Range(0, 360f)]
		public float nearestAngle = 90f;
		[Space]
		public LayerMask targetMask;
		public LayerMask obstacleMask;
	}
}