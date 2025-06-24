using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Global/", new Type[] { })]
public class ZoneServiceConsumptionGlobalMode : EntityQueryModePrefab
{
	[BurstCompile]
	private struct ModeJob : IJobChunk
	{
		public float m_UpkeepMultiplier;

		public float m_ElectricityConsumptionMultiplier;

		public float m_WaterConsumptionMultiplier;

		public float m_GarbageAccumlationMultiplier;

		public float m_TelecomNeedMultiplier;

		public ComponentTypeHandle<ConsumptionData> m_ConsumptionType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<ConsumptionData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ConsumptionData>(ref m_ConsumptionType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				ConsumptionData consumptionData = nativeArray[i];
				consumptionData.m_Upkeep = (int)((float)consumptionData.m_Upkeep * m_UpkeepMultiplier);
				consumptionData.m_ElectricityConsumption *= m_ElectricityConsumptionMultiplier;
				consumptionData.m_WaterConsumption *= m_WaterConsumptionMultiplier;
				consumptionData.m_GarbageAccumulation *= m_GarbageAccumlationMultiplier;
				consumptionData.m_TelecomNeed *= m_TelecomNeedMultiplier;
				nativeArray[i] = consumptionData;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[Header("Modify the upkeep and consumption of zone buildings.")]
	public float m_UpkeepMultiplier;

	public float m_ElectricityConsumptionMultiplier;

	public float m_WaterConsumptionMultiplier;

	public float m_GarbageAccumlationMultiplier;

	public float m_TelecomNeedMultiplier;

	private Dictionary<Entity, ConsumptionData> m_OriginalConsumptionData;

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
			ComponentType.ReadOnly<ConsumptionData>(),
			ComponentType.ReadOnly<SpawnableBuildingData>()
		};
		return val;
	}

	protected override void RecordChanges(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).GetComponentData<ConsumptionData>(entity);
	}

	public override void StoreDefaultData(EntityManager entityManager, ref NativeArray<Entity> entities, PrefabSystem prefabSystem)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		m_OriginalConsumptionData = new Dictionary<Entity, ConsumptionData>(entities.Length);
		for (int i = 0; i < entities.Length; i++)
		{
			Entity val = entities[i];
			ConsumptionData componentData = ((EntityManager)(ref entityManager)).GetComponentData<ConsumptionData>(val);
			m_OriginalConsumptionData.Add(val, componentData);
		}
	}

	public override JobHandle ApplyModeData(EntityManager entityManager, EntityQuery requestedQuery, JobHandle deps)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		return JobChunkExtensions.ScheduleParallel<ModeJob>(new ModeJob
		{
			m_UpkeepMultiplier = m_UpkeepMultiplier,
			m_ElectricityConsumptionMultiplier = m_ElectricityConsumptionMultiplier,
			m_WaterConsumptionMultiplier = m_WaterConsumptionMultiplier,
			m_GarbageAccumlationMultiplier = m_GarbageAccumlationMultiplier,
			m_TelecomNeedMultiplier = m_TelecomNeedMultiplier,
			m_ConsumptionType = ((EntityManager)(ref entityManager)).GetComponentTypeHandle<ConsumptionData>(false)
		}, requestedQuery, deps);
	}

	public override void RestoreDefaultData(EntityManager entityManager, ref NativeArray<Entity> entities, PrefabSystem prefabSystem)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < entities.Length; i++)
		{
			Entity val = entities[i];
			if (m_OriginalConsumptionData.TryGetValue(val, out var value))
			{
				((EntityManager)(ref entityManager)).SetComponentData<ConsumptionData>(val, value);
				continue;
			}
			value = ((EntityManager)(ref entityManager)).GetComponentData<ConsumptionData>(val);
			m_OriginalConsumptionData.Add(val, value);
		}
	}
}
