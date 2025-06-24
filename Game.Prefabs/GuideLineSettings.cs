using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Settings/", new Type[] { typeof(RenderingSettingsPrefab) })]
public class GuideLineSettings : ComponentBase
{
	[Serializable]
	public class WaterSourceColor
	{
		public Color m_Outline;

		public Color m_Fill;

		public Color m_ProjectedOutline;

		public Color m_ProjectedFill;
	}

	public Color m_VeryLowPriorityColor = new Color(0.7f, 0.7f, 1f, 0.025f);

	public Color m_LowPriorityColor = new Color(0.7f, 0.7f, 1f, 0.05f);

	public Color m_MediumPriorityColor = new Color(0.7f, 0.7f, 1f, 0.1f);

	public Color m_HighPriorityColor = new Color(0.7f, 0.7f, 1f, 0.2f);

	public Color m_PositiveFeedbackColor = new Color(0.5f, 1f, 0.5f, 0.1f);

	public WaterSourceColor[] m_WaterSourceColors;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<GuideLineSettingsData>());
		components.Add(ComponentType.ReadWrite<WaterSourceColorElement>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
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
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		GuideLineSettingsData guideLineSettingsData = new GuideLineSettingsData
		{
			m_VeryLowPriorityColor = m_VeryLowPriorityColor,
			m_LowPriorityColor = m_LowPriorityColor,
			m_MediumPriorityColor = m_MediumPriorityColor,
			m_HighPriorityColor = m_HighPriorityColor,
			m_PositiveFeedbackColor = m_PositiveFeedbackColor
		};
		((EntityManager)(ref entityManager)).SetComponentData<GuideLineSettingsData>(entity, guideLineSettingsData);
		if (m_WaterSourceColors != null)
		{
			DynamicBuffer<WaterSourceColorElement> buffer = ((EntityManager)(ref entityManager)).GetBuffer<WaterSourceColorElement>(entity, false);
			buffer.ResizeUninitialized(m_WaterSourceColors.Length);
			for (int i = 0; i < m_WaterSourceColors.Length; i++)
			{
				WaterSourceColor waterSourceColor = m_WaterSourceColors[i];
				buffer[i] = new WaterSourceColorElement
				{
					m_Outline = waterSourceColor.m_Outline,
					m_Fill = waterSourceColor.m_Fill,
					m_ProjectedOutline = waterSourceColor.m_ProjectedOutline,
					m_ProjectedFill = waterSourceColor.m_ProjectedFill
				};
			}
		}
	}
}
