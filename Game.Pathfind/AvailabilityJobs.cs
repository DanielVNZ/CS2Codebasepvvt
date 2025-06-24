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

public static class AvailabilityJobs
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

		public NodeAvailability m_Availability;

		public EdgeID m_EdgeID;

		public EdgeID m_NextID;

		public NodeData(int accessRequirement, NodeAvailability availability, EdgeID edgeID, EdgeID nextID, FullNode pathNode)
		{
			m_PathNode = pathNode;
			m_NextNodeIndex = -1;
			m_Processed = 0;
			m_AccessRequirement = accessRequirement;
			m_Availability = availability;
			m_EdgeID = edgeID;
			m_NextID = nextID;
		}
	}

	private struct HeapData : ILessThan<HeapData>, IComparable<HeapData>
	{
		public float m_Availability;

		public int m_NodeIndex;

		public HeapData(float availability, int nodeIndex)
		{
			m_Availability = availability;
			m_NodeIndex = nodeIndex;
		}

		public bool LessThan(HeapData other)
		{
			return m_Availability > other.m_Availability;
		}

		public int CompareTo(HeapData other)
		{
			return m_NodeIndex - other.m_NodeIndex;
		}
	}

	private struct NodeAvailability
	{
		public float m_Availability;

		public int m_Provider;

		public NodeAvailability(float availability, int provider)
		{
			m_Availability = availability;
			m_Provider = provider;
		}
	}

	private struct ProviderItem
	{
		public float m_Capacity;

		public float m_Cost;
	}

	private struct AvailabilityExecutor
	{
		private UnsafePathfindData m_PathfindData;

		private Allocator m_Allocator;

		private AvailabilityParameters m_Parameters;

		private UnsafeParallelMultiHashMap<Entity, PathTarget> m_ProviderTargets;

		private UnsafeList<ProviderItem> m_Providers;

		private UnsafeList<int> m_ProviderIndex;

		private UnsafeList<int> m_NodeIndex;

		private UnsafeList<int> m_NodeIndexBits;

		private UnsafeMinHeap<HeapData> m_Heap;

		private UnsafeList<NodeData> m_NodeData;

		public void Initialize(NativePathfindData pathfindData, Allocator allocator, AvailabilityParameters parameters)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			m_PathfindData = pathfindData.GetReadOnlyData();
			m_Allocator = allocator;
			m_Parameters = parameters;
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
			if (m_ProviderTargets.IsCreated)
			{
				m_ProviderTargets.Dispose();
			}
			if (m_Providers.IsCreated)
			{
				m_Providers.Dispose();
				m_ProviderIndex.Dispose();
			}
		}

		public void AddSources(ref UnsafeQueue<PathTarget> pathTargets)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			int count = pathTargets.Count;
			m_ProviderTargets = new UnsafeParallelMultiHashMap<Entity, PathTarget>(count, AllocatorHandle.op_Implicit(m_Allocator));
			PathTarget pathTarget = default(PathTarget);
			while (pathTargets.TryDequeue(ref pathTarget))
			{
				m_ProviderTargets.Add(pathTarget.m_Target, pathTarget);
			}
		}

		public void AddProviders(ref UnsafeQueue<AvailabilityProvider> availabilityProviders)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			int count = availabilityProviders.Count;
			m_Providers = new UnsafeList<ProviderItem>(count, AllocatorHandle.op_Implicit(m_Allocator), (NativeArrayOptions)0);
			m_ProviderIndex = new UnsafeList<int>(count, AllocatorHandle.op_Implicit(m_Allocator), (NativeArrayOptions)0);
			PathTarget pathTarget = default(PathTarget);
			NativeParallelMultiHashMapIterator<Entity> val = default(NativeParallelMultiHashMapIterator<Entity>);
			EdgeID edgeID = default(EdgeID);
			bool3 directions = default(bool3);
			for (int i = 0; i < count; i++)
			{
				AvailabilityProvider availabilityProvider = availabilityProviders.Dequeue();
				ProviderItem providerItem = new ProviderItem
				{
					m_Capacity = availabilityProvider.m_Capacity,
					m_Cost = availabilityProvider.m_Cost * m_Parameters.m_CostFactor
				};
				m_Providers.Add(ref providerItem);
				m_ProviderIndex.Add(ref i);
				float num = 0f;
				if (m_ProviderTargets.TryGetFirstValue(availabilityProvider.m_Provider, ref pathTarget, ref val))
				{
					do
					{
						if (m_PathfindData.m_PathEdges.TryGetValue(pathTarget.m_Entity, ref edgeID))
						{
							ref Edge edge = ref m_PathfindData.GetEdge(edgeID);
							((bool3)(ref directions))._002Ector((edge.m_Specification.m_Flags & EdgeFlags.Forward) != 0 || pathTarget.m_Delta == 1f, (edge.m_Specification.m_Flags & EdgeFlags.AllowMiddle) != 0, (edge.m_Specification.m_Flags & EdgeFlags.Backward) != 0 || pathTarget.m_Delta == 0f);
							num += AddConnections(edgeID, in edge, providerItem, i, pathTarget.m_Delta, directions);
						}
					}
					while (m_ProviderTargets.TryGetNextValue(ref pathTarget, ref val));
				}
				providerItem = m_Providers[i];
				providerItem.m_Cost += num;
				m_Providers[i] = providerItem;
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

		public bool FindAvailabilityNodes()
		{
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
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
				int providerIndex = GetProviderIndex(reference.m_Availability.m_Provider);
				ProviderItem providerItem = m_Providers[providerIndex];
				float num = 0f;
				if (reference.m_NextID.m_Index != -1)
				{
					ref Edge edge = ref m_PathfindData.GetEdge(reference.m_NextID);
					num += CheckNextEdge(reference.m_NextID, reference.m_PathNode, in edge, providerItem, providerIndex);
				}
				else
				{
					int connectionCount = m_PathfindData.GetConnectionCount(reference.m_PathNode.m_NodeID);
					((int2)(ref val))._002Ector(-1, reference.m_AccessRequirement);
					FullNode pathNode = reference.m_PathNode;
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
								num += CheckNextEdge(edgeID2, pathNode, in edge2, providerItem, providerIndex);
							}
						}
					}
				}
				m_Providers.ElementAt(providerIndex).m_Cost += num;
			}
			return m_NodeData.Length != 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private float CheckNextEdge(EdgeID nextID, FullNode pathNode, in Edge edge, ProviderItem providerItem, int providerIndex)
		{
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
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
					return 0f;
				}
				startDelta = pathNode.m_CurvePos;
				((bool3)(ref directions))._002Ector((edge.m_Specification.m_Flags & EdgeFlags.Forward) != 0, (edge.m_Specification.m_Flags & EdgeFlags.AllowMiddle) != 0, (edge.m_Specification.m_Flags & EdgeFlags.Backward) != 0);
			}
			return AddConnections(nextID, in edge, providerItem, providerIndex, startDelta, directions);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private float AddConnections(EdgeID id, in Edge edge, ProviderItem providerItem, int providerIndex, float startDelta, bool3 directions)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			float num = 0f;
			if (directions.x)
			{
				float num2 = PathUtils.CalculateCost(in edge.m_Specification, in m_Parameters, new float2(startDelta, 1f));
				num += num2;
				AddHeapData(id, in edge, GetAvailability(providerItem, num2), providerIndex, new FullNode(edge.m_EndID, edge.m_EndCurvePos));
			}
			if (directions.y)
			{
				int connectionCount = m_PathfindData.GetConnectionCount(edge.m_MiddleID);
				if (connectionCount != 0)
				{
					int2 val = default(int2);
					((int2)(ref val))._002Ector(-1, edge.m_Specification.m_AccessRequirement);
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
								float num3 = PathUtils.CalculateCost(in edge.m_Specification, in m_Parameters, new float2(startDelta, startCurvePos));
								num += num3;
								AddHeapData(id, edgeID, in edge, GetAvailability(providerItem, num3), providerIndex, new FullNode(edge.m_StartID, startCurvePos));
							}
						}
						if (edge.m_MiddleID.Equals(edge2.m_EndID) & ((edge2.m_Specification.m_Flags & EdgeFlags.Backward) != 0))
						{
							float endCurvePos = edge2.m_EndCurvePos;
							if ((directions.x && endCurvePos >= startDelta) | (directions.z && endCurvePos <= startDelta))
							{
								float num4 = PathUtils.CalculateCost(in edge.m_Specification, in m_Parameters, new float2(startDelta, endCurvePos));
								num += num4;
								AddHeapData(id, edgeID, in edge, GetAvailability(providerItem, num4), providerIndex, new FullNode(edge.m_EndID, endCurvePos));
							}
						}
					}
				}
			}
			if (directions.z)
			{
				float num5 = PathUtils.CalculateCost(in edge.m_Specification, in m_Parameters, new float2(startDelta, 0f));
				num += num5;
				AddHeapData(id, in edge, GetAvailability(providerItem, num5), providerIndex, new FullNode(edge.m_StartID, edge.m_StartCurvePos));
			}
			return num;
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
		private void AddHeapData(EdgeID id, in Edge edge, float availability, int providerIndex, FullNode pathNode)
		{
			if (GetOrAddNodeIndex(pathNode, out var nodeIndex))
			{
				ref NodeData reference = ref m_NodeData.ElementAt(nodeIndex);
				int providerIndex2 = GetProviderIndex(reference.m_Availability.m_Provider);
				if (providerIndex != providerIndex2)
				{
					MergeProviders(providerIndex, providerIndex2);
				}
				if (reference.m_Processed == 0 && availability < reference.m_Availability.m_Availability)
				{
					reference = new NodeData(edge.m_Specification.m_AccessRequirement, new NodeAvailability(availability, providerIndex), id, new EdgeID
					{
						m_Index = -1
					}, pathNode);
					HeapInsert(new HeapData(availability, nodeIndex));
				}
			}
			else
			{
				ref UnsafeList<NodeData> nodeData = ref m_NodeData;
				NodeData nodeData2 = new NodeData(edge.m_Specification.m_AccessRequirement, new NodeAvailability(availability, providerIndex), id, new EdgeID
				{
					m_Index = -1
				}, pathNode);
				nodeData.Add(ref nodeData2);
				HeapInsert(new HeapData(availability, nodeIndex));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AddHeapData(EdgeID id, EdgeID id2, in Edge edge, float availability, int providerIndex, FullNode pathNode)
		{
			if (GetOrAddNodeIndex(pathNode, out var nodeIndex))
			{
				ref NodeData reference = ref m_NodeData.ElementAt(nodeIndex);
				int providerIndex2 = GetProviderIndex(reference.m_Availability.m_Provider);
				if (providerIndex != providerIndex2)
				{
					MergeProviders(providerIndex, providerIndex2);
				}
				if (reference.m_Processed == 0)
				{
					if (availability < reference.m_Availability.m_Availability)
					{
						reference = new NodeData(edge.m_Specification.m_AccessRequirement, new NodeAvailability(availability, providerIndex), id, id2, pathNode);
						HeapInsert(new HeapData(availability, nodeIndex));
					}
					else if (!id2.Equals(reference.m_NextID))
					{
						reference.m_NextID.m_Index = -1;
					}
				}
			}
			else
			{
				ref UnsafeList<NodeData> nodeData = ref m_NodeData;
				NodeData nodeData2 = new NodeData(edge.m_Specification.m_AccessRequirement, new NodeAvailability(availability, providerIndex), id, id2, pathNode);
				nodeData.Add(ref nodeData2);
				HeapInsert(new HeapData(availability, nodeIndex));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool DisallowConnection(PathSpecification newSpec)
		{
			if ((newSpec.m_Methods & PathMethod.Road) == 0)
			{
				return true;
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void MergeProviders(int providerIndex1, int providerIndex2)
		{
			ProviderItem providerItem = m_Providers[providerIndex1];
			ProviderItem providerItem2 = m_Providers[providerIndex2];
			providerItem.m_Capacity += providerItem2.m_Capacity;
			providerItem.m_Cost += providerItem2.m_Cost;
			providerItem.m_Capacity *= (1f + providerItem.m_Cost) / (2f + providerItem.m_Cost);
			m_Providers[providerIndex1] = providerItem;
			m_ProviderIndex[providerIndex2] = providerIndex1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private float GetAvailability(ProviderItem providerItem, float cost)
		{
			return providerItem.m_Capacity / (1f + providerItem.m_Cost + cost);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int GetProviderIndex(int storedIndex)
		{
			int num = m_ProviderIndex[storedIndex];
			if (num != storedIndex)
			{
				int num2 = storedIndex;
				storedIndex = num;
				num = m_ProviderIndex[storedIndex];
				if (num != storedIndex)
				{
					do
					{
						storedIndex = num;
						num = m_ProviderIndex[storedIndex];
					}
					while (num != storedIndex);
					m_ProviderIndex[num2] = num;
				}
			}
			return num;
		}

		public void FillResults(ref UnsafeList<AvailabilityResult> results)
		{
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
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
							if (reference2.m_Processed != 0)
							{
								AvailabilityResult availabilityResult = new AvailabilityResult
								{
									m_Target = edge.m_Owner,
									m_Availability = NormalizeAvailability(new float2(reference.m_Availability.m_Availability, reference2.m_Availability.m_Availability), m_Parameters)
								};
								results.Add(ref availabilityResult);
							}
						}
					}
					else if (reference.m_PathNode.Equals(new FullNode(edge.m_EndID, edge.m_EndCurvePos)) && (edge.m_Specification.m_Flags & (EdgeFlags.Forward | EdgeFlags.Backward)) == EdgeFlags.Backward && TryGetNodeIndex(new FullNode(edge.m_StartID, edge.m_StartCurvePos), out nodeIndex2))
					{
						ref NodeData reference3 = ref m_NodeData.ElementAt(nodeIndex2);
						if (reference3.m_Processed != 0)
						{
							AvailabilityResult availabilityResult2 = new AvailabilityResult
							{
								m_Target = edge.m_Owner,
								m_Availability = NormalizeAvailability(new float2(reference3.m_Availability.m_Availability, reference.m_Availability.m_Availability), m_Parameters)
							};
							results.Add(ref availabilityResult2);
						}
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private float2 NormalizeAvailability(float2 availability, AvailabilityParameters availabilityParameters)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return availability * availabilityParameters.m_ResultFactor;
		}
	}

	[BurstCompile]
	public struct AvailabilityJob : IJob
	{
		[ReadOnly]
		public NativePathfindData m_PathfindData;

		public AvailabilityAction m_Action;

		public void Execute()
		{
			Execute(m_PathfindData, (Allocator)2, ref m_Action.data);
		}

		public static void Execute(NativePathfindData pathfindData, Allocator allocator, ref AvailabilityActionData actionData)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			if (!actionData.m_Providers.IsEmpty())
			{
				AvailabilityExecutor availabilityExecutor = default(AvailabilityExecutor);
				availabilityExecutor.Initialize(pathfindData, allocator, actionData.m_Parameters);
				availabilityExecutor.AddSources(ref actionData.m_Sources);
				availabilityExecutor.AddProviders(ref actionData.m_Providers);
				if (availabilityExecutor.FindAvailabilityNodes())
				{
					availabilityExecutor.FillResults(ref actionData.m_Results);
				}
				availabilityExecutor.Release();
			}
		}
	}

	public struct ResultItem
	{
		public Entity m_Owner;

		public UnsafeList<AvailabilityResult> m_Results;
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
		public BufferLookup<AvailabilityElement> m_AvailabilityElements;

		public void Execute(int index)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			ResultItem resultItem = m_ResultItems[index];
			if (!m_AvailabilityElements.HasBuffer(resultItem.m_Owner))
			{
				return;
			}
			NativeParallelHashMap<Entity, float2> val = default(NativeParallelHashMap<Entity, float2>);
			val._002Ector(1000, AllocatorHandle.op_Implicit((Allocator)2));
			float2 val2 = default(float2);
			for (int i = 0; i < resultItem.m_Results.Length; i++)
			{
				AvailabilityResult availabilityResult = resultItem.m_Results[i];
				if (!m_EdgeLaneData.HasComponent(availabilityResult.m_Target) || !m_OwnerData.HasComponent(availabilityResult.m_Target))
				{
					continue;
				}
				EdgeLane edgeLane = m_EdgeLaneData[availabilityResult.m_Target];
				Owner owner = m_OwnerData[availabilityResult.m_Target];
				float2 availability = GetAvailability(availabilityResult.m_Availability, edgeLane.m_EdgeDelta);
				if (val.TryGetValue(owner.m_Owner, ref val2))
				{
					availability = math.max(val2, availability);
					if (math.any(availability != val2))
					{
						val[owner.m_Owner] = availability;
					}
				}
				else
				{
					val.Add(owner.m_Owner, availability);
				}
			}
			DynamicBuffer<AvailabilityElement> val3 = m_AvailabilityElements[resultItem.m_Owner];
			val3.Clear();
			Enumerator<Entity, float2> enumerator = val.GetEnumerator();
			while (enumerator.MoveNext())
			{
				val3.Add(new AvailabilityElement
				{
					m_Edge = enumerator.Current.Key,
					m_Availability = enumerator.Current.Value
				});
			}
			enumerator.Dispose();
			val.Dispose();
		}

		private static float2 GetAvailability(float2 availability, float2 edgeDelta)
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
			return math.select(math.select(float2.op_Implicit(0f), ((float2)(ref availability)).yx, ((float2)(ref edgeDelta)).yx == val), availability, edgeDelta == val);
		}
	}
}
