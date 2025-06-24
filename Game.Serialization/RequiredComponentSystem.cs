using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game.Agents;
using Game.Areas;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Creatures;
using Game.Economy;
using Game.Effects;
using Game.Events;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
using Game.Policies;
using Game.Prefabs;
using Game.Rendering;
using Game.Routes;
using Game.Simulation;
using Game.Tools;
using Game.Triggers;
using Game.Vehicles;
using Game.Zones;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Serialization;

[CompilerGenerated]
public class RequiredComponentSystem : GameSystemBase
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct TypeHandle
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
		}
	}

	private LoadGameSystem m_LoadGameSystem;

	private EntityQuery m_BlockedLaneQuery;

	private EntityQuery m_CarLaneQuery;

	private EntityQuery m_BuildingEfficiencyQuery;

	private EntityQuery m_PolicyQuery;

	private EntityQuery m_CityModifierQuery;

	private EntityQuery m_ServiceDispatchQuery;

	private EntityQuery m_PathInformationQuery;

	private EntityQuery m_NodeGeometryQuery;

	private EntityQuery m_MeshColorQuery;

	private EntityQuery m_MeshBatchQuery;

	private EntityQuery m_RoutePolicyQuery;

	private EntityQuery m_RouteModifierQuery;

	private EntityQuery m_EdgeQuery;

	private EntityQuery m_StorageTaxQuery;

	private EntityQuery m_CityFeeQuery;

	private EntityQuery m_CityFeeQuery2;

	private EntityQuery m_ServiceFeeParameterQuery;

	private EntityQuery m_OutsideGarbageQuery;

	private EntityQuery m_OutsideFireStationQuery;

	private EntityQuery m_OutsidePoliceStationQuery;

	private EntityQuery m_OutsideEfficiencyQuery;

	private EntityQuery m_RouteInfoQuery;

	private EntityQuery m_CompanyProfitabilityQuery;

	private EntityQuery m_StorageQuery;

	private EntityQuery m_RouteBufferIndexQuery;

	private EntityQuery m_CurveElementQuery;

	private EntityQuery m_CitizenPrefabQuery;

	private EntityQuery m_CitizenNameQuery;

	private EntityQuery m_HouseholdNameQuery;

	private EntityQuery m_LabelVertexQuery;

	private EntityQuery m_DistrictNameQuery;

	private EntityQuery m_AnimalNameQuery;

	private EntityQuery m_HouseholdPetQuery;

	private EntityQuery m_RoadNameQuery;

	private EntityQuery m_RouteNumberQuery;

	private EntityQuery m_ChirpRandomLocQuery;

	private EntityQuery m_BlockerQuery;

	private EntityQuery m_CitizenPresenceQuery;

	private EntityQuery m_SubLaneQuery;

	private EntityQuery m_SubObjectQuery;

	private EntityQuery m_NativeQuery;

	private EntityQuery m_GuestVehicleQuery;

	private EntityQuery m_TravelPurposeQuery;

	private EntityQuery m_TreeEffectQuery;

	private EntityQuery m_TakeoffLocationQuery;

	private EntityQuery m_LeisureQuery;

	private EntityQuery m_PlayerMoneyQuery;

	private EntityQuery m_PseudoRandomSeedQuery;

	private EntityQuery m_TransportDepotQuery;

	private EntityQuery m_ServiceUsageQuery;

	private EntityQuery m_OutsideSellerQuery;

	private EntityQuery m_LoadingResourcesQuery;

	private EntityQuery m_CompanyVehicleQuery;

	private EntityQuery m_LaneRestrictionQuery;

	private EntityQuery m_LaneOverlapQuery;

	private EntityQuery m_DispatchedRequestQuery;

	private EntityQuery m_HomelessShelterQuery;

	private EntityQuery m_QueueQuery;

	private EntityQuery m_BoneHistoryQuery;

	private EntityQuery m_UnspawnedQuery;

	private EntityQuery m_ConnectionLaneQuery;

	private EntityQuery m_AreaLaneQuery;

	private EntityQuery m_OfficeQuery;

	private EntityQuery m_VehicleModelQuery;

	private EntityQuery m_PassengerTransportQuery;

	private EntityQuery m_ObjectColorQuery;

	private EntityQuery m_OutsideConnectionQuery;

	private EntityQuery m_NetConditionQuery;

	private EntityQuery m_NetPollutionQuery;

	private EntityQuery m_TrafficSpawnerQuery;

	private EntityQuery m_AreaExpandQuery;

	private EntityQuery m_EmissiveQuery;

	private EntityQuery m_TrainBogieFrameQuery;

	private EntityQuery m_EditorContainerQuery;

	private EntityQuery m_ProcessingTradeCostQuery;

	private EntityQuery m_StorageConditionQuery;

	private EntityQuery m_LaneColorQuery;

	private EntityQuery m_CompanyNotificationQuery;

	private EntityQuery m_PlantQuery;

	private EntityQuery m_CityPopulationQuery;

	private EntityQuery m_CityTourismQuery;

	private EntityQuery m_LaneElevationQuery;

	private EntityQuery m_BuildingNotificationQuery;

	private EntityQuery m_AreaElevationQuery;

	private EntityQuery m_BuildingLotQuery;

	private EntityQuery m_AreaTerrainQuery;

	private EntityQuery m_OwnedVehicleQuery;

	private EntityQuery m_EdgeMappingQuery;

	private EntityQuery m_SubFlowQuery;

	private EntityQuery m_PointOfInterestQuery;

	private EntityQuery m_BuildableAreaQuery;

	private EntityQuery m_SubAreaQuery;

	private EntityQuery m_CrimeVictimQuery;

	private EntityQuery m_ArrivedQuery;

	private EntityQuery m_MailSenderQuery;

	private EntityQuery m_CarKeeperQuery;

	private EntityQuery m_NeedAddHasJobSeekerQuery;

	private EntityQuery m_NeedAddPropertySeekerQuery;

	private EntityQuery m_AgeGroupQuery;

	private EntityQuery m_PrefabRefQuery;

	private EntityQuery m_LabelMaterialQuery;

	private EntityQuery m_ArrowMaterialQuery;

	private EntityQuery m_LockedQuery;

	private EntityQuery m_OutsideUpdateQuery;

	private EntityQuery m_WaitingPassengersQuery;

	private EntityQuery m_ObjectSurfaceQuery;

	private EntityQuery m_WaitingPassengersQuery2;

	private EntityQuery m_PillarQuery;

	private EntityQuery m_LegacyEfficiencyQuery;

	private EntityQuery m_SignatureQuery;

	private EntityQuery m_SubObjectOwnerQuery;

	private EntityQuery m_DangerLevelMissingQuery;

	private EntityQuery m_MeshGroupQuery;

	private EntityQuery m_UpdateFrameQuery;

	private EntityQuery m_FenceQuery;

	private EntityQuery m_NetGeometrySectionQuery;

	private EntityQuery m_NetLaneArchetypeDataQuery;

	private EntityQuery m_PathfindUpdatedQuery;

	private EntityQuery m_RouteColorQuery;

	private EntityQuery m_CitizenQuery;

	private EntityQuery m_ServiceUpkeepQuery;

	private EntityQuery m_MoveableBridgeQuery;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_1938549531_0;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Expected O, but got Unknown
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Expected O, but got Unknown
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Expected O, but got Unknown
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Expected O, but got Unknown
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0505: Unknown result type (might be due to invalid IL or missing references)
		//IL_050a: Unknown result type (might be due to invalid IL or missing references)
		//IL_050f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_0528: Unknown result type (might be due to invalid IL or missing references)
		//IL_052f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_055e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_0577: Unknown result type (might be due to invalid IL or missing references)
		//IL_057c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0583: Unknown result type (might be due to invalid IL or missing references)
		//IL_0588: Unknown result type (might be due to invalid IL or missing references)
		//IL_058d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0592: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0601: Unknown result type (might be due to invalid IL or missing references)
		//IL_0606: Unknown result type (might be due to invalid IL or missing references)
		//IL_060d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0612: Unknown result type (might be due to invalid IL or missing references)
		//IL_0617: Unknown result type (might be due to invalid IL or missing references)
		//IL_061c: Unknown result type (might be due to invalid IL or missing references)
		//IL_062b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0630: Unknown result type (might be due to invalid IL or missing references)
		//IL_0637: Unknown result type (might be due to invalid IL or missing references)
		//IL_063c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0643: Unknown result type (might be due to invalid IL or missing references)
		//IL_0648: Unknown result type (might be due to invalid IL or missing references)
		//IL_064d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0652: Unknown result type (might be due to invalid IL or missing references)
		//IL_0661: Unknown result type (might be due to invalid IL or missing references)
		//IL_0666: Unknown result type (might be due to invalid IL or missing references)
		//IL_066d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0672: Unknown result type (might be due to invalid IL or missing references)
		//IL_0677: Unknown result type (might be due to invalid IL or missing references)
		//IL_067c: Unknown result type (might be due to invalid IL or missing references)
		//IL_068b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0690: Unknown result type (might be due to invalid IL or missing references)
		//IL_0697: Unknown result type (might be due to invalid IL or missing references)
		//IL_069c: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0701: Unknown result type (might be due to invalid IL or missing references)
		//IL_0706: Unknown result type (might be due to invalid IL or missing references)
		//IL_0715: Unknown result type (might be due to invalid IL or missing references)
		//IL_071a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0721: Unknown result type (might be due to invalid IL or missing references)
		//IL_0726: Unknown result type (might be due to invalid IL or missing references)
		//IL_072b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0730: Unknown result type (might be due to invalid IL or missing references)
		//IL_073f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0745: Expected O, but got Unknown
		//IL_074e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0753: Unknown result type (might be due to invalid IL or missing references)
		//IL_075a: Unknown result type (might be due to invalid IL or missing references)
		//IL_075f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0772: Unknown result type (might be due to invalid IL or missing references)
		//IL_0777: Unknown result type (might be due to invalid IL or missing references)
		//IL_0783: Unknown result type (might be due to invalid IL or missing references)
		//IL_0788: Unknown result type (might be due to invalid IL or missing references)
		//IL_0797: Unknown result type (might be due to invalid IL or missing references)
		//IL_079d: Expected O, but got Unknown
		//IL_07a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_07db: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0800: Unknown result type (might be due to invalid IL or missing references)
		//IL_0805: Unknown result type (might be due to invalid IL or missing references)
		//IL_080a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0819: Unknown result type (might be due to invalid IL or missing references)
		//IL_081e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0825: Unknown result type (might be due to invalid IL or missing references)
		//IL_082a: Unknown result type (might be due to invalid IL or missing references)
		//IL_082f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0834: Unknown result type (might be due to invalid IL or missing references)
		//IL_0843: Unknown result type (might be due to invalid IL or missing references)
		//IL_0848: Unknown result type (might be due to invalid IL or missing references)
		//IL_084f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0854: Unknown result type (might be due to invalid IL or missing references)
		//IL_0859: Unknown result type (might be due to invalid IL or missing references)
		//IL_085e: Unknown result type (might be due to invalid IL or missing references)
		//IL_086d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0872: Unknown result type (might be due to invalid IL or missing references)
		//IL_0879: Unknown result type (might be due to invalid IL or missing references)
		//IL_087e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0885: Unknown result type (might be due to invalid IL or missing references)
		//IL_088a: Unknown result type (might be due to invalid IL or missing references)
		//IL_088f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0894: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a9: Expected O, but got Unknown
		//IL_08b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08be: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08db: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0904: Unknown result type (might be due to invalid IL or missing references)
		//IL_0913: Unknown result type (might be due to invalid IL or missing references)
		//IL_0918: Unknown result type (might be due to invalid IL or missing references)
		//IL_091d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0922: Unknown result type (might be due to invalid IL or missing references)
		//IL_0931: Unknown result type (might be due to invalid IL or missing references)
		//IL_0936: Unknown result type (might be due to invalid IL or missing references)
		//IL_093d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0942: Unknown result type (might be due to invalid IL or missing references)
		//IL_0947: Unknown result type (might be due to invalid IL or missing references)
		//IL_094c: Unknown result type (might be due to invalid IL or missing references)
		//IL_095b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0960: Unknown result type (might be due to invalid IL or missing references)
		//IL_0967: Unknown result type (might be due to invalid IL or missing references)
		//IL_096c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0973: Unknown result type (might be due to invalid IL or missing references)
		//IL_0978: Unknown result type (might be due to invalid IL or missing references)
		//IL_097d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0982: Unknown result type (might be due to invalid IL or missing references)
		//IL_0991: Unknown result type (might be due to invalid IL or missing references)
		//IL_0996: Unknown result type (might be due to invalid IL or missing references)
		//IL_099d: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_09cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09df: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_09fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a03: Expected O, but got Unknown
		//IL_0a0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a11: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a18: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a24: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a29: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a35: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a41: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a48: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a71: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a76: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a85: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a91: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a96: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0abb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0acc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0afb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b00: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b14: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b20: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b25: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b39: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b54: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b63: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b69: Expected O, but got Unknown
		//IL_0b72: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b77: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b96: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bbf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bcb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bfc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c03: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c08: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c12: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c21: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c27: Expected O, but got Unknown
		//IL_0c30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c35: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c41: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c54: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c59: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c79: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c7f: Expected O, but got Unknown
		//IL_0c88: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c8d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c94: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c99: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cb1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cbd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cc2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cdd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cfb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d00: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d07: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d11: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d16: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d25: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d31: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d36: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d40: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d54: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d79: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d85: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d94: Unknown result type (might be due to invalid IL or missing references)
		//IL_0da3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0da8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0daf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dbe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dcd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ddc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0deb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0df0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0df7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dfc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e03: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e08: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e14: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e19: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e33: Expected O, but got Unknown
		//IL_0e3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e41: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e48: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e54: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e59: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e71: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e84: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e89: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e95: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ea9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eaf: Expected O, but got Unknown
		//IL_0eb8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ebd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0edc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ee1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ef2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f06: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f12: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f17: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f37: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f41: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f46: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f55: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f61: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f66: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f72: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f77: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f90: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f97: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fa1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fa6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fb5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fbb: Expected O, but got Unknown
		//IL_0fc4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fc9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fe8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ff9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ffe: Unknown result type (might be due to invalid IL or missing references)
		//IL_100d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1012: Unknown result type (might be due to invalid IL or missing references)
		//IL_1019: Unknown result type (might be due to invalid IL or missing references)
		//IL_101e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1023: Unknown result type (might be due to invalid IL or missing references)
		//IL_1028: Unknown result type (might be due to invalid IL or missing references)
		//IL_1037: Unknown result type (might be due to invalid IL or missing references)
		//IL_103c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1043: Unknown result type (might be due to invalid IL or missing references)
		//IL_1048: Unknown result type (might be due to invalid IL or missing references)
		//IL_104d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1052: Unknown result type (might be due to invalid IL or missing references)
		//IL_1061: Unknown result type (might be due to invalid IL or missing references)
		//IL_1067: Expected O, but got Unknown
		//IL_1070: Unknown result type (might be due to invalid IL or missing references)
		//IL_1075: Unknown result type (might be due to invalid IL or missing references)
		//IL_107c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1081: Unknown result type (might be due to invalid IL or missing references)
		//IL_1088: Unknown result type (might be due to invalid IL or missing references)
		//IL_108d: Unknown result type (might be due to invalid IL or missing references)
		//IL_10a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_10a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_10bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_10c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_10c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_10dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_10e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_10f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1101: Unknown result type (might be due to invalid IL or missing references)
		//IL_1106: Unknown result type (might be due to invalid IL or missing references)
		//IL_110d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1112: Unknown result type (might be due to invalid IL or missing references)
		//IL_1117: Unknown result type (might be due to invalid IL or missing references)
		//IL_111c: Unknown result type (might be due to invalid IL or missing references)
		//IL_112b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1131: Expected O, but got Unknown
		//IL_113a: Unknown result type (might be due to invalid IL or missing references)
		//IL_113f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1146: Unknown result type (might be due to invalid IL or missing references)
		//IL_114b: Unknown result type (might be due to invalid IL or missing references)
		//IL_115e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1163: Unknown result type (might be due to invalid IL or missing references)
		//IL_116f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1174: Unknown result type (might be due to invalid IL or missing references)
		//IL_1183: Unknown result type (might be due to invalid IL or missing references)
		//IL_1188: Unknown result type (might be due to invalid IL or missing references)
		//IL_118f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1194: Unknown result type (might be due to invalid IL or missing references)
		//IL_1199: Unknown result type (might be due to invalid IL or missing references)
		//IL_119e: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_11b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_11b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_11be: Unknown result type (might be due to invalid IL or missing references)
		//IL_11c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_11c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_11d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_11dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_11e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_11e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1201: Unknown result type (might be due to invalid IL or missing references)
		//IL_1206: Unknown result type (might be due to invalid IL or missing references)
		//IL_120d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1212: Unknown result type (might be due to invalid IL or missing references)
		//IL_1217: Unknown result type (might be due to invalid IL or missing references)
		//IL_121c: Unknown result type (might be due to invalid IL or missing references)
		//IL_122b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1230: Unknown result type (might be due to invalid IL or missing references)
		//IL_1237: Unknown result type (might be due to invalid IL or missing references)
		//IL_123c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1241: Unknown result type (might be due to invalid IL or missing references)
		//IL_1246: Unknown result type (might be due to invalid IL or missing references)
		//IL_1255: Unknown result type (might be due to invalid IL or missing references)
		//IL_125a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1261: Unknown result type (might be due to invalid IL or missing references)
		//IL_1266: Unknown result type (might be due to invalid IL or missing references)
		//IL_126d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1272: Unknown result type (might be due to invalid IL or missing references)
		//IL_1279: Unknown result type (might be due to invalid IL or missing references)
		//IL_127e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1285: Unknown result type (might be due to invalid IL or missing references)
		//IL_128a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1291: Unknown result type (might be due to invalid IL or missing references)
		//IL_1296: Unknown result type (might be due to invalid IL or missing references)
		//IL_129b: Unknown result type (might be due to invalid IL or missing references)
		//IL_12a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_12af: Unknown result type (might be due to invalid IL or missing references)
		//IL_12b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_12bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_12c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_12c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_12ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_12de: Unknown result type (might be due to invalid IL or missing references)
		//IL_12e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_12ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_12ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_12f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1303: Unknown result type (might be due to invalid IL or missing references)
		//IL_1308: Unknown result type (might be due to invalid IL or missing references)
		//IL_130f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1314: Unknown result type (might be due to invalid IL or missing references)
		//IL_131b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1320: Unknown result type (might be due to invalid IL or missing references)
		//IL_1325: Unknown result type (might be due to invalid IL or missing references)
		//IL_132a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1339: Unknown result type (might be due to invalid IL or missing references)
		//IL_133e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1345: Unknown result type (might be due to invalid IL or missing references)
		//IL_134a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1351: Unknown result type (might be due to invalid IL or missing references)
		//IL_1356: Unknown result type (might be due to invalid IL or missing references)
		//IL_135b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1360: Unknown result type (might be due to invalid IL or missing references)
		//IL_136f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1374: Unknown result type (might be due to invalid IL or missing references)
		//IL_137b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1380: Unknown result type (might be due to invalid IL or missing references)
		//IL_1385: Unknown result type (might be due to invalid IL or missing references)
		//IL_138a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1399: Unknown result type (might be due to invalid IL or missing references)
		//IL_139e: Unknown result type (might be due to invalid IL or missing references)
		//IL_13a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_13aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_13af: Unknown result type (might be due to invalid IL or missing references)
		//IL_13b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_13c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_13c9: Expected O, but got Unknown
		//IL_13d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_13d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_13de: Unknown result type (might be due to invalid IL or missing references)
		//IL_13e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_13ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_13ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_1402: Unknown result type (might be due to invalid IL or missing references)
		//IL_1407: Unknown result type (might be due to invalid IL or missing references)
		//IL_1413: Unknown result type (might be due to invalid IL or missing references)
		//IL_1418: Unknown result type (might be due to invalid IL or missing references)
		//IL_1427: Unknown result type (might be due to invalid IL or missing references)
		//IL_142c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1433: Unknown result type (might be due to invalid IL or missing references)
		//IL_1438: Unknown result type (might be due to invalid IL or missing references)
		//IL_143d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1442: Unknown result type (might be due to invalid IL or missing references)
		//IL_1451: Unknown result type (might be due to invalid IL or missing references)
		//IL_1456: Unknown result type (might be due to invalid IL or missing references)
		//IL_145d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1462: Unknown result type (might be due to invalid IL or missing references)
		//IL_1467: Unknown result type (might be due to invalid IL or missing references)
		//IL_146c: Unknown result type (might be due to invalid IL or missing references)
		//IL_147b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1480: Unknown result type (might be due to invalid IL or missing references)
		//IL_1487: Unknown result type (might be due to invalid IL or missing references)
		//IL_148c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1491: Unknown result type (might be due to invalid IL or missing references)
		//IL_1496: Unknown result type (might be due to invalid IL or missing references)
		//IL_14a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_14aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_14b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_14b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_14bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_14c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_14cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_14d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_14db: Unknown result type (might be due to invalid IL or missing references)
		//IL_14e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_14e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_14ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_14f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_14fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_1505: Unknown result type (might be due to invalid IL or missing references)
		//IL_150a: Unknown result type (might be due to invalid IL or missing references)
		//IL_150f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1514: Unknown result type (might be due to invalid IL or missing references)
		//IL_1523: Unknown result type (might be due to invalid IL or missing references)
		//IL_1528: Unknown result type (might be due to invalid IL or missing references)
		//IL_152f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1534: Unknown result type (might be due to invalid IL or missing references)
		//IL_1539: Unknown result type (might be due to invalid IL or missing references)
		//IL_153e: Unknown result type (might be due to invalid IL or missing references)
		//IL_154d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1553: Expected O, but got Unknown
		//IL_155c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1561: Unknown result type (might be due to invalid IL or missing references)
		//IL_1568: Unknown result type (might be due to invalid IL or missing references)
		//IL_156d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1580: Unknown result type (might be due to invalid IL or missing references)
		//IL_1585: Unknown result type (might be due to invalid IL or missing references)
		//IL_1591: Unknown result type (might be due to invalid IL or missing references)
		//IL_1596: Unknown result type (might be due to invalid IL or missing references)
		//IL_15a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_15ab: Expected O, but got Unknown
		//IL_15b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_15b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_15cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_15d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_15d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_15dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_15e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_15e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_15f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_15f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1601: Unknown result type (might be due to invalid IL or missing references)
		//IL_1606: Unknown result type (might be due to invalid IL or missing references)
		//IL_1615: Unknown result type (might be due to invalid IL or missing references)
		//IL_161a: Unknown result type (might be due to invalid IL or missing references)
		//IL_161f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1624: Unknown result type (might be due to invalid IL or missing references)
		//IL_1633: Unknown result type (might be due to invalid IL or missing references)
		//IL_1638: Unknown result type (might be due to invalid IL or missing references)
		//IL_163f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1644: Unknown result type (might be due to invalid IL or missing references)
		//IL_1649: Unknown result type (might be due to invalid IL or missing references)
		//IL_164e: Unknown result type (might be due to invalid IL or missing references)
		//IL_165d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1662: Unknown result type (might be due to invalid IL or missing references)
		//IL_1669: Unknown result type (might be due to invalid IL or missing references)
		//IL_166e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1673: Unknown result type (might be due to invalid IL or missing references)
		//IL_1678: Unknown result type (might be due to invalid IL or missing references)
		//IL_1687: Unknown result type (might be due to invalid IL or missing references)
		//IL_168c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1693: Unknown result type (might be due to invalid IL or missing references)
		//IL_1698: Unknown result type (might be due to invalid IL or missing references)
		//IL_169d: Unknown result type (might be due to invalid IL or missing references)
		//IL_16a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_16b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_16b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_16bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_16c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_16cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_16d5: Expected O, but got Unknown
		//IL_16de: Unknown result type (might be due to invalid IL or missing references)
		//IL_16e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_16f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_16fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1702: Unknown result type (might be due to invalid IL or missing references)
		//IL_1707: Unknown result type (might be due to invalid IL or missing references)
		//IL_171a: Unknown result type (might be due to invalid IL or missing references)
		//IL_171f: Unknown result type (might be due to invalid IL or missing references)
		//IL_172b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1730: Unknown result type (might be due to invalid IL or missing references)
		//IL_173f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1744: Unknown result type (might be due to invalid IL or missing references)
		//IL_174b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1750: Unknown result type (might be due to invalid IL or missing references)
		//IL_1757: Unknown result type (might be due to invalid IL or missing references)
		//IL_175c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1761: Unknown result type (might be due to invalid IL or missing references)
		//IL_1766: Unknown result type (might be due to invalid IL or missing references)
		//IL_1775: Unknown result type (might be due to invalid IL or missing references)
		//IL_177b: Expected O, but got Unknown
		//IL_1784: Unknown result type (might be due to invalid IL or missing references)
		//IL_1789: Unknown result type (might be due to invalid IL or missing references)
		//IL_1790: Unknown result type (might be due to invalid IL or missing references)
		//IL_1795: Unknown result type (might be due to invalid IL or missing references)
		//IL_17a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_17ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_17b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_17b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_17cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_17d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_17dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_17e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_17f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_17f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_17fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1800: Unknown result type (might be due to invalid IL or missing references)
		//IL_180f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1814: Unknown result type (might be due to invalid IL or missing references)
		//IL_181b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1820: Unknown result type (might be due to invalid IL or missing references)
		//IL_1827: Unknown result type (might be due to invalid IL or missing references)
		//IL_182c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1833: Unknown result type (might be due to invalid IL or missing references)
		//IL_1838: Unknown result type (might be due to invalid IL or missing references)
		//IL_183f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1844: Unknown result type (might be due to invalid IL or missing references)
		//IL_1849: Unknown result type (might be due to invalid IL or missing references)
		//IL_184e: Unknown result type (might be due to invalid IL or missing references)
		//IL_185d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1863: Expected O, but got Unknown
		//IL_186c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1871: Unknown result type (might be due to invalid IL or missing references)
		//IL_1878: Unknown result type (might be due to invalid IL or missing references)
		//IL_187d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1890: Unknown result type (might be due to invalid IL or missing references)
		//IL_1895: Unknown result type (might be due to invalid IL or missing references)
		//IL_189c: Unknown result type (might be due to invalid IL or missing references)
		//IL_18a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_18b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_18b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_18c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_18c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_18d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_18d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_18e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_18eb: Expected O, but got Unknown
		//IL_18f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_18f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_190c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1911: Unknown result type (might be due to invalid IL or missing references)
		//IL_1918: Unknown result type (might be due to invalid IL or missing references)
		//IL_191d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1930: Unknown result type (might be due to invalid IL or missing references)
		//IL_1935: Unknown result type (might be due to invalid IL or missing references)
		//IL_1941: Unknown result type (might be due to invalid IL or missing references)
		//IL_1946: Unknown result type (might be due to invalid IL or missing references)
		//IL_1955: Unknown result type (might be due to invalid IL or missing references)
		//IL_195a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1961: Unknown result type (might be due to invalid IL or missing references)
		//IL_1966: Unknown result type (might be due to invalid IL or missing references)
		//IL_196d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1972: Unknown result type (might be due to invalid IL or missing references)
		//IL_1977: Unknown result type (might be due to invalid IL or missing references)
		//IL_197c: Unknown result type (might be due to invalid IL or missing references)
		//IL_198b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1991: Expected O, but got Unknown
		//IL_199a: Unknown result type (might be due to invalid IL or missing references)
		//IL_199f: Unknown result type (might be due to invalid IL or missing references)
		//IL_19b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_19b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_19be: Unknown result type (might be due to invalid IL or missing references)
		//IL_19c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_19ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_19cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_19e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_19e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_19f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_19fb: Expected O, but got Unknown
		//IL_1a04: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a09: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a21: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a28: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a41: Expected O, but got Unknown
		//IL_1a4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a56: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a73: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a84: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a93: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a98: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aa4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aa9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aae: Unknown result type (might be due to invalid IL or missing references)
		//IL_1abd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ac2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ac9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ace: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ad5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ada: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ae1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ae6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aed: Unknown result type (might be due to invalid IL or missing references)
		//IL_1af2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1af7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1afc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b10: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b17: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b21: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b26: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b35: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b41: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b46: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b50: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b65: Expected O, but got Unknown
		//IL_1b6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b73: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b86: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b92: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b97: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ba3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1baa: Unknown result type (might be due to invalid IL or missing references)
		//IL_1baf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bb6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bbb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bc2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bc7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bce: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bd3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bda: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bdf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bf2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bf7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c03: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c08: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c17: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c23: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c28: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c32: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c41: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c46: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c50: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c64: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c70: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c75: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c89: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c95: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ca1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ca6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cab: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cb0: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_LoadGameSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LoadGameSystem>();
		m_BlockedLaneQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Car>(),
			ComponentType.Exclude<BlockedLane>()
		});
		m_CarLaneQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Net.CarLane>(),
			ComponentType.Exclude<MasterLane>(),
			ComponentType.Exclude<LaneFlow>()
		});
		m_BuildingEfficiencyQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Buildings.TransportDepot>(),
			ComponentType.ReadOnly<Building>(),
			ComponentType.Exclude<Efficiency>()
		});
		m_PolicyQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.City.City>(),
			ComponentType.Exclude<Policy>()
		});
		m_CityModifierQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.City.City>(),
			ComponentType.Exclude<CityModifier>()
		});
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Vehicles.PublicTransport>(),
			ComponentType.ReadOnly<Game.Vehicles.CargoTransport>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ServiceDispatch>() };
		array[0] = val;
		m_ServiceDispatchQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Vehicles.PublicTransport>(),
			ComponentType.ReadOnly<Game.Vehicles.CargoTransport>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PathInformation>() };
		array2[0] = val;
		m_PathInformationQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		m_NodeGeometryQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Net.Node>(),
			ComponentType.ReadOnly<Game.Net.SubLane>(),
			ComponentType.Exclude<NodeGeometry>()
		});
		EntityQueryDesc[] array3 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Tree>(),
			ComponentType.ReadOnly<Plant>(),
			ComponentType.ReadOnly<Human>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.Exclude<MeshColor>() };
		array3[0] = val;
		m_MeshColorQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array3);
		EntityQueryDesc[] array4 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<NodeGeometry>(),
			ComponentType.ReadOnly<EdgeGeometry>(),
			ComponentType.ReadOnly<LaneGeometry>(),
			ComponentType.ReadOnly<ObjectGeometry>(),
			ComponentType.ReadOnly<Game.Objects.Marker>(),
			ComponentType.ReadOnly<Block>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<MeshBatch>() };
		array4[0] = val;
		m_MeshBatchQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array4);
		m_RoutePolicyQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Route>(),
			ComponentType.Exclude<Policy>(),
			ComponentType.Exclude<RouteModifier>()
		});
		m_RouteModifierQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Route>(),
			ComponentType.Exclude<RouteModifier>()
		});
		m_EdgeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Net.Edge>(),
			ComponentType.ReadOnly<ConnectedBuilding>(),
			ComponentType.Exclude<Density>()
		});
		m_StorageTaxQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Companies.StorageCompany>(),
			ComponentType.ReadOnly<TaxPayer>()
		});
		m_CityFeeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.City.City>(),
			ComponentType.Exclude<ServiceFee>()
		});
		m_CityFeeQuery2 = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.City.City>(),
			ComponentType.ReadWrite<ServiceFee>()
		});
		m_ServiceFeeParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ServiceFeeParameterData>() });
		m_OutsideGarbageQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Objects.OutsideConnection>(),
			ComponentType.Exclude<Game.Buildings.GarbageFacility>()
		});
		m_OutsideFireStationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Objects.OutsideConnection>(),
			ComponentType.Exclude<Game.Buildings.FireStation>()
		});
		m_OutsidePoliceStationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Objects.OutsideConnection>(),
			ComponentType.Exclude<Game.Buildings.PoliceStation>()
		});
		m_OutsideEfficiencyQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Objects.OutsideConnection>(),
			ComponentType.Exclude<Efficiency>()
		});
		m_RouteInfoQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Routes.Segment>(),
			ComponentType.ReadOnly<PathTargets>(),
			ComponentType.Exclude<RouteInfo>()
		});
		m_CompanyProfitabilityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<CompanyData>(),
			ComponentType.Exclude<Profitability>(),
			ComponentType.Exclude<Game.Companies.StorageCompany>()
		});
		m_StorageQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<IndustrialProperty>(),
			ComponentType.Exclude<StorageProperty>()
		});
		m_RouteBufferIndexQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Route>(),
			ComponentType.Exclude<RouteBufferIndex>()
		});
		m_CurveElementQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Routes.Segment>(),
			ComponentType.Exclude<CurveElement>()
		});
		m_CitizenPrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Citizen>(),
			ComponentType.Exclude<PrefabRef>()
		});
		m_CitizenNameQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Citizen>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<RandomLocalizationIndex>()
		});
		m_HouseholdNameQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Household>(),
			ComponentType.Exclude<RandomLocalizationIndex>()
		});
		m_DistrictNameQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<District>(),
			ComponentType.ReadOnly<Area>(),
			ComponentType.Exclude<RandomLocalizationIndex>()
		});
		m_AnimalNameQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Animal>(),
			ComponentType.Exclude<RandomLocalizationIndex>()
		});
		m_HouseholdPetQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<HouseholdPet>(),
			ComponentType.Exclude<RandomLocalizationIndex>()
		});
		m_RoadNameQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Aggregate>(),
			ComponentType.ReadOnly<LabelMaterial>(),
			ComponentType.Exclude<RandomLocalizationIndex>()
		});
		m_LabelVertexQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Areas.LabelExtents>(),
			ComponentType.Exclude<Game.Areas.LabelVertex>()
		});
		m_RouteNumberQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<TransportLine>(),
			ComponentType.Exclude<RouteNumber>()
		});
		EntityQueryDesc[] array5 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<LifePathEntry>(),
			ComponentType.ReadOnly<ChirpEntity>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<RandomLocalizationIndex>() };
		array5[0] = val;
		m_ChirpRandomLocQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array5);
		EntityQueryDesc[] array6 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<HumanCurrentLane>(),
			ComponentType.ReadOnly<AnimalCurrentLane>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Blocker>() };
		array6[0] = val;
		m_BlockerQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array6);
		m_CitizenPresenceQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.Exclude<CitizenPresence>()
		});
		m_SubLaneQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.Exclude<Game.Net.SubLane>()
		});
		m_SubObjectQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.Exclude<Game.Objects.SubObject>()
		});
		m_NativeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<MapTile>(),
			ComponentType.Exclude<Native>(),
			ComponentType.Exclude<Owner>()
		});
		EntityQueryDesc[] array7 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Buildings.PostFacility>(),
			ComponentType.ReadOnly<Game.Buildings.GarbageFacility>()
		};
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Buildings.ServiceUpgrade>(),
			ComponentType.ReadOnly<Game.Objects.OutsideConnection>(),
			ComponentType.ReadOnly<GuestVehicle>()
		};
		array7[0] = val;
		m_GuestVehicleQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array7);
		m_TravelPurposeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TravelPurpose>() });
		m_TreeEffectQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Tree>(),
			ComponentType.Exclude<EnabledEffect>()
		});
		m_TakeoffLocationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<AirplaneStop>(),
			ComponentType.ReadOnly<Game.Net.SubLane>(),
			ComponentType.Exclude<Game.Routes.TakeoffLocation>()
		});
		m_LeisureQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<CompanyData>(),
			ComponentType.Exclude<Game.Buildings.LeisureProvider>(),
			ComponentType.ReadOnly<PrefabRef>()
		});
		m_PlayerMoneyQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.City.City>(),
			ComponentType.ReadWrite<Resources>(),
			ComponentType.Exclude<PlayerMoney>()
		});
		EntityQueryDesc[] array8 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<NodeGeometry>(),
			ComponentType.ReadOnly<EdgeGeometry>(),
			ComponentType.ReadOnly<ObjectGeometry>(),
			ComponentType.ReadOnly<AssetStamp>(),
			ComponentType.ReadOnly<Game.Objects.Marker>(),
			ComponentType.ReadOnly<Game.Areas.Lot>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PseudoRandomSeed>() };
		array8[0] = val;
		m_PseudoRandomSeedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array8);
		m_TransportDepotQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Objects.OutsideConnection>(),
			ComponentType.ReadOnly<Game.Buildings.GarbageFacility>(),
			ComponentType.Exclude<Game.Buildings.TransportDepot>()
		});
		m_ServiceUsageQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<CityServiceUpkeep>(),
			ComponentType.Exclude<ServiceUsage>()
		});
		m_OutsideSellerQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Objects.OutsideConnection>(),
			ComponentType.Exclude<ResourceSeller>()
		});
		m_LoadingResourcesQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Vehicles.CargoTransport>(),
			ComponentType.Exclude<LoadingResources>()
		});
		m_CompanyVehicleQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Companies.ProcessingCompany>(),
			ComponentType.Exclude<OwnedVehicle>()
		});
		EntityQueryDesc[] array9 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Owner>() };
		val.Any = (ComponentType[])(object)new ComponentType[7]
		{
			ComponentType.ReadOnly<Game.Net.CarLane>(),
			ComponentType.ReadOnly<Game.Net.ParkingLane>(),
			ComponentType.ReadOnly<Game.Net.PedestrianLane>(),
			ComponentType.ReadOnly<Game.Net.ConnectionLane>(),
			ComponentType.ReadOnly<Game.Routes.TransportStop>(),
			ComponentType.ReadOnly<Game.Routes.TakeoffLocation>(),
			ComponentType.ReadOnly<Game.Objects.SpawnLocation>()
		};
		array9[0] = val;
		m_LaneRestrictionQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array9);
		m_LaneOverlapQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Net.ParkingLane>(),
			ComponentType.Exclude<LaneOverlap>()
		});
		EntityQueryDesc[] array10 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<TransportLine>(),
			ComponentType.ReadOnly<TaxiStand>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<DispatchedRequest>() };
		array10[0] = val;
		m_DispatchedRequestQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array10);
		EntityQueryDesc[] array11 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Buildings.Park>(),
			ComponentType.ReadOnly<Abandoned>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Renter>() };
		array11[0] = val;
		m_HomelessShelterQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array11);
		m_QueueQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Human>(),
			ComponentType.Exclude<Queue>()
		});
		m_BoneHistoryQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Bone>(),
			ComponentType.Exclude<BoneHistory>()
		});
		m_UnspawnedQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<CurrentVehicle>(),
			ComponentType.Exclude<Unspawned>()
		});
		m_ConnectionLaneQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Net.ConnectionLane>(),
			ComponentType.ReadOnly<NodeLane>()
		});
		m_AreaLaneQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Area>(),
			ComponentType.ReadOnly<Game.Net.SubLane>()
		});
		m_OfficeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<IndustrialProperty>(),
			ComponentType.Exclude<OfficeProperty>()
		});
		m_VehicleModelQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TransportLine>() });
		m_PassengerTransportQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Game.Vehicles.PublicTransport>(),
			ComponentType.Exclude<PassengerTransport>(),
			ComponentType.Exclude<EvacuatingTransport>(),
			ComponentType.Exclude<PrisonerTransport>()
		});
		EntityQueryDesc[] array12 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Tree>(),
			ComponentType.ReadOnly<Vehicle>(),
			ComponentType.ReadOnly<Creature>(),
			ComponentType.ReadOnly<Extension>(),
			ComponentType.ReadOnly<Game.Objects.UtilityObject>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Objects.Color>() };
		array12[0] = val;
		m_ObjectColorQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array12);
		EntityQueryDesc[] array13 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Objects.ElectricityOutsideConnection>(),
			ComponentType.ReadOnly<Game.Objects.WaterPipeOutsideConnection>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Objects.OutsideConnection>() };
		array13[0] = val;
		m_OutsideConnectionQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array13);
		m_NetConditionQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Road>(),
			ComponentType.Exclude<NetCondition>()
		});
		m_NetPollutionQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Road>(),
			ComponentType.Exclude<Game.Net.Pollution>()
		});
		m_TrafficSpawnerQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Objects.OutsideConnection>(),
			ComponentType.ReadOnly<OwnedVehicle>(),
			ComponentType.Exclude<Game.Buildings.TrafficSpawner>()
		});
		m_AreaExpandQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Areas.Surface>(),
			ComponentType.Exclude<Expand>()
		});
		EntityQueryDesc[] array14 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<TrafficLight>(),
			ComponentType.ReadOnly<Car>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Emissive>() };
		array14[0] = val;
		m_EmissiveQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array14);
		m_TrainBogieFrameQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<TrainCurrentLane>(),
			ComponentType.Exclude<TrainBogieFrame>()
		});
		m_ProcessingTradeCostQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Companies.ProcessingCompany>(),
			ComponentType.Exclude<TradeCost>()
		});
		EntityQueryDesc[] array15 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Net.Node>(),
			ComponentType.ReadOnly<Game.Net.Edge>(),
			ComponentType.ReadOnly<Game.Objects.Object>()
		};
		val.None = (ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<NodeGeometry>(),
			ComponentType.ReadOnly<EdgeGeometry>(),
			ComponentType.ReadOnly<ObjectGeometry>(),
			ComponentType.ReadOnly<AssetStamp>(),
			ComponentType.ReadOnly<Game.Objects.Marker>(),
			ComponentType.ReadOnly<Game.Tools.EditorContainer>()
		};
		array15[0] = val;
		m_EditorContainerQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array15);
		m_StorageConditionQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Companies.StorageCompany>(),
			ComponentType.ReadOnly<PropertyRenter>()
		});
		EntityQueryDesc[] array16 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Net.TrackLane>(),
			ComponentType.ReadOnly<Game.Net.UtilityLane>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<LaneColor>() };
		array16[0] = val;
		m_LaneColorQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array16);
		m_CompanyNotificationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<CompanyData>(),
			ComponentType.Exclude<CompanyNotifications>()
		});
		m_PlantQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Tree>(),
			ComponentType.Exclude<Plant>()
		});
		m_CityPopulationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.City.City>(),
			ComponentType.Exclude<Population>()
		});
		m_CityTourismQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.City.City>(),
			ComponentType.Exclude<Tourism>()
		});
		m_BuildingNotificationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<ResidentialProperty>(),
			ComponentType.Exclude<BuildingNotifications>()
		});
		m_LaneElevationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<Lane>(),
			ComponentType.ReadOnly<Owner>(),
			ComponentType.Exclude<EdgeLane>(),
			ComponentType.Exclude<AreaLane>(),
			ComponentType.Exclude<Game.Net.ConnectionLane>(),
			ComponentType.Exclude<Game.Net.Elevation>()
		});
		m_AreaElevationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Area>(),
			ComponentType.ReadOnly<Owner>()
		});
		m_BuildingLotQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.Exclude<Game.Buildings.Lot>()
		});
		m_AreaTerrainQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Areas.Lot>(),
			ComponentType.ReadOnly<Storage>(),
			ComponentType.Exclude<Game.Areas.Terrain>()
		});
		m_OwnedVehicleQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Areas.Lot>(),
			ComponentType.ReadOnly<Storage>(),
			ComponentType.Exclude<OwnedVehicle>()
		});
		m_EdgeMappingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Net.UtilityLane>(),
			ComponentType.Exclude<EdgeMapping>()
		});
		m_SubFlowQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Net.UtilityLane>(),
			ComponentType.Exclude<SubFlow>()
		});
		EntityQueryDesc[] array17 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Vehicles.PoliceCar>(),
			ComponentType.ReadOnly<RenewableElectricityProduction>(),
			ComponentType.ReadOnly<Game.Buildings.ExtractorFacility>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PointOfInterest>() };
		array17[0] = val;
		m_PointOfInterestQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array17);
		m_BuildableAreaQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<MapFeatureElement>(),
			ComponentType.Exclude<Updated>()
		});
		m_SubAreaQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Extractor>(),
			ComponentType.Exclude<Game.Areas.SubArea>()
		});
		m_CrimeVictimQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Citizen>(),
			ComponentType.Exclude<CrimeVictim>()
		});
		m_ArrivedQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Citizen>(),
			ComponentType.Exclude<Arrived>()
		});
		m_MailSenderQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Citizen>(),
			ComponentType.Exclude<MailSender>()
		});
		m_CarKeeperQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Citizen>(),
			ComponentType.Exclude<CarKeeper>()
		});
		m_NeedAddHasJobSeekerQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Citizen>(),
			ComponentType.Exclude<HasJobSeeker>()
		});
		EntityQueryDesc[] array18 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Household>(),
			ComponentType.ReadOnly<CompanyData>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.Exclude<PropertySeeker>() };
		array18[0] = val;
		m_NeedAddPropertySeekerQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array18);
		EntityQueryDesc[] array19 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadWrite<Citizen>() };
		val.Any = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Child>(),
			ComponentType.ReadOnly<Teen>(),
			ComponentType.ReadOnly<Adult>(),
			ComponentType.ReadOnly<Elderly>()
		};
		array19[0] = val;
		m_AgeGroupQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array19);
		m_PrefabRefQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PrefabRef>() });
		m_LabelMaterialQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Net.LabelExtents>(),
			ComponentType.Exclude<LabelMaterial>()
		});
		m_ArrowMaterialQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<ArrowPosition>(),
			ComponentType.Exclude<ArrowMaterial>()
		});
		m_LockedQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<UnlockRequirement>(),
			ComponentType.Exclude<Locked>()
		});
		m_OutsideUpdateQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Objects.OutsideConnection>() });
		EntityQueryDesc[] array20 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<AccessLane>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<TaxiStand>(),
			ComponentType.ReadOnly<Waypoint>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<WaitingPassengers>() };
		array20[0] = val;
		m_WaitingPassengersQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array20);
		m_WaitingPassengersQuery2 = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<WaitingPassengers>(),
			ComponentType.Exclude<TaxiStand>(),
			ComponentType.Exclude<Waypoint>()
		});
		EntityQueryDesc[] array21 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<ObjectGeometry>(),
			ComponentType.ReadOnly<PrefabRef>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Objects.UtilityObject>(),
			ComponentType.ReadOnly<Game.Objects.NetObject>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Pillar>() };
		array21[0] = val;
		m_PillarQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array21);
		m_LegacyEfficiencyQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Buildings.BuildingEfficiency>() });
		m_SignatureQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Game.Objects.UniqueObject>(),
			ComponentType.ReadOnly<Renter>(),
			ComponentType.Exclude<Game.Buildings.Park>(),
			ComponentType.Exclude<Signature>()
		});
		EntityQueryDesc[] array22 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Objects.Object>(),
			ComponentType.ReadOnly<Owner>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Creatures.CreatureSpawner>(),
			ComponentType.ReadOnly<Game.Tools.EditorContainer>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Vehicle>(),
			ComponentType.ReadOnly<Creature>()
		};
		array22[0] = val;
		m_SubObjectOwnerQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array22);
		EntityQueryDesc[] array23 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Events.Event>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Events.WeatherPhenomenon>(),
			ComponentType.ReadOnly<WaterLevelChange>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Events.DangerLevel>() };
		array23[0] = val;
		m_DangerLevelMissingQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array23);
		m_MeshGroupQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Human>(),
			ComponentType.ReadOnly<MeshBatch>(),
			ComponentType.Exclude<MeshGroup>()
		});
		EntityQueryDesc[] array24 = new EntityQueryDesc[3];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ObjectGeometry>() };
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Creature>(),
			ComponentType.ReadOnly<Vehicle>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Objects.Surface>() };
		array24[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ObjectGeometry>() };
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Objects.Surface>(),
			ComponentType.ReadOnly<Owner>()
		};
		array24[1] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Road>(),
			ComponentType.ReadOnly<Game.Net.Node>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Objects.Surface>() };
		array24[2] = val;
		m_ObjectSurfaceQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array24);
		m_UpdateFrameQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Plant>(),
			ComponentType.Exclude<UpdateFrame>()
		});
		m_FenceQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<LaneGeometry>(),
			ComponentType.ReadOnly<Game.Net.UtilityLane>(),
			ComponentType.Exclude<PseudoRandomSeed>(),
			ComponentType.Exclude<MeshColor>(),
			ComponentType.Exclude<UpdateFrame>()
		});
		m_NetGeometrySectionQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<NetGeometryData>(),
			ComponentType.Exclude<NetGeometrySection>()
		});
		m_NetLaneArchetypeDataQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<NetLaneData>(),
			ComponentType.Exclude<NetLaneArchetypeData>()
		});
		EntityQueryDesc[] array25 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Owner>() };
		val.Any = (ComponentType[])(object)new ComponentType[8]
		{
			ComponentType.ReadOnly<Game.Net.CarLane>(),
			ComponentType.ReadOnly<Game.Net.PedestrianLane>(),
			ComponentType.ReadOnly<Game.Net.TrackLane>(),
			ComponentType.ReadOnly<Game.Net.ParkingLane>(),
			ComponentType.ReadOnly<Game.Net.ConnectionLane>(),
			ComponentType.ReadOnly<Game.Routes.TransportStop>(),
			ComponentType.ReadOnly<Game.Routes.TakeoffLocation>(),
			ComponentType.ReadOnly<Game.Objects.SpawnLocation>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Routes.MailBox>() };
		array25[0] = val;
		m_PathfindUpdatedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array25);
		m_RouteColorQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<CurrentRoute>(),
			ComponentType.Exclude<Game.Routes.Color>()
		});
		m_CitizenQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Citizen>() });
		m_ServiceUpkeepQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<CityServiceUpkeep>(),
			ComponentType.Exclude<OwnedVehicle>()
		});
		m_MoveableBridgeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Stack>(),
			ComponentType.ReadOnly<Pillar>(),
			ComponentType.ReadOnly<PointOfInterest>()
		});
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_051f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		//IL_0638: Unknown result type (might be due to invalid IL or missing references)
		//IL_063c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_0654: Unknown result type (might be due to invalid IL or missing references)
		//IL_0659: Unknown result type (might be due to invalid IL or missing references)
		//IL_065d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_054a: Unknown result type (might be due to invalid IL or missing references)
		//IL_054f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_0557: Unknown result type (might be due to invalid IL or missing references)
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_067b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0680: Unknown result type (might be due to invalid IL or missing references)
		//IL_0685: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_056c: Unknown result type (might be due to invalid IL or missing references)
		//IL_056f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0579: Unknown result type (might be due to invalid IL or missing references)
		//IL_057e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0585: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_05da: Unknown result type (might be due to invalid IL or missing references)
		//IL_05df: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0769: Unknown result type (might be due to invalid IL or missing references)
		//IL_076e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0772: Unknown result type (might be due to invalid IL or missing references)
		//IL_068d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0692: Unknown result type (might be due to invalid IL or missing references)
		//IL_0699: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0604: Unknown result type (might be due to invalid IL or missing references)
		//IL_078a: Unknown result type (might be due to invalid IL or missing references)
		//IL_078f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0793: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_070d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0714: Unknown result type (might be due to invalid IL or missing references)
		//IL_0828: Unknown result type (might be due to invalid IL or missing references)
		//IL_082d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0832: Unknown result type (might be due to invalid IL or missing references)
		//IL_088b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0890: Unknown result type (might be due to invalid IL or missing references)
		//IL_0895: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_072c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0731: Unknown result type (might be due to invalid IL or missing references)
		//IL_0738: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_083a: Unknown result type (might be due to invalid IL or missing references)
		//IL_083f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0846: Unknown result type (might be due to invalid IL or missing references)
		//IL_084b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0850: Unknown result type (might be due to invalid IL or missing references)
		//IL_0951: Unknown result type (might be due to invalid IL or missing references)
		//IL_0956: Unknown result type (might be due to invalid IL or missing references)
		//IL_095b: Unknown result type (might be due to invalid IL or missing references)
		//IL_089d: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0900: Unknown result type (might be due to invalid IL or missing references)
		//IL_0905: Unknown result type (might be due to invalid IL or missing references)
		//IL_090c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0911: Unknown result type (might be due to invalid IL or missing references)
		//IL_0916: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a17: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a21: Unknown result type (might be due to invalid IL or missing references)
		//IL_0963: Unknown result type (might be due to invalid IL or missing references)
		//IL_0968: Unknown result type (might be due to invalid IL or missing references)
		//IL_096f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0974: Unknown result type (might be due to invalid IL or missing references)
		//IL_0979: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a84: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_09dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0adc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a29: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a35: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0afd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a91: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b19: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b22: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b40: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0baa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bcb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bcf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b52: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b57: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b5e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c2c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c31: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c35: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c10: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c14: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c56: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d43: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d64: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c70: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c77: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d85: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0da2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e11: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e15: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cb1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cb6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cbd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e33: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e38: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0daa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0daf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dbb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ced: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ea1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dcd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cfa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d06: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eb9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ebe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e51: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ef3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ef8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0efc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eda: Unknown result type (might be due to invalid IL or missing references)
		//IL_0edf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ee3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f41: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f20: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f24: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f62: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f66: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f84: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f89: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fdc: Unknown result type (might be due to invalid IL or missing references)
		//IL_100d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1012: Unknown result type (might be due to invalid IL or missing references)
		//IL_1016: Unknown result type (might be due to invalid IL or missing references)
		//IL_101b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ff4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ff9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ffd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f96: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fa2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fa7: Unknown result type (might be due to invalid IL or missing references)
		//IL_104e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1053: Unknown result type (might be due to invalid IL or missing references)
		//IL_1057: Unknown result type (might be due to invalid IL or missing references)
		//IL_105c: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_10be: Unknown result type (might be due to invalid IL or missing references)
		//IL_1035: Unknown result type (might be due to invalid IL or missing references)
		//IL_103a: Unknown result type (might be due to invalid IL or missing references)
		//IL_103e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1076: Unknown result type (might be due to invalid IL or missing references)
		//IL_107b: Unknown result type (might be due to invalid IL or missing references)
		//IL_107f: Unknown result type (might be due to invalid IL or missing references)
		//IL_116f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1174: Unknown result type (might be due to invalid IL or missing references)
		//IL_1178: Unknown result type (might be due to invalid IL or missing references)
		//IL_10e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_10e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_1097: Unknown result type (might be due to invalid IL or missing references)
		//IL_109c: Unknown result type (might be due to invalid IL or missing references)
		//IL_10a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1190: Unknown result type (might be due to invalid IL or missing references)
		//IL_1195: Unknown result type (might be due to invalid IL or missing references)
		//IL_1199: Unknown result type (might be due to invalid IL or missing references)
		//IL_11b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_11b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_10f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_11eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_11d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_11d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_11db: Unknown result type (might be due to invalid IL or missing references)
		//IL_110e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1115: Unknown result type (might be due to invalid IL or missing references)
		//IL_1234: Unknown result type (might be due to invalid IL or missing references)
		//IL_1239: Unknown result type (might be due to invalid IL or missing references)
		//IL_123d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1213: Unknown result type (might be due to invalid IL or missing references)
		//IL_1218: Unknown result type (might be due to invalid IL or missing references)
		//IL_121c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1132: Unknown result type (might be due to invalid IL or missing references)
		//IL_1137: Unknown result type (might be due to invalid IL or missing references)
		//IL_113e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1255: Unknown result type (might be due to invalid IL or missing references)
		//IL_125a: Unknown result type (might be due to invalid IL or missing references)
		//IL_125e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1276: Unknown result type (might be due to invalid IL or missing references)
		//IL_127b: Unknown result type (might be due to invalid IL or missing references)
		//IL_127f: Unknown result type (might be due to invalid IL or missing references)
		//IL_128a: Unknown result type (might be due to invalid IL or missing references)
		//IL_128f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1293: Unknown result type (might be due to invalid IL or missing references)
		//IL_12ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_12b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_12b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_12e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_12e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_12cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1359: Unknown result type (might be due to invalid IL or missing references)
		//IL_135e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1301: Unknown result type (might be due to invalid IL or missing references)
		//IL_1307: Invalid comparison between Unknown and I4
		//IL_141e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1423: Unknown result type (might be due to invalid IL or missing references)
		//IL_1427: Unknown result type (might be due to invalid IL or missing references)
		//IL_1374: Unknown result type (might be due to invalid IL or missing references)
		//IL_1379: Unknown result type (might be due to invalid IL or missing references)
		//IL_137e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1334: Unknown result type (might be due to invalid IL or missing references)
		//IL_1339: Unknown result type (might be due to invalid IL or missing references)
		//IL_133d: Unknown result type (might be due to invalid IL or missing references)
		//IL_130a: Unknown result type (might be due to invalid IL or missing references)
		//IL_130f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1313: Unknown result type (might be due to invalid IL or missing references)
		//IL_131e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1323: Unknown result type (might be due to invalid IL or missing references)
		//IL_1327: Unknown result type (might be due to invalid IL or missing references)
		//IL_143f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1444: Unknown result type (might be due to invalid IL or missing references)
		//IL_1448: Unknown result type (might be due to invalid IL or missing references)
		//IL_1460: Unknown result type (might be due to invalid IL or missing references)
		//IL_1465: Unknown result type (might be due to invalid IL or missing references)
		//IL_1469: Unknown result type (might be due to invalid IL or missing references)
		//IL_1386: Unknown result type (might be due to invalid IL or missing references)
		//IL_138f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1481: Unknown result type (might be due to invalid IL or missing references)
		//IL_1486: Unknown result type (might be due to invalid IL or missing references)
		//IL_148a: Unknown result type (might be due to invalid IL or missing references)
		//IL_139e: Unknown result type (might be due to invalid IL or missing references)
		//IL_13a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_14a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_14a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_14ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_13b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_13bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_14d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_14dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_14c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_14c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_14cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_13d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_13da: Unknown result type (might be due to invalid IL or missing references)
		//IL_13df: Unknown result type (might be due to invalid IL or missing references)
		//IL_13ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_177d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1782: Unknown result type (might be due to invalid IL or missing references)
		//IL_1786: Unknown result type (might be due to invalid IL or missing references)
		//IL_1503: Unknown result type (might be due to invalid IL or missing references)
		//IL_1508: Unknown result type (might be due to invalid IL or missing references)
		//IL_150d: Unknown result type (might be due to invalid IL or missing references)
		//IL_179e: Unknown result type (might be due to invalid IL or missing references)
		//IL_17a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_17a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1649: Unknown result type (might be due to invalid IL or missing references)
		//IL_164e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1653: Unknown result type (might be due to invalid IL or missing references)
		//IL_17d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_17d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_17bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_17c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_17c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_151b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1520: Unknown result type (might be due to invalid IL or missing references)
		//IL_1523: Unknown result type (might be due to invalid IL or missing references)
		//IL_1528: Unknown result type (might be due to invalid IL or missing references)
		//IL_152b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1532: Unknown result type (might be due to invalid IL or missing references)
		//IL_1537: Unknown result type (might be due to invalid IL or missing references)
		//IL_153a: Unknown result type (might be due to invalid IL or missing references)
		//IL_153f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1661: Unknown result type (might be due to invalid IL or missing references)
		//IL_1666: Unknown result type (might be due to invalid IL or missing references)
		//IL_1669: Unknown result type (might be due to invalid IL or missing references)
		//IL_166e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1671: Unknown result type (might be due to invalid IL or missing references)
		//IL_1674: Unknown result type (might be due to invalid IL or missing references)
		//IL_1679: Unknown result type (might be due to invalid IL or missing references)
		//IL_167c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1681: Unknown result type (might be due to invalid IL or missing references)
		//IL_1684: Unknown result type (might be due to invalid IL or missing references)
		//IL_154e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1553: Unknown result type (might be due to invalid IL or missing references)
		//IL_1556: Unknown result type (might be due to invalid IL or missing references)
		//IL_1568: Unknown result type (might be due to invalid IL or missing references)
		//IL_156d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1579: Unknown result type (might be due to invalid IL or missing references)
		//IL_1592: Unknown result type (might be due to invalid IL or missing references)
		//IL_1597: Unknown result type (might be due to invalid IL or missing references)
		//IL_15a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_15b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_15b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_15bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_15c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_15cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_15d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_15da: Unknown result type (might be due to invalid IL or missing references)
		//IL_15dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_184e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1853: Unknown result type (might be due to invalid IL or missing references)
		//IL_1857: Unknown result type (might be due to invalid IL or missing references)
		//IL_185c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1835: Unknown result type (might be due to invalid IL or missing references)
		//IL_183a: Unknown result type (might be due to invalid IL or missing references)
		//IL_183e: Unknown result type (might be due to invalid IL or missing references)
		//IL_17f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_17f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_17fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1691: Unknown result type (might be due to invalid IL or missing references)
		//IL_1696: Unknown result type (might be due to invalid IL or missing references)
		//IL_15ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_15f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_15f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_15fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_15ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_1605: Unknown result type (might be due to invalid IL or missing references)
		//IL_160a: Unknown result type (might be due to invalid IL or missing references)
		//IL_160d: Unknown result type (might be due to invalid IL or missing references)
		//IL_188b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1890: Unknown result type (might be due to invalid IL or missing references)
		//IL_1814: Unknown result type (might be due to invalid IL or missing references)
		//IL_1819: Unknown result type (might be due to invalid IL or missing references)
		//IL_181d: Unknown result type (might be due to invalid IL or missing references)
		//IL_16a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_16ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_18bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_18c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1876: Unknown result type (might be due to invalid IL or missing references)
		//IL_187b: Unknown result type (might be due to invalid IL or missing references)
		//IL_187f: Unknown result type (might be due to invalid IL or missing references)
		//IL_193b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1940: Unknown result type (might be due to invalid IL or missing references)
		//IL_18aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_18af: Unknown result type (might be due to invalid IL or missing references)
		//IL_18b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_19b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_19bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_18e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_18e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_18ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_18f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_18f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_18fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_16da: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a33: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a38: Unknown result type (might be due to invalid IL or missing references)
		//IL_1960: Unknown result type (might be due to invalid IL or missing references)
		//IL_1965: Unknown result type (might be due to invalid IL or missing references)
		//IL_196a: Unknown result type (might be due to invalid IL or missing references)
		//IL_196d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1972: Unknown result type (might be due to invalid IL or missing references)
		//IL_1976: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aaf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ab4: Unknown result type (might be due to invalid IL or missing references)
		//IL_19dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_19e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_19e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_19e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_19ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_19f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_190a: Unknown result type (might be due to invalid IL or missing references)
		//IL_190f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1916: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a58: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a62: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a65: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1986: Unknown result type (might be due to invalid IL or missing references)
		//IL_198b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1992: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c35: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b40: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b45: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ad4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ad9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ade: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ae1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ae6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aea: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a02: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a07: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c52: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c57: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c60: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c65: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a83: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cd3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cd8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cdc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b58: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b60: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b65: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b68: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b72: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b77: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1afa: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aff: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b06: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d09: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cf4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cf9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cfd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c72: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c77: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c83: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ba2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b87: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1df0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1df5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c90: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c95: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bc2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bc7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bca: Unknown result type (might be due to invalid IL or missing references)
		//IL_1baf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bb4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bb7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c07: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e91: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d34: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d39: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bed: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bf2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bf5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bd7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bdc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bdf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ec0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ec5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e18: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e22: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f15: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1eab: Unknown result type (might be due to invalid IL or missing references)
		//IL_1eb0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1eb4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d49: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d52: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fbb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fc0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1edf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ee4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ee8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e2f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e36: Unknown result type (might be due to invalid IL or missing references)
		//IL_1db2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1db7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dbe: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d62: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d67: Unknown result type (might be due to invalid IL or missing references)
		//IL_208d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2092: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f40: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f45: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f53: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f58: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f00: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f05: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f09: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e43: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e48: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e60: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e67: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d74: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d79: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_20c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_20c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fe0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fe5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fe9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ff4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ff9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ffd: Unknown result type (might be due to invalid IL or missing references)
		//IL_200e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2013: Unknown result type (might be due to invalid IL or missing references)
		//IL_2018: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d91: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d97: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1da3: Unknown result type (might be due to invalid IL or missing references)
		//IL_2180: Unknown result type (might be due to invalid IL or missing references)
		//IL_2185: Unknown result type (might be due to invalid IL or missing references)
		//IL_20ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_20b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_20b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f65: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f76: Unknown result type (might be due to invalid IL or missing references)
		//IL_21b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_21b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_20ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_20f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_20f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_2020: Unknown result type (might be due to invalid IL or missing references)
		//IL_2029: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f83: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f88: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_21e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_21ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_219f: Unknown result type (might be due to invalid IL or missing references)
		//IL_21a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_21a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_2038: Unknown result type (might be due to invalid IL or missing references)
		//IL_203f: Unknown result type (might be due to invalid IL or missing references)
		//IL_221c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2221: Unknown result type (might be due to invalid IL or missing references)
		//IL_21d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_21d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_21dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_2101: Unknown result type (might be due to invalid IL or missing references)
		//IL_210a: Unknown result type (might be due to invalid IL or missing references)
		//IL_2250: Unknown result type (might be due to invalid IL or missing references)
		//IL_2255: Unknown result type (might be due to invalid IL or missing references)
		//IL_2207: Unknown result type (might be due to invalid IL or missing references)
		//IL_220c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2210: Unknown result type (might be due to invalid IL or missing references)
		//IL_2119: Unknown result type (might be due to invalid IL or missing references)
		//IL_211e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2123: Unknown result type (might be due to invalid IL or missing references)
		//IL_205c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2061: Unknown result type (might be due to invalid IL or missing references)
		//IL_2068: Unknown result type (might be due to invalid IL or missing references)
		//IL_22c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_22c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_223b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2240: Unknown result type (might be due to invalid IL or missing references)
		//IL_2244: Unknown result type (might be due to invalid IL or missing references)
		//IL_2130: Unknown result type (might be due to invalid IL or missing references)
		//IL_2135: Unknown result type (might be due to invalid IL or missing references)
		//IL_213c: Unknown result type (might be due to invalid IL or missing references)
		//IL_234d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2352: Unknown result type (might be due to invalid IL or missing references)
		//IL_2275: Unknown result type (might be due to invalid IL or missing references)
		//IL_227a: Unknown result type (might be due to invalid IL or missing references)
		//IL_227f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2492: Unknown result type (might be due to invalid IL or missing references)
		//IL_2497: Unknown result type (might be due to invalid IL or missing references)
		//IL_22e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_22ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_22f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_24da: Unknown result type (might be due to invalid IL or missing references)
		//IL_24df: Unknown result type (might be due to invalid IL or missing references)
		//IL_2378: Unknown result type (might be due to invalid IL or missing references)
		//IL_237d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2382: Unknown result type (might be due to invalid IL or missing references)
		//IL_2287: Unknown result type (might be due to invalid IL or missing references)
		//IL_228c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2293: Unknown result type (might be due to invalid IL or missing references)
		//IL_250e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2513: Unknown result type (might be due to invalid IL or missing references)
		//IL_24b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_24b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_24ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_24c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_24ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_24ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_2315: Unknown result type (might be due to invalid IL or missing references)
		//IL_231a: Unknown result type (might be due to invalid IL or missing references)
		//IL_2321: Unknown result type (might be due to invalid IL or missing references)
		//IL_2542: Unknown result type (might be due to invalid IL or missing references)
		//IL_2547: Unknown result type (might be due to invalid IL or missing references)
		//IL_24f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_24fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_2502: Unknown result type (might be due to invalid IL or missing references)
		//IL_2390: Unknown result type (might be due to invalid IL or missing references)
		//IL_2395: Unknown result type (might be due to invalid IL or missing references)
		//IL_2398: Unknown result type (might be due to invalid IL or missing references)
		//IL_239d: Unknown result type (might be due to invalid IL or missing references)
		//IL_25dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_25e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_252d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2532: Unknown result type (might be due to invalid IL or missing references)
		//IL_2536: Unknown result type (might be due to invalid IL or missing references)
		//IL_23ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_23b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_256a: Unknown result type (might be due to invalid IL or missing references)
		//IL_256f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2574: Unknown result type (might be due to invalid IL or missing references)
		//IL_2433: Unknown result type (might be due to invalid IL or missing references)
		//IL_2438: Unknown result type (might be due to invalid IL or missing references)
		//IL_243d: Unknown result type (might be due to invalid IL or missing references)
		//IL_27d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_27db: Unknown result type (might be due to invalid IL or missing references)
		//IL_27c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_27c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_27ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_260b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2610: Unknown result type (might be due to invalid IL or missing references)
		//IL_2619: Unknown result type (might be due to invalid IL or missing references)
		//IL_261e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2623: Unknown result type (might be due to invalid IL or missing references)
		//IL_262e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2633: Unknown result type (might be due to invalid IL or missing references)
		//IL_2638: Unknown result type (might be due to invalid IL or missing references)
		//IL_263d: Unknown result type (might be due to invalid IL or missing references)
		//IL_244a: Unknown result type (might be due to invalid IL or missing references)
		//IL_244f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2452: Unknown result type (might be due to invalid IL or missing references)
		//IL_245b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2460: Unknown result type (might be due to invalid IL or missing references)
		//IL_2463: Unknown result type (might be due to invalid IL or missing references)
		//IL_23d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_23da: Unknown result type (might be due to invalid IL or missing references)
		//IL_257c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2585: Unknown result type (might be due to invalid IL or missing references)
		//IL_23dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_23e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_2802: Unknown result type (might be due to invalid IL or missing references)
		//IL_2807: Unknown result type (might be due to invalid IL or missing references)
		//IL_280c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2677: Unknown result type (might be due to invalid IL or missing references)
		//IL_2680: Unknown result type (might be due to invalid IL or missing references)
		//IL_2594: Unknown result type (might be due to invalid IL or missing references)
		//IL_259b: Unknown result type (might be due to invalid IL or missing references)
		//IL_240f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2414: Unknown result type (might be due to invalid IL or missing references)
		//IL_2417: Unknown result type (might be due to invalid IL or missing references)
		//IL_2422: Unknown result type (might be due to invalid IL or missing references)
		//IL_2427: Unknown result type (might be due to invalid IL or missing references)
		//IL_242a: Unknown result type (might be due to invalid IL or missing references)
		//IL_23ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_23f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_26df: Unknown result type (might be due to invalid IL or missing references)
		//IL_26e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_25aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_25af: Unknown result type (might be due to invalid IL or missing references)
		//IL_25b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_2405: Unknown result type (might be due to invalid IL or missing references)
		//IL_240a: Unknown result type (might be due to invalid IL or missing references)
		//IL_2817: Unknown result type (might be due to invalid IL or missing references)
		//IL_2820: Unknown result type (might be due to invalid IL or missing references)
		//IL_2699: Unknown result type (might be due to invalid IL or missing references)
		//IL_269e: Unknown result type (might be due to invalid IL or missing references)
		//IL_282f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2838: Unknown result type (might be due to invalid IL or missing references)
		//IL_26c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_26c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_26ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_26ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_26b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_26b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_2847: Unknown result type (might be due to invalid IL or missing references)
		//IL_284e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2865: Unknown result type (might be due to invalid IL or missing references)
		//IL_2871: Unknown result type (might be due to invalid IL or missing references)
		//IL_2876: Unknown result type (might be due to invalid IL or missing references)
		//IL_288b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2890: Unknown result type (might be due to invalid IL or missing references)
		//IL_2897: Unknown result type (might be due to invalid IL or missing references)
		//IL_2780: Unknown result type (might be due to invalid IL or missing references)
		//IL_2785: Unknown result type (might be due to invalid IL or missing references)
		//IL_278c: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager;
		if (!((EntityQuery)(ref m_BlockedLaneQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<BlockedLane>(m_BlockedLaneQuery);
		}
		if (!((EntityQuery)(ref m_CarLaneQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<LaneFlow>(m_CarLaneQuery);
		}
		if (!((EntityQuery)(ref m_BuildingEfficiencyQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Efficiency>(m_BuildingEfficiencyQuery);
		}
		if (!((EntityQuery)(ref m_PolicyQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Policy>(m_PolicyQuery);
		}
		if (!((EntityQuery)(ref m_CityModifierQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<CityModifier>(m_CityModifierQuery);
		}
		if (!((EntityQuery)(ref m_ServiceDispatchQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<ServiceDispatch>(m_ServiceDispatchQuery);
		}
		if (!((EntityQuery)(ref m_PathInformationQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<PathInformation>(m_PathInformationQuery);
		}
		if (!((EntityQuery)(ref m_NodeGeometryQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<NodeGeometry>(m_NodeGeometryQuery);
		}
		if (!((EntityQuery)(ref m_MeshBatchQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<MeshBatch>(m_MeshBatchQuery);
		}
		if (!((EntityQuery)(ref m_RoutePolicyQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Policy>(m_RoutePolicyQuery);
		}
		if (!((EntityQuery)(ref m_RouteModifierQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<RouteModifier>(m_RouteModifierQuery);
		}
		if (!((EntityQuery)(ref m_EdgeQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Density>(m_EdgeQuery);
		}
		if (!((EntityQuery)(ref m_StorageTaxQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).RemoveComponent<TaxPayer>(m_StorageTaxQuery);
		}
		if (!((EntityQuery)(ref m_CityFeeQuery)).IsEmptyIgnoreFilter)
		{
			ServiceFeeParameterData singleton = ((EntityQuery)(ref m_ServiceFeeParameterQuery)).GetSingleton<ServiceFeeParameterData>();
			NativeArray<Entity> val = ((EntityQuery)(ref m_CityFeeQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			for (int i = 0; i < val.Length; i++)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				DynamicBuffer<ServiceFee> val2 = ((EntityManager)(ref entityManager)).AddBuffer<ServiceFee>(val[i]);
				foreach (ServiceFee defaultFee in singleton.GetDefaultFees())
				{
					val2.Add(defaultFee);
				}
			}
			val.Dispose();
		}
		if (!((EntityQuery)(ref m_CityFeeQuery2)).IsEmptyIgnoreFilter)
		{
			Entity singletonEntity = ((EntityQuery)(ref m_CityFeeQuery2)).GetSingletonEntity();
			entityManager = ((ComponentSystemBase)this).EntityManager;
			DynamicBuffer<ServiceFee> buffer = ((EntityManager)(ref entityManager)).GetBuffer<ServiceFee>(singletonEntity, false);
			bool flag = false;
			for (int j = 0; j < buffer.Length; j++)
			{
				if (buffer[j].m_Resource == PlayerResource.Water)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				ServiceFee serviceFee = default(ServiceFee);
				serviceFee.m_Resource = PlayerResource.Water;
				serviceFee.m_Fee = serviceFee.GetDefaultFee(serviceFee.m_Resource);
				buffer.Add(serviceFee);
			}
		}
		if (!((EntityQuery)(ref m_OutsideGarbageQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val3 = ((EntityQuery)(ref m_OutsideGarbageQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			DynamicBuffer<Resources> resources = default(DynamicBuffer<Resources>);
			for (int k = 0; k < val3.Length; k++)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				Entity prefab = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(val3[k]).m_Prefab;
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<GarbageFacilityData>(prefab))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					GarbageFacilityData componentData = ((EntityManager)(ref entityManager)).GetComponentData<GarbageFacilityData>(prefab);
					if (EntitiesExtensions.TryGetBuffer<Resources>(((ComponentSystemBase)this).EntityManager, val3[k], false, ref resources))
					{
						EconomyUtils.SetResources(Resource.Garbage, resources, componentData.m_GarbageCapacity / 2);
					}
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (!((EntityManager)(ref entityManager)).HasComponent<ServiceDispatch>(val3[k]))
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						((EntityManager)(ref entityManager)).AddBuffer<ServiceDispatch>(val3[k]);
					}
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (!((EntityManager)(ref entityManager)).HasComponent<OwnedVehicle>(val3[k]))
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						((EntityManager)(ref entityManager)).AddBuffer<OwnedVehicle>(val3[k]);
					}
				}
			}
			val3.Dispose();
		}
		if (!((EntityQuery)(ref m_OutsideFireStationQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val4 = ((EntityQuery)(ref m_OutsideFireStationQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			for (int l = 0; l < val4.Length; l++)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				Entity prefab2 = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(val4[l]).m_Prefab;
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<FireStationData>(prefab2))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					((EntityManager)(ref entityManager)).GetComponentData<FireStationData>(prefab2);
					entityManager = ((ComponentSystemBase)this).EntityManager;
					((EntityManager)(ref entityManager)).AddComponentData<Game.Buildings.FireStation>(val4[l], default(Game.Buildings.FireStation));
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (!((EntityManager)(ref entityManager)).HasComponent<ServiceDispatch>(val4[l]))
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						((EntityManager)(ref entityManager)).AddBuffer<ServiceDispatch>(val4[l]);
					}
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (!((EntityManager)(ref entityManager)).HasComponent<OwnedVehicle>(val4[l]))
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						((EntityManager)(ref entityManager)).AddBuffer<OwnedVehicle>(val4[l]);
					}
				}
			}
			val4.Dispose();
		}
		if (!((EntityQuery)(ref m_OutsidePoliceStationQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val5 = ((EntityQuery)(ref m_OutsidePoliceStationQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			for (int m = 0; m < val5.Length; m++)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				Entity prefab3 = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(val5[m]).m_Prefab;
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<PoliceStationData>(prefab3))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					PoliceStationData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<PoliceStationData>(prefab3);
					entityManager = ((ComponentSystemBase)this).EntityManager;
					((EntityManager)(ref entityManager)).AddComponentData<Game.Buildings.PoliceStation>(val5[m], new Game.Buildings.PoliceStation
					{
						m_PurposeMask = componentData2.m_PurposeMask
					});
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (!((EntityManager)(ref entityManager)).HasComponent<ServiceDispatch>(val5[m]))
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						((EntityManager)(ref entityManager)).AddBuffer<ServiceDispatch>(val5[m]);
					}
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (!((EntityManager)(ref entityManager)).HasComponent<OwnedVehicle>(val5[m]))
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						((EntityManager)(ref entityManager)).AddBuffer<OwnedVehicle>(val5[m]);
					}
				}
			}
			val5.Dispose();
		}
		if (!((EntityQuery)(ref m_OutsideEfficiencyQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Efficiency>(m_OutsideEfficiencyQuery);
		}
		if (!((EntityQuery)(ref m_RouteInfoQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<RouteInfo>(m_RouteInfoQuery);
		}
		if (!((EntityQuery)(ref m_CompanyProfitabilityQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val6 = ((EntityQuery)(ref m_CompanyProfitabilityQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			for (int n = 0; n < val6.Length; n++)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddComponentData<Profitability>(val6[n], new Profitability
				{
					m_Profitability = 127
				});
			}
			val6.Dispose();
		}
		if (!((EntityQuery)(ref m_StorageQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val7 = ((EntityQuery)(ref m_StorageQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			PrefabRef prefabRef = default(PrefabRef);
			BuildingPropertyData buildingPropertyData = default(BuildingPropertyData);
			for (int num = 0; num < val7.Length; num++)
			{
				if (EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, val7[num], ref prefabRef) && EntitiesExtensions.TryGetComponent<BuildingPropertyData>(((ComponentSystemBase)this).EntityManager, prefabRef.m_Prefab, ref buildingPropertyData) && buildingPropertyData.m_AllowedStored != Resource.NoResource)
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					((EntityManager)(ref entityManager)).AddComponent<StorageProperty>(val7[num]);
				}
			}
			val7.Dispose();
		}
		if (!((EntityQuery)(ref m_RouteBufferIndexQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<RouteBufferIndex>(m_RouteBufferIndexQuery);
		}
		if (!((EntityQuery)(ref m_CurveElementQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<CurveElement>(m_CurveElementQuery);
		}
		if (!((EntityQuery)(ref m_CitizenPrefabQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<PrefabRef>(m_CitizenPrefabQuery);
			NativeArray<Entity> val8 = ((EntityQuery)(ref m_CitizenPrefabQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			for (int num2 = 0; num2 < val8.Length; num2++)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddBuffer<RandomLocalizationIndex>(val8[num2]).Add(RandomLocalizationIndex.kNone);
			}
			val8.Dispose();
		}
		if (!((EntityQuery)(ref m_CitizenNameQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val9 = ((EntityQuery)(ref m_CitizenNameQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			for (int num3 = 0; num3 < val9.Length; num3++)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddBuffer<RandomLocalizationIndex>(val9[num3]).Add(RandomLocalizationIndex.kNone);
			}
			val9.Dispose();
		}
		if (!((EntityQuery)(ref m_HouseholdNameQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val10 = ((EntityQuery)(ref m_HouseholdNameQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			for (int num4 = 0; num4 < val10.Length; num4++)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddBuffer<RandomLocalizationIndex>(val10[num4]).Add(RandomLocalizationIndex.kNone);
			}
			val10.Dispose();
		}
		if (!((EntityQuery)(ref m_DistrictNameQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val11 = ((EntityQuery)(ref m_DistrictNameQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			for (int num5 = 0; num5 < val11.Length; num5++)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddBuffer<RandomLocalizationIndex>(val11[num5]).Add(RandomLocalizationIndex.kNone);
			}
			val11.Dispose();
		}
		if (!((EntityQuery)(ref m_AnimalNameQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val12 = ((EntityQuery)(ref m_AnimalNameQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			for (int num6 = 0; num6 < val12.Length; num6++)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddBuffer<RandomLocalizationIndex>(val12[num6]).Add(RandomLocalizationIndex.kNone);
			}
			val12.Dispose();
		}
		if (!((EntityQuery)(ref m_HouseholdPetQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val13 = ((EntityQuery)(ref m_HouseholdPetQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			for (int num7 = 0; num7 < val13.Length; num7++)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddBuffer<RandomLocalizationIndex>(val13[num7]).Add(RandomLocalizationIndex.kNone);
			}
			val13.Dispose();
		}
		if (!((EntityQuery)(ref m_RoadNameQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val14 = ((EntityQuery)(ref m_RoadNameQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			for (int num8 = 0; num8 < val14.Length; num8++)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddBuffer<RandomLocalizationIndex>(val14[num8]).Add(RandomLocalizationIndex.kNone);
			}
			val14.Dispose();
		}
		if (!((EntityQuery)(ref m_ChirpRandomLocQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val15 = ((EntityQuery)(ref m_ChirpRandomLocQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			for (int num9 = 0; num9 < val15.Length; num9++)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddBuffer<RandomLocalizationIndex>(val15[num9]).Add(RandomLocalizationIndex.kNone);
			}
			val15.Dispose();
		}
		if (!((EntityQuery)(ref m_LabelVertexQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Game.Areas.LabelVertex>(m_LabelVertexQuery);
		}
		if (!((EntityQuery)(ref m_RouteNumberQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<RouteNumber>(m_RouteNumberQuery);
		}
		if (!((EntityQuery)(ref m_BlockerQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Blocker>(m_BlockerQuery);
		}
		if (!((EntityQuery)(ref m_CitizenPresenceQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val16 = ((EntityQuery)(ref m_CitizenPresenceQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			for (int num10 = 0; num10 < val16.Length; num10++)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddComponentData<CitizenPresence>(val16[num10], new CitizenPresence
				{
					m_Presence = 128
				});
			}
			val16.Dispose();
		}
		if (!((EntityQuery)(ref m_SubLaneQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Game.Net.SubLane>(m_SubLaneQuery);
		}
		if (!((EntityQuery)(ref m_SubObjectQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Game.Objects.SubObject>(m_SubObjectQuery);
		}
		Context context = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LoadGameSystem>().context;
		if (((Context)(ref context)).version < Version.netUpkeepCost && !((EntityQuery)(ref m_NativeQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Native>(m_NativeQuery);
		}
		if (!((EntityQuery)(ref m_GuestVehicleQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<GuestVehicle>(m_GuestVehicleQuery);
		}
		if (!((EntityQuery)(ref m_TravelPurposeQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val17 = ((EntityQuery)(ref m_TravelPurposeQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			for (int num11 = 0; num11 < val17.Length; num11++)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				TravelPurpose componentData3 = ((EntityManager)(ref entityManager)).GetComponentData<TravelPurpose>(val17[num11]);
				if (componentData3.m_Purpose == Purpose.GoingToWork || componentData3.m_Purpose == Purpose.Working)
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (!((EntityManager)(ref entityManager)).HasComponent<Worker>(val17[num11]))
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						((EntityManager)(ref entityManager)).RemoveComponent<TravelPurpose>(val17[num11]);
						continue;
					}
				}
				if (componentData3.m_Purpose == Purpose.GoingToSchool || componentData3.m_Purpose == Purpose.Studying)
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (!((EntityManager)(ref entityManager)).HasComponent<Game.Citizens.Student>(val17[num11]))
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						((EntityManager)(ref entityManager)).RemoveComponent<TravelPurpose>(val17[num11]);
					}
				}
			}
			val17.Dispose();
		}
		if (!((EntityQuery)(ref m_TreeEffectQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<EnabledEffect>(m_TreeEffectQuery);
		}
		if (!((EntityQuery)(ref m_TakeoffLocationQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Game.Routes.TakeoffLocation>(m_TakeoffLocationQuery);
		}
		if (!((EntityQuery)(ref m_LeisureQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val18 = ((EntityQuery)(ref m_LeisureQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			NativeArray<PrefabRef> val19 = ((EntityQuery)(ref m_LeisureQuery)).ToComponentDataArray<PrefabRef>(AllocatorHandle.op_Implicit((Allocator)3));
			for (int num12 = 0; num12 < val19.Length; num12++)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<LeisureProviderData>(val19[num12].m_Prefab))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					((EntityManager)(ref entityManager)).AddComponent<Game.Buildings.LeisureProvider>(val18[num12]);
				}
			}
			val19.Dispose();
			val18.Dispose();
		}
		if (!((EntityQuery)(ref m_TransportDepotQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Game.Buildings.TransportDepot>(m_TransportDepotQuery);
		}
		if (!((EntityQuery)(ref m_ServiceUsageQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val20 = ((EntityQuery)(ref m_ServiceUsageQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			for (int num13 = 0; num13 < val20.Length; num13++)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddComponentData<ServiceUsage>(val20[num13], new ServiceUsage
				{
					m_Usage = 1f
				});
			}
			val20.Dispose();
		}
		if (!((EntityQuery)(ref m_OutsideSellerQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<ResourceSeller>(m_OutsideSellerQuery);
		}
		if (!((EntityQuery)(ref m_LoadingResourcesQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<LoadingResources>(m_LoadingResourcesQuery);
		}
		if (!((EntityQuery)(ref m_CompanyVehicleQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<OwnedVehicle>(m_CompanyVehicleQuery);
		}
		Context context2 = m_LoadGameSystem.context;
		if (((Context)(ref context2)).version < Version.pathfindAccessRestriction && !((EntityQuery)(ref m_LaneRestrictionQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<PathfindUpdated>(m_LaneRestrictionQuery);
		}
		if (!((EntityQuery)(ref m_LaneOverlapQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<LaneOverlap>(m_LaneOverlapQuery);
		}
		if (!((EntityQuery)(ref m_DispatchedRequestQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<DispatchedRequest>(m_DispatchedRequestQuery);
		}
		if (!((EntityQuery)(ref m_HomelessShelterQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val21 = ((EntityQuery)(ref m_HomelessShelterQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			for (int num14 = 0; num14 < val21.Length; num14++)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddBuffer<Renter>(val21[num14]);
			}
			val21.Dispose();
		}
		if (!((EntityQuery)(ref m_QueueQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Queue>(m_QueueQuery);
		}
		if (!((EntityQuery)(ref m_BoneHistoryQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<BoneHistory>(m_BoneHistoryQuery);
		}
		context2 = m_LoadGameSystem.context;
		if (((Context)(ref context2)).version < Version.currentVehicleRefactoring && !((EntityQuery)(ref m_UnspawnedQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Unspawned>(m_UnspawnedQuery);
		}
		context2 = m_LoadGameSystem.context;
		if (((Context)(ref context2)).version < Version.areaLaneComponent)
		{
			if (!((EntityQuery)(ref m_ConnectionLaneQuery)).IsEmptyIgnoreFilter)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).RemoveComponent<NodeLane>(m_ConnectionLaneQuery);
			}
			if (!((EntityQuery)(ref m_AreaLaneQuery)).IsEmptyIgnoreFilter)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddComponent<Updated>(m_AreaLaneQuery);
			}
		}
		context2 = m_LoadGameSystem.context;
		if (((Context)(ref context2)).version < Version.officePropertyComponent && !((EntityQuery)(ref m_OfficeQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val22 = ((EntityQuery)(ref m_OfficeQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			PrefabRef prefabRef2 = default(PrefabRef);
			BuildingPropertyData buildingPropertyData2 = default(BuildingPropertyData);
			for (int num15 = 0; num15 < val22.Length; num15++)
			{
				if (EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, val22[num15], ref prefabRef2) && EntitiesExtensions.TryGetComponent<BuildingPropertyData>(((ComponentSystemBase)this).EntityManager, prefabRef2.m_Prefab, ref buildingPropertyData2) && EconomyUtils.IsOfficeResource(buildingPropertyData2.m_AllowedManufactured))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					((EntityManager)(ref entityManager)).AddComponent<OfficeProperty>(val22[num15]);
				}
			}
			val22.Dispose();
		}
		if (!((EntityQuery)(ref m_PassengerTransportQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<PassengerTransport>(m_PassengerTransportQuery);
		}
		if (!((EntityQuery)(ref m_ObjectColorQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Game.Objects.Color>(m_ObjectColorQuery);
		}
		if (!((EntityQuery)(ref m_OutsideConnectionQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Game.Objects.OutsideConnection>(m_OutsideConnectionQuery);
		}
		if (!((EntityQuery)(ref m_NetConditionQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<NetCondition>(m_NetConditionQuery);
		}
		context2 = m_LoadGameSystem.context;
		if (((Context)(ref context2)).version < Version.netPollutionAccumulation && !((EntityQuery)(ref m_NetPollutionQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Game.Net.Pollution>(m_NetPollutionQuery);
		}
		if (!((EntityQuery)(ref m_TrafficSpawnerQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Game.Buildings.TrafficSpawner>(m_TrafficSpawnerQuery);
		}
		if (!((EntityQuery)(ref m_AreaExpandQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Expand>(m_AreaExpandQuery);
		}
		if (!((EntityQuery)(ref m_EmissiveQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<LightState>(m_EmissiveQuery);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Emissive>(m_EmissiveQuery);
		}
		if (!((EntityQuery)(ref m_TrainBogieFrameQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<TrainBogieFrame>(m_TrainBogieFrameQuery);
		}
		if (!((EntityQuery)(ref m_ProcessingTradeCostQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<TradeCost>(m_ProcessingTradeCostQuery);
		}
		if (((Context)(ref context)).version < Version.editorContainerFix && !((EntityQuery)(ref m_EditorContainerQuery)).IsEmptyIgnoreFilter)
		{
			if ((int)((Context)(ref context)).purpose == 5)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddComponent<CullingInfo>(m_EditorContainerQuery);
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddComponent<Game.Tools.EditorContainer>(m_EditorContainerQuery);
			}
			else
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).DestroyEntity(m_EditorContainerQuery);
			}
		}
		if (!((EntityQuery)(ref m_StorageConditionQuery)).IsEmptyIgnoreFilter && ((Context)(ref context)).version < Version.storageConditionReset)
		{
			NativeArray<Entity> val23 = ((EntityQuery)(ref m_StorageConditionQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			PropertyRenter propertyRenter = default(PropertyRenter);
			BuildingCondition buildingCondition = default(BuildingCondition);
			DynamicBuffer<Resources> resources2 = default(DynamicBuffer<Resources>);
			for (int num16 = 0; num16 < val23.Length; num16++)
			{
				if (EntitiesExtensions.TryGetComponent<PropertyRenter>(((ComponentSystemBase)this).EntityManager, val23[num16], ref propertyRenter) && EntitiesExtensions.TryGetComponent<BuildingCondition>(((ComponentSystemBase)this).EntityManager, propertyRenter.m_Property, ref buildingCondition) && EntitiesExtensions.TryGetBuffer<Resources>(((ComponentSystemBase)this).EntityManager, val23[num16], false, ref resources2))
				{
					buildingCondition.m_Condition = 0;
					entityManager = ((ComponentSystemBase)this).EntityManager;
					((EntityManager)(ref entityManager)).SetComponentData<BuildingCondition>(propertyRenter.m_Property, buildingCondition);
					EconomyUtils.SetResources(Resource.Money, resources2, 0);
				}
			}
			val23.Dispose();
		}
		if (!((EntityQuery)(ref m_LaneColorQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<LaneColor>(m_LaneColorQuery);
		}
		if (!((EntityQuery)(ref m_CompanyNotificationQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<CompanyNotifications>(m_CompanyNotificationQuery);
		}
		if (!((EntityQuery)(ref m_PlantQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Plant>(m_PlantQuery);
		}
		if (!((EntityQuery)(ref m_CityPopulationQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Population>(m_CityPopulationQuery);
		}
		if (!((EntityQuery)(ref m_CityTourismQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Tourism>(m_CityTourismQuery);
		}
		if (!((EntityQuery)(ref m_BuildingNotificationQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<BuildingNotifications>(m_BuildingNotificationQuery);
		}
		if (((Context)(ref context)).version < Version.laneElevation)
		{
			if (!((EntityQuery)(ref m_LaneElevationQuery)).IsEmptyIgnoreFilter)
			{
				NativeArray<Entity> val24 = ((EntityQuery)(ref m_LaneElevationQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
				Transform transform = default(Transform);
				Game.Net.Elevation elevation = default(Game.Net.Elevation);
				for (int num17 = 0; num17 < val24.Length; num17++)
				{
					Entity val25 = val24[num17];
					entityManager = ((ComponentSystemBase)this).EntityManager;
					Entity owner = ((EntityManager)(ref entityManager)).GetComponentData<Owner>(val25).m_Owner;
					if (EntitiesExtensions.TryGetComponent<Transform>(((ComponentSystemBase)this).EntityManager, owner, ref transform))
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						Curve componentData4 = ((EntityManager)(ref entityManager)).GetComponentData<Curve>(val25);
						elevation.m_Elevation.x = componentData4.m_Bezier.a.y - transform.m_Position.y;
						elevation.m_Elevation.y = componentData4.m_Bezier.d.y - transform.m_Position.y;
						entityManager = ((ComponentSystemBase)this).EntityManager;
						((EntityManager)(ref entityManager)).RemoveComponent<NodeLane>(val25);
						bool2 val26 = math.abs(elevation.m_Elevation) >= 0.1f;
						if (math.any(val26))
						{
							elevation.m_Elevation = math.select(float2.op_Implicit(float.MinValue), elevation.m_Elevation, val26);
							entityManager = ((ComponentSystemBase)this).EntityManager;
							((EntityManager)(ref entityManager)).AddComponentData<Game.Net.Elevation>(val25, elevation);
						}
					}
				}
				val24.Dispose();
			}
			if (!((EntityQuery)(ref m_AreaElevationQuery)).IsEmptyIgnoreFilter)
			{
				NativeArray<Entity> val27 = ((EntityQuery)(ref m_AreaElevationQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
				Owner owner2 = default(Owner);
				Transform transform2 = default(Transform);
				for (int num18 = 0; num18 < val27.Length; num18++)
				{
					Entity val28 = val27[num18];
					entityManager = ((ComponentSystemBase)this).EntityManager;
					DynamicBuffer<Game.Areas.Node> buffer2 = ((EntityManager)(ref entityManager)).GetBuffer<Game.Areas.Node>(val28, false);
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<Space>(val28) && EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, val28, ref owner2) && EntitiesExtensions.TryGetComponent<Transform>(((ComponentSystemBase)this).EntityManager, owner2.m_Owner, ref transform2))
					{
						for (int num19 = 0; num19 < buffer2.Length; num19++)
						{
							ref Game.Areas.Node reference = ref buffer2.ElementAt(num19);
							reference.m_Elevation = reference.m_Position.y - transform2.m_Position.y;
							reference.m_Elevation = math.select(float.MinValue, reference.m_Elevation, math.abs(reference.m_Elevation) >= 0.1f);
						}
					}
					else
					{
						for (int num20 = 0; num20 < buffer2.Length; num20++)
						{
							buffer2.ElementAt(num20).m_Elevation = float.MinValue;
						}
					}
				}
				val27.Dispose();
			}
		}
		if (!((EntityQuery)(ref m_BuildingLotQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Game.Buildings.Lot>(m_BuildingLotQuery);
		}
		if (!((EntityQuery)(ref m_AreaTerrainQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Game.Areas.Terrain>(m_AreaTerrainQuery);
		}
		if (!((EntityQuery)(ref m_OwnedVehicleQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<OwnedVehicle>(m_OwnedVehicleQuery);
		}
		if (((Context)(ref context)).version < Version.laneSubFlow)
		{
			if (!((EntityQuery)(ref m_EdgeMappingQuery)).IsEmptyIgnoreFilter)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddComponent<EdgeMapping>(m_EdgeMappingQuery);
			}
			if (!((EntityQuery)(ref m_SubFlowQuery)).IsEmptyIgnoreFilter)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddComponent<SubFlow>(m_SubFlowQuery);
			}
		}
		if (!((EntityQuery)(ref m_PointOfInterestQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<PointOfInterest>(m_PointOfInterestQuery);
		}
		context2 = m_LoadGameSystem.context;
		if (((Context)(ref context2)).version < Version.buildableArea && !((EntityQuery)(ref m_BuildableAreaQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Updated>(m_BuildableAreaQuery);
		}
		if (((Context)(ref context)).version < Version.extractorSubAreas && !((EntityQuery)(ref m_SubAreaQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Game.Areas.SubArea>(m_SubAreaQuery);
		}
		if (((Context)(ref context)).version < Version.enableableCrimeVictim && !((EntityQuery)(ref m_CrimeVictimQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val29 = ((EntityQuery)(ref m_CrimeVictimQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<CrimeVictim>(m_CrimeVictimQuery);
			for (int num21 = 0; num21 < val29.Length; num21++)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentEnabled<CrimeVictim>(val29[num21], false);
			}
			val29.Dispose();
		}
		if (((Context)(ref context)).version < Version.enableableCrimeVictim && !((EntityQuery)(ref m_ArrivedQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val30 = ((EntityQuery)(ref m_ArrivedQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Arrived>(m_ArrivedQuery);
			for (int num22 = 0; num22 < val30.Length; num22++)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentEnabled<Arrived>(val30[num22], false);
			}
			val30.Dispose();
		}
		if (((Context)(ref context)).version < Version.enableableCrimeVictim && !((EntityQuery)(ref m_MailSenderQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val31 = ((EntityQuery)(ref m_MailSenderQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<MailSender>(m_MailSenderQuery);
			for (int num23 = 0; num23 < val31.Length; num23++)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentEnabled<MailSender>(val31[num23], false);
			}
			val31.Dispose();
		}
		if (((Context)(ref context)).version < Version.enableableCrimeVictim && !((EntityQuery)(ref m_CarKeeperQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val32 = ((EntityQuery)(ref m_CarKeeperQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<CarKeeper>(m_CarKeeperQuery);
			for (int num24 = 0; num24 < val32.Length; num24++)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentEnabled<CarKeeper>(val32[num24], false);
			}
			val32.Dispose();
		}
		if (((Context)(ref context)).version < Version.findJobOptimize && !((EntityQuery)(ref m_NeedAddHasJobSeekerQuery)).IsEmpty)
		{
			NativeArray<Entity> val33 = ((EntityQuery)(ref m_NeedAddHasJobSeekerQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<HasJobSeeker>(m_NeedAddHasJobSeekerQuery);
			for (int num25 = 0; num25 < val33.Length; num25++)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentEnabled<HasJobSeeker>(val33[num25], false);
			}
			val33.Dispose();
		}
		if (!((EntityQuery)(ref m_AgeGroupQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val34 = ((EntityQuery)(ref m_AgeGroupQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			for (int num26 = 0; num26 < val34.Length; num26++)
			{
				Entity val35 = val34[num26];
				entityManager = ((ComponentSystemBase)this).EntityManager;
				Citizen componentData5 = ((EntityManager)(ref entityManager)).GetComponentData<Citizen>(val35);
				entityManager = ((ComponentSystemBase)this).EntityManager;
				CitizenAge age;
				if (((EntityManager)(ref entityManager)).HasComponent<Child>(val35))
				{
					age = CitizenAge.Child;
					entityManager = ((ComponentSystemBase)this).EntityManager;
					((EntityManager)(ref entityManager)).RemoveComponent<Child>(val35);
				}
				else
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<Teen>(val35))
					{
						age = CitizenAge.Teen;
						entityManager = ((ComponentSystemBase)this).EntityManager;
						((EntityManager)(ref entityManager)).RemoveComponent<Teen>(val35);
					}
					else
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						if (((EntityManager)(ref entityManager)).HasComponent<Adult>(val35))
						{
							age = CitizenAge.Adult;
							entityManager = ((ComponentSystemBase)this).EntityManager;
							((EntityManager)(ref entityManager)).RemoveComponent<Adult>(val35);
						}
						else
						{
							age = CitizenAge.Elderly;
							entityManager = ((ComponentSystemBase)this).EntityManager;
							((EntityManager)(ref entityManager)).RemoveComponent<Elderly>(val35);
						}
					}
				}
				componentData5.SetAge(age);
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentData<Citizen>(val35, componentData5);
			}
			val34.Dispose();
		}
		if (((Context)(ref context)).version < Version.prefabRefAbuseFix)
		{
			NativeArray<Entity> val36 = ((EntityQuery)(ref m_PrefabRefQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			NativeArray<PrefabRef> val37 = ((EntityQuery)(ref m_PrefabRefQuery)).ToComponentDataArray<PrefabRef>(AllocatorHandle.op_Implicit((Allocator)3));
			for (int num27 = 0; num27 < val37.Length; num27++)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (!((EntityManager)(ref entityManager)).HasComponent<PrefabData>((Entity)val37[num27]))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					((EntityManager)(ref entityManager)).DestroyEntity(val36[num27]);
				}
			}
			val36.Dispose();
			val37.Dispose();
		}
		if (!((EntityQuery)(ref m_LabelMaterialQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<LabelMaterial>(m_LabelMaterialQuery);
		}
		if (!((EntityQuery)(ref m_ArrowMaterialQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<ArrowMaterial>(m_ArrowMaterialQuery);
		}
		if (((Context)(ref context)).version < Version.trainRouteSecondaryModelFix && !((EntityQuery)(ref m_VehicleModelQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val38 = ((EntityQuery)(ref m_VehicleModelQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			VehicleModel vehicleModel = default(VehicleModel);
			for (int num28 = 0; num28 < val38.Length; num28++)
			{
				if (EntitiesExtensions.TryGetComponent<VehicleModel>(((ComponentSystemBase)this).EntityManager, val38[num28], ref vehicleModel))
				{
					if (vehicleModel.m_SecondaryPrefab != Entity.Null)
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						if (((EntityManager)(ref entityManager)).HasComponent<TrainEngineData>(vehicleModel.m_PrimaryPrefab))
						{
							vehicleModel.m_SecondaryPrefab = Entity.Null;
							entityManager = ((ComponentSystemBase)this).EntityManager;
							((EntityManager)(ref entityManager)).SetComponentData<VehicleModel>(val38[num28], vehicleModel);
						}
					}
				}
				else
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					((EntityManager)(ref entityManager)).AddComponentData<VehicleModel>(val38[num28], default(VehicleModel));
				}
			}
			val38.Dispose();
		}
		if (((Context)(ref context)).version < Version.enableableLocked && !((EntityQuery)(ref m_LockedQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val39 = ((EntityQuery)(ref m_LockedQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			for (int num29 = 0; num29 < val39.Length; num29++)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (!((EntityManager)(ref entityManager)).HasComponent<Locked>(val39[num29]))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					((EntityManager)(ref entityManager)).AddComponent<Locked>(val39[num29]);
					entityManager = ((ComponentSystemBase)this).EntityManager;
					((EntityManager)(ref entityManager)).SetComponentEnabled<Locked>(val39[num29], false);
				}
			}
			val39.Dispose();
		}
		if (((Context)(ref context)).version < Version.pedestrianBorderCost && !((EntityQuery)(ref m_OutsideUpdateQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Updated>(m_OutsideUpdateQuery);
		}
		if (((Context)(ref context)).version < Version.passengerWaitTimeCost)
		{
			if (!((EntityQuery)(ref m_WaitingPassengersQuery)).IsEmptyIgnoreFilter)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddComponent<WaitingPassengers>(m_WaitingPassengersQuery);
			}
			if (!((EntityQuery)(ref m_WaitingPassengersQuery2)).IsEmptyIgnoreFilter)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).RemoveComponent<WaitingPassengers>(m_WaitingPassengersQuery2);
			}
		}
		if (((Context)(ref context)).version < Version.pillarTerrainModification && !((EntityQuery)(ref m_PillarQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val40 = ((EntityQuery)(ref m_PillarQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			NativeArray<PrefabRef> val41 = ((EntityQuery)(ref m_PillarQuery)).ToComponentDataArray<PrefabRef>(AllocatorHandle.op_Implicit((Allocator)3));
			for (int num30 = 0; num30 < val40.Length; num30++)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<PillarData>(val41[num30].m_Prefab))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					((EntityManager)(ref entityManager)).AddComponent<Pillar>(val40[num30]);
				}
			}
			val40.Dispose();
			val41.Dispose();
		}
		if (((Context)(ref context)).version < Version.buildingEfficiencyRework && !((EntityQuery)(ref m_LegacyEfficiencyQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Efficiency>(m_LegacyEfficiencyQuery);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).RemoveComponent<Game.Buildings.BuildingEfficiency>(m_LegacyEfficiencyQuery);
			NativeArray<Entity> val42 = ((EntityQuery)(ref m_LegacyEfficiencyQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			PrefabRef prefabRef3 = default(PrefabRef);
			ConsumptionData consumptionData = default(ConsumptionData);
			for (int num31 = 0; num31 < val42.Length; num31++)
			{
				if (EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, val42[num31], ref prefabRef3) && EntitiesExtensions.TryGetComponent<ConsumptionData>(((ComponentSystemBase)this).EntityManager, (Entity)prefabRef3, ref consumptionData) && consumptionData.m_TelecomNeed > 0f)
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					((EntityManager)(ref entityManager)).AddComponent<TelecomConsumer>(val42[num31]);
				}
			}
			val42.Dispose();
		}
		if (((Context)(ref context)).version < Version.signatureBuildingComponent && !((EntityQuery)(ref m_SignatureQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Signature>(m_SignatureQuery);
		}
		if (((Context)(ref context)).version < Version.missingOwnerFix && !((EntityQuery)(ref m_SubObjectOwnerQuery)).IsEmptyIgnoreFilter)
		{
			int num32 = 0;
			NativeArray<Entity> val43 = ((EntityQuery)(ref m_SubObjectOwnerQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			Owner owner3 = default(Owner);
			for (int num33 = 0; num33 < val43.Length; num33++)
			{
				if (EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, val43[num33], ref owner3))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (!((EntityManager)(ref entityManager)).Exists(owner3.m_Owner))
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						((EntityManager)(ref entityManager)).DestroyEntity(val43[num33]);
						num32++;
					}
				}
			}
			val43.Dispose();
			if (num32 != 0)
			{
				Debug.LogWarning((object)$"Destroyed {num32} entities with missing owners");
			}
		}
		if (((Context)(ref context)).version < Version.dangerLevel && !((EntityQuery)(ref m_DangerLevelMissingQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Game.Events.DangerLevel>(m_DangerLevelMissingQuery);
		}
		if (((Context)(ref context)).version < Version.meshGroups && !((EntityQuery)(ref m_MeshGroupQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<MeshGroup>(m_MeshGroupQuery);
		}
		if (((Context)(ref context)).version < Version.surfaceStates && !((EntityQuery)(ref m_ObjectSurfaceQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Game.Objects.Surface>(m_ObjectSurfaceQuery);
		}
		if (((Context)(ref context)).version < Version.meshColors && !((EntityQuery)(ref m_MeshColorQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<MeshColor>(m_MeshColorQuery);
		}
		if (((Context)(ref context)).version < Version.plantUpdateFrame && !((EntityQuery)(ref m_UpdateFrameQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val44 = ((EntityQuery)(ref m_UpdateFrameQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			for (int num34 = 0; num34 < val44.Length; num34++)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddSharedComponent<UpdateFrame>(val44[num34], new UpdateFrame((uint)(num34 & 0xF)));
			}
			val44.Dispose();
		}
		if (((Context)(ref context)).version < Version.fenceColors && !((EntityQuery)(ref m_PseudoRandomSeedQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val45 = ((EntityQuery)(ref m_PseudoRandomSeedQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			Random random = default(Random);
			((Random)(ref random))._002Ector(math.max(1u, (uint)DateTime.Now.Ticks));
			for (int num35 = 0; num35 < val45.Length; num35++)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddComponentData<PseudoRandomSeed>(val45[num35], new PseudoRandomSeed(ref random));
			}
			val45.Dispose();
		}
		if (((Context)(ref context)).version < Version.fenceColors && !((EntityQuery)(ref m_FenceQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val46 = ((EntityQuery)(ref m_FenceQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			PrefabRef prefabRef4 = default(PrefabRef);
			NetLaneData netLaneData = default(NetLaneData);
			Owner owner4 = default(Owner);
			for (int num36 = 0; num36 < val46.Length; num36++)
			{
				Entity val47 = val46[num36];
				if (!EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, val47, ref prefabRef4))
				{
					continue;
				}
				if (EntitiesExtensions.TryGetComponent<NetLaneData>(((ComponentSystemBase)this).EntityManager, prefabRef4.m_Prefab, ref netLaneData) && (netLaneData.m_Flags & LaneFlags.PseudoRandom) != 0)
				{
					PseudoRandomSeed pseudoRandomSeed = default(PseudoRandomSeed);
					Entity val48 = val47;
					while (EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, val48, ref owner4) && !EntitiesExtensions.TryGetComponent<PseudoRandomSeed>(((ComponentSystemBase)this).EntityManager, owner4.m_Owner, ref pseudoRandomSeed))
					{
						val48 = owner4.m_Owner;
					}
					entityManager = ((ComponentSystemBase)this).EntityManager;
					((EntityManager)(ref entityManager)).AddComponentData<PseudoRandomSeed>(val47, pseudoRandomSeed);
					entityManager = ((ComponentSystemBase)this).EntityManager;
					((EntityManager)(ref entityManager)).AddComponent<MeshColor>(val47);
				}
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<PlantData>(prefabRef4.m_Prefab))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					((EntityManager)(ref entityManager)).AddComponent<Plant>(val47);
					entityManager = ((ComponentSystemBase)this).EntityManager;
					((EntityManager)(ref entityManager)).AddSharedComponent<UpdateFrame>(val47, new UpdateFrame((uint)(num36 & 0xF)));
				}
			}
			val46.Dispose();
		}
		if (((Context)(ref context)).version < Version.obsoleteNetPrefabs && !((EntityQuery)(ref m_NetGeometrySectionQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<NetGeometryComposition>(m_NetGeometrySectionQuery);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<NetGeometrySection>(m_NetGeometrySectionQuery);
		}
		if (((Context)(ref context)).version < Version.obsoleteNetLanePrefabs && !((EntityQuery)(ref m_NetLaneArchetypeDataQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<NetLaneArchetypeData>(m_NetLaneArchetypeDataQuery);
		}
		if (((Context)(ref context)).version < Version.pathfindRestrictions && !((EntityQuery)(ref m_PathfindUpdatedQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<PathfindUpdated>(m_PathfindUpdatedQuery);
		}
		if (((Context)(ref context)).version < Version.cacheRouteColors && !((EntityQuery)(ref m_RouteColorQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val49 = ((EntityQuery)(ref m_RouteColorQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			CurrentRoute currentRoute = default(CurrentRoute);
			Game.Routes.Color color = default(Game.Routes.Color);
			for (int num37 = 0; num37 < val49.Length; num37++)
			{
				if (EntitiesExtensions.TryGetComponent<CurrentRoute>(((ComponentSystemBase)this).EntityManager, val49[num37], ref currentRoute) && EntitiesExtensions.TryGetComponent<Game.Routes.Color>(((ComponentSystemBase)this).EntityManager, currentRoute.m_Route, ref color))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					((EntityManager)(ref entityManager)).AddComponentData<Game.Routes.Color>(val49[num37], color);
				}
			}
			val49.Dispose();
		}
		if (((Context)(ref context)).version < Version.deathWaveMitigation && !((EntityQuery)(ref m_CitizenQuery)).IsEmptyIgnoreFilter)
		{
			Random random2 = RandomSeed.Next().GetRandom(0);
			NativeArray<Entity> val50 = ((EntityQuery)(ref m_CitizenQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			EntityQuery entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TimeData>() });
			TimeData singleton2 = ((EntityQuery)(ref entityQuery)).GetSingleton<TimeData>();
			uint frameIndex = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>().frameIndex;
			int day = TimeSystem.GetDay(frameIndex, ((EntityQuery)(ref __query_1938549531_0)).GetSingleton<TimeData>());
			HealthProblem healthProblem = default(HealthProblem);
			Citizen citizen = default(Citizen);
			for (int num38 = 0; num38 < val50.Length; num38++)
			{
				if (EntitiesExtensions.TryGetComponent<HealthProblem>(((ComponentSystemBase)this).EntityManager, val50[num38], ref healthProblem) && CitizenUtils.IsDead(healthProblem))
				{
					if (!(healthProblem.m_HealthcareRequest == Entity.Null))
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						if (((EntityManager)(ref entityManager)).HasComponent<Dispatched>(healthProblem.m_HealthcareRequest))
						{
							goto IL_26de;
						}
					}
					entityManager = ((ComponentSystemBase)this).EntityManager;
					((EntityManager)(ref entityManager)).AddComponent<Deleted>(val50[num38]);
					continue;
				}
				goto IL_26de;
				IL_26de:
				if (EntitiesExtensions.TryGetComponent<Citizen>(((ComponentSystemBase)this).EntityManager, val50[num38], ref citizen) && citizen.GetAgeInDays(frameIndex, singleton2) >= (float)AgingSystem.GetElderAgeLimitInDays() && ((Random)(ref random2)).NextInt(100) > 1)
				{
					switch (((Random)(ref random2)).NextInt(3))
					{
					case 0:
						citizen.m_BirthDay = (short)(day - 54 + ((Random)(ref random2)).NextInt(18));
						break;
					case 1:
						citizen.m_BirthDay = (short)(day - 69 + ((Random)(ref random2)).NextInt(21));
						break;
					default:
						citizen.m_BirthDay = (short)(day - 84 + ((Random)(ref random2)).NextInt(21));
						break;
					}
					citizen.SetAge(CitizenAge.Adult);
					entityManager = ((ComponentSystemBase)this).EntityManager;
					((EntityManager)(ref entityManager)).SetComponentData<Citizen>(val50[num38], citizen);
				}
			}
			val50.Dispose();
		}
		if (!((EntityQuery)(ref m_ServiceUpkeepQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<OwnedVehicle>(m_ServiceUpkeepQuery);
		}
		ContextFormat format = ((Context)(ref context)).format;
		if (((ContextFormat)(ref format)).Has<FormatTags>(FormatTags.StandingLegOffset) || ((EntityQuery)(ref m_MoveableBridgeQuery)).IsEmptyIgnoreFilter)
		{
			return;
		}
		NativeArray<Entity> val51 = ((EntityQuery)(ref m_MoveableBridgeQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		Stack stack = default(Stack);
		PrefabRef prefabRef5 = default(PrefabRef);
		ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
		for (int num39 = 0; num39 < val51.Length; num39++)
		{
			if (EntitiesExtensions.TryGetComponent<Stack>(((ComponentSystemBase)this).EntityManager, val51[num39], ref stack) && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, val51[num39], ref prefabRef5) && EntitiesExtensions.TryGetComponent<ObjectGeometryData>(((ComponentSystemBase)this).EntityManager, prefabRef5.m_Prefab, ref objectGeometryData))
			{
				stack.m_Range.max = math.max(stack.m_Range.max, objectGeometryData.m_Bounds.max.y);
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentData<Stack>(val51[num39], stack);
			}
		}
		val51.Dispose();
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
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<TimeData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_1938549531_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
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
	public RequiredComponentSystem()
	{
	}
}
