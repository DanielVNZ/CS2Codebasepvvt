using Colossal.Entities;
using Game.Prefabs;
using Game.Rendering;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.Tools;

public class HeatmapPreviewSystem : GameSystemBase
{
	private TelecomPreviewSystem m_TelecomPreviewSystem;

	private EntityQuery m_InfomodeQuery;

	private ComponentSystemBase m_LastPreviewSystem;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_TelecomPreviewSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TelecomPreviewSystem>();
		m_InfomodeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<InfomodeActive>(),
			ComponentType.ReadOnly<InfoviewHeatmapData>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_InfomodeQuery);
	}

	[Preserve]
	protected override void OnStopRunning()
	{
		if (m_LastPreviewSystem != null)
		{
			m_LastPreviewSystem.Enabled = false;
			m_LastPreviewSystem.Update();
			m_LastPreviewSystem = null;
		}
		((COSystemBase)this).OnStopRunning();
	}

	private ComponentSystemBase GetPreviewSystem()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityQuery)(ref m_InfomodeQuery)).IsEmptyIgnoreFilter)
		{
			return null;
		}
		NativeArray<InfoviewHeatmapData> val = ((EntityQuery)(ref m_InfomodeQuery)).ToComponentDataArray<InfoviewHeatmapData>(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			for (int i = 0; i < val.Length; i++)
			{
				if (val[i].m_Type == HeatmapData.TelecomCoverage)
				{
					return (ComponentSystemBase)(object)m_TelecomPreviewSystem;
				}
			}
		}
		finally
		{
			val.Dispose();
		}
		return null;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		ComponentSystemBase previewSystem = GetPreviewSystem();
		if (previewSystem != m_LastPreviewSystem)
		{
			if (m_LastPreviewSystem != null)
			{
				m_LastPreviewSystem.Enabled = false;
				m_LastPreviewSystem.Update();
			}
			m_LastPreviewSystem = previewSystem;
			if (m_LastPreviewSystem != null)
			{
				m_LastPreviewSystem.Enabled = true;
			}
		}
		if (previewSystem != null)
		{
			previewSystem.Update();
		}
	}

	[Preserve]
	public HeatmapPreviewSystem()
	{
	}
}
