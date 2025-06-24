using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Areas;
using Game.Buildings;
using Game.Citizens;
using Game.Common;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class ResidentsSection : InfoSectionBase
{
	public enum Result
	{
		Visible,
		ResidentCount,
		PetCount,
		HouseholdCount,
		MaxHouseholds,
		ResultCount
	}

	[BurstCompile]
	public struct CountHouseholdsJob : IJob
	{
		[ReadOnly]
		public Entity m_SelectedEntity;

		[ReadOnly]
		public Entity m_SelectedPrefab;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Park> m_ParkLookup;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingLookup;

		[ReadOnly]
		public ComponentLookup<Abandoned> m_AbandonedLookup;

		[ReadOnly]
		public ComponentLookup<Household> m_HouseholdLookup;

		[ReadOnly]
		public ComponentLookup<HomelessHousehold> m_HomelessHouseholdLookup;

		[ReadOnly]
		public ComponentLookup<HealthProblem> m_HealthProblemLookup;

		[ReadOnly]
		public ComponentLookup<TravelPurpose> m_TravelPurposeLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenterLookup;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> m_PropertyDataLookup;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> m_HouseholdCitizenLookup;

		[ReadOnly]
		public BufferLookup<HouseholdAnimal> m_HouseholdAnimalLookup;

		[ReadOnly]
		public BufferLookup<Renter> m_RenterLookup;

		public NativeArray<int> m_Results;

		public NativeList<Entity> m_HouseholdsResult;

		public NativeValue<Entity> m_ResidenceResult;

		public void Execute()
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			int residentCount = 0;
			int petCount = 0;
			int householdCount = 0;
			int maxHouseholds = 0;
			if (m_BuildingLookup.HasComponent(m_SelectedEntity) && TryCountHouseholds(ref residentCount, ref petCount, ref householdCount, ref maxHouseholds, m_SelectedEntity, m_SelectedPrefab, ref m_ParkLookup, ref m_AbandonedLookup, ref m_PropertyDataLookup, ref m_HealthProblemLookup, ref m_TravelPurposeLookup, ref m_HouseholdLookup, ref m_RenterLookup, ref m_HouseholdCitizenLookup, ref m_HouseholdAnimalLookup, m_HouseholdsResult))
			{
				m_Results[0] = 1;
				m_Results[1] = residentCount;
				m_Results[2] = petCount;
				m_Results[3] = householdCount;
				m_Results[4] = maxHouseholds;
			}
			else
			{
				DynamicBuffer<HouseholdCitizen> val = default(DynamicBuffer<HouseholdCitizen>);
				if (!m_HouseholdLookup.HasComponent(m_SelectedEntity) || !m_HouseholdCitizenLookup.TryGetBuffer(m_SelectedEntity, ref val))
				{
					return;
				}
				PropertyRenter propertyRenter = default(PropertyRenter);
				HomelessHousehold homelessHousehold = default(HomelessHousehold);
				if (m_PropertyRenterLookup.TryGetComponent(m_SelectedEntity, ref propertyRenter))
				{
					m_HouseholdsResult.Add(ref m_SelectedEntity);
					m_ResidenceResult.value = propertyRenter.m_Property;
				}
				else if (m_HomelessHouseholdLookup.TryGetComponent(m_SelectedEntity, ref homelessHousehold))
				{
					m_HouseholdsResult.Add(ref m_SelectedEntity);
					m_ResidenceResult.value = homelessHousehold.m_TempHome;
				}
				for (int i = 0; i < val.Length; i++)
				{
					if (!CitizenUtils.IsCorpsePickedByHearse(val[i].m_Citizen, ref m_HealthProblemLookup, ref m_TravelPurposeLookup))
					{
						residentCount++;
					}
				}
				DynamicBuffer<HouseholdAnimal> val2 = default(DynamicBuffer<HouseholdAnimal>);
				if (m_HouseholdAnimalLookup.TryGetBuffer(m_SelectedEntity, ref val2))
				{
					petCount += val2.Length;
				}
				m_Results[0] = 1;
				m_Results[1] = residentCount;
				m_Results[2] = petCount;
				m_Results[3] = 1;
				m_Results[4] = 1;
			}
		}
	}

	[BurstCompile]
	public struct CountDistrictHouseholdsJob : IJobChunk
	{
		[ReadOnly]
		public Entity m_SelectedEntity;

		[ReadOnly]
		public EntityTypeHandle m_EntityHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentDistrict> m_CurrentDistrictHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefHandle;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Park> m_ParkLookup;

		[ReadOnly]
		public ComponentLookup<Abandoned> m_AbandonedLookup;

		[ReadOnly]
		public ComponentLookup<HealthProblem> m_HealthProblemLookup;

		[ReadOnly]
		public ComponentLookup<TravelPurpose> m_TravelPurposeLookup;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> m_PropertyDataLookup;

		[ReadOnly]
		public ComponentLookup<Household> m_HouseholdLookup;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> m_HouseholdCitizenLookup;

		[ReadOnly]
		public BufferLookup<HouseholdAnimal> m_HouseholdAnimalLookup;

		[ReadOnly]
		public BufferLookup<Renter> m_RenterLookup;

		public NativeArray<int> m_Results;

		public NativeList<Entity> m_HouseholdsResult;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityHandle);
			NativeArray<CurrentDistrict> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentDistrict>(ref m_CurrentDistrictHandle);
			NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefHandle);
			int num = 0;
			int residentCount = 0;
			int petCount = 0;
			int householdCount = 0;
			int maxHouseholds = 0;
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity entity = nativeArray[i];
				CurrentDistrict currentDistrict = nativeArray2[i];
				PrefabRef prefabRef = nativeArray3[i];
				if (!(currentDistrict.m_District != m_SelectedEntity) && TryCountHouseholds(ref residentCount, ref petCount, ref householdCount, ref maxHouseholds, entity, prefabRef.m_Prefab, ref m_ParkLookup, ref m_AbandonedLookup, ref m_PropertyDataLookup, ref m_HealthProblemLookup, ref m_TravelPurposeLookup, ref m_HouseholdLookup, ref m_RenterLookup, ref m_HouseholdCitizenLookup, ref m_HouseholdAnimalLookup, m_HouseholdsResult))
				{
					num = 1;
				}
			}
			ref NativeArray<int> reference = ref m_Results;
			reference[0] = reference[0] + num;
			reference = ref m_Results;
			reference[1] = reference[1] + residentCount;
			reference = ref m_Results;
			reference[2] = reference[2] + petCount;
			reference = ref m_Results;
			reference[3] = reference[3] + householdCount;
			reference = ref m_Results;
			reference[4] = reference[4] + maxHouseholds;
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
		public ComponentTypeHandle<CurrentDistrict> __Game_Areas_CurrentDistrict_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Park> __Game_Buildings_Park_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Abandoned> __Game_Buildings_Abandoned_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HealthProblem> __Game_Citizens_HealthProblem_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TravelPurpose> __Game_Citizens_TravelPurpose_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> __Game_Prefabs_BuildingPropertyData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Household> __Game_Citizens_Household_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<HouseholdAnimal> __Game_Citizens_HouseholdAnimal_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Renter> __Game_Buildings_Renter_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HomelessHousehold> __Game_Citizens_HomelessHousehold_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Areas_CurrentDistrict_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentDistrict>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Buildings_Park_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.Park>(true);
			__Game_Buildings_Abandoned_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Abandoned>(true);
			__Game_Citizens_HealthProblem_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HealthProblem>(true);
			__Game_Citizens_TravelPurpose_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TravelPurpose>(true);
			__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingPropertyData>(true);
			__Game_Citizens_Household_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Household>(true);
			__Game_Citizens_HouseholdCitizen_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdCitizen>(true);
			__Game_Citizens_HouseholdAnimal_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdAnimal>(true);
			__Game_Buildings_Renter_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Renter>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Citizens_HomelessHousehold_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HomelessHousehold>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
		}
	}

	private EntityQuery m_DistrictBuildingQuery;

	private EntityQuery m_HappinessParameterQuery;

	private NativeArray<int> m_Results;

	private NativeValue<Entity> m_ResidenceResult;

	private NativeList<Entity> m_HouseholdsResult;

	private TypeHandle __TypeHandle;

	protected override string group => "ResidentsSection";

	private bool isHousehold { get; set; }

	private int householdCount { get; set; }

	private int maxHouseholds { get; set; }

	private HouseholdWealthKey wealthKey { get; set; }

	private int residentCount { get; set; }

	private int petCount { get; set; }

	private Entity residenceEntity { get; set; }

	private CitizenResidenceKey residenceKey { get; set; }

	private EducationData educationData { get; set; }

	private AgeData ageData { get; set; }

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_DistrictBuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[7]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<ResidentialProperty>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<Renter>(),
			ComponentType.ReadOnly<CurrentDistrict>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_HappinessParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<CitizenHappinessParameterData>() });
		m_HouseholdsResult = new NativeList<Entity>(AllocatorHandle.op_Implicit((Allocator)4));
		m_ResidenceResult = new NativeValue<Entity>((Allocator)4);
		m_Results = new NativeArray<int>(5, (Allocator)4, (NativeArrayOptions)1);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_HouseholdsResult.Dispose();
		m_ResidenceResult.Dispose();
		m_Results.Dispose();
		base.OnDestroy();
	}

	protected override void Reset()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		isHousehold = false;
		householdCount = 0;
		maxHouseholds = 0;
		residentCount = 0;
		petCount = 0;
		educationData = default(EducationData);
		ageData = default(AgeData);
		m_HouseholdsResult.Clear();
		m_ResidenceResult.value = Entity.Null;
		m_Results[0] = 0;
		m_Results[1] = 0;
		m_Results[2] = 0;
		m_Results[4] = 0;
		m_Results[3] = 0;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		JobHandle val;
		if (((EntityManager)(ref entityManager)).HasComponent<District>(selectedEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Area>(selectedEntity))
			{
				val = JobChunkExtensions.Schedule<CountDistrictHouseholdsJob>(new CountDistrictHouseholdsJob
				{
					m_SelectedEntity = selectedEntity,
					m_EntityHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_CurrentDistrictHandle = InternalCompilerInterface.GetComponentTypeHandle<CurrentDistrict>(ref __TypeHandle.__Game_Areas_CurrentDistrict_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabRefHandle = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_ParkLookup = InternalCompilerInterface.GetComponentLookup<Game.Buildings.Park>(ref __TypeHandle.__Game_Buildings_Park_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_AbandonedLookup = InternalCompilerInterface.GetComponentLookup<Abandoned>(ref __TypeHandle.__Game_Buildings_Abandoned_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_HealthProblemLookup = InternalCompilerInterface.GetComponentLookup<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_TravelPurposeLookup = InternalCompilerInterface.GetComponentLookup<TravelPurpose>(ref __TypeHandle.__Game_Citizens_TravelPurpose_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PropertyDataLookup = InternalCompilerInterface.GetComponentLookup<BuildingPropertyData>(ref __TypeHandle.__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_HouseholdLookup = InternalCompilerInterface.GetComponentLookup<Household>(ref __TypeHandle.__Game_Citizens_Household_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_HouseholdCitizenLookup = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_HouseholdAnimalLookup = InternalCompilerInterface.GetBufferLookup<HouseholdAnimal>(ref __TypeHandle.__Game_Citizens_HouseholdAnimal_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_RenterLookup = InternalCompilerInterface.GetBufferLookup<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_Results = m_Results,
					m_HouseholdsResult = m_HouseholdsResult
				}, m_DistrictBuildingQuery, ((SystemBase)this).Dependency);
				((JobHandle)(ref val)).Complete();
				base.visible = m_Results[0] > 0;
				return;
			}
		}
		val = IJobExtensions.Schedule<CountHouseholdsJob>(new CountHouseholdsJob
		{
			m_SelectedEntity = selectedEntity,
			m_SelectedPrefab = selectedPrefab,
			m_BuildingLookup = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkLookup = InternalCompilerInterface.GetComponentLookup<Game.Buildings.Park>(ref __TypeHandle.__Game_Buildings_Park_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AbandonedLookup = InternalCompilerInterface.GetComponentLookup<Abandoned>(ref __TypeHandle.__Game_Buildings_Abandoned_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdLookup = InternalCompilerInterface.GetComponentLookup<Household>(ref __TypeHandle.__Game_Citizens_Household_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HomelessHouseholdLookup = InternalCompilerInterface.GetComponentLookup<HomelessHousehold>(ref __TypeHandle.__Game_Citizens_HomelessHousehold_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HealthProblemLookup = InternalCompilerInterface.GetComponentLookup<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TravelPurposeLookup = InternalCompilerInterface.GetComponentLookup<TravelPurpose>(ref __TypeHandle.__Game_Citizens_TravelPurpose_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyRenterLookup = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyDataLookup = InternalCompilerInterface.GetComponentLookup<BuildingPropertyData>(ref __TypeHandle.__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdCitizenLookup = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdAnimalLookup = InternalCompilerInterface.GetBufferLookup<HouseholdAnimal>(ref __TypeHandle.__Game_Citizens_HouseholdAnimal_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RenterLookup = InternalCompilerInterface.GetBufferLookup<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Results = m_Results,
			m_HouseholdsResult = m_HouseholdsResult,
			m_ResidenceResult = m_ResidenceResult
		}, ((SystemBase)this).Dependency);
		((JobHandle)(ref val)).Complete();
		base.visible = m_Results[0] > 0;
	}

	protected override void OnProcess()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Household>(selectedEntity))
		{
			isHousehold = true;
		}
		householdCount = m_Results[3];
		residentCount = m_Results[1];
		petCount = m_Results[2];
		maxHouseholds = m_Results[4];
		wealthKey = CitizenUIUtils.GetAverageHouseholdWealth(((ComponentSystemBase)this).EntityManager, m_HouseholdsResult, ((EntityQuery)(ref m_HappinessParameterQuery)).GetSingleton<CitizenHappinessParameterData>());
		DynamicBuffer<HouseholdCitizen> citizens = default(DynamicBuffer<HouseholdCitizen>);
		if (!isHousehold)
		{
			for (int i = 0; i < m_HouseholdsResult.Length; i++)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				DynamicBuffer<HouseholdCitizen> buffer = ((EntityManager)(ref entityManager)).GetBuffer<HouseholdCitizen>(m_HouseholdsResult[i], true);
				ageData += GetAgeData(buffer);
				educationData += GetEducationData(buffer);
			}
		}
		else if (EntitiesExtensions.TryGetBuffer<HouseholdCitizen>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref citizens))
		{
			ageData += GetAgeData(citizens);
			educationData += GetEducationData(citizens);
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).Exists(m_ResidenceResult.value))
		{
			residenceEntity = Entity.Null;
			residenceKey = CitizenResidenceKey.Home;
			return;
		}
		residenceEntity = m_ResidenceResult.value;
		entityManager = ((ComponentSystemBase)this).EntityManager;
		int num;
		if (!((EntityManager)(ref entityManager)).HasComponent<TouristHousehold>(selectedEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			num = (((EntityManager)(ref entityManager)).HasComponent<HomelessHousehold>(selectedEntity) ? 2 : 0);
		}
		else
		{
			num = 1;
		}
		residenceKey = (CitizenResidenceKey)num;
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.Park>(residenceEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (!((EntityManager)(ref entityManager)).HasComponent<Abandoned>(residenceEntity))
			{
				goto IL_0247;
			}
		}
		DynamicBuffer<Renter> val = default(DynamicBuffer<Renter>);
		if (EntitiesExtensions.TryGetBuffer<Renter>(((ComponentSystemBase)this).EntityManager, residenceEntity, true, ref val) && val.Length > 0)
		{
			m_InfoUISystem.tooltipTags.Add(TooltipTags.HomelessShelter);
			base.tooltipTags.Add(TooltipTags.HomelessShelter.ToString());
			base.tooltipKeys.Add(TooltipTags.HomelessShelter.ToString());
			return;
		}
		goto IL_0247;
		IL_0247:
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.Park>(selectedEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (!((EntityManager)(ref entityManager)).HasComponent<Abandoned>(selectedEntity))
			{
				return;
			}
		}
		DynamicBuffer<Renter> val2 = default(DynamicBuffer<Renter>);
		if (EntitiesExtensions.TryGetBuffer<Renter>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref val2) && val2.Length > 0)
		{
			m_InfoUISystem.tooltipTags.Add(TooltipTags.HomelessShelter);
			base.tooltipTags.Add(TooltipTags.HomelessShelter.ToString());
			base.tooltipKeys.Add(TooltipTags.HomelessShelter.ToString());
		}
	}

	private AgeData GetAgeData(DynamicBuffer<HouseholdCitizen> citizens)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		Citizen citizen2 = default(Citizen);
		for (int i = 0; i < citizens.Length; i++)
		{
			Entity citizen = citizens[i].m_Citizen;
			if (EntitiesExtensions.TryGetComponent<Citizen>(((ComponentSystemBase)this).EntityManager, citizen, ref citizen2) && !CitizenUtils.IsCorpsePickedByHearse(((ComponentSystemBase)this).EntityManager, citizen))
			{
				switch (citizen2.GetAge())
				{
				case CitizenAge.Child:
					num++;
					break;
				case CitizenAge.Teen:
					num2++;
					break;
				case CitizenAge.Adult:
					num3++;
					break;
				case CitizenAge.Elderly:
					num4++;
					break;
				}
			}
		}
		return new AgeData(num, num2, num3, num4);
	}

	private EducationData GetEducationData(DynamicBuffer<HouseholdCitizen> citizens)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		Citizen citizen = default(Citizen);
		for (int i = 0; i < citizens.Length; i++)
		{
			if (EntitiesExtensions.TryGetComponent<Citizen>(((ComponentSystemBase)this).EntityManager, citizens[i].m_Citizen, ref citizen) && !CitizenUtils.IsCorpsePickedByHearse(((ComponentSystemBase)this).EntityManager, citizens[i].m_Citizen))
			{
				switch (citizen.GetEducationLevel())
				{
				case 0:
					num++;
					break;
				case 1:
					num2++;
					break;
				case 2:
					num3++;
					break;
				case 3:
					num4++;
					break;
				case 4:
					num5++;
					break;
				}
			}
		}
		return new EducationData(num, num2, num3, num4, num5);
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		writer.PropertyName("isHousehold");
		writer.Write(isHousehold);
		writer.PropertyName("householdCount");
		writer.Write(householdCount);
		writer.PropertyName("maxHouseholds");
		writer.Write(maxHouseholds);
		writer.PropertyName("residentCount");
		writer.Write(residentCount);
		writer.PropertyName("petCount");
		writer.Write(petCount);
		writer.PropertyName("wealthKey");
		writer.Write(wealthKey.ToString());
		writer.PropertyName("residence");
		if (residenceEntity == Entity.Null)
		{
			writer.WriteNull();
		}
		else
		{
			m_NameSystem.BindName(writer, residenceEntity);
		}
		writer.PropertyName("residenceEntity");
		if (residenceEntity == Entity.Null)
		{
			writer.WriteNull();
		}
		else
		{
			UnityWriters.Write(writer, residenceEntity);
		}
		writer.PropertyName("residenceKey");
		writer.Write(Enum.GetName(typeof(CitizenResidenceKey), residenceKey));
		writer.PropertyName("ageData");
		JsonWriterExtensions.Write<AgeData>(writer, ageData);
		writer.PropertyName("educationData");
		JsonWriterExtensions.Write<EducationData>(writer, educationData);
	}

	private static bool TryCountHouseholds(ref int residentCount, ref int petCount, ref int householdCount, ref int maxHouseholds, Entity entity, Entity prefab, ref ComponentLookup<Game.Buildings.Park> parkLookup, ref ComponentLookup<Abandoned> abandonedLookup, ref ComponentLookup<BuildingPropertyData> propertyDataLookup, ref ComponentLookup<HealthProblem> healthProblemLookup, ref ComponentLookup<TravelPurpose> travelPurposeLookup, ref ComponentLookup<Household> householdLookup, ref BufferLookup<Renter> renterLookup, ref BufferLookup<HouseholdCitizen> householdCitizenLookup, ref BufferLookup<HouseholdAnimal> householdAnimalLookup, NativeList<Entity> householdsResult)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		bool flag = abandonedLookup.HasComponent(entity);
		DynamicBuffer<Renter> val = default(DynamicBuffer<Renter>);
		bool flag2 = renterLookup.TryGetBuffer(entity, ref val) && val.Length > 0;
		bool flag3 = parkLookup.HasComponent(entity);
		BuildingPropertyData buildingPropertyData = default(BuildingPropertyData);
		bool num = propertyDataLookup.TryGetComponent(prefab, ref buildingPropertyData) && buildingPropertyData.m_ResidentialProperties > 0 && !flag;
		bool flag4 = (flag3 || flag) && flag2;
		if (num || flag4)
		{
			result = true;
			maxHouseholds += buildingPropertyData.m_ResidentialProperties;
			DynamicBuffer<HouseholdCitizen> val2 = default(DynamicBuffer<HouseholdCitizen>);
			DynamicBuffer<HouseholdAnimal> val3 = default(DynamicBuffer<HouseholdAnimal>);
			for (int i = 0; i < val.Length; i++)
			{
				Entity renter = val[i].m_Renter;
				if (!householdLookup.HasComponent(renter) || !householdCitizenLookup.TryGetBuffer(renter, ref val2))
				{
					continue;
				}
				householdCount++;
				householdsResult.Add(ref renter);
				for (int j = 0; j < val2.Length; j++)
				{
					if (!CitizenUtils.IsCorpsePickedByHearse(val2[j].m_Citizen, ref healthProblemLookup, ref travelPurposeLookup))
					{
						residentCount++;
					}
				}
				if (householdAnimalLookup.TryGetBuffer(renter, ref val3))
				{
					petCount += val3.Length;
				}
			}
		}
		return result;
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
	public ResidentsSection()
	{
	}
}
