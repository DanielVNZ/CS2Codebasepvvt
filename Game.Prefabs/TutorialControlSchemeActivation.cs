using System;
using System.Collections.Generic;
using Game.Input;
using Game.Tutorials;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Tutorials/Activation/", new Type[] { typeof(TutorialPrefab) })]
public class TutorialControlSchemeActivation : TutorialActivation
{
	public InputManager.ControlScheme m_ControlScheme;

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<ControlSchemeActivationData>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<ControlSchemeActivationData>(entity, new ControlSchemeActivationData(m_ControlScheme));
	}
}
