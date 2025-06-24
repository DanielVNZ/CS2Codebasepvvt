using Game.Buildings;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Simulation;

public struct TransformerData
{
	[ReadOnly]
	public ComponentLookup<Deleted> m_Deleted;

	[ReadOnly]
	public ComponentLookup<PrefabRef> m_PrefabRefs;

	[ReadOnly]
	public ComponentLookup<ElectricityConnectionData> m_ElectricityConnectionDatas;

	[ReadOnly]
	public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

	[ReadOnly]
	public BufferLookup<Game.Net.SubNet> m_SubNets;

	[ReadOnly]
	public ComponentLookup<Node> m_NetNodes;

	[ReadOnly]
	public ComponentLookup<ElectricityNodeConnection> m_ElectricityNodeConnections;

	[ReadOnly]
	public ComponentLookup<ElectricityValveConnection> m_ElectricityValveConnections;

	[ReadOnly]
	public BufferLookup<ConnectedFlowEdge> m_FlowConnections;

	[ReadOnly]
	public ComponentLookup<ElectricityFlowEdge> m_FlowEdges;

	public void GetTransformerData(Entity entity, out int capacity, out int flow)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		int lowVoltageCapacity = 0;
		int highVoltageCapacity = 0;
		flow = 0;
		DynamicBuffer<Game.Net.SubNet> subNets = default(DynamicBuffer<Game.Net.SubNet>);
		if (m_SubNets.TryGetBuffer(entity, ref subNets))
		{
			ProcessMarkerNodes(subNets, ref lowVoltageCapacity, ref highVoltageCapacity, ref flow);
		}
		DynamicBuffer<InstalledUpgrade> upgrades = default(DynamicBuffer<InstalledUpgrade>);
		if (m_InstalledUpgrades.TryGetBuffer(entity, ref upgrades))
		{
			ProcessMarkerNodes(upgrades, ref lowVoltageCapacity, ref highVoltageCapacity, ref flow);
		}
		capacity = math.min(lowVoltageCapacity, highVoltageCapacity);
	}

	private void ProcessMarkerNodes(DynamicBuffer<InstalledUpgrade> upgrades, ref int lowVoltageCapacity, ref int highVoltageCapacity, ref int flow)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<Game.Net.SubNet> subNets = default(DynamicBuffer<Game.Net.SubNet>);
		for (int i = 0; i < upgrades.Length; i++)
		{
			InstalledUpgrade installedUpgrade = upgrades[i];
			if (!BuildingUtils.CheckOption(installedUpgrade, BuildingOption.Inactive) && m_SubNets.TryGetBuffer(installedUpgrade.m_Upgrade, ref subNets))
			{
				ProcessMarkerNodes(subNets, ref lowVoltageCapacity, ref highVoltageCapacity, ref flow);
			}
		}
	}

	private void ProcessMarkerNodes(DynamicBuffer<Game.Net.SubNet> subNets, ref int lowVoltageCapacity, ref int highVoltageCapacity, ref int flow)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		ElectricityNodeConnection electricityNodeConnection = default(ElectricityNodeConnection);
		ElectricityValveConnection electricityValveConnection = default(ElectricityValveConnection);
		PrefabRef prefabRef = default(PrefabRef);
		ElectricityConnectionData electricityConnectionData = default(ElectricityConnectionData);
		for (int i = 0; i < subNets.Length; i++)
		{
			Entity subNet = subNets[i].m_SubNet;
			if (!m_NetNodes.HasComponent(subNet) || m_Deleted.HasComponent(subNet) || !m_ElectricityNodeConnections.TryGetComponent(subNet, ref electricityNodeConnection) || !m_ElectricityValveConnections.TryGetComponent(subNet, ref electricityValveConnection) || !m_PrefabRefs.TryGetComponent(subNet, ref prefabRef) || !m_ElectricityConnectionDatas.TryGetComponent(prefabRef.m_Prefab, ref electricityConnectionData))
			{
				continue;
			}
			if (electricityConnectionData.m_Voltage == Game.Prefabs.ElectricityConnection.Voltage.Low)
			{
				lowVoltageCapacity += electricityConnectionData.m_Capacity;
				if (ElectricityGraphUtils.TryGetFlowEdge(electricityValveConnection.m_ValveNode, electricityNodeConnection.m_ElectricityNode, ref m_FlowConnections, ref m_FlowEdges, out ElectricityFlowEdge edge))
				{
					flow += edge.m_Flow;
				}
			}
			else
			{
				highVoltageCapacity += electricityConnectionData.m_Capacity;
			}
		}
	}
}
