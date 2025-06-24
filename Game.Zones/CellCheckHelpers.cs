using System;
using Colossal.Collections;
using Colossal.Mathematics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Game.Zones;

public static class CellCheckHelpers
{
	public struct SortedEntity : IComparable<SortedEntity>
	{
		public Entity m_Entity;

		public int CompareTo(SortedEntity other)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return m_Entity.Index - other.m_Entity.Index;
		}
	}

	public struct BlockOverlap : IComparable<BlockOverlap>
	{
		public int m_Group;

		public uint m_Priority;

		public Entity m_Block;

		public Entity m_Other;

		public Entity m_Left;

		public Entity m_Right;

		public int CompareTo(BlockOverlap other)
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			int num = m_Group - other.m_Group;
			int num2 = math.select(math.select(0, 1, m_Priority > other.m_Priority), -1, m_Priority < other.m_Priority);
			return math.select(math.select(m_Block.Index - other.m_Block.Index, num2, num2 != 0), num, num != 0);
		}
	}

	public struct OverlapGroup
	{
		public int m_StartIndex;

		public int m_EndIndex;
	}

	[BurstCompile]
	public struct FindUpdatedBlocksSingleIterationJob : IJobParallelForDefer
	{
		private struct Iterator : INativeQuadTreeIterator<Entity, Bounds2>, IUnsafeQuadTreeIterator<Entity, Bounds2>
		{
			public Bounds2 m_Bounds;

			public ParallelWriter<Entity> m_ResultQueue;

			public bool Intersect(Bounds2 bounds)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(bounds, m_Bounds);
			}

			public void Iterate(Bounds2 bounds, Entity blockEntity)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(bounds, m_Bounds))
				{
					m_ResultQueue.Enqueue(blockEntity);
				}
			}
		}

		[ReadOnly]
		public NativeArray<Bounds2> m_Bounds;

		[ReadOnly]
		public NativeQuadTree<Entity, Bounds2> m_SearchTree;

		public ParallelWriter<Entity> m_ResultQueue;

		public void Execute(int index)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			Iterator iterator = new Iterator
			{
				m_Bounds = m_Bounds[index],
				m_ResultQueue = m_ResultQueue
			};
			m_SearchTree.Iterate<Iterator>(ref iterator, 0);
		}
	}

	[BurstCompile]
	public struct FindUpdatedBlocksDoubleIterationJob : IJobParallelForDefer
	{
		private struct FirstIterator : INativeQuadTreeIterator<Entity, Bounds2>, IUnsafeQuadTreeIterator<Entity, Bounds2>
		{
			public Bounds2 m_Bounds;

			public Bounds2 m_ResultBounds;

			public bool Intersect(Bounds2 bounds)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(bounds, m_Bounds);
			}

			public void Iterate(Bounds2 bounds, Entity blockEntity)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(bounds, m_Bounds))
				{
					m_ResultBounds |= bounds;
				}
			}
		}

		private struct SecondIterator : INativeQuadTreeIterator<Entity, Bounds2>, IUnsafeQuadTreeIterator<Entity, Bounds2>
		{
			public Bounds2 m_Bounds;

			public ParallelWriter<Entity> m_ResultQueue;

			public bool Intersect(Bounds2 bounds)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(bounds, m_Bounds);
			}

			public void Iterate(Bounds2 bounds, Entity blockEntity)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(bounds, m_Bounds))
				{
					m_ResultQueue.Enqueue(blockEntity);
				}
			}
		}

		[ReadOnly]
		public NativeArray<Bounds2> m_Bounds;

		[ReadOnly]
		public NativeQuadTree<Entity, Bounds2> m_SearchTree;

		public ParallelWriter<Entity> m_ResultQueue;

		public void Execute(int index)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			FirstIterator firstIterator = new FirstIterator
			{
				m_Bounds = m_Bounds[index],
				m_ResultBounds = new Bounds2(float2.op_Implicit(float.MaxValue), float2.op_Implicit(float.MinValue))
			};
			m_SearchTree.Iterate<FirstIterator>(ref firstIterator, 0);
			SecondIterator secondIterator = new SecondIterator
			{
				m_Bounds = firstIterator.m_ResultBounds,
				m_ResultQueue = m_ResultQueue
			};
			m_SearchTree.Iterate<SecondIterator>(ref secondIterator, 0);
		}
	}

	[BurstCompile]
	public struct CollectBlocksJob : IJob
	{
		public NativeQueue<Entity> m_Queue1;

		public NativeQueue<Entity> m_Queue2;

		public NativeQueue<Entity> m_Queue3;

		public NativeQueue<Entity> m_Queue4;

		public NativeList<SortedEntity> m_ResultList;

		public void Execute()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			ProcessQueue(m_Queue1);
			ProcessQueue(m_Queue2);
			ProcessQueue(m_Queue3);
			ProcessQueue(m_Queue4);
			RemoveDuplicates();
		}

		private void ProcessQueue(NativeQueue<Entity> queue)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			Entity entity = default(Entity);
			while (queue.TryDequeue(ref entity))
			{
				ref NativeList<SortedEntity> resultList = ref m_ResultList;
				SortedEntity sortedEntity = new SortedEntity
				{
					m_Entity = entity
				};
				resultList.Add(ref sortedEntity);
			}
		}

		private void RemoveDuplicates()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			NativeSortExtension.Sort<SortedEntity>(m_ResultList);
			int i = 0;
			int num = 0;
			while (i < m_ResultList.Length)
			{
				SortedEntity sortedEntity = m_ResultList[i++];
				for (; i < m_ResultList.Length; i++)
				{
					SortedEntity sortedEntity2 = m_ResultList[i];
					if (!((Entity)(ref sortedEntity2.m_Entity)).Equals(sortedEntity.m_Entity))
					{
						break;
					}
				}
				m_ResultList[num++] = sortedEntity;
			}
			if (num < m_ResultList.Length)
			{
				m_ResultList.RemoveRange(num, m_ResultList.Length - num);
			}
		}
	}

	[BurstCompile]
	public struct FindOverlappingBlocksJob : IJobParallelForDefer
	{
		private struct Iterator : INativeQuadTreeIterator<Entity, Bounds2>, IUnsafeQuadTreeIterator<Entity, Bounds2>
		{
			public Entity m_BlockEntity;

			public Block m_BlockData;

			public ValidArea m_ValidAreaData;

			public BuildOrder m_BuildOrderData;

			public Bounds2 m_Bounds;

			public Quad2 m_Quad;

			public int m_OverlapCount;

			public ComponentLookup<Block> m_BlockDataFromEntity;

			public ComponentLookup<ValidArea> m_ValidAreaDataEntity;

			public ComponentLookup<BuildOrder> m_BuildOrderDataEntity;

			public ParallelWriter<BlockOverlap> m_ResultQueue;

			public bool Intersect(Bounds2 bounds)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(bounds, m_Bounds);
			}

			public void Iterate(Bounds2 bounds, Entity blockEntity2)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_0060: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0071: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_015a: Unknown result type (might be due to invalid IL or missing references)
				//IL_015f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0166: Unknown result type (might be due to invalid IL or missing references)
				//IL_0167: Unknown result type (might be due to invalid IL or missing references)
				//IL_009e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00be: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_012c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(bounds, m_Bounds) || ((Entity)(ref m_BlockEntity)).Equals(blockEntity2))
				{
					return;
				}
				Block block = m_BlockDataFromEntity[blockEntity2];
				ValidArea validArea = m_ValidAreaDataEntity[blockEntity2];
				BuildOrder buildOrder = m_BuildOrderDataEntity[blockEntity2];
				if (validArea.m_Area.y <= validArea.m_Area.x)
				{
					return;
				}
				Quad2 val = MathUtils.Expand(ZoneUtils.CalculateCorners(block, validArea), -0.01f);
				if (!MathUtils.Intersect(m_Quad, val))
				{
					if (!ZoneUtils.IsNeighbor(m_BlockData, block, m_BuildOrderData, buildOrder))
					{
						return;
					}
					if (math.dot(((float3)(ref block.m_Position)).xz - ((float3)(ref m_BlockData.m_Position)).xz, MathUtils.Right(m_BlockData.m_Direction)) > 0f)
					{
						if ((m_ValidAreaData.m_Area.x != 0) | (validArea.m_Area.y != block.m_Size.x))
						{
							return;
						}
					}
					else if ((m_ValidAreaData.m_Area.y != m_BlockData.m_Size.x) | (validArea.m_Area.x != 0))
					{
						return;
					}
				}
				BlockOverlap blockOverlap = new BlockOverlap
				{
					m_Priority = m_BuildOrderData.m_Order,
					m_Block = m_BlockEntity,
					m_Other = blockEntity2
				};
				m_ResultQueue.Enqueue(blockOverlap);
				m_OverlapCount++;
			}
		}

		[ReadOnly]
		public NativeArray<SortedEntity> m_Blocks;

		[ReadOnly]
		public NativeQuadTree<Entity, Bounds2> m_SearchTree;

		[ReadOnly]
		public ComponentLookup<Block> m_BlockData;

		[ReadOnly]
		public ComponentLookup<ValidArea> m_ValidAreaData;

		[ReadOnly]
		public ComponentLookup<BuildOrder> m_BuildOrderData;

		public ParallelWriter<BlockOverlap> m_ResultQueue;

		public void Execute(int index)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			Entity entity = m_Blocks[index].m_Entity;
			Block block = m_BlockData[entity];
			ValidArea validArea = m_ValidAreaData[entity];
			BuildOrder buildOrderData = m_BuildOrderData[entity];
			if (validArea.m_Area.y > validArea.m_Area.x)
			{
				Iterator iterator = new Iterator
				{
					m_BlockEntity = entity,
					m_BlockData = block,
					m_ValidAreaData = validArea,
					m_BuildOrderData = buildOrderData,
					m_Bounds = MathUtils.Expand(ZoneUtils.CalculateBounds(block), float2.op_Implicit(1.6f)),
					m_Quad = MathUtils.Expand(ZoneUtils.CalculateCorners(block, validArea), -0.01f),
					m_BlockDataFromEntity = m_BlockData,
					m_ValidAreaDataEntity = m_ValidAreaData,
					m_BuildOrderDataEntity = m_BuildOrderData,
					m_ResultQueue = m_ResultQueue
				};
				m_SearchTree.Iterate<Iterator>(ref iterator, 0);
				if (iterator.m_OverlapCount == 0)
				{
					BlockOverlap blockOverlap = new BlockOverlap
					{
						m_Priority = buildOrderData.m_Order,
						m_Block = entity
					};
					m_ResultQueue.Enqueue(blockOverlap);
				}
			}
		}
	}

	[BurstCompile]
	public struct GroupOverlappingBlocksJob : IJob
	{
		[ReadOnly]
		public NativeArray<SortedEntity> m_Blocks;

		public NativeQueue<BlockOverlap> m_OverlapQueue;

		public NativeList<BlockOverlap> m_BlockOverlaps;

		public NativeList<OverlapGroup> m_OverlapGroups;

		public void Execute()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			NativeParallelHashMap<Entity, int> val = default(NativeParallelHashMap<Entity, int>);
			val._002Ector(m_Blocks.Length, AllocatorHandle.op_Implicit((Allocator)2));
			NativeList<int> groups = default(NativeList<int>);
			groups._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
			int num = default(int);
			int num2 = default(int);
			BlockOverlap blockOverlap = default(BlockOverlap);
			while (m_OverlapQueue.TryDequeue(ref blockOverlap))
			{
				if (val.TryGetValue(blockOverlap.m_Block, ref num))
				{
					num = groups[num];
					if (blockOverlap.m_Other != Entity.Null)
					{
						if (val.TryGetValue(blockOverlap.m_Other, ref num2))
						{
							num2 = groups[num2];
							if (num != num2)
							{
								num = MergeGroups(groups, num, num2);
							}
						}
						else
						{
							val.TryAdd(blockOverlap.m_Other, num);
						}
					}
				}
				else if (blockOverlap.m_Other != Entity.Null)
				{
					if (val.TryGetValue(blockOverlap.m_Other, ref num))
					{
						num = groups[num];
						val.TryAdd(blockOverlap.m_Block, num);
					}
					else
					{
						num = CreateGroup(groups);
						val.TryAdd(blockOverlap.m_Block, num);
						val.TryAdd(blockOverlap.m_Other, num);
					}
				}
				else
				{
					num = CreateGroup(groups);
					val.TryAdd(blockOverlap.m_Block, num);
				}
				blockOverlap.m_Group = num;
				m_BlockOverlaps.Add(ref blockOverlap);
			}
			if (m_BlockOverlaps.Length != 0)
			{
				for (int i = 0; i < groups.Length; i++)
				{
					groups[i] = groups[groups[i]];
				}
				for (int j = 0; j < m_BlockOverlaps.Length; j++)
				{
					blockOverlap = m_BlockOverlaps[j];
					blockOverlap.m_Group = groups[blockOverlap.m_Group];
					m_BlockOverlaps[j] = blockOverlap;
				}
				NativeSortExtension.Sort<BlockOverlap>(m_BlockOverlaps);
				OverlapGroup overlapGroup = new OverlapGroup
				{
					m_StartIndex = 0
				};
				int num3 = m_BlockOverlaps[0].m_Group;
				for (int k = 0; k < m_BlockOverlaps.Length; k++)
				{
					int num4 = m_BlockOverlaps[k].m_Group;
					if (num4 != num3)
					{
						overlapGroup.m_EndIndex = k;
						m_OverlapGroups.Add(ref overlapGroup);
						overlapGroup.m_StartIndex = k;
						num3 = num4;
					}
				}
				overlapGroup.m_EndIndex = m_BlockOverlaps.Length;
				m_OverlapGroups.Add(ref overlapGroup);
			}
			groups.Dispose();
			val.Dispose();
		}

		private int CreateGroup(NativeList<int> groups)
		{
			int length = groups.Length;
			groups.Add(ref length);
			return length;
		}

		private int MergeGroups(NativeList<int> groups, int group1, int group2)
		{
			int num = math.min(group1, group2);
			groups[math.max(group1, group2)] = num;
			return num;
		}
	}

	[BurstCompile]
	public struct UpdateBlocksJob : IJobParallelForDefer
	{
		[ReadOnly]
		public NativeArray<SortedEntity> m_Blocks;

		[ReadOnly]
		public ComponentLookup<Block> m_BlockData;

		[NativeDisableParallelForRestriction]
		public BufferLookup<Cell> m_Cells;

		public void Execute(int index)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			Entity entity = m_Blocks[index].m_Entity;
			Block blockData = m_BlockData[entity];
			DynamicBuffer<Cell> cells = m_Cells[entity];
			SetVisible(blockData, cells);
		}

		private void SetVisible(Block blockData, DynamicBuffer<Cell> cells)
		{
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < blockData.m_Size.y; i++)
			{
				for (int j = 0; j < blockData.m_Size.x; j++)
				{
					int num = i * blockData.m_Size.x + j;
					Cell cell = cells[num];
					if ((cell.m_State & (CellFlags.Blocked | CellFlags.Redundant)) != CellFlags.None)
					{
						cell.m_State &= ~(CellFlags.Shared | CellFlags.Visible);
					}
					else
					{
						cell.m_State |= CellFlags.Visible;
					}
					cell.m_State &= ~CellFlags.Updating;
					cells[num] = cell;
				}
			}
		}
	}
}
