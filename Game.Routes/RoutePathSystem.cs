using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Net;
using Game.Pathfind;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Routes;

[CompilerGenerated]
public class RoutePathSystem : GameSystemBase, IDefaultSerializable, ISerializable
{
	public struct RoutePathType : IEquatable<RoutePathType>
	{
		public RouteConnectionType m_ConnectionType;

		public RoadTypes m_RoadType;

		public TrackTypes m_TrackType;

		public bool Equals(RoutePathType other)
		{
			if (m_ConnectionType == other.m_ConnectionType && m_RoadType == other.m_RoadType)
			{
				return m_TrackType == other.m_TrackType;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (int)((uint)((int)m_ConnectionType << 16) | ((uint)m_RoadType << 8)) | (int)m_TrackType;
		}
	}

	[BurstCompile]
	private struct CheckRoutePathsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public BufferTypeHandle<PathElement> m_PathElementType;

		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		public ParallelWriter<Entity> m_UpdateQueue;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			BufferAccessor<PathElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<PathElement>(ref m_PathElementType);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				DynamicBuffer<PathElement> val = bufferAccessor[i];
				for (int j = 0; j < val.Length; j++)
				{
					if (m_DeletedData.HasComponent(val[j].m_Target))
					{
						m_UpdateQueue.Enqueue(nativeArray[i]);
						break;
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
	private struct CheckAppliedLanesJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<Game.Net.CarLane> m_CarLaneType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.TrackLane> m_TrackLaneType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.PedestrianLane> m_PedestrianLaneType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentLookup<CarLaneData> m_CarLaneData;

		[ReadOnly]
		public ComponentLookup<TrackLaneData> m_TrackLaneData;

		public NativeParallelHashSet<RoutePathType> m_PathTypeSet;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).Has<Game.Net.CarLane>(ref m_CarLaneType))
			{
				NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				for (int i = 0; i < nativeArray.Length; i++)
				{
					PrefabRef prefabRef = nativeArray[i];
					CarLaneData carLaneData = m_CarLaneData[prefabRef.m_Prefab];
					if ((carLaneData.m_RoadTypes & RoadTypes.Car) != RoadTypes.None)
					{
						m_PathTypeSet.Add(new RoutePathType
						{
							m_ConnectionType = RouteConnectionType.Road,
							m_RoadType = RoadTypes.Car
						});
					}
					if ((carLaneData.m_RoadTypes & RoadTypes.Watercraft) != RoadTypes.None)
					{
						m_PathTypeSet.Add(new RoutePathType
						{
							m_ConnectionType = RouteConnectionType.Road,
							m_RoadType = RoadTypes.Watercraft
						});
					}
					if ((carLaneData.m_RoadTypes & RoadTypes.Helicopter) != RoadTypes.None)
					{
						m_PathTypeSet.Add(new RoutePathType
						{
							m_ConnectionType = RouteConnectionType.Road,
							m_RoadType = RoadTypes.Helicopter
						});
					}
					if ((carLaneData.m_RoadTypes & RoadTypes.Airplane) != RoadTypes.None)
					{
						m_PathTypeSet.Add(new RoutePathType
						{
							m_ConnectionType = RouteConnectionType.Road,
							m_RoadType = RoadTypes.Airplane
						});
					}
				}
			}
			if (((ArchetypeChunk)(ref chunk)).Has<Game.Net.TrackLane>(ref m_TrackLaneType))
			{
				NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					PrefabRef prefabRef2 = nativeArray2[j];
					TrackLaneData trackLaneData = m_TrackLaneData[prefabRef2.m_Prefab];
					if ((trackLaneData.m_TrackTypes & TrackTypes.Train) != TrackTypes.None)
					{
						m_PathTypeSet.Add(new RoutePathType
						{
							m_ConnectionType = RouteConnectionType.Track,
							m_TrackType = TrackTypes.Train
						});
					}
					if ((trackLaneData.m_TrackTypes & TrackTypes.Tram) != TrackTypes.None)
					{
						m_PathTypeSet.Add(new RoutePathType
						{
							m_ConnectionType = RouteConnectionType.Track,
							m_TrackType = TrackTypes.Tram
						});
					}
					if ((trackLaneData.m_TrackTypes & TrackTypes.Subway) != TrackTypes.None)
					{
						m_PathTypeSet.Add(new RoutePathType
						{
							m_ConnectionType = RouteConnectionType.Track,
							m_TrackType = TrackTypes.Subway
						});
					}
				}
			}
			if (((ArchetypeChunk)(ref chunk)).Has<Game.Net.PedestrianLane>(ref m_PedestrianLaneType))
			{
				m_PathTypeSet.Add(new RoutePathType
				{
					m_ConnectionType = RouteConnectionType.Pedestrian
				});
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct CheckSegmentRoutes : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<RouteConnectionData> m_RouteConnectionData;

		[ReadOnly]
		public NativeParallelHashSet<RoutePathType> m_PathTypeSet;

		public ParallelWriter<Entity> m_UpdateQueue;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Owner> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			RouteConnectionData routeConnectionData = default(RouteConnectionData);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Owner owner = nativeArray2[i];
				PrefabRef prefabRef = m_PrefabRefData[owner.m_Owner];
				if (m_RouteConnectionData.TryGetComponent(prefabRef.m_Prefab, ref routeConnectionData))
				{
					RoutePathType routePathType = new RoutePathType
					{
						m_ConnectionType = routeConnectionData.m_RouteConnectionType,
						m_RoadType = routeConnectionData.m_RouteRoadType,
						m_TrackType = routeConnectionData.m_RouteTrackType
					};
					if (m_PathTypeSet.Contains(routePathType))
					{
						m_UpdateQueue.Enqueue(nativeArray[i]);
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
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public BufferTypeHandle<PathElement> __Game_Pathfind_PathElement_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.CarLane> __Game_Net_CarLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.TrackLane> __Game_Net_TrackLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.PedestrianLane> __Game_Net_PedestrianLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<CarLaneData> __Game_Prefabs_CarLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrackLaneData> __Game_Prefabs_TrackLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RouteConnectionData> __Game_Prefabs_RouteConnectionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<Segment> __Game_Routes_Segment_RO_ComponentTypeHandle;

		public ComponentTypeHandle<PathTargets> __Game_Routes_PathTargets_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> __Game_Routes_RouteWaypoint_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<RouteLane> __Game_Routes_RouteLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Position> __Game_Routes_Position_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RouteData> __Game_Prefabs_RouteData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Segment> __Game_Routes_Segment_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Pathfind_PathElement_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<PathElement>(true);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Net_CarLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.CarLane>(true);
			__Game_Net_TrackLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.TrackLane>(true);
			__Game_Net_PedestrianLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.PedestrianLane>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Prefabs_CarLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarLaneData>(true);
			__Game_Prefabs_TrackLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrackLaneData>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_RouteConnectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RouteConnectionData>(true);
			__Game_Routes_Segment_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Segment>(true);
			__Game_Routes_PathTargets_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathTargets>(false);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Routes_RouteWaypoint_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteWaypoint>(true);
			__Game_Routes_RouteLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RouteLane>(true);
			__Game_Routes_Position_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Position>(true);
			__Game_Prefabs_RouteData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RouteData>(true);
			__Game_Routes_Segment_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Segment>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
		}
	}

	private PathfindQueueSystem m_PathfindQueueSystem;

	private EntityQuery m_UpdatedSegmentQuery;

	private EntityQuery m_DeletedLaneQuery;

	private EntityQuery m_AppliedLaneQuery;

	private EntityQuery m_SegmentQuery;

	private NativeParallelHashSet<Entity> m_LazyUpdateSet;

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
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PathfindQueueSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindQueueSystem>();
		m_UpdatedSegmentQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Segment>(),
			ComponentType.ReadWrite<PathTargets>()
		});
		m_DeletedLaneQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Lane>(),
			ComponentType.Exclude<Temp>()
		});
		m_AppliedLaneQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Applied>(),
			ComponentType.ReadOnly<Lane>()
		});
		m_SegmentQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Segment>(),
			ComponentType.ReadOnly<PathElement>(),
			ComponentType.Exclude<Deleted>()
		});
		m_LazyUpdateSet = new NativeParallelHashSet<Entity>(20, AllocatorHandle.op_Implicit((Allocator)4));
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_LazyUpdateSet.Dispose();
		base.OnDestroy();
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		int num = m_LazyUpdateSet.Count();
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		Enumerator<Entity> enumerator = m_LazyUpdateSet.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Entity current = enumerator.Current;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(current);
		}
		enumerator.Dispose();
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		m_LazyUpdateSet.Clear();
		int num = default(int);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		Entity val = default(Entity);
		for (int i = 0; i < num; i++)
		{
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref val);
			if (val != Entity.Null)
			{
				m_LazyUpdateSet.Add(val);
			}
		}
	}

	public void SetDefaults(Context context)
	{
		m_LazyUpdateSet.Clear();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_068c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0691: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0704: Unknown result type (might be due to invalid IL or missing references)
		//IL_0709: Unknown result type (might be due to invalid IL or missing references)
		//IL_071c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0721: Unknown result type (might be due to invalid IL or missing references)
		//IL_0734: Unknown result type (might be due to invalid IL or missing references)
		//IL_0739: Unknown result type (might be due to invalid IL or missing references)
		//IL_0743: Unknown result type (might be due to invalid IL or missing references)
		//IL_0748: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_0761: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_08de: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_090e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0913: Unknown result type (might be due to invalid IL or missing references)
		//IL_0926: Unknown result type (might be due to invalid IL or missing references)
		//IL_092b: Unknown result type (might be due to invalid IL or missing references)
		//IL_093e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0943: Unknown result type (might be due to invalid IL or missing references)
		//IL_0956: Unknown result type (might be due to invalid IL or missing references)
		//IL_095b: Unknown result type (might be due to invalid IL or missing references)
		//IL_096e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0973: Unknown result type (might be due to invalid IL or missing references)
		//IL_0986: Unknown result type (might be due to invalid IL or missing references)
		//IL_098b: Unknown result type (might be due to invalid IL or missing references)
		//IL_098e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0993: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0770: Unknown result type (might be due to invalid IL or missing references)
		//IL_077a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0785: Unknown result type (might be due to invalid IL or missing references)
		//IL_0790: Unknown result type (might be due to invalid IL or missing references)
		//IL_079d: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0886: Unknown result type (might be due to invalid IL or missing references)
		//IL_0877: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0806: Unknown result type (might be due to invalid IL or missing references)
		//IL_080b: Unknown result type (might be due to invalid IL or missing references)
		//IL_080f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0816: Unknown result type (might be due to invalid IL or missing references)
		//IL_081b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0821: Unknown result type (might be due to invalid IL or missing references)
		//IL_0831: Unknown result type (might be due to invalid IL or missing references)
		//IL_083e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0840: Unknown result type (might be due to invalid IL or missing references)
		//IL_0842: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_09db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a02: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a24: Unknown result type (might be due to invalid IL or missing references)
		//IL_052e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a52: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a62: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a66: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a71: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a83: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a88: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a93: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0abb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0abd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0abf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05db: Unknown result type (might be due to invalid IL or missing references)
		//IL_05df: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0601: Unknown result type (might be due to invalid IL or missing references)
		//IL_060e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0610: Unknown result type (might be due to invalid IL or missing references)
		//IL_0612: Unknown result type (might be due to invalid IL or missing references)
		//IL_0625: Unknown result type (might be due to invalid IL or missing references)
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0575: Unknown result type (might be due to invalid IL or missing references)
		//IL_0583: Unknown result type (might be due to invalid IL or missing references)
		//IL_0588: Unknown result type (might be due to invalid IL or missing references)
		//IL_058a: Unknown result type (might be due to invalid IL or missing references)
		//IL_058f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0599: Unknown result type (might be due to invalid IL or missing references)
		bool flag = !((EntityQuery)(ref m_DeletedLaneQuery)).IsEmptyIgnoreFilter && !((EntityQuery)(ref m_SegmentQuery)).IsEmptyIgnoreFilter;
		bool flag2 = !((EntityQuery)(ref m_AppliedLaneQuery)).IsEmptyIgnoreFilter && !((EntityQuery)(ref m_SegmentQuery)).IsEmptyIgnoreFilter;
		bool flag3 = !((EntityQuery)(ref m_UpdatedSegmentQuery)).IsEmptyIgnoreFilter;
		if (!flag && !flag2 && !flag3 && m_LazyUpdateSet.IsEmpty)
		{
			return;
		}
		NativeQueue<Entity> val = default(NativeQueue<Entity>);
		NativeQueue<Entity> val2 = default(NativeQueue<Entity>);
		NativeParallelHashSet<Entity> val3 = default(NativeParallelHashSet<Entity>);
		JobHandle val4 = default(JobHandle);
		JobHandle val5 = default(JobHandle);
		if (flag)
		{
			val._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			val4 = JobChunkExtensions.ScheduleParallel<CheckRoutePathsJob>(new CheckRoutePathsJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PathElementType = InternalCompilerInterface.GetBufferTypeHandle<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_UpdateQueue = val.AsParallelWriter()
			}, m_SegmentQuery, ((SystemBase)this).Dependency);
			JobHandle.ScheduleBatchedJobs();
		}
		if (flag2)
		{
			NativeParallelHashSet<RoutePathType> pathTypeSet = default(NativeParallelHashSet<RoutePathType>);
			pathTypeSet._002Ector(10, AllocatorHandle.op_Implicit((Allocator)3));
			val2._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			CheckAppliedLanesJob checkAppliedLanesJob = new CheckAppliedLanesJob
			{
				m_CarLaneType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TrackLaneType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.TrackLane>(ref __TypeHandle.__Game_Net_TrackLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PedestrianLaneType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.PedestrianLane>(ref __TypeHandle.__Game_Net_PedestrianLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CarLaneData = InternalCompilerInterface.GetComponentLookup<CarLaneData>(ref __TypeHandle.__Game_Prefabs_CarLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TrackLaneData = InternalCompilerInterface.GetComponentLookup<TrackLaneData>(ref __TypeHandle.__Game_Prefabs_TrackLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PathTypeSet = pathTypeSet
			};
			CheckSegmentRoutes obj = new CheckSegmentRoutes
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_RouteConnectionData = InternalCompilerInterface.GetComponentLookup<RouteConnectionData>(ref __TypeHandle.__Game_Prefabs_RouteConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PathTypeSet = pathTypeSet,
				m_UpdateQueue = val2.AsParallelWriter()
			};
			JobHandle val6 = JobChunkExtensions.Schedule<CheckAppliedLanesJob>(checkAppliedLanesJob, m_AppliedLaneQuery, ((SystemBase)this).Dependency);
			JobHandle val7 = JobChunkExtensions.ScheduleParallel<CheckSegmentRoutes>(obj, m_SegmentQuery, val6);
			pathTypeSet.Dispose(val7);
			val5 = val7;
			JobHandle.ScheduleBatchedJobs();
		}
		if (flag || flag3)
		{
			val3._002Ector(10, AllocatorHandle.op_Implicit((Allocator)2));
		}
		JobHandle dependency;
		if (flag3)
		{
			NativeArray<ArchetypeChunk> val8 = ((EntityQuery)(ref m_UpdatedSegmentQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
			EntityTypeHandle entityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<Segment> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<Segment>(ref __TypeHandle.__Game_Routes_Segment_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<Owner> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<PathTargets> componentTypeHandle3 = InternalCompilerInterface.GetComponentTypeHandle<PathTargets>(ref __TypeHandle.__Game_Routes_PathTargets_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<Temp> componentTypeHandle4 = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<PrefabRef> componentTypeHandle5 = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			BufferLookup<RouteWaypoint> bufferLookup = InternalCompilerInterface.GetBufferLookup<RouteWaypoint>(ref __TypeHandle.__Game_Routes_RouteWaypoint_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<RouteLane> componentLookup = InternalCompilerInterface.GetComponentLookup<RouteLane>(ref __TypeHandle.__Game_Routes_RouteLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<Position> componentLookup2 = InternalCompilerInterface.GetComponentLookup<Position>(ref __TypeHandle.__Game_Routes_Position_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<RouteData> componentLookup3 = InternalCompilerInterface.GetComponentLookup<RouteData>(ref __TypeHandle.__Game_Prefabs_RouteData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<RouteConnectionData> componentLookup4 = InternalCompilerInterface.GetComponentLookup<RouteConnectionData>(ref __TypeHandle.__Game_Prefabs_RouteConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			dependency = ((SystemBase)this).Dependency;
			((JobHandle)(ref dependency)).Complete();
			float2 val12 = default(float2);
			for (int i = 0; i < val8.Length; i++)
			{
				ArchetypeChunk val9 = val8[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val9)).GetNativeArray(entityTypeHandle);
				NativeArray<Segment> nativeArray2 = ((ArchetypeChunk)(ref val9)).GetNativeArray<Segment>(ref componentTypeHandle);
				NativeArray<Owner> nativeArray3 = ((ArchetypeChunk)(ref val9)).GetNativeArray<Owner>(ref componentTypeHandle2);
				NativeArray<PathTargets> nativeArray4 = ((ArchetypeChunk)(ref val9)).GetNativeArray<PathTargets>(ref componentTypeHandle3);
				NativeArray<PrefabRef> nativeArray5 = ((ArchetypeChunk)(ref val9)).GetNativeArray<PrefabRef>(ref componentTypeHandle5);
				bool highPriority = ((ArchetypeChunk)(ref val9)).Has<Temp>(ref componentTypeHandle4);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Entity val10 = nativeArray[j];
					Segment segment = nativeArray2[j];
					Owner owner = nativeArray3[j];
					PathTargets pathTargets = nativeArray4[j];
					PrefabRef prefabRef = nativeArray5[j];
					if (!bufferLookup.HasBuffer(owner.m_Owner))
					{
						continue;
					}
					DynamicBuffer<RouteWaypoint> val11 = bufferLookup[owner.m_Owner];
					int num = segment.m_Index + 1;
					if (num == val11.Length)
					{
						num = 0;
					}
					Entity waypoint = val11[segment.m_Index].m_Waypoint;
					Entity waypoint2 = val11[num].m_Waypoint;
					if (componentLookup.HasComponent(waypoint) && componentLookup.HasComponent(waypoint2))
					{
						RouteLane startLane = componentLookup[waypoint];
						RouteLane endLane = componentLookup[waypoint2];
						((float2)(ref val12))._002Ector(startLane.m_EndCurvePos, endLane.m_StartCurvePos);
						if (!(pathTargets.m_StartLane == startLane.m_EndLane) || !(pathTargets.m_EndLane == endLane.m_StartLane) || !math.all(math.abs(pathTargets.m_CurvePositions - val12) < 0.001f))
						{
							pathTargets.m_StartLane = startLane.m_EndLane;
							pathTargets.m_EndLane = endLane.m_StartLane;
							pathTargets.m_CurvePositions = val12;
							float3 position = componentLookup2[waypoint].m_Position;
							float3 position2 = componentLookup2[waypoint2].m_Position;
							RouteData route = componentLookup3[prefabRef.m_Prefab];
							RouteConnectionData routeConnection = componentLookup4[prefabRef.m_Prefab];
							SetupPathfind(val10, position, position2, startLane, endLane, route, routeConnection, highPriority);
							val3.Add(val10);
							m_LazyUpdateSet.Remove(val10);
							nativeArray4[j] = pathTargets;
						}
					}
				}
			}
			val8.Dispose();
		}
		if (flag)
		{
			BufferLookup<RouteWaypoint> bufferLookup2 = InternalCompilerInterface.GetBufferLookup<RouteWaypoint>(ref __TypeHandle.__Game_Routes_RouteWaypoint_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<RouteLane> componentLookup5 = InternalCompilerInterface.GetComponentLookup<RouteLane>(ref __TypeHandle.__Game_Routes_RouteLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<Position> componentLookup6 = InternalCompilerInterface.GetComponentLookup<Position>(ref __TypeHandle.__Game_Routes_Position_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<Segment> componentLookup7 = InternalCompilerInterface.GetComponentLookup<Segment>(ref __TypeHandle.__Game_Routes_Segment_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<Owner> componentLookup8 = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<PrefabRef> componentLookup9 = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<RouteData> componentLookup10 = InternalCompilerInterface.GetComponentLookup<RouteData>(ref __TypeHandle.__Game_Prefabs_RouteData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<RouteConnectionData> componentLookup11 = InternalCompilerInterface.GetComponentLookup<RouteConnectionData>(ref __TypeHandle.__Game_Prefabs_RouteConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			((JobHandle)(ref val4)).Complete();
			dependency = ((SystemBase)this).Dependency;
			((JobHandle)(ref dependency)).Complete();
			Entity val13 = default(Entity);
			while (val.TryDequeue(ref val13))
			{
				if (val3.Add(val13))
				{
					m_LazyUpdateSet.Remove(val13);
					Segment segment2 = componentLookup7[val13];
					Owner owner2 = componentLookup8[val13];
					PrefabRef prefabRef2 = componentLookup9[val13];
					DynamicBuffer<RouteWaypoint> val14 = bufferLookup2[owner2.m_Owner];
					int num2 = segment2.m_Index + 1;
					if (num2 == val14.Length)
					{
						num2 = 0;
					}
					Entity waypoint3 = val14[segment2.m_Index].m_Waypoint;
					Entity waypoint4 = val14[num2].m_Waypoint;
					RouteLane startLane2 = componentLookup5[waypoint3];
					RouteLane endLane2 = componentLookup5[waypoint4];
					float3 position3 = componentLookup6[waypoint3].m_Position;
					float3 position4 = componentLookup6[waypoint4].m_Position;
					RouteData route2 = componentLookup10[prefabRef2.m_Prefab];
					RouteConnectionData routeConnection2 = componentLookup11[prefabRef2.m_Prefab];
					SetupPathfind(val13, position3, position4, startLane2, endLane2, route2, routeConnection2, highPriority: false);
				}
			}
		}
		if (flag2)
		{
			((JobHandle)(ref val5)).Complete();
			Entity val15 = default(Entity);
			while (val2.TryDequeue(ref val15))
			{
				if (!val3.IsCreated || !val3.Contains(val15))
				{
					m_LazyUpdateSet.Add(val15);
				}
			}
		}
		if (!m_LazyUpdateSet.IsEmpty && (!val3.IsCreated || val3.IsEmpty))
		{
			BufferLookup<RouteWaypoint> bufferLookup3 = InternalCompilerInterface.GetBufferLookup<RouteWaypoint>(ref __TypeHandle.__Game_Routes_RouteWaypoint_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<RouteLane> componentLookup12 = InternalCompilerInterface.GetComponentLookup<RouteLane>(ref __TypeHandle.__Game_Routes_RouteLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<Position> componentLookup13 = InternalCompilerInterface.GetComponentLookup<Position>(ref __TypeHandle.__Game_Routes_Position_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<Segment> componentLookup14 = InternalCompilerInterface.GetComponentLookup<Segment>(ref __TypeHandle.__Game_Routes_Segment_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<Owner> componentLookup15 = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<Deleted> componentLookup16 = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<PrefabRef> componentLookup17 = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<RouteData> componentLookup18 = InternalCompilerInterface.GetComponentLookup<RouteData>(ref __TypeHandle.__Game_Prefabs_RouteData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<RouteConnectionData> componentLookup19 = InternalCompilerInterface.GetComponentLookup<RouteConnectionData>(ref __TypeHandle.__Game_Prefabs_RouteConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			dependency = ((SystemBase)this).Dependency;
			((JobHandle)(ref dependency)).Complete();
			Enumerator<Entity> enumerator = m_LazyUpdateSet.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Entity current = enumerator.Current;
				m_LazyUpdateSet.Remove(current);
				enumerator.Dispose();
				enumerator = m_LazyUpdateSet.GetEnumerator();
				if (componentLookup14.HasComponent(current) && !componentLookup16.HasComponent(current))
				{
					Segment segment3 = componentLookup14[current];
					Owner owner3 = componentLookup15[current];
					PrefabRef prefabRef3 = componentLookup17[current];
					DynamicBuffer<RouteWaypoint> val16 = bufferLookup3[owner3.m_Owner];
					int num3 = segment3.m_Index + 1;
					if (num3 == val16.Length)
					{
						num3 = 0;
					}
					Entity waypoint5 = val16[segment3.m_Index].m_Waypoint;
					Entity waypoint6 = val16[num3].m_Waypoint;
					RouteLane startLane3 = componentLookup12[waypoint5];
					RouteLane endLane3 = componentLookup12[waypoint6];
					float3 position5 = componentLookup13[waypoint5].m_Position;
					float3 position6 = componentLookup13[waypoint6].m_Position;
					RouteData route3 = componentLookup18[prefabRef3.m_Prefab];
					RouteConnectionData routeConnection3 = componentLookup19[prefabRef3.m_Prefab];
					SetupPathfind(current, position5, position6, startLane3, endLane3, route3, routeConnection3, highPriority: false);
					break;
				}
			}
			enumerator.Dispose();
		}
		if (val.IsCreated)
		{
			val.Dispose();
		}
		if (val2.IsCreated)
		{
			val2.Dispose();
		}
		if (val3.IsCreated)
		{
			val3.Dispose();
		}
	}

	private void SetupPathfind(Entity entity, float3 startPos, float3 endPos, RouteLane startLane, RouteLane endLane, RouteData route, RouteConnectionData routeConnection, bool highPriority)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		PathfindParameters parameters = new PathfindParameters
		{
			m_MaxSpeed = float2.op_Implicit(277.77777f),
			m_WalkSpeed = float2.op_Implicit(5.555556f),
			m_Weights = new PathfindWeights(1f, 1f, 1f, 1f),
			m_PathfindFlags = (PathfindFlags.Stable | PathfindFlags.IgnoreFlow),
			m_IgnoredRules = (RuleFlags.HasBlockage | RuleFlags.ForbidCombustionEngines | RuleFlags.ForbidHeavyTraffic | RuleFlags.ForbidPrivateTraffic | RuleFlags.ForbidSlowTraffic),
			m_Methods = RouteUtils.GetPathMethods(routeConnection.m_RouteConnectionType, route.m_Type, routeConnection.m_RouteTrackType, routeConnection.m_RouteRoadType, routeConnection.m_RouteSizeClass)
		};
		if (routeConnection.m_RouteConnectionType != RouteConnectionType.Road || routeConnection.m_RouteRoadType != RoadTypes.Car)
		{
			parameters.m_IgnoredRules |= RuleFlags.ForbidTransitTraffic;
		}
		PathfindAction action = new PathfindAction(1, 1, (Allocator)4, parameters, SetupTargetType.None, SetupTargetType.None);
		action.data.m_StartTargets[0] = new PathTarget(startLane.m_EndLane, startLane.m_EndLane, startLane.m_EndCurvePos, 0f);
		action.data.m_EndTargets[0] = new PathTarget(endLane.m_StartLane, endLane.m_StartLane, endLane.m_StartCurvePos, 0f);
		PathEventData eventData = new PathEventData
		{
			m_Position1 = startPos,
			m_Position2 = endPos
		};
		m_PathfindQueueSystem.Enqueue(action, entity, default(JobHandle), uint.MaxValue, this, eventData, highPriority);
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
	public RoutePathSystem()
	{
	}
}
