using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.Citizens;
using Game.Economy;
using Game.Prefabs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class HouseholdSidebarSection : InfoSectionBase
{
	public enum Result
	{
		Visible,
		ResidentCount,
		Type,
		ResultCount
	}

	private enum HouseholdSidebarVariant
	{
		Citizen,
		Household,
		Building
	}

	[BurstCompile]
	public struct CheckVisibilityJob : IJob
	{
		[ReadOnly]
		public Entity m_SelectedEntity;

		[ReadOnly]
		public Entity m_SelectedPrefab;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingLookup;

		[ReadOnly]
		public ComponentLookup<Abandoned> m_AbandonedLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Park> m_ParkFromLookup;

		[ReadOnly]
		public ComponentLookup<Household> m_HouseholdLookup;

		[ReadOnly]
		public ComponentLookup<Citizen> m_CitizenLookup;

		[ReadOnly]
		public ComponentLookup<HouseholdPet> m_HouseholdPetLookup;

		[ReadOnly]
		public ComponentLookup<HealthProblem> m_HealthProblemLookup;

		[ReadOnly]
		public ComponentLookup<TravelPurpose> m_TravelPurposeLookup;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> m_PropertyDataLookup;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> m_HouseholdCitizenLookup;

		[ReadOnly]
		public BufferLookup<Renter> m_RenterLookup;

		public NativeArray<int> m_Results;

		public void Execute()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			int residentCount = 0;
			int householdCount = 0;
			HouseholdPet householdPet = default(HouseholdPet);
			if (m_CitizenLookup.HasComponent(m_SelectedEntity) || (m_HouseholdPetLookup.TryGetComponent(m_SelectedEntity, ref householdPet) && householdPet.m_Household != Entity.Null))
			{
				m_Results[0] = 1;
				m_Results[1] = 1;
				m_Results[2] = 0;
			}
			else if (m_BuildingLookup.HasComponent(m_SelectedEntity) && HasResidentialProperties(ref residentCount, ref householdCount, m_SelectedEntity, m_SelectedPrefab))
			{
				m_Results[0] = 1;
				m_Results[1] = residentCount;
				m_Results[2] = 2;
			}
			else
			{
				DynamicBuffer<HouseholdCitizen> val = default(DynamicBuffer<HouseholdCitizen>);
				if (!m_HouseholdLookup.HasComponent(m_SelectedEntity) || !m_HouseholdCitizenLookup.TryGetBuffer(m_SelectedEntity, ref val))
				{
					return;
				}
				for (int i = 0; i < val.Length; i++)
				{
					if (!CitizenUtils.IsCorpsePickedByHearse(val[i].m_Citizen, ref m_HealthProblemLookup, ref m_TravelPurposeLookup))
					{
						residentCount++;
					}
				}
				m_Results[0] = 1;
				m_Results[1] = residentCount;
				m_Results[2] = 1;
			}
		}

		private bool HasResidentialProperties(ref int residentCount, ref int householdCount, Entity entity, Entity prefab)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			bool result = false;
			bool flag = m_AbandonedLookup.HasComponent(entity);
			DynamicBuffer<Renter> val = default(DynamicBuffer<Renter>);
			bool flag2 = m_RenterLookup.TryGetBuffer(entity, ref val) && val.Length > 0;
			bool flag3 = m_ParkFromLookup.HasComponent(entity);
			BuildingPropertyData buildingPropertyData = default(BuildingPropertyData);
			bool num = !flag && m_PropertyDataLookup.TryGetComponent(prefab, ref buildingPropertyData) && buildingPropertyData.m_ResidentialProperties > 0;
			bool flag4 = (flag3 || flag) && flag2;
			if (num || flag4)
			{
				result = true;
				DynamicBuffer<HouseholdCitizen> val2 = default(DynamicBuffer<HouseholdCitizen>);
				for (int i = 0; i < val.Length; i++)
				{
					Entity renter = val[i].m_Renter;
					if (!m_HouseholdCitizenLookup.TryGetBuffer(renter, ref val2))
					{
						continue;
					}
					householdCount++;
					for (int j = 0; j < val2.Length; j++)
					{
						if (!CitizenUtils.IsCorpsePickedByHearse(val2[j].m_Citizen, ref m_HealthProblemLookup, ref m_TravelPurposeLookup))
						{
							residentCount++;
						}
					}
				}
			}
			return result;
		}
	}

	[BurstCompile]
	private struct CollectDataJob : IJob
	{
		[ReadOnly]
		public Entity m_SelectedEntity;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingLookup;

		[ReadOnly]
		public ComponentLookup<Household> m_HouseholdLookup;

		[ReadOnly]
		public ComponentLookup<HomelessHousehold> m_HomelessHouseholdLookup;

		[ReadOnly]
		public ComponentLookup<HouseholdMember> m_HouseholdMemberLookup;

		[ReadOnly]
		public ComponentLookup<Citizen> m_CitizenLookup;

		[ReadOnly]
		public ComponentLookup<HouseholdPet> m_HouseholdPetLookup;

		[ReadOnly]
		public ComponentLookup<HealthProblem> m_HealthProblemLookup;

		[ReadOnly]
		public ComponentLookup<TravelPurpose> m_TravelPurposeLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenterLookup;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> m_HouseholdCitizenLookup;

		[ReadOnly]
		public BufferLookup<Resources> m_ResourcesLookup;

		[ReadOnly]
		public BufferLookup<HouseholdAnimal> m_HouseholdAnimalsLookup;

		[ReadOnly]
		public BufferLookup<Renter> m_RenterLookup;

		public NativeArray<Entity> m_ResidenceResult;

		public NativeArray<HouseholdResult> m_HouseholdResult;

		public NativeList<HouseholdResult> m_HouseholdsResult;

		public NativeList<ResidentResult> m_ResidentsResult;

		public NativeList<Entity> m_PetsResult;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			HouseholdPet householdPet = default(HouseholdPet);
			if (m_HouseholdPetLookup.TryGetComponent(m_SelectedEntity, ref householdPet))
			{
				Entity household = householdPet.m_Household;
				m_HouseholdResult[0] = AddHousehold(household);
				PropertyRenter propertyRenter = default(PropertyRenter);
				HomelessHousehold homelessHousehold = default(HomelessHousehold);
				if (m_PropertyRenterLookup.TryGetComponent(household, ref propertyRenter))
				{
					m_ResidenceResult[0] = propertyRenter.m_Property;
				}
				else if (m_HomelessHouseholdLookup.TryGetComponent(household, ref homelessHousehold))
				{
					m_ResidenceResult[0] = homelessHousehold.m_TempHome;
				}
			}
			else if (m_CitizenLookup.HasComponent(m_SelectedEntity))
			{
				Entity household2 = m_HouseholdMemberLookup[m_SelectedEntity].m_Household;
				m_HouseholdResult[0] = AddHousehold(household2);
				PropertyRenter propertyRenter2 = default(PropertyRenter);
				HomelessHousehold homelessHousehold2 = default(HomelessHousehold);
				if (m_PropertyRenterLookup.TryGetComponent(household2, ref propertyRenter2))
				{
					m_ResidenceResult[0] = propertyRenter2.m_Property;
				}
				else if (m_HomelessHouseholdLookup.TryGetComponent(household2, ref homelessHousehold2))
				{
					m_ResidenceResult[0] = homelessHousehold2.m_TempHome;
				}
			}
			else if (m_BuildingLookup.HasComponent(m_SelectedEntity))
			{
				DynamicBuffer<Renter> val = m_RenterLookup[m_SelectedEntity];
				m_ResidenceResult[0] = m_SelectedEntity;
				for (int i = 0; i < val.Length; i++)
				{
					Entity renter = val[i].m_Renter;
					HouseholdResult householdResult = AddHousehold(renter);
					if (i == 0)
					{
						m_HouseholdResult[0] = householdResult;
					}
				}
			}
			else
			{
				m_HouseholdResult[0] = AddHousehold(m_SelectedEntity);
				PropertyRenter propertyRenter3 = default(PropertyRenter);
				HomelessHousehold homelessHousehold3 = default(HomelessHousehold);
				if (m_PropertyRenterLookup.TryGetComponent(m_SelectedEntity, ref propertyRenter3))
				{
					m_ResidenceResult[0] = propertyRenter3.m_Property;
				}
				else if (m_HomelessHouseholdLookup.TryGetComponent(m_SelectedEntity, ref homelessHousehold3))
				{
					m_ResidenceResult[0] = homelessHousehold3.m_TempHome;
				}
			}
		}

		private HouseholdResult AddHousehold(Entity householdEntity)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			HouseholdResult result = default(HouseholdResult);
			Household householdData = default(Household);
			DynamicBuffer<HouseholdCitizen> val = default(DynamicBuffer<HouseholdCitizen>);
			DynamicBuffer<Resources> resources = default(DynamicBuffer<Resources>);
			if (m_HouseholdLookup.TryGetComponent(householdEntity, ref householdData) && m_HouseholdCitizenLookup.TryGetBuffer(householdEntity, ref val) && m_ResourcesLookup.TryGetBuffer(householdEntity, ref resources))
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				Citizen citizen2 = default(Citizen);
				for (int i = 0; i < val.Length; i++)
				{
					Entity citizen = val[i].m_Citizen;
					if (m_CitizenLookup.TryGetComponent(citizen, ref citizen2) && !CitizenUtils.IsCorpsePickedByHearse(citizen, ref m_HealthProblemLookup, ref m_TravelPurposeLookup))
					{
						int happiness = citizen2.Happiness;
						CitizenAge age = citizen2.GetAge();
						int educationLevel = citizen2.GetEducationLevel();
						ref NativeList<ResidentResult> reference = ref m_ResidentsResult;
						ResidentResult residentResult = new ResidentResult
						{
							m_Entity = citizen,
							m_Age = age,
							m_Happiness = happiness,
							m_Education = educationLevel
						};
						reference.Add(ref residentResult);
						num++;
						num3 += happiness;
						num4 = (int)(num4 + age);
						if (age == CitizenAge.Adult || age == CitizenAge.Teen)
						{
							num2++;
							num5 += citizen2.GetEducationLevel();
						}
					}
				}
				DynamicBuffer<HouseholdAnimal> val2 = default(DynamicBuffer<HouseholdAnimal>);
				if (m_HouseholdAnimalsLookup.TryGetBuffer(householdEntity, ref val2))
				{
					for (int j = 0; j < val2.Length; j++)
					{
						ref NativeList<Entity> reference2 = ref m_PetsResult;
						HouseholdAnimal householdAnimal = val2[j];
						reference2.Add(ref householdAnimal.m_HouseholdPet);
					}
				}
				result = new HouseholdResult
				{
					m_Entity = householdEntity,
					m_Members = num,
					m_Age = num4 / math.max(num, 1),
					m_Happiness = num3 / math.max(num, 1),
					m_Education = num5 / math.max(num2, 1),
					m_Wealth = EconomyUtils.GetHouseholdTotalWealth(householdData, resources)
				};
				if (result.m_Members > 0)
				{
					m_HouseholdsResult.Add(ref result);
				}
			}
			return result;
		}
	}

	public struct HouseholdResult : IComparable<HouseholdResult>, IEquatable<HouseholdResult>
	{
		public Entity m_Entity;

		public int m_Members;

		public int m_Age;

		public int m_Education;

		public int m_Happiness;

		public int m_Wealth;

		public int CompareTo(HouseholdResult other)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			int num = other.m_Members.CompareTo(m_Members);
			if (num == 0)
			{
				num = ((Entity)(ref other.m_Entity)).CompareTo(m_Entity);
			}
			return num;
		}

		public bool Equals(HouseholdResult other)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			if (((Entity)(ref m_Entity)).Equals(other.m_Entity))
			{
				return m_Members == other.m_Members;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is HouseholdResult other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return HashCode.Combine<Entity, int>(m_Entity, m_Members);
		}
	}

	public class HouseholdComparer : IComparer<HouseholdResult>
	{
		public enum CompareBy
		{
			Members,
			Age,
			Education,
			Happiness,
			Wealth
		}

		private readonly Func<HouseholdResult, HouseholdResult, int> m_Compare;

		public HouseholdComparer(CompareBy compareBy)
		{
			m_Compare = compareBy switch
			{
				CompareBy.Members => (HouseholdResult x, HouseholdResult y) => y.m_Members.CompareTo(x.m_Members), 
				CompareBy.Age => (HouseholdResult x, HouseholdResult y) => y.m_Age.CompareTo(x.m_Age), 
				CompareBy.Education => (HouseholdResult x, HouseholdResult y) => y.m_Education.CompareTo(x.m_Education), 
				CompareBy.Wealth => (HouseholdResult x, HouseholdResult y) => y.m_Wealth.CompareTo(x.m_Wealth), 
				_ => (HouseholdResult x, HouseholdResult y) => 0, 
			};
		}

		public int Compare(HouseholdResult x, HouseholdResult y)
		{
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			int num = m_Compare?.Invoke(x, y) ?? 0;
			if (num == 0)
			{
				num = y.m_Members.CompareTo(x.m_Members);
			}
			if (num == 0)
			{
				num = y.m_Age.CompareTo(x.m_Age);
			}
			if (num == 0)
			{
				num = y.m_Education.CompareTo(x.m_Education);
			}
			if (num == 0)
			{
				num = y.m_Wealth.CompareTo(x.m_Wealth);
			}
			if (num == 0)
			{
				num = ((Entity)(ref y.m_Entity)).CompareTo(x.m_Entity);
			}
			return num;
		}
	}

	public struct ResidentResult : IComparable<ResidentResult>, IEquatable<ResidentResult>
	{
		public Entity m_Entity;

		public CitizenAge m_Age;

		public int m_Education;

		public int m_Happiness;

		public int CompareTo(ResidentResult other)
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			int num = other.m_Age.CompareTo(m_Age);
			if (num == 0)
			{
				num = other.m_Education.CompareTo(m_Education);
			}
			if (num == 0)
			{
				num = ((Entity)(ref other.m_Entity)).CompareTo(m_Entity);
			}
			return num;
		}

		public bool Equals(ResidentResult other)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			if (((Entity)(ref m_Entity)).Equals(other.m_Entity))
			{
				return m_Age == other.m_Age;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is ResidentResult other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return HashCode.Combine<Entity, int>(m_Entity, (int)m_Age);
		}
	}

	public class ResidentComparer : IComparer<ResidentResult>
	{
		public enum CompareBy
		{
			Age,
			Education,
			Happiness
		}

		private readonly Func<ResidentResult, ResidentResult, int> m_Compare;

		public ResidentComparer(CompareBy compareBy)
		{
			m_Compare = compareBy switch
			{
				CompareBy.Age => (ResidentResult x, ResidentResult y) => y.m_Age.CompareTo(x.m_Age), 
				CompareBy.Education => (ResidentResult x, ResidentResult y) => y.m_Education.CompareTo(x.m_Education), 
				_ => (ResidentResult x, ResidentResult y) => 0, 
			};
		}

		public int Compare(ResidentResult x, ResidentResult y)
		{
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			int num = m_Compare?.Invoke(x, y) ?? 0;
			if (num == 0)
			{
				num = y.m_Age.CompareTo(x.m_Age);
			}
			if (num == 0)
			{
				num = y.m_Education.CompareTo(x.m_Education);
			}
			if (num == 0)
			{
				num = ((Entity)(ref y.m_Entity)).CompareTo(x.m_Entity);
			}
			return num;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<Game.Buildings.Park> __Game_Buildings_Park_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Abandoned> __Game_Buildings_Abandoned_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Household> __Game_Citizens_Household_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HouseholdPet> __Game_Citizens_HouseholdPet_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HealthProblem> __Game_Citizens_HealthProblem_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TravelPurpose> __Game_Citizens_TravelPurpose_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> __Game_Prefabs_BuildingPropertyData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Renter> __Game_Buildings_Renter_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<HomelessHousehold> __Game_Citizens_HomelessHousehold_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HouseholdMember> __Game_Citizens_HouseholdMember_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Resources> __Game_Economy_Resources_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<HouseholdAnimal> __Game_Citizens_HouseholdAnimal_RO_BufferLookup;

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
			__Game_Buildings_Park_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.Park>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Buildings_Abandoned_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Abandoned>(true);
			__Game_Citizens_Household_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Household>(true);
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
			__Game_Citizens_HouseholdPet_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HouseholdPet>(true);
			__Game_Citizens_HealthProblem_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HealthProblem>(true);
			__Game_Citizens_TravelPurpose_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TravelPurpose>(true);
			__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingPropertyData>(true);
			__Game_Citizens_HouseholdCitizen_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdCitizen>(true);
			__Game_Buildings_Renter_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Renter>(true);
			__Game_Citizens_HomelessHousehold_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HomelessHousehold>(true);
			__Game_Citizens_HouseholdMember_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HouseholdMember>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
			__Game_Economy_Resources_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Resources>(true);
			__Game_Citizens_HouseholdAnimal_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdAnimal>(true);
		}
	}

	private const string kHouseholdIcon = "Media/Game/Icons/Household.svg";

	private const string kResidenceIcon = "Media/Glyphs/Residence.svg";

	private const string kHomelessShelterIcon = "Media/Glyphs/HomelessShelter.svg";

	private const string kPetIcon = "Media/Game/Icons/Pet.svg";

	private const string kItemType = "Game.UI.InGame.HouseholdSidebarSection+HouseholdSidebarItem";

	private NativeArray<int> m_Results;

	private NativeArray<Entity> m_ResidenceResult;

	private NativeArray<HouseholdResult> m_HouseholdResult;

	private NativeList<ResidentResult> m_ResidentsResult;

	private NativeList<Entity> m_PetsResult;

	private NativeList<HouseholdResult> m_HouseholdsResult;

	private RawMapBinding<int> m_HouseholdMap;

	private RawMapBinding<int> m_ResidentMap;

	private RawMapBinding<int> m_PetMap;

	private TypeHandle __TypeHandle;

	protected override string group => "HouseholdSidebarSection";

	protected override bool displayForDestroyedObjects => true;

	protected override bool displayForUnderConstruction => true;

	private Entity residenceEntity { get; set; }

	private HouseholdResult household { get; set; }

	private HouseholdSidebarVariant variant { get; set; }

	private bool residenceIsHomelessShelter { get; set; }

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ResidentsResult = new NativeList<ResidentResult>(AllocatorHandle.op_Implicit((Allocator)4));
		m_PetsResult = new NativeList<Entity>(AllocatorHandle.op_Implicit((Allocator)4));
		m_HouseholdsResult = new NativeList<HouseholdResult>(AllocatorHandle.op_Implicit((Allocator)4));
		m_Results = new NativeArray<int>(3, (Allocator)4, (NativeArrayOptions)1);
		m_ResidenceResult = new NativeArray<Entity>(1, (Allocator)4, (NativeArrayOptions)1);
		m_HouseholdResult = new NativeArray<HouseholdResult>(1, (Allocator)4, (NativeArrayOptions)1);
		AddBinding((IBinding)(object)(m_HouseholdMap = new RawMapBinding<int>(group, "householdMap", (Action<IJsonWriter, int>)BindHousehold, (IReader<int>)null, (IWriter<int>)null)));
		AddBinding((IBinding)(object)(m_ResidentMap = new RawMapBinding<int>(group, "residentMap", (Action<IJsonWriter, int>)BindResident, (IReader<int>)null, (IWriter<int>)null)));
		AddBinding((IBinding)(object)(m_PetMap = new RawMapBinding<int>(group, "petMap", (Action<IJsonWriter, int>)BindPet, (IReader<int>)null, (IWriter<int>)null)));
	}

	private void BindHousehold(IJsonWriter writer, int index)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		Entity val = m_HouseholdsResult[index].m_Entity;
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		WriteItem(writer, val, "Media/Game/Icons/Household.svg", ((EntityManager)(ref entityManager)).GetBuffer<HouseholdCitizen>(val, false).Length);
	}

	private void BindResident(IJsonWriter writer, int index)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		Entity entity = m_ResidentsResult[index].m_Entity;
		WriteItem(writer, entity, null);
	}

	private void BindPet(IJsonWriter writer, int index)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Entity entity = m_PetsResult[index];
		WriteItem(writer, entity, "Media/Game/Icons/Pet.svg");
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_ResidentsResult.Dispose();
		m_HouseholdResult.Dispose();
		m_PetsResult.Dispose();
		m_HouseholdsResult.Dispose();
		m_Results.Dispose();
		m_ResidenceResult.Dispose();
		base.OnDestroy();
	}

	protected override void Reset()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		m_ResidentsResult.Clear();
		m_PetsResult.Clear();
		m_HouseholdsResult.Clear();
		m_ResidenceResult[0] = Entity.Null;
		m_HouseholdResult[0] = default(HouseholdResult);
		m_Results[0] = 0;
		m_Results[1] = 0;
		m_Results[2] = 0;
		residenceIsHomelessShelter = false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = IJobExtensions.Schedule<CheckVisibilityJob>(new CheckVisibilityJob
		{
			m_SelectedEntity = selectedEntity,
			m_SelectedPrefab = selectedPrefab,
			m_ParkFromLookup = InternalCompilerInterface.GetComponentLookup<Game.Buildings.Park>(ref __TypeHandle.__Game_Buildings_Park_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingLookup = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AbandonedLookup = InternalCompilerInterface.GetComponentLookup<Abandoned>(ref __TypeHandle.__Game_Buildings_Abandoned_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdLookup = InternalCompilerInterface.GetComponentLookup<Household>(ref __TypeHandle.__Game_Citizens_Household_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CitizenLookup = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdPetLookup = InternalCompilerInterface.GetComponentLookup<HouseholdPet>(ref __TypeHandle.__Game_Citizens_HouseholdPet_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HealthProblemLookup = InternalCompilerInterface.GetComponentLookup<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TravelPurposeLookup = InternalCompilerInterface.GetComponentLookup<TravelPurpose>(ref __TypeHandle.__Game_Citizens_TravelPurpose_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyDataLookup = InternalCompilerInterface.GetComponentLookup<BuildingPropertyData>(ref __TypeHandle.__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdCitizenLookup = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RenterLookup = InternalCompilerInterface.GetBufferLookup<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Results = m_Results
		}, ((SystemBase)this).Dependency);
		((JobHandle)(ref val)).Complete();
		base.visible = m_Results[0] == 1 && m_Results[1] > 0;
		if (base.visible)
		{
			val = IJobExtensions.Schedule<CollectDataJob>(new CollectDataJob
			{
				m_SelectedEntity = selectedEntity,
				m_BuildingLookup = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HomelessHouseholdLookup = InternalCompilerInterface.GetComponentLookup<HomelessHousehold>(ref __TypeHandle.__Game_Citizens_HomelessHousehold_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HouseholdLookup = InternalCompilerInterface.GetComponentLookup<Household>(ref __TypeHandle.__Game_Citizens_Household_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CitizenLookup = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HouseholdMemberLookup = InternalCompilerInterface.GetComponentLookup<HouseholdMember>(ref __TypeHandle.__Game_Citizens_HouseholdMember_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HouseholdPetLookup = InternalCompilerInterface.GetComponentLookup<HouseholdPet>(ref __TypeHandle.__Game_Citizens_HouseholdPet_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HealthProblemLookup = InternalCompilerInterface.GetComponentLookup<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TravelPurposeLookup = InternalCompilerInterface.GetComponentLookup<TravelPurpose>(ref __TypeHandle.__Game_Citizens_TravelPurpose_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PropertyRenterLookup = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HouseholdCitizenLookup = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ResourcesLookup = InternalCompilerInterface.GetBufferLookup<Resources>(ref __TypeHandle.__Game_Economy_Resources_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HouseholdAnimalsLookup = InternalCompilerInterface.GetBufferLookup<HouseholdAnimal>(ref __TypeHandle.__Game_Citizens_HouseholdAnimal_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_RenterLookup = InternalCompilerInterface.GetBufferLookup<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ResidenceResult = m_ResidenceResult,
				m_HouseholdResult = m_HouseholdResult,
				m_HouseholdsResult = m_HouseholdsResult,
				m_ResidentsResult = m_ResidentsResult,
				m_PetsResult = m_PetsResult
			}, ((SystemBase)this).Dependency);
			((JobHandle)(ref val)).Complete();
			NativeSortExtension.Sort<HouseholdResult>(m_HouseholdsResult);
			NativeSortExtension.Sort<ResidentResult>(m_ResidentsResult);
			NativeSortExtension.Sort<Entity>(m_PetsResult);
			for (int i = 0; i < m_HouseholdsResult.Length; i++)
			{
				((MapBindingBase<int>)(object)m_HouseholdMap).Update(i);
			}
			for (int j = 0; j < m_ResidentsResult.Length; j++)
			{
				((MapBindingBase<int>)(object)m_ResidentMap).Update(j);
			}
			for (int k = 0; k < m_PetsResult.Length; k++)
			{
				((MapBindingBase<int>)(object)m_PetMap).Update(k);
			}
		}
	}

	protected override void OnProcess()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		residenceEntity = m_ResidenceResult[0];
		household = m_HouseholdResult[0];
		variant = (HouseholdSidebarVariant)m_Results[2];
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).Exists(residenceEntity))
		{
			residenceEntity = Entity.Null;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.Park>(residenceEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (!((EntityManager)(ref entityManager)).HasComponent<Abandoned>(residenceEntity))
			{
				return;
			}
		}
		DynamicBuffer<Renter> val = default(DynamicBuffer<Renter>);
		if (EntitiesExtensions.TryGetBuffer<Renter>(((ComponentSystemBase)this).EntityManager, residenceEntity, true, ref val) && val.Length > 0)
		{
			m_InfoUISystem.tooltipTags.Add(TooltipTags.HomelessShelter);
			base.tooltipTags.Add(TooltipTags.HomelessShelter.ToString());
			base.tooltipKeys.Add(TooltipTags.HomelessShelter.ToString());
			residenceIsHomelessShelter = true;
		}
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		writer.PropertyName("variant");
		writer.Write(variant.ToString());
		writer.PropertyName("residence");
		WriteItem(writer, residenceEntity, residenceIsHomelessShelter ? "Media/Glyphs/HomelessShelter.svg" : "Media/Glyphs/Residence.svg");
		writer.PropertyName("household");
		WriteItem(writer, household.m_Entity, "Media/Game/Icons/Household.svg", household.m_Members);
		writer.PropertyName("households");
		writer.Write(m_HouseholdsResult.Length);
		writer.PropertyName("residents");
		writer.Write(m_ResidentsResult.Length);
		writer.PropertyName("pets");
		writer.Write(m_PetsResult.Length);
	}

	private void WriteItem(IJsonWriter writer, Entity entity, string iconPath, int memberCount = 0)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		writer.TypeBegin("Game.UI.InGame.HouseholdSidebarSection+HouseholdSidebarItem");
		writer.PropertyName("entity");
		UnityWriters.Write(writer, entity);
		writer.PropertyName("name");
		m_NameSystem.BindName(writer, entity);
		writer.PropertyName("familyName");
		if (!(entity == Entity.Null))
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Household>(entity))
			{
				m_NameSystem.BindFamilyName(writer, entity);
				goto IL_0073;
			}
		}
		writer.WriteNull();
		goto IL_0073;
		IL_0073:
		writer.PropertyName("icon");
		writer.Write(iconPath);
		writer.PropertyName("selected");
		writer.Write(entity == selectedEntity);
		writer.PropertyName("count");
		if (memberCount == 0)
		{
			writer.WriteNull();
		}
		else
		{
			writer.Write(memberCount);
		}
		writer.TypeEnd();
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
	public HouseholdSidebarSection()
	{
	}
}
