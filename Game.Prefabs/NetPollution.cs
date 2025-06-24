using System;
using System.Collections.Generic;
using Game.Net;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

[ComponentMenu("Net/", new Type[] { typeof(NetPrefab) })]
public class NetPollution : ComponentBase
{
	public float m_NoisePollutionFactor = 1f;

	public float m_AirPollutionFactor = 1f;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<NetPollutionData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (components.Contains(ComponentType.ReadWrite<Node>()) || components.Contains(ComponentType.ReadWrite<Edge>()))
		{
			components.Add(ComponentType.ReadWrite<Game.Net.Pollution>());
		}
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		NetPollutionData netPollutionData = default(NetPollutionData);
		netPollutionData.m_Factors = new float2(m_NoisePollutionFactor, m_AirPollutionFactor);
		((EntityManager)(ref entityManager)).SetComponentData<NetPollutionData>(entity, netPollutionData);
	}
}
