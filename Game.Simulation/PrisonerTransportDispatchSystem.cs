using System.Runtime.CompilerServices;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Net;
using Game.Pathfind;
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
public class PrisonerTransportDispatchSystem : GameSystemBase
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
	private struct PrisonerTransportDispatchJob : IJobChunk
	{
		[ReadOnly]
		public uint m_UpdateFrameIndex;

		[ReadOnly]
		public uint m_NextUpdateFrameIndex;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrisonerTransportRequest> m_PrisonerTransportRequestType;

		[ReadOnly]
		public ComponentTypeHandle<Dispatched> m_DispatchedType;

		[ReadOnly]
		public ComponentTypeHandle<PathInformation> m_PathInformationType;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		public ComponentTypeHandle<ServiceRequest> m_ServiceRequestType;

		[ReadOnly]
		public ComponentLookup<PrisonerTransportRequest> m_PrisonerTransportRequestData;

		[ReadOnly]
		public ComponentLookup<ParkedCar> m_ParkedCarData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<CurrentDistrict> m_CurrentDistrictData;

		[ReadOnly]
		public BufferLookup<ServiceDispatch> m_ServiceDispatches;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<PoliceStation> m_PoliceStationData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Prison> m_PrisonData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<PublicTransport> m_PublicTransportData;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<VehicleDispatch> m_VehicleDispatches;

		public ParallelWriter<SetupQueueItem> m_PathfindQueue;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			uint index = ((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index;
			if (index == m_NextUpdateFrameIndex && !((ArchetypeChunk)(ref chunk)).Has<Dispatched>(ref m_DispatchedType) && !((ArchetypeChunk)(ref chunk)).Has<PathInformation>(ref m_PathInformationType))
			{
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				NativeArray<PrisonerTransportRequest> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrisonerTransportRequest>(ref m_PrisonerTransportRequestType);
				NativeArray<ServiceRequest> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ServiceRequest>(ref m_ServiceRequestType);
				for (int i = 0; i < nativeArray2.Length; i++)
				{
					Entity val = nativeArray[i];
					PrisonerTransportRequest prisonerTransportRequest = nativeArray2[i];
					ServiceRequest serviceRequest = nativeArray3[i];
					if ((serviceRequest.m_Flags & ServiceRequestFlags.Reversed) != 0)
					{
						if (!ValidateReversed(val, prisonerTransportRequest.m_Target))
						{
							((ParallelWriter)(ref m_CommandBuffer)).DestroyEntity(unfilteredChunkIndex, val);
							continue;
						}
						if (SimulationUtils.TickServiceRequest(ref serviceRequest))
						{
							FindVehicleTarget(unfilteredChunkIndex, val, prisonerTransportRequest.m_Target);
						}
					}
					else
					{
						if (!ValidateTarget(val, prisonerTransportRequest.m_Target))
						{
							((ParallelWriter)(ref m_CommandBuffer)).DestroyEntity(unfilteredChunkIndex, val);
							continue;
						}
						if (SimulationUtils.TickServiceRequest(ref serviceRequest))
						{
							FindVehicleSource(unfilteredChunkIndex, val, prisonerTransportRequest.m_Target);
						}
					}
					nativeArray3[i] = serviceRequest;
				}
			}
			if (index != m_UpdateFrameIndex)
			{
				return;
			}
			NativeArray<Dispatched> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Dispatched>(ref m_DispatchedType);
			NativeArray<PrisonerTransportRequest> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrisonerTransportRequest>(ref m_PrisonerTransportRequestType);
			NativeArray<ServiceRequest> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ServiceRequest>(ref m_ServiceRequestType);
			if (nativeArray4.Length != 0)
			{
				NativeArray<Entity> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				for (int j = 0; j < nativeArray4.Length; j++)
				{
					Entity val2 = nativeArray7[j];
					Dispatched dispatched = nativeArray4[j];
					PrisonerTransportRequest prisonerTransportRequest2 = nativeArray5[j];
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
						if (!ValidateTarget(val2, prisonerTransportRequest2.m_Target))
						{
							((ParallelWriter)(ref m_CommandBuffer)).DestroyEntity(unfilteredChunkIndex, val2);
							continue;
						}
						ResetFailedRequest(unfilteredChunkIndex, val2, dispatched: true, ref serviceRequest2);
					}
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
				PrisonerTransportRequest prisonerTransportRequest3 = nativeArray5[k];
				PathInformation pathInformation = nativeArray8[k];
				ServiceRequest serviceRequest3 = nativeArray6[k];
				if ((serviceRequest3.m_Flags & ServiceRequestFlags.Reversed) != 0)
				{
					if (!ValidateReversed(val3, prisonerTransportRequest3.m_Target))
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
						ResetFailedRequest(unfilteredChunkIndex, val3, dispatched: false, ref serviceRequest3);
					}
				}
				else
				{
					if (!ValidateTarget(val3, prisonerTransportRequest3.m_Target))
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
						ResetFailedRequest(unfilteredChunkIndex, val3, dispatched: false, ref serviceRequest3);
					}
				}
				nativeArray6[k] = serviceRequest3;
			}
		}

		private bool ValidateReversed(Entity entity, Entity source)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			Prison prison = default(Prison);
			if (m_PrisonData.TryGetComponent(source, ref prison))
			{
				if ((prison.m_Flags & PrisonFlags.HasAvailablePrisonVans) == 0)
				{
					return false;
				}
				if (prison.m_TargetRequest != entity)
				{
					if (m_PrisonerTransportRequestData.HasComponent(prison.m_TargetRequest))
					{
						return false;
					}
					prison.m_TargetRequest = entity;
					m_PrisonData[source] = prison;
				}
				return true;
			}
			PublicTransport publicTransport = default(PublicTransport);
			if (m_PublicTransportData.TryGetComponent(source, ref publicTransport))
			{
				if ((publicTransport.m_State & (PublicTransportFlags.EnRoute | PublicTransportFlags.RequiresMaintenance | PublicTransportFlags.Disabled | PublicTransportFlags.Full)) != 0 || publicTransport.m_RequestCount > 1 || m_ParkedCarData.HasComponent(source))
				{
					return false;
				}
				if (publicTransport.m_TargetRequest != entity)
				{
					if (m_PrisonerTransportRequestData.HasComponent(publicTransport.m_TargetRequest))
					{
						return false;
					}
					publicTransport.m_TargetRequest = entity;
					m_PublicTransportData[source] = publicTransport;
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
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			PoliceStation policeStation = default(PoliceStation);
			if (!m_PoliceStationData.TryGetComponent(target, ref policeStation))
			{
				return false;
			}
			if ((policeStation.m_Flags & PoliceStationFlags.NeedPrisonerTransport) == 0)
			{
				return false;
			}
			if (policeStation.m_PrisonerTransportRequest != entity)
			{
				if (m_PrisonerTransportRequestData.HasComponent(policeStation.m_PrisonerTransportRequest))
				{
					return false;
				}
				policeStation.m_PrisonerTransportRequest = entity;
				m_PoliceStationData[target] = policeStation;
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

		private void ResetFailedRequest(int jobIndex, Entity entity, bool dispatched, ref ServiceRequest serviceRequest)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			SimulationUtils.ResetFailedRequest(ref serviceRequest);
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

		private void FindVehicleSource(int jobIndex, Entity requestEntity, Entity target)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			Entity entity = Entity.Null;
			if (m_CurrentDistrictData.HasComponent(target))
			{
				entity = m_CurrentDistrictData[target].m_District;
			}
			PathfindParameters parameters = new PathfindParameters
			{
				m_MaxSpeed = float2.op_Implicit(111.111115f),
				m_WalkSpeed = float2.op_Implicit(5.555556f),
				m_Weights = new PathfindWeights(1f, 1f, 1f, 1f),
				m_Methods = PathMethod.Road,
				m_IgnoredRules = (RuleFlags.ForbidCombustionEngines | RuleFlags.ForbidHeavyTraffic | RuleFlags.ForbidPrivateTraffic | RuleFlags.ForbidSlowTraffic)
			};
			SetupQueueTarget origin = new SetupQueueTarget
			{
				m_Type = SetupTargetType.PrisonerTransport,
				m_Methods = PathMethod.Road,
				m_RoadTypes = RoadTypes.Car,
				m_Entity = entity
			};
			SetupQueueTarget destination = new SetupQueueTarget
			{
				m_Type = SetupTargetType.CurrentLocation,
				m_Methods = PathMethod.Road,
				m_RoadTypes = RoadTypes.Car,
				m_Entity = target
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
				m_IgnoredRules = (RuleFlags.ForbidCombustionEngines | RuleFlags.ForbidHeavyTraffic | RuleFlags.ForbidPrivateTraffic | RuleFlags.ForbidSlowTraffic)
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
				m_Type = SetupTargetType.PrisonerTransportRequest,
				m_Methods = PathMethod.Road,
				m_RoadTypes = RoadTypes.Car
			};
			PublicTransport publicTransport = default(PublicTransport);
			if (m_PublicTransportData.TryGetComponent(vehicleSource, ref publicTransport) && (publicTransport.m_State & PublicTransportFlags.Returning) == 0)
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
		public ComponentTypeHandle<PrisonerTransportRequest> __Game_Simulation_PrisonerTransportRequest_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Dispatched> __Game_Simulation_Dispatched_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentTypeHandle;

		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		public ComponentTypeHandle<ServiceRequest> __Game_Simulation_ServiceRequest_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrisonerTransportRequest> __Game_Simulation_PrisonerTransportRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkedCar> __Game_Vehicles_ParkedCar_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentDistrict> __Game_Areas_CurrentDistrict_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ServiceDispatch> __Game_Simulation_ServiceDispatch_RO_BufferLookup;

		public ComponentLookup<PoliceStation> __Game_Buildings_PoliceStation_RW_ComponentLookup;

		public ComponentLookup<Prison> __Game_Buildings_Prison_RW_ComponentLookup;

		public ComponentLookup<PublicTransport> __Game_Vehicles_PublicTransport_RW_ComponentLookup;

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
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
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
			__Game_Simulation_PrisonerTransportRequest_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrisonerTransportRequest>(true);
			__Game_Simulation_Dispatched_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Dispatched>(true);
			__Game_Pathfind_PathInformation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathInformation>(true);
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Simulation_ServiceRequest_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ServiceRequest>(false);
			__Game_Simulation_PrisonerTransportRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrisonerTransportRequest>(true);
			__Game_Vehicles_ParkedCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedCar>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Areas_CurrentDistrict_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentDistrict>(true);
			__Game_Simulation_ServiceDispatch_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceDispatch>(true);
			__Game_Buildings_PoliceStation_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PoliceStation>(false);
			__Game_Buildings_Prison_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Prison>(false);
			__Game_Vehicles_PublicTransport_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PublicTransport>(false);
			__Game_Simulation_ServiceRequest_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceRequest>(false);
			__Game_Simulation_ServiceDispatch_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceDispatch>(false);
		}
	}

	private EndFrameBarrier m_EndFrameBarrier;

	private SimulationSystem m_SimulationSystem;

	private PathfindSetupSystem m_PathfindSetupSystem;

	private EntityQuery m_RequestQuery;

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
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_PathfindSetupSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindSetupSystem>();
		m_RequestQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PrisonerTransportRequest>(),
			ComponentType.ReadOnly<UpdateFrame>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_RequestQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		uint num = (m_SimulationSystem.frameIndex >> 4) & 0xF;
		uint nextUpdateFrameIndex = (num + 4) & 0xF;
		NativeQueue<VehicleDispatch> vehicleDispatches = default(NativeQueue<VehicleDispatch>);
		vehicleDispatches._002Ector(AllocatorHandle.op_Implicit((Allocator)4));
		PrisonerTransportDispatchJob prisonerTransportDispatchJob = new PrisonerTransportDispatchJob
		{
			m_UpdateFrameIndex = num,
			m_NextUpdateFrameIndex = nextUpdateFrameIndex,
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrisonerTransportRequestType = InternalCompilerInterface.GetComponentTypeHandle<PrisonerTransportRequest>(ref __TypeHandle.__Game_Simulation_PrisonerTransportRequest_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DispatchedType = InternalCompilerInterface.GetComponentTypeHandle<Dispatched>(ref __TypeHandle.__Game_Simulation_Dispatched_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathInformationType = InternalCompilerInterface.GetComponentTypeHandle<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceRequestType = InternalCompilerInterface.GetComponentTypeHandle<ServiceRequest>(ref __TypeHandle.__Game_Simulation_ServiceRequest_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrisonerTransportRequestData = InternalCompilerInterface.GetComponentLookup<PrisonerTransportRequest>(ref __TypeHandle.__Game_Simulation_PrisonerTransportRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedCarData = InternalCompilerInterface.GetComponentLookup<ParkedCar>(ref __TypeHandle.__Game_Vehicles_ParkedCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentDistrictData = InternalCompilerInterface.GetComponentLookup<CurrentDistrict>(ref __TypeHandle.__Game_Areas_CurrentDistrict_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceDispatches = InternalCompilerInterface.GetBufferLookup<ServiceDispatch>(ref __TypeHandle.__Game_Simulation_ServiceDispatch_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PoliceStationData = InternalCompilerInterface.GetComponentLookup<PoliceStation>(ref __TypeHandle.__Game_Buildings_PoliceStation_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrisonData = InternalCompilerInterface.GetComponentLookup<Prison>(ref __TypeHandle.__Game_Buildings_Prison_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PublicTransportData = InternalCompilerInterface.GetComponentLookup<PublicTransport>(ref __TypeHandle.__Game_Vehicles_PublicTransport_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		prisonerTransportDispatchJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		prisonerTransportDispatchJob.m_VehicleDispatches = vehicleDispatches.AsParallelWriter();
		prisonerTransportDispatchJob.m_PathfindQueue = m_PathfindSetupSystem.GetQueue(this, 64).AsParallelWriter();
		PrisonerTransportDispatchJob prisonerTransportDispatchJob2 = prisonerTransportDispatchJob;
		DispatchVehiclesJob obj = new DispatchVehiclesJob
		{
			m_VehicleDispatches = vehicleDispatches,
			m_ServiceRequestData = InternalCompilerInterface.GetComponentLookup<ServiceRequest>(ref __TypeHandle.__Game_Simulation_ServiceRequest_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceDispatches = InternalCompilerInterface.GetBufferLookup<ServiceDispatch>(ref __TypeHandle.__Game_Simulation_ServiceDispatch_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<PrisonerTransportDispatchJob>(prisonerTransportDispatchJob2, m_RequestQuery, ((SystemBase)this).Dependency);
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
	public PrisonerTransportDispatchSystem()
	{
	}
}
