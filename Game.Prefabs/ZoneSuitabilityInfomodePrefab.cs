using System;
using System.Collections.Generic;
using Game.Zones;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Tools/Infomode/", new Type[] { })]
public class ZoneSuitabilityInfomodePrefab : GradientInfomodeBasePrefab
{
	public AreaType m_AreaType;

	public bool m_Office;

	public override string infomodeTypeLocaleKey => "NetworkColor";

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<InfoviewAvailabilityData>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<InfoviewAvailabilityData>(entity, new InfoviewAvailabilityData
		{
			m_AreaType = m_AreaType,
			m_Office = m_Office
		});
	}
}
