using Colossal.Mathematics;
using Game.Areas;
using Game.Net;
using Game.Notifications;
using Game.Prefabs;
using Game.Routes;
using Unity.Mathematics;

namespace Game.Common;

public struct RaycastInput
{
	public Segment m_Line;

	public float3 m_Offset;

	public TypeMask m_TypeMask;

	public RaycastFlags m_Flags;

	public CollisionMask m_CollisionMask;

	public Layer m_NetLayerMask;

	public AreaTypeMask m_AreaTypeMask;

	public RouteType m_RouteType;

	public TransportType m_TransportType;

	public IconLayerMask m_IconLayerMask;

	public UtilityTypes m_UtilityTypeMask;

	public bool IsDisabled()
	{
		return (m_Flags & (RaycastFlags.DebugDisable | RaycastFlags.UIDisable | RaycastFlags.ToolDisable | RaycastFlags.FreeCameraDisable)) != 0;
	}
}
