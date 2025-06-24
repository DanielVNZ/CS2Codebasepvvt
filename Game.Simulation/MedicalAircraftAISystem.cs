using System.Runtime.CompilerServices;
using Game.Areas;
using Game.Buildings;
using Game.Citizens;
using Game.Common;
using Game.Creatures;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Rendering;
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
public class MedicalAircraftAISystem : GameSystemBase
{
	[BurstCompile]
	private struct MedicalAircraftTickJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Unspawned> m_UnspawnedType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<PathInformation> m_PathInformationType;

		[ReadOnly]
		public ComponentTypeHandle<Stopped> m_StoppedType;

		public ComponentTypeHandle<Game.Vehicles.Ambulance> m_AmbulanceType;

		public ComponentTypeHandle<Aircraft> m_AircraftType;

		public ComponentTypeHandle<AircraftCurrentLane> m_CurrentLaneType;

		public ComponentTypeHandle<Target> m_TargetType;

		public ComponentTypeHandle<PathOwner> m_PathOwnerType;

		public BufferTypeHandle<AircraftNavigationLane> m_AircraftNavigationLaneType;

		public BufferTypeHandle<Passenger> m_PassengerType;

		public BufferTypeHandle<ServiceDispatch> m_ServiceDispatchType;

		[ReadOnly]
		public EntityStorageInfoLookup m_EntityLookup;

		[ReadOnly]
		public ComponentLookup<PathInformation> m_PathInformationData;

		[ReadOnly]
		public ComponentLookup<ParkedCar> m_ParkedCarData;

		[ReadOnly]
		public ComponentLookup<Controller> m_ControllerData;

		[ReadOnly]
		public ComponentLookup<HelicopterData> m_PrefabHelicopterData;

		[ReadOnly]
		public ComponentLookup<HealthcareRequest> m_HealthcareRequestData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> m_SpawnLocationData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Hospital> m_HospitalData;

		[ReadOnly]
		public ComponentLookup<CurrentBuilding> m_CurrentBuildingData;

		[ReadOnly]
		public ComponentLookup<CurrentTransport> m_CurrentTransportData;

		[ReadOnly]
		public ComponentLookup<HealthProblem> m_HealthProblemData;

		[ReadOnly]
		public ComponentLookup<TravelPurpose> m_TravelPurposeData;

		[ReadOnly]
		public ComponentLookup<Citizen> m_CitizenData;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> m_CurrentVehicleData;

		[ReadOnly]
		public ComponentLookup<CurrentDistrict> m_CurrentDistrictData;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_LayoutElements;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Blocker> m_BlockerData;

		[NativeDisableParallelForRestriction]
		public BufferLookup<PathElement> m_PathElements;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public EntityArchetype m_HandleRequestArchetype;

		[ReadOnly]
		public ComponentTypeSet m_MovingToParkedAircraftRemoveTypes;

		[ReadOnly]
		public ComponentTypeSet m_MovingToParkedAddTypes;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<SetupQueueItem> m_PathfindQueue;

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
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Owner> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<PathInformation> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathInformation>(ref m_PathInformationType);
			NativeArray<AircraftCurrentLane> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AircraftCurrentLane>(ref m_CurrentLaneType);
			NativeArray<Game.Vehicles.Ambulance> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Vehicles.Ambulance>(ref m_AmbulanceType);
			NativeArray<Aircraft> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Aircraft>(ref m_AircraftType);
			NativeArray<Target> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Target>(ref m_TargetType);
			NativeArray<PathOwner> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathOwner>(ref m_PathOwnerType);
			BufferAccessor<AircraftNavigationLane> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<AircraftNavigationLane>(ref m_AircraftNavigationLaneType);
			BufferAccessor<Passenger> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Passenger>(ref m_PassengerType);
			BufferAccessor<ServiceDispatch> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ServiceDispatch>(ref m_ServiceDispatchType);
			bool isStopped = ((ArchetypeChunk)(ref chunk)).Has<Stopped>(ref m_StoppedType);
			bool isUnspawned = ((ArchetypeChunk)(ref chunk)).Has<Unspawned>(ref m_UnspawnedType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity entity = nativeArray[i];
				Owner owner = nativeArray2[i];
				PrefabRef prefabRef = nativeArray3[i];
				PathInformation pathInformation = nativeArray4[i];
				Game.Vehicles.Ambulance ambulance = nativeArray6[i];
				Aircraft aircraft = nativeArray7[i];
				AircraftCurrentLane currentLane = nativeArray5[i];
				PathOwner pathOwner = nativeArray9[i];
				Target target = nativeArray8[i];
				DynamicBuffer<AircraftNavigationLane> navigationLanes = bufferAccessor[i];
				DynamicBuffer<Passenger> passengers = bufferAccessor2[i];
				DynamicBuffer<ServiceDispatch> serviceDispatches = bufferAccessor3[i];
				VehicleUtils.CheckUnspawned(unfilteredChunkIndex, entity, currentLane, isUnspawned, m_CommandBuffer);
				Tick(unfilteredChunkIndex, entity, owner, prefabRef, pathInformation, navigationLanes, passengers, serviceDispatches, isStopped, ref random, ref ambulance, ref aircraft, ref currentLane, ref pathOwner, ref target);
				nativeArray6[i] = ambulance;
				nativeArray7[i] = aircraft;
				nativeArray5[i] = currentLane;
				nativeArray9[i] = pathOwner;
				nativeArray8[i] = target;
			}
		}

		private void Tick(int jobIndex, Entity entity, Owner owner, PrefabRef prefabRef, PathInformation pathInformation, DynamicBuffer<AircraftNavigationLane> navigationLanes, DynamicBuffer<Passenger> passengers, DynamicBuffer<ServiceDispatch> serviceDispatches, bool isStopped, ref Random random, ref Game.Vehicles.Ambulance ambulance, ref Aircraft aircraft, ref AircraftCurrentLane currentLane, ref PathOwner pathOwner, ref Target target)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			CheckServiceDispatches(entity, serviceDispatches, ref ambulance);
			if (VehicleUtils.ResetUpdatedPath(ref pathOwner))
			{
				ResetPath(jobIndex, entity, pathInformation, passengers, serviceDispatches, ref ambulance, ref aircraft, ref currentLane, ref target);
			}
			if (VehicleUtils.IsStuck(pathOwner))
			{
				Blocker blocker = m_BlockerData[entity];
				bool num = m_ParkedCarData.HasComponent(blocker.m_Blocker);
				if (num)
				{
					Entity val = blocker.m_Blocker;
					Controller controller = default(Controller);
					if (m_ControllerData.TryGetComponent(val, ref controller))
					{
						val = controller.m_Controller;
					}
					DynamicBuffer<LayoutElement> layout = default(DynamicBuffer<LayoutElement>);
					m_LayoutElements.TryGetBuffer(val, ref layout);
					VehicleUtils.DeleteVehicle(m_CommandBuffer, jobIndex, val, layout);
				}
				if (num || blocker.m_Blocker == Entity.Null)
				{
					pathOwner.m_State &= ~PathFlags.Stuck;
					m_BlockerData[entity] = default(Blocker);
				}
			}
			if (!((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(target.m_Target) || VehicleUtils.PathfindFailed(pathOwner))
			{
				if (VehicleUtils.IsStuck(pathOwner) || (ambulance.m_State & AmbulanceFlags.Returning) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, entity, default(Deleted));
				}
				else
				{
					ReturnToDepot(owner, serviceDispatches, ref ambulance, ref aircraft, ref pathOwner, ref target);
				}
				return;
			}
			if (VehicleUtils.PathEndReached(currentLane) || (ambulance.m_State & (AmbulanceFlags.AtTarget | AmbulanceFlags.Disembarking)) != 0)
			{
				if ((ambulance.m_State & AmbulanceFlags.Returning) != 0)
				{
					if (UnloadPatients(passengers, ref ambulance))
					{
						if (VehicleUtils.ParkingSpaceReached(currentLane, pathOwner))
						{
							ParkAircraft(jobIndex, entity, owner, ref aircraft, ref ambulance, ref currentLane);
						}
						else
						{
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, entity, default(Deleted));
						}
					}
					return;
				}
				if ((ambulance.m_State & AmbulanceFlags.Transporting) != 0)
				{
					if (!UnloadPatients(passengers, ref ambulance))
					{
						return;
					}
					if (!SelectNextDispatch(jobIndex, entity, navigationLanes, serviceDispatches, ref ambulance, ref aircraft, ref currentLane, ref pathOwner, ref target))
					{
						if (target.m_Target == owner.m_Owner)
						{
							if (VehicleUtils.ParkingSpaceReached(currentLane, pathOwner))
							{
								ParkAircraft(jobIndex, entity, owner, ref aircraft, ref ambulance, ref currentLane);
							}
							else
							{
								((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, entity, default(Deleted));
							}
							return;
						}
						ReturnToDepot(owner, serviceDispatches, ref ambulance, ref aircraft, ref pathOwner, ref target);
					}
				}
				else if (LoadPatients(jobIndex, entity, passengers, serviceDispatches, isStopped, ref random, ref ambulance, ref target))
				{
					if ((ambulance.m_State & AmbulanceFlags.Transporting) != 0)
					{
						TransportToHospital(owner, serviceDispatches, ref ambulance, ref aircraft, ref pathOwner, ref target);
					}
					else if (!SelectNextDispatch(jobIndex, entity, navigationLanes, serviceDispatches, ref ambulance, ref aircraft, ref currentLane, ref pathOwner, ref target))
					{
						if (target.m_Target == owner.m_Owner)
						{
							if (VehicleUtils.ParkingSpaceReached(currentLane, pathOwner))
							{
								ParkAircraft(jobIndex, entity, owner, ref aircraft, ref ambulance, ref currentLane);
							}
							else
							{
								((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, entity, default(Deleted));
							}
							return;
						}
						ReturnToDepot(owner, serviceDispatches, ref ambulance, ref aircraft, ref pathOwner, ref target);
					}
				}
			}
			else
			{
				if ((ambulance.m_State & (AmbulanceFlags.Returning | AmbulanceFlags.Transporting | AmbulanceFlags.Disabled)) == AmbulanceFlags.Disabled)
				{
					ReturnToDepot(owner, serviceDispatches, ref ambulance, ref aircraft, ref pathOwner, ref target);
				}
				if (isStopped)
				{
					StartVehicle(jobIndex, entity);
				}
			}
			if ((ambulance.m_State & (AmbulanceFlags.Returning | AmbulanceFlags.Dispatched | AmbulanceFlags.Transporting)) == (AmbulanceFlags.Returning | AmbulanceFlags.Dispatched) && !SelectNextDispatch(jobIndex, entity, navigationLanes, serviceDispatches, ref ambulance, ref aircraft, ref currentLane, ref pathOwner, ref target))
			{
				serviceDispatches.Clear();
				ambulance.m_State &= ~AmbulanceFlags.Dispatched;
			}
			if ((ambulance.m_State & (AmbulanceFlags.AtTarget | AmbulanceFlags.Disembarking)) == 0)
			{
				if (VehicleUtils.RequireNewPath(pathOwner))
				{
					FindNewPath(entity, prefabRef, ref ambulance, ref aircraft, ref currentLane, ref pathOwner, ref target);
				}
				else if ((pathOwner.m_State & (PathFlags.Pending | PathFlags.Failed | PathFlags.Stuck)) == 0)
				{
					CheckParkingSpace(ref aircraft, ref currentLane, ref pathOwner);
				}
			}
		}

		private void CheckParkingSpace(ref Aircraft aircraft, ref AircraftCurrentLane currentLane, ref PathOwner pathOwner)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			Game.Objects.SpawnLocation spawnLocation = default(Game.Objects.SpawnLocation);
			if ((currentLane.m_LaneFlags & AircraftLaneFlags.EndOfPath) == 0 || !m_SpawnLocationData.TryGetComponent(currentLane.m_Lane, ref spawnLocation))
			{
				return;
			}
			if ((spawnLocation.m_Flags & SpawnLocationFlags.ParkedVehicle) != 0)
			{
				if ((aircraft.m_Flags & AircraftFlags.IgnoreParkedVehicle) == 0)
				{
					aircraft.m_Flags |= AircraftFlags.IgnoreParkedVehicle;
					pathOwner.m_State |= PathFlags.Obsolete;
				}
			}
			else
			{
				aircraft.m_Flags &= ~AircraftFlags.IgnoreParkedVehicle;
			}
		}

		private void ParkAircraft(int jobIndex, Entity entity, Owner owner, ref Aircraft aircraft, ref Game.Vehicles.Ambulance ambulance, ref AircraftCurrentLane currentLane)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			aircraft.m_Flags &= ~(AircraftFlags.Emergency | AircraftFlags.IgnoreParkedVehicle);
			ambulance.m_State = (AmbulanceFlags)0u;
			Game.Buildings.Hospital hospital = default(Game.Buildings.Hospital);
			if (m_HospitalData.TryGetComponent(owner.m_Owner, ref hospital) && (hospital.m_Flags & HospitalFlags.HasAvailableMedicalHelicopters) == 0)
			{
				ambulance.m_State |= AmbulanceFlags.Disabled;
			}
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(jobIndex, entity, ref m_MovingToParkedAircraftRemoveTypes);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent(jobIndex, entity, ref m_MovingToParkedAddTypes);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ParkedCar>(jobIndex, entity, new ParkedCar(currentLane.m_Lane, currentLane.m_CurvePosition.x));
			if (m_SpawnLocationData.HasComponent(currentLane.m_Lane))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(jobIndex, currentLane.m_Lane);
			}
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<FixParkingLocation>(jobIndex, entity, new FixParkingLocation(Entity.Null, entity));
		}

		private void StopVehicle(int jobIndex, Entity entity)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Moving>(jobIndex, entity);
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TransformFrame>(jobIndex, entity);
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<InterpolatedTransform>(jobIndex, entity);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Stopped>(jobIndex, entity, default(Stopped));
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, entity, default(Updated));
		}

		private void StartVehicle(int jobIndex, Entity entity)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Stopped>(jobIndex, entity);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Moving>(jobIndex, entity, default(Moving));
			((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<TransformFrame>(jobIndex, entity);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<InterpolatedTransform>(jobIndex, entity, default(InterpolatedTransform));
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, entity, default(Updated));
		}

		private bool LoadPatients(int jobIndex, Entity entity, DynamicBuffer<Passenger> passengers, DynamicBuffer<ServiceDispatch> serviceDispatches, bool isStopped, ref Random random, ref Game.Vehicles.Ambulance ambulance, ref Target target)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			ambulance.m_State |= AmbulanceFlags.AtTarget;
			bool flag = false;
			if (serviceDispatches.Length == 0 || (ambulance.m_State & AmbulanceFlags.Dispatched) == 0)
			{
				ambulance.m_TargetPatient = Entity.Null;
				return true;
			}
			if (!m_HealthProblemData.HasComponent(ambulance.m_TargetPatient))
			{
				ambulance.m_TargetPatient = Entity.Null;
				return true;
			}
			HealthProblem healthProblem = m_HealthProblemData[ambulance.m_TargetPatient];
			if (healthProblem.m_HealthcareRequest != serviceDispatches[0].m_Request || (healthProblem.m_Flags & HealthProblemFlags.RequireTransport) == 0)
			{
				ambulance.m_TargetPatient = Entity.Null;
				return true;
			}
			CurrentBuilding currentBuilding = default(CurrentBuilding);
			if (m_CurrentBuildingData.TryGetComponent(ambulance.m_TargetPatient, ref currentBuilding) && currentBuilding.m_CurrentBuilding != Entity.Null)
			{
				flag |= currentBuilding.m_CurrentBuilding == target.m_Target;
			}
			CurrentTransport currentTransport = default(CurrentTransport);
			if (m_CurrentTransportData.TryGetComponent(ambulance.m_TargetPatient, ref currentTransport) && currentTransport.m_CurrentTransport != Entity.Null)
			{
				flag |= currentTransport.m_CurrentTransport == target.m_Target;
				TravelPurpose travelPurpose = default(TravelPurpose);
				if (!flag && ((m_TravelPurposeData.TryGetComponent(ambulance.m_TargetPatient, ref travelPurpose) && travelPurpose.m_Purpose == Purpose.Hospital) || travelPurpose.m_Purpose == Purpose.Deathcare))
				{
					flag = true;
				}
				CurrentVehicle currentVehicle = default(CurrentVehicle);
				if (flag && m_CurrentVehicleData.TryGetComponent(currentTransport.m_CurrentTransport, ref currentVehicle))
				{
					if (currentVehicle.m_Vehicle != entity)
					{
						flag = false;
					}
					else if ((currentVehicle.m_Flags & CreatureVehicleFlags.Ready) != 0)
					{
						Citizen citizen = default(Citizen);
						for (int i = 0; i < passengers.Length; i++)
						{
							if (passengers[i].m_Passenger == currentTransport.m_CurrentTransport)
							{
								ambulance.m_State |= AmbulanceFlags.Transporting;
								if (m_CitizenData.TryGetComponent(ambulance.m_TargetPatient, ref citizen) && ((Random)(ref random)).NextInt(100) >= citizen.m_Health)
								{
									ambulance.m_State |= AmbulanceFlags.Critical;
								}
								Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_HandleRequestArchetype);
								((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HandleRequest>(jobIndex, val, new HandleRequest(healthProblem.m_HealthcareRequest, entity, completed: true));
								return true;
							}
						}
					}
				}
			}
			if (flag)
			{
				if (!isStopped)
				{
					StopVehicle(jobIndex, entity);
				}
				return false;
			}
			ambulance.m_TargetPatient = Entity.Null;
			return true;
		}

		private bool UnloadPatients(DynamicBuffer<Passenger> passengers, ref Game.Vehicles.Ambulance ambulance)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			if (passengers.Length > 0)
			{
				ambulance.m_State |= AmbulanceFlags.Disembarking;
				return false;
			}
			passengers.Clear();
			ambulance.m_State &= ~(AmbulanceFlags.Transporting | AmbulanceFlags.Disembarking | AmbulanceFlags.Critical);
			ambulance.m_TargetPatient = Entity.Null;
			return true;
		}

		private void FindNewPath(Entity vehicleEntity, PrefabRef prefabRef, ref Game.Vehicles.Ambulance ambulance, ref Aircraft aircraft, ref AircraftCurrentLane currentLane, ref PathOwner pathOwner, ref Target target)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			HelicopterData helicopterData = m_PrefabHelicopterData[prefabRef.m_Prefab];
			PathfindParameters parameters = new PathfindParameters
			{
				m_MaxSpeed = float2.op_Implicit(helicopterData.m_FlyingMaxSpeed),
				m_WalkSpeed = float2.op_Implicit(5.555556f),
				m_Methods = (PathMethod.Road | PathMethod.Flying),
				m_IgnoredRules = (RuleFlags.ForbidCombustionEngines | RuleFlags.ForbidTransitTraffic | RuleFlags.ForbidPrivateTraffic | RuleFlags.ForbidSlowTraffic)
			};
			SetupQueueTarget origin = new SetupQueueTarget
			{
				m_Type = SetupTargetType.CurrentLocation,
				m_Methods = (PathMethod.Road | PathMethod.Flying),
				m_RoadTypes = RoadTypes.Helicopter,
				m_FlyingTypes = RoadTypes.Helicopter
			};
			SetupQueueTarget destination = default(SetupQueueTarget);
			if ((ambulance.m_State & AmbulanceFlags.FindHospital) != 0)
			{
				destination.m_Entity = FindDistrict(ambulance.m_TargetLocation);
				destination.m_Type = SetupTargetType.Hospital;
				destination.m_Methods = PathMethod.Road;
				destination.m_RoadTypes = RoadTypes.Helicopter;
			}
			else if ((ambulance.m_State & AmbulanceFlags.Returning) != 0)
			{
				destination.m_Type = SetupTargetType.CurrentLocation;
				destination.m_Methods = PathMethod.Road;
				destination.m_RoadTypes = RoadTypes.Helicopter;
				destination.m_Entity = target.m_Target;
			}
			else
			{
				destination.m_Type = SetupTargetType.CurrentLocation;
				destination.m_Methods = PathMethod.Road | PathMethod.Flying;
				destination.m_RoadTypes = RoadTypes.Helicopter;
				destination.m_FlyingTypes = RoadTypes.Helicopter;
				destination.m_Entity = target.m_Target;
			}
			if ((ambulance.m_State & (AmbulanceFlags.Returning | AmbulanceFlags.Dispatched | AmbulanceFlags.Transporting)) == AmbulanceFlags.Dispatched || (ambulance.m_State & (AmbulanceFlags.Transporting | AmbulanceFlags.Critical)) == (AmbulanceFlags.Transporting | AmbulanceFlags.Critical))
			{
				parameters.m_Weights = new PathfindWeights(1f, 0f, 0f, 0f);
				parameters.m_IgnoredRules = RuleFlags.ForbidHeavyTraffic;
			}
			else
			{
				parameters.m_Weights = new PathfindWeights(1f, 1f, 1f, 1f);
			}
			if ((ambulance.m_State & (AmbulanceFlags.Returning | AmbulanceFlags.Dispatched | AmbulanceFlags.Transporting)) != AmbulanceFlags.Dispatched)
			{
				destination.m_RandomCost = 30f;
			}
			VehicleUtils.SetupPathfind(item: new SetupQueueItem(vehicleEntity, parameters, origin, destination), currentLane: ref currentLane, pathOwner: ref pathOwner, queue: m_PathfindQueue);
		}

		private Entity FindDistrict(Entity building)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (m_CurrentDistrictData.HasComponent(building))
			{
				return m_CurrentDistrictData[building].m_District;
			}
			return Entity.Null;
		}

		private void CheckServiceDispatches(Entity vehicleEntity, DynamicBuffer<ServiceDispatch> serviceDispatches, ref Game.Vehicles.Ambulance ambulance)
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			if ((ambulance.m_State & AmbulanceFlags.Transporting) != 0)
			{
				serviceDispatches.Clear();
				return;
			}
			if ((ambulance.m_State & AmbulanceFlags.Dispatched) != 0)
			{
				if (serviceDispatches.Length > 1)
				{
					serviceDispatches.RemoveRange(1, serviceDispatches.Length - 1);
				}
				return;
			}
			Entity val = Entity.Null;
			for (int i = 0; i < serviceDispatches.Length; i++)
			{
				Entity request = serviceDispatches[i].m_Request;
				if (m_HealthcareRequestData.HasComponent(request))
				{
					HealthcareRequest healthcareRequest = m_HealthcareRequestData[request];
					if (m_CurrentTransportData.HasComponent(healthcareRequest.m_Citizen) || m_CurrentBuildingData.HasComponent(healthcareRequest.m_Citizen))
					{
						val = request;
						break;
					}
				}
			}
			if (val != Entity.Null)
			{
				serviceDispatches[0] = new ServiceDispatch(val);
				if (serviceDispatches.Length > 1)
				{
					serviceDispatches.RemoveRange(1, serviceDispatches.Length - 1);
				}
				ambulance.m_State |= AmbulanceFlags.Dispatched;
			}
			else
			{
				serviceDispatches.Clear();
			}
		}

		private bool SelectNextDispatch(int jobIndex, Entity vehicleEntity, DynamicBuffer<AircraftNavigationLane> navigationLanes, DynamicBuffer<ServiceDispatch> serviceDispatches, ref Game.Vehicles.Ambulance ambulance, ref Aircraft aircraft, ref AircraftCurrentLane currentLane, ref PathOwner pathOwner, ref Target targetData)
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			if ((ambulance.m_State & AmbulanceFlags.Returning) == 0 && (ambulance.m_State & AmbulanceFlags.Dispatched) != 0 && serviceDispatches.Length > 0)
			{
				serviceDispatches.RemoveAt(0);
				ambulance.m_State &= ~AmbulanceFlags.Dispatched;
			}
			while ((ambulance.m_State & AmbulanceFlags.Dispatched) != 0 && serviceDispatches.Length > 0)
			{
				Entity request = serviceDispatches[0].m_Request;
				Entity val = Entity.Null;
				if (m_HealthcareRequestData.HasComponent(request))
				{
					val = m_HealthcareRequestData[request].m_Citizen;
				}
				Entity val2 = Entity.Null;
				if (m_CurrentTransportData.HasComponent(val))
				{
					val2 = m_CurrentTransportData[val].m_CurrentTransport;
				}
				else if (m_CurrentBuildingData.HasComponent(val))
				{
					val2 = m_CurrentBuildingData[val].m_CurrentBuilding;
				}
				if (!((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(val2))
				{
					serviceDispatches.RemoveAt(0);
					ambulance.m_State &= ~AmbulanceFlags.Dispatched;
					continue;
				}
				aircraft.m_Flags &= ~AircraftFlags.IgnoreParkedVehicle;
				ambulance.m_TargetPatient = val;
				ambulance.m_TargetLocation = val2;
				ambulance.m_State &= ~(AmbulanceFlags.Returning | AmbulanceFlags.FindHospital | AmbulanceFlags.AtTarget);
				Entity val3 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_HandleRequestArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HandleRequest>(jobIndex, val3, new HandleRequest(request, vehicleEntity, completed: false, pathConsumed: true));
				if (m_PathElements.HasBuffer(request))
				{
					DynamicBuffer<PathElement> appendPath = m_PathElements[request];
					if (appendPath.Length != 0)
					{
						DynamicBuffer<PathElement> val4 = m_PathElements[vehicleEntity];
						PathUtils.TrimPath(val4, ref pathOwner);
						float num = ambulance.m_PathElementTime * (float)val4.Length + m_PathInformationData[request].m_Duration;
						if (PathUtils.TryAppendPath(ref currentLane, navigationLanes, val4, appendPath))
						{
							ambulance.m_PathElementTime = num / (float)math.max(1, val4.Length);
							targetData.m_Target = val2;
							VehicleUtils.ClearEndOfPath(ref currentLane, navigationLanes);
							aircraft.m_Flags |= AircraftFlags.Emergency;
							return true;
						}
					}
				}
				VehicleUtils.SetTarget(ref pathOwner, ref targetData, val2);
				return true;
			}
			return false;
		}

		private void TransportToHospital(Owner owner, DynamicBuffer<ServiceDispatch> serviceDispatches, ref Game.Vehicles.Ambulance ambulance, ref Aircraft aircraft, ref PathOwner pathOwner, ref Target target)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			if ((ambulance.m_State & AmbulanceFlags.AnyHospital) != 0)
			{
				serviceDispatches.Clear();
				ambulance.m_State &= ~(AmbulanceFlags.Dispatched | AmbulanceFlags.AtTarget);
				ambulance.m_State |= AmbulanceFlags.FindHospital;
				VehicleUtils.SetTarget(ref pathOwner, ref target, Entity.Null);
			}
			else
			{
				ReturnToDepot(owner, serviceDispatches, ref ambulance, ref aircraft, ref pathOwner, ref target);
			}
		}

		private void ReturnToDepot(Owner owner, DynamicBuffer<ServiceDispatch> serviceDispatches, ref Game.Vehicles.Ambulance ambulance, ref Aircraft aircraft, ref PathOwner pathOwner, ref Target target)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			serviceDispatches.Clear();
			aircraft.m_Flags &= ~AircraftFlags.IgnoreParkedVehicle;
			ambulance.m_State &= ~(AmbulanceFlags.Dispatched | AmbulanceFlags.FindHospital | AmbulanceFlags.AtTarget);
			ambulance.m_State |= AmbulanceFlags.Returning;
			VehicleUtils.SetTarget(ref pathOwner, ref target, owner.m_Owner);
		}

		private void ResetPath(int jobIndex, Entity vehicleEntity, PathInformation pathInformation, DynamicBuffer<Passenger> passengers, DynamicBuffer<ServiceDispatch> serviceDispatches, ref Game.Vehicles.Ambulance ambulance, ref Aircraft aircraft, ref AircraftCurrentLane currentLane, ref Target target)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<PathElement> path = m_PathElements[vehicleEntity];
			PathUtils.ResetPath(ref currentLane, path);
			if ((ambulance.m_State & AmbulanceFlags.FindHospital) != 0)
			{
				target.m_Target = pathInformation.m_Destination;
				ambulance.m_State &= ~AmbulanceFlags.FindHospital;
				for (int i = 0; i < passengers.Length; i++)
				{
					Entity passenger = passengers[i].m_Passenger;
					if (m_CurrentVehicleData.HasComponent(passenger))
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Target>(jobIndex, passenger, target);
					}
				}
			}
			if ((ambulance.m_State & AmbulanceFlags.Dispatched) != 0 && serviceDispatches.Length > 0)
			{
				Entity request = serviceDispatches[0].m_Request;
				if (m_HealthcareRequestData.HasComponent(request))
				{
					aircraft.m_Flags |= AircraftFlags.Emergency;
				}
				else
				{
					aircraft.m_Flags &= ~AircraftFlags.Emergency;
				}
			}
			else if ((ambulance.m_State & AmbulanceFlags.Critical) != 0)
			{
				aircraft.m_Flags |= AircraftFlags.Emergency;
			}
			else
			{
				aircraft.m_Flags &= ~AircraftFlags.Emergency;
			}
			ambulance.m_PathElementTime = pathInformation.m_Duration / (float)math.max(1, path.Length);
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
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Unspawned> __Game_Objects_Unspawned_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Stopped> __Game_Objects_Stopped_RO_ComponentTypeHandle;

		public ComponentTypeHandle<Game.Vehicles.Ambulance> __Game_Vehicles_Ambulance_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Aircraft> __Game_Vehicles_Aircraft_RW_ComponentTypeHandle;

		public ComponentTypeHandle<AircraftCurrentLane> __Game_Vehicles_AircraftCurrentLane_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Target> __Game_Common_Target_RW_ComponentTypeHandle;

		public ComponentTypeHandle<PathOwner> __Game_Pathfind_PathOwner_RW_ComponentTypeHandle;

		public BufferTypeHandle<AircraftNavigationLane> __Game_Vehicles_AircraftNavigationLane_RW_BufferTypeHandle;

		public BufferTypeHandle<Passenger> __Game_Vehicles_Passenger_RW_BufferTypeHandle;

		public BufferTypeHandle<ServiceDispatch> __Game_Simulation_ServiceDispatch_RW_BufferTypeHandle;

		[ReadOnly]
		public EntityStorageInfoLookup __EntityStorageInfoLookup;

		[ReadOnly]
		public ComponentLookup<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkedCar> __Game_Vehicles_ParkedCar_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Controller> __Game_Vehicles_Controller_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HelicopterData> __Game_Prefabs_HelicopterData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HealthcareRequest> __Game_Simulation_HealthcareRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> __Game_Objects_SpawnLocation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Hospital> __Game_Buildings_Hospital_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentBuilding> __Game_Citizens_CurrentBuilding_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentTransport> __Game_Citizens_CurrentTransport_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HealthProblem> __Game_Citizens_HealthProblem_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TravelPurpose> __Game_Citizens_TravelPurpose_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> __Game_Creatures_CurrentVehicle_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentDistrict> __Game_Areas_CurrentDistrict_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferLookup;

		public ComponentLookup<Blocker> __Game_Vehicles_Blocker_RW_ComponentLookup;

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
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Objects_Unspawned_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Unspawned>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Pathfind_PathInformation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathInformation>(true);
			__Game_Objects_Stopped_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Stopped>(true);
			__Game_Vehicles_Ambulance_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Vehicles.Ambulance>(false);
			__Game_Vehicles_Aircraft_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Aircraft>(false);
			__Game_Vehicles_AircraftCurrentLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AircraftCurrentLane>(false);
			__Game_Common_Target_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Target>(false);
			__Game_Pathfind_PathOwner_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathOwner>(false);
			__Game_Vehicles_AircraftNavigationLane_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<AircraftNavigationLane>(false);
			__Game_Vehicles_Passenger_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Passenger>(false);
			__Game_Simulation_ServiceDispatch_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ServiceDispatch>(false);
			__EntityStorageInfoLookup = ((SystemState)(ref state)).GetEntityStorageInfoLookup();
			__Game_Pathfind_PathInformation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathInformation>(true);
			__Game_Vehicles_ParkedCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedCar>(true);
			__Game_Vehicles_Controller_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Controller>(true);
			__Game_Prefabs_HelicopterData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HelicopterData>(true);
			__Game_Simulation_HealthcareRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HealthcareRequest>(true);
			__Game_Objects_SpawnLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.SpawnLocation>(true);
			__Game_Buildings_Hospital_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.Hospital>(true);
			__Game_Citizens_CurrentBuilding_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentBuilding>(true);
			__Game_Citizens_CurrentTransport_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentTransport>(true);
			__Game_Citizens_HealthProblem_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HealthProblem>(true);
			__Game_Citizens_TravelPurpose_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TravelPurpose>(true);
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
			__Game_Creatures_CurrentVehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentVehicle>(true);
			__Game_Areas_CurrentDistrict_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentDistrict>(true);
			__Game_Vehicles_LayoutElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(true);
			__Game_Vehicles_Blocker_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Blocker>(false);
			__Game_Pathfind_PathElement_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PathElement>(false);
		}
	}

	private EndFrameBarrier m_EndFrameBarrier;

	private PathfindSetupSystem m_PathfindSetupSystem;

	private EntityQuery m_VehicleQuery;

	private EntityArchetype m_HandleRequestArchetype;

	private ComponentTypeSet m_MovingToParkedAircraftRemoveTypes;

	private ComponentTypeSet m_MovingToParkedAddTypes;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 10;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_PathfindSetupSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindSetupSystem>();
		m_VehicleQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[10]
		{
			ComponentType.ReadWrite<Game.Vehicles.Ambulance>(),
			ComponentType.ReadWrite<AircraftCurrentLane>(),
			ComponentType.ReadOnly<Owner>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadWrite<PathOwner>(),
			ComponentType.ReadWrite<Target>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<TripSource>(),
			ComponentType.Exclude<OutOfControl>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_HandleRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<HandleRequest>(),
			ComponentType.ReadWrite<Event>()
		});
		m_MovingToParkedAircraftRemoveTypes = new ComponentTypeSet((ComponentType[])(object)new ComponentType[12]
		{
			ComponentType.ReadWrite<Moving>(),
			ComponentType.ReadWrite<TransformFrame>(),
			ComponentType.ReadWrite<InterpolatedTransform>(),
			ComponentType.ReadWrite<AircraftNavigation>(),
			ComponentType.ReadWrite<AircraftNavigationLane>(),
			ComponentType.ReadWrite<AircraftCurrentLane>(),
			ComponentType.ReadWrite<PathOwner>(),
			ComponentType.ReadWrite<Target>(),
			ComponentType.ReadWrite<Blocker>(),
			ComponentType.ReadWrite<PathElement>(),
			ComponentType.ReadWrite<PathInformation>(),
			ComponentType.ReadWrite<ServiceDispatch>()
		});
		m_MovingToParkedAddTypes = new ComponentTypeSet(ComponentType.ReadWrite<ParkedCar>(), ComponentType.ReadWrite<Stopped>(), ComponentType.ReadWrite<Updated>());
		((ComponentSystemBase)this).RequireForUpdate(m_VehicleQuery);
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
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		MedicalAircraftTickJob medicalAircraftTickJob = new MedicalAircraftTickJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UnspawnedType = InternalCompilerInterface.GetComponentTypeHandle<Unspawned>(ref __TypeHandle.__Game_Objects_Unspawned_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathInformationType = InternalCompilerInterface.GetComponentTypeHandle<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_StoppedType = InternalCompilerInterface.GetComponentTypeHandle<Stopped>(ref __TypeHandle.__Game_Objects_Stopped_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AmbulanceType = InternalCompilerInterface.GetComponentTypeHandle<Game.Vehicles.Ambulance>(ref __TypeHandle.__Game_Vehicles_Ambulance_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AircraftType = InternalCompilerInterface.GetComponentTypeHandle<Aircraft>(ref __TypeHandle.__Game_Vehicles_Aircraft_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<AircraftCurrentLane>(ref __TypeHandle.__Game_Vehicles_AircraftCurrentLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TargetType = InternalCompilerInterface.GetComponentTypeHandle<Target>(ref __TypeHandle.__Game_Common_Target_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathOwnerType = InternalCompilerInterface.GetComponentTypeHandle<PathOwner>(ref __TypeHandle.__Game_Pathfind_PathOwner_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AircraftNavigationLaneType = InternalCompilerInterface.GetBufferTypeHandle<AircraftNavigationLane>(ref __TypeHandle.__Game_Vehicles_AircraftNavigationLane_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PassengerType = InternalCompilerInterface.GetBufferTypeHandle<Passenger>(ref __TypeHandle.__Game_Vehicles_Passenger_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceDispatchType = InternalCompilerInterface.GetBufferTypeHandle<ServiceDispatch>(ref __TypeHandle.__Game_Simulation_ServiceDispatch_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EntityLookup = InternalCompilerInterface.GetEntityStorageInfoLookup(ref __TypeHandle.__EntityStorageInfoLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathInformationData = InternalCompilerInterface.GetComponentLookup<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedCarData = InternalCompilerInterface.GetComponentLookup<ParkedCar>(ref __TypeHandle.__Game_Vehicles_ParkedCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ControllerData = InternalCompilerInterface.GetComponentLookup<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabHelicopterData = InternalCompilerInterface.GetComponentLookup<HelicopterData>(ref __TypeHandle.__Game_Prefabs_HelicopterData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HealthcareRequestData = InternalCompilerInterface.GetComponentLookup<HealthcareRequest>(ref __TypeHandle.__Game_Simulation_HealthcareRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnLocationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HospitalData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.Hospital>(ref __TypeHandle.__Game_Buildings_Hospital_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentBuildingData = InternalCompilerInterface.GetComponentLookup<CurrentBuilding>(ref __TypeHandle.__Game_Citizens_CurrentBuilding_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentTransportData = InternalCompilerInterface.GetComponentLookup<CurrentTransport>(ref __TypeHandle.__Game_Citizens_CurrentTransport_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HealthProblemData = InternalCompilerInterface.GetComponentLookup<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TravelPurposeData = InternalCompilerInterface.GetComponentLookup<TravelPurpose>(ref __TypeHandle.__Game_Citizens_TravelPurpose_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CitizenData = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentVehicleData = InternalCompilerInterface.GetComponentLookup<CurrentVehicle>(ref __TypeHandle.__Game_Creatures_CurrentVehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentDistrictData = InternalCompilerInterface.GetComponentLookup<CurrentDistrict>(ref __TypeHandle.__Game_Areas_CurrentDistrict_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LayoutElements = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BlockerData = InternalCompilerInterface.GetComponentLookup<Blocker>(ref __TypeHandle.__Game_Vehicles_Blocker_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathElements = InternalCompilerInterface.GetBufferLookup<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RandomSeed = RandomSeed.Next(),
			m_HandleRequestArchetype = m_HandleRequestArchetype,
			m_MovingToParkedAircraftRemoveTypes = m_MovingToParkedAircraftRemoveTypes,
			m_MovingToParkedAddTypes = m_MovingToParkedAddTypes
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		medicalAircraftTickJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		medicalAircraftTickJob.m_PathfindQueue = m_PathfindSetupSystem.GetQueue(this, 64).AsParallelWriter();
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<MedicalAircraftTickJob>(medicalAircraftTickJob, m_VehicleQuery, ((SystemBase)this).Dependency);
		m_PathfindSetupSystem.AddQueueWriter(val2);
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
	public MedicalAircraftAISystem()
	{
	}
}
