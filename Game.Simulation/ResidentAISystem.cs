using System.Runtime.CompilerServices;
using System.Threading;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Agents;
using Game.Areas;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Creatures;
using Game.Debug;
using Game.Economy;
using Game.Events;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Reflection;
using Game.Rendering;
using Game.Routes;
using Game.Tools;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class ResidentAISystem : GameSystemBase
{
	[CompilerGenerated]
	public class Actions : GameSystemBase
	{
		private struct TypeHandle
		{
			[ReadOnly]
			public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

			[ReadOnly]
			public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

			[ReadOnly]
			public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

			[ReadOnly]
			public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

			[ReadOnly]
			public ComponentLookup<TaxiData> __Game_Prefabs_TaxiData_RO_ComponentLookup;

			[ReadOnly]
			public ComponentLookup<PublicTransportVehicleData> __Game_Prefabs_PublicTransportVehicleData_RO_ComponentLookup;

			[ReadOnly]
			public ComponentLookup<PersonalCarData> __Game_Prefabs_PersonalCarData_RO_ComponentLookup;

			[ReadOnly]
			public BufferLookup<GroupCreature> __Game_Creatures_GroupCreature_RO_BufferLookup;

			[ReadOnly]
			public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferLookup;

			[ReadOnly]
			public BufferLookup<ActivityLocationElement> __Game_Prefabs_ActivityLocationElement_RO_BufferLookup;

			public ComponentLookup<Game.Creatures.Resident> __Game_Creatures_Resident_RW_ComponentLookup;

			public ComponentLookup<Creature> __Game_Creatures_Creature_RW_ComponentLookup;

			public ComponentLookup<Game.Vehicles.Taxi> __Game_Vehicles_Taxi_RW_ComponentLookup;

			public ComponentLookup<Game.Vehicles.PublicTransport> __Game_Vehicles_PublicTransport_RW_ComponentLookup;

			public ComponentLookup<WaitingPassengers> __Game_Routes_WaitingPassengers_RW_ComponentLookup;

			public BufferLookup<Queue> __Game_Creatures_Queue_RW_BufferLookup;

			public BufferLookup<Passenger> __Game_Vehicles_Passenger_RW_BufferLookup;

			public BufferLookup<LaneObject> __Game_Net_LaneObject_RW_BufferLookup;

			public BufferLookup<Resources> __Game_Economy_Resources_RW_BufferLookup;

			public ComponentLookup<PlayerMoney> __Game_City_PlayerMoney_RW_ComponentLookup;

			[ReadOnly]
			public ComponentLookup<MailBoxData> __Game_Prefabs_MailBoxData_RO_ComponentLookup;

			public ComponentLookup<HouseholdNeed> __Game_Citizens_HouseholdNeed_RW_ComponentLookup;

			public ComponentLookup<Game.Routes.MailBox> __Game_Routes_MailBox_RW_ComponentLookup;

			public ComponentLookup<MailSender> __Game_Citizens_MailSender_RW_ComponentLookup;

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
				//IL_0085: Unknown result type (might be due to invalid IL or missing references)
				//IL_008a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0092: Unknown result type (might be due to invalid IL or missing references)
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				//IL_009f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00be: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_0107: Unknown result type (might be due to invalid IL or missing references)
				//IL_010c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0114: Unknown result type (might be due to invalid IL or missing references)
				//IL_0119: Unknown result type (might be due to invalid IL or missing references)
				//IL_0121: Unknown result type (might be due to invalid IL or missing references)
				//IL_0126: Unknown result type (might be due to invalid IL or missing references)
				//IL_012e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0133: Unknown result type (might be due to invalid IL or missing references)
				__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
				__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
				__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
				__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
				__Game_Prefabs_TaxiData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TaxiData>(true);
				__Game_Prefabs_PublicTransportVehicleData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PublicTransportVehicleData>(true);
				__Game_Prefabs_PersonalCarData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PersonalCarData>(true);
				__Game_Creatures_GroupCreature_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<GroupCreature>(true);
				__Game_Vehicles_LayoutElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(true);
				__Game_Prefabs_ActivityLocationElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ActivityLocationElement>(true);
				__Game_Creatures_Resident_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Creatures.Resident>(false);
				__Game_Creatures_Creature_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Creature>(false);
				__Game_Vehicles_Taxi_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.Taxi>(false);
				__Game_Vehicles_PublicTransport_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.PublicTransport>(false);
				__Game_Routes_WaitingPassengers_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaitingPassengers>(false);
				__Game_Creatures_Queue_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Queue>(false);
				__Game_Vehicles_Passenger_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Passenger>(false);
				__Game_Net_LaneObject_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneObject>(false);
				__Game_Economy_Resources_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Resources>(false);
				__Game_City_PlayerMoney_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlayerMoney>(false);
				__Game_Prefabs_MailBoxData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MailBoxData>(true);
				__Game_Citizens_HouseholdNeed_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HouseholdNeed>(false);
				__Game_Routes_MailBox_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Routes.MailBox>(false);
				__Game_Citizens_MailSender_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MailSender>(false);
			}
		}

		private EndFrameBarrier m_EndFrameBarrier;

		private Game.Objects.SearchSystem m_ObjectSearchSystem;

		private CityStatisticsSystem m_CityStatisticsSystem;

		private CitySystem m_CitySystem;

		private ServiceFeeSystem m_ServiceFeeSystem;

		private ComponentTypeSet m_CurrentLaneTypes;

		private ComponentTypeSet m_CurrentLaneTypesRelative;

		public NativeQueue<Boarding> m_BoardingQueue;

		public NativeQueue<ResidentAction> m_ActionQueue;

		public JobHandle m_Dependency;

		private TypeHandle __TypeHandle;

		[Preserve]
		protected override void OnCreate()
		{
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			base.OnCreate();
			m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
			m_ObjectSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.SearchSystem>();
			m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
			m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
			m_ServiceFeeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ServiceFeeSystem>();
			m_CurrentLaneTypes = new ComponentTypeSet((ComponentType[])(object)new ComponentType[6]
			{
				ComponentType.ReadWrite<Moving>(),
				ComponentType.ReadWrite<TransformFrame>(),
				ComponentType.ReadWrite<InterpolatedTransform>(),
				ComponentType.ReadWrite<HumanNavigation>(),
				ComponentType.ReadWrite<HumanCurrentLane>(),
				ComponentType.ReadWrite<Blocker>()
			});
			m_CurrentLaneTypesRelative = new ComponentTypeSet((ComponentType[])(object)new ComponentType[5]
			{
				ComponentType.ReadWrite<Moving>(),
				ComponentType.ReadWrite<TransformFrame>(),
				ComponentType.ReadWrite<HumanNavigation>(),
				ComponentType.ReadWrite<HumanCurrentLane>(),
				ComponentType.ReadWrite<Blocker>()
			});
		}

		[Preserve]
		protected override void OnUpdate()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
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
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_03da: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0402: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_041c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0424: Unknown result type (might be due to invalid IL or missing references)
			//IL_0426: Unknown result type (might be due to invalid IL or missing references)
			//IL_0428: Unknown result type (might be due to invalid IL or missing references)
			JobHandle val = JobHandle.CombineDependencies(((SystemBase)this).Dependency, m_Dependency);
			JobHandle dependencies;
			JobHandle deps;
			JobHandle deps2;
			BoardingJob boardingJob = new BoardingJob
			{
				m_Citizens = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Transforms = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TaxiData = InternalCompilerInterface.GetComponentLookup<TaxiData>(ref __TypeHandle.__Game_Prefabs_TaxiData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PublicTransportVehicleData = InternalCompilerInterface.GetComponentLookup<PublicTransportVehicleData>(ref __TypeHandle.__Game_Prefabs_PublicTransportVehicleData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabPersonalCarData = InternalCompilerInterface.GetComponentLookup<PersonalCarData>(ref __TypeHandle.__Game_Prefabs_PersonalCarData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_GroupCreatures = InternalCompilerInterface.GetBufferLookup<GroupCreature>(ref __TypeHandle.__Game_Creatures_GroupCreature_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_VehicleLayouts = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ActivityLocations = InternalCompilerInterface.GetBufferLookup<ActivityLocationElement>(ref __TypeHandle.__Game_Prefabs_ActivityLocationElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Residents = InternalCompilerInterface.GetComponentLookup<Game.Creatures.Resident>(ref __TypeHandle.__Game_Creatures_Resident_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Creatures = InternalCompilerInterface.GetComponentLookup<Creature>(ref __TypeHandle.__Game_Creatures_Creature_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Taxis = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.Taxi>(ref __TypeHandle.__Game_Vehicles_Taxi_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PublicTransports = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.PublicTransport>(ref __TypeHandle.__Game_Vehicles_PublicTransport_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_WaitingPassengers = InternalCompilerInterface.GetComponentLookup<WaitingPassengers>(ref __TypeHandle.__Game_Routes_WaitingPassengers_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Queues = InternalCompilerInterface.GetBufferLookup<Queue>(ref __TypeHandle.__Game_Creatures_Queue_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Passengers = InternalCompilerInterface.GetBufferLookup<Passenger>(ref __TypeHandle.__Game_Vehicles_Passenger_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LaneObjects = InternalCompilerInterface.GetBufferLookup<LaneObject>(ref __TypeHandle.__Game_Net_LaneObject_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Resources = InternalCompilerInterface.GetBufferLookup<Resources>(ref __TypeHandle.__Game_Economy_Resources_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PlayerMoney = InternalCompilerInterface.GetComponentLookup<PlayerMoney>(ref __TypeHandle.__Game_City_PlayerMoney_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_City = m_CitySystem.City,
				m_CurrentLaneTypes = m_CurrentLaneTypes,
				m_CurrentLaneTypesRelative = m_CurrentLaneTypesRelative,
				m_BoardingQueue = m_BoardingQueue,
				m_SearchTree = m_ObjectSearchSystem.GetMovingSearchTree(readOnly: false, out dependencies),
				m_CommandBuffer = m_EndFrameBarrier.CreateCommandBuffer(),
				m_StatisticsEventQueue = m_CityStatisticsSystem.GetStatisticsEventQueue(out deps).AsParallelWriter(),
				m_FeeQueue = m_ServiceFeeSystem.GetFeeQueue(out deps2)
			};
			ResidentActionJob obj = new ResidentActionJob
			{
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabMailBoxData = InternalCompilerInterface.GetComponentLookup<MailBoxData>(ref __TypeHandle.__Game_Prefabs_MailBoxData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HouseholdNeedData = InternalCompilerInterface.GetComponentLookup<HouseholdNeed>(ref __TypeHandle.__Game_Citizens_HouseholdNeed_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_MailBoxData = InternalCompilerInterface.GetComponentLookup<Game.Routes.MailBox>(ref __TypeHandle.__Game_Routes_MailBox_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_MailSenderData = InternalCompilerInterface.GetComponentLookup<MailSender>(ref __TypeHandle.__Game_Citizens_MailSender_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ActionQueue = m_ActionQueue,
				m_CommandBuffer = m_EndFrameBarrier.CreateCommandBuffer()
			};
			JobHandle val2 = IJobExtensions.Schedule<BoardingJob>(boardingJob, JobUtils.CombineDependencies(val, dependencies, deps, deps2));
			JobHandle val3 = IJobExtensions.Schedule<ResidentActionJob>(obj, val);
			m_BoardingQueue.Dispose(val2);
			m_ActionQueue.Dispose(val3);
			m_CityStatisticsSystem.AddWriter(val2);
			m_ObjectSearchSystem.AddMovingSearchTreeWriter(val2);
			m_EndFrameBarrier.AddJobHandleForProducer(val2);
			m_ServiceFeeSystem.AddQueueWriter(val2);
			m_EndFrameBarrier.AddJobHandleForProducer(val3);
			((SystemBase)this).Dependency = JobHandle.CombineDependencies(val2, val3);
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

	public struct Boarding
	{
		public Entity m_Passenger;

		public Entity m_Leader;

		public Entity m_Household;

		public Entity m_Vehicle;

		public Entity m_LeaderVehicle;

		public Entity m_Waypoint;

		public HumanCurrentLane m_CurrentLane;

		public CreatureVehicleFlags m_Flags;

		public float3 m_Position;

		public quaternion m_Rotation;

		public int m_TicketPrice;

		public BoardingType m_Type;

		public static Boarding ExitVehicle(Entity passenger, Entity household, Entity vehicle, HumanCurrentLane newCurrentLane, float3 position, quaternion rotation, int ticketPrice)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			return new Boarding
			{
				m_Passenger = passenger,
				m_Household = household,
				m_Vehicle = vehicle,
				m_CurrentLane = newCurrentLane,
				m_Position = position,
				m_Rotation = rotation,
				m_TicketPrice = ticketPrice,
				m_Type = BoardingType.Exit
			};
		}

		public static Boarding TryEnterVehicle(Entity passenger, Entity leader, Entity vehicle, Entity leaderVehicle, Entity waypoint, float3 position, CreatureVehicleFlags flags)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			return new Boarding
			{
				m_Passenger = passenger,
				m_Leader = leader,
				m_Vehicle = vehicle,
				m_LeaderVehicle = leaderVehicle,
				m_Waypoint = waypoint,
				m_Position = position,
				m_Flags = flags,
				m_Type = BoardingType.TryEnter
			};
		}

		public static Boarding FinishEnterVehicle(Entity passenger, Entity household, Entity vehicle, Entity controllerVehicle, HumanCurrentLane oldCurrentLane, int ticketPrice)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			return new Boarding
			{
				m_Passenger = passenger,
				m_Household = household,
				m_Vehicle = vehicle,
				m_LeaderVehicle = controllerVehicle,
				m_CurrentLane = oldCurrentLane,
				m_TicketPrice = ticketPrice,
				m_Type = BoardingType.FinishEnter
			};
		}

		public static Boarding CancelEnterVehicle(Entity passenger, Entity vehicle)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			return new Boarding
			{
				m_Passenger = passenger,
				m_Vehicle = vehicle,
				m_Type = BoardingType.CancelEnter
			};
		}

		public static Boarding RequireStop(Entity passenger, Entity vehicle, float3 position)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			return new Boarding
			{
				m_Passenger = passenger,
				m_Vehicle = vehicle,
				m_Position = position,
				m_Type = BoardingType.RequireStop
			};
		}

		public static Boarding WaitTimeExceeded(Entity passenger, Entity waypoint)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			return new Boarding
			{
				m_Passenger = passenger,
				m_Waypoint = waypoint,
				m_Type = BoardingType.WaitTimeExceeded
			};
		}

		public static Boarding WaitTimeEstimate(Entity waypoint, int seconds)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return new Boarding
			{
				m_Waypoint = waypoint,
				m_TicketPrice = seconds,
				m_Type = BoardingType.WaitTimeEstimate
			};
		}

		public static Boarding FinishExitVehicle(Entity passenger, Entity vehicle)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			return new Boarding
			{
				m_Passenger = passenger,
				m_Vehicle = vehicle,
				m_Type = BoardingType.FinishExit
			};
		}
	}

	public struct ResidentAction
	{
		public Entity m_Citizen;

		public Entity m_Target;

		public Entity m_Household;

		public Resource m_Resource;

		public ResidentActionType m_Type;

		public int m_Amount;

		public float m_Distance;
	}

	public enum BoardingType
	{
		Exit,
		TryEnter,
		FinishEnter,
		CancelEnter,
		RequireStop,
		WaitTimeExceeded,
		WaitTimeEstimate,
		FinishExit
	}

	public enum ResidentActionType
	{
		SendMail,
		GoShopping
	}

	private enum DeletedResidentType
	{
		StuckLoop,
		NoPathToHome,
		NoPathToHome_AlreadyOutside,
		WaitingHome_AlreadyOutside,
		NoPath_AlreadyMovingAway,
		InvalidVehicleTarget,
		Dead,
		Count
	}

	[BurstCompile]
	private struct ResidentTickJob : IJobChunk
	{
		private struct TransportEstimateBuffer : RouteUtils.ITransportEstimateBuffer
		{
			public ParallelWriter<Boarding> m_BoardingQueue;

			public void AddWaitEstimate(Entity waypoint, int seconds)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				m_BoardingQueue.Enqueue(Boarding.WaitTimeEstimate(waypoint, seconds));
			}
		}

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentVehicle> m_CurrentVehicleType;

		[ReadOnly]
		public ComponentTypeHandle<GroupMember> m_GroupMemberType;

		[ReadOnly]
		public ComponentTypeHandle<Unspawned> m_UnspawnedType;

		[ReadOnly]
		public ComponentTypeHandle<HumanNavigation> m_HumanNavigationType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<GroupCreature> m_GroupCreatureType;

		public ComponentTypeHandle<Game.Creatures.Resident> m_ResidentType;

		public ComponentTypeHandle<Creature> m_CreatureType;

		[NativeDisableContainerSafetyRestriction]
		public ComponentTypeHandle<Human> m_HumanType;

		public ComponentTypeHandle<HumanCurrentLane> m_CurrentLaneType;

		public ComponentTypeHandle<Target> m_TargetType;

		public ComponentTypeHandle<Divert> m_DivertType;

		[ReadOnly]
		public EntityStorageInfoLookup m_EntityLookup;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> m_CurrentVehicleData;

		[ReadOnly]
		public ComponentLookup<Destroyed> m_DestroyedData;

		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		[ReadOnly]
		public ComponentLookup<Unspawned> m_UnspawnedData;

		[ReadOnly]
		public ComponentLookup<RideNeeder> m_RideNeederData;

		[ReadOnly]
		public ComponentLookup<Dispatched> m_Dispatched;

		[ReadOnly]
		public ComponentLookup<ServiceRequest> m_ServiceRequestData;

		[ReadOnly]
		public ComponentLookup<Moving> m_MovingData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> m_SpawnLocation;

		[ReadOnly]
		public ComponentLookup<Animal> m_AnimalData;

		[ReadOnly]
		public ComponentLookup<OnFire> m_OnFireData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<EdgeLane> m_EdgeLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> m_ParkingLaneData;

		[ReadOnly]
		public ComponentLookup<GarageLane> m_GarageLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.PedestrianLane> m_PedestrianLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> m_ConnectionLaneData;

		[ReadOnly]
		public ComponentLookup<HangaroundLocation> m_HangaroundLocationData;

		[ReadOnly]
		public ComponentLookup<Citizen> m_CitizenData;

		[ReadOnly]
		public ComponentLookup<HouseholdMember> m_HouseholdMembers;

		[ReadOnly]
		public ComponentLookup<Household> m_HouseholdData;

		[ReadOnly]
		public ComponentLookup<CurrentBuilding> m_CurrentBuildingData;

		[ReadOnly]
		public ComponentLookup<CurrentTransport> m_CurrentTransportData;

		[ReadOnly]
		public ComponentLookup<Worker> m_WorkerData;

		[ReadOnly]
		public ComponentLookup<CarKeeper> m_CarKeeperData;

		[ReadOnly]
		public ComponentLookup<HealthProblem> m_HealthProblemData;

		[ReadOnly]
		public ComponentLookup<TravelPurpose> m_TravelPurposeData;

		[ReadOnly]
		public ComponentLookup<TouristHousehold> m_TouristHouseholds;

		[ReadOnly]
		public ComponentLookup<HomelessHousehold> m_HomelessHouseholdData;

		[ReadOnly]
		public ComponentLookup<HouseholdNeed> m_HouseholdNeedData;

		[ReadOnly]
		public ComponentLookup<AttendingMeeting> m_AttendingMeetingData;

		[ReadOnly]
		public ComponentLookup<CoordinatedMeeting> m_CoordinatedMeetingData;

		[ReadOnly]
		public ComponentLookup<MovingAway> m_MovingAwayData;

		[ReadOnly]
		public ComponentLookup<ServiceAvailable> m_ServiceAvailableData;

		[ReadOnly]
		public ComponentLookup<ParkedCar> m_ParkedCarData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PersonalCar> m_PersonalCarData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.Taxi> m_TaxiData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PublicTransport> m_PublicTransportData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PoliceCar> m_PoliceCarData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.Ambulance> m_AmbulanceData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.Hearse> m_HearseData;

		[ReadOnly]
		public ComponentLookup<Controller> m_ControllerData;

		[ReadOnly]
		public ComponentLookup<Vehicle> m_VehicleData;

		[ReadOnly]
		public ComponentLookup<Train> m_TrainData;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenters;

		[ReadOnly]
		public ComponentLookup<AttractivenessProvider> m_AttractivenessProviderData;

		[ReadOnly]
		public ComponentLookup<Connected> m_RouteConnectedData;

		[ReadOnly]
		public ComponentLookup<BoardingVehicle> m_BoardingVehicleData;

		[ReadOnly]
		public ComponentLookup<CurrentRoute> m_CurrentRouteData;

		[ReadOnly]
		public ComponentLookup<TransportLine> m_TransportLineData;

		[ReadOnly]
		public ComponentLookup<AccessLane> m_AccessLaneLaneData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<CreatureData> m_PrefabCreatureData;

		[ReadOnly]
		public ComponentLookup<HumanData> m_PrefabHumanData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<CarData> m_PrefabCarData;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> m_PrefabIndustrialProcessData;

		[ReadOnly]
		public ComponentLookup<TransportStopData> m_PrefabTransportStopData;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> m_PrefabSpawnLocationData;

		[ReadOnly]
		public BufferLookup<HouseholdAnimal> m_HouseholdAnimals;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> m_HouseholdCitizens;

		[ReadOnly]
		public BufferLookup<ConnectedRoute> m_ConnectedRoutes;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_VehicleLayouts;

		[ReadOnly]
		public BufferLookup<CarNavigationLane> m_CarNavigationLanes;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> m_AreaNodes;

		[ReadOnly]
		public BufferLookup<Triangle> m_AreaTriangles;

		[ReadOnly]
		public BufferLookup<ConnectedBuilding> m_ConnectedBuildings;

		[ReadOnly]
		public BufferLookup<Renter> m_Renters;

		[ReadOnly]
		public BufferLookup<SpawnLocationElement> m_SpawnLocationElements;

		[ReadOnly]
		public BufferLookup<Resources> m_Resources;

		[ReadOnly]
		public BufferLookup<ServiceDispatch> m_ServiceDispatches;

		[ReadOnly]
		public BufferLookup<ActivityLocationElement> m_PrefabActivityLocationElements;

		[NativeDisableContainerSafetyRestriction]
		public ComponentLookup<Human> m_HumanData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<PathOwner> m_PathOwnerData;

		[NativeDisableParallelForRestriction]
		public BufferLookup<PathElement> m_PathElements;

		[ReadOnly]
		public float m_TimeOfDay;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public uint m_SimulationFrameIndex;

		[ReadOnly]
		public bool m_LefthandTraffic;

		[ReadOnly]
		public bool m_GroupMember;

		[ReadOnly]
		public PersonalCarSelectData m_PersonalCarSelectData;

		[ReadOnly]
		public EntityArchetype m_ResetTripArchetype;

		[ReadOnly]
		public ComponentTypeSet m_ParkedToMovingCarRemoveTypes;

		[ReadOnly]
		public ComponentTypeSet m_ParkedToMovingCarAddTypes;

		[ReadOnly]
		public ComponentTypeSet m_ParkedToMovingTrailerAddTypes;

		[ReadOnly]
		public NativeArray<int> m_DeletedResidents;

		public ParallelWriter<SetupQueueItem> m_PathfindQueue;

		public ParallelWriter<Boarding> m_BoardingQueue;

		public ParallelWriter<ResidentAction> m_ActionQueue;

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
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_049d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0510: Unknown result type (might be due to invalid IL or missing references)
			//IL_051a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0523: Unknown result type (might be due to invalid IL or missing references)
			//IL_0531: Unknown result type (might be due to invalid IL or missing references)
			//IL_0547: Unknown result type (might be due to invalid IL or missing references)
			//IL_0562: Unknown result type (might be due to invalid IL or missing references)
			//IL_0597: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0406: Unknown result type (might be due to invalid IL or missing references)
			//IL_0421: Unknown result type (might be due to invalid IL or missing references)
			//IL_0456: Unknown result type (might be due to invalid IL or missing references)
			//IL_0462: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<Creature> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Creature>(ref m_CreatureType);
			NativeArray<Game.Creatures.Resident> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Creatures.Resident>(ref m_ResidentType);
			NativeArray<Target> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Target>(ref m_TargetType);
			NativeArray<CurrentVehicle> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentVehicle>(ref m_CurrentVehicleType);
			NativeArray<HumanCurrentLane> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HumanCurrentLane>(ref m_CurrentLaneType);
			NativeArray<HumanNavigation> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HumanNavigation>(ref m_HumanNavigationType);
			NativeArray<Divert> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Divert>(ref m_DivertType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			if (m_GroupMember)
			{
				NativeArray<GroupMember> nativeArray10 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<GroupMember>(ref m_GroupMemberType);
				RefRW<Human> refRW;
				if (nativeArray6.Length != 0)
				{
					HumanCurrentLane currentLane = default(HumanCurrentLane);
					HumanNavigation navigation = default(HumanNavigation);
					Divert divert = default(Divert);
					for (int i = 0; i < nativeArray.Length; i++)
					{
						Entity val = nativeArray[i];
						PrefabRef prefabRef = nativeArray2[i];
						Game.Creatures.Resident resident = nativeArray4[i];
						Creature creature = nativeArray3[i];
						CurrentVehicle currentVehicle = nativeArray6[i];
						Target target = nativeArray5[i];
						GroupMember groupMember = nativeArray10[i];
						CollectionUtils.TryGet<HumanCurrentLane>(nativeArray7, i, ref currentLane);
						CollectionUtils.TryGet<HumanNavigation>(nativeArray8, i, ref navigation);
						CollectionUtils.TryGet<Divert>(nativeArray9, i, ref divert);
						PathOwner pathOwner = m_PathOwnerData[val];
						refRW = m_HumanData.GetRefRW(val);
						ref Human valueRW = ref refRW.ValueRW;
						TickGroupMemberInVehicle(unfilteredChunkIndex, ref random, val, prefabRef, navigation, groupMember, currentVehicle, nativeArray7.Length != 0, ref resident, ref valueRW, ref currentLane, ref pathOwner, ref target, ref divert);
						TickQueue(ref random, ref resident, ref creature, ref currentLane);
						m_PathOwnerData[val] = pathOwner;
						nativeArray4[i] = resident;
						nativeArray3[i] = creature;
						nativeArray5[i] = target;
						CollectionUtils.TrySet<HumanCurrentLane>(nativeArray7, i, currentLane);
						CollectionUtils.TrySet<Divert>(nativeArray9, i, divert);
					}
					return;
				}
				bool isUnspawned = ((ArchetypeChunk)(ref chunk)).Has<Unspawned>(ref m_UnspawnedType);
				HumanCurrentLane currentLane2 = default(HumanCurrentLane);
				Divert divert2 = default(Divert);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Entity val2 = nativeArray[j];
					PrefabRef prefabRef2 = nativeArray2[j];
					Game.Creatures.Resident resident2 = nativeArray4[j];
					Creature creature2 = nativeArray3[j];
					HumanNavigation navigation2 = nativeArray8[j];
					Target target2 = nativeArray5[j];
					GroupMember groupMember2 = nativeArray10[j];
					CollectionUtils.TryGet<HumanCurrentLane>(nativeArray7, j, ref currentLane2);
					CollectionUtils.TryGet<Divert>(nativeArray9, j, ref divert2);
					PathOwner pathOwner2 = m_PathOwnerData[val2];
					refRW = m_HumanData.GetRefRW(val2);
					ref Human valueRW2 = ref refRW.ValueRW;
					CreatureUtils.CheckUnspawned(unfilteredChunkIndex, val2, currentLane2, valueRW2, isUnspawned, m_CommandBuffer);
					TickGroupMemberWalking(unfilteredChunkIndex, ref random, val2, prefabRef2, navigation2, groupMember2, ref resident2, ref creature2, ref valueRW2, ref currentLane2, ref pathOwner2, ref target2, ref divert2);
					TickQueue(ref random, ref resident2, ref creature2, ref currentLane2);
					m_PathOwnerData[val2] = pathOwner2;
					nativeArray4[j] = resident2;
					nativeArray3[j] = creature2;
					nativeArray5[j] = target2;
					CollectionUtils.TrySet<HumanCurrentLane>(nativeArray7, j, currentLane2);
					CollectionUtils.TrySet<Divert>(nativeArray9, j, divert2);
				}
				return;
			}
			NativeArray<Human> nativeArray11 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Human>(ref m_HumanType);
			BufferAccessor<GroupCreature> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<GroupCreature>(ref m_GroupCreatureType);
			if (nativeArray6.Length != 0)
			{
				HumanCurrentLane currentLane3 = default(HumanCurrentLane);
				HumanNavigation navigation3 = default(HumanNavigation);
				Divert divert3 = default(Divert);
				DynamicBuffer<GroupCreature> groupCreatures = default(DynamicBuffer<GroupCreature>);
				for (int k = 0; k < nativeArray.Length; k++)
				{
					Entity val3 = nativeArray[k];
					PrefabRef prefabRef3 = nativeArray2[k];
					Game.Creatures.Resident resident3 = nativeArray4[k];
					Creature creature3 = nativeArray3[k];
					Human human = nativeArray11[k];
					CurrentVehicle currentVehicle2 = nativeArray6[k];
					Target target3 = nativeArray5[k];
					CollectionUtils.TryGet<HumanCurrentLane>(nativeArray7, k, ref currentLane3);
					CollectionUtils.TryGet<HumanNavigation>(nativeArray8, k, ref navigation3);
					CollectionUtils.TryGet<Divert>(nativeArray9, k, ref divert3);
					CollectionUtils.TryGet<GroupCreature>(bufferAccessor, k, ref groupCreatures);
					PathOwner pathOwner3 = m_PathOwnerData[val3];
					TickInVehicle(unfilteredChunkIndex, ref random, val3, prefabRef3, navigation3, currentVehicle2, nativeArray7.Length != 0, ref resident3, ref creature3, ref human, ref currentLane3, ref pathOwner3, ref target3, ref divert3, groupCreatures);
					TickQueue(ref random, ref resident3, ref creature3, ref currentLane3);
					m_PathOwnerData[val3] = pathOwner3;
					nativeArray4[k] = resident3;
					nativeArray3[k] = creature3;
					nativeArray11[k] = human;
					nativeArray5[k] = target3;
					CollectionUtils.TrySet<HumanCurrentLane>(nativeArray7, k, currentLane3);
					CollectionUtils.TrySet<Divert>(nativeArray9, k, divert3);
				}
				return;
			}
			bool isUnspawned2 = ((ArchetypeChunk)(ref chunk)).Has<Unspawned>(ref m_UnspawnedType);
			HumanCurrentLane currentLane4 = default(HumanCurrentLane);
			Divert divert4 = default(Divert);
			DynamicBuffer<GroupCreature> groupCreatures2 = default(DynamicBuffer<GroupCreature>);
			for (int l = 0; l < nativeArray.Length; l++)
			{
				Entity val4 = nativeArray[l];
				PrefabRef prefabRef4 = nativeArray2[l];
				Game.Creatures.Resident resident4 = nativeArray4[l];
				Creature creature4 = nativeArray3[l];
				Human human2 = nativeArray11[l];
				HumanNavigation navigation4 = nativeArray8[l];
				Target target4 = nativeArray5[l];
				CollectionUtils.TryGet<HumanCurrentLane>(nativeArray7, l, ref currentLane4);
				CollectionUtils.TryGet<Divert>(nativeArray9, l, ref divert4);
				CollectionUtils.TryGet<GroupCreature>(bufferAccessor, l, ref groupCreatures2);
				PathOwner pathOwner4 = m_PathOwnerData[val4];
				CreatureUtils.CheckUnspawned(unfilteredChunkIndex, val4, currentLane4, human2, isUnspawned2, m_CommandBuffer);
				TickWalking(unfilteredChunkIndex, ref random, val4, prefabRef4, navigation4, isUnspawned2, ref resident4, ref creature4, ref human2, ref currentLane4, ref pathOwner4, ref target4, ref divert4, groupCreatures2);
				TickQueue(ref random, ref resident4, ref creature4, ref currentLane4);
				m_PathOwnerData[val4] = pathOwner4;
				nativeArray4[l] = resident4;
				nativeArray3[l] = creature4;
				nativeArray11[l] = human2;
				nativeArray5[l] = target4;
				CollectionUtils.TrySet<HumanCurrentLane>(nativeArray7, l, currentLane4);
				CollectionUtils.TrySet<Divert>(nativeArray9, l, divert4);
			}
		}

		private void TickGroupMemberInVehicle(int jobIndex, ref Random random, Entity entity, PrefabRef prefabRef, HumanNavigation navigation, GroupMember groupMember, CurrentVehicle currentVehicle, bool hasCurrentLane, ref Game.Creatures.Resident resident, ref Human human, ref HumanCurrentLane currentLane, ref PathOwner pathOwner, ref Target target, ref Divert divert)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			if (!((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(currentVehicle.m_Vehicle))
			{
				AddDeletedResident(DeletedResidentType.InvalidVehicleTarget);
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, entity, default(Deleted));
				return;
			}
			Entity val = currentVehicle.m_Vehicle;
			Controller controller = default(Controller);
			if (m_ControllerData.TryGetComponent(currentVehicle.m_Vehicle, ref controller) && controller.m_Controller != Entity.Null)
			{
				val = controller.m_Controller;
			}
			if ((currentVehicle.m_Flags & CreatureVehicleFlags.Ready) == 0)
			{
				if (hasCurrentLane)
				{
					if (CreatureUtils.IsStuck(pathOwner))
					{
						AddDeletedResident(DeletedResidentType.StuckLoop);
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, entity, default(Deleted));
						return;
					}
					if (!m_CurrentVehicleData.HasComponent(groupMember.m_Leader))
					{
						CancelEnterVehicle(entity, currentVehicle.m_Vehicle, ref resident, ref human, ref currentLane, ref pathOwner);
						return;
					}
					Game.Vehicles.PublicTransport publicTransport = default(Game.Vehicles.PublicTransport);
					if (m_PublicTransportData.TryGetComponent(val, ref publicTransport))
					{
						if (m_SimulationFrameIndex >= publicTransport.m_DepartureFrame)
						{
							human.m_Flags |= HumanFlags.Run;
						}
						if ((publicTransport.m_State & PublicTransportFlags.Boarding) == 0 && currentLane.m_Lane == currentVehicle.m_Vehicle)
						{
							currentLane.m_Flags |= CreatureLaneFlags.EndReached;
						}
					}
					if (CreatureUtils.ParkingSpaceReached(currentLane) || CreatureUtils.TransportStopReached(currentLane))
					{
						SetEnterVehiclePath(entity, currentVehicle.m_Vehicle, groupMember, ref random, ref currentLane, ref pathOwner);
					}
					else if (CreatureUtils.PathEndReached(currentLane) || CreatureUtils.RequireNewPath(pathOwner) || resident.m_Timer >= 250)
					{
						if (ShouldFinishEnterVehicle(navigation))
						{
							FinishEnterVehicle(entity, currentVehicle.m_Vehicle, val, ref resident, ref human, ref currentLane);
							hasCurrentLane = false;
						}
						else if ((currentVehicle.m_Flags & CreatureVehicleFlags.Entering) == 0)
						{
							currentVehicle.m_Flags |= CreatureVehicleFlags.Entering;
							((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CurrentVehicle>(jobIndex, entity, currentVehicle);
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(jobIndex, entity, default(BatchesUpdated));
						}
					}
					else if (CreatureUtils.ActionLocationReached(currentLane) && ActionLocationReached(entity, ref resident, ref human, ref currentLane, ref pathOwner))
					{
						return;
					}
				}
				if (!hasCurrentLane)
				{
					currentVehicle.m_Flags &= ~CreatureVehicleFlags.Entering;
					currentVehicle.m_Flags |= CreatureVehicleFlags.Ready;
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CurrentVehicle>(jobIndex, entity, currentVehicle);
				}
			}
			else if ((currentVehicle.m_Flags & CreatureVehicleFlags.Exiting) != 0)
			{
				if (ShouldFinishExitVehicle(navigation))
				{
					FinishExitVehicle(entity, currentVehicle.m_Vehicle, ref currentLane);
				}
			}
			else
			{
				if ((resident.m_Flags & ResidentFlags.Disembarking) == 0 && !m_CurrentVehicleData.HasComponent(groupMember.m_Leader))
				{
					GroupLeaderDisembarking(entity, ref resident, ref pathOwner);
				}
				if ((resident.m_Flags & ResidentFlags.Disembarking) != ResidentFlags.None)
				{
					ExitVehicle(entity, jobIndex, ref random, val, prefabRef, currentVehicle, ref resident, ref human, ref divert, ref pathOwner);
				}
			}
			UpdateMoodFlags(ref random, navigation, hasCurrentLane, ref resident, ref human, ref divert);
		}

		private void TickInVehicle(int jobIndex, ref Random random, Entity entity, PrefabRef prefabRef, HumanNavigation navigation, CurrentVehicle currentVehicle, bool hasCurrentLane, ref Game.Creatures.Resident resident, ref Creature creature, ref Human human, ref HumanCurrentLane currentLane, ref PathOwner pathOwner, ref Target target, ref Divert divert, DynamicBuffer<GroupCreature> groupCreatures)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0524: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0422: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0385: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0469: Unknown result type (might be due to invalid IL or missing references)
			//IL_0430: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0477: Unknown result type (might be due to invalid IL or missing references)
			//IL_0455: Unknown result type (might be due to invalid IL or missing references)
			//IL_0456: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_0443: Unknown result type (might be due to invalid IL or missing references)
			//IL_040e: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0499: Unknown result type (might be due to invalid IL or missing references)
			//IL_049a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0489: Unknown result type (might be due to invalid IL or missing references)
			//IL_048a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04da: Unknown result type (might be due to invalid IL or missing references)
			//IL_04db: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
			if (!m_PrefabRefData.HasComponent(currentVehicle.m_Vehicle))
			{
				AddDeletedResident(DeletedResidentType.InvalidVehicleTarget);
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, entity, default(Deleted));
				return;
			}
			if (CreatureUtils.ResetUpdatedPath(ref pathOwner) && CheckPath(jobIndex, entity, prefabRef, ref random, ref creature, ref human, ref currentLane, ref target, ref divert, ref pathOwner, ref resident))
			{
				FindNewPath(entity, prefabRef, ref resident, ref human, ref currentLane, ref pathOwner, ref target, ref divert);
				return;
			}
			Entity val = currentVehicle.m_Vehicle;
			Controller controller = default(Controller);
			if (m_ControllerData.TryGetComponent(currentVehicle.m_Vehicle, ref controller) && controller.m_Controller != Entity.Null)
			{
				val = controller.m_Controller;
			}
			if ((currentVehicle.m_Flags & CreatureVehicleFlags.Ready) == 0)
			{
				if (hasCurrentLane)
				{
					if (CreatureUtils.IsStuck(pathOwner))
					{
						AddDeletedResident(DeletedResidentType.StuckLoop);
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, entity, default(Deleted));
						return;
					}
					Game.Vehicles.PublicTransport publicTransport = default(Game.Vehicles.PublicTransport);
					if (m_PublicTransportData.TryGetComponent(val, ref publicTransport))
					{
						if (m_SimulationFrameIndex >= publicTransport.m_DepartureFrame)
						{
							human.m_Flags |= HumanFlags.Run;
						}
						if ((publicTransport.m_State & PublicTransportFlags.Boarding) == 0)
						{
							if (!(currentLane.m_Lane == currentVehicle.m_Vehicle))
							{
								CancelEnterVehicle(entity, currentVehicle.m_Vehicle, ref resident, ref human, ref currentLane, ref pathOwner);
								return;
							}
							currentLane.m_Flags |= CreatureLaneFlags.EndReached;
						}
					}
					if (CreatureUtils.ParkingSpaceReached(currentLane) || CreatureUtils.TransportStopReached(currentLane))
					{
						SetEnterVehiclePath(entity, currentVehicle.m_Vehicle, default(GroupMember), ref random, ref currentLane, ref pathOwner);
					}
					else if (CreatureUtils.PathEndReached(currentLane) || CreatureUtils.RequireNewPath(pathOwner) || resident.m_Timer >= 250)
					{
						if (ShouldFinishEnterVehicle(navigation))
						{
							FinishEnterVehicle(entity, currentVehicle.m_Vehicle, val, ref resident, ref human, ref currentLane);
							hasCurrentLane = false;
						}
						else if ((currentVehicle.m_Flags & CreatureVehicleFlags.Entering) == 0)
						{
							currentVehicle.m_Flags |= CreatureVehicleFlags.Entering;
							((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CurrentVehicle>(jobIndex, entity, currentVehicle);
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(jobIndex, entity, default(BatchesUpdated));
						}
					}
					else if (CreatureUtils.ActionLocationReached(currentLane) && ActionLocationReached(entity, ref resident, ref human, ref currentLane, ref pathOwner))
					{
						return;
					}
				}
				if (!hasCurrentLane && HasEveryoneBoarded(groupCreatures))
				{
					currentVehicle.m_Flags &= ~CreatureVehicleFlags.Entering;
					currentVehicle.m_Flags |= CreatureVehicleFlags.Ready;
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CurrentVehicle>(jobIndex, entity, currentVehicle);
				}
			}
			else if ((currentVehicle.m_Flags & CreatureVehicleFlags.Exiting) != 0)
			{
				if (ShouldFinishExitVehicle(navigation))
				{
					FinishExitVehicle(entity, currentVehicle.m_Vehicle, ref currentLane);
				}
			}
			else
			{
				if ((resident.m_Flags & ResidentFlags.Disembarking) == 0)
				{
					if (m_DestroyedData.HasComponent(val))
					{
						if (!m_MovingData.HasComponent(val))
						{
							resident.m_Flags |= ResidentFlags.Disembarking;
							pathOwner.m_State &= ~PathFlags.Failed;
							pathOwner.m_State |= PathFlags.Obsolete;
						}
					}
					else if (m_PersonalCarData.HasComponent(val))
					{
						Game.Vehicles.PersonalCar personalCar = m_PersonalCarData[val];
						if ((personalCar.m_State & PersonalCarFlags.Disembarking) != 0)
						{
							CurrentVehicleDisembarking(jobIndex, entity, val, ref resident, ref pathOwner, ref target);
						}
						else if ((personalCar.m_State & PersonalCarFlags.Transporting) != 0)
						{
							CurrentVehicleTransporting(entity, val, ref pathOwner);
						}
					}
					else if (m_PublicTransportData.HasComponent(val))
					{
						Game.Vehicles.PublicTransport publicTransport2 = m_PublicTransportData[val];
						if ((publicTransport2.m_State & PublicTransportFlags.Boarding) != 0)
						{
							CurrentVehicleBoarding(jobIndex, entity, val, ref resident, ref pathOwner, ref target);
						}
						else if ((publicTransport2.m_State & (PublicTransportFlags.Testing | PublicTransportFlags.RequireStop)) == PublicTransportFlags.Testing)
						{
							CurrentVehicleTesting(jobIndex, entity, val, ref resident, ref pathOwner, ref target);
						}
					}
					else if (m_TaxiData.HasComponent(val))
					{
						Game.Vehicles.Taxi taxi = m_TaxiData[val];
						if ((taxi.m_State & TaxiFlags.Disembarking) != 0)
						{
							CurrentVehicleDisembarking(jobIndex, entity, val, ref resident, ref pathOwner, ref target);
						}
						else if ((taxi.m_State & TaxiFlags.Transporting) != 0)
						{
							CurrentVehicleTransporting(entity, val, ref pathOwner);
						}
					}
					else if (m_PoliceCarData.HasComponent(val))
					{
						if ((m_PoliceCarData[val].m_State & PoliceCarFlags.Disembarking) != 0)
						{
							CurrentVehicleDisembarking(jobIndex, entity, val, ref resident, ref pathOwner, ref target);
						}
						else
						{
							CurrentVehicleTransporting(entity, val, ref pathOwner);
						}
					}
					else if (m_AmbulanceData.HasComponent(val))
					{
						if ((m_AmbulanceData[val].m_State & AmbulanceFlags.Disembarking) != 0)
						{
							CurrentVehicleDisembarking(jobIndex, entity, val, ref resident, ref pathOwner, ref target);
						}
						else
						{
							CurrentVehicleTransporting(entity, val, ref pathOwner);
						}
					}
					else if (m_HearseData.HasComponent(val))
					{
						if ((m_HearseData[val].m_State & HearseFlags.Disembarking) != 0)
						{
							CurrentVehicleDisembarking(jobIndex, entity, val, ref resident, ref pathOwner, ref target);
						}
						else
						{
							CurrentVehicleTransporting(entity, val, ref pathOwner);
						}
					}
				}
				if ((resident.m_Flags & ResidentFlags.Disembarking) != ResidentFlags.None)
				{
					ExitVehicle(entity, jobIndex, ref random, val, prefabRef, currentVehicle, ref resident, ref human, ref divert, ref pathOwner);
				}
				else if ((currentVehicle.m_Flags & CreatureVehicleFlags.Leader) == 0)
				{
					currentVehicle.m_Flags |= CreatureVehicleFlags.Leader;
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CurrentVehicle>(jobIndex, entity, currentVehicle);
				}
			}
			UpdateMoodFlags(ref random, navigation, hasCurrentLane, ref resident, ref human, ref divert);
		}

		private static bool ShouldFinishEnterVehicle(HumanNavigation humanNavigation)
		{
			if (humanNavigation.m_TargetActivity == 10)
			{
				if (humanNavigation.m_LastActivity == humanNavigation.m_TargetActivity)
				{
					return humanNavigation.m_TransformState != TransformState.Action;
				}
				return false;
			}
			return true;
		}

		private static bool ShouldFinishExitVehicle(HumanNavigation humanNavigation)
		{
			if (humanNavigation.m_TargetActivity == 11)
			{
				if (humanNavigation.m_LastActivity == humanNavigation.m_TargetActivity)
				{
					return humanNavigation.m_TransformState != TransformState.Action;
				}
				return false;
			}
			return true;
		}

		private void TickGroupMemberWalking(int jobIndex, ref Random random, Entity entity, PrefabRef prefabRef, HumanNavigation navigation, GroupMember groupMember, ref Game.Creatures.Resident resident, ref Creature creature, ref Human human, ref HumanCurrentLane currentLane, ref PathOwner pathOwner, ref Target target, ref Divert divert)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			if ((resident.m_Flags & ResidentFlags.Disembarking) != ResidentFlags.None)
			{
				resident.m_Flags &= ~ResidentFlags.Disembarking;
			}
			else if (divert.m_Purpose == Purpose.None && !((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(target.m_Target))
			{
				HealthProblem healthProblem = default(HealthProblem);
				if (m_HealthProblemData.TryGetComponent(resident.m_Citizen, ref healthProblem) && (healthProblem.m_Flags & HealthProblemFlags.RequireTransport) != HealthProblemFlags.None && (healthProblem.m_Flags & (HealthProblemFlags.Dead | HealthProblemFlags.Injured)) != HealthProblemFlags.None)
				{
					if ((healthProblem.m_Flags & HealthProblemFlags.Dead) != HealthProblemFlags.None)
					{
						AddDeletedResident(DeletedResidentType.Dead);
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, entity, default(Deleted));
					}
					else
					{
						SetTarget(jobIndex, entity, ref resident, ref currentLane, ref divert, ref pathOwner, ref target, Purpose.None, Entity.Null);
						WaitHere(entity, ref currentLane, ref pathOwner);
					}
					return;
				}
				if (ReturnHome(jobIndex, entity, ref random, ref resident, ref currentLane, ref target, ref divert, ref pathOwner))
				{
					return;
				}
			}
			else
			{
				if (CreatureUtils.IsStuck(pathOwner))
				{
					AddDeletedResident(DeletedResidentType.StuckLoop);
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, entity, default(Deleted));
					return;
				}
				if (CreatureUtils.ActionLocationReached(currentLane) && ActionLocationReached(entity, ref resident, ref human, ref currentLane, ref pathOwner))
				{
					return;
				}
			}
			Human human2 = default(Human);
			if ((resident.m_Flags & ResidentFlags.Arrived) == 0 && m_HumanData.TryGetComponent(groupMember.m_Leader, ref human2))
			{
				human.m_Flags |= human2.m_Flags & (HumanFlags.Run | HumanFlags.Emergency);
			}
			UpdateMoodFlags(ref random, navigation, hasCurrentLane: true, ref resident, ref human, ref divert);
			if (!m_CurrentVehicleData.HasComponent(groupMember.m_Leader) || (currentLane.m_Flags & CreatureLaneFlags.EndReached) == 0)
			{
				return;
			}
			CurrentVehicle currentVehicle = m_CurrentVehicleData[groupMember.m_Leader];
			Transform transform = m_TransformData[entity];
			Entity vehicle = currentVehicle.m_Vehicle;
			if (m_ControllerData.HasComponent(currentVehicle.m_Vehicle))
			{
				Controller controller = m_ControllerData[currentVehicle.m_Vehicle];
				if (controller.m_Controller != Entity.Null)
				{
					vehicle = controller.m_Controller;
				}
			}
			m_BoardingQueue.Enqueue(Boarding.TryEnterVehicle(entity, groupMember.m_Leader, vehicle, currentVehicle.m_Vehicle, Entity.Null, transform.m_Position, (CreatureVehicleFlags)0u));
		}

		private void TickWalking(int jobIndex, ref Random random, Entity entity, PrefabRef prefabRef, HumanNavigation navigation, bool isUnspawned, ref Game.Creatures.Resident resident, ref Creature creature, ref Human human, ref HumanCurrentLane currentLane, ref PathOwner pathOwner, ref Target target, ref Divert divert, DynamicBuffer<GroupCreature> groupCreatures)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			if (CreatureUtils.ResetUpdatedPath(ref pathOwner) && CheckPath(jobIndex, entity, prefabRef, ref random, ref creature, ref human, ref currentLane, ref target, ref divert, ref pathOwner, ref resident))
			{
				FindNewPath(entity, prefabRef, ref resident, ref human, ref currentLane, ref pathOwner, ref target, ref divert);
				return;
			}
			if (CreatureUtils.ResetUncheckedLane(ref currentLane) && CheckLane(jobIndex, entity, prefabRef, ref random, ref human, ref currentLane, ref target, ref divert, ref pathOwner, ref resident))
			{
				FindNewPath(entity, prefabRef, ref resident, ref human, ref currentLane, ref pathOwner, ref target, ref divert);
				return;
			}
			UpdateMoodFlags(ref random, navigation, hasCurrentLane: true, ref resident, ref human, ref divert);
			if ((resident.m_Flags & ResidentFlags.Disembarking) != ResidentFlags.None)
			{
				resident.m_Flags &= ~ResidentFlags.Disembarking;
			}
			else if (divert.m_Purpose == Purpose.None && !((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(target.m_Target))
			{
				if (HandleHealthProblem(jobIndex, entity, ref resident, ref currentLane, ref pathOwner) || ReturnHome(jobIndex, entity, ref random, ref resident, ref currentLane, ref target, ref divert, ref pathOwner))
				{
					return;
				}
			}
			else if (CreatureUtils.PathfindFailed(pathOwner))
			{
				if (CreatureUtils.IsStuck(pathOwner))
				{
					AddDeletedResident(DeletedResidentType.StuckLoop);
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, entity, default(Deleted));
					return;
				}
				if (divert.m_Purpose != Purpose.None)
				{
					SetDivert(jobIndex, entity, ref resident, ref currentLane, ref divert, ref pathOwner, Purpose.None, Entity.Null, 0, Resource.NoResource);
				}
				else if (ReturnHome(jobIndex, entity, ref random, ref resident, ref currentLane, ref target, ref divert, ref pathOwner))
				{
					return;
				}
			}
			else if (CreatureUtils.EndReached(currentLane))
			{
				if (CreatureUtils.PathEndReached(currentLane))
				{
					if (PathEndReached(jobIndex, entity, ref random, ref resident, ref human, ref currentLane, ref target, ref divert, ref pathOwner))
					{
						return;
					}
				}
				else if (CreatureUtils.ParkingSpaceReached(currentLane))
				{
					if (ParkingSpaceReached(jobIndex, ref random, entity, ref resident, ref currentLane, ref pathOwner, ref target, groupCreatures))
					{
						return;
					}
				}
				else if (CreatureUtils.TransportStopReached(currentLane))
				{
					if (TransportStopReached(jobIndex, ref random, entity, prefabRef, isUnspawned, ref resident, ref currentLane, ref pathOwner, ref target))
					{
						return;
					}
				}
				else if (CreatureUtils.ActionLocationReached(currentLane) && ActionLocationReached(entity, ref resident, ref human, ref currentLane, ref pathOwner))
				{
					return;
				}
			}
			else if (currentLane.m_QueueArea.radius > 0f)
			{
				QueueReached(entity, ref resident, ref currentLane, ref pathOwner);
			}
			if (CreatureUtils.RequireNewPath(pathOwner))
			{
				FindNewPath(entity, prefabRef, ref resident, ref human, ref currentLane, ref pathOwner, ref target, ref divert);
			}
		}

		private bool HandleHealthProblem(int jobIndex, Entity entity, ref Game.Creatures.Resident resident, ref HumanCurrentLane currentLane, ref PathOwner pathOwner)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			HealthProblem healthProblem = default(HealthProblem);
			if (!m_HealthProblemData.TryGetComponent(resident.m_Citizen, ref healthProblem))
			{
				return false;
			}
			if ((healthProblem.m_Flags & HealthProblemFlags.RequireTransport) == 0)
			{
				return false;
			}
			if ((healthProblem.m_Flags & HealthProblemFlags.Dead) != HealthProblemFlags.None)
			{
				AddDeletedResident(DeletedResidentType.Dead);
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, entity, default(Deleted));
				return true;
			}
			if ((healthProblem.m_Flags & HealthProblemFlags.Injured) != HealthProblemFlags.None)
			{
				WaitHere(entity, ref currentLane, ref pathOwner);
				return true;
			}
			CurrentBuilding currentBuilding = default(CurrentBuilding);
			if ((healthProblem.m_Flags & HealthProblemFlags.Sick) != HealthProblemFlags.None && m_CurrentBuildingData.TryGetComponent(resident.m_Citizen, ref currentBuilding) && currentBuilding.m_CurrentBuilding != Entity.Null)
			{
				WaitHere(entity, ref currentLane, ref pathOwner);
				return true;
			}
			return false;
		}

		private void UpdateMoodFlags(ref Random random, HumanNavigation navigation, bool hasCurrentLane, ref Game.Creatures.Resident resident, ref Human human, ref Divert divert)
		{
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			if (hasCurrentLane && (resident.m_Flags & ResidentFlags.Arrived) == 0 && navigation.m_MaxSpeed < 0.1f)
			{
				if ((human.m_Flags & HumanFlags.Waiting) == 0 && ((Random)(ref random)).NextInt(10) == 0)
				{
					human.m_Flags |= HumanFlags.Waiting;
				}
			}
			else
			{
				human.m_Flags &= ~HumanFlags.Waiting;
			}
			if (divert.m_Purpose == Purpose.Safety)
			{
				human.m_Flags |= HumanFlags.Angry;
			}
			else if ((human.m_Flags & HumanFlags.Angry) != 0 && ((Random)(ref random)).NextInt(10) == 0)
			{
				human.m_Flags &= ~HumanFlags.Angry;
			}
			if (((Random)(ref random)).NextInt(100) == 0)
			{
				int num = ((Random)(ref random)).NextInt(20, 40);
				int num2 = ((Random)(ref random)).NextInt(60, 80);
				Citizen citizen = default(Citizen);
				int num3 = ((!m_CitizenData.TryGetComponent(resident.m_Citizen, ref citizen)) ? ((Random)(ref random)).NextInt(101) : citizen.Happiness);
				if (num3 < num)
				{
					human.m_Flags &= ~HumanFlags.Happy;
					human.m_Flags |= HumanFlags.Sad;
				}
				else if (citizen.Happiness > num2)
				{
					human.m_Flags &= ~HumanFlags.Sad;
					human.m_Flags |= HumanFlags.Happy;
				}
				else
				{
					human.m_Flags &= ~(HumanFlags.Sad | HumanFlags.Happy);
				}
			}
		}

		private void SetEnterVehiclePath(Entity entity, Entity vehicle, GroupMember groupMember, ref Random random, ref HumanCurrentLane currentLane, ref PathOwner pathOwner)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			currentLane.m_Flags &= ~(CreatureLaneFlags.ParkingSpace | CreatureLaneFlags.Transport | CreatureLaneFlags.Taxi);
			DynamicBuffer<PathElement> path = m_PathElements[entity];
			if (groupMember.m_Leader != Entity.Null)
			{
				path.Clear();
				path.Add(new PathElement(vehicle, float2.op_Implicit(0f)));
				pathOwner.m_ElementIndex = 0;
			}
			else
			{
				if (path.Length > pathOwner.m_ElementIndex && path[pathOwner.m_ElementIndex].m_Target == vehicle)
				{
					return;
				}
				if (pathOwner.m_ElementIndex > 0)
				{
					path[--pathOwner.m_ElementIndex] = new PathElement(vehicle, float2.op_Implicit(0f));
				}
				else
				{
					path.Insert(pathOwner.m_ElementIndex, new PathElement(vehicle, float2.op_Implicit(0f)));
				}
			}
			if (m_TransformData.HasComponent(vehicle) && m_LaneData.HasComponent(currentLane.m_Lane))
			{
				float3 position = m_TransformData[vehicle].m_Position;
				if (pathOwner.m_ElementIndex > 0)
				{
					path[--pathOwner.m_ElementIndex] = new PathElement(currentLane.m_Lane, float2.op_Implicit(currentLane.m_CurvePosition.y));
				}
				else
				{
					path.Insert(pathOwner.m_ElementIndex, new PathElement(currentLane.m_Lane, float2.op_Implicit(currentLane.m_CurvePosition.y)));
				}
				CreatureUtils.FixEnterPath(ref random, position, pathOwner.m_ElementIndex, path, ref m_OwnerData, ref m_LaneData, ref m_EdgeLaneData, ref m_ConnectionLaneData, ref m_CurveData, ref m_SubLanes, ref m_AreaNodes, ref m_AreaTriangles);
			}
		}

		private unsafe void AddDeletedResident(DeletedResidentType type)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			Interlocked.Increment(ref UnsafeUtility.ArrayElementAsRef<int>(NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<int>(m_DeletedResidents), (int)type));
		}

		private void TickQueue(ref Random random, ref Game.Creatures.Resident resident, ref Creature creature, ref HumanCurrentLane currentLane)
		{
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			resident.m_Timer += ((Random)(ref random)).NextInt(1, 3);
			if (currentLane.m_QueueArea.radius > 0f)
			{
				if ((creature.m_QueueArea.radius == 0f || currentLane.m_QueueEntity != creature.m_QueueEntity) && (m_RouteConnectedData.HasComponent(currentLane.m_QueueEntity) || m_BoardingVehicleData.HasComponent(currentLane.m_QueueEntity)) && (resident.m_Flags & ResidentFlags.WaitingTransport) == 0)
				{
					resident.m_Flags |= ResidentFlags.WaitingTransport;
					resident.m_Timer = 0;
				}
				creature.m_QueueEntity = currentLane.m_QueueEntity;
				creature.m_QueueArea = currentLane.m_QueueArea;
			}
			else
			{
				creature.m_QueueEntity = Entity.Null;
				creature.m_QueueArea = default(Sphere3);
			}
		}

		private void QueueReached(Entity entity, ref Game.Creatures.Resident resident, ref HumanCurrentLane currentLane, ref PathOwner pathOwner)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			if ((resident.m_Flags & ResidentFlags.WaitingTransport) != ResidentFlags.None && resident.m_Timer >= 5000)
			{
				m_BoardingQueue.Enqueue(Boarding.WaitTimeExceeded(entity, currentLane.m_QueueEntity));
				if (m_BoardingVehicleData.HasComponent(currentLane.m_QueueEntity))
				{
					resident.m_Flags |= ResidentFlags.IgnoreTaxi;
				}
				else
				{
					resident.m_Flags |= ResidentFlags.IgnoreTransport;
				}
				pathOwner.m_State &= ~PathFlags.Failed;
				pathOwner.m_State |= PathFlags.Obsolete;
			}
		}

		private void WaitHere(Entity entity, ref HumanCurrentLane currentLane, ref PathOwner pathOwner)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			currentLane.m_CurvePosition.y = currentLane.m_CurvePosition.x;
			pathOwner.m_ElementIndex = 0;
			m_PathElements[entity].Clear();
		}

		private void ExitVehicle(Entity entity, int jobIndex, ref Random random, Entity controllerVehicle, PrefabRef prefabRef, CurrentVehicle currentVehicle, ref Game.Creatures.Resident resident, ref Human human, ref Divert divert, ref PathOwner pathOwner)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0464: Unknown result type (might be due to invalid IL or missing references)
			//IL_0472: Unknown result type (might be due to invalid IL or missing references)
			//IL_0473: Unknown result type (might be due to invalid IL or missing references)
			//IL_0476: Unknown result type (might be due to invalid IL or missing references)
			//IL_0487: Unknown result type (might be due to invalid IL or missing references)
			//IL_048e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			//IL_043c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0445: Unknown result type (might be due to invalid IL or missing references)
			//IL_044c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03df: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			Entity household = GetHousehold(resident);
			int ticketPrice = 0;
			if ((currentVehicle.m_Flags & CreatureVehicleFlags.Leader) != 0 && m_TaxiData.HasComponent(controllerVehicle))
			{
				Game.Vehicles.Taxi taxi = m_TaxiData[controllerVehicle];
				ticketPrice = math.select((int)taxi.m_CurrentFee, -taxi.m_CurrentFee, (taxi.m_State & TaxiFlags.FromOutside) != 0);
			}
			if (m_TransformData.HasComponent(currentVehicle.m_Vehicle))
			{
				Transform transform = m_TransformData[currentVehicle.m_Vehicle];
				Transform transform2 = transform;
				DynamicBuffer<PathElement> path = m_PathElements[entity];
				HumanCurrentLane newCurrentLane = default(HumanCurrentLane);
				float3 targetPosition = transform2.m_Position;
				if (pathOwner.m_ElementIndex < path.Length && (pathOwner.m_State & PathFlags.Obsolete) == 0)
				{
					PathElement pathElement = path[pathOwner.m_ElementIndex];
					Curve curve = default(Curve);
					Transform transform3 = default(Transform);
					if (m_CurveData.TryGetComponent(pathElement.m_Target, ref curve))
					{
						targetPosition = MathUtils.Position(curve.m_Bezier, pathElement.m_TargetDelta.x);
					}
					else if (m_TransformData.TryGetComponent(pathElement.m_Target, ref transform3))
					{
						targetPosition = transform3.m_Position;
					}
				}
				BufferLookup<SubMeshGroup> subMeshGroupBuffers = default(BufferLookup<SubMeshGroup>);
				BufferLookup<CharacterElement> characterElementBuffers = default(BufferLookup<CharacterElement>);
				BufferLookup<SubMesh> subMeshBuffers = default(BufferLookup<SubMesh>);
				BufferLookup<AnimationClip> animationClipBuffers = default(BufferLookup<AnimationClip>);
				BufferLookup<AnimationMotion> animationMotionBuffers = default(BufferLookup<AnimationMotion>);
				bool isDriver = (currentVehicle.m_Flags & CreatureVehicleFlags.Driver) != 0;
				ActivityCondition conditions = CreatureUtils.GetConditions(human);
				transform2 = CreatureUtils.GetVehicleDoorPosition(ref random, ActivityType.Exit, conditions, transform, targetPosition, isDriver, m_LefthandTraffic, prefabRef.m_Prefab, currentVehicle.m_Vehicle, default(DynamicBuffer<MeshGroup>), ref m_PublicTransportData, ref m_TrainData, ref m_ControllerData, ref m_PrefabRefData, ref m_PrefabCarData, ref m_PrefabActivityLocationElements, ref subMeshGroupBuffers, ref characterElementBuffers, ref subMeshBuffers, ref animationClipBuffers, ref animationMotionBuffers, out var activityMask, out var _);
				if (pathOwner.m_ElementIndex < path.Length && (pathOwner.m_State & PathFlags.Obsolete) == 0)
				{
					CreatureUtils.FixPathStart(ref random, transform2.m_Position, pathOwner.m_ElementIndex, path, ref m_OwnerData, ref m_LaneData, ref m_EdgeLaneData, ref m_ConnectionLaneData, ref m_CurveData, ref m_SubLanes, ref m_AreaNodes, ref m_AreaTriangles);
					PathElement pathElement2 = path[pathOwner.m_ElementIndex];
					CreatureLaneFlags creatureLaneFlags = (CreatureLaneFlags)0u;
					if (pathElement2.m_TargetDelta.y < pathElement2.m_TargetDelta.x)
					{
						creatureLaneFlags |= CreatureLaneFlags.Backward;
					}
					if (m_PedestrianLaneData.HasComponent(pathElement2.m_Target))
					{
						newCurrentLane = new HumanCurrentLane(pathElement2, creatureLaneFlags);
					}
					else if (m_ConnectionLaneData.HasComponent(pathElement2.m_Target))
					{
						Game.Net.ConnectionLane connectionLane = m_ConnectionLaneData[pathElement2.m_Target];
						if ((connectionLane.m_Flags & ConnectionLaneFlags.Area) != 0)
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
						if ((connectionLane.m_Flags & ConnectionLaneFlags.Parking) == 0)
						{
							newCurrentLane = new HumanCurrentLane(pathElement2, creatureLaneFlags);
						}
					}
					else if (m_SpawnLocation.HasComponent(pathElement2.m_Target))
					{
						creatureLaneFlags |= CreatureLaneFlags.TransformTarget;
						if (++pathOwner.m_ElementIndex >= path.Length)
						{
							creatureLaneFlags |= CreatureLaneFlags.EndOfPath;
						}
						newCurrentLane = new HumanCurrentLane(pathElement2, creatureLaneFlags);
					}
					else if (m_PrefabRefData.HasComponent(pathElement2.m_Target))
					{
						creatureLaneFlags |= CreatureLaneFlags.FindLane;
						newCurrentLane = new HumanCurrentLane(creatureLaneFlags);
					}
				}
				if (newCurrentLane.m_Lane == Entity.Null)
				{
					if (m_UnspawnedData.HasComponent(currentVehicle.m_Vehicle))
					{
						newCurrentLane.m_Flags |= CreatureLaneFlags.EmergeUnspawned;
					}
					PathOwner pathOwner2 = default(PathOwner);
					if (m_PathOwnerData.TryGetComponent(controllerVehicle, ref pathOwner2) && VehicleUtils.PathfindFailed(pathOwner2))
					{
						newCurrentLane.m_Flags |= CreatureLaneFlags.EmergeUnspawned;
						pathOwner.m_State |= PathFlags.Stuck;
					}
				}
				if ((activityMask.m_Mask & new ActivityMask(ActivityType.Driving).m_Mask) != 0)
				{
					newCurrentLane.m_Flags |= CreatureLaneFlags.EndOfPath | CreatureLaneFlags.EndReached;
				}
				m_BoardingQueue.Enqueue(Boarding.ExitVehicle(entity, household, currentVehicle.m_Vehicle, newCurrentLane, transform2.m_Position, transform2.m_Rotation, ticketPrice));
			}
			else
			{
				Transform transform4 = m_TransformData[entity];
				m_BoardingQueue.Enqueue(Boarding.ExitVehicle(entity, household, currentVehicle.m_Vehicle, default(HumanCurrentLane), transform4.m_Position, transform4.m_Rotation, ticketPrice));
				pathOwner.m_State &= ~PathFlags.Failed;
				pathOwner.m_State |= PathFlags.Obsolete;
			}
			currentVehicle.m_Flags |= CreatureVehicleFlags.Exiting;
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CurrentVehicle>(jobIndex, entity, currentVehicle);
			switch (divert.m_Purpose)
			{
			case Purpose.None:
			{
				TravelPurpose travelPurpose = default(TravelPurpose);
				if (m_TravelPurposeData.TryGetComponent(resident.m_Citizen, ref travelPurpose) && travelPurpose.m_Purpose == Purpose.EmergencyShelter)
				{
					human.m_Flags |= HumanFlags.Run | HumanFlags.Emergency;
				}
				break;
			}
			case Purpose.Safety:
			case Purpose.Escape:
				human.m_Flags |= HumanFlags.Run;
				break;
			}
		}

		private bool HasEveryoneBoarded(DynamicBuffer<GroupCreature> group)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			if (group.IsCreated)
			{
				for (int i = 0; i < group.Length; i++)
				{
					Entity creature = group[i].m_Creature;
					if (!m_CurrentVehicleData.HasComponent(creature))
					{
						return false;
					}
					if ((m_CurrentVehicleData[creature].m_Flags & CreatureVehicleFlags.Ready) == 0)
					{
						return false;
					}
				}
			}
			return true;
		}

		private bool CheckLane(int jobIndex, Entity entity, PrefabRef prefabRef, ref Random random, ref Human human, ref HumanCurrentLane currentLane, ref Target target, ref Divert divert, ref PathOwner pathOwner, ref Game.Creatures.Resident resident)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			Entity val = Entity.Null;
			if (m_OwnerData.HasComponent(currentLane.m_Lane))
			{
				val = m_OwnerData[currentLane.m_Lane].m_Owner;
			}
			DynamicBuffer<PathElement> val2 = m_PathElements[entity];
			if (val2.Length > pathOwner.m_ElementIndex)
			{
				PathElement pathElement = val2[pathOwner.m_ElementIndex];
				if (m_OwnerData.HasComponent(pathElement.m_Target))
				{
					Entity owner = m_OwnerData[pathElement.m_Target].m_Owner;
					if (owner != val)
					{
						return FindDivertTargets(jobIndex, entity, prefabRef, ref random, ref human, ref currentLane, ref target, ref divert, ref pathOwner, ref resident, owner, val);
					}
				}
			}
			return false;
		}

		private bool GetDivertNeeds(PrefabRef prefabRef, ref Random random, ref Human human, ref Game.Creatures.Resident resident, ref Divert divert, out ActivityMask actionMask, out HouseholdNeed householdNeed)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			CreatureData creatureData = m_PrefabCreatureData[prefabRef.m_Prefab];
			householdNeed = default(HouseholdNeed);
			actionMask = default(ActivityMask);
			if ((human.m_Flags & HumanFlags.Selfies) == 0)
			{
				actionMask.m_Mask |= creatureData.m_SupportedActivities.m_Mask & new ActivityMask(ActivityType.Selfies).m_Mask;
			}
			if ((resident.m_Flags & ResidentFlags.ActivityDone) != ResidentFlags.None)
			{
				if (((Random)(ref random)).NextInt(3) != 0)
				{
					actionMask = default(ActivityMask);
				}
				resident.m_Flags &= ~ResidentFlags.ActivityDone;
			}
			bool flag = actionMask.m_Mask != 0;
			if (divert.m_Purpose != Purpose.None)
			{
				return flag;
			}
			if (m_AttendingMeetingData.HasComponent(resident.m_Citizen))
			{
				AttendingMeeting attendingMeeting = m_AttendingMeetingData[resident.m_Citizen];
				if (m_PrefabRefData.HasComponent(attendingMeeting.m_Meeting))
				{
					return flag;
				}
			}
			if (m_CitizenData.HasComponent(resident.m_Citizen))
			{
				CitizenAge age = m_CitizenData[resident.m_Citizen].GetAge();
				if ((age == CitizenAge.Adult || age == CitizenAge.Elderly) && m_HouseholdMembers.HasComponent(resident.m_Citizen))
				{
					HouseholdMember householdMember = m_HouseholdMembers[resident.m_Citizen];
					if (m_HouseholdNeedData.HasComponent(householdMember.m_Household))
					{
						householdNeed = m_HouseholdNeedData[householdMember.m_Household];
						flag |= householdNeed.m_Resource != Resource.NoResource;
					}
				}
			}
			return flag;
		}

		private bool FindDivertTargets(int jobIndex, Entity entity, PrefabRef prefabRef, ref Random random, ref Human human, ref HumanCurrentLane currentLane, ref Target target, ref Divert divert, ref PathOwner pathOwner, ref Game.Creatures.Resident resident, Entity element, Entity ignoreElement)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			if (!GetDivertNeeds(prefabRef, ref random, ref human, ref resident, ref divert, out var actionMask, out var householdNeed))
			{
				return false;
			}
			if (m_ConnectedEdges.HasBuffer(element))
			{
				DynamicBuffer<ConnectedEdge> val = m_ConnectedEdges[element];
				for (int i = 0; i < val.Length; i++)
				{
					Entity edge = val[i].m_Edge;
					if (!(edge == ignoreElement) && FindDivertTargets(jobIndex, entity, ref random, ref human, ref currentLane, ref target, ref divert, ref pathOwner, ref resident, edge, ref actionMask, householdNeed))
					{
						return true;
					}
				}
				return false;
			}
			if (m_ConnectedEdges.HasBuffer(ignoreElement))
			{
				DynamicBuffer<ConnectedEdge> val2 = m_ConnectedEdges[ignoreElement];
				for (int j = 0; j < val2.Length; j++)
				{
					if (val2[j].m_Edge == element)
					{
						return false;
					}
				}
			}
			return FindDivertTargets(jobIndex, entity, ref random, ref human, ref currentLane, ref target, ref divert, ref pathOwner, ref resident, element, ref actionMask, householdNeed);
		}

		private HumanFlags SelectAttractionFlags(ref Random random, ActivityMask actionMask)
		{
			HumanFlags result = (HumanFlags)0u;
			int count = 0;
			CheckActionFlags(ref result, ref count, ref random, actionMask, ActivityType.Selfies, HumanFlags.Selfies);
			return result;
		}

		private void CheckActionFlags(ref HumanFlags result, ref int count, ref Random random, ActivityMask actionMask, ActivityType activityType, HumanFlags flags)
		{
			if ((actionMask.m_Mask & new ActivityMask(activityType).m_Mask) != 0 && ((Random)(ref random)).NextInt(++count) == 0)
			{
				result = flags;
			}
		}

		private bool FindDivertTargets(int jobIndex, Entity entity, ref Random random, ref Human human, ref HumanCurrentLane currentLane, ref Target target, ref Divert divert, ref PathOwner pathOwner, ref Game.Creatures.Resident resident, Entity element, ref ActivityMask actionMask, HouseholdNeed householdNeed)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			if (m_ConnectedBuildings.HasBuffer(element))
			{
				DynamicBuffer<ConnectedBuilding> val = m_ConnectedBuildings[element];
				int num = ((Random)(ref random)).NextInt(val.Length);
				bool flag = actionMask.m_Mask != 0;
				bool flag2 = householdNeed.m_Resource != Resource.NoResource;
				for (int i = 0; i < val.Length; i++)
				{
					int num2 = num + i;
					num2 = math.select(num2, num2 - val.Length, num2 >= val.Length);
					Entity building = val[num2].m_Building;
					if (flag)
					{
						int num3 = 0;
						if (m_AttractivenessProviderData.HasComponent(building))
						{
							num3 += m_AttractivenessProviderData[building].m_Attractiveness;
						}
						if (m_OnFireData.HasComponent(building))
						{
							num3 += Mathf.RoundToInt(m_OnFireData[building].m_Intensity);
						}
						if (((Random)(ref random)).NextInt(10) < num3 && AddPathAction(entity, ref random, ref currentLane, ref pathOwner, building))
						{
							human.m_Flags |= SelectAttractionFlags(ref random, actionMask);
							actionMask = default(ActivityMask);
							flag = false;
						}
					}
					if (building == target.m_Target || !flag2 || !m_Renters.HasBuffer(building))
					{
						continue;
					}
					DynamicBuffer<Renter> val2 = m_Renters[building];
					for (int j = 0; j < val2.Length; j++)
					{
						Entity renter = val2[j].m_Renter;
						if (renter == target.m_Target || !m_ServiceAvailableData.HasComponent(renter))
						{
							continue;
						}
						PrefabRef prefabRef = m_PrefabRefData[renter];
						if (m_PrefabIndustrialProcessData.HasComponent(prefabRef.m_Prefab) && (m_PrefabIndustrialProcessData[prefabRef.m_Prefab].m_Output.m_Resource & householdNeed.m_Resource) != Resource.NoResource)
						{
							ServiceAvailable serviceAvailable = m_ServiceAvailableData[renter];
							DynamicBuffer<Resources> resources = m_Resources[renter];
							if (math.min(EconomyUtils.GetResources(householdNeed.m_Resource, resources), serviceAvailable.m_ServiceAvailable) >= householdNeed.m_Amount)
							{
								SetDivert(jobIndex, entity, ref resident, ref currentLane, ref divert, ref pathOwner, Purpose.Shopping, renter, householdNeed.m_Amount, householdNeed.m_Resource);
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		private bool AddPathAction(Entity entity, ref Random random, ref HumanCurrentLane currentLane, ref PathOwner pathOwner, Entity actionTarget)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = m_TransformData[actionTarget];
			PrefabRef prefabRef = m_PrefabRefData[actionTarget];
			float3 val;
			if (m_PrefabObjectGeometryData.HasComponent(prefabRef.m_Prefab))
			{
				ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
				val = ObjectUtils.LocalToWorld(transform, ((Random)(ref random)).NextFloat3(objectGeometryData.m_Bounds.min, objectGeometryData.m_Bounds.max));
			}
			else
			{
				val = transform.m_Position + ((Random)(ref random)).NextFloat3(float3.op_Implicit(-10f), float3.op_Implicit(10f));
			}
			float num = float.MaxValue;
			float num2 = 0f;
			int num3 = -2;
			if (currentLane.m_CurvePosition.y != currentLane.m_CurvePosition.x && m_PedestrianLaneData.HasComponent(currentLane.m_Lane))
			{
				num = MathUtils.Distance(m_CurveData[currentLane.m_Lane].m_Bezier, val, currentLane.m_CurvePosition, ref num2);
				num3 = -1;
			}
			DynamicBuffer<PathElement> val2 = m_PathElements[entity];
			int num4 = math.min(val2.Length, pathOwner.m_ElementIndex + 8);
			float num6 = default(float);
			for (int i = pathOwner.m_ElementIndex; i < num4; i++)
			{
				PathElement pathElement = val2[i];
				if (pathElement.m_TargetDelta.y != pathElement.m_TargetDelta.x && m_PedestrianLaneData.HasComponent(pathElement.m_Target))
				{
					float num5 = MathUtils.Distance(m_CurveData[pathElement.m_Target].m_Bezier, val, pathElement.m_TargetDelta, ref num6);
					if (num5 < num)
					{
						num = num5;
						num2 = num6;
						num3 = i;
					}
				}
			}
			Entity target;
			float y;
			switch (num3)
			{
			case -2:
				return false;
			case -1:
				target = currentLane.m_Lane;
				y = currentLane.m_CurvePosition.y;
				currentLane.m_CurvePosition.y = num2;
				num3 = pathOwner.m_ElementIndex;
				break;
			default:
			{
				PathElement pathElement2 = val2[num3];
				target = pathElement2.m_Target;
				y = pathElement2.m_TargetDelta.y;
				pathElement2.m_TargetDelta.y = num2;
				val2[num3++] = pathElement2;
				break;
			}
			}
			val2.Insert(num3, new PathElement
			{
				m_Target = target,
				m_TargetDelta = new float2(num2, y)
			});
			val2.Insert(num3, new PathElement
			{
				m_Target = actionTarget,
				m_Flags = PathElementFlags.Action
			});
			return true;
		}

		private bool CheckPath(int jobIndex, Entity entity, PrefabRef prefabRef, ref Random random, ref Creature creature, ref Human human, ref HumanCurrentLane currentLane, ref Target target, ref Divert divert, ref PathOwner pathOwner, ref Game.Creatures.Resident resident)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_042e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			//IL_043f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0441: Unknown result type (might be due to invalid IL or missing references)
			//IL_0447: Unknown result type (might be due to invalid IL or missing references)
			//IL_044d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_0459: Unknown result type (might be due to invalid IL or missing references)
			//IL_045f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0465: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0471: Unknown result type (might be due to invalid IL or missing references)
			//IL_0477: Unknown result type (might be due to invalid IL or missing references)
			//IL_047d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0491: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04af: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<PathElement> path = m_PathElements[entity];
			human.m_Flags &= ~(HumanFlags.Selfies | HumanFlags.Carried);
			resident.m_Flags &= ~(ResidentFlags.WaitingTransport | ResidentFlags.NoLateDeparture);
			resident.m_Timer = 0;
			creature.m_QueueEntity = Entity.Null;
			creature.m_QueueArea = default(Sphere3);
			Purpose purpose = divert.m_Purpose;
			switch (purpose)
			{
			case Purpose.Shopping:
				if (purpose == Purpose.Shopping && divert.m_Data != 0 && m_HouseholdMembers.HasComponent(resident.m_Citizen))
				{
					m_ActionQueue.Enqueue(new ResidentAction
					{
						m_Type = ResidentActionType.GoShopping,
						m_Citizen = resident.m_Citizen,
						m_Household = m_HouseholdMembers[resident.m_Citizen].m_Household,
						m_Resource = divert.m_Resource,
						m_Target = divert.m_Target,
						m_Amount = divert.m_Data,
						m_Distance = 100f
					});
					divert.m_Data = 0;
				}
				break;
			case Purpose.None:
			{
				TravelPurpose travelPurpose = default(TravelPurpose);
				if (m_TravelPurposeData.TryGetComponent(resident.m_Citizen, ref travelPurpose) && travelPurpose.m_Purpose == Purpose.EmergencyShelter)
				{
					human.m_Flags |= HumanFlags.Run | HumanFlags.Emergency;
				}
				break;
			}
			case Purpose.Safety:
			case Purpose.Escape:
				human.m_Flags |= HumanFlags.Run;
				break;
			case Purpose.Disappear:
				if (divert.m_Target == Entity.Null && path.Length >= 1)
				{
					divert.m_Target = path[path.Length - 1].m_Target;
					path.RemoveAt(path.Length - 1);
				}
				break;
			}
			ParkedCar parkedCar = default(ParkedCar);
			CarKeeper carKeeper = default(CarKeeper);
			if (EntitiesExtensions.TryGetEnabledComponent<CarKeeper>(m_CarKeeperData, resident.m_Citizen, ref carKeeper))
			{
				m_ParkedCarData.TryGetComponent(carKeeper.m_Car, ref parkedCar);
			}
			int length = path.Length;
			SpawnLocationData spawnLocationData = default(SpawnLocationData);
			for (int i = 0; i < path.Length; i++)
			{
				PathElement pathElement = path[i];
				if (pathElement.m_Target == parkedCar.m_Lane)
				{
					VehicleUtils.SetParkingCurvePos(path, pathOwner, i, currentLane.m_Lane, parkedCar.m_CurvePosition, ref m_CurveData);
					length = i;
					break;
				}
				if (m_ParkingLaneData.HasComponent(pathElement.m_Target))
				{
					float curvePos = ((Random)(ref random)).NextFloat(0.05f, 0.95f);
					VehicleUtils.SetParkingCurvePos(path, pathOwner, i, currentLane.m_Lane, curvePos, ref m_CurveData);
					length = i;
					break;
				}
				if (m_ConnectionLaneData.HasComponent(pathElement.m_Target))
				{
					Game.Net.ConnectionLane connectionLane = m_ConnectionLaneData[pathElement.m_Target];
					if ((connectionLane.m_Flags & ConnectionLaneFlags.Parking) != 0)
					{
						float curvePos2 = ((Random)(ref random)).NextFloat(0.05f, 0.95f);
						VehicleUtils.SetParkingCurvePos(path, pathOwner, i, currentLane.m_Lane, curvePos2, ref m_CurveData);
						length = i;
						break;
					}
					if ((connectionLane.m_Flags & ConnectionLaneFlags.Area) != 0 && i == path.Length - 1)
					{
						CreatureUtils.SetRandomAreaTarget(ref random, i, path, m_OwnerData, m_CurveData, m_LaneData, m_ConnectionLaneData, m_SubLanes, m_AreaNodes, m_AreaTriangles);
						length = path.Length;
						break;
					}
				}
				else if (i == path.Length - 1 && m_SpawnLocation.HasComponent(pathElement.m_Target))
				{
					PrefabRef prefabRef2 = m_PrefabRefData[pathElement.m_Target];
					if (i != 0 && m_PrefabSpawnLocationData.TryGetComponent((Entity)prefabRef2, ref spawnLocationData) && spawnLocationData.m_HangaroundOnLane)
					{
						ref PathElement reference = ref path.ElementAt(i - 1);
						reference.m_TargetDelta.y = ((Random)(ref random)).NextFloat();
						reference.m_Flags |= PathElementFlags.Hangaround;
						path.RemoveAt(i);
						length = path.Length;
						break;
					}
				}
			}
			TransportEstimateBuffer transportEstimateBuffer = new TransportEstimateBuffer
			{
				m_BoardingQueue = m_BoardingQueue
			};
			RouteUtils.StripTransportSegments(ref random, length, path, m_RouteConnectedData, m_BoardingVehicleData, m_OwnerData, m_LaneData, m_ConnectionLaneData, m_CurveData, m_PrefabRefData, m_PrefabTransportStopData, m_SubLanes, m_AreaNodes, m_AreaTriangles, transportEstimateBuffer);
			if (m_OwnerData.HasComponent(currentLane.m_Lane))
			{
				Entity owner = m_OwnerData[currentLane.m_Lane].m_Owner;
				return FindDivertTargets(jobIndex, entity, prefabRef, ref random, ref human, ref currentLane, ref target, ref divert, ref pathOwner, ref resident, owner, Entity.Null);
			}
			return false;
		}

		private void CurrentVehicleBoarding(int jobIndex, Entity entity, Entity controllerVehicle, ref Game.Creatures.Resident resident, ref PathOwner pathOwner, ref Target target)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			Game.Vehicles.PublicTransport publicTransport = m_PublicTransportData[controllerVehicle];
			if ((publicTransport.m_State & (PublicTransportFlags.Evacuating | PublicTransportFlags.PrisonerTransport)) != 0)
			{
				if ((publicTransport.m_State & PublicTransportFlags.Returning) == 0)
				{
					return;
				}
			}
			else
			{
				DynamicBuffer<PathElement> val = m_PathElements[entity];
				if (val.Length >= pathOwner.m_ElementIndex + 2)
				{
					Entity target2 = val[pathOwner.m_ElementIndex + 1].m_Target;
					Entity nextLane = Entity.Null;
					if (val.Length >= pathOwner.m_ElementIndex + 3)
					{
						nextLane = val[pathOwner.m_ElementIndex + 2].m_Target;
					}
					if (!RouteUtils.ShouldExitVehicle(nextLane, target2, controllerVehicle, ref m_OwnerData, ref m_RouteConnectedData, ref m_BoardingVehicleData, ref m_CurrentRouteData, ref m_AccessLaneLaneData, ref m_PublicTransportData, ref m_ConnectedRoutes, testing: false, out var obsolete))
					{
						return;
					}
					pathOwner.m_ElementIndex += 2;
					if (!obsolete)
					{
						resident.m_Flags |= ResidentFlags.Disembarking;
						return;
					}
				}
			}
			pathOwner.m_State &= ~PathFlags.Failed;
			pathOwner.m_State |= PathFlags.Obsolete;
			resident.m_Flags |= ResidentFlags.Disembarking;
		}

		private void CurrentVehicleTesting(int jobIndex, Entity entity, Entity controllerVehicle, ref Game.Creatures.Resident resident, ref PathOwner pathOwner, ref Target target)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			Game.Vehicles.PublicTransport publicTransport = m_PublicTransportData[controllerVehicle];
			if ((publicTransport.m_State & (PublicTransportFlags.Evacuating | PublicTransportFlags.PrisonerTransport)) != 0)
			{
				if ((publicTransport.m_State & PublicTransportFlags.Returning) == 0)
				{
					return;
				}
			}
			else
			{
				DynamicBuffer<PathElement> val = m_PathElements[entity];
				if (val.Length >= pathOwner.m_ElementIndex + 2)
				{
					Entity target2 = val[pathOwner.m_ElementIndex + 1].m_Target;
					Entity nextLane = Entity.Null;
					if (val.Length >= pathOwner.m_ElementIndex + 3)
					{
						nextLane = val[pathOwner.m_ElementIndex + 2].m_Target;
					}
					if (!RouteUtils.ShouldExitVehicle(nextLane, target2, controllerVehicle, ref m_OwnerData, ref m_RouteConnectedData, ref m_BoardingVehicleData, ref m_CurrentRouteData, ref m_AccessLaneLaneData, ref m_PublicTransportData, ref m_ConnectedRoutes, testing: true, out var _))
					{
						return;
					}
				}
			}
			m_BoardingQueue.Enqueue(Boarding.RequireStop(Entity.Null, controllerVehicle, default(float3)));
		}

		private void CurrentVehicleDisembarking(int jobIndex, Entity entity, Entity controllerVehicle, ref Game.Creatures.Resident resident, ref PathOwner pathOwner, ref Target target)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<PathElement> targetElements = m_PathElements[entity];
			DynamicBuffer<PathElement> sourceElements = m_PathElements[controllerVehicle];
			PathOwner sourceOwner = m_PathOwnerData[controllerVehicle];
			if (sourceElements.Length > sourceOwner.m_ElementIndex)
			{
				PathUtils.CopyPath(sourceElements, sourceOwner, 0, targetElements);
				pathOwner.m_ElementIndex = 0;
				pathOwner.m_State |= PathFlags.Updated;
			}
			else
			{
				pathOwner.m_State &= ~PathFlags.Failed;
				pathOwner.m_State |= PathFlags.Obsolete;
			}
			resident.m_Flags |= ResidentFlags.Disembarking;
		}

		private void CurrentVehicleTransporting(Entity entity, Entity controllerVehicle, ref PathOwner pathOwner)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			m_PathElements[entity].Clear();
			pathOwner.m_ElementIndex = 0;
		}

		private void GroupLeaderDisembarking(Entity entity, ref Game.Creatures.Resident resident, ref PathOwner pathOwner)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			m_PathElements[entity].Clear();
			pathOwner.m_ElementIndex = 0;
			resident.m_Flags |= ResidentFlags.Disembarking;
		}

		private bool PathEndReached(int jobIndex, Entity entity, ref Random random, ref Game.Creatures.Resident resident, ref Human human, ref HumanCurrentLane currentLane, ref Target target, ref Divert divert, ref PathOwner pathOwner)
		{
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			return divert.m_Purpose switch
			{
				Purpose.None => ReachTarget(jobIndex, entity, ref random, ref resident, ref human, ref currentLane, ref target, ref divert, ref pathOwner, new ResetTrip
				{
					m_Target = target.m_Target
				}), 
				Purpose.SendMail => ReachSendMail(jobIndex, entity, ref resident, ref currentLane, ref target, ref divert, ref pathOwner), 
				Purpose.Safety => ReachSafety(jobIndex, entity, ref random, ref resident, ref human, ref currentLane, ref target, ref divert, ref pathOwner), 
				Purpose.Escape => ReachEscape(jobIndex, entity, ref resident, ref currentLane, ref target, ref divert, ref pathOwner), 
				Purpose.WaitingHome => ReachWaitingHome(jobIndex, entity, ref random, ref resident, ref currentLane, ref target, ref divert, ref pathOwner), 
				Purpose.PathFailed => ReachPathFailed(jobIndex, entity, ref random, ref resident, ref currentLane, ref target, ref divert, ref pathOwner), 
				_ => ReachDivert(jobIndex, entity, ref random, ref resident, ref human, ref currentLane, ref target, ref divert, ref pathOwner), 
			};
		}

		private bool ActionLocationReached(Entity entity, ref Game.Creatures.Resident resident, ref Human human, ref HumanCurrentLane currentLane, ref PathOwner pathOwner)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			if ((currentLane.m_Flags & CreatureLaneFlags.ActivityDone) != 0)
			{
				resident.m_Flags |= ResidentFlags.ActivityDone;
				human.m_Flags &= ~HumanFlags.Selfies;
				DynamicBuffer<PathElement> val = m_PathElements[entity];
				pathOwner.m_ElementIndex += math.select(0, 1, pathOwner.m_ElementIndex < val.Length);
				return false;
			}
			return true;
		}

		private bool ReachTarget(int jobIndex, Entity creatureEntity, ref Random random, ref Game.Creatures.Resident resident, ref Human human, ref HumanCurrentLane currentLane, ref Target target, ref Divert divert, ref PathOwner pathOwner, ResetTrip resetTrip)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			if (m_VehicleData.HasComponent(target.m_Target))
			{
				return ReachVehicle(jobIndex, creatureEntity, ref resident, ref currentLane, ref target, ref pathOwner);
			}
			Entity val = target.m_Target;
			if (m_PropertyRenters.HasComponent(val))
			{
				val = m_PropertyRenters[val].m_Property;
			}
			if (m_OnFireData.HasComponent(val) || m_DestroyedData.HasComponent(val))
			{
				SetDivert(jobIndex, creatureEntity, ref resident, ref currentLane, ref divert, ref pathOwner, Purpose.Safety, Entity.Null, 0, Resource.NoResource);
				return false;
			}
			if ((currentLane.m_Flags & CreatureLaneFlags.Hangaround) != 0)
			{
				if ((currentLane.m_Flags & (CreatureLaneFlags.TransformTarget | CreatureLaneFlags.ActivityDone)) == (CreatureLaneFlags.TransformTarget | CreatureLaneFlags.ActivityDone) || ((Random)(ref random)).NextInt(2500) == 0)
				{
					ResidentFlags ignoreFlags = GetIgnoreFlags(val, ref resident, ref currentLane);
					if (ignoreFlags != ResidentFlags.None)
					{
						if ((ignoreFlags & ~resident.m_Flags) != ResidentFlags.None)
						{
							resident.m_Flags |= ignoreFlags;
							pathOwner.m_State &= ~PathFlags.Failed;
							pathOwner.m_State |= PathFlags.Obsolete;
							resident.m_Flags &= ~ResidentFlags.Hangaround;
							if ((resident.m_Flags & ResidentFlags.Arrived) != ResidentFlags.None)
							{
								resetTrip.m_Source = target.m_Target;
								resetTrip.m_Target = target.m_Target;
								bool flag = false;
								HouseholdMember householdMember = default(HouseholdMember);
								DynamicBuffer<HouseholdAnimal> val2 = default(DynamicBuffer<HouseholdAnimal>);
								if (m_HouseholdMembers.TryGetComponent(resident.m_Citizen, ref householdMember) && m_HouseholdAnimals.TryGetBuffer(householdMember.m_Household, ref val2))
								{
									CurrentBuilding currentBuilding = default(CurrentBuilding);
									CurrentTransport currentTransport = default(CurrentTransport);
									for (int i = 0; i < val2.Length; i++)
									{
										HouseholdAnimal householdAnimal = val2[i];
										if (m_CurrentBuildingData.TryGetComponent(householdAnimal.m_HouseholdPet, ref currentBuilding) && m_CurrentTransportData.TryGetComponent(householdAnimal.m_HouseholdPet, ref currentTransport) && currentBuilding.m_CurrentBuilding == val)
										{
											Entity val3 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_ResetTripArchetype);
											resetTrip.m_Creature = currentTransport.m_CurrentTransport;
											((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ResetTrip>(jobIndex, val3, resetTrip);
											flag = true;
										}
									}
								}
								if (flag)
								{
									Entity val4 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_ResetTripArchetype);
									resetTrip.m_Creature = creatureEntity;
									((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ResetTrip>(jobIndex, val4, resetTrip);
								}
							}
						}
						return false;
					}
				}
				resident.m_Flags |= ResidentFlags.Hangaround;
			}
			human.m_Flags &= ~(HumanFlags.Run | HumanFlags.Emergency);
			resident.m_Flags &= ~(ResidentFlags.IgnoreTaxi | ResidentFlags.IgnoreTransport);
			if ((resident.m_Flags & ResidentFlags.Arrived) == 0)
			{
				resetTrip.m_Creature = creatureEntity;
				resetTrip.m_Arrived = val;
				Entity val5 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_ResetTripArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ResetTrip>(jobIndex, val5, resetTrip);
				resident.m_Flags |= ResidentFlags.Arrived;
				return false;
			}
			if ((resident.m_Flags & ResidentFlags.Hangaround) != ResidentFlags.None)
			{
				return false;
			}
			if (((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(resident.m_Citizen))
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<CurrentTransport>(jobIndex, resident.m_Citizen);
			}
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, creatureEntity, default(Deleted));
			return true;
		}

		private ResidentFlags GetIgnoreFlags(Entity building, ref Game.Creatures.Resident resident, ref HumanCurrentLane currentLane)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			if ((resident.m_Flags & ResidentFlags.CannotIgnore) != ResidentFlags.None)
			{
				return ResidentFlags.None;
			}
			ResidentFlags residentFlags = ResidentFlags.None;
			Owner owner = default(Owner);
			PrefabRef prefabRef2 = default(PrefabRef);
			SpawnLocationData spawnLocationData2 = default(SpawnLocationData);
			if ((currentLane.m_Flags & CreatureLaneFlags.TransformTarget) != 0)
			{
				PrefabRef prefabRef = default(PrefabRef);
				SpawnLocationData spawnLocationData = default(SpawnLocationData);
				if (m_PrefabRefData.TryGetComponent(currentLane.m_Lane, ref prefabRef) && m_PrefabSpawnLocationData.TryGetComponent(prefabRef.m_Prefab, ref spawnLocationData) && ((spawnLocationData.m_ActivityMask.m_Mask & new ActivityMask(ActivityType.BenchSitting).m_Mask) != 0 || (spawnLocationData.m_ActivityMask.m_Mask & new ActivityMask(ActivityType.PullUps).m_Mask) != 0))
				{
					residentFlags |= ResidentFlags.IgnoreBenches;
				}
			}
			else if ((currentLane.m_Flags & CreatureLaneFlags.Area) != 0 && m_OwnerData.TryGetComponent(currentLane.m_Lane, ref owner) && m_PrefabRefData.TryGetComponent(owner.m_Owner, ref prefabRef2) && m_PrefabSpawnLocationData.TryGetComponent(prefabRef2.m_Prefab, ref spawnLocationData2) && ((spawnLocationData2.m_ActivityMask.m_Mask & new ActivityMask(ActivityType.Standing).m_Mask) != 0 || (spawnLocationData2.m_ActivityMask.m_Mask & new ActivityMask(ActivityType.GroundLaying).m_Mask) != 0 || (spawnLocationData2.m_ActivityMask.m_Mask & new ActivityMask(ActivityType.GroundSitting).m_Mask) != 0 || (spawnLocationData2.m_ActivityMask.m_Mask & new ActivityMask(ActivityType.PushUps).m_Mask) != 0 || (spawnLocationData2.m_ActivityMask.m_Mask & new ActivityMask(ActivityType.SitUps).m_Mask) != 0 || (spawnLocationData2.m_ActivityMask.m_Mask & new ActivityMask(ActivityType.JumpingJacks).m_Mask) != 0 || (spawnLocationData2.m_ActivityMask.m_Mask & new ActivityMask(ActivityType.JumpingLunges).m_Mask) != 0 || (spawnLocationData2.m_ActivityMask.m_Mask & new ActivityMask(ActivityType.Squats).m_Mask) != 0 || (spawnLocationData2.m_ActivityMask.m_Mask & new ActivityMask(ActivityType.Yoga).m_Mask) != 0))
			{
				residentFlags |= ResidentFlags.IgnoreAreas;
			}
			if ((residentFlags & ~resident.m_Flags) != ResidentFlags.None)
			{
				ResidentFlags residentFlags2 = ~(resident.m_Flags | residentFlags);
				bool flag = false;
				DynamicBuffer<SpawnLocationElement> val = default(DynamicBuffer<SpawnLocationElement>);
				if (m_SpawnLocationElements.TryGetBuffer(building, ref val))
				{
					for (int i = 0; i < val.Length; i++)
					{
						SpawnLocationElement spawnLocationElement = val[i];
						if (spawnLocationElement.m_Type == SpawnLocationType.SpawnLocation || spawnLocationElement.m_Type == SpawnLocationType.HangaroundLocation)
						{
							PrefabRef prefabRef3 = m_PrefabRefData[spawnLocationElement.m_SpawnLocation];
							SpawnLocationData spawnLocationData3 = m_PrefabSpawnLocationData[prefabRef3.m_Prefab];
							ResidentFlags residentFlags3 = ResidentFlags.None;
							if ((spawnLocationData3.m_ActivityMask.m_Mask & new ActivityMask(ActivityType.BenchSitting).m_Mask) != 0 || (spawnLocationData3.m_ActivityMask.m_Mask & new ActivityMask(ActivityType.PullUps).m_Mask) != 0)
							{
								residentFlags3 |= ResidentFlags.IgnoreBenches;
							}
							if ((spawnLocationData3.m_ActivityMask.m_Mask & new ActivityMask(ActivityType.Standing).m_Mask) != 0 || (spawnLocationData3.m_ActivityMask.m_Mask & new ActivityMask(ActivityType.GroundLaying).m_Mask) != 0 || (spawnLocationData3.m_ActivityMask.m_Mask & new ActivityMask(ActivityType.GroundSitting).m_Mask) != 0 || (spawnLocationData3.m_ActivityMask.m_Mask & new ActivityMask(ActivityType.PushUps).m_Mask) != 0 || (spawnLocationData3.m_ActivityMask.m_Mask & new ActivityMask(ActivityType.SitUps).m_Mask) != 0 || (spawnLocationData3.m_ActivityMask.m_Mask & new ActivityMask(ActivityType.JumpingJacks).m_Mask) != 0 || (spawnLocationData3.m_ActivityMask.m_Mask & new ActivityMask(ActivityType.JumpingLunges).m_Mask) != 0 || (spawnLocationData3.m_ActivityMask.m_Mask & new ActivityMask(ActivityType.Squats).m_Mask) != 0 || (spawnLocationData3.m_ActivityMask.m_Mask & new ActivityMask(ActivityType.Yoga).m_Mask) != 0)
							{
								residentFlags3 |= ResidentFlags.IgnoreAreas;
							}
							if (residentFlags3 == ResidentFlags.None || (residentFlags3 & residentFlags2) != ResidentFlags.None)
							{
								return residentFlags;
							}
							flag = flag || (residentFlags3 & ~residentFlags) != 0;
						}
					}
				}
				if (!flag)
				{
					resident.m_Flags |= ResidentFlags.CannotIgnore;
					return ResidentFlags.None;
				}
				resident.m_Flags &= (ResidentFlags)(0xFFFFF3FFu | (uint)residentFlags);
			}
			return residentFlags;
		}

		private bool ReachVehicle(int jobIndex, Entity entity, ref Game.Creatures.Resident resident, ref HumanCurrentLane currentLane, ref Target target, ref PathOwner pathOwner)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			Entity val = target.m_Target;
			if (m_ControllerData.HasComponent(target.m_Target))
			{
				Controller controller = m_ControllerData[target.m_Target];
				if (controller.m_Controller != Entity.Null)
				{
					val = controller.m_Controller;
				}
			}
			if (m_PublicTransportData.HasComponent(val))
			{
				if ((m_PublicTransportData[val].m_State & PublicTransportFlags.Boarding) != 0 && m_OwnerData.HasComponent(val))
				{
					Owner owner = m_OwnerData[val];
					if (m_PrefabRefData.HasComponent(owner.m_Owner))
					{
						TryEnterVehicle(entity, target.m_Target, Entity.Null, ref resident, ref currentLane);
						target.m_Target = owner.m_Owner;
						pathOwner.m_State &= ~PathFlags.Failed;
						pathOwner.m_State |= PathFlags.Obsolete;
						return true;
					}
				}
			}
			else if (m_PoliceCarData.HasComponent(val))
			{
				if ((m_PoliceCarData[val].m_State & PoliceCarFlags.AtTarget) != 0 && m_OwnerData.HasComponent(val))
				{
					Owner owner2 = m_OwnerData[val];
					if (m_PrefabRefData.HasComponent(owner2.m_Owner))
					{
						TryEnterVehicle(entity, target.m_Target, Entity.Null, ref resident, ref currentLane);
						target.m_Target = owner2.m_Owner;
						pathOwner.m_State &= ~PathFlags.Failed;
						pathOwner.m_State |= PathFlags.Obsolete;
						return true;
					}
				}
			}
			else if (m_AmbulanceData.HasComponent(val))
			{
				if ((m_AmbulanceData[val].m_State & AmbulanceFlags.AtTarget) != 0 && m_OwnerData.HasComponent(val))
				{
					Owner owner3 = m_OwnerData[val];
					if (m_PrefabRefData.HasComponent(owner3.m_Owner))
					{
						TryEnterVehicle(entity, target.m_Target, Entity.Null, ref resident, ref currentLane);
						target.m_Target = owner3.m_Owner;
						pathOwner.m_State &= ~PathFlags.Failed;
						pathOwner.m_State |= PathFlags.Obsolete;
						return true;
					}
				}
			}
			else if (m_HearseData.HasComponent(val) && (m_HearseData[val].m_State & HearseFlags.AtTarget) != 0 && m_OwnerData.HasComponent(val))
			{
				Owner owner4 = m_OwnerData[val];
				if (m_PrefabRefData.HasComponent(owner4.m_Owner))
				{
					TryEnterVehicle(entity, target.m_Target, Entity.Null, ref resident, ref currentLane);
					target.m_Target = owner4.m_Owner;
					pathOwner.m_State &= ~PathFlags.Failed;
					pathOwner.m_State |= PathFlags.Obsolete;
					return true;
				}
			}
			AddDeletedResident(DeletedResidentType.InvalidVehicleTarget);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, entity, default(Deleted));
			return true;
		}

		private bool ReachDivert(int jobIndex, Entity entity, ref Random random, ref Game.Creatures.Resident resident, ref Human human, ref HumanCurrentLane currentLane, ref Target target, ref Divert divert, ref PathOwner pathOwner)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			ResetTrip resetTrip = new ResetTrip
			{
				m_Target = divert.m_Target,
				m_TravelPurpose = divert.m_Purpose,
				m_TravelData = divert.m_Data,
				m_TravelResource = divert.m_Resource
			};
			if (m_TravelPurposeData.HasComponent(resident.m_Citizen) && m_PrefabRefData.HasComponent(target.m_Target))
			{
				TravelPurpose travelPurpose = m_TravelPurposeData[resident.m_Citizen];
				resetTrip.m_NextTarget = target.m_Target;
				resetTrip.m_NextPurpose = travelPurpose.m_Purpose;
				resetTrip.m_NextData = travelPurpose.m_Data;
				resetTrip.m_NextResource = travelPurpose.m_Resource;
			}
			target.m_Target = divert.m_Target;
			divert = default(Divert);
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Divert>(jobIndex, entity);
			pathOwner.m_State &= ~PathFlags.CachedObsolete;
			return ReachTarget(jobIndex, entity, ref random, ref resident, ref human, ref currentLane, ref target, ref divert, ref pathOwner, resetTrip);
		}

		private bool ReachSendMail(int jobIndex, Entity entity, ref Game.Creatures.Resident resident, ref HumanCurrentLane currentLane, ref Target target, ref Divert divert, ref PathOwner pathOwner)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			m_ActionQueue.Enqueue(new ResidentAction
			{
				m_Type = ResidentActionType.SendMail,
				m_Citizen = resident.m_Citizen,
				m_Target = currentLane.m_Lane
			});
			SetDivert(jobIndex, entity, ref resident, ref currentLane, ref divert, ref pathOwner, Purpose.None, Entity.Null, 0, Resource.NoResource);
			return false;
		}

		private bool ReachSafety(int jobIndex, Entity entity, ref Random random, ref Game.Creatures.Resident resident, ref Human human, ref HumanCurrentLane currentLane, ref Target target, ref Divert divert, ref PathOwner pathOwner)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			human.m_Flags &= ~(HumanFlags.Run | HumanFlags.Emergency);
			if (!m_PrefabRefData.HasComponent(target.m_Target) || m_DestroyedData.HasComponent(target.m_Target))
			{
				bool movingAway;
				Entity homeBuilding = GetHomeBuilding(ref resident, out movingAway);
				if (homeBuilding != Entity.Null && !m_DestroyedData.HasComponent(homeBuilding))
				{
					if (m_OnFireData.HasComponent(homeBuilding))
					{
						FindWaitingPosition(jobIndex, entity, ref random, ref resident, ref currentLane, ref target, ref divert, ref pathOwner);
						return false;
					}
					SetTarget(jobIndex, entity, ref resident, ref currentLane, ref divert, ref pathOwner, ref target, Purpose.GoingHome, homeBuilding);
					return false;
				}
				if (movingAway)
				{
					target.m_Target = Entity.Null;
					SetDivert(jobIndex, entity, ref resident, ref currentLane, ref divert, ref pathOwner, Purpose.Disappear, Entity.Null, 0, Resource.NoResource);
					return false;
				}
				SetDivert(jobIndex, entity, ref resident, ref currentLane, ref divert, ref pathOwner, Purpose.WaitingHome, Entity.Null, 0, Resource.NoResource);
				return false;
			}
			if (m_OnFireData.HasComponent(target.m_Target))
			{
				FindWaitingPosition(jobIndex, entity, ref random, ref resident, ref currentLane, ref target, ref divert, ref pathOwner);
				return false;
			}
			Purpose purpose = Purpose.None;
			if (m_TravelPurposeData.HasComponent(resident.m_Citizen))
			{
				purpose = m_TravelPurposeData[resident.m_Citizen].m_Purpose;
			}
			SetTarget(jobIndex, entity, ref resident, ref currentLane, ref divert, ref pathOwner, ref target, purpose, target.m_Target);
			return false;
		}

		private bool ReachEscape(int jobIndex, Entity entity, ref Game.Creatures.Resident resident, ref HumanCurrentLane currentLane, ref Target target, ref Divert divert, ref PathOwner pathOwner)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			bool movingAway;
			Entity homeBuilding = GetHomeBuilding(ref resident, out movingAway);
			if (homeBuilding == Entity.Null || m_OnFireData.HasComponent(homeBuilding) || m_DestroyedData.HasComponent(homeBuilding))
			{
				SetDivert(jobIndex, entity, ref resident, ref currentLane, ref divert, ref pathOwner, Purpose.Disappear, Entity.Null, 0, Resource.NoResource);
				return false;
			}
			SetTarget(jobIndex, entity, ref resident, ref currentLane, ref divert, ref pathOwner, ref target, Purpose.GoingHome, homeBuilding);
			return false;
		}

		private bool ReachWaitingHome(int jobIndex, Entity entity, ref Random random, ref Game.Creatures.Resident resident, ref HumanCurrentLane currentLane, ref Target target, ref Divert divert, ref PathOwner pathOwner)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			bool movingAway;
			Entity homeBuilding = GetHomeBuilding(ref resident, out movingAway);
			if (homeBuilding == Entity.Null || m_DestroyedData.HasComponent(homeBuilding))
			{
				if (movingAway)
				{
					SetDivert(jobIndex, entity, ref resident, ref currentLane, ref divert, ref pathOwner, Purpose.Disappear, Entity.Null, 0, Resource.NoResource);
					return false;
				}
				divert.m_Data += ((Random)(ref random)).NextInt(1, 3);
				bool flag = divert.m_Data <= 2500;
				if (!flag)
				{
					Game.Net.ConnectionLane connectionLane = default(Game.Net.ConnectionLane);
					flag = (currentLane.m_Flags & CreatureLaneFlags.Connection) == 0 || !m_ConnectionLaneData.TryGetComponent(currentLane.m_Lane, ref connectionLane) || (connectionLane.m_Flags & ConnectionLaneFlags.Outside) == 0;
				}
				if (flag)
				{
					FindWaitingPosition(jobIndex, entity, ref random, ref resident, ref currentLane, ref target, ref divert, ref pathOwner);
					return false;
				}
				if (m_CitizenData.HasComponent(resident.m_Citizen))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, resident.m_Citizen, default(Deleted));
				}
				AddDeletedResident(DeletedResidentType.WaitingHome_AlreadyOutside);
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, entity, default(Deleted));
				return true;
			}
			SetTarget(jobIndex, entity, ref resident, ref currentLane, ref divert, ref pathOwner, ref target, Purpose.GoingHome, homeBuilding);
			return false;
		}

		private bool ReachPathFailed(int jobIndex, Entity entity, ref Random random, ref Game.Creatures.Resident resident, ref HumanCurrentLane currentLane, ref Target target, ref Divert divert, ref PathOwner pathOwner)
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			divert.m_Data += ((Random)(ref random)).NextInt(1, 3);
			if (divert.m_Data <= 2500)
			{
				FindWaitingPosition(jobIndex, entity, ref random, ref resident, ref currentLane, ref target, ref divert, ref pathOwner);
				return false;
			}
			SetDivert(jobIndex, entity, ref resident, ref currentLane, ref divert, ref pathOwner, Purpose.None, Entity.Null, 0, Resource.NoResource);
			return false;
		}

		private bool ReturnHome(int jobIndex, Entity entity, ref Random random, ref Game.Creatures.Resident resident, ref HumanCurrentLane currentLane, ref Target target, ref Divert divert, ref PathOwner pathOwner)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			if (m_AttendingMeetingData.HasComponent(resident.m_Citizen))
			{
				AttendingMeeting attendingMeeting = m_AttendingMeetingData[resident.m_Citizen];
				if (m_CoordinatedMeetingData.HasComponent(attendingMeeting.m_Meeting) && m_CoordinatedMeetingData[attendingMeeting.m_Meeting].m_Target == target.m_Target)
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<AttendingMeeting>(jobIndex, resident.m_Citizen);
				}
			}
			bool movingAway;
			Entity homeBuilding = GetHomeBuilding(ref resident, out movingAway);
			if (homeBuilding != Entity.Null && homeBuilding != target.m_Target)
			{
				SetTarget(jobIndex, entity, ref resident, ref currentLane, ref divert, ref pathOwner, ref target, Purpose.GoingHome, homeBuilding);
				return false;
			}
			if (homeBuilding == Entity.Null && currentLane.m_Lane != Entity.Null)
			{
				if (movingAway)
				{
					AddDeletedResident(DeletedResidentType.NoPath_AlreadyMovingAway);
					if (m_CitizenData.HasComponent(resident.m_Citizen))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, resident.m_Citizen, default(Deleted));
					}
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, entity, default(Deleted));
					return true;
				}
				SetDivert(jobIndex, entity, ref resident, ref currentLane, ref divert, ref pathOwner, Purpose.WaitingHome, Entity.Null, 0, Resource.NoResource);
				return false;
			}
			Game.Net.ConnectionLane connectionLane = default(Game.Net.ConnectionLane);
			if ((currentLane.m_Flags & CreatureLaneFlags.Connection) != 0 && m_ConnectionLaneData.TryGetComponent(currentLane.m_Lane, ref connectionLane) && (connectionLane.m_Flags & ConnectionLaneFlags.Outside) != 0)
			{
				if (m_CitizenData.HasComponent(resident.m_Citizen))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, resident.m_Citizen, default(Deleted));
				}
				AddDeletedResident(DeletedResidentType.NoPathToHome_AlreadyOutside);
			}
			else
			{
				AddDeletedResident(DeletedResidentType.NoPathToHome);
			}
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, entity, default(Deleted));
			return true;
		}

		private void FindWaitingPosition(int jobIndex, Entity entity, ref Random random, ref Game.Creatures.Resident resident, ref HumanCurrentLane currentLane, ref Target target, ref Divert divert, ref PathOwner pathOwner)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			if (m_PedestrianLaneData.HasComponent(currentLane.m_Lane))
			{
				if ((m_PedestrianLaneData[currentLane.m_Lane].m_Flags & PedestrianLaneFlags.Crosswalk) == 0)
				{
					return;
				}
				pathOwner.m_ElementIndex = 0;
				DynamicBuffer<PathElement> val = m_PathElements[entity];
				val.Clear();
				NativeParallelHashSet<Entity> ignoreLanes = default(NativeParallelHashSet<Entity>);
				ignoreLanes._002Ector(16, AllocatorHandle.op_Implicit((Allocator)2));
				ignoreLanes.Add(currentLane.m_Lane);
				Entity lane = currentLane.m_Lane;
				float2 yy = ((float2)(ref currentLane.m_CurvePosition)).yy;
				if (yy.y >= 0.5f)
				{
					if (yy.y != 1f)
					{
						yy.y = 1f;
						val.Add(new PathElement(currentLane.m_Lane, yy));
					}
				}
				else if (yy.y != 0f)
				{
					yy.y = 0f;
					val.Add(new PathElement(currentLane.m_Lane, yy));
				}
				while (TryFindNextLane(ignoreLanes, ref lane, ref yy.y))
				{
					ignoreLanes.Add(lane);
					yy.x = yy.y;
					if ((m_PedestrianLaneData[lane].m_Flags & PedestrianLaneFlags.Crosswalk) == 0)
					{
						yy.y = ((Random)(ref random)).NextFloat(0f, 1f);
						val.Add(new PathElement(lane, yy));
						break;
					}
					yy.y = math.select(0f, 1f, yy.x < 0.5f);
					val.Add(new PathElement(lane, yy));
				}
				ignoreLanes.Dispose();
				if (val.Length != 0)
				{
					currentLane.m_Flags &= ~(CreatureLaneFlags.EndOfPath | CreatureLaneFlags.ParkingSpace | CreatureLaneFlags.Transport | CreatureLaneFlags.Taxi | CreatureLaneFlags.Action);
				}
			}
			else if (m_ConnectionLaneData.HasComponent(currentLane.m_Lane) && (currentLane.m_Flags & CreatureLaneFlags.WaitPosition) == 0 && (m_ConnectionLaneData[currentLane.m_Lane].m_Flags & ConnectionLaneFlags.Outside) != 0)
			{
				currentLane.m_Flags &= ~CreatureLaneFlags.EndReached;
				currentLane.m_Flags |= CreatureLaneFlags.WaitPosition;
				currentLane.m_CurvePosition.y = ((Random)(ref random)).NextFloat(0f, 1f);
			}
		}

		private bool TryFindNextLane(NativeParallelHashSet<Entity> ignoreLanes, ref Entity lane, ref float curveDelta)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			if (!m_OwnerData.HasComponent(lane))
			{
				return false;
			}
			Owner owner = m_OwnerData[lane];
			if (TryFindNextLane(ignoreLanes, owner.m_Owner, ref lane, ref curveDelta))
			{
				return true;
			}
			if (m_ConnectedEdges.HasBuffer(owner.m_Owner))
			{
				DynamicBuffer<ConnectedEdge> val = m_ConnectedEdges[owner.m_Owner];
				for (int i = 0; i < val.Length; i++)
				{
					if (TryFindNextLane(ignoreLanes, val[i].m_Edge, ref lane, ref curveDelta))
					{
						return true;
					}
				}
			}
			else if (m_EdgeData.HasComponent(owner.m_Owner))
			{
				Game.Net.Edge edge = m_EdgeData[owner.m_Owner];
				if (TryFindNextLane(ignoreLanes, edge.m_Start, ref lane, ref curveDelta))
				{
					return true;
				}
				if (TryFindNextLane(ignoreLanes, edge.m_End, ref lane, ref curveDelta))
				{
					return true;
				}
			}
			return false;
		}

		private bool TryFindNextLane(NativeParallelHashSet<Entity> ignoreLanes, Entity owner, ref Entity lane, ref float curveDelta)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			if (!m_SubLanes.HasBuffer(owner))
			{
				return false;
			}
			DynamicBuffer<Game.Net.SubLane> val = m_SubLanes[owner];
			Lane lane2 = m_LaneData[lane];
			PathNode other = ((curveDelta == 0f) ? lane2.m_StartNode : ((curveDelta != 1f) ? lane2.m_MiddleNode : lane2.m_EndNode));
			for (int i = 0; i < val.Length; i++)
			{
				Entity subLane = val[i].m_SubLane;
				if (!ignoreLanes.Contains(subLane) && m_PedestrianLaneData.HasComponent(subLane))
				{
					Lane lane3 = m_LaneData[subLane];
					if (lane3.m_StartNode.EqualsIgnoreCurvePos(other))
					{
						lane = subLane;
						curveDelta = 0f;
						return true;
					}
					if (lane3.m_EndNode.EqualsIgnoreCurvePos(other))
					{
						lane = subLane;
						curveDelta = 1f;
						return true;
					}
					if (lane3.m_MiddleNode.EqualsIgnoreCurvePos(other))
					{
						lane = subLane;
						curveDelta = lane3.m_MiddleNode.GetCurvePos();
						return true;
					}
				}
			}
			return false;
		}

		private void SetDivert(int jobIndex, Entity entity, ref Game.Creatures.Resident resident, ref HumanCurrentLane currentLane, ref Divert divert, ref PathOwner pathOwner, Purpose purpose, Entity targetEntity, int data = 0, Resource resource = Resource.NoResource)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			if (purpose != Purpose.None)
			{
				bool num = divert.m_Purpose == Purpose.None;
				divert = new Divert
				{
					m_Purpose = purpose,
					m_Target = targetEntity,
					m_Data = data,
					m_Resource = resource
				};
				if (num)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Divert>(jobIndex, entity, divert);
				}
				pathOwner.m_State |= PathFlags.DivertObsolete;
			}
			else if (divert.m_Purpose != Purpose.None)
			{
				divert = default(Divert);
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Divert>(jobIndex, entity);
				if ((pathOwner.m_State & PathFlags.CachedObsolete) != 0)
				{
					pathOwner.m_State &= ~PathFlags.CachedObsolete;
					pathOwner.m_State |= PathFlags.Obsolete;
				}
			}
			if ((resident.m_Flags & ResidentFlags.Arrived) != ResidentFlags.None && m_PrefabRefData.HasComponent(resident.m_Citizen))
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<CurrentBuilding>(jobIndex, resident.m_Citizen);
			}
			currentLane.m_Flags &= ~CreatureLaneFlags.EndOfPath;
			resident.m_Flags &= ~(ResidentFlags.Arrived | ResidentFlags.Hangaround | ResidentFlags.IgnoreBenches | ResidentFlags.IgnoreAreas | ResidentFlags.CannotIgnore);
			pathOwner.m_State &= ~PathFlags.Failed;
		}

		private void SetTarget(int jobIndex, Entity entity, ref Game.Creatures.Resident resident, ref HumanCurrentLane currentLane, ref Divert divert, ref PathOwner pathOwner, ref Target target, Purpose purpose, Entity targetEntity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			Entity source = Entity.Null;
			if ((resident.m_Flags & ResidentFlags.Arrived) != ResidentFlags.None)
			{
				if (m_PrefabRefData.HasComponent(resident.m_Citizen))
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<CurrentBuilding>(jobIndex, resident.m_Citizen);
				}
				source = target.m_Target;
			}
			Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_ResetTripArchetype);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ResetTrip>(jobIndex, val, new ResetTrip
			{
				m_Creature = entity,
				m_Source = source,
				m_Target = targetEntity,
				m_TravelPurpose = purpose
			});
		}

		private bool ParkingSpaceReached(int jobIndex, ref Random random, Entity entity, ref Game.Creatures.Resident resident, ref HumanCurrentLane currentLane, ref PathOwner pathOwner, ref Target target, DynamicBuffer<GroupCreature> groupCreatures)
		{
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			if ((currentLane.m_Flags & CreatureLaneFlags.Taxi) != 0)
			{
				if (m_RideNeederData.HasComponent(entity))
				{
					RideNeeder rideNeeder = m_RideNeederData[entity];
					if (m_Dispatched.HasComponent(rideNeeder.m_RideRequest))
					{
						Dispatched dispatched = m_Dispatched[rideNeeder.m_RideRequest];
						if (m_TaxiData.HasComponent(dispatched.m_Handler))
						{
							Game.Vehicles.Taxi taxi = m_TaxiData[dispatched.m_Handler];
							DynamicBuffer<ServiceDispatch> val = m_ServiceDispatches[dispatched.m_Handler];
							if ((taxi.m_State & TaxiFlags.Dispatched) != 0 && val.Length >= 1 && val[0].m_Request == rideNeeder.m_RideRequest)
							{
								if (m_CarNavigationLanes.HasBuffer(dispatched.m_Handler))
								{
									DynamicBuffer<PathElement> val2 = m_PathElements[entity];
									DynamicBuffer<CarNavigationLane> val3 = m_CarNavigationLanes[dispatched.m_Handler];
									if (val3.Length > 0 && val2.Length > pathOwner.m_ElementIndex)
									{
										PathElement pathElement = val2[pathOwner.m_ElementIndex];
										CarNavigationLane carNavigationLane = val3[val3.Length - 1];
										if (carNavigationLane.m_Lane == pathElement.m_Target && carNavigationLane.m_CurvePosition.y != pathElement.m_TargetDelta.y && m_CurveData.HasComponent(currentLane.m_Lane) && m_CurveData.HasComponent(pathElement.m_Target))
										{
											pathElement.m_TargetDelta = float2.op_Implicit(carNavigationLane.m_CurvePosition.y);
											val2[pathOwner.m_ElementIndex] = pathElement;
											float3 val4 = MathUtils.Position(m_CurveData[pathElement.m_Target].m_Bezier, pathElement.m_TargetDelta.y);
											float num = default(float);
											MathUtils.Distance(m_CurveData[currentLane.m_Lane].m_Bezier, val4, ref num);
											if (num != currentLane.m_CurvePosition.y)
											{
												currentLane.m_CurvePosition.y = num;
												currentLane.m_Flags &= ~CreatureLaneFlags.EndReached;
												return true;
											}
										}
									}
								}
								if ((taxi.m_State & TaxiFlags.Boarding) != 0)
								{
									Transform transform = m_TransformData[entity];
									m_BoardingQueue.Enqueue(Boarding.TryEnterVehicle(entity, Entity.Null, dispatched.m_Handler, Entity.Null, Entity.Null, transform.m_Position, CreatureVehicleFlags.Leader));
								}
							}
						}
					}
					else if (m_ServiceRequestData.HasComponent(rideNeeder.m_RideRequest) && m_ServiceRequestData[rideNeeder.m_RideRequest].m_FailCount >= 3)
					{
						resident.m_Flags |= ResidentFlags.IgnoreTaxi;
						currentLane.m_Flags &= ~(CreatureLaneFlags.ParkingSpace | CreatureLaneFlags.Taxi);
						pathOwner.m_State &= ~PathFlags.Failed;
						pathOwner.m_State |= PathFlags.Obsolete;
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<RideNeeder>(jobIndex, entity);
						return false;
					}
					return true;
				}
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<RideNeeder>(jobIndex, entity, default(RideNeeder));
				return true;
			}
			CarKeeper carKeeper = default(CarKeeper);
			if (EntitiesExtensions.TryGetEnabledComponent<CarKeeper>(m_CarKeeperData, resident.m_Citizen, ref carKeeper) && m_ParkedCarData.HasComponent(carKeeper.m_Car))
			{
				ActivateParkedCar(jobIndex, ref random, entity, carKeeper.m_Car, ref resident, ref pathOwner, ref target, groupCreatures);
				Transform transform2 = m_TransformData[entity];
				m_BoardingQueue.Enqueue(Boarding.TryEnterVehicle(entity, Entity.Null, carKeeper.m_Car, Entity.Null, Entity.Null, transform2.m_Position, CreatureVehicleFlags.Leader | CreatureVehicleFlags.Driver));
				return true;
			}
			currentLane.m_Flags &= ~(CreatureLaneFlags.ParkingSpace | CreatureLaneFlags.Taxi);
			pathOwner.m_State &= ~PathFlags.Failed;
			pathOwner.m_State |= PathFlags.Obsolete;
			return false;
		}

		private bool TransportStopReached(int jobIndex, ref Random random, Entity entity, PrefabRef prefabRef, bool isUnspawned, ref Game.Creatures.Resident resident, ref HumanCurrentLane currentLane, ref PathOwner pathOwner, ref Target target)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<PathElement> val = m_PathElements[entity];
			if (val.Length >= pathOwner.m_ElementIndex + 2)
			{
				Entity target2 = val[pathOwner.m_ElementIndex].m_Target;
				Entity target3 = val[pathOwner.m_ElementIndex + 1].m_Target;
				if ((resident.m_Flags & ResidentFlags.WaitingTransport) == 0)
				{
					resident.m_Flags |= ResidentFlags.NoLateDeparture;
				}
				uint minDeparture = math.select(0u, m_SimulationFrameIndex, (resident.m_Flags & ResidentFlags.NoLateDeparture) != 0);
				if (RouteUtils.GetBoardingVehicle(currentLane.m_Lane, target2, target3, minDeparture, ref m_OwnerData, ref m_RouteConnectedData, ref m_BoardingVehicleData, ref m_CurrentRouteData, ref m_AccessLaneLaneData, ref m_PublicTransportData, ref m_TaxiData, ref m_ConnectedRoutes, out var vehicle, out var testing, out var obsolete))
				{
					TryEnterVehicle(entity, vehicle, target2, ref resident, ref currentLane);
					SetQueuePosition(entity, prefabRef, target2, ref currentLane);
					return true;
				}
				if (!obsolete)
				{
					if ((resident.m_Flags & ResidentFlags.WaitingTransport) == 0 || resident.m_Timer < 5000)
					{
						if (testing)
						{
							Transform transform = m_TransformData[entity];
							m_BoardingQueue.Enqueue(Boarding.RequireStop(entity, vehicle, transform.m_Position));
						}
						if (isUnspawned && (currentLane.m_Flags & (CreatureLaneFlags.TransformTarget | CreatureLaneFlags.Connection)) != 0 && (currentLane.m_Flags & CreatureLaneFlags.WaitPosition) == 0)
						{
							currentLane.m_Flags &= ~CreatureLaneFlags.EndReached;
							currentLane.m_Flags |= CreatureLaneFlags.WaitPosition;
							currentLane.m_CurvePosition.y = ((Random)(ref random)).NextFloat(0f, 1f);
						}
						SetQueuePosition(entity, prefabRef, target2, ref currentLane);
						return true;
					}
					m_BoardingQueue.Enqueue(Boarding.WaitTimeExceeded(entity, target2));
					if (m_BoardingVehicleData.HasComponent(target2))
					{
						resident.m_Flags |= ResidentFlags.IgnoreTaxi;
					}
					else
					{
						resident.m_Flags |= ResidentFlags.IgnoreTransport;
					}
				}
			}
			currentLane.m_Flags &= ~CreatureLaneFlags.Transport;
			pathOwner.m_State &= ~PathFlags.Failed;
			pathOwner.m_State |= PathFlags.Obsolete;
			return false;
		}

		private void SetQueuePosition(Entity entity, PrefabRef prefabRef, Entity targetEntity, ref HumanCurrentLane currentLane)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = m_TransformData[entity];
			Sphere3 queueArea = CreatureUtils.GetQueueArea(m_PrefabObjectGeometryData[prefabRef.m_Prefab], transform.m_Position);
			CreatureUtils.SetQueue(ref currentLane.m_QueueEntity, ref currentLane.m_QueueArea, targetEntity, queueArea);
		}

		private Entity GetHomeBuilding(ref Game.Creatures.Resident resident, out bool movingAway)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			movingAway = false;
			HouseholdMember householdMember = default(HouseholdMember);
			if (m_HouseholdMembers.TryGetComponent(resident.m_Citizen, ref householdMember))
			{
				if (m_MovingAwayData.HasComponent(householdMember.m_Household))
				{
					movingAway = true;
					return Entity.Null;
				}
				PropertyRenter propertyRenter = default(PropertyRenter);
				if (m_PropertyRenters.TryGetComponent(householdMember.m_Household, ref propertyRenter) && ((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(propertyRenter.m_Property) && !m_DeletedData.HasComponent(propertyRenter.m_Property))
				{
					return propertyRenter.m_Property;
				}
				TouristHousehold touristHousehold = default(TouristHousehold);
				if (m_TouristHouseholds.TryGetComponent(householdMember.m_Household, ref touristHousehold) && m_PropertyRenters.TryGetComponent(touristHousehold.m_Hotel, ref propertyRenter) && ((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(propertyRenter.m_Property) && !m_DeletedData.HasComponent(propertyRenter.m_Property))
				{
					return propertyRenter.m_Property;
				}
				HomelessHousehold homelessHousehold = default(HomelessHousehold);
				if (m_HomelessHouseholdData.TryGetComponent(householdMember.m_Household, ref homelessHousehold) && ((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(homelessHousehold.m_TempHome) && !m_DeletedData.HasComponent(homelessHousehold.m_TempHome))
				{
					return homelessHousehold.m_TempHome;
				}
				Citizen citizen = default(Citizen);
				if (m_CitizenData.TryGetComponent(resident.m_Citizen, ref citizen))
				{
					movingAway = (citizen.m_State & CitizenFlags.Commuter) != CitizenFlags.None || !((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(householdMember.m_Household);
					return Entity.Null;
				}
			}
			movingAway = true;
			return Entity.Null;
		}

		private void TryEnterVehicle(Entity entity, Entity vehicle, Entity waypoint, ref Game.Creatures.Resident resident, ref HumanCurrentLane currentLane)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = m_TransformData[entity];
			m_BoardingQueue.Enqueue(Boarding.TryEnterVehicle(entity, Entity.Null, vehicle, Entity.Null, waypoint, transform.m_Position, CreatureVehicleFlags.Leader));
		}

		private void FinishEnterVehicle(Entity entity, Entity vehicle, Entity controllerVehicle, ref Game.Creatures.Resident resident, ref Human human, ref HumanCurrentLane currentLane)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			Entity household = GetHousehold(resident);
			int ticketPrice = GetTicketPrice(vehicle);
			m_BoardingQueue.Enqueue(Boarding.FinishEnterVehicle(entity, household, vehicle, controllerVehicle, currentLane, ticketPrice));
			human.m_Flags &= ~(HumanFlags.Run | HumanFlags.Emergency);
		}

		private void FinishExitVehicle(Entity entity, Entity vehicle, ref HumanCurrentLane currentLane)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			currentLane.m_Flags &= ~CreatureLaneFlags.EndOfPath;
			m_BoardingQueue.Enqueue(Boarding.FinishExitVehicle(entity, vehicle));
		}

		private void CancelEnterVehicle(Entity entity, Entity vehicle, ref Game.Creatures.Resident resident, ref Human human, ref HumanCurrentLane currentLane, ref PathOwner pathOwner)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			m_BoardingQueue.Enqueue(Boarding.CancelEnterVehicle(entity, vehicle));
			human.m_Flags &= ~(HumanFlags.Run | HumanFlags.Emergency);
			DynamicBuffer<PathElement> val = m_PathElements[entity];
			for (int i = pathOwner.m_ElementIndex; i < val.Length; i++)
			{
				if (val[i].m_Target == vehicle)
				{
					val.RemoveRange(0, i + 1);
					pathOwner.m_ElementIndex = 0;
					return;
				}
			}
			val.Clear();
			pathOwner.m_ElementIndex = 0;
			pathOwner.m_State &= ~PathFlags.Failed;
			pathOwner.m_State |= PathFlags.Obsolete;
		}

		private Entity GetHousehold(Game.Creatures.Resident resident)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			if (m_HouseholdMembers.HasComponent(resident.m_Citizen))
			{
				return m_HouseholdMembers[resident.m_Citizen].m_Household;
			}
			return Entity.Null;
		}

		private int GetTicketPrice(Entity vehicle)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			if (m_CurrentRouteData.HasComponent(vehicle))
			{
				CurrentRoute currentRoute = m_CurrentRouteData[vehicle];
				if (m_TransportLineData.HasComponent(currentRoute.m_Route))
				{
					return m_TransportLineData[currentRoute.m_Route].m_TicketPrice;
				}
			}
			return 0;
		}

		private void FindNewPath(Entity entity, PrefabRef prefabRef, ref Game.Creatures.Resident resident, ref Human human, ref HumanCurrentLane currentLane, ref PathOwner pathOwner, ref Target target, ref Divert divert)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0591: Unknown result type (might be due to invalid IL or missing references)
			//IL_0592: Unknown result type (might be due to invalid IL or missing references)
			//IL_0597: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0617: Unknown result type (might be due to invalid IL or missing references)
			//IL_0618: Unknown result type (might be due to invalid IL or missing references)
			//IL_061d: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
			CreatureData creatureData = m_PrefabCreatureData[prefabRef.m_Prefab];
			HumanData humanData = m_PrefabHumanData[prefabRef.m_Prefab];
			pathOwner.m_State &= ~(PathFlags.AddDestination | PathFlags.Divert);
			PathfindParameters parameters = new PathfindParameters
			{
				m_MaxSpeed = float2.op_Implicit(277.77777f),
				m_WalkSpeed = float2.op_Implicit(humanData.m_WalkSpeed),
				m_Weights = new PathfindWeights(1f, 1f, 1f, 1f),
				m_Methods = (PathMethod.Pedestrian | RouteUtils.GetTaxiMethods(resident) | RouteUtils.GetPublicTransportMethods(resident, m_TimeOfDay)),
				m_SecondaryIgnoredRules = VehicleUtils.GetIgnoredPathfindRulesTaxiDefaults(),
				m_MaxCost = CitizenBehaviorSystem.kMaxPathfindCost
			};
			SetupQueueTarget origin = new SetupQueueTarget
			{
				m_Type = SetupTargetType.CurrentLocation,
				m_Methods = PathMethod.Pedestrian,
				m_RandomCost = 30f
			};
			SetupQueueTarget destination = new SetupQueueTarget
			{
				m_Type = SetupTargetType.CurrentLocation,
				m_Methods = PathMethod.Pedestrian,
				m_Entity = target.m_Target,
				m_RandomCost = 30f,
				m_ActivityMask = creatureData.m_SupportedActivities
			};
			if (m_CitizenData.HasComponent(resident.m_Citizen))
			{
				Citizen citizen = m_CitizenData[resident.m_Citizen];
				Entity household = m_HouseholdMembers[resident.m_Citizen].m_Household;
				Household household2 = m_HouseholdData[household];
				parameters.m_Weights = CitizenUtils.GetPathfindWeights(citizen, household2, m_HouseholdCitizens[household].Length);
			}
			if (m_HouseholdMembers.HasComponent(resident.m_Citizen))
			{
				Entity household3 = m_HouseholdMembers[resident.m_Citizen].m_Household;
				if (m_PropertyRenters.HasComponent(household3))
				{
					parameters.m_Authorization1 = m_PropertyRenters[household3].m_Property;
				}
			}
			if (m_WorkerData.HasComponent(resident.m_Citizen))
			{
				Worker worker = m_WorkerData[resident.m_Citizen];
				if (m_PropertyRenters.HasComponent(worker.m_Workplace))
				{
					parameters.m_Authorization2 = m_PropertyRenters[worker.m_Workplace].m_Property;
				}
				else
				{
					parameters.m_Authorization2 = worker.m_Workplace;
				}
			}
			CarKeeper carKeeper = default(CarKeeper);
			ParkedCar parkedCar = default(ParkedCar);
			if (EntitiesExtensions.TryGetEnabledComponent<CarKeeper>(m_CarKeeperData, resident.m_Citizen, ref carKeeper) && m_ParkedCarData.TryGetComponent(carKeeper.m_Car, ref parkedCar))
			{
				PrefabRef prefabRef2 = m_PrefabRefData[carKeeper.m_Car];
				CarData carData = m_PrefabCarData[prefabRef2.m_Prefab];
				parameters.m_MaxSpeed.x = carData.m_MaxSpeed;
				parameters.m_ParkingTarget = parkedCar.m_Lane;
				parameters.m_ParkingDelta = parkedCar.m_CurvePosition;
				parameters.m_ParkingSize = VehicleUtils.GetParkingSize(carKeeper.m_Car, ref m_PrefabRefData, ref m_PrefabObjectGeometryData);
				parameters.m_Methods |= VehicleUtils.GetPathMethods(carData) | PathMethod.Parking;
				parameters.m_IgnoredRules = VehicleUtils.GetIgnoredPathfindRules(carData);
				Game.Vehicles.PersonalCar personalCar = default(Game.Vehicles.PersonalCar);
				if (m_PersonalCarData.TryGetComponent(carKeeper.m_Car, ref personalCar) && (personalCar.m_State & PersonalCarFlags.HomeTarget) == 0)
				{
					parameters.m_PathfindFlags |= PathfindFlags.ParkingReset;
				}
			}
			bool flag = false;
			TravelPurpose travelPurpose = default(TravelPurpose);
			if (m_TravelPurposeData.TryGetComponent(resident.m_Citizen, ref travelPurpose))
			{
				switch (travelPurpose.m_Purpose)
				{
				case Purpose.EmergencyShelter:
					parameters.m_Weights = new PathfindWeights(1f, 0.2f, 0f, 0.1f);
					break;
				case Purpose.Hospital:
				case Purpose.Deathcare:
				{
					HealthProblem healthProblem = default(HealthProblem);
					flag = m_HealthProblemData.TryGetComponent(resident.m_Citizen, ref healthProblem) && (healthProblem.m_Flags & HealthProblemFlags.RequireTransport) != 0;
					break;
				}
				case Purpose.MovingAway:
					parameters.m_MaxCost = CitizenBehaviorSystem.kMaxMovingAwayCost;
					break;
				}
			}
			if ((resident.m_Flags & ResidentFlags.IgnoreBenches) != ResidentFlags.None)
			{
				destination.m_ActivityMask.m_Mask &= ~new ActivityMask(ActivityType.BenchSitting).m_Mask;
				destination.m_ActivityMask.m_Mask &= ~new ActivityMask(ActivityType.PullUps).m_Mask;
			}
			if ((resident.m_Flags & ResidentFlags.IgnoreAreas) != ResidentFlags.None)
			{
				destination.m_ActivityMask.m_Mask &= ~new ActivityMask(ActivityType.Standing).m_Mask;
				destination.m_ActivityMask.m_Mask &= ~new ActivityMask(ActivityType.GroundLaying).m_Mask;
				destination.m_ActivityMask.m_Mask &= ~new ActivityMask(ActivityType.GroundSitting).m_Mask;
				destination.m_ActivityMask.m_Mask &= ~new ActivityMask(ActivityType.PushUps).m_Mask;
				destination.m_ActivityMask.m_Mask &= ~new ActivityMask(ActivityType.SitUps).m_Mask;
				destination.m_ActivityMask.m_Mask &= ~new ActivityMask(ActivityType.JumpingJacks).m_Mask;
				destination.m_ActivityMask.m_Mask &= ~new ActivityMask(ActivityType.JumpingLunges).m_Mask;
				destination.m_ActivityMask.m_Mask &= ~new ActivityMask(ActivityType.Squats).m_Mask;
				destination.m_ActivityMask.m_Mask &= ~new ActivityMask(ActivityType.Yoga).m_Mask;
			}
			if (flag)
			{
				human.m_Flags |= HumanFlags.Carried;
				currentLane.m_CurvePosition.y = currentLane.m_CurvePosition.x;
				pathOwner.m_ElementIndex = 0;
				pathOwner.m_State &= ~(PathFlags.Failed | PathFlags.Obsolete | PathFlags.DivertObsolete | PathFlags.CachedObsolete);
				m_PathElements[entity].Clear();
			}
			else if (CreatureUtils.DivertDestination(ref destination, ref pathOwner, divert))
			{
				CreatureUtils.SetupPathfind(item: new SetupQueueItem(entity, parameters, origin, destination), currentLane: ref currentLane, pathOwner: ref pathOwner, queue: m_PathfindQueue);
			}
			else
			{
				currentLane.m_CurvePosition.y = currentLane.m_CurvePosition.x;
				pathOwner.m_ElementIndex = 0;
				pathOwner.m_State |= PathFlags.CachedObsolete;
				pathOwner.m_State &= ~(PathFlags.Failed | PathFlags.Obsolete | PathFlags.DivertObsolete);
				m_PathElements[entity].Clear();
			}
		}

		private void ActivateParkedCar(int jobIndex, ref Random random, Entity entity, Entity carEntity, ref Game.Creatures.Resident resident, ref PathOwner pathOwner, ref Target target, DynamicBuffer<GroupCreature> groupCreatures)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0508: Unknown result type (might be due to invalid IL or missing references)
			//IL_0516: Unknown result type (might be due to invalid IL or missing references)
			//IL_0520: Unknown result type (might be due to invalid IL or missing references)
			//IL_0525: Unknown result type (might be due to invalid IL or missing references)
			//IL_0527: Unknown result type (might be due to invalid IL or missing references)
			//IL_0529: Unknown result type (might be due to invalid IL or missing references)
			//IL_0451: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0557: Unknown result type (might be due to invalid IL or missing references)
			//IL_0559: Unknown result type (might be due to invalid IL or missing references)
			//IL_055e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0545: Unknown result type (might be due to invalid IL or missing references)
			//IL_0547: Unknown result type (might be due to invalid IL or missing references)
			//IL_054c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0562: Unknown result type (might be due to invalid IL or missing references)
			//IL_0571: Unknown result type (might be due to invalid IL or missing references)
			//IL_0585: Unknown result type (might be due to invalid IL or missing references)
			//IL_0587: Unknown result type (might be due to invalid IL or missing references)
			//IL_059a: Unknown result type (might be due to invalid IL or missing references)
			ParkedCar parkedCar = m_ParkedCarData[carEntity];
			Game.Vehicles.CarLaneFlags carLaneFlags = Game.Vehicles.CarLaneFlags.EndReached | Game.Vehicles.CarLaneFlags.ParkingSpace | Game.Vehicles.CarLaneFlags.FixedLane;
			DynamicBuffer<LayoutElement> val = default(DynamicBuffer<LayoutElement>);
			if (m_VehicleLayouts.HasBuffer(carEntity))
			{
				val = m_VehicleLayouts[carEntity];
			}
			if (parkedCar.m_Lane == Entity.Null)
			{
				DynamicBuffer<PathElement> val2 = m_PathElements[entity];
				if (val2.Length > pathOwner.m_ElementIndex)
				{
					PathElement pathElement = val2[pathOwner.m_ElementIndex];
					if (m_CurveData.HasComponent(pathElement.m_Target))
					{
						parkedCar.m_Lane = pathElement.m_Target;
						Curve curve = m_CurveData[parkedCar.m_Lane];
						Transform transform = m_TransformData[entity];
						MathUtils.Distance(curve.m_Bezier, transform.m_Position, ref parkedCar.m_CurvePosition);
						Transform transform2 = VehicleUtils.CalculateTransform(curve, parkedCar.m_CurvePosition);
						bool flag = false;
						if (m_ConnectionLaneData.HasComponent(parkedCar.m_Lane))
						{
							Game.Net.ConnectionLane connectionLane = m_ConnectionLaneData[parkedCar.m_Lane];
							if ((connectionLane.m_Flags & ConnectionLaneFlags.Parking) != 0)
							{
								parkedCar.m_CurvePosition = ((Random)(ref random)).NextFloat(0f, 1f);
								transform2.m_Position = VehicleUtils.GetConnectionParkingPosition(connectionLane, curve.m_Bezier, parkedCar.m_CurvePosition);
							}
							flag = true;
						}
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Transform>(jobIndex, carEntity, transform2);
						if (flag)
						{
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Unspawned>(jobIndex, carEntity, default(Unspawned));
						}
						if (val.IsCreated)
						{
							for (int i = 1; i < val.Length; i++)
							{
								Entity vehicle = val[i].m_Vehicle;
								((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Transform>(jobIndex, vehicle, transform2);
								if (flag)
								{
									((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Unspawned>(jobIndex, vehicle, default(Unspawned));
								}
							}
						}
					}
				}
			}
			if (m_ConnectionLaneData.HasComponent(parkedCar.m_Lane))
			{
				carLaneFlags = (((m_ConnectionLaneData[parkedCar.m_Lane].m_Flags & ConnectionLaneFlags.Area) == 0) ? (carLaneFlags | Game.Vehicles.CarLaneFlags.Connection) : (carLaneFlags | Game.Vehicles.CarLaneFlags.Area));
			}
			Game.Vehicles.PersonalCar personalCar = m_PersonalCarData[carEntity];
			personalCar.m_State |= PersonalCarFlags.Boarding;
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(jobIndex, carEntity, ref m_ParkedToMovingCarRemoveTypes);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent(jobIndex, carEntity, ref m_ParkedToMovingCarAddTypes);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Game.Vehicles.PersonalCar>(jobIndex, carEntity, personalCar);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CarCurrentLane>(jobIndex, carEntity, new CarCurrentLane(parkedCar, carLaneFlags));
			if (m_ParkingLaneData.HasComponent(parkedCar.m_Lane) || m_GarageLaneData.HasComponent(parkedCar.m_Lane))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(jobIndex, parkedCar.m_Lane);
			}
			if (val.IsCreated)
			{
				for (int j = 1; j < val.Length; j++)
				{
					Entity vehicle2 = val[j].m_Vehicle;
					ParkedCar parkedCar2 = m_ParkedCarData[vehicle2];
					if (parkedCar2.m_Lane == Entity.Null)
					{
						parkedCar2.m_Lane = parkedCar.m_Lane;
						parkedCar2.m_CurvePosition = parkedCar.m_CurvePosition;
					}
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(jobIndex, vehicle2, ref m_ParkedToMovingCarRemoveTypes);
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent(jobIndex, vehicle2, ref m_ParkedToMovingTrailerAddTypes);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CarTrailerLane>(jobIndex, vehicle2, new CarTrailerLane(parkedCar2));
					if ((m_ParkingLaneData.HasComponent(parkedCar2.m_Lane) || m_GarageLaneData.HasComponent(parkedCar2.m_Lane)) && parkedCar2.m_Lane != parkedCar.m_Lane)
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(jobIndex, parkedCar2.m_Lane);
					}
				}
			}
			if (val.IsCreated && val.Length > 1)
			{
				return;
			}
			int num = 1;
			int num2 = 0;
			if (groupCreatures.IsCreated)
			{
				for (int k = 0; k < groupCreatures.Length; k++)
				{
					if (m_AnimalData.HasComponent(groupCreatures[k].m_Creature))
					{
						num2++;
					}
					else
					{
						num++;
					}
				}
			}
			int num3 = num;
			int num4 = 1 + num2;
			if (m_TravelPurposeData.HasComponent(resident.m_Citizen))
			{
				switch (m_TravelPurposeData[resident.m_Citizen].m_Purpose)
				{
				case Purpose.MovingAway:
					if (((Random)(ref random)).NextInt(20) == 0)
					{
						num3 += 5;
						num4 += 5;
					}
					else if (((Random)(ref random)).NextInt(10) == 0)
					{
						num4 += 5;
						if (((Random)(ref random)).NextInt(10) == 0)
						{
							num4 += 5;
						}
					}
					break;
				case Purpose.Leisure:
					if (((Random)(ref random)).NextInt(20) == 0)
					{
						num3 += 5;
						num4 += 5;
					}
					break;
				case Purpose.Shopping:
					if (((Random)(ref random)).NextInt(10) == 0)
					{
						num4 += 5;
						if (((Random)(ref random)).NextInt(10) == 0)
						{
							num4 += 5;
						}
					}
					break;
				}
			}
			Transform tractorTransform = m_TransformData[carEntity];
			PrefabRef prefabRef = m_PrefabRefData[carEntity];
			Entity val3 = m_PersonalCarSelectData.CreateTrailer(m_CommandBuffer, jobIndex, ref random, num3, num4, noSlowVehicles: false, prefabRef.m_Prefab, tractorTransform, (PersonalCarFlags)0u, stopped: false);
			if (val3 != Entity.Null)
			{
				DynamicBuffer<LayoutElement> val4 = ((!val.IsCreated) ? ((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<LayoutElement>(jobIndex, carEntity) : ((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<LayoutElement>(jobIndex, carEntity));
				val4.Add(new LayoutElement(carEntity));
				val4.Add(new LayoutElement(val3));
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Controller>(jobIndex, val3, new Controller(carEntity));
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CarTrailerLane>(jobIndex, val3, new CarTrailerLane(parkedCar));
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct BoardingJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<Citizen> m_Citizens;

		[ReadOnly]
		public ComponentLookup<Transform> m_Transforms;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_ObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<TaxiData> m_TaxiData;

		[ReadOnly]
		public ComponentLookup<PublicTransportVehicleData> m_PublicTransportVehicleData;

		[ReadOnly]
		public ComponentLookup<PersonalCarData> m_PrefabPersonalCarData;

		[ReadOnly]
		public BufferLookup<GroupCreature> m_GroupCreatures;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_VehicleLayouts;

		[ReadOnly]
		public BufferLookup<ActivityLocationElement> m_ActivityLocations;

		public ComponentLookup<Game.Creatures.Resident> m_Residents;

		public ComponentLookup<Creature> m_Creatures;

		public ComponentLookup<Game.Vehicles.Taxi> m_Taxis;

		public ComponentLookup<Game.Vehicles.PublicTransport> m_PublicTransports;

		public ComponentLookup<WaitingPassengers> m_WaitingPassengers;

		public BufferLookup<Queue> m_Queues;

		public BufferLookup<Passenger> m_Passengers;

		public BufferLookup<LaneObject> m_LaneObjects;

		public BufferLookup<Resources> m_Resources;

		public ComponentLookup<PlayerMoney> m_PlayerMoney;

		[ReadOnly]
		public Entity m_City;

		[ReadOnly]
		public ComponentTypeSet m_CurrentLaneTypes;

		[ReadOnly]
		public ComponentTypeSet m_CurrentLaneTypesRelative;

		public NativeQueue<Boarding> m_BoardingQueue;

		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_SearchTree;

		public EntityCommandBuffer m_CommandBuffer;

		public ParallelWriter<StatisticsEvent> m_StatisticsEventQueue;

		public NativeQueue<ServiceFeeSystem.FeeEvent> m_FeeQueue;

		public void Execute()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			NativeParallelHashMap<Entity, int3> freeSpaceMap = default(NativeParallelHashMap<Entity, int3>);
			Boarding boarding = default(Boarding);
			while (m_BoardingQueue.TryDequeue(ref boarding))
			{
				switch (boarding.m_Type)
				{
				case BoardingType.Exit:
					ExitVehicle(ref freeSpaceMap, boarding.m_Passenger, boarding.m_Household, boarding.m_Vehicle, boarding.m_CurrentLane, boarding.m_Position, boarding.m_Rotation, boarding.m_TicketPrice);
					break;
				case BoardingType.TryEnter:
					TryEnterVehicle(ref freeSpaceMap, boarding.m_Passenger, boarding.m_Leader, boarding.m_Vehicle, boarding.m_LeaderVehicle, boarding.m_Waypoint, boarding.m_Position, boarding.m_Flags);
					break;
				case BoardingType.FinishEnter:
					FinishEnterVehicle(boarding.m_Passenger, boarding.m_Household, boarding.m_Vehicle, boarding.m_LeaderVehicle, boarding.m_CurrentLane, boarding.m_TicketPrice);
					break;
				case BoardingType.CancelEnter:
					CancelEnterVehicle(ref freeSpaceMap, boarding.m_Passenger, boarding.m_Vehicle);
					break;
				case BoardingType.RequireStop:
					RequireStop(ref freeSpaceMap, boarding.m_Passenger, boarding.m_Vehicle, boarding.m_Position);
					break;
				case BoardingType.WaitTimeExceeded:
					WaitTimeExceeded(boarding.m_Passenger, boarding.m_Waypoint);
					break;
				case BoardingType.WaitTimeEstimate:
					WaitTimeEstimate(boarding.m_Waypoint, boarding.m_TicketPrice);
					break;
				case BoardingType.FinishExit:
					FinishExitVehicle(ref freeSpaceMap, boarding.m_Passenger, boarding.m_Vehicle);
					break;
				}
			}
			if (freeSpaceMap.IsCreated)
			{
				freeSpaceMap.Dispose();
			}
		}

		private void ExitVehicle(ref NativeParallelHashMap<Entity, int3> freeSpaceMap, Entity passenger, Entity household, Entity vehicle, HumanCurrentLane newCurrentLane, float3 position, quaternion rotation, int ticketPrice)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Relative>(passenger);
			Game.Creatures.Resident resident = m_Residents[passenger];
			resident.m_Flags &= ~ResidentFlags.InVehicle;
			resident.m_Timer = 0;
			m_Residents[passenger] = resident;
			if (m_LaneObjects.HasBuffer(newCurrentLane.m_Lane))
			{
				NetUtils.AddLaneObject(m_LaneObjects[newCurrentLane.m_Lane], passenger, float2.op_Implicit(newCurrentLane.m_CurvePosition.x));
			}
			else
			{
				PrefabRef prefabRef = m_PrefabRefData[passenger];
				ObjectGeometryData geometryData = m_ObjectGeometryData[prefabRef.m_Prefab];
				Bounds3 bounds = ObjectUtils.CalculateBounds(position, quaternion.identity, geometryData);
				m_SearchTree.Add(passenger, new QuadTreeBoundsXZ(bounds));
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent(passenger, ref m_CurrentLaneTypes);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(passenger, default(Updated));
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<HumanCurrentLane>(passenger, newCurrentLane);
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Transform>(passenger, new Transform(position, rotation));
			if (ticketPrice != 0 && m_Resources.HasBuffer(household) && m_PlayerMoney.HasComponent(m_City))
			{
				DynamicBuffer<Resources> resources = m_Resources[household];
				if (ticketPrice > 0)
				{
					PlayerMoney playerMoney = m_PlayerMoney[m_City];
					playerMoney.Add(ticketPrice);
					m_PlayerMoney[m_City] = playerMoney;
					m_FeeQueue.Enqueue(new ServiceFeeSystem.FeeEvent
					{
						m_Amount = 1f,
						m_Cost = ticketPrice,
						m_Resource = PlayerResource.PublicTransport,
						m_Outside = false
					});
					ticketPrice = -ticketPrice;
				}
				EconomyUtils.AddResources(Resource.Money, ticketPrice, resources);
			}
		}

		private void FinishExitVehicle(ref NativeParallelHashMap<Entity, int3> freeSpaceMap, Entity passenger, Entity vehicle)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			if (m_Passengers.HasBuffer(vehicle) && CollectionUtils.RemoveValue<Passenger>(m_Passengers[vehicle], new Passenger(passenger)))
			{
				int3 freeSpace = GetFreeSpace(ref freeSpaceMap, vehicle);
				freeSpace.x++;
				freeSpaceMap[vehicle] = freeSpace;
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<CurrentVehicle>(passenger);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(passenger, default(BatchesUpdated));
		}

		private Entity TryFindVehicle(ref NativeParallelHashMap<Entity, int3> freeSpaceMap, Entity vehicle, Entity leaderVehicle, float3 position, bool isLeader, int requiredSpace, out float distance)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			Entity result = vehicle;
			int3 val = int3.op_Implicit(0);
			DynamicBuffer<LayoutElement> val2 = default(DynamicBuffer<LayoutElement>);
			if (m_VehicleLayouts.TryGetBuffer(vehicle, ref val2))
			{
				distance = float.MaxValue;
				int num = 0;
				for (int i = 0; i < val2.Length; i++)
				{
					Entity vehicle2 = val2[i].m_Vehicle;
					float num2 = math.distancesq(position, m_Transforms[vehicle2].m_Position);
					int3 freeSpace = GetFreeSpace(ref freeSpaceMap, vehicle2);
					if (isLeader)
					{
						((int3)(ref val)).xy = ((int3)(ref val)).xy + ((int3)(ref freeSpace)).xy;
						val.z |= freeSpace.z;
						freeSpace.x = math.min(freeSpace.x, requiredSpace);
					}
					else
					{
						freeSpace.x += math.select(0, requiredSpace, vehicle2 == leaderVehicle);
						((int3)(ref val)).xy = ((int3)(ref val)).xy + ((int3)(ref freeSpace)).xy;
						val.z |= freeSpace.z;
						freeSpace.x = math.min(freeSpace.x, 1) * 2;
						freeSpace.x += math.select(0, 1, vehicle2 == leaderVehicle);
					}
					if ((freeSpace.x > num) | ((freeSpace.x == num) & (num2 < distance)))
					{
						distance = num2;
						num = freeSpace.x;
						result = vehicle2;
						if ((freeSpace.z & 4) != 0 && isLeader)
						{
							break;
						}
					}
				}
				distance = math.sqrt(distance);
			}
			else
			{
				val = GetFreeSpace(ref freeSpaceMap, vehicle);
				distance = math.distance(position, m_Transforms[vehicle].m_Position);
			}
			if (isLeader)
			{
				if ((val.z & 1) != 0 && val.x >= requiredSpace)
				{
					return result;
				}
				if ((val.z & 6) != 0 && val.x == val.y)
				{
					return result;
				}
				return Entity.Null;
			}
			return result;
		}

		private int3 GetFreeSpace(ref NativeParallelHashMap<Entity, int3> freeSpaceMap, Entity vehicle)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			if (!freeSpaceMap.IsCreated)
			{
				freeSpaceMap = new NativeParallelHashMap<Entity, int3>(20, AllocatorHandle.op_Implicit((Allocator)2));
			}
			int3 val = default(int3);
			if (freeSpaceMap.TryGetValue(vehicle, ref val))
			{
				return val;
			}
			DynamicBuffer<Passenger> val2 = default(DynamicBuffer<Passenger>);
			if (m_Passengers.TryGetBuffer(vehicle, ref val2))
			{
				val = int3.op_Implicit(0);
				Game.Creatures.Resident resident = default(Game.Creatures.Resident);
				for (int i = 0; i < val2.Length; i++)
				{
					Passenger passenger = val2[i];
					if (m_Residents.TryGetComponent(passenger.m_Passenger, ref resident) && (resident.m_Flags & ResidentFlags.InVehicle) != ResidentFlags.None)
					{
						val.x -= 1 + GetPendingGroupMemberCount(passenger.m_Passenger);
					}
				}
				PrefabRef prefabRef = m_PrefabRefData[vehicle];
				PublicTransportVehicleData publicTransportVehicleData = default(PublicTransportVehicleData);
				TaxiData taxiData = default(TaxiData);
				PersonalCarData personalCarData = default(PersonalCarData);
				if (m_PublicTransportVehicleData.TryGetComponent(prefabRef.m_Prefab, ref publicTransportVehicleData))
				{
					((int3)(ref val)).xy = ((int3)(ref val)).xy + publicTransportVehicleData.m_PassengerCapacity;
					val.z |= 1;
				}
				else if (m_TaxiData.TryGetComponent(prefabRef.m_Prefab, ref taxiData))
				{
					((int3)(ref val)).xy = ((int3)(ref val)).xy + taxiData.m_PassengerCapacity;
					val.z |= 2;
				}
				else if (m_PrefabPersonalCarData.TryGetComponent(prefabRef.m_Prefab, ref personalCarData))
				{
					((int3)(ref val)).xy = ((int3)(ref val)).xy + personalCarData.m_PassengerCapacity;
					val.z |= 4;
				}
				else
				{
					((int3)(ref val)).xy = ((int3)(ref val)).xy + 1000000;
					val.z |= 1;
				}
				freeSpaceMap.Add(vehicle, val);
				return val;
			}
			freeSpaceMap.Add(vehicle, int3.op_Implicit(0));
			return int3.op_Implicit(0);
		}

		private int GetPendingGroupMemberCount(Entity leader)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			DynamicBuffer<GroupCreature> val = default(DynamicBuffer<GroupCreature>);
			if (m_GroupCreatures.TryGetBuffer(leader, ref val))
			{
				Game.Creatures.Resident resident = default(Game.Creatures.Resident);
				for (int i = 0; i < val.Length; i++)
				{
					GroupCreature groupCreature = val[i];
					if (m_Residents.TryGetComponent(groupCreature.m_Creature, ref resident) && (resident.m_Flags & ResidentFlags.InVehicle) == 0)
					{
						num++;
					}
				}
			}
			return num;
		}

		private void TryEnterVehicle(ref NativeParallelHashMap<Entity, int3> freeSpaceMap, Entity passenger, Entity leader, Entity vehicle, Entity leaderVehicle, Entity waypoint, float3 position, CreatureVehicleFlags flags)
		{
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			int num;
			if ((flags & CreatureVehicleFlags.Leader) != 0)
			{
				Entity val = vehicle;
				num = 1 + GetPendingGroupMemberCount(passenger);
				vehicle = TryFindVehicle(ref freeSpaceMap, vehicle, Entity.Null, position, isLeader: true, num, out var distance);
				if (vehicle == Entity.Null)
				{
					return;
				}
				Game.Vehicles.Taxi taxi = default(Game.Vehicles.Taxi);
				if (m_Taxis.TryGetComponent(val, ref taxi) && distance > taxi.m_MaxBoardingDistance)
				{
					taxi.m_MinWaitingDistance = math.min(taxi.m_MinWaitingDistance, distance);
					m_Taxis[val] = taxi;
					return;
				}
				Game.Vehicles.PublicTransport publicTransport = default(Game.Vehicles.PublicTransport);
				if (m_PublicTransports.TryGetComponent(val, ref publicTransport) && distance > publicTransport.m_MaxBoardingDistance)
				{
					publicTransport.m_MinWaitingDistance = math.min(publicTransport.m_MinWaitingDistance, distance);
					m_PublicTransports[val] = publicTransport;
					return;
				}
				int3 val2 = freeSpaceMap[vehicle];
				val2.x -= num;
				freeSpaceMap[vehicle] = val2;
			}
			else
			{
				num = GetPendingGroupMemberCount(leader);
				vehicle = TryFindVehicle(ref freeSpaceMap, vehicle, leaderVehicle, position, isLeader: false, num, out var _);
				if (vehicle == Entity.Null)
				{
					return;
				}
				if (vehicle != leaderVehicle)
				{
					int3 val3 = freeSpaceMap[leaderVehicle];
					val3.x++;
					freeSpaceMap[leaderVehicle] = val3;
					val3 = freeSpaceMap[vehicle];
					val3.x--;
					freeSpaceMap[vehicle] = val3;
				}
			}
			m_Passengers[vehicle].Add(new Passenger(passenger));
			ref Game.Creatures.Resident valueRW = ref m_Residents.GetRefRW(passenger).ValueRW;
			if ((flags & CreatureVehicleFlags.Leader) != 0 && m_WaitingPassengers.HasComponent(waypoint))
			{
				ref WaitingPassengers valueRW2 = ref m_WaitingPassengers.GetRefRW(waypoint).ValueRW;
				int num2 = (int)((float)(valueRW.m_Timer * num) * (2f / 15f));
				valueRW2.m_ConcludedAccumulation += num2;
				valueRW2.m_SuccessAccumulation = (ushort)math.min(65535, valueRW2.m_SuccessAccumulation + num);
			}
			valueRW.m_Flags &= ~(ResidentFlags.WaitingTransport | ResidentFlags.NoLateDeparture);
			valueRW.m_Flags |= ResidentFlags.InVehicle;
			valueRW.m_Timer = 0;
			m_Queues[passenger].Clear();
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CurrentVehicle>(passenger, new CurrentVehicle(vehicle, flags));
		}

		private void CancelEnterVehicle(ref NativeParallelHashMap<Entity, int3> freeSpaceMap, Entity passenger, Entity vehicle)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			if (m_Passengers.HasBuffer(vehicle) && CollectionUtils.RemoveValue<Passenger>(m_Passengers[vehicle], new Passenger(passenger)))
			{
				int3 freeSpace = GetFreeSpace(ref freeSpaceMap, vehicle);
				freeSpace.x++;
				freeSpaceMap[vehicle] = freeSpace;
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<CurrentVehicle>(passenger);
			ref Game.Creatures.Resident valueRW = ref m_Residents.GetRefRW(passenger).ValueRW;
			valueRW.m_Flags &= ~ResidentFlags.InVehicle;
			valueRW.m_Timer = 0;
		}

		private void FinishEnterVehicle(Entity passenger, Entity household, Entity vehicle, Entity controllerVehicle, HumanCurrentLane oldCurrentLane, int ticketPrice)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			TransportType transportType = TransportType.None;
			PrefabRef prefabRef = m_PrefabRefData[vehicle];
			if (m_TaxiData.HasComponent(prefabRef.m_Prefab))
			{
				transportType = TransportType.Taxi;
			}
			else if (m_PublicTransportVehicleData.HasComponent(prefabRef.m_Prefab))
			{
				transportType = m_PublicTransportVehicleData[prefabRef.m_Prefab].m_TransportType;
				Game.Vehicles.PublicTransport publicTransport = default(Game.Vehicles.PublicTransport);
				if (m_PublicTransports.TryGetComponent(controllerVehicle, ref publicTransport) && (publicTransport.m_State & (PublicTransportFlags.Evacuating | PublicTransportFlags.PrisonerTransport)) != 0)
				{
					transportType = TransportType.None;
				}
			}
			Creature creature = m_Creatures[passenger];
			creature.m_QueueEntity = Entity.Null;
			creature.m_QueueArea = default(Sphere3);
			m_Creatures[passenger] = creature;
			m_Queues[passenger].Clear();
			Game.Creatures.Resident resident = m_Residents[passenger];
			Citizen citizen = default(Citizen);
			if (m_Citizens.TryGetComponent(resident.m_Citizen, ref citizen))
			{
				PassengerType parameter = (((citizen.m_State & CitizenFlags.Tourist) != CitizenFlags.None) ? PassengerType.Tourist : PassengerType.Citizen);
				switch (transportType)
				{
				case TransportType.Bus:
					EnqueueStat(StatisticType.PassengerCountBus, 1, (int)parameter);
					break;
				case TransportType.Subway:
					EnqueueStat(StatisticType.PassengerCountSubway, 1, (int)parameter);
					break;
				case TransportType.Tram:
					EnqueueStat(StatisticType.PassengerCountTram, 1, (int)parameter);
					break;
				case TransportType.Train:
					EnqueueStat(StatisticType.PassengerCountTrain, 1, (int)parameter);
					break;
				case TransportType.Ship:
					EnqueueStat(StatisticType.PassengerCountShip, 1, (int)parameter);
					break;
				case TransportType.Airplane:
					EnqueueStat(StatisticType.PassengerCountAirplane, 1, (int)parameter);
					break;
				case TransportType.Taxi:
					EnqueueStat(StatisticType.PassengerCountTaxi, 1, (int)parameter);
					break;
				}
			}
			if (m_LaneObjects.HasBuffer(oldCurrentLane.m_Lane))
			{
				NetUtils.RemoveLaneObject(m_LaneObjects[oldCurrentLane.m_Lane], passenger);
			}
			else
			{
				m_SearchTree.TryRemove(passenger);
			}
			if (TryGetRelativeLocation(prefabRef.m_Prefab, out var relative))
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent(passenger, ref m_CurrentLaneTypesRelative);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Relative>(passenger, relative);
			}
			else
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent(passenger, ref m_CurrentLaneTypes);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Unspawned>(passenger, default(Unspawned));
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(passenger, default(Updated));
			if (ticketPrice != 0 && m_Resources.HasBuffer(household) && m_PlayerMoney.HasComponent(m_City))
			{
				DynamicBuffer<Resources> resources = m_Resources[household];
				EconomyUtils.AddResources(Resource.Money, -ticketPrice, resources);
				PlayerMoney playerMoney = m_PlayerMoney[m_City];
				playerMoney.Add(ticketPrice);
				m_PlayerMoney[m_City] = playerMoney;
				m_FeeQueue.Enqueue(new ServiceFeeSystem.FeeEvent
				{
					m_Amount = 1f,
					m_Cost = ticketPrice,
					m_Resource = PlayerResource.PublicTransport,
					m_Outside = false
				});
			}
		}

		private bool TryGetRelativeLocation(Entity prefab, out Relative relative)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			relative = default(Relative);
			DynamicBuffer<ActivityLocationElement> val = default(DynamicBuffer<ActivityLocationElement>);
			if (m_ActivityLocations.TryGetBuffer(prefab, ref val))
			{
				ActivityMask activityMask = new ActivityMask(ActivityType.Driving);
				for (int i = 0; i < val.Length; i++)
				{
					ActivityLocationElement activityLocationElement = val[i];
					if ((activityLocationElement.m_ActivityMask.m_Mask & activityMask.m_Mask) != 0)
					{
						relative.m_Position = activityLocationElement.m_Position;
						relative.m_Rotation = activityLocationElement.m_Rotation;
						relative.m_BoneIndex = new int3(0, -1, -1);
						return true;
					}
				}
			}
			return false;
		}

		private void RequireStop(ref NativeParallelHashMap<Entity, int3> freeSpaceMap, Entity passenger, Entity vehicle, float3 position)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			if (passenger != Entity.Null)
			{
				int requiredSpace = 1 + GetPendingGroupMemberCount(passenger);
				if (TryFindVehicle(ref freeSpaceMap, vehicle, Entity.Null, position, isLeader: true, requiredSpace, out var _) == Entity.Null)
				{
					return;
				}
			}
			Game.Vehicles.PublicTransport publicTransport = default(Game.Vehicles.PublicTransport);
			if (m_PublicTransports.TryGetComponent(vehicle, ref publicTransport))
			{
				publicTransport.m_State |= PublicTransportFlags.RequireStop;
				m_PublicTransports[vehicle] = publicTransport;
			}
		}

		private void WaitTimeExceeded(Entity passenger, Entity waypoint)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			if (m_WaitingPassengers.HasComponent(waypoint))
			{
				int num = 1 + GetPendingGroupMemberCount(passenger);
				ref WaitingPassengers valueRW = ref m_WaitingPassengers.GetRefRW(waypoint).ValueRW;
				int num2 = (int)((float)(5000 * num) * (2f / 15f));
				valueRW.m_ConcludedAccumulation += num2;
			}
		}

		private void WaitTimeEstimate(Entity waypoint, int seconds)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (m_WaitingPassengers.HasComponent(waypoint))
			{
				m_WaitingPassengers.GetRefRW(waypoint).ValueRW.m_ConcludedAccumulation += seconds;
			}
		}

		private void EnqueueStat(StatisticType statisticType, int change, int parameter)
		{
			m_StatisticsEventQueue.Enqueue(new StatisticsEvent
			{
				m_Statistic = statisticType,
				m_Change = change,
				m_Parameter = parameter
			});
		}
	}

	[BurstCompile]
	private struct ResidentActionJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<MailBoxData> m_PrefabMailBoxData;

		public ComponentLookup<MailSender> m_MailSenderData;

		public ComponentLookup<HouseholdNeed> m_HouseholdNeedData;

		public ComponentLookup<Game.Routes.MailBox> m_MailBoxData;

		public NativeQueue<ResidentAction> m_ActionQueue;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			int count = m_ActionQueue.Count;
			for (int i = 0; i < count; i++)
			{
				ResidentAction action = m_ActionQueue.Dequeue();
				switch (action.m_Type)
				{
				case ResidentActionType.SendMail:
					SendMail(action);
					break;
				case ResidentActionType.GoShopping:
					GoShopping(action);
					break;
				}
			}
		}

		private void SendMail(ResidentAction action)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			MailSender mailSender = default(MailSender);
			Game.Routes.MailBox mailBox = default(Game.Routes.MailBox);
			PrefabRef prefabRef = default(PrefabRef);
			MailBoxData mailBoxData = default(MailBoxData);
			if (!EntitiesExtensions.TryGetEnabledComponent<MailSender>(m_MailSenderData, action.m_Citizen, ref mailSender) || !m_MailBoxData.TryGetComponent(action.m_Target, ref mailBox) || !m_PrefabRefData.TryGetComponent(action.m_Target, ref prefabRef) || !m_PrefabMailBoxData.TryGetComponent(prefabRef.m_Prefab, ref mailBoxData))
			{
				return;
			}
			int num = math.min((int)mailSender.m_Amount, mailBoxData.m_MailCapacity - mailBox.m_MailAmount);
			if (num > 0)
			{
				mailSender.m_Amount = (ushort)(mailSender.m_Amount - num);
				mailBox.m_MailAmount += num;
				m_MailSenderData[action.m_Citizen] = mailSender;
				m_MailBoxData[action.m_Target] = mailBox;
				if (mailSender.m_Amount == 0)
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponentEnabled<MailSender>(action.m_Citizen, false);
				}
			}
		}

		private void GoShopping(ResidentAction action)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			if (m_HouseholdNeedData.HasComponent(action.m_Household))
			{
				HouseholdNeed householdNeed = m_HouseholdNeedData[action.m_Household];
				householdNeed.m_Resource = Resource.NoResource;
				m_HouseholdNeedData[action.m_Household] = householdNeed;
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<ResourceBought>(action.m_Citizen, new ResourceBought
			{
				m_Seller = action.m_Target,
				m_Payer = action.m_Household,
				m_Resource = action.m_Resource,
				m_Amount = action.m_Amount,
				m_Distance = action.m_Distance
			});
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentVehicle> __Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<GroupMember> __Game_Creatures_GroupMember_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Unspawned> __Game_Objects_Unspawned_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<HumanNavigation> __Game_Creatures_HumanNavigation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<GroupCreature> __Game_Creatures_GroupCreature_RO_BufferTypeHandle;

		public ComponentTypeHandle<Creature> __Game_Creatures_Creature_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Human> __Game_Creatures_Human_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Game.Creatures.Resident> __Game_Creatures_Resident_RW_ComponentTypeHandle;

		public ComponentTypeHandle<HumanCurrentLane> __Game_Creatures_HumanCurrentLane_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Target> __Game_Common_Target_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Divert> __Game_Creatures_Divert_RW_ComponentTypeHandle;

		[ReadOnly]
		public EntityStorageInfoLookup __EntityStorageInfoLookup;

		public ComponentLookup<Human> __Game_Creatures_Human_RW_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> __Game_Creatures_CurrentVehicle_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Destroyed> __Game_Common_Destroyed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Unspawned> __Game_Objects_Unspawned_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RideNeeder> __Game_Creatures_RideNeeder_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Moving> __Game_Objects_Moving_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> __Game_Objects_SpawnLocation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Animal> __Game_Creatures_Animal_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Dispatched> __Game_Simulation_Dispatched_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ServiceRequest> __Game_Simulation_ServiceRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<OnFire> __Game_Events_OnFire_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Lane> __Game_Net_Lane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeLane> __Game_Net_EdgeLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> __Game_Net_ParkingLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<GarageLane> __Game_Net_GarageLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.PedestrianLane> __Game_Net_PedestrianLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> __Game_Net_ConnectionLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HangaroundLocation> __Game_Areas_HangaroundLocation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HouseholdMember> __Game_Citizens_HouseholdMember_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Household> __Game_Citizens_Household_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentBuilding> __Game_Citizens_CurrentBuilding_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentTransport> __Game_Citizens_CurrentTransport_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Worker> __Game_Citizens_Worker_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarKeeper> __Game_Citizens_CarKeeper_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HealthProblem> __Game_Citizens_HealthProblem_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TravelPurpose> __Game_Citizens_TravelPurpose_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HomelessHousehold> __Game_Citizens_HomelessHousehold_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TouristHousehold> __Game_Citizens_TouristHousehold_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HouseholdNeed> __Game_Citizens_HouseholdNeed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AttendingMeeting> __Game_Citizens_AttendingMeeting_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CoordinatedMeeting> __Game_Citizens_CoordinatedMeeting_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MovingAway> __Game_Agents_MovingAway_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ServiceAvailable> __Game_Companies_ServiceAvailable_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkedCar> __Game_Vehicles_ParkedCar_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PersonalCar> __Game_Vehicles_PersonalCar_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.Taxi> __Game_Vehicles_Taxi_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PublicTransport> __Game_Vehicles_PublicTransport_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PoliceCar> __Game_Vehicles_PoliceCar_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.Ambulance> __Game_Vehicles_Ambulance_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.Hearse> __Game_Vehicles_Hearse_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Controller> __Game_Vehicles_Controller_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Vehicle> __Game_Vehicles_Vehicle_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Train> __Game_Vehicles_Train_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AttractivenessProvider> __Game_Buildings_AttractivenessProvider_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Connected> __Game_Routes_Connected_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BoardingVehicle> __Game_Routes_BoardingVehicle_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentRoute> __Game_Routes_CurrentRoute_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TransportLine> __Game_Routes_TransportLine_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AccessLane> __Game_Routes_AccessLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CreatureData> __Game_Prefabs_CreatureData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HumanData> __Game_Prefabs_HumanData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarData> __Game_Prefabs_CarData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> __Game_Prefabs_IndustrialProcessData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TransportStopData> __Game_Prefabs_TransportStopData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> __Game_Prefabs_SpawnLocationData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<HouseholdAnimal> __Game_Citizens_HouseholdAnimal_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedRoute> __Game_Routes_ConnectedRoute_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<CarNavigationLane> __Game_Vehicles_CarNavigationLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> __Game_Areas_Node_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Triangle> __Game_Areas_Triangle_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedBuilding> __Game_Buildings_ConnectedBuilding_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Renter> __Game_Buildings_Renter_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SpawnLocationElement> __Game_Buildings_SpawnLocationElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Resources> __Game_Economy_Resources_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ServiceDispatch> __Game_Simulation_ServiceDispatch_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ActivityLocationElement> __Game_Prefabs_ActivityLocationElement_RO_BufferLookup;

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
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
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
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0404: Unknown result type (might be due to invalid IL or missing references)
			//IL_0409: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_0416: Unknown result type (might be due to invalid IL or missing references)
			//IL_041e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0423: Unknown result type (might be due to invalid IL or missing references)
			//IL_042b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0430: Unknown result type (might be due to invalid IL or missing references)
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_043d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0445: Unknown result type (might be due to invalid IL or missing references)
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0452: Unknown result type (might be due to invalid IL or missing references)
			//IL_0457: Unknown result type (might be due to invalid IL or missing references)
			//IL_045f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0464: Unknown result type (might be due to invalid IL or missing references)
			//IL_046c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0471: Unknown result type (might be due to invalid IL or missing references)
			//IL_0479: Unknown result type (might be due to invalid IL or missing references)
			//IL_047e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0486: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0493: Unknown result type (might be due to invalid IL or missing references)
			//IL_0498: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentVehicle>(true);
			__Game_Creatures_GroupMember_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<GroupMember>(true);
			__Game_Objects_Unspawned_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Unspawned>(true);
			__Game_Creatures_HumanNavigation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HumanNavigation>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Creatures_GroupCreature_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<GroupCreature>(true);
			__Game_Creatures_Creature_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Creature>(false);
			__Game_Creatures_Human_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Human>(false);
			__Game_Creatures_Resident_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Creatures.Resident>(false);
			__Game_Creatures_HumanCurrentLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HumanCurrentLane>(false);
			__Game_Common_Target_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Target>(false);
			__Game_Creatures_Divert_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Divert>(false);
			__EntityStorageInfoLookup = ((SystemState)(ref state)).GetEntityStorageInfoLookup();
			__Game_Creatures_Human_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Human>(false);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Creatures_CurrentVehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentVehicle>(true);
			__Game_Common_Destroyed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Destroyed>(true);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Objects_Unspawned_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Unspawned>(true);
			__Game_Creatures_RideNeeder_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RideNeeder>(true);
			__Game_Objects_Moving_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Moving>(true);
			__Game_Objects_SpawnLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.SpawnLocation>(true);
			__Game_Creatures_Animal_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Animal>(true);
			__Game_Simulation_Dispatched_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Dispatched>(true);
			__Game_Simulation_ServiceRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceRequest>(true);
			__Game_Events_OnFire_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<OnFire>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Edge>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_Lane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(true);
			__Game_Net_EdgeLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeLane>(true);
			__Game_Net_ParkingLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ParkingLane>(true);
			__Game_Net_GarageLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarageLane>(true);
			__Game_Net_PedestrianLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.PedestrianLane>(true);
			__Game_Net_ConnectionLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ConnectionLane>(true);
			__Game_Areas_HangaroundLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HangaroundLocation>(true);
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
			__Game_Citizens_HouseholdMember_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HouseholdMember>(true);
			__Game_Citizens_Household_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Household>(true);
			__Game_Citizens_CurrentBuilding_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentBuilding>(true);
			__Game_Citizens_CurrentTransport_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentTransport>(true);
			__Game_Citizens_Worker_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Worker>(true);
			__Game_Citizens_CarKeeper_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarKeeper>(true);
			__Game_Citizens_HealthProblem_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HealthProblem>(true);
			__Game_Citizens_TravelPurpose_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TravelPurpose>(true);
			__Game_Citizens_HomelessHousehold_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HomelessHousehold>(true);
			__Game_Citizens_TouristHousehold_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TouristHousehold>(true);
			__Game_Citizens_HouseholdNeed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HouseholdNeed>(true);
			__Game_Citizens_AttendingMeeting_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AttendingMeeting>(true);
			__Game_Citizens_CoordinatedMeeting_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CoordinatedMeeting>(true);
			__Game_Agents_MovingAway_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MovingAway>(true);
			__Game_Companies_ServiceAvailable_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceAvailable>(true);
			__Game_Vehicles_ParkedCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedCar>(true);
			__Game_Vehicles_PersonalCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.PersonalCar>(true);
			__Game_Vehicles_Taxi_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.Taxi>(true);
			__Game_Vehicles_PublicTransport_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.PublicTransport>(true);
			__Game_Vehicles_PoliceCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.PoliceCar>(true);
			__Game_Vehicles_Ambulance_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.Ambulance>(true);
			__Game_Vehicles_Hearse_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.Hearse>(true);
			__Game_Vehicles_Controller_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Controller>(true);
			__Game_Vehicles_Vehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Vehicle>(true);
			__Game_Vehicles_Train_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Train>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
			__Game_Buildings_AttractivenessProvider_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AttractivenessProvider>(true);
			__Game_Routes_Connected_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Connected>(true);
			__Game_Routes_BoardingVehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BoardingVehicle>(true);
			__Game_Routes_CurrentRoute_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentRoute>(true);
			__Game_Routes_TransportLine_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TransportLine>(true);
			__Game_Routes_AccessLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AccessLane>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_CreatureData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CreatureData>(true);
			__Game_Prefabs_HumanData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HumanData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_CarData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarData>(true);
			__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<IndustrialProcessData>(true);
			__Game_Prefabs_TransportStopData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TransportStopData>(true);
			__Game_Prefabs_SpawnLocationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnLocationData>(true);
			__Game_Citizens_HouseholdAnimal_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdAnimal>(true);
			__Game_Citizens_HouseholdCitizen_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdCitizen>(true);
			__Game_Routes_ConnectedRoute_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedRoute>(true);
			__Game_Vehicles_LayoutElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(true);
			__Game_Vehicles_CarNavigationLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CarNavigationLane>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Areas_Node_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.Node>(true);
			__Game_Areas_Triangle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Triangle>(true);
			__Game_Buildings_ConnectedBuilding_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedBuilding>(true);
			__Game_Buildings_Renter_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Renter>(true);
			__Game_Buildings_SpawnLocationElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SpawnLocationElement>(true);
			__Game_Economy_Resources_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Resources>(true);
			__Game_Simulation_ServiceDispatch_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceDispatch>(true);
			__Game_Prefabs_ActivityLocationElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ActivityLocationElement>(true);
			__Game_Pathfind_PathOwner_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathOwner>(false);
			__Game_Pathfind_PathElement_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PathElement>(false);
		}
	}

	private EndFrameBarrier m_EndFrameBarrier;

	private SimulationSystem m_SimulationSystem;

	private PathfindSetupSystem m_PathfindSetupSystem;

	private TimeSystem m_TimeSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private Actions m_Actions;

	private PersonalCarSelectData m_PersonalCarSelectData;

	private EntityQuery m_CreatureQuery;

	private EntityQuery m_GroupCreatureQuery;

	private EntityQuery m_CarPrefabQuery;

	private EntityArchetype m_ResetTripArchetype;

	private ComponentTypeSet m_ParkedToMovingCarRemoveTypes;

	private ComponentTypeSet m_ParkedToMovingCarAddTypes;

	private ComponentTypeSet m_ParkedToMovingTrailerAddTypes;

	[EnumArray(typeof(DeletedResidentType))]
	[DebugWatchValue]
	private NativeArray<int> m_DeletedResidents;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_PathfindSetupSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindSetupSystem>();
		m_TimeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TimeSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_Actions = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Actions>();
		m_PersonalCarSelectData = new PersonalCarSelectData((SystemBase)(object)this);
		m_CreatureQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadWrite<Game.Creatures.Resident>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.Exclude<GroupMember>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Stumbling>()
		});
		m_GroupCreatureQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadWrite<Game.Creatures.Resident>(),
			ComponentType.ReadOnly<GroupMember>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Stumbling>()
		});
		m_CarPrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)new EntityQueryDesc[1] { PersonalCarSelectData.GetEntityQueryDesc() });
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_ResetTripArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Game.Common.Event>(),
			ComponentType.ReadWrite<ResetTrip>()
		});
		m_ParkedToMovingCarRemoveTypes = new ComponentTypeSet(ComponentType.ReadWrite<ParkedCar>(), ComponentType.ReadWrite<Stopped>());
		m_ParkedToMovingCarAddTypes = new ComponentTypeSet((ComponentType[])(object)new ComponentType[12]
		{
			ComponentType.ReadWrite<Moving>(),
			ComponentType.ReadWrite<TransformFrame>(),
			ComponentType.ReadWrite<InterpolatedTransform>(),
			ComponentType.ReadWrite<CarNavigation>(),
			ComponentType.ReadWrite<CarNavigationLane>(),
			ComponentType.ReadWrite<CarCurrentLane>(),
			ComponentType.ReadWrite<PathOwner>(),
			ComponentType.ReadWrite<Target>(),
			ComponentType.ReadWrite<Blocker>(),
			ComponentType.ReadWrite<PathElement>(),
			ComponentType.ReadWrite<Swaying>(),
			ComponentType.ReadWrite<Updated>()
		});
		m_ParkedToMovingTrailerAddTypes = new ComponentTypeSet((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadWrite<Moving>(),
			ComponentType.ReadWrite<TransformFrame>(),
			ComponentType.ReadWrite<InterpolatedTransform>(),
			ComponentType.ReadWrite<CarTrailerLane>(),
			ComponentType.ReadWrite<Swaying>(),
			ComponentType.ReadWrite<Updated>()
		});
		m_DeletedResidents = new NativeArray<int>(7, (Allocator)4, (NativeArrayOptions)1);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_DeletedResidents.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_053d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_0577: Unknown result type (might be due to invalid IL or missing references)
		//IL_058f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0594: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0603: Unknown result type (might be due to invalid IL or missing references)
		//IL_0608: Unknown result type (might be due to invalid IL or missing references)
		//IL_0620: Unknown result type (might be due to invalid IL or missing references)
		//IL_0625: Unknown result type (might be due to invalid IL or missing references)
		//IL_063d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0642: Unknown result type (might be due to invalid IL or missing references)
		//IL_065a: Unknown result type (might be due to invalid IL or missing references)
		//IL_065f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0677: Unknown result type (might be due to invalid IL or missing references)
		//IL_067c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0694: Unknown result type (might be due to invalid IL or missing references)
		//IL_0699: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0708: Unknown result type (might be due to invalid IL or missing references)
		//IL_070d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0725: Unknown result type (might be due to invalid IL or missing references)
		//IL_072a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0742: Unknown result type (might be due to invalid IL or missing references)
		//IL_0747: Unknown result type (might be due to invalid IL or missing references)
		//IL_075f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0764: Unknown result type (might be due to invalid IL or missing references)
		//IL_077c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0781: Unknown result type (might be due to invalid IL or missing references)
		//IL_0799: Unknown result type (might be due to invalid IL or missing references)
		//IL_079e: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_080d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0812: Unknown result type (might be due to invalid IL or missing references)
		//IL_082a: Unknown result type (might be due to invalid IL or missing references)
		//IL_082f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0847: Unknown result type (might be due to invalid IL or missing references)
		//IL_084c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0864: Unknown result type (might be due to invalid IL or missing references)
		//IL_0869: Unknown result type (might be due to invalid IL or missing references)
		//IL_0881: Unknown result type (might be due to invalid IL or missing references)
		//IL_0886: Unknown result type (might be due to invalid IL or missing references)
		//IL_089e: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0912: Unknown result type (might be due to invalid IL or missing references)
		//IL_0917: Unknown result type (might be due to invalid IL or missing references)
		//IL_092f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0934: Unknown result type (might be due to invalid IL or missing references)
		//IL_094c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0951: Unknown result type (might be due to invalid IL or missing references)
		//IL_0969: Unknown result type (might be due to invalid IL or missing references)
		//IL_096e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0986: Unknown result type (might be due to invalid IL or missing references)
		//IL_098b: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a17: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a34: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a39: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a51: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a56: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a73: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a90: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b04: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b21: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b39: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b90: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0baa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bbf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c07: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c23: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c35: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c39: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c48: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c53: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c54: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c59: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c5e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c69: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c6f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c74: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c89: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c96: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cab: Unknown result type (might be due to invalid IL or missing references)
		uint index = m_SimulationSystem.frameIndex % 16;
		((EntityQuery)(ref m_CreatureQuery)).SetSharedComponentFilter<UpdateFrame>(new UpdateFrame(index));
		((EntityQuery)(ref m_GroupCreatureQuery)).SetSharedComponentFilter<UpdateFrame>(new UpdateFrame(index));
		m_Actions.m_BoardingQueue = new NativeQueue<Boarding>(AllocatorHandle.op_Implicit((Allocator)3));
		m_Actions.m_ActionQueue = new NativeQueue<ResidentAction>(AllocatorHandle.op_Implicit((Allocator)3));
		m_PersonalCarSelectData.PreUpdate((SystemBase)(object)this, m_CityConfigurationSystem, m_CarPrefabQuery, (Allocator)3, out var jobHandle);
		ResidentTickJob residentTickJob = new ResidentTickJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentVehicleType = InternalCompilerInterface.GetComponentTypeHandle<CurrentVehicle>(ref __TypeHandle.__Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_GroupMemberType = InternalCompilerInterface.GetComponentTypeHandle<GroupMember>(ref __TypeHandle.__Game_Creatures_GroupMember_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UnspawnedType = InternalCompilerInterface.GetComponentTypeHandle<Unspawned>(ref __TypeHandle.__Game_Objects_Unspawned_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HumanNavigationType = InternalCompilerInterface.GetComponentTypeHandle<HumanNavigation>(ref __TypeHandle.__Game_Creatures_HumanNavigation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_GroupCreatureType = InternalCompilerInterface.GetBufferTypeHandle<GroupCreature>(ref __TypeHandle.__Game_Creatures_GroupCreature_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CreatureType = InternalCompilerInterface.GetComponentTypeHandle<Creature>(ref __TypeHandle.__Game_Creatures_Creature_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HumanType = InternalCompilerInterface.GetComponentTypeHandle<Human>(ref __TypeHandle.__Game_Creatures_Human_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResidentType = InternalCompilerInterface.GetComponentTypeHandle<Game.Creatures.Resident>(ref __TypeHandle.__Game_Creatures_Resident_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<HumanCurrentLane>(ref __TypeHandle.__Game_Creatures_HumanCurrentLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TargetType = InternalCompilerInterface.GetComponentTypeHandle<Target>(ref __TypeHandle.__Game_Common_Target_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DivertType = InternalCompilerInterface.GetComponentTypeHandle<Divert>(ref __TypeHandle.__Game_Creatures_Divert_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EntityLookup = InternalCompilerInterface.GetEntityStorageInfoLookup(ref __TypeHandle.__EntityStorageInfoLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HumanData = InternalCompilerInterface.GetComponentLookup<Human>(ref __TypeHandle.__Game_Creatures_Human_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentVehicleData = InternalCompilerInterface.GetComponentLookup<CurrentVehicle>(ref __TypeHandle.__Game_Creatures_CurrentVehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DestroyedData = InternalCompilerInterface.GetComponentLookup<Destroyed>(ref __TypeHandle.__Game_Common_Destroyed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UnspawnedData = InternalCompilerInterface.GetComponentLookup<Unspawned>(ref __TypeHandle.__Game_Objects_Unspawned_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RideNeederData = InternalCompilerInterface.GetComponentLookup<RideNeeder>(ref __TypeHandle.__Game_Creatures_RideNeeder_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MovingData = InternalCompilerInterface.GetComponentLookup<Moving>(ref __TypeHandle.__Game_Objects_Moving_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnLocation = InternalCompilerInterface.GetComponentLookup<Game.Objects.SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AnimalData = InternalCompilerInterface.GetComponentLookup<Animal>(ref __TypeHandle.__Game_Creatures_Animal_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Dispatched = InternalCompilerInterface.GetComponentLookup<Dispatched>(ref __TypeHandle.__Game_Simulation_Dispatched_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceRequestData = InternalCompilerInterface.GetComponentLookup<ServiceRequest>(ref __TypeHandle.__Game_Simulation_ServiceRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OnFireData = InternalCompilerInterface.GetComponentLookup<OnFire>(ref __TypeHandle.__Game_Events_OnFire_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Game.Net.Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeLaneData = InternalCompilerInterface.GetComponentLookup<EdgeLane>(ref __TypeHandle.__Game_Net_EdgeLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkingLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ParkingLane>(ref __TypeHandle.__Game_Net_ParkingLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GarageLaneData = InternalCompilerInterface.GetComponentLookup<GarageLane>(ref __TypeHandle.__Game_Net_GarageLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PedestrianLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.PedestrianLane>(ref __TypeHandle.__Game_Net_PedestrianLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectionLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HangaroundLocationData = InternalCompilerInterface.GetComponentLookup<HangaroundLocation>(ref __TypeHandle.__Game_Areas_HangaroundLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CitizenData = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdMembers = InternalCompilerInterface.GetComponentLookup<HouseholdMember>(ref __TypeHandle.__Game_Citizens_HouseholdMember_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdData = InternalCompilerInterface.GetComponentLookup<Household>(ref __TypeHandle.__Game_Citizens_Household_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentBuildingData = InternalCompilerInterface.GetComponentLookup<CurrentBuilding>(ref __TypeHandle.__Game_Citizens_CurrentBuilding_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentTransportData = InternalCompilerInterface.GetComponentLookup<CurrentTransport>(ref __TypeHandle.__Game_Citizens_CurrentTransport_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WorkerData = InternalCompilerInterface.GetComponentLookup<Worker>(ref __TypeHandle.__Game_Citizens_Worker_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarKeeperData = InternalCompilerInterface.GetComponentLookup<CarKeeper>(ref __TypeHandle.__Game_Citizens_CarKeeper_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HealthProblemData = InternalCompilerInterface.GetComponentLookup<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TravelPurposeData = InternalCompilerInterface.GetComponentLookup<TravelPurpose>(ref __TypeHandle.__Game_Citizens_TravelPurpose_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HomelessHouseholdData = InternalCompilerInterface.GetComponentLookup<HomelessHousehold>(ref __TypeHandle.__Game_Citizens_HomelessHousehold_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TouristHouseholds = InternalCompilerInterface.GetComponentLookup<TouristHousehold>(ref __TypeHandle.__Game_Citizens_TouristHousehold_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdNeedData = InternalCompilerInterface.GetComponentLookup<HouseholdNeed>(ref __TypeHandle.__Game_Citizens_HouseholdNeed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AttendingMeetingData = InternalCompilerInterface.GetComponentLookup<AttendingMeeting>(ref __TypeHandle.__Game_Citizens_AttendingMeeting_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CoordinatedMeetingData = InternalCompilerInterface.GetComponentLookup<CoordinatedMeeting>(ref __TypeHandle.__Game_Citizens_CoordinatedMeeting_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MovingAwayData = InternalCompilerInterface.GetComponentLookup<MovingAway>(ref __TypeHandle.__Game_Agents_MovingAway_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceAvailableData = InternalCompilerInterface.GetComponentLookup<ServiceAvailable>(ref __TypeHandle.__Game_Companies_ServiceAvailable_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedCarData = InternalCompilerInterface.GetComponentLookup<ParkedCar>(ref __TypeHandle.__Game_Vehicles_ParkedCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PersonalCarData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.PersonalCar>(ref __TypeHandle.__Game_Vehicles_PersonalCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TaxiData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.Taxi>(ref __TypeHandle.__Game_Vehicles_Taxi_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PublicTransportData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.PublicTransport>(ref __TypeHandle.__Game_Vehicles_PublicTransport_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PoliceCarData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.PoliceCar>(ref __TypeHandle.__Game_Vehicles_PoliceCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AmbulanceData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.Ambulance>(ref __TypeHandle.__Game_Vehicles_Ambulance_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HearseData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.Hearse>(ref __TypeHandle.__Game_Vehicles_Hearse_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ControllerData = InternalCompilerInterface.GetComponentLookup<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_VehicleData = InternalCompilerInterface.GetComponentLookup<Vehicle>(ref __TypeHandle.__Game_Vehicles_Vehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrainData = InternalCompilerInterface.GetComponentLookup<Train>(ref __TypeHandle.__Game_Vehicles_Train_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyRenters = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AttractivenessProviderData = InternalCompilerInterface.GetComponentLookup<AttractivenessProvider>(ref __TypeHandle.__Game_Buildings_AttractivenessProvider_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteConnectedData = InternalCompilerInterface.GetComponentLookup<Connected>(ref __TypeHandle.__Game_Routes_Connected_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BoardingVehicleData = InternalCompilerInterface.GetComponentLookup<BoardingVehicle>(ref __TypeHandle.__Game_Routes_BoardingVehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentRouteData = InternalCompilerInterface.GetComponentLookup<CurrentRoute>(ref __TypeHandle.__Game_Routes_CurrentRoute_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransportLineData = InternalCompilerInterface.GetComponentLookup<TransportLine>(ref __TypeHandle.__Game_Routes_TransportLine_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AccessLaneLaneData = InternalCompilerInterface.GetComponentLookup<AccessLane>(ref __TypeHandle.__Game_Routes_AccessLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCreatureData = InternalCompilerInterface.GetComponentLookup<CreatureData>(ref __TypeHandle.__Game_Prefabs_CreatureData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabHumanData = InternalCompilerInterface.GetComponentLookup<HumanData>(ref __TypeHandle.__Game_Prefabs_HumanData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCarData = InternalCompilerInterface.GetComponentLookup<CarData>(ref __TypeHandle.__Game_Prefabs_CarData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabIndustrialProcessData = InternalCompilerInterface.GetComponentLookup<IndustrialProcessData>(ref __TypeHandle.__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTransportStopData = InternalCompilerInterface.GetComponentLookup<TransportStopData>(ref __TypeHandle.__Game_Prefabs_TransportStopData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSpawnLocationData = InternalCompilerInterface.GetComponentLookup<SpawnLocationData>(ref __TypeHandle.__Game_Prefabs_SpawnLocationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdAnimals = InternalCompilerInterface.GetBufferLookup<HouseholdAnimal>(ref __TypeHandle.__Game_Citizens_HouseholdAnimal_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdCitizens = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedRoutes = InternalCompilerInterface.GetBufferLookup<ConnectedRoute>(ref __TypeHandle.__Game_Routes_ConnectedRoute_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_VehicleLayouts = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarNavigationLanes = InternalCompilerInterface.GetBufferLookup<CarNavigationLane>(ref __TypeHandle.__Game_Vehicles_CarNavigationLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubLanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaNodes = InternalCompilerInterface.GetBufferLookup<Game.Areas.Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaTriangles = InternalCompilerInterface.GetBufferLookup<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedBuildings = InternalCompilerInterface.GetBufferLookup<ConnectedBuilding>(ref __TypeHandle.__Game_Buildings_ConnectedBuilding_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Renters = InternalCompilerInterface.GetBufferLookup<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnLocationElements = InternalCompilerInterface.GetBufferLookup<SpawnLocationElement>(ref __TypeHandle.__Game_Buildings_SpawnLocationElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Resources = InternalCompilerInterface.GetBufferLookup<Resources>(ref __TypeHandle.__Game_Economy_Resources_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceDispatches = InternalCompilerInterface.GetBufferLookup<ServiceDispatch>(ref __TypeHandle.__Game_Simulation_ServiceDispatch_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabActivityLocationElements = InternalCompilerInterface.GetBufferLookup<ActivityLocationElement>(ref __TypeHandle.__Game_Prefabs_ActivityLocationElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathOwnerData = InternalCompilerInterface.GetComponentLookup<PathOwner>(ref __TypeHandle.__Game_Pathfind_PathOwner_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathElements = InternalCompilerInterface.GetBufferLookup<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RandomSeed = RandomSeed.Next(),
			m_SimulationFrameIndex = m_SimulationSystem.frameIndex,
			m_LefthandTraffic = m_CityConfigurationSystem.leftHandTraffic,
			m_GroupMember = false,
			m_PersonalCarSelectData = m_PersonalCarSelectData,
			m_ResetTripArchetype = m_ResetTripArchetype,
			m_ParkedToMovingCarRemoveTypes = m_ParkedToMovingCarRemoveTypes,
			m_ParkedToMovingCarAddTypes = m_ParkedToMovingCarAddTypes,
			m_ParkedToMovingTrailerAddTypes = m_ParkedToMovingTrailerAddTypes,
			m_DeletedResidents = m_DeletedResidents,
			m_TimeOfDay = m_TimeSystem.normalizedTime,
			m_PathfindQueue = m_PathfindSetupSystem.GetQueue(this, 64).AsParallelWriter(),
			m_BoardingQueue = m_Actions.m_BoardingQueue.AsParallelWriter(),
			m_ActionQueue = m_Actions.m_ActionQueue.AsParallelWriter()
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		residentTickJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		ResidentTickJob residentTickJob2 = residentTickJob;
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<ResidentTickJob>(residentTickJob2, m_CreatureQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, jobHandle));
		residentTickJob2.m_GroupMember = true;
		JobHandle val3 = JobChunkExtensions.ScheduleParallel<ResidentTickJob>(residentTickJob2, m_GroupCreatureQuery, val2);
		m_PersonalCarSelectData.PostUpdate(val3);
		m_PathfindSetupSystem.AddQueueWriter(val3);
		m_EndFrameBarrier.AddJobHandleForProducer(val3);
		m_Actions.m_Dependency = val3;
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
	public ResidentAISystem()
	{
	}
}
