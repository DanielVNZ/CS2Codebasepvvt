using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Global/", new Type[] { })]
public class ServiceCoverageGlobalMode : EntityQueryModePrefab
{
	[BurstCompile]
	private struct ModeJob : IJobChunk
	{
		public float m_RangeMultiplier;

		public float m_CapacityMultiplier;

		public float m_MagnitudeMultiplier;

		public ComponentTypeHandle<CoverageData> m_CoverageType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<CoverageData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CoverageData>(ref m_CoverageType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				CoverageData coverageData = nativeArray[i];
				coverageData.m_Range *= m_RangeMultiplier;
				coverageData.m_Capacity *= m_CapacityMultiplier;
				coverageData.m_Magnitude *= m_MagnitudeMultiplier;
				nativeArray[i] = coverageData;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	public float m_RangeMultiplier;

	public float m_CapacityMultiplier;

	public float m_MagnitudeMultiplier;

	private Dictionary<Entity, CoverageData> m_OriginalCoverageData;

	public override EntityQueryDesc GetEntityQueryDesc()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<CoverageData>() };
		return val;
	}

	protected override void RecordChanges(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).GetComponentData<CoverageData>(entity);
	}

	public override void StoreDefaultData(EntityManager entityManager, ref NativeArray<Entity> entities, PrefabSystem prefabSystem)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		m_OriginalCoverageData = new Dictionary<Entity, CoverageData>();
		for (int i = 0; i < entities.Length; i++)
		{
			Entity val = entities[i];
			CoverageData componentData = ((EntityManager)(ref entityManager)).GetComponentData<CoverageData>(val);
			if (prefabSystem.TryGetPrefab<PrefabBase>(val, out var prefabBase) && prefabBase.TryGetExactly<ServiceCoverage>(out var _))
			{
				m_OriginalCoverageData[val] = componentData;
			}
		}
	}

	public override JobHandle ApplyModeData(EntityManager entityManager, EntityQuery requestedQuery, JobHandle deps)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		return JobChunkExtensions.ScheduleParallel<ModeJob>(new ModeJob
		{
			m_RangeMultiplier = m_RangeMultiplier,
			m_MagnitudeMultiplier = m_MagnitudeMultiplier,
			m_CapacityMultiplier = m_CapacityMultiplier,
			m_CoverageType = ((EntityManager)(ref entityManager)).GetComponentTypeHandle<CoverageData>(false)
		}, requestedQuery, deps);
	}

	public override void RestoreDefaultData(EntityManager entityManager, ref NativeArray<Entity> entities, PrefabSystem prefabSystem)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < entities.Length; i++)
		{
			Entity val = entities[i];
			PrefabBase prefabBase;
			ServiceCoverage component;
			if (m_OriginalCoverageData.TryGetValue(val, out var value))
			{
				((EntityManager)(ref entityManager)).SetComponentData<CoverageData>(val, value);
			}
			else if (prefabSystem.TryGetPrefab<PrefabBase>(val, out prefabBase) && prefabBase.TryGetExactly<ServiceCoverage>(out component))
			{
				CoverageData componentData = ((EntityManager)(ref entityManager)).GetComponentData<CoverageData>(val);
				componentData.m_Range = component.m_Range;
				componentData.m_Capacity = component.m_Capacity;
				componentData.m_Magnitude = component.m_Magnitude;
				((EntityManager)(ref entityManager)).SetComponentData<CoverageData>(val, componentData);
			}
			else
			{
				m_OriginalCoverageData.Add(val, ((EntityManager)(ref entityManager)).GetComponentData<CoverageData>(val));
			}
		}
	}
}
