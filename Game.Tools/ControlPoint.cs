using System;
using System.Runtime.CompilerServices;
using Game.Common;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Tools;

public struct ControlPoint : IEquatable<ControlPoint>
{
	public float3 m_Position;

	public float3 m_HitPosition;

	public float2 m_Direction;

	public float3 m_HitDirection;

	public quaternion m_Rotation;

	public Entity m_OriginalEntity;

	public float2 m_SnapPriority;

	public int2 m_ElementIndex;

	public float m_CurvePosition;

	public float m_Elevation;

	public ControlPoint(Entity raycastEntity, RaycastHit raycastHit)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		m_Position = raycastHit.m_Position;
		m_HitPosition = raycastHit.m_HitPosition;
		m_Direction = default(float2);
		m_HitDirection = raycastHit.m_HitDirection;
		m_Rotation = quaternion.identity;
		m_OriginalEntity = raycastEntity;
		m_SnapPriority = default(float2);
		m_ElementIndex = raycastHit.m_CellIndex;
		m_CurvePosition = raycastHit.m_CurvePosition;
		m_Elevation = 0f;
	}

	public bool Equals(ControlPoint other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		if (((float3)(ref m_Position)).Equals(other.m_Position) && ((float3)(ref m_HitPosition)).Equals(other.m_HitPosition) && ((float2)(ref m_Direction)).Equals(other.m_Direction) && ((float3)(ref m_HitDirection)).Equals(other.m_HitDirection) && ((quaternion)(ref m_Rotation)).Equals(other.m_Rotation) && ((Entity)(ref m_OriginalEntity)).Equals(other.m_OriginalEntity) && ((float2)(ref m_SnapPriority)).Equals(other.m_SnapPriority) && ((int2)(ref m_ElementIndex)).Equals(other.m_ElementIndex) && m_CurvePosition == other.m_CurvePosition)
		{
			return m_Elevation == other.m_Elevation;
		}
		return false;
	}

	public bool EqualsIgnoreHit(ControlPoint other)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		if (math.all(math.abs(m_Position - other.m_Position) < 0.001f) && math.all(math.abs(m_Direction - other.m_Direction) < 0.001f) && math.all(math.abs(m_Rotation.value - other.m_Rotation.value) < 0.001f) && math.abs(m_CurvePosition - other.m_CurvePosition) < 0.001f && math.abs(m_Elevation - other.m_Elevation) < 0.001f && ((Entity)(ref m_OriginalEntity)).Equals(other.m_OriginalEntity))
		{
			return ((int2)(ref m_ElementIndex)).Equals(other.m_ElementIndex);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (((((((((17 * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<float3, float3>(ref m_Position)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<float3, float3>(ref m_HitPosition)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<float2, float2>(ref m_Direction)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<float3, float3>(ref m_HitDirection)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<quaternion, quaternion>(ref m_Rotation)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_OriginalEntity)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<float2, float2>(ref m_SnapPriority)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<int2, int2>(ref m_ElementIndex)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + m_CurvePosition.GetHashCode()) * 31 + m_Elevation.GetHashCode();
	}
}
