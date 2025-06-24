using System;
using System.Collections.Generic;
using System.Linq;
using Colossal;
using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Colossal.UI;
using Colossal.UI.Binding;
using Game.Assets;
using Game.SceneFlow;

namespace Game.UI.Menu;

public static class MenuHelpers
{
	public class SaveGamePreviewSettings
	{
		public bool stylized { get; set; }

		public float stylizedRadius { get; set; }

		public TextureAsset overlayImage { get; set; }

		public SaveGamePreviewSettings()
		{
			SetDefaults();
		}

		public void SetDefaults()
		{
			stylized = false;
			stylizedRadius = 0f;
			overlayImage = null;
		}

		public void FromUri(UrlQuery query)
		{
			bool flag = default(bool);
			if (query.Read("stylized", ref flag))
			{
				stylized = flag;
			}
			float num = default(float);
			if (query.Read("stylizedRadius", ref num))
			{
				stylizedRadius = num;
			}
			TextureAsset val = default(TextureAsset);
			if (query.ReadAsset<TextureAsset>("overlayImage", ref val))
			{
				overlayImage = val;
			}
		}

		public string ToUri()
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			object arg = stylized;
			object arg2 = stylizedRadius;
			TextureAsset obj = overlayImage;
			return $"stylized={arg}&stylizedRadius={arg2}&overlayImage={((obj != null) ? new Hash128?(((AssetData)obj).id.guid) : ((Hash128?)null))}";
		}
	}

	private static ILog log = LogManager.GetLogger("SceneFlow");

	public const int kPreviewWidth = 680;

	public const int kPreviewHeight = 383;

	public static TextureAsset defaultPreview => ((IAssetDatabase)AssetDatabase.game).GetAsset<TextureAsset>("cc1e5421d5a16f15bbd580cffdbee7d4");

	public static TextureAsset defaultThumbnail => ((IAssetDatabase)AssetDatabase.game).GetAsset<TextureAsset>("735aa687f0dd7cda5e7d1aa4c4987b26");

	public static bool hasPreviouslySavedGame => GameManager.instance.settings.userState.lastSaveGameMetadata?.isValidSaveGame ?? false;

	public static SaveGameMetadata GetLastModifiedSave()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		SaveGameMetadata result = null;
		DateTime dateTime = DateTime.MinValue;
		foreach (SaveGameMetadata asset in AssetDatabase.global.GetAssets<SaveGameMetadata>(default(SearchFilter<SaveGameMetadata>)))
		{
			DateTime lastModified = ((Metadata<SaveInfo>)asset).target.lastModified;
			if (lastModified > dateTime)
			{
				dateTime = lastModified;
				result = asset;
			}
		}
		return result;
	}

	public static void UpdateMeta<T>(ValueBinding<List<T>> binding, Func<Metadata<T>, bool> filter = null) where T : IContentPrerequisite
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		List<T> value = binding.value;
		value.Clear();
		foreach (Metadata<T> asset in AssetDatabase.global.GetAssets<Metadata<T>>(default(SearchFilter<Metadata<T>>)))
		{
			try
			{
				if (filter == null || filter(asset))
				{
					value.Add(asset.target);
				}
			}
			catch (Exception ex)
			{
				log.WarnFormat(ex, "An error occured while updating {0}", (object)asset);
			}
		}
		binding.TriggerUpdate();
	}

	public static List<string> GetAvailableCloudTargets()
	{
		return (from x in AssetDatabase.global.GetAvailableRemoteStorages()
			select x.name).ToList();
	}

	public static (string name, ILocalAssetDatabase db) GetSanitizedCloudTarget(string cloudTarget)
	{
		(string, ILocalAssetDatabase) result = default((string, ILocalAssetDatabase));
		foreach (var availableRemoteStorage in AssetDatabase.global.GetAvailableRemoteStorages())
		{
			if (availableRemoteStorage.Item1 == cloudTarget)
			{
				return availableRemoteStorage;
			}
			if (availableRemoteStorage.Item1 == "Local")
			{
				result = availableRemoteStorage;
			}
		}
		return result;
	}
}
