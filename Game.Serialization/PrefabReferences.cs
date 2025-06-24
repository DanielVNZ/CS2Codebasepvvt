using Game.Prefabs;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Serialization;

public struct PrefabReferences
{
	[ReadOnly]
	private ComponentLookup<PrefabData> m_PrefabData;

	[ReadOnly]
	private NativeArray<Entity> m_PrefabArray;

	private UnsafeList<bool> m_ReferencedPrefabs;

	private Entity m_LastPrefabIn;

	private Entity m_LastPrefabOut;

	private bool m_IsLoading;

	public PrefabReferences(NativeArray<Entity> prefabArray, UnsafeList<bool> referencedPrefabs, ComponentLookup<PrefabData> prefabData, bool isLoading)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		m_PrefabArray = prefabArray;
		m_ReferencedPrefabs = referencedPrefabs;
		m_PrefabData = prefabData;
		m_LastPrefabIn = Entity.Null;
		m_LastPrefabOut = Entity.Null;
		m_IsLoading = isLoading;
	}

	public void SetDirty(Entity prefab)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (!m_IsLoading)
		{
			PrefabData prefabData = m_PrefabData[prefab];
			if (prefabData.m_Index >= 0)
			{
				m_ReferencedPrefabs[prefabData.m_Index] = true;
			}
		}
	}

	public void Check(ref Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		if (prefab != m_LastPrefabIn)
		{
			PrefabData prefabData = m_PrefabData[prefab];
			prefabData.m_Index = math.select(prefabData.m_Index, m_ReferencedPrefabs.Length + prefabData.m_Index, prefabData.m_Index < 0);
			m_LastPrefabIn = prefab;
			if (m_IsLoading)
			{
				m_LastPrefabOut = m_PrefabArray[prefabData.m_Index];
				if (m_LastPrefabOut == m_LastPrefabIn)
				{
					m_ReferencedPrefabs[prefabData.m_Index] = true;
				}
			}
			else
			{
				m_LastPrefabOut = prefab;
				m_ReferencedPrefabs[prefabData.m_Index] = true;
			}
		}
		prefab = m_LastPrefabOut;
	}

	public Entity Check(EntityManager entityManager, Entity prefab)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		if (prefab == Entity.Null)
		{
			return Entity.Null;
		}
		if (m_IsLoading && ((EntityManager)(ref entityManager)).HasComponent<LoadedIndex>(prefab))
		{
			return prefab;
		}
		if (prefab != m_LastPrefabIn)
		{
			PrefabData componentData = ((EntityManager)(ref entityManager)).GetComponentData<PrefabData>(prefab);
			componentData.m_Index = math.select(componentData.m_Index, m_ReferencedPrefabs.Length + componentData.m_Index, componentData.m_Index < 0);
			m_LastPrefabIn = prefab;
			if (m_IsLoading)
			{
				m_LastPrefabOut = m_PrefabArray[componentData.m_Index];
				if (m_LastPrefabOut == m_LastPrefabIn)
				{
					m_ReferencedPrefabs[componentData.m_Index] = true;
				}
			}
			else
			{
				m_LastPrefabOut = prefab;
				m_ReferencedPrefabs[componentData.m_Index] = true;
			}
		}
		return m_LastPrefabOut;
	}
}
