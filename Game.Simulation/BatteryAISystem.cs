using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Buildings;
using Game.Common;
using Game.Notifications;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class BatteryAISystem : GameSystemBase
{
	[BurstCompile]
	private struct BatteryTickJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		[ReadOnly]
		public ComponentTypeHandle<ElectricityBuildingConnection> m_BuildingConnectionType;

		[ReadOnly]
		public BufferTypeHandle<Efficiency> m_EfficiencyType;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> m_InstalledUpgradeType;

		public ComponentTypeHandle<Game.Buildings.Battery> m_BatteryType;

		public ComponentTypeHandle<Game.Buildings.EmergencyGenerator> m_EmergencyGeneratorType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<BatteryData> m_BatteryDatas;

		[ReadOnly]
		public ComponentLookup<EmergencyGeneratorData> m_EmergencyGeneratorDatas;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.ResourceConsumer> m_ResourceConsumers;

		[NativeDisableContainerSafetyRestriction]
		public ComponentLookup<ServiceUsage> m_ServiceUsages;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<ElectricityFlowEdge> m_FlowEdges;

		public IconCommandBuffer m_IconCommandBuffer;

		public ElectricityParameterData m_ElectricityParameterData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_049c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_0343: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_040d: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabType);
			NativeArray<Game.Buildings.Battery> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.Battery>(ref m_BatteryType);
			NativeArray<Game.Buildings.EmergencyGenerator> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.EmergencyGenerator>(ref m_EmergencyGeneratorType);
			NativeArray<ElectricityBuildingConnection> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ElectricityBuildingConnection>(ref m_BuildingConnectionType);
			BufferAccessor<Efficiency> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Efficiency>(ref m_EfficiencyType);
			BufferAccessor<InstalledUpgrade> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<InstalledUpgrade>(ref m_InstalledUpgradeType);
			BatteryData data = default(BatteryData);
			EmergencyGeneratorData emergencyGeneratorData = default(EmergencyGeneratorData);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity owner = nativeArray[i];
				Entity prefab = nativeArray2[i].m_Prefab;
				ref Game.Buildings.Battery reference = ref CollectionUtils.ElementAt<Game.Buildings.Battery>(nativeArray3, i);
				ElectricityBuildingConnection electricityBuildingConnection = nativeArray5[i];
				float efficiency = BuildingUtils.GetEfficiency(bufferAccessor, i);
				if (electricityBuildingConnection.m_ChargeEdge == Entity.Null || electricityBuildingConnection.m_DischargeEdge == Entity.Null)
				{
					Debug.LogError((object)"Battery is missing charge or discharge edge!");
					continue;
				}
				m_BatteryDatas.TryGetComponent(prefab, ref data);
				if (bufferAccessor2.Length != 0)
				{
					UpgradeUtils.CombineStats<BatteryData>(ref data, bufferAccessor2[i], ref m_Prefabs, ref m_BatteryDatas);
				}
				bool flag = reference.m_StoredEnergy == 0;
				ElectricityFlowEdge electricityFlowEdge = m_FlowEdges[electricityBuildingConnection.m_DischargeEdge];
				ElectricityFlowEdge electricityFlowEdge2 = m_FlowEdges[electricityBuildingConnection.m_ChargeEdge];
				int num = electricityFlowEdge2.m_Flow - electricityFlowEdge.m_Flow;
				reference.m_StoredEnergy = math.clamp(reference.m_StoredEnergy + num, 0L, data.capacityTicks);
				reference.m_Capacity = data.m_Capacity;
				reference.m_LastFlow = num;
				bool flag2 = reference.m_StoredEnergy == 0;
				if (flag2 && !flag)
				{
					m_IconCommandBuffer.Add(owner, m_ElectricityParameterData.m_BatteryEmptyNotificationPrefab, IconPriority.Problem);
				}
				else if (!flag2 && flag)
				{
					m_IconCommandBuffer.Remove(owner, m_ElectricityParameterData.m_BatteryEmptyNotificationPrefab);
				}
				if (nativeArray4.Length != 0)
				{
					Bounds1 val = default(Bounds1);
					int num2 = 0;
					int num3 = 0;
					if (bufferAccessor2.Length != 0)
					{
						Enumerator<InstalledUpgrade> enumerator = bufferAccessor2[i].GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								InstalledUpgrade current = enumerator.Current;
								if (!BuildingUtils.CheckOption(current, BuildingOption.Inactive) && m_EmergencyGeneratorDatas.TryGetComponent((Entity)m_Prefabs[(Entity)current], ref emergencyGeneratorData))
								{
									val = new Bounds1(math.max(val.min, emergencyGeneratorData.m_ActivationThreshold.min), math.max(val.max, emergencyGeneratorData.m_ActivationThreshold.max));
									if (HasResources(current))
									{
										num2 += Mathf.CeilToInt(efficiency * (float)emergencyGeneratorData.m_ElectricityProduction);
										num3 += emergencyGeneratorData.m_ElectricityProduction;
									}
								}
							}
						}
						finally
						{
							((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
						}
					}
					ref Game.Buildings.EmergencyGenerator reference2 = ref CollectionUtils.ElementAt<Game.Buildings.EmergencyGenerator>(nativeArray4, i);
					float num4 = (float)reference.m_StoredEnergy / (float)math.max(1L, data.capacityTicks);
					bool flag3 = reference2.m_Production > 0;
					bool flag4 = efficiency > 0f && (num4 < val.min || (flag3 && num4 < val.max));
					reference2.m_Production = (flag4 ? math.min(num2, (int)(data.capacityTicks - reference.m_StoredEnergy)) : 0);
					float num5 = ((num3 > 0) ? ((float)reference2.m_Production / (float)num3) : 0f);
					if (bufferAccessor2.Length != 0)
					{
						Enumerator<InstalledUpgrade> enumerator = bufferAccessor2[i].GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								InstalledUpgrade current2 = enumerator.Current;
								if (!BuildingUtils.CheckOption(current2, BuildingOption.Inactive) && m_EmergencyGeneratorDatas.HasComponent((Entity)current2) && m_ServiceUsages.HasComponent((Entity)current2))
								{
									m_ServiceUsages[(Entity)current2] = new ServiceUsage
									{
										m_Usage = (HasResources(current2) ? num5 : 0f)
									};
								}
							}
						}
						finally
						{
							((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
						}
					}
					Assert.IsTrue(reference2.m_Production >= 0);
					reference.m_StoredEnergy += reference2.m_Production;
				}
				electricityFlowEdge.m_Capacity = (int)((efficiency > 0f) ? math.min((long)data.m_PowerOutput, reference.m_StoredEnergy) : 0);
				m_FlowEdges[electricityBuildingConnection.m_DischargeEdge] = electricityFlowEdge;
				electricityFlowEdge2.m_Capacity = (int)math.min((long)Mathf.RoundToInt(efficiency * (float)data.m_PowerOutput), data.capacityTicks - reference.m_StoredEnergy);
				m_FlowEdges[electricityBuildingConnection.m_ChargeEdge] = electricityFlowEdge2;
			}
		}

		private bool HasResources(Entity upgrade)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			Game.Buildings.ResourceConsumer resourceConsumer = default(Game.Buildings.ResourceConsumer);
			if (m_ResourceConsumers.TryGetComponent(upgrade, ref resourceConsumer))
			{
				return resourceConsumer.m_ResourceAvailability > 0;
			}
			return true;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ElectricityBuildingConnection> __Game_Simulation_ElectricityBuildingConnection_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Efficiency> __Game_Buildings_Efficiency_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle;

		public ComponentTypeHandle<Game.Buildings.Battery> __Game_Buildings_Battery_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Game.Buildings.EmergencyGenerator> __Game_Buildings_EmergencyGenerator_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BatteryData> __Game_Prefabs_BatteryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EmergencyGeneratorData> __Game_Prefabs_EmergencyGeneratorData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.ResourceConsumer> __Game_Buildings_ResourceConsumer_RO_ComponentLookup;

		public ComponentLookup<ServiceUsage> __Game_Buildings_ServiceUsage_RW_ComponentLookup;

		public ComponentLookup<ElectricityFlowEdge> __Game_Simulation_ElectricityFlowEdge_RW_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Simulation_ElectricityBuildingConnection_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ElectricityBuildingConnection>(true);
			__Game_Buildings_Efficiency_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Efficiency>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<InstalledUpgrade>(true);
			__Game_Buildings_Battery_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.Battery>(false);
			__Game_Buildings_EmergencyGenerator_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.EmergencyGenerator>(false);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_BatteryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BatteryData>(true);
			__Game_Prefabs_EmergencyGeneratorData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EmergencyGeneratorData>(true);
			__Game_Buildings_ResourceConsumer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.ResourceConsumer>(true);
			__Game_Buildings_ServiceUsage_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceUsage>(false);
			__Game_Simulation_ElectricityFlowEdge_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityFlowEdge>(false);
		}
	}

	private EntityQuery m_BatteryQuery;

	private EntityQuery m_SettingsQuery;

	private IconCommandSystem m_IconCommandSystem;

	private TypeHandle __TypeHandle;

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
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_IconCommandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IconCommandSystem>();
		m_BatteryQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Game.Buildings.Battery>(),
			ComponentType.ReadOnly<ElectricityBuildingConnection>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_SettingsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ElectricityParameterData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_BatteryQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		BatteryTickJob batteryTickJob = new BatteryTickJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingConnectionType = InternalCompilerInterface.GetComponentTypeHandle<ElectricityBuildingConnection>(ref __TypeHandle.__Game_Simulation_ElectricityBuildingConnection_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EfficiencyType = InternalCompilerInterface.GetBufferTypeHandle<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgradeType = InternalCompilerInterface.GetBufferTypeHandle<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BatteryType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.Battery>(ref __TypeHandle.__Game_Buildings_Battery_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EmergencyGeneratorType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.EmergencyGenerator>(ref __TypeHandle.__Game_Buildings_EmergencyGenerator_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BatteryDatas = InternalCompilerInterface.GetComponentLookup<BatteryData>(ref __TypeHandle.__Game_Prefabs_BatteryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EmergencyGeneratorDatas = InternalCompilerInterface.GetComponentLookup<EmergencyGeneratorData>(ref __TypeHandle.__Game_Prefabs_EmergencyGeneratorData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceConsumers = InternalCompilerInterface.GetComponentLookup<Game.Buildings.ResourceConsumer>(ref __TypeHandle.__Game_Buildings_ResourceConsumer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceUsages = InternalCompilerInterface.GetComponentLookup<ServiceUsage>(ref __TypeHandle.__Game_Buildings_ServiceUsage_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FlowEdges = InternalCompilerInterface.GetComponentLookup<ElectricityFlowEdge>(ref __TypeHandle.__Game_Simulation_ElectricityFlowEdge_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_IconCommandBuffer = m_IconCommandSystem.CreateCommandBuffer(),
			m_ElectricityParameterData = ((EntityQuery)(ref m_SettingsQuery)).GetSingleton<ElectricityParameterData>()
		};
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<BatteryTickJob>(batteryTickJob, m_BatteryQuery, ((SystemBase)this).Dependency);
		m_IconCommandSystem.AddCommandBufferWriter(((SystemBase)this).Dependency);
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
	public BatteryAISystem()
	{
	}
}
