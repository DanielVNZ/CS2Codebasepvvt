using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Areas;
using Game.Audio;
using Game.Common;
using Game.Input;
using Game.Prefabs;
using Game.Simulation;
using Game.Zones;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class ZoneToolSystem : ToolBaseSystem
{
	public enum Mode
	{
		FloodFill,
		Marquee,
		Paint
	}

	private enum State
	{
		Default,
		Zoning,
		Dezoning
	}

	[BurstCompile]
	private struct SetZoneTypeJob : IJobChunk
	{
		[ReadOnly]
		public ZoneType m_Type;

		[ReadOnly]
		public ComponentTypeHandle<Block> m_BlockType;

		public BufferTypeHandle<Cell> m_CellType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Block> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Block>(ref m_BlockType);
			BufferAccessor<Cell> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Cell>(ref m_CellType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Block block = nativeArray[i];
				DynamicBuffer<Cell> val = bufferAccessor[i];
				for (int j = 0; j < block.m_Size.y; j++)
				{
					for (int k = 0; k < block.m_Size.x; k++)
					{
						int num = j * block.m_Size.x + k;
						Cell cell = val[num];
						cell.m_Zone = m_Type;
						val[num] = cell;
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct SnapJob : IJob
	{
		[ReadOnly]
		public Snap m_Snap;

		[ReadOnly]
		public Mode m_Mode;

		[ReadOnly]
		public State m_State;

		[ReadOnly]
		public float3 m_CameraRight;

		[ReadOnly]
		public ControlPoint m_StartPoint;

		[ReadOnly]
		public ControlPoint m_RaycastPoint;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_TempChunks;

		[ReadOnly]
		public ComponentTypeHandle<Block> m_BlockType;

		[ReadOnly]
		public BufferTypeHandle<Cell> m_CellType;

		[ReadOnly]
		public ComponentLookup<Block> m_BlockData;

		public NativeValue<ControlPoint> m_SnapPoint;

		public void Execute()
		{
			switch (m_Mode)
			{
			case Mode.FloodFill:
			case Mode.Paint:
				CheckSelectedCell();
				break;
			case Mode.Marquee:
				CheckMarqueeCell();
				break;
			}
		}

		private void CheckSelectedCell()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_TempChunks.Length; i++)
			{
				ArchetypeChunk val = m_TempChunks[i];
				NativeArray<Block> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<Block>(ref m_BlockType);
				BufferAccessor<Cell> bufferAccessor = ((ArchetypeChunk)(ref val)).GetBufferAccessor<Cell>(ref m_CellType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Block block = nativeArray[j];
					DynamicBuffer<Cell> val2 = bufferAccessor[j];
					int2 cellIndex = ZoneUtils.GetCellIndex(block, ((float3)(ref m_RaycastPoint.m_HitPosition)).xz);
					if (math.all((cellIndex >= 0) & (cellIndex < block.m_Size)) && (val2[cellIndex.y * block.m_Size.x + cellIndex.x].m_State & CellFlags.Selected) != CellFlags.None)
					{
						return;
					}
				}
			}
			m_SnapPoint.value = m_RaycastPoint;
		}

		private void CheckMarqueeCell()
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			ControlPoint value = m_RaycastPoint;
			if ((m_Snap & Snap.ExistingGeometry) == 0)
			{
				value.m_OriginalEntity = Entity.Null;
			}
			if (m_BlockData.HasComponent(value.m_OriginalEntity))
			{
				Block block = m_BlockData[value.m_OriginalEntity];
				value.m_Position = ZoneUtils.GetCellPosition(block, value.m_ElementIndex);
				value.m_HitPosition = value.m_Position;
				m_SnapPoint.value = value;
			}
			else if ((m_Snap & Snap.CellLength) != Snap.None && m_State != State.Default)
			{
				float2 val = ((!m_BlockData.HasComponent(m_StartPoint.m_OriginalEntity)) ? math.normalizesafe(((float3)(ref m_CameraRight)).xz, default(float2)) : m_BlockData[m_StartPoint.m_OriginalEntity].m_Direction);
				float2 val2 = MathUtils.Right(val);
				float2 xz = ((float3)(ref m_StartPoint.m_HitPosition)).xz;
				float2 val3 = ((float3)(ref value.m_HitPosition)).xz - xz;
				float num = MathUtils.Snap(math.dot(val3, val), 8f);
				float num2 = MathUtils.Snap(math.dot(val3, val2), 8f);
				value.m_HitPosition.y = m_StartPoint.m_HitPosition.y;
				((float3)(ref value.m_HitPosition)).xz = xz + val * num + val2 * num2;
				m_SnapPoint.value = value;
			}
			else
			{
				m_SnapPoint.value = value;
			}
		}
	}

	[BurstCompile]
	private struct CreateDefinitionsJob : IJob
	{
		[ReadOnly]
		public Entity m_Prefab;

		[ReadOnly]
		public Mode m_Mode;

		[ReadOnly]
		public State m_State;

		[ReadOnly]
		public float3 m_CameraRight;

		[ReadOnly]
		public bool m_Overwrite;

		[ReadOnly]
		public ControlPoint m_StartPoint;

		[ReadOnly]
		public NativeValue<ControlPoint> m_SnapPoint;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[ReadOnly]
		public ComponentLookup<Block> m_BlockData;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0445: Unknown result type (might be due to invalid IL or missing references)
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_046c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0471: Unknown result type (might be due to invalid IL or missing references)
			//IL_0493: Unknown result type (might be due to invalid IL or missing references)
			//IL_0498: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_039e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0401: Unknown result type (might be due to invalid IL or missing references)
			//IL_0403: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_040c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_041d: Unknown result type (might be due to invalid IL or missing references)
			//IL_041f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0421: Unknown result type (might be due to invalid IL or missing references)
			//IL_0426: Unknown result type (might be due to invalid IL or missing references)
			//IL_0428: Unknown result type (might be due to invalid IL or missing references)
			//IL_042d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_0343: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_038a: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			ControlPoint value = m_SnapPoint.value;
			if (value.Equals(default(ControlPoint)))
			{
				return;
			}
			Entity val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
			CreationDefinition creationDefinition = new CreationDefinition
			{
				m_Prefab = m_Prefab,
				m_Original = value.m_OriginalEntity
			};
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val, creationDefinition);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val, default(Updated));
			Zoning zoning = default(Zoning);
			if (m_State == State.Dezoning)
			{
				zoning.m_Flags |= ZoningFlags.Dezone | ZoningFlags.Overwrite;
			}
			else if (m_Overwrite)
			{
				zoning.m_Flags |= ZoningFlags.Zone | ZoningFlags.Overwrite;
			}
			else
			{
				zoning.m_Flags |= ZoningFlags.Zone;
			}
			switch (m_Mode)
			{
			case Mode.FloodFill:
				zoning.m_Flags |= ZoningFlags.FloodFill;
				if (m_State != State.Default)
				{
					float3 hitPosition3 = m_StartPoint.m_HitPosition;
					float3 hitPosition4 = value.m_HitPosition;
					zoning.m_Position = new Quad3(hitPosition3, hitPosition3, hitPosition4, hitPosition4);
				}
				else
				{
					float3 hitPosition5 = value.m_HitPosition;
					zoning.m_Position = new Quad3(hitPosition5, hitPosition5, hitPosition5, hitPosition5);
				}
				break;
			case Mode.Paint:
				zoning.m_Flags |= ZoningFlags.Paint;
				if (m_State != State.Default)
				{
					float3 hitPosition6 = m_StartPoint.m_HitPosition;
					float3 hitPosition7 = value.m_HitPosition;
					zoning.m_Position = new Quad3(hitPosition6, hitPosition6, hitPosition7, hitPosition7);
				}
				else
				{
					float3 hitPosition8 = value.m_HitPosition;
					zoning.m_Position = new Quad3(hitPosition8, hitPosition8, hitPosition8, hitPosition8);
				}
				break;
			case Mode.Marquee:
			{
				zoning.m_Flags |= ZoningFlags.Marquee;
				float3 val2 = float3.op_Implicit(0f);
				if (m_State != State.Default && m_BlockData.HasComponent(m_StartPoint.m_OriginalEntity))
				{
					((float3)(ref val2)).xz = m_BlockData[m_StartPoint.m_OriginalEntity].m_Direction;
				}
				else if (m_State == State.Default && m_BlockData.HasComponent(value.m_OriginalEntity))
				{
					((float3)(ref val2)).xz = m_BlockData[value.m_OriginalEntity].m_Direction;
				}
				else
				{
					((float3)(ref val2)).xz = math.normalizesafe(((float3)(ref m_CameraRight)).xz, default(float2));
				}
				float3 val3 = float3.op_Implicit(0f);
				((float3)(ref val3)).xz = MathUtils.Right(((float3)(ref val2)).xz);
				if (m_State != State.Default)
				{
					float3 hitPosition = m_StartPoint.m_HitPosition;
					float3 val4 = value.m_HitPosition - hitPosition;
					float num = math.dot(val4, val2);
					float num2 = math.dot(val4, val3);
					if (num < 0f)
					{
						val2 = -val2;
						num = 0f - num;
					}
					if (num2 < 0f)
					{
						val3 = -val3;
						num2 = 0f - num2;
					}
					zoning.m_Position.a = hitPosition - (val2 + val3) * 4f;
					zoning.m_Position.b = hitPosition - val2 * 4f + val3 * (num2 + 4f);
					zoning.m_Position.c = hitPosition + val2 * (num + 4f) + val3 * (num2 + 4f);
					zoning.m_Position.d = hitPosition + val2 * (num + 4f) - val3 * 4f;
				}
				else
				{
					val2 *= 4f;
					val3 *= 4f;
					float3 hitPosition2 = value.m_HitPosition;
					zoning.m_Position.a = hitPosition2 - val2 - val3;
					zoning.m_Position.b = hitPosition2 - val2 + val3;
					zoning.m_Position.c = hitPosition2 + val2 + val3;
					zoning.m_Position.d = hitPosition2 + val2 - val3;
				}
				break;
			}
			}
			zoning.m_Position.a.y = TerrainUtils.SampleHeight(ref m_TerrainHeightData, zoning.m_Position.a);
			zoning.m_Position.b.y = TerrainUtils.SampleHeight(ref m_TerrainHeightData, zoning.m_Position.b);
			zoning.m_Position.c.y = TerrainUtils.SampleHeight(ref m_TerrainHeightData, zoning.m_Position.c);
			zoning.m_Position.d.y = TerrainUtils.SampleHeight(ref m_TerrainHeightData, zoning.m_Position.d);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Zoning>(val, zoning);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<Block> __Game_Zones_Block_RO_ComponentTypeHandle;

		public BufferTypeHandle<Cell> __Game_Zones_Cell_RW_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Cell> __Game_Zones_Cell_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Block> __Game_Zones_Block_RO_ComponentLookup;

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
			__Game_Zones_Block_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Block>(true);
			__Game_Zones_Cell_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Cell>(false);
			__Game_Zones_Cell_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Cell>(true);
			__Game_Zones_Block_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Block>(true);
		}
	}

	public const string kToolID = "Zone Tool";

	private ZonePrefab m_Prefab;

	private ToolOutputBarrier m_ToolOutputBarrier;

	private AudioManager m_AudioManager;

	private TerrainSystem m_TerrainSystem;

	private EntityQuery m_DefinitionGroup;

	private EntityQuery m_TempBlockQuery;

	private EntityQuery m_SoundQuery;

	private IProxyAction m_ApplyZone;

	private IProxyAction m_RemoveZone;

	private IProxyAction m_DiscardZoning;

	private IProxyAction m_DiscardDezoning;

	private IProxyAction m_DefaultDiscardApply;

	private IProxyAction m_DefaultDiscardRemove;

	private bool m_ApplyBlocked;

	private ControlPoint m_RaycastPoint;

	private ControlPoint m_StartPoint;

	private NativeValue<ControlPoint> m_SnapPoint;

	private State m_State;

	private TypeHandle __TypeHandle;

	public override string toolID => "Zone Tool";

	public override int uiModeIndex => (int)mode;

	public Mode mode { get; set; }

	public ZonePrefab prefab
	{
		get
		{
			return m_Prefab;
		}
		set
		{
			if ((Object)(object)m_Prefab != (Object)(object)value)
			{
				m_ForceUpdate = true;
				m_Prefab = value;
			}
		}
	}

	public bool overwrite { get; set; }

	private protected override IEnumerable<IProxyAction> toolActions
	{
		get
		{
			yield return m_ApplyZone;
			yield return m_RemoveZone;
			yield return m_DiscardZoning;
			yield return m_DiscardDezoning;
		}
	}

	public override void GetUIModes(List<ToolMode> modes)
	{
		modes.Add(new ToolMode(Mode.FloodFill.ToString(), 0));
		modes.Add(new ToolMode(Mode.Marquee.ToString(), 1));
		modes.Add(new ToolMode(Mode.Paint.ToString(), 2));
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolOutputBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolOutputBarrier>();
		m_AudioManager = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AudioManager>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_DefinitionGroup = GetDefinitionQuery();
		m_TempBlockQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Block>(),
			ComponentType.ReadWrite<Cell>()
		});
		m_SoundQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ToolUXSoundSettingsData>() });
		m_ApplyZone = InputManager.instance.toolActionCollection.GetActionState("Apply Zone", "ZoneToolSystem");
		m_RemoveZone = InputManager.instance.toolActionCollection.GetActionState("Remove Zone", "ZoneToolSystem");
		m_DiscardZoning = InputManager.instance.toolActionCollection.GetActionState("Discard Zoning", "ZoneToolSystem");
		m_DiscardDezoning = InputManager.instance.toolActionCollection.GetActionState("Discard Dezoning", "ZoneToolSystem");
		m_DefaultDiscardApply = InputManager.instance.toolActionCollection.GetActionState("Discard Primary", "ZoneToolSystem");
		m_DefaultDiscardRemove = InputManager.instance.toolActionCollection.GetActionState("Discard Secondary", "ZoneToolSystem");
		m_SnapPoint = new NativeValue<ControlPoint>((Allocator)4);
		overwrite = true;
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_SnapPoint.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnStartRunning()
	{
		base.OnStartRunning();
		base.requireZones = true;
		base.requireAreas = AreaTypeMask.Lots;
		m_RaycastPoint = default(ControlPoint);
		m_StartPoint = default(ControlPoint);
		m_State = State.Default;
		m_ApplyBlocked = false;
	}

	private protected override void UpdateActions()
	{
		using (ProxyAction.DeferStateUpdating())
		{
			switch (m_State)
			{
			case State.Zoning:
				base.applyAction.enabled = base.actionsEnabled;
				base.secondaryApplyAction.enabled = false;
				base.cancelAction.enabled = base.actionsEnabled;
				base.applyActionOverride = m_ApplyZone;
				base.secondaryApplyActionOverride = null;
				base.cancelActionOverride = ((mode == Mode.Marquee) ? m_DiscardZoning : m_DefaultDiscardApply);
				break;
			case State.Dezoning:
				base.applyAction.enabled = false;
				base.secondaryApplyAction.enabled = base.actionsEnabled;
				base.cancelAction.enabled = base.actionsEnabled;
				base.applyActionOverride = null;
				base.secondaryApplyActionOverride = m_RemoveZone;
				base.cancelActionOverride = ((mode == Mode.Marquee) ? m_DiscardDezoning : m_DefaultDiscardRemove);
				break;
			default:
				base.applyAction.enabled = base.actionsEnabled && GetAllowApplyZone();
				base.secondaryApplyAction.enabled = base.actionsEnabled && GetAllowRemoveZone();
				base.cancelAction.enabled = false;
				base.applyActionOverride = m_ApplyZone;
				base.secondaryApplyActionOverride = m_RemoveZone;
				base.cancelActionOverride = null;
				break;
			}
		}
	}

	protected bool GetAllowApplyZone()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		Mode mode = this.mode;
		if ((uint)(mode - 1) <= 1u)
		{
			return true;
		}
		GetRaycastResult(out Entity entity, out RaycastHit _);
		if (entity == Entity.Null)
		{
			return false;
		}
		Block block = default(Block);
		if (!EntitiesExtensions.TryGetComponent<Block>(((ComponentSystemBase)this).EntityManager, entity, ref block))
		{
			return false;
		}
		Entity entity2 = m_PrefabSystem.GetEntity(prefab);
		if (entity2 == Entity.Null)
		{
			return false;
		}
		ZoneData zoneData = default(ZoneData);
		if (!EntitiesExtensions.TryGetComponent<ZoneData>(((ComponentSystemBase)this).EntityManager, entity2, ref zoneData))
		{
			return false;
		}
		DynamicBuffer<Cell> val = default(DynamicBuffer<Cell>);
		if (!EntitiesExtensions.TryGetBuffer<Cell>(((ComponentSystemBase)this).EntityManager, entity, true, ref val))
		{
			return false;
		}
		for (int i = 0; i < val.Length; i++)
		{
			if (val[i].m_Zone.m_Index != zoneData.m_ZoneType.m_Index)
			{
				return true;
			}
		}
		return false;
	}

	protected bool GetAllowRemoveZone()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		Mode mode = this.mode;
		if ((uint)(mode - 1) <= 1u)
		{
			return true;
		}
		GetRaycastResult(out Entity entity, out RaycastHit _);
		if (entity == Entity.Null)
		{
			return false;
		}
		Block block = default(Block);
		if (!EntitiesExtensions.TryGetComponent<Block>(((ComponentSystemBase)this).EntityManager, entity, ref block))
		{
			return false;
		}
		DynamicBuffer<Cell> val = default(DynamicBuffer<Cell>);
		if (!EntitiesExtensions.TryGetBuffer<Cell>(((ComponentSystemBase)this).EntityManager, entity, true, ref val))
		{
			return false;
		}
		for (int i = 0; i < val.Length; i++)
		{
			if (val[i].m_Zone.m_Index != 0)
			{
				return true;
			}
		}
		return false;
	}

	public override PrefabBase GetPrefab()
	{
		return prefab;
	}

	public override bool TrySetPrefab(PrefabBase prefab)
	{
		if (prefab is ZonePrefab zonePrefab)
		{
			this.prefab = zonePrefab;
			return true;
		}
		return false;
	}

	public override void InitializeRaycast()
	{
		base.InitializeRaycast();
		if ((Object)(object)prefab != (Object)null)
		{
			GetAvailableSnapMask(out var onMask, out var offMask);
			switch (mode)
			{
			case Mode.FloodFill:
			case Mode.Paint:
				m_ToolRaycastSystem.typeMask = TypeMask.Terrain | TypeMask.Zones;
				break;
			case Mode.Marquee:
				if ((ToolBaseSystem.GetActualSnap(selectedSnap, onMask, offMask) & Snap.ExistingGeometry) != Snap.None)
				{
					m_ToolRaycastSystem.typeMask = TypeMask.Terrain | TypeMask.Zones;
				}
				else
				{
					m_ToolRaycastSystem.typeMask = TypeMask.Terrain;
				}
				break;
			default:
				m_ToolRaycastSystem.typeMask = TypeMask.None;
				break;
			}
		}
		else
		{
			m_ToolRaycastSystem.typeMask = TypeMask.None;
		}
	}

	[Preserve]
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		UpdateActions();
		if (m_FocusChanged)
		{
			return inputDeps;
		}
		if (m_State != State.Default && (!base.applyAction.enabled || !base.cancelAction.enabled) && (!base.secondaryApplyAction.enabled || !base.cancelAction.enabled))
		{
			m_StartPoint = default(ControlPoint);
			m_State = State.Default;
			return Clear(inputDeps);
		}
		if ((Object)(object)prefab != (Object)null)
		{
			UpdateInfoview(m_PrefabSystem.GetEntity(prefab));
			GetAvailableSnapMask(out m_SnapOnMask, out m_SnapOffMask);
			if ((m_ToolRaycastSystem.raycastFlags & (RaycastFlags.DebugDisable | RaycastFlags.UIDisable)) == 0)
			{
				switch (m_State)
				{
				case State.Default:
					if (m_ApplyBlocked)
					{
						if (mode != Mode.Marquee || base.applyAction.WasReleasedThisFrame() || base.secondaryApplyAction.WasReleasedThisFrame())
						{
							m_ApplyBlocked = false;
						}
						return Update(inputDeps);
					}
					if (base.secondaryApplyAction.WasPressedThisFrame())
					{
						return Cancel(inputDeps, base.secondaryApplyAction.WasReleasedThisFrame());
					}
					if (base.applyAction.WasPressedThisFrame())
					{
						return Apply(inputDeps, base.applyAction.WasReleasedThisFrame());
					}
					return Update(inputDeps);
				case State.Zoning:
					if (base.cancelAction.WasPressedThisFrame())
					{
						m_ApplyBlocked = mode == Mode.Marquee;
						return Cancel(inputDeps);
					}
					if (base.applyAction.WasPressedThisFrame() || base.applyAction.WasReleasedThisFrame())
					{
						return Apply(inputDeps);
					}
					return Update(inputDeps);
				case State.Dezoning:
					if (base.cancelAction.WasPressedThisFrame())
					{
						m_ApplyBlocked = mode == Mode.Marquee;
						return Apply(inputDeps);
					}
					if (base.secondaryApplyAction.WasPressedThisFrame() || base.secondaryApplyAction.WasReleasedThisFrame())
					{
						return Cancel(inputDeps);
					}
					return Update(inputDeps);
				}
			}
		}
		else
		{
			UpdateInfoview(Entity.Null);
		}
		if (m_State != State.Default && (base.applyAction.WasReleasedThisFrame() || base.secondaryApplyAction.WasReleasedThisFrame()))
		{
			m_StartPoint = default(ControlPoint);
			m_State = State.Default;
		}
		return Clear(inputDeps);
	}

	public override void GetAvailableSnapMask(out Snap onMask, out Snap offMask)
	{
		switch (mode)
		{
		case Mode.FloodFill:
		case Mode.Paint:
			onMask = Snap.ExistingGeometry;
			offMask = Snap.None;
			break;
		case Mode.Marquee:
			onMask = Snap.ExistingGeometry | Snap.CellLength;
			offMask = Snap.ExistingGeometry | Snap.CellLength;
			break;
		default:
			base.GetAvailableSnapMask(out onMask, out offMask);
			break;
		}
		onMask |= Snap.ContourLines;
		offMask |= Snap.ContourLines;
	}

	private JobHandle Clear(JobHandle inputDeps)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		base.applyMode = ApplyMode.Clear;
		return inputDeps;
	}

	private JobHandle Cancel(JobHandle inputDeps, bool singleFrameOnly = false)
	{
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		if (m_State == State.Default)
		{
			switch (mode)
			{
			case Mode.FloodFill:
				m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_ZoningRemoveFillSound);
				base.applyMode = ApplyMode.Apply;
				break;
			case Mode.Paint:
				m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_ZoningStartRemovePaintSound);
				base.applyMode = ApplyMode.Apply;
				break;
			case Mode.Marquee:
				m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_ZoningMarqueeClearStartSound);
				base.applyMode = ApplyMode.Clear;
				break;
			}
			if (!singleFrameOnly)
			{
				m_StartPoint = m_SnapPoint.value;
				m_State = State.Dezoning;
			}
			GetRaycastResult(out m_RaycastPoint);
			JobHandle val = SnapPoint(inputDeps);
			JobHandle val2 = SetZoneType(val);
			JobHandle val3 = UpdateDefinitions(val);
			return JobHandle.CombineDependencies(val2, val3);
		}
		if (m_State == State.Dezoning)
		{
			base.applyMode = ApplyMode.Apply;
			if (math.distance(m_StartPoint.m_Position, m_RaycastPoint.m_Position) > 5f && mode == Mode.Marquee)
			{
				m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_ZoningMarqueeClearEndSound);
			}
			if (mode == Mode.Paint)
			{
				m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_ZoningEndRemovePaintSound);
			}
			m_StartPoint = default(ControlPoint);
			m_State = State.Default;
			GetRaycastResult(out m_RaycastPoint);
			inputDeps = SnapPoint(inputDeps);
			return UpdateDefinitions(inputDeps);
		}
		base.applyMode = ApplyMode.Clear;
		m_StartPoint = default(ControlPoint);
		m_State = State.Default;
		GetRaycastResult(out m_RaycastPoint);
		inputDeps = SnapPoint(inputDeps);
		return UpdateDefinitions(inputDeps);
	}

	private JobHandle Apply(JobHandle inputDeps, bool singleFrameOnly = false)
	{
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		if (m_State == State.Default)
		{
			switch (mode)
			{
			case Mode.FloodFill:
				m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_ZoningFillSound);
				base.applyMode = ApplyMode.Apply;
				break;
			case Mode.Paint:
				m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_ZoningStartPaintSound);
				base.applyMode = ApplyMode.Apply;
				break;
			case Mode.Marquee:
				m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_ZoningMarqueeStartSound);
				base.applyMode = ApplyMode.Clear;
				break;
			}
			if (!singleFrameOnly)
			{
				m_StartPoint = m_SnapPoint.value;
				m_State = State.Zoning;
			}
			GetRaycastResult(out m_RaycastPoint);
			inputDeps = SnapPoint(inputDeps);
			return UpdateDefinitions(inputDeps);
		}
		if (m_State == State.Zoning)
		{
			base.applyMode = ApplyMode.Apply;
			if (math.distance(m_StartPoint.m_Position, m_RaycastPoint.m_Position) > 5f && mode == Mode.Marquee)
			{
				m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_ZoningMarqueeEndSound);
			}
			if (mode == Mode.Paint)
			{
				m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_ZoningEndPaintSound);
			}
			m_StartPoint = default(ControlPoint);
			m_State = State.Default;
			GetRaycastResult(out m_RaycastPoint);
			inputDeps = SnapPoint(inputDeps);
			return UpdateDefinitions(inputDeps);
		}
		base.applyMode = ApplyMode.Clear;
		m_StartPoint = default(ControlPoint);
		m_State = State.Default;
		GetRaycastResult(out m_RaycastPoint);
		inputDeps = SnapPoint(inputDeps);
		return UpdateDefinitions(inputDeps);
	}

	private JobHandle Update(JobHandle inputDeps)
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		if (GetRaycastResult(out ControlPoint controlPoint, out bool forceUpdate))
		{
			ControlPoint value = m_SnapPoint.value;
			if (m_RaycastPoint.Equals(controlPoint) && !forceUpdate)
			{
				switch (this.mode)
				{
				case Mode.FloodFill:
				case Mode.Paint:
					if (m_State == State.Default || m_StartPoint.Equals(value))
					{
						base.applyMode = ApplyMode.None;
						return inputDeps;
					}
					break;
				case Mode.Marquee:
					base.applyMode = ApplyMode.None;
					return inputDeps;
				}
			}
			else
			{
				m_RaycastPoint = controlPoint;
				inputDeps = SnapPoint(inputDeps);
				JobHandle.ScheduleBatchedJobs();
				((JobHandle)(ref inputDeps)).Complete();
			}
			if (value.Equals(m_SnapPoint.value) && !forceUpdate)
			{
				switch (this.mode)
				{
				case Mode.FloodFill:
				case Mode.Paint:
					if (m_State == State.Default || m_StartPoint.Equals(value))
					{
						base.applyMode = ApplyMode.None;
						return inputDeps;
					}
					break;
				case Mode.Marquee:
					base.applyMode = ApplyMode.None;
					return inputDeps;
				}
			}
			switch (this.mode)
			{
			case Mode.FloodFill:
			case Mode.Paint:
				if (m_State != State.Default)
				{
					base.applyMode = ApplyMode.Apply;
					m_StartPoint = value;
				}
				else
				{
					base.applyMode = ApplyMode.Clear;
				}
				return UpdateDefinitions(inputDeps);
			case Mode.Marquee:
				base.applyMode = ApplyMode.Clear;
				return UpdateDefinitions(inputDeps);
			}
		}
		else
		{
			if (m_RaycastPoint.Equals(default(ControlPoint)))
			{
				base.applyMode = (forceUpdate ? ApplyMode.Clear : ApplyMode.None);
				return inputDeps;
			}
			m_RaycastPoint = default(ControlPoint);
			Mode mode = this.mode;
			if ((mode == Mode.FloodFill || mode == Mode.Paint) && m_State != State.Default)
			{
				m_StartPoint = m_SnapPoint.value;
				base.applyMode = ApplyMode.Apply;
				inputDeps = SnapPoint(inputDeps);
				return UpdateDefinitions(inputDeps);
			}
		}
		base.applyMode = ApplyMode.Clear;
		inputDeps = SnapPoint(inputDeps);
		return UpdateDefinitions(inputDeps);
	}

	private JobHandle SetZoneType(JobHandle inputDeps)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityQuery)(ref m_TempBlockQuery)).IsEmptyIgnoreFilter)
		{
			return inputDeps;
		}
		return JobChunkExtensions.ScheduleParallel<SetZoneTypeJob>(new SetZoneTypeJob
		{
			m_Type = default(ZoneType),
			m_BlockType = InternalCompilerInterface.GetComponentTypeHandle<Block>(ref __TypeHandle.__Game_Zones_Block_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CellType = InternalCompilerInterface.GetBufferTypeHandle<Cell>(ref __TypeHandle.__Game_Zones_Cell_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef)
		}, m_TempBlockQuery, inputDeps);
	}

	private JobHandle SnapPoint(JobHandle inputDeps)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (m_RaycastPoint.Equals(default(ControlPoint)))
		{
			m_SnapPoint.value = default(ControlPoint);
			return inputDeps;
		}
		JobHandle val = default(JobHandle);
		NativeList<ArchetypeChunk> tempChunks = ((EntityQuery)(ref m_TempBlockQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		Transform transform = ((Component)Camera.main).transform;
		JobHandle val2 = IJobExtensions.Schedule<SnapJob>(new SnapJob
		{
			m_Snap = GetActualSnap(),
			m_Mode = mode,
			m_State = m_State,
			m_CameraRight = float3.op_Implicit(transform.right),
			m_StartPoint = m_StartPoint,
			m_RaycastPoint = m_RaycastPoint,
			m_TempChunks = tempChunks,
			m_BlockType = InternalCompilerInterface.GetComponentTypeHandle<Block>(ref __TypeHandle.__Game_Zones_Block_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CellType = InternalCompilerInterface.GetBufferTypeHandle<Cell>(ref __TypeHandle.__Game_Zones_Cell_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BlockData = InternalCompilerInterface.GetComponentLookup<Block>(ref __TypeHandle.__Game_Zones_Block_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SnapPoint = m_SnapPoint
		}, JobHandle.CombineDependencies(inputDeps, val));
		tempChunks.Dispose(val2);
		return val2;
	}

	private JobHandle UpdateDefinitions(JobHandle inputDeps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = DestroyDefinitions(m_DefinitionGroup, m_ToolOutputBarrier, inputDeps);
		if (!m_RaycastPoint.Equals(default(ControlPoint)))
		{
			Transform transform = ((Component)Camera.main).transform;
			JobHandle val2 = IJobExtensions.Schedule<CreateDefinitionsJob>(new CreateDefinitionsJob
			{
				m_Prefab = m_PrefabSystem.GetEntity(prefab),
				m_Mode = mode,
				m_State = m_State,
				m_CameraRight = float3.op_Implicit(transform.right),
				m_Overwrite = overwrite,
				m_StartPoint = m_StartPoint,
				m_SnapPoint = m_SnapPoint,
				m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
				m_BlockData = InternalCompilerInterface.GetComponentLookup<Block>(ref __TypeHandle.__Game_Zones_Block_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CommandBuffer = m_ToolOutputBarrier.CreateCommandBuffer()
			}, inputDeps);
			((EntityCommandBufferSystem)m_ToolOutputBarrier).AddJobHandleForProducer(val2);
			m_TerrainSystem.AddCPUHeightReader(val2);
			val = JobHandle.CombineDependencies(val, val2);
		}
		return val;
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
		base.OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public ZoneToolSystem()
	{
	}
}
