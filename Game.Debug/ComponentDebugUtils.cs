using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;

namespace Game.Debug;

public static class ComponentDebugUtils
{
	public class ComponentInfo
	{
		public Type m_Type;

		public int m_ArchetypeCount;

		public int m_EntityCount;

		public int m_ChunkCapacity;

		public int m_ChunkCount;

		public bool m_Matching;
	}

	public static List<ComponentInfo> GetCommonComponents(EntityManager entityManager, string filter, bool unusedOnly, out int archetypeCount, out int filteredArchetypeCount, out int chunkCount, out int chunkCapacity, out int entityCount)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<ComponentType, ComponentInfo> dictionary = new Dictionary<ComponentType, ComponentInfo>();
		NativeList<EntityArchetype> val = default(NativeList<EntityArchetype>);
		val._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		((EntityManager)(ref entityManager)).GetAllArchetypes(val);
		archetypeCount = val.Length;
		filteredArchetypeCount = 0;
		Enumerator<EntityArchetype> enumerator = val.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				EntityArchetype current = enumerator.Current;
				NativeArray<ComponentType> componentTypes = ((EntityArchetype)(ref current)).GetComponentTypes((Allocator)2);
				bool flag = false;
				if (!string.IsNullOrEmpty(filter))
				{
					flag = true;
					Enumerator<ComponentType> enumerator2 = componentTypes.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							ComponentType current2 = enumerator2.Current;
							if (dictionary.TryGetValue(current2, out var value))
							{
								if (value.m_Matching)
								{
									flag = false;
									break;
								}
							}
							else if (IsMatching(current2, filter))
							{
								flag = false;
								break;
							}
						}
					}
					finally
					{
						((IDisposable)enumerator2/*cast due to .constrained prefix*/).Dispose();
					}
				}
				if (!flag && (!unusedOnly || ((EntityArchetype)(ref current)).ChunkCount == 0))
				{
					filteredArchetypeCount++;
					Enumerator<ComponentType> enumerator2 = componentTypes.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							ComponentType current3 = enumerator2.Current;
							if (dictionary.TryGetValue(current3, out var value2))
							{
								value2.m_ArchetypeCount++;
								value2.m_ChunkCount++;
								continue;
							}
							dictionary[current3] = new ComponentInfo
							{
								m_Type = ((ComponentType)(ref current3)).GetManagedType(),
								m_ArchetypeCount = 1,
								m_Matching = IsMatching(current3, filter),
								m_ChunkCount = ((EntityArchetype)(ref current)).ChunkCount
							};
						}
					}
					finally
					{
						((IDisposable)enumerator2/*cast due to .constrained prefix*/).Dispose();
					}
				}
				componentTypes.Dispose();
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		val.Dispose();
		entityCount = 0;
		chunkCount = 0;
		chunkCapacity = 0;
		NativeArray<ArchetypeChunk> allChunks = ((EntityManager)(ref entityManager)).GetAllChunks((Allocator)2);
		Enumerator<ArchetypeChunk> enumerator3 = allChunks.GetEnumerator();
		try
		{
			while (enumerator3.MoveNext())
			{
				ArchetypeChunk current4 = enumerator3.Current;
				entityCount += ((ArchetypeChunk)(ref current4)).Count;
				chunkCount++;
				chunkCapacity += ((ArchetypeChunk)(ref current4)).Capacity;
				EntityArchetype archetype = ((ArchetypeChunk)(ref current4)).Archetype;
				NativeArray<ComponentType> componentTypes2 = ((EntityArchetype)(ref archetype)).GetComponentTypes((Allocator)2);
				Enumerator<ComponentType> enumerator2 = componentTypes2.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						ComponentType current5 = enumerator2.Current;
						if (dictionary.TryGetValue(current5, out var value3))
						{
							value3.m_EntityCount += ((ArchetypeChunk)(ref current4)).Count;
							value3.m_ChunkCapacity += ((ArchetypeChunk)(ref current4)).Capacity;
						}
					}
				}
				finally
				{
					((IDisposable)enumerator2/*cast due to .constrained prefix*/).Dispose();
				}
				componentTypes2.Dispose();
			}
		}
		finally
		{
			((IDisposable)enumerator3/*cast due to .constrained prefix*/).Dispose();
		}
		allChunks.Dispose();
		return (from pair in dictionary
			select pair.Value into pair
			orderby pair.m_ArchetypeCount descending
			select pair).Take(100).ToList();
	}

	private static bool IsMatching(ComponentType type, string filter)
	{
		return ((ComponentType)(ref type)).GetManagedType().FullName.ToLower().Contains(filter.ToLower());
	}
}
