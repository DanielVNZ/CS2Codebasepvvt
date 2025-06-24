using System.Runtime.CompilerServices;
using Colossal.Entities;
using Game.Buildings;
using Game.Citizens;
using Game.Prefabs;
using Game.Tools;
using Game.UI.Localization;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.Tooltip;

[CompilerGenerated]
public class HomelessTooltipSystem : TooltipSystemBase
{
	private IntTooltip m_HomelessCountTooltip;

	private ToolRaycastSystem m_ToolRaycastSystem;

	private PrefabSystem m_PrefabSystem;

	private EntityQuery m_ConfigQuery;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolRaycastSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolRaycastSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_ConfigQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<UIInfoviewsConfigurationData>() });
		m_HomelessCountTooltip = new IntTooltip
		{
			path = "HomelessCount",
			label = LocalizedString.Id("Infoviews.INFOVIEW[HomelessCount]"),
			unit = "integer"
		};
		((ComponentSystemBase)this).RequireForUpdate(m_ConfigQuery);
	}

	private bool IsInfomodeActivated()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		Entity singletonEntity = ((EntityQuery)(ref m_ConfigQuery)).GetSingletonEntity();
		if (m_PrefabSystem.TryGetPrefab<UIInfoviewsConfigurationPrefab>(singletonEntity, out var prefab))
		{
			Entity entity = m_PrefabSystem.GetEntity(prefab.m_HomelessInfomodePrefab);
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<InfomodeActive>(entity);
		}
		return false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		if (!IsInfomodeActivated())
		{
			return;
		}
		((SystemBase)this).CompleteDependency();
		m_HomelessCountTooltip.value = 0;
		DynamicBuffer<Renter> val = default(DynamicBuffer<Renter>);
		if (m_ToolRaycastSystem.GetRaycastResult(out var result) && BuildingUtils.IsHomelessShelterBuilding(((ComponentSystemBase)this).EntityManager, result.m_Owner) && EntitiesExtensions.TryGetBuffer<Renter>(((ComponentSystemBase)this).EntityManager, result.m_Owner, true, ref val))
		{
			DynamicBuffer<HouseholdCitizen> val2 = default(DynamicBuffer<HouseholdCitizen>);
			for (int i = 0; i < val.Length; i++)
			{
				Renter renter = val[i];
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				if (!((EntityManager)(ref entityManager)).HasComponent<HomelessHousehold>(renter.m_Renter) || !EntitiesExtensions.TryGetBuffer<HouseholdCitizen>(((ComponentSystemBase)this).EntityManager, renter.m_Renter, true, ref val2))
				{
					continue;
				}
				for (int j = 0; j < val2.Length; j++)
				{
					HouseholdCitizen householdCitizen = val2[j];
					if (!CitizenUtils.IsDead(((ComponentSystemBase)this).EntityManager, householdCitizen.m_Citizen))
					{
						m_HomelessCountTooltip.value++;
					}
				}
			}
		}
		if (m_HomelessCountTooltip.value > 0)
		{
			AddMouseTooltip(m_HomelessCountTooltip);
		}
	}

	[Preserve]
	public HomelessTooltipSystem()
	{
	}
}
