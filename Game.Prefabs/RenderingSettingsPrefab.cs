using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Settings/", new Type[] { })]
public class RenderingSettingsPrefab : PrefabBase
{
	public Color m_HoveredColor = new Color(0.5f, 0.5f, 1f, 0.1f);

	public Color m_OverrideColor = new Color(1f, 1f, 1f, 0.1f);

	public Color m_WarningColor = new Color(1f, 1f, 0.5f, 0.1f);

	public Color m_ErrorColor = new Color(1f, 0.5f, 0.5f, 0.1f);

	public Color m_OwnerColor = new Color(0.5f, 1f, 0.5f, 0.1f);

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<RenderingSettingsData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		base.GetArchetypeComponents(components);
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		RenderingSettingsData renderingSettingsData = new RenderingSettingsData
		{
			m_HoveredColor = m_HoveredColor,
			m_OverrideColor = m_OverrideColor,
			m_WarningColor = m_WarningColor,
			m_ErrorColor = m_ErrorColor,
			m_OwnerColor = m_OwnerColor
		};
		((EntityManager)(ref entityManager)).SetComponentData<RenderingSettingsData>(entity, renderingSettingsData);
	}
}
