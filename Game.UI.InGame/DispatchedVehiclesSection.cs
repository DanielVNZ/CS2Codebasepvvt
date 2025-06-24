using System.Runtime.CompilerServices;
using Colossal.UI.Binding;
using Game.Citizens;
using Game.Common;
using Game.Events;
using Game.Simulation;
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

namespace Game.UI.InGame;

[CompilerGenerated]
public class DispatchedVehiclesSection : InfoSectionBase
{
	[BurstCompile]
	private struct CollectDispatchedVehiclesJob : IJobChunk
	{
		[ReadOnly]
		public Entity m_SelectedEntity;

		[ReadOnly]
		public EntityTypeHandle m_EntityHandle;

		[ReadOnly]
		public BufferTypeHandle<ServiceDispatch> m_ServiceDispatchHandle;

		[ReadOnly]
		public ComponentLookup<CurrentBuilding> m_CurrentBuildingFromEntity;

		[ReadOnly]
		public ComponentLookup<FireRescueRequest> m_FireRequestFromEntity;

		[ReadOnly]
		public ComponentLookup<PoliceEmergencyRequest> m_PoliceRequestFromEntity;

		[ReadOnly]
		public ComponentLookup<HealthcareRequest> m_HealthcareRequestFromEntity;

		[ReadOnly]
		public ComponentLookup<EvacuationRequest> m_EvacuationRequestFromEntity;

		[ReadOnly]
		public ComponentLookup<GarbageCollectionRequest> m_GarbageCollectionRequest;

		[ReadOnly]
		public ComponentLookup<MaintenanceRequest> m_MaintenanceRequest;

		[ReadOnly]
		public ComponentLookup<OnFire> m_OnFireFromEntity;

		[ReadOnly]
		public ComponentLookup<Destroyed> m_DestroyedFromEntity;

		[ReadOnly]
		public ComponentLookup<AccidentSite> m_AccidentSiteFromEntity;

		[ReadOnly]
		public ComponentLookup<InDanger> m_InDangerFromEntity;

		[ReadOnly]
		public ComponentLookup<FireEngine> m_FireEngineFromEntity;

		[ReadOnly]
		public ComponentLookup<PoliceCar> m_PoliceCarFromEntity;

		[ReadOnly]
		public ComponentLookup<Ambulance> m_AmbulanceFromEntity;

		[ReadOnly]
		public ComponentLookup<PublicTransport> m_PublicTransportFromEntity;

		[ReadOnly]
		public ComponentLookup<Hearse> m_HearseFromEntity;

		[ReadOnly]
		public ComponentLookup<MaintenanceVehicle> m_MaintenanceVehicleFromEntity;

		public NativeList<Entity> m_VehiclesResult;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			BufferAccessor<ServiceDispatch> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ServiceDispatch>(ref m_ServiceDispatchHandle);
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityHandle);
			FireEngine fireEngine = default(FireEngine);
			PoliceCar policeCar = default(PoliceCar);
			Ambulance ambulance = default(Ambulance);
			PublicTransport publicTransport = default(PublicTransport);
			Hearse hearse = default(Hearse);
			MaintenanceVehicle maintenanceVehicle = default(MaintenanceVehicle);
			FireRescueRequest fireRescueRequest = default(FireRescueRequest);
			OnFire onFire = default(OnFire);
			Destroyed destroyed = default(Destroyed);
			PoliceEmergencyRequest policeEmergencyRequest = default(PoliceEmergencyRequest);
			AccidentSite accidentSite = default(AccidentSite);
			HealthcareRequest healthcareRequest = default(HealthcareRequest);
			CurrentBuilding currentBuilding = default(CurrentBuilding);
			EvacuationRequest evacuationRequest = default(EvacuationRequest);
			InDanger inDanger = default(InDanger);
			GarbageCollectionRequest garbageCollectionRequest = default(GarbageCollectionRequest);
			MaintenanceRequest maintenanceRequest = default(MaintenanceRequest);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				if (NativeListExtensions.Contains<Entity, Entity>(m_VehiclesResult, val))
				{
					continue;
				}
				int num = 0;
				if (m_FireEngineFromEntity.TryGetComponent(val, ref fireEngine))
				{
					num = fireEngine.m_RequestCount;
				}
				if (m_PoliceCarFromEntity.TryGetComponent(val, ref policeCar))
				{
					num = policeCar.m_RequestCount;
				}
				if (m_AmbulanceFromEntity.TryGetComponent(val, ref ambulance) && (ambulance.m_State & AmbulanceFlags.Dispatched) != 0)
				{
					num = 1;
				}
				if (m_PublicTransportFromEntity.TryGetComponent(val, ref publicTransport))
				{
					num = publicTransport.m_RequestCount;
				}
				if (m_HearseFromEntity.TryGetComponent(val, ref hearse) && (hearse.m_State & HearseFlags.Dispatched) != 0)
				{
					num = 1;
				}
				if (m_MaintenanceVehicleFromEntity.TryGetComponent(val, ref maintenanceVehicle))
				{
					num = maintenanceVehicle.m_RequestCount;
				}
				DynamicBuffer<ServiceDispatch> val2 = bufferAccessor[i];
				for (int j = 0; j < math.min(val2.Length, num); j++)
				{
					ServiceDispatch serviceDispatch = val2[j];
					if (m_FireRequestFromEntity.TryGetComponent(serviceDispatch.m_Request, ref fireRescueRequest) && (fireRescueRequest.m_Target == m_SelectedEntity || (m_OnFireFromEntity.TryGetComponent(fireRescueRequest.m_Target, ref onFire) && onFire.m_Event == m_SelectedEntity) || (m_DestroyedFromEntity.TryGetComponent(fireRescueRequest.m_Target, ref destroyed) && destroyed.m_Event == m_SelectedEntity)))
					{
						m_VehiclesResult.Add(ref val);
					}
					if (m_PoliceRequestFromEntity.TryGetComponent(serviceDispatch.m_Request, ref policeEmergencyRequest) && (policeEmergencyRequest.m_Target == m_SelectedEntity || (m_AccidentSiteFromEntity.TryGetComponent(policeEmergencyRequest.m_Target, ref accidentSite) && accidentSite.m_Event == m_SelectedEntity)))
					{
						m_VehiclesResult.Add(ref val);
					}
					if (m_HealthcareRequestFromEntity.TryGetComponent(serviceDispatch.m_Request, ref healthcareRequest) && (healthcareRequest.m_Citizen == m_SelectedEntity || (m_CurrentBuildingFromEntity.TryGetComponent(healthcareRequest.m_Citizen, ref currentBuilding) && currentBuilding.m_CurrentBuilding == m_SelectedEntity)))
					{
						m_VehiclesResult.Add(ref val);
					}
					if (m_EvacuationRequestFromEntity.TryGetComponent(serviceDispatch.m_Request, ref evacuationRequest) && (evacuationRequest.m_Target == m_SelectedEntity || (m_InDangerFromEntity.TryGetComponent(evacuationRequest.m_Target, ref inDanger) && inDanger.m_Event == m_SelectedEntity)))
					{
						m_VehiclesResult.Add(ref val);
					}
					if (m_GarbageCollectionRequest.TryGetComponent(serviceDispatch.m_Request, ref garbageCollectionRequest) && garbageCollectionRequest.m_Target == m_SelectedEntity)
					{
						m_VehiclesResult.Add(ref val);
					}
					if (m_MaintenanceRequest.TryGetComponent(serviceDispatch.m_Request, ref maintenanceRequest) && maintenanceRequest.m_Target == m_SelectedEntity)
					{
						m_VehiclesResult.Add(ref val);
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
		public BufferTypeHandle<ServiceDispatch> __Game_Simulation_ServiceDispatch_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<CurrentBuilding> __Game_Citizens_CurrentBuilding_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<FireRescueRequest> __Game_Simulation_FireRescueRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PoliceEmergencyRequest> __Game_Simulation_PoliceEmergencyRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HealthcareRequest> __Game_Simulation_HealthcareRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EvacuationRequest> __Game_Simulation_EvacuationRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<GarbageCollectionRequest> __Game_Simulation_GarbageCollectionRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<OnFire> __Game_Events_OnFire_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Destroyed> __Game_Common_Destroyed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AccidentSite> __Game_Events_AccidentSite_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<InDanger> __Game_Events_InDanger_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<FireEngine> __Game_Vehicles_FireEngine_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Ambulance> __Game_Vehicles_Ambulance_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PoliceCar> __Game_Vehicles_PoliceCar_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PublicTransport> __Game_Vehicles_PublicTransport_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Hearse> __Game_Vehicles_Hearse_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MaintenanceVehicle> __Game_Vehicles_MaintenanceVehicle_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MaintenanceRequest> __Game_Simulation_MaintenanceRequest_RO_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Simulation_ServiceDispatch_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ServiceDispatch>(true);
			__Game_Citizens_CurrentBuilding_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentBuilding>(true);
			__Game_Simulation_FireRescueRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<FireRescueRequest>(true);
			__Game_Simulation_PoliceEmergencyRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PoliceEmergencyRequest>(true);
			__Game_Simulation_HealthcareRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HealthcareRequest>(true);
			__Game_Simulation_EvacuationRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EvacuationRequest>(true);
			__Game_Simulation_GarbageCollectionRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarbageCollectionRequest>(true);
			__Game_Events_OnFire_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<OnFire>(true);
			__Game_Common_Destroyed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Destroyed>(true);
			__Game_Events_AccidentSite_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AccidentSite>(true);
			__Game_Events_InDanger_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<InDanger>(true);
			__Game_Vehicles_FireEngine_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<FireEngine>(true);
			__Game_Vehicles_Ambulance_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Ambulance>(true);
			__Game_Vehicles_PoliceCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PoliceCar>(true);
			__Game_Vehicles_PublicTransport_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PublicTransport>(true);
			__Game_Vehicles_Hearse_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Hearse>(true);
			__Game_Vehicles_MaintenanceVehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MaintenanceVehicle>(true);
			__Game_Simulation_MaintenanceRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MaintenanceRequest>(true);
		}
	}

	private EntityQuery m_ServiceDispatchQuery;

	private NativeList<Entity> m_VehiclesResult;

	private TypeHandle __TypeHandle;

	protected override string group => "DispatchedVehiclesSection";

	protected override bool displayForDestroyedObjects => true;

	protected override bool displayForUpgrades => true;

	private NativeList<VehiclesSection.UIVehicle> vehicleList { get; set; }

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ServiceDispatchQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Vehicle>(),
			ComponentType.ReadOnly<ServiceDispatch>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_VehiclesResult = new NativeList<Entity>(5, AllocatorHandle.op_Implicit((Allocator)4));
		vehicleList = new NativeList<VehiclesSection.UIVehicle>(5, AllocatorHandle.op_Implicit((Allocator)4));
	}

	[Preserve]
	protected override void OnDestroy()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		vehicleList.Dispose();
		m_VehiclesResult.Dispose();
		base.OnDestroy();
	}

	protected override void Reset()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		vehicleList.Clear();
		m_VehiclesResult.Clear();
	}

	private bool Visible()
	{
		return m_VehiclesResult.Length > 0;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = JobChunkExtensions.Schedule<CollectDispatchedVehiclesJob>(new CollectDispatchedVehiclesJob
		{
			m_SelectedEntity = selectedEntity,
			m_EntityHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceDispatchHandle = InternalCompilerInterface.GetBufferTypeHandle<ServiceDispatch>(ref __TypeHandle.__Game_Simulation_ServiceDispatch_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentBuildingFromEntity = InternalCompilerInterface.GetComponentLookup<CurrentBuilding>(ref __TypeHandle.__Game_Citizens_CurrentBuilding_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FireRequestFromEntity = InternalCompilerInterface.GetComponentLookup<FireRescueRequest>(ref __TypeHandle.__Game_Simulation_FireRescueRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PoliceRequestFromEntity = InternalCompilerInterface.GetComponentLookup<PoliceEmergencyRequest>(ref __TypeHandle.__Game_Simulation_PoliceEmergencyRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HealthcareRequestFromEntity = InternalCompilerInterface.GetComponentLookup<HealthcareRequest>(ref __TypeHandle.__Game_Simulation_HealthcareRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EvacuationRequestFromEntity = InternalCompilerInterface.GetComponentLookup<EvacuationRequest>(ref __TypeHandle.__Game_Simulation_EvacuationRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageCollectionRequest = InternalCompilerInterface.GetComponentLookup<GarbageCollectionRequest>(ref __TypeHandle.__Game_Simulation_GarbageCollectionRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OnFireFromEntity = InternalCompilerInterface.GetComponentLookup<OnFire>(ref __TypeHandle.__Game_Events_OnFire_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DestroyedFromEntity = InternalCompilerInterface.GetComponentLookup<Destroyed>(ref __TypeHandle.__Game_Common_Destroyed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AccidentSiteFromEntity = InternalCompilerInterface.GetComponentLookup<AccidentSite>(ref __TypeHandle.__Game_Events_AccidentSite_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InDangerFromEntity = InternalCompilerInterface.GetComponentLookup<InDanger>(ref __TypeHandle.__Game_Events_InDanger_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FireEngineFromEntity = InternalCompilerInterface.GetComponentLookup<FireEngine>(ref __TypeHandle.__Game_Vehicles_FireEngine_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AmbulanceFromEntity = InternalCompilerInterface.GetComponentLookup<Ambulance>(ref __TypeHandle.__Game_Vehicles_Ambulance_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PoliceCarFromEntity = InternalCompilerInterface.GetComponentLookup<PoliceCar>(ref __TypeHandle.__Game_Vehicles_PoliceCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PublicTransportFromEntity = InternalCompilerInterface.GetComponentLookup<PublicTransport>(ref __TypeHandle.__Game_Vehicles_PublicTransport_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HearseFromEntity = InternalCompilerInterface.GetComponentLookup<Hearse>(ref __TypeHandle.__Game_Vehicles_Hearse_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MaintenanceVehicleFromEntity = InternalCompilerInterface.GetComponentLookup<MaintenanceVehicle>(ref __TypeHandle.__Game_Vehicles_MaintenanceVehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MaintenanceRequest = InternalCompilerInterface.GetComponentLookup<MaintenanceRequest>(ref __TypeHandle.__Game_Simulation_MaintenanceRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_VehiclesResult = m_VehiclesResult
		}, m_ServiceDispatchQuery, ((SystemBase)this).Dependency);
		((JobHandle)(ref val)).Complete();
		base.visible = Visible();
	}

	protected override void OnProcess()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_VehiclesResult.Length; i++)
		{
			Entity vehicle = m_VehiclesResult[i];
			VehiclesSection.AddVehicle(((ComponentSystemBase)this).EntityManager, vehicle, vehicleList);
		}
		NativeSortExtension.Sort<VehiclesSection.UIVehicle>(vehicleList);
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		writer.PropertyName("vehicleList");
		JsonWriterExtensions.ArrayBegin(writer, vehicleList.Length);
		for (int i = 0; i < vehicleList.Length; i++)
		{
			VehiclesSection.BindVehicle(m_NameSystem, writer, vehicleList[i]);
		}
		writer.ArrayEnd();
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
	public DispatchedVehiclesSection()
	{
	}
}
