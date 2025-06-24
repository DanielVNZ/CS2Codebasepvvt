using System.Runtime.CompilerServices;
using Game.Common;
using Game.Creatures;
using Game.Pathfind;
using Game.Tools;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class StuckMovingObjectSystem : GameSystemBase
{
	[BurstCompile]
	private struct StuckCheckJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Blocker> m_BlockerType;

		[ReadOnly]
		public ComponentTypeHandle<GroupMember> m_GroupMemberType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentVehicle> m_CurrentVehicleType;

		[ReadOnly]
		public ComponentTypeHandle<RideNeeder> m_RideNeederType;

		[ReadOnly]
		public ComponentTypeHandle<Target> m_TargetType;

		[ReadOnly]
		public ComponentTypeHandle<Car> m_CarType;

		[ReadOnly]
		public ComponentLookup<Blocker> m_BlockerData;

		[ReadOnly]
		public ComponentLookup<Controller> m_ControllerData;

		[ReadOnly]
		public ComponentLookup<ParkedCar> m_ParkedCarData;

		[ReadOnly]
		public ComponentLookup<ParkedTrain> m_ParkedTrainData;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> m_CurrentVehicleData;

		[ReadOnly]
		public ComponentLookup<Dispatched> m_DispatchedData;

		public ComponentTypeHandle<PathOwner> m_PathOwnerType;

		public ComponentTypeHandle<AnimalCurrentLane> m_AnimalCurrentLaneType;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<CarCurrentLane> m_CarCurrentLaneData;

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
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Blocker> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Blocker>(ref m_BlockerType);
			NativeArray<GroupMember> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<GroupMember>(ref m_GroupMemberType);
			NativeArray<CurrentVehicle> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentVehicle>(ref m_CurrentVehicleType);
			NativeArray<RideNeeder> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<RideNeeder>(ref m_RideNeederType);
			NativeArray<Target> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Target>(ref m_TargetType);
			NativeArray<PathOwner> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathOwner>(ref m_PathOwnerType);
			NativeArray<AnimalCurrentLane> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AnimalCurrentLane>(ref m_AnimalCurrentLaneType);
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<Car>(ref m_CarType);
			Controller controller = default(Controller);
			CarCurrentLane carCurrentLane = default(CarCurrentLane);
			Dispatched dispatched = default(Dispatched);
			CurrentVehicle currentVehicle = default(CurrentVehicle);
			Controller controller2 = default(Controller);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Blocker blocker = nativeArray2[i];
				if (blocker.m_Blocker == Entity.Null || blocker.m_Type == BlockerType.Temporary)
				{
					continue;
				}
				if (flag && blocker.m_Type == BlockerType.Crossing)
				{
					Entity val = blocker.m_Blocker;
					if (m_ControllerData.TryGetComponent(val, ref controller))
					{
						val = controller.m_Controller;
					}
					if (m_CarCurrentLaneData.TryGetComponent(val, ref carCurrentLane))
					{
						carCurrentLane.m_LaneFlags |= CarLaneFlags.RequestSpace;
						m_CarCurrentLaneData[val] = carCurrentLane;
					}
				}
				if (blocker.m_MaxSpeed >= 6)
				{
					continue;
				}
				Entity val2 = nativeArray[i];
				Entity val3 = Entity.Null;
				bool flag2;
				if (m_ParkedTrainData.HasComponent(blocker.m_Blocker) || (!flag && m_ParkedCarData.HasComponent(blocker.m_Blocker)))
				{
					flag2 = true;
				}
				else
				{
					if (nativeArray4.Length != 0)
					{
						val3 = nativeArray4[i].m_Vehicle;
					}
					else if (nativeArray5.Length != 0)
					{
						RideNeeder rideNeeder = nativeArray5[i];
						if (m_DispatchedData.TryGetComponent(rideNeeder.m_RideRequest, ref dispatched))
						{
							val3 = dispatched.m_Handler;
						}
					}
					else if (nativeArray3.Length != 0)
					{
						GroupMember groupMember = nativeArray3[i];
						if (m_CurrentVehicleData.TryGetComponent(groupMember.m_Leader, ref currentVehicle))
						{
							val3 = currentVehicle.m_Vehicle;
						}
					}
					if (nativeArray6.Length != 0 && val3 == Entity.Null)
					{
						val3 = nativeArray6[i].m_Target;
					}
					if (val3 != Entity.Null)
					{
						if (m_ControllerData.TryGetComponent(val3, ref controller2))
						{
							val3 = controller2.m_Controller;
						}
						flag2 = IsBlocked(val2, val3, blocker);
					}
					else
					{
						flag2 = IsBlocked(val2, blocker);
					}
				}
				if (!flag2)
				{
					continue;
				}
				if (nativeArray7.Length != 0)
				{
					PathOwner pathOwner = nativeArray7[i];
					if ((pathOwner.m_State & PathFlags.Pending) == 0)
					{
						pathOwner.m_State |= PathFlags.Stuck;
						nativeArray7[i] = pathOwner;
					}
				}
				else if (nativeArray8.Length != 0)
				{
					AnimalCurrentLane animalCurrentLane = nativeArray8[i];
					animalCurrentLane.m_Flags |= CreatureLaneFlags.Stuck;
					nativeArray8[i] = animalCurrentLane;
				}
			}
		}

		private bool IsBlocked(Entity entity, Blocker blocker)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			Controller controller = default(Controller);
			if (m_ControllerData.TryGetComponent(blocker.m_Blocker, ref controller))
			{
				blocker.m_Blocker = controller.m_Controller;
			}
			Blocker blocker2 = default(Blocker);
			while (m_BlockerData.TryGetComponent(blocker.m_Blocker, ref blocker2))
			{
				if ((long)(++num) == 100 || blocker.m_Blocker == entity)
				{
					return true;
				}
				blocker = blocker2;
				if (blocker.m_Blocker == Entity.Null)
				{
					return false;
				}
				if (blocker.m_Type == BlockerType.Temporary)
				{
					return false;
				}
				if (blocker.m_MaxSpeed >= 6)
				{
					return false;
				}
				if (m_ControllerData.TryGetComponent(blocker.m_Blocker, ref controller))
				{
					blocker.m_Blocker = controller.m_Controller;
				}
			}
			return false;
		}

		private bool IsBlocked(Entity entity1, Entity entity2, Blocker blocker)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			Controller controller = default(Controller);
			if (m_ControllerData.TryGetComponent(blocker.m_Blocker, ref controller))
			{
				blocker.m_Blocker = controller.m_Controller;
			}
			Blocker blocker2 = default(Blocker);
			while (m_BlockerData.TryGetComponent(blocker.m_Blocker, ref blocker2))
			{
				if ((long)(++num) == 100 || blocker.m_Blocker == entity1 || blocker.m_Blocker == entity2)
				{
					return true;
				}
				blocker = blocker2;
				if (blocker.m_Blocker == Entity.Null)
				{
					return false;
				}
				if (blocker.m_Type == BlockerType.Temporary)
				{
					return false;
				}
				if (blocker.m_MaxSpeed >= 6)
				{
					return false;
				}
				if (m_ControllerData.TryGetComponent(blocker.m_Blocker, ref controller))
				{
					blocker.m_Blocker = controller.m_Controller;
				}
			}
			return false;
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
		public ComponentTypeHandle<Blocker> __Game_Vehicles_Blocker_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<GroupMember> __Game_Creatures_GroupMember_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentVehicle> __Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<RideNeeder> __Game_Creatures_RideNeeder_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Target> __Game_Common_Target_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Car> __Game_Vehicles_Car_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Blocker> __Game_Vehicles_Blocker_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Controller> __Game_Vehicles_Controller_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkedCar> __Game_Vehicles_ParkedCar_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkedTrain> __Game_Vehicles_ParkedTrain_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> __Game_Creatures_CurrentVehicle_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Dispatched> __Game_Simulation_Dispatched_RO_ComponentLookup;

		public ComponentTypeHandle<PathOwner> __Game_Pathfind_PathOwner_RW_ComponentTypeHandle;

		public ComponentTypeHandle<AnimalCurrentLane> __Game_Creatures_AnimalCurrentLane_RW_ComponentTypeHandle;

		public ComponentLookup<CarCurrentLane> __Game_Vehicles_CarCurrentLane_RW_ComponentLookup;

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
			__Game_Vehicles_Blocker_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Blocker>(true);
			__Game_Creatures_GroupMember_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<GroupMember>(true);
			__Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentVehicle>(true);
			__Game_Creatures_RideNeeder_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<RideNeeder>(true);
			__Game_Common_Target_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Target>(true);
			__Game_Vehicles_Car_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Car>(true);
			__Game_Vehicles_Blocker_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Blocker>(true);
			__Game_Vehicles_Controller_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Controller>(true);
			__Game_Vehicles_ParkedCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedCar>(true);
			__Game_Vehicles_ParkedTrain_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedTrain>(true);
			__Game_Creatures_CurrentVehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentVehicle>(true);
			__Game_Simulation_Dispatched_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Dispatched>(true);
			__Game_Pathfind_PathOwner_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathOwner>(false);
			__Game_Creatures_AnimalCurrentLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AnimalCurrentLane>(false);
			__Game_Vehicles_CarCurrentLane_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarCurrentLane>(false);
		}
	}

	private SimulationSystem m_SimulationSystem;

	private EntityQuery m_ObjectQuery;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 4;
	}

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
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_ObjectQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Blocker>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_ObjectQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		uint index = (m_SimulationSystem.frameIndex >> 2) % 16;
		((EntityQuery)(ref m_ObjectQuery)).ResetFilter();
		((EntityQuery)(ref m_ObjectQuery)).SetSharedComponentFilter<UpdateFrame>(new UpdateFrame(index));
		StuckCheckJob stuckCheckJob = new StuckCheckJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BlockerType = InternalCompilerInterface.GetComponentTypeHandle<Blocker>(ref __TypeHandle.__Game_Vehicles_Blocker_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_GroupMemberType = InternalCompilerInterface.GetComponentTypeHandle<GroupMember>(ref __TypeHandle.__Game_Creatures_GroupMember_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentVehicleType = InternalCompilerInterface.GetComponentTypeHandle<CurrentVehicle>(ref __TypeHandle.__Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RideNeederType = InternalCompilerInterface.GetComponentTypeHandle<RideNeeder>(ref __TypeHandle.__Game_Creatures_RideNeeder_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TargetType = InternalCompilerInterface.GetComponentTypeHandle<Target>(ref __TypeHandle.__Game_Common_Target_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CarType = InternalCompilerInterface.GetComponentTypeHandle<Car>(ref __TypeHandle.__Game_Vehicles_Car_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BlockerData = InternalCompilerInterface.GetComponentLookup<Blocker>(ref __TypeHandle.__Game_Vehicles_Blocker_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ControllerData = InternalCompilerInterface.GetComponentLookup<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedCarData = InternalCompilerInterface.GetComponentLookup<ParkedCar>(ref __TypeHandle.__Game_Vehicles_ParkedCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedTrainData = InternalCompilerInterface.GetComponentLookup<ParkedTrain>(ref __TypeHandle.__Game_Vehicles_ParkedTrain_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentVehicleData = InternalCompilerInterface.GetComponentLookup<CurrentVehicle>(ref __TypeHandle.__Game_Creatures_CurrentVehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DispatchedData = InternalCompilerInterface.GetComponentLookup<Dispatched>(ref __TypeHandle.__Game_Simulation_Dispatched_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathOwnerType = InternalCompilerInterface.GetComponentTypeHandle<PathOwner>(ref __TypeHandle.__Game_Pathfind_PathOwner_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AnimalCurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<AnimalCurrentLane>(ref __TypeHandle.__Game_Creatures_AnimalCurrentLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CarCurrentLaneData = InternalCompilerInterface.GetComponentLookup<CarCurrentLane>(ref __TypeHandle.__Game_Vehicles_CarCurrentLane_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<StuckCheckJob>(stuckCheckJob, m_ObjectQuery, ((SystemBase)this).Dependency);
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
	public StuckMovingObjectSystem()
	{
	}
}
