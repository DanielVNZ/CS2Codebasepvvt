using Colossal.Collections;
using Colossal.Mathematics;
using Game.Common;
using Game.Prefabs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Game.Zones;

public static class LotSizeJobs
{
	[BurstCompile]
	public struct UpdateLotSizeJob : IJobParallelForDefer
	{
		private struct Iterator : INativeQuadTreeIterator<Entity, Bounds2>, IUnsafeQuadTreeIterator<Entity, Bounds2>
		{
			public Entity m_Entity;

			public Block m_Block;

			public ValidArea m_ValidArea;

			public BuildOrder m_BuildOrder;

			public Bounds2 m_Bounds;

			public Quad2 m_Quad;

			public ComponentLookup<Block> m_BlockData;

			public ComponentLookup<ValidArea> m_ValidAreaData;

			public ComponentLookup<BuildOrder> m_BuildOrderData;

			public BufferLookup<Cell> m_CellData;

			public NativeArray<Cell> m_Cells;

			public bool Intersect(Bounds2 bounds)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(bounds, m_Bounds);
			}

			public void Iterate(Bounds2 bounds, Entity entity2)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0062: Unknown result type (might be due to invalid IL or missing references)
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				//IL_0098: Unknown result type (might be due to invalid IL or missing references)
				//IL_009e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00de: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_015f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0160: Unknown result type (might be due to invalid IL or missing references)
				//IL_0165: Unknown result type (might be due to invalid IL or missing references)
				//IL_016a: Unknown result type (might be due to invalid IL or missing references)
				//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0181: Unknown result type (might be due to invalid IL or missing references)
				//IL_029d: Unknown result type (might be due to invalid IL or missing references)
				//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0195: Unknown result type (might be due to invalid IL or missing references)
				//IL_0197: Unknown result type (might be due to invalid IL or missing references)
				//IL_0199: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
				//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_01df: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
				//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0214: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(bounds, m_Bounds) || ((Entity)(ref m_Entity)).Equals(entity2))
				{
					return;
				}
				ValidArea validArea = m_ValidAreaData[entity2];
				if (validArea.m_Area.y <= validArea.m_Area.x)
				{
					return;
				}
				Block block = m_BlockData[entity2];
				Quad2 val = MathUtils.Expand(ZoneUtils.CalculateCorners(block, validArea), -0.01f);
				if (!MathUtils.Intersect(m_Quad, val))
				{
					return;
				}
				BuildOrder buildOrder = m_BuildOrderData[entity2];
				if (!ZoneUtils.CanShareCells(m_Block, block, m_BuildOrder, buildOrder))
				{
					return;
				}
				float3 cellPosition = ZoneUtils.GetCellPosition(block, default(int2));
				int2 cellIndex = ZoneUtils.GetCellIndex(m_Block, ((float3)(ref cellPosition)).xz);
				float num = math.dot(m_Block.m_Direction, block.m_Direction);
				float num2 = math.dot(MathUtils.Left(m_Block.m_Direction), block.m_Direction);
				int2 val2 = default(int2);
				int2 val3 = default(int2);
				if (num > 0.5f)
				{
					((int2)(ref val2))._002Ector(1, 0);
					((int2)(ref val3))._002Ector(0, 1);
				}
				else if (num < -0.5f)
				{
					((int2)(ref val2))._002Ector(-1, 0);
					((int2)(ref val3))._002Ector(0, -1);
				}
				else if (num2 > 0.5f)
				{
					((int2)(ref val2))._002Ector(0, -1);
					((int2)(ref val3))._002Ector(1, 0);
				}
				else
				{
					((int2)(ref val2))._002Ector(0, 1);
					((int2)(ref val3))._002Ector(-1, 0);
				}
				DynamicBuffer<Cell> val4 = m_CellData[entity2];
				int2 val5 = default(int2);
				val5.y = validArea.m_Area.z;
				while (val5.y < validArea.m_Area.w)
				{
					val5.x = validArea.m_Area.x;
					while (val5.x < validArea.m_Area.y)
					{
						int2 val6 = cellIndex + new int2(math.dot(val2, val5), math.dot(val3, val5));
						if (!(math.any(val6 < 0) | math.any(val6 >= m_Block.m_Size)))
						{
							int num3 = val5.y * block.m_Size.x + val5.x;
							int num4 = val6.y * m_Block.m_Size.x + val6.x;
							Cell cell = val4[num3];
							if ((cell.m_State & (CellFlags.Blocked | CellFlags.Shared | CellFlags.Occupied | CellFlags.Redundant)) == 0 && !cell.m_Zone.Equals(ZoneType.None))
							{
								if ((cell.m_State & (CellFlags.Roadside | CellFlags.RoadLeft | CellFlags.RoadRight | CellFlags.RoadBack)) != CellFlags.None)
								{
									cell.m_State = (cell.m_State & ~(CellFlags.Roadside | CellFlags.RoadLeft | CellFlags.RoadRight | CellFlags.RoadBack)) | ZoneUtils.GetRoadDirection(m_Block, block, cell.m_State);
								}
								m_Cells[num4] = cell;
							}
						}
						val5.x++;
					}
					val5.y++;
				}
			}
		}

		[ReadOnly]
		public NativeArray<CellCheckHelpers.SortedEntity> m_Blocks;

		[ReadOnly]
		public ZonePrefabs m_ZonePrefabs;

		[ReadOnly]
		public ComponentLookup<Block> m_BlockData;

		[ReadOnly]
		public ComponentLookup<ValidArea> m_ValidAreaData;

		[ReadOnly]
		public ComponentLookup<BuildOrder> m_BuildOrderData;

		[ReadOnly]
		public ComponentLookup<Updated> m_UpdatedData;

		[ReadOnly]
		public ComponentLookup<ZoneData> m_ZoneData;

		[ReadOnly]
		public BufferLookup<Cell> m_Cells;

		[ReadOnly]
		public NativeQuadTree<Entity, Bounds2> m_SearchTree;

		[NativeDisableParallelForRestriction]
		public BufferLookup<VacantLot> m_VacantLots;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<Bounds2> m_BoundsQueue;

		public void Execute(int index)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0542: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0586: Unknown result type (might be due to invalid IL or missing references)
			//IL_056a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0595: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0363: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_052e: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Unknown result type (might be due to invalid IL or missing references)
			//IL_0385: Unknown result type (might be due to invalid IL or missing references)
			//IL_038a: Unknown result type (might be due to invalid IL or missing references)
			//IL_04db: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_041f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0424: Unknown result type (might be due to invalid IL or missing references)
			//IL_0426: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0514: Unknown result type (might be due to invalid IL or missing references)
			//IL_0516: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0502: Unknown result type (might be due to invalid IL or missing references)
			//IL_0504: Unknown result type (might be due to invalid IL or missing references)
			//IL_0506: Unknown result type (might be due to invalid IL or missing references)
			//IL_048a: Unknown result type (might be due to invalid IL or missing references)
			//IL_048c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_044f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0451: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_0455: Unknown result type (might be due to invalid IL or missing references)
			//IL_045a: Unknown result type (might be due to invalid IL or missing references)
			//IL_045c: Unknown result type (might be due to invalid IL or missing references)
			//IL_045e: Unknown result type (might be due to invalid IL or missing references)
			//IL_046d: Unknown result type (might be due to invalid IL or missing references)
			//IL_046f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0471: Unknown result type (might be due to invalid IL or missing references)
			//IL_0473: Unknown result type (might be due to invalid IL or missing references)
			//IL_0478: Unknown result type (might be due to invalid IL or missing references)
			//IL_047a: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			Entity entity = m_Blocks[index].m_Entity;
			Block block = m_BlockData[entity];
			ValidArea validArea = m_ValidAreaData[entity];
			BuildOrder buildOrder = m_BuildOrderData[entity];
			DynamicBuffer<Cell> val = m_Cells[entity];
			DynamicBuffer<VacantLot> val2 = default(DynamicBuffer<VacantLot>);
			if (m_VacantLots.HasBuffer(entity))
			{
				val2 = m_VacantLots[entity];
				val2.Clear();
			}
			NativeArray<Cell> cells = default(NativeArray<Cell>);
			int2 expandedOffset = default(int2);
			Block expandedBlock = default(Block);
			int num = validArea.m_Area.x;
			int2 min = default(int2);
			int2 val8 = default(int2);
			int2 val9 = default(int2);
			int2 val10 = default(int2);
			while (num < validArea.m_Area.y)
			{
				Cell cell = val[validArea.m_Area.z * block.m_Size.x + num];
				if ((cell.m_State & (CellFlags.Blocked | CellFlags.Occupied)) == 0 && !cell.m_Zone.Equals(ZoneType.None))
				{
					((int2)(ref min))._002Ector(num, validArea.m_Area.z);
					int2 max = default(int2);
					if (!cells.IsCreated)
					{
						FindDepth(block, val.AsNativeArray(), ref min, ref max, cell.m_Zone);
						ExpandRight(block, val.AsNativeArray(), ref min, ref max, cell.m_Zone);
						if (min.x == 0 || math.any(max == block.m_Size))
						{
							cells = ExpandArea(entity, block, validArea, buildOrder, val.AsNativeArray(), out expandedOffset, out expandedBlock);
						}
					}
					int2 val3 = min;
					int2 val4 = max;
					Entity val5 = m_ZonePrefabs[cell.m_Zone];
					ZoneData zoneData = m_ZoneData[val5];
					Cell cell2;
					Cell cell3;
					if (cells.IsCreated)
					{
						min += expandedOffset;
						FindDepth(expandedBlock, cells, ref min, ref max, cell.m_Zone);
						ExpandRight(expandedBlock, cells, ref min, ref max, cell.m_Zone);
						if (num == 0)
						{
							ExpandLeft(expandedBlock, cells, ref min, ref max, cell.m_Zone);
						}
						min -= expandedOffset;
						max -= expandedOffset;
						val3 = min;
						val4 = max;
						int num2 = math.select(0, 1, (zoneData.m_ZoneFlags & ZoneFlags.SupportNarrow) == 0);
						if (min.x < -num2)
						{
							WidthReductionLeft(block, ref min, ref max, num2);
						}
						if (max.x > block.m_Size.x + num2)
						{
							WidthReductionRight(block, ref min, ref max, num2);
						}
						int2 val6 = min + expandedOffset;
						int2 val7 = max + expandedOffset;
						cell2 = cells[val6.y * expandedBlock.m_Size.x + val6.x];
						cell3 = cells[val6.y * expandedBlock.m_Size.x + (val7.x - 1)];
					}
					else
					{
						cell2 = val[min.y * block.m_Size.x + min.x];
						cell3 = val[min.y * block.m_Size.x + (max.x - 1)];
					}
					LotFlags lotFlags = (LotFlags)0;
					if ((cell2.m_State & CellFlags.RoadLeft) != CellFlags.None)
					{
						lotFlags |= LotFlags.CornerLeft;
					}
					if ((cell3.m_State & CellFlags.RoadRight) != CellFlags.None)
					{
						lotFlags |= LotFlags.CornerRight;
					}
					((int2)(ref val8))._002Ector(math.select(2, 1, (zoneData.m_ZoneFlags & ZoneFlags.SupportNarrow) != 0), 2);
					if (math.all(max - min >= val8))
					{
						if (!val2.IsCreated)
						{
							val2 = ((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<VacantLot>(index, entity);
						}
						if (max.x - min.x > 8)
						{
							int num3 = math.min(min.x + val4.x + 1 >> 1, min.x + 6 + 2);
							int num4 = math.max(val3.x + max.x >> 1, max.x - 6 - 2);
							((int2)(ref val9))._002Ector(num3, max.y);
							((int2)(ref val10))._002Ector(num4, min.y);
							int height = cell.m_Height;
							int height2 = cell.m_Height;
							FindHeight(block, val.AsNativeArray(), min, val9, ref height);
							FindHeight(block, val.AsNativeArray(), min, val9, ref height2);
							if (cells.IsCreated)
							{
								FindHeight(expandedBlock, cells, min + expandedOffset, val9 + expandedOffset, ref height);
								FindHeight(expandedBlock, cells, val10 + expandedOffset, max + expandedOffset, ref height2);
							}
							val2.Add(new VacantLot(min, val9, cell.m_Zone, height, lotFlags & ~LotFlags.CornerRight));
							val2.Add(new VacantLot(val10, max, cell.m_Zone, height2, lotFlags & ~LotFlags.CornerLeft));
						}
						else
						{
							int height3 = cell.m_Height;
							FindHeight(block, val.AsNativeArray(), min, max, ref height3);
							if (cells.IsCreated)
							{
								FindHeight(expandedBlock, cells, min + expandedOffset, max + expandedOffset, ref height3);
							}
							val2.Add(new VacantLot(min, max, cell.m_Zone, height3, lotFlags));
						}
					}
					num = val4.x;
				}
				else
				{
					num++;
				}
			}
			if (val2.IsCreated && val2.Length == 0)
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<VacantLot>(index, entity);
			}
			if (cells.IsCreated)
			{
				cells.Dispose();
			}
			if (!m_UpdatedData.HasComponent(entity))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(index, entity, default(Updated));
				m_BoundsQueue.Enqueue(ZoneUtils.CalculateBounds(block));
			}
		}

		private void FindHeight(Block block, NativeArray<Cell> cells, int2 min, int2 max, ref int height)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			min = math.max(min, int2.op_Implicit(0));
			max = math.min(max, block.m_Size);
			for (int i = min.y; i < max.y; i++)
			{
				for (int j = min.x; j < max.x; j++)
				{
					int num = i * block.m_Size.x + j;
					Cell cell = cells[num];
					height = math.min(height, (int)cell.m_Height);
				}
			}
		}

		private void FindDepth(Block block, NativeArray<Cell> cells, ref int2 min, ref int2 max, ZoneType zone)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			max.y = block.m_Size.y;
			for (int i = min.y + 1; i < block.m_Size.y; i++)
			{
				int num = i * block.m_Size.x + min.x;
				Cell cell = cells[num];
				if ((cell.m_State & (CellFlags.Blocked | CellFlags.Occupied)) != CellFlags.None || !cell.m_Zone.Equals(zone))
				{
					max.y = i;
					break;
				}
			}
			if (max.y > 6)
			{
				max.y = max.y + 1 >> 1;
			}
		}

		private void ExpandRight(Block block, NativeArray<Cell> cells, ref int2 min, ref int2 max, ZoneType zone)
		{
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			for (int i = min.x + 1; i < block.m_Size.x; i++)
			{
				for (int j = min.y; j < max.y; j++)
				{
					int num = j * block.m_Size.x + i;
					Cell cell = cells[num];
					if ((cell.m_State & (CellFlags.Blocked | CellFlags.Occupied)) != CellFlags.None || !cell.m_Zone.Equals(zone))
					{
						max.x = i;
						return;
					}
				}
			}
			max.x = block.m_Size.x;
		}

		private void ExpandLeft(Block block, NativeArray<Cell> cells, ref int2 min, ref int2 max, ZoneType zone)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			for (int num = min.x - 1; num >= 0; num--)
			{
				for (int i = min.y; i < max.y; i++)
				{
					int num2 = i * block.m_Size.x + num;
					Cell cell = cells[num2];
					if ((cell.m_State & (CellFlags.Blocked | CellFlags.Occupied)) != CellFlags.None || !cell.m_Zone.Equals(zone))
					{
						min.x = num + 1;
						return;
					}
				}
			}
			min.x = 0;
		}

		private void WidthReductionLeft(Block block, ref int2 min, ref int2 max, int sizeOffset)
		{
			int num = 3;
			if (max.x < num && min.x < -max.x)
			{
				min.x = max.x;
			}
			else if ((min.x <= -num || max.x < -min.x) && max.x - min.x > 6)
			{
				min.x = math.min(-sizeOffset, min.x + max.x >> 1);
			}
		}

		private void WidthReductionRight(Block block, ref int2 min, ref int2 max, int sizeOffset)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			int num = 3;
			if (min.x > block.m_Size.x - num && max.x - block.m_Size.x > block.m_Size.x - min.x)
			{
				max.x = min.x;
			}
			else if ((max.x - block.m_Size.x >= num || block.m_Size.x - min.x < max.x - block.m_Size.x) && max.x - min.x > 6)
			{
				max.x = math.max(block.m_Size.x + sizeOffset, min.x + max.x + 1 >> 1);
			}
		}

		private NativeArray<Cell> ExpandArea(Entity entity, Block block, ValidArea validArea, BuildOrder buildOrder, NativeArray<Cell> cells, out int2 expandedOffset, out Block expandedBlock)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			expandedBlock = block;
			expandedOffset.x = math.select(0, 6, validArea.m_Area.x == 0);
			expandedOffset.y = 0;
			ref int2 size = ref expandedBlock.m_Size;
			size += expandedOffset + math.select(default(int2), int2.op_Implicit(6), ((int4)(ref validArea.m_Area)).yw == block.m_Size);
			float3 val = default(float3);
			((float3)(ref val))._002Ector(0f - block.m_Direction.y, 0f, block.m_Direction.x);
			float3 val2 = default(float3);
			((float3)(ref val2))._002Ector(0f - block.m_Direction.x, 0f, 0f - block.m_Direction.y);
			float2 val3 = float2.op_Implicit(expandedBlock.m_Size - (expandedOffset << 1) - block.m_Size) * 4f;
			ref float3 position = ref expandedBlock.m_Position;
			position += val * val3.x + val2 * val3.y;
			NativeArray<Cell> val4 = default(NativeArray<Cell>);
			val4._002Ector(expandedBlock.m_Size.x * expandedBlock.m_Size.y, (Allocator)2, (NativeArrayOptions)1);
			int2 val5 = default(int2);
			val5.y = 0;
			while (val5.y < block.m_Size.y)
			{
				val5.x = 0;
				while (val5.x < block.m_Size.x)
				{
					int2 val6 = val5 + expandedOffset;
					int num = val5.y * block.m_Size.x + val5.x;
					int num2 = val6.y * expandedBlock.m_Size.x + val6.x;
					val4[num2] = cells[num];
					val5.x++;
				}
				val5.y++;
			}
			Quad2 val7 = ZoneUtils.CalculateCorners(expandedBlock);
			Iterator iterator = new Iterator
			{
				m_Entity = entity,
				m_Block = expandedBlock,
				m_ValidArea = validArea,
				m_BuildOrder = buildOrder,
				m_Bounds = MathUtils.Bounds(val7),
				m_Quad = val7,
				m_BlockData = m_BlockData,
				m_ValidAreaData = m_ValidAreaData,
				m_BuildOrderData = m_BuildOrderData,
				m_CellData = m_Cells,
				m_Cells = val4
			};
			m_SearchTree.Iterate<Iterator>(ref iterator, 0);
			return val4;
		}
	}

	[BurstCompile]
	public struct UpdateBoundsJob : IJob
	{
		public NativeQueue<Bounds2> m_BoundsQueue;

		public NativeList<Bounds2> m_BoundsList;

		public void Execute()
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			int count = m_BoundsQueue.Count;
			if (count != 0)
			{
				m_BoundsList.Capacity = math.max(m_BoundsList.Capacity, m_BoundsList.Length + count);
				for (int i = 0; i < count; i++)
				{
					ref NativeList<Bounds2> boundsList = ref m_BoundsList;
					Bounds2 val = m_BoundsQueue.Dequeue();
					boundsList.Add(ref val);
				}
			}
		}
	}
}
