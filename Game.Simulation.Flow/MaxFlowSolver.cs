using Colossal.Collections;
using Unity.Assertions;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Simulation.Flow;

public struct MaxFlowSolver
{
	public const int kMaxNodes = 16777216;

	public int m_LayerHeight;

	public int m_SourceNode;

	public int m_SinkNode;

	public NativeArray<Node> m_Nodes;

	public NativeArray<Edge> m_Edges;

	public NativeArray<Connection> m_Connections;

	public NativeList<Layer> m_Layers;

	public NativeQueue<int> m_LabelQueue;

	public NativeArray<UnsafeList<int>> m_ActiveQueue;

	public bool m_Complete;

	public int m_CurrentLayerIndex;

	public int m_NextLayerIndex;

	public int m_CurrentLabelVersion;

	public int m_CurrentActiveVersion;

	public int m_StepCounter;

	public void InitializeState()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		m_Complete = false;
		m_NextLayerIndex = 0;
		m_CurrentLabelVersion = 1;
		m_CurrentActiveVersion = 1;
		Node node = GetNode(m_SourceNode);
		Layer layer = new Layer(node.connectionCount, (Allocator)2);
		for (int i = node.m_FirstConnection; i < node.m_LastConnection; i++)
		{
			Connection connection = GetConnection(i);
			if (connection.GetOutgoingResidualCapacity(m_Edges) > 0)
			{
				CutElement element = new CutElement
				{
					m_Flags = (CutElementFlags.Created | CutElementFlags.Admissible | CutElementFlags.Changed),
					m_StartNode = connection.m_StartNode,
					m_EndNode = connection.m_EndNode,
					m_Edge = connection.m_Edge,
					m_Version = m_CurrentLabelVersion,
					m_LinkedElements = -1,
					m_NextIndex = -1
				};
				int num = layer.AddCutElement(in element);
				layer.GetCutElement(num).m_Group = num;
				GetEdge(connection.m_Edge).m_CutElementId = new Identifier(num, m_CurrentLabelVersion);
			}
		}
		m_Layers.Clear();
		m_Layers.Add(ref layer);
	}

	public void LoadState(NativeReference<MaxFlowSolverState> solverState, NativeList<LayerState> layerStates, NativeList<CutElement> layerElements, NativeList<CutElementRef> layerElementRefs)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		MaxFlowSolverState value = solverState.Value;
		m_Complete = value.m_Complete;
		m_NextLayerIndex = value.m_NextLayerIndex;
		m_CurrentLabelVersion = value.m_CurrentLabelVersion;
		m_CurrentActiveVersion = value.m_CurrentActiveVersion;
		int elementIndex = 0;
		int elementRefIndex = 0;
		for (int i = 0; i < layerStates.Length; i++)
		{
			ref NativeList<Layer> layers = ref m_Layers;
			Layer layer = Layer.Load(layerStates[i], layerElements.AsArray(), ref elementIndex, layerElementRefs.AsArray(), ref elementRefIndex);
			layers.Add(ref layer);
		}
	}

	public void SaveState(NativeReference<MaxFlowSolverState> solverState, NativeList<LayerState> layerStates, NativeList<CutElement> layerElements, NativeList<CutElementRef> layerElementRefs)
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		solverState.Value = new MaxFlowSolverState
		{
			m_Complete = m_Complete,
			m_NextLayerIndex = m_NextLayerIndex,
			m_CurrentLabelVersion = m_CurrentLabelVersion,
			m_CurrentActiveVersion = m_CurrentActiveVersion
		};
		layerStates.Length = 0;
		layerElements.Length = 0;
		layerElementRefs.Length = 0;
		if (!m_Complete)
		{
			for (int i = 0; i < m_Layers.Length; i++)
			{
				m_Layers[i].Save(layerStates, layerElements, layerElementRefs);
			}
		}
	}

	public void ResetNetwork()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		ResetNetwork(m_Nodes, m_Edges, m_SourceNode);
	}

	public static void ResetNetwork(NativeArray<Node> nodes, NativeArray<Edge> edges, int sourceNode)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < nodes.Length; i++)
		{
			ref Node reference = ref CollectionUtils.ElementAt<Node>(nodes, i);
			reference.m_Height = 16777216;
			reference.m_Excess = 0;
			reference.m_CutElementId = default(Identifier);
			reference.m_Version = 0;
			reference.m_Retreat = false;
		}
		for (int j = 0; j < edges.Length; j++)
		{
			ref Edge reference2 = ref CollectionUtils.ElementAt<Edge>(edges, j);
			reference2.m_CutElementId = default(Identifier);
			reference2.FinalizeTempFlow();
		}
		CollectionUtils.ElementAt<Node>(nodes, sourceNode).m_Height = 0;
	}

	public void Solve()
	{
		while (!m_Complete)
		{
			SolveNextLayer();
		}
	}

	public void SolveNextLayer()
	{
		Layer layer = GetLayer(m_NextLayerIndex);
		if (layer.isEmpty)
		{
			m_Complete = true;
			return;
		}
		m_CurrentLayerIndex = m_NextLayerIndex;
		m_NextLayerIndex = m_CurrentLayerIndex + 1;
		m_CurrentActiveVersion++;
		if (m_Layers.Length <= m_NextLayerIndex)
		{
			int initialLength = 2 * math.max(layer.usedElementCount, layer.usedElementRefCount);
			ref NativeList<Layer> layers = ref m_Layers;
			Layer layer2 = new Layer(initialLength, (Allocator)2);
			layers.Add(ref layer2);
		}
		if (LabelPreflow())
		{
			int num = AdvanceToSource();
			if (num <= m_CurrentLayerIndex)
			{
				RetreatFromLayer(num);
			}
		}
	}

	private bool LabelPreflow()
	{
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		ref Layer layer = ref GetLayer(m_CurrentLayerIndex);
		int num = m_CurrentLayerIndex + 1;
		Layer layer2 = GetLayer(num);
		int lowerCutHeight = GetLowerCutHeight(m_CurrentLayerIndex);
		int upperCutHeight = GetUpperCutHeight(m_CurrentLayerIndex);
		for (int i = 0; i < layer.m_Elements.Length; i++)
		{
			CutElement cutElement = layer.GetCutElement(i);
			if (!cutElement.isCreated || (!cutElement.isChanged && !cutElement.isDeleted))
			{
				continue;
			}
			int num2 = cutElement.m_Group;
			do
			{
				ref CutElement cutElement2 = ref layer.GetCutElement(num2);
				int nextIndex = cutElement2.m_NextIndex;
				DeleteLinkedElements(m_CurrentLayerIndex, num2);
				if (cutElement2.isDeleted)
				{
					layer.FreeCutElement(num2);
				}
				else if (!cutElement2.isAdmissible)
				{
					Node node = GetNode(cutElement2.m_StartNode);
					int layerIndexForHeight = GetLayerIndexForHeight(node.m_Height);
					Assert.IsTrue(layerIndexForHeight < m_CurrentLayerIndex);
					ref Layer layer3 = ref GetLayer(layerIndexForHeight);
					layer.FreeCutElement(num2);
					layer3.RemoveElementLink(node.m_CutElementId.m_Index, m_CurrentLayerIndex, num2);
				}
				else
				{
					Identifier cutElementId = new Identifier(num2, m_CurrentLabelVersion);
					cutElement2.m_Group = num2;
					cutElement2.m_Version = m_CurrentLabelVersion;
					GetEdge(cutElement2.m_Edge).m_CutElementId = cutElementId;
					cutElement2.isChanged = false;
					cutElement2.m_NextIndex = -1;
					ref Node node2 = ref GetNode(cutElement2.m_EndNode);
					if (node2.m_CutElementId.m_Version != m_CurrentLabelVersion)
					{
						node2.m_Height = lowerCutHeight;
						node2.m_CutElementId = cutElementId;
						m_LabelQueue.Enqueue(cutElement2.m_EndNode);
					}
					else
					{
						layer.MergeGroups(node2.m_CutElementId.m_Index, num2);
					}
				}
				num2 = nextIndex;
			}
			while (num2 != -1);
		}
		int index = default(int);
		while (m_LabelQueue.TryDequeue(ref index))
		{
			m_StepCounter++;
			ref Node node3 = ref GetNode(index);
			for (int j = node3.m_FirstConnection; j < node3.m_LastConnection; j++)
			{
				Connection connection = GetConnection(j);
				ref Edge edge = ref GetEdge(connection.m_Edge);
				int endNode = connection.m_EndNode;
				edge.FinalizeTempFlow();
				if (endNode != m_SinkNode)
				{
					if (endNode == m_SourceNode)
					{
						continue;
					}
					ref Node node4 = ref GetNode(endNode);
					int nodeValidLayerIndex = GetNodeValidLayerIndex(node4);
					if (nodeValidLayerIndex == -1)
					{
						if (connection.GetOutgoingResidualCapacity(m_Edges) > 0)
						{
							if (node3.m_Height != upperCutHeight)
							{
								edge.m_CutElementId = node3.m_CutElementId;
								node4.m_Height = node3.m_Height + 1;
								node4.m_CutElementId = node3.m_CutElementId;
								node4.m_Retreat = false;
								m_LabelQueue.Enqueue(endNode);
							}
							else
							{
								node4.m_Height = node3.m_Height + 1;
								node4.m_CutElementId = default(Identifier);
								if (layer2.ContainsCutElementForConnection(edge.m_CutElementId, connection, admissible: true))
								{
									BumpAdmissibleLayerCutElement(m_CurrentLayerIndex, num, in connection);
								}
								else
								{
									AddAdmissibleLayerCutElement(m_CurrentLayerIndex, num, in connection);
								}
							}
						}
						else
						{
							node4.m_Height = 16777216;
							node4.m_CutElementId = default(Identifier);
						}
					}
					else if (nodeValidLayerIndex == m_CurrentLayerIndex)
					{
						if (!node4.m_CutElementId.Equals(node3.m_CutElementId))
						{
							layer.MergeGroups(node4.m_CutElementId.m_Index, node3.m_CutElementId.m_Index);
						}
					}
					else if (nodeValidLayerIndex < m_CurrentLayerIndex)
					{
						if (connection.GetOutgoingResidualCapacity(m_Edges) > 0 && connection.GetIncomingResidualCapacity(m_Edges) == 0)
						{
							AddInadmissibleLayerCutElement(nodeValidLayerIndex, m_CurrentLayerIndex, connection.Reverse());
						}
					}
					else if (node3.m_Height != upperCutHeight)
					{
						if (connection.GetIncomingResidualCapacity(m_Edges) > 0)
						{
							BumpInadmissibleLayerCutElement(m_CurrentLayerIndex, nodeValidLayerIndex, in connection);
						}
					}
					else if (connection.GetOutgoingResidualCapacity(m_Edges) > 0)
					{
						BumpAdmissibleLayerCutElement(m_CurrentLayerIndex, nodeValidLayerIndex, in connection);
					}
					else if (connection.GetIncomingResidualCapacity(m_Edges) > 0)
					{
						BumpInadmissibleLayerCutElement(m_CurrentLayerIndex, nodeValidLayerIndex, in connection);
					}
				}
				else
				{
					int outgoingResidualCapacity = connection.GetOutgoingResidualCapacity(m_Edges);
					if (outgoingResidualCapacity > 0)
					{
						AugmentOutgoingTempFlow(in connection, outgoingResidualCapacity);
						node3.m_Version = m_CurrentActiveVersion;
						edge.m_CutElementId = node3.m_CutElementId;
						CollectionUtils.ElementAt<UnsafeList<int>>(m_ActiveQueue, node3.m_Height - lowerCutHeight).Add(ref index);
						result = true;
					}
				}
			}
		}
		return result;
	}

	private void DeleteLinkedElements(int lowerLayerIndex, int index)
	{
		ref Layer layer = ref GetLayer(lowerLayerIndex);
		ref CutElement cutElement = ref layer.GetCutElement(index);
		int num = cutElement.m_LinkedElements;
		cutElement.m_LinkedElements = -1;
		while (num != -1)
		{
			CutElementRef cutElementRef = layer.GetCutElementRef(num);
			Layer layer2 = GetLayer(cutElementRef.m_Layer);
			layer2.GetCutElement(cutElementRef.m_Index).isDeleted = true;
			int nextIndex = cutElementRef.m_NextIndex;
			layer.FreeCutElementRef(num);
			num = nextIndex;
		}
	}

	private int GetNodeValidLayerIndex(Node node)
	{
		if (node.m_Height != 16777216 && node.m_CutElementId.m_Version != 0)
		{
			int layerIndexForHeight = GetLayerIndexForHeight(node.m_Height);
			Layer layer = GetLayer(layerIndexForHeight);
			if (layer.ContainsCutElement(node.m_CutElementId))
			{
				return layerIndexForHeight;
			}
		}
		return -1;
	}

	private void AddAdmissibleLayerCutElement(int lowerLayerIndex, int higherLayerIndex, in Connection lhConnection)
	{
		ref Layer layer = ref GetLayer(higherLayerIndex);
		CutElement element = new CutElement
		{
			m_Flags = (CutElementFlags.Created | CutElementFlags.Admissible | CutElementFlags.Changed),
			m_StartNode = lhConnection.m_StartNode,
			m_EndNode = lhConnection.m_EndNode,
			m_Edge = lhConnection.m_Edge,
			m_Version = m_CurrentLabelVersion,
			m_LinkedElements = -1,
			m_NextIndex = -1
		};
		int num = layer.AddCutElement(in element);
		layer.GetCutElement(num).m_Group = num;
		GetEdge(lhConnection.m_Edge).m_CutElementId = new Identifier(num, m_CurrentLabelVersion);
		int index = GetNode(lhConnection.m_StartNode).m_CutElementId.m_Index;
		CreateElementLink(lowerLayerIndex, index, higherLayerIndex, num);
	}

	private void BumpAdmissibleLayerCutElement(int lowerLayerIndex, int higherLayerIndex, in Connection lhConnection)
	{
		Layer layer = GetLayer(higherLayerIndex);
		int index = GetEdge(lhConnection.m_Edge).m_CutElementId.m_Index;
		layer.GetCutElement(index).isDeleted = false;
		int index2 = GetNode(lhConnection.m_StartNode).m_CutElementId.m_Index;
		CreateElementLink(lowerLayerIndex, index2, higherLayerIndex, index);
	}

	private void AddInadmissibleLayerCutElement(int lowerLayerIndex, int higherLayerIndex, in Connection lhConnection)
	{
		ref Layer layer = ref GetLayer(higherLayerIndex);
		int index = GetNode(lhConnection.m_EndNode).m_CutElementId.m_Index;
		ref CutElement cutElement = ref layer.GetCutElement(index);
		CutElement element = new CutElement
		{
			m_Flags = CutElementFlags.Created,
			m_StartNode = lhConnection.m_StartNode,
			m_EndNode = lhConnection.m_EndNode,
			m_Edge = lhConnection.m_Edge,
			m_Group = cutElement.m_Group,
			m_Version = m_CurrentLabelVersion,
			m_LinkedElements = -1,
			m_NextIndex = cutElement.m_NextIndex
		};
		int num = layer.AddCutElement(in element);
		layer.GetCutElement(index).m_NextIndex = num;
		GetEdge(lhConnection.m_Edge).m_CutElementId = new Identifier(num, m_CurrentLabelVersion);
		int index2 = GetNode(lhConnection.m_StartNode).m_CutElementId.m_Index;
		CreateElementLink(lowerLayerIndex, index2, higherLayerIndex, num);
	}

	private void BumpInadmissibleLayerCutElement(int lowerLayerIndex, int higherLayerIndex, in Connection lhConnection)
	{
		Layer layer = GetLayer(higherLayerIndex);
		int index = GetEdge(lhConnection.m_Edge).m_CutElementId.m_Index;
		ref CutElement cutElement = ref layer.GetCutElement(index);
		cutElement.isDeleted = false;
		if (cutElement.isAdmissible)
		{
			cutElement.isAdmissible = false;
			cutElement.isChanged = true;
		}
		int index2 = GetNode(lhConnection.m_StartNode).m_CutElementId.m_Index;
		CreateElementLink(lowerLayerIndex, index2, higherLayerIndex, index);
	}

	private void CreateElementLink(int lowerLayerIndex, int lowerElementIndex, int higherLayerIndex, int higherElementIndex)
	{
		ref Layer layer = ref GetLayer(lowerLayerIndex);
		ref CutElement cutElement = ref layer.GetCutElement(lowerElementIndex);
		CutElementRef elementRef = new CutElementRef
		{
			m_Layer = higherLayerIndex,
			m_Index = higherElementIndex,
			m_NextIndex = cutElement.m_LinkedElements
		};
		int linkedElements = layer.AddCutElementRef(in elementRef);
		cutElement.m_LinkedElements = linkedElements;
	}

	private int AdvanceToSource()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		int retreatLayerIndex = m_CurrentLayerIndex + 1;
		for (int num = m_CurrentLayerIndex; num >= 0; num--)
		{
			AdvanceActiveLayer(num, ref retreatLayerIndex);
		}
		ref UnsafeList<int> reference = ref CollectionUtils.ElementAt<UnsafeList<int>>(m_ActiveQueue, m_LayerHeight - 1);
		Assert.AreEqual(1, reference.Length);
		Assert.AreEqual(m_SourceNode, reference[0]);
		reference.Clear();
		ref Node node = ref GetNode(m_SourceNode);
		node.m_Retreat = false;
		node.m_Version = m_CurrentActiveVersion;
		if (m_NextLayerIndex <= m_CurrentLayerIndex)
		{
			m_CurrentLabelVersion++;
		}
		return retreatLayerIndex;
	}

	private void AdvanceActiveLayer(int activeLayerIndex, ref int retreatLayerIndex)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		bool flag = activeLayerIndex == m_CurrentLayerIndex;
		for (int num = m_LayerHeight - 1; num >= 0; num--)
		{
			bool flag2 = num == 0;
			ref UnsafeList<int> reference = ref CollectionUtils.ElementAt<UnsafeList<int>>(m_ActiveQueue, num);
			for (int i = 0; i < reference.Length; i++)
			{
				m_StepCounter++;
				int num2 = reference[i];
				ref Node node = ref GetNode(num2);
				Assert.AreEqual(m_CurrentActiveVersion, node.m_Version);
				Assert.AreNotEqual(m_SourceNode, num2);
				Assert.AreNotEqual(m_SinkNode, num2);
				Assert.IsFalse(node.m_Excess <= 0);
				int num3 = node.m_Height - 1;
				Assert.AreEqual(GetLowerCutHeight(activeLayerIndex) + num - 1, num3);
				bool flag3 = false;
				bool flag4 = false;
				for (int j = node.m_FirstConnection; j < node.m_LastConnection; j++)
				{
					Connection connection = GetConnection(j);
					int endNode = connection.m_EndNode;
					ref Node node2 = ref GetNode(endNode);
					if (!flag && node2.m_Version != m_CurrentActiveVersion)
					{
						node2.m_Retreat = false;
						FinalizeTempFlow(in connection);
					}
					if (node2.m_Height == num3)
					{
						int incomingResidualCapacity = connection.GetIncomingResidualCapacity(m_Edges);
						int num4 = math.min(incomingResidualCapacity, node.m_Excess);
						if (num4 != 0)
						{
							if (node2.m_Version != m_CurrentActiveVersion)
							{
								node2.m_Retreat = false;
								node2.m_Version = m_CurrentActiveVersion;
								int num5 = (flag2 ? (m_LayerHeight - 1) : (num - 1));
								CollectionUtils.ElementAt<UnsafeList<int>>(m_ActiveQueue, num5).Add(ref endNode);
							}
							FinalizeTempFlow(in connection);
							AugmentIncomingTempFlow(in connection, num4);
							flag3 = true;
							if (node.m_Retreat)
							{
								node2.m_Retreat = true;
							}
							if (num4 != incomingResidualCapacity)
							{
								Assert.IsTrue(connection.GetIncomingResidualCapacity(m_Edges) > 0);
								flag4 = true;
							}
							else
							{
								Assert.AreEqual(0, connection.GetIncomingResidualCapacity(m_Edges));
							}
						}
					}
					if (flag && flag4 && node.m_Excess == 0)
					{
						break;
					}
					Assert.IsFalse(node.m_Excess < 0);
				}
				Assert.IsTrue(flag3);
				if (!flag4)
				{
					bool flag5 = node.m_Excess != 0;
					if (num3 > 0)
					{
						m_NextLayerIndex = GetLayerIndexForHeight(num3);
						Layer layer = GetLayer(m_NextLayerIndex);
						if (flag5)
						{
							retreatLayerIndex = activeLayerIndex;
						}
						for (int k = node.m_FirstConnection; k < node.m_LastConnection; k++)
						{
							ref Node node3 = ref GetNode(GetConnection(k).m_EndNode);
							if (node3.m_Height == num3)
							{
								layer.GetCutElement(node3.m_CutElementId.m_Index).isChanged = true;
								if (flag5)
								{
									node3.m_Retreat = true;
								}
							}
						}
					}
					else
					{
						m_NextLayerIndex = 0;
						Layer layer2 = GetLayer(m_NextLayerIndex);
						if (flag5)
						{
							retreatLayerIndex = 0;
						}
						layer2.GetCutElement(node.m_CutElementId.m_Index).isDeleted = true;
					}
				}
				else
				{
					Assert.AreEqual(0, node.m_Excess);
				}
			}
			reference.Clear();
		}
	}

	private void RetreatFromLayer(int retreatLayerIndex)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		m_CurrentActiveVersion++;
		Layer layer = GetLayer(retreatLayerIndex);
		ref UnsafeList<int> reference = ref CollectionUtils.ElementAt<UnsafeList<int>>(m_ActiveQueue, 0);
		for (int i = 0; i < layer.m_Elements.Length; i++)
		{
			CutElement cutElement = layer.GetCutElement(i);
			if (cutElement.isAdmissible)
			{
				ref Node node = ref GetNode(cutElement.m_EndNode);
				if ((node.m_Retreat || node.m_Excess > 0) && node.m_Version != m_CurrentActiveVersion)
				{
					node.m_Version = m_CurrentActiveVersion;
					reference.Add(ref cutElement.m_EndNode);
				}
			}
		}
		for (int j = retreatLayerIndex; j <= m_CurrentLayerIndex; j++)
		{
			RetreatActiveLayer(j);
		}
	}

	private void RetreatActiveLayer(int activeLayerIndex)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		bool flag = activeLayerIndex == m_CurrentLayerIndex;
		for (int i = 0; i < m_LayerHeight; i++)
		{
			bool flag2 = flag && i == 0;
			bool flag3 = i == m_LayerHeight - 1;
			ref UnsafeList<int> reference = ref CollectionUtils.ElementAt<UnsafeList<int>>(m_ActiveQueue, i);
			for (int j = 0; j < reference.Length; j++)
			{
				m_StepCounter++;
				int index = reference[j];
				ref Node node = ref GetNode(index);
				Assert.IsFalse(node.m_Excess < 0);
				Assert.IsTrue(node.m_Excess > 0 || node.m_Retreat);
				node.m_Retreat = false;
				int num = node.m_Height + 1;
				GetTotalAdvancedFlow(node, num, out var branchFlow, out var sinkFlow, out var sinkConnection);
				Assert.IsTrue(sinkFlow == 0 || m_CurrentLayerIndex == activeLayerIndex);
				Assert.IsFalse(branchFlow + sinkFlow < node.m_Excess);
				if (flag2)
				{
					Layer layer = GetLayer(activeLayerIndex);
					layer.GetCutElement(node.m_CutElementId.m_Index).isChanged = true;
				}
				for (int k = node.m_FirstConnection; k < node.m_LastConnection; k++)
				{
					Connection connection = GetConnection(k);
					int endNode = connection.m_EndNode;
					if (endNode == m_SinkNode)
					{
						continue;
					}
					ref Node node2 = ref GetNode(endNode);
					if (node2.m_Height != num)
					{
						continue;
					}
					if (branchFlow != 0)
					{
						int outgoingTempFlow = GetOutgoingTempFlow(in connection);
						if (outgoingTempFlow != 0)
						{
							float num2 = (float)outgoingTempFlow / (float)branchFlow;
							branchFlow -= outgoingTempFlow;
							int num3 = math.clamp(node.m_Excess - branchFlow, 0, outgoingTempFlow);
							int num4 = math.min(outgoingTempFlow, node.m_Excess);
							int num5 = math.clamp(Mathf.RoundToInt((float)node.m_Excess * num2), num3, num4);
							if (num5 != 0)
							{
								AugmentIncomingTempFlow(in connection, num5);
							}
						}
					}
					if (node2.m_Version != m_CurrentActiveVersion && (node2.m_Excess != 0 || node2.m_Retreat))
					{
						node2.m_Version = m_CurrentActiveVersion;
						int num6 = ((!flag3) ? (i + 1) : 0);
						CollectionUtils.ElementAt<UnsafeList<int>>(m_ActiveQueue, num6).Add(ref endNode);
					}
				}
				if (node.m_Excess > 0 && sinkFlow > 0)
				{
					Assert.IsTrue(node.m_Excess <= sinkFlow);
					AugmentIncomingTempFlow(in sinkConnection, node.m_Excess);
				}
				Assert.AreEqual(0, node.m_Excess);
			}
			reference.Clear();
		}
	}

	private void GetTotalAdvancedFlow(Node currentNode, int heightPlusOne, out int branchFlow, out int sinkFlow, out Connection sinkConnection)
	{
		sinkConnection = default(Connection);
		branchFlow = 0;
		sinkFlow = 0;
		for (int i = currentNode.m_FirstConnection; i < currentNode.m_LastConnection; i++)
		{
			Connection connection = GetConnection(i);
			if (connection.m_EndNode == m_SinkNode)
			{
				int outgoingTempFlow = GetOutgoingTempFlow(in connection);
				Assert.IsFalse(outgoingTempFlow < 0);
				Assert.IsTrue(sinkFlow == 0);
				sinkFlow = outgoingTempFlow;
				sinkConnection = connection;
			}
			else if (GetNode(connection.m_EndNode).m_Height == heightPlusOne)
			{
				int outgoingTempFlow2 = GetOutgoingTempFlow(in connection);
				Assert.IsFalse(outgoingTempFlow2 < 0);
				branchFlow += outgoingTempFlow2;
			}
		}
	}

	private int GetLayerIndexForHeight(int height)
	{
		return (height - 1) / m_LayerHeight;
	}

	private int GetLowerCutHeight(int layerIndex)
	{
		return layerIndex * m_LayerHeight + 1;
	}

	private int GetUpperCutHeight(int layerIndex)
	{
		return (layerIndex + 1) * m_LayerHeight;
	}

	private void FinalizeTempFlow(in Connection connection)
	{
		GetEdge(connection.m_Edge).FinalizeTempFlow();
	}

	private void AugmentOutgoingTempFlow(in Connection connection, int flow)
	{
		Assert.IsTrue(flow >= 0);
		ref Node node = ref GetNode(connection.m_StartNode);
		ref Node node2 = ref GetNode(connection.m_EndNode);
		ref Edge edge = ref GetEdge(connection.m_Edge);
		node.m_Excess += flow;
		edge.m_TempFlow += (connection.m_Backwards ? (-flow) : flow);
		node2.m_Excess -= flow;
		int flow2 = edge.flow;
		Assert.IsTrue(connection.m_StartNode == m_SinkNode || node.m_Excess >= 0);
		Assert.IsTrue(connection.m_EndNode == m_SinkNode || node2.m_Excess >= 0);
		Assert.IsFalse(flow2 < -edge.GetCapacity(backwards: true));
		Assert.IsFalse(flow2 > edge.GetCapacity(backwards: false));
	}

	private void AugmentIncomingTempFlow(in Connection connection, int flow)
	{
		Assert.IsTrue(flow >= 0);
		ref Node node = ref GetNode(connection.m_StartNode);
		ref Node node2 = ref GetNode(connection.m_EndNode);
		ref Edge edge = ref GetEdge(connection.m_Edge);
		node2.m_Excess += flow;
		edge.m_TempFlow += (connection.m_Backwards ? flow : (-flow));
		node.m_Excess -= flow;
		int flow2 = edge.flow;
		Assert.IsTrue(connection.m_EndNode == m_SinkNode || node2.m_Excess >= 0);
		Assert.IsTrue(connection.m_StartNode == m_SinkNode || node.m_Excess >= 0);
		Assert.IsFalse(flow2 < -edge.GetCapacity(backwards: true));
		Assert.IsFalse(flow2 > edge.GetCapacity(backwards: false));
	}

	private int GetOutgoingTempFlow(in Connection connection)
	{
		Edge edge = GetEdge(connection.m_Edge);
		if (!connection.m_Backwards)
		{
			return edge.m_TempFlow;
		}
		return -edge.m_TempFlow;
	}

	private ref Node GetNode(int index)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return ref CollectionUtils.ElementAt<Node>(m_Nodes, index);
	}

	private ref Edge GetEdge(int index)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return ref CollectionUtils.ElementAt<Edge>(m_Edges, index);
	}

	private Connection GetConnection(int index)
	{
		return m_Connections[index];
	}

	private ref Layer GetLayer(int index)
	{
		return ref m_Layers.ElementAt(index);
	}
}
