using System;
using System.Collections.Generic;
using System.Reflection;
using Colossal;
using Colossal.Annotations;
using Colossal.IO.AssetDatabase;
using Colossal.UI;
using Game.Prefabs;
using Game.UI.Localization;
using Game.UI.Widgets;
using UnityEngine;

namespace Game.UI.Editor;

public static class EditorPrefabUtils
{
	public struct IconInfo
	{
		public ImageAsset m_Asset;

		public string m_URI;

		public FieldInfo m_Field;

		public ComponentBase m_Component;
	}

	private static readonly Dictionary<Type, string[]> s_PrefabTypes = new Dictionary<Type, string[]>();

	private static readonly Dictionary<Type, string[]> s_PrefabTags = new Dictionary<Type, string[]>();

	public static readonly LocalizedString kNone = LocalizedString.Id("Editor.NONE_VALUE");

	public static string GetPrefabTypeName(Type type)
	{
		return type.FullName;
	}

	[CanBeNull]
	public static PrefabBase GetPrefabByID([CanBeNull] string prefabID)
	{
		return null;
	}

	[CanBeNull]
	public static T GetPrefabByID<T>([CanBeNull] string prefabID) where T : PrefabBase
	{
		return null;
	}

	[CanBeNull]
	public static string GetPrefabID(PrefabBase prefab)
	{
		string result = default(string);
		if ((Object)(object)prefab != (Object)null && AssetDatabase.global.resources.prefabsMap.TryGetGuid((Object)(object)prefab, ref result))
		{
			return result;
		}
		return null;
	}

	public static string[] GetPrefabTypes(Type type)
	{
		if (s_PrefabTypes.TryGetValue(type, out var value))
		{
			return value;
		}
		List<string> list = new List<string>();
		Type type2 = type;
		while (type2 != null && type2 != typeof(PrefabBase) && typeof(PrefabBase).IsAssignableFrom(type2))
		{
			list.Add(GetPrefabTypeName(type2));
			type2 = type2.BaseType;
		}
		return s_PrefabTypes[type] = list.ToArray();
	}

	public static string[] GetPrefabTags(Type type)
	{
		if (s_PrefabTags.TryGetValue(type, out var value))
		{
			return value;
		}
		List<string> list = new List<string>();
		Type type2 = type;
		while (type2 != null && type2 != typeof(PrefabBase) && typeof(PrefabBase).IsAssignableFrom(type2))
		{
			string text = type2.Name;
			if (text.Length > 6 && text.EndsWith("Prefab"))
			{
				text = text.Substring(0, text.Length - 6);
			}
			list.Add(text.ToLowerInvariant());
			type2 = type2.BaseType;
		}
		return s_PrefabTags[type] = list.ToArray();
	}

	public static void SavePrefab(PrefabBase prefab)
	{
		((AssetData)(prefab.asset ?? PrefabAssetExtensions.AddAsset(AssetDatabase.user, AssetDataPath.Create("StreamingData~/" + ((Object)prefab).name, ((Object)prefab).name ?? "", (EscapeStrategy)2), (ScriptableObject)(object)prefab))).Save(false);
	}

	public static IEnumerable<LocaleAsset> GetLocaleAssets(PrefabBase prefab)
	{
		if (!((AssetData)(object)prefab.asset != (IAssetData)null) || ((AssetData)prefab.asset).database == AssetDatabase.game)
		{
			yield break;
		}
		foreach (LocaleAsset asset in AssetDatabase.global.GetAssets<LocaleAsset>(SearchFilter<LocaleAsset>.ByCondition((Func<LocaleAsset, bool>)((LocaleAsset a) => ((AssetData)a).subPath == prefab.asset.subPath), false)))
		{
			yield return asset;
		}
	}

	public static IEnumerable<IconInfo> GetIcons(PrefabBase prefab)
	{
		ImageAsset asset = default(ImageAsset);
		foreach (ComponentBase comp in prefab.components)
		{
			FieldInfo[] fields = ((object)comp).GetType().GetFields();
			foreach (FieldInfo fieldInfo in fields)
			{
				if (fieldInfo.FieldType != typeof(string))
				{
					continue;
				}
				CustomFieldAttribute customAttribute = ((MemberInfo)fieldInfo).GetCustomAttribute<CustomFieldAttribute>();
				if (customAttribute != null && !(customAttribute.Factory != typeof(UIIconField)))
				{
					string text = (string)fieldInfo.GetValue(comp);
					if (UIExtensions.TryGetImageAsset(text, ref asset))
					{
						yield return new IconInfo
						{
							m_Asset = asset,
							m_URI = text,
							m_Field = fieldInfo,
							m_Component = comp
						};
					}
				}
			}
		}
	}

	public static LocalizedString GetPrefabLabel(PrefabBase prefab)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)prefab == (Object)null)
		{
			return kNone;
		}
		if ((AssetData)(object)prefab.asset != (IAssetData)null)
		{
			SourceMeta meta = ((AssetData)prefab.asset).GetMeta();
			if ((object)((AssetData)prefab.asset).database == AssetDatabase<ParadoxMods>.instance)
			{
				return LocalizedString.Value($"{((Object)prefab).name} - ({meta.platformID})");
			}
			if (((AssetData)prefab.asset).database == AssetDatabase.user)
			{
				string text = (((SourceMeta)(ref meta)).packaged ? meta.packageName : ((AssetData)prefab.asset).name);
				return LocalizedString.Value(((Object)prefab).name + " - (" + text + ")");
			}
		}
		return LocalizedString.Value(((Object)prefab).name);
	}

	public static IEnumerable<AssetItem> GetUserImages()
	{
		yield return new AssetItem
		{
			guid = default(Hash128),
			displayName = kNone
		};
		foreach (ImageAsset asset in AssetDatabase.global.GetAssets<ImageAsset>(SearchFilter<ImageAsset>.ByCondition((Func<ImageAsset, bool>)((ImageAsset a) => ((AssetData)a).GetMeta().subPath?.StartsWith(ScreenUtility.kScreenshotDirectory) ?? false), false)))
		{
			ImageAsset val = asset;
			try
			{
				yield return new AssetItem
				{
					guid = Identifier.op_Implicit(((AssetData)asset).id),
					fileName = ((AssetData)asset).name,
					displayName = ((AssetData)asset).name,
					image = UIExtensions.ToUri((AssetData)(object)asset)
				};
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}
}
