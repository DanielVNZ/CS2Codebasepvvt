using System;
using System.Collections.Generic;
using Colossal.Mathematics;
using Game.Buildings;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Buildings/", new Type[]
{
	typeof(BuildingPrefab),
	typeof(BuildingExtensionPrefab)
})]
public class LocalEffects : ComponentBase, IServiceUpgrade
{
	public LocalEffectInfo[] m_Effects;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<LocalModifierData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)GetComponent<ServiceUpgrade>() == (Object)null)
		{
			components.Add(ComponentType.ReadWrite<LocalEffectProvider>());
		}
	}

	public void GetUpgradeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<LocalEffectProvider>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		if (m_Effects != null)
		{
			DynamicBuffer<LocalModifierData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<LocalModifierData>(entity, false);
			for (int i = 0; i < m_Effects.Length; i++)
			{
				LocalEffectInfo localEffectInfo = m_Effects[i];
				buffer.Add(new LocalModifierData(localEffectInfo.m_Type, localEffectInfo.m_Mode, localEffectInfo.m_RadiusCombineMode, new Bounds1(0f, localEffectInfo.m_Delta), new Bounds1(0f, localEffectInfo.m_Radius)));
			}
		}
	}
}
