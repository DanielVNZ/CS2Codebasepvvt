using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Economy;
using Game.Objects;
using Game.Prefabs;
using Game.Routes;
using Game.Serialization;
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
public class StorageCompanySystem : GameSystemBase, IPostDeserialize
{
	[BurstCompile]
	private struct StorageJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		[ReadOnly]
		public BufferTypeHandle<Resources> m_CompanyResourceType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		[ReadOnly]
		public ComponentTypeHandle<PropertyRenter> m_PropertyRenterType;

		public BufferTypeHandle<TradeCost> m_TradeCostType;

		[ReadOnly]
		public ComponentLookup<StorageLimitData> m_Limits;

		[ReadOnly]
		public ComponentLookup<StorageCompanyData> m_StorageCompanyDatas;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> m_SpawnableBuildingDatas;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_BuildingDatas;

		[ReadOnly]
		public ComponentLookup<Game.Companies.StorageCompany> m_StorageCompanies;

		[ReadOnly]
		public BufferLookup<OwnedVehicle> m_OwnedVehicles;

		public BufferLookup<StorageTransferRequest> m_StorageTransferRequests;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.DeliveryTruck> m_Trucks;

		[ReadOnly]
		public ComponentLookup<Target> m_Targets;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_LayoutElements;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> m_OutsideConnections;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenters;

		[ReadOnly]
		public BufferLookup<GuestVehicle> m_GuestVehicles;

		public ParallelWriter m_CommandBuffer;

		public uint m_UpdateFrameIndex;

		[ReadOnly]
		public DeliveryTruckSelectData m_DeliveryTruckSelectData;

		public uint m_SimulationFrame;

		public RandomSeed m_RandomSeed;

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
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index != m_UpdateFrameIndex)
			{
				return;
			}
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabType);
			NativeArray<PropertyRenter> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PropertyRenter>(ref m_PropertyRenterType);
			BufferAccessor<Resources> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Resources>(ref m_CompanyResourceType);
			BufferAccessor<TradeCost> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TradeCost>(ref m_TradeCostType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity val = nativeArray[i];
				if (m_Prefabs.HasComponent(nativeArray3[i].m_Property))
				{
					DynamicBuffer<Resources> resourceBuffer = bufferAccessor[i];
					DynamicBuffer<TradeCost> tradeCosts = bufferAccessor2[i];
					Entity prefab = nativeArray2[i].m_Prefab;
					StorageLimitData limitData = m_Limits[prefab];
					StorageCompanyData storageCompanyData = m_StorageCompanyDatas[prefab];
					Entity prefab2 = m_Prefabs[nativeArray3[i].m_Property].m_Prefab;
					SpawnableBuildingData spawnableData = (m_SpawnableBuildingDatas.HasComponent(prefab2) ? m_SpawnableBuildingDatas[prefab2] : new SpawnableBuildingData
					{
						m_Level = 1
					});
					BuildingData buildingData = (m_BuildingDatas.HasComponent(prefab2) ? m_BuildingDatas[prefab2] : new BuildingData
					{
						m_LotSize = new int2(1, 1)
					});
					if (!m_GuestVehicles.HasBuffer(val))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<GuestVehicle>(unfilteredChunkIndex, val);
					}
					ProcessStorage(unfilteredChunkIndex, val, nativeArray3[i].m_Property, storageCompanyData.m_StoredResources, storageCompanyData, resourceBuffer, m_StorageTransferRequests[val], limitData, spawnableData, buildingData, m_DeliveryTruckSelectData, m_SimulationFrame, tradeCosts, m_CommandBuffer, station: false, hasConnectedRoute: false, 0, ref random, ref m_StorageCompanies, ref m_OwnedVehicles, ref m_StorageTransferRequests, ref m_Trucks, ref m_Targets, ref m_LayoutElements, ref m_PropertyRenters, ref m_OutsideConnections);
				}
				else
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, val);
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct StationStorageJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public BufferTypeHandle<Resources> m_CompanyResourceType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> m_InstalledUpgradeType;

		public BufferTypeHandle<TradeCost> m_TradeCostType;

		[ReadOnly]
		public ComponentLookup<StorageLimitData> m_Limits;

		[ReadOnly]
		public ComponentLookup<StorageCompanyData> m_StorageCompanyDatas;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<Game.Companies.StorageCompany> m_StorageCompanies;

		[ReadOnly]
		public BufferLookup<OwnedVehicle> m_OwnedVehicles;

		public BufferLookup<StorageTransferRequest> m_StorageTransferRequests;

		public BufferLookup<TripNeeded> m_TripNeededsBuffers;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.DeliveryTruck> m_Trucks;

		[ReadOnly]
		public ComponentLookup<Target> m_Targets;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_LayoutElements;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjectBuffers;

		[ReadOnly]
		public ComponentLookup<TransportLineData> m_TransportLineData;

		[ReadOnly]
		public BufferLookup<ConnectedRoute> m_ConnectedRouteBuffers;

		[ReadOnly]
		public ComponentLookup<Owner> m_Owners;

		[ReadOnly]
		public BufferLookup<RouteVehicle> m_RouteVehicles;

		[ReadOnly]
		public BufferLookup<Resources> m_ResourceBuffers;

		[ReadOnly]
		public ComponentLookup<Connected> m_Connecteds;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> m_OutsideConnections;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenters;

		[ReadOnly]
		public ComponentLookup<Building> m_Buildings;

		public ParallelWriter m_CommandBuffer;

		[ReadOnly]
		public DeliveryTruckSelectData m_DeliveryTruckSelectData;

		public uint m_SimulationFrame;

		public RandomSeed m_RandomSeed;

		public int m_UpdateInterval;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
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
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabType);
			BufferAccessor<Resources> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Resources>(ref m_CompanyResourceType);
			BufferAccessor<TradeCost> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TradeCost>(ref m_TradeCostType);
			BufferAccessor<InstalledUpgrade> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<InstalledUpgrade>(ref m_InstalledUpgradeType);
			Resource resource = EconomyUtils.GetResource((int)(m_SimulationFrame / m_UpdateInterval) % EconomyUtils.ResourceCount);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				int incomingAmount = 0;
				Entity val = nativeArray[i];
				DynamicBuffer<Resources> resourceBuffer = bufferAccessor[i];
				Entity prefab = nativeArray2[i].m_Prefab;
				StorageLimitData data = m_Limits[prefab];
				StorageCompanyData data2 = m_StorageCompanyDatas[prefab];
				DynamicBuffer<TradeCost> tradeCosts = bufferAccessor2[i];
				if (bufferAccessor3.Length != 0)
				{
					UpgradeUtils.CombineStats<StorageLimitData>(ref data, bufferAccessor3[i], ref m_PrefabRefData, ref m_Limits);
					UpgradeUtils.CombineStats<StorageCompanyData>(ref data2, bufferAccessor3[i], ref m_PrefabRefData, ref m_StorageCompanyDatas);
				}
				bool hasConnectedRoute = false;
				CheckConnectedRoute(val, resource, ref hasConnectedRoute, ref incomingAmount);
				if (m_Buildings.HasComponent(val) && BuildingUtils.CheckOption(m_Buildings[val], BuildingOption.Inactive))
				{
					if (m_TripNeededsBuffers.HasBuffer(val))
					{
						m_TripNeededsBuffers[val].Clear();
					}
					if (m_StorageTransferRequests.HasBuffer(val))
					{
						m_StorageTransferRequests[val].Clear();
					}
				}
				else
				{
					ProcessStorage(unfilteredChunkIndex, val, val, resource, data2, resourceBuffer, m_StorageTransferRequests[val], data, new SpawnableBuildingData
					{
						m_Level = 1
					}, new BuildingData
					{
						m_LotSize = new int2(1, 1)
					}, m_DeliveryTruckSelectData, m_SimulationFrame, tradeCosts, m_CommandBuffer, station: true, hasConnectedRoute, incomingAmount, ref random, ref m_StorageCompanies, ref m_OwnedVehicles, ref m_StorageTransferRequests, ref m_Trucks, ref m_Targets, ref m_LayoutElements, ref m_PropertyRenters, ref m_OutsideConnections);
				}
			}
		}

		private void CheckConnectedRoute(Entity entity, Resource resource, ref bool hasConnectedRoute, ref int incomingAmount)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<ConnectedRoute> val = default(DynamicBuffer<ConnectedRoute>);
			if (m_ConnectedRouteBuffers.TryGetBuffer(entity, ref val))
			{
				for (int i = 0; i < val.Length; i++)
				{
					Entity waypoint = val[i].m_Waypoint;
					if (!m_Owners.HasComponent(waypoint))
					{
						continue;
					}
					Entity owner = m_Owners[waypoint].m_Owner;
					if (!m_RouteVehicles.HasBuffer(owner) || !m_PrefabRefData.HasComponent(owner) || !m_TransportLineData.HasComponent(m_PrefabRefData[owner].m_Prefab) || !m_TransportLineData[m_PrefabRefData[owner].m_Prefab].m_CargoTransport)
					{
						continue;
					}
					hasConnectedRoute = true;
					DynamicBuffer<RouteVehicle> val2 = m_RouteVehicles[owner];
					for (int j = 0; j < val2.Length; j++)
					{
						Entity vehicle = val2[j].m_Vehicle;
						if (!m_Targets.HasComponent(vehicle) || !m_ResourceBuffers.HasBuffer(vehicle))
						{
							continue;
						}
						Entity val3 = m_Targets[vehicle].m_Target;
						if (m_Connecteds.HasComponent(val3))
						{
							val3 = m_Connecteds[val3].m_Connected;
						}
						if (m_Owners.HasComponent(val3))
						{
							val3 = m_Owners[val3].m_Owner;
						}
						if (!(val3 == entity))
						{
							continue;
						}
						DynamicBuffer<Resources> resources = m_ResourceBuffers[vehicle];
						incomingAmount += EconomyUtils.GetResources(resource, resources);
						if (!m_LayoutElements.HasBuffer(vehicle))
						{
							continue;
						}
						DynamicBuffer<LayoutElement> val4 = m_LayoutElements[vehicle];
						for (int k = 0; k < val4.Length; k++)
						{
							Entity vehicle2 = val4[k].m_Vehicle;
							if (m_ResourceBuffers.HasBuffer(vehicle2))
							{
								resources = m_ResourceBuffers[vehicle2];
								incomingAmount += EconomyUtils.GetResources(resource, resources);
							}
						}
					}
				}
			}
			DynamicBuffer<Game.Objects.SubObject> val5 = default(DynamicBuffer<Game.Objects.SubObject>);
			if (m_SubObjectBuffers.TryGetBuffer(entity, ref val5))
			{
				for (int l = 0; l < val5.Length; l++)
				{
					CheckConnectedRoute(val5[l].m_SubObject, resource, ref hasConnectedRoute, ref incomingAmount);
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct OCStationStorageJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public BufferTypeHandle<Resources> m_CompanyResourceType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		public BufferTypeHandle<TradeCost> m_TradeCostType;

		[ReadOnly]
		public ComponentLookup<StorageLimitData> m_Limits;

		[ReadOnly]
		public ComponentLookup<StorageCompanyData> m_StorageCompanyDatas;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<Game.Companies.StorageCompany> m_StorageCompanies;

		public BufferLookup<StorageTransferRequest> m_StorageTransferRequests;

		[ReadOnly]
		public ComponentLookup<Target> m_Targets;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_LayoutElements;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjectBuffers;

		[ReadOnly]
		public ComponentLookup<TransportLineData> m_TransportLineData;

		[ReadOnly]
		public BufferLookup<ConnectedRoute> m_ConnectedRouteBuffers;

		[ReadOnly]
		public ComponentLookup<Owner> m_Owners;

		[ReadOnly]
		public BufferLookup<RouteVehicle> m_RouteVehicles;

		[ReadOnly]
		public BufferLookup<Resources> m_ResourceBuffers;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> m_OutsideConnections;

		[ReadOnly]
		public ComponentLookup<Connected> m_Connecteds;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> m_RouteWaypoints;

		public ParallelWriter m_CommandBuffer;

		public uint m_SimulationFrame;

		public RandomSeed m_RandomSeed;

		public int m_UpdateInterval;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabType);
			BufferAccessor<Resources> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Resources>(ref m_CompanyResourceType);
			BufferAccessor<TradeCost> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TradeCost>(ref m_TradeCostType);
			Resource resource = EconomyUtils.GetResource((int)(m_SimulationFrame / m_UpdateInterval) % EconomyUtils.ResourceCount);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				int incomingAmount = 0;
				Entity val = nativeArray[i];
				DynamicBuffer<Resources> resourceBuffer = bufferAccessor[i];
				Entity prefab = nativeArray2[i].m_Prefab;
				StorageLimitData limitData = m_Limits[prefab];
				StorageCompanyData storageCompanyData = m_StorageCompanyDatas[prefab];
				DynamicBuffer<TradeCost> tradeCosts = bufferAccessor2[i];
				bool hasConnectedRoute = false;
				CheckConnectedRoute(val, resource, ref hasConnectedRoute, ref incomingAmount);
				if (hasConnectedRoute)
				{
					bool flag = CheckIfOCIsBeforeOrAfterStation(checkBefore: true, m_ConnectedRouteBuffers[val], ref m_RouteWaypoints, ref m_Connecteds, ref m_OutsideConnections);
					bool flag2 = CheckIfOCIsBeforeOrAfterStation(checkBefore: false, m_ConnectedRouteBuffers[val], ref m_RouteWaypoints, ref m_Connecteds, ref m_OutsideConnections);
					if (!flag || !flag2)
					{
						OCProcessStorage(unfilteredChunkIndex, flag, flag2, val, val, resource, storageCompanyData, resourceBuffer, m_StorageTransferRequests[val], limitData, m_SimulationFrame, tradeCosts, m_CommandBuffer, incomingAmount, ref random, ref m_StorageCompanies, ref m_StorageTransferRequests);
					}
				}
			}
		}

		private bool OCProcessStorage(int chunkIndex, bool isBeforeStation, bool isAfterStation, Entity company, Entity building, Resource resource, StorageCompanyData storageCompanyData, DynamicBuffer<Resources> resourceBuffer, DynamicBuffer<StorageTransferRequest> requests, StorageLimitData limitData, uint simulationFrame, DynamicBuffer<TradeCost> tradeCosts, ParallelWriter commandBuffer, int incomingAmount, ref Random random, ref ComponentLookup<Game.Companies.StorageCompany> storageCompanies, ref BufferLookup<StorageTransferRequest> storageTransferRequests)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0398: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			int num = EconomyUtils.CountResources(storageCompanyData.m_StoredResources);
			int num2 = limitData.m_Limit / 2 / num;
			if ((storageCompanyData.m_StoredResources & resource) != Resource.NoResource)
			{
				int resources = EconomyUtils.GetResources(resource, resourceBuffer);
				int num3 = resources;
				for (int i = 0; i < requests.Length; i++)
				{
					StorageTransferRequest storageTransferRequest = requests[i];
					if (storageTransferRequest.m_Resource != resource)
					{
						continue;
					}
					if (!storageCompanies.HasComponent(storageTransferRequest.m_Target) || !storageTransferRequests.HasBuffer(storageTransferRequest.m_Target))
					{
						requests.RemoveAtSwapBack(i);
						i--;
						continue;
					}
					bool flag2 = (storageTransferRequest.m_Flags & StorageTransferFlags.Incoming) != 0;
					if (flag2)
					{
						int num4 = 0;
						DynamicBuffer<StorageTransferRequest> val = storageTransferRequests[storageTransferRequest.m_Target];
						for (int j = 0; j < val.Length; j++)
						{
							StorageTransferRequest storageTransferRequest2 = val[j];
							if ((storageTransferRequest2.m_Target == company || storageTransferRequest2.m_Target == building) && storageTransferRequest2.m_Resource == resource && (storageTransferRequest2.m_Flags & StorageTransferFlags.Incoming) == 0)
							{
								num4 += storageTransferRequest2.m_Amount;
							}
						}
						int num5 = 0;
						if (num4 < storageTransferRequest.m_Amount && incomingAmount > 0)
						{
							int num6 = math.min(storageTransferRequest.m_Amount - num4, incomingAmount);
							num5 += num6;
							incomingAmount -= num6;
						}
						if (num5 + num4 == 0)
						{
							requests.RemoveAtSwapBack(i);
							i--;
							continue;
						}
						if (num5 + num4 < storageTransferRequest.m_Amount)
						{
							storageTransferRequest.m_Amount = num5 + num4;
							requests[i] = storageTransferRequest;
						}
					}
					else
					{
						int num7 = 0;
						DynamicBuffer<StorageTransferRequest> val2 = storageTransferRequests[storageTransferRequest.m_Target];
						for (int k = 0; k < val2.Length; k++)
						{
							StorageTransferRequest storageTransferRequest3 = val2[k];
							if ((storageTransferRequest3.m_Target == company || storageTransferRequest3.m_Target == building) && storageTransferRequest3.m_Resource == resource && (storageTransferRequest3.m_Flags & StorageTransferFlags.Incoming) != 0)
							{
								num7 = storageTransferRequest3.m_Amount;
								break;
							}
						}
						if (num7 == 0)
						{
							requests.RemoveAtSwapBack(i);
							i--;
							continue;
						}
						if (num7 < storageTransferRequest.m_Amount)
						{
							storageTransferRequest.m_Amount = num7;
							requests[i] = storageTransferRequest;
						}
					}
					num3 += (flag2 ? storageTransferRequest.m_Amount : (-storageTransferRequest.m_Amount));
				}
				TradeCost tradeCost = EconomyUtils.GetTradeCost(resource, tradeCosts);
				long lastTradeRequestTime = EconomyUtils.GetLastTradeRequestTime(tradeCosts);
				if (tradeCost.m_LastTransferRequestTime == 0L)
				{
					tradeCost.m_LastTransferRequestTime = simulationFrame - kTransferCooldown / 2;
					EconomyUtils.SetTradeCost(resource, tradeCost, tradeCosts, keepLastTime: false);
				}
				if (simulationFrame - lastTradeRequestTime >= kTransferCooldown + ((Random)(ref random)).NextInt(storageCompanyData.m_TransportInterval.x, storageCompanyData.m_TransportInterval.y) || tradeCost.m_LastTransferRequestTime == 0L)
				{
					if (resources > num2 && num3 > num2 && isBeforeStation)
					{
						int num8 = resources - num2;
						num8 = math.max(num8, kStationMinimalTransferAmount);
						((ParallelWriter)(ref commandBuffer)).AddComponent<StorageTransfer>(chunkIndex, company, new StorageTransfer
						{
							m_Resource = resource,
							m_Amount = num8
						});
						tradeCost.m_LastTransferRequestTime = simulationFrame;
						EconomyUtils.SetTradeCost(resource, tradeCost, tradeCosts, keepLastTime: false);
						flag = true;
					}
					else if (resources < num2 && num3 < num2 && isAfterStation)
					{
						StorageTransfer storageTransfer = new StorageTransfer
						{
							m_Resource = resource
						};
						int num9 = num2 - resources;
						num9 = math.max(num9, kStationMinimalTransferAmount);
						storageTransfer.m_Amount = -num9;
						((ParallelWriter)(ref commandBuffer)).AddComponent<StorageTransfer>(chunkIndex, company, storageTransfer);
						tradeCost.m_LastTransferRequestTime = simulationFrame;
						flag = true;
					}
					if (((Random)(ref random)).NextInt(kCostFadeProbability) == 0)
					{
						tradeCost.m_BuyCost *= 0.99f;
						tradeCost.m_SellCost *= 0.99f;
						if (!flag)
						{
							EconomyUtils.SetTradeCost(resource, tradeCost, tradeCosts, keepLastTime: false);
						}
					}
					if (flag)
					{
						EconomyUtils.SetTradeCost(resource, tradeCost, tradeCosts, keepLastTime: false);
					}
				}
			}
			return flag;
		}

		private void CheckConnectedRoute(Entity entity, Resource resource, ref bool hasConnectedRoute, ref int incomingAmount)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<ConnectedRoute> val = default(DynamicBuffer<ConnectedRoute>);
			if (m_ConnectedRouteBuffers.TryGetBuffer(entity, ref val))
			{
				for (int i = 0; i < val.Length; i++)
				{
					Entity waypoint = val[i].m_Waypoint;
					if (!m_Owners.HasComponent(waypoint))
					{
						continue;
					}
					Entity owner = m_Owners[waypoint].m_Owner;
					if (!m_RouteVehicles.HasBuffer(owner) || !m_PrefabRefData.HasComponent(owner) || !m_TransportLineData.HasComponent(m_PrefabRefData[owner].m_Prefab) || !m_TransportLineData[m_PrefabRefData[owner].m_Prefab].m_CargoTransport)
					{
						continue;
					}
					hasConnectedRoute = true;
					DynamicBuffer<RouteVehicle> val2 = m_RouteVehicles[owner];
					for (int j = 0; j < val2.Length; j++)
					{
						Entity vehicle = val2[j].m_Vehicle;
						if (!m_Targets.HasComponent(vehicle) || !m_ResourceBuffers.HasBuffer(vehicle))
						{
							continue;
						}
						Entity val3 = m_Targets[vehicle].m_Target;
						if (m_Connecteds.HasComponent(val3))
						{
							val3 = m_Connecteds[val3].m_Connected;
						}
						if (m_Owners.HasComponent(val3))
						{
							val3 = m_Owners[val3].m_Owner;
						}
						if (!(val3 == entity))
						{
							continue;
						}
						DynamicBuffer<Resources> resources = m_ResourceBuffers[vehicle];
						incomingAmount += EconomyUtils.GetResources(resource, resources);
						if (!m_LayoutElements.HasBuffer(vehicle))
						{
							continue;
						}
						DynamicBuffer<LayoutElement> val4 = m_LayoutElements[vehicle];
						for (int k = 0; k < val4.Length; k++)
						{
							Entity vehicle2 = val4[k].m_Vehicle;
							if (m_ResourceBuffers.HasBuffer(vehicle2))
							{
								resources = m_ResourceBuffers[vehicle2];
								incomingAmount += EconomyUtils.GetResources(resource, resources);
							}
						}
					}
				}
			}
			DynamicBuffer<Game.Objects.SubObject> val5 = default(DynamicBuffer<Game.Objects.SubObject>);
			if (m_SubObjectBuffers.TryGetBuffer(entity, ref val5))
			{
				for (int l = 0; l < val5.Length; l++)
				{
					CheckConnectedRoute(val5[l].m_SubObject, resource, ref hasConnectedRoute, ref incomingAmount);
				}
			}
		}

		private bool CheckIfOCIsBeforeOrAfterStation(bool checkBefore, DynamicBuffer<ConnectedRoute> connectedRoutes, ref BufferLookup<RouteWaypoint> routeWaypoints, ref ComponentLookup<Connected> connects, ref ComponentLookup<Game.Objects.OutsideConnection> outsideConnections)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			if (connectedRoutes.Length == 0)
			{
				return false;
			}
			Entity waypoint = connectedRoutes[0].m_Waypoint;
			if (!m_Owners.HasComponent(waypoint))
			{
				return false;
			}
			Entity owner = m_Owners[waypoint].m_Owner;
			DynamicBuffer<RouteWaypoint> val = default(DynamicBuffer<RouteWaypoint>);
			if (!routeWaypoints.TryGetBuffer(owner, ref val))
			{
				return false;
			}
			Connected connected = default(Connected);
			for (int i = 0; i < val.Length; i++)
			{
				if (!(val[i].m_Waypoint != waypoint))
				{
					int num = ((!checkBefore) ? 1 : (-1));
					int num2 = (i + num + val.Length) % val.Length;
					Entity waypoint2 = val[num2].m_Waypoint;
					if (!connects.TryGetComponent(waypoint2, ref connected))
					{
						return false;
					}
					return !outsideConnections.HasComponent(connected.m_Connected);
				}
			}
			return false;
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

		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		public BufferTypeHandle<TradeCost> __Game_Companies_TradeCost_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<StorageLimitData> __Game_Companies_StorageLimitData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StorageCompanyData> __Game_Prefabs_StorageCompanyData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<OwnedVehicle> __Game_Vehicles_OwnedVehicle_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Game.Companies.StorageCompany> __Game_Companies_StorageCompany_RO_ComponentLookup;

		public BufferLookup<StorageTransferRequest> __Game_Companies_StorageTransferRequest_RW_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Target> __Game_Common_Target_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.DeliveryTruck> __Game_Vehicles_DeliveryTruck_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> __Game_Objects_OutsideConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<GuestVehicle> __Game_Vehicles_GuestVehicle_RO_BufferLookup;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle;

		public BufferLookup<TripNeeded> __Game_Citizens_TripNeeded_RW_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedRoute> __Game_Routes_ConnectedRoute_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Resources> __Game_Economy_Resources_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<RouteVehicle> __Game_Routes_RouteVehicle_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<TransportLineData> __Game_Prefabs_TransportLineData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Connected> __Game_Routes_Connected_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> __Game_Routes_RouteWaypoint_RO_BufferLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Economy_Resources_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Resources>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Companies_TradeCost_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<TradeCost>(false);
			__Game_Buildings_PropertyRenter_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PropertyRenter>(true);
			__Game_Companies_StorageLimitData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StorageLimitData>(true);
			__Game_Prefabs_StorageCompanyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StorageCompanyData>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableBuildingData>(true);
			__Game_Vehicles_OwnedVehicle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<OwnedVehicle>(true);
			__Game_Companies_StorageCompany_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Companies.StorageCompany>(true);
			__Game_Companies_StorageTransferRequest_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<StorageTransferRequest>(false);
			__Game_Common_Target_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Target>(true);
			__Game_Vehicles_DeliveryTruck_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.DeliveryTruck>(true);
			__Game_Vehicles_LayoutElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(true);
			__Game_Objects_OutsideConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.OutsideConnection>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
			__Game_Vehicles_GuestVehicle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<GuestVehicle>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<InstalledUpgrade>(true);
			__Game_Citizens_TripNeeded_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TripNeeded>(false);
			__Game_Routes_ConnectedRoute_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedRoute>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Economy_Resources_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Resources>(true);
			__Game_Routes_RouteVehicle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteVehicle>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
			__Game_Prefabs_TransportLineData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TransportLineData>(true);
			__Game_Routes_Connected_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Connected>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Routes_RouteWaypoint_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteWaypoint>(true);
		}
	}

	private static readonly int kTransferCooldown = 400;

	private static readonly int kCostFadeProbability = 256;

	private static readonly float kMaxTransportUnitCost = 0.03f;

	public static readonly int kStorageLowStockAmount = 25000;

	public static readonly int kStationLowStockAmount = 100000;

	public static readonly int kStorageExportStartAmount = 100000;

	public static readonly int kStationExportStartAmount = 200000;

	private static readonly int kStorageMinimalTransferAmount = 10000;

	private static readonly int kStationMinimalTransferAmount = 30000;

	private SimulationSystem m_SimulationSystem;

	private VehicleCapacitySystem m_VehicleCapacitySystem;

	private EntityQuery m_CompanyGroup;

	private EntityQuery m_StationGroup;

	private EntityQuery m_OCStationGroup;

	private EndFrameBarrier m_EndFrameBarrier;

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
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_VehicleCapacitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<VehicleCapacitySystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_CompanyGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[8]
		{
			ComponentType.ReadOnly<Game.Companies.StorageCompany>(),
			ComponentType.ReadOnly<PropertyRenter>(),
			ComponentType.ReadWrite<Resources>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.ReadOnly<CompanyData>(),
			ComponentType.Exclude<StorageTransfer>(),
			ComponentType.Exclude<Deleted>()
		});
		m_StationGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[8]
		{
			ComponentType.ReadOnly<Game.Companies.StorageCompany>(),
			ComponentType.ReadOnly<CityServiceUpkeep>(),
			ComponentType.ReadWrite<Resources>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<CompanyData>(),
			ComponentType.Exclude<StorageTransfer>(),
			ComponentType.Exclude<Game.Objects.OutsideConnection>(),
			ComponentType.Exclude<Deleted>()
		});
		m_OCStationGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[8]
		{
			ComponentType.ReadOnly<Game.Companies.StorageCompany>(),
			ComponentType.ReadOnly<Game.Objects.OutsideConnection>(),
			ComponentType.ReadWrite<Resources>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<CompanyData>(),
			ComponentType.Exclude<StorageTransfer>(),
			ComponentType.Exclude<CityServiceUpkeep>(),
			ComponentType.Exclude<Deleted>()
		});
		((ComponentSystemBase)this).RequireAnyForUpdate((EntityQuery[])(object)new EntityQuery[3] { m_CompanyGroup, m_StationGroup, m_OCStationGroup });
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
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0573: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_0590: Unknown result type (might be due to invalid IL or missing references)
		//IL_0595: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0614: Unknown result type (might be due to invalid IL or missing references)
		//IL_0619: Unknown result type (might be due to invalid IL or missing references)
		//IL_061b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0620: Unknown result type (might be due to invalid IL or missing references)
		//IL_0625: Unknown result type (might be due to invalid IL or missing references)
		//IL_062a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0631: Unknown result type (might be due to invalid IL or missing references)
		//IL_0652: Unknown result type (might be due to invalid IL or missing references)
		//IL_0657: Unknown result type (might be due to invalid IL or missing references)
		//IL_066f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0674: Unknown result type (might be due to invalid IL or missing references)
		//IL_068c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0691: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0700: Unknown result type (might be due to invalid IL or missing references)
		//IL_0705: Unknown result type (might be due to invalid IL or missing references)
		//IL_071d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0722: Unknown result type (might be due to invalid IL or missing references)
		//IL_073a: Unknown result type (might be due to invalid IL or missing references)
		//IL_073f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0757: Unknown result type (might be due to invalid IL or missing references)
		//IL_075c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0774: Unknown result type (might be due to invalid IL or missing references)
		//IL_0779: Unknown result type (might be due to invalid IL or missing references)
		//IL_0791: Unknown result type (might be due to invalid IL or missing references)
		//IL_0796: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0805: Unknown result type (might be due to invalid IL or missing references)
		//IL_080a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0822: Unknown result type (might be due to invalid IL or missing references)
		//IL_0827: Unknown result type (might be due to invalid IL or missing references)
		//IL_083f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0844: Unknown result type (might be due to invalid IL or missing references)
		//IL_085c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0861: Unknown result type (might be due to invalid IL or missing references)
		//IL_0879: Unknown result type (might be due to invalid IL or missing references)
		//IL_087e: Unknown result type (might be due to invalid IL or missing references)
		//IL_088b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0890: Unknown result type (might be due to invalid IL or missing references)
		//IL_0894: Unknown result type (might be due to invalid IL or missing references)
		//IL_0899: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08da: Unknown result type (might be due to invalid IL or missing references)
		//IL_08df: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f5: Unknown result type (might be due to invalid IL or missing references)
		uint updateFrameWithInterval = SimulationUtils.GetUpdateFrameWithInterval(m_SimulationSystem.frameIndex, (uint)GetUpdateInterval(SystemUpdatePhase.GameSimulation), 16);
		StorageJob storageJob = new StorageJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CompanyResourceType = InternalCompilerInterface.GetBufferTypeHandle<Resources>(ref __TypeHandle.__Game_Economy_Resources_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TradeCostType = InternalCompilerInterface.GetBufferTypeHandle<TradeCost>(ref __TypeHandle.__Game_Companies_TradeCost_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyRenterType = InternalCompilerInterface.GetComponentTypeHandle<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Limits = InternalCompilerInterface.GetComponentLookup<StorageLimitData>(ref __TypeHandle.__Game_Companies_StorageLimitData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StorageCompanyDatas = InternalCompilerInterface.GetComponentLookup<StorageCompanyData>(ref __TypeHandle.__Game_Prefabs_StorageCompanyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingDatas = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnableBuildingDatas = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnedVehicles = InternalCompilerInterface.GetBufferLookup<OwnedVehicle>(ref __TypeHandle.__Game_Vehicles_OwnedVehicle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StorageCompanies = InternalCompilerInterface.GetComponentLookup<Game.Companies.StorageCompany>(ref __TypeHandle.__Game_Companies_StorageCompany_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StorageTransferRequests = InternalCompilerInterface.GetBufferLookup<StorageTransferRequest>(ref __TypeHandle.__Game_Companies_StorageTransferRequest_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Targets = InternalCompilerInterface.GetComponentLookup<Target>(ref __TypeHandle.__Game_Common_Target_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Trucks = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.DeliveryTruck>(ref __TypeHandle.__Game_Vehicles_DeliveryTruck_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LayoutElements = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OutsideConnections = InternalCompilerInterface.GetComponentLookup<Game.Objects.OutsideConnection>(ref __TypeHandle.__Game_Objects_OutsideConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyRenters = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GuestVehicles = InternalCompilerInterface.GetBufferLookup<GuestVehicle>(ref __TypeHandle.__Game_Vehicles_GuestVehicle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		storageJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		storageJob.m_UpdateFrameIndex = updateFrameWithInterval;
		storageJob.m_DeliveryTruckSelectData = m_VehicleCapacitySystem.GetDeliveryTruckSelectData();
		storageJob.m_SimulationFrame = m_SimulationSystem.frameIndex;
		storageJob.m_RandomSeed = RandomSeed.Next();
		JobHandle val2 = JobChunkExtensions.Schedule<StorageJob>(storageJob, m_CompanyGroup, ((SystemBase)this).Dependency);
		m_EndFrameBarrier.AddJobHandleForProducer(val2);
		StationStorageJob stationStorageJob = new StationStorageJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CompanyResourceType = InternalCompilerInterface.GetBufferTypeHandle<Resources>(ref __TypeHandle.__Game_Economy_Resources_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgradeType = InternalCompilerInterface.GetBufferTypeHandle<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TradeCostType = InternalCompilerInterface.GetBufferTypeHandle<TradeCost>(ref __TypeHandle.__Game_Companies_TradeCost_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Limits = InternalCompilerInterface.GetComponentLookup<StorageLimitData>(ref __TypeHandle.__Game_Companies_StorageLimitData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StorageCompanyDatas = InternalCompilerInterface.GetComponentLookup<StorageCompanyData>(ref __TypeHandle.__Game_Prefabs_StorageCompanyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnedVehicles = InternalCompilerInterface.GetBufferLookup<OwnedVehicle>(ref __TypeHandle.__Game_Vehicles_OwnedVehicle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StorageCompanies = InternalCompilerInterface.GetComponentLookup<Game.Companies.StorageCompany>(ref __TypeHandle.__Game_Companies_StorageCompany_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StorageTransferRequests = InternalCompilerInterface.GetBufferLookup<StorageTransferRequest>(ref __TypeHandle.__Game_Companies_StorageTransferRequest_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TripNeededsBuffers = InternalCompilerInterface.GetBufferLookup<TripNeeded>(ref __TypeHandle.__Game_Citizens_TripNeeded_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Targets = InternalCompilerInterface.GetComponentLookup<Target>(ref __TypeHandle.__Game_Common_Target_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Trucks = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.DeliveryTruck>(ref __TypeHandle.__Game_Vehicles_DeliveryTruck_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LayoutElements = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedRouteBuffers = InternalCompilerInterface.GetBufferLookup<ConnectedRoute>(ref __TypeHandle.__Game_Routes_ConnectedRoute_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Owners = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceBuffers = InternalCompilerInterface.GetBufferLookup<Resources>(ref __TypeHandle.__Game_Economy_Resources_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteVehicles = InternalCompilerInterface.GetBufferLookup<RouteVehicle>(ref __TypeHandle.__Game_Routes_RouteVehicle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjectBuffers = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransportLineData = InternalCompilerInterface.GetComponentLookup<TransportLineData>(ref __TypeHandle.__Game_Prefabs_TransportLineData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Connecteds = InternalCompilerInterface.GetComponentLookup<Connected>(ref __TypeHandle.__Game_Routes_Connected_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OutsideConnections = InternalCompilerInterface.GetComponentLookup<Game.Objects.OutsideConnection>(ref __TypeHandle.__Game_Objects_OutsideConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyRenters = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Buildings = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		val = m_EndFrameBarrier.CreateCommandBuffer();
		stationStorageJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		stationStorageJob.m_DeliveryTruckSelectData = m_VehicleCapacitySystem.GetDeliveryTruckSelectData();
		stationStorageJob.m_SimulationFrame = m_SimulationSystem.frameIndex;
		stationStorageJob.m_RandomSeed = RandomSeed.Next();
		stationStorageJob.m_UpdateInterval = GetUpdateInterval(SystemUpdatePhase.GameSimulation);
		JobHandle val3 = JobChunkExtensions.Schedule<StationStorageJob>(stationStorageJob, m_StationGroup, JobHandle.CombineDependencies(val2, ((SystemBase)this).Dependency));
		m_EndFrameBarrier.AddJobHandleForProducer(val3);
		OCStationStorageJob oCStationStorageJob = new OCStationStorageJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CompanyResourceType = InternalCompilerInterface.GetBufferTypeHandle<Resources>(ref __TypeHandle.__Game_Economy_Resources_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TradeCostType = InternalCompilerInterface.GetBufferTypeHandle<TradeCost>(ref __TypeHandle.__Game_Companies_TradeCost_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Limits = InternalCompilerInterface.GetComponentLookup<StorageLimitData>(ref __TypeHandle.__Game_Companies_StorageLimitData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StorageCompanyDatas = InternalCompilerInterface.GetComponentLookup<StorageCompanyData>(ref __TypeHandle.__Game_Prefabs_StorageCompanyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StorageCompanies = InternalCompilerInterface.GetComponentLookup<Game.Companies.StorageCompany>(ref __TypeHandle.__Game_Companies_StorageCompany_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StorageTransferRequests = InternalCompilerInterface.GetBufferLookup<StorageTransferRequest>(ref __TypeHandle.__Game_Companies_StorageTransferRequest_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Targets = InternalCompilerInterface.GetComponentLookup<Target>(ref __TypeHandle.__Game_Common_Target_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LayoutElements = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjectBuffers = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransportLineData = InternalCompilerInterface.GetComponentLookup<TransportLineData>(ref __TypeHandle.__Game_Prefabs_TransportLineData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedRouteBuffers = InternalCompilerInterface.GetBufferLookup<ConnectedRoute>(ref __TypeHandle.__Game_Routes_ConnectedRoute_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Owners = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteVehicles = InternalCompilerInterface.GetBufferLookup<RouteVehicle>(ref __TypeHandle.__Game_Routes_RouteVehicle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceBuffers = InternalCompilerInterface.GetBufferLookup<Resources>(ref __TypeHandle.__Game_Economy_Resources_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OutsideConnections = InternalCompilerInterface.GetComponentLookup<Game.Objects.OutsideConnection>(ref __TypeHandle.__Game_Objects_OutsideConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Connecteds = InternalCompilerInterface.GetComponentLookup<Connected>(ref __TypeHandle.__Game_Routes_Connected_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteWaypoints = InternalCompilerInterface.GetBufferLookup<RouteWaypoint>(ref __TypeHandle.__Game_Routes_RouteWaypoint_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		val = m_EndFrameBarrier.CreateCommandBuffer();
		oCStationStorageJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		oCStationStorageJob.m_SimulationFrame = m_SimulationSystem.frameIndex;
		oCStationStorageJob.m_RandomSeed = RandomSeed.Next();
		oCStationStorageJob.m_UpdateInterval = GetUpdateInterval(SystemUpdatePhase.GameSimulation);
		JobHandle val4 = JobChunkExtensions.Schedule<OCStationStorageJob>(oCStationStorageJob, m_OCStationGroup, JobHandle.CombineDependencies(val3, ((SystemBase)this).Dependency));
		m_EndFrameBarrier.AddJobHandleForProducer(val4);
		((SystemBase)this).Dependency = JobHandle.CombineDependencies(val2, val3, val4);
	}

	private static bool RemoveFromRequests(Resource resource, int amount, Entity owner, Entity target1, Entity target2, ref BufferLookup<StorageTransferRequest> storageTransferRequests)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<StorageTransferRequest> val = storageTransferRequests[owner];
		for (int i = 0; i < val.Length; i++)
		{
			StorageTransferRequest storageTransferRequest = val[i];
			if ((storageTransferRequest.m_Target == target1 || storageTransferRequest.m_Target == target2) && storageTransferRequest.m_Resource == resource && (storageTransferRequest.m_Flags & StorageTransferFlags.Incoming) == 0)
			{
				if (storageTransferRequest.m_Amount > amount)
				{
					storageTransferRequest.m_Amount -= amount;
					val[i] = storageTransferRequest;
					return true;
				}
				amount -= storageTransferRequest.m_Amount;
				val.RemoveAtSwapBack(i);
				i--;
			}
		}
		return amount == 0;
	}

	private static bool ProcessStorage(int chunkIndex, Entity company, Entity building, Resource resource, StorageCompanyData storageCompanyData, DynamicBuffer<Resources> resourceBuffer, DynamicBuffer<StorageTransferRequest> requests, StorageLimitData limitData, SpawnableBuildingData spawnableData, BuildingData buildingData, DeliveryTruckSelectData truckSelectData, uint simulationFrame, DynamicBuffer<TradeCost> tradeCosts, ParallelWriter commandBuffer, bool station, bool hasConnectedRoute, int incomingAmount, ref Random random, ref ComponentLookup<Game.Companies.StorageCompany> storageCompanies, ref BufferLookup<OwnedVehicle> ownedVehicles, ref BufferLookup<StorageTransferRequest> storageTransferRequests, ref ComponentLookup<Game.Vehicles.DeliveryTruck> trucks, ref ComponentLookup<Target> targets, ref BufferLookup<LayoutElement> layoutElements, ref ComponentLookup<PropertyRenter> propertyRenters, ref ComponentLookup<Game.Objects.OutsideConnection> outsideConnections)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0669: Unknown result type (might be due to invalid IL or missing references)
		//IL_0651: Unknown result type (might be due to invalid IL or missing references)
		//IL_054a: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		int num = EconomyUtils.CountResources(storageCompanyData.m_StoredResources);
		if (num == 0)
		{
			return false;
		}
		int num2 = limitData.GetAdjustedLimitForWarehouse(spawnableData, buildingData) / num;
		if ((storageCompanyData.m_StoredResources & resource) != Resource.NoResource)
		{
			int resources = EconomyUtils.GetResources(resource, resourceBuffer);
			int num3 = resources;
			for (int i = 0; i < requests.Length; i++)
			{
				StorageTransferRequest storageTransferRequest = requests[i];
				if (storageTransferRequest.m_Resource != resource)
				{
					continue;
				}
				if (!storageCompanies.HasComponent(storageTransferRequest.m_Target) || !storageTransferRequests.HasBuffer(storageTransferRequest.m_Target) || (!propertyRenters.HasComponent(storageTransferRequest.m_Target) && !outsideConnections.HasComponent(storageTransferRequest.m_Target)))
				{
					requests.RemoveAtSwapBack(i);
					i--;
					continue;
				}
				bool flag2 = (storageTransferRequest.m_Flags & StorageTransferFlags.Incoming) != 0;
				if (flag2)
				{
					int num4 = 0;
					DynamicBuffer<StorageTransferRequest> val = storageTransferRequests[storageTransferRequest.m_Target];
					for (int j = 0; j < val.Length; j++)
					{
						StorageTransferRequest storageTransferRequest2 = val[j];
						if ((storageTransferRequest2.m_Target == company || storageTransferRequest2.m_Target == building) && storageTransferRequest2.m_Resource == resource && (storageTransferRequest2.m_Flags & StorageTransferFlags.Incoming) == 0)
						{
							num4 += storageTransferRequest2.m_Amount;
						}
					}
					int num5 = 0;
					if (ownedVehicles.HasBuffer(storageTransferRequest.m_Target))
					{
						DynamicBuffer<OwnedVehicle> val2 = ownedVehicles[storageTransferRequest.m_Target];
						for (int k = 0; k < val2.Length; k++)
						{
							Entity vehicle = val2[k].m_Vehicle;
							if (!trucks.HasComponent(vehicle) || !targets.HasComponent(vehicle))
							{
								continue;
							}
							Game.Vehicles.DeliveryTruck deliveryTruck = trucks[vehicle];
							Entity target = targets[vehicle].m_Target;
							if (!(target == company) && !(target == building))
							{
								continue;
							}
							int num6 = 0;
							if (deliveryTruck.m_Resource == resource)
							{
								num6 += deliveryTruck.m_Amount;
							}
							if (layoutElements.HasBuffer(vehicle))
							{
								DynamicBuffer<LayoutElement> val3 = layoutElements[vehicle];
								for (int l = 0; l < val3.Length; l++)
								{
									Entity vehicle2 = val3[l].m_Vehicle;
									if (trucks.HasComponent(vehicle2))
									{
										deliveryTruck = trucks[vehicle2];
										if (deliveryTruck.m_Resource == resource)
										{
											num6 += deliveryTruck.m_Amount;
										}
									}
								}
							}
							num5 += num6;
						}
					}
					if (station && num4 + num5 < storageTransferRequest.m_Amount && incomingAmount > 0)
					{
						int num7 = math.min(storageTransferRequest.m_Amount - num5 - num4, incomingAmount);
						num5 += num7;
						incomingAmount -= num7;
					}
					if (num5 + num4 == 0)
					{
						requests.RemoveAtSwapBack(i);
						i--;
						continue;
					}
					if (num5 + num4 < storageTransferRequest.m_Amount)
					{
						storageTransferRequest.m_Amount = num5 + num4;
						requests[i] = storageTransferRequest;
					}
				}
				else
				{
					int num8 = 0;
					DynamicBuffer<StorageTransferRequest> val4 = storageTransferRequests[storageTransferRequest.m_Target];
					for (int m = 0; m < val4.Length; m++)
					{
						StorageTransferRequest storageTransferRequest3 = val4[m];
						if ((storageTransferRequest3.m_Target == company || storageTransferRequest3.m_Target == building) && storageTransferRequest3.m_Resource == resource && (storageTransferRequest3.m_Flags & StorageTransferFlags.Incoming) != 0)
						{
							num8 = storageTransferRequest3.m_Amount;
							break;
						}
					}
					if (num8 == 0)
					{
						requests.RemoveAtSwapBack(i);
						i--;
						continue;
					}
					if (num8 < storageTransferRequest.m_Amount)
					{
						storageTransferRequest.m_Amount = num8;
						requests[i] = storageTransferRequest;
					}
				}
				num3 += (flag2 ? storageTransferRequest.m_Amount : (-storageTransferRequest.m_Amount));
			}
			int num9 = num2 - resources;
			TradeCost tradeCost = EconomyUtils.GetTradeCost(resource, tradeCosts);
			long lastTradeRequestTime = EconomyUtils.GetLastTradeRequestTime(tradeCosts);
			if (station && tradeCost.m_LastTransferRequestTime == 0L)
			{
				tradeCost.m_LastTransferRequestTime = simulationFrame - kTransferCooldown / 2;
				EconomyUtils.SetTradeCost(resource, tradeCost, tradeCosts, keepLastTime: false);
			}
			if (simulationFrame - lastTradeRequestTime >= kTransferCooldown + ((Random)(ref random)).NextInt(storageCompanyData.m_TransportInterval.x, storageCompanyData.m_TransportInterval.y) || tradeCost.m_LastTransferRequestTime == 0L)
			{
				int num10 = (station ? kStationExportStartAmount : kStorageExportStartAmount);
				num10 = (int)math.min((float)num2 * 0.8f, (float)num10);
				int num11 = (station ? kStationLowStockAmount : kStorageLowStockAmount);
				num11 = (int)math.min((float)num2 * 0.5f, (float)num11);
				int num12 = (station ? kStationMinimalTransferAmount : kStorageMinimalTransferAmount);
				num12 = (int)math.min((float)num2 * 0.5f, (float)num12);
				int num13 = (num10 - num11) / 2 + num11;
				if (resources > num10 && num3 > num10)
				{
					int num14 = 0;
					int num15 = resources - num11;
					if (!station)
					{
						truckSelectData.TrySelectItem(ref random, resource, num15, out var item);
						if (item.m_Capacity > 0)
						{
							num15 = item.m_Capacity * math.max(num15 / item.m_Capacity, 1);
						}
						num14 = item.m_Cost;
					}
					if (station || (float)num14 / (float)math.min(resources, num15) < kMaxTransportUnitCost)
					{
						((ParallelWriter)(ref commandBuffer)).AddComponent<StorageTransfer>(chunkIndex, company, new StorageTransfer
						{
							m_Resource = resource,
							m_Amount = num15
						});
						tradeCost.m_LastTransferRequestTime = simulationFrame;
						EconomyUtils.SetTradeCost(resource, tradeCost, tradeCosts, keepLastTime: false);
						flag = true;
					}
				}
				else if (resources < num11 && num3 < num11)
				{
					if (station && !hasConnectedRoute)
					{
						return false;
					}
					StorageTransfer storageTransfer = new StorageTransfer
					{
						m_Resource = resource
					};
					int num16 = math.min((int)((float)num9 * 0.9f), math.max(num13 - resources, num12));
					storageTransfer.m_Amount = -num16;
					if (!station)
					{
						truckSelectData.TrySelectItem(ref random, resource, num16, out var item2);
						if (item2.m_Capacity > 0)
						{
							storageTransfer.m_Amount = -math.max(num16 / item2.m_Capacity, 1) * item2.m_Capacity;
						}
					}
					((ParallelWriter)(ref commandBuffer)).AddComponent<StorageTransfer>(chunkIndex, company, storageTransfer);
					tradeCost.m_LastTransferRequestTime = simulationFrame;
					flag = true;
				}
				if (((Random)(ref random)).NextInt(kCostFadeProbability) == 0)
				{
					tradeCost.m_BuyCost *= 0.99f;
					tradeCost.m_SellCost *= 0.99f;
					if (!flag)
					{
						EconomyUtils.SetTradeCost(resource, tradeCost, tradeCosts, keepLastTime: false);
					}
				}
				if (flag)
				{
					EconomyUtils.SetTradeCost(resource, tradeCost, tradeCosts, keepLastTime: false);
				}
			}
		}
		return flag;
	}

	public void PostDeserialize(Context context)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		if (!(((Context)(ref context)).version < Version.storageConditionReset))
		{
			return;
		}
		NativeArray<Entity> val = ((EntityQuery)(ref m_CompanyGroup)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
		Enumerator<Entity> enumerator = val.GetEnumerator();
		EntityManager entityManager;
		try
		{
			while (enumerator.MoveNext())
			{
				Entity current = enumerator.Current;
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).GetBuffer<TradeCost>(current, false).Clear();
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		val.Dispose();
		val = ((EntityQuery)(ref m_StationGroup)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
		enumerator = val.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Entity current2 = enumerator.Current;
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).GetBuffer<TradeCost>(current2, false).Clear();
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		val.Dispose();
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
	public StorageCompanySystem()
	{
	}
}
