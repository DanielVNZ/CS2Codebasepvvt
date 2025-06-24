using System;
using System.Collections.Generic;
using Game.Net;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Net/", new Type[] { typeof(AggregateNetPrefab) })]
public class NetLabel : ComponentBase
{
	public Material m_NameMaterial;

	public Color m_NameColor = Color.white;

	public Color m_SelectedNameColor = new Color(0.5f, 0.75f, 1f, 1f);

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<NetNameData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<LabelMaterial>());
		components.Add(ComponentType.ReadWrite<LabelExtents>());
		components.Add(ComponentType.ReadWrite<LabelPosition>());
		components.Add(ComponentType.ReadWrite<LabelVertex>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<NetNameData>(entity, new NetNameData
		{
			m_Color = Color32.op_Implicit(((Color)(ref m_NameColor)).linear),
			m_SelectedColor = Color32.op_Implicit(((Color)(ref m_SelectedNameColor)).linear)
		});
	}
}
