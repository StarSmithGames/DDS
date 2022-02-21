using UnityEngine;

namespace Game.Systems.EnvironmentSystem
{
	public class FogNormal : MonoBehaviour
	{
		private Material material = null;
		public Material Material
		{
			get
			{
				if(material == null)
				{
					material = GetComponent<Renderer>().material;
				}

				return material;
			}
		}
	}
}