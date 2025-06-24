using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Prefabs/Content/", new Type[] { })]
public class ContentPrefab : PrefabBase
{
	public bool IsAvailable()
	{
		foreach (ComponentBase component in components)
		{
			if (component is ContentRequirementBase contentRequirementBase && !contentRequirementBase.CheckRequirement())
			{
				return false;
			}
		}
		return true;
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<ContentData>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		ContentData componentData = ((EntityManager)(ref entityManager)).GetComponentData<ContentData>(entity);
		if (TryGet<DlcRequirement>(out var component))
		{
			componentData.m_Flags |= ContentFlags.RequireDlc;
			componentData.m_DlcID = component.m_Dlc.id;
		}
		if (Has<PdxLoginRequirement>())
		{
			componentData.m_Flags |= ContentFlags.RequirePdxLogin;
		}
		((EntityManager)(ref entityManager)).SetComponentData<ContentData>(entity, componentData);
	}
}
