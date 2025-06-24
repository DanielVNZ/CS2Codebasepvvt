using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Colossal.PSI.Common;
using Colossal.UI;
using Game.Assets;
using Game.Prefabs;
using Game.UI.Editor;
using UnityEngine;

namespace Game.UI.Menu;

public static class AssetUploadUtils
{
	private static ILog sLog = LogManager.GetLogger("AssetUpload");

	public static ExternalLinkData defaultExternalLink => new ExternalLinkData
	{
		m_Type = ExternalLinkInfo.kAcceptedTypes[0].m_Type,
		m_URL = string.Empty
	};

	public static bool LockLinkType(string url, out string type)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		string text = url.ToLower().Trim();
		ExternalLinkInfo[] kAcceptedTypes = ExternalLinkInfo.kAcceptedTypes;
		foreach (ExternalLinkInfo val in kAcceptedTypes)
		{
			string[] uRLs = val.m_URLs;
			foreach (string text2 in uRLs)
			{
				if (text.StartsWith(text2 + "/") && text.Length >= text2.Length + 2)
				{
					type = val.m_Type;
					return true;
				}
				if (text.StartsWith("https://" + text2 + "/") && text.Length >= text2.Length + 10)
				{
					type = val.m_Type;
					return true;
				}
			}
		}
		type = null;
		return false;
	}

	public static bool ValidateExternalLink(ExternalLinkData link)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrWhiteSpace(link.m_URL))
		{
			return true;
		}
		if (LockLinkType(link.m_URL, out var type) && type == link.m_Type)
		{
			return true;
		}
		return false;
	}

	public static bool ValidateExternalLinks(IEnumerable<ExternalLinkData> links)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		foreach (ExternalLinkData link in links)
		{
			if (!ValidateExternalLink(link))
			{
				return false;
			}
		}
		return true;
	}

	public static bool ValidateForumLink(string link)
	{
		if (string.IsNullOrWhiteSpace(link))
		{
			return true;
		}
		return link.ToLower().Contains("paradoxplaza.com");
	}

	public static AssetData CopyPreviewImage(AssetData asset, ILocalAssetDatabase database, AssetDataPath path)
	{
		try
		{
			ImageAsset val = (ImageAsset)(object)((asset is ImageAsset) ? asset : null);
			if (val != null)
			{
				return (AssetData)(object)val.Save((FileFormat)1, path, database);
			}
			TextureAsset val2 = (TextureAsset)(object)((asset is TextureAsset) ? asset : null);
			if (val2 != null)
			{
				return (AssetData)(object)val2.SaveAsImageAsset((FileFormat)1, path, database);
			}
		}
		catch (Exception ex)
		{
			sLog.Error(ex);
		}
		return null;
	}

	public static void CopyAsset(AssetData asset, ILocalAssetDatabase database, Dictionary<AssetData, AssetData> processed, HashSet<ModDependency> externalReferences, bool copyReverseDependencies, bool binaryPackAssets, int platformID = 0)
	{
		if (asset is MapMetadata metadata)
		{
			CopyMap(metadata, database, processed);
			return;
		}
		if (asset is SaveGameMetadata metadata2)
		{
			CopySave(metadata2, database, processed);
			return;
		}
		PrefabAsset val = (PrefabAsset)(object)((asset is PrefabAsset) ? asset : null);
		if (val != null)
		{
			CopyPrefab(val, database, processed, externalReferences, copyReverseDependencies, binaryPackAssets, platformID);
		}
		else
		{
			CopyAssetGeneric(asset, database, processed, keepGuid: true);
		}
	}

	public static void CopyMap(MapMetadata metadata, ILocalAssetDatabase database, Dictionary<AssetData, AssetData> processed)
	{
		MapInfo mapInfo = ((Metadata<MapInfo>)metadata).target.Copy();
		MapData mapData = AssetUploadUtils.CopyAssetGeneric<MapData>(mapInfo.mapData, database, processed, keepGuid: false);
		mapInfo.mapData = mapData;
		if ((AssetData)(object)((Metadata<MapInfo>)metadata).target.preview != (IAssetData)null)
		{
			TextureAsset preview = AssetUploadUtils.CopyAssetGeneric<TextureAsset>(mapInfo.preview, database, processed, keepGuid: false);
			mapInfo.preview = preview;
		}
		if ((AssetData)(object)((Metadata<MapInfo>)metadata).target.thumbnail != (IAssetData)null)
		{
			TextureAsset thumbnail = AssetUploadUtils.CopyAssetGeneric<TextureAsset>(mapInfo.thumbnail, database, processed, keepGuid: false);
			mapInfo.thumbnail = thumbnail;
		}
		if (mapInfo.localeAssets != null)
		{
			LocaleAsset[] array = (LocaleAsset[])(object)new LocaleAsset[mapInfo.localeAssets.Length];
			for (int i = 0; i < mapInfo.localeAssets.Length; i++)
			{
				array[i] = AssetUploadUtils.CopyAssetGeneric<LocaleAsset>(mapInfo.localeAssets[i], database, processed, keepGuid: false);
			}
			mapInfo.localeAssets = array;
		}
		if ((AssetData)(object)mapInfo.climate != (IAssetData)null)
		{
			mapInfo.climate = AssetUploadUtils.CopyAssetGeneric<PrefabAsset>(mapInfo.climate, database, processed, keepGuid: false);
		}
		MapMetadata mapMetadata = (mapInfo.metaData = AssetUploadUtils.CopyAssetGeneric<MapMetadata>(metadata, database, processed, keepGuid: false));
		((Metadata<MapInfo>)mapMetadata).target = mapInfo;
		((AssetData)mapMetadata).Save(false);
	}

	public static void CopySave(SaveGameMetadata metadata, ILocalAssetDatabase database, Dictionary<AssetData, AssetData> processed)
	{
		SaveInfo saveInfo = ((Metadata<SaveInfo>)metadata).target.Copy();
		SaveGameData saveGameData = AssetUploadUtils.CopyAssetGeneric<SaveGameData>(saveInfo.saveGameData, database, processed, keepGuid: false);
		saveInfo.saveGameData = saveGameData;
		if ((AssetData)(object)((Metadata<SaveInfo>)metadata).target.preview != (IAssetData)null)
		{
			TextureAsset preview = AssetUploadUtils.CopyAssetGeneric<TextureAsset>(saveInfo.preview, database, processed, keepGuid: false);
			saveInfo.preview = preview;
		}
		SaveGameMetadata saveGameMetadata = (saveInfo.metaData = AssetUploadUtils.CopyAssetGeneric<SaveGameMetadata>(metadata, database, processed, keepGuid: false));
		((Metadata<SaveInfo>)saveGameMetadata).target = saveInfo;
		((AssetData)saveGameMetadata).Save(false);
	}

	public static void CopyPrefab(PrefabAsset prefabAsset, ILocalAssetDatabase database, Dictionary<AssetData, AssetData> processed, HashSet<ModDependency> externalReferences, bool copyReverseDependencies, bool binaryPackAssets, int platformID = 0)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		HashSet<AssetData> hashSet = new HashSet<AssetData>();
		CollectPrefabAssetDependencies(prefabAsset, hashSet, copyReverseDependencies);
		foreach (AssetData item in hashSet)
		{
			if (processed.ContainsKey(item))
			{
				continue;
			}
			SourceMeta meta = item.GetMeta();
			if (meta.platformID > 0 && meta.platformID != platformID)
			{
				externalReferences.Add(new ModDependency
				{
					m_Id = meta.platformID,
					m_Version = meta.platformVersion
				});
				continue;
			}
			AssetData value;
			if (binaryPackAssets)
			{
				PrefabAsset val = (PrefabAsset)(object)((item is PrefabAsset) ? item : null);
				if (val != null)
				{
					PrefabBase obj = (PrefabBase)(object)val.Load();
					PrefabBase prefabBase = obj.Clone(((Object)obj).name);
					PrefabAsset obj2 = database.AddAsset<PrefabAsset, ScriptableObject>(AssetDataPath.op_Implicit(item.name), (ScriptableObject)(object)prefabBase, Identifier.op_Implicit(item.id));
					obj2.Save((ContentType)0, false, true);
					value = (AssetData)(object)obj2;
					goto IL_00e0;
				}
			}
			value = CopyAssetGeneric(item, database, processed, !(item is LocaleAsset));
			goto IL_00e0;
			IL_00e0:
			processed[item] = value;
		}
	}

	public static T CopyAssetGeneric<T>(T asset, ILocalAssetDatabase database, Dictionary<AssetData, AssetData> processed, bool keepGuid = false) where T : AssetData
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (processed.TryGetValue((AssetData)(object)asset, out var value))
		{
			return (T)(object)((value is T) ? value : null);
		}
		using Stream stream = ((AssetData)asset).GetReadStream();
		T val = database.AddAsset<T>(AssetDataPath.op_Implicit(((AssetData)asset).name), Identifier.op_Implicit((Identifier)(keepGuid ? ((AssetData)asset).id : default(Identifier))));
		using (Stream destination = ((AssetData)val).GetWriteStream())
		{
			stream.CopyTo(destination);
		}
		processed.Add((AssetData)(object)asset, (AssetData)(object)val);
		return val;
	}

	public static AssetData CopyAssetGeneric(AssetData asset, ILocalAssetDatabase database, Dictionary<AssetData, AssetData> processed, bool keepGuid = false)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (processed.TryGetValue(asset, out var value))
		{
			return value;
		}
		using Stream stream = asset.GetReadStream();
		IAssetData obj = database.AddAsset(AssetDataPath.op_Implicit(asset.name), ((object)asset).GetType(), Identifier.op_Implicit((Identifier)(keepGuid ? asset.id : default(Identifier))));
		value = (AssetData)(object)((obj is AssetData) ? obj : null);
		using (Stream destination = value.GetWriteStream())
		{
			stream.CopyTo(destination);
		}
		processed.Add(asset, value);
		return value;
	}

	public static bool TryGetPreview(AssetData asset, out AssetData result)
	{
		if (asset is SaveGameMetadata saveGameMetadata)
		{
			result = (AssetData)(object)((Metadata<SaveInfo>)saveGameMetadata).target.preview;
			return result != (IAssetData)null;
		}
		if (asset is MapMetadata mapMetadata)
		{
			result = (AssetData)(object)((Metadata<MapInfo>)mapMetadata).target.preview;
			return result != (IAssetData)null;
		}
		PrefabAsset val = (PrefabAsset)(object)((asset is PrefabAsset) ? asset : null);
		ImageAsset val2 = default(ImageAsset);
		if (val != null && val.Load() is PrefabBase prefabBase && prefabBase.TryGet<UIObject>(out var component) && UIExtensions.TryGetImageAsset(component.m_Icon, ref val2))
		{
			result = (AssetData)(object)val2;
			return true;
		}
		result = null;
		return false;
	}

	public static string GetImageURI(AssetData asset)
	{
		ImageAsset val = (ImageAsset)(object)((asset is ImageAsset) ? asset : null);
		if (val != null)
		{
			return UIExtensions.ToUri((AssetData)(object)val);
		}
		TextureAsset val2 = (TextureAsset)(object)((asset is TextureAsset) ? asset : null);
		if (val2 != null)
		{
			return UIExtensions.ToUri(val2, (TextureAsset)null, 0);
		}
		return UIExtensions.ToUri(MenuHelpers.defaultPreview, (TextureAsset)null, 0);
	}

	public static void CollectPrefabAssetDependencies(PrefabAsset prefabAsset, HashSet<AssetData> dependencies, bool collectReverseDependencies)
	{
		HashSet<PrefabBase> hashSet = new HashSet<PrefabBase>();
		CollectPrefabDependencies(prefabAsset.Load() as PrefabBase, hashSet, collectReverseDependencies);
		foreach (PrefabBase item in hashSet)
		{
			List<AssetData> list = new List<AssetData>();
			GetAssets(item, list);
			foreach (AssetData item2 in list)
			{
				if (item2.database != AssetDatabase.game)
				{
					dependencies.Add(item2);
				}
			}
		}
	}

	public static void CollectPrefabDependencies(PrefabBase mainPrefab, HashSet<PrefabBase> prefabs, bool collectReverseDependencies)
	{
		if (prefabs.Contains(mainPrefab))
		{
			return;
		}
		Stack<PrefabBase> stack = new Stack<PrefabBase>();
		stack.Push(mainPrefab);
		prefabs.Add(mainPrefab);
		PrefabBase prefabBase = default(PrefabBase);
		while (stack.TryPop(ref prefabBase))
		{
			List<PrefabBase> list = new List<PrefabBase>();
			List<ComponentBase> list2 = new List<ComponentBase>();
			prefabBase.GetComponents(list2);
			foreach (ComponentBase item in list2)
			{
				item.GetDependencies(list);
			}
			if (collectReverseDependencies)
			{
				CollectExtraPrefabDependencies(prefabBase, mainPrefab, list);
			}
			foreach (PrefabBase item2 in list)
			{
				if ((Object)(object)item2 != (Object)null && (AssetData)(object)item2.asset != (IAssetData)null && ((AssetData)item2.asset).database != AssetDatabase.game && !prefabs.Contains(item2))
				{
					stack.Push(item2);
					prefabs.Add(item2);
				}
			}
		}
	}

	private static void CollectExtraPrefabDependencies(PrefabBase prefab, PrefabBase mainPrefab, List<PrefabBase> prefabDependencies)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		if (prefab is ZonePrefab zonePrefab && ((Object)(object)prefab == (Object)(object)mainPrefab || (prefab.TryGet<AssetPackItem>(out var component) && component.m_Packs != null && component.m_Packs.Contains(mainPrefab))))
		{
			foreach (PrefabAsset asset in AssetDatabase.global.GetAssets<PrefabAsset>(default(SearchFilter<PrefabAsset>)))
			{
				if (asset.Load() is PrefabBase prefabBase && prefabBase.TryGet<SpawnableBuilding>(out var component2) && (Object)(object)component2.m_ZoneType == (Object)(object)zonePrefab)
				{
					prefabDependencies.Add(prefabBase);
				}
			}
			return;
		}
		if (!(prefab is AssetPackPrefab value) || !((Object)(object)prefab == (Object)(object)mainPrefab))
		{
			return;
		}
		foreach (PrefabAsset asset2 in AssetDatabase.global.GetAssets<PrefabAsset>(default(SearchFilter<PrefabAsset>)))
		{
			if (asset2.Load() is PrefabBase prefabBase2 && prefabBase2.TryGet<AssetPackItem>(out var component3) && component3.m_Packs != null && component3.m_Packs.Contains(value))
			{
				prefabDependencies.Add(prefabBase2);
			}
		}
	}

	private static void GetAssets(PrefabBase prefab, List<AssetData> assets)
	{
		assets.Add((AssetData)(object)prefab.asset);
		if (prefab is RenderPrefab renderPrefab)
		{
			assets.Add((AssetData)(object)renderPrefab.geometryAsset);
			foreach (SurfaceAsset surfaceAsset in renderPrefab.surfaceAssets)
			{
				assets.Add((AssetData)(object)surfaceAsset);
				surfaceAsset.LoadProperties(false);
				foreach (TextureAsset value in surfaceAsset.textures.Values)
				{
					assets.Add((AssetData)(object)value);
				}
			}
		}
		foreach (LocaleAsset localeAsset in EditorPrefabUtils.GetLocaleAssets(prefab))
		{
			assets.Add((AssetData)(object)localeAsset);
		}
		foreach (EditorPrefabUtils.IconInfo icon in EditorPrefabUtils.GetIcons(prefab))
		{
			assets.Add((AssetData)(object)icon.m_Asset);
		}
	}

	public static void CreateThumbnailAtlas(Dictionary<AssetData, AssetData> processed, ILocalAssetDatabase database)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		AtlasFrame val = new AtlasFrame(0, 0, false, 0);
		HashSet<AssetData> hashSet = new HashSet<AssetData>();
		foreach (AssetData key in processed.Keys)
		{
			PrefabAsset val2 = (PrefabAsset)(object)((key is PrefabAsset) ? key : null);
			if (val2 == null)
			{
				continue;
			}
			foreach (EditorPrefabUtils.IconInfo icon in EditorPrefabUtils.GetIcons((PrefabBase)(object)val2.Load()))
			{
				if (processed.TryGetValue((AssetData)(object)icon.m_Asset, out var value))
				{
					PrefabAsset val3 = (PrefabAsset)processed[(AssetData)(object)val2];
					ComponentBase componentExactly = ((PrefabBase)(object)val3.Load()).GetComponentExactly(((object)icon.m_Component).GetType());
					Debug.Log((object)$"{((AssetData)val3).name}: {value.name}\n{icon.m_Field.DeclaringType?.Name}.{icon.m_Field.Name}: {icon.m_Field.GetValue(componentExactly)}");
					if (val.TryAdd(value.name, ((ImageAsset)value).Load(-1)))
					{
						icon.m_Field.SetValue(componentExactly, "thumbnail://insert thumbnail URI here");
						hashSet.Add(value);
						((AssetData)val3).Save(true);
					}
				}
			}
		}
		if (hashSet.Count <= 0)
		{
			return;
		}
		AtlasAssetExtensions.AddAsset(database, AssetDataPath.op_Implicit("ThumbnailAtlas"), val);
		foreach (AssetData item in hashSet)
		{
			((IAssetDatabase)database).DeleteAsset<AssetData>(item);
		}
	}
}
