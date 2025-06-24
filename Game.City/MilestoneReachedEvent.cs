using Unity.Entities;

namespace Game.City;

public struct MilestoneReachedEvent : IComponentData, IQueryTypeParameter
{
	public Entity m_Milestone;

	public int m_Index;

	public MilestoneReachedEvent(Entity milestone, int index)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Milestone = milestone;
		m_Index = index;
	}
}
