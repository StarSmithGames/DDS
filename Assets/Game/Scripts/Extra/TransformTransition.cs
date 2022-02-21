using Sirenix.OdinInspector;

using System.Collections;

using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TransformTransition
{
	public bool IsTransitionProcess => transitionCoroutine != null;
	private Coroutine transitionCoroutine = null;

	private Transform item;
	private Vector3 lastPosition;
	private Quaternion lastRotation;

	private Settings settings;
	private Transform origin;
	private AsyncManager asyncManager;

	public TransformTransition(GlobalSettings settings, Transform origin, AsyncManager asyncManager)
	{
		this.settings = settings.transitionSettings;
		this.origin = origin;
		this.asyncManager = asyncManager;
	}

	public TransformTransition SetItem(Transform item)
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
			Transition(new TransitionData()
			{
				transition = settings.transtionIn,
				onFinish = onFinish,
				fromPosition = lastPosition,
				toPosition = origin.position,
				fromRotation = lastRotation,
				toRotation = Quaternion.identity,
			}));
	}

	public void In()
	{
		item.position = origin.position;
		item.rotation = Quaternion.identity;
	}

	public void TransitionOut(UnityAction onFinish = null)
	{
		StopTransition();
		transitionCoroutine = asyncManager.StartCoroutine(
			Transition(new TransitionData() 
			{
				transition = settings.transtionOut,
				onFinish = onFinish,
				fromPosition = origin.position,
				toPosition = lastPosition,
				fromRotation = Quaternion.identity,
				toRotation = lastRotation,
			}));
	}

	private IEnumerator Transition(TransitionData data)
	{
		float t = 0;

		if (data.transition.type == TransitionType.Custom)
		{ 
		
		}
		else if(data.transition.type == TransitionType.LinearInterpolation)
		{
			float time = data.transition.time;
			while (t < time)
			{
				t += Time.deltaTime;

				item.position = Vector3.Lerp(data.fromPosition, data.toPosition, t / time);
				item.rotation = Quaternion.Lerp(data.fromRotation, data.toRotation, t / time);

				yield return null;
			}
		}
		else if (data.transition.type == TransitionType.SmoothStep)
		{

		}

		item.position = data.toPosition;
		item.rotation = data.toRotation;

		StopTransition();

		data.onFinish?.Invoke();
	}

	private void StopTransition()
	{
		if (IsTransitionProcess)
		{
			asyncManager.StopCoroutine(transitionCoroutine);
			transitionCoroutine = null;
		}
	}

	private struct TransitionData
	{
		public TransitionSettings transition;

		public UnityAction onFinish;

		public Vector3 fromPosition;
		public Vector3 toPosition;

		public Quaternion fromRotation;
		public Quaternion toRotation;
	}

	[System.Serializable]
	public class Settings
	{
		public TransitionSettings transtionIn;
		public TransitionSettings transtionOut;
	}
	[System.Serializable]
	public class TransitionSettings
	{
		public TransitionType type = TransitionType.LinearInterpolation;
		[HideIf("type", TransitionType.Custom)]
		public float time;
		[ShowIf("type", TransitionType.Custom)]
		public AnimationCurve curve;
	}
}
public enum TransitionType
{
	Custom				= 0,
	LinearInterpolation = 1,
	SmoothStep			= 2,
}