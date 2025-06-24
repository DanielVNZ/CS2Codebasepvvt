using System.Runtime.CompilerServices;
using System.Threading;
using Colossal;
using Colossal.Collections;
using Colossal.Mathematics;
using Colossal.PSI.Common;
using Game.Achievements;
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
using Game.Prefabs;
using Game.Routes;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class ExtractorCompanySystem : GameSystemBase
{
	[BurstCompile]
	private struct ExtractorJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		public BufferTypeHandle<Resources> m_CompanyResourceType;

		[ReadOnly]
		public BufferTypeHandle<Employee> m_EmployeeType;

		[ReadOnly]
		public ComponentTypeHandle<PropertyRenter> m_PropertyType;

		public ComponentTypeHandle<TaxPayer> m_TaxPayerType;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> m_IndustrialProcessDatas;

		[ReadOnly]
		public ComponentLookup<StorageLimitData> m_StorageLimitDatas;

		[NativeDisableParallelForRestriction]
		public BufferLookup<Efficiency> m_BuildingEfficiencies;

		[ReadOnly]
		public ComponentLookup<WorkplaceData> m_WorkplaceDatas;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> m_SpawnableDatas;

		[ReadOnly]
		public ComponentLookup<Attached> m_Attached;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> m_SubAreas;

		[ReadOnly]
		public BufferLookup<SubRoute> m_SubRouteBufs;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubNet> m_SubNetsBufs;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> m_RouteWaypointBufs;

		[ReadOnly]
		public ComponentLookup<Connected> m_Connecteds;

		[ReadOnly]
		public ComponentLookup<Owner> m_Owners;

		[ReadOnly]
		public ComponentLookup<ExtractorFacilityData> m_ExtractorFacilityDatas;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifiers;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Extractor> m_ExtractorAreas;

		[ReadOnly]
		public ComponentLookup<Geometry> m_GeometryData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<ExtractorAreaData> m_ExtractorAreaDatas;

		[ReadOnly]
		public ComponentLookup<Citizen> m_Citizens;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.ServiceUpgrade> m_ServiceUpgradeData;

		[ReadOnly]
		public ComponentLookup<Edge> m_Edges;

		[ReadOnly]
		public ComponentLookup<PlaceableObjectData> m_PlaceableObjectDatas;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Game.Net.ResourceConnection> m_ResourceConnectionData;

		[ReadOnly]
		public BufferLookup<Game.Net.SubNet> m_SubNets;

		public EconomyParameterData m_EconomyParameters;

		public ExtractorParameterData m_ExtractorParameters;

		[ReadOnly]
		public NativeArray<int> m_TaxRates;

		[ReadOnly]
		public ResourcePrefabs m_ResourcePrefabs;

		[ReadOnly]
		public ComponentLookup<ResourceData> m_ResourceDatas;

		public RandomSeed m_RandomSeed;

		public ParallelWriter m_CommandBuffer;

		public uint m_UpdateFrameIndex;

		public NativeArray<long> m_ProducedResources;

		[ReadOnly]
		public bool m_ShouldCheckOffshoreOilProduce;

		[ReadOnly]
		public bool m_ShouldCheckProducedFish;

		public Concurrent m_OffshoreOilProduceCounter;

		public Concurrent m_ProducedFishCounter;

		public ParallelWriter<ProductionSpecializationSystem.ProducedResource> m_ProductionQueue;

		[ReadOnly]
		public Entity m_City;

		[ReadOnly]
		public DeliveryTruckSelectData m_DeliveryTruckSelectData;

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
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_0569: Unknown result type (might be due to invalid IL or missing references)
			//IL_057a: Unknown result type (might be due to invalid IL or missing references)
			//IL_058c: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0683: Unknown result type (might be due to invalid IL or missing references)
			//IL_0622: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_06be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0631: Unknown result type (might be due to invalid IL or missing references)
			//IL_0408: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_042d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03da: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0443: Unknown result type (might be due to invalid IL or missing references)
			//IL_0476: Unknown result type (might be due to invalid IL or missing references)
			//IL_0481: Unknown result type (might be due to invalid IL or missing references)
			//IL_0529: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index != m_UpdateFrameIndex)
			{
				return;
			}
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			BufferAccessor<Resources> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Resources>(ref m_CompanyResourceType);
			BufferAccessor<Employee> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Employee>(ref m_EmployeeType);
			NativeArray<PropertyRenter> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PropertyRenter>(ref m_PropertyType);
			NativeArray<TaxPayer> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TaxPayer>(ref m_TaxPayerType);
			DynamicBuffer<CityModifier> modifiers = m_CityModifiers[m_City];
			DynamicBuffer<Efficiency> buffer = default(DynamicBuffer<Efficiency>);
			DynamicBuffer<Game.Areas.SubArea> val2 = default(DynamicBuffer<Game.Areas.SubArea>);
			DynamicBuffer<InstalledUpgrade> val3 = default(DynamicBuffer<InstalledUpgrade>);
			PrefabRef prefabRef = default(PrefabRef);
			ExtractorFacilityData extractorFacilityData = default(ExtractorFacilityData);
			DynamicBuffer<Game.Areas.SubArea> val4 = default(DynamicBuffer<Game.Areas.SubArea>);
			PrefabRef prefabRef2 = default(PrefabRef);
			ExtractorAreaData extractorAreaData = default(ExtractorAreaData);
			Game.Net.ResourceConnection resourceConnection = default(Game.Net.ResourceConnection);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity val = nativeArray[i];
				Entity property = nativeArray2[i].m_Property;
				DynamicBuffer<Resources> resources = bufferAccessor[i];
				Entity prefab = m_Prefabs[val].m_Prefab;
				Entity prefab2 = m_Prefabs[property].m_Prefab;
				IndustrialProcessData processData = m_IndustrialProcessDatas[prefab];
				StorageLimitData storageLimitData = m_StorageLimitDatas[prefab];
				if (m_Attached.HasComponent(property) && m_InstalledUpgrades.HasBuffer(m_Attached[property].m_Parent) && UpgradeUtils.TryGetCombinedComponent<StorageLimitData>(m_Attached[property].m_Parent, out StorageLimitData data, ref m_Prefabs, ref m_StorageLimitDatas, ref m_InstalledUpgrades))
				{
					storageLimitData.m_Limit += data.m_Limit;
				}
				_ = m_WorkplaceDatas[prefab];
				_ = m_SpawnableDatas[prefab2];
				int totalStorageUsed = EconomyUtils.GetTotalStorageUsed(resources);
				int num = storageLimitData.m_Limit - totalStorageUsed;
				if (!m_Attached.HasComponent(property))
				{
					continue;
				}
				Entity parent = m_Attached[property].m_Parent;
				float concentration;
				float size;
				bool bestConcentration = GetBestConcentration(processData.m_Output.m_Resource, parent, ref m_SubAreas, ref m_InstalledUpgrades, ref m_ExtractorAreas, ref m_GeometryData, ref m_Prefabs, ref m_ExtractorAreaDatas, m_ExtractorParameters, m_ResourcePrefabs, ref m_ResourceDatas, out concentration, out size);
				float buildingEfficiency = 1f;
				if (m_BuildingEfficiencies.TryGetBuffer(property, ref buffer))
				{
					if (processData.m_Output.m_Resource == Resource.Fish)
					{
						float value = 100f;
						CityUtils.ApplyModifier(ref value, modifiers, CityModifierType.IndustrialFishHubEfficiency);
						BuildingUtils.SetEfficiencyFactor(buffer, EfficiencyFactor.CityModifierFishHub, value / 100f);
					}
					BuildingUtils.SetEfficiencyFactor(buffer, EfficiencyFactor.NaturalResources, concentration);
					buildingEfficiency = BuildingUtils.GetEfficiency(buffer);
				}
				if (!bestConcentration)
				{
					continue;
				}
				int companyProductionPerDay = EconomyUtils.GetCompanyProductionPerDay(buildingEfficiency, isIndustrial: true, bufferAccessor2[i], processData, m_ResourcePrefabs, ref m_ResourceDatas, ref m_Citizens, ref m_EconomyParameters);
				float num2 = 1f * (float)companyProductionPerDay / (float)EconomyUtils.kCompanyUpdatesPerDay;
				num2 = math.min((float)num, num2);
				float num3 = 0f;
				bool requireNaturalResource = m_ResourceDatas[m_ResourcePrefabs[processData.m_Output.m_Resource]].m_RequireNaturalResource;
				if (m_SubAreas.TryGetBuffer(parent, ref val2))
				{
					for (int j = 0; j < val2.Length; j++)
					{
						num3 += ProcessArea(val2[j].m_Area, num2, concentration, size, requireNaturalResource);
					}
				}
				if (m_InstalledUpgrades.TryGetBuffer(parent, ref val3))
				{
					for (int k = 0; k < val3.Length; k++)
					{
						if (BuildingUtils.CheckOption(val3[k], BuildingOption.Inactive))
						{
							continue;
						}
						Entity stopObject = Entity.Null;
						if (m_Prefabs.TryGetComponent(val3[k].m_Upgrade, ref prefabRef) && m_ExtractorFacilityDatas.TryGetComponent(prefabRef.m_Prefab, ref extractorFacilityData))
						{
							if (((extractorFacilityData.m_Requirements & ExtractorRequirementFlags.RouteConnect) != ExtractorRequirementFlags.None && !CheckHaveValidRoute(parent, val3[k].m_Upgrade, out stopObject)) || ((extractorFacilityData.m_Requirements & ExtractorRequirementFlags.NetConnect) != ExtractorRequirementFlags.None && (!FindResourceConnectionNode(val3[k].m_Upgrade, out stopObject, out var connected) || !connected)))
							{
								continue;
							}
						}
						else if (m_SubAreas.TryGetBuffer(val3[k].m_Upgrade, ref val4))
						{
							bool flag = false;
							for (int l = 0; l < val4.Length; l++)
							{
								if (m_Prefabs.TryGetComponent(val4[l].m_Area, ref prefabRef2) && m_ExtractorAreaDatas.TryGetComponent(prefabRef2.m_Prefab, ref extractorAreaData) && extractorAreaData.m_MapFeature == MapFeature.Fish)
								{
									flag = true;
									break;
								}
							}
							if (flag && !CheckHaveValidRoute(parent, val3[k].m_Upgrade, out var _))
							{
								continue;
							}
						}
						if (m_SubAreas.TryGetBuffer(val3[k].m_Upgrade, ref val2))
						{
							float num4 = 0f;
							for (int m = 0; m < val2.Length; m++)
							{
								num4 += ProcessArea(val2[m].m_Area, num2, concentration, size, requireNaturalResource);
							}
							num3 += num4;
							if (m_ResourceConnectionData.TryGetComponent(stopObject, ref resourceConnection))
							{
								resourceConnection.m_Flow.y |= MathUtils.RoundToIntRandom(ref random, num4) << 1;
								m_ResourceConnectionData[stopObject] = resourceConnection;
							}
						}
					}
				}
				int num5 = math.min(num, MathUtils.RoundToIntRandom(ref random, num3));
				ResourceStack output = processData.m_Output;
				int industrialTaxRate = TaxSystem.GetIndustrialTaxRate(output.m_Resource, m_TaxRates);
				if (num5 > 0)
				{
					ref TaxPayer reference = ref CollectionUtils.ElementAt<TaxPayer>(nativeArray3, i);
					int num6 = EconomyUtils.GetCompanyProfitPerDay(buildingEfficiency, isIndustrial: true, bufferAccessor2[i], processData, m_ResourcePrefabs, ref m_ResourceDatas, ref m_Citizens, ref m_EconomyParameters) / EconomyUtils.kCompanyUpdatesPerDay;
					if (num6 > 0)
					{
						reference.m_AverageTaxRate = (int)math.round(math.lerp((float)reference.m_AverageTaxRate, (float)industrialTaxRate, (float)num6 / (float)(num6 + reference.m_UntaxedIncome)));
						reference.m_UntaxedIncome += num6;
					}
				}
				AddProducedResource(output.m_Resource, num5);
				if (m_ShouldCheckOffshoreOilProduce && output.m_Resource == Resource.Oil && m_PlaceableObjectDatas.HasComponent(prefab2) && (m_PlaceableObjectDatas[prefab2].m_Flags & Game.Objects.PlacementFlags.Shoreline) != Game.Objects.PlacementFlags.None)
				{
					((Concurrent)(ref m_OffshoreOilProduceCounter)).Increment(num5);
				}
				else if (m_ShouldCheckProducedFish && output.m_Resource == Resource.Fish)
				{
					((Concurrent)(ref m_ProducedFishCounter)).Increment(num5);
				}
				int num7 = EconomyUtils.AddResources(output.m_Resource, num5, resources);
				if (num < 100 || num7 > 3 * storageLimitData.m_Limit / 4)
				{
					m_DeliveryTruckSelectData.GetCapacityRange(output.m_Resource, out var _, out var max);
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<ResourceExporter>(unfilteredChunkIndex, val, new ResourceExporter
					{
						m_Resource = output.m_Resource,
						m_Amount = math.min(max, storageLimitData.m_Limit / 2)
					});
				}
			}
		}

		private bool FindResourceConnectionNode(Entity ownerEntity, out Entity node, out bool connected)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<Game.Net.SubNet> val = default(DynamicBuffer<Game.Net.SubNet>);
			if (m_SubNets.TryGetBuffer(ownerEntity, ref val))
			{
				Game.Net.ResourceConnection resourceConnection = default(Game.Net.ResourceConnection);
				for (int i = 0; i < val.Length; i++)
				{
					Game.Net.SubNet subNet = val[i];
					if (!m_ServiceUpgradeData.HasComponent(subNet.m_SubNet) && !m_Edges.HasComponent(subNet.m_SubNet) && m_ResourceConnectionData.TryGetComponent(subNet.m_SubNet, ref resourceConnection))
					{
						node = subNet.m_SubNet;
						connected = (resourceConnection.m_Flow.y & 1) != 0;
						return true;
					}
				}
			}
			node = Entity.Null;
			connected = false;
			return false;
		}

		private bool CheckHaveValidRoute(Entity placeholderEntity, Entity upgradeEntity, out Entity stopObject)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			bool flag2 = false;
			stopObject = Entity.Null;
			DynamicBuffer<SubRoute> val = default(DynamicBuffer<SubRoute>);
			if (m_SubRouteBufs.TryGetBuffer(placeholderEntity, ref val))
			{
				for (int i = 0; i < val.Length; i++)
				{
					flag = false;
					flag2 = false;
					if (m_RouteWaypointBufs.HasBuffer(val[i].m_Route))
					{
						for (int j = 0; j < m_RouteWaypointBufs[val[i].m_Route].Length; j++)
						{
							Entity waypoint = m_RouteWaypointBufs[val[i].m_Route][j].m_Waypoint;
							if (m_Connecteds.HasComponent(waypoint) && m_Owners.HasComponent(m_Connecteds[waypoint].m_Connected))
							{
								if (m_Owners[m_Connecteds[waypoint].m_Connected].m_Owner == upgradeEntity)
								{
									flag = true;
								}
								else if (m_Edges.HasComponent(m_Owners[m_Connecteds[waypoint].m_Connected].m_Owner))
								{
									stopObject = m_Connecteds[waypoint].m_Connected;
									flag2 = true;
								}
							}
						}
					}
					if (flag && flag2)
					{
						break;
					}
				}
			}
			return flag && flag2;
		}

		private unsafe void AddProducedResource(Resource resource, int amount)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			if (resource != Resource.NoResource)
			{
				long* unsafePtr = (long*)NativeArrayUnsafeUtility.GetUnsafePtr<long>(m_ProducedResources);
				unsafePtr += EconomyUtils.GetResourceIndex(resource);
				Interlocked.Add(ref *unsafePtr, amount);
				m_ProductionQueue.Enqueue(new ProductionSpecializationSystem.ProducedResource
				{
					m_Resource = resource,
					m_Amount = amount
				});
			}
		}

		private float ProcessArea(Entity area, float totalProduced, float totalConcentration, float totalSize, bool requireNaturalResources)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			Extractor extractor = default(Extractor);
			Geometry geometry = default(Geometry);
			if (!m_ExtractorAreas.TryGetComponent(area, ref extractor) || !m_GeometryData.TryGetComponent(area, ref geometry))
			{
				return 0f;
			}
			float num = totalProduced * geometry.m_SurfaceArea / math.max(1f, totalConcentration * totalSize);
			float num2 = 1f;
			Entity prefab = m_Prefabs[area].m_Prefab;
			ExtractorAreaData extractorAreaData = default(ExtractorAreaData);
			bool flag = m_ExtractorAreaDatas.TryGetComponent(prefab, ref extractorAreaData);
			if (flag)
			{
				num2 = extractorAreaData.m_WorkAmountFactor;
			}
			if (requireNaturalResources && flag && extractorAreaData.m_RequireNaturalResource)
			{
				float num3 = extractor.m_ResourceAmount - extractor.m_ExtractedAmount;
				float effectiveConcentration = GetEffectiveConcentration(m_ExtractorParameters, extractorAreaData.m_MapFeature, extractor.m_MaxConcentration);
				effectiveConcentration = math.min(1f, effectiveConcentration);
				num = math.clamp(num * effectiveConcentration, 0f, num3);
			}
			float num4 = GetExtractionMultiplier(area) * num;
			extractor.m_ExtractedAmount += num4;
			extractor.m_TotalExtracted += num4;
			extractor.m_WorkAmount += num * num2;
			m_ExtractorAreas[area] = extractor;
			return num;
		}

		private float GetExtractionMultiplier(Entity subArea)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			float result = 1f;
			PrefabRef prefabRef = default(PrefabRef);
			ExtractorAreaData extractorAreaData = default(ExtractorAreaData);
			if (m_Prefabs.TryGetComponent(subArea, ref prefabRef) && m_ExtractorAreaDatas.TryGetComponent((Entity)prefabRef, ref extractorAreaData))
			{
				result = extractorAreaData.m_MapFeature switch
				{
					MapFeature.FertileLand => m_ExtractorParameters.m_FertilityConsumption, 
					MapFeature.Fish => m_ExtractorParameters.m_FishConsumption, 
					MapFeature.Forest => m_ExtractorParameters.m_ForestConsumption, 
					_ => 1f, 
				};
			}
			return result;
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

		public BufferTypeHandle<Resources> __Game_Economy_Resources_RW_BufferTypeHandle;

		public ComponentLookup<Extractor> __Game_Areas_Extractor_RW_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Geometry> __Game_Areas_Geometry_RO_ComponentLookup;

		[ReadOnly]
		public BufferTypeHandle<Employee> __Game_Companies_Employee_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentTypeHandle;

		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		public ComponentTypeHandle<TaxPayer> __Game_Agents_TaxPayer_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> __Game_Prefabs_IndustrialProcessData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StorageLimitData> __Game_Companies_StorageLimitData_RO_ComponentLookup;

		public BufferLookup<Efficiency> __Game_Buildings_Efficiency_RW_BufferLookup;

		[ReadOnly]
		public ComponentLookup<WorkplaceData> __Game_Prefabs_WorkplaceData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Attached> __Game_Objects_Attached_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> __Game_Areas_SubArea_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<CityModifier> __Game_City_CityModifier_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ExtractorAreaData> __Game_Prefabs_ExtractorAreaData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<SubRoute> __Game_Routes_SubRoute_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> __Game_Routes_RouteWaypoint_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Connected> __Game_Routes_Connected_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ExtractorFacilityData> __Game_Prefabs_ExtractorFacilityData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.ServiceUpgrade> __Game_Buildings_ServiceUpgrade_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PlaceableObjectData> __Game_Prefabs_PlaceableObjectData_RO_ComponentLookup;

		public ComponentLookup<Game.Net.ResourceConnection> __Game_Net_ResourceConnection_RW_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubNet> __Game_Net_SubNet_RO_BufferLookup;

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
			__Game_Economy_Resources_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Resources>(false);
			__Game_Areas_Extractor_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Extractor>(false);
			__Game_Areas_Geometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Geometry>(true);
			__Game_Companies_Employee_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Employee>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PropertyRenter>(true);
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Agents_TaxPayer_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TaxPayer>(false);
			__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<IndustrialProcessData>(true);
			__Game_Companies_StorageLimitData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StorageLimitData>(true);
			__Game_Buildings_Efficiency_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Efficiency>(false);
			__Game_Prefabs_WorkplaceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WorkplaceData>(true);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableBuildingData>(true);
			__Game_Objects_Attached_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attached>(true);
			__Game_Areas_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.SubArea>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<InstalledUpgrade>(true);
			__Game_City_CityModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ExtractorAreaData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ExtractorAreaData>(true);
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
			__Game_Routes_SubRoute_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubRoute>(true);
			__Game_Routes_RouteWaypoint_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteWaypoint>(true);
			__Game_Routes_Connected_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Connected>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Prefabs_ExtractorFacilityData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ExtractorFacilityData>(true);
			__Game_Buildings_ServiceUpgrade_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.ServiceUpgrade>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Prefabs_PlaceableObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceableObjectData>(true);
			__Game_Net_ResourceConnection_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ResourceConnection>(false);
			__Game_Net_SubNet_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubNet>(true);
			__Game_Prefabs_ResourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceData>(true);
		}
	}

	private SimulationSystem m_SimulationSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private TaxSystem m_TaxSystem;

	private ResourceSystem m_ResourceSystem;

	private VehicleCapacitySystem m_VehicleCapacitySystem;

	private ProcessingCompanySystem m_ProcessingCompanySystem;

	private ProductionSpecializationSystem m_ProductionSpecializationSystem;

	private AchievementTriggerSystem m_AchievementTriggerSystem;

	private CitySystem m_CitySystem;

	private EntityQuery m_CompanyGroup;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_1012523227_0;

	private EntityQuery __query_1012523227_1;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / (EconomyUtils.kCompanyUpdatesPerDay * 16);
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_TaxSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TaxSystem>();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		m_VehicleCapacitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<VehicleCapacitySystem>();
		m_ProcessingCompanySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ProcessingCompanySystem>();
		m_ProductionSpecializationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ProductionSpecializationSystem>();
		m_AchievementTriggerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AchievementTriggerSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetExistingSystemManaged<CitySystem>();
		m_CompanyGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[8]
		{
			ComponentType.ReadOnly<Game.Companies.ExtractorCompany>(),
			ComponentType.ReadOnly<PropertyRenter>(),
			ComponentType.ReadWrite<Resources>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<WorkProvider>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.ReadWrite<CompanyData>(),
			ComponentType.ReadWrite<Employee>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_CompanyGroup);
		((ComponentSystemBase)this).RequireForUpdate<EconomyParameterData>();
		((ComponentSystemBase)this).RequireForUpdate<ExtractorParameterData>();
	}

	public static MapFeature GetRequiredMapFeature(Resource output, Entity lotPrefab, ResourcePrefabs resourcePrefabs, ComponentLookup<ResourceData> resourceDatas, ComponentLookup<ExtractorAreaData> extractorAreaDatas)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		ResourceData resourceData = default(ResourceData);
		ExtractorAreaData extractorAreaData = default(ExtractorAreaData);
		if (resourceDatas.TryGetComponent(resourcePrefabs[output], ref resourceData) && resourceData.m_RequireNaturalResource && extractorAreaDatas.TryGetComponent(lotPrefab, ref extractorAreaData) && extractorAreaData.m_RequireNaturalResource)
		{
			return extractorAreaData.m_MapFeature;
		}
		return MapFeature.None;
	}

	public static bool GetBestConcentration(Resource resource, Entity mainBuilding, ref BufferLookup<Game.Areas.SubArea> subAreas, ref BufferLookup<InstalledUpgrade> installedUpgrades, ref ComponentLookup<Extractor> extractors, ref ComponentLookup<Geometry> geometries, ref ComponentLookup<PrefabRef> prefabs, ref ComponentLookup<ExtractorAreaData> extractorDatas, ExtractorParameterData extractorParameters, ResourcePrefabs resourcePrefabs, ref ComponentLookup<ResourceData> resourceDatas, out float concentration, out float size)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		concentration = 0f;
		size = 0f;
		ResourceData resourceData = default(ResourceData);
		bool requireNaturalResource = resourceDatas.TryGetComponent(resourcePrefabs[resource], ref resourceData) && resourceData.m_RequireNaturalResource;
		DynamicBuffer<Game.Areas.SubArea> subAreas2 = default(DynamicBuffer<Game.Areas.SubArea>);
		if (subAreas.TryGetBuffer(mainBuilding, ref subAreas2))
		{
			GetBestConcentration(subAreas2, ref extractors, ref geometries, ref prefabs, ref extractorDatas, extractorParameters, requireNaturalResource, ref concentration, ref size);
		}
		DynamicBuffer<InstalledUpgrade> val = default(DynamicBuffer<InstalledUpgrade>);
		if (installedUpgrades.TryGetBuffer(mainBuilding, ref val))
		{
			for (int i = 0; i < val.Length; i++)
			{
				if (subAreas.TryGetBuffer(val[i].m_Upgrade, ref subAreas2))
				{
					GetBestConcentration(subAreas2, ref extractors, ref geometries, ref prefabs, ref extractorDatas, extractorParameters, requireNaturalResource, ref concentration, ref size);
				}
			}
		}
		concentration = math.min(1f, concentration / math.max(1f, size));
		return concentration > 0f;
	}

	private static void GetBestConcentration(DynamicBuffer<Game.Areas.SubArea> subAreas, ref ComponentLookup<Extractor> extractors, ref ComponentLookup<Geometry> geometries, ref ComponentLookup<PrefabRef> prefabs, ref ComponentLookup<ExtractorAreaData> extractorDatas, ExtractorParameterData extractorParameters, bool requireNaturalResource, ref float concentration, ref float size)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		Extractor extractor = default(Extractor);
		Geometry geometry = default(Geometry);
		PrefabRef prefabRef = default(PrefabRef);
		ExtractorAreaData extractorAreaData = default(ExtractorAreaData);
		for (int i = 0; i < subAreas.Length; i++)
		{
			Entity area = subAreas[i].m_Area;
			if (extractors.TryGetComponent(area, ref extractor) && geometries.TryGetComponent(area, ref geometry) && prefabs.TryGetComponent(area, ref prefabRef) && extractorDatas.TryGetComponent(prefabRef.m_Prefab, ref extractorAreaData))
			{
				if (requireNaturalResource && extractorAreaData.m_RequireNaturalResource)
				{
					float effectiveConcentration = GetEffectiveConcentration(extractorParameters, extractorAreaData.m_MapFeature, extractor.m_MaxConcentration);
					effectiveConcentration = math.min(1f, effectiveConcentration);
					concentration += effectiveConcentration * geometry.m_SurfaceArea;
					size += geometry.m_SurfaceArea;
				}
				else
				{
					concentration += geometry.m_SurfaceArea;
					size += geometry.m_SurfaceArea;
				}
			}
		}
	}

	public static float GetEffectiveConcentration(ExtractorParameterData extractorParameters, MapFeature feature, float concentration)
	{
		return math.min(1f, concentration / feature switch
		{
			MapFeature.Oil => extractorParameters.m_FullOil, 
			MapFeature.FertileLand => extractorParameters.m_FullFertility, 
			MapFeature.Fish => extractorParameters.m_FullFish, 
			MapFeature.Ore => extractorParameters.m_FullOre, 
			_ => 1f, 
		});
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0507: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0511: Unknown result type (might be due to invalid IL or missing references)
		//IL_0516: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_053d: Unknown result type (might be due to invalid IL or missing references)
		uint updateFrame = SimulationUtils.GetUpdateFrame(m_SimulationSystem.frameIndex, EconomyUtils.kCompanyUpdatesPerDay, 16);
		ExtractorJob extractorJob = new ExtractorJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CompanyResourceType = InternalCompilerInterface.GetBufferTypeHandle<Resources>(ref __TypeHandle.__Game_Economy_Resources_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ExtractorAreas = InternalCompilerInterface.GetComponentLookup<Extractor>(ref __TypeHandle.__Game_Areas_Extractor_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GeometryData = InternalCompilerInterface.GetComponentLookup<Geometry>(ref __TypeHandle.__Game_Areas_Geometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EmployeeType = InternalCompilerInterface.GetBufferTypeHandle<Employee>(ref __TypeHandle.__Game_Companies_Employee_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyType = InternalCompilerInterface.GetComponentTypeHandle<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TaxPayerType = InternalCompilerInterface.GetComponentTypeHandle<TaxPayer>(ref __TypeHandle.__Game_Agents_TaxPayer_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_IndustrialProcessDatas = InternalCompilerInterface.GetComponentLookup<IndustrialProcessData>(ref __TypeHandle.__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StorageLimitDatas = InternalCompilerInterface.GetComponentLookup<StorageLimitData>(ref __TypeHandle.__Game_Companies_StorageLimitData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingEfficiencies = InternalCompilerInterface.GetBufferLookup<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WorkplaceDatas = InternalCompilerInterface.GetComponentLookup<WorkplaceData>(ref __TypeHandle.__Game_Prefabs_WorkplaceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnableDatas = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Attached = InternalCompilerInterface.GetComponentLookup<Attached>(ref __TypeHandle.__Game_Objects_Attached_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubAreas = InternalCompilerInterface.GetBufferLookup<Game.Areas.SubArea>(ref __TypeHandle.__Game_Areas_SubArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgrades = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CityModifiers = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ExtractorAreaDatas = InternalCompilerInterface.GetComponentLookup<ExtractorAreaData>(ref __TypeHandle.__Game_Prefabs_ExtractorAreaData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Citizens = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubRouteBufs = InternalCompilerInterface.GetBufferLookup<SubRoute>(ref __TypeHandle.__Game_Routes_SubRoute_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteWaypointBufs = InternalCompilerInterface.GetBufferLookup<RouteWaypoint>(ref __TypeHandle.__Game_Routes_RouteWaypoint_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Connecteds = InternalCompilerInterface.GetComponentLookup<Connected>(ref __TypeHandle.__Game_Routes_Connected_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Owners = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ExtractorFacilityDatas = InternalCompilerInterface.GetComponentLookup<ExtractorFacilityData>(ref __TypeHandle.__Game_Prefabs_ExtractorFacilityData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceUpgradeData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.ServiceUpgrade>(ref __TypeHandle.__Game_Buildings_ServiceUpgrade_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Edges = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PlaceableObjectDatas = InternalCompilerInterface.GetComponentLookup<PlaceableObjectData>(ref __TypeHandle.__Game_Prefabs_PlaceableObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceConnectionData = InternalCompilerInterface.GetComponentLookup<Game.Net.ResourceConnection>(ref __TypeHandle.__Game_Net_ResourceConnection_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubNets = InternalCompilerInterface.GetBufferLookup<Game.Net.SubNet>(ref __TypeHandle.__Game_Net_SubNet_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourcePrefabs = m_ResourceSystem.GetPrefabs(),
			m_ResourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TaxRates = m_TaxSystem.GetTaxRates(),
			m_EconomyParameters = ((EntityQuery)(ref __query_1012523227_0)).GetSingleton<EconomyParameterData>(),
			m_ExtractorParameters = ((EntityQuery)(ref __query_1012523227_1)).GetSingleton<ExtractorParameterData>()
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		extractorJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		extractorJob.m_City = m_CitySystem.City;
		extractorJob.m_RandomSeed = RandomSeed.Next();
		extractorJob.m_UpdateFrameIndex = updateFrame;
		IAchievement val2 = default(IAchievement);
		extractorJob.m_ShouldCheckOffshoreOilProduce = PlatformManager.instance.achievementsEnabled && PlatformManager.instance.GetAchievement(Game.Achievements.Achievements.ADifferentPlatformer, ref val2) && !val2.achieved;
		IAchievement val3 = default(IAchievement);
		extractorJob.m_ShouldCheckProducedFish = PlatformManager.instance.achievementsEnabled && PlatformManager.instance.GetAchievement(Game.Achievements.Achievements.HowMuchIsTheFish, ref val3) && !val3.achieved;
		extractorJob.m_OffshoreOilProduceCounter = ((NativeCounter)(ref m_AchievementTriggerSystem.m_OffshoreOilProduceCounter)).ToConcurrent();
		extractorJob.m_ProducedFishCounter = ((NativeCounter)(ref m_AchievementTriggerSystem.m_ProducedFishCounter)).ToConcurrent();
		extractorJob.m_ProducedResources = m_ProcessingCompanySystem.GetProducedResourcesArray(out var dependencies);
		extractorJob.m_ProductionQueue = m_ProductionSpecializationSystem.GetQueue(out var deps).AsParallelWriter();
		extractorJob.m_DeliveryTruckSelectData = m_VehicleCapacitySystem.GetDeliveryTruckSelectData();
		ExtractorJob extractorJob2 = extractorJob;
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<ExtractorJob>(extractorJob2, m_CompanyGroup, JobHandle.CombineDependencies(dependencies, deps, ((SystemBase)this).Dependency));
		m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
		m_TaxSystem.AddReader(((SystemBase)this).Dependency);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<EconomyParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_1012523227_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		val2 = ((EntityQueryBuilder)(ref val)).WithAll<ExtractorParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_1012523227_1 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public ExtractorCompanySystem()
	{
	}
}
