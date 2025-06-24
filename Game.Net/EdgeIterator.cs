using Colossal.Collections;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;

namespace Game.Net;

public struct EdgeIterator
{
	private BufferLookup<ConnectedEdge> m_Edges;

	private ComponentLookup<Edge> m_EdgeData;

	private ComponentLookup<Temp> m_TempData;

	private ComponentLookup<Hidden> m_HiddenData;

	private DynamicBuffer<ConnectedEdge> m_Buffer;

	private int m_Iterator;

	private Entity m_Node;

	private Entity m_Edge;

	private Entity m_OriginalEdge;

	private bool m_Permanent;

	private bool m_Delete;

	private bool m_Middles;

	public EdgeIterator(Entity edge, Entity node, BufferLookup<ConnectedEdge> edges, ComponentLookup<Edge> edgeData, ComponentLookup<Temp> tempData, ComponentLookup<Hidden> hiddenData, bool includeMiddleConnections = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		m_Node = node;
		m_Edge = edge;
		m_OriginalEdge = Entity.Null;
		m_Edges = edges;
		m_EdgeData = edgeData;
		m_TempData = tempData;
		m_HiddenData = hiddenData;
		m_Buffer = m_Edges[node];
		m_Iterator = 0;
		m_Permanent = !m_TempData.HasComponent(node);
		m_Delete = false;
		m_Middles = includeMiddleConnections;
		if (edge != Entity.Null)
		{
			m_Delete = GetDelete(edge, out m_OriginalEdge);
		}
		else if (!m_Permanent)
		{
			m_Delete = GetDelete(node);
		}
	}

	public int GetMaxCount()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		int num = m_Buffer.Length;
		DynamicBuffer<ConnectedEdge> val = default(DynamicBuffer<ConnectedEdge>);
		if (!m_Permanent && m_Edges.TryGetBuffer(m_TempData[m_Node].m_Original, ref val))
		{
			num += val.Length;
		}
		return num;
	}

	public void AddSorted(ref ComponentLookup<BuildOrder> buildOrderData, ref StackList<EdgeIteratorValueSorted> list)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		EdgeIteratorValue value;
		BuildOrder buildOrder = default(BuildOrder);
		while (GetNext(out value))
		{
			buildOrderData.TryGetComponent(value.m_Edge, ref buildOrder);
			list.AddNoResize(new EdgeIteratorValueSorted
			{
				m_Edge = value.m_Edge,
				m_SortIndex = (uint)((ulong)((long)buildOrder.m_Start + (long)buildOrder.m_End) >> 1),
				m_End = value.m_End,
				m_Middle = value.m_Middle
			});
		}
		NativeSortExtension.Sort<EdgeIteratorValueSorted>(list.AsArray());
	}

	private bool GetDelete(Entity entity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Temp temp = default(Temp);
		if (m_TempData.TryGetComponent(entity, ref temp))
		{
			return (temp.m_Flags & TempFlags.Delete) != 0;
		}
		return false;
	}

	private bool GetDelete(Entity entity, out Entity original)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Temp temp = default(Temp);
		if (m_TempData.TryGetComponent(entity, ref temp))
		{
			original = temp.m_Original;
			return (temp.m_Flags & TempFlags.Delete) != 0;
		}
		original = Entity.Null;
		return false;
	}

	public bool GetNext(out EdgeIteratorValue value)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		Temp temp = default(Temp);
		while (true)
		{
			bool flag = m_Buffer.Length > m_Iterator;
			if (flag)
			{
				value.m_Edge = m_Buffer[m_Iterator++].m_Edge;
			}
			else
			{
				value.m_Edge = Entity.Null;
			}
			while (flag)
			{
				if (m_Permanent)
				{
					Edge edge = m_EdgeData[value.m_Edge];
					value.m_End = edge.m_End == m_Node;
					if (value.m_End || edge.m_Start == m_Node)
					{
						value.m_Middle = false;
						return true;
					}
					if (m_Middles)
					{
						value.m_Middle = true;
						return true;
					}
				}
				else if (m_Delete)
				{
					if (value.m_Edge == m_Edge || (m_HiddenData.HasComponent(value.m_Edge) && value.m_Edge != m_OriginalEdge))
					{
						Edge edge2 = m_EdgeData[value.m_Edge];
						value.m_End = edge2.m_End == m_Node;
						if (value.m_End || edge2.m_Start == m_Node)
						{
							value.m_Middle = false;
							return true;
						}
						if (m_Middles)
						{
							value.m_Middle = true;
							return true;
						}
					}
				}
				else if (!m_HiddenData.HasComponent(value.m_Edge) && !GetDelete(value.m_Edge))
				{
					Edge edge3 = m_EdgeData[value.m_Edge];
					value.m_End = edge3.m_End == m_Node;
					if (value.m_End || edge3.m_Start == m_Node)
					{
						value.m_Middle = false;
						return true;
					}
					if (m_Middles)
					{
						value.m_Middle = true;
						return true;
					}
				}
				flag = m_Buffer.Length > m_Iterator;
				if (flag)
				{
					value.m_Edge = m_Buffer[m_Iterator++].m_Edge;
				}
				else
				{
					value.m_Edge = Entity.Null;
				}
			}
			if (!m_TempData.TryGetComponent(m_Node, ref temp))
			{
				break;
			}
			m_Node = temp.m_Original;
			if (!m_Edges.TryGetBuffer(m_Node, ref m_Buffer))
			{
				break;
			}
			m_Iterator = 0;
		}
		value.m_Edge = Entity.Null;
		value.m_End = false;
		value.m_Middle = false;
		return false;
	}
}
