using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Colossal.Mathematics;
using Game.Common;
using Game.Economy;
using Game.Net;
using Game.Objects;
using Game.Rendering;
using Game.Simulation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Prefabs;

[CompilerGenerated]
public class BuildingInitializeSystem : GameSystemBase
{
	[BurstCompile]
	private struct FindConnectionRequirementsJob : IJobParallelFor
	{
		[ReadOnly]
		public ComponentTypeHandle<SpawnableBuildingData> m_SpawnableBuildingDataType;

		[ReadOnly]
		public ComponentTypeHandle<ServiceUpgradeData> m_ServiceUpgradeDataType;

		[ReadOnly]
		public ComponentTypeHandle<ExtractorFacilityData> m_ExtractorFacilityDataType;

		[ReadOnly]
		public ComponentTypeHandle<ConsumptionData> m_ConsumptionDataType;

		[ReadOnly]
		public ComponentTypeHandle<WorkplaceData> m_WorkplaceDataType;

		[ReadOnly]
		public ComponentTypeHandle<WaterPumpingStationData> m_WaterPumpingStationDataType;

		[ReadOnly]
		public ComponentTypeHandle<WaterTowerData> m_WaterTowerDataType;

		[ReadOnly]
		public ComponentTypeHandle<SewageOutletData> m_SewageOutletDataType;

		[ReadOnly]
		public ComponentTypeHandle<WastewaterTreatmentPlantData> m_WastewaterTreatmentPlantDataType;

		[ReadOnly]
		public ComponentTypeHandle<TransformerData> m_TransformerDataType;

		[ReadOnly]
		public ComponentTypeHandle<ParkingFacilityData> m_ParkingFacilityDataType;

		[ReadOnly]
		public ComponentTypeHandle<PublicTransportStationData> m_PublicTransportStationDataType;

		[ReadOnly]
		public ComponentTypeHandle<CargoTransportStationData> m_CargoTransportStationDataType;

		[ReadOnly]
		public ComponentTypeHandle<ParkData> m_ParkDataType;

		[ReadOnly]
		public BufferTypeHandle<SubNet> m_SubNetType;

		[ReadOnly]
		public BufferTypeHandle<SubObject> m_SubObjectType;

		[ReadOnly]
		public BufferTypeHandle<SubMesh> m_SubMeshType;

		public ComponentTypeHandle<BuildingData> m_BuildingDataType;

		public BufferTypeHandle<Effect> m_EffectType;

		[ReadOnly]
		public ComponentLookup<NetData> m_NetData;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> m_SpawnLocationData;

		[ReadOnly]
		public ComponentLookup<MeshData> m_MeshData;

		[ReadOnly]
		public ComponentLookup<EffectData> m_EffectData;

		[ReadOnly]
		public ComponentLookup<VFXData> m_VFXData;

		[ReadOnly]
		public BufferLookup<AudioSourceData> m_AudioSourceData;

		[ReadOnly]
		public ComponentLookup<AudioSpotData> m_AudioSpotData;

		[ReadOnly]
		public ComponentLookup<AudioEffectData> m_AudioEffectData;

		[ReadOnly]
		public BufferLookup<SubObject> m_SubObjects;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public NativeArray<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public BuildingConfigurationData m_BuildingConfigurationData;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0493: Unknown result type (might be due to invalid IL or missing references)
			//IL_0498: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a87: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a94: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab6: Unknown result type (might be due to invalid IL or missing references)
			//IL_051b: Unknown result type (might be due to invalid IL or missing references)
			//IL_053f: Unknown result type (might be due to invalid IL or missing references)
			//IL_054b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0586: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad8: Unknown result type (might be due to invalid IL or missing references)
			//IL_059e: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05de: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_083d: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0601: Unknown result type (might be due to invalid IL or missing references)
			//IL_060e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0613: Unknown result type (might be due to invalid IL or missing references)
			//IL_0618: Unknown result type (might be due to invalid IL or missing references)
			//IL_061d: Unknown result type (might be due to invalid IL or missing references)
			//IL_061f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0626: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b0e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b32: Unknown result type (might be due to invalid IL or missing references)
			//IL_084a: Unknown result type (might be due to invalid IL or missing references)
			//IL_084f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0851: Unknown result type (might be due to invalid IL or missing references)
			//IL_085b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0860: Unknown result type (might be due to invalid IL or missing references)
			//IL_0865: Unknown result type (might be due to invalid IL or missing references)
			//IL_086a: Unknown result type (might be due to invalid IL or missing references)
			//IL_086c: Unknown result type (might be due to invalid IL or missing references)
			//IL_086e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0870: Unknown result type (might be due to invalid IL or missing references)
			//IL_0875: Unknown result type (might be due to invalid IL or missing references)
			//IL_087a: Unknown result type (might be due to invalid IL or missing references)
			//IL_087e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0883: Unknown result type (might be due to invalid IL or missing references)
			//IL_0894: Unknown result type (might be due to invalid IL or missing references)
			//IL_0899: Unknown result type (might be due to invalid IL or missing references)
			//IL_089e: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0902: Unknown result type (might be due to invalid IL or missing references)
			//IL_0907: Unknown result type (might be due to invalid IL or missing references)
			//IL_0909: Unknown result type (might be due to invalid IL or missing references)
			//IL_091d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0922: Unknown result type (might be due to invalid IL or missing references)
			//IL_0927: Unknown result type (might be due to invalid IL or missing references)
			//IL_092c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0930: Unknown result type (might be due to invalid IL or missing references)
			//IL_0937: Unknown result type (might be due to invalid IL or missing references)
			//IL_0953: Unknown result type (might be due to invalid IL or missing references)
			//IL_095a: Unknown result type (might be due to invalid IL or missing references)
			//IL_07de: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0806: Unknown result type (might be due to invalid IL or missing references)
			//IL_080b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0634: Unknown result type (might be due to invalid IL or missing references)
			//IL_0636: Unknown result type (might be due to invalid IL or missing references)
			//IL_0638: Unknown result type (might be due to invalid IL or missing references)
			//IL_063d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0642: Unknown result type (might be due to invalid IL or missing references)
			//IL_0646: Unknown result type (might be due to invalid IL or missing references)
			//IL_064b: Unknown result type (might be due to invalid IL or missing references)
			//IL_065c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0661: Unknown result type (might be due to invalid IL or missing references)
			//IL_0666: Unknown result type (might be due to invalid IL or missing references)
			//IL_066a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0679: Unknown result type (might be due to invalid IL or missing references)
			//IL_0680: Unknown result type (might be due to invalid IL or missing references)
			//IL_0685: Unknown result type (might be due to invalid IL or missing references)
			//IL_068a: Unknown result type (might be due to invalid IL or missing references)
			//IL_068c: Unknown result type (might be due to invalid IL or missing references)
			//IL_068e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0690: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a50: Unknown result type (might be due to invalid IL or missing references)
			//IL_07be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b80: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b89: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a3c: Unknown result type (might be due to invalid IL or missing references)
			//IL_07aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_097e: Unknown result type (might be due to invalid IL or missing references)
			//IL_098a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0994: Unknown result type (might be due to invalid IL or missing references)
			//IL_0999: Unknown result type (might be due to invalid IL or missing references)
			//IL_099e: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_09bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_09da: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_09eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a00: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a02: Unknown result type (might be due to invalid IL or missing references)
			//IL_071d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0722: Unknown result type (might be due to invalid IL or missing references)
			//IL_0729: Unknown result type (might be due to invalid IL or missing references)
			//IL_072b: Unknown result type (might be due to invalid IL or missing references)
			//IL_072d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0734: Unknown result type (might be due to invalid IL or missing references)
			//IL_0739: Unknown result type (might be due to invalid IL or missing references)
			//IL_073e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0740: Unknown result type (might be due to invalid IL or missing references)
			//IL_0747: Unknown result type (might be due to invalid IL or missing references)
			//IL_074c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0751: Unknown result type (might be due to invalid IL or missing references)
			//IL_075a: Unknown result type (might be due to invalid IL or missing references)
			//IL_075f: Unknown result type (might be due to invalid IL or missing references)
			//IL_076b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0770: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_0413: Unknown result type (might be due to invalid IL or missing references)
			//IL_0418: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_0450: Unknown result type (might be due to invalid IL or missing references)
			//IL_0464: Unknown result type (might be due to invalid IL or missing references)
			ArchetypeChunk val = m_Chunks[index];
			NativeArray<BuildingData> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<BuildingData>(ref m_BuildingDataType);
			BufferAccessor<SubMesh> bufferAccessor = ((ArchetypeChunk)(ref val)).GetBufferAccessor<SubMesh>(ref m_SubMeshType);
			BufferAccessor<SubObject> bufferAccessor2 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<SubObject>(ref m_SubObjectType);
			BufferAccessor<Effect> bufferAccessor3 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<Effect>(ref m_EffectType);
			if (((ArchetypeChunk)(ref val)).Has<SpawnableBuildingData>(ref m_SpawnableBuildingDataType))
			{
				DynamicBuffer<SubObject> subObjects = default(DynamicBuffer<SubObject>);
				for (int i = 0; i < nativeArray.Length; i++)
				{
					BuildingData buildingData = nativeArray[i];
					buildingData.m_Flags |= BuildingFlags.RequireRoad | BuildingFlags.RestrictedPedestrian | BuildingFlags.RestrictedCar | BuildingFlags.RestrictedParking | BuildingFlags.RestrictedTrack;
					if (bufferAccessor[i].Length == 0)
					{
						buildingData.m_Flags |= BuildingFlags.ColorizeLot;
					}
					if (CollectionUtils.TryGet<SubObject>(bufferAccessor2, i, ref subObjects))
					{
						CheckPropFlags(ref buildingData.m_Flags, subObjects);
					}
					nativeArray[i] = buildingData;
				}
			}
			else if (((ArchetypeChunk)(ref val)).Has<ServiceUpgradeData>(ref m_ServiceUpgradeDataType) || ((ArchetypeChunk)(ref val)).Has<ExtractorFacilityData>(ref m_ExtractorFacilityDataType))
			{
				DynamicBuffer<SubObject> subObjects2 = default(DynamicBuffer<SubObject>);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					BuildingData buildingData2 = nativeArray[j];
					buildingData2.m_Flags |= BuildingFlags.NoRoadConnection | BuildingFlags.RestrictedPedestrian | BuildingFlags.RestrictedCar | BuildingFlags.RestrictedParking | BuildingFlags.RestrictedTrack;
					if (bufferAccessor[j].Length == 0)
					{
						buildingData2.m_Flags |= BuildingFlags.ColorizeLot;
					}
					if (CollectionUtils.TryGet<SubObject>(bufferAccessor2, j, ref subObjects2))
					{
						CheckPropFlags(ref buildingData2.m_Flags, subObjects2);
					}
					nativeArray[j] = buildingData2;
				}
			}
			else
			{
				NativeArray<ConsumptionData> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<ConsumptionData>(ref m_ConsumptionDataType);
				NativeArray<WorkplaceData> nativeArray3 = ((ArchetypeChunk)(ref val)).GetNativeArray<WorkplaceData>(ref m_WorkplaceDataType);
				BufferAccessor<SubNet> bufferAccessor4 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<SubNet>(ref m_SubNetType);
				bool flag = ((ArchetypeChunk)(ref val)).Has<WaterPumpingStationData>(ref m_WaterPumpingStationDataType);
				bool flag2 = ((ArchetypeChunk)(ref val)).Has<WaterTowerData>(ref m_WaterTowerDataType);
				bool flag3 = ((ArchetypeChunk)(ref val)).Has<SewageOutletData>(ref m_SewageOutletDataType);
				bool flag4 = ((ArchetypeChunk)(ref val)).Has<WastewaterTreatmentPlantData>(ref m_WastewaterTreatmentPlantDataType);
				bool flag5 = ((ArchetypeChunk)(ref val)).Has<TransformerData>(ref m_TransformerDataType);
				bool num = ((ArchetypeChunk)(ref val)).Has<ParkingFacilityData>(ref m_ParkingFacilityDataType);
				bool flag6 = ((ArchetypeChunk)(ref val)).Has<PublicTransportStationData>(ref m_PublicTransportStationDataType);
				bool flag7 = ((ArchetypeChunk)(ref val)).Has<CargoTransportStationData>(ref m_CargoTransportStationDataType);
				bool flag8 = ((ArchetypeChunk)(ref val)).Has<ParkData>(ref m_ParkDataType);
				BuildingFlags buildingFlags = (BuildingFlags)0u;
				if (!num && !flag6)
				{
					buildingFlags |= BuildingFlags.RestrictedPedestrian;
				}
				if (!num && !flag7 && !flag6)
				{
					buildingFlags |= BuildingFlags.RestrictedCar;
				}
				if (!num)
				{
					buildingFlags |= BuildingFlags.RestrictedParking;
				}
				if (!flag6 && !flag7)
				{
					buildingFlags |= BuildingFlags.RestrictedTrack;
				}
				DynamicBuffer<SubObject> subObjects3 = default(DynamicBuffer<SubObject>);
				for (int k = 0; k < nativeArray.Length; k++)
				{
					Layer layer = Layer.None;
					Layer layer2 = Layer.None;
					Layer layer3 = Layer.None;
					if (nativeArray2.Length != 0)
					{
						ConsumptionData consumptionData = nativeArray2[k];
						if (consumptionData.m_ElectricityConsumption > 0f)
						{
							layer |= Layer.PowerlineLow;
						}
						if (consumptionData.m_GarbageAccumulation > 0f)
						{
							layer |= Layer.Road;
						}
						if (consumptionData.m_WaterConsumption > 0f)
						{
							layer |= Layer.WaterPipe | Layer.SewagePipe;
						}
					}
					if (nativeArray3.Length != 0 && nativeArray3[k].m_MaxWorkers > 0)
					{
						layer |= Layer.Road;
					}
					if (flag || flag2)
					{
						layer |= Layer.WaterPipe;
					}
					if (flag3 || flag4)
					{
						layer |= Layer.SewagePipe;
					}
					if (flag5)
					{
						layer |= Layer.PowerlineLow;
					}
					if (layer != Layer.None && bufferAccessor4.Length != 0)
					{
						DynamicBuffer<SubNet> val2 = bufferAccessor4[k];
						for (int l = 0; l < val2.Length; l++)
						{
							SubNet subNet = val2[l];
							if (m_NetData.HasComponent(subNet.m_Prefab))
							{
								NetData netData = m_NetData[subNet.m_Prefab];
								if ((netData.m_RequiredLayers & Layer.Road) == 0)
								{
									layer2 |= netData.m_RequiredLayers | netData.m_LocalConnectLayers;
									layer3 |= netData.m_RequiredLayers;
								}
							}
						}
					}
					BuildingData buildingData3 = nativeArray[k];
					buildingData3.m_Flags |= buildingFlags;
					if ((layer & ~layer2) != Layer.None)
					{
						buildingData3.m_Flags |= BuildingFlags.RequireRoad;
					}
					if ((layer3 & Layer.PowerlineLow) != Layer.None)
					{
						buildingData3.m_Flags |= BuildingFlags.HasLowVoltageNode;
					}
					if ((layer3 & Layer.WaterPipe) != Layer.None)
					{
						buildingData3.m_Flags |= BuildingFlags.HasWaterNode;
					}
					if ((layer3 & Layer.SewagePipe) != Layer.None)
					{
						buildingData3.m_Flags |= BuildingFlags.HasSewageNode;
					}
					if (bufferAccessor[k].Length == 0)
					{
						buildingData3.m_Flags |= BuildingFlags.ColorizeLot;
					}
					if (flag8 && (buildingData3.m_Flags & (BuildingFlags.LeftAccess | BuildingFlags.RightAccess | BuildingFlags.BackAccess)) != 0)
					{
						buildingData3.m_Flags &= ~BuildingFlags.RestrictedPedestrian;
					}
					if (CollectionUtils.TryGet<SubObject>(bufferAccessor2, k, ref subObjects3))
					{
						CheckPropFlags(ref buildingData3.m_Flags, subObjects3);
					}
					nativeArray[k] = buildingData3;
				}
			}
			Random random = m_RandomSeed.GetRandom(index);
			bool2 val5 = default(bool2);
			EffectData effectData = default(EffectData);
			MeshData meshData = default(MeshData);
			float2 val14 = default(float2);
			float3 val19 = default(float3);
			for (int m = 0; m < bufferAccessor3.Length; m++)
			{
				DynamicBuffer<Effect> val3 = bufferAccessor3[m];
				DynamicBuffer<SubMesh> val4 = bufferAccessor[m];
				((bool2)(ref val5))._002Ector(false, nativeArray.Length == 0);
				int num2 = 0;
				while (true)
				{
					if (num2 < val3.Length)
					{
						if (m_EffectData.TryGetComponent(val3[num2].m_Effect, ref effectData) && (effectData.m_Flags.m_RequiredFlags & EffectConditionFlags.Collapsing) != EffectConditionFlags.None)
						{
							val5.x |= m_VFXData.HasComponent(val3[num2].m_Effect);
							val5.y |= m_AudioSourceData.HasBuffer(val3[num2].m_Effect);
							if (math.all(val5))
							{
								break;
							}
						}
						num2++;
						continue;
					}
					for (int n = 0; n < val4.Length; n++)
					{
						SubMesh subMesh = val4[n];
						if (!m_MeshData.TryGetComponent(subMesh.m_SubMesh, ref meshData))
						{
							continue;
						}
						float2 val6 = MathUtils.Center(((Bounds3)(ref meshData.m_Bounds)).xz);
						float2 val7 = MathUtils.Size(((Bounds3)(ref meshData.m_Bounds)).xz);
						float3 val8 = subMesh.m_Position + math.rotate(subMesh.m_Rotation, new float3(val6.x, 0f, val6.y));
						if (!val5.y)
						{
							int2 val9 = math.max(int2.op_Implicit(0), (int2)(val7 * m_BuildingConfigurationData.m_CollapseSFXDensity));
							if (val9.x * val9.y > 1)
							{
								float2 val10 = val7 / float2.op_Implicit(val9);
								float3 val11 = math.rotate(subMesh.m_Rotation, new float3(val10.x, 0f, 0f));
								float3 val12 = math.rotate(subMesh.m_Rotation, new float3(0f, 0f, val10.y));
								float3 val13 = val8 - (val11 * ((float)val9.x * 0.5f - 0.5f) + val12 * ((float)val9.y * 0.5f - 0.5f));
								val3.Capacity = val3.Length + val9.x * val9.y;
								for (int num3 = 0; num3 < val9.y; num3++)
								{
									for (int num4 = 0; num4 < val9.x; num4++)
									{
										((float2)(ref val14))._002Ector((float)num4, (float)num3);
										val3.Add(new Effect
										{
											m_Effect = m_BuildingConfigurationData.m_CollapseSFX,
											m_Position = val13 + val11 * val14.x + val12 * val14.y,
											m_Rotation = subMesh.m_Rotation,
											m_Scale = float3.op_Implicit(0.5f),
											m_Intensity = 0.5f,
											m_ParentMesh = n,
											m_AnimationIndex = -1,
											m_Procedural = true
										});
									}
								}
							}
							else
							{
								val3.Add(new Effect
								{
									m_Effect = m_BuildingConfigurationData.m_CollapseSFX,
									m_Position = val8,
									m_Rotation = subMesh.m_Rotation,
									m_Scale = float3.op_Implicit(1f),
									m_Intensity = 1f,
									m_ParentMesh = n,
									m_AnimationIndex = -1,
									m_Procedural = true
								});
							}
						}
						if (val5.x)
						{
							continue;
						}
						int2 val15 = math.max(int2.op_Implicit(1), (int2)(math.sqrt(val7) * 0.5f));
						float2 val16 = val7 / float2.op_Implicit(val15);
						float3 val17 = math.rotate(subMesh.m_Rotation, new float3(val16.x, 0f, 0f));
						float3 val18 = math.rotate(subMesh.m_Rotation, new float3(0f, 0f, val16.y));
						((float3)(ref val19))._002Ector(val16.x * 0.05f, 1f, val16.y * 0.05f);
						val8 -= val17 * ((float)val15.x * 0.5f - 0.5f) + val18 * ((float)val15.y * 0.5f - 0.5f);
						val19.y = (val19.x + val19.y) * 0.5f;
						val3.Capacity = val3.Length + val15.x * val15.y;
						for (int num5 = 0; num5 < val15.y; num5++)
						{
							for (int num6 = 0; num6 < val15.x; num6++)
							{
								float2 val20 = new float2((float)num6, (float)num5) + ((Random)(ref random)).NextFloat2(float2.op_Implicit(-0.25f), float2.op_Implicit(0.25f));
								val3.Add(new Effect
								{
									m_Effect = m_BuildingConfigurationData.m_CollapseVFX,
									m_Position = val8 + val17 * val20.x + val18 * val20.y,
									m_Rotation = subMesh.m_Rotation,
									m_Scale = val19,
									m_Intensity = 1f,
									m_ParentMesh = n,
									m_AnimationIndex = -1,
									m_Procedural = true
								});
							}
						}
					}
					break;
				}
			}
			NativeList<Effect> val21 = default(NativeList<Effect>);
			val21._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
			NativeList<float3> val22 = default(NativeList<float3>);
			val22._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
			float num7 = 125f;
			bool2 hasFireSfxEffects = default(bool2);
			EffectData effectData2 = default(EffectData);
			for (int num8 = 0; num8 < bufferAccessor3.Length; num8++)
			{
				DynamicBuffer<Effect> effects = bufferAccessor3[num8];
				((bool2)(ref hasFireSfxEffects))._002Ector(false, false);
				for (int num9 = 0; num9 < effects.Length; num9++)
				{
					if (m_EffectData.TryGetComponent(effects[num9].m_Effect, ref effectData2) && (effectData2.m_Flags.m_RequiredFlags & EffectConditionFlags.OnFire) != EffectConditionFlags.None)
					{
						hasFireSfxEffects.x |= m_AudioEffectData.HasComponent(effects[num9].m_Effect);
						hasFireSfxEffects.y |= m_AudioSpotData.HasComponent(effects[num9].m_Effect);
						Effect effect = effects[num9];
						val21.Add(ref effect);
					}
				}
				for (int num10 = 0; num10 < val21.Length; num10++)
				{
					Effect effect2 = val21[num10];
					bool flag9 = false;
					for (int num11 = 0; num11 < val22.Length; num11++)
					{
						if (math.distancesq(effect2.m_Position, val22[num11]) < num7 * num7)
						{
							flag9 = true;
							break;
						}
					}
					if (!flag9)
					{
						val22.Add(ref effect2.m_Position);
						AddFireSfxToBuilding(ref hasFireSfxEffects, effects, effect2.m_Position, effect2.m_Rotation, effect2.m_ParentMesh);
					}
				}
				val21.Clear();
				val22.Clear();
			}
		}

		private void AddFireSfxToBuilding(ref bool2 hasFireSfxEffects, DynamicBuffer<Effect> effects, float3 position, quaternion rotation, int parent)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			if (!hasFireSfxEffects.x)
			{
				effects.Add(new Effect
				{
					m_Effect = m_BuildingConfigurationData.m_FireLoopSFX,
					m_Position = position,
					m_Rotation = rotation,
					m_Scale = float3.op_Implicit(1f),
					m_Intensity = 1f,
					m_ParentMesh = parent,
					m_AnimationIndex = -1,
					m_Procedural = true
				});
			}
			if (!hasFireSfxEffects.y)
			{
				effects.Add(new Effect
				{
					m_Effect = m_BuildingConfigurationData.m_FireSpotSFX,
					m_Position = position,
					m_Rotation = rotation,
					m_Scale = float3.op_Implicit(1f),
					m_Intensity = 1f,
					m_ParentMesh = parent,
					m_AnimationIndex = -1,
					m_Procedural = true
				});
			}
		}

		private void CheckPropFlags(ref BuildingFlags flags, DynamicBuffer<SubObject> subObjects, int maxDepth = 10)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			if (--maxDepth < 0)
			{
				return;
			}
			SpawnLocationData spawnLocationData = default(SpawnLocationData);
			DynamicBuffer<SubObject> subObjects2 = default(DynamicBuffer<SubObject>);
			for (int i = 0; i < subObjects.Length; i++)
			{
				SubObject subObject = subObjects[i];
				if (m_SpawnLocationData.TryGetComponent(subObject.m_Prefab, ref spawnLocationData) && spawnLocationData.m_ActivityMask.m_Mask == 0 && spawnLocationData.m_ConnectionType == RouteConnectionType.Pedestrian)
				{
					flags |= BuildingFlags.HasInsideRoom;
				}
				if (m_SubObjects.TryGetBuffer(subObject.m_Prefab, ref subObjects2))
				{
					CheckPropFlags(ref flags, subObjects2, maxDepth);
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> __Game_Common_Deleted_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabData> __Game_Prefabs_PrefabData_RO_ComponentTypeHandle;

		public ComponentTypeHandle<BuildingData> __Game_Prefabs_BuildingData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<BuildingExtensionData> __Game_Prefabs_BuildingExtensionData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<BuildingTerraformData> __Game_Prefabs_BuildingTerraformData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<ConsumptionData> __Game_Prefabs_ConsumptionData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<SignatureBuildingData> __Game_Prefabs_SignatureBuildingData_RO_ComponentTypeHandle;

		public ComponentTypeHandle<PlaceableObjectData> __Game_Prefabs_PlaceableObjectData_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ServiceUpgradeData> __Game_Prefabs_ServiceUpgradeData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<BuildingPropertyData> __Game_Prefabs_BuildingPropertyData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WaterPoweredData> __Game_Prefabs_WaterPoweredData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<SewageOutletData> __Game_Prefabs_SewageOutletData_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<ServiceUpgradeBuilding> __Game_Prefabs_ServiceUpgradeBuilding_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CollectedServiceBuildingBudgetData> __Game_Simulation_CollectedServiceBuildingBudgetData_RO_ComponentTypeHandle;

		public BufferTypeHandle<ServiceUpkeepData> __Game_Prefabs_ServiceUpkeepData_RW_BufferTypeHandle;

		public ComponentLookup<ZoneData> __Game_Prefabs_ZoneData_RW_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ZoneServiceConsumptionData> __Game_Prefabs_ZoneServiceConsumptionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<ExtractorFacilityData> __Game_Prefabs_ExtractorFacilityData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ConsumptionData> __Game_Prefabs_ConsumptionData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WorkplaceData> __Game_Prefabs_WorkplaceData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WaterPumpingStationData> __Game_Prefabs_WaterPumpingStationData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WaterTowerData> __Game_Prefabs_WaterTowerData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WastewaterTreatmentPlantData> __Game_Prefabs_WastewaterTreatmentPlantData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TransformerData> __Game_Prefabs_TransformerData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ParkingFacilityData> __Game_Prefabs_ParkingFacilityData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PublicTransportStationData> __Game_Prefabs_PublicTransportStationData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CargoTransportStationData> __Game_Prefabs_CargoTransportStationData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ParkData> __Game_Prefabs_ParkData_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<SubNet> __Game_Prefabs_SubNet_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<SubObject> __Game_Prefabs_SubObject_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<SubMesh> __Game_Prefabs_SubMesh_RO_BufferTypeHandle;

		public BufferTypeHandle<Effect> __Game_Prefabs_Effect_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<NetData> __Game_Prefabs_NetData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> __Game_Prefabs_SpawnLocationData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MeshData> __Game_Prefabs_MeshData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EffectData> __Game_Prefabs_EffectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<VFXData> __Game_Prefabs_VFXData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<AudioSourceData> __Game_Prefabs_AudioSourceData_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<AudioSpotData> __Game_Prefabs_AudioSpotData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AudioEffectData> __Game_Prefabs_AudioEffectData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<SubObject> __Game_Prefabs_SubObject_RO_BufferLookup;

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
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Deleted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Deleted>(true);
			__Game_Prefabs_PrefabData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabData>(true);
			__Game_Prefabs_BuildingData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BuildingData>(false);
			__Game_Prefabs_BuildingExtensionData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BuildingExtensionData>(false);
			__Game_Prefabs_BuildingTerraformData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BuildingTerraformData>(false);
			__Game_Prefabs_ConsumptionData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ConsumptionData>(false);
			__Game_Prefabs_ObjectGeometryData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ObjectGeometryData>(false);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<SpawnableBuildingData>(true);
			__Game_Prefabs_SignatureBuildingData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<SignatureBuildingData>(true);
			__Game_Prefabs_PlaceableObjectData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PlaceableObjectData>(false);
			__Game_Prefabs_ServiceUpgradeData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ServiceUpgradeData>(true);
			__Game_Prefabs_BuildingPropertyData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BuildingPropertyData>(true);
			__Game_Prefabs_WaterPoweredData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WaterPoweredData>(true);
			__Game_Prefabs_SewageOutletData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<SewageOutletData>(true);
			__Game_Prefabs_ServiceUpgradeBuilding_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ServiceUpgradeBuilding>(true);
			__Game_Simulation_CollectedServiceBuildingBudgetData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CollectedServiceBuildingBudgetData>(true);
			__Game_Prefabs_ServiceUpkeepData_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ServiceUpkeepData>(false);
			__Game_Prefabs_ZoneData_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ZoneData>(false);
			__Game_Prefabs_ZoneServiceConsumptionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ZoneServiceConsumptionData>(true);
			__Game_Prefabs_ExtractorFacilityData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ExtractorFacilityData>(true);
			__Game_Prefabs_ConsumptionData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ConsumptionData>(true);
			__Game_Prefabs_WorkplaceData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WorkplaceData>(true);
			__Game_Prefabs_WaterPumpingStationData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WaterPumpingStationData>(true);
			__Game_Prefabs_WaterTowerData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WaterTowerData>(true);
			__Game_Prefabs_WastewaterTreatmentPlantData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WastewaterTreatmentPlantData>(true);
			__Game_Prefabs_TransformerData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TransformerData>(true);
			__Game_Prefabs_ParkingFacilityData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ParkingFacilityData>(true);
			__Game_Prefabs_PublicTransportStationData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PublicTransportStationData>(true);
			__Game_Prefabs_CargoTransportStationData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CargoTransportStationData>(true);
			__Game_Prefabs_ParkData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ParkData>(true);
			__Game_Prefabs_SubNet_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubNet>(true);
			__Game_Prefabs_SubObject_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubObject>(true);
			__Game_Prefabs_SubMesh_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubMesh>(true);
			__Game_Prefabs_Effect_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Effect>(false);
			__Game_Prefabs_NetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetData>(true);
			__Game_Prefabs_SpawnLocationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnLocationData>(true);
			__Game_Prefabs_MeshData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MeshData>(true);
			__Game_Prefabs_EffectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EffectData>(true);
			__Game_Prefabs_VFXData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<VFXData>(true);
			__Game_Prefabs_AudioSourceData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<AudioSourceData>(true);
			__Game_Prefabs_AudioSpotData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AudioSpotData>(true);
			__Game_Prefabs_AudioEffectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AudioEffectData>(true);
			__Game_Prefabs_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubObject>(true);
		}
	}

	private static ILog log;

	private EntityQuery m_PrefabQuery;

	private EntityQuery m_ConfigurationQuery;

	private PrefabSystem m_PrefabSystem;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_547773813_0;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		log = LogManager.GetLogger("Simulation");
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[2];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<PrefabData>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadWrite<BuildingData>(),
			ComponentType.ReadWrite<BuildingExtensionData>(),
			ComponentType.ReadWrite<ServiceUpgradeData>(),
			ComponentType.ReadWrite<SpawnableBuildingData>()
		};
		array[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadWrite<ServiceUpgradeData>()
		};
		array[1] = val;
		m_PrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_ConfigurationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<BuildingConfigurationData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_PrefabQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_113b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1140: Unknown result type (might be due to invalid IL or missing references)
		//IL_1158: Unknown result type (might be due to invalid IL or missing references)
		//IL_115d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1175: Unknown result type (might be due to invalid IL or missing references)
		//IL_117a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1192: Unknown result type (might be due to invalid IL or missing references)
		//IL_1197: Unknown result type (might be due to invalid IL or missing references)
		//IL_11af: Unknown result type (might be due to invalid IL or missing references)
		//IL_11b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_11cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_11d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_11e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_1206: Unknown result type (might be due to invalid IL or missing references)
		//IL_120b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1223: Unknown result type (might be due to invalid IL or missing references)
		//IL_1228: Unknown result type (might be due to invalid IL or missing references)
		//IL_1240: Unknown result type (might be due to invalid IL or missing references)
		//IL_1245: Unknown result type (might be due to invalid IL or missing references)
		//IL_125d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1262: Unknown result type (might be due to invalid IL or missing references)
		//IL_127a: Unknown result type (might be due to invalid IL or missing references)
		//IL_127f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1297: Unknown result type (might be due to invalid IL or missing references)
		//IL_129c: Unknown result type (might be due to invalid IL or missing references)
		//IL_12b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_12b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_12ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_12f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_130b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1310: Unknown result type (might be due to invalid IL or missing references)
		//IL_1328: Unknown result type (might be due to invalid IL or missing references)
		//IL_132d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1345: Unknown result type (might be due to invalid IL or missing references)
		//IL_134a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1362: Unknown result type (might be due to invalid IL or missing references)
		//IL_1367: Unknown result type (might be due to invalid IL or missing references)
		//IL_137f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1384: Unknown result type (might be due to invalid IL or missing references)
		//IL_139c: Unknown result type (might be due to invalid IL or missing references)
		//IL_13a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_13b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_13be: Unknown result type (might be due to invalid IL or missing references)
		//IL_13d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_13db: Unknown result type (might be due to invalid IL or missing references)
		//IL_13f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_13f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1410: Unknown result type (might be due to invalid IL or missing references)
		//IL_1415: Unknown result type (might be due to invalid IL or missing references)
		//IL_142d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1432: Unknown result type (might be due to invalid IL or missing references)
		//IL_144a: Unknown result type (might be due to invalid IL or missing references)
		//IL_144f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1462: Unknown result type (might be due to invalid IL or missing references)
		//IL_1463: Unknown result type (might be due to invalid IL or missing references)
		//IL_1486: Unknown result type (might be due to invalid IL or missing references)
		//IL_148c: Unknown result type (might be due to invalid IL or missing references)
		//IL_148e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1493: Unknown result type (might be due to invalid IL or missing references)
		//IL_14a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aab: Unknown result type (might be due to invalid IL or missing references)
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0525: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0531: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_053a: Unknown result type (might be due to invalid IL or missing references)
		//IL_053c: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_0545: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_056a: Unknown result type (might be due to invalid IL or missing references)
		//IL_056f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0583: Unknown result type (might be due to invalid IL or missing references)
		//IL_0588: Unknown result type (might be due to invalid IL or missing references)
		//IL_058d: Unknown result type (might be due to invalid IL or missing references)
		//IL_059b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05be: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0600: Unknown result type (might be due to invalid IL or missing references)
		//IL_0605: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0640: Unknown result type (might be due to invalid IL or missing references)
		//IL_0646: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_102b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1030: Unknown result type (might be due to invalid IL or missing references)
		//IL_1036: Unknown result type (might be due to invalid IL or missing references)
		//IL_103b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b06: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0700: Unknown result type (might be due to invalid IL or missing references)
		//IL_0709: Unknown result type (might be due to invalid IL or missing references)
		//IL_070e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0657: Unknown result type (might be due to invalid IL or missing references)
		//IL_065c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0661: Unknown result type (might be due to invalid IL or missing references)
		//IL_0663: Unknown result type (might be due to invalid IL or missing references)
		//IL_066a: Unknown result type (might be due to invalid IL or missing references)
		//IL_066f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0673: Unknown result type (might be due to invalid IL or missing references)
		//IL_067a: Unknown result type (might be due to invalid IL or missing references)
		//IL_067f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0686: Unknown result type (might be due to invalid IL or missing references)
		//IL_0690: Unknown result type (might be due to invalid IL or missing references)
		//IL_0697: Unknown result type (might be due to invalid IL or missing references)
		//IL_069c: Unknown result type (might be due to invalid IL or missing references)
		//IL_06aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f53: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f58: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0caf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b21: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0934: Unknown result type (might be due to invalid IL or missing references)
		//IL_0939: Unknown result type (might be due to invalid IL or missing references)
		//IL_0945: Unknown result type (might be due to invalid IL or missing references)
		//IL_094a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0954: Unknown result type (might be due to invalid IL or missing references)
		//IL_095e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0963: Unknown result type (might be due to invalid IL or missing references)
		//IL_096d: Unknown result type (might be due to invalid IL or missing references)
		//IL_098b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0720: Unknown result type (might be due to invalid IL or missing references)
		//IL_0725: Unknown result type (might be due to invalid IL or missing references)
		//IL_104e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1053: Unknown result type (might be due to invalid IL or missing references)
		//IL_1059: Unknown result type (might be due to invalid IL or missing references)
		//IL_105f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1064: Unknown result type (might be due to invalid IL or missing references)
		//IL_1068: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_09db: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c84: Unknown result type (might be due to invalid IL or missing references)
		//IL_0742: Unknown result type (might be due to invalid IL or missing references)
		//IL_075c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0761: Unknown result type (might be due to invalid IL or missing references)
		//IL_0766: Unknown result type (might be due to invalid IL or missing references)
		//IL_0768: Unknown result type (might be due to invalid IL or missing references)
		//IL_076f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0774: Unknown result type (might be due to invalid IL or missing references)
		//IL_0776: Unknown result type (might be due to invalid IL or missing references)
		//IL_0778: Unknown result type (might be due to invalid IL or missing references)
		//IL_077a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0781: Unknown result type (might be due to invalid IL or missing references)
		//IL_0786: Unknown result type (might be due to invalid IL or missing references)
		//IL_0913: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a41: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a55: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a24: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a26: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cdb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_10a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0882: Unknown result type (might be due to invalid IL or missing references)
		//IL_0887: Unknown result type (might be due to invalid IL or missing references)
		//IL_088e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0893: Unknown result type (might be due to invalid IL or missing references)
		//IL_089a: Unknown result type (might be due to invalid IL or missing references)
		//IL_089f: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_081a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0821: Unknown result type (might be due to invalid IL or missing references)
		//IL_0826: Unknown result type (might be due to invalid IL or missing references)
		//IL_082d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0832: Unknown result type (might be due to invalid IL or missing references)
		//IL_083e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0843: Unknown result type (might be due to invalid IL or missing references)
		//IL_084d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0854: Unknown result type (might be due to invalid IL or missing references)
		//IL_0859: Unknown result type (might be due to invalid IL or missing references)
		//IL_0860: Unknown result type (might be due to invalid IL or missing references)
		//IL_0865: Unknown result type (might be due to invalid IL or missing references)
		//IL_0871: Unknown result type (might be due to invalid IL or missing references)
		//IL_0876: Unknown result type (might be due to invalid IL or missing references)
		//IL_087b: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c32: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c65: Unknown result type (might be due to invalid IL or missing references)
		EntityCommandBuffer val = default(EntityCommandBuffer);
		((EntityCommandBuffer)(ref val))._002Ector((Allocator)3, (PlaybackPolicy)0);
		NativeArray<ArchetypeChunk> chunks = ((EntityQuery)(ref m_PrefabQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		EntityTypeHandle entityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<Deleted> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<PrefabData> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<BuildingData> componentTypeHandle3 = InternalCompilerInterface.GetComponentTypeHandle<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<BuildingExtensionData> componentTypeHandle4 = InternalCompilerInterface.GetComponentTypeHandle<BuildingExtensionData>(ref __TypeHandle.__Game_Prefabs_BuildingExtensionData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<BuildingTerraformData> componentTypeHandle5 = InternalCompilerInterface.GetComponentTypeHandle<BuildingTerraformData>(ref __TypeHandle.__Game_Prefabs_BuildingTerraformData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<ConsumptionData> componentTypeHandle6 = InternalCompilerInterface.GetComponentTypeHandle<ConsumptionData>(ref __TypeHandle.__Game_Prefabs_ConsumptionData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<ObjectGeometryData> componentTypeHandle7 = InternalCompilerInterface.GetComponentTypeHandle<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<SpawnableBuildingData> componentTypeHandle8 = InternalCompilerInterface.GetComponentTypeHandle<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<SignatureBuildingData> componentTypeHandle9 = InternalCompilerInterface.GetComponentTypeHandle<SignatureBuildingData>(ref __TypeHandle.__Game_Prefabs_SignatureBuildingData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<PlaceableObjectData> componentTypeHandle10 = InternalCompilerInterface.GetComponentTypeHandle<PlaceableObjectData>(ref __TypeHandle.__Game_Prefabs_PlaceableObjectData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<ServiceUpgradeData> componentTypeHandle11 = InternalCompilerInterface.GetComponentTypeHandle<ServiceUpgradeData>(ref __TypeHandle.__Game_Prefabs_ServiceUpgradeData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<BuildingPropertyData> componentTypeHandle12 = InternalCompilerInterface.GetComponentTypeHandle<BuildingPropertyData>(ref __TypeHandle.__Game_Prefabs_BuildingPropertyData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<WaterPoweredData> componentTypeHandle13 = InternalCompilerInterface.GetComponentTypeHandle<WaterPoweredData>(ref __TypeHandle.__Game_Prefabs_WaterPoweredData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<SewageOutletData> componentTypeHandle14 = InternalCompilerInterface.GetComponentTypeHandle<SewageOutletData>(ref __TypeHandle.__Game_Prefabs_SewageOutletData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		BufferTypeHandle<ServiceUpgradeBuilding> bufferTypeHandle = InternalCompilerInterface.GetBufferTypeHandle<ServiceUpgradeBuilding>(ref __TypeHandle.__Game_Prefabs_ServiceUpgradeBuilding_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<CollectedServiceBuildingBudgetData> componentTypeHandle15 = InternalCompilerInterface.GetComponentTypeHandle<CollectedServiceBuildingBudgetData>(ref __TypeHandle.__Game_Simulation_CollectedServiceBuildingBudgetData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		BufferTypeHandle<ServiceUpkeepData> bufferTypeHandle2 = InternalCompilerInterface.GetBufferTypeHandle<ServiceUpkeepData>(ref __TypeHandle.__Game_Prefabs_ServiceUpkeepData_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<ZoneData> componentLookup = InternalCompilerInterface.GetComponentLookup<ZoneData>(ref __TypeHandle.__Game_Prefabs_ZoneData_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<ZoneServiceConsumptionData> componentLookup2 = InternalCompilerInterface.GetComponentLookup<ZoneServiceConsumptionData>(ref __TypeHandle.__Game_Prefabs_ZoneServiceConsumptionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		((SystemBase)this).CompleteDependency();
		PlaceableObjectData placeableObjectData = default(PlaceableObjectData);
		Bounds2 xz2 = default(Bounds2);
		Bounds2 val6 = default(Bounds2);
		for (int i = 0; i < chunks.Length; i++)
		{
			ArchetypeChunk val2 = chunks[i];
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray(entityTypeHandle);
			BufferAccessor<ServiceUpgradeBuilding> bufferAccessor = ((ArchetypeChunk)(ref val2)).GetBufferAccessor<ServiceUpgradeBuilding>(ref bufferTypeHandle);
			EntityManager entityManager;
			if (((ArchetypeChunk)(ref val2)).Has<Deleted>(ref componentTypeHandle))
			{
				if (bufferAccessor.Length == 0)
				{
					continue;
				}
				for (int j = 0; j < bufferAccessor.Length; j++)
				{
					Entity upgrade = nativeArray[j];
					DynamicBuffer<ServiceUpgradeBuilding> val3 = bufferAccessor[j];
					for (int k = 0; k < val3.Length; k++)
					{
						ServiceUpgradeBuilding serviceUpgradeBuilding = val3[k];
						entityManager = ((ComponentSystemBase)this).EntityManager;
						CollectionUtils.RemoveValue<BuildingUpgradeElement>(((EntityManager)(ref entityManager)).GetBuffer<BuildingUpgradeElement>(serviceUpgradeBuilding.m_Building, false), new BuildingUpgradeElement(upgrade));
					}
				}
				continue;
			}
			NativeArray<PrefabData> nativeArray2 = ((ArchetypeChunk)(ref val2)).GetNativeArray<PrefabData>(ref componentTypeHandle2);
			NativeArray<ObjectGeometryData> nativeArray3 = ((ArchetypeChunk)(ref val2)).GetNativeArray<ObjectGeometryData>(ref componentTypeHandle7);
			NativeArray<BuildingData> nativeArray4 = ((ArchetypeChunk)(ref val2)).GetNativeArray<BuildingData>(ref componentTypeHandle3);
			NativeArray<BuildingExtensionData> nativeArray5 = ((ArchetypeChunk)(ref val2)).GetNativeArray<BuildingExtensionData>(ref componentTypeHandle4);
			NativeArray<ConsumptionData> nativeArray6 = ((ArchetypeChunk)(ref val2)).GetNativeArray<ConsumptionData>(ref componentTypeHandle6);
			NativeArray<SpawnableBuildingData> nativeArray7 = ((ArchetypeChunk)(ref val2)).GetNativeArray<SpawnableBuildingData>(ref componentTypeHandle8);
			NativeArray<PlaceableObjectData> nativeArray8 = ((ArchetypeChunk)(ref val2)).GetNativeArray<PlaceableObjectData>(ref componentTypeHandle10);
			NativeArray<ServiceUpgradeData> nativeArray9 = ((ArchetypeChunk)(ref val2)).GetNativeArray<ServiceUpgradeData>(ref componentTypeHandle11);
			NativeArray<BuildingPropertyData> nativeArray10 = ((ArchetypeChunk)(ref val2)).GetNativeArray<BuildingPropertyData>(ref componentTypeHandle12);
			BufferAccessor<ServiceUpkeepData> bufferAccessor2 = ((ArchetypeChunk)(ref val2)).GetBufferAccessor<ServiceUpkeepData>(ref bufferTypeHandle2);
			bool flag = ((ArchetypeChunk)(ref val2)).Has<CollectedServiceBuildingBudgetData>(ref componentTypeHandle15);
			bool flag2 = ((ArchetypeChunk)(ref val2)).Has<SignatureBuildingData>(ref componentTypeHandle9);
			bool flag3 = ((ArchetypeChunk)(ref val2)).Has<WaterPoweredData>(ref componentTypeHandle13);
			bool flag4 = ((ArchetypeChunk)(ref val2)).Has<SewageOutletData>(ref componentTypeHandle14);
			if (nativeArray4.Length != 0)
			{
				NativeArray<BuildingTerraformData> nativeArray11 = ((ArchetypeChunk)(ref val2)).GetNativeArray<BuildingTerraformData>(ref componentTypeHandle5);
				for (int l = 0; l < nativeArray4.Length; l++)
				{
					BuildingPrefab prefab = m_PrefabSystem.GetPrefab<BuildingPrefab>(nativeArray2[l]);
					BuildingTerraformOverride component = prefab.GetComponent<BuildingTerraformOverride>();
					ObjectGeometryData objectGeometryData = nativeArray3[l];
					BuildingTerraformData buildingTerraformData = nativeArray11[l];
					BuildingData buildingData = nativeArray4[l];
					InitializeLotSize(prefab, component, ref objectGeometryData, ref buildingTerraformData, ref buildingData);
					if (nativeArray7.Length != 0 && !flag2)
					{
						objectGeometryData.m_Flags |= Game.Objects.GeometryFlags.DeleteOverridden;
					}
					else
					{
						objectGeometryData.m_Flags &= ~Game.Objects.GeometryFlags.Overridable;
						objectGeometryData.m_Flags |= Game.Objects.GeometryFlags.OverrideZone;
					}
					if (flag3)
					{
						objectGeometryData.m_Flags |= Game.Objects.GeometryFlags.CanSubmerge;
					}
					else if (flag4 && prefab.GetComponent<SewageOutlet>().m_AllowSubmerged)
					{
						objectGeometryData.m_Flags |= Game.Objects.GeometryFlags.CanSubmerge;
					}
					objectGeometryData.m_Flags &= ~Game.Objects.GeometryFlags.Brushable;
					objectGeometryData.m_Flags |= Game.Objects.GeometryFlags.ExclusiveGround | Game.Objects.GeometryFlags.WalkThrough | Game.Objects.GeometryFlags.OccupyZone | Game.Objects.GeometryFlags.HasLot;
					if (CollectionUtils.TryGet<PlaceableObjectData>(nativeArray8, l, ref placeableObjectData) && (placeableObjectData.m_Flags & (Game.Objects.PlacementFlags.OnGround | Game.Objects.PlacementFlags.Floating | Game.Objects.PlacementFlags.Swaying)) == (Game.Objects.PlacementFlags.Floating | Game.Objects.PlacementFlags.Swaying))
					{
						objectGeometryData.m_Flags &= ~(Game.Objects.GeometryFlags.ExclusiveGround | Game.Objects.GeometryFlags.OccupyZone);
					}
					nativeArray3[l] = objectGeometryData;
					nativeArray11[l] = buildingTerraformData;
					nativeArray4[l] = buildingData;
				}
			}
			if (nativeArray5.Length != 0)
			{
				NativeArray<BuildingTerraformData> nativeArray12 = ((ArchetypeChunk)(ref val2)).GetNativeArray<BuildingTerraformData>(ref componentTypeHandle5);
				for (int m = 0; m < nativeArray5.Length; m++)
				{
					BuildingExtensionPrefab prefab2 = m_PrefabSystem.GetPrefab<BuildingExtensionPrefab>(nativeArray2[m]);
					ObjectGeometryData objectGeometryData2 = nativeArray3[m];
					if ((objectGeometryData2.m_Flags & Game.Objects.GeometryFlags.Standing) != Game.Objects.GeometryFlags.None)
					{
						float2 xz = ((float3)(ref objectGeometryData2.m_Pivot)).xz;
						float2 val4 = ((float3)(ref objectGeometryData2.m_LegSize)).xz * 0.5f + objectGeometryData2.m_LegOffset;
						((Bounds2)(ref xz2))._002Ector(xz - val4, xz + val4);
					}
					else
					{
						xz2 = ((Bounds3)(ref objectGeometryData2.m_Bounds)).xz;
					}
					objectGeometryData2.m_Bounds.min = math.min(objectGeometryData2.m_Bounds.min, new float3(-0.5f, 0f, -0.5f));
					objectGeometryData2.m_Bounds.max = math.max(objectGeometryData2.m_Bounds.max, new float3(0.5f, 5f, 0.5f));
					objectGeometryData2.m_Flags &= ~(Game.Objects.GeometryFlags.Overridable | Game.Objects.GeometryFlags.Brushable);
					objectGeometryData2.m_Flags |= Game.Objects.GeometryFlags.ExclusiveGround | Game.Objects.GeometryFlags.WalkThrough | Game.Objects.GeometryFlags.OccupyZone | Game.Objects.GeometryFlags.HasLot;
					BuildingExtensionData buildingExtensionData = nativeArray5[m];
					buildingExtensionData.m_Position = prefab2.m_Position;
					buildingExtensionData.m_LotSize = prefab2.m_OverrideLotSize;
					buildingExtensionData.m_External = prefab2.m_ExternalLot;
					if (prefab2.m_OverrideHeight > 0f)
					{
						objectGeometryData2.m_Bounds.max.y = prefab2.m_OverrideHeight;
					}
					if (math.all(buildingExtensionData.m_LotSize > 0))
					{
						float2 val5 = float2.op_Implicit(buildingExtensionData.m_LotSize);
						val5 *= 8f;
						((Bounds2)(ref val6))._002Ector(val5 * -0.5f, val5 * 0.5f);
						val5 -= 0.4f;
						((float3)(ref objectGeometryData2.m_Bounds.min)).xz = val5 * -0.5f;
						((float3)(ref objectGeometryData2.m_Bounds.max)).xz = val5 * 0.5f;
						if (bufferAccessor.Length != 0)
						{
							objectGeometryData2.m_Flags |= Game.Objects.GeometryFlags.OverrideZone;
						}
					}
					else
					{
						Bounds3 bounds = objectGeometryData2.m_Bounds;
						val6 = ((Bounds3)(ref objectGeometryData2.m_Bounds)).xz;
						if (bufferAccessor.Length != 0)
						{
							DynamicBuffer<ServiceUpgradeBuilding> val7 = bufferAccessor[m];
							for (int n = 0; n < val7.Length; n++)
							{
								ServiceUpgradeBuilding serviceUpgradeBuilding2 = val7[n];
								BuildingPrefab prefab3 = m_PrefabSystem.GetPrefab<BuildingPrefab>(serviceUpgradeBuilding2.m_Building);
								float2 val8 = float2.op_Implicit(new int2(prefab3.m_LotWidth, prefab3.m_LotDepth));
								val8 *= 8f;
								float2 val9 = val8;
								val8 -= 0.4f;
								if ((objectGeometryData2.m_Flags & Game.Objects.GeometryFlags.Standing) == 0 && prefab3.TryGet<StandingObject>(out var component2))
								{
									val8 = ((float3)(ref component2.m_LegSize)).xz + math.select(default(float2), ((float3)(ref component2.m_LegSize)).xz + component2.m_LegGap, component2.m_LegGap != 0f);
									val8 -= 0.4f;
									val9 = val8;
									if (component2.m_CircularLeg)
									{
										objectGeometryData2.m_Flags |= Game.Objects.GeometryFlags.Circular;
									}
								}
								if (n == 0)
								{
									((Bounds3)(ref bounds)).xz = new Bounds2(val8 * -0.5f, val8 * 0.5f) - ((float3)(ref prefab2.m_Position)).xz;
									val6 = new Bounds2(val9 * -0.5f, val9 * 0.5f) - ((float3)(ref prefab2.m_Position)).xz;
								}
								else
								{
									((Bounds3)(ref bounds)).xz = ((Bounds3)(ref bounds)).xz & (new Bounds2(val8 * -0.5f, val8 * 0.5f) - ((float3)(ref prefab2.m_Position)).xz);
									val6 &= new Bounds2(val9 * -0.5f, val9 * 0.5f) - ((float3)(ref prefab2.m_Position)).xz;
								}
							}
							((Bounds3)(ref objectGeometryData2.m_Bounds)).xz = ((Bounds3)(ref bounds)).xz;
							objectGeometryData2.m_Flags |= Game.Objects.GeometryFlags.OverrideZone;
						}
						float2 val10 = math.min(-((float3)(ref bounds.min)).xz, ((float3)(ref bounds.max)).xz) * 0.25f - 0.01f;
						buildingExtensionData.m_LotSize.x = math.max(1, Mathf.CeilToInt(val10.x));
						buildingExtensionData.m_LotSize.y = math.max(1, Mathf.CeilToInt(val10.y));
					}
					if (buildingExtensionData.m_External)
					{
						float2 val11 = float2.op_Implicit(buildingExtensionData.m_LotSize);
						val11 *= 8f;
						objectGeometryData2.m_Layers |= MeshLayer.Default;
						objectGeometryData2.m_MinLod = math.min(objectGeometryData2.m_MinLod, RenderingUtils.CalculateLodLimit(RenderingUtils.GetRenderingSize(new float3(val11.x, 0f, val11.y))));
					}
					if (nativeArray12.Length != 0)
					{
						BuildingTerraformOverride component3 = prefab2.GetComponent<BuildingTerraformOverride>();
						BuildingTerraformData buildingTerraformData2 = nativeArray12[m];
						InitializeTerraformData(component3, ref buildingTerraformData2, val6, xz2);
						nativeArray12[m] = buildingTerraformData2;
					}
					objectGeometryData2.m_Size = math.max(ObjectUtils.GetSize(objectGeometryData2.m_Bounds), new float3(1f, 5f, 1f));
					nativeArray3[m] = objectGeometryData2;
					nativeArray5[m] = buildingExtensionData;
				}
			}
			if (nativeArray7.Length != 0)
			{
				for (int num = 0; num < nativeArray7.Length; num++)
				{
					Entity val12 = nativeArray[num];
					BuildingPrefab prefab4 = m_PrefabSystem.GetPrefab<BuildingPrefab>(nativeArray2[num]);
					BuildingPropertyData buildingPropertyData = ((nativeArray10.Length != 0) ? nativeArray10[num] : default(BuildingPropertyData));
					SpawnableBuildingData spawnableBuildingData = nativeArray7[num];
					if (!(spawnableBuildingData.m_ZonePrefab != Entity.Null))
					{
						continue;
					}
					Entity zonePrefab = spawnableBuildingData.m_ZonePrefab;
					ZoneData zoneData = componentLookup[zonePrefab];
					if (!flag2)
					{
						((EntityCommandBuffer)(ref val)).SetSharedComponent<BuildingSpawnGroupData>(val12, new BuildingSpawnGroupData(zoneData.m_ZoneType));
						ushort num2 = (ushort)math.clamp(Mathf.CeilToInt(nativeArray3[num].m_Size.y), 0, 65535);
						if (spawnableBuildingData.m_Level == 1)
						{
							if (prefab4.m_LotWidth == 1 && (zoneData.m_ZoneFlags & ZoneFlags.SupportNarrow) == 0)
							{
								zoneData.m_ZoneFlags |= ZoneFlags.SupportNarrow;
								componentLookup[zonePrefab] = zoneData;
							}
							if (prefab4.m_AccessType == BuildingAccessType.LeftCorner && (zoneData.m_ZoneFlags & ZoneFlags.SupportLeftCorner) == 0)
							{
								zoneData.m_ZoneFlags |= ZoneFlags.SupportLeftCorner;
								componentLookup[zonePrefab] = zoneData;
							}
							if (prefab4.m_AccessType == BuildingAccessType.RightCorner && (zoneData.m_ZoneFlags & ZoneFlags.SupportRightCorner) == 0)
							{
								zoneData.m_ZoneFlags |= ZoneFlags.SupportRightCorner;
								componentLookup[zonePrefab] = zoneData;
							}
							if (prefab4.m_AccessType == BuildingAccessType.Front && prefab4.m_LotWidth <= 3 && prefab4.m_LotDepth <= 2)
							{
								if ((prefab4.m_LotWidth == 1 || prefab4.m_LotWidth == 3) && num2 < zoneData.m_MinOddHeight)
								{
									zoneData.m_MinOddHeight = num2;
									componentLookup[zonePrefab] = zoneData;
								}
								if ((prefab4.m_LotWidth == 1 || prefab4.m_LotWidth == 2) && num2 < zoneData.m_MinEvenHeight)
								{
									zoneData.m_MinEvenHeight = num2;
									componentLookup[zonePrefab] = zoneData;
								}
							}
						}
						if (num2 > zoneData.m_MaxHeight)
						{
							zoneData.m_MaxHeight = num2;
							componentLookup[zonePrefab] = zoneData;
						}
					}
					int level = spawnableBuildingData.m_Level;
					BuildingData buildingData2 = nativeArray4[num];
					int lotSize = buildingData2.m_LotSize.x * buildingData2.m_LotSize.y;
					if (nativeArray6.Length != 0 && !prefab4.Has<ServiceConsumption>() && componentLookup2.HasComponent(zonePrefab))
					{
						ZoneServiceConsumptionData zoneServiceConsumptionData = componentLookup2[zonePrefab];
						ref ConsumptionData reference = ref CollectionUtils.ElementAt<ConsumptionData>(nativeArray6, num);
						if (flag2)
						{
							level = 2;
						}
						bool isStorage = buildingPropertyData.m_AllowedStored != Resource.NoResource;
						EconomyParameterData economyParameterData = ((EntityQuery)(ref __query_547773813_0)).GetSingleton<EconomyParameterData>();
						reference.m_Upkeep = PropertyRenterSystem.GetUpkeep(level, zoneServiceConsumptionData.m_Upkeep, lotSize, zoneData.m_AreaType, ref economyParameterData, isStorage);
					}
				}
			}
			if (nativeArray8.Length != 0)
			{
				if (nativeArray9.Length != 0)
				{
					for (int num3 = 0; num3 < nativeArray8.Length; num3++)
					{
						PlaceableObjectData placeableObjectData2 = nativeArray8[num3];
						ObjectGeometryData objectGeometryData3 = nativeArray3[num3];
						ServiceUpgradeData serviceUpgradeData = nativeArray9[num3];
						if (nativeArray4.Length != 0)
						{
							placeableObjectData2.m_Flags |= Game.Objects.PlacementFlags.OwnerSide;
							if (serviceUpgradeData.m_MaxPlacementDistance != 0f)
							{
								placeableObjectData2.m_Flags |= Game.Objects.PlacementFlags.RoadSide;
							}
						}
						if ((placeableObjectData2.m_Flags & Game.Objects.PlacementFlags.NetObject) != Game.Objects.PlacementFlags.None)
						{
							objectGeometryData3.m_Flags |= Game.Objects.GeometryFlags.IgnoreLegCollision;
							if (nativeArray4.Length != 0)
							{
								BuildingData buildingData3 = nativeArray4[num3];
								buildingData3.m_Flags |= BuildingFlags.CanBeOnRoad;
								nativeArray4[num3] = buildingData3;
							}
							if ((placeableObjectData2.m_Flags & Game.Objects.PlacementFlags.Shoreline) != Game.Objects.PlacementFlags.None)
							{
								placeableObjectData2.m_Flags &= ~(Game.Objects.PlacementFlags.RoadSide | Game.Objects.PlacementFlags.OwnerSide);
							}
						}
						placeableObjectData2.m_ConstructionCost = serviceUpgradeData.m_UpgradeCost;
						nativeArray8[num3] = placeableObjectData2;
						nativeArray3[num3] = objectGeometryData3;
					}
				}
				else
				{
					for (int num4 = 0; num4 < nativeArray8.Length; num4++)
					{
						PlaceableObjectData placeableObjectData3 = nativeArray8[num4];
						ObjectGeometryData objectGeometryData4 = nativeArray3[num4];
						if (nativeArray4.Length != 0)
						{
							placeableObjectData3.m_Flags |= Game.Objects.PlacementFlags.RoadSide;
						}
						if ((placeableObjectData3.m_Flags & Game.Objects.PlacementFlags.NetObject) != Game.Objects.PlacementFlags.None)
						{
							objectGeometryData4.m_Flags |= Game.Objects.GeometryFlags.IgnoreLegCollision;
							if (nativeArray4.Length != 0)
							{
								BuildingData buildingData4 = nativeArray4[num4];
								buildingData4.m_Flags |= BuildingFlags.CanBeOnRoad;
								nativeArray4[num4] = buildingData4;
							}
							if ((placeableObjectData3.m_Flags & Game.Objects.PlacementFlags.Shoreline) != Game.Objects.PlacementFlags.None)
							{
								placeableObjectData3.m_Flags &= ~Game.Objects.PlacementFlags.RoadSide;
							}
						}
						nativeArray8[num4] = placeableObjectData3;
						nativeArray3[num4] = objectGeometryData4;
					}
				}
			}
			bool flag5 = false;
			if (flag)
			{
				for (int num5 = 0; num5 < nativeArray.Length; num5++)
				{
					if (nativeArray6.Length == 0 || nativeArray6[num5].m_Upkeep <= 0)
					{
						continue;
					}
					bool flag6 = false;
					DynamicBuffer<ServiceUpkeepData> val13 = bufferAccessor2[num5];
					for (int num6 = 0; num6 < val13.Length; num6++)
					{
						if (val13[num6].m_Upkeep.m_Resource == Resource.Money)
						{
							log.WarnFormat("Warning: {0} has monetary upkeep in both ConsumptionData and CityServiceUpkeep", (object)((Object)m_PrefabSystem.GetPrefab<PrefabBase>(nativeArray[num5])).name);
						}
					}
					if (!flag6)
					{
						val13.Add(new ServiceUpkeepData
						{
							m_ScaleWithUsage = false,
							m_Upkeep = new ResourceStack
							{
								m_Amount = nativeArray6[num5].m_Upkeep,
								m_Resource = Resource.Money
							}
						});
						flag5 = true;
					}
				}
			}
			if (bufferAccessor.Length == 0)
			{
				continue;
			}
			for (int num7 = 0; num7 < bufferAccessor.Length; num7++)
			{
				Entity upgrade2 = nativeArray[num7];
				DynamicBuffer<ServiceUpgradeBuilding> val14 = bufferAccessor[num7];
				for (int num8 = 0; num8 < val14.Length; num8++)
				{
					ServiceUpgradeBuilding serviceUpgradeBuilding3 = val14[num8];
					entityManager = ((ComponentSystemBase)this).EntityManager;
					((EntityManager)(ref entityManager)).GetBuffer<BuildingUpgradeElement>(serviceUpgradeBuilding3.m_Building, false).Add(new BuildingUpgradeElement(upgrade2));
				}
				if (!flag5 && nativeArray6.Length != 0 && nativeArray6[num7].m_Upkeep > 0)
				{
					bufferAccessor2[num7].Add(new ServiceUpkeepData
					{
						m_ScaleWithUsage = false,
						m_Upkeep = new ResourceStack
						{
							m_Amount = nativeArray6[num7].m_Upkeep,
							m_Resource = Resource.Money
						}
					});
				}
			}
		}
		JobHandle val15 = IJobParallelForExtensions.Schedule<FindConnectionRequirementsJob>(new FindConnectionRequirementsJob
		{
			m_SpawnableBuildingDataType = InternalCompilerInterface.GetComponentTypeHandle<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceUpgradeDataType = InternalCompilerInterface.GetComponentTypeHandle<ServiceUpgradeData>(ref __TypeHandle.__Game_Prefabs_ServiceUpgradeData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ExtractorFacilityDataType = InternalCompilerInterface.GetComponentTypeHandle<ExtractorFacilityData>(ref __TypeHandle.__Game_Prefabs_ExtractorFacilityData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ConsumptionDataType = InternalCompilerInterface.GetComponentTypeHandle<ConsumptionData>(ref __TypeHandle.__Game_Prefabs_ConsumptionData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WorkplaceDataType = InternalCompilerInterface.GetComponentTypeHandle<WorkplaceData>(ref __TypeHandle.__Game_Prefabs_WorkplaceData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WaterPumpingStationDataType = InternalCompilerInterface.GetComponentTypeHandle<WaterPumpingStationData>(ref __TypeHandle.__Game_Prefabs_WaterPumpingStationData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WaterTowerDataType = InternalCompilerInterface.GetComponentTypeHandle<WaterTowerData>(ref __TypeHandle.__Game_Prefabs_WaterTowerData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SewageOutletDataType = InternalCompilerInterface.GetComponentTypeHandle<SewageOutletData>(ref __TypeHandle.__Game_Prefabs_SewageOutletData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WastewaterTreatmentPlantDataType = InternalCompilerInterface.GetComponentTypeHandle<WastewaterTreatmentPlantData>(ref __TypeHandle.__Game_Prefabs_WastewaterTreatmentPlantData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformerDataType = InternalCompilerInterface.GetComponentTypeHandle<TransformerData>(ref __TypeHandle.__Game_Prefabs_TransformerData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ParkingFacilityDataType = InternalCompilerInterface.GetComponentTypeHandle<ParkingFacilityData>(ref __TypeHandle.__Game_Prefabs_ParkingFacilityData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PublicTransportStationDataType = InternalCompilerInterface.GetComponentTypeHandle<PublicTransportStationData>(ref __TypeHandle.__Game_Prefabs_PublicTransportStationData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CargoTransportStationDataType = InternalCompilerInterface.GetComponentTypeHandle<CargoTransportStationData>(ref __TypeHandle.__Game_Prefabs_CargoTransportStationData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ParkDataType = InternalCompilerInterface.GetComponentTypeHandle<ParkData>(ref __TypeHandle.__Game_Prefabs_ParkData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubNetType = InternalCompilerInterface.GetBufferTypeHandle<SubNet>(ref __TypeHandle.__Game_Prefabs_SubNet_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjectType = InternalCompilerInterface.GetBufferTypeHandle<SubObject>(ref __TypeHandle.__Game_Prefabs_SubObject_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubMeshType = InternalCompilerInterface.GetBufferTypeHandle<SubMesh>(ref __TypeHandle.__Game_Prefabs_SubMesh_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingDataType = InternalCompilerInterface.GetComponentTypeHandle<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EffectType = InternalCompilerInterface.GetBufferTypeHandle<Effect>(ref __TypeHandle.__Game_Prefabs_Effect_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NetData = InternalCompilerInterface.GetComponentLookup<NetData>(ref __TypeHandle.__Game_Prefabs_NetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnLocationData = InternalCompilerInterface.GetComponentLookup<SpawnLocationData>(ref __TypeHandle.__Game_Prefabs_SpawnLocationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MeshData = InternalCompilerInterface.GetComponentLookup<MeshData>(ref __TypeHandle.__Game_Prefabs_MeshData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EffectData = InternalCompilerInterface.GetComponentLookup<EffectData>(ref __TypeHandle.__Game_Prefabs_EffectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_VFXData = InternalCompilerInterface.GetComponentLookup<VFXData>(ref __TypeHandle.__Game_Prefabs_VFXData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AudioSourceData = InternalCompilerInterface.GetBufferLookup<AudioSourceData>(ref __TypeHandle.__Game_Prefabs_AudioSourceData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AudioSpotData = InternalCompilerInterface.GetComponentLookup<AudioSpotData>(ref __TypeHandle.__Game_Prefabs_AudioSpotData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AudioEffectData = InternalCompilerInterface.GetComponentLookup<AudioEffectData>(ref __TypeHandle.__Game_Prefabs_AudioEffectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<SubObject>(ref __TypeHandle.__Game_Prefabs_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RandomSeed = RandomSeed.Next(),
			m_Chunks = chunks,
			m_BuildingConfigurationData = ((EntityQuery)(ref m_ConfigurationQuery)).GetSingleton<BuildingConfigurationData>()
		}, chunks.Length, 1, default(JobHandle));
		((JobHandle)(ref val15)).Complete();
		chunks.Dispose();
		((EntityCommandBuffer)(ref val)).Playback(((ComponentSystemBase)this).EntityManager);
		((EntityCommandBuffer)(ref val)).Dispose();
	}

	private void InitializeLotSize(BuildingPrefab buildingPrefab, BuildingTerraformOverride terraformOverride, ref ObjectGeometryData objectGeometryData, ref BuildingTerraformData buildingTerraformData, ref BuildingData buildingData)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		buildingData.m_LotSize = new int2(buildingPrefab.m_LotWidth, buildingPrefab.m_LotDepth);
		float2 val = default(float2);
		((float2)(ref val))._002Ector((float)buildingPrefab.m_LotWidth, (float)buildingPrefab.m_LotDepth);
		val *= 8f;
		bool flag = false;
		Bounds2 xz2 = default(Bounds2);
		if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Standing) != Game.Objects.GeometryFlags.None)
		{
			int2 val2 = default(int2);
			val2.x = Mathf.RoundToInt((objectGeometryData.m_LegSize.x + objectGeometryData.m_LegOffset.x * 2f) / 8f);
			val2.y = Mathf.RoundToInt((objectGeometryData.m_LegSize.z + objectGeometryData.m_LegOffset.y * 2f) / 8f);
			flag = math.all(val2 == buildingData.m_LotSize);
			buildingData.m_LotSize = val2;
			float2 xz = ((float3)(ref objectGeometryData.m_Pivot)).xz;
			float2 val3 = ((float3)(ref objectGeometryData.m_LegSize)).xz * 0.5f + objectGeometryData.m_LegOffset;
			((Bounds2)(ref xz2))._002Ector(xz - val3, xz + val3);
			((float3)(ref objectGeometryData.m_LegSize)).xz = float2.op_Implicit(val2) * 8f - objectGeometryData.m_LegOffset * 2f - 0.4f;
		}
		else
		{
			xz2 = ((Bounds3)(ref objectGeometryData.m_Bounds)).xz;
		}
		Bounds2 val4 = default(Bounds2);
		val4.max = float2.op_Implicit(buildingData.m_LotSize) * 4f;
		val4.min = -val4.max;
		InitializeTerraformData(terraformOverride, ref buildingTerraformData, val4, xz2);
		objectGeometryData.m_Layers |= MeshLayer.Default;
		objectGeometryData.m_MinLod = math.min(objectGeometryData.m_MinLod, RenderingUtils.CalculateLodLimit(RenderingUtils.GetRenderingSize(new float3(val.x, 0f, val.y))));
		switch (buildingPrefab.m_AccessType)
		{
		case BuildingAccessType.LeftCorner:
			buildingData.m_Flags |= BuildingFlags.LeftAccess;
			break;
		case BuildingAccessType.RightCorner:
			buildingData.m_Flags |= BuildingFlags.RightAccess;
			break;
		case BuildingAccessType.LeftAndRightCorner:
			buildingData.m_Flags |= BuildingFlags.LeftAccess | BuildingFlags.RightAccess;
			break;
		case BuildingAccessType.LeftAndBackCorner:
			buildingData.m_Flags |= BuildingFlags.LeftAccess | BuildingFlags.BackAccess;
			break;
		case BuildingAccessType.RightAndBackCorner:
			buildingData.m_Flags |= BuildingFlags.RightAccess | BuildingFlags.BackAccess;
			break;
		case BuildingAccessType.FrontAndBack:
			buildingData.m_Flags |= BuildingFlags.BackAccess;
			break;
		case BuildingAccessType.All:
			buildingData.m_Flags |= BuildingFlags.LeftAccess | BuildingFlags.RightAccess | BuildingFlags.BackAccess;
			break;
		}
		if (!flag)
		{
			if (math.any(((float3)(ref objectGeometryData.m_Size)).xz > val + 0.5f) && AssetDatabase.global.AreAssetsWarningsEnabled((AssetData)(object)buildingPrefab.asset))
			{
				log.WarnFormat("Building geometry doesn't fit inside the lot ({0}): {1}m x {2}m ({3}x{4})", (object)((Object)buildingPrefab).name, (object)objectGeometryData.m_Size.x, (object)objectGeometryData.m_Size.z, (object)buildingData.m_LotSize.x, (object)buildingData.m_LotSize.y);
			}
			val -= 0.4f;
			((float3)(ref objectGeometryData.m_Size)).xz = val;
			((float3)(ref objectGeometryData.m_Bounds.min)).xz = val * -0.5f;
			((float3)(ref objectGeometryData.m_Bounds.max)).xz = val * 0.5f;
		}
		objectGeometryData.m_Size.y = math.max(objectGeometryData.m_Size.y, 5f);
		objectGeometryData.m_Bounds.min.y = math.min(objectGeometryData.m_Bounds.min.y, 0f);
		objectGeometryData.m_Bounds.max.y = math.max(objectGeometryData.m_Bounds.max.y, 5f);
	}

	public static void InitializeTerraformData(BuildingTerraformOverride terraformOverride, ref BuildingTerraformData buildingTerraformData, Bounds2 lotBounds, Bounds2 flatBounds)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		float3 val = default(float3);
		((float3)(ref val))._002Ector(1f, 0f, 1f);
		float3 val2 = default(float3);
		((float3)(ref val2))._002Ector(1f, 0f, 1f);
		float3 val3 = default(float3);
		((float3)(ref val3))._002Ector(1f, 0f, 1f);
		float3 val4 = default(float3);
		((float3)(ref val4))._002Ector(1f, 0f, 1f);
		((float4)(ref buildingTerraformData.m_Smooth)).xy = lotBounds.min;
		((float4)(ref buildingTerraformData.m_Smooth)).zw = lotBounds.max;
		if ((Object)(object)terraformOverride != (Object)null)
		{
			ref float2 min = ref flatBounds.min;
			min += terraformOverride.m_LevelMinOffset;
			ref float2 max = ref flatBounds.max;
			max += terraformOverride.m_LevelMaxOffset;
			val.x = terraformOverride.m_LevelBackRight.x;
			val.z = terraformOverride.m_LevelFrontRight.x;
			val2.x = terraformOverride.m_LevelBackRight.y;
			val2.z = terraformOverride.m_LevelBackLeft.y;
			val3.x = terraformOverride.m_LevelBackLeft.x;
			val3.z = terraformOverride.m_LevelFrontLeft.x;
			val4.x = terraformOverride.m_LevelFrontRight.y;
			val4.z = terraformOverride.m_LevelFrontLeft.y;
			ref float4 smooth = ref buildingTerraformData.m_Smooth;
			((float4)(ref smooth)).xy = ((float4)(ref smooth)).xy + terraformOverride.m_SmoothMinOffset;
			ref float4 smooth2 = ref buildingTerraformData.m_Smooth;
			((float4)(ref smooth2)).zw = ((float4)(ref smooth2)).zw + terraformOverride.m_SmoothMaxOffset;
			buildingTerraformData.m_HeightOffset = terraformOverride.m_HeightOffset;
			buildingTerraformData.m_DontRaise = terraformOverride.m_DontRaise;
			buildingTerraformData.m_DontLower = terraformOverride.m_DontLower;
		}
		float3 val5 = flatBounds.min.x + val;
		float3 val6 = flatBounds.min.y + val2;
		float3 val7 = flatBounds.max.x - val3;
		float3 val8 = flatBounds.max.y - val4;
		float3 val9 = (val5 + val7) * 0.5f;
		float3 val10 = (val6 + val8) * 0.5f;
		buildingTerraformData.m_FlatX0 = math.min(val5, math.max(val9, val7));
		buildingTerraformData.m_FlatZ0 = math.min(val6, math.max(val10, val8));
		buildingTerraformData.m_FlatX1 = math.max(val7, math.min(val9, val5));
		buildingTerraformData.m_FlatZ1 = math.max(val8, math.min(val10, val6));
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
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<EconomyParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_547773813_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
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
	public BuildingInitializeSystem()
	{
	}
}
