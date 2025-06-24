using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Game.Buildings;
using Game.Common;
using Game.Policies;
using Game.Prefabs;
using Game.Rendering;
using Game.Simulation;
using Game.Tools;
using Game.UI.Localization;
using Game.Zones;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.Tooltip;

[CompilerGenerated]
public class CityPolicyTooltipSystem : TooltipSystemBase
{
	private PrefabSystem m_PrefabSystem;

	private ToolSystem m_ToolSystem;

	private DefaultToolSystem m_DefaultTool;

	private RaycastSystem m_RaycastSystem;

	private CameraUpdateSystem m_CameraUpdateSystem;

	private CitySystem m_CitySystem;

	private EntityQuery m_ActiveInfomodeQuery;

	private Entity m_AdvancedPollutionManagementPolicy;

	private StringTooltip m_PollutionManagement;

	private RaycastResult m_RaycastResult;

	private RaycastResult raycastResult
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			if (m_RaycastResult.m_Owner == Entity.Null && m_CameraUpdateSystem.TryGetViewer(out var viewer))
			{
				RaycastInput input = new RaycastInput
				{
					m_Line = ToolRaycastSystem.CalculateRaycastLine(viewer.camera),
					m_TypeMask = TypeMask.StaticObjects,
					m_CollisionMask = (CollisionMask.OnGround | CollisionMask.Overground)
				};
				m_RaycastSystem.AddInput(this, input);
				NativeArray<RaycastResult> result = m_RaycastSystem.GetResult(this);
				if (result.Length != 0)
				{
					m_RaycastResult = result[0];
				}
			}
			return m_RaycastResult;
		}
		set
		{
			m_RaycastResult = value;
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Expected O, but got Unknown
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_DefaultTool = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DefaultToolSystem>();
		m_RaycastSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RaycastSystem>();
		m_CameraUpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CameraUpdateSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<InfomodeData>(),
			ComponentType.ReadOnly<InfomodeActive>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<InfoviewHeatmapData>(),
			ComponentType.ReadOnly<InfoviewBuildingStatusData>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<InfomodeGroup>() };
		array[0] = val;
		m_ActiveInfomodeQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_PollutionManagement = new StringTooltip
		{
			path = "cityPolicyEffect",
			icon = "Media/Game/Policies/AdvancedPollutionManagement.svg"
		};
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PolicyData>(),
			ComponentType.ReadOnly<CityModifierData>()
		};
		array2[0] = val;
		EntityQuery entityQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		Enumerator<Entity> enumerator = ((EntityQuery)(ref entityQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Entity current = enumerator.Current;
				if (((Object)m_PrefabSystem.GetPrefab<PolicyPrefab>(current)).name == "Advanced Pollution Management")
				{
					m_AdvancedPollutionManagementPolicy = current;
					break;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		if (m_AdvancedPollutionManagementPolicy == Entity.Null)
		{
			throw new Exception("Advanced pollution management policy not found");
		}
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		if (m_ToolSystem.activeTool != m_DefaultTool || (Object)(object)m_ToolSystem.activeInfoview == (Object)null || ((EntityQuery)(ref m_ActiveInfomodeQuery)).IsEmptyIgnoreFilter)
		{
			return;
		}
		raycastResult = default(RaycastResult);
		Enumerator<Entity> enumerator = ((EntityQuery)(ref m_ActiveInfomodeQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2)).GetEnumerator();
		try
		{
			InfoviewHeatmapData infoviewHeatmapData = default(InfoviewHeatmapData);
			PrefabRef prefabRef2 = default(PrefabRef);
			SpawnableBuildingData spawnableBuildingData2 = default(SpawnableBuildingData);
			ZoneData zoneData2 = default(ZoneData);
			PollutionData pollutionData2 = default(PollutionData);
			PrefabRef prefabRef = default(PrefabRef);
			SpawnableBuildingData spawnableBuildingData = default(SpawnableBuildingData);
			ZoneData zoneData = default(ZoneData);
			PollutionData pollutionData = default(PollutionData);
			InfoviewBuildingStatusData infoviewBuildingStatusData = default(InfoviewBuildingStatusData);
			PrefabRef prefabRef3 = default(PrefabRef);
			SpawnableBuildingData spawnableBuildingData3 = default(SpawnableBuildingData);
			ZoneData zoneData3 = default(ZoneData);
			while (enumerator.MoveNext())
			{
				Entity current = enumerator.Current;
				EntityManager entityManager;
				if (EntitiesExtensions.TryGetComponent<InfoviewHeatmapData>(((ComponentSystemBase)this).EntityManager, current, ref infoviewHeatmapData))
				{
					switch (infoviewHeatmapData.m_Type)
					{
					case HeatmapData.GroundPollution:
						if (IsAdvancedPollutionManagementEnabled() && raycastResult.m_Owner != Entity.Null)
						{
							entityManager = ((ComponentSystemBase)this).EntityManager;
							if (((EntityManager)(ref entityManager)).HasComponent<Building>(raycastResult.m_Owner) && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, raycastResult.m_Owner, ref prefabRef2) && EntitiesExtensions.TryGetComponent<SpawnableBuildingData>(((ComponentSystemBase)this).EntityManager, prefabRef2.m_Prefab, ref spawnableBuildingData2) && EntitiesExtensions.TryGetComponent<ZoneData>(((ComponentSystemBase)this).EntityManager, spawnableBuildingData2.m_ZonePrefab, ref zoneData2) && zoneData2.m_AreaType == AreaType.Industrial && EntitiesExtensions.TryGetComponent<PollutionData>(((ComponentSystemBase)this).EntityManager, prefabRef2.m_Prefab, ref pollutionData2) && pollutionData2.m_GroundPollution > 0f)
							{
								m_PollutionManagement.value = LocalizedString.Id("DefaultTool.INFOMODE_TOOLTIP[PolicyEffectGroundPollution]");
								AddMouseTooltip(m_PollutionManagement);
							}
						}
						return;
					case HeatmapData.AirPollution:
						if (IsAdvancedPollutionManagementEnabled() && raycastResult.m_Owner != Entity.Null)
						{
							entityManager = ((ComponentSystemBase)this).EntityManager;
							if (((EntityManager)(ref entityManager)).HasComponent<Building>(raycastResult.m_Owner) && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, raycastResult.m_Owner, ref prefabRef) && EntitiesExtensions.TryGetComponent<SpawnableBuildingData>(((ComponentSystemBase)this).EntityManager, prefabRef.m_Prefab, ref spawnableBuildingData) && EntitiesExtensions.TryGetComponent<ZoneData>(((ComponentSystemBase)this).EntityManager, spawnableBuildingData.m_ZonePrefab, ref zoneData) && zoneData.m_AreaType == AreaType.Industrial && EntitiesExtensions.TryGetComponent<PollutionData>(((ComponentSystemBase)this).EntityManager, prefabRef.m_Prefab, ref pollutionData) && pollutionData.m_AirPollution > 0f)
							{
								m_PollutionManagement.value = LocalizedString.Id("DefaultTool.INFOMODE_TOOLTIP[PolicyEffectAirPolution]");
								AddMouseTooltip(m_PollutionManagement);
							}
						}
						return;
					}
				}
				else
				{
					if (!EntitiesExtensions.TryGetComponent<InfoviewBuildingStatusData>(((ComponentSystemBase)this).EntityManager, current, ref infoviewBuildingStatusData) || infoviewBuildingStatusData.m_Type != BuildingStatusType.GarbageAccumulation)
					{
						continue;
					}
					if (!IsAdvancedPollutionManagementEnabled() || !(raycastResult.m_Owner != Entity.Null))
					{
						break;
					}
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<Building>(raycastResult.m_Owner) && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, raycastResult.m_Owner, ref prefabRef3) && EntitiesExtensions.TryGetComponent<SpawnableBuildingData>(((ComponentSystemBase)this).EntityManager, prefabRef3.m_Prefab, ref spawnableBuildingData3) && EntitiesExtensions.TryGetComponent<ZoneData>(((ComponentSystemBase)this).EntityManager, spawnableBuildingData3.m_ZonePrefab, ref zoneData3) && zoneData3.m_AreaType == AreaType.Industrial)
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						if (((EntityManager)(ref entityManager)).HasComponent<GarbageProducer>(raycastResult.m_Owner))
						{
							m_PollutionManagement.value = LocalizedString.Id("DefaultTool.INFOMODE_TOOLTIP[PolicyEffectGarbage]");
							AddMouseTooltip(m_PollutionManagement);
						}
					}
					break;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
	}

	private bool IsAdvancedPollutionManagementEnabled()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Enumerator<Policy> enumerator = ((EntityManager)(ref entityManager)).GetBuffer<Policy>(m_CitySystem.City, true).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Policy current = enumerator.Current;
				if (!(current.m_Policy != m_AdvancedPollutionManagementPolicy))
				{
					return (current.m_Flags & PolicyFlags.Active) != 0;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		return false;
	}

	[Preserve]
	public CityPolicyTooltipSystem()
	{
	}
}
