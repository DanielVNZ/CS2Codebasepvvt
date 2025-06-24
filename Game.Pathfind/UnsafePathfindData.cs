using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Pathfind;

[GenerateTestsForBurstCompatibility]
public struct UnsafePathfindData : IDisposable
{
	public UnsafeList<Edge> m_Edges;

	public UnsafeList<EdgeID> m_FreeIDs;

	public UnsafeHashMap<Entity, EdgeID> m_PathEdges;

	public UnsafeHashMap<Entity, EdgeID> m_SecondaryEdges;

	public UnsafeHashMap<PathNode, NodeID> m_NodeIDs;

	public UnsafeHashMap<NodeID, PathNode> m_PathNodes;

	private UnsafeHeapAllocator m_ConnectionAllocator;

	private unsafe void* m_Connections;

	private unsafe void* m_ReversedConnections;

	private int m_NodeCount;

	private readonly Allocator m_AllocatorLabel;

	private const int NODE_META_SIZE = 1;

	public unsafe UnsafePathfindData(Allocator allocator)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		m_Edges = new UnsafeList<Edge>(1000, AllocatorHandle.op_Implicit(allocator), (NativeArrayOptions)0);
		m_FreeIDs = new UnsafeList<EdgeID>(100, AllocatorHandle.op_Implicit(allocator), (NativeArrayOptions)0);
		m_PathEdges = new UnsafeHashMap<Entity, EdgeID>(1000, AllocatorHandle.op_Implicit(allocator));
		m_SecondaryEdges = new UnsafeHashMap<Entity, EdgeID>(1000, AllocatorHandle.op_Implicit(allocator));
		m_NodeIDs = new UnsafeHashMap<PathNode, NodeID>(1000, AllocatorHandle.op_Implicit(allocator));
		m_PathNodes = new UnsafeHashMap<NodeID, PathNode>(1000, AllocatorHandle.op_Implicit(allocator));
		m_ConnectionAllocator = new UnsafeHeapAllocator(1000u, 1u, allocator);
		m_Connections = UnsafeUtility.Malloc((long)(((UnsafeHeapAllocator)(ref m_ConnectionAllocator)).Size * 4), (int)(((UnsafeHeapAllocator)(ref m_ConnectionAllocator)).MinimumAlignment * 4), allocator);
		m_ReversedConnections = UnsafeUtility.Malloc((long)(((UnsafeHeapAllocator)(ref m_ConnectionAllocator)).Size * 4), (int)(((UnsafeHeapAllocator)(ref m_ConnectionAllocator)).MinimumAlignment * 4), allocator);
		m_AllocatorLabel = allocator;
		m_NodeCount = 0;
	}

	public unsafe void Dispose()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		m_Edges.Dispose();
		m_FreeIDs.Dispose();
		m_PathEdges.Dispose();
		m_SecondaryEdges.Dispose();
		m_NodeIDs.Dispose();
		m_PathNodes.Dispose();
		((UnsafeHeapAllocator)(ref m_ConnectionAllocator)).Dispose();
		UnsafeUtility.Free(m_Connections, m_AllocatorLabel);
		UnsafeUtility.Free(m_ReversedConnections, m_AllocatorLabel);
	}

	public void Clear()
	{
		m_Edges.Clear();
		m_FreeIDs.Clear();
		m_PathEdges.Clear();
		m_SecondaryEdges.Clear();
		m_NodeIDs.Clear();
		m_PathNodes.Clear();
		((UnsafeHeapAllocator)(ref m_ConnectionAllocator)).Clear();
		m_NodeCount = 0;
	}

	public unsafe void GetMemoryStats(out uint used, out uint allocated)
	{
		used = (uint)(m_Edges.Length * (System.Runtime.CompilerServices.Unsafe.SizeOf<Edge>() + System.Runtime.CompilerServices.Unsafe.SizeOf<Entity>() + sizeof(EdgeID)) + m_FreeIDs.Length * sizeof(EdgeID) + m_NodeCount * (sizeof(PathNode) * 2 + sizeof(NodeID) * 2)) + ((UnsafeHeapAllocator)(ref m_ConnectionAllocator)).UsedSpace * 4;
		allocated = (uint)(m_Edges.Capacity * System.Runtime.CompilerServices.Unsafe.SizeOf<Edge>() + m_FreeIDs.Capacity * sizeof(EdgeID) + (m_PathEdges.Capacity + m_SecondaryEdges.Capacity) * (System.Runtime.CompilerServices.Unsafe.SizeOf<Entity>() + sizeof(EdgeID)) + (int)(m_NodeIDs.Capacity + (uint)m_PathNodes.Capacity) * (sizeof(PathNode) + sizeof(NodeID))) + ((UnsafeHeapAllocator)(ref m_ConnectionAllocator)).Size * 4;
	}

	public int GetNodeIDSize()
	{
		return (int)((UnsafeHeapAllocator)(ref m_ConnectionAllocator)).OnePastHighestUsedAddress;
	}

	public EdgeID CreateEdge(PathNode startNode, PathNode middleNode, PathNode endNode, PathSpecification specification, LocationSpecification location)
	{
		EdgeID edgeID;
		if (m_FreeIDs.Length > 0)
		{
			int num = m_FreeIDs.Length - 1;
			edgeID = m_FreeIDs[num];
			m_FreeIDs.Length = num;
		}
		else
		{
			edgeID = new EdgeID
			{
				m_Index = m_Edges.Length
			};
			ref UnsafeList<Edge> edges = ref m_Edges;
			Edge edge = default(Edge);
			edges.Add(ref edge);
		}
		ref Edge reference = ref m_Edges.ElementAt(edgeID.m_Index);
		reference.m_StartID = new NodeID
		{
			m_Index = -1
		};
		reference.m_MiddleID = new NodeID
		{
			m_Index = -1
		};
		reference.m_EndID = new NodeID
		{
			m_Index = -1
		};
		int accessIndex = math.select(-1, specification.m_AccessRequirement, (specification.m_Flags & EdgeFlags.AllowEnter) == 0);
		if ((specification.m_Flags & EdgeFlags.Forward) != 0)
		{
			reference.m_StartID = AddConnection(startNode.StripCurvePos(), edgeID, accessIndex);
			reference.m_EndID = AddReversedConnection(endNode.StripCurvePos(), edgeID, accessIndex);
		}
		if ((specification.m_Flags & EdgeFlags.AllowMiddle) != 0)
		{
			reference.m_MiddleID = AddConnection(middleNode.StripCurvePos(), edgeID, accessIndex);
			reference.m_MiddleID = AddReversedConnection(middleNode.StripCurvePos(), edgeID, accessIndex);
		}
		if ((specification.m_Flags & EdgeFlags.Backward) != 0)
		{
			reference.m_EndID = AddConnection(endNode.StripCurvePos(), edgeID, accessIndex);
			reference.m_StartID = AddReversedConnection(startNode.StripCurvePos(), edgeID, accessIndex);
		}
		reference.m_StartCurvePos = startNode.GetCurvePos();
		reference.m_EndCurvePos = endNode.GetCurvePos();
		reference.m_Specification = specification;
		reference.m_Location = location;
		return edgeID;
	}

	public void UpdateEdge(EdgeID edgeID, PathNode startNode, PathNode middleNode, PathNode endNode, PathSpecification specification, LocationSpecification location)
	{
		ref Edge reference = ref m_Edges.ElementAt(edgeID.m_Index);
		EdgeFlags edgeFlags = reference.m_Specification.m_Flags & specification.m_Flags;
		EdgeFlags edgeFlags2 = (EdgeFlags)((uint)reference.m_Specification.m_Flags & (uint)(ushort)(~(int)specification.m_Flags));
		EdgeFlags edgeFlags3 = (EdgeFlags)((uint)(ushort)(~(int)reference.m_Specification.m_Flags) & (uint)specification.m_Flags);
		EdgeFlags edgeFlags4 = edgeFlags2;
		EdgeFlags edgeFlags5 = edgeFlags3;
		NodeID other = default(NodeID);
		if (!m_NodeIDs.TryGetValue(startNode.StripCurvePos(), ref other))
		{
			other = new NodeID
			{
				m_Index = -2
			};
		}
		NodeID other2 = default(NodeID);
		if (!m_NodeIDs.TryGetValue(middleNode.StripCurvePos(), ref other2))
		{
			other2 = new NodeID
			{
				m_Index = -2
			};
		}
		NodeID other3 = default(NodeID);
		if (!m_NodeIDs.TryGetValue(endNode.StripCurvePos(), ref other3))
		{
			other3 = new NodeID
			{
				m_Index = -2
			};
		}
		EdgeFlags edgeFlags6 = edgeFlags & EdgeFlags.Forward;
		EdgeFlags edgeFlags7 = edgeFlags & EdgeFlags.AllowMiddle;
		EdgeFlags edgeFlags8 = edgeFlags & EdgeFlags.Backward;
		if (!reference.m_StartID.Equals(other))
		{
			edgeFlags2 |= edgeFlags6;
			edgeFlags3 |= edgeFlags6;
			edgeFlags4 |= edgeFlags8;
			edgeFlags5 |= edgeFlags8;
		}
		if (!reference.m_MiddleID.Equals(other2))
		{
			edgeFlags2 |= edgeFlags7;
			edgeFlags3 |= edgeFlags7;
			edgeFlags4 |= edgeFlags7;
			edgeFlags5 |= edgeFlags7;
		}
		if (!reference.m_EndID.Equals(other3))
		{
			edgeFlags2 |= edgeFlags8;
			edgeFlags3 |= edgeFlags8;
			edgeFlags4 |= edgeFlags6;
			edgeFlags5 |= edgeFlags6;
		}
		int num = math.select(-1, specification.m_AccessRequirement, (specification.m_Flags & EdgeFlags.AllowEnter) == 0);
		int num2 = math.select(-1, reference.m_Specification.m_AccessRequirement, (reference.m_Specification.m_Flags & EdgeFlags.AllowEnter) == 0);
		Edge edge = reference;
		if ((edgeFlags2 & EdgeFlags.Forward) != 0)
		{
			RemoveConnection(edge.m_StartID, edgeID);
		}
		if ((edgeFlags2 & EdgeFlags.AllowMiddle) != 0)
		{
			RemoveConnection(edge.m_MiddleID, edgeID);
		}
		if ((edgeFlags2 & EdgeFlags.Backward) != 0)
		{
			RemoveConnection(edge.m_EndID, edgeID);
		}
		if ((edgeFlags3 & EdgeFlags.Forward) != 0)
		{
			reference.m_StartID = AddConnection(startNode.StripCurvePos(), edgeID, num);
		}
		if ((edgeFlags3 & EdgeFlags.AllowMiddle) != 0)
		{
			reference.m_MiddleID = AddConnection(middleNode.StripCurvePos(), edgeID, num);
		}
		if ((edgeFlags3 & EdgeFlags.Backward) != 0)
		{
			reference.m_EndID = AddConnection(endNode.StripCurvePos(), edgeID, num);
		}
		if ((edgeFlags4 & EdgeFlags.Forward) != 0)
		{
			RemoveReversedConnection(edge.m_EndID, edgeID);
		}
		if ((edgeFlags4 & EdgeFlags.AllowMiddle) != 0)
		{
			RemoveReversedConnection(edge.m_MiddleID, edgeID);
		}
		if ((edgeFlags4 & EdgeFlags.Backward) != 0)
		{
			RemoveReversedConnection(edge.m_StartID, edgeID);
		}
		if ((edgeFlags5 & EdgeFlags.Forward) != 0)
		{
			reference.m_EndID = AddReversedConnection(endNode.StripCurvePos(), edgeID, num);
		}
		if ((edgeFlags5 & EdgeFlags.AllowMiddle) != 0)
		{
			reference.m_MiddleID = AddReversedConnection(middleNode.StripCurvePos(), edgeID, num);
		}
		if ((edgeFlags5 & EdgeFlags.Backward) != 0)
		{
			reference.m_StartID = AddReversedConnection(startNode.StripCurvePos(), edgeID, num);
		}
		if (num != num2)
		{
			EdgeFlags edgeFlags9 = (EdgeFlags)((uint)edgeFlags & (uint)(ushort)(~(int)edgeFlags3));
			int num3 = (int)edgeFlags & (int)(ushort)(~(int)edgeFlags5);
			if ((edgeFlags9 & EdgeFlags.Forward) != 0)
			{
				UpdateAccessRequirement(reference.m_StartID, edgeID, num);
			}
			if ((edgeFlags9 & EdgeFlags.AllowMiddle) != 0)
			{
				UpdateAccessRequirement(reference.m_MiddleID, edgeID, num);
			}
			if ((edgeFlags9 & EdgeFlags.Backward) != 0)
			{
				UpdateAccessRequirement(reference.m_EndID, edgeID, num);
			}
			if ((num3 & 1) != 0)
			{
				UpdateReversedAccessRequirement(reference.m_EndID, edgeID, num);
			}
			if ((num3 & 4) != 0)
			{
				UpdateReversedAccessRequirement(reference.m_MiddleID, edgeID, num);
			}
			if ((num3 & 2) != 0)
			{
				UpdateReversedAccessRequirement(reference.m_StartID, edgeID, num);
			}
		}
		if ((specification.m_Flags & (EdgeFlags.Forward | EdgeFlags.Backward)) == 0)
		{
			reference.m_StartID = new NodeID
			{
				m_Index = -1
			};
			reference.m_EndID = new NodeID
			{
				m_Index = -1
			};
		}
		if ((specification.m_Flags & EdgeFlags.AllowMiddle) == 0)
		{
			reference.m_MiddleID = new NodeID
			{
				m_Index = -1
			};
		}
		reference.m_StartCurvePos = startNode.GetCurvePos();
		reference.m_EndCurvePos = endNode.GetCurvePos();
		reference.m_Specification = specification;
		reference.m_Location = location;
	}

	public void SetEdgeDirections(EdgeID edgeID, PathNode startNode, PathNode endNode, bool enableForward, bool enableBackward)
	{
		ref Edge reference = ref m_Edges.ElementAt(edgeID.m_Index);
		int accessIndex = math.select(-1, reference.m_Specification.m_AccessRequirement, (reference.m_Specification.m_Flags & EdgeFlags.AllowEnter) == 0);
		if (enableForward != ((reference.m_Specification.m_Flags & EdgeFlags.Forward) != 0))
		{
			if (enableForward)
			{
				reference.m_StartID = AddConnection(startNode.StripCurvePos(), edgeID, accessIndex);
				reference.m_EndID = AddReversedConnection(endNode.StripCurvePos(), edgeID, accessIndex);
				reference.m_Specification.m_Flags |= EdgeFlags.Forward;
			}
			else
			{
				RemoveConnection(reference.m_StartID, edgeID);
				RemoveReversedConnection(reference.m_EndID, edgeID);
				reference.m_Specification.m_Flags &= ~EdgeFlags.Forward;
			}
		}
		if (enableBackward != ((reference.m_Specification.m_Flags & EdgeFlags.Backward) != 0))
		{
			if (enableBackward)
			{
				reference.m_EndID = AddConnection(endNode.StripCurvePos(), edgeID, accessIndex);
				reference.m_StartID = AddReversedConnection(startNode.StripCurvePos(), edgeID, accessIndex);
				reference.m_Specification.m_Flags |= EdgeFlags.Backward;
			}
			else
			{
				RemoveConnection(reference.m_EndID, edgeID);
				RemoveReversedConnection(reference.m_StartID, edgeID);
				reference.m_Specification.m_Flags &= ~EdgeFlags.Backward;
			}
		}
		if ((reference.m_Specification.m_Flags & (EdgeFlags.Forward | EdgeFlags.Backward)) == 0)
		{
			reference.m_StartID = new NodeID
			{
				m_Index = -1
			};
			reference.m_EndID = new NodeID
			{
				m_Index = -1
			};
		}
	}

	public void DestroyEdge(EdgeID edgeID)
	{
		ref Edge reference = ref m_Edges.ElementAt(edgeID.m_Index);
		if ((reference.m_Specification.m_Flags & EdgeFlags.Forward) != 0)
		{
			RemoveConnection(reference.m_StartID, edgeID);
			RemoveReversedConnection(reference.m_EndID, edgeID);
		}
		if ((reference.m_Specification.m_Flags & EdgeFlags.AllowMiddle) != 0)
		{
			RemoveConnection(reference.m_MiddleID, edgeID);
			RemoveReversedConnection(reference.m_MiddleID, edgeID);
		}
		if ((reference.m_Specification.m_Flags & EdgeFlags.Backward) != 0)
		{
			RemoveConnection(reference.m_EndID, edgeID);
			RemoveReversedConnection(reference.m_StartID, edgeID);
		}
		if (edgeID.m_Index == m_Edges.Length - 1)
		{
			m_Edges.RemoveAt(edgeID.m_Index);
			return;
		}
		m_Edges[edgeID.m_Index] = default(Edge);
		m_FreeIDs.Add(ref edgeID);
	}

	public void AddEdge(Entity owner, EdgeID edgeID)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		m_Edges.ElementAt(edgeID.m_Index).m_Owner = owner;
		m_PathEdges.Add(owner, edgeID);
	}

	public void AddSecondaryEdge(Entity owner, EdgeID edgeID)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		m_Edges.ElementAt(edgeID.m_Index).m_Owner = owner;
		m_SecondaryEdges.Add(owner, edgeID);
	}

	public bool GetEdge(Entity owner, out EdgeID edgeID)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return m_PathEdges.TryGetValue(owner, ref edgeID);
	}

	public bool GetSecondaryEdge(Entity owner, out EdgeID edgeID)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return m_SecondaryEdges.TryGetValue(owner, ref edgeID);
	}

	public bool RemoveEdge(Entity owner, out EdgeID edgeID)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (m_PathEdges.TryGetValue(owner, ref edgeID))
		{
			m_PathEdges.Remove(owner);
			return true;
		}
		return false;
	}

	public bool RemoveSecondaryEdge(Entity owner, out EdgeID edgeID)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (m_SecondaryEdges.TryGetValue(owner, ref edgeID))
		{
			m_SecondaryEdges.Remove(owner);
			return true;
		}
		return false;
	}

	public unsafe void SwapConnections()
	{
		void* connections = m_Connections;
		m_Connections = m_ReversedConnections;
		m_ReversedConnections = connections;
	}

	private NodeID AddConnection(PathNode pathNode, EdgeID edgeID, int accessIndex)
	{
		NodeID nodeID = default(NodeID);
		if (m_NodeIDs.TryGetValue(pathNode, ref nodeID))
		{
			ref ushort connectionCount = ref GetConnectionCount(nodeID);
			ref ushort connectionCapacity = ref GetConnectionCapacity(nodeID);
			int num = (connectionCount << 1) + 2;
			if (num > connectionCapacity)
			{
				m_PathNodes.Remove(nodeID);
				ResizeConnections(ref nodeID, num);
				connectionCount = ref GetConnectionCount(nodeID);
				m_NodeIDs[pathNode] = nodeID;
				m_PathNodes.Add(nodeID, pathNode);
			}
			ref int connection = ref GetConnection(nodeID, connectionCount);
			ref int accessRequirement = ref GetAccessRequirement(nodeID, connectionCount);
			connection = edgeID.m_Index;
			accessRequirement = accessIndex;
			connectionCount++;
			return nodeID;
		}
		nodeID = CreateConnections(2);
		ref ushort connectionCount2 = ref GetConnectionCount(nodeID);
		ref int connection2 = ref GetConnection(nodeID, connectionCount2);
		ref int accessRequirement2 = ref GetAccessRequirement(nodeID, connectionCount2);
		connection2 = edgeID.m_Index;
		accessRequirement2 = accessIndex;
		connectionCount2++;
		m_NodeIDs.Add(pathNode, nodeID);
		m_PathNodes.Add(nodeID, pathNode);
		m_NodeCount++;
		return nodeID;
	}

	private NodeID AddReversedConnection(PathNode pathNode, EdgeID edgeID, int accessIndex)
	{
		NodeID nodeID = default(NodeID);
		if (m_NodeIDs.TryGetValue(pathNode, ref nodeID))
		{
			ref ushort reversedConnectionCount = ref GetReversedConnectionCount(nodeID);
			ref ushort reversedConnectionCapacity = ref GetReversedConnectionCapacity(nodeID);
			int num = (reversedConnectionCount << 1) + 2;
			if (num > reversedConnectionCapacity)
			{
				m_PathNodes.Remove(nodeID);
				ResizeConnections(ref nodeID, num);
				reversedConnectionCount = ref GetReversedConnectionCount(nodeID);
				m_NodeIDs[pathNode] = nodeID;
				m_PathNodes.Add(nodeID, pathNode);
			}
			ref int reversedConnection = ref GetReversedConnection(nodeID, reversedConnectionCount);
			ref int reversedAccessRequirement = ref GetReversedAccessRequirement(nodeID, reversedConnectionCount);
			reversedConnection = edgeID.m_Index;
			reversedAccessRequirement = accessIndex;
			reversedConnectionCount++;
			return nodeID;
		}
		nodeID = CreateConnections(2);
		ref ushort reversedConnectionCount2 = ref GetReversedConnectionCount(nodeID);
		ref int reversedConnection2 = ref GetReversedConnection(nodeID, reversedConnectionCount2);
		ref int reversedAccessRequirement2 = ref GetReversedAccessRequirement(nodeID, reversedConnectionCount2);
		reversedConnection2 = edgeID.m_Index;
		reversedAccessRequirement2 = accessIndex;
		reversedConnectionCount2++;
		m_NodeIDs.Add(pathNode, nodeID);
		m_PathNodes.Add(nodeID, pathNode);
		m_NodeCount++;
		return nodeID;
	}

	private void RemoveConnection(NodeID nodeID, EdgeID edgeID)
	{
		ref ushort connectionCount = ref GetConnectionCount(nodeID);
		for (int i = 0; i < connectionCount; i++)
		{
			ref int connection = ref GetConnection(nodeID, i);
			if (connection == edgeID.m_Index)
			{
				if (i != --connectionCount)
				{
					ref int accessRequirement = ref GetAccessRequirement(nodeID, i);
					connection = GetConnection(nodeID, connectionCount);
					accessRequirement = GetAccessRequirement(nodeID, connectionCount);
				}
				else if (connectionCount == 0 && GetReversedConnectionCount(nodeID) == 0)
				{
					m_NodeIDs.Remove(m_PathNodes[nodeID]);
					m_PathNodes.Remove(nodeID);
					DestroyConnections(nodeID);
					m_NodeCount--;
				}
				break;
			}
		}
	}

	private void RemoveReversedConnection(NodeID nodeID, EdgeID edgeID)
	{
		ref ushort reversedConnectionCount = ref GetReversedConnectionCount(nodeID);
		for (int i = 0; i < reversedConnectionCount; i++)
		{
			ref int reversedConnection = ref GetReversedConnection(nodeID, i);
			if (reversedConnection == edgeID.m_Index)
			{
				if (i != --reversedConnectionCount)
				{
					ref int reversedAccessRequirement = ref GetReversedAccessRequirement(nodeID, i);
					reversedConnection = GetReversedConnection(nodeID, reversedConnectionCount);
					reversedAccessRequirement = GetReversedAccessRequirement(nodeID, reversedConnectionCount);
				}
				else if (reversedConnectionCount == 0 && GetConnectionCount(nodeID) == 0)
				{
					m_NodeIDs.Remove(m_PathNodes[nodeID]);
					m_PathNodes.Remove(nodeID);
					DestroyConnections(nodeID);
					m_NodeCount--;
				}
				break;
			}
		}
	}

	private void UpdateAccessRequirement(NodeID nodeID, EdgeID edgeID, int accessIndex)
	{
		ref ushort connectionCount = ref GetConnectionCount(nodeID);
		for (int i = 0; i < connectionCount; i++)
		{
			if (GetConnection(nodeID, i) == edgeID.m_Index)
			{
				GetAccessRequirement(nodeID, i) = accessIndex;
				break;
			}
		}
	}

	private void UpdateReversedAccessRequirement(NodeID nodeID, EdgeID edgeID, int accessIndex)
	{
		ref ushort reversedConnectionCount = ref GetReversedConnectionCount(nodeID);
		for (int i = 0; i < reversedConnectionCount; i++)
		{
			if (GetReversedConnection(nodeID, i) == edgeID.m_Index)
			{
				GetReversedAccessRequirement(nodeID, i) = accessIndex;
				break;
			}
		}
	}

	private NodeID CreateConnections(int connectionCapacity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		UnsafeHeapBlock val = AllocateConnections(connectionCapacity);
		NodeID nodeID = new NodeID
		{
			m_Index = (int)val.begin
		};
		ref ushort connectionCount = ref GetConnectionCount(nodeID);
		ref ushort connectionCapacity2 = ref GetConnectionCapacity(nodeID);
		connectionCount = 0;
		connectionCapacity2 = (ushort)(val.end - val.begin - 1);
		connectionCount = ref GetReversedConnectionCount(nodeID);
		ref ushort reversedConnectionCapacity = ref GetReversedConnectionCapacity(nodeID);
		connectionCount = 0;
		reversedConnectionCapacity = (ushort)(val.end - val.begin - 1);
		return nodeID;
	}

	private void ResizeConnections(ref NodeID nodeID, int connectionCapacity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		UnsafeHeapBlock val = AllocateConnections(connectionCapacity);
		UnsafeHeapBlock val2 = default(UnsafeHeapBlock);
		((UnsafeHeapBlock)(ref val2))._002Ector((uint)nodeID.m_Index, (uint)(nodeID.m_Index + GetConnectionCapacity(nodeID) + 1));
		NodeID nodeID2 = new NodeID
		{
			m_Index = (int)val.begin
		};
		ref ushort connectionCount = ref GetConnectionCount(nodeID);
		ref ushort connectionCount2 = ref GetConnectionCount(nodeID2);
		ref ushort connectionCapacity2 = ref GetConnectionCapacity(nodeID2);
		connectionCount2 = connectionCount;
		connectionCapacity2 = (ushort)(val.end - val.begin - 1);
		for (int i = 0; i < connectionCount; i++)
		{
			ref int connection = ref GetConnection(nodeID, i);
			ref int accessRequirement = ref GetAccessRequirement(nodeID, i);
			ref int connection2 = ref GetConnection(nodeID2, i);
			ref int accessRequirement2 = ref GetAccessRequirement(nodeID2, i);
			connection2 = connection;
			accessRequirement2 = accessRequirement;
			ref Edge reference = ref m_Edges.ElementAt(connection);
			reference.m_StartID.m_Index = math.select(reference.m_StartID.m_Index, nodeID2.m_Index, reference.m_StartID.Equals(nodeID));
			reference.m_MiddleID.m_Index = math.select(reference.m_MiddleID.m_Index, nodeID2.m_Index, reference.m_MiddleID.Equals(nodeID));
			reference.m_EndID.m_Index = math.select(reference.m_EndID.m_Index, nodeID2.m_Index, reference.m_EndID.Equals(nodeID));
		}
		connectionCount = ref GetReversedConnectionCount(nodeID);
		connectionCount2 = ref GetReversedConnectionCount(nodeID2);
		ref ushort reversedConnectionCapacity = ref GetReversedConnectionCapacity(nodeID2);
		connectionCount2 = connectionCount;
		reversedConnectionCapacity = (ushort)(val.end - val.begin - 1);
		for (int j = 0; j < connectionCount; j++)
		{
			ref int reversedConnection = ref GetReversedConnection(nodeID, j);
			ref int reversedAccessRequirement = ref GetReversedAccessRequirement(nodeID, j);
			ref int reversedConnection2 = ref GetReversedConnection(nodeID2, j);
			ref int reversedAccessRequirement2 = ref GetReversedAccessRequirement(nodeID2, j);
			reversedConnection2 = reversedConnection;
			reversedAccessRequirement2 = reversedAccessRequirement;
			ref Edge reference2 = ref m_Edges.ElementAt(reversedConnection);
			reference2.m_StartID.m_Index = math.select(reference2.m_StartID.m_Index, nodeID2.m_Index, reference2.m_StartID.Equals(nodeID));
			reference2.m_MiddleID.m_Index = math.select(reference2.m_MiddleID.m_Index, nodeID2.m_Index, reference2.m_MiddleID.Equals(nodeID));
			reference2.m_EndID.m_Index = math.select(reference2.m_EndID.m_Index, nodeID2.m_Index, reference2.m_EndID.Equals(nodeID));
		}
		((UnsafeHeapAllocator)(ref m_ConnectionAllocator)).Release(val2);
		nodeID = nodeID2;
	}

	private void DestroyConnections(NodeID nodeID)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		ref ushort connectionCapacity = ref GetConnectionCapacity(nodeID);
		UnsafeHeapBlock val = default(UnsafeHeapBlock);
		((UnsafeHeapBlock)(ref val))._002Ector((uint)nodeID.m_Index, (uint)(nodeID.m_Index + connectionCapacity + 1));
		((UnsafeHeapAllocator)(ref m_ConnectionAllocator)).Release(val);
	}

	private unsafe UnsafeHeapBlock AllocateConnections(int connectionCapacity)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		uint num = (uint)(connectionCapacity + 1);
		UnsafeHeapBlock result = ((UnsafeHeapAllocator)(ref m_ConnectionAllocator)).Allocate(num, 1u);
		if (!((UnsafeHeapBlock)(ref result)).Empty)
		{
			return result;
		}
		((UnsafeHeapAllocator)(ref m_ConnectionAllocator)).Resize(math.max(((UnsafeHeapAllocator)(ref m_ConnectionAllocator)).Size * 3 / 2, ((UnsafeHeapAllocator)(ref m_ConnectionAllocator)).Size + num));
		void* ptr = UnsafeUtility.Malloc((long)(((UnsafeHeapAllocator)(ref m_ConnectionAllocator)).Size * 4), (int)(((UnsafeHeapAllocator)(ref m_ConnectionAllocator)).MinimumAlignment * 4), m_AllocatorLabel);
		void* ptr2 = UnsafeUtility.Malloc((long)(((UnsafeHeapAllocator)(ref m_ConnectionAllocator)).Size * 4), (int)(((UnsafeHeapAllocator)(ref m_ConnectionAllocator)).MinimumAlignment * 4), m_AllocatorLabel);
		uint num2 = ((UnsafeHeapAllocator)(ref m_ConnectionAllocator)).OnePastHighestUsedAddress * 4;
		if (num2 != 0)
		{
			UnsafeUtility.MemCpy(ptr, m_Connections, (long)num2);
			UnsafeUtility.MemCpy(ptr2, m_ReversedConnections, (long)num2);
		}
		UnsafeUtility.Free(m_Connections, m_AllocatorLabel);
		UnsafeUtility.Free(m_ReversedConnections, m_AllocatorLabel);
		m_Connections = ptr;
		m_ReversedConnections = ptr2;
		return ((UnsafeHeapAllocator)(ref m_ConnectionAllocator)).Allocate(num, 1u);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ref Edge GetEdge(EdgeID edgeID)
	{
		return ref m_Edges.ElementAt(edgeID.m_Index);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe ref ushort GetConnectionCount(NodeID nodeID)
	{
		return ref *(ushort*)((byte*)m_Connections + (nint)(nodeID.m_Index << 1) * (nint)2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe ref ushort GetReversedConnectionCount(NodeID nodeID)
	{
		return ref *(ushort*)((byte*)m_ReversedConnections + (nint)(nodeID.m_Index << 1) * (nint)2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe ref ushort GetConnectionCapacity(NodeID nodeID)
	{
		return ref *(ushort*)((byte*)m_Connections + (nint)((nodeID.m_Index << 1) + 1) * (nint)2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe ref ushort GetReversedConnectionCapacity(NodeID nodeID)
	{
		return ref *(ushort*)((byte*)m_ReversedConnections + (nint)((nodeID.m_Index << 1) + 1) * (nint)2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe ref int GetConnection(NodeID nodeID, int connectionIndex)
	{
		return ref *(int*)((byte*)m_Connections + (nint)(nodeID.m_Index + (connectionIndex << 1) + 1) * (nint)4);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe ref int GetReversedConnection(NodeID nodeID, int connectionIndex)
	{
		return ref *(int*)((byte*)m_ReversedConnections + (nint)(nodeID.m_Index + (connectionIndex << 1) + 1) * (nint)4);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe ref int GetAccessRequirement(NodeID nodeID, int connectionIndex)
	{
		return ref *(int*)((byte*)m_Connections + (nint)(nodeID.m_Index + (connectionIndex << 1) + 2) * (nint)4);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe ref int GetReversedAccessRequirement(NodeID nodeID, int connectionIndex)
	{
		return ref *(int*)((byte*)m_ReversedConnections + (nint)(nodeID.m_Index + (connectionIndex << 1) + 2) * (nint)4);
	}
}
