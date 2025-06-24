using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Routes;

public struct AccessLane : IComponentData, IQueryTypeParameter, IEquatable<AccessLane>, ISerializable
{
	public Entity m_Lane;

	public float m_CurvePos;

	public AccessLane(Entity lane, float curvePos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Lane = lane;
		m_CurvePos = curvePos;
	}

	public bool Equals(AccessLane other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (((Entity)(ref m_Lane)).Equals(other.m_Lane))
		{
			return m_CurvePos.Equals(other.m_CurvePos);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (17 * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Lane)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + m_CurvePos.GetHashCode();
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity lane = m_Lane;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(lane);
		float curvePos = m_CurvePos;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(curvePos);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity lane = ref m_Lane;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref lane);
		ref float curvePos = ref m_CurvePos;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref curvePos);
	}
}
