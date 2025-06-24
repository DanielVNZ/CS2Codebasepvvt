using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Creatures;
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
public class AnimalNavigationSystem : GameSystemBase
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
			public ComponentLookup<HumanCurrentLane> __Game_Creatures_HumanCurrentLane_RO_ComponentLookup;

			public ComponentLookup<AnimalCurrentLane> __Game_Creatures_AnimalCurrentLane_RW_ComponentLookup;

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
				__Game_Creatures_GroupMember_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<GroupMember>(true);
				__Game_Creatures_HumanCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HumanCurrentLane>(true);
				__Game_Creatures_AnimalCurrentLane_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AnimalCurrentLane>(false);
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
				ComponentType.ReadOnly<Animal>(),
				ComponentType.ReadOnly<GroupMember>(),
				ComponentType.ReadOnly<UpdateFrame>(),
				ComponentType.ReadWrite<AnimalCurrentLane>(),
				ComponentType.Exclude<Deleted>(),
				ComponentType.Exclude<Temp>()
			});
		}

		[Preserve]
		protected override void OnUpdate()
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			uint num = m_SimulationSystem.frameIndex % 16;
			if (num == 5 || num == 9 || num == 13)
			{
				((EntityQuery)(ref m_CreatureQuery)).ResetFilter();
				((EntityQuery)(ref m_CreatureQuery)).SetSharedComponentFilter<UpdateFrame>(new UpdateFrame(num));
				JobHandle dependency = JobChunkExtensions.ScheduleParallel<GroupNavigationJob>(new GroupNavigationJob
				{
					m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_GroupMemberType = InternalCompilerInterface.GetComponentTypeHandle<GroupMember>(ref __TypeHandle.__Game_Creatures_GroupMember_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_HumanCurrentLaneData = InternalCompilerInterface.GetComponentLookup<HumanCurrentLane>(ref __TypeHandle.__Game_Creatures_HumanCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_AnimalCurrentLaneData = InternalCompilerInterface.GetComponentLookup<AnimalCurrentLane>(ref __TypeHandle.__Game_Creatures_AnimalCurrentLane_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
				}, m_CreatureQuery, ((SystemBase)this).Dependency);
				((SystemBase)this).Dependency = dependency;
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
		public Groups()
		{
		}
	}

	public class Actions : GameSystemBase
	{
		private SimulationSystem m_SimulationSystem;

		public LaneObjectUpdater m_LaneObjectUpdater;

		public JobHandle m_Dependency;

		[Preserve]
		protected override void OnCreate()
		{
			base.OnCreate();
			m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
			m_LaneObjectUpdater = new LaneObjectUpdater((SystemBase)(object)this);
		}

		[Preserve]
		protected override void OnUpdate()
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			uint num = m_SimulationSystem.frameIndex % 16;
			if (num == 5 || num == 9 || num == 13)
			{
				JobHandle dependencies = JobHandle.CombineDependencies(((SystemBase)this).Dependency, m_Dependency);
				JobHandle dependency = m_LaneObjectUpdater.Apply((SystemBase)(object)this, dependencies);
				((SystemBase)this).Dependency = dependency;
			}
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
		public ComponentLookup<HumanCurrentLane> m_HumanCurrentLaneData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<AnimalCurrentLane> m_AnimalCurrentLaneData;

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
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<GroupMember> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<GroupMember>(ref m_GroupMemberType);
			if (nativeArray.Length == 0)
			{
				return;
			}
			NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			HumanCurrentLane humanCurrentLane = default(HumanCurrentLane);
			AnimalCurrentLane animalCurrentLane2 = default(AnimalCurrentLane);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Entity val = nativeArray2[i];
				GroupMember groupMember = nativeArray[i];
				AnimalCurrentLane animalCurrentLane = m_AnimalCurrentLaneData[val];
				if (m_HumanCurrentLaneData.TryGetComponent(groupMember.m_Leader, ref humanCurrentLane))
				{
					if (humanCurrentLane.m_Lane != animalCurrentLane.m_Lane)
					{
						if (humanCurrentLane.m_Lane != animalCurrentLane.m_NextLane)
						{
							animalCurrentLane.m_NextLane = humanCurrentLane.m_Lane;
							animalCurrentLane.m_NextPosition = humanCurrentLane.m_CurvePosition;
						}
						else
						{
							animalCurrentLane.m_NextPosition.y = humanCurrentLane.m_CurvePosition.y;
						}
						animalCurrentLane.m_NextFlags = (CreatureLaneFlags)((uint)humanCurrentLane.m_Flags & 0xFFFFFDECu);
					}
					else
					{
						if (animalCurrentLane.m_CurvePosition.y != humanCurrentLane.m_CurvePosition.y)
						{
							animalCurrentLane.m_CurvePosition.y = humanCurrentLane.m_CurvePosition.y;
							animalCurrentLane.m_Flags = (CreatureLaneFlags)((uint)humanCurrentLane.m_Flags & 0xFFFFFDECu);
						}
						animalCurrentLane.m_NextLane = Entity.Null;
					}
				}
				else if (m_AnimalCurrentLaneData.TryGetComponent(groupMember.m_Leader, ref animalCurrentLane2))
				{
					if (animalCurrentLane2.m_Lane != animalCurrentLane.m_Lane)
					{
						if (animalCurrentLane2.m_Lane != animalCurrentLane.m_NextLane)
						{
							animalCurrentLane.m_NextLane = animalCurrentLane2.m_Lane;
							animalCurrentLane.m_NextPosition = animalCurrentLane2.m_CurvePosition;
						}
						else
						{
							animalCurrentLane.m_NextPosition.y = animalCurrentLane2.m_CurvePosition.y;
						}
						animalCurrentLane.m_NextFlags = (CreatureLaneFlags)((uint)animalCurrentLane2.m_Flags & 0xFFFFFDECu);
					}
					else
					{
						if (animalCurrentLane.m_CurvePosition.y != animalCurrentLane2.m_CurvePosition.y)
						{
							animalCurrentLane.m_CurvePosition.y = animalCurrentLane2.m_CurvePosition.y;
							animalCurrentLane.m_Flags = (CreatureLaneFlags)((uint)animalCurrentLane2.m_Flags & 0xFFFFFDECu);
						}
						animalCurrentLane.m_NextLane = Entity.Null;
					}
				}
				m_AnimalCurrentLaneData[val] = animalCurrentLane;
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
		public ComponentTypeHandle<Animal> m_AnimalType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<MeshGroup> m_MeshGroupType;

		public ComponentTypeHandle<AnimalNavigation> m_NavigationType;

		public ComponentTypeHandle<Blocker> m_BlockerType;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<HumanCurrentLane> m_HumanCurrentLaneData;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> m_CurrentVehicleData;

		[ReadOnly]
		public ComponentLookup<Game.Net.PedestrianLane> m_PedestrianLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> m_ConnectionLaneData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<LaneReservation> m_LaneReservationData;

		[ReadOnly]
		public ComponentLookup<AreaLane> m_AreaLaneData;

		[ReadOnly]
		public ComponentLookup<Position> m_PositionData;

		[ReadOnly]
		public ComponentLookup<TaxiStand> m_TaxiStandData;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenterData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Moving> m_MovingData;

		[ReadOnly]
		public ComponentLookup<Creature> m_CreatureData;

		[ReadOnly]
		public ComponentLookup<Animal> m_AnimalData;

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
		public ComponentLookup<PathOwner> m_PathOwnerData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<CreatureData> m_PrefabCreatureData;

		[ReadOnly]
		public ComponentLookup<AnimalData> m_PrefabAnimalData;

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
		public BufferLookup<PathElement> m_PathElements;

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

		[NativeDisableParallelForRestriction]
		public ComponentLookup<AnimalCurrentLane> m_AnimalCurrentLaneData;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public bool m_LeftHandTraffic;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[ReadOnly]
		public WaterSurfaceData m_WaterSurfaceData;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_StaticObjectSearchTree;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_MovingObjectSearchTree;

		[ReadOnly]
		public NativeQuadTree<AreaSearchItem, QuadTreeBoundsXZ> m_AreaSearchTree;

		public LaneObjectCommandBuffer m_LaneObjectBuffer;

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
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Transform> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			NativeArray<Moving> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Moving>(ref m_MovingType);
			NativeArray<GroupMember> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<GroupMember>(ref m_GroupMemberType);
			NativeArray<Animal> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Animal>(ref m_AnimalType);
			NativeArray<AnimalNavigation> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AnimalNavigation>(ref m_NavigationType);
			NativeArray<Blocker> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Blocker>(ref m_BlockerType);
			NativeArray<PrefabRef> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<MeshGroup> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<MeshGroup>(ref m_MeshGroupType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			if (((ArchetypeChunk)(ref chunk)).Has<Stumbling>(ref m_StumblingType))
			{
				Moving moving = default(Moving);
				GroupMember groupMember = default(GroupMember);
				for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
				{
					Entity val = nativeArray[i];
					Transform transform = nativeArray2[i];
					Animal animal = nativeArray5[i];
					AnimalNavigation navigation = nativeArray6[i];
					Blocker blocker = nativeArray7[i];
					PrefabRef prefabRef = nativeArray8[i];
					AnimalCurrentLane currentLane = m_AnimalCurrentLaneData[val];
					ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
					CollectionUtils.TryGet<Moving>(nativeArray3, i, ref moving);
					CollectionUtils.TryGet<GroupMember>(nativeArray4, i, ref groupMember);
					AnimalNavigationHelpers.CurrentLaneCache currentLaneCache = new AnimalNavigationHelpers.CurrentLaneCache(ref currentLane, m_PrefabRefData, m_MovingObjectSearchTree);
					UpdateStumbling(val, transform, groupMember, animal, objectGeometryData, ref navigation, ref currentLane, ref blocker);
					currentLaneCache.CheckChanges(val, ref currentLane, m_LaneObjectBuffer, m_LaneObjects, transform, moving, navigation, objectGeometryData);
					nativeArray6[i] = navigation;
					nativeArray7[i] = blocker;
					m_AnimalCurrentLaneData[val] = currentLane;
				}
				return;
			}
			NativeArray<TripSource> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TripSource>(ref m_TripSourceType);
			GroupMember groupMember2 = default(GroupMember);
			TripSource tripSource = default(TripSource);
			DynamicBuffer<MeshGroup> meshGroups = default(DynamicBuffer<MeshGroup>);
			for (int j = 0; j < ((ArchetypeChunk)(ref chunk)).Count; j++)
			{
				Entity val2 = nativeArray[j];
				Transform transform2 = nativeArray2[j];
				Moving moving2 = nativeArray3[j];
				Animal animal2 = nativeArray5[j];
				AnimalNavigation navigation2 = nativeArray6[j];
				Blocker blocker2 = nativeArray7[j];
				PrefabRef prefabRef2 = nativeArray8[j];
				AnimalCurrentLane currentLane2 = m_AnimalCurrentLaneData[val2];
				CreatureData prefabCreatureData = m_PrefabCreatureData[prefabRef2.m_Prefab];
				AnimalData prefabAnimalData = m_PrefabAnimalData[prefabRef2.m_Prefab];
				ObjectGeometryData objectGeometryData2 = m_PrefabObjectGeometryData[prefabRef2.m_Prefab];
				CollectionUtils.TryGet<GroupMember>(nativeArray4, j, ref groupMember2);
				CollectionUtils.TryGet<TripSource>(nativeArray9, j, ref tripSource);
				CollectionUtils.TryGet<MeshGroup>(bufferAccessor, j, ref meshGroups);
				AnimalNavigationHelpers.CurrentLaneCache currentLaneCache2 = new AnimalNavigationHelpers.CurrentLaneCache(ref currentLane2, m_PrefabRefData, m_MovingObjectSearchTree);
				if (currentLane2.m_Lane == Entity.Null || (currentLane2.m_Flags & CreatureLaneFlags.Obsolete) != 0)
				{
					TryFindCurrentLane(ref currentLane2, transform2, animal2);
				}
				UpdateNavigationTarget(ref random, val2, transform2, moving2, tripSource, groupMember2, animal2, prefabRef2, prefabCreatureData, prefabAnimalData, objectGeometryData2, ref navigation2, ref currentLane2, ref blocker2, meshGroups);
				currentLaneCache2.CheckChanges(val2, ref currentLane2, m_LaneObjectBuffer, m_LaneObjects, transform2, moving2, navigation2, objectGeometryData2);
				nativeArray6[j] = navigation2;
				nativeArray7[j] = blocker2;
				m_AnimalCurrentLaneData[val2] = currentLane2;
			}
		}

		private void UpdateStumbling(Entity entity, Transform transform, GroupMember groupMember, Animal animal, ObjectGeometryData prefabObjectGeometryData, ref AnimalNavigation navigation, ref AnimalCurrentLane currentLane, ref Blocker blocker)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			TryFindCurrentLane(ref currentLane, transform, animal);
			navigation = new AnimalNavigation
			{
				m_TargetPosition = transform.m_Position
			};
			blocker = default(Blocker);
		}

		private void TryFindCurrentLane(ref AnimalCurrentLane currentLane, Transform transformData, Animal animal)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			currentLane.m_Flags &= ~CreatureLaneFlags.Obsolete;
			currentLane.m_Lane = Entity.Null;
			currentLane.m_NextLane = Entity.Null;
			if ((animal.m_Flags & AnimalFlags.Roaming) == 0)
			{
				bool flag = (currentLane.m_Flags & CreatureLaneFlags.EmergeUnspawned) != 0;
				currentLane.m_Flags &= ~(CreatureLaneFlags.EndOfPath | CreatureLaneFlags.EndReached | CreatureLaneFlags.TransformTarget | CreatureLaneFlags.ParkingSpace | CreatureLaneFlags.Transport | CreatureLaneFlags.Connection | CreatureLaneFlags.Taxi | CreatureLaneFlags.FindLane | CreatureLaneFlags.Area | CreatureLaneFlags.Hangaround | CreatureLaneFlags.WaitPosition | CreatureLaneFlags.EmergeUnspawned);
				float3 position = transformData.m_Position;
				Bounds3 bounds = default(Bounds3);
				((Bounds3)(ref bounds))._002Ector(position - 100f, position + 100f);
				AnimalNavigationHelpers.FindLaneIterator findLaneIterator = new AnimalNavigationHelpers.FindLaneIterator
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
				m_NetSearchTree.Iterate<AnimalNavigationHelpers.FindLaneIterator>(ref findLaneIterator, 0);
				m_StaticObjectSearchTree.Iterate<AnimalNavigationHelpers.FindLaneIterator>(ref findLaneIterator, 0);
				if (!flag)
				{
					m_AreaSearchTree.Iterate<AnimalNavigationHelpers.FindLaneIterator>(ref findLaneIterator, 0);
				}
				currentLane = findLaneIterator.m_Result;
			}
		}

		private void UpdateNavigationTarget(ref Random random, Entity entity, Transform transform, Moving moving, TripSource tripSource, GroupMember groupMember, Animal animal, PrefabRef prefabRef, CreatureData prefabCreatureData, AnimalData prefabAnimalData, ObjectGeometryData prefabObjectGeometryData, ref AnimalNavigation navigation, ref AnimalCurrentLane currentLane, ref Blocker blocker, DynamicBuffer<MeshGroup> meshGroups)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_056c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0571: Unknown result type (might be due to invalid IL or missing references)
			//IL_057e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0583: Unknown result type (might be due to invalid IL or missing references)
			//IL_0592: Unknown result type (might be due to invalid IL or missing references)
			//IL_0597: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04be: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04de: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_060a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0399: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07da: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0806: Unknown result type (might be due to invalid IL or missing references)
			//IL_080b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0822: Unknown result type (might be due to invalid IL or missing references)
			//IL_0827: Unknown result type (might be due to invalid IL or missing references)
			//IL_0830: Unknown result type (might be due to invalid IL or missing references)
			//IL_0835: Unknown result type (might be due to invalid IL or missing references)
			//IL_062c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0632: Unknown result type (might be due to invalid IL or missing references)
			//IL_0637: Unknown result type (might be due to invalid IL or missing references)
			//IL_063e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0644: Unknown result type (might be due to invalid IL or missing references)
			//IL_0646: Unknown result type (might be due to invalid IL or missing references)
			//IL_064b: Unknown result type (might be due to invalid IL or missing references)
			//IL_064d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0653: Unknown result type (might be due to invalid IL or missing references)
			//IL_0658: Unknown result type (might be due to invalid IL or missing references)
			//IL_065d: Unknown result type (might be due to invalid IL or missing references)
			//IL_065f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0661: Unknown result type (might be due to invalid IL or missing references)
			//IL_061e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0527: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0679: Unknown result type (might be due to invalid IL or missing references)
			//IL_068d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0691: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0414: Unknown result type (might be due to invalid IL or missing references)
			//IL_0420: Unknown result type (might be due to invalid IL or missing references)
			//IL_043d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_044b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0450: Unknown result type (might be due to invalid IL or missing references)
			//IL_0454: Unknown result type (might be due to invalid IL or missing references)
			//IL_0456: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0462: Unknown result type (might be due to invalid IL or missing references)
			//IL_0464: Unknown result type (might be due to invalid IL or missing references)
			//IL_0469: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0470: Unknown result type (might be due to invalid IL or missing references)
			//IL_0477: Unknown result type (might be due to invalid IL or missing references)
			//IL_047d: Unknown result type (might be due to invalid IL or missing references)
			//IL_047f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0484: Unknown result type (might be due to invalid IL or missing references)
			//IL_048a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0491: Unknown result type (might be due to invalid IL or missing references)
			//IL_1250: Unknown result type (might be due to invalid IL or missing references)
			//IL_1255: Unknown result type (might be due to invalid IL or missing references)
			//IL_125d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1262: Unknown result type (might be due to invalid IL or missing references)
			//IL_126a: Unknown result type (might be due to invalid IL or missing references)
			//IL_126f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1277: Unknown result type (might be due to invalid IL or missing references)
			//IL_127c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1284: Unknown result type (might be due to invalid IL or missing references)
			//IL_1289: Unknown result type (might be due to invalid IL or missing references)
			//IL_1291: Unknown result type (might be due to invalid IL or missing references)
			//IL_1296: Unknown result type (might be due to invalid IL or missing references)
			//IL_129e: Unknown result type (might be due to invalid IL or missing references)
			//IL_12a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_12ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_12b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_12b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_12bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_12c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_12ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_12d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_12d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_12df: Unknown result type (might be due to invalid IL or missing references)
			//IL_12e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_12ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_12f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_12f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_12fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_1305: Unknown result type (might be due to invalid IL or missing references)
			//IL_1306: Unknown result type (might be due to invalid IL or missing references)
			//IL_130f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1314: Unknown result type (might be due to invalid IL or missing references)
			//IL_132c: Unknown result type (might be due to invalid IL or missing references)
			//IL_132d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1335: Unknown result type (might be due to invalid IL or missing references)
			//IL_133a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1342: Unknown result type (might be due to invalid IL or missing references)
			//IL_1347: Unknown result type (might be due to invalid IL or missing references)
			//IL_134c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1355: Unknown result type (might be due to invalid IL or missing references)
			//IL_135a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1383: Unknown result type (might be due to invalid IL or missing references)
			//IL_1388: Unknown result type (might be due to invalid IL or missing references)
			//IL_13ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_13b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_13c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_13ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_13d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_13dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_13e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_13e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_1234: Unknown result type (might be due to invalid IL or missing references)
			//IL_1236: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06db: Unknown result type (might be due to invalid IL or missing references)
			//IL_13f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bbf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0863: Unknown result type (might be due to invalid IL or missing references)
			//IL_0868: Unknown result type (might be due to invalid IL or missing references)
			//IL_161d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1622: Unknown result type (might be due to invalid IL or missing references)
			//IL_1658: Unknown result type (might be due to invalid IL or missing references)
			//IL_165d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1666: Unknown result type (might be due to invalid IL or missing references)
			//IL_166b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1674: Unknown result type (might be due to invalid IL or missing references)
			//IL_1679: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d1f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d24: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c50: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c55: Unknown result type (might be due to invalid IL or missing references)
			//IL_11d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_11d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_11ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_11b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d3b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0895: Unknown result type (might be due to invalid IL or missing references)
			//IL_154b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1552: Unknown result type (might be due to invalid IL or missing references)
			//IL_1470: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ee6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d52: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c8e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c93: Unknown result type (might be due to invalid IL or missing references)
			//IL_0caa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0caf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cbb: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_156b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1487: Unknown result type (might be due to invalid IL or missing references)
			//IL_1499: Unknown result type (might be due to invalid IL or missing references)
			//IL_149e: Unknown result type (might be due to invalid IL or missing references)
			//IL_14a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_1160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0efd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e96: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e9d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d84: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cda: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b1b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b2f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b43: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b68: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b7b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b81: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b91: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a53: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a61: Unknown result type (might be due to invalid IL or missing references)
			//IL_09cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_09df: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a02: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a07: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a13: Unknown result type (might be due to invalid IL or missing references)
			//IL_0966: Unknown result type (might be due to invalid IL or missing references)
			//IL_0969: Unknown result type (might be due to invalid IL or missing references)
			//IL_096e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0974: Unknown result type (might be due to invalid IL or missing references)
			//IL_098d: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_157f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1591: Unknown result type (might be due to invalid IL or missing references)
			//IL_1596: Unknown result type (might be due to invalid IL or missing references)
			//IL_159b: Unknown result type (might be due to invalid IL or missing references)
			//IL_14d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_1174: Unknown result type (might be due to invalid IL or missing references)
			//IL_1184: Unknown result type (might be due to invalid IL or missing references)
			//IL_1189: Unknown result type (might be due to invalid IL or missing references)
			//IL_1195: Unknown result type (might be due to invalid IL or missing references)
			//IL_119a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ec7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ecc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d9b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0daf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0db4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0db9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cf1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cfb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d01: Unknown result type (might be due to invalid IL or missing references)
			//IL_0abd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ada: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a75: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a86: Unknown result type (might be due to invalid IL or missing references)
			//IL_0916: Unknown result type (might be due to invalid IL or missing references)
			//IL_091d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0924: Unknown result type (might be due to invalid IL or missing references)
			//IL_092b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0930: Unknown result type (might be due to invalid IL or missing references)
			//IL_0941: Unknown result type (might be due to invalid IL or missing references)
			//IL_0946: Unknown result type (might be due to invalid IL or missing references)
			//IL_0952: Unknown result type (might be due to invalid IL or missing references)
			//IL_14e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_14ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_14f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_14fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0de6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab8: Unknown result type (might be due to invalid IL or missing references)
			//IL_15d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_15dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_1113: Unknown result type (might be due to invalid IL or missing references)
			//IL_111a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f42: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f47: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f54: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f6d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f7f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f84: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f88: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f94: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fa0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fa7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fb1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fb6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e22: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e3f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e46: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e4d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e52: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e56: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e58: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e5d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e64: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e66: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e6b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e6d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e72: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e79: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e7f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e81: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e86: Unknown result type (might be due to invalid IL or missing references)
			//IL_1521: Unknown result type (might be due to invalid IL or missing references)
			//IL_1528: Unknown result type (might be due to invalid IL or missing references)
			//IL_1144: Unknown result type (might be due to invalid IL or missing references)
			//IL_1149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fd3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fdc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fe1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fe6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0feb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ff6: Unknown result type (might be due to invalid IL or missing references)
			//IL_1053: Unknown result type (might be due to invalid IL or missing references)
			//IL_101f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1028: Unknown result type (might be due to invalid IL or missing references)
			//IL_102d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1032: Unknown result type (might be due to invalid IL or missing references)
			//IL_1036: Unknown result type (might be due to invalid IL or missing references)
			//IL_103d: Unknown result type (might be due to invalid IL or missing references)
			//IL_105f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1063: Unknown result type (might be due to invalid IL or missing references)
			//IL_1068: Unknown result type (might be due to invalid IL or missing references)
			//IL_106d: Unknown result type (might be due to invalid IL or missing references)
			//IL_106f: Unknown result type (might be due to invalid IL or missing references)
			//IL_10a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_10ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_1085: Unknown result type (might be due to invalid IL or missing references)
			//IL_108a: Unknown result type (might be due to invalid IL or missing references)
			//IL_108e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1093: Unknown result type (might be due to invalid IL or missing references)
			//IL_1098: Unknown result type (might be due to invalid IL or missing references)
			float num = 4f / 15f;
			float num2 = math.length(moving.m_Velocity);
			if ((animal.m_Flags & AnimalFlags.SwimmingTarget) != 0)
			{
				currentLane.m_Flags |= CreatureLaneFlags.Swimming;
			}
			else
			{
				prefabAnimalData.m_SwimDepth.min = 0f;
			}
			if ((animal.m_Flags & AnimalFlags.FlyingTarget) != 0)
			{
				currentLane.m_Flags |= CreatureLaneFlags.Flying;
			}
			else
			{
				prefabAnimalData.m_FlyHeight.min = 0f;
			}
			if ((currentLane.m_Flags & CreatureLaneFlags.Connection) != 0)
			{
				prefabAnimalData.m_MoveSpeed = 277.77777f;
				prefabAnimalData.m_Acceleration = 277.77777f;
			}
			else
			{
				if ((currentLane.m_Flags & CreatureLaneFlags.Swimming) != 0)
				{
					prefabAnimalData.m_MoveSpeed = prefabAnimalData.m_SwimSpeed;
				}
				else if ((currentLane.m_Flags & CreatureLaneFlags.Flying) != 0)
				{
					prefabAnimalData.m_MoveSpeed = prefabAnimalData.m_FlySpeed;
				}
				num2 = math.min(num2, prefabAnimalData.m_MoveSpeed);
			}
			Bounds1 val = default(Bounds1);
			((Bounds1)(ref val))._002Ector(num2 + new float2(0f - prefabAnimalData.m_Acceleration, prefabAnimalData.m_Acceleration) * num);
			float num3 = math.select(prefabAnimalData.m_MoveSpeed * ((Random)(ref random)).NextFloat(0.9f, 1f), 0f, tripSource.m_Source != Entity.Null);
			navigation.m_MaxSpeed = MathUtils.Clamp(num3, val);
			float num4 = math.max(prefabObjectGeometryData.m_Bounds.max.z, (prefabObjectGeometryData.m_Bounds.max.x - prefabObjectGeometryData.m_Bounds.min.x) * 0.5f);
			float num5;
			if ((currentLane.m_Flags & (CreatureLaneFlags.EndReached | CreatureLaneFlags.TransformTarget | CreatureLaneFlags.Area)) != 0 || currentLane.m_Lane == Entity.Null || ((currentLane.m_Flags & CreatureLaneFlags.Connection) != 0 && (currentLane.m_Flags & (CreatureLaneFlags.ParkingSpace | CreatureLaneFlags.WaitPosition)) != 0))
			{
				if ((animal.m_Flags & AnimalFlags.Roaming) != 0 && math.distance(((float3)(ref transform.m_Position)).xz, ((float3)(ref navigation.m_TargetPosition)).xz) < num4 + 1f)
				{
					if ((currentLane.m_Flags & CreatureLaneFlags.Swimming) != 0)
					{
						Bounds1 val2 = WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, navigation.m_TargetPosition) - MathUtils.Invert(prefabAnimalData.m_SwimDepth);
						navigation.m_TargetPosition.y = MathUtils.Clamp(navigation.m_TargetPosition.y, val2);
					}
					else if ((currentLane.m_Flags & CreatureLaneFlags.Flying) != 0)
					{
						Bounds1 val3 = WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, navigation.m_TargetPosition) + prefabAnimalData.m_FlyHeight;
						navigation.m_TargetPosition.y = MathUtils.Clamp(navigation.m_TargetPosition.y, val3);
					}
					else
					{
						navigation.m_TargetPosition.y = TerrainUtils.SampleHeight(ref m_TerrainHeightData, navigation.m_TargetPosition);
					}
				}
				num5 = math.distance(transform.m_Position, navigation.m_TargetPosition);
				float distance = math.select(num5, math.max(0f, num5 - num4), (currentLane.m_Flags & (CreatureLaneFlags.TransformTarget | CreatureLaneFlags.Swimming | CreatureLaneFlags.Flying)) == 0);
				float maxBrakingSpeed = CreatureUtils.GetMaxBrakingSpeed(prefabAnimalData, distance, num);
				maxBrakingSpeed = MathUtils.Clamp(maxBrakingSpeed, val);
				navigation.m_MaxSpeed = math.min(navigation.m_MaxSpeed, maxBrakingSpeed);
			}
			else
			{
				if ((currentLane.m_Flags & CreatureLaneFlags.WaitSignal) != 0)
				{
					navigation.m_TargetPosition = transform.m_Position;
					navigation.m_TargetDirection = default(float3);
					navigation.m_TargetActivity = 0;
					num5 = 0f;
					if (m_PathOwnerData.HasComponent(groupMember.m_Leader))
					{
						PathOwner pathOwner = m_PathOwnerData[groupMember.m_Leader];
						DynamicBuffer<PathElement> val4 = m_PathElements[groupMember.m_Leader];
						if (pathOwner.m_ElementIndex < val4.Length)
						{
							PathElement pathElement = val4[pathOwner.m_ElementIndex];
							if (m_CurveData.HasComponent(pathElement.m_Target))
							{
								float lanePosition = math.select(currentLane.m_LanePosition, 0f - currentLane.m_LanePosition, (currentLane.m_Flags & CreatureLaneFlags.Backward) != 0 != pathElement.m_TargetDelta.y < pathElement.m_TargetDelta.x);
								Segment val5 = CalculateTargetPos(prefabObjectGeometryData, pathElement.m_Target, pathElement.m_TargetDelta, lanePosition);
								navigation.m_TargetPosition = val5.a;
								navigation.m_TargetDirection = math.normalizesafe(val5.b - val5.a, default(float3));
								num5 = math.distance(transform.m_Position, navigation.m_TargetPosition);
							}
						}
					}
				}
				else
				{
					navigation.m_TargetPosition = CalculateTargetPos(prefabObjectGeometryData, currentLane.m_Lane, currentLane.m_CurvePosition.x, currentLane.m_LanePosition);
					navigation.m_TargetDirection = default(float3);
					navigation.m_TargetActivity = 0;
					num5 = math.distance(transform.m_Position, navigation.m_TargetPosition);
				}
				float brakingDistance = CreatureUtils.GetBrakingDistance(prefabAnimalData, navigation.m_MaxSpeed, num);
				float num6 = math.max(0f, num5 - num4);
				if (num6 < brakingDistance)
				{
					float maxBrakingSpeed2 = CreatureUtils.GetMaxBrakingSpeed(prefabAnimalData, num6, num);
					maxBrakingSpeed2 = MathUtils.Clamp(maxBrakingSpeed2, val);
					navigation.m_MaxSpeed = math.min(navigation.m_MaxSpeed, maxBrakingSpeed2);
				}
			}
			navigation.m_MaxSpeed = math.select(navigation.m_MaxSpeed, 0f, navigation.m_MaxSpeed < 0.1f);
			Entity blocker2 = blocker.m_Blocker;
			float num7 = navigation.m_MaxSpeed;
			blocker.m_Blocker = Entity.Null;
			blocker.m_Type = BlockerType.None;
			currentLane.m_QueueEntity = Entity.Null;
			currentLane.m_QueueArea = default(Sphere3);
			if (m_HumanCurrentLaneData.HasComponent(groupMember.m_Leader) && m_HumanCurrentLaneData[groupMember.m_Leader].m_Lane == currentLane.m_Lane)
			{
				Transform transform2 = m_TransformData[groupMember.m_Leader];
				Moving moving2 = default(Moving);
				if (m_MovingData.HasComponent(groupMember.m_Leader))
				{
					moving2 = m_MovingData[groupMember.m_Leader];
				}
				float3 val6 = math.normalizesafe(navigation.m_TargetPosition - transform.m_Position, default(float3));
				float3 val7 = transform2.m_Position - transform.m_Position;
				if (math.dot(val7, val6) < 0f)
				{
					float distance2 = math.max(0f, 3f - math.length(val7));
					float maxResultSpeed = math.max(0f, math.dot(val6, moving2.m_Velocity));
					float maxBrakingSpeed3 = CreatureUtils.GetMaxBrakingSpeed(prefabAnimalData, distance2, maxResultSpeed, num);
					maxBrakingSpeed3 = MathUtils.Clamp(maxBrakingSpeed3, val);
					if (maxBrakingSpeed3 < navigation.m_MaxSpeed)
					{
						navigation.m_MaxSpeed = maxBrakingSpeed3;
						num7 = maxBrakingSpeed3;
						blocker.m_Blocker = groupMember.m_Leader;
						blocker.m_Type = BlockerType.Continuing;
					}
				}
			}
			float num8 = num4 + math.max(1f, navigation.m_MaxSpeed * num) + CreatureUtils.GetBrakingDistance(prefabAnimalData, navigation.m_MaxSpeed, num);
			float num9 = num4 + 1f;
			if (num2 > 0.01f && (animal.m_Flags & AnimalFlags.Roaming) == 0)
			{
				float num10 = num2 * num;
				float num11 = ((Random)(ref random)).NextFloat(0f, 1f);
				num11 *= num11;
				num11 = math.select(0.5f - num11, num11 - 0.5f, m_LeftHandTraffic != ((currentLane.m_Flags & CreatureLaneFlags.Backward) != 0));
				currentLane.m_LanePosition = math.lerp(currentLane.m_LanePosition, num11, math.min(1f, num10 * 0.01f));
			}
			if (num5 < num8)
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
				Animal animal2 = default(Animal);
				while (true)
				{
					byte activity = 0;
					if ((currentLane.m_Flags & (CreatureLaneFlags.EndReached | CreatureLaneFlags.WaitSignal)) == 0 && (animal.m_Flags & AnimalFlags.Roaming) == 0 && currentLane.m_Lane != Entity.Null)
					{
						if ((currentLane.m_Flags & CreatureLaneFlags.TransformTarget) != 0)
						{
							CurrentVehicle currentVehicle = default(CurrentVehicle);
							if (m_CurrentVehicleData.HasComponent(groupMember.m_Leader))
							{
								currentVehicle = m_CurrentVehicleData[groupMember.m_Leader];
							}
							if ((currentLane.m_Flags & CreatureLaneFlags.WaitPosition) != 0)
							{
								if (MoveTransformTarget(entity, prefabRef.m_Prefab, meshGroups, ref random, currentVehicle, transform.m_Position, ref navigation.m_TargetPosition, ref navigation.m_TargetDirection, ref activity, 0f, currentLane.m_Lane, prefabCreatureData.m_SupportedActivities))
								{
									navigation.m_TargetPosition = VehicleUtils.GetConnectionParkingPosition(default(Game.Net.ConnectionLane), new Bezier4x3(navigation.m_TargetPosition, navigation.m_TargetPosition, navigation.m_TargetPosition, navigation.m_TargetPosition), currentLane.m_CurvePosition.y);
									navigation.m_TargetDirection = default(float3);
									navigation.m_TargetActivity = 0;
								}
							}
							else if (MoveTransformTarget(entity, prefabRef.m_Prefab, meshGroups, ref random, currentVehicle, transform.m_Position, ref navigation.m_TargetPosition, ref navigation.m_TargetDirection, ref activity, num8, currentLane.m_Lane, prefabCreatureData.m_SupportedActivities))
							{
								break;
							}
						}
						else if ((currentLane.m_Flags & CreatureLaneFlags.Connection) != 0 && (currentLane.m_Flags & (CreatureLaneFlags.ParkingSpace | CreatureLaneFlags.WaitPosition)) != 0)
						{
							Curve curve = m_CurveData[currentLane.m_Lane];
							Game.Net.ConnectionLane connectionLane = m_ConnectionLaneData[currentLane.m_Lane];
							navigation.m_TargetPosition = VehicleUtils.GetConnectionParkingPosition(connectionLane, curve.m_Bezier, currentLane.m_CurvePosition.y);
							navigation.m_TargetDirection = default(float3);
							navigation.m_TargetActivity = 0;
						}
						else if ((currentLane.m_Flags & CreatureLaneFlags.Area) != 0)
						{
							navigation.m_TargetActivity = 0;
							float navigationSize = CreatureUtils.GetNavigationSize(prefabObjectGeometryData);
							PathOwner pathOwner2 = default(PathOwner);
							DynamicBuffer<PathElement> pathElements = default(DynamicBuffer<PathElement>);
							if (m_HumanCurrentLaneData.HasComponent(groupMember.m_Leader) && m_HumanCurrentLaneData[groupMember.m_Leader].m_Lane == currentLane.m_Lane)
							{
								pathOwner2 = m_PathOwnerData[groupMember.m_Leader];
								pathElements = m_PathElements[groupMember.m_Leader];
							}
							if (MoveAreaTarget(ref random, transform.m_Position, pathOwner2, pathElements, ref navigation.m_TargetPosition, ref navigation.m_TargetDirection, ref activity, num8, currentLane.m_Lane, currentLane.m_NextLane, prefabCreatureData.m_SupportedActivities, ref currentLane.m_CurvePosition, currentLane.m_NextPosition, currentLane.m_LanePosition, navigationSize))
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
							navigation.m_TargetDirection = default(float3);
							navigation.m_TargetActivity = 0;
							if (MoveLaneTarget(ref targetIterator, currentLane.m_Lane, transform.m_Position, ref navigation.m_TargetPosition, num8, curve2.m_Bezier, ref currentLane.m_CurvePosition, laneOffset))
							{
								break;
							}
						}
					}
					if ((currentLane.m_Flags & CreatureLaneFlags.EndOfPath) != 0)
					{
						num5 = math.distance(transform.m_Position, navigation.m_TargetPosition);
						if ((currentLane.m_Flags & CreatureLaneFlags.EndReached) == 0 && num5 < num9 && num2 < 0.1f)
						{
							navigation.m_TargetActivity = activity;
							currentLane.m_Flags |= CreatureLaneFlags.EndReached;
							if ((animal.m_Flags & AnimalFlags.SwimmingTarget) == 0)
							{
								currentLane.m_Flags &= ~CreatureLaneFlags.Swimming;
							}
							if ((animal.m_Flags & AnimalFlags.FlyingTarget) == 0)
							{
								currentLane.m_Flags &= ~CreatureLaneFlags.Flying;
							}
						}
						break;
					}
					if ((animal.m_Flags & AnimalFlags.Roaming) == 0 && currentLane.m_NextLane != Entity.Null)
					{
						if (((currentLane.m_Flags ^ currentLane.m_NextFlags) & CreatureLaneFlags.Backward) != 0)
						{
							currentLane.m_LanePosition = 0f - currentLane.m_LanePosition;
						}
						currentLane.m_Lane = currentLane.m_NextLane;
						currentLane.m_Flags = currentLane.m_NextFlags;
						currentLane.m_CurvePosition = currentLane.m_NextPosition;
						currentLane.m_NextLane = Entity.Null;
						if ((currentLane.m_Flags & CreatureLaneFlags.Area) == 0 && m_CurveData.HasComponent(currentLane.m_Lane))
						{
							MathUtils.Distance(m_CurveData[currentLane.m_Lane].m_Bezier, transform.m_Position, ref currentLane.m_CurvePosition.x);
						}
						continue;
					}
					if (groupMember.m_Leader != Entity.Null)
					{
						if (m_HumanCurrentLaneData.HasComponent(groupMember.m_Leader))
						{
							if ((m_HumanCurrentLaneData[groupMember.m_Leader].m_Flags & CreatureLaneFlags.WaitSignal) != 0)
							{
								currentLane.m_Flags |= CreatureLaneFlags.WaitSignal;
								if (m_PathOwnerData.HasComponent(groupMember.m_Leader))
								{
									PathOwner pathOwner3 = m_PathOwnerData[groupMember.m_Leader];
									DynamicBuffer<PathElement> val8 = m_PathElements[groupMember.m_Leader];
									if (pathOwner3.m_ElementIndex < val8.Length)
									{
										PathElement pathElement2 = val8[pathOwner3.m_ElementIndex];
										if (m_CurveData.HasComponent(pathElement2.m_Target))
										{
											float lanePosition2 = math.select(currentLane.m_LanePosition, 0f - currentLane.m_LanePosition, (currentLane.m_Flags & CreatureLaneFlags.Backward) != 0 != pathElement2.m_TargetDelta.y < pathElement2.m_TargetDelta.x);
											Segment val9 = CalculateTargetPos(prefabObjectGeometryData, pathElement2.m_Target, pathElement2.m_TargetDelta, lanePosition2);
											navigation.m_TargetPosition = val9.a;
											navigation.m_TargetDirection = math.normalizesafe(val9.b - val9.a, default(float3));
											navigation.m_TargetActivity = 0;
										}
									}
								}
							}
							else
							{
								num5 = math.distance(transform.m_Position, navigation.m_TargetPosition);
								if (num5 < num9 && num2 < 0.1f)
								{
									currentLane.m_Flags |= CreatureLaneFlags.EndReached;
								}
							}
							targetIterator.m_Blocker = groupMember.m_Leader;
							targetIterator.m_BlockerType = BlockerType.Continuing;
							break;
						}
						if (m_AnimalCurrentLaneData.HasComponent(groupMember.m_Leader) && m_AnimalData.TryGetComponent(groupMember.m_Leader, ref animal2))
						{
							if (((animal.m_Flags ^ animal2.m_Flags) & AnimalFlags.Roaming) != 0)
							{
								currentLane.m_Flags |= CreatureLaneFlags.EndReached;
							}
							else if ((animal2.m_Flags & AnimalFlags.Roaming) != 0)
							{
								currentLane.m_Lane = Entity.Null;
								Transform transform3 = m_TransformData[groupMember.m_Leader];
								float2 val10 = MathUtils.RotateLeft(new float2(0f, num4 * -2f), currentLane.m_LanePosition * ((float)Math.PI * 2f));
								float3 val11 = transform3.m_Position + math.mul(transform3.m_Rotation, new float3(val10.x, 0f, val10.y));
								if ((currentLane.m_Flags & CreatureLaneFlags.Swimming) != 0)
								{
									Bounds1 val12 = WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, val11) - MathUtils.Invert(prefabAnimalData.m_SwimDepth);
									val11.y = MathUtils.Clamp(val11.y, val12);
								}
								else if ((currentLane.m_Flags & CreatureLaneFlags.Flying) != 0)
								{
									Bounds1 val13 = WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, val11) + prefabAnimalData.m_FlyHeight;
									val11.y = MathUtils.Clamp(val11.y, val13);
								}
								else
								{
									val11.y = TerrainUtils.SampleHeight(ref m_TerrainHeightData, val11);
								}
								float3 val14 = val11 - navigation.m_TargetPosition;
								if (math.length(val14) >= 0.1f)
								{
									ref float3 targetPosition = ref navigation.m_TargetPosition;
									targetPosition += MathUtils.ClampLength(val14, num8);
									break;
								}
								targetIterator.m_Blocker = groupMember.m_Leader;
								targetIterator.m_BlockerType = BlockerType.Continuing;
								if (num5 < num9 && num2 < 0.1f)
								{
									navigation.m_TargetActivity = 0;
									if ((animal.m_Flags & AnimalFlags.SwimmingTarget) == 0)
									{
										currentLane.m_Flags &= ~CreatureLaneFlags.Swimming;
									}
									if ((animal.m_Flags & AnimalFlags.FlyingTarget) == 0)
									{
										currentLane.m_Flags &= ~CreatureLaneFlags.Flying;
									}
								}
							}
							else
							{
								num5 = math.distance(transform.m_Position, navigation.m_TargetPosition);
								if (num5 < num9 && num2 < 0.1f)
								{
									currentLane.m_Flags |= CreatureLaneFlags.EndReached;
								}
								targetIterator.m_Blocker = groupMember.m_Leader;
								targetIterator.m_BlockerType = BlockerType.Continuing;
							}
							break;
						}
						if (m_CurrentVehicleData.HasComponent(groupMember.m_Leader))
						{
							currentLane.m_Lane = m_CurrentVehicleData[groupMember.m_Leader].m_Vehicle;
							currentLane.m_CurvePosition = float2.op_Implicit(0f);
							currentLane.m_Flags = CreatureLaneFlags.EndOfPath | CreatureLaneFlags.TransformTarget;
							continue;
						}
					}
					if (tripSource.m_Source != Entity.Null)
					{
						break;
					}
					currentLane.m_Flags |= CreatureLaneFlags.EndOfPath;
				}
				blocker.m_Blocker = targetIterator.m_Blocker;
				blocker.m_Type = targetIterator.m_BlockerType;
			}
			if (navigation.m_TargetActivity == 0)
			{
				if ((currentLane.m_Flags & CreatureLaneFlags.Swimming) != 0)
				{
					navigation.m_TargetActivity = 8;
				}
				else if ((currentLane.m_Flags & CreatureLaneFlags.Flying) != 0)
				{
					navigation.m_TargetActivity = 9;
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
					m_TimeStep = num,
					m_PrefabObjectGeometry = prefabObjectGeometryData,
					m_SpeedRange = val,
					m_CurrentPosition = transform.m_Position,
					m_CurrentDirection = math.forward(transform.m_Rotation),
					m_CurrentVelocity = moving.m_Velocity,
					m_TargetDistance = num8,
					m_MinSpeed = ((Random)(ref random)).NextFloat(0.4f, 0.6f),
					m_TargetPosition = navigation.m_TargetPosition,
					m_MaxSpeed = navigation.m_MaxSpeed,
					m_LanePosition = currentLane.m_LanePosition,
					m_Blocker = blocker.m_Blocker,
					m_BlockerType = blocker.m_Type,
					m_QueueEntity = currentLane.m_QueueEntity,
					m_QueueArea = currentLane.m_QueueArea
				};
				if (blocker2 != Entity.Null)
				{
					creatureCollisionIterator.IterateBlocker(prefabAnimalData, blocker2);
					creatureCollisionIterator.m_MaxSpeed = math.select(creatureCollisionIterator.m_MaxSpeed, 0f, creatureCollisionIterator.m_MaxSpeed < 0.1f);
				}
				if (creatureCollisionIterator.m_MaxSpeed != 0f && (currentLane.m_Flags & CreatureLaneFlags.Connection) == 0)
				{
					bool isBackward = (currentLane.m_Flags & CreatureLaneFlags.Backward) != 0;
					if ((currentLane.m_Flags & CreatureLaneFlags.WaitSignal) != 0)
					{
						if (m_PathOwnerData.HasComponent(groupMember.m_Leader))
						{
							PathOwner pathOwner4 = m_PathOwnerData[groupMember.m_Leader];
							DynamicBuffer<PathElement> val15 = m_PathElements[groupMember.m_Leader];
							int elementIndex = pathOwner4.m_ElementIndex;
							if (elementIndex < val15.Length)
							{
								PathElement pathElement3 = val15[elementIndex++];
								if (m_CurveData.HasComponent(pathElement3.m_Target) && creatureCollisionIterator.IterateFirstLane(currentLane.m_Lane, pathElement3.m_Target, currentLane.m_CurvePosition, pathElement3.m_TargetDelta, isBackward))
								{
									while (creatureCollisionIterator.IterateNextLane(pathElement3.m_Target, pathElement3.m_TargetDelta) && elementIndex < val15.Length)
									{
										pathElement3 = val15[elementIndex++];
									}
								}
							}
						}
					}
					else if (creatureCollisionIterator.IterateFirstLane(currentLane.m_Lane, currentLane.m_CurvePosition, isBackward) && m_PathOwnerData.HasComponent(groupMember.m_Leader))
					{
						PathOwner pathOwner5 = m_PathOwnerData[groupMember.m_Leader];
						DynamicBuffer<PathElement> val16 = m_PathElements[groupMember.m_Leader];
						int elementIndex2 = pathOwner5.m_ElementIndex;
						if (elementIndex2 < val16.Length)
						{
							PathElement pathElement4 = val16[elementIndex2++];
							while (creatureCollisionIterator.IterateNextLane(pathElement4.m_Target, pathElement4.m_TargetDelta) && elementIndex2 < val16.Length)
							{
								pathElement4 = val16[elementIndex2++];
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
				num7 = creatureCollisionIterator.m_MaxSpeed;
			}
			blocker.m_MaxSpeed = (byte)math.clamp(Mathf.RoundToInt(num7 * 45.899998f), 0, 255);
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

		private bool MoveAreaTarget(ref Random random, float3 comparePosition, PathOwner pathOwner, DynamicBuffer<PathElement> pathElements, ref float3 targetPosition, ref float3 targetDirection, ref byte activity, float minDistance, Entity target, Entity nextTarget, ActivityMask activityMask, ref float2 curveDelta, float2 nextCurveDelta, float lanePosition, float navigationSize)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_04df: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0489: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			//IL_048d: Unknown result type (might be due to invalid IL or missing references)
			//IL_048f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0498: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0505: Unknown result type (might be due to invalid IL or missing references)
			//IL_0508: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0343: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03da: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0403: Unknown result type (might be due to invalid IL or missing references)
			//IL_0408: Unknown result type (might be due to invalid IL or missing references)
			//IL_040c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0418: Unknown result type (might be due to invalid IL or missing references)
			//IL_041d: Unknown result type (might be due to invalid IL or missing references)
			//IL_041f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0421: Unknown result type (might be due to invalid IL or missing references)
			//IL_0423: Unknown result type (might be due to invalid IL or missing references)
			//IL_0425: Unknown result type (might be due to invalid IL or missing references)
			//IL_0427: Unknown result type (might be due to invalid IL or missing references)
			//IL_0428: Unknown result type (might be due to invalid IL or missing references)
			//IL_0429: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_0459: Unknown result type (might be due to invalid IL or missing references)
			//IL_045f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0465: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Unknown result type (might be due to invalid IL or missing references)
			if ((pathOwner.m_State & (PathFlags.Pending | PathFlags.Obsolete | PathFlags.Updated)) != 0)
			{
				return true;
			}
			Entity owner = m_OwnerData[target].m_Owner;
			AreaLane areaLane = m_AreaLaneData[target];
			DynamicBuffer<Game.Areas.Node> nodes = m_AreaNodes[owner];
			bool flag = curveDelta.y < curveDelta.x;
			PathElement nextElement = new PathElement(nextTarget, nextCurveDelta);
			targetDirection = default(float3);
			activity = 0;
			if (areaLane.m_Nodes.y == areaLane.m_Nodes.z)
			{
				float3 position = nodes[areaLane.m_Nodes.x].m_Position;
				float3 position2 = nodes[areaLane.m_Nodes.y].m_Position;
				float3 position3 = nodes[areaLane.m_Nodes.w].m_Position;
				if (CreatureUtils.SetTriangleTarget(position, position2, position3, comparePosition, nextElement, pathOwner.m_ElementIndex, pathElements, ref targetPosition, minDistance, lanePosition, curveDelta.y, navigationSize, isSingle: true, m_TransformData, m_TaxiStandData, m_AreaLaneData, m_CurveData))
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
					if (CreatureUtils.SetAreaTarget(position4, position4, position5, position6, position7, owner, nodes, comparePosition, nextElement, pathOwner.m_ElementIndex, pathElements, ref targetPosition, minDistance, lanePosition, curveDelta.y, navigationSize, flag, m_TransformData, m_TaxiStandData, m_AreaLaneData, m_CurveData, m_OwnerData))
					{
						return true;
					}
					curveDelta.x = 0.5f;
					((bool4)(ref val)).xz = bool2.op_Implicit(false);
				}
				if (nextElement.m_Target == Entity.Null && pathElements.IsCreated && pathOwner.m_ElementIndex < pathElements.Length)
				{
					nextElement = pathElements[pathOwner.m_ElementIndex++];
				}
				Owner owner2 = default(Owner);
				if (nextElement.m_Target != Entity.Null && m_OwnerData.TryGetComponent(nextElement.m_Target, ref owner2) && owner2.m_Owner == owner)
				{
					bool4 val3 = default(bool4);
					((bool4)(ref val3))._002Ector(nextElement.m_TargetDelta < 0.5f, nextElement.m_TargetDelta > 0.5f);
					if (math.any(!((bool4)(ref val)).xz) & math.any(((bool4)(ref val)).yw) & math.any(((bool4)(ref val3)).xy & ((bool4)(ref val3)).wz))
					{
						AreaLane areaLane2 = m_AreaLaneData[nextElement.m_Target];
						bool flag2 = nextElement.m_TargetDelta.y < nextElement.m_TargetDelta.x;
						lanePosition = math.select(lanePosition, 0f - lanePosition, flag2 != flag);
						val2 = math.select(int2.op_Implicit(areaLane2.m_Nodes.x), int2.op_Implicit(areaLane2.m_Nodes.w), ((bool4)(ref val3)).zw);
						position4 = nodes[val2.x].m_Position;
						if (CreatureUtils.SetAreaTarget(math.select(position5, position6, ((float3)(ref position4)).Equals(position5)), left: nodes[areaLane2.m_Nodes.y].m_Position, right: nodes[areaLane2.m_Nodes.z].m_Position, next: nodes[val2.y].m_Position, prev: position4, areaEntity: owner, nodes: nodes, comparePosition: comparePosition, nextElement: default(PathElement), elementIndex: pathOwner.m_ElementIndex, pathElements: pathElements, targetPosition: ref targetPosition, minDistance: minDistance, lanePosition: lanePosition, curveDelta: nextElement.m_TargetDelta.y, navigationSize: navigationSize, isBackward: flag2, transforms: m_TransformData, taxiStands: m_TaxiStandData, areaLanes: m_AreaLaneData, curves: m_CurveData, owners: m_OwnerData))
						{
							return true;
						}
					}
					curveDelta.x = curveDelta.y;
					return false;
				}
				if (CreatureUtils.SetTriangleTarget(position5, position6, position7, comparePosition, nextElement, pathOwner.m_ElementIndex, pathElements, ref targetPosition, minDistance, lanePosition, curveDelta.y, navigationSize, isSingle: false, m_TransformData, m_TaxiStandData, m_AreaLaneData, m_CurveData))
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

		private bool MoveTransformTarget(Entity creature, Entity creaturePrefab, DynamicBuffer<MeshGroup> meshGroups, ref Random random, CurrentVehicle currentVehicle, float3 comparePosition, ref float3 targetPosition, ref float3 targetDirection, ref byte activity, float minDistance, Entity target, ActivityMask activityMask)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			Transform result = new Transform
			{
				m_Position = targetPosition
			};
			ActivityType activity2 = ActivityType.None;
			if (CreatureUtils.CalculateTransformPosition(creature, creaturePrefab, meshGroups, ref random, ref result, ref activity2, currentVehicle, target, m_LeftHandTraffic, activityMask, (ActivityCondition)0u, m_MovingObjectSearchTree, ref m_TransformData, ref m_PositionData, ref m_PublicTransportData, ref m_TrainData, ref m_ControllerData, ref m_PrefabRefData, ref m_PrefabBuildingData, ref m_PrefabCarData, ref m_PrefabActivityLocations, ref m_SubMeshGroups, ref m_CharacterElements, ref m_SubMeshes, ref m_AnimationClips, ref m_AnimationMotions))
			{
				targetPosition = result.m_Position;
				if (((quaternion)(ref result.m_Rotation)).Equals(default(quaternion)))
				{
					targetDirection = default(float3);
				}
				else
				{
					targetDirection = math.forward(result.m_Rotation);
				}
				activity = (byte)activity2;
				return math.distance(comparePosition, targetPosition) >= minDistance;
			}
			return false;
		}

		private bool GetTransformTarget(ref Entity entity, Transform transform, Entity target, Entity prevLane, float prevCurvePosition, float prevLanePosition, ObjectGeometryData prefabObjectGeometryData)
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
		public ComponentTypeHandle<Animal> __Game_Creatures_Animal_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<MeshGroup> __Game_Rendering_MeshGroup_RO_BufferTypeHandle;

		public ComponentTypeHandle<AnimalNavigation> __Game_Creatures_AnimalNavigation_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Blocker> __Game_Vehicles_Blocker_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HumanCurrentLane> __Game_Creatures_HumanCurrentLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> __Game_Creatures_CurrentVehicle_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.PedestrianLane> __Game_Net_PedestrianLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> __Game_Net_ConnectionLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LaneReservation> __Game_Net_LaneReservation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AreaLane> __Game_Net_AreaLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Position> __Game_Routes_Position_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TaxiStand> __Game_Routes_TaxiStand_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Moving> __Game_Objects_Moving_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Creature> __Game_Creatures_Creature_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Animal> __Game_Creatures_Animal_RO_ComponentLookup;

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
		public ComponentLookup<PathOwner> __Game_Pathfind_PathOwner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CreatureData> __Game_Prefabs_CreatureData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AnimalData> __Game_Prefabs_AnimalData_RO_ComponentLookup;

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
		public BufferLookup<PathElement> __Game_Pathfind_PathElement_RO_BufferLookup;

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

		public ComponentLookup<AnimalCurrentLane> __Game_Creatures_AnimalCurrentLane_RW_ComponentLookup;

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
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Objects_Moving_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Moving>(true);
			__Game_Creatures_GroupMember_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<GroupMember>(true);
			__Game_Creatures_Stumbling_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Stumbling>(true);
			__Game_Objects_TripSource_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TripSource>(true);
			__Game_Creatures_Animal_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Animal>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Rendering_MeshGroup_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<MeshGroup>(true);
			__Game_Creatures_AnimalNavigation_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AnimalNavigation>(false);
			__Game_Vehicles_Blocker_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Blocker>(false);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Creatures_HumanCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HumanCurrentLane>(true);
			__Game_Creatures_CurrentVehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentVehicle>(true);
			__Game_Net_PedestrianLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.PedestrianLane>(true);
			__Game_Net_ConnectionLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ConnectionLane>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_LaneReservation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneReservation>(true);
			__Game_Net_AreaLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AreaLane>(true);
			__Game_Routes_Position_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Position>(true);
			__Game_Routes_TaxiStand_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TaxiStand>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Objects_Moving_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Moving>(true);
			__Game_Creatures_Creature_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Creature>(true);
			__Game_Creatures_Animal_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Animal>(true);
			__Game_Creatures_GroupMember_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GroupMember>(true);
			__Game_Areas_HangaroundLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HangaroundLocation>(true);
			__Game_Vehicles_PublicTransport_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.PublicTransport>(true);
			__Game_Vehicles_Train_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Train>(true);
			__Game_Vehicles_Controller_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Controller>(true);
			__Game_Pathfind_PathOwner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathOwner>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_CreatureData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CreatureData>(true);
			__Game_Prefabs_AnimalData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AnimalData>(true);
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
			__Game_Pathfind_PathElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PathElement>(true);
			__Game_Prefabs_ActivityLocationElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ActivityLocationElement>(true);
			__Game_Prefabs_SubMeshGroup_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubMeshGroup>(true);
			__Game_Prefabs_CharacterElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CharacterElement>(true);
			__Game_Prefabs_AnimationClip_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<AnimationClip>(true);
			__Game_Prefabs_AnimationMotion_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<AnimationMotion>(true);
			__Game_Prefabs_SubMesh_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubMesh>(true);
			__Game_Creatures_AnimalCurrentLane_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AnimalCurrentLane>(false);
		}
	}

	private SimulationSystem m_SimulationSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private Game.Net.SearchSystem m_NetSearchSystem;

	private Game.Areas.SearchSystem m_AreaSearchSystem;

	private Game.Objects.SearchSystem m_ObjectSearchSystem;

	private TerrainSystem m_TerrainSystem;

	private WaterSystem m_WaterSystem;

	private Actions m_Actions;

	private EntityQuery m_CreatureQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		m_AreaSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Areas.SearchSystem>();
		m_ObjectSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.SearchSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_Actions = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Actions>();
		m_CreatureQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Animal>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.ReadWrite<AnimalCurrentLane>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
	}

	[Preserve]
	protected override void OnUpdate()
	{
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
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_0551: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0573: Unknown result type (might be due to invalid IL or missing references)
		//IL_058b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0590: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0604: Unknown result type (might be due to invalid IL or missing references)
		//IL_061c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0621: Unknown result type (might be due to invalid IL or missing references)
		//IL_0639: Unknown result type (might be due to invalid IL or missing references)
		//IL_063e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0693: Unknown result type (might be due to invalid IL or missing references)
		//IL_0698: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0702: Unknown result type (might be due to invalid IL or missing references)
		//IL_0703: Unknown result type (might be due to invalid IL or missing references)
		//IL_0704: Unknown result type (might be due to invalid IL or missing references)
		//IL_0706: Unknown result type (might be due to invalid IL or missing references)
		//IL_0708: Unknown result type (might be due to invalid IL or missing references)
		//IL_0709: Unknown result type (might be due to invalid IL or missing references)
		//IL_070e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0713: Unknown result type (might be due to invalid IL or missing references)
		//IL_071b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0728: Unknown result type (might be due to invalid IL or missing references)
		//IL_0735: Unknown result type (might be due to invalid IL or missing references)
		//IL_0742: Unknown result type (might be due to invalid IL or missing references)
		//IL_074f: Unknown result type (might be due to invalid IL or missing references)
		//IL_075c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0769: Unknown result type (might be due to invalid IL or missing references)
		//IL_076b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0771: Unknown result type (might be due to invalid IL or missing references)
		uint num = m_SimulationSystem.frameIndex % 16;
		if (num == 5 || num == 9 || num == 13)
		{
			((EntityQuery)(ref m_CreatureQuery)).ResetFilter();
			((EntityQuery)(ref m_CreatureQuery)).SetSharedComponentFilter<UpdateFrame>(new UpdateFrame(num));
			JobHandle deps;
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
				m_AnimalType = InternalCompilerInterface.GetComponentTypeHandle<Animal>(ref __TypeHandle.__Game_Creatures_Animal_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_MeshGroupType = InternalCompilerInterface.GetBufferTypeHandle<MeshGroup>(ref __TypeHandle.__Game_Rendering_MeshGroup_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_NavigationType = InternalCompilerInterface.GetComponentTypeHandle<AnimalNavigation>(ref __TypeHandle.__Game_Creatures_AnimalNavigation_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_BlockerType = InternalCompilerInterface.GetComponentTypeHandle<Blocker>(ref __TypeHandle.__Game_Vehicles_Blocker_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HumanCurrentLaneData = InternalCompilerInterface.GetComponentLookup<HumanCurrentLane>(ref __TypeHandle.__Game_Creatures_HumanCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CurrentVehicleData = InternalCompilerInterface.GetComponentLookup<CurrentVehicle>(ref __TypeHandle.__Game_Creatures_CurrentVehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PedestrianLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.PedestrianLane>(ref __TypeHandle.__Game_Net_PedestrianLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectionLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LaneReservationData = InternalCompilerInterface.GetComponentLookup<LaneReservation>(ref __TypeHandle.__Game_Net_LaneReservation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AreaLaneData = InternalCompilerInterface.GetComponentLookup<AreaLane>(ref __TypeHandle.__Game_Net_AreaLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PositionData = InternalCompilerInterface.GetComponentLookup<Position>(ref __TypeHandle.__Game_Routes_Position_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TaxiStandData = InternalCompilerInterface.GetComponentLookup<TaxiStand>(ref __TypeHandle.__Game_Routes_TaxiStand_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PropertyRenterData = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_MovingData = InternalCompilerInterface.GetComponentLookup<Moving>(ref __TypeHandle.__Game_Objects_Moving_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CreatureData = InternalCompilerInterface.GetComponentLookup<Creature>(ref __TypeHandle.__Game_Creatures_Creature_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AnimalData = InternalCompilerInterface.GetComponentLookup<Animal>(ref __TypeHandle.__Game_Creatures_Animal_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_GroupMemberData = InternalCompilerInterface.GetComponentLookup<GroupMember>(ref __TypeHandle.__Game_Creatures_GroupMember_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HangaroundLocationData = InternalCompilerInterface.GetComponentLookup<HangaroundLocation>(ref __TypeHandle.__Game_Areas_HangaroundLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PublicTransportData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.PublicTransport>(ref __TypeHandle.__Game_Vehicles_PublicTransport_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TrainData = InternalCompilerInterface.GetComponentLookup<Train>(ref __TypeHandle.__Game_Vehicles_Train_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ControllerData = InternalCompilerInterface.GetComponentLookup<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PathOwnerData = InternalCompilerInterface.GetComponentLookup<PathOwner>(ref __TypeHandle.__Game_Pathfind_PathOwner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabCreatureData = InternalCompilerInterface.GetComponentLookup<CreatureData>(ref __TypeHandle.__Game_Prefabs_CreatureData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabAnimalData = InternalCompilerInterface.GetComponentLookup<AnimalData>(ref __TypeHandle.__Game_Prefabs_AnimalData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
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
				m_PathElements = InternalCompilerInterface.GetBufferLookup<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabActivityLocations = InternalCompilerInterface.GetBufferLookup<ActivityLocationElement>(ref __TypeHandle.__Game_Prefabs_ActivityLocationElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubMeshGroups = InternalCompilerInterface.GetBufferLookup<SubMeshGroup>(ref __TypeHandle.__Game_Prefabs_SubMeshGroup_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CharacterElements = InternalCompilerInterface.GetBufferLookup<CharacterElement>(ref __TypeHandle.__Game_Prefabs_CharacterElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AnimationClips = InternalCompilerInterface.GetBufferLookup<AnimationClip>(ref __TypeHandle.__Game_Prefabs_AnimationClip_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AnimationMotions = InternalCompilerInterface.GetBufferLookup<AnimationMotion>(ref __TypeHandle.__Game_Prefabs_AnimationMotion_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubMeshes = InternalCompilerInterface.GetBufferLookup<SubMesh>(ref __TypeHandle.__Game_Prefabs_SubMesh_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AnimalCurrentLaneData = InternalCompilerInterface.GetComponentLookup<AnimalCurrentLane>(ref __TypeHandle.__Game_Creatures_AnimalCurrentLane_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_RandomSeed = RandomSeed.Next(),
				m_LeftHandTraffic = m_CityConfigurationSystem.leftHandTraffic,
				m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
				m_WaterSurfaceData = m_WaterSystem.GetSurfaceData(out deps),
				m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies),
				m_StaticObjectSearchTree = m_ObjectSearchSystem.GetStaticSearchTree(readOnly: true, out dependencies2),
				m_MovingObjectSearchTree = m_ObjectSearchSystem.GetMovingSearchTree(readOnly: true, out dependencies3),
				m_AreaSearchTree = m_AreaSearchSystem.GetSearchTree(readOnly: true, out dependencies4),
				m_LaneObjectBuffer = m_Actions.m_LaneObjectUpdater.Begin((Allocator)3)
			}, m_CreatureQuery, JobUtils.CombineDependencies(((SystemBase)this).Dependency, dependencies, dependencies2, dependencies3, dependencies4, deps));
			m_TerrainSystem.AddCPUHeightReader(val);
			m_WaterSystem.AddSurfaceReader(val);
			m_NetSearchSystem.AddNetSearchTreeReader(val);
			m_ObjectSearchSystem.AddStaticSearchTreeReader(val);
			m_ObjectSearchSystem.AddMovingSearchTreeReader(val);
			m_AreaSearchSystem.AddSearchTreeReader(val);
			m_Actions.m_Dependency = val;
			((SystemBase)this).Dependency = val;
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
	public AnimalNavigationSystem()
	{
	}
}
