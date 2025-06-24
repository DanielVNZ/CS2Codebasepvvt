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
public class CarNavigationSystem : GameSystemBase
{
	[CompilerGenerated]
	public class Actions : GameSystemBase
	{
		private struct TypeHandle
		{
			public ComponentLookup<LaneReservation> __Game_Net_LaneReservation_RW_ComponentLookup;

			[ReadOnly]
			public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

			[ReadOnly]
			public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

			[ReadOnly]
			public ComponentLookup<LaneDeteriorationData> __Game_Prefabs_LaneDeteriorationData_RO_ComponentLookup;

			public ComponentLookup<Game.Net.Pollution> __Game_Net_Pollution_RW_ComponentLookup;

			public ComponentLookup<LaneCondition> __Game_Net_LaneCondition_RW_ComponentLookup;

			public ComponentLookup<LaneFlow> __Game_Net_LaneFlow_RW_ComponentLookup;

			public ComponentLookup<LaneSignal> __Game_Net_LaneSignal_RW_ComponentLookup;

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
				__Game_Net_LaneReservation_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneReservation>(false);
				__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
				__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
				__Game_Prefabs_LaneDeteriorationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneDeteriorationData>(true);
				__Game_Net_Pollution_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Pollution>(false);
				__Game_Net_LaneCondition_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneCondition>(false);
				__Game_Net_LaneFlow_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneFlow>(false);
				__Game_Net_LaneSignal_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneSignal>(false);
			}
		}

		private TrafficAmbienceSystem m_TrafficAmbienceSystem;

		public LaneObjectUpdater m_LaneObjectUpdater;

		public NativeQueue<CarNavigationHelpers.LaneReservation> m_LaneReservationQueue;

		public NativeQueue<CarNavigationHelpers.LaneEffects> m_LaneEffectsQueue;

		public NativeQueue<CarNavigationHelpers.LaneSignal> m_LaneSignalQueue;

		public NativeQueue<TrafficAmbienceEffect> m_TrafficAmbienceQueue;

		public JobHandle m_Dependency;

		private TypeHandle __TypeHandle;

		[Preserve]
		protected override void OnCreate()
		{
			base.OnCreate();
			m_TrafficAmbienceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TrafficAmbienceSystem>();
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
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			JobHandle val = JobHandle.CombineDependencies(((SystemBase)this).Dependency, m_Dependency);
			UpdateLaneReservationsJob updateLaneReservationsJob = new UpdateLaneReservationsJob
			{
				m_LaneReservationQueue = m_LaneReservationQueue,
				m_LaneReservationData = InternalCompilerInterface.GetComponentLookup<LaneReservation>(ref __TypeHandle.__Game_Net_LaneReservation_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			ApplyLaneEffectsJob applyLaneEffectsJob = new ApplyLaneEffectsJob
			{
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LaneDeteriorationData = InternalCompilerInterface.GetComponentLookup<LaneDeteriorationData>(ref __TypeHandle.__Game_Prefabs_LaneDeteriorationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PollutionData = InternalCompilerInterface.GetComponentLookup<Game.Net.Pollution>(ref __TypeHandle.__Game_Net_Pollution_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LaneConditionData = InternalCompilerInterface.GetComponentLookup<LaneCondition>(ref __TypeHandle.__Game_Net_LaneCondition_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LaneFlowData = InternalCompilerInterface.GetComponentLookup<LaneFlow>(ref __TypeHandle.__Game_Net_LaneFlow_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LaneEffectsQueue = m_LaneEffectsQueue
			};
			JobHandle dependencies;
			ApplyTrafficAmbienceJob applyTrafficAmbienceJob = new ApplyTrafficAmbienceJob
			{
				m_EffectsQueue = m_TrafficAmbienceQueue,
				m_TrafficAmbienceMap = m_TrafficAmbienceSystem.GetMap(readOnly: false, out dependencies)
			};
			UpdateLaneSignalsJob obj = new UpdateLaneSignalsJob
			{
				m_LaneSignalQueue = m_LaneSignalQueue,
				m_LaneSignalData = InternalCompilerInterface.GetComponentLookup<LaneSignal>(ref __TypeHandle.__Game_Net_LaneSignal_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			JobHandle val2 = IJobExtensions.Schedule<UpdateLaneReservationsJob>(updateLaneReservationsJob, val);
			JobHandle val3 = IJobExtensions.Schedule<ApplyLaneEffectsJob>(applyLaneEffectsJob, val);
			JobHandle val4 = IJobExtensions.Schedule<ApplyTrafficAmbienceJob>(applyTrafficAmbienceJob, JobHandle.CombineDependencies(dependencies, val));
			JobHandle val5 = IJobExtensions.Schedule<UpdateLaneSignalsJob>(obj, val);
			m_LaneReservationQueue.Dispose(val2);
			m_LaneEffectsQueue.Dispose(val3);
			m_LaneSignalQueue.Dispose(val5);
			m_TrafficAmbienceQueue.Dispose(val4);
			m_TrafficAmbienceSystem.AddWriter(val4);
			JobHandle val6 = m_LaneObjectUpdater.Apply((SystemBase)(object)this, val);
			((SystemBase)this).Dependency = JobUtils.CombineDependencies(val6, val2, val3, val5);
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
	private struct UpdateNavigationJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<Moving> m_MovingType;

		[ReadOnly]
		public ComponentTypeHandle<Target> m_TargetType;

		[ReadOnly]
		public ComponentTypeHandle<Car> m_CarType;

		[ReadOnly]
		public ComponentTypeHandle<OutOfControl> m_OutOfControlType;

		[ReadOnly]
		public ComponentTypeHandle<PseudoRandomSeed> m_PseudoRandomSeedType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<LayoutElement> m_LayoutElementType;

		public ComponentTypeHandle<CarNavigation> m_NavigationType;

		public ComponentTypeHandle<CarCurrentLane> m_CurrentLaneType;

		public ComponentTypeHandle<PathOwner> m_PathOwnerType;

		public ComponentTypeHandle<Blocker> m_BlockerType;

		public ComponentTypeHandle<Odometer> m_OdometerType;

		public BufferTypeHandle<CarNavigationLane> m_NavigationLaneType;

		public BufferTypeHandle<PathElement> m_PathElementType;

		[ReadOnly]
		public EntityStorageInfoLookup m_EntityStorageInfoLookup;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Unspawned> m_UnspawnedData;

		[ReadOnly]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> m_CarLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> m_ParkingLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> m_ConnectionLaneData;

		[ReadOnly]
		public ComponentLookup<MasterLane> m_MasterLaneData;

		[ReadOnly]
		public ComponentLookup<SlaveLane> m_SlaveLaneData;

		[ReadOnly]
		public ComponentLookup<AreaLane> m_AreaLaneData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<LaneReservation> m_LaneReservationData;

		[ReadOnly]
		public ComponentLookup<LaneCondition> m_LaneConditionData;

		[ReadOnly]
		public ComponentLookup<LaneSignal> m_LaneSignalData;

		[ReadOnly]
		public ComponentLookup<Road> m_RoadData;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenterData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Position> m_PositionData;

		[ReadOnly]
		public ComponentLookup<Moving> m_MovingData;

		[ReadOnly]
		public ComponentLookup<Car> m_CarData;

		[ReadOnly]
		public ComponentLookup<Train> m_TrainData;

		[ReadOnly]
		public ComponentLookup<Controller> m_ControllerData;

		[ReadOnly]
		public ComponentLookup<Vehicle> m_VehicleData;

		[ReadOnly]
		public ComponentLookup<Creature> m_CreatureData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<CarData> m_PrefabCarData;

		[ReadOnly]
		public ComponentLookup<TrainData> m_PrefabTrainData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_PrefabBuildingData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<VehicleSideEffectData> m_PrefabSideEffectData;

		[ReadOnly]
		public ComponentLookup<NetLaneData> m_PrefabLaneData;

		[ReadOnly]
		public ComponentLookup<CarLaneData> m_PrefabCarLaneData;

		[ReadOnly]
		public ComponentLookup<ParkingLaneData> m_PrefabParkingLaneData;

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

		[NativeDisableParallelForRestriction]
		public ComponentLookup<CarTrailerLane> m_TrailerLaneData;

		[NativeDisableParallelForRestriction]
		public BufferLookup<BlockedLane> m_BlockedLanes;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public uint m_SimulationFrame;

		[ReadOnly]
		public bool m_LeftHandTraffic;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		[ReadOnly]
		public NativeQuadTree<AreaSearchItem, QuadTreeBoundsXZ> m_AreaSearchTree;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_StaticObjectSearchTree;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_MovingObjectSearchTree;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		public LaneObjectCommandBuffer m_LaneObjectBuffer;

		public ParallelWriter<CarNavigationHelpers.LaneReservation> m_LaneReservations;

		public ParallelWriter<CarNavigationHelpers.LaneEffects> m_LaneEffects;

		public ParallelWriter<CarNavigationHelpers.LaneSignal> m_LaneSignals;

		public ParallelWriter<TrafficAmbienceEffect> m_TrafficAmbienceEffects;

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
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0504: Unknown result type (might be due to invalid IL or missing references)
			//IL_054c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0551: Unknown result type (might be due to invalid IL or missing references)
			//IL_0557: Unknown result type (might be due to invalid IL or missing references)
			//IL_055c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0564: Unknown result type (might be due to invalid IL or missing references)
			//IL_0566: Unknown result type (might be due to invalid IL or missing references)
			//IL_056b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0575: Unknown result type (might be due to invalid IL or missing references)
			//IL_0583: Unknown result type (might be due to invalid IL or missing references)
			//IL_0586: Unknown result type (might be due to invalid IL or missing references)
			//IL_058c: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_060c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0408: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_0417: Unknown result type (might be due to invalid IL or missing references)
			//IL_041d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0426: Unknown result type (might be due to invalid IL or missing references)
			//IL_045d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0462: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Transform> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			NativeArray<Moving> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Moving>(ref m_MovingType);
			NativeArray<Blocker> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Blocker>(ref m_BlockerType);
			NativeArray<CarCurrentLane> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarCurrentLane>(ref m_CurrentLaneType);
			NativeArray<CarNavigation> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarNavigation>(ref m_NavigationType);
			NativeArray<PrefabRef> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<PathOwner> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathOwner>(ref m_PathOwnerType);
			BufferAccessor<CarNavigationLane> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<CarNavigationLane>(ref m_NavigationLaneType);
			BufferAccessor<PathElement> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<PathElement>(ref m_PathElementType);
			BufferAccessor<LayoutElement> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<LayoutElement>(ref m_LayoutElementType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			if (((ArchetypeChunk)(ref chunk)).Has<OutOfControl>(ref m_OutOfControlType))
			{
				NativeList<BlockedLane> val = default(NativeList<BlockedLane>);
				val._002Ector(16, AllocatorHandle.op_Implicit((Allocator)2));
				for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
				{
					Entity val2 = nativeArray[i];
					Transform transform = nativeArray2[i];
					CarNavigation carNavigation = nativeArray6[i];
					CarCurrentLane currentLane = nativeArray5[i];
					Blocker blocker = nativeArray4[i];
					PathOwner pathOwner = nativeArray8[i];
					PrefabRef prefabRef = nativeArray7[i];
					DynamicBuffer<CarNavigationLane> navigationLanes = bufferAccessor[i];
					DynamicBuffer<PathElement> pathElements = bufferAccessor2[i];
					DynamicBuffer<BlockedLane> blockedLanes = m_BlockedLanes[val2];
					ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
					Moving moving = default(Moving);
					if (nativeArray3.Length != 0)
					{
						moving = nativeArray3[i];
					}
					CarNavigationHelpers.CurrentLaneCache currentLaneCache = new CarNavigationHelpers.CurrentLaneCache(ref currentLane, blockedLanes, m_EntityStorageInfoLookup, m_MovingObjectSearchTree);
					UpdateOutOfControl(val2, transform, objectGeometryData, ref carNavigation, ref currentLane, ref blocker, ref pathOwner, navigationLanes, pathElements, blockedLanes, val);
					currentLaneCache.CheckChanges(val2, ref currentLane, val, m_LaneObjectBuffer, m_LaneObjects, transform, moving, carNavigation, objectGeometryData);
					nativeArray6[i] = carNavigation;
					nativeArray5[i] = currentLane;
					nativeArray8[i] = pathOwner;
					nativeArray4[i] = blocker;
					val.Clear();
					if (bufferAccessor3.Length != 0)
					{
						UpdateOutOfControlTrailers(carNavigation, bufferAccessor3[i], val);
					}
				}
				val.Dispose();
				return;
			}
			if (nativeArray3.Length != 0)
			{
				NativeArray<Target> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Target>(ref m_TargetType);
				NativeArray<Car> nativeArray10 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Car>(ref m_CarType);
				NativeArray<Odometer> nativeArray11 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Odometer>(ref m_OdometerType);
				NativeArray<PseudoRandomSeed> nativeArray12 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PseudoRandomSeed>(ref m_PseudoRandomSeedType);
				NativeList<Entity> tempBuffer = default(NativeList<Entity>);
				CarLaneSelectBuffer laneSelectBuffer = default(CarLaneSelectBuffer);
				bool flag = nativeArray11.Length != 0;
				for (int j = 0; j < ((ArchetypeChunk)(ref chunk)).Count; j++)
				{
					Entity val3 = nativeArray[j];
					Transform transform2 = nativeArray2[j];
					Moving moving2 = nativeArray3[j];
					Target target = nativeArray9[j];
					Car car = nativeArray10[j];
					CarNavigation navigation = nativeArray6[j];
					CarCurrentLane currentLane2 = nativeArray5[j];
					PseudoRandomSeed pseudoRandomSeed = nativeArray12[j];
					Blocker blocker2 = nativeArray4[j];
					PathOwner pathOwner2 = nativeArray8[j];
					PrefabRef prefabRef2 = nativeArray7[j];
					DynamicBuffer<CarNavigationLane> navigationLanes2 = bufferAccessor[j];
					DynamicBuffer<PathElement> pathElements2 = bufferAccessor2[j];
					DynamicBuffer<BlockedLane> blockedLanes2 = m_BlockedLanes[val3];
					CarData prefabCarData = m_PrefabCarData[prefabRef2.m_Prefab];
					ObjectGeometryData objectGeometryData2 = m_PrefabObjectGeometryData[prefabRef2.m_Prefab];
					if (bufferAccessor3.Length != 0)
					{
						UpdateCarLimits(ref prefabCarData, bufferAccessor3[j]);
					}
					CarNavigationHelpers.CurrentLaneCache currentLaneCache2 = new CarNavigationHelpers.CurrentLaneCache(ref currentLane2, blockedLanes2, m_EntityStorageInfoLookup, m_MovingObjectSearchTree);
					int priority = VehicleUtils.GetPriority(car);
					Odometer odometer = default(Odometer);
					if (flag)
					{
						odometer = nativeArray11[j];
					}
					UpdateNavigationLanes(ref random, priority, val3, transform2, moving2, target, car, prefabCarData, ref laneSelectBuffer, ref currentLane2, ref blocker2, ref pathOwner2, navigationLanes2, pathElements2);
					UpdateNavigationTarget(ref random, priority, val3, transform2, moving2, car, pseudoRandomSeed, prefabRef2, prefabCarData, objectGeometryData2, ref navigation, ref currentLane2, ref blocker2, ref odometer, ref pathOwner2, ref tempBuffer, navigationLanes2, pathElements2);
					ReserveNavigationLanes(ref random, priority, val3, prefabCarData, objectGeometryData2, car, ref navigation, ref currentLane2, navigationLanes2);
					currentLaneCache2.CheckChanges(val3, ref currentLane2, default(NativeList<BlockedLane>), m_LaneObjectBuffer, m_LaneObjects, transform2, moving2, navigation, objectGeometryData2);
					m_TrafficAmbienceEffects.Enqueue(new TrafficAmbienceEffect
					{
						m_Amount = CalculateNoise(ref currentLane2, prefabRef2, prefabCarData),
						m_Position = transform2.m_Position
					});
					nativeArray6[j] = navigation;
					nativeArray5[j] = currentLane2;
					nativeArray8[j] = pathOwner2;
					nativeArray4[j] = blocker2;
					if (flag)
					{
						nativeArray11[j] = odometer;
					}
					if (bufferAccessor3.Length != 0)
					{
						UpdateTrailers(navigation, currentLane2, bufferAccessor3[j]);
					}
				}
				laneSelectBuffer.Dispose();
				if (tempBuffer.IsCreated)
				{
					tempBuffer.Dispose();
				}
				return;
			}
			for (int k = 0; k < ((ArchetypeChunk)(ref chunk)).Count; k++)
			{
				Entity val4 = nativeArray[k];
				Transform transform3 = nativeArray2[k];
				CarNavigation navigation2 = nativeArray6[k];
				CarCurrentLane currentLane3 = nativeArray5[k];
				Blocker blocker3 = nativeArray4[k];
				PathOwner pathOwner3 = nativeArray8[k];
				PrefabRef prefabRef3 = nativeArray7[k];
				DynamicBuffer<CarNavigationLane> navigationLanes3 = bufferAccessor[k];
				DynamicBuffer<PathElement> pathElements3 = bufferAccessor2[k];
				DynamicBuffer<BlockedLane> blockedLanes3 = m_BlockedLanes[val4];
				ObjectGeometryData objectGeometryData3 = m_PrefabObjectGeometryData[prefabRef3.m_Prefab];
				CarNavigationHelpers.CurrentLaneCache currentLaneCache3 = new CarNavigationHelpers.CurrentLaneCache(ref currentLane3, blockedLanes3, m_EntityStorageInfoLookup, m_MovingObjectSearchTree);
				UpdateStopped(transform3, ref currentLane3, ref blocker3, ref pathOwner3, navigationLanes3, pathElements3);
				currentLaneCache3.CheckChanges(val4, ref currentLane3, default(NativeList<BlockedLane>), m_LaneObjectBuffer, m_LaneObjects, transform3, default(Moving), navigation2, objectGeometryData3);
				nativeArray5[k] = currentLane3;
				nativeArray8[k] = pathOwner3;
				nativeArray4[k] = blocker3;
				if (bufferAccessor3.Length != 0)
				{
					UpdateStoppedTrailers(navigation2, bufferAccessor3[k]);
				}
			}
		}

		private void UpdateCarLimits(ref CarData prefabCarData, DynamicBuffer<LayoutElement> layout)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 1; i < layout.Length; i++)
			{
				Entity vehicle = layout[i].m_Vehicle;
				PrefabRef prefabRef = m_PrefabRefData[vehicle];
				CarData carData = m_PrefabCarData[prefabRef.m_Prefab];
				prefabCarData.m_Acceleration = math.min(prefabCarData.m_Acceleration, carData.m_Acceleration);
				prefabCarData.m_Braking = math.min(prefabCarData.m_Braking, carData.m_Braking);
				prefabCarData.m_MaxSpeed = math.min(prefabCarData.m_MaxSpeed, carData.m_MaxSpeed);
				prefabCarData.m_Turning = math.min(prefabCarData.m_Turning, carData.m_Turning);
			}
		}

		private void UpdateTrailers(CarNavigation navigation, CarCurrentLane currentLane, DynamicBuffer<LayoutElement> layout)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			Entity lane = currentLane.m_Lane;
			float2 nextPosition = ((float3)(ref currentLane.m_CurvePosition)).xy;
			bool forceNext = (currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.Connection) != 0;
			for (int i = 1; i < layout.Length; i++)
			{
				Entity vehicle = layout[i].m_Vehicle;
				CarTrailerLane trailerLane = m_TrailerLaneData[vehicle];
				Transform transform = m_TransformData[vehicle];
				Moving moving = m_MovingData[vehicle];
				DynamicBuffer<BlockedLane> blockedLanes = m_BlockedLanes[vehicle];
				PrefabRef prefabRef = m_PrefabRefData[vehicle];
				ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
				CarNavigationHelpers.TrailerLaneCache trailerLaneCache = new CarNavigationHelpers.TrailerLaneCache(ref trailerLane, blockedLanes, m_PrefabRefData, m_MovingObjectSearchTree);
				if (trailerLane.m_Lane == Entity.Null)
				{
					TryFindCurrentLane(ref trailerLane, transform, moving);
				}
				UpdateTrailer(vehicle, transform, objectGeometryData, lane, nextPosition, forceNext, ref trailerLane);
				trailerLaneCache.CheckChanges(vehicle, ref trailerLane, default(NativeList<BlockedLane>), m_LaneObjectBuffer, m_LaneObjects, transform, moving, navigation, objectGeometryData);
				m_TrailerLaneData[vehicle] = trailerLane;
				lane = trailerLane.m_Lane;
				nextPosition = trailerLane.m_CurvePosition;
			}
		}

		private void UpdateOutOfControlTrailers(CarNavigation navigation, DynamicBuffer<LayoutElement> layout, NativeList<BlockedLane> tempBlockedLanes)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 1; i < layout.Length; i++)
			{
				Entity vehicle = layout[i].m_Vehicle;
				CarTrailerLane trailerLane = m_TrailerLaneData[vehicle];
				Transform transform = m_TransformData[vehicle];
				Moving moving = m_MovingData[vehicle];
				DynamicBuffer<BlockedLane> blockedLanes = m_BlockedLanes[vehicle];
				PrefabRef prefabRef = m_PrefabRefData[vehicle];
				ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
				CarNavigationHelpers.TrailerLaneCache trailerLaneCache = new CarNavigationHelpers.TrailerLaneCache(ref trailerLane, blockedLanes, m_PrefabRefData, m_MovingObjectSearchTree);
				UpdateOutOfControl(vehicle, transform, objectGeometryData, ref trailerLane, blockedLanes, tempBlockedLanes);
				trailerLaneCache.CheckChanges(vehicle, ref trailerLane, tempBlockedLanes, m_LaneObjectBuffer, m_LaneObjects, transform, moving, navigation, objectGeometryData);
				m_TrailerLaneData[vehicle] = trailerLane;
				tempBlockedLanes.Clear();
			}
		}

		private void UpdateStoppedTrailers(CarNavigation navigation, DynamicBuffer<LayoutElement> layout)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 1; i < layout.Length; i++)
			{
				Entity vehicle = layout[i].m_Vehicle;
				CarTrailerLane trailerLane = m_TrailerLaneData[vehicle];
				Transform transform = m_TransformData[vehicle];
				DynamicBuffer<BlockedLane> blockedLanes = m_BlockedLanes[vehicle];
				PrefabRef prefabRef = m_PrefabRefData[vehicle];
				ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
				CarNavigationHelpers.TrailerLaneCache trailerLaneCache = new CarNavigationHelpers.TrailerLaneCache(ref trailerLane, blockedLanes, m_PrefabRefData, m_MovingObjectSearchTree);
				if (trailerLane.m_Lane == Entity.Null)
				{
					TryFindCurrentLane(ref trailerLane, transform, default(Moving));
				}
				trailerLaneCache.CheckChanges(vehicle, ref trailerLane, default(NativeList<BlockedLane>), m_LaneObjectBuffer, m_LaneObjects, transform, default(Moving), navigation, objectGeometryData);
				m_TrailerLaneData[vehicle] = trailerLane;
			}
		}

		private void UpdateStopped(Transform transform, ref CarCurrentLane currentLane, ref Blocker blocker, ref PathOwner pathOwner, DynamicBuffer<CarNavigationLane> navigationLanes, DynamicBuffer<PathElement> pathElements)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			if (currentLane.m_Lane == Entity.Null || (currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.Obsolete) != 0)
			{
				TryFindCurrentLane(ref currentLane, transform, default(Moving));
				navigationLanes.Clear();
				pathElements.Clear();
				pathOwner.m_ElementIndex = 0;
				pathOwner.m_State |= PathFlags.Obsolete;
			}
			if ((currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.QueueReached) != 0 && (!m_CarData.HasComponent(blocker.m_Blocker) || (m_CarData[blocker.m_Blocker].m_Flags & CarFlags.Queueing) == 0))
			{
				currentLane.m_LaneFlags &= ~Game.Vehicles.CarLaneFlags.QueueReached;
				blocker = default(Blocker);
			}
		}

		private void UpdateOutOfControl(Entity entity, Transform transform, ObjectGeometryData prefabObjectGeometryData, ref CarTrailerLane trailerLane, DynamicBuffer<BlockedLane> blockedLanes, NativeList<BlockedLane> tempBlockedLanes)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			float3 position = transform.m_Position;
			float3 val = math.forward(transform.m_Rotation);
			Segment val2 = default(Segment);
			((Segment)(ref val2))._002Ector(position - val * math.max(0.1f, 0f - prefabObjectGeometryData.m_Bounds.min.z - prefabObjectGeometryData.m_Size.x * 0.5f), position + val * math.max(0.1f, prefabObjectGeometryData.m_Bounds.max.z - prefabObjectGeometryData.m_Size.x * 0.5f));
			float num = prefabObjectGeometryData.m_Size.x * 0.5f;
			Bounds3 bounds = MathUtils.Expand(MathUtils.Bounds(val2), float3.op_Implicit(num));
			CarNavigationHelpers.FindBlockedLanesIterator findBlockedLanesIterator = new CarNavigationHelpers.FindBlockedLanesIterator
			{
				m_Bounds = bounds,
				m_Line = val2,
				m_Radius = num,
				m_BlockedLanes = tempBlockedLanes,
				m_SubLanes = m_Lanes,
				m_MasterLaneData = m_MasterLaneData,
				m_CurveData = m_CurveData,
				m_PrefabRefData = m_PrefabRefData,
				m_PrefabLaneData = m_PrefabLaneData
			};
			m_NetSearchTree.Iterate<CarNavigationHelpers.FindBlockedLanesIterator>(ref findBlockedLanesIterator, 0);
			trailerLane = default(CarTrailerLane);
		}

		private void UpdateOutOfControl(Entity entity, Transform transform, ObjectGeometryData prefabObjectGeometryData, ref CarNavigation carNavigation, ref CarCurrentLane currentLane, ref Blocker blocker, ref PathOwner pathOwner, DynamicBuffer<CarNavigationLane> navigationLanes, DynamicBuffer<PathElement> pathElements, DynamicBuffer<BlockedLane> blockedLanes, NativeList<BlockedLane> tempBlockedLanes)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			float3 position = transform.m_Position;
			float3 val = math.forward(transform.m_Rotation);
			Segment val2 = default(Segment);
			((Segment)(ref val2))._002Ector(position - val * math.max(0.1f, 0f - prefabObjectGeometryData.m_Bounds.min.z - prefabObjectGeometryData.m_Size.x * 0.5f), position + val * math.max(0.1f, prefabObjectGeometryData.m_Bounds.max.z - prefabObjectGeometryData.m_Size.x * 0.5f));
			float num = prefabObjectGeometryData.m_Size.x * 0.5f;
			Bounds3 bounds = MathUtils.Expand(MathUtils.Bounds(val2), float3.op_Implicit(num));
			CarNavigationHelpers.FindBlockedLanesIterator findBlockedLanesIterator = new CarNavigationHelpers.FindBlockedLanesIterator
			{
				m_Bounds = bounds,
				m_Line = val2,
				m_Radius = num,
				m_BlockedLanes = tempBlockedLanes,
				m_SubLanes = m_Lanes,
				m_MasterLaneData = m_MasterLaneData,
				m_CurveData = m_CurveData,
				m_PrefabRefData = m_PrefabRefData,
				m_PrefabLaneData = m_PrefabLaneData
			};
			m_NetSearchTree.Iterate<CarNavigationHelpers.FindBlockedLanesIterator>(ref findBlockedLanesIterator, 0);
			carNavigation = new CarNavigation
			{
				m_TargetPosition = transform.m_Position
			};
			currentLane = default(CarCurrentLane);
			blocker = default(Blocker);
			pathOwner.m_ElementIndex = 0;
			navigationLanes.Clear();
			pathElements.Clear();
		}

		private void UpdateNavigationLanes(ref Random random, int priority, Entity entity, Transform transform, Moving moving, Target target, Car car, CarData prefabCarData, ref CarLaneSelectBuffer laneSelectBuffer, ref CarCurrentLane currentLane, ref Blocker blocker, ref PathOwner pathOwner, DynamicBuffer<CarNavigationLane> navigationLanes, DynamicBuffer<PathElement> pathElements)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			int invalidPath = 10000000;
			if (currentLane.m_Lane == Entity.Null || (currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.Obsolete) != 0)
			{
				invalidPath = -1;
				TryFindCurrentLane(ref currentLane, transform, moving);
			}
			else if ((pathOwner.m_State & (PathFlags.Pending | PathFlags.Failed | PathFlags.Obsolete | PathFlags.Updated)) != 0 && (pathOwner.m_State & PathFlags.Append) == 0)
			{
				ClearNavigationLanes(ref currentLane, navigationLanes, invalidPath);
			}
			else if ((pathOwner.m_State & PathFlags.Updated) == 0)
			{
				FillNavigationPaths(ref random, priority, entity, transform, target, car, ref laneSelectBuffer, ref currentLane, ref blocker, ref pathOwner, navigationLanes, pathElements, ref invalidPath);
			}
			if (invalidPath != 10000000)
			{
				ClearNavigationLanes(moving, prefabCarData, ref currentLane, navigationLanes, invalidPath);
				pathElements.Clear();
				pathOwner.m_ElementIndex = 0;
				pathOwner.m_State |= PathFlags.Obsolete;
			}
		}

		private void ClearNavigationLanes(ref CarCurrentLane currentLane, DynamicBuffer<CarNavigationLane> navigationLanes, int invalidPath)
		{
			if ((currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.ClearedForPathfind) == 0)
			{
				currentLane.m_CurvePosition.z = currentLane.m_CurvePosition.y;
			}
			if (invalidPath > 0)
			{
				for (int i = 0; i < navigationLanes.Length; i++)
				{
					if ((navigationLanes[i].m_Flags & (Game.Vehicles.CarLaneFlags.Reserved | Game.Vehicles.CarLaneFlags.ClearedForPathfind)) == 0)
					{
						invalidPath = math.min(i, invalidPath);
						break;
					}
				}
			}
			invalidPath = math.max(invalidPath, 0);
			if (invalidPath < navigationLanes.Length)
			{
				navigationLanes.RemoveRange(invalidPath, navigationLanes.Length - invalidPath);
			}
		}

		private void ClearNavigationLanes(Moving moving, CarData prefabCarData, ref CarCurrentLane currentLane, DynamicBuffer<CarNavigationLane> navigationLanes, int invalidPath)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			if (invalidPath >= 0)
			{
				VehicleUtils.ClearNavigationForPathfind(moving, prefabCarData, ref currentLane, navigationLanes, ref m_CarLaneData, ref m_CurveData);
			}
			else
			{
				currentLane.m_CurvePosition.z = currentLane.m_CurvePosition.y;
			}
			invalidPath = math.max(invalidPath, 0);
			if (invalidPath < navigationLanes.Length)
			{
				navigationLanes.RemoveRange(invalidPath, navigationLanes.Length - invalidPath);
			}
		}

		private void TryFindCurrentLane(ref CarCurrentLane currentLane, Transform transform, Moving moving)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			float num = 4f / 15f;
			currentLane.m_LaneFlags &= ~(Game.Vehicles.CarLaneFlags.TransformTarget | Game.Vehicles.CarLaneFlags.ParkingSpace | Game.Vehicles.CarLaneFlags.Obsolete | Game.Vehicles.CarLaneFlags.TurnLeft | Game.Vehicles.CarLaneFlags.TurnRight | Game.Vehicles.CarLaneFlags.Area);
			currentLane.m_Lane = Entity.Null;
			currentLane.m_ChangeLane = Entity.Null;
			float3 val = transform.m_Position + moving.m_Velocity * (num * 2f);
			float num2 = 100f;
			Bounds3 bounds = default(Bounds3);
			((Bounds3)(ref bounds))._002Ector(val - num2, val + num2);
			CarNavigationHelpers.FindLaneIterator findLaneIterator = new CarNavigationHelpers.FindLaneIterator
			{
				m_Bounds = bounds,
				m_Position = val,
				m_MinDistance = num2,
				m_Result = currentLane,
				m_CarType = RoadTypes.Car,
				m_SubLanes = m_Lanes,
				m_AreaNodes = m_AreaNodes,
				m_AreaTriangles = m_AreaTriangles,
				m_CarLaneData = m_CarLaneData,
				m_MasterLaneData = m_MasterLaneData,
				m_ConnectionLaneData = m_ConnectionLaneData,
				m_CurveData = m_CurveData,
				m_PrefabRefData = m_PrefabRefData,
				m_PrefabCarLaneData = m_PrefabCarLaneData
			};
			m_NetSearchTree.Iterate<CarNavigationHelpers.FindLaneIterator>(ref findLaneIterator, 0);
			m_StaticObjectSearchTree.Iterate<CarNavigationHelpers.FindLaneIterator>(ref findLaneIterator, 0);
			m_AreaSearchTree.Iterate<CarNavigationHelpers.FindLaneIterator>(ref findLaneIterator, 0);
			currentLane = findLaneIterator.m_Result;
		}

		private void TryFindCurrentLane(ref CarTrailerLane trailerLane, Transform transform, Moving moving)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
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
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			float num = 4f / 15f;
			float3 val = transform.m_Position + moving.m_Velocity * (num * 2f);
			float num2 = 100f;
			Bounds3 bounds = default(Bounds3);
			((Bounds3)(ref bounds))._002Ector(val - num2, val + num2);
			CarNavigationHelpers.FindLaneIterator findLaneIterator = new CarNavigationHelpers.FindLaneIterator
			{
				m_Bounds = bounds,
				m_Position = val,
				m_MinDistance = num2,
				m_CarType = RoadTypes.Car,
				m_SubLanes = m_Lanes,
				m_AreaNodes = m_AreaNodes,
				m_AreaTriangles = m_AreaTriangles,
				m_CarLaneData = m_CarLaneData,
				m_MasterLaneData = m_MasterLaneData,
				m_ConnectionLaneData = m_ConnectionLaneData,
				m_CurveData = m_CurveData,
				m_PrefabRefData = m_PrefabRefData,
				m_PrefabCarLaneData = m_PrefabCarLaneData
			};
			m_NetSearchTree.Iterate<CarNavigationHelpers.FindLaneIterator>(ref findLaneIterator, 0);
			m_StaticObjectSearchTree.Iterate<CarNavigationHelpers.FindLaneIterator>(ref findLaneIterator, 0);
			m_AreaSearchTree.Iterate<CarNavigationHelpers.FindLaneIterator>(ref findLaneIterator, 0);
			trailerLane.m_Lane = findLaneIterator.m_Result.m_Lane;
			trailerLane.m_CurvePosition = ((float3)(ref findLaneIterator.m_Result.m_CurvePosition)).xy;
			trailerLane.m_NextLane = Entity.Null;
			trailerLane.m_NextPosition = default(float2);
		}

		private void FillNavigationPaths(ref Random random, int priority, Entity entity, Transform transform, Target target, Car car, ref CarLaneSelectBuffer laneSelectBuffer, ref CarCurrentLane currentLane, ref Blocker blocker, ref PathOwner pathOwner, DynamicBuffer<CarNavigationLane> navigationLanes, DynamicBuffer<PathElement> pathElements, ref int invalidPath)
		{
			//IL_0a8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a94: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0abb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0add: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0afc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b04: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b09: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b10: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b11: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b1a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b1f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a16: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a3a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a41: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_065f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0495: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0593: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0737: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_050d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0822: Unknown result type (might be due to invalid IL or missing references)
			//IL_079b: Unknown result type (might be due to invalid IL or missing references)
			//IL_074d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_060e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0618: Unknown result type (might be due to invalid IL or missing references)
			//IL_061d: Unknown result type (might be due to invalid IL or missing references)
			//IL_062e: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0901: Unknown result type (might be due to invalid IL or missing references)
			//IL_08cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_090e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0914: Unknown result type (might be due to invalid IL or missing references)
			//IL_091b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0967: Unknown result type (might be due to invalid IL or missing references)
			//IL_093e: Unknown result type (might be due to invalid IL or missing references)
			if ((currentLane.m_LaneFlags & (Game.Vehicles.CarLaneFlags.EndOfPath | Game.Vehicles.CarLaneFlags.ParkingSpace | Game.Vehicles.CarLaneFlags.Waypoint)) == 0)
			{
				Owner owner = default(Owner);
				Game.Net.ParkingLane parkingLane2 = default(Game.Net.ParkingLane);
				Game.Net.ParkingLane parkingLane3 = default(Game.Net.ParkingLane);
				for (int i = 0; i <= 8; i++)
				{
					if (i >= navigationLanes.Length)
					{
						if (i == 8)
						{
							if ((pathOwner.m_State & PathFlags.Pending) != 0)
							{
								break;
							}
							int num = math.min(40000, pathElements.Length - pathOwner.m_ElementIndex);
							if (num <= 0)
							{
								break;
							}
							int num2 = ((Random)(ref random)).NextInt(num) * (((Random)(ref random)).NextInt(num) + 1) / num;
							PathElement pathElement = pathElements[pathOwner.m_ElementIndex + num2];
							if (((EntityStorageInfoLookup)(ref m_EntityStorageInfoLookup)).Exists(pathElement.m_Target))
							{
								break;
							}
							invalidPath = navigationLanes.Length;
							return;
						}
						i = navigationLanes.Length;
						if (pathOwner.m_ElementIndex >= pathElements.Length)
						{
							if ((pathOwner.m_State & PathFlags.Pending) != 0)
							{
								break;
							}
							CarNavigationLane navLaneData = default(CarNavigationLane);
							if (i > 0)
							{
								CarNavigationLane carNavigationLane = navigationLanes[i - 1];
								if ((carNavigationLane.m_Flags & Game.Vehicles.CarLaneFlags.TransformTarget) == 0 && (car.m_Flags & (CarFlags.StayOnRoad | CarFlags.AnyLaneTarget)) != (CarFlags.StayOnRoad | CarFlags.AnyLaneTarget) && GetTransformTarget(ref navLaneData.m_Lane, target))
								{
									if ((carNavigationLane.m_Flags & Game.Vehicles.CarLaneFlags.GroupTarget) == 0)
									{
										Entity lane = navLaneData.m_Lane;
										navLaneData.m_Lane = carNavigationLane.m_Lane;
										navLaneData.m_Flags = carNavigationLane.m_Flags & (Game.Vehicles.CarLaneFlags.Connection | Game.Vehicles.CarLaneFlags.Area);
										navLaneData.m_CurvePosition = ((float2)(ref carNavigationLane.m_CurvePosition)).yy;
										float3 position = default(float3);
										if (VehicleUtils.CalculateTransformPosition(ref position, lane, m_TransformData, m_PositionData, m_PrefabRefData, m_PrefabBuildingData))
										{
											UpdateSlaveLane(ref navLaneData, position);
										}
										if ((car.m_Flags & CarFlags.StayOnRoad) != 0)
										{
											navLaneData.m_Flags |= Game.Vehicles.CarLaneFlags.EndOfPath | Game.Vehicles.CarLaneFlags.GroupTarget;
											navigationLanes.Add(navLaneData);
											currentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.UpdateOptimalLane;
											break;
										}
										navLaneData.m_Flags |= Game.Vehicles.CarLaneFlags.GroupTarget;
										navigationLanes.Add(navLaneData);
										currentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.UpdateOptimalLane;
										continue;
									}
									navLaneData.m_Flags |= Game.Vehicles.CarLaneFlags.EndOfPath | Game.Vehicles.CarLaneFlags.TransformTarget;
									navigationLanes.Add(navLaneData);
									currentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.UpdateOptimalLane;
									break;
								}
								carNavigationLane.m_Flags |= Game.Vehicles.CarLaneFlags.EndOfPath;
								navigationLanes[i - 1] = carNavigationLane;
								currentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.UpdateOptimalLane;
								break;
							}
							if ((currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.TransformTarget) != 0 || (car.m_Flags & CarFlags.StayOnRoad) != 0 || !GetTransformTarget(ref navLaneData.m_Lane, target))
							{
								currentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.EndOfPath;
								break;
							}
							navLaneData.m_Flags |= Game.Vehicles.CarLaneFlags.EndOfPath | Game.Vehicles.CarLaneFlags.TransformTarget;
							navigationLanes.Add(navLaneData);
							currentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.UpdateOptimalLane;
							break;
						}
						PathElement pathElement2 = pathElements[pathOwner.m_ElementIndex++];
						CarNavigationLane navLaneData2 = new CarNavigationLane
						{
							m_Lane = pathElement2.m_Target,
							m_CurvePosition = pathElement2.m_TargetDelta
						};
						if (!m_CarLaneData.HasComponent(navLaneData2.m_Lane))
						{
							if (m_ParkingLaneData.HasComponent(navLaneData2.m_Lane))
							{
								Game.Net.ParkingLane parkingLane = m_ParkingLaneData[navLaneData2.m_Lane];
								navLaneData2.m_Flags |= Game.Vehicles.CarLaneFlags.ParkingSpace | Game.Vehicles.CarLaneFlags.FixedLane;
								if ((parkingLane.m_Flags & ParkingLaneFlags.ParkingLeft) != 0)
								{
									navLaneData2.m_Flags |= Game.Vehicles.CarLaneFlags.TurnLeft;
								}
								if ((parkingLane.m_Flags & ParkingLaneFlags.ParkingRight) != 0)
								{
									navLaneData2.m_Flags |= Game.Vehicles.CarLaneFlags.TurnRight;
								}
								navigationLanes.Add(navLaneData2);
								if (i > 0)
								{
									float3 targetPosition = MathUtils.Position(m_CurveData[navLaneData2.m_Lane].m_Bezier, navLaneData2.m_CurvePosition.y);
									CarNavigationLane navLaneData3 = navigationLanes[i - 1];
									UpdateSlaveLane(ref navLaneData3, targetPosition);
									navigationLanes[i - 1] = navLaneData3;
								}
								currentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.UpdateOptimalLane;
								break;
							}
							if (m_ConnectionLaneData.HasComponent(navLaneData2.m_Lane))
							{
								Game.Net.ConnectionLane connectionLane = m_ConnectionLaneData[navLaneData2.m_Lane];
								navLaneData2.m_Flags |= Game.Vehicles.CarLaneFlags.FixedLane;
								if ((connectionLane.m_Flags & ConnectionLaneFlags.Area) != 0)
								{
									navLaneData2.m_Flags |= Game.Vehicles.CarLaneFlags.Area;
								}
								else
								{
									navLaneData2.m_Flags |= Game.Vehicles.CarLaneFlags.Connection;
								}
								currentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.UpdateOptimalLane;
								if ((connectionLane.m_Flags & ConnectionLaneFlags.Parking) != 0)
								{
									navLaneData2.m_Flags |= Game.Vehicles.CarLaneFlags.ParkingSpace;
									navigationLanes.Add(navLaneData2);
									break;
								}
								navigationLanes.Add(navLaneData2);
								continue;
							}
							if (m_LaneData.HasComponent(navLaneData2.m_Lane))
							{
								if (pathOwner.m_ElementIndex >= pathElements.Length && (pathOwner.m_State & PathFlags.Pending) != 0)
								{
									pathOwner.m_ElementIndex--;
									break;
								}
								if (i > 0)
								{
									float3 targetPosition2 = MathUtils.Position(m_CurveData[navLaneData2.m_Lane].m_Bezier, navLaneData2.m_CurvePosition.y);
									CarNavigationLane navLaneData4 = navigationLanes[i - 1];
									UpdateSlaveLane(ref navLaneData4, targetPosition2);
									navLaneData4.m_Flags |= Game.Vehicles.CarLaneFlags.Waypoint;
									if (pathOwner.m_ElementIndex >= pathElements.Length)
									{
										navLaneData4.m_Flags |= Game.Vehicles.CarLaneFlags.EndOfPath;
									}
									navigationLanes[i - 1] = navLaneData4;
								}
								else
								{
									currentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.Waypoint;
									if (pathOwner.m_ElementIndex >= pathElements.Length)
									{
										currentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.EndOfPath;
									}
								}
								currentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.UpdateOptimalLane;
								break;
							}
							if (m_TransformData.HasComponent(navLaneData2.m_Lane))
							{
								if (pathOwner.m_ElementIndex >= pathElements.Length && (pathOwner.m_State & PathFlags.Pending) != 0)
								{
									pathOwner.m_ElementIndex--;
									break;
								}
								if ((car.m_Flags & CarFlags.StayOnRoad) == 0 || pathElements.Length > pathOwner.m_ElementIndex)
								{
									navLaneData2.m_Flags |= Game.Vehicles.CarLaneFlags.TransformTarget;
									navigationLanes.Add(navLaneData2);
									if (i > 0)
									{
										float3 position2 = m_TransformData[navLaneData2.m_Lane].m_Position;
										CarNavigationLane navLaneData5 = navigationLanes[i - 1];
										UpdateSlaveLane(ref navLaneData5, position2);
										navigationLanes[i - 1] = navLaneData5;
									}
									currentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.UpdateOptimalLane;
								}
								continue;
							}
							invalidPath = i;
							return;
						}
						Game.Net.CarLane carLane = m_CarLaneData[navLaneData2.m_Lane];
						if ((carLane.m_Flags & Game.Net.CarLaneFlags.Forward) == 0)
						{
							bool flag = (carLane.m_Flags & (Game.Net.CarLaneFlags.UTurnLeft | Game.Net.CarLaneFlags.TurnLeft | Game.Net.CarLaneFlags.GentleTurnLeft)) != 0;
							bool flag2 = (carLane.m_Flags & (Game.Net.CarLaneFlags.TurnRight | Game.Net.CarLaneFlags.UTurnRight | Game.Net.CarLaneFlags.GentleTurnRight)) != 0;
							if (flag && !flag2)
							{
								navLaneData2.m_Flags |= Game.Vehicles.CarLaneFlags.TurnLeft;
							}
							if (flag2 && !flag)
							{
								navLaneData2.m_Flags |= Game.Vehicles.CarLaneFlags.TurnRight;
							}
						}
						if ((carLane.m_Flags & (Game.Net.CarLaneFlags.Approach | Game.Net.CarLaneFlags.Roundabout)) == Game.Net.CarLaneFlags.Roundabout)
						{
							navLaneData2.m_Flags |= Game.Vehicles.CarLaneFlags.Roundabout;
						}
						if ((carLane.m_Flags & Game.Net.CarLaneFlags.Twoway) != 0)
						{
							navLaneData2.m_Flags |= Game.Vehicles.CarLaneFlags.CanReverse;
						}
						if ((carLane.m_Flags & Game.Net.CarLaneFlags.Unsafe) != 0 && ((carLane.m_Flags & (Game.Net.CarLaneFlags.UTurnLeft | Game.Net.CarLaneFlags.UTurnRight)) != 0 || (m_OwnerData.TryGetComponent(navLaneData2.m_Lane, ref owner) && m_CurveData.HasComponent(owner.m_Owner))))
						{
							navLaneData2.m_Flags |= Game.Vehicles.CarLaneFlags.RequestSpace;
						}
						navLaneData2.m_Flags |= Game.Vehicles.CarLaneFlags.UpdateOptimalLane;
						currentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.UpdateOptimalLane;
						if (i == 0)
						{
							if ((currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.ParkingSpace) != 0 && m_ParkingLaneData.TryGetComponent(currentLane.m_Lane, ref parkingLane2))
							{
								currentLane.m_LaneFlags &= ~(Game.Vehicles.CarLaneFlags.TurnLeft | Game.Vehicles.CarLaneFlags.TurnRight);
								if ((parkingLane2.m_Flags & ParkingLaneFlags.ParkingRight) != 0)
								{
									currentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.TurnLeft;
								}
								if ((parkingLane2.m_Flags & ParkingLaneFlags.ParkingLeft) != 0)
								{
									currentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.TurnRight;
								}
							}
						}
						else
						{
							CarNavigationLane carNavigationLane2 = navigationLanes[i - 1];
							if ((carNavigationLane2.m_Flags & Game.Vehicles.CarLaneFlags.ParkingSpace) != 0 && m_ParkingLaneData.TryGetComponent(carNavigationLane2.m_Lane, ref parkingLane3))
							{
								carNavigationLane2.m_Flags &= ~(Game.Vehicles.CarLaneFlags.TurnLeft | Game.Vehicles.CarLaneFlags.TurnRight);
								if ((parkingLane3.m_Flags & ParkingLaneFlags.ParkingRight) != 0)
								{
									carNavigationLane2.m_Flags |= Game.Vehicles.CarLaneFlags.TurnLeft;
								}
								if ((parkingLane3.m_Flags & ParkingLaneFlags.ParkingLeft) != 0)
								{
									carNavigationLane2.m_Flags |= Game.Vehicles.CarLaneFlags.TurnRight;
								}
								navigationLanes[i - 1] = carNavigationLane2;
							}
						}
						if (i == 0 && (currentLane.m_LaneFlags & (Game.Vehicles.CarLaneFlags.FixedLane | Game.Vehicles.CarLaneFlags.Connection)) == Game.Vehicles.CarLaneFlags.FixedLane)
						{
							GetSlaveLaneFromMasterLane(ref random, ref navLaneData2, currentLane);
						}
						else
						{
							GetSlaveLaneFromMasterLane(ref random, ref navLaneData2);
						}
						if ((pathElement2.m_Flags & PathElementFlags.PathStart) != 0)
						{
							Entity lane2;
							float prevCurvePos;
							if (i == 0)
							{
								lane2 = currentLane.m_Lane;
								prevCurvePos = currentLane.m_CurvePosition.z;
							}
							else
							{
								lane2 = navigationLanes[i - 1].m_Lane;
								prevCurvePos = navigationLanes[i - 1].m_CurvePosition.y;
							}
							if (IsContinuous(lane2, prevCurvePos, pathElement2.m_Target, pathElement2.m_TargetDelta.x, out var sameLane))
							{
								if (sameLane)
								{
									if (i == 0)
									{
										currentLane.m_CurvePosition.z = pathElement2.m_TargetDelta.y;
										continue;
									}
									CarNavigationLane carNavigationLane3 = navigationLanes[i - 1];
									carNavigationLane3.m_CurvePosition.y = pathElement2.m_TargetDelta.y;
									navigationLanes[i - 1] = carNavigationLane3;
									continue;
								}
							}
							else
							{
								navLaneData2.m_Flags |= Game.Vehicles.CarLaneFlags.Interruption;
							}
						}
						navigationLanes.Add(navLaneData2);
					}
					else
					{
						CarNavigationLane carNavigationLane4 = navigationLanes[i];
						if (!((EntityStorageInfoLookup)(ref m_EntityStorageInfoLookup)).Exists(carNavigationLane4.m_Lane))
						{
							invalidPath = i;
							return;
						}
						if ((carNavigationLane4.m_Flags & (Game.Vehicles.CarLaneFlags.EndOfPath | Game.Vehicles.CarLaneFlags.ParkingSpace | Game.Vehicles.CarLaneFlags.Waypoint)) != 0)
						{
							break;
						}
					}
				}
			}
			if ((currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.UpdateOptimalLane) == 0)
			{
				return;
			}
			currentLane.m_LaneFlags &= ~Game.Vehicles.CarLaneFlags.UpdateOptimalLane;
			if ((currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.IsBlocked) != 0)
			{
				if (IsBlockedLane(currentLane.m_Lane, ((float3)(ref currentLane.m_CurvePosition)).xz))
				{
					invalidPath = -1;
					return;
				}
				for (int j = 0; j < navigationLanes.Length; j++)
				{
					CarNavigationLane carNavigationLane5 = navigationLanes[j];
					if (IsBlockedLane(carNavigationLane5.m_Lane, carNavigationLane5.m_CurvePosition))
					{
						invalidPath = j;
						return;
					}
				}
				currentLane.m_LaneFlags &= ~(Game.Vehicles.CarLaneFlags.FixedLane | Game.Vehicles.CarLaneFlags.IsBlocked);
				currentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.IgnoreBlocker;
			}
			CarLaneSelectIterator carLaneSelectIterator = new CarLaneSelectIterator
			{
				m_OwnerData = m_OwnerData,
				m_LaneData = m_LaneData,
				m_CarLaneData = m_CarLaneData,
				m_SlaveLaneData = m_SlaveLaneData,
				m_LaneReservationData = m_LaneReservationData,
				m_MovingData = m_MovingData,
				m_CarData = m_CarData,
				m_ControllerData = m_ControllerData,
				m_Lanes = m_Lanes,
				m_LaneObjects = m_LaneObjects,
				m_Entity = entity,
				m_Blocker = blocker.m_Blocker,
				m_Priority = priority,
				m_ForbidLaneFlags = VehicleUtils.GetForbiddenLaneFlags(car),
				m_PreferLaneFlags = VehicleUtils.GetPreferredLaneFlags(car)
			};
			carLaneSelectIterator.SetBuffer(ref laneSelectBuffer);
			if (navigationLanes.Length != 0)
			{
				CarNavigationLane carNavigationLane6 = navigationLanes[navigationLanes.Length - 1];
				carLaneSelectIterator.CalculateLaneCosts(carNavigationLane6, navigationLanes.Length - 1);
				for (int num3 = navigationLanes.Length - 2; num3 >= 0; num3--)
				{
					CarNavigationLane carNavigationLane7 = navigationLanes[num3];
					carLaneSelectIterator.CalculateLaneCosts(carNavigationLane7, carNavigationLane6, num3);
					carNavigationLane6 = carNavigationLane7;
				}
				carLaneSelectIterator.UpdateOptimalLane(ref currentLane, navigationLanes[0]);
				for (int k = 0; k < navigationLanes.Length; k++)
				{
					CarNavigationLane navLaneData6 = navigationLanes[k];
					carLaneSelectIterator.UpdateOptimalLane(ref navLaneData6);
					navLaneData6.m_Flags &= ~Game.Vehicles.CarLaneFlags.Reserved;
					navigationLanes[k] = navLaneData6;
				}
			}
			else if (currentLane.m_CurvePosition.x != currentLane.m_CurvePosition.z)
			{
				carLaneSelectIterator.UpdateOptimalLane(ref currentLane, default(CarNavigationLane));
			}
		}

		private bool IsContinuous(Entity prevLane, float prevCurvePos, Entity pathTarget, float nextCurvePos, out bool sameLane)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			sameLane = false;
			if (m_SlaveLaneData.HasComponent(prevLane))
			{
				SlaveLane slaveLane = m_SlaveLaneData[prevLane];
				Entity owner = m_OwnerData[prevLane].m_Owner;
				prevLane = m_Lanes[owner][(int)slaveLane.m_MasterIndex].m_SubLane;
				if (!m_MasterLaneData.HasComponent(prevLane))
				{
					return false;
				}
			}
			if (prevLane == pathTarget && prevCurvePos == nextCurvePos)
			{
				sameLane = true;
				return true;
			}
			if (!m_LaneData.HasComponent(prevLane) || !m_LaneData.HasComponent(pathTarget))
			{
				return false;
			}
			Lane lane = m_LaneData[prevLane];
			Lane lane2 = m_LaneData[pathTarget];
			return lane.m_EndNode.Equals(lane2.m_StartNode);
		}

		private bool IsBlockedLane(Entity lane, float2 range)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			if (m_SlaveLaneData.HasComponent(lane))
			{
				SlaveLane slaveLane = m_SlaveLaneData[lane];
				Entity owner = m_OwnerData[lane].m_Owner;
				lane = m_Lanes[owner][(int)slaveLane.m_MasterIndex].m_SubLane;
				if (!m_MasterLaneData.HasComponent(lane))
				{
					return false;
				}
			}
			if (!m_CarLaneData.HasComponent(lane))
			{
				return false;
			}
			Game.Net.CarLane carLane = m_CarLaneData[lane];
			if (carLane.m_BlockageEnd < carLane.m_BlockageStart)
			{
				return false;
			}
			if (math.min(range.x, range.y) <= (float)(int)carLane.m_BlockageEnd * 0.003921569f)
			{
				return math.max(range.x, range.y) >= (float)(int)carLane.m_BlockageStart * 0.003921569f;
			}
			return false;
		}

		private bool GetTransformTarget(ref Entity entity, Target target)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			if (m_PropertyRenterData.HasComponent(target.m_Target))
			{
				target.m_Target = m_PropertyRenterData[target.m_Target].m_Property;
			}
			if (m_TransformData.HasComponent(target.m_Target))
			{
				entity = target.m_Target;
				return true;
			}
			if (m_PositionData.HasComponent(target.m_Target))
			{
				entity = target.m_Target;
				return true;
			}
			return false;
		}

		private void UpdateSlaveLane(ref CarNavigationLane navLaneData, float3 targetPosition)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			if (m_SlaveLaneData.HasComponent(navLaneData.m_Lane))
			{
				SlaveLane slaveLane = m_SlaveLaneData[navLaneData.m_Lane];
				Entity owner = m_OwnerData[navLaneData.m_Lane].m_Owner;
				DynamicBuffer<Game.Net.SubLane> lanes = m_Lanes[owner];
				int num = NetUtils.ChooseClosestLane(slaveLane.m_MinIndex, slaveLane.m_MaxIndex, targetPosition, lanes, m_CurveData, navLaneData.m_CurvePosition.y);
				navLaneData.m_Lane = lanes[num].m_SubLane;
			}
			navLaneData.m_Flags |= Game.Vehicles.CarLaneFlags.FixedLane;
		}

		private void GetSlaveLaneFromMasterLane(ref Random random, ref CarNavigationLane navLaneData, CarCurrentLane currentLaneData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			if (m_MasterLaneData.HasComponent(navLaneData.m_Lane))
			{
				MasterLane masterLane = m_MasterLaneData[navLaneData.m_Lane];
				Owner owner = m_OwnerData[navLaneData.m_Lane];
				DynamicBuffer<Game.Net.SubLane> lanes = m_Lanes[owner.m_Owner];
				if ((currentLaneData.m_LaneFlags & Game.Vehicles.CarLaneFlags.TransformTarget) != 0)
				{
					float3 position = default(float3);
					if (VehicleUtils.CalculateTransformPosition(ref position, currentLaneData.m_Lane, m_TransformData, m_PositionData, m_PrefabRefData, m_PrefabBuildingData))
					{
						int num = NetUtils.ChooseClosestLane(masterLane.m_MinIndex, masterLane.m_MaxIndex, position, lanes, m_CurveData, navLaneData.m_CurvePosition.y);
						navLaneData.m_Lane = lanes[num].m_SubLane;
						navLaneData.m_Flags |= Game.Vehicles.CarLaneFlags.FixedStart;
					}
					else
					{
						int num2 = ((Random)(ref random)).NextInt((int)masterLane.m_MinIndex, masterLane.m_MaxIndex + 1);
						navLaneData.m_Lane = lanes[num2].m_SubLane;
					}
				}
				else
				{
					float3 comparePosition = MathUtils.Position(m_CurveData[currentLaneData.m_Lane].m_Bezier, currentLaneData.m_CurvePosition.z);
					int num3 = NetUtils.ChooseClosestLane(masterLane.m_MinIndex, masterLane.m_MaxIndex, comparePosition, lanes, m_CurveData, navLaneData.m_CurvePosition.x);
					navLaneData.m_Lane = lanes[num3].m_SubLane;
					navLaneData.m_Flags |= Game.Vehicles.CarLaneFlags.FixedStart;
				}
			}
			else
			{
				navLaneData.m_Flags |= Game.Vehicles.CarLaneFlags.FixedLane;
			}
		}

		private void GetSlaveLaneFromMasterLane(ref Random random, ref CarNavigationLane navLaneData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			if (m_MasterLaneData.HasComponent(navLaneData.m_Lane))
			{
				MasterLane masterLane = m_MasterLaneData[navLaneData.m_Lane];
				Entity owner = m_OwnerData[navLaneData.m_Lane].m_Owner;
				DynamicBuffer<Game.Net.SubLane> val = m_Lanes[owner];
				int num = ((Random)(ref random)).NextInt((int)masterLane.m_MinIndex, masterLane.m_MaxIndex + 1);
				navLaneData.m_Lane = val[num].m_SubLane;
			}
			else
			{
				navLaneData.m_Flags |= Game.Vehicles.CarLaneFlags.FixedLane;
			}
		}

		private bool GetNextLane(Entity prevLane, Entity nextLane, out Entity selectedLane)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			SlaveLane slaveLane = default(SlaveLane);
			Lane lane = default(Lane);
			if (m_SlaveLaneData.TryGetComponent(nextLane, ref slaveLane) && m_LaneData.TryGetComponent(prevLane, ref lane))
			{
				Entity owner = m_OwnerData[nextLane].m_Owner;
				DynamicBuffer<Game.Net.SubLane> val = m_Lanes[owner];
				int num = math.min((int)slaveLane.m_MaxIndex, val.Length - 1);
				for (int i = slaveLane.m_MinIndex; i <= num; i++)
				{
					if (m_LaneData[val[i].m_SubLane].m_StartNode.Equals(lane.m_EndNode))
					{
						selectedLane = val[i].m_SubLane;
						return true;
					}
				}
			}
			selectedLane = Entity.Null;
			return false;
		}

		private void CheckBlocker(ref CarCurrentLane currentLane, ref Blocker blocker, ref CarLaneSpeedIterator laneIterator)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			if (laneIterator.m_Blocker != blocker.m_Blocker)
			{
				currentLane.m_LaneFlags &= ~(Game.Vehicles.CarLaneFlags.IgnoreBlocker | Game.Vehicles.CarLaneFlags.QueueReached);
			}
			if (laneIterator.m_Blocker != Entity.Null)
			{
				if (!m_MovingData.HasComponent(laneIterator.m_Blocker))
				{
					if (m_CarData.HasComponent(laneIterator.m_Blocker))
					{
						if ((m_CarData[laneIterator.m_Blocker].m_Flags & CarFlags.Queueing) != 0 && (currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.Queue) != 0)
						{
							if (laneIterator.m_MaxSpeed <= 3f)
							{
								currentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.QueueReached;
							}
						}
						else
						{
							currentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.UpdateOptimalLane;
							if (laneIterator.m_MaxSpeed <= 3f)
							{
								currentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.IsBlocked;
							}
						}
					}
					else
					{
						currentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.UpdateOptimalLane;
						if (laneIterator.m_MaxSpeed <= 3f)
						{
							currentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.IsBlocked;
						}
					}
				}
				else if (laneIterator.m_Blocker != blocker.m_Blocker)
				{
					currentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.UpdateOptimalLane;
				}
			}
			blocker.m_Blocker = laneIterator.m_Blocker;
			blocker.m_Type = laneIterator.m_BlockerType;
			blocker.m_MaxSpeed = (byte)math.clamp(Mathf.RoundToInt(laneIterator.m_MaxSpeed * 2.2949998f), 0, 255);
		}

		private void UpdateTrailer(Entity entity, Transform transform, ObjectGeometryData prefabObjectGeometryData, Entity nextLane, float2 nextPosition, bool forceNext, ref CarTrailerLane trailerLane)
		{
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			if (forceNext)
			{
				trailerLane.m_Lane = nextLane;
				trailerLane.m_CurvePosition = nextPosition;
				trailerLane.m_NextLane = Entity.Null;
				trailerLane.m_NextPosition = default(float2);
				if (m_CurveData.HasComponent(nextLane))
				{
					MathUtils.Distance(m_CurveData[nextLane].m_Bezier, transform.m_Position, ref trailerLane.m_CurvePosition.x);
				}
				return;
			}
			if (nextLane != Entity.Null)
			{
				if (trailerLane.m_Lane == nextLane)
				{
					trailerLane.m_CurvePosition.y = nextPosition.y;
					trailerLane.m_NextLane = Entity.Null;
					trailerLane.m_NextPosition = default(float2);
					nextLane = Entity.Null;
					nextPosition = default(float2);
				}
				else if (trailerLane.m_NextLane == nextLane)
				{
					trailerLane.m_NextPosition.y = nextPosition.y;
					nextLane = Entity.Null;
					nextPosition = default(float2);
				}
				else if (trailerLane.m_NextLane == Entity.Null)
				{
					trailerLane.m_NextLane = nextLane;
					trailerLane.m_NextPosition = nextPosition;
					nextLane = Entity.Null;
					nextPosition = default(float2);
				}
			}
			float3 val = float3.op_Implicit(float.MaxValue);
			float3 val2 = default(float3);
			if (m_CurveData.HasComponent(trailerLane.m_Lane))
			{
				val.x = MathUtils.Distance(m_CurveData[trailerLane.m_Lane].m_Bezier, transform.m_Position, ref val2.x);
			}
			if (m_CurveData.HasComponent(trailerLane.m_NextLane))
			{
				val.y = MathUtils.Distance(m_CurveData[trailerLane.m_NextLane].m_Bezier, transform.m_Position, ref val2.y);
			}
			if (m_CurveData.HasComponent(nextLane))
			{
				val.z = MathUtils.Distance(m_CurveData[nextLane].m_Bezier, transform.m_Position, ref val2.z);
			}
			if (math.all(val.z < ((float3)(ref val)).xy) || forceNext)
			{
				trailerLane.m_Lane = nextLane;
				trailerLane.m_CurvePosition = new float2(val2.z, nextPosition.y);
				trailerLane.m_NextLane = Entity.Null;
				trailerLane.m_NextPosition = default(float2);
			}
			else if (val.y < val.x)
			{
				trailerLane.m_Lane = trailerLane.m_NextLane;
				trailerLane.m_CurvePosition = new float2(val2.y, trailerLane.m_NextPosition.y);
				trailerLane.m_NextLane = nextLane;
				trailerLane.m_NextPosition = nextPosition;
			}
			else
			{
				trailerLane.m_CurvePosition.x = val2.x;
			}
		}

		private void UpdateNavigationTarget(ref Random random, int priority, Entity entity, Transform transform, Moving moving, Car car, PseudoRandomSeed pseudoRandomSeed, PrefabRef prefabRef, CarData prefabCarData, ObjectGeometryData prefabObjectGeometryData, ref CarNavigation navigation, ref CarCurrentLane currentLane, ref Blocker blocker, ref Odometer odometer, ref PathOwner pathOwner, ref NativeList<Entity> tempBuffer, DynamicBuffer<CarNavigationLane> navigationLanes, DynamicBuffer<PathElement> pathElements)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
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
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_081c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0821: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0857: Unknown result type (might be due to invalid IL or missing references)
			//IL_085b: Unknown result type (might be due to invalid IL or missing references)
			//IL_082c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0830: Unknown result type (might be due to invalid IL or missing references)
			//IL_0841: Unknown result type (might be due to invalid IL or missing references)
			//IL_0846: Unknown result type (might be due to invalid IL or missing references)
			//IL_084b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0850: Unknown result type (might be due to invalid IL or missing references)
			//IL_0482: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0496: Unknown result type (might be due to invalid IL or missing references)
			//IL_0452: Unknown result type (might be due to invalid IL or missing references)
			//IL_0915: Unknown result type (might be due to invalid IL or missing references)
			//IL_091a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0945: Unknown result type (might be due to invalid IL or missing references)
			//IL_0950: Unknown result type (might be due to invalid IL or missing references)
			//IL_0955: Unknown result type (might be due to invalid IL or missing references)
			//IL_0959: Unknown result type (might be due to invalid IL or missing references)
			//IL_098c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0521: Unknown result type (might be due to invalid IL or missing references)
			//IL_0526: Unknown result type (might be due to invalid IL or missing references)
			//IL_0511: Unknown result type (might be due to invalid IL or missing references)
			//IL_0516: Unknown result type (might be due to invalid IL or missing references)
			//IL_0518: Unknown result type (might be due to invalid IL or missing references)
			//IL_051d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_09dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_053a: Unknown result type (might be due to invalid IL or missing references)
			//IL_054e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0571: Unknown result type (might be due to invalid IL or missing references)
			//IL_0578: Unknown result type (might be due to invalid IL or missing references)
			//IL_057f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0584: Unknown result type (might be due to invalid IL or missing references)
			//IL_0586: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0add: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b13: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b18: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b26: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b2f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b34: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b39: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b7b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b96: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0656: Unknown result type (might be due to invalid IL or missing references)
			//IL_065b: Unknown result type (might be due to invalid IL or missing references)
			//IL_162c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1633: Unknown result type (might be due to invalid IL or missing references)
			//IL_139e: Unknown result type (might be due to invalid IL or missing references)
			//IL_13a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_13ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_13b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_13b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_13bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_13c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_13ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_13d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_13d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_13df: Unknown result type (might be due to invalid IL or missing references)
			//IL_13e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_13ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_13f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_13f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_13fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_1406: Unknown result type (might be due to invalid IL or missing references)
			//IL_140b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1413: Unknown result type (might be due to invalid IL or missing references)
			//IL_1418: Unknown result type (might be due to invalid IL or missing references)
			//IL_1420: Unknown result type (might be due to invalid IL or missing references)
			//IL_1425: Unknown result type (might be due to invalid IL or missing references)
			//IL_142d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1432: Unknown result type (might be due to invalid IL or missing references)
			//IL_143a: Unknown result type (might be due to invalid IL or missing references)
			//IL_143f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1453: Unknown result type (might be due to invalid IL or missing references)
			//IL_1454: Unknown result type (might be due to invalid IL or missing references)
			//IL_145d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1462: Unknown result type (might be due to invalid IL or missing references)
			//IL_148d: Unknown result type (might be due to invalid IL or missing references)
			//IL_148f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1498: Unknown result type (might be due to invalid IL or missing references)
			//IL_149d: Unknown result type (might be due to invalid IL or missing references)
			//IL_14a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_14ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_14bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_14c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_14e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_14ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c40: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c6c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c12: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c13: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0703: Unknown result type (might be due to invalid IL or missing references)
			//IL_070f: Unknown result type (might be due to invalid IL or missing references)
			//IL_073d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0744: Unknown result type (might be due to invalid IL or missing references)
			//IL_074d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0752: Unknown result type (might be due to invalid IL or missing references)
			//IL_0766: Unknown result type (might be due to invalid IL or missing references)
			//IL_15fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_1609: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d3c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d4e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d5f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d64: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d6d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d7e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d83: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d88: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d8d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c83: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c95: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc5: Unknown result type (might be due to invalid IL or missing references)
			//IL_078e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0791: Unknown result type (might be due to invalid IL or missing references)
			//IL_165d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1662: Unknown result type (might be due to invalid IL or missing references)
			//IL_1664: Unknown result type (might be due to invalid IL or missing references)
			//IL_1669: Unknown result type (might be due to invalid IL or missing references)
			//IL_151f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1559: Unknown result type (might be due to invalid IL or missing references)
			//IL_0efa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f02: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f07: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e5a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e8a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e96: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e98: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ea5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ece: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0de3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0de8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0def: Unknown result type (might be due to invalid IL or missing references)
			//IL_0df5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e06: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e08: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d92: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d96: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d1a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d1f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d28: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cdb: Unknown result type (might be due to invalid IL or missing references)
			//IL_16b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_16bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_16c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_16c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_167e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1685: Unknown result type (might be due to invalid IL or missing references)
			//IL_1697: Unknown result type (might be due to invalid IL or missing references)
			//IL_1699: Unknown result type (might be due to invalid IL or missing references)
			//IL_169b: Unknown result type (might be due to invalid IL or missing references)
			//IL_16a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_15b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_15b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_1569: Unknown result type (might be due to invalid IL or missing references)
			//IL_156e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1018: Unknown result type (might be due to invalid IL or missing references)
			//IL_102c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f1e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f32: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f46: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f5a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f6e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f82: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fb4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fc1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fc8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e2a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0db8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cef: Unknown result type (might be due to invalid IL or missing references)
			//IL_1134: Unknown result type (might be due to invalid IL or missing references)
			//IL_1141: Unknown result type (might be due to invalid IL or missing references)
			//IL_1071: Unknown result type (might be due to invalid IL or missing references)
			//IL_1076: Unknown result type (might be due to invalid IL or missing references)
			//IL_1082: Unknown result type (might be due to invalid IL or missing references)
			//IL_1087: Unknown result type (might be due to invalid IL or missing references)
			//IL_1186: Unknown result type (might be due to invalid IL or missing references)
			//IL_118a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1758: Unknown result type (might be due to invalid IL or missing references)
			//IL_175d: Unknown result type (might be due to invalid IL or missing references)
			//IL_16d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_16db: Unknown result type (might be due to invalid IL or missing references)
			//IL_16df: Unknown result type (might be due to invalid IL or missing references)
			//IL_16e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_16e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_11e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_177a: Unknown result type (might be due to invalid IL or missing references)
			//IL_177c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1780: Unknown result type (might be due to invalid IL or missing references)
			//IL_1786: Unknown result type (might be due to invalid IL or missing references)
			//IL_1788: Unknown result type (might be due to invalid IL or missing references)
			//IL_12cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_12d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_1282: Unknown result type (might be due to invalid IL or missing references)
			//IL_1323: Unknown result type (might be due to invalid IL or missing references)
			//IL_1328: Unknown result type (might be due to invalid IL or missing references)
			//IL_132f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1334: Unknown result type (might be due to invalid IL or missing references)
			//IL_12e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_12e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_1231: Unknown result type (might be due to invalid IL or missing references)
			//IL_1235: Unknown result type (might be due to invalid IL or missing references)
			//IL_134e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1353: Unknown result type (might be due to invalid IL or missing references)
			//IL_12f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_12fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_12a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_1308: Unknown result type (might be due to invalid IL or missing references)
			//IL_130a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1313: Unknown result type (might be due to invalid IL or missing references)
			//IL_1318: Unknown result type (might be due to invalid IL or missing references)
			float num = 4f / 15f;
			float num2 = math.length(moving.m_Velocity);
			float speedLimitFactor = VehicleUtils.GetSpeedLimitFactor(car);
			VehicleUtils.GetDrivingStyle(m_SimulationFrame, pseudoRandomSeed, out var safetyTime);
			if ((currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.Connection) != 0)
			{
				prefabCarData.m_MaxSpeed = 277.77777f;
				prefabCarData.m_Acceleration = 277.77777f;
				prefabCarData.m_Braking = 277.77777f;
			}
			else
			{
				num2 = math.min(num2, prefabCarData.m_MaxSpeed);
				if ((pathOwner.m_State & (PathFlags.Pending | PathFlags.Obsolete)) != 0)
				{
					prefabCarData.m_Acceleration = 0f;
				}
			}
			Bounds1 val = default(Bounds1);
			if ((currentLane.m_LaneFlags & (Game.Vehicles.CarLaneFlags.Connection | Game.Vehicles.CarLaneFlags.ResetSpeed)) != 0)
			{
				((Bounds1)(ref val))._002Ector(0f, prefabCarData.m_MaxSpeed);
			}
			else
			{
				val = VehicleUtils.CalculateSpeedRange(prefabCarData, num2, num);
			}
			bool flag = blocker.m_Type == BlockerType.Temporary;
			bool flag2 = math.asuint(navigation.m_MaxSpeed) >> 31 != 0;
			CarLaneSpeedIterator laneIterator = new CarLaneSpeedIterator
			{
				m_TransformData = m_TransformData,
				m_MovingData = m_MovingData,
				m_CarData = m_CarData,
				m_TrainData = m_TrainData,
				m_ControllerData = m_ControllerData,
				m_LaneReservationData = m_LaneReservationData,
				m_LaneConditionData = m_LaneConditionData,
				m_LaneSignalData = m_LaneSignalData,
				m_CurveData = m_CurveData,
				m_CarLaneData = m_CarLaneData,
				m_ParkingLaneData = m_ParkingLaneData,
				m_UnspawnedData = m_UnspawnedData,
				m_CreatureData = m_CreatureData,
				m_PrefabRefData = m_PrefabRefData,
				m_PrefabObjectGeometryData = m_PrefabObjectGeometryData,
				m_PrefabCarData = m_PrefabCarData,
				m_PrefabTrainData = m_PrefabTrainData,
				m_PrefabParkingLaneData = m_PrefabParkingLaneData,
				m_LaneOverlapData = m_LaneOverlaps,
				m_LaneObjectData = m_LaneObjects,
				m_Entity = entity,
				m_Ignore = (((currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.IgnoreBlocker) != 0) ? blocker.m_Blocker : Entity.Null),
				m_TempBuffer = tempBuffer,
				m_Priority = priority,
				m_TimeStep = num,
				m_SafeTimeStep = num + safetyTime,
				m_DistanceOffset = math.select(0f, math.max(-0.5f, -0.5f * math.lengthsq(1.5f - num2)), num2 < 1.5f),
				m_SpeedLimitFactor = speedLimitFactor,
				m_CurrentSpeed = num2,
				m_PrefabCar = prefabCarData,
				m_PrefabObjectGeometry = prefabObjectGeometryData,
				m_SpeedRange = val,
				m_PushBlockers = ((currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.PushBlockers) != 0),
				m_MaxSpeed = val.max,
				m_CanChangeLane = 1f,
				m_CurrentPosition = transform.m_Position
			};
			Game.Vehicles.CarLaneFlags carLaneFlags = (Game.Vehicles.CarLaneFlags)0u;
			Game.Net.CarLaneFlags laneFlags;
			if ((currentLane.m_LaneFlags & (Game.Vehicles.CarLaneFlags.TransformTarget | Game.Vehicles.CarLaneFlags.ParkingSpace)) != 0)
			{
				laneIterator.IterateTarget(navigation.m_TargetPosition);
				navigation.m_MaxSpeed = laneIterator.m_MaxSpeed;
				blocker.m_Blocker = Entity.Null;
				blocker.m_Type = BlockerType.None;
				blocker.m_MaxSpeed = byte.MaxValue;
			}
			else
			{
				if ((currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.Area) == 0)
				{
					if (currentLane.m_Lane == Entity.Null)
					{
						navigation.m_MaxSpeed = math.max(0f, num2 - prefabCarData.m_Braking * num);
						blocker.m_Blocker = Entity.Null;
						blocker.m_Type = BlockerType.None;
						blocker.m_MaxSpeed = byte.MaxValue;
						return;
					}
					PrefabRef prefabRef2 = m_PrefabRefData[currentLane.m_Lane];
					NetLaneData prefabLaneData = m_PrefabLaneData[prefabRef2.m_Prefab];
					float laneOffset = VehicleUtils.GetLaneOffset(prefabObjectGeometryData, prefabLaneData, currentLane.m_LanePosition);
					if ((currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.HighBeams) != 0)
					{
						if (!m_CarLaneData.HasComponent(currentLane.m_Lane) || !AllowHighBeams(transform, blocker, ref currentLane, navigationLanes, 100f, 2f))
						{
							currentLane.m_LaneFlags &= ~Game.Vehicles.CarLaneFlags.HighBeams;
						}
					}
					else if (m_CarLaneData.HasComponent(currentLane.m_Lane) && (m_CarLaneData[currentLane.m_Lane].m_Flags & Game.Net.CarLaneFlags.Highway) != 0 && !IsLit(currentLane.m_Lane) && AllowHighBeams(transform, blocker, ref currentLane, navigationLanes, 150f, 0f))
					{
						currentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.HighBeams;
					}
					Entity nextLane = Entity.Null;
					float2 nextOffset = float2.op_Implicit(0f);
					if (navigationLanes.Length > 0)
					{
						CarNavigationLane carNavigationLane = navigationLanes[0];
						nextLane = carNavigationLane.m_Lane;
						nextOffset = carNavigationLane.m_CurvePosition;
					}
					if (currentLane.m_ChangeLane != Entity.Null)
					{
						PrefabRef prefabRef3 = m_PrefabRefData[currentLane.m_ChangeLane];
						NetLaneData prefabLaneData2 = m_PrefabLaneData[prefabRef3.m_Prefab];
						float laneOffset2 = VehicleUtils.GetLaneOffset(prefabObjectGeometryData, prefabLaneData2, 0f - currentLane.m_LanePosition);
						if (!laneIterator.IterateFirstLane(currentLane.m_Lane, currentLane.m_ChangeLane, currentLane.m_CurvePosition, nextLane, nextOffset, currentLane.m_ChangeProgress, laneOffset, laneOffset2, (currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.RequestSpace) != 0, out laneFlags))
						{
							goto IL_05e3;
						}
					}
					else if (!laneIterator.IterateFirstLane(currentLane.m_Lane, currentLane.m_CurvePosition, nextLane, nextOffset, laneOffset, (currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.RequestSpace) != 0, out laneFlags))
					{
						goto IL_05e3;
					}
					goto IL_07c5;
				}
				navigation.m_TargetPosition.y = TerrainUtils.SampleHeight(ref m_TerrainHeightData, navigation.m_TargetPosition);
				laneIterator.IterateTarget(navigation.m_TargetPosition, 11.111112f);
				navigation.m_MaxSpeed = laneIterator.m_MaxSpeed;
				blocker.m_Blocker = Entity.Null;
				blocker.m_Type = BlockerType.None;
				blocker.m_MaxSpeed = byte.MaxValue;
			}
			goto IL_0802;
			IL_05e3:
			int num3 = 0;
			while (true)
			{
				if (num3 < navigationLanes.Length)
				{
					CarNavigationLane carNavigationLane2 = navigationLanes[num3];
					if ((carNavigationLane2.m_Flags & (Game.Vehicles.CarLaneFlags.TransformTarget | Game.Vehicles.CarLaneFlags.Area)) == 0)
					{
						if ((carNavigationLane2.m_Flags & Game.Vehicles.CarLaneFlags.Connection) != 0)
						{
							laneIterator.m_PrefabCar.m_MaxSpeed = 277.77777f;
							laneIterator.m_PrefabCar.m_Acceleration = 277.77777f;
							laneIterator.m_PrefabCar.m_Braking = 277.77777f;
							laneIterator.m_SpeedRange = new Bounds1(0f, 277.77777f);
						}
						else
						{
							if ((currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.Connection) != 0)
							{
								goto IL_07b7;
							}
							if ((carNavigationLane2.m_Flags & Game.Vehicles.CarLaneFlags.Interruption) != 0)
							{
								laneIterator.m_PrefabCar.m_MaxSpeed = 3f;
							}
						}
						if ((num3 == 0 || (carNavigationLane2.m_Flags & (Game.Vehicles.CarLaneFlags.ParkingSpace | Game.Vehicles.CarLaneFlags.Roundabout)) == 0) && carLaneFlags == (Game.Vehicles.CarLaneFlags)0u && (carNavigationLane2.m_Flags & (Game.Vehicles.CarLaneFlags.ParkingSpace | Game.Vehicles.CarLaneFlags.Validated)) != Game.Vehicles.CarLaneFlags.ParkingSpace)
						{
							carLaneFlags |= carNavigationLane2.m_Flags & (Game.Vehicles.CarLaneFlags.TurnLeft | Game.Vehicles.CarLaneFlags.TurnRight);
						}
						bool flag3 = (carNavigationLane2.m_Lane == currentLane.m_Lane) | (carNavigationLane2.m_Lane == currentLane.m_ChangeLane);
						float num4 = math.select(-1f, 2f, carNavigationLane2.m_CurvePosition.y < carNavigationLane2.m_CurvePosition.x);
						num4 = math.select(num4, currentLane.m_CurvePosition.y, flag3);
						bool needSignal;
						bool num5 = laneIterator.IterateNextLane(carNavigationLane2.m_Lane, carNavigationLane2.m_CurvePosition, num4, navigationLanes.AsNativeArray().GetSubArray(num3 + 1, navigationLanes.Length - 1 - num3), (carNavigationLane2.m_Flags & Game.Vehicles.CarLaneFlags.RequestSpace) != 0, ref laneFlags, out needSignal);
						if (needSignal)
						{
							m_LaneSignals.Enqueue(new CarNavigationHelpers.LaneSignal(entity, carNavigationLane2.m_Lane, priority));
						}
						if (num5)
						{
							break;
						}
						num3++;
						continue;
					}
				}
				goto IL_07b7;
				IL_07b7:
				laneIterator.IterateTarget(laneIterator.m_CurrentPosition);
				break;
			}
			goto IL_07c5;
			IL_0802:
			float num6 = math.select(prefabCarData.m_PivotOffset, 0f - prefabCarData.m_PivotOffset, flag2);
			float3 val2 = transform.m_Position;
			if (num6 < 0f)
			{
				val2 += math.rotate(transform.m_Rotation, new float3(0f, 0f, num6));
				num6 = 0f - num6;
			}
			float num7 = math.lerp(math.distance(val2, navigation.m_TargetPosition), 0f, laneIterator.m_Oncoming);
			float num8 = math.max(1f, navigation.m_MaxSpeed * num) + num6;
			float num9 = num8;
			if ((currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.Area) != 0)
			{
				float brakingDistance = VehicleUtils.GetBrakingDistance(prefabCarData, math.min(prefabCarData.m_MaxSpeed, 11.111112f), num);
				num9 = math.max(num8, brakingDistance + 1f + num6);
				num7 = math.select(num7, 0f, currentLane.m_ChangeProgress != 0f);
			}
			if (currentLane.m_ChangeLane != Entity.Null)
			{
				float num10 = 0.05f;
				float num11 = 1f + prefabObjectGeometryData.m_Bounds.max.z * num10;
				float2 val3 = default(float2);
				((float2)(ref val3))._002Ector(0.4f, 0.6f * math.saturate(num2 * num10));
				val3 *= laneIterator.m_CanChangeLane * num;
				val3.x = math.min(val3.x, math.max(0f, 1f - currentLane.m_ChangeProgress));
				currentLane.m_ChangeProgress = math.min(num11, currentLane.m_ChangeProgress + math.csum(val3));
				if (currentLane.m_ChangeProgress == num11 || (currentLane.m_LaneFlags & (Game.Vehicles.CarLaneFlags.Connection | Game.Vehicles.CarLaneFlags.ResetSpeed)) != 0)
				{
					ApplySideEffects(ref currentLane, speedLimitFactor, prefabRef, prefabCarData);
					currentLane.m_LanePosition = 0f - currentLane.m_LanePosition;
					currentLane.m_Lane = currentLane.m_ChangeLane;
					currentLane.m_ChangeLane = Entity.Null;
					currentLane.m_LaneFlags &= ~(Game.Vehicles.CarLaneFlags.TurnLeft | Game.Vehicles.CarLaneFlags.TurnRight);
				}
			}
			if ((currentLane.m_LaneFlags & (Game.Vehicles.CarLaneFlags.TurnLeft | Game.Vehicles.CarLaneFlags.TurnRight)) == 0)
			{
				currentLane.m_LaneFlags |= carLaneFlags;
			}
			bool num12 = blocker.m_Type == BlockerType.Temporary;
			if (num12 != flag || currentLane.m_Duration >= 30f)
			{
				ApplySideEffects(ref currentLane, speedLimitFactor, prefabRef, prefabCarData);
			}
			if (num12)
			{
				if (currentLane.m_Duration >= 5f)
				{
					currentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.PushBlockers;
				}
			}
			else if (currentLane.m_Duration >= 5f)
			{
				currentLane.m_LaneFlags &= ~Game.Vehicles.CarLaneFlags.PushBlockers;
			}
			currentLane.m_Duration += num;
			if (num2 > 0.01f)
			{
				float num13 = num2 * num;
				currentLane.m_Distance += num13;
				odometer.m_Distance += num13;
				carLaneFlags = currentLane.m_LaneFlags & (Game.Vehicles.CarLaneFlags.TurnLeft | Game.Vehicles.CarLaneFlags.TurnRight);
				float4 val4 = math.select(new float4(-0.5f, 0.5f, 0.002f, 0.1f), new float4(0f, 0f, 0.01f, 0.1f), new bool4(carLaneFlags == Game.Vehicles.CarLaneFlags.TurnRight, carLaneFlags == Game.Vehicles.CarLaneFlags.TurnLeft, carLaneFlags != (Game.Vehicles.CarLaneFlags)0u, true));
				((float4)(ref val4)).zw = math.min(float2.op_Implicit(1f), num13 * ((float4)(ref val4)).zw);
				currentLane.m_LanePosition -= (math.max(0f, currentLane.m_LanePosition - 0.5f) + math.min(0f, currentLane.m_LanePosition + 0.5f)) * val4.w;
				currentLane.m_LanePosition = math.lerp(currentLane.m_LanePosition, ((Random)(ref random)).NextFloat(val4.x, val4.y), val4.z);
			}
			if ((currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.ResetSpeed) != 0)
			{
				if (currentLane.m_Distance > 10f + num2 * 0.5f)
				{
					currentLane.m_LaneFlags &= ~Game.Vehicles.CarLaneFlags.ResetSpeed;
					currentLane.m_Distance = 0f;
					currentLane.m_Duration = 0f;
				}
				else if (currentLane.m_Duration > 60f)
				{
					blocker.m_Blocker = entity;
					blocker.m_Type = BlockerType.Spawn;
				}
			}
			if (num7 < num9)
			{
				Owner owner = default(Owner);
				Game.Net.CarLane carLane = default(Game.Net.CarLane);
				while (true)
				{
					if ((currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.ParkingSpace) != 0)
					{
						Curve curve = m_CurveData[currentLane.m_Lane];
						currentLane.m_CurvePosition.x = currentLane.m_CurvePosition.z;
						if (m_ParkingLaneData.HasComponent(currentLane.m_Lane))
						{
							Game.Net.ParkingLane parkingLane = m_ParkingLaneData[currentLane.m_Lane];
							PrefabRef prefabRef4 = m_PrefabRefData[currentLane.m_Lane];
							ParkingLaneData parkingLaneData = m_PrefabParkingLaneData[prefabRef4.m_Prefab];
							Transform ownerTransform = default(Transform);
							if (m_OwnerData.TryGetComponent(currentLane.m_Lane, ref owner) && m_TransformData.HasComponent(owner.m_Owner))
							{
								ownerTransform = m_TransformData[owner.m_Owner];
							}
							Transform transform2 = VehicleUtils.CalculateParkingSpaceTarget(parkingLane, parkingLaneData, prefabObjectGeometryData, curve, ownerTransform, currentLane.m_CurvePosition.x);
							navigation.m_TargetPosition = transform2.m_Position;
							navigation.m_TargetRotation = transform2.m_Rotation;
						}
						else
						{
							Game.Net.ConnectionLane connectionLane = m_ConnectionLaneData[currentLane.m_Lane];
							navigation.m_TargetPosition = VehicleUtils.GetConnectionParkingPosition(connectionLane, curve.m_Bezier, currentLane.m_CurvePosition.x);
							navigation.m_TargetRotation = quaternion.LookRotationSafe(MathUtils.Tangent(curve.m_Bezier, currentLane.m_CurvePosition.x), math.up());
						}
						num7 = math.distance(val2, navigation.m_TargetPosition);
						if (num7 >= 1f + num6)
						{
							navigation.m_TargetRotation = default(quaternion);
						}
					}
					else if ((currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.TransformTarget) != 0)
					{
						bool flag4 = false;
						if ((currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.ResetSpeed) != 0)
						{
							quaternion targetRotation = CalculateNavigationRotation(currentLane.m_Lane, navigationLanes);
							flag4 = !((quaternion)(ref targetRotation)).Equals(navigation.m_TargetRotation);
							navigation.m_TargetRotation = targetRotation;
						}
						else
						{
							navigation.m_TargetRotation = default(quaternion);
						}
						if (MoveTarget(val2, ref navigation.m_TargetPosition, num8, currentLane.m_Lane) || flag4)
						{
							break;
						}
					}
					else if ((currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.Area) != 0)
					{
						navigation.m_TargetRotation = default(quaternion);
						currentLane.m_LanePosition = math.clamp(currentLane.m_LanePosition, -0.5f, 0.5f);
						float navigationSize = VehicleUtils.GetNavigationSize(prefabObjectGeometryData);
						bool num14 = MoveAreaTarget(ref random, transform.m_Position, pathOwner, navigationLanes, pathElements, ref navigation.m_TargetPosition, num9, currentLane.m_Lane, ref currentLane.m_CurvePosition, currentLane.m_LanePosition, navigationSize);
						navigation.m_TargetPosition.y = TerrainUtils.SampleHeight(ref m_TerrainHeightData, navigation.m_TargetPosition);
						currentLane.m_ChangeProgress = 0f;
						if (num14)
						{
							break;
						}
					}
					else
					{
						navigation.m_TargetRotation = default(quaternion);
						if (currentLane.m_ChangeLane != Entity.Null)
						{
							Curve curve2 = m_CurveData[currentLane.m_Lane];
							Curve curve3 = m_CurveData[currentLane.m_ChangeLane];
							PrefabRef prefabRef5 = m_PrefabRefData[currentLane.m_Lane];
							PrefabRef prefabRef6 = m_PrefabRefData[currentLane.m_ChangeLane];
							NetLaneData prefabLaneData3 = m_PrefabLaneData[prefabRef5.m_Prefab];
							NetLaneData prefabLaneData4 = m_PrefabLaneData[prefabRef6.m_Prefab];
							float laneOffset3 = VehicleUtils.GetLaneOffset(prefabObjectGeometryData, prefabLaneData3, currentLane.m_LanePosition);
							float laneOffset4 = VehicleUtils.GetLaneOffset(prefabObjectGeometryData, prefabLaneData4, 0f - currentLane.m_LanePosition);
							if (MoveTarget(val2, ref navigation.m_TargetPosition, num8, curve2.m_Bezier, curve3.m_Bezier, currentLane.m_ChangeProgress, ref currentLane.m_CurvePosition, laneOffset3, laneOffset4))
							{
								if ((prefabLaneData3.m_Flags & LaneFlags.Twoway) == 0)
								{
									currentLane.m_LaneFlags &= ~Game.Vehicles.CarLaneFlags.CanReverse;
								}
								break;
							}
						}
						else
						{
							Curve curve4 = m_CurveData[currentLane.m_Lane];
							PrefabRef prefabRef7 = m_PrefabRefData[currentLane.m_Lane];
							NetLaneData prefabLaneData5 = m_PrefabLaneData[prefabRef7.m_Prefab];
							float num15 = VehicleUtils.GetLaneOffset(prefabObjectGeometryData, prefabLaneData5, currentLane.m_LanePosition);
							if (laneIterator.m_Oncoming != 0f)
							{
								float num16 = prefabObjectGeometryData.m_Bounds.max.x - prefabObjectGeometryData.m_Bounds.min.x;
								float num17 = math.lerp(num15, num16 * math.select(0.5f, -0.5f, m_LeftHandTraffic), math.min(1f, laneIterator.m_Oncoming));
								num15 = math.select(num15, num17, (!m_LeftHandTraffic && num17 > num15) | (m_LeftHandTraffic && num17 < num15));
								currentLane.m_LanePosition = num15 / math.max(0.1f, prefabLaneData5.m_Width - num16);
							}
							num15 = math.select(num15, 0f - num15, currentLane.m_CurvePosition.z < currentLane.m_CurvePosition.x);
							if (MoveTarget(val2, ref navigation.m_TargetPosition, num8, curve4.m_Bezier, ref currentLane.m_CurvePosition, num15))
							{
								if ((prefabLaneData5.m_Flags & LaneFlags.Twoway) == 0)
								{
									currentLane.m_LaneFlags &= ~Game.Vehicles.CarLaneFlags.CanReverse;
								}
								break;
							}
						}
					}
					if (navigationLanes.Length == 0)
					{
						num7 = math.distance(val2, navigation.m_TargetPosition);
						if (num7 < 1f + num6 && num2 < 0.1f)
						{
							currentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.EndReached;
						}
						break;
					}
					CarNavigationLane carNavigationLane3 = navigationLanes[0];
					if ((carNavigationLane3.m_Flags & (Game.Vehicles.CarLaneFlags.ParkingSpace | Game.Vehicles.CarLaneFlags.Validated)) == Game.Vehicles.CarLaneFlags.ParkingSpace || !((EntityStorageInfoLookup)(ref m_EntityStorageInfoLookup)).Exists(carNavigationLane3.m_Lane))
					{
						break;
					}
					if ((currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.Connection) != 0)
					{
						if ((carNavigationLane3.m_Flags & Game.Vehicles.CarLaneFlags.TransformTarget) != 0)
						{
							carNavigationLane3.m_Flags |= Game.Vehicles.CarLaneFlags.ResetSpeed;
						}
						else if ((carNavigationLane3.m_Flags & Game.Vehicles.CarLaneFlags.Connection) == 0)
						{
							num7 = math.distance(val2, navigation.m_TargetPosition);
							if (num7 >= 1f + num6 || num2 > 3f)
							{
								break;
							}
							carNavigationLane3.m_Flags |= Game.Vehicles.CarLaneFlags.ResetSpeed;
						}
					}
					if ((currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.HighBeams) != 0 && m_CarLaneData.TryGetComponent(carNavigationLane3.m_Lane, ref carLane) && (carLane.m_Flags & Game.Net.CarLaneFlags.Highway) != 0 && !IsLit(carNavigationLane3.m_Lane))
					{
						carNavigationLane3.m_Flags |= Game.Vehicles.CarLaneFlags.HighBeams;
					}
					ApplySideEffects(ref currentLane, speedLimitFactor, prefabRef, prefabCarData);
					if (currentLane.m_ChangeLane != Entity.Null && GetNextLane(currentLane.m_Lane, carNavigationLane3.m_Lane, out var selectedLane) && selectedLane != carNavigationLane3.m_Lane)
					{
						currentLane.m_Lane = selectedLane;
						currentLane.m_ChangeLane = carNavigationLane3.m_Lane;
					}
					else
					{
						currentLane.m_Lane = carNavigationLane3.m_Lane;
						currentLane.m_ChangeLane = Entity.Null;
						currentLane.m_ChangeProgress = 0f;
					}
					currentLane.m_CurvePosition = ((float2)(ref carNavigationLane3.m_CurvePosition)).xxy;
					currentLane.m_LaneFlags = carNavigationLane3.m_Flags | (currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.PushBlockers);
					navigationLanes.RemoveAt(0);
				}
			}
			if ((currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.Area) != 0)
			{
				VehicleCollisionIterator vehicleCollisionIterator = new VehicleCollisionIterator
				{
					m_OwnerData = m_OwnerData,
					m_TransformData = m_TransformData,
					m_MovingData = m_MovingData,
					m_ControllerData = m_ControllerData,
					m_CreatureData = m_CreatureData,
					m_CurveData = m_CurveData,
					m_AreaLaneData = m_AreaLaneData,
					m_PrefabRefData = m_PrefabRefData,
					m_PrefabObjectGeometryData = m_PrefabObjectGeometryData,
					m_PrefabLaneData = m_PrefabLaneData,
					m_AreaNodes = m_AreaNodes,
					m_StaticObjectSearchTree = m_StaticObjectSearchTree,
					m_MovingObjectSearchTree = m_MovingObjectSearchTree,
					m_TerrainHeightData = m_TerrainHeightData,
					m_Entity = entity,
					m_CurrentLane = currentLane.m_Lane,
					m_CurvePosition = currentLane.m_CurvePosition.z,
					m_TimeStep = num,
					m_PrefabObjectGeometry = prefabObjectGeometryData,
					m_SpeedRange = val,
					m_CurrentPosition = transform.m_Position,
					m_CurrentVelocity = moving.m_Velocity,
					m_MinDistance = num9,
					m_TargetPosition = navigation.m_TargetPosition,
					m_MaxSpeed = navigation.m_MaxSpeed,
					m_LanePosition = currentLane.m_LanePosition,
					m_Blocker = blocker.m_Blocker,
					m_BlockerType = blocker.m_Type
				};
				if (vehicleCollisionIterator.m_MaxSpeed != 0f && !flag2)
				{
					vehicleCollisionIterator.IterateFirstLane(currentLane.m_Lane);
					vehicleCollisionIterator.m_MaxSpeed = math.select(vehicleCollisionIterator.m_MaxSpeed, 0f, vehicleCollisionIterator.m_MaxSpeed < 0.1f);
					if (!((float3)(ref navigation.m_TargetPosition)).Equals(vehicleCollisionIterator.m_TargetPosition))
					{
						navigation.m_TargetPosition = vehicleCollisionIterator.m_TargetPosition;
						currentLane.m_LanePosition = math.lerp(currentLane.m_LanePosition, vehicleCollisionIterator.m_LanePosition, 0.1f);
						currentLane.m_ChangeProgress = 1f;
					}
					navigation.m_MaxSpeed = vehicleCollisionIterator.m_MaxSpeed;
					blocker.m_Blocker = vehicleCollisionIterator.m_Blocker;
					blocker.m_Type = vehicleCollisionIterator.m_BlockerType;
					blocker.m_MaxSpeed = (byte)math.clamp(Mathf.RoundToInt(vehicleCollisionIterator.m_MaxSpeed * 2.2949998f), 0, 255);
				}
				navigation.m_MaxSpeed = math.min(navigation.m_MaxSpeed, math.distance(((float3)(ref transform.m_Position)).xz, ((float3)(ref navigation.m_TargetPosition)).xz) / num);
			}
			else
			{
				navigation.m_MaxSpeed = math.min(navigation.m_MaxSpeed, math.distance(transform.m_Position, navigation.m_TargetPosition) / num);
			}
			if ((currentLane.m_LaneFlags & (Game.Vehicles.CarLaneFlags.Connection | Game.Vehicles.CarLaneFlags.ResetSpeed)) != 0)
			{
				return;
			}
			float3 val5 = navigation.m_TargetPosition - val2;
			if ((currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.Area) != 0)
			{
				((float3)(ref val5)).xz = MathUtils.ClampLength(((float3)(ref val5)).xz, num8);
				val5.y = TerrainUtils.SampleHeight(ref m_TerrainHeightData, val2 + val5) - val2.y;
			}
			num7 = math.length(val5);
			float3 val6 = math.forward(transform.m_Rotation);
			if (flag2)
			{
				if (num7 < 1f + num6 || math.dot(val6, math.normalizesafe(val5, default(float3))) < 0.8f)
				{
					navigation.m_MaxSpeed = 0f - math.min(3f, navigation.m_MaxSpeed);
				}
				else if (num2 >= 0.1f)
				{
					navigation.m_MaxSpeed = 0f - math.max(0f, math.min(navigation.m_MaxSpeed, num2 - prefabCarData.m_Braking * num));
				}
			}
			else
			{
				if (!(num7 >= 1f + num6) || (!(currentLane.m_ChangeLane == Entity.Null) && !(currentLane.m_ChangeProgress <= 0f)) || !(math.dot(val6, math.normalizesafe(val5, default(float3))) < 0.7f))
				{
					return;
				}
				if (num2 >= 0.1f)
				{
					navigation.m_MaxSpeed = math.max(0f, math.min(navigation.m_MaxSpeed, num2 - prefabCarData.m_Braking * num));
					return;
				}
				navigation.m_MaxSpeed = 0f - math.min(3f, navigation.m_MaxSpeed);
				if ((currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.Area) == 0)
				{
					currentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.CanReverse;
				}
			}
			return;
			IL_07c5:
			navigation.m_MaxSpeed = laneIterator.m_MaxSpeed;
			CheckBlocker(ref currentLane, ref blocker, ref laneIterator);
			if (laneIterator.m_TempBuffer.IsCreated)
			{
				tempBuffer = laneIterator.m_TempBuffer;
				tempBuffer.Clear();
			}
			goto IL_0802;
		}

		private quaternion CalculateNavigationRotation(Entity sourceLocation, DynamicBuffer<CarNavigationLane> navigationLanes)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			float3 val = default(float3);
			bool flag = false;
			Transform transform = default(Transform);
			if (m_TransformData.TryGetComponent(sourceLocation, ref transform))
			{
				val = transform.m_Position;
				flag = true;
			}
			Curve curve = default(Curve);
			for (int i = 0; i < navigationLanes.Length; i++)
			{
				CarNavigationLane carNavigationLane = navigationLanes[i];
				if (m_TransformData.TryGetComponent(carNavigationLane.m_Lane, ref transform))
				{
					if (flag)
					{
						float3 val2 = transform.m_Position - val;
						if (MathUtils.TryNormalize(ref val2))
						{
							return quaternion.LookRotationSafe(val2, math.up());
						}
					}
					else
					{
						val = transform.m_Position;
						flag = true;
					}
				}
				else
				{
					if (!m_CurveData.TryGetComponent(carNavigationLane.m_Lane, ref curve))
					{
						continue;
					}
					float3 val3 = MathUtils.Position(curve.m_Bezier, carNavigationLane.m_CurvePosition.x);
					if (flag)
					{
						float3 val4 = val3 - val;
						if (MathUtils.TryNormalize(ref val4))
						{
							return quaternion.LookRotationSafe(val4, math.up());
						}
					}
					else
					{
						val = val3;
						flag = true;
					}
					if (carNavigationLane.m_CurvePosition.x != carNavigationLane.m_CurvePosition.y)
					{
						float3 val5 = MathUtils.Tangent(curve.m_Bezier, carNavigationLane.m_CurvePosition.x);
						val5 = math.select(val5, -val5, carNavigationLane.m_CurvePosition.y < carNavigationLane.m_CurvePosition.x);
						if (MathUtils.TryNormalize(ref val5))
						{
							return quaternion.LookRotationSafe(val5, math.up());
						}
					}
				}
			}
			return default(quaternion);
		}

		private bool IsLit(Entity lane)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			Owner owner = default(Owner);
			Road road = default(Road);
			if (m_OwnerData.TryGetComponent(lane, ref owner) && m_RoadData.TryGetComponent(owner.m_Owner, ref road))
			{
				return (road.m_Flags & (Game.Net.RoadFlags.IsLit | Game.Net.RoadFlags.LightsOff)) == Game.Net.RoadFlags.IsLit;
			}
			return false;
		}

		private float CalculateNoise(ref CarCurrentLane currentLaneData, PrefabRef prefabRefData, CarData prefabCarData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			if (m_PrefabSideEffectData.HasComponent(prefabRefData.m_Prefab) && m_CarLaneData.HasComponent(currentLaneData.m_Lane))
			{
				VehicleSideEffectData vehicleSideEffectData = m_PrefabSideEffectData[prefabRefData.m_Prefab];
				Game.Net.CarLane carLaneData = m_CarLaneData[currentLaneData.m_Lane];
				float maxDriveSpeed = VehicleUtils.GetMaxDriveSpeed(prefabCarData, carLaneData);
				float num = math.select(currentLaneData.m_Distance / currentLaneData.m_Duration, maxDriveSpeed, currentLaneData.m_Duration == 0f) / prefabCarData.m_MaxSpeed;
				num = math.saturate(num * num);
				return math.lerp(vehicleSideEffectData.m_Min.z, vehicleSideEffectData.m_Max.z, num) * currentLaneData.m_Duration;
			}
			return 0f;
		}

		private void ApplySideEffects(ref CarCurrentLane currentLane, float speedLimitFactor, PrefabRef prefabRefData, CarData prefabCarData)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			if ((currentLane.m_LaneFlags & Game.Vehicles.CarLaneFlags.ResetSpeed) != 0)
			{
				return;
			}
			if (m_CarLaneData.HasComponent(currentLane.m_Lane) && (currentLane.m_Duration != 0f || currentLane.m_Distance != 0f))
			{
				Game.Net.CarLane carLaneData = m_CarLaneData[currentLane.m_Lane];
				Curve curve = m_CurveData[currentLane.m_Lane];
				carLaneData.m_SpeedLimit *= speedLimitFactor;
				float maxDriveSpeed = VehicleUtils.GetMaxDriveSpeed(prefabCarData, carLaneData);
				float num = 1f / math.max(1f, curve.m_Length);
				float3 sideEffects = default(float3);
				if (m_PrefabSideEffectData.HasComponent(prefabRefData.m_Prefab))
				{
					VehicleSideEffectData vehicleSideEffectData = m_PrefabSideEffectData[prefabRefData.m_Prefab];
					float num2 = math.select(currentLane.m_Distance / currentLane.m_Duration, maxDriveSpeed, currentLane.m_Duration == 0f) / prefabCarData.m_MaxSpeed;
					num2 = math.saturate(num2 * num2);
					sideEffects = math.lerp(vehicleSideEffectData.m_Min, vehicleSideEffectData.m_Max, num2);
					float num3 = math.min(1f, currentLane.m_Distance * num);
					sideEffects *= new float3(num3, currentLane.m_Duration, currentLane.m_Duration);
				}
				maxDriveSpeed = math.min(prefabCarData.m_MaxSpeed, carLaneData.m_SpeedLimit);
				float2 flow = new float2(currentLane.m_Duration * maxDriveSpeed, currentLane.m_Distance) * num;
				m_LaneEffects.Enqueue(new CarNavigationHelpers.LaneEffects(currentLane.m_Lane, sideEffects, flow));
			}
			currentLane.m_Duration = 0f;
			currentLane.m_Distance = 0f;
		}

		private bool AllowHighBeams(Transform transform, Blocker blocker, ref CarCurrentLane currentLaneData, DynamicBuffer<CarNavigationLane> navigationLanes, float maxDistance, float minOffset)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			Transform transform2 = default(Transform);
			if (blocker.m_Blocker != Entity.Null && m_TransformData.TryGetComponent(blocker.m_Blocker, ref transform2))
			{
				float3 val = transform2.m_Position - transform.m_Position;
				if (math.lengthsq(val) < maxDistance * maxDistance && math.dot(math.forward(transform.m_Rotation), val) > minOffset && m_VehicleData.HasComponent(blocker.m_Blocker))
				{
					return false;
				}
			}
			float num = maxDistance - m_CurveData[currentLaneData.m_Lane].m_Length * math.abs(currentLaneData.m_CurvePosition.z - currentLaneData.m_CurvePosition.x);
			Entity val2 = Entity.Null;
			Owner owner = default(Owner);
			if (m_OwnerData.TryGetComponent(currentLaneData.m_Lane, ref owner) && val2 != owner.m_Owner)
			{
				if (!AllowHighBeams(transform, owner.m_Owner, maxDistance, minOffset))
				{
					return false;
				}
				val2 = owner.m_Owner;
			}
			for (int i = 0; i < navigationLanes.Length; i++)
			{
				if (!(num > 0f))
				{
					break;
				}
				CarNavigationLane carNavigationLane = navigationLanes[i];
				if (!m_CarLaneData.HasComponent(carNavigationLane.m_Lane))
				{
					break;
				}
				if (m_OwnerData.TryGetComponent(carNavigationLane.m_Lane, ref owner) && val2 != owner.m_Owner)
				{
					if (!AllowHighBeams(transform, owner.m_Owner, maxDistance, minOffset))
					{
						return false;
					}
					val2 = owner.m_Owner;
				}
				num -= m_CurveData[carNavigationLane.m_Lane].m_Length * math.abs(carNavigationLane.m_CurvePosition.y - carNavigationLane.m_CurvePosition.x);
			}
			return true;
		}

		private bool AllowHighBeams(Transform transform, Entity owner, float maxDistance, float minOffset)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<Game.Net.SubLane> val = default(DynamicBuffer<Game.Net.SubLane>);
			if (m_Lanes.TryGetBuffer(owner, ref val))
			{
				float3 val2 = math.forward(transform.m_Rotation);
				maxDistance *= maxDistance;
				DynamicBuffer<LaneObject> val3 = default(DynamicBuffer<LaneObject>);
				Transform transform2 = default(Transform);
				for (int i = 0; i < val.Length; i++)
				{
					Game.Net.SubLane subLane = val[i];
					if ((subLane.m_PathMethods & (PathMethod.Road | PathMethod.Track)) == 0 || !m_LaneObjects.TryGetBuffer(subLane.m_SubLane, ref val3))
					{
						continue;
					}
					for (int j = 0; j < val3.Length; j++)
					{
						LaneObject laneObject = val3[j];
						if (m_TransformData.TryGetComponent(laneObject.m_LaneObject, ref transform2))
						{
							float3 val4 = transform2.m_Position - transform.m_Position;
							if (math.lengthsq(val4) < maxDistance && math.dot(val2, val4) > minOffset && m_VehicleData.HasComponent(laneObject.m_LaneObject))
							{
								return false;
							}
						}
					}
				}
			}
			return true;
		}

		private void ReserveNavigationLanes(ref Random random, int priority, Entity entity, CarData prefabCarData, ObjectGeometryData prefabObjectGeometryData, Car carData, ref CarNavigation navigationData, ref CarCurrentLane currentLaneData, DynamicBuffer<CarNavigationLane> navigationLanes)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0502: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_052b: Unknown result type (might be due to invalid IL or missing references)
			//IL_053e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0463: Unknown result type (might be due to invalid IL or missing references)
			//IL_046f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0430: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0571: Unknown result type (might be due to invalid IL or missing references)
			//IL_057d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0552: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_0414: Unknown result type (might be due to invalid IL or missing references)
			float timeStep = 4f / 15f;
			if (!m_CarLaneData.HasComponent(currentLaneData.m_Lane))
			{
				return;
			}
			Curve curve = m_CurveData[currentLaneData.m_Lane];
			bool flag = currentLaneData.m_CurvePosition.z < currentLaneData.m_CurvePosition.x;
			float num = math.max(0f, VehicleUtils.GetBrakingDistance(prefabCarData, math.abs(navigationData.m_MaxSpeed), timeStep) - 0.01f);
			float num2 = num;
			float num3 = num2 / math.max(1E-06f, curve.m_Length) + 1E-06f;
			currentLaneData.m_CurvePosition.y = currentLaneData.m_CurvePosition.x + math.select(num3, 0f - num3, flag);
			num2 -= curve.m_Length * math.abs(currentLaneData.m_CurvePosition.z - currentLaneData.m_CurvePosition.x);
			int i = 0;
			if ((carData.m_Flags & CarFlags.Emergency) != 0 && num > 1f)
			{
				if (currentLaneData.m_ChangeLane != Entity.Null)
				{
					ReserveOtherLanesInGroup(currentLaneData.m_ChangeLane, 102);
				}
				else
				{
					ReserveOtherLanesInGroup(currentLaneData.m_Lane, 102);
				}
			}
			if ((currentLaneData.m_LaneFlags & Game.Vehicles.CarLaneFlags.RequestSpace) != 0 && m_LaneReservationData.HasComponent(currentLaneData.m_Lane))
			{
				m_LaneReservations.Enqueue(new CarNavigationHelpers.LaneReservation(currentLaneData.m_Lane, 0f, 96));
			}
			if (navigationLanes.Length > 0)
			{
				CarNavigationLane carNavigationLane = navigationLanes[0];
				if ((carNavigationLane.m_Flags & Game.Vehicles.CarLaneFlags.RequestSpace) != 0 && m_LaneReservationData.HasComponent(carNavigationLane.m_Lane))
				{
					m_LaneReservations.Enqueue(new CarNavigationHelpers.LaneReservation(carNavigationLane.m_Lane, 0f, 96));
				}
			}
			bool2 val = ((float3)(ref currentLaneData.m_CurvePosition)).yz > ((float3)(ref currentLaneData.m_CurvePosition)).zy;
			if (flag ? val.y : val.x)
			{
				currentLaneData.m_CurvePosition.y = currentLaneData.m_CurvePosition.z;
				while (i < navigationLanes.Length && num2 > 0f)
				{
					CarNavigationLane carNavigationLane2 = navigationLanes[i];
					if (!m_CarLaneData.HasComponent(carNavigationLane2.m_Lane))
					{
						break;
					}
					curve = m_CurveData[carNavigationLane2.m_Lane];
					if (m_LaneReservationData.HasComponent(carNavigationLane2.m_Lane))
					{
						num3 = num2 / math.max(1E-06f, curve.m_Length);
						num3 = math.max(carNavigationLane2.m_CurvePosition.x, math.min(carNavigationLane2.m_CurvePosition.y, carNavigationLane2.m_CurvePosition.x + num3));
						m_LaneReservations.Enqueue(new CarNavigationHelpers.LaneReservation(carNavigationLane2.m_Lane, num3, priority));
					}
					if ((carData.m_Flags & CarFlags.Emergency) != 0)
					{
						ReserveOtherLanesInGroup(carNavigationLane2.m_Lane, 102);
						if (m_LaneSignalData.HasComponent(carNavigationLane2.m_Lane))
						{
							m_LaneSignals.Enqueue(new CarNavigationHelpers.LaneSignal(entity, carNavigationLane2.m_Lane, priority));
						}
					}
					num2 -= curve.m_Length * math.abs(carNavigationLane2.m_CurvePosition.y - carNavigationLane2.m_CurvePosition.x);
					carNavigationLane2.m_Flags |= Game.Vehicles.CarLaneFlags.Reserved;
					navigationLanes[i++] = carNavigationLane2;
				}
			}
			if ((carData.m_Flags & CarFlags.Emergency) != 0)
			{
				num2 += num;
				if (((Random)(ref random)).NextInt(4) != 0)
				{
					num2 += prefabObjectGeometryData.m_Bounds.max.z + 1f;
				}
				LaneSignal laneSignal = default(LaneSignal);
				for (; i < navigationLanes.Length; i++)
				{
					if (!(num2 > 0f))
					{
						break;
					}
					CarNavigationLane carNavigationLane3 = navigationLanes[i];
					if (m_CarLaneData.HasComponent(carNavigationLane3.m_Lane))
					{
						curve = m_CurveData[carNavigationLane3.m_Lane];
						bool flag2 = true;
						if (m_LaneSignalData.TryGetComponent(carNavigationLane3.m_Lane, ref laneSignal))
						{
							flag2 = (laneSignal.m_Flags & LaneSignalFlags.Physical) == 0 || laneSignal.m_Signal == LaneSignalType.Go;
							m_LaneSignals.Enqueue(new CarNavigationHelpers.LaneSignal(entity, carNavigationLane3.m_Lane, priority));
						}
						if (flag2 && m_LaneReservationData.HasComponent(carNavigationLane3.m_Lane))
						{
							m_LaneReservations.Enqueue(new CarNavigationHelpers.LaneReservation(carNavigationLane3.m_Lane, 0f, priority));
						}
						num2 -= curve.m_Length * math.abs(carNavigationLane3.m_CurvePosition.y - carNavigationLane3.m_CurvePosition.x);
						continue;
					}
					break;
				}
			}
			else
			{
				if ((currentLaneData.m_LaneFlags & Game.Vehicles.CarLaneFlags.Roundabout) == 0)
				{
					return;
				}
				num2 += num * 0.5f;
				if (((Random)(ref random)).NextInt(2) != 0)
				{
					num2 += prefabObjectGeometryData.m_Bounds.max.z + 1f;
				}
				for (; i < navigationLanes.Length; i++)
				{
					if (!(num2 > 0f))
					{
						break;
					}
					CarNavigationLane carNavigationLane4 = navigationLanes[i];
					if (m_CarLaneData.HasComponent(carNavigationLane4.m_Lane) && (carNavigationLane4.m_Flags & Game.Vehicles.CarLaneFlags.Roundabout) != 0)
					{
						curve = m_CurveData[carNavigationLane4.m_Lane];
						if (m_LaneReservationData.HasComponent(carNavigationLane4.m_Lane))
						{
							m_LaneReservations.Enqueue(new CarNavigationHelpers.LaneReservation(carNavigationLane4.m_Lane, 0f, priority));
						}
						num2 -= curve.m_Length * math.abs(carNavigationLane4.m_CurvePosition.y - carNavigationLane4.m_CurvePosition.x);
						continue;
					}
					break;
				}
			}
		}

		private void ReserveOtherLanesInGroup(Entity lane, int priority)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			if (!m_SlaveLaneData.HasComponent(lane))
			{
				return;
			}
			SlaveLane slaveLane = m_SlaveLaneData[lane];
			Owner owner = m_OwnerData[lane];
			DynamicBuffer<Game.Net.SubLane> val = m_Lanes[owner.m_Owner];
			int num = math.min((int)slaveLane.m_MaxIndex, val.Length - 1);
			for (int i = slaveLane.m_MinIndex; i <= num; i++)
			{
				Entity subLane = val[i].m_SubLane;
				if (subLane != lane && m_LaneReservationData.HasComponent(subLane))
				{
					m_LaneReservations.Enqueue(new CarNavigationHelpers.LaneReservation(subLane, 0f, priority));
				}
			}
		}

		private bool MoveAreaTarget(ref Random random, float3 comparePosition, PathOwner pathOwner, DynamicBuffer<CarNavigationLane> navigationLanes, DynamicBuffer<PathElement> pathElements, ref float3 targetPosition, float minDistance, Entity target, ref float3 curveDelta, float lanePosition, float navigationSize)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_043c: Unknown result type (might be due to invalid IL or missing references)
			//IL_043e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0440: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_0457: Unknown result type (might be due to invalid IL or missing references)
			//IL_045d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0463: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_0481: Unknown result type (might be due to invalid IL or missing references)
			//IL_0488: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0410: Unknown result type (might be due to invalid IL or missing references)
			//IL_0416: Unknown result type (might be due to invalid IL or missing references)
			//IL_041c: Unknown result type (might be due to invalid IL or missing references)
			if ((pathOwner.m_State & (PathFlags.Pending | PathFlags.Obsolete | PathFlags.Updated)) != 0)
			{
				return true;
			}
			Entity owner = m_OwnerData[target].m_Owner;
			AreaLane areaLane = m_AreaLaneData[target];
			DynamicBuffer<Game.Areas.Node> nodes = m_AreaNodes[owner];
			int num = math.min(pathOwner.m_ElementIndex, pathElements.Length);
			NativeArray<PathElement> subArray = pathElements.AsNativeArray().GetSubArray(num, pathElements.Length - num);
			num = 0;
			bool flag = curveDelta.z < curveDelta.x;
			float lanePosition2 = math.select(lanePosition, 0f - lanePosition, flag);
			if (areaLane.m_Nodes.y == areaLane.m_Nodes.z)
			{
				float3 position = nodes[areaLane.m_Nodes.x].m_Position;
				float3 position2 = nodes[areaLane.m_Nodes.y].m_Position;
				float3 position3 = nodes[areaLane.m_Nodes.w].m_Position;
				if (VehicleUtils.SetTriangleTarget(position, position2, position3, comparePosition, num, navigationLanes, subArray, ref targetPosition, minDistance, lanePosition2, curveDelta.z, navigationSize, isSingle: true, m_TransformData, m_AreaLaneData, m_CurveData))
				{
					return true;
				}
				curveDelta.y = curveDelta.z;
			}
			else
			{
				bool4 val = default(bool4);
				((bool4)(ref val))._002Ector(((float3)(ref curveDelta)).yz < 0.5f, ((float3)(ref curveDelta)).yz > 0.5f);
				int2 val2 = math.select(int2.op_Implicit(areaLane.m_Nodes.x), int2.op_Implicit(areaLane.m_Nodes.w), ((bool4)(ref val)).zw);
				float3 position4 = nodes[val2.x].m_Position;
				float3 position5 = nodes[areaLane.m_Nodes.y].m_Position;
				float3 position6 = nodes[areaLane.m_Nodes.z].m_Position;
				float3 position7 = nodes[val2.y].m_Position;
				if (math.any(((bool4)(ref val)).xy & ((bool4)(ref val)).wz))
				{
					if (VehicleUtils.SetAreaTarget(position4, position4, position5, position6, position7, owner, nodes, comparePosition, num, navigationLanes, subArray, ref targetPosition, minDistance, lanePosition2, curveDelta.z, navigationSize, flag, m_TransformData, m_AreaLaneData, m_CurveData, m_OwnerData))
					{
						return true;
					}
					curveDelta.y = 0.5f;
					((bool4)(ref val)).xz = bool2.op_Implicit(false);
				}
				Owner owner2 = default(Owner);
				if (VehicleUtils.GetPathElement(num, navigationLanes, subArray, out var pathElement) && m_OwnerData.TryGetComponent(pathElement.m_Target, ref owner2) && owner2.m_Owner == owner)
				{
					bool4 val3 = default(bool4);
					((bool4)(ref val3))._002Ector(pathElement.m_TargetDelta < 0.5f, pathElement.m_TargetDelta > 0.5f);
					if (math.any(!((bool4)(ref val)).xz) & math.any(((bool4)(ref val)).yw) & math.any(((bool4)(ref val3)).xy & ((bool4)(ref val3)).wz))
					{
						AreaLane areaLane2 = m_AreaLaneData[pathElement.m_Target];
						val2 = math.select(int2.op_Implicit(areaLane2.m_Nodes.x), int2.op_Implicit(areaLane2.m_Nodes.w), ((bool4)(ref val3)).zw);
						position4 = nodes[val2.x].m_Position;
						float3 prev = math.select(position5, position6, ((float3)(ref position4)).Equals(position5));
						position5 = nodes[areaLane2.m_Nodes.y].m_Position;
						position6 = nodes[areaLane2.m_Nodes.z].m_Position;
						position7 = nodes[val2.y].m_Position;
						bool flag2 = pathElement.m_TargetDelta.y < pathElement.m_TargetDelta.x;
						if (VehicleUtils.SetAreaTarget(lanePosition: math.select(lanePosition, 0f - lanePosition, flag2), prev2: prev, prev: position4, left: position5, right: position6, next: position7, areaEntity: owner, nodes: nodes, comparePosition: comparePosition, elementIndex: num + 1, navigationLanes: navigationLanes, pathElements: subArray, targetPosition: ref targetPosition, minDistance: minDistance, curveDelta: pathElement.m_TargetDelta.y, navigationSize: navigationSize, isBackward: flag2, transforms: m_TransformData, areaLanes: m_AreaLaneData, curves: m_CurveData, owners: m_OwnerData))
						{
							return true;
						}
					}
					curveDelta.y = curveDelta.z;
					return false;
				}
				if (VehicleUtils.SetTriangleTarget(position5, position6, position7, comparePosition, num, navigationLanes, subArray, ref targetPosition, minDistance, lanePosition2, curveDelta.z, navigationSize, isSingle: false, m_TransformData, m_AreaLaneData, m_CurveData))
				{
					return true;
				}
				curveDelta.y = curveDelta.z;
			}
			return math.distance(((float3)(ref comparePosition)).xz, ((float3)(ref targetPosition)).xz) >= minDistance;
		}

		private bool MoveTarget(float3 comparePosition, ref float3 targetPosition, float minDistance, Entity target)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			if (VehicleUtils.CalculateTransformPosition(ref targetPosition, target, m_TransformData, m_PositionData, m_PrefabRefData, m_PrefabBuildingData))
			{
				return math.distance(comparePosition, targetPosition) >= minDistance;
			}
			return false;
		}

		private bool MoveTarget(float3 comparePosition, ref float3 targetPosition, float minDistance, Bezier4x3 curve, ref float3 curveDelta, float laneOffset)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			float3 lanePosition = VehicleUtils.GetLanePosition(curve, curveDelta.z, laneOffset);
			if (math.distance(comparePosition, lanePosition) < minDistance)
			{
				curveDelta.x = curveDelta.z;
				targetPosition = lanePosition;
				return false;
			}
			float2 xz = ((float3)(ref curveDelta)).xz;
			for (int i = 0; i < 8; i++)
			{
				float num = math.lerp(xz.x, xz.y, 0.5f);
				float3 lanePosition2 = VehicleUtils.GetLanePosition(curve, num, laneOffset);
				if (math.distance(comparePosition, lanePosition2) < minDistance)
				{
					xz.x = num;
				}
				else
				{
					xz.y = num;
				}
			}
			curveDelta.x = xz.y;
			targetPosition = VehicleUtils.GetLanePosition(curve, xz.y, laneOffset);
			return true;
		}

		private bool MoveTarget(float3 comparePosition, ref float3 targetPosition, float minDistance, Bezier4x3 curve1, Bezier4x3 curve2, float curveSelect, ref float3 curveDelta, float laneOffset1, float laneOffset2)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			curveSelect = math.saturate(curveSelect);
			float3 lanePosition = VehicleUtils.GetLanePosition(curve1, curveDelta.z, laneOffset1);
			float3 lanePosition2 = VehicleUtils.GetLanePosition(curve2, curveDelta.z, laneOffset2);
			float num = default(float);
			if (MathUtils.Distance(new Segment(lanePosition, lanePosition2), comparePosition, ref num) < minDistance)
			{
				curveDelta.x = curveDelta.z;
				targetPosition = math.lerp(lanePosition, lanePosition2, curveSelect);
				return false;
			}
			float2 xz = ((float3)(ref curveDelta)).xz;
			for (int i = 0; i < 8; i++)
			{
				float num2 = math.lerp(xz.x, xz.y, 0.5f);
				float3 lanePosition3 = VehicleUtils.GetLanePosition(curve1, num2, laneOffset1);
				float3 lanePosition4 = VehicleUtils.GetLanePosition(curve2, num2, laneOffset2);
				if (MathUtils.Distance(new Segment(lanePosition3, lanePosition4), comparePosition, ref num) < minDistance)
				{
					xz.x = num2;
				}
				else
				{
					xz.y = num2;
				}
			}
			curveDelta.x = xz.y;
			float3 lanePosition5 = VehicleUtils.GetLanePosition(curve1, xz.y, laneOffset1);
			float3 lanePosition6 = VehicleUtils.GetLanePosition(curve2, xz.y, laneOffset2);
			targetPosition = math.lerp(lanePosition5, lanePosition6, curveSelect);
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
		public NativeQueue<CarNavigationHelpers.LaneSignal> m_LaneSignalQueue;

		public ComponentLookup<LaneSignal> m_LaneSignalData;

		public void Execute()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			CarNavigationHelpers.LaneSignal laneSignal = default(CarNavigationHelpers.LaneSignal);
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

	[BurstCompile]
	private struct UpdateLaneReservationsJob : IJob
	{
		public NativeQueue<CarNavigationHelpers.LaneReservation> m_LaneReservationQueue;

		public ComponentLookup<LaneReservation> m_LaneReservationData;

		public void Execute()
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			CarNavigationHelpers.LaneReservation laneReservation = default(CarNavigationHelpers.LaneReservation);
			while (m_LaneReservationQueue.TryDequeue(ref laneReservation))
			{
				ref LaneReservation valueRW = ref m_LaneReservationData.GetRefRW(laneReservation.m_Lane).ValueRW;
				if (laneReservation.m_Offset > valueRW.m_Next.m_Offset)
				{
					valueRW.m_Next.m_Offset = laneReservation.m_Offset;
				}
				if (laneReservation.m_Priority > valueRW.m_Next.m_Priority)
				{
					if (laneReservation.m_Priority >= valueRW.m_Prev.m_Priority)
					{
						valueRW.m_Blocker = Entity.Null;
					}
					valueRW.m_Next.m_Priority = laneReservation.m_Priority;
				}
			}
		}
	}

	public struct TrafficAmbienceEffect
	{
		public float3 m_Position;

		public float m_Amount;
	}

	[BurstCompile]
	private struct ApplyTrafficAmbienceJob : IJob
	{
		public NativeQueue<TrafficAmbienceEffect> m_EffectsQueue;

		public NativeArray<TrafficAmbienceCell> m_TrafficAmbienceMap;

		public void Execute()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			TrafficAmbienceEffect trafficAmbienceEffect = default(TrafficAmbienceEffect);
			while (m_EffectsQueue.TryDequeue(ref trafficAmbienceEffect))
			{
				int2 cell = CellMapSystem<TrafficAmbienceCell>.GetCell(trafficAmbienceEffect.m_Position, CellMapSystem<TrafficAmbienceCell>.kMapSize, TrafficAmbienceSystem.kTextureSize);
				if (cell.x >= 0 && cell.y >= 0 && cell.x < TrafficAmbienceSystem.kTextureSize && cell.y < TrafficAmbienceSystem.kTextureSize)
				{
					int num = cell.x + cell.y * TrafficAmbienceSystem.kTextureSize;
					TrafficAmbienceCell trafficAmbienceCell = m_TrafficAmbienceMap[num];
					trafficAmbienceCell.m_Accumulator += trafficAmbienceEffect.m_Amount;
					m_TrafficAmbienceMap[num] = trafficAmbienceCell;
				}
			}
		}
	}

	[BurstCompile]
	private struct ApplyLaneEffectsJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<LaneDeteriorationData> m_LaneDeteriorationData;

		public ComponentLookup<Game.Net.Pollution> m_PollutionData;

		public ComponentLookup<LaneCondition> m_LaneConditionData;

		public ComponentLookup<LaneFlow> m_LaneFlowData;

		public NativeQueue<CarNavigationHelpers.LaneEffects> m_LaneEffectsQueue;

		public void Execute()
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			CarNavigationHelpers.LaneEffects laneEffects = default(CarNavigationHelpers.LaneEffects);
			while (m_LaneEffectsQueue.TryDequeue(ref laneEffects))
			{
				Entity owner = m_OwnerData[laneEffects.m_Lane].m_Owner;
				if (m_LaneConditionData.HasComponent(laneEffects.m_Lane))
				{
					PrefabRef prefabRef = m_PrefabRefData[laneEffects.m_Lane];
					LaneDeteriorationData laneDeteriorationData = m_LaneDeteriorationData[prefabRef.m_Prefab];
					LaneCondition laneCondition = m_LaneConditionData[laneEffects.m_Lane];
					laneCondition.m_Wear = math.min(laneCondition.m_Wear + laneEffects.m_SideEffects.x * laneDeteriorationData.m_TrafficFactor, 10f);
					m_LaneConditionData[laneEffects.m_Lane] = laneCondition;
				}
				if (m_LaneFlowData.HasComponent(laneEffects.m_Lane))
				{
					LaneFlow laneFlow = m_LaneFlowData[laneEffects.m_Lane];
					ref float2 next = ref laneFlow.m_Next;
					next += laneEffects.m_Flow;
					m_LaneFlowData[laneEffects.m_Lane] = laneFlow;
				}
				if (m_PollutionData.HasComponent(owner))
				{
					Game.Net.Pollution pollution = m_PollutionData[owner];
					ref float2 pollution2 = ref pollution.m_Pollution;
					pollution2 += ((float3)(ref laneEffects.m_SideEffects)).yz;
					m_PollutionData[owner] = pollution;
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
		public ComponentTypeHandle<Target> __Game_Common_Target_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Car> __Game_Vehicles_Car_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<OutOfControl> __Game_Vehicles_OutOfControl_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PseudoRandomSeed> __Game_Common_PseudoRandomSeed_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferTypeHandle;

		public ComponentTypeHandle<CarNavigation> __Game_Vehicles_CarNavigation_RW_ComponentTypeHandle;

		public ComponentTypeHandle<CarCurrentLane> __Game_Vehicles_CarCurrentLane_RW_ComponentTypeHandle;

		public ComponentTypeHandle<PathOwner> __Game_Pathfind_PathOwner_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Blocker> __Game_Vehicles_Blocker_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Odometer> __Game_Vehicles_Odometer_RW_ComponentTypeHandle;

		public BufferTypeHandle<CarNavigationLane> __Game_Vehicles_CarNavigationLane_RW_BufferTypeHandle;

		public BufferTypeHandle<PathElement> __Game_Pathfind_PathElement_RW_BufferTypeHandle;

		[ReadOnly]
		public EntityStorageInfoLookup __EntityStorageInfoLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Unspawned> __Game_Objects_Unspawned_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Lane> __Game_Net_Lane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> __Game_Net_CarLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> __Game_Net_ParkingLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> __Game_Net_ConnectionLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MasterLane> __Game_Net_MasterLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SlaveLane> __Game_Net_SlaveLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AreaLane> __Game_Net_AreaLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LaneReservation> __Game_Net_LaneReservation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LaneCondition> __Game_Net_LaneCondition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LaneSignal> __Game_Net_LaneSignal_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Road> __Game_Net_Road_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Position> __Game_Routes_Position_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Moving> __Game_Objects_Moving_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Car> __Game_Vehicles_Car_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Train> __Game_Vehicles_Train_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Controller> __Game_Vehicles_Controller_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Vehicle> __Game_Vehicles_Vehicle_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Creature> __Game_Creatures_Creature_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarData> __Game_Prefabs_CarData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrainData> __Game_Prefabs_TrainData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<VehicleSideEffectData> __Game_Prefabs_VehicleSideEffectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneData> __Game_Prefabs_NetLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarLaneData> __Game_Prefabs_CarLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkingLaneData> __Game_Prefabs_ParkingLaneData_RO_ComponentLookup;

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

		public ComponentLookup<CarTrailerLane> __Game_Vehicles_CarTrailerLane_RW_ComponentLookup;

		public BufferLookup<BlockedLane> __Game_Objects_BlockedLane_RW_BufferLookup;

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
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Objects_Moving_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Moving>(true);
			__Game_Common_Target_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Target>(true);
			__Game_Vehicles_Car_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Car>(true);
			__Game_Vehicles_OutOfControl_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<OutOfControl>(true);
			__Game_Common_PseudoRandomSeed_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PseudoRandomSeed>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Vehicles_LayoutElement_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<LayoutElement>(true);
			__Game_Vehicles_CarNavigation_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CarNavigation>(false);
			__Game_Vehicles_CarCurrentLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CarCurrentLane>(false);
			__Game_Pathfind_PathOwner_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathOwner>(false);
			__Game_Vehicles_Blocker_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Blocker>(false);
			__Game_Vehicles_Odometer_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Odometer>(false);
			__Game_Vehicles_CarNavigationLane_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<CarNavigationLane>(false);
			__Game_Pathfind_PathElement_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<PathElement>(false);
			__EntityStorageInfoLookup = ((SystemState)(ref state)).GetEntityStorageInfoLookup();
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Objects_Unspawned_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Unspawned>(true);
			__Game_Net_Lane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(true);
			__Game_Net_CarLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.CarLane>(true);
			__Game_Net_ParkingLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ParkingLane>(true);
			__Game_Net_ConnectionLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ConnectionLane>(true);
			__Game_Net_MasterLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MasterLane>(true);
			__Game_Net_SlaveLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SlaveLane>(true);
			__Game_Net_AreaLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AreaLane>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_LaneReservation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneReservation>(true);
			__Game_Net_LaneCondition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneCondition>(true);
			__Game_Net_LaneSignal_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneSignal>(true);
			__Game_Net_Road_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Road>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Routes_Position_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Position>(true);
			__Game_Objects_Moving_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Moving>(true);
			__Game_Vehicles_Car_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Car>(true);
			__Game_Vehicles_Train_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Train>(true);
			__Game_Vehicles_Controller_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Controller>(true);
			__Game_Vehicles_Vehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Vehicle>(true);
			__Game_Creatures_Creature_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Creature>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_CarData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarData>(true);
			__Game_Prefabs_TrainData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrainData>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_VehicleSideEffectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<VehicleSideEffectData>(true);
			__Game_Prefabs_NetLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneData>(true);
			__Game_Prefabs_CarLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarLaneData>(true);
			__Game_Prefabs_ParkingLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkingLaneData>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Net_LaneObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneObject>(true);
			__Game_Net_LaneOverlap_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneOverlap>(true);
			__Game_Areas_Node_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.Node>(true);
			__Game_Areas_Triangle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Triangle>(true);
			__Game_Vehicles_CarTrailerLane_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarTrailerLane>(false);
			__Game_Objects_BlockedLane_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<BlockedLane>(false);
		}
	}

	private SimulationSystem m_SimulationSystem;

	private TerrainSystem m_TerrainSystem;

	private Game.Net.SearchSystem m_NetSearchSystem;

	private Game.Areas.SearchSystem m_AreaSearchSystem;

	private Game.Objects.SearchSystem m_ObjectSearchSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private Actions m_Actions;

	private EntityQuery m_VehicleQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		m_AreaSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Areas.SearchSystem>();
		m_ObjectSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.SearchSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_Actions = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Actions>();
		m_VehicleQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[7]
		{
			ComponentType.ReadOnly<Car>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.ReadWrite<CarCurrentLane>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<TripSource>(),
			ComponentType.Exclude<ParkedCar>()
		});
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0565: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_059f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_060e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0613: Unknown result type (might be due to invalid IL or missing references)
		//IL_062b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0630: Unknown result type (might be due to invalid IL or missing references)
		//IL_0648: Unknown result type (might be due to invalid IL or missing references)
		//IL_064d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0665: Unknown result type (might be due to invalid IL or missing references)
		//IL_066a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0682: Unknown result type (might be due to invalid IL or missing references)
		//IL_0687: Unknown result type (might be due to invalid IL or missing references)
		//IL_069f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06de: Unknown result type (might be due to invalid IL or missing references)
		//IL_071e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0723: Unknown result type (might be due to invalid IL or missing references)
		//IL_0733: Unknown result type (might be due to invalid IL or missing references)
		//IL_0738: Unknown result type (might be due to invalid IL or missing references)
		//IL_0748: Unknown result type (might be due to invalid IL or missing references)
		//IL_074d: Unknown result type (might be due to invalid IL or missing references)
		//IL_075d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0762: Unknown result type (might be due to invalid IL or missing references)
		//IL_079f: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0801: Unknown result type (might be due to invalid IL or missing references)
		//IL_0806: Unknown result type (might be due to invalid IL or missing references)
		//IL_080b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0813: Unknown result type (might be due to invalid IL or missing references)
		//IL_0820: Unknown result type (might be due to invalid IL or missing references)
		//IL_082d: Unknown result type (might be due to invalid IL or missing references)
		//IL_083a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0847: Unknown result type (might be due to invalid IL or missing references)
		//IL_0854: Unknown result type (might be due to invalid IL or missing references)
		//IL_0856: Unknown result type (might be due to invalid IL or missing references)
		//IL_085c: Unknown result type (might be due to invalid IL or missing references)
		uint index = m_SimulationSystem.frameIndex % 16;
		((EntityQuery)(ref m_VehicleQuery)).ResetFilter();
		((EntityQuery)(ref m_VehicleQuery)).SetSharedComponentFilter<UpdateFrame>(new UpdateFrame(index));
		m_Actions.m_LaneReservationQueue = new NativeQueue<CarNavigationHelpers.LaneReservation>(AllocatorHandle.op_Implicit((Allocator)3));
		m_Actions.m_LaneEffectsQueue = new NativeQueue<CarNavigationHelpers.LaneEffects>(AllocatorHandle.op_Implicit((Allocator)3));
		m_Actions.m_LaneSignalQueue = new NativeQueue<CarNavigationHelpers.LaneSignal>(AllocatorHandle.op_Implicit((Allocator)3));
		m_Actions.m_TrafficAmbienceQueue = new NativeQueue<TrafficAmbienceEffect>(AllocatorHandle.op_Implicit((Allocator)3));
		JobHandle dependencies;
		JobHandle dependencies2;
		JobHandle dependencies3;
		JobHandle dependencies4;
		JobHandle val = JobChunkExtensions.ScheduleParallel<UpdateNavigationJob>(new UpdateNavigationJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MovingType = InternalCompilerInterface.GetComponentTypeHandle<Moving>(ref __TypeHandle.__Game_Objects_Moving_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TargetType = InternalCompilerInterface.GetComponentTypeHandle<Target>(ref __TypeHandle.__Game_Common_Target_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CarType = InternalCompilerInterface.GetComponentTypeHandle<Car>(ref __TypeHandle.__Game_Vehicles_Car_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OutOfControlType = InternalCompilerInterface.GetComponentTypeHandle<OutOfControl>(ref __TypeHandle.__Game_Vehicles_OutOfControl_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PseudoRandomSeedType = InternalCompilerInterface.GetComponentTypeHandle<PseudoRandomSeed>(ref __TypeHandle.__Game_Common_PseudoRandomSeed_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LayoutElementType = InternalCompilerInterface.GetBufferTypeHandle<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NavigationType = InternalCompilerInterface.GetComponentTypeHandle<CarNavigation>(ref __TypeHandle.__Game_Vehicles_CarNavigation_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<CarCurrentLane>(ref __TypeHandle.__Game_Vehicles_CarCurrentLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathOwnerType = InternalCompilerInterface.GetComponentTypeHandle<PathOwner>(ref __TypeHandle.__Game_Pathfind_PathOwner_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BlockerType = InternalCompilerInterface.GetComponentTypeHandle<Blocker>(ref __TypeHandle.__Game_Vehicles_Blocker_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OdometerType = InternalCompilerInterface.GetComponentTypeHandle<Odometer>(ref __TypeHandle.__Game_Vehicles_Odometer_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NavigationLaneType = InternalCompilerInterface.GetBufferTypeHandle<CarNavigationLane>(ref __TypeHandle.__Game_Vehicles_CarNavigationLane_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathElementType = InternalCompilerInterface.GetBufferTypeHandle<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EntityStorageInfoLookup = InternalCompilerInterface.GetEntityStorageInfoLookup(ref __TypeHandle.__EntityStorageInfoLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UnspawnedData = InternalCompilerInterface.GetComponentLookup<Unspawned>(ref __TypeHandle.__Game_Objects_Unspawned_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkingLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ParkingLane>(ref __TypeHandle.__Game_Net_ParkingLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectionLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MasterLaneData = InternalCompilerInterface.GetComponentLookup<MasterLane>(ref __TypeHandle.__Game_Net_MasterLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SlaveLaneData = InternalCompilerInterface.GetComponentLookup<SlaveLane>(ref __TypeHandle.__Game_Net_SlaveLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaLaneData = InternalCompilerInterface.GetComponentLookup<AreaLane>(ref __TypeHandle.__Game_Net_AreaLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneReservationData = InternalCompilerInterface.GetComponentLookup<LaneReservation>(ref __TypeHandle.__Game_Net_LaneReservation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneConditionData = InternalCompilerInterface.GetComponentLookup<LaneCondition>(ref __TypeHandle.__Game_Net_LaneCondition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneSignalData = InternalCompilerInterface.GetComponentLookup<LaneSignal>(ref __TypeHandle.__Game_Net_LaneSignal_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RoadData = InternalCompilerInterface.GetComponentLookup<Road>(ref __TypeHandle.__Game_Net_Road_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyRenterData = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PositionData = InternalCompilerInterface.GetComponentLookup<Position>(ref __TypeHandle.__Game_Routes_Position_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MovingData = InternalCompilerInterface.GetComponentLookup<Moving>(ref __TypeHandle.__Game_Objects_Moving_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarData = InternalCompilerInterface.GetComponentLookup<Car>(ref __TypeHandle.__Game_Vehicles_Car_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrainData = InternalCompilerInterface.GetComponentLookup<Train>(ref __TypeHandle.__Game_Vehicles_Train_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ControllerData = InternalCompilerInterface.GetComponentLookup<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_VehicleData = InternalCompilerInterface.GetComponentLookup<Vehicle>(ref __TypeHandle.__Game_Vehicles_Vehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CreatureData = InternalCompilerInterface.GetComponentLookup<Creature>(ref __TypeHandle.__Game_Creatures_Creature_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCarData = InternalCompilerInterface.GetComponentLookup<CarData>(ref __TypeHandle.__Game_Prefabs_CarData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTrainData = InternalCompilerInterface.GetComponentLookup<TrainData>(ref __TypeHandle.__Game_Prefabs_TrainData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabBuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSideEffectData = InternalCompilerInterface.GetComponentLookup<VehicleSideEffectData>(ref __TypeHandle.__Game_Prefabs_VehicleSideEffectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabLaneData = InternalCompilerInterface.GetComponentLookup<NetLaneData>(ref __TypeHandle.__Game_Prefabs_NetLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCarLaneData = InternalCompilerInterface.GetComponentLookup<CarLaneData>(ref __TypeHandle.__Game_Prefabs_CarLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabParkingLaneData = InternalCompilerInterface.GetComponentLookup<ParkingLaneData>(ref __TypeHandle.__Game_Prefabs_ParkingLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Lanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneObjects = InternalCompilerInterface.GetBufferLookup<LaneObject>(ref __TypeHandle.__Game_Net_LaneObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneOverlaps = InternalCompilerInterface.GetBufferLookup<LaneOverlap>(ref __TypeHandle.__Game_Net_LaneOverlap_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaNodes = InternalCompilerInterface.GetBufferLookup<Game.Areas.Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaTriangles = InternalCompilerInterface.GetBufferLookup<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrailerLaneData = InternalCompilerInterface.GetComponentLookup<CarTrailerLane>(ref __TypeHandle.__Game_Vehicles_CarTrailerLane_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BlockedLanes = InternalCompilerInterface.GetBufferLookup<BlockedLane>(ref __TypeHandle.__Game_Objects_BlockedLane_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RandomSeed = RandomSeed.Next(),
			m_SimulationFrame = m_SimulationSystem.frameIndex,
			m_LeftHandTraffic = m_CityConfigurationSystem.leftHandTraffic,
			m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies),
			m_AreaSearchTree = m_AreaSearchSystem.GetSearchTree(readOnly: true, out dependencies2),
			m_StaticObjectSearchTree = m_ObjectSearchSystem.GetStaticSearchTree(readOnly: true, out dependencies3),
			m_MovingObjectSearchTree = m_ObjectSearchSystem.GetMovingSearchTree(readOnly: true, out dependencies4),
			m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
			m_LaneObjectBuffer = m_Actions.m_LaneObjectUpdater.Begin((Allocator)3),
			m_LaneReservations = m_Actions.m_LaneReservationQueue.AsParallelWriter(),
			m_LaneEffects = m_Actions.m_LaneEffectsQueue.AsParallelWriter(),
			m_LaneSignals = m_Actions.m_LaneSignalQueue.AsParallelWriter(),
			m_TrafficAmbienceEffects = m_Actions.m_TrafficAmbienceQueue.AsParallelWriter()
		}, m_VehicleQuery, JobUtils.CombineDependencies(((SystemBase)this).Dependency, dependencies, dependencies2, dependencies3, dependencies4));
		m_NetSearchSystem.AddNetSearchTreeReader(val);
		m_AreaSearchSystem.AddSearchTreeReader(val);
		m_ObjectSearchSystem.AddStaticSearchTreeReader(val);
		m_ObjectSearchSystem.AddMovingSearchTreeReader(val);
		m_TerrainSystem.AddCPUHeightReader(val);
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
	public CarNavigationSystem()
	{
	}
}
