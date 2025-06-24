using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.Tools;

public class ToolOutputSystem : GameSystemBase
{
	private ToolSystem m_ToolSystem;

	private UpdateSystem m_UpdateSystem;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_UpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UpdateSystem>();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		switch (m_ToolSystem.applyMode)
		{
		case ApplyMode.Clear:
			m_UpdateSystem.Update(SystemUpdatePhase.ClearTool);
			break;
		case ApplyMode.Apply:
			m_UpdateSystem.Update(SystemUpdatePhase.ApplyTool);
			break;
		}
	}

	[Preserve]
	public ToolOutputSystem()
	{
	}
}
