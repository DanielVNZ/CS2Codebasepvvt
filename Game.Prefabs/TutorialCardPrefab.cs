using System;
using Game.Tutorials;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Tutorials/Phases/", new Type[] { })]
public class TutorialCardPrefab : TutorialPhasePrefab
{
	public bool m_CenterCard;

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<TutorialPhaseData>(entity, new TutorialPhaseData
		{
			m_Type = ((!m_CenterCard) ? TutorialPhaseType.Card : TutorialPhaseType.CenterCard),
			m_OverrideCompletionDelay = m_OverrideCompletionDelay
		});
	}
}
