using System;
using System.Collections.Generic;
using Game.Net;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Net/", new Type[] { typeof(AggregateNetPrefab) })]
public class NetArrow : ComponentBase
{
	public Material m_ArrowMaterial;

	public Color m_RoadArrowColor = Color.white;

	public Color m_TrackArrowColor = Color.white;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<NetArrowData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<ArrowMaterial>());
		components.Add(ComponentType.ReadWrite<ArrowPosition>());
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
		((EntityManager)(ref entityManager)).SetComponentData<NetArrowData>(entity, new NetArrowData
		{
			m_RoadColor = Color32.op_Implicit(((Color)(ref m_RoadArrowColor)).linear),
			m_TrackColor = Color32.op_Implicit(((Color)(ref m_TrackArrowColor)).linear)
		});
	}
}
