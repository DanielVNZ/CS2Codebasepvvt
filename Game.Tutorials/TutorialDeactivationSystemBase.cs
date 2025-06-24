using Game.Common;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.Tutorials;

public abstract class TutorialDeactivationSystemBase : GameSystemBase
{
	private EntityQuery m_ActivePhaseQuery;

	protected EntityCommandBufferSystem m_BarrierSystem;

	protected bool phaseCanDeactivate => !((EntityQuery)(ref m_ActivePhaseQuery)).IsEmptyIgnoreFilter;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_BarrierSystem = (EntityCommandBufferSystem)(object)((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier3>();
		m_ActivePhaseQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<TutorialPhaseData>(),
			ComponentType.ReadOnly<TutorialPhaseActive>(),
			ComponentType.ReadOnly<TutorialPhaseCanDeactivate>()
		});
	}

	[Preserve]
	protected TutorialDeactivationSystemBase()
	{
	}
}
