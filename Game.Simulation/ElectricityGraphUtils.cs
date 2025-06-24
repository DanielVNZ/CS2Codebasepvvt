using System;
using Game.Common;
using Game.Net;
using Unity.Assertions;
using Unity.Collections;
using Unity.Entities;

namespace Game.Simulation;

public static class ElectricityGraphUtils
{
	public static bool HasAnyFlowEdge(Entity node1, Entity node2, ref BufferLookup<ConnectedFlowEdge> flowConnections, ref ComponentLookup<ElectricityFlowEdge> flowEdges)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		Assert.IsTrue(node1.Index > 0);
		Assert.IsTrue(node2.Index > 0);
		Enumerator<ConnectedFlowEdge> enumerator = flowConnections[node1].GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				ElectricityFlowEdge electricityFlowEdge = flowEdges[enumerator.Current.m_Edge];
				if ((electricityFlowEdge.m_Start == node1 && electricityFlowEdge.m_End == node2) || (electricityFlowEdge.m_Start == node2 && electricityFlowEdge.m_End == node1))
				{
					return true;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		return false;
	}

	public static bool TryGetFlowEdge(Entity startNode, Entity endNode, ref BufferLookup<ConnectedFlowEdge> flowConnections, ref ComponentLookup<ElectricityFlowEdge> flowEdges, out Entity entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		ElectricityFlowEdge edge;
		return TryGetFlowEdge(startNode, endNode, ref flowConnections, ref flowEdges, out entity, out edge);
	}

	public static bool TryGetFlowEdge(Entity startNode, Entity endNode, ref BufferLookup<ConnectedFlowEdge> flowConnections, ref ComponentLookup<ElectricityFlowEdge> flowEdges, out ElectricityFlowEdge edge)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Entity entity;
		return TryGetFlowEdge(startNode, endNode, ref flowConnections, ref flowEdges, out entity, out edge);
	}

	public static bool TryGetFlowEdge(Entity startNode, Entity endNode, ref BufferLookup<ConnectedFlowEdge> flowConnections, ref ComponentLookup<ElectricityFlowEdge> flowEdges, out Entity entity, out ElectricityFlowEdge edge)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		Assert.IsTrue(startNode.Index > 0);
		Assert.IsTrue(endNode.Index > 0);
		Enumerator<ConnectedFlowEdge> enumerator = flowConnections[startNode].GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				entity = enumerator.Current.m_Edge;
				edge = flowEdges[entity];
				if (edge.m_Start == startNode && edge.m_End == endNode)
				{
					return true;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		entity = default(Entity);
		edge = default(ElectricityFlowEdge);
		return false;
	}

	public static bool TrySetFlowEdge(Entity startNode, Entity endNode, FlowDirection direction, int capacity, ref BufferLookup<ConnectedFlowEdge> flowConnections, ref ComponentLookup<ElectricityFlowEdge> flowEdges)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetFlowEdge(startNode, endNode, ref flowConnections, ref flowEdges, out var entity, out var edge))
		{
			edge.direction = direction;
			edge.m_Capacity = capacity;
			flowEdges[entity] = edge;
			return true;
		}
		return false;
	}

	public static Entity CreateFlowEdge(EntityCommandBuffer commandBuffer, EntityArchetype edgeArchetype, Entity startNode, Entity endNode, FlowDirection direction, int capacity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		Assert.AreNotEqual<Entity>(startNode, Entity.Null);
		Assert.AreNotEqual<Entity>(endNode, Entity.Null);
		Entity val = ((EntityCommandBuffer)(ref commandBuffer)).CreateEntity(edgeArchetype);
		((EntityCommandBuffer)(ref commandBuffer)).SetComponent<ElectricityFlowEdge>(val, new ElectricityFlowEdge
		{
			m_Start = startNode,
			m_End = endNode,
			direction = direction,
			m_Capacity = capacity
		});
		((EntityCommandBuffer)(ref commandBuffer)).AppendToBuffer<ConnectedFlowEdge>(startNode, new ConnectedFlowEdge(val));
		((EntityCommandBuffer)(ref commandBuffer)).AppendToBuffer<ConnectedFlowEdge>(endNode, new ConnectedFlowEdge(val));
		return val;
	}

	public static Entity CreateFlowEdge(ParallelWriter commandBuffer, int jobIndex, EntityArchetype edgeArchetype, Entity startNode, Entity endNode, FlowDirection direction, int capacity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		Assert.AreNotEqual<Entity>(startNode, Entity.Null);
		Assert.AreNotEqual<Entity>(endNode, Entity.Null);
		Entity val = ((ParallelWriter)(ref commandBuffer)).CreateEntity(jobIndex, edgeArchetype);
		((ParallelWriter)(ref commandBuffer)).SetComponent<ElectricityFlowEdge>(jobIndex, val, new ElectricityFlowEdge
		{
			m_Start = startNode,
			m_End = endNode,
			direction = direction,
			m_Capacity = capacity
		});
		((ParallelWriter)(ref commandBuffer)).AppendToBuffer<ConnectedFlowEdge>(jobIndex, startNode, new ConnectedFlowEdge(val));
		((ParallelWriter)(ref commandBuffer)).AppendToBuffer<ConnectedFlowEdge>(jobIndex, endNode, new ConnectedFlowEdge(val));
		return val;
	}

	public static Entity CreateFlowEdge(EntityManager entityManager, EntityArchetype edgeArchetype, Entity startNode, Entity endNode, FlowDirection direction, int capacity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		Assert.AreNotEqual<Entity>(startNode, Entity.Null);
		Assert.AreNotEqual<Entity>(endNode, Entity.Null);
		Entity val = ((EntityManager)(ref entityManager)).CreateEntity(edgeArchetype);
		((EntityManager)(ref entityManager)).SetComponentData<ElectricityFlowEdge>(val, new ElectricityFlowEdge
		{
			m_Start = startNode,
			m_End = endNode,
			direction = direction,
			m_Capacity = capacity
		});
		((EntityManager)(ref entityManager)).GetBuffer<ConnectedFlowEdge>(startNode, false).Add(new ConnectedFlowEdge(val));
		((EntityManager)(ref entityManager)).GetBuffer<ConnectedFlowEdge>(endNode, false).Add(new ConnectedFlowEdge(val));
		return val;
	}

	public static void DeleteFlowNode(ParallelWriter commandBuffer, int jobIndex, Entity node, ref BufferLookup<ConnectedFlowEdge> flowConnections)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		((ParallelWriter)(ref commandBuffer)).AddComponent<Deleted>(jobIndex, node);
		Enumerator<ConnectedFlowEdge> enumerator = flowConnections[node].GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				((ParallelWriter)(ref commandBuffer)).AddComponent<Deleted>(jobIndex, enumerator.Current.m_Edge);
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
	}

	public static void DeleteFlowNode(EntityManager entityManager, Entity node)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).AddComponent<Deleted>(node);
		Enumerator<ConnectedFlowEdge> enumerator = ((EntityManager)(ref entityManager)).GetBuffer<ConnectedFlowEdge>(node, true).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				((EntityManager)(ref entityManager)).AddComponent<Deleted>(enumerator.Current.m_Edge);
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
	}

	public static void DeleteBuildingNodes(ParallelWriter commandBuffer, int jobIndex, ElectricityBuildingConnection connection, ref BufferLookup<ConnectedFlowEdge> flowConnections, ref ComponentLookup<ElectricityFlowEdge> flowEdges)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		if (connection.m_TransformerNode != Entity.Null)
		{
			DeleteFlowNode(commandBuffer, jobIndex, connection.m_TransformerNode, ref flowConnections);
		}
		if (connection.m_ProducerEdge != Entity.Null)
		{
			DeleteFlowNode(commandBuffer, jobIndex, connection.GetProducerNode(ref flowEdges), ref flowConnections);
		}
		if (connection.m_ConsumerEdge != Entity.Null)
		{
			DeleteFlowNode(commandBuffer, jobIndex, connection.GetConsumerNode(ref flowEdges), ref flowConnections);
		}
		if (connection.m_ChargeEdge != Entity.Null)
		{
			DeleteFlowNode(commandBuffer, jobIndex, connection.GetChargeNode(ref flowEdges), ref flowConnections);
		}
		if (connection.m_DischargeEdge != Entity.Null)
		{
			DeleteFlowNode(commandBuffer, jobIndex, connection.GetDischargeNode(ref flowEdges), ref flowConnections);
		}
	}
}
