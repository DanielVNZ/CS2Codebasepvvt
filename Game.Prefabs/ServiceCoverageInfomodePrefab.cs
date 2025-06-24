using System;
using System.Collections.Generic;
using Colossal.Mathematics;
using Game.Net;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Tools/Infomode/", new Type[] { })]
public class ServiceCoverageInfomodePrefab : GradientInfomodeBasePrefab
{
	public CoverageService m_Service;

	public Bounds1 m_Range = new Bounds1(0f, 5f);

	public override string infomodeTypeLocaleKey => "NetworkColor";

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<InfoviewCoverageData>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<InfoviewCoverageData>(entity, new InfoviewCoverageData
		{
			m_Service = m_Service,
			m_Range = m_Range
		});
	}
}
