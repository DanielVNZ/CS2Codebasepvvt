using System;
using System.Collections.Generic;
using Colossal.Annotations;
using Colossal.Entities;
using Game.Tutorials;
using Unity.Collections;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Tutorials/", new Type[] { })]
public class TutorialPrefab : PrefabBase
{
	[NotNull]
	public TutorialPhasePrefab[] m_Phases;

	public int m_Priority;

	public bool m_ReplaceActive;

	public bool m_Mandatory;

	public bool m_EditorTutorial;

	public bool m_FireTelemetry;

	public override bool ignoreUnlockDependencies => true;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		TutorialPhasePrefab[] phases = m_Phases;
		foreach (TutorialPhasePrefab item in phases)
		{
			prefabs.Add(item);
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<TutorialData>());
		components.Add(ComponentType.ReadWrite<TutorialPhaseRef>());
		if (m_ReplaceActive)
		{
			components.Add(ComponentType.ReadWrite<ReplaceActiveData>());
		}
		if (m_FireTelemetry)
		{
			components.Add(ComponentType.ReadOnly<TutorialFireTelemetry>());
		}
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<TutorialData>(entity, new TutorialData(m_Priority));
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem existingSystemManaged = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PrefabSystem>();
		DynamicBuffer<TutorialPhaseRef> buffer = ((EntityManager)(ref entityManager)).GetBuffer<TutorialPhaseRef>(entity, false);
		NativeParallelHashSet<Entity> linkedPrefabs = default(NativeParallelHashSet<Entity>);
		linkedPrefabs._002Ector(5, AllocatorHandle.op_Implicit((Allocator)3));
		TutorialPhasePrefab[] phases = m_Phases;
		foreach (TutorialPhasePrefab tutorialPhasePrefab in phases)
		{
			Entity entity2 = existingSystemManaged.GetEntity(tutorialPhasePrefab);
			TutorialPhaseRef tutorialPhaseRef = new TutorialPhaseRef
			{
				m_Phase = entity2
			};
			buffer.Add(tutorialPhaseRef);
			tutorialPhasePrefab.GenerateTutorialLinks(entityManager, linkedPrefabs);
		}
		Enumerator<Entity> enumerator = linkedPrefabs.GetEnumerator();
		try
		{
			DynamicBuffer<TutorialLinkData> val = default(DynamicBuffer<TutorialLinkData>);
			while (enumerator.MoveNext())
			{
				Entity current = enumerator.Current;
				if (!EntitiesExtensions.TryGetBuffer<TutorialLinkData>(entityManager, current, false, ref val))
				{
					val = ((EntityManager)(ref entityManager)).AddBuffer<TutorialLinkData>(current);
				}
				val.Add(new TutorialLinkData
				{
					m_Tutorial = entity
				});
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		if (m_EditorTutorial)
		{
			((EntityManager)(ref entityManager)).AddComponent<EditorTutorial>(entity);
		}
		linkedPrefabs.Dispose();
	}
}
