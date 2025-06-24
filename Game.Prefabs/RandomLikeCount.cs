using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Chirps/", new Type[] { typeof(ChirpPrefab) })]
public class RandomLikeCount : ComponentBase
{
	[Tooltip("Percentage of the educated people will give a like")]
	[Range(0f, 1f)]
	public float m_EducatedPercentage = 0.8f;

	[Tooltip("Percentage of the uneducated people will give a like")]
	[Range(0f, 1f)]
	public float m_UneducatedPercentage = 0.5f;

	[Tooltip("The like count of the chirp will actively increase during these days")]
	public float2 m_ActiveDays = new float2(0.1f, 1f);

	[Tooltip("Go viral means the like count will increase significantly at the beginning in short time")]
	public int2 m_GoViralFactor = new int2(20, 60);

	[Tooltip("Use random amount factor to make the like amount result more randomly)")]
	public float2 m_RandomAmountFactor = new float2(0.01f, 0.8f);

	[Tooltip("Use continuous factor to make the like count change continuous of not continuous, value belong to random[0,1))")]
	public float m_ContinuousFactor = 0.2f;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<RandomLikeCountData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		RandomLikeCountData randomLikeCountData = new RandomLikeCountData
		{
			m_EducatedPercentage = m_EducatedPercentage,
			m_UneducatedPercentage = m_UneducatedPercentage,
			m_GoViralFactor = m_GoViralFactor,
			m_ActiveDays = m_ActiveDays,
			m_RandomAmountFactor = m_RandomAmountFactor,
			m_ContinuousFactor = m_ContinuousFactor
		};
		((EntityManager)(ref entityManager)).SetComponentData<RandomLikeCountData>(entity, randomLikeCountData);
	}
}
