using System;
using System.Collections.Generic;

using UnityEngine;
using Zenject;

namespace Game.Systems.LocalizationSystem
{
	public class LocalizationSystem : IInitializable, IDisposable
	{
		public static SystemLanguage CurrentLanguageStatic { get; private set; }

		public SystemLanguage CurrentLanguage { get; private set; }

		private SystemLanguage defaultLanguage = SystemLanguage.English;
		private Dictionary<SystemLanguage, LocalizationData> dictionary = new Dictionary<SystemLanguage, LocalizationData>();

		private SignalBus signalBus;

		public LocalizationSystem(SignalBus signalBus, SystemLanguage defaultLanguage, List<LocalizationData> localizations)
		{
			this.signalBus = signalBus;
			this.defaultLanguage = defaultLanguage;

			for (int i = 0; i < localizations.Count; i++)
			{
				localizations[i].CreatePars();

				var lang = ConvertToSystemLanguage(localizations[i].Id);
				if (!dictionary.ContainsKey(lang))
				{
					dictionary.Add(lang, localizations[i]);
				}
				else
				{
					Debug.LogError("Dictionary Contains => " + lang);
				}
			}
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
			CurrentLanguageStatic = language;

			signalBus?.Fire(new SignalLocalizationChanged() { language = language });
		}

		public string _(string key)
		{
			if (dictionary.TryGetValue(CurrentLanguage, out LocalizationData translation))
			{
				return translation.pars[key];
			}

			if (dictionary.TryGetValue(CurrentLanguage, out LocalizationData translationDefault))
			{
				return translationDefault.pars[key];
			}

			return "";
		}

		// :/
		public string GetDeclension(int number, string key)
		{
			var nominativ = key;
			var genetiv = key + "_genetiv";
			var plural = key + "_plural";

			number = number % 100;

			if (number >= 11 && number <= 19)
			{
				return plural;
			}

			var i = number % 10;

			switch (i)
			{
				case 1:
				return nominativ;
				case 2:
				case 3:
				case 4:
				return genetiv;
				default:
				return plural;
			}
		}


		private SystemLanguage ConvertToSystemLanguage(string language)
		{
			switch (language) 
			{
				case "ru":
				{
					return SystemLanguage.Russian;
				}
				case "en":
				{
					return SystemLanguage.English;
				}
				case "de":
				{
					return SystemLanguage.German;
				}
				case "fr":
				{
					return SystemLanguage.French;
				}
				case "es":
				{
					return SystemLanguage.Spanish;
				}
				case "pt-br":
				{
					return SystemLanguage.Portuguese;
				}
				case "it":
				{
					return SystemLanguage.Italian;
				}
				case "ja":
				{
					return SystemLanguage.Japanese;
				}
				case "ko":
				{
					return SystemLanguage.Korean;
				}
				case "zhs":
				{
					return SystemLanguage.Chinese;
				}

				default:
				{
					return SystemLanguage.English;
				}
			}
		}
	}
}