using Game.Buildings;
using Game.Common;
using Game.Events;
using Game.Objects;
using Game.Tools;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.Tutorials;

public class TutorialFireActivationSystem : GameSystemBase
{
	protected EntityCommandBufferSystem m_BarrierSystem;

	private EntityQuery m_BuildingFireQuery;

	private EntityQuery m_ForestFireQuery;

	private EntityQuery m_BuildingFireTutorialQuery;

	private EntityQuery m_ForestFireTutorialQuery;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_BarrierSystem = (EntityCommandBufferSystem)(object)((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier4>();
		m_BuildingFireQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<OnFire>(),
			ComponentType.ReadOnly<Building>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_ForestFireQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<OnFire>(),
			ComponentType.ReadOnly<Tree>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_BuildingFireTutorialQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<BuildingFireActivationData>(),
			ComponentType.Exclude<TutorialActivated>(),
			ComponentType.Exclude<TutorialCompleted>()
		});
		m_ForestFireTutorialQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<ForestFireActivationData>(),
			ComponentType.Exclude<TutorialActivated>(),
			ComponentType.Exclude<TutorialCompleted>()
		});
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		bool flag = !((EntityQuery)(ref m_ForestFireQuery)).IsEmptyIgnoreFilter && !((EntityQuery)(ref m_ForestFireTutorialQuery)).IsEmptyIgnoreFilter;
		bool flag2 = !((EntityQuery)(ref m_BuildingFireQuery)).IsEmptyIgnoreFilter && !((EntityQuery)(ref m_BuildingFireTutorialQuery)).IsEmptyIgnoreFilter;
		if (flag || flag2)
		{
			EntityCommandBuffer val = m_BarrierSystem.CreateCommandBuffer();
			if (flag)
			{
				((EntityCommandBuffer)(ref val)).AddComponent<TutorialActivated>(m_ForestFireTutorialQuery, (EntityQueryCaptureMode)1);
			}
			if (flag2)
			{
				((EntityCommandBuffer)(ref val)).AddComponent<TutorialActivated>(m_BuildingFireTutorialQuery, (EntityQueryCaptureMode)1);
			}
		}
	}

	[Preserve]
	public TutorialFireActivationSystem()
	{
	}
}
