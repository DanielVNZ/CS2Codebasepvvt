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
public class ZonePollutionGlobalMode : EntityQueryModePrefab
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

	[Header("Modify the pollution of zone buildings, excluding others.")]
	public float m_GroundPollutionMultiplier;

	public float m_AirPollutionMultiplier;

	public float m_NoisePollutionMultiplier;

	private Dictionary<Entity, PollutionData> m_OriginalPollutionData;

	public override EntityQueryDesc GetEntityQueryDesc()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<SpawnableBuildingData>(),
			ComponentType.ReadOnly<BuildingSpawnGroupData>(),
			ComponentType.ReadOnly<PollutionData>()
		};
		return val;
	}

	protected override void RecordChanges(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).GetComponentData<PollutionData>(entity);
	}

	public override void StoreDefaultData(EntityManager entityManager, ref NativeArray<Entity> entities, PrefabSystem prefabSystem)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		m_OriginalPollutionData = new Dictionary<Entity, PollutionData>(entities.Length);
		for (int i = 0; i < entities.Length; i++)
		{
			Entity val = entities[i];
			PollutionData componentData = ((EntityManager)(ref entityManager)).GetComponentData<PollutionData>(val);
			m_OriginalPollutionData.Add(val, componentData);
		}
		entities.Dispose();
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
			m_AirPollutionMultiplier = m_AirPollutionMultiplier,
			m_NoisePollutionMultiplier = m_NoisePollutionMultiplier,
			m_PollutionType = ((EntityManager)(ref entityManager)).GetComponentTypeHandle<PollutionData>(false)
		}, requestedQuery, deps);
	}

	public override void RestoreDefaultData(EntityManager entityManager, ref NativeArray<Entity> entities, PrefabSystem prefabSystem)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < entities.Length; i++)
		{
			Entity val = entities[i];
			if (m_OriginalPollutionData.TryGetValue(val, out var value))
			{
				((EntityManager)(ref entityManager)).SetComponentData<PollutionData>(val, value);
			}
			else
			{
				m_OriginalPollutionData.Add(val, ((EntityManager)(ref entityManager)).GetComponentData<PollutionData>(val));
			}
		}
	}
}
