using System;
using Colossal.Entities;
using Game.Prefabs;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace Game.UI.InGame;

public static class ProgressionUtils
{
	public static bool CollectSubRequirements(EntityManager entityManager, Entity prefab, NativeParallelHashMap<Entity, UnlockFlags> requiredPrefabs, UnlockFlags flags = UnlockFlags.RequireAll | UnlockFlags.RequireAny)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		if (prefab == Entity.Null || (requiredPrefabs.ContainsKey(prefab) && (requiredPrefabs[prefab] & flags) != 0))
		{
			return false;
		}
		DynamicBuffer<UnlockRequirement> val = default(DynamicBuffer<UnlockRequirement>);
		if (EntitiesExtensions.TryGetBuffer<UnlockRequirement>(entityManager, prefab, true, ref val))
		{
			for (int i = 0; i < val.Length; i++)
			{
				UnlockRequirement unlockRequirement = val[i];
				if (unlockRequirement.m_Prefab == prefab && (unlockRequirement.m_Flags & UnlockFlags.RequireAll) != 0)
				{
					return true;
				}
			}
			for (int j = 0; j < val.Length; j++)
			{
				UnlockRequirement unlockRequirement2 = val[j];
				if (unlockRequirement2.m_Prefab == prefab)
				{
					continue;
				}
				requiredPrefabs.Add(prefab, UnlockFlags.RequireAll | UnlockFlags.RequireAny);
				if (CollectSubRequirements(entityManager, unlockRequirement2.m_Prefab, requiredPrefabs, unlockRequirement2.m_Flags))
				{
					if (requiredPrefabs.ContainsKey(unlockRequirement2.m_Prefab))
					{
						Entity prefab2 = unlockRequirement2.m_Prefab;
						requiredPrefabs[prefab2] |= unlockRequirement2.m_Flags;
					}
					else
					{
						requiredPrefabs.Add(unlockRequirement2.m_Prefab, unlockRequirement2.m_Flags);
					}
				}
				requiredPrefabs.Remove(prefab);
			}
		}
		return false;
	}

	public static int GetRequiredMilestone(EntityManager entityManager, Entity entity)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		if (((EntityManager)(ref entityManager)).HasComponent<UnlockRequirement>(entity))
		{
			NativeParallelHashMap<Entity, UnlockFlags> requiredPrefabs = default(NativeParallelHashMap<Entity, UnlockFlags>);
			requiredPrefabs._002Ector(10, AllocatorHandle.op_Implicit((Allocator)3));
			CollectSubRequirements(entityManager, entity, requiredPrefabs);
			Enumerator<Entity, UnlockFlags> enumerator = requiredPrefabs.GetEnumerator();
			try
			{
				MilestoneData milestoneData = default(MilestoneData);
				while (enumerator.MoveNext())
				{
					KeyValue<Entity, UnlockFlags> current = enumerator.Current;
					if ((current.Value & UnlockFlags.RequireAll) != 0 && EntitiesExtensions.TryGetComponent<MilestoneData>(entityManager, current.Key, ref milestoneData) && milestoneData.m_Index > num)
					{
						num = milestoneData.m_Index;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
			requiredPrefabs.Dispose();
		}
		return num;
	}
}
