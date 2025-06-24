using System;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Pathfind;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Net;

public static class AirwayHelpers
{
	public struct AirwayMap : IDisposable
	{
		private int2 m_GridSize;

		private float m_CellSize;

		private float m_PathHeight;

		private NativeArray<Entity> m_Entities;

		public NativeArray<Entity> entities => m_Entities;

		public AirwayMap(int2 gridSize, float cellSize, float pathHeight, Allocator allocator)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			m_GridSize = gridSize;
			m_CellSize = cellSize;
			m_PathHeight = pathHeight;
			int num = (m_GridSize.x * 3 + 1) * m_GridSize.y + m_GridSize.x;
			m_Entities = new NativeArray<Entity>(num, allocator, (NativeArrayOptions)1);
		}

		public void Dispose()
		{
			if (m_Entities.IsCreated)
			{
				m_Entities.Dispose();
			}
		}

		public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			int2 gridSize = m_GridSize;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(gridSize);
			float cellSize = m_CellSize;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(cellSize);
			float pathHeight = m_PathHeight;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(pathHeight);
			int length = m_Entities.Length;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(length);
			NativeArray<Entity> val = m_Entities;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(val);
		}

		public void Deserialize<TReader>(TReader reader) where TReader : IReader
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			ref int2 gridSize = ref m_GridSize;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref gridSize);
			ref float cellSize = ref m_CellSize;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref cellSize);
			Context context = ((IReader)reader).context;
			if (((Context)(ref context)).version >= Version.airplaneAirways)
			{
				ref float pathHeight = ref m_PathHeight;
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref pathHeight);
			}
			int num = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
			NativeArray<Entity> val = m_Entities;
			((IReader)reader/*cast due to .constrained prefix*/).Read(val);
		}

		public void SetDefaults(Context context)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_Entities.Length; i++)
			{
				m_Entities[i] = Entity.Null;
			}
		}

		public int2 GetCellIndex(int entityIndex, out LaneDirection direction)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			int num = m_GridSize.x * 3 + 1;
			int2 val = default(int2);
			val.y = entityIndex / num;
			val.x = entityIndex - val.y * num;
			direction = LaneDirection.HorizontalX;
			if (val.y < m_GridSize.y)
			{
				val.x /= 3;
				direction = (LaneDirection)(entityIndex - val.x * 3 - val.y * num);
				direction += ((int)direction >> 1) & (val.x + val.y);
			}
			return val;
		}

		public int GetEntityIndex(int2 nodeIndex, LaneDirection direction)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			int num = m_GridSize.x * 3 + 1;
			int num2 = nodeIndex.y * num;
			if (nodeIndex.y < m_GridSize.y)
			{
				return num2 + (nodeIndex.x * 3 + math.min(2, (int)direction));
			}
			return num2 + nodeIndex.x;
		}

		public PathNode GetPathNode(int2 index)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			int num = m_GridSize.x * 3 + 1;
			if (index.y < m_GridSize.y)
			{
				return new PathNode(m_Entities[index.y * num + index.x * 3], 0);
			}
			if (index.x < m_GridSize.x)
			{
				return new PathNode(m_Entities[m_GridSize.y * num + index.x], 0);
			}
			return new PathNode(m_Entities[m_GridSize.y * num + m_GridSize.x - 1], 2);
		}

		public float3 GetNodePosition(int2 nodeIndex)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			float2 val = (float2.op_Implicit(nodeIndex) - float2.op_Implicit(m_GridSize) * 0.5f) * m_CellSize;
			return new float3(val.x, m_PathHeight, val.y);
		}

		public int2 GetCellIndex(float3 position)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			return math.clamp((int2)(((float3)(ref position)).xz / m_CellSize + float2.op_Implicit(m_GridSize) * 0.5f), int2.op_Implicit(0), m_GridSize - 1);
		}

		public void FindClosestLane(float3 position, ComponentLookup<Curve> curveData, ref Entity lane, ref float curvePos, ref float distance)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			int2 cellIndex = GetCellIndex(position);
			FindClosestLaneImpl(position, curveData, ref lane, ref curvePos, ref distance, GetEntityIndex(cellIndex, LaneDirection.HorizontalZ));
			FindClosestLaneImpl(position, curveData, ref lane, ref curvePos, ref distance, GetEntityIndex(cellIndex, LaneDirection.HorizontalX));
			FindClosestLaneImpl(position, curveData, ref lane, ref curvePos, ref distance, GetEntityIndex(cellIndex, LaneDirection.Diagonal));
			FindClosestLaneImpl(position, curveData, ref lane, ref curvePos, ref distance, GetEntityIndex(new int2(cellIndex.x + 1, cellIndex.y), LaneDirection.HorizontalZ));
			FindClosestLaneImpl(position, curveData, ref lane, ref curvePos, ref distance, GetEntityIndex(new int2(cellIndex.x, cellIndex.y + 1), LaneDirection.HorizontalX));
		}

		private void FindClosestLaneImpl(float3 position, ComponentLookup<Curve> curveData, ref Entity bestLane, ref float bestCurvePos, ref float bestDistance, int entityIndex)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			Entity val = m_Entities[entityIndex];
			if (curveData.HasComponent(val))
			{
				float num2 = default(float);
				float num = MathUtils.Distance(curveData[val].m_Bezier, position, ref num2);
				if (num < bestDistance)
				{
					bestLane = val;
					bestCurvePos = num2;
					bestDistance = num;
				}
			}
		}
	}

	public struct AirwayData : IDisposable
	{
		public AirwayMap helicopterMap { get; private set; }

		public AirwayMap airplaneMap { get; private set; }

		public AirwayData(AirwayMap _helicopterMap, AirwayMap _airplaneMap)
		{
			helicopterMap = _helicopterMap;
			airplaneMap = _airplaneMap;
		}

		public void Dispose()
		{
			helicopterMap.Dispose();
			airplaneMap.Dispose();
		}
	}

	public enum LaneDirection
	{
		HorizontalZ,
		HorizontalX,
		Diagonal,
		DiagonalCross
	}
}
