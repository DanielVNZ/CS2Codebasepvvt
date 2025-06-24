using System;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Global/", new Type[] { })]
public class PollutionGlobalMode : EntityQueryModePrefab
{
	[BurstCompile]
	private struct ModeJob : IJobChunk
	{
		public float m_GroundPollutionMultiplier;

		public float m_AirPollutionMultiplier;

		public float m_NoisePollutionMultiplier;

		public ComponentTypeHandle<PollutionData> m_PollutionType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<PollutionData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PollutionData>(ref m_PollutionType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				PollutionData pollutionData = nativeArray[i];
				pollutionData.m_GroundPollution *= m_GroundPollutionMultiplier;
				pollutionData.m_AirPollution *= m_AirPollutionMultiplier;
				pollutionData.m_NoisePollution *= m_NoisePollutionMultiplier;
				nativeArray[i] = pollutionData;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[Header("Modify the pollution of buildings, excluding zone buildings.")]
	public float m_GroundPollutionMultiplier;

	public float m_AirPollutionMultiplier;

	public float m_NoisePollutionMultiplier;

	public override EntityQueryDesc GetEntityQueryDesc()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PollutionData>() };
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
		((EntityManager)(ref entityManager)).GetComponentData<PollutionData>(entity);
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
			m_GroundPollutionMultiplier = m_GroundPollutionMultiplier,
			m_NoisePollutionMultiplier = m_NoisePollutionMultiplier,
			m_AirPollutionMultiplier = m_AirPollutionMultiplier,
			m_PollutionType = ((EntityManager)(ref entityManager)).GetComponentTypeHandle<PollutionData>(false)
		}, requestedQuery, deps);
	}

	public unsafe override void RestoreDefaultData(EntityManager entityManager, ref NativeArray<Entity> entities, PrefabSystem prefabSystem)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < entities.Length; i++)
		{
			Entity val = entities[i];
			if (!prefabSystem.TryGetPrefab<PrefabBase>(val, out var prefabBase) || !prefabBase.TryGetExactly<Pollution>(out var component))
			{
				ComponentBase.baseLog.Warn((object)$"Prefab data not found {this} : {((object)(*(Entity*)(&val))/*cast due to .constrained prefix*/).ToString()} : {prefabBase}");
				continue;
			}
			PollutionData componentData = ((EntityManager)(ref entityManager)).GetComponentData<PollutionData>(val);
			componentData.m_GroundPollution = component.m_GroundPollution;
			componentData.m_AirPollution = component.m_AirPollution;
			componentData.m_NoisePollution = component.m_NoisePollution;
			((EntityManager)(ref entityManager)).SetComponentData<PollutionData>(val, componentData);
		}
	}
}
