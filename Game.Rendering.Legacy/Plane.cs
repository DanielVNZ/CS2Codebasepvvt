using Unity.Mathematics;

namespace Game.Rendering.Legacy;

public struct Plane
{
	private float3 m_Normal;

	private float m_Distance;

	public float3 normal
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_Normal;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_Normal = value;
		}
	}

	public float distance
	{
		get
		{
			return m_Distance;
		}
		set
		{
			m_Distance = value;
		}
	}

	public Plane flipped => new Plane(-m_Normal, 0f - m_Distance);

	public Plane(float3 inNormal, float3 inPoint)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		m_Normal = math.normalize(inNormal);
		m_Distance = 0f - math.dot(m_Normal, inPoint);
	}

	public Plane(float3 inNormal, float d)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		m_Normal = math.normalize(inNormal);
		m_Distance = d;
	}

	public Plane(float3 a, float3 b, float3 c)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		m_Normal = math.normalize(math.cross(b - a, c - a));
		m_Distance = 0f - math.dot(m_Normal, a);
	}

	public void SetNormalAndPosition(float3 inNormal, float3 inPoint)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		m_Normal = math.normalize(inNormal);
		m_Distance = 0f - math.dot(inNormal, inPoint);
	}

	public void Set3Points(float3 a, float3 b, float3 c)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		m_Normal = math.normalize(math.cross(b - a, c - a));
		m_Distance = 0f - math.dot(m_Normal, a);
	}

	public void Flip()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		m_Normal = -m_Normal;
		m_Distance = 0f - m_Distance;
	}

	public void Translate(float3 translation)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_Distance += math.dot(m_Normal, translation);
	}

	public static Plane Translate(Plane plane, float3 translation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		return new Plane(plane.m_Normal, plane.m_Distance += math.dot(plane.m_Normal, translation));
	}

	public float3 ClosestPointOnPlane(float3 point)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		float num = math.dot(m_Normal, point) + m_Distance;
		return point - m_Normal * num;
	}

	public float GetDistanceToPoint(float3 point)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return math.dot(m_Normal, point) + m_Distance;
	}

	public bool GetSide(float3 point)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return math.dot(m_Normal, point) + m_Distance > 0f;
	}

	public bool SameSide(float3 inPt0, float3 inPt1)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		float distanceToPoint = GetDistanceToPoint(inPt0);
		float distanceToPoint2 = GetDistanceToPoint(inPt1);
		if (!(distanceToPoint > 0f) || !(distanceToPoint2 > 0f))
		{
			if (distanceToPoint <= 0f)
			{
				return distanceToPoint2 <= 0f;
			}
			return false;
		}
		return true;
	}

	public override string ToString()
	{
		return $"(normal:({m_Normal.x:F1}, {m_Normal.y:F1}, {m_Normal.z:F1}), distance:{m_Distance:F1})";
	}
}
