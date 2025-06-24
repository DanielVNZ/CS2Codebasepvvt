using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Net;

[InternalBufferCapacity(0)]
public struct LaneObject : IBufferElementData, IEquatable<LaneObject>, IComparable<LaneObject>, IEmptySerializable
{
	public Entity m_LaneObject;

	public float2 m_CurvePosition;

	public LaneObject(Entity laneObject)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_LaneObject = laneObject;
		m_CurvePosition = default(float2);
	}

	public LaneObject(Entity laneObject, float2 curvePosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		m_LaneObject = laneObject;
		m_CurvePosition = curvePosition;
	}

	public bool Equals(LaneObject other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((Entity)(ref m_LaneObject)).Equals(other.m_LaneObject);
	}

	public int CompareTo(LaneObject other)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return (int)math.sign(m_CurvePosition.x - other.m_CurvePosition.x);
	}

	public override int GetHashCode()
	{
		return (17 * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_LaneObject)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<float2, float2>(ref m_CurvePosition)/*cast due to .constrained prefix*/).GetHashCode();
	}
}
