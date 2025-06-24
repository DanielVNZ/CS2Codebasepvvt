using System;
using System.Collections.Generic;
using Game.Objects;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Objects/", new Type[] { typeof(ObjectPrefab) })]
public class SubObjectDefaultProbability : ComponentBase
{
	[Range(0f, 100f)]
	public int m_DefaultProbability = 100;

	public RotationSymmetry m_RotationSymmetry;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<PlaceableObjectData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		if (base.prefab.Has<ServiceUpgrade>())
		{
			ComponentBase.baseLog.ErrorFormat((Object)(object)base.prefab, "ServiceUpgrade cannot have SubObjectDefaultProbability: {0}", (object)((Object)base.prefab).name);
		}
		PlaceableObjectData componentData = ((EntityManager)(ref entityManager)).GetComponentData<PlaceableObjectData>(entity);
		componentData.m_DefaultProbability = (byte)m_DefaultProbability;
		componentData.m_RotationSymmetry = m_RotationSymmetry;
		componentData.m_Flags |= PlacementFlags.HasProbability;
		((EntityManager)(ref entityManager)).SetComponentData<PlaceableObjectData>(entity, componentData);
	}
}
