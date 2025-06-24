using System.Runtime.CompilerServices;
using Colossal.Entities;
using Game.Buildings;
using Game.Creatures;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Routes;
using Game.Tools;
using Game.Vehicles;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.Tooltip;

[CompilerGenerated]
public class RaycastNameTooltipSystem : TooltipSystemBase
{
	private ToolSystem m_ToolSystem;

	private DefaultToolSystem m_DefaultTool;

	private NameSystem m_NameSystem;

	private ImageSystem m_ImageSystem;

	private ToolRaycastSystem m_ToolRaycastSystem;

	private NameTooltip m_Tooltip;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_DefaultTool = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DefaultToolSystem>();
		m_NameSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NameSystem>();
		m_ImageSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ImageSystem>();
		m_ToolRaycastSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolRaycastSystem>();
		m_Tooltip = new NameTooltip
		{
			path = "raycastName",
			nameBinder = m_NameSystem
		};
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		if (m_ToolSystem.activeTool != m_DefaultTool || !m_ToolRaycastSystem.GetRaycastResult(out var result))
		{
			return;
		}
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<Building>(result.m_Owner))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (!((EntityManager)(ref entityManager)).HasComponent<Game.Routes.TransportStop>(result.m_Owner))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (!((EntityManager)(ref entityManager)).HasComponent<Game.Objects.OutsideConnection>(result.m_Owner))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (!((EntityManager)(ref entityManager)).HasComponent<Route>(result.m_Owner))
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						if (!((EntityManager)(ref entityManager)).HasComponent<Creature>(result.m_Owner))
						{
							entityManager = ((ComponentSystemBase)this).EntityManager;
							if (!((EntityManager)(ref entityManager)).HasComponent<Vehicle>(result.m_Owner))
							{
								entityManager = ((ComponentSystemBase)this).EntityManager;
								if (!((EntityManager)(ref entityManager)).HasComponent<Aggregate>(result.m_Owner))
								{
									entityManager = ((ComponentSystemBase)this).EntityManager;
									if (!((EntityManager)(ref entityManager)).HasComponent<Game.Objects.NetObject>(result.m_Owner))
									{
										return;
									}
								}
							}
						}
					}
				}
			}
		}
		PrefabRef prefabRef = default(PrefabRef);
		if (EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, result.m_Owner, ref prefabRef))
		{
			Entity instance = result.m_Owner;
			Entity prefab = prefabRef.m_Prefab;
			AdjustTargets(ref instance, ref prefab);
			m_Tooltip.icon = m_ImageSystem.GetInstanceIcon(instance, prefab);
			m_Tooltip.entity = instance;
			AddMouseTooltip(m_Tooltip);
		}
	}

	private void AdjustTargets(ref Entity instance, ref Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		Game.Creatures.Resident resident = default(Game.Creatures.Resident);
		PrefabRef prefabRef = default(PrefabRef);
		if (EntitiesExtensions.TryGetComponent<Game.Creatures.Resident>(((ComponentSystemBase)this).EntityManager, instance, ref resident) && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, resident.m_Citizen, ref prefabRef))
		{
			instance = resident.m_Citizen;
			prefab = prefabRef.m_Prefab;
		}
		Controller controller = default(Controller);
		PrefabRef prefabRef2 = default(PrefabRef);
		if (EntitiesExtensions.TryGetComponent<Controller>(((ComponentSystemBase)this).EntityManager, instance, ref controller) && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, controller.m_Controller, ref prefabRef2))
		{
			instance = controller.m_Controller;
			prefab = prefabRef2.m_Prefab;
		}
		Game.Creatures.Pet pet = default(Game.Creatures.Pet);
		PrefabRef prefabRef3 = default(PrefabRef);
		if (EntitiesExtensions.TryGetComponent<Game.Creatures.Pet>(((ComponentSystemBase)this).EntityManager, instance, ref pet) && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, pet.m_HouseholdPet, ref prefabRef3))
		{
			instance = pet.m_HouseholdPet;
			prefab = prefabRef3.m_Prefab;
		}
	}

	[Preserve]
	public RaycastNameTooltipSystem()
	{
	}
}
