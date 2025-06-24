using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Common;
using Game.Prefabs;
using Game.Zones;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class GenerateZonesSystem : GameSystemBase
{
	private struct CellData
	{
		public int2 m_Location;

		public ZoneType m_ZoneType;
	}

	private struct BaseCell
	{
		public Entity m_Block;

		public int2 m_Location;
	}

	[BurstCompile]
	private struct FillBlocksListJob : IJobChunk
	{
		private struct MarqueeIterator : INativeQuadTreeIterator<Entity, Bounds2>, IUnsafeQuadTreeIterator<Entity, Bounds2>
		{
			public Bounds2 m_Bounds;

			public Quad2 m_Quad;

			public ZoneType m_NewZoneType;

			public bool m_Overwrite;

			public ComponentLookup<Block> m_BlockData;

			public BufferLookup<Cell> m_Cells;

			public NativeParallelMultiHashMap<Entity, CellData> m_ZonedCells;

			public NativeList<Entity> m_ZonedBlocks;

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
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0151: Unknown result type (might be due to invalid IL or missing references)
				//IL_015c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0125: Unknown result type (might be due to invalid IL or missing references)
				//IL_0130: Unknown result type (might be due to invalid IL or missing references)
				//IL_0071: Unknown result type (might be due to invalid IL or missing references)
				//IL_007c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
				//IL_010c: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(bounds, m_Bounds))
				{
					return;
				}
				Block block = m_BlockData[blockEntity];
				Quad2 val = ZoneUtils.CalculateCorners(block);
				if (!MathUtils.Intersect(m_Quad, val))
				{
					return;
				}
				DynamicBuffer<Cell> val2 = m_Cells[blockEntity];
				CellData cellData = default(CellData);
				cellData.m_ZoneType = m_NewZoneType;
				cellData.m_Location.y = 0;
				CellData cellData2 = default(CellData);
				NativeParallelMultiHashMapIterator<Entity> val3 = default(NativeParallelMultiHashMapIterator<Entity>);
				while (cellData.m_Location.y < block.m_Size.y)
				{
					cellData.m_Location.x = 0;
					while (cellData.m_Location.x < block.m_Size.x)
					{
						int num = cellData.m_Location.y * block.m_Size.x + cellData.m_Location.x;
						Cell cell = val2[num];
						if ((cell.m_State & CellFlags.Visible) != CellFlags.None)
						{
							float3 cellPosition = ZoneUtils.GetCellPosition(block, cellData.m_Location);
							if (MathUtils.Intersect(m_Quad, ((float3)(ref cellPosition)).xz) && (m_Overwrite | cell.m_Zone.Equals(ZoneType.None)))
							{
								if (!m_ZonedCells.TryGetFirstValue(blockEntity, ref cellData2, ref val3))
								{
									m_ZonedBlocks.Add(ref blockEntity);
								}
								m_ZonedCells.Add(blockEntity, cellData);
							}
						}
						cellData.m_Location.x++;
					}
					cellData.m_Location.y++;
				}
			}
		}

		private struct BaseLineIterator : INativeQuadTreeIterator<Entity, Bounds2>, IUnsafeQuadTreeIterator<Entity, Bounds2>
		{
			public Segment m_Line;

			public ComponentLookup<Block> m_BlockData;

			public BufferLookup<Cell> m_Cells;

			public NativeList<BaseCell> m_BaseCells;

			public bool Intersect(Bounds2 bounds)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				float2 val = default(float2);
				return MathUtils.Intersect(bounds, m_Line, ref val);
			}

			public void Iterate(Bounds2 bounds, Entity blockEntity)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0049: Unknown result type (might be due to invalid IL or missing references)
				//IL_004e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_0054: Unknown result type (might be due to invalid IL or missing references)
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0061: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				//IL_006d: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				//IL_0084: Unknown result type (might be due to invalid IL or missing references)
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				//IL_008c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0091: Unknown result type (might be due to invalid IL or missing references)
				//IL_0098: Unknown result type (might be due to invalid IL or missing references)
				//IL_009e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00db: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_0104: Unknown result type (might be due to invalid IL or missing references)
				//IL_0109: Unknown result type (might be due to invalid IL or missing references)
				//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_0118: Unknown result type (might be due to invalid IL or missing references)
				//IL_011a: Unknown result type (might be due to invalid IL or missing references)
				//IL_011f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0121: Unknown result type (might be due to invalid IL or missing references)
				//IL_012b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0133: Unknown result type (might be due to invalid IL or missing references)
				//IL_0138: Unknown result type (might be due to invalid IL or missing references)
				//IL_013f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0141: Unknown result type (might be due to invalid IL or missing references)
				//IL_0146: Unknown result type (might be due to invalid IL or missing references)
				//IL_0148: Unknown result type (might be due to invalid IL or missing references)
				//IL_0152: Unknown result type (might be due to invalid IL or missing references)
				//IL_015a: Unknown result type (might be due to invalid IL or missing references)
				//IL_015f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0166: Unknown result type (might be due to invalid IL or missing references)
				//IL_016e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0170: Unknown result type (might be due to invalid IL or missing references)
				//IL_0175: Unknown result type (might be due to invalid IL or missing references)
				//IL_0177: Unknown result type (might be due to invalid IL or missing references)
				//IL_017c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0183: Unknown result type (might be due to invalid IL or missing references)
				//IL_018b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0190: Unknown result type (might be due to invalid IL or missing references)
				//IL_0197: Unknown result type (might be due to invalid IL or missing references)
				//IL_0199: Unknown result type (might be due to invalid IL or missing references)
				//IL_019e: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01be: Unknown result type (might be due to invalid IL or missing references)
				//IL_029c: Unknown result type (might be due to invalid IL or missing references)
				//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
				//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
				//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0207: Unknown result type (might be due to invalid IL or missing references)
				//IL_020f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0214: Unknown result type (might be due to invalid IL or missing references)
				//IL_021e: Unknown result type (might be due to invalid IL or missing references)
				//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
				//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
				//IL_02be: Unknown result type (might be due to invalid IL or missing references)
				//IL_027a: Unknown result type (might be due to invalid IL or missing references)
				//IL_027c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0281: Unknown result type (might be due to invalid IL or missing references)
				//IL_0288: Unknown result type (might be due to invalid IL or missing references)
				//IL_028a: Unknown result type (might be due to invalid IL or missing references)
				//IL_028f: Unknown result type (might be due to invalid IL or missing references)
				//IL_023a: Unknown result type (might be due to invalid IL or missing references)
				//IL_023d: Unknown result type (might be due to invalid IL or missing references)
				//IL_025b: Unknown result type (might be due to invalid IL or missing references)
				//IL_025c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0267: Unknown result type (might be due to invalid IL or missing references)
				//IL_026c: Unknown result type (might be due to invalid IL or missing references)
				float2 val = default(float2);
				if (!MathUtils.Intersect(bounds, m_Line, ref val))
				{
					return;
				}
				Block block = m_BlockData[blockEntity];
				int2 cellIndex = ZoneUtils.GetCellIndex(block, m_Line.a);
				int2 cellIndex2 = ZoneUtils.GetCellIndex(block, m_Line.b);
				int2 val2 = math.max(math.min(cellIndex, cellIndex2), int2.op_Implicit(0));
				int2 val3 = math.min(math.max(cellIndex, cellIndex2), block.m_Size - 1);
				if (!math.all(val3 >= val2))
				{
					return;
				}
				DynamicBuffer<Cell> val4 = m_Cells[blockEntity];
				Quad2 val5 = ZoneUtils.CalculateCorners(block);
				float2 val6 = new float2(1f) / float2.op_Implicit(block.m_Size);
				Quad2 val7 = new Quad2
				{
					a = math.lerp(val5.a, val5.d, (float)val2.y * val6.y),
					b = math.lerp(val5.b, val5.c, (float)val2.y * val6.y)
				};
				for (int i = val2.y; i <= val3.y; i++)
				{
					val7.d = math.lerp(val5.a, val5.d, (float)(i + 1) * val6.y);
					val7.c = math.lerp(val5.b, val5.c, (float)(i + 1) * val6.y);
					Quad2 val8 = new Quad2
					{
						a = math.lerp(val7.a, val7.b, (float)val2.x * val6.x),
						d = math.lerp(val7.d, val7.c, (float)val2.x * val6.x)
					};
					for (int j = val2.x; j <= val3.x; j++)
					{
						val8.b = math.lerp(val7.a, val7.b, (float)(j + 1) * val6.x);
						val8.c = math.lerp(val7.d, val7.c, (float)(j + 1) * val6.x);
						if ((val4[i * block.m_Size.x + j].m_State & CellFlags.Visible) != CellFlags.None && MathUtils.Intersect(val8, m_Line, ref val))
						{
							ref NativeList<BaseCell> reference = ref m_BaseCells;
							BaseCell baseCell = new BaseCell
							{
								m_Block = blockEntity,
								m_Location = new int2(j, i)
							};
							reference.Add(ref baseCell);
						}
						val8.a = val8.b;
						val8.d = val8.c;
					}
					val7.a = val7.d;
					val7.b = val7.c;
				}
			}
		}

		private struct FloodFillIterator : INativeQuadTreeIterator<Entity, Bounds2>, IUnsafeQuadTreeIterator<Entity, Bounds2>
		{
			public Block m_BaseBlockData;

			public float2 m_Position;

			public CellFlags m_StateMask;

			public ZoneType m_OldZoneType;

			public ZoneType m_NewZoneType;

			public bool m_Overwrite;

			public ComponentLookup<Block> m_BlockData;

			public BufferLookup<Cell> m_Cells;

			public NativeParallelMultiHashMap<Entity, CellData> m_ZonedCells;

			public NativeList<Entity> m_ZonedBlocks;

			public int m_FoundCells;

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
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0049: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0054: Unknown result type (might be due to invalid IL or missing references)
				//IL_0059: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				//IL_008f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00da: Unknown result type (might be due to invalid IL or missing references)
				//IL_0121: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(bounds, m_Position))
				{
					return;
				}
				Block block = m_BlockData[blockEntity];
				CellData cellData = default(CellData);
				cellData.m_Location = ZoneUtils.GetCellIndex(block, m_Position);
				cellData.m_ZoneType = m_NewZoneType;
				if (!math.all((cellData.m_Location >= 0) & (cellData.m_Location < block.m_Size)))
				{
					return;
				}
				Cell cell = m_Cells[blockEntity][cellData.m_Location.y * block.m_Size.x + cellData.m_Location.x];
				if ((((cell.m_State & (CellFlags.Visible | CellFlags.Occupied)) == m_StateMask) & cell.m_Zone.Equals(m_OldZoneType)) && ZoneUtils.CanShareCells(m_BaseBlockData, block))
				{
					CellData cellData2 = default(CellData);
					NativeParallelMultiHashMapIterator<Entity> val = default(NativeParallelMultiHashMapIterator<Entity>);
					if (!m_ZonedCells.TryGetFirstValue(blockEntity, ref cellData2, ref val))
					{
						m_ZonedBlocks.Add(ref blockEntity);
					}
					if (!m_Overwrite && !cell.m_Zone.Equals(ZoneType.None))
					{
						cellData.m_ZoneType = cell.m_Zone;
					}
					m_ZonedCells.Add(blockEntity, cellData);
					m_FoundCells++;
				}
			}
		}

		[ReadOnly]
		public ComponentTypeHandle<CreationDefinition> m_CreationDefinitionType;

		[ReadOnly]
		public ComponentTypeHandle<Zoning> m_ZoningType;

		[ReadOnly]
		public ComponentLookup<Block> m_BlockData;

		[ReadOnly]
		public ComponentLookup<ZoneData> m_ZoneData;

		[ReadOnly]
		public BufferLookup<Cell> m_Cells;

		[ReadOnly]
		public NativeQuadTree<Entity, Bounds2> m_SearchTree;

		public NativeParallelMultiHashMap<Entity, CellData> m_ZonedCells;

		public NativeList<Entity> m_ZonedBlocks;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<CreationDefinition> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CreationDefinition>(ref m_CreationDefinitionType);
			NativeArray<Zoning> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Zoning>(ref m_ZoningType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				CreationDefinition definitionData = nativeArray[i];
				Zoning zoningData = nativeArray2[i];
				if (definitionData.m_Prefab != Entity.Null)
				{
					ZoneData zoneData = m_ZoneData[definitionData.m_Prefab];
					if ((zoningData.m_Flags & ZoningFlags.FloodFill) != 0)
					{
						FloodFillBlocks(definitionData, zoningData, zoneData);
					}
					if ((zoningData.m_Flags & ZoningFlags.Paint) != 0)
					{
						PaintBlocks(definitionData, zoningData, zoneData);
					}
					if ((zoningData.m_Flags & ZoningFlags.Marquee) != 0)
					{
						MarqueeBlocks(definitionData, zoningData, zoneData);
					}
				}
			}
		}

		private void MarqueeBlocks(CreationDefinition definitionData, Zoning zoningData, ZoneData zoneData)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			ZoneType newZoneType;
			if ((zoningData.m_Flags & ZoningFlags.Zone) != 0)
			{
				newZoneType = zoneData.m_ZoneType;
			}
			else
			{
				if ((zoningData.m_Flags & ZoningFlags.Dezone) == 0)
				{
					return;
				}
				newZoneType = ZoneType.None;
			}
			MarqueeIterator marqueeIterator = new MarqueeIterator
			{
				m_Bounds = MathUtils.Bounds(((Quad3)(ref zoningData.m_Position)).xz),
				m_Quad = ((Quad3)(ref zoningData.m_Position)).xz,
				m_NewZoneType = newZoneType,
				m_Overwrite = ((zoningData.m_Flags & ZoningFlags.Overwrite) != 0),
				m_BlockData = m_BlockData,
				m_Cells = m_Cells,
				m_ZonedCells = m_ZonedCells,
				m_ZonedBlocks = m_ZonedBlocks
			};
			m_SearchTree.Iterate<MarqueeIterator>(ref marqueeIterator, 0);
		}

		private void PaintBlocks(CreationDefinition definitionData, Zoning zoningData, ZoneData zoneData)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			ZoneType zoneType;
			if ((zoningData.m_Flags & ZoningFlags.Zone) != 0)
			{
				zoneType = zoneData.m_ZoneType;
			}
			else
			{
				if ((zoningData.m_Flags & ZoningFlags.Dezone) == 0)
				{
					return;
				}
				zoneType = ZoneType.None;
			}
			NativeList<BaseCell> baseCells = default(NativeList<BaseCell>);
			baseCells._002Ector(10, AllocatorHandle.op_Implicit((Allocator)2));
			Quad2 xz = ((Quad3)(ref zoningData.m_Position)).xz;
			AddCells(((Quad2)(ref xz)).bc, baseCells);
			CellData cellData = default(CellData);
			CellData cellData2 = default(CellData);
			NativeParallelMultiHashMapIterator<Entity> val = default(NativeParallelMultiHashMapIterator<Entity>);
			for (int i = 0; i < baseCells.Length; i++)
			{
				BaseCell baseCell = baseCells[i];
				Block block = m_BlockData[baseCell.m_Block];
				cellData.m_Location = baseCell.m_Location;
				cellData.m_ZoneType = zoneType;
				if (!math.all((cellData.m_Location >= 0) & (cellData.m_Location < block.m_Size)))
				{
					continue;
				}
				Cell cell = m_Cells[baseCell.m_Block][cellData.m_Location.y * block.m_Size.x + cellData.m_Location.x];
				if ((cell.m_State & CellFlags.Visible) != CellFlags.None)
				{
					if (!m_ZonedCells.TryGetFirstValue(baseCell.m_Block, ref cellData2, ref val))
					{
						m_ZonedBlocks.Add(ref baseCell.m_Block);
					}
					if ((zoningData.m_Flags & ZoningFlags.Overwrite) == 0 && !cell.m_Zone.Equals(ZoneType.None))
					{
						cellData.m_ZoneType = cell.m_Zone;
					}
					m_ZonedCells.Add(baseCell.m_Block, cellData);
				}
			}
		}

		private void FloodFillBlocks(CreationDefinition definitionData, Zoning zoningData, ZoneData zoneData)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			ZoneType newZoneType;
			if ((zoningData.m_Flags & ZoningFlags.Zone) != 0)
			{
				newZoneType = zoneData.m_ZoneType;
			}
			else
			{
				if ((zoningData.m_Flags & ZoningFlags.Dezone) == 0)
				{
					return;
				}
				newZoneType = ZoneType.None;
			}
			NativeParallelHashSet<int> val = default(NativeParallelHashSet<int>);
			val._002Ector(1000, AllocatorHandle.op_Implicit((Allocator)2));
			NativeList<BaseCell> baseCells = default(NativeList<BaseCell>);
			baseCells._002Ector(10, AllocatorHandle.op_Implicit((Allocator)2));
			NativeList<int2> val2 = default(NativeList<int2>);
			val2._002Ector(1000, AllocatorHandle.op_Implicit((Allocator)2));
			Quad2 xz = ((Quad3)(ref zoningData.m_Position)).xz;
			AddCells(((Quad2)(ref xz)).bc, baseCells);
			for (int i = 0; i < baseCells.Length; i++)
			{
				BaseCell baseCell = baseCells[i];
				Block block = m_BlockData[baseCell.m_Block];
				DynamicBuffer<Cell> val3 = m_Cells[baseCell.m_Block];
				int2 val4 = baseCell.m_Location;
				Cell cell = val3[val4.y * block.m_Size.x + val4.x];
				FloodFillIterator floodFillIterator = new FloodFillIterator
				{
					m_BaseBlockData = block,
					m_StateMask = (cell.m_State & (CellFlags.Visible | CellFlags.Occupied)),
					m_OldZoneType = cell.m_Zone,
					m_NewZoneType = newZoneType,
					m_Overwrite = ((zoningData.m_Flags & ZoningFlags.Overwrite) != 0),
					m_BlockData = m_BlockData,
					m_Cells = m_Cells,
					m_ZonedCells = m_ZonedCells,
					m_ZonedBlocks = m_ZonedBlocks
				};
				val.Add(PackToInt(val4));
				val2.Add(ref val4);
				int num = 0;
				while (num < val2.Length)
				{
					val4 = val2[num++];
					float3 cellPosition = ZoneUtils.GetCellPosition(block, val4);
					floodFillIterator.m_Position = ((float3)(ref cellPosition)).xz;
					floodFillIterator.m_FoundCells = 0;
					m_SearchTree.Iterate<FloodFillIterator>(ref floodFillIterator, 0);
					if (floodFillIterator.m_FoundCells != 0)
					{
						int2 cellIndex = val4;
						int2 cellIndex2 = val4;
						int2 cellIndex3 = val4;
						int2 cellIndex4 = val4;
						cellIndex.x--;
						cellIndex2.y--;
						cellIndex3.x++;
						cellIndex4.y++;
						if (val.Add(PackToInt(cellIndex)))
						{
							val2.Add(ref cellIndex);
						}
						if (val.Add(PackToInt(cellIndex2)))
						{
							val2.Add(ref cellIndex2);
						}
						if (val.Add(PackToInt(cellIndex3)))
						{
							val2.Add(ref cellIndex3);
						}
						if (val.Add(PackToInt(cellIndex4)))
						{
							val2.Add(ref cellIndex4);
						}
					}
				}
				val.Clear();
				val2.Clear();
			}
		}

		private static int PackToInt(int2 cellIndex)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			return (cellIndex.y << 16) | (cellIndex.x & 0xFFFF);
		}

		private void AddCells(Segment line, NativeList<BaseCell> baseCells)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			BaseLineIterator baseLineIterator = new BaseLineIterator
			{
				m_Line = line,
				m_BlockData = m_BlockData,
				m_Cells = m_Cells,
				m_BaseCells = baseCells
			};
			m_SearchTree.Iterate<BaseLineIterator>(ref baseLineIterator, 0);
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct CreateBlocksJob : IJobParallelForDefer
	{
		[ReadOnly]
		public ComponentLookup<Block> m_BlockData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ZoneBlockData> m_ZoneBlockDataData;

		[ReadOnly]
		public BufferLookup<Cell> m_Cells;

		[ReadOnly]
		public NativeParallelMultiHashMap<Entity, CellData> m_ZonedCells;

		[ReadOnly]
		public NativeArray<Entity> m_ZonedBlocks;

		public ParallelWriter m_CommandBuffer;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			Entity val = m_ZonedBlocks[index];
			Block block = m_BlockData[val];
			PrefabRef prefabRef = m_PrefabRefData[val];
			DynamicBuffer<Cell> val2 = m_Cells[val];
			ZoneBlockData zoneBlockData = m_ZoneBlockDataData[prefabRef.m_Prefab];
			Entity val3 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(index, zoneBlockData.m_Archetype);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(index, val3, prefabRef);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Block>(index, val3, block);
			DynamicBuffer<Cell> val4 = ((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<Cell>(index, val3);
			Temp temp = new Temp
			{
				m_Original = val
			};
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Temp>(index, val3, temp);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Hidden>(index, val, default(Hidden));
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(index, val, default(BatchesUpdated));
			for (int i = 0; i < val2.Length; i++)
			{
				val4.Add(val2[i]);
			}
			CellData cellData = default(CellData);
			NativeParallelMultiHashMapIterator<Entity> val5 = default(NativeParallelMultiHashMapIterator<Entity>);
			if (m_ZonedCells.TryGetFirstValue(val, ref cellData, ref val5))
			{
				do
				{
					int num = cellData.m_Location.y * block.m_Size.x + cellData.m_Location.x;
					Cell cell = val4[num];
					cell.m_State |= CellFlags.Selected;
					cell.m_Zone = cellData.m_ZoneType;
					val4[num] = cell;
				}
				while (m_ZonedCells.TryGetNextValue(ref cellData, ref val5));
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<CreationDefinition> __Game_Tools_CreationDefinition_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Zoning> __Game_Tools_Zoning_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Block> __Game_Zones_Block_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ZoneData> __Game_Prefabs_ZoneData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Cell> __Game_Zones_Cell_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ZoneBlockData> __Game_Prefabs_ZoneBlockData_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			__Game_Tools_CreationDefinition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CreationDefinition>(true);
			__Game_Tools_Zoning_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Zoning>(true);
			__Game_Zones_Block_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Block>(true);
			__Game_Prefabs_ZoneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ZoneData>(true);
			__Game_Zones_Cell_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Cell>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ZoneBlockData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ZoneBlockData>(true);
		}
	}

	private SearchSystem m_ZoneSearchSystem;

	private ModificationBarrier1 m_ModificationBarrier;

	private EntityQuery m_DefinitionQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ZoneSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SearchSystem>();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier1>();
		m_DefinitionQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<CreationDefinition>(),
			ComponentType.ReadOnly<Zoning>(),
			ComponentType.ReadOnly<Updated>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_DefinitionQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		NativeParallelMultiHashMap<Entity, CellData> zonedCells = default(NativeParallelMultiHashMap<Entity, CellData>);
		zonedCells._002Ector(1000, AllocatorHandle.op_Implicit((Allocator)3));
		NativeList<Entity> val = default(NativeList<Entity>);
		val._002Ector(20, AllocatorHandle.op_Implicit((Allocator)3));
		JobHandle dependencies;
		FillBlocksListJob fillBlocksListJob = new FillBlocksListJob
		{
			m_CreationDefinitionType = InternalCompilerInterface.GetComponentTypeHandle<CreationDefinition>(ref __TypeHandle.__Game_Tools_CreationDefinition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ZoningType = InternalCompilerInterface.GetComponentTypeHandle<Zoning>(ref __TypeHandle.__Game_Tools_Zoning_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BlockData = InternalCompilerInterface.GetComponentLookup<Block>(ref __TypeHandle.__Game_Zones_Block_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ZoneData = InternalCompilerInterface.GetComponentLookup<ZoneData>(ref __TypeHandle.__Game_Prefabs_ZoneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Cells = InternalCompilerInterface.GetBufferLookup<Cell>(ref __TypeHandle.__Game_Zones_Cell_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SearchTree = m_ZoneSearchSystem.GetSearchTree(readOnly: true, out dependencies),
			m_ZonedCells = zonedCells,
			m_ZonedBlocks = val
		};
		CreateBlocksJob createBlocksJob = new CreateBlocksJob
		{
			m_BlockData = InternalCompilerInterface.GetComponentLookup<Block>(ref __TypeHandle.__Game_Zones_Block_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ZoneBlockDataData = InternalCompilerInterface.GetComponentLookup<ZoneBlockData>(ref __TypeHandle.__Game_Prefabs_ZoneBlockData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Cells = InternalCompilerInterface.GetBufferLookup<Cell>(ref __TypeHandle.__Game_Zones_Cell_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ZonedCells = zonedCells,
			m_ZonedBlocks = val.AsDeferredJobArray()
		};
		EntityCommandBuffer val2 = m_ModificationBarrier.CreateCommandBuffer();
		createBlocksJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
		CreateBlocksJob createBlocksJob2 = createBlocksJob;
		JobHandle val3 = JobChunkExtensions.Schedule<FillBlocksListJob>(fillBlocksListJob, m_DefinitionQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies));
		JobHandle val4 = IJobParallelForDeferExtensions.Schedule<CreateBlocksJob, Entity>(createBlocksJob2, val, 1, val3);
		zonedCells.Dispose(val4);
		val.Dispose(val4);
		m_ZoneSearchSystem.AddSearchTreeReader(val3);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val4);
		((SystemBase)this).Dependency = val4;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public GenerateZonesSystem()
	{
	}
}
