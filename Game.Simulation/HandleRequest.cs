using Unity.Entities;

namespace Game.Simulation;

public struct HandleRequest : IComponentData, IQueryTypeParameter
{
	public Entity m_Request;

	public Entity m_Handler;

	public bool m_Completed;

	public bool m_PathConsumed;

	public HandleRequest(Entity request, Entity handler, bool completed, bool pathConsumed = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		m_Request = request;
		m_Handler = handler;
		m_Completed = completed;
		m_PathConsumed = pathConsumed;
	}
}
