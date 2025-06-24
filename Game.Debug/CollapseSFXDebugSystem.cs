using System.Runtime.CompilerServices;
using Colossal;
using Game.Buildings;
using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Debug;

[CompilerGenerated]
public class CollapseSFXDebugSystem : BaseDebugSystem
{
	[BurstCompile]
	private struct CollapseSfxCoverageGizmoJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PreFabRefType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public BufferLookup<Effect> m_EffectsBuffs;

		[ReadOnly]
		public BuildingConfigurationData m_BuildingConfigurationData;

		public GizmoBatcher m_GizmoBatcher;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PreFabRefType);
			NativeArray<Transform> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			NativeArray<Entity> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			DynamicBuffer<Effect> val = default(DynamicBuffer<Effect>);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity prefab = nativeArray[i].m_Prefab;
				_ = nativeArray3[i];
				Transform transform = nativeArray2[i];
				if (!m_EffectsBuffs.TryGetBuffer(prefab, ref val))
				{
					continue;
				}
				for (int j = 0; j < val.Length; j++)
				{
					Effect effect = val[j];
					if (effect.m_Effect == m_BuildingConfigurationData.m_CollapseSFX)
					{
						float3 val2 = ObjectUtils.LocalToWorld(transform, effect.m_Position);
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val2, val2 + new float3(0f, 200f, 0f), Color.yellow);
					}
					else if (effect.m_Effect == m_BuildingConfigurationData.m_FireLoopSFX || effect.m_Effect == m_BuildingConfigurationData.m_FireSpotSFX)
					{
						float3 val3 = ObjectUtils.LocalToWorld(transform, effect.m_Position);
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val3, val3 + new float3(0f, 200f, 0f), Color.red);
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private EntityQuery m_BuildingEffectGroup;

	private EntityQuery m_ConfigurationQuery;

	private GizmosSystem m_GizmosSystem;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_GizmosSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GizmosSystem>();
		m_BuildingEffectGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Transform>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_ConfigurationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<BuildingConfigurationData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_BuildingEffectGroup);
		((ComponentSystemBase)this).Enabled = false;
	}

	[Preserve]
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val2 = default(JobHandle);
		JobHandle val = JobChunkExtensions.ScheduleParallel<CollapseSfxCoverageGizmoJob>(new CollapseSfxCoverageGizmoJob
		{
			m_EntityType = ((ComponentSystemBase)this).GetEntityTypeHandle(),
			m_PreFabRefType = ((ComponentSystemBase)this).GetComponentTypeHandle<PrefabRef>(true),
			m_TransformType = ((ComponentSystemBase)this).GetComponentTypeHandle<Transform>(true),
			m_EffectsBuffs = ((SystemBase)this).GetBufferLookup<Effect>(true),
			m_BuildingConfigurationData = ((EntityQuery)(ref m_ConfigurationQuery)).GetSingleton<BuildingConfigurationData>(),
			m_GizmoBatcher = m_GizmosSystem.GetGizmosBatcher(ref val2)
		}, m_BuildingEffectGroup, JobHandle.CombineDependencies(inputDeps, val2));
		m_GizmosSystem.AddGizmosBatcherWriter(val);
		return JobHandle.CombineDependencies(inputDeps, val);
	}

	[Preserve]
	public CollapseSFXDebugSystem()
	{
	}
}
