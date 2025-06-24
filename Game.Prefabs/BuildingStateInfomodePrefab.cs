using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Tools/Infomode/", new Type[] { })]
public class BuildingStateInfomodePrefab : ColorInfomodeBasePrefab
{
	public BuildingStatusType m_Type;

	public override string infomodeTypeLocaleKey => "BuildingColor";

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<InfoviewBuildingStatusData>());
		if (m_Type == BuildingStatusType.LeisureProvider)
		{
			components.Add(ComponentType.ReadWrite<InfoviewNetStatusData>());
		}
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<InfoviewBuildingStatusData>(entity, new InfoviewBuildingStatusData
		{
			m_Type = m_Type
		});
		if (m_Type == BuildingStatusType.LeisureProvider)
		{
			((EntityManager)(ref entityManager)).SetComponentData<InfoviewNetStatusData>(entity, new InfoviewNetStatusData
			{
				m_Type = NetStatusType.LeisureProvider
			});
		}
	}
}
