using System;
using System.Runtime.CompilerServices;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Net;

public struct LaneObjectAction : IComparable<LaneObjectAction>
{
	public Entity m_Lane;

	public Entity m_Remove;

	public Entity m_Add;

	public float2 m_CurvePosition;

	public LaneObjectAction(Entity lane, Entity remove)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		m_Lane = lane;
		m_Remove = remove;
		m_Add = Entity.Null;
		m_CurvePosition = default(float2);
	}

	public LaneObjectAction(Entity lane, Entity add, float2 curvePosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		m_Lane = lane;
		m_Remove = Entity.Null;
		m_Add = add;
		m_CurvePosition = curvePosition;
	}

	public LaneObjectAction(Entity lane, Entity remove, Entity add, float2 curvePosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		m_Lane = lane;
		m_Remove = remove;
		m_Add = add;
		m_CurvePosition = curvePosition;
	}

	public int CompareTo(LaneObjectAction other)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return m_Lane.Index - other.m_Lane.Index;
	}

	public override int GetHashCode()
	{
		return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Lane)/*cast due to .constrained prefix*/).GetHashCode();
	}
}
