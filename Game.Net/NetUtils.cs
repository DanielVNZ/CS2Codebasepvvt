using System;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Buildings;
using Game.Common;
using Game.Pathfind;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Net;

public static class NetUtils
{
	public const float DEFAULT_ELEVATION_STEP = 10f;

	public const float MAX_LANE_WEAR = 10f;

	public const float MAX_LOCAL_CONNECT_DISTANCE = 4f;

	public const float MAX_LOCAL_CONNECT_HEIGHT = 1000f;

	public const float MAX_SNAP_HEIGHT = 50f;

	public const float UTURN_LIMIT_COS = 0.7547096f;

	public const float TURN_LIMIT_COS = -0.4848096f;

	public const float GENTLETURN_LIMIT_COS = -0.9335804f;

	public const float MAX_PASSING_CURVINESS_STREET = (float)Math.PI / 180f;

	public const float MAX_PASSING_CURVINESS_HIGHWAY = (float)Math.PI / 360f;

	public const float MIN_VISIBLE_EDGE_LENGTH = 0.1f;

	public const float MIN_VISIBLE_NODE_LENGTH = 0.05f;

	public const float MIN_VISIBLE_LANE_LENGTH = 0.1f;

	public static Bezier4x3 OffsetCurveLeftSmooth(Bezier4x3 curve, float2 offset)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		float3 val = MathUtils.StartTangent(curve);
		float3 val2 = MathUtils.Tangent(curve, 0.5f);
		float3 val3 = MathUtils.EndTangent(curve);
		val = MathUtils.Normalize(val, ((float3)(ref val)).xz);
		val2 = MathUtils.Normalize(val2, ((float3)(ref val2)).xz);
		val3 = MathUtils.Normalize(val3, ((float3)(ref val3)).xz);
		val.y = math.clamp(val.y, -1f, 1f);
		val3.y = math.clamp(val3.y, -1f, 1f);
		float3 a = curve.a;
		float3 middlePos = MathUtils.Position(curve, 0.5f);
		float3 d = curve.d;
		float4 val4 = default(float4);
		((float4)(ref val4))._002Ector(-offset, offset);
		((float3)(ref a)).xz = ((float3)(ref a)).xz + ((float3)(ref val)).zx * ((float4)(ref val4)).xz;
		((float3)(ref middlePos)).xz = ((float3)(ref middlePos)).xz + ((float3)(ref val2)).zx * (((float4)(ref val4)).xz + ((float4)(ref val4)).yw) * 0.5f;
		((float3)(ref d)).xz = ((float3)(ref d)).xz + ((float3)(ref val3)).zx * ((float4)(ref val4)).yw;
		return FitCurve(a, val, middlePos, val3, d);
	}

	public static Bezier4x3 CircleCurve(float3 center, float xOffset, float zOffset)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		Bezier4x3 result = default(Bezier4x3);
		((Bezier4x3)(ref result))._002Ector(center, center, center, center);
		result.a.x += xOffset;
		result.b.x += xOffset;
		result.b.z += zOffset * 0.5522848f;
		result.c.x += xOffset * 0.5522848f;
		result.c.z += zOffset;
		result.d.z += zOffset;
		return result;
	}

	public static Bezier4x3 CircleCurve(float3 center, quaternion rotation, float xOffset, float zOffset)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		float3 val = math.forward(rotation);
		float2 xz = ((float3)(ref val)).xz;
		float2 val2 = MathUtils.Right(xz);
		Bezier4x3 result = default(Bezier4x3);
		((Bezier4x3)(ref result))._002Ector(center, center, center, center);
		ref float3 a = ref result.a;
		((float3)(ref a)).xz = ((float3)(ref a)).xz + val2 * xOffset;
		ref float3 b = ref result.b;
		((float3)(ref b)).xz = ((float3)(ref b)).xz + val2 * xOffset;
		ref float3 b2 = ref result.b;
		((float3)(ref b2)).xz = ((float3)(ref b2)).xz + xz * (zOffset * 0.5522848f);
		ref float3 c = ref result.c;
		((float3)(ref c)).xz = ((float3)(ref c)).xz + val2 * (xOffset * 0.5522848f);
		ref float3 c2 = ref result.c;
		((float3)(ref c2)).xz = ((float3)(ref c2)).xz + xz * zOffset;
		ref float3 d = ref result.d;
		((float3)(ref d)).xz = ((float3)(ref d)).xz + xz * zOffset;
		return result;
	}

	public static Bezier4x3 FitCurve(float3 startPos, float3 startTangent, float3 middlePos, float3 endTangent, float3 endPos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		Bezier4x3 val = FitCurve(startPos, startTangent, endTangent, endPos);
		float3 val2 = middlePos - MathUtils.Position(val, 0.5f);
		float2 val3 = default(float2);
		((float2)(ref val3))._002Ector(math.dot(((float3)(ref startTangent)).xz, ((float3)(ref val2)).xz), math.dot(((float3)(ref endTangent)).xz, ((float3)(ref val2)).xz));
		float2 val4 = val3 * math.dot(((float3)(ref startTangent)).xz, ((float3)(ref endTangent)).xz);
		val3 *= math.abs(val3) / math.max(float2.op_Implicit(1E-06f), 0.375f * (math.abs(val3) + math.abs(((float2)(ref val4)).yx)));
		ref float3 b = ref val.b;
		b += startTangent * math.max(val3.x, math.min(0f, 1f - math.distance(((float3)(ref val.a)).xz, ((float3)(ref val.b)).xz)));
		ref float3 c = ref val.c;
		c += endTangent * math.min(val3.y, math.max(0f, math.distance(((float3)(ref val.d)).xz, ((float3)(ref val.c)).xz) - 1f));
		return val;
	}

	public static Bezier4x3 FitCurve(float3 startPos, float3 startTangent, float3 endTangent, float3 endPos)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		float num = math.distance(((float3)(ref startPos)).xz, ((float3)(ref endPos)).xz);
		Line3 val = default(Line3);
		((Line3)(ref val))._002Ector(startPos, startPos + startTangent);
		Line3 val2 = default(Line3);
		((Line3)(ref val2))._002Ector(endPos, endPos - endTangent);
		float2 val3 = ((!MathUtils.Intersect(((Line3)(ref val)).xz, ((Line3)(ref val2)).xz, ref val3)) ? float2.op_Implicit(num * 0.75f) : math.clamp(val3, float2.op_Implicit(num * 0.01f), float2.op_Implicit(num)));
		float num2 = math.dot(((float3)(ref startTangent)).xz, ((float3)(ref endTangent)).xz);
		if (num2 > 0f)
		{
			val3 = math.lerp(val3, float2.op_Implicit(num / math.sqrt(2f * num2 + 2f)), math.min(1f, num2 * num2));
		}
		else if (num2 < 0f)
		{
			val3 = math.lerp(val3, float2.op_Implicit(num * 1.2071068f), math.min(1f, num2 * num2));
		}
		return FitCurve(MathUtils.Cut(val, new float2(0f, val3.x)), MathUtils.Cut(val2, new float2(0f, val3.y)));
	}

	public static Bezier4x3 FitCurve(Segment startLine, Segment endLine)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		float3 val = MathUtils.Tangent(startLine);
		float3 val2 = MathUtils.Tangent(endLine);
		float num = math.length(((float3)(ref val)).xz);
		float num2 = math.length(((float3)(ref val2)).xz);
		if (num != 0f)
		{
			val /= num;
		}
		else
		{
			((float3)(ref val)).xz = ((float3)(ref endLine.b)).xz - ((float3)(ref startLine.a)).xz;
			num = math.length(((float3)(ref val)).xz);
			if (num != 0f)
			{
				val /= num;
			}
		}
		if (num2 != 0f)
		{
			val2 /= num2;
		}
		else
		{
			((float3)(ref val2)).xz = ((float3)(ref startLine.b)).xz - ((float3)(ref endLine.a)).xz;
			num2 = math.length(((float3)(ref val2)).xz);
			if (num2 != 0f)
			{
				val2 /= num2;
			}
		}
		val.y = math.clamp(val.y, -1f, 1f);
		val2.y = math.clamp(val2.y, -1f, 1f);
		float num3 = math.acos(math.saturate(0f - math.dot(((float3)(ref val)).xz, ((float3)(ref val2)).xz)));
		float num4 = math.tan(num3 / 2f);
		float num5 = (num + num2) * (1f / 6f);
		num5 = ((!(num4 >= 0.0001f)) ? (num5 * 2f) : (num5 * (4f * math.tan(num3 / 4f) / num4)));
		return new Bezier4x3
		{
			a = startLine.a,
			b = startLine.a + val * math.min(num, num5),
			c = endLine.a + val2 * math.min(num2, num5),
			d = endLine.a
		};
	}

	public static Bezier4x3 StraightCurve(float3 startPos, float3 endPos)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		return new Bezier4x3
		{
			a = startPos,
			b = math.lerp(startPos, endPos, 1f / 3f),
			c = math.lerp(startPos, endPos, 2f / 3f),
			d = endPos
		};
	}

	public static Bezier4x3 StraightCurve(float3 startPos, float3 endPos, float hanging)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		Bezier4x3 result = new Bezier4x3
		{
			a = startPos,
			b = math.lerp(startPos, endPos, 1f / 3f),
			c = math.lerp(startPos, endPos, 2f / 3f),
			d = endPos
		};
		float num = math.distance(((float3)(ref result.a)).xz, ((float3)(ref result.d)).xz) * hanging * 1.3333334f;
		result.b.y -= num;
		result.c.y -= num;
		return result;
	}

	public static float FindMiddleTangentPos(Bezier4x2 curve, float2 offset)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		float num = math.lerp(offset.x, offset.y, 0.5f);
		float num2 = num;
		float2 val = MathUtils.Tangent(curve, offset.x);
		float2 val2 = MathUtils.Tangent(curve, offset.y);
		if (!MathUtils.TryNormalize(ref val) || !MathUtils.TryNormalize(ref val2))
		{
			return num;
		}
		float2 val3 = offset;
		for (int i = 0; i < 24; i++)
		{
			float2 val4 = MathUtils.Tangent(curve, num2);
			if (!MathUtils.TryNormalize(ref val4))
			{
				break;
			}
			float num3 = math.distancesq(val, val4);
			float num4 = math.distancesq(val2, val4);
			if (num3 < num4)
			{
				val3.x = num2;
			}
			else
			{
				if (!(num3 > num4))
				{
					break;
				}
				val3.y = num2;
			}
			num2 = math.lerp(val3.x, val3.y, 0.5f);
		}
		return math.lerp(num2, num, math.saturate(0.5f + math.dot(val, val2) * 0.5f));
	}

	public static float CalculateCurviness(Curve curve, float width)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		if (curve.m_Length > 0.1f)
		{
			float3 tangent = MathUtils.StartTangent(curve.m_Bezier);
			float3 a = curve.m_Bezier.a;
			float3 val = MathUtils.Tangent(curve.m_Bezier, 0.25f);
			float3 val2 = MathUtils.Position(curve.m_Bezier, 0.25f);
			float3 val3 = MathUtils.Tangent(curve.m_Bezier, 0.5f);
			float3 val4 = MathUtils.Position(curve.m_Bezier, 0.5f);
			float3 val5 = MathUtils.Tangent(curve.m_Bezier, 0.75f);
			float3 val6 = MathUtils.Position(curve.m_Bezier, 0.75f);
			float3 tangent2 = MathUtils.EndTangent(curve.m_Bezier);
			float3 d = curve.m_Bezier.d;
			float4 val7 = default(float4);
			val7.x = CalculateCurviness(a, tangent, val2, val);
			val7.y = CalculateCurviness(val2, val, val4, val3);
			val7.z = CalculateCurviness(val4, val3, val6, val5);
			val7.w = CalculateCurviness(val6, val5, d, tangent2);
			float num = math.cmax(val7);
			if (curve.m_Length < width * 2f)
			{
				num = math.lerp(math.min(num, CalculateCurviness(a, tangent, d, tangent2)), num, math.smoothstep(width * 0.1f, width * 2f, curve.m_Length));
			}
			return num;
		}
		return 0f;
	}

	public static float CalculateStartCurviness(Curve curve, float width)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		if (curve.m_Length > 0.1f)
		{
			float3 tangent = MathUtils.StartTangent(curve.m_Bezier);
			float3 a = curve.m_Bezier.a;
			float3 val = MathUtils.Tangent(curve.m_Bezier, 0.25f);
			float3 val2 = MathUtils.Position(curve.m_Bezier, 0.25f);
			float3 tangent2 = MathUtils.Tangent(curve.m_Bezier, 0.5f);
			float3 position = MathUtils.Position(curve.m_Bezier, 0.5f);
			float2 val3 = default(float2);
			val3.x = CalculateCurviness(a, tangent, val2, val);
			val3.y = CalculateCurviness(val2, val, position, tangent2);
			return math.cmax(val3);
		}
		return 0f;
	}

	public static float CalculateEndCurviness(Curve curve, float width)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		if (curve.m_Length > 0.1f)
		{
			float3 tangent = MathUtils.Tangent(curve.m_Bezier, 0.5f);
			float3 position = MathUtils.Position(curve.m_Bezier, 0.5f);
			float3 val = MathUtils.Tangent(curve.m_Bezier, 0.75f);
			float3 val2 = MathUtils.Position(curve.m_Bezier, 0.75f);
			float3 tangent2 = MathUtils.EndTangent(curve.m_Bezier);
			float3 d = curve.m_Bezier.d;
			float2 val3 = default(float2);
			val3.x = CalculateCurviness(position, tangent, val2, val);
			val3.y = CalculateCurviness(val2, val, d, tangent2);
			return math.cmax(val3);
		}
		return 0f;
	}

	public static float CalculateCurviness(float3 position1, float3 tangent1, float3 position2, float3 tangent2)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		float num = math.distance(position1, position2);
		if (MathUtils.TryNormalize(ref tangent1) && MathUtils.TryNormalize(ref tangent2) && num >= 1E-06f)
		{
			return CalculateCurviness(tangent1, tangent2, num);
		}
		return 0f;
	}

	public static float CalculateCurviness(float3 tangent1, float3 tangent2, float distance)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		float num = math.acos(math.clamp(math.dot(tangent1, tangent2), -1f, 1f));
		return 2f * math.sin(num * 0.5f) / distance;
	}

	public static quaternion GetNodeRotation(float3 tangent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return GetNodeRotation(tangent, quaternion.identity);
	}

	public static quaternion GetNodeRotation(float3 tangent, quaternion defaultRotation)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		tangent.y = 0f;
		if (MathUtils.TryNormalize(ref tangent))
		{
			return quaternion.LookRotation(tangent, math.up());
		}
		return defaultRotation;
	}

	public static float ExtendedDistance(Bezier4x2 curve, float2 position, out float t)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		float num2 = default(float);
		float num = MathUtils.Distance(new Line2(curve.a, curve.a * 2f - curve.b), position, ref num2);
		float num4 = default(float);
		float num3 = MathUtils.Distance(curve, position, ref num4);
		float num6 = default(float);
		float num5 = MathUtils.Distance(new Line2(curve.d, curve.d * 2f - curve.c), position, ref num6);
		if (num2 >= 0f && num < num3 && (num < num5 || num6 < 0f))
		{
			t = 0f - num2;
			return num;
		}
		if (num6 >= 0f && num5 < num3)
		{
			t = 1f + num6;
			return num5;
		}
		t = num4;
		return num3;
	}

	public static float ExtendedLength(Bezier4x2 curve, float t)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (t <= 0f)
		{
			return math.distance(curve.a, curve.b) * t;
		}
		if (t <= 1f)
		{
			return MathUtils.Length(curve, new Bounds1(0f, t));
		}
		return MathUtils.Length(curve) + math.distance(curve.c, curve.d) * (t - 1f);
	}

	public static float ExtendedClampLength(Bezier4x2 curve, float distance)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (distance <= 0f)
		{
			float num = math.distance(curve.a, curve.b);
			return math.select(distance / num, 0f, num == 0f);
		}
		Bounds1 val = default(Bounds1);
		((Bounds1)(ref val))._002Ector(0f, 1f);
		if (MathUtils.ClampLength(curve, ref val, distance))
		{
			return val.max;
		}
		distance -= MathUtils.Length(curve);
		float num2 = math.distance(curve.c, curve.d);
		return math.select(1f + distance / num2, 1f, num2 == 0f);
	}

	public static void ExtendedPositionAndTangent(Bezier4x3 curve, float t, out float3 position, out float3 tangent)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (t <= 0f)
		{
			position = MathUtils.Position(new Line3(curve.a, curve.a * 2f - curve.b), 0f - t);
			tangent = curve.b - curve.a;
		}
		else if (t <= 1f)
		{
			position = MathUtils.Position(curve, t);
			tangent = MathUtils.Tangent(curve, t);
		}
		else
		{
			position = MathUtils.Position(new Line3(curve.d, curve.d * 2f - curve.c), t - 1f);
			tangent = curve.d - curve.c;
		}
	}

	public static int ChooseClosestLane(int minIndex, int maxIndex, float3 comparePosition, DynamicBuffer<SubLane> lanes, ComponentLookup<Curve> curves, float curvePosition)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		float num = float.MaxValue;
		int result = minIndex;
		maxIndex = math.min(maxIndex, lanes.Length - 1);
		float num3 = default(float);
		for (int i = minIndex; i <= maxIndex; i++)
		{
			Entity subLane = lanes[i].m_SubLane;
			float num2 = MathUtils.DistanceSquared(curves[subLane].m_Bezier, comparePosition, ref num3);
			if (num2 < num)
			{
				num = num2;
				result = i;
			}
		}
		return result;
	}

	public static float GetAvailability(DynamicBuffer<ResourceAvailability> availabilities, AvailableResource resource, float curvePos)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if ((int)resource >= availabilities.Length)
		{
			return 0f;
		}
		float2 availability = availabilities[(int)resource].m_Availability;
		return math.lerp(availability.x, availability.y, curvePos);
	}

	public static float GetServiceCoverage(DynamicBuffer<ServiceCoverage> coverages, CoverageService service, float curvePos)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		ServiceCoverage serviceCoverage = coverages[(int)service];
		return math.lerp(serviceCoverage.m_Coverage.x, serviceCoverage.m_Coverage.y, curvePos);
	}

	public static void AddLaneObject(DynamicBuffer<LaneObject> buffer, Entity laneObject, float2 curvePosition)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < buffer.Length; i++)
		{
			if (buffer[i].m_CurvePosition.y >= curvePosition.y)
			{
				buffer.Insert(i, new LaneObject(laneObject, curvePosition));
				return;
			}
		}
		buffer.Add(new LaneObject(laneObject, curvePosition));
	}

	public static void UpdateLaneObject(DynamicBuffer<LaneObject> buffer, Entity laneObject, float2 curvePosition)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		LaneObject laneObject2 = new LaneObject(laneObject, curvePosition);
		for (int i = 0; i < buffer.Length; i++)
		{
			LaneObject laneObject3 = buffer[i];
			if (laneObject3.m_LaneObject == laneObject)
			{
				for (int j = i + 1; j < buffer.Length; j++)
				{
					laneObject3 = buffer[j];
					if (laneObject3.m_CurvePosition.y >= curvePosition.y)
					{
						buffer[j - 1] = laneObject2;
						return;
					}
					buffer[j - 1] = laneObject3;
				}
				buffer[buffer.Length - 1] = laneObject2;
				return;
			}
			if (!(laneObject3.m_CurvePosition.y >= curvePosition.y))
			{
				continue;
			}
			buffer[i] = laneObject2;
			laneObject2 = laneObject3;
			for (int k = i + 1; k < buffer.Length; k++)
			{
				laneObject3 = buffer[k];
				buffer[k] = laneObject2;
				laneObject2 = laneObject3;
				if (laneObject2.m_LaneObject == laneObject)
				{
					return;
				}
			}
			break;
		}
		buffer.Add(laneObject2);
	}

	public static void RemoveLaneObject(DynamicBuffer<LaneObject> buffer, Entity laneObject)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		CollectionUtils.RemoveValue<LaneObject>(buffer, new LaneObject(laneObject));
	}

	public static bool CanConnect(NetData netData1, NetData netData2)
	{
		if ((netData1.m_RequiredLayers & netData2.m_ConnectLayers) != netData1.m_RequiredLayers)
		{
			return (netData2.m_RequiredLayers & netData1.m_ConnectLayers) == netData2.m_RequiredLayers;
		}
		return true;
	}

	public static bool FindConnectedLane(ref Entity laneEntity, ref bool forward, ref ComponentLookup<Lane> laneData, ref ComponentLookup<EdgeLane> edgeLaneData, ref ComponentLookup<Owner> ownerData, ref ComponentLookup<Edge> edgeData, ref BufferLookup<ConnectedEdge> connectedEdges, ref BufferLookup<SubLane> subLanes)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		Lane lane = laneData[laneEntity];
		Entity val = ownerData[laneEntity].m_Owner;
		Entity val2 = val;
		PathNode other = (forward ? lane.m_EndNode : lane.m_StartNode);
		if (edgeLaneData.HasComponent(laneEntity))
		{
			EdgeLane edgeLane = edgeLaneData[laneEntity];
			float num = (forward ? edgeLane.m_EdgeDelta.y : edgeLane.m_EdgeDelta.x);
			if (num == 0f)
			{
				val = edgeData[val].m_Start;
			}
			else if (num == 1f)
			{
				val = edgeData[val].m_End;
			}
			DynamicBuffer<SubLane> val3 = subLanes[val];
			for (int i = 0; i < val3.Length; i++)
			{
				Entity subLane = val3[i].m_SubLane;
				if (!(subLane == laneEntity))
				{
					Lane lane2 = laneData[subLane];
					if (lane2.m_StartNode.Equals(other))
					{
						laneEntity = subLane;
						forward = true;
						return true;
					}
					if (lane2.m_EndNode.Equals(other))
					{
						laneEntity = subLane;
						forward = false;
						return true;
					}
				}
			}
			if (val == val2 || !other.OwnerEquals(new PathNode(val, 0)))
			{
				return false;
			}
		}
		DynamicBuffer<ConnectedEdge> val4 = default(DynamicBuffer<ConnectedEdge>);
		if (connectedEdges.TryGetBuffer(val, ref val4))
		{
			for (int j = 0; j < val4.Length; j++)
			{
				val = val4[j].m_Edge;
				if (val == val2)
				{
					continue;
				}
				DynamicBuffer<SubLane> val5 = subLanes[val];
				for (int k = 0; k < val5.Length; k++)
				{
					Entity subLane2 = val5[k].m_SubLane;
					if (!(subLane2 == laneEntity))
					{
						Lane lane3 = laneData[subLane2];
						if (lane3.m_StartNode.Equals(other))
						{
							laneEntity = subLane2;
							forward = true;
							return true;
						}
						if (lane3.m_EndNode.Equals(other))
						{
							laneEntity = subLane2;
							forward = false;
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	public static CollisionMask GetCollisionMask(NetCompositionData compositionData, bool ignoreMarkers)
	{
		if ((compositionData.m_State & CompositionState.NoSubCollisions) != 0 && ignoreMarkers)
		{
			return (CollisionMask)0;
		}
		CollisionMask collisionMask = (CollisionMask)0;
		if ((compositionData.m_State & CompositionState.ExclusiveGround) != 0)
		{
			collisionMask |= CollisionMask.OnGround | CollisionMask.ExclusiveGround;
		}
		collisionMask = (((compositionData.m_Flags.m_General & CompositionFlags.General.Elevated) != 0) ? (collisionMask | CollisionMask.Overground) : (((compositionData.m_Flags.m_General & CompositionFlags.General.Tunnel) != 0) ? (collisionMask | CollisionMask.Underground) : (((compositionData.m_State & CompositionState.HasSurface) == 0) ? (collisionMask | CollisionMask.Overground) : (collisionMask | (CollisionMask.OnGround | CollisionMask.Overground)))));
		if (((compositionData.m_Flags.m_Left | compositionData.m_Flags.m_Right) & CompositionFlags.Side.Lowered) != 0)
		{
			collisionMask |= CollisionMask.Underground;
		}
		return collisionMask;
	}

	public static CollisionMask GetCollisionMask(LabelPosition labelPosition)
	{
		if (!labelPosition.m_IsUnderground)
		{
			return CollisionMask.Overground;
		}
		return CollisionMask.Underground;
	}

	public static bool IsTurn(float2 startPosition, float2 startDirection, float2 endPosition, float2 endDirection, out bool right, out bool gentle, out bool uturn)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		float2 val = MathUtils.Right(startDirection);
		float4 val2 = default(float4);
		val2.y = math.dot(startDirection, endDirection);
		val2.w = math.dot(val, endDirection);
		float num = math.distance(startPosition, endPosition);
		if (num > 0.1f)
		{
			float2 val3 = (startPosition - endPosition) / num;
			val2.x = math.dot(startDirection, val3);
			val2.z = math.dot(val, val3);
		}
		else
		{
			val2.x = -1f;
			val2.z = 0f;
		}
		val2 = math.lerp(val2, new float4(-1f, -1f, ((float4)(ref val2)).wz), math.saturate(new float4(val2.z * val2.w * new float2(-2f, -4f), ((float4)(ref val2)).xy)));
		val2 = math.select(val2, ((float4)(ref val2)).yxwz, val2.y > val2.x);
		right = val2.z < 0f;
		gentle = (val2.x > -0.9335804f) & (val2.x <= -0.4848096f);
		uturn = val2.x > 0.7547096f;
		return val2.x > -0.9335804f;
	}

	public static int GetConstructionCost(Curve curve, Elevation startElevation, Elevation endElevation, PlaceableNetComposition placeableNetData)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		int num = math.max(1, Mathf.RoundToInt(curve.m_Length / 8f));
		int num2 = math.max(0, Mathf.RoundToInt(math.max(math.cmin(startElevation.m_Elevation), math.cmin(endElevation.m_Elevation)) / 10f));
		return num * ((int)placeableNetData.m_ConstructionCost + num2 * (int)placeableNetData.m_ElevationCost);
	}

	public static int GetUpkeepCost(Curve curve, PlaceableNetComposition placeableNetData)
	{
		float num = math.max(1f, math.round(curve.m_Length / 8f));
		return math.max(1, Mathf.RoundToInt(num * placeableNetData.m_UpkeepCost));
	}

	public static int GetRefundAmount(Recent recent, uint simulationFrame, EconomyParameterData economyParameterData)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		if ((float)simulationFrame < (float)recent.m_ModificationFrame + 262144f * economyParameterData.m_RoadRefundTimeRange.x)
		{
			return (int)((float)recent.m_ModificationCost * economyParameterData.m_RoadRefundPercentage.x);
		}
		if ((float)simulationFrame < (float)recent.m_ModificationFrame + 262144f * economyParameterData.m_RoadRefundTimeRange.y)
		{
			return (int)((float)recent.m_ModificationCost * economyParameterData.m_RoadRefundPercentage.y);
		}
		if ((float)simulationFrame < (float)recent.m_ModificationFrame + 262144f * economyParameterData.m_RoadRefundTimeRange.z)
		{
			return (int)((float)recent.m_ModificationCost * economyParameterData.m_RoadRefundPercentage.z);
		}
		return 0;
	}

	public static int GetUpgradeCost(int newCost, int oldCost)
	{
		return math.max(0, newCost - oldCost);
	}

	public static int GetUpgradeCost(int newCost, int oldCost, Recent recent, uint simulationFrame, EconomyParameterData economyParameterData)
	{
		if (newCost >= oldCost)
		{
			return GetUpgradeCost(newCost, oldCost);
		}
		recent.m_ModificationCost = math.min(recent.m_ModificationCost, oldCost - newCost);
		return -GetRefundAmount(recent, simulationFrame, economyParameterData);
	}

	public static bool FindNextLane(ref Entity entity, ref ComponentLookup<Owner> ownerData, ref ComponentLookup<Lane> laneData, ref BufferLookup<SubLane> subLanes)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		Owner owner = default(Owner);
		Lane lane = default(Lane);
		if (!ownerData.TryGetComponent(entity, ref owner) || !laneData.TryGetComponent(entity, ref lane))
		{
			return false;
		}
		DynamicBuffer<SubLane> val = default(DynamicBuffer<SubLane>);
		if (!subLanes.TryGetBuffer(owner.m_Owner, ref val))
		{
			return false;
		}
		for (int i = 0; i < val.Length; i++)
		{
			Entity subLane = val[i].m_SubLane;
			Lane lane2 = laneData[subLane];
			if (lane.m_EndNode.Equals(lane2.m_StartNode))
			{
				entity = subLane;
				return true;
			}
		}
		return false;
	}

	public static bool FindPrevLane(ref Entity entity, ref ComponentLookup<Owner> ownerData, ref ComponentLookup<Lane> laneData, ref BufferLookup<SubLane> subLanes)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		Owner owner = default(Owner);
		Lane lane = default(Lane);
		if (!ownerData.TryGetComponent(entity, ref owner) || !laneData.TryGetComponent(entity, ref lane))
		{
			return false;
		}
		DynamicBuffer<SubLane> val = default(DynamicBuffer<SubLane>);
		if (!subLanes.TryGetBuffer(owner.m_Owner, ref val))
		{
			return false;
		}
		for (int i = 0; i < val.Length; i++)
		{
			Entity subLane = val[i].m_SubLane;
			Lane lane2 = laneData[subLane];
			if (lane.m_StartNode.Equals(lane2.m_EndNode))
			{
				entity = subLane;
				return true;
			}
		}
		return false;
	}

	public static bool FindEdgeLane(ref Entity entity, ref ComponentLookup<Owner> ownerData, ref ComponentLookup<Lane> laneData, ref BufferLookup<SubLane> subLanes, bool startNode)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		Owner owner = default(Owner);
		Lane lane = default(Lane);
		if (!ownerData.TryGetComponent(entity, ref owner) || !laneData.TryGetComponent(entity, ref lane))
		{
			return false;
		}
		DynamicBuffer<SubLane> val = default(DynamicBuffer<SubLane>);
		if (!subLanes.TryGetBuffer(owner.m_Owner, ref val))
		{
			return false;
		}
		PathNode pathNode = (startNode ? lane.m_StartNode : lane.m_EndNode);
		for (int i = 0; i < val.Length; i++)
		{
			Entity subLane = val[i].m_SubLane;
			if (pathNode.EqualsIgnoreCurvePos(laneData[subLane].m_MiddleNode))
			{
				entity = subLane;
				return true;
			}
		}
		return false;
	}

	public static float4 GetTrafficFlowSpeed(Road road)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		return GetTrafficFlowSpeed(road.m_TrafficFlowDuration0 + road.m_TrafficFlowDuration1, road.m_TrafficFlowDistance0 + road.m_TrafficFlowDistance1);
	}

	public static float4 GetTrafficFlowSpeed(float4 duration, float4 distance)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return math.saturate(distance / duration);
	}

	public static float GetTrafficFlowSpeed(float duration, float distance)
	{
		return math.saturate(distance / duration);
	}

	public static Node AdjustPosition(Node node, ref TerrainHeightData terrainHeightData)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Node result = node;
		result.m_Position.y = TerrainUtils.SampleHeight(ref terrainHeightData, node.m_Position);
		return result;
	}

	public static Node AdjustPosition(Node node, ref BuildingUtils.LotInfo lotInfo)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Node result = node;
		BuildingUtils.SampleHeight(ref lotInfo, node.m_Position);
		return result;
	}

	public static Curve AdjustPosition(Curve curve, bool fixedStart, bool linearMiddle, bool fixedEnd, ref TerrainHeightData terrainHeightData)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		Curve result = curve;
		if (!fixedStart)
		{
			result.m_Bezier.a.y = TerrainUtils.SampleHeight(ref terrainHeightData, curve.m_Bezier.a);
		}
		if (!fixedEnd)
		{
			result.m_Bezier.d.y = TerrainUtils.SampleHeight(ref terrainHeightData, curve.m_Bezier.d);
		}
		if (linearMiddle)
		{
			result.m_Bezier.b.y = math.lerp(result.m_Bezier.a.y, result.m_Bezier.d.y, 1f / 3f);
			result.m_Bezier.c.y = math.lerp(result.m_Bezier.a.y, result.m_Bezier.d.y, 2f / 3f);
		}
		else
		{
			result.m_Bezier.b.y = TerrainUtils.SampleHeight(ref terrainHeightData, curve.m_Bezier.b);
			result.m_Bezier.c.y = TerrainUtils.SampleHeight(ref terrainHeightData, curve.m_Bezier.c);
			float num = result.m_Bezier.b.y - MathUtils.Position(((Bezier4x3)(ref result.m_Bezier)).y, 1f / 3f);
			float num2 = result.m_Bezier.c.y - MathUtils.Position(((Bezier4x3)(ref result.m_Bezier)).y, 2f / 3f);
			result.m_Bezier.b.y += num * 3f - num2 * 1.5f;
			result.m_Bezier.c.y += num2 * 3f - num * 1.5f;
		}
		return result;
	}

	public static Curve AdjustPosition(Curve curve, bool fixedStart, bool linearMiddle, bool fixedEnd, ref TerrainHeightData terrainHeightData, ref WaterSurfaceData waterSurfaceData)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		Curve result = curve;
		if (!fixedStart)
		{
			result.m_Bezier.a.y = WaterUtils.SampleHeight(ref waterSurfaceData, ref terrainHeightData, curve.m_Bezier.a);
		}
		if (!fixedEnd)
		{
			result.m_Bezier.d.y = WaterUtils.SampleHeight(ref waterSurfaceData, ref terrainHeightData, curve.m_Bezier.d);
		}
		if (linearMiddle)
		{
			result.m_Bezier.b.y = math.lerp(result.m_Bezier.a.y, result.m_Bezier.d.y, 1f / 3f);
			result.m_Bezier.c.y = math.lerp(result.m_Bezier.a.y, result.m_Bezier.d.y, 2f / 3f);
		}
		else
		{
			result.m_Bezier.b.y = WaterUtils.SampleHeight(ref waterSurfaceData, ref terrainHeightData, curve.m_Bezier.b);
			result.m_Bezier.c.y = WaterUtils.SampleHeight(ref waterSurfaceData, ref terrainHeightData, curve.m_Bezier.c);
			float num = result.m_Bezier.b.y - MathUtils.Position(((Bezier4x3)(ref result.m_Bezier)).y, 1f / 3f);
			float num2 = result.m_Bezier.c.y - MathUtils.Position(((Bezier4x3)(ref result.m_Bezier)).y, 2f / 3f);
			result.m_Bezier.b.y += num * 3f - num2 * 1.5f;
			result.m_Bezier.c.y += num2 * 3f - num * 1.5f;
		}
		return result;
	}

	public static Curve AdjustPosition(Curve curve, bool2 fixedStart, bool linearMiddle, bool2 fixedEnd, ref BuildingUtils.LotInfo lotInfo)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		Curve result = curve;
		if (!fixedStart.x)
		{
			result.m_Bezier.a.y = BuildingUtils.SampleHeight(ref lotInfo, curve.m_Bezier.a);
		}
		if (!fixedEnd.x)
		{
			result.m_Bezier.d.y = BuildingUtils.SampleHeight(ref lotInfo, curve.m_Bezier.d);
		}
		if (linearMiddle)
		{
			if (!fixedStart.y)
			{
				result.m_Bezier.b.y = math.lerp(result.m_Bezier.a.y, result.m_Bezier.d.y, 1f / 3f);
			}
			if (!fixedEnd.y)
			{
				result.m_Bezier.c.y = math.lerp(result.m_Bezier.a.y, result.m_Bezier.d.y, 2f / 3f);
			}
		}
		else
		{
			if (!fixedStart.y)
			{
				result.m_Bezier.b.y = BuildingUtils.SampleHeight(ref lotInfo, curve.m_Bezier.b);
			}
			if (!fixedEnd.y)
			{
				result.m_Bezier.c.y = BuildingUtils.SampleHeight(ref lotInfo, curve.m_Bezier.c);
			}
			float num = result.m_Bezier.b.y - MathUtils.Position(((Bezier4x3)(ref result.m_Bezier)).y, 1f / 3f);
			float num2 = result.m_Bezier.c.y - MathUtils.Position(((Bezier4x3)(ref result.m_Bezier)).y, 2f / 3f);
			if (!fixedStart.y)
			{
				result.m_Bezier.b.y += num * 3f - num2 * 1.5f;
			}
			if (!fixedEnd.y)
			{
				result.m_Bezier.c.y += num2 * 3f - num * 1.5f;
			}
		}
		return result;
	}

	public static bool ShouldInvert(NetInvertMode invertMode, bool lefthandTraffic)
	{
		if (!(invertMode == NetInvertMode.LefthandTraffic && lefthandTraffic) && (invertMode != NetInvertMode.RighthandTraffic || lefthandTraffic))
		{
			return invertMode == NetInvertMode.Always;
		}
		return true;
	}

	public static Game.Prefabs.SubNet GetSubNet(DynamicBuffer<Game.Prefabs.SubNet> subNets, int index, bool lefthandTraffic, ref ComponentLookup<NetGeometryData> netGeometryLookup)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		Game.Prefabs.SubNet result = subNets[index];
		if (ShouldInvert(result.m_InvertMode, lefthandTraffic))
		{
			NetGeometryData netGeometryData = default(NetGeometryData);
			if (netGeometryLookup.TryGetComponent(result.m_Prefab, ref netGeometryData) && (netGeometryData.m_Flags & GeometryFlags.FlipTrafficHandedness) != 0)
			{
				result.m_Curve = MathUtils.Invert(result.m_Curve);
				result.m_NodeIndex = ((int2)(ref result.m_NodeIndex)).yx;
				result.m_ParentMesh = ((int2)(ref result.m_ParentMesh)).yx;
			}
			else
			{
				FlipUpgradeTrafficHandedness(ref result.m_Upgrades);
			}
		}
		return result;
	}

	public static void FlipUpgradeTrafficHandedness(ref CompositionFlags flags)
	{
		uint bitMask = (uint)flags.m_Left;
		uint bitMask2 = (uint)flags.m_Right;
		CommonUtils.SwapBits(ref bitMask, 16777216u, 33554432u);
		CommonUtils.SwapBits(ref bitMask2, 16777216u, 33554432u);
		flags.m_Left = (CompositionFlags.Side)bitMask2;
		flags.m_Right = (CompositionFlags.Side)bitMask;
	}

	public static float GetTerrainSmoothingWidth(NetData netData)
	{
		if ((netData.m_RequiredLayers & (Layer.Taxiway | Layer.MarkerTaxiway)) != Layer.None)
		{
			return 100f;
		}
		if ((netData.m_RequiredLayers & Layer.Waterway) != Layer.None)
		{
			return 20f;
		}
		return 8f;
	}

	public static int GetParkingSlotCount(Curve curve, ParkingLane parkingLane, ParkingLaneData prefabParkingLane)
	{
		return (int)math.floor((GetParkingSlotSpace(curve, parkingLane, prefabParkingLane) + 0.01f) / prefabParkingLane.m_SlotInterval);
	}

	public static float GetParkingSlotInterval(Curve curve, ParkingLane parkingLane, ParkingLaneData prefabParkingLane, int slotCount)
	{
		if (slotCount == 0 || (parkingLane.m_Flags & ParkingLaneFlags.FindConnections) != 0)
		{
			return prefabParkingLane.m_SlotInterval;
		}
		return GetParkingSlotSpace(curve, parkingLane, prefabParkingLane) / (float)slotCount;
	}

	private static float GetParkingSlotSpace(Curve curve, ParkingLane parkingLane, ParkingLaneData prefabParkingLane)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		float num = curve.m_Length;
		if ((parkingLane.m_Flags & ParkingLaneFlags.FindConnections) == 0)
		{
			num -= math.select(0f, 0.2f, (parkingLane.m_Flags & ParkingLaneFlags.StartingLane) != 0);
			num -= math.select(0f, 0.2f, (parkingLane.m_Flags & ParkingLaneFlags.EndingLane) != 0);
			if (prefabParkingLane.m_SlotAngle > 0.25f)
			{
				float2 val = default(float2);
				((float2)(ref val))._002Ector(math.cos(prefabParkingLane.m_SlotAngle), math.sin(prefabParkingLane.m_SlotAngle));
				float num2 = math.min(math.dot(prefabParkingLane.m_SlotSize, val), prefabParkingLane.m_SlotSize.y);
				switch (parkingLane.m_Flags & (ParkingLaneFlags.StartingLane | ParkingLaneFlags.EndingLane))
				{
				case ParkingLaneFlags.StartingLane:
				case ParkingLaneFlags.EndingLane:
					num -= num2 * 0.5f * math.tan((float)Math.PI / 2f - prefabParkingLane.m_SlotAngle);
					break;
				case ParkingLaneFlags.StartingLane | ParkingLaneFlags.EndingLane:
					num -= num2 * math.tan((float)Math.PI / 2f - prefabParkingLane.m_SlotAngle);
					break;
				}
			}
		}
		return num;
	}

	public static bool TryGetCombinedSegmentForLanes(EdgeGeometry edgeGeometry, NetGeometryData prefabGeometryData, out Segment segment)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		bool flag = (prefabGeometryData.m_Flags & (GeometryFlags.StraightEdges | GeometryFlags.SmoothSlopes)) == GeometryFlags.StraightEdges;
		segment = edgeGeometry.m_Start;
		segment.m_Left = MathUtils.Join(edgeGeometry.m_Start.m_Left, edgeGeometry.m_End.m_Left);
		segment.m_Right = MathUtils.Join(edgeGeometry.m_Start.m_Right, edgeGeometry.m_End.m_Right);
		segment.m_Length = edgeGeometry.m_Start.m_Length + edgeGeometry.m_End.m_Length;
		if (!flag && MathUtils.Length(MathUtils.Lerp(((Bezier4x3)(ref segment.m_Left)).xz, ((Bezier4x3)(ref segment.m_Right)).xz, 0.5f)) <= prefabGeometryData.m_EdgeLengthRange.max * 0.5f)
		{
			float3 val = default(float3);
			float num = default(float);
			val.x = MathUtils.Distance(segment.m_Left, MathUtils.Position(edgeGeometry.m_Start.m_Left, 0.5f), ref num);
			val.y = MathUtils.Distance(segment.m_Left, edgeGeometry.m_Start.m_Left.d, ref num);
			val.z = MathUtils.Distance(segment.m_Left, MathUtils.Position(edgeGeometry.m_End.m_Left, 0.5f), ref num);
			float3 val2 = default(float3);
			val2.x = MathUtils.Distance(segment.m_Right, MathUtils.Position(edgeGeometry.m_Start.m_Right, 0.5f), ref num);
			val2.y = MathUtils.Distance(segment.m_Right, edgeGeometry.m_Start.m_Right.d, ref num);
			val2.z = MathUtils.Distance(segment.m_Right, MathUtils.Position(edgeGeometry.m_End.m_Right, 0.5f), ref num);
			flag = math.all((val < 0.2f) & (val2 < 0.2f));
		}
		return flag;
	}
}
