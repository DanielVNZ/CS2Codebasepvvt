using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Settings/", new Type[] { })]
public class AudioGroupingSettingsPrefab : PrefabBase
{
	public AudioGroupSettings[] m_Settings;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<AudioGroupingSettingsData>());
	}

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		if (m_Settings != null)
		{
			for (int i = 0; i < m_Settings.Length; i++)
			{
				if ((Object)(object)m_Settings[i].m_GroupSoundFar != (Object)null)
				{
					prefabs.Add(m_Settings[i].m_GroupSoundFar);
				}
				if ((Object)(object)m_Settings[i].m_GroupSoundNear != (Object)null)
				{
					prefabs.Add(m_Settings[i].m_GroupSoundNear);
				}
			}
		}
		base.GetDependencies(prefabs);
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem orCreateSystemManaged = ((EntityManager)(ref entityManager)).World.GetOrCreateSystemManaged<PrefabSystem>();
		DynamicBuffer<AudioGroupingSettingsData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<AudioGroupingSettingsData>(entity, false);
		if (m_Settings != null)
		{
			for (int i = 0; i < m_Settings.Length; i++)
			{
				AudioGroupSettings audioGroupSettings = m_Settings[i];
				buffer.Add(new AudioGroupingSettingsData
				{
					m_Type = audioGroupSettings.m_Type,
					m_FadeSpeed = audioGroupSettings.m_FadeSpeed,
					m_Scale = audioGroupSettings.m_Scale,
					m_GroupSoundFar = orCreateSystemManaged.GetEntity(audioGroupSettings.m_GroupSoundFar),
					m_GroupSoundNear = (((Object)(object)audioGroupSettings.m_GroupSoundNear != (Object)null) ? orCreateSystemManaged.GetEntity(audioGroupSettings.m_GroupSoundNear) : Entity.Null),
					m_Height = audioGroupSettings.m_Height,
					m_NearHeight = audioGroupSettings.m_NearHeight,
					m_NearWeight = audioGroupSettings.m_NearWeight
				});
			}
		}
	}
}
