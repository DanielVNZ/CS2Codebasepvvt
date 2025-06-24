using Unity.Collections;
using Unity.Entities;
using UnityEngine.Rendering;

namespace Game.Prefabs;

[InternalBufferCapacity(0)]
public struct MeshIndex : IBufferElementData
{
	public int m_Index;

	public MeshIndex(int index)
	{
		m_Index = index;
	}

	public static void Unpack(NativeArray<byte> src, DynamicBuffer<MeshIndex> dst, int count, IndexFormat format)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		dst.ResizeUninitialized(count);
		Unpack(src, dst.AsNativeArray(), count, format, 0);
	}

	public static void Unpack(NativeArray<byte> src, NativeArray<MeshIndex> dst, int count, IndexFormat format, int vertexOffset)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if ((int)format == 1)
		{
			NativeArray<int> val = src.Reinterpret<int>(1);
			for (int i = 0; i < val.Length; i++)
			{
				dst[i] = new MeshIndex(val[i] + vertexOffset);
			}
		}
		else
		{
			NativeArray<ushort> val2 = src.Reinterpret<ushort>(1);
			for (int j = 0; j < val2.Length; j++)
			{
				dst[j] = new MeshIndex(val2[j] + vertexOffset);
			}
		}
	}
}
