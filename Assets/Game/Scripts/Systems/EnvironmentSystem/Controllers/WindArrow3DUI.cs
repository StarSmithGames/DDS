using UnityEngine;

namespace Game.Systems.EnvironmentSystem
{
	public class WindArrow3DUI : MonoBehaviour
	{
		public Transform Arrow => arrow;
		[SerializeField] private Transform arrow;
	}
}