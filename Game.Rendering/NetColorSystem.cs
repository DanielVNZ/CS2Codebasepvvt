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
using Game.Simulation;
using Game.Tools;
using Game.Zones;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class NetColorSystem : GameSystemBase
{
	[BurstCompile]
	private struct UpdateEdgeColorsJob : IJobChunk
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_InfomodeChunks;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<InfomodeActive> m_InfomodeActiveType;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewCoverageData> m_InfoviewCoverageType;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewAvailabilityData> m_InfoviewAvailabilityType;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewNetGeometryData> m_InfoviewNetGeometryType;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewNetStatusData> m_InfoviewNetStatusType;

		[ReadOnly]
		public ComponentTypeHandle<TrainTrack> m_TrainTrackType;

		[ReadOnly]
		public ComponentTypeHandle<TramTrack> m_TramTrackType;

		[ReadOnly]
		public ComponentTypeHandle<Waterway> m_WaterwayType;

		[ReadOnly]
		public ComponentTypeHandle<SubwayTrack> m_SubwayTrackType;

		[ReadOnly]
		public ComponentTypeHandle<NetCondition> m_NetConditionType;

		[ReadOnly]
		public ComponentTypeHandle<Road> m_RoadType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.Pollution> m_PollutionType;

		[ReadOnly]
		public ComponentTypeHandle<EdgeGeometry> m_EdgeGeometryType;

		[ReadOnly]
		public BufferTypeHandle<Game.Net.ServiceCoverage> m_ServiceCoverageType;

		[ReadOnly]
		public BufferTypeHandle<ResourceAvailability> m_ResourceAvailabilityType;

		[ReadOnly]
		public ComponentLookup<LandValue> m_LandValues;

		[ReadOnly]
		public ComponentLookup<Edge> m_Edges;

		[ReadOnly]
		public ComponentLookup<Node> m_Nodes;

		[ReadOnly]
		public ComponentLookup<Temp> m_Temps;

		[ReadOnly]
		public ComponentLookup<ResourceData> m_ResourceDatas;

		[ReadOnly]
		public ComponentLookup<ZonePropertiesData> m_ZonePropertiesDatas;

		[ReadOnly]
		public ComponentLookup<PathwayData> m_PrefabPathwayData;

		[ReadOnly]
		public BufferLookup<Game.Net.ServiceCoverage> m_ServiceCoverageData;

		[ReadOnly]
		public BufferLookup<ResourceAvailability> m_ResourceAvailabilityData;

		[ReadOnly]
		public BufferLookup<ProcessEstimate> m_ProcessEstimates;

		[ReadOnly]
		public ComponentTypeHandle<Edge> m_EdgeType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		public ComponentTypeHandle<EdgeColor> m_ColorType;

		[ReadOnly]
		public Entity m_ZonePrefab;

		[ReadOnly]
		public ResourcePrefabs m_ResourcePrefabs;

		[ReadOnly]
		public NativeArray<GroundPollution> m_PollutionMap;

		[ReadOnly]
		public NativeArray<int> m_IndustrialDemands;

		[ReadOnly]
		public NativeArray<int> m_StorageDemands;

		[ReadOnly]
		public NativeList<IndustrialProcessData> m_Processes;

		[ReadOnly]
		public ZonePreferenceData m_ZonePreferences;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04df: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_0363: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_040c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0416: Unknown result type (might be due to invalid IL or missing references)
			//IL_041c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0425: Unknown result type (might be due to invalid IL or missing references)
			//IL_0428: Unknown result type (might be due to invalid IL or missing references)
			//IL_0434: Unknown result type (might be due to invalid IL or missing references)
			//IL_045e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0468: Unknown result type (might be due to invalid IL or missing references)
			//IL_046e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0477: Unknown result type (might be due to invalid IL or missing references)
			//IL_047a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0486: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<EdgeColor> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<EdgeColor>(ref m_ColorType);
			InfoviewAvailabilityData availabilityData;
			InfomodeActive activeData2;
			InfoviewNetStatusData statusData;
			InfomodeActive activeData3;
			int index;
			if (((ArchetypeChunk)(ref chunk)).Has<Game.Net.ServiceCoverage>(ref m_ServiceCoverageType) && GetServiceCoverageData(chunk, out var coverageData, out var activeData))
			{
				NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
				BufferAccessor<Game.Net.ServiceCoverage> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Game.Net.ServiceCoverage>(ref m_ServiceCoverageType);
				Temp temp = default(Temp);
				DynamicBuffer<Game.Net.ServiceCoverage> val2 = default(DynamicBuffer<Game.Net.ServiceCoverage>);
				EdgeColor edgeColor = default(EdgeColor);
				for (int i = 0; i < nativeArray.Length; i++)
				{
					DynamicBuffer<Game.Net.ServiceCoverage> val = bufferAccessor[i];
					if (CollectionUtils.TryGet<Temp>(nativeArray2, i, ref temp) && m_ServiceCoverageData.TryGetBuffer(temp.m_Original, ref val2))
					{
						val = val2;
					}
					if (val.Length == 0)
					{
						nativeArray[i] = default(EdgeColor);
						continue;
					}
					Game.Net.ServiceCoverage serviceCoverage = val[(int)coverageData.m_Service];
					edgeColor.m_Index = (byte)activeData.m_Index;
					edgeColor.m_Value0 = (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(coverageData, serviceCoverage.m_Coverage.x) * 255f), 0, 255);
					edgeColor.m_Value1 = (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(coverageData, serviceCoverage.m_Coverage.y) * 255f), 0, 255);
					nativeArray[i] = edgeColor;
				}
			}
			else if (((ArchetypeChunk)(ref chunk)).Has<ResourceAvailability>(ref m_ResourceAvailabilityType) && GetResourceAvailabilityData(chunk, out availabilityData, out activeData2))
			{
				ZonePreferenceData preferences = m_ZonePreferences;
				NativeArray<Edge> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Edge>(ref m_EdgeType);
				NativeArray<Temp> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
				BufferAccessor<ResourceAvailability> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ResourceAvailability>(ref m_ResourceAvailabilityType);
				Temp temp2 = default(Temp);
				Edge edge2 = default(Edge);
				Temp temp3 = default(Temp);
				LandValue landValue = default(LandValue);
				Temp temp4 = default(Temp);
				LandValue landValue2 = default(LandValue);
				DynamicBuffer<ResourceAvailability> val3 = default(DynamicBuffer<ResourceAvailability>);
				DynamicBuffer<ProcessEstimate> estimates = default(DynamicBuffer<ProcessEstimate>);
				ZonePropertiesData zonePropertiesData = default(ZonePropertiesData);
				EdgeColor edgeColor2 = default(EdgeColor);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Edge edge = nativeArray3[j];
					DynamicBuffer<ResourceAvailability> availabilityBuffer = bufferAccessor2[j];
					float num;
					float num2;
					if (CollectionUtils.TryGet<Temp>(nativeArray4, j, ref temp2))
					{
						if (!m_Edges.TryGetComponent(temp2.m_Original, ref edge2))
						{
							num = ((!m_Temps.TryGetComponent(edge.m_Start, ref temp3) || !m_LandValues.TryGetComponent(temp3.m_Original, ref landValue)) ? m_LandValues[edge.m_Start].m_LandValue : landValue.m_LandValue);
							num2 = ((!m_Temps.TryGetComponent(edge.m_End, ref temp4) || !m_LandValues.TryGetComponent(temp4.m_Original, ref landValue2)) ? m_LandValues[edge.m_End].m_LandValue : landValue2.m_LandValue);
						}
						else
						{
							edge = edge2;
							num = m_LandValues[edge2.m_Start].m_LandValue;
							num2 = m_LandValues[edge2.m_End].m_LandValue;
							if (m_ResourceAvailabilityData.TryGetBuffer(temp2.m_Original, ref val3))
							{
								availabilityBuffer = val3;
							}
						}
					}
					else
					{
						num = m_LandValues[edge.m_Start].m_LandValue;
						num2 = m_LandValues[edge.m_End].m_LandValue;
					}
					if (availabilityBuffer.Length == 0)
					{
						nativeArray[j] = default(EdgeColor);
						continue;
					}
					float3 position = m_Nodes[edge.m_Start].m_Position;
					float3 position2 = m_Nodes[edge.m_End].m_Position;
					GroundPollution pollution = GroundPollutionSystem.GetPollution(position, m_PollutionMap);
					GroundPollution pollution2 = GroundPollutionSystem.GetPollution(position2, m_PollutionMap);
					float pollution3 = pollution.m_Pollution;
					float pollution4 = pollution2.m_Pollution;
					m_ProcessEstimates.TryGetBuffer(m_ZonePrefab, ref estimates);
					if (m_ZonePropertiesDatas.TryGetComponent(m_ZonePrefab, ref zonePropertiesData))
					{
						float num3 = ((availabilityData.m_AreaType != AreaType.Residential) ? zonePropertiesData.m_SpaceMultiplier : (zonePropertiesData.m_ScaleResidentials ? zonePropertiesData.m_ResidentialProperties : (zonePropertiesData.m_ResidentialProperties / 8f)));
						num /= num3;
						num2 /= num3;
					}
					edgeColor2.m_Index = (byte)activeData2.m_Index;
					edgeColor2.m_Value0 = (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(availabilityData, availabilityBuffer, 0f, ref preferences, m_IndustrialDemands, m_StorageDemands, pollution3, num, estimates, m_Processes, m_ResourcePrefabs, m_ResourceDatas) * 255f), 0, 255);
					edgeColor2.m_Value1 = (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(availabilityData, availabilityBuffer, 1f, ref preferences, m_IndustrialDemands, m_StorageDemands, pollution4, num2, estimates, m_Processes, m_ResourcePrefabs, m_ResourceDatas) * 255f), 0, 255);
					nativeArray[j] = edgeColor2;
				}
			}
			else if (GetNetStatusType(chunk, out statusData, out activeData3))
			{
				GetNetStatusColors(nativeArray, chunk, statusData, activeData3);
			}
			else if (GetNetGeometryColor(chunk, out index))
			{
				for (int k = 0; k < nativeArray.Length; k++)
				{
					nativeArray[k] = new EdgeColor((byte)index, 0, 0);
				}
			}
			else
			{
				for (int l = 0; l < nativeArray.Length; l++)
				{
					nativeArray[l] = new EdgeColor(0, byte.MaxValue, byte.MaxValue);
				}
			}
		}

		private bool GetServiceCoverageData(ArchetypeChunk chunk, out InfoviewCoverageData coverageData, out InfomodeActive activeData)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			coverageData = default(InfoviewCoverageData);
			activeData = default(InfomodeActive);
			int num = int.MaxValue;
			for (int i = 0; i < m_InfomodeChunks.Length; i++)
			{
				ArchetypeChunk val = m_InfomodeChunks[i];
				NativeArray<InfoviewCoverageData> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<InfoviewCoverageData>(ref m_InfoviewCoverageType);
				if (nativeArray.Length == 0)
				{
					continue;
				}
				NativeArray<InfomodeActive> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<InfomodeActive>(ref m_InfomodeActiveType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					InfomodeActive infomodeActive = nativeArray2[j];
					int priority = infomodeActive.m_Priority;
					if (priority < num)
					{
						coverageData = nativeArray[j];
						coverageData.m_Service = CoverageService.Count;
						activeData = infomodeActive;
						num = priority;
					}
				}
			}
			return num != int.MaxValue;
		}

		private bool GetResourceAvailabilityData(ArchetypeChunk chunk, out InfoviewAvailabilityData availabilityData, out InfomodeActive activeData)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			availabilityData = default(InfoviewAvailabilityData);
			activeData = default(InfomodeActive);
			int num = int.MaxValue;
			for (int i = 0; i < m_InfomodeChunks.Length; i++)
			{
				ArchetypeChunk val = m_InfomodeChunks[i];
				NativeArray<InfoviewAvailabilityData> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<InfoviewAvailabilityData>(ref m_InfoviewAvailabilityType);
				if (nativeArray.Length == 0)
				{
					continue;
				}
				NativeArray<InfomodeActive> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<InfomodeActive>(ref m_InfomodeActiveType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					InfomodeActive infomodeActive = nativeArray2[j];
					int priority = infomodeActive.m_Priority;
					if (priority < num)
					{
						availabilityData = nativeArray[j];
						activeData = infomodeActive;
						num = priority;
					}
				}
			}
			return num != int.MaxValue;
		}

		private bool GetNetStatusType(ArchetypeChunk chunk, out InfoviewNetStatusData statusData, out InfomodeActive activeData)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			statusData = default(InfoviewNetStatusData);
			activeData = default(InfomodeActive);
			int num = int.MaxValue;
			for (int i = 0; i < m_InfomodeChunks.Length; i++)
			{
				ArchetypeChunk val = m_InfomodeChunks[i];
				NativeArray<InfoviewNetStatusData> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<InfoviewNetStatusData>(ref m_InfoviewNetStatusType);
				if (nativeArray.Length == 0)
				{
					continue;
				}
				NativeArray<InfomodeActive> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<InfomodeActive>(ref m_InfomodeActiveType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					InfomodeActive infomodeActive = nativeArray2[j];
					int priority = infomodeActive.m_Priority;
					if (priority < num)
					{
						InfoviewNetStatusData infoviewNetStatusData = nativeArray[j];
						if (HasNetStatus(nativeArray[j], chunk))
						{
							statusData = infoviewNetStatusData;
							activeData = infomodeActive;
							num = priority;
						}
					}
				}
			}
			return num != int.MaxValue;
		}

		private bool HasNetStatus(InfoviewNetStatusData infoviewNetStatusData, ArchetypeChunk chunk)
		{
			return infoviewNetStatusData.m_Type switch
			{
				NetStatusType.Wear => ((ArchetypeChunk)(ref chunk)).Has<NetCondition>(ref m_NetConditionType), 
				NetStatusType.TrafficFlow => ((ArchetypeChunk)(ref chunk)).Has<Road>(ref m_RoadType), 
				NetStatusType.NoisePollutionSource => ((ArchetypeChunk)(ref chunk)).Has<Game.Net.Pollution>(ref m_PollutionType), 
				NetStatusType.AirPollutionSource => ((ArchetypeChunk)(ref chunk)).Has<Game.Net.Pollution>(ref m_PollutionType), 
				NetStatusType.TrafficVolume => ((ArchetypeChunk)(ref chunk)).Has<Road>(ref m_RoadType), 
				NetStatusType.LeisureProvider => !((ArchetypeChunk)(ref chunk)).Has<Game.Net.ServiceCoverage>(ref m_ServiceCoverageType), 
				_ => false, 
			};
		}

		private void GetNetStatusColors(NativeArray<EdgeColor> results, ArchetypeChunk chunk, InfoviewNetStatusData statusData, InfomodeActive activeData)
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_039e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0481: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0426: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
			switch (statusData.m_Type)
			{
			case NetStatusType.Wear:
			{
				NativeArray<NetCondition> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<NetCondition>(ref m_NetConditionType);
				EdgeColor edgeColor4 = default(EdgeColor);
				for (int l = 0; l < nativeArray5.Length; l++)
				{
					NetCondition netCondition = nativeArray5[l];
					edgeColor4.m_Index = (byte)activeData.m_Index;
					edgeColor4.m_Value0 = (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(statusData, netCondition.m_Wear.x / 10f) * 255f), 0, 255);
					edgeColor4.m_Value1 = (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(statusData, netCondition.m_Wear.y / 10f) * 255f), 0, 255);
					results[l] = edgeColor4;
				}
				break;
			}
			case NetStatusType.TrafficFlow:
			{
				NativeArray<Road> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Road>(ref m_RoadType);
				EdgeColor edgeColor6 = default(EdgeColor);
				for (int n = 0; n < nativeArray8.Length; n++)
				{
					Road road2 = nativeArray8[n];
					float4 trafficFlowSpeed = NetUtils.GetTrafficFlowSpeed(road2.m_TrafficFlowDuration0, road2.m_TrafficFlowDistance0);
					float4 trafficFlowSpeed2 = NetUtils.GetTrafficFlowSpeed(road2.m_TrafficFlowDuration1, road2.m_TrafficFlowDistance1);
					edgeColor6.m_Index = (byte)activeData.m_Index;
					edgeColor6.m_Value0 = (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(statusData, math.csum(trafficFlowSpeed) * 0.125f + math.cmin(trafficFlowSpeed) * 0.5f) * 255f), 0, 255);
					edgeColor6.m_Value1 = (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(statusData, math.csum(trafficFlowSpeed2) * 0.125f + math.cmin(trafficFlowSpeed2) * 0.5f) * 255f), 0, 255);
					results[n] = edgeColor6;
				}
				break;
			}
			case NetStatusType.NoisePollutionSource:
			{
				NativeArray<Game.Net.Pollution> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Net.Pollution>(ref m_PollutionType);
				NativeArray<EdgeGeometry> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<EdgeGeometry>(ref m_EdgeGeometryType);
				EdgeColor edgeColor2 = default(EdgeColor);
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					float status = nativeArray2[j].m_Accumulation.x / math.max(0.1f, nativeArray3[j].m_Start.middleLength + nativeArray3[j].m_End.middleLength);
					edgeColor2.m_Index = (byte)activeData.m_Index;
					edgeColor2.m_Value0 = (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(statusData, status) * 255f), 0, 255);
					edgeColor2.m_Value1 = edgeColor2.m_Value0;
					results[j] = edgeColor2;
				}
				break;
			}
			case NetStatusType.AirPollutionSource:
			{
				NativeArray<Game.Net.Pollution> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Net.Pollution>(ref m_PollutionType);
				NativeArray<EdgeGeometry> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<EdgeGeometry>(ref m_EdgeGeometryType);
				EdgeColor edgeColor5 = default(EdgeColor);
				for (int m = 0; m < nativeArray6.Length; m++)
				{
					float status2 = nativeArray6[m].m_Accumulation.y / math.max(0.1f, nativeArray7[m].m_Start.middleLength + nativeArray7[m].m_End.middleLength);
					edgeColor5.m_Index = (byte)activeData.m_Index;
					edgeColor5.m_Value0 = (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(statusData, status2) * 255f), 0, 255);
					edgeColor5.m_Value1 = edgeColor5.m_Value0;
					results[m] = edgeColor5;
				}
				break;
			}
			case NetStatusType.TrafficVolume:
			{
				NativeArray<Road> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Road>(ref m_RoadType);
				EdgeColor edgeColor3 = default(EdgeColor);
				for (int k = 0; k < nativeArray4.Length; k++)
				{
					Road road = nativeArray4[k];
					float4 val = math.sqrt(road.m_TrafficFlowDistance0 * 5.3333335f);
					float4 val2 = math.sqrt(road.m_TrafficFlowDistance1 * 5.3333335f);
					edgeColor3.m_Index = (byte)activeData.m_Index;
					edgeColor3.m_Value0 = (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(statusData, math.csum(val) * 0.25f) * 255f), 0, 255);
					edgeColor3.m_Value1 = (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(statusData, math.csum(val2) * 0.25f) * 255f), 0, 255);
					results[k] = edgeColor3;
				}
				break;
			}
			case NetStatusType.LeisureProvider:
			{
				NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				PathwayData pathwayData = default(PathwayData);
				for (int i = 0; i < nativeArray.Length; i++)
				{
					EdgeColor edgeColor = new EdgeColor
					{
						m_Value0 = byte.MaxValue,
						m_Value1 = byte.MaxValue
					};
					if (m_PrefabPathwayData.TryGetComponent(nativeArray[i].m_Prefab, ref pathwayData) && pathwayData.m_LeisureProvider)
					{
						edgeColor.m_Index = (byte)activeData.m_Index;
					}
					results[i] = edgeColor;
				}
				break;
			}
			case NetStatusType.LowVoltageFlow:
			case NetStatusType.HighVoltageFlow:
			case NetStatusType.PipeWaterFlow:
			case NetStatusType.PipeSewageFlow:
			case NetStatusType.OilFlow:
				break;
			}
		}

		private bool GetNetGeometryColor(ArchetypeChunk chunk, out int index)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			index = 0;
			int num = int.MaxValue;
			for (int i = 0; i < m_InfomodeChunks.Length; i++)
			{
				ArchetypeChunk val = m_InfomodeChunks[i];
				NativeArray<InfoviewNetGeometryData> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<InfoviewNetGeometryData>(ref m_InfoviewNetGeometryType);
				if (nativeArray.Length == 0)
				{
					continue;
				}
				NativeArray<InfomodeActive> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<InfomodeActive>(ref m_InfomodeActiveType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					InfomodeActive infomodeActive = nativeArray2[j];
					int priority = infomodeActive.m_Priority;
					if (priority < num && HasNetGeometryColor(nativeArray[j], chunk))
					{
						index = infomodeActive.m_Index;
						num = priority;
					}
				}
			}
			return num != int.MaxValue;
		}

		private bool HasNetGeometryColor(InfoviewNetGeometryData infoviewNetGeometryData, ArchetypeChunk chunk)
		{
			return infoviewNetGeometryData.m_Type switch
			{
				NetType.TrainTrack => ((ArchetypeChunk)(ref chunk)).Has<TrainTrack>(ref m_TrainTrackType), 
				NetType.TramTrack => ((ArchetypeChunk)(ref chunk)).Has<TramTrack>(ref m_TramTrackType), 
				NetType.Waterway => ((ArchetypeChunk)(ref chunk)).Has<Waterway>(ref m_WaterwayType), 
				NetType.SubwayTrack => ((ArchetypeChunk)(ref chunk)).Has<SubwayTrack>(ref m_SubwayTrackType), 
				NetType.Road => ((ArchetypeChunk)(ref chunk)).Has<Road>(ref m_RoadType), 
				_ => false, 
			};
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct UpdateNodeColorsJob : IJobChunk
	{
		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<EdgeColor> m_ColorData;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> m_StartNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> m_EndNodeGeometryData;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public BufferTypeHandle<ConnectedEdge> m_ConnectedEdgeType;

		public ComponentTypeHandle<NodeColor> m_ColorType;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_InfomodeChunks;

		[ReadOnly]
		public ComponentTypeHandle<InfomodeActive> m_InfomodeActiveType;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewNetGeometryData> m_InfoviewNetGeometryType;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewNetStatusData> m_InfoviewNetStatusType;

		[ReadOnly]
		public ComponentTypeHandle<TrainTrack> m_TrainTrackType;

		[ReadOnly]
		public ComponentTypeHandle<TramTrack> m_TramTrackType;

		[ReadOnly]
		public ComponentTypeHandle<Waterway> m_WaterwayType;

		[ReadOnly]
		public ComponentTypeHandle<SubwayTrack> m_SubwayTrackType;

		[ReadOnly]
		public ComponentTypeHandle<NetCondition> m_NetConditionType;

		[ReadOnly]
		public ComponentTypeHandle<Road> m_RoadType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.Pollution> m_PollutionType;

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
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<NodeColor> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<NodeColor>(ref m_ColorType);
			NativeArray<Temp> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			BufferAccessor<ConnectedEdge> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ConnectedEdge>(ref m_ConnectedEdgeType);
			bool flag = false;
			int index;
			if (GetNetStatusType(chunk, out var statusData, out var activeData))
			{
				GetNetStatusColors(nativeArray2, chunk, statusData, activeData);
				flag = true;
			}
			else if (GetNetGeometryColor(chunk, out index))
			{
				for (int i = 0; i < nativeArray2.Length; i++)
				{
					nativeArray2[i] = new NodeColor((byte)index, 0);
				}
				flag = true;
			}
			Temp temp = default(Temp);
			DynamicBuffer<ConnectedEdge> val3 = default(DynamicBuffer<ConnectedEdge>);
			bool2 val6 = default(bool2);
			StartNodeGeometry startNodeGeometry = default(StartNodeGeometry);
			EndNodeGeometry endNodeGeometry = default(EndNodeGeometry);
			int3 val7 = default(int3);
			for (int j = 0; j < nativeArray.Length; j++)
			{
				Entity val = nativeArray[j];
				DynamicBuffer<ConnectedEdge> val2 = bufferAccessor[j];
				if (CollectionUtils.TryGet<Temp>(nativeArray3, j, ref temp) && m_ConnectedEdges.TryGetBuffer(temp.m_Original, ref val3))
				{
					val = temp.m_Original;
					val2 = val3;
				}
				int3 val4 = default(int3);
				int3 val5 = default(int3);
				bool flag2 = flag;
				for (int k = 0; k < val2.Length; k++)
				{
					Entity edge = val2[k].m_Edge;
					if (!m_ColorData.HasComponent(edge))
					{
						continue;
					}
					Edge edge2 = m_EdgeData[edge];
					((bool2)(ref val6))._002Ector(edge2.m_Start == val, edge2.m_End == val);
					if (!math.any(val6))
					{
						continue;
					}
					if (flag2)
					{
						if (val6.x)
						{
							if (m_StartNodeGeometryData.TryGetComponent(edge, ref startNodeGeometry))
							{
								flag2 = math.any(startNodeGeometry.m_Geometry.m_Left.m_Length > 0.05f) | math.any(startNodeGeometry.m_Geometry.m_Right.m_Length > 0.05f);
							}
						}
						else if (m_EndNodeGeometryData.TryGetComponent(edge, ref endNodeGeometry))
						{
							flag2 = math.any(endNodeGeometry.m_Geometry.m_Left.m_Length > 0.05f) | math.any(endNodeGeometry.m_Geometry.m_Right.m_Length > 0.05f);
						}
					}
					EdgeColor edgeColor = m_ColorData[edge];
					if (edgeColor.m_Index != 0)
					{
						val7.x = edgeColor.m_Index;
						val7.y = (val6.x ? edgeColor.m_Value0 : edgeColor.m_Value1);
						val7.z = 1;
						if ((val7.x == val4.x) | (val4.z == 0))
						{
							val4.x = val7.x;
							((int3)(ref val4)).yz = ((int3)(ref val4)).yz + ((int3)(ref val7)).yz;
						}
						else if ((val7.x == val5.x) | (val5.z == 0))
						{
							val5.x = val7.x;
							((int3)(ref val5)).yz = ((int3)(ref val5)).yz + ((int3)(ref val7)).yz;
						}
					}
				}
				if (!flag2)
				{
					val4 = math.select(val4, val5, (val5.z > val4.z) | ((val5.z == val4.z) & (val5.x < val4.x)));
					if (val4.z > 0)
					{
						val4.y /= val4.z;
						nativeArray2[j] = new NodeColor((byte)val4.x, (byte)val4.y);
					}
					else
					{
						nativeArray2[j] = new NodeColor(0, byte.MaxValue);
					}
				}
			}
		}

		private bool GetNetStatusType(ArchetypeChunk chunk, out InfoviewNetStatusData statusData, out InfomodeActive activeData)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			statusData = default(InfoviewNetStatusData);
			activeData = default(InfomodeActive);
			int num = int.MaxValue;
			for (int i = 0; i < m_InfomodeChunks.Length; i++)
			{
				ArchetypeChunk val = m_InfomodeChunks[i];
				NativeArray<InfoviewNetStatusData> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<InfoviewNetStatusData>(ref m_InfoviewNetStatusType);
				if (nativeArray.Length == 0)
				{
					continue;
				}
				NativeArray<InfomodeActive> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<InfomodeActive>(ref m_InfomodeActiveType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					InfomodeActive infomodeActive = nativeArray2[j];
					int priority = infomodeActive.m_Priority;
					if (priority < num)
					{
						InfoviewNetStatusData infoviewNetStatusData = nativeArray[j];
						if (HasNetStatus(nativeArray[j], chunk))
						{
							statusData = infoviewNetStatusData;
							activeData = infomodeActive;
							num = priority;
						}
					}
				}
			}
			return num != int.MaxValue;
		}

		private bool HasNetStatus(InfoviewNetStatusData infoviewNetStatusData, ArchetypeChunk chunk)
		{
			return infoviewNetStatusData.m_Type switch
			{
				NetStatusType.Wear => ((ArchetypeChunk)(ref chunk)).Has<NetCondition>(ref m_NetConditionType), 
				NetStatusType.TrafficFlow => ((ArchetypeChunk)(ref chunk)).Has<Road>(ref m_RoadType), 
				NetStatusType.NoisePollutionSource => ((ArchetypeChunk)(ref chunk)).Has<Game.Net.Pollution>(ref m_PollutionType), 
				NetStatusType.AirPollutionSource => ((ArchetypeChunk)(ref chunk)).Has<Game.Net.Pollution>(ref m_PollutionType), 
				NetStatusType.TrafficVolume => ((ArchetypeChunk)(ref chunk)).Has<Road>(ref m_RoadType), 
				_ => false, 
			};
		}

		private float GetRelativeLength(Entity entity, DynamicBuffer<ConnectedEdge> edges)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			float num = 0f;
			bool2 val = default(bool2);
			StartNodeGeometry startNodeGeometry = default(StartNodeGeometry);
			EndNodeGeometry endNodeGeometry = default(EndNodeGeometry);
			for (int i = 0; i < edges.Length; i++)
			{
				Entity edge = edges[i].m_Edge;
				Edge edge2 = m_EdgeData[edge];
				((bool2)(ref val))._002Ector(edge2.m_Start == entity, edge2.m_End == entity);
				if (!math.any(val))
				{
					continue;
				}
				EdgeNodeGeometry edgeNodeGeometry = default(EdgeNodeGeometry);
				if (val.x)
				{
					if (m_StartNodeGeometryData.TryGetComponent(edge, ref startNodeGeometry))
					{
						edgeNodeGeometry = startNodeGeometry.m_Geometry;
					}
				}
				else if (m_EndNodeGeometryData.TryGetComponent(edge, ref endNodeGeometry))
				{
					edgeNodeGeometry = endNodeGeometry.m_Geometry;
				}
				num = ((!(edgeNodeGeometry.m_MiddleRadius > 0f)) ? (num + (edgeNodeGeometry.m_Left.middleLength + edgeNodeGeometry.m_Right.middleLength) * 0.5f) : (num + (edgeNodeGeometry.m_Left.middleLength + edgeNodeGeometry.m_Right.middleLength)));
			}
			return num;
		}

		private void GetNetStatusColors(NativeArray<NodeColor> results, ArchetypeChunk chunk, InfoviewNetStatusData statusData, InfomodeActive activeData)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			switch (statusData.m_Type)
			{
			case NetStatusType.Wear:
			{
				NativeArray<NetCondition> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<NetCondition>(ref m_NetConditionType);
				NodeColor nodeColor2 = default(NodeColor);
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					nodeColor2.m_Index = (byte)activeData.m_Index;
					nodeColor2.m_Value = (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(statusData, math.cmax(nativeArray2[j].m_Wear) / 10f) * 255f), 0, 255);
					results[j] = nodeColor2;
				}
				break;
			}
			case NetStatusType.TrafficFlow:
			{
				NativeArray<Road> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Road>(ref m_RoadType);
				NodeColor nodeColor4 = default(NodeColor);
				for (int l = 0; l < nativeArray5.Length; l++)
				{
					float4 trafficFlowSpeed = NetUtils.GetTrafficFlowSpeed(nativeArray5[l]);
					nodeColor4.m_Index = (byte)activeData.m_Index;
					nodeColor4.m_Value = (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(statusData, math.csum(trafficFlowSpeed) * 0.125f + math.cmin(trafficFlowSpeed) * 0.5f) * 255f), 0, 255);
					results[l] = nodeColor4;
				}
				break;
			}
			case NetStatusType.NoisePollutionSource:
			{
				NativeArray<Entity> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				NativeArray<Game.Net.Pollution> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Net.Pollution>(ref m_PollutionType);
				BufferAccessor<ConnectedEdge> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ConnectedEdge>(ref m_ConnectedEdgeType);
				NodeColor nodeColor5 = default(NodeColor);
				for (int m = 0; m < nativeArray7.Length; m++)
				{
					float status2 = nativeArray7[m].m_Accumulation.x / math.max(0.1f, GetRelativeLength(nativeArray6[m], bufferAccessor2[m]));
					nodeColor5.m_Index = (byte)activeData.m_Index;
					nodeColor5.m_Value = (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(statusData, status2) * 255f), 0, 255);
					results[m] = nodeColor5;
				}
				break;
			}
			case NetStatusType.AirPollutionSource:
			{
				NativeArray<Entity> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				NativeArray<Game.Net.Pollution> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Net.Pollution>(ref m_PollutionType);
				BufferAccessor<ConnectedEdge> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ConnectedEdge>(ref m_ConnectedEdgeType);
				NodeColor nodeColor3 = default(NodeColor);
				for (int k = 0; k < nativeArray4.Length; k++)
				{
					float status = nativeArray4[k].m_Accumulation.y / math.max(0.1f, GetRelativeLength(nativeArray3[k], bufferAccessor[k]));
					nodeColor3.m_Index = (byte)activeData.m_Index;
					nodeColor3.m_Value = (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(statusData, status) * 255f), 0, 255);
					results[k] = nodeColor3;
				}
				break;
			}
			case NetStatusType.TrafficVolume:
			{
				NativeArray<Road> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Road>(ref m_RoadType);
				NodeColor nodeColor = default(NodeColor);
				for (int i = 0; i < nativeArray.Length; i++)
				{
					Road road = nativeArray[i];
					float4 val = math.sqrt((road.m_TrafficFlowDistance0 + road.m_TrafficFlowDistance1) * 2.6666667f);
					nodeColor.m_Index = (byte)activeData.m_Index;
					nodeColor.m_Value = (byte)math.clamp(Mathf.RoundToInt(InfoviewUtils.GetColor(statusData, math.csum(val) * 0.25f) * 255f), 0, 255);
					results[i] = nodeColor;
				}
				break;
			}
			}
		}

		private bool GetNetGeometryColor(ArchetypeChunk chunk, out int index)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			index = 0;
			int num = int.MaxValue;
			for (int i = 0; i < m_InfomodeChunks.Length; i++)
			{
				ArchetypeChunk val = m_InfomodeChunks[i];
				NativeArray<InfoviewNetGeometryData> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<InfoviewNetGeometryData>(ref m_InfoviewNetGeometryType);
				if (nativeArray.Length == 0)
				{
					continue;
				}
				NativeArray<InfomodeActive> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<InfomodeActive>(ref m_InfomodeActiveType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					InfomodeActive infomodeActive = nativeArray2[j];
					int priority = infomodeActive.m_Priority;
					if (priority < num && HasNetGeometryColor(nativeArray[j], chunk))
					{
						index = infomodeActive.m_Index;
						num = priority;
					}
				}
			}
			return num != int.MaxValue;
		}

		private bool HasNetGeometryColor(InfoviewNetGeometryData infoviewNetGeometryData, ArchetypeChunk chunk)
		{
			return infoviewNetGeometryData.m_Type switch
			{
				NetType.TrainTrack => ((ArchetypeChunk)(ref chunk)).Has<TrainTrack>(ref m_TrainTrackType), 
				NetType.TramTrack => ((ArchetypeChunk)(ref chunk)).Has<TramTrack>(ref m_TramTrackType), 
				NetType.Waterway => ((ArchetypeChunk)(ref chunk)).Has<Waterway>(ref m_WaterwayType), 
				NetType.SubwayTrack => ((ArchetypeChunk)(ref chunk)).Has<SubwayTrack>(ref m_SubwayTrackType), 
				NetType.Road => ((ArchetypeChunk)(ref chunk)).Has<Road>(ref m_RoadType), 
				_ => false, 
			};
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct UpdateEdgeColors2Job : IJobChunk
	{
		[ReadOnly]
		public ComponentLookup<NodeColor> m_ColorData;

		[ReadOnly]
		public ComponentTypeHandle<Edge> m_EdgeType;

		[ReadOnly]
		public ComponentTypeHandle<StartNodeGeometry> m_StartNodeGeometryType;

		[ReadOnly]
		public ComponentTypeHandle<EndNodeGeometry> m_EndNodeGeometryType;

		public ComponentTypeHandle<EdgeColor> m_ColorType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Edge> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Edge>(ref m_EdgeType);
			NativeArray<StartNodeGeometry> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<StartNodeGeometry>(ref m_StartNodeGeometryType);
			NativeArray<EndNodeGeometry> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<EndNodeGeometry>(ref m_EndNodeGeometryType);
			NativeArray<EdgeColor> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<EdgeColor>(ref m_ColorType);
			NodeColor nodeColor = default(NodeColor);
			NodeColor nodeColor2 = default(NodeColor);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Edge edge = nativeArray[i];
				EdgeColor edgeColor = nativeArray4[i];
				bool2 val = bool2.op_Implicit(false);
				if (nativeArray2.Length != 0)
				{
					StartNodeGeometry startNodeGeometry = nativeArray2[i];
					if (math.any(startNodeGeometry.m_Geometry.m_Left.m_Length > 0.05f) | math.any(startNodeGeometry.m_Geometry.m_Right.m_Length > 0.05f))
					{
						val.x = true;
					}
				}
				if (nativeArray3.Length != 0)
				{
					EndNodeGeometry endNodeGeometry = nativeArray3[i];
					if (math.any(endNodeGeometry.m_Geometry.m_Left.m_Length > 0.05f) | math.any(endNodeGeometry.m_Geometry.m_Right.m_Length > 0.05f))
					{
						val.y = true;
					}
				}
				if (!val.x && m_ColorData.TryGetComponent(edge.m_Start, ref nodeColor) && nodeColor.m_Index == edgeColor.m_Index)
				{
					edgeColor.m_Value0 = nodeColor.m_Value;
				}
				if (!val.y && m_ColorData.TryGetComponent(edge.m_End, ref nodeColor2) && nodeColor2.m_Index == edgeColor.m_Index)
				{
					edgeColor.m_Value1 = nodeColor2.m_Value;
				}
				nativeArray4[i] = edgeColor;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct LaneColorJob : IJobChunk
	{
		private interface IFlowImplementation
		{
			Entity sinkNode { get; }

			bool subObjects { get; }

			bool connectedBuildings { get; }

			int multiplier { get; }

			bool TryGetFlowNode(Entity entity, out Entity flowNode);

			bool TryGetFlowEdge(Entity startNode, Entity endNode, out int flow, out int capacity, out float warning);

			void GetConsumption(Entity building, out int wantedConsumption, out int fulfilledConsumption, out float warning);
		}

		private struct ElectricityFlow : IFlowImplementation
		{
			[ReadOnly]
			public ComponentLookup<ElectricityNodeConnection> m_NodeConnectionData;

			[ReadOnly]
			public ComponentLookup<ElectricityFlowEdge> m_FlowEdgeData;

			[ReadOnly]
			public ComponentLookup<ElectricityBuildingConnection> m_BuildingConnectionData;

			[ReadOnly]
			public ComponentLookup<ElectricityConsumer> m_ConsumerData;

			[ReadOnly]
			public BufferLookup<ConnectedFlowEdge> m_ConnectedFlowEdges;

			public Entity sinkNode { get; set; }

			public bool subObjects => false;

			public bool connectedBuildings => true;

			public int multiplier => 1;

			public bool TryGetFlowNode(Entity entity, out Entity flowNode)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				ElectricityNodeConnection electricityNodeConnection = default(ElectricityNodeConnection);
				if (m_NodeConnectionData.TryGetComponent(entity, ref electricityNodeConnection))
				{
					flowNode = electricityNodeConnection.m_ElectricityNode;
					return true;
				}
				flowNode = default(Entity);
				return false;
			}

			public bool TryGetFlowEdge(Entity startNode, Entity endNode, out int flow, out int capacity, out float warning)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				if (ElectricityGraphUtils.TryGetFlowEdge(startNode, endNode, ref m_ConnectedFlowEdges, ref m_FlowEdgeData, out ElectricityFlowEdge edge))
				{
					flow = edge.m_Flow;
					capacity = edge.m_Capacity;
					warning = math.select(0f, 0.75f, (edge.m_Flags & ElectricityFlowEdgeFlags.BeyondBottleneck) != 0);
					warning = math.select(warning, 1f, (edge.m_Flags & ElectricityFlowEdgeFlags.Bottleneck) != 0);
					return true;
				}
				flow = (capacity = 0);
				warning = 0f;
				return false;
			}

			public void GetConsumption(Entity building, out int wantedConsumption, out int fulfilledConsumption, out float warning)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				ElectricityConsumer electricityConsumer = default(ElectricityConsumer);
				if (m_ConsumerData.TryGetComponent(building, ref electricityConsumer) && !m_BuildingConnectionData.HasComponent(building))
				{
					wantedConsumption = electricityConsumer.m_WantedConsumption;
					fulfilledConsumption = electricityConsumer.m_FulfilledConsumption;
					warning = math.select(0f, 0.75f, (electricityConsumer.m_Flags & ElectricityConsumerFlags.BottleneckWarning) != 0);
				}
				else
				{
					wantedConsumption = (fulfilledConsumption = 0);
					warning = 0f;
				}
			}
		}

		private struct WaterFlow : IFlowImplementation
		{
			[ReadOnly]
			public ComponentLookup<WaterPipeNodeConnection> m_NodeConnectionData;

			[ReadOnly]
			public ComponentLookup<WaterPipeEdge> m_FlowEdgeData;

			[ReadOnly]
			public ComponentLookup<WaterPipeBuildingConnection> m_BuildingConnectionData;

			[ReadOnly]
			public ComponentLookup<WaterConsumer> m_ConsumerData;

			[ReadOnly]
			public BufferLookup<ConnectedFlowEdge> m_ConnectedFlowEdges;

			public float m_MaxToleratedPollution;

			public Entity sinkNode { get; set; }

			public bool subObjects => false;

			public bool connectedBuildings => true;

			public int multiplier => 1;

			public bool TryGetFlowNode(Entity entity, out Entity flowNode)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				WaterPipeNodeConnection waterPipeNodeConnection = default(WaterPipeNodeConnection);
				if (m_NodeConnectionData.TryGetComponent(entity, ref waterPipeNodeConnection))
				{
					flowNode = waterPipeNodeConnection.m_WaterPipeNode;
					return true;
				}
				flowNode = default(Entity);
				return false;
			}

			public bool TryGetFlowEdge(Entity startNode, Entity endNode, out int flow, out int capacity, out float warning)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				if (WaterPipeGraphUtils.TryGetFlowEdge(startNode, endNode, ref m_ConnectedFlowEdges, ref m_FlowEdgeData, out WaterPipeEdge edge))
				{
					flow = edge.m_FreshFlow;
					capacity = 10000;
					warning = math.saturate(edge.m_FreshPollution / m_MaxToleratedPollution);
					return true;
				}
				flow = (capacity = 0);
				warning = 0f;
				return false;
			}

			public void GetConsumption(Entity building, out int wantedConsumption, out int fulfilledConsumption, out float warning)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				WaterConsumer waterConsumer = default(WaterConsumer);
				if (m_ConsumerData.TryGetComponent(building, ref waterConsumer) && !m_BuildingConnectionData.HasComponent(building))
				{
					wantedConsumption = waterConsumer.m_WantedConsumption;
					fulfilledConsumption = waterConsumer.m_FulfilledFresh;
					warning = math.select(0f, 1f, waterConsumer.m_Pollution > 0f);
				}
				else
				{
					wantedConsumption = (fulfilledConsumption = 0);
					warning = 0f;
				}
			}
		}

		private struct SewageFlow : IFlowImplementation
		{
			[ReadOnly]
			public ComponentLookup<WaterPipeNodeConnection> m_NodeConnectionData;

			[ReadOnly]
			public ComponentLookup<WaterPipeEdge> m_FlowEdgeData;

			[ReadOnly]
			public ComponentLookup<WaterPipeBuildingConnection> m_BuildingConnectionData;

			[ReadOnly]
			public ComponentLookup<WaterConsumer> m_ConsumerData;

			[ReadOnly]
			public BufferLookup<ConnectedFlowEdge> m_ConnectedFlowEdges;

			public Entity sinkNode { get; set; }

			public bool subObjects => false;

			public bool connectedBuildings => true;

			public int multiplier => -1;

			public bool TryGetFlowNode(Entity entity, out Entity flowNode)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				WaterPipeNodeConnection waterPipeNodeConnection = default(WaterPipeNodeConnection);
				if (m_NodeConnectionData.TryGetComponent(entity, ref waterPipeNodeConnection))
				{
					flowNode = waterPipeNodeConnection.m_WaterPipeNode;
					return true;
				}
				flowNode = default(Entity);
				return false;
			}

			public bool TryGetFlowEdge(Entity startNode, Entity endNode, out int flow, out int capacity, out float warning)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				if (WaterPipeGraphUtils.TryGetFlowEdge(startNode, endNode, ref m_ConnectedFlowEdges, ref m_FlowEdgeData, out WaterPipeEdge edge))
				{
					flow = edge.m_SewageFlow;
					capacity = 10000;
					warning = 0f;
					return true;
				}
				flow = (capacity = 0);
				warning = 0f;
				return false;
			}

			public void GetConsumption(Entity building, out int wantedConsumption, out int fulfilledConsumption, out float warning)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				WaterConsumer waterConsumer = default(WaterConsumer);
				if (m_ConsumerData.TryGetComponent(building, ref waterConsumer) && !m_BuildingConnectionData.HasComponent(building))
				{
					wantedConsumption = waterConsumer.m_WantedConsumption;
					fulfilledConsumption = waterConsumer.m_FulfilledSewage;
				}
				else
				{
					wantedConsumption = (fulfilledConsumption = 0);
				}
				warning = 0f;
			}
		}

		private struct ResourceFlow : IFlowImplementation
		{
			[ReadOnly]
			public ComponentLookup<Edge> m_EdgeData;

			[ReadOnly]
			public ComponentLookup<Game.Net.ResourceConnection> m_ResourceConnectionData;

			[ReadOnly]
			public BufferLookup<ConnectedEdge> m_ConnectedEdges;

			public Entity sinkNode { get; set; }

			public bool subObjects => true;

			public bool connectedBuildings => false;

			public int multiplier => -1;

			public bool TryGetFlowNode(Entity entity, out Entity flowNode)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				if (m_ResourceConnectionData.HasComponent(entity))
				{
					flowNode = entity;
					return true;
				}
				flowNode = default(Entity);
				return false;
			}

			public bool TryGetFlowEdge(Entity startNode, Entity endNode, out int flow, out int capacity, out float warning)
			{
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_006d: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_004e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_0081: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_017c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0189: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0128: Unknown result type (might be due to invalid IL or missing references)
				//IL_012d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0105: Unknown result type (might be due to invalid IL or missing references)
				//IL_013d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0116: Unknown result type (might be due to invalid IL or missing references)
				//IL_014e: Unknown result type (might be due to invalid IL or missing references)
				int num = 0;
				Edge edge = default(Edge);
				if (m_EdgeData.TryGetComponent(startNode, ref edge))
				{
					CommonUtils.Swap(ref startNode, ref endNode);
					num = -1;
				}
				else if (m_EdgeData.TryGetComponent(endNode, ref edge))
				{
					num = 1;
				}
				if (num != 0)
				{
					flow = 0;
					if (startNode == edge.m_Start)
					{
						Game.Net.ResourceConnection resourceConnection = default(Game.Net.ResourceConnection);
						if (m_ResourceConnectionData.TryGetComponent(endNode, ref resourceConnection))
						{
							flow = resourceConnection.m_Flow.x;
						}
					}
					else if (startNode == edge.m_End)
					{
						Game.Net.ResourceConnection resourceConnection2 = default(Game.Net.ResourceConnection);
						if (m_ResourceConnectionData.TryGetComponent(endNode, ref resourceConnection2))
						{
							flow = -resourceConnection2.m_Flow.y;
						}
					}
					else
					{
						bool flag = false;
						DynamicBuffer<ConnectedEdge> val = default(DynamicBuffer<ConnectedEdge>);
						if (m_ConnectedEdges.TryGetBuffer(startNode, ref val))
						{
							Game.Net.ResourceConnection resourceConnection3 = default(Game.Net.ResourceConnection);
							Game.Net.ResourceConnection resourceConnection4 = default(Game.Net.ResourceConnection);
							for (int i = 0; i < val.Length; i++)
							{
								ConnectedEdge connectedEdge = val[i];
								if (connectedEdge.m_Edge == endNode)
								{
									continue;
								}
								edge = m_EdgeData[connectedEdge.m_Edge];
								if (edge.m_Start == startNode)
								{
									if (m_ResourceConnectionData.TryGetComponent(connectedEdge.m_Edge, ref resourceConnection3))
									{
										flow = -resourceConnection3.m_Flow.x;
										flag = true;
									}
									break;
								}
								if (edge.m_End == startNode)
								{
									if (m_ResourceConnectionData.TryGetComponent(connectedEdge.m_Edge, ref resourceConnection4))
									{
										flow = resourceConnection4.m_Flow.y;
										flag = true;
									}
									break;
								}
							}
						}
						Game.Net.ResourceConnection resourceConnection5 = default(Game.Net.ResourceConnection);
						if (!flag && m_ResourceConnectionData.TryGetComponent(startNode, ref resourceConnection5))
						{
							flow = -resourceConnection5.m_Flow.x;
						}
					}
					flow *= num;
					capacity = 100;
					warning = 0f;
					return true;
				}
				flow = (capacity = 0);
				warning = 0f;
				return false;
			}

			public void GetConsumption(Entity building, out int wantedConsumption, out int fulfilledConsumption, out float warning)
			{
				wantedConsumption = (fulfilledConsumption = 0);
				warning = 0f;
			}
		}

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Curve> m_CurveType;

		[ReadOnly]
		public ComponentTypeHandle<EdgeLane> m_EdgeLaneType;

		[ReadOnly]
		public ComponentTypeHandle<NodeLane> m_NodeLaneType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.TrackLane> m_TrackLaneType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.UtilityLane> m_UtilityLaneType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.SecondaryLane> m_SecondaryLaneType;

		[ReadOnly]
		public ComponentTypeHandle<EdgeMapping> m_EdgeMappingType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		public ComponentTypeHandle<LaneColor> m_ColorType;

		public BufferTypeHandle<SubFlow> m_SubFlowType;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_InfomodeChunks;

		[ReadOnly]
		public ComponentTypeHandle<InfomodeActive> m_InfomodeActiveType;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewNetGeometryData> m_InfoviewNetGeometryType;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewNetStatusData> m_InfoviewNetStatusType;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ResourceConnection> m_ResourceConnectionData;

		[ReadOnly]
		public ComponentLookup<Color> m_ObjectColorData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<NodeColor> m_NodeColorData;

		[ReadOnly]
		public ComponentLookup<EdgeColor> m_EdgeColorData;

		[ReadOnly]
		public ComponentLookup<ElectricityNodeConnection> m_ElectricityNodeConnectionData;

		[ReadOnly]
		public ComponentLookup<ElectricityFlowEdge> m_ElectricityFlowEdgeData;

		[ReadOnly]
		public ComponentLookup<ElectricityBuildingConnection> m_ElectricityBuildingConnectionData;

		[ReadOnly]
		public ComponentLookup<WaterPipeNodeConnection> m_WaterPipeNodeConnectionData;

		[ReadOnly]
		public ComponentLookup<WaterPipeEdge> m_WaterPipeEdgeData;

		[ReadOnly]
		public ComponentLookup<WaterPipeBuildingConnection> m_WaterPipeBuildingConnectionData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<ElectricityConsumer> m_ElectricityConsumerData;

		[ReadOnly]
		public ComponentLookup<WaterConsumer> m_WaterConsumerData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<TrackLaneData> m_PrefabTrackLaneData;

		[ReadOnly]
		public ComponentLookup<UtilityLaneData> m_PrefabUtilityLaneData;

		[ReadOnly]
		public ComponentLookup<NetLaneData> m_PrefabNetLaneData;

		[ReadOnly]
		public BufferLookup<ConnectedNode> m_ConnectedNodes;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		[ReadOnly]
		public BufferLookup<ConnectedBuilding> m_ConnectedBuildings;

		[ReadOnly]
		public BufferLookup<ConnectedFlowEdge> m_ConnectedFlowEdges;

		public Entity m_ElectricitySinkNode;

		public Entity m_WaterSinkNode;

		public WaterPipeParameterData m_WaterPipeParameters;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
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
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			//IL_0881: Unknown result type (might be due to invalid IL or missing references)
			//IL_0952: Unknown result type (might be due to invalid IL or missing references)
			//IL_0906: Unknown result type (might be due to invalid IL or missing references)
			//IL_089a: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_08bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_08cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0971: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_052e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0533: Unknown result type (might be due to invalid IL or missing references)
			//IL_053e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0543: Unknown result type (might be due to invalid IL or missing references)
			//IL_055a: Unknown result type (might be due to invalid IL or missing references)
			//IL_073e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0742: Unknown result type (might be due to invalid IL or missing references)
			//IL_0682: Unknown result type (might be due to invalid IL or missing references)
			//IL_056f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0574: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0582: Unknown result type (might be due to invalid IL or missing references)
			//IL_0596: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_075d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0761: Unknown result type (might be due to invalid IL or missing references)
			//IL_078e: Unknown result type (might be due to invalid IL or missing references)
			//IL_077c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0780: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_060e: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_062b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0714: Unknown result type (might be due to invalid IL or missing references)
			//IL_0704: Unknown result type (might be due to invalid IL or missing references)
			//IL_0658: Unknown result type (might be due to invalid IL or missing references)
			//IL_0648: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<LaneColor> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<LaneColor>(ref m_ColorType);
			NativeArray<Owner> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<Curve> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Curve>(ref m_CurveType);
			NativeArray<EdgeLane> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<EdgeLane>(ref m_EdgeLaneType);
			NativeArray<NodeLane> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<NodeLane>(ref m_NodeLaneType);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			float num8 = 0f;
			float num9 = 0f;
			float num10 = 0f;
			float num11 = 0f;
			float num12 = 0f;
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<Game.Net.TrackLane>(ref m_TrackLaneType);
			bool flag2 = ((ArchetypeChunk)(ref chunk)).Has<Game.Net.UtilityLane>(ref m_UtilityLaneType);
			bool flag3 = ((ArchetypeChunk)(ref chunk)).Has<Game.Net.SecondaryLane>(ref m_SecondaryLaneType);
			bool flag4 = ((ArchetypeChunk)(ref chunk)).Has<Temp>(ref m_TempType);
			NativeArray<EdgeMapping> val = default(NativeArray<EdgeMapping>);
			NativeArray<PrefabRef> val2 = default(NativeArray<PrefabRef>);
			BufferAccessor<SubFlow> val3 = default(BufferAccessor<SubFlow>);
			if (flag)
			{
				val2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				int num13 = int.MaxValue;
				int num14 = int.MaxValue;
				for (int i = 0; i < m_InfomodeChunks.Length; i++)
				{
					ArchetypeChunk val4 = m_InfomodeChunks[i];
					NativeArray<InfoviewNetGeometryData> nativeArray6 = ((ArchetypeChunk)(ref val4)).GetNativeArray<InfoviewNetGeometryData>(ref m_InfoviewNetGeometryType);
					if (nativeArray6.Length == 0)
					{
						continue;
					}
					NativeArray<InfomodeActive> nativeArray7 = ((ArchetypeChunk)(ref val4)).GetNativeArray<InfomodeActive>(ref m_InfomodeActiveType);
					for (int j = 0; j < nativeArray6.Length; j++)
					{
						InfoviewNetGeometryData infoviewNetGeometryData = nativeArray6[j];
						InfomodeActive infomodeActive = nativeArray7[j];
						int priority = infomodeActive.m_Priority;
						switch (infoviewNetGeometryData.m_Type)
						{
						case NetType.TrainTrack:
							if (priority < num13)
							{
								num = infomodeActive.m_Index;
								num13 = priority;
							}
							break;
						case NetType.TramTrack:
							if (priority < num14)
							{
								num2 = infomodeActive.m_Index;
								num14 = priority;
							}
							break;
						}
					}
				}
			}
			if (flag2)
			{
				val = ((ArchetypeChunk)(ref chunk)).GetNativeArray<EdgeMapping>(ref m_EdgeMappingType);
				val2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				val3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<SubFlow>(ref m_SubFlowType);
				int num15 = int.MaxValue;
				int num16 = int.MaxValue;
				int num17 = int.MaxValue;
				int num18 = int.MaxValue;
				int num19 = int.MaxValue;
				for (int k = 0; k < m_InfomodeChunks.Length; k++)
				{
					ArchetypeChunk val5 = m_InfomodeChunks[k];
					NativeArray<InfoviewNetStatusData> nativeArray8 = ((ArchetypeChunk)(ref val5)).GetNativeArray<InfoviewNetStatusData>(ref m_InfoviewNetStatusType);
					if (nativeArray8.Length == 0)
					{
						continue;
					}
					NativeArray<InfomodeActive> nativeArray9 = ((ArchetypeChunk)(ref val5)).GetNativeArray<InfomodeActive>(ref m_InfomodeActiveType);
					for (int l = 0; l < nativeArray8.Length; l++)
					{
						InfoviewNetStatusData infoviewNetStatusData = nativeArray8[l];
						InfomodeActive infomodeActive2 = nativeArray9[l];
						int priority2 = infomodeActive2.m_Priority;
						switch (infoviewNetStatusData.m_Type)
						{
						case NetStatusType.LowVoltageFlow:
							if (priority2 < num15)
							{
								num3 = infomodeActive2.m_Index;
								num8 = infoviewNetStatusData.m_Tiling;
								num15 = priority2;
							}
							break;
						case NetStatusType.HighVoltageFlow:
							if (priority2 < num16)
							{
								num4 = infomodeActive2.m_Index;
								num9 = infoviewNetStatusData.m_Tiling;
								num16 = priority2;
							}
							break;
						case NetStatusType.PipeWaterFlow:
							if (priority2 < num17)
							{
								num5 = infomodeActive2.m_Index;
								num10 = infoviewNetStatusData.m_Tiling;
								num17 = priority2;
							}
							break;
						case NetStatusType.PipeSewageFlow:
							if (priority2 < num18)
							{
								num6 = infomodeActive2.m_Index;
								num11 = infoviewNetStatusData.m_Tiling;
								num18 = priority2;
							}
							break;
						case NetStatusType.OilFlow:
							if (priority2 < num19)
							{
								num7 = infomodeActive2.m_Index;
								num12 = infoviewNetStatusData.m_Tiling;
								num19 = priority2;
							}
							break;
						}
					}
				}
			}
			bool flag5 = flag && (num != 0 || num2 != 0);
			bool flag6 = flag2 && val3.Length != 0 && (num3 != 0 || num4 != 0 || num5 != 0 || num6 != 0 || num7 != 0);
			TrackLaneData trackLaneData = default(TrackLaneData);
			UtilityLaneData utilityLaneData = default(UtilityLaneData);
			Bezier4x3 laneCurve = default(Bezier4x3);
			Bezier4x3 laneCurve2 = default(Bezier4x3);
			int2 val8 = default(int2);
			EdgeColor edgeColor = default(EdgeColor);
			NodeColor nodeColor = default(NodeColor);
			Color color = default(Color);
			Owner owner3 = default(Owner);
			for (int m = 0; m < nativeArray.Length; m++)
			{
				if (flag5)
				{
					PrefabRef prefabRef = val2[m];
					if (m_PrefabTrackLaneData.TryGetComponent(prefabRef.m_Prefab, ref trackLaneData))
					{
						if ((trackLaneData.m_TrackTypes & TrackTypes.Train) != 0 && num != 0)
						{
							nativeArray[m] = new LaneColor((byte)num, 0, 0);
							continue;
						}
						if ((trackLaneData.m_TrackTypes & TrackTypes.Tram) != 0 && num2 != 0)
						{
							nativeArray[m] = new LaneColor((byte)num2, 0, 0);
							continue;
						}
					}
				}
				if (flag6)
				{
					PrefabRef prefabRef2 = val2[m];
					if (m_PrefabUtilityLaneData.TryGetComponent(prefabRef2.m_Prefab, ref utilityLaneData))
					{
						int num20 = 0;
						float num21 = 0f;
						if ((utilityLaneData.m_UtilityTypes & UtilityTypes.LowVoltageLine) != UtilityTypes.None && num3 != 0)
						{
							num20 = num3;
							num21 = num8;
						}
						else if ((utilityLaneData.m_UtilityTypes & UtilityTypes.HighVoltageLine) != UtilityTypes.None && num4 != 0)
						{
							num20 = num4;
							num21 = num9;
						}
						else if ((utilityLaneData.m_UtilityTypes & UtilityTypes.WaterPipe) != UtilityTypes.None && num5 != 0)
						{
							num20 = num5;
							num21 = num10;
						}
						else if ((utilityLaneData.m_UtilityTypes & UtilityTypes.SewagePipe) != UtilityTypes.None && num6 != 0)
						{
							num20 = num6;
							num21 = num11;
						}
						else if ((utilityLaneData.m_UtilityTypes & UtilityTypes.Resource) != UtilityTypes.None && num7 != 0)
						{
							num20 = num7;
							num21 = num12;
						}
						if (num20 != 0)
						{
							Curve curve = nativeArray3[m];
							EdgeMapping edgeMapping = val[m];
							DynamicBuffer<SubFlow> val6 = val3[m];
							Owner owner = default(Owner);
							if (nativeArray2.Length != 0)
							{
								owner = nativeArray2[m];
							}
							if (val6.Length != 16)
							{
								val6.ResizeUninitialized(16);
							}
							NativeArray<SubFlow> val7 = val6.AsNativeArray();
							float warning = 0f;
							if (edgeMapping.m_Parent1 != Entity.Null)
							{
								if (m_EdgeData.HasComponent(edgeMapping.m_Parent1))
								{
									if (flag4)
									{
										if (edgeMapping.m_Parent2 != Entity.Null)
										{
											MathUtils.Divide(curve.m_Bezier, ref laneCurve, ref laneCurve2, 0.5f);
											GetOriginalEdge(laneCurve, ref edgeMapping.m_Parent1, ref edgeMapping.m_CurveDelta1);
											GetOriginalEdge(laneCurve2, ref edgeMapping.m_Parent2, ref edgeMapping.m_CurveDelta2);
										}
										else
										{
											GetOriginalEdge(curve.m_Bezier, ref edgeMapping.m_Parent1, ref edgeMapping.m_CurveDelta1);
										}
									}
									if (num20 == num3 || num20 == num4)
									{
										FillEdgeFlow(GetElectricityFlow(), val7, edgeMapping, out warning);
									}
									else if (num20 == num5)
									{
										FillEdgeFlow(GetWaterFlow(), val7, edgeMapping, out warning);
									}
									else if (num20 == num6)
									{
										FillEdgeFlow(GetSewageFlow(), val7, edgeMapping, out warning);
									}
									else if (num20 == num7)
									{
										FillEdgeFlow(GetResourceFlow(), val7, edgeMapping, out warning);
									}
									else
									{
										CollectionUtils.Fill<SubFlow>(val7, default(SubFlow));
									}
								}
								else
								{
									if (flag4)
									{
										GetOriginalNode(ref edgeMapping.m_Parent1);
										GetOriginalEdge(curve.m_Bezier, ref edgeMapping.m_Parent2, ref edgeMapping.m_CurveDelta2);
									}
									if (num20 == num3 || num20 == num4)
									{
										FillNodeFlow(GetElectricityFlow(), val7, edgeMapping, out warning);
									}
									else if (num20 == num5)
									{
										FillNodeFlow(GetWaterFlow(), val7, edgeMapping, out warning);
									}
									else if (num20 == num6)
									{
										FillNodeFlow(GetSewageFlow(), val7, edgeMapping, out warning);
									}
									else if (num20 == num7)
									{
										FillNodeFlow(GetResourceFlow(), val7, edgeMapping, out warning);
									}
									else
									{
										CollectionUtils.Fill<SubFlow>(val7, default(SubFlow));
									}
								}
							}
							else if (flag3)
							{
								if (num20 == num3 || num20 == num4)
								{
									FillBuildingFlow(GetElectricityFlow(), val7, owner.m_Owner, out warning);
								}
								else if (num20 == num5)
								{
									FillBuildingFlow(GetWaterFlow(), val7, owner.m_Owner, out warning);
								}
								else if (num20 == num6)
								{
									FillBuildingFlow(GetSewageFlow(), val7, owner.m_Owner, out warning);
								}
								else
								{
									CollectionUtils.Fill<SubFlow>(val7, default(SubFlow));
								}
							}
							else
							{
								num20 = 0;
							}
							if (num20 != 0)
							{
								((int2)(ref val8))._002Ector((int)val6[0].m_Value, (int)val6[15].m_Value);
								bool flag7 = (((val8.x ^ val8.y) & 0x80) != 0) & math.all(val8 != 0);
								int num22 = math.clamp(Mathf.RoundToInt(curve.m_Length * num21), 1, 255);
								int num23 = math.clamp(Mathf.RoundToInt(warning * 255f), 0, 255);
								num22 = math.select(num22, 2, num22 == 1 && flag7);
								nativeArray[m] = new LaneColor((byte)num20, (byte)num22, (byte)num23);
								continue;
							}
						}
					}
				}
				if (nativeArray2.Length != 0)
				{
					Owner owner2 = nativeArray2[m];
					if (nativeArray4.Length != 0)
					{
						if (m_EdgeColorData.TryGetComponent(owner2.m_Owner, ref edgeColor))
						{
							float2 val9 = math.lerp(float2.op_Implicit((float)(int)edgeColor.m_Value0), float2.op_Implicit((float)(int)edgeColor.m_Value1), nativeArray4[m].m_EdgeDelta);
							nativeArray[m] = new LaneColor(edgeColor.m_Index, (byte)Mathf.RoundToInt(val9.x), (byte)Mathf.RoundToInt(val9.y));
							continue;
						}
					}
					else if (nativeArray5.Length != 0)
					{
						if (m_NodeColorData.TryGetComponent(owner2.m_Owner, ref nodeColor))
						{
							nativeArray[m] = new LaneColor(nodeColor.m_Index, nodeColor.m_Value, nodeColor.m_Value);
							continue;
						}
					}
					else
					{
						PrefabRef prefabRef3 = val2[m];
						if ((m_PrefabNetLaneData[prefabRef3.m_Prefab].m_Flags & LaneFlags.Underground) == 0)
						{
							while (!m_ObjectColorData.TryGetComponent(owner2.m_Owner, ref color))
							{
								if (m_OwnerData.TryGetComponent(owner2.m_Owner, ref owner3))
								{
									owner2 = owner3;
									continue;
								}
								goto IL_09c9;
							}
							if (color.m_SubColor)
							{
								nativeArray[m] = new LaneColor(color.m_Index, color.m_Value, color.m_Value);
								continue;
							}
						}
					}
				}
				goto IL_09c9;
				IL_09c9:
				nativeArray[m] = default(LaneColor);
			}
		}

		private void GetOriginalEdge(Bezier4x3 laneCurve, ref Entity parent, ref float2 curveMapping)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			Temp temp = default(Temp);
			if (!m_TempData.TryGetComponent(parent, ref temp))
			{
				return;
			}
			Edge edge = default(Edge);
			Temp temp2 = default(Temp);
			Temp temp3 = default(Temp);
			if (temp.m_Original != Entity.Null)
			{
				parent = temp.m_Original;
			}
			else if (m_EdgeData.TryGetComponent(parent, ref edge) && m_TempData.TryGetComponent(edge.m_Start, ref temp2) && m_TempData.TryGetComponent(edge.m_End, ref temp3) && temp2.m_Original != Entity.Null && temp3.m_Original != Entity.Null)
			{
				Curve curve = default(Curve);
				Curve curve2 = default(Curve);
				if (m_CurveData.TryGetComponent(temp2.m_Original, ref curve))
				{
					parent = temp2.m_Original;
					MathUtils.Distance(((Bezier4x3)(ref curve.m_Bezier)).xz, ((float3)(ref laneCurve.a)).xz, ref curveMapping.x);
					MathUtils.Distance(((Bezier4x3)(ref curve.m_Bezier)).xz, ((float3)(ref laneCurve.d)).xz, ref curveMapping.y);
				}
				else if (m_CurveData.TryGetComponent(temp3.m_Original, ref curve2))
				{
					parent = temp3.m_Original;
					MathUtils.Distance(((Bezier4x3)(ref curve2.m_Bezier)).xz, ((float3)(ref laneCurve.a)).xz, ref curveMapping.x);
					MathUtils.Distance(((Bezier4x3)(ref curve2.m_Bezier)).xz, ((float3)(ref laneCurve.d)).xz, ref curveMapping.y);
				}
			}
		}

		private void GetOriginalNode(ref Entity parent)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			Temp temp = default(Temp);
			if (m_TempData.TryGetComponent(parent, ref temp))
			{
				parent = temp.m_Original;
			}
		}

		private void FillEdgeFlow<T>(T impl, NativeArray<SubFlow> flowArray, EdgeMapping edgeMapping, out float warning) where T : struct, IFlowImplementation
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			if (edgeMapping.m_Parent2 != Entity.Null)
			{
				FillEdgeFlow(impl, flowArray.GetSubArray(0, 8), edgeMapping.m_Parent1, edgeMapping.m_CurveDelta1, out warning);
				FillEdgeFlow(impl, flowArray.GetSubArray(8, 8), edgeMapping.m_Parent2, edgeMapping.m_CurveDelta2, out warning);
			}
			else
			{
				FillEdgeFlow(impl, flowArray, edgeMapping.m_Parent1, edgeMapping.m_CurveDelta1, out warning);
			}
		}

		private unsafe void FillEdgeFlow<T>(T impl, NativeArray<SubFlow> flows, Entity edge, float2 curveMapping, out float warning) where T : struct, IFlowImplementation
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_03de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			Edge edge2 = default(Edge);
			if (m_EdgeData.TryGetComponent(edge, ref edge2) && impl.TryGetFlowNode(edge, out var flowNode) && impl.TryGetFlowNode(edge2.m_Start, out var flowNode2) && impl.TryGetFlowNode(edge2.m_End, out var flowNode3) && impl.TryGetFlowEdge(flowNode2, flowNode, out var flow, out var capacity, out var warning2) && impl.TryGetFlowEdge(flowNode, flowNode3, out var flow2, out var capacity2, out var warning3))
			{
				capacity = math.max(1, capacity);
				if (curveMapping.y < curveMapping.x)
				{
					capacity2 = -flow2;
					int num = -flow;
					flow = capacity2;
					flow2 = num;
				}
				int* ptr = stackalloc int[flows.Length];
				DynamicBuffer<ConnectedNode> val = default(DynamicBuffer<ConnectedNode>);
				float warning4;
				if (m_ConnectedNodes.TryGetBuffer(edge, ref val))
				{
					Enumerator<ConnectedNode> enumerator = val.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							ConnectedNode current = enumerator.Current;
							if (impl.TryGetFlowNode(current.m_Node, out var flowNode4) && impl.TryGetFlowEdge(flowNode4, flowNode, out var flow3, out capacity2, out warning4))
							{
								AddTempFlow(flow3, current.m_CurvePosition, ptr, flows.Length, curveMapping);
							}
						}
					}
					finally
					{
						((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
					}
				}
				DynamicBuffer<Game.Objects.SubObject> val2 = default(DynamicBuffer<Game.Objects.SubObject>);
				if (impl.subObjects && m_SubObjects.TryGetBuffer(edge, ref val2))
				{
					Enumerator<Game.Objects.SubObject> enumerator2 = val2.GetEnumerator();
					try
					{
						Curve curve = default(Curve);
						Transform transform = default(Transform);
						float curvePosition = default(float);
						while (enumerator2.MoveNext())
						{
							Game.Objects.SubObject current2 = enumerator2.Current;
							if (impl.TryGetFlowNode(current2.m_SubObject, out var flowNode5) && impl.TryGetFlowEdge(flowNode5, flowNode, out var flow4, out capacity2, out warning4) && m_CurveData.TryGetComponent(edge, ref curve) && m_TransformData.TryGetComponent(current2.m_SubObject, ref transform))
							{
								MathUtils.Distance(new Segment(curve.m_Bezier.a, curve.m_Bezier.d), transform.m_Position, ref curvePosition);
								AddTempFlow(flow4, curvePosition, ptr, flows.Length, curveMapping);
							}
						}
					}
					finally
					{
						((IDisposable)enumerator2/*cast due to .constrained prefix*/).Dispose();
					}
				}
				DynamicBuffer<ConnectedBuilding> val3 = default(DynamicBuffer<ConnectedBuilding>);
				if (impl.connectedBuildings && impl.TryGetFlowEdge(flowNode, impl.sinkNode, out var flow5, out capacity2, out warning4) && m_ConnectedBuildings.TryGetBuffer(edge, ref val3))
				{
					int totalDemand = 0;
					Enumerator<ConnectedBuilding> enumerator3 = val3.GetEnumerator();
					try
					{
						while (enumerator3.MoveNext())
						{
							impl.GetConsumption(enumerator3.Current.m_Building, out var wantedConsumption, out capacity2, out warning4);
							totalDemand += wantedConsumption;
						}
					}
					finally
					{
						((IDisposable)enumerator3/*cast due to .constrained prefix*/).Dispose();
					}
					enumerator3 = val3.GetEnumerator();
					try
					{
						while (enumerator3.MoveNext())
						{
							ConnectedBuilding current3 = enumerator3.Current;
							impl.GetConsumption(current3.m_Building, out var wantedConsumption2, out capacity2, out warning4);
							AddTempFlow(-FlowUtils.ConsumeFromTotal(wantedConsumption2, ref flow5, ref totalDemand), m_BuildingData[current3.m_Building].m_CurvePosition, ptr, flows.Length, curveMapping);
						}
					}
					finally
					{
						((IDisposable)enumerator3/*cast due to .constrained prefix*/).Dispose();
					}
				}
				int num2 = flow;
				for (int i = 0; i < flows.Length; i++)
				{
					num2 += ptr[i];
					flows[i] = GetSubFlow(impl.multiplier * num2, capacity);
				}
				if (MathUtils.Max(curveMapping) == 1f)
				{
					flows[flows.Length - 1] = GetSubFlow(impl.multiplier * flow2, capacity);
				}
				warning = math.max(warning2, warning3);
			}
			else
			{
				CollectionUtils.Fill<SubFlow>(flows, default(SubFlow));
				warning = 0f;
			}
		}

		private unsafe static void AddTempFlow(int flow, float curvePosition, int* tempFlows, int length, float2 curveMapping)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			float num = curveMapping.y - curveMapping.x;
			if (num != 0f)
			{
				float num2 = (curvePosition - curveMapping.x) / num;
				if (num2 < 0f)
				{
					*tempFlows += flow;
				}
				else if (num2 < 1f)
				{
					int num3 = math.clamp(Mathf.RoundToInt(num2 * (float)(length - 1)), 1, length - 1);
					tempFlows[num3] += flow;
				}
			}
			else if (curvePosition < curveMapping.x)
			{
				*tempFlows += flow;
			}
		}

		private static SubFlow GetSubFlow(int flow, int capacity)
		{
			int num = 127 * flow / capacity;
			return new SubFlow
			{
				m_Value = (sbyte)((num != 0) ? math.clamp(num, -127, 127) : math.clamp(flow, -1, 1))
			};
		}

		private void FillNodeFlow<T>(T impl, NativeArray<SubFlow> flows, EdgeMapping edgeMapping, out float warning) where T : struct, IFlowImplementation
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			FillNodeFlow(impl, flows, edgeMapping.m_Parent1, edgeMapping.m_Parent2, edgeMapping.m_CurveDelta1, out warning);
		}

		private void FillNodeFlow<T>(T impl, NativeArray<SubFlow> flows, Entity node, Entity edge, float2 curveMapping, out float warning) where T : struct, IFlowImplementation
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			float num = 0f;
			if (impl.TryGetFlowNode(node, out var flowNode) && impl.TryGetFlowNode(edge, out var flowNode2))
			{
				if (impl.TryGetFlowEdge(flowNode, flowNode2, out var flow, out var capacity, out warning))
				{
					num = (float)flow / (float)capacity;
				}
				else if (impl.TryGetFlowEdge(flowNode2, flowNode, out flow, out capacity, out warning))
				{
					num = (float)(-flow) / (float)capacity;
				}
			}
			else
			{
				warning = 0f;
			}
			num = math.select(num, 0f - num, curveMapping.y < curveMapping.x);
			SubFlow subFlow = GetSubFlow((float)impl.multiplier * num);
			CollectionUtils.Fill<SubFlow>(flows, subFlow);
		}

		private void FillBuildingFlow<T>(T impl, NativeArray<SubFlow> flows, Entity building, out float warning) where T : struct, IFlowImplementation
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			impl.GetConsumption(building, out var _, out var fulfilledConsumption, out warning);
			float num = (0f - (float)fulfilledConsumption) / (float)(10000 + math.abs(fulfilledConsumption));
			SubFlow subFlow = GetSubFlow((float)impl.multiplier * num);
			CollectionUtils.Fill<SubFlow>(flows, subFlow);
		}

		private SubFlow GetSubFlow(float value)
		{
			int num = math.clamp(Mathf.RoundToInt(value * 127f), -127, 127);
			num = math.select(num, 1, num == 0 && value > 0f);
			num = math.select(num, -1, num == 0 && value < 0f);
			return new SubFlow
			{
				m_Value = (sbyte)num
			};
		}

		private ElectricityFlow GetElectricityFlow()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			return new ElectricityFlow
			{
				sinkNode = m_ElectricitySinkNode,
				m_NodeConnectionData = m_ElectricityNodeConnectionData,
				m_FlowEdgeData = m_ElectricityFlowEdgeData,
				m_BuildingConnectionData = m_ElectricityBuildingConnectionData,
				m_ConsumerData = m_ElectricityConsumerData,
				m_ConnectedFlowEdges = m_ConnectedFlowEdges
			};
		}

		private WaterFlow GetWaterFlow()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			return new WaterFlow
			{
				sinkNode = m_WaterSinkNode,
				m_NodeConnectionData = m_WaterPipeNodeConnectionData,
				m_FlowEdgeData = m_WaterPipeEdgeData,
				m_BuildingConnectionData = m_WaterPipeBuildingConnectionData,
				m_ConsumerData = m_WaterConsumerData,
				m_ConnectedFlowEdges = m_ConnectedFlowEdges,
				m_MaxToleratedPollution = m_WaterPipeParameters.m_MaxToleratedPollution
			};
		}

		private SewageFlow GetSewageFlow()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			return new SewageFlow
			{
				sinkNode = m_WaterSinkNode,
				m_NodeConnectionData = m_WaterPipeNodeConnectionData,
				m_FlowEdgeData = m_WaterPipeEdgeData,
				m_BuildingConnectionData = m_WaterPipeBuildingConnectionData,
				m_ConsumerData = m_WaterConsumerData,
				m_ConnectedFlowEdges = m_ConnectedFlowEdges
			};
		}

		private ResourceFlow GetResourceFlow()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			return new ResourceFlow
			{
				sinkNode = m_WaterSinkNode,
				m_EdgeData = m_EdgeData,
				m_ResourceConnectionData = m_ResourceConnectionData,
				m_ConnectedEdges = m_ConnectedEdges
			};
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
		public ComponentTypeHandle<InfomodeActive> __Game_Prefabs_InfomodeActive_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewCoverageData> __Game_Prefabs_InfoviewCoverageData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewAvailabilityData> __Game_Prefabs_InfoviewAvailabilityData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewNetGeometryData> __Game_Prefabs_InfoviewNetGeometryData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewNetStatusData> __Game_Prefabs_InfoviewNetStatusData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TrainTrack> __Game_Net_TrainTrack_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TramTrack> __Game_Net_TramTrack_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Waterway> __Game_Net_Waterway_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<SubwayTrack> __Game_Net_SubwayTrack_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<NetCondition> __Game_Net_NetCondition_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Road> __Game_Net_Road_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.Pollution> __Game_Net_Pollution_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<EdgeGeometry> __Game_Net_EdgeGeometry_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Game.Net.ServiceCoverage> __Game_Net_ServiceCoverage_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<ResourceAvailability> __Game_Net_ResourceAvailability_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<LandValue> __Game_Net_LandValue_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Node> __Game_Net_Node_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ResourceData> __Game_Prefabs_ResourceData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ZonePropertiesData> __Game_Prefabs_ZonePropertiesData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathwayData> __Game_Prefabs_PathwayData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ProcessEstimate> __Game_Zones_ProcessEstimate_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.ServiceCoverage> __Game_Net_ServiceCoverage_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ResourceAvailability> __Game_Net_ResourceAvailability_RO_BufferLookup;

		[ReadOnly]
		public ComponentTypeHandle<Edge> __Game_Net_Edge_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		public ComponentTypeHandle<EdgeColor> __Game_Net_EdgeColor_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<EdgeColor> __Game_Net_EdgeColor_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> __Game_Net_StartNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> __Game_Net_EndNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public BufferTypeHandle<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferTypeHandle;

		public ComponentTypeHandle<NodeColor> __Game_Net_NodeColor_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<NodeColor> __Game_Net_NodeColor_RO_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<StartNodeGeometry> __Game_Net_StartNodeGeometry_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<EndNodeGeometry> __Game_Net_EndNodeGeometry_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Curve> __Game_Net_Curve_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<EdgeLane> __Game_Net_EdgeLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<NodeLane> __Game_Net_NodeLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.TrackLane> __Game_Net_TrackLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.UtilityLane> __Game_Net_UtilityLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.SecondaryLane> __Game_Net_SecondaryLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<EdgeMapping> __Game_Net_EdgeMapping_RO_ComponentTypeHandle;

		public ComponentTypeHandle<LaneColor> __Game_Net_LaneColor_RW_ComponentTypeHandle;

		public BufferTypeHandle<SubFlow> __Game_Net_SubFlow_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ResourceConnection> __Game_Net_ResourceConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Color> __Game_Objects_Color_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityNodeConnection> __Game_Simulation_ElectricityNodeConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityFlowEdge> __Game_Simulation_ElectricityFlowEdge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityBuildingConnection> __Game_Simulation_ElectricityBuildingConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WaterPipeNodeConnection> __Game_Simulation_WaterPipeNodeConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WaterPipeEdge> __Game_Simulation_WaterPipeEdge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WaterPipeBuildingConnection> __Game_Simulation_WaterPipeBuildingConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityConsumer> __Game_Buildings_ElectricityConsumer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WaterConsumer> __Game_Buildings_WaterConsumer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrackLaneData> __Game_Prefabs_TrackLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<UtilityLaneData> __Game_Prefabs_UtilityLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneData> __Game_Prefabs_NetLaneData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedNode> __Game_Net_ConnectedNode_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedBuilding> __Game_Buildings_ConnectedBuilding_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedFlowEdge> __Game_Simulation_ConnectedFlowEdge_RO_BufferLookup;

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
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Prefabs_InfomodeActive_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfomodeActive>(true);
			__Game_Prefabs_InfoviewCoverageData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfoviewCoverageData>(true);
			__Game_Prefabs_InfoviewAvailabilityData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfoviewAvailabilityData>(true);
			__Game_Prefabs_InfoviewNetGeometryData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfoviewNetGeometryData>(true);
			__Game_Prefabs_InfoviewNetStatusData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfoviewNetStatusData>(true);
			__Game_Net_TrainTrack_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TrainTrack>(true);
			__Game_Net_TramTrack_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TramTrack>(true);
			__Game_Net_Waterway_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Waterway>(true);
			__Game_Net_SubwayTrack_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<SubwayTrack>(true);
			__Game_Net_NetCondition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NetCondition>(true);
			__Game_Net_Road_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Road>(true);
			__Game_Net_Pollution_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.Pollution>(true);
			__Game_Net_EdgeGeometry_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EdgeGeometry>(true);
			__Game_Net_ServiceCoverage_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Game.Net.ServiceCoverage>(true);
			__Game_Net_ResourceAvailability_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ResourceAvailability>(true);
			__Game_Net_LandValue_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LandValue>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_Node_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Node>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Prefabs_ResourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceData>(true);
			__Game_Prefabs_ZonePropertiesData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ZonePropertiesData>(true);
			__Game_Prefabs_PathwayData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathwayData>(true);
			__Game_Zones_ProcessEstimate_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ProcessEstimate>(true);
			__Game_Net_ServiceCoverage_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.ServiceCoverage>(true);
			__Game_Net_ResourceAvailability_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ResourceAvailability>(true);
			__Game_Net_Edge_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Edge>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Net_EdgeColor_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EdgeColor>(false);
			__Game_Net_EdgeColor_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeColor>(true);
			__Game_Net_StartNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StartNodeGeometry>(true);
			__Game_Net_EndNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EndNodeGeometry>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Net_ConnectedEdge_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ConnectedEdge>(true);
			__Game_Net_NodeColor_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NodeColor>(false);
			__Game_Net_NodeColor_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NodeColor>(true);
			__Game_Net_StartNodeGeometry_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<StartNodeGeometry>(true);
			__Game_Net_EndNodeGeometry_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EndNodeGeometry>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Net_Curve_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Curve>(true);
			__Game_Net_EdgeLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EdgeLane>(true);
			__Game_Net_NodeLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NodeLane>(true);
			__Game_Net_TrackLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.TrackLane>(true);
			__Game_Net_UtilityLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.UtilityLane>(true);
			__Game_Net_SecondaryLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.SecondaryLane>(true);
			__Game_Net_EdgeMapping_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EdgeMapping>(true);
			__Game_Net_LaneColor_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<LaneColor>(false);
			__Game_Net_SubFlow_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubFlow>(false);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_ResourceConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ResourceConnection>(true);
			__Game_Objects_Color_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Color>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Simulation_ElectricityNodeConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityNodeConnection>(true);
			__Game_Simulation_ElectricityFlowEdge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityFlowEdge>(true);
			__Game_Simulation_ElectricityBuildingConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityBuildingConnection>(true);
			__Game_Simulation_WaterPipeNodeConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterPipeNodeConnection>(true);
			__Game_Simulation_WaterPipeEdge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterPipeEdge>(true);
			__Game_Simulation_WaterPipeBuildingConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterPipeBuildingConnection>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Buildings_ElectricityConsumer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityConsumer>(true);
			__Game_Buildings_WaterConsumer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterConsumer>(true);
			__Game_Prefabs_TrackLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrackLaneData>(true);
			__Game_Prefabs_UtilityLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<UtilityLaneData>(true);
			__Game_Prefabs_NetLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneData>(true);
			__Game_Net_ConnectedNode_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedNode>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
			__Game_Buildings_ConnectedBuilding_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedBuilding>(true);
			__Game_Simulation_ConnectedFlowEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedFlowEdge>(true);
		}
	}

	private EntityQuery m_ZonePreferenceParameterGroup;

	private EntityQuery m_EdgeQuery;

	private EntityQuery m_NodeQuery;

	private EntityQuery m_LaneQuery;

	private EntityQuery m_InfomodeQuery;

	private EntityQuery m_ProcessQuery;

	private ToolSystem m_ToolSystem;

	private ZoneToolSystem m_ZoneToolSystem;

	private ObjectToolSystem m_ObjectToolSystem;

	private IndustrialDemandSystem m_IndustrialDemandSystem;

	private PrefabSystem m_PrefabSystem;

	private ResourceSystem m_ResourceSystem;

	private GroundPollutionSystem m_GroundPollutionSystem;

	private ElectricityFlowSystem m_ElectricityFlowSystem;

	private WaterPipeFlowSystem m_WaterPipeFlowSystem;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_1733354667_0;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Expected O, but got Unknown
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_ZoneToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ZoneToolSystem>();
		m_ObjectToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ObjectToolSystem>();
		m_IndustrialDemandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IndustrialDemandSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		m_GroundPollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GroundPollutionSystem>();
		m_ElectricityFlowSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ElectricityFlowSystem>();
		m_WaterPipeFlowSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterPipeFlowSystem>();
		m_ZonePreferenceParameterGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ZonePreferenceData>() });
		m_EdgeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Edge>(),
			ComponentType.ReadWrite<EdgeColor>(),
			ComponentType.Exclude<Deleted>()
		});
		m_NodeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Node>(),
			ComponentType.ReadWrite<NodeColor>(),
			ComponentType.Exclude<Deleted>()
		});
		m_LaneQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Lane>(),
			ComponentType.ReadWrite<LaneColor>(),
			ComponentType.Exclude<Hidden>(),
			ComponentType.Exclude<Deleted>()
		});
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<InfomodeActive>() };
		val.Any = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<InfoviewCoverageData>(),
			ComponentType.ReadOnly<InfoviewAvailabilityData>(),
			ComponentType.ReadOnly<InfoviewNetGeometryData>(),
			ComponentType.ReadOnly<InfoviewNetStatusData>()
		};
		val.None = (ComponentType[])(object)new ComponentType[0];
		array[0] = val;
		m_InfomodeQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_ProcessQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<IndustrialProcessData>() });
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_0519: Unknown result type (might be due to invalid IL or missing references)
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0525: Unknown result type (might be due to invalid IL or missing references)
		//IL_052a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0573: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_0590: Unknown result type (might be due to invalid IL or missing references)
		//IL_0595: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_060c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0611: Unknown result type (might be due to invalid IL or missing references)
		//IL_0629: Unknown result type (might be due to invalid IL or missing references)
		//IL_062e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0646: Unknown result type (might be due to invalid IL or missing references)
		//IL_064b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0663: Unknown result type (might be due to invalid IL or missing references)
		//IL_0668: Unknown result type (might be due to invalid IL or missing references)
		//IL_0680: Unknown result type (might be due to invalid IL or missing references)
		//IL_0685: Unknown result type (might be due to invalid IL or missing references)
		//IL_069d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0711: Unknown result type (might be due to invalid IL or missing references)
		//IL_0716: Unknown result type (might be due to invalid IL or missing references)
		//IL_072e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0733: Unknown result type (might be due to invalid IL or missing references)
		//IL_074b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0750: Unknown result type (might be due to invalid IL or missing references)
		//IL_0768: Unknown result type (might be due to invalid IL or missing references)
		//IL_076d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0791: Unknown result type (might be due to invalid IL or missing references)
		//IL_0796: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0805: Unknown result type (might be due to invalid IL or missing references)
		//IL_080a: Unknown result type (might be due to invalid IL or missing references)
		//IL_082e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0833: Unknown result type (might be due to invalid IL or missing references)
		//IL_084b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0850: Unknown result type (might be due to invalid IL or missing references)
		//IL_0868: Unknown result type (might be due to invalid IL or missing references)
		//IL_086d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0885: Unknown result type (might be due to invalid IL or missing references)
		//IL_088a: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0916: Unknown result type (might be due to invalid IL or missing references)
		//IL_091b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0933: Unknown result type (might be due to invalid IL or missing references)
		//IL_0938: Unknown result type (might be due to invalid IL or missing references)
		//IL_0950: Unknown result type (might be due to invalid IL or missing references)
		//IL_0955: Unknown result type (might be due to invalid IL or missing references)
		//IL_096d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0972: Unknown result type (might be due to invalid IL or missing references)
		//IL_0979: Unknown result type (might be due to invalid IL or missing references)
		//IL_097a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0992: Unknown result type (might be due to invalid IL or missing references)
		//IL_0997: Unknown result type (might be due to invalid IL or missing references)
		//IL_09af: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_09cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a06: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a23: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a28: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a40: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a62: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a97: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b10: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b28: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b62: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b67: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b84: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bbe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bdb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c10: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c15: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c32: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c67: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c84: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c89: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cbe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cc3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d03: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d09: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d11: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d13: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d15: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d17: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d21: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d26: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d32: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d37: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d43: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d46: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d52: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d56: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d58: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d64: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d71: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d86: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)m_ToolSystem.activeInfoview == (Object)null || (((EntityQuery)(ref m_EdgeQuery)).IsEmptyIgnoreFilter && ((EntityQuery)(ref m_NodeQuery)).IsEmptyIgnoreFilter))
		{
			return;
		}
		ZonePreferenceData zonePreferences = ((((EntityQuery)(ref m_ZonePreferenceParameterGroup)).CalculateEntityCount() > 0) ? ((EntityQuery)(ref m_ZonePreferenceParameterGroup)).GetSingleton<ZonePreferenceData>() : default(ZonePreferenceData));
		JobHandle val = default(JobHandle);
		NativeList<ArchetypeChunk> infomodeChunks = ((EntityQuery)(ref m_InfomodeQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val);
		Entity zonePrefab = Entity.Null;
		if (m_ToolSystem.activeTool == m_ZoneToolSystem && (Object)(object)m_ZoneToolSystem.prefab != (Object)null)
		{
			zonePrefab = m_PrefabSystem.GetEntity(m_ZoneToolSystem.prefab);
		}
		else if (m_ToolSystem.activeTool == m_ObjectToolSystem && (Object)(object)m_ObjectToolSystem.prefab != (Object)null)
		{
			PlaceholderBuilding component2;
			if (m_ObjectToolSystem.prefab.TryGet<SignatureBuilding>(out var component) && (Object)(object)component.m_ZoneType != (Object)null)
			{
				zonePrefab = m_PrefabSystem.GetEntity(component.m_ZoneType);
			}
			else if (m_ObjectToolSystem.prefab.TryGet<PlaceholderBuilding>(out component2) && (Object)(object)component2.m_ZoneType != (Object)null)
			{
				zonePrefab = m_PrefabSystem.GetEntity(component2.m_ZoneType);
			}
		}
		JobHandle dependencies;
		JobHandle deps;
		JobHandle deps2;
		JobHandle val2 = default(JobHandle);
		UpdateEdgeColorsJob updateEdgeColorsJob = new UpdateEdgeColorsJob
		{
			m_InfomodeChunks = infomodeChunks,
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InfomodeActiveType = InternalCompilerInterface.GetComponentTypeHandle<InfomodeActive>(ref __TypeHandle.__Game_Prefabs_InfomodeActive_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InfoviewCoverageType = InternalCompilerInterface.GetComponentTypeHandle<InfoviewCoverageData>(ref __TypeHandle.__Game_Prefabs_InfoviewCoverageData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InfoviewAvailabilityType = InternalCompilerInterface.GetComponentTypeHandle<InfoviewAvailabilityData>(ref __TypeHandle.__Game_Prefabs_InfoviewAvailabilityData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InfoviewNetGeometryType = InternalCompilerInterface.GetComponentTypeHandle<InfoviewNetGeometryData>(ref __TypeHandle.__Game_Prefabs_InfoviewNetGeometryData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InfoviewNetStatusType = InternalCompilerInterface.GetComponentTypeHandle<InfoviewNetStatusData>(ref __TypeHandle.__Game_Prefabs_InfoviewNetStatusData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TrainTrackType = InternalCompilerInterface.GetComponentTypeHandle<TrainTrack>(ref __TypeHandle.__Game_Net_TrainTrack_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TramTrackType = InternalCompilerInterface.GetComponentTypeHandle<TramTrack>(ref __TypeHandle.__Game_Net_TramTrack_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WaterwayType = InternalCompilerInterface.GetComponentTypeHandle<Waterway>(ref __TypeHandle.__Game_Net_Waterway_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubwayTrackType = InternalCompilerInterface.GetComponentTypeHandle<SubwayTrack>(ref __TypeHandle.__Game_Net_SubwayTrack_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NetConditionType = InternalCompilerInterface.GetComponentTypeHandle<NetCondition>(ref __TypeHandle.__Game_Net_NetCondition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RoadType = InternalCompilerInterface.GetComponentTypeHandle<Road>(ref __TypeHandle.__Game_Net_Road_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PollutionType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.Pollution>(ref __TypeHandle.__Game_Net_Pollution_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeGeometryType = InternalCompilerInterface.GetComponentTypeHandle<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceCoverageType = InternalCompilerInterface.GetBufferTypeHandle<Game.Net.ServiceCoverage>(ref __TypeHandle.__Game_Net_ServiceCoverage_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceAvailabilityType = InternalCompilerInterface.GetBufferTypeHandle<ResourceAvailability>(ref __TypeHandle.__Game_Net_ResourceAvailability_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LandValues = InternalCompilerInterface.GetComponentLookup<LandValue>(ref __TypeHandle.__Game_Net_LandValue_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Edges = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Nodes = InternalCompilerInterface.GetComponentLookup<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Temps = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ZonePropertiesDatas = InternalCompilerInterface.GetComponentLookup<ZonePropertiesData>(ref __TypeHandle.__Game_Prefabs_ZonePropertiesData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPathwayData = InternalCompilerInterface.GetComponentLookup<PathwayData>(ref __TypeHandle.__Game_Prefabs_PathwayData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ProcessEstimates = InternalCompilerInterface.GetBufferLookup<ProcessEstimate>(ref __TypeHandle.__Game_Zones_ProcessEstimate_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceCoverageData = InternalCompilerInterface.GetBufferLookup<Game.Net.ServiceCoverage>(ref __TypeHandle.__Game_Net_ServiceCoverage_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceAvailabilityData = InternalCompilerInterface.GetBufferLookup<ResourceAvailability>(ref __TypeHandle.__Game_Net_ResourceAvailability_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeType = InternalCompilerInterface.GetComponentTypeHandle<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ColorType = InternalCompilerInterface.GetComponentTypeHandle<EdgeColor>(ref __TypeHandle.__Game_Net_EdgeColor_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ZonePrefab = zonePrefab,
			m_ResourcePrefabs = m_ResourceSystem.GetPrefabs(),
			m_PollutionMap = m_GroundPollutionSystem.GetMap(readOnly: true, out dependencies),
			m_IndustrialDemands = m_IndustrialDemandSystem.GetBuildingDemands(out deps),
			m_StorageDemands = m_IndustrialDemandSystem.GetStorageBuildingDemands(out deps2),
			m_Processes = ((EntityQuery)(ref m_ProcessQuery)).ToComponentDataListAsync<IndustrialProcessData>(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val2),
			m_ZonePreferences = zonePreferences
		};
		UpdateNodeColorsJob updateNodeColorsJob = new UpdateNodeColorsJob
		{
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ColorData = InternalCompilerInterface.GetComponentLookup<EdgeColor>(ref __TypeHandle.__Game_Net_EdgeColor_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StartNodeGeometryData = InternalCompilerInterface.GetComponentLookup<StartNodeGeometry>(ref __TypeHandle.__Game_Net_StartNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EndNodeGeometryData = InternalCompilerInterface.GetComponentLookup<EndNodeGeometry>(ref __TypeHandle.__Game_Net_EndNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InfomodeChunks = infomodeChunks,
			m_InfomodeActiveType = InternalCompilerInterface.GetComponentTypeHandle<InfomodeActive>(ref __TypeHandle.__Game_Prefabs_InfomodeActive_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InfoviewNetGeometryType = InternalCompilerInterface.GetComponentTypeHandle<InfoviewNetGeometryData>(ref __TypeHandle.__Game_Prefabs_InfoviewNetGeometryData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InfoviewNetStatusType = InternalCompilerInterface.GetComponentTypeHandle<InfoviewNetStatusData>(ref __TypeHandle.__Game_Prefabs_InfoviewNetStatusData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TrainTrackType = InternalCompilerInterface.GetComponentTypeHandle<TrainTrack>(ref __TypeHandle.__Game_Net_TrainTrack_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TramTrackType = InternalCompilerInterface.GetComponentTypeHandle<TramTrack>(ref __TypeHandle.__Game_Net_TramTrack_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WaterwayType = InternalCompilerInterface.GetComponentTypeHandle<Waterway>(ref __TypeHandle.__Game_Net_Waterway_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubwayTrackType = InternalCompilerInterface.GetComponentTypeHandle<SubwayTrack>(ref __TypeHandle.__Game_Net_SubwayTrack_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NetConditionType = InternalCompilerInterface.GetComponentTypeHandle<NetCondition>(ref __TypeHandle.__Game_Net_NetCondition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RoadType = InternalCompilerInterface.GetComponentTypeHandle<Road>(ref __TypeHandle.__Game_Net_Road_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PollutionType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.Pollution>(ref __TypeHandle.__Game_Net_Pollution_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedEdgeType = InternalCompilerInterface.GetBufferTypeHandle<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ColorType = InternalCompilerInterface.GetComponentTypeHandle<NodeColor>(ref __TypeHandle.__Game_Net_NodeColor_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef)
		};
		UpdateEdgeColors2Job updateEdgeColors2Job = new UpdateEdgeColors2Job
		{
			m_ColorData = InternalCompilerInterface.GetComponentLookup<NodeColor>(ref __TypeHandle.__Game_Net_NodeColor_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeType = InternalCompilerInterface.GetComponentTypeHandle<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_StartNodeGeometryType = InternalCompilerInterface.GetComponentTypeHandle<StartNodeGeometry>(ref __TypeHandle.__Game_Net_StartNodeGeometry_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EndNodeGeometryType = InternalCompilerInterface.GetComponentTypeHandle<EndNodeGeometry>(ref __TypeHandle.__Game_Net_EndNodeGeometry_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ColorType = InternalCompilerInterface.GetComponentTypeHandle<EdgeColor>(ref __TypeHandle.__Game_Net_EdgeColor_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef)
		};
		LaneColorJob obj = new LaneColorJob
		{
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurveType = InternalCompilerInterface.GetComponentTypeHandle<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeLaneType = InternalCompilerInterface.GetComponentTypeHandle<EdgeLane>(ref __TypeHandle.__Game_Net_EdgeLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NodeLaneType = InternalCompilerInterface.GetComponentTypeHandle<NodeLane>(ref __TypeHandle.__Game_Net_NodeLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TrackLaneType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.TrackLane>(ref __TypeHandle.__Game_Net_TrackLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UtilityLaneType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.UtilityLane>(ref __TypeHandle.__Game_Net_UtilityLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SecondaryLaneType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.SecondaryLane>(ref __TypeHandle.__Game_Net_SecondaryLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeMappingType = InternalCompilerInterface.GetComponentTypeHandle<EdgeMapping>(ref __TypeHandle.__Game_Net_EdgeMapping_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ColorType = InternalCompilerInterface.GetComponentTypeHandle<LaneColor>(ref __TypeHandle.__Game_Net_LaneColor_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubFlowType = InternalCompilerInterface.GetBufferTypeHandle<SubFlow>(ref __TypeHandle.__Game_Net_SubFlow_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InfomodeChunks = infomodeChunks,
			m_InfomodeActiveType = InternalCompilerInterface.GetComponentTypeHandle<InfomodeActive>(ref __TypeHandle.__Game_Prefabs_InfomodeActive_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InfoviewNetGeometryType = InternalCompilerInterface.GetComponentTypeHandle<InfoviewNetGeometryData>(ref __TypeHandle.__Game_Prefabs_InfoviewNetGeometryData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InfoviewNetStatusType = InternalCompilerInterface.GetComponentTypeHandle<InfoviewNetStatusData>(ref __TypeHandle.__Game_Prefabs_InfoviewNetStatusData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceConnectionData = InternalCompilerInterface.GetComponentLookup<Game.Net.ResourceConnection>(ref __TypeHandle.__Game_Net_ResourceConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NodeColorData = InternalCompilerInterface.GetComponentLookup<NodeColor>(ref __TypeHandle.__Game_Net_NodeColor_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeColorData = InternalCompilerInterface.GetComponentLookup<EdgeColor>(ref __TypeHandle.__Game_Net_EdgeColor_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectColorData = InternalCompilerInterface.GetComponentLookup<Color>(ref __TypeHandle.__Game_Objects_Color_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ElectricityNodeConnectionData = InternalCompilerInterface.GetComponentLookup<ElectricityNodeConnection>(ref __TypeHandle.__Game_Simulation_ElectricityNodeConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ElectricityFlowEdgeData = InternalCompilerInterface.GetComponentLookup<ElectricityFlowEdge>(ref __TypeHandle.__Game_Simulation_ElectricityFlowEdge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ElectricityBuildingConnectionData = InternalCompilerInterface.GetComponentLookup<ElectricityBuildingConnection>(ref __TypeHandle.__Game_Simulation_ElectricityBuildingConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WaterPipeNodeConnectionData = InternalCompilerInterface.GetComponentLookup<WaterPipeNodeConnection>(ref __TypeHandle.__Game_Simulation_WaterPipeNodeConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WaterPipeEdgeData = InternalCompilerInterface.GetComponentLookup<WaterPipeEdge>(ref __TypeHandle.__Game_Simulation_WaterPipeEdge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WaterPipeBuildingConnectionData = InternalCompilerInterface.GetComponentLookup<WaterPipeBuildingConnection>(ref __TypeHandle.__Game_Simulation_WaterPipeBuildingConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ElectricityConsumerData = InternalCompilerInterface.GetComponentLookup<ElectricityConsumer>(ref __TypeHandle.__Game_Buildings_ElectricityConsumer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WaterConsumerData = InternalCompilerInterface.GetComponentLookup<WaterConsumer>(ref __TypeHandle.__Game_Buildings_WaterConsumer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTrackLaneData = InternalCompilerInterface.GetComponentLookup<TrackLaneData>(ref __TypeHandle.__Game_Prefabs_TrackLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabUtilityLaneData = InternalCompilerInterface.GetComponentLookup<UtilityLaneData>(ref __TypeHandle.__Game_Prefabs_UtilityLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabNetLaneData = InternalCompilerInterface.GetComponentLookup<NetLaneData>(ref __TypeHandle.__Game_Prefabs_NetLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedNodes = InternalCompilerInterface.GetBufferLookup<ConnectedNode>(ref __TypeHandle.__Game_Net_ConnectedNode_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedBuildings = InternalCompilerInterface.GetBufferLookup<ConnectedBuilding>(ref __TypeHandle.__Game_Buildings_ConnectedBuilding_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedFlowEdges = InternalCompilerInterface.GetBufferLookup<ConnectedFlowEdge>(ref __TypeHandle.__Game_Simulation_ConnectedFlowEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ElectricitySinkNode = m_ElectricityFlowSystem.sinkNode,
			m_WaterSinkNode = m_WaterPipeFlowSystem.sinkNode,
			m_WaterPipeParameters = ((EntityQuery)(ref __query_1733354667_0)).GetSingleton<WaterPipeParameterData>()
		};
		JobHandle val3 = JobChunkExtensions.ScheduleParallel<UpdateEdgeColorsJob>(updateEdgeColorsJob, m_EdgeQuery, JobUtils.CombineDependencies(((SystemBase)this).Dependency, val, val2, dependencies, deps, deps2));
		JobHandle val4 = JobChunkExtensions.ScheduleParallel<UpdateNodeColorsJob>(updateNodeColorsJob, m_NodeQuery, val3);
		JobHandle val5 = JobChunkExtensions.ScheduleParallel<UpdateEdgeColors2Job>(updateEdgeColors2Job, m_EdgeQuery, val4);
		JobHandle val6 = JobChunkExtensions.ScheduleParallel<LaneColorJob>(obj, m_LaneQuery, val5);
		infomodeChunks.Dispose(val6);
		m_GroundPollutionSystem.AddReader(val3);
		m_IndustrialDemandSystem.AddReader(val3);
		m_ResourceSystem.AddPrefabsReader(val3);
		((SystemBase)this).Dependency = val6;
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
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<WaterPipeParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_1733354667_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
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
	public NetColorSystem()
	{
	}
}
