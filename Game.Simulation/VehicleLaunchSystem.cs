using System.Runtime.CompilerServices;
using Game.Common;
using Game.Events;
using Game.Net;
using Game.Pathfind;
using Game.Prefabs;
using Game.Tools;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class VehicleLaunchSystem : GameSystemBase
{
	[BurstCompile]
	private struct VehicleLaunchJob : IJobChunk
	{
		[ReadOnly]
		public uint m_SimulationFrame;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<SetupQueueItem> m_PathfindQueue;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Duration> m_DurationType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<TargetElement> m_TargetElementType;

		public ComponentTypeHandle<Game.Events.VehicleLaunch> m_VehicleLaunchType;

		[ReadOnly]
		public ComponentLookup<SpectatorSite> m_SpectatorSiteData;

		[ReadOnly]
		public ComponentLookup<PathInformation> m_PathInformationData;

		[ReadOnly]
		public ComponentLookup<Produced> m_ProducedData;

		[ReadOnly]
		public ComponentLookup<VehicleLaunchData> m_VehicleLaunchData;

		[ReadOnly]
		public BufferLookup<OwnedVehicle> m_OwnedVehicles;

		[ReadOnly]
		public BufferLookup<PathElement> m_PathElements;

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
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Duration> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Duration>(ref m_DurationType);
			NativeArray<Game.Events.VehicleLaunch> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Events.VehicleLaunch>(ref m_VehicleLaunchType);
			NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<TargetElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TargetElement>(ref m_TargetElementType);
			for (int i = 0; i < nativeArray3.Length; i++)
			{
				Entity eventEntity = nativeArray[i];
				Game.Events.VehicleLaunch vehicleLaunch = nativeArray3[i];
				PrefabRef prefabRef = nativeArray4[i];
				if ((vehicleLaunch.m_Flags & VehicleLaunchFlags.PathRequested) == 0)
				{
					if (nativeArray2.Length != 0 && nativeArray2[i].m_StartFrame > m_SimulationFrame)
					{
						continue;
					}
					Entity siteEntity = FindSpectatorSite(bufferAccessor[i], eventEntity);
					Entity vehicleEntity = FindProducedVehicle(siteEntity);
					VehicleLaunchData vehicleLaunchData = m_VehicleLaunchData[prefabRef.m_Prefab];
					FindPath(unfilteredChunkIndex, eventEntity, vehicleEntity, vehicleLaunchData);
					vehicleLaunch.m_Flags |= VehicleLaunchFlags.PathRequested;
				}
				else if ((vehicleLaunch.m_Flags & VehicleLaunchFlags.Launched) == 0)
				{
					Entity siteEntity2 = FindSpectatorSite(bufferAccessor[i], eventEntity);
					Entity vehicleEntity2 = FindProducedVehicle(siteEntity2);
					LaunchVehicle(unfilteredChunkIndex, eventEntity, vehicleEntity2);
					vehicleLaunch.m_Flags |= VehicleLaunchFlags.Launched;
				}
				nativeArray3[i] = vehicleLaunch;
			}
		}

		private Entity FindSpectatorSite(DynamicBuffer<TargetElement> targetElements, Entity eventEntity)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < targetElements.Length; i++)
			{
				Entity entity = targetElements[i].m_Entity;
				if (m_SpectatorSiteData.HasComponent(entity) && m_SpectatorSiteData[entity].m_Event == eventEntity)
				{
					return entity;
				}
			}
			return Entity.Null;
		}

		private Entity FindProducedVehicle(Entity siteEntity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			if (m_OwnedVehicles.HasBuffer(siteEntity))
			{
				DynamicBuffer<OwnedVehicle> val = m_OwnedVehicles[siteEntity];
				for (int i = 0; i < val.Length; i++)
				{
					Entity vehicle = val[i].m_Vehicle;
					if (m_ProducedData.HasComponent(vehicle))
					{
						return vehicle;
					}
				}
			}
			return Entity.Null;
		}

		private void FindPath(int jobIndex, Entity eventEntity, Entity vehicleEntity, VehicleLaunchData vehicleLaunchData)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			if (vehicleEntity != Entity.Null)
			{
				PathfindParameters parameters = new PathfindParameters
				{
					m_MaxSpeed = float2.op_Implicit(277.77777f),
					m_WalkSpeed = float2.op_Implicit(5.555556f),
					m_Weights = new PathfindWeights(1f, 1f, 1f, 1f),
					m_IgnoredRules = (RuleFlags.ForbidCombustionEngines | RuleFlags.ForbidTransitTraffic | RuleFlags.ForbidHeavyTraffic | RuleFlags.ForbidPrivateTraffic | RuleFlags.ForbidSlowTraffic)
				};
				SetupQueueTarget origin = new SetupQueueTarget
				{
					m_Type = SetupTargetType.CurrentLocation,
					m_Entity = vehicleEntity
				};
				SetupQueueTarget destination = new SetupQueueTarget
				{
					m_Type = SetupTargetType.OutsideConnection,
					m_Value2 = 1000f
				};
				if (vehicleLaunchData.m_TransportType == TransportType.Rocket)
				{
					parameters.m_Methods = PathMethod.Road | PathMethod.Flying;
					origin.m_Methods = PathMethod.Road;
					origin.m_RoadTypes = RoadTypes.Helicopter;
					destination.m_Methods = PathMethod.Flying;
					destination.m_FlyingTypes = RoadTypes.Helicopter;
					m_PathfindQueue.Enqueue(new SetupQueueItem(eventEntity, parameters, origin, destination));
				}
			}
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathInformation>(jobIndex, eventEntity, default(PathInformation));
			((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<PathElement>(jobIndex, eventEntity);
		}

		private void LaunchVehicle(int jobIndex, Entity eventEntity, Entity vehicleEntity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			if (vehicleEntity != Entity.Null)
			{
				PathInformation pathInformation = m_PathInformationData[eventEntity];
				DynamicBuffer<PathElement> sourceElements = m_PathElements[eventEntity];
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Produced>(jobIndex, vehicleEntity);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Target>(jobIndex, vehicleEntity, new Target(pathInformation.m_Destination));
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PathOwner>(jobIndex, vehicleEntity, new PathOwner(PathFlags.Updated));
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Game.Vehicles.PublicTransport>(jobIndex, vehicleEntity, new Game.Vehicles.PublicTransport
				{
					m_State = PublicTransportFlags.Launched
				});
				DynamicBuffer<PathElement> targetElements = ((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<PathElement>(jobIndex, vehicleEntity);
				PathUtils.CopyPath(sourceElements, default(PathOwner), 0, targetElements);
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
		public ComponentTypeHandle<Duration> __Game_Events_Duration_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<TargetElement> __Game_Events_TargetElement_RO_BufferTypeHandle;

		public ComponentTypeHandle<Game.Events.VehicleLaunch> __Game_Events_VehicleLaunch_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<SpectatorSite> __Game_Events_SpectatorSite_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Produced> __Game_Vehicles_Produced_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<VehicleLaunchData> __Game_Prefabs_VehicleLaunchData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<OwnedVehicle> __Game_Vehicles_OwnedVehicle_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<PathElement> __Game_Pathfind_PathElement_RO_BufferLookup;

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
			__Game_Events_Duration_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Duration>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Events_TargetElement_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<TargetElement>(true);
			__Game_Events_VehicleLaunch_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Events.VehicleLaunch>(false);
			__Game_Events_SpectatorSite_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpectatorSite>(true);
			__Game_Pathfind_PathInformation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathInformation>(true);
			__Game_Vehicles_Produced_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Produced>(true);
			__Game_Prefabs_VehicleLaunchData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<VehicleLaunchData>(true);
			__Game_Vehicles_OwnedVehicle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<OwnedVehicle>(true);
			__Game_Pathfind_PathElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PathElement>(true);
		}
	}

	private const uint UPDATE_INTERVAL = 64u;

	private SimulationSystem m_SimulationSystem;

	private PathfindSetupSystem m_PathfindSetupSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_LaunchQuery;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 64;
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
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_PathfindSetupSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindSetupSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_LaunchQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<Game.Events.VehicleLaunch>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_LaunchQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		VehicleLaunchJob vehicleLaunchJob = new VehicleLaunchJob
		{
			m_SimulationFrame = m_SimulationSystem.frameIndex
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		vehicleLaunchJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		vehicleLaunchJob.m_PathfindQueue = m_PathfindSetupSystem.GetQueue(this, 64).AsParallelWriter();
		vehicleLaunchJob.m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
		vehicleLaunchJob.m_DurationType = InternalCompilerInterface.GetComponentTypeHandle<Duration>(ref __TypeHandle.__Game_Events_Duration_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		vehicleLaunchJob.m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		vehicleLaunchJob.m_TargetElementType = InternalCompilerInterface.GetBufferTypeHandle<TargetElement>(ref __TypeHandle.__Game_Events_TargetElement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		vehicleLaunchJob.m_VehicleLaunchType = InternalCompilerInterface.GetComponentTypeHandle<Game.Events.VehicleLaunch>(ref __TypeHandle.__Game_Events_VehicleLaunch_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		vehicleLaunchJob.m_SpectatorSiteData = InternalCompilerInterface.GetComponentLookup<SpectatorSite>(ref __TypeHandle.__Game_Events_SpectatorSite_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		vehicleLaunchJob.m_PathInformationData = InternalCompilerInterface.GetComponentLookup<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		vehicleLaunchJob.m_ProducedData = InternalCompilerInterface.GetComponentLookup<Produced>(ref __TypeHandle.__Game_Vehicles_Produced_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		vehicleLaunchJob.m_VehicleLaunchData = InternalCompilerInterface.GetComponentLookup<VehicleLaunchData>(ref __TypeHandle.__Game_Prefabs_VehicleLaunchData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		vehicleLaunchJob.m_OwnedVehicles = InternalCompilerInterface.GetBufferLookup<OwnedVehicle>(ref __TypeHandle.__Game_Vehicles_OwnedVehicle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		vehicleLaunchJob.m_PathElements = InternalCompilerInterface.GetBufferLookup<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		VehicleLaunchJob vehicleLaunchJob2 = vehicleLaunchJob;
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<VehicleLaunchJob>(vehicleLaunchJob2, m_LaunchQuery, ((SystemBase)this).Dependency);
		m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
		m_PathfindSetupSystem.AddQueueWriter(((SystemBase)this).Dependency);
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
	public VehicleLaunchSystem()
	{
	}
}
