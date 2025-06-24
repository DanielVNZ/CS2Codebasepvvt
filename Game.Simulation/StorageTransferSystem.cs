using System.Runtime.CompilerServices;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Economy;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Routes;
using Game.Tools;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class StorageTransferSystem : GameSystemBase
{
	private struct StorageTransferEvent
	{
		public Entity m_Source;

		public Entity m_Destination;

		public float m_Distance;

		public Resource m_Resource;

		public int m_Amount;
	}

	[BurstCompile]
	private struct TransferJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<StorageTransfer> m_TransferType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		[ReadOnly]
		public BufferTypeHandle<Resources> m_ResourceType;

		[ReadOnly]
		public ComponentLookup<StorageLimitData> m_Limits;

		[ReadOnly]
		public ComponentLookup<PathInformation> m_PathInformation;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_Properties;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> m_OutsideConnections;

		[ReadOnly]
		public ComponentLookup<StorageCompanyData> m_StorageCompanyDatas;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public BufferLookup<Resources> m_Resources;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

		[ReadOnly]
		public BufferLookup<StorageTransferRequest> m_StorageTransferRequests;

		[ReadOnly]
		public BufferLookup<GuestVehicle> m_GuestVehicles;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_LayoutElements;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.DeliveryTruck> m_DeliveryTrucks;

		[ReadOnly]
		public ComponentLookup<ResourceData> m_ResourceDatas;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> m_SpawnableDatas;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_BuildingDatas;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.CargoTransportStation> m_CargoTransportStations;

		[ReadOnly]
		public ResourcePrefabs m_ResourcePrefabs;

		public ParallelWriter<SetupQueueItem> m_PathfindQueue;

		public ParallelWriter<StorageTransferEvent> m_TransferQueue;

		[ReadOnly]
		public DeliveryTruckSelectData m_DeliveryTruckSelectData;

		public ParallelWriter m_CommandBuffer;

		public RandomSeed m_RandomSeed;

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
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_052a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0508: Unknown result type (might be due to invalid IL or missing references)
			//IL_0516: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0494: Unknown result type (might be due to invalid IL or missing references)
			//IL_0496: Unknown result type (might be due to invalid IL or missing references)
			//IL_049d: Unknown result type (might be due to invalid IL or missing references)
			//IL_049f: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<StorageTransfer> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<StorageTransfer>(ref m_TransferType);
			NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabType);
			BufferAccessor<Resources> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Resources>(ref m_ResourceType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity val = nativeArray2[i];
				StorageTransfer storageTransfer = nativeArray[i];
				Entity prefab = nativeArray3[i].m_Prefab;
				if (!m_Limits.HasComponent(prefab) || !m_StorageCompanyDatas.HasComponent(prefab))
				{
					continue;
				}
				StorageCompanyData data = m_StorageCompanyDatas[prefab];
				if (m_InstalledUpgrades.HasBuffer(val))
				{
					UpgradeUtils.CombineStats<StorageCompanyData>(ref data, m_InstalledUpgrades[val], ref m_Prefabs, ref m_StorageCompanyDatas);
				}
				int num = EconomyUtils.CountResources(data.m_StoredResources);
				if (num == 0)
				{
					continue;
				}
				int num2 = GetStorageLimit(val, prefab) / num;
				DynamicBuffer<Resources> resources = bufferAccessor[i];
				int resources2 = EconomyUtils.GetResources(storageTransfer.m_Resource, resources);
				if (m_PathInformation.HasComponent(val))
				{
					PathInformation pathInformation = m_PathInformation[val];
					if ((pathInformation.m_State & PathFlags.Pending) != 0)
					{
						continue;
					}
					Entity val2 = ((storageTransfer.m_Amount < 0) ? pathInformation.m_Origin : pathInformation.m_Destination);
					bool flag = m_OutsideConnections.HasComponent(val2);
					bool flag2 = m_CargoTransportStations.HasComponent(val2);
					if ((m_Properties.HasComponent(val2) || flag) && val != val2)
					{
						prefab = m_Prefabs[val2].m_Prefab;
						data = m_StorageCompanyDatas[prefab];
						if (m_InstalledUpgrades.HasBuffer(val) && m_InstalledUpgrades[val].Length != 0)
						{
							UpgradeUtils.CombineStats<StorageCompanyData>(ref data, m_InstalledUpgrades[val], ref m_Prefabs, ref m_StorageCompanyDatas);
						}
						num = EconomyUtils.CountResources(data.m_StoredResources);
						if (num == 0)
						{
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<StorageTransfer>(unfilteredChunkIndex, val);
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<PathInformation>(unfilteredChunkIndex, val);
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<PathElement>(unfilteredChunkIndex, val);
							continue;
						}
						int targetCapacity = GetStorageLimit(val2, prefab) / num;
						resources = m_Resources[val2];
						long num3 = EconomyUtils.GetResources(storageTransfer.m_Resource, resources);
						int allBuyingResourcesTrucks = VehicleUtils.GetAllBuyingResourcesTrucks(val2, storageTransfer.m_Resource, ref m_DeliveryTrucks, ref m_GuestVehicles, ref m_LayoutElements);
						num3 -= allBuyingResourcesTrucks;
						if (m_StorageTransferRequests.HasBuffer(val2))
						{
							long num4 = 0L;
							DynamicBuffer<StorageTransferRequest> val3 = m_StorageTransferRequests[val2];
							for (int j = 0; j < val3.Length; j++)
							{
								StorageTransferRequest storageTransferRequest = val3[j];
								if (storageTransferRequest.m_Resource == storageTransfer.m_Resource)
								{
									num4 += (((storageTransferRequest.m_Flags & StorageTransferFlags.Incoming) != 0) ? storageTransferRequest.m_Amount : (-storageTransferRequest.m_Amount));
								}
							}
							num3 += num4;
						}
						if (flag2 || flag)
						{
							if (storageTransfer.m_Amount < 0)
							{
								if (num3 > 0)
								{
									storageTransfer.m_Amount = -math.min((int)num3, math.abs(storageTransfer.m_Amount));
								}
								else
								{
									storageTransfer.m_Amount = 0;
								}
							}
						}
						else
						{
							storageTransfer.m_Amount = CalculateTransferableAmount(storageTransfer.m_Amount, resources2, num2, (int)math.max(0L, num3), targetCapacity);
						}
						m_DeliveryTruckSelectData.TrySelectItem(ref random, storageTransfer.m_Resource, math.abs(storageTransfer.m_Amount), out var item);
						if (storageTransfer.m_Amount != 0 && (float)item.m_Cost / (float)math.min(math.abs(storageTransfer.m_Amount), item.m_Capacity) <= kMaxTransportUnitCost)
						{
							int num5 = math.abs(storageTransfer.m_Amount) / item.m_Capacity * item.m_Capacity;
							if (num5 != 0)
							{
								m_DeliveryTruckSelectData.TrySelectItem(ref random, storageTransfer.m_Resource, math.abs(storageTransfer.m_Amount) - num5, out item);
								if (math.abs(storageTransfer.m_Amount) - num5 > 0 && (float)(item.m_Cost / (math.abs(storageTransfer.m_Amount) - num5)) > kMaxTransportUnitCost)
								{
									storageTransfer.m_Amount = ((storageTransfer.m_Amount > 0) ? num5 : (-num5));
								}
							}
							if (storageTransfer.m_Amount != 0)
							{
								m_TransferQueue.Enqueue(new StorageTransferEvent
								{
									m_Amount = storageTransfer.m_Amount,
									m_Destination = val2,
									m_Source = val,
									m_Distance = pathInformation.m_Distance,
									m_Resource = storageTransfer.m_Resource
								});
							}
						}
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<StorageTransfer>(unfilteredChunkIndex, val);
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<PathInformation>(unfilteredChunkIndex, val);
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<PathElement>(unfilteredChunkIndex, val);
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<StorageTransfer>(unfilteredChunkIndex, val);
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<PathInformation>(unfilteredChunkIndex, val);
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<PathElement>(unfilteredChunkIndex, val);
					}
				}
				else
				{
					float fillProportion = (float)resources2 / (float)num2;
					FindTarget(unfilteredChunkIndex, val, storageTransfer.m_Resource, storageTransfer.m_Amount, fillProportion, num2);
				}
			}
		}

		private int GetStorageLimit(Entity entity, Entity prefab)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			StorageLimitData data = m_Limits[prefab];
			if (m_InstalledUpgrades.HasBuffer(entity))
			{
				UpgradeUtils.CombineStats<StorageLimitData>(ref data, m_InstalledUpgrades[entity], ref m_Prefabs, ref m_Limits);
			}
			if (m_Properties.HasComponent(entity) && m_Prefabs.HasComponent(m_Properties[entity].m_Property))
			{
				Entity prefab2 = m_Prefabs[m_Properties[entity].m_Property].m_Prefab;
				return data.GetAdjustedLimitForWarehouse(m_SpawnableDatas.HasComponent(prefab2) ? m_SpawnableDatas[prefab2] : new SpawnableBuildingData
				{
					m_Level = 1
				}, m_SpawnableDatas.HasComponent(prefab2) ? m_BuildingDatas[prefab2] : new BuildingData
				{
					m_LotSize = new int2(1, 1)
				});
			}
			if (m_OutsideConnections.HasComponent(entity))
			{
				return data.m_Limit;
			}
			return 0;
		}

		private void FindTarget(int chunkIndex, Entity storage, Resource resource, int amount, float fillProportion, int capacity)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathInformation>(chunkIndex, storage, new PathInformation
			{
				m_State = PathFlags.Pending
			});
			((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<PathElement>(chunkIndex, storage);
			float transportCost = EconomyUtils.GetTransportCost(1f, math.abs(amount), m_ResourceDatas[m_ResourcePrefabs[resource]].m_Weight, StorageTransferFlags.Car);
			PathfindParameters parameters = new PathfindParameters
			{
				m_MaxSpeed = float2.op_Implicit(111.111115f),
				m_WalkSpeed = float2.op_Implicit(5.555556f),
				m_Weights = new PathfindWeights(0.01f, 0.01f, transportCost, 0.01f),
				m_Methods = (PathMethod.Road | PathMethod.CargoTransport | PathMethod.CargoLoading),
				m_IgnoredRules = RuleFlags.ForbidSlowTraffic
			};
			SetupQueueTarget a = new SetupQueueTarget
			{
				m_Type = SetupTargetType.CurrentLocation,
				m_Methods = (PathMethod.Road | PathMethod.CargoLoading),
				m_RoadTypes = RoadTypes.Car
			};
			SetupQueueTarget b = new SetupQueueTarget
			{
				m_Type = SetupTargetType.StorageTransfer,
				m_Methods = (PathMethod.Road | PathMethod.CargoLoading),
				m_RoadTypes = RoadTypes.Car,
				m_Entity = storage,
				m_Resource = resource,
				m_Value = amount,
				m_Value2 = fillProportion,
				m_Value3 = capacity
			};
			if (amount < 0)
			{
				CommonUtils.Swap(ref a, ref b);
			}
			SetupQueueItem setupQueueItem = new SetupQueueItem(storage, parameters, a, b);
			m_PathfindQueue.Enqueue(setupQueueItem);
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct HandleTransfersJob : IJob
	{
		public NativeQueue<StorageTransferEvent> m_TransferQueue;

		public BufferLookup<TradeCost> m_TradeCosts;

		public ComponentLookup<Game.Companies.StorageCompany> m_StorageCompanies;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> m_CarLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.TrackLane> m_TrackLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.PedestrianLane> m_PedestrianLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> m_ConnectionLaneData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public BufferLookup<Renter> m_Renters;

		[ReadOnly]
		public ComponentLookup<Connected> m_ConnectedData;

		[ReadOnly]
		public ComponentLookup<Game.Routes.Segment> m_SegmentData;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> m_RouteWaypoints;

		[ReadOnly]
		public ComponentLookup<PathInformation> m_PathInfos;

		[ReadOnly]
		public BufferLookup<PathElement> m_Paths;

		[ReadOnly]
		public ComponentLookup<Building> m_Buildings;

		[ReadOnly]
		public ComponentLookup<Curve> m_Curves;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> m_OutsideConnections;

		[ReadOnly]
		public ComponentLookup<CityServiceUpkeep> m_CityServiceUpkeeps;

		[ReadOnly]
		public ResourcePrefabs m_ResourcePrefabs;

		[ReadOnly]
		public ComponentLookup<ResourceData> m_ResourceDatas;

		public BufferLookup<StorageTransferRequest> m_Requests;

		public BufferLookup<Resources> m_Resources;

		public Entity m_City;

		private Entity GetStorageCompanyFromLane(Entity entity)
		{
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			while (m_OwnerData.HasComponent(entity))
			{
				entity = m_OwnerData[entity].m_Owner;
				if (m_StorageCompanies.HasComponent(entity))
				{
					return entity;
				}
				if (!m_Renters.HasBuffer(entity))
				{
					continue;
				}
				DynamicBuffer<Renter> val = m_Renters[entity];
				for (int i = 0; i < val.Length; i++)
				{
					Entity renter = val[i].m_Renter;
					if (m_StorageCompanies.HasComponent(renter))
					{
						return renter;
					}
				}
			}
			return Entity.Null;
		}

		private Entity GetStorageCompanyFromWaypoint(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			if (m_ConnectedData.HasComponent(entity))
			{
				entity = m_ConnectedData[entity].m_Connected;
				while (true)
				{
					if (m_StorageCompanies.HasComponent(entity))
					{
						return entity;
					}
					if (m_Renters.HasBuffer(entity))
					{
						DynamicBuffer<Renter> val = m_Renters[entity];
						for (int i = 0; i < val.Length; i++)
						{
							Entity renter = val[i].m_Renter;
							if (m_StorageCompanies.HasComponent(renter))
							{
								return renter;
							}
						}
					}
					if (!m_OwnerData.HasComponent(entity))
					{
						break;
					}
					entity = m_OwnerData[entity].m_Owner;
				}
			}
			return Entity.Null;
		}

		private void GetStorageCompaniesFromSegment(Entity entity, out Entity startCompany, out Entity endCompany)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			Entity owner = m_OwnerData[entity].m_Owner;
			Game.Routes.Segment segment = m_SegmentData[entity];
			DynamicBuffer<RouteWaypoint> val = m_RouteWaypoints[owner];
			int num = segment.m_Index + 1;
			if (num == val.Length)
			{
				num = 0;
			}
			startCompany = GetStorageCompanyFromWaypoint(val[segment.m_Index].m_Waypoint);
			endCompany = GetStorageCompanyFromWaypoint(val[num].m_Waypoint);
		}

		private float HandleCargoPath(PathInformation pathInformation, DynamicBuffer<PathElement> path, Resource resource, int amount, float weight)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			float num = 0f;
			float num2 = 0f;
			Entity val = pathInformation.m_Origin;
			StorageTransferFlags storageTransferFlags = (StorageTransferFlags)0;
			int num3 = path.Length;
			int num4 = 0;
			for (int i = 0; i < path.Length; i++)
			{
				Entity target = path[i].m_Target;
				if (m_Curves.HasComponent(target))
				{
					num2 += m_Curves[target].m_Length * math.abs(path[i].m_TargetDelta.y - path[i].m_TargetDelta.x);
				}
				if (m_CarLaneData.HasComponent(target))
				{
					storageTransferFlags |= StorageTransferFlags.Car;
					num3 = math.min(num3, i);
					num4 = math.max(num4, i + 1);
				}
				else if (m_TrackLaneData.HasComponent(target))
				{
					storageTransferFlags |= StorageTransferFlags.Track;
					num3 = math.min(num3, i);
					num4 = math.max(num4, i + 1);
				}
				else if (m_PedestrianLaneData.HasComponent(target))
				{
					Entity storageCompanyFromLane = GetStorageCompanyFromLane(target);
					if (storageCompanyFromLane != Entity.Null && storageCompanyFromLane != val)
					{
						num += AddCargoPathSection(val, storageCompanyFromLane, path, num3, num4 - num3, storageTransferFlags, resource, amount, weight, num2);
						val = storageCompanyFromLane;
						storageTransferFlags = (StorageTransferFlags)0;
						num3 = path.Length;
						num4 = 0;
						num2 = 0f;
					}
				}
				else if (m_ConnectionLaneData.HasComponent(target))
				{
					Game.Net.ConnectionLane connectionLane = m_ConnectionLaneData[target];
					if ((connectionLane.m_Flags & ConnectionLaneFlags.Road) != 0)
					{
						storageTransferFlags |= StorageTransferFlags.Car;
						num3 = math.min(num3, i);
						num4 = math.max(num4, i + 1);
					}
					else if ((connectionLane.m_Flags & ConnectionLaneFlags.Track) != 0)
					{
						storageTransferFlags |= StorageTransferFlags.Track;
						num3 = math.min(num3, i);
						num4 = math.max(num4, i + 1);
					}
				}
				else if (m_SegmentData.HasComponent(target))
				{
					GetStorageCompaniesFromSegment(target, out var startCompany, out var endCompany);
					if (startCompany != Entity.Null && startCompany != val)
					{
						num += AddCargoPathSection(val, startCompany, path, num3, num4 - num3, storageTransferFlags, resource, amount, weight, num2);
						val = startCompany;
						storageTransferFlags = (StorageTransferFlags)0;
						num3 = path.Length;
						num4 = 0;
						num2 = 0f;
					}
					storageTransferFlags |= StorageTransferFlags.Transport;
					if (endCompany != Entity.Null && endCompany != val)
					{
						num += AddCargoPathSection(val, endCompany, path, num3, num4 - num3, storageTransferFlags, resource, amount, weight, num2);
						val = endCompany;
						storageTransferFlags = (StorageTransferFlags)0;
						num3 = path.Length;
						num4 = 0;
						num2 = 0f;
					}
				}
			}
			if (pathInformation.m_Destination != val)
			{
				num += AddCargoPathSection(val, pathInformation.m_Destination, path, num3, num4 - num3, storageTransferFlags, resource, amount, weight, num2);
			}
			return num;
		}

		private void AddRequest(DynamicBuffer<StorageTransferRequest> requests, Entity destination, StorageTransferFlags flags, Resource resource, int amount)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			if (m_Buildings.HasComponent(destination) && BuildingUtils.CheckOption(m_Buildings[destination], BuildingOption.Inactive))
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i < requests.Length; i++)
			{
				StorageTransferRequest storageTransferRequest = requests[i];
				if (storageTransferRequest.m_Target == destination && storageTransferRequest.m_Resource == resource && storageTransferRequest.m_Flags == flags)
				{
					storageTransferRequest.m_Amount += amount;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				requests.Add(new StorageTransferRequest
				{
					m_Amount = math.abs(amount),
					m_Resource = resource,
					m_Target = destination,
					m_Flags = flags
				});
			}
		}

		private float AddCargoPathSection(Entity origin, Entity destination, DynamicBuffer<PathElement> path, int startIndex, int length, StorageTransferFlags flags, Resource resource, int amount, float weight, float distance)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			if (m_Requests.HasBuffer(origin) && m_Requests.HasBuffer(destination))
			{
				DynamicBuffer<StorageTransferRequest> requests = m_Requests[origin];
				AddRequest(requests, destination, flags, resource, amount);
				requests = m_Requests[destination];
				AddRequest(requests, origin, flags | StorageTransferFlags.Incoming, resource, math.abs(amount));
				EconomyUtils.GetTransportCost(distance, math.abs(amount), weight, flags);
				return EconomyUtils.GetTransportCost(distance, math.abs(amount), weight, flags);
			}
			return 0f;
		}

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<TradeCost> costs = m_TradeCosts[m_City];
			StorageTransferEvent storageTransferEvent = default(StorageTransferEvent);
			while (m_TransferQueue.TryDequeue(ref storageTransferEvent))
			{
				if (!m_PathInfos.HasComponent(storageTransferEvent.m_Source) || !m_Paths.HasBuffer(storageTransferEvent.m_Source))
				{
					continue;
				}
				float weight = EconomyUtils.GetWeight(storageTransferEvent.m_Resource, m_ResourcePrefabs, ref m_ResourceDatas);
				float num = HandleCargoPath(m_PathInfos[storageTransferEvent.m_Source], m_Paths[storageTransferEvent.m_Source], storageTransferEvent.m_Resource, storageTransferEvent.m_Amount, weight);
				DynamicBuffer<TradeCost> costs2 = m_TradeCosts[storageTransferEvent.m_Source];
				DynamicBuffer<TradeCost> costs3 = m_TradeCosts[storageTransferEvent.m_Destination];
				TradeCost tradeCost = EconomyUtils.GetTradeCost(storageTransferEvent.m_Resource, costs2);
				TradeCost tradeCost2 = EconomyUtils.GetTradeCost(storageTransferEvent.m_Resource, costs3);
				float num2 = num / (1f + (float)math.abs(storageTransferEvent.m_Amount));
				if (storageTransferEvent.m_Amount > 0)
				{
					tradeCost.m_SellCost = math.lerp(tradeCost.m_SellCost, num2 + tradeCost2.m_SellCost, 0.5f);
					tradeCost2.m_BuyCost = math.lerp(tradeCost2.m_BuyCost, num2 + tradeCost.m_BuyCost, 0.5f);
				}
				else
				{
					tradeCost.m_BuyCost = math.lerp(tradeCost.m_BuyCost, num2 + tradeCost2.m_BuyCost, 0.5f);
					tradeCost2.m_SellCost = math.lerp(tradeCost2.m_SellCost, num2 + tradeCost.m_SellCost, 0.5f);
				}
				int amount = Mathf.RoundToInt(kStorageProfit * num);
				EconomyUtils.GetTradeCost(storageTransferEvent.m_Resource, costs);
				if (!m_OutsideConnections.HasComponent(storageTransferEvent.m_Source))
				{
					EconomyUtils.SetTradeCost(storageTransferEvent.m_Resource, tradeCost, costs2, keepLastTime: true);
					if (m_Resources.HasBuffer(storageTransferEvent.m_Source) && !m_CityServiceUpkeeps.HasComponent(storageTransferEvent.m_Source))
					{
						EconomyUtils.AddResources(Resource.Money, amount, m_Resources[storageTransferEvent.m_Source]);
					}
				}
				else if (storageTransferEvent.m_Amount > 0)
				{
					EconomyUtils.SetTradeCost(storageTransferEvent.m_Resource, tradeCost2, costs, keepLastTime: false, 0.1f, 0f);
					EconomyUtils.GetTradeCost(storageTransferEvent.m_Resource, costs);
				}
				else
				{
					EconomyUtils.SetTradeCost(storageTransferEvent.m_Resource, tradeCost2, costs, keepLastTime: false, 0f, 0.1f);
					EconomyUtils.GetTradeCost(storageTransferEvent.m_Resource, costs);
				}
				if (!m_OutsideConnections.HasComponent(storageTransferEvent.m_Destination))
				{
					EconomyUtils.SetTradeCost(storageTransferEvent.m_Resource, tradeCost2, costs3, keepLastTime: true);
					if (m_Resources.HasBuffer(storageTransferEvent.m_Destination) && !m_CityServiceUpkeeps.HasComponent(storageTransferEvent.m_Destination))
					{
						EconomyUtils.AddResources(Resource.Money, amount, m_Resources[storageTransferEvent.m_Destination]);
					}
				}
				else if (storageTransferEvent.m_Amount > 0)
				{
					EconomyUtils.SetTradeCost(storageTransferEvent.m_Resource, tradeCost, costs, keepLastTime: false, 0f, 0.1f);
					EconomyUtils.GetTradeCost(storageTransferEvent.m_Resource, costs);
				}
				else
				{
					EconomyUtils.SetTradeCost(storageTransferEvent.m_Resource, tradeCost, costs, keepLastTime: false, 0.1f, 0f);
					EconomyUtils.GetTradeCost(storageTransferEvent.m_Resource, costs);
				}
				Game.Companies.StorageCompany storageCompany = m_StorageCompanies[storageTransferEvent.m_Source];
				storageCompany.m_LastTradePartner = storageTransferEvent.m_Destination;
				m_StorageCompanies[storageTransferEvent.m_Source] = storageCompany;
				Game.Companies.StorageCompany storageCompany2 = m_StorageCompanies[storageTransferEvent.m_Destination];
				storageCompany2.m_LastTradePartner = storageTransferEvent.m_Source;
				m_StorageCompanies[storageTransferEvent.m_Destination] = storageCompany2;
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<StorageTransfer> __Game_Companies_StorageTransfer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Resources> __Game_Economy_Resources_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> __Game_Objects_OutsideConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StorageLimitData> __Game_Companies_StorageLimitData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StorageCompanyData> __Game_Prefabs_StorageCompanyData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Resources> __Game_Economy_Resources_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<StorageTransferRequest> __Game_Companies_StorageTransferRequest_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<ResourceData> __Game_Prefabs_ResourceData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.CargoTransportStation> __Game_Buildings_CargoTransportStation_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<GuestVehicle> __Game_Vehicles_GuestVehicle_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.DeliveryTruck> __Game_Vehicles_DeliveryTruck_RO_ComponentLookup;

		public BufferLookup<TradeCost> __Game_Companies_TradeCost_RW_BufferLookup;

		public ComponentLookup<Game.Companies.StorageCompany> __Game_Companies_StorageCompany_RW_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Routes.Segment> __Game_Routes_Segment_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Connected> __Game_Routes_Connected_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> __Game_Net_CarLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.TrackLane> __Game_Net_TrackLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.PedestrianLane> __Game_Net_PedestrianLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> __Game_Net_ConnectionLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Renter> __Game_Buildings_Renter_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> __Game_Routes_RouteWaypoint_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<PathElement> __Game_Pathfind_PathElement_RO_BufferLookup;

		public BufferLookup<StorageTransferRequest> __Game_Companies_StorageTransferRequest_RW_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CityServiceUpkeep> __Game_City_CityServiceUpkeep_RO_ComponentLookup;

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
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Companies_StorageTransfer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<StorageTransfer>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Economy_Resources_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Resources>(true);
			__Game_Pathfind_PathInformation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathInformation>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
			__Game_Objects_OutsideConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.OutsideConnection>(true);
			__Game_Companies_StorageLimitData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StorageLimitData>(true);
			__Game_Prefabs_StorageCompanyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StorageCompanyData>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Economy_Resources_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Resources>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<InstalledUpgrade>(true);
			__Game_Companies_StorageTransferRequest_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<StorageTransferRequest>(true);
			__Game_Prefabs_ResourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceData>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableBuildingData>(true);
			__Game_Buildings_CargoTransportStation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.CargoTransportStation>(true);
			__Game_Vehicles_GuestVehicle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<GuestVehicle>(true);
			__Game_Vehicles_LayoutElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(true);
			__Game_Vehicles_DeliveryTruck_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.DeliveryTruck>(true);
			__Game_Companies_TradeCost_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TradeCost>(false);
			__Game_Companies_StorageCompany_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Companies.StorageCompany>(false);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Routes_Segment_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Routes.Segment>(true);
			__Game_Routes_Connected_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Connected>(true);
			__Game_Net_CarLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.CarLane>(true);
			__Game_Net_TrackLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.TrackLane>(true);
			__Game_Net_PedestrianLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.PedestrianLane>(true);
			__Game_Net_ConnectionLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ConnectionLane>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Buildings_Renter_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Renter>(true);
			__Game_Routes_RouteWaypoint_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteWaypoint>(true);
			__Game_Pathfind_PathElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PathElement>(true);
			__Game_Companies_StorageTransferRequest_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<StorageTransferRequest>(false);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_City_CityServiceUpkeep_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CityServiceUpkeep>(true);
			__Game_Economy_Resources_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Resources>(false);
		}
	}

	public static readonly float kStorageProfit = 0.01f;

	public static readonly float kMaxTransportUnitCost = 0.01f;

	private EntityQuery m_TransferGroup;

	private PathfindSetupSystem m_PathfindSetupSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private ResourceSystem m_ResourceSystem;

	private VehicleCapacitySystem m_VehicleCapacitySystem;

	private CitySystem m_CitySystem;

	private NativeQueue<StorageTransferEvent> m_TransferQueue;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PathfindSetupSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindSetupSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		m_VehicleCapacitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<VehicleCapacitySystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_TransferQueue = new NativeQueue<StorageTransferEvent>(AllocatorHandle.op_Implicit((Allocator)4));
		m_TransferGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<StorageTransfer>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<Resources>(),
			ComponentType.ReadWrite<TripNeeded>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_TransferGroup);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_TransferQueue.Dispose();
		base.OnDestroy();
	}

	public static int CalculateTransferableAmount(int original, int sourceAmount, int sourceCapacity, int targetAmount, int targetCapacity)
	{
		if (targetCapacity == 0 && sourceCapacity == 0)
		{
			return 0;
		}
		if (original > 0)
		{
			return math.min(targetCapacity - targetAmount, original);
		}
		return -math.min(sourceCapacity - sourceAmount, -original);
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
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		//IL_054b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0553: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Unknown result type (might be due to invalid IL or missing references)
		//IL_0565: Unknown result type (might be due to invalid IL or missing references)
		//IL_056a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0571: Unknown result type (might be due to invalid IL or missing references)
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_0577: Unknown result type (might be due to invalid IL or missing references)
		//IL_057e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0585: Unknown result type (might be due to invalid IL or missing references)
		TransferJob transferJob = new TransferJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransferType = InternalCompilerInterface.GetComponentTypeHandle<StorageTransfer>(ref __TypeHandle.__Game_Companies_StorageTransfer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceType = InternalCompilerInterface.GetBufferTypeHandle<Resources>(ref __TypeHandle.__Game_Economy_Resources_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathInformation = InternalCompilerInterface.GetComponentLookup<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Properties = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OutsideConnections = InternalCompilerInterface.GetComponentLookup<Game.Objects.OutsideConnection>(ref __TypeHandle.__Game_Objects_OutsideConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Limits = InternalCompilerInterface.GetComponentLookup<StorageLimitData>(ref __TypeHandle.__Game_Companies_StorageLimitData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StorageCompanyDatas = InternalCompilerInterface.GetComponentLookup<StorageCompanyData>(ref __TypeHandle.__Game_Prefabs_StorageCompanyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Resources = InternalCompilerInterface.GetBufferLookup<Resources>(ref __TypeHandle.__Game_Economy_Resources_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgrades = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StorageTransferRequests = InternalCompilerInterface.GetBufferLookup<StorageTransferRequest>(ref __TypeHandle.__Game_Companies_StorageTransferRequest_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingDatas = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnableDatas = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CargoTransportStations = InternalCompilerInterface.GetComponentLookup<Game.Buildings.CargoTransportStation>(ref __TypeHandle.__Game_Buildings_CargoTransportStation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GuestVehicles = InternalCompilerInterface.GetBufferLookup<GuestVehicle>(ref __TypeHandle.__Game_Vehicles_GuestVehicle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LayoutElements = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DeliveryTrucks = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.DeliveryTruck>(ref __TypeHandle.__Game_Vehicles_DeliveryTruck_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourcePrefabs = m_ResourceSystem.GetPrefabs(),
			m_PathfindQueue = m_PathfindSetupSystem.GetQueue(this, 64).AsParallelWriter(),
			m_TransferQueue = m_TransferQueue.AsParallelWriter(),
			m_DeliveryTruckSelectData = m_VehicleCapacitySystem.GetDeliveryTruckSelectData()
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		transferJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		transferJob.m_RandomSeed = RandomSeed.Next();
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<TransferJob>(transferJob, m_TransferGroup, ((SystemBase)this).Dependency);
		m_EndFrameBarrier.AddJobHandleForProducer(val2);
		m_PathfindSetupSystem.AddQueueWriter(val2);
		JobHandle val3 = IJobExtensions.Schedule<HandleTransfersJob>(new HandleTransfersJob
		{
			m_TradeCosts = InternalCompilerInterface.GetBufferLookup<TradeCost>(ref __TypeHandle.__Game_Companies_TradeCost_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StorageCompanies = InternalCompilerInterface.GetComponentLookup<Game.Companies.StorageCompany>(ref __TypeHandle.__Game_Companies_StorageCompany_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SegmentData = InternalCompilerInterface.GetComponentLookup<Game.Routes.Segment>(ref __TypeHandle.__Game_Routes_Segment_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedData = InternalCompilerInterface.GetComponentLookup<Connected>(ref __TypeHandle.__Game_Routes_Connected_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrackLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.TrackLane>(ref __TypeHandle.__Game_Net_TrackLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PedestrianLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.PedestrianLane>(ref __TypeHandle.__Game_Net_PedestrianLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectionLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Buildings = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Renters = InternalCompilerInterface.GetBufferLookup<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteWaypoints = InternalCompilerInterface.GetBufferLookup<RouteWaypoint>(ref __TypeHandle.__Game_Routes_RouteWaypoint_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathInfos = InternalCompilerInterface.GetComponentLookup<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Paths = InternalCompilerInterface.GetBufferLookup<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Requests = InternalCompilerInterface.GetBufferLookup<StorageTransferRequest>(ref __TypeHandle.__Game_Companies_StorageTransferRequest_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Curves = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OutsideConnections = InternalCompilerInterface.GetComponentLookup<Game.Objects.OutsideConnection>(ref __TypeHandle.__Game_Objects_OutsideConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CityServiceUpkeeps = InternalCompilerInterface.GetComponentLookup<CityServiceUpkeep>(ref __TypeHandle.__Game_City_CityServiceUpkeep_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Resources = InternalCompilerInterface.GetBufferLookup<Resources>(ref __TypeHandle.__Game_Economy_Resources_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourcePrefabs = m_ResourceSystem.GetPrefabs(),
			m_ResourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransferQueue = m_TransferQueue,
			m_City = m_CitySystem.City
		}, val2);
		m_ResourceSystem.AddPrefabsReader(val3);
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
	public StorageTransferSystem()
	{
	}
}
