using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game.Areas;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Debug;
using Game.Economy;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Game.Triggers;
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
public class CitizenHappinessSystem : GameSystemBase, IDefaultSerializable, ISerializable
{
	public enum HappinessFactor
	{
		Telecom,
		Crime,
		AirPollution,
		Apartment,
		Electricity,
		Healthcare,
		GroundPollution,
		NoisePollution,
		Water,
		WaterPollution,
		Sewage,
		Garbage,
		Entertainment,
		Education,
		Mail,
		Welfare,
		Leisure,
		Tax,
		Buildings,
		Consumption,
		TrafficPenalty,
		DeathPenalty,
		Homelessness,
		ElectricityFee,
		WaterFee,
		Count
	}

	private struct FactorItem
	{
		public HappinessFactor m_Factor;

		public int4 m_Value;

		public uint m_UpdateFrame;
	}

	[BurstCompile]
	private struct CitizenHappinessJob : IJobChunk
	{
		[NativeDisableContainerSafetyRestriction]
		public ParallelWriter<int> m_DebugQueue;

		public bool m_DebugOn;

		public ComponentTypeHandle<Citizen> m_CitizenType;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<HouseholdMember> m_HouseholdMemberType;

		[ReadOnly]
		public ComponentTypeHandle<CrimeVictim> m_CrimeVictimType;

		[ReadOnly]
		public ComponentTypeHandle<Criminal> m_CriminalType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Citizens.Student> m_StudentType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentBuilding> m_CurrentBuildingType;

		[ReadOnly]
		public ComponentTypeHandle<HealthProblem> m_HealthProblemType;

		[ReadOnly]
		public BufferLookup<Resources> m_Resources;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_Properties;

		[ReadOnly]
		public ComponentLookup<ElectricityConsumer> m_ElectricityConsumers;

		[ReadOnly]
		public ComponentLookup<WaterConsumer> m_WaterConsumers;

		[ReadOnly]
		public ComponentLookup<Building> m_Buildings;

		[ReadOnly]
		public ComponentLookup<Transform> m_Transforms;

		[ReadOnly]
		public ComponentLookup<CurrentDistrict> m_CurrentDistrictData;

		[ReadOnly]
		public BufferLookup<Game.Net.ServiceCoverage> m_ServiceCoverages;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<GarbageProducer> m_Garbages;

		[ReadOnly]
		public ComponentLookup<Household> m_Households;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> m_HouseholdCitizens;

		[ReadOnly]
		public ComponentLookup<Locked> m_Locked;

		[ReadOnly]
		public ComponentLookup<CrimeProducer> m_CrimeProducers;

		[ReadOnly]
		public ComponentLookup<MailProducer> m_MailProducers;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> m_SpawnableBuildings;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_BuildingDatas;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> m_BuildingPropertyDatas;

		[ReadOnly]
		public BufferLookup<DistrictModifier> m_DistrictModifiers;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifiers;

		[ReadOnly]
		public BufferLookup<ServiceFee> m_ServiceFees;

		[ReadOnly]
		public ComponentLookup<HealthProblem> m_HealthProblems;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Prison> m_Prisons;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.School> m_Schools;

		[ReadOnly]
		public ComponentLookup<HomelessHousehold> m_HomelessHouseholds;

		[ReadOnly]
		public NativeArray<GroundPollution> m_PollutionMap;

		[ReadOnly]
		public NativeArray<AirPollution> m_AirPollutionMap;

		[ReadOnly]
		public NativeArray<NoisePollution> m_NoisePollutionMap;

		[ReadOnly]
		public CellMapData<TelecomCoverage> m_TelecomCoverage;

		[ReadOnly]
		public LocalEffectSystem.ReadData m_LocalEffectData;

		[ReadOnly]
		public NativeArray<int> m_TaxRates;

		public HealthcareParameterData m_HealthcareParameters;

		public ParkParameterData m_ParkParameters;

		public EducationParameterData m_EducationParameters;

		public TelecomParameterData m_TelecomParameters;

		public GarbageParameterData m_GarbageParameters;

		public PoliceConfigurationData m_PoliceParameters;

		public CitizenHappinessParameterData m_CitizenHappinessParameters;

		public TimeSettingsData m_TimeSettings;

		public ServiceFeeParameterData m_FeeParameters;

		public TimeData m_TimeData;

		public Entity m_City;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		public uint m_RawUpdateFrame;

		public ParallelWriter<FactorItem> m_FactorQueue;

		public uint m_SimulationFrame;

		public ParallelWriter<StatisticsEvent> m_StatisticsEventQueue;

		private void AddData(float value)
		{
			if (m_DebugOn)
			{
				m_DebugQueue.Enqueue(Mathf.RoundToInt(value));
			}
		}

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
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_132c: Unknown result type (might be due to invalid IL or missing references)
			//IL_132e: Unknown result type (might be due to invalid IL or missing references)
			//IL_135f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1361: Unknown result type (might be due to invalid IL or missing references)
			//IL_1392: Unknown result type (might be due to invalid IL or missing references)
			//IL_1394: Unknown result type (might be due to invalid IL or missing references)
			//IL_13c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_13c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_13f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_13fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_142c: Unknown result type (might be due to invalid IL or missing references)
			//IL_142e: Unknown result type (might be due to invalid IL or missing references)
			//IL_145f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1461: Unknown result type (might be due to invalid IL or missing references)
			//IL_1492: Unknown result type (might be due to invalid IL or missing references)
			//IL_1494: Unknown result type (might be due to invalid IL or missing references)
			//IL_14c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_14c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_14f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_14fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_152c: Unknown result type (might be due to invalid IL or missing references)
			//IL_152e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1560: Unknown result type (might be due to invalid IL or missing references)
			//IL_1562: Unknown result type (might be due to invalid IL or missing references)
			//IL_1594: Unknown result type (might be due to invalid IL or missing references)
			//IL_1596: Unknown result type (might be due to invalid IL or missing references)
			//IL_15c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_15ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_15fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_15fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_1630: Unknown result type (might be due to invalid IL or missing references)
			//IL_1632: Unknown result type (might be due to invalid IL or missing references)
			//IL_1664: Unknown result type (might be due to invalid IL or missing references)
			//IL_1666: Unknown result type (might be due to invalid IL or missing references)
			//IL_1698: Unknown result type (might be due to invalid IL or missing references)
			//IL_169a: Unknown result type (might be due to invalid IL or missing references)
			//IL_16cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_16ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_1700: Unknown result type (might be due to invalid IL or missing references)
			//IL_1702: Unknown result type (might be due to invalid IL or missing references)
			//IL_1734: Unknown result type (might be due to invalid IL or missing references)
			//IL_1736: Unknown result type (might be due to invalid IL or missing references)
			//IL_1768: Unknown result type (might be due to invalid IL or missing references)
			//IL_176a: Unknown result type (might be due to invalid IL or missing references)
			//IL_179c: Unknown result type (might be due to invalid IL or missing references)
			//IL_179e: Unknown result type (might be due to invalid IL or missing references)
			//IL_17d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_17d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_1804: Unknown result type (might be due to invalid IL or missing references)
			//IL_1806: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_040c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_0416: Unknown result type (might be due to invalid IL or missing references)
			//IL_0423: Unknown result type (might be due to invalid IL or missing references)
			//IL_042b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0443: Unknown result type (might be due to invalid IL or missing references)
			//IL_044b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0463: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0473: Unknown result type (might be due to invalid IL or missing references)
			//IL_047b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0483: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0493: Unknown result type (might be due to invalid IL or missing references)
			//IL_049b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0506: Unknown result type (might be due to invalid IL or missing references)
			//IL_050b: Unknown result type (might be due to invalid IL or missing references)
			//IL_050f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0514: Unknown result type (might be due to invalid IL or missing references)
			//IL_051c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0521: Unknown result type (might be due to invalid IL or missing references)
			//IL_0529: Unknown result type (might be due to invalid IL or missing references)
			//IL_052e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0536: Unknown result type (might be due to invalid IL or missing references)
			//IL_053b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0543: Unknown result type (might be due to invalid IL or missing references)
			//IL_0548: Unknown result type (might be due to invalid IL or missing references)
			//IL_0550: Unknown result type (might be due to invalid IL or missing references)
			//IL_0555: Unknown result type (might be due to invalid IL or missing references)
			//IL_055d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0562: Unknown result type (might be due to invalid IL or missing references)
			//IL_0564: Unknown result type (might be due to invalid IL or missing references)
			//IL_0569: Unknown result type (might be due to invalid IL or missing references)
			//IL_0578: Unknown result type (might be due to invalid IL or missing references)
			//IL_059d: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_060c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0611: Unknown result type (might be due to invalid IL or missing references)
			//IL_061c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0623: Unknown result type (might be due to invalid IL or missing references)
			//IL_0636: Unknown result type (might be due to invalid IL or missing references)
			//IL_0648: Unknown result type (might be due to invalid IL or missing references)
			//IL_065d: Unknown result type (might be due to invalid IL or missing references)
			//IL_066b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0670: Unknown result type (might be due to invalid IL or missing references)
			//IL_067b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0682: Unknown result type (might be due to invalid IL or missing references)
			//IL_0695: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0708: Unknown result type (might be due to invalid IL or missing references)
			//IL_071d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0725: Unknown result type (might be due to invalid IL or missing references)
			//IL_072d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0732: Unknown result type (might be due to invalid IL or missing references)
			//IL_073d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0744: Unknown result type (might be due to invalid IL or missing references)
			//IL_0757: Unknown result type (might be due to invalid IL or missing references)
			//IL_0769: Unknown result type (might be due to invalid IL or missing references)
			//IL_077e: Unknown result type (might be due to invalid IL or missing references)
			//IL_078c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0791: Unknown result type (might be due to invalid IL or missing references)
			//IL_079c: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0587: Unknown result type (might be due to invalid IL or missing references)
			//IL_058f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0594: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e9e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ea3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eb5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ec8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eda: Unknown result type (might be due to invalid IL or missing references)
			//IL_098d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0996: Unknown result type (might be due to invalid IL or missing references)
			//IL_099b: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_09cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_09df: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a02: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a21: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a34: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a46: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a5b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a64: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a6f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a74: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a86: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a99: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ace: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0adb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b00: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b12: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b29: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b37: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b3f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b44: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b4f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b56: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b69: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b7b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b90: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b9e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bbd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c0b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c13: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c18: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c23: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c2a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c3d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c4f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c76: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0800: Unknown result type (might be due to invalid IL or missing references)
			//IL_0808: Unknown result type (might be due to invalid IL or missing references)
			//IL_0810: Unknown result type (might be due to invalid IL or missing references)
			//IL_0815: Unknown result type (might be due to invalid IL or missing references)
			//IL_0820: Unknown result type (might be due to invalid IL or missing references)
			//IL_0827: Unknown result type (might be due to invalid IL or missing references)
			//IL_083a: Unknown result type (might be due to invalid IL or missing references)
			//IL_084c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0863: Unknown result type (might be due to invalid IL or missing references)
			//IL_0865: Unknown result type (might be due to invalid IL or missing references)
			//IL_086d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0875: Unknown result type (might be due to invalid IL or missing references)
			//IL_087a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0885: Unknown result type (might be due to invalid IL or missing references)
			//IL_088c: Unknown result type (might be due to invalid IL or missing references)
			//IL_089f: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08da: Unknown result type (might be due to invalid IL or missing references)
			//IL_08df: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0904: Unknown result type (might be due to invalid IL or missing references)
			//IL_0916: Unknown result type (might be due to invalid IL or missing references)
			//IL_092d: Unknown result type (might be due to invalid IL or missing references)
			//IL_093c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0941: Unknown result type (might be due to invalid IL or missing references)
			//IL_094c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0953: Unknown result type (might be due to invalid IL or missing references)
			//IL_0966: Unknown result type (might be due to invalid IL or missing references)
			//IL_0978: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f30: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f42: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f67: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f6c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f77: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f7e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f91: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fa3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fb8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fc6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fcb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fd6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fdd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ff0: Unknown result type (might be due to invalid IL or missing references)
			//IL_1002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0efb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f06: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f0b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dbc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0de9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0df9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e00: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e13: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e25: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c88: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c9a: Unknown result type (might be due to invalid IL or missing references)
			//IL_105d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1065: Unknown result type (might be due to invalid IL or missing references)
			//IL_106d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1075: Unknown result type (might be due to invalid IL or missing references)
			//IL_107d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1085: Unknown result type (might be due to invalid IL or missing references)
			//IL_108d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1095: Unknown result type (might be due to invalid IL or missing references)
			//IL_109d: Unknown result type (might be due to invalid IL or missing references)
			//IL_10a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_10ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_10b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_10bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_10c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_10cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_10d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_10dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_10e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_10ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_10f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_10fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_1105: Unknown result type (might be due to invalid IL or missing references)
			//IL_1116: Unknown result type (might be due to invalid IL or missing references)
			//IL_111e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1126: Unknown result type (might be due to invalid IL or missing references)
			//IL_112e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1136: Unknown result type (might be due to invalid IL or missing references)
			//IL_113e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1146: Unknown result type (might be due to invalid IL or missing references)
			//IL_114e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1156: Unknown result type (might be due to invalid IL or missing references)
			//IL_115e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1166: Unknown result type (might be due to invalid IL or missing references)
			//IL_116e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1176: Unknown result type (might be due to invalid IL or missing references)
			//IL_117e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1198: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cac: Unknown result type (might be due to invalid IL or missing references)
			//IL_11e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_11a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_11ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_11cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cbe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ccd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cdc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cfc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d3e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d45: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d58: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d6a: Unknown result type (might be due to invalid IL or missing references)
			//IL_11ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_11f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_11f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_11fa: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Citizen> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Citizen>(ref m_CitizenType);
			NativeArray<HouseholdMember> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HouseholdMember>(ref m_HouseholdMemberType);
			NativeArray<CrimeVictim> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CrimeVictim>(ref m_CrimeVictimType);
			NativeArray<Criminal> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Criminal>(ref m_CriminalType);
			NativeArray<Game.Citizens.Student> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Citizens.Student>(ref m_StudentType);
			NativeArray<CurrentBuilding> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentBuilding>(ref m_CurrentBuildingType);
			NativeArray<HealthProblem> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HealthProblem>(ref m_HealthProblemType);
			EnabledMask enabledMask = ((ArchetypeChunk)(ref chunk)).GetEnabledMask<CrimeVictim>(ref m_CrimeVictimType);
			DynamicBuffer<CityModifier> cityModifiers = m_CityModifiers[m_City];
			DynamicBuffer<ServiceFee> fees = m_ServiceFees[m_City];
			float relativeFee = ServiceFeeSystem.GetFee(PlayerResource.Electricity, fees) / m_FeeParameters.m_ElectricityFee.m_Default;
			float relativeFee2 = ServiceFeeSystem.GetFee(PlayerResource.Water, fees) / m_FeeParameters.m_WaterFee.m_Default;
			int4 value = default(int4);
			int4 value2 = default(int4);
			int4 value3 = default(int4);
			int4 value4 = default(int4);
			int4 value5 = default(int4);
			int4 value6 = default(int4);
			int4 value7 = default(int4);
			int4 value8 = default(int4);
			int4 value9 = default(int4);
			int4 value10 = default(int4);
			int4 value11 = default(int4);
			int4 value12 = default(int4);
			int4 value13 = default(int4);
			int4 value14 = default(int4);
			int4 value15 = default(int4);
			int4 value16 = default(int4);
			int4 value17 = default(int4);
			int4 value18 = default(int4);
			int4 value19 = default(int4);
			int4 value20 = default(int4);
			int4 val = default(int4);
			int4 value21 = default(int4);
			int4 value22 = default(int4);
			int4 value23 = default(int4);
			int4 value24 = default(int4);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			int num = 0;
			int num2 = 0;
			HealthProblem healthProblem = default(HealthProblem);
			Criminal criminal = default(Criminal);
			CurrentBuilding currentBuilding = default(CurrentBuilding);
			Game.Buildings.Prison prison = default(Game.Buildings.Prison);
			Game.Citizens.Student student = default(Game.Citizens.Student);
			Game.Buildings.School school = default(Game.Buildings.School);
			int2 val6 = default(int2);
			PropertyRenter propertyRenter = default(PropertyRenter);
			PrefabRef prefabRef = default(PrefabRef);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				_ = nativeArray[i];
				Entity household = nativeArray3[i].m_Household;
				if (!m_Resources.HasBuffer(household))
				{
					return;
				}
				Citizen citizen = nativeArray2[i];
				if ((CollectionUtils.TryGet<HealthProblem>(nativeArray8, i, ref healthProblem) && CitizenUtils.IsDead(healthProblem)) || ((m_Households[household].m_Flags & HouseholdFlags.MovedIn) == 0 && (citizen.m_State & CitizenFlags.Tourist) == 0))
				{
					continue;
				}
				Entity val2 = Entity.Null;
				Entity val3 = Entity.Null;
				if (m_Properties.HasComponent(household))
				{
					val2 = m_Properties[household].m_Property;
					if (m_CurrentDistrictData.HasComponent(val2))
					{
						val3 = m_CurrentDistrictData[val2].m_District;
					}
				}
				DynamicBuffer<HouseholdCitizen> householdCitizens = m_HouseholdCitizens[household];
				int num3 = 0;
				for (int j = 0; j < householdCitizens.Length; j++)
				{
					if (citizen.GetAge() == CitizenAge.Child)
					{
						num3++;
					}
				}
				int householdTotalWealth = EconomyUtils.GetHouseholdTotalWealth(m_Households[household], m_Resources[household]);
				int2 val4 = ((householdTotalWealth > 0) ? new int2(0, math.min(15, householdTotalWealth / 1000)) : default(int2));
				value21.x += val4.x + val4.y;
				value21.y++;
				value21.z += val4.x;
				value21.w += val4.y;
				int2 val5 = int2.op_Implicit(0);
				if (CollectionUtils.TryGet<Criminal>(nativeArray5, i, ref criminal) && (criminal.m_Flags & CriminalFlags.Prisoner) != 0 && CollectionUtils.TryGet<CurrentBuilding>(nativeArray7, i, ref currentBuilding) && m_Prisons.TryGetComponent(currentBuilding.m_CurrentBuilding, ref prison))
				{
					val5 += new int2((int)prison.m_PrisonerHealth, (int)prison.m_PrisonerWellbeing);
				}
				if (CollectionUtils.TryGet<Game.Citizens.Student>(nativeArray6, i, ref student) && m_Schools.TryGetComponent(student.m_School, ref school))
				{
					val5 += new int2((int)school.m_StudentHealth, (int)school.m_StudentWellbeing);
				}
				val += new int4(val5.x + val5.y, 1, val5.x, val5.y);
				((int2)(ref val6))._002Ector(0, 0);
				int2 val7 = default(int2);
				int2 val8 = default(int2);
				int2 val9 = default(int2);
				int2 val10 = default(int2);
				int2 val11 = default(int2);
				int2 val12 = default(int2);
				int2 val13 = default(int2);
				int2 val14 = default(int2);
				int2 val15 = default(int2);
				int2 val16 = default(int2);
				int2 val17 = default(int2);
				int2 val18 = default(int2);
				int2 val19 = default(int2);
				int2 val20 = default(int2);
				int2 val21 = default(int2);
				int2 val22 = default(int2);
				int2 val23 = default(int2);
				int2 val24 = default(int2);
				int2 val25 = default(int2);
				CrimeVictim crimeVictim = default(CrimeVictim);
				if (((EnabledMask)(ref enabledMask))[i])
				{
					crimeVictim = nativeArray4[i];
				}
				if (m_Properties.TryGetComponent(household, ref propertyRenter) && m_Prefabs.TryGetComponent(propertyRenter.m_Property, ref prefabRef))
				{
					Entity prefab = prefabRef.m_Prefab;
					Entity property = propertyRenter.m_Property;
					Entity healthcareServicePrefab = m_HealthcareParameters.m_HealthcareServicePrefab;
					Entity parkServicePrefab = m_ParkParameters.m_ParkServicePrefab;
					Entity educationServicePrefab = m_EducationParameters.m_EducationServicePrefab;
					Entity telecomServicePrefab = m_TelecomParameters.m_TelecomServicePrefab;
					Entity garbageServicePrefab = m_GarbageParameters.m_GarbageServicePrefab;
					Entity policeServicePrefab = m_PoliceParameters.m_PoliceServicePrefab;
					Entity val26 = Entity.Null;
					float curvePosition = 0f;
					if (m_Buildings.HasComponent(property))
					{
						Building building = m_Buildings[property];
						val26 = building.m_RoadEdge;
						curvePosition = building.m_CurvePosition;
					}
					val7 = GetElectricitySupplyBonuses(property, ref m_ElectricityConsumers, in m_CitizenHappinessParameters);
					value5.x += val7.x + val7.y;
					value5.z += val7.x;
					value5.w += val7.y;
					value5.y++;
					val8 = GetElectricityFeeBonuses(property, ref m_ElectricityConsumers, relativeFee, in m_CitizenHappinessParameters);
					value6.x += val8.x + val8.y;
					value6.z += val8.x;
					value6.w += val8.y;
					value6.y++;
					val13 = GetWaterSupplyBonuses(property, ref m_WaterConsumers, in m_CitizenHappinessParameters);
					value10.x += val13.x + val13.y;
					value10.z += val13.x;
					value10.w += val13.y;
					value10.y++;
					val14 = GetWaterFeeBonuses(property, ref m_WaterConsumers, relativeFee2, in m_CitizenHappinessParameters);
					value11.x += val14.x + val14.y;
					value11.z += val14.x;
					value11.w += val14.y;
					value11.y++;
					val15 = GetWaterPollutionBonuses(property, ref m_WaterConsumers, cityModifiers, in m_CitizenHappinessParameters);
					value12.x += val15.x + val15.y;
					value12.z += val15.x;
					value12.w += val15.y;
					value12.y++;
					val16 = GetSewageBonuses(property, ref m_WaterConsumers, in m_CitizenHappinessParameters);
					value13.x += val16.x + val16.y;
					value13.z += val16.x;
					value13.w += val16.y;
					value13.y++;
					if (m_ServiceCoverages.HasBuffer(val26))
					{
						DynamicBuffer<Game.Net.ServiceCoverage> serviceCoverage = m_ServiceCoverages[val26];
						val9 = GetHealthcareBonuses(curvePosition, serviceCoverage, ref m_Locked, healthcareServicePrefab, in m_CitizenHappinessParameters);
						value7.x += val9.x + val9.y;
						value7.z += val9.x;
						value7.w += val9.y;
						value7.y++;
						val19 = GetEntertainmentBonuses(curvePosition, serviceCoverage, cityModifiers, ref m_Locked, parkServicePrefab, in m_CitizenHappinessParameters);
						value15.x += val19.x + val19.y;
						value15.z += val19.x;
						value15.w += val19.y;
						value15.y++;
						val20 = GetEducationBonuses(curvePosition, serviceCoverage, ref m_Locked, educationServicePrefab, in m_CitizenHappinessParameters, num3);
						value16.x += val20.x + val20.y;
						value16.z += val20.x;
						value16.w += val20.y;
						value16.y++;
						val23 = GetWellfareBonuses(curvePosition, serviceCoverage, in m_CitizenHappinessParameters, citizen.Happiness);
						value18.x += val23.x + val23.y;
						value18.z += val23.x;
						value18.w += val23.y;
						value18.y++;
					}
					val10 = GetGroundPollutionBonuses(property, ref m_Transforms, m_PollutionMap, cityModifiers, in m_CitizenHappinessParameters);
					value8.x += val10.x + val10.y;
					value8.z += val10.x;
					value8.w += val10.y;
					value8.y++;
					val11 = GetAirPollutionBonuses(property, ref m_Transforms, m_AirPollutionMap, cityModifiers, in m_CitizenHappinessParameters);
					value3.x += val11.x + val11.y;
					value3.z += val11.x;
					value3.w += val11.y;
					value3.y++;
					val12 = GetNoiseBonuses(property, ref m_Transforms, m_NoisePollutionMap, in m_CitizenHappinessParameters);
					value9.x += val12.x + val12.y;
					value9.z += val12.x;
					value9.w += val12.y;
					value9.y++;
					val17 = GetGarbageBonuses(property, ref m_Garbages, ref m_Locked, garbageServicePrefab, in m_GarbageParameters);
					value14.x += val17.x + val17.y;
					value14.z += val17.x;
					value14.w += val17.y;
					value14.y++;
					val18 = GetCrimeBonuses(crimeVictim, property, ref m_CrimeProducers, ref m_Locked, policeServicePrefab, in m_CitizenHappinessParameters);
					value.x += val18.x + val18.y;
					value.z += val18.x;
					value.w += val18.y;
					value.y++;
					val21 = GetMailBonuses(property, ref m_MailProducers, ref m_Locked, telecomServicePrefab, in m_CitizenHappinessParameters);
					value17.x += val21.x + val21.y;
					value17.z += val21.x;
					value17.w += val21.y;
					value17.y++;
					val22 = GetTelecomBonuses(property, ref m_Transforms, m_TelecomCoverage, ref m_Locked, telecomServicePrefab, in m_CitizenHappinessParameters);
					value2.x += val22.x + val22.y;
					value2.z += val22.x;
					value2.w += val22.y;
					value2.y++;
					value24.y++;
					if (m_SpawnableBuildings.HasComponent(prefab) && m_BuildingDatas.HasComponent(prefab) && m_BuildingPropertyDatas.HasComponent(prefab) && !m_HomelessHouseholds.HasComponent(household))
					{
						SpawnableBuildingData spawnableBuildingData = m_SpawnableBuildings[prefab];
						BuildingData buildingData = m_BuildingDatas[prefab];
						BuildingPropertyData buildingPropertyData = m_BuildingPropertyDatas[prefab];
						float num4 = buildingPropertyData.m_SpaceMultiplier * (float)buildingData.m_LotSize.x * (float)buildingData.m_LotSize.y / (float)(householdCitizens.Length * buildingPropertyData.m_ResidentialProperties);
						val6.y = Mathf.RoundToInt(GetApartmentWellbeing(num4, spawnableBuildingData.m_Level));
						value4.x += val6.x + val6.y;
						value4.z += val6.x;
						value4.w += val6.y;
						value4.y++;
						AddData(math.min(100f, 100f * num4));
					}
					else
					{
						val6.y = Mathf.RoundToInt(GetApartmentWellbeing(0.01f, 1));
						value4.x += val6.y;
						value4.w += val6.y;
						value4.y++;
						val25 = GetHomelessBonuses(in m_CitizenHappinessParameters);
						value24.x += val25.x + val25.y;
						value24.z += val25.x;
						value24.w += val25.y;
					}
				}
				bool flag = (citizen.m_State & CitizenFlags.Tourist) != 0;
				if (((Random)(ref random)).NextFloat() < 0.02f * (flag ? 10f : 1f))
				{
					citizen.m_LeisureCounter = (byte)math.min(255, math.max(0, citizen.m_LeisureCounter - 1));
				}
				citizen.m_PenaltyCounter = (byte)math.max(0, citizen.m_PenaltyCounter - 1);
				int2 leisureBonuses = GetLeisureBonuses(citizen.m_LeisureCounter);
				value19.x += leisureBonuses.x + leisureBonuses.y;
				value19.z += leisureBonuses.x;
				value19.w += leisureBonuses.y;
				value19.y++;
				if (!flag)
				{
					val24 = GetTaxBonuses(citizen.GetEducationLevel(), m_TaxRates, in m_CitizenHappinessParameters);
				}
				value20.x += val24.x + val24.y;
				value20.z += val24.x;
				value20.w += val24.y;
				value20.y++;
				int2 sicknessBonuses = GetSicknessBonuses(nativeArray8.Length != 0, in m_CitizenHappinessParameters);
				value7.x += sicknessBonuses.x + sicknessBonuses.y;
				value7.z += sicknessBonuses.x;
				value7.w += sicknessBonuses.y;
				value7.y++;
				int2 deathPenalty = GetDeathPenalty(householdCitizens, ref m_HealthProblems, in m_CitizenHappinessParameters);
				value23.x += deathPenalty.x + deathPenalty.y;
				value23.z += deathPenalty.x;
				value23.w += deathPenalty.y;
				value23.y++;
				int num5 = ((citizen.m_PenaltyCounter > 0) ? m_CitizenHappinessParameters.m_PenaltyEffect : 0);
				value22.x += num5;
				value22.w += num5;
				value22.y++;
				int num6 = math.max(0, 50 + num5 + deathPenalty.y + val4.y + val7.y + val8.y + val13.y + val14.y + val16.y + val9.y + leisureBonuses.y + val5.y + val15.y + val12.y + val17.y + val18.y + val19.y + val21.y + val20.y + val22.y + val6.y + val23.y + val24.y + val25.y);
				int num7 = 50 + val9.x + sicknessBonuses.x + deathPenalty.x + val5.x + val10.x + val11.x + val7.x + val13.x + val16.x + val15.x + val17.x + val6.x + val23.x + val25.x;
				float value25 = num6;
				float value26 = num7;
				if (m_Transforms.HasComponent(val2))
				{
					Transform transform = m_Transforms[val2];
					m_LocalEffectData.ApplyModifier(ref value25, transform.m_Position, LocalModifierType.Wellbeing);
					m_LocalEffectData.ApplyModifier(ref value26, transform.m_Position, LocalModifierType.Health);
				}
				if (m_DistrictModifiers.HasBuffer(val3))
				{
					DynamicBuffer<DistrictModifier> modifiers = m_DistrictModifiers[val3];
					AreaUtils.ApplyModifier(ref value25, modifiers, DistrictModifierType.Wellbeing);
				}
				num6 = Mathf.RoundToInt(value25);
				num7 = Mathf.RoundToInt(value26);
				int num8 = ((((Random)(ref random)).NextInt(100) > 50 + citizen.m_WellBeing - num6) ? 1 : (-1));
				citizen.m_WellBeing = (byte)math.max(0, math.min(100, citizen.m_WellBeing + num8));
				num8 = ((((Random)(ref random)).NextInt(100) > 50 + citizen.m_Health - num7) ? 1 : (-1));
				int maxHealth = GetMaxHealth(citizen.GetAgeInDays(m_SimulationFrame, m_TimeData) / (float)m_TimeSettings.m_DaysPerYear);
				citizen.m_Health = (byte)math.max(0, math.min(maxHealth, citizen.m_Health + num8));
				if (citizen.m_WellBeing < m_CitizenHappinessParameters.m_LowWellbeing)
				{
					num++;
				}
				if (citizen.m_Health < m_CitizenHappinessParameters.m_LowHealth)
				{
					num2++;
				}
				nativeArray2[i] = citizen;
			}
			m_FactorQueue.Enqueue(new FactorItem
			{
				m_Factor = HappinessFactor.Telecom,
				m_UpdateFrame = m_RawUpdateFrame,
				m_Value = value2
			});
			m_FactorQueue.Enqueue(new FactorItem
			{
				m_Factor = HappinessFactor.Crime,
				m_UpdateFrame = m_RawUpdateFrame,
				m_Value = value
			});
			m_FactorQueue.Enqueue(new FactorItem
			{
				m_Factor = HappinessFactor.AirPollution,
				m_UpdateFrame = m_RawUpdateFrame,
				m_Value = value3
			});
			m_FactorQueue.Enqueue(new FactorItem
			{
				m_Factor = HappinessFactor.Apartment,
				m_UpdateFrame = m_RawUpdateFrame,
				m_Value = value4
			});
			m_FactorQueue.Enqueue(new FactorItem
			{
				m_Factor = HappinessFactor.Electricity,
				m_UpdateFrame = m_RawUpdateFrame,
				m_Value = value5
			});
			m_FactorQueue.Enqueue(new FactorItem
			{
				m_Factor = HappinessFactor.ElectricityFee,
				m_UpdateFrame = m_RawUpdateFrame,
				m_Value = value6
			});
			m_FactorQueue.Enqueue(new FactorItem
			{
				m_Factor = HappinessFactor.Healthcare,
				m_UpdateFrame = m_RawUpdateFrame,
				m_Value = value7
			});
			m_FactorQueue.Enqueue(new FactorItem
			{
				m_Factor = HappinessFactor.GroundPollution,
				m_UpdateFrame = m_RawUpdateFrame,
				m_Value = value8
			});
			m_FactorQueue.Enqueue(new FactorItem
			{
				m_Factor = HappinessFactor.NoisePollution,
				m_UpdateFrame = m_RawUpdateFrame,
				m_Value = value9
			});
			m_FactorQueue.Enqueue(new FactorItem
			{
				m_Factor = HappinessFactor.Water,
				m_UpdateFrame = m_RawUpdateFrame,
				m_Value = value10
			});
			m_FactorQueue.Enqueue(new FactorItem
			{
				m_Factor = HappinessFactor.WaterFee,
				m_UpdateFrame = m_RawUpdateFrame,
				m_Value = value11
			});
			m_FactorQueue.Enqueue(new FactorItem
			{
				m_Factor = HappinessFactor.WaterPollution,
				m_UpdateFrame = m_RawUpdateFrame,
				m_Value = value12
			});
			m_FactorQueue.Enqueue(new FactorItem
			{
				m_Factor = HappinessFactor.Sewage,
				m_UpdateFrame = m_RawUpdateFrame,
				m_Value = value13
			});
			m_FactorQueue.Enqueue(new FactorItem
			{
				m_Factor = HappinessFactor.Garbage,
				m_UpdateFrame = m_RawUpdateFrame,
				m_Value = value14
			});
			m_FactorQueue.Enqueue(new FactorItem
			{
				m_Factor = HappinessFactor.Entertainment,
				m_UpdateFrame = m_RawUpdateFrame,
				m_Value = value15
			});
			m_FactorQueue.Enqueue(new FactorItem
			{
				m_Factor = HappinessFactor.Education,
				m_UpdateFrame = m_RawUpdateFrame,
				m_Value = value16
			});
			m_FactorQueue.Enqueue(new FactorItem
			{
				m_Factor = HappinessFactor.Mail,
				m_UpdateFrame = m_RawUpdateFrame,
				m_Value = value17
			});
			m_FactorQueue.Enqueue(new FactorItem
			{
				m_Factor = HappinessFactor.Welfare,
				m_UpdateFrame = m_RawUpdateFrame,
				m_Value = value18
			});
			m_FactorQueue.Enqueue(new FactorItem
			{
				m_Factor = HappinessFactor.Leisure,
				m_UpdateFrame = m_RawUpdateFrame,
				m_Value = value19
			});
			m_FactorQueue.Enqueue(new FactorItem
			{
				m_Factor = HappinessFactor.Tax,
				m_UpdateFrame = m_RawUpdateFrame,
				m_Value = value20
			});
			m_FactorQueue.Enqueue(new FactorItem
			{
				m_Factor = HappinessFactor.Buildings,
				m_UpdateFrame = m_RawUpdateFrame,
				m_Value = val
			});
			m_FactorQueue.Enqueue(new FactorItem
			{
				m_Factor = HappinessFactor.Consumption,
				m_UpdateFrame = m_RawUpdateFrame,
				m_Value = value21
			});
			m_FactorQueue.Enqueue(new FactorItem
			{
				m_Factor = HappinessFactor.TrafficPenalty,
				m_UpdateFrame = m_RawUpdateFrame,
				m_Value = value22
			});
			m_FactorQueue.Enqueue(new FactorItem
			{
				m_Factor = HappinessFactor.DeathPenalty,
				m_UpdateFrame = m_RawUpdateFrame,
				m_Value = value23
			});
			m_FactorQueue.Enqueue(new FactorItem
			{
				m_Factor = HappinessFactor.Homelessness,
				m_UpdateFrame = m_RawUpdateFrame,
				m_Value = value24
			});
			m_StatisticsEventQueue.Enqueue(new StatisticsEvent
			{
				m_Statistic = StatisticType.WellbeingLevel,
				m_Change = num
			});
			m_StatisticsEventQueue.Enqueue(new StatisticsEvent
			{
				m_Statistic = StatisticType.HealthLevel,
				m_Change = num2
			});
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct HappinessFactorJob : IJob
	{
		public NativeArray<int4> m_HappinessFactors;

		public NativeQueue<FactorItem> m_FactorQueue;

		public NativeQueue<TriggerAction> m_TriggerActionQueue;

		public uint m_RawUpdateFrame;

		public Entity m_ParameterEntity;

		[ReadOnly]
		public BufferLookup<HappinessFactorParameterData> m_Parameters;

		[ReadOnly]
		public ComponentLookup<Locked> m_Locked;

		public void Execute()
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < 25; i++)
			{
				m_HappinessFactors[GetFactorIndex((HappinessFactor)i, m_RawUpdateFrame)] = default(int4);
			}
			FactorItem factorItem = default(FactorItem);
			while (m_FactorQueue.TryDequeue(ref factorItem))
			{
				if (factorItem.m_UpdateFrame != m_RawUpdateFrame)
				{
					Debug.LogWarning((object)"Different updateframe in HappinessFactorJob than in its queue");
				}
				ref NativeArray<int4> reference = ref m_HappinessFactors;
				int factorIndex = GetFactorIndex(factorItem.m_Factor, factorItem.m_UpdateFrame);
				reference[factorIndex] += factorItem.m_Value;
			}
			DynamicBuffer<HappinessFactorParameterData> parameters = m_Parameters[m_ParameterEntity];
			for (int j = 0; j < 25; j++)
			{
				m_TriggerActionQueue.Enqueue(new TriggerAction(GetTriggerTypeForHappinessFactor((HappinessFactor)j), Entity.Null, GetHappinessFactor((HappinessFactor)j, m_HappinessFactors, parameters, ref m_Locked).x));
			}
		}
	}

	private struct TypeHandle
	{
		public ComponentTypeHandle<Citizen> __Game_Citizens_Citizen_RW_ComponentTypeHandle;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<HouseholdMember> __Game_Citizens_HouseholdMember_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CrimeVictim> __Game_Citizens_CrimeVictim_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Criminal> __Game_Citizens_Criminal_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Citizens.Student> __Game_Citizens_Student_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentBuilding> __Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<HealthProblem> __Game_Citizens_HealthProblem_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Household> __Game_Citizens_Household_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HealthProblem> __Game_Citizens_HealthProblem_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityConsumer> __Game_Buildings_ElectricityConsumer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Resources> __Game_Economy_Resources_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.ServiceCoverage> __Game_Net_ServiceCoverage_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentDistrict> __Game_Areas_CurrentDistrict_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WaterConsumer> __Game_Buildings_WaterConsumer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<GarbageProducer> __Game_Buildings_GarbageProducer_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Locked> __Game_Prefabs_Locked_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CrimeProducer> __Game_Buildings_CrimeProducer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MailProducer> __Game_Buildings_MailProducer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> __Game_Prefabs_BuildingPropertyData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<DistrictModifier> __Game_Areas_DistrictModifier_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<CityModifier> __Game_City_CityModifier_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ServiceFee> __Game_City_ServiceFee_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Prison> __Game_Buildings_Prison_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.School> __Game_Buildings_School_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HomelessHousehold> __Game_Citizens_HomelessHousehold_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<HappinessFactorParameterData> __Game_Prefabs_HappinessFactorParameterData_RO_BufferLookup;

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
			__Game_Citizens_Citizen_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Citizen>(false);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Citizens_HouseholdMember_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HouseholdMember>(true);
			__Game_Citizens_CrimeVictim_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CrimeVictim>(true);
			__Game_Citizens_Criminal_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Criminal>(true);
			__Game_Citizens_Student_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Citizens.Student>(true);
			__Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentBuilding>(true);
			__Game_Citizens_HealthProblem_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HealthProblem>(true);
			__Game_Citizens_Household_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Household>(true);
			__Game_Citizens_HealthProblem_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HealthProblem>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Buildings_ElectricityConsumer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityConsumer>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
			__Game_Economy_Resources_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Resources>(true);
			__Game_Net_ServiceCoverage_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.ServiceCoverage>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Areas_CurrentDistrict_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentDistrict>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Buildings_WaterConsumer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterConsumer>(true);
			__Game_Buildings_GarbageProducer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarbageProducer>(true);
			__Game_Citizens_HouseholdCitizen_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdCitizen>(true);
			__Game_Prefabs_Locked_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Locked>(true);
			__Game_Buildings_CrimeProducer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CrimeProducer>(true);
			__Game_Buildings_MailProducer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MailProducer>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingPropertyData>(true);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableBuildingData>(true);
			__Game_Areas_DistrictModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<DistrictModifier>(true);
			__Game_City_CityModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(true);
			__Game_City_ServiceFee_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceFee>(true);
			__Game_Buildings_Prison_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.Prison>(true);
			__Game_Buildings_School_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.School>(true);
			__Game_Citizens_HomelessHousehold_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HomelessHousehold>(true);
			__Game_Prefabs_HappinessFactorParameterData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HappinessFactorParameterData>(true);
		}
	}

	[DebugWatchValue]
	private DebugWatchDistribution m_DebugData;

	private NativeQueue<FactorItem> m_FactorQueue;

	private EntityQuery m_CitizenQuery;

	private EntityQuery m_HappinessFactorParameterQuery;

	private SimulationSystem m_SimulationSystem;

	private GroundPollutionSystem m_GroundPollutionSystem;

	private AirPollutionSystem m_AirPollutionSystem;

	private NoisePollutionSystem m_NoisePollutionSystem;

	private TelecomCoverageSystem m_TelecomCoverageSystem;

	private LocalEffectSystem m_LocalEffectSystem;

	private CitySystem m_CitySystem;

	private TriggerSystem m_TriggerSystem;

	private TaxSystem m_TaxSystem;

	private CityStatisticsSystem m_CityStatisticsSystem;

	private EntityQuery m_HealthcareParameterQuery;

	private EntityQuery m_ParkParameterQuery;

	private EntityQuery m_EducationParameterQuery;

	private EntityQuery m_TelecomParameterQuery;

	private EntityQuery m_GarbageParameterQuery;

	private EntityQuery m_PoliceParameterQuery;

	private EntityQuery m_CitizenHappinessParameterQuery;

	private EntityQuery m_TimeSettingQuery;

	private EntityQuery m_TimeDataQuery;

	private NativeArray<int4> m_HappinessFactors;

	private JobHandle m_LastDeps;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_429327288_0;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	private static int GetFactorIndex(HappinessFactor factor, uint updateFrame)
	{
		return (int)factor + (int)(25 * updateFrame);
	}

	public float3 GetHappinessFactor(HappinessFactor factor, DynamicBuffer<HappinessFactorParameterData> parameters, ref ComponentLookup<Locked> locked)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_LastDeps)).Complete();
		return GetHappinessFactor(factor, m_HappinessFactors, parameters, ref locked);
	}

	private static float3 GetHappinessFactor(HappinessFactor factor, NativeArray<int4> happinessFactors, DynamicBuffer<HappinessFactorParameterData> parameters, ref ComponentLookup<Locked> locked)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		int4 val = int4.op_Implicit(0);
		for (uint num = 0u; num < 16; num++)
		{
			val += happinessFactors[GetFactorIndex(factor, num)];
		}
		Entity lockedEntity = parameters[(int)factor].m_LockedEntity;
		if (lockedEntity != Entity.Null && EntitiesExtensions.HasEnabledComponent<Locked>(locked, lockedEntity))
		{
			return float3.op_Implicit(0);
		}
		return ((val.y > 0) ? new float3((float)val.x / (2f * (float)val.y), (float)(val.z / val.y), (float)(val.w / val.y)) : default(float3)) - (float)parameters[(int)factor].m_BaseLevel;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Expected O, but got Unknown
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_GroundPollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GroundPollutionSystem>();
		m_AirPollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AirPollutionSystem>();
		m_NoisePollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NoisePollutionSystem>();
		m_TelecomCoverageSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TelecomCoverageSystem>();
		m_LocalEffectSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LocalEffectSystem>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_TriggerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TriggerSystem>();
		m_TaxSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TaxSystem>();
		m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
		m_HappinessFactors = new NativeArray<int4>(400, (Allocator)4, (NativeArrayOptions)1);
		m_FactorQueue = new NativeQueue<FactorItem>(AllocatorHandle.op_Implicit((Allocator)4));
		m_HealthcareParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<HealthcareParameterData>() });
		m_ParkParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ParkParameterData>() });
		m_EducationParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EducationParameterData>() });
		m_TelecomParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TelecomParameterData>() });
		m_GarbageParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<GarbageParameterData>() });
		m_PoliceParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PoliceConfigurationData>() });
		m_CitizenHappinessParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<CitizenHappinessParameterData>() });
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<Citizen>(),
			ComponentType.ReadOnly<HouseholdMember>(),
			ComponentType.ReadOnly<UpdateFrame>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_CitizenQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_TimeSettingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TimeSettingsData>() });
		m_TimeDataQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TimeData>() });
		m_HappinessFactorParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<HappinessFactorParameterData>() });
		m_DebugData = new DebugWatchDistribution();
		((ComponentSystemBase)this).RequireForUpdate(m_CitizenQuery);
		((ComponentSystemBase)this).RequireForUpdate<ServiceFeeParameterData>();
	}

	public static float GetApartmentWellbeing(float sizePerResident, int level)
	{
		return 0.8f * (4f * (float)(level - 1) + (24.55531f + -70.21f / math.pow(1f + math.pow(sizePerResident / 0.03690514f, 25.2376f), 0.01494523f)));
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_DebugData.Dispose();
		m_HappinessFactors.Dispose();
		m_FactorQueue.Dispose();
		base.OnDestroy();
	}

	public static float GetFreetimeWellbeingDifferential(int freetime)
	{
		return 4f / (float)freetime;
	}

	public static float GetFreetimeWellbeing(int freetime)
	{
		return 4f * math.log((float)math.max(1, freetime)) - 25f;
	}

	public static int GetElectricityFeeHappinessEffect(float relativeFee, in CitizenHappinessParameterData data)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return (int)math.round((float)math.csum(GetElectricityFeeBonuses(relativeFee, in data)) / 2f);
	}

	public static int2 GetElectricityFeeBonuses(Entity building, ref ComponentLookup<ElectricityConsumer> electricityConsumers, float relativeFee, in CitizenHappinessParameterData data)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		ElectricityConsumer electricityConsumer = default(ElectricityConsumer);
		if (electricityConsumers.TryGetComponent(building, ref electricityConsumer) && electricityConsumer.m_WantedConsumption > 0)
		{
			return GetElectricityFeeBonuses(relativeFee, in data);
		}
		return default(int2);
	}

	public static int2 GetElectricityFeeBonuses(float relativeFee, in CitizenHappinessParameterData data)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		return new int2
		{
			y = (int)math.round(((AnimationCurve1)(ref data.m_ElectricityFeeWellbeingEffect)).Evaluate(relativeFee))
		};
	}

	public static int2 GetElectricitySupplyBonuses(Entity building, ref ComponentLookup<ElectricityConsumer> electricityConsumers, in CitizenHappinessParameterData data)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		ElectricityConsumer electricityConsumer = default(ElectricityConsumer);
		if (electricityConsumers.TryGetComponent(building, ref electricityConsumer))
		{
			float num = math.saturate((float)electricityConsumer.m_CooldownCounter / data.m_ElectricityPenaltyDelay);
			return new int2
			{
				y = (int)math.round((0f - data.m_ElectricityWellbeingPenalty) * num)
			};
		}
		return default(int2);
	}

	public static int GetWaterFeeHappinessEffect(float relativeFee, in CitizenHappinessParameterData data)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return (int)math.round((float)math.csum(GetWaterFeeBonuses(relativeFee, in data)) / 2f);
	}

	public static int2 GetWaterFeeBonuses(Entity building, ref ComponentLookup<WaterConsumer> waterConsumers, float relativeFee, in CitizenHappinessParameterData data)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		WaterConsumer waterConsumer = default(WaterConsumer);
		if (waterConsumers.TryGetComponent(building, ref waterConsumer) && waterConsumer.m_WantedConsumption > 0)
		{
			return GetWaterFeeBonuses(relativeFee, in data);
		}
		return default(int2);
	}

	public static int2 GetWaterFeeBonuses(float relativeFee, in CitizenHappinessParameterData data)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		return new int2
		{
			x = (int)math.round(((AnimationCurve1)(ref data.m_WaterFeeHealthEffect)).Evaluate(relativeFee)),
			y = (int)math.round(((AnimationCurve1)(ref data.m_WaterFeeWellbeingEffect)).Evaluate(relativeFee))
		};
	}

	public static int2 GetWaterSupplyBonuses(Entity building, ref ComponentLookup<WaterConsumer> waterConsumers, in CitizenHappinessParameterData data)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		WaterConsumer waterConsumer = default(WaterConsumer);
		if (waterConsumers.TryGetComponent(building, ref waterConsumer))
		{
			float num = math.saturate((float)(int)waterConsumer.m_FreshCooldownCounter / data.m_WaterPenaltyDelay);
			return new int2
			{
				x = (int)math.round((float)(-data.m_WaterHealthPenalty) * num),
				y = (int)math.round((float)(-data.m_WaterWellbeingPenalty) * num)
			};
		}
		return default(int2);
	}

	public static int2 GetWaterPollutionBonuses(Entity building, ref ComponentLookup<WaterConsumer> waterConsumers, DynamicBuffer<CityModifier> cityModifiers, in CitizenHappinessParameterData data)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		int2 result = default(int2);
		if (waterConsumers.HasComponent(building))
		{
			WaterConsumer waterConsumer = waterConsumers[building];
			if (waterConsumer.m_Pollution > 0f)
			{
				float value = 1f;
				CityUtils.ApplyModifier(ref value, cityModifiers, CityModifierType.PollutionHealthAffect);
				result.x = Mathf.RoundToInt(value * data.m_WaterPollutionBonusMultiplier * math.min(1f, 10f * waterConsumer.m_Pollution));
			}
		}
		return result;
	}

	public static int2 GetSewageBonuses(Entity building, ref ComponentLookup<WaterConsumer> waterConsumers, in CitizenHappinessParameterData data)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		WaterConsumer waterConsumer = default(WaterConsumer);
		if (waterConsumers.TryGetComponent(building, ref waterConsumer))
		{
			float num = math.saturate((float)(int)waterConsumer.m_SewageCooldownCounter / data.m_SewagePenaltyDelay);
			return new int2
			{
				x = (int)math.round((float)(-data.m_SewageHealthEffect) * num),
				y = (int)math.round((float)(-data.m_SewageWellbeingEffect) * num)
			};
		}
		return default(int2);
	}

	public static int2 GetHealthcareBonuses(float curvePosition, DynamicBuffer<Game.Net.ServiceCoverage> serviceCoverage, ref ComponentLookup<Locked> locked, Entity healthcareService, in CitizenHappinessParameterData data)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (EntitiesExtensions.HasEnabledComponent<Locked>(locked, healthcareService))
		{
			return new int2(0, 0);
		}
		int2 result = default(int2);
		float serviceCoverage2 = NetUtils.GetServiceCoverage(serviceCoverage, CoverageService.Healthcare, curvePosition);
		result.x = Mathf.RoundToInt(data.m_HealthCareHealthMultiplier * serviceCoverage2);
		result.y = Mathf.RoundToInt(data.m_HealthCareWellbeingMultiplier * serviceCoverage2);
		return result;
	}

	public static int2 GetEducationBonuses(float curvePosition, DynamicBuffer<Game.Net.ServiceCoverage> serviceCoverage, ref ComponentLookup<Locked> locked, Entity educationService, in CitizenHappinessParameterData data, int children)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (EntitiesExtensions.HasEnabledComponent<Locked>(locked, educationService))
		{
			return new int2(0, 0);
		}
		int2 result = default(int2);
		float num = math.sqrt((float)children) * data.m_EducationWellbeingMultiplier * (NetUtils.GetServiceCoverage(serviceCoverage, CoverageService.Education, curvePosition) - data.m_NeutralEducation);
		result.y = Mathf.RoundToInt(num);
		return result;
	}

	public static int2 GetEntertainmentBonuses(float curvePosition, DynamicBuffer<Game.Net.ServiceCoverage> serviceCoverage, DynamicBuffer<CityModifier> cityModifiers, ref ComponentLookup<Locked> locked, Entity entertainmentService, in CitizenHappinessParameterData data)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (EntitiesExtensions.HasEnabledComponent<Locked>(locked, entertainmentService))
		{
			return new int2(0, 0);
		}
		int2 result = default(int2);
		float value = NetUtils.GetServiceCoverage(serviceCoverage, CoverageService.Park, curvePosition);
		CityUtils.ApplyModifier(ref value, cityModifiers, CityModifierType.Entertainment);
		value = data.m_EntertainmentWellbeingMultiplier * math.min(1f, math.sqrt(value / 1.5f));
		result.x = 0;
		result.y = Mathf.RoundToInt(value);
		return result;
	}

	public static int2 GetGroundPollutionBonuses(Entity building, ref ComponentLookup<Transform> transforms, NativeArray<GroundPollution> pollutionMap, DynamicBuffer<CityModifier> cityModifiers, in CitizenHappinessParameterData data)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		int2 result = default(int2);
		if (transforms.HasComponent(building))
		{
			short num = (short)(GroundPollutionSystem.GetPollution(transforms[building].m_Position, pollutionMap).m_Pollution / data.m_PollutionBonusDivisor);
			float value = 1f;
			CityUtils.ApplyModifier(ref value, cityModifiers, CityModifierType.PollutionHealthAffect);
			result.x = (int)((float)(-math.min(data.m_MaxAirAndGroundPollutionBonus, (int)num)) * value);
		}
		return result;
	}

	public static int2 GetAirPollutionBonuses(Entity building, ref ComponentLookup<Transform> transforms, NativeArray<AirPollution> airPollutionMap, DynamicBuffer<CityModifier> cityModifiers, in CitizenHappinessParameterData data)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		int2 result = default(int2);
		if (transforms.HasComponent(building))
		{
			short num = (short)(AirPollutionSystem.GetPollution(transforms[building].m_Position, airPollutionMap).m_Pollution / data.m_PollutionBonusDivisor);
			float value = 1f;
			CityUtils.ApplyModifier(ref value, cityModifiers, CityModifierType.PollutionHealthAffect);
			result.x = (int)((float)(-math.min(data.m_MaxAirAndGroundPollutionBonus, (int)num)) * value);
		}
		return result;
	}

	public static int2 GetNoiseBonuses(Entity building, ref ComponentLookup<Transform> transforms, NativeArray<NoisePollution> noiseMap, in CitizenHappinessParameterData data)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		int2 result = default(int2);
		if (transforms.HasComponent(building))
		{
			short num = (short)(NoisePollutionSystem.GetPollution(transforms[building].m_Position, noiseMap).m_Pollution / data.m_PollutionBonusDivisor);
			result.y = -math.min(data.m_MaxNoisePollutionBonus, (int)num);
		}
		return result;
	}

	public static int2 GetGarbageBonuses(Entity building, ref ComponentLookup<GarbageProducer> garbages, ref ComponentLookup<Locked> locked, Entity garbageService, in GarbageParameterData data)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (EntitiesExtensions.HasEnabledComponent<Locked>(locked, garbageService))
		{
			return new int2(0, 0);
		}
		int2 result = default(int2);
		if (garbages.HasComponent(building))
		{
			int num = math.max(0, (garbages[building].m_Garbage - data.m_HappinessEffectBaseline) / data.m_HappinessEffectStep);
			result.x = -math.min(10, num);
			result.y = -math.min(10, num);
		}
		return result;
	}

	public static int2 GetCrimeBonuses(CrimeVictim crimeVictim, Entity building, ref ComponentLookup<CrimeProducer> crimes, ref ComponentLookup<Locked> locked, Entity policeService, in CitizenHappinessParameterData data)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (EntitiesExtensions.HasEnabledComponent<Locked>(locked, policeService))
		{
			return new int2(0, 0);
		}
		int2 result = default(int2);
		if (crimes.HasComponent(building))
		{
			int num = Mathf.RoundToInt(math.max(0f, (crimes[building].m_Crime - (float)data.m_NegligibleCrime) * data.m_CrimeMultiplier));
			result.x = 0;
			result.y = -math.min(data.m_MaxCrimePenalty, num);
		}
		result.y -= crimeVictim.m_Effect;
		return result;
	}

	public static int2 GetMailBonuses(Entity building, ref ComponentLookup<MailProducer> mails, ref ComponentLookup<Locked> locked, Entity telecomService, in CitizenHappinessParameterData data)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		if (EntitiesExtensions.HasEnabledComponent<Locked>(locked, telecomService))
		{
			return new int2(0, 0);
		}
		int2 result = default(int2);
		if (mails.HasComponent(building))
		{
			MailProducer mailProducer = mails[building];
			int num = math.max(0, math.max((int)mailProducer.m_SendingMail, mailProducer.receivingMail) - data.m_NegligibleMail);
			result.x = 0;
			if (num < 25)
			{
				if (!mailProducer.mailDelivered)
				{
					return result;
				}
				int num2 = 125;
				int num3 = 25 - num;
				result.y = (num3 * num3 + (num2 >> 1)) / num2;
			}
			else
			{
				int num4 = 250;
				int num5 = math.min(50, num - 25);
				result.y = -((num5 * num5 + (num4 >> 1)) / num4);
			}
			result.y *= Mathf.RoundToInt(data.m_MailMultiplier);
		}
		return result;
	}

	public static int2 GetTelecomBonuses(Entity building, ref ComponentLookup<Transform> transforms, CellMapData<TelecomCoverage> telecomCoverage, ref ComponentLookup<Locked> locked, Entity telecomService, in CitizenHappinessParameterData data)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (EntitiesExtensions.HasEnabledComponent<Locked>(locked, telecomService))
		{
			return default(int2);
		}
		int2 result = default(int2);
		if (transforms.HasComponent(building))
		{
			float3 position = transforms[building].m_Position;
			float num = TelecomCoverage.SampleNetworkQuality(telecomCoverage, position);
			float telecomBaseline = data.m_TelecomBaseline;
			if (num >= telecomBaseline)
			{
				float num2 = (num - telecomBaseline) / (1f - telecomBaseline);
				result.y = Mathf.RoundToInt(num2 * num2 * data.m_TelecomBonusMultiplier);
			}
			else
			{
				float num3 = 1f - num / telecomBaseline;
				result.y = Mathf.RoundToInt(num3 * num3 * (0f - data.m_TelecomPenaltyMultiplier));
			}
		}
		return result;
	}

	public static int2 GetTaxBonuses(int educationLevel, NativeArray<int> taxRates, in CitizenHappinessParameterData data)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		int residentialTaxRate = TaxSystem.GetResidentialTaxRate(educationLevel, taxRates);
		float num = 0f;
		switch (educationLevel)
		{
		case 0:
			num = data.m_TaxUneducatedMultiplier;
			break;
		case 1:
			num = data.m_TaxPoorlyEducatedMultiplier;
			break;
		case 2:
			num = data.m_TaxEducatedMultiplier;
			break;
		case 3:
			num = data.m_TaxWellEducatedMultiplier;
			break;
		case 4:
			num = data.m_TaxHighlyEducatedMultiplier;
			break;
		}
		return new int2(0, Mathf.RoundToInt((float)(residentialTaxRate - 10) * num));
	}

	public static int2 GetWellfareBonuses(float curvePosition, DynamicBuffer<Game.Net.ServiceCoverage> serviceCoverage, in CitizenHappinessParameterData data, int currentHappiness)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		int2 result = default(int2);
		float num = data.m_WelfareMultiplier * NetUtils.GetServiceCoverage(serviceCoverage, CoverageService.Welfare, curvePosition);
		result.y = Mathf.RoundToInt(num * (float)math.max(0, (50 - currentHappiness) / 50));
		return result;
	}

	public static float GetWelfareValue(float curvePosition, DynamicBuffer<Game.Net.ServiceCoverage> serviceCoverage, in CitizenHappinessParameterData data)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return data.m_WelfareMultiplier * NetUtils.GetServiceCoverage(serviceCoverage, CoverageService.Welfare, curvePosition);
	}

	public static int2 GetCachedWelfareBonuses(float cachedValue, int currentHappiness)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		return new int2
		{
			y = Mathf.RoundToInt(cachedValue * (float)math.max(0, (50 - currentHappiness) / 50))
		};
	}

	public static int2 GetSicknessBonuses(bool hasHealthProblem, in CitizenHappinessParameterData data)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (hasHealthProblem)
		{
			return new int2(-data.m_HealthProblemHealthPenalty, 0);
		}
		return default(int2);
	}

	public static int2 GetHomelessBonuses(in CitizenHappinessParameterData data)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return new int2(data.m_HomelessHealthEffect, data.m_HomelessWellbeingEffect);
	}

	public static int2 GetDeathPenalty(DynamicBuffer<HouseholdCitizen> householdCitizens, ref ComponentLookup<HealthProblem> healthProblems, in CitizenHappinessParameterData data)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		Enumerator<HouseholdCitizen> enumerator = householdCitizens.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				if (CitizenUtils.IsDead(enumerator.Current.m_Citizen, ref healthProblems))
				{
					flag = true;
					break;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		if (flag)
		{
			return new int2(-data.m_DeathHealthPenalty, -data.m_DeathWellbeingPenalty);
		}
		return default(int2);
	}

	public static float GetConsumptionHappinessDifferential(float dailyConsumption, int citizens)
	{
		if (dailyConsumption <= 0f)
		{
			return 100f;
		}
		float num = dailyConsumption / math.max(1f, (float)citizens);
		return 8f / (1f + 0.2f * num) - 50000f * math.pow(2f * num + 190f, -2f);
	}

	public static int2 GetConsumptionBonuses(float dailyConsumption, int citizens, in CitizenHappinessParameterData data)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		float num = dailyConsumption / math.max(1f, (float)citizens);
		float num2 = 20f * math.log(1f + 0.2f * num) + 12500f / (2f * num + 190f) - 112f;
		return new int2(0, math.clamp(Mathf.RoundToInt(num2), -40, 40));
	}

	public static int2 GetLeisureBonuses(byte leisureValue)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return new int2(0, (leisureValue - 128) / 16);
	}

	public static int GetMaxHealth(float ageInYears)
	{
		if (ageInYears < 2f)
		{
			return 100;
		}
		if (ageInYears < 3f)
		{
			return 90;
		}
		if (ageInYears < 6f)
		{
			return 80;
		}
		return 80 - 10 * Mathf.FloorToInt(ageInYears - 5f);
	}

	public static void GetBuildingHappinessFactors(Entity property, NativeArray<int> factors, ref ComponentLookup<PrefabRef> prefabs, ref ComponentLookup<SpawnableBuildingData> spawnableBuildings, ref ComponentLookup<BuildingPropertyData> buildingPropertyDatas, ref ComponentLookup<ConsumptionData> consumptionDatas, ref BufferLookup<CityModifier> cityModifiers, ref ComponentLookup<Building> buildings, ref ComponentLookup<ElectricityConsumer> electricityConsumers, ref ComponentLookup<WaterConsumer> waterConsumers, ref BufferLookup<Game.Net.ServiceCoverage> serviceCoverages, ref ComponentLookup<Locked> locked, ref ComponentLookup<Transform> transforms, ref ComponentLookup<GarbageProducer> garbageProducers, ref ComponentLookup<CrimeProducer> crimeProducers, ref ComponentLookup<MailProducer> mailProducers, ref ComponentLookup<OfficeBuilding> officeBuildings, ref BufferLookup<Renter> renters, ref ComponentLookup<Citizen> citizenDatas, ref BufferLookup<HouseholdCitizen> householdCitizens, ref ComponentLookup<BuildingData> buildingDatas, ref ComponentLookup<CompanyData> companies, ref ComponentLookup<IndustrialProcessData> industrialProcessDatas, ref ComponentLookup<WorkProvider> workProviders, ref BufferLookup<Employee> employees, ref ComponentLookup<WorkplaceData> workplaceDatas, ref ComponentLookup<Citizen> citizens, ref ComponentLookup<HealthProblem> healthProblems, ref ComponentLookup<ServiceAvailable> serviceAvailables, ref ComponentLookup<ResourceData> resourceDatas, ref ComponentLookup<ZonePropertiesData> zonePropertiesDatas, ref BufferLookup<Efficiency> efficiencies, ref ComponentLookup<ServiceCompanyData> serviceCompanyDatas, ref BufferLookup<ResourceAvailability> availabilities, ref BufferLookup<TradeCost> tradeCosts, CitizenHappinessParameterData citizenHappinessParameters, GarbageParameterData garbageParameters, HealthcareParameterData healthcareParameters, ParkParameterData parkParameters, EducationParameterData educationParameters, TelecomParameterData telecomParameters, ref EconomyParameterData economyParameters, DynamicBuffer<HappinessFactorParameterData> happinessFactorParameters, NativeArray<GroundPollution> pollutionMap, NativeArray<NoisePollution> noisePollutionMap, NativeArray<AirPollution> airPollutionMap, CellMapData<TelecomCoverage> telecomCoverage, Entity city, NativeArray<int> taxRates, NativeArray<Entity> processes, ResourcePrefabs resourcePrefabs, float relativeElectricityFee, float relativeWaterFee)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0613: Unknown result type (might be due to invalid IL or missing references)
		//IL_0621: Unknown result type (might be due to invalid IL or missing references)
		//IL_05de: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0666: Unknown result type (might be due to invalid IL or missing references)
		//IL_0673: Unknown result type (might be due to invalid IL or missing references)
		//IL_062d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0634: Unknown result type (might be due to invalid IL or missing references)
		//IL_0639: Unknown result type (might be due to invalid IL or missing references)
		//IL_063f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0646: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_067f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0684: Unknown result type (might be due to invalid IL or missing references)
		//IL_0689: Unknown result type (might be due to invalid IL or missing references)
		//IL_068e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0695: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0707: Unknown result type (might be due to invalid IL or missing references)
		//IL_0715: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06da: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_052e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_056c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0579: Unknown result type (might be due to invalid IL or missing references)
		//IL_0586: Unknown result type (might be due to invalid IL or missing references)
		//IL_0591: Unknown result type (might be due to invalid IL or missing references)
		//IL_0758: Unknown result type (might be due to invalid IL or missing references)
		//IL_0766: Unknown result type (might be due to invalid IL or missing references)
		//IL_0721: Unknown result type (might be due to invalid IL or missing references)
		//IL_0724: Unknown result type (might be due to invalid IL or missing references)
		//IL_0727: Unknown result type (might be due to invalid IL or missing references)
		//IL_072c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0731: Unknown result type (might be due to invalid IL or missing references)
		//IL_0738: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0772: Unknown result type (might be due to invalid IL or missing references)
		//IL_0777: Unknown result type (might be due to invalid IL or missing references)
		//IL_077c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0782: Unknown result type (might be due to invalid IL or missing references)
		//IL_0789: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_0924: Unknown result type (might be due to invalid IL or missing references)
		//IL_0931: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07be: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0975: Unknown result type (might be due to invalid IL or missing references)
		//IL_0982: Unknown result type (might be due to invalid IL or missing references)
		//IL_093d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0940: Unknown result type (might be due to invalid IL or missing references)
		//IL_0942: Unknown result type (might be due to invalid IL or missing references)
		//IL_0945: Unknown result type (might be due to invalid IL or missing references)
		//IL_094a: Unknown result type (might be due to invalid IL or missing references)
		//IL_094f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0956: Unknown result type (might be due to invalid IL or missing references)
		//IL_0815: Unknown result type (might be due to invalid IL or missing references)
		//IL_0823: Unknown result type (might be due to invalid IL or missing references)
		//IL_07dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_098e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0991: Unknown result type (might be due to invalid IL or missing references)
		//IL_0993: Unknown result type (might be due to invalid IL or missing references)
		//IL_0996: Unknown result type (might be due to invalid IL or missing references)
		//IL_099b: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_086c: Unknown result type (might be due to invalid IL or missing references)
		//IL_087a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0831: Unknown result type (might be due to invalid IL or missing references)
		//IL_0833: Unknown result type (might be due to invalid IL or missing references)
		//IL_0836: Unknown result type (might be due to invalid IL or missing references)
		//IL_083a: Unknown result type (might be due to invalid IL or missing references)
		//IL_083f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0845: Unknown result type (might be due to invalid IL or missing references)
		//IL_084c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a16: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a24: Unknown result type (might be due to invalid IL or missing references)
		//IL_09df: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0888: Unknown result type (might be due to invalid IL or missing references)
		//IL_088c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0891: Unknown result type (might be due to invalid IL or missing references)
		//IL_0896: Unknown result type (might be due to invalid IL or missing references)
		//IL_089e: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a77: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a84: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a50: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a57: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0904: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ade: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0abf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b33: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b40: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0afd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b06: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b13: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b87: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b95: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b53: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b57: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b5c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b61: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b68: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0baf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bfd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c06: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c14: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c23: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c28: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c36: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c40: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c58: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c5e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c62: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c67: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c71: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c80: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c84: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c89: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c93: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cf6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cfb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d08: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < factors.Length; i++)
		{
			factors[i] = 0;
		}
		if (!prefabs.HasComponent(property))
		{
			return;
		}
		Entity prefab = prefabs[property].m_Prefab;
		if (!spawnableBuildings.HasComponent(prefab) || !buildingDatas.HasComponent(prefab))
		{
			return;
		}
		BuildingPropertyData buildingPropertyData = buildingPropertyDatas[prefab];
		DynamicBuffer<CityModifier> cityModifiers2 = cityModifiers[city];
		BuildingData buildingData = buildingDatas[prefab];
		float num = buildingData.m_LotSize.x * buildingData.m_LotSize.y;
		Entity val = Entity.Null;
		float curvePosition = 0f;
		SpawnableBuildingData spawnableData = spawnableBuildings[prefab];
		int level = spawnableData.m_Level;
		Building building = default(Building);
		if (buildings.HasComponent(property))
		{
			building = buildings[property];
			val = building.m_RoadEdge;
			curvePosition = building.m_CurvePosition;
		}
		bool flag = false;
		Entity val2 = default(Entity);
		Entity val3 = default(Entity);
		IndustrialProcessData processData = default(IndustrialProcessData);
		ServiceCompanyData serviceCompanyData = default(ServiceCompanyData);
		Resource resource = buildingPropertyData.m_AllowedManufactured | buildingPropertyData.m_AllowedSold;
		if (resource != Resource.NoResource)
		{
			if (renters.HasBuffer(property))
			{
				DynamicBuffer<Renter> val4 = renters[property];
				for (int j = 0; j < val4.Length; j++)
				{
					val2 = val4[j].m_Renter;
					if (!companies.HasComponent(val2) || !prefabs.HasComponent(val2))
					{
						continue;
					}
					val3 = prefabs[val2].m_Prefab;
					if (industrialProcessDatas.HasComponent(val3))
					{
						if (serviceCompanyDatas.HasComponent(val3))
						{
							serviceCompanyData = serviceCompanyDatas[val3];
						}
						processData = industrialProcessDatas[val3];
						flag = true;
						break;
					}
				}
			}
			int num2 = 0;
			if (flag)
			{
				AddCompanyHappinessFactors(factors, property, prefab, val2, val3, processData, serviceCompanyData, buildingPropertyData.m_AllowedSold != Resource.NoResource, level, ref officeBuildings, ref workProviders, ref employees, ref workplaceDatas, ref serviceAvailables, ref resourceDatas, ref efficiencies, ref buildingPropertyDatas, ref availabilities, ref tradeCosts, taxRates, building, spawnableData, buildingData, resourcePrefabs, ref economyParameters);
				num2++;
			}
			else
			{
				for (int k = 0; k < processes.Length; k++)
				{
					processData = industrialProcessDatas[processes[k]];
					bool num3 = buildingPropertyData.m_AllowedSold != Resource.NoResource;
					if (num3 && serviceCompanyDatas.HasComponent(processes[k]))
					{
						serviceCompanyData = serviceCompanyDatas[processes[k]];
					}
					if ((!num3 || serviceCompanyDatas.HasComponent(processes[k])) && (resource & processData.m_Output.m_Resource) != Resource.NoResource)
					{
						AddCompanyHappinessFactors(factors, property, prefab, val2, val3, processData, serviceCompanyData, buildingPropertyData.m_AllowedSold != Resource.NoResource, level, ref officeBuildings, ref workProviders, ref employees, ref workplaceDatas, ref serviceAvailables, ref resourceDatas, ref efficiencies, ref buildingPropertyDatas, ref availabilities, ref tradeCosts, taxRates, building, spawnableData, buildingData, resourcePrefabs, ref economyParameters);
						num2++;
					}
				}
			}
			for (int l = 0; l < factors.Length; l++)
			{
				int num4 = l;
				factors[num4] /= num2;
			}
		}
		if (buildingPropertyData.m_ResidentialProperties <= 0)
		{
			return;
		}
		for (int m = 0; m < factors.Length; m++)
		{
			factors[m] = Mathf.RoundToInt((float)factors[m] / (1f - economyParameters.m_MixedBuildingCompanyRentPercentage));
		}
		num /= (float)buildingPropertyData.m_ResidentialProperties;
		float num5 = 1f;
		int currentHappiness = 50;
		int num6 = 128;
		float num7 = 0.3f;
		float num8 = 0.25f;
		float num9 = 0.25f;
		float num10 = 0.15f;
		float num11 = 0.05f;
		float num12 = 2f;
		if (renters.HasBuffer(property))
		{
			num7 = 0f;
			num8 = 0f;
			num9 = 0f;
			num10 = 0f;
			num11 = 0f;
			int2 val5 = default(int2);
			int2 val6 = default(int2);
			int num13 = 0;
			int num14 = 0;
			DynamicBuffer<Renter> val7 = renters[property];
			for (int n = 0; n < val7.Length; n++)
			{
				Entity renter = val7[n].m_Renter;
				if (!householdCitizens.HasBuffer(renter))
				{
					continue;
				}
				num14++;
				DynamicBuffer<HouseholdCitizen> val8 = householdCitizens[renter];
				for (int num15 = 0; num15 < val8.Length; num15++)
				{
					Entity citizen = val8[num15].m_Citizen;
					if (citizenDatas.HasComponent(citizen))
					{
						Citizen citizen2 = citizenDatas[citizen];
						val6.x += citizen2.Happiness;
						val6.y++;
						num13 += citizen2.m_LeisureCounter;
						switch (citizen2.GetEducationLevel())
						{
						case 0:
							num7 += 1f;
							break;
						case 1:
							num8 += 1f;
							break;
						case 2:
							num9 += 1f;
							break;
						case 3:
							num10 += 1f;
							break;
						case 4:
							num11 += 1f;
							break;
						}
						if (citizen2.GetAge() == CitizenAge.Child)
						{
							val5.x++;
						}
					}
				}
				val5.y++;
			}
			if (val5.y > 0)
			{
				num5 = val5.x / val5.y;
			}
			if (val6.y > 0)
			{
				currentHappiness = Mathf.RoundToInt((float)(val6.x / val6.y));
				num6 = Mathf.RoundToInt((float)(num13 / val6.y));
				num7 /= (float)val6.y;
				num8 /= (float)val6.y;
				num9 /= (float)val6.y;
				num10 /= (float)val6.y;
				num11 /= (float)val6.y;
				num12 = (float)val6.y / (float)num14;
			}
		}
		Entity healthcareServicePrefab = healthcareParameters.m_HealthcareServicePrefab;
		Entity parkServicePrefab = parkParameters.m_ParkServicePrefab;
		Entity educationServicePrefab = educationParameters.m_EducationServicePrefab;
		Entity telecomServicePrefab = telecomParameters.m_TelecomServicePrefab;
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[4].m_LockedEntity))
		{
			int2 electricitySupplyBonuses = GetElectricitySupplyBonuses(property, ref electricityConsumers, in citizenHappinessParameters);
			factors[3] = (electricitySupplyBonuses.x + electricitySupplyBonuses.y) / 2 - happinessFactorParameters[4].m_BaseLevel;
		}
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[23].m_LockedEntity))
		{
			int2 electricityFeeBonuses = GetElectricityFeeBonuses(property, ref electricityConsumers, relativeElectricityFee, in citizenHappinessParameters);
			factors[26] = (electricityFeeBonuses.x + electricityFeeBonuses.y) / 2 - happinessFactorParameters[23].m_BaseLevel;
		}
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[8].m_LockedEntity))
		{
			int2 waterSupplyBonuses = GetWaterSupplyBonuses(property, ref waterConsumers, in citizenHappinessParameters);
			factors[7] = (waterSupplyBonuses.x + waterSupplyBonuses.y) / 2 - happinessFactorParameters[8].m_BaseLevel;
		}
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[24].m_LockedEntity))
		{
			int2 waterFeeBonuses = GetWaterFeeBonuses(property, ref waterConsumers, relativeWaterFee, in citizenHappinessParameters);
			factors[27] = (waterFeeBonuses.x + waterFeeBonuses.y) / 2 - happinessFactorParameters[24].m_BaseLevel;
		}
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[9].m_LockedEntity))
		{
			int2 waterPollutionBonuses = GetWaterPollutionBonuses(property, ref waterConsumers, cityModifiers2, in citizenHappinessParameters);
			factors[8] = (waterPollutionBonuses.x + waterPollutionBonuses.y) / 2 - happinessFactorParameters[9].m_BaseLevel;
		}
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[10].m_LockedEntity))
		{
			int2 sewageBonuses = GetSewageBonuses(property, ref waterConsumers, in citizenHappinessParameters);
			factors[9] = (sewageBonuses.x + sewageBonuses.y) / 2 - happinessFactorParameters[10].m_BaseLevel;
		}
		if (serviceCoverages.HasBuffer(val))
		{
			DynamicBuffer<Game.Net.ServiceCoverage> serviceCoverage = serviceCoverages[val];
			if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[5].m_LockedEntity))
			{
				int2 healthcareBonuses = GetHealthcareBonuses(curvePosition, serviceCoverage, ref locked, healthcareServicePrefab, in citizenHappinessParameters);
				factors[4] = (healthcareBonuses.x + healthcareBonuses.y) / 2 - happinessFactorParameters[5].m_BaseLevel;
			}
			if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[12].m_LockedEntity))
			{
				int2 entertainmentBonuses = GetEntertainmentBonuses(curvePosition, serviceCoverage, cityModifiers2, ref locked, parkServicePrefab, in citizenHappinessParameters);
				factors[11] = (entertainmentBonuses.x + entertainmentBonuses.y) / 2 - happinessFactorParameters[12].m_BaseLevel;
			}
			if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[13].m_LockedEntity))
			{
				int2 educationBonuses = GetEducationBonuses(curvePosition, serviceCoverage, ref locked, educationServicePrefab, in citizenHappinessParameters, 1);
				factors[12] = Mathf.RoundToInt(num5 * (float)(educationBonuses.x + educationBonuses.y) / 2f) - happinessFactorParameters[13].m_BaseLevel;
			}
			if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[15].m_LockedEntity))
			{
				int2 wellfareBonuses = GetWellfareBonuses(curvePosition, serviceCoverage, in citizenHappinessParameters, currentHappiness);
				factors[14] = (wellfareBonuses.x + wellfareBonuses.y) / 2 - happinessFactorParameters[15].m_BaseLevel;
			}
		}
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[6].m_LockedEntity))
		{
			int2 groundPollutionBonuses = GetGroundPollutionBonuses(property, ref transforms, pollutionMap, cityModifiers2, in citizenHappinessParameters);
			factors[5] = (groundPollutionBonuses.x + groundPollutionBonuses.y) / 2 - happinessFactorParameters[6].m_BaseLevel;
		}
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[2].m_LockedEntity))
		{
			int2 airPollutionBonuses = GetAirPollutionBonuses(property, ref transforms, airPollutionMap, cityModifiers2, in citizenHappinessParameters);
			factors[2] = (airPollutionBonuses.x + airPollutionBonuses.y) / 2 - happinessFactorParameters[2].m_BaseLevel;
		}
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[7].m_LockedEntity))
		{
			int2 noiseBonuses = GetNoiseBonuses(property, ref transforms, noisePollutionMap, in citizenHappinessParameters);
			factors[6] = (noiseBonuses.x + noiseBonuses.y) / 2 - happinessFactorParameters[7].m_BaseLevel;
		}
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[11].m_LockedEntity))
		{
			int2 garbageBonuses = GetGarbageBonuses(property, ref garbageProducers, ref locked, happinessFactorParameters[11].m_LockedEntity, in garbageParameters);
			factors[10] = (garbageBonuses.x + garbageBonuses.y) / 2 - happinessFactorParameters[11].m_BaseLevel;
		}
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[1].m_LockedEntity))
		{
			int2 crimeBonuses = GetCrimeBonuses(default(CrimeVictim), property, ref crimeProducers, ref locked, happinessFactorParameters[1].m_LockedEntity, in citizenHappinessParameters);
			factors[1] = (crimeBonuses.x + crimeBonuses.y) / 2 - happinessFactorParameters[1].m_BaseLevel;
		}
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[14].m_LockedEntity))
		{
			int2 mailBonuses = GetMailBonuses(property, ref mailProducers, ref locked, telecomServicePrefab, in citizenHappinessParameters);
			factors[13] = (mailBonuses.x + mailBonuses.y) / 2 - happinessFactorParameters[14].m_BaseLevel;
		}
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[0].m_LockedEntity))
		{
			int2 telecomBonuses = GetTelecomBonuses(property, ref transforms, telecomCoverage, ref locked, telecomServicePrefab, in citizenHappinessParameters);
			factors[0] = (telecomBonuses.x + telecomBonuses.y) / 2 - happinessFactorParameters[0].m_BaseLevel;
		}
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[16].m_LockedEntity))
		{
			int2 leisureBonuses = GetLeisureBonuses((byte)num6);
			factors[15] = (leisureBonuses.x + leisureBonuses.y) / 2 - happinessFactorParameters[16].m_BaseLevel;
		}
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[17].m_LockedEntity))
		{
			float2 val9 = new float2(num7, num7) * float2.op_Implicit(GetTaxBonuses(0, taxRates, in citizenHappinessParameters)) + new float2(num8, num8) * float2.op_Implicit(GetTaxBonuses(1, taxRates, in citizenHappinessParameters)) + new float2(num9, num9) * float2.op_Implicit(GetTaxBonuses(2, taxRates, in citizenHappinessParameters)) + new float2(num10, num10) * float2.op_Implicit(GetTaxBonuses(3, taxRates, in citizenHappinessParameters)) + new float2(num11, num11) * float2.op_Implicit(GetTaxBonuses(4, taxRates, in citizenHappinessParameters));
			factors[16] = Mathf.RoundToInt(val9.x + val9.y) / 2 - happinessFactorParameters[17].m_BaseLevel;
		}
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[3].m_LockedEntity))
		{
			float2 val10 = float2.op_Implicit(GetApartmentWellbeing(buildingPropertyData.m_SpaceMultiplier * num / num12, level));
			factors[21] = Mathf.RoundToInt(val10.x + val10.y) / 2 - happinessFactorParameters[3].m_BaseLevel;
		}
		if (resource != Resource.NoResource)
		{
			for (int num16 = 0; num16 < factors.Length; num16++)
			{
				factors[num16] = Mathf.RoundToInt((float)factors[num16] * (1f - economyParameters.m_MixedBuildingCompanyRentPercentage));
			}
		}
	}

	private static void AddCompanyHappinessFactors(NativeArray<int> factors, Entity property, Entity prefab, Entity renter, Entity renterPrefab, IndustrialProcessData processData, ServiceCompanyData serviceCompanyData, bool commercial, int level, ref ComponentLookup<OfficeBuilding> officeBuildings, ref ComponentLookup<WorkProvider> workProviders, ref BufferLookup<Employee> employees, ref ComponentLookup<WorkplaceData> workplaceDatas, ref ComponentLookup<ServiceAvailable> serviceAvailables, ref ComponentLookup<ResourceData> resourceDatas, ref BufferLookup<Efficiency> efficiencies, ref ComponentLookup<BuildingPropertyData> buildingPropertyDatas, ref BufferLookup<ResourceAvailability> availabilities, ref BufferLookup<TradeCost> tradeCosts, NativeArray<int> taxRates, Building building, SpawnableBuildingData spawnableData, BuildingData buildingData, ResourcePrefabs resourcePrefabs, ref EconomyParameterData economyParameters)
	{
	}

	private static int GetFactor(float profit, float defaultProfit)
	{
		return Mathf.RoundToInt(10f * (profit / defaultProfit - 1f));
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0579: Unknown result type (might be due to invalid IL or missing references)
		//IL_057e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0593: Unknown result type (might be due to invalid IL or missing references)
		//IL_0598: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05db: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0604: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0626: Unknown result type (might be due to invalid IL or missing references)
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		//IL_0640: Unknown result type (might be due to invalid IL or missing references)
		//IL_064d: Unknown result type (might be due to invalid IL or missing references)
		//IL_065a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0667: Unknown result type (might be due to invalid IL or missing references)
		//IL_0674: Unknown result type (might be due to invalid IL or missing references)
		//IL_0686: Unknown result type (might be due to invalid IL or missing references)
		//IL_068b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0693: Unknown result type (might be due to invalid IL or missing references)
		//IL_0698: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_070a: Unknown result type (might be due to invalid IL or missing references)
		//IL_070c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0718: Unknown result type (might be due to invalid IL or missing references)
		//IL_071d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0729: Unknown result type (might be due to invalid IL or missing references)
		//IL_0619: Unknown result type (might be due to invalid IL or missing references)
		uint updateFrameWithInterval = SimulationUtils.GetUpdateFrameWithInterval(m_SimulationSystem.frameIndex, (uint)GetUpdateInterval(SystemUpdatePhase.GameSimulation), 16);
		((EntityQuery)(ref m_CitizenQuery)).ResetFilter();
		((EntityQuery)(ref m_CitizenQuery)).AddSharedComponentFilter<UpdateFrame>(new UpdateFrame(updateFrameWithInterval));
		ParallelWriter<int> debugQueue = default(ParallelWriter<int>);
		if (m_DebugData.IsEnabled)
		{
			debugQueue = m_DebugData.GetQueue(clear: false, out var _).AsParallelWriter();
		}
		JobHandle dependencies;
		JobHandle dependencies2;
		JobHandle dependencies3;
		JobHandle dependencies4;
		JobHandle dependencies5;
		JobHandle deps2;
		JobHandle val = JobChunkExtensions.ScheduleParallel<CitizenHappinessJob>(new CitizenHappinessJob
		{
			m_DebugQueue = debugQueue,
			m_DebugOn = m_DebugData.IsEnabled,
			m_CitizenType = InternalCompilerInterface.GetComponentTypeHandle<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdMemberType = InternalCompilerInterface.GetComponentTypeHandle<HouseholdMember>(ref __TypeHandle.__Game_Citizens_HouseholdMember_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CrimeVictimType = InternalCompilerInterface.GetComponentTypeHandle<CrimeVictim>(ref __TypeHandle.__Game_Citizens_CrimeVictim_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CriminalType = InternalCompilerInterface.GetComponentTypeHandle<Criminal>(ref __TypeHandle.__Game_Citizens_Criminal_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_StudentType = InternalCompilerInterface.GetComponentTypeHandle<Game.Citizens.Student>(ref __TypeHandle.__Game_Citizens_Student_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentBuildingType = InternalCompilerInterface.GetComponentTypeHandle<CurrentBuilding>(ref __TypeHandle.__Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HealthProblemType = InternalCompilerInterface.GetComponentTypeHandle<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Households = InternalCompilerInterface.GetComponentLookup<Household>(ref __TypeHandle.__Game_Citizens_Household_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HealthProblems = InternalCompilerInterface.GetComponentLookup<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Buildings = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ElectricityConsumers = InternalCompilerInterface.GetComponentLookup<ElectricityConsumer>(ref __TypeHandle.__Game_Buildings_ElectricityConsumer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Properties = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Resources = InternalCompilerInterface.GetBufferLookup<Resources>(ref __TypeHandle.__Game_Economy_Resources_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceCoverages = InternalCompilerInterface.GetBufferLookup<Game.Net.ServiceCoverage>(ref __TypeHandle.__Game_Net_ServiceCoverage_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Transforms = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentDistrictData = InternalCompilerInterface.GetComponentLookup<CurrentDistrict>(ref __TypeHandle.__Game_Areas_CurrentDistrict_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WaterConsumers = InternalCompilerInterface.GetComponentLookup<WaterConsumer>(ref __TypeHandle.__Game_Buildings_WaterConsumer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Garbages = InternalCompilerInterface.GetComponentLookup<GarbageProducer>(ref __TypeHandle.__Game_Buildings_GarbageProducer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdCitizens = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Locked = InternalCompilerInterface.GetComponentLookup<Locked>(ref __TypeHandle.__Game_Prefabs_Locked_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CrimeProducers = InternalCompilerInterface.GetComponentLookup<CrimeProducer>(ref __TypeHandle.__Game_Buildings_CrimeProducer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MailProducers = InternalCompilerInterface.GetComponentLookup<MailProducer>(ref __TypeHandle.__Game_Buildings_MailProducer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingDatas = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingPropertyDatas = InternalCompilerInterface.GetComponentLookup<BuildingPropertyData>(ref __TypeHandle.__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnableBuildings = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DistrictModifiers = InternalCompilerInterface.GetBufferLookup<DistrictModifier>(ref __TypeHandle.__Game_Areas_DistrictModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CityModifiers = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceFees = InternalCompilerInterface.GetBufferLookup<ServiceFee>(ref __TypeHandle.__Game_City_ServiceFee_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Prisons = InternalCompilerInterface.GetComponentLookup<Game.Buildings.Prison>(ref __TypeHandle.__Game_Buildings_Prison_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Schools = InternalCompilerInterface.GetComponentLookup<Game.Buildings.School>(ref __TypeHandle.__Game_Buildings_School_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HomelessHouseholds = InternalCompilerInterface.GetComponentLookup<HomelessHousehold>(ref __TypeHandle.__Game_Citizens_HomelessHousehold_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PollutionMap = m_GroundPollutionSystem.GetMap(readOnly: true, out dependencies),
			m_AirPollutionMap = m_AirPollutionSystem.GetMap(readOnly: true, out dependencies2),
			m_NoisePollutionMap = m_NoisePollutionSystem.GetMap(readOnly: true, out dependencies3),
			m_TelecomCoverage = m_TelecomCoverageSystem.GetData(readOnly: true, out dependencies4),
			m_LocalEffectData = m_LocalEffectSystem.GetReadData(out dependencies5),
			m_HealthcareParameters = ((EntityQuery)(ref m_HealthcareParameterQuery)).GetSingleton<HealthcareParameterData>(),
			m_ParkParameters = ((EntityQuery)(ref m_ParkParameterQuery)).GetSingleton<ParkParameterData>(),
			m_EducationParameters = ((EntityQuery)(ref m_EducationParameterQuery)).GetSingleton<EducationParameterData>(),
			m_TelecomParameters = ((EntityQuery)(ref m_TelecomParameterQuery)).GetSingleton<TelecomParameterData>(),
			m_GarbageParameters = ((EntityQuery)(ref m_GarbageParameterQuery)).GetSingleton<GarbageParameterData>(),
			m_PoliceParameters = ((EntityQuery)(ref m_PoliceParameterQuery)).GetSingleton<PoliceConfigurationData>(),
			m_CitizenHappinessParameters = ((EntityQuery)(ref m_CitizenHappinessParameterQuery)).GetSingleton<CitizenHappinessParameterData>(),
			m_SimulationFrame = m_SimulationSystem.frameIndex,
			m_TimeSettings = ((EntityQuery)(ref m_TimeSettingQuery)).GetSingleton<TimeSettingsData>(),
			m_FeeParameters = ((EntityQuery)(ref __query_429327288_0)).GetSingleton<ServiceFeeParameterData>(),
			m_TimeData = ((EntityQuery)(ref m_TimeDataQuery)).GetSingleton<TimeData>(),
			m_TaxRates = m_TaxSystem.GetTaxRates(),
			m_RawUpdateFrame = updateFrameWithInterval,
			m_City = m_CitySystem.City,
			m_RandomSeed = RandomSeed.Next(),
			m_FactorQueue = m_FactorQueue.AsParallelWriter(),
			m_StatisticsEventQueue = m_CityStatisticsSystem.GetStatisticsEventQueue(out deps2).AsParallelWriter()
		}, m_CitizenQuery, JobHandle.CombineDependencies(dependencies5, dependencies4, JobHandle.CombineDependencies(dependencies2, dependencies3, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies, deps2))));
		if (m_DebugData.IsEnabled)
		{
			m_DebugData.AddWriter(val);
		}
		m_GroundPollutionSystem.AddReader(val);
		m_AirPollutionSystem.AddReader(val);
		m_NoisePollutionSystem.AddReader(val);
		m_TelecomCoverageSystem.AddReader(val);
		m_LocalEffectSystem.AddLocalEffectReader(val);
		m_TaxSystem.AddReader(val);
		m_CityStatisticsSystem.AddWriter(val);
		HappinessFactorJob happinessFactorJob = new HappinessFactorJob
		{
			m_FactorQueue = m_FactorQueue,
			m_HappinessFactors = m_HappinessFactors,
			m_RawUpdateFrame = updateFrameWithInterval,
			m_TriggerActionQueue = m_TriggerSystem.CreateActionBuffer(),
			m_ParameterEntity = ((EntityQuery)(ref m_HappinessFactorParameterQuery)).GetSingletonEntity(),
			m_Parameters = InternalCompilerInterface.GetBufferLookup<HappinessFactorParameterData>(ref __TypeHandle.__Game_Prefabs_HappinessFactorParameterData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Locked = InternalCompilerInterface.GetComponentLookup<Locked>(ref __TypeHandle.__Game_Prefabs_Locked_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<HappinessFactorJob>(happinessFactorJob, val);
		m_LastDeps = ((SystemBase)this).Dependency;
		m_TriggerSystem.AddActionBufferWriter(((SystemBase)this).Dependency);
	}

	private static TriggerType GetTriggerTypeForHappinessFactor(HappinessFactor factor)
	{
		switch (factor)
		{
		case HappinessFactor.Telecom:
			return TriggerType.TelecomHappinessFactor;
		case HappinessFactor.Crime:
			return TriggerType.CrimeHappinessFactor;
		case HappinessFactor.AirPollution:
			return TriggerType.AirPollutionHappinessFactor;
		case HappinessFactor.Apartment:
			return TriggerType.ApartmentHappinessFactor;
		case HappinessFactor.Electricity:
			return TriggerType.ElectricityHappinessFactor;
		case HappinessFactor.Healthcare:
			return TriggerType.HealthcareHappinessFactor;
		case HappinessFactor.GroundPollution:
			return TriggerType.GroundPollutionHappinessFactor;
		case HappinessFactor.NoisePollution:
			return TriggerType.NoisePollutionHappinessFactor;
		case HappinessFactor.Water:
			return TriggerType.WaterHappinessFactor;
		case HappinessFactor.WaterPollution:
			return TriggerType.WaterPollutionHappinessFactor;
		case HappinessFactor.Sewage:
			return TriggerType.SewageHappinessFactor;
		case HappinessFactor.Garbage:
			return TriggerType.GarbageHappinessFactor;
		case HappinessFactor.Entertainment:
			return TriggerType.EntertainmentHappinessFactor;
		case HappinessFactor.Education:
			return TriggerType.EducationHappinessFactor;
		case HappinessFactor.Mail:
			return TriggerType.MailHappinessFactor;
		case HappinessFactor.Welfare:
			return TriggerType.WelfareHappinessFactor;
		case HappinessFactor.Leisure:
			return TriggerType.LeisureHappinessFactor;
		case HappinessFactor.Tax:
			return TriggerType.TaxHappinessFactor;
		case HappinessFactor.Buildings:
			return TriggerType.BuildingsHappinessFactor;
		case HappinessFactor.Consumption:
			return TriggerType.WealthHappinessFactor;
		case HappinessFactor.TrafficPenalty:
			return TriggerType.TrafficPenaltyHappinessFactor;
		case HappinessFactor.DeathPenalty:
			return TriggerType.DeathPenaltyHappinessFactor;
		case HappinessFactor.Homelessness:
			return TriggerType.HomelessnessHappinessFactor;
		case HappinessFactor.ElectricityFee:
			return TriggerType.ElectricityFeeHappinessFactor;
		case HappinessFactor.WaterFee:
			return TriggerType.WaterFeeHappinessFactor;
		default:
			Debug.LogError((object)$"Unknown trigger type for happiness factor: {factor}");
			return TriggerType.NewNotification;
		}
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		((IWriter)writer/*cast due to .constrained prefix*/).Write(25);
		for (int i = 0; i < 400; i++)
		{
			int4 val = m_HappinessFactors[i];
			((IWriter)writer/*cast due to .constrained prefix*/).Write(val);
		}
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.happinessFactorSerialization)
		{
			int4 val = default(int4);
			for (int i = 0; i < 352; i++)
			{
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref val);
				m_HappinessFactors[i] = val;
			}
			return;
		}
		int num = default(int);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		int4 val2 = default(int4);
		for (int j = 0; j < num * 16; j++)
		{
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref val2);
			m_HappinessFactors[j] = val2;
		}
	}

	public void SetDefaults(Context context)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 400; i++)
		{
			m_HappinessFactors[i] = default(int4);
		}
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
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<ServiceFeeParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_429327288_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
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
	public CitizenHappinessSystem()
	{
	}
}
