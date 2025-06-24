using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Settings/", new Type[] { })]
public class AudioSettingsPrefab : PrefabBase
{
	public EffectPrefab[] m_Effects;

	public float m_MinHeight;

	public float m_MaxHeight;

	[Range(0f, 1f)]
	public float m_OverlapRatio = 0.8f;

	[Range(0f, 1f)]
	public float m_MinDistanceRatio = 0.5f;

	[Header("Culling")]
	public int m_FireCullMaxAmount;

	public float m_FireCullMaxDistance;

	public int m_CarEngineCullMaxAmount;

	public float m_CarEngineCullMaxDistance;

	public int m_PublicTransCullMaxAmount;

	public float m_PublicTransCullMaxDistance;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<AmbientAudioSettingsData>());
		components.Add(ComponentType.ReadWrite<AmbientAudioEffect>());
		components.Add(ComponentType.ReadWrite<CullingAudioSettingsData>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		DynamicBuffer<AmbientAudioEffect> buffer = ((EntityManager)(ref entityManager)).GetBuffer<AmbientAudioEffect>(entity, false);
		PrefabSystem orCreateSystemManaged = ((EntityManager)(ref entityManager)).World.GetOrCreateSystemManaged<PrefabSystem>();
		for (int i = 0; i < m_Effects.Length; i++)
		{
			buffer.Add(new AmbientAudioEffect
			{
				m_Effect = orCreateSystemManaged.GetEntity(m_Effects[i])
			});
		}
		AmbientAudioSettingsData ambientAudioSettingsData = new AmbientAudioSettingsData
		{
			m_MaxHeight = m_MaxHeight,
			m_MinDistanceRatio = m_MinDistanceRatio,
			m_MinHeight = m_MinHeight,
			m_OverlapRatio = m_OverlapRatio
		};
		((EntityManager)(ref entityManager)).SetComponentData<AmbientAudioSettingsData>(entity, ambientAudioSettingsData);
		CullingAudioSettingsData cullingAudioSettingsData = new CullingAudioSettingsData
		{
			m_FireCullMaxAmount = m_FireCullMaxAmount,
			m_FireCullMaxDistance = m_FireCullMaxDistance,
			m_CarEngineCullMaxAmount = m_CarEngineCullMaxAmount,
			m_CarEngineCullMaxDistance = m_CarEngineCullMaxDistance,
			m_PublicTransCullMaxAmount = m_PublicTransCullMaxAmount,
			m_PublicTransCullMaxDistance = m_PublicTransCullMaxDistance
		};
		((EntityManager)(ref entityManager)).SetComponentData<CullingAudioSettingsData>(entity, cullingAudioSettingsData);
	}
}
