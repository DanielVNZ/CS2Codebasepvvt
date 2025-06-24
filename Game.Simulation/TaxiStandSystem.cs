using System.Runtime.CompilerServices;
using Game.Areas;
using Game.City;
using Game.Common;
using Game.Prefabs;
using Game.Routes;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class TaxiStandSystem : GameSystemBase
{
	[BurstCompile]
	private struct TaxiStandTickJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<RouteLane> m_RouteLaneType;

		[ReadOnly]
		public ComponentTypeHandle<WaitingPassengers> m_WaitingPassengersType;

		public ComponentTypeHandle<TaxiStand> m_TaxiStandType;

		public BufferTypeHandle<RouteVehicle> m_RouteVehicleType;

		public BufferTypeHandle<DispatchedRequest> m_DispatchedRequestType;

		[ReadOnly]
		public ComponentLookup<TaxiRequest> m_TaxiRequestData;

		[ReadOnly]
		public ComponentLookup<CurrentRoute> m_CurrentRouteData;

		[ReadOnly]
		public ComponentLookup<Dispatched> m_DispatchedData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<CurrentDistrict> m_CurrentDistrictData;

		[ReadOnly]
		public ComponentLookup<BorderDistrict> m_BorderDistrictData;

		[ReadOnly]
		public ComponentLookup<Game.City.City> m_CityData;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifiers;

		[ReadOnly]
		public Entity m_City;

		[ReadOnly]
		public EntityArchetype m_TaxiRequestArchetype;

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
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<TaxiStand> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TaxiStand>(ref m_TaxiStandType);
			NativeArray<RouteLane> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<RouteLane>(ref m_RouteLaneType);
			NativeArray<WaitingPassengers> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WaitingPassengers>(ref m_WaitingPassengersType);
			BufferAccessor<RouteVehicle> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<RouteVehicle>(ref m_RouteVehicleType);
			BufferAccessor<DispatchedRequest> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<DispatchedRequest>(ref m_DispatchedRequestType);
			ushort num = 0;
			if (CityUtils.CheckOption(m_CityData[m_City], CityOption.PaidTaxiStart))
			{
				float value = 0f;
				CityUtils.ApplyModifier(ref value, m_CityModifiers[m_City], CityModifierType.TaxiStartingFee);
				num = (ushort)math.clamp(Mathf.RoundToInt(value), 0, 65535);
			}
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				_ = nativeArray2[i];
				TaxiStand taxiStand = nativeArray3[i];
				WaitingPassengers waitingPassengers = nativeArray5[i];
				DynamicBuffer<RouteVehicle> vehicles = bufferAccessor[i];
				DynamicBuffer<DispatchedRequest> requests = bufferAccessor2[i];
				int maxTaxiCount = RouteUtils.GetMaxTaxiCount(waitingPassengers);
				CheckVehicles(val, vehicles, out var count);
				CheckRequests(ref taxiStand, requests);
				if (count < maxTaxiCount)
				{
					taxiStand.m_Flags |= TaxiStandFlags.RequireVehicles;
					count += requests.Length;
					if (count < maxTaxiCount)
					{
						Entity lane = Entity.Null;
						if (nativeArray4.Length != 0)
						{
							lane = nativeArray4[i].m_EndLane;
						}
						RequestNewVehicleIfNeeded(unfilteredChunkIndex, val, lane, taxiStand, maxTaxiCount - count);
					}
				}
				else
				{
					taxiStand.m_Flags &= ~TaxiStandFlags.RequireVehicles;
				}
				if (taxiStand.m_StartingFee != num)
				{
					taxiStand.m_StartingFee = num;
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(unfilteredChunkIndex, val, default(PathfindUpdated));
				}
				nativeArray3[i] = taxiStand;
			}
		}

		private void CheckVehicles(Entity route, DynamicBuffer<RouteVehicle> vehicles, out int count)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			count = 0;
			while (count < vehicles.Length)
			{
				Entity vehicle = vehicles[count].m_Vehicle;
				CurrentRoute currentRoute = default(CurrentRoute);
				if (m_CurrentRouteData.HasComponent(vehicle))
				{
					currentRoute = m_CurrentRouteData[vehicle];
				}
				if (currentRoute.m_Route == route)
				{
					count++;
				}
				else
				{
					vehicles.RemoveAt(count);
				}
			}
		}

		private void CheckRequests(ref TaxiStand taxiStand, DynamicBuffer<DispatchedRequest> requests)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < requests.Length; i++)
			{
				Entity vehicleRequest = requests[i].m_VehicleRequest;
				if (!m_TaxiRequestData.HasComponent(vehicleRequest))
				{
					requests.RemoveAtSwapBack(i--);
				}
			}
			if (m_DispatchedData.HasComponent(taxiStand.m_TaxiRequest))
			{
				requests.Add(new DispatchedRequest
				{
					m_VehicleRequest = taxiStand.m_TaxiRequest
				});
				taxiStand.m_TaxiRequest = Entity.Null;
			}
		}

		private void RequestNewVehicleIfNeeded(int jobIndex, Entity entity, Entity lane, TaxiStand taxiStand, int priority)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			if (!m_TaxiRequestData.HasComponent(taxiStand.m_TaxiRequest))
			{
				GetDistricts(lane, out var district, out var district2);
				Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_TaxiRequestArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<TaxiRequest>(jobIndex, val, new TaxiRequest(entity, district, district2, TaxiRequestType.Stand, priority));
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RequestGroup>(jobIndex, val, new RequestGroup(16u));
			}
		}

		private void GetDistricts(Entity entity, out Entity district1, out Entity district2)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			BorderDistrict borderDistrict = default(BorderDistrict);
			CurrentDistrict currentDistrict = default(CurrentDistrict);
			Owner owner = default(Owner);
			while (true)
			{
				if (m_BorderDistrictData.TryGetComponent(entity, ref borderDistrict))
				{
					district1 = borderDistrict.m_Left;
					district2 = borderDistrict.m_Right;
					return;
				}
				if (m_CurrentDistrictData.TryGetComponent(entity, ref currentDistrict))
				{
					district1 = currentDistrict.m_District;
					district2 = currentDistrict.m_District;
					return;
				}
				if (!m_OwnerData.TryGetComponent(entity, ref owner))
				{
					break;
				}
				entity = owner.m_Owner;
			}
			district1 = Entity.Null;
			district2 = Entity.Null;
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
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<RouteLane> __Game_Routes_RouteLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WaitingPassengers> __Game_Routes_WaitingPassengers_RO_ComponentTypeHandle;

		public ComponentTypeHandle<TaxiStand> __Game_Routes_TaxiStand_RW_ComponentTypeHandle;

		public BufferTypeHandle<RouteVehicle> __Game_Routes_RouteVehicle_RW_BufferTypeHandle;

		public BufferTypeHandle<DispatchedRequest> __Game_Routes_DispatchedRequest_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<TaxiRequest> __Game_Simulation_TaxiRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentRoute> __Game_Routes_CurrentRoute_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Dispatched> __Game_Simulation_Dispatched_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentDistrict> __Game_Areas_CurrentDistrict_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BorderDistrict> __Game_Areas_BorderDistrict_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.City.City> __Game_City_City_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<CityModifier> __Game_City_CityModifier_RO_BufferLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Routes_RouteLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<RouteLane>(true);
			__Game_Routes_WaitingPassengers_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WaitingPassengers>(true);
			__Game_Routes_TaxiStand_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TaxiStand>(false);
			__Game_Routes_RouteVehicle_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<RouteVehicle>(false);
			__Game_Routes_DispatchedRequest_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<DispatchedRequest>(false);
			__Game_Simulation_TaxiRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TaxiRequest>(true);
			__Game_Routes_CurrentRoute_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentRoute>(true);
			__Game_Simulation_Dispatched_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Dispatched>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Areas_CurrentDistrict_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentDistrict>(true);
			__Game_Areas_BorderDistrict_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BorderDistrict>(true);
			__Game_City_City_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.City.City>(true);
			__Game_City_CityModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(true);
		}
	}

	public const uint UPDATE_INTERVAL = 256u;

	private EntityQuery m_StandQuery;

	private EntityArchetype m_VehicleRequestArchetype;

	private CitySystem m_CitySystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 256;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_StandQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadWrite<TaxiStand>(),
			ComponentType.ReadOnly<Game.Routes.TransportStop>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_VehicleRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<ServiceRequest>(),
			ComponentType.ReadWrite<TaxiRequest>(),
			ComponentType.ReadWrite<RequestGroup>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_StandQuery);
		Assert.IsTrue(true);
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
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		TaxiStandTickJob taxiStandTickJob = new TaxiStandTickJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RouteLaneType = InternalCompilerInterface.GetComponentTypeHandle<RouteLane>(ref __TypeHandle.__Game_Routes_RouteLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WaitingPassengersType = InternalCompilerInterface.GetComponentTypeHandle<WaitingPassengers>(ref __TypeHandle.__Game_Routes_WaitingPassengers_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TaxiStandType = InternalCompilerInterface.GetComponentTypeHandle<TaxiStand>(ref __TypeHandle.__Game_Routes_TaxiStand_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RouteVehicleType = InternalCompilerInterface.GetBufferTypeHandle<RouteVehicle>(ref __TypeHandle.__Game_Routes_RouteVehicle_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DispatchedRequestType = InternalCompilerInterface.GetBufferTypeHandle<DispatchedRequest>(ref __TypeHandle.__Game_Routes_DispatchedRequest_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TaxiRequestData = InternalCompilerInterface.GetComponentLookup<TaxiRequest>(ref __TypeHandle.__Game_Simulation_TaxiRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentRouteData = InternalCompilerInterface.GetComponentLookup<CurrentRoute>(ref __TypeHandle.__Game_Routes_CurrentRoute_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DispatchedData = InternalCompilerInterface.GetComponentLookup<Dispatched>(ref __TypeHandle.__Game_Simulation_Dispatched_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentDistrictData = InternalCompilerInterface.GetComponentLookup<CurrentDistrict>(ref __TypeHandle.__Game_Areas_CurrentDistrict_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BorderDistrictData = InternalCompilerInterface.GetComponentLookup<BorderDistrict>(ref __TypeHandle.__Game_Areas_BorderDistrict_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CityData = InternalCompilerInterface.GetComponentLookup<Game.City.City>(ref __TypeHandle.__Game_City_City_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CityModifiers = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_City = m_CitySystem.City,
			m_TaxiRequestArchetype = m_VehicleRequestArchetype
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		taxiStandTickJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<TaxiStandTickJob>(taxiStandTickJob, m_StandQuery, ((SystemBase)this).Dependency);
		m_EndFrameBarrier.AddJobHandleForProducer(val2);
		((SystemBase)this).Dependency = val2;
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
	public TaxiStandSystem()
	{
	}
}
