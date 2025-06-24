using System;
using Colossal.Collections;
using Colossal.Mathematics;
using Colossal.Rendering;
using Game.Buildings;
using Game.Common;
using Game.Events;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Rendering;

public static class BatchDataHelpers
{
	public struct CompositionParameters
	{
		public float3x4 m_TransformMatrix;

		public float4x4 m_CompositionMatrix0;

		public float4x4 m_CompositionMatrix1;

		public float4x4 m_CompositionMatrix2;

		public float4x4 m_CompositionMatrix3;

		public float4x4 m_CompositionMatrix4;

		public float4x4 m_CompositionMatrix5;

		public float4x4 m_CompositionMatrix6;

		public float4x4 m_CompositionMatrix7;

		public float4 m_CompositionSync0;

		public float4 m_CompositionSync1;

		public float4 m_CompositionSync2;

		public float4 m_CompositionSync3;
	}

	public static float4 GetBuildingState(PseudoRandomSeed pseudoRandomSeed, CitizenPresence citizenPresence, float lightFactor, bool abandoned, bool electricity)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		Random random = pseudoRandomSeed.GetRandom(PseudoRandomSeed.kBuildingState);
		float num = math.select(0.2f + (float)(int)citizenPresence.m_Presence * 0.0031372549f, 0f, abandoned);
		float num2 = math.select(0f, num * lightFactor, electricity);
		float num3 = math.select(0.09f, 0f, abandoned || !electricity);
		return new float4(((Random)(ref random)).NextFloat(1f), num2, num3, 0f);
	}

	public static float4 GetBuildingState(PseudoRandomSeed pseudoRandomSeed, int passengersCount, int passengerCapacity, float lightFactor, bool destroyed)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		Random random = pseudoRandomSeed.GetRandom(PseudoRandomSeed.kBuildingState);
		float num = (float)passengersCount / (float)math.max(1, passengerCapacity);
		float num2 = math.select(0.2f + num * 0.0031372549f, 0f, destroyed) * lightFactor;
		float num3 = math.select(0.09f, 0f, destroyed);
		return new float4(((Random)(ref random)).NextFloat(1f), num2, num3, 0f);
	}

	public static float3 GetAnimationCoordinate(AnimationClip clip, float time, float previousTime)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		float2 val = default(float2);
		((float2)(ref val))._002Ector(time, previousTime);
		val /= clip.m_AnimationLength;
		return new float3((math.select(math.saturate(val), math.frac(val), clip.m_Playback != AnimationPlayback.Once) * clip.m_TextureRange + clip.m_TextureOffset) * clip.m_TextureWidth + 0.5f, clip.m_OneOverTextureWidth);
	}

	public static float2 GetBoneParameters(Skeleton skeleton)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		uint num = math.select(((NativeHeapBlock)(ref skeleton.m_BufferAllocation)).End - 1, 0u, ((NativeHeapBlock)(ref skeleton.m_BufferAllocation)).End == 0);
		return new float2(math.asfloat(((NativeHeapBlock)(ref skeleton.m_BufferAllocation)).Begin), math.asfloat(num));
	}

	public static float2 GetBoneParameters(Animated animated)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		uint num = math.select(((NativeHeapBlock)(ref animated.m_BoneAllocation)).End - 1, 0u, ((NativeHeapBlock)(ref animated.m_BoneAllocation)).End == 0);
		return new float2(math.asfloat(((NativeHeapBlock)(ref animated.m_BoneAllocation)).Begin), math.asfloat(num));
	}

	public static float2 GetLightParameters(Emissive emissive)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		uint num = math.select(((NativeHeapBlock)(ref emissive.m_BufferAllocation)).End - 1, 0u, ((NativeHeapBlock)(ref emissive.m_BufferAllocation)).End == 0);
		return new float2(math.asfloat(((NativeHeapBlock)(ref emissive.m_BufferAllocation)).Begin), math.asfloat(num));
	}

	public static float4 GetWetness(Surface surface)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		return new float4((float)(int)surface.m_Wetness, (float)(int)surface.m_SnowAmount, (float)(int)surface.m_AccumulatedWetness, (float)(int)surface.m_AccumulatedSnow) * 0.003921569f;
	}

	public static float4 GetDamage(Surface surface, Damaged damaged, OnFire onFire)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		float2 val = 1f - new float2((float)(int)surface.m_Dirtyness * 0.003921569f, damaged.m_Damage.x);
		float4 result = default(float4);
		result.x = 1f - val.x * val.y;
		result.y = damaged.m_Damage.y;
		result.z = damaged.m_Damage.z;
		result.w = onFire.m_Intensity * 0.01f;
		return result;
	}

	public static SubMeshFlags CalculateTreeSubMeshData(Tree tree, GrowthScaleData growthScaleData, out float3 scale)
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		switch (tree.m_State & (TreeState.Teen | TreeState.Adult | TreeState.Elderly | TreeState.Dead | TreeState.Stump))
		{
		case TreeState.Teen:
			if (tree.m_Growth < 128)
			{
				scale = math.lerp(math.sqrt(growthScaleData.m_ChildSize / growthScaleData.m_TeenSize), float3.op_Implicit(1f), (float)(int)tree.m_Growth * (1f / 128f));
			}
			else
			{
				scale = math.lerp(float3.op_Implicit(1f), math.sqrt(growthScaleData.m_AdultSize / growthScaleData.m_TeenSize), (float)(tree.m_Growth - 128) * (1f / 128f));
			}
			return SubMeshFlags.RequireTeen;
		case TreeState.Adult:
			if (tree.m_Growth < 128)
			{
				scale = math.lerp(math.sqrt(growthScaleData.m_TeenSize / growthScaleData.m_AdultSize), float3.op_Implicit(1f), (float)(int)tree.m_Growth * (1f / 128f));
			}
			else
			{
				scale = math.lerp(float3.op_Implicit(1f), math.sqrt(growthScaleData.m_ElderlySize / growthScaleData.m_AdultSize), (float)(tree.m_Growth - 128) * (1f / 128f));
			}
			return SubMeshFlags.RequireAdult;
		case TreeState.Elderly:
			if (tree.m_Growth < 128)
			{
				scale = math.lerp(math.sqrt(growthScaleData.m_AdultSize / growthScaleData.m_ElderlySize), float3.op_Implicit(1f), (float)(int)tree.m_Growth * (1f / 128f));
			}
			else
			{
				scale = math.lerp(float3.op_Implicit(1f), math.sqrt(growthScaleData.m_DeadSize / growthScaleData.m_ElderlySize), (float)(tree.m_Growth - 128) * (1f / 128f));
			}
			return SubMeshFlags.RequireElderly;
		case TreeState.Dead:
			if (tree.m_Growth < 128)
			{
				scale = math.lerp(math.sqrt(growthScaleData.m_ElderlySize / growthScaleData.m_DeadSize), float3.op_Implicit(1f), (float)(int)tree.m_Growth * (1f / 128f));
			}
			else
			{
				scale = float3.op_Implicit(1f);
			}
			return SubMeshFlags.RequireDead;
		case TreeState.Stump:
			scale = float3.op_Implicit(1f);
			return SubMeshFlags.RequireStump;
		default:
			if (tree.m_Growth < 128)
			{
				scale = math.lerp(math.sqrt(growthScaleData.m_ChildSize / growthScaleData.m_TeenSize), float3.op_Implicit(1f), (float)(int)tree.m_Growth * (1f / 128f));
			}
			else
			{
				scale = math.lerp(float3.op_Implicit(1f), math.sqrt(growthScaleData.m_TeenSize / growthScaleData.m_ChildSize), (float)(tree.m_Growth - 128) * (1f / 128f));
			}
			return SubMeshFlags.RequireChild;
		}
	}

	public static SubMeshFlags CalculateNetObjectSubMeshData(Game.Objects.NetObject netObject)
	{
		return (SubMeshFlags)((((netObject.m_Flags & NetObjectFlags.TrackPassThrough) != 0) ? 4096 : 2048) | (((netObject.m_Flags & NetObjectFlags.Backward) != 0) ? 8388608 : 4194304));
	}

	public static SubMeshFlags CalculateQuantitySubMeshData(Quantity quantity, QuantityObjectData quantityObjectData, bool editorMode)
	{
		if (editorMode)
		{
			if ((quantityObjectData.m_StepMask & 1) == 0)
			{
				return SubMeshFlags.RequireFull;
			}
			return SubMeshFlags.RequireEmpty;
		}
		switch (quantityObjectData.m_StepMask & 6)
		{
		case 6u:
			if (quantity.m_Fullness > 66)
			{
				return SubMeshFlags.RequireFull;
			}
			if (quantity.m_Fullness > 33)
			{
				return SubMeshFlags.RequirePartial2;
			}
			if (quantity.m_Fullness > 0)
			{
				return SubMeshFlags.RequirePartial1;
			}
			return SubMeshFlags.RequireEmpty;
		case 4u:
			if (quantity.m_Fullness > 50)
			{
				return SubMeshFlags.RequireFull;
			}
			if (quantity.m_Fullness > 0)
			{
				return SubMeshFlags.RequirePartial2;
			}
			return SubMeshFlags.RequireEmpty;
		case 2u:
			if (quantity.m_Fullness > 50)
			{
				return SubMeshFlags.RequireFull;
			}
			if (quantity.m_Fullness > 0)
			{
				return SubMeshFlags.RequirePartial1;
			}
			return SubMeshFlags.RequireEmpty;
		default:
			if (quantity.m_Fullness == 0)
			{
				return SubMeshFlags.RequireEmpty;
			}
			return SubMeshFlags.RequireFull;
		}
	}

	public static SubMeshFlags CalculateStackSubMeshData(Stack stack, StackData stackData, out int3 tileCounts, out float3 offsets, out float3 scale)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		float num = MathUtils.Size(stackData.m_FirstBounds);
		float num2 = MathUtils.Size(stackData.m_MiddleBounds);
		float num3 = MathUtils.Size(stackData.m_LastBounds);
		float num4 = MathUtils.Size(stack.m_Range);
		float num5 = num4 - num - num3;
		int num6 = (int)(num5 / num2 + math.select(0.5f, 0.001f, stackData.m_DontScale.y));
		num6 = math.select(0, num6, num5 > 0f && num2 > 0f);
		num5 = math.select(num5, (float)num6 * num2, stackData.m_DontScale.y || num6 == 0);
		num4 -= num5;
		num4 -= math.csum(math.select(float2.op_Implicit(0f), new float2(num, num3), ((bool3)(ref stackData.m_DontScale)).xz));
		float num7 = math.csum(math.select(new float2(num, num3), float2.op_Implicit(0f), ((bool3)(ref stackData.m_DontScale)).xz));
		float num8 = math.select(num, math.max(0.5f, num4 / num7) * num, num6 == 0 && num > 0f && !stackData.m_DontScale.x);
		float num9 = math.select(num3, math.max(0.5f, num4 / num7) * num3, num6 == 0 && num3 > 0f && !stackData.m_DontScale.z);
		tileCounts.x = math.select(0, 1, num8 > 0f);
		tileCounts.y = num6;
		tileCounts.z = math.select(0, 1, num9 > 0f);
		scale.x = math.select(1f, num8 / num, num > 0f);
		scale.y = math.select(1f, num5 / ((float)num6 * num2), num6 > 0 && num2 > 0f);
		scale.z = math.select(1f, num9 / num3, num3 > 0f);
		offsets.x = stack.m_Range.min - stackData.m_FirstBounds.min * scale.x;
		offsets.y = stack.m_Range.min + num8 - stackData.m_MiddleBounds.min * scale.y;
		offsets.z = stack.m_Range.max - stackData.m_LastBounds.max * scale.z;
		return SubMeshFlags.IsStackStart | SubMeshFlags.IsStackMiddle | SubMeshFlags.IsStackEnd;
	}

	public static void AlignStack(ref Stack stack, StackData stackData, bool start, bool end)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		float num = MathUtils.Size(stack.m_Range);
		float num2 = MathUtils.Size(stackData.m_FirstBounds);
		float num3 = MathUtils.Size(stackData.m_MiddleBounds);
		float num4 = MathUtils.Size(stackData.m_LastBounds);
		num2 = math.select(num2 * 0.5f, num2, stackData.m_DontScale.x);
		num4 = math.select(num4 * 0.5f, num4, stackData.m_DontScale.z);
		float num5 = math.max(num, num2 + num4);
		if (math.all(((bool3)(ref stackData.m_DontScale)).xz))
		{
			int num6 = (int)((num5 - num2 - num4) / num3 + math.select(0.5f, 0.001f, stackData.m_DontScale.y));
			num5 = math.select(num5, num2 + num4 + (float)num6 * num3, num6 == 0 || stackData.m_DontScale.y);
		}
		float num7 = (num5 - num) * math.select(1f, 0.5f, start == end);
		stack.m_Range.min -= math.select(num7, 0f, start && !end);
		stack.m_Range.max += math.select(num7, 0f, end && !start);
	}

	public static void CalculateStackSubMeshData(StackData stackData, float3 offsets, float3 scales, int tileIndex, SubMeshFlags subMeshFlags, ref float3 subMeshPosition, ref float3 subMeshScale)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		float num;
		float num2;
		if ((subMeshFlags & SubMeshFlags.IsStackStart) != 0)
		{
			num = offsets.x + scales.x * MathUtils.Size(stackData.m_FirstBounds) * (float)tileIndex;
			num2 = scales.x;
		}
		else if ((subMeshFlags & SubMeshFlags.IsStackMiddle) != 0)
		{
			num = offsets.y + scales.y * MathUtils.Size(stackData.m_MiddleBounds) * (float)tileIndex;
			num2 = scales.y;
		}
		else
		{
			num = offsets.z + scales.z * MathUtils.Size(stackData.m_LastBounds) * (float)tileIndex;
			num2 = scales.z;
		}
		switch (stackData.m_Direction)
		{
		case StackDirection.Right:
			subMeshPosition.x += num;
			subMeshScale.x = num2;
			break;
		case StackDirection.Up:
			subMeshPosition.y += num;
			subMeshScale.y = num2;
			break;
		case StackDirection.Forward:
			subMeshPosition.z += num;
			subMeshScale.z = num2;
			break;
		}
	}

	public static void CalculateEdgeParameters(EdgeGeometry edgeGeometry, bool isRotated, out CompositionParameters compositionParameters)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		float3 val = MathUtils.Center(edgeGeometry.m_Bounds);
		float2 val2 = edgeGeometry.m_Start.m_Length + edgeGeometry.m_End.m_Length;
		CalculateMappingOffsets(edgeGeometry.m_Start, out var leftOffsets, out var rightOffsets, new float2(0f, edgeGeometry.m_Start.m_Length.x), new float2(0f, edgeGeometry.m_Start.m_Length.y));
		CalculateMappingOffsets(edgeGeometry.m_End, out var leftOffsets2, out var rightOffsets2, new float2(edgeGeometry.m_Start.m_Length.x, val2.x), new float2(edgeGeometry.m_Start.m_Length.y, val2.y));
		compositionParameters.m_TransformMatrix = TransformHelper.Translate(val);
		if (isRotated)
		{
			compositionParameters.m_CompositionMatrix0 = BuildEdgeMatrix(MathUtils.Invert(edgeGeometry.m_End.m_Right), val, ((float4)(ref rightOffsets2)).wzyx);
			compositionParameters.m_CompositionMatrix1 = BuildEdgeMatrix(MathUtils.Invert(edgeGeometry.m_Start.m_Right), val, ((float4)(ref rightOffsets)).wzyx);
			compositionParameters.m_CompositionMatrix2 = BuildEdgeMatrix(MathUtils.Invert(edgeGeometry.m_End.m_Left), val, ((float4)(ref leftOffsets2)).wzyx);
			compositionParameters.m_CompositionMatrix3 = BuildEdgeMatrix(MathUtils.Invert(edgeGeometry.m_Start.m_Left), val, ((float4)(ref leftOffsets)).wzyx);
		}
		else
		{
			compositionParameters.m_CompositionMatrix0 = BuildEdgeMatrix(edgeGeometry.m_Start.m_Left, val, leftOffsets);
			compositionParameters.m_CompositionMatrix1 = BuildEdgeMatrix(edgeGeometry.m_End.m_Left, val, leftOffsets2);
			compositionParameters.m_CompositionMatrix2 = BuildEdgeMatrix(edgeGeometry.m_Start.m_Right, val, rightOffsets);
			compositionParameters.m_CompositionMatrix3 = BuildEdgeMatrix(edgeGeometry.m_End.m_Right, val, rightOffsets2);
		}
		compositionParameters.m_CompositionMatrix4 = float4x4.identity;
		compositionParameters.m_CompositionMatrix5 = float4x4.identity;
		compositionParameters.m_CompositionMatrix6 = float4x4.identity;
		compositionParameters.m_CompositionMatrix7 = float4x4.identity;
		compositionParameters.m_CompositionSync0 = new float4(0.2f, 0.4f, 0.6f, 0.8f);
		compositionParameters.m_CompositionSync1 = new float4(0.2f, 0.4f, 0.6f, 0.8f);
		compositionParameters.m_CompositionSync2 = new float4(0.2f, 0.4f, 0.6f, 0.8f);
		compositionParameters.m_CompositionSync3 = new float4(0.2f, 0.4f, 0.6f, 0.8f);
	}

	public static void CalculateNodeParameters(EdgeNodeGeometry nodeGeometry, NetCompositionData prefabCompositionData, out CompositionParameters compositionParameters)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		float3 val = MathUtils.Center(nodeGeometry.m_Bounds);
		float4 mappingOffsets = new float4(0f, 1f / 3f, 2f / 3f, 1f) * prefabCompositionData.m_Width;
		compositionParameters.m_TransformMatrix = TransformHelper.Translate(val);
		compositionParameters.m_CompositionSync0 = prefabCompositionData.m_SyncVertexOffsetsLeft;
		compositionParameters.m_CompositionSync1 = nodeGeometry.m_SyncVertexTargetsLeft;
		compositionParameters.m_CompositionSync2 = prefabCompositionData.m_SyncVertexOffsetsRight;
		compositionParameters.m_CompositionSync3 = nodeGeometry.m_SyncVertexTargetsRight;
		if (nodeGeometry.m_MiddleRadius > 0f)
		{
			float2 val2 = nodeGeometry.m_Left.m_Length + nodeGeometry.m_Right.m_Length;
			CalculateMappingOffsets(nodeGeometry.m_Left, out var leftOffsets, out var rightOffsets, new float2(0f, nodeGeometry.m_Left.m_Length.x), new float2(0f, nodeGeometry.m_Left.m_Length.y));
			CalculateMappingOffsets(nodeGeometry.m_Right, out var leftOffsets2, out var rightOffsets2, new float2(nodeGeometry.m_Left.m_Length.x, val2.x), new float2(nodeGeometry.m_Left.m_Length.y, val2.y));
			float3 direction = MathUtils.StartTangent(nodeGeometry.m_Left.m_Left);
			float3 direction2 = MathUtils.StartTangent(nodeGeometry.m_Left.m_Right);
			Bezier4x3 curve = default(Bezier4x3);
			Bezier4x3 curve2 = default(Bezier4x3);
			MathUtils.Divide(nodeGeometry.m_Middle, ref curve, ref curve2, 0.99f);
			float4 mappingOffsets2 = math.lerp(leftOffsets, rightOffsets, 0.5f);
			float4 mappingOffsets3 = math.lerp(leftOffsets2, rightOffsets2, 0.5f);
			compositionParameters.m_CompositionMatrix0 = BuildEdgeMatrix(nodeGeometry.m_Left.m_Left, val, leftOffsets);
			compositionParameters.m_CompositionMatrix1 = BuildEdgeMatrix(nodeGeometry.m_Right.m_Left, val, leftOffsets2);
			compositionParameters.m_CompositionMatrix2 = BuildEdgeMatrix(nodeGeometry.m_Left.m_Right, val, rightOffsets);
			compositionParameters.m_CompositionMatrix3 = BuildEdgeMatrix(nodeGeometry.m_Right.m_Right, val, rightOffsets2);
			compositionParameters.m_CompositionMatrix4 = BuildEdgeMatrix(curve, val, mappingOffsets2);
			compositionParameters.m_CompositionMatrix5 = BuildEdgeMatrix(curve2, val, mappingOffsets3);
			compositionParameters.m_CompositionMatrix6 = BuildEdgeMatrix(BuildCurve(nodeGeometry.m_Left.m_Left.a, direction, prefabCompositionData.m_Width), val, mappingOffsets);
			compositionParameters.m_CompositionMatrix7 = BuildEdgeMatrix(BuildCurve(nodeGeometry.m_Left.m_Right.a, direction2, prefabCompositionData.m_Width), val, mappingOffsets);
		}
		else
		{
			CalculateMappingOffsets(nodeGeometry.m_Left, out var leftOffsets3, out var rightOffsets3, new float2(0f, nodeGeometry.m_Left.m_Length.x), new float2(0f, nodeGeometry.m_Left.m_Length.y));
			CalculateMappingOffsets(nodeGeometry.m_Right, out var leftOffsets4, out var rightOffsets4, new float2(0f, nodeGeometry.m_Right.m_Length.x), new float2(0f, nodeGeometry.m_Right.m_Length.y));
			float3 direction3 = MathUtils.StartTangent(nodeGeometry.m_Left.m_Left);
			float3 direction4 = MathUtils.StartTangent(nodeGeometry.m_Right.m_Right);
			compositionParameters.m_CompositionMatrix0 = BuildEdgeMatrix(nodeGeometry.m_Left.m_Left, val, leftOffsets3);
			compositionParameters.m_CompositionMatrix1 = BuildEdgeMatrix(nodeGeometry.m_Right.m_Left, val, leftOffsets4);
			compositionParameters.m_CompositionMatrix2 = BuildEdgeMatrix(nodeGeometry.m_Left.m_Right, val, rightOffsets3);
			compositionParameters.m_CompositionMatrix3 = BuildEdgeMatrix(nodeGeometry.m_Right.m_Right, val, rightOffsets4);
			compositionParameters.m_CompositionMatrix4 = BuildEdgeMatrix(nodeGeometry.m_Middle, val, math.lerp(leftOffsets4, rightOffsets3, 0.5f));
			compositionParameters.m_CompositionMatrix5 = float4x4.identity;
			compositionParameters.m_CompositionMatrix6 = BuildEdgeMatrix(BuildCurve(nodeGeometry.m_Left.m_Left.a, direction3, prefabCompositionData.m_Width), val, mappingOffsets);
			compositionParameters.m_CompositionMatrix7 = BuildEdgeMatrix(BuildCurve(nodeGeometry.m_Right.m_Right.a, direction4, prefabCompositionData.m_Width), val, mappingOffsets);
		}
	}

	public static void CalculateOrphanParameters(Node node, NodeGeometry nodeGeometry, NetCompositionData prefabCompositionData, bool isPrimary, out CompositionParameters compositionParameters)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		float3 val = MathUtils.Center(nodeGeometry.m_Bounds);
		Segment segment = default(Segment);
		Bezier4x3 left;
		float3 direction = default(float3);
		if (isPrimary)
		{
			segment.m_Left.a = new float3(node.m_Position.x - prefabCompositionData.m_Width * 0.5f, node.m_Position.y, node.m_Position.z);
			segment.m_Left.b = new float3(node.m_Position.x - prefabCompositionData.m_Width * 0.5f, node.m_Position.y, node.m_Position.z + prefabCompositionData.m_Width * 0.2761424f);
			segment.m_Left.c = new float3(node.m_Position.x - prefabCompositionData.m_Width * 0.2761424f, node.m_Position.y, node.m_Position.z + prefabCompositionData.m_Width * 0.5f);
			segment.m_Left.d = new float3(node.m_Position.x, node.m_Position.y, node.m_Position.z + prefabCompositionData.m_Width * 0.5f);
			segment.m_Right = new Bezier4x3(node.m_Position, node.m_Position, node.m_Position, node.m_Position);
			segment.m_Length = new float2(prefabCompositionData.m_Width * ((float)Math.PI / 2f), 0f);
			left = segment.m_Left;
			left.a.x += prefabCompositionData.m_Width;
			left.b.x += prefabCompositionData.m_Width;
			left.c.x = node.m_Position.x * 2f - left.c.x;
			((float3)(ref direction))._002Ector(0f, 0f, 1f);
		}
		else
		{
			segment.m_Left.a = new float3(node.m_Position.x + prefabCompositionData.m_Width * 0.5f, node.m_Position.y, node.m_Position.z);
			segment.m_Left.b = new float3(node.m_Position.x + prefabCompositionData.m_Width * 0.5f, node.m_Position.y, node.m_Position.z - prefabCompositionData.m_Width * 0.2761424f);
			segment.m_Left.c = new float3(node.m_Position.x + prefabCompositionData.m_Width * 0.2761424f, node.m_Position.y, node.m_Position.z - prefabCompositionData.m_Width * 0.5f);
			segment.m_Left.d = new float3(node.m_Position.x, node.m_Position.y, node.m_Position.z - prefabCompositionData.m_Width * 0.5f);
			segment.m_Right = new Bezier4x3(node.m_Position, node.m_Position, node.m_Position, node.m_Position);
			segment.m_Length = new float2(prefabCompositionData.m_Width * ((float)Math.PI / 2f), 0f);
			left = segment.m_Left;
			left.a.x -= prefabCompositionData.m_Width;
			left.b.x -= prefabCompositionData.m_Width;
			left.c.x = node.m_Position.x * 2f - left.c.x;
			((float3)(ref direction))._002Ector(0f, 0f, -1f);
		}
		CalculateMappingOffsets(segment, out var leftOffsets, out var rightOffsets, new float2(0f, segment.m_Length.x), new float2(0f, 0f));
		float4 mappingOffsets = new float4(0f, 1f / 3f, 2f / 3f, 1f) * prefabCompositionData.m_Width;
		compositionParameters.m_TransformMatrix = TransformHelper.Translate(val);
		compositionParameters.m_CompositionSync0 = prefabCompositionData.m_SyncVertexOffsetsLeft;
		compositionParameters.m_CompositionSync1 = prefabCompositionData.m_SyncVertexOffsetsLeft;
		compositionParameters.m_CompositionSync2 = prefabCompositionData.m_SyncVertexOffsetsRight;
		compositionParameters.m_CompositionSync3 = prefabCompositionData.m_SyncVertexOffsetsRight;
		compositionParameters.m_CompositionMatrix0 = BuildEdgeMatrix(segment.m_Left, val, leftOffsets);
		compositionParameters.m_CompositionMatrix1 = BuildEdgeMatrix(segment.m_Right, val, rightOffsets);
		compositionParameters.m_CompositionMatrix2 = compositionParameters.m_CompositionMatrix1;
		compositionParameters.m_CompositionMatrix3 = BuildEdgeMatrix(left, val, leftOffsets);
		compositionParameters.m_CompositionMatrix4 = compositionParameters.m_CompositionMatrix1;
		compositionParameters.m_CompositionMatrix5 = float4x4.identity;
		compositionParameters.m_CompositionMatrix6 = BuildEdgeMatrix(BuildCurve(segment.m_Left.a, direction, prefabCompositionData.m_Width), val, mappingOffsets);
		compositionParameters.m_CompositionMatrix7 = BuildEdgeMatrix(BuildCurve(left.a, direction, prefabCompositionData.m_Width), val, mappingOffsets);
	}

	private static void CalculateMappingOffsets(Segment segment, out float4 leftOffsets, out float4 rightOffsets, float2 leftMappingOffset, float2 rightMappingOffset)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		float2 val = default(float2);
		val.x = Vector3.Distance(float3.op_Implicit(segment.m_Left.a), float3.op_Implicit(segment.m_Left.b));
		val.y = Vector3.Distance(float3.op_Implicit(segment.m_Left.c), float3.op_Implicit(segment.m_Left.d));
		float2 val2 = default(float2);
		val2.x = Vector3.Distance(float3.op_Implicit(segment.m_Right.a), float3.op_Implicit(segment.m_Right.b));
		val2.y = Vector3.Distance(float3.op_Implicit(segment.m_Right.c), float3.op_Implicit(segment.m_Right.d));
		val *= (leftMappingOffset.y - leftMappingOffset.x) / math.max(1f, segment.m_Length.x);
		val2 *= (rightMappingOffset.y - rightMappingOffset.x) / math.max(1f, segment.m_Length.y);
		leftOffsets = new float4(leftMappingOffset.x, leftMappingOffset.x + val.x, leftMappingOffset.y - val.y, leftMappingOffset.y);
		rightOffsets = new float4(rightMappingOffset.x, rightMappingOffset.x + val2.x, rightMappingOffset.y - val2.y, rightMappingOffset.y);
	}

	private static float4x4 BuildEdgeMatrix(Bezier4x3 curve, float3 offset, float4 mappingOffsets)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		return new float4x4
		{
			c0 = new float4(curve.a - offset, mappingOffsets.x),
			c1 = new float4(curve.b - offset, mappingOffsets.y),
			c2 = new float4(curve.c - offset, mappingOffsets.z),
			c3 = new float4(curve.d - offset, mappingOffsets.w)
		};
	}

	private static Bezier4x3 BuildCurve(float3 startPos, float3 direction, float length)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		direction = MathUtils.Normalize(direction, ((float3)(ref direction)).xz);
		direction.y = math.clamp(direction.y, -1f, 1f);
		Bezier4x3 result = default(Bezier4x3);
		result.a = startPos;
		result.b = startPos + direction * (length * (1f / 3f));
		result.c = startPos + direction * (length * (2f / 3f));
		result.d = startPos + direction * length;
		return result;
	}

	public static float4x4 BuildTransformMatrix(Curve curve, float4 size, float4 curveScale, float smoothingDistance, bool isDecal, bool isLoaded)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		if (isDecal)
		{
			float3 val = curve.m_Bezier.d - curve.m_Bezier.a;
			float3 val2 = math.select(new float3(2.5f, 2.5f, 2f), ((float4)(ref size)).xyz, isLoaded);
			((float4)(ref size)).xy = ((float4)(ref size)).xy * math.max(((float4)(ref curveScale)).xy, ((float4)(ref curveScale)).zw);
			if (MathUtils.TryNormalize(ref val))
			{
				float3 val3 = math.normalizesafe(MathUtils.StartTangent(curve.m_Bezier), default(float3));
				float3 val4 = math.normalizesafe(MathUtils.EndTangent(curve.m_Bezier), default(float3));
				ref float3 a = ref curve.m_Bezier.a;
				a -= val3 * smoothingDistance;
				ref float3 d = ref curve.m_Bezier.d;
				d += val4 * smoothingDistance;
				float3 val5 = default(float3);
				((float3)(ref val5)).xz = math.normalizesafe(MathUtils.Right(((float3)(ref val)).xz), new float2(1f, 0f));
				float3 val6 = math.cross(val, val5);
				float3 val7 = curve.m_Bezier.b - curve.m_Bezier.a;
				float3 val8 = curve.m_Bezier.c - curve.m_Bezier.a;
				float3 val9 = curve.m_Bezier.d - curve.m_Bezier.a;
				float3 val10 = default(float3);
				((float3)(ref val10))._002Ector(math.dot(val7, val5), math.dot(val7, val6), math.dot(val7, val));
				float3 val11 = default(float3);
				((float3)(ref val11))._002Ector(math.dot(val8, val5), math.dot(val8, val6), math.dot(val8, val));
				float3 val12 = default(float3);
				((float3)(ref val12))._002Ector(math.dot(val9, val5), math.dot(val9, val6), math.dot(val9, val));
				float3 val13 = math.min(math.min(float3.op_Implicit(0f), val10), math.min(val11, val12));
				float3 val14 = math.max(math.max(float3.op_Implicit(0f), val10), math.max(val11, val12));
				float2 val15 = default(float2);
				((float2)(ref val15))._002Ector(math.dot(val5, val3), math.dot(val5, val4));
				float2 val16 = default(float2);
				((float2)(ref val16))._002Ector(val3.y, val4.y);
				float3 val17 = new float3(((float4)(ref size)).xy, size.x * math.cmax(math.abs(val15)) + size.y * math.cmax(math.abs(val16))) * 0.5f;
				val13 -= val17;
				val14 += val17;
				float3 val18 = math.lerp(val13, val14, 0.5f);
				quaternion val19 = quaternion.LookRotation(val, val6);
				float3 val20 = curve.m_Bezier.a + math.rotate(val19, val18);
				float3 val21 = (val14 - val13) / val2;
				val20.y += size.w;
				val20 -= val6 * (size.w * val21.y);
				return float4x4.TRS(val20, val19, val21);
			}
			float3 val22 = math.lerp(curve.m_Bezier.a, curve.m_Bezier.d, 0.5f);
			quaternion identity = quaternion.identity;
			size.z += smoothingDistance;
			float3 val23 = ((float4)(ref size)).xyx / val2;
			return float4x4.TRS(val22, identity, val23);
		}
		if (isLoaded)
		{
			return float4x4.Translate(math.lerp(curve.m_Bezier.a, curve.m_Bezier.d, 0.5f));
		}
		float3 val24 = math.lerp(curve.m_Bezier.a, curve.m_Bezier.d, 0.5f);
		quaternion identity2 = quaternion.identity;
		float3 val25 = default(float3);
		((float3)(ref val25))._002Ector(math.max(((float4)(ref size)).xy, float2.op_Implicit(0.02f)) * 0.4f, 1f);
		return float4x4.TRS(val24, identity2, val25);
	}

	public static float4x4 BuildCurveMatrix(Curve curve, float3x4 transformMatrix, float4 size, int tilingCount)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		float2 val = default(float2);
		val.x = math.distance(curve.m_Bezier.a, curve.m_Bezier.b);
		val.y = math.distance(curve.m_Bezier.c, curve.m_Bezier.d);
		val /= curve.m_Length;
		float4 val2 = default(float4);
		((float4)(ref val2))._002Ector(0f, val.x, 1f - val.y, 1f);
		float3 c = transformMatrix.c3;
		float num = curve.m_Length / math.max(1f, size.z);
		num = math.select(num, math.round(num * (float)tilingCount) / (float)tilingCount, tilingCount != 0);
		val2 *= num;
		float4x4 result = default(float4x4);
		result.c0 = new float4(curve.m_Bezier.a - c, val2.x);
		result.c1 = new float4(curve.m_Bezier.b - c, val2.y);
		result.c2 = new float4(curve.m_Bezier.c - c, val2.z);
		result.c3 = new float4(curve.m_Bezier.d - c, val2.w);
		return result;
	}

	public static float4 BuildCurveParams(float4 size, NodeLane nodeLane)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		return new float4(size.z, 0f, math.select(1f / new float2(1f + (float)(int)nodeLane.m_SharedStartCount, 1f + (float)(int)nodeLane.m_SharedEndCount), float2.op_Implicit(-1f), new bool2(nodeLane.m_SharedStartCount == byte.MaxValue, nodeLane.m_SharedEndCount == byte.MaxValue)));
	}

	public static float4 BuildCurveParams(float4 size, EdgeLane edgeLane)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		return new float4(size.z, 0f, math.select(float2.op_Implicit(1f), float2.op_Implicit(0f), new bool2(edgeLane.m_ConnectedStartCount == 0, edgeLane.m_ConnectedEndCount == 0)));
	}

	public static float4 BuildCurveParams(float4 size, Game.Net.Elevation elevation)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		return new float4(size.z, math.select(0f, 1f, math.any(elevation.m_Elevation == float.MinValue)), 1f, 1f);
	}

	public static float4 BuildCurveParams(float4 size)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		return new float4(size.z, 1f, 1f, 1f);
	}

	public static float4 BuildCurveScale(NodeLane nodeLane, NetLaneData netLaneData)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		float2 val = math.select(1f + nodeLane.m_WidthOffset / netLaneData.m_Width, float2.op_Implicit(1f), netLaneData.m_Width == 0f);
		return new float4(((float2)(ref val)).xxyy);
	}

	public static float4 BuildCurveScale()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return float4.op_Implicit(1f);
	}

	public static int GetTileCount(Curve curve, float length, int tilingCount, bool geometryTiling, out int clipCount)
	{
		if (tilingCount != 0)
		{
			float num = curve.m_Length / math.max(1f, length);
			clipCount = Mathf.RoundToInt(num * (float)tilingCount);
			int num2 = (clipCount + tilingCount - 1) / tilingCount;
			return math.select(math.min(1, num2), math.min(256, num2), geometryTiling);
		}
		if (geometryTiling)
		{
			float num3 = curve.m_Length / math.max(1f, length);
			clipCount = 0;
			return math.clamp(Mathf.CeilToInt(num3 - 0.0001f), 1, 256);
		}
		clipCount = 0;
		return 1;
	}
}
