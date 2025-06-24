using System;
using Colossal.AssetPipeline.Native;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Rendering;

namespace Game.Prefabs;

[InternalBufferCapacity(0)]
public struct MeshTangent : IBufferElementData
{
	public float4 m_Tangent;

	public MeshTangent(float4 tangent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Tangent = tangent;
	}

	public static void Unpack(NativeSlice<byte> src, DynamicBuffer<MeshTangent> dst, int count, VertexAttributeFormat format, int dimension)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		dst.ResizeUninitialized(count);
		Unpack(src, dst.AsNativeArray(), count, format, dimension);
	}

	public unsafe static void Unpack(NativeSlice<byte> src, NativeArray<MeshTangent> dst, int count, VertexAttributeFormat format, int dimension)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Invalid comparison between Unknown and I4
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if ((int)format == 0 && dimension == 4)
		{
			src.SliceConvert<MeshTangent>().CopyTo(dst);
			return;
		}
		if ((int)format == 1)
		{
			NativeMath.ArrayHalfToFloat((IntPtr)NativeSliceUnsafeUtility.GetUnsafeReadOnlyPtr<byte>(src), (long)count, dimension, (IntPtr)NativeArrayUnsafeUtility.GetUnsafePtr<MeshTangent>(dst), 4);
			return;
		}
		if ((int)format == 0 && dimension == 1)
		{
			NativeMath.ArrayOctahedralToTangents((IntPtr)NativeSliceUnsafeUtility.GetUnsafeReadOnlyPtr<byte>(src), (long)count, (IntPtr)NativeArrayUnsafeUtility.GetUnsafePtr<MeshTangent>(dst));
			return;
		}
		throw new Exception($"Unsupported source tangents format/dimension in Unpack {format} {dimension}");
	}
}
