using Colossal.Collections;
using Colossal.Mathematics;
using Game.Buildings;
using Game.Common;
using Game.Economy;
using Game.Pathfind;
using Game.Prefabs;
using Game.Rendering;
using Game.Simulation;
using Game.Tools;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Objects;

public static class ObjectUtils
{
	public struct ActivityStartPositionCache
	{
		public ActivityType m_ActivityType;

		public float3 m_PositionOffset;

		public quaternion m_RotationOffset;
	}

	public const float MAX_SPAWN_LOCATION_CONNECTION_DISTANCE = 32f;

	public const float MIN_TREE_WOOD_RESOURCE = 1f;

	public const float MAX_TREE_AGE = 40f;

	public const float TREE_AGE_PHASE_CHILD = 0.1f;

	public const float TREE_AGE_PHASE_TEEN = 0.15f;

	public const float TREE_AGE_PHASE_ADULT = 0.35f;

	public const float TREE_AGE_PHASE_ELDERLY = 0.35f;

	public const float TREE_AGE_PHASE_DEAD = 0.05f;

	public const float TREE_WOOD_GROWTH_CHILD = 0.2f;

	public const float TREE_WOOD_GROWTH_TEEN = 0.5f;

	public const float TREE_WOOD_GROWTH_ADULT = 0.3f;

	public static float3 GetSize(Bounds3 bounds)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		float3 result = default(float3);
		float3 val = math.max(-bounds.min, bounds.max);
		((float3)(ref result)).xz = ((float3)(ref val)).xz * 2f;
		result.y = bounds.max.y;
		return result;
	}

	public static Bounds3 CalculateBounds(float3 position, quaternion rotation, ObjectGeometryData geometryData)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if ((geometryData.m_Flags & GeometryFlags.Circular) != GeometryFlags.None)
		{
			float num = geometryData.m_Size.x * 0.5f;
			return new Bounds3(position + new float3(0f - num, geometryData.m_Bounds.min.y, 0f - num), position + new float3(num, geometryData.m_Bounds.max.y, num));
		}
		return CalculateBounds(position, rotation, geometryData.m_Bounds);
	}

	public static Bounds3 GetBounds(ObjectGeometryData geometryData)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		Bounds3 bounds = geometryData.m_Bounds;
		if ((geometryData.m_Flags & GeometryFlags.Circular) != GeometryFlags.None)
		{
			((float3)(ref bounds.min)).xz = ((float3)(ref geometryData.m_Size)).xz * -0.5f;
			((float3)(ref bounds.max)).xz = ((float3)(ref geometryData.m_Size)).xz * 0.5f;
		}
		return bounds;
	}

	public static Bounds3 GetBounds(Stack stack, ObjectGeometryData geometryData, StackData stackData)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		Bounds3 bounds = GetBounds(geometryData);
		switch (stackData.m_Direction)
		{
		case StackDirection.Right:
			((Bounds3)(ref bounds)).x = stack.m_Range;
			break;
		case StackDirection.Up:
			((Bounds3)(ref bounds)).y = stack.m_Range;
			break;
		case StackDirection.Forward:
			((Bounds3)(ref bounds)).z = stack.m_Range;
			break;
		}
		return bounds;
	}

	public static Bounds3 CalculateBounds(float3 position, quaternion rotation, Stack stack, ObjectGeometryData geometryData, StackData stackData)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		Segment val = default(Segment);
		switch (stackData.m_Direction)
		{
		case StackDirection.Right:
			val.a = ObjectUtils.LocalToWorld(position, rotation, new float3(stack.m_Range.min, 0f, 0f));
			val.b = ObjectUtils.LocalToWorld(position, rotation, new float3(stack.m_Range.max, 0f, 0f));
			break;
		case StackDirection.Up:
			val.a = ObjectUtils.LocalToWorld(position, rotation, new float3(0f, stack.m_Range.min, 0f));
			val.b = ObjectUtils.LocalToWorld(position, rotation, new float3(0f, stack.m_Range.max, 0f));
			break;
		case StackDirection.Forward:
			val.a = ObjectUtils.LocalToWorld(position, rotation, new float3(0f, 0f, stack.m_Range.min));
			val.b = ObjectUtils.LocalToWorld(position, rotation, new float3(0f, 0f, stack.m_Range.max));
			break;
		default:
			return CalculateBounds(position, rotation, geometryData);
		}
		if ((geometryData.m_Flags & GeometryFlags.Circular) != GeometryFlags.None)
		{
			float num = geometryData.m_Size.x * 0.5f;
			return new Bounds3(MathUtils.Min(val) + new float3(0f - num, geometryData.m_Bounds.min.y, 0f - num), MathUtils.Max(val) + new float3(num, geometryData.m_Bounds.max.y, num));
		}
		return CalculateBounds(val, rotation, geometryData.m_Bounds);
	}

	public static Bounds3 CalculateBounds(float3 position, quaternion rotation, Bounds3 bounds)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		float3 val = math.mul(rotation, new float3(1f, 0f, 0f));
		float3 val2 = math.mul(rotation, new float3(0f, 1f, 0f));
		float3 val3 = math.mul(rotation, new float3(0f, 0f, 1f));
		float3 val4 = val * bounds.min.x;
		float3 val5 = val * bounds.max.x;
		float3 val6 = val2 * bounds.min.y;
		float3 val7 = val2 * bounds.max.y;
		float3 val8 = val3 * bounds.min.z;
		float3 val9 = val3 * bounds.max.z;
		return new Bounds3
		{
			min = position + math.min(val4, val5) + math.min(val6, val7) + math.min(val8, val9),
			max = position + math.max(val4, val5) + math.max(val6, val7) + math.max(val8, val9)
		};
	}

	public static Bounds3 CalculateBounds(Segment positionRange, quaternion rotation, Bounds3 bounds)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		float3 val = math.mul(rotation, new float3(1f, 0f, 0f));
		float3 val2 = math.mul(rotation, new float3(0f, 1f, 0f));
		float3 val3 = math.mul(rotation, new float3(0f, 0f, 1f));
		float3 val4 = val * bounds.min.x;
		float3 val5 = val * bounds.max.x;
		float3 val6 = val2 * bounds.min.y;
		float3 val7 = val2 * bounds.max.y;
		float3 val8 = val3 * bounds.min.z;
		float3 val9 = val3 * bounds.max.z;
		return new Bounds3
		{
			min = MathUtils.Min(positionRange) + math.min(val4, val5) + math.min(val6, val7) + math.min(val8, val9),
			max = MathUtils.Max(positionRange) + math.max(val4, val5) + math.max(val6, val7) + math.max(val8, val9)
		};
	}

	public static Quad3 CalculateBaseCorners(float3 position, quaternion rotation, Bounds3 bounds)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		float3 val = math.mul(rotation, new float3(0f, 0f, 1f));
		float3 val2 = math.mul(rotation, new float3(1f, 0f, 0f));
		float3 val3 = position + val * bounds.max.z;
		float3 val4 = position + val * bounds.min.z;
		float3 val5 = val2 * bounds.max.x;
		float3 val6 = val2 * bounds.min.x;
		return new Quad3(val3 + val6, val3 + val5, val4 + val5, val4 + val6);
	}

	public static Quad3 CalculateBaseCorners(float3 position, quaternion rotation, float2 size)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		size *= 0.5f;
		float3 val = math.mul(rotation, new float3(0f, 0f, 1f)) * size.y;
		float3 val2 = math.mul(rotation, new float3(1f, 0f, 0f)) * size.x;
		float3 val3 = position + val;
		float3 val4 = position - val;
		return new Quad3(val3 - val2, val3 + val2, val4 + val2, val4 - val2);
	}

	public static float3 CalculatePointVelocity(float3 offset, Moving moving)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return moving.m_Velocity + math.cross(moving.m_AngularVelocity, offset);
	}

	public static float3 CalculateMomentOfInertia(quaternion rotation, float3 size)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		size *= 0.5f;
		size *= size;
		float3 val = math.abs(math.rotate(rotation, new float3(size.x, 0f, 0f)));
		float3 val2 = math.abs(math.rotate(rotation, new float3(0f, size.y, 0f)));
		float3 val3 = math.abs(math.rotate(rotation, new float3(0f, 0f, size.z)));
		float3 val4 = val + val2 + val3;
		return ((float3)(ref val4)).yzx + ((float3)(ref val4)).zxy;
	}

	public static Transform InverseTransform(Transform transform)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		Transform result = default(Transform);
		result.m_Position = -transform.m_Position;
		result.m_Rotation = math.inverse(transform.m_Rotation);
		return result;
	}

	public static float3 LocalToWorld(Transform transform, float3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return transform.m_Position + math.mul(transform.m_Rotation, position);
	}

	public static float3 LocalToWorld(float3 transformPosition, quaternion transformRotation, float3 position)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return transformPosition + math.mul(transformRotation, position);
	}

	public static Bezier4x3 LocalToWorld(float3 transformPosition, quaternion transformRotation, Bezier4x3 curve)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		Bezier4x3 result = default(Bezier4x3);
		result.a = LocalToWorld(transformPosition, transformRotation, curve.a);
		result.b = LocalToWorld(transformPosition, transformRotation, curve.b);
		result.c = LocalToWorld(transformPosition, transformRotation, curve.c);
		result.d = LocalToWorld(transformPosition, transformRotation, curve.d);
		return result;
	}

	public static Transform LocalToWorld(Transform transform, float3 position, quaternion rotation)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		Transform result = default(Transform);
		result.m_Position = transform.m_Position + math.mul(transform.m_Rotation, position);
		result.m_Rotation = math.mul(transform.m_Rotation, rotation);
		return result;
	}

	public static InterpolatedTransform LocalToWorld(InterpolatedTransform transform, float3 position, quaternion rotation)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		InterpolatedTransform result = transform;
		result.m_Position = transform.m_Position + math.mul(transform.m_Rotation, position);
		result.m_Rotation = math.mul(transform.m_Rotation, rotation);
		return result;
	}

	public static Transform LocalToWorld(Transform parentTransform, Transform transform)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		Transform result = default(Transform);
		result.m_Position = parentTransform.m_Position + math.mul(parentTransform.m_Rotation, transform.m_Position);
		result.m_Rotation = math.mul(parentTransform.m_Rotation, transform.m_Rotation);
		return result;
	}

	public static Transform WorldToLocal(Transform inverseParentTransform, Transform transform)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		Transform result = default(Transform);
		result.m_Position = math.mul(inverseParentTransform.m_Rotation, transform.m_Position + inverseParentTransform.m_Position);
		result.m_Rotation = math.mul(inverseParentTransform.m_Rotation, transform.m_Rotation);
		return result;
	}

	public static float3 WorldToLocal(Transform inverseParentTransform, float3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return math.mul(inverseParentTransform.m_Rotation, position + inverseParentTransform.m_Position);
	}

	public static CollisionMask GetCollisionMask(ObjectGeometryData geometryData, Elevation elevation, bool ignoreMarkers)
	{
		if ((geometryData.m_Flags & GeometryFlags.Marker) != 0 && ignoreMarkers)
		{
			return (CollisionMask)0;
		}
		CollisionMask collisionMask = (CollisionMask)0;
		if ((geometryData.m_Flags & GeometryFlags.ExclusiveGround) != GeometryFlags.None)
		{
			collisionMask |= CollisionMask.OnGround | CollisionMask.ExclusiveGround;
		}
		if (elevation.m_Elevation < 0f)
		{
			collisionMask |= CollisionMask.Underground;
			if ((elevation.m_Flags & ElevationFlags.Lowered) != 0)
			{
				collisionMask |= CollisionMask.Overground;
			}
		}
		else
		{
			collisionMask |= CollisionMask.Overground;
		}
		return collisionMask;
	}

	public static CollisionMask GetCollisionMask(ObjectGeometryData geometryData, bool ignoreMarkers)
	{
		if ((geometryData.m_Flags & GeometryFlags.Marker) != 0 && ignoreMarkers)
		{
			return (CollisionMask)0;
		}
		CollisionMask collisionMask = (CollisionMask)0;
		if ((geometryData.m_Flags & (GeometryFlags.ExclusiveGround | GeometryFlags.BaseCollision)) != GeometryFlags.None)
		{
			collisionMask |= CollisionMask.ExclusiveGround;
		}
		return collisionMask | (CollisionMask.OnGround | CollisionMask.Overground);
	}

	public static int GetContructionCost(int constructionCost, Tree tree, in EconomyParameterData economyParameterData)
	{
		return (tree.m_State & (TreeState.Teen | TreeState.Adult | TreeState.Elderly | TreeState.Dead | TreeState.Stump)) switch
		{
			TreeState.Teen => constructionCost * economyParameterData.m_TreeCostMultipliers.x, 
			TreeState.Adult => constructionCost * economyParameterData.m_TreeCostMultipliers.y, 
			TreeState.Elderly => constructionCost * economyParameterData.m_TreeCostMultipliers.z, 
			_ => constructionCost, 
		};
	}

	public static int GetRelocationCost(int constructionCost, EconomyParameterData economyParameterData)
	{
		int num = (constructionCost + 1000) / 2000 * 500;
		num = (int)((float)math.select(num, 500, num == 0 && constructionCost > 0) * economyParameterData.m_RelocationCostMultiplier);
		return math.min(num, constructionCost);
	}

	public static int GetRelocationCost(int constructionCost, Recent recent, uint simulationFrame, EconomyParameterData economyParameterData)
	{
		int refundAmount = GetRefundAmount(recent, simulationFrame, economyParameterData);
		constructionCost = math.max(constructionCost / 4, constructionCost - refundAmount);
		return GetRelocationCost(constructionCost, economyParameterData);
	}

	public static int GetRebuildCost(int constructionCost)
	{
		int num = (constructionCost + 500) / 1000 * 500;
		num = math.select(num, 500, num == 0 && constructionCost > 0);
		return math.min(num, constructionCost);
	}

	public static int GetRebuildCost(int constructionCost, Recent recent, uint simulationFrame, EconomyParameterData economyParameterData)
	{
		int refundAmount = GetRefundAmount(recent, simulationFrame, economyParameterData);
		constructionCost = math.max(constructionCost / 4, constructionCost - refundAmount);
		return GetRebuildCost(constructionCost);
	}

	public static int GetUpgradeCost(int constructionCost, int originalCost)
	{
		return math.max(0, constructionCost - originalCost);
	}

	public static int GetUpgradeCost(int constructionCost, int originalCost, Recent recent, uint simulationFrame, EconomyParameterData economyParameterData)
	{
		if (constructionCost >= originalCost)
		{
			return GetUpgradeCost(constructionCost, originalCost);
		}
		recent.m_ModificationCost = math.min(recent.m_ModificationCost, originalCost - constructionCost);
		return -GetRefundAmount(recent, simulationFrame, economyParameterData);
	}

	public static int GetRefundAmount(Recent recent, uint simulationFrame, EconomyParameterData economyParameterData)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		if ((float)simulationFrame < (float)recent.m_ModificationFrame + 262144f * economyParameterData.m_BuildRefundTimeRange.x)
		{
			return (int)((float)recent.m_ModificationCost * economyParameterData.m_BuildRefundPercentage.x);
		}
		if ((float)simulationFrame < (float)recent.m_ModificationFrame + 262144f * economyParameterData.m_BuildRefundTimeRange.y)
		{
			return (int)((float)recent.m_ModificationCost * economyParameterData.m_BuildRefundPercentage.y);
		}
		if ((float)simulationFrame < (float)recent.m_ModificationFrame + 262144f * economyParameterData.m_BuildRefundTimeRange.z)
		{
			return (int)((float)recent.m_ModificationCost * economyParameterData.m_BuildRefundPercentage.z);
		}
		return 0;
	}

	public static float CalculateWoodAmount(Tree tree, Plant plant, Damaged damaged, TreeData treeData)
	{
		float num = 0f;
		switch (tree.m_State & (TreeState.Teen | TreeState.Adult | TreeState.Elderly | TreeState.Dead | TreeState.Stump))
		{
		case TreeState.Teen:
			num = math.lerp(0.2f, 0.7f, (float)(int)tree.m_Growth * 0.00390625f) * treeData.m_WoodAmount;
			break;
		case TreeState.Adult:
			num = math.lerp(0.7f, 1f, (float)(int)tree.m_Growth * 0.00390625f) * treeData.m_WoodAmount;
			break;
		case TreeState.Elderly:
			num = treeData.m_WoodAmount;
			break;
		case TreeState.Dead:
		case TreeState.Stump:
			return 0f;
		default:
			num = math.lerp(0f, 0.2f, (float)(int)tree.m_Growth * 0.00390625f) * treeData.m_WoodAmount;
			break;
		}
		return num * (1f - plant.m_Pollution) * (1f - GetTotalDamage(damaged));
	}

	public static float CalculateGrowthRate(Tree tree, Plant plant, TreeData treeData)
	{
		float num = 0f;
		switch (tree.m_State & (TreeState.Teen | TreeState.Adult | TreeState.Elderly | TreeState.Dead | TreeState.Stump))
		{
		case TreeState.Teen:
			num = 0.025f * treeData.m_WoodAmount;
			break;
		case TreeState.Adult:
			num = 3f / 140f * treeData.m_WoodAmount;
			break;
		case TreeState.Elderly:
		case TreeState.Dead:
		case TreeState.Stump:
			return 0f;
		default:
			num = 0.05f * treeData.m_WoodAmount;
			break;
		}
		return num * (1f - plant.m_Pollution);
	}

	public static Tree InitializeTreeState(float age)
	{
		Tree result = default(Tree);
		if (age < 0.1f)
		{
			result.m_Growth = (byte)math.clamp(Mathf.FloorToInt(age * 2560f), 0, 255);
		}
		else if (age < 0.25f)
		{
			result.m_State = TreeState.Teen;
			result.m_Growth = (byte)math.clamp(Mathf.FloorToInt((age - 0.1f) * 1706.6666f), 0, 255);
		}
		else if (age < 0.6f)
		{
			result.m_State = TreeState.Adult;
			result.m_Growth = (byte)math.clamp(Mathf.FloorToInt((age - 0.25f) * 731.4286f), 0, 255);
		}
		else if (age < 0.95000005f)
		{
			result.m_State = TreeState.Elderly;
			result.m_Growth = (byte)math.clamp(Mathf.FloorToInt((age - 0.6f) * 731.4286f), 0, 255);
		}
		else
		{
			result.m_State = TreeState.Dead;
			result.m_Growth = (byte)math.clamp(Mathf.FloorToInt((age - 0.95f) * 5120f), 0, 255);
		}
		return result;
	}

	public static void UpdateAnimation(Entity prefab, float timeStep, DynamicBuffer<MeshGroup> meshGroups, ref BufferLookup<SubMeshGroup> subMeshGroupBuffers, ref BufferLookup<CharacterElement> characterElementBuffers, ref BufferLookup<SubMesh> subMeshBuffers, ref BufferLookup<AnimationClip> animationClipBuffers, ref BufferLookup<AnimationMotion> animationMotionBuffers, AnimatedPropID oldPropID, AnimatedPropID newPropID, ActivityCondition conditions, ref float maxSpeed, ref byte activity, ref float3 targetPosition, ref float3 targetDirection, ref Transform transform, ref TransformFrame oldFrameData, ref TransformFrame newFrameData)
	{
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_065b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0668: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0571: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_0505: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Unknown result type (might be due to invalid IL or missing references)
		//IL_0519: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_0628: Unknown result type (might be due to invalid IL or missing references)
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		//IL_0613: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_054c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0551: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_072a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0735: Unknown result type (might be due to invalid IL or missing references)
		//IL_0646: Unknown result type (might be due to invalid IL or missing references)
		//IL_064c: Unknown result type (might be due to invalid IL or missing references)
		//IL_074d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0752: Unknown result type (might be due to invalid IL or missing references)
		//IL_0757: Unknown result type (might be due to invalid IL or missing references)
		//IL_075d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0766: Unknown result type (might be due to invalid IL or missing references)
		//IL_0776: Unknown result type (might be due to invalid IL or missing references)
		//IL_077d: Unknown result type (might be due to invalid IL or missing references)
		//IL_079e: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
		bool flag = newFrameData.m_Activity == 0;
		bool flag2 = oldFrameData.m_Activity == 0;
		if (oldFrameData.m_Activity != newFrameData.m_Activity)
		{
			byte parentActivity = GetParentActivity(oldFrameData.m_Activity);
			byte parentActivity2 = GetParentActivity(newFrameData.m_Activity);
			if (parentActivity != 0)
			{
				if (parentActivity != newFrameData.m_Activity)
				{
					newFrameData.m_Activity = parentActivity;
				}
				else
				{
					flag = true;
				}
			}
			else if (parentActivity2 != 0)
			{
				if (parentActivity2 != oldFrameData.m_Activity)
				{
					newFrameData.m_Activity = parentActivity2;
				}
				else
				{
					flag2 = true;
				}
			}
		}
		CharacterElement characterElement;
		AnimationClip animationClip;
		bool crossFade;
		float stateDuration;
		switch (oldFrameData.m_State)
		{
		case TransformState.Default:
			flag2 = true;
			break;
		case TransformState.Idle:
			if (newFrameData.m_State == TransformState.Idle && newFrameData.m_Activity == oldFrameData.m_Activity)
			{
				newFrameData.m_StateTimer = (ushort)(oldFrameData.m_StateTimer + 1);
				return;
			}
			stateDuration = GetStateDuration(prefab, TransformState.Idle, oldFrameData.m_Activity, oldPropID, conditions, meshGroups, ref subMeshGroupBuffers, ref characterElementBuffers, ref subMeshBuffers, ref animationClipBuffers, out characterElement, out animationClip, out crossFade);
			if (animationClip.m_Playback != AnimationPlayback.RandomLoop && stateDuration > 0f)
			{
				float2 val4 = default(float2);
				val4.x = (float)(int)oldFrameData.m_StateTimer * timeStep;
				val4.y = val4.x + timeStep;
				val4 = math.floor(val4 / math.select(stateDuration, stateDuration * 0.5f, animationClip.m_Playback == AnimationPlayback.HalfLoop));
				if (val4.x > val4.y - 0.5f)
				{
					newFrameData.m_State = TransformState.Idle;
					newFrameData.m_Activity = oldFrameData.m_Activity;
					newFrameData.m_StateTimer = (ushort)(oldFrameData.m_StateTimer + 1);
					maxSpeed = 0f;
					return;
				}
			}
			break;
		case TransformState.Move:
			if (newFrameData.m_State == TransformState.Move)
			{
				newFrameData.m_StateTimer = (ushort)(oldFrameData.m_StateTimer + 1);
				return;
			}
			break;
		case TransformState.Start:
		{
			stateDuration = GetStateDuration(prefab, TransformState.Start, oldFrameData.m_Activity, oldPropID, conditions, meshGroups, ref subMeshGroupBuffers, ref characterElementBuffers, ref subMeshBuffers, ref animationClipBuffers, out characterElement, out animationClip, out crossFade);
			float2 val3 = default(float2);
			val3.x = (float)(int)oldFrameData.m_StateTimer * timeStep;
			val3.y = val3.x + timeStep;
			val3 = math.min(val3, float2.op_Implicit(stateDuration));
			if (animationClip.m_MotionRange.y != animationClip.m_MotionRange.x && val3.y > val3.x && stateDuration > 0f)
			{
				DynamicBuffer<AnimationMotion> motions3 = animationMotionBuffers[characterElement.m_Style];
				ApplyRootMotion(ref transform, ref newFrameData, motions3, characterElement.m_ShapeWeights, animationClip.m_MotionRange, new float3(val3, 1f) / stateDuration);
				targetPosition = transform.m_Position;
				targetDirection = math.forward(transform.m_Rotation);
			}
			if (val3.y < stateDuration)
			{
				newFrameData.m_State = TransformState.Start;
				newFrameData.m_Activity = oldFrameData.m_Activity;
				newFrameData.m_StateTimer = (ushort)(oldFrameData.m_StateTimer + 1);
				maxSpeed = 0f;
				return;
			}
			if (animationClip.m_MotionRange.y == animationClip.m_MotionRange.x)
			{
				ApplyRootMotion(ref transform, ref targetPosition, ref targetDirection, animationClip.m_RootOffset, animationClip.m_RootRotation);
			}
			if (newFrameData.m_Activity == oldFrameData.m_Activity)
			{
				return;
			}
			break;
		}
		case TransformState.End:
		{
			stateDuration = GetStateDuration(prefab, TransformState.End, oldFrameData.m_Activity, oldPropID, conditions, meshGroups, ref subMeshGroupBuffers, ref characterElementBuffers, ref subMeshBuffers, ref animationClipBuffers, out characterElement, out animationClip, out crossFade);
			float2 val2 = default(float2);
			val2.x = (float)(int)oldFrameData.m_StateTimer * timeStep;
			val2.y = val2.x + timeStep;
			val2 = math.min(val2, float2.op_Implicit(stateDuration));
			if (animationClip.m_MotionRange.y != animationClip.m_MotionRange.x && val2.y > val2.x && stateDuration > 0f)
			{
				DynamicBuffer<AnimationMotion> motions2 = animationMotionBuffers[characterElement.m_Style];
				ApplyRootMotion(ref transform, ref newFrameData, motions2, characterElement.m_ShapeWeights, animationClip.m_MotionRange, new float3(val2, 1f) / stateDuration);
				targetPosition = transform.m_Position;
				targetDirection = math.forward(transform.m_Rotation);
			}
			if (val2.y < stateDuration)
			{
				newFrameData.m_State = TransformState.End;
				newFrameData.m_Activity = oldFrameData.m_Activity;
				newFrameData.m_StateTimer = (ushort)(oldFrameData.m_StateTimer + 1);
				maxSpeed = 0f;
				return;
			}
			if (animationClip.m_MotionRange.y == animationClip.m_MotionRange.x)
			{
				ApplyRootMotion(ref transform, ref targetPosition, ref targetDirection, animationClip.m_RootOffset, animationClip.m_RootRotation);
			}
			flag2 = true;
			break;
		}
		case TransformState.Action:
		case TransformState.Done:
		{
			stateDuration = GetStateDuration(prefab, TransformState.Action, oldFrameData.m_Activity, oldPropID, conditions, meshGroups, ref subMeshGroupBuffers, ref characterElementBuffers, ref subMeshBuffers, ref animationClipBuffers, out characterElement, out animationClip, out crossFade);
			float2 val = default(float2);
			val.x = (float)(int)oldFrameData.m_StateTimer * timeStep;
			val.y = val.x + timeStep;
			val = math.min(val, float2.op_Implicit(stateDuration));
			if (animationClip.m_MotionRange.y != animationClip.m_MotionRange.x && val.y > val.x && stateDuration > 0f)
			{
				DynamicBuffer<AnimationMotion> motions = animationMotionBuffers[characterElement.m_Style];
				ApplyRootMotion(ref transform, ref newFrameData, motions, characterElement.m_ShapeWeights, animationClip.m_MotionRange, new float3(val, 1f) / stateDuration);
				if (animationClip.m_Playback != AnimationPlayback.OptionalOnce)
				{
					targetPosition = transform.m_Position;
					targetDirection = math.forward(transform.m_Rotation);
				}
			}
			if (animationClip.m_Playback != AnimationPlayback.OptionalOnce || maxSpeed < 0.1f)
			{
				if (val.y < stateDuration)
				{
					newFrameData.m_State = TransformState.Action;
					newFrameData.m_Activity = oldFrameData.m_Activity;
					newFrameData.m_StateTimer = (ushort)(oldFrameData.m_StateTimer + 1);
					maxSpeed = 0f;
					return;
				}
				if (newFrameData.m_Activity == oldFrameData.m_Activity && newFrameData.m_Activity == 10)
				{
					newFrameData.m_State = TransformState.Done;
					newFrameData.m_Activity = oldFrameData.m_Activity;
					newFrameData.m_StateTimer = (ushort)math.min(65535, oldFrameData.m_StateTimer + 1);
					maxSpeed = 0f;
					return;
				}
			}
			if (newFrameData.m_Activity == oldFrameData.m_Activity)
			{
				targetDirection = default(float3);
				activity = 0;
				newFrameData.m_Activity = activity;
			}
			if (animationClip.m_MotionRange.y == animationClip.m_MotionRange.x)
			{
				ApplyRootMotion(ref transform, ref targetPosition, ref targetDirection, animationClip.m_RootOffset, animationClip.m_RootRotation);
			}
			flag2 = true;
			break;
		}
		}
		if (!flag2 && (stateDuration = GetStateDuration(prefab, TransformState.End, oldFrameData.m_Activity, oldPropID, conditions, meshGroups, ref subMeshGroupBuffers, ref characterElementBuffers, ref subMeshBuffers, ref animationClipBuffers, out characterElement, out animationClip, out var crossFade2)) > 0f)
		{
			newFrameData.m_State = TransformState.End;
			newFrameData.m_Activity = oldFrameData.m_Activity;
		}
		else if (!flag && (stateDuration = GetStateDuration(prefab, TransformState.Start, newFrameData.m_Activity, newPropID, conditions, meshGroups, ref subMeshGroupBuffers, ref characterElementBuffers, ref subMeshBuffers, ref animationClipBuffers, out characterElement, out animationClip, out crossFade2)) > 0f)
		{
			newFrameData.m_State = TransformState.Start;
		}
		else
		{
			if (!((stateDuration = GetStateDuration(prefab, TransformState.Action, newFrameData.m_Activity, newPropID, conditions, meshGroups, ref subMeshGroupBuffers, ref characterElementBuffers, ref subMeshBuffers, ref animationClipBuffers, out characterElement, out animationClip, out crossFade2)) > 0f))
			{
				return;
			}
			newFrameData.m_State = TransformState.Action;
		}
		newFrameData.m_StateTimer = (ushort)math.select(0, 1, crossFade2);
		maxSpeed = 0f;
		if (crossFade2 && animationClip.m_MotionRange.y != animationClip.m_MotionRange.x && stateDuration > 0f)
		{
			DynamicBuffer<AnimationMotion> motions4 = animationMotionBuffers[characterElement.m_Style];
			ApplyRootMotion(ref transform, ref newFrameData, motions4, characterElement.m_ShapeWeights, animationClip.m_MotionRange, new float3(0f, timeStep, 1f) / stateDuration);
			if (newFrameData.m_State != TransformState.Action || animationClip.m_Playback != AnimationPlayback.OptionalOnce)
			{
				targetPosition = transform.m_Position;
				targetDirection = math.forward(transform.m_Rotation);
			}
		}
	}

	public static Transform GetActivityStartPosition(Entity prefab, DynamicBuffer<MeshGroup> meshGroups, Transform activityTransform, TransformState state, ActivityType activityType, AnimatedPropID propID, ActivityCondition conditions, ref BufferLookup<SubMeshGroup> subMeshGroupBuffers, ref BufferLookup<CharacterElement> characterElementBuffers, ref BufferLookup<SubMesh> subMeshBuffers, ref BufferLookup<AnimationClip> animationClipBuffers, ref BufferLookup<AnimationMotion> animationMotionBuffers, ref ActivityStartPositionCache cache)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		if (activityType != cache.m_ActivityType)
		{
			cache.m_ActivityType = activityType;
			CharacterElement characterElement;
			AnimationClip animationClip;
			bool crossFade;
			float stateDuration = GetStateDuration(prefab, state, (byte)activityType, propID, conditions, meshGroups, ref subMeshGroupBuffers, ref characterElementBuffers, ref subMeshBuffers, ref animationClipBuffers, out characterElement, out animationClip, out crossFade);
			if (animationClip.m_MotionRange.y != animationClip.m_MotionRange.x && stateDuration > 0f)
			{
				DynamicBuffer<AnimationMotion> motions = animationMotionBuffers[characterElement.m_Style];
				GetRootMotion(motions, animationClip.m_MotionRange, characterElement.m_ShapeWeights, 0f, out var rootOffset, out var rootVelocity, out var rootRotation);
				GetRootMotion(motions, animationClip.m_MotionRange, characterElement.m_ShapeWeights, 1f, out var rootOffset2, out rootVelocity, out var rootRotation2);
				cache.m_RotationOffset = math.inverse(rootRotation2);
				cache.m_PositionOffset = math.mul(cache.m_RotationOffset, rootOffset - rootOffset2);
				cache.m_RotationOffset = math.mul(cache.m_RotationOffset, rootRotation);
			}
			else
			{
				cache.m_PositionOffset = default(float3);
				cache.m_RotationOffset = quaternion.identity;
			}
		}
		if (cache.m_ActivityType != ActivityType.None)
		{
			return LocalToWorld(activityTransform, cache.m_PositionOffset, cache.m_RotationOffset);
		}
		return activityTransform;
	}

	private static byte GetParentActivity(byte activity)
	{
		if (activity == 6)
		{
			return 5;
		}
		return 0;
	}

	private static void ApplyRootMotion(ref Transform transform, ref TransformFrame newFrameData, DynamicBuffer<AnimationMotion> motions, BlendWeights weights, int2 motionRange, float3 deltaRange)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		GetRootMotion(motions, motionRange, weights, deltaRange.x, out var rootOffset, out var _, out var rootRotation);
		GetRootMotion(motions, motionRange, weights, deltaRange.y, out var rootOffset2, out var rootVelocity2, out var rootRotation2);
		transform.m_Rotation = math.mul(transform.m_Rotation, math.inverse(rootRotation));
		ref float3 position = ref transform.m_Position;
		position += math.mul(transform.m_Rotation, rootOffset2 - rootOffset);
		ref float3 velocity = ref newFrameData.m_Velocity;
		velocity += math.mul(transform.m_Rotation, rootVelocity2 * deltaRange.z);
		transform.m_Rotation = math.normalize(math.mul(transform.m_Rotation, rootRotation2));
	}

	private static void ApplyRootMotion(ref Transform transform, ref float3 targetPosition, ref float3 targetDirection, float3 rootOffset, quaternion rootRotation)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if (!((float3)(ref rootOffset)).Equals(default(float3)))
		{
			rootOffset = math.mul(transform.m_Rotation, rootOffset);
			ref float3 position = ref transform.m_Position;
			position += rootOffset;
			targetPosition = transform.m_Position;
		}
		if (!((quaternion)(ref rootRotation)).Equals(default(quaternion)))
		{
			transform.m_Rotation = math.mul(transform.m_Rotation, rootRotation);
			targetDirection = math.forward(transform.m_Rotation);
		}
	}

	private static float GetStateDuration(Entity prefab, TransformState state, byte activity, AnimatedPropID propID, ActivityCondition conditions, DynamicBuffer<MeshGroup> meshGroups, ref BufferLookup<SubMeshGroup> subMeshGroupBuffers, ref BufferLookup<CharacterElement> characterElementBuffers, ref BufferLookup<SubMesh> subMeshBuffers, ref BufferLookup<AnimationClip> animationClipBuffers, out CharacterElement characterElement, out AnimationClip animationClip, out bool crossFade)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		characterElement = default(CharacterElement);
		animationClip = default(AnimationClip);
		crossFade = false;
		AnimationType animationType;
		switch (state)
		{
		case TransformState.Idle:
			animationType = AnimationType.Idle;
			break;
		case TransformState.Start:
			animationType = AnimationType.Start;
			break;
		case TransformState.End:
			animationType = AnimationType.End;
			break;
		case TransformState.Action:
			animationType = AnimationType.Action;
			break;
		case TransformState.Done:
			animationType = AnimationType.Action;
			break;
		default:
			return 0f;
		}
		float num = 0f;
		int num2 = 0;
		DynamicBuffer<CharacterElement> val = default(DynamicBuffer<CharacterElement>);
		DynamicBuffer<SubMesh> val2 = default(DynamicBuffer<SubMesh>);
		DynamicBuffer<SubMeshGroup> val3 = default(DynamicBuffer<SubMeshGroup>);
		if (subMeshGroupBuffers.TryGetBuffer(prefab, ref val3))
		{
			if (meshGroups.IsCreated)
			{
				num2 = meshGroups.Length;
			}
			crossFade = characterElementBuffers.TryGetBuffer(prefab, ref val);
		}
		else
		{
			val2 = subMeshBuffers[prefab];
			num2 = val2.Length;
		}
		MeshGroup meshGroup = default(MeshGroup);
		MeshGroup meshGroup2 = default(MeshGroup);
		DynamicBuffer<AnimationClip> val4 = default(DynamicBuffer<AnimationClip>);
		for (int i = 0; i < num2; i++)
		{
			CharacterElement characterElement2 = default(CharacterElement);
			if (val.IsCreated)
			{
				CollectionUtils.TryGet<MeshGroup>(meshGroups, i, ref meshGroup);
				characterElement2 = val[(int)meshGroup.m_SubMeshGroup];
			}
			else
			{
				int num3 = i;
				if (val3.IsCreated)
				{
					CollectionUtils.TryGet<MeshGroup>(meshGroups, i, ref meshGroup2);
					num3 = val3[(int)meshGroup2.m_SubMeshGroup].m_SubMeshRange.x;
				}
				characterElement2.m_Style = val2[num3].m_SubMesh;
			}
			if (!animationClipBuffers.TryGetBuffer(characterElement2.m_Style, ref val4))
			{
				continue;
			}
			int num4 = int.MaxValue;
			float num5 = 0f;
			for (int j = 0; j < val4.Length; j++)
			{
				AnimationClip animationClip2 = val4[j];
				if (animationClip2.m_Type == animationType && animationClip2.m_Activity == (ActivityType)activity && animationClip2.m_Layer == AnimationLayer.Body && animationClip2.m_PropID == propID)
				{
					ActivityCondition activityCondition = animationClip2.m_Conditions ^ conditions;
					if (activityCondition == (ActivityCondition)0u)
					{
						num5 = animationClip2.m_AnimationLength;
						characterElement = characterElement2;
						animationClip = animationClip2;
						break;
					}
					int num6 = math.countbits((uint)activityCondition);
					if (num6 < num4)
					{
						num4 = num6;
						num5 = animationClip2.m_AnimationLength;
						characterElement = characterElement2;
						animationClip = animationClip2;
					}
				}
			}
			num = math.max(num, num5);
		}
		return num;
	}

	public static void GetRootMotion(DynamicBuffer<AnimationMotion> motions, int2 range, BlendWeights weights, float t, out float3 rootOffset, out float3 rootVelocity, out quaternion rootRotation)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (range.y == range.x + 1)
		{
			GetRootMotion(motions[range.x], t, out rootOffset, out rootVelocity, out rootRotation);
			return;
		}
		GetRootMotion(motions[range.x], t, out var rootOffset2, out var rootVelocity2, out var rootRotation2);
		GetRootMotion(motions[range.x + weights.m_Weight0.m_Index + 1], t, out var rootOffset3, out var rootVelocity3, out var rootRotation3);
		GetRootMotion(motions[range.x + weights.m_Weight1.m_Index + 1], t, out var rootOffset4, out var rootVelocity4, out var rootRotation4);
		GetRootMotion(motions[range.x + weights.m_Weight2.m_Index + 1], t, out var rootOffset5, out var rootVelocity5, out var rootRotation5);
		GetRootMotion(motions[range.x + weights.m_Weight3.m_Index + 1], t, out var rootOffset6, out var rootVelocity6, out var rootRotation6);
		GetRootMotion(motions[range.x + weights.m_Weight4.m_Index + 1], t, out var rootOffset7, out var rootVelocity7, out var rootRotation7);
		GetRootMotion(motions[range.x + weights.m_Weight5.m_Index + 1], t, out var rootOffset8, out var rootVelocity8, out var rootRotation8);
		GetRootMotion(motions[range.x + weights.m_Weight6.m_Index + 1], t, out var rootOffset9, out var rootVelocity9, out var rootRotation9);
		GetRootMotion(motions[range.x + weights.m_Weight7.m_Index + 1], t, out var rootOffset10, out var rootVelocity10, out var rootRotation10);
		rootOffset3 *= weights.m_Weight0.m_Weight;
		rootOffset4 *= weights.m_Weight1.m_Weight;
		rootOffset5 *= weights.m_Weight2.m_Weight;
		rootOffset6 *= weights.m_Weight3.m_Weight;
		rootOffset7 *= weights.m_Weight4.m_Weight;
		rootOffset8 *= weights.m_Weight5.m_Weight;
		rootOffset9 *= weights.m_Weight6.m_Weight;
		rootOffset10 *= weights.m_Weight7.m_Weight;
		rootVelocity3 *= weights.m_Weight0.m_Weight;
		rootVelocity4 *= weights.m_Weight1.m_Weight;
		rootVelocity5 *= weights.m_Weight2.m_Weight;
		rootVelocity6 *= weights.m_Weight3.m_Weight;
		rootVelocity7 *= weights.m_Weight4.m_Weight;
		rootVelocity8 *= weights.m_Weight5.m_Weight;
		rootVelocity9 *= weights.m_Weight6.m_Weight;
		rootVelocity10 *= weights.m_Weight7.m_Weight;
		rootOffset = rootOffset2 + rootOffset3 + rootOffset4 + rootOffset5 + rootOffset6 + rootOffset7 + rootOffset8 + rootOffset9 + rootOffset10;
		rootVelocity = rootVelocity2 + rootVelocity3 + rootVelocity4 + rootVelocity5 + rootVelocity6 + rootVelocity7 + rootVelocity8 + rootVelocity9 + rootVelocity10;
		rootRotation3 = math.slerp(quaternion.identity, rootRotation3, weights.m_Weight0.m_Weight);
		rootRotation4 = math.slerp(quaternion.identity, rootRotation4, weights.m_Weight1.m_Weight);
		rootRotation5 = math.slerp(quaternion.identity, rootRotation5, weights.m_Weight2.m_Weight);
		rootRotation6 = math.slerp(quaternion.identity, rootRotation6, weights.m_Weight3.m_Weight);
		rootRotation7 = math.slerp(quaternion.identity, rootRotation7, weights.m_Weight4.m_Weight);
		rootRotation8 = math.slerp(quaternion.identity, rootRotation8, weights.m_Weight5.m_Weight);
		rootRotation9 = math.slerp(quaternion.identity, rootRotation9, weights.m_Weight6.m_Weight);
		rootRotation10 = math.slerp(quaternion.identity, rootRotation10, weights.m_Weight7.m_Weight);
		rootRotation = math.mul(rootRotation10, math.mul(rootRotation9, math.mul(rootRotation8, math.mul(rootRotation7, math.mul(rootRotation6, math.mul(rootRotation5, math.mul(rootRotation4, math.mul(rootRotation3, rootRotation2))))))));
	}

	private static void GetRootMotion(AnimationMotion motion, float t, out float3 rootOffset, out float3 rootVelocity, out quaternion rootRotation)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		Bezier4x3 val = default(Bezier4x3);
		((Bezier4x3)(ref val))._002Ector(motion.m_StartOffset, motion.m_StartOffset, motion.m_EndOffset, motion.m_EndOffset);
		rootOffset = MathUtils.Position(val, t);
		rootVelocity = MathUtils.Tangent(val, t);
		rootRotation = math.slerp(motion.m_StartRotation, motion.m_EndRotation, t);
	}

	public static float GetTotalDamage(Damaged damaged)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		float3 damage = damaged.m_Damage;
		damage.z = math.max(0f, damage.z - math.min(0.5f, math.csum(((float3)(ref damage)).xy)));
		return math.min(1f, math.csum(damage));
	}

	public static void UpdateResourcesDamage(Entity entity, float totalDamage, ref BufferLookup<Renter> renterData, ref BufferLookup<Resources> resourcesData)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<Renter> val = default(DynamicBuffer<Renter>);
		if (!renterData.TryGetBuffer(entity, ref val))
		{
			return;
		}
		DynamicBuffer<Resources> val2 = default(DynamicBuffer<Resources>);
		for (int i = 0; i < val.Length; i++)
		{
			if (!resourcesData.TryGetBuffer(val[i].m_Renter, ref val2))
			{
				continue;
			}
			for (int j = 0; j < val2.Length; j++)
			{
				Resources resources = val2[j];
				if (resources.m_Resource != Resource.Money)
				{
					resources.m_Amount = (int)((float)resources.m_Amount * (1f - totalDamage));
				}
				val2[j] = resources;
			}
		}
	}

	public static Transform AdjustPosition(Transform transform, ref Elevation elevation, Entity prefab, out bool angledSample, ref TerrainHeightData terrainHeightData, ref WaterSurfaceData waterSurfaceData, ref ComponentLookup<PlaceableObjectData> placeableObjectDatas, ref ComponentLookup<ObjectGeometryData> objectGeometryDatas)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		Transform result = transform;
		float num = 0f;
		float num2 = 0f;
		angledSample = true;
		PlaceableObjectData placeableObjectData = default(PlaceableObjectData);
		if (placeableObjectDatas.TryGetComponent(prefab, ref placeableObjectData))
		{
			if ((placeableObjectData.m_Flags & PlacementFlags.Hovering) != PlacementFlags.None)
			{
				result.m_Position.y = WaterUtils.SampleHeight(ref waterSurfaceData, ref terrainHeightData, transform.m_Position);
				result.m_Position.y += placeableObjectData.m_PlacementOffset.y;
				angledSample = false;
			}
			else if ((placeableObjectData.m_Flags & (PlacementFlags.Shoreline | PlacementFlags.Floating)) != PlacementFlags.None)
			{
				WaterUtils.SampleHeight(ref waterSurfaceData, ref terrainHeightData, transform.m_Position, out result.m_Position.y, out var waterHeight, out var waterDepth);
				if (waterDepth >= 0.2f)
				{
					float y = result.m_Position.y;
					result.m_Position.y = math.max(result.m_Position.y, waterHeight + placeableObjectData.m_PlacementOffset.y);
					if ((placeableObjectData.m_Flags & PlacementFlags.Floating) != PlacementFlags.None)
					{
						num2 = math.max(0f, result.m_Position.y - y);
					}
				}
				angledSample = false;
			}
			else
			{
				num = placeableObjectData.m_PlacementOffset.y;
			}
		}
		if (angledSample)
		{
			ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
			if (objectGeometryDatas.TryGetComponent(prefab, ref objectGeometryData) && (objectGeometryData.m_Flags & (GeometryFlags.Standing | GeometryFlags.HasBase)) != GeometryFlags.Standing)
			{
				float3 val = math.forward(transform.m_Rotation);
				val.y = 0f;
				val = math.normalizesafe(val, math.forward());
				float3 val2 = default(float3);
				((float3)(ref val2)).xz = MathUtils.Right(((float3)(ref val)).xz);
				float4 val3 = default(float4);
				val3.x = TerrainUtils.SampleHeight(ref terrainHeightData, transform.m_Position + val2 * objectGeometryData.m_Bounds.min.x + val * objectGeometryData.m_Bounds.min.z);
				val3.y = TerrainUtils.SampleHeight(ref terrainHeightData, transform.m_Position + val2 * objectGeometryData.m_Bounds.min.x + val * objectGeometryData.m_Bounds.max.z);
				val3.z = TerrainUtils.SampleHeight(ref terrainHeightData, transform.m_Position + val2 * objectGeometryData.m_Bounds.max.x + val * objectGeometryData.m_Bounds.max.z);
				val3.w = TerrainUtils.SampleHeight(ref terrainHeightData, transform.m_Position + val2 * objectGeometryData.m_Bounds.max.x + val * objectGeometryData.m_Bounds.min.z);
				if ((objectGeometryData.m_Flags & GeometryFlags.HasBase) != GeometryFlags.None)
				{
					result.m_Position.y = math.cmax(val3);
				}
				else
				{
					float4 val4 = ((float4)(ref val3)).wzyz - ((float4)(ref val3)).xyxw;
					((float4)(ref val4)).xy = (((float4)(ref val4)).xz + ((float4)(ref val4)).yw) / (2f * math.max(float2.op_Implicit(0.01f), MathUtils.Size(((Bounds3)(ref objectGeometryData.m_Bounds)).xz)));
					val2.y = val4.x;
					val.y = val4.y;
					val = math.normalizesafe(val, math.forward());
					float3 val5 = math.normalizesafe(math.cross(val, val2), math.up());
					result.m_Rotation = quaternion.LookRotationSafe(val, val5);
					result.m_Position.y = math.csum(val3) * 0.25f;
				}
			}
			else
			{
				result.m_Position.y = TerrainUtils.SampleHeight(ref terrainHeightData, transform.m_Position);
				angledSample = false;
			}
			result.m_Position.y += num;
		}
		result.m_Position.y += elevation.m_Elevation;
		elevation.m_Elevation += num2;
		return result;
	}

	public static int GetSubParentMesh(ElevationFlags elevationFlags)
	{
		return (elevationFlags & (ElevationFlags.Stacked | ElevationFlags.OnGround)) switch
		{
			ElevationFlags.OnGround => -2, 
			ElevationFlags.Stacked => 1000, 
			ElevationFlags.Stacked | ElevationFlags.OnGround => -1001, 
			_ => 0, 
		};
	}

	public static float GetTerrainSmoothingWidth(ObjectGeometryData objectGeometryData)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return GetTerrainSmoothingWidth(MathUtils.Size(((Bounds3)(ref objectGeometryData.m_Bounds)).xz));
	}

	public static float GetTerrainSmoothingWidth(float2 size)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return math.max(8f, math.length(size) * (1f / 12f));
	}

	public static uint GetRemainingConstructionFrames(UnderConstruction underConstruction)
	{
		return (uint)(math.clamp(100 - underConstruction.m_Progress, 0, 100) * (int)(8192u / (uint)math.max(1, (int)underConstruction.m_Speed)) + 64);
	}

	public static uint GetTripDelayFrames(UnderConstruction underConstruction, PathInformation pathInformation)
	{
		uint remainingConstructionFrames = GetRemainingConstructionFrames(underConstruction);
		uint num = (uint)(pathInformation.m_Duration * 60f + 0.5f);
		return math.select(remainingConstructionFrames - num, 0u, num > remainingConstructionFrames);
	}

	public static bool GetStandingLegCount(ObjectGeometryData objectGeometryData, out int legCount)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		bool3 val = new bool3
		{
			x = ((objectGeometryData.m_Flags & GeometryFlags.Standing) != 0)
		};
		((bool3)(ref val)).yz = objectGeometryData.m_LegOffset != 0f;
		int3 val2 = math.select(int3.op_Implicit(0), int3.op_Implicit(1), val);
		legCount = val2.x << math.csum(((int3)(ref val2)).yz);
		return val.x;
	}

	public static float3 GetStandingLegPosition(ObjectGeometryData objectGeometryData, Transform transform, int legIndex)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		float3 position = transform.m_Position;
		float3 standingLegOffset = GetStandingLegOffset(objectGeometryData, legIndex);
		return position + math.mul(transform.m_Rotation, standingLegOffset);
	}

	public static float3 GetStandingLegOffset(ObjectGeometryData objectGeometryData, int legIndex)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		float3 result = default(float3);
		bool2 val = (new int2(legIndex, math.select(legIndex, legIndex >> 1, objectGeometryData.m_LegOffset.x != 0f)) & 1) != 0;
		((float3)(ref result)).xz = math.select(-objectGeometryData.m_LegOffset, objectGeometryData.m_LegOffset, val);
		return result;
	}
}
