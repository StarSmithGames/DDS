using System.Collections.Generic;

using UnityEngine;
using Sirenix.OdinInspector;

namespace Game.Systems.LocalizationSystem
{
	public class LocalizationData : ScriptableObject
	{
		[ReadOnly]
		public string Id;
		[ReadOnly]
		[ListDrawerSettings(ShowIndexLabels = true, IsReadOnly = true)]
		public List<string> Keys = new List<string>();
		[ReadOnly]
		[ListDrawerSettings(ShowIndexLabels = true, IsReadOnly = true)]
		public List<string> Values = new List<string>();

		public Dictionary<string, string> pars = new Dictionary<string, string>();

		public void CreatePars()
		{
			pars.Clear();

			for (int i = 0; i < Keys.Count; i++)
			{
				pars.Add(Keys[i], Values[i]);
			}
		}

		public override string ToString()
		{
			string result = "";

			for (int i = 0; i < Keys.Count; i++)
			{
				result += $"{Keys[i]} => {Values[i]}\n";
			}

			return result;
		}
	}
}