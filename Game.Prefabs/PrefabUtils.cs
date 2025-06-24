using System.Collections.Generic;
using Colossal.Annotations;
using Colossal.PSI.Common;
using Unity.Collections;
using Unity.Entities;

namespace Game.Prefabs;

public static class PrefabUtils
{
	public static T[] ToArray<T>(HashSet<T> hashSet)
	{
		T[] array = new T[hashSet.Count];
		hashSet.CopyTo(array);
		return array;
	}

	public static bool HasUnlockedPrefab<T>(EntityManager entityManager, EntityQuery unlockQuery) where T : unmanaged
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref unlockQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Unlock> val = ((EntityQuery)(ref unlockQuery)).ToComponentDataArray<Unlock>(AllocatorHandle.op_Implicit((Allocator)3));
			try
			{
				for (int i = 0; i < val.Length; i++)
				{
					if (((EntityManager)(ref entityManager)).HasComponent<T>(val[i].m_Prefab))
					{
						return true;
					}
				}
			}
			finally
			{
				val.Dispose();
			}
		}
		return false;
	}

	public static bool HasUnlockedPrefabAll<T1, T2>(EntityManager entityManager, EntityQuery unlockQuery) where T1 : unmanaged where T2 : unmanaged
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref unlockQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Unlock> val = ((EntityQuery)(ref unlockQuery)).ToComponentDataArray<Unlock>(AllocatorHandle.op_Implicit((Allocator)3));
			try
			{
				for (int i = 0; i < val.Length; i++)
				{
					if (((EntityManager)(ref entityManager)).HasComponent<T1>(val[i].m_Prefab) && ((EntityManager)(ref entityManager)).HasComponent<T2>(val[i].m_Prefab))
					{
						return true;
					}
				}
			}
			finally
			{
				val.Dispose();
			}
		}
		return false;
	}

	public static bool HasUnlockedPrefabAny<T1, T2, T3, T4>(EntityManager entityManager, EntityQuery unlockQuery) where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged where T4 : unmanaged
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref unlockQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Unlock> val = ((EntityQuery)(ref unlockQuery)).ToComponentDataArray<Unlock>(AllocatorHandle.op_Implicit((Allocator)3));
			try
			{
				for (int i = 0; i < val.Length; i++)
				{
					if (((EntityManager)(ref entityManager)).HasComponent<T1>(val[i].m_Prefab) || ((EntityManager)(ref entityManager)).HasComponent<T2>(val[i].m_Prefab) || ((EntityManager)(ref entityManager)).HasComponent<T3>(val[i].m_Prefab) || ((EntityManager)(ref entityManager)).HasComponent<T4>(val[i].m_Prefab))
					{
						return true;
					}
				}
			}
			finally
			{
				val.Dispose();
			}
		}
		return false;
	}

	[CanBeNull]
	public static string GetContentPrerequisite(PrefabBase prefab)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (prefab.TryGet<ContentPrerequisite>(out var component) && component.m_ContentPrerequisite.TryGet<DlcRequirement>(out var component2))
		{
			return PlatformManager.instance.GetDlcName(component2.m_Dlc);
		}
		return null;
	}
}
