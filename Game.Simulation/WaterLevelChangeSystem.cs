using System;
using System.Runtime.CompilerServices;
using Game.Common;
using Game.Events;
using Game.Prefabs;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class WaterLevelChangeSystem : GameSystemBase
{
	[BurstCompile]
	private struct WaterLevelChangeJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		public ComponentTypeHandle<WaterLevelChange> m_WaterLevelChangeType;

		[ReadOnly]
		public ComponentTypeHandle<Duration> m_DurationType;

		[ReadOnly]
		public ComponentLookup<WaterLevelChangeData> m_PrefabWaterLevelChangeData;

		[ReadOnly]
		public uint m_SimulationFrame;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<WaterLevelChange> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WaterLevelChange>(ref m_WaterLevelChangeType);
			NativeArray<Duration> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Duration>(ref m_DurationType);
			for (int i = 0; i < nativeArray3.Length; i++)
			{
				_ = nativeArray[i];
				PrefabRef prefabRef = nativeArray2[i];
				WaterLevelChange waterLevelChange = nativeArray3[i];
				Duration duration = nativeArray4[i];
				WaterLevelChangeData waterLevelChangeData = m_PrefabWaterLevelChangeData[prefabRef.m_Prefab];
				float num = (float)(m_SimulationFrame - duration.m_StartFrame) / 60f - waterLevelChangeData.m_EscalationDelay;
				if (num < 0f)
				{
					continue;
				}
				if (waterLevelChangeData.m_ChangeType == WaterLevelChangeType.Sine)
				{
					float num2 = (float)(duration.m_EndFrame - TsunamiEndDelay - duration.m_StartFrame) / 60f;
					if (num < 0.05f * num2)
					{
						waterLevelChange.m_Intensity = -0.2f * waterLevelChange.m_MaxIntensity * math.sin(20f * num / num2 * (float)Math.PI);
					}
					else if (num < num2)
					{
						waterLevelChange.m_Intensity = waterLevelChange.m_MaxIntensity * (0.5f * math.sin(5f * (num - 0.05f * num2) / (0.95f * num2) * 2f * (float)Math.PI) + 0.5f * math.saturate((num - 0.05f * num2) / (0.2f * num2)));
					}
					else
					{
						waterLevelChange.m_Intensity = 0f;
					}
					waterLevelChange.m_Intensity *= 4f;
				}
				else
				{
					_ = waterLevelChangeData.m_ChangeType;
					_ = 2;
				}
				nativeArray3[i] = waterLevelChange;
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
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		public ComponentTypeHandle<WaterLevelChange> __Game_Events_WaterLevelChange_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Duration> __Game_Events_Duration_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<WaterLevelChangeData> __Game_Prefabs_WaterLevelChangeData_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Events_WaterLevelChange_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WaterLevelChange>(false);
			__Game_Events_Duration_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Duration>(true);
			__Game_Prefabs_WaterLevelChangeData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterLevelChangeData>(true);
		}
	}

	public static readonly int kUpdateInterval = 4;

	private SimulationSystem m_SimulationSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_WaterLevelChangeQuery;

	private TypeHandle __TypeHandle;

	public static int TsunamiEndDelay => Mathf.RoundToInt((float)WaterSystem.kMapSize / WaterSystem.WaveSpeed);

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return kUpdateInterval;
	}

	public static uint GetMinimumDelayAt(WaterLevelChange change, float3 position)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		float2 val = (float)(WaterSystem.kMapSize / 2) * new float2(math.cos(0f - change.m_Direction.x), math.sin(0f - change.m_Direction.y));
		float2 val2 = default(float2);
		((float2)(ref val2))._002Ector(change.m_Direction.y, 0f - change.m_Direction.x);
		float2 val3 = math.dot(val2, ((float3)(ref position)).xz - val) * val2;
		return (uint)Mathf.RoundToInt(math.length(((float3)(ref position)).xz - val - val3) / WaterSystem.WaveSpeed);
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_WaterLevelChangeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<WaterLevelChange>(),
			ComponentType.Exclude<Deleted>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_WaterLevelChangeQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		WaterLevelChangeJob waterLevelChangeJob = new WaterLevelChangeJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WaterLevelChangeType = InternalCompilerInterface.GetComponentTypeHandle<WaterLevelChange>(ref __TypeHandle.__Game_Events_WaterLevelChange_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DurationType = InternalCompilerInterface.GetComponentTypeHandle<Duration>(ref __TypeHandle.__Game_Events_Duration_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabWaterLevelChangeData = InternalCompilerInterface.GetComponentLookup<WaterLevelChangeData>(ref __TypeHandle.__Game_Prefabs_WaterLevelChangeData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SimulationFrame = m_SimulationSystem.frameIndex
		};
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<WaterLevelChangeJob>(waterLevelChangeJob, m_WaterLevelChangeQuery, ((SystemBase)this).Dependency);
		m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
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
	public WaterLevelChangeSystem()
	{
	}
}
