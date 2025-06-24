using System.Runtime.CompilerServices;
using Game.Buildings;
using Game.Citizens;
using Game.Common;
using Game.Economy;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
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
public class GarbageTransferDispatchSystem : GameSystemBase
{
	private struct DispatchAction
	{
		public Entity m_Request;

		public Entity m_DispatchSource;

		public Entity m_DeliverFacility;

		public Entity m_ReceiveFacility;

		public DispatchAction(Entity request, Entity dispatchSource, Entity deliverFacility, Entity receiveFacility)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			m_Request = request;
			m_DispatchSource = dispatchSource;
			m_DeliverFacility = deliverFacility;
			m_ReceiveFacility = receiveFacility;
		}
	}

	[BurstCompile]
	private struct GarbageTransferDispatchJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<GarbageTransferRequest> m_GarbageTransferRequestType;

		[ReadOnly]
		public ComponentTypeHandle<Dispatched> m_DispatchedType;

		[ReadOnly]
		public ComponentTypeHandle<PathInformation> m_PathInformationType;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		public ComponentTypeHandle<ServiceRequest> m_ServiceRequestType;

		[ReadOnly]
		public ComponentLookup<GarbageTransferRequest> m_GarbageTransferRequestData;

		[ReadOnly]
		public ComponentLookup<GarbageFacility> m_GarbageFacilityData;

		[ReadOnly]
		public BufferLookup<ServiceDispatch> m_ServiceDispatches;

		[ReadOnly]
		public BufferLookup<TripNeeded> m_TripNeededs;

		[ReadOnly]
		public uint m_UpdateFrameIndex;

		[ReadOnly]
		public uint m_NextUpdateFrameIndex;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<DispatchAction> m_DispatchActions;

		public ParallelWriter<SetupQueueItem> m_PathfindQueue;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			uint index = ((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index;
			if (index == m_NextUpdateFrameIndex && !((ArchetypeChunk)(ref chunk)).Has<Dispatched>(ref m_DispatchedType) && !((ArchetypeChunk)(ref chunk)).Has<PathInformation>(ref m_PathInformationType))
			{
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				NativeArray<GarbageTransferRequest> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<GarbageTransferRequest>(ref m_GarbageTransferRequestType);
				NativeArray<ServiceRequest> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ServiceRequest>(ref m_ServiceRequestType);
				for (int i = 0; i < nativeArray2.Length; i++)
				{
					Entity val = nativeArray[i];
					GarbageTransferRequest requestData = nativeArray2[i];
					ServiceRequest serviceRequest = nativeArray3[i];
					if (!ValidateTarget(val, requestData))
					{
						((ParallelWriter)(ref m_CommandBuffer)).DestroyEntity(unfilteredChunkIndex, val);
						continue;
					}
					if (SimulationUtils.TickServiceRequest(ref serviceRequest))
					{
						FindVehicleSource(unfilteredChunkIndex, val, requestData);
					}
					nativeArray3[i] = serviceRequest;
				}
			}
			if (index != m_UpdateFrameIndex)
			{
				return;
			}
			NativeArray<Dispatched> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Dispatched>(ref m_DispatchedType);
			NativeArray<GarbageTransferRequest> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<GarbageTransferRequest>(ref m_GarbageTransferRequestType);
			NativeArray<ServiceRequest> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ServiceRequest>(ref m_ServiceRequestType);
			if (nativeArray4.Length != 0)
			{
				NativeArray<Entity> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				for (int j = 0; j < nativeArray4.Length; j++)
				{
					Entity val2 = nativeArray7[j];
					Dispatched dispatched = nativeArray4[j];
					GarbageTransferRequest requestData2 = nativeArray5[j];
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
						if (!ValidateTarget(val2, requestData2))
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
				GarbageTransferRequest requestData3 = nativeArray5[k];
				PathInformation pathInformation = nativeArray8[k];
				ServiceRequest serviceRequest3 = nativeArray6[k];
				if (!ValidateTarget(val3, requestData3))
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
				nativeArray6[k] = serviceRequest3;
			}
		}

		private bool ValidateHandler(Entity entity, Entity handler)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			if (m_ServiceDispatches.HasBuffer(handler))
			{
				DynamicBuffer<ServiceDispatch> val = m_ServiceDispatches[handler];
				for (int i = 0; i < val.Length; i++)
				{
					if (val[i].m_Request == entity)
					{
						return true;
					}
				}
			}
			if (m_TripNeededs.HasBuffer(handler))
			{
				DynamicBuffer<TripNeeded> val2 = m_TripNeededs[handler];
				for (int j = 0; j < val2.Length; j++)
				{
					if (val2[j].m_TargetAgent == entity)
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool ValidateTarget(Entity entity, GarbageTransferRequest requestData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			if (!m_GarbageFacilityData.HasComponent(requestData.m_Facility))
			{
				return false;
			}
			GarbageFacility garbageFacility = m_GarbageFacilityData[requestData.m_Facility];
			if ((requestData.m_Flags & GarbageTransferRequestFlags.Deliver) != 0)
			{
				if (garbageFacility.m_AcceptGarbagePriority <= 0f)
				{
					return false;
				}
				if (garbageFacility.m_GarbageDeliverRequest != entity)
				{
					if (m_GarbageTransferRequestData.HasComponent(garbageFacility.m_GarbageDeliverRequest))
					{
						return false;
					}
					m_DispatchActions.Enqueue(new DispatchAction(entity, Entity.Null, requestData.m_Facility, Entity.Null));
				}
				return true;
			}
			if ((requestData.m_Flags & GarbageTransferRequestFlags.Receive) != 0)
			{
				if (garbageFacility.m_DeliverGarbagePriority <= 0f)
				{
					return false;
				}
				if (garbageFacility.m_GarbageReceiveRequest != entity)
				{
					if (m_GarbageTransferRequestData.HasComponent(garbageFacility.m_GarbageReceiveRequest))
					{
						return false;
					}
					m_DispatchActions.Enqueue(new DispatchAction(entity, Entity.Null, Entity.Null, requestData.m_Facility));
				}
				return true;
			}
			return false;
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
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			m_DispatchActions.Enqueue(new DispatchAction(entity, pathInformation.m_Origin, Entity.Null, Entity.Null));
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Dispatched>(jobIndex, entity, new Dispatched(pathInformation.m_Origin));
		}

		private void FindVehicleSource(int jobIndex, Entity requestEntity, GarbageTransferRequest requestData)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			PathfindParameters parameters = new PathfindParameters
			{
				m_MaxSpeed = float2.op_Implicit(111.111115f),
				m_WalkSpeed = float2.op_Implicit(5.555556f),
				m_Weights = new PathfindWeights(1f, 1f, 1f, 1f),
				m_Methods = (PathMethod.Road | PathMethod.CargoLoading),
				m_IgnoredRules = RuleFlags.ForbidSlowTraffic
			};
			SetupQueueTarget a = new SetupQueueTarget
			{
				m_Type = SetupTargetType.GarbageTransfer,
				m_Methods = (PathMethod.Road | PathMethod.CargoLoading),
				m_RoadTypes = RoadTypes.Car,
				m_Value = requestData.m_Amount,
				m_Value2 = requestData.m_Priority,
				m_Resource = Resource.Garbage,
				m_RandomCost = 30f
			};
			SetupQueueTarget b = new SetupQueueTarget
			{
				m_Type = SetupTargetType.CurrentLocation,
				m_Methods = (PathMethod.Road | PathMethod.CargoLoading),
				m_RoadTypes = RoadTypes.Car,
				m_Entity = requestData.m_Facility,
				m_RandomCost = 30f
			};
			if ((requestData.m_Flags & GarbageTransferRequestFlags.Receive) != 0)
			{
				a.m_Flags |= SetupTargetFlags.Import;
			}
			if ((requestData.m_Flags & GarbageTransferRequestFlags.Deliver) != 0)
			{
				a.m_Flags |= SetupTargetFlags.Export;
			}
			if ((requestData.m_Flags & GarbageTransferRequestFlags.RequireTransport) != 0)
			{
				a.m_Flags |= SetupTargetFlags.RequireTransport;
			}
			else
			{
				CommonUtils.Swap(ref a, ref b);
			}
			SetupQueueItem setupQueueItem = new SetupQueueItem(requestEntity, parameters, a, b);
			m_PathfindQueue.Enqueue(setupQueueItem);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathInformation>(jobIndex, requestEntity, default(PathInformation));
			((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<PathElement>(jobIndex, requestEntity);
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct DispatchActionJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<GarbageTransferRequest> m_RequestData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> m_OutsideConnectionData;

		public ComponentLookup<GarbageFacility> m_GarbageFacilityData;

		public BufferLookup<ServiceDispatch> m_ServiceDispatches;

		public BufferLookup<TripNeeded> m_TripNeededs;

		public BufferLookup<Resources> m_Resources;

		public NativeQueue<DispatchAction> m_DispatchActions;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			DispatchAction dispatchAction = default(DispatchAction);
			while (m_DispatchActions.TryDequeue(ref dispatchAction))
			{
				if (m_GarbageFacilityData.HasComponent(dispatchAction.m_DispatchSource) && !m_OutsideConnectionData.HasComponent(dispatchAction.m_DispatchSource))
				{
					if (m_ServiceDispatches.HasBuffer(dispatchAction.m_DispatchSource))
					{
						m_ServiceDispatches[dispatchAction.m_DispatchSource].Add(new ServiceDispatch(dispatchAction.m_Request));
					}
				}
				else if (m_TripNeededs.HasBuffer(dispatchAction.m_DispatchSource))
				{
					GarbageTransferRequest garbageTransferRequest = m_RequestData[dispatchAction.m_Request];
					DynamicBuffer<TripNeeded> val = m_TripNeededs[dispatchAction.m_DispatchSource];
					TripNeeded tripNeeded = new TripNeeded
					{
						m_TargetAgent = dispatchAction.m_Request,
						m_Resource = Resource.Garbage
					};
					if ((garbageTransferRequest.m_Flags & GarbageTransferRequestFlags.RequireTransport) != 0)
					{
						if ((garbageTransferRequest.m_Flags & GarbageTransferRequestFlags.Deliver) != 0)
						{
							tripNeeded.m_Purpose = Purpose.Exporting;
						}
						if ((garbageTransferRequest.m_Flags & GarbageTransferRequestFlags.Receive) != 0)
						{
							tripNeeded.m_Purpose = Purpose.Collect;
						}
					}
					else
					{
						if ((garbageTransferRequest.m_Flags & GarbageTransferRequestFlags.Deliver) != 0)
						{
							tripNeeded.m_Purpose = Purpose.Collect;
						}
						if ((garbageTransferRequest.m_Flags & GarbageTransferRequestFlags.Receive) != 0)
						{
							tripNeeded.m_Purpose = Purpose.Exporting;
						}
					}
					tripNeeded.m_Data = garbageTransferRequest.m_Amount;
					if (tripNeeded.m_Purpose == Purpose.Exporting)
					{
						DynamicBuffer<Resources> resources = m_Resources[dispatchAction.m_DispatchSource];
						int resources2 = EconomyUtils.GetResources(tripNeeded.m_Resource, resources);
						tripNeeded.m_Data = math.min(tripNeeded.m_Data, resources2);
						if (tripNeeded.m_Data <= 0)
						{
							continue;
						}
						EconomyUtils.AddResources(tripNeeded.m_Resource, -tripNeeded.m_Data, resources);
					}
					else
					{
						tripNeeded.m_Purpose = Purpose.ReturnGarbage;
						tripNeeded.m_Resource = Resource.NoResource;
					}
					val.Add(tripNeeded);
				}
				if (dispatchAction.m_DeliverFacility != Entity.Null)
				{
					GarbageFacility garbageFacility = m_GarbageFacilityData[dispatchAction.m_DeliverFacility];
					garbageFacility.m_GarbageDeliverRequest = dispatchAction.m_Request;
					m_GarbageFacilityData[dispatchAction.m_DeliverFacility] = garbageFacility;
				}
				if (dispatchAction.m_ReceiveFacility != Entity.Null)
				{
					GarbageFacility garbageFacility2 = m_GarbageFacilityData[dispatchAction.m_ReceiveFacility];
					garbageFacility2.m_GarbageReceiveRequest = dispatchAction.m_Request;
					m_GarbageFacilityData[dispatchAction.m_ReceiveFacility] = garbageFacility2;
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<GarbageTransferRequest> __Game_Simulation_GarbageTransferRequest_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Dispatched> __Game_Simulation_Dispatched_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentTypeHandle;

		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		public ComponentTypeHandle<ServiceRequest> __Game_Simulation_ServiceRequest_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<GarbageTransferRequest> __Game_Simulation_GarbageTransferRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<GarbageFacility> __Game_Buildings_GarbageFacility_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ServiceDispatch> __Game_Simulation_ServiceDispatch_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<TripNeeded> __Game_Citizens_TripNeeded_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> __Game_Objects_OutsideConnection_RO_ComponentLookup;

		public ComponentLookup<GarbageFacility> __Game_Buildings_GarbageFacility_RW_ComponentLookup;

		public BufferLookup<ServiceDispatch> __Game_Simulation_ServiceDispatch_RW_BufferLookup;

		public BufferLookup<TripNeeded> __Game_Citizens_TripNeeded_RW_BufferLookup;

		public BufferLookup<Resources> __Game_Economy_Resources_RW_BufferLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Simulation_GarbageTransferRequest_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<GarbageTransferRequest>(true);
			__Game_Simulation_Dispatched_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Dispatched>(true);
			__Game_Pathfind_PathInformation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathInformation>(true);
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Simulation_ServiceRequest_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ServiceRequest>(false);
			__Game_Simulation_GarbageTransferRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarbageTransferRequest>(true);
			__Game_Buildings_GarbageFacility_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarbageFacility>(true);
			__Game_Simulation_ServiceDispatch_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceDispatch>(true);
			__Game_Citizens_TripNeeded_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TripNeeded>(true);
			__Game_Objects_OutsideConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.OutsideConnection>(true);
			__Game_Buildings_GarbageFacility_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarbageFacility>(false);
			__Game_Simulation_ServiceDispatch_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceDispatch>(false);
			__Game_Citizens_TripNeeded_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TripNeeded>(false);
			__Game_Economy_Resources_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Resources>(false);
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
			ComponentType.ReadOnly<GarbageTransferRequest>(),
			ComponentType.ReadOnly<UpdateFrame>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_RequestQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		uint num = (m_SimulationSystem.frameIndex >> 4) & 7;
		uint nextUpdateFrameIndex = (num + 4) & 7;
		NativeQueue<DispatchAction> dispatchActions = default(NativeQueue<DispatchAction>);
		dispatchActions._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		GarbageTransferDispatchJob garbageTransferDispatchJob = new GarbageTransferDispatchJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageTransferRequestType = InternalCompilerInterface.GetComponentTypeHandle<GarbageTransferRequest>(ref __TypeHandle.__Game_Simulation_GarbageTransferRequest_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DispatchedType = InternalCompilerInterface.GetComponentTypeHandle<Dispatched>(ref __TypeHandle.__Game_Simulation_Dispatched_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathInformationType = InternalCompilerInterface.GetComponentTypeHandle<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceRequestType = InternalCompilerInterface.GetComponentTypeHandle<ServiceRequest>(ref __TypeHandle.__Game_Simulation_ServiceRequest_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageTransferRequestData = InternalCompilerInterface.GetComponentLookup<GarbageTransferRequest>(ref __TypeHandle.__Game_Simulation_GarbageTransferRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageFacilityData = InternalCompilerInterface.GetComponentLookup<GarbageFacility>(ref __TypeHandle.__Game_Buildings_GarbageFacility_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceDispatches = InternalCompilerInterface.GetBufferLookup<ServiceDispatch>(ref __TypeHandle.__Game_Simulation_ServiceDispatch_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TripNeededs = InternalCompilerInterface.GetBufferLookup<TripNeeded>(ref __TypeHandle.__Game_Citizens_TripNeeded_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameIndex = num,
			m_NextUpdateFrameIndex = nextUpdateFrameIndex
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		garbageTransferDispatchJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		garbageTransferDispatchJob.m_DispatchActions = dispatchActions.AsParallelWriter();
		garbageTransferDispatchJob.m_PathfindQueue = m_PathfindSetupSystem.GetQueue(this, 64).AsParallelWriter();
		GarbageTransferDispatchJob garbageTransferDispatchJob2 = garbageTransferDispatchJob;
		DispatchActionJob obj = new DispatchActionJob
		{
			m_RequestData = InternalCompilerInterface.GetComponentLookup<GarbageTransferRequest>(ref __TypeHandle.__Game_Simulation_GarbageTransferRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OutsideConnectionData = InternalCompilerInterface.GetComponentLookup<Game.Objects.OutsideConnection>(ref __TypeHandle.__Game_Objects_OutsideConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageFacilityData = InternalCompilerInterface.GetComponentLookup<GarbageFacility>(ref __TypeHandle.__Game_Buildings_GarbageFacility_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceDispatches = InternalCompilerInterface.GetBufferLookup<ServiceDispatch>(ref __TypeHandle.__Game_Simulation_ServiceDispatch_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TripNeededs = InternalCompilerInterface.GetBufferLookup<TripNeeded>(ref __TypeHandle.__Game_Citizens_TripNeeded_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Resources = InternalCompilerInterface.GetBufferLookup<Resources>(ref __TypeHandle.__Game_Economy_Resources_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DispatchActions = dispatchActions,
			m_CommandBuffer = m_EndFrameBarrier.CreateCommandBuffer()
		};
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<GarbageTransferDispatchJob>(garbageTransferDispatchJob2, m_RequestQuery, ((SystemBase)this).Dependency);
		JobHandle val3 = IJobExtensions.Schedule<DispatchActionJob>(obj, val2);
		dispatchActions.Dispose(val3);
		m_PathfindSetupSystem.AddQueueWriter(val2);
		m_EndFrameBarrier.AddJobHandleForProducer(val3);
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
	public GarbageTransferDispatchSystem()
	{
	}
}
