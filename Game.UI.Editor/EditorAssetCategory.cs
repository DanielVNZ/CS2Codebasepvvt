using System;
using System.Collections.Generic;
using System.Linq;
using Game.Prefabs;
using Game.UI.Localization;
using Unity.Collections;
using Unity.Entities;

namespace Game.UI.Editor;

public class EditorAssetCategory
{
	public static readonly string kNameFormat = "Editor.ASSET_CATEGORY_TITLE[{0}]";

	private List<EditorAssetCategory> m_SubCategories = new List<EditorAssetCategory>();

	public IReadOnlyList<EditorAssetCategory> subCategories => m_SubCategories;

	public string id { get; set; }

	public string path { get; set; }

	public EntityQuery entityQuery { get; set; }

	public EditorAssetCategorySystem.IEditorAssetCategoryFilter filter { get; set; }

	private HashSet<Entity> exclude { get; set; }

	private List<Entity> include { get; set; }

	public string icon { get; set; }

	public bool includeChildCategories { get; set; } = true;

	public bool defaultSelection { get; set; }

	public void AddSubCategory(EditorAssetCategory category)
	{
		m_SubCategories.Add(category);
	}

	public HashSet<PrefabBase> GetPrefabs(EntityManager entityManager, PrefabSystem prefabSystem, EntityTypeHandle entityType)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		HashSet<PrefabBase> hashSet = new HashSet<PrefabBase>();
		foreach (Entity entity in GetEntities(entityManager, prefabSystem, entityType))
		{
			if (prefabSystem.TryGetPrefab<PrefabBase>(entity, out var prefab))
			{
				hashSet.Add(prefab);
			}
		}
		return hashSet;
	}

	public IEnumerable<Entity> GetEntities(EntityManager entityManager, PrefabSystem prefabSystem, EntityTypeHandle entityType)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		EntityQuery val = entityQuery;
		EntityQuery val2 = default(EntityQuery);
		if (val != val2)
		{
			val2 = entityQuery;
			NativeArray<ArchetypeChunk> chunks = ((EntityQuery)(ref val2)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)2));
			try
			{
				int i = 0;
				while (i < chunks.Length)
				{
					ArchetypeChunk val3 = chunks[i];
					NativeArray<Entity> entities = ((ArchetypeChunk)(ref val3)).GetNativeArray(entityType);
					int num;
					for (int j = 0; j < entities.Length; j = num)
					{
						if (CheckFilters(entities[j], entityManager, prefabSystem))
						{
							yield return entities[j];
						}
						num = j + 1;
					}
					num = i + 1;
					i = num;
				}
			}
			finally
			{
				((IDisposable)chunks/*cast due to .constrained prefix*/).Dispose();
			}
		}
		if (includeChildCategories)
		{
			foreach (EditorAssetCategory subCategory in m_SubCategories)
			{
				foreach (Entity entity in subCategory.GetEntities(entityManager, prefabSystem, entityType))
				{
					yield return entity;
				}
			}
		}
		if (include == null)
		{
			yield break;
		}
		foreach (Entity item in include)
		{
			yield return item;
		}
	}

	public bool IsEmpty(EntityManager entityManager, PrefabSystem prefabSystem, EntityTypeHandle entityType)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		EntityQuery val = entityQuery;
		EntityQuery val2 = default(EntityQuery);
		if (val != val2)
		{
			val2 = entityQuery;
			if (!((EntityQuery)(ref val2)).IsEmptyIgnoreFilter && filter == null && exclude == null)
			{
				return false;
			}
		}
		return !GetEntities(entityManager, prefabSystem, entityType).Any();
	}

	public void AddExclusion(Entity entity)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (exclude == null)
		{
			exclude = new HashSet<Entity>(1);
		}
		exclude.Add(entity);
	}

	public void AddEntity(Entity entity)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (include == null)
		{
			include = new List<Entity>(1);
		}
		include.Add(entity);
	}

	public string GetLocalizationID()
	{
		return string.Format(kNameFormat, path);
	}

	private bool CheckFilters(Entity entity, EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (filter == null || filter.Contains(entity, entityManager, prefabSystem))
		{
			if (exclude != null)
			{
				return !exclude.Contains(entity);
			}
			return true;
		}
		return false;
	}

	public HierarchyItem<EditorAssetCategory> ToHierarchyItem(int level = 0)
	{
		string localizationID = GetLocalizationID();
		return new HierarchyItem<EditorAssetCategory>
		{
			m_Data = this,
			m_DisplayName = LocalizedString.IdWithFallback(localizationID, id),
			m_Level = level,
			m_Icon = icon,
			m_Selectable = true,
			m_Selected = defaultSelection,
			m_Expandable = (m_SubCategories.Count > 0),
			m_Expanded = false
		};
	}
}
