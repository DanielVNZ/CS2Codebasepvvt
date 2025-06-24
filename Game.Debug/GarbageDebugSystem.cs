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
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Debug;

[CompilerGenerated]
public class GarbageDebugSystem : BaseDebugSystem
{
	[BurstCompile]
	private struct GarbageGizmoJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		[ReadOnly]
		public ComponentTypeHandle<GarbageProducer> m_GarbageProducerType;

		[ReadOnly]
		public ComponentLookup<ConsumptionData> m_ConsumptionDatas;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public GarbageParameterData m_GarbageParameterData;

		public bool m_AccumulatedOption;

		public bool m_ProduceOption;

		public GizmoBatcher m_GizmoBatcher;

		private void DrawGarbage(Transform t, int value)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			float3 position = t.m_Position;
			float num = (float)value / 2f;
			position.y += num / 2f;
			int num2 = m_GarbageParameterData.m_HappinessEffectBaseline + m_GarbageParameterData.m_HappinessEffectStep;
			Color val = Color.green;
			if (value > num2)
			{
				val = Color.Lerp(Color.green, Color.red, math.saturate((float)(value - num2) * 1f / (float)m_GarbageParameterData.m_HappinessEffectStep * 9f));
			}
			((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCube(position, new float3(5f, num, 5f), val);
		}

		private void DrawConsume(Transform t, float value)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			float3 position = t.m_Position;
			float num = value / 3f;
			position.y += num / 2f;
			Color val = Color.Lerp(Color.green, Color.red, math.saturate(value / 20000f));
			((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCube(position, new float3(5f, num, 5f), val);
		}

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabType);
			NativeArray<GarbageProducer> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<GarbageProducer>(ref m_GarbageProducerType);
			NativeArray<Transform> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			if (m_AccumulatedOption)
			{
				for (int i = 0; i < nativeArray2.Length; i++)
				{
					DrawGarbage(nativeArray3[i], nativeArray2[i].m_Garbage);
				}
			}
			if (m_ProduceOption)
			{
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Entity prefab = nativeArray[j].m_Prefab;
					DrawConsume(nativeArray3[j], m_ConsumptionDatas[prefab].m_GarbageAccumulation);
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<GarbageProducer> __Game_Buildings_GarbageProducer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<ConsumptionData> __Game_Prefabs_ConsumptionData_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			__Game_Buildings_GarbageProducer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<GarbageProducer>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Prefabs_ConsumptionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ConsumptionData>(true);
		}
	}

	private EntityQuery m_BuildingGroup;

	private GizmosSystem m_GizmosSystem;

	private Option m_AccumulatedOption;

	private Option m_ProduceOption;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_GizmosSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GizmosSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Transform>(),
			ComponentType.ReadOnly<GarbageProducer>()
		};
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Hidden>()
		};
		array[0] = val;
		m_BuildingGroup = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		((ComponentSystemBase)this).Enabled = false;
		m_AccumulatedOption = AddOption("Accumulated Garbage", defaultEnabled: true);
		m_ProduceOption = AddOption("Produce Garbage", defaultEnabled: true);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_BuildingGroup)).IsEmptyIgnoreFilter)
		{
			JobHandle val = default(JobHandle);
			GarbageGizmoJob garbageGizmoJob = new GarbageGizmoJob
			{
				m_GarbageProducerType = InternalCompilerInterface.GetComponentTypeHandle<GarbageProducer>(ref __TypeHandle.__Game_Buildings_GarbageProducer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ConsumptionDatas = InternalCompilerInterface.GetComponentLookup<ConsumptionData>(ref __TypeHandle.__Game_Prefabs_ConsumptionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AccumulatedOption = m_AccumulatedOption.enabled,
				m_ProduceOption = m_ProduceOption.enabled,
				m_GizmoBatcher = m_GizmosSystem.GetGizmosBatcher(ref val)
			};
			EntityQuery entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<GarbageParameterData>() });
			garbageGizmoJob.m_GarbageParameterData = ((EntityQuery)(ref entityQuery)).GetSingleton<GarbageParameterData>();
			GarbageGizmoJob garbageGizmoJob2 = garbageGizmoJob;
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<GarbageGizmoJob>(garbageGizmoJob2, m_BuildingGroup, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val));
			m_GizmosSystem.AddGizmosBatcherWriter(((SystemBase)this).Dependency);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public GarbageDebugSystem()
	{
	}
}
