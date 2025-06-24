using System;
using System.Collections.Generic;
using Game.Areas;
using Game.Policies;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Areas/", new Type[] { })]
public class DistrictPrefab : AreaPrefab
{
	public Color m_NameColor = Color.white;

	public Color m_SelectedNameColor = new Color(0.5f, 0.75f, 1f, 1f);

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<DistrictData>());
		components.Add(ComponentType.ReadWrite<AreaNameData>());
		components.Add(ComponentType.ReadWrite<AreaGeometryData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		components.Add(ComponentType.ReadWrite<District>());
		components.Add(ComponentType.ReadWrite<Geometry>());
		components.Add(ComponentType.ReadWrite<LabelExtents>());
		components.Add(ComponentType.ReadWrite<LabelVertex>());
		components.Add(ComponentType.ReadWrite<DistrictModifier>());
		components.Add(ComponentType.ReadWrite<Policy>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<AreaNameData>(entity, new AreaNameData
		{
			m_Color = Color32.op_Implicit(m_NameColor),
			m_SelectedColor = Color32.op_Implicit(m_SelectedNameColor)
		});
	}
}
