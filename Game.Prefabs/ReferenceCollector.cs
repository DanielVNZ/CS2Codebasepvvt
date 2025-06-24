using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Colossal;
using Colossal.IO.AssetDatabase;
using UnityEngine;

namespace Game.Prefabs;

public class ReferenceCollector
{
	public class CollectedReferences
	{
		private readonly HashSet<PrefabBase> m_PrefabReferences = new HashSet<PrefabBase>();

		private readonly HashSet<ScriptableObject> m_ScriptableObjectReferences = new HashSet<ScriptableObject>();

		private readonly HashSet<AssetReference> m_AssetReferences = new HashSet<AssetReference>();

		private readonly HashSet<AssetData> m_AssetDatas = new HashSet<AssetData>();

		public IReadOnlyCollection<PrefabBase> prefabReferences => m_PrefabReferences;

		public IReadOnlyCollection<ScriptableObject> scriptableObjectReferences => m_ScriptableObjectReferences;

		public IReadOnlyCollection<AssetReference> assetReferences => m_AssetReferences;

		public IReadOnlyCollection<AssetData> assetDatas => m_AssetDatas;

		public void Add(object obj)
		{
			if (obj == null)
			{
				return;
			}
			if (obj is PrefabBase item)
			{
				m_PrefabReferences.Add(item);
				return;
			}
			if (!(obj is ComponentBase))
			{
				ScriptableObject val = (ScriptableObject)((obj is ScriptableObject) ? obj : null);
				if (val != null)
				{
					m_ScriptableObjectReferences.Add(val);
					return;
				}
			}
			AssetReference val2 = (AssetReference)((obj is AssetReference) ? obj : null);
			if (val2 != null)
			{
				m_AssetReferences.Add(val2);
				return;
			}
			AssetData val3 = (AssetData)((obj is AssetData) ? obj : null);
			if (val3 != null)
			{
				m_AssetDatas.Add(val3);
			}
		}
	}

	private readonly Dictionary<object, bool> m_VisitedObjects;

	private readonly Dictionary<Type, FieldInfo[]> m_CachedFields;

	public ReferenceCollector()
	{
		m_VisitedObjects = new Dictionary<object, bool>();
		m_CachedFields = new Dictionary<Type, FieldInfo[]>(100);
	}

	public CollectedReferences CollectDependencies(IEnumerable<IAssetData> objs, bool addRoot)
	{
		m_VisitedObjects.Clear();
		CollectedReferences collectedReferences = new CollectedReferences();
		foreach (IAssetData obj in objs)
		{
			TraverseObject(obj, collectedReferences);
			if (addRoot)
			{
				collectedReferences.Add(obj);
			}
		}
		return collectedReferences;
	}

	public CollectedReferences CollectDependencies(IAssetData obj)
	{
		m_VisitedObjects.Clear();
		CollectedReferences collectedReferences = new CollectedReferences();
		TraverseObject(obj, collectedReferences);
		return collectedReferences;
	}

	private void TraverseSurfaceAsset(SurfaceAsset surfaceAsset, CollectedReferences references)
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		if ((AssetData)(object)surfaceAsset == (IAssetData)null || !TryAddVisited(surfaceAsset))
		{
			return;
		}
		surfaceAsset.LoadProperties(false);
		foreach (KeyValuePair<string, TextureAsset> texture in surfaceAsset.textures)
		{
			references.Add(texture.Value);
		}
		if (!surfaceAsset.isVTMaterial)
		{
			return;
		}
		references.Add(surfaceAsset.vtSurfaceAsset);
		for (int i = 0; i < surfaceAsset.stackCount; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				Hash128 preProcessedTextureGuid = surfaceAsset.GetPreProcessedTextureGuid(i, j);
				references.Add(AssetDatabase.global.GetAsset(preProcessedTextureGuid));
			}
		}
	}

	private void TraverseObject(object obj, CollectedReferences references)
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		if (obj == null || !TryAddVisited(obj))
		{
			return;
		}
		if (obj is PrefabBase prefabBase)
		{
			references.Add(prefabBase);
			references.Add(prefabBase.asset);
		}
		else
		{
			PrefabAsset val = (PrefabAsset)((obj is PrefabAsset) ? obj : null);
			if (val != null)
			{
				TraverseObject(val.Load<PrefabBase>(), references);
			}
			else
			{
				SurfaceAsset val2 = (SurfaceAsset)((obj is SurfaceAsset) ? obj : null);
				if (val2 != null)
				{
					TraverseSurfaceAsset(val2, references);
				}
				else if (obj is AssetReference<SurfaceAsset> val3)
				{
					TraverseSurfaceAsset(AssetReference<SurfaceAsset>.op_Implicit(val3), references);
				}
			}
		}
		AssetReference val4 = (AssetReference)((obj is AssetReference) ? obj : null);
		if (val4 != null)
		{
			references.Add(AssetDatabase.global.GetAsset(val4.guid));
		}
		Type type = obj.GetType();
		if (!m_CachedFields.TryGetValue(type, out var value))
		{
			List<FieldInfo> list = type.GetFields(BindingFlags.Instance | BindingFlags.Public).ToList();
			list.AddRange(from field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
				where ((MemberInfo)field).GetCustomAttribute<SerializeField>() != null
				select field);
			value = list.ToArray();
			m_CachedFields.Add(type, value);
		}
		FieldInfo[] array = value;
		for (int num = 0; num < array.Length; num++)
		{
			object value2 = array[num].GetValue(obj);
			if (value2 == null)
			{
				continue;
			}
			references.Add(value2);
			if (value2 is IDictionary dictionary)
			{
				foreach (DictionaryEntry item in dictionary)
				{
					references.Add(item.Key);
					references.Add(item.Value);
					TraverseObject(item.Key, references);
					TraverseObject(item.Value, references);
				}
			}
			else if (value2 is IEnumerable enumerable)
			{
				foreach (object item2 in enumerable)
				{
					references.Add(item2);
					TraverseObject(item2, references);
				}
			}
			else
			{
				TraverseObject(value2, references);
			}
		}
	}

	private bool TryAddVisited(object obj)
	{
		return m_VisitedObjects.TryAdd(obj, true);
	}
}
