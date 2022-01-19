using Game.Entities;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

public class ParentTest : MonoBehaviour
{
	protected SignalBus signalBus;

	[Inject]
	private void Construct(SignalBus signalBus)
	{
		this.signalBus = signalBus;
	}
}
