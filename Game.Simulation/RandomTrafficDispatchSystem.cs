using System.Runtime.CompilerServices;
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
public class RandomTrafficDispatchSystem : GameSystemBase
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
	private struct RandomTrafficDispatchJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<RandomTrafficRequest> m_RandomTrafficRequestType;

		[ReadOnly]
		public ComponentTypeHandle<Dispatched> m_DispatchedType;

		[ReadOnly]
		public ComponentTypeHandle<PathInformation> m_PathInformationType;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		public ComponentTypeHandle<ServiceRequest> m_ServiceRequestType;

		[ReadOnly]
		public ComponentLookup<RandomTrafficRequest> m_RandomTrafficRequestData;

		[ReadOnly]
		public BufferLookup<ServiceDispatch> m_ServiceDispatches;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<TrafficSpawner> m_TrafficSpawnerData;

		[ReadOnly]
		public uint m_UpdateFrameIndex;

		[ReadOnly]
		public uint m_NextUpdateFrameIndex;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<VehicleDispatch> m_VehicleDispatches;

		public ParallelWriter<SetupQueueItem> m_PathfindQueue;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			uint index = ((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index;
			if (index == m_NextUpdateFrameIndex && !((ArchetypeChunk)(ref chunk)).Has<Dispatched>(ref m_DispatchedType) && !((ArchetypeChunk)(ref chunk)).Has<PathInformation>(ref m_PathInformationType))
			{
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				NativeArray<RandomTrafficRequest> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<RandomTrafficRequest>(ref m_RandomTrafficRequestType);
				NativeArray<ServiceRequest> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ServiceRequest>(ref m_ServiceRequestType);
				Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
				for (int i = 0; i < nativeArray2.Length; i++)
				{
					Entity val = nativeArray[i];
					RandomTrafficRequest trafficRequest = nativeArray2[i];
					ServiceRequest serviceRequest = nativeArray3[i];
					if (!ValidateTarget(val, trafficRequest.m_Target))
					{
						((ParallelWriter)(ref m_CommandBuffer)).DestroyEntity(unfilteredChunkIndex, val);
						continue;
					}
					if (SimulationUtils.TickServiceRequest(ref serviceRequest))
					{
						FindVehicleSource(unfilteredChunkIndex, ref random, val, trafficRequest);
					}
					nativeArray3[i] = serviceRequest;
				}
			}
			if (index != m_UpdateFrameIndex)
			{
				return;
			}
			NativeArray<Dispatched> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Dispatched>(ref m_DispatchedType);
			NativeArray<RandomTrafficRequest> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<RandomTrafficRequest>(ref m_RandomTrafficRequestType);
			NativeArray<ServiceRequest> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ServiceRequest>(ref m_ServiceRequestType);
			if (nativeArray4.Length != 0)
			{
				NativeArray<Entity> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				for (int j = 0; j < nativeArray4.Length; j++)
				{
					Entity val2 = nativeArray7[j];
					Dispatched dispatched = nativeArray4[j];
					RandomTrafficRequest randomTrafficRequest = nativeArray5[j];
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
						if (!ValidateTarget(val2, randomTrafficRequest.m_Target))
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
				RandomTrafficRequest randomTrafficRequest2 = nativeArray5[k];
				PathInformation pathInformation = nativeArray8[k];
				ServiceRequest serviceRequest3 = nativeArray6[k];
				if (!ValidateTarget(val3, randomTrafficRequest2.m_Target))
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
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			TrafficSpawner trafficSpawner = default(TrafficSpawner);
			if (!m_TrafficSpawnerData.TryGetComponent(target, ref trafficSpawner))
			{
				return false;
			}
			if (trafficSpawner.m_TrafficRequest != entity)
			{
				if (m_RandomTrafficRequestData.HasComponent(trafficSpawner.m_TrafficRequest))
				{
					return false;
				}
				trafficSpawner.m_TrafficRequest = entity;
				m_TrafficSpawnerData[target] = trafficSpawner;
			}
			return true;
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
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			VehicleDispatch vehicleDispatch = new VehicleDispatch(entity, pathInformation.m_Origin);
			m_VehicleDispatches.Enqueue(vehicleDispatch);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Dispatched>(jobIndex, entity, new Dispatched(pathInformation.m_Origin));
		}

		private void FindVehicleSource(int jobIndex, ref Random random, Entity requestEntity, RandomTrafficRequest trafficRequest)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			PathfindParameters parameters = new PathfindParameters
			{
				m_MaxSpeed = float2.op_Implicit(111.111115f),
				m_WalkSpeed = float2.op_Implicit(5.555556f),
				m_Weights = new PathfindWeights(1f, 1f, 1f, 1f),
				m_IgnoredRules = RuleFlags.ForbidSlowTraffic
			};
			SetupQueueTarget a = new SetupQueueTarget
			{
				m_Type = SetupTargetType.RandomTraffic,
				m_RoadTypes = trafficRequest.m_RoadType,
				m_TrackTypes = trafficRequest.m_TrackType,
				m_Entity = trafficRequest.m_Target
			};
			SetupQueueTarget b = new SetupQueueTarget
			{
				m_Type = SetupTargetType.CurrentLocation,
				m_RoadTypes = trafficRequest.m_RoadType,
				m_TrackTypes = trafficRequest.m_TrackType,
				m_Entity = trafficRequest.m_Target
			};
			if ((trafficRequest.m_RoadType & RoadTypes.Car) != RoadTypes.None)
			{
				parameters.m_MaxSpeed = math.max(parameters.m_MaxSpeed, float2.op_Implicit(111.111115f));
				parameters.m_Methods |= PathMethod.Road;
				a.m_Methods |= PathMethod.Road;
				b.m_Methods |= PathMethod.Road;
				if ((trafficRequest.m_Flags & RandomTrafficRequestFlags.DeliveryTruck) != 0)
				{
					parameters.m_Methods |= PathMethod.CargoLoading;
					a.m_Methods |= PathMethod.CargoLoading;
					b.m_Methods |= PathMethod.CargoLoading;
				}
				if ((int)trafficRequest.m_SizeClass <= 1)
				{
					parameters.m_Methods |= PathMethod.MediumRoad;
					a.m_Methods |= PathMethod.MediumRoad;
					b.m_Methods |= PathMethod.MediumRoad;
				}
			}
			if ((trafficRequest.m_RoadType & RoadTypes.Airplane) != RoadTypes.None)
			{
				parameters.m_MaxSpeed = math.max(parameters.m_MaxSpeed, float2.op_Implicit(277.77777f));
				parameters.m_Methods |= PathMethod.Road | PathMethod.Flying;
				a.m_Methods |= PathMethod.Road;
				b.m_Methods |= PathMethod.Road;
				if ((int)trafficRequest.m_SizeClass <= 1)
				{
					parameters.m_Methods |= PathMethod.MediumRoad;
					a.m_Methods |= PathMethod.MediumRoad;
					b.m_Methods |= PathMethod.MediumRoad;
				}
			}
			if ((trafficRequest.m_RoadType & RoadTypes.Watercraft) != RoadTypes.None)
			{
				parameters.m_MaxSpeed = math.max(parameters.m_MaxSpeed, float2.op_Implicit(55.555557f));
				parameters.m_Methods |= PathMethod.Road;
				a.m_Methods |= PathMethod.Road;
				b.m_Methods |= PathMethod.Road;
				if ((int)trafficRequest.m_SizeClass <= 1)
				{
					parameters.m_Methods |= PathMethod.MediumRoad;
					a.m_Methods |= PathMethod.MediumRoad;
					b.m_Methods |= PathMethod.MediumRoad;
				}
			}
			if ((trafficRequest.m_TrackType & TrackTypes.Train) != TrackTypes.None)
			{
				parameters.m_MaxSpeed = math.max(parameters.m_MaxSpeed, float2.op_Implicit(138.88889f));
				parameters.m_Methods |= PathMethod.Track;
				a.m_Methods |= PathMethod.Track;
				b.m_Methods |= PathMethod.Track;
			}
			if ((int)trafficRequest.m_SizeClass < 2)
			{
				parameters.m_IgnoredRules |= RuleFlags.ForbidHeavyTraffic;
			}
			if (trafficRequest.m_EnergyTypes == EnergyTypes.Electricity)
			{
				parameters.m_IgnoredRules |= RuleFlags.ForbidCombustionEngines;
			}
			if (((Random)(ref random)).NextBool())
			{
				CommonUtils.Swap(ref a, ref b);
			}
			m_PathfindQueue.Enqueue(new SetupQueueItem(requestEntity, parameters, a, b));
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathInformation>(jobIndex, requestEntity, default(PathInformation));
			((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<PathElement>(jobIndex, requestEntity);
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

		public BufferLookup<ServiceDispatch> m_ServiceDispatches;

		public void Execute()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			VehicleDispatch vehicleDispatch = default(VehicleDispatch);
			DynamicBuffer<ServiceDispatch> val = default(DynamicBuffer<ServiceDispatch>);
			while (m_VehicleDispatches.TryDequeue(ref vehicleDispatch))
			{
				if (m_ServiceDispatches.TryGetBuffer(vehicleDispatch.m_Source, ref val))
				{
					val.Add(new ServiceDispatch(vehicleDispatch.m_Request));
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<RandomTrafficRequest> __Game_Simulation_RandomTrafficRequest_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Dispatched> __Game_Simulation_Dispatched_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentTypeHandle;

		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		public ComponentTypeHandle<ServiceRequest> __Game_Simulation_ServiceRequest_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<RandomTrafficRequest> __Game_Simulation_RandomTrafficRequest_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ServiceDispatch> __Game_Simulation_ServiceDispatch_RO_BufferLookup;

		public ComponentLookup<TrafficSpawner> __Game_Buildings_TrafficSpawner_RW_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Simulation_RandomTrafficRequest_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<RandomTrafficRequest>(true);
			__Game_Simulation_Dispatched_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Dispatched>(true);
			__Game_Pathfind_PathInformation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathInformation>(true);
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Simulation_ServiceRequest_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ServiceRequest>(false);
			__Game_Simulation_RandomTrafficRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RandomTrafficRequest>(true);
			__Game_Simulation_ServiceDispatch_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceDispatch>(true);
			__Game_Buildings_TrafficSpawner_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrafficSpawner>(false);
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
		if (phase == SystemUpdatePhase.LoadSimulation)
		{
			return 1;
		}
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
			ComponentType.ReadOnly<RandomTrafficRequest>(),
			ComponentType.ReadOnly<UpdateFrame>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_RequestQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		uint num;
		uint nextUpdateFrameIndex;
		int maxDelayFrames;
		if (m_SimulationSystem.loadingProgress != 1f)
		{
			num = m_SimulationSystem.frameIndex & 0xF;
			nextUpdateFrameIndex = num;
			maxDelayFrames = 16;
		}
		else
		{
			num = (m_SimulationSystem.frameIndex >> 4) & 0xF;
			nextUpdateFrameIndex = (num + 4) & 0xF;
			maxDelayFrames = 64;
		}
		NativeQueue<VehicleDispatch> vehicleDispatches = default(NativeQueue<VehicleDispatch>);
		vehicleDispatches._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		RandomTrafficDispatchJob randomTrafficDispatchJob = new RandomTrafficDispatchJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RandomTrafficRequestType = InternalCompilerInterface.GetComponentTypeHandle<RandomTrafficRequest>(ref __TypeHandle.__Game_Simulation_RandomTrafficRequest_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DispatchedType = InternalCompilerInterface.GetComponentTypeHandle<Dispatched>(ref __TypeHandle.__Game_Simulation_Dispatched_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathInformationType = InternalCompilerInterface.GetComponentTypeHandle<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceRequestType = InternalCompilerInterface.GetComponentTypeHandle<ServiceRequest>(ref __TypeHandle.__Game_Simulation_ServiceRequest_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RandomTrafficRequestData = InternalCompilerInterface.GetComponentLookup<RandomTrafficRequest>(ref __TypeHandle.__Game_Simulation_RandomTrafficRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceDispatches = InternalCompilerInterface.GetBufferLookup<ServiceDispatch>(ref __TypeHandle.__Game_Simulation_ServiceDispatch_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrafficSpawnerData = InternalCompilerInterface.GetComponentLookup<TrafficSpawner>(ref __TypeHandle.__Game_Buildings_TrafficSpawner_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameIndex = num,
			m_NextUpdateFrameIndex = nextUpdateFrameIndex,
			m_RandomSeed = RandomSeed.Next()
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		randomTrafficDispatchJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		randomTrafficDispatchJob.m_VehicleDispatches = vehicleDispatches.AsParallelWriter();
		randomTrafficDispatchJob.m_PathfindQueue = m_PathfindSetupSystem.GetQueue(this, maxDelayFrames).AsParallelWriter();
		RandomTrafficDispatchJob randomTrafficDispatchJob2 = randomTrafficDispatchJob;
		DispatchVehiclesJob obj = new DispatchVehiclesJob
		{
			m_VehicleDispatches = vehicleDispatches,
			m_ServiceDispatches = InternalCompilerInterface.GetBufferLookup<ServiceDispatch>(ref __TypeHandle.__Game_Simulation_ServiceDispatch_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<RandomTrafficDispatchJob>(randomTrafficDispatchJob2, m_RequestQuery, ((SystemBase)this).Dependency);
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
	public RandomTrafficDispatchSystem()
	{
	}
}
