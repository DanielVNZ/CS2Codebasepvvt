using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Events;

[InternalBufferCapacity(4)]
public struct HotspotFrame : IBufferElementData, IEmptySerializable
{
	public float3 m_Position;

	public float3 m_Velocity;

	public HotspotFrame(WeatherPhenomenon weatherPhenomenon)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		m_Position = weatherPhenomenon.m_HotspotPosition;
		m_Velocity = weatherPhenomenon.m_HotspotVelocity;
	}
}
