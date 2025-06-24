using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[RequireComponent(typeof(ManualUnlockable))]
public class DevTreeNodePrefab : PrefabBase
{
	public ServicePrefab m_Service;

	public DevTreeNodePrefab[] m_Requirements;

	public int m_Cost;

	public int m_HorizontalPosition;

	public float m_VerticalPosition;

	public string m_IconPath;

	public PrefabBase m_IconPrefab;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		if ((Object)(object)m_Service != (Object)null)
		{
			prefabs.Add(m_Service);
		}
		if (m_Requirements == null)
		{
			return;
		}
		DevTreeNodePrefab[] requirements = m_Requirements;
		foreach (DevTreeNodePrefab devTreeNodePrefab in requirements)
		{
			if ((Object)(object)devTreeNodePrefab != (Object)null)
			{
				prefabs.Add(devTreeNodePrefab);
			}
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<DevTreeNodeData>());
		if (HasRequirements())
		{
			components.Add(ComponentType.ReadWrite<DevTreeNodeRequirement>());
		}
		if (m_Cost == 0)
		{
			components.Add(ComponentType.ReadWrite<DevTreeNodeAutoUnlock>());
		}
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		Entity entity2 = existingSystemManaged.GetEntity(m_Service);
		((EntityManager)(ref entityManager)).SetComponentData<DevTreeNodeData>(entity, new DevTreeNodeData
		{
			m_Cost = m_Cost,
			m_Service = entity2
		});
		if (!((EntityManager)(ref entityManager)).HasComponent<DevTreeNodeRequirement>(entity))
		{
			return;
		}
		DynamicBuffer<DevTreeNodeRequirement> buffer = ((EntityManager)(ref entityManager)).GetBuffer<DevTreeNodeRequirement>(entity, false);
		DevTreeNodePrefab[] requirements = m_Requirements;
		foreach (DevTreeNodePrefab devTreeNodePrefab in requirements)
		{
			if ((Object)(object)devTreeNodePrefab != (Object)null)
			{
				Entity entity3 = existingSystemManaged.GetEntity(devTreeNodePrefab);
				buffer.Add(new DevTreeNodeRequirement
				{
					m_Node = entity3
				});
			}
		}
	}

	private bool HasRequirements()
	{
		if (m_Requirements != null)
		{
			DevTreeNodePrefab[] requirements = m_Requirements;
			for (int i = 0; i < requirements.Length; i++)
			{
				if ((Object)(object)requirements[i] != (Object)null)
				{
					return true;
				}
			}
		}
		return false;
	}
}
