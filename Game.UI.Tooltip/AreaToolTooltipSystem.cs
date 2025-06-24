using System.Runtime.CompilerServices;
using Colossal.Entities;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Game.UI.Localization;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.UI.Tooltip;

[CompilerGenerated]
public class AreaToolTooltipSystem : TooltipSystemBase
{
	private struct TypeHandle
	{
		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> __Game_Areas_SubArea_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Game.Areas.Lot> __Game_Areas_Lot_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Geometry> __Game_Areas_Geometry_RO_ComponentLookup;

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
			__Game_Areas_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.SubArea>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<InstalledUpgrade>(true);
			__Game_Areas_Lot_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Areas.Lot>(true);
			__Game_Areas_Geometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Geometry>(true);
		}
	}

	private ToolSystem m_ToolSystem;

	private AreaToolSystem m_AreaTool;

	private ResourceSystem m_ResourceSystem;

	private EntityQuery m_TempQuery;

	private IntTooltip m_Resources;

	private IntTooltip m_AreaSizeToolTip;

	private IntTooltip m_Storage;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_AreaTool = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AreaToolSystem>();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Area>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Deleted>() };
		array[0] = val;
		m_TempQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		((ComponentSystemBase)this).RequireForUpdate(m_TempQuery);
		m_AreaSizeToolTip = new IntTooltip
		{
			path = "areaToolAreaSize",
			label = LocalizedString.Id("Tools.AREASIZE_LABEL"),
			unit = "area"
		};
		m_Resources = new IntTooltip
		{
			path = "areaToolResources",
			label = LocalizedString.Id("Tools.RESOURCES_LABEL"),
			unit = "weight"
		};
		m_Storage = new IntTooltip
		{
			path = "areaToolStorage",
			label = LocalizedString.Id("Tools.STORAGECAPACITY_LABEL"),
			unit = "weight"
		};
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		if (m_ToolSystem.activeTool != m_AreaTool || m_AreaTool.tooltip == AreaToolSystem.Tooltip.None || !ShouldShowResources(m_AreaTool.tooltip))
		{
			return;
		}
		NativeArray<Entity> val = ((EntityQuery)(ref m_TempQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			Extractor extractor = default(Extractor);
			Owner owner = default(Owner);
			Owner owner2 = default(Owner);
			PrefabRef prefabRef = default(PrefabRef);
			BuildingPropertyData buildingPropertyData = default(BuildingPropertyData);
			ResourceData resourceData = default(ResourceData);
			ExtractorAreaData extractorAreaData = default(ExtractorAreaData);
			Owner owner3 = default(Owner);
			PrefabRef prefabRef2 = default(PrefabRef);
			GarbageFacilityData data = default(GarbageFacilityData);
			DynamicBuffer<InstalledUpgrade> upgrades = default(DynamicBuffer<InstalledUpgrade>);
			for (int i = 0; i < val.Length; i++)
			{
				Entity val2 = val[i];
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				PrefabRef componentData = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(val2);
				if (EntitiesExtensions.TryGetComponent<Extractor>(((ComponentSystemBase)this).EntityManager, val2, ref extractor))
				{
					if (((EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, val2, ref owner) && EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, owner.m_Owner, ref owner2)) || EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, val2, ref owner2)) && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, owner2.m_Owner, ref prefabRef) && EntitiesExtensions.TryGetComponent<BuildingPropertyData>(((ComponentSystemBase)this).EntityManager, prefabRef.m_Prefab, ref buildingPropertyData) && EntitiesExtensions.TryGetComponent<ResourceData>(((ComponentSystemBase)this).EntityManager, m_ResourceSystem.GetPrefab(buildingPropertyData.m_AllowedManufactured), ref resourceData) && resourceData.m_RequireNaturalResource && EntitiesExtensions.TryGetComponent<ExtractorAreaData>(((ComponentSystemBase)this).EntityManager, componentData.m_Prefab, ref extractorAreaData) && extractorAreaData.m_RequireNaturalResource)
					{
						num2 += (int)math.round(extractor.m_ResourceAmount);
						flag = true;
					}
					else
					{
						BufferLookup<Game.Areas.SubArea> subAreas = InternalCompilerInterface.GetBufferLookup<Game.Areas.SubArea>(ref __TypeHandle.__Game_Areas_SubArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
						BufferLookup<InstalledUpgrade> installedUpgrades = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
						ComponentLookup<Game.Areas.Lot> lots = InternalCompilerInterface.GetComponentLookup<Game.Areas.Lot>(ref __TypeHandle.__Game_Areas_Lot_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
						ComponentLookup<Geometry> geometries = InternalCompilerInterface.GetComponentLookup<Geometry>(ref __TypeHandle.__Game_Areas_Geometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
						float area = ExtractorAISystem.GetArea(val2, ref subAreas, ref installedUpgrades, ref lots, ref geometries);
						num += (int)math.round(area);
						flag3 = true;
					}
				}
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (!((EntityManager)(ref entityManager)).HasComponent<Storage>(val2))
				{
					continue;
				}
				entityManager = ((ComponentSystemBase)this).EntityManager;
				Geometry componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<Geometry>(val2);
				entityManager = ((ComponentSystemBase)this).EntityManager;
				StorageAreaData componentData3 = ((EntityManager)(ref entityManager)).GetComponentData<StorageAreaData>(componentData.m_Prefab);
				int num4 = AreaUtils.CalculateStorageCapacity(componentData2, componentData3);
				if (EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, val2, ref owner3) && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, owner3.m_Owner, ref prefabRef2) && EntitiesExtensions.TryGetComponent<GarbageFacilityData>(((ComponentSystemBase)this).EntityManager, prefabRef2.m_Prefab, ref data))
				{
					if (EntitiesExtensions.TryGetBuffer<InstalledUpgrade>(((ComponentSystemBase)this).EntityManager, owner3.m_Owner, true, ref upgrades))
					{
						UpgradeUtils.CombineStats<GarbageFacilityData>(((ComponentSystemBase)this).EntityManager, ref data, upgrades);
					}
					num4 += data.m_GarbageCapacity;
				}
				num3 += num4;
				flag2 = true;
			}
			if (flag)
			{
				m_Resources.value = num2;
				AddMouseTooltip(m_Resources);
			}
			if (flag3)
			{
				m_AreaSizeToolTip.value = num;
				AddMouseTooltip(m_AreaSizeToolTip);
			}
			if (flag2)
			{
				m_Storage.value = num3;
				AddMouseTooltip(m_Storage);
			}
		}
		finally
		{
			val.Dispose();
		}
	}

	private static bool ShouldShowResources(AreaToolSystem.Tooltip tooltip)
	{
		if ((uint)(tooltip - 6) <= 4u)
		{
			return true;
		}
		return false;
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
	public AreaToolTooltipSystem()
	{
	}
}
