using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Tutorials/", new Type[] { typeof(TriggerPrefab) })]
public class TutorialActivationEvent : ComponentBase
{
	public TutorialPrefab[] m_Tutorials;

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<TutorialActivationEventData>());
	}

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		if (m_Tutorials != null)
		{
			for (int i = 0; i < m_Tutorials.Length; i++)
			{
				prefabs.Add(m_Tutorials[i]);
			}
		}
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem orCreateSystemManaged = ((EntityManager)(ref entityManager)).World.GetOrCreateSystemManaged<PrefabSystem>();
		DynamicBuffer<TutorialActivationEventData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<TutorialActivationEventData>(entity, false);
		if (m_Tutorials != null)
		{
			for (int i = 0; i < m_Tutorials.Length; i++)
			{
				buffer.Add(new TutorialActivationEventData
				{
					m_Tutorial = orCreateSystemManaged.GetEntity(m_Tutorials[i])
				});
			}
		}
	}
}
