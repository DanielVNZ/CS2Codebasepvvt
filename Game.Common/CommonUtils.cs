using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Common;

public static class CommonUtils
{
	public static Entity GetRandomEntity(ref Random random, NativeArray<ArchetypeChunk> chunks, EntityTypeHandle entityType)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		for (int i = 0; i < chunks.Length; i++)
		{
			ArchetypeChunk val = chunks[i];
			num += ((ArchetypeChunk)(ref val)).Count;
		}
		if (num == 0)
		{
			return Entity.Null;
		}
		num = ((Random)(ref random)).NextInt(num);
		for (int j = 0; j < chunks.Length; j++)
		{
			ArchetypeChunk val2 = chunks[j];
			if (num < ((ArchetypeChunk)(ref val2)).Count)
			{
				return ((ArchetypeChunk)(ref val2)).GetNativeArray(entityType)[num];
			}
			num -= ((ArchetypeChunk)(ref val2)).Count;
		}
		return Entity.Null;
	}

	public static Entity GetRandomEntity<T>(ref Random random, NativeArray<ArchetypeChunk> chunks, EntityTypeHandle entityType, ComponentTypeHandle<T> componentType, out T componentData) where T : unmanaged, IComponentData
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		componentData = default(T);
		int num = 0;
		for (int i = 0; i < chunks.Length; i++)
		{
			ArchetypeChunk val = chunks[i];
			num += ((ArchetypeChunk)(ref val)).Count;
		}
		if (num == 0)
		{
			return Entity.Null;
		}
		num = ((Random)(ref random)).NextInt(num);
		for (int j = 0; j < chunks.Length; j++)
		{
			ArchetypeChunk val2 = chunks[j];
			if (num < ((ArchetypeChunk)(ref val2)).Count)
			{
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray(entityType);
				componentData = ((ArchetypeChunk)(ref val2)).GetNativeArray<T>(ref componentType)[num];
				return nativeArray[num];
			}
			num -= ((ArchetypeChunk)(ref val2)).Count;
		}
		return Entity.Null;
	}

	public static Entity GetRandomEntity<T1, T2>(ref Random random, NativeArray<ArchetypeChunk> chunks, EntityTypeHandle entityType, ComponentTypeHandle<T1> componentType1, ComponentTypeHandle<T2> componentType2, out T1 componentData1, out T2 componentData2) where T1 : unmanaged, IComponentData where T2 : unmanaged, IComponentData
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		componentData1 = default(T1);
		componentData2 = default(T2);
		int num = 0;
		for (int i = 0; i < chunks.Length; i++)
		{
			ArchetypeChunk val = chunks[i];
			num += ((ArchetypeChunk)(ref val)).Count;
		}
		if (num == 0)
		{
			return Entity.Null;
		}
		num = ((Random)(ref random)).NextInt(num);
		for (int j = 0; j < chunks.Length; j++)
		{
			ArchetypeChunk val2 = chunks[j];
			if (num < ((ArchetypeChunk)(ref val2)).Count)
			{
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray(entityType);
				NativeArray<T1> nativeArray2 = ((ArchetypeChunk)(ref val2)).GetNativeArray<T1>(ref componentType1);
				NativeArray<T2> nativeArray3 = ((ArchetypeChunk)(ref val2)).GetNativeArray<T2>(ref componentType2);
				componentData1 = nativeArray2[num];
				componentData2 = nativeArray3[num];
				return nativeArray[num];
			}
			num -= ((ArchetypeChunk)(ref val2)).Count;
		}
		return Entity.Null;
	}

	public static void Swap<T>(ref T a, ref T b)
	{
		T val = a;
		a = b;
		b = val;
	}

	public static void SwapBits(ref uint bitMask, uint a, uint b)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		uint2 val = math.select(uint2.op_Implicit(0u), new uint2(b, a), (bitMask & new uint2(a, b)) != 0u);
		bitMask = (bitMask & ~(a | b)) | val.x | val.y;
	}

	public static BoundsMask GetBoundsMask(MeshLayer meshLayers)
	{
		BoundsMask boundsMask = (BoundsMask)0;
		if ((meshLayers & (MeshLayer.Default | MeshLayer.Moving | MeshLayer.Tunnel | MeshLayer.Marker)) != 0)
		{
			boundsMask |= BoundsMask.NormalLayers;
		}
		if ((meshLayers & MeshLayer.Pipeline) != 0)
		{
			boundsMask |= BoundsMask.PipelineLayer;
		}
		if ((meshLayers & MeshLayer.SubPipeline) != 0)
		{
			boundsMask |= BoundsMask.SubPipelineLayer;
		}
		if ((meshLayers & MeshLayer.Waterway) != 0)
		{
			boundsMask |= BoundsMask.WaterwayLayer;
		}
		return boundsMask;
	}

	public static bool ExclusiveGroundCollision(CollisionMask mask1, CollisionMask mask2)
	{
		if ((mask1 & mask2 & CollisionMask.OnGround) != 0)
		{
			return ((mask1 | mask2) & CollisionMask.ExclusiveGround) != 0;
		}
		return false;
	}
}
