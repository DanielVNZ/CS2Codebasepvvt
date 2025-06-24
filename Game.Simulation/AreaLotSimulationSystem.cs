using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.City;
using Game.Common;
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
public class AreaLotSimulationSystem : GameSystemBase
{
	[BurstCompile]
	private struct ManageVehiclesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<PathInformation> m_PathInformationType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<WoodResource> m_WoodResourceType;

		[ReadOnly]
		public BufferTypeHandle<PathElement> m_PathElementType;

		public ComponentTypeHandle<Extractor> m_ExtractorType;

		public ComponentTypeHandle<Storage> m_StorageType;

		public ComponentTypeHandle<Game.Buildings.CargoTransportStation> m_CargoTransportStationType;

		public BufferTypeHandle<OwnedVehicle> m_OwnedVehicleType;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Attachment> m_AttachmentData;

		[ReadOnly]
		public ComponentLookup<Connected> m_ConnectedData;

		[ReadOnly]
		public ComponentLookup<WorkRoute> m_WorkRouteData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ExtractorAreaData> m_PrefabExtractorAreaData;

		[ReadOnly]
		public ComponentLookup<StorageAreaData> m_PrefabStorageAreaData;

		[ReadOnly]
		public ComponentLookup<LotData> m_PrefabLotData;

		[ReadOnly]
		public ComponentLookup<WorkVehicleData> m_PrefabWorkVehicleData;

		[ReadOnly]
		public ComponentLookup<NavigationAreaData> m_PrefabNavigationAreaData;

		[ReadOnly]
		public ComponentLookup<CargoTransportStationData> m_PrefabCargoTransportStationData;

		[ReadOnly]
		public ComponentLookup<StorageCompanyData> m_PrefabStorageCompanyData;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> m_SubAreas;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_VehicleLayouts;

		[ReadOnly]
		public BufferLookup<SubRoute> m_SubRoutes;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> m_RouteWaypoints;

		[ReadOnly]
		public BufferLookup<RouteVehicle> m_RouteVehicles;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Game.Vehicles.WorkVehicle> m_WorkVehicleData;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public WorkVehicleSelectData m_WorkVehicleSelectData;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<SetupQueueItem> m_PathfindQueue;

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
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_060f: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0730: Unknown result type (might be due to invalid IL or missing references)
			//IL_0744: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0648: Unknown result type (might be due to invalid IL or missing references)
			//IL_0657: Unknown result type (might be due to invalid IL or missing references)
			//IL_054e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0513: Unknown result type (might be due to invalid IL or missing references)
			//IL_0688: Unknown result type (might be due to invalid IL or missing references)
			//IL_0697: Unknown result type (might be due to invalid IL or missing references)
			//IL_0579: Unknown result type (might be due to invalid IL or missing references)
			//IL_0597: Unknown result type (might be due to invalid IL or missing references)
			//IL_052b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_077c: Unknown result type (might be due to invalid IL or missing references)
			//IL_077e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0790: Unknown result type (might be due to invalid IL or missing references)
			//IL_043c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0401: Unknown result type (might be due to invalid IL or missing references)
			//IL_0464: Unknown result type (might be due to invalid IL or missing references)
			//IL_0482: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Extractor> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Extractor>(ref m_ExtractorType);
			NativeArray<Storage> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Storage>(ref m_StorageType);
			NativeArray<PathInformation> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathInformation>(ref m_PathInformationType);
			NativeArray<Game.Buildings.CargoTransportStation> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.CargoTransportStation>(ref m_CargoTransportStationType);
			NativeArray<PrefabRef> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			if (nativeArray4.Length != 0)
			{
				BufferAccessor<PathElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<PathElement>(ref m_PathElementType);
				LotData lotData = default(LotData);
				for (int i = 0; i < nativeArray4.Length; i++)
				{
					Entity val = nativeArray[i];
					PathInformation pathInformation = nativeArray4[i];
					DynamicBuffer<PathElement> path = bufferAccessor[i];
					PrefabRef prefabRef = nativeArray6[i];
					m_PrefabLotData.TryGetComponent(prefabRef.m_Prefab, ref lotData);
					if (nativeArray2.Length != 0)
					{
						Extractor extractor = nativeArray2[i];
						ExtractorAreaData extractorAreaData = m_PrefabExtractorAreaData[prefabRef.m_Prefab];
						switch (extractor.m_WorkType)
						{
						case VehicleWorkType.Harvest:
							TrySpawnVehicle(unfilteredChunkIndex, ref random, val, Entity.Null, pathInformation, path, extractor.m_WorkType, extractorAreaData.m_MapFeature, Resource.NoResource, WorkVehicleFlags.ExtractorVehicle, lotData.m_OnWater, ref extractor.m_WorkAmount);
							break;
						case VehicleWorkType.Collect:
							TrySpawnVehicle(unfilteredChunkIndex, ref random, val, Entity.Null, pathInformation, path, extractor.m_WorkType, extractorAreaData.m_MapFeature, Resource.NoResource, WorkVehicleFlags.ExtractorVehicle, lotData.m_OnWater, ref extractor.m_HarvestedAmount);
							break;
						}
						nativeArray2[i] = extractor;
					}
					if (nativeArray3.Length != 0)
					{
						Storage storage = nativeArray3[i];
						StorageAreaData storageAreaData = m_PrefabStorageAreaData[prefabRef.m_Prefab];
						TrySpawnVehicle(unfilteredChunkIndex, ref random, val, Entity.Null, pathInformation, path, VehicleWorkType.Collect, MapFeature.None, storageAreaData.m_Resources, WorkVehicleFlags.StorageVehicle, lotData.m_OnWater, ref storage.m_WorkAmount);
						nativeArray3[i] = storage;
					}
					if (nativeArray5.Length != 0)
					{
						Game.Buildings.CargoTransportStation cargoTransportStation = nativeArray5[i];
						CargoTransportStationData cargoTransportStationData = m_PrefabCargoTransportStationData[prefabRef.m_Prefab];
						StorageCompanyData storageCompanyData = m_PrefabStorageCompanyData[prefabRef.m_Prefab];
						float lotWorkAmount = cargoTransportStation.m_WorkAmount * cargoTransportStationData.m_WorkMultiplier;
						TrySpawnVehicle(unfilteredChunkIndex, ref random, val, Entity.Null, pathInformation, path, VehicleWorkType.Move, MapFeature.None, storageCompanyData.m_StoredResources, WorkVehicleFlags.CargoMoveVehicle, lotData.m_OnWater, ref lotWorkAmount);
						cargoTransportStation.m_WorkAmount = lotWorkAmount / cargoTransportStationData.m_WorkMultiplier;
						nativeArray5[i] = cargoTransportStation;
					}
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<PathInformation>(unfilteredChunkIndex, val);
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<PathElement>(unfilteredChunkIndex, val);
				}
				return;
			}
			NativeArray<Owner> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			BufferAccessor<WoodResource> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<WoodResource>(ref m_WoodResourceType);
			BufferAccessor<OwnedVehicle> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<OwnedVehicle>(ref m_OwnedVehicleType);
			LotData lotData2 = default(LotData);
			Owner owner = default(Owner);
			Extractor extractor2 = default(Extractor);
			Storage storage2 = default(Storage);
			Game.Buildings.CargoTransportStation cargoTransportStation2 = default(Game.Buildings.CargoTransportStation);
			DynamicBuffer<WoodResource> woodResources = default(DynamicBuffer<WoodResource>);
			Game.Vehicles.WorkVehicle workVehicle = default(Game.Vehicles.WorkVehicle);
			DynamicBuffer<LayoutElement> val4 = default(DynamicBuffer<LayoutElement>);
			for (int j = 0; j < bufferAccessor3.Length; j++)
			{
				Entity val2 = nativeArray[j];
				PrefabRef prefabRef2 = nativeArray6[j];
				DynamicBuffer<OwnedVehicle> val3 = bufferAccessor3[j];
				m_PrefabLotData.TryGetComponent(prefabRef2.m_Prefab, ref lotData2);
				CollectionUtils.TryGet<Owner>(nativeArray7, j, ref owner);
				CollectionUtils.TryGet<Extractor>(nativeArray2, j, ref extractor2);
				CollectionUtils.TryGet<Storage>(nativeArray3, j, ref storage2);
				CollectionUtils.TryGet<Game.Buildings.CargoTransportStation>(nativeArray5, j, ref cargoTransportStation2);
				CollectionUtils.TryGet<WoodResource>(bufferAccessor2, j, ref woodResources);
				float pendingWorkAmount = 0f;
				float pendingWorkAmount2 = 0f;
				float pendingWorkAmount3 = 0f;
				float pendingWorkAmount4 = 0f;
				for (int k = 0; k < val3.Length; k++)
				{
					Entity vehicle = val3[k].m_Vehicle;
					if (m_WorkVehicleData.TryGetComponent(vehicle, ref workVehicle))
					{
						if (m_VehicleLayouts.TryGetBuffer(vehicle, ref val4) && val4.Length != 0)
						{
							for (int l = 0; l < val4.Length; l++)
							{
								Entity vehicle2 = val4[l].m_Vehicle;
								PrefabRef prefabRef3 = m_PrefabRefData[vehicle2];
								WorkVehicleData workVehicleData = m_PrefabWorkVehicleData[prefabRef3.m_Prefab];
								if ((workVehicle.m_State & WorkVehicleFlags.ExtractorVehicle) != 0)
								{
									switch (workVehicleData.m_WorkType)
									{
									case VehicleWorkType.Harvest:
										CheckVehicle(vehicle2, workVehicleData, ref extractor2.m_WorkAmount, ref pendingWorkAmount);
										break;
									case VehicleWorkType.Collect:
										CheckVehicle(vehicle2, workVehicleData, ref extractor2.m_HarvestedAmount, ref pendingWorkAmount2);
										break;
									}
								}
								else if ((workVehicle.m_State & WorkVehicleFlags.StorageVehicle) != 0)
								{
									CheckVehicle(vehicle2, workVehicleData, ref storage2.m_WorkAmount, ref pendingWorkAmount3);
								}
								else if ((workVehicle.m_State & WorkVehicleFlags.CargoMoveVehicle) != 0)
								{
									CargoTransportStationData cargoTransportStationData2 = m_PrefabCargoTransportStationData[prefabRef2.m_Prefab];
									float lotWorkAmount2 = cargoTransportStation2.m_WorkAmount * cargoTransportStationData2.m_WorkMultiplier;
									CheckVehicle(vehicle2, workVehicleData, ref lotWorkAmount2, ref pendingWorkAmount4);
									if (lotWorkAmount2 != cargoTransportStation2.m_WorkAmount * cargoTransportStationData2.m_WorkMultiplier)
									{
										cargoTransportStation2.m_WorkAmount = lotWorkAmount2 / cargoTransportStationData2.m_WorkMultiplier;
									}
								}
							}
							continue;
						}
						PrefabRef prefabRef4 = m_PrefabRefData[vehicle];
						WorkVehicleData workVehicleData2 = m_PrefabWorkVehicleData[prefabRef4.m_Prefab];
						if ((workVehicle.m_State & WorkVehicleFlags.ExtractorVehicle) != 0)
						{
							switch (workVehicleData2.m_WorkType)
							{
							case VehicleWorkType.Harvest:
								CheckVehicle(vehicle, workVehicleData2, ref extractor2.m_WorkAmount, ref pendingWorkAmount);
								break;
							case VehicleWorkType.Collect:
								CheckVehicle(vehicle, workVehicleData2, ref extractor2.m_HarvestedAmount, ref pendingWorkAmount2);
								break;
							}
						}
						else if ((workVehicle.m_State & WorkVehicleFlags.StorageVehicle) != 0)
						{
							CheckVehicle(vehicle, workVehicleData2, ref storage2.m_WorkAmount, ref pendingWorkAmount3);
						}
						else if ((workVehicle.m_State & WorkVehicleFlags.CargoMoveVehicle) != 0)
						{
							CargoTransportStationData cargoTransportStationData3 = m_PrefabCargoTransportStationData[prefabRef2.m_Prefab];
							float lotWorkAmount3 = cargoTransportStation2.m_WorkAmount * cargoTransportStationData3.m_WorkMultiplier;
							CheckVehicle(vehicle, workVehicleData2, ref lotWorkAmount3, ref pendingWorkAmount4);
							if (lotWorkAmount3 != cargoTransportStation2.m_WorkAmount * cargoTransportStationData3.m_WorkMultiplier)
							{
								cargoTransportStation2.m_WorkAmount = lotWorkAmount3 / cargoTransportStationData3.m_WorkMultiplier;
							}
						}
					}
					else if (!m_PrefabRefData.HasComponent(vehicle))
					{
						val3.RemoveAtSwapBack(k--);
					}
				}
				if (nativeArray2.Length != 0)
				{
					ExtractorAreaData extractorAreaData2 = m_PrefabExtractorAreaData[prefabRef2.m_Prefab];
					if (extractorAreaData2.m_MapFeature == MapFeature.Forest)
					{
						extractor2.m_ExtractedAmount = extractor2.m_WorkAmount + pendingWorkAmount;
					}
					if (extractor2.m_WorkAmount >= 1000f)
					{
						FindTarget(unfilteredChunkIndex, ref random, val2, owner, extractorAreaData2.m_MapFeature, VehicleWorkType.Harvest, Resource.NoResource, WorkVehicleFlags.ExtractorVehicle, woodResources, lotData2.m_OnWater, ref extractor2.m_WorkAmount);
						extractor2.m_WorkType = VehicleWorkType.Harvest;
					}
					else if (extractor2.m_HarvestedAmount >= 1000f)
					{
						FindTarget(unfilteredChunkIndex, ref random, val2, owner, extractorAreaData2.m_MapFeature, VehicleWorkType.Collect, Resource.NoResource, WorkVehicleFlags.ExtractorVehicle, woodResources, lotData2.m_OnWater, ref extractor2.m_HarvestedAmount);
						extractor2.m_WorkType = VehicleWorkType.Collect;
					}
					nativeArray2[j] = extractor2;
				}
				if (nativeArray3.Length != 0)
				{
					StorageAreaData storageAreaData2 = m_PrefabStorageAreaData[prefabRef2.m_Prefab];
					if (storage2.m_WorkAmount >= 1000f)
					{
						FindTarget(unfilteredChunkIndex, ref random, val2, owner, MapFeature.None, VehicleWorkType.Collect, storageAreaData2.m_Resources, WorkVehicleFlags.StorageVehicle, woodResources, lotData2.m_OnWater, ref storage2.m_WorkAmount);
					}
					nativeArray3[j] = storage2;
				}
				if (nativeArray5.Length == 0)
				{
					continue;
				}
				CargoTransportStationData cargoTransportStationData4 = m_PrefabCargoTransportStationData[prefabRef2.m_Prefab];
				StorageCompanyData storageCompanyData2 = m_PrefabStorageCompanyData[prefabRef2.m_Prefab];
				if (cargoTransportStationData4.m_WorkMultiplier > 0f)
				{
					float extractorWorkAmount = cargoTransportStation2.m_WorkAmount * cargoTransportStationData4.m_WorkMultiplier;
					if (extractorWorkAmount >= 1000f)
					{
						FindTarget(unfilteredChunkIndex, ref random, val2, new Owner(val2), MapFeature.None, VehicleWorkType.Move, storageCompanyData2.m_StoredResources, WorkVehicleFlags.CargoMoveVehicle, woodResources, onWater: false, ref extractorWorkAmount);
						cargoTransportStation2.m_WorkAmount = extractorWorkAmount / cargoTransportStationData4.m_WorkMultiplier;
					}
				}
				else
				{
					cargoTransportStation2.m_WorkAmount = 0f;
				}
				nativeArray5[j] = cargoTransportStation2;
			}
		}

		private void CheckVehicle(Entity vehicle, WorkVehicleData workVehicleData, ref float lotWorkAmount, ref float pendingWorkAmount)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			Game.Vehicles.WorkVehicle workVehicle = m_WorkVehicleData[vehicle];
			if (lotWorkAmount >= 1f)
			{
				float num = (((workVehicle.m_State & WorkVehicleFlags.Returning) != 0) ? math.min(lotWorkAmount, workVehicle.m_DoneAmount - workVehicle.m_WorkAmount) : math.min(lotWorkAmount, workVehicleData.m_MaxWorkAmount - workVehicle.m_WorkAmount));
				if (num > 0f)
				{
					workVehicle.m_WorkAmount += num;
					lotWorkAmount -= num;
					m_WorkVehicleData[vehicle] = workVehicle;
				}
			}
			pendingWorkAmount += workVehicle.m_WorkAmount - workVehicle.m_DoneAmount;
		}

		private bool HasNavigation(Entity entity, RoadTypes roadTypes)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			if (HasNavigationSelf(entity, roadTypes))
			{
				return true;
			}
			DynamicBuffer<Game.Areas.SubArea> val = default(DynamicBuffer<Game.Areas.SubArea>);
			if (m_SubAreas.TryGetBuffer(entity, ref val))
			{
				for (int i = 0; i < val.Length; i++)
				{
					if (HasNavigationSelf(val[i].m_Area, roadTypes))
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool HasNavigationSelf(Entity entity, RoadTypes roadTypes)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = default(PrefabRef);
			NavigationAreaData navigationAreaData = default(NavigationAreaData);
			if (m_PrefabRefData.TryGetComponent(entity, ref prefabRef) && m_PrefabNavigationAreaData.TryGetComponent(prefabRef.m_Prefab, ref navigationAreaData))
			{
				return (navigationAreaData.m_RoadTypes & roadTypes) != 0;
			}
			return false;
		}

		private void FindTarget(int jobIndex, ref Random random, Entity entity, Owner owner, MapFeature mapFeature, VehicleWorkType workType, Resource resource, WorkVehicleFlags flags, DynamicBuffer<WoodResource> woodResources, bool onWater, ref float extractorWorkAmount)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			if (FindRoute(owner.m_Owner, out var route, out var targetWaypoint) && FindStartWaypoint(owner.m_Owner, route, out var firstWaypoint, out var nextWaypoint))
			{
				PathInformation pathInformation = new PathInformation
				{
					m_Origin = firstWaypoint,
					m_Destination = nextWaypoint
				};
				RoadTypes roadTypes = ((!onWater) ? RoadTypes.Car : RoadTypes.Watercraft);
				flags |= WorkVehicleFlags.RouteSource;
				if (targetWaypoint == nextWaypoint && HasNavigation(entity, roadTypes))
				{
					pathInformation.m_Destination = entity;
					flags |= WorkVehicleFlags.WorkLocation;
				}
				TrySpawnVehicle(jobIndex, ref random, entity, route, pathInformation, default(DynamicBuffer<PathElement>), workType, mapFeature, resource, flags, onWater, ref extractorWorkAmount);
				return;
			}
			Entity val = Entity.Null;
			Attachment attachment = default(Attachment);
			if (m_AttachmentData.TryGetComponent(owner.m_Owner, ref attachment))
			{
				val = attachment.m_Attached;
			}
			else if (m_TransformData.HasComponent(owner.m_Owner))
			{
				val = owner.m_Owner;
			}
			if (val != Entity.Null)
			{
				if (mapFeature == MapFeature.Forest)
				{
					if (woodResources.IsCreated && woodResources.Length != 0)
					{
						FindTarget(jobIndex, entity, val, SetupTargetType.WoodResource, workType, onWater);
					}
					else
					{
						extractorWorkAmount = 0f;
					}
				}
				else
				{
					FindTarget(jobIndex, entity, val, SetupTargetType.AreaLocation, workType, onWater);
				}
			}
			else
			{
				extractorWorkAmount = 0f;
			}
		}

		private bool FindRoute(Entity areaOwner, out Entity route, out Entity targetWaypoint)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			Entity val = areaOwner;
			Owner owner = default(Owner);
			if (m_OwnerData.TryGetComponent(areaOwner, ref owner))
			{
				val = owner.m_Owner;
			}
			int num = int.MaxValue;
			route = Entity.Null;
			targetWaypoint = Entity.Null;
			DynamicBuffer<SubRoute> val2 = default(DynamicBuffer<SubRoute>);
			if (m_SubRoutes.TryGetBuffer(val, ref val2))
			{
				DynamicBuffer<RouteWaypoint> val3 = default(DynamicBuffer<RouteWaypoint>);
				Connected connected = default(Connected);
				DynamicBuffer<RouteVehicle> val4 = default(DynamicBuffer<RouteVehicle>);
				for (int i = 0; i < val2.Length; i++)
				{
					SubRoute subRoute = val2[i];
					if (!m_WorkRouteData.HasComponent(subRoute.m_Route) || !m_RouteWaypoints.TryGetBuffer(subRoute.m_Route, ref val3))
					{
						continue;
					}
					for (int j = 0; j < val3.Length; j++)
					{
						RouteWaypoint routeWaypoint = val3[j];
						if (m_ConnectedData.TryGetComponent(routeWaypoint.m_Waypoint, ref connected) && m_OwnerData.TryGetComponent(connected.m_Connected, ref owner) && owner.m_Owner == areaOwner)
						{
							int num2 = 0;
							if (m_RouteVehicles.TryGetBuffer(subRoute.m_Route, ref val4))
							{
								num2 = val4.Length;
							}
							if (num2 < num)
							{
								num = num2;
								route = subRoute.m_Route;
								targetWaypoint = routeWaypoint.m_Waypoint;
							}
							break;
						}
					}
				}
			}
			return route != Entity.Null;
		}

		private bool FindStartWaypoint(Entity areaOwner, Entity route, out Entity firstWaypoint, out Entity nextWaypoint)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<RouteWaypoint> val = default(DynamicBuffer<RouteWaypoint>);
			if (m_RouteWaypoints.TryGetBuffer(route, ref val))
			{
				Connected connected = default(Connected);
				Owner owner = default(Owner);
				for (int i = 0; i < val.Length; i++)
				{
					RouteWaypoint routeWaypoint = val[i];
					if (m_ConnectedData.TryGetComponent(routeWaypoint.m_Waypoint, ref connected) && m_OwnerData.TryGetComponent(connected.m_Connected, ref owner) && owner.m_Owner != areaOwner)
					{
						int num = i + 1;
						num = math.select(num, 0, num >= val.Length);
						firstWaypoint = routeWaypoint.m_Waypoint;
						nextWaypoint = val[num].m_Waypoint;
						return true;
					}
				}
			}
			firstWaypoint = Entity.Null;
			nextWaypoint = Entity.Null;
			return false;
		}

		private void FindTarget(int jobIndex, Entity owner, Entity source, SetupTargetType targetType, VehicleWorkType workType, bool onWater)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			RoadTypes roadTypes = ((!onWater) ? RoadTypes.Car : RoadTypes.Watercraft);
			PathMethod pathMethod = PathMethod.Road | PathMethod.Offroad | PathMethod.MediumRoad;
			PathfindParameters parameters = new PathfindParameters
			{
				m_MaxSpeed = float2.op_Implicit(277.77777f),
				m_WalkSpeed = float2.op_Implicit(5.555556f),
				m_Weights = new PathfindWeights(1f, 1f, 1f, 1f),
				m_Methods = pathMethod
			};
			if (workType == VehicleWorkType.Move)
			{
				pathMethod |= PathMethod.CargoLoading;
			}
			SetupQueueTarget origin = new SetupQueueTarget
			{
				m_Type = SetupTargetType.CurrentLocation,
				m_Methods = pathMethod,
				m_RoadTypes = roadTypes,
				m_Entity = source
			};
			SetupQueueTarget destination = new SetupQueueTarget
			{
				m_Type = targetType,
				m_Methods = pathMethod,
				m_RoadTypes = roadTypes,
				m_Entity = owner,
				m_Value = (int)workType
			};
			m_PathfindQueue.Enqueue(new SetupQueueItem(owner, parameters, origin, destination));
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathInformation>(jobIndex, owner, default(PathInformation));
			((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<PathElement>(jobIndex, owner);
		}

		private void TrySpawnVehicle(int jobIndex, ref Random random, Entity owner, Entity route, PathInformation pathInformation, DynamicBuffer<PathElement> path, VehicleWorkType workType, MapFeature mapFeature, Resource resource, WorkVehicleFlags flags, bool onWater, ref float lotWorkAmount)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			if (pathInformation.m_Destination != Entity.Null)
			{
				float workAmount = lotWorkAmount;
				if (SpawnVehicle(jobIndex, ref random, owner, route, pathInformation, path, workType, mapFeature, resource, flags, onWater, ref workAmount))
				{
					lotWorkAmount -= workAmount;
				}
				else
				{
					lotWorkAmount = 0f;
				}
			}
			else
			{
				lotWorkAmount = 0f;
			}
		}

		private bool SpawnVehicle(int jobIndex, ref Random random, Entity owner, Entity route, PathInformation pathInformation, DynamicBuffer<PathElement> path, VehicleWorkType workType, MapFeature mapFeature, Resource resource, WorkVehicleFlags flags, bool onWater, ref float workAmount)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			Entity val = pathInformation.m_Origin;
			Connected connected = default(Connected);
			if (route != Entity.Null && m_ConnectedData.TryGetComponent(val, ref connected))
			{
				val = connected.m_Connected;
			}
			Transform transform = default(Transform);
			if (!m_TransformData.TryGetComponent(val, ref transform))
			{
				return false;
			}
			RoadTypes roadTypes = ((!onWater) ? RoadTypes.Car : RoadTypes.Watercraft);
			SizeClass sizeClass = SizeClass.Undefined;
			Entity val2 = m_WorkVehicleSelectData.CreateVehicle(m_CommandBuffer, jobIndex, ref random, roadTypes, sizeClass, workType, mapFeature, resource, ref workAmount, transform, pathInformation.m_Origin, flags);
			if (val2 != Entity.Null)
			{
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Target>(jobIndex, val2, new Target(pathInformation.m_Destination));
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(jobIndex, val2, new Owner(owner));
				if (route != Entity.Null)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<CurrentRoute>(jobIndex, val2, new CurrentRoute(route));
				}
				else if (path.IsCreated && path.Length != 0)
				{
					DynamicBuffer<PathElement> targetElements = ((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<PathElement>(jobIndex, val2);
					PathUtils.CopyPath(path, default(PathOwner), 0, targetElements);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PathOwner>(jobIndex, val2, new PathOwner(PathFlags.Updated));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PathInformation>(jobIndex, val2, pathInformation);
				}
				return true;
			}
			return false;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct ExtractResourcesJob : IJob
	{
		private struct AreaIterator : INativeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>
		{
			public Bounds2 m_Bounds;

			public ComponentLookup<Extractor> m_ExtractorData;

			public BufferLookup<MapFeatureElement> m_MapFeatureElements;

			public NativeParallelHashSet<Entity> m_UpdateSet;

			public NativeList<Entity> m_UpdateList;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, AreaSearchItem item)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds) && (m_ExtractorData.HasComponent(item.m_Area) || m_MapFeatureElements.HasBuffer(item.m_Area)) && m_UpdateSet.Add(item.m_Area))
				{
					m_UpdateList.Add(ref item.m_Area);
				}
			}
		}

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<Game.Areas.Node> m_NodeType;

		[ReadOnly]
		public BufferTypeHandle<Triangle> m_TriangleType;

		[ReadOnly]
		public ComponentLookup<ExtractorAreaData> m_ExtractorAreaData;

		[ReadOnly]
		public BufferLookup<MapFeatureElement> m_MapFeatureElements;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifiers;

		public ComponentLookup<Extractor> m_ExtractorData;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public NativeQuadTree<AreaSearchItem, QuadTreeBoundsXZ> m_AreaTree;

		[ReadOnly]
		public Entity m_City;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public ExtractorParameterData m_ExtractorParameters;

		public CellMapData<NaturalResourceCell> m_NaturalResourceData;

		public NativeList<Entity> m_UpdateList;

		public void Execute()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			NativeParallelHashSet<Entity> updateSet = default(NativeParallelHashSet<Entity>);
			updateSet._002Ector(16, AllocatorHandle.op_Implicit((Allocator)2));
			AreaIterator iterator = new AreaIterator
			{
				m_ExtractorData = m_ExtractorData,
				m_MapFeatureElements = m_MapFeatureElements,
				m_UpdateSet = updateSet,
				m_UpdateList = m_UpdateList
			};
			Random random = m_RandomSeed.GetRandom(0);
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				ArchetypeChunk val = m_Chunks[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				BufferAccessor<Game.Areas.Node> bufferAccessor = ((ArchetypeChunk)(ref val)).GetBufferAccessor<Game.Areas.Node>(ref m_NodeType);
				BufferAccessor<Triangle> bufferAccessor2 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<Triangle>(ref m_TriangleType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Entity val2 = nativeArray[j];
					PrefabRef prefabRef = nativeArray2[j];
					ExtractorAreaData extractorAreaData = m_ExtractorAreaData[prefabRef.m_Prefab];
					if (extractorAreaData.m_MapFeature == MapFeature.Forest)
					{
						if (updateSet.Add(val2))
						{
							m_UpdateList.Add(ref val2);
						}
						continue;
					}
					Extractor extractor = m_ExtractorData[val2];
					if (extractor.m_ExtractedAmount >= math.max(1f, (extractorAreaData.m_MapFeature == MapFeature.Ore || extractorAreaData.m_MapFeature == MapFeature.Oil) ? 1f : (extractor.m_ResourceAmount * 0.001f)))
					{
						switch (extractorAreaData.m_MapFeature)
						{
						case MapFeature.FertileLand:
						case MapFeature.Oil:
						case MapFeature.Ore:
						case MapFeature.Fish:
						{
							DynamicBuffer<CityModifier> cityModifiers = m_CityModifiers[m_City];
							ExtractNaturalResources(ref random, ref iterator, bufferAccessor[j], bufferAccessor2[j], cityModifiers, ref extractor, extractorAreaData.m_MapFeature);
							break;
						}
						}
						m_ExtractorData[val2] = extractor;
					}
				}
			}
			updateSet.Dispose();
		}

		private int GetUnlimitedUsage(float originalConcentration, float currentConcentration, float mu, ref Random random, int extractedAmount)
		{
			float num = math.log(originalConcentration) - math.log(currentConcentration);
			return MathUtils.RoundToIntRandom(ref random, mu * originalConcentration * math.exp(0f - num) * (float)extractedAmount * 10000f);
		}

		private void ExtractNaturalResources(ref Random random, ref AreaIterator iterator, DynamicBuffer<Game.Areas.Node> nodes, DynamicBuffer<Triangle> triangles, DynamicBuffer<CityModifier> cityModifiers, ref Extractor extractor, MapFeature mapFeature)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_059c: Unknown result type (might be due to invalid IL or missing references)
			//IL_059e: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			float2 val = 1f / m_NaturalResourceData.m_CellSize;
			float4 xyxy = ((float2)(ref val)).xyxy;
			val = float2.op_Implicit(m_NaturalResourceData.m_TextureSize) * 0.5f;
			float4 xyxy2 = ((float2)(ref val)).xyxy;
			float num = 1f / (m_NaturalResourceData.m_CellSize.x * m_NaturalResourceData.m_CellSize.y);
			int num2 = Mathf.FloorToInt(extractor.m_ExtractedAmount);
			Bounds2 val5 = default(Bounds2);
			float num12 = default(float);
			do
			{
				int num3 = -1;
				int num4 = 0;
				float num5 = 0f;
				bool flag = mapFeature == MapFeature.Ore || mapFeature == MapFeature.Oil;
				Bounds2 bounds = default(Bounds2);
				for (int i = 0; i < triangles.Length; i++)
				{
					Triangle2 triangle = AreaUtils.GetTriangle2(nodes, triangles[i]);
					Bounds2 val2 = MathUtils.Bounds(triangle);
					int4 val3 = (int4)math.floor(new float4(val2.min, val2.max) * xyxy + xyxy2);
					val3 = math.clamp(val3, int4.op_Implicit(0), ((int2)(ref m_NaturalResourceData.m_TextureSize)).xyxy - 1);
					float num6 = 0f;
					float num7 = 0f;
					Bounds2 val4 = default(Bounds2);
					int num8 = 0;
					float num9 = 0f;
					for (int j = val3.y; j <= val3.w; j++)
					{
						val5.min.y = ((float)j - xyxy2.y) * m_NaturalResourceData.m_CellSize.y;
						val5.max.y = val5.min.y + m_NaturalResourceData.m_CellSize.y;
						for (int k = val3.x; k <= val3.z; k++)
						{
							int num10 = k + m_NaturalResourceData.m_TextureSize.x * j;
							NaturalResourceCell naturalResourceCell = m_NaturalResourceData.m_Buffer[num10];
							float num11;
							switch (mapFeature)
							{
							case MapFeature.FertileLand:
								num11 = (int)naturalResourceCell.m_Fertility.m_Base;
								num11 -= (float)(int)naturalResourceCell.m_Fertility.m_Used;
								break;
							case MapFeature.Ore:
								num11 = (int)naturalResourceCell.m_Ore.m_Base;
								CityUtils.ApplyModifier(ref num11, cityModifiers, CityModifierType.OreResourceAmount);
								num11 -= (float)(int)naturalResourceCell.m_Ore.m_Used;
								break;
							case MapFeature.Oil:
								num11 = (int)naturalResourceCell.m_Oil.m_Base;
								CityUtils.ApplyModifier(ref num11, cityModifiers, CityModifierType.OilResourceAmount);
								num11 -= (float)(int)naturalResourceCell.m_Oil.m_Used;
								break;
							case MapFeature.Fish:
								num11 = (int)naturalResourceCell.m_Fish.m_Base;
								num11 -= (float)(int)naturalResourceCell.m_Fish.m_Used;
								break;
							default:
								num11 = 0f;
								break;
							}
							num11 = math.clamp(num11, 0f, 65535f);
							if (num11 == 0f)
							{
								continue;
							}
							val5.min.x = ((float)k - xyxy2.x) * m_NaturalResourceData.m_CellSize.x;
							val5.max.x = val5.min.x + m_NaturalResourceData.m_CellSize.x;
							if (MathUtils.Intersect(val5, triangle, ref num12))
							{
								num6 += num12 * ((Random)(ref random)).NextFloat(0.99f, 1.01f) * math.min(num11 * 0.0001f, 1f);
								num7 += num12;
								if (num11 * num12 * num > num9)
								{
									num9 = num11 * num12 * num;
									num8 = num10;
									val4 = val5;
								}
							}
						}
					}
					num6 = ((num7 > 0.01f) ? (num6 / num7) : 0f);
					if (num6 > num5)
					{
						num3 = num8;
						num4 = (flag ? num2 : math.min(Mathf.RoundToInt(num9), num2));
						num5 = num6;
						bounds = val4;
					}
				}
				if (num4 > 0)
				{
					NaturalResourceCell naturalResourceCell2 = m_NaturalResourceData.m_Buffer[num3];
					switch (mapFeature)
					{
					case MapFeature.FertileLand:
						naturalResourceCell2.m_Fertility.m_Used = (ushort)math.min(65535, naturalResourceCell2.m_Fertility.m_Used + num4);
						break;
					case MapFeature.Ore:
					{
						float originalConcentration2 = (float)(int)naturalResourceCell2.m_Ore.m_Base * 0.0001f;
						float currentConcentration2 = (float)(naturalResourceCell2.m_Ore.m_Base - naturalResourceCell2.m_Ore.m_Used) * 0.0001f;
						int unlimitedUsage2 = GetUnlimitedUsage(originalConcentration2, currentConcentration2, 1f / m_ExtractorParameters.m_OreConsumption, ref random, num4);
						naturalResourceCell2.m_Ore.m_Used = (ushort)math.min(65535, naturalResourceCell2.m_Ore.m_Used + unlimitedUsage2);
						break;
					}
					case MapFeature.Oil:
					{
						float originalConcentration = (float)(int)naturalResourceCell2.m_Oil.m_Base * 0.0001f;
						float currentConcentration = (float)(naturalResourceCell2.m_Oil.m_Base - naturalResourceCell2.m_Oil.m_Used) * 0.0001f;
						int unlimitedUsage = GetUnlimitedUsage(originalConcentration, currentConcentration, 1f / m_ExtractorParameters.m_OilConsumption, ref random, num4);
						naturalResourceCell2.m_Oil.m_Used = (ushort)math.min(65535, naturalResourceCell2.m_Oil.m_Used + unlimitedUsage);
						break;
					}
					case MapFeature.Fish:
						naturalResourceCell2.m_Fish.m_Used = (ushort)math.min(65535, naturalResourceCell2.m_Fish.m_Used + num4);
						break;
					}
					m_NaturalResourceData.m_Buffer[num3] = naturalResourceCell2;
					extractor.m_ExtractedAmount -= num4;
					iterator.m_Bounds = bounds;
					m_AreaTree.Iterate<AreaIterator>(ref iterator, 0);
					num2 = Mathf.FloorToInt(extractor.m_ExtractedAmount);
					continue;
				}
				break;
			}
			while (num2 > 0);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<WoodResource> __Game_Areas_WoodResource_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<PathElement> __Game_Pathfind_PathElement_RO_BufferTypeHandle;

		public ComponentTypeHandle<Extractor> __Game_Areas_Extractor_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Storage> __Game_Areas_Storage_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Game.Buildings.CargoTransportStation> __Game_Buildings_CargoTransportStation_RW_ComponentTypeHandle;

		public BufferTypeHandle<OwnedVehicle> __Game_Vehicles_OwnedVehicle_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Attachment> __Game_Objects_Attachment_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Connected> __Game_Routes_Connected_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WorkRoute> __Game_Routes_WorkRoute_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ExtractorAreaData> __Game_Prefabs_ExtractorAreaData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StorageAreaData> __Game_Prefabs_StorageAreaData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LotData> __Game_Prefabs_LotData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WorkVehicleData> __Game_Prefabs_WorkVehicleData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NavigationAreaData> __Game_Prefabs_NavigationAreaData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CargoTransportStationData> __Game_Prefabs_CargoTransportStationData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StorageCompanyData> __Game_Prefabs_StorageCompanyData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> __Game_Areas_SubArea_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubRoute> __Game_Routes_SubRoute_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> __Game_Routes_RouteWaypoint_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<RouteVehicle> __Game_Routes_RouteVehicle_RO_BufferLookup;

		public ComponentLookup<Game.Vehicles.WorkVehicle> __Game_Vehicles_WorkVehicle_RW_ComponentLookup;

		[ReadOnly]
		public BufferTypeHandle<Game.Areas.Node> __Game_Areas_Node_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Triangle> __Game_Areas_Triangle_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferLookup<MapFeatureElement> __Game_Areas_MapFeatureElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<CityModifier> __Game_City_CityModifier_RO_BufferLookup;

		public ComponentLookup<Extractor> __Game_Areas_Extractor_RW_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Geometry> __Game_Areas_Geometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Tree> __Game_Objects_Tree_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Plant> __Game_Objects_Plant_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Damaged> __Game_Objects_Damaged_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TreeData> __Game_Prefabs_TreeData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> __Game_Areas_Node_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Triangle> __Game_Areas_Triangle_RO_BufferLookup;

		public BufferLookup<WoodResource> __Game_Areas_WoodResource_RW_BufferLookup;

		public BufferLookup<MapFeatureElement> __Game_Areas_MapFeatureElement_RW_BufferLookup;

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
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Pathfind_PathInformation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathInformation>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Areas_WoodResource_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<WoodResource>(true);
			__Game_Pathfind_PathElement_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<PathElement>(true);
			__Game_Areas_Extractor_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Extractor>(false);
			__Game_Areas_Storage_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Storage>(false);
			__Game_Buildings_CargoTransportStation_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.CargoTransportStation>(false);
			__Game_Vehicles_OwnedVehicle_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<OwnedVehicle>(false);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Objects_Attachment_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attachment>(true);
			__Game_Routes_Connected_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Connected>(true);
			__Game_Routes_WorkRoute_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WorkRoute>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ExtractorAreaData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ExtractorAreaData>(true);
			__Game_Prefabs_StorageAreaData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StorageAreaData>(true);
			__Game_Prefabs_LotData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LotData>(true);
			__Game_Prefabs_WorkVehicleData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WorkVehicleData>(true);
			__Game_Prefabs_NavigationAreaData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NavigationAreaData>(true);
			__Game_Prefabs_CargoTransportStationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CargoTransportStationData>(true);
			__Game_Prefabs_StorageCompanyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StorageCompanyData>(true);
			__Game_Areas_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.SubArea>(true);
			__Game_Vehicles_LayoutElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(true);
			__Game_Routes_SubRoute_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubRoute>(true);
			__Game_Routes_RouteWaypoint_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteWaypoint>(true);
			__Game_Routes_RouteVehicle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteVehicle>(true);
			__Game_Vehicles_WorkVehicle_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.WorkVehicle>(false);
			__Game_Areas_Node_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Game.Areas.Node>(true);
			__Game_Areas_Triangle_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Triangle>(true);
			__Game_Areas_MapFeatureElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<MapFeatureElement>(true);
			__Game_City_CityModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(true);
			__Game_Areas_Extractor_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Extractor>(false);
			__Game_Areas_Geometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Geometry>(true);
			__Game_Objects_Tree_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Tree>(true);
			__Game_Objects_Plant_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Plant>(true);
			__Game_Objects_Damaged_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Damaged>(true);
			__Game_Prefabs_TreeData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TreeData>(true);
			__Game_Areas_Node_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.Node>(true);
			__Game_Areas_Triangle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Triangle>(true);
			__Game_Areas_WoodResource_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<WoodResource>(false);
			__Game_Areas_MapFeatureElement_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<MapFeatureElement>(false);
		}
	}

	private const uint UPDATE_INTERVAL = 512u;

	private Game.Objects.SearchSystem m_ObjectSearchSystem;

	private Game.Areas.SearchSystem m_AreaSearchSystem;

	private NaturalResourceSystem m_NaturalResourceSystem;

	private PathfindSetupSystem m_PathfindSetupSystem;

	private CitySystem m_CitySystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private TerrainSystem m_TerrainSystem;

	private WaterSystem m_Watersystem;

	private GroundWaterSystem m_GroundWaterSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_AreaQuery;

	private EntityQuery m_ExtractorQuery;

	private EntityQuery m_VehiclePrefabQuery;

	private EntityQuery m_ExtractorParameterQuery;

	private WorkVehicleSelectData m_WorkVehicleSelectData;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 512;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Expected O, but got Unknown
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ObjectSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.SearchSystem>();
		m_AreaSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Areas.SearchSystem>();
		m_NaturalResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NaturalResourceSystem>();
		m_PathfindSetupSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindSetupSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_Watersystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_GroundWaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GroundWaterSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_WorkVehicleSelectData = new WorkVehicleSelectData((SystemBase)(object)this);
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<Extractor>(),
			ComponentType.ReadWrite<Storage>(),
			ComponentType.ReadWrite<Game.Buildings.CargoTransportStation>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array[0] = val;
		m_AreaQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Areas.Lot>(),
			ComponentType.ReadWrite<Extractor>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array2[0] = val;
		m_ExtractorQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		m_VehiclePrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)new EntityQueryDesc[1] { WorkVehicleSelectData.GetEntityQueryDesc() });
		m_ExtractorParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ExtractorParameterData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_AreaQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_053a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0541: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_059b: Unknown result type (might be due to invalid IL or missing references)
		//IL_059d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_061c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0621: Unknown result type (might be due to invalid IL or missing references)
		//IL_0639: Unknown result type (might be due to invalid IL or missing references)
		//IL_063e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0656: Unknown result type (might be due to invalid IL or missing references)
		//IL_065b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0673: Unknown result type (might be due to invalid IL or missing references)
		//IL_0678: Unknown result type (might be due to invalid IL or missing references)
		//IL_0690: Unknown result type (might be due to invalid IL or missing references)
		//IL_0695: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0704: Unknown result type (might be due to invalid IL or missing references)
		//IL_0709: Unknown result type (might be due to invalid IL or missing references)
		//IL_0721: Unknown result type (might be due to invalid IL or missing references)
		//IL_0726: Unknown result type (might be due to invalid IL or missing references)
		//IL_073e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0743: Unknown result type (might be due to invalid IL or missing references)
		//IL_075b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0760: Unknown result type (might be due to invalid IL or missing references)
		//IL_0778: Unknown result type (might be due to invalid IL or missing references)
		//IL_077d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0795: Unknown result type (might be due to invalid IL or missing references)
		//IL_079a: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07db: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0800: Unknown result type (might be due to invalid IL or missing references)
		//IL_0802: Unknown result type (might be due to invalid IL or missing references)
		//IL_080e: Unknown result type (might be due to invalid IL or missing references)
		//IL_081b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0828: Unknown result type (might be due to invalid IL or missing references)
		//IL_0835: Unknown result type (might be due to invalid IL or missing references)
		//IL_0842: Unknown result type (might be due to invalid IL or missing references)
		//IL_084f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0857: Unknown result type (might be due to invalid IL or missing references)
		m_WorkVehicleSelectData.PreUpdate((SystemBase)(object)this, m_CityConfigurationSystem, m_VehiclePrefabQuery, (Allocator)3, out var jobHandle);
		ManageVehiclesJob manageVehiclesJob = new ManageVehiclesJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathInformationType = InternalCompilerInterface.GetComponentTypeHandle<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WoodResourceType = InternalCompilerInterface.GetBufferTypeHandle<WoodResource>(ref __TypeHandle.__Game_Areas_WoodResource_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathElementType = InternalCompilerInterface.GetBufferTypeHandle<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ExtractorType = InternalCompilerInterface.GetComponentTypeHandle<Extractor>(ref __TypeHandle.__Game_Areas_Extractor_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_StorageType = InternalCompilerInterface.GetComponentTypeHandle<Storage>(ref __TypeHandle.__Game_Areas_Storage_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CargoTransportStationType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.CargoTransportStation>(ref __TypeHandle.__Game_Buildings_CargoTransportStation_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnedVehicleType = InternalCompilerInterface.GetBufferTypeHandle<OwnedVehicle>(ref __TypeHandle.__Game_Vehicles_OwnedVehicle_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AttachmentData = InternalCompilerInterface.GetComponentLookup<Attachment>(ref __TypeHandle.__Game_Objects_Attachment_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedData = InternalCompilerInterface.GetComponentLookup<Connected>(ref __TypeHandle.__Game_Routes_Connected_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WorkRouteData = InternalCompilerInterface.GetComponentLookup<WorkRoute>(ref __TypeHandle.__Game_Routes_WorkRoute_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabExtractorAreaData = InternalCompilerInterface.GetComponentLookup<ExtractorAreaData>(ref __TypeHandle.__Game_Prefabs_ExtractorAreaData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabStorageAreaData = InternalCompilerInterface.GetComponentLookup<StorageAreaData>(ref __TypeHandle.__Game_Prefabs_StorageAreaData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabLotData = InternalCompilerInterface.GetComponentLookup<LotData>(ref __TypeHandle.__Game_Prefabs_LotData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabWorkVehicleData = InternalCompilerInterface.GetComponentLookup<WorkVehicleData>(ref __TypeHandle.__Game_Prefabs_WorkVehicleData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabNavigationAreaData = InternalCompilerInterface.GetComponentLookup<NavigationAreaData>(ref __TypeHandle.__Game_Prefabs_NavigationAreaData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCargoTransportStationData = InternalCompilerInterface.GetComponentLookup<CargoTransportStationData>(ref __TypeHandle.__Game_Prefabs_CargoTransportStationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabStorageCompanyData = InternalCompilerInterface.GetComponentLookup<StorageCompanyData>(ref __TypeHandle.__Game_Prefabs_StorageCompanyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubAreas = InternalCompilerInterface.GetBufferLookup<Game.Areas.SubArea>(ref __TypeHandle.__Game_Areas_SubArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_VehicleLayouts = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubRoutes = InternalCompilerInterface.GetBufferLookup<SubRoute>(ref __TypeHandle.__Game_Routes_SubRoute_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteWaypoints = InternalCompilerInterface.GetBufferLookup<RouteWaypoint>(ref __TypeHandle.__Game_Routes_RouteWaypoint_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteVehicles = InternalCompilerInterface.GetBufferLookup<RouteVehicle>(ref __TypeHandle.__Game_Routes_RouteVehicle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WorkVehicleData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.WorkVehicle>(ref __TypeHandle.__Game_Vehicles_WorkVehicle_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RandomSeed = RandomSeed.Next(),
			m_WorkVehicleSelectData = m_WorkVehicleSelectData
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		manageVehiclesJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		manageVehiclesJob.m_PathfindQueue = m_PathfindSetupSystem.GetQueue(this, 512).AsParallelWriter();
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<ManageVehiclesJob>(manageVehiclesJob, m_AreaQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, jobHandle));
		m_WorkVehicleSelectData.PostUpdate(val2);
		m_EndFrameBarrier.AddJobHandleForProducer(val2);
		m_PathfindSetupSystem.AddQueueWriter(val2);
		((SystemBase)this).Dependency = val2;
		if (!((EntityQuery)(ref m_ExtractorQuery)).IsEmptyIgnoreFilter)
		{
			NativeList<Entity> val3 = default(NativeList<Entity>);
			val3._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			JobHandle dependencies;
			CellMapData<NaturalResourceCell> data = m_NaturalResourceSystem.GetData(readOnly: false, out dependencies);
			JobHandle val4 = default(JobHandle);
			JobHandle dependencies2;
			ExtractResourcesJob extractResourcesJob = new ExtractResourcesJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_NodeType = InternalCompilerInterface.GetBufferTypeHandle<Game.Areas.Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TriangleType = InternalCompilerInterface.GetBufferTypeHandle<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_MapFeatureElements = InternalCompilerInterface.GetBufferLookup<MapFeatureElement>(ref __TypeHandle.__Game_Areas_MapFeatureElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CityModifiers = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ExtractorAreaData = InternalCompilerInterface.GetComponentLookup<ExtractorAreaData>(ref __TypeHandle.__Game_Prefabs_ExtractorAreaData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ExtractorData = InternalCompilerInterface.GetComponentLookup<Extractor>(ref __TypeHandle.__Game_Areas_Extractor_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Chunks = ((EntityQuery)(ref m_ExtractorQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val4),
				m_AreaTree = m_AreaSearchSystem.GetSearchTree(readOnly: true, out dependencies2),
				m_City = m_CitySystem.City,
				m_RandomSeed = RandomSeed.Next(),
				m_ExtractorParameters = ((EntityQuery)(ref m_ExtractorParameterQuery)).GetSingleton<ExtractorParameterData>(),
				m_NaturalResourceData = data,
				m_UpdateList = val3
			};
			JobHandle dependencies3;
			JobHandle dependencies4;
			JobHandle deps;
			AreaResourceSystem.UpdateAreaResourcesJob obj = new AreaResourceSystem.UpdateAreaResourcesJob
			{
				m_City = m_CitySystem.City,
				m_FullUpdate = false,
				m_UpdateList = val3.AsDeferredJobArray(),
				m_ObjectTree = m_ObjectSearchSystem.GetStaticSearchTree(readOnly: true, out dependencies3),
				m_NaturalResourceData = data,
				m_GroundWaterResourceData = m_GroundWaterSystem.GetData(readOnly: true, out dependencies4),
				m_GeometryData = InternalCompilerInterface.GetComponentLookup<Geometry>(ref __TypeHandle.__Game_Areas_Geometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TreeData = InternalCompilerInterface.GetComponentLookup<Tree>(ref __TypeHandle.__Game_Objects_Tree_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PlantData = InternalCompilerInterface.GetComponentLookup<Plant>(ref __TypeHandle.__Game_Objects_Plant_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_DamagedData = InternalCompilerInterface.GetComponentLookup<Damaged>(ref __TypeHandle.__Game_Objects_Damaged_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ExtractorAreaData = InternalCompilerInterface.GetComponentLookup<ExtractorAreaData>(ref __TypeHandle.__Game_Prefabs_ExtractorAreaData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabTreeData = InternalCompilerInterface.GetComponentLookup<TreeData>(ref __TypeHandle.__Game_Prefabs_TreeData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Nodes = InternalCompilerInterface.GetBufferLookup<Game.Areas.Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Triangles = InternalCompilerInterface.GetBufferLookup<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CityModifiers = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ExtractorData = InternalCompilerInterface.GetComponentLookup<Extractor>(ref __TypeHandle.__Game_Areas_Extractor_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_WoodResources = InternalCompilerInterface.GetBufferLookup<WoodResource>(ref __TypeHandle.__Game_Areas_WoodResource_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_MapFeatureElements = InternalCompilerInterface.GetBufferLookup<MapFeatureElement>(ref __TypeHandle.__Game_Areas_MapFeatureElement_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
				m_WaterSurfaceData = m_Watersystem.GetSurfaceData(out deps)
			};
			JobHandle val5 = IJobExtensions.Schedule<ExtractResourcesJob>(extractResourcesJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, JobHandle.CombineDependencies(val4, dependencies2, dependencies)));
			JobHandle val6 = IJobParallelForDeferExtensions.Schedule<AreaResourceSystem.UpdateAreaResourcesJob, Entity>(obj, val3, 1, JobUtils.CombineDependencies(val5, dependencies3, deps, dependencies4));
			val3.Dispose(val6);
			m_AreaSearchSystem.AddSearchTreeReader(val5);
			m_ObjectSearchSystem.AddStaticSearchTreeReader(val6);
			m_NaturalResourceSystem.AddWriter(val6);
			m_TerrainSystem.AddCPUHeightReader(val6);
			m_Watersystem.AddSurfaceReader(val6);
			m_GroundWaterSystem.AddReader(val6);
			((SystemBase)this).Dependency = val6;
		}
	}

	public static int GetUnlimitedTotalAmount(int used, int originalAmount, float mu)
	{
		return Mathf.RoundToInt(math.log((float)(originalAmount / 10000)) - math.log((float)((originalAmount - used) / 10000)) / mu);
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
	public AreaLotSimulationSystem()
	{
	}
}
