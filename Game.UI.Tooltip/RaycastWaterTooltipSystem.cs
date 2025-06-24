using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Game.Buildings;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Game.UI.Localization;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.UI.Tooltip;

[CompilerGenerated]
public class RaycastWaterTooltipSystem : TooltipSystemBase
{
	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<WaterPipeNodeConnection> __Game_Simulation_WaterPipeNodeConnection_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedFlowEdge> __Game_Simulation_ConnectedFlowEdge_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<WaterPipeEdge> __Game_Simulation_WaterPipeEdge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WaterConsumer> __Game_Buildings_WaterConsumer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WaterPipeBuildingConnection> __Game_Simulation_WaterPipeBuildingConnection_RO_ComponentLookup;

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
			__Game_Simulation_WaterPipeNodeConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterPipeNodeConnection>(true);
			__Game_Simulation_ConnectedFlowEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedFlowEdge>(true);
			__Game_Simulation_WaterPipeEdge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterPipeEdge>(true);
			__Game_Buildings_WaterConsumer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterConsumer>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Simulation_WaterPipeBuildingConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterPipeBuildingConnection>(true);
		}
	}

	private ToolSystem m_ToolSystem;

	private DefaultToolSystem m_DefaultTool;

	private ToolRaycastSystem m_ToolRaycastSystem;

	private WaterPipeFlowSystem m_WaterPipeFlowSystem;

	private EntityQuery m_InfomodeQuery;

	private IntTooltip m_WaterCapacity;

	private IntTooltip m_WaterUsage;

	private IntTooltip m_SewageCapacity;

	private IntTooltip m_SewageUsage;

	private ProgressTooltip m_WaterConsumption;

	private ProgressTooltip m_SewageConsumption;

	private IntTooltip m_WaterFlow;

	private IntTooltip m_SewageFlow;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_DefaultTool = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DefaultToolSystem>();
		m_ToolRaycastSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolRaycastSystem>();
		m_WaterPipeFlowSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterPipeFlowSystem>();
		m_InfomodeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<InfomodeActive>(),
			ComponentType.ReadOnly<InfoviewNetStatusData>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_InfomodeQuery);
		m_WaterCapacity = new IntTooltip
		{
			path = "waterCapacity",
			label = LocalizedString.Id("SelectedInfoPanel.WATER_OUTPUT"),
			unit = "volume"
		};
		m_WaterUsage = new IntTooltip
		{
			path = "waterUsage",
			label = LocalizedString.Id("SelectedInfoPanel.WATER_PUMP_USAGE"),
			unit = "percentage"
		};
		m_SewageCapacity = new IntTooltip
		{
			path = "sewageCapacity",
			label = LocalizedString.Id("SelectedInfoPanel.SEWAGE_PROCESSING_CAPACITY"),
			unit = "volume"
		};
		m_SewageUsage = new IntTooltip
		{
			path = "sewageUsage",
			label = LocalizedString.Id("SelectedInfoPanel.SEWAGE_OUTLET_USAGE"),
			unit = "percentage"
		};
		m_WaterConsumption = new ProgressTooltip
		{
			path = "waterConsumption",
			label = LocalizedString.Id("Tools.WATER_CONSUMPTION_LABEL"),
			unit = "volume",
			color = TooltipColor.Warning,
			omitMax = true
		};
		m_SewageConsumption = new ProgressTooltip
		{
			path = "sewageConsumption",
			label = LocalizedString.Id("Tools.SEWAGE_CONSUMPTION_LABEL"),
			unit = "volume",
			color = TooltipColor.Warning,
			omitMax = true
		};
		m_WaterFlow = new IntTooltip
		{
			path = "waterFlow",
			label = LocalizedString.Id("Tools.WATER_FLOW_LABEL"),
			unit = "volume"
		};
		m_SewageFlow = new IntTooltip
		{
			path = "sewageFlow",
			label = LocalizedString.Id("Tools.SEWAGE_FLOW_LABEL"),
			unit = "volume"
		};
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		((SystemBase)this).CompleteDependency();
		if (!IsInfomodeActivated() || m_ToolSystem.activeTool != m_DefaultTool || !m_ToolRaycastSystem.GetRaycastResult(out var result))
		{
			return;
		}
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Destroyed>(result.m_Owner))
		{
			return;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Net.UtilityLane>(result.m_Owner))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			PrefabRef componentData = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(result.m_Owner);
			UtilityLaneData utilityLaneData = default(UtilityLaneData);
			EdgeMapping edgeMapping = default(EdgeMapping);
			if (EntitiesExtensions.TryGetComponent<UtilityLaneData>(((ComponentSystemBase)this).EntityManager, componentData.m_Prefab, ref utilityLaneData) && EntitiesExtensions.TryGetComponent<EdgeMapping>(((ComponentSystemBase)this).EntityManager, result.m_Owner, ref edgeMapping) && (utilityLaneData.m_UtilityTypes & (UtilityTypes.WaterPipe | UtilityTypes.SewagePipe)) != UtilityTypes.None)
			{
				if (edgeMapping.m_Parent1 != Entity.Null)
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<Edge>(edgeMapping.m_Parent1))
					{
						if (edgeMapping.m_Parent2 != Entity.Null)
						{
							if (result.m_Hit.m_CurvePosition < 0.5f)
							{
								float curvePosition = math.lerp(edgeMapping.m_CurveDelta1.x, edgeMapping.m_CurveDelta1.y, result.m_Hit.m_CurvePosition * 2f);
								AddEdgeFlow(edgeMapping.m_Parent1, curvePosition);
							}
							else
							{
								float curvePosition2 = math.lerp(edgeMapping.m_CurveDelta2.x, edgeMapping.m_CurveDelta2.y, result.m_Hit.m_CurvePosition * 2f - 1f);
								AddEdgeFlow(edgeMapping.m_Parent2, curvePosition2);
							}
						}
						else
						{
							float curvePosition3 = math.lerp(edgeMapping.m_CurveDelta1.x, edgeMapping.m_CurveDelta1.y, result.m_Hit.m_CurvePosition);
							AddEdgeFlow(edgeMapping.m_Parent1, curvePosition3);
						}
					}
					else
					{
						AddNodeFlow(edgeMapping.m_Parent1, edgeMapping.m_Parent2);
					}
				}
				else
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					Owner owner = default(Owner);
					if (((EntityManager)(ref entityManager)).HasComponent<Game.Net.SecondaryLane>(result.m_Owner) && EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, result.m_Owner, ref owner))
					{
						result.m_Owner = owner.m_Owner;
					}
				}
			}
		}
		Game.Buildings.SewageOutlet sewageOutlet = default(Game.Buildings.SewageOutlet);
		if (EntitiesExtensions.TryGetComponent<Game.Buildings.SewageOutlet>(((ComponentSystemBase)this).EntityManager, result.m_Owner, ref sewageOutlet))
		{
			m_SewageCapacity.value = sewageOutlet.m_Capacity;
			AddMouseTooltip(m_SewageCapacity);
			if (sewageOutlet.m_Capacity > 0)
			{
				m_SewageUsage.value = ((sewageOutlet.m_LastProcessed > 0) ? math.clamp(100 * sewageOutlet.m_LastProcessed / sewageOutlet.m_Capacity, 1, 100) : 0);
				m_SewageUsage.color = TooltipColor.Info;
				AddMouseTooltip(m_SewageUsage);
			}
		}
		Game.Buildings.WaterPumpingStation waterPumpingStation = default(Game.Buildings.WaterPumpingStation);
		if (EntitiesExtensions.TryGetComponent<Game.Buildings.WaterPumpingStation>(((ComponentSystemBase)this).EntityManager, result.m_Owner, ref waterPumpingStation))
		{
			m_WaterCapacity.value = waterPumpingStation.m_Capacity;
			AddMouseTooltip(m_WaterCapacity);
			if (waterPumpingStation.m_Capacity > 0)
			{
				m_WaterUsage.value = ((waterPumpingStation.m_LastProduction > 0) ? math.clamp(100 * waterPumpingStation.m_LastProduction / waterPumpingStation.m_Capacity, 1, 100) : 0);
				m_WaterUsage.color = TooltipColor.Info;
				AddMouseTooltip(m_WaterUsage);
			}
		}
		WaterConsumer waterConsumer = default(WaterConsumer);
		if (EntitiesExtensions.TryGetComponent<WaterConsumer>(((ComponentSystemBase)this).EntityManager, result.m_Owner, ref waterConsumer))
		{
			m_WaterConsumption.value = waterConsumer.m_FulfilledFresh;
			m_WaterConsumption.max = waterConsumer.m_WantedConsumption;
			m_WaterConsumption.color = ((waterConsumer.m_FulfilledFresh < waterConsumer.m_WantedConsumption) ? TooltipColor.Warning : TooltipColor.Info);
			AddMouseTooltip(m_WaterConsumption);
			if (waterConsumer.m_FulfilledFresh < waterConsumer.m_WantedConsumption || waterConsumer.m_FulfilledSewage < waterConsumer.m_WantedConsumption)
			{
				m_SewageConsumption.value = waterConsumer.m_FulfilledSewage;
				m_SewageConsumption.max = waterConsumer.m_WantedConsumption;
				m_SewageConsumption.color = ((waterConsumer.m_FulfilledSewage < waterConsumer.m_WantedConsumption) ? TooltipColor.Warning : TooltipColor.Info);
				AddMouseTooltip(m_SewageConsumption);
			}
		}
	}

	private void AddEdgeFlow(Entity edge, float curvePosition)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		ComponentLookup<WaterPipeNodeConnection> componentLookup = InternalCompilerInterface.GetComponentLookup<WaterPipeNodeConnection>(ref __TypeHandle.__Game_Simulation_WaterPipeNodeConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		BufferLookup<ConnectedFlowEdge> flowConnections = InternalCompilerInterface.GetBufferLookup<ConnectedFlowEdge>(ref __TypeHandle.__Game_Simulation_ConnectedFlowEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<WaterPipeEdge> flowEdges = InternalCompilerInterface.GetComponentLookup<WaterPipeEdge>(ref __TypeHandle.__Game_Simulation_WaterPipeEdge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<WaterConsumer> componentLookup2 = InternalCompilerInterface.GetComponentLookup<WaterConsumer>(ref __TypeHandle.__Game_Buildings_WaterConsumer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<Building> componentLookup3 = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<WaterPipeBuildingConnection> componentLookup4 = InternalCompilerInterface.GetComponentLookup<WaterPipeBuildingConnection>(ref __TypeHandle.__Game_Simulation_WaterPipeBuildingConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		Edge edge2 = default(Edge);
		WaterPipeNodeConnection waterPipeNodeConnection = default(WaterPipeNodeConnection);
		WaterPipeNodeConnection waterPipeNodeConnection2 = default(WaterPipeNodeConnection);
		WaterPipeNodeConnection waterPipeNodeConnection3 = default(WaterPipeNodeConnection);
		if (!EntitiesExtensions.TryGetComponent<Edge>(((ComponentSystemBase)this).EntityManager, edge, ref edge2) || !componentLookup.TryGetComponent(edge, ref waterPipeNodeConnection) || !componentLookup.TryGetComponent(edge2.m_Start, ref waterPipeNodeConnection2) || !componentLookup.TryGetComponent(edge2.m_End, ref waterPipeNodeConnection3) || !WaterPipeGraphUtils.TryGetFlowEdge(waterPipeNodeConnection2.m_WaterPipeNode, waterPipeNodeConnection.m_WaterPipeNode, ref flowConnections, ref flowEdges, out WaterPipeEdge edge3))
		{
			return;
		}
		int2 val = math.max(int2.op_Implicit(1), edge3.capacity);
		int2 val2 = edge3.flow;
		DynamicBuffer<ConnectedNode> val3 = default(DynamicBuffer<ConnectedNode>);
		if (EntitiesExtensions.TryGetBuffer<ConnectedNode>(((ComponentSystemBase)this).EntityManager, edge, true, ref val3))
		{
			WaterPipeNodeConnection waterPipeNodeConnection4 = default(WaterPipeNodeConnection);
			for (int i = 0; i < val3.Length; i++)
			{
				ConnectedNode connectedNode = val3[i];
				if (connectedNode.m_CurvePosition < curvePosition && componentLookup.TryGetComponent(connectedNode.m_Node, ref waterPipeNodeConnection4) && WaterPipeGraphUtils.TryGetFlowEdge(waterPipeNodeConnection4.m_WaterPipeNode, waterPipeNodeConnection.m_WaterPipeNode, ref flowConnections, ref flowEdges, out WaterPipeEdge edge4))
				{
					val2 += edge4.flow;
				}
			}
		}
		DynamicBuffer<ConnectedBuilding> val4 = default(DynamicBuffer<ConnectedBuilding>);
		if (WaterPipeGraphUtils.TryGetFlowEdge(waterPipeNodeConnection.m_WaterPipeNode, m_WaterPipeFlowSystem.sinkNode, ref flowConnections, ref flowEdges, out WaterPipeEdge edge5) && EntitiesExtensions.TryGetBuffer<ConnectedBuilding>(((ComponentSystemBase)this).EntityManager, edge, true, ref val4))
		{
			int2 flow = edge5.flow;
			int2 val5 = int2.op_Implicit(0);
			WaterConsumer waterConsumer = default(WaterConsumer);
			for (int j = 0; j < val4.Length; j++)
			{
				ConnectedBuilding connectedBuilding = val4[j];
				if (!componentLookup4.HasComponent(connectedBuilding.m_Building) && componentLookup2.TryGetComponent(connectedBuilding.m_Building, ref waterConsumer))
				{
					val5 += waterConsumer.m_WantedConsumption;
				}
			}
			WaterConsumer waterConsumer2 = default(WaterConsumer);
			int2 val6 = default(int2);
			for (int k = 0; k < val4.Length; k++)
			{
				ConnectedBuilding connectedBuilding2 = val4[k];
				if (!componentLookup4.HasComponent(connectedBuilding2.m_Building) && componentLookup2.TryGetComponent(connectedBuilding2.m_Building, ref waterConsumer2))
				{
					((int2)(ref val6))._002Ector(FlowUtils.ConsumeFromTotal(waterConsumer2.m_WantedConsumption, ref flow.x, ref val5.x), FlowUtils.ConsumeFromTotal(waterConsumer2.m_WantedConsumption, ref flow.y, ref val5.y));
					if (componentLookup3[connectedBuilding2.m_Building].m_CurvePosition < curvePosition)
					{
						val2 -= val6;
					}
				}
			}
		}
		if (val.x > 0)
		{
			m_WaterFlow.value = math.abs(val2.x);
			AddMouseTooltip(m_WaterFlow);
		}
		if (val.y > 0)
		{
			m_SewageFlow.value = math.abs(val2.y);
			AddMouseTooltip(m_SewageFlow);
		}
	}

	private void AddNodeFlow(Entity node, Entity edge)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		WaterPipeNodeConnection waterPipeNodeConnection = default(WaterPipeNodeConnection);
		WaterPipeNodeConnection waterPipeNodeConnection2 = default(WaterPipeNodeConnection);
		DynamicBuffer<ConnectedFlowEdge> val = default(DynamicBuffer<ConnectedFlowEdge>);
		if (!EntitiesExtensions.TryGetComponent<WaterPipeNodeConnection>(((ComponentSystemBase)this).EntityManager, node, ref waterPipeNodeConnection) || !EntitiesExtensions.TryGetComponent<WaterPipeNodeConnection>(((ComponentSystemBase)this).EntityManager, edge, ref waterPipeNodeConnection2) || !EntitiesExtensions.TryGetBuffer<ConnectedFlowEdge>(((ComponentSystemBase)this).EntityManager, waterPipeNodeConnection.m_WaterPipeNode, true, ref val))
		{
			return;
		}
		int2 val2 = int2.op_Implicit(0);
		int2 val3 = int2.op_Implicit(0);
		for (int i = 0; i < val.Length; i++)
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			WaterPipeEdge componentData = ((EntityManager)(ref entityManager)).GetComponentData<WaterPipeEdge>(val[i].m_Edge);
			if (componentData.m_Start == waterPipeNodeConnection2.m_WaterPipeNode || componentData.m_End == waterPipeNodeConnection2.m_WaterPipeNode)
			{
				val2 = math.max(val2, math.abs(componentData.flow));
				val3 = math.max(val3, componentData.capacity);
			}
		}
		if (val3.x > 0)
		{
			m_WaterFlow.value = val2.x;
			AddMouseTooltip(m_WaterFlow);
		}
		if (val3.y > 0)
		{
			m_SewageFlow.value = val2.y;
			AddMouseTooltip(m_SewageFlow);
		}
	}

	private bool IsInfomodeActivated()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<InfoviewNetStatusData> val = ((EntityQuery)(ref m_InfomodeQuery)).ToComponentDataArray<InfoviewNetStatusData>(AllocatorHandle.op_Implicit((Allocator)2));
		try
		{
			Enumerator<InfoviewNetStatusData> enumerator = val.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					InfoviewNetStatusData current = enumerator.Current;
					if (current.m_Type == NetStatusType.PipeWaterFlow || current.m_Type == NetStatusType.PipeSewageFlow)
					{
						return true;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
		}
		finally
		{
			val.Dispose();
		}
		return false;
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
	public RaycastWaterTooltipSystem()
	{
	}
}
