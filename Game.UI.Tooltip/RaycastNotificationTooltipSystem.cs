using System.Runtime.CompilerServices;
using Colossal.Entities;
using Game.Notifications;
using Game.Prefabs;
using Game.Tools;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.Tooltip;

[CompilerGenerated]
public class RaycastNotificationTooltipSystem : TooltipSystemBase
{
	private ToolSystem m_ToolSystem;

	private DefaultToolSystem m_DefaultTool;

	private PrefabSystem m_PrefabSystem;

	private NameSystem m_NameSystem;

	private ImageSystem m_ImageSystem;

	private ToolRaycastSystem m_ToolRaycastSystem;

	private EntityQuery m_ConfigurationQuery;

	private NotificationTooltip m_Tooltip;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_DefaultTool = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DefaultToolSystem>();
		m_NameSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NameSystem>();
		m_ImageSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ImageSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_ToolRaycastSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolRaycastSystem>();
		m_ConfigurationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<IconConfigurationData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_ConfigurationQuery);
		m_Tooltip = new NotificationTooltip
		{
			path = "raycastNotification",
			verbose = true
		};
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		Icon icon = default(Icon);
		PrefabRef refData = default(PrefabRef);
		if (m_ToolSystem.activeTool == m_DefaultTool && m_ToolRaycastSystem.GetRaycastResult(out var result) && EntitiesExtensions.TryGetComponent<Icon>(((ComponentSystemBase)this).EntityManager, result.m_Owner, ref icon) && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, result.m_Owner, ref refData))
		{
			IconConfigurationData singleton = ((EntityQuery)(ref m_ConfigurationQuery)).GetSingleton<IconConfigurationData>();
			if (!(refData.m_Prefab == singleton.m_SelectedMarker) && !(refData.m_Prefab == singleton.m_FollowedMarker))
			{
				m_Tooltip.name = (m_PrefabSystem.TryGetPrefab<NotificationIconPrefab>(refData, out var prefab) ? ((Object)prefab).name : m_PrefabSystem.GetObsoleteID(refData.m_Prefab).GetName());
				m_Tooltip.color = NotificationTooltip.GetColor(icon.m_Priority);
				AddMouseTooltip(m_Tooltip);
			}
		}
	}

	[Preserve]
	public RaycastNotificationTooltipSystem()
	{
	}
}
