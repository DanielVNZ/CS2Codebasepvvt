using Colossal.Entities;
using Game.Agents;
using Game.Areas;
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

public struct CitizenPathfindSetup
{
	[BurstCompile]
	private struct SetupTouristTargetJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentLookup<LodgingProvider> m_LodgingProviders;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingDatas;

		[ReadOnly]
		public ComponentLookup<TouristHousehold> m_TouristHouseholds;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenters;

		[ReadOnly]
		public BufferLookup<ResourceAvailability> m_ResourceAvailabilityBufs;

		public PathfindSetupSystem.SetupData m_SetupData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			for (int i = 0; i < m_SetupData.Length; i++)
			{
				m_SetupData.GetItem(i, out var entity, out var targetSeeker);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Entity val = nativeArray[j];
					Entity val2 = val;
					float num = 0f;
					bool flag = m_TouristHouseholds.HasComponent(entity) && m_TouristHouseholds[entity].m_Hotel == Entity.Null;
					if (m_LodgingProviders.HasComponent(val) && flag)
					{
						if (m_LodgingProviders[val].m_FreeRooms == 0 || !m_PropertyRenters.HasComponent(val) || m_PropertyRenters[val].m_Property == Entity.Null)
						{
							continue;
						}
						val2 = m_PropertyRenters[val].m_Property;
						num -= 5000f;
						num += -10f * (float)m_LodgingProviders[val].m_FreeRooms;
						float num2 = m_LodgingProviders[val].m_Price;
						num += math.min(num2, 500f);
					}
					else
					{
						num += 5000f;
					}
					if (!m_BuildingDatas.HasComponent(val2))
					{
						continue;
					}
					Building building = m_BuildingDatas[val2];
					if (!BuildingUtils.CheckOption(building, BuildingOption.Inactive))
					{
						if (m_ResourceAvailabilityBufs.HasBuffer(building.m_RoadEdge))
						{
							float availability = NetUtils.GetAvailability(m_ResourceAvailabilityBufs[building.m_RoadEdge], AvailableResource.Attractiveness, building.m_CurvePosition);
							num -= availability * 0.01f;
						}
						targetSeeker.FindTargets(val2, num);
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
	private struct SetupLeisureTargetJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<ServiceAvailable> m_ServiceAvailableType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> m_IndustrialProcessDatas;

		[ReadOnly]
		public ComponentLookup<LeisureProviderData> m_LeisureProviderDatas;

		[ReadOnly]
		public ComponentLookup<ResourceData> m_ResourceDatas;

		[ReadOnly]
		public ComponentLookup<ServiceCompanyData> m_ServiceDatas;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingDatas;

		[ReadOnly]
		public BufferLookup<Resources> m_Resources;

		[ReadOnly]
		public ResourcePrefabs m_ResourcePrefabs;

		public PathfindSetupSystem.SetupData m_SetupData;

		public int m_LeisureSystemUpdateInterval;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<ServiceAvailable> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ServiceAvailable>(ref m_ServiceAvailableType);
			NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabType);
			for (int i = 0; i < m_SetupData.Length; i++)
			{
				m_SetupData.GetItem(i, out var _, out var targetSeeker);
				LeisureType value = (LeisureType)targetSeeker.m_SetupQueueTarget.m_Value;
				float value2 = targetSeeker.m_SetupQueueTarget.m_Value2;
				float num = targetSeeker.m_PathfindParameters.m_Weights.time * 0.01f;
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Entity val = nativeArray[j];
					if (m_BuildingDatas.HasComponent(val) && BuildingUtils.CheckOption(m_BuildingDatas[val], BuildingOption.Inactive))
					{
						continue;
					}
					Entity prefab = nativeArray3[j].m_Prefab;
					if (!m_LeisureProviderDatas.HasComponent(prefab))
					{
						continue;
					}
					LeisureProviderData leisureProviderData = m_LeisureProviderDatas[prefab];
					float num2 = 0f;
					if (value != leisureProviderData.m_LeisureType)
					{
						continue;
					}
					if ((value == LeisureType.Commercial || value == LeisureType.Meals) && nativeArray2.Length > 0 && m_ServiceDatas.HasComponent(prefab))
					{
						int serviceAvailable = nativeArray2[j].m_ServiceAvailable;
						if (m_IndustrialProcessDatas.HasComponent(prefab))
						{
							IndustrialProcessData industrialProcessData = m_IndustrialProcessDatas[prefab];
							if (industrialProcessData.m_Output.m_Resource != Resource.NoResource)
							{
								serviceAvailable = math.min(serviceAvailable, EconomyUtils.GetResources(industrialProcessData.m_Output.m_Resource, m_Resources[val]));
								float marketPrice = EconomyUtils.GetMarketPrice(m_ResourceDatas[m_ResourcePrefabs[industrialProcessData.m_Output.m_Resource]]);
								num2 = 0.2f * value2 * marketPrice * (EconomyUtils.GetServicePriceMultiplier(serviceAvailable, m_ServiceDatas[prefab].m_MaxService) - 1f);
							}
						}
					}
					float num3 = value2 * (float)m_LeisureSystemUpdateInterval * 16f / (float)leisureProviderData.m_Efficiency;
					num2 += num * num3;
					targetSeeker.FindTargets(val, num2);
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct SetupSchoolSeekerToJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<Game.Buildings.Student> m_StudentType;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> m_UpgradeType;

		[ReadOnly]
		public ComponentLookup<SchoolData> m_SchoolDatas;

		[ReadOnly]
		public BufferLookup<Efficiency> m_Efficiencies;

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
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<Game.Buildings.Student> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Game.Buildings.Student>(ref m_StudentType);
			BufferAccessor<InstalledUpgrade> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<InstalledUpgrade>(ref m_UpgradeType);
			bool flag = bufferAccessor2.Length != 0;
			DynamicBuffer<Efficiency> buffer = default(DynamicBuffer<Efficiency>);
			for (int i = 0; i < m_SetupData.Length; i++)
			{
				m_SetupData.GetItem(i, out var entity, out var targetSeeker);
				int value = targetSeeker.m_SetupQueueTarget.m_Value;
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Entity val = nativeArray[j];
					if (!AreaUtils.CheckServiceDistrict(entity, val, m_ServiceDistricts))
					{
						continue;
					}
					Entity prefab = nativeArray2[j].m_Prefab;
					if (m_SchoolDatas.HasComponent(prefab))
					{
						SchoolData data = m_SchoolDatas[prefab];
						if (flag)
						{
							UpgradeUtils.CombineStats<SchoolData>(ref data, bufferAccessor2[j], ref targetSeeker.m_PrefabRef, ref m_SchoolDatas);
						}
						int num = data.m_StudentCapacity;
						if (m_Efficiencies.TryGetBuffer(val, ref buffer))
						{
							num = Mathf.RoundToInt((float)num * math.min(1f, BuildingUtils.GetEfficiency(buffer)));
						}
						bool flag2 = data.m_EducationLevel == 5;
						int num2 = num - bufferAccessor[j].Length;
						if (((flag2 && value > 1) || data.m_EducationLevel == value) && num2 > 0)
						{
							targetSeeker.FindTargets(val, math.max(0f, (float)(-num2) + (flag2 ? 5000f : 0f)));
						}
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
	private struct SetupJobSeekerToJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<FreeWorkplaces> m_FreeWorkplaceType;

		[ReadOnly]
		public ComponentTypeHandle<WorkProvider> m_WorkProviderType;

		[ReadOnly]
		public ComponentTypeHandle<CityServiceUpkeep> m_CityServiceType;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> m_OutsideConnections;

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
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<FreeWorkplaces> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<FreeWorkplaces>(ref m_FreeWorkplaceType);
			NativeArray<WorkProvider> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WorkProvider>(ref m_WorkProviderType);
			float num = (((ArchetypeChunk)(ref chunk)).Has<CityServiceUpkeep>(ref m_CityServiceType) ? (-4000f) : 0f);
			for (int i = 0; i < m_SetupData.Length; i++)
			{
				m_SetupData.GetItem(i, out var _, out var targetSeeker);
				Random random = targetSeeker.m_RandomSeed.GetRandom(unfilteredChunkIndex);
				int num2 = targetSeeker.m_SetupQueueTarget.m_Value % 5;
				int num3 = targetSeeker.m_SetupQueueTarget.m_Value / 5 - 1;
				float value = targetSeeker.m_SetupQueueTarget.m_Value2;
				SetupTargetFlags flags = targetSeeker.m_SetupQueueTarget.m_Flags;
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Entity val = nativeArray[j];
					FreeWorkplaces freeWorkplaces = nativeArray2[j];
					if ((flags & SetupTargetFlags.Export) != SetupTargetFlags.None)
					{
						if (freeWorkplaces.GetFree(num2) > 0 && !m_OutsideConnections.HasComponent(val))
						{
							targetSeeker.FindTargets(val, 2000f);
						}
					}
					else
					{
						if ((flags & SetupTargetFlags.Import) != SetupTargetFlags.None && m_OutsideConnections.HasComponent(val))
						{
							continue;
						}
						int lowestFree = freeWorkplaces.GetLowestFree();
						if (num2 >= lowestFree && num2 >= num3)
						{
							int bestFor = freeWorkplaces.GetBestFor(num2);
							int maxWorkers = nativeArray3[j].m_MaxWorkers;
							if (freeWorkplaces.Count > 0 && maxWorkers > 0)
							{
								float num4 = (float)freeWorkplaces.Count / (float)maxWorkers;
								int num5 = ((Random)(ref random)).NextInt(4000);
								int num6 = (m_OutsideConnections.HasComponent(val) ? 8000 : (-4000));
								targetSeeker.FindTargets(val, 6000f * (1f - num4) + math.max(0f, 2f - value) * 4000f * (float)(num2 - bestFor) + num + (float)num5 + (float)num6);
							}
						}
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
	private struct SetupAttractionJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentLookup<AttractivenessProvider> m_AttractivenessProviders;

		public PathfindSetupSystem.SetupData m_SetupData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			for (int i = 0; i < m_SetupData.Length; i++)
			{
				m_SetupData.GetItem(i, out var _, out var targetSeeker);
				Random random = targetSeeker.m_RandomSeed.GetRandom(unfilteredChunkIndex);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Entity val = nativeArray[j];
					if (m_AttractivenessProviders.HasComponent(val))
					{
						targetSeeker.FindTargets(val, -100f * (float)m_AttractivenessProviders[val].m_Attractiveness * ((Random)(ref random)).NextFloat());
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
	private struct SetupHomelessJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public BufferTypeHandle<Renter> m_RenterType;

		[ReadOnly]
		public ComponentTypeHandle<Building> m_BuildingType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_BuildingDatas;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> m_BuildingProperties;

		[ReadOnly]
		public BufferLookup<Game.Net.ServiceCoverage> m_Coverages;

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
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabType);
			NativeArray<Building> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Building>(ref m_BuildingType);
			BufferAccessor<Renter> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Renter>(ref m_RenterType);
			for (int i = 0; i < m_SetupData.Length; i++)
			{
				m_SetupData.GetItem(i, out var _, out var targetSeeker);
				Random random = targetSeeker.m_RandomSeed.GetRandom(unfilteredChunkIndex);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Entity entity2 = nativeArray[j];
					Entity prefab = nativeArray2[j].m_Prefab;
					Building building = nativeArray3[j];
					if (building.m_RoadEdge != Entity.Null && m_Coverages.HasBuffer(building.m_RoadEdge) && m_BuildingDatas.HasComponent(prefab))
					{
						float serviceCoverage = NetUtils.GetServiceCoverage(m_Coverages[building.m_RoadEdge], CoverageService.Police, building.m_CurvePosition);
						float num = BuildingUtils.GetShelterHomelessCapacity(prefab, ref m_BuildingDatas, ref m_BuildingProperties);
						targetSeeker.FindTargets(entity2, 100f * serviceCoverage + 1000f * ((float)bufferAccessor[j].Length / num) + ((Random)(ref random)).NextFloat(1000f));
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
	private struct SetupFindHomeJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public BufferTypeHandle<Renter> m_RenterType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		[ReadOnly]
		public ComponentLookup<Building> m_Buildings;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_BuildingDatas;

		[ReadOnly]
		public BufferLookup<Game.Net.ServiceCoverage> m_Coverages;

		public PathfindSetupSystem.SetupData m_SetupData;

		[ReadOnly]
		public ComponentLookup<PropertyOnMarket> m_PropertiesOnMarket;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefs;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> m_BuildingProperties;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> m_SpawnableDatas;

		[ReadOnly]
		public BufferLookup<ResourceAvailability> m_Availabilities;

		[ReadOnly]
		public BufferLookup<Game.Net.ServiceCoverage> m_ServiceCoverages;

		[ReadOnly]
		public ComponentLookup<Household> m_Households;

		[ReadOnly]
		public ComponentLookup<HomelessHousehold> m_HomelessHouseholds;

		[ReadOnly]
		public ComponentLookup<Citizen> m_Citizens;

		[ReadOnly]
		public ComponentLookup<HealthProblem> m_HealthProblems;

		[ReadOnly]
		public ComponentLookup<Worker> m_Workers;

		[ReadOnly]
		public ComponentLookup<Game.Citizens.Student> m_Students;

		[ReadOnly]
		public ComponentLookup<CrimeProducer> m_Crimes;

		[ReadOnly]
		public ComponentLookup<Transform> m_Transforms;

		[ReadOnly]
		public ComponentLookup<Locked> m_Lockeds;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifiers;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Park> m_Parks;

		[ReadOnly]
		public ComponentLookup<Abandoned> m_Abandoneds;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> m_HouseholdCitizens;

		[ReadOnly]
		public BufferLookup<Resources> m_ResourcesBufs;

		[ReadOnly]
		public ComponentLookup<ElectricityConsumer> m_ElectricityConsumers;

		[ReadOnly]
		public ComponentLookup<WaterConsumer> m_WaterConsumers;

		[ReadOnly]
		public ComponentLookup<GarbageProducer> m_GarbageProducers;

		[ReadOnly]
		public ComponentLookup<MailProducer> m_MailProducers;

		[ReadOnly]
		public NativeArray<int> m_TaxRates;

		[ReadOnly]
		public NativeArray<AirPollution> m_AirPollutionMap;

		[ReadOnly]
		public NativeArray<GroundPollution> m_PollutionMap;

		[ReadOnly]
		public NativeArray<NoisePollution> m_NoiseMap;

		[ReadOnly]
		public CellMapData<TelecomCoverage> m_TelecomCoverages;

		public HealthcareParameterData m_HealthcareParameters;

		public ParkParameterData m_ParkParameters;

		public EducationParameterData m_EducationParameters;

		public EconomyParameterData m_EconomyParameters;

		public TelecomParameterData m_TelecomParameters;

		public GarbageParameterData m_GarbageParameters;

		public PoliceConfigurationData m_PoliceParameters;

		public ServiceFeeParameterData m_ServiceFeeParameterData;

		public CitizenHappinessParameterData m_CitizenHappinessParameterData;

		[ReadOnly]
		public Entity m_City;

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
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabType);
			BufferAccessor<Renter> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Renter>(ref m_RenterType);
			DynamicBuffer<HouseholdCitizen> val = default(DynamicBuffer<HouseholdCitizen>);
			for (int i = 0; i < m_SetupData.Length; i++)
			{
				m_SetupData.GetItem(i, out var _, out var targetSeeker);
				targetSeeker.m_RandomSeed.GetRandom(unfilteredChunkIndex);
				Entity entity2 = targetSeeker.m_SetupQueueTarget.m_Entity;
				if (!m_HouseholdCitizens.TryGetBuffer(entity2, ref val))
				{
					continue;
				}
				bool flag = m_HomelessHouseholds.HasComponent(entity2) && m_HomelessHouseholds[entity2].m_TempHome != Entity.Null;
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Entity val2 = nativeArray[j];
					Entity prefab = nativeArray2[j].m_Prefab;
					Building building = m_Buildings[val2];
					if (!(building.m_RoadEdge != Entity.Null) || !m_Coverages.HasBuffer(building.m_RoadEdge) || !m_BuildingDatas.HasComponent(prefab))
					{
						continue;
					}
					if (BuildingUtils.IsHomelessShelterBuilding(val2, ref m_Parks, ref m_Abandoneds))
					{
						if (!flag)
						{
							float serviceCoverage = NetUtils.GetServiceCoverage(m_Coverages[building.m_RoadEdge], CoverageService.Police, building.m_CurvePosition);
							int shelterHomelessCapacity = BuildingUtils.GetShelterHomelessCapacity(prefab, ref m_BuildingDatas, ref m_BuildingProperties);
							if (bufferAccessor[j].Length < shelterHomelessCapacity)
							{
								targetSeeker.FindTargets(val2, 100f * serviceCoverage + 1000f * (float)bufferAccessor[j].Length / (float)shelterHomelessCapacity + 10000f);
							}
						}
						continue;
					}
					int askingRent = m_PropertiesOnMarket[val2].m_AskingRent;
					int x = m_ServiceFeeParameterData.m_GarbageFeeRCIO.x;
					int householdIncome = EconomyUtils.GetHouseholdIncome(val, ref m_Workers, ref m_Citizens, ref m_HealthProblems, ref m_EconomyParameters, m_TaxRates);
					int num = math.max(0, EconomyUtils.GetResources(Resource.Money, m_ResourcesBufs[entity2]));
					if (CitizenUtils.IsHouseholdNeedSupport(val, ref m_Citizens, ref m_Students) || askingRent + x <= householdIncome + num)
					{
						float propertyScore = PropertyUtils.GetPropertyScore(val2, entity2, val, ref m_PrefabRefs, ref m_BuildingProperties, ref m_Buildings, ref m_BuildingDatas, ref m_Households, ref m_Citizens, ref m_Students, ref m_Workers, ref m_SpawnableDatas, ref m_Crimes, ref m_ServiceCoverages, ref m_Lockeds, ref m_ElectricityConsumers, ref m_WaterConsumers, ref m_GarbageProducers, ref m_MailProducers, ref m_Transforms, ref m_Abandoneds, ref m_Parks, ref m_Availabilities, m_TaxRates, m_PollutionMap, m_AirPollutionMap, m_NoiseMap, m_TelecomCoverages, m_CityModifiers[m_City], m_HealthcareParameters.m_HealthcareServicePrefab, m_ParkParameters.m_ParkServicePrefab, m_EducationParameters.m_EducationServicePrefab, m_TelecomParameters.m_TelecomServicePrefab, m_GarbageParameters.m_GarbageServicePrefab, m_PoliceParameters.m_PoliceServicePrefab, m_CitizenHappinessParameterData, m_GarbageParameters);
						targetSeeker.FindTargets(val2, 0f - propertyScore + (float)askingRent + (float)x - (float)householdIncome);
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private EntityQuery m_LeisureProviderQuery;

	private EntityQuery m_TouristTargetQuery;

	private EntityQuery m_SchoolQuery;

	private EntityQuery m_FreeWorkplaceQuery;

	private EntityQuery m_AttractionQuery;

	private EntityQuery m_HomelessShelterQuery;

	private EntityQuery m_FindHomeQuery;

	private GroundPollutionSystem m_GroundPollutionSystem;

	private AirPollutionSystem m_AirPollutionSystem;

	private NoisePollutionSystem m_NoisePollutionSystem;

	private TelecomCoverageSystem m_TelecomCoverageSystem;

	private CitySystem m_CitySystem;

	private ResourceSystem m_ResourceSystem;

	private LeisureSystem m_LeisureSystem;

	private TaxSystem m_TaxSystem;

	private EntityQuery m_EconomyParameterQuery;

	private EntityQuery m_HealthcareParameterQuery;

	private EntityQuery m_ParkParameterQuery;

	private EntityQuery m_EducationParameterQuery;

	private EntityQuery m_TelecomParameterQuery;

	private EntityQuery m_GarbageParameterQuery;

	private EntityQuery m_PoliceParameterQuery;

	private EntityQuery m_CitizenHappinessParameterQuery;

	private EntityQuery m_ServiceFeeParameterQuery;

	private EntityTypeHandle m_EntityType;

	private ComponentTypeHandle<ServiceAvailable> m_ServiceAvailableType;

	private ComponentTypeHandle<FreeWorkplaces> m_FreeWorkplaceType;

	private ComponentTypeHandle<WorkProvider> m_WorkProviderType;

	private ComponentTypeHandle<CityServiceUpkeep> m_CityServiceType;

	private ComponentTypeHandle<Building> m_BuildingType;

	private ComponentTypeHandle<PrefabRef> m_PrefabRefType;

	private BufferTypeHandle<Renter> m_RenterType;

	private BufferTypeHandle<Game.Buildings.Student> m_StudentType;

	private BufferTypeHandle<InstalledUpgrade> m_UpgradeType;

	private ComponentLookup<Game.Objects.OutsideConnection> m_OutsideConnections;

	private ComponentLookup<ServiceCompanyData> m_ServiceDatas;

	private ComponentLookup<Building> m_Buildings;

	private ComponentLookup<Household> m_Households;

	private ComponentLookup<HomelessHousehold> m_HomelessHouseholds;

	private ComponentLookup<Worker> m_Workers;

	private ComponentLookup<Game.Citizens.Student> m_Students;

	private ComponentLookup<Citizen> m_Citizens;

	private ComponentLookup<HealthProblem> m_HealthProblems;

	private ComponentLookup<TouristHousehold> m_TouristHouseholds;

	private BufferLookup<HouseholdCitizen> m_HouseholdCitizens;

	private ComponentLookup<Transform> m_Transforms;

	private ComponentLookup<Building> m_BuildingDatas;

	private BufferLookup<Efficiency> m_Efficiencies;

	private ComponentLookup<LodgingProvider> m_LodgingProviders;

	private ComponentLookup<AttractivenessProvider> m_AttractivenessProviders;

	private ComponentLookup<PropertyOnMarket> m_PropertiesOnMarket;

	private ComponentLookup<PropertyRenter> m_PropertyRenters;

	private ComponentLookup<CrimeProducer> m_Crimes;

	private ComponentLookup<Game.Buildings.Park> m_Parks;

	private ComponentLookup<Abandoned> m_Abandoneds;

	private ComponentLookup<ElectricityConsumer> m_ElectricityConsumers;

	private ComponentLookup<WaterConsumer> m_WaterConsumers;

	private ComponentLookup<GarbageProducer> m_GarbageProducers;

	private ComponentLookup<MailProducer> m_MailProducers;

	private ComponentLookup<PathInformation> m_PathInfos;

	private ComponentLookup<PrefabRef> m_Prefabs;

	private ComponentLookup<IndustrialProcessData> m_IndustrialProcessDatas;

	private ComponentLookup<LeisureProviderData> m_LeisureProviderDatas;

	private ComponentLookup<ResourceData> m_ResourceDatas;

	private ComponentLookup<SchoolData> m_SchoolDatas;

	private ComponentLookup<BuildingData> m_PrefabBuildingDatas;

	private ComponentLookup<BuildingPropertyData> m_BuildingProperties;

	private ComponentLookup<SpawnableBuildingData> m_SpawnableDatas;

	private ComponentLookup<Locked> m_Lockeds;

	private BufferLookup<Resources> m_Resources;

	private BufferLookup<ServiceDistrict> m_ServiceDistricts;

	private BufferLookup<ResourceAvailability> m_Availabilities;

	private BufferLookup<Game.Net.ServiceCoverage> m_ServiceCoverages;

	private BufferLookup<Renter> m_Renters;

	private BufferLookup<CityModifier> m_CityModifiers;

	private BufferLookup<OwnedVehicle> m_OwnedVehicles;

	public CitizenPathfindSetup(PathfindSetupSystem system)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Expected O, but got Unknown
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Expected O, but got Unknown
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_0505: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		//IL_051f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_0541: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0553: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0575: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_058f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0594: Unknown result type (might be due to invalid IL or missing references)
		//IL_059c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0604: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Unknown result type (might be due to invalid IL or missing references)
		//IL_0611: Unknown result type (might be due to invalid IL or missing references)
		//IL_0616: Unknown result type (might be due to invalid IL or missing references)
		//IL_061e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0623: Unknown result type (might be due to invalid IL or missing references)
		//IL_062b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0630: Unknown result type (might be due to invalid IL or missing references)
		//IL_0638: Unknown result type (might be due to invalid IL or missing references)
		//IL_063d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0645: Unknown result type (might be due to invalid IL or missing references)
		//IL_064a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0652: Unknown result type (might be due to invalid IL or missing references)
		//IL_0657: Unknown result type (might be due to invalid IL or missing references)
		//IL_065f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0664: Unknown result type (might be due to invalid IL or missing references)
		//IL_066c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0671: Unknown result type (might be due to invalid IL or missing references)
		//IL_0679: Unknown result type (might be due to invalid IL or missing references)
		//IL_067e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0686: Unknown result type (might be due to invalid IL or missing references)
		//IL_068b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0693: Unknown result type (might be due to invalid IL or missing references)
		//IL_0698: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0700: Unknown result type (might be due to invalid IL or missing references)
		m_LeisureProviderQuery = system.GetSetupQuery(ComponentType.ReadOnly<Game.Buildings.LeisureProvider>(), ComponentType.Exclude<Temp>(), ComponentType.Exclude<Deleted>(), ComponentType.Exclude<Destroyed>());
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[0];
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<LodgingProvider>(),
			ComponentType.ReadOnly<AttractivenessProvider>()
		};
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Destroyed>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_TouristTargetQuery = system.GetSetupQuery((EntityQueryDesc[])(object)array);
		m_SchoolQuery = system.GetSetupQuery(ComponentType.ReadOnly<Game.Buildings.School>(), ComponentType.ReadOnly<PrefabRef>(), ComponentType.Exclude<Game.Buildings.ServiceUpgrade>(), ComponentType.Exclude<Deleted>(), ComponentType.Exclude<Destroyed>(), ComponentType.Exclude<Temp>());
		m_FreeWorkplaceQuery = system.GetSetupQuery(ComponentType.ReadOnly<FreeWorkplaces>(), ComponentType.Exclude<Deleted>(), ComponentType.Exclude<Destroyed>(), ComponentType.Exclude<Temp>());
		m_AttractionQuery = system.GetSetupQuery(ComponentType.ReadOnly<Building>(), ComponentType.ReadOnly<AttractivenessProvider>(), ComponentType.Exclude<Deleted>(), ComponentType.Exclude<Destroyed>(), ComponentType.Exclude<Temp>());
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Building>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Abandoned>(),
			ComponentType.ReadOnly<Game.Buildings.Park>()
		};
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Destroyed>(),
			ComponentType.ReadOnly<Temp>()
		};
		EntityQueryDesc val2 = val;
		m_HomelessShelterQuery = system.GetSetupQuery(val2);
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<PropertyOnMarket>(),
			ComponentType.ReadOnly<ResidentialProperty>(),
			ComponentType.ReadOnly<Building>()
		};
		val.None = (ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Abandoned>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Destroyed>(),
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Condemned>()
		};
		EntityQueryDesc val3 = val;
		m_FindHomeQuery = system.GetSetupQuery(val2, val3);
		m_GroundPollutionSystem = ((ComponentSystemBase)system).World.GetOrCreateSystemManaged<GroundPollutionSystem>();
		m_AirPollutionSystem = ((ComponentSystemBase)system).World.GetOrCreateSystemManaged<AirPollutionSystem>();
		m_NoisePollutionSystem = ((ComponentSystemBase)system).World.GetOrCreateSystemManaged<NoisePollutionSystem>();
		m_TelecomCoverageSystem = ((ComponentSystemBase)system).World.GetOrCreateSystemManaged<TelecomCoverageSystem>();
		m_CitySystem = ((ComponentSystemBase)system).World.GetOrCreateSystemManaged<CitySystem>();
		m_ResourceSystem = ((ComponentSystemBase)system).World.GetOrCreateSystemManaged<ResourceSystem>();
		m_LeisureSystem = ((ComponentSystemBase)system).World.GetOrCreateSystemManaged<LeisureSystem>();
		m_TaxSystem = ((ComponentSystemBase)system).World.GetOrCreateSystemManaged<TaxSystem>();
		m_EconomyParameterQuery = system.GetSetupQuery(ComponentType.ReadOnly<EconomyParameterData>());
		m_HealthcareParameterQuery = system.GetSetupQuery(ComponentType.ReadOnly<HealthcareParameterData>());
		m_ParkParameterQuery = system.GetSetupQuery(ComponentType.ReadOnly<ParkParameterData>());
		m_EducationParameterQuery = system.GetSetupQuery(ComponentType.ReadOnly<EducationParameterData>());
		m_TelecomParameterQuery = system.GetSetupQuery(ComponentType.ReadOnly<TelecomParameterData>());
		m_GarbageParameterQuery = system.GetSetupQuery(ComponentType.ReadOnly<GarbageParameterData>());
		m_PoliceParameterQuery = system.GetSetupQuery(ComponentType.ReadOnly<PoliceConfigurationData>());
		m_CitizenHappinessParameterQuery = system.GetSetupQuery(ComponentType.ReadOnly<CitizenHappinessParameterData>());
		m_ServiceFeeParameterQuery = system.GetSetupQuery(ComponentType.ReadOnly<ServiceFeeParameterData>());
		m_EntityType = ((ComponentSystemBase)system).GetEntityTypeHandle();
		m_ServiceAvailableType = ((ComponentSystemBase)system).GetComponentTypeHandle<ServiceAvailable>(true);
		m_FreeWorkplaceType = ((ComponentSystemBase)system).GetComponentTypeHandle<FreeWorkplaces>(true);
		m_WorkProviderType = ((ComponentSystemBase)system).GetComponentTypeHandle<WorkProvider>(true);
		m_CityServiceType = ((ComponentSystemBase)system).GetComponentTypeHandle<CityServiceUpkeep>(true);
		m_BuildingType = ((ComponentSystemBase)system).GetComponentTypeHandle<Building>(true);
		m_PrefabRefType = ((ComponentSystemBase)system).GetComponentTypeHandle<PrefabRef>(true);
		m_RenterType = ((ComponentSystemBase)system).GetBufferTypeHandle<Renter>(true);
		m_StudentType = ((ComponentSystemBase)system).GetBufferTypeHandle<Game.Buildings.Student>(true);
		m_UpgradeType = ((ComponentSystemBase)system).GetBufferTypeHandle<InstalledUpgrade>(true);
		m_Buildings = ((SystemBase)system).GetComponentLookup<Building>(true);
		m_Households = ((SystemBase)system).GetComponentLookup<Household>(true);
		m_HomelessHouseholds = ((SystemBase)system).GetComponentLookup<HomelessHousehold>(true);
		m_OutsideConnections = ((SystemBase)system).GetComponentLookup<Game.Objects.OutsideConnection>(true);
		m_ServiceDatas = ((SystemBase)system).GetComponentLookup<ServiceCompanyData>(true);
		m_Workers = ((SystemBase)system).GetComponentLookup<Worker>(true);
		m_Students = ((SystemBase)system).GetComponentLookup<Game.Citizens.Student>(true);
		m_Citizens = ((SystemBase)system).GetComponentLookup<Citizen>(true);
		m_TouristHouseholds = ((SystemBase)system).GetComponentLookup<TouristHousehold>(true);
		m_HealthProblems = ((SystemBase)system).GetComponentLookup<HealthProblem>(true);
		m_Transforms = ((SystemBase)system).GetComponentLookup<Transform>(true);
		m_BuildingDatas = ((SystemBase)system).GetComponentLookup<Building>(true);
		m_Efficiencies = ((SystemBase)system).GetBufferLookup<Efficiency>(true);
		m_AttractivenessProviders = ((SystemBase)system).GetComponentLookup<AttractivenessProvider>(true);
		m_LodgingProviders = ((SystemBase)system).GetComponentLookup<LodgingProvider>(true);
		m_PropertiesOnMarket = ((SystemBase)system).GetComponentLookup<PropertyOnMarket>(true);
		m_PropertyRenters = ((SystemBase)system).GetComponentLookup<PropertyRenter>(true);
		m_Crimes = ((SystemBase)system).GetComponentLookup<CrimeProducer>(true);
		m_Parks = ((SystemBase)system).GetComponentLookup<Game.Buildings.Park>(true);
		m_Abandoneds = ((SystemBase)system).GetComponentLookup<Abandoned>(true);
		m_ElectricityConsumers = ((SystemBase)system).GetComponentLookup<ElectricityConsumer>(true);
		m_WaterConsumers = ((SystemBase)system).GetComponentLookup<WaterConsumer>(true);
		m_GarbageProducers = ((SystemBase)system).GetComponentLookup<GarbageProducer>(true);
		m_MailProducers = ((SystemBase)system).GetComponentLookup<MailProducer>(true);
		m_PathInfos = ((SystemBase)system).GetComponentLookup<PathInformation>(true);
		m_Prefabs = ((SystemBase)system).GetComponentLookup<PrefabRef>(true);
		m_IndustrialProcessDatas = ((SystemBase)system).GetComponentLookup<IndustrialProcessData>(true);
		m_LeisureProviderDatas = ((SystemBase)system).GetComponentLookup<LeisureProviderData>(true);
		m_ResourceDatas = ((SystemBase)system).GetComponentLookup<ResourceData>(true);
		m_SchoolDatas = ((SystemBase)system).GetComponentLookup<SchoolData>(true);
		m_PrefabBuildingDatas = ((SystemBase)system).GetComponentLookup<BuildingData>(true);
		m_BuildingProperties = ((SystemBase)system).GetComponentLookup<BuildingPropertyData>(true);
		m_SpawnableDatas = ((SystemBase)system).GetComponentLookup<SpawnableBuildingData>(true);
		m_Lockeds = ((SystemBase)system).GetComponentLookup<Locked>(true);
		m_Resources = ((SystemBase)system).GetBufferLookup<Resources>(true);
		m_ServiceDistricts = ((SystemBase)system).GetBufferLookup<ServiceDistrict>(true);
		m_Availabilities = ((SystemBase)system).GetBufferLookup<ResourceAvailability>(true);
		m_ServiceCoverages = ((SystemBase)system).GetBufferLookup<Game.Net.ServiceCoverage>(true);
		m_Renters = ((SystemBase)system).GetBufferLookup<Renter>(true);
		m_CityModifiers = ((SystemBase)system).GetBufferLookup<CityModifier>(true);
		m_OwnedVehicles = ((SystemBase)system).GetBufferLookup<OwnedVehicle>(true);
		m_HouseholdCitizens = ((SystemBase)system).GetBufferLookup<HouseholdCitizen>(true);
	}

	public JobHandle SetupLeisureTarget(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
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
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		((EntityTypeHandle)(ref m_EntityType)).Update((SystemBase)(object)system);
		m_ServiceAvailableType.Update((SystemBase)(object)system);
		m_PrefabRefType.Update((SystemBase)(object)system);
		m_LeisureProviderDatas.Update((SystemBase)(object)system);
		m_Resources.Update((SystemBase)(object)system);
		m_IndustrialProcessDatas.Update((SystemBase)(object)system);
		m_ResourceDatas.Update((SystemBase)(object)system);
		m_ServiceDatas.Update((SystemBase)(object)system);
		m_BuildingDatas.Update((SystemBase)(object)system);
		JobHandle val = JobChunkExtensions.ScheduleParallel<SetupLeisureTargetJob>(new SetupLeisureTargetJob
		{
			m_EntityType = m_EntityType,
			m_ServiceAvailableType = m_ServiceAvailableType,
			m_PrefabType = m_PrefabRefType,
			m_LeisureProviderDatas = m_LeisureProviderDatas,
			m_Resources = m_Resources,
			m_IndustrialProcessDatas = m_IndustrialProcessDatas,
			m_ResourceDatas = m_ResourceDatas,
			m_ServiceDatas = m_ServiceDatas,
			m_BuildingDatas = m_BuildingDatas,
			m_ResourcePrefabs = m_ResourceSystem.GetPrefabs(),
			m_SetupData = setupData,
			m_LeisureSystemUpdateInterval = m_LeisureSystem.GetUpdateInterval(SystemUpdatePhase.GameSimulation)
		}, m_LeisureProviderQuery, inputDeps);
		m_ResourceSystem.AddPrefabsReader(val);
		return val;
	}

	public JobHandle SetupTouristTarget(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		((EntityTypeHandle)(ref m_EntityType)).Update((SystemBase)(object)system);
		m_TouristHouseholds.Update((SystemBase)(object)system);
		m_LodgingProviders.Update((SystemBase)(object)system);
		m_PropertyRenters.Update((SystemBase)(object)system);
		m_BuildingDatas.Update((SystemBase)(object)system);
		m_Availabilities.Update((SystemBase)(object)system);
		return JobChunkExtensions.ScheduleParallel<SetupTouristTargetJob>(new SetupTouristTargetJob
		{
			m_EntityType = m_EntityType,
			m_LodgingProviders = m_LodgingProviders,
			m_TouristHouseholds = m_TouristHouseholds,
			m_PropertyRenters = m_PropertyRenters,
			m_BuildingDatas = m_BuildingDatas,
			m_ResourceAvailabilityBufs = m_Availabilities,
			m_SetupData = setupData
		}, m_TouristTargetQuery, inputDeps);
	}

	public JobHandle SetupSchoolSeekerTo(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
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
		m_PrefabRefType.Update((SystemBase)(object)system);
		m_StudentType.Update((SystemBase)(object)system);
		m_UpgradeType.Update((SystemBase)(object)system);
		m_SchoolDatas.Update((SystemBase)(object)system);
		m_Efficiencies.Update((SystemBase)(object)system);
		m_ServiceDistricts.Update((SystemBase)(object)system);
		return JobChunkExtensions.ScheduleParallel<SetupSchoolSeekerToJob>(new SetupSchoolSeekerToJob
		{
			m_EntityType = m_EntityType,
			m_PrefabRefType = m_PrefabRefType,
			m_StudentType = m_StudentType,
			m_UpgradeType = m_UpgradeType,
			m_SchoolDatas = m_SchoolDatas,
			m_Efficiencies = m_Efficiencies,
			m_ServiceDistricts = m_ServiceDistricts,
			m_SetupData = setupData
		}, m_SchoolQuery, inputDeps);
	}

	public JobHandle SetupJobSeekerTo(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		((EntityTypeHandle)(ref m_EntityType)).Update((SystemBase)(object)system);
		m_FreeWorkplaceType.Update((SystemBase)(object)system);
		m_WorkProviderType.Update((SystemBase)(object)system);
		m_CityServiceType.Update((SystemBase)(object)system);
		m_OutsideConnections.Update((SystemBase)(object)system);
		return JobChunkExtensions.ScheduleParallel<SetupJobSeekerToJob>(new SetupJobSeekerToJob
		{
			m_EntityType = m_EntityType,
			m_FreeWorkplaceType = m_FreeWorkplaceType,
			m_WorkProviderType = m_WorkProviderType,
			m_CityServiceType = m_CityServiceType,
			m_OutsideConnections = m_OutsideConnections,
			m_SetupData = setupData
		}, m_FreeWorkplaceQuery, inputDeps);
	}

	public JobHandle SetupHomeless(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		((EntityTypeHandle)(ref m_EntityType)).Update((SystemBase)(object)system);
		m_RenterType.Update((SystemBase)(object)system);
		m_PrefabRefType.Update((SystemBase)(object)system);
		m_BuildingType.Update((SystemBase)(object)system);
		m_PrefabBuildingDatas.Update((SystemBase)(object)system);
		m_ServiceCoverages.Update((SystemBase)(object)system);
		return JobChunkExtensions.ScheduleParallel<SetupHomelessJob>(new SetupHomelessJob
		{
			m_EntityType = m_EntityType,
			m_RenterType = m_RenterType,
			m_PrefabType = m_PrefabRefType,
			m_BuildingType = m_BuildingType,
			m_BuildingProperties = m_BuildingProperties,
			m_BuildingDatas = m_PrefabBuildingDatas,
			m_Coverages = m_ServiceCoverages,
			m_SetupData = setupData
		}, m_HomelessShelterQuery, inputDeps);
	}

	public JobHandle SetupFindHome(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		((EntityTypeHandle)(ref m_EntityType)).Update((SystemBase)(object)system);
		m_RenterType.Update((SystemBase)(object)system);
		m_PrefabRefType.Update((SystemBase)(object)system);
		m_BuildingType.Update((SystemBase)(object)system);
		m_Buildings.Update((SystemBase)(object)system);
		m_Households.Update((SystemBase)(object)system);
		m_HomelessHouseholds.Update((SystemBase)(object)system);
		m_PrefabBuildingDatas.Update((SystemBase)(object)system);
		m_ServiceCoverages.Update((SystemBase)(object)system);
		m_PropertiesOnMarket.Update((SystemBase)(object)system);
		m_Availabilities.Update((SystemBase)(object)system);
		m_SpawnableDatas.Update((SystemBase)(object)system);
		m_BuildingProperties.Update((SystemBase)(object)system);
		m_BuildingDatas.Update((SystemBase)(object)system);
		m_PathInfos.Update((SystemBase)(object)system);
		m_Prefabs.Update((SystemBase)(object)system);
		m_Renters.Update((SystemBase)(object)system);
		m_ServiceCoverages.Update((SystemBase)(object)system);
		m_Workers.Update((SystemBase)(object)system);
		m_Students.Update((SystemBase)(object)system);
		m_PropertyRenters.Update((SystemBase)(object)system);
		m_ResourceDatas.Update((SystemBase)(object)system);
		m_Citizens.Update((SystemBase)(object)system);
		m_Crimes.Update((SystemBase)(object)system);
		m_Lockeds.Update((SystemBase)(object)system);
		m_Transforms.Update((SystemBase)(object)system);
		m_CityModifiers.Update((SystemBase)(object)system);
		m_HealthProblems.Update((SystemBase)(object)system);
		m_HouseholdCitizens.Update((SystemBase)(object)system);
		m_OwnedVehicles.Update((SystemBase)(object)system);
		m_Abandoneds.Update((SystemBase)(object)system);
		m_Parks.Update((SystemBase)(object)system);
		m_ElectricityConsumers.Update((SystemBase)(object)system);
		m_WaterConsumers.Update((SystemBase)(object)system);
		m_GarbageProducers.Update((SystemBase)(object)system);
		m_MailProducers.Update((SystemBase)(object)system);
		m_Resources.Update((SystemBase)(object)system);
		JobHandle dependencies;
		JobHandle dependencies2;
		JobHandle dependencies3;
		JobHandle dependencies4;
		return JobChunkExtensions.ScheduleParallel<SetupFindHomeJob>(new SetupFindHomeJob
		{
			m_EntityType = m_EntityType,
			m_RenterType = m_RenterType,
			m_PrefabType = m_PrefabRefType,
			m_Buildings = m_Buildings,
			m_Households = m_Households,
			m_HomelessHouseholds = m_HomelessHouseholds,
			m_BuildingDatas = m_PrefabBuildingDatas,
			m_Coverages = m_ServiceCoverages,
			m_PropertiesOnMarket = m_PropertiesOnMarket,
			m_Availabilities = m_Availabilities,
			m_SpawnableDatas = m_SpawnableDatas,
			m_BuildingProperties = m_BuildingProperties,
			m_PrefabRefs = m_Prefabs,
			m_ServiceCoverages = m_ServiceCoverages,
			m_Citizens = m_Citizens,
			m_Crimes = m_Crimes,
			m_Lockeds = m_Lockeds,
			m_Transforms = m_Transforms,
			m_CityModifiers = m_CityModifiers,
			m_HouseholdCitizens = m_HouseholdCitizens,
			m_Abandoneds = m_Abandoneds,
			m_Parks = m_Parks,
			m_ElectricityConsumers = m_ElectricityConsumers,
			m_WaterConsumers = m_WaterConsumers,
			m_GarbageProducers = m_GarbageProducers,
			m_MailProducers = m_MailProducers,
			m_HealthProblems = m_HealthProblems,
			m_Workers = m_Workers,
			m_Students = m_Students,
			m_ResourcesBufs = m_Resources,
			m_TaxRates = m_TaxSystem.GetTaxRates(),
			m_PollutionMap = m_GroundPollutionSystem.GetMap(readOnly: true, out dependencies),
			m_AirPollutionMap = m_AirPollutionSystem.GetMap(readOnly: true, out dependencies2),
			m_NoiseMap = m_NoisePollutionSystem.GetMap(readOnly: true, out dependencies3),
			m_TelecomCoverages = m_TelecomCoverageSystem.GetData(readOnly: true, out dependencies4),
			m_HealthcareParameters = ((EntityQuery)(ref m_HealthcareParameterQuery)).GetSingleton<HealthcareParameterData>(),
			m_ParkParameters = ((EntityQuery)(ref m_ParkParameterQuery)).GetSingleton<ParkParameterData>(),
			m_EducationParameters = ((EntityQuery)(ref m_EducationParameterQuery)).GetSingleton<EducationParameterData>(),
			m_EconomyParameters = ((EntityQuery)(ref m_EconomyParameterQuery)).GetSingleton<EconomyParameterData>(),
			m_TelecomParameters = ((EntityQuery)(ref m_TelecomParameterQuery)).GetSingleton<TelecomParameterData>(),
			m_GarbageParameters = ((EntityQuery)(ref m_GarbageParameterQuery)).GetSingleton<GarbageParameterData>(),
			m_PoliceParameters = ((EntityQuery)(ref m_PoliceParameterQuery)).GetSingleton<PoliceConfigurationData>(),
			m_ServiceFeeParameterData = ((EntityQuery)(ref m_ServiceFeeParameterQuery)).GetSingleton<ServiceFeeParameterData>(),
			m_CitizenHappinessParameterData = ((EntityQuery)(ref m_CitizenHappinessParameterQuery)).GetSingleton<CitizenHappinessParameterData>(),
			m_City = m_CitySystem.City,
			m_SetupData = setupData
		}, m_FindHomeQuery, JobUtils.CombineDependencies(inputDeps, dependencies, dependencies2, dependencies3, dependencies4));
	}

	public JobHandle SetupAttraction(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		((EntityTypeHandle)(ref m_EntityType)).Update((SystemBase)(object)system);
		m_AttractivenessProviders.Update((SystemBase)(object)system);
		return JobChunkExtensions.ScheduleParallel<SetupAttractionJob>(new SetupAttractionJob
		{
			m_EntityType = m_EntityType,
			m_AttractivenessProviders = m_AttractivenessProviders,
			m_SetupData = setupData
		}, m_AttractionQuery, inputDeps);
	}
}
