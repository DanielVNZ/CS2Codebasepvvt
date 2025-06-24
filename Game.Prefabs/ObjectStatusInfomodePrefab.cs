using System;
using System.Collections.Generic;
using Colossal.Mathematics;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Tools/Infomode/", new Type[] { })]
public class ObjectStatusInfomodePrefab : GradientInfomodeBasePrefab
{
	public ObjectStatusType m_Type;

	public Bounds1 m_Range;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<InfoviewObjectStatusData>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<InfoviewObjectStatusData>(entity, new InfoviewObjectStatusData
		{
			m_Type = m_Type,
			m_Range = m_Range
		});
	}
}
