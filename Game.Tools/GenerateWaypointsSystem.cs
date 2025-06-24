using System;
using System.Runtime.CompilerServices;
using Game.Common;
using Game.Pathfind;
using Game.Prefabs;
using Game.Routes;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class GenerateWaypointsSystem : GameSystemBase
{
	[BurstCompile]
	private struct CreateWaypointsJob : IJob
	{
		private struct SegmentKey : IEquatable<SegmentKey>
		{
			private Entity m_Prefab;

			private Entity m_OriginalRoute;

			private float4 m_Position;

			public SegmentKey(Entity prefab, Entity originalRoute, float4 position)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				m_Prefab = prefab;
				m_OriginalRoute = originalRoute;
				m_Position = position;
			}

			public bool Equals(SegmentKey other)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				if (((Entity)(ref m_Prefab)).Equals(other.m_Prefab) && ((Entity)(ref m_OriginalRoute)).Equals(other.m_OriginalRoute))
				{
					return ((float4)(ref m_Position)).Equals(other.m_Position);
				}
				return false;
			}

			public override int GetHashCode()
			{
				return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Prefab)/*cast due to .constrained prefix*/).GetHashCode() ^ ((object)System.Runtime.CompilerServices.Unsafe.As<float4, float4>(ref m_Position)/*cast due to .constrained prefix*/).GetHashCode();
			}
		}

		private struct SegmentValue
		{
			public Entity m_Segment;

			public float4 m_StartPosition;

			public float4 m_EndPosition;

			public SegmentValue(Entity segment, float4 startPosition, float4 endPosition)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				m_Segment = segment;
				m_StartPosition = startPosition;
				m_EndPosition = endPosition;
			}
		}

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_DefinitionChunks;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_DeletedChunks;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<CreationDefinition> m_CreationDefinitionType;

		[ReadOnly]
		public ComponentTypeHandle<Segment> m_SegmentType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<WaypointDefinition> m_WaypointDefinitionType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<RouteData> m_RouteData;

		[ReadOnly]
		public ComponentLookup<Position> m_PositionData;

		[ReadOnly]
		public ComponentLookup<Segment> m_SegmentData;

		[ReadOnly]
		public ComponentLookup<PathTargets> m_PathTargetsData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<PathInformation> m_PathInformationData;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> m_RouteWaypoints;

		[ReadOnly]
		public BufferLookup<RouteSegment> m_RouteSegments;

		[ReadOnly]
		public BufferLookup<PathElement> m_PathElementData;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			NativeParallelHashMap<SegmentKey, SegmentValue> oldSegments = default(NativeParallelHashMap<SegmentKey, SegmentValue>);
			oldSegments._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
			for (int i = 0; i < m_DeletedChunks.Length; i++)
			{
				FillOldSegments(m_DeletedChunks[i], oldSegments);
			}
			for (int j = 0; j < m_DefinitionChunks.Length; j++)
			{
				CreateWaypointsAndSegments(m_DefinitionChunks[j], oldSegments);
			}
			oldSegments.Dispose();
		}

		private void FillOldSegments(ArchetypeChunk chunk, NativeParallelHashMap<SegmentKey, SegmentValue> oldSegments)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Segment> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Segment>(ref m_SegmentType);
			NativeArray<Owner> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			float4 val2 = default(float4);
			float4 val3 = default(float4);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Entity segment = nativeArray[i];
				Segment segment2 = nativeArray2[i];
				Entity owner = nativeArray3[i].m_Owner;
				Entity prefab = nativeArray4[i].m_Prefab;
				if (!m_RouteWaypoints.HasBuffer(owner) || !m_TempData.HasComponent(owner))
				{
					continue;
				}
				DynamicBuffer<RouteWaypoint> val = m_RouteWaypoints[owner];
				Temp temp = m_TempData[owner];
				if (val.Length > segment2.m_Index)
				{
					Entity waypoint = val[segment2.m_Index].m_Waypoint;
					Entity waypoint2 = val[math.select(segment2.m_Index + 1, 0, segment2.m_Index + 1 == val.Length)].m_Waypoint;
					if (m_PositionData.HasComponent(waypoint) && m_PositionData.HasComponent(waypoint2))
					{
						((float4)(ref val2))._002Ector(m_PositionData[waypoint].m_Position, 0f);
						((float4)(ref val3))._002Ector(m_PositionData[waypoint2].m_Position, 1f);
						oldSegments.TryAdd(new SegmentKey(prefab, temp.m_Original, val2), new SegmentValue(segment, val2, val3));
						oldSegments.TryAdd(new SegmentKey(prefab, temp.m_Original, val3), new SegmentValue(segment, val2, val3));
					}
				}
			}
		}

		private void FillOriginalSegments(Entity route, NativeParallelHashMap<SegmentKey, SegmentValue> originalSegments)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<RouteWaypoint> val = m_RouteWaypoints[route];
			DynamicBuffer<RouteSegment> val2 = m_RouteSegments[route];
			Entity prefab = m_PrefabRefData[route].m_Prefab;
			float4 val3 = default(float4);
			float4 val4 = default(float4);
			for (int i = 0; i < val2.Length; i++)
			{
				Entity segment = val2[i].m_Segment;
				Segment segment2 = m_SegmentData[segment];
				if (val.Length > segment2.m_Index)
				{
					Entity waypoint = val[segment2.m_Index].m_Waypoint;
					Entity waypoint2 = val[math.select(segment2.m_Index + 1, 0, segment2.m_Index + 1 == val.Length)].m_Waypoint;
					if (m_PositionData.HasComponent(waypoint) && m_PositionData.HasComponent(waypoint2))
					{
						((float4)(ref val3))._002Ector(m_PositionData[waypoint].m_Position, 0f);
						((float4)(ref val4))._002Ector(m_PositionData[waypoint2].m_Position, 1f);
						originalSegments.TryAdd(new SegmentKey(prefab, Entity.Null, val3), new SegmentValue(segment, val3, val4));
						originalSegments.TryAdd(new SegmentKey(prefab, Entity.Null, val4), new SegmentValue(segment, val3, val4));
					}
				}
			}
		}

		private void CreateWaypointsAndSegments(ArchetypeChunk chunk, NativeParallelHashMap<SegmentKey, SegmentValue> oldSegments)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<CreationDefinition> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CreationDefinition>(ref m_CreationDefinitionType);
			BufferAccessor<WaypointDefinition> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<WaypointDefinition>(ref m_WaypointDefinitionType);
			NativeParallelHashMap<SegmentKey, SegmentValue> originalSegments = default(NativeParallelHashMap<SegmentKey, SegmentValue>);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				CreationDefinition creationDefinition = nativeArray[i];
				DynamicBuffer<WaypointDefinition> val = bufferAccessor[i];
				Entity prefab = creationDefinition.m_Prefab;
				if (creationDefinition.m_Original != Entity.Null)
				{
					originalSegments._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
					FillOriginalSegments(creationDefinition.m_Original, originalSegments);
					prefab = m_PrefabRefData[creationDefinition.m_Original].m_Prefab;
					RouteData prefabRouteData = m_RouteData[prefab];
					TempFlags tempFlags = (TempFlags)0u;
					if ((creationDefinition.m_Flags & CreationFlags.Delete) != 0)
					{
						tempFlags |= TempFlags.Delete;
					}
					for (int j = 0; j < val.Length; j++)
					{
						CreateWaypoint(prefabRouteData, prefab, tempFlags, val[j], j);
					}
					if (val.Length >= 2)
					{
						for (int k = 0; k < val.Length; k++)
						{
							WaypointDefinition startDefinition = val[k];
							WaypointDefinition endDefinition = val[math.select(k + 1, 0, k + 1 == val.Length)];
							Entity originalSegment = GetOriginalSegment(originalSegments, prefab, startDefinition, endDefinition);
							CreateSegment(oldSegments, prefabRouteData, prefab, originalSegment, creationDefinition.m_Original, tempFlags, startDefinition, endDefinition, k);
						}
					}
					originalSegments.Dispose();
					continue;
				}
				RouteData prefabRouteData2 = m_RouteData[prefab];
				int num = val.Length;
				bool flag = false;
				if (num >= 3)
				{
					WaypointDefinition waypointDefinition = val[0];
					if (((float3)(ref waypointDefinition.m_Position)).Equals(val[num - 1].m_Position))
					{
						num--;
						flag = true;
					}
				}
				for (int l = 0; l < num; l++)
				{
					CreateWaypoint(prefabRouteData2, prefab, TempFlags.Create, val[l], l);
				}
				for (int m = 1; m < num; m++)
				{
					WaypointDefinition startDefinition2 = val[m - 1];
					WaypointDefinition endDefinition2 = val[m];
					CreateSegment(oldSegments, prefabRouteData2, prefab, Entity.Null, Entity.Null, TempFlags.Create, startDefinition2, endDefinition2, m - 1);
				}
				if (flag)
				{
					WaypointDefinition startDefinition3 = val[num - 1];
					WaypointDefinition endDefinition3 = val[0];
					CreateSegment(oldSegments, prefabRouteData2, prefab, Entity.Null, Entity.Null, TempFlags.Create, startDefinition3, endDefinition3, num - 1);
				}
			}
		}

		private Entity GetOriginalSegment(NativeParallelHashMap<SegmentKey, SegmentValue> originalSegments, Entity prefab, WaypointDefinition startDefinition, WaypointDefinition endDefinition)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			SegmentKey segmentKey = new SegmentKey(prefab, Entity.Null, new float4(startDefinition.m_Position, 0f));
			SegmentKey segmentKey2 = new SegmentKey(prefab, Entity.Null, new float4(endDefinition.m_Position, 1f));
			SegmentValue segmentValue = default(SegmentValue);
			if (originalSegments.TryGetValue(segmentKey, ref segmentValue))
			{
				originalSegments.Remove(new SegmentKey(prefab, Entity.Null, segmentValue.m_StartPosition));
				originalSegments.Remove(new SegmentKey(prefab, Entity.Null, segmentValue.m_EndPosition));
				return segmentValue.m_Segment;
			}
			if (originalSegments.TryGetValue(segmentKey2, ref segmentValue))
			{
				originalSegments.Remove(new SegmentKey(prefab, Entity.Null, segmentValue.m_StartPosition));
				originalSegments.Remove(new SegmentKey(prefab, Entity.Null, segmentValue.m_EndPosition));
				return segmentValue.m_Segment;
			}
			return Entity.Null;
		}

		private void CreateWaypoint(RouteData prefabRouteData, Entity prefab, TempFlags tempFlags, WaypointDefinition definition, int index)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			Entity val;
			if (definition.m_Connection != Entity.Null)
			{
				val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(prefabRouteData.m_ConnectedArchetype);
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Connected>(val, new Connected(definition.m_Connection));
			}
			else
			{
				val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(prefabRouteData.m_WaypointArchetype);
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Waypoint>(val, new Waypoint(index));
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Position>(val, new Position(definition.m_Position));
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PrefabRef>(val, new PrefabRef(prefab));
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Temp>(val, new Temp(definition.m_Original, tempFlags));
		}

		private void CreateSegment(NativeParallelHashMap<SegmentKey, SegmentValue> oldSegments, RouteData prefabRouteData, Entity prefab, Entity original, Entity originalRoute, TempFlags tempFlags, WaypointDefinition startDefinition, WaypointDefinition endDefinition, int index)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0412: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_043f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0421: Unknown result type (might be due to invalid IL or missing references)
			//IL_0430: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_046c: Unknown result type (might be due to invalid IL or missing references)
			//IL_044e: Unknown result type (might be due to invalid IL or missing references)
			//IL_045d: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_047b: Unknown result type (might be due to invalid IL or missing references)
			//IL_047d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0482: Unknown result type (might be due to invalid IL or missing references)
			//IL_048a: Unknown result type (might be due to invalid IL or missing references)
			//IL_048c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0491: Unknown result type (might be due to invalid IL or missing references)
			//IL_0497: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			SegmentKey segmentKey = new SegmentKey(prefab, originalRoute, new float4(startDefinition.m_Position, 0f));
			SegmentKey segmentKey2 = new SegmentKey(prefab, originalRoute, new float4(endDefinition.m_Position, 1f));
			bool2 val = default(bool2);
			SegmentValue segmentValue = default(SegmentValue);
			val.x = oldSegments.TryGetValue(segmentKey, ref segmentValue);
			SegmentValue segmentValue2 = default(SegmentValue);
			val.y = oldSegments.TryGetValue(segmentKey2, ref segmentValue2);
			if (math.all(val) && segmentValue.m_Segment != segmentValue2.m_Segment)
			{
				oldSegments.Remove(new SegmentKey(prefab, originalRoute, segmentValue.m_StartPosition));
				oldSegments.Remove(new SegmentKey(prefab, originalRoute, segmentValue.m_EndPosition));
				oldSegments.Remove(new SegmentKey(prefab, originalRoute, segmentValue2.m_StartPosition));
				oldSegments.Remove(new SegmentKey(prefab, originalRoute, segmentValue2.m_EndPosition));
				((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Deleted>(segmentValue.m_Segment);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(segmentValue.m_Segment, default(Updated));
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Segment>(segmentValue.m_Segment, new Segment(index));
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Temp>(segmentValue.m_Segment, new Temp(original, tempFlags));
				if (m_PathInformationData.HasComponent(segmentValue.m_Segment) && m_PathInformationData.HasComponent(segmentValue2.m_Segment))
				{
					PathInformation pathInformation = m_PathInformationData[segmentValue.m_Segment];
					PathInformation pathInformation2 = m_PathInformationData[segmentValue2.m_Segment];
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PathInformation>(segmentValue.m_Segment, PathUtils.CombinePaths(pathInformation, pathInformation2));
				}
				bool2 val2 = bool2.op_Implicit(false);
				if (m_PathElementData.HasBuffer(segmentValue.m_Segment) && m_PathElementData.HasBuffer(segmentValue2.m_Segment))
				{
					DynamicBuffer<PathElement> sourceElements = m_PathElementData[segmentValue.m_Segment];
					DynamicBuffer<PathElement> sourceElements2 = m_PathElementData[segmentValue2.m_Segment];
					DynamicBuffer<PathElement> targetElements = ((EntityCommandBuffer)(ref m_CommandBuffer)).SetBuffer<PathElement>(segmentValue.m_Segment);
					PathUtils.CombinePaths(sourceElements, sourceElements2, targetElements);
					((bool2)(ref val2))._002Ector(sourceElements.Length == 0, sourceElements2.Length == 0);
				}
				if (m_PathTargetsData.HasComponent(segmentValue.m_Segment) && m_PathTargetsData.HasComponent(segmentValue2.m_Segment))
				{
					PathTargets pathTargets = m_PathTargetsData[segmentValue.m_Segment];
					PathTargets pathTargets2 = m_PathTargetsData[segmentValue2.m_Segment];
					pathTargets.m_StartLane = Entity.Null;
					pathTargets.m_EndLane = Entity.Null;
					pathTargets.m_CurvePositions = float2.op_Implicit(0f);
					if (math.all(val2))
					{
						pathTargets.m_ReadyEndPosition = pathTargets.m_ReadyStartPosition;
					}
					else if (val2.x)
					{
						pathTargets.m_ReadyStartPosition = pathTargets2.m_ReadyStartPosition;
						pathTargets.m_ReadyEndPosition = pathTargets2.m_ReadyEndPosition;
					}
					else if (!val2.y)
					{
						pathTargets.m_ReadyEndPosition = pathTargets2.m_ReadyEndPosition;
					}
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PathTargets>(segmentValue.m_Segment, pathTargets);
				}
			}
			else if (math.any(val))
			{
				SegmentValue segmentValue3 = (val.x ? segmentValue : segmentValue2);
				oldSegments.Remove(new SegmentKey(prefab, originalRoute, segmentValue3.m_StartPosition));
				oldSegments.Remove(new SegmentKey(prefab, originalRoute, segmentValue3.m_EndPosition));
				((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Deleted>(segmentValue3.m_Segment);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(segmentValue3.m_Segment, default(Updated));
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Segment>(segmentValue3.m_Segment, new Segment(index));
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Temp>(segmentValue3.m_Segment, new Temp(original, tempFlags));
			}
			else
			{
				Entity val3 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(prefabRouteData.m_SegmentArchetype);
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Segment>(val3, new Segment(index));
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PrefabRef>(val3, new PrefabRef(prefab));
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Temp>(val3, new Temp(original, tempFlags));
				if (m_PathTargetsData.HasComponent(original))
				{
					PathTargets pathTargets3 = m_PathTargetsData[original];
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PathTargets>(val3, pathTargets3);
				}
				if (m_PathInformationData.HasComponent(original))
				{
					PathInformation pathInformation3 = m_PathInformationData[original];
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PathInformation>(val3, pathInformation3);
				}
				if (m_PathElementData.HasBuffer(original))
				{
					DynamicBuffer<PathElement> val4 = m_PathElementData[original];
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetBuffer<PathElement>(val3).CopyFrom(val4.AsNativeArray());
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CreationDefinition> __Game_Tools_CreationDefinition_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Segment> __Game_Routes_Segment_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<WaypointDefinition> __Game_Routes_WaypointDefinition_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RouteData> __Game_Prefabs_RouteData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Position> __Game_Routes_Position_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Segment> __Game_Routes_Segment_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathTargets> __Game_Routes_PathTargets_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> __Game_Routes_RouteWaypoint_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<RouteSegment> __Game_Routes_RouteSegment_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<PathElement> __Game_Pathfind_PathElement_RO_BufferLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Tools_CreationDefinition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CreationDefinition>(true);
			__Game_Routes_Segment_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Segment>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Routes_WaypointDefinition_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<WaypointDefinition>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_RouteData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RouteData>(true);
			__Game_Routes_Position_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Position>(true);
			__Game_Routes_Segment_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Segment>(true);
			__Game_Routes_PathTargets_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathTargets>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Pathfind_PathInformation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathInformation>(true);
			__Game_Routes_RouteWaypoint_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteWaypoint>(true);
			__Game_Routes_RouteSegment_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteSegment>(true);
			__Game_Pathfind_PathElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PathElement>(true);
		}
	}

	private ModificationBarrier1 m_ModificationBarrier;

	private EntityQuery m_DefinitionQuery;

	private EntityQuery m_DeletedQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier1>();
		m_DefinitionQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<CreationDefinition>(),
			ComponentType.ReadOnly<WaypointDefinition>(),
			ComponentType.ReadOnly<Updated>()
		});
		m_DeletedQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Segment>(),
			ComponentType.ReadOnly<Owner>(),
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<PrefabRef>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_DefinitionQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		NativeList<ArchetypeChunk> definitionChunks = ((EntityQuery)(ref m_DefinitionQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		JobHandle val2 = default(JobHandle);
		NativeList<ArchetypeChunk> deletedChunks = ((EntityQuery)(ref m_DeletedQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val2);
		JobHandle val3 = IJobExtensions.Schedule<CreateWaypointsJob>(new CreateWaypointsJob
		{
			m_DefinitionChunks = definitionChunks,
			m_DeletedChunks = deletedChunks,
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CreationDefinitionType = InternalCompilerInterface.GetComponentTypeHandle<CreationDefinition>(ref __TypeHandle.__Game_Tools_CreationDefinition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SegmentType = InternalCompilerInterface.GetComponentTypeHandle<Segment>(ref __TypeHandle.__Game_Routes_Segment_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WaypointDefinitionType = InternalCompilerInterface.GetBufferTypeHandle<WaypointDefinition>(ref __TypeHandle.__Game_Routes_WaypointDefinition_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteData = InternalCompilerInterface.GetComponentLookup<RouteData>(ref __TypeHandle.__Game_Prefabs_RouteData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PositionData = InternalCompilerInterface.GetComponentLookup<Position>(ref __TypeHandle.__Game_Routes_Position_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SegmentData = InternalCompilerInterface.GetComponentLookup<Segment>(ref __TypeHandle.__Game_Routes_Segment_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathTargetsData = InternalCompilerInterface.GetComponentLookup<PathTargets>(ref __TypeHandle.__Game_Routes_PathTargets_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathInformationData = InternalCompilerInterface.GetComponentLookup<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteWaypoints = InternalCompilerInterface.GetBufferLookup<RouteWaypoint>(ref __TypeHandle.__Game_Routes_RouteWaypoint_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteSegments = InternalCompilerInterface.GetBufferLookup<RouteSegment>(ref __TypeHandle.__Game_Routes_RouteSegment_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathElementData = InternalCompilerInterface.GetBufferLookup<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer()
		}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val, val2));
		definitionChunks.Dispose(val3);
		deletedChunks.Dispose(val3);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val3);
		((SystemBase)this).Dependency = val3;
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
	public GenerateWaypointsSystem()
	{
	}
}
