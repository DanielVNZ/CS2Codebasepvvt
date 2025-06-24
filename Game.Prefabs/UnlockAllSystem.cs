using System.Runtime.CompilerServices;
using Game.Common;
using Game.Simulation;
using Game.UI.InGame;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.Prefabs;

[CompilerGenerated]
public class UnlockAllSystem : GameSystemBase
{
	private MilestoneSystem m_MilestoneSystem;

	private ModificationBarrier1 m_ModificationBarrier;

	private UIHighlightSystem m_UIHighlightSystem;

	private SignatureBuildingUISystem m_SignatureBuildingUISystem;

	private EntityQuery m_LockedQuery;

	private EntityArchetype m_UnlockEventArchetype;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_MilestoneSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<MilestoneSystem>();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier1>();
		m_UIHighlightSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UIHighlightSystem>();
		m_SignatureBuildingUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SignatureBuildingUISystem>();
		m_LockedQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Locked>(),
			ComponentType.Exclude<MilestoneData>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_UnlockEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Event>(),
			ComponentType.ReadWrite<Unlock>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_LockedQuery);
		((ComponentSystemBase)this).Enabled = false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		UnlockAllImpl();
		((ComponentSystemBase)this).Enabled = false;
	}

	private void UnlockAllImpl()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		EntityCommandBuffer val = m_ModificationBarrier.CreateCommandBuffer();
		NativeArray<Entity> val2 = ((EntityQuery)(ref m_LockedQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		for (int i = 0; i < val2.Length; i++)
		{
			Entity prefab = val2[i];
			Entity val3 = ((EntityCommandBuffer)(ref val)).CreateEntity(m_UnlockEventArchetype);
			((EntityCommandBuffer)(ref val)).SetComponent<Unlock>(val3, new Unlock(prefab));
		}
		val2.Dispose();
		m_MilestoneSystem.UnlockAllMilestones();
		m_UIHighlightSystem.SkipUpdate();
		m_SignatureBuildingUISystem.SkipUpdate();
	}

	[Preserve]
	public UnlockAllSystem()
	{
	}
}
