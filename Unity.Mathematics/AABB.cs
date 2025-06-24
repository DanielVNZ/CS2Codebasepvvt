using System;

namespace Unity.Mathematics;

[Serializable]
public struct AABB
{
	public float3 Center;

	public float3 Extents;

	public float3 Size => Extents * 2f;

	public float3 Min => Center - Extents;

	public float3 Max => Center + Extents;

	public override string ToString()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		return $"AABB(Center:{Center}, Extents:{Extents}";
	}

	public bool Contains(float3 point)
	{
		if (((float3)(ref point))[0] < ((float3)(ref Center))[0] - ((float3)(ref Extents))[0])
		{
			return false;
		}
		if (((float3)(ref point))[0] > ((float3)(ref Center))[0] + ((float3)(ref Extents))[0])
		{
			return false;
		}
		if (((float3)(ref point))[1] < ((float3)(ref Center))[1] - ((float3)(ref Extents))[1])
		{
			return false;
		}
		if (((float3)(ref point))[1] > ((float3)(ref Center))[1] + ((float3)(ref Extents))[1])
		{
			return false;
		}
		if (((float3)(ref point))[2] < ((float3)(ref Center))[2] - ((float3)(ref Extents))[2])
		{
			return false;
		}
		if (((float3)(ref point))[2] > ((float3)(ref Center))[2] + ((float3)(ref Extents))[2])
		{
			return false;
		}
		return true;
	}

	public bool Contains(AABB b)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		if (Contains(b.Center + math.float3(0f - b.Extents.x, 0f - b.Extents.y, 0f - b.Extents.z)) && Contains(b.Center + math.float3(0f - b.Extents.x, 0f - b.Extents.y, b.Extents.z)) && Contains(b.Center + math.float3(0f - b.Extents.x, b.Extents.y, 0f - b.Extents.z)) && Contains(b.Center + math.float3(0f - b.Extents.x, b.Extents.y, b.Extents.z)) && Contains(b.Center + math.float3(b.Extents.x, 0f - b.Extents.y, 0f - b.Extents.z)) && Contains(b.Center + math.float3(b.Extents.x, 0f - b.Extents.y, b.Extents.z)) && Contains(b.Center + math.float3(b.Extents.x, b.Extents.y, 0f - b.Extents.z)))
		{
			return Contains(b.Center + math.float3(b.Extents.x, b.Extents.y, b.Extents.z));
		}
		return false;
	}

	private static float3 RotateExtents(float3 extents, float3 m0, float3 m1, float3 m2)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		return math.abs(m0 * extents.x) + math.abs(m1 * extents.y) + math.abs(m2 * extents.z);
	}

	public static AABB Transform(float4x4 transform, AABB localBounds)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		AABB result = default(AABB);
		result.Extents = RotateExtents(localBounds.Extents, ((float4)(ref transform.c0)).xyz, ((float4)(ref transform.c1)).xyz, ((float4)(ref transform.c2)).xyz);
		result.Center = math.transform(transform, localBounds.Center);
		return result;
	}

	public float DistanceSq(float3 point)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		return math.lengthsq(math.max(math.abs(point - Center), Extents) - Extents);
	}
}
