using Colossal.Mathematics;
using Game.Prefabs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Game.Zones;

public static class CellOverlapJobs
{
	[BurstCompile]
	public struct CheckBlockOverlapJob : IJobParallelForDefer
	{
		private struct CellReduction
		{
			public Entity m_BlockEntity;

			public Entity m_LeftNeightbor;

			public Entity m_RightNeightbor;

			public CellFlags m_Flag;

			public ZonePrefabs m_ZonePrefabs;

			public ComponentLookup<Block> m_BlockDataFromEntity;

			public ComponentLookup<ValidArea> m_ValidAreaDataFromEntity;

			public ComponentLookup<BuildOrder> m_BuildOrderDataFromEntity;

			public ComponentLookup<ZoneData> m_ZoneData;

			public BufferLookup<Cell> m_CellsFromEntity;

			private Block m_BlockData;

			private Block m_LeftBlockData;

			private Block m_RightBlockData;

			private ValidArea m_ValidAreaData;

			private ValidArea m_LeftValidAreaData;

			private ValidArea m_RightValidAreaData;

			private BuildOrder m_BuildOrderData;

			private BuildOrder m_LeftBuildOrderData;

			private BuildOrder m_RightBuildOrderData;

			private DynamicBuffer<Cell> m_Cells;

			private DynamicBuffer<Cell> m_LeftCells;

			private DynamicBuffer<Cell> m_RightCells;

			public void Clear()
			{
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				m_BlockData = m_BlockDataFromEntity[m_BlockEntity];
				m_ValidAreaData = m_ValidAreaDataFromEntity[m_BlockEntity];
				m_Cells = m_CellsFromEntity[m_BlockEntity];
				for (int i = m_ValidAreaData.m_Area.x; i < m_ValidAreaData.m_Area.y; i++)
				{
					for (int j = m_ValidAreaData.m_Area.z; j < m_ValidAreaData.m_Area.w; j++)
					{
						int num = j * m_BlockData.m_Size.x + i;
						Cell cell = m_Cells[num];
						if ((cell.m_State & m_Flag) != CellFlags.None)
						{
							cell.m_State &= (CellFlags)(ushort)(~(int)m_Flag);
							m_Cells[num] = cell;
						}
					}
				}
			}

			public void Perform()
			{
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0052: Unknown result type (might be due to invalid IL or missing references)
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0062: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00de: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
				//IL_0109: Unknown result type (might be due to invalid IL or missing references)
				//IL_0120: Unknown result type (might be due to invalid IL or missing references)
				//IL_0137: Unknown result type (might be due to invalid IL or missing references)
				//IL_013c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0141: Unknown result type (might be due to invalid IL or missing references)
				//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_031c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0321: Unknown result type (might be due to invalid IL or missing references)
				//IL_0330: Unknown result type (might be due to invalid IL or missing references)
				//IL_0335: Unknown result type (might be due to invalid IL or missing references)
				//IL_033d: Unknown result type (might be due to invalid IL or missing references)
				//IL_034a: Unknown result type (might be due to invalid IL or missing references)
				//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_0456: Unknown result type (might be due to invalid IL or missing references)
				//IL_046c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0471: Unknown result type (might be due to invalid IL or missing references)
				//IL_0489: Unknown result type (might be due to invalid IL or missing references)
				//IL_0493: Unknown result type (might be due to invalid IL or missing references)
				//IL_0498: Unknown result type (might be due to invalid IL or missing references)
				//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
				//IL_040e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0413: Unknown result type (might be due to invalid IL or missing references)
				//IL_042b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0435: Unknown result type (might be due to invalid IL or missing references)
				//IL_043a: Unknown result type (might be due to invalid IL or missing references)
				m_BlockData = m_BlockDataFromEntity[m_BlockEntity];
				m_ValidAreaData = m_ValidAreaDataFromEntity[m_BlockEntity];
				m_BuildOrderData = m_BuildOrderDataFromEntity[m_BlockEntity];
				m_Cells = m_CellsFromEntity[m_BlockEntity];
				if (m_LeftNeightbor != Entity.Null)
				{
					m_LeftBlockData = m_BlockDataFromEntity[m_LeftNeightbor];
					m_LeftValidAreaData = m_ValidAreaDataFromEntity[m_LeftNeightbor];
					m_LeftBuildOrderData = m_BuildOrderDataFromEntity[m_LeftNeightbor];
					m_LeftCells = m_CellsFromEntity[m_LeftNeightbor];
				}
				else
				{
					m_LeftBlockData = default(Block);
				}
				if (m_RightNeightbor != Entity.Null)
				{
					m_RightBlockData = m_BlockDataFromEntity[m_RightNeightbor];
					m_RightValidAreaData = m_ValidAreaDataFromEntity[m_RightNeightbor];
					m_RightBuildOrderData = m_BuildOrderDataFromEntity[m_RightNeightbor];
					m_RightCells = m_CellsFromEntity[m_RightNeightbor];
				}
				else
				{
					m_RightBlockData = default(Block);
				}
				CellFlags cellFlags = m_Flag | CellFlags.Blocked;
				for (int i = m_ValidAreaData.m_Area.x; i < m_ValidAreaData.m_Area.y; i++)
				{
					Cell cell = m_Cells[i];
					Cell cell2 = m_Cells[m_BlockData.m_Size.x + i];
					if (((cell.m_State & cellFlags) == 0) & ((cell2.m_State & cellFlags) == m_Flag))
					{
						cell.m_State |= m_Flag;
						m_Cells[i] = cell;
					}
					for (int j = m_ValidAreaData.m_Area.z + 1; j < m_ValidAreaData.m_Area.w; j++)
					{
						int num = j * m_BlockData.m_Size.x + i;
						Cell cell3 = m_Cells[num];
						if (((cell3.m_State & cellFlags) == 0) & ((cell.m_State & cellFlags) == m_Flag))
						{
							cell3.m_State |= m_Flag;
							m_Cells[num] = cell3;
						}
						cell = cell3;
					}
				}
				int num2 = m_ValidAreaData.m_Area.x;
				int num3 = m_ValidAreaData.m_Area.y - 1;
				ValidArea validArea = default(ValidArea);
				((int4)(ref validArea.m_Area)).xz = m_BlockData.m_Size;
				while (num3 >= m_ValidAreaData.m_Area.x)
				{
					if (m_Flag == CellFlags.Occupied)
					{
						Cell cell4 = m_Cells[num2];
						Cell cell5 = m_Cells[num3];
						Entity val = m_ZonePrefabs[cell4.m_Zone];
						Entity val2 = m_ZonePrefabs[cell5.m_Zone];
						ZoneData zoneData = m_ZoneData[val];
						ZoneData zoneData2 = m_ZoneData[val2];
						if ((zoneData.m_ZoneFlags & ZoneFlags.SupportNarrow) == 0)
						{
							int newDepth = CalculateLeftDepth(num2, cell4.m_Zone);
							ReduceDepth(num2, newDepth);
						}
						if ((zoneData2.m_ZoneFlags & ZoneFlags.SupportNarrow) == 0)
						{
							int newDepth2 = CalculateRightDepth(num3, cell5.m_Zone);
							ReduceDepth(num3, newDepth2);
						}
					}
					else
					{
						int num4 = CalculateLeftDepth(num2, ZoneType.None);
						ReduceDepth(num2, num4);
						int num5 = CalculateRightDepth(num3, ZoneType.None);
						ReduceDepth(num3, num5);
						if (num3 <= num2 && m_Flag == CellFlags.Blocked)
						{
							if (num4 != 0 && num2 != num3)
							{
								((int4)(ref validArea.m_Area)).xz = math.min(((int4)(ref validArea.m_Area)).xz, new int2(num2, m_ValidAreaData.m_Area.z));
								((int4)(ref validArea.m_Area)).yw = math.max(((int4)(ref validArea.m_Area)).yw, new int2(num2 + 1, num4));
							}
							if (num5 != 0)
							{
								((int4)(ref validArea.m_Area)).xz = math.min(((int4)(ref validArea.m_Area)).xz, new int2(num3, m_ValidAreaData.m_Area.z));
								((int4)(ref validArea.m_Area)).yw = math.max(((int4)(ref validArea.m_Area)).yw, new int2(num3 + 1, num5));
							}
						}
					}
					num2++;
					num3--;
				}
				if (m_Flag == CellFlags.Blocked)
				{
					m_ValidAreaDataFromEntity[m_BlockEntity] = validArea;
				}
			}

			private int CalculateLeftDepth(int x, ZoneType zoneType)
			{
				int depth = GetDepth(x - 1, zoneType);
				int depth2 = GetDepth(x, zoneType);
				if (depth2 <= depth)
				{
					return depth2;
				}
				int depth3 = GetDepth(x - 2, zoneType);
				if (depth != depth3 && depth != 0)
				{
					return depth;
				}
				int depth4 = GetDepth(x + 1, zoneType);
				if (depth4 - depth2 < depth2 - depth)
				{
					return math.min(math.max(depth, depth4), depth2);
				}
				if (GetDepth(x + 2, zoneType) != depth4)
				{
					return math.min(math.max(depth, depth4), depth2);
				}
				return depth;
			}

			private int CalculateRightDepth(int x, ZoneType zoneType)
			{
				int depth = GetDepth(x + 1, zoneType);
				int depth2 = GetDepth(x, zoneType);
				if (depth2 <= depth)
				{
					return depth2;
				}
				int depth3 = GetDepth(x + 2, zoneType);
				if (depth != depth3 && depth != 0)
				{
					return depth;
				}
				int depth4 = GetDepth(x - 1, zoneType);
				if (depth4 - depth2 < depth2 - depth)
				{
					return math.min(math.max(depth4, depth), depth2);
				}
				if (GetDepth(x - 2, zoneType) != depth4)
				{
					return math.min(math.max(depth4, depth), depth2);
				}
				return depth;
			}

			private int GetDepth(int x, ZoneType zoneType)
			{
				//IL_0153: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_0130: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
				if (x < 0)
				{
					x += m_LeftBlockData.m_Size.x;
					if (x < 0)
					{
						return 0;
					}
					if ((m_BuildOrderData.m_Order < m_LeftBuildOrderData.m_Order) & (m_Flag == CellFlags.Blocked))
					{
						return GetDepth(m_BlockData, m_ValidAreaData, m_Cells, 0, m_Flag | CellFlags.Blocked, zoneType);
					}
					return GetDepth(m_LeftBlockData, m_LeftValidAreaData, m_LeftCells, x, m_Flag | CellFlags.Blocked, zoneType);
				}
				if (x >= m_BlockData.m_Size.x)
				{
					x -= m_BlockData.m_Size.x;
					if (x >= m_RightBlockData.m_Size.x)
					{
						return 0;
					}
					if ((m_BuildOrderData.m_Order < m_RightBuildOrderData.m_Order) & (m_Flag == CellFlags.Blocked))
					{
						return GetDepth(m_BlockData, m_ValidAreaData, m_Cells, m_BlockData.m_Size.x - 1, m_Flag | CellFlags.Blocked, zoneType);
					}
					return GetDepth(m_RightBlockData, m_RightValidAreaData, m_RightCells, x, m_Flag | CellFlags.Blocked, zoneType);
				}
				return GetDepth(m_BlockData, m_ValidAreaData, m_Cells, x, m_Flag | CellFlags.Blocked, zoneType);
			}

			private int GetDepth(Block blockData, ValidArea validAreaData, DynamicBuffer<Cell> cells, int x, CellFlags flags, ZoneType zoneType)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_007c: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				int i = validAreaData.m_Area.z;
				int num = x;
				if (m_Flag == CellFlags.Occupied)
				{
					for (; i < validAreaData.m_Area.w; i++)
					{
						if ((cells[num].m_State & flags) != CellFlags.None)
						{
							break;
						}
						if (!cells[num].m_Zone.Equals(zoneType))
						{
							break;
						}
						num += blockData.m_Size.x;
					}
				}
				else
				{
					for (; i < validAreaData.m_Area.w; i++)
					{
						if ((cells[num].m_State & flags) != CellFlags.None)
						{
							break;
						}
						num += blockData.m_Size.x;
					}
				}
				return i;
			}

			private void ReduceDepth(int x, int newDepth)
			{
				CellFlags cellFlags = m_Flag | CellFlags.Blocked;
				int num = m_BlockData.m_Size.x * newDepth + x;
				for (int i = newDepth; i < m_ValidAreaData.m_Area.w; i++)
				{
					Cell cell = m_Cells[num];
					if ((cell.m_State & cellFlags) != CellFlags.None)
					{
						break;
					}
					cell.m_State |= m_Flag;
					m_Cells[num] = cell;
					num += m_BlockData.m_Size.x;
				}
			}
		}

		private struct OverlapIterator
		{
			public Entity m_BlockEntity;

			public Quad2 m_Quad;

			public Bounds2 m_Bounds;

			public Block m_BlockData;

			public ValidArea m_ValidAreaData;

			public BuildOrder m_BuildOrderData;

			public DynamicBuffer<Cell> m_Cells;

			public ComponentLookup<Block> m_BlockDataFromEntity;

			public ComponentLookup<ValidArea> m_ValidAreaDataFromEntity;

			public ComponentLookup<BuildOrder> m_BuildOrderDataFromEntity;

			public BufferLookup<Cell> m_CellsFromEntity;

			public bool m_CheckSharing;

			public bool m_CheckBlocking;

			public bool m_CheckDepth;

			private Block m_BlockData2;

			private ValidArea m_ValidAreaData2;

			private BuildOrder m_BuildOrderData2;

			private DynamicBuffer<Cell> m_Cells2;

			public void Iterate(Entity blockEntity2)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00de: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0104: Unknown result type (might be due to invalid IL or missing references)
				m_BlockData2 = m_BlockDataFromEntity[blockEntity2];
				m_ValidAreaData2 = m_ValidAreaDataFromEntity[blockEntity2];
				m_BuildOrderData2 = m_BuildOrderDataFromEntity[blockEntity2];
				m_Cells2 = m_CellsFromEntity[blockEntity2];
				if (m_ValidAreaData2.m_Area.y <= m_ValidAreaData2.m_Area.x)
				{
					return;
				}
				if (ZoneUtils.CanShareCells(m_BlockData, m_BlockData2, m_BuildOrderData, m_BuildOrderData2))
				{
					if (!m_CheckSharing)
					{
						return;
					}
					m_CheckDepth = false;
				}
				else
				{
					if (m_CheckSharing)
					{
						return;
					}
					m_CheckDepth = math.dot(m_BlockData.m_Direction, m_BlockData2.m_Direction) < -0.6946584f;
				}
				Quad2 val = ZoneUtils.CalculateCorners(m_BlockData2, m_ValidAreaData2);
				CheckOverlapX1(m_Bounds, MathUtils.Bounds(val), m_Quad, val, m_ValidAreaData.m_Area, m_ValidAreaData2.m_Area);
			}

			private void CheckOverlapX1(Bounds2 bounds1, Bounds2 bounds2, Quad2 quad1, Quad2 quad2, int4 xxzz1, int4 xxzz2)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00df: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
				//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
				if (xxzz1.y - xxzz1.x >= 2)
				{
					int4 val = xxzz1;
					int4 xxzz3 = xxzz1;
					val.y = xxzz1.x + xxzz1.y >> 1;
					xxzz3.x = val.y;
					Quad2 val2 = quad1;
					Quad2 val3 = quad1;
					float num = (float)(val.y - xxzz1.x) / (float)(xxzz1.y - xxzz1.x);
					val2.b = math.lerp(quad1.a, quad1.b, num);
					val2.c = math.lerp(quad1.d, quad1.c, num);
					val3.a = val2.b;
					val3.d = val2.c;
					Bounds2 val4 = MathUtils.Bounds(val2);
					Bounds2 val5 = MathUtils.Bounds(val3);
					if (MathUtils.Intersect(val4, bounds2))
					{
						CheckOverlapZ1(val4, bounds2, val2, quad2, val, xxzz2);
					}
					if (MathUtils.Intersect(val5, bounds2))
					{
						CheckOverlapZ1(val5, bounds2, val3, quad2, xxzz3, xxzz2);
					}
				}
				else
				{
					CheckOverlapZ1(bounds1, bounds2, quad1, quad2, xxzz1, xxzz2);
				}
			}

			private void CheckOverlapZ1(Bounds2 bounds1, Bounds2 bounds2, Quad2 quad1, Quad2 quad2, int4 xxzz1, int4 xxzz2)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00df: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
				//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
				if (xxzz1.w - xxzz1.z >= 2)
				{
					int4 val = xxzz1;
					int4 xxzz3 = xxzz1;
					val.w = xxzz1.z + xxzz1.w >> 1;
					xxzz3.z = val.w;
					Quad2 val2 = quad1;
					Quad2 val3 = quad1;
					float num = (float)(val.w - xxzz1.z) / (float)(xxzz1.w - xxzz1.z);
					val2.d = math.lerp(quad1.a, quad1.d, num);
					val2.c = math.lerp(quad1.b, quad1.c, num);
					val3.a = val2.d;
					val3.b = val2.c;
					Bounds2 val4 = MathUtils.Bounds(val2);
					Bounds2 val5 = MathUtils.Bounds(val3);
					if (MathUtils.Intersect(val4, bounds2))
					{
						CheckOverlapX2(val4, bounds2, val2, quad2, val, xxzz2);
					}
					if (MathUtils.Intersect(val5, bounds2))
					{
						CheckOverlapX2(val5, bounds2, val3, quad2, xxzz3, xxzz2);
					}
				}
				else
				{
					CheckOverlapX2(bounds1, bounds2, quad1, quad2, xxzz1, xxzz2);
				}
			}

			private void CheckOverlapX2(Bounds2 bounds1, Bounds2 bounds2, Quad2 quad1, Quad2 quad2, int4 xxzz1, int4 xxzz2)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_0101: Unknown result type (might be due to invalid IL or missing references)
				//IL_0103: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0071: Unknown result type (might be due to invalid IL or missing references)
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				//IL_007a: Unknown result type (might be due to invalid IL or missing references)
				//IL_007f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0086: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				//IL_008f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0096: Unknown result type (might be due to invalid IL or missing references)
				//IL_009b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00da: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
				if (xxzz2.y - xxzz2.x >= 2)
				{
					int4 val = xxzz2;
					int4 xxzz3 = xxzz2;
					val.y = xxzz2.x + xxzz2.y >> 1;
					xxzz3.x = val.y;
					Quad2 val2 = quad2;
					Quad2 val3 = quad2;
					float num = (float)(val.y - xxzz2.x) / (float)(xxzz2.y - xxzz2.x);
					val2.b = math.lerp(quad2.a, quad2.b, num);
					val2.c = math.lerp(quad2.d, quad2.c, num);
					val3.a = val2.b;
					val3.d = val2.c;
					Bounds2 val4 = MathUtils.Bounds(val2);
					Bounds2 val5 = MathUtils.Bounds(val3);
					if (MathUtils.Intersect(bounds1, val4))
					{
						CheckOverlapZ2(bounds1, val4, quad1, val2, xxzz1, val);
					}
					if (MathUtils.Intersect(bounds1, val5))
					{
						CheckOverlapZ2(bounds1, val5, quad1, val3, xxzz1, xxzz3);
					}
				}
				else
				{
					CheckOverlapZ2(bounds1, bounds2, quad1, quad2, xxzz1, xxzz2);
				}
			}

			private void CheckOverlapZ2(Bounds2 bounds1, Bounds2 bounds2, Quad2 quad1, Quad2 quad2, int4 xxzz1, int4 xxzz2)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0100: Unknown result type (might be due to invalid IL or missing references)
				//IL_0107: Unknown result type (might be due to invalid IL or missing references)
				//IL_010c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0112: Unknown result type (might be due to invalid IL or missing references)
				//IL_011e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0125: Unknown result type (might be due to invalid IL or missing references)
				//IL_012a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0130: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0071: Unknown result type (might be due to invalid IL or missing references)
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				//IL_007a: Unknown result type (might be due to invalid IL or missing references)
				//IL_007f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0086: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				//IL_008f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0096: Unknown result type (might be due to invalid IL or missing references)
				//IL_009b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_014d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0165: Unknown result type (might be due to invalid IL or missing references)
				//IL_016f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0187: Unknown result type (might be due to invalid IL or missing references)
				//IL_013e: Unknown result type (might be due to invalid IL or missing references)
				//IL_013f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0140: Unknown result type (might be due to invalid IL or missing references)
				//IL_0141: Unknown result type (might be due to invalid IL or missing references)
				//IL_0143: Unknown result type (might be due to invalid IL or missing references)
				//IL_0145: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00da: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
				//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_01db: Unknown result type (might be due to invalid IL or missing references)
				//IL_034b: Unknown result type (might be due to invalid IL or missing references)
				//IL_034c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0351: Unknown result type (might be due to invalid IL or missing references)
				//IL_0353: Unknown result type (might be due to invalid IL or missing references)
				//IL_0358: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
				//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
				//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
				//IL_0302: Unknown result type (might be due to invalid IL or missing references)
				//IL_0307: Unknown result type (might be due to invalid IL or missing references)
				//IL_0309: Unknown result type (might be due to invalid IL or missing references)
				//IL_030a: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
				//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
				//IL_0374: Unknown result type (might be due to invalid IL or missing references)
				//IL_026e: Unknown result type (might be due to invalid IL or missing references)
				if (xxzz2.w - xxzz2.z >= 2)
				{
					int4 val = xxzz2;
					int4 xxzz3 = xxzz2;
					val.w = xxzz2.z + xxzz2.w >> 1;
					xxzz3.z = val.w;
					Quad2 val2 = quad2;
					Quad2 val3 = quad2;
					float num = (float)(val.w - xxzz2.z) / (float)(xxzz2.w - xxzz2.z);
					val2.d = math.lerp(quad2.a, quad2.d, num);
					val2.c = math.lerp(quad2.b, quad2.c, num);
					val3.a = val2.d;
					val3.b = val2.c;
					Bounds2 val4 = MathUtils.Bounds(val2);
					Bounds2 val5 = MathUtils.Bounds(val3);
					if (MathUtils.Intersect(bounds1, val4))
					{
						CheckOverlapX1(bounds1, val4, quad1, val2, xxzz1, val);
					}
					if (MathUtils.Intersect(bounds1, val5))
					{
						CheckOverlapX1(bounds1, val5, quad1, val3, xxzz1, xxzz3);
					}
					return;
				}
				if (math.any(((int4)(ref xxzz1)).yw - ((int4)(ref xxzz1)).xz >= 2) | math.any(((int4)(ref xxzz2)).yw - ((int4)(ref xxzz2)).xz >= 2))
				{
					CheckOverlapX1(bounds1, bounds2, quad1, quad2, xxzz1, xxzz2);
					return;
				}
				int num2 = xxzz1.z * m_BlockData.m_Size.x + xxzz1.x;
				int num3 = xxzz2.z * m_BlockData2.m_Size.x + xxzz2.x;
				Cell cell = m_Cells[num2];
				Cell cell2 = m_Cells2[num3];
				if (((cell.m_State | cell2.m_State) & CellFlags.Blocked) != CellFlags.None)
				{
					return;
				}
				if (m_CheckSharing)
				{
					if (math.lengthsq(MathUtils.Center(quad1) - MathUtils.Center(quad2)) < 16f)
					{
						if (CheckPriority(cell, cell2, xxzz1.z, xxzz2.z, m_BuildOrderData.m_Order, m_BuildOrderData2.m_Order) && (cell2.m_State & CellFlags.Shared) == 0)
						{
							cell.m_State |= CellFlags.Shared;
							cell.m_State = (cell.m_State & ~CellFlags.Overridden) | (cell2.m_State & CellFlags.Overridden);
							cell.m_Zone = cell2.m_Zone;
						}
						if ((cell2.m_State & CellFlags.Roadside) != CellFlags.None && xxzz2.z == 0)
						{
							cell.m_State |= ZoneUtils.GetRoadDirection(m_BlockData, m_BlockData2);
						}
						cell.m_State &= ~CellFlags.Occupied | (cell2.m_State & CellFlags.Occupied);
						m_Cells[num2] = cell;
					}
				}
				else if (CheckPriority(cell, cell2, xxzz1.z, xxzz2.z, m_BuildOrderData.m_Order, m_BuildOrderData2.m_Order))
				{
					quad1 = MathUtils.Expand(quad1, -0.01f);
					quad2 = MathUtils.Expand(quad2, -0.01f);
					if (MathUtils.Intersect(quad1, quad2))
					{
						cell.m_State = (CellFlags)((uint)(cell.m_State & ~CellFlags.Shared) | (uint)(m_CheckBlocking ? 1 : 128));
						m_Cells[num2] = cell;
					}
				}
				else if (math.lengthsq(MathUtils.Center(quad1) - MathUtils.Center(quad2)) < 64f && (cell2.m_State & CellFlags.Roadside) != CellFlags.None && xxzz2.z == 0)
				{
					cell.m_State |= ZoneUtils.GetRoadDirection(m_BlockData, m_BlockData2);
					m_Cells[num2] = cell;
				}
			}

			private bool CheckPriority(Cell cell1, Cell cell2, int depth1, int depth2, uint order1, uint order2)
			{
				if ((cell2.m_State & CellFlags.Updating) == 0)
				{
					return (cell2.m_State & CellFlags.Visible) != 0;
				}
				if (m_CheckBlocking)
				{
					return ((uint)cell1.m_State & (uint)(ushort)(~(int)cell2.m_State) & 0x80) != 0;
				}
				if (m_CheckDepth)
				{
					if (cell1.m_Zone.Equals(ZoneType.None) != cell2.m_Zone.Equals(ZoneType.None))
					{
						return cell1.m_Zone.Equals(ZoneType.None);
					}
					if (cell1.m_Zone.Equals(ZoneType.None) && ((cell1.m_State | cell2.m_State) & CellFlags.Overridden) == 0 && math.max(0, depth1 - 1) != math.max(0, depth2 - 1))
					{
						return depth2 < depth1;
					}
				}
				if (((cell1.m_State ^ cell2.m_State) & CellFlags.Visible) != CellFlags.None)
				{
					return (cell2.m_State & CellFlags.Visible) != 0;
				}
				return order2 < order1;
			}
		}

		[NativeDisableParallelForRestriction]
		public NativeArray<CellCheckHelpers.BlockOverlap> m_BlockOverlaps;

		[ReadOnly]
		public NativeArray<CellCheckHelpers.OverlapGroup> m_OverlapGroups;

		[ReadOnly]
		public ZonePrefabs m_ZonePrefabs;

		[ReadOnly]
		public ComponentLookup<Block> m_BlockData;

		[ReadOnly]
		public ComponentLookup<BuildOrder> m_BuildOrderData;

		[ReadOnly]
		public ComponentLookup<ZoneData> m_ZoneData;

		[NativeDisableParallelForRestriction]
		public BufferLookup<Cell> m_Cells;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<ValidArea> m_ValidAreaData;

		public void Execute(int index)
		{
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0552: Unknown result type (might be due to invalid IL or missing references)
			//IL_0557: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0510: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0401: Unknown result type (might be due to invalid IL or missing references)
			//IL_0590: Unknown result type (might be due to invalid IL or missing references)
			//IL_0595: Unknown result type (might be due to invalid IL or missing references)
			//IL_059d: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_05af: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_051e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0523: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_0441: Unknown result type (might be due to invalid IL or missing references)
			//IL_0446: Unknown result type (might be due to invalid IL or missing references)
			//IL_044f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0454: Unknown result type (might be due to invalid IL or missing references)
			//IL_045d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0462: Unknown result type (might be due to invalid IL or missing references)
			//IL_0471: Unknown result type (might be due to invalid IL or missing references)
			//IL_048a: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04db: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0533: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0654: Unknown result type (might be due to invalid IL or missing references)
			//IL_0659: Unknown result type (might be due to invalid IL or missing references)
			//IL_0661: Unknown result type (might be due to invalid IL or missing references)
			//IL_0666: Unknown result type (might be due to invalid IL or missing references)
			//IL_066e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0673: Unknown result type (might be due to invalid IL or missing references)
			//IL_067b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0680: Unknown result type (might be due to invalid IL or missing references)
			//IL_0688: Unknown result type (might be due to invalid IL or missing references)
			//IL_068d: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0604: Unknown result type (might be due to invalid IL or missing references)
			//IL_060d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0612: Unknown result type (might be due to invalid IL or missing references)
			//IL_061b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0620: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0715: Unknown result type (might be due to invalid IL or missing references)
			//IL_071a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0722: Unknown result type (might be due to invalid IL or missing references)
			//IL_0727: Unknown result type (might be due to invalid IL or missing references)
			//IL_072f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0734: Unknown result type (might be due to invalid IL or missing references)
			//IL_073c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0741: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06db: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_076c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0773: Unknown result type (might be due to invalid IL or missing references)
			//IL_0828: Unknown result type (might be due to invalid IL or missing references)
			//IL_0839: Unknown result type (might be due to invalid IL or missing references)
			//IL_0786: Unknown result type (might be due to invalid IL or missing references)
			//IL_078b: Unknown result type (might be due to invalid IL or missing references)
			//IL_079a: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0804: Unknown result type (might be due to invalid IL or missing references)
			//IL_0809: Unknown result type (might be due to invalid IL or missing references)
			//IL_0812: Unknown result type (might be due to invalid IL or missing references)
			//IL_0817: Unknown result type (might be due to invalid IL or missing references)
			//IL_081c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0847: Unknown result type (might be due to invalid IL or missing references)
			//IL_084c: Unknown result type (might be due to invalid IL or missing references)
			//IL_085c: Unknown result type (might be due to invalid IL or missing references)
			CellCheckHelpers.OverlapGroup overlapGroup = m_OverlapGroups[index];
			CellCheckHelpers.BlockOverlap blockOverlap = default(CellCheckHelpers.BlockOverlap);
			int num = 0;
			Block block = default(Block);
			BuildOrder buildOrder = default(BuildOrder);
			for (int i = overlapGroup.m_StartIndex; i < overlapGroup.m_EndIndex; i++)
			{
				CellCheckHelpers.BlockOverlap blockOverlap2 = m_BlockOverlaps[i];
				if (blockOverlap2.m_Block != blockOverlap.m_Block)
				{
					if (blockOverlap.m_Block != Entity.Null)
					{
						m_BlockOverlaps[num] = blockOverlap;
					}
					blockOverlap = blockOverlap2;
					num = i;
					block = m_BlockData[blockOverlap2.m_Block];
					_ = m_ValidAreaData[blockOverlap2.m_Block];
					buildOrder = m_BuildOrderData[blockOverlap2.m_Block];
					_ = m_Cells[blockOverlap2.m_Block];
				}
				if (!(blockOverlap2.m_Other != Entity.Null))
				{
					continue;
				}
				Block block2 = m_BlockData[blockOverlap2.m_Other];
				BuildOrder buildOrder2 = m_BuildOrderData[blockOverlap2.m_Other];
				if (ZoneUtils.IsNeighbor(block, block2, buildOrder, buildOrder2))
				{
					if (math.dot(((float3)(ref block2.m_Position)).xz - ((float3)(ref block.m_Position)).xz, MathUtils.Right(block.m_Direction)) > 0f)
					{
						blockOverlap.m_Left = blockOverlap2.m_Other;
					}
					else
					{
						blockOverlap.m_Right = blockOverlap2.m_Other;
					}
				}
			}
			if (blockOverlap.m_Block != Entity.Null)
			{
				m_BlockOverlaps[num] = blockOverlap;
			}
			OverlapIterator overlapIterator = new OverlapIterator
			{
				m_BlockDataFromEntity = m_BlockData,
				m_ValidAreaDataFromEntity = m_ValidAreaData,
				m_BuildOrderDataFromEntity = m_BuildOrderData,
				m_CellsFromEntity = m_Cells
			};
			CellReduction cellReduction = new CellReduction
			{
				m_BlockDataFromEntity = m_BlockData,
				m_ValidAreaDataFromEntity = m_ValidAreaData,
				m_BuildOrderDataFromEntity = m_BuildOrderData,
				m_CellsFromEntity = m_Cells,
				m_Flag = CellFlags.Redundant
			};
			for (int j = overlapGroup.m_StartIndex; j < overlapGroup.m_EndIndex; j++)
			{
				CellCheckHelpers.BlockOverlap blockOverlap3 = m_BlockOverlaps[j];
				if (blockOverlap3.m_Block != overlapIterator.m_BlockEntity)
				{
					if (cellReduction.m_BlockEntity != Entity.Null)
					{
						cellReduction.Perform();
					}
					cellReduction.m_BlockEntity = blockOverlap3.m_Block;
					cellReduction.m_LeftNeightbor = blockOverlap3.m_Left;
					cellReduction.m_RightNeightbor = blockOverlap3.m_Right;
					overlapIterator.m_BlockEntity = blockOverlap3.m_Block;
					overlapIterator.m_BlockData = m_BlockData[overlapIterator.m_BlockEntity];
					overlapIterator.m_ValidAreaData = m_ValidAreaData[overlapIterator.m_BlockEntity];
					overlapIterator.m_BuildOrderData = m_BuildOrderData[overlapIterator.m_BlockEntity];
					overlapIterator.m_Cells = m_Cells[overlapIterator.m_BlockEntity];
					overlapIterator.m_Quad = ZoneUtils.CalculateCorners(overlapIterator.m_BlockData, overlapIterator.m_ValidAreaData);
					overlapIterator.m_Bounds = MathUtils.Bounds(overlapIterator.m_Quad);
				}
				if (overlapIterator.m_ValidAreaData.m_Area.y > overlapIterator.m_ValidAreaData.m_Area.x && blockOverlap3.m_Other != Entity.Null)
				{
					overlapIterator.Iterate(blockOverlap3.m_Other);
				}
			}
			if (cellReduction.m_BlockEntity != Entity.Null)
			{
				cellReduction.Perform();
			}
			overlapIterator.m_BlockEntity = Entity.Null;
			overlapIterator.m_CheckBlocking = true;
			cellReduction.m_BlockEntity = Entity.Null;
			for (int k = overlapGroup.m_StartIndex; k < overlapGroup.m_EndIndex; k++)
			{
				CellCheckHelpers.BlockOverlap blockOverlap4 = m_BlockOverlaps[k];
				if (blockOverlap4.m_Block != overlapIterator.m_BlockEntity)
				{
					if (cellReduction.m_BlockEntity != Entity.Null)
					{
						cellReduction.m_Flag = CellFlags.Redundant;
						cellReduction.Clear();
						cellReduction.m_Flag = CellFlags.Blocked;
						cellReduction.Perform();
					}
					cellReduction.m_BlockEntity = blockOverlap4.m_Block;
					cellReduction.m_LeftNeightbor = blockOverlap4.m_Left;
					cellReduction.m_RightNeightbor = blockOverlap4.m_Right;
					overlapIterator.m_BlockEntity = blockOverlap4.m_Block;
					overlapIterator.m_BlockData = m_BlockData[overlapIterator.m_BlockEntity];
					overlapIterator.m_ValidAreaData = m_ValidAreaData[overlapIterator.m_BlockEntity];
					overlapIterator.m_BuildOrderData = m_BuildOrderData[overlapIterator.m_BlockEntity];
					overlapIterator.m_Cells = m_Cells[overlapIterator.m_BlockEntity];
					overlapIterator.m_Quad = ZoneUtils.CalculateCorners(overlapIterator.m_BlockData, overlapIterator.m_ValidAreaData);
					overlapIterator.m_Bounds = MathUtils.Bounds(overlapIterator.m_Quad);
				}
				if (overlapIterator.m_ValidAreaData.m_Area.y > overlapIterator.m_ValidAreaData.m_Area.x && blockOverlap4.m_Other != Entity.Null)
				{
					overlapIterator.Iterate(blockOverlap4.m_Other);
				}
			}
			if (cellReduction.m_BlockEntity != Entity.Null)
			{
				cellReduction.m_Flag = CellFlags.Redundant;
				cellReduction.Clear();
				cellReduction.m_Flag = CellFlags.Blocked;
				cellReduction.Perform();
			}
			CellReduction cellReduction2 = new CellReduction
			{
				m_BlockDataFromEntity = m_BlockData,
				m_ValidAreaDataFromEntity = m_ValidAreaData,
				m_BuildOrderDataFromEntity = m_BuildOrderData,
				m_CellsFromEntity = m_Cells,
				m_Flag = CellFlags.Redundant
			};
			for (int l = overlapGroup.m_StartIndex; l < overlapGroup.m_EndIndex; l++)
			{
				CellCheckHelpers.BlockOverlap blockOverlap5 = m_BlockOverlaps[l];
				if (blockOverlap5.m_Block != cellReduction2.m_BlockEntity)
				{
					cellReduction2.m_BlockEntity = blockOverlap5.m_Block;
					cellReduction2.m_LeftNeightbor = blockOverlap5.m_Left;
					cellReduction2.m_RightNeightbor = blockOverlap5.m_Right;
					cellReduction2.Perform();
				}
			}
			CellReduction cellReduction3 = new CellReduction
			{
				m_ZonePrefabs = m_ZonePrefabs,
				m_BlockDataFromEntity = m_BlockData,
				m_ValidAreaDataFromEntity = m_ValidAreaData,
				m_BuildOrderDataFromEntity = m_BuildOrderData,
				m_ZoneData = m_ZoneData,
				m_CellsFromEntity = m_Cells,
				m_Flag = CellFlags.Occupied
			};
			for (int m = overlapGroup.m_StartIndex; m < overlapGroup.m_EndIndex; m++)
			{
				CellCheckHelpers.BlockOverlap blockOverlap6 = m_BlockOverlaps[m];
				if (blockOverlap6.m_Block != cellReduction3.m_BlockEntity)
				{
					cellReduction3.m_BlockEntity = blockOverlap6.m_Block;
					cellReduction3.m_LeftNeightbor = blockOverlap6.m_Left;
					cellReduction3.m_RightNeightbor = blockOverlap6.m_Right;
					cellReduction3.Perform();
				}
			}
			OverlapIterator overlapIterator2 = new OverlapIterator
			{
				m_BlockDataFromEntity = m_BlockData,
				m_ValidAreaDataFromEntity = m_ValidAreaData,
				m_BuildOrderDataFromEntity = m_BuildOrderData,
				m_CellsFromEntity = m_Cells,
				m_CheckSharing = true
			};
			for (int n = overlapGroup.m_StartIndex; n < overlapGroup.m_EndIndex; n++)
			{
				CellCheckHelpers.BlockOverlap blockOverlap7 = m_BlockOverlaps[n];
				if (blockOverlap7.m_Block != overlapIterator2.m_BlockEntity)
				{
					overlapIterator2.m_BlockEntity = blockOverlap7.m_Block;
					overlapIterator2.m_BlockData = m_BlockData[overlapIterator2.m_BlockEntity];
					overlapIterator2.m_ValidAreaData = m_ValidAreaData[overlapIterator2.m_BlockEntity];
					overlapIterator2.m_BuildOrderData = m_BuildOrderData[overlapIterator2.m_BlockEntity];
					overlapIterator2.m_Cells = m_Cells[overlapIterator2.m_BlockEntity];
					overlapIterator2.m_Quad = ZoneUtils.CalculateCorners(overlapIterator2.m_BlockData, overlapIterator2.m_ValidAreaData);
					overlapIterator2.m_Bounds = MathUtils.Bounds(overlapIterator2.m_Quad);
				}
				if (overlapIterator2.m_ValidAreaData.m_Area.y > overlapIterator2.m_ValidAreaData.m_Area.x && blockOverlap7.m_Other != Entity.Null)
				{
					overlapIterator2.Iterate(blockOverlap7.m_Other);
				}
			}
		}
	}
}
