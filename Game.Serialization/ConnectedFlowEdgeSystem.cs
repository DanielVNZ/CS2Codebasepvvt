using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Simulation;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine.Scripting;

namespace Game.Serialization;

[CompilerGenerated]
public class ConnectedFlowEdgeSystem : GameSystemBase
{
	[BurstCompile]
	public struct ConnectedFlowEdgeJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<ElectricityFlowEdge> m_ElectricityFlowEdgeType;

		[ReadOnly]
		public ComponentTypeHandle<WaterPipeEdge> m_WaterPipeEdgeType;

		public BufferLookup<ConnectedFlowEdge> m_ConnectedFlowEdges;

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
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<ElectricityFlowEdge> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ElectricityFlowEdge>(ref m_ElectricityFlowEdgeType);
			NativeArray<WaterPipeEdge> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WaterPipeEdge>(ref m_WaterPipeEdgeType);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Entity edge = nativeArray[i];
				ElectricityFlowEdge electricityFlowEdge = nativeArray2[i];
				DynamicBuffer<ConnectedFlowEdge> val = m_ConnectedFlowEdges[electricityFlowEdge.m_Start];
				DynamicBuffer<ConnectedFlowEdge> val2 = m_ConnectedFlowEdges[electricityFlowEdge.m_End];
				CollectionUtils.TryAddUniqueValue<ConnectedFlowEdge>(val, new ConnectedFlowEdge(edge));
				CollectionUtils.TryAddUniqueValue<ConnectedFlowEdge>(val2, new ConnectedFlowEdge(edge));
			}
			for (int j = 0; j < nativeArray3.Length; j++)
			{
				Entity edge2 = nativeArray[j];
				WaterPipeEdge waterPipeEdge = nativeArray3[j];
				DynamicBuffer<ConnectedFlowEdge> val3 = m_ConnectedFlowEdges[waterPipeEdge.m_Start];
				DynamicBuffer<ConnectedFlowEdge> val4 = m_ConnectedFlowEdges[waterPipeEdge.m_End];
				CollectionUtils.TryAddUniqueValue<ConnectedFlowEdge>(val3, new ConnectedFlowEdge(edge2));
				CollectionUtils.TryAddUniqueValue<ConnectedFlowEdge>(val4, new ConnectedFlowEdge(edge2));
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
		public ComponentTypeHandle<ElectricityFlowEdge> __Game_Simulation_ElectricityFlowEdge_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WaterPipeEdge> __Game_Simulation_WaterPipeEdge_RO_ComponentTypeHandle;

		public BufferLookup<ConnectedFlowEdge> __Game_Simulation_ConnectedFlowEdge_RW_BufferLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Simulation_ElectricityFlowEdge_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ElectricityFlowEdge>(true);
			__Game_Simulation_WaterPipeEdge_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WaterPipeEdge>(true);
			__Game_Simulation_ConnectedFlowEdge_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedFlowEdge>(false);
		}
	}

	private EntityQuery m_Query;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<ElectricityFlowEdge>(),
			ComponentType.ReadOnly<WaterPipeEdge>()
		};
		array[0] = val;
		m_Query = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		((ComponentSystemBase)this).RequireForUpdate(m_Query);
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
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		ConnectedFlowEdgeJob connectedFlowEdgeJob = new ConnectedFlowEdgeJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ElectricityFlowEdgeType = InternalCompilerInterface.GetComponentTypeHandle<ElectricityFlowEdge>(ref __TypeHandle.__Game_Simulation_ElectricityFlowEdge_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WaterPipeEdgeType = InternalCompilerInterface.GetComponentTypeHandle<WaterPipeEdge>(ref __TypeHandle.__Game_Simulation_WaterPipeEdge_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedFlowEdges = InternalCompilerInterface.GetBufferLookup<ConnectedFlowEdge>(ref __TypeHandle.__Game_Simulation_ConnectedFlowEdge_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		((SystemBase)this).Dependency = JobChunkExtensions.Schedule<ConnectedFlowEdgeJob>(connectedFlowEdgeJob, m_Query, ((SystemBase)this).Dependency);
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
	public ConnectedFlowEdgeSystem()
	{
	}
}
