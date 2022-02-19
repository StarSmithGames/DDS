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

		public LocalizationSystem(SignalBus signalBus, SystemLanguage defaultLanguage)
		{
			this.signalBus = signalBus;
			this.defaultLanguage = defaultLanguage;
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

		public string _(string key)
		{
			if (assets[CurrentLanguage.ToString()].TryGetValue(key, out string translation))
			{
				return translation;
			}

			if (assets[defaultLanguage.ToString()].TryGetValue(key, out string translationDefault))
			{
				return translationDefault;
			}

			return "";
		}
	}
}