using System;
using System.Collections.Generic;
using Game.Economy;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Global/", new Type[] { })]
public class ServiceConsumptionGlobalMode : EntityQueryModePrefab
{
	[BurstCompile]
	private struct ModeJob : IJobChunk
	{
		public float m_UpkeepMultiplier;

		public float m_ElectricityConsumptionMultiplier;

		public float m_WaterConsumptionMultiplier;

		public float m_GarbageAccumlationMultiplier;

		public ComponentTypeHandle<ConsumptionData> m_ConsumptionType;

		public BufferTypeHandle<ServiceUpkeepData> m_ServiceUpkeepType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<ConsumptionData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ConsumptionData>(ref m_ConsumptionType);
			BufferAccessor<ServiceUpkeepData> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ServiceUpkeepData>(ref m_ServiceUpkeepType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				ConsumptionData consumptionData = nativeArray[i];
				consumptionData.m_Upkeep = (int)((float)consumptionData.m_Upkeep * m_UpkeepMultiplier);
				consumptionData.m_ElectricityConsumption = (int)(consumptionData.m_ElectricityConsumption * m_ElectricityConsumptionMultiplier);
				consumptionData.m_WaterConsumption = (int)(consumptionData.m_WaterConsumption * m_WaterConsumptionMultiplier);
				consumptionData.m_GarbageAccumulation = (int)(consumptionData.m_GarbageAccumulation * m_GarbageAccumlationMultiplier);
				nativeArray[i] = consumptionData;
				DynamicBuffer<ServiceUpkeepData> val = bufferAccessor[i];
				for (int j = 0; j < val.Length; j++)
				{
					if (val[j].m_Upkeep.m_Resource == Resource.Money)
					{
						ServiceUpkeepData serviceUpkeepData = val[j];
						serviceUpkeepData.m_Upkeep.m_Amount = (int)((float)serviceUpkeepData.m_Upkeep.m_Amount * m_UpkeepMultiplier);
						val[j] = serviceUpkeepData;
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[Header("Modify the upkeep and consumption of service buildings, excluding zone buildings.")]
	public float m_UpkeepMultiplier;

	public float m_ElectricityConsumptionMultiplier;

	public float m_WaterConsumptionMultiplier;

	public float m_GarbageAccumlationMultiplier;

	private Dictionary<Entity, ServiceUpkeepData> m_CachedUpkeepDatasDatas;

	public override EntityQueryDesc GetEntityQueryDesc()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<ConsumptionData>(),
			ComponentType.ReadOnly<ServiceUpkeepData>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<BuildingData>(),
			ComponentType.ReadOnly<BuildingExtensionData>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<SpawnableBuildingData>() };
		return val;
	}

	protected override void RecordChanges(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).GetBuffer<ServiceUpkeepData>(entity, false);
		((EntityManager)(ref entityManager)).GetComponentData<ConsumptionData>(entity);
	}

	public override void StoreDefaultData(EntityManager entityManager, ref NativeArray<Entity> entities, PrefabSystem prefabSystem)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		m_CachedUpkeepDatasDatas = new Dictionary<Entity, ServiceUpkeepData>(entities.Length);
		for (int i = 0; i < entities.Length; i++)
		{
			Entity val = entities[i];
			DynamicBuffer<ServiceUpkeepData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<ServiceUpkeepData>(val, false);
			for (int j = 0; j < buffer.Length; j++)
			{
				if (buffer[j].m_Upkeep.m_Resource == Resource.Money)
				{
					m_CachedUpkeepDatasDatas.Add(val, buffer[j]);
				}
			}
		}
	}

	public override JobHandle ApplyModeData(EntityManager entityManager, EntityQuery requestedQuery, JobHandle deps)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		return JobChunkExtensions.ScheduleParallel<ModeJob>(new ModeJob
		{
			m_UpkeepMultiplier = m_UpkeepMultiplier,
			m_ElectricityConsumptionMultiplier = m_ElectricityConsumptionMultiplier,
			m_WaterConsumptionMultiplier = m_WaterConsumptionMultiplier,
			m_GarbageAccumlationMultiplier = m_GarbageAccumlationMultiplier,
			m_ConsumptionType = ((EntityManager)(ref entityManager)).GetComponentTypeHandle<ConsumptionData>(false),
			m_ServiceUpkeepType = ((EntityManager)(ref entityManager)).GetBufferTypeHandle<ServiceUpkeepData>(false)
		}, requestedQuery, deps);
	}

	public unsafe override void RestoreDefaultData(EntityManager entityManager, ref NativeArray<Entity> entities, PrefabSystem prefabSystem)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < entities.Length; i++)
		{
			Entity val = entities[i];
			if (!prefabSystem.TryGetPrefab<PrefabBase>(val, out var prefabBase) || !prefabBase.TryGetExactly<ServiceConsumption>(out var component))
			{
				ComponentBase.baseLog.Warn((object)$"Prefab data not found {this} : {((object)(*(Entity*)(&val))/*cast due to .constrained prefix*/).ToString()} : {prefabBase}");
				continue;
			}
			ConsumptionData componentData = ((EntityManager)(ref entityManager)).GetComponentData<ConsumptionData>(val);
			componentData.m_Upkeep = component.m_Upkeep;
			componentData.m_ElectricityConsumption = component.m_ElectricityConsumption;
			componentData.m_WaterConsumption = component.m_WaterConsumption;
			componentData.m_GarbageAccumulation = component.m_GarbageAccumulation;
			((EntityManager)(ref entityManager)).SetComponentData<ConsumptionData>(val, componentData);
			DynamicBuffer<ServiceUpkeepData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<ServiceUpkeepData>(val, false);
			for (int j = 0; j < buffer.Length; j++)
			{
				if (buffer[j].m_Upkeep.m_Resource == Resource.Money)
				{
					if (!m_CachedUpkeepDatasDatas.TryGetValue(val, out var value))
					{
						ComponentBase.baseLog.Critical((object)("Cached ServiceUpkeepData not found " + ((object)(*(Entity*)(&val))/*cast due to .constrained prefix*/).ToString()));
						continue;
					}
					ServiceUpkeepData serviceUpkeepData = buffer[j];
					serviceUpkeepData.m_Upkeep.m_Amount = value.m_Upkeep.m_Amount;
					buffer[j] = serviceUpkeepData;
				}
			}
		}
	}
}
