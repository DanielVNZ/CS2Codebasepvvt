using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Global/", new Type[] { })]
public class WorkPlaceGlobalMode : EntityQueryModePrefab
{
	[BurstCompile]
	private struct ModeJob : IJobChunk
	{
		public float m_WorkplacesMultiplier;

		public ComponentTypeHandle<WorkplaceData> m_WorkplaceType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<WorkplaceData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WorkplaceData>(ref m_WorkplaceType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				WorkplaceData workplaceData = nativeArray[i];
				workplaceData.m_MaxWorkers = (int)((float)workplaceData.m_MaxWorkers * m_WorkplacesMultiplier);
				nativeArray[i] = workplaceData;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	public float m_WorkplacesMultiplier;

	private Dictionary<Entity, WorkplaceData> m_OriginalWorkplaceData;

	public override EntityQueryDesc GetEntityQueryDesc()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<WorkplaceData>(),
			ComponentType.ReadOnly<BuildingData>()
		};
		return val;
	}

	protected override void RecordChanges(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).GetComponentData<WorkplaceData>(entity);
	}

	public override void StoreDefaultData(EntityManager entityManager, ref NativeArray<Entity> entities, PrefabSystem prefabSystem)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		m_OriginalWorkplaceData = new Dictionary<Entity, WorkplaceData>();
		for (int i = 0; i < entities.Length; i++)
		{
			Entity val = entities[i];
			WorkplaceData componentData = ((EntityManager)(ref entityManager)).GetComponentData<WorkplaceData>(val);
			if (prefabSystem.TryGetPrefab<PrefabBase>(val, out var prefabBase) && prefabBase.TryGetExactly<Workplace>(out var _))
			{
				m_OriginalWorkplaceData[val] = componentData;
			}
		}
	}

	public override JobHandle ApplyModeData(EntityManager entityManager, EntityQuery requestedQuery, JobHandle deps)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		return JobChunkExtensions.ScheduleParallel<ModeJob>(new ModeJob
		{
			m_WorkplacesMultiplier = m_WorkplacesMultiplier,
			m_WorkplaceType = ((EntityManager)(ref entityManager)).GetComponentTypeHandle<WorkplaceData>(false)
		}, requestedQuery, deps);
	}

	public override void RestoreDefaultData(EntityManager entityManager, ref NativeArray<Entity> entities, PrefabSystem prefabSystem)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < entities.Length; i++)
		{
			Entity val = entities[i];
			WorkplaceData componentData = ((EntityManager)(ref entityManager)).GetComponentData<WorkplaceData>(val);
			PrefabBase prefabBase;
			Workplace component;
			if (m_OriginalWorkplaceData.TryGetValue(val, out var value))
			{
				componentData.m_MaxWorkers = value.m_MaxWorkers;
			}
			else if (prefabSystem.TryGetPrefab<PrefabBase>(val, out prefabBase) && prefabBase.TryGetExactly<Workplace>(out component))
			{
				componentData.m_MaxWorkers = component.m_Workplaces;
			}
			else
			{
				m_OriginalWorkplaceData.Add(val, ((EntityManager)(ref entityManager)).GetComponentData<WorkplaceData>(val));
			}
			((EntityManager)(ref entityManager)).SetComponentData<WorkplaceData>(val, componentData);
		}
	}
}
