using Game.Notifications;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Tools;

public struct IconDefinition : IComponentData, IQueryTypeParameter
{
	public float3 m_Location;

	public IconPriority m_Priority;

	public IconClusterLayer m_ClusterLayer;

	public IconFlags m_Flags;

	public IconDefinition(Icon icon)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		m_Location = icon.m_Location;
		m_Priority = icon.m_Priority;
		m_ClusterLayer = icon.m_ClusterLayer;
		m_Flags = icon.m_Flags;
	}
}
