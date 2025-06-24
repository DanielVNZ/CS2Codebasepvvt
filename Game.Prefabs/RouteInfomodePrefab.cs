using System;
using System.Collections.Generic;
using Game.Routes;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Tools/Infomode/", new Type[] { })]
public class RouteInfomodePrefab : InfomodeBasePrefab
{
	public RouteType m_Type;

	public override string infomodeTypeLocaleKey => "NetworkColor";

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<InfoviewRouteData>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<InfoviewRouteData>(entity, new InfoviewRouteData
		{
			m_Type = m_Type
		});
	}
}
