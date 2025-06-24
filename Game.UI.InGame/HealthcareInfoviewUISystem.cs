using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Collections;
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
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class HealthcareInfoviewUISystem : InfoviewUISystemBase
{
	private enum Result
	{
		CitizenHealth,
		CitizenCount,
		SickCitizens,
		PatientCount,
		PatientCapacity,
		ProcessingRate,
		CemeteryUse,
		CemeteryCapacity,
		ResultCount
	}

	[BurstCompile]
	public struct CalculateAverageHealthJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<Household> m_HouseholdType;

		[ReadOnly]
		public BufferTypeHandle<HouseholdCitizen> m_HouseholdCitizenType;

		[ReadOnly]
		public ComponentLookup<Citizen> m_Citizens;

		[ReadOnly]
		public ComponentLookup<HealthProblem> m_HealthProblems;

		public NativeArray<float> m_Results;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Household> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Household>(ref m_HouseholdType);
			BufferAccessor<HouseholdCitizen> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<HouseholdCitizen>(ref m_HouseholdCitizenType);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			HealthProblem healthProblem = default(HealthProblem);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				if ((nativeArray[i].m_Flags & HouseholdFlags.MovedIn) == 0)
				{
					continue;
				}
				DynamicBuffer<HouseholdCitizen> val = bufferAccessor[i];
				for (int j = 0; j < val.Length; j++)
				{
					Entity citizen = val[j].m_Citizen;
					if (!CitizenUtils.IsDead(citizen, ref m_HealthProblems) && CitizenUtils.TryGetResident(citizen, m_Citizens, out var citizen2))
					{
						if (m_HealthProblems.TryGetComponent(citizen, ref healthProblem) && (healthProblem.m_Flags & (HealthProblemFlags.Sick | HealthProblemFlags.Injured)) != HealthProblemFlags.None)
						{
							num3++;
						}
						num2 += citizen2.m_Health;
						num++;
					}
				}
			}
			ref NativeArray<float> reference = ref m_Results;
			reference[0] = reference[0] + (float)num2;
			reference = ref m_Results;
			reference[1] = reference[1] + (float)num;
			reference = ref m_Results;
			reference[2] = reference[2] + (float)num3;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct UpdateHealthcareJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<Patient> m_PatientType;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> m_InstalledUpgradeType;

		[ReadOnly]
		public BufferTypeHandle<Efficiency> m_EfficiencyType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<HospitalData> m_HospitalDatas;

		public NativeArray<float> m_Result;

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
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			BufferAccessor<Patient> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Patient>(ref m_PatientType);
			NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<InstalledUpgrade> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<InstalledUpgrade>(ref m_InstalledUpgradeType);
			BufferAccessor<Efficiency> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Efficiency>(ref m_EfficiencyType);
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				PrefabRef prefabRef = nativeArray[i];
				DynamicBuffer<Patient> val = bufferAccessor[i];
				if (BuildingUtils.GetEfficiency(bufferAccessor3, i) != 0f)
				{
					HospitalData data = default(HospitalData);
					if (m_HospitalDatas.HasComponent(prefabRef.m_Prefab))
					{
						data = m_HospitalDatas[prefabRef.m_Prefab];
					}
					if (bufferAccessor2.Length != 0)
					{
						UpgradeUtils.CombineStats<HospitalData>(ref data, bufferAccessor2[i], ref m_Prefabs, ref m_HospitalDatas);
					}
					num += val.Length;
					num2 += data.m_PatientCapacity;
				}
			}
			ref NativeArray<float> reference = ref m_Result;
			reference[3] = reference[3] + (float)num;
			reference = ref m_Result;
			reference[4] = reference[4] + (float)num2;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct UpdateDeathcareJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.DeathcareFacility> m_DeathcareFacilityType;

		[ReadOnly]
		public BufferTypeHandle<Efficiency> m_EfficiencyType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> m_InstalledUpgradeType;

		[ReadOnly]
		public ComponentLookup<DeathcareFacilityData> m_DeathcareFacilities;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		public NativeArray<float> m_Results;

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
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Game.Buildings.DeathcareFacility> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.DeathcareFacility>(ref m_DeathcareFacilityType);
			BufferAccessor<Efficiency> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Efficiency>(ref m_EfficiencyType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<InstalledUpgrade> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<InstalledUpgrade>(ref m_InstalledUpgradeType);
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity prefab = nativeArray2[i].m_Prefab;
				float efficiency = BuildingUtils.GetEfficiency(bufferAccessor, i);
				if (efficiency != 0f)
				{
					DeathcareFacilityData data = default(DeathcareFacilityData);
					if (m_DeathcareFacilities.HasComponent(prefab))
					{
						data = m_DeathcareFacilities[prefab];
					}
					if (bufferAccessor2.Length != 0)
					{
						UpgradeUtils.CombineStats<DeathcareFacilityData>(ref data, bufferAccessor2[i], ref m_Prefabs, ref m_DeathcareFacilities);
					}
					if (data.m_LongTermStorage)
					{
						num2 += (float)nativeArray[i].m_LongTermStoredCount;
						num3 += (float)data.m_StorageCapacity;
					}
					num += efficiency * data.m_ProcessingRate;
				}
			}
			ref NativeArray<float> reference = ref m_Results;
			reference[5] = reference[5] + num;
			reference = ref m_Results;
			reference[6] = reference[6] + num2;
			reference = ref m_Results;
			reference[7] = reference[7] + num3;
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
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Patient> __Game_Buildings_Patient_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Efficiency> __Game_Buildings_Efficiency_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HospitalData> __Game_Prefabs_HospitalData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.DeathcareFacility> __Game_Buildings_DeathcareFacility_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<DeathcareFacilityData> __Game_Prefabs_DeathcareFacilityData_RO_ComponentLookup;

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
			__Game_Citizens_Household_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Household>(true);
			__Game_Citizens_HouseholdCitizen_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<HouseholdCitizen>(true);
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
			__Game_Citizens_HealthProblem_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HealthProblem>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Buildings_Patient_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Patient>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<InstalledUpgrade>(true);
			__Game_Buildings_Efficiency_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Efficiency>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_HospitalData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HospitalData>(true);
			__Game_Buildings_DeathcareFacility_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.DeathcareFacility>(true);
			__Game_Prefabs_DeathcareFacilityData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<DeathcareFacilityData>(true);
		}
	}

	private const string kGroup = "healthcareInfo";

	private CityStatisticsSystem m_CityStatisticsSystem;

	private ValueBinding<float> m_AverageHealth;

	private ValueBinding<int> m_PatientCount;

	private ValueBinding<int> m_SickCount;

	private ValueBinding<int> m_PatientCapacity;

	private ValueBinding<float> m_DeathRate;

	private ValueBinding<float> m_ProcessingRate;

	private ValueBinding<int> m_CemeteryUse;

	private ValueBinding<int> m_CemeteryCapacity;

	private GetterValueBinding<IndicatorValue> m_HealthcareAvailability;

	private GetterValueBinding<IndicatorValue> m_DeathcareAvailability;

	private GetterValueBinding<IndicatorValue> m_CemeteryAvailability;

	private EntityQuery m_HouseholdQuery;

	private EntityQuery m_DeathcareFacilityQuery;

	private EntityQuery m_HealthcareFacilityQuery;

	private EntityQuery m_DeathcareFacilityModifiedQuery;

	private EntityQuery m_HealthcareFacilityModifiedQuery;

	private NativeArray<float> m_Results;

	private TypeHandle __TypeHandle;

	protected override bool Active
	{
		get
		{
			if (!base.Active && !((EventBindingBase)m_AverageHealth).active && !((EventBindingBase)m_PatientCount).active && !((EventBindingBase)m_SickCount).active && !((EventBindingBase)m_PatientCapacity).active && !((EventBindingBase)m_HealthcareAvailability).active && !((EventBindingBase)m_DeathRate).active && !((EventBindingBase)m_ProcessingRate).active && !((EventBindingBase)m_CemeteryUse).active && !((EventBindingBase)m_CemeteryCapacity).active && !((EventBindingBase)m_DeathcareAvailability).active)
			{
				return ((EventBindingBase)m_CemeteryAvailability).active;
			}
			return true;
		}
	}

	protected override bool Modified
	{
		get
		{
			if (((EntityQuery)(ref m_DeathcareFacilityModifiedQuery)).IsEmptyIgnoreFilter)
			{
				return !((EntityQuery)(ref m_HealthcareFacilityModifiedQuery)).IsEmptyIgnoreFilter;
			}
			return true;
		}
	}

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
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Expected O, but got Unknown
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Expected O, but got Unknown
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_HouseholdQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<Household>(),
			ComponentType.ReadOnly<HouseholdCitizen>(),
			ComponentType.ReadOnly<PropertyRenter>(),
			ComponentType.Exclude<CommuterHousehold>(),
			ComponentType.Exclude<TouristHousehold>(),
			ComponentType.Exclude<MovingAway>()
		});
		m_DeathcareFacilityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[7]
		{
			ComponentType.ReadOnly<Game.Buildings.DeathcareFacility>(),
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<ServiceDispatch>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<Patient>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Game.Buildings.DeathcareFacility>(),
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<ServiceDispatch>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<Patient>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Updated>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array[0] = val;
		m_DeathcareFacilityModifiedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_HealthcareFacilityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[7]
		{
			ComponentType.ReadOnly<Game.Buildings.Hospital>(),
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<ServiceDispatch>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<Patient>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Game.Buildings.Hospital>(),
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<ServiceDispatch>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<Patient>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Updated>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array2[0] = val;
		m_HealthcareFacilityModifiedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
		AddBinding((IBinding)(object)(m_AverageHealth = new ValueBinding<float>("healthcareInfo", "averageHealth", 0f, (IWriter<float>)null, (EqualityComparer<float>)null)));
		AddBinding((IBinding)(object)(m_DeathRate = new ValueBinding<float>("healthcareInfo", "deathRate", 0f, (IWriter<float>)null, (EqualityComparer<float>)null)));
		AddBinding((IBinding)(object)(m_ProcessingRate = new ValueBinding<float>("healthcareInfo", "processingRate", 0f, (IWriter<float>)null, (EqualityComparer<float>)null)));
		AddBinding((IBinding)(object)(m_CemeteryUse = new ValueBinding<int>("healthcareInfo", "cemeteryUse", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_CemeteryCapacity = new ValueBinding<int>("healthcareInfo", "cemeteryCapacity", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_SickCount = new ValueBinding<int>("healthcareInfo", "sickCount", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_PatientCount = new ValueBinding<int>("healthcareInfo", "patientCount", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_PatientCapacity = new ValueBinding<int>("healthcareInfo", "patientCapacity", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_HealthcareAvailability = new GetterValueBinding<IndicatorValue>("healthcareInfo", "healthcareAvailability", (Func<IndicatorValue>)GetHealthcareAvailability, (IWriter<IndicatorValue>)(object)new ValueWriter<IndicatorValue>(), (EqualityComparer<IndicatorValue>)null)));
		AddBinding((IBinding)(object)(m_DeathcareAvailability = new GetterValueBinding<IndicatorValue>("healthcareInfo", "deathcareAvailability", (Func<IndicatorValue>)GetDeathcareAvailability, (IWriter<IndicatorValue>)(object)new ValueWriter<IndicatorValue>(), (EqualityComparer<IndicatorValue>)null)));
		AddBinding((IBinding)(object)(m_CemeteryAvailability = new GetterValueBinding<IndicatorValue>("healthcareInfo", "cemeteryAvailability", (Func<IndicatorValue>)GetCemeteryAvailability, (IWriter<IndicatorValue>)(object)new ValueWriter<IndicatorValue>(), (EqualityComparer<IndicatorValue>)null)));
		m_Results = new NativeArray<float>(8, (Allocator)4, (NativeArrayOptions)1);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_Results.Dispose();
		base.OnDestroy();
	}

	protected override void PerformUpdate()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		CollectionUtils.Fill<float>(m_Results, 0f);
		JobHandle val = JobChunkExtensions.Schedule<CalculateAverageHealthJob>(new CalculateAverageHealthJob
		{
			m_HouseholdType = InternalCompilerInterface.GetComponentTypeHandle<Household>(ref __TypeHandle.__Game_Citizens_Household_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdCitizenType = InternalCompilerInterface.GetBufferTypeHandle<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Citizens = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HealthProblems = InternalCompilerInterface.GetComponentLookup<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Results = m_Results
		}, m_HouseholdQuery, ((SystemBase)this).Dependency);
		((JobHandle)(ref val)).Complete();
		val = JobChunkExtensions.Schedule<UpdateHealthcareJob>(new UpdateHealthcareJob
		{
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PatientType = InternalCompilerInterface.GetBufferTypeHandle<Patient>(ref __TypeHandle.__Game_Buildings_Patient_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgradeType = InternalCompilerInterface.GetBufferTypeHandle<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EfficiencyType = InternalCompilerInterface.GetBufferTypeHandle<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HospitalDatas = InternalCompilerInterface.GetComponentLookup<HospitalData>(ref __TypeHandle.__Game_Prefabs_HospitalData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Result = m_Results
		}, m_HealthcareFacilityQuery, ((SystemBase)this).Dependency);
		((JobHandle)(ref val)).Complete();
		val = JobChunkExtensions.Schedule<UpdateDeathcareJob>(new UpdateDeathcareJob
		{
			m_DeathcareFacilityType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.DeathcareFacility>(ref __TypeHandle.__Game_Buildings_DeathcareFacility_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EfficiencyType = InternalCompilerInterface.GetBufferTypeHandle<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgradeType = InternalCompilerInterface.GetBufferTypeHandle<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DeathcareFacilities = InternalCompilerInterface.GetComponentLookup<DeathcareFacilityData>(ref __TypeHandle.__Game_Prefabs_DeathcareFacilityData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Results = m_Results
		}, m_DeathcareFacilityQuery, ((SystemBase)this).Dependency);
		((JobHandle)(ref val)).Complete();
		float num = m_Results[0];
		float num2 = m_Results[1];
		m_AverageHealth.Update(math.round(num / math.max(num2, 1f)));
		m_PatientCount.Update((int)m_Results[3]);
		m_SickCount.Update((int)m_Results[2]);
		m_PatientCapacity.Update((int)m_Results[4]);
		m_DeathRate.Update((float)m_CityStatisticsSystem.GetStatisticValue(StatisticType.DeathRate));
		m_ProcessingRate.Update(m_Results[5]);
		m_CemeteryUse.Update((int)m_Results[6]);
		m_CemeteryCapacity.Update((int)m_Results[7]);
		m_DeathcareAvailability.Update();
		m_CemeteryAvailability.Update();
		m_HealthcareAvailability.Update();
	}

	private IndicatorValue GetHealthcareAvailability()
	{
		return IndicatorValue.Calculate(m_PatientCapacity.value, m_SickCount.value);
	}

	private IndicatorValue GetDeathcareAvailability()
	{
		return IndicatorValue.Calculate(m_ProcessingRate.value, m_DeathRate.value);
	}

	private IndicatorValue GetCemeteryAvailability()
	{
		return IndicatorValue.Calculate(m_CemeteryCapacity.value, m_CemeteryUse.value);
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
	public HealthcareInfoviewUISystem()
	{
	}
}
