using System;
using System.Collections.Generic;
using Colossal.Collections;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Settings/", new Type[] { })]
public class IconConfigurationPrefab : PrefabBase
{
	public Material m_Material;

	public NotificationIconPrefab m_SelectedMarker;

	public NotificationIconPrefab m_FollowedMarker;

	public IconAnimationInfo[] m_Animations;

	public Texture2D m_MissingIcon;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		prefabs.Add(m_SelectedMarker);
		prefabs.Add(m_FollowedMarker);
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<IconConfigurationData>());
		components.Add(ComponentType.ReadWrite<IconAnimationElement>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		IconConfigurationData iconConfigurationData = default(IconConfigurationData);
		iconConfigurationData.m_SelectedMarker = existingSystemManaged.GetEntity(m_SelectedMarker);
		iconConfigurationData.m_FollowedMarker = existingSystemManaged.GetEntity(m_FollowedMarker);
		((EntityManager)(ref entityManager)).SetComponentData<IconConfigurationData>(entity, iconConfigurationData);
		if (m_Animations == null)
		{
			return;
		}
		DynamicBuffer<IconAnimationElement> buffer = ((EntityManager)(ref entityManager)).GetBuffer<IconAnimationElement>(entity, false);
		for (int i = 0; i < m_Animations.Length; i++)
		{
			IconAnimationInfo iconAnimationInfo = m_Animations[i];
			IconAnimationElement iconAnimationElement = new IconAnimationElement
			{
				m_Duration = iconAnimationInfo.m_Duration,
				m_AnimationCurve = new AnimationCurve3(iconAnimationInfo.m_Scale, iconAnimationInfo.m_Alpha, iconAnimationInfo.m_ScreenY)
			};
			int type = (int)iconAnimationInfo.m_Type;
			if (buffer.Length > type)
			{
				buffer[type] = iconAnimationElement;
				continue;
			}
			while (buffer.Length < type)
			{
				buffer.Add(default(IconAnimationElement));
			}
			buffer.Add(iconAnimationElement);
		}
	}
}
