using System;
using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Routes;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Net;

[CompilerGenerated]
public class OutsideConnectionSystem : GameSystemBase
{
	private enum ConnectionType
	{
		Pedestrian,
		Road,
		Track,
		Parking
	}

	private struct NodeData : IComparable<NodeData>
	{
		public float m_Order;

		public float m_Remoteness;

		public float3 m_Position1;

		public float3 m_Position2;

		public PathNode m_Node1;

		public PathNode m_Node2;

		public PathNode m_Node3;

		public ConnectionType m_ConnectionType;

		public TrackTypes m_TrackType;

		public RoadTypes m_RoadType;

		public Entity m_Owner;

		public int CompareTo(NodeData other)
		{
			return math.select(math.select(math.select(math.select(0, math.select(-1, 1, m_Order > other.m_Order), m_Order != other.m_Order), (int)(m_TrackType - other.m_TrackType), m_TrackType != other.m_TrackType), (int)(m_RoadType - other.m_RoadType), m_RoadType != other.m_RoadType), m_ConnectionType - other.m_ConnectionType, m_ConnectionType != other.m_ConnectionType);
		}
	}

	private struct LaneData : IEquatable<LaneData>
	{
		public PathNode m_Start;

		public PathNode m_End;

		public LaneData(Lane lane)
		{
			m_Start = lane.m_StartNode;
			m_End = lane.m_EndNode;
		}

		public LaneData(NodeData nodeData)
		{
			m_Start = nodeData.m_Node1;
			m_End = nodeData.m_Node3;
		}

		public LaneData(NodeData nodeData1, NodeData nodeData2)
		{
			m_Start = nodeData1.m_Node3;
			m_End = nodeData2.m_Node3;
		}

		public bool Equals(LaneData other)
		{
			if (m_Start.Equals(other.m_Start))
			{
				return m_End.Equals(other.m_End);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (17 * 31 + m_Start.GetHashCode()) * 31 + m_End.GetHashCode();
		}
	}

	[BurstCompile]
	private struct UpdateOutsideConnectionsJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_ConnectionChunks;

		[ReadOnly]
		public NativeList<Entity> m_PrefabEntities;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Lane> m_LaneType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.OutsideConnection> m_OutsideConnectionType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<SubLane> m_SubLaneType;

		[ReadOnly]
		public BufferTypeHandle<ConnectedRoute> m_ConnectedRouteType;

		[ReadOnly]
		public ComponentLookup<Updated> m_UpdatedData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionData;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

		[ReadOnly]
		public ComponentLookup<EdgeLane> m_EdgeLaneData;

		[ReadOnly]
		public ComponentLookup<SecondaryLane> m_SecondaryLaneData;

		[ReadOnly]
		public ComponentLookup<SlaveLane> m_SlaveLaneData;

		[ReadOnly]
		public ComponentLookup<MasterLane> m_MasterLaneData;

		[ReadOnly]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<ConnectionLane> m_ConnectionLaneData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NetLaneData> m_PrefabLaneData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

		[ReadOnly]
		public ComponentLookup<CarLaneData> m_PrefabCarLaneData;

		[ReadOnly]
		public ComponentLookup<TrackLaneData> m_PrefabTrackLaneData;

		[ReadOnly]
		public ComponentLookup<RouteConnectionData> m_PrefabRouteConnectionData;

		[ReadOnly]
		public ComponentLookup<NetLaneArchetypeData> m_PrefabLaneArchetypeData;

		[ReadOnly]
		public ComponentLookup<OutsideConnectionData> m_PrefabOutsideConnectionData;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public BufferLookup<SubLane> m_SubLanes;

		[ReadOnly]
		public BufferLookup<NetCompositionLane> m_PrefabCompositionLanes;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_040b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0410: Unknown result type (might be due to invalid IL or missing references)
			//IL_0415: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0426: Unknown result type (might be due to invalid IL or missing references)
			//IL_0462: Unknown result type (might be due to invalid IL or missing references)
			//IL_0467: Unknown result type (might be due to invalid IL or missing references)
			//IL_0480: Unknown result type (might be due to invalid IL or missing references)
			//IL_0485: Unknown result type (might be due to invalid IL or missing references)
			//IL_0490: Unknown result type (might be due to invalid IL or missing references)
			//IL_0495: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			NativeList<NodeData> val = default(NativeList<NodeData>);
			val._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
			NativeParallelHashMap<LaneData, Entity> laneMap = default(NativeParallelHashMap<LaneData, Entity>);
			laneMap._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
			for (int i = 0; i < m_ConnectionChunks.Length; i++)
			{
				ArchetypeChunk val2 = m_ConnectionChunks[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray(m_EntityType);
				if (((ArchetypeChunk)(ref val2)).Has<Game.Objects.OutsideConnection>(ref m_OutsideConnectionType))
				{
					NativeArray<Owner> nativeArray2 = ((ArchetypeChunk)(ref val2)).GetNativeArray<Owner>(ref m_OwnerType);
					NativeArray<Transform> nativeArray3 = ((ArchetypeChunk)(ref val2)).GetNativeArray<Transform>(ref m_TransformType);
					NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref val2)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
					bool flag = ((ArchetypeChunk)(ref val2)).Has<SubLane>(ref m_SubLaneType);
					for (int j = 0; j < nativeArray.Length; j++)
					{
						PrefabRef prefabRef = nativeArray4[j];
						OutsideConnectionData outsideConnectionData = m_PrefabOutsideConnectionData[prefabRef.m_Prefab];
						if (nativeArray2.Length != 0)
						{
							Owner owner = nativeArray2[j];
							if (m_ConnectedEdges.HasBuffer(owner.m_Owner))
							{
								DynamicBuffer<ConnectedEdge> connectedEdges = m_ConnectedEdges[owner.m_Owner];
								FillNodeData(owner.m_Owner, connectedEdges, outsideConnectionData, val);
								continue;
							}
						}
						if (flag)
						{
							Entity owner2 = nativeArray[j];
							Transform transform = nativeArray3[j];
							if (m_PrefabRouteConnectionData.HasComponent(prefabRef.m_Prefab))
							{
								RouteConnectionData routeConnectionData = m_PrefabRouteConnectionData[prefabRef.m_Prefab];
								FillNodeData(owner2, transform, outsideConnectionData, routeConnectionData, val);
							}
						}
					}
				}
				else
				{
					NativeArray<Lane> nativeArray5 = ((ArchetypeChunk)(ref val2)).GetNativeArray<Lane>(ref m_LaneType);
					for (int k = 0; k < nativeArray5.Length; k++)
					{
						Entity val3 = nativeArray[k];
						Lane lane = nativeArray5[k];
						laneMap.TryAdd(new LaneData(lane), val3);
					}
				}
			}
			NativeSortExtension.Sort<NodeData>(val);
			for (int l = 0; l < val.Length; l++)
			{
				TryCreateLane(val[l], laneMap);
			}
			if (val.Length >= 2)
			{
				int num = 0;
				bool2 val4 = default(bool2);
				while (num < val.Length)
				{
					int num2 = num;
					NodeData nodeData = val[num];
					while (++num2 < val.Length)
					{
						NodeData nodeData2 = val[num2];
						if (nodeData2.m_ConnectionType != nodeData.m_ConnectionType || nodeData2.m_TrackType != nodeData.m_TrackType || nodeData2.m_RoadType != nodeData.m_RoadType)
						{
							break;
						}
					}
					if (nodeData.m_ConnectionType != ConnectionType.Parking)
					{
						int num3 = num2 - num;
						int num4 = num3 - 2;
						for (int m = num; m < num2; m++)
						{
							int num5 = m - 1;
							int num6 = m;
							if (m == num)
							{
								if (num3 <= 2)
								{
									continue;
								}
								num5 += num3;
							}
							NodeData nodeData3 = val[num5];
							NodeData nodeData4 = val[num6];
							TryCreateLane(nodeData3, nodeData4, laneMap);
							float num7 = nodeData4.m_Remoteness - nodeData3.m_Remoteness;
							((bool2)(ref val4))._002Ector(num7 <= 0f, num7 >= 0f);
							float2 val5 = math.select(new float2(float.MinValue, float.MaxValue), float2.op_Implicit(num7), val4);
							for (int n = 1; n < num4; n++)
							{
								if (num7 == 0f)
								{
									break;
								}
								num6 = m + n;
								num6 -= math.select(0, num3, num6 >= num2);
								nodeData4 = val[num6];
								num7 = nodeData4.m_Remoteness - nodeData3.m_Remoteness;
								val4 = new bool2(num7 <= 0f, num7 >= 0f) & new bool2(num7 > val5.x, num7 < val5.y);
								if (math.any(val4))
								{
									TryCreateLane(nodeData3, nodeData4, laneMap);
									val5 = math.select(val5, float2.op_Implicit(num7), val4);
								}
							}
						}
					}
					num = num2;
				}
			}
			if (laneMap.Count() != 0)
			{
				NativeArray<Entity> valueArray = laneMap.GetValueArray(AllocatorHandle.op_Implicit((Allocator)2));
				for (int num8 = 0; num8 < valueArray.Length; num8++)
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Deleted>(valueArray[num8], default(Deleted));
				}
				valueArray.Dispose();
				for (int num9 = 0; num9 < m_ConnectionChunks.Length; num9++)
				{
					ArchetypeChunk val6 = m_ConnectionChunks[num9];
					if (!((ArchetypeChunk)(ref val6)).Has<Game.Objects.OutsideConnection>(ref m_OutsideConnectionType))
					{
						continue;
					}
					BufferAccessor<ConnectedRoute> bufferAccessor = ((ArchetypeChunk)(ref val6)).GetBufferAccessor<ConnectedRoute>(ref m_ConnectedRouteType);
					for (int num10 = 0; num10 < bufferAccessor.Length; num10++)
					{
						DynamicBuffer<ConnectedRoute> val7 = bufferAccessor[num10];
						for (int num11 = 0; num11 < val7.Length; num11++)
						{
							((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val7[num11].m_Waypoint, default(Updated));
						}
					}
				}
			}
			laneMap.Dispose();
			val.Dispose();
		}

		private void FillNodeData(Entity owner, Transform transform, OutsideConnectionData outsideConnectionData, RouteConnectionData routeConnectionData, NativeList<NodeData> buffer)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			if (routeConnectionData.m_AccessConnectionType == RouteConnectionType.Road)
			{
				NodeData nodeData = default(NodeData);
				nodeData.m_Position1 = transform.m_Position;
				nodeData.m_Position2 = CalculateEndPos(nodeData.m_Position1);
				nodeData.m_Node1 = new PathNode(owner, (ushort)num++);
				nodeData.m_Node2 = new PathNode(owner, (ushort)num++);
				nodeData.m_Node3 = new PathNode(owner, (ushort)num++);
				nodeData.m_Order = math.atan2(nodeData.m_Position1.z, nodeData.m_Position1.x);
				nodeData.m_Remoteness = outsideConnectionData.m_Remoteness;
				nodeData.m_Owner = owner;
				nodeData.m_ConnectionType = ConnectionType.Road;
				nodeData.m_TrackType = TrackTypes.None;
				nodeData.m_RoadType = routeConnectionData.m_AccessRoadType;
				buffer.Add(ref nodeData);
			}
			if (routeConnectionData.m_AccessConnectionType == RouteConnectionType.Road)
			{
				NodeData nodeData2 = default(NodeData);
				nodeData2.m_Position1 = transform.m_Position;
				nodeData2.m_Position1.y += 2f;
				nodeData2.m_Position2 = CalculateEndPos(nodeData2.m_Position1);
				nodeData2.m_Node1 = new PathNode(owner, (ushort)num++);
				nodeData2.m_Node2 = new PathNode(owner, (ushort)num++);
				nodeData2.m_Node3 = new PathNode(owner, (ushort)num++);
				nodeData2.m_Order = math.atan2(nodeData2.m_Position1.z, nodeData2.m_Position1.x);
				nodeData2.m_Remoteness = outsideConnectionData.m_Remoteness;
				nodeData2.m_Owner = owner;
				nodeData2.m_ConnectionType = ConnectionType.Pedestrian;
				nodeData2.m_TrackType = TrackTypes.None;
				nodeData2.m_RoadType = RoadTypes.None;
				buffer.Add(ref nodeData2);
			}
		}

		private void FillNodeData(Entity node, DynamicBuffer<ConnectedEdge> connectedEdges, OutsideConnectionData outsideConnectionData, NativeList<NodeData> buffer)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_040e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0413: Unknown result type (might be due to invalid IL or missing references)
			//IL_0418: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0713: Unknown result type (might be due to invalid IL or missing references)
			//IL_071f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0742: Unknown result type (might be due to invalid IL or missing references)
			//IL_0743: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0473: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0508: Unknown result type (might be due to invalid IL or missing references)
			//IL_052a: Unknown result type (might be due to invalid IL or missing references)
			//IL_053d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0552: Unknown result type (might be due to invalid IL or missing references)
			//IL_055e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0581: Unknown result type (might be due to invalid IL or missing references)
			//IL_0582: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_080d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0812: Unknown result type (might be due to invalid IL or missing references)
			//IL_081b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0820: Unknown result type (might be due to invalid IL or missing references)
			//IL_083f: Unknown result type (might be due to invalid IL or missing references)
			//IL_086c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0878: Unknown result type (might be due to invalid IL or missing references)
			//IL_089b: Unknown result type (might be due to invalid IL or missing references)
			//IL_089c: Unknown result type (might be due to invalid IL or missing references)
			//IL_059b: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_061f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0623: Unknown result type (might be due to invalid IL or missing references)
			//IL_0628: Unknown result type (might be due to invalid IL or missing references)
			//IL_062d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_065b: Unknown result type (might be due to invalid IL or missing references)
			//IL_065f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0664: Unknown result type (might be due to invalid IL or missing references)
			//IL_0669: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			int length = buffer.Length;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			float3 val = default(float3);
			NodeData nodeData = default(NodeData);
			NodeData nodeData2 = default(NodeData);
			for (int i = 0; i < connectedEdges.Length; i++)
			{
				ConnectedEdge connectedEdge = connectedEdges[i];
				bool flag = m_EdgeData[connectedEdge.m_Edge].m_End == node;
				if (!m_UpdatedData.HasComponent(connectedEdge.m_Edge) && m_SubLanes.HasBuffer(connectedEdge.m_Edge))
				{
					DynamicBuffer<SubLane> val2 = m_SubLanes[connectedEdge.m_Edge];
					float num5 = math.select(0f, 1f, flag);
					for (int j = 0; j < val2.Length; j++)
					{
						Entity subLane = val2[j].m_SubLane;
						if (!m_EdgeLaneData.HasComponent(subLane) || m_SecondaryLaneData.HasComponent(subLane) || m_SlaveLaneData.HasComponent(subLane))
						{
							continue;
						}
						bool2 val3 = m_EdgeLaneData[subLane].m_EdgeDelta == num5;
						if (!math.any(val3))
						{
							continue;
						}
						bool y = val3.y;
						Curve curve = m_CurveData[subLane];
						if (y)
						{
							curve.m_Bezier = MathUtils.Invert(curve.m_Bezier);
						}
						PrefabRef prefabRef = m_PrefabRefData[subLane];
						NetLaneData netLaneData = m_PrefabLaneData[prefabRef.m_Prefab];
						if ((!m_MasterLaneData.HasComponent(subLane) && ((uint)netLaneData.m_Flags & (uint)(y ? 512 : 256)) != 0) || (netLaneData.m_Flags & (LaneFlags.Parking | LaneFlags.Utility | LaneFlags.FindAnchor)) != 0)
						{
							continue;
						}
						Lane lane = m_LaneData[subLane];
						byte laneIndex = ((!y) ? ((byte)(lane.m_StartNode.GetLaneIndex() & 0xFF)) : ((byte)(lane.m_EndNode.GetLaneIndex() & 0xFF)));
						nodeData.m_Position1 = curve.m_Bezier.a;
						nodeData.m_Position2 = CalculateEndPos(nodeData.m_Position1);
						nodeData.m_Node1 = new PathNode(connectedEdge.m_Edge, laneIndex, (byte)math.select(0, 4, flag));
						nodeData.m_Node2 = new PathNode(node, (ushort)num++);
						nodeData.m_Node3 = new PathNode(node, (ushort)num++);
						nodeData.m_Order = math.atan2(nodeData.m_Position1.z, nodeData.m_Position1.x);
						nodeData.m_Remoteness = outsideConnectionData.m_Remoteness;
						nodeData.m_Owner = node;
						if ((netLaneData.m_Flags & LaneFlags.Track) != 0)
						{
							TrackLaneData trackLaneData = m_PrefabTrackLaneData[prefabRef.m_Prefab];
							nodeData.m_ConnectionType = ConnectionType.Track;
							nodeData.m_TrackType = trackLaneData.m_TrackTypes;
							nodeData.m_RoadType = RoadTypes.None;
							num2++;
							val += nodeData.m_Position1;
						}
						else if ((netLaneData.m_Flags & LaneFlags.Road) != 0)
						{
							CarLaneData carLaneData = m_PrefabCarLaneData[prefabRef.m_Prefab];
							nodeData.m_ConnectionType = ConnectionType.Road;
							nodeData.m_TrackType = TrackTypes.None;
							nodeData.m_RoadType = carLaneData.m_RoadTypes;
							num3++;
							val += nodeData.m_Position1;
						}
						else
						{
							if ((netLaneData.m_Flags & LaneFlags.Pedestrian) == 0)
							{
								continue;
							}
							nodeData.m_ConnectionType = ConnectionType.Pedestrian;
							nodeData.m_TrackType = TrackTypes.None;
							nodeData.m_RoadType = RoadTypes.None;
							num4++;
							val += nodeData.m_Position1;
						}
						buffer.Add(ref nodeData);
					}
					continue;
				}
				Composition composition = m_CompositionData[connectedEdge.m_Edge];
				EdgeGeometry edgeGeometry = m_EdgeGeometryData[connectedEdge.m_Edge];
				NetCompositionData netCompositionData = m_PrefabCompositionData[composition.m_Edge];
				DynamicBuffer<NetCompositionLane> val4 = m_PrefabCompositionLanes[composition.m_Edge];
				if (flag)
				{
					edgeGeometry.m_Start.m_Left = MathUtils.Invert(edgeGeometry.m_End.m_Right);
					edgeGeometry.m_Start.m_Right = MathUtils.Invert(edgeGeometry.m_End.m_Left);
				}
				for (int k = 0; k < val4.Length; k++)
				{
					NetCompositionLane netCompositionLane = val4[k];
					bool flag2 = flag == ((netCompositionLane.m_Flags & LaneFlags.Invert) == 0);
					if ((netCompositionLane.m_Flags & (LaneFlags.Slave | LaneFlags.Parking | LaneFlags.Utility)) != 0 || ((uint)netCompositionLane.m_Flags & (uint)(flag2 ? 512 : 256)) != 0)
					{
						continue;
					}
					float num6 = netCompositionLane.m_Position.x / math.max(1f, netCompositionData.m_Width) + 0.5f;
					if (!flag)
					{
						num6 = 1f - num6;
					}
					Bezier4x3 val5 = MathUtils.Lerp(edgeGeometry.m_Start.m_Right, edgeGeometry.m_Start.m_Left, num6);
					nodeData2.m_Position1 = val5.a;
					nodeData2.m_Position1.y += netCompositionLane.m_Position.y;
					nodeData2.m_Position2 = CalculateEndPos(nodeData2.m_Position1);
					nodeData2.m_Node1 = new PathNode(connectedEdge.m_Edge, netCompositionLane.m_Index, (byte)math.select(0, 4, flag));
					nodeData2.m_Node2 = new PathNode(node, (ushort)num++);
					nodeData2.m_Node3 = new PathNode(node, (ushort)num++);
					nodeData2.m_Order = math.atan2(nodeData2.m_Position1.z, nodeData2.m_Position1.x);
					nodeData2.m_Remoteness = outsideConnectionData.m_Remoteness;
					nodeData2.m_Owner = node;
					if ((netCompositionLane.m_Flags & LaneFlags.Track) != 0)
					{
						TrackLaneData trackLaneData2 = m_PrefabTrackLaneData[netCompositionLane.m_Lane];
						nodeData2.m_ConnectionType = ConnectionType.Track;
						nodeData2.m_TrackType = trackLaneData2.m_TrackTypes;
						nodeData2.m_RoadType = RoadTypes.None;
						num2++;
						val += nodeData2.m_Position1;
					}
					else if ((netCompositionLane.m_Flags & LaneFlags.Road) != 0)
					{
						CarLaneData carLaneData2 = m_PrefabCarLaneData[netCompositionLane.m_Lane];
						nodeData2.m_ConnectionType = ConnectionType.Road;
						nodeData2.m_TrackType = TrackTypes.None;
						nodeData2.m_RoadType = carLaneData2.m_RoadTypes;
						num3++;
						val += nodeData2.m_Position1;
					}
					else
					{
						if ((netCompositionLane.m_Flags & LaneFlags.Pedestrian) == 0)
						{
							continue;
						}
						nodeData2.m_ConnectionType = ConnectionType.Pedestrian;
						nodeData2.m_TrackType = TrackTypes.None;
						nodeData2.m_RoadType = RoadTypes.None;
						num4++;
						val += nodeData2.m_Position1;
					}
					buffer.Add(ref nodeData2);
				}
			}
			if (num4 == 0 && (num2 != 0 || num3 != 0))
			{
				val /= (float)(num2 + num3);
				NodeData nodeData3 = default(NodeData);
				nodeData3.m_Position1 = val;
				nodeData3.m_Position2 = CalculateEndPos(nodeData3.m_Position1);
				nodeData3.m_Node1 = new PathNode(node, (ushort)num++);
				nodeData3.m_Node2 = new PathNode(node, (ushort)num++);
				nodeData3.m_Node3 = new PathNode(node, (ushort)num++);
				nodeData3.m_Order = math.atan2(nodeData3.m_Position1.z, nodeData3.m_Position1.x);
				nodeData3.m_Remoteness = outsideConnectionData.m_Remoteness;
				nodeData3.m_Owner = node;
				nodeData3.m_ConnectionType = ConnectionType.Pedestrian;
				nodeData3.m_TrackType = TrackTypes.None;
				nodeData3.m_RoadType = RoadTypes.None;
				buffer.Add(ref nodeData3);
			}
			if (num3 == 0)
			{
				return;
			}
			int length2 = buffer.Length;
			NodeData nodeData7 = default(NodeData);
			for (int l = length; l < length2; l++)
			{
				NodeData nodeData4 = buffer[l];
				if (nodeData4.m_ConnectionType != ConnectionType.Road || nodeData4.m_RoadType != RoadTypes.Car)
				{
					continue;
				}
				float num7 = float.MaxValue;
				NodeData nodeData5 = default(NodeData);
				bool flag3 = false;
				for (int m = length; m < length2; m++)
				{
					NodeData nodeData6 = buffer[m];
					if (nodeData6.m_ConnectionType == ConnectionType.Pedestrian)
					{
						float num8 = math.distance(nodeData4.m_Position1, nodeData6.m_Position1);
						if (num8 < num7)
						{
							num7 = num8;
							nodeData5 = nodeData6;
							flag3 = true;
						}
					}
				}
				if (flag3)
				{
					nodeData7.m_Position1 = nodeData4.m_Position2;
					nodeData7.m_Position2 = nodeData5.m_Position2;
					nodeData7.m_Node1 = new PathNode(nodeData4.m_Node2, 1f);
					nodeData7.m_Node2 = new PathNode(node, (ushort)num++);
					nodeData7.m_Node3 = new PathNode(nodeData5.m_Node2, 1f);
					nodeData7.m_Order = math.atan2(nodeData7.m_Position1.z, nodeData7.m_Position1.x);
					nodeData7.m_Remoteness = outsideConnectionData.m_Remoteness;
					nodeData7.m_Owner = node;
					nodeData7.m_ConnectionType = ConnectionType.Parking;
					nodeData7.m_TrackType = TrackTypes.None;
					nodeData7.m_RoadType = RoadTypes.Car;
					buffer.Add(ref nodeData7);
				}
			}
		}

		private void TryCreateLane(NodeData nodeData, NativeParallelHashMap<LaneData, Entity> laneMap)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			ConnectionLane connectionLane = default(ConnectionLane);
			connectionLane.m_AccessRestriction = Entity.Null;
			connectionLane.m_Flags = ConnectionLaneFlags.Start | ConnectionLaneFlags.Outside;
			connectionLane.m_TrackTypes = nodeData.m_TrackType;
			connectionLane.m_RoadTypes = nodeData.m_RoadType;
			switch (nodeData.m_ConnectionType)
			{
			case ConnectionType.Road:
				if (nodeData.m_RoadType == RoadTypes.Car)
				{
					connectionLane.m_Flags |= ConnectionLaneFlags.SecondaryStart | ConnectionLaneFlags.SecondaryEnd | ConnectionLaneFlags.Road | ConnectionLaneFlags.AllowMiddle;
				}
				else
				{
					connectionLane.m_Flags |= ConnectionLaneFlags.Road | ConnectionLaneFlags.AllowMiddle;
				}
				break;
			case ConnectionType.Track:
				connectionLane.m_Flags |= ConnectionLaneFlags.Track | ConnectionLaneFlags.AllowMiddle;
				break;
			case ConnectionType.Pedestrian:
				connectionLane.m_Flags |= ConnectionLaneFlags.Pedestrian | ConnectionLaneFlags.AllowMiddle | ConnectionLaneFlags.AllowCargo;
				break;
			case ConnectionType.Parking:
				connectionLane.m_Flags |= ConnectionLaneFlags.SecondaryStart | ConnectionLaneFlags.Parking;
				break;
			}
			Entity val = default(Entity);
			if (laneMap.TryGetValue(new LaneData(nodeData), ref val) && m_ConnectionLaneData[val].Equals(connectionLane))
			{
				Curve curve = CalculateCurve(nodeData);
				Curve curve2 = m_CurveData[val];
				if (!((Bezier4x3)(ref curve.m_Bezier)).Equals(curve2.m_Bezier))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Curve>(val, curve);
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val, default(Updated));
				}
				laneMap.Remove(new LaneData(nodeData));
			}
			else if (m_PrefabEntities.Length != 0)
			{
				Entity val2 = m_PrefabEntities[0];
				NetLaneArchetypeData netLaneArchetypeData = m_PrefabLaneArchetypeData[val2];
				Lane lane = default(Lane);
				lane.m_StartNode = nodeData.m_Node1;
				lane.m_MiddleNode = nodeData.m_Node2;
				lane.m_EndNode = nodeData.m_Node3;
				val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(netLaneArchetypeData.m_LaneArchetype);
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PrefabRef>(val, new PrefabRef(val2));
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Lane>(val, lane);
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Curve>(val, CalculateCurve(nodeData));
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<ConnectionLane>(val, connectionLane);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OutsideConnection>(val, default(OutsideConnection));
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Owner>(val, new Owner(nodeData.m_Owner));
			}
		}

		private void TryCreateLane(NodeData nodeData1, NodeData nodeData2, NativeParallelHashMap<LaneData, Entity> laneMap)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			ConnectionLane connectionLane = default(ConnectionLane);
			connectionLane.m_AccessRestriction = Entity.Null;
			connectionLane.m_Flags = ConnectionLaneFlags.Distance | ConnectionLaneFlags.Outside;
			connectionLane.m_TrackTypes = nodeData1.m_TrackType;
			connectionLane.m_RoadTypes = nodeData1.m_RoadType;
			switch (nodeData1.m_ConnectionType)
			{
			case ConnectionType.Road:
				if (nodeData1.m_RoadType == RoadTypes.Car)
				{
					connectionLane.m_Flags |= ConnectionLaneFlags.SecondaryStart | ConnectionLaneFlags.SecondaryEnd | ConnectionLaneFlags.Road;
				}
				else
				{
					connectionLane.m_Flags |= ConnectionLaneFlags.Road;
				}
				break;
			case ConnectionType.Track:
				connectionLane.m_Flags |= ConnectionLaneFlags.Track;
				break;
			case ConnectionType.Pedestrian:
				connectionLane.m_Flags |= ConnectionLaneFlags.Pedestrian;
				break;
			}
			Entity val = default(Entity);
			if (laneMap.TryGetValue(new LaneData(nodeData1, nodeData2), ref val) && m_ConnectionLaneData[val].Equals(connectionLane))
			{
				Curve curve = CalculateCurve(nodeData1, nodeData2);
				Curve curve2 = m_CurveData[val];
				if (!((Bezier4x3)(ref curve.m_Bezier)).Equals(curve2.m_Bezier))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Curve>(val, curve);
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val, default(Updated));
				}
				laneMap.Remove(new LaneData(nodeData1, nodeData2));
			}
			else if (m_PrefabEntities.Length != 0)
			{
				Entity val2 = m_PrefabEntities[0];
				NetLaneArchetypeData netLaneArchetypeData = m_PrefabLaneArchetypeData[val2];
				Lane lane = default(Lane);
				lane.m_StartNode = nodeData1.m_Node3;
				lane.m_MiddleNode = default(PathNode);
				lane.m_EndNode = nodeData2.m_Node3;
				val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(netLaneArchetypeData.m_LaneArchetype);
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PrefabRef>(val, new PrefabRef(val2));
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Lane>(val, lane);
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Curve>(val, CalculateCurve(nodeData1, nodeData2));
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<ConnectionLane>(val, connectionLane);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OutsideConnection>(val, default(OutsideConnection));
			}
		}

		private float3 CalculateEndPos(float3 startPos)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			float3 result = startPos;
			float2 xz = ((float3)(ref startPos)).xz;
			if (MathUtils.TryNormalize(ref xz, 10f))
			{
				((float3)(ref result)).xz = ((float3)(ref result)).xz + xz;
			}
			return result;
		}

		private Curve CalculateCurve(NodeData nodeData)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			Curve result = default(Curve);
			result.m_Bezier = NetUtils.StraightCurve(nodeData.m_Position1, nodeData.m_Position2);
			result.m_Length = 10f;
			return result;
		}

		private Curve CalculateCurve(NodeData nodeData1, NodeData nodeData2)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			float3 val = nodeData1.m_Position2;
			float3 val2 = nodeData2.m_Position2;
			float3 val3 = math.lerp(val, val2, 1f / 3f);
			float3 val4 = math.lerp(val, val2, 2f / 3f);
			float2 val5 = ((float3)(ref val3)).xz;
			float2 val6 = ((float3)(ref val4)).xz;
			float num = math.cmax(math.abs(((float3)(ref val)).xz - ((float3)(ref val2)).xz));
			float2 val7 = default(float2);
			((float2)(ref val7))._002Ector(math.length(((float3)(ref val)).xz), math.length(((float3)(ref val2)).xz));
			val7 = math.lerp(float2.op_Implicit(val7.x), float2.op_Implicit(val7.y), new float2(1f / 3f, 2f / 3f)) + num;
			if (!MathUtils.TryNormalize(ref val5, val7.x))
			{
				((float2)(ref val5))._002Ector(0f, val7.x);
			}
			if (!MathUtils.TryNormalize(ref val6, val7.y))
			{
				((float2)(ref val6))._002Ector(0f, val7.y);
			}
			float num2 = 50f + math.abs(nodeData2.m_Remoteness - nodeData1.m_Remoteness) * 0.5f;
			float2 val8 = ((float3)(ref val2)).xz - ((float3)(ref val)).xz;
			val5 += val5 * (num2 / math.max(1f, val7.x));
			val6 += val6 * (num2 / math.max(1f, val7.y));
			if (MathUtils.TryNormalize(ref val8, num2))
			{
				val5 -= val8;
				val6 += val8;
			}
			((float3)(ref val3)).xz = val5;
			((float3)(ref val4)).xz = val6;
			Curve result = default(Curve);
			result.m_Bezier = new Bezier4x3(val, val3, val4, val2);
			result.m_Length = MathUtils.Length(result.m_Bezier);
			return result;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Lane> __Game_Net_Lane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.OutsideConnection> __Game_Objects_OutsideConnection_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<SubLane> __Game_Net_SubLane_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<ConnectedRoute> __Game_Routes_ConnectedRoute_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Updated> __Game_Common_Updated_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Composition> __Game_Net_Composition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> __Game_Net_EdgeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeLane> __Game_Net_EdgeLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SecondaryLane> __Game_Net_SecondaryLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SlaveLane> __Game_Net_SlaveLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MasterLane> __Game_Net_MasterLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Lane> __Game_Net_Lane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ConnectionLane> __Game_Net_ConnectionLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> __Game_Prefabs_NetCompositionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneData> __Game_Prefabs_NetLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarLaneData> __Game_Prefabs_CarLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrackLaneData> __Game_Prefabs_TrackLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RouteConnectionData> __Game_Prefabs_RouteConnectionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneArchetypeData> __Game_Prefabs_NetLaneArchetypeData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<OutsideConnectionData> __Game_Prefabs_OutsideConnectionData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<NetCompositionLane> __Game_Prefabs_NetCompositionLane_RO_BufferLookup;

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
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Net_Lane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Lane>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Objects_OutsideConnection_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Objects.OutsideConnection>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Net_SubLane_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubLane>(true);
			__Game_Routes_ConnectedRoute_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ConnectedRoute>(true);
			__Game_Common_Updated_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Updated>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_Composition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Composition>(true);
			__Game_Net_EdgeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeGeometry>(true);
			__Game_Net_EdgeLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeLane>(true);
			__Game_Net_SecondaryLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SecondaryLane>(true);
			__Game_Net_SlaveLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SlaveLane>(true);
			__Game_Net_MasterLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MasterLane>(true);
			__Game_Net_Lane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(true);
			__Game_Net_ConnectionLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ConnectionLane>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_NetCompositionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionData>(true);
			__Game_Prefabs_NetLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneData>(true);
			__Game_Prefabs_CarLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarLaneData>(true);
			__Game_Prefabs_TrackLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrackLaneData>(true);
			__Game_Prefabs_RouteConnectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RouteConnectionData>(true);
			__Game_Prefabs_NetLaneArchetypeData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneArchetypeData>(true);
			__Game_Prefabs_OutsideConnectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<OutsideConnectionData>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubLane>(true);
			__Game_Prefabs_NetCompositionLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<NetCompositionLane>(true);
		}
	}

	private ModificationBarrier4 m_ModificationBarrier;

	private EntityQuery m_UpdatedQuery;

	private EntityQuery m_ConnectionQuery;

	private EntityQuery m_PrefabQuery;

	private bool m_Regenerate;

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
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Expected O, but got Unknown
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier4>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Objects.OutsideConnection>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Game.Objects.ElectricityOutsideConnection>(),
			ComponentType.ReadOnly<Game.Objects.WaterPipeOutsideConnection>()
		};
		array[0] = val;
		m_UpdatedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[2];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Objects.OutsideConnection>(),
			ComponentType.ReadOnly<Transform>()
		};
		val.None = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Game.Objects.ElectricityOutsideConnection>(),
			ComponentType.ReadOnly<Game.Objects.WaterPipeOutsideConnection>()
		};
		array2[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<OutsideConnection>(),
			ComponentType.ReadOnly<Lane>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array2[1] = val;
		m_ConnectionQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		m_PrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<ConnectionLaneData>(),
			ComponentType.ReadOnly<PrefabData>()
		});
	}

	protected override void OnGameLoaded(Context context)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		base.OnGameLoaded(context);
		if (((Context)(ref context)).version < Version.outsideConnectionRemoteness)
		{
			m_Regenerate = true;
		}
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		if (m_Regenerate || !((EntityQuery)(ref m_UpdatedQuery)).IsEmptyIgnoreFilter)
		{
			m_Regenerate = false;
			JobHandle val = default(JobHandle);
			NativeList<ArchetypeChunk> connectionChunks = ((EntityQuery)(ref m_ConnectionQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
			JobHandle val2 = default(JobHandle);
			NativeList<Entity> prefabEntities = ((EntityQuery)(ref m_PrefabQuery)).ToEntityListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val2);
			JobHandle val3 = IJobExtensions.Schedule<UpdateOutsideConnectionsJob>(new UpdateOutsideConnectionsJob
			{
				m_ConnectionChunks = connectionChunks,
				m_PrefabEntities = prefabEntities,
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_LaneType = InternalCompilerInterface.GetComponentTypeHandle<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OutsideConnectionType = InternalCompilerInterface.GetComponentTypeHandle<Game.Objects.OutsideConnection>(ref __TypeHandle.__Game_Objects_OutsideConnection_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_SubLaneType = InternalCompilerInterface.GetBufferTypeHandle<SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectedRouteType = InternalCompilerInterface.GetBufferTypeHandle<ConnectedRoute>(ref __TypeHandle.__Game_Routes_ConnectedRoute_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_UpdatedData = InternalCompilerInterface.GetComponentLookup<Updated>(ref __TypeHandle.__Game_Common_Updated_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CompositionData = InternalCompilerInterface.GetComponentLookup<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeGeometryData = InternalCompilerInterface.GetComponentLookup<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeLaneData = InternalCompilerInterface.GetComponentLookup<EdgeLane>(ref __TypeHandle.__Game_Net_EdgeLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SecondaryLaneData = InternalCompilerInterface.GetComponentLookup<SecondaryLane>(ref __TypeHandle.__Game_Net_SecondaryLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SlaveLaneData = InternalCompilerInterface.GetComponentLookup<SlaveLane>(ref __TypeHandle.__Game_Net_SlaveLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_MasterLaneData = InternalCompilerInterface.GetComponentLookup<MasterLane>(ref __TypeHandle.__Game_Net_MasterLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectionLaneData = InternalCompilerInterface.GetComponentLookup<ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabLaneData = InternalCompilerInterface.GetComponentLookup<NetLaneData>(ref __TypeHandle.__Game_Prefabs_NetLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabCarLaneData = InternalCompilerInterface.GetComponentLookup<CarLaneData>(ref __TypeHandle.__Game_Prefabs_CarLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabTrackLaneData = InternalCompilerInterface.GetComponentLookup<TrackLaneData>(ref __TypeHandle.__Game_Prefabs_TrackLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRouteConnectionData = InternalCompilerInterface.GetComponentLookup<RouteConnectionData>(ref __TypeHandle.__Game_Prefabs_RouteConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabLaneArchetypeData = InternalCompilerInterface.GetComponentLookup<NetLaneArchetypeData>(ref __TypeHandle.__Game_Prefabs_NetLaneArchetypeData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabOutsideConnectionData = InternalCompilerInterface.GetComponentLookup<OutsideConnectionData>(ref __TypeHandle.__Game_Prefabs_OutsideConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubLanes = InternalCompilerInterface.GetBufferLookup<SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabCompositionLanes = InternalCompilerInterface.GetBufferLookup<NetCompositionLane>(ref __TypeHandle.__Game_Prefabs_NetCompositionLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer()
			}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val, val2));
			connectionChunks.Dispose(val3);
			prefabEntities.Dispose(val3);
			((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val3);
			((SystemBase)this).Dependency = val3;
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
	public OutsideConnectionSystem()
	{
	}
}
