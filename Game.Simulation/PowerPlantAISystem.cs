using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Buildings;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class PowerPlantAISystem : GameSystemBase
{
	[BurstCompile]
	private struct PowerPlantTickJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.GarbageFacility> m_GarbageFacilityType;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> m_InstalledUpgradeType;

		[ReadOnly]
		public ComponentTypeHandle<ElectricityBuildingConnection> m_BuildingConnectionType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.ResourceConsumer> m_ResourceConsumerType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.WaterPowered> m_WaterPoweredType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public BufferTypeHandle<Game.Net.SubNet> m_SubNetType;

		public ComponentTypeHandle<ElectricityProducer> m_ElectricityProducerType;

		public BufferTypeHandle<Efficiency> m_EfficiencyType;

		public ComponentTypeHandle<ServiceUsage> m_ServiceUsageType;

		public ComponentTypeHandle<PointOfInterest> m_PointOfInterestType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<PowerPlantData> m_PowerPlantDatas;

		[ReadOnly]
		public ComponentLookup<GarbagePoweredData> m_GarbagePoweredData;

		[ReadOnly]
		public ComponentLookup<WindPoweredData> m_WindPoweredData;

		[ReadOnly]
		public ComponentLookup<WaterPoweredData> m_WaterPoweredData;

		[ReadOnly]
		public ComponentLookup<SolarPoweredData> m_SolarPoweredData;

		[ReadOnly]
		public ComponentLookup<GroundWaterPoweredData> m_GroundWaterPoweredData;

		[ReadOnly]
		public ComponentLookup<PlaceableNetData> m_PlaceableNetData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_NetCompositionData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.ResourceConsumer> m_ResourceConsumers;

		[ReadOnly]
		public ComponentLookup<Curve> m_Curves;

		[ReadOnly]
		public ComponentLookup<Composition> m_Compositions;

		[NativeDisableContainerSafetyRestriction]
		public ComponentLookup<ServiceUsage> m_ServiceUsages;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<ElectricityFlowEdge> m_FlowEdges;

		[ReadOnly]
		public NativeArray<Wind> m_WindMap;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[ReadOnly]
		public WaterSurfaceData m_WaterSurfaceData;

		[ReadOnly]
		public NativeArray<GroundWater> m_GroundWaterMap;

		public float m_SunLight;

		public unsafe void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
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
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Unknown result type (might be due to invalid IL or missing references)
			//IL_0436: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_0470: Unknown result type (might be due to invalid IL or missing references)
			//IL_048d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_056f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0574: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_050a: Unknown result type (might be due to invalid IL or missing references)
			//IL_050f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0511: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_052a: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0612: Unknown result type (might be due to invalid IL or missing references)
			//IL_0614: Unknown result type (might be due to invalid IL or missing references)
			//IL_0616: Unknown result type (might be due to invalid IL or missing references)
			//IL_061b: Unknown result type (might be due to invalid IL or missing references)
			//IL_061d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0622: Unknown result type (might be due to invalid IL or missing references)
			//IL_0624: Unknown result type (might be due to invalid IL or missing references)
			//IL_0629: Unknown result type (might be due to invalid IL or missing references)
			//IL_062b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0630: Unknown result type (might be due to invalid IL or missing references)
			//IL_0635: Unknown result type (might be due to invalid IL or missing references)
			//IL_063b: Unknown result type (might be due to invalid IL or missing references)
			//IL_065a: Unknown result type (might be due to invalid IL or missing references)
			//IL_060b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0610: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0596: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05af: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0536: Unknown result type (might be due to invalid IL or missing references)
			//IL_0542: Unknown result type (might be due to invalid IL or missing references)
			//IL_0547: Unknown result type (might be due to invalid IL or missing references)
			//IL_0553: Unknown result type (might be due to invalid IL or missing references)
			//IL_055a: Unknown result type (might be due to invalid IL or missing references)
			//IL_055f: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0672: Unknown result type (might be due to invalid IL or missing references)
			//IL_038a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0726: Unknown result type (might be due to invalid IL or missing references)
			//IL_072b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0683: Unknown result type (might be due to invalid IL or missing references)
			//IL_068a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0694: Unknown result type (might be due to invalid IL or missing references)
			//IL_069b: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0709: Unknown result type (might be due to invalid IL or missing references)
			//IL_071a: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabType);
			NativeArray<Game.Buildings.GarbageFacility> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.GarbageFacility>(ref m_GarbageFacilityType);
			BufferAccessor<InstalledUpgrade> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<InstalledUpgrade>(ref m_InstalledUpgradeType);
			NativeArray<ElectricityBuildingConnection> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ElectricityBuildingConnection>(ref m_BuildingConnectionType);
			NativeArray<ElectricityProducer> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ElectricityProducer>(ref m_ElectricityProducerType);
			NativeArray<Game.Buildings.WaterPowered> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.WaterPowered>(ref m_WaterPoweredType);
			NativeArray<Transform> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			BufferAccessor<Game.Net.SubNet> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Game.Net.SubNet>(ref m_SubNetType);
			NativeArray<Game.Buildings.ResourceConsumer> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.ResourceConsumer>(ref m_ResourceConsumerType);
			BufferAccessor<Efficiency> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Efficiency>(ref m_EfficiencyType);
			NativeArray<ServiceUsage> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ServiceUsage>(ref m_ServiceUsageType);
			NativeArray<PointOfInterest> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PointOfInterest>(ref m_PointOfInterestType);
			Span<float> factors = new Span<float>((void*)stackalloc byte[120], 30);
			Game.Buildings.ResourceConsumer resourceConsumer = default(Game.Buildings.ResourceConsumer);
			PowerPlantData powerPlantData = default(PowerPlantData);
			Game.Buildings.ResourceConsumer resourceConsumer2 = default(Game.Buildings.ResourceConsumer);
			GarbagePoweredData data = default(GarbagePoweredData);
			WindPoweredData data2 = default(WindPoweredData);
			WaterPoweredData data3 = default(WaterPoweredData);
			SolarPoweredData data4 = default(SolarPoweredData);
			GroundWaterPoweredData data5 = default(GroundWaterPoweredData);
			float4 weights = default(float4);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity prefab = nativeArray[i].m_Prefab;
				ref ElectricityProducer reference = ref CollectionUtils.ElementAt<ElectricityProducer>(nativeArray4, i);
				ElectricityBuildingConnection electricityBuildingConnection = nativeArray3[i];
				byte b = ((nativeArray7.Length != 0) ? nativeArray7[i].m_ResourceAvailability : byte.MaxValue);
				Transform transform = nativeArray6[i];
				if (bufferAccessor3.Length != 0)
				{
					BuildingUtils.GetEfficiencyFactors(bufferAccessor3[i], factors);
					factors[17] = 1f;
					factors[18] = 1f;
					factors[19] = 1f;
					factors[20] = 1f;
				}
				else
				{
					factors.Fill(1f);
				}
				float efficiency = BuildingUtils.GetEfficiency(factors);
				if (electricityBuildingConnection.m_ProducerEdge == Entity.Null)
				{
					Debug.LogError((object)"PowerPlant is missing producer edge!");
					continue;
				}
				ElectricityFlowEdge electricityFlowEdge = m_FlowEdges[electricityBuildingConnection.m_ProducerEdge];
				reference.m_LastProduction = electricityFlowEdge.m_Flow;
				float num = ((reference.m_Capacity > 0) ? ((float)reference.m_LastProduction / (float)reference.m_Capacity) : 0f);
				if (nativeArray8.Length != 0)
				{
					nativeArray8[i] = new ServiceUsage
					{
						m_Usage = ((b > 0) ? num : 0f)
					};
				}
				if (bufferAccessor.Length != 0)
				{
					Enumerator<InstalledUpgrade> enumerator = bufferAccessor[i].GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							InstalledUpgrade current = enumerator.Current;
							if (!BuildingUtils.CheckOption(current, BuildingOption.Inactive) && m_PowerPlantDatas.HasComponent((Entity)current) && m_ServiceUsages.HasComponent((Entity)current))
							{
								byte b2 = (m_ResourceConsumers.TryGetComponent(current.m_Upgrade, ref resourceConsumer) ? resourceConsumer.m_ResourceAvailability : b);
								m_ServiceUsages[(Entity)current] = new ServiceUsage
								{
									m_Usage = ((b2 > 0) ? num : 0f)
								};
							}
						}
					}
					finally
					{
						((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
					}
				}
				float2 val = float2.zero;
				if (m_PowerPlantDatas.TryGetComponent(prefab, ref powerPlantData))
				{
					val += GetPowerPlantProduction(powerPlantData, b, efficiency);
				}
				if (bufferAccessor.Length != 0)
				{
					Enumerator<InstalledUpgrade> enumerator = bufferAccessor[i].GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							InstalledUpgrade current2 = enumerator.Current;
							if (!BuildingUtils.CheckOption(current2, BuildingOption.Inactive) && m_PowerPlantDatas.TryGetComponent((Entity)m_Prefabs[current2.m_Upgrade], ref powerPlantData))
							{
								byte resourceAvailability = (m_ResourceConsumers.TryGetComponent(current2.m_Upgrade, ref resourceConsumer2) ? resourceConsumer2.m_ResourceAvailability : b);
								val += GetPowerPlantProduction(powerPlantData, resourceAvailability, efficiency);
							}
						}
					}
					finally
					{
						((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
					}
				}
				m_GarbagePoweredData.TryGetComponent(prefab, ref data);
				m_WindPoweredData.TryGetComponent(prefab, ref data2);
				m_WaterPoweredData.TryGetComponent(prefab, ref data3);
				m_SolarPoweredData.TryGetComponent(prefab, ref data4);
				m_GroundWaterPoweredData.TryGetComponent(prefab, ref data5);
				if (bufferAccessor.Length != 0)
				{
					UpgradeUtils.CombineStats<GarbagePoweredData>(ref data, bufferAccessor[i], ref m_Prefabs, ref m_GarbagePoweredData);
					UpgradeUtils.CombineStats<WindPoweredData>(ref data2, bufferAccessor[i], ref m_Prefabs, ref m_WindPoweredData);
					UpgradeUtils.CombineStats<WaterPoweredData>(ref data3, bufferAccessor[i], ref m_Prefabs, ref m_WaterPoweredData);
					UpgradeUtils.CombineStats<SolarPoweredData>(ref data4, bufferAccessor[i], ref m_Prefabs, ref m_SolarPoweredData);
					UpgradeUtils.CombineStats<GroundWaterPoweredData>(ref data5, bufferAccessor[i], ref m_Prefabs, ref m_GroundWaterPoweredData);
				}
				float2 val2 = float2.zero;
				if (data.m_Capacity > 0 && nativeArray2.Length != 0)
				{
					val2 = float2.op_Implicit(GetGarbageProduction(data, nativeArray2[i]));
				}
				float2 val3 = float2.zero;
				if (data2.m_Production > 0)
				{
					Wind wind = WindSystem.GetWind(nativeArray6[i].m_Position, m_WindMap);
					val3 = GetWindProduction(data2, wind, efficiency);
					if (val3.x > 0f && nativeArray9.Length != 0 && math.any(wind.m_Wind))
					{
						ref PointOfInterest reference2 = ref CollectionUtils.ElementAt<PointOfInterest>(nativeArray9, i);
						reference2.m_Position = transform.m_Position;
						ref float3 position = ref reference2.m_Position;
						((float3)(ref position)).xz = ((float3)(ref position)).xz - wind.m_Wind;
						reference2.m_IsValid = true;
					}
				}
				float2 val4 = float2.zero;
				if (nativeArray5.Length != 0 && bufferAccessor2.Length != 0 && data3.m_ProductionFactor > 0f)
				{
					val4 += GetWaterProduction(data3, nativeArray5[i], bufferAccessor2[i], efficiency);
				}
				if (data5.m_Production > 0 && data5.m_MaximumGroundWater > 0)
				{
					val4 += GetGroundWaterProduction(data5, nativeArray6[i].m_Position, efficiency, m_GroundWaterMap);
				}
				float2 val5 = float2.zero;
				if (data4.m_Production > 0)
				{
					val5 = GetSolarProduction(data4, efficiency);
				}
				float2 val6 = math.round(val + val2 + val3 + val4 + val5);
				electricityFlowEdge.m_Capacity = (reference.m_Capacity = (int)val6.x);
				m_FlowEdges[electricityBuildingConnection.m_ProducerEdge] = electricityFlowEdge;
				if (bufferAccessor3.Length != 0)
				{
					if (val6.y > 0f)
					{
						float targetEfficiency = val6.x / val6.y;
						((float4)(ref weights))._002Ector(val.y - val.x, val3.y - val3.x, val4.y - val4.x, val5.y - val5.x);
						float4 val7 = BuildingUtils.ApproximateEfficiencyFactors(targetEfficiency, weights);
						factors[17] = val7.x;
						factors[18] = val7.y;
						factors[19] = val7.z;
						factors[20] = val7.w;
					}
					BuildingUtils.SetEfficiencyFactors(bufferAccessor3[i], factors);
				}
			}
		}

		private static float2 GetPowerPlantProduction(PowerPlantData powerPlantData, byte resourceAvailability, float efficiency)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			float num = efficiency * (float)powerPlantData.m_ElectricityProduction;
			return new float2((resourceAvailability > 0) ? num : 0f, num);
		}

		private static float GetGarbageProduction(GarbagePoweredData garbageData, Game.Buildings.GarbageFacility garbageFacility)
		{
			return math.clamp((float)garbageFacility.m_ProcessingRate / garbageData.m_ProductionPerUnit, 0f, (float)garbageData.m_Capacity);
		}

		private float2 GetWaterProduction(WaterPoweredData waterData, Game.Buildings.WaterPowered waterPowered, DynamicBuffer<Game.Net.SubNet> subNets, float efficiency)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			float num = 0f;
			Curve curve = default(Curve);
			Composition composition = default(Composition);
			PlaceableNetData placeableData = default(PlaceableNetData);
			NetCompositionData compositionData = default(NetCompositionData);
			for (int i = 0; i < subNets.Length; i++)
			{
				Entity subNet = subNets[i].m_SubNet;
				PrefabRef prefabRef = m_Prefabs[subNet];
				if (m_Curves.TryGetComponent(subNet, ref curve) && m_Compositions.TryGetComponent(subNet, ref composition) && m_PlaceableNetData.TryGetComponent(prefabRef.m_Prefab, ref placeableData) && m_NetCompositionData.TryGetComponent(composition.m_Edge, ref compositionData) && (placeableData.m_PlacementFlags & (Game.Net.PlacementFlags.FlowLeft | Game.Net.PlacementFlags.FlowRight)) != Game.Net.PlacementFlags.None && (compositionData.m_Flags.m_General & (CompositionFlags.General.Spillway | CompositionFlags.General.Front | CompositionFlags.General.Back)) == 0)
				{
					num += GetWaterProduction(waterData, curve, placeableData, compositionData, m_TerrainHeightData, m_WaterSurfaceData);
				}
			}
			float num2 = efficiency * GetWaterCapacity(waterPowered, waterData);
			return new float2(math.clamp(efficiency * num, 0f, num2), num2);
		}

		private float GetWaterProduction(WaterPoweredData waterData, Curve curve, PlaceableNetData placeableData, NetCompositionData compositionData, TerrainHeightData terrainHeightData, WaterSurfaceData waterSurfaceData)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			int num = math.max(1, (int)math.round(curve.m_Length * waterSurfaceData.scale.x));
			bool flag = (placeableData.m_PlacementFlags & Game.Net.PlacementFlags.FlowLeft) != 0;
			float num2 = 0f;
			for (int i = 0; i < num; i++)
			{
				float num3 = ((float)i + 0.5f) / (float)num;
				float3 val = MathUtils.Position(curve.m_Bezier, num3);
				float3 val2 = MathUtils.Tangent(curve.m_Bezier, num3);
				float2 val3 = math.normalizesafe(math.select(MathUtils.Right(((float3)(ref val2)).xz), MathUtils.Left(((float3)(ref val2)).xz), flag), default(float2));
				float3 val4 = val;
				float3 worldPosition = val;
				((float3)(ref val4)).xz = ((float3)(ref val4)).xz - val3 * (compositionData.m_Width * 0.5f);
				((float3)(ref worldPosition)).xz = ((float3)(ref worldPosition)).xz + val3 * (compositionData.m_Width * 0.5f);
				float waterDepth;
				float num4 = WaterUtils.SampleHeight(ref waterSurfaceData, ref terrainHeightData, val4, out waterDepth);
				float waterDepth2;
				float num5 = WaterUtils.SampleHeight(ref waterSurfaceData, ref terrainHeightData, worldPosition, out waterDepth2);
				float2 val5 = WaterUtils.SampleVelocity(ref waterSurfaceData, val4);
				float2 val6 = WaterUtils.SampleVelocity(ref waterSurfaceData, worldPosition);
				if (num4 > val4.y)
				{
					waterDepth = math.max(0f, waterDepth - (num4 - val4.y));
					num4 = val4.y;
				}
				num2 += (math.dot(val5, val3) * waterDepth + math.dot(val6, val3) * waterDepth2) * 0.5f * math.max(0f, num4 - num5);
			}
			return num2 * waterData.m_ProductionFactor * curve.m_Length / (float)num;
		}

		private float2 GetSolarProduction(SolarPoweredData solarData, float efficiency)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			float num = efficiency * (float)solarData.m_Production;
			return new float2(math.clamp(num * m_SunLight, 0f, num), num);
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.GarbageFacility> __Game_Buildings_GarbageFacility_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ElectricityBuildingConnection> __Game_Simulation_ElectricityBuildingConnection_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.ResourceConsumer> __Game_Buildings_ResourceConsumer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.WaterPowered> __Game_Buildings_WaterPowered_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Game.Net.SubNet> __Game_Net_SubNet_RO_BufferTypeHandle;

		public ComponentTypeHandle<ElectricityProducer> __Game_Buildings_ElectricityProducer_RW_ComponentTypeHandle;

		public BufferTypeHandle<Efficiency> __Game_Buildings_Efficiency_RW_BufferTypeHandle;

		public ComponentTypeHandle<ServiceUsage> __Game_Buildings_ServiceUsage_RW_ComponentTypeHandle;

		public ComponentTypeHandle<PointOfInterest> __Game_Common_PointOfInterest_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PowerPlantData> __Game_Prefabs_PowerPlantData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<GarbagePoweredData> __Game_Prefabs_GarbagePoweredData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WindPoweredData> __Game_Prefabs_WindPoweredData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WaterPoweredData> __Game_Prefabs_WaterPoweredData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SolarPoweredData> __Game_Prefabs_SolarPoweredData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<GroundWaterPoweredData> __Game_Prefabs_GroundWaterPoweredData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PlaceableNetData> __Game_Prefabs_PlaceableNetData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> __Game_Prefabs_NetCompositionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.ResourceConsumer> __Game_Buildings_ResourceConsumer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Composition> __Game_Net_Composition_RO_ComponentLookup;

		public ComponentLookup<ServiceUsage> __Game_Buildings_ServiceUsage_RW_ComponentLookup;

		public ComponentLookup<ElectricityFlowEdge> __Game_Simulation_ElectricityFlowEdge_RW_ComponentLookup;

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
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Buildings_GarbageFacility_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.GarbageFacility>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<InstalledUpgrade>(true);
			__Game_Simulation_ElectricityBuildingConnection_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ElectricityBuildingConnection>(true);
			__Game_Buildings_ResourceConsumer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.ResourceConsumer>(true);
			__Game_Buildings_WaterPowered_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.WaterPowered>(true);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Net_SubNet_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Game.Net.SubNet>(true);
			__Game_Buildings_ElectricityProducer_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ElectricityProducer>(false);
			__Game_Buildings_Efficiency_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Efficiency>(false);
			__Game_Buildings_ServiceUsage_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ServiceUsage>(false);
			__Game_Common_PointOfInterest_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PointOfInterest>(false);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_PowerPlantData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PowerPlantData>(true);
			__Game_Prefabs_GarbagePoweredData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarbagePoweredData>(true);
			__Game_Prefabs_WindPoweredData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WindPoweredData>(true);
			__Game_Prefabs_WaterPoweredData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterPoweredData>(true);
			__Game_Prefabs_SolarPoweredData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SolarPoweredData>(true);
			__Game_Prefabs_GroundWaterPoweredData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GroundWaterPoweredData>(true);
			__Game_Prefabs_PlaceableNetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceableNetData>(true);
			__Game_Prefabs_NetCompositionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionData>(true);
			__Game_Buildings_ResourceConsumer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.ResourceConsumer>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_Composition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Composition>(true);
			__Game_Buildings_ServiceUsage_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceUsage>(false);
			__Game_Simulation_ElectricityFlowEdge_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityFlowEdge>(false);
		}
	}

	public const int MAX_WATERPOWERED_SIZE = 1000000;

	private PlanetarySystem m_PlanetarySystem;

	private WindSystem m_WindSystem;

	private TerrainSystem m_TerrainSystem;

	private WaterSystem m_WaterSystem;

	private GroundWaterSystem m_GroundWaterSystem;

	private ClimateSystem m_ClimateSystem;

	private EntityQuery m_PowerPlantQuery;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_833752410_0;

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
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PlanetarySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PlanetarySystem>();
		m_WindSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WindSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_GroundWaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GroundWaterSystem>();
		m_ClimateSystem = ((ComponentSystemBase)this).World.GetExistingSystemManaged<ClimateSystem>();
		m_PowerPlantQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<ElectricityProducer>(),
			ComponentType.ReadOnly<ElectricityBuildingConnection>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<Transform>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_PowerPlantQuery);
		((ComponentSystemBase)this).RequireForUpdate<ElectricityParameterData>();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		ElectricityParameterData singleton = ((EntityQuery)(ref __query_833752410_0)).GetSingleton<ElectricityParameterData>();
		PlanetarySystem.LightData sunLight = m_PlanetarySystem.SunLight;
		float num = 0f;
		if (sunLight.isValid)
		{
			num = math.max(0f, 0f - sunLight.transform.forward.y) * sunLight.additionalData.intensity / 110000f;
		}
		num *= math.lerp(1f, 1f - singleton.m_CloudinessSolarPenalty, m_ClimateSystem.cloudiness.value);
		JobHandle dependencies;
		JobHandle deps;
		JobHandle dependencies2;
		PowerPlantTickJob powerPlantTickJob = new PowerPlantTickJob
		{
			m_PrefabType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageFacilityType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.GarbageFacility>(ref __TypeHandle.__Game_Buildings_GarbageFacility_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgradeType = InternalCompilerInterface.GetBufferTypeHandle<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingConnectionType = InternalCompilerInterface.GetComponentTypeHandle<ElectricityBuildingConnection>(ref __TypeHandle.__Game_Simulation_ElectricityBuildingConnection_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceConsumerType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.ResourceConsumer>(ref __TypeHandle.__Game_Buildings_ResourceConsumer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WaterPoweredType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.WaterPowered>(ref __TypeHandle.__Game_Buildings_WaterPowered_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubNetType = InternalCompilerInterface.GetBufferTypeHandle<Game.Net.SubNet>(ref __TypeHandle.__Game_Net_SubNet_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ElectricityProducerType = InternalCompilerInterface.GetComponentTypeHandle<ElectricityProducer>(ref __TypeHandle.__Game_Buildings_ElectricityProducer_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EfficiencyType = InternalCompilerInterface.GetBufferTypeHandle<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceUsageType = InternalCompilerInterface.GetComponentTypeHandle<ServiceUsage>(ref __TypeHandle.__Game_Buildings_ServiceUsage_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PointOfInterestType = InternalCompilerInterface.GetComponentTypeHandle<PointOfInterest>(ref __TypeHandle.__Game_Common_PointOfInterest_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PowerPlantDatas = InternalCompilerInterface.GetComponentLookup<PowerPlantData>(ref __TypeHandle.__Game_Prefabs_PowerPlantData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GarbagePoweredData = InternalCompilerInterface.GetComponentLookup<GarbagePoweredData>(ref __TypeHandle.__Game_Prefabs_GarbagePoweredData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WindPoweredData = InternalCompilerInterface.GetComponentLookup<WindPoweredData>(ref __TypeHandle.__Game_Prefabs_WindPoweredData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WaterPoweredData = InternalCompilerInterface.GetComponentLookup<WaterPoweredData>(ref __TypeHandle.__Game_Prefabs_WaterPoweredData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SolarPoweredData = InternalCompilerInterface.GetComponentLookup<SolarPoweredData>(ref __TypeHandle.__Game_Prefabs_SolarPoweredData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GroundWaterPoweredData = InternalCompilerInterface.GetComponentLookup<GroundWaterPoweredData>(ref __TypeHandle.__Game_Prefabs_GroundWaterPoweredData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PlaceableNetData = InternalCompilerInterface.GetComponentLookup<PlaceableNetData>(ref __TypeHandle.__Game_Prefabs_PlaceableNetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceConsumers = InternalCompilerInterface.GetComponentLookup<Game.Buildings.ResourceConsumer>(ref __TypeHandle.__Game_Buildings_ResourceConsumer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Curves = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Compositions = InternalCompilerInterface.GetComponentLookup<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceUsages = InternalCompilerInterface.GetComponentLookup<ServiceUsage>(ref __TypeHandle.__Game_Buildings_ServiceUsage_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FlowEdges = InternalCompilerInterface.GetComponentLookup<ElectricityFlowEdge>(ref __TypeHandle.__Game_Simulation_ElectricityFlowEdge_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WindMap = m_WindSystem.GetMap(readOnly: true, out dependencies),
			m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
			m_WaterSurfaceData = m_WaterSystem.GetVelocitiesSurfaceData(out deps),
			m_GroundWaterMap = m_GroundWaterSystem.GetMap(readOnly: true, out dependencies2),
			m_SunLight = num
		};
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<PowerPlantTickJob>(powerPlantTickJob, m_PowerPlantQuery, JobUtils.CombineDependencies(((SystemBase)this).Dependency, dependencies, deps, dependencies2));
		m_WindSystem.AddReader(((SystemBase)this).Dependency);
		m_TerrainSystem.AddCPUHeightReader(((SystemBase)this).Dependency);
		m_WaterSystem.AddVelocitySurfaceReader(((SystemBase)this).Dependency);
		m_GroundWaterSystem.AddReader(((SystemBase)this).Dependency);
	}

	public static float2 GetWindProduction(WindPoweredData windData, Wind wind, float efficiency)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		float num = efficiency * (float)windData.m_Production;
		float num2 = math.lengthsq(wind.m_Wind) / (windData.m_MaximumWind * windData.m_MaximumWind);
		return new float2(num * math.saturate(math.pow(num2, 1.5f)), num);
	}

	public static float GetWaterCapacity(Game.Buildings.WaterPowered waterPowered, WaterPoweredData waterData)
	{
		return math.min(waterPowered.m_Length * waterPowered.m_Height, 1000000f) * waterData.m_CapacityFactor;
	}

	public static float2 GetGroundWaterProduction(GroundWaterPoweredData groundWaterData, float3 position, float efficiency, NativeArray<GroundWater> groundWaterMap)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)GroundWaterSystem.GetGroundWater(position, groundWaterMap).m_Amount / (float)groundWaterData.m_MaximumGroundWater;
		float num2 = efficiency * (float)groundWaterData.m_Production;
		return new float2(math.clamp(num2 * num, 0f, num2), num2);
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
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<ElectricityParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_833752410_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
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
	public PowerPlantAISystem()
	{
	}
}
