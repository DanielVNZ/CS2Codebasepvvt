using System;
using System.Collections.Generic;
using Game.Economy;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Buildings/", new Type[] { typeof(BuildingExtensionPrefab) })]
public class UpkeepModifier : ComponentBase, IServiceUpgrade
{
	public UpkeepModifierInfo[] m_Modifiers;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<UpkeepModifierData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public void GetUpgradeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<UpkeepModifier>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		if (m_Modifiers != null)
		{
			DynamicBuffer<UpkeepModifierData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<UpkeepModifierData>(entity, false);
			for (int i = 0; i < m_Modifiers.Length; i++)
			{
				UpkeepModifierInfo upkeepModifierInfo = m_Modifiers[i];
				buffer.Add(new UpkeepModifierData
				{
					m_Resource = EconomyUtils.GetResource(upkeepModifierInfo.m_Resource),
					m_Multiplier = upkeepModifierInfo.m_Multiplier
				});
			}
		}
	}
}
