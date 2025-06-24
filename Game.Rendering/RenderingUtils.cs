using Colossal.Mathematics;
using Game.Prefabs;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Rendering;

public static class RenderingUtils
{
	public static Matrix4x4 ToMatrix4x4(float4x4 matrix)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		Matrix4x4 result = default(Matrix4x4);
		((Matrix4x4)(ref result)).SetColumn(0, float4.op_Implicit(matrix.c0));
		((Matrix4x4)(ref result)).SetColumn(1, float4.op_Implicit(matrix.c1));
		((Matrix4x4)(ref result)).SetColumn(2, float4.op_Implicit(matrix.c2));
		((Matrix4x4)(ref result)).SetColumn(3, float4.op_Implicit(matrix.c3));
		return result;
	}

	public static Color ToColor(float4 vector)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return new Color(vector.x, vector.y, vector.z, vector.w);
	}

	public static float4 Lerp(float4 c0, float4 c0_5, float4 c1, float t)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (t <= 0.5f)
		{
			return math.lerp(c0, c0_5, t * 2f);
		}
		return math.lerp(c0_5, c1, t * 2f - 1f);
	}

	public static Bounds ToBounds(Bounds3 bounds)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return new Bounds(float3.op_Implicit(MathUtils.Center(bounds)), float3.op_Implicit(MathUtils.Size(bounds)));
	}

	public static float4 CalculateLodParameters(float lodFactor, BatchCullingContext cullingContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return CalculateLodParameters(lodFactor, cullingContext.lodParameters);
	}

	public static float4 CalculateLodParameters(float lodFactor, LODParameters lodParameters)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		float num = 1f / math.tan(math.radians(((LODParameters)(ref lodParameters)).fieldOfView * 0.5f));
		lodFactor *= 540f * num;
		return new float4(lodFactor, 1f / (lodFactor * lodFactor), num + 1f, num);
	}

	public static float CalculateMinDistance(Bounds3 bounds, float3 cameraPosition, float3 cameraDirection, float4 lodParameters)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		float3 val = bounds.min - cameraPosition;
		float3 val2 = bounds.max - cameraPosition;
		float num = math.length(math.max(float3.op_Implicit(0f), math.max(val, -val2)));
		val *= cameraDirection;
		val2 *= cameraDirection;
		return num * lodParameters.z - lodParameters.w * math.clamp(math.csum(math.max(val, val2)), 0f, num);
	}

	public static float CalculateMaxDistance(Bounds3 bounds, float3 cameraPosition, float3 cameraDirection, float4 lodParameters)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		float3 val = bounds.min - cameraPosition;
		float3 val2 = bounds.max - cameraPosition;
		float num = math.length(math.max(-val, val2));
		val *= cameraDirection;
		val2 *= cameraDirection;
		return num * lodParameters.z - lodParameters.w * math.clamp(math.csum(math.min(val, val2)), 0f, num);
	}

	public static int CalculateLodLimit(float metersPerPixel, float bias)
	{
		metersPerPixel *= math.pow(2f, 0f - bias);
		return CalculateLodLimit(metersPerPixel);
	}

	public static int CalculateLodLimit(float metersPerPixel)
	{
		float num = metersPerPixel * metersPerPixel;
		return (255 - (math.asint(num * num * num) >> 23)) & 0xFF;
	}

	public static int CalculateLod(float distanceSq, float4 lodParameters)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		distanceSq *= lodParameters.y;
		return (255 - (math.asint(distanceSq * distanceSq * distanceSq) >> 23)) & 0xFF;
	}

	public static float CalculateDistanceFactor(int lod)
	{
		return math.pow(2f, (float)(128 - lod) * (1f / 6f));
	}

	public static float CalculateDistance(int lod, float4 lodParameters)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return CalculateDistanceFactor(lod) * lodParameters.x;
	}

	public static Bounds3 SafeBounds(Bounds3 bounds)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		float3 val = math.min(float3.op_Implicit(0f), MathUtils.Size(bounds) - 0.01f);
		ref float3 min = ref bounds.min;
		min += val;
		ref float3 max = ref bounds.max;
		max -= val;
		return bounds;
	}

	public static float GetRenderingSize(float3 size)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return math.csum(size) * (1f / 3f);
	}

	public static float GetRenderingSize(float3 size, float indexCount)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return math.csum(size) * 0.57735026f * math.rsqrt(indexCount);
	}

	public static float GetRenderingSize(float2 size)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		return RenderingUtils.GetRenderingSize(new float3(size, math.cmax(size * new float2(8f, 4f))));
	}

	public static float GetShadowRenderingSize(float2 size)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		return RenderingUtils.GetRenderingSize(new float3(size, math.cmax(size * new float2(2f, 2f))));
	}

	public static float GetRenderingSize(float2 size, float indexFactor)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		float num = math.cmax(size * new float2(8f, 4f));
		float indexCount = indexFactor * num;
		return RenderingUtils.GetRenderingSize(new float3(size, num), indexCount);
	}

	public static float GetRenderingSize(float3 boundsSize, StackDirection stackDirection)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		return stackDirection switch
		{
			StackDirection.Right => GetRenderingSize(((float3)(ref boundsSize)).zy), 
			StackDirection.Up => RenderingUtils.GetRenderingSize(new float2(math.cmax(((float3)(ref boundsSize)).xz), math.cmin(((float3)(ref boundsSize)).xz))), 
			StackDirection.Forward => GetRenderingSize(((float3)(ref boundsSize)).xy), 
			_ => GetRenderingSize(boundsSize), 
		};
	}

	public static float GetShadowRenderingSize(float3 boundsSize, StackDirection stackDirection)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		return stackDirection switch
		{
			StackDirection.Right => GetShadowRenderingSize(((float3)(ref boundsSize)).zy), 
			StackDirection.Up => GetShadowRenderingSize(new float2(math.cmax(((float3)(ref boundsSize)).xz), math.cmin(((float3)(ref boundsSize)).xz))), 
			StackDirection.Forward => GetShadowRenderingSize(((float3)(ref boundsSize)).xy), 
			_ => GetRenderingSize(boundsSize), 
		};
	}

	public static float GetRenderingSize(float3 boundsSize, float3 meshSize, float indexCount, StackDirection stackDirection)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		switch (stackDirection)
		{
		case StackDirection.Right:
		{
			float indexFactor3 = indexCount / math.max(1f, meshSize.x);
			return GetRenderingSize(((float3)(ref boundsSize)).zy, indexFactor3);
		}
		case StackDirection.Up:
		{
			float indexFactor2 = indexCount / math.max(1f, meshSize.y);
			return RenderingUtils.GetRenderingSize(new float2(math.cmax(((float3)(ref boundsSize)).xz), math.cmin(((float3)(ref boundsSize)).xz)), indexFactor2);
		}
		case StackDirection.Forward:
		{
			float indexFactor = indexCount / math.max(1f, meshSize.z);
			return GetRenderingSize(((float3)(ref boundsSize)).xy, indexFactor);
		}
		default:
			return GetRenderingSize(boundsSize, indexCount);
		}
	}

	public static int2 FindBoneIndex(Entity prefab, ref float3 position, ref quaternion rotation, int boneID, ref BufferLookup<SubMesh> subMeshBuffers, ref BufferLookup<ProceduralBone> proceduralBoneBuffers)
	{
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<SubMesh> val = default(DynamicBuffer<SubMesh>);
		if (boneID > 0 && subMeshBuffers.TryGetBuffer(prefab, ref val) && boneID >= val.Length)
		{
			int num = 0;
			DynamicBuffer<ProceduralBone> val2 = default(DynamicBuffer<ProceduralBone>);
			int2 result = default(int2);
			for (int i = 0; i < val.Length; i++)
			{
				SubMesh subMesh = val[i];
				if (proceduralBoneBuffers.TryGetBuffer(subMesh.m_SubMesh, ref val2))
				{
					for (int j = 0; j < val2.Length; j++)
					{
						ProceduralBone proceduralBone = val2[j];
						if (proceduralBone.m_ConnectionID == boneID)
						{
							if ((subMesh.m_Flags & SubMeshFlags.HasTransform) != 0)
							{
								proceduralBone.m_ObjectPosition = subMesh.m_Position + math.rotate(subMesh.m_Rotation, proceduralBone.m_ObjectPosition);
								proceduralBone.m_ObjectRotation = math.mul(subMesh.m_Rotation, proceduralBone.m_ObjectRotation);
							}
							float4x4 val3 = float4x4.TRS(proceduralBone.m_ObjectPosition, proceduralBone.m_ObjectRotation, float3.op_Implicit(1f));
							val3 = math.inverse(math.mul(val3, proceduralBone.m_BindPose));
							position = math.transform(val3, position);
							float3 val4 = math.rotate(val3, math.forward(rotation));
							float3 val5 = math.rotate(val3, math.mul(rotation, math.up()));
							rotation = quaternion.LookRotation(val4, val5);
							result.x = num + j;
							result.y = math.select(-1, i, (subMesh.m_Flags & SubMeshFlags.HasTransform) != 0);
							return result;
						}
					}
					num += val2.Length;
				}
				boneID++;
			}
		}
		return int2.op_Implicit(-1);
	}

	public static BlendWeight GetBlendWeight(CharacterGroup.IndexWeight indexWeight)
	{
		return new BlendWeight
		{
			m_Index = indexWeight.index,
			m_Weight = indexWeight.weight
		};
	}

	public static BlendWeights GetBlendWeights(CharacterGroup.IndexWeight8 indexWeight8)
	{
		return new BlendWeights
		{
			m_Weight0 = GetBlendWeight(indexWeight8.w0),
			m_Weight1 = GetBlendWeight(indexWeight8.w1),
			m_Weight2 = GetBlendWeight(indexWeight8.w2),
			m_Weight3 = GetBlendWeight(indexWeight8.w3),
			m_Weight4 = GetBlendWeight(indexWeight8.w4),
			m_Weight5 = GetBlendWeight(indexWeight8.w5),
			m_Weight6 = GetBlendWeight(indexWeight8.w6),
			m_Weight7 = GetBlendWeight(indexWeight8.w7)
		};
	}
}
