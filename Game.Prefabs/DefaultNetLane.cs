using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

[InternalBufferCapacity(0)]
public struct DefaultNetLane : IBufferElementData
{
	public Entity m_Lane;

	public float3 m_Position;

	public LaneFlags m_Flags;

	public byte m_Carriageway;

	public byte m_Group;

	public byte m_Index;

	public DefaultNetLane(NetCompositionLane source)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		m_Lane = source.m_Lane;
		m_Position = source.m_Position;
		m_Flags = source.m_Flags;
		m_Carriageway = source.m_Carriageway;
		m_Group = source.m_Group;
		m_Index = source.m_Index;
	}
}
