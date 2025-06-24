using Game.Buildings;
using Game.Citizens;
using Game.Common;
using Game.Companies;
using Game.Economy;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Tools;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Simulation;

public struct ResourcePathfindSetup
{
	[BurstCompile]
	private struct SetupResourceSellerJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> m_IndustrialProcessDatas;

		[ReadOnly]
		public ComponentLookup<ServiceAvailable> m_ServiceAvailables;

		[ReadOnly]
		public ComponentLookup<StorageCompanyData> m_StorageCompanyDatas;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenters;

		[ReadOnly]
		public BufferLookup<Resources> m_Resources;

		[ReadOnly]
		public BufferLookup<TradeCost> m_TradeCosts;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> m_OutsideConnections;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.CargoTransportStation> m_CargoTransportStations;

		[ReadOnly]
		public BufferTypeHandle<StorageTransferRequest> m_StorageTransferRequestType;

		[ReadOnly]
		public BufferTypeHandle<TripNeeded> m_TripNeededType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<Building> m_Buildings;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.DeliveryTruck> m_DeliveryTrucks;

		[ReadOnly]
		public BufferLookup<GuestVehicle> m_GuestVehicleBufs;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_LayoutElementBufs;

		public PathfindSetupSystem.SetupData m_SetupData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			BufferAccessor<StorageTransferRequest> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<StorageTransferRequest>(ref m_StorageTransferRequestType);
			BufferAccessor<TripNeeded> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TripNeeded>(ref m_TripNeededType);
			for (int i = 0; i < m_SetupData.Length; i++)
			{
				m_SetupData.GetItem(i, out var entity, out var targetSeeker);
				Resource resource = targetSeeker.m_SetupQueueTarget.m_Resource;
				int value = targetSeeker.m_SetupQueueTarget.m_Value;
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Entity val = nativeArray[j];
					Entity prefab = m_Prefabs[val].m_Prefab;
					int num = ((bufferAccessor.Length > 0) ? bufferAccessor[j].Length : 0);
					if (((Entity)(ref val)).Equals(entity))
					{
						continue;
					}
					bool flag = m_OutsideConnections.HasComponent(val);
					bool flag2 = m_CargoTransportStations.HasComponent(val);
					bool flag3 = m_StorageCompanyDatas.HasComponent(prefab) && !flag2 && !flag;
					bool flag4 = m_ServiceAvailables.HasComponent(val);
					bool flag5 = m_IndustrialProcessDatas.HasComponent(prefab) && !flag4 && !flag3;
					bool flag6 = EconomyUtils.IsOfficeResource(resource);
					if ((m_Buildings.HasComponent(val) && BuildingUtils.CheckOption(m_Buildings[val], BuildingOption.Inactive)) || ((flag4 || flag5) && (!m_PropertyRenters.HasComponent(val) || m_PropertyRenters[val].m_Property == Entity.Null)))
					{
						continue;
					}
					bool flag7 = false;
					if (flag6 && flag5 && (m_IndustrialProcessDatas[prefab].m_Output.m_Resource & resource) != Resource.NoResource)
					{
						flag7 = true;
					}
					else if ((targetSeeker.m_SetupQueueTarget.m_Flags & SetupTargetFlags.Commercial) != 0 && flag4 && (m_IndustrialProcessDatas[prefab].m_Output.m_Resource & resource) != Resource.NoResource)
					{
						flag7 = true;
					}
					else if ((targetSeeker.m_SetupQueueTarget.m_Flags & SetupTargetFlags.Industrial) != 0 && flag5 && (m_IndustrialProcessDatas[prefab].m_Output.m_Resource & resource) != Resource.NoResource)
					{
						flag7 = true;
					}
					else if ((targetSeeker.m_SetupQueueTarget.m_Flags & SetupTargetFlags.Import) != SetupTargetFlags.None && (flag || flag2 || flag3))
					{
						flag7 = true;
					}
					if (!flag7)
					{
						continue;
					}
					int allBuyingResourcesTrucks = VehicleUtils.GetAllBuyingResourcesTrucks(val, resource, ref m_DeliveryTrucks, ref m_GuestVehicleBufs, ref m_LayoutElementBufs);
					int num2 = EconomyUtils.GetResources(resource, m_Resources[val]) - allBuyingResourcesTrucks;
					if (num2 <= 0)
					{
						continue;
					}
					float num3 = 0f;
					if (m_ServiceAvailables.HasComponent(val))
					{
						num3 -= (float)(math.min(num2, m_ServiceAvailables[val].m_ServiceAvailable) * 100);
					}
					else
					{
						if (!flag && num2 / 2 < value)
						{
							continue;
						}
						float num4 = math.min(1f, (float)num2 * 1f / (float)value);
						num3 += 100f * (1f - num4);
						if (flag2)
						{
							if (bufferAccessor2.Length > 0 && bufferAccessor2[j].Length >= kCargoStationMaxTripNeededQueue)
							{
								continue;
							}
							num3 += kCargoStationAmountBasedPenalty * (float)value;
							num3 += kCargoStationPerRequestPenalty * (float)num;
						}
						if (flag)
						{
							num3 += kOutsideConnectionAmountBasedPenalty * (float)value;
						}
					}
					if (m_TradeCosts.HasBuffer(val))
					{
						DynamicBuffer<TradeCost> costs = m_TradeCosts[val];
						num3 += EconomyUtils.GetTradeCost(resource, costs).m_BuyCost * (float)value * 0.01f;
					}
					targetSeeker.FindTargets(val, num3 * 100f);
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct SetupResourceExportJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public BufferTypeHandle<Resources> m_ResourceType;

		[ReadOnly]
		public BufferTypeHandle<TradeCost> m_TradeCosts;

		[ReadOnly]
		public BufferTypeHandle<OwnedVehicle> m_OwnedVehicles;

		[ReadOnly]
		public BufferTypeHandle<TripNeeded> m_TripNeededType;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> m_InstalledUpgradeType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		[ReadOnly]
		public ComponentLookup<StorageLimitData> m_Limits;

		[ReadOnly]
		public ComponentLookup<StorageCompanyData> m_StorageCompanyDatas;

		[ReadOnly]
		public ComponentLookup<TransportCompanyData> m_TransportCompanyData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_BuildingDatas;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> m_SpawnableBuildingData;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_Properties;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.CargoTransportStation> m_CargoTransportStations;

		[ReadOnly]
		public ComponentLookup<Building> m_Buildings;

		public PathfindSetupSystem.SetupData m_SetupData;

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
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabType);
			BufferAccessor<Resources> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Resources>(ref m_ResourceType);
			BufferAccessor<TradeCost> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TradeCost>(ref m_TradeCosts);
			BufferAccessor<OwnedVehicle> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<OwnedVehicle>(ref m_OwnedVehicles);
			BufferAccessor<TripNeeded> bufferAccessor4 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TripNeeded>(ref m_TripNeededType);
			BufferAccessor<InstalledUpgrade> bufferAccessor5 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<InstalledUpgrade>(ref m_InstalledUpgradeType);
			for (int i = 0; i < m_SetupData.Length; i++)
			{
				m_SetupData.GetItem(i, out var _, out var targetSeeker);
				Resource resource = targetSeeker.m_SetupQueueTarget.m_Resource;
				int value = targetSeeker.m_SetupQueueTarget.m_Value;
				if ((targetSeeker.m_SetupQueueTarget.m_Flags & SetupTargetFlags.RequireTransport) != SetupTargetFlags.None && bufferAccessor3.Length == 0)
				{
					continue;
				}
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Entity val = nativeArray[j];
					Entity prefab = nativeArray2[j].m_Prefab;
					if (m_Buildings.HasComponent(val) && BuildingUtils.CheckOption(m_Buildings[val], BuildingOption.Inactive))
					{
						continue;
					}
					bool flag = m_CargoTransportStations.HasComponent(val);
					int num = value;
					float num2 = 0.01f;
					if (m_Limits.HasComponent(prefab))
					{
						StorageLimitData data = m_Limits[prefab];
						if (bufferAccessor5.Length != 0)
						{
							UpgradeUtils.CombineStats<StorageLimitData>(ref data, bufferAccessor5[j], ref targetSeeker.m_PrefabRef, ref m_Limits);
						}
						if (m_Properties.HasComponent(val))
						{
							Entity property = m_Properties[val].m_Property;
							if (m_Prefabs.HasComponent(property))
							{
								Entity prefab2 = m_Prefabs[property].m_Prefab;
								int adjustedLimitForWarehouse = data.GetAdjustedLimitForWarehouse(m_SpawnableBuildingData.HasComponent(prefab2) ? m_SpawnableBuildingData[prefab2] : new SpawnableBuildingData
								{
									m_Level = 1
								}, m_SpawnableBuildingData.HasComponent(prefab2) ? m_BuildingDatas[prefab2] : new BuildingData
								{
									m_LotSize = new int2(1, 1)
								});
								num = adjustedLimitForWarehouse - EconomyUtils.GetResources(resource, bufferAccessor[j]);
								num2 = (float)num / math.max(1f, (float)adjustedLimitForWarehouse);
							}
						}
					}
					StorageCompanyData data2 = m_StorageCompanyDatas[prefab];
					if (bufferAccessor5.Length != 0)
					{
						UpgradeUtils.CombineStats<StorageCompanyData>(ref data2, bufferAccessor5[j], ref targetSeeker.m_PrefabRef, ref m_StorageCompanyDatas);
					}
					if ((resource & data2.m_StoredResources) == Resource.NoResource || num < value)
					{
						continue;
					}
					float num3 = 0f;
					if ((targetSeeker.m_SetupQueueTarget.m_Flags & SetupTargetFlags.RequireTransport) != SetupTargetFlags.None)
					{
						if (!m_TransportCompanyData.HasComponent(prefab))
						{
							continue;
						}
						TransportCompanyData transportCompanyData = m_TransportCompanyData[prefab];
						if (bufferAccessor3[j].Length >= transportCompanyData.m_MaxTransports)
						{
							continue;
						}
					}
					if (!flag || bufferAccessor4.Length <= 0 || bufferAccessor4[j].Length < kCargoStationMaxTripNeededQueue)
					{
						float num4 = EconomyUtils.GetTradeCost(resource, bufferAccessor2[j]).m_SellCost * (float)value * 0.01f;
						num4 += num3 * (float)kCargoStationVehiclePenalty;
						targetSeeker.FindTargets(val, math.max(0f, -2000f * num2 + num4));
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct SetupStorageTransferJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Companies.StorageCompany> m_StorageType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.OutsideConnection> m_OutsideConnectionType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		[ReadOnly]
		public BufferTypeHandle<Resources> m_ResourceType;

		[ReadOnly]
		public BufferTypeHandle<TradeCost> m_TradeCostType;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> m_InstalledUpgradeType;

		[ReadOnly]
		public BufferTypeHandle<StorageTransferRequest> m_StorageTransferRequestType;

		[ReadOnly]
		public BufferTypeHandle<TripNeeded> m_TripNeededType;

		[ReadOnly]
		public BufferTypeHandle<OwnedVehicle> m_OwnedVehicleType;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.CargoTransportStation> m_CargoTransportStations;

		[ReadOnly]
		public ComponentLookup<StorageCompanyData> m_StorageCompanyDatas;

		[ReadOnly]
		public ComponentLookup<StorageLimitData> m_StorageLimits;

		[ReadOnly]
		public ComponentLookup<TransportCompanyData> m_TransportCompanyDatas;

		[ReadOnly]
		public ComponentLookup<Building> m_Buildings;

		public PathfindSetupSystem.SetupData m_SetupData;

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
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0414: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Game.Companies.StorageCompany> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Companies.StorageCompany>(ref m_StorageType);
			NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabType);
			BufferAccessor<Resources> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Resources>(ref m_ResourceType);
			BufferAccessor<TradeCost> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TradeCost>(ref m_TradeCostType);
			BufferAccessor<InstalledUpgrade> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<InstalledUpgrade>(ref m_InstalledUpgradeType);
			BufferAccessor<StorageTransferRequest> bufferAccessor4 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<StorageTransferRequest>(ref m_StorageTransferRequestType);
			BufferAccessor<OwnedVehicle> bufferAccessor5 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<OwnedVehicle>(ref m_OwnedVehicleType);
			BufferAccessor<TripNeeded> bufferAccessor6 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TripNeeded>(ref m_TripNeededType);
			for (int i = 0; i < m_SetupData.Length; i++)
			{
				m_SetupData.GetItem(i, out var entity, out var targetSeeker);
				Resource resource = targetSeeker.m_SetupQueueTarget.m_Resource;
				int value = targetSeeker.m_SetupQueueTarget.m_Value;
				float value2 = targetSeeker.m_SetupQueueTarget.m_Value2;
				long num = targetSeeker.m_SetupQueueTarget.m_Value3;
				long num2 = Mathf.RoundToInt(value2 * (float)num);
				bool flag = ((ArchetypeChunk)(ref chunk)).Has<Game.Objects.OutsideConnection>(ref m_OutsideConnectionType);
				switch (resource)
				{
				case Resource.LocalMail:
					if (value > 0 && flag)
					{
						continue;
					}
					break;
				case Resource.UnsortedMail:
				case Resource.OutgoingMail:
					if (value < 0 && flag)
					{
						continue;
					}
					break;
				}
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Entity val = nativeArray[j];
					bool flag2 = m_CargoTransportStations.HasComponent(val);
					if (((Entity)(ref val)).Equals(entity) || (m_Buildings.HasComponent(val) && BuildingUtils.CheckOption(m_Buildings[val], BuildingOption.Inactive)))
					{
						continue;
					}
					_ = nativeArray2[j];
					Entity prefab = nativeArray3[j].m_Prefab;
					if (!m_StorageCompanyDatas.HasComponent(prefab))
					{
						continue;
					}
					StorageCompanyData data = m_StorageCompanyDatas[prefab];
					StorageLimitData data2 = m_StorageLimits[prefab];
					int num3 = (m_TransportCompanyDatas.HasComponent(prefab) ? m_TransportCompanyDatas[prefab].m_MaxTransports : 0);
					DynamicBuffer<Resources> resources = bufferAccessor[j];
					DynamicBuffer<StorageTransferRequest> val2 = bufferAccessor4[j];
					if (bufferAccessor3.Length != 0)
					{
						UpgradeUtils.CombineStats<StorageLimitData>(ref data2, bufferAccessor3[j], ref targetSeeker.m_PrefabRef, ref m_StorageLimits);
						UpgradeUtils.CombineStats<StorageCompanyData>(ref data, bufferAccessor3[j], ref targetSeeker.m_PrefabRef, ref m_StorageCompanyDatas);
					}
					long num4 = EconomyUtils.GetResources(resource, resources);
					long num5 = 0L;
					for (int k = 0; k < val2.Length; k++)
					{
						StorageTransferRequest storageTransferRequest = val2[k];
						if (storageTransferRequest.m_Resource == resource)
						{
							num5 += (((storageTransferRequest.m_Flags & StorageTransferFlags.Incoming) != 0) ? storageTransferRequest.m_Amount : (-storageTransferRequest.m_Amount));
						}
					}
					num4 += num5;
					int num6 = math.max(1, EconomyUtils.CountResources(data.m_StoredResources));
					long num7 = data2.m_Limit / num6;
					long num8 = value;
					if (flag2)
					{
						num8 = ((value <= 0) ? (-math.min((long)(-value), num4)) : math.min((long)value, num7 - num4));
					}
					else if (!flag)
					{
						if (num7 + num > 0)
						{
							num8 = (num7 * num2 - num * num4) / (num7 + num);
							if ((value > 0 && num8 < 0) || (value < 0 && num8 > 0))
							{
								num8 = 0L;
							}
						}
						else
						{
							num8 = 0L;
						}
					}
					if (math.abs(num8) < 4000)
					{
						continue;
					}
					float num9 = ((value != 0) ? (1000f * (float)math.abs(num8 / value)) : 0f);
					DynamicBuffer<TradeCost> costs = bufferAccessor2[j];
					TradeCost tradeCost = EconomyUtils.GetTradeCost(resource, costs);
					float num10 = 0.01f * (float)value * math.max(0.1f, (value > 0) ? tradeCost.m_SellCost : (0f - tradeCost.m_BuyCost));
					if (flag2)
					{
						num10 += (float)val2.Length * kCargoStationPerRequestPenalty;
						if (bufferAccessor6.Length > 0 && bufferAccessor6[j].Length > kCargoStationMaxTripNeededQueue)
						{
							continue;
						}
						if (bufferAccessor5.Length > 0 && bufferAccessor5[j].Length >= num3)
						{
							num10 += 1f * (float)bufferAccessor5[j].Length / (float)num3 * (float)kCargoStationVehiclePenalty;
						}
					}
					if ((data.m_StoredResources & resource) != Resource.NoResource && num9 > 0f)
					{
						targetSeeker.FindTargets(val, num10 - num9);
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	public static readonly float kOutsideConnectionAmountBasedPenalty = 0.001f;

	public static readonly float kCargoStationAmountBasedPenalty = 0.0001f;

	public static readonly float kCargoStationPerRequestPenalty = 0.0001f;

	public static readonly int kCargoStationVehiclePenalty = 5000;

	public static readonly int kCargoStationMaxRequestAmount = 5;

	public static readonly int kCargoStationMaxTripNeededQueue = 10;

	private EntityQuery m_ResourceSellerQuery;

	private EntityQuery m_ExportTargetQuery;

	private EntityQuery m_StorageQuery;

	private ResourceSystem m_ResourceSystem;

	private EntityTypeHandle m_EntityType;

	private ComponentTypeHandle<Game.Objects.OutsideConnection> m_OutsideConnectionType;

	private ComponentTypeHandle<Game.Companies.StorageCompany> m_StorageCompanyType;

	private ComponentTypeHandle<PrefabRef> m_PrefabType;

	private BufferTypeHandle<TradeCost> m_TradeCostType;

	private BufferTypeHandle<StorageTransferRequest> m_StorageTransferRequestType;

	private BufferTypeHandle<Resources> m_ResourceType;

	private BufferTypeHandle<OwnedVehicle> m_OwnedVehicleType;

	private BufferTypeHandle<InstalledUpgrade> m_InstalledUpgradeType;

	private BufferTypeHandle<TripNeeded> m_TripNeededType;

	private ComponentLookup<Game.Objects.OutsideConnection> m_OutsideConnections;

	private ComponentLookup<ServiceCompanyData> m_ServiceCompanies;

	private ComponentLookup<ServiceAvailable> m_ServiceAvailables;

	private ComponentLookup<Game.Companies.StorageCompany> m_StorageCompanys;

	private ComponentLookup<StorageLimitData> m_StorageLimits;

	private ComponentLookup<TransportCompanyData> m_TransportCompanyData;

	private ComponentLookup<Game.Buildings.CargoTransportStation> m_CargoTransportStations;

	private ComponentLookup<PropertyRenter> m_PropertyRenters;

	private ComponentLookup<PrefabRef> m_Prefabs;

	private ComponentLookup<IndustrialProcessData> m_IndustrialProcessDatas;

	private ComponentLookup<ResourceData> m_ResourceDatas;

	private ComponentLookup<StorageCompanyData> m_StorageCompanyDatas;

	private ComponentLookup<SpawnableBuildingData> m_SpawnableBuildingDatas;

	private ComponentLookup<BuildingData> m_BuildingDatas;

	private ComponentLookup<Building> m_Buildings;

	private ComponentLookup<Game.Vehicles.DeliveryTruck> m_DeliveryTrucks;

	private BufferLookup<Resources> m_Resources;

	private BufferLookup<TradeCost> m_TradeCosts;

	private BufferLookup<GuestVehicle> m_GuestVehicleBufs;

	private BufferLookup<LayoutElement> m_LayoutElementBufs;

	public ResourcePathfindSetup(PathfindSetupSystem system)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Expected O, but got Unknown
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		m_ResourceSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<ResourceSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<Resources>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Companies.StorageCompany>(),
			ComponentType.ReadOnly<Game.Buildings.CargoTransportStation>(),
			ComponentType.ReadOnly<ResourceSeller>()
		};
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Destroyed>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_ResourceSellerQuery = system.GetSetupQuery((EntityQueryDesc[])(object)array);
		m_ExportTargetQuery = system.GetSetupQuery(ComponentType.ReadOnly<Game.Companies.StorageCompany>(), ComponentType.ReadOnly<PrefabRef>(), ComponentType.ReadOnly<Resources>(), ComponentType.ReadOnly<TradeCost>(), ComponentType.Exclude<Deleted>(), ComponentType.Exclude<Destroyed>(), ComponentType.Exclude<Temp>());
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Companies.StorageCompany>(),
			ComponentType.ReadOnly<PrefabRef>()
		};
		val.None = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Game.Companies.ProcessingCompany>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Destroyed>(),
			ComponentType.ReadOnly<Temp>()
		};
		array2[0] = val;
		m_StorageQuery = system.GetSetupQuery((EntityQueryDesc[])(object)array2);
		m_EntityType = ((ComponentSystemBase)system).GetEntityTypeHandle();
		m_OutsideConnectionType = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Objects.OutsideConnection>(true);
		m_StorageCompanyType = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Companies.StorageCompany>(true);
		m_PrefabType = ((ComponentSystemBase)system).GetComponentTypeHandle<PrefabRef>(true);
		m_TradeCostType = ((ComponentSystemBase)system).GetBufferTypeHandle<TradeCost>(true);
		m_StorageTransferRequestType = ((ComponentSystemBase)system).GetBufferTypeHandle<StorageTransferRequest>(true);
		m_TripNeededType = ((ComponentSystemBase)system).GetBufferTypeHandle<TripNeeded>(true);
		m_OwnedVehicleType = ((ComponentSystemBase)system).GetBufferTypeHandle<OwnedVehicle>(true);
		m_ResourceType = ((ComponentSystemBase)system).GetBufferTypeHandle<Resources>(true);
		m_InstalledUpgradeType = ((ComponentSystemBase)system).GetBufferTypeHandle<InstalledUpgrade>(true);
		m_OutsideConnections = ((SystemBase)system).GetComponentLookup<Game.Objects.OutsideConnection>(true);
		m_ServiceCompanies = ((SystemBase)system).GetComponentLookup<ServiceCompanyData>(true);
		m_ServiceAvailables = ((SystemBase)system).GetComponentLookup<ServiceAvailable>(true);
		m_StorageCompanys = ((SystemBase)system).GetComponentLookup<Game.Companies.StorageCompany>(true);
		m_StorageLimits = ((SystemBase)system).GetComponentLookup<StorageLimitData>(true);
		m_TransportCompanyData = ((SystemBase)system).GetComponentLookup<TransportCompanyData>(true);
		m_PropertyRenters = ((SystemBase)system).GetComponentLookup<PropertyRenter>(true);
		m_Prefabs = ((SystemBase)system).GetComponentLookup<PrefabRef>(true);
		m_IndustrialProcessDatas = ((SystemBase)system).GetComponentLookup<IndustrialProcessData>(true);
		m_ResourceDatas = ((SystemBase)system).GetComponentLookup<ResourceData>(true);
		m_StorageCompanyDatas = ((SystemBase)system).GetComponentLookup<StorageCompanyData>(true);
		m_SpawnableBuildingDatas = ((SystemBase)system).GetComponentLookup<SpawnableBuildingData>(true);
		m_BuildingDatas = ((SystemBase)system).GetComponentLookup<BuildingData>(true);
		m_Buildings = ((SystemBase)system).GetComponentLookup<Building>(true);
		m_DeliveryTrucks = ((SystemBase)system).GetComponentLookup<Game.Vehicles.DeliveryTruck>(true);
		m_Resources = ((SystemBase)system).GetBufferLookup<Resources>(true);
		m_TradeCosts = ((SystemBase)system).GetBufferLookup<TradeCost>(true);
		m_GuestVehicleBufs = ((SystemBase)system).GetBufferLookup<GuestVehicle>(true);
		m_LayoutElementBufs = ((SystemBase)system).GetBufferLookup<LayoutElement>(true);
		m_CargoTransportStations = ((SystemBase)system).GetComponentLookup<Game.Buildings.CargoTransportStation>(true);
	}

	public JobHandle SetupResourceSeller(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		((EntityTypeHandle)(ref m_EntityType)).Update((SystemBase)(object)system);
		m_Resources.Update((SystemBase)(object)system);
		m_IndustrialProcessDatas.Update((SystemBase)(object)system);
		m_CargoTransportStations.Update((SystemBase)(object)system);
		m_StorageCompanyDatas.Update((SystemBase)(object)system);
		m_PropertyRenters.Update((SystemBase)(object)system);
		m_TradeCosts.Update((SystemBase)(object)system);
		m_ServiceAvailables.Update((SystemBase)(object)system);
		m_OutsideConnections.Update((SystemBase)(object)system);
		m_StorageTransferRequestType.Update((SystemBase)(object)system);
		m_TripNeededType.Update((SystemBase)(object)system);
		m_Prefabs.Update((SystemBase)(object)system);
		m_Buildings.Update((SystemBase)(object)system);
		m_DeliveryTrucks.Update((SystemBase)(object)system);
		m_GuestVehicleBufs.Update((SystemBase)(object)system);
		m_LayoutElementBufs.Update((SystemBase)(object)system);
		JobHandle val = JobChunkExtensions.ScheduleParallel<SetupResourceSellerJob>(new SetupResourceSellerJob
		{
			m_EntityType = m_EntityType,
			m_StorageTransferRequestType = m_StorageTransferRequestType,
			m_TripNeededType = m_TripNeededType,
			m_Resources = m_Resources,
			m_IndustrialProcessDatas = m_IndustrialProcessDatas,
			m_CargoTransportStations = m_CargoTransportStations,
			m_StorageCompanyDatas = m_StorageCompanyDatas,
			m_PropertyRenters = m_PropertyRenters,
			m_TradeCosts = m_TradeCosts,
			m_ServiceAvailables = m_ServiceAvailables,
			m_OutsideConnections = m_OutsideConnections,
			m_Prefabs = m_Prefabs,
			m_Buildings = m_Buildings,
			m_DeliveryTrucks = m_DeliveryTrucks,
			m_GuestVehicleBufs = m_GuestVehicleBufs,
			m_LayoutElementBufs = m_LayoutElementBufs,
			m_SetupData = setupData
		}, m_ResourceSellerQuery, inputDeps);
		m_ResourceSystem.AddPrefabsReader(val);
		return val;
	}

	public JobHandle SetupResourceExport(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		((EntityTypeHandle)(ref m_EntityType)).Update((SystemBase)(object)system);
		m_StorageLimits.Update((SystemBase)(object)system);
		m_PrefabType.Update((SystemBase)(object)system);
		m_ResourceType.Update((SystemBase)(object)system);
		m_OwnedVehicleType.Update((SystemBase)(object)system);
		m_TripNeededType.Update((SystemBase)(object)system);
		m_TradeCostType.Update((SystemBase)(object)system);
		m_InstalledUpgradeType.Update((SystemBase)(object)system);
		m_StorageCompanyDatas.Update((SystemBase)(object)system);
		m_TransportCompanyData.Update((SystemBase)(object)system);
		m_BuildingDatas.Update((SystemBase)(object)system);
		m_SpawnableBuildingDatas.Update((SystemBase)(object)system);
		m_Prefabs.Update((SystemBase)(object)system);
		m_PropertyRenters.Update((SystemBase)(object)system);
		m_CargoTransportStations.Update((SystemBase)(object)system);
		m_Buildings.Update((SystemBase)(object)system);
		return JobChunkExtensions.ScheduleParallel<SetupResourceExportJob>(new SetupResourceExportJob
		{
			m_EntityType = m_EntityType,
			m_Limits = m_StorageLimits,
			m_PrefabType = m_PrefabType,
			m_ResourceType = m_ResourceType,
			m_OwnedVehicles = m_OwnedVehicleType,
			m_TripNeededType = m_TripNeededType,
			m_TradeCosts = m_TradeCostType,
			m_InstalledUpgradeType = m_InstalledUpgradeType,
			m_StorageCompanyDatas = m_StorageCompanyDatas,
			m_TransportCompanyData = m_TransportCompanyData,
			m_BuildingDatas = m_BuildingDatas,
			m_SpawnableBuildingData = m_SpawnableBuildingDatas,
			m_Prefabs = m_Prefabs,
			m_Properties = m_PropertyRenters,
			m_CargoTransportStations = m_CargoTransportStations,
			m_Buildings = m_Buildings,
			m_SetupData = setupData
		}, m_ExportTargetQuery, inputDeps);
	}

	public JobHandle SetupStorageTransfer(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		((EntityTypeHandle)(ref m_EntityType)).Update((SystemBase)(object)system);
		m_StorageCompanyType.Update((SystemBase)(object)system);
		m_OutsideConnectionType.Update((SystemBase)(object)system);
		m_CargoTransportStations.Update((SystemBase)(object)system);
		m_PrefabType.Update((SystemBase)(object)system);
		m_ResourceType.Update((SystemBase)(object)system);
		m_TradeCostType.Update((SystemBase)(object)system);
		m_InstalledUpgradeType.Update((SystemBase)(object)system);
		m_StorageTransferRequestType.Update((SystemBase)(object)system);
		m_TripNeededType.Update((SystemBase)(object)system);
		m_OwnedVehicleType.Update((SystemBase)(object)system);
		m_Buildings.Update((SystemBase)(object)system);
		m_StorageLimits.Update((SystemBase)(object)system);
		m_StorageCompanyDatas.Update((SystemBase)(object)system);
		m_TransportCompanyData.Update((SystemBase)(object)system);
		return JobChunkExtensions.ScheduleParallel<SetupStorageTransferJob>(new SetupStorageTransferJob
		{
			m_EntityType = m_EntityType,
			m_StorageType = m_StorageCompanyType,
			m_OutsideConnectionType = m_OutsideConnectionType,
			m_CargoTransportStations = m_CargoTransportStations,
			m_PrefabType = m_PrefabType,
			m_ResourceType = m_ResourceType,
			m_TradeCostType = m_TradeCostType,
			m_InstalledUpgradeType = m_InstalledUpgradeType,
			m_StorageTransferRequestType = m_StorageTransferRequestType,
			m_TripNeededType = m_TripNeededType,
			m_OwnedVehicleType = m_OwnedVehicleType,
			m_Buildings = m_Buildings,
			m_StorageLimits = m_StorageLimits,
			m_StorageCompanyDatas = m_StorageCompanyDatas,
			m_TransportCompanyDatas = m_TransportCompanyData,
			m_SetupData = setupData
		}, m_StorageQuery, inputDeps);
	}
}
