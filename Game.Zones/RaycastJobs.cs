using Colossal.Collections;
using Colossal.Mathematics;
using Game.Common;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Game.Zones;

public static class RaycastJobs
{
	[BurstCompile]
	public struct FindZoneBlockJob : IJobParallelFor
	{
		private struct Iterator : INativeQuadTreeIterator<Entity, Bounds2>, IUnsafeQuadTreeIterator<Entity, Bounds2>
		{
			public float2 m_Position;

			public Entity m_Block;

			public int2 m_CellIndex;

			public ComponentLookup<Block> m_BlockData;

			public BufferLookup<Cell> m_Cells;

			public bool Intersect(Bounds2 bounds)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(bounds, m_Position);
			}

			public void Iterate(Bounds2 bounds, Entity blockEntity)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_004e: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0054: Unknown result type (might be due to invalid IL or missing references)
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_0081: Unknown result type (might be due to invalid IL or missing references)
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(bounds, m_Position))
				{
					Block block = m_BlockData[blockEntity];
					int2 cellIndex = ZoneUtils.GetCellIndex(block, m_Position);
					if (math.all((cellIndex >= 0) & (cellIndex < block.m_Size)) && (m_Cells[blockEntity][cellIndex.y * block.m_Size.x + cellIndex.x].m_State & (CellFlags.Shared | CellFlags.Visible)) == CellFlags.Visible)
					{
						m_Block = blockEntity;
						m_CellIndex = cellIndex;
					}
				}
			}
		}

		[ReadOnly]
		public NativeArray<RaycastInput> m_Input;

		[ReadOnly]
		public ComponentLookup<Block> m_BlockData;

		[ReadOnly]
		public BufferLookup<Cell> m_Cells;

		[ReadOnly]
		public NativeQuadTree<Entity, Bounds2> m_SearchTree;

		[ReadOnly]
		public NativeArray<RaycastResult> m_TerrainResults;

		[NativeDisableContainerSafetyRestriction]
		public ParallelWriter<RaycastResult> m_Results;

		public void Execute(int index)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			int num = index % m_Input.Length;
			RaycastInput raycastInput = m_Input[num];
			RaycastResult raycastResult = m_TerrainResults[index];
			if ((raycastInput.m_TypeMask & TypeMask.Zones) != TypeMask.None && !(raycastResult.m_Owner == Entity.Null))
			{
				Iterator iterator = new Iterator
				{
					m_Position = ((float3)(ref raycastResult.m_Hit.m_HitPosition)).xz,
					m_BlockData = m_BlockData,
					m_Cells = m_Cells
				};
				m_SearchTree.Iterate<Iterator>(ref iterator, 0);
				if (iterator.m_Block != Entity.Null)
				{
					raycastResult.m_Owner = iterator.m_Block;
					raycastResult.m_Hit.m_CellIndex = iterator.m_CellIndex;
					raycastResult.m_Hit.m_NormalizedDistance -= 1f / math.max(1f, MathUtils.Length(raycastInput.m_Line));
					m_Results.Accumulate(num, raycastResult);
				}
			}
		}
	}
}
