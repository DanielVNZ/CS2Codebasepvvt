using System;
using System.Collections.Generic;
using Game.Tools;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Notifications/", new Type[] { typeof(NotificationIconPrefab) })]
public class ToolError : ComponentBase
{
	public ErrorType m_Error;

	public bool m_TemporaryOnly;

	public bool m_DisableInGame;

	public bool m_DisableInEditor;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<ToolErrorData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		ToolErrorData toolErrorData = default(ToolErrorData);
		toolErrorData.m_Error = m_Error;
		toolErrorData.m_Flags = (ToolErrorFlags)0;
		if (m_TemporaryOnly)
		{
			toolErrorData.m_Flags |= ToolErrorFlags.TemporaryOnly;
		}
		if (m_DisableInGame)
		{
			toolErrorData.m_Flags |= ToolErrorFlags.DisableInGame;
		}
		if (m_DisableInEditor)
		{
			toolErrorData.m_Flags |= ToolErrorFlags.DisableInEditor;
		}
		((EntityManager)(ref entityManager)).SetComponentData<ToolErrorData>(entity, toolErrorData);
	}
}
