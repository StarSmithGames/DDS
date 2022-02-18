using UnityEngine;

namespace Game.Systems.EnvironmentSystem
{
	public class UI3DWindArrow : MonoBehaviour
	{
		public Transform Arrow => arrow;
		[SerializeField] private Transform arrow;
	}
}