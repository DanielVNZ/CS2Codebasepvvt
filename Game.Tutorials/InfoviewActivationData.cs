using Unity.Entities;

namespace Game.Tutorials;

public struct InfoviewActivationData : IComponentData, IQueryTypeParameter
{
	public Entity m_Infoview;

	public InfoviewActivationData(Entity infoview)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Infoview = infoview;
	}
}
