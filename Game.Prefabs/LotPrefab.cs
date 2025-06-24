using System;
using System.Collections.Generic;
using Game.Areas;
using Game.Common;
using Game.Objects;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Areas/", new Type[] { })]
public class LotPrefab : AreaPrefab
{
	public float m_MaxRadius = 200f;

	public Color m_RangeColor = Color.white;

	public bool m_OnWater;

	public bool m_AllowOverlap;

	public bool m_AllowEditing = true;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<LotData>());
		components.Add(ComponentType.ReadWrite<AreaGeometryData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		components.Add(ComponentType.ReadWrite<Lot>());
		components.Add(ComponentType.ReadWrite<Geometry>());
		components.Add(ComponentType.ReadWrite<Game.Objects.SubObject>());
		components.Add(ComponentType.ReadWrite<PseudoRandomSeed>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<LotData>(entity, new LotData(m_MaxRadius, Color32.op_Implicit(m_RangeColor), m_OnWater, m_AllowOverlap));
	}
}
