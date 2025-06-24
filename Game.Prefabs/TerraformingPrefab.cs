using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Tools/", new Type[] { })]
public class TerraformingPrefab : PrefabBase
{
	public TerraformingType m_Type;

	public TerraformingTarget m_Target;

	public Material m_BrushMaterial;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<TerraformingData>());
		components.Add(ComponentType.ReadWrite<PlaceableInfoviewItem>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		TerraformingData terraformingData = default(TerraformingData);
		terraformingData.m_Type = m_Type;
		terraformingData.m_Target = m_Target;
		((EntityManager)(ref entityManager)).SetComponentData<TerraformingData>(entity, terraformingData);
	}
}
