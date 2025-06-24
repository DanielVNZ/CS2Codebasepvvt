using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Economy;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Game.UI.Localization;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.UI.Tooltip;

[CompilerGenerated]
public class TempExtractorTooltipSystem : TooltipSystemBase
{
	private struct TreeIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
	{
		public Bounds2 m_Bounds;

		public Circle2 m_Circle;

		public ComponentLookup<Overridden> m_OverriddenData;

		public ComponentLookup<Transform> m_TransformData;

		public ComponentLookup<PrefabRef> m_PrefabRefData;

		public ComponentLookup<TreeData> m_PrefabTreeData;

		public float m_Result;

		public bool Intersect(QuadTreeBoundsXZ bounds)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds);
		}

		public void Iterate(QuadTreeBoundsXZ bounds, Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			if (m_OverriddenData.HasComponent(entity))
			{
				return;
			}
			Transform transform = m_TransformData[entity];
			if (!MathUtils.Intersect(m_Circle, ((float3)(ref transform.m_Position)).xz))
			{
				return;
			}
			PrefabRef prefabRef = m_PrefabRefData[entity];
			if (m_PrefabTreeData.HasComponent(prefabRef.m_Prefab))
			{
				TreeData treeData = m_PrefabTreeData[prefabRef.m_Prefab];
				if (treeData.m_WoodAmount >= 1f)
				{
					m_Result += treeData.m_WoodAmount;
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<Overridden> __Game_Common_Overridden_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TreeData> __Game_Prefabs_TreeData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PlaceholderBuildingData> __Game_Prefabs_PlaceholderBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> __Game_Prefabs_BuildingPropertyData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ExtractorAreaData> __Game_Prefabs_ExtractorAreaData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LotData> __Game_Prefabs_LotData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ResourceData> __Game_Prefabs_ResourceData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> __Game_Areas_SubArea_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubAreaNode> __Game_Prefabs_SubAreaNode_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferLookup;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			__Game_Common_Overridden_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Overridden>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_TreeData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TreeData>(true);
			__Game_Prefabs_PlaceholderBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceholderBuildingData>(true);
			__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingPropertyData>(true);
			__Game_Prefabs_ExtractorAreaData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ExtractorAreaData>(true);
			__Game_Prefabs_LotData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LotData>(true);
			__Game_Prefabs_ResourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceData>(true);
			__Game_Areas_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.SubArea>(true);
			__Game_Prefabs_SubAreaNode_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubAreaNode>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<InstalledUpgrade>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
		}
	}

	private NaturalResourceSystem m_NaturalResourceSystem;

	private ResourceSystem m_ResourceSystem;

	private CitySystem m_CitySystem;

	private IndustrialDemandSystem m_IndustrialDemandSystem;

	private CountCompanyDataSystem m_CountCompanyDataSystem;

	private ClimateSystem m_ClimateSystem;

	private PrefabSystem m_PrefabSystem;

	private Game.Objects.SearchSystem m_SearchSystem;

	private EntityQuery m_ErrorQuery;

	private EntityQuery m_TempQuery;

	private EntityQuery m_ExtractorParameterQuery;

	private StringTooltip m_ResourceAvailable;

	private StringTooltip m_ResourceUnavailable;

	private IntTooltip m_Surplus;

	private IntTooltip m_Deficit;

	private StringTooltip m_ClimateAvailable;

	private StringTooltip m_ClimateUnavailable;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_NaturalResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NaturalResourceSystem>();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_IndustrialDemandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IndustrialDemandSystem>();
		m_CountCompanyDataSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CountCompanyDataSystem>();
		m_ClimateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ClimateSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_SearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.SearchSystem>();
		m_ErrorQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Error>()
		});
		m_TempQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[7]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Transform>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<Placeholder>(),
			ComponentType.Exclude<Hidden>(),
			ComponentType.Exclude<Deleted>()
		});
		m_ExtractorParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ExtractorParameterData>() });
		m_ResourceAvailable = new StringTooltip
		{
			path = "extractorMapFeatureAvailable"
		};
		m_ResourceUnavailable = new StringTooltip
		{
			path = "extractorMapFeatureUnavailable",
			color = TooltipColor.Warning
		};
		m_ClimateAvailable = new StringTooltip
		{
			path = "extractorClimateAvailable",
			value = LocalizedString.Id("Tools.EXTRACTOR_CLIMATE_REQUIRED_AVAILABLE")
		};
		m_ClimateUnavailable = new StringTooltip
		{
			path = "extractorClimateUnavailable",
			value = LocalizedString.Id("Tools.EXTRACTOR_CLIMATE_REQUIRED_UNAVAILABLE"),
			color = TooltipColor.Warning
		};
		m_Surplus = new IntTooltip
		{
			path = "extractorCityProductionSurplus",
			label = LocalizedString.Id("Tools.EXTRACTOR_PRODUCTION_SURPLUS"),
			unit = "weightPerMonth"
		};
		m_Deficit = new IntTooltip
		{
			path = "extractorCityProductionDeficit",
			label = LocalizedString.Id("Tools.EXTRACTOR_PRODUCTION_DEFICIT"),
			unit = "weightPerMonth"
		};
		((ComponentSystemBase)this).RequireForUpdate(m_TempQuery);
	}

	private bool FindWoodResource(Circle2 circle)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		TreeIterator treeIterator = new TreeIterator
		{
			m_OverriddenData = InternalCompilerInterface.GetComponentLookup<Overridden>(ref __TypeHandle.__Game_Common_Overridden_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTreeData = InternalCompilerInterface.GetComponentLookup<TreeData>(ref __TypeHandle.__Game_Prefabs_TreeData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Bounds = new Bounds2(circle.position - circle.radius, circle.position + circle.radius),
			m_Circle = circle,
			m_Result = 0f
		};
		JobHandle dependencies;
		NativeQuadTree<Entity, QuadTreeBoundsXZ> staticSearchTree = m_SearchSystem.GetStaticSearchTree(readOnly: true, out dependencies);
		((JobHandle)(ref dependencies)).Complete();
		staticSearchTree.Iterate<TreeIterator>(ref treeIterator, 0);
		return treeIterator.m_Result > 0f;
	}

	private bool FindResource(Circle2 circle, MapFeature requiredFeature, CellMapData<NaturalResourceCell> resourceMap, DynamicBuffer<CityModifier> cityModifiers)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		int2 cell = CellMapSystem<NaturalResourceCell>.GetCell(new float3(circle.position.x - circle.radius, 0f, circle.position.y - circle.radius), CellMapSystem<NaturalResourceCell>.kMapSize, resourceMap.m_TextureSize.x);
		int2 cell2 = CellMapSystem<NaturalResourceCell>.GetCell(new float3(circle.position.x + circle.radius, 0f, circle.position.y + circle.radius), CellMapSystem<NaturalResourceCell>.kMapSize, resourceMap.m_TextureSize.x);
		cell = math.max(new int2(0, 0), cell);
		cell2 = math.min(new int2(resourceMap.m_TextureSize.x - 1, resourceMap.m_TextureSize.y - 1), cell2);
		int2 val = default(int2);
		val.x = cell.x;
		while (val.x <= cell2.x)
		{
			val.y = cell.y;
			while (val.y <= cell2.y)
			{
				float3 cellCenter = CellMapSystem<NaturalResourceCell>.GetCellCenter(val, resourceMap.m_TextureSize.x);
				if (MathUtils.Intersect(circle, ((float3)(ref cellCenter)).xz))
				{
					NaturalResourceCell naturalResourceCell = resourceMap.m_Buffer[val.x + val.y * resourceMap.m_TextureSize.x];
					float num = 0f;
					switch (requiredFeature)
					{
					case MapFeature.FertileLand:
						num = (int)naturalResourceCell.m_Fertility.m_Base;
						num -= (float)(int)naturalResourceCell.m_Fertility.m_Used;
						break;
					case MapFeature.Ore:
						num = (int)naturalResourceCell.m_Ore.m_Base;
						if (cityModifiers.IsCreated)
						{
							CityUtils.ApplyModifier(ref num, cityModifiers, CityModifierType.OreResourceAmount);
						}
						num -= (float)(int)naturalResourceCell.m_Ore.m_Used;
						break;
					case MapFeature.Oil:
						num = (int)naturalResourceCell.m_Oil.m_Base;
						if (cityModifiers.IsCreated)
						{
							CityUtils.ApplyModifier(ref num, cityModifiers, CityModifierType.OilResourceAmount);
						}
						num -= (float)(int)naturalResourceCell.m_Oil.m_Used;
						break;
					case MapFeature.Fish:
						num = (int)naturalResourceCell.m_Fish.m_Base;
						num -= (float)(int)naturalResourceCell.m_Fish.m_Used;
						break;
					default:
						num = 0f;
						break;
					}
					if (num > 0f)
					{
						return true;
					}
				}
				val.y++;
			}
			val.x++;
		}
		return false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<CityModifier> cityModifiers = default(DynamicBuffer<CityModifier>);
		if (!((EntityQuery)(ref m_ErrorQuery)).IsEmptyIgnoreFilter || !EntitiesExtensions.TryGetBuffer<CityModifier>(((ComponentSystemBase)this).EntityManager, m_CitySystem.City, true, ref cityModifiers))
		{
			return;
		}
		((SystemBase)this).CompleteDependency();
		JobHandle dependencies;
		CellMapData<NaturalResourceCell> data = m_NaturalResourceSystem.GetData(readOnly: true, out dependencies);
		((JobHandle)(ref dependencies)).Complete();
		ComponentLookup<PlaceholderBuildingData> componentLookup = InternalCompilerInterface.GetComponentLookup<PlaceholderBuildingData>(ref __TypeHandle.__Game_Prefabs_PlaceholderBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<BuildingPropertyData> componentLookup2 = InternalCompilerInterface.GetComponentLookup<BuildingPropertyData>(ref __TypeHandle.__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<ExtractorAreaData> componentLookup3 = InternalCompilerInterface.GetComponentLookup<ExtractorAreaData>(ref __TypeHandle.__Game_Prefabs_ExtractorAreaData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<LotData> componentLookup4 = InternalCompilerInterface.GetComponentLookup<LotData>(ref __TypeHandle.__Game_Prefabs_LotData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<ResourceData> componentLookup5 = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<Transform> componentLookup6 = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<PrefabRef> componentLookup7 = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		BufferLookup<Game.Areas.SubArea> bufferLookup = InternalCompilerInterface.GetBufferLookup<Game.Areas.SubArea>(ref __TypeHandle.__Game_Areas_SubArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		BufferLookup<SubAreaNode> bufferLookup2 = InternalCompilerInterface.GetBufferLookup<SubAreaNode>(ref __TypeHandle.__Game_Prefabs_SubAreaNode_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		BufferLookup<InstalledUpgrade> bufferLookup3 = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		ResourcePrefabs prefabs = m_ResourceSystem.GetPrefabs();
		NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_TempQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		MapFeature requiredFeature = MapFeature.None;
		bool foundResource = false;
		bool flag = false;
		Resource resource = Resource.NoResource;
		try
		{
			ComponentTypeHandle<Temp> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			((JobHandle)(ref dependencies)).Complete();
			Enumerator<ArchetypeChunk> enumerator = val.GetEnumerator();
			try
			{
				DynamicBuffer<Game.Areas.SubArea> subAreas = default(DynamicBuffer<Game.Areas.SubArea>);
				DynamicBuffer<InstalledUpgrade> val4 = default(DynamicBuffer<InstalledUpgrade>);
				DynamicBuffer<SubAreaNode> subAreaNodeBuf = default(DynamicBuffer<SubAreaNode>);
				while (enumerator.MoveNext())
				{
					ArchetypeChunk current = enumerator.Current;
					NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref current)).GetNativeArray(InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef));
					NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref current)).GetNativeArray<Temp>(ref componentTypeHandle);
					for (int i = 0; i < nativeArray.Length; i++)
					{
						Entity val2 = nativeArray[i];
						if ((nativeArray2[i].m_Flags & (TempFlags.Create | TempFlags.Upgrade)) != 0)
						{
							Entity val3 = componentLookup7[val2];
							if (componentLookup.HasComponent(val3) && componentLookup2.HasComponent(val3) && componentLookup[val3].m_Type == BuildingType.ExtractorBuilding)
							{
								resource = componentLookup2[val3].m_AllowedManufactured;
								if (bufferLookup.TryGetBuffer(val2, ref subAreas) && ProcessAreas(subAreas, componentLookup3, componentLookup4, resource, prefabs, componentLookup5, componentLookup6[val2], data, cityModifiers, ref requiredFeature, ref foundResource))
								{
									flag = true;
								}
								else if (bufferLookup3.TryGetBuffer(val2, ref val4))
								{
									for (int j = 0; j < val4.Length; j++)
									{
										if (bufferLookup2.TryGetBuffer((Entity)componentLookup7[val4[j].m_Upgrade], ref subAreaNodeBuf) && bufferLookup.TryGetBuffer(val4[j].m_Upgrade, ref subAreas) && ProcessAreaNodes(subAreas, subAreaNodeBuf, componentLookup3, componentLookup4, resource, prefabs, componentLookup5, componentLookup6[val4[j].m_Upgrade], data, cityModifiers, ref requiredFeature, ref foundResource))
										{
											flag = true;
										}
										else if (bufferLookup.TryGetBuffer(val4[j].m_Upgrade, ref subAreas) && ProcessAreas(subAreas, componentLookup3, componentLookup4, resource, prefabs, componentLookup5, componentLookup6[val4[j].m_Upgrade], data, cityModifiers, ref requiredFeature, ref foundResource))
										{
											flag = true;
										}
										if (flag)
										{
											break;
										}
									}
								}
							}
						}
						if (flag)
						{
							break;
						}
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
		}
		finally
		{
			val.Dispose();
		}
		if (!flag)
		{
			return;
		}
		JobHandle deps;
		NativeArray<int> production = m_CountCompanyDataSystem.GetProduction(out deps);
		JobHandle deps2;
		NativeArray<int> consumption = m_IndustrialDemandSystem.GetConsumption(out deps2);
		int resourceIndex = EconomyUtils.GetResourceIndex(resource);
		((JobHandle)(ref deps)).Complete();
		((JobHandle)(ref deps2)).Complete();
		int num = production[resourceIndex] - consumption[resourceIndex];
		Entity val5 = prefabs[resource];
		ResourceData resourceData = componentLookup5[val5];
		string icon = ImageSystem.GetIcon(m_PrefabSystem.GetPrefab<PrefabBase>(val5));
		if (num > 0)
		{
			m_Surplus.value = num;
			m_Surplus.icon = icon;
			AddMouseTooltip(m_Surplus);
		}
		else
		{
			m_Deficit.value = -num;
			m_Deficit.icon = icon;
			AddMouseTooltip(m_Deficit);
		}
		bool flag2 = ShouldMapFeatureUseResourceIcon(resource);
		if (requiredFeature != MapFeature.None)
		{
			string mapFeatureIconName = AreaTools.GetMapFeatureIconName(requiredFeature);
			if (foundResource)
			{
				m_ResourceAvailable.icon = (flag2 ? icon : ("Media/Game/Icons/" + mapFeatureIconName + ".svg"));
				m_ResourceAvailable.value = LocalizedString.Id("Tools.EXTRACTOR_MAP_FEATURE_REQUIRED_AVAILABLE");
				AddMouseTooltip(m_ResourceAvailable);
			}
			else
			{
				m_ResourceUnavailable.icon = (flag2 ? icon : ("Media/Game/Icons/" + mapFeatureIconName + ".svg"));
				m_ResourceUnavailable.value = LocalizedString.Id("Tools.EXTRACTOR_MAP_FEATURE_REQUIRED_MISSING");
				AddMouseTooltip(m_ResourceUnavailable);
			}
		}
		if (resourceData.m_RequireTemperature)
		{
			if (m_ClimateSystem.averageTemperature >= resourceData.m_RequiredTemperature)
			{
				AddMouseTooltip(m_ClimateAvailable);
			}
			else
			{
				AddMouseTooltip(m_ClimateUnavailable);
			}
		}
	}

	private bool ShouldMapFeatureUseResourceIcon(Resource resource)
	{
		if (resource == Resource.Fish)
		{
			return true;
		}
		return false;
	}

	private bool ProcessAreaNodes(DynamicBuffer<Game.Areas.SubArea> subAreas, DynamicBuffer<SubAreaNode> subAreaNodeBuf, ComponentLookup<ExtractorAreaData> extractorAreaDatas, ComponentLookup<LotData> lotDatas, Resource extractedResource, ResourcePrefabs resourcePrefabs, ComponentLookup<ResourceData> resourceDatas, Transform transform, CellMapData<NaturalResourceCell> resourceMap, DynamicBuffer<CityModifier> cityModifiers, ref MapFeature requiredFeature, ref bool foundResource)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		float num = 0f;
		for (int i = 0; i < subAreaNodeBuf.Length; i++)
		{
			float num2 = ((math.abs(subAreaNodeBuf[i].m_Position.x) > math.abs(subAreaNodeBuf[i].m_Position.z)) ? math.abs(subAreaNodeBuf[i].m_Position.x) : math.abs(subAreaNodeBuf[i].m_Position.z));
			if (num2 > num)
			{
				num = num2;
			}
		}
		PrefabRef prefabRef = default(PrefabRef);
		Circle2 circle = default(Circle2);
		for (int j = 0; j < subAreas.Length; j++)
		{
			if (!EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, subAreas[j].m_Area, ref prefabRef))
			{
				continue;
			}
			Entity prefab = prefabRef.m_Prefab;
			if (!extractorAreaDatas.HasComponent(prefab) || !lotDatas.HasComponent(prefab))
			{
				continue;
			}
			result = true;
			requiredFeature = ExtractorCompanySystem.GetRequiredMapFeature(extractedResource, prefab, resourcePrefabs, resourceDatas, extractorAreaDatas);
			if (requiredFeature != MapFeature.None)
			{
				float3 position = transform.m_Position;
				((Circle2)(ref circle))._002Ector(num, ((float3)(ref position)).xz);
				if (requiredFeature == MapFeature.Forest)
				{
					foundResource = FindWoodResource(circle);
				}
				else
				{
					foundResource = FindResource(circle, requiredFeature, resourceMap, cityModifiers);
				}
			}
		}
		return result;
	}

	private bool ProcessAreas(DynamicBuffer<Game.Areas.SubArea> subAreas, ComponentLookup<ExtractorAreaData> extractorAreaDatas, ComponentLookup<LotData> lotDatas, Resource extractedResource, ResourcePrefabs resourcePrefabs, ComponentLookup<ResourceData> resourceDatas, Transform transform, CellMapData<NaturalResourceCell> resourceMap, DynamicBuffer<CityModifier> cityModifiers, ref MapFeature requiredFeature, ref bool foundResource)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		PrefabRef prefabRef = default(PrefabRef);
		Circle2 circle = default(Circle2);
		for (int i = 0; i < subAreas.Length; i++)
		{
			if (!EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, subAreas[i].m_Area, ref prefabRef))
			{
				continue;
			}
			Entity prefab = prefabRef.m_Prefab;
			if (!extractorAreaDatas.HasComponent(prefab) || !lotDatas.HasComponent(prefab))
			{
				continue;
			}
			result = true;
			float maxRadius = lotDatas[prefab].m_MaxRadius;
			requiredFeature = ExtractorCompanySystem.GetRequiredMapFeature(extractedResource, prefab, resourcePrefabs, resourceDatas, extractorAreaDatas);
			if (requiredFeature != MapFeature.None)
			{
				float3 position = transform.m_Position;
				((Circle2)(ref circle))._002Ector(maxRadius, ((float3)(ref position)).xz);
				if (requiredFeature == MapFeature.Forest)
				{
					foundResource = FindWoodResource(circle);
				}
				else
				{
					foundResource = FindResource(circle, requiredFeature, resourceMap, cityModifiers);
				}
			}
		}
		return result;
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
	public TempExtractorTooltipSystem()
	{
	}
}
