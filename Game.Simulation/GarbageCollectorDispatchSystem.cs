using System.Runtime.CompilerServices;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Net;
using Game.Pathfind;
using Game.Prefabs;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class GarbageCollectorDispatchSystem : GameSystemBase
{
	private struct VehicleDispatch
	{
		public Entity m_Request;

		public Entity m_Source;

		public VehicleDispatch(Entity request, Entity source)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			m_Request = request;
			m_Source = source;
		}
	}

	[BurstCompile]
	private struct GarbageDispatchJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Dispatched> m_DispatchedType;

		[ReadOnly]
		public ComponentTypeHandle<PathInformation> m_PathInformationType;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		public ComponentTypeHandle<GarbageCollectionRequest> m_GarbageCollectionRequestType;

		public ComponentTypeHandle<ServiceRequest> m_ServiceRequestType;

		[NativeDisableContainerSafetyRestriction]
		[ReadOnly]
		public ComponentLookup<GarbageCollectionRequest> m_GarbageCollectionRequestData;

		[ReadOnly]
		public ComponentLookup<ParkedCar> m_ParkedCarData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<CurrentDistrict> m_CurrentDistrictData;

		[ReadOnly]
		public BufferLookup<ServiceDispatch> m_ServiceDispatches;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<GarbageProducer> m_GarbageProducerData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Game.Buildings.GarbageFacility> m_GarbageFacilityData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Game.Vehicles.GarbageTruck> m_GarbageTruckData;

		[ReadOnly]
		public GarbageParameterData m_GarbageParameters;

		[ReadOnly]
		public uint m_UpdateFrameIndex;

		[ReadOnly]
		public uint m_NextUpdateFrameIndex;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<VehicleDispatch> m_VehicleDispatches;

		public ParallelWriter<SetupQueueItem> m_PathfindQueue;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			uint index = ((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index;
			if (index == m_NextUpdateFrameIndex && !((ArchetypeChunk)(ref chunk)).Has<Dispatched>(ref m_DispatchedType) && !((ArchetypeChunk)(ref chunk)).Has<PathInformation>(ref m_PathInformationType))
			{
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				NativeArray<GarbageCollectionRequest> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<GarbageCollectionRequest>(ref m_GarbageCollectionRequestType);
				NativeArray<ServiceRequest> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ServiceRequest>(ref m_ServiceRequestType);
				for (int i = 0; i < nativeArray2.Length; i++)
				{
					Entity val = nativeArray[i];
					GarbageCollectionRequest garbageCollectionRequest = nativeArray2[i];
					ServiceRequest serviceRequest = nativeArray3[i];
					if ((serviceRequest.m_Flags & ServiceRequestFlags.Reversed) != 0)
					{
						if (!ValidateReversed(val, garbageCollectionRequest.m_Target))
						{
							((ParallelWriter)(ref m_CommandBuffer)).DestroyEntity(unfilteredChunkIndex, val);
							continue;
						}
						if (SimulationUtils.TickServiceRequest(ref serviceRequest))
						{
							FindVehicleTarget(unfilteredChunkIndex, val, garbageCollectionRequest.m_Target);
						}
					}
					else
					{
						if (!ValidateTarget(val, garbageCollectionRequest.m_Target))
						{
							((ParallelWriter)(ref m_CommandBuffer)).DestroyEntity(unfilteredChunkIndex, val);
							continue;
						}
						if (SimulationUtils.TickServiceRequest(ref serviceRequest))
						{
							FindVehicleSource(unfilteredChunkIndex, val, garbageCollectionRequest.m_Target);
						}
					}
					nativeArray2[i] = garbageCollectionRequest;
					nativeArray3[i] = serviceRequest;
				}
			}
			if (index != m_UpdateFrameIndex)
			{
				return;
			}
			NativeArray<Dispatched> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Dispatched>(ref m_DispatchedType);
			NativeArray<GarbageCollectionRequest> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<GarbageCollectionRequest>(ref m_GarbageCollectionRequestType);
			NativeArray<ServiceRequest> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ServiceRequest>(ref m_ServiceRequestType);
			if (nativeArray4.Length != 0)
			{
				NativeArray<Entity> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				for (int j = 0; j < nativeArray4.Length; j++)
				{
					Entity val2 = nativeArray7[j];
					Dispatched dispatched = nativeArray4[j];
					GarbageCollectionRequest garbageCollectionRequest2 = nativeArray5[j];
					ServiceRequest serviceRequest2 = nativeArray6[j];
					if (ValidateHandler(val2, dispatched.m_Handler))
					{
						serviceRequest2.m_Cooldown = 0;
					}
					else if (serviceRequest2.m_Cooldown == 0)
					{
						serviceRequest2.m_Cooldown = 1;
					}
					else
					{
						if (!ValidateTarget(val2, garbageCollectionRequest2.m_Target))
						{
							((ParallelWriter)(ref m_CommandBuffer)).DestroyEntity(unfilteredChunkIndex, val2);
							continue;
						}
						ResetFailedRequest(unfilteredChunkIndex, val2, dispatched: true, ref garbageCollectionRequest2, ref serviceRequest2);
					}
					nativeArray5[j] = garbageCollectionRequest2;
					nativeArray6[j] = serviceRequest2;
				}
				return;
			}
			NativeArray<PathInformation> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathInformation>(ref m_PathInformationType);
			if (nativeArray8.Length == 0)
			{
				return;
			}
			NativeArray<Entity> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			for (int k = 0; k < nativeArray5.Length; k++)
			{
				Entity val3 = nativeArray9[k];
				GarbageCollectionRequest garbageCollectionRequest3 = nativeArray5[k];
				PathInformation pathInformation = nativeArray8[k];
				ServiceRequest serviceRequest3 = nativeArray6[k];
				if ((serviceRequest3.m_Flags & ServiceRequestFlags.Reversed) != 0)
				{
					if (!ValidateReversed(val3, garbageCollectionRequest3.m_Target))
					{
						((ParallelWriter)(ref m_CommandBuffer)).DestroyEntity(unfilteredChunkIndex, val3);
						continue;
					}
					if (pathInformation.m_Destination != Entity.Null)
					{
						ResetReverseRequest(unfilteredChunkIndex, val3, pathInformation, ref serviceRequest3);
					}
					else
					{
						ResetFailedRequest(unfilteredChunkIndex, val3, dispatched: false, ref garbageCollectionRequest3, ref serviceRequest3);
					}
				}
				else
				{
					if (!ValidateTarget(val3, garbageCollectionRequest3.m_Target))
					{
						((ParallelWriter)(ref m_CommandBuffer)).DestroyEntity(unfilteredChunkIndex, val3);
						continue;
					}
					if (pathInformation.m_Origin != Entity.Null)
					{
						DispatchVehicle(unfilteredChunkIndex, val3, pathInformation);
					}
					else
					{
						ResetFailedRequest(unfilteredChunkIndex, val3, dispatched: false, ref garbageCollectionRequest3, ref serviceRequest3);
					}
				}
				nativeArray5[k] = garbageCollectionRequest3;
				nativeArray6[k] = serviceRequest3;
			}
		}

		private bool ValidateReversed(Entity entity, Entity source)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			Game.Buildings.GarbageFacility garbageFacility = default(Game.Buildings.GarbageFacility);
			if (m_GarbageFacilityData.TryGetComponent(source, ref garbageFacility))
			{
				if ((garbageFacility.m_Flags & (GarbageFacilityFlags.HasAvailableGarbageTrucks | GarbageFacilityFlags.HasAvailableSpace)) != (GarbageFacilityFlags.HasAvailableGarbageTrucks | GarbageFacilityFlags.HasAvailableSpace))
				{
					return false;
				}
				if (garbageFacility.m_TargetRequest != entity)
				{
					if (m_GarbageCollectionRequestData.HasComponent(garbageFacility.m_TargetRequest))
					{
						return false;
					}
					garbageFacility.m_TargetRequest = entity;
					m_GarbageFacilityData[source] = garbageFacility;
				}
				return true;
			}
			Game.Vehicles.GarbageTruck garbageTruck = default(Game.Vehicles.GarbageTruck);
			if (m_GarbageTruckData.TryGetComponent(source, ref garbageTruck))
			{
				if ((garbageTruck.m_State & (GarbageTruckFlags.Disabled | GarbageTruckFlags.EstimatedFull)) != 0 || garbageTruck.m_RequestCount > 1 || m_ParkedCarData.HasComponent(source))
				{
					return false;
				}
				if (garbageTruck.m_TargetRequest != entity)
				{
					if (m_GarbageCollectionRequestData.HasComponent(garbageTruck.m_TargetRequest))
					{
						return false;
					}
					garbageTruck.m_TargetRequest = entity;
					m_GarbageTruckData[source] = garbageTruck;
				}
				return true;
			}
			return false;
		}

		private bool ValidateHandler(Entity entity, Entity handler)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<ServiceDispatch> val = default(DynamicBuffer<ServiceDispatch>);
			if (m_ServiceDispatches.TryGetBuffer(handler, ref val))
			{
				for (int i = 0; i < val.Length; i++)
				{
					if (val[i].m_Request == entity)
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool ValidateTarget(Entity entity, Entity target)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			GarbageProducer garbageProducer = default(GarbageProducer);
			if (!m_GarbageProducerData.TryGetComponent(target, ref garbageProducer))
			{
				return false;
			}
			if (garbageProducer.m_Garbage <= m_GarbageParameters.m_RequestGarbageLimit)
			{
				return false;
			}
			if (garbageProducer.m_CollectionRequest != entity)
			{
				if (m_GarbageCollectionRequestData.HasComponent(garbageProducer.m_CollectionRequest))
				{
					return false;
				}
				garbageProducer.m_CollectionRequest = entity;
				m_GarbageProducerData[target] = garbageProducer;
			}
			return true;
		}

		private void ResetReverseRequest(int jobIndex, Entity entity, PathInformation pathInformation, ref ServiceRequest serviceRequest)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			VehicleDispatch vehicleDispatch = new VehicleDispatch(entity, pathInformation.m_Destination);
			m_VehicleDispatches.Enqueue(vehicleDispatch);
			SimulationUtils.ResetReverseRequest(ref serviceRequest);
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<PathInformation>(jobIndex, entity);
		}

		private void ResetFailedRequest(int jobIndex, Entity entity, bool dispatched, ref GarbageCollectionRequest garbageCollectionRequest, ref ServiceRequest serviceRequest)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			SimulationUtils.ResetFailedRequest(ref serviceRequest);
			garbageCollectionRequest.m_DispatchIndex++;
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<PathInformation>(jobIndex, entity);
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<PathElement>(jobIndex, entity);
			if (dispatched)
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Dispatched>(jobIndex, entity);
			}
		}

		private void DispatchVehicle(int jobIndex, Entity entity, PathInformation pathInformation)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			Entity val = pathInformation.m_Origin;
			Owner owner = default(Owner);
			if (m_ParkedCarData.HasComponent(val) && m_OwnerData.TryGetComponent(val, ref owner))
			{
				val = owner.m_Owner;
			}
			VehicleDispatch vehicleDispatch = new VehicleDispatch(entity, val);
			m_VehicleDispatches.Enqueue(vehicleDispatch);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Dispatched>(jobIndex, entity, new Dispatched(val));
		}

		private void FindVehicleSource(int jobIndex, Entity requestEntity, Entity targetEntity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			Entity entity = Entity.Null;
			if (m_CurrentDistrictData.HasComponent(targetEntity))
			{
				entity = m_CurrentDistrictData[targetEntity].m_District;
			}
			PathfindParameters parameters = new PathfindParameters
			{
				m_MaxSpeed = float2.op_Implicit(111.111115f),
				m_WalkSpeed = float2.op_Implicit(5.555556f),
				m_Weights = new PathfindWeights(1f, 1f, 1f, 1f),
				m_Methods = PathMethod.Road,
				m_IgnoredRules = (RuleFlags.ForbidCombustionEngines | RuleFlags.ForbidTransitTraffic | RuleFlags.ForbidHeavyTraffic | RuleFlags.ForbidPrivateTraffic | RuleFlags.ForbidSlowTraffic)
			};
			SetupQueueTarget origin = new SetupQueueTarget
			{
				m_Type = SetupTargetType.GarbageCollector,
				m_Methods = PathMethod.Road,
				m_RoadTypes = RoadTypes.Car,
				m_Entity = entity
			};
			SetupQueueTarget destination = new SetupQueueTarget
			{
				m_Type = SetupTargetType.CurrentLocation,
				m_Methods = PathMethod.Road,
				m_RoadTypes = RoadTypes.Car,
				m_Entity = targetEntity
			};
			m_PathfindQueue.Enqueue(new SetupQueueItem(requestEntity, parameters, origin, destination));
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathInformation>(jobIndex, requestEntity, default(PathInformation));
			((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<PathElement>(jobIndex, requestEntity);
		}

		private void FindVehicleTarget(int jobIndex, Entity requestEntity, Entity vehicleSource)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			PathfindParameters parameters = new PathfindParameters
			{
				m_MaxSpeed = float2.op_Implicit(111.111115f),
				m_WalkSpeed = float2.op_Implicit(5.555556f),
				m_Weights = new PathfindWeights(1f, 1f, 1f, 1f),
				m_Methods = PathMethod.Road,
				m_IgnoredRules = (RuleFlags.ForbidCombustionEngines | RuleFlags.ForbidTransitTraffic | RuleFlags.ForbidHeavyTraffic | RuleFlags.ForbidPrivateTraffic | RuleFlags.ForbidSlowTraffic)
			};
			SetupQueueTarget origin = new SetupQueueTarget
			{
				m_Type = SetupTargetType.CurrentLocation,
				m_Methods = PathMethod.Road,
				m_RoadTypes = RoadTypes.Car,
				m_Entity = vehicleSource
			};
			SetupQueueTarget destination = new SetupQueueTarget
			{
				m_Type = SetupTargetType.GarbageCollectorRequest,
				m_Methods = PathMethod.Road,
				m_RoadTypes = RoadTypes.Car
			};
			Game.Vehicles.GarbageTruck garbageTruck = default(Game.Vehicles.GarbageTruck);
			if (m_GarbageTruckData.TryGetComponent(vehicleSource, ref garbageTruck) && (garbageTruck.m_State & GarbageTruckFlags.Returning) == 0)
			{
				origin.m_Flags |= SetupTargetFlags.PathEnd;
			}
			m_PathfindQueue.Enqueue(new SetupQueueItem(requestEntity, parameters, origin, destination));
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathInformation>(jobIndex, requestEntity, default(PathInformation));
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct DispatchVehiclesJob : IJob
	{
		public NativeQueue<VehicleDispatch> m_VehicleDispatches;

		public ComponentLookup<ServiceRequest> m_ServiceRequestData;

		public BufferLookup<ServiceDispatch> m_ServiceDispatches;

		public void Execute()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			VehicleDispatch vehicleDispatch = default(VehicleDispatch);
			DynamicBuffer<ServiceDispatch> val = default(DynamicBuffer<ServiceDispatch>);
			ServiceRequest serviceRequest = default(ServiceRequest);
			while (m_VehicleDispatches.TryDequeue(ref vehicleDispatch))
			{
				if (m_ServiceDispatches.TryGetBuffer(vehicleDispatch.m_Source, ref val))
				{
					val.Add(new ServiceDispatch(vehicleDispatch.m_Request));
				}
				else if (m_ServiceRequestData.TryGetComponent(vehicleDispatch.m_Source, ref serviceRequest))
				{
					serviceRequest.m_Flags |= ServiceRequestFlags.SkipCooldown;
					m_ServiceRequestData[vehicleDispatch.m_Source] = serviceRequest;
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Dispatched> __Game_Simulation_Dispatched_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentTypeHandle;

		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		public ComponentTypeHandle<GarbageCollectionRequest> __Game_Simulation_GarbageCollectionRequest_RW_ComponentTypeHandle;

		public ComponentTypeHandle<ServiceRequest> __Game_Simulation_ServiceRequest_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<GarbageCollectionRequest> __Game_Simulation_GarbageCollectionRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkedCar> __Game_Vehicles_ParkedCar_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentDistrict> __Game_Areas_CurrentDistrict_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ServiceDispatch> __Game_Simulation_ServiceDispatch_RO_BufferLookup;

		public ComponentLookup<GarbageProducer> __Game_Buildings_GarbageProducer_RW_ComponentLookup;

		public ComponentLookup<Game.Buildings.GarbageFacility> __Game_Buildings_GarbageFacility_RW_ComponentLookup;

		public ComponentLookup<Game.Vehicles.GarbageTruck> __Game_Vehicles_GarbageTruck_RW_ComponentLookup;

		public ComponentLookup<ServiceRequest> __Game_Simulation_ServiceRequest_RW_ComponentLookup;

		public BufferLookup<ServiceDispatch> __Game_Simulation_ServiceDispatch_RW_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Simulation_Dispatched_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Dispatched>(true);
			__Game_Pathfind_PathInformation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathInformation>(true);
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Simulation_GarbageCollectionRequest_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<GarbageCollectionRequest>(false);
			__Game_Simulation_ServiceRequest_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ServiceRequest>(false);
			__Game_Simulation_GarbageCollectionRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarbageCollectionRequest>(true);
			__Game_Vehicles_ParkedCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedCar>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Areas_CurrentDistrict_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentDistrict>(true);
			__Game_Simulation_ServiceDispatch_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceDispatch>(true);
			__Game_Buildings_GarbageProducer_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarbageProducer>(false);
			__Game_Buildings_GarbageFacility_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.GarbageFacility>(false);
			__Game_Vehicles_GarbageTruck_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.GarbageTruck>(false);
			__Game_Simulation_ServiceRequest_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceRequest>(false);
			__Game_Simulation_ServiceDispatch_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceDispatch>(false);
		}
	}

	private EndFrameBarrier m_EndFrameBarrier;

	private SimulationSystem m_SimulationSystem;

	private PathfindSetupSystem m_PathfindSetupSystem;

	private EntityQuery m_RequestQuery;

	private EntityQuery m_GarbageSettingsQuery;

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
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_PathfindSetupSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindSetupSystem>();
		m_RequestQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<GarbageCollectionRequest>(),
			ComponentType.ReadOnly<UpdateFrame>()
		});
		m_GarbageSettingsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<GarbageParameterData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_RequestQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		uint num = (m_SimulationSystem.frameIndex >> 4) & 0x1F;
		uint nextUpdateFrameIndex = (num + 4) & 0x1F;
		NativeQueue<VehicleDispatch> vehicleDispatches = default(NativeQueue<VehicleDispatch>);
		vehicleDispatches._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		GarbageDispatchJob garbageDispatchJob = new GarbageDispatchJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DispatchedType = InternalCompilerInterface.GetComponentTypeHandle<Dispatched>(ref __TypeHandle.__Game_Simulation_Dispatched_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathInformationType = InternalCompilerInterface.GetComponentTypeHandle<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageCollectionRequestType = InternalCompilerInterface.GetComponentTypeHandle<GarbageCollectionRequest>(ref __TypeHandle.__Game_Simulation_GarbageCollectionRequest_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceRequestType = InternalCompilerInterface.GetComponentTypeHandle<ServiceRequest>(ref __TypeHandle.__Game_Simulation_ServiceRequest_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageCollectionRequestData = InternalCompilerInterface.GetComponentLookup<GarbageCollectionRequest>(ref __TypeHandle.__Game_Simulation_GarbageCollectionRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedCarData = InternalCompilerInterface.GetComponentLookup<ParkedCar>(ref __TypeHandle.__Game_Vehicles_ParkedCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentDistrictData = InternalCompilerInterface.GetComponentLookup<CurrentDistrict>(ref __TypeHandle.__Game_Areas_CurrentDistrict_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceDispatches = InternalCompilerInterface.GetBufferLookup<ServiceDispatch>(ref __TypeHandle.__Game_Simulation_ServiceDispatch_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageProducerData = InternalCompilerInterface.GetComponentLookup<GarbageProducer>(ref __TypeHandle.__Game_Buildings_GarbageProducer_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageFacilityData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.GarbageFacility>(ref __TypeHandle.__Game_Buildings_GarbageFacility_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageTruckData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.GarbageTruck>(ref __TypeHandle.__Game_Vehicles_GarbageTruck_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageParameters = ((EntityQuery)(ref m_GarbageSettingsQuery)).GetSingleton<GarbageParameterData>(),
			m_UpdateFrameIndex = num,
			m_NextUpdateFrameIndex = nextUpdateFrameIndex
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		garbageDispatchJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		garbageDispatchJob.m_VehicleDispatches = vehicleDispatches.AsParallelWriter();
		garbageDispatchJob.m_PathfindQueue = m_PathfindSetupSystem.GetQueue(this, 64).AsParallelWriter();
		GarbageDispatchJob garbageDispatchJob2 = garbageDispatchJob;
		DispatchVehiclesJob obj = new DispatchVehiclesJob
		{
			m_VehicleDispatches = vehicleDispatches,
			m_ServiceRequestData = InternalCompilerInterface.GetComponentLookup<ServiceRequest>(ref __TypeHandle.__Game_Simulation_ServiceRequest_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceDispatches = InternalCompilerInterface.GetBufferLookup<ServiceDispatch>(ref __TypeHandle.__Game_Simulation_ServiceDispatch_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<GarbageDispatchJob>(garbageDispatchJob2, m_RequestQuery, ((SystemBase)this).Dependency);
		JobHandle val3 = IJobExtensions.Schedule<DispatchVehiclesJob>(obj, val2);
		vehicleDispatches.Dispose(val3);
		m_PathfindSetupSystem.AddQueueWriter(val2);
		m_EndFrameBarrier.AddJobHandleForProducer(val2);
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
	public GarbageCollectorDispatchSystem()
	{
	}
}
