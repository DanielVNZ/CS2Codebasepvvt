using Colossal.IO.AssetDatabase;
using Colossal.PSI.Environment;
using Game.Assets;
using Game.PSI.PdxSdk;
using Game.Settings;

namespace Game.SceneFlow;

public static class SaveHelpers
{
	public const string kSaveLoadTaskName = "SaveLoadGame";

	public static AssetDataPath GetAssetDataPath<T>(ILocalAssetDatabase database, string saveName)
	{
		AssetDataPath result = AssetDataPath.op_Implicit(saveName);
		if (!((IDataSourceAccessor)database).dataSource.isRemoteStorageSource)
		{
			string specialPath = EnvPath.GetSpecialPath<T>();
			if (specialPath != null)
			{
				result = AssetDataPath.Create(specialPath, saveName, (EscapeStrategy)2);
			}
		}
		return result;
	}

	public static void DeleteSaveGame(SaveGameMetadata saveGameMetadata)
	{
		UserState userState = GameManager.instance.settings.userState;
		if ((AssetData)(object)userState.lastSaveGameMetadata == (IAssetData)(object)saveGameMetadata)
		{
			userState.lastSaveGameMetadata = null;
			userState.ApplyAndSave();
			Launcher.DeleteLastSaveMetadata();
		}
		AssetDatabase.global.DeleteAsset<SaveGameMetadata>(saveGameMetadata);
	}
}
