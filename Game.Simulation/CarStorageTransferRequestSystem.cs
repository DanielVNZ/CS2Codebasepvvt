using System.Runtime.CompilerServices;
using Game.Buildings;
using Game.Citizens;
using Game.Common;
using Game.Companies;
using Game.Economy;
using Game.Objects;
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
public class CarStorageTransferRequestSystem : GameSystemBase
{
	[BurstCompile]
	private struct TransferJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public BufferLookup<GuestVehicle> m_GuestVehicles;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_LayoutElements;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.DeliveryTruck> m_DeliveryTrucks;

		public BufferTypeHandle<StorageTransferRequest> m_RequestType;

		public BufferTypeHandle<Resources> m_ResourceType;

		public BufferTypeHandle<TripNeeded> m_TripType;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_Properties;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> m_OutsideConnections;

		[ReadOnly]
		public DeliveryTruckSelectData m_DeliveryTruckSelectData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			BufferAccessor<StorageTransferRequest> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<StorageTransferRequest>(ref m_RequestType);
			BufferAccessor<Resources> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Resources>(ref m_ResourceType);
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			BufferAccessor<TripNeeded> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TripNeeded>(ref m_TripType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity destination = nativeArray[i];
				DynamicBuffer<StorageTransferRequest> val = bufferAccessor[i];
				DynamicBuffer<Resources> resources = bufferAccessor2[i];
				DynamicBuffer<TripNeeded> val2 = bufferAccessor3[i];
				for (int j = 0; j < val.Length; j++)
				{
					StorageTransferRequest storageTransferRequest = val[j];
					int resources2 = EconomyUtils.GetResources(storageTransferRequest.m_Resource, resources);
					int allBuyingResourcesTrucks = VehicleUtils.GetAllBuyingResourcesTrucks(destination, storageTransferRequest.m_Resource, ref m_DeliveryTrucks, ref m_GuestVehicles, ref m_LayoutElements);
					resources2 -= allBuyingResourcesTrucks;
					if ((storageTransferRequest.m_Flags & StorageTransferFlags.Incoming) != 0 || (storageTransferRequest.m_Flags & StorageTransferFlags.Car) == 0 || resources2 < storageTransferRequest.m_Amount)
					{
						continue;
					}
					if (m_Properties.HasComponent(storageTransferRequest.m_Target) || m_OutsideConnections.HasComponent(storageTransferRequest.m_Target))
					{
						if (storageTransferRequest.m_Amount > 0 && resources2 > 0)
						{
							m_DeliveryTruckSelectData.GetCapacityRange(storageTransferRequest.m_Resource, out var _, out var max);
							TripNeeded tripNeeded = new TripNeeded
							{
								m_TargetAgent = storageTransferRequest.m_Target,
								m_Purpose = Purpose.StorageTransfer,
								m_Resource = storageTransferRequest.m_Resource,
								m_Data = math.min(math.min(max, storageTransferRequest.m_Amount), resources2)
							};
							val2.Add(tripNeeded);
							EconomyUtils.AddResources(storageTransferRequest.m_Resource, -tripNeeded.m_Data, resources);
							storageTransferRequest.m_Amount -= tripNeeded.m_Data;
						}
						if (storageTransferRequest.m_Amount <= 0)
						{
							val.RemoveAt(j);
						}
						else
						{
							val[j] = storageTransferRequest;
						}
					}
					else
					{
						val.RemoveAt(j);
					}
					break;
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
		public ComponentLookup<Game.Vehicles.DeliveryTruck> __Game_Vehicles_DeliveryTruck_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<GuestVehicle> __Game_Vehicles_GuestVehicle_RO_BufferLookup;

		public BufferTypeHandle<StorageTransferRequest> __Game_Companies_StorageTransferRequest_RW_BufferTypeHandle;

		public BufferTypeHandle<Resources> __Game_Economy_Resources_RW_BufferTypeHandle;

		public BufferTypeHandle<TripNeeded> __Game_Citizens_TripNeeded_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> __Game_Objects_OutsideConnection_RO_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Vehicles_DeliveryTruck_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.DeliveryTruck>(true);
			__Game_Vehicles_LayoutElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(true);
			__Game_Vehicles_GuestVehicle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<GuestVehicle>(true);
			__Game_Companies_StorageTransferRequest_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<StorageTransferRequest>(false);
			__Game_Economy_Resources_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Resources>(false);
			__Game_Citizens_TripNeeded_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<TripNeeded>(false);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
			__Game_Objects_OutsideConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.OutsideConnection>(true);
		}
	}

	private EntityQuery m_TransferGroup;

	private VehicleCapacitySystem m_VehicleCapacitySystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
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
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_VehicleCapacitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<VehicleCapacitySystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_TransferGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<StorageTransferRequest>(),
			ComponentType.ReadOnly<Resources>(),
			ComponentType.ReadWrite<TripNeeded>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_TransferGroup);
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
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		TransferJob transferJob = new TransferJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DeliveryTrucks = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.DeliveryTruck>(ref __TypeHandle.__Game_Vehicles_DeliveryTruck_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LayoutElements = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GuestVehicles = InternalCompilerInterface.GetBufferLookup<GuestVehicle>(ref __TypeHandle.__Game_Vehicles_GuestVehicle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RequestType = InternalCompilerInterface.GetBufferTypeHandle<StorageTransferRequest>(ref __TypeHandle.__Game_Companies_StorageTransferRequest_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceType = InternalCompilerInterface.GetBufferTypeHandle<Resources>(ref __TypeHandle.__Game_Economy_Resources_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TripType = InternalCompilerInterface.GetBufferTypeHandle<TripNeeded>(ref __TypeHandle.__Game_Citizens_TripNeeded_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Properties = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OutsideConnections = InternalCompilerInterface.GetComponentLookup<Game.Objects.OutsideConnection>(ref __TypeHandle.__Game_Objects_OutsideConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DeliveryTruckSelectData = m_VehicleCapacitySystem.GetDeliveryTruckSelectData()
		};
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<TransferJob>(transferJob, m_TransferGroup, ((SystemBase)this).Dependency);
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
	public CarStorageTransferRequestSystem()
	{
	}
}
