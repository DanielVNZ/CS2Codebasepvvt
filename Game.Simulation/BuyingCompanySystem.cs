using System.Runtime.CompilerServices;
using Game.Buildings;
using Game.Citizens;
using Game.Common;
using Game.Companies;
using Game.Economy;
using Game.Notifications;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
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
public class BuyingCompanySystem : GameSystemBase
{
	[BurstCompile]
	private struct CompanyBuyJob : IJobChunk
	{
		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public BufferTypeHandle<OwnedVehicle> m_VehicleBufType;

		[ReadOnly]
		public BufferTypeHandle<Resources> m_ResourceBufType;

		[ReadOnly]
		public BufferTypeHandle<TripNeeded> m_TripNeededBufType;

		[ReadOnly]
		public BufferTypeHandle<TradeCost> m_TradeCostBufType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		[ReadOnly]
		public ComponentTypeHandle<PropertyRenter> m_PropertyRenterType;

		public ComponentTypeHandle<CompanyNotifications> m_CompanyNotificationsType;

		[ReadOnly]
		public ComponentLookup<StorageLimitData> m_StorageLimits;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> m_IndustrialProcessDatas;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.DeliveryTruck> m_Trucks;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenters;

		[ReadOnly]
		public ComponentLookup<Transform> m_Transforms;

		[ReadOnly]
		public ComponentLookup<ResourceData> m_ResourceDatas;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_Layouts;

		[ReadOnly]
		public DeliveryTruckSelectData m_DeliveryTruckSelectData;

		[ReadOnly]
		public ResourcePrefabs m_ResourcePrefabs;

		[ReadOnly]
		public uint m_UpdateFrameIndex;

		[ReadOnly]
		public CompanyNotificationParameterData m_CompanyNotificationParameters;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		public ParallelWriter m_CommandBuffer;

		public IconCommandBuffer m_IconCommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_0343: Unknown result type (might be due to invalid IL or missing references)
			//IL_034b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_042d: Unknown result type (might be due to invalid IL or missing references)
			//IL_043f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0446: Unknown result type (might be due to invalid IL or missing references)
			//IL_044b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_0466: Unknown result type (might be due to invalid IL or missing references)
			//IL_0468: Unknown result type (might be due to invalid IL or missing references)
			//IL_0492: Unknown result type (might be due to invalid IL or missing references)
			//IL_0499: Unknown result type (might be due to invalid IL or missing references)
			//IL_049e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index != m_UpdateFrameIndex)
			{
				return;
			}
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			BufferAccessor<OwnedVehicle> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<OwnedVehicle>(ref m_VehicleBufType);
			BufferAccessor<TripNeeded> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TripNeeded>(ref m_TripNeededBufType);
			BufferAccessor<Resources> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Resources>(ref m_ResourceBufType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabType);
			BufferAccessor<TradeCost> bufferAccessor4 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TradeCost>(ref m_TradeCostBufType);
			NativeArray<CompanyNotifications> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CompanyNotifications>(ref m_CompanyNotificationsType);
			NativeArray<PropertyRenter> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PropertyRenter>(ref m_PropertyRenterType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				CompanyNotifications companyNotifications = nativeArray3[i];
				DynamicBuffer<OwnedVehicle> vehicles = bufferAccessor[i];
				DynamicBuffer<TradeCost> tradeCosts = bufferAccessor4[i];
				DynamicBuffer<Resources> resourceBuffers = bufferAccessor3[i];
				DynamicBuffer<TripNeeded> trips = bufferAccessor2[i];
				int num = int.MaxValue;
				Entity prefab = nativeArray2[i].m_Prefab;
				if (m_StorageLimits.HasComponent(prefab))
				{
					num = m_StorageLimits[prefab].m_Limit;
				}
				IndustrialProcessData industrialProcessData = m_IndustrialProcessDatas[prefab];
				Entity val2 = val;
				if (nativeArray4.Length > 0)
				{
					val2 = nativeArray4[i].m_Property;
				}
				Resource needResource = Resource.NoResource;
				int needResourceLeft = 0;
				int storageLeft = num;
				bool expensive = false;
				bool flag = industrialProcessData.m_Input2.m_Resource != Resource.NoResource;
				bool flag2 = !flag && (industrialProcessData.m_Output.m_Resource == industrialProcessData.m_Input1.m_Resource || m_ResourceDatas[m_ResourcePrefabs[industrialProcessData.m_Output.m_Resource]].m_Weight <= 0f);
				CalculateResourceNeeded(isInput: true, industrialProcessData.m_Input1.m_Resource, ref needResource, ref needResourceLeft, ref storageLeft, ref expensive, tradeCosts, resourceBuffers, vehicles, trips);
				if (flag)
				{
					CalculateResourceNeeded(isInput: true, industrialProcessData.m_Input2.m_Resource, ref needResource, ref needResourceLeft, ref storageLeft, ref expensive, tradeCosts, resourceBuffers, vehicles, trips);
				}
				if (industrialProcessData.m_Output.m_Resource != industrialProcessData.m_Input1.m_Resource)
				{
					CalculateResourceNeeded(isInput: false, industrialProcessData.m_Output.m_Resource, ref needResource, ref needResourceLeft, ref storageLeft, ref expensive, tradeCosts, resourceBuffers, vehicles, trips);
				}
				if (companyNotifications.m_NoInputEntity == default(Entity) && expensive)
				{
					m_IconCommandBuffer.Add(val2, m_CompanyNotificationParameters.m_NoInputsNotificationPrefab, IconPriority.Problem);
					companyNotifications.m_NoInputEntity = val2;
					nativeArray3[i] = companyNotifications;
				}
				else if (companyNotifications.m_NoInputEntity != default(Entity))
				{
					if (!expensive)
					{
						m_IconCommandBuffer.Remove(companyNotifications.m_NoInputEntity, m_CompanyNotificationParameters.m_NoInputsNotificationPrefab);
						companyNotifications.m_NoInputEntity = Entity.Null;
						nativeArray3[i] = companyNotifications;
					}
					else if (val2 != companyNotifications.m_NoInputEntity)
					{
						m_IconCommandBuffer.Remove(companyNotifications.m_NoInputEntity, m_CompanyNotificationParameters.m_NoInputsNotificationPrefab);
						m_IconCommandBuffer.Add(val2, m_CompanyNotificationParameters.m_NoInputsNotificationPrefab, IconPriority.Problem);
						companyNotifications.m_NoInputEntity = val2;
						nativeArray3[i] = companyNotifications;
					}
				}
				if (needResource == Resource.NoResource)
				{
					continue;
				}
				m_DeliveryTruckSelectData.GetCapacityRange(needResource, out var _, out var max);
				int num2 = kResourceMinimumRequestAmount;
				DeliveryTruckSelectItem item = default(DeliveryTruckSelectItem);
				ResourceData resourceData = m_ResourceDatas[m_ResourcePrefabs[needResource]];
				int num3 = 0;
				if (resourceData.m_Weight > 0f)
				{
					num3 = ((!flag) ? (flag2 ? num : (num / 2)) : (num / 3));
					num2 = math.min(num3 - needResourceLeft, math.min(storageLeft, max));
					if (num2 <= kResourceMinimumRequestAmount)
					{
						continue;
					}
					m_DeliveryTruckSelectData.TrySelectItem(ref random, needResource, num2, out item);
				}
				else
				{
					item.m_Capacity = num2;
				}
				if (m_PropertyRenters.HasComponent(val))
				{
					Entity property = m_PropertyRenters[val].m_Property;
					if (m_Transforms.HasComponent(property))
					{
						ResourceBuyer resourceBuyer = new ResourceBuyer
						{
							m_Payer = val,
							m_AmountNeeded = math.min(num2, item.m_Capacity),
							m_Flags = (SetupTargetFlags.Industrial | SetupTargetFlags.Import),
							m_Location = m_Transforms[property].m_Position,
							m_ResourceNeeded = needResource
						};
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<ResourceBuyer>(unfilteredChunkIndex, val, resourceBuyer);
					}
				}
			}
		}

		private void CalculateResourceNeeded(bool isInput, Resource resource, ref Resource needResource, ref int needResourceLeft, ref int storageLeft, ref bool expensive, DynamicBuffer<TradeCost> tradeCosts, DynamicBuffer<Resources> resourceBuffers, DynamicBuffer<OwnedVehicle> vehicles, DynamicBuffer<TripNeeded> trips)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			int num = EconomyUtils.GetResources(resource, resourceBuffers);
			if (isInput)
			{
				if (EconomyUtils.GetTradeCost(resource, tradeCosts).m_BuyCost > kNotificationCostLimit)
				{
					expensive = true;
				}
				for (int i = 0; i < vehicles.Length; i++)
				{
					Entity vehicle = vehicles[i].m_Vehicle;
					num += VehicleUtils.GetBuyingTrucksLoad(vehicle, resource, ref m_Trucks, ref m_Layouts);
				}
				for (int j = 0; j < trips.Length; j++)
				{
					TripNeeded tripNeeded = trips[j];
					if (tripNeeded.m_Purpose == Purpose.Shopping && tripNeeded.m_Resource == resource)
					{
						num += tripNeeded.m_Data;
					}
				}
				if (needResource == Resource.NoResource && num < kResourceLowStockAmount)
				{
					needResource = resource;
					needResourceLeft = num;
				}
			}
			if (EconomyUtils.IsMaterial(resource, m_ResourcePrefabs, ref m_ResourceDatas))
			{
				storageLeft -= num;
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
		public BufferTypeHandle<Resources> __Game_Economy_Resources_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<OwnedVehicle> __Game_Vehicles_OwnedVehicle_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<TripNeeded> __Game_Citizens_TripNeeded_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<TradeCost> __Game_Companies_TradeCost_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentTypeHandle;

		public ComponentTypeHandle<CompanyNotifications> __Game_Companies_CompanyNotifications_RW_ComponentTypeHandle;

		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> __Game_Prefabs_IndustrialProcessData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StorageLimitData> __Game_Companies_StorageLimitData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.DeliveryTruck> __Game_Vehicles_DeliveryTruck_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<ResourceData> __Game_Prefabs_ResourceData_RO_ComponentLookup;

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
			__Game_Economy_Resources_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Resources>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Vehicles_OwnedVehicle_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<OwnedVehicle>(true);
			__Game_Citizens_TripNeeded_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<TripNeeded>(true);
			__Game_Companies_TradeCost_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<TradeCost>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PropertyRenter>(true);
			__Game_Companies_CompanyNotifications_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CompanyNotifications>(false);
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<IndustrialProcessData>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
			__Game_Companies_StorageLimitData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StorageLimitData>(true);
			__Game_Vehicles_DeliveryTruck_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.DeliveryTruck>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Vehicles_LayoutElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(true);
			__Game_Prefabs_ResourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceData>(true);
		}
	}

	private static readonly float kNotificationCostLimit = 5f;

	private static readonly int kResourceLowStockAmount = 4000;

	private static readonly int kResourceMinimumRequestAmount = 2000;

	private SimulationSystem m_SimulationSystem;

	private ResourceSystem m_ResourceSystem;

	private VehicleCapacitySystem m_VehicleCapacitySystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private IconCommandSystem m_IconCommandSystem;

	private EntityQuery m_CompanyNotificationParameterQuery;

	private EntityQuery m_CompanyGroup;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 256;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		m_VehicleCapacitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<VehicleCapacitySystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_IconCommandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IconCommandSystem>();
		m_CompanyGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[11]
		{
			ComponentType.ReadOnly<BuyingCompany>(),
			ComponentType.ReadOnly<Resources>(),
			ComponentType.ReadWrite<OwnedVehicle>(),
			ComponentType.ReadOnly<PropertyRenter>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<TradeCost>(),
			ComponentType.ReadWrite<CompanyNotifications>(),
			ComponentType.ReadWrite<TripNeeded>(),
			ComponentType.Exclude<ResourceBuyer>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.ReadOnly<UpdateFrame>()
		});
		m_CompanyNotificationParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<CompanyNotificationParameterData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_CompanyGroup);
		((ComponentSystemBase)this).RequireForUpdate(m_CompanyNotificationParameterQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		uint updateFrameWithInterval = SimulationUtils.GetUpdateFrameWithInterval(m_SimulationSystem.frameIndex, (uint)GetUpdateInterval(SystemUpdatePhase.GameSimulation), 16);
		CompanyBuyJob companyBuyJob = new CompanyBuyJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceBufType = InternalCompilerInterface.GetBufferTypeHandle<Resources>(ref __TypeHandle.__Game_Economy_Resources_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_VehicleBufType = InternalCompilerInterface.GetBufferTypeHandle<OwnedVehicle>(ref __TypeHandle.__Game_Vehicles_OwnedVehicle_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TripNeededBufType = InternalCompilerInterface.GetBufferTypeHandle<TripNeeded>(ref __TypeHandle.__Game_Citizens_TripNeeded_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TradeCostBufType = InternalCompilerInterface.GetBufferTypeHandle<TradeCost>(ref __TypeHandle.__Game_Companies_TradeCost_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyRenterType = InternalCompilerInterface.GetComponentTypeHandle<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CompanyNotificationsType = InternalCompilerInterface.GetComponentTypeHandle<CompanyNotifications>(ref __TypeHandle.__Game_Companies_CompanyNotifications_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_IndustrialProcessDatas = InternalCompilerInterface.GetComponentLookup<IndustrialProcessData>(ref __TypeHandle.__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyRenters = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StorageLimits = InternalCompilerInterface.GetComponentLookup<StorageLimitData>(ref __TypeHandle.__Game_Companies_StorageLimitData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Trucks = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.DeliveryTruck>(ref __TypeHandle.__Game_Vehicles_DeliveryTruck_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Transforms = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Layouts = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourcePrefabs = m_ResourceSystem.GetPrefabs(),
			m_ResourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CompanyNotificationParameters = ((EntityQuery)(ref m_CompanyNotificationParameterQuery)).GetSingleton<CompanyNotificationParameterData>()
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		companyBuyJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		companyBuyJob.m_DeliveryTruckSelectData = m_VehicleCapacitySystem.GetDeliveryTruckSelectData();
		companyBuyJob.m_UpdateFrameIndex = updateFrameWithInterval;
		companyBuyJob.m_IconCommandBuffer = m_IconCommandSystem.CreateCommandBuffer();
		companyBuyJob.m_RandomSeed = RandomSeed.Next();
		CompanyBuyJob companyBuyJob2 = companyBuyJob;
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<CompanyBuyJob>(companyBuyJob2, m_CompanyGroup, ((SystemBase)this).Dependency);
		m_ResourceSystem.AddPrefabsReader(((SystemBase)this).Dependency);
		m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
		m_IconCommandSystem.AddCommandBufferWriter(((SystemBase)this).Dependency);
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
	public BuyingCompanySystem()
	{
	}
}
