using System;
using System.Collections.Generic;
using Game.City;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Statistics/", new Type[] { })]
public abstract class ParametricStatistic : StatisticsPrefab
{
	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<StatisticParameterData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		components.Add(ComponentType.ReadWrite<StatisticParameter>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		DynamicBuffer<StatisticParameterData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<StatisticParameterData>(entity, false);
		foreach (StatisticParameterData parameter in GetParameters())
		{
			buffer.Add(parameter);
		}
	}

	public abstract IEnumerable<StatisticParameterData> GetParameters();

	public abstract string GetParameterName(int parameter);
}
