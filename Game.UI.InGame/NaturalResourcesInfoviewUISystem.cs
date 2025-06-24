using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.UI.Binding;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Companies;
using Game.Economy;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class NaturalResourcesInfoviewUISystem : InfoviewUISystemBase
{
	private enum Result
	{
		FertilityAmount,
		ForestAmount,
		OilAmount,
		OreAmount,
		FishAmount,
		FertilityExtraction,
		ForestExtraction,
		OilExtraction,
		OreExtraction,
		FertilityRenewal,
		ForestRenewal,
		FishExtraction,
		FishRenewal,
		Count
	}

	[BurstCompile]
	private struct UpdateResourcesJob : IJobChunk
	{
		[ReadOnly]
		public BufferTypeHandle<MapFeatureElement> m_MapFeatureElementHandle;

		public NativeArray<float> m_Results;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			BufferAccessor<MapFeatureElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<MapFeatureElement>(ref m_MapFeatureElementHandle);
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 0f;
			float num7 = 0f;
			float num8 = 0f;
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				DynamicBuffer<MapFeatureElement> val = bufferAccessor[i];
				num += val[4].m_Amount;
				num2 += val[5].m_Amount;
				num7 += val[8].m_Amount;
				num8 += val[8].m_RenewalRate;
				num3 += val[3].m_Amount;
				num4 += val[3].m_RenewalRate;
				num5 += val[2].m_Amount;
				num6 += val[2].m_RenewalRate;
			}
			ref NativeArray<float> reference = ref m_Results;
			reference[0] = reference[0] + num5;
			reference = ref m_Results;
			reference[9] = reference[9] + num6;
			reference = ref m_Results;
			reference[1] = reference[1] + num3;
			reference = ref m_Results;
			reference[10] = reference[10] + num4;
			reference = ref m_Results;
			reference[2] = reference[2] + num;
			reference = ref m_Results;
			reference[3] = reference[3] + num2;
			reference = ref m_Results;
			reference[4] = reference[4] + num7;
			reference = ref m_Results;
			reference[12] = reference[12] + num8;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct UpdateExtractionJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityHandle;

		[ReadOnly]
		public ComponentTypeHandle<PropertyRenter> m_PropertyRenterHandle;

		[ReadOnly]
		public ComponentTypeHandle<WorkProvider> m_WorkProviderHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefHandle;

		[ReadOnly]
		public ComponentLookup<Attached> m_AttachedFromEntity;

		[ReadOnly]
		public ComponentLookup<Extractor> m_ExtractorsFromEntity;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefFromEntity;

		[ReadOnly]
		public ComponentLookup<ExtractorAreaData> m_ExtractorAreaDataFromEntity;

		[ReadOnly]
		public BufferLookup<Efficiency> m_BuildingEfficiencyFromEntity;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> m_SpawnableBuildingDataFromEntity;

		[ReadOnly]
		public ComponentLookup<WorkplaceData> m_WorkplaceDataFromEntity;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> m_IndustrialProcessDataFromEntity;

		[ReadOnly]
		public ComponentLookup<ResourceData> m_ResourceDatas;

		[ReadOnly]
		public ResourcePrefabs m_ResourcePrefabs;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> m_SubAreaBufs;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

		public EconomyParameterData m_EconomyParameters;

		public NativeArray<float> m_Result;

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
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityHandle);
			NativeArray<PropertyRenter> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PropertyRenter>(ref m_PropertyRenterHandle);
			NativeArray<WorkProvider> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WorkProvider>(ref m_WorkProviderHandle);
			NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefHandle);
			float totalOil = 0f;
			float totalOre = 0f;
			float totalForest = 0f;
			float totalFertility = 0f;
			float totalFish = 0f;
			IndustrialProcessData processData = default(IndustrialProcessData);
			WorkplaceData workplaceData = default(WorkplaceData);
			PrefabRef prefabRef2 = default(PrefabRef);
			Attached attached = default(Attached);
			SpawnableBuildingData spawnableBuildingData = default(SpawnableBuildingData);
			DynamicBuffer<Game.Areas.SubArea> subAreas = default(DynamicBuffer<Game.Areas.SubArea>);
			DynamicBuffer<InstalledUpgrade> val = default(DynamicBuffer<InstalledUpgrade>);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				_ = nativeArray[i];
				PropertyRenter propertyRenter = nativeArray2[i];
				WorkProvider workProvider = nativeArray3[i];
				PrefabRef prefabRef = nativeArray4[i];
				if (!m_IndustrialProcessDataFromEntity.TryGetComponent(prefabRef.m_Prefab, ref processData) || !m_WorkplaceDataFromEntity.TryGetComponent(prefabRef.m_Prefab, ref workplaceData) || !m_PrefabRefFromEntity.TryGetComponent(propertyRenter.m_Property, ref prefabRef2) || !m_AttachedFromEntity.TryGetComponent(propertyRenter.m_Property, ref attached) || !m_SpawnableBuildingDataFromEntity.TryGetComponent(prefabRef2.m_Prefab, ref spawnableBuildingData))
				{
					continue;
				}
				float efficiency = BuildingUtils.GetEfficiency(propertyRenter.m_Property, ref m_BuildingEfficiencyFromEntity);
				float resourcesInArea = ExtractorAISystem.GetResourcesInArea(attached.m_Parent, ref m_SubAreaBufs, ref m_InstalledUpgrades, ref m_ExtractorsFromEntity);
				int maxWorkers = workProvider.m_MaxWorkers;
				int level = spawnableBuildingData.m_Level;
				int dailyProduction = Mathf.FloorToInt(math.min(resourcesInArea, (float)EconomyUtils.GetCompanyProductionPerDay(efficiency, maxWorkers, level, isIndustrial: true, workplaceData, processData, m_ResourcePrefabs, ref m_ResourceDatas, ref m_EconomyParameters)));
				if (m_SubAreaBufs.TryGetBuffer(attached.m_Parent, ref subAreas))
				{
					ProcessAreas(subAreas, dailyProduction, ref totalFertility, ref totalForest, ref totalOil, ref totalOre, ref totalFish);
				}
				if (!m_InstalledUpgrades.TryGetBuffer(attached.m_Parent, ref val))
				{
					continue;
				}
				for (int j = 0; j < val.Length; j++)
				{
					if (!BuildingUtils.CheckOption(val[j], BuildingOption.Inactive) && m_SubAreaBufs.TryGetBuffer(val[j].m_Upgrade, ref subAreas))
					{
						ProcessAreas(subAreas, dailyProduction, ref totalFertility, ref totalForest, ref totalOil, ref totalOre, ref totalFish);
					}
				}
			}
			ref NativeArray<float> reference = ref m_Result;
			reference[5] = reference[5] + totalFertility;
			reference = ref m_Result;
			reference[6] = reference[6] + totalForest;
			reference = ref m_Result;
			reference[7] = reference[7] + totalOil;
			reference = ref m_Result;
			reference[8] = reference[8] + totalOre;
			reference = ref m_Result;
			reference[11] = reference[11] + totalFish;
		}

		private void ProcessAreas(DynamicBuffer<Game.Areas.SubArea> subAreas, int dailyProduction, ref float totalFertility, ref float totalForest, ref float totalOil, ref float totalOre, ref float totalFish)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			PrefabRef prefabRef = default(PrefabRef);
			ExtractorAreaData extractorAreaData = default(ExtractorAreaData);
			for (int i = 0; i < subAreas.Length; i++)
			{
				Game.Areas.SubArea subArea = subAreas[i];
				if (m_PrefabRefFromEntity.TryGetComponent(subArea.m_Area, ref prefabRef) && m_ExtractorAreaDataFromEntity.TryGetComponent(prefabRef.m_Prefab, ref extractorAreaData))
				{
					switch (extractorAreaData.m_MapFeature)
					{
					case MapFeature.FertileLand:
						num4 += (float)dailyProduction;
						break;
					case MapFeature.Forest:
						num3 += (float)dailyProduction;
						break;
					case MapFeature.Oil:
						num += (float)dailyProduction;
						break;
					case MapFeature.Ore:
						num2 += (float)dailyProduction;
						break;
					case MapFeature.Fish:
						num5 += (float)dailyProduction;
						break;
					}
				}
			}
			totalFertility += num4;
			totalForest += num3;
			totalOil += num;
			totalOre += num2;
			totalFish += num5;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public BufferTypeHandle<MapFeatureElement> __Game_Areas_MapFeatureElement_RO_BufferTypeHandle;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WorkProvider> __Game_Companies_WorkProvider_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Attached> __Game_Objects_Attached_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Extractor> __Game_Areas_Extractor_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ExtractorAreaData> __Game_Prefabs_ExtractorAreaData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WorkplaceData> __Game_Prefabs_WorkplaceData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> __Game_Prefabs_IndustrialProcessData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Efficiency> __Game_Buildings_Efficiency_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> __Game_Areas_SubArea_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<ResourceData> __Game_Prefabs_ResourceData_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
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
			__Game_Areas_MapFeatureElement_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<MapFeatureElement>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Buildings_PropertyRenter_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PropertyRenter>(true);
			__Game_Companies_WorkProvider_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WorkProvider>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Objects_Attached_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attached>(true);
			__Game_Areas_Extractor_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Extractor>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ExtractorAreaData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ExtractorAreaData>(true);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableBuildingData>(true);
			__Game_Prefabs_WorkplaceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WorkplaceData>(true);
			__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<IndustrialProcessData>(true);
			__Game_Buildings_Efficiency_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Efficiency>(true);
			__Game_Areas_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.SubArea>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<InstalledUpgrade>(true);
			__Game_Prefabs_ResourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceData>(true);
		}
	}

	private const string kGroup = "naturalResourceInfo";

	private ResourceSystem m_ResourceSystem;

	private ValueBinding<float> m_AvailableOil;

	private ValueBinding<float> m_AvailableOre;

	private ValueBinding<float> m_AvailableForest;

	private ValueBinding<float> m_AvailableFertility;

	private ValueBinding<float> m_ForestRenewalRate;

	private ValueBinding<float> m_FertilityRenewalRate;

	private ValueBinding<float> m_FishRenewalRate;

	private ValueBinding<float> m_AvailableFish;

	private ValueBinding<float> m_OilExtractionRate;

	private ValueBinding<float> m_OreExtractionRate;

	private ValueBinding<float> m_ForestExtractionRate;

	private ValueBinding<float> m_FertilityExtractionRate;

	private ValueBinding<float> m_FishExtractionRate;

	private EntityQuery m_MapTileQuery;

	private EntityQuery m_ExtractorQuery;

	private NativeArray<float> m_Results;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_1701516005_0;

	public override GameMode gameMode => GameMode.GameOrEditor;

	protected override bool Active
	{
		get
		{
			if (!base.Active && !((EventBindingBase)m_AvailableFertility).active && !((EventBindingBase)m_AvailableForest).active && !((EventBindingBase)m_AvailableOil).active && !((EventBindingBase)m_AvailableOre).active && !((EventBindingBase)m_FertilityExtractionRate).active && !((EventBindingBase)m_ForestExtractionRate).active && !((EventBindingBase)m_OilExtractionRate).active)
			{
				return ((EventBindingBase)m_OreExtractionRate).active;
			}
			return true;
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		AddBinding((IBinding)(object)(m_AvailableOil = new ValueBinding<float>("naturalResourceInfo", "availableOil", 0f, (IWriter<float>)null, (EqualityComparer<float>)null)));
		AddBinding((IBinding)(object)(m_AvailableOre = new ValueBinding<float>("naturalResourceInfo", "availableOre", 0f, (IWriter<float>)null, (EqualityComparer<float>)null)));
		AddBinding((IBinding)(object)(m_AvailableForest = new ValueBinding<float>("naturalResourceInfo", "availableForest", 0f, (IWriter<float>)null, (EqualityComparer<float>)null)));
		AddBinding((IBinding)(object)(m_AvailableFertility = new ValueBinding<float>("naturalResourceInfo", "availableFertility", 0f, (IWriter<float>)null, (EqualityComparer<float>)null)));
		AddBinding((IBinding)(object)(m_ForestRenewalRate = new ValueBinding<float>("naturalResourceInfo", "forestRenewalRate", 0f, (IWriter<float>)null, (EqualityComparer<float>)null)));
		AddBinding((IBinding)(object)(m_FertilityRenewalRate = new ValueBinding<float>("naturalResourceInfo", "fertilityRenewalRate", 0f, (IWriter<float>)null, (EqualityComparer<float>)null)));
		AddBinding((IBinding)(object)(m_FishRenewalRate = new ValueBinding<float>("naturalResourceInfo", "fishRenewalRate", 0f, (IWriter<float>)null, (EqualityComparer<float>)null)));
		AddBinding((IBinding)(object)(m_AvailableFish = new ValueBinding<float>("naturalResourceInfo", "availableFish", 0f, (IWriter<float>)null, (EqualityComparer<float>)null)));
		AddBinding((IBinding)(object)(m_OilExtractionRate = new ValueBinding<float>("naturalResourceInfo", "oilExtractionRate", 0f, (IWriter<float>)null, (EqualityComparer<float>)null)));
		AddBinding((IBinding)(object)(m_OreExtractionRate = new ValueBinding<float>("naturalResourceInfo", "oreExtractionRate", 0f, (IWriter<float>)null, (EqualityComparer<float>)null)));
		AddBinding((IBinding)(object)(m_ForestExtractionRate = new ValueBinding<float>("naturalResourceInfo", "forestExtractionRate", 0f, (IWriter<float>)null, (EqualityComparer<float>)null)));
		AddBinding((IBinding)(object)(m_FertilityExtractionRate = new ValueBinding<float>("naturalResourceInfo", "fertilityExtractionRate", 0f, (IWriter<float>)null, (EqualityComparer<float>)null)));
		AddBinding((IBinding)(object)(m_FishExtractionRate = new ValueBinding<float>("naturalResourceInfo", "fishExtractionRate", 0f, (IWriter<float>)null, (EqualityComparer<float>)null)));
		m_MapTileQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<MapTile>(),
			ComponentType.ReadOnly<MapFeatureElement>(),
			ComponentType.Exclude<Native>()
		});
		m_ExtractorQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<Game.Companies.ExtractorCompany>(),
			ComponentType.ReadOnly<PropertyRenter>(),
			ComponentType.ReadOnly<WorkProvider>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_Results = new NativeArray<float>(13, (Allocator)4, (NativeArrayOptions)1);
		((ComponentSystemBase)this).RequireForUpdate<ExtractorParameterData>();
		((ComponentSystemBase)this).RequireForUpdate<EconomyParameterData>();
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_Results.Dispose();
		base.OnDestroy();
	}

	protected override void PerformUpdate()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		ResetResults<float>(m_Results);
		JobHandle val = JobChunkExtensions.Schedule<UpdateResourcesJob>(new UpdateResourcesJob
		{
			m_MapFeatureElementHandle = InternalCompilerInterface.GetBufferTypeHandle<MapFeatureElement>(ref __TypeHandle.__Game_Areas_MapFeatureElement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Results = m_Results
		}, m_MapTileQuery, ((SystemBase)this).Dependency);
		((JobHandle)(ref val)).Complete();
		val = JobChunkExtensions.Schedule<UpdateExtractionJob>(new UpdateExtractionJob
		{
			m_EntityHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyRenterHandle = InternalCompilerInterface.GetComponentTypeHandle<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WorkProviderHandle = InternalCompilerInterface.GetComponentTypeHandle<WorkProvider>(ref __TypeHandle.__Game_Companies_WorkProvider_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefHandle = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AttachedFromEntity = InternalCompilerInterface.GetComponentLookup<Attached>(ref __TypeHandle.__Game_Objects_Attached_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ExtractorsFromEntity = InternalCompilerInterface.GetComponentLookup<Extractor>(ref __TypeHandle.__Game_Areas_Extractor_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefFromEntity = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ExtractorAreaDataFromEntity = InternalCompilerInterface.GetComponentLookup<ExtractorAreaData>(ref __TypeHandle.__Game_Prefabs_ExtractorAreaData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnableBuildingDataFromEntity = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WorkplaceDataFromEntity = InternalCompilerInterface.GetComponentLookup<WorkplaceData>(ref __TypeHandle.__Game_Prefabs_WorkplaceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_IndustrialProcessDataFromEntity = InternalCompilerInterface.GetComponentLookup<IndustrialProcessData>(ref __TypeHandle.__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingEfficiencyFromEntity = InternalCompilerInterface.GetBufferLookup<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubAreaBufs = InternalCompilerInterface.GetBufferLookup<Game.Areas.SubArea>(ref __TypeHandle.__Game_Areas_SubArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgrades = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourcePrefabs = m_ResourceSystem.GetPrefabs(),
			m_ResourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EconomyParameters = ((EntityQuery)(ref __query_1701516005_0)).GetSingleton<EconomyParameterData>(),
			m_Result = m_Results
		}, m_ExtractorQuery, ((SystemBase)this).Dependency);
		((JobHandle)(ref val)).Complete();
		m_FertilityExtractionRate.Update(m_Results[5]);
		m_ForestExtractionRate.Update(m_Results[6]);
		m_OreExtractionRate.Update(m_Results[8]);
		m_OilExtractionRate.Update(m_Results[7]);
		m_FishExtractionRate.Update(m_Results[11]);
		m_AvailableFertility.Update(m_Results[0]);
		m_AvailableForest.Update(m_Results[1]);
		m_AvailableOre.Update(m_Results[3]);
		m_AvailableOil.Update(m_Results[2]);
		m_AvailableFish.Update(m_Results[4]);
		m_ForestRenewalRate.Update(m_Results[10]);
		m_FertilityRenewalRate.Update(m_Results[9]);
		m_FishRenewalRate.Update(m_Results[12]);
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
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<EconomyParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_1701516005_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
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
	public NaturalResourcesInfoviewUISystem()
	{
	}
}
