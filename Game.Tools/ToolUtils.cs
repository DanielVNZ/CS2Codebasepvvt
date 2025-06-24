using Colossal.Mathematics;
using Game.Net;
using Unity.Collections;
using Unity.Mathematics;

namespace Game.Tools;

public static class ToolUtils
{
	public const float WATER_DEPTH_LIMIT = 0.2f;

	public const int MAX_ACTIVE_INFOMODES = 100;

	public const int INFOMODE_COLOR_GROUP_COUNT = 3;

	public const int INFOMODE_COLOR_GROUP_SIZE = 4;

	public const int INFOMODE_COLOR_GROUP_TERRAIN = 0;

	public const int INFOMODE_COLOR_GROUP_WATER = 1;

	public const int INFOMODE_COLOR_GROUP_OTHER = 2;

	public static quaternion CalculateRotation(float2 direction)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (((float2)(ref direction)).Equals(default(float2)))
		{
			return quaternion.identity;
		}
		return quaternion.LookRotation(new float3(direction.x, 0f, direction.y), math.up());
	}

	public static float2 CalculateSnapPriority(float level, float priority, float heightWeight, float3 origPos, float3 newPos, float2 direction)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		float3 val = newPos - origPos;
		((float3)(ref val))._002Ector(math.dot(((float3)(ref val)).xz, direction), val.y, math.dot(((float3)(ref val)).xz, MathUtils.Right(direction)));
		return CalculateSnapPriority(level, priority, heightWeight, val);
	}

	public static float2 CalculateSnapPriority(float level, float priority, float heightWeight, float3 offset)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		offset /= 8f;
		offset *= offset;
		float num = math.min(1f, offset.x + offset.z);
		float num2 = math.max(offset.x, offset.z) + math.min(offset.x, offset.z) * 0.001f;
		return new float2(level, priority * (2f - num - num2) / (1f + offset.y * heightWeight));
	}

	public static bool CompareSnapPriority(float2 priority, float2 other)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		bool2 val = priority > other;
		return val.x | (val.y & (priority.x == other.x));
	}

	public static void AddSnapLine(ref ControlPoint bestSnapPosition, NativeList<SnapLine> snapLines, SnapLine snapLine)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		Line2 val2 = default(Line2);
		float2 val3 = default(float2);
		for (int i = 0; i < snapLines.Length; i++)
		{
			SnapLine snapLine2 = snapLines[i];
			if (math.abs(math.dot(snapLine.m_ControlPoint.m_Direction, snapLine2.m_ControlPoint.m_Direction)) > 0.999999f)
			{
				continue;
			}
			Line2 val = new Line2(((float3)(ref snapLine.m_ControlPoint.m_Position)).xz, ((float3)(ref snapLine.m_ControlPoint.m_Position)).xz + snapLine.m_ControlPoint.m_Direction);
			((Line2)(ref val2))._002Ector(((float3)(ref snapLine2.m_ControlPoint.m_Position)).xz, ((float3)(ref snapLine2.m_ControlPoint.m_Position)).xz + snapLine2.m_ControlPoint.m_Direction);
			if (MathUtils.Intersect(val, val2, ref val3))
			{
				SnapLine snapLine3;
				if (snapLine.m_ControlPoint.m_SnapPriority.x >= snapLine2.m_ControlPoint.m_SnapPriority.x)
				{
					snapLine3 = snapLine;
					ref float3 position = ref snapLine3.m_ControlPoint.m_Position;
					((float3)(ref position)).xz = ((float3)(ref position)).xz + snapLine.m_ControlPoint.m_Direction * val3.x;
				}
				else
				{
					snapLine3 = snapLine2;
					ref float3 position2 = ref snapLine3.m_ControlPoint.m_Position;
					((float3)(ref position2)).xz = ((float3)(ref position2)).xz + snapLine2.m_ControlPoint.m_Direction * val3.y;
				}
				if (snapLine.m_HeightWeight != snapLine2.m_HeightWeight)
				{
					snapLine3.m_ControlPoint.m_Position.y = math.select(snapLine.m_ControlPoint.m_Position.y, snapLine2.m_ControlPoint.m_Position.y, snapLine2.m_HeightWeight > snapLine.m_HeightWeight);
				}
				if ((snapLine3.m_Flags & SnapLineFlags.ExtendedCurve) != 0)
				{
					NetUtils.ExtendedDistance(((Bezier4x3)(ref snapLine3.m_Curve)).xz, ((float3)(ref snapLine3.m_ControlPoint.m_Position)).xz, out var t);
					float num = NetUtils.ExtendedLength(((Bezier4x3)(ref snapLine3.m_Curve)).xz, t);
					num = MathUtils.Snap(num, 4f);
					snapLine3.m_ControlPoint.m_CurvePosition = NetUtils.ExtendedClampLength(((Bezier4x3)(ref snapLine3.m_Curve)).xz, num);
				}
				float level = math.max(snapLine.m_ControlPoint.m_SnapPriority.x, snapLine2.m_ControlPoint.m_SnapPriority.x);
				float heightWeight = math.max(snapLine.m_HeightWeight, snapLine2.m_HeightWeight);
				snapLine3.m_ControlPoint.m_SnapPriority = CalculateSnapPriority(level, 2f, heightWeight, snapLine3.m_ControlPoint.m_HitPosition, snapLine3.m_ControlPoint.m_Position, snapLine3.m_ControlPoint.m_Direction);
				AddSnapPosition(ref bestSnapPosition, snapLine3.m_ControlPoint);
			}
		}
		snapLines.Add(ref snapLine);
	}

	public static void AddSnapPosition(ref ControlPoint bestSnapPosition, ControlPoint snapPosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (CompareSnapPriority(snapPosition.m_SnapPriority, bestSnapPosition.m_SnapPriority))
		{
			bestSnapPosition = snapPosition;
		}
	}

	public static void DirectionSnap(ref float bestDirectionDistance, ref float3 resultPos, ref float3 resultDir, float3 refPos, float3 snapOrig, float3 snapDir, float snapDistance)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		float3 val = default(float3);
		float3 val2 = default(float3);
		((float3)(ref val)).xz = ((float3)(ref snapDir)).xz;
		((float3)(ref val2)).xz = MathUtils.Right(((float3)(ref snapDir)).xz);
		Line3 val3 = default(Line3);
		((Line3)(ref val3))._002Ector(snapOrig, snapOrig + val);
		Line3 val4 = default(Line3);
		((Line3)(ref val4))._002Ector(snapOrig, snapOrig + val2);
		float num2 = default(float);
		float num = MathUtils.Distance(((Line3)(ref val3)).xz, ((float3)(ref refPos)).xz, ref num2);
		float num4 = default(float);
		float num3 = MathUtils.Distance(((Line3)(ref val4)).xz, ((float3)(ref refPos)).xz, ref num4);
		if (num < bestDirectionDistance)
		{
			bestDirectionDistance = num;
			if (num < snapDistance)
			{
				resultDir = math.select(val, -val, num2 < 0f);
				((float3)(ref resultPos)).xz = MathUtils.Position(((Line3)(ref val3)).xz, num2);
			}
		}
		if (num3 < bestDirectionDistance)
		{
			bestDirectionDistance = num3;
			if (num3 < snapDistance)
			{
				resultDir = math.select(val2, -val2, num4 < 0f);
				((float3)(ref resultPos)).xz = MathUtils.Position(((Line3)(ref val4)).xz, num4);
			}
		}
	}

	public static Bounds2 GetBounds(Brush brush)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		quaternion val = quaternion.RotateY(brush.m_Angle);
		float3 val2 = math.mul(val, new float3(brush.m_Size * 0.5f, 0f, 0f));
		float2 xz = ((float3)(ref val2)).xz;
		val2 = math.mul(val, new float3(0f, 0f, brush.m_Size * 0.5f));
		float2 xz2 = ((float3)(ref val2)).xz;
		float2 val3 = math.abs(xz) + math.abs(xz2);
		return new Bounds2(((float3)(ref brush.m_Position)).xz - val3, ((float3)(ref brush.m_Position)).xz + val3);
	}

	public static float GetRandomAge(ref Random random, AgeMask ageMask)
	{
		int num = ((Random)(ref random)).NextInt(math.countbits((int)ageMask));
		for (int i = 0; i < 4; i++)
		{
			AgeMask ageMask2 = (AgeMask)(1 << i);
			if ((ageMask & ageMask2) != 0 && num-- == 0)
			{
				switch (ageMask2)
				{
				case AgeMask.Sapling:
					return 0f;
				case AgeMask.Young:
					return 0.17500001f;
				case AgeMask.Mature:
					return 0.425f;
				case AgeMask.Elderly:
					return 0.77500004f;
				}
			}
		}
		return 0f;
	}
}
