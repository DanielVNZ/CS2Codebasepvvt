using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Common;
using Game.Net;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Game.Pathfind;

public static class CoverageJobs
{
	private struct FullNode : IEquatable<FullNode>
	{
		public NodeID m_NodeID;

		public float m_CurvePos;

		public FullNode(NodeID nodeID, float curvePos)
		{
			m_NodeID = nodeID;
			m_CurvePos = curvePos;
		}

		public bool Equals(FullNode other)
		{
			if (m_NodeID.Equals(other.m_NodeID))
			{
				return m_CurvePos == other.m_CurvePos;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (m_NodeID.m_Index >> 2) ^ math.asint(m_CurvePos);
		}
	}

	private struct NodeData
	{
		public FullNode m_PathNode;

		public int m_NextNodeIndex;

		public int m_Processed;

		public int m_AccessRequirement;

		public float2 m_Costs;

		public EdgeID m_EdgeID;

		public EdgeID m_NextID;

		public NodeData(int accessRequirement, float cost, float distance, EdgeID edgeID, EdgeID nextID, FullNode pathNode)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			m_PathNode = pathNode;
			m_NextNodeIndex = -1;
			m_Processed = 0;
			m_AccessRequirement = accessRequirement;
			m_Costs = new float2(cost, distance);
			m_EdgeID = edgeID;
			m_NextID = nextID;
		}
	}

	private struct HeapData : ILessThan<HeapData>, IComparable<HeapData>
	{
		public float m_Cost;

		public int m_NodeIndex;

		public HeapData(float cost, int nodeIndex)
		{
			m_Cost = cost;
			m_NodeIndex = nodeIndex;
		}

		public bool LessThan(HeapData other)
		{
			return m_Cost < other.m_Cost;
		}

		public int CompareTo(HeapData other)
		{
			return m_NodeIndex - other.m_NodeIndex;
		}
	}

	private struct CoverageExecutor
	{
		private UnsafePathfindData m_PathfindData;

		private CoverageParameters m_Parameters;

		private float4 m_MinDistance;

		private float4 m_MaxDistance;

		private UnsafeList<int> m_NodeIndex;

		private UnsafeList<int> m_NodeIndexBits;

		private UnsafeMinHeap<HeapData> m_Heap;

		private UnsafeList<NodeData> m_NodeData;

		public void Initialize(NativePathfindData pathfindData, Allocator allocator, CoverageParameters parameters)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			m_PathfindData = pathfindData.GetReadOnlyData();
			m_Parameters = parameters;
			m_MinDistance = parameters.m_Range * new float4(0f, 0.6f, 0f, 0.6f);
			m_MaxDistance = parameters.m_Range * new float4(2f, 1.2f, 2f, 1.2f);
			int num = (math.max(m_PathfindData.GetNodeIDSize(), m_PathfindData.m_Edges.Length) >> 5) + 1;
			int num2 = (num >> 5) + 1;
			m_NodeIndex = new UnsafeList<int>(num, AllocatorHandle.op_Implicit(allocator), (NativeArrayOptions)0);
			m_NodeIndexBits = new UnsafeList<int>(num2, AllocatorHandle.op_Implicit(allocator), (NativeArrayOptions)0);
			m_Heap = new UnsafeMinHeap<HeapData>(1000, allocator);
			m_NodeData = new UnsafeList<NodeData>(10000, AllocatorHandle.op_Implicit(allocator), (NativeArrayOptions)0);
			m_NodeIndex.Resize(num, (NativeArrayOptions)0);
			m_NodeIndexBits.Resize(num2, (NativeArrayOptions)1);
		}

		public void Release()
		{
			if (m_NodeData.IsCreated)
			{
				m_NodeIndex.Dispose();
				m_NodeIndexBits.Dispose();
				m_Heap.Dispose();
				m_NodeData.Dispose();
			}
		}

		public void AddSources(ref UnsafeQueue<PathTarget> pathTargets)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			PathTarget pathTarget = default(PathTarget);
			EdgeID edgeID = default(EdgeID);
			bool3 directions = default(bool3);
			while (pathTargets.TryDequeue(ref pathTarget))
			{
				if (m_PathfindData.m_PathEdges.TryGetValue(pathTarget.m_Entity, ref edgeID))
				{
					ref Edge edge = ref m_PathfindData.GetEdge(edgeID);
					((bool3)(ref directions))._002Ector((edge.m_Specification.m_Flags & EdgeFlags.Forward) != 0 || pathTarget.m_Delta == 1f, (edge.m_Specification.m_Flags & EdgeFlags.AllowMiddle) != 0, (edge.m_Specification.m_Flags & EdgeFlags.Backward) != 0 || pathTarget.m_Delta == 0f);
					AddConnections(edgeID, in edge, new float2(pathTarget.m_Cost, 0f), pathTarget.m_Delta, directions);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool HeapExtract(out HeapData heapData)
		{
			if (m_Heap.Length != 0)
			{
				heapData = m_Heap.Extract();
				return true;
			}
			heapData = default(HeapData);
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void HeapInsert(HeapData heapData)
		{
			m_Heap.Insert(heapData);
		}

		public bool FindCoveredNodes()
		{
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			bool result = false;
			HeapData heapData;
			int2 val = default(int2);
			while (HeapExtract(out heapData))
			{
				ref NodeData reference = ref m_NodeData.ElementAt(heapData.m_NodeIndex);
				if (reference.m_Processed != 0)
				{
					continue;
				}
				reference.m_Processed = 1;
				result = true;
				if (!(reference.m_Costs.y < m_MaxDistance.y))
				{
					continue;
				}
				if (reference.m_NextID.m_Index != -1)
				{
					ref Edge edge = ref m_PathfindData.GetEdge(reference.m_NextID);
					CheckNextEdge(reference.m_NextID, reference.m_PathNode, reference.m_Costs, in edge);
					continue;
				}
				int connectionCount = m_PathfindData.GetConnectionCount(reference.m_PathNode.m_NodeID);
				((int2)(ref val))._002Ector(-1, reference.m_AccessRequirement);
				FullNode pathNode = reference.m_PathNode;
				float2 costs = reference.m_Costs;
				EdgeID edgeID = reference.m_EdgeID;
				for (int i = 0; i < connectionCount; i++)
				{
					EdgeID edgeID2 = new EdgeID
					{
						m_Index = m_PathfindData.GetConnection(pathNode.m_NodeID, i)
					};
					int accessRequirement = m_PathfindData.GetAccessRequirement(pathNode.m_NodeID, i);
					if (!edgeID.Equals(edgeID2) && !math.all(val != accessRequirement))
					{
						ref Edge edge2 = ref m_PathfindData.GetEdge(edgeID2);
						if (!DisallowConnection(edge2.m_Specification))
						{
							CheckNextEdge(edgeID2, pathNode, costs, in edge2);
						}
					}
				}
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void CheckNextEdge(EdgeID nextID, FullNode pathNode, float2 baseCosts, in Edge edge)
		{
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			float startDelta;
			bool3 directions = default(bool3);
			if (pathNode.Equals(new FullNode(edge.m_StartID, edge.m_StartCurvePos)))
			{
				startDelta = 0f;
				((bool3)(ref directions))._002Ector((edge.m_Specification.m_Flags & EdgeFlags.Forward) != 0, (edge.m_Specification.m_Flags & (EdgeFlags.Forward | EdgeFlags.AllowMiddle)) == (EdgeFlags.Forward | EdgeFlags.AllowMiddle), false);
			}
			else if (pathNode.Equals(new FullNode(edge.m_EndID, edge.m_EndCurvePos)))
			{
				startDelta = 1f;
				((bool3)(ref directions))._002Ector(false, (edge.m_Specification.m_Flags & (EdgeFlags.Backward | EdgeFlags.AllowMiddle)) == (EdgeFlags.Backward | EdgeFlags.AllowMiddle), (edge.m_Specification.m_Flags & EdgeFlags.Backward) != 0);
			}
			else
			{
				if (!pathNode.m_NodeID.Equals(edge.m_MiddleID))
				{
					return;
				}
				startDelta = pathNode.m_CurvePos;
				((bool3)(ref directions))._002Ector((edge.m_Specification.m_Flags & EdgeFlags.Forward) != 0, (edge.m_Specification.m_Flags & EdgeFlags.AllowMiddle) != 0, (edge.m_Specification.m_Flags & EdgeFlags.Backward) != 0);
			}
			AddConnections(nextID, in edge, baseCosts, startDelta, directions);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AddConnections(EdgeID id, in Edge edge, float2 baseCosts, float startDelta, bool3 directions)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			if (directions.x)
			{
				FullNode pathNode = new FullNode(edge.m_EndID, edge.m_EndCurvePos);
				float2 edgeDelta = default(float2);
				((float2)(ref edgeDelta))._002Ector(startDelta, 1f);
				AddHeapData(id, in edge, baseCosts, pathNode, edgeDelta);
			}
			if (directions.y)
			{
				int connectionCount = m_PathfindData.GetConnectionCount(edge.m_MiddleID);
				if (connectionCount != 0)
				{
					int2 val = default(int2);
					((int2)(ref val))._002Ector(-1, edge.m_Specification.m_AccessRequirement);
					float2 edgeDelta2 = default(float2);
					float2 edgeDelta3 = default(float2);
					for (int i = 0; i < connectionCount; i++)
					{
						EdgeID edgeID = new EdgeID
						{
							m_Index = m_PathfindData.GetConnection(edge.m_MiddleID, i)
						};
						int accessRequirement = m_PathfindData.GetAccessRequirement(edge.m_MiddleID, i);
						if (id.Equals(edgeID) || math.all(val != accessRequirement))
						{
							continue;
						}
						ref Edge edge2 = ref m_PathfindData.GetEdge(edgeID);
						if (DisallowConnection(edge2.m_Specification))
						{
							continue;
						}
						if (edge.m_MiddleID.Equals(edge2.m_StartID) & ((edge2.m_Specification.m_Flags & EdgeFlags.Forward) != 0))
						{
							float startCurvePos = edge2.m_StartCurvePos;
							if ((directions.x && startCurvePos >= startDelta) | (directions.z && startCurvePos <= startDelta))
							{
								FullNode pathNode2 = new FullNode(edge2.m_StartID, startCurvePos);
								((float2)(ref edgeDelta2))._002Ector(startDelta, startCurvePos);
								AddHeapData(id, edgeID, in edge, baseCosts, pathNode2, edgeDelta2);
							}
						}
						if (edge.m_MiddleID.Equals(edge2.m_EndID) & ((edge2.m_Specification.m_Flags & EdgeFlags.Backward) != 0))
						{
							float endCurvePos = edge2.m_EndCurvePos;
							if ((directions.x && endCurvePos >= startDelta) | (directions.z && endCurvePos <= startDelta))
							{
								FullNode pathNode3 = new FullNode(edge2.m_EndID, endCurvePos);
								((float2)(ref edgeDelta3))._002Ector(startDelta, endCurvePos);
								AddHeapData(id, edgeID, in edge, baseCosts, pathNode3, edgeDelta3);
							}
						}
					}
				}
			}
			if (directions.z)
			{
				FullNode pathNode4 = new FullNode(edge.m_StartID, edge.m_StartCurvePos);
				float2 edgeDelta4 = default(float2);
				((float2)(ref edgeDelta4))._002Ector(startDelta, 0f);
				AddHeapData(id, in edge, baseCosts, pathNode4, edgeDelta4);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool GetOrAddNodeIndex(FullNode pathNode, out int nodeIndex)
		{
			int num = math.abs(pathNode.m_NodeID.m_Index) >> 5;
			int num2 = num >> 5;
			int num3 = 1 << (num & 0x1F);
			if ((m_NodeIndexBits[num2] & num3) != 0)
			{
				nodeIndex = m_NodeIndex[num];
				ref NodeData reference = ref m_NodeData.ElementAt(nodeIndex);
				while (true)
				{
					if (reference.m_PathNode.Equals(pathNode))
					{
						return true;
					}
					nodeIndex = reference.m_NextNodeIndex;
					if (nodeIndex == -1)
					{
						break;
					}
					reference = ref m_NodeData.ElementAt(nodeIndex);
				}
				nodeIndex = m_NodeData.Length;
				reference.m_NextNodeIndex = nodeIndex;
			}
			else
			{
				ref UnsafeList<int> nodeIndexBits = ref m_NodeIndexBits;
				int num4 = num2;
				nodeIndexBits[num4] |= num3;
				nodeIndex = m_NodeData.Length;
				m_NodeIndex[num] = nodeIndex;
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool TryGetNodeIndex(FullNode pathNode, out int nodeIndex)
		{
			int num = math.abs(pathNode.m_NodeID.m_Index) >> 5;
			int num2 = num >> 5;
			int num3 = 1 << (num & 0x1F);
			if ((m_NodeIndexBits[num2] & num3) != 0)
			{
				nodeIndex = m_NodeIndex[num];
				ref NodeData reference = ref m_NodeData.ElementAt(nodeIndex);
				while (true)
				{
					if (reference.m_PathNode.Equals(pathNode))
					{
						return true;
					}
					nodeIndex = reference.m_NextNodeIndex;
					if (nodeIndex == -1)
					{
						break;
					}
					reference = ref m_NodeData.ElementAt(nodeIndex);
				}
			}
			else
			{
				nodeIndex = -1;
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AddHeapData(EdgeID id, in Edge edge, float2 baseCosts, FullNode pathNode, float2 edgeDelta)
		{
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			if (GetOrAddNodeIndex(pathNode, out var nodeIndex))
			{
				ref NodeData reference = ref m_NodeData.ElementAt(nodeIndex);
				if (reference.m_Processed == 0)
				{
					float num = baseCosts.x + PathUtils.CalculateCost(in edge.m_Specification, in m_Parameters, edgeDelta);
					if (num < reference.m_Costs.x)
					{
						float distance = baseCosts.y + edge.m_Specification.m_Length * math.abs(edgeDelta.x - edgeDelta.y);
						reference = new NodeData(edge.m_Specification.m_AccessRequirement, num, distance, id, new EdgeID
						{
							m_Index = -1
						}, pathNode);
						HeapInsert(new HeapData(num, nodeIndex));
					}
				}
			}
			else
			{
				float cost = baseCosts.x + PathUtils.CalculateCost(in edge.m_Specification, in m_Parameters, edgeDelta);
				float distance2 = baseCosts.y + edge.m_Specification.m_Length * math.abs(edgeDelta.x - edgeDelta.y);
				ref UnsafeList<NodeData> nodeData = ref m_NodeData;
				NodeData nodeData2 = new NodeData(edge.m_Specification.m_AccessRequirement, cost, distance2, id, new EdgeID
				{
					m_Index = -1
				}, pathNode);
				nodeData.Add(ref nodeData2);
				HeapInsert(new HeapData(cost, nodeIndex));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AddHeapData(EdgeID id, EdgeID id2, in Edge edge, float2 baseCosts, FullNode pathNode, float2 edgeDelta)
		{
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			if (GetOrAddNodeIndex(pathNode, out var nodeIndex))
			{
				ref NodeData reference = ref m_NodeData.ElementAt(nodeIndex);
				if (reference.m_Processed == 0)
				{
					float num = baseCosts.x + PathUtils.CalculateCost(in edge.m_Specification, in m_Parameters, edgeDelta);
					if (num < reference.m_Costs.x)
					{
						float distance = baseCosts.y + edge.m_Specification.m_Length * math.abs(edgeDelta.x - edgeDelta.y);
						reference = new NodeData(edge.m_Specification.m_AccessRequirement, num, distance, id, id2, pathNode);
						HeapInsert(new HeapData(num, nodeIndex));
					}
					else if (!id2.Equals(reference.m_NextID))
					{
						reference.m_NextID.m_Index = -1;
					}
				}
			}
			else
			{
				float cost = baseCosts.x + PathUtils.CalculateCost(in edge.m_Specification, in m_Parameters, edgeDelta);
				float distance2 = baseCosts.y + edge.m_Specification.m_Length * math.abs(edgeDelta.x - edgeDelta.y);
				ref UnsafeList<NodeData> nodeData = ref m_NodeData;
				NodeData nodeData2 = new NodeData(edge.m_Specification.m_AccessRequirement, cost, distance2, id, id2, pathNode);
				nodeData.Add(ref nodeData2);
				HeapInsert(new HeapData(cost, nodeIndex));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool DisallowConnection(PathSpecification newSpec)
		{
			if ((newSpec.m_Methods & m_Parameters.m_Methods) == 0)
			{
				return true;
			}
			return false;
		}

		public void FillResults(ref UnsafeList<CoverageResult> results)
		{
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_NodeData.Length; i++)
			{
				ref NodeData reference = ref m_NodeData.ElementAt(i);
				if (reference.m_Processed == 0)
				{
					continue;
				}
				int connectionCount = m_PathfindData.GetConnectionCount(reference.m_PathNode.m_NodeID);
				for (int j = 0; j < connectionCount; j++)
				{
					EdgeID edgeID = new EdgeID
					{
						m_Index = m_PathfindData.GetConnection(reference.m_PathNode.m_NodeID, j)
					};
					ref Edge edge = ref m_PathfindData.GetEdge(edgeID);
					int nodeIndex2;
					if (reference.m_PathNode.Equals(new FullNode(edge.m_StartID, edge.m_StartCurvePos)) && (edge.m_Specification.m_Flags & EdgeFlags.Forward) != 0)
					{
						if (TryGetNodeIndex(new FullNode(edge.m_EndID, edge.m_EndCurvePos), out var nodeIndex))
						{
							ref NodeData reference2 = ref m_NodeData.ElementAt(nodeIndex);
							if (reference2.m_Processed != 0 && math.min(reference.m_Costs.y, reference2.m_Costs.y) < m_MaxDistance.y)
							{
								float4 val = (new float4(reference.m_Costs, reference2.m_Costs) - m_MinDistance) / (m_MaxDistance - m_MinDistance);
								CoverageResult coverageResult = new CoverageResult
								{
									m_Target = edge.m_Owner,
									m_TargetCost = math.saturate(math.max(((float4)(ref val)).xz, ((float4)(ref val)).yw))
								};
								results.Add(ref coverageResult);
							}
						}
					}
					else if (reference.m_PathNode.Equals(new FullNode(edge.m_EndID, edge.m_EndCurvePos)) && (edge.m_Specification.m_Flags & (EdgeFlags.Forward | EdgeFlags.Backward)) == EdgeFlags.Backward && TryGetNodeIndex(new FullNode(edge.m_StartID, edge.m_StartCurvePos), out nodeIndex2))
					{
						ref NodeData reference3 = ref m_NodeData.ElementAt(nodeIndex2);
						if (reference3.m_Processed != 0 && math.min(reference.m_Costs.y, reference3.m_Costs.y) < m_MaxDistance.y)
						{
							float4 val2 = (new float4(reference3.m_Costs, reference.m_Costs) - m_MinDistance) / (m_MaxDistance - m_MinDistance);
							CoverageResult coverageResult2 = new CoverageResult
							{
								m_Target = edge.m_Owner,
								m_TargetCost = math.saturate(math.max(((float4)(ref val2)).xz, ((float4)(ref val2)).yw))
							};
							results.Add(ref coverageResult2);
						}
					}
				}
			}
		}
	}

	[BurstCompile]
	public struct CoverageJob : IJob
	{
		[ReadOnly]
		public NativePathfindData m_PathfindData;

		public CoverageAction m_Action;

		public void Execute()
		{
			Execute(m_PathfindData, (Allocator)2, ref m_Action.data);
		}

		public static void Execute(NativePathfindData pathfindData, Allocator allocator, ref CoverageActionData actionData)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			if (!actionData.m_Sources.IsEmpty())
			{
				CoverageExecutor coverageExecutor = default(CoverageExecutor);
				coverageExecutor.Initialize(pathfindData, allocator, actionData.m_Parameters);
				coverageExecutor.AddSources(ref actionData.m_Sources);
				if (coverageExecutor.FindCoveredNodes())
				{
					coverageExecutor.FillResults(ref actionData.m_Results);
				}
				coverageExecutor.Release();
			}
		}
	}

	public struct ResultItem
	{
		public Entity m_Owner;

		public UnsafeList<CoverageResult> m_Results;
	}

	[BurstCompile]
	public struct ProcessResultsJob : IJobParallelFor
	{
		[ReadOnly]
		public NativeList<ResultItem> m_ResultItems;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<EdgeLane> m_EdgeLaneData;

		[NativeDisableParallelForRestriction]
		public BufferLookup<CoverageElement> m_CoverageElements;

		public void Execute(int index)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			ResultItem resultItem = m_ResultItems[index];
			if (!m_CoverageElements.HasBuffer(resultItem.m_Owner))
			{
				return;
			}
			NativeParallelHashMap<Entity, float2> val = default(NativeParallelHashMap<Entity, float2>);
			val._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
			NativeList<Entity> val2 = default(NativeList<Entity>);
			val2._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
			float2 val3 = default(float2);
			for (int i = 0; i < resultItem.m_Results.Length; i++)
			{
				CoverageResult coverageResult = resultItem.m_Results[i];
				if (!m_EdgeLaneData.HasComponent(coverageResult.m_Target))
				{
					continue;
				}
				EdgeLane edgeLane = m_EdgeLaneData[coverageResult.m_Target];
				Owner owner = m_OwnerData[coverageResult.m_Target];
				float2 cost = GetCost(coverageResult.m_TargetCost, edgeLane.m_EdgeDelta);
				if (val.TryGetValue(owner.m_Owner, ref val3))
				{
					if (math.any(cost < val3))
					{
						cost = math.min(val3, cost);
						val.Remove(owner.m_Owner);
						val.TryAdd(owner.m_Owner, cost);
					}
				}
				else
				{
					val.TryAdd(owner.m_Owner, cost);
					val2.Add(ref owner.m_Owner);
				}
			}
			DynamicBuffer<CoverageElement> val4 = m_CoverageElements[resultItem.m_Owner];
			val4.Clear();
			for (int j = 0; j < val2.Length; j++)
			{
				CoverageElement coverageElement = new CoverageElement
				{
					m_Edge = val2[j]
				};
				if (val.TryGetValue(coverageElement.m_Edge, ref coverageElement.m_Cost))
				{
					val4.Add(coverageElement);
				}
			}
			val.Dispose();
			val2.Dispose();
		}

		private static float2 GetCost(float2 cost, float2 edgeDelta)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			float2 val = default(float2);
			((float2)(ref val))._002Ector(0f, 1f);
			return math.select(math.select(float2.op_Implicit(float.MaxValue), ((float2)(ref cost)).yx, ((float2)(ref edgeDelta)).yx == val), cost, edgeDelta == val);
		}
	}
}
