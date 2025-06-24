using Unity.Entities;

namespace Game.Tutorials;

public struct TutorialPhaseRef : IBufferElementData
{
	public Entity m_Phase;

	public TutorialPhaseRef(Entity phase)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Phase = phase;
	}
}
