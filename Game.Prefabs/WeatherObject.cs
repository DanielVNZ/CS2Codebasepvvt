using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Objects/", new Type[]
{
	typeof(StaticObjectPrefab),
	typeof(MarkerObjectPrefab)
})]
public class WeatherObject : ComponentBase
{
	public bool m_RequireSnow;

	public bool m_ForbidSnow;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<ObjectRequirementElement>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		DynamicBuffer<ObjectRequirementElement> buffer = ((EntityManager)(ref entityManager)).GetBuffer<ObjectRequirementElement>(entity, false);
		int length = buffer.Length;
		ObjectRequirementFlags objectRequirementFlags = (ObjectRequirementFlags)0;
		ObjectRequirementFlags objectRequirementFlags2 = (ObjectRequirementFlags)0;
		if (m_RequireSnow)
		{
			objectRequirementFlags |= ObjectRequirementFlags.Snow;
		}
		if (m_ForbidSnow)
		{
			objectRequirementFlags2 |= ObjectRequirementFlags.Snow;
		}
		buffer.Add(new ObjectRequirementElement(objectRequirementFlags, objectRequirementFlags2, length));
	}
}
