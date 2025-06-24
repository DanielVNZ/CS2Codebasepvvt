using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Areas;
using Game.Common;
using Game.Economy;
using Game.Net;
using Game.Rendering;
using Game.Routes;
using Game.Simulation;
using Game.Zones;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Prefabs;

[CompilerGenerated]
public class InfoviewInitializeSystem : GameSystemBase
{
	private struct InfoModeData
	{
		public Entity m_Mode;

		public int m_Priority;
	}

	[BurstCompile]
	private struct FindInfoviewJob : IJobChunk
	{
		private struct InfoviewSearchData
		{
			public CoverageService m_CoverageService;

			public Game.Zones.AreaType m_AreaType;

			public bool m_IsOffice;

			public TransportType m_TransportType;

			public RouteType m_RouteType;

			public MapFeature m_MapFeature;

			public MaintenanceType m_MaintenanceType;

			public TerraformingTarget m_TerraformingTarget;

			public PollutionType m_PollutionTypes;

			public WaterType m_WaterTypes;

			public ulong m_BuildingTypes;

			public uint m_VehicleTypes;

			public uint m_NetStatusTypes;

			public int m_BuildingPriority;

			public int m_VehiclePriority;

			public int m_ZonePriority;

			public int m_TransportStopPriority;

			public int m_RoutePriority;

			public int m_CoveragePriority;

			public int m_ExtractorAreaPriority;

			public int m_MaintenanceDepotPriority;

			public int m_TerraformingPriority;

			public int m_WindPriority;

			public int m_PollutionPriority;

			public int m_WaterPriority;

			public int m_FlowPriority;

			public int m_NetStatusPriority;
		}

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_InfoviewChunks;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_InfomodeChunks;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<CoverageData> m_CoverageType;

		[ReadOnly]
		public ComponentTypeHandle<HospitalData> m_HospitalType;

		[ReadOnly]
		public ComponentTypeHandle<PowerPlantData> m_PowerPlantType;

		[ReadOnly]
		public ComponentTypeHandle<TransformerData> m_TransformerType;

		[ReadOnly]
		public ComponentTypeHandle<BatteryData> m_BatteryType;

		[ReadOnly]
		public ComponentTypeHandle<WaterPumpingStationData> m_WaterPumpingStationType;

		[ReadOnly]
		public ComponentTypeHandle<WaterTowerData> m_WaterTowerType;

		[ReadOnly]
		public ComponentTypeHandle<SewageOutletData> m_SewageOutletType;

		[ReadOnly]
		public ComponentTypeHandle<TransportDepotData> m_TransportDepotType;

		[ReadOnly]
		public ComponentTypeHandle<TransportStationData> m_TransportStationType;

		[ReadOnly]
		public ComponentTypeHandle<GarbageFacilityData> m_GarbageFacilityType;

		[ReadOnly]
		public ComponentTypeHandle<FireStationData> m_FireStationType;

		[ReadOnly]
		public ComponentTypeHandle<PoliceStationData> m_PoliceStationType;

		[ReadOnly]
		public ComponentTypeHandle<MaintenanceDepotData> m_MaintenanceDepotType;

		[ReadOnly]
		public ComponentTypeHandle<PostFacilityData> m_PostFacilityDataType;

		[ReadOnly]
		public ComponentTypeHandle<TelecomFacilityData> m_TelecomFacilityDataType;

		[ReadOnly]
		public ComponentTypeHandle<SchoolData> m_SchoolDataType;

		[ReadOnly]
		public ComponentTypeHandle<ParkData> m_ParkDataType;

		[ReadOnly]
		public ComponentTypeHandle<EmergencyShelterData> m_EmergencyShelterDataType;

		[ReadOnly]
		public ComponentTypeHandle<DisasterFacilityData> m_DisasterFacilityDataType;

		[ReadOnly]
		public ComponentTypeHandle<FirewatchTowerData> m_FirewatchTowerDataType;

		[ReadOnly]
		public ComponentTypeHandle<DeathcareFacilityData> m_DeathcareFacilityDataType;

		[ReadOnly]
		public ComponentTypeHandle<PrisonData> m_PrisonDataType;

		[ReadOnly]
		public ComponentTypeHandle<AdminBuildingData> m_AdminBuildingDataType;

		[ReadOnly]
		public ComponentTypeHandle<WelfareOfficeData> m_WelfareOfficeDataType;

		[ReadOnly]
		public ComponentTypeHandle<ResearchFacilityData> m_ResearchFacilityDataType;

		[ReadOnly]
		public ComponentTypeHandle<ParkingFacilityData> m_ParkingFacilityDataType;

		[ReadOnly]
		public ComponentTypeHandle<PowerLineData> m_PowerLineType;

		[ReadOnly]
		public ComponentTypeHandle<PipelineData> m_PipelineType;

		[ReadOnly]
		public ComponentTypeHandle<ElectricityConnectionData> m_ElectricityConnectionType;

		[ReadOnly]
		public ComponentTypeHandle<WaterPipeConnectionData> m_WaterPipeConnectionType;

		[ReadOnly]
		public ComponentTypeHandle<ResourceConnectionData> m_ResourceConnectionType;

		[ReadOnly]
		public ComponentTypeHandle<ZoneData> m_ZoneType;

		[ReadOnly]
		public ComponentTypeHandle<TransportStopData> m_TransportStopType;

		[ReadOnly]
		public ComponentTypeHandle<RouteData> m_RouteType;

		[ReadOnly]
		public ComponentTypeHandle<TransportLineData> m_TransportLineType;

		[ReadOnly]
		public ComponentTypeHandle<ExtractorAreaData> m_ExtractorAreaType;

		[ReadOnly]
		public ComponentTypeHandle<TerraformingData> m_TerraformingType;

		[ReadOnly]
		public ComponentTypeHandle<WindPoweredData> m_WindPoweredType;

		[ReadOnly]
		public ComponentTypeHandle<WaterPoweredData> m_WaterPoweredType;

		[ReadOnly]
		public ComponentTypeHandle<GroundWaterPoweredData> m_GroundWaterPoweredType;

		[ReadOnly]
		public ComponentTypeHandle<PollutionData> m_PollutionType;

		[ReadOnly]
		public ComponentTypeHandle<SpawnableBuildingData> m_SpawnableBuildingType;

		[ReadOnly]
		public ComponentTypeHandle<BuildingPropertyData> m_BuildingPropertyType;

		[ReadOnly]
		public ComponentTypeHandle<ServiceUpgradeData> m_ServiceUpgradeType;

		[ReadOnly]
		public BufferTypeHandle<InfoviewMode> m_InfoviewModeType;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewCoverageData> m_InfoviewCoverageType;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewAvailabilityData> m_InfoviewAvailabilityType;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewBuildingData> m_InfoviewBuildingType;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewVehicleData> m_InfoviewVehicleType;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewTransportStopData> m_InfoviewTransportStopType;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewRouteData> m_InfoviewRouteType;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewHeatmapData> m_InfoviewHeatmapType;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewObjectStatusData> m_InfoviewObjectStatusType;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewNetStatusData> m_InfoviewNetStatusType;

		public BufferTypeHandle<PlaceableInfoviewItem> m_PlaceableInfoviewType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_062a: Unknown result type (might be due to invalid IL or missing references)
			//IL_062f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0636: Unknown result type (might be due to invalid IL or missing references)
			//IL_0645: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bbc: Unknown result type (might be due to invalid IL or missing references)
			//IL_065c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0661: Unknown result type (might be due to invalid IL or missing references)
			//IL_066a: Unknown result type (might be due to invalid IL or missing references)
			//IL_066f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0678: Unknown result type (might be due to invalid IL or missing references)
			//IL_067d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0686: Unknown result type (might be due to invalid IL or missing references)
			//IL_068b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0694: Unknown result type (might be due to invalid IL or missing references)
			//IL_0699: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06be: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06da: Unknown result type (might be due to invalid IL or missing references)
			//IL_06df: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0704: Unknown result type (might be due to invalid IL or missing references)
			//IL_0709: Unknown result type (might be due to invalid IL or missing references)
			//IL_0712: Unknown result type (might be due to invalid IL or missing references)
			//IL_0717: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bcf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bda: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c05: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c07: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c3b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c40: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ade: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b1f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b21: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b55: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b5a: Unknown result type (might be due to invalid IL or missing references)
			InfoviewSearchData searchData = default(InfoviewSearchData);
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool num = ((ArchetypeChunk)(ref chunk)).Has<SpawnableBuildingData>(ref m_SpawnableBuildingType);
			bool flag4 = ((ArchetypeChunk)(ref chunk)).Has<ServiceUpgradeData>(ref m_ServiceUpgradeType);
			if (!num && !flag4)
			{
				CheckBuildingType<HospitalData>(ref searchData.m_BuildingTypes, ref searchData.m_BuildingPriority, BuildingType.Hospital, chunk, m_HospitalType);
				CheckBuildingType<PowerPlantData>(ref searchData.m_BuildingTypes, ref searchData.m_BuildingPriority, BuildingType.PowerPlant, chunk, m_PowerPlantType);
				CheckBuildingType<TransformerData>(ref searchData.m_BuildingTypes, ref searchData.m_BuildingPriority, BuildingType.Transformer, chunk, m_TransformerType);
				CheckBuildingType<BatteryData>(ref searchData.m_BuildingTypes, ref searchData.m_BuildingPriority, BuildingType.Battery, chunk, m_BatteryType);
				CheckBuildingType<WaterPumpingStationData>(ref searchData.m_BuildingTypes, ref searchData.m_BuildingPriority, BuildingType.FreshWaterBuilding, chunk, m_WaterPumpingStationType);
				CheckBuildingType<WaterTowerData>(ref searchData.m_BuildingTypes, ref searchData.m_BuildingPriority, BuildingType.FreshWaterBuilding, chunk, m_WaterTowerType);
				CheckBuildingType<SewageOutletData>(ref searchData.m_BuildingTypes, ref searchData.m_BuildingPriority, BuildingType.SewageBuilding, chunk, m_SewageOutletType);
				CheckBuildingType<TransportDepotData>(ref searchData.m_BuildingTypes, ref searchData.m_BuildingPriority, BuildingType.TransportDepot, chunk, m_TransportDepotType);
				CheckBuildingType<TransportStationData>(ref searchData.m_BuildingTypes, ref searchData.m_BuildingPriority, BuildingType.TransportStation, chunk, m_TransportStationType);
				CheckBuildingType<GarbageFacilityData>(ref searchData.m_BuildingTypes, ref searchData.m_BuildingPriority, BuildingType.GarbageFacility, chunk, m_GarbageFacilityType);
				CheckBuildingType<FireStationData>(ref searchData.m_BuildingTypes, ref searchData.m_BuildingPriority, BuildingType.FireStation, chunk, m_FireStationType);
				CheckBuildingType<PoliceStationData>(ref searchData.m_BuildingTypes, ref searchData.m_BuildingPriority, BuildingType.PoliceStation, chunk, m_PoliceStationType);
				CheckBuildingType<PostFacilityData>(ref searchData.m_BuildingTypes, ref searchData.m_BuildingPriority, BuildingType.PostFacility, chunk, m_PostFacilityDataType);
				CheckBuildingType<TelecomFacilityData>(ref searchData.m_BuildingTypes, ref searchData.m_BuildingPriority, BuildingType.TelecomFacility, chunk, m_TelecomFacilityDataType);
				CheckBuildingType<SchoolData>(ref searchData.m_BuildingTypes, ref searchData.m_BuildingPriority, BuildingType.School, chunk, m_SchoolDataType);
				CheckBuildingType<ParkData>(ref searchData.m_BuildingTypes, ref searchData.m_BuildingPriority, BuildingType.Park, chunk, m_ParkDataType);
				CheckBuildingType<EmergencyShelterData>(ref searchData.m_BuildingTypes, ref searchData.m_BuildingPriority, BuildingType.EmergencyShelter, chunk, m_EmergencyShelterDataType);
				CheckBuildingType<DisasterFacilityData>(ref searchData.m_BuildingTypes, ref searchData.m_BuildingPriority, BuildingType.DisasterFacility, chunk, m_DisasterFacilityDataType);
				CheckBuildingType<FirewatchTowerData>(ref searchData.m_BuildingTypes, ref searchData.m_BuildingPriority, BuildingType.FirewatchTower, chunk, m_FirewatchTowerDataType);
				CheckBuildingType<DeathcareFacilityData>(ref searchData.m_BuildingTypes, ref searchData.m_BuildingPriority, BuildingType.DeathcareFacility, chunk, m_DeathcareFacilityDataType);
				CheckBuildingType<PrisonData>(ref searchData.m_BuildingTypes, ref searchData.m_BuildingPriority, BuildingType.Prison, chunk, m_PrisonDataType);
				CheckBuildingType<AdminBuildingData>(ref searchData.m_BuildingTypes, ref searchData.m_BuildingPriority, BuildingType.AdminBuilding, chunk, m_AdminBuildingDataType);
				CheckBuildingType<WelfareOfficeData>(ref searchData.m_BuildingTypes, ref searchData.m_BuildingPriority, BuildingType.WelfareOffice, chunk, m_WelfareOfficeDataType);
				CheckBuildingType<ResearchFacilityData>(ref searchData.m_BuildingTypes, ref searchData.m_BuildingPriority, BuildingType.ResearchFacility, chunk, m_ResearchFacilityDataType);
				CheckBuildingType<ParkingFacilityData>(ref searchData.m_BuildingTypes, ref searchData.m_BuildingPriority, BuildingType.ParkingFacility, chunk, m_ParkingFacilityDataType);
				if ((searchData.m_BuildingTypes & 0x4000006) != 0L)
				{
					searchData.m_NetStatusPriority = 100;
					searchData.m_NetStatusTypes = 96u;
				}
				if ((searchData.m_BuildingTypes & 0x18) != 0L)
				{
					searchData.m_WaterPriority = 10000;
					flag = true;
				}
				if ((searchData.m_BuildingTypes & 8) != 0L)
				{
					searchData.m_NetStatusPriority = 100;
					searchData.m_NetStatusTypes = 128u;
				}
				if ((searchData.m_BuildingTypes & 0x10) != 0L)
				{
					searchData.m_NetStatusPriority = 100;
					searchData.m_NetStatusTypes = 256u;
				}
				if ((searchData.m_BuildingTypes & 0x80) != 0L)
				{
					searchData.m_RouteType = RouteType.TransportLine;
					searchData.m_RoutePriority = 10000;
				}
				if ((searchData.m_BuildingTypes & 0x100) != 0L)
				{
					searchData.m_VehicleTypes |= 256u;
					searchData.m_VehiclePriority = 10000;
				}
				if (((ArchetypeChunk)(ref chunk)).Has<TransportStopData>(ref m_TransportStopType))
				{
					searchData.m_TransportStopPriority = 1000000;
					searchData.m_RoutePriority = 10000;
					flag = true;
				}
				else if (((ArchetypeChunk)(ref chunk)).Has<RouteData>(ref m_RouteType))
				{
					searchData.m_RoutePriority = 1000000;
					searchData.m_TransportStopPriority = 10000;
					flag = true;
				}
				if (((ArchetypeChunk)(ref chunk)).Has<MaintenanceDepotData>(ref m_MaintenanceDepotType))
				{
					searchData.m_MaintenanceDepotPriority = 1000000;
					flag = true;
				}
				if (((ArchetypeChunk)(ref chunk)).Has<TerraformingData>(ref m_TerraformingType))
				{
					searchData.m_TerraformingPriority = 1000000;
					flag = true;
				}
				if (((ArchetypeChunk)(ref chunk)).Has<CoverageData>(ref m_CoverageType))
				{
					searchData.m_CoverageService = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CoverageData>(ref m_CoverageType)[0].m_Service;
					searchData.m_CoveragePriority = 10000;
				}
				if (((ArchetypeChunk)(ref chunk)).Has<WindPoweredData>(ref m_WindPoweredType))
				{
					searchData.m_WindPriority = 10000;
				}
				if (((ArchetypeChunk)(ref chunk)).Has<WaterPoweredData>(ref m_WaterPoweredType))
				{
					searchData.m_WaterPriority = 10000;
					searchData.m_WaterTypes |= WaterType.Flowing;
				}
				if (((ArchetypeChunk)(ref chunk)).Has<GroundWaterPoweredData>(ref m_GroundWaterPoweredType))
				{
					searchData.m_WaterPriority = 10000;
					searchData.m_WaterTypes |= WaterType.Ground;
				}
				if (((ArchetypeChunk)(ref chunk)).Has<ExtractorAreaData>(ref m_ExtractorAreaType))
				{
					searchData.m_ExtractorAreaPriority = 10000;
					flag = true;
				}
				else if (((ArchetypeChunk)(ref chunk)).Has<BuildingPropertyData>(ref m_BuildingPropertyType))
				{
					flag = true;
				}
				if (((ArchetypeChunk)(ref chunk)).Has<ZoneData>(ref m_ZoneType))
				{
					searchData.m_ZonePriority = 1000000;
					searchData.m_PollutionPriority = 10000;
					flag = true;
				}
				else if (((ArchetypeChunk)(ref chunk)).Has<PollutionData>(ref m_PollutionType))
				{
					searchData.m_PollutionPriority = 100;
					flag = true;
				}
			}
			if (!num)
			{
				if (((ArchetypeChunk)(ref chunk)).Has<PowerLineData>(ref m_PowerLineType))
				{
					searchData.m_NetStatusPriority = 1000000;
					flag = true;
					flag2 = true;
				}
				if (((ArchetypeChunk)(ref chunk)).Has<PipelineData>(ref m_PipelineType))
				{
					searchData.m_NetStatusPriority = 1000000;
					flag = true;
					flag3 = true;
				}
			}
			BufferAccessor<PlaceableInfoviewItem> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<PlaceableInfoviewItem>(ref m_PlaceableInfoviewType);
			NativeParallelHashMap<Entity, int> infomodeScores = default(NativeParallelHashMap<Entity, int>);
			infomodeScores._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
			NativeList<InfoModeData> supplementalModes = default(NativeList<InfoModeData>);
			supplementalModes._002Ector(10, AllocatorHandle.op_Implicit((Allocator)2));
			if (flag)
			{
				NativeArray<ZoneData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ZoneData>(ref m_ZoneType);
				NativeArray<TransportStopData> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TransportStopData>(ref m_TransportStopType);
				NativeArray<RouteData> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<RouteData>(ref m_RouteType);
				NativeArray<TransportLineData> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TransportLineData>(ref m_TransportLineType);
				NativeArray<ExtractorAreaData> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ExtractorAreaData>(ref m_ExtractorAreaType);
				NativeArray<BuildingPropertyData> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<BuildingPropertyData>(ref m_BuildingPropertyType);
				NativeArray<MaintenanceDepotData> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<MaintenanceDepotData>(ref m_MaintenanceDepotType);
				NativeArray<TerraformingData> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TerraformingData>(ref m_TerraformingType);
				NativeArray<PollutionData> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PollutionData>(ref m_PollutionType);
				NativeArray<WaterPumpingStationData> nativeArray10 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WaterPumpingStationData>(ref m_WaterPumpingStationType);
				NativeArray<SewageOutletData> nativeArray11 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<SewageOutletData>(ref m_SewageOutletType);
				NativeArray<ElectricityConnectionData> nativeArray12 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ElectricityConnectionData>(ref m_ElectricityConnectionType);
				NativeArray<WaterPipeConnectionData> nativeArray13 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WaterPipeConnectionData>(ref m_WaterPipeConnectionType);
				NativeArray<ResourceConnectionData> nativeArray14 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ResourceConnectionData>(ref m_ResourceConnectionType);
				bool flag5 = ((ArchetypeChunk)(ref chunk)).Has<WaterPoweredData>(ref m_WaterPoweredType);
				for (int i = 0; i < bufferAccessor.Length; i++)
				{
					if (searchData.m_ZonePriority != 0 && nativeArray.Length != 0)
					{
						ZoneData zoneData = nativeArray[i];
						searchData.m_AreaType = zoneData.m_AreaType;
						searchData.m_IsOffice = (zoneData.m_ZoneFlags & ZoneFlags.Office) != 0;
					}
					if (searchData.m_TransportStopPriority != 0)
					{
						if (nativeArray2.Length != 0)
						{
							searchData.m_TransportType = nativeArray2[i].m_TransportType;
						}
						else if (nativeArray4.Length != 0)
						{
							searchData.m_TransportType = nativeArray4[i].m_TransportType;
						}
						else if (nativeArray3.Length != 0)
						{
							searchData.m_TransportType = TransportType.None;
						}
					}
					if (searchData.m_RoutePriority != 0)
					{
						if (nativeArray2.Length != 0)
						{
							searchData.m_RouteType = RouteType.TransportLine;
						}
						else if (nativeArray3.Length != 0)
						{
							searchData.m_RouteType = nativeArray3[i].m_Type;
						}
					}
					if (searchData.m_ExtractorAreaPriority != 0 && nativeArray5.Length != 0)
					{
						ExtractorAreaData extractorAreaData = nativeArray5[i];
						if (extractorAreaData.m_RequireNaturalResource)
						{
							searchData.m_MapFeature = extractorAreaData.m_MapFeature;
						}
					}
					if (nativeArray6.Length != 0 && EconomyUtils.IsExtractorResource(nativeArray6[i].m_AllowedManufactured))
					{
						searchData.m_BuildingPriority = 1000000;
						searchData.m_BuildingTypes |= 4294967296uL;
					}
					if (searchData.m_MaintenanceDepotPriority != 0 && nativeArray7.Length != 0)
					{
						searchData.m_MaintenanceType = nativeArray7[i].m_MaintenanceType;
					}
					if (searchData.m_TerraformingPriority != 0 && nativeArray8.Length != 0)
					{
						searchData.m_TerraformingTarget = nativeArray8[i].m_Target;
					}
					if (searchData.m_PollutionPriority != 0)
					{
						if (nativeArray.Length != 0)
						{
							ZoneData zoneData2 = nativeArray[i];
							if (zoneData2.m_AreaType == Game.Zones.AreaType.Residential)
							{
								searchData.m_PollutionTypes |= PollutionType.Ground;
							}
							if (zoneData2.m_AreaType == Game.Zones.AreaType.Industrial)
							{
								searchData.m_PollutionTypes |= PollutionType.Ground | PollutionType.Air;
							}
						}
						else if (nativeArray9.Length != 0)
						{
							PollutionData pollutionData = nativeArray9[i];
							searchData.m_PollutionTypes = PollutionType.None;
							if (pollutionData.m_GroundPollution > 0f)
							{
								searchData.m_PollutionTypes |= PollutionType.Ground;
							}
							if (pollutionData.m_AirPollution > 0f)
							{
								searchData.m_PollutionTypes |= PollutionType.Air;
							}
							if (pollutionData.m_NoisePollution > 0f)
							{
								searchData.m_PollutionTypes |= PollutionType.Noise;
							}
						}
					}
					if (searchData.m_WaterPriority != 0 && (nativeArray10.Length != 0 || nativeArray11.Length != 0 || flag5))
					{
						searchData.m_WaterTypes = WaterType.None;
						if (nativeArray10.Length != 0)
						{
							WaterPumpingStationData waterPumpingStationData = nativeArray10[i];
							if ((waterPumpingStationData.m_Types & AllowedWaterTypes.Groundwater) != AllowedWaterTypes.None)
							{
								searchData.m_WaterTypes |= WaterType.Ground;
							}
							if ((waterPumpingStationData.m_Types & AllowedWaterTypes.SurfaceWater) != AllowedWaterTypes.None)
							{
								searchData.m_WaterTypes |= WaterType.Flowing;
							}
						}
						if (nativeArray11.Length != 0)
						{
							searchData.m_WaterTypes |= WaterType.Flowing;
						}
						if (flag5)
						{
							searchData.m_WaterTypes |= WaterType.Flowing;
						}
					}
					if (searchData.m_NetStatusPriority != 0)
					{
						if (nativeArray12.Length != 0 && flag2)
						{
							switch (nativeArray12[i].m_Voltage)
							{
							case ElectricityConnection.Voltage.Low:
								searchData.m_NetStatusTypes |= 32u;
								break;
							case ElectricityConnection.Voltage.High:
								searchData.m_NetStatusTypes |= 64u;
								break;
							}
						}
						if (nativeArray13.Length != 0 && flag3)
						{
							WaterPipeConnectionData waterPipeConnectionData = nativeArray13[i];
							if (waterPipeConnectionData.m_FreshCapacity != 0)
							{
								searchData.m_NetStatusTypes |= 128u;
							}
							if (waterPipeConnectionData.m_SewageCapacity != 0)
							{
								searchData.m_NetStatusTypes |= 256u;
							}
						}
						if (nativeArray14.Length != 0 && flag3 && nativeArray14[i].m_Resource == Resource.Oil)
						{
							searchData.m_NetStatusTypes |= 512u;
						}
					}
					CalculateInfomodeScores(searchData, infomodeScores);
					supplementalModes.Clear();
					int bestScore;
					Entity bestInfoView = GetBestInfoView(searchData, infomodeScores, supplementalModes, out bestScore);
					DynamicBuffer<PlaceableInfoviewItem> val = bufferAccessor[i];
					val.Clear();
					if (bestInfoView != Entity.Null)
					{
						val.Capacity = 1 + supplementalModes.Length;
						val.Add(new PlaceableInfoviewItem
						{
							m_Item = bestInfoView,
							m_Priority = bestScore
						});
						for (int j = 0; j < supplementalModes.Length; j++)
						{
							InfoModeData infoModeData = supplementalModes[j];
							val.Add(new PlaceableInfoviewItem
							{
								m_Item = infoModeData.m_Mode,
								m_Priority = infoModeData.m_Priority
							});
						}
					}
				}
			}
			else
			{
				CalculateInfomodeScores(searchData, infomodeScores);
				supplementalModes.Clear();
				int bestScore2;
				Entity bestInfoView2 = GetBestInfoView(searchData, infomodeScores, supplementalModes, out bestScore2);
				for (int k = 0; k < bufferAccessor.Length; k++)
				{
					DynamicBuffer<PlaceableInfoviewItem> val2 = bufferAccessor[k];
					val2.Clear();
					if (bestInfoView2 != Entity.Null)
					{
						val2.Capacity = 1 + supplementalModes.Length;
						val2.Add(new PlaceableInfoviewItem
						{
							m_Item = bestInfoView2,
							m_Priority = bestScore2
						});
						for (int l = 0; l < supplementalModes.Length; l++)
						{
							InfoModeData infoModeData2 = supplementalModes[l];
							val2.Add(new PlaceableInfoviewItem
							{
								m_Item = infoModeData2.m_Mode,
								m_Priority = infoModeData2.m_Priority
							});
						}
					}
				}
			}
			infomodeScores.Dispose();
			supplementalModes.Dispose();
		}

		private void CheckBuildingType<T>(ref ulong mask, ref int priority, BuildingType type, ArchetypeChunk chunk, ComponentTypeHandle<T> componentType) where T : struct, IComponentData
		{
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<T>(ref componentType);
			mask = math.select(mask, mask | (ulong)(1L << (int)type), flag);
			priority = math.select(priority, 1000000, flag);
		}

		private void CalculateInfomodeScores(InfoviewSearchData searchData, NativeParallelHashMap<Entity, int> infomodeScores)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_039e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_040c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_0471: Unknown result type (might be due to invalid IL or missing references)
			//IL_0476: Unknown result type (might be due to invalid IL or missing references)
			//IL_0480: Unknown result type (might be due to invalid IL or missing references)
			//IL_0485: Unknown result type (might be due to invalid IL or missing references)
			//IL_03da: Unknown result type (might be due to invalid IL or missing references)
			//IL_03df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0541: Unknown result type (might be due to invalid IL or missing references)
			//IL_0546: Unknown result type (might be due to invalid IL or missing references)
			//IL_043c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0441: Unknown result type (might be due to invalid IL or missing references)
			//IL_05af: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_057d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0582: Unknown result type (might be due to invalid IL or missing references)
			//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_050f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0514: Unknown result type (might be due to invalid IL or missing references)
			infomodeScores.Clear();
			for (int i = 0; i < m_InfomodeChunks.Length; i++)
			{
				ArchetypeChunk val = m_InfomodeChunks[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				if (searchData.m_BuildingPriority != 0)
				{
					NativeArray<InfoviewBuildingData> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<InfoviewBuildingData>(ref m_InfoviewBuildingType);
					for (int j = 0; j < nativeArray2.Length; j++)
					{
						InfoviewBuildingData infoviewBuildingData = nativeArray2[j];
						int score = math.select(-1, searchData.m_BuildingPriority, (searchData.m_BuildingTypes & (ulong)(1L << (int)infoviewBuildingData.m_Type)) != 0);
						AddInfomodeScore(infomodeScores, nativeArray[j], score);
					}
				}
				if (searchData.m_MaintenanceDepotPriority != 0)
				{
					NativeArray<InfoviewBuildingData> nativeArray3 = ((ArchetypeChunk)(ref val)).GetNativeArray<InfoviewBuildingData>(ref m_InfoviewBuildingType);
					for (int k = 0; k < nativeArray3.Length; k++)
					{
						InfoviewBuildingData infoviewBuildingData2 = nativeArray3[k];
						int score2 = math.select(-1, searchData.m_MaintenanceDepotPriority, (searchData.m_MaintenanceType & GetMaintenanceType(infoviewBuildingData2.m_Type)) != 0);
						AddInfomodeScore(infomodeScores, nativeArray[k], score2);
					}
				}
				if (searchData.m_ZonePriority != 0)
				{
					NativeArray<InfoviewAvailabilityData> nativeArray4 = ((ArchetypeChunk)(ref val)).GetNativeArray<InfoviewAvailabilityData>(ref m_InfoviewAvailabilityType);
					for (int l = 0; l < nativeArray4.Length; l++)
					{
						InfoviewAvailabilityData infoviewAvailabilityData = nativeArray4[l];
						int score3 = math.select(-1, searchData.m_ZonePriority, searchData.m_AreaType == infoviewAvailabilityData.m_AreaType && searchData.m_IsOffice == infoviewAvailabilityData.m_Office);
						AddInfomodeScore(infomodeScores, nativeArray[l], score3);
					}
				}
				if (searchData.m_TransportStopPriority != 0)
				{
					NativeArray<InfoviewTransportStopData> nativeArray5 = ((ArchetypeChunk)(ref val)).GetNativeArray<InfoviewTransportStopData>(ref m_InfoviewTransportStopType);
					for (int m = 0; m < nativeArray5.Length; m++)
					{
						InfoviewTransportStopData infoviewTransportStopData = nativeArray5[m];
						int score4 = math.select(-1, searchData.m_TransportStopPriority, searchData.m_TransportType == infoviewTransportStopData.m_Type);
						AddInfomodeScore(infomodeScores, nativeArray[m], score4);
					}
				}
				if (searchData.m_RoutePriority != 0)
				{
					NativeArray<InfoviewRouteData> nativeArray6 = ((ArchetypeChunk)(ref val)).GetNativeArray<InfoviewRouteData>(ref m_InfoviewRouteType);
					for (int n = 0; n < nativeArray6.Length; n++)
					{
						InfoviewRouteData infoviewRouteData = nativeArray6[n];
						int score5 = math.select(-1, searchData.m_RoutePriority, searchData.m_RouteType == infoviewRouteData.m_Type);
						AddInfomodeScore(infomodeScores, nativeArray[n], score5);
					}
				}
				if (searchData.m_TerraformingPriority != 0)
				{
					NativeArray<InfoviewHeatmapData> nativeArray7 = ((ArchetypeChunk)(ref val)).GetNativeArray<InfoviewHeatmapData>(ref m_InfoviewHeatmapType);
					for (int num = 0; num < nativeArray7.Length; num++)
					{
						InfoviewHeatmapData infoviewHeatmapData = nativeArray7[num];
						int score6 = math.select(-1, searchData.m_TerraformingPriority, searchData.m_TerraformingTarget == GetTerraformingTarget(infoviewHeatmapData.m_Type));
						AddInfomodeScore(infomodeScores, nativeArray[num], score6);
					}
				}
				if (searchData.m_VehiclePriority != 0)
				{
					NativeArray<InfoviewVehicleData> nativeArray8 = ((ArchetypeChunk)(ref val)).GetNativeArray<InfoviewVehicleData>(ref m_InfoviewVehicleType);
					for (int num2 = 0; num2 < nativeArray8.Length; num2++)
					{
						InfoviewVehicleData infoviewVehicleData = nativeArray8[num2];
						int score7 = math.select(-1, searchData.m_VehiclePriority, (searchData.m_VehicleTypes & (uint)(1 << (int)infoviewVehicleData.m_Type)) != 0);
						AddInfomodeScore(infomodeScores, nativeArray[num2], score7);
					}
				}
				if (searchData.m_CoveragePriority != 0)
				{
					NativeArray<InfoviewCoverageData> nativeArray9 = ((ArchetypeChunk)(ref val)).GetNativeArray<InfoviewCoverageData>(ref m_InfoviewCoverageType);
					for (int num3 = 0; num3 < nativeArray9.Length; num3++)
					{
						InfoviewCoverageData infoviewCoverageData = nativeArray9[num3];
						int score8 = math.select(-1, searchData.m_CoveragePriority, searchData.m_CoverageService == infoviewCoverageData.m_Service);
						AddInfomodeScore(infomodeScores, nativeArray[num3], score8);
					}
				}
				if (searchData.m_WaterPriority != 0)
				{
					NativeArray<InfoviewHeatmapData> nativeArray10 = ((ArchetypeChunk)(ref val)).GetNativeArray<InfoviewHeatmapData>(ref m_InfoviewHeatmapType);
					for (int num4 = 0; num4 < nativeArray10.Length; num4++)
					{
						InfoviewHeatmapData infoviewHeatmapData2 = nativeArray10[num4];
						int score9 = math.select(-1, searchData.m_WaterPriority, (searchData.m_WaterTypes & GetWaterType(infoviewHeatmapData2.m_Type)) != 0);
						AddInfomodeScore(infomodeScores, nativeArray[num4], score9);
					}
				}
				if (searchData.m_WindPriority != 0)
				{
					NativeArray<InfoviewHeatmapData> nativeArray11 = ((ArchetypeChunk)(ref val)).GetNativeArray<InfoviewHeatmapData>(ref m_InfoviewHeatmapType);
					for (int num5 = 0; num5 < nativeArray11.Length; num5++)
					{
						InfoviewHeatmapData infoviewHeatmapData3 = nativeArray11[num5];
						int score10 = math.select(-1, searchData.m_WindPriority, infoviewHeatmapData3.m_Type == HeatmapData.Wind);
						AddInfomodeScore(infomodeScores, nativeArray[num5], score10);
					}
				}
				if (searchData.m_ExtractorAreaPriority != 0)
				{
					NativeArray<InfoviewHeatmapData> nativeArray12 = ((ArchetypeChunk)(ref val)).GetNativeArray<InfoviewHeatmapData>(ref m_InfoviewHeatmapType);
					NativeArray<InfoviewObjectStatusData> nativeArray13 = ((ArchetypeChunk)(ref val)).GetNativeArray<InfoviewObjectStatusData>(ref m_InfoviewObjectStatusType);
					for (int num6 = 0; num6 < nativeArray12.Length; num6++)
					{
						InfoviewHeatmapData infoviewHeatmapData4 = nativeArray12[num6];
						int score11 = math.select(-1, searchData.m_ExtractorAreaPriority, searchData.m_MapFeature == GetMapFeature(infoviewHeatmapData4.m_Type));
						AddInfomodeScore(infomodeScores, nativeArray[num6], score11);
					}
					for (int num7 = 0; num7 < nativeArray13.Length; num7++)
					{
						InfoviewObjectStatusData infoviewObjectStatusData = nativeArray13[num7];
						int score12 = math.select(-1, searchData.m_ExtractorAreaPriority, searchData.m_MapFeature == GetMapFeature(infoviewObjectStatusData.m_Type));
						AddInfomodeScore(infomodeScores, nativeArray[num7], score12);
					}
				}
				if (searchData.m_PollutionPriority != 0)
				{
					NativeArray<InfoviewHeatmapData> nativeArray14 = ((ArchetypeChunk)(ref val)).GetNativeArray<InfoviewHeatmapData>(ref m_InfoviewHeatmapType);
					for (int num8 = 0; num8 < nativeArray14.Length; num8++)
					{
						InfoviewHeatmapData infoviewHeatmapData5 = nativeArray14[num8];
						int score13 = math.select(-1, searchData.m_PollutionPriority, (searchData.m_PollutionTypes & GetPollutionType(infoviewHeatmapData5.m_Type)) != 0);
						AddInfomodeScore(infomodeScores, nativeArray[num8], score13);
					}
				}
				if (searchData.m_NetStatusPriority != 0)
				{
					NativeArray<InfoviewNetStatusData> nativeArray15 = ((ArchetypeChunk)(ref val)).GetNativeArray<InfoviewNetStatusData>(ref m_InfoviewNetStatusType);
					for (int num9 = 0; num9 < nativeArray15.Length; num9++)
					{
						InfoviewNetStatusData infoviewNetStatusData = nativeArray15[num9];
						int score14 = math.select(-1, searchData.m_NetStatusPriority, (searchData.m_NetStatusTypes & (uint)(1 << (int)infoviewNetStatusData.m_Type)) != 0);
						AddInfomodeScore(infomodeScores, nativeArray[num9], score14);
					}
				}
			}
		}

		private void AddInfomodeScore(NativeParallelHashMap<Entity, int> infomodeScores, Entity entity, int score)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			if (!infomodeScores.TryAdd(entity, score))
			{
				infomodeScores[entity] = math.max(infomodeScores[entity], score);
			}
		}

		private Entity GetBestInfoView(InfoviewSearchData searchData, NativeParallelHashMap<Entity, int> infomodeScores, NativeList<InfoModeData> supplementalModes, out int bestScore)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			bestScore = int.MinValue;
			Entity result = Entity.Null;
			int num2 = default(int);
			int num3 = default(int);
			for (int i = 0; i < m_InfoviewChunks.Length; i++)
			{
				ArchetypeChunk val = m_InfoviewChunks[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				BufferAccessor<InfoviewMode> bufferAccessor = ((ArchetypeChunk)(ref val)).GetBufferAccessor<InfoviewMode>(ref m_InfoviewModeType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					DynamicBuffer<InfoviewMode> val2 = bufferAccessor[j];
					int num = 0;
					bool flag = false;
					for (int k = 0; k < val2.Length; k++)
					{
						InfoviewMode infoviewMode = val2[k];
						if (infomodeScores.TryGetValue(infoviewMode.m_Mode, ref num2))
						{
							bool flag2 = infoviewMode.m_Supplemental | infoviewMode.m_Optional;
							num += math.select(num2, 0, flag2 && num2 < 0);
							flag = flag || (flag2 && num2 > 0);
						}
					}
					if (num <= bestScore && (bestScore != 0 || val2.Length != 0))
					{
						continue;
					}
					bestScore = num;
					result = nativeArray[j];
					supplementalModes.Clear();
					if (!flag)
					{
						continue;
					}
					for (int l = 0; l < val2.Length; l++)
					{
						InfoviewMode infoviewMode2 = val2[l];
						if ((infoviewMode2.m_Supplemental | infoviewMode2.m_Optional) && infomodeScores.TryGetValue(infoviewMode2.m_Mode, ref num3) && num3 > 0)
						{
							InfoModeData infoModeData = new InfoModeData
							{
								m_Mode = infoviewMode2.m_Mode,
								m_Priority = num3
							};
							supplementalModes.Add(ref infoModeData);
						}
					}
				}
			}
			return result;
		}

		public static MapFeature GetMapFeature(HeatmapData heatmapType)
		{
			return heatmapType switch
			{
				HeatmapData.Fertility => MapFeature.FertileLand, 
				HeatmapData.Ore => MapFeature.Ore, 
				HeatmapData.Oil => MapFeature.Oil, 
				HeatmapData.Fish => MapFeature.Fish, 
				_ => MapFeature.None, 
			};
		}

		public static TerraformingTarget GetTerraformingTarget(HeatmapData heatmapType)
		{
			return heatmapType switch
			{
				HeatmapData.Fertility => TerraformingTarget.FertileLand, 
				HeatmapData.Ore => TerraformingTarget.Ore, 
				HeatmapData.Oil => TerraformingTarget.Oil, 
				HeatmapData.GroundWater => TerraformingTarget.GroundWater, 
				_ => TerraformingTarget.None, 
			};
		}

		public static MapFeature GetMapFeature(ObjectStatusType statusType)
		{
			if (statusType == ObjectStatusType.WoodResource)
			{
				return MapFeature.Forest;
			}
			return MapFeature.None;
		}

		public static MaintenanceType GetMaintenanceType(BuildingType buildingType)
		{
			return buildingType switch
			{
				BuildingType.RoadMaintenanceDepot => MaintenanceType.Road | MaintenanceType.Snow | MaintenanceType.Vehicle, 
				BuildingType.ParkMaintenanceDepot => MaintenanceType.Park, 
				_ => MaintenanceType.None, 
			};
		}

		public static PollutionType GetPollutionType(HeatmapData heatmapType)
		{
			return heatmapType switch
			{
				HeatmapData.GroundPollution => PollutionType.Ground, 
				HeatmapData.GroundWater => PollutionType.Ground, 
				HeatmapData.Wind => PollutionType.Air, 
				_ => PollutionType.None, 
			};
		}

		public static WaterType GetWaterType(HeatmapData heatmapType)
		{
			return heatmapType switch
			{
				HeatmapData.GroundWater => WaterType.Ground, 
				HeatmapData.GroundWaterPollution => WaterType.Ground, 
				HeatmapData.WaterFlow => WaterType.Flowing, 
				HeatmapData.WaterPollution => WaterType.Flowing, 
				_ => WaterType.None, 
			};
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[Flags]
	private enum PollutionType
	{
		None = 0,
		Ground = 1,
		Air = 2,
		Noise = 4
	}

	[Flags]
	private enum WaterType
	{
		None = 0,
		Ground = 1,
		Flowing = 2
	}

	private struct InfoviewBufferData
	{
		public Entity m_Target;

		public Entity m_Source;
	}

	[BurstCompile]
	private struct FindSubInfoviewJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<SpawnableBuildingData> m_SpawnableBuildingType;

		[ReadOnly]
		public ComponentTypeHandle<BuildingPropertyData> m_BuildingPropertyType;

		[ReadOnly]
		public BufferTypeHandle<PlaceableInfoviewItem> m_PlaceableInfoviewType;

		[ReadOnly]
		public BufferTypeHandle<SubArea> m_SubAreaType;

		[ReadOnly]
		public BufferTypeHandle<BuildingUpgradeElement> m_BuildingUpgradeElementType;

		[ReadOnly]
		public ComponentLookup<LotData> m_LotData;

		[ReadOnly]
		public ComponentLookup<ResourceData> m_ResourceData;

		[ReadOnly]
		public BufferLookup<PlaceableInfoviewItem> m_PlaceableInfoviewData;

		[ReadOnly]
		public BufferLookup<InfoviewMode> m_InfoviewModes;

		[ReadOnly]
		public BufferLookup<SubArea> m_SubAreas;

		[ReadOnly]
		public ResourcePrefabs m_ResourcePrefabs;

		public ParallelWriter<InfoviewBufferData> m_InfoViewBuffer;

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
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<SpawnableBuildingData> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<SpawnableBuildingData>(ref m_SpawnableBuildingType);
			NativeArray<BuildingPropertyData> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<BuildingPropertyData>(ref m_BuildingPropertyType);
			BufferAccessor<PlaceableInfoviewItem> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<PlaceableInfoviewItem>(ref m_PlaceableInfoviewType);
			BufferAccessor<SubArea> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<SubArea>(ref m_SubAreaType);
			BufferAccessor<BuildingUpgradeElement> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<BuildingUpgradeElement>(ref m_BuildingUpgradeElementType);
			SpawnableBuildingData spawnableBuildingData = default(SpawnableBuildingData);
			DynamicBuffer<PlaceableInfoviewItem> val2 = default(DynamicBuffer<PlaceableInfoviewItem>);
			BuildingPropertyData buildingPropertyData = default(BuildingPropertyData);
			ResourceData resourceData = default(ResourceData);
			DynamicBuffer<SubArea> subAreas = default(DynamicBuffer<SubArea>);
			DynamicBuffer<BuildingUpgradeElement> val3 = default(DynamicBuffer<BuildingUpgradeElement>);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				DynamicBuffer<PlaceableInfoviewItem> val = bufferAccessor[i];
				int priority = int.MinValue;
				Entity sourceEntity = Entity.Null;
				if (val.Length != 0)
				{
					PlaceableInfoviewItem placeableInfoviewItem = val[0];
					if (m_InfoviewModes[placeableInfoviewItem.m_Item].Length != 0)
					{
						priority = placeableInfoviewItem.m_Priority;
					}
				}
				if (CollectionUtils.TryGet<SpawnableBuildingData>(nativeArray2, i, ref spawnableBuildingData) && m_PlaceableInfoviewData.TryGetBuffer(spawnableBuildingData.m_ZonePrefab, ref val2) && val2.Length != 0)
				{
					PlaceableInfoviewItem placeableInfoviewItem2 = val2[0];
					if (m_InfoviewModes[placeableInfoviewItem2.m_Item].Length != 0 && placeableInfoviewItem2.m_Priority > priority)
					{
						priority = placeableInfoviewItem2.m_Priority;
						sourceEntity = spawnableBuildingData.m_ZonePrefab;
					}
				}
				if (CollectionUtils.TryGet<BuildingPropertyData>(nativeArray3, i, ref buildingPropertyData) && EconomyUtils.IsExtractorResource(buildingPropertyData.m_AllowedManufactured))
				{
					ResourceIterator resourceIterator = default(ResourceIterator);
					while (resourceIterator.Next())
					{
						if ((buildingPropertyData.m_AllowedManufactured & resourceIterator.resource) != Resource.NoResource && m_ResourceData.TryGetComponent(m_ResourcePrefabs[buildingPropertyData.m_AllowedManufactured], ref resourceData) && resourceData.m_RequireNaturalResource)
						{
							priority = math.min(priority, 9000);
							break;
						}
					}
				}
				if (CollectionUtils.TryGet<SubArea>(bufferAccessor2, i, ref subAreas))
				{
					CheckSubAreas(ref priority, ref sourceEntity, subAreas);
				}
				if (CollectionUtils.TryGet<BuildingUpgradeElement>(bufferAccessor3, i, ref val3))
				{
					for (int j = 0; j < val3.Length; j++)
					{
						if (m_SubAreas.TryGetBuffer(val3[j].m_Upgrade, ref subAreas))
						{
							CheckSubAreas(ref priority, ref sourceEntity, subAreas);
						}
					}
				}
				if (sourceEntity != Entity.Null)
				{
					m_InfoViewBuffer.Enqueue(new InfoviewBufferData
					{
						m_Target = nativeArray[i],
						m_Source = sourceEntity
					});
				}
			}
		}

		private void CheckSubAreas(ref int priority, ref Entity sourceEntity, DynamicBuffer<SubArea> subAreas)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<PlaceableInfoviewItem> val = default(DynamicBuffer<PlaceableInfoviewItem>);
			for (int i = 0; i < subAreas.Length; i++)
			{
				Entity prefab = subAreas[i].m_Prefab;
				if (m_LotData.HasComponent(prefab) && m_PlaceableInfoviewData.TryGetBuffer(prefab, ref val) && val.Length != 0)
				{
					PlaceableInfoviewItem placeableInfoviewItem = val[0];
					if (m_InfoviewModes[placeableInfoviewItem.m_Item].Length != 0 && placeableInfoviewItem.m_Priority > priority)
					{
						priority = placeableInfoviewItem.m_Priority;
						sourceEntity = prefab;
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct AssignInfoviewJob : IJob
	{
		public NativeQueue<InfoviewBufferData> m_InfoViewBuffer;

		public BufferLookup<PlaceableInfoviewItem> m_PlaceableInfoviewData;

		public void Execute()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			InfoviewBufferData infoviewBufferData = default(InfoviewBufferData);
			while (m_InfoViewBuffer.TryDequeue(ref infoviewBufferData))
			{
				DynamicBuffer<PlaceableInfoviewItem> val = m_PlaceableInfoviewData[infoviewBufferData.m_Target];
				DynamicBuffer<PlaceableInfoviewItem> val2 = m_PlaceableInfoviewData[infoviewBufferData.m_Source];
				val.CopyFrom(val2);
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<PrefabData> __Game_Prefabs_PrefabData_RO_ComponentTypeHandle;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		public BufferTypeHandle<InfoviewMode> __Game_Prefabs_InfoviewMode_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<InfomodeGroup> __Game_Prefabs_InfomodeGroup_RO_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<CoverageData> __Game_Prefabs_CoverageData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<HospitalData> __Game_Prefabs_HospitalData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PowerPlantData> __Game_Prefabs_PowerPlantData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TransformerData> __Game_Prefabs_TransformerData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<BatteryData> __Game_Prefabs_BatteryData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WaterPumpingStationData> __Game_Prefabs_WaterPumpingStationData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WaterTowerData> __Game_Prefabs_WaterTowerData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<SewageOutletData> __Game_Prefabs_SewageOutletData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TransportDepotData> __Game_Prefabs_TransportDepotData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TransportStationData> __Game_Prefabs_TransportStationData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<GarbageFacilityData> __Game_Prefabs_GarbageFacilityData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<FireStationData> __Game_Prefabs_FireStationData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PoliceStationData> __Game_Prefabs_PoliceStationData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<MaintenanceDepotData> __Game_Prefabs_MaintenanceDepotData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PostFacilityData> __Game_Prefabs_PostFacilityData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TelecomFacilityData> __Game_Prefabs_TelecomFacilityData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<SchoolData> __Game_Prefabs_SchoolData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ParkData> __Game_Prefabs_ParkData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<EmergencyShelterData> __Game_Prefabs_EmergencyShelterData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<DisasterFacilityData> __Game_Prefabs_DisasterFacilityData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<FirewatchTowerData> __Game_Prefabs_FirewatchTowerData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<DeathcareFacilityData> __Game_Prefabs_DeathcareFacilityData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrisonData> __Game_Prefabs_PrisonData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<AdminBuildingData> __Game_Prefabs_AdminBuildingData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WelfareOfficeData> __Game_Prefabs_WelfareOfficeData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ResearchFacilityData> __Game_Prefabs_ResearchFacilityData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ParkingFacilityData> __Game_Prefabs_ParkingFacilityData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PowerLineData> __Game_Prefabs_PowerLineData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PipelineData> __Game_Prefabs_PipelineData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ElectricityConnectionData> __Game_Prefabs_ElectricityConnectionData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WaterPipeConnectionData> __Game_Prefabs_WaterPipeConnectionData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ResourceConnectionData> __Game_Prefabs_ResourceConnectionData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ZoneData> __Game_Prefabs_ZoneData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TransportStopData> __Game_Prefabs_TransportStopData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<RouteData> __Game_Prefabs_RouteData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TransportLineData> __Game_Prefabs_TransportLineData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ExtractorAreaData> __Game_Prefabs_ExtractorAreaData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TerraformingData> __Game_Prefabs_TerraformingData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WindPoweredData> __Game_Prefabs_WindPoweredData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WaterPoweredData> __Game_Prefabs_WaterPoweredData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<GroundWaterPoweredData> __Game_Prefabs_GroundWaterPoweredData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PollutionData> __Game_Prefabs_PollutionData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<BuildingPropertyData> __Game_Prefabs_BuildingPropertyData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ServiceUpgradeData> __Game_Prefabs_ServiceUpgradeData_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<InfoviewMode> __Game_Prefabs_InfoviewMode_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewCoverageData> __Game_Prefabs_InfoviewCoverageData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewAvailabilityData> __Game_Prefabs_InfoviewAvailabilityData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewBuildingData> __Game_Prefabs_InfoviewBuildingData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewVehicleData> __Game_Prefabs_InfoviewVehicleData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewTransportStopData> __Game_Prefabs_InfoviewTransportStopData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewRouteData> __Game_Prefabs_InfoviewRouteData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewHeatmapData> __Game_Prefabs_InfoviewHeatmapData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewObjectStatusData> __Game_Prefabs_InfoviewObjectStatusData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewNetStatusData> __Game_Prefabs_InfoviewNetStatusData_RO_ComponentTypeHandle;

		public BufferTypeHandle<PlaceableInfoviewItem> __Game_Prefabs_PlaceableInfoviewItem_RW_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<PlaceableInfoviewItem> __Game_Prefabs_PlaceableInfoviewItem_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<SubArea> __Game_Prefabs_SubArea_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<BuildingUpgradeElement> __Game_Prefabs_BuildingUpgradeElement_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<LotData> __Game_Prefabs_LotData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ResourceData> __Game_Prefabs_ResourceData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<PlaceableInfoviewItem> __Game_Prefabs_PlaceableInfoviewItem_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<InfoviewMode> __Game_Prefabs_InfoviewMode_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubArea> __Game_Prefabs_SubArea_RO_BufferLookup;

		public BufferLookup<PlaceableInfoviewItem> __Game_Prefabs_PlaceableInfoviewItem_RW_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
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
			__Game_Prefabs_PrefabData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabData>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_InfoviewMode_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<InfoviewMode>(false);
			__Game_Prefabs_InfomodeGroup_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<InfomodeGroup>(true);
			__Game_Prefabs_CoverageData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CoverageData>(true);
			__Game_Prefabs_HospitalData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HospitalData>(true);
			__Game_Prefabs_PowerPlantData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PowerPlantData>(true);
			__Game_Prefabs_TransformerData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TransformerData>(true);
			__Game_Prefabs_BatteryData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BatteryData>(true);
			__Game_Prefabs_WaterPumpingStationData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WaterPumpingStationData>(true);
			__Game_Prefabs_WaterTowerData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WaterTowerData>(true);
			__Game_Prefabs_SewageOutletData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<SewageOutletData>(true);
			__Game_Prefabs_TransportDepotData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TransportDepotData>(true);
			__Game_Prefabs_TransportStationData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TransportStationData>(true);
			__Game_Prefabs_GarbageFacilityData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<GarbageFacilityData>(true);
			__Game_Prefabs_FireStationData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<FireStationData>(true);
			__Game_Prefabs_PoliceStationData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PoliceStationData>(true);
			__Game_Prefabs_MaintenanceDepotData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<MaintenanceDepotData>(true);
			__Game_Prefabs_PostFacilityData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PostFacilityData>(true);
			__Game_Prefabs_TelecomFacilityData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TelecomFacilityData>(true);
			__Game_Prefabs_SchoolData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<SchoolData>(true);
			__Game_Prefabs_ParkData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ParkData>(true);
			__Game_Prefabs_EmergencyShelterData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EmergencyShelterData>(true);
			__Game_Prefabs_DisasterFacilityData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<DisasterFacilityData>(true);
			__Game_Prefabs_FirewatchTowerData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<FirewatchTowerData>(true);
			__Game_Prefabs_DeathcareFacilityData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<DeathcareFacilityData>(true);
			__Game_Prefabs_PrisonData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrisonData>(true);
			__Game_Prefabs_AdminBuildingData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AdminBuildingData>(true);
			__Game_Prefabs_WelfareOfficeData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WelfareOfficeData>(true);
			__Game_Prefabs_ResearchFacilityData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ResearchFacilityData>(true);
			__Game_Prefabs_ParkingFacilityData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ParkingFacilityData>(true);
			__Game_Prefabs_PowerLineData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PowerLineData>(true);
			__Game_Prefabs_PipelineData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PipelineData>(true);
			__Game_Prefabs_ElectricityConnectionData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ElectricityConnectionData>(true);
			__Game_Prefabs_WaterPipeConnectionData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WaterPipeConnectionData>(true);
			__Game_Prefabs_ResourceConnectionData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ResourceConnectionData>(true);
			__Game_Prefabs_ZoneData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ZoneData>(true);
			__Game_Prefabs_TransportStopData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TransportStopData>(true);
			__Game_Prefabs_RouteData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<RouteData>(true);
			__Game_Prefabs_TransportLineData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TransportLineData>(true);
			__Game_Prefabs_ExtractorAreaData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ExtractorAreaData>(true);
			__Game_Prefabs_TerraformingData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TerraformingData>(true);
			__Game_Prefabs_WindPoweredData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WindPoweredData>(true);
			__Game_Prefabs_WaterPoweredData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WaterPoweredData>(true);
			__Game_Prefabs_GroundWaterPoweredData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<GroundWaterPoweredData>(true);
			__Game_Prefabs_PollutionData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PollutionData>(true);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<SpawnableBuildingData>(true);
			__Game_Prefabs_BuildingPropertyData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BuildingPropertyData>(true);
			__Game_Prefabs_ServiceUpgradeData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ServiceUpgradeData>(true);
			__Game_Prefabs_InfoviewMode_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<InfoviewMode>(true);
			__Game_Prefabs_InfoviewCoverageData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfoviewCoverageData>(true);
			__Game_Prefabs_InfoviewAvailabilityData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfoviewAvailabilityData>(true);
			__Game_Prefabs_InfoviewBuildingData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfoviewBuildingData>(true);
			__Game_Prefabs_InfoviewVehicleData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfoviewVehicleData>(true);
			__Game_Prefabs_InfoviewTransportStopData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfoviewTransportStopData>(true);
			__Game_Prefabs_InfoviewRouteData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfoviewRouteData>(true);
			__Game_Prefabs_InfoviewHeatmapData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfoviewHeatmapData>(true);
			__Game_Prefabs_InfoviewObjectStatusData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfoviewObjectStatusData>(true);
			__Game_Prefabs_InfoviewNetStatusData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfoviewNetStatusData>(true);
			__Game_Prefabs_PlaceableInfoviewItem_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<PlaceableInfoviewItem>(false);
			__Game_Prefabs_PlaceableInfoviewItem_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<PlaceableInfoviewItem>(true);
			__Game_Prefabs_SubArea_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubArea>(true);
			__Game_Prefabs_BuildingUpgradeElement_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<BuildingUpgradeElement>(true);
			__Game_Prefabs_LotData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LotData>(true);
			__Game_Prefabs_ResourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceData>(true);
			__Game_Prefabs_PlaceableInfoviewItem_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PlaceableInfoviewItem>(true);
			__Game_Prefabs_InfoviewMode_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<InfoviewMode>(true);
			__Game_Prefabs_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubArea>(true);
			__Game_Prefabs_PlaceableInfoviewItem_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PlaceableInfoviewItem>(false);
		}
	}

	private EntityQuery m_NewInfoviewQuery;

	private EntityQuery m_AllInfoviewQuery;

	private EntityQuery m_AllInfomodeQuery;

	private EntityQuery m_NewPlaceableQuery;

	private EntityQuery m_AllPlaceableQuery;

	private PrefabSystem m_PrefabSystem;

	private ResourceSystem m_ResourceSystem;

	private const int TYPE_PRIORITY = 1000000;

	private const int PRIMARY_REQUIREMENT_PRIORITY = 10000;

	private const int SECONDARY_REQUIREMENT_PRIORITY = 10000;

	private const int PRIMARY_EFFECT_PRIORITY = 10000;

	private const int SECONDARY_EFFECT_PRIORITY = 100;

	private TypeHandle __TypeHandle;

	public IEnumerable<InfoviewPrefab> infoviews
	{
		get
		{
			ComponentTypeHandle<PrefabData> prefabType = InternalCompilerInterface.GetComponentTypeHandle<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			NativeArray<ArchetypeChunk> chunks = ((EntityQuery)(ref m_AllInfoviewQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
			try
			{
				int i = 0;
				while (i < chunks.Length)
				{
					ArchetypeChunk val = chunks[i];
					NativeArray<PrefabData> prefabs = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabData>(ref prefabType);
					int num;
					for (int j = 0; j < prefabs.Length; j = num)
					{
						yield return m_PrefabSystem.GetPrefab<InfoviewPrefab>(prefabs[j]);
						num = j + 1;
					}
					num = i + 1;
					i = num;
				}
			}
			finally
			{
				chunks.Dispose();
			}
		}
	}

	public IEnumerable<InfomodePrefab> infomodes
	{
		get
		{
			ComponentTypeHandle<PrefabData> prefabType = InternalCompilerInterface.GetComponentTypeHandle<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			NativeArray<ArchetypeChunk> chunks = ((EntityQuery)(ref m_AllInfomodeQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
			try
			{
				int i = 0;
				while (i < chunks.Length)
				{
					ArchetypeChunk val = chunks[i];
					NativeArray<PrefabData> prefabs = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabData>(ref prefabType);
					int num;
					for (int j = 0; j < prefabs.Length; j = num)
					{
						yield return m_PrefabSystem.GetPrefab<InfomodePrefab>(prefabs[j]);
						num = j + 1;
					}
					num = i + 1;
					i = num;
				}
			}
			finally
			{
				chunks.Dispose();
			}
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		m_NewInfoviewQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<InfoviewData>(),
			ComponentType.ReadOnly<Created>()
		});
		m_AllInfoviewQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<InfoviewData>()
		});
		m_AllInfomodeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<InfomodeData>(),
			ComponentType.Exclude<InfomodeGroup>()
		});
		m_NewPlaceableQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadWrite<PlaceableInfoviewItem>(),
			ComponentType.ReadOnly<Created>()
		});
		m_AllPlaceableQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadWrite<PlaceableInfoviewItem>()
		});
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_NewInfoviewQuery)).IsEmptyIgnoreFilter)
		{
			((SystemBase)this).Dependency = InitializeInfoviews(((SystemBase)this).Dependency, m_NewInfoviewQuery, m_AllInfomodeQuery);
			((SystemBase)this).Dependency = FindInfoviews(((SystemBase)this).Dependency, m_AllInfoviewQuery, m_AllInfomodeQuery, m_AllPlaceableQuery);
		}
		else if (!((EntityQuery)(ref m_NewPlaceableQuery)).IsEmptyIgnoreFilter)
		{
			((SystemBase)this).Dependency = FindInfoviews(((SystemBase)this).Dependency, m_AllInfoviewQuery, m_AllInfomodeQuery, m_NewPlaceableQuery);
		}
	}

	private JobHandle InitializeInfoviews(JobHandle inputDeps, EntityQuery infoviewGroup, EntityQuery infomodeGroup)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref infoviewGroup)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<ArchetypeChunk> val2 = ((EntityQuery)(ref infomodeGroup)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		NativeParallelMultiHashMap<Entity, InfoModeData> val3 = default(NativeParallelMultiHashMap<Entity, InfoModeData>);
		val3._002Ector(100, AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			EntityTypeHandle entityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<PrefabData> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			BufferTypeHandle<InfoviewMode> bufferTypeHandle = InternalCompilerInterface.GetBufferTypeHandle<InfoviewMode>(ref __TypeHandle.__Game_Prefabs_InfoviewMode_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<InfomodeGroup> componentLookup = InternalCompilerInterface.GetComponentLookup<InfomodeGroup>(ref __TypeHandle.__Game_Prefabs_InfomodeGroup_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			((JobHandle)(ref inputDeps)).Complete();
			for (int i = 0; i < val2.Length; i++)
			{
				ArchetypeChunk val4 = val2[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val4)).GetNativeArray(entityTypeHandle);
				NativeArray<PrefabData> nativeArray2 = ((ArchetypeChunk)(ref val4)).GetNativeArray<PrefabData>(ref componentTypeHandle);
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					Entity mode = nativeArray[j];
					PrefabData prefabData = nativeArray2[j];
					InfomodeBasePrefab prefab = m_PrefabSystem.GetPrefab<InfomodeBasePrefab>(prefabData);
					if (prefab.m_IncludeInGroups != null)
					{
						for (int k = 0; k < prefab.m_IncludeInGroups.Length; k++)
						{
							Entity entity = m_PrefabSystem.GetEntity(prefab.m_IncludeInGroups[k]);
							val3.Add(entity, new InfoModeData
							{
								m_Mode = mode,
								m_Priority = prefab.m_Priority
							});
						}
					}
				}
			}
			InfoModeData infoModeData = default(InfoModeData);
			NativeParallelMultiHashMapIterator<Entity> val7 = default(NativeParallelMultiHashMapIterator<Entity>);
			for (int l = 0; l < val.Length; l++)
			{
				ArchetypeChunk val5 = val[l];
				NativeArray<PrefabData> nativeArray3 = ((ArchetypeChunk)(ref val5)).GetNativeArray<PrefabData>(ref componentTypeHandle);
				BufferAccessor<InfoviewMode> bufferAccessor = ((ArchetypeChunk)(ref val5)).GetBufferAccessor<InfoviewMode>(ref bufferTypeHandle);
				for (int m = 0; m < bufferAccessor.Length; m++)
				{
					PrefabData prefabData2 = nativeArray3[m];
					DynamicBuffer<InfoviewMode> val6 = bufferAccessor[m];
					InfoviewPrefab prefab2 = m_PrefabSystem.GetPrefab<InfoviewPrefab>(prefabData2);
					if (prefab2.m_Infomodes == null)
					{
						continue;
					}
					for (int n = 0; n < prefab2.m_Infomodes.Length; n++)
					{
						InfomodeInfo infomodeInfo = prefab2.m_Infomodes[n];
						Entity entity2 = m_PrefabSystem.GetEntity(infomodeInfo.m_Mode);
						if (componentLookup.HasComponent(entity2))
						{
							if (val3.TryGetFirstValue(entity2, ref infoModeData, ref val7))
							{
								do
								{
									int priority = infomodeInfo.m_Priority * 1000000 + infomodeInfo.m_Mode.m_Priority * 1000 + infoModeData.m_Priority;
									val6.Add(new InfoviewMode(infoModeData.m_Mode, priority, infomodeInfo.m_Supplemental, infomodeInfo.m_Optional));
								}
								while (val3.TryGetNextValue(ref infoModeData, ref val7));
							}
						}
						else
						{
							int priority2 = infomodeInfo.m_Priority * 1000000 + infomodeInfo.m_Mode.m_Priority;
							val6.Add(new InfoviewMode(entity2, priority2, infomodeInfo.m_Supplemental, infomodeInfo.m_Optional));
						}
					}
				}
			}
		}
		finally
		{
			val.Dispose();
			val2.Dispose();
			val3.Dispose();
		}
		return default(JobHandle);
	}

	private JobHandle FindInfoviews(JobHandle inputDeps, EntityQuery infoviewQuery, EntityQuery infomodeQuery, EntityQuery objectQuery)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0502: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		//IL_051f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0537: Unknown result type (might be due to invalid IL or missing references)
		//IL_053c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0554: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_0571: Unknown result type (might be due to invalid IL or missing references)
		//IL_0576: Unknown result type (might be due to invalid IL or missing references)
		//IL_058e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0593: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0602: Unknown result type (might be due to invalid IL or missing references)
		//IL_0607: Unknown result type (might be due to invalid IL or missing references)
		//IL_061f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0624: Unknown result type (might be due to invalid IL or missing references)
		//IL_063c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0641: Unknown result type (might be due to invalid IL or missing references)
		//IL_0659: Unknown result type (might be due to invalid IL or missing references)
		//IL_065e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0676: Unknown result type (might be due to invalid IL or missing references)
		//IL_067b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0693: Unknown result type (might be due to invalid IL or missing references)
		//IL_0698: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06de: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0713: Unknown result type (might be due to invalid IL or missing references)
		//IL_0718: Unknown result type (might be due to invalid IL or missing references)
		//IL_0730: Unknown result type (might be due to invalid IL or missing references)
		//IL_0735: Unknown result type (might be due to invalid IL or missing references)
		//IL_074d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0752: Unknown result type (might be due to invalid IL or missing references)
		//IL_076a: Unknown result type (might be due to invalid IL or missing references)
		//IL_076f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0787: Unknown result type (might be due to invalid IL or missing references)
		//IL_078c: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07de: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0800: Unknown result type (might be due to invalid IL or missing references)
		//IL_081b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0820: Unknown result type (might be due to invalid IL or missing references)
		//IL_0833: Unknown result type (might be due to invalid IL or missing references)
		//IL_0834: Unknown result type (might be due to invalid IL or missing references)
		//IL_084c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0851: Unknown result type (might be due to invalid IL or missing references)
		//IL_085a: Unknown result type (might be due to invalid IL or missing references)
		//IL_085c: Unknown result type (might be due to invalid IL or missing references)
		//IL_085d: Unknown result type (might be due to invalid IL or missing references)
		//IL_085e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0860: Unknown result type (might be due to invalid IL or missing references)
		//IL_0865: Unknown result type (might be due to invalid IL or missing references)
		//IL_086a: Unknown result type (might be due to invalid IL or missing references)
		//IL_086e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0870: Unknown result type (might be due to invalid IL or missing references)
		//IL_0872: Unknown result type (might be due to invalid IL or missing references)
		//IL_0877: Unknown result type (might be due to invalid IL or missing references)
		//IL_0879: Unknown result type (might be due to invalid IL or missing references)
		//IL_087b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0880: Unknown result type (might be due to invalid IL or missing references)
		//IL_0884: Unknown result type (might be due to invalid IL or missing references)
		//IL_0886: Unknown result type (might be due to invalid IL or missing references)
		//IL_088e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0890: Unknown result type (might be due to invalid IL or missing references)
		//IL_0898: Unknown result type (might be due to invalid IL or missing references)
		//IL_089a: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a0: Unknown result type (might be due to invalid IL or missing references)
		NativeQueue<InfoviewBufferData> infoViewBuffer = default(NativeQueue<InfoviewBufferData>);
		infoViewBuffer._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		JobHandle val = default(JobHandle);
		NativeList<ArchetypeChunk> infoviewChunks = ((EntityQuery)(ref infoviewQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		JobHandle val2 = default(JobHandle);
		NativeList<ArchetypeChunk> infomodeChunks = ((EntityQuery)(ref infomodeQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val2);
		FindInfoviewJob findInfoviewJob = new FindInfoviewJob
		{
			m_InfoviewChunks = infoviewChunks,
			m_InfomodeChunks = infomodeChunks,
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CoverageType = InternalCompilerInterface.GetComponentTypeHandle<CoverageData>(ref __TypeHandle.__Game_Prefabs_CoverageData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HospitalType = InternalCompilerInterface.GetComponentTypeHandle<HospitalData>(ref __TypeHandle.__Game_Prefabs_HospitalData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PowerPlantType = InternalCompilerInterface.GetComponentTypeHandle<PowerPlantData>(ref __TypeHandle.__Game_Prefabs_PowerPlantData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformerType = InternalCompilerInterface.GetComponentTypeHandle<TransformerData>(ref __TypeHandle.__Game_Prefabs_TransformerData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BatteryType = InternalCompilerInterface.GetComponentTypeHandle<BatteryData>(ref __TypeHandle.__Game_Prefabs_BatteryData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WaterPumpingStationType = InternalCompilerInterface.GetComponentTypeHandle<WaterPumpingStationData>(ref __TypeHandle.__Game_Prefabs_WaterPumpingStationData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WaterTowerType = InternalCompilerInterface.GetComponentTypeHandle<WaterTowerData>(ref __TypeHandle.__Game_Prefabs_WaterTowerData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SewageOutletType = InternalCompilerInterface.GetComponentTypeHandle<SewageOutletData>(ref __TypeHandle.__Game_Prefabs_SewageOutletData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransportDepotType = InternalCompilerInterface.GetComponentTypeHandle<TransportDepotData>(ref __TypeHandle.__Game_Prefabs_TransportDepotData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransportStationType = InternalCompilerInterface.GetComponentTypeHandle<TransportStationData>(ref __TypeHandle.__Game_Prefabs_TransportStationData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageFacilityType = InternalCompilerInterface.GetComponentTypeHandle<GarbageFacilityData>(ref __TypeHandle.__Game_Prefabs_GarbageFacilityData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_FireStationType = InternalCompilerInterface.GetComponentTypeHandle<FireStationData>(ref __TypeHandle.__Game_Prefabs_FireStationData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PoliceStationType = InternalCompilerInterface.GetComponentTypeHandle<PoliceStationData>(ref __TypeHandle.__Game_Prefabs_PoliceStationData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MaintenanceDepotType = InternalCompilerInterface.GetComponentTypeHandle<MaintenanceDepotData>(ref __TypeHandle.__Game_Prefabs_MaintenanceDepotData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PostFacilityDataType = InternalCompilerInterface.GetComponentTypeHandle<PostFacilityData>(ref __TypeHandle.__Game_Prefabs_PostFacilityData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TelecomFacilityDataType = InternalCompilerInterface.GetComponentTypeHandle<TelecomFacilityData>(ref __TypeHandle.__Game_Prefabs_TelecomFacilityData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SchoolDataType = InternalCompilerInterface.GetComponentTypeHandle<SchoolData>(ref __TypeHandle.__Game_Prefabs_SchoolData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ParkDataType = InternalCompilerInterface.GetComponentTypeHandle<ParkData>(ref __TypeHandle.__Game_Prefabs_ParkData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EmergencyShelterDataType = InternalCompilerInterface.GetComponentTypeHandle<EmergencyShelterData>(ref __TypeHandle.__Game_Prefabs_EmergencyShelterData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DisasterFacilityDataType = InternalCompilerInterface.GetComponentTypeHandle<DisasterFacilityData>(ref __TypeHandle.__Game_Prefabs_DisasterFacilityData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_FirewatchTowerDataType = InternalCompilerInterface.GetComponentTypeHandle<FirewatchTowerData>(ref __TypeHandle.__Game_Prefabs_FirewatchTowerData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DeathcareFacilityDataType = InternalCompilerInterface.GetComponentTypeHandle<DeathcareFacilityData>(ref __TypeHandle.__Game_Prefabs_DeathcareFacilityData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrisonDataType = InternalCompilerInterface.GetComponentTypeHandle<PrisonData>(ref __TypeHandle.__Game_Prefabs_PrisonData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AdminBuildingDataType = InternalCompilerInterface.GetComponentTypeHandle<AdminBuildingData>(ref __TypeHandle.__Game_Prefabs_AdminBuildingData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WelfareOfficeDataType = InternalCompilerInterface.GetComponentTypeHandle<WelfareOfficeData>(ref __TypeHandle.__Game_Prefabs_WelfareOfficeData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResearchFacilityDataType = InternalCompilerInterface.GetComponentTypeHandle<ResearchFacilityData>(ref __TypeHandle.__Game_Prefabs_ResearchFacilityData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ParkingFacilityDataType = InternalCompilerInterface.GetComponentTypeHandle<ParkingFacilityData>(ref __TypeHandle.__Game_Prefabs_ParkingFacilityData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PowerLineType = InternalCompilerInterface.GetComponentTypeHandle<PowerLineData>(ref __TypeHandle.__Game_Prefabs_PowerLineData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PipelineType = InternalCompilerInterface.GetComponentTypeHandle<PipelineData>(ref __TypeHandle.__Game_Prefabs_PipelineData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ElectricityConnectionType = InternalCompilerInterface.GetComponentTypeHandle<ElectricityConnectionData>(ref __TypeHandle.__Game_Prefabs_ElectricityConnectionData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WaterPipeConnectionType = InternalCompilerInterface.GetComponentTypeHandle<WaterPipeConnectionData>(ref __TypeHandle.__Game_Prefabs_WaterPipeConnectionData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceConnectionType = InternalCompilerInterface.GetComponentTypeHandle<ResourceConnectionData>(ref __TypeHandle.__Game_Prefabs_ResourceConnectionData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ZoneType = InternalCompilerInterface.GetComponentTypeHandle<ZoneData>(ref __TypeHandle.__Game_Prefabs_ZoneData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransportStopType = InternalCompilerInterface.GetComponentTypeHandle<TransportStopData>(ref __TypeHandle.__Game_Prefabs_TransportStopData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RouteType = InternalCompilerInterface.GetComponentTypeHandle<RouteData>(ref __TypeHandle.__Game_Prefabs_RouteData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransportLineType = InternalCompilerInterface.GetComponentTypeHandle<TransportLineData>(ref __TypeHandle.__Game_Prefabs_TransportLineData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ExtractorAreaType = InternalCompilerInterface.GetComponentTypeHandle<ExtractorAreaData>(ref __TypeHandle.__Game_Prefabs_ExtractorAreaData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TerraformingType = InternalCompilerInterface.GetComponentTypeHandle<TerraformingData>(ref __TypeHandle.__Game_Prefabs_TerraformingData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WindPoweredType = InternalCompilerInterface.GetComponentTypeHandle<WindPoweredData>(ref __TypeHandle.__Game_Prefabs_WindPoweredData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WaterPoweredType = InternalCompilerInterface.GetComponentTypeHandle<WaterPoweredData>(ref __TypeHandle.__Game_Prefabs_WaterPoweredData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_GroundWaterPoweredType = InternalCompilerInterface.GetComponentTypeHandle<GroundWaterPoweredData>(ref __TypeHandle.__Game_Prefabs_GroundWaterPoweredData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PollutionType = InternalCompilerInterface.GetComponentTypeHandle<PollutionData>(ref __TypeHandle.__Game_Prefabs_PollutionData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnableBuildingType = InternalCompilerInterface.GetComponentTypeHandle<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingPropertyType = InternalCompilerInterface.GetComponentTypeHandle<BuildingPropertyData>(ref __TypeHandle.__Game_Prefabs_BuildingPropertyData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceUpgradeType = InternalCompilerInterface.GetComponentTypeHandle<ServiceUpgradeData>(ref __TypeHandle.__Game_Prefabs_ServiceUpgradeData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InfoviewModeType = InternalCompilerInterface.GetBufferTypeHandle<InfoviewMode>(ref __TypeHandle.__Game_Prefabs_InfoviewMode_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InfoviewCoverageType = InternalCompilerInterface.GetComponentTypeHandle<InfoviewCoverageData>(ref __TypeHandle.__Game_Prefabs_InfoviewCoverageData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InfoviewAvailabilityType = InternalCompilerInterface.GetComponentTypeHandle<InfoviewAvailabilityData>(ref __TypeHandle.__Game_Prefabs_InfoviewAvailabilityData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InfoviewBuildingType = InternalCompilerInterface.GetComponentTypeHandle<InfoviewBuildingData>(ref __TypeHandle.__Game_Prefabs_InfoviewBuildingData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InfoviewVehicleType = InternalCompilerInterface.GetComponentTypeHandle<InfoviewVehicleData>(ref __TypeHandle.__Game_Prefabs_InfoviewVehicleData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InfoviewTransportStopType = InternalCompilerInterface.GetComponentTypeHandle<InfoviewTransportStopData>(ref __TypeHandle.__Game_Prefabs_InfoviewTransportStopData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InfoviewRouteType = InternalCompilerInterface.GetComponentTypeHandle<InfoviewRouteData>(ref __TypeHandle.__Game_Prefabs_InfoviewRouteData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InfoviewHeatmapType = InternalCompilerInterface.GetComponentTypeHandle<InfoviewHeatmapData>(ref __TypeHandle.__Game_Prefabs_InfoviewHeatmapData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InfoviewObjectStatusType = InternalCompilerInterface.GetComponentTypeHandle<InfoviewObjectStatusData>(ref __TypeHandle.__Game_Prefabs_InfoviewObjectStatusData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InfoviewNetStatusType = InternalCompilerInterface.GetComponentTypeHandle<InfoviewNetStatusData>(ref __TypeHandle.__Game_Prefabs_InfoviewNetStatusData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PlaceableInfoviewType = InternalCompilerInterface.GetBufferTypeHandle<PlaceableInfoviewItem>(ref __TypeHandle.__Game_Prefabs_PlaceableInfoviewItem_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef)
		};
		FindSubInfoviewJob findSubInfoviewJob = new FindSubInfoviewJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnableBuildingType = InternalCompilerInterface.GetComponentTypeHandle<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingPropertyType = InternalCompilerInterface.GetComponentTypeHandle<BuildingPropertyData>(ref __TypeHandle.__Game_Prefabs_BuildingPropertyData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PlaceableInfoviewType = InternalCompilerInterface.GetBufferTypeHandle<PlaceableInfoviewItem>(ref __TypeHandle.__Game_Prefabs_PlaceableInfoviewItem_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubAreaType = InternalCompilerInterface.GetBufferTypeHandle<SubArea>(ref __TypeHandle.__Game_Prefabs_SubArea_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingUpgradeElementType = InternalCompilerInterface.GetBufferTypeHandle<BuildingUpgradeElement>(ref __TypeHandle.__Game_Prefabs_BuildingUpgradeElement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LotData = InternalCompilerInterface.GetComponentLookup<LotData>(ref __TypeHandle.__Game_Prefabs_LotData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceData = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PlaceableInfoviewData = InternalCompilerInterface.GetBufferLookup<PlaceableInfoviewItem>(ref __TypeHandle.__Game_Prefabs_PlaceableInfoviewItem_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InfoviewModes = InternalCompilerInterface.GetBufferLookup<InfoviewMode>(ref __TypeHandle.__Game_Prefabs_InfoviewMode_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubAreas = InternalCompilerInterface.GetBufferLookup<SubArea>(ref __TypeHandle.__Game_Prefabs_SubArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourcePrefabs = m_ResourceSystem.GetPrefabs(),
			m_InfoViewBuffer = infoViewBuffer.AsParallelWriter()
		};
		AssignInfoviewJob obj = new AssignInfoviewJob
		{
			m_InfoViewBuffer = infoViewBuffer,
			m_PlaceableInfoviewData = InternalCompilerInterface.GetBufferLookup<PlaceableInfoviewItem>(ref __TypeHandle.__Game_Prefabs_PlaceableInfoviewItem_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		JobHandle val3 = JobChunkExtensions.ScheduleParallel<FindInfoviewJob>(findInfoviewJob, objectQuery, JobHandle.CombineDependencies(inputDeps, val, val2));
		JobHandle val4 = JobChunkExtensions.ScheduleParallel<FindSubInfoviewJob>(findSubInfoviewJob, objectQuery, val3);
		JobHandle val5 = IJobExtensions.Schedule<AssignInfoviewJob>(obj, val4);
		infoViewBuffer.Dispose(val5);
		infoviewChunks.Dispose(val3);
		infomodeChunks.Dispose(val3);
		return val5;
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
	public InfoviewInitializeSystem()
	{
	}
}
