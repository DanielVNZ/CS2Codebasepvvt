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
public class ProcessingCompanyGlobalMode : EntityQueryModePrefab
{
	[BurstCompile]
	private struct ModeJob : IJobChunk
	{
		public float m_InputMultiplier;

		public float m_OutputMultiplier;

		public ComponentTypeHandle<IndustrialProcessData> m_ProcessingType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<IndustrialProcessData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<IndustrialProcessData>(ref m_ProcessingType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				IndustrialProcessData industrialProcessData = nativeArray[i];
				industrialProcessData.m_Input1.m_Amount = (int)((float)industrialProcessData.m_Input1.m_Amount * m_InputMultiplier);
				industrialProcessData.m_Input2.m_Amount = (int)((float)industrialProcessData.m_Input2.m_Amount * m_InputMultiplier);
				industrialProcessData.m_Output.m_Amount = (int)((float)industrialProcessData.m_Output.m_Amount * m_OutputMultiplier);
				nativeArray[i] = industrialProcessData;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[Header("Modify the all input requirements and output production of processing companies.")]
	public float m_InputMultiplier;

	public float m_OutputMultiplier;

	private Dictionary<Entity, IndustrialProcessData> m_OriginalData;

	public override EntityQueryDesc GetEntityQueryDesc()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<IndustrialProcessData>() };
		return val;
	}

	protected override void RecordChanges(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).GetComponentData<IndustrialProcessData>(entity);
	}

	public override void StoreDefaultData(EntityManager entityManager, ref NativeArray<Entity> entities, PrefabSystem prefabSystem)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		m_OriginalData = new Dictionary<Entity, IndustrialProcessData>(entities.Length);
		for (int i = 0; i < entities.Length; i++)
		{
			Entity val = entities[i];
			IndustrialProcessData componentData = ((EntityManager)(ref entityManager)).GetComponentData<IndustrialProcessData>(val);
			m_OriginalData.Add(val, componentData);
		}
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
			m_InputMultiplier = m_InputMultiplier,
			m_OutputMultiplier = m_OutputMultiplier,
			m_ProcessingType = ((EntityManager)(ref entityManager)).GetComponentTypeHandle<IndustrialProcessData>(false)
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
			if (m_OriginalData.TryGetValue(val, out var value))
			{
				((EntityManager)(ref entityManager)).SetComponentData<IndustrialProcessData>(val, value);
			}
			else
			{
				m_OriginalData.Add(val, ((EntityManager)(ref entityManager)).GetComponentData<IndustrialProcessData>(val));
			}
		}
	}
}
