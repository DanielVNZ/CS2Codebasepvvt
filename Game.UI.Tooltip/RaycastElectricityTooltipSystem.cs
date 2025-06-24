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
public class RaycastElectricityTooltipSystem : TooltipSystemBase
{
	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityConnectionData> __Game_Prefabs_ElectricityConnectionData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubNet> __Game_Net_SubNet_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Node> __Game_Net_Node_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityNodeConnection> __Game_Simulation_ElectricityNodeConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityValveConnection> __Game_Simulation_ElectricityValveConnection_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedFlowEdge> __Game_Simulation_ConnectedFlowEdge_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityFlowEdge> __Game_Simulation_ElectricityFlowEdge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityConsumer> __Game_Buildings_ElectricityConsumer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityBuildingConnection> __Game_Simulation_ElectricityBuildingConnection_RO_ComponentLookup;

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
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ElectricityConnectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityConnectionData>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<InstalledUpgrade>(true);
			__Game_Net_SubNet_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubNet>(true);
			__Game_Net_Node_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Node>(true);
			__Game_Simulation_ElectricityNodeConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityNodeConnection>(true);
			__Game_Simulation_ElectricityValveConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityValveConnection>(true);
			__Game_Simulation_ConnectedFlowEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedFlowEdge>(true);
			__Game_Simulation_ElectricityFlowEdge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityFlowEdge>(true);
			__Game_Buildings_ElectricityConsumer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityConsumer>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Simulation_ElectricityBuildingConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityBuildingConnection>(true);
		}
	}

	private ToolSystem m_ToolSystem;

	private DefaultToolSystem m_DefaultTool;

	private ToolRaycastSystem m_ToolRaycastSystem;

	private ElectricityFlowSystem m_ElectricityFlowSystem;

	private EntityQuery m_InfomodeQuery;

	private IntTooltip m_Production;

	private IntTooltip m_TransformerCapacity;

	private IntTooltip m_Usage;

	private IntTooltip m_BatteryFlow;

	private IntTooltip m_BatteryCharge;

	private ProgressTooltip m_Consumption;

	private ProgressTooltip m_Flow;

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
		m_ElectricityFlowSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ElectricityFlowSystem>();
		m_InfomodeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<InfomodeActive>(),
			ComponentType.ReadOnly<InfoviewNetStatusData>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_InfomodeQuery);
		m_Production = new IntTooltip
		{
			path = "electricityProduction",
			label = LocalizedString.Id("Tools.ELECTRICITY_PRODUCTION_LABEL"),
			unit = "power"
		};
		m_TransformerCapacity = new IntTooltip
		{
			path = "transformerCapacity",
			label = LocalizedString.Id("SelectedInfoPanel.ELECTRICITY_TRANSFORMER_CAPACITY"),
			unit = "power"
		};
		m_Usage = new IntTooltip
		{
			path = "electricityUsage",
			label = LocalizedString.Id("SelectedInfoPanel.ELECTRICITY_POWER_USAGE"),
			unit = "percentage"
		};
		m_BatteryFlow = new IntTooltip
		{
			path = "batteryFlow",
			label = LocalizedString.Id("Tools.BATTERY_FLOW"),
			unit = "power",
			signed = true
		};
		m_BatteryCharge = new IntTooltip
		{
			path = "batteryCharge",
			label = LocalizedString.Id("Tools.BATTERY_CHARGE"),
			unit = "percentage"
		};
		m_Consumption = new ProgressTooltip
		{
			path = "cElectricityConsumption",
			label = LocalizedString.Id("Tools.ELECTRICITY_CONSUMPTION_LABEL"),
			unit = "power",
			color = TooltipColor.Warning,
			omitMax = true
		};
		m_Flow = new ProgressTooltip
		{
			path = "electricityFlow",
			label = LocalizedString.Id("Tools.ELECTRICITY_FLOW_LABEL"),
			unit = "power"
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
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
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
			if (EntitiesExtensions.TryGetComponent<UtilityLaneData>(((ComponentSystemBase)this).EntityManager, componentData.m_Prefab, ref utilityLaneData) && EntitiesExtensions.TryGetComponent<EdgeMapping>(((ComponentSystemBase)this).EntityManager, result.m_Owner, ref edgeMapping) && (utilityLaneData.m_UtilityTypes & (UtilityTypes.LowVoltageLine | UtilityTypes.HighVoltageLine)) != UtilityTypes.None)
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
		ElectricityProducer electricityProducer = default(ElectricityProducer);
		if (EntitiesExtensions.TryGetComponent<ElectricityProducer>(((ComponentSystemBase)this).EntityManager, result.m_Owner, ref electricityProducer))
		{
			m_Production.value = electricityProducer.m_Capacity;
			AddMouseTooltip(m_Production);
			if (electricityProducer.m_Capacity > 0)
			{
				m_Usage.value = ((electricityProducer.m_LastProduction > 0) ? math.clamp(100 * electricityProducer.m_LastProduction / electricityProducer.m_Capacity, 1, 100) : 0);
				m_Usage.color = (HasBottleneck(result.m_Owner) ? TooltipColor.Warning : TooltipColor.Info);
				AddMouseTooltip(m_Usage);
			}
		}
		else
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.Transformer>(result.m_Owner))
			{
				Game.Simulation.TransformerData transformerData = new Game.Simulation.TransformerData
				{
					m_Deleted = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabRefs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_ElectricityConnectionDatas = InternalCompilerInterface.GetComponentLookup<ElectricityConnectionData>(ref __TypeHandle.__Game_Prefabs_ElectricityConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_InstalledUpgrades = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_SubNets = InternalCompilerInterface.GetBufferLookup<Game.Net.SubNet>(ref __TypeHandle.__Game_Net_SubNet_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_NetNodes = InternalCompilerInterface.GetComponentLookup<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_ElectricityNodeConnections = InternalCompilerInterface.GetComponentLookup<ElectricityNodeConnection>(ref __TypeHandle.__Game_Simulation_ElectricityNodeConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_ElectricityValveConnections = InternalCompilerInterface.GetComponentLookup<ElectricityValveConnection>(ref __TypeHandle.__Game_Simulation_ElectricityValveConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_FlowConnections = InternalCompilerInterface.GetBufferLookup<ConnectedFlowEdge>(ref __TypeHandle.__Game_Simulation_ConnectedFlowEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_FlowEdges = InternalCompilerInterface.GetComponentLookup<ElectricityFlowEdge>(ref __TypeHandle.__Game_Simulation_ElectricityFlowEdge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
				};
				transformerData.GetTransformerData(result.m_Owner, out var capacity, out var flow);
				m_TransformerCapacity.value = capacity;
				AddMouseTooltip(m_TransformerCapacity);
				if (capacity > 0)
				{
					m_Usage.value = ((flow != 0) ? math.clamp(100 * math.abs(flow) / capacity, 1, 100) : 0);
					m_Usage.color = (HasBottleneck(result.m_Owner) ? TooltipColor.Warning : TooltipColor.Info);
					AddMouseTooltip(m_Usage);
				}
			}
		}
		Game.Buildings.Battery battery = default(Game.Buildings.Battery);
		if (EntitiesExtensions.TryGetComponent<Game.Buildings.Battery>(((ComponentSystemBase)this).EntityManager, result.m_Owner, ref battery) && battery.m_Capacity > 0)
		{
			m_BatteryFlow.value = battery.m_LastFlow;
			m_BatteryCharge.value = 100 * battery.storedEnergyHours / battery.m_Capacity;
			m_BatteryCharge.color = ((battery.m_StoredEnergy <= 0) ? TooltipColor.Warning : TooltipColor.Info);
			AddMouseTooltip(m_BatteryFlow);
			AddMouseTooltip(m_BatteryCharge);
		}
		ElectricityConsumer electricityConsumer = default(ElectricityConsumer);
		if (EntitiesExtensions.TryGetComponent<ElectricityConsumer>(((ComponentSystemBase)this).EntityManager, result.m_Owner, ref electricityConsumer))
		{
			m_Consumption.value = electricityConsumer.m_FulfilledConsumption;
			m_Consumption.max = electricityConsumer.m_WantedConsumption;
			m_Consumption.color = ((electricityConsumer.m_FulfilledConsumption < electricityConsumer.m_WantedConsumption) ? TooltipColor.Warning : TooltipColor.Info);
			AddMouseTooltip(m_Consumption);
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
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		ComponentLookup<ElectricityNodeConnection> componentLookup = InternalCompilerInterface.GetComponentLookup<ElectricityNodeConnection>(ref __TypeHandle.__Game_Simulation_ElectricityNodeConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		BufferLookup<ConnectedFlowEdge> flowConnections = InternalCompilerInterface.GetBufferLookup<ConnectedFlowEdge>(ref __TypeHandle.__Game_Simulation_ConnectedFlowEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<ElectricityFlowEdge> flowEdges = InternalCompilerInterface.GetComponentLookup<ElectricityFlowEdge>(ref __TypeHandle.__Game_Simulation_ElectricityFlowEdge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<ElectricityConsumer> componentLookup2 = InternalCompilerInterface.GetComponentLookup<ElectricityConsumer>(ref __TypeHandle.__Game_Buildings_ElectricityConsumer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<Building> componentLookup3 = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<ElectricityBuildingConnection> componentLookup4 = InternalCompilerInterface.GetComponentLookup<ElectricityBuildingConnection>(ref __TypeHandle.__Game_Simulation_ElectricityBuildingConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		Edge edge2 = default(Edge);
		ElectricityNodeConnection electricityNodeConnection = default(ElectricityNodeConnection);
		ElectricityNodeConnection electricityNodeConnection2 = default(ElectricityNodeConnection);
		ElectricityNodeConnection electricityNodeConnection3 = default(ElectricityNodeConnection);
		if (!EntitiesExtensions.TryGetComponent<Edge>(((ComponentSystemBase)this).EntityManager, edge, ref edge2) || !componentLookup.TryGetComponent(edge, ref electricityNodeConnection) || !componentLookup.TryGetComponent(edge2.m_Start, ref electricityNodeConnection2) || !componentLookup.TryGetComponent(edge2.m_End, ref electricityNodeConnection3) || !ElectricityGraphUtils.TryGetFlowEdge(electricityNodeConnection2.m_ElectricityNode, electricityNodeConnection.m_ElectricityNode, ref flowConnections, ref flowEdges, out ElectricityFlowEdge edge3))
		{
			return;
		}
		int num = math.max(1, edge3.m_Capacity);
		int num2 = edge3.m_Flow;
		DynamicBuffer<ConnectedNode> val = default(DynamicBuffer<ConnectedNode>);
		if (EntitiesExtensions.TryGetBuffer<ConnectedNode>(((ComponentSystemBase)this).EntityManager, edge, true, ref val))
		{
			ElectricityNodeConnection electricityNodeConnection4 = default(ElectricityNodeConnection);
			for (int i = 0; i < val.Length; i++)
			{
				ConnectedNode connectedNode = val[i];
				if (connectedNode.m_CurvePosition < curvePosition && componentLookup.TryGetComponent(connectedNode.m_Node, ref electricityNodeConnection4) && ElectricityGraphUtils.TryGetFlowEdge(electricityNodeConnection4.m_ElectricityNode, electricityNodeConnection.m_ElectricityNode, ref flowConnections, ref flowEdges, out ElectricityFlowEdge edge4))
				{
					num2 += edge4.m_Flow;
				}
			}
		}
		DynamicBuffer<ConnectedBuilding> val2 = default(DynamicBuffer<ConnectedBuilding>);
		if (ElectricityGraphUtils.TryGetFlowEdge(electricityNodeConnection.m_ElectricityNode, m_ElectricityFlowSystem.sinkNode, ref flowConnections, ref flowEdges, out ElectricityFlowEdge edge5) && EntitiesExtensions.TryGetBuffer<ConnectedBuilding>(((ComponentSystemBase)this).EntityManager, edge, true, ref val2))
		{
			int totalSupply = edge5.m_Flow;
			int totalDemand = 0;
			ElectricityConsumer electricityConsumer = default(ElectricityConsumer);
			for (int j = 0; j < val2.Length; j++)
			{
				ConnectedBuilding connectedBuilding = val2[j];
				if (!componentLookup4.HasComponent(connectedBuilding.m_Building) && componentLookup2.TryGetComponent(connectedBuilding.m_Building, ref electricityConsumer))
				{
					totalDemand += electricityConsumer.m_WantedConsumption;
				}
			}
			ElectricityConsumer electricityConsumer2 = default(ElectricityConsumer);
			for (int k = 0; k < val2.Length; k++)
			{
				ConnectedBuilding connectedBuilding2 = val2[k];
				if (!componentLookup4.HasComponent(connectedBuilding2.m_Building) && componentLookup2.TryGetComponent(connectedBuilding2.m_Building, ref electricityConsumer2))
				{
					int num3 = FlowUtils.ConsumeFromTotal(electricityConsumer2.m_WantedConsumption, ref totalSupply, ref totalDemand);
					if (componentLookup3[connectedBuilding2.m_Building].m_CurvePosition < curvePosition)
					{
						num2 -= num3;
					}
				}
			}
		}
		m_Flow.value = math.abs(num2);
		m_Flow.max = num;
		AddMouseTooltip(m_Flow);
	}

	private void AddNodeFlow(Entity node, Entity edge)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		ElectricityNodeConnection electricityNodeConnection = default(ElectricityNodeConnection);
		ElectricityNodeConnection electricityNodeConnection2 = default(ElectricityNodeConnection);
		DynamicBuffer<ConnectedFlowEdge> val = default(DynamicBuffer<ConnectedFlowEdge>);
		if (!EntitiesExtensions.TryGetComponent<ElectricityNodeConnection>(((ComponentSystemBase)this).EntityManager, node, ref electricityNodeConnection) || !EntitiesExtensions.TryGetComponent<ElectricityNodeConnection>(((ComponentSystemBase)this).EntityManager, edge, ref electricityNodeConnection2) || !EntitiesExtensions.TryGetBuffer<ConnectedFlowEdge>(((ComponentSystemBase)this).EntityManager, electricityNodeConnection.m_ElectricityNode, true, ref val))
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < val.Length; i++)
		{
			Entity edge2 = val[i].m_Edge;
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			ElectricityFlowEdge componentData = ((EntityManager)(ref entityManager)).GetComponentData<ElectricityFlowEdge>(edge2);
			if (componentData.m_Start == electricityNodeConnection2.m_ElectricityNode || componentData.m_End == electricityNodeConnection2.m_ElectricityNode)
			{
				int num3 = math.abs(componentData.m_Flow);
				if (num3 > num || (num3 == num && componentData.m_Capacity > num2))
				{
					num = num3;
					num2 = componentData.m_Capacity;
				}
			}
		}
		if (num2 > 0)
		{
			m_Flow.value = num;
			m_Flow.max = num2;
			AddMouseTooltip(m_Flow);
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
					if (current.m_Type == NetStatusType.LowVoltageFlow || current.m_Type == NetStatusType.HighVoltageFlow)
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

	private bool HasBottleneck(Entity building)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		ElectricityBuildingConnection electricityBuildingConnection = default(ElectricityBuildingConnection);
		if (EntitiesExtensions.TryGetComponent<ElectricityBuildingConnection>(((ComponentSystemBase)this).EntityManager, building, ref electricityBuildingConnection))
		{
			EntityManager entityManager;
			if (electricityBuildingConnection.m_ProducerEdge != Entity.Null)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).GetComponentData<ElectricityFlowEdge>(electricityBuildingConnection.m_ProducerEdge).isBottleneck)
				{
					return true;
				}
			}
			if (electricityBuildingConnection.m_TransformerNode != Entity.Null)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				Enumerator<ConnectedFlowEdge> enumerator = ((EntityManager)(ref entityManager)).GetBuffer<ConnectedFlowEdge>(electricityBuildingConnection.m_TransformerNode, true).GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						ConnectedFlowEdge current = enumerator.Current;
						entityManager = ((ComponentSystemBase)this).EntityManager;
						if (((EntityManager)(ref entityManager)).GetComponentData<ElectricityFlowEdge>(current.m_Edge).isBottleneck)
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
	public RaycastElectricityTooltipSystem()
	{
	}
}
