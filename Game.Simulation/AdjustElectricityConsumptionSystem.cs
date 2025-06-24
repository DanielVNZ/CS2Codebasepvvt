using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Prefabs;
using Game.Tools;
using Unity.Assertions;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class AdjustElectricityConsumptionSystem : GameSystemBase
{
	[BurstCompile]
	public struct AdjustElectricityConsumptionJob : IJobChunk
	{
		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<Building> m_BuildingType;

		[ReadOnly]
		public ComponentTypeHandle<CityServiceUpkeep> m_CityServiceType;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> m_UpgradeType;

		[ReadOnly]
		public ComponentTypeHandle<ElectricityBuildingConnection> m_BuildingConnectionType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentDistrict> m_CurrentDistrictType;

		[ReadOnly]
		public BufferTypeHandle<Renter> m_RenterType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.Park> m_ParkType;

		[ReadOnly]
		public ComponentTypeHandle<StorageProperty> m_StoragePropertyType;

		public ComponentTypeHandle<ElectricityConsumer> m_ConsumerType;

		public BufferTypeHandle<Efficiency> m_EfficiencyType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<ConsumptionData> m_ServiceConsumption;

		[ReadOnly]
		public BufferLookup<ServiceFee> m_Fees;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> m_HouseholdCitizens;

		[ReadOnly]
		public BufferLookup<Employee> m_Employees;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> m_SpawnableDatas;

		[ReadOnly]
		public ComponentLookup<Citizen> m_Citizens;

		[ReadOnly]
		public BufferLookup<DistrictModifier> m_DistrictModifiers;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<ElectricityFlowEdge> m_FlowEdges;

		public ParallelWriter<Entity> m_UpdatedEdges;

		public ServiceFeeParameterData m_FeeParameters;

		public BuildingEfficiencyParameterData m_EfficiencyParameters;

		public RandomSeed m_RandomSeed;

		public Entity m_City;

		public float m_TemperatureMultiplier;

		public uint m_UpdateFrameIndex;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index != m_UpdateFrameIndex)
			{
				return;
			}
			NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<Building> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Building>(ref m_BuildingType);
			NativeArray<CurrentDistrict> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentDistrict>(ref m_CurrentDistrictType);
			BufferAccessor<InstalledUpgrade> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<InstalledUpgrade>(ref m_UpgradeType);
			NativeArray<ElectricityBuildingConnection> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ElectricityBuildingConnection>(ref m_BuildingConnectionType);
			BufferAccessor<Renter> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Renter>(ref m_RenterType);
			NativeArray<ElectricityConsumer> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ElectricityConsumer>(ref m_ConsumerType);
			BufferAccessor<Efficiency> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Efficiency>(ref m_EfficiencyType);
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.Park>(ref m_ParkType);
			bool flag2 = ((ArchetypeChunk)(ref chunk)).Has<StorageProperty>(ref m_StoragePropertyType);
			Random random = m_RandomSeed.GetRandom(1 + unfilteredChunkIndex);
			float num;
			float efficiency;
			if (((ArchetypeChunk)(ref chunk)).Has<CityServiceUpkeep>(ref m_CityServiceType))
			{
				num = 1f;
				efficiency = 1f;
			}
			else
			{
				float relativeFee = ServiceFeeSystem.GetFee(PlayerResource.Electricity, m_Fees[m_City]) / m_FeeParameters.m_ElectricityFee.m_Default;
				num = GetFeeConsumptionMultiplier(relativeFee, in m_FeeParameters);
				efficiency = GetFeeEfficiencyFactor(relativeFee, in m_EfficiencyParameters);
			}
			ConsumptionData data = default(ConsumptionData);
			DynamicBuffer<DistrictModifier> modifiers = default(DynamicBuffer<DistrictModifier>);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity prefab = nativeArray[i].m_Prefab;
				m_ServiceConsumption.TryGetComponent(prefab, ref data);
				if (bufferAccessor.Length != 0)
				{
					UpgradeUtils.CombineStats<ConsumptionData>(ref data, bufferAccessor[i], ref m_Prefabs, ref m_ServiceConsumption);
				}
				float electricityConsumption = data.m_ElectricityConsumption;
				electricityConsumption *= m_TemperatureMultiplier;
				electricityConsumption *= num;
				if (nativeArray3.Length != 0)
				{
					Entity district = nativeArray3[i].m_District;
					if (m_DistrictModifiers.TryGetBuffer(district, ref modifiers))
					{
						AreaUtils.ApplyModifier(ref electricityConsumption, modifiers, DistrictModifierType.EnergyConsumptionAwareness);
					}
				}
				if (!flag && !flag2 && bufferAccessor2.Length != 0)
				{
					bool flag3 = electricityConsumption > 0f;
					electricityConsumption *= FlowUtils.GetRenterConsumptionMultiplier(prefab, bufferAccessor2[i], ref m_HouseholdCitizens, ref m_Employees, ref m_Citizens, ref m_SpawnableDatas);
					electricityConsumption = math.select(electricityConsumption, 1f, flag3 && electricityConsumption < 1f);
				}
				else
				{
					electricityConsumption = math.select(electricityConsumption, 1f, electricityConsumption > 0f && electricityConsumption < 1f);
				}
				ref ElectricityConsumer reference = ref CollectionUtils.ElementAt<ElectricityConsumer>(nativeArray5, i);
				int num2 = ((electricityConsumption > 0f) ? MathUtils.RoundToIntRandom(ref random, electricityConsumption) : 0);
				if (BuildingUtils.CheckOption(nativeArray2[i], BuildingOption.Inactive))
				{
					num2 /= 10;
				}
				if (num2 != reference.m_WantedConsumption)
				{
					reference.m_WantedConsumption = num2;
					if (nativeArray2[i].m_RoadEdge != Entity.Null)
					{
						m_UpdatedEdges.Enqueue(nativeArray2[i].m_RoadEdge);
					}
					if (nativeArray4.Length != 0)
					{
						Entity consumerEdge = nativeArray4[i].m_ConsumerEdge;
						if (consumerEdge != Entity.Null)
						{
							ElectricityFlowEdge electricityFlowEdge = m_FlowEdges[consumerEdge];
							electricityFlowEdge.m_Capacity = num2;
							m_FlowEdges[consumerEdge] = electricityFlowEdge;
						}
					}
				}
				if (bufferAccessor3.Length != 0)
				{
					BuildingUtils.SetEfficiencyFactor(bufferAccessor3[i], EfficiencyFactor.ElectricityFee, efficiency);
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct UpdateEdgesJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<ElectricityNodeConnection> m_NodeConnections;

		[ReadOnly]
		public ComponentLookup<ElectricityConsumer> m_Consumers;

		[ReadOnly]
		public ComponentLookup<ElectricityBuildingConnection> m_BuildingConnections;

		[ReadOnly]
		public BufferLookup<ConnectedBuilding> m_ConnectedBuildings;

		public NativeQueue<Entity> m_UpdatedEdges;

		[ReadOnly]
		public BufferLookup<ConnectedFlowEdge> m_FlowConnections;

		public ComponentLookup<ElectricityFlowEdge> m_FlowEdges;

		public Entity m_SinkNode;

		public void Execute()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			NativeParallelHashSet<Entity> val = default(NativeParallelHashSet<Entity>);
			val._002Ector(m_UpdatedEdges.Count, AllocatorHandle.op_Implicit((Allocator)2));
			Entity val2 = default(Entity);
			ElectricityNodeConnection electricityNodeConnection = default(ElectricityNodeConnection);
			while (m_UpdatedEdges.TryDequeue(ref val2))
			{
				if (val.Add(val2) && m_NodeConnections.TryGetComponent(val2, ref electricityNodeConnection) && ElectricityGraphUtils.TryGetFlowEdge(electricityNodeConnection.m_ElectricityNode, m_SinkNode, ref m_FlowConnections, ref m_FlowEdges, out var entity, out var edge))
				{
					edge.m_Capacity = GetNonConnectedBuildingConsumption(val2);
					m_FlowEdges[entity] = edge;
				}
			}
		}

		private int GetNonConnectedBuildingConsumption(Entity roadEdge)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			DynamicBuffer<ConnectedBuilding> val = default(DynamicBuffer<ConnectedBuilding>);
			if (m_ConnectedBuildings.TryGetBuffer(roadEdge, ref val))
			{
				Enumerator<ConnectedBuilding> enumerator = val.GetEnumerator();
				try
				{
					ElectricityConsumer electricityConsumer = default(ElectricityConsumer);
					while (enumerator.MoveNext())
					{
						ConnectedBuilding current = enumerator.Current;
						if (m_Consumers.TryGetComponent(current.m_Building, ref electricityConsumer) && !m_BuildingConnections.HasComponent(current.m_Building))
						{
							num += electricityConsumer.m_WantedConsumption;
						}
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
				}
			}
			return num;
		}
	}

	private struct TypeHandle
	{
		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Building> __Game_Buildings_Building_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CityServiceUpkeep> __Game_City_CityServiceUpkeep_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ElectricityBuildingConnection> __Game_Simulation_ElectricityBuildingConnection_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentDistrict> __Game_Areas_CurrentDistrict_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Renter> __Game_Buildings_Renter_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.Park> __Game_Buildings_Park_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<StorageProperty> __Game_Buildings_StorageProperty_RO_ComponentTypeHandle;

		public ComponentTypeHandle<ElectricityConsumer> __Game_Buildings_ElectricityConsumer_RW_ComponentTypeHandle;

		public BufferTypeHandle<Efficiency> __Game_Buildings_Efficiency_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ConsumptionData> __Game_Prefabs_ConsumptionData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ServiceFee> __Game_City_ServiceFee_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Employee> __Game_Companies_Employee_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<DistrictModifier> __Game_Areas_DistrictModifier_RO_BufferLookup;

		public ComponentLookup<ElectricityFlowEdge> __Game_Simulation_ElectricityFlowEdge_RW_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityNodeConnection> __Game_Simulation_ElectricityNodeConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityBuildingConnection> __Game_Simulation_ElectricityBuildingConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityConsumer> __Game_Buildings_ElectricityConsumer_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedBuilding> __Game_Buildings_ConnectedBuilding_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedFlowEdge> __Game_Simulation_ConnectedFlowEdge_RO_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Buildings_Building_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Building>(true);
			__Game_City_CityServiceUpkeep_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CityServiceUpkeep>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<InstalledUpgrade>(true);
			__Game_Simulation_ElectricityBuildingConnection_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ElectricityBuildingConnection>(true);
			__Game_Areas_CurrentDistrict_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentDistrict>(true);
			__Game_Buildings_Renter_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Renter>(true);
			__Game_Buildings_Park_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.Park>(true);
			__Game_Buildings_StorageProperty_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<StorageProperty>(true);
			__Game_Buildings_ElectricityConsumer_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ElectricityConsumer>(false);
			__Game_Buildings_Efficiency_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Efficiency>(false);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ConsumptionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ConsumptionData>(true);
			__Game_City_ServiceFee_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceFee>(true);
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
			__Game_Companies_Employee_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Employee>(true);
			__Game_Citizens_HouseholdCitizen_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdCitizen>(true);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableBuildingData>(true);
			__Game_Areas_DistrictModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<DistrictModifier>(true);
			__Game_Simulation_ElectricityFlowEdge_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityFlowEdge>(false);
			__Game_Simulation_ElectricityNodeConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityNodeConnection>(true);
			__Game_Simulation_ElectricityBuildingConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityBuildingConnection>(true);
			__Game_Buildings_ElectricityConsumer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityConsumer>(true);
			__Game_Buildings_ConnectedBuilding_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedBuilding>(true);
			__Game_Simulation_ConnectedFlowEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedFlowEdge>(true);
		}
	}

	private const int kFullUpdatesPerDay = 128;

	private ClimateSystem m_ClimateSystem;

	private SimulationSystem m_SimulationSystem;

	private CitySystem m_CitySystem;

	private ElectricityFlowSystem m_ElectricityFlowSystem;

	private EntityQuery m_ConsumerQuery;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_653552652_0;

	private EntityQuery __query_653552652_1;

	private EntityQuery __query_653552652_2;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 128;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 0;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		Assert.IsTrue(GetUpdateInterval(SystemUpdatePhase.GameSimulation) >= 128);
		m_ClimateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ClimateSystem>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_ElectricityFlowSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ElectricityFlowSystem>();
		m_ConsumerQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadWrite<ElectricityConsumer>(),
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_ConsumerQuery);
		((ComponentSystemBase)this).RequireForUpdate<ServiceFeeParameterData>();
		((ComponentSystemBase)this).RequireForUpdate<BuildingEfficiencyParameterData>();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		NativeQueue<Entity> updatedEdges = default(NativeQueue<Entity>);
		updatedEdges._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		uint updateFrame = SimulationUtils.GetUpdateFrame(m_SimulationSystem.frameIndex, 128, 16);
		JobHandle val = JobChunkExtensions.ScheduleParallel<AdjustElectricityConsumptionJob>(new AdjustElectricityConsumptionJob
		{
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingType = InternalCompilerInterface.GetComponentTypeHandle<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CityServiceType = InternalCompilerInterface.GetComponentTypeHandle<CityServiceUpkeep>(ref __TypeHandle.__Game_City_CityServiceUpkeep_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpgradeType = InternalCompilerInterface.GetBufferTypeHandle<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingConnectionType = InternalCompilerInterface.GetComponentTypeHandle<ElectricityBuildingConnection>(ref __TypeHandle.__Game_Simulation_ElectricityBuildingConnection_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentDistrictType = InternalCompilerInterface.GetComponentTypeHandle<CurrentDistrict>(ref __TypeHandle.__Game_Areas_CurrentDistrict_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RenterType = InternalCompilerInterface.GetBufferTypeHandle<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ParkType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.Park>(ref __TypeHandle.__Game_Buildings_Park_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_StoragePropertyType = InternalCompilerInterface.GetComponentTypeHandle<StorageProperty>(ref __TypeHandle.__Game_Buildings_StorageProperty_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ConsumerType = InternalCompilerInterface.GetComponentTypeHandle<ElectricityConsumer>(ref __TypeHandle.__Game_Buildings_ElectricityConsumer_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EfficiencyType = InternalCompilerInterface.GetBufferTypeHandle<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceConsumption = InternalCompilerInterface.GetComponentLookup<ConsumptionData>(ref __TypeHandle.__Game_Prefabs_ConsumptionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Fees = InternalCompilerInterface.GetBufferLookup<ServiceFee>(ref __TypeHandle.__Game_City_ServiceFee_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Citizens = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Employees = InternalCompilerInterface.GetBufferLookup<Employee>(ref __TypeHandle.__Game_Companies_Employee_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdCitizens = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnableDatas = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DistrictModifiers = InternalCompilerInterface.GetBufferLookup<DistrictModifier>(ref __TypeHandle.__Game_Areas_DistrictModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FlowEdges = InternalCompilerInterface.GetComponentLookup<ElectricityFlowEdge>(ref __TypeHandle.__Game_Simulation_ElectricityFlowEdge_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UpdatedEdges = updatedEdges.AsParallelWriter(),
			m_FeeParameters = ((EntityQuery)(ref __query_653552652_0)).GetSingleton<ServiceFeeParameterData>(),
			m_EfficiencyParameters = ((EntityQuery)(ref __query_653552652_1)).GetSingleton<BuildingEfficiencyParameterData>(),
			m_RandomSeed = RandomSeed.Next(),
			m_City = m_CitySystem.City,
			m_TemperatureMultiplier = GetTemperatureMultiplier(m_ClimateSystem.temperature),
			m_UpdateFrameIndex = updateFrame
		}, m_ConsumerQuery, ((SystemBase)this).Dependency);
		UpdateEdgesJob updateEdgesJob = new UpdateEdgesJob
		{
			m_NodeConnections = InternalCompilerInterface.GetComponentLookup<ElectricityNodeConnection>(ref __TypeHandle.__Game_Simulation_ElectricityNodeConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingConnections = InternalCompilerInterface.GetComponentLookup<ElectricityBuildingConnection>(ref __TypeHandle.__Game_Simulation_ElectricityBuildingConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Consumers = InternalCompilerInterface.GetComponentLookup<ElectricityConsumer>(ref __TypeHandle.__Game_Buildings_ElectricityConsumer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedBuildings = InternalCompilerInterface.GetBufferLookup<ConnectedBuilding>(ref __TypeHandle.__Game_Buildings_ConnectedBuilding_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FlowConnections = InternalCompilerInterface.GetBufferLookup<ConnectedFlowEdge>(ref __TypeHandle.__Game_Simulation_ConnectedFlowEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FlowEdges = InternalCompilerInterface.GetComponentLookup<ElectricityFlowEdge>(ref __TypeHandle.__Game_Simulation_ElectricityFlowEdge_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UpdatedEdges = updatedEdges,
			m_SinkNode = m_ElectricityFlowSystem.sinkNode
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<UpdateEdgesJob>(updateEdgesJob, val);
		updatedEdges.Dispose(((SystemBase)this).Dependency);
	}

	public float GetTemperatureMultiplier(float temperature)
	{
		ElectricityParameterData electricityParameterData = default(ElectricityParameterData);
		if (!((EntityQuery)(ref __query_653552652_2)).TryGetSingleton<ElectricityParameterData>(ref electricityParameterData))
		{
			return 1f;
		}
		return ((AnimationCurve1)(ref electricityParameterData.m_TemperatureConsumptionMultiplier)).Evaluate(temperature);
	}

	public static float GetFeeConsumptionMultiplier(float relativeFee, in ServiceFeeParameterData feeParameters)
	{
		return ((AnimationCurve1)(ref feeParameters.m_ElectricityFeeConsumptionMultiplier)).Evaluate(relativeFee);
	}

	public static float GetFeeEfficiencyFactor(float relativeFee, in BuildingEfficiencyParameterData efficiencyParameters)
	{
		return ((AnimationCurve1)(ref efficiencyParameters.m_ElectricityFeeFactor)).Evaluate(relativeFee);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<ServiceFeeParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_653552652_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		val2 = ((EntityQueryBuilder)(ref val)).WithAll<BuildingEfficiencyParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_653552652_1 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		val2 = ((EntityQueryBuilder)(ref val)).WithAll<ElectricityParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_653552652_2 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public AdjustElectricityConsumptionSystem()
	{
	}
}
