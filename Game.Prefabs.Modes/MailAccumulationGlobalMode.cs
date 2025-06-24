using System;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Global/", new Type[] { })]
public class MailAccumulationGlobalMode : EntityQueryModePrefab
{
	[BurstCompile]
	private struct ModeJob : IJobChunk
	{
		public float m_SendingRateMultiplier;

		public float m_ReceivingRateMultiplier;

		public ComponentTypeHandle<MailAccumulationData> m_MailAccumulateType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<MailAccumulationData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<MailAccumulationData>(ref m_MailAccumulateType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				MailAccumulationData mailAccumulationData = nativeArray[i];
				mailAccumulationData.m_AccumulationRate = new float2(mailAccumulationData.m_AccumulationRate.x * m_SendingRateMultiplier, mailAccumulationData.m_AccumulationRate.y * m_ReceivingRateMultiplier);
				nativeArray[i] = mailAccumulationData;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	public float m_SendingRateMultiplier;

	public float m_ReceivingRateMultiplier;

	public override EntityQueryDesc GetEntityQueryDesc()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<MailAccumulationData>() };
		return val;
	}

	protected override void RecordChanges(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).GetComponentData<MailAccumulationData>(entity);
	}

	public override JobHandle ApplyModeData(EntityManager entityManager, EntityQuery requestedQuery, JobHandle deps)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		return JobChunkExtensions.ScheduleParallel<ModeJob>(new ModeJob
		{
			m_SendingRateMultiplier = m_SendingRateMultiplier,
			m_ReceivingRateMultiplier = m_ReceivingRateMultiplier,
			m_MailAccumulateType = ((EntityManager)(ref entityManager)).GetComponentTypeHandle<MailAccumulationData>(false)
		}, requestedQuery, deps);
	}

	public unsafe override void RestoreDefaultData(EntityManager entityManager, ref NativeArray<Entity> entities, PrefabSystem prefabSystem)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < entities.Length; i++)
		{
			Entity val = entities[i];
			if (!prefabSystem.TryGetPrefab<PrefabBase>(val, out var prefabBase) || !prefabBase.TryGetExactly<MailAccumulation>(out var component))
			{
				ComponentBase.baseLog.Warn((object)$"Prefab data not found {this} : {((object)(*(Entity*)(&val))/*cast due to .constrained prefix*/).ToString()} : {prefabBase}");
				continue;
			}
			MailAccumulationData componentData = ((EntityManager)(ref entityManager)).GetComponentData<MailAccumulationData>(val);
			componentData.m_AccumulationRate = new float2(component.m_SendingRate, component.m_ReceivingRate);
			((EntityManager)(ref entityManager)).SetComponentData<MailAccumulationData>(val, componentData);
		}
	}
}
