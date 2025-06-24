using System;
using System.IO;
using Colossal;
using Colossal.Json;
using Colossal.PSI.Environment;
using Game.Assets;
using Game.SceneFlow;
using Unity.Mathematics;
using UnityEngine;

namespace Game.PSI.PdxSdk;

public static class Launcher
{
	internal static class LocaleID
	{
		public const string kPopulation = "Population";

		public const string kPopulationID = "GameListScreen.POPULATION_LABEL";

		public const string kMoney = "Money";

		public const string kMoneyID = "GameListScreen.MONEY_LABEL";

		public const string kMoneyValue = "{0}¢{1}";

		public const string kMoneyValueID = "Common.VALUE_MONEY";

		public const string kUnlimitedMoney = "Unlimited";

		public const string kUnlimitedMoneyID = "Menu.UNLIMITED_MONEY_LABEL";
	}

	private class SaveInfoData
	{
		public string title;

		public string desc;

		public string date;

		public string rawGameVersion;
	}

	private const string kLastSaveInfoFileName = "continue_game.json";

	private static readonly string kLastSaveInfoPath = EnvPath.kUserDataPath + "/continue_game.json";

	private static string LocalizedString(string id, string def)
	{
		string result = default(string);
		if (GameManager.instance.localizationManager.activeDictionary.TryGetValue(id, ref result))
		{
			return result;
		}
		return def;
	}

	private static string FormatMoney(int money, bool unlimitedMoney)
	{
		if (unlimitedMoney)
		{
			return LocalizedString("Menu.UNLIMITED_MONEY_LABEL", "Unlimited");
		}
		return string.Format(LocalizedString("Common.VALUE_MONEY", "{0}¢{1}").Replace("SIGN", "0").Replace("VALUE", "1"), ((float)math.sign(money) < 0f) ? "-" : "", money);
	}

	public static void SaveLastSaveMetadata(SaveInfo saveInfo)
	{
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			SaveInfoData saveInfoData = new SaveInfoData();
			saveInfoData.title = saveInfo.cityName;
			saveInfoData.desc = string.Format("{0}: {1} {2}: {3}", LocalizedString("GameListScreen.POPULATION_LABEL", "Population"), saveInfo.population, LocalizedString("GameListScreen.MONEY_LABEL", "Money"), FormatMoney(saveInfo.money, saveInfo.options != null && saveInfo.options["unlimitedMoney"]));
			saveInfoData.date = saveInfo.lastModified.ToString("s");
			Version current = Version.current;
			saveInfoData.rawGameVersion = ((Version)(ref current)).shortVersion;
			SaveInfoData saveInfoData2 = saveInfoData;
			File.WriteAllText(kLastSaveInfoPath, JSON.Dump((object)saveInfoData2, (EncodeOptions)0));
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	public static void DeleteLastSaveMetadata()
	{
		if (File.Exists(kLastSaveInfoPath))
		{
			File.Delete(kLastSaveInfoPath);
		}
	}
}
