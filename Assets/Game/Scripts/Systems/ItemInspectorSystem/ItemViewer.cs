using System.Collections;

using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ItemViewer
{
	public bool IsTransitionProccess => transitionCoroutine != null;
	private Coroutine transitionCoroutine = null;

	private Transform item;
	private Vector3 lastPosition;
	private Quaternion lastRotation;

	private TransitionSettings settings;
	private Transform origin;
	private AsyncManager asyncManager;

	public ItemViewer(GlobalSettings settings, Transform origin, AsyncManager asyncManager)
	{
		this.settings = settings.transitionSettings;
		this.origin = origin;
		this.asyncManager = asyncManager;
	}

	public ItemViewer SetItem(Transform item)
	{
		this.item = item;
		return this;
	}

	public void TransitionIn(UnityAction onFinish = null)
	{
		StopTransition();

		lastPosition = item.position;
		lastRotation = item.rotation;

		transitionCoroutine = asyncManager.StartCoroutine(
			Transition(
				new TransitionData()
				{
					fromPosition = lastPosition,
					toPosition = origin.position,
					fromRotation = lastRotation,
					toRotation = Quaternion.identity,
					onFinish = onFinish
				}, settings.timeIn));
	}

	public void TransitionOut(UnityAction onFinish = null)
	{
		StopTransition();
		transitionCoroutine = asyncManager.StartCoroutine(
			Transition(new TransitionData() 
			{ 
				fromPosition = origin.position,
				toPosition = lastPosition,
				fromRotation = Quaternion.identity,
				toRotation = lastRotation,
				onFinish = onFinish 
			}, settings.timeOut));
	}

	private IEnumerator Transition(TransitionData data, float time)
	{
		float t = 0;

		while (t < time)
		{
			t += Time.deltaTime;

			item.position = Vector3.Lerp(data.fromPosition, data.toPosition, t / time);
			item.rotation = Quaternion.Lerp(data.fromRotation, data.toRotation, t / time);

			yield return null;
		}

		item.position = data.toPosition;
		item.rotation = data.toRotation;

		StopTransition();

		data.onFinish?.Invoke();
	}

	private void StopTransition()
	{
		if (IsTransitionProccess)
		{
			asyncManager.StopCoroutine(transitionCoroutine);
			transitionCoroutine = null;
		}
	}

	private struct TransitionData
	{
		public UnityAction onFinish;

		public Vector3 fromPosition;
		public Vector3 toPosition;

		public Quaternion fromRotation;
		public Quaternion toRotation;
	}

	[System.Serializable]
	public class TransitionSettings
	{
		public float timeIn = 0.5f;
		public float timeOut = 0.25f;
	}
}