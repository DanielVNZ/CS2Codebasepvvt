using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Game.Common;
using Game.Prefabs;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Net;

[CompilerGenerated]
public class FlipTrafficHandednessSystem : GameSystemBase
{
	[BurstCompile]
	private struct FlipOnewayRoadsJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabGeometryData;

		public ComponentTypeHandle<Edge> m_EdgeType;

		public ComponentTypeHandle<Curve> m_CurveType;

		public ComponentTypeHandle<Elevation> m_ElevationType;

		public ComponentTypeHandle<Upgraded> m_UpgradedType;

		public ComponentTypeHandle<BuildOrder> m_BuildOrderType;

		public BufferTypeHandle<ConnectedNode> m_ConnectedNodeType;

		public BufferTypeHandle<ServiceCoverage> m_ServiceCoverageType;

		public BufferTypeHandle<ResourceAvailability> m_ResourceAvailabilityType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<Edge> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Edge>(ref m_EdgeType);
			NativeArray<Curve> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Curve>(ref m_CurveType);
			NativeArray<Elevation> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Elevation>(ref m_ElevationType);
			NativeArray<Upgraded> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Upgraded>(ref m_UpgradedType);
			NativeArray<BuildOrder> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<BuildOrder>(ref m_BuildOrderType);
			BufferAccessor<ConnectedNode> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ConnectedNode>(ref m_ConnectedNodeType);
			BufferAccessor<ServiceCoverage> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ServiceCoverage>(ref m_ServiceCoverageType);
			BufferAccessor<ResourceAvailability> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ResourceAvailability>(ref m_ResourceAvailabilityType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				PrefabRef prefabRef = nativeArray[i];
				if (!m_PrefabGeometryData.HasComponent(prefabRef.m_Prefab))
				{
					continue;
				}
				if ((m_PrefabGeometryData[prefabRef.m_Prefab].m_Flags & GeometryFlags.FlipTrafficHandedness) == 0)
				{
					if (nativeArray5.Length != 0)
					{
						Upgraded upgraded = nativeArray5[i];
						NetUtils.FlipUpgradeTrafficHandedness(ref upgraded.m_Flags);
						nativeArray5[i] = upgraded;
					}
					continue;
				}
				Edge edge = nativeArray2[i];
				CommonUtils.Swap(ref edge.m_Start, ref edge.m_End);
				nativeArray2[i] = edge;
				Curve curve = nativeArray3[i];
				curve.m_Bezier = MathUtils.Invert(curve.m_Bezier);
				nativeArray3[i] = curve;
				if (nativeArray4.Length != 0)
				{
					Elevation elevation = nativeArray4[i];
					elevation.m_Elevation = ((float2)(ref elevation.m_Elevation)).yx;
					nativeArray4[i] = elevation;
				}
				if (nativeArray6.Length != 0)
				{
					BuildOrder buildOrder = nativeArray6[i];
					CommonUtils.Swap(ref buildOrder.m_Start, ref buildOrder.m_End);
					nativeArray6[i] = buildOrder;
				}
				if (bufferAccessor.Length != 0)
				{
					DynamicBuffer<ConnectedNode> val = bufferAccessor[i];
					for (int j = 0; j < val.Length; j++)
					{
						ConnectedNode connectedNode = val[j];
						connectedNode.m_CurvePosition = math.saturate(1f - connectedNode.m_CurvePosition);
						val[j] = connectedNode;
					}
				}
				if (bufferAccessor2.Length != 0)
				{
					DynamicBuffer<ServiceCoverage> val2 = bufferAccessor2[i];
					for (int k = 0; k < val2.Length; k++)
					{
						ServiceCoverage serviceCoverage = val2[k];
						serviceCoverage.m_Coverage = ((float2)(ref serviceCoverage.m_Coverage)).yx;
						val2[k] = serviceCoverage;
					}
				}
				if (bufferAccessor3.Length != 0)
				{
					DynamicBuffer<ResourceAvailability> val3 = bufferAccessor3[i];
					for (int l = 0; l < val3.Length; l++)
					{
						ResourceAvailability resourceAvailability = val3[l];
						resourceAvailability.m_Availability = ((float2)(ref resourceAvailability.m_Availability)).yx;
						val3[l] = resourceAvailability;
					}
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
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

		public ComponentTypeHandle<Edge> __Game_Net_Edge_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Curve> __Game_Net_Curve_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Elevation> __Game_Net_Elevation_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Upgraded> __Game_Net_Upgraded_RW_ComponentTypeHandle;

		public ComponentTypeHandle<BuildOrder> __Game_Net_BuildOrder_RW_ComponentTypeHandle;

		public BufferTypeHandle<ConnectedNode> __Game_Net_ConnectedNode_RW_BufferTypeHandle;

		public BufferTypeHandle<ServiceCoverage> __Game_Net_ServiceCoverage_RW_BufferTypeHandle;

		public BufferTypeHandle<ResourceAvailability> __Game_Net_ResourceAvailability_RW_BufferTypeHandle;

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
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
			__Game_Net_Edge_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Edge>(false);
			__Game_Net_Curve_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Curve>(false);
			__Game_Net_Elevation_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Elevation>(false);
			__Game_Net_Upgraded_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Upgraded>(false);
			__Game_Net_BuildOrder_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BuildOrder>(false);
			__Game_Net_ConnectedNode_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ConnectedNode>(false);
			__Game_Net_ServiceCoverage_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ServiceCoverage>(false);
			__Game_Net_ResourceAvailability_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ResourceAvailability>(false);
		}
	}

	private EntityQuery m_RoadEdgeQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_RoadEdgeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Road>(),
			ComponentType.ReadOnly<Edge>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_RoadEdgeQuery);
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
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		FlipOnewayRoadsJob flipOnewayRoadsJob = new FlipOnewayRoadsJob
		{
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeType = InternalCompilerInterface.GetComponentTypeHandle<Edge>(ref __TypeHandle.__Game_Net_Edge_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurveType = InternalCompilerInterface.GetComponentTypeHandle<Curve>(ref __TypeHandle.__Game_Net_Curve_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ElevationType = InternalCompilerInterface.GetComponentTypeHandle<Elevation>(ref __TypeHandle.__Game_Net_Elevation_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpgradedType = InternalCompilerInterface.GetComponentTypeHandle<Upgraded>(ref __TypeHandle.__Game_Net_Upgraded_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildOrderType = InternalCompilerInterface.GetComponentTypeHandle<BuildOrder>(ref __TypeHandle.__Game_Net_BuildOrder_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedNodeType = InternalCompilerInterface.GetBufferTypeHandle<ConnectedNode>(ref __TypeHandle.__Game_Net_ConnectedNode_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceCoverageType = InternalCompilerInterface.GetBufferTypeHandle<ServiceCoverage>(ref __TypeHandle.__Game_Net_ServiceCoverage_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceAvailabilityType = InternalCompilerInterface.GetBufferTypeHandle<ResourceAvailability>(ref __TypeHandle.__Game_Net_ResourceAvailability_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef)
		};
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<FlipOnewayRoadsJob>(flipOnewayRoadsJob, m_RoadEdgeQuery, ((SystemBase)this).Dependency);
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
	public FlipTrafficHandednessSystem()
	{
	}
}
