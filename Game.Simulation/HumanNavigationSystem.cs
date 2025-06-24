using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Creatures;
using Game.Events;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Rendering;
using Game.Routes;
using Game.Tools;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class HumanNavigationSystem : GameSystemBase
{
	[CompilerGenerated]
	public class Groups : GameSystemBase
	{
		private struct TypeHandle
		{
			[ReadOnly]
			public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

			[ReadOnly]
			public ComponentTypeHandle<GroupMember> __Game_Creatures_GroupMember_RO_ComponentTypeHandle;

			[ReadOnly]
			public ComponentLookup<CurrentVehicle> __Game_Creatures_CurrentVehicle_RO_ComponentLookup;

			[ReadOnly]
			public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

			public ComponentLookup<HumanCurrentLane> __Game_Creatures_HumanCurrentLane_RW_ComponentLookup;

			public ComponentLookup<PathOwner> __Game_Pathfind_PathOwner_RW_ComponentLookup;

			public BufferLookup<PathElement> __Game_Pathfind_PathElement_RW_BufferLookup;

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
				__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
				__Game_Creatures_GroupMember_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<GroupMember>(true);
				__Game_Creatures_CurrentVehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentVehicle>(true);
				__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
				__Game_Creatures_HumanCurrentLane_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HumanCurrentLane>(false);
				__Game_Pathfind_PathOwner_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathOwner>(false);
				__Game_Pathfind_PathElement_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PathElement>(false);
			}
		}

		private SimulationSystem m_SimulationSystem;

		private EntityQuery m_CreatureQuery;

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
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			base.OnCreate();
			m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
			m_CreatureQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
			{
				ComponentType.ReadOnly<Human>(),
				ComponentType.ReadOnly<GroupMember>(),
				ComponentType.ReadOnly<UpdateFrame>(),
				ComponentType.ReadWrite<HumanCurrentLane>(),
				ComponentType.Exclude<Deleted>(),
				ComponentType.Exclude<Temp>()
			});
		}

		[Preserve]
		protected override void OnUpdate()
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			uint index = m_SimulationSystem.frameIndex % 16;
			((EntityQuery)(ref m_CreatureQuery)).ResetFilter();
			((EntityQuery)(ref m_CreatureQuery)).SetSharedComponentFilter<UpdateFrame>(new UpdateFrame(index));
			JobHandle dependency = JobChunkExtensions.ScheduleParallel<GroupNavigationJob>(new GroupNavigationJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_GroupMemberType = InternalCompilerInterface.GetComponentTypeHandle<GroupMember>(ref __TypeHandle.__Game_Creatures_GroupMember_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CurrentVehicleData = InternalCompilerInterface.GetComponentLookup<CurrentVehicle>(ref __TypeHandle.__Game_Creatures_CurrentVehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CurrentLaneData = InternalCompilerInterface.GetComponentLookup<HumanCurrentLane>(ref __TypeHandle.__Game_Creatures_HumanCurrentLane_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PathOwnerData = InternalCompilerInterface.GetComponentLookup<PathOwner>(ref __TypeHandle.__Game_Pathfind_PathOwner_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Paths = InternalCompilerInterface.GetBufferLookup<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
			}, m_CreatureQuery, ((SystemBase)this).Dependency);
			((SystemBase)this).Dependency = dependency;
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
		public Groups()
		{
		}
	}

	[CompilerGenerated]
	public class Actions : GameSystemBase
	{
		private struct TypeHandle
		{
			public ComponentLookup<LaneSignal> __Game_Net_LaneSignal_RW_ComponentLookup;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void __AssignHandles(ref SystemState state)
			{
				//IL_0003: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				__Game_Net_LaneSignal_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneSignal>(false);
			}
		}

		public LaneObjectUpdater m_LaneObjectUpdater;

		public NativeQueue<HumanNavigationHelpers.LaneSignal> m_LaneSignalQueue;

		public JobHandle m_Dependency;

		private TypeHandle __TypeHandle;

		[Preserve]
		protected override void OnCreate()
		{
			base.OnCreate();
			m_LaneObjectUpdater = new LaneObjectUpdater((SystemBase)(object)this);
		}

		[Preserve]
		protected override void OnUpdate()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			JobHandle val = JobHandle.CombineDependencies(((SystemBase)this).Dependency, m_Dependency);
			JobHandle val2 = IJobExtensions.Schedule<UpdateLaneSignalsJob>(new UpdateLaneSignalsJob
			{
				m_LaneSignalQueue = m_LaneSignalQueue,
				m_LaneSignalData = InternalCompilerInterface.GetComponentLookup<LaneSignal>(ref __TypeHandle.__Game_Net_LaneSignal_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
			}, val);
			m_LaneSignalQueue.Dispose(val2);
			JobHandle val3 = m_LaneObjectUpdater.Apply((SystemBase)(object)this, val);
			((SystemBase)this).Dependency = JobHandle.CombineDependencies(val3, val2);
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
		public Actions()
		{
		}
	}

	[BurstCompile]
	private struct GroupNavigationJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<GroupMember> m_GroupMemberType;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> m_CurrentVehicleData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<HumanCurrentLane> m_CurrentLaneData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<PathOwner> m_PathOwnerData;

		[NativeDisableParallelForRestriction]
		public BufferLookup<PathElement> m_Paths;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<GroupMember> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<GroupMember>(ref m_GroupMemberType);
			if (nativeArray.Length == 0)
			{
				return;
			}
			NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			Owner owner = default(Owner);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Entity val = nativeArray2[i];
				GroupMember groupMember = nativeArray[i];
				HumanCurrentLane humanCurrentLane = m_CurrentLaneData[val];
				PathOwner pathOwner = m_PathOwnerData[val];
				DynamicBuffer<PathElement> val2 = m_Paths[val];
				if (pathOwner.m_ElementIndex > 0)
				{
					val2.RemoveRange(0, pathOwner.m_ElementIndex);
					pathOwner.m_ElementIndex = 0;
				}
				humanCurrentLane.m_Flags &= ~CreatureLaneFlags.Leader;
				pathOwner.m_State &= PathFlags.Stuck;
				if (m_CurrentVehicleData.HasComponent(groupMember.m_Leader))
				{
					if (val2.Length == 0 && (humanCurrentLane.m_Flags & (CreatureLaneFlags.ParkingSpace | CreatureLaneFlags.Transport)) == 0)
					{
						humanCurrentLane.m_Flags |= CreatureLaneFlags.Transport;
					}
				}
				else if (m_CurrentLaneData.HasComponent(groupMember.m_Leader))
				{
					HumanCurrentLane humanCurrentLane2 = m_CurrentLaneData[groupMember.m_Leader];
					PathOwner pathOwner2 = m_PathOwnerData[groupMember.m_Leader];
					DynamicBuffer<PathElement> val3 = m_Paths[groupMember.m_Leader];
					if ((pathOwner2.m_State & (PathFlags.Pending | PathFlags.Failed | PathFlags.Obsolete | PathFlags.Updated)) == 0)
					{
						int num = -1;
						if (humanCurrentLane.m_Lane == humanCurrentLane2.m_Lane && humanCurrentLane.m_CurvePosition.y == humanCurrentLane2.m_CurvePosition.y && ((humanCurrentLane.m_Flags ^ humanCurrentLane2.m_Flags) & (CreatureLaneFlags.Taxi | CreatureLaneFlags.WaitPosition)) == 0)
						{
							humanCurrentLane.m_Flags |= CreatureLaneFlags.Leader;
							num = 0;
						}
						else
						{
							for (int j = 0; j < val2.Length; j++)
							{
								PathElement pathElement = val2[j];
								if (pathElement.m_Target == humanCurrentLane2.m_Lane && pathElement.m_TargetDelta.y == humanCurrentLane2.m_CurvePosition.y)
								{
									pathElement.m_Flags |= PathElementFlags.Leader;
									val2[j] = pathElement;
									num = j + 1;
									break;
								}
								pathElement.m_Flags &= ~PathElementFlags.Leader;
								val2[j] = pathElement;
							}
						}
						if (num == -1)
						{
							PathElementFlags pathElementFlags = PathElementFlags.Leader;
							if ((humanCurrentLane2.m_Flags & CreatureLaneFlags.Taxi) != 0)
							{
								pathElementFlags |= PathElementFlags.Secondary;
							}
							if ((humanCurrentLane2.m_Flags & CreatureLaneFlags.WaitPosition) != 0)
							{
								pathElementFlags |= PathElementFlags.WaitPosition;
							}
							val2.Clear();
							val2.Add(new PathElement(humanCurrentLane2.m_Lane, humanCurrentLane2.m_CurvePosition, pathElementFlags));
						}
						else if (num < val2.Length)
						{
							val2.RemoveRange(num, val2.Length - num);
						}
						if ((humanCurrentLane2.m_Flags & CreatureLaneFlags.Area) != 0)
						{
							Entity val4 = Entity.Null;
							if (m_OwnerData.TryGetComponent(humanCurrentLane2.m_Lane, ref owner))
							{
								val4 = owner.m_Owner;
							}
							for (int k = pathOwner2.m_ElementIndex; k < val3.Length; k++)
							{
								PathElement pathElement2 = val3[k];
								val2.Add(pathElement2);
								if (!m_OwnerData.TryGetComponent(pathElement2.m_Target, ref owner) || owner.m_Owner != val4)
								{
									break;
								}
							}
						}
						else
						{
							int num2 = math.min(pathOwner2.m_ElementIndex + 2, val3.Length);
							for (int l = pathOwner2.m_ElementIndex; l < num2; l++)
							{
								val2.Add(val3[l]);
							}
						}
					}
				}
				m_CurrentLaneData[val] = humanCurrentLane;
				m_PathOwnerData[val] = pathOwner;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct UpdateNavigationJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<Moving> m_MovingType;

		[ReadOnly]
		public ComponentTypeHandle<GroupMember> m_GroupMemberType;

		[ReadOnly]
		public ComponentTypeHandle<Stumbling> m_StumblingType;

		[ReadOnly]
		public ComponentTypeHandle<TripSource> m_TripSourceType;

		[ReadOnly]
		public ComponentTypeHandle<Human> m_HumanType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentVehicle> m_CurrentVehicleType;

		[ReadOnly]
		public ComponentTypeHandle<InvolvedInAccident> m_InvolvedInAccidentType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<MeshGroup> m_MeshGroupType;

		public ComponentTypeHandle<HumanNavigation> m_NavigationType;

		public ComponentTypeHandle<HumanCurrentLane> m_CurrentLaneType;

		public ComponentTypeHandle<Blocker> m_BlockerType;

		public ComponentTypeHandle<PathOwner> m_PathOwnerType;

		public BufferTypeHandle<Queue> m_QueueType;

		public BufferTypeHandle<PathElement> m_PathElementType;

		[ReadOnly]
		public EntityStorageInfoLookup m_EntityLookup;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Game.Net.PedestrianLane> m_PedestrianLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> m_ParkingLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> m_ConnectionLaneData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<LaneSignal> m_LaneSignalData;

		[ReadOnly]
		public ComponentLookup<LaneReservation> m_LaneReservationData;

		[ReadOnly]
		public ComponentLookup<AreaLane> m_AreaLaneData;

		[ReadOnly]
		public ComponentLookup<Waypoint> m_WaypointData;

		[ReadOnly]
		public ComponentLookup<TaxiStand> m_TaxiStandData;

		[ReadOnly]
		public ComponentLookup<Position> m_PositionData;

		[ReadOnly]
		public ComponentLookup<Connected> m_ConnectedData;

		[ReadOnly]
		public ComponentLookup<Game.Routes.TakeoffLocation> m_TakeoffLocationData;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenterData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Moving> m_MovingData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> m_SpawnLocationData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.ActivityLocation> m_ActivityLocationData;

		[ReadOnly]
		public ComponentLookup<Creature> m_CreatureData;

		[ReadOnly]
		public ComponentLookup<GroupMember> m_GroupMemberData;

		[ReadOnly]
		public ComponentLookup<HangaroundLocation> m_HangaroundLocationData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PublicTransport> m_PublicTransportData;

		[ReadOnly]
		public ComponentLookup<Train> m_TrainData;

		[ReadOnly]
		public ComponentLookup<Controller> m_ControllerData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<CreatureData> m_PrefabCreatureData;

		[ReadOnly]
		public ComponentLookup<HumanData> m_PrefabHumanData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_PrefabBuildingData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<NetLaneData> m_PrefabLaneData;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> m_PrefabSpawnLocationData;

		[ReadOnly]
		public ComponentLookup<CarData> m_PrefabCarData;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_Lanes;

		[ReadOnly]
		public BufferLookup<LaneObject> m_LaneObjects;

		[ReadOnly]
		public BufferLookup<LaneOverlap> m_LaneOverlaps;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> m_AreaNodes;

		[ReadOnly]
		public BufferLookup<Triangle> m_AreaTriangles;

		[ReadOnly]
		public BufferLookup<ActivityLocationElement> m_PrefabActivityLocations;

		[ReadOnly]
		public BufferLookup<SubMeshGroup> m_SubMeshGroups;

		[ReadOnly]
		public BufferLookup<CharacterElement> m_CharacterElements;

		[ReadOnly]
		public BufferLookup<AnimationClip> m_AnimationClips;

		[ReadOnly]
		public BufferLookup<AnimationMotion> m_AnimationMotions;

		[ReadOnly]
		public BufferLookup<SubMesh> m_SubMeshes;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public bool m_LeftHandTraffic;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_StaticObjectSearchTree;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_MovingObjectSearchTree;

		[ReadOnly]
		public NativeQuadTree<AreaSearchItem, QuadTreeBoundsXZ> m_AreaSearchTree;

		public LaneObjectCommandBuffer m_LaneObjectBuffer;

		public ParallelWriter<HumanNavigationHelpers.LaneSignal> m_LaneSignals;

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
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03da: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Transform> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			NativeArray<Moving> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Moving>(ref m_MovingType);
			NativeArray<GroupMember> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<GroupMember>(ref m_GroupMemberType);
			NativeArray<HumanNavigation> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HumanNavigation>(ref m_NavigationType);
			NativeArray<HumanCurrentLane> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HumanCurrentLane>(ref m_CurrentLaneType);
			NativeArray<Blocker> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Blocker>(ref m_BlockerType);
			NativeArray<PathOwner> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathOwner>(ref m_PathOwnerType);
			NativeArray<PrefabRef> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<Queue> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Queue>(ref m_QueueType);
			BufferAccessor<PathElement> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<PathElement>(ref m_PathElementType);
			BufferAccessor<MeshGroup> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<MeshGroup>(ref m_MeshGroupType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			if (((ArchetypeChunk)(ref chunk)).Has<Stumbling>(ref m_StumblingType))
			{
				Moving moving = default(Moving);
				GroupMember groupMember = default(GroupMember);
				for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
				{
					Entity entity = nativeArray[i];
					Transform transform = nativeArray2[i];
					HumanNavigation navigation = nativeArray5[i];
					HumanCurrentLane currentLane = nativeArray6[i];
					Blocker blocker = nativeArray7[i];
					PathOwner pathOwner = nativeArray8[i];
					PrefabRef prefabRef = nativeArray9[i];
					DynamicBuffer<Queue> val = bufferAccessor[i];
					DynamicBuffer<PathElement> pathElements = bufferAccessor2[i];
					ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
					CollectionUtils.TryGet<Moving>(nativeArray3, i, ref moving);
					CollectionUtils.TryGet<GroupMember>(nativeArray4, i, ref groupMember);
					HumanNavigationHelpers.CurrentLaneCache currentLaneCache = new HumanNavigationHelpers.CurrentLaneCache(ref currentLane, m_EntityLookup, m_MovingObjectSearchTree);
					val.Clear();
					UpdateStumbling(entity, transform, groupMember, objectGeometryData, ref navigation, ref currentLane, ref blocker, ref pathOwner, pathElements);
					currentLaneCache.CheckChanges(entity, ref currentLane, m_LaneObjectBuffer, m_LaneObjects, transform, moving, navigation, objectGeometryData);
					nativeArray5[i] = navigation;
					nativeArray6[i] = currentLane;
					nativeArray7[i] = blocker;
					nativeArray8[i] = pathOwner;
				}
				return;
			}
			NativeArray<TripSource> nativeArray10 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TripSource>(ref m_TripSourceType);
			NativeArray<Human> nativeArray11 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Human>(ref m_HumanType);
			NativeArray<CurrentVehicle> nativeArray12 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentVehicle>(ref m_CurrentVehicleType);
			bool isInvolvedInAccident = ((ArchetypeChunk)(ref chunk)).Has<InvolvedInAccident>(ref m_InvolvedInAccidentType);
			GroupMember groupMember2 = default(GroupMember);
			TripSource tripSource = default(TripSource);
			CurrentVehicle currentVehicle = default(CurrentVehicle);
			DynamicBuffer<MeshGroup> meshGroups = default(DynamicBuffer<MeshGroup>);
			for (int j = 0; j < ((ArchetypeChunk)(ref chunk)).Count; j++)
			{
				Entity entity2 = nativeArray[j];
				Transform transform2 = nativeArray2[j];
				Moving moving2 = nativeArray3[j];
				Human human = nativeArray11[j];
				HumanNavigation navigation2 = nativeArray5[j];
				HumanCurrentLane currentLane2 = nativeArray6[j];
				Blocker blocker2 = nativeArray7[j];
				PathOwner pathOwner2 = nativeArray8[j];
				PrefabRef prefabRef2 = nativeArray9[j];
				DynamicBuffer<Queue> queues = bufferAccessor[j];
				DynamicBuffer<PathElement> pathElements2 = bufferAccessor2[j];
				CreatureData prefabCreatureData = m_PrefabCreatureData[prefabRef2.m_Prefab];
				HumanData prefabHumanData = m_PrefabHumanData[prefabRef2.m_Prefab];
				ObjectGeometryData objectGeometryData2 = m_PrefabObjectGeometryData[prefabRef2.m_Prefab];
				CollectionUtils.TryGet<GroupMember>(nativeArray4, j, ref groupMember2);
				CollectionUtils.TryGet<TripSource>(nativeArray10, j, ref tripSource);
				CollectionUtils.TryGet<CurrentVehicle>(nativeArray12, j, ref currentVehicle);
				CollectionUtils.TryGet<MeshGroup>(bufferAccessor3, j, ref meshGroups);
				HumanNavigationHelpers.CurrentLaneCache currentLaneCache2 = new HumanNavigationHelpers.CurrentLaneCache(ref currentLane2, m_EntityLookup, m_MovingObjectSearchTree);
				if ((currentLane2.m_Lane == Entity.Null || (currentLane2.m_Flags & CreatureLaneFlags.Obsolete) != 0) && (human.m_Flags & HumanFlags.Carried) == 0)
				{
					if ((currentLane2.m_Flags & (CreatureLaneFlags.Obsolete | CreatureLaneFlags.FindLane)) == CreatureLaneFlags.FindLane)
					{
						TryFindCurrentLane(ref currentLane2, transform2);
					}
					else
					{
						TryFindCurrentLane(ref currentLane2, transform2);
						if (groupMember2.m_Leader == Entity.Null)
						{
							pathElements2.Clear();
							pathOwner2.m_ElementIndex = 0;
							pathOwner2.m_State |= PathFlags.Obsolete;
						}
					}
				}
				UpdateQueues(currentVehicle, ref pathOwner2, queues);
				UpdateNavigationTarget(ref random, isInvolvedInAccident, entity2, transform2, moving2, tripSource, currentVehicle, human, groupMember2, prefabRef2, prefabCreatureData, prefabHumanData, objectGeometryData2, ref navigation2, ref currentLane2, ref blocker2, ref pathOwner2, queues, pathElements2, meshGroups);
				currentLaneCache2.CheckChanges(entity2, ref currentLane2, m_LaneObjectBuffer, m_LaneObjects, transform2, moving2, navigation2, objectGeometryData2);
				nativeArray5[j] = navigation2;
				nativeArray6[j] = currentLane2;
				nativeArray7[j] = blocker2;
				nativeArray8[j] = pathOwner2;
			}
		}

		private void UpdateStumbling(Entity entity, Transform transform, GroupMember groupMember, ObjectGeometryData prefabObjectGeometryData, ref HumanNavigation navigation, ref HumanCurrentLane currentLane, ref Blocker blocker, ref PathOwner pathOwner, DynamicBuffer<PathElement> pathElements)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			TryFindCurrentLane(ref currentLane, transform);
			navigation = new HumanNavigation
			{
				m_TargetPosition = transform.m_Position
			};
			blocker = default(Blocker);
			pathOwner.m_ElementIndex = 0;
			pathElements.Clear();
			if (groupMember.m_Leader == Entity.Null)
			{
				pathOwner.m_State |= PathFlags.Obsolete;
			}
		}

		private void TryFindCurrentLane(ref HumanCurrentLane currentLane, Transform transform)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			bool flag = (currentLane.m_Flags & CreatureLaneFlags.EmergeUnspawned) != 0;
			currentLane.m_Flags &= ~(CreatureLaneFlags.EndOfPath | CreatureLaneFlags.EndReached | CreatureLaneFlags.TransformTarget | CreatureLaneFlags.ParkingSpace | CreatureLaneFlags.Obsolete | CreatureLaneFlags.Transport | CreatureLaneFlags.Connection | CreatureLaneFlags.Taxi | CreatureLaneFlags.FindLane | CreatureLaneFlags.Area | CreatureLaneFlags.Hangaround | CreatureLaneFlags.WaitPosition | CreatureLaneFlags.EmergeUnspawned);
			currentLane.m_Lane = Entity.Null;
			float3 position = transform.m_Position;
			Bounds3 bounds = default(Bounds3);
			((Bounds3)(ref bounds))._002Ector(position - 100f, position + 100f);
			HumanNavigationHelpers.FindLaneIterator findLaneIterator = new HumanNavigationHelpers.FindLaneIterator
			{
				m_Bounds = bounds,
				m_Position = position,
				m_MinDistance = 1000f,
				m_UnspawnedEmerge = flag,
				m_Result = currentLane,
				m_SubLanes = m_Lanes,
				m_AreaNodes = m_AreaNodes,
				m_AreaTriangles = m_AreaTriangles,
				m_PedestrianLaneData = m_PedestrianLaneData,
				m_ConnectionLaneData = m_ConnectionLaneData,
				m_CurveData = m_CurveData,
				m_HangaroundLocationData = m_HangaroundLocationData
			};
			m_NetSearchTree.Iterate<HumanNavigationHelpers.FindLaneIterator>(ref findLaneIterator, 0);
			m_StaticObjectSearchTree.Iterate<HumanNavigationHelpers.FindLaneIterator>(ref findLaneIterator, 0);
			if (!flag)
			{
				m_AreaSearchTree.Iterate<HumanNavigationHelpers.FindLaneIterator>(ref findLaneIterator, 0);
			}
			currentLane = findLaneIterator.m_Result;
		}

		private float GetTargetSpeed(TripSource tripSource, Human human, HumanData prefabHumanData, ref HumanCurrentLane currentLane)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			float result = 0f;
			if (tripSource.m_Source == Entity.Null)
			{
				if ((human.m_Flags & HumanFlags.Run) != 0)
				{
					return prefabHumanData.m_RunSpeed;
				}
				result = prefabHumanData.m_WalkSpeed;
			}
			if (m_PedestrianLaneData.HasComponent(currentLane.m_Lane))
			{
				Game.Net.PedestrianLane pedestrianLane = m_PedestrianLaneData[currentLane.m_Lane];
				if ((pedestrianLane.m_Flags & PedestrianLaneFlags.Unsafe) != 0)
				{
					return prefabHumanData.m_RunSpeed;
				}
				if ((pedestrianLane.m_Flags & PedestrianLaneFlags.Crosswalk) != 0)
				{
					result = math.lerp(prefabHumanData.m_WalkSpeed, prefabHumanData.m_RunSpeed, 0.25f);
				}
			}
			if (m_LaneSignalData.HasComponent(currentLane.m_Lane))
			{
				LaneSignal laneSignal = m_LaneSignalData[currentLane.m_Lane];
				if (laneSignal.m_Signal == LaneSignalType.SafeStop || laneSignal.m_Signal == LaneSignalType.Stop)
				{
					return prefabHumanData.m_RunSpeed;
				}
			}
			return result;
		}

		private void UpdateQueues(CurrentVehicle currentVehicle, ref PathOwner pathOwner, DynamicBuffer<Queue> queues)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			if ((pathOwner.m_State & (PathFlags.Pending | PathFlags.Failed | PathFlags.Obsolete | PathFlags.Updated)) != 0 || currentVehicle.m_Vehicle != Entity.Null)
			{
				queues.Clear();
				return;
			}
			for (int i = 0; i < queues.Length; i++)
			{
				Queue queue = queues[i];
				if (++queue.m_ObsoleteTime >= 500)
				{
					queues.RemoveAt(i--);
				}
				else
				{
					queues[i] = queue;
				}
			}
		}

		private void UpdateNavigationTarget(ref Random random, bool isInvolvedInAccident, Entity entity, Transform transform, Moving moving, TripSource tripSource, CurrentVehicle currentVehicle, Human human, GroupMember groupMember, PrefabRef prefabRef, CreatureData prefabCreatureData, HumanData prefabHumanData, ObjectGeometryData prefabObjectGeometryData, ref HumanNavigation navigation, ref HumanCurrentLane currentLane, ref Blocker blocker, ref PathOwner pathOwner, DynamicBuffer<Queue> queues, DynamicBuffer<PathElement> pathElements, DynamicBuffer<MeshGroup> meshGroups)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03de: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0404: Unknown result type (might be due to invalid IL or missing references)
			//IL_0410: Unknown result type (might be due to invalid IL or missing references)
			//IL_1309: Unknown result type (might be due to invalid IL or missing references)
			//IL_130e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04de: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0505: Unknown result type (might be due to invalid IL or missing references)
			//IL_050a: Unknown result type (might be due to invalid IL or missing references)
			//IL_051c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0521: Unknown result type (might be due to invalid IL or missing references)
			//IL_0538: Unknown result type (might be due to invalid IL or missing references)
			//IL_053d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0546: Unknown result type (might be due to invalid IL or missing references)
			//IL_054b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_034b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_1469: Unknown result type (might be due to invalid IL or missing references)
			//IL_146e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1476: Unknown result type (might be due to invalid IL or missing references)
			//IL_147b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1483: Unknown result type (might be due to invalid IL or missing references)
			//IL_1488: Unknown result type (might be due to invalid IL or missing references)
			//IL_1490: Unknown result type (might be due to invalid IL or missing references)
			//IL_1495: Unknown result type (might be due to invalid IL or missing references)
			//IL_149d: Unknown result type (might be due to invalid IL or missing references)
			//IL_14a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_14aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_14af: Unknown result type (might be due to invalid IL or missing references)
			//IL_14b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_14bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_14c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_14c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_14d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_14d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_14de: Unknown result type (might be due to invalid IL or missing references)
			//IL_14e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_14eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_14f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_14f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_14fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_1505: Unknown result type (might be due to invalid IL or missing references)
			//IL_150a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1512: Unknown result type (might be due to invalid IL or missing references)
			//IL_1517: Unknown result type (might be due to invalid IL or missing references)
			//IL_151f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1524: Unknown result type (might be due to invalid IL or missing references)
			//IL_152c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1531: Unknown result type (might be due to invalid IL or missing references)
			//IL_1538: Unknown result type (might be due to invalid IL or missing references)
			//IL_1539: Unknown result type (might be due to invalid IL or missing references)
			//IL_1542: Unknown result type (might be due to invalid IL or missing references)
			//IL_1547: Unknown result type (might be due to invalid IL or missing references)
			//IL_1550: Unknown result type (might be due to invalid IL or missing references)
			//IL_1555: Unknown result type (might be due to invalid IL or missing references)
			//IL_155e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1563: Unknown result type (might be due to invalid IL or missing references)
			//IL_158e: Unknown result type (might be due to invalid IL or missing references)
			//IL_158f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1598: Unknown result type (might be due to invalid IL or missing references)
			//IL_159d: Unknown result type (might be due to invalid IL or missing references)
			//IL_15a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_15ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_15b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_15b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_15be: Unknown result type (might be due to invalid IL or missing references)
			//IL_15dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_15de: Unknown result type (might be due to invalid IL or missing references)
			//IL_15fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_1603: Unknown result type (might be due to invalid IL or missing references)
			//IL_1628: Unknown result type (might be due to invalid IL or missing references)
			//IL_162d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1644: Unknown result type (might be due to invalid IL or missing references)
			//IL_1649: Unknown result type (might be due to invalid IL or missing references)
			//IL_1652: Unknown result type (might be due to invalid IL or missing references)
			//IL_1657: Unknown result type (might be due to invalid IL or missing references)
			//IL_165e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1660: Unknown result type (might be due to invalid IL or missing references)
			//IL_1669: Unknown result type (might be due to invalid IL or missing references)
			//IL_166b: Unknown result type (might be due to invalid IL or missing references)
			//IL_144d: Unknown result type (might be due to invalid IL or missing references)
			//IL_144f: Unknown result type (might be due to invalid IL or missing references)
			//IL_13b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_13bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_1337: Unknown result type (might be due to invalid IL or missing references)
			//IL_056b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0570: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_167b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1347: Unknown result type (might be due to invalid IL or missing references)
			//IL_134e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1353: Unknown result type (might be due to invalid IL or missing references)
			//IL_1358: Unknown result type (might be due to invalid IL or missing references)
			//IL_135b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1380: Unknown result type (might be due to invalid IL or missing references)
			//IL_1825: Unknown result type (might be due to invalid IL or missing references)
			//IL_182a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1860: Unknown result type (might be due to invalid IL or missing references)
			//IL_1865: Unknown result type (might be due to invalid IL or missing references)
			//IL_186e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1873: Unknown result type (might be due to invalid IL or missing references)
			//IL_187c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1881: Unknown result type (might be due to invalid IL or missing references)
			//IL_13e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_13f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_13ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_13b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_12d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_12d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_12ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_12f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_12fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_1302: Unknown result type (might be due to invalid IL or missing references)
			//IL_1409: Unknown result type (might be due to invalid IL or missing references)
			//IL_1410: Unknown result type (might be due to invalid IL or missing references)
			//IL_1415: Unknown result type (might be due to invalid IL or missing references)
			//IL_141a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1420: Unknown result type (might be due to invalid IL or missing references)
			//IL_1425: Unknown result type (might be due to invalid IL or missing references)
			//IL_142e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1433: Unknown result type (might be due to invalid IL or missing references)
			//IL_1435: Unknown result type (might be due to invalid IL or missing references)
			//IL_143a: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_09da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0641: Unknown result type (might be due to invalid IL or missing references)
			//IL_0644: Unknown result type (might be due to invalid IL or missing references)
			//IL_0649: Unknown result type (might be due to invalid IL or missing references)
			//IL_0652: Unknown result type (might be due to invalid IL or missing references)
			//IL_066b: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_178e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1795: Unknown result type (might be due to invalid IL or missing references)
			//IL_0913: Unknown result type (might be due to invalid IL or missing references)
			//IL_091a: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_077d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0791: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_07dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0728: Unknown result type (might be due to invalid IL or missing references)
			//IL_0734: Unknown result type (might be due to invalid IL or missing references)
			//IL_074a: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0606: Unknown result type (might be due to invalid IL or missing references)
			//IL_060b: Unknown result type (might be due to invalid IL or missing references)
			//IL_061c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0621: Unknown result type (might be due to invalid IL or missing references)
			//IL_062d: Unknown result type (might be due to invalid IL or missing references)
			//IL_171b: Unknown result type (might be due to invalid IL or missing references)
			//IL_172e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1735: Unknown result type (might be due to invalid IL or missing references)
			//IL_173c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1743: Unknown result type (might be due to invalid IL or missing references)
			//IL_17dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_17e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_176a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1771: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b0b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a4f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a56: Unknown result type (might be due to invalid IL or missing references)
			//IL_105b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b22: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd5: Unknown result type (might be due to invalid IL or missing references)
			//IL_11cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_11d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_1087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b75: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b7c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b50: Unknown result type (might be due to invalid IL or missing references)
			//IL_1202: Unknown result type (might be due to invalid IL or missing references)
			//IL_109e: Unknown result type (might be due to invalid IL or missing references)
			//IL_10a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c15: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c1c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0de1: Unknown result type (might be due to invalid IL or missing references)
			//IL_121f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1224: Unknown result type (might be due to invalid IL or missing references)
			//IL_1107: Unknown result type (might be due to invalid IL or missing references)
			//IL_110e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1115: Unknown result type (might be due to invalid IL or missing references)
			//IL_111a: Unknown result type (might be due to invalid IL or missing references)
			//IL_111e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1120: Unknown result type (might be due to invalid IL or missing references)
			//IL_1125: Unknown result type (might be due to invalid IL or missing references)
			//IL_1133: Unknown result type (might be due to invalid IL or missing references)
			//IL_113f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1144: Unknown result type (might be due to invalid IL or missing references)
			//IL_114b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1151: Unknown result type (might be due to invalid IL or missing references)
			//IL_1153: Unknown result type (might be due to invalid IL or missing references)
			//IL_1158: Unknown result type (might be due to invalid IL or missing references)
			//IL_1169: Unknown result type (might be due to invalid IL or missing references)
			//IL_116e: Unknown result type (might be due to invalid IL or missing references)
			//IL_117f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1184: Unknown result type (might be due to invalid IL or missing references)
			//IL_118d: Unknown result type (might be due to invalid IL or missing references)
			//IL_118f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1194: Unknown result type (might be due to invalid IL or missing references)
			//IL_1196: Unknown result type (might be due to invalid IL or missing references)
			//IL_119b: Unknown result type (might be due to invalid IL or missing references)
			//IL_11a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f2b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0df8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ff1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f4a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f4f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f5b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f60: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f75: Unknown result type (might be due to invalid IL or missing references)
			//IL_1252: Unknown result type (might be due to invalid IL or missing references)
			//IL_1259: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c6c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c71: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c79: Unknown result type (might be due to invalid IL or missing references)
			//IL_1018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e3f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e46: Unknown result type (might be due to invalid IL or missing references)
			//IL_12a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_12ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_12b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_12bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d21: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d26: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d33: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c9b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0caf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cbd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c88: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c94: Unknown result type (might be due to invalid IL or missing references)
			//IL_1042: Unknown result type (might be due to invalid IL or missing references)
			//IL_1047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fa1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fb0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ece: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d7e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d4a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d56: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d5b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d62: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d68: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d6a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d6f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cdf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ce1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ced: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cf2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d01: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d03: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d0a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d0c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ee5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ef9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d95: Unknown result type (might be due to invalid IL or missing references)
			//IL_0da1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0da6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0db3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0db5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dba: Unknown result type (might be due to invalid IL or missing references)
			float num = 4f / 15f;
			float num2 = math.length(moving.m_Velocity);
			if ((currentLane.m_Flags & CreatureLaneFlags.Connection) != 0)
			{
				prefabHumanData.m_WalkSpeed = 277.77777f;
				prefabHumanData.m_RunSpeed = 277.77777f;
				prefabHumanData.m_Acceleration = 277.77777f;
			}
			else
			{
				num2 = math.min(num2, prefabHumanData.m_RunSpeed);
			}
			Bounds1 val = default(Bounds1);
			((Bounds1)(ref val))._002Ector(num2 + new float2(0f - prefabHumanData.m_Acceleration, prefabHumanData.m_Acceleration) * num);
			float targetSpeed = GetTargetSpeed(tripSource, human, prefabHumanData, ref currentLane);
			float num3 = prefabHumanData.m_Acceleration * 0.1f;
			if (num2 <= prefabHumanData.m_WalkSpeed)
			{
				targetSpeed = math.min(targetSpeed, math.max(prefabHumanData.m_WalkSpeed, num2 + num3 * num));
				navigation.m_MaxSpeed = MathUtils.Clamp(targetSpeed, val);
			}
			else
			{
				Bounds1 val2 = default(Bounds1);
				((Bounds1)(ref val2))._002Ector(num2 + new float2(0f - num3, num3) * num);
				navigation.m_MaxSpeed = MathUtils.Clamp(targetSpeed, val2);
			}
			float num4 = math.max(prefabObjectGeometryData.m_Bounds.max.z, (prefabObjectGeometryData.m_Bounds.max.x - prefabObjectGeometryData.m_Bounds.min.x) * 0.5f);
			float num5;
			if ((currentLane.m_Flags & (CreatureLaneFlags.EndReached | CreatureLaneFlags.TransformTarget | CreatureLaneFlags.Area)) != 0 || currentLane.m_Lane == Entity.Null || ((currentLane.m_Flags & CreatureLaneFlags.Connection) != 0 && (currentLane.m_Flags & (CreatureLaneFlags.ParkingSpace | CreatureLaneFlags.WaitPosition)) != 0))
			{
				num5 = math.distance(transform.m_Position, navigation.m_TargetPosition);
				float distance = math.select(num5, math.max(0f, num5 - num4), (currentLane.m_Flags & CreatureLaneFlags.TransformTarget) == 0);
				float maxBrakingSpeed = CreatureUtils.GetMaxBrakingSpeed(prefabHumanData, distance, num);
				maxBrakingSpeed = MathUtils.Clamp(maxBrakingSpeed, val);
				navigation.m_MaxSpeed = math.min(navigation.m_MaxSpeed, maxBrakingSpeed);
			}
			else
			{
				if ((currentLane.m_Flags & CreatureLaneFlags.WaitSignal) != 0)
				{
					navigation.m_TargetPosition = transform.m_Position;
					navigation.m_TargetDirection = default(float2);
					navigation.m_TargetActivity = 0;
					num5 = 0f;
					if (pathOwner.m_ElementIndex < pathElements.Length)
					{
						PathElement pathElement = pathElements[pathOwner.m_ElementIndex];
						if (m_CurveData.HasComponent(pathElement.m_Target))
						{
							float lanePosition = math.select(currentLane.m_LanePosition, 0f - currentLane.m_LanePosition, (currentLane.m_Flags & CreatureLaneFlags.Backward) != 0 != pathElement.m_TargetDelta.y < pathElement.m_TargetDelta.x);
							Segment val3 = CalculateTargetPos(prefabObjectGeometryData, pathElement.m_Target, pathElement.m_TargetDelta, lanePosition);
							navigation.m_TargetPosition = val3.a;
							navigation.m_TargetDirection = math.normalizesafe(((float3)(ref val3.b)).xz - ((float3)(ref val3.a)).xz, default(float2));
							num5 = math.distance(transform.m_Position, navigation.m_TargetPosition);
						}
					}
				}
				else
				{
					navigation.m_TargetPosition = CalculateTargetPos(prefabObjectGeometryData, currentLane.m_Lane, currentLane.m_CurvePosition.x, currentLane.m_LanePosition);
					navigation.m_TargetDirection = default(float2);
					navigation.m_TargetActivity = 0;
					num5 = math.distance(transform.m_Position, navigation.m_TargetPosition);
				}
				float brakingDistance = CreatureUtils.GetBrakingDistance(prefabHumanData, navigation.m_MaxSpeed, num);
				float num6 = math.max(0f, num5 - num4);
				if (num6 < brakingDistance)
				{
					float maxBrakingSpeed2 = CreatureUtils.GetMaxBrakingSpeed(prefabHumanData, num6, num);
					maxBrakingSpeed2 = MathUtils.Clamp(maxBrakingSpeed2, val);
					navigation.m_MaxSpeed = math.min(navigation.m_MaxSpeed, maxBrakingSpeed2);
				}
			}
			navigation.m_MaxSpeed = math.select(navigation.m_MaxSpeed, 0f, navigation.m_MaxSpeed < 0.1f);
			Entity blocker2 = blocker.m_Blocker;
			float maxSpeed = navigation.m_MaxSpeed;
			blocker.m_Blocker = Entity.Null;
			blocker.m_Type = BlockerType.None;
			currentLane.m_QueueEntity = Entity.Null;
			currentLane.m_QueueArea = default(Sphere3);
			float num7 = num4 + math.max(1f, navigation.m_MaxSpeed * num) + CreatureUtils.GetBrakingDistance(prefabHumanData, navigation.m_MaxSpeed, num);
			if (num2 >= 0.1f)
			{
				float num8 = num2 * num;
				float num9 = ((Random)(ref random)).NextFloat(0f, 1f);
				num9 *= num9;
				num9 = math.select(0.5f - num9, num9 - 0.5f, m_LeftHandTraffic != ((currentLane.m_Flags & CreatureLaneFlags.Backward) != 0));
				currentLane.m_LanePosition = math.lerp(currentLane.m_LanePosition, num9, math.min(1f, num8 * 0.01f));
			}
			if (num5 < num7)
			{
				CreatureTargetIterator targetIterator = new CreatureTargetIterator
				{
					m_MovingData = m_MovingData,
					m_CurveData = m_CurveData,
					m_LaneReservationData = m_LaneReservationData,
					m_LaneOverlaps = m_LaneOverlaps,
					m_LaneObjects = m_LaneObjects,
					m_PrefabObjectGeometry = prefabObjectGeometryData,
					m_Blocker = blocker.m_Blocker,
					m_BlockerType = blocker.m_Type,
					m_QueueEntity = currentLane.m_QueueEntity,
					m_QueueArea = currentLane.m_QueueArea
				};
				Position position = default(Position);
				Transform transform2 = default(Transform);
				LaneSignal laneSignal = default(LaneSignal);
				while (true)
				{
					byte activity = 0;
					if ((currentLane.m_Flags & (CreatureLaneFlags.EndReached | CreatureLaneFlags.WaitSignal)) == 0 && currentLane.m_Lane != Entity.Null)
					{
						if ((currentLane.m_Flags & CreatureLaneFlags.TransformTarget) != 0)
						{
							if ((currentLane.m_Flags & CreatureLaneFlags.WaitPosition) != 0)
							{
								if (MoveTransformTarget(entity, prefabRef.m_Prefab, meshGroups, ref random, human, currentVehicle, transform.m_Position, ref navigation.m_TargetPosition, ref navigation.m_TargetDirection, ref activity, 0f, currentLane.m_Lane, prefabCreatureData.m_SupportedActivities))
								{
									navigation.m_TargetPosition = VehicleUtils.GetConnectionParkingPosition(default(Game.Net.ConnectionLane), new Bezier4x3(navigation.m_TargetPosition, navigation.m_TargetPosition, navigation.m_TargetPosition, navigation.m_TargetPosition), currentLane.m_CurvePosition.y);
									navigation.m_TargetDirection = default(float2);
									navigation.m_TargetActivity = 0;
								}
							}
							else if (MoveTransformTarget(entity, prefabRef.m_Prefab, meshGroups, ref random, human, currentVehicle, transform.m_Position, ref navigation.m_TargetPosition, ref navigation.m_TargetDirection, ref activity, num7, currentLane.m_Lane, prefabCreatureData.m_SupportedActivities))
							{
								break;
							}
						}
						else if ((currentLane.m_Flags & CreatureLaneFlags.Connection) != 0 && (currentLane.m_Flags & (CreatureLaneFlags.ParkingSpace | CreatureLaneFlags.WaitPosition)) != 0)
						{
							Curve curve = m_CurveData[currentLane.m_Lane];
							Game.Net.ConnectionLane connectionLane = m_ConnectionLaneData[currentLane.m_Lane];
							navigation.m_TargetPosition = VehicleUtils.GetConnectionParkingPosition(connectionLane, curve.m_Bezier, currentLane.m_CurvePosition.y);
							navigation.m_TargetDirection = default(float2);
							navigation.m_TargetActivity = 0;
						}
						else if ((currentLane.m_Flags & CreatureLaneFlags.Area) != 0)
						{
							navigation.m_TargetActivity = 0;
							float navigationSize = CreatureUtils.GetNavigationSize(prefabObjectGeometryData);
							if (MoveAreaTarget(ref random, transform.m_Position, pathOwner, pathElements, ref navigation.m_TargetPosition, ref navigation.m_TargetDirection, ref activity, num7, currentLane.m_Lane, prefabCreatureData.m_SupportedActivities, ref currentLane.m_CurvePosition, currentLane.m_LanePosition, navigationSize))
							{
								break;
							}
						}
						else
						{
							Curve curve2 = m_CurveData[currentLane.m_Lane];
							PrefabRef prefabRef2 = m_PrefabRefData[currentLane.m_Lane];
							NetLaneData prefabLaneData = m_PrefabLaneData[prefabRef2.m_Prefab];
							float laneOffset = CreatureUtils.GetLaneOffset(prefabObjectGeometryData, prefabLaneData, currentLane.m_LanePosition);
							navigation.m_TargetDirection = default(float2);
							navigation.m_TargetActivity = 0;
							if (MoveLaneTarget(ref targetIterator, currentLane.m_Lane, transform.m_Position, ref navigation.m_TargetPosition, num7, curve2.m_Bezier, ref currentLane.m_CurvePosition, laneOffset))
							{
								break;
							}
						}
					}
					if ((pathOwner.m_State & (PathFlags.Pending | PathFlags.Failed | PathFlags.Obsolete | PathFlags.Updated)) != 0 || isInvolvedInAccident)
					{
						if ((currentLane.m_Flags & CreatureLaneFlags.Action) == 0)
						{
							break;
						}
						if ((currentLane.m_Flags & CreatureLaneFlags.EndReached) != 0)
						{
							if (navigation.m_TargetActivity == 0 || navigation.m_TransformState == TransformState.Idle)
							{
								navigation.m_TargetActivity = 0;
								currentLane.m_Flags |= CreatureLaneFlags.ActivityDone;
							}
						}
						else
						{
							currentLane.m_Flags &= ~CreatureLaneFlags.Action;
						}
						break;
					}
					if ((currentLane.m_Flags & CreatureLaneFlags.EndOfPath) != 0 || pathOwner.m_ElementIndex >= pathElements.Length)
					{
						if (groupMember.m_Leader != Entity.Null)
						{
							if ((currentLane.m_Flags & CreatureLaneFlags.EndOfPath) == 0)
							{
								targetIterator.m_Blocker = groupMember.m_Leader;
								targetIterator.m_BlockerType = BlockerType.Continuing;
							}
							else if (pathOwner.m_ElementIndex < pathElements.Length)
							{
								currentLane.m_Flags &= ~CreatureLaneFlags.EndOfPath;
							}
						}
						else
						{
							currentLane.m_Flags &= ~(CreatureLaneFlags.ParkingSpace | CreatureLaneFlags.Transport | CreatureLaneFlags.Taxi);
							currentLane.m_Flags |= CreatureLaneFlags.EndOfPath;
						}
						num5 = math.distance(transform.m_Position, navigation.m_TargetPosition);
						float num10 = math.select(0.1f, num4 + 1f, (currentLane.m_Flags & CreatureLaneFlags.TransformTarget) == 0);
						if (!(num5 < num10) || !(num2 < 0.1f))
						{
							break;
						}
						if ((currentLane.m_Flags & CreatureLaneFlags.EndReached) != 0)
						{
							if (navigation.m_TargetActivity == 0)
							{
								currentLane.m_Flags |= CreatureLaneFlags.ActivityDone;
							}
							break;
						}
						navigation.m_TargetActivity = activity;
						currentLane.m_Flags |= CreatureLaneFlags.EndReached;
						if (navigation.m_TargetActivity == 0)
						{
							currentLane.m_Flags |= CreatureLaneFlags.ActivityDone;
						}
						break;
					}
					PathElement pathElement2 = pathElements[pathOwner.m_ElementIndex];
					CreatureLaneFlags creatureLaneFlags = ((pathElement2.m_TargetDelta.y < pathElement2.m_TargetDelta.x) ? CreatureLaneFlags.Backward : ((CreatureLaneFlags)0u));
					if ((pathElement2.m_Flags & PathElementFlags.Leader) != 0)
					{
						creatureLaneFlags |= CreatureLaneFlags.Leader;
					}
					if ((pathElement2.m_Flags & PathElementFlags.Hangaround) != 0)
					{
						creatureLaneFlags |= CreatureLaneFlags.Hangaround;
					}
					currentLane.m_Flags &= ~(CreatureLaneFlags.ParkingSpace | CreatureLaneFlags.Transport | CreatureLaneFlags.Taxi | CreatureLaneFlags.Action);
					if ((pathElement2.m_Flags & PathElementFlags.Action) != 0)
					{
						currentLane.m_Flags |= CreatureLaneFlags.Action;
						num5 = math.distance(transform.m_Position, navigation.m_TargetPosition);
						float num11 = math.select(0.1f, num4 + 1f, (currentLane.m_Flags & CreatureLaneFlags.TransformTarget) == 0);
						if (!(num5 < num11) || !(num2 < 0.1f))
						{
							break;
						}
						if ((currentLane.m_Flags & CreatureLaneFlags.EndReached) != 0)
						{
							if (navigation.m_TargetActivity == 0 || navigation.m_TransformState == TransformState.Idle)
							{
								navigation.m_TargetActivity = 0;
								currentLane.m_Flags |= CreatureLaneFlags.ActivityDone;
							}
						}
						else
						{
							SetActionTarget(ref navigation, transform, human, pathElement2);
							currentLane.m_Flags &= ~CreatureLaneFlags.ActivityDone;
							currentLane.m_Flags |= CreatureLaneFlags.EndReached;
						}
						break;
					}
					if (!m_PedestrianLaneData.HasComponent(pathElement2.m_Target))
					{
						if (m_ParkingLaneData.HasComponent(pathElement2.m_Target))
						{
							currentLane.m_Flags |= CreatureLaneFlags.ParkingSpace;
							if ((pathElement2.m_Flags & PathElementFlags.Secondary) != 0 && (m_ParkingLaneData[pathElement2.m_Target].m_Flags & ParkingLaneFlags.SecondaryStart) == 0)
							{
								currentLane.m_Flags |= CreatureLaneFlags.Taxi;
							}
							num5 = math.distance(transform.m_Position, navigation.m_TargetPosition);
							float num12 = math.select(0.1f, num4 + 1f, (currentLane.m_Flags & CreatureLaneFlags.TransformTarget) == 0);
							if (num5 < num12 && num2 < 0.1f)
							{
								currentLane.m_Flags |= CreatureLaneFlags.EndReached;
							}
							break;
						}
						if (m_WaypointData.HasComponent(pathElement2.m_Target) || m_TaxiStandData.HasComponent(pathElement2.m_Target))
						{
							currentLane.m_Flags |= CreatureLaneFlags.Transport;
							if ((currentLane.m_Flags & CreatureLaneFlags.EndReached) != 0)
							{
								break;
							}
							num5 = math.distance(transform.m_Position, navigation.m_TargetPosition);
							float num13 = math.select(0.1f, num4 + 1f, (currentLane.m_Flags & CreatureLaneFlags.TransformTarget) == 0);
							if (!(num5 < num13) || !(num2 < 0.1f))
							{
								break;
							}
							if ((currentLane.m_Flags & CreatureLaneFlags.TransformTarget) == 0)
							{
								Entity val4 = pathElement2.m_Target;
								if (m_ConnectedData.HasComponent(val4))
								{
									val4 = m_ConnectedData[val4].m_Connected;
								}
								byte activity2 = 0;
								float3 targetPosition = navigation.m_TargetPosition;
								float2 targetDirection = navigation.m_TargetDirection;
								MoveTransformTarget(entity, prefabRef.m_Prefab, meshGroups, ref random, human, currentVehicle, transform.m_Position, ref targetPosition, ref targetDirection, ref activity2, num7, val4, prefabCreatureData.m_SupportedActivities);
								if (activity2 != 0)
								{
									currentLane.m_Lane = val4;
									currentLane.m_CurvePosition = float2.op_Implicit(0f);
									currentLane.m_Flags = CreatureLaneFlags.TransformTarget;
									navigation.m_TargetPosition = targetPosition;
									navigation.m_TargetDirection = targetDirection;
									continue;
								}
							}
							if (activity == 0)
							{
								navigation.m_TargetPosition = transform.m_Position;
								if (m_PositionData.TryGetComponent(pathElement2.m_Target, ref position))
								{
									navigation.m_TargetDirection = math.normalizesafe(((float3)(ref position.m_Position)).xz - ((float3)(ref transform.m_Position)).xz, default(float2));
								}
								else if (m_TransformData.TryGetComponent(pathElement2.m_Target, ref transform2))
								{
									navigation.m_TargetDirection = math.normalizesafe(((float3)(ref transform2.m_Position)).xz - ((float3)(ref transform.m_Position)).xz, default(float2));
								}
							}
							navigation.m_TargetActivity = activity;
							currentLane.m_Flags |= CreatureLaneFlags.EndReached;
							break;
						}
						if (m_ConnectionLaneData.HasComponent(pathElement2.m_Target))
						{
							Game.Net.ConnectionLane connectionLane2 = m_ConnectionLaneData[pathElement2.m_Target];
							if ((connectionLane2.m_Flags & ConnectionLaneFlags.Parking) != 0)
							{
								currentLane.m_Flags |= CreatureLaneFlags.ParkingSpace;
								if ((pathElement2.m_Flags & PathElementFlags.Secondary) != 0)
								{
									currentLane.m_Flags |= CreatureLaneFlags.Taxi;
								}
								num5 = math.distance(transform.m_Position, navigation.m_TargetPosition);
								float num14 = math.select(0.1f, num4 + 1f, (currentLane.m_Flags & CreatureLaneFlags.TransformTarget) == 0);
								if (num5 < num14 && num2 < 0.1f)
								{
									currentLane.m_Flags |= CreatureLaneFlags.EndReached;
								}
								break;
							}
							if ((pathElement2.m_Flags & PathElementFlags.WaitPosition) != 0)
							{
								creatureLaneFlags |= CreatureLaneFlags.WaitPosition;
							}
							if ((connectionLane2.m_Flags & ConnectionLaneFlags.Area) != 0)
							{
								creatureLaneFlags |= CreatureLaneFlags.Area;
								if (m_OwnerData.HasComponent(pathElement2.m_Target))
								{
									Owner owner = m_OwnerData[pathElement2.m_Target];
									if (m_HangaroundLocationData.HasComponent(owner.m_Owner))
									{
										creatureLaneFlags |= CreatureLaneFlags.Hangaround;
									}
								}
							}
							else
							{
								creatureLaneFlags |= CreatureLaneFlags.Connection;
							}
						}
						else
						{
							if (m_SpawnLocationData.HasComponent(pathElement2.m_Target))
							{
								pathOwner.m_ElementIndex++;
								currentLane.m_Lane = pathElement2.m_Target;
								currentLane.m_CurvePosition = float2.op_Implicit(0f);
								currentLane.m_Flags = CreatureLaneFlags.TransformTarget;
								if (m_ActivityLocationData.HasComponent(pathElement2.m_Target))
								{
									currentLane.m_Flags |= CreatureLaneFlags.Hangaround;
								}
								if ((pathElement2.m_Flags & PathElementFlags.WaitPosition) != 0)
								{
									currentLane.m_CurvePosition = float2.op_Implicit(pathElement2.m_TargetDelta.y);
									currentLane.m_Flags |= CreatureLaneFlags.WaitPosition;
								}
								if (pathOwner.m_ElementIndex >= pathElements.Length)
								{
									currentLane.m_Flags |= CreatureLaneFlags.EndOfPath;
								}
								continue;
							}
							if (m_TakeoffLocationData.HasComponent(pathElement2.m_Target))
							{
								pathOwner.m_ElementIndex++;
								continue;
							}
							if (GetTransformTarget(ref currentLane.m_Lane, pathElement2.m_Target))
							{
								pathOwner.m_ElementIndex++;
								navigation.m_TargetActivity = 0;
								currentLane.m_CurvePosition = float2.op_Implicit(0f);
								currentLane.m_Flags = CreatureLaneFlags.EndOfPath | CreatureLaneFlags.TransformTarget;
								continue;
							}
						}
					}
					else if (pathElement2.m_Target != currentLane.m_Lane && (human.m_Flags & HumanFlags.Emergency) == 0 && m_LaneSignalData.TryGetComponent(pathElement2.m_Target, ref laneSignal))
					{
						m_LaneSignals.Enqueue(new HumanNavigationHelpers.LaneSignal(entity, pathElement2.m_Target, 100));
						if (laneSignal.m_Signal == LaneSignalType.Stop || laneSignal.m_Signal == LaneSignalType.SafeStop)
						{
							currentLane.m_Flags |= CreatureLaneFlags.WaitSignal;
							float lanePosition2 = math.select(currentLane.m_LanePosition, 0f - currentLane.m_LanePosition, ((currentLane.m_Flags ^ creatureLaneFlags) & CreatureLaneFlags.Backward) != 0);
							Segment val5 = CalculateTargetPos(prefabObjectGeometryData, pathElement2.m_Target, pathElement2.m_TargetDelta, lanePosition2);
							navigation.m_TargetPosition = val5.a;
							navigation.m_TargetDirection = math.normalizesafe(((float3)(ref val5.b)).xz - ((float3)(ref val5.a)).xz, default(float2));
							navigation.m_TargetActivity = 0;
							targetIterator.m_Blocker = laneSignal.m_Blocker;
							targetIterator.m_BlockerType = BlockerType.Signal;
							targetIterator.m_QueueEntity = pathElement2.m_Target;
							targetIterator.m_QueueArea = CreatureUtils.GetQueueArea(prefabObjectGeometryData, val5.a, val5.b);
							break;
						}
					}
					if (((currentLane.m_Flags & ~creatureLaneFlags & CreatureLaneFlags.Connection) != 0 && num5 >= num4 + 1f) || (groupMember.m_Leader != Entity.Null && (currentLane.m_Flags & CreatureLaneFlags.Leader) != 0))
					{
						break;
					}
					pathOwner.m_ElementIndex++;
					if (!m_CurveData.HasComponent(pathElement2.m_Target))
					{
						pathElements.Clear();
						pathOwner.m_ElementIndex = 0;
						if (groupMember.m_Leader == Entity.Null)
						{
							pathOwner.m_State |= PathFlags.Obsolete;
						}
						break;
					}
					for (int i = 0; i < queues.Length; i++)
					{
						if (queues[i].m_TargetEntity == currentLane.m_Lane)
						{
							queues.RemoveAt(i--);
						}
					}
					if (((currentLane.m_Flags ^ creatureLaneFlags) & CreatureLaneFlags.Backward) != 0)
					{
						currentLane.m_LanePosition = 0f - currentLane.m_LanePosition;
					}
					currentLane.m_Lane = pathElement2.m_Target;
					currentLane.m_CurvePosition = pathElement2.m_TargetDelta;
					currentLane.m_Flags = creatureLaneFlags;
				}
				blocker.m_Blocker = targetIterator.m_Blocker;
				blocker.m_Type = targetIterator.m_BlockerType;
				currentLane.m_QueueEntity = targetIterator.m_QueueEntity;
				currentLane.m_QueueArea = targetIterator.m_QueueArea;
			}
			if (groupMember.m_Leader != Entity.Null)
			{
				Transform transform3 = default(Transform);
				if ((currentLane.m_Flags & CreatureLaneFlags.Leader) != 0 && m_TransformData.TryGetComponent(groupMember.m_Leader, ref transform3))
				{
					Segment val6 = new Segment(transform.m_Position, navigation.m_TargetPosition);
					float num15 = default(float);
					MathUtils.Distance(val6, transform3.m_Position, ref num15);
					float distance2 = MathUtils.Length(val6) * num15;
					float maxBrakingSpeed3 = CreatureUtils.GetMaxBrakingSpeed(prefabHumanData, distance2, num);
					maxBrakingSpeed3 = MathUtils.Clamp(maxBrakingSpeed3, val);
					if (maxBrakingSpeed3 < navigation.m_MaxSpeed)
					{
						navigation.m_MaxSpeed = math.min(navigation.m_MaxSpeed, maxBrakingSpeed3);
						blocker.m_Blocker = groupMember.m_Leader;
					}
				}
				if (blocker.m_Blocker == groupMember.m_Leader && currentLane.m_QueueArea.radius <= 0f)
				{
					Creature creature = m_CreatureData[groupMember.m_Leader];
					if (creature.m_QueueArea.radius > 0f)
					{
						Sphere3 queueArea = CreatureUtils.GetQueueArea(prefabObjectGeometryData, transform.m_Position, navigation.m_TargetPosition);
						currentLane.m_QueueEntity = creature.m_QueueEntity;
						currentLane.m_QueueArea = MathUtils.Sphere(creature.m_QueueArea, queueArea);
					}
				}
			}
			if (navigation.m_MaxSpeed != 0f || blocker2 != Entity.Null)
			{
				CreatureCollisionIterator creatureCollisionIterator = new CreatureCollisionIterator
				{
					m_OwnerData = m_OwnerData,
					m_TransformData = m_TransformData,
					m_MovingData = m_MovingData,
					m_CreatureData = m_CreatureData,
					m_GroupMemberData = m_GroupMemberData,
					m_WaypointData = m_WaypointData,
					m_TaxiStandData = m_TaxiStandData,
					m_CurveData = m_CurveData,
					m_AreaLaneData = m_AreaLaneData,
					m_PrefabRefData = m_PrefabRefData,
					m_PrefabObjectGeometryData = m_PrefabObjectGeometryData,
					m_PrefabLaneData = m_PrefabLaneData,
					m_LaneObjects = m_LaneObjects,
					m_AreaNodes = m_AreaNodes,
					m_StaticObjectSearchTree = m_StaticObjectSearchTree,
					m_MovingObjectSearchTree = m_MovingObjectSearchTree,
					m_Entity = entity,
					m_Leader = groupMember.m_Leader,
					m_CurrentLane = currentLane.m_Lane,
					m_CurrentVehicle = currentVehicle.m_Vehicle,
					m_CurvePosition = currentLane.m_CurvePosition.y,
					m_TimeStep = num,
					m_PrefabObjectGeometry = prefabObjectGeometryData,
					m_SpeedRange = val,
					m_CurrentPosition = transform.m_Position,
					m_CurrentDirection = math.forward(transform.m_Rotation),
					m_CurrentVelocity = moving.m_Velocity,
					m_TargetDistance = num7,
					m_PathOwner = pathOwner,
					m_PathElements = pathElements,
					m_MinSpeed = ((Random)(ref random)).NextFloat(0.4f, 0.6f),
					m_TargetPosition = navigation.m_TargetPosition,
					m_MaxSpeed = navigation.m_MaxSpeed,
					m_LanePosition = currentLane.m_LanePosition,
					m_Blocker = blocker.m_Blocker,
					m_BlockerType = blocker.m_Type,
					m_QueueEntity = currentLane.m_QueueEntity,
					m_QueueArea = currentLane.m_QueueArea,
					m_Queues = queues
				};
				if (blocker2 != Entity.Null)
				{
					creatureCollisionIterator.IterateBlocker(prefabHumanData, blocker2);
					creatureCollisionIterator.m_MaxSpeed = math.select(creatureCollisionIterator.m_MaxSpeed, 0f, creatureCollisionIterator.m_MaxSpeed < 0.1f);
				}
				if (creatureCollisionIterator.m_MaxSpeed != 0f)
				{
					if ((currentLane.m_Flags & CreatureLaneFlags.Connection) == 0)
					{
						bool isBackward = (currentLane.m_Flags & CreatureLaneFlags.Backward) != 0;
						if ((currentLane.m_Flags & CreatureLaneFlags.WaitSignal) != 0)
						{
							int elementIndex = pathOwner.m_ElementIndex;
							if (elementIndex < pathElements.Length)
							{
								PathElement pathElement3 = pathElements[elementIndex++];
								if (m_CurveData.HasComponent(pathElement3.m_Target) && creatureCollisionIterator.IterateFirstLane(currentLane.m_Lane, pathElement3.m_Target, currentLane.m_CurvePosition, pathElement3.m_TargetDelta, isBackward))
								{
									while (creatureCollisionIterator.IterateNextLane(pathElement3.m_Target, pathElement3.m_TargetDelta) && elementIndex < pathElements.Length)
									{
										pathElement3 = pathElements[elementIndex++];
									}
								}
							}
						}
						else if (creatureCollisionIterator.IterateFirstLane(currentLane.m_Lane, currentLane.m_CurvePosition, isBackward))
						{
							int elementIndex2 = pathOwner.m_ElementIndex;
							if (elementIndex2 < pathElements.Length)
							{
								PathElement pathElement4 = pathElements[elementIndex2++];
								while (creatureCollisionIterator.IterateNextLane(pathElement4.m_Target, pathElement4.m_TargetDelta) && elementIndex2 < pathElements.Length)
								{
									pathElement4 = pathElements[elementIndex2++];
								}
							}
						}
					}
					creatureCollisionIterator.m_MaxSpeed = math.select(creatureCollisionIterator.m_MaxSpeed, 0f, creatureCollisionIterator.m_MaxSpeed < 0.1f);
				}
				navigation.m_TargetPosition = creatureCollisionIterator.m_TargetPosition;
				navigation.m_MaxSpeed = creatureCollisionIterator.m_MaxSpeed;
				currentLane.m_LanePosition = math.lerp(currentLane.m_LanePosition, creatureCollisionIterator.m_LanePosition, 0.5f);
				currentLane.m_QueueEntity = creatureCollisionIterator.m_QueueEntity;
				currentLane.m_QueueArea = creatureCollisionIterator.m_QueueArea;
				blocker.m_Blocker = creatureCollisionIterator.m_Blocker;
				blocker.m_Type = creatureCollisionIterator.m_BlockerType;
				maxSpeed = creatureCollisionIterator.m_MaxSpeed;
			}
			blocker.m_MaxSpeed = (byte)math.clamp(Mathf.RoundToInt(maxSpeed * 45.899998f), 0, 255);
			if ((human.m_Flags & (HumanFlags.Waiting | HumanFlags.Sad | HumanFlags.Happy | HumanFlags.Angry)) != 0 && navigation.m_MaxSpeed < 0.1f && navigation.m_TargetActivity == 0 && ((Random)(ref random)).NextInt(100) == 0)
			{
				navigation.m_TargetActivity = 21;
			}
		}

		private float3 CalculateTargetPos(ObjectGeometryData prefabObjectGeometryData, Entity lane, float curvePosition, float lanePosition)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			Curve curve = m_CurveData[lane];
			PrefabRef prefabRef = m_PrefabRefData[lane];
			NetLaneData prefabLaneData = m_PrefabLaneData[prefabRef.m_Prefab];
			return CreatureUtils.GetLanePosition(laneOffset: CreatureUtils.GetLaneOffset(prefabObjectGeometryData, prefabLaneData, lanePosition), curve: curve.m_Bezier, curvePosition: curvePosition);
		}

		private Segment CalculateTargetPos(ObjectGeometryData prefabObjectGeometryData, Entity lane, float2 curvePosition, float lanePosition)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			Curve curve = m_CurveData[lane];
			PrefabRef prefabRef = m_PrefabRefData[lane];
			NetLaneData prefabLaneData = m_PrefabLaneData[prefabRef.m_Prefab];
			float laneOffset = CreatureUtils.GetLaneOffset(prefabObjectGeometryData, prefabLaneData, lanePosition);
			Segment result = default(Segment);
			result.a = CreatureUtils.GetLanePosition(curve.m_Bezier, curvePosition.x, laneOffset);
			result.b = CreatureUtils.GetLanePosition(curve.m_Bezier, curvePosition.y, laneOffset);
			return result;
		}

		private void SetActionTarget(ref HumanNavigation navigation, Transform transform, Human human, PathElement pathElement)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			if ((human.m_Flags & HumanFlags.Selfies) != 0)
			{
				navigation.m_TargetActivity = 7;
				flag = true;
			}
			if (m_TransformData.HasComponent(pathElement.m_Target))
			{
				Transform transform2 = m_TransformData[pathElement.m_Target];
				if (flag)
				{
					navigation.m_TargetDirection = math.normalizesafe(((float3)(ref transform.m_Position)).xz - ((float3)(ref transform2.m_Position)).xz, default(float2));
				}
				else
				{
					navigation.m_TargetDirection = math.normalizesafe(((float3)(ref transform2.m_Position)).xz - ((float3)(ref transform.m_Position)).xz, default(float2));
				}
			}
		}

		private bool MoveAreaTarget(ref Random random, float3 comparePosition, PathOwner pathOwner, DynamicBuffer<PathElement> pathElements, ref float3 targetPosition, ref float2 targetDirection, ref byte activity, float minDistance, Entity target, ActivityMask activityMask, ref float2 curveDelta, float lanePosition, float navigationSize)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0454: Unknown result type (might be due to invalid IL or missing references)
			//IL_0456: Unknown result type (might be due to invalid IL or missing references)
			//IL_0458: Unknown result type (might be due to invalid IL or missing references)
			//IL_045a: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Unknown result type (might be due to invalid IL or missing references)
			//IL_047e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0484: Unknown result type (might be due to invalid IL or missing references)
			//IL_048a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0490: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_039a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_041e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0424: Unknown result type (might be due to invalid IL or missing references)
			//IL_042a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0430: Unknown result type (might be due to invalid IL or missing references)
			//IL_0436: Unknown result type (might be due to invalid IL or missing references)
			if ((pathOwner.m_State & (PathFlags.Pending | PathFlags.Obsolete | PathFlags.Updated)) != 0)
			{
				return true;
			}
			Entity owner = m_OwnerData[target].m_Owner;
			AreaLane areaLane = m_AreaLaneData[target];
			DynamicBuffer<Game.Areas.Node> nodes = m_AreaNodes[owner];
			bool flag = curveDelta.y < curveDelta.x;
			targetDirection = default(float2);
			activity = 0;
			if (areaLane.m_Nodes.y == areaLane.m_Nodes.z)
			{
				float3 position = nodes[areaLane.m_Nodes.x].m_Position;
				float3 position2 = nodes[areaLane.m_Nodes.y].m_Position;
				float3 position3 = nodes[areaLane.m_Nodes.w].m_Position;
				if (CreatureUtils.SetTriangleTarget(position, position2, position3, comparePosition, default(PathElement), pathOwner.m_ElementIndex, pathElements, ref targetPosition, minDistance, lanePosition, curveDelta.y, navigationSize, isSingle: true, m_TransformData, m_TaxiStandData, m_AreaLaneData, m_CurveData))
				{
					return true;
				}
				curveDelta.x = curveDelta.y;
			}
			else
			{
				bool4 val = default(bool4);
				((bool4)(ref val))._002Ector(curveDelta < 0.5f, curveDelta > 0.5f);
				int2 val2 = math.select(int2.op_Implicit(areaLane.m_Nodes.x), int2.op_Implicit(areaLane.m_Nodes.w), ((bool4)(ref val)).zw);
				float3 position4 = nodes[val2.x].m_Position;
				float3 position5 = nodes[areaLane.m_Nodes.y].m_Position;
				float3 position6 = nodes[areaLane.m_Nodes.z].m_Position;
				float3 position7 = nodes[val2.y].m_Position;
				if (math.any(((bool4)(ref val)).xy & ((bool4)(ref val)).wz))
				{
					if (CreatureUtils.SetAreaTarget(position4, position4, position5, position6, position7, owner, nodes, comparePosition, default(PathElement), pathOwner.m_ElementIndex, pathElements, ref targetPosition, minDistance, lanePosition, curveDelta.y, navigationSize, flag, m_TransformData, m_TaxiStandData, m_AreaLaneData, m_CurveData, m_OwnerData))
					{
						return true;
					}
					curveDelta.x = 0.5f;
					((bool4)(ref val)).xz = bool2.op_Implicit(false);
				}
				if (pathElements.Length > pathOwner.m_ElementIndex)
				{
					PathElement pathElement = pathElements[pathOwner.m_ElementIndex];
					Owner owner2 = default(Owner);
					if (m_OwnerData.TryGetComponent(pathElement.m_Target, ref owner2) && owner2.m_Owner == owner)
					{
						bool4 val3 = default(bool4);
						((bool4)(ref val3))._002Ector(pathElement.m_TargetDelta < 0.5f, pathElement.m_TargetDelta > 0.5f);
						if (math.any(!((bool4)(ref val)).xz) & math.any(((bool4)(ref val)).yw) & math.any(((bool4)(ref val3)).xy & ((bool4)(ref val3)).wz))
						{
							AreaLane areaLane2 = m_AreaLaneData[pathElement.m_Target];
							bool flag2 = pathElement.m_TargetDelta.y < pathElement.m_TargetDelta.x;
							lanePosition = math.select(lanePosition, 0f - lanePosition, flag2 != flag);
							val2 = math.select(int2.op_Implicit(areaLane2.m_Nodes.x), int2.op_Implicit(areaLane2.m_Nodes.w), ((bool4)(ref val3)).zw);
							position4 = nodes[val2.x].m_Position;
							if (CreatureUtils.SetAreaTarget(math.select(position5, position6, ((float3)(ref position4)).Equals(position5)), left: nodes[areaLane2.m_Nodes.y].m_Position, right: nodes[areaLane2.m_Nodes.z].m_Position, next: nodes[val2.y].m_Position, prev: position4, areaEntity: owner, nodes: nodes, comparePosition: comparePosition, nextElement: default(PathElement), elementIndex: pathOwner.m_ElementIndex + 1, pathElements: pathElements, targetPosition: ref targetPosition, minDistance: minDistance, lanePosition: lanePosition, curveDelta: pathElement.m_TargetDelta.y, navigationSize: navigationSize, isBackward: flag2, transforms: m_TransformData, taxiStands: m_TaxiStandData, areaLanes: m_AreaLaneData, curves: m_CurveData, owners: m_OwnerData))
							{
								return true;
							}
						}
						curveDelta.x = curveDelta.y;
						return false;
					}
				}
				if (CreatureUtils.SetTriangleTarget(position5, position6, position7, comparePosition, default(PathElement), pathOwner.m_ElementIndex, pathElements, ref targetPosition, minDistance, lanePosition, curveDelta.y, navigationSize, isSingle: false, m_TransformData, m_TaxiStandData, m_AreaLaneData, m_CurveData))
				{
					return true;
				}
				curveDelta.x = curveDelta.y;
			}
			ActivityType activity2 = ActivityType.None;
			CreatureUtils.GetAreaActivity(ref random, ref activity2, target, activityMask, m_OwnerData, m_PrefabRefData, m_PrefabSpawnLocationData);
			if (activity2 != ActivityType.Standing)
			{
				activity = (byte)activity2;
			}
			return math.distance(comparePosition, targetPosition) >= minDistance;
		}

		private bool MoveTransformTarget(Entity creature, Entity creaturePrefab, DynamicBuffer<MeshGroup> meshGroups, ref Random random, Human human, CurrentVehicle currentVehicle, float3 comparePosition, ref float3 targetPosition, ref float2 targetDirection, ref byte activity, float minDistance, Entity target, ActivityMask activityMask)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			Transform result = new Transform
			{
				m_Position = targetPosition
			};
			ActivityType activity2 = ActivityType.None;
			ActivityCondition conditions = CreatureUtils.GetConditions(human);
			if (CreatureUtils.CalculateTransformPosition(creature, creaturePrefab, meshGroups, ref random, ref result, ref activity2, currentVehicle, target, m_LeftHandTraffic, activityMask, conditions, m_MovingObjectSearchTree, ref m_TransformData, ref m_PositionData, ref m_PublicTransportData, ref m_TrainData, ref m_ControllerData, ref m_PrefabRefData, ref m_PrefabBuildingData, ref m_PrefabCarData, ref m_PrefabActivityLocations, ref m_SubMeshGroups, ref m_CharacterElements, ref m_SubMeshes, ref m_AnimationClips, ref m_AnimationMotions))
			{
				targetPosition = result.m_Position;
				if (((quaternion)(ref result.m_Rotation)).Equals(default(quaternion)))
				{
					targetDirection = default(float2);
				}
				else
				{
					float3 val = math.forward(result.m_Rotation);
					targetDirection = math.normalizesafe(((float3)(ref val)).xz, default(float2));
				}
				activity = (byte)activity2;
				return math.distance(comparePosition, targetPosition) >= minDistance;
			}
			return false;
		}

		private bool GetTransformTarget(ref Entity entity, Entity target)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			if (m_PropertyRenterData.HasComponent(target))
			{
				target = m_PropertyRenterData[target].m_Property;
			}
			if (m_TransformData.HasComponent(target))
			{
				entity = target;
				return true;
			}
			if (m_PositionData.HasComponent(target))
			{
				entity = target;
				return true;
			}
			return false;
		}

		private bool MoveLaneTarget(ref CreatureTargetIterator targetIterator, Entity lane, float3 comparePosition, ref float3 targetPosition, float minDistance, Bezier4x3 curve, ref float2 curveDelta, float laneOffset)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			float3 lanePosition = CreatureUtils.GetLanePosition(curve, curveDelta.y, laneOffset);
			if (math.distance(comparePosition, lanePosition) < minDistance)
			{
				if (targetIterator.IterateLane(lane, ref curveDelta.x, curveDelta.y))
				{
					targetPosition = lanePosition;
					return false;
				}
				targetPosition = CreatureUtils.GetLanePosition(curve, curveDelta.x, laneOffset);
				return true;
			}
			float2 val = curveDelta;
			for (int i = 0; i < 8; i++)
			{
				float num = math.lerp(val.x, val.y, 0.5f);
				float3 lanePosition2 = CreatureUtils.GetLanePosition(curve, num, laneOffset);
				if (math.distance(comparePosition, lanePosition2) < minDistance)
				{
					val.x = num;
				}
				else
				{
					val.y = num;
				}
			}
			targetIterator.IterateLane(lane, ref curveDelta.x, val.y);
			targetPosition = CreatureUtils.GetLanePosition(curve, curveDelta.x, laneOffset);
			return true;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct UpdateLaneSignalsJob : IJob
	{
		public NativeQueue<HumanNavigationHelpers.LaneSignal> m_LaneSignalQueue;

		public ComponentLookup<LaneSignal> m_LaneSignalData;

		public void Execute()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			HumanNavigationHelpers.LaneSignal laneSignal = default(HumanNavigationHelpers.LaneSignal);
			while (m_LaneSignalQueue.TryDequeue(ref laneSignal))
			{
				LaneSignal laneSignal2 = m_LaneSignalData[laneSignal.m_Lane];
				if (laneSignal.m_Priority > laneSignal2.m_Priority)
				{
					laneSignal2.m_Petitioner = laneSignal.m_Petitioner;
					laneSignal2.m_Priority = laneSignal.m_Priority;
					m_LaneSignalData[laneSignal.m_Lane] = laneSignal2;
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Moving> __Game_Objects_Moving_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<GroupMember> __Game_Creatures_GroupMember_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Stumbling> __Game_Creatures_Stumbling_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TripSource> __Game_Objects_TripSource_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Human> __Game_Creatures_Human_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentVehicle> __Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InvolvedInAccident> __Game_Events_InvolvedInAccident_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<MeshGroup> __Game_Rendering_MeshGroup_RO_BufferTypeHandle;

		public ComponentTypeHandle<HumanNavigation> __Game_Creatures_HumanNavigation_RW_ComponentTypeHandle;

		public ComponentTypeHandle<HumanCurrentLane> __Game_Creatures_HumanCurrentLane_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Blocker> __Game_Vehicles_Blocker_RW_ComponentTypeHandle;

		public ComponentTypeHandle<PathOwner> __Game_Pathfind_PathOwner_RW_ComponentTypeHandle;

		public BufferTypeHandle<Queue> __Game_Creatures_Queue_RW_BufferTypeHandle;

		public BufferTypeHandle<PathElement> __Game_Pathfind_PathElement_RW_BufferTypeHandle;

		[ReadOnly]
		public EntityStorageInfoLookup __EntityStorageInfoLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.PedestrianLane> __Game_Net_PedestrianLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> __Game_Net_ParkingLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> __Game_Net_ConnectionLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LaneSignal> __Game_Net_LaneSignal_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LaneReservation> __Game_Net_LaneReservation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AreaLane> __Game_Net_AreaLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Waypoint> __Game_Routes_Waypoint_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TaxiStand> __Game_Routes_TaxiStand_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Position> __Game_Routes_Position_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Connected> __Game_Routes_Connected_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Routes.TakeoffLocation> __Game_Routes_TakeoffLocation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Moving> __Game_Objects_Moving_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> __Game_Objects_SpawnLocation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.ActivityLocation> __Game_Objects_ActivityLocation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Creature> __Game_Creatures_Creature_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<GroupMember> __Game_Creatures_GroupMember_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HangaroundLocation> __Game_Areas_HangaroundLocation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PublicTransport> __Game_Vehicles_PublicTransport_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Train> __Game_Vehicles_Train_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Controller> __Game_Vehicles_Controller_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CreatureData> __Game_Prefabs_CreatureData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HumanData> __Game_Prefabs_HumanData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneData> __Game_Prefabs_NetLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> __Game_Prefabs_SpawnLocationData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarData> __Game_Prefabs_CarData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LaneObject> __Game_Net_LaneObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LaneOverlap> __Game_Net_LaneOverlap_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> __Game_Areas_Node_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Triangle> __Game_Areas_Triangle_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ActivityLocationElement> __Game_Prefabs_ActivityLocationElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubMeshGroup> __Game_Prefabs_SubMeshGroup_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<CharacterElement> __Game_Prefabs_CharacterElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<AnimationClip> __Game_Prefabs_AnimationClip_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<AnimationMotion> __Game_Prefabs_AnimationMotion_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubMesh> __Game_Prefabs_SubMesh_RO_BufferLookup;

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
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Objects_Moving_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Moving>(true);
			__Game_Creatures_GroupMember_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<GroupMember>(true);
			__Game_Creatures_Stumbling_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Stumbling>(true);
			__Game_Objects_TripSource_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TripSource>(true);
			__Game_Creatures_Human_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Human>(true);
			__Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentVehicle>(true);
			__Game_Events_InvolvedInAccident_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InvolvedInAccident>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Rendering_MeshGroup_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<MeshGroup>(true);
			__Game_Creatures_HumanNavigation_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HumanNavigation>(false);
			__Game_Creatures_HumanCurrentLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HumanCurrentLane>(false);
			__Game_Vehicles_Blocker_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Blocker>(false);
			__Game_Pathfind_PathOwner_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathOwner>(false);
			__Game_Creatures_Queue_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Queue>(false);
			__Game_Pathfind_PathElement_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<PathElement>(false);
			__EntityStorageInfoLookup = ((SystemState)(ref state)).GetEntityStorageInfoLookup();
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Net_PedestrianLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.PedestrianLane>(true);
			__Game_Net_ParkingLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ParkingLane>(true);
			__Game_Net_ConnectionLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ConnectionLane>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_LaneSignal_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneSignal>(true);
			__Game_Net_LaneReservation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneReservation>(true);
			__Game_Net_AreaLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AreaLane>(true);
			__Game_Routes_Waypoint_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Waypoint>(true);
			__Game_Routes_TaxiStand_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TaxiStand>(true);
			__Game_Routes_Position_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Position>(true);
			__Game_Routes_Connected_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Connected>(true);
			__Game_Routes_TakeoffLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Routes.TakeoffLocation>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Objects_Moving_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Moving>(true);
			__Game_Objects_SpawnLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.SpawnLocation>(true);
			__Game_Objects_ActivityLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.ActivityLocation>(true);
			__Game_Creatures_Creature_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Creature>(true);
			__Game_Creatures_GroupMember_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GroupMember>(true);
			__Game_Areas_HangaroundLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HangaroundLocation>(true);
			__Game_Vehicles_PublicTransport_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.PublicTransport>(true);
			__Game_Vehicles_Train_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Train>(true);
			__Game_Vehicles_Controller_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Controller>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_CreatureData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CreatureData>(true);
			__Game_Prefabs_HumanData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HumanData>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_NetLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneData>(true);
			__Game_Prefabs_SpawnLocationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnLocationData>(true);
			__Game_Prefabs_CarData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarData>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Net_LaneObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneObject>(true);
			__Game_Net_LaneOverlap_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneOverlap>(true);
			__Game_Areas_Node_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.Node>(true);
			__Game_Areas_Triangle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Triangle>(true);
			__Game_Prefabs_ActivityLocationElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ActivityLocationElement>(true);
			__Game_Prefabs_SubMeshGroup_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubMeshGroup>(true);
			__Game_Prefabs_CharacterElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CharacterElement>(true);
			__Game_Prefabs_AnimationClip_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<AnimationClip>(true);
			__Game_Prefabs_AnimationMotion_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<AnimationMotion>(true);
			__Game_Prefabs_SubMesh_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubMesh>(true);
		}
	}

	private SimulationSystem m_SimulationSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private Game.Net.SearchSystem m_NetSearchSystem;

	private Game.Areas.SearchSystem m_AreaSearchSystem;

	private Game.Objects.SearchSystem m_ObjectSearchSystem;

	private Actions m_Actions;

	private EntityQuery m_CreatureQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		m_AreaSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Areas.SearchSystem>();
		m_ObjectSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.SearchSystem>();
		m_Actions = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Actions>();
		m_CreatureQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Human>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.ReadWrite<HumanCurrentLane>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Unknown result type (might be due to invalid IL or missing references)
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_053b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Unknown result type (might be due to invalid IL or missing references)
		//IL_055d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0575: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0592: Unknown result type (might be due to invalid IL or missing references)
		//IL_0597: Unknown result type (might be due to invalid IL or missing references)
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0606: Unknown result type (might be due to invalid IL or missing references)
		//IL_060b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0623: Unknown result type (might be due to invalid IL or missing references)
		//IL_0628: Unknown result type (might be due to invalid IL or missing references)
		//IL_0640: Unknown result type (might be due to invalid IL or missing references)
		//IL_0645: Unknown result type (might be due to invalid IL or missing references)
		//IL_065d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0662: Unknown result type (might be due to invalid IL or missing references)
		//IL_067a: Unknown result type (might be due to invalid IL or missing references)
		//IL_067f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0697: Unknown result type (might be due to invalid IL or missing references)
		//IL_069c: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_070b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0710: Unknown result type (might be due to invalid IL or missing references)
		//IL_0728: Unknown result type (might be due to invalid IL or missing references)
		//IL_072d: Unknown result type (might be due to invalid IL or missing references)
		//IL_075b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0760: Unknown result type (might be due to invalid IL or missing references)
		//IL_0770: Unknown result type (might be due to invalid IL or missing references)
		//IL_0775: Unknown result type (might be due to invalid IL or missing references)
		//IL_0785: Unknown result type (might be due to invalid IL or missing references)
		//IL_078a: Unknown result type (might be due to invalid IL or missing references)
		//IL_079a: Unknown result type (might be due to invalid IL or missing references)
		//IL_079f: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0805: Unknown result type (might be due to invalid IL or missing references)
		//IL_0812: Unknown result type (might be due to invalid IL or missing references)
		//IL_081f: Unknown result type (might be due to invalid IL or missing references)
		//IL_082c: Unknown result type (might be due to invalid IL or missing references)
		//IL_082e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0834: Unknown result type (might be due to invalid IL or missing references)
		uint index = m_SimulationSystem.frameIndex % 16;
		((EntityQuery)(ref m_CreatureQuery)).ResetFilter();
		((EntityQuery)(ref m_CreatureQuery)).SetSharedComponentFilter<UpdateFrame>(new UpdateFrame(index));
		m_Actions.m_LaneSignalQueue = new NativeQueue<HumanNavigationHelpers.LaneSignal>(AllocatorHandle.op_Implicit((Allocator)3));
		JobHandle dependencies;
		JobHandle dependencies2;
		JobHandle dependencies3;
		JobHandle dependencies4;
		JobHandle val = JobChunkExtensions.ScheduleParallel<UpdateNavigationJob>(new UpdateNavigationJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MovingType = InternalCompilerInterface.GetComponentTypeHandle<Moving>(ref __TypeHandle.__Game_Objects_Moving_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_GroupMemberType = InternalCompilerInterface.GetComponentTypeHandle<GroupMember>(ref __TypeHandle.__Game_Creatures_GroupMember_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_StumblingType = InternalCompilerInterface.GetComponentTypeHandle<Stumbling>(ref __TypeHandle.__Game_Creatures_Stumbling_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TripSourceType = InternalCompilerInterface.GetComponentTypeHandle<TripSource>(ref __TypeHandle.__Game_Objects_TripSource_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HumanType = InternalCompilerInterface.GetComponentTypeHandle<Human>(ref __TypeHandle.__Game_Creatures_Human_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentVehicleType = InternalCompilerInterface.GetComponentTypeHandle<CurrentVehicle>(ref __TypeHandle.__Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InvolvedInAccidentType = InternalCompilerInterface.GetComponentTypeHandle<InvolvedInAccident>(ref __TypeHandle.__Game_Events_InvolvedInAccident_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MeshGroupType = InternalCompilerInterface.GetBufferTypeHandle<MeshGroup>(ref __TypeHandle.__Game_Rendering_MeshGroup_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NavigationType = InternalCompilerInterface.GetComponentTypeHandle<HumanNavigation>(ref __TypeHandle.__Game_Creatures_HumanNavigation_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<HumanCurrentLane>(ref __TypeHandle.__Game_Creatures_HumanCurrentLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BlockerType = InternalCompilerInterface.GetComponentTypeHandle<Blocker>(ref __TypeHandle.__Game_Vehicles_Blocker_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathOwnerType = InternalCompilerInterface.GetComponentTypeHandle<PathOwner>(ref __TypeHandle.__Game_Pathfind_PathOwner_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_QueueType = InternalCompilerInterface.GetBufferTypeHandle<Queue>(ref __TypeHandle.__Game_Creatures_Queue_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathElementType = InternalCompilerInterface.GetBufferTypeHandle<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EntityLookup = InternalCompilerInterface.GetEntityStorageInfoLookup(ref __TypeHandle.__EntityStorageInfoLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PedestrianLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.PedestrianLane>(ref __TypeHandle.__Game_Net_PedestrianLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkingLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ParkingLane>(ref __TypeHandle.__Game_Net_ParkingLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectionLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneSignalData = InternalCompilerInterface.GetComponentLookup<LaneSignal>(ref __TypeHandle.__Game_Net_LaneSignal_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneReservationData = InternalCompilerInterface.GetComponentLookup<LaneReservation>(ref __TypeHandle.__Game_Net_LaneReservation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaLaneData = InternalCompilerInterface.GetComponentLookup<AreaLane>(ref __TypeHandle.__Game_Net_AreaLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WaypointData = InternalCompilerInterface.GetComponentLookup<Waypoint>(ref __TypeHandle.__Game_Routes_Waypoint_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TaxiStandData = InternalCompilerInterface.GetComponentLookup<TaxiStand>(ref __TypeHandle.__Game_Routes_TaxiStand_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PositionData = InternalCompilerInterface.GetComponentLookup<Position>(ref __TypeHandle.__Game_Routes_Position_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedData = InternalCompilerInterface.GetComponentLookup<Connected>(ref __TypeHandle.__Game_Routes_Connected_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TakeoffLocationData = InternalCompilerInterface.GetComponentLookup<Game.Routes.TakeoffLocation>(ref __TypeHandle.__Game_Routes_TakeoffLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyRenterData = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MovingData = InternalCompilerInterface.GetComponentLookup<Moving>(ref __TypeHandle.__Game_Objects_Moving_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnLocationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ActivityLocationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.ActivityLocation>(ref __TypeHandle.__Game_Objects_ActivityLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CreatureData = InternalCompilerInterface.GetComponentLookup<Creature>(ref __TypeHandle.__Game_Creatures_Creature_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GroupMemberData = InternalCompilerInterface.GetComponentLookup<GroupMember>(ref __TypeHandle.__Game_Creatures_GroupMember_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HangaroundLocationData = InternalCompilerInterface.GetComponentLookup<HangaroundLocation>(ref __TypeHandle.__Game_Areas_HangaroundLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PublicTransportData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.PublicTransport>(ref __TypeHandle.__Game_Vehicles_PublicTransport_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrainData = InternalCompilerInterface.GetComponentLookup<Train>(ref __TypeHandle.__Game_Vehicles_Train_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ControllerData = InternalCompilerInterface.GetComponentLookup<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCreatureData = InternalCompilerInterface.GetComponentLookup<CreatureData>(ref __TypeHandle.__Game_Prefabs_CreatureData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabHumanData = InternalCompilerInterface.GetComponentLookup<HumanData>(ref __TypeHandle.__Game_Prefabs_HumanData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabBuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabLaneData = InternalCompilerInterface.GetComponentLookup<NetLaneData>(ref __TypeHandle.__Game_Prefabs_NetLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSpawnLocationData = InternalCompilerInterface.GetComponentLookup<SpawnLocationData>(ref __TypeHandle.__Game_Prefabs_SpawnLocationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCarData = InternalCompilerInterface.GetComponentLookup<CarData>(ref __TypeHandle.__Game_Prefabs_CarData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Lanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneObjects = InternalCompilerInterface.GetBufferLookup<LaneObject>(ref __TypeHandle.__Game_Net_LaneObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneOverlaps = InternalCompilerInterface.GetBufferLookup<LaneOverlap>(ref __TypeHandle.__Game_Net_LaneOverlap_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaNodes = InternalCompilerInterface.GetBufferLookup<Game.Areas.Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaTriangles = InternalCompilerInterface.GetBufferLookup<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabActivityLocations = InternalCompilerInterface.GetBufferLookup<ActivityLocationElement>(ref __TypeHandle.__Game_Prefabs_ActivityLocationElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubMeshGroups = InternalCompilerInterface.GetBufferLookup<SubMeshGroup>(ref __TypeHandle.__Game_Prefabs_SubMeshGroup_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CharacterElements = InternalCompilerInterface.GetBufferLookup<CharacterElement>(ref __TypeHandle.__Game_Prefabs_CharacterElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AnimationClips = InternalCompilerInterface.GetBufferLookup<AnimationClip>(ref __TypeHandle.__Game_Prefabs_AnimationClip_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AnimationMotions = InternalCompilerInterface.GetBufferLookup<AnimationMotion>(ref __TypeHandle.__Game_Prefabs_AnimationMotion_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubMeshes = InternalCompilerInterface.GetBufferLookup<SubMesh>(ref __TypeHandle.__Game_Prefabs_SubMesh_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RandomSeed = RandomSeed.Next(),
			m_LeftHandTraffic = m_CityConfigurationSystem.leftHandTraffic,
			m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies),
			m_StaticObjectSearchTree = m_ObjectSearchSystem.GetStaticSearchTree(readOnly: true, out dependencies2),
			m_MovingObjectSearchTree = m_ObjectSearchSystem.GetMovingSearchTree(readOnly: true, out dependencies3),
			m_AreaSearchTree = m_AreaSearchSystem.GetSearchTree(readOnly: true, out dependencies4),
			m_LaneObjectBuffer = m_Actions.m_LaneObjectUpdater.Begin((Allocator)3),
			m_LaneSignals = m_Actions.m_LaneSignalQueue.AsParallelWriter()
		}, m_CreatureQuery, JobUtils.CombineDependencies(((SystemBase)this).Dependency, dependencies, dependencies2, dependencies3, dependencies4));
		m_NetSearchSystem.AddNetSearchTreeReader(val);
		m_ObjectSearchSystem.AddStaticSearchTreeReader(val);
		m_ObjectSearchSystem.AddMovingSearchTreeReader(val);
		m_AreaSearchSystem.AddSearchTreeReader(val);
		m_Actions.m_Dependency = val;
		((SystemBase)this).Dependency = val;
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
	public HumanNavigationSystem()
	{
	}
}
