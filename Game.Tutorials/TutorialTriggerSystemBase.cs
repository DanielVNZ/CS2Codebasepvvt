using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game.Common;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.Tutorials;

public abstract class TutorialTriggerSystemBase : GameSystemBase
{
	protected ModificationBarrier5 m_BarrierSystem;

	protected EntityQuery m_ActiveTriggerQuery;

	private TutorialSystem m_TutorialSystem;

	private Entity m_LastPhase;

	protected bool triggersChanged { get; private set; }

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_BarrierSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier5>();
		m_TutorialSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TutorialSystem>();
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		base.OnGameLoaded(serializationContext);
		m_LastPhase = Entity.Null;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		Entity activeTutorialPhase = m_TutorialSystem.activeTutorialPhase;
		if (activeTutorialPhase != m_LastPhase)
		{
			m_LastPhase = activeTutorialPhase;
			triggersChanged = true;
		}
		else
		{
			triggersChanged = false;
		}
	}

	[Preserve]
	protected override void OnStopRunning()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		((COSystemBase)this).OnStopRunning();
		m_LastPhase = Entity.Null;
	}

	[Preserve]
	protected TutorialTriggerSystemBase()
	{
	}
}
