using System;
using System.Collections.Generic;
using Game.Buildings;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

[ComponentMenu("Tools/Infomode/", new Type[] { })]
public class EffectRangeInfomodePrefab : ColorInfomodeBasePrefab
{
	public LocalModifierType m_Type;

	public override string infomodeTypeLocaleKey => "Radius";

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<InfoviewLocalEffectData>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<InfoviewLocalEffectData>(entity, new InfoviewLocalEffectData
		{
			m_Type = m_Type,
			m_Color = new float4(m_Color.r, m_Color.g, m_Color.b, m_Color.a)
		});
	}
}
