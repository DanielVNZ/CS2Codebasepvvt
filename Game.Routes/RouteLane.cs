using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Routes;

public struct RouteLane : IComponentData, IQueryTypeParameter, IEquatable<RouteLane>, ISerializable
{
	public Entity m_StartLane;

	public Entity m_EndLane;

	public float m_StartCurvePos;

	public float m_EndCurvePos;

	public RouteLane(Entity startLane, Entity endLane, float startCurvePos, float endCurvePos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		m_StartLane = startLane;
		m_EndLane = endLane;
		m_StartCurvePos = startCurvePos;
		m_EndCurvePos = endCurvePos;
	}

	public bool Equals(RouteLane other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (((Entity)(ref m_StartLane)).Equals(other.m_StartLane) && ((Entity)(ref m_EndLane)).Equals(other.m_EndLane) && m_StartCurvePos.Equals(other.m_StartCurvePos))
		{
			return m_EndCurvePos.Equals(other.m_EndCurvePos);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (((17 * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_StartLane)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_EndLane)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + m_StartCurvePos.GetHashCode()) * 31 + m_EndCurvePos.GetHashCode();
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Entity startLane = m_StartLane;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(startLane);
		Entity endLane = m_EndLane;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(endLane);
		float startCurvePos = m_StartCurvePos;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(startCurvePos);
		float endCurvePos = m_EndCurvePos;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(endCurvePos);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity startLane = ref m_StartLane;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref startLane);
		ref Entity endLane = ref m_EndLane;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref endLane);
		ref float startCurvePos = ref m_StartCurvePos;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref startCurvePos);
		ref float endCurvePos = ref m_EndCurvePos;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref endCurvePos);
	}
}
