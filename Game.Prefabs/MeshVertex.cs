using System;
using Colossal.AssetPipeline.Native;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Rendering;

namespace Game.Prefabs;

[InternalBufferCapacity(0)]
public struct MeshVertex : IBufferElementData
{
	public float3 m_Vertex;

	public MeshVertex(float3 vertex)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Vertex = vertex;
	}

	public static void Unpack(NativeSlice<byte> src, DynamicBuffer<MeshVertex> dst, int count, VertexAttributeFormat format, int dimension)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		dst.ResizeUninitialized(count);
		Unpack(src, dst.AsNativeArray(), count, format, dimension);
	}

	public unsafe static void Unpack(NativeSlice<byte> src, NativeArray<MeshVertex> dst, int count, VertexAttributeFormat format, int dimension)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Invalid comparison between Unknown and I4
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if ((int)format == 0 && dimension == 3)
		{
			src.SliceConvert<MeshVertex>().CopyTo(dst);
			return;
		}
		if ((int)format == 1)
		{
			NativeMath.ArrayHalfToFloat((IntPtr)NativeSliceUnsafeUtility.GetUnsafeReadOnlyPtr<byte>(src), (long)count, dimension, (IntPtr)NativeArrayUnsafeUtility.GetUnsafePtr<MeshVertex>(dst), 3);
			return;
		}
		throw new Exception($"Unsupported source position format/dimension in Unpack {format} {dimension}");
	}
}
