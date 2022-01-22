using System;
using System.Collections.Generic;

using UnityEngine;
using Zenject;

namespace Game.Systems.LocalizationSystem
{
	public class LocalizationSystem : IInitializable, IDisposable
	{
		public SystemLanguage CurrentLanguage { get; private set; }

		private SystemLanguage defaultLanguage = SystemLanguage.English;
		private Dictionary<string, Dictionary<string, string>> assets = new Dictionary<string, Dictionary<string, string>>();//язык, ассеты(название ассета, value)

		private SignalBus signalBus;

		public LocalizationSystem(SignalBus signalBus)
		{
			this.signalBus = signalBus;
		}

		public void Initialize()
		{
			SelectLanguage(SystemLanguage.English);
		}

		public void Dispose()
		{

		}

		public void SelectLanguage(SystemLanguage language)
		{
			CurrentLanguage = language;

			signalBus?.Fire(new SignalLocalizationChanged() { language = language });
		}

		//public string _(string key)
		//{
		//	if (assets[currentLanguage].TryGetValue(key, out string translation))
		//	{
		//		return translation;
		//	}

		//	if (assets[defaultLanguage].TryGetValue(key, out string translationDefault))
		//	{
		//		return translationDefault;
		//	}

		//	return "";
		//}
	}
}