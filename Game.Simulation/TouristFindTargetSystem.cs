using System.Runtime.CompilerServices;
using Game.Agents;
using Game.Buildings;
using Game.Citizens;
using Game.Common;
using Game.Companies;
using Game.Events;
using Game.Pathfind;
using Game.Tools;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class TouristFindTargetSystem : GameSystemBase
{
	private struct HotelReserveAction
	{
		public Entity m_Household;

		public Entity m_Target;
	}

	[BurstCompile]
	private struct TouristFindTargetJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<TouristHousehold> m_HouseholdType;

		[ReadOnly]
		public ComponentLookup<CurrentBuilding> m_CurrentBuildings;

		[ReadOnly]
		public ComponentLookup<PathInformation> m_PathInformations;

		[ReadOnly]
		public ComponentLookup<LodgingProvider> m_LodgingProviders;

		[ReadOnly]
		public BufferLookup<Renter> m_RenterBufs;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> m_HouseholdCitizenBufs;

		[ReadOnly]
		public BufferLookup<OwnedVehicle> m_OwnedVehicleBufs;

		public ParallelWriter<SetupQueueItem> m_PathfindQueue;

		public ParallelWriter<AddMeetingSystem.AddMeeting> m_MeetingQueue;

		public ParallelWriter<HotelReserveAction> m_ReserveQueue;

		public ParallelWriter m_CommandBuffer;

		[ReadOnly]
		public ComponentTypeSet m_PathfindTypeSet;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				if (!m_PathInformations.HasComponent(val))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent(unfilteredChunkIndex, val, ref m_PathfindTypeSet);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PathInformation>(unfilteredChunkIndex, val, new PathInformation
					{
						m_State = PathFlags.Pending
					});
					PathfindParameters parameters = new PathfindParameters
					{
						m_MaxSpeed = float2.op_Implicit(277.77777f),
						m_WalkSpeed = float2.op_Implicit(1.6666667f),
						m_Weights = new PathfindWeights(0.1f, 0.1f, 0.1f, 0.2f),
						m_Methods = (PathMethod.PublicTransportDay | PathMethod.Taxi | PathMethod.PublicTransportNight),
						m_SecondaryIgnoredRules = VehicleUtils.GetIgnoredPathfindRulesTaxiDefaults(),
						m_PathfindFlags = (PathfindFlags.IgnoreFlow | PathfindFlags.Simplified | PathfindFlags.IgnorePath)
					};
					Entity entity = Entity.Null;
					for (int j = 0; j < m_HouseholdCitizenBufs[val].Length; j++)
					{
						if (m_CurrentBuildings.HasComponent(m_HouseholdCitizenBufs[val][j].m_Citizen))
						{
							entity = m_CurrentBuildings[m_HouseholdCitizenBufs[val][j].m_Citizen].m_CurrentBuilding;
						}
					}
					SetupQueueTarget origin = new SetupQueueTarget
					{
						m_Type = SetupTargetType.CurrentLocation,
						m_Methods = PathMethod.Pedestrian,
						m_Entity = entity
					};
					SetupQueueTarget destination = new SetupQueueTarget
					{
						m_Type = SetupTargetType.TouristFindTarget,
						m_Methods = PathMethod.Pedestrian,
						m_Entity = val
					};
					PathUtils.UpdateOwnedVehicleMethods(val, ref m_OwnedVehicleBufs, ref parameters, ref origin, ref destination);
					SetupQueueItem setupQueueItem = new SetupQueueItem(val, parameters, origin, destination);
					m_PathfindQueue.Enqueue(setupQueueItem);
					continue;
				}
				PathInformation pathInformation = m_PathInformations[val];
				if ((pathInformation.m_State & PathFlags.Pending) != 0)
				{
					continue;
				}
				Entity destination2 = pathInformation.m_Destination;
				if (destination2 != Entity.Null)
				{
					if (m_RenterBufs.HasBuffer(destination2) && m_RenterBufs[destination2].Length > 0 && m_LodgingProviders.HasComponent(m_RenterBufs[destination2][0].m_Renter))
					{
						m_ReserveQueue.Enqueue(new HotelReserveAction
						{
							m_Household = val,
							m_Target = m_RenterBufs[pathInformation.m_Destination][0].m_Renter
						});
					}
					else
					{
						m_MeetingQueue.Enqueue(new AddMeetingSystem.AddMeeting
						{
							m_Household = val,
							m_Type = LeisureType.Attractions
						});
					}
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Target>(unfilteredChunkIndex, val, new Target(destination2));
				}
				else
				{
					CitizenUtils.HouseholdMoveAway(m_CommandBuffer, unfilteredChunkIndex, val);
				}
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<PathInformation>(unfilteredChunkIndex, val);
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<LodgingSeeker>(unfilteredChunkIndex, val);
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct HotelReserveJob : IJob
	{
		public ComponentLookup<LodgingProvider> m_LodgingProviders;

		public ComponentLookup<TouristHousehold> m_TouristHouseholds;

		public BufferLookup<Renter> m_RenterBufs;

		public EntityCommandBuffer m_CommandBuffer;

		public NativeQueue<HotelReserveAction> m_ReserveQueue;

		public void Execute()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			HotelReserveAction hotelReserveAction = default(HotelReserveAction);
			while (m_ReserveQueue.TryDequeue(ref hotelReserveAction))
			{
				Entity val = hotelReserveAction.m_Target;
				Entity val2 = hotelReserveAction.m_Household;
				if (m_RenterBufs.HasBuffer(val) && m_LodgingProviders.HasComponent(val) && m_TouristHouseholds.HasComponent(val2))
				{
					DynamicBuffer<Renter> val3 = m_RenterBufs[val];
					LodgingProvider lodgingProvider = m_LodgingProviders[val];
					TouristHousehold touristHousehold = m_TouristHouseholds[val2];
					if (lodgingProvider.m_FreeRooms > 0)
					{
						lodgingProvider.m_FreeRooms--;
						m_LodgingProviders[val] = lodgingProvider;
						val3.Add(new Renter
						{
							m_Renter = val2
						});
						touristHousehold.m_Hotel = val;
						m_TouristHouseholds[val2] = touristHousehold;
						((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<LodgingSeeker>(val2);
					}
					else
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<LodgingSeeker>(val2);
					}
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TouristHousehold> __Game_Citizens_TouristHousehold_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<CurrentBuilding> __Game_Citizens_CurrentBuilding_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Renter> __Game_Buildings_Renter_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<OwnedVehicle> __Game_Vehicles_OwnedVehicle_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LodgingProvider> __Game_Companies_LodgingProvider_RO_ComponentLookup;

		public ComponentLookup<TouristHousehold> __Game_Citizens_TouristHousehold_RW_ComponentLookup;

		public ComponentLookup<LodgingProvider> __Game_Companies_LodgingProvider_RW_ComponentLookup;

		public BufferLookup<Renter> __Game_Buildings_Renter_RW_BufferLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Citizens_TouristHousehold_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TouristHousehold>(true);
			__Game_Citizens_CurrentBuilding_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentBuilding>(true);
			__Game_Buildings_Renter_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Renter>(true);
			__Game_Citizens_HouseholdCitizen_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdCitizen>(true);
			__Game_Vehicles_OwnedVehicle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<OwnedVehicle>(true);
			__Game_Pathfind_PathInformation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathInformation>(true);
			__Game_Companies_LodgingProvider_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LodgingProvider>(true);
			__Game_Citizens_TouristHousehold_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TouristHousehold>(false);
			__Game_Companies_LodgingProvider_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LodgingProvider>(false);
			__Game_Buildings_Renter_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Renter>(false);
		}
	}

	private EntityQuery m_SeekerQuery;

	private ComponentTypeSet m_PathfindTypes;

	private EndFrameBarrier m_EndFrameBarrier;

	private PathfindSetupSystem m_PathfindSetupSystem;

	private AddMeetingSystem m_AddMeetingSystem;

	private NativeQueue<HotelReserveAction> m_HotelReserveQueue;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_PathfindSetupSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindSetupSystem>();
		m_AddMeetingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AddMeetingSystem>();
		m_SeekerQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadWrite<TouristHousehold>(),
			ComponentType.ReadWrite<LodgingSeeker>(),
			ComponentType.Exclude<MovingAway>(),
			ComponentType.Exclude<Target>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_PathfindTypes = new ComponentTypeSet(ComponentType.ReadWrite<PathInformation>());
		m_HotelReserveQueue = new NativeQueue<HotelReserveAction>(AllocatorHandle.op_Implicit((Allocator)4));
		((ComponentSystemBase)this).RequireForUpdate(m_SeekerQuery);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_HotelReserveQueue.Dispose();
		base.OnDestroy();
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
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		TouristFindTargetJob touristFindTargetJob = new TouristFindTargetJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdType = InternalCompilerInterface.GetComponentTypeHandle<TouristHousehold>(ref __TypeHandle.__Game_Citizens_TouristHousehold_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentBuildings = InternalCompilerInterface.GetComponentLookup<CurrentBuilding>(ref __TypeHandle.__Game_Citizens_CurrentBuilding_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RenterBufs = InternalCompilerInterface.GetBufferLookup<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdCitizenBufs = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnedVehicleBufs = InternalCompilerInterface.GetBufferLookup<OwnedVehicle>(ref __TypeHandle.__Game_Vehicles_OwnedVehicle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathfindTypeSet = m_PathfindTypes,
			m_PathInformations = InternalCompilerInterface.GetComponentLookup<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LodgingProviders = InternalCompilerInterface.GetComponentLookup<LodgingProvider>(ref __TypeHandle.__Game_Companies_LodgingProvider_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathfindQueue = m_PathfindSetupSystem.GetQueue(this, 64).AsParallelWriter()
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		touristFindTargetJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		touristFindTargetJob.m_MeetingQueue = m_AddMeetingSystem.GetMeetingQueue(out var deps).AsParallelWriter();
		touristFindTargetJob.m_ReserveQueue = m_HotelReserveQueue.AsParallelWriter();
		TouristFindTargetJob touristFindTargetJob2 = touristFindTargetJob;
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<TouristFindTargetJob>(touristFindTargetJob2, m_SeekerQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, deps));
		m_PathfindSetupSystem.AddQueueWriter(((SystemBase)this).Dependency);
		HotelReserveJob hotelReserveJob = new HotelReserveJob
		{
			m_TouristHouseholds = InternalCompilerInterface.GetComponentLookup<TouristHousehold>(ref __TypeHandle.__Game_Citizens_TouristHousehold_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LodgingProviders = InternalCompilerInterface.GetComponentLookup<LodgingProvider>(ref __TypeHandle.__Game_Companies_LodgingProvider_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RenterBufs = InternalCompilerInterface.GetBufferLookup<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ReserveQueue = m_HotelReserveQueue,
			m_CommandBuffer = m_EndFrameBarrier.CreateCommandBuffer()
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<HotelReserveJob>(hotelReserveJob, ((SystemBase)this).Dependency);
		m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
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
	public TouristFindTargetSystem()
	{
	}
}
