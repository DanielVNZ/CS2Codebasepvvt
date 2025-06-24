using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Common;
using Game.Pathfind;
using Game.Routes;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class ApplyRoutesSystem : GameSystemBase
{
	[BurstCompile]
	private struct PatchTempReferencesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Waypoint> m_WaypointType;

		[ReadOnly]
		public ComponentLookup<Connected> m_WaypointConnectionData;

		public BufferLookup<ConnectedRoute> m_Routes;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Waypoint> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Waypoint>(ref m_WaypointType);
			if (nativeArray.Length == 0)
			{
				return;
			}
			NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Temp> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray2[i];
				Temp temp = nativeArray3[i];
				if (!(temp.m_Original != Entity.Null))
				{
					continue;
				}
				Connected connected = default(Connected);
				Connected connected2 = default(Connected);
				if (m_WaypointConnectionData.HasComponent(val))
				{
					connected = m_WaypointConnectionData[val];
				}
				if (m_WaypointConnectionData.HasComponent(temp.m_Original))
				{
					connected2 = m_WaypointConnectionData[temp.m_Original];
				}
				if (connected.m_Connected != connected2.m_Connected)
				{
					if (m_Routes.HasBuffer(connected2.m_Connected))
					{
						CollectionUtils.RemoveValue<ConnectedRoute>(m_Routes[connected2.m_Connected], new ConnectedRoute(temp.m_Original));
					}
					if (m_Routes.HasBuffer(connected.m_Connected))
					{
						CollectionUtils.TryAddUniqueValue<ConnectedRoute>(m_Routes[connected.m_Connected], new ConnectedRoute(temp.m_Original));
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct HandleTempEntitiesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Waypoint> m_WaypointType;

		[ReadOnly]
		public ComponentTypeHandle<Segment> m_SegmentType;

		[ReadOnly]
		public ComponentTypeHandle<Position> m_RoutePositionType;

		[ReadOnly]
		public ComponentTypeHandle<Connected> m_RouteConnectedType;

		[ReadOnly]
		public ComponentTypeHandle<PathTargets> m_RoutePathTargetsType;

		[ReadOnly]
		public ComponentTypeHandle<PathInformation> m_PathInformationType;

		[ReadOnly]
		public BufferTypeHandle<RouteWaypoint> m_RouteWaypointType;

		[ReadOnly]
		public BufferTypeHandle<RouteSegment> m_RouteSegmentType;

		[ReadOnly]
		public BufferTypeHandle<PathElement> m_PathElementType;

		[ReadOnly]
		public ComponentLookup<Hidden> m_HiddenData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<Position> m_RoutePositionData;

		[ReadOnly]
		public ComponentLookup<Connected> m_RouteConnectedData;

		[ReadOnly]
		public ComponentLookup<VehicleTiming> m_VehicleTimingData;

		[ReadOnly]
		public ComponentLookup<RouteInfo> m_RouteInfoData;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> m_Waypoints;

		[ReadOnly]
		public BufferLookup<RouteSegment> m_Segments;

		[ReadOnly]
		public EntityArchetype m_PathTargetEventArchetype;

		[ReadOnly]
		public ComponentTypeSet m_AppliedTypes;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			NativeArray<Waypoint> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Waypoint>(ref m_WaypointType);
			if (nativeArray3.Length != 0)
			{
				NativeArray<Position> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Position>(ref m_RoutePositionType);
				NativeArray<Connected> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Connected>(ref m_RouteConnectedType);
				for (int i = 0; i < nativeArray.Length; i++)
				{
					Entity entity = nativeArray[i];
					Temp temp = nativeArray2[i];
					if ((temp.m_Flags & TempFlags.Delete) != 0)
					{
						Delete(unfilteredChunkIndex, entity, temp);
					}
					else if (temp.m_Original != Entity.Null)
					{
						if (nativeArray5.Length != 0)
						{
							Update(unfilteredChunkIndex, entity, temp, nativeArray3[i], nativeArray4[i], nativeArray5[i]);
						}
						else
						{
							Update(unfilteredChunkIndex, entity, temp, nativeArray3[i], nativeArray4[i], default(Connected));
						}
						UpdateComponent<VehicleTiming>(unfilteredChunkIndex, entity, temp.m_Original, m_VehicleTimingData, updateValue: false);
					}
					else
					{
						Create(unfilteredChunkIndex, entity);
					}
				}
				return;
			}
			NativeArray<Segment> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Segment>(ref m_SegmentType);
			if (nativeArray6.Length != 0)
			{
				NativeArray<PathTargets> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathTargets>(ref m_RoutePathTargetsType);
				NativeArray<PathInformation> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathInformation>(ref m_PathInformationType);
				BufferAccessor<PathElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<PathElement>(ref m_PathElementType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Entity entity2 = nativeArray[j];
					Temp temp2 = nativeArray2[j];
					if ((temp2.m_Flags & TempFlags.Delete) != 0)
					{
						Delete(unfilteredChunkIndex, entity2, temp2);
					}
					else if (temp2.m_Original != Entity.Null)
					{
						if (nativeArray7.Length != 0)
						{
							CopyToOriginal<PathTargets>(unfilteredChunkIndex, temp2, nativeArray7[j]);
						}
						if (nativeArray8.Length != 0)
						{
							UpdatePathInfo(unfilteredChunkIndex, temp2, nativeArray8[j]);
						}
						if (bufferAccessor.Length != 0)
						{
							CopyToOriginal<PathElement>(unfilteredChunkIndex, temp2, bufferAccessor[j]);
						}
						Update(unfilteredChunkIndex, entity2, temp2, nativeArray6[j]);
					}
					else
					{
						Create(unfilteredChunkIndex, entity2);
					}
				}
				return;
			}
			BufferAccessor<RouteWaypoint> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<RouteWaypoint>(ref m_RouteWaypointType);
			if (bufferAccessor2.Length != 0)
			{
				BufferAccessor<RouteSegment> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<RouteSegment>(ref m_RouteSegmentType);
				for (int k = 0; k < nativeArray.Length; k++)
				{
					Entity entity3 = nativeArray[k];
					Temp temp3 = nativeArray2[k];
					if ((temp3.m_Flags & TempFlags.Delete) != 0)
					{
						if (temp3.m_Original != Entity.Null)
						{
							Delete(unfilteredChunkIndex, entity3, temp3, bufferAccessor2[k], bufferAccessor3[k]);
						}
						else
						{
							Delete(unfilteredChunkIndex, entity3, temp3);
						}
					}
					else if (temp3.m_Original != Entity.Null)
					{
						Update(unfilteredChunkIndex, entity3, temp3, bufferAccessor2[k], bufferAccessor3[k]);
					}
					else
					{
						Create(unfilteredChunkIndex, entity3);
					}
				}
				return;
			}
			for (int l = 0; l < nativeArray.Length; l++)
			{
				Entity entity4 = nativeArray[l];
				Temp temp4 = nativeArray2[l];
				if ((temp4.m_Flags & TempFlags.Delete) != 0)
				{
					Delete(unfilteredChunkIndex, entity4, temp4);
				}
				else if (temp4.m_Original != Entity.Null)
				{
					Update(unfilteredChunkIndex, entity4, temp4);
				}
				else
				{
					Create(unfilteredChunkIndex, entity4);
				}
			}
		}

		private void Delete(int chunkIndex, Entity entity, Temp temp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (temp.m_Original != Entity.Null)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(chunkIndex, temp.m_Original, default(Deleted));
			}
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(chunkIndex, entity, default(Deleted));
		}

		private void UpdateComponent<T>(int chunkIndex, Entity entity, Entity original, ComponentLookup<T> data, bool updateValue) where T : unmanaged, IComponentData
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			if (data.HasComponent(entity))
			{
				if (data.HasComponent(original))
				{
					if (updateValue)
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<T>(chunkIndex, original, data[entity]);
					}
				}
				else if (updateValue)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<T>(chunkIndex, original, data[entity]);
				}
				else
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<T>(chunkIndex, original, default(T));
				}
			}
			else if (data.HasComponent(original))
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<T>(chunkIndex, original);
			}
		}

		private void Update(int chunkIndex, Entity entity, Temp temp, bool updateOriginal = true)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			if (m_HiddenData.HasComponent(temp.m_Original))
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Hidden>(chunkIndex, temp.m_Original);
			}
			if (updateOriginal)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(chunkIndex, temp.m_Original, default(Updated));
			}
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(chunkIndex, entity, default(Deleted));
		}

		private void Update(int chunkIndex, Entity entity, Temp temp, Waypoint waypoint, Position position, Connected connected)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Waypoint>(chunkIndex, temp.m_Original, waypoint);
			Position position2 = m_RoutePositionData[temp.m_Original];
			if (!((float3)(ref position2.m_Position)).Equals(position.m_Position))
			{
				Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(chunkIndex, m_PathTargetEventArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PathTargetMoved>(chunkIndex, val, new PathTargetMoved(temp.m_Original, position2.m_Position, position.m_Position));
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Position>(chunkIndex, temp.m_Original, position);
			}
			if (connected.m_Connected != Entity.Null)
			{
				if (m_RouteConnectedData.HasComponent(temp.m_Original))
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Connected>(chunkIndex, temp.m_Original, connected);
				}
				else
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Connected>(chunkIndex, temp.m_Original, connected);
				}
			}
			else if (m_RouteConnectedData.HasComponent(temp.m_Original))
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Connected>(chunkIndex, temp.m_Original);
			}
			Update(chunkIndex, entity, temp);
		}

		private void CopyToOriginal<T>(int chunkIndex, Temp temp, T data) where T : unmanaged, IComponentData
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<T>(chunkIndex, temp.m_Original, data);
		}

		private void CopyToOriginal<T>(int chunkIndex, Temp temp, DynamicBuffer<T> data) where T : unmanaged, IBufferElementData
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<T>(chunkIndex, temp.m_Original).CopyFrom(data.AsNativeArray());
		}

		private void UpdatePathInfo(int chunkIndex, Temp temp, PathInformation data)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PathInformation>(chunkIndex, temp.m_Original, data);
			if (m_RouteInfoData.HasComponent(temp.m_Original))
			{
				RouteInfo routeInfo = m_RouteInfoData[temp.m_Original];
				routeInfo.m_Distance = math.max(routeInfo.m_Distance, data.m_Distance);
				routeInfo.m_Duration = math.max(routeInfo.m_Duration, data.m_Duration);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RouteInfo>(chunkIndex, temp.m_Original, routeInfo);
			}
		}

		private void Update(int chunkIndex, Entity entity, Temp temp, Segment segment)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Segment>(chunkIndex, temp.m_Original, segment);
			Update(chunkIndex, entity, temp);
		}

		private void Delete(int chunkIndex, Entity entity, Temp temp, DynamicBuffer<RouteWaypoint> waypoints, DynamicBuffer<RouteSegment> segments)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<RouteWaypoint> val = m_Waypoints[temp.m_Original];
			DynamicBuffer<RouteSegment> val2 = m_Segments[temp.m_Original];
			NativeParallelHashMap<Entity, int> val3 = default(NativeParallelHashMap<Entity, int>);
			val3._002Ector(val.Length, AllocatorHandle.op_Implicit((Allocator)2));
			for (int i = 0; i < waypoints.Length; i++)
			{
				RouteWaypoint routeWaypoint = waypoints[i];
				if (routeWaypoint.m_Waypoint != Entity.Null)
				{
					Temp temp2 = m_TempData[routeWaypoint.m_Waypoint];
					if (temp2.m_Original != Entity.Null)
					{
						val3.TryAdd(temp2.m_Original, 1);
					}
				}
			}
			int num = default(int);
			for (int j = 0; j < val.Length; j++)
			{
				RouteWaypoint routeWaypoint2 = val[j];
				if (routeWaypoint2.m_Waypoint != Entity.Null && !val3.TryGetValue(routeWaypoint2.m_Waypoint, ref num))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(chunkIndex, routeWaypoint2.m_Waypoint, default(Deleted));
				}
			}
			val3.Clear();
			for (int k = 0; k < segments.Length; k++)
			{
				RouteSegment routeSegment = segments[k];
				if (routeSegment.m_Segment != Entity.Null)
				{
					Temp temp3 = m_TempData[routeSegment.m_Segment];
					if (temp3.m_Original != Entity.Null)
					{
						val3.TryAdd(temp3.m_Original, 1);
					}
				}
			}
			int num2 = default(int);
			for (int l = 0; l < val2.Length; l++)
			{
				RouteSegment routeSegment2 = val2[l];
				if (routeSegment2.m_Segment != Entity.Null && !val3.TryGetValue(routeSegment2.m_Segment, ref num2))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(chunkIndex, routeSegment2.m_Segment, default(Deleted));
				}
			}
			val3.Dispose();
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(chunkIndex, temp.m_Original, default(Deleted));
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(chunkIndex, entity, default(Deleted));
		}

		private void Update(int chunkIndex, Entity entity, Temp temp, DynamicBuffer<RouteWaypoint> waypoints, DynamicBuffer<RouteSegment> segments)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<RouteWaypoint> val = m_Waypoints[temp.m_Original];
			DynamicBuffer<RouteSegment> val2 = m_Segments[temp.m_Original];
			DynamicBuffer<RouteWaypoint> val3 = ((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<RouteWaypoint>(chunkIndex, temp.m_Original);
			DynamicBuffer<RouteSegment> val4 = ((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<RouteSegment>(chunkIndex, temp.m_Original);
			val3.ResizeUninitialized(waypoints.Length);
			val4.ResizeUninitialized(segments.Length);
			NativeParallelHashMap<Entity, int> val5 = default(NativeParallelHashMap<Entity, int>);
			val5._002Ector(val.Length, AllocatorHandle.op_Implicit((Allocator)2));
			for (int i = 0; i < waypoints.Length; i++)
			{
				RouteWaypoint routeWaypoint = waypoints[i];
				if (routeWaypoint.m_Waypoint != Entity.Null)
				{
					Temp temp2 = m_TempData[routeWaypoint.m_Waypoint];
					if (temp2.m_Original != Entity.Null)
					{
						val5.TryAdd(temp2.m_Original, 1);
						routeWaypoint.m_Waypoint = temp2.m_Original;
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Owner>(chunkIndex, routeWaypoint.m_Waypoint, new Owner(temp.m_Original));
					}
				}
				val3[i] = routeWaypoint;
			}
			int num = default(int);
			for (int j = 0; j < val.Length; j++)
			{
				RouteWaypoint routeWaypoint2 = val[j];
				if (routeWaypoint2.m_Waypoint != Entity.Null && !val5.TryGetValue(routeWaypoint2.m_Waypoint, ref num))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(chunkIndex, routeWaypoint2.m_Waypoint, default(Deleted));
				}
			}
			val5.Clear();
			for (int k = 0; k < segments.Length; k++)
			{
				RouteSegment routeSegment = segments[k];
				if (routeSegment.m_Segment != Entity.Null)
				{
					Temp temp3 = m_TempData[routeSegment.m_Segment];
					if (temp3.m_Original != Entity.Null)
					{
						val5.TryAdd(temp3.m_Original, 1);
						routeSegment.m_Segment = temp3.m_Original;
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Owner>(chunkIndex, routeSegment.m_Segment, new Owner(temp.m_Original));
					}
				}
				val4[k] = routeSegment;
			}
			int num2 = default(int);
			for (int l = 0; l < val2.Length; l++)
			{
				RouteSegment routeSegment2 = val2[l];
				if (routeSegment2.m_Segment != Entity.Null && !val5.TryGetValue(routeSegment2.m_Segment, ref num2))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(chunkIndex, routeSegment2.m_Segment, default(Deleted));
				}
			}
			val5.Dispose();
			Update(chunkIndex, entity, temp);
		}

		private void Create(int chunkIndex, Entity entity)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Temp>(chunkIndex, entity);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent(chunkIndex, entity, ref m_AppliedTypes);
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
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Waypoint> __Game_Routes_Waypoint_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Connected> __Game_Routes_Connected_RO_ComponentLookup;

		public BufferLookup<ConnectedRoute> __Game_Routes_ConnectedRoute_RW_BufferLookup;

		[ReadOnly]
		public ComponentTypeHandle<Segment> __Game_Routes_Segment_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Position> __Game_Routes_Position_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Connected> __Game_Routes_Connected_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PathTargets> __Game_Routes_PathTargets_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<RouteWaypoint> __Game_Routes_RouteWaypoint_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<RouteSegment> __Game_Routes_RouteSegment_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<PathElement> __Game_Pathfind_PathElement_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Hidden> __Game_Tools_Hidden_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Position> __Game_Routes_Position_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<VehicleTiming> __Game_Routes_VehicleTiming_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RouteInfo> __Game_Routes_RouteInfo_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> __Game_Routes_RouteWaypoint_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<RouteSegment> __Game_Routes_RouteSegment_RO_BufferLookup;

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
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Routes_Waypoint_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Waypoint>(true);
			__Game_Routes_Connected_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Connected>(true);
			__Game_Routes_ConnectedRoute_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedRoute>(false);
			__Game_Routes_Segment_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Segment>(true);
			__Game_Routes_Position_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Position>(true);
			__Game_Routes_Connected_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Connected>(true);
			__Game_Routes_PathTargets_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathTargets>(true);
			__Game_Pathfind_PathInformation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathInformation>(true);
			__Game_Routes_RouteWaypoint_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<RouteWaypoint>(true);
			__Game_Routes_RouteSegment_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<RouteSegment>(true);
			__Game_Pathfind_PathElement_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<PathElement>(true);
			__Game_Tools_Hidden_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Hidden>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Routes_Position_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Position>(true);
			__Game_Routes_VehicleTiming_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<VehicleTiming>(true);
			__Game_Routes_RouteInfo_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RouteInfo>(true);
			__Game_Routes_RouteWaypoint_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteWaypoint>(true);
			__Game_Routes_RouteSegment_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteSegment>(true);
		}
	}

	private ToolOutputBarrier m_ToolOutputBarrier;

	private EntityQuery m_TempQuery;

	private EntityArchetype m_PathTargetEventArchetype;

	private ComponentTypeSet m_AppliedTypes;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolOutputBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolOutputBarrier>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Route>(),
			ComponentType.ReadOnly<Waypoint>(),
			ComponentType.ReadOnly<Segment>()
		};
		array[0] = val;
		m_TempQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_PathTargetEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Event>(),
			ComponentType.ReadWrite<PathTargetMoved>()
		});
		m_AppliedTypes = new ComponentTypeSet(ComponentType.ReadWrite<Applied>(), ComponentType.ReadWrite<Created>(), ComponentType.ReadWrite<Updated>());
		((ComponentSystemBase)this).RequireForUpdate(m_TempQuery);
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
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		PatchTempReferencesJob patchTempReferencesJob = new PatchTempReferencesJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WaypointType = InternalCompilerInterface.GetComponentTypeHandle<Waypoint>(ref __TypeHandle.__Game_Routes_Waypoint_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WaypointConnectionData = InternalCompilerInterface.GetComponentLookup<Connected>(ref __TypeHandle.__Game_Routes_Connected_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Routes = InternalCompilerInterface.GetBufferLookup<ConnectedRoute>(ref __TypeHandle.__Game_Routes_ConnectedRoute_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		HandleTempEntitiesJob handleTempEntitiesJob = new HandleTempEntitiesJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WaypointType = InternalCompilerInterface.GetComponentTypeHandle<Waypoint>(ref __TypeHandle.__Game_Routes_Waypoint_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SegmentType = InternalCompilerInterface.GetComponentTypeHandle<Segment>(ref __TypeHandle.__Game_Routes_Segment_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RoutePositionType = InternalCompilerInterface.GetComponentTypeHandle<Position>(ref __TypeHandle.__Game_Routes_Position_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RouteConnectedType = InternalCompilerInterface.GetComponentTypeHandle<Connected>(ref __TypeHandle.__Game_Routes_Connected_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RoutePathTargetsType = InternalCompilerInterface.GetComponentTypeHandle<PathTargets>(ref __TypeHandle.__Game_Routes_PathTargets_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathInformationType = InternalCompilerInterface.GetComponentTypeHandle<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RouteWaypointType = InternalCompilerInterface.GetBufferTypeHandle<RouteWaypoint>(ref __TypeHandle.__Game_Routes_RouteWaypoint_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RouteSegmentType = InternalCompilerInterface.GetBufferTypeHandle<RouteSegment>(ref __TypeHandle.__Game_Routes_RouteSegment_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathElementType = InternalCompilerInterface.GetBufferTypeHandle<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HiddenData = InternalCompilerInterface.GetComponentLookup<Hidden>(ref __TypeHandle.__Game_Tools_Hidden_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RoutePositionData = InternalCompilerInterface.GetComponentLookup<Position>(ref __TypeHandle.__Game_Routes_Position_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteConnectedData = InternalCompilerInterface.GetComponentLookup<Connected>(ref __TypeHandle.__Game_Routes_Connected_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_VehicleTimingData = InternalCompilerInterface.GetComponentLookup<VehicleTiming>(ref __TypeHandle.__Game_Routes_VehicleTiming_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteInfoData = InternalCompilerInterface.GetComponentLookup<RouteInfo>(ref __TypeHandle.__Game_Routes_RouteInfo_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Waypoints = InternalCompilerInterface.GetBufferLookup<RouteWaypoint>(ref __TypeHandle.__Game_Routes_RouteWaypoint_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Segments = InternalCompilerInterface.GetBufferLookup<RouteSegment>(ref __TypeHandle.__Game_Routes_RouteSegment_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathTargetEventArchetype = m_PathTargetEventArchetype,
			m_AppliedTypes = m_AppliedTypes
		};
		EntityCommandBuffer val = m_ToolOutputBarrier.CreateCommandBuffer();
		handleTempEntitiesJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		HandleTempEntitiesJob handleTempEntitiesJob2 = handleTempEntitiesJob;
		JobHandle val2 = JobChunkExtensions.Schedule<PatchTempReferencesJob>(patchTempReferencesJob, m_TempQuery, ((SystemBase)this).Dependency);
		JobHandle val3 = JobChunkExtensions.ScheduleParallel<HandleTempEntitiesJob>(handleTempEntitiesJob2, m_TempQuery, ((SystemBase)this).Dependency);
		((SystemBase)this).Dependency = JobHandle.CombineDependencies(val2, val3);
		((EntityCommandBufferSystem)m_ToolOutputBarrier).AddJobHandleForProducer(val3);
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
	public ApplyRoutesSystem()
	{
	}
}
