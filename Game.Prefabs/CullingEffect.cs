using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Effects/", new Type[] { typeof(EffectPrefab) })]
public class CullingEffect : ComponentBase
{
	public enum AudioCullingGroup : byte
	{
		None,
		Fire,
		CarEngine,
		PublicTrans,
		Count
	}

	[Tooltip("The audio culling group")]
	public AudioCullingGroup m_AudioCullGroup;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (m_AudioCullGroup != AudioCullingGroup.None)
		{
			components.Add(ComponentType.ReadWrite<CullingGroupData>());
		}
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		if (m_AudioCullGroup != AudioCullingGroup.None)
		{
			CullingGroupData cullingGroupData = new CullingGroupData
			{
				m_GroupIndex = (int)m_AudioCullGroup
			};
			((EntityManager)(ref entityManager)).SetComponentData<CullingGroupData>(entity, cullingGroupData);
		}
	}
}
