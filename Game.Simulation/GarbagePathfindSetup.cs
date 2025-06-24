using Game.Areas;
using Game.Buildings;
using Game.City;
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

namespace Game.Simulation;

public struct GarbagePathfindSetup
{
	[BurstCompile]
	private struct SetupGarbageCollectorsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.GarbageFacility> m_GarbageFacilityType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Vehicles.GarbageTruck> m_GarbageTruckType;

		[ReadOnly]
		public ComponentTypeHandle<PathOwner> m_PathOwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public BufferTypeHandle<PathElement> m_PathElementType;

		[ReadOnly]
		public BufferTypeHandle<ServiceDispatch> m_ServiceDispatchType;

		[ReadOnly]
		public ComponentLookup<PathInformation> m_PathInformationData;

		[ReadOnly]
		public ComponentLookup<GarbageCollectionRequest> m_GarbageCollectionRequestData;

		[ReadOnly]
		public BufferLookup<PathElement> m_PathElements;

		[ReadOnly]
		public BufferLookup<ServiceDistrict> m_ServiceDistricts;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> m_OutsideConnections;

		[ReadOnly]
		public ComponentLookup<Game.City.City> m_CityData;

		[ReadOnly]
		public Entity m_City;

		public PathfindSetupSystem.SetupData m_SetupData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03de: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).Has<Game.Objects.OutsideConnection>() && !CityUtils.CheckOption(m_CityData[m_City], CityOption.ImportOutsideServices))
			{
				return;
			}
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Game.Buildings.GarbageFacility> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.GarbageFacility>(ref m_GarbageFacilityType);
			if (nativeArray2.Length != 0)
			{
				for (int i = 0; i < nativeArray2.Length; i++)
				{
					Game.Buildings.GarbageFacility garbageFacility = nativeArray2[i];
					if ((garbageFacility.m_Flags & (GarbageFacilityFlags.HasAvailableGarbageTrucks | GarbageFacilityFlags.HasAvailableSpace)) != (GarbageFacilityFlags.HasAvailableGarbageTrucks | GarbageFacilityFlags.HasAvailableSpace))
					{
						continue;
					}
					for (int j = 0; j < m_SetupData.Length; j++)
					{
						m_SetupData.GetItem(j, out var entity, out var owner, out var targetSeeker);
						GarbageCollectionRequest garbageCollectionRequest = default(GarbageCollectionRequest);
						if (m_GarbageCollectionRequestData.HasComponent(owner))
						{
							garbageCollectionRequest = m_GarbageCollectionRequestData[owner];
						}
						float num = targetSeeker.m_PathfindParameters.m_Weights.time * 10f;
						if ((garbageFacility.m_Flags & GarbageFacilityFlags.IndustrialWasteOnly) != 0)
						{
							if ((garbageCollectionRequest.m_Flags & GarbageCollectionRequestFlags.IndustrialWaste) == 0)
							{
								continue;
							}
						}
						else if ((garbageCollectionRequest.m_Flags & GarbageCollectionRequestFlags.IndustrialWaste) != 0)
						{
							num += 30f;
						}
						Entity val = nativeArray[i];
						if (AreaUtils.CheckServiceDistrict(entity, val, m_ServiceDistricts))
						{
							targetSeeker.FindTargets(val, num);
						}
					}
				}
				return;
			}
			NativeArray<Game.Vehicles.GarbageTruck> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Vehicles.GarbageTruck>(ref m_GarbageTruckType);
			if (nativeArray3.Length == 0)
			{
				return;
			}
			NativeArray<PathOwner> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathOwner>(ref m_PathOwnerType);
			NativeArray<Owner> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			BufferAccessor<PathElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<PathElement>(ref m_PathElementType);
			BufferAccessor<ServiceDispatch> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ServiceDispatch>(ref m_ServiceDispatchType);
			GarbageCollectionRequest garbageCollectionRequest2 = default(GarbageCollectionRequest);
			PathInformation pathInformation = default(PathInformation);
			DynamicBuffer<PathElement> val5 = default(DynamicBuffer<PathElement>);
			for (int k = 0; k < nativeArray3.Length; k++)
			{
				Entity val2 = nativeArray[k];
				Game.Vehicles.GarbageTruck garbageTruck = nativeArray3[k];
				if ((garbageTruck.m_State & (GarbageTruckFlags.Disabled | GarbageTruckFlags.EstimatedFull)) != 0)
				{
					continue;
				}
				for (int l = 0; l < m_SetupData.Length; l++)
				{
					m_SetupData.GetItem(l, out var entity2, out var owner2, out var targetSeeker2);
					float num2 = 0f;
					if (nativeArray5.Length != 0)
					{
						if (!AreaUtils.CheckServiceDistrict(entity2, nativeArray5[k].m_Owner, m_ServiceDistricts))
						{
							continue;
						}
						if (m_OutsideConnections.HasComponent(nativeArray5[k].m_Owner))
						{
							num2 += 30f;
						}
					}
					m_GarbageCollectionRequestData.TryGetComponent(owner2, ref garbageCollectionRequest2);
					if ((garbageTruck.m_State & GarbageTruckFlags.IndustrialWasteOnly) != 0)
					{
						if ((garbageCollectionRequest2.m_Flags & GarbageCollectionRequestFlags.IndustrialWaste) == 0)
						{
							continue;
						}
					}
					else if ((garbageCollectionRequest2.m_Flags & GarbageCollectionRequestFlags.IndustrialWaste) != 0)
					{
						num2 += 30f;
					}
					if ((garbageTruck.m_State & GarbageTruckFlags.Returning) != 0 || nativeArray4.Length == 0)
					{
						targetSeeker2.FindTargets(val2, num2);
						continue;
					}
					PathOwner pathOwner = nativeArray4[k];
					DynamicBuffer<ServiceDispatch> val3 = bufferAccessor2[k];
					int num3 = math.min(garbageTruck.m_RequestCount, val3.Length);
					PathElement pathElement = default(PathElement);
					bool flag = false;
					if (num3 >= 1)
					{
						DynamicBuffer<PathElement> val4 = bufferAccessor[k];
						if (pathOwner.m_ElementIndex < val4.Length)
						{
							num2 += (float)(val4.Length - pathOwner.m_ElementIndex) * garbageTruck.m_PathElementTime * targetSeeker2.m_PathfindParameters.m_Weights.time;
							pathElement = val4[val4.Length - 1];
							flag = true;
						}
					}
					for (int m = 1; m < num3; m++)
					{
						Entity request = val3[m].m_Request;
						if (m_PathInformationData.TryGetComponent(request, ref pathInformation))
						{
							num2 += pathInformation.m_Duration * targetSeeker2.m_PathfindParameters.m_Weights.time;
						}
						if (m_PathElements.TryGetBuffer(request, ref val5) && val5.Length != 0)
						{
							pathElement = val5[val5.Length - 1];
							flag = true;
						}
					}
					if (flag)
					{
						targetSeeker2.m_Buffer.Enqueue(new PathTarget(val2, pathElement.m_Target, pathElement.m_TargetDelta.y, num2));
					}
					else
					{
						targetSeeker2.FindTargets(val2, val2, num2, EdgeFlags.DefaultMask, allowAccessRestriction: true, num3 >= 1);
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
	private struct SetupGarbageTransferJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.GarbageFacility> m_GarbageFacilityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.OutsideConnection> m_OutsideConnectionType;

		[ReadOnly]
		public BufferTypeHandle<Resources> m_ResourcesType;

		[ReadOnly]
		public BufferTypeHandle<TradeCost> m_TradeCostType;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> m_InstalledUpgradeType;

		[ReadOnly]
		public ComponentLookup<StorageLimitData> m_StorageLimitData;

		[ReadOnly]
		public ComponentLookup<StorageCompanyData> m_StorageCompanyData;

		[ReadOnly]
		public BufferLookup<ServiceDistrict> m_ServiceDistricts;

		public PathfindSetupSystem.SetupData m_SetupData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			if (((ArchetypeChunk)(ref chunk)).Has<Game.Objects.OutsideConnection>(ref m_OutsideConnectionType))
			{
				return;
			}
			NativeArray<Game.Buildings.GarbageFacility> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.GarbageFacility>(ref m_GarbageFacilityType);
			if (nativeArray2.Length == 0)
			{
				return;
			}
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Game.Buildings.GarbageFacility garbageFacility = nativeArray2[i];
				for (int j = 0; j < m_SetupData.Length; j++)
				{
					m_SetupData.GetItem(j, out var _, out var targetSeeker);
					float value = targetSeeker.m_SetupQueueTarget.m_Value2;
					if ((targetSeeker.m_SetupQueueTarget.m_Flags & SetupTargetFlags.RequireTransport) != SetupTargetFlags.None && (garbageFacility.m_Flags & GarbageFacilityFlags.HasAvailableDeliveryTrucks) == 0)
					{
						continue;
					}
					float num = math.max(0f, 1f - value);
					if ((targetSeeker.m_SetupQueueTarget.m_Flags & SetupTargetFlags.Import) != SetupTargetFlags.None)
					{
						if (garbageFacility.m_AcceptGarbagePriority > num)
						{
							Entity entity2 = nativeArray[i];
							targetSeeker.FindTargets(entity2, 120f * math.saturate(1f - (garbageFacility.m_AcceptGarbagePriority - num)));
						}
					}
					else if ((targetSeeker.m_SetupQueueTarget.m_Flags & SetupTargetFlags.Export) != SetupTargetFlags.None && garbageFacility.m_DeliverGarbagePriority > num)
					{
						Entity entity3 = nativeArray[i];
						targetSeeker.FindTargets(entity3, 120f * math.saturate(1f - (garbageFacility.m_DeliverGarbagePriority - num)));
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
	private struct GarbageCollectorRequestsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<ServiceRequest> m_ServiceRequestType;

		[ReadOnly]
		public ComponentTypeHandle<GarbageCollectionRequest> m_GarbageCollectionRequestType;

		[ReadOnly]
		public ComponentLookup<GarbageCollectionRequest> m_GarbageCollectionRequestData;

		[ReadOnly]
		public ComponentLookup<CurrentDistrict> m_CurrentDistrictData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.GarbageTruck> m_GarbageTruckData;

		[ReadOnly]
		public BufferLookup<ServiceDistrict> m_ServiceDistricts;

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
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<ServiceRequest> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ServiceRequest>(ref m_ServiceRequestType);
			NativeArray<GarbageCollectionRequest> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<GarbageCollectionRequest>(ref m_GarbageCollectionRequestType);
			GarbageCollectionRequest garbageCollectionRequest = default(GarbageCollectionRequest);
			Owner owner2 = default(Owner);
			for (int i = 0; i < m_SetupData.Length; i++)
			{
				m_SetupData.GetItem(i, out var _, out var owner, out var targetSeeker);
				if (!m_GarbageCollectionRequestData.TryGetComponent(owner, ref garbageCollectionRequest))
				{
					continue;
				}
				Random random = targetSeeker.m_RandomSeed.GetRandom(unfilteredChunkIndex);
				Entity service = Entity.Null;
				if (m_GarbageTruckData.HasComponent(garbageCollectionRequest.m_Target))
				{
					if (targetSeeker.m_Owner.TryGetComponent(garbageCollectionRequest.m_Target, ref owner2))
					{
						service = owner2.m_Owner;
					}
				}
				else
				{
					if (!targetSeeker.m_PrefabRef.HasComponent(garbageCollectionRequest.m_Target))
					{
						continue;
					}
					service = garbageCollectionRequest.m_Target;
				}
				for (int j = 0; j < nativeArray3.Length; j++)
				{
					if ((nativeArray2[j].m_Flags & ServiceRequestFlags.Reversed) != 0)
					{
						continue;
					}
					GarbageCollectionRequest garbageCollectionRequest2 = nativeArray3[j];
					Entity district = Entity.Null;
					if (m_CurrentDistrictData.HasComponent(garbageCollectionRequest2.m_Target))
					{
						district = m_CurrentDistrictData[garbageCollectionRequest2.m_Target].m_District;
					}
					float num = ((Random)(ref random)).NextFloat(30f);
					if ((garbageCollectionRequest.m_Flags & GarbageCollectionRequestFlags.IndustrialWaste) != 0)
					{
						if ((garbageCollectionRequest2.m_Flags & GarbageCollectionRequestFlags.IndustrialWaste) == 0)
						{
							continue;
						}
					}
					else if ((garbageCollectionRequest2.m_Flags & GarbageCollectionRequestFlags.IndustrialWaste) != 0)
					{
						num += 30f;
					}
					if (AreaUtils.CheckServiceDistrict(district, service, m_ServiceDistricts))
					{
						targetSeeker.FindTargets(nativeArray[j], garbageCollectionRequest2.m_Target, num, EdgeFlags.DefaultMask, allowAccessRestriction: true, navigationEnd: false);
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private EntityQuery m_GarbageCollectorQuery;

	private EntityQuery m_GarbageTransferQuery;

	private EntityQuery m_GarbageCollectionRequestQuery;

	private EntityTypeHandle m_EntityType;

	private ComponentTypeHandle<PathOwner> m_PathOwnerType;

	private ComponentTypeHandle<Owner> m_OwnerType;

	private ComponentTypeHandle<Game.Objects.OutsideConnection> m_OutsideConnectionType;

	private ComponentTypeHandle<ServiceRequest> m_ServiceRequestType;

	private ComponentTypeHandle<GarbageCollectionRequest> m_GarbageCollectionRequestType;

	private ComponentTypeHandle<Game.Buildings.GarbageFacility> m_GarbageFacilityType;

	private ComponentTypeHandle<Game.Vehicles.GarbageTruck> m_GarbageTruckType;

	private ComponentTypeHandle<PrefabRef> m_PrefabRefType;

	private BufferTypeHandle<PathElement> m_PathElementType;

	private BufferTypeHandle<ServiceDispatch> m_ServiceDispatchType;

	private BufferTypeHandle<Resources> m_ResourcesType;

	private BufferTypeHandle<TradeCost> m_TradeCostType;

	private BufferTypeHandle<InstalledUpgrade> m_InstalledUpgradeType;

	private ComponentLookup<PathInformation> m_PathInformationData;

	private ComponentLookup<GarbageCollectionRequest> m_GarbageCollectionRequestData;

	private ComponentLookup<Game.Objects.OutsideConnection> m_OutsideConnections;

	private ComponentLookup<Game.City.City> m_CityData;

	private ComponentLookup<CurrentDistrict> m_CurrentDistrictData;

	private ComponentLookup<Game.Vehicles.GarbageTruck> m_GarbageTruckData;

	private ComponentLookup<StorageLimitData> m_StorageLimitData;

	private ComponentLookup<StorageCompanyData> m_StorageCompanyData;

	private BufferLookup<PathElement> m_PathElements;

	private BufferLookup<ServiceDistrict> m_ServiceDistricts;

	private CitySystem m_CitySystem;

	public GarbagePathfindSetup(PathfindSetupSystem system)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected O, but got Unknown
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Expected O, but got Unknown
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Buildings.GarbageFacility>(),
			ComponentType.ReadOnly<Game.Vehicles.GarbageTruck>()
		};
		val.None = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Destroyed>(),
			ComponentType.ReadOnly<Game.Buildings.ServiceUpgrade>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_GarbageCollectorQuery = system.GetSetupQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[2];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Buildings.GarbageFacility>(),
			ComponentType.ReadOnly<ServiceDispatch>(),
			ComponentType.ReadOnly<PrefabRef>()
		};
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Destroyed>(),
			ComponentType.ReadOnly<Temp>()
		};
		array2[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Game.Companies.StorageCompany>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<Resources>(),
			ComponentType.ReadOnly<TradeCost>()
		};
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Destroyed>(),
			ComponentType.ReadOnly<Temp>()
		};
		array2[1] = val;
		m_GarbageTransferQuery = system.GetSetupQuery((EntityQueryDesc[])(object)array2);
		m_GarbageCollectionRequestQuery = system.GetSetupQuery(ComponentType.ReadOnly<GarbageCollectionRequest>(), ComponentType.Exclude<Dispatched>(), ComponentType.Exclude<PathInformation>());
		m_EntityType = ((ComponentSystemBase)system).GetEntityTypeHandle();
		m_PathOwnerType = ((ComponentSystemBase)system).GetComponentTypeHandle<PathOwner>(true);
		m_OwnerType = ((ComponentSystemBase)system).GetComponentTypeHandle<Owner>(true);
		m_OutsideConnectionType = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Objects.OutsideConnection>(true);
		m_ServiceRequestType = ((ComponentSystemBase)system).GetComponentTypeHandle<ServiceRequest>(true);
		m_GarbageCollectionRequestType = ((ComponentSystemBase)system).GetComponentTypeHandle<GarbageCollectionRequest>(true);
		m_GarbageFacilityType = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Buildings.GarbageFacility>(true);
		m_GarbageTruckType = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Vehicles.GarbageTruck>(true);
		m_PrefabRefType = ((ComponentSystemBase)system).GetComponentTypeHandle<PrefabRef>(true);
		m_PathElementType = ((ComponentSystemBase)system).GetBufferTypeHandle<PathElement>(true);
		m_ServiceDispatchType = ((ComponentSystemBase)system).GetBufferTypeHandle<ServiceDispatch>(true);
		m_ResourcesType = ((ComponentSystemBase)system).GetBufferTypeHandle<Resources>(true);
		m_TradeCostType = ((ComponentSystemBase)system).GetBufferTypeHandle<TradeCost>(true);
		m_InstalledUpgradeType = ((ComponentSystemBase)system).GetBufferTypeHandle<InstalledUpgrade>(true);
		m_PathInformationData = ((SystemBase)system).GetComponentLookup<PathInformation>(true);
		m_GarbageCollectionRequestData = ((SystemBase)system).GetComponentLookup<GarbageCollectionRequest>(true);
		m_OutsideConnections = ((SystemBase)system).GetComponentLookup<Game.Objects.OutsideConnection>(true);
		m_CurrentDistrictData = ((SystemBase)system).GetComponentLookup<CurrentDistrict>(true);
		m_GarbageTruckData = ((SystemBase)system).GetComponentLookup<Game.Vehicles.GarbageTruck>(true);
		m_StorageLimitData = ((SystemBase)system).GetComponentLookup<StorageLimitData>(true);
		m_StorageCompanyData = ((SystemBase)system).GetComponentLookup<StorageCompanyData>(true);
		m_CityData = ((SystemBase)system).GetComponentLookup<Game.City.City>(true);
		m_PathElements = ((SystemBase)system).GetBufferLookup<PathElement>(true);
		m_ServiceDistricts = ((SystemBase)system).GetBufferLookup<ServiceDistrict>(true);
		m_CitySystem = ((ComponentSystemBase)system).World.GetOrCreateSystemManaged<CitySystem>();
	}

	public JobHandle SetupGarbageCollector(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		((EntityTypeHandle)(ref m_EntityType)).Update((SystemBase)(object)system);
		m_GarbageFacilityType.Update((SystemBase)(object)system);
		m_GarbageTruckType.Update((SystemBase)(object)system);
		m_PathOwnerType.Update((SystemBase)(object)system);
		m_OwnerType.Update((SystemBase)(object)system);
		m_PathElementType.Update((SystemBase)(object)system);
		m_ServiceDispatchType.Update((SystemBase)(object)system);
		m_PathInformationData.Update((SystemBase)(object)system);
		m_GarbageCollectionRequestData.Update((SystemBase)(object)system);
		m_PathElements.Update((SystemBase)(object)system);
		m_ServiceDistricts.Update((SystemBase)(object)system);
		m_OutsideConnections.Update((SystemBase)(object)system);
		m_CityData.Update((SystemBase)(object)system);
		return JobChunkExtensions.ScheduleParallel<SetupGarbageCollectorsJob>(new SetupGarbageCollectorsJob
		{
			m_EntityType = m_EntityType,
			m_GarbageFacilityType = m_GarbageFacilityType,
			m_GarbageTruckType = m_GarbageTruckType,
			m_PathOwnerType = m_PathOwnerType,
			m_OwnerType = m_OwnerType,
			m_PathElementType = m_PathElementType,
			m_ServiceDispatchType = m_ServiceDispatchType,
			m_PathInformationData = m_PathInformationData,
			m_GarbageCollectionRequestData = m_GarbageCollectionRequestData,
			m_PathElements = m_PathElements,
			m_ServiceDistricts = m_ServiceDistricts,
			m_OutsideConnections = m_OutsideConnections,
			m_CityData = m_CityData,
			m_City = m_CitySystem.City,
			m_SetupData = setupData
		}, m_GarbageCollectorQuery, inputDeps);
	}

	public JobHandle SetupGarbageTransfer(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
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
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		((EntityTypeHandle)(ref m_EntityType)).Update((SystemBase)(object)system);
		m_GarbageFacilityType.Update((SystemBase)(object)system);
		m_PrefabRefType.Update((SystemBase)(object)system);
		m_OutsideConnectionType.Update((SystemBase)(object)system);
		m_ResourcesType.Update((SystemBase)(object)system);
		m_TradeCostType.Update((SystemBase)(object)system);
		m_InstalledUpgradeType.Update((SystemBase)(object)system);
		m_StorageCompanyData.Update((SystemBase)(object)system);
		m_StorageLimitData.Update((SystemBase)(object)system);
		m_ServiceDistricts.Update((SystemBase)(object)system);
		return JobChunkExtensions.ScheduleParallel<SetupGarbageTransferJob>(new SetupGarbageTransferJob
		{
			m_EntityType = m_EntityType,
			m_GarbageFacilityType = m_GarbageFacilityType,
			m_PrefabRefType = m_PrefabRefType,
			m_OutsideConnectionType = m_OutsideConnectionType,
			m_ResourcesType = m_ResourcesType,
			m_TradeCostType = m_TradeCostType,
			m_InstalledUpgradeType = m_InstalledUpgradeType,
			m_StorageCompanyData = m_StorageCompanyData,
			m_StorageLimitData = m_StorageLimitData,
			m_ServiceDistricts = m_ServiceDistricts,
			m_SetupData = setupData
		}, m_GarbageTransferQuery, inputDeps);
	}

	public JobHandle SetupGarbageCollectorRequest(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		((EntityTypeHandle)(ref m_EntityType)).Update((SystemBase)(object)system);
		m_ServiceRequestType.Update((SystemBase)(object)system);
		m_GarbageCollectionRequestType.Update((SystemBase)(object)system);
		m_GarbageCollectionRequestData.Update((SystemBase)(object)system);
		m_CurrentDistrictData.Update((SystemBase)(object)system);
		m_GarbageTruckData.Update((SystemBase)(object)system);
		m_ServiceDistricts.Update((SystemBase)(object)system);
		return JobChunkExtensions.ScheduleParallel<GarbageCollectorRequestsJob>(new GarbageCollectorRequestsJob
		{
			m_EntityType = m_EntityType,
			m_ServiceRequestType = m_ServiceRequestType,
			m_GarbageCollectionRequestType = m_GarbageCollectionRequestType,
			m_GarbageCollectionRequestData = m_GarbageCollectionRequestData,
			m_CurrentDistrictData = m_CurrentDistrictData,
			m_GarbageTruckData = m_GarbageTruckData,
			m_ServiceDistricts = m_ServiceDistricts,
			m_SetupData = setupData
		}, m_GarbageCollectionRequestQuery, inputDeps);
	}
}
