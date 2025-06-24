using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Notifications;
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
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Net;

[CompilerGenerated]
public class ConnectionWarningSystem : GameSystemBase
{
	[BurstCompile]
	private struct CollectOwnersJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Node> m_NodeType;

		[ReadOnly]
		public ComponentTypeHandle<Building> m_BuildingType;

		[ReadOnly]
		public ComponentTypeHandle<RoadConnectionUpdated> m_RoadConnectionUpdatedType;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		public NativeList<Entity> m_Owners;

		public void Execute()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			NativeParallelHashSet<Entity> val = default(NativeParallelHashSet<Entity>);
			val._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
			for (int i = 0; i < m_Owners.Length; i++)
			{
				val.Add(m_Owners[i]);
			}
			Owner owner2 = default(Owner);
			Owner owner3 = default(Owner);
			for (int j = 0; j < m_Chunks.Length; j++)
			{
				ArchetypeChunk val2 = m_Chunks[j];
				NativeArray<RoadConnectionUpdated> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray<RoadConnectionUpdated>(ref m_RoadConnectionUpdatedType);
				if (nativeArray.Length != 0)
				{
					for (int k = 0; k < nativeArray.Length; k++)
					{
						RoadConnectionUpdated roadConnectionUpdated = nativeArray[k];
						if (!m_TempData.HasComponent(roadConnectionUpdated.m_Building) && val.Add(roadConnectionUpdated.m_Building))
						{
							m_Owners.Add(ref roadConnectionUpdated.m_Building);
						}
					}
					continue;
				}
				NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref val2)).GetNativeArray(m_EntityType);
				NativeArray<Owner> nativeArray3 = ((ArchetypeChunk)(ref val2)).GetNativeArray<Owner>(ref m_OwnerType);
				if (nativeArray3.Length != 0)
				{
					bool flag = ((ArchetypeChunk)(ref val2)).Has<Node>(ref m_NodeType);
					bool flag2 = ((ArchetypeChunk)(ref val2)).Has<Building>(ref m_BuildingType);
					for (int l = 0; l < nativeArray3.Length; l++)
					{
						Owner owner = nativeArray3[l];
						if (flag && m_EdgeData.HasComponent(owner.m_Owner))
						{
							if (!m_OwnerData.TryGetComponent(owner.m_Owner, ref owner2))
							{
								Entity val3 = nativeArray2[l];
								if (val.Add(val3))
								{
									m_Owners.Add(ref val3);
								}
								continue;
							}
							owner = owner2;
						}
						if (flag2)
						{
							Entity val4 = nativeArray2[l];
							if (val.Add(val4))
							{
								m_Owners.Add(ref val4);
							}
						}
						while (val.Add(owner.m_Owner))
						{
							m_Owners.Add(ref owner.m_Owner);
							if (!m_OwnerData.TryGetComponent(owner.m_Owner, ref owner3))
							{
								break;
							}
							owner = owner3;
						}
					}
					continue;
				}
				for (int m = 0; m < nativeArray2.Length; m++)
				{
					Entity val5 = nativeArray2[m];
					if (val.Add(val5))
					{
						m_Owners.Add(ref val5);
					}
				}
			}
			val.Dispose();
		}
	}

	[BurstCompile]
	private struct CollectOwnersJob2 : IJob
	{
		public struct NodeIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Bounds2 m_Bounds;

			public ComponentLookup<Node> m_NodeData;

			public ComponentLookup<Edge> m_EdgeData;

			public ComponentLookup<Owner> m_OwnerData;

			public NativeParallelHashSet<Entity> m_OwnerSet;

			public NativeList<Entity> m_Owners;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity entity)
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
				if (!Intersect(bounds) || !m_NodeData.HasComponent(entity))
				{
					return;
				}
				Owner owner = default(Owner);
				if (m_OwnerData.TryGetComponent(entity, ref owner))
				{
					if (m_EdgeData.HasComponent(owner.m_Owner))
					{
						Owner owner2 = default(Owner);
						if (!m_OwnerData.TryGetComponent(owner.m_Owner, ref owner2))
						{
							if (m_OwnerSet.Add(entity))
							{
								m_Owners.Add(ref entity);
							}
							return;
						}
						owner = owner2;
					}
					Owner owner3 = default(Owner);
					while (m_OwnerSet.Add(owner.m_Owner))
					{
						m_Owners.Add(ref owner.m_Owner);
						if (m_OwnerData.TryGetComponent(owner.m_Owner, ref owner3))
						{
							owner = owner3;
							continue;
						}
						break;
					}
				}
				else if (m_OwnerSet.Add(entity))
				{
					m_Owners.Add(ref entity);
				}
			}
		}

		public struct BuildingIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Bounds2 m_Bounds;

			public ComponentLookup<Building> m_BuildingData;

			public ComponentLookup<Owner> m_OwnerData;

			public NativeParallelHashSet<Entity> m_OwnerSet;

			public NativeList<Entity> m_Owners;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity entity)
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				if (!Intersect(bounds) || !m_BuildingData.HasComponent(entity))
				{
					return;
				}
				if (m_OwnerSet.Add(entity))
				{
					m_Owners.Add(ref entity);
				}
				Owner owner = default(Owner);
				if (!m_OwnerData.TryGetComponent(entity, ref owner))
				{
					return;
				}
				Owner owner2 = default(Owner);
				while (m_OwnerSet.Add(owner.m_Owner))
				{
					m_Owners.Add(ref owner.m_Owner);
					if (m_OwnerData.TryGetComponent(owner.m_Owner, ref owner2))
					{
						owner = owner2;
						continue;
					}
					break;
				}
			}
		}

		[ReadOnly]
		public ComponentLookup<Node> m_NodeData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_ObjectSearchTree;

		[ReadOnly]
		public NativeList<Bounds2> m_Bounds;

		public NativeList<Entity> m_Owners;

		public void Execute()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			NativeParallelHashSet<Entity> ownerSet = default(NativeParallelHashSet<Entity>);
			ownerSet._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
			for (int i = 0; i < m_Owners.Length; i++)
			{
				ownerSet.Add(m_Owners[i]);
			}
			NodeIterator nodeIterator = new NodeIterator
			{
				m_NodeData = m_NodeData,
				m_EdgeData = m_EdgeData,
				m_OwnerData = m_OwnerData,
				m_OwnerSet = ownerSet,
				m_Owners = m_Owners
			};
			BuildingIterator buildingIterator = new BuildingIterator
			{
				m_BuildingData = m_BuildingData,
				m_OwnerData = m_OwnerData,
				m_OwnerSet = ownerSet,
				m_Owners = m_Owners
			};
			for (int j = 0; j < m_Bounds.Length; j++)
			{
				nodeIterator.m_Bounds = (buildingIterator.m_Bounds = m_Bounds[j]);
				m_NetSearchTree.Iterate<NodeIterator>(ref nodeIterator, 0);
				m_ObjectSearchTree.Iterate<BuildingIterator>(ref buildingIterator, 0);
			}
			ownerSet.Dispose();
		}
	}

	private struct PathfindElement
	{
		public PathNode m_StartNode;

		public PathNode m_MiddleNode;

		public PathNode m_EndNode;

		public Entity m_Entity;

		public bool2 m_Connected;

		public bool2 m_Directions;

		public byte m_IconType;

		public byte m_IconLocation;

		public byte m_IconLocation2;

		public sbyte m_Priority;

		public bool m_CanIgnore;

		public bool m_Optional;

		public sbyte m_SubConnection;
	}

	private struct BufferElement
	{
		public PathNode m_Node;

		public bool2 m_Connected;
	}

	private struct Connection
	{
		public RoadTypes m_RoadTypes;

		public RoadTypes m_RoadTypes2;

		public TrackTypes m_TrackTypes;

		public TrackTypes m_TrackTypes2;
	}

	[BurstCompile]
	private struct CheckOwnersJob : IJobParallelForDefer
	{
		public struct MapTileIterator : INativeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>
		{
			public int m_Result;

			public float3 m_Position;

			public ComponentLookup<Native> m_NativeData;

			public ComponentLookup<MapTile> m_MapTileData;

			public BufferLookup<Game.Areas.Node> m_AreaNodes;

			public BufferLookup<Triangle> m_AreaTriangles;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				if (m_Result == 0)
				{
					return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((float3)(ref m_Position)).xz);
				}
				return false;
			}

			public void Iterate(QuadTreeBoundsXZ bounds, AreaSearchItem item)
			{
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_004e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_0061: Unknown result type (might be due to invalid IL or missing references)
				//IL_0079: Unknown result type (might be due to invalid IL or missing references)
				if (Intersect(bounds) && m_MapTileData.HasComponent(item.m_Area))
				{
					Triangle3 triangle = AreaUtils.GetTriangle3(m_AreaNodes[item.m_Area], m_AreaTriangles[item.m_Area][item.m_Triangle]);
					float2 val = default(float2);
					if (MathUtils.Intersect(((Triangle3)(ref triangle)).xz, ((float3)(ref m_Position)).xz, ref val))
					{
						m_Result = math.select(1, 2, m_NativeData.HasComponent(item.m_Area));
					}
				}
			}
		}

		private struct IconItem : IEquatable<IconItem>
		{
			public Entity m_Target;

			public IconFlags m_Flags;

			public IconItem(Entity target, IconFlags flags)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				m_Target = target;
				m_Flags = flags;
			}

			public bool Equals(IconItem other)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				if (m_Target == other.m_Target)
				{
					return m_Flags == other.m_Flags;
				}
				return false;
			}

			public override int GetHashCode()
			{
				int hashCode = ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Target)/*cast due to .constrained prefix*/).GetHashCode();
				byte b = (byte)m_Flags;
				return hashCode ^ b.GetHashCode();
			}
		}

		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public NativeArray<Entity> m_Owners;

		[ReadOnly]
		public WaterPipeParameterData m_WaterPipeParameterData;

		[ReadOnly]
		public ElectricityParameterData m_ElectricityParameterData;

		[ReadOnly]
		public BuildingConfigurationData m_BuildingConfigurationData;

		[ReadOnly]
		public TrafficConfigurationData m_TrafficConfigurationData;

		[ReadOnly]
		public NativeQuadTree<AreaSearchItem, QuadTreeBoundsXZ> m_AreaSearchTree;

		public IconCommandBuffer m_IconCommandBuffer;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Node> m_NodeData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<EdgeLane> m_EdgeLaneData;

		[ReadOnly]
		public ComponentLookup<SlaveLane> m_SlaveLaneData;

		[ReadOnly]
		public ComponentLookup<CarLane> m_CarLaneData;

		[ReadOnly]
		public ComponentLookup<TrackLane> m_TrackLaneData;

		[ReadOnly]
		public ComponentLookup<PedestrianLane> m_PedestrianLaneData;

		[ReadOnly]
		public ComponentLookup<ConnectionLane> m_ConnectionLaneData;

		[ReadOnly]
		public ComponentLookup<LaneConnection> m_LaneConnectionData;

		[ReadOnly]
		public ComponentLookup<OutsideConnection> m_OutsideConnectionData;

		[ReadOnly]
		public ComponentLookup<ResourceConnection> m_ResourceConnectionData;

		[ReadOnly]
		public ComponentLookup<Target> m_TargetData;

		[ReadOnly]
		public ComponentLookup<Native> m_NativeData;

		[ReadOnly]
		public ComponentLookup<Destroyed> m_DestroyedData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> m_SpawnLocationData;

		[ReadOnly]
		public ComponentLookup<Game.Routes.TakeoffLocation> m_TakeoffLocationData;

		[ReadOnly]
		public ComponentLookup<AccessLane> m_AccessLaneData;

		[ReadOnly]
		public ComponentLookup<RouteLane> m_RouteLaneData;

		[ReadOnly]
		public ComponentLookup<ElectricityProducer> m_ElectricityProducerData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Transformer> m_TransformerData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.ServiceUpgrade> m_ServiceUpgradeData;

		[ReadOnly]
		public ComponentLookup<MapTile> m_MapTileData;

		[ReadOnly]
		public ComponentLookup<Icon> m_IconData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NetData> m_PrefabNetData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_PrefabBuildingData;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> m_PrefabSpawnLocationData;

		[ReadOnly]
		public ComponentLookup<RouteConnectionData> m_PrefabRouteConnectionData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabNetGeometryData;

		[ReadOnly]
		public ComponentLookup<LocalConnectData> m_PrefabLocalConnectData;

		[ReadOnly]
		public ComponentLookup<CarLaneData> m_PrefabCarLaneData;

		[ReadOnly]
		public ComponentLookup<TrackLaneData> m_PrefabTrackLaneData;

		[ReadOnly]
		public ComponentLookup<ResourceConnectionData> m_PrefabResourceConnectionData;

		[ReadOnly]
		public BufferLookup<SubNet> m_SubNets;

		[ReadOnly]
		public BufferLookup<SubLane> m_SubLanes;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public BufferLookup<ConnectedNode> m_ConnectedNodes;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> m_SubAreas;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> m_AreaNodes;

		[ReadOnly]
		public BufferLookup<Triangle> m_AreaTriangles;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		[ReadOnly]
		public BufferLookup<IconElement> m_IconElements;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			Entity val = m_Owners[index];
			if (m_ConnectedEdges.HasBuffer(val))
			{
				UpdateNodeConnectionWarnings(val, IsNativeMapTile(m_NodeData[val].m_Position));
			}
			else
			{
				if (!m_BuildingData.HasComponent(val))
				{
					return;
				}
				Building building = m_BuildingData[val];
				PrefabRef prefabRef = m_PrefabRefData[val];
				BuildingData buildingData = m_PrefabBuildingData[prefabRef.m_Prefab];
				bool isSubBuilding = false;
				bool flag = !m_DestroyedData.HasComponent(val);
				Owner owner = default(Owner);
				if (m_OwnerData.TryGetComponent(val, ref owner))
				{
					isSubBuilding = true;
					PrefabRef prefabRef2 = default(PrefabRef);
					BuildingData buildingData2 = default(BuildingData);
					flag &= (buildingData.m_Flags & Game.Prefabs.BuildingFlags.RequireRoad) != 0 || (m_PrefabRefData.TryGetComponent(owner.m_Owner, ref prefabRef2) && m_PrefabBuildingData.TryGetComponent(prefabRef2.m_Prefab, ref buildingData2) && (buildingData2.m_Flags & Game.Prefabs.BuildingFlags.RequireRoad) != 0);
				}
				else
				{
					UpdateSubnetConnectionWarnings(val);
					flag &= m_PrefabRefData.HasComponent(building.m_RoadEdge) && (buildingData.m_Flags & Game.Prefabs.BuildingFlags.RequireRoad) != 0;
				}
				if (flag)
				{
					Transform transform = m_TransformData[val];
					if (IsNativeMapTile(transform.m_Position))
					{
						ClearPathfindIslandWarnings(val);
						return;
					}
					float2 val2 = float2.op_Implicit(buildingData.m_LotSize);
					Bounds3 bounds = default(Bounds3);
					((float3)(ref bounds.min)).xz = val2 * -4f;
					((float3)(ref bounds.max)).xz = val2 * 4f;
					Quad3 lot = ObjectUtils.CalculateBaseCorners(transform.m_Position, transform.m_Rotation, bounds);
					UpdatePathfindIslandWarnings(val, lot, isSubBuilding);
				}
				else
				{
					ClearPathfindIslandWarnings(val);
				}
			}
		}

		private bool IsNativeMapTile(float3 position)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			MapTileIterator mapTileIterator = new MapTileIterator
			{
				m_Position = position,
				m_NativeData = m_NativeData,
				m_MapTileData = m_MapTileData,
				m_AreaNodes = m_AreaNodes,
				m_AreaTriangles = m_AreaTriangles
			};
			m_AreaSearchTree.Iterate<MapTileIterator>(ref mapTileIterator, 0);
			m_NativeData = mapTileIterator.m_NativeData;
			m_MapTileData = mapTileIterator.m_MapTileData;
			m_AreaNodes = mapTileIterator.m_AreaNodes;
			m_AreaTriangles = mapTileIterator.m_AreaTriangles;
			return mapTileIterator.m_Result == 2;
		}

		private void ClearPathfindIslandWarnings(Entity owner)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			if (!m_IconElements.HasBuffer(owner))
			{
				return;
			}
			DynamicBuffer<IconElement> val = m_IconElements[owner];
			Target target = default(Target);
			for (int i = 0; i < val.Length; i++)
			{
				Entity icon = val[i].m_Icon;
				Entity prefab = m_PrefabRefData[icon].m_Prefab;
				if ((prefab == m_TrafficConfigurationData.m_CarConnectionNotification || prefab == m_TrafficConfigurationData.m_ShipConnectionNotification || prefab == m_TrafficConfigurationData.m_PedestrianConnectionNotification || prefab == m_TrafficConfigurationData.m_TrainConnectionNotification || prefab == m_TrafficConfigurationData.m_RoadConnectionNotification) && m_TargetData.TryGetComponent(icon, ref target))
				{
					IconFlags flags = m_IconData[icon].m_Flags & IconFlags.SecondaryLocation;
					m_IconCommandBuffer.Remove(owner, prefab, target.m_Target, flags);
				}
			}
		}

		private void UpdatePathfindIslandWarnings(Entity owner, Quad3 lot, bool isSubBuilding)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			NativeList<PathfindElement> ownedElements = default(NativeList<PathfindElement>);
			ownedElements._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
			NativeParallelMultiHashMap<PathNode, int> nodeMap = default(NativeParallelMultiHashMap<PathNode, int>);
			nodeMap._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
			NativeParallelHashSet<PathNode> externalNodes = default(NativeParallelHashSet<PathNode>);
			externalNodes._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
			AddPathfindElements(ownedElements, nodeMap, externalNodes, owner, owner, lot, isRoad: false, isSubBuilding);
			Owner owner2 = default(Owner);
			DynamicBuffer<SubLane> subLanes = default(DynamicBuffer<SubLane>);
			if (isSubBuilding && m_OwnerData.TryGetComponent(owner, ref owner2) && m_SubLanes.TryGetBuffer(owner2.m_Owner, ref subLanes))
			{
				AddPathfindElements(ownedElements, nodeMap, subLanes, pedestrianIcon: true, isRoad: false, onlyExisting: true, bool2.op_Implicit(false), float2.op_Implicit(-1f));
			}
			CheckConnectedElements(ownedElements, nodeMap, externalNodes, out var canIgnore);
			UpdatePathfindWarnings(ownedElements, owner, canIgnore);
			ownedElements.Dispose();
			nodeMap.Dispose();
			externalNodes.Dispose();
		}

		private void UpdatePathfindWarnings(NativeList<PathfindElement> ownedElements, Entity owner, bool canIgnore)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_042c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03de: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_043d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0474: Unknown result type (might be due to invalid IL or missing references)
			//IL_0475: Unknown result type (might be due to invalid IL or missing references)
			//IL_0479: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0406: Unknown result type (might be due to invalid IL or missing references)
			//IL_0412: Unknown result type (might be due to invalid IL or missing references)
			//IL_041a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			NativeHashSet<IconItem> val = default(NativeHashSet<IconItem>);
			if (!canIgnore)
			{
				bool flag = false;
				for (int i = 0; i < ownedElements.Length; i++)
				{
					PathfindElement pathfindElement = ownedElements[i];
					if (!math.all(pathfindElement.m_Connected) && pathfindElement.m_Priority >= 0 && !pathfindElement.m_Optional && (pathfindElement.m_Priority != 0 || pathfindElement.m_SubConnection == 0) && pathfindElement.m_IconType == 4)
					{
						flag = true;
						break;
					}
				}
				EdgeLane edgeLane = default(EdgeLane);
				Owner owner2 = default(Owner);
				for (int j = 0; j < ownedElements.Length; j++)
				{
					PathfindElement pathfindElement2 = ownedElements[j];
					if (math.all(pathfindElement2.m_Connected) || pathfindElement2.m_Priority < 0 || pathfindElement2.m_Optional || (pathfindElement2.m_Priority == 0 && pathfindElement2.m_SubConnection != 0))
					{
						continue;
					}
					Entity val2 = (Entity)(pathfindElement2.m_IconType switch
					{
						1 => m_TrafficConfigurationData.m_CarConnectionNotification, 
						2 => m_TrafficConfigurationData.m_PedestrianConnectionNotification, 
						3 => m_TrafficConfigurationData.m_TrainConnectionNotification, 
						4 => m_TrafficConfigurationData.m_RoadConnectionNotification, 
						5 => m_TrafficConfigurationData.m_ShipConnectionNotification, 
						_ => Entity.Null, 
					});
					if (!(val2 != Entity.Null))
					{
						continue;
					}
					if (!val.IsCreated)
					{
						val._002Ector(10, AllocatorHandle.op_Implicit((Allocator)2));
					}
					if (pathfindElement2.m_IconType == 4)
					{
						if (m_EdgeLaneData.TryGetComponent(pathfindElement2.m_Entity, ref edgeLane))
						{
							float num = math.lerp(edgeLane.m_EdgeDelta.x, edgeLane.m_EdgeDelta.y, (float)(int)pathfindElement2.m_IconLocation * 0.003921569f);
							float num2 = math.lerp(edgeLane.m_EdgeDelta.x, edgeLane.m_EdgeDelta.y, (float)(int)pathfindElement2.m_IconLocation2 * 0.003921569f);
							pathfindElement2.m_IconLocation = (byte)math.clamp(Mathf.RoundToInt(num * 255f), 0, 255);
							pathfindElement2.m_IconLocation2 = (byte)math.clamp(Mathf.RoundToInt(num2 * 255f), 0, 255);
						}
						if (m_OwnerData.TryGetComponent(pathfindElement2.m_Entity, ref owner2))
						{
							pathfindElement2.m_Entity = owner2.m_Owner;
						}
					}
					else if (flag)
					{
						continue;
					}
					if (!val.Add(new IconItem(pathfindElement2.m_Entity, (IconFlags)0)))
					{
						continue;
					}
					if (m_CurveData.HasComponent(pathfindElement2.m_Entity))
					{
						float3 location = MathUtils.Position(m_CurveData[pathfindElement2.m_Entity].m_Bezier, (float)(int)pathfindElement2.m_IconLocation * 0.003921569f);
						m_IconCommandBuffer.Add(owner, val2, location, IconPriority.Warning, IconClusterLayer.Default, IconFlags.TargetLocation, pathfindElement2.m_Entity);
						if (pathfindElement2.m_IconLocation2 != pathfindElement2.m_IconLocation && val.Add(new IconItem(pathfindElement2.m_Entity, IconFlags.SecondaryLocation)))
						{
							location = MathUtils.Position(m_CurveData[pathfindElement2.m_Entity].m_Bezier, (float)(int)pathfindElement2.m_IconLocation2 * 0.003921569f);
							m_IconCommandBuffer.Add(owner, val2, location, IconPriority.Warning, IconClusterLayer.Default, IconFlags.TargetLocation | IconFlags.SecondaryLocation, pathfindElement2.m_Entity);
						}
					}
					else
					{
						m_IconCommandBuffer.Add(owner, val2, IconPriority.Warning, IconClusterLayer.Default, IconFlags.TargetLocation, pathfindElement2.m_Entity);
					}
				}
			}
			if (m_IconElements.HasBuffer(owner))
			{
				DynamicBuffer<IconElement> val3 = m_IconElements[owner];
				Target target = default(Target);
				for (int k = 0; k < val3.Length; k++)
				{
					Entity icon = val3[k].m_Icon;
					Entity prefab = m_PrefabRefData[icon].m_Prefab;
					if ((prefab == m_TrafficConfigurationData.m_CarConnectionNotification || prefab == m_TrafficConfigurationData.m_ShipConnectionNotification || prefab == m_TrafficConfigurationData.m_PedestrianConnectionNotification || prefab == m_TrafficConfigurationData.m_TrainConnectionNotification || prefab == m_TrafficConfigurationData.m_RoadConnectionNotification) && m_TargetData.TryGetComponent(icon, ref target))
					{
						IconFlags flags = m_IconData[icon].m_Flags & IconFlags.SecondaryLocation;
						if (!val.IsCreated || !val.Contains(new IconItem(target.m_Target, flags)))
						{
							m_IconCommandBuffer.Remove(owner, prefab, target.m_Target, flags);
						}
					}
				}
			}
			if (val.IsCreated)
			{
				val.Dispose();
			}
		}

		private void CheckConnectedElements(NativeList<PathfindElement> ownedElements, NativeParallelMultiHashMap<PathNode, int> nodeMap, NativeParallelHashSet<PathNode> externalNodes, out bool canIgnore)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_054b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0610: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0466: Unknown result type (might be due to invalid IL or missing references)
			//IL_0472: Unknown result type (might be due to invalid IL or missing references)
			//IL_0477: Unknown result type (might be due to invalid IL or missing references)
			//IL_047d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0482: Unknown result type (might be due to invalid IL or missing references)
			//IL_0487: Unknown result type (might be due to invalid IL or missing references)
			//IL_048c: Unknown result type (might be due to invalid IL or missing references)
			//IL_048e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_0398: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_049f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0400: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0428: Unknown result type (might be due to invalid IL or missing references)
			//IL_0431: Unknown result type (might be due to invalid IL or missing references)
			//IL_0436: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Unknown result type (might be due to invalid IL or missing references)
			NativeList<BufferElement> val = default(NativeList<BufferElement>);
			val._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
			canIgnore = true;
			int num = default(int);
			NativeParallelMultiHashMapIterator<PathNode> val2 = default(NativeParallelMultiHashMapIterator<PathNode>);
			for (int i = 0; i < ownedElements.Length; i++)
			{
				PathfindElement pathfindElement = ownedElements[i];
				canIgnore &= pathfindElement.m_CanIgnore;
				if (math.all(pathfindElement.m_Connected))
				{
					continue;
				}
				if (math.all(pathfindElement.m_Directions))
				{
					if (externalNodes.Contains(pathfindElement.m_StartNode.StripCurvePos()))
					{
						BufferElement bufferElement = new BufferElement
						{
							m_Node = pathfindElement.m_StartNode,
							m_Connected = bool2.op_Implicit(true)
						};
						val.Add(ref bufferElement);
					}
					if (externalNodes.Contains(pathfindElement.m_EndNode.StripCurvePos()))
					{
						BufferElement bufferElement = new BufferElement
						{
							m_Node = pathfindElement.m_EndNode,
							m_Connected = bool2.op_Implicit(true)
						};
						val.Add(ref bufferElement);
					}
				}
				else if (pathfindElement.m_Directions.x)
				{
					if (!pathfindElement.m_Connected.x && externalNodes.Contains(pathfindElement.m_StartNode.StripCurvePos()))
					{
						BufferElement bufferElement = new BufferElement
						{
							m_Node = pathfindElement.m_StartNode,
							m_Connected = new bool2(true, false)
						};
						val.Add(ref bufferElement);
					}
					if (!pathfindElement.m_Connected.y && externalNodes.Contains(pathfindElement.m_EndNode.StripCurvePos()))
					{
						BufferElement bufferElement = new BufferElement
						{
							m_Node = pathfindElement.m_EndNode,
							m_Connected = new bool2(false, true)
						};
						val.Add(ref bufferElement);
					}
				}
				else if (pathfindElement.m_Directions.y)
				{
					if (!pathfindElement.m_Connected.x && externalNodes.Contains(pathfindElement.m_EndNode.StripCurvePos()))
					{
						BufferElement bufferElement = new BufferElement
						{
							m_Node = pathfindElement.m_EndNode,
							m_Connected = new bool2(true, false)
						};
						val.Add(ref bufferElement);
					}
					if (!pathfindElement.m_Connected.y && externalNodes.Contains(pathfindElement.m_StartNode.StripCurvePos()))
					{
						BufferElement bufferElement = new BufferElement
						{
							m_Node = pathfindElement.m_StartNode,
							m_Connected = new bool2(false, true)
						};
						val.Add(ref bufferElement);
					}
				}
				while (val.Length > 0)
				{
					BufferElement bufferElement2 = val[val.Length - 1];
					val.RemoveAt(val.Length - 1);
					if (!nodeMap.TryGetFirstValue(bufferElement2.m_Node.StripCurvePos(), ref num, ref val2))
					{
						continue;
					}
					do
					{
						pathfindElement = ownedElements[num];
						if (pathfindElement.m_StartNode.EqualsIgnoreCurvePos(bufferElement2.m_Node))
						{
							bool2 val3 = bufferElement2.m_Connected & pathfindElement.m_Directions & !pathfindElement.m_Connected;
							if (math.any(val3))
							{
								ref bool2 reference = ref pathfindElement.m_Connected;
								reference |= val3;
								ownedElements[num] = pathfindElement;
								BufferElement bufferElement = new BufferElement
								{
									m_Node = pathfindElement.m_MiddleNode,
									m_Connected = val3
								};
								val.Add(ref bufferElement);
								bufferElement = new BufferElement
								{
									m_Node = pathfindElement.m_EndNode,
									m_Connected = val3
								};
								val.Add(ref bufferElement);
							}
						}
						else if (pathfindElement.m_MiddleNode.EqualsIgnoreCurvePos(bufferElement2.m_Node))
						{
							bool2 val4 = bufferElement2.m_Connected & !pathfindElement.m_Connected;
							if (math.any(val4))
							{
								ref bool2 reference2 = ref pathfindElement.m_Connected;
								reference2 |= val4;
								ownedElements[num] = pathfindElement;
								if (math.any(val4 & pathfindElement.m_Directions))
								{
									BufferElement bufferElement = new BufferElement
									{
										m_Node = pathfindElement.m_EndNode,
										m_Connected = (val4 & pathfindElement.m_Directions)
									};
									val.Add(ref bufferElement);
								}
								if (math.any(val4 & ((bool2)(ref pathfindElement.m_Directions)).yx))
								{
									BufferElement bufferElement = new BufferElement
									{
										m_Node = pathfindElement.m_StartNode,
										m_Connected = (val4 & ((bool2)(ref pathfindElement.m_Directions)).yx)
									};
									val.Add(ref bufferElement);
								}
							}
						}
						else if (pathfindElement.m_EndNode.EqualsIgnoreCurvePos(bufferElement2.m_Node))
						{
							bool2 val5 = bufferElement2.m_Connected & ((bool2)(ref pathfindElement.m_Directions)).yx & !pathfindElement.m_Connected;
							if (math.any(val5))
							{
								ref bool2 reference3 = ref pathfindElement.m_Connected;
								reference3 |= val5;
								ownedElements[num] = pathfindElement;
								BufferElement bufferElement = new BufferElement
								{
									m_Node = pathfindElement.m_MiddleNode,
									m_Connected = val5
								};
								val.Add(ref bufferElement);
								bufferElement = new BufferElement
								{
									m_Node = pathfindElement.m_StartNode,
									m_Connected = val5
								};
								val.Add(ref bufferElement);
							}
						}
					}
					while (nodeMap.TryGetNextValue(ref num, ref val2));
				}
			}
			int num2 = default(int);
			NativeParallelMultiHashMapIterator<PathNode> val6 = default(NativeParallelMultiHashMapIterator<PathNode>);
			int num3 = default(int);
			NativeParallelMultiHashMapIterator<PathNode> val7 = default(NativeParallelMultiHashMapIterator<PathNode>);
			int num4 = default(int);
			NativeParallelMultiHashMapIterator<PathNode> val8 = default(NativeParallelMultiHashMapIterator<PathNode>);
			for (int j = 0; j < ownedElements.Length; j++)
			{
				PathfindElement pathfindElement2 = ownedElements[j];
				if (math.all(pathfindElement2.m_Connected))
				{
					continue;
				}
				if (pathfindElement2.m_Priority > 0)
				{
					bool flag = pathfindElement2.m_Optional;
					bool flag2 = false;
					BufferElement bufferElement = new BufferElement
					{
						m_Node = pathfindElement2.m_StartNode
					};
					val.Add(ref bufferElement);
					bufferElement = new BufferElement
					{
						m_Node = pathfindElement2.m_EndNode
					};
					val.Add(ref bufferElement);
					while (val.Length > 0)
					{
						BufferElement bufferElement3 = val[val.Length - 1];
						val.RemoveAt(val.Length - 1);
						if (!nodeMap.TryGetFirstValue(bufferElement3.m_Node.StripCurvePos(), ref num2, ref val6))
						{
							continue;
						}
						do
						{
							pathfindElement2 = ownedElements[num2];
							flag2 |= flag & !pathfindElement2.m_Optional & !math.all(pathfindElement2.m_Connected);
							if (pathfindElement2.m_Priority == 0)
							{
								pathfindElement2.m_Priority = -1;
								ownedElements[num2] = pathfindElement2;
								if (pathfindElement2.m_StartNode.EqualsIgnoreCurvePos(bufferElement3.m_Node))
								{
									bufferElement = new BufferElement
									{
										m_Node = pathfindElement2.m_MiddleNode
									};
									val.Add(ref bufferElement);
									bufferElement = new BufferElement
									{
										m_Node = pathfindElement2.m_EndNode
									};
									val.Add(ref bufferElement);
								}
								else if (pathfindElement2.m_MiddleNode.EqualsIgnoreCurvePos(bufferElement3.m_Node))
								{
									bufferElement = new BufferElement
									{
										m_Node = pathfindElement2.m_StartNode
									};
									val.Add(ref bufferElement);
									bufferElement = new BufferElement
									{
										m_Node = pathfindElement2.m_EndNode
									};
									val.Add(ref bufferElement);
								}
								else if (pathfindElement2.m_EndNode.EqualsIgnoreCurvePos(bufferElement3.m_Node))
								{
									bufferElement = new BufferElement
									{
										m_Node = pathfindElement2.m_MiddleNode
									};
									val.Add(ref bufferElement);
									bufferElement = new BufferElement
									{
										m_Node = pathfindElement2.m_StartNode
									};
									val.Add(ref bufferElement);
								}
							}
						}
						while (nodeMap.TryGetNextValue(ref num2, ref val6));
					}
					if (!flag2)
					{
						continue;
					}
					pathfindElement2 = ownedElements[j];
					pathfindElement2.m_Optional = false;
					ownedElements[j] = pathfindElement2;
					bufferElement = new BufferElement
					{
						m_Node = pathfindElement2.m_StartNode
					};
					val.Add(ref bufferElement);
					bufferElement = new BufferElement
					{
						m_Node = pathfindElement2.m_EndNode
					};
					val.Add(ref bufferElement);
					while (val.Length > 0)
					{
						BufferElement bufferElement4 = val[val.Length - 1];
						val.RemoveAt(val.Length - 1);
						if (!nodeMap.TryGetFirstValue(bufferElement4.m_Node.StripCurvePos(), ref num3, ref val7))
						{
							continue;
						}
						do
						{
							pathfindElement2 = ownedElements[num3];
							if (pathfindElement2.m_Optional)
							{
								pathfindElement2.m_Optional = false;
								ownedElements[num3] = pathfindElement2;
								if (pathfindElement2.m_StartNode.EqualsIgnoreCurvePos(bufferElement4.m_Node))
								{
									bufferElement = new BufferElement
									{
										m_Node = pathfindElement2.m_MiddleNode
									};
									val.Add(ref bufferElement);
									bufferElement = new BufferElement
									{
										m_Node = pathfindElement2.m_EndNode
									};
									val.Add(ref bufferElement);
								}
								else if (pathfindElement2.m_MiddleNode.EqualsIgnoreCurvePos(bufferElement4.m_Node))
								{
									bufferElement = new BufferElement
									{
										m_Node = pathfindElement2.m_StartNode
									};
									val.Add(ref bufferElement);
									bufferElement = new BufferElement
									{
										m_Node = pathfindElement2.m_EndNode
									};
									val.Add(ref bufferElement);
								}
								else if (pathfindElement2.m_EndNode.EqualsIgnoreCurvePos(bufferElement4.m_Node))
								{
									bufferElement = new BufferElement
									{
										m_Node = pathfindElement2.m_MiddleNode
									};
									val.Add(ref bufferElement);
									bufferElement = new BufferElement
									{
										m_Node = pathfindElement2.m_StartNode
									};
									val.Add(ref bufferElement);
								}
							}
						}
						while (nodeMap.TryGetNextValue(ref num3, ref val7));
					}
				}
				else
				{
					if (pathfindElement2.m_SubConnection <= 0)
					{
						continue;
					}
					BufferElement bufferElement = new BufferElement
					{
						m_Node = pathfindElement2.m_StartNode
					};
					val.Add(ref bufferElement);
					bufferElement = new BufferElement
					{
						m_Node = pathfindElement2.m_EndNode
					};
					val.Add(ref bufferElement);
					while (val.Length > 0)
					{
						BufferElement bufferElement5 = val[val.Length - 1];
						val.RemoveAt(val.Length - 1);
						if (!nodeMap.TryGetFirstValue(bufferElement5.m_Node.StripCurvePos(), ref num4, ref val8))
						{
							continue;
						}
						do
						{
							pathfindElement2 = ownedElements[num4];
							if (pathfindElement2.m_SubConnection == 0)
							{
								pathfindElement2.m_SubConnection = -1;
								ownedElements[num4] = pathfindElement2;
								if (pathfindElement2.m_StartNode.EqualsIgnoreCurvePos(bufferElement5.m_Node))
								{
									bufferElement = new BufferElement
									{
										m_Node = pathfindElement2.m_MiddleNode
									};
									val.Add(ref bufferElement);
									bufferElement = new BufferElement
									{
										m_Node = pathfindElement2.m_EndNode
									};
									val.Add(ref bufferElement);
								}
								else if (pathfindElement2.m_MiddleNode.EqualsIgnoreCurvePos(bufferElement5.m_Node))
								{
									bufferElement = new BufferElement
									{
										m_Node = pathfindElement2.m_StartNode
									};
									val.Add(ref bufferElement);
									bufferElement = new BufferElement
									{
										m_Node = pathfindElement2.m_EndNode
									};
									val.Add(ref bufferElement);
								}
								else if (pathfindElement2.m_EndNode.EqualsIgnoreCurvePos(bufferElement5.m_Node))
								{
									bufferElement = new BufferElement
									{
										m_Node = pathfindElement2.m_MiddleNode
									};
									val.Add(ref bufferElement);
									bufferElement = new BufferElement
									{
										m_Node = pathfindElement2.m_StartNode
									};
									val.Add(ref bufferElement);
								}
							}
						}
						while (nodeMap.TryGetNextValue(ref num4, ref val8));
					}
				}
			}
			val.Dispose();
		}

		private bool IsDeadEnd(Entity edge, Entity node, Entity topOwner, Entity owner, out bool maybe)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			maybe = false;
			DynamicBuffer<ConnectedEdge> val = default(DynamicBuffer<ConnectedEdge>);
			if (m_ConnectedEdges.TryGetBuffer(node, ref val))
			{
				if (val.Length == 1)
				{
					return true;
				}
				Owner owner2 = default(Owner);
				for (int i = 0; i < val.Length; i++)
				{
					Entity edge2 = val[i].m_Edge;
					if (!(edge2 == edge) && m_OwnerData.TryGetComponent(edge2, ref owner2))
					{
						Entity owner3 = owner2.m_Owner;
						if (owner3 == owner)
						{
							return false;
						}
						while (m_OwnerData.TryGetComponent(owner3, ref owner2))
						{
							owner3 = owner2.m_Owner;
						}
						if (owner3 == topOwner)
						{
							return false;
						}
					}
				}
				maybe = true;
				return true;
			}
			return false;
		}

		private void AddPathfindElements(NativeList<PathfindElement> ownedElements, NativeParallelMultiHashMap<PathNode, int> nodeMap, NativeParallelHashSet<PathNode> externalNodes, Entity topOwner, Entity owner, Quad3 lot, bool isRoad, bool isSubBuilding)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0402: Unknown result type (might be due to invalid IL or missing references)
			//IL_042b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_038a: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0465: Unknown result type (might be due to invalid IL or missing references)
			//IL_043f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0601: Unknown result type (might be due to invalid IL or missing references)
			//IL_0609: Unknown result type (might be due to invalid IL or missing references)
			//IL_0497: Unknown result type (might be due to invalid IL or missing references)
			//IL_0479: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0616: Unknown result type (might be due to invalid IL or missing references)
			//IL_0617: Unknown result type (might be due to invalid IL or missing references)
			//IL_0618: Unknown result type (might be due to invalid IL or missing references)
			//IL_0619: Unknown result type (might be due to invalid IL or missing references)
			//IL_061b: Unknown result type (might be due to invalid IL or missing references)
			//IL_061d: Unknown result type (might be due to invalid IL or missing references)
			//IL_062d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_063f: Unknown result type (might be due to invalid IL or missing references)
			//IL_064e: Unknown result type (might be due to invalid IL or missing references)
			//IL_065f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0675: Unknown result type (might be due to invalid IL or missing references)
			//IL_0677: Unknown result type (might be due to invalid IL or missing references)
			//IL_067f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0684: Unknown result type (might be due to invalid IL or missing references)
			//IL_068f: Unknown result type (might be due to invalid IL or missing references)
			//IL_069e: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0530: Unknown result type (might be due to invalid IL or missing references)
			//IL_0534: Unknown result type (might be due to invalid IL or missing references)
			//IL_0539: Unknown result type (might be due to invalid IL or missing references)
			//IL_053b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_080a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0819: Unknown result type (might be due to invalid IL or missing references)
			//IL_0828: Unknown result type (might be due to invalid IL or missing references)
			//IL_0839: Unknown result type (might be due to invalid IL or missing references)
			//IL_084d: Unknown result type (might be due to invalid IL or missing references)
			//IL_084f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0857: Unknown result type (might be due to invalid IL or missing references)
			//IL_085c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0867: Unknown result type (might be due to invalid IL or missing references)
			//IL_0876: Unknown result type (might be due to invalid IL or missing references)
			//IL_0885: Unknown result type (might be due to invalid IL or missing references)
			//IL_0553: Unknown result type (might be due to invalid IL or missing references)
			//IL_0561: Unknown result type (might be due to invalid IL or missing references)
			//IL_056d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0503: Unknown result type (might be due to invalid IL or missing references)
			//IL_0507: Unknown result type (might be due to invalid IL or missing references)
			//IL_0513: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_072e: Unknown result type (might be due to invalid IL or missing references)
			//IL_057b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0580: Unknown result type (might be due to invalid IL or missing references)
			//IL_0584: Unknown result type (might be due to invalid IL or missing references)
			//IL_0590: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_08fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_073b: Unknown result type (might be due to invalid IL or missing references)
			//IL_073c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0740: Unknown result type (might be due to invalid IL or missing references)
			//IL_0752: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0911: Unknown result type (might be due to invalid IL or missing references)
			//IL_0912: Unknown result type (might be due to invalid IL or missing references)
			//IL_0916: Unknown result type (might be due to invalid IL or missing references)
			//IL_0921: Unknown result type (might be due to invalid IL or missing references)
			//IL_0922: Unknown result type (might be due to invalid IL or missing references)
			//IL_0926: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_093b: Unknown result type (might be due to invalid IL or missing references)
			//IL_096d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_099f: Unknown result type (might be due to invalid IL or missing references)
			//IL_09aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_09cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d9: Unknown result type (might be due to invalid IL or missing references)
			if (m_SubNets.HasBuffer(owner))
			{
				DynamicBuffer<SubNet> val = m_SubNets[owner];
				DynamicBuffer<ConnectedEdge> val2 = default(DynamicBuffer<ConnectedEdge>);
				Edge edge2 = default(Edge);
				DynamicBuffer<ConnectedNode> val7 = default(DynamicBuffer<ConnectedNode>);
				for (int i = 0; i < val.Length; i++)
				{
					Entity subNet = val[i].m_SubNet;
					PrefabRef prefabRef = m_PrefabRefData[subNet];
					bool isRoad2 = (m_PrefabNetData[prefabRef.m_Prefab].m_RequiredLayers & Layer.Road) != 0;
					AddPathfindElements(ownedElements, nodeMap, externalNodes, topOwner, subNet, lot, isRoad2, isSubBuilding);
					if (m_ConnectedEdges.TryGetBuffer(subNet, ref val2))
					{
						for (int j = 0; j < val2.Length; j++)
						{
							ConnectedEdge connectedEdge = val2[j];
							if (!AddExternalNodes(externalNodes, topOwner, owner, connectedEdge.m_Edge))
							{
								continue;
							}
							Edge edge = m_EdgeData[connectedEdge.m_Edge];
							DynamicBuffer<ConnectedNode> val3 = m_ConnectedNodes[connectedEdge.m_Edge];
							if (AddExternalNodes(externalNodes, topOwner, owner, edge.m_Start))
							{
								DynamicBuffer<ConnectedEdge> val4 = m_ConnectedEdges[edge.m_Start];
								for (int k = 0; k < val4.Length; k++)
								{
									ConnectedEdge connectedEdge2 = val4[k];
									if (connectedEdge2.m_Edge != connectedEdge.m_Edge)
									{
										AddExternalNodes(externalNodes, topOwner, owner, connectedEdge2.m_Edge);
									}
								}
							}
							if (AddExternalNodes(externalNodes, topOwner, owner, edge.m_End))
							{
								DynamicBuffer<ConnectedEdge> val5 = m_ConnectedEdges[edge.m_End];
								for (int l = 0; l < val5.Length; l++)
								{
									ConnectedEdge connectedEdge3 = val5[l];
									if (connectedEdge3.m_Edge != connectedEdge.m_Edge)
									{
										AddExternalNodes(externalNodes, topOwner, owner, connectedEdge3.m_Edge);
									}
								}
							}
							for (int m = 0; m < val3.Length; m++)
							{
								ConnectedNode connectedNode = val3[m];
								if (!AddExternalNodes(externalNodes, topOwner, owner, connectedNode.m_Node))
								{
									continue;
								}
								DynamicBuffer<ConnectedEdge> val6 = m_ConnectedEdges[connectedNode.m_Node];
								for (int n = 0; n < val6.Length; n++)
								{
									ConnectedEdge connectedEdge4 = val6[n];
									if (connectedEdge4.m_Edge != connectedEdge.m_Edge)
									{
										AddExternalNodes(externalNodes, topOwner, owner, connectedEdge4.m_Edge);
									}
								}
							}
						}
					}
					else
					{
						if (!m_EdgeData.TryGetComponent(subNet, ref edge2) || !m_ConnectedNodes.TryGetBuffer(subNet, ref val7))
						{
							continue;
						}
						AddExternalNodes(externalNodes, topOwner, owner, edge2.m_Start);
						AddExternalNodes(externalNodes, topOwner, owner, edge2.m_End);
						for (int num = 0; num < val7.Length; num++)
						{
							ConnectedNode connectedNode2 = val7[num];
							if (!AddExternalNodes(externalNodes, topOwner, owner, connectedNode2.m_Node))
							{
								continue;
							}
							DynamicBuffer<ConnectedEdge> val8 = m_ConnectedEdges[connectedNode2.m_Node];
							for (int num2 = 0; num2 < val8.Length; num2++)
							{
								ConnectedEdge connectedEdge5 = val8[num2];
								if (connectedEdge5.m_Edge != subNet)
								{
									AddExternalNodes(externalNodes, topOwner, owner, connectedEdge5.m_Edge);
								}
							}
						}
					}
				}
			}
			if (m_SubAreas.HasBuffer(owner))
			{
				DynamicBuffer<Game.Areas.SubArea> val9 = m_SubAreas[owner];
				for (int num3 = 0; num3 < val9.Length; num3++)
				{
					Entity area = val9[num3].m_Area;
					AddPathfindElements(ownedElements, nodeMap, externalNodes, topOwner, area, lot, isRoad: false, isSubBuilding);
				}
			}
			bool pedestrianIcon;
			bool2 maybeDeadEnd;
			float2 errorLocation;
			float num4;
			Edge edge3;
			Segment ab;
			float num5 = default(float);
			if (m_SubLanes.HasBuffer(owner))
			{
				PrefabRef prefabRef2 = m_PrefabRefData[owner];
				pedestrianIcon = true;
				maybeDeadEnd = bool2.op_Implicit(false);
				errorLocation = float2.op_Implicit(-1f);
				if (m_PrefabNetData.HasComponent(prefabRef2.m_Prefab))
				{
					pedestrianIcon = (m_PrefabNetData[prefabRef2.m_Prefab].m_RequiredLayers & (Layer.TrainTrack | Layer.Waterway | Layer.TramTrack | Layer.SubwayTrack)) == 0;
					num4 = 0f;
					if (m_PrefabNetGeometryData.HasComponent(prefabRef2.m_Prefab))
					{
						num4 += m_PrefabNetGeometryData[prefabRef2.m_Prefab].m_DefaultWidth * 0.5f;
					}
					if (m_PrefabLocalConnectData.HasComponent(prefabRef2.m_Prefab))
					{
						num4 += m_PrefabLocalConnectData[prefabRef2.m_Prefab].m_SearchDistance;
					}
					if (m_EdgeData.HasComponent(owner))
					{
						edge3 = m_EdgeData[owner];
						if (IsDeadEnd(owner, edge3.m_Start, topOwner, owner, out maybeDeadEnd.x))
						{
							Node node = m_NodeData[edge3.m_Start];
							if (MathUtils.Intersect(((Quad3)(ref lot)).xz, ((float3)(ref node.m_Position)).xz))
							{
								ab = ((Quad3)(ref lot)).ab;
								if (!(MathUtils.Distance(((Segment)(ref ab)).xz, ((float3)(ref node.m_Position)).xz, ref num5) <= num4))
								{
									goto IL_052f;
								}
							}
							errorLocation.x = 0f;
						}
						goto IL_052f;
					}
				}
				goto IL_05ac;
			}
			goto IL_05ca;
			IL_05ca:
			if (!m_SubObjects.HasBuffer(owner))
			{
				return;
			}
			DynamicBuffer<Game.Objects.SubObject> val10 = m_SubObjects[owner];
			for (int num6 = 0; num6 < val10.Length; num6++)
			{
				Entity subObject = val10[num6].m_SubObject;
				if (m_BuildingData.HasComponent(subObject))
				{
					continue;
				}
				AddPathfindElements(ownedElements, nodeMap, externalNodes, topOwner, subObject, lot, isRoad: false, isSubBuilding);
				if (m_SpawnLocationData.HasComponent(subObject))
				{
					Game.Objects.SpawnLocation spawnLocation = m_SpawnLocationData[subObject];
					PrefabRef prefabRef3 = m_PrefabRefData[subObject];
					SpawnLocationData spawnLocationData = m_PrefabSpawnLocationData[prefabRef3.m_Prefab];
					PathfindElement pathfindElement = new PathfindElement
					{
						m_Entity = subObject,
						m_Directions = bool2.op_Implicit(true)
					};
					pathfindElement.m_StartNode = new PathNode(subObject, 2);
					pathfindElement.m_MiddleNode = new PathNode(subObject, 1);
					pathfindElement.m_EndNode = new PathNode(subObject, 0);
					switch (spawnLocationData.m_ConnectionType)
					{
					case RouteConnectionType.Road:
						pathfindElement.m_IconType = 1;
						break;
					case RouteConnectionType.Pedestrian:
						pathfindElement.m_IconType = 2;
						break;
					case RouteConnectionType.Air:
						pathfindElement.m_IconType = (byte)math.select(1, 0, m_EditorMode);
						break;
					case RouteConnectionType.Parking:
						pathfindElement.m_IconType = 1;
						break;
					case RouteConnectionType.Track:
						pathfindElement.m_IconType = 3;
						break;
					default:
						continue;
					}
					if (m_LaneData.HasComponent(spawnLocation.m_ConnectedLane1))
					{
						AddExternalNode(externalNodes, topOwner, spawnLocation.m_ConnectedLane1);
						pathfindElement.m_StartNode = new PathNode(m_LaneData[spawnLocation.m_ConnectedLane1].m_MiddleNode, spawnLocation.m_CurvePosition1);
					}
					else if (spawnLocationData.m_ConnectionType == RouteConnectionType.Pedestrian)
					{
						pathfindElement.m_CanIgnore = true;
					}
					pathfindElement.m_SubConnection = (sbyte)(isSubBuilding ? 1 : 0);
					int length = ownedElements.Length;
					ownedElements.Add(ref pathfindElement);
					nodeMap.Add(pathfindElement.m_StartNode.StripCurvePos(), length);
					nodeMap.Add(pathfindElement.m_MiddleNode.StripCurvePos(), length);
					nodeMap.Add(pathfindElement.m_EndNode.StripCurvePos(), length);
					continue;
				}
				if (!m_TakeoffLocationData.HasComponent(subObject))
				{
					continue;
				}
				AccessLane accessLane = m_AccessLaneData[subObject];
				RouteLane routeLane = m_RouteLaneData[subObject];
				PrefabRef prefabRef4 = m_PrefabRefData[subObject];
				RouteConnectionData routeConnectionData = m_PrefabRouteConnectionData[prefabRef4.m_Prefab];
				PathfindElement pathfindElement2 = new PathfindElement
				{
					m_Entity = subObject,
					m_Directions = bool2.op_Implicit(true)
				};
				pathfindElement2.m_StartNode = new PathNode(subObject, 2);
				pathfindElement2.m_MiddleNode = new PathNode(subObject, 1);
				pathfindElement2.m_EndNode = new PathNode(subObject, 0);
				switch (routeConnectionData.m_AccessConnectionType)
				{
				case RouteConnectionType.Road:
					pathfindElement2.m_IconType = 1;
					break;
				case RouteConnectionType.Pedestrian:
					pathfindElement2.m_IconType = 2;
					break;
				case RouteConnectionType.Air:
					pathfindElement2.m_IconType = (byte)math.select(1, 0, m_EditorMode);
					break;
				default:
					continue;
				}
				bool num7 = m_LaneData.HasComponent(accessLane.m_Lane);
				bool flag = m_LaneData.HasComponent(routeLane.m_EndLane);
				if (num7 && flag)
				{
					AddExternalNode(externalNodes, topOwner, accessLane.m_Lane);
					AddExternalNode(externalNodes, topOwner, routeLane.m_EndLane);
				}
				if (num7)
				{
					pathfindElement2.m_StartNode = new PathNode(m_LaneData[accessLane.m_Lane].m_MiddleNode, accessLane.m_CurvePos);
				}
				if (flag)
				{
					pathfindElement2.m_EndNode = new PathNode(m_LaneData[routeLane.m_EndLane].m_MiddleNode, routeLane.m_EndCurvePos);
				}
				if (!num7 || !flag)
				{
					Transform transform = m_TransformData[subObject];
					if (MathUtils.Intersect(((Quad3)(ref lot)).xz, ((float3)(ref transform.m_Position)).xz))
					{
						ab = ((Quad3)(ref lot)).ab;
						if (!(MathUtils.Distance(((Segment)(ref ab)).xz, ((float3)(ref transform.m_Position)).xz, ref num5) <= 10f))
						{
							goto IL_09f4;
						}
					}
					pathfindElement2.m_Priority = 1;
				}
				goto IL_09f4;
				IL_09f4:
				int length2 = ownedElements.Length;
				ownedElements.Add(ref pathfindElement2);
				nodeMap.Add(pathfindElement2.m_StartNode.StripCurvePos(), length2);
				nodeMap.Add(pathfindElement2.m_MiddleNode.StripCurvePos(), length2);
				nodeMap.Add(pathfindElement2.m_EndNode.StripCurvePos(), length2);
			}
			return;
			IL_05ac:
			AddPathfindElements(ownedElements, nodeMap, m_SubLanes[owner], pedestrianIcon, isRoad, onlyExisting: false, maybeDeadEnd, errorLocation);
			goto IL_05ca;
			IL_052f:
			if (IsDeadEnd(owner, edge3.m_End, topOwner, owner, out maybeDeadEnd.y))
			{
				Node node2 = m_NodeData[edge3.m_End];
				if (MathUtils.Intersect(((Quad3)(ref lot)).xz, ((float3)(ref node2.m_Position)).xz))
				{
					ab = ((Quad3)(ref lot)).ab;
					if (!(MathUtils.Distance(((Segment)(ref ab)).xz, ((float3)(ref node2.m_Position)).xz, ref num5) <= num4))
					{
						goto IL_05ac;
					}
				}
				errorLocation.y = 1f;
			}
			goto IL_05ac;
		}

		private void AddPathfindElements(NativeList<PathfindElement> ownedElements, NativeParallelMultiHashMap<PathNode, int> nodeMap, DynamicBuffer<SubLane> subLanes, bool pedestrianIcon, bool isRoad, bool onlyExisting, bool2 maybeDeadEnd, float2 errorLocation)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = default(PrefabRef);
			CarLaneData carLaneData = default(CarLaneData);
			LaneConnection laneConnection = default(LaneConnection);
			Lane lane2 = default(Lane);
			Lane lane3 = default(Lane);
			for (int i = 0; i < subLanes.Length; i++)
			{
				Entity subLane = subLanes[i].m_SubLane;
				PathfindElement pathfindElement = new PathfindElement
				{
					m_Entity = subLane,
					m_IconLocation = 128
				};
				bool2 val = bool2.op_Implicit(false);
				if (m_CarLaneData.HasComponent(subLane))
				{
					if (m_SlaveLaneData.HasComponent(subLane))
					{
						continue;
					}
					pathfindElement.m_Directions = new bool2(true, (m_CarLaneData[subLane].m_Flags & CarLaneFlags.Twoway) != 0);
					pathfindElement.m_IconType = (byte)math.select(1, 4, isRoad);
					if (m_PrefabRefData.TryGetComponent(subLane, ref prefabRef) && m_PrefabCarLaneData.TryGetComponent(prefabRef.m_Prefab, ref carLaneData))
					{
						pathfindElement.m_IconType = (byte)math.select((int)pathfindElement.m_IconType, 5, (carLaneData.m_RoadTypes & (RoadTypes.Car | RoadTypes.Watercraft)) == RoadTypes.Watercraft);
					}
				}
				else if (m_PedestrianLaneData.HasComponent(subLane))
				{
					pathfindElement.m_Directions = bool2.op_Implicit(true);
					pathfindElement.m_IconType = (byte)math.select(2, 4, isRoad);
					pathfindElement.m_IconType = (byte)math.select(0, (int)pathfindElement.m_IconType, pedestrianIcon);
				}
				else if (m_TrackLaneData.HasComponent(subLane))
				{
					TrackLane trackLane = m_TrackLaneData[subLane];
					pathfindElement.m_Directions = new bool2(true, (trackLane.m_Flags & TrackLaneFlags.Twoway) != 0);
					pathfindElement.m_IconType = 3;
					pathfindElement.m_Optional = (trackLane.m_Flags & TrackLaneFlags.Twoway) == 0;
					val.x = (trackLane.m_Flags & TrackLaneFlags.StartingLane) != 0;
					val.y = (trackLane.m_Flags & TrackLaneFlags.EndingLane) != 0;
				}
				else
				{
					if (!m_ConnectionLaneData.HasComponent(subLane))
					{
						continue;
					}
					ConnectionLane connectionLane = m_ConnectionLaneData[subLane];
					if ((connectionLane.m_Flags & ConnectionLaneFlags.Road) != 0)
					{
						pathfindElement.m_Directions = bool2.op_Implicit(true);
						pathfindElement.m_IconType = (byte)math.select(1, 4, isRoad);
						pathfindElement.m_IconType = (byte)math.select((int)pathfindElement.m_IconType, 5, (connectionLane.m_RoadTypes & (RoadTypes.Car | RoadTypes.Watercraft)) == RoadTypes.Watercraft);
					}
					else if ((connectionLane.m_Flags & ConnectionLaneFlags.Pedestrian) != 0)
					{
						pathfindElement.m_Directions = bool2.op_Implicit(true);
						pathfindElement.m_IconType = (byte)math.select(2, 4, isRoad);
						pathfindElement.m_IconType = (byte)math.select(0, (int)pathfindElement.m_IconType, pedestrianIcon);
					}
					else
					{
						if ((connectionLane.m_Flags & ConnectionLaneFlags.Track) == 0)
						{
							continue;
						}
						pathfindElement.m_Directions = bool2.op_Implicit(true);
						pathfindElement.m_IconType = 3;
					}
				}
				if (math.any(errorLocation >= 0f) && pathfindElement.m_IconType != 0 && m_EdgeLaneData.HasComponent(subLane))
				{
					EdgeLane edgeLane = m_EdgeLaneData[subLane];
					bool2 val2 = (edgeLane.m_EdgeDelta.x == errorLocation) & (val.x | !maybeDeadEnd);
					bool2 val3 = (edgeLane.m_EdgeDelta.y == errorLocation) & (val.y | !maybeDeadEnd);
					if (math.any(val2))
					{
						pathfindElement.m_Priority = 1;
						pathfindElement.m_IconLocation = 0;
						pathfindElement.m_IconLocation2 = 0;
					}
					if (math.any(val3))
					{
						pathfindElement.m_Priority = 1;
						pathfindElement.m_IconLocation = (byte)math.select(255, 0, math.any(val2));
						pathfindElement.m_IconLocation2 = byte.MaxValue;
					}
				}
				Lane lane = m_LaneData[subLane];
				if (m_LaneConnectionData.TryGetComponent(subLane, ref laneConnection))
				{
					if (m_LaneData.TryGetComponent(laneConnection.m_StartLane, ref lane2))
					{
						lane.m_StartNode = new PathNode(lane2.m_MiddleNode, laneConnection.m_StartPosition);
					}
					if (m_LaneData.TryGetComponent(laneConnection.m_EndLane, ref lane3))
					{
						lane.m_EndNode = new PathNode(lane3.m_MiddleNode, laneConnection.m_EndPosition);
					}
				}
				if (!onlyExisting || (nodeMap.ContainsKey(lane.m_StartNode.StripCurvePos()) && nodeMap.ContainsKey(lane.m_EndNode.StripCurvePos())))
				{
					pathfindElement.m_StartNode = lane.m_StartNode;
					pathfindElement.m_MiddleNode = lane.m_MiddleNode;
					pathfindElement.m_EndNode = lane.m_EndNode;
					int length = ownedElements.Length;
					ownedElements.Add(ref pathfindElement);
					nodeMap.Add(lane.m_StartNode.StripCurvePos(), length);
					nodeMap.Add(lane.m_MiddleNode.StripCurvePos(), length);
					nodeMap.Add(lane.m_EndNode.StripCurvePos(), length);
				}
			}
		}

		private void AddExternalNode(NativeParallelHashSet<PathNode> externalNodes, Entity topOwner, Entity laneEntity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			Entity val = laneEntity;
			Owner owner = default(Owner);
			while (m_OwnerData.TryGetComponent(val, ref owner) && !m_BuildingData.HasComponent(val))
			{
				val = owner.m_Owner;
			}
			if (!(val == topOwner))
			{
				Lane lane = m_LaneData[laneEntity];
				externalNodes.Add(lane.m_StartNode.StripCurvePos());
				externalNodes.Add(lane.m_MiddleNode.StripCurvePos());
				externalNodes.Add(lane.m_EndNode.StripCurvePos());
			}
		}

		private bool AddExternalNodes(NativeParallelHashSet<PathNode> externalNodes, Entity topOwner, Entity owner, Entity netEntity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			Owner owner2 = default(Owner);
			if (m_OwnerData.TryGetComponent(netEntity, ref owner2))
			{
				Entity owner3 = owner2.m_Owner;
				if (owner3 == owner)
				{
					return false;
				}
				while (m_OwnerData.TryGetComponent(owner3, ref owner2) && !m_BuildingData.HasComponent(owner3))
				{
					owner3 = owner2.m_Owner;
				}
				if (owner3 == topOwner)
				{
					return false;
				}
			}
			if (m_SubLanes.HasBuffer(netEntity))
			{
				DynamicBuffer<SubLane> val = m_SubLanes[netEntity];
				for (int i = 0; i < val.Length; i++)
				{
					Entity subLane = val[i].m_SubLane;
					Lane lane = m_LaneData[subLane];
					externalNodes.Add(lane.m_StartNode.StripCurvePos());
					externalNodes.Add(lane.m_MiddleNode.StripCurvePos());
					externalNodes.Add(lane.m_EndNode.StripCurvePos());
				}
			}
			return true;
		}

		private void UpdateSubnetConnectionWarnings(Entity owner)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			NativeHashSet<Entity> resourceConnections = default(NativeHashSet<Entity>);
			Layer connectedLayers = Layer.None;
			Layer disconnectedLayers = Layer.None;
			DynamicBuffer<SubNet> subNets = default(DynamicBuffer<SubNet>);
			if (m_SubNets.TryGetBuffer(owner, ref subNets))
			{
				CheckConnectedLayers(ref connectedLayers, ref disconnectedLayers, ref resourceConnections, owner, subNets, subBuilding: false);
			}
			DynamicBuffer<Game.Objects.SubObject> subObjects = default(DynamicBuffer<Game.Objects.SubObject>);
			if (m_SubObjects.TryGetBuffer(owner, ref subObjects))
			{
				CheckConnectedLayers(ref connectedLayers, ref disconnectedLayers, ref resourceConnections, subObjects, subBuilding: false);
			}
			Layer layer = connectedLayers | disconnectedLayers;
			Building building = default(Building);
			PrefabRef prefabRef = default(PrefabRef);
			if ((disconnectedLayers & (Layer.PowerlineLow | Layer.PowerlineHigh | Layer.WaterPipe | Layer.SewagePipe | Layer.StormwaterPipe)) != Layer.None && m_BuildingData.TryGetComponent(owner, ref building) && m_PrefabRefData.TryGetComponent(building.m_RoadEdge, ref prefabRef))
			{
				NetData netData = m_PrefabNetData[prefabRef.m_Prefab];
				connectedLayers = (Layer)((uint)connectedLayers | ((uint)netData.m_LocalConnectLayers & 0xFFFFFFE5u));
			}
			if ((disconnectedLayers & Layer.PowerlineHigh) != Layer.None && (connectedLayers & Layer.PowerlineLow) != Layer.None && m_ElectricityProducerData.HasComponent(owner) && m_TransformerData.HasComponent(owner))
			{
				disconnectedLayers = (Layer)((uint)disconnectedLayers & 0xFFFFFFFBu);
			}
			disconnectedLayers &= ~connectedLayers;
			if (layer != Layer.None)
			{
				if (subNets.IsCreated)
				{
					UpdateConnectionWarnings(layer, connectedLayers, disconnectedLayers, resourceConnections, owner, subNets);
				}
				if (subObjects.IsCreated)
				{
					UpdateConnectionWarnings(layer, connectedLayers, disconnectedLayers, resourceConnections, subObjects);
				}
			}
			if (resourceConnections.IsCreated)
			{
				resourceConnections.Dispose();
			}
			if (!subNets.IsCreated)
			{
				return;
			}
			NativeHashMap<PathNode, Connection> nodeConnections = default(NativeHashMap<PathNode, Connection>);
			DynamicBuffer<ConnectedEdge> connectedEdges = default(DynamicBuffer<ConnectedEdge>);
			DynamicBuffer<SubLane> subLanes = default(DynamicBuffer<SubLane>);
			for (int i = 0; i < subNets.Length; i++)
			{
				Entity subNet = subNets[i].m_SubNet;
				if (!m_ConnectedEdges.TryGetBuffer(subNet, ref connectedEdges) || !m_SubLanes.TryGetBuffer(subNet, ref subLanes))
				{
					continue;
				}
				bool flag = false;
				for (int j = 0; j < connectedEdges.Length; j++)
				{
					if (!m_OwnerData.HasComponent(connectedEdges[j].m_Edge))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					bool flag2 = IsNativeMapTile(m_NodeData[subNet].m_Position) || m_OutsideConnectionData.HasComponent(subNet);
					if (!flag2)
					{
						FillNodeConnections(subLanes, ref nodeConnections);
						FillNodeConnections(connectedEdges, ref nodeConnections, subNet);
					}
					CheckNodeConnections(connectedEdges, nodeConnections, subNet, flag2, standaloneOnly: true);
					if (nodeConnections.IsCreated)
					{
						nodeConnections.Clear();
					}
				}
			}
			if (nodeConnections.IsCreated)
			{
				nodeConnections.Dispose();
			}
		}

		private void CheckConnectedLayers(ref Layer connectedLayers, ref Layer disconnectedLayers, ref NativeHashSet<Entity> resourceConnections, DynamicBuffer<Game.Objects.SubObject> subObjects, bool subBuilding)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<SubNet> subNets = default(DynamicBuffer<SubNet>);
			DynamicBuffer<Game.Objects.SubObject> subObjects2 = default(DynamicBuffer<Game.Objects.SubObject>);
			for (int i = 0; i < subObjects.Length; i++)
			{
				Entity subObject = subObjects[i].m_SubObject;
				bool subBuilding2 = subBuilding || m_BuildingData.HasComponent(subObject);
				if (m_SubNets.TryGetBuffer(subObject, ref subNets))
				{
					CheckConnectedLayers(ref connectedLayers, ref disconnectedLayers, ref resourceConnections, subObject, subNets, subBuilding2);
				}
				if (m_SubObjects.TryGetBuffer(subObject, ref subObjects2))
				{
					CheckConnectedLayers(ref connectedLayers, ref disconnectedLayers, ref resourceConnections, subObjects2, subBuilding2);
				}
			}
		}

		private void CheckConnectedLayers(ref Layer connectedLayers, ref Layer disconnectedLayers, ref NativeHashSet<Entity> resourceConnections, Entity owner, DynamicBuffer<SubNet> subNets, bool subBuilding)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < subNets.Length; i++)
			{
				Entity subNet = subNets[i].m_SubNet;
				if (!m_ConnectedEdges.HasBuffer(subNet))
				{
					continue;
				}
				PrefabRef prefabRef = m_PrefabRefData[subNet];
				NetData netData = m_PrefabNetData[prefabRef.m_Prefab];
				if (m_ResourceConnectionData.HasComponent(subNet))
				{
					if (subBuilding || m_ServiceUpgradeData.HasComponent(subNet))
					{
						disconnectedLayers |= Layer.ResourceLine;
					}
					else
					{
						AddResourceConnections(ref resourceConnections, subNet);
					}
					continue;
				}
				DynamicBuffer<ConnectedEdge> connectedEdges = m_ConnectedEdges[subNet];
				Layer connectedOnce = netData.m_RequiredLayers;
				Layer connectedTwice = Layer.None;
				FindEdgeConnections(subNet, connectedEdges, owner, netData.m_RequiredLayers, ref connectedOnce, ref connectedTwice);
				if (m_OutsideConnectionData.HasComponent(subNet))
				{
					connectedTwice |= connectedOnce;
				}
				connectedLayers |= netData.m_RequiredLayers & connectedTwice;
				disconnectedLayers |= netData.m_RequiredLayers & ~connectedTwice;
				if ((connectedOnce & ~netData.m_RequiredLayers & ~connectedTwice) != Layer.None)
				{
					FindSecondaryConnections(subNet, connectedEdges, ref connectedTwice);
				}
				if (connectedOnce != Layer.None)
				{
					UpdateConnectionWarnings(subNet, Entity.Null, prefabRef.m_Prefab, connectedOnce, netData.m_RequiredLayers | connectedTwice);
				}
			}
		}

		private void AddResourceConnections(ref NativeHashSet<Entity> resourceConnections, Entity subNet)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			if (!resourceConnections.IsCreated)
			{
				resourceConnections = new NativeHashSet<Entity>(100, AllocatorHandle.op_Implicit((Allocator)2));
			}
			if (!resourceConnections.Add(subNet) || m_ConnectedEdges[subNet].Length == 0)
			{
				return;
			}
			NativeList<Entity> val = default(NativeList<Entity>);
			val._002Ector(10, AllocatorHandle.op_Implicit((Allocator)2));
			val.Add(ref subNet);
			while (val.Length != 0)
			{
				subNet = val[val.Length - 1];
				val.RemoveAt(val.Length - 1);
				DynamicBuffer<ConnectedEdge> val2 = m_ConnectedEdges[subNet];
				for (int i = 0; i < val2.Length; i++)
				{
					ConnectedEdge connectedEdge = val2[i];
					Edge edge = m_EdgeData[connectedEdge.m_Edge];
					DynamicBuffer<ConnectedNode> val3 = m_ConnectedNodes[connectedEdge.m_Edge];
					if (resourceConnections.Add(edge.m_Start))
					{
						val.Add(ref edge.m_Start);
					}
					if (resourceConnections.Add(edge.m_End))
					{
						val.Add(ref edge.m_End);
					}
					for (int j = 0; j < val3.Length; j++)
					{
						Entity node = val3[j].m_Node;
						if (resourceConnections.Add(node))
						{
							val.Add(ref node);
						}
					}
				}
			}
		}

		private void UpdateConnectionWarnings(Layer allLayers, Layer connectedLayers, Layer disconnectedLayers, NativeHashSet<Entity> resourceConnections, DynamicBuffer<Game.Objects.SubObject> subObjects)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<SubNet> subNets = default(DynamicBuffer<SubNet>);
			DynamicBuffer<Game.Objects.SubObject> subObjects2 = default(DynamicBuffer<Game.Objects.SubObject>);
			for (int i = 0; i < subObjects.Length; i++)
			{
				Entity subObject = subObjects[i].m_SubObject;
				if (m_SubNets.TryGetBuffer(subObject, ref subNets))
				{
					UpdateConnectionWarnings(allLayers, connectedLayers, disconnectedLayers, resourceConnections, subObject, subNets);
				}
				if (m_SubObjects.TryGetBuffer(subObject, ref subObjects2))
				{
					UpdateConnectionWarnings(allLayers, connectedLayers, disconnectedLayers, resourceConnections, subObjects2);
				}
			}
		}

		private void UpdateConnectionWarnings(Layer allLayers, Layer connectedLayers, Layer disconnectedLayers, NativeHashSet<Entity> resourceConnections, Entity owner, DynamicBuffer<SubNet> subNets)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < subNets.Length; i++)
			{
				Entity subNet = subNets[i].m_SubNet;
				if (!m_ConnectedEdges.HasBuffer(subNet))
				{
					continue;
				}
				PrefabRef prefabRef = m_PrefabRefData[subNet];
				Layer layer = m_PrefabNetData[prefabRef.m_Prefab].m_RequiredLayers;
				if (m_ResourceConnectionData.HasComponent(subNet))
				{
					layer |= Layer.ResourceLine;
				}
				layer &= allLayers;
				if (layer != Layer.None)
				{
					Layer layer2 = connectedLayers | ~disconnectedLayers;
					if (resourceConnections.IsCreated && resourceConnections.Contains(subNet))
					{
						layer2 |= Layer.ResourceLine;
					}
					UpdateConnectionWarnings(owner, subNet, prefabRef.m_Prefab, layer, layer2);
				}
			}
		}

		private void UpdateNodeConnectionWarnings(Entity node, bool allConnected)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = m_PrefabRefData[node];
			NetData netData = m_PrefabNetData[prefabRef.m_Prefab];
			DynamicBuffer<ConnectedEdge> connectedEdges = m_ConnectedEdges[node];
			Layer connectedOnce = Layer.None;
			Layer connectedTwice = Layer.None;
			FindEdgeConnections(node, connectedEdges, netData.m_RequiredLayers, ref connectedOnce, ref connectedTwice);
			Layer layer = netData.m_RequiredLayers | connectedOnce;
			allConnected |= m_OutsideConnectionData.HasComponent(node);
			if ((layer & ~connectedTwice) != Layer.None)
			{
				FindSecondaryConnections(node, connectedEdges, ref connectedTwice);
				if (allConnected)
				{
					connectedTwice |= connectedOnce;
				}
			}
			if (layer != Layer.None)
			{
				UpdateConnectionWarnings(node, Entity.Null, prefabRef.m_Prefab, layer, connectedTwice);
			}
			DynamicBuffer<SubLane> subLanes = default(DynamicBuffer<SubLane>);
			if (m_SubLanes.TryGetBuffer(node, ref subLanes))
			{
				NativeHashMap<PathNode, Connection> nodeConnections = default(NativeHashMap<PathNode, Connection>);
				if (!allConnected)
				{
					FillNodeConnections(subLanes, ref nodeConnections);
					FillNodeConnections(connectedEdges, ref nodeConnections, node);
				}
				CheckNodeConnections(connectedEdges, nodeConnections, node, allConnected, standaloneOnly: false);
				if (nodeConnections.IsCreated)
				{
					nodeConnections.Dispose();
				}
			}
		}

		private void FillNodeConnections(DynamicBuffer<SubLane> subLanes, ref NativeHashMap<PathNode, Connection> nodeConnections)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			CarLaneData carLaneData = default(CarLaneData);
			TrackLaneData trackLaneData = default(TrackLaneData);
			for (int i = 0; i < subLanes.Length; i++)
			{
				Entity subLane = subLanes[i].m_SubLane;
				PrefabRef prefabRef = m_PrefabRefData[subLane];
				Connection connection = default(Connection);
				if (m_PrefabCarLaneData.TryGetComponent(prefabRef.m_Prefab, ref carLaneData) && !m_SlaveLaneData.HasComponent(subLane))
				{
					connection.m_RoadTypes = carLaneData.m_RoadTypes;
				}
				if (m_PrefabTrackLaneData.TryGetComponent(prefabRef.m_Prefab, ref trackLaneData))
				{
					connection.m_TrackTypes = trackLaneData.m_TrackTypes;
				}
				if (connection.m_RoadTypes != RoadTypes.None || connection.m_TrackTypes != TrackTypes.None)
				{
					Lane lane = m_LaneData[subLane];
					AddNodeConnection(lane.m_StartNode, connection, ref nodeConnections);
					AddNodeConnection(lane.m_EndNode, connection, ref nodeConnections);
				}
			}
		}

		private void FillNodeConnections(DynamicBuffer<ConnectedEdge> connectedEdges, ref NativeHashMap<PathNode, Connection> nodeConnections, Entity node)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<SubLane> subLanes = default(DynamicBuffer<SubLane>);
			for (int i = 0; i < connectedEdges.Length; i++)
			{
				Entity edge = connectedEdges[i].m_Edge;
				if (m_SubLanes.TryGetBuffer(edge, ref subLanes))
				{
					Edge edge2 = m_EdgeData[edge];
					if (edge2.m_Start == node)
					{
						FillNodeConnections(edge, subLanes, ref nodeConnections, 0f);
					}
					else if (edge2.m_End == node)
					{
						FillNodeConnections(edge, subLanes, ref nodeConnections, 1f);
					}
				}
			}
		}

		private void FillNodeConnections(Entity edgeEntity, DynamicBuffer<SubLane> subLanes, ref NativeHashMap<PathNode, Connection> nodeConnections, float edgeDelta)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			PathNode other = new PathNode(edgeEntity, 0);
			EdgeLane edgeLane = default(EdgeLane);
			CarLaneData carLaneData = default(CarLaneData);
			TrackLaneData trackLaneData = default(TrackLaneData);
			for (int i = 0; i < subLanes.Length; i++)
			{
				Entity subLane = subLanes[i].m_SubLane;
				if (!m_EdgeLaneData.TryGetComponent(subLane, ref edgeLane))
				{
					continue;
				}
				Lane lane = m_LaneData[subLane];
				PrefabRef prefabRef = m_PrefabRefData[subLane];
				Connection connection = default(Connection);
				if (m_PrefabCarLaneData.TryGetComponent(prefabRef.m_Prefab, ref carLaneData) && !m_SlaveLaneData.HasComponent(subLane))
				{
					if (edgeLane.m_EdgeDelta.x == edgeDelta && !lane.m_StartNode.OwnerEquals(other))
					{
						connection.m_RoadTypes2 = carLaneData.m_RoadTypes;
					}
					if (edgeLane.m_EdgeDelta.y == edgeDelta && !lane.m_EndNode.OwnerEquals(other))
					{
						connection.m_RoadTypes2 = carLaneData.m_RoadTypes;
					}
				}
				if (m_PrefabTrackLaneData.TryGetComponent(prefabRef.m_Prefab, ref trackLaneData))
				{
					if (edgeLane.m_EdgeDelta.x == edgeDelta && !lane.m_StartNode.OwnerEquals(other))
					{
						connection.m_TrackTypes2 = trackLaneData.m_TrackTypes;
					}
					if (edgeLane.m_EdgeDelta.y == edgeDelta && !lane.m_EndNode.OwnerEquals(other))
					{
						connection.m_TrackTypes2 = trackLaneData.m_TrackTypes;
					}
				}
				if (connection.m_RoadTypes2 != RoadTypes.None || connection.m_TrackTypes2 != TrackTypes.None)
				{
					if (edgeLane.m_EdgeDelta.x == edgeDelta)
					{
						AddNodeConnection(lane.m_StartNode, connection, ref nodeConnections);
					}
					if (edgeLane.m_EdgeDelta.y == edgeDelta)
					{
						AddNodeConnection(lane.m_EndNode, connection, ref nodeConnections);
					}
				}
			}
		}

		private void AddNodeConnection(PathNode pathNode, Connection connection, ref NativeHashMap<PathNode, Connection> nodeConnections)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			if (!nodeConnections.IsCreated)
			{
				nodeConnections = new NativeHashMap<PathNode, Connection>(100, AllocatorHandle.op_Implicit((Allocator)2));
			}
			Connection connection2 = default(Connection);
			if (nodeConnections.TryGetValue(pathNode, ref connection2))
			{
				connection.m_RoadTypes |= connection2.m_RoadTypes | (connection2.m_RoadTypes2 & connection.m_RoadTypes2);
				connection.m_RoadTypes2 |= connection2.m_RoadTypes2;
				connection.m_TrackTypes |= connection2.m_TrackTypes | (connection2.m_TrackTypes2 & connection.m_TrackTypes2);
				connection.m_TrackTypes2 |= connection2.m_TrackTypes2;
				if (connection.m_RoadTypes != connection2.m_RoadTypes || connection.m_RoadTypes2 != connection2.m_RoadTypes2 || connection.m_TrackTypes != connection2.m_TrackTypes || connection.m_TrackTypes2 != connection2.m_TrackTypes2)
				{
					nodeConnections[pathNode] = connection;
				}
			}
			else
			{
				nodeConnections.Add(pathNode, connection);
			}
		}

		private void CheckNodeConnections(DynamicBuffer<ConnectedEdge> connectedEdges, NativeHashMap<PathNode, Connection> nodeConnections, Entity node, bool allConnected, bool standaloneOnly)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<SubLane> subLanes = default(DynamicBuffer<SubLane>);
			for (int i = 0; i < connectedEdges.Length; i++)
			{
				Entity edge = connectedEdges[i].m_Edge;
				if ((!standaloneOnly || !m_OwnerData.HasComponent(edge)) && m_SubLanes.TryGetBuffer(edge, ref subLanes))
				{
					Edge edge2 = m_EdgeData[edge];
					if (edge2.m_Start == node)
					{
						CheckNodeConnections(subLanes, nodeConnections, 0f, allConnected);
					}
					else if (edge2.m_End == node)
					{
						CheckNodeConnections(subLanes, nodeConnections, 1f, allConnected);
					}
				}
			}
		}

		private void CheckNodeConnections(DynamicBuffer<SubLane> subLanes, NativeHashMap<PathNode, Connection> nodeConnections, float edgeDelta, bool allConnected)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			EdgeLane edgeLane = default(EdgeLane);
			CarLane carLane = default(CarLane);
			CarLaneData carLaneData = default(CarLaneData);
			Connection connection = default(Connection);
			Connection connection2 = default(Connection);
			TrackLane trackLane = default(TrackLane);
			TrackLaneData trackLaneData = default(TrackLaneData);
			Connection connection3 = default(Connection);
			Connection connection4 = default(Connection);
			for (int i = 0; i < subLanes.Length; i++)
			{
				Entity subLane = subLanes[i].m_SubLane;
				if (!m_EdgeLaneData.TryGetComponent(subLane, ref edgeLane))
				{
					continue;
				}
				Lane lane = m_LaneData[subLane];
				Curve curve = m_CurveData[subLane];
				PrefabRef prefabRef = m_PrefabRefData[subLane];
				if (m_CarLaneData.TryGetComponent(subLane, ref carLane) && !m_SlaveLaneData.HasComponent(subLane) && m_PrefabCarLaneData.TryGetComponent(prefabRef.m_Prefab, ref carLaneData))
				{
					if (edgeLane.m_EdgeDelta.x == edgeDelta)
					{
						if (allConnected || (carLane.m_Flags & CarLaneFlags.Twoway) != 0 || (nodeConnections.IsCreated && nodeConnections.TryGetValue(lane.m_StartNode, ref connection) && (connection.m_RoadTypes & carLaneData.m_RoadTypes) == carLaneData.m_RoadTypes))
						{
							m_IconCommandBuffer.Remove(subLane, m_TrafficConfigurationData.m_DeadEndNotification, Entity.Null);
						}
						else
						{
							m_IconCommandBuffer.Add(subLane, m_TrafficConfigurationData.m_DeadEndNotification, curve.m_Bezier.a, IconPriority.Warning);
						}
					}
					if (edgeLane.m_EdgeDelta.y == edgeDelta)
					{
						if (allConnected || (carLane.m_Flags & CarLaneFlags.Twoway) != 0 || (nodeConnections.IsCreated && nodeConnections.TryGetValue(lane.m_EndNode, ref connection2) && (connection2.m_RoadTypes & carLaneData.m_RoadTypes) == carLaneData.m_RoadTypes))
						{
							m_IconCommandBuffer.Remove(subLane, m_TrafficConfigurationData.m_DeadEndNotification, Entity.Null, IconFlags.SecondaryLocation);
						}
						else
						{
							m_IconCommandBuffer.Add(subLane, m_TrafficConfigurationData.m_DeadEndNotification, curve.m_Bezier.d, IconPriority.Warning, IconClusterLayer.Default, IconFlags.SecondaryLocation);
						}
					}
				}
				if (!m_TrackLaneData.TryGetComponent(subLane, ref trackLane) || !m_PrefabTrackLaneData.TryGetComponent(prefabRef.m_Prefab, ref trackLaneData))
				{
					continue;
				}
				if (edgeLane.m_EdgeDelta.x == edgeDelta)
				{
					if (allConnected || (trackLane.m_Flags & TrackLaneFlags.Twoway) != 0 || (nodeConnections.IsCreated && nodeConnections.TryGetValue(lane.m_StartNode, ref connection3) && (connection3.m_TrackTypes & trackLaneData.m_TrackTypes) == trackLaneData.m_TrackTypes))
					{
						m_IconCommandBuffer.Remove(subLane, m_TrafficConfigurationData.m_TrackConnectionNotification, Entity.Null);
					}
					else
					{
						m_IconCommandBuffer.Add(subLane, m_TrafficConfigurationData.m_TrackConnectionNotification, curve.m_Bezier.a, IconPriority.Warning);
					}
				}
				if (edgeLane.m_EdgeDelta.y == edgeDelta)
				{
					if (allConnected || (trackLane.m_Flags & TrackLaneFlags.Twoway) != 0 || (nodeConnections.IsCreated && nodeConnections.TryGetValue(lane.m_EndNode, ref connection4) && (connection4.m_TrackTypes & trackLaneData.m_TrackTypes) == trackLaneData.m_TrackTypes))
					{
						m_IconCommandBuffer.Remove(subLane, m_TrafficConfigurationData.m_TrackConnectionNotification, Entity.Null, IconFlags.SecondaryLocation);
					}
					else
					{
						m_IconCommandBuffer.Add(subLane, m_TrafficConfigurationData.m_TrackConnectionNotification, curve.m_Bezier.d, IconPriority.Warning, IconClusterLayer.Default, IconFlags.SecondaryLocation);
					}
				}
			}
		}

		private void FindEdgeConnections(Entity node, DynamicBuffer<ConnectedEdge> connectedEdges, Layer nodeLayers, ref Layer connectedOnce, ref Layer connectedTwice)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < connectedEdges.Length; i++)
			{
				Entity edge = connectedEdges[i].m_Edge;
				Edge edge2 = m_EdgeData[edge];
				PrefabRef prefabRef = m_PrefabRefData[edge];
				NetData netData = m_PrefabNetData[prefabRef.m_Prefab];
				Layer layer = netData.m_RequiredLayers | (nodeLayers & netData.m_LocalConnectLayers);
				connectedTwice |= ((edge2.m_Start == node || edge2.m_End == node) ? (connectedOnce & layer) : layer);
				connectedOnce |= layer;
			}
		}

		private void FindEdgeConnections(Entity node, DynamicBuffer<ConnectedEdge> connectedEdges, Entity owner, Layer nodeLayers, ref Layer connectedOnce, ref Layer connectedTwice)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			Layer layer = Layer.None;
			Layer layer2 = Layer.None;
			Owner owner2 = default(Owner);
			for (int i = 0; i < connectedEdges.Length; i++)
			{
				Entity edge = connectedEdges[i].m_Edge;
				Edge edge2 = m_EdgeData[edge];
				PrefabRef prefabRef = m_PrefabRefData[edge];
				NetData netData = m_PrefabNetData[prefabRef.m_Prefab];
				Layer layer3 = netData.m_RequiredLayers | (nodeLayers & netData.m_LocalConnectLayers);
				if (m_OwnerData.TryGetComponent(edge, ref owner2) && owner2.m_Owner == owner)
				{
					connectedTwice |= ((edge2.m_Start == node || edge2.m_End == node) ? (layer & layer3) : layer3);
					layer |= layer3;
				}
				else
				{
					connectedTwice |= ((edge2.m_Start == node || edge2.m_End == node) ? (layer2 & layer3) : layer3);
					layer2 |= layer3;
				}
			}
			connectedTwice |= (layer | connectedOnce) & layer2;
			connectedOnce |= layer | layer2;
		}

		private void FindSecondaryConnections(Entity node, DynamicBuffer<ConnectedEdge> connectedEdges, ref Layer connected)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < connectedEdges.Length; i++)
			{
				Entity edge = connectedEdges[i].m_Edge;
				Edge edge2 = m_EdgeData[edge];
				PrefabRef prefabRef = m_PrefabRefData[edge];
				NetData netData = m_PrefabNetData[prefabRef.m_Prefab];
				if (edge2.m_Start == node)
				{
					DynamicBuffer<ConnectedNode> val = m_ConnectedNodes[edge];
					for (int j = 0; j < val.Length; j++)
					{
						ConnectedNode connectedNode = val[j];
						if (connectedNode.m_CurvePosition <= 0.5f)
						{
							PrefabRef prefabRef2 = m_PrefabRefData[connectedNode.m_Node];
							NetData netData2 = m_PrefabNetData[prefabRef2.m_Prefab];
							connected |= netData.m_RequiredLayers & netData2.m_RequiredLayers;
						}
					}
				}
				else
				{
					if (!(edge2.m_End == node))
					{
						continue;
					}
					DynamicBuffer<ConnectedNode> val2 = m_ConnectedNodes[edge];
					for (int k = 0; k < val2.Length; k++)
					{
						ConnectedNode connectedNode2 = val2[k];
						if (connectedNode2.m_CurvePosition >= 0.5f)
						{
							PrefabRef prefabRef3 = m_PrefabRefData[connectedNode2.m_Node];
							NetData netData3 = m_PrefabNetData[prefabRef3.m_Prefab];
							connected |= netData.m_RequiredLayers & netData3.m_RequiredLayers;
						}
					}
				}
			}
		}

		private void UpdateConnectionWarnings(Entity owner, Entity subNet, Entity prefab, Layer required, Layer connected)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			if ((required & (Layer.WaterPipe | Layer.SewagePipe)) != Layer.None)
			{
				Layer layer = required & ~connected;
				UpdateConnectionWarning(owner, subNet, m_WaterPipeParameterData.m_WaterPipeNotConnectedNotification, (layer & Layer.WaterPipe) != 0);
				UpdateConnectionWarning(owner, subNet, m_WaterPipeParameterData.m_SewagePipeNotConnectedNotification, (layer & Layer.SewagePipe) != 0);
			}
			if ((required & (Layer.PowerlineLow | Layer.PowerlineHigh)) != Layer.None)
			{
				Layer layer2 = required & ~connected;
				UpdateConnectionWarning(owner, subNet, m_ElectricityParameterData.m_LowVoltageNotConnectedPrefab, (layer2 & Layer.PowerlineLow) != 0);
				UpdateConnectionWarning(owner, subNet, m_ElectricityParameterData.m_HighVoltageNotConnectedPrefab, (layer2 & Layer.PowerlineHigh) != 0);
			}
			ResourceConnectionData resourceConnectionData = default(ResourceConnectionData);
			if ((required & Layer.ResourceLine) != Layer.None && m_PrefabResourceConnectionData.TryGetComponent(prefab, ref resourceConnectionData) && resourceConnectionData.m_ConnectionWarningNotification != Entity.Null)
			{
				Layer layer3 = required & ~connected;
				UpdateConnectionWarning(owner, subNet, resourceConnectionData.m_ConnectionWarningNotification, (layer3 & Layer.ResourceLine) != 0);
			}
		}

		private void UpdateConnectionWarning(Entity owner, Entity subNet, Entity icon, bool active)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			if (!(icon != Entity.Null))
			{
				return;
			}
			if (subNet != Entity.Null)
			{
				if (active)
				{
					m_IconCommandBuffer.Add(owner, icon, IconPriority.Warning, IconClusterLayer.Default, IconFlags.TargetLocation, subNet);
				}
				else
				{
					m_IconCommandBuffer.Remove(owner, icon, subNet);
				}
			}
			else if (active)
			{
				m_IconCommandBuffer.Add(owner, icon, IconPriority.Warning);
			}
			else
			{
				m_IconCommandBuffer.Remove(owner, icon);
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Node> __Game_Net_Node_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Building> __Game_Buildings_Building_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<RoadConnectionUpdated> __Game_Buildings_RoadConnectionUpdated_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Node> __Game_Net_Node_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Lane> __Game_Net_Lane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeLane> __Game_Net_EdgeLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SlaveLane> __Game_Net_SlaveLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarLane> __Game_Net_CarLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrackLane> __Game_Net_TrackLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PedestrianLane> __Game_Net_PedestrianLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ConnectionLane> __Game_Net_ConnectionLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LaneConnection> __Game_Net_LaneConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<OutsideConnection> __Game_Net_OutsideConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ResourceConnection> __Game_Net_ResourceConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Target> __Game_Common_Target_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Native> __Game_Common_Native_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Destroyed> __Game_Common_Destroyed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> __Game_Objects_SpawnLocation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Routes.TakeoffLocation> __Game_Routes_TakeoffLocation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AccessLane> __Game_Routes_AccessLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RouteLane> __Game_Routes_RouteLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityProducer> __Game_Buildings_ElectricityProducer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Transformer> __Game_Buildings_Transformer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.ServiceUpgrade> __Game_Buildings_ServiceUpgrade_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MapTile> __Game_Areas_MapTile_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Icon> __Game_Notifications_Icon_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetData> __Game_Prefabs_NetData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> __Game_Prefabs_SpawnLocationData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RouteConnectionData> __Game_Prefabs_RouteConnectionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LocalConnectData> __Game_Prefabs_LocalConnectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarLaneData> __Game_Prefabs_CarLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrackLaneData> __Game_Prefabs_TrackLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ResourceConnectionData> __Game_Prefabs_ResourceConnectionData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<SubNet> __Game_Net_SubNet_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedNode> __Game_Net_ConnectedNode_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> __Game_Areas_SubArea_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> __Game_Areas_Node_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Triangle> __Game_Areas_Triangle_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<IconElement> __Game_Notifications_IconElement_RO_BufferLookup;

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
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Net_Node_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Node>(true);
			__Game_Buildings_Building_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Building>(true);
			__Game_Buildings_RoadConnectionUpdated_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<RoadConnectionUpdated>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Net_Node_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Node>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_Lane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(true);
			__Game_Net_EdgeLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeLane>(true);
			__Game_Net_SlaveLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SlaveLane>(true);
			__Game_Net_CarLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarLane>(true);
			__Game_Net_TrackLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrackLane>(true);
			__Game_Net_PedestrianLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PedestrianLane>(true);
			__Game_Net_ConnectionLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ConnectionLane>(true);
			__Game_Net_LaneConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneConnection>(true);
			__Game_Net_OutsideConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<OutsideConnection>(true);
			__Game_Net_ResourceConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceConnection>(true);
			__Game_Common_Target_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Target>(true);
			__Game_Common_Native_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Native>(true);
			__Game_Common_Destroyed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Destroyed>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Objects_SpawnLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.SpawnLocation>(true);
			__Game_Routes_TakeoffLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Routes.TakeoffLocation>(true);
			__Game_Routes_AccessLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AccessLane>(true);
			__Game_Routes_RouteLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RouteLane>(true);
			__Game_Buildings_ElectricityProducer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityProducer>(true);
			__Game_Buildings_Transformer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.Transformer>(true);
			__Game_Buildings_ServiceUpgrade_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.ServiceUpgrade>(true);
			__Game_Areas_MapTile_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MapTile>(true);
			__Game_Notifications_Icon_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Icon>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_NetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetData>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_SpawnLocationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnLocationData>(true);
			__Game_Prefabs_RouteConnectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RouteConnectionData>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
			__Game_Prefabs_LocalConnectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LocalConnectData>(true);
			__Game_Prefabs_CarLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarLaneData>(true);
			__Game_Prefabs_TrackLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrackLaneData>(true);
			__Game_Prefabs_ResourceConnectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceConnectionData>(true);
			__Game_Net_SubNet_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubNet>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubLane>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Net_ConnectedNode_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedNode>(true);
			__Game_Areas_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.SubArea>(true);
			__Game_Areas_Node_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.Node>(true);
			__Game_Areas_Triangle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Triangle>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
			__Game_Notifications_IconElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<IconElement>(true);
		}
	}

	private ToolSystem m_ToolSystem;

	private IconCommandSystem m_IconCommandSystem;

	private Game.Areas.SearchSystem m_AreaSearchSystem;

	private Game.Areas.UpdateCollectSystem m_AreaUpdateCollectSystem;

	private SearchSystem m_NetSearchSystem;

	private Game.Objects.SearchSystem m_ObjectSearchSystem;

	private EntityQuery m_UpdateQuery;

	private EntityQuery m_NewGameQuery;

	private EntityQuery m_WaterConfigQuery;

	private EntityQuery m_ElectricityConfigQuery;

	private EntityQuery m_TrafficConfigQuery;

	private bool m_IsNewGame;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Expected O, but got Unknown
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Expected O, but got Unknown
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_IconCommandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IconCommandSystem>();
		m_AreaSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Areas.SearchSystem>();
		m_AreaUpdateCollectSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Areas.UpdateCollectSystem>();
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SearchSystem>();
		m_ObjectSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.SearchSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[2];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Updated>() };
		val.Any = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Node>(),
			ComponentType.ReadOnly<Game.Buildings.ServiceUpgrade>(),
			ComponentType.ReadOnly<Game.Objects.SpawnLocation>(),
			ComponentType.ReadOnly<Game.Routes.TakeoffLocation>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<RoadConnectionUpdated>(),
			ComponentType.ReadOnly<Event>()
		};
		array[1] = val;
		m_UpdateQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Node>(),
			ComponentType.ReadOnly<Game.Buildings.ServiceUpgrade>(),
			ComponentType.ReadOnly<Game.Objects.SpawnLocation>(),
			ComponentType.ReadOnly<Game.Routes.TakeoffLocation>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array2[0] = val;
		m_NewGameQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		m_WaterConfigQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<WaterPipeParameterData>() });
		m_ElectricityConfigQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ElectricityParameterData>() });
		m_TrafficConfigQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TrafficConfigurationData>() });
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Invalid comparison between Unknown and I4
		base.OnGameLoaded(serializationContext);
		m_IsNewGame = (int)((Context)(ref serializationContext)).purpose == 1;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0595: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Unknown result type (might be due to invalid IL or missing references)
		//IL_060e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0626: Unknown result type (might be due to invalid IL or missing references)
		//IL_062b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0643: Unknown result type (might be due to invalid IL or missing references)
		//IL_0648: Unknown result type (might be due to invalid IL or missing references)
		//IL_0660: Unknown result type (might be due to invalid IL or missing references)
		//IL_0665: Unknown result type (might be due to invalid IL or missing references)
		//IL_067d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0682: Unknown result type (might be due to invalid IL or missing references)
		//IL_069a: Unknown result type (might be due to invalid IL or missing references)
		//IL_069f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_070e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0713: Unknown result type (might be due to invalid IL or missing references)
		//IL_072b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0730: Unknown result type (might be due to invalid IL or missing references)
		//IL_0748: Unknown result type (might be due to invalid IL or missing references)
		//IL_074d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0765: Unknown result type (might be due to invalid IL or missing references)
		//IL_076a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0782: Unknown result type (might be due to invalid IL or missing references)
		//IL_0787: Unknown result type (might be due to invalid IL or missing references)
		//IL_079f: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07de: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0813: Unknown result type (might be due to invalid IL or missing references)
		//IL_0818: Unknown result type (might be due to invalid IL or missing references)
		//IL_0830: Unknown result type (might be due to invalid IL or missing references)
		//IL_0835: Unknown result type (might be due to invalid IL or missing references)
		//IL_084d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0852: Unknown result type (might be due to invalid IL or missing references)
		//IL_086a: Unknown result type (might be due to invalid IL or missing references)
		//IL_086f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0876: Unknown result type (might be due to invalid IL or missing references)
		//IL_0878: Unknown result type (might be due to invalid IL or missing references)
		//IL_087a: Unknown result type (might be due to invalid IL or missing references)
		//IL_087c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0881: Unknown result type (might be due to invalid IL or missing references)
		//IL_0886: Unknown result type (might be due to invalid IL or missing references)
		//IL_088a: Unknown result type (might be due to invalid IL or missing references)
		//IL_088c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0898: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		EntityQuery val = (m_IsNewGame ? m_NewGameQuery : m_UpdateQuery);
		m_IsNewGame = false;
		bool flag = !((EntityQuery)(ref val)).IsEmptyIgnoreFilter;
		bool mapTilesUpdated = m_AreaUpdateCollectSystem.mapTilesUpdated;
		if (flag || mapTilesUpdated)
		{
			NativeList<Entity> val2 = default(NativeList<Entity>);
			val2._002Ector(32, AllocatorHandle.op_Implicit((Allocator)3));
			JobHandle val3 = ((SystemBase)this).Dependency;
			if (flag)
			{
				JobHandle val4 = default(JobHandle);
				NativeList<ArchetypeChunk> chunks = ((EntityQuery)(ref val)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val4);
				JobHandle val5 = IJobExtensions.Schedule<CollectOwnersJob>(new CollectOwnersJob
				{
					m_Chunks = chunks,
					m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_NodeType = InternalCompilerInterface.GetComponentTypeHandle<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_BuildingType = InternalCompilerInterface.GetComponentTypeHandle<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_RoadConnectionUpdatedType = InternalCompilerInterface.GetComponentTypeHandle<RoadConnectionUpdated>(ref __TypeHandle.__Game_Buildings_RoadConnectionUpdated_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_Owners = val2
				}, JobHandle.CombineDependencies(val3, val4));
				chunks.Dispose(val5);
				val3 = val5;
			}
			if (mapTilesUpdated)
			{
				JobHandle dependencies;
				JobHandle dependencies2;
				JobHandle dependencies3;
				JobHandle val6 = IJobExtensions.Schedule<CollectOwnersJob2>(new CollectOwnersJob2
				{
					m_NodeData = InternalCompilerInterface.GetComponentLookup<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies),
					m_ObjectSearchTree = m_ObjectSearchSystem.GetStaticSearchTree(readOnly: true, out dependencies2),
					m_Bounds = m_AreaUpdateCollectSystem.GetUpdatedMapTileBounds(out dependencies3),
					m_Owners = val2
				}, JobUtils.CombineDependencies(val3, dependencies, dependencies2, dependencies3));
				m_NetSearchSystem.AddNetSearchTreeReader(val6);
				m_ObjectSearchSystem.AddStaticSearchTreeReader(val6);
				m_AreaUpdateCollectSystem.AddMapTileBoundsReader(val6);
				val3 = val6;
			}
			JobHandle dependencies4;
			JobHandle val7 = IJobParallelForDeferExtensions.Schedule<CheckOwnersJob, Entity>(new CheckOwnersJob
			{
				m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
				m_Owners = val2.AsDeferredJobArray(),
				m_WaterPipeParameterData = GetConfigData<WaterPipeParameterData>(m_WaterConfigQuery),
				m_ElectricityParameterData = GetConfigData<ElectricityParameterData>(m_ElectricityConfigQuery),
				m_TrafficConfigurationData = GetConfigData<TrafficConfigurationData>(m_TrafficConfigQuery),
				m_AreaSearchTree = m_AreaSearchSystem.GetSearchTree(readOnly: true, out dependencies4),
				m_IconCommandBuffer = m_IconCommandSystem.CreateCommandBuffer(),
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NodeData = InternalCompilerInterface.GetComponentLookup<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeLaneData = InternalCompilerInterface.GetComponentLookup<EdgeLane>(ref __TypeHandle.__Game_Net_EdgeLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SlaveLaneData = InternalCompilerInterface.GetComponentLookup<SlaveLane>(ref __TypeHandle.__Game_Net_SlaveLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CarLaneData = InternalCompilerInterface.GetComponentLookup<CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TrackLaneData = InternalCompilerInterface.GetComponentLookup<TrackLane>(ref __TypeHandle.__Game_Net_TrackLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PedestrianLaneData = InternalCompilerInterface.GetComponentLookup<PedestrianLane>(ref __TypeHandle.__Game_Net_PedestrianLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectionLaneData = InternalCompilerInterface.GetComponentLookup<ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LaneConnectionData = InternalCompilerInterface.GetComponentLookup<LaneConnection>(ref __TypeHandle.__Game_Net_LaneConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OutsideConnectionData = InternalCompilerInterface.GetComponentLookup<OutsideConnection>(ref __TypeHandle.__Game_Net_OutsideConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ResourceConnectionData = InternalCompilerInterface.GetComponentLookup<ResourceConnection>(ref __TypeHandle.__Game_Net_ResourceConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TargetData = InternalCompilerInterface.GetComponentLookup<Target>(ref __TypeHandle.__Game_Common_Target_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NativeData = InternalCompilerInterface.GetComponentLookup<Native>(ref __TypeHandle.__Game_Common_Native_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_DestroyedData = InternalCompilerInterface.GetComponentLookup<Destroyed>(ref __TypeHandle.__Game_Common_Destroyed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SpawnLocationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TakeoffLocationData = InternalCompilerInterface.GetComponentLookup<Game.Routes.TakeoffLocation>(ref __TypeHandle.__Game_Routes_TakeoffLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AccessLaneData = InternalCompilerInterface.GetComponentLookup<AccessLane>(ref __TypeHandle.__Game_Routes_AccessLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_RouteLaneData = InternalCompilerInterface.GetComponentLookup<RouteLane>(ref __TypeHandle.__Game_Routes_RouteLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ElectricityProducerData = InternalCompilerInterface.GetComponentLookup<ElectricityProducer>(ref __TypeHandle.__Game_Buildings_ElectricityProducer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TransformerData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.Transformer>(ref __TypeHandle.__Game_Buildings_Transformer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ServiceUpgradeData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.ServiceUpgrade>(ref __TypeHandle.__Game_Buildings_ServiceUpgrade_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_MapTileData = InternalCompilerInterface.GetComponentLookup<MapTile>(ref __TypeHandle.__Game_Areas_MapTile_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_IconData = InternalCompilerInterface.GetComponentLookup<Icon>(ref __TypeHandle.__Game_Notifications_Icon_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabNetData = InternalCompilerInterface.GetComponentLookup<NetData>(ref __TypeHandle.__Game_Prefabs_NetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabBuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabSpawnLocationData = InternalCompilerInterface.GetComponentLookup<SpawnLocationData>(ref __TypeHandle.__Game_Prefabs_SpawnLocationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRouteConnectionData = InternalCompilerInterface.GetComponentLookup<RouteConnectionData>(ref __TypeHandle.__Game_Prefabs_RouteConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabNetGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabLocalConnectData = InternalCompilerInterface.GetComponentLookup<LocalConnectData>(ref __TypeHandle.__Game_Prefabs_LocalConnectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabCarLaneData = InternalCompilerInterface.GetComponentLookup<CarLaneData>(ref __TypeHandle.__Game_Prefabs_CarLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabTrackLaneData = InternalCompilerInterface.GetComponentLookup<TrackLaneData>(ref __TypeHandle.__Game_Prefabs_TrackLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabResourceConnectionData = InternalCompilerInterface.GetComponentLookup<ResourceConnectionData>(ref __TypeHandle.__Game_Prefabs_ResourceConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubNets = InternalCompilerInterface.GetBufferLookup<SubNet>(ref __TypeHandle.__Game_Net_SubNet_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubLanes = InternalCompilerInterface.GetBufferLookup<SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectedNodes = InternalCompilerInterface.GetBufferLookup<ConnectedNode>(ref __TypeHandle.__Game_Net_ConnectedNode_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubAreas = InternalCompilerInterface.GetBufferLookup<Game.Areas.SubArea>(ref __TypeHandle.__Game_Areas_SubArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AreaNodes = InternalCompilerInterface.GetBufferLookup<Game.Areas.Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AreaTriangles = InternalCompilerInterface.GetBufferLookup<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_IconElements = InternalCompilerInterface.GetBufferLookup<IconElement>(ref __TypeHandle.__Game_Notifications_IconElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
			}, val2, 1, JobHandle.CombineDependencies(val3, dependencies4));
			val2.Dispose(val7);
			m_AreaSearchSystem.AddSearchTreeReader(val7);
			m_IconCommandSystem.AddCommandBufferWriter(val7);
			((SystemBase)this).Dependency = val7;
		}
	}

	private T GetConfigData<T>(EntityQuery query) where T : unmanaged, IComponentData
	{
		if (((EntityQuery)(ref query)).IsEmptyIgnoreFilter)
		{
			return default(T);
		}
		return ((EntityQuery)(ref query)).GetSingleton<T>();
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
	public ConnectionWarningSystem()
	{
	}
}
