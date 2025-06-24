using Unity.Entities;

namespace Game.Tutorials;

public struct TutorialRef : IBufferElementData
{
	public Entity m_Tutorial;

	public TutorialRef(Entity tutorial)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Tutorial = tutorial;
	}
}
