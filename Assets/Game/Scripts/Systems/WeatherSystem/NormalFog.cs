using UnityEngine;

namespace Game.Systems.WeatherSystem
{
	public class NormalFog : MonoBehaviour
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