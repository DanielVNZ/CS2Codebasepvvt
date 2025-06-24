using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.UI.Binding;
using Game.Agents;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class EducationInfoviewUISystem : InfoviewUISystemBase
{
	private enum Result
	{
		Uneducated,
		ElementaryEducated,
		HighSchoolEducated,
		CollegeEducated,
		UniversityEducated,
		ElementaryEligible,
		HighSchoolEligible,
		CollegeEligible,
		UniversityEligible,
		ElementaryStudents,
		HighSchoolStudents,
		CollegeStudents,
		UniversityStudents,
		ElementaryCapacity,
		HighSchoolCapacity,
		CollegeCapacity,
		UniversityCapacity,
		Count
	}

	[BurstCompile]
	private struct UpdateEducationDataJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<Household> m_HouseholdHandle;

		[ReadOnly]
		public BufferTypeHandle<HouseholdCitizen> m_HouseholdCitizenHandle;

		[ReadOnly]
		public ComponentLookup<Citizen> m_CitizenFromEntity;

		[ReadOnly]
		public ComponentLookup<HealthProblem> m_HealthProblemFromEntity;

		public NativeArray<int> m_Results;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Household> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Household>(ref m_HouseholdHandle);
			BufferAccessor<HouseholdCitizen> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<HouseholdCitizen>(ref m_HouseholdCitizenHandle);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			Citizen citizen2 = default(Citizen);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				Household household = nativeArray[i];
				DynamicBuffer<HouseholdCitizen> val = bufferAccessor[i];
				if ((household.m_Flags & HouseholdFlags.MovedIn) == 0)
				{
					continue;
				}
				for (int j = 0; j < val.Length; j++)
				{
					Entity citizen = val[j].m_Citizen;
					if (!CitizenUtils.IsDead(citizen, ref m_HealthProblemFromEntity) && m_CitizenFromEntity.TryGetComponent(citizen, ref citizen2))
					{
						switch (citizen2.GetEducationLevel())
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
			}
			ref NativeArray<int> reference = ref m_Results;
			reference[0] = reference[0] + num;
			reference = ref m_Results;
			reference[1] = reference[1] + num2;
			reference = ref m_Results;
			reference[2] = reference[2] + num3;
			reference = ref m_Results;
			reference[3] = reference[3] + num4;
			reference = ref m_Results;
			reference[4] = reference[4] + num5;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct UpdateStudentCountsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityHandle;

		[ReadOnly]
		public BufferTypeHandle<Game.Buildings.Student> m_StudentHandle;

		[ReadOnly]
		public BufferTypeHandle<Efficiency> m_EfficiencyType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefFromEntity;

		[ReadOnly]
		public ComponentLookup<SchoolData> m_SchoolDataFromEntity;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgradeFromEntity;

		public NativeArray<int> m_Results;

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
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityHandle);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefTypeHandle);
			BufferAccessor<Game.Buildings.Student> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Game.Buildings.Student>(ref m_StudentHandle);
			BufferAccessor<Efficiency> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Efficiency>(ref m_EfficiencyType);
			SchoolData data = default(SchoolData);
			DynamicBuffer<InstalledUpgrade> upgrades = default(DynamicBuffer<InstalledUpgrade>);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				Entity prefab = nativeArray2[i].m_Prefab;
				if (BuildingUtils.GetEfficiency(bufferAccessor2, i) != 0f)
				{
					DynamicBuffer<Game.Buildings.Student> val2 = bufferAccessor[i];
					m_SchoolDataFromEntity.TryGetComponent(prefab, ref data);
					if (m_InstalledUpgradeFromEntity.TryGetBuffer(val, ref upgrades))
					{
						UpgradeUtils.CombineStats<SchoolData>(ref data, upgrades, ref m_PrefabRefFromEntity, ref m_SchoolDataFromEntity);
					}
					switch (data.m_EducationLevel)
					{
					case 1:
					{
						ref NativeArray<int> reference = ref m_Results;
						reference[9] = reference[9] + val2.Length;
						reference = ref m_Results;
						reference[13] = reference[13] + data.m_StudentCapacity;
						break;
					}
					case 2:
					{
						ref NativeArray<int> reference = ref m_Results;
						reference[10] = reference[10] + val2.Length;
						reference = ref m_Results;
						reference[14] = reference[14] + data.m_StudentCapacity;
						break;
					}
					case 3:
					{
						ref NativeArray<int> reference = ref m_Results;
						reference[11] = reference[11] + val2.Length;
						reference = ref m_Results;
						reference[15] = reference[15] + data.m_StudentCapacity;
						break;
					}
					case 4:
					{
						ref NativeArray<int> reference = ref m_Results;
						reference[12] = reference[12] + val2.Length;
						reference = ref m_Results;
						reference[16] = reference[16] + data.m_StudentCapacity;
						break;
					}
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
	private struct UpdateEligibilityJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityHandle;

		[ReadOnly]
		public ComponentTypeHandle<Citizen> m_CitizenHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Citizens.Student> m_StudentHandle;

		[ReadOnly]
		public ComponentTypeHandle<Worker> m_WorkerHandle;

		[ReadOnly]
		public ComponentLookup<Household> m_HouseholdFromEntity;

		[ReadOnly]
		public ComponentLookup<HouseholdMember> m_HouseholdMemberFromEntity;

		[ReadOnly]
		public ComponentLookup<HealthProblem> m_HealthProblemFromEntity;

		[ReadOnly]
		public ComponentLookup<MovingAway> m_MovingAways;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenters;

		[ReadOnly]
		public BufferLookup<ServiceFee> m_ServiceFeeFromEntity;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifierFromEntity;

		public uint m_SimulationFrame;

		public Entity m_City;

		public EconomyParameterData m_EconomyParameterData;

		public EducationParameterData m_EducationParameterData;

		public TimeData m_TimeData;

		public NativeArray<int> m_Results;

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
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityHandle);
			NativeArray<Citizen> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Citizen>(ref m_CitizenHandle);
			NativeArray<Game.Citizens.Student> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Citizens.Student>(ref m_StudentHandle);
			NativeArray<Worker> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Worker>(ref m_WorkerHandle);
			_ = m_ServiceFeeFromEntity[m_City];
			DynamicBuffer<CityModifier> cityModifiers = m_CityModifierFromEntity[m_City];
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			HouseholdMember householdMember = default(HouseholdMember);
			Household household = default(Household);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				if (CitizenUtils.IsDead(val, ref m_HealthProblemFromEntity) || !m_HouseholdMemberFromEntity.TryGetComponent(val, ref householdMember) || !m_HouseholdFromEntity.TryGetComponent(householdMember.m_Household, ref household) || (household.m_Flags & HouseholdFlags.MovedIn) == 0 || (household.m_Flags & HouseholdFlags.Tourist) != HouseholdFlags.None || m_MovingAways.HasComponent(householdMember.m_Household) || !m_PropertyRenters.HasComponent(householdMember.m_Household))
				{
					continue;
				}
				if (((ArchetypeChunk)(ref chunk)).Has<Game.Citizens.Student>(ref m_StudentHandle))
				{
					switch (nativeArray3[i].m_Level)
					{
					case 1:
						num += 1f;
						break;
					case 2:
						num2 += 1f;
						break;
					case 3:
						num3 += 1f;
						break;
					case 4:
						num4 += 1f;
						break;
					}
					continue;
				}
				Citizen citizen = nativeArray2[i];
				CitizenAge age = citizen.GetAge();
				Random pseudoRandom = citizen.GetPseudoRandom(CitizenPseudoRandom.StudyWillingness);
				float willingness = ((Random)(ref pseudoRandom)).NextFloat();
				if (age == CitizenAge.Child)
				{
					num += 1f;
					continue;
				}
				if (citizen.GetEducationLevel() == 1 && age <= CitizenAge.Adult)
				{
					num2 += ApplyToSchoolSystem.GetEnteringProbability(age, nativeArray4.IsCreated, 2, citizen.m_WellBeing, willingness, cityModifiers, ref m_EducationParameterData);
					continue;
				}
				int failedEducationCount = citizen.GetFailedEducationCount();
				if (citizen.GetEducationLevel() == 2 && failedEducationCount < 3)
				{
					float enteringProbability = ApplyToSchoolSystem.GetEnteringProbability(age, nativeArray4.IsCreated, 4, citizen.m_WellBeing, willingness, cityModifiers, ref m_EducationParameterData);
					num4 += enteringProbability;
					num3 += (1f - enteringProbability) * ApplyToSchoolSystem.GetEnteringProbability(age, nativeArray4.IsCreated, 3, citizen.m_WellBeing, willingness, cityModifiers, ref m_EducationParameterData);
				}
			}
			ref NativeArray<int> reference = ref m_Results;
			reference[5] = reference[5] + Mathf.CeilToInt(num);
			reference = ref m_Results;
			reference[6] = reference[6] + Mathf.CeilToInt(num2);
			reference = ref m_Results;
			reference[7] = reference[7] + Mathf.CeilToInt(num3);
			reference = ref m_Results;
			reference[8] = reference[8] + Mathf.CeilToInt(num4);
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<Household> __Game_Citizens_Household_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HealthProblem> __Game_Citizens_HealthProblem_RO_ComponentLookup;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Game.Buildings.Student> __Game_Buildings_Student_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Efficiency> __Game_Buildings_Efficiency_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SchoolData> __Game_Prefabs_SchoolData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferLookup;

		[ReadOnly]
		public ComponentTypeHandle<Citizen> __Game_Citizens_Citizen_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Citizens.Student> __Game_Citizens_Student_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Worker> __Game_Citizens_Worker_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<HouseholdMember> __Game_Citizens_HouseholdMember_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Household> __Game_Citizens_Household_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MovingAway> __Game_Agents_MovingAway_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ServiceFee> __Game_City_ServiceFee_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<CityModifier> __Game_City_CityModifier_RO_BufferLookup;

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
			__Game_Citizens_Household_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Household>(true);
			__Game_Citizens_HouseholdCitizen_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<HouseholdCitizen>(true);
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
			__Game_Citizens_HealthProblem_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HealthProblem>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Buildings_Student_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Game.Buildings.Student>(true);
			__Game_Buildings_Efficiency_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Efficiency>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_SchoolData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SchoolData>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<InstalledUpgrade>(true);
			__Game_Citizens_Citizen_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Citizen>(true);
			__Game_Citizens_Student_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Citizens.Student>(true);
			__Game_Citizens_Worker_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Worker>(true);
			__Game_Citizens_HouseholdMember_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HouseholdMember>(true);
			__Game_Citizens_Household_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Household>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
			__Game_Agents_MovingAway_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MovingAway>(true);
			__Game_City_ServiceFee_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceFee>(true);
			__Game_City_CityModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(true);
		}
	}

	private const string kGroup = "educationInfo";

	private SimulationSystem m_SimulationSystem;

	private CitySystem m_CitySystem;

	private RawValueBinding m_EducationData;

	private ValueBinding<int> m_ElementaryStudents;

	private ValueBinding<int> m_HighSchoolStudents;

	private ValueBinding<int> m_CollegeStudents;

	private ValueBinding<int> m_UniversityStudents;

	private ValueBinding<int> m_ElementaryEligible;

	private ValueBinding<int> m_HighSchoolEligible;

	private ValueBinding<int> m_CollegeEligible;

	private ValueBinding<int> m_UniversityEligible;

	private ValueBinding<int> m_ElementaryCapacity;

	private ValueBinding<int> m_HighSchoolCapacity;

	private ValueBinding<int> m_CollegeCapacity;

	private ValueBinding<int> m_UniversityCapacity;

	private GetterValueBinding<IndicatorValue> m_ElementaryAvailability;

	private GetterValueBinding<IndicatorValue> m_HighSchoolAvailability;

	private GetterValueBinding<IndicatorValue> m_CollegeAvailability;

	private GetterValueBinding<IndicatorValue> m_UniversityAvailability;

	private EntityQuery m_HouseholdQuery;

	private EntityQuery m_SchoolQuery;

	private EntityQuery m_SchoolModifiedQuery;

	private EntityQuery m_EligibleQuery;

	private EntityQuery m_TimeDataQuery;

	private NativeArray<int> m_Results;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_607537787_0;

	private EntityQuery __query_607537787_1;

	private EntityQuery __query_607537787_2;

	protected override bool Active
	{
		get
		{
			if (!base.Active && !((EventBindingBase)m_EducationData).active && !((EventBindingBase)m_ElementaryStudents).active && !((EventBindingBase)m_ElementaryCapacity).active && !((EventBindingBase)m_ElementaryEligible).active && !((EventBindingBase)m_ElementaryAvailability).active && !((EventBindingBase)m_HighSchoolStudents).active && !((EventBindingBase)m_HighSchoolCapacity).active && !((EventBindingBase)m_HighSchoolEligible).active && !((EventBindingBase)m_HighSchoolAvailability).active && !((EventBindingBase)m_CollegeStudents).active && !((EventBindingBase)m_CollegeCapacity).active && !((EventBindingBase)m_CollegeEligible).active && !((EventBindingBase)m_CollegeAvailability).active && !((EventBindingBase)m_UniversityStudents).active && !((EventBindingBase)m_UniversityCapacity).active && !((EventBindingBase)m_UniversityEligible).active)
			{
				return ((EventBindingBase)m_UniversityAvailability).active;
			}
			return true;
		}
	}

	protected override bool Modified => !((EntityQuery)(ref m_SchoolModifiedQuery)).IsEmptyIgnoreFilter;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Expected O, but got Unknown
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Expected O, but got Unknown
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Expected O, but got Unknown
		//IL_0204: Expected O, but got Unknown
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		((ComponentSystemBase)this).RequireForUpdate<EconomyParameterData>();
		((ComponentSystemBase)this).RequireForUpdate<TimeData>();
		m_HouseholdQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<Household>(),
			ComponentType.ReadOnly<PropertyRenter>(),
			ComponentType.ReadOnly<HouseholdCitizen>(),
			ComponentType.Exclude<TouristHousehold>(),
			ComponentType.Exclude<CommuterHousehold>(),
			ComponentType.Exclude<MovingAway>()
		});
		m_SchoolQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Game.Buildings.School>(),
			ComponentType.ReadOnly<Game.Buildings.Student>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Game.Buildings.School>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Updated>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array[0] = val;
		m_SchoolModifiedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Citizen>(),
			ComponentType.ReadOnly<UpdateFrame>()
		};
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<HasJobSeeker>(),
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array2[0] = val;
		m_EligibleQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		RawValueBinding val2 = new RawValueBinding("educationInfo", "educationData", (Action<IJsonWriter>)UpdateEducationData);
		RawValueBinding binding = val2;
		m_EducationData = val2;
		AddBinding((IBinding)(object)binding);
		AddBinding((IBinding)(object)(m_ElementaryStudents = new ValueBinding<int>("educationInfo", "elementaryStudentCount", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_HighSchoolStudents = new ValueBinding<int>("educationInfo", "highSchoolStudentCount", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_CollegeStudents = new ValueBinding<int>("educationInfo", "collegeStudentCount", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_UniversityStudents = new ValueBinding<int>("educationInfo", "universityStudentCount", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_ElementaryEligible = new ValueBinding<int>("educationInfo", "elementaryEligible", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_HighSchoolEligible = new ValueBinding<int>("educationInfo", "highSchoolEligible", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_CollegeEligible = new ValueBinding<int>("educationInfo", "collegeEligible", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_UniversityEligible = new ValueBinding<int>("educationInfo", "universityEligible", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_ElementaryCapacity = new ValueBinding<int>("educationInfo", "elementaryCapacity", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_HighSchoolCapacity = new ValueBinding<int>("educationInfo", "highSchoolCapacity", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_CollegeCapacity = new ValueBinding<int>("educationInfo", "collegeCapacity", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_UniversityCapacity = new ValueBinding<int>("educationInfo", "universityCapacity", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_ElementaryAvailability = new GetterValueBinding<IndicatorValue>("educationInfo", "elementaryAvailability", (Func<IndicatorValue>)UpdateElementaryAvailability, (IWriter<IndicatorValue>)(object)new ValueWriter<IndicatorValue>(), (EqualityComparer<IndicatorValue>)null)));
		AddBinding((IBinding)(object)(m_HighSchoolAvailability = new GetterValueBinding<IndicatorValue>("educationInfo", "highSchoolAvailability", (Func<IndicatorValue>)UpdateHighSchoolAvailability, (IWriter<IndicatorValue>)(object)new ValueWriter<IndicatorValue>(), (EqualityComparer<IndicatorValue>)null)));
		AddBinding((IBinding)(object)(m_CollegeAvailability = new GetterValueBinding<IndicatorValue>("educationInfo", "collegeAvailability", (Func<IndicatorValue>)UpdateCollegeAvailability, (IWriter<IndicatorValue>)(object)new ValueWriter<IndicatorValue>(), (EqualityComparer<IndicatorValue>)null)));
		AddBinding((IBinding)(object)(m_UniversityAvailability = new GetterValueBinding<IndicatorValue>("educationInfo", "universityAvailability", (Func<IndicatorValue>)UpdateUniversityAvailability, (IWriter<IndicatorValue>)(object)new ValueWriter<IndicatorValue>(), (EqualityComparer<IndicatorValue>)null)));
		m_Results = new NativeArray<int>(17, (Allocator)4, (NativeArrayOptions)1);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_Results.Dispose();
		base.OnDestroy();
	}

	protected override void PerformUpdate()
	{
		ResetResults();
		UpdateStudentCounts();
		UpdateEligibility();
		m_EducationData.Update();
		m_ElementaryStudents.Update(m_Results[9]);
		m_ElementaryCapacity.Update(m_Results[13]);
		m_ElementaryEligible.Update(m_Results[5]);
		m_HighSchoolStudents.Update(m_Results[10]);
		m_HighSchoolCapacity.Update(m_Results[14]);
		m_HighSchoolEligible.Update(m_Results[6]);
		m_CollegeStudents.Update(m_Results[11]);
		m_CollegeCapacity.Update(m_Results[15]);
		m_CollegeEligible.Update(m_Results[7]);
		m_UniversityStudents.Update(m_Results[12]);
		m_UniversityCapacity.Update(m_Results[16]);
		m_UniversityEligible.Update(m_Results[8]);
		m_ElementaryAvailability.Update();
		m_HighSchoolAvailability.Update();
		m_CollegeAvailability.Update();
		m_UniversityAvailability.Update();
	}

	private void ResetResults()
	{
		for (int i = 0; i < m_Results.Length; i++)
		{
			m_Results[i] = 0;
		}
	}

	private void UpdateEducationData(IJsonWriter binder)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = JobChunkExtensions.Schedule<UpdateEducationDataJob>(new UpdateEducationDataJob
		{
			m_HouseholdHandle = InternalCompilerInterface.GetComponentTypeHandle<Household>(ref __TypeHandle.__Game_Citizens_Household_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdCitizenHandle = InternalCompilerInterface.GetBufferTypeHandle<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CitizenFromEntity = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HealthProblemFromEntity = InternalCompilerInterface.GetComponentLookup<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Results = m_Results
		}, m_HouseholdQuery, ((SystemBase)this).Dependency);
		((JobHandle)(ref val)).Complete();
		InfoviewsUIUtils.UpdateFiveSlicePieChartData(binder, m_Results[0], m_Results[1], m_Results[2], m_Results[3], m_Results[4]);
	}

	private void UpdateStudentCounts()
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
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = JobChunkExtensions.Schedule<UpdateStudentCountsJob>(new UpdateStudentCountsJob
		{
			m_EntityHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_StudentHandle = InternalCompilerInterface.GetBufferTypeHandle<Game.Buildings.Student>(ref __TypeHandle.__Game_Buildings_Student_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EfficiencyType = InternalCompilerInterface.GetBufferTypeHandle<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefFromEntity = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SchoolDataFromEntity = InternalCompilerInterface.GetComponentLookup<SchoolData>(ref __TypeHandle.__Game_Prefabs_SchoolData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgradeFromEntity = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Results = m_Results
		}, m_SchoolQuery, ((SystemBase)this).Dependency);
		((JobHandle)(ref val)).Complete();
	}

	private void UpdateEligibility()
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
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = JobChunkExtensions.Schedule<UpdateEligibilityJob>(new UpdateEligibilityJob
		{
			m_EntityHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CitizenHandle = InternalCompilerInterface.GetComponentTypeHandle<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_StudentHandle = InternalCompilerInterface.GetComponentTypeHandle<Game.Citizens.Student>(ref __TypeHandle.__Game_Citizens_Student_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WorkerHandle = InternalCompilerInterface.GetComponentTypeHandle<Worker>(ref __TypeHandle.__Game_Citizens_Worker_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdMemberFromEntity = InternalCompilerInterface.GetComponentLookup<HouseholdMember>(ref __TypeHandle.__Game_Citizens_HouseholdMember_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdFromEntity = InternalCompilerInterface.GetComponentLookup<Household>(ref __TypeHandle.__Game_Citizens_Household_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HealthProblemFromEntity = InternalCompilerInterface.GetComponentLookup<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyRenters = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MovingAways = InternalCompilerInterface.GetComponentLookup<MovingAway>(ref __TypeHandle.__Game_Agents_MovingAway_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceFeeFromEntity = InternalCompilerInterface.GetBufferLookup<ServiceFee>(ref __TypeHandle.__Game_City_ServiceFee_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CityModifierFromEntity = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SimulationFrame = m_SimulationSystem.frameIndex,
			m_City = m_CitySystem.City,
			m_EconomyParameterData = ((EntityQuery)(ref __query_607537787_0)).GetSingleton<EconomyParameterData>(),
			m_EducationParameterData = ((EntityQuery)(ref __query_607537787_1)).GetSingleton<EducationParameterData>(),
			m_TimeData = ((EntityQuery)(ref __query_607537787_2)).GetSingleton<TimeData>(),
			m_Results = m_Results
		}, m_EligibleQuery, ((SystemBase)this).Dependency);
		((JobHandle)(ref val)).Complete();
	}

	private IndicatorValue UpdateElementaryAvailability()
	{
		return IndicatorValue.Calculate(m_ElementaryCapacity.value, m_ElementaryEligible.value);
	}

	private IndicatorValue UpdateHighSchoolAvailability()
	{
		return IndicatorValue.Calculate(m_HighSchoolCapacity.value, m_HighSchoolEligible.value);
	}

	private IndicatorValue UpdateCollegeAvailability()
	{
		return IndicatorValue.Calculate(m_CollegeCapacity.value, m_CollegeEligible.value);
	}

	private IndicatorValue UpdateUniversityAvailability()
	{
		return IndicatorValue.Calculate(m_UniversityCapacity.value, m_UniversityEligible.value);
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
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<EconomyParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_607537787_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		val2 = ((EntityQueryBuilder)(ref val)).WithAll<EducationParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_607537787_1 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		val2 = ((EntityQueryBuilder)(ref val)).WithAll<TimeData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_607537787_2 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
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
	public EducationInfoviewUISystem()
	{
	}
}
