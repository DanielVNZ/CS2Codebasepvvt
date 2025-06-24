using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Serialization.Entities;
using Game.Agents;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Debug;
using Game.Economy;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class CountHouseholdDataSystem : GameSystemBase, IDefaultSerializable, ISerializable
{
	public struct HouseholdNeedData : IAccumulable<HouseholdNeedData>
	{
		public int m_HouseholdNeed;

		public void Accumulate(HouseholdNeedData other)
		{
			m_HouseholdNeed += other.m_HouseholdNeed;
		}
	}

	public struct HouseholdData : IAccumulable<HouseholdData>, ISerializable
	{
		public int m_MovingInHouseholdCount;

		public int m_MovingInCitizenCount;

		public int m_MovingAwayHouseholdCount;

		public int m_CommuterHouseholdCount;

		public int m_TouristCitizenCount;

		public int m_HomelessHouseholdCount;

		public int m_HomelessCitizenCount;

		public int m_MovedInHouseholdCount;

		public int m_MovedInCitizenCount;

		public int m_ChildrenCount;

		public int m_TeenCount;

		public int m_AdultCount;

		public int m_SeniorCount;

		public int m_StudentCount;

		public int m_UneducatedCount;

		public int m_PoorlyEducatedCount;

		public int m_EducatedCount;

		public int m_WellEducatedCount;

		public int m_HighlyEducatedCount;

		public int m_WorkableCitizenCount;

		public int m_CityWorkerCount;

		public int m_DeadCitizenCount;

		public long m_TotalMovedInCitizenHappiness;

		public long m_TotalMovedInCitizenWellbeing;

		public long m_TotalMovedInCitizenHealth;

		public int m_EmployableByEducation0;

		public int m_EmployableByEducation1;

		public int m_EmployableByEducation2;

		public int m_EmployableByEducation3;

		public int m_EmployableByEducation4;

		public void Accumulate(HouseholdData other)
		{
			m_MovingInHouseholdCount += other.m_MovingInHouseholdCount;
			m_MovingInCitizenCount += other.m_MovingInCitizenCount;
			m_MovingAwayHouseholdCount += other.m_MovingAwayHouseholdCount;
			m_CommuterHouseholdCount += other.m_CommuterHouseholdCount;
			m_TouristCitizenCount += other.m_TouristCitizenCount;
			m_HomelessHouseholdCount += other.m_HomelessHouseholdCount;
			m_HomelessCitizenCount += other.m_HomelessCitizenCount;
			m_MovedInHouseholdCount += other.m_MovedInHouseholdCount;
			m_MovedInCitizenCount += other.m_MovedInCitizenCount;
			m_ChildrenCount += other.m_ChildrenCount;
			m_TeenCount += other.m_TeenCount;
			m_AdultCount += other.m_AdultCount;
			m_SeniorCount += other.m_SeniorCount;
			m_StudentCount += other.m_StudentCount;
			m_UneducatedCount += other.m_UneducatedCount;
			m_PoorlyEducatedCount += other.m_PoorlyEducatedCount;
			m_EducatedCount += other.m_EducatedCount;
			m_WellEducatedCount += other.m_WellEducatedCount;
			m_HighlyEducatedCount += other.m_HighlyEducatedCount;
			m_WorkableCitizenCount += other.m_WorkableCitizenCount;
			m_CityWorkerCount += other.m_CityWorkerCount;
			m_DeadCitizenCount += other.m_DeadCitizenCount;
			m_TotalMovedInCitizenHappiness += other.m_TotalMovedInCitizenHappiness;
			m_TotalMovedInCitizenWellbeing += other.m_TotalMovedInCitizenWellbeing;
			m_TotalMovedInCitizenHealth += other.m_TotalMovedInCitizenHealth;
			m_EmployableByEducation0 += other.m_EmployableByEducation0;
			m_EmployableByEducation1 += other.m_EmployableByEducation1;
			m_EmployableByEducation2 += other.m_EmployableByEducation2;
			m_EmployableByEducation3 += other.m_EmployableByEducation3;
			m_EmployableByEducation4 += other.m_EmployableByEducation4;
		}

		public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
		{
			int num = m_MovingInHouseholdCount;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
			int num2 = m_MovingInCitizenCount;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num2);
			int num3 = m_MovingAwayHouseholdCount;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num3);
			int num4 = m_CommuterHouseholdCount;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num4);
			int num5 = m_TouristCitizenCount;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num5);
			int num6 = m_HomelessHouseholdCount;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num6);
			int num7 = m_HomelessCitizenCount;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num7);
			int num8 = m_MovedInHouseholdCount;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num8);
			int num9 = m_MovedInCitizenCount;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num9);
			int num10 = m_ChildrenCount;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num10);
			int num11 = m_TeenCount;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num11);
			int num12 = m_AdultCount;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num12);
			int num13 = m_SeniorCount;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num13);
			int num14 = m_StudentCount;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num14);
			int num15 = m_UneducatedCount;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num15);
			int num16 = m_PoorlyEducatedCount;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num16);
			int num17 = m_EducatedCount;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num17);
			int num18 = m_WellEducatedCount;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num18);
			int num19 = m_HighlyEducatedCount;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num19);
			int num20 = m_WorkableCitizenCount;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num20);
			int num21 = m_CityWorkerCount;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num21);
			int num22 = m_DeadCitizenCount;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num22);
			long num23 = m_TotalMovedInCitizenHealth;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num23);
			long num24 = m_TotalMovedInCitizenHappiness;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num24);
			long num25 = m_TotalMovedInCitizenWellbeing;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num25);
			int num26 = m_EmployableByEducation0;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num26);
			int num27 = m_EmployableByEducation1;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num27);
			int num28 = m_EmployableByEducation2;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num28);
			int num29 = m_EmployableByEducation3;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num29);
			int num30 = m_EmployableByEducation4;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num30);
		}

		public void Deserialize<TReader>(TReader reader) where TReader : IReader
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			Context context = ((IReader)reader).context;
			if (((Context)(ref context)).version < Version.statisticUnifying)
			{
				int num = default(int);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
				return;
			}
			ref int reference = ref m_MovingInHouseholdCount;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
			ref int reference2 = ref m_MovingInCitizenCount;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference2);
			ref int reference3 = ref m_MovingAwayHouseholdCount;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference3);
			ref int reference4 = ref m_CommuterHouseholdCount;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference4);
			ref int reference5 = ref m_TouristCitizenCount;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference5);
			ref int reference6 = ref m_HomelessHouseholdCount;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference6);
			ref int reference7 = ref m_HomelessCitizenCount;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference7);
			ref int reference8 = ref m_MovedInHouseholdCount;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference8);
			ref int reference9 = ref m_MovedInCitizenCount;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference9);
			ref int reference10 = ref m_ChildrenCount;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference10);
			ref int reference11 = ref m_TeenCount;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference11);
			ref int reference12 = ref m_AdultCount;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference12);
			ref int reference13 = ref m_SeniorCount;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference13);
			ref int reference14 = ref m_StudentCount;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference14);
			ref int reference15 = ref m_UneducatedCount;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference15);
			ref int reference16 = ref m_PoorlyEducatedCount;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference16);
			ref int reference17 = ref m_EducatedCount;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference17);
			ref int reference18 = ref m_WellEducatedCount;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference18);
			ref int reference19 = ref m_HighlyEducatedCount;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference19);
			ref int reference20 = ref m_WorkableCitizenCount;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference20);
			ref int reference21 = ref m_CityWorkerCount;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference21);
			ref int reference22 = ref m_DeadCitizenCount;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference22);
			ref long reference23 = ref m_TotalMovedInCitizenHealth;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference23);
			ref long reference24 = ref m_TotalMovedInCitizenHappiness;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference24);
			ref long reference25 = ref m_TotalMovedInCitizenWellbeing;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference25);
			ref int reference26 = ref m_EmployableByEducation0;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference26);
			ref int reference27 = ref m_EmployableByEducation1;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference27);
			ref int reference28 = ref m_EmployableByEducation2;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference28);
			ref int reference29 = ref m_EmployableByEducation3;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference29);
			ref int reference30 = ref m_EmployableByEducation4;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference30);
		}

		public int Population()
		{
			return m_MovedInCitizenCount;
		}

		public int Unemployed()
		{
			return m_WorkableCitizenCount - m_CityWorkerCount;
		}
	}

	[BurstCompile]
	private struct CountHouseholdJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<Household> m_HouseholdType;

		[ReadOnly]
		public ComponentTypeHandle<HomelessHousehold> m_HomelessHouseholdType;

		[ReadOnly]
		public ComponentTypeHandle<HouseholdNeed> m_HouseholdNeedType;

		[ReadOnly]
		public ComponentTypeHandle<PropertyRenter> m_PropertyRenterType;

		[ReadOnly]
		public BufferTypeHandle<Resources> m_ResourcesType;

		[ReadOnly]
		public BufferTypeHandle<HouseholdCitizen> m_HouseholdCitizenType;

		[ReadOnly]
		public ComponentLookup<HealthProblem> m_HealthProblems;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Park> m_Parks;

		[ReadOnly]
		public ComponentLookup<Abandoned> m_Abandoneds;

		[ReadOnly]
		public ComponentLookup<Citizen> m_Citizens;

		[ReadOnly]
		public ComponentLookup<Game.Citizens.Student> m_Students;

		[ReadOnly]
		public ComponentLookup<Worker> m_Workers;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> m_OutsideConnections;

		public ParallelWriter<HouseholdData> m_HouseholdCountData;

		public ParallelWriter<HouseholdNeedData> m_HouseholdNeedCountData;

		[ReadOnly]
		public CitizenHappinessParameterData m_CitizenHappinessParameterData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_0398: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_03de: Unknown result type (might be due to invalid IL or missing references)
			HouseholdData householdData = default(HouseholdData);
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<TouristHousehold>();
			bool flag2 = ((ArchetypeChunk)(ref chunk)).Has<CommuterHousehold>();
			bool flag3 = ((ArchetypeChunk)(ref chunk)).Has<HomelessHousehold>();
			if (((ArchetypeChunk)(ref chunk)).Has<MovingAway>() && !flag && !flag2)
			{
				householdData.m_MovingAwayHouseholdCount += ((ArchetypeChunk)(ref chunk)).Count;
				m_HouseholdCountData.Accumulate(householdData);
				return;
			}
			NativeArray<Household> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Household>(ref m_HouseholdType);
			NativeArray<HomelessHousehold> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HomelessHousehold>(ref m_HomelessHouseholdType);
			NativeArray<HouseholdNeed> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HouseholdNeed>(ref m_HouseholdNeedType);
			NativeArray<PropertyRenter> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PropertyRenter>(ref m_PropertyRenterType);
			((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Resources>(ref m_ResourcesType);
			BufferAccessor<HouseholdCitizen> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<HouseholdCitizen>(ref m_HouseholdCitizenType);
			if (flag2)
			{
				householdData.m_CommuterHouseholdCount += ((ArchetypeChunk)(ref chunk)).Count;
			}
			for (int i = 0; i < nativeArray.Length; i++)
			{
				bool flag4 = true;
				bool flag5 = !(flag2 || flag);
				Entity val = ((nativeArray4.Length != 0) ? nativeArray4[i].m_Property : Entity.Null);
				if (nativeArray2.Length > 0)
				{
					val = nativeArray2[i].m_TempHome;
				}
				if ((nativeArray[i].m_Flags & HouseholdFlags.MovedIn) == 0 && val == Entity.Null && ((ArchetypeChunk)(ref chunk)).Has<PropertySeeker>())
				{
					householdData.m_MovingInHouseholdCount++;
					householdData.m_MovingInCitizenCount += bufferAccessor[i].Length;
					flag4 = false;
					flag5 = false;
				}
				if (flag4 && nativeArray3[i].m_Resource != Resource.NoResource)
				{
					int resourceIndex = EconomyUtils.GetResourceIndex(nativeArray3[i].m_Resource);
					m_HouseholdNeedCountData.Accumulate(resourceIndex, new HouseholdNeedData
					{
						m_HouseholdNeed = nativeArray3[i].m_Amount
					});
				}
				if (flag && ((ArchetypeChunk)(ref chunk)).Has<Target>())
				{
					householdData.m_TouristCitizenCount += bufferAccessor[i].Length;
				}
				if (!flag5)
				{
					continue;
				}
				householdData.m_MovedInHouseholdCount++;
				if (flag3)
				{
					householdData.m_HomelessHouseholdCount++;
				}
				DynamicBuffer<HouseholdCitizen> val2 = bufferAccessor[i];
				for (int j = 0; j < val2.Length; j++)
				{
					if (CitizenUtils.IsDead(val2[j].m_Citizen, ref m_HealthProblems))
					{
						householdData.m_DeadCitizenCount++;
						continue;
					}
					Citizen citizen = m_Citizens[val2[j].m_Citizen];
					switch (citizen.GetAge())
					{
					case CitizenAge.Child:
						householdData.m_ChildrenCount++;
						break;
					case CitizenAge.Teen:
						householdData.m_TeenCount++;
						break;
					case CitizenAge.Adult:
						householdData.m_AdultCount++;
						break;
					case CitizenAge.Elderly:
						householdData.m_SeniorCount++;
						break;
					}
					int educationLevel = citizen.GetEducationLevel();
					switch (educationLevel)
					{
					case 0:
						householdData.m_UneducatedCount++;
						break;
					case 1:
						householdData.m_PoorlyEducatedCount++;
						break;
					case 2:
						householdData.m_EducatedCount++;
						break;
					case 3:
						householdData.m_WellEducatedCount++;
						break;
					case 4:
						householdData.m_HighlyEducatedCount++;
						break;
					}
					if (m_Students.HasComponent(val2[j].m_Citizen))
					{
						householdData.m_StudentCount++;
					}
					bool flag6 = false;
					bool flag7 = false;
					if (m_Workers.HasComponent(val2[j].m_Citizen))
					{
						flag6 = true;
						flag7 = m_OutsideConnections.HasComponent(m_Workers[val2[j].m_Citizen].m_Workplace);
						if (!flag7)
						{
							householdData.m_CityWorkerCount++;
						}
					}
					if (CitizenUtils.IsWorkableCitizen(val2[j].m_Citizen, ref m_Citizens, ref m_Students, ref m_HealthProblems))
					{
						householdData.m_WorkableCitizenCount++;
						if (!flag6 || flag7 || m_Workers[val2[j].m_Citizen].m_Level < educationLevel)
						{
							switch (educationLevel)
							{
							case 0:
								householdData.m_EmployableByEducation0++;
								break;
							case 1:
								householdData.m_EmployableByEducation1++;
								break;
							case 2:
								householdData.m_EmployableByEducation2++;
								break;
							case 3:
								householdData.m_EmployableByEducation3++;
								break;
							case 4:
								householdData.m_EmployableByEducation4++;
								break;
							}
						}
					}
					householdData.m_TotalMovedInCitizenHappiness += citizen.Happiness;
					householdData.m_TotalMovedInCitizenWellbeing += citizen.m_WellBeing;
					householdData.m_TotalMovedInCitizenHealth += citizen.m_Health;
					householdData.m_MovedInCitizenCount++;
					if (flag3)
					{
						householdData.m_HomelessCitizenCount++;
					}
				}
			}
			m_HouseholdCountData.Accumulate(householdData);
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct ResultJob : IJob
	{
		[ReadOnly]
		public NativeAccumulator<HouseholdData> m_HouseholdData;

		[ReadOnly]
		public NativeAccumulator<HouseholdNeedData> m_HouseholdNeedData;

		public NativeArray<int> m_ResourceNeed;

		public NativeArray<int> m_EmployableByEducation;

		public Entity m_City;

		public ComponentLookup<Population> m_Populations;

		public void Execute()
		{
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < EconomyUtils.ResourceCount; i++)
			{
				m_ResourceNeed[i] = m_HouseholdNeedData.GetResult(i).m_HouseholdNeed;
			}
			HouseholdData result = m_HouseholdData.GetResult(0);
			m_EmployableByEducation[0] = result.m_EmployableByEducation0;
			m_EmployableByEducation[1] = result.m_EmployableByEducation1;
			m_EmployableByEducation[2] = result.m_EmployableByEducation2;
			m_EmployableByEducation[3] = result.m_EmployableByEducation3;
			m_EmployableByEducation[4] = result.m_EmployableByEducation4;
			Population population = m_Populations[m_City];
			population.m_Population = result.Population();
			population.m_PopulationWithMoveIn = result.m_MovedInCitizenCount + result.m_MovingInCitizenCount;
			if (population.m_Population > 0)
			{
				population.m_AverageHappiness = (int)(result.m_TotalMovedInCitizenHappiness / result.m_MovedInCitizenCount);
				population.m_AverageHealth = (int)(result.m_TotalMovedInCitizenHealth / result.m_MovedInCitizenCount);
			}
			else
			{
				population.m_AverageHappiness = 50;
				population.m_AverageHealth = 50;
			}
			m_Populations[m_City] = population;
		}
	}

	[BurstCompile]
	private struct CitizenRequirementJob : IJobChunk
	{
		[ReadOnly]
		public EntityArchetype m_UnlockEventArchetype;

		public ParallelWriter m_CommandBuffer;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<CitizenRequirementData> m_CitizenRequirementType;

		public ComponentTypeHandle<UnlockRequirementData> m_UnlockRequirementType;

		[ReadOnly]
		public ComponentLookup<Population> m_Populations;

		public Entity m_City;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<CitizenRequirementData> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CitizenRequirementData>(ref m_CitizenRequirementType);
			NativeArray<UnlockRequirementData> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<UnlockRequirementData>(ref m_UnlockRequirementType);
			ChunkEntityEnumerator val = default(ChunkEntityEnumerator);
			((ChunkEntityEnumerator)(ref val))._002Ector(useEnabledMask, chunkEnabledMask, ((ArchetypeChunk)(ref chunk)).Count);
			int num = default(int);
			while (((ChunkEntityEnumerator)(ref val)).NextEntityIndex(ref num))
			{
				CitizenRequirementData citizenRequirement = nativeArray2[num];
				UnlockRequirementData unlockRequirement = nativeArray3[num];
				if (ShouldUnlock(citizenRequirement, ref unlockRequirement))
				{
					Entity val2 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(unfilteredChunkIndex, m_UnlockEventArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Unlock>(unfilteredChunkIndex, val2, new Unlock(nativeArray[num]));
				}
				nativeArray3[num] = unlockRequirement;
			}
		}

		private bool ShouldUnlock(CitizenRequirementData citizenRequirement, ref UnlockRequirementData unlockRequirement)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			Population population = m_Populations[m_City];
			if (population.m_Population < citizenRequirement.m_MinimumPopulation || citizenRequirement.m_MinimumHappiness == 0)
			{
				unlockRequirement.m_Progress = math.min(population.m_Population, citizenRequirement.m_MinimumPopulation);
			}
			else
			{
				unlockRequirement.m_Progress = math.min(population.m_AverageHappiness, citizenRequirement.m_MinimumHappiness);
			}
			if (population.m_Population >= citizenRequirement.m_MinimumPopulation)
			{
				return population.m_AverageHappiness >= citizenRequirement.m_MinimumHappiness;
			}
			return false;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		public ComponentTypeHandle<Household> __Game_Citizens_Household_RW_ComponentTypeHandle;

		public ComponentTypeHandle<HomelessHousehold> __Game_Citizens_HomelessHousehold_RW_ComponentTypeHandle;

		public ComponentTypeHandle<HouseholdNeed> __Game_Citizens_HouseholdNeed_RW_ComponentTypeHandle;

		public ComponentTypeHandle<PropertyRenter> __Game_Buildings_PropertyRenter_RW_ComponentTypeHandle;

		public BufferTypeHandle<Resources> __Game_Economy_Resources_RW_BufferTypeHandle;

		public BufferTypeHandle<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RW_BufferTypeHandle;

		public ComponentLookup<HealthProblem> __Game_Citizens_HealthProblem_RW_ComponentLookup;

		public ComponentLookup<Game.Buildings.Park> __Game_Buildings_Park_RW_ComponentLookup;

		public ComponentLookup<Abandoned> __Game_Buildings_Abandoned_RW_ComponentLookup;

		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RW_ComponentLookup;

		public ComponentLookup<Game.Citizens.Student> __Game_Citizens_Student_RW_ComponentLookup;

		public ComponentLookup<Worker> __Game_Citizens_Worker_RW_ComponentLookup;

		public ComponentLookup<Game.Objects.OutsideConnection> __Game_Objects_OutsideConnection_RW_ComponentLookup;

		public ComponentLookup<Population> __Game_City_Population_RW_ComponentLookup;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CitizenRequirementData> __Game_Prefabs_CitizenRequirementData_RO_ComponentTypeHandle;

		public ComponentTypeHandle<UnlockRequirementData> __Game_Prefabs_UnlockRequirementData_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Population> __Game_City_Population_RO_ComponentLookup;

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
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			__Game_Citizens_Household_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Household>(false);
			__Game_Citizens_HomelessHousehold_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HomelessHousehold>(false);
			__Game_Citizens_HouseholdNeed_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HouseholdNeed>(false);
			__Game_Buildings_PropertyRenter_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PropertyRenter>(false);
			__Game_Economy_Resources_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Resources>(false);
			__Game_Citizens_HouseholdCitizen_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<HouseholdCitizen>(false);
			__Game_Citizens_HealthProblem_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HealthProblem>(false);
			__Game_Buildings_Park_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.Park>(false);
			__Game_Buildings_Abandoned_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Abandoned>(false);
			__Game_Citizens_Citizen_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(false);
			__Game_Citizens_Student_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Citizens.Student>(false);
			__Game_Citizens_Worker_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Worker>(false);
			__Game_Objects_OutsideConnection_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.OutsideConnection>(false);
			__Game_City_Population_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Population>(false);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_CitizenRequirementData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CitizenRequirementData>(true);
			__Game_Prefabs_UnlockRequirementData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<UnlockRequirementData>(false);
			__Game_City_Population_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Population>(true);
		}
	}

	private ResourceSystem m_ResourceSystem;

	private CitySystem m_CitySystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_HouseholdQuery;

	private EntityQuery m_RequirementQuery;

	private EntityArchetype m_UnlockEventArchetype;

	[DebugWatchDeps]
	private JobHandle m_HouseholdDataWriteDependencies;

	private JobHandle m_HouseholdDataReadDependencies;

	private NativeAccumulator<HouseholdData> m_HouseholdCountData;

	private NativeAccumulator<HouseholdNeedData> m_HouseholdNeedCountData;

	private HouseholdData m_LastHouseholdCountData;

	[ResourceArray]
	[DebugWatchValue]
	private NativeArray<int> m_ResourceNeed;

	private bool m_NeedForceCountData;

	[DebugWatchValue]
	private NativeArray<int> m_EmployableByEducation;

	private TypeHandle __TypeHandle;

	[DebugWatchValue]
	public int MovingInHouseholdCount => m_LastHouseholdCountData.m_MovingInHouseholdCount;

	[DebugWatchValue]
	public int MovingInCitizenCount => m_LastHouseholdCountData.m_MovingInCitizenCount;

	[DebugWatchValue]
	public int MovingAwayHouseholdCount => m_LastHouseholdCountData.m_MovingAwayHouseholdCount;

	[DebugWatchValue]
	public int CommuterHouseholdCount => m_LastHouseholdCountData.m_CommuterHouseholdCount;

	[DebugWatchValue]
	public int TouristCitizenCount => m_LastHouseholdCountData.m_TouristCitizenCount;

	[DebugWatchValue]
	public int HomelessHouseholdCount => m_LastHouseholdCountData.m_HomelessHouseholdCount;

	[DebugWatchValue]
	public int HomelessCitizenCount => m_LastHouseholdCountData.m_HomelessCitizenCount;

	[DebugWatchValue]
	public int MovedInHouseholdCount => m_LastHouseholdCountData.m_MovedInHouseholdCount;

	[DebugWatchValue]
	public int MovedInCitizenCount => m_LastHouseholdCountData.m_MovedInCitizenCount;

	[DebugWatchValue]
	public int ChildrenCount => m_LastHouseholdCountData.m_ChildrenCount;

	[DebugWatchValue]
	public int AdultCount => m_LastHouseholdCountData.m_AdultCount;

	[DebugWatchValue]
	public int TeenCount => m_LastHouseholdCountData.m_TeenCount;

	[DebugWatchValue]
	public int SeniorCount => m_LastHouseholdCountData.m_SeniorCount;

	[DebugWatchValue]
	public int StudentCount => m_LastHouseholdCountData.m_StudentCount;

	[DebugWatchValue]
	public int UneducatedCount => m_LastHouseholdCountData.m_UneducatedCount;

	[DebugWatchValue]
	public int PoorlyEducatedCount => m_LastHouseholdCountData.m_PoorlyEducatedCount;

	[DebugWatchValue]
	public int EducatedCount => m_LastHouseholdCountData.m_EducatedCount;

	[DebugWatchValue]
	public int WellEducatedCount => m_LastHouseholdCountData.m_WellEducatedCount;

	[DebugWatchValue]
	public int HighlyEducatedCount => m_LastHouseholdCountData.m_HighlyEducatedCount;

	[DebugWatchValue]
	public int WorkableCitizenCount => m_LastHouseholdCountData.m_WorkableCitizenCount;

	[DebugWatchValue]
	public int CityWorkerCount => m_LastHouseholdCountData.m_CityWorkerCount;

	[DebugWatchValue]
	public int DeadCitizenCount => m_LastHouseholdCountData.m_DeadCitizenCount;

	[DebugWatchValue]
	public int AverageCitizenHappiness
	{
		get
		{
			if (m_LastHouseholdCountData.m_MovedInCitizenCount == 0)
			{
				return 0;
			}
			return (int)(m_LastHouseholdCountData.m_TotalMovedInCitizenHappiness / m_LastHouseholdCountData.m_MovedInCitizenCount);
		}
	}

	[DebugWatchValue]
	public int AverageCitizenHealth
	{
		get
		{
			if (m_LastHouseholdCountData.m_MovedInCitizenCount == 0)
			{
				return 0;
			}
			return (int)(m_LastHouseholdCountData.m_TotalMovedInCitizenHealth / m_LastHouseholdCountData.m_MovedInCitizenCount);
		}
	}

	[DebugWatchValue]
	public float UnemploymentRate
	{
		get
		{
			if (m_LastHouseholdCountData.m_WorkableCitizenCount == 0)
			{
				return 0f;
			}
			return 100f * (float)math.max(0, m_LastHouseholdCountData.m_WorkableCitizenCount - m_LastHouseholdCountData.m_CityWorkerCount) / (float)m_LastHouseholdCountData.m_WorkableCitizenCount;
		}
	}

	[DebugWatchValue]
	public float HomelessnessRate
	{
		get
		{
			if (m_LastHouseholdCountData.m_MovedInCitizenCount == 0)
			{
				return 0f;
			}
			return 100f * (float)m_LastHouseholdCountData.m_HomelessCitizenCount / (float)m_LastHouseholdCountData.m_MovedInCitizenCount;
		}
	}

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	public HouseholdData GetHouseholdCountData()
	{
		return m_LastHouseholdCountData;
	}

	public NativeArray<int> GetResourceNeeds(out JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		deps = m_HouseholdDataWriteDependencies;
		return m_ResourceNeed;
	}

	public bool IsCountDataNotReady()
	{
		return m_NeedForceCountData;
	}

	public NativeArray<int> GetEmployables()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return m_EmployableByEducation;
	}

	public void AddHouseholdDataReader(JobHandle reader)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_HouseholdDataReadDependencies = JobHandle.CombineDependencies(m_HouseholdDataReadDependencies, reader);
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Household>() };
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_HouseholdQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_RequirementQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<CitizenRequirementData>(),
			ComponentType.ReadWrite<UnlockRequirementData>(),
			ComponentType.ReadOnly<Locked>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_UnlockEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Event>(),
			ComponentType.ReadWrite<Unlock>()
		});
		m_HouseholdCountData = new NativeAccumulator<HouseholdData>(AllocatorHandle.op_Implicit((Allocator)4));
		m_HouseholdNeedCountData = new NativeAccumulator<HouseholdNeedData>(EconomyUtils.ResourceCount, AllocatorHandle.op_Implicit((Allocator)4));
		m_ResourceNeed = new NativeArray<int>(EconomyUtils.ResourceCount, (Allocator)4, (NativeArrayOptions)1);
		m_EmployableByEducation = new NativeArray<int>(5, (Allocator)4, (NativeArrayOptions)1);
		((ComponentSystemBase)this).RequireForUpdate(m_HouseholdQuery);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_HouseholdCountData.Dispose();
		m_HouseholdNeedCountData.Dispose();
		m_ResourceNeed.Dispose();
		m_EmployableByEducation.Dispose();
		base.OnDestroy();
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		HouseholdData householdData = m_LastHouseholdCountData;
		((IWriter)writer/*cast due to .constrained prefix*/).Write<HouseholdData>(householdData);
		NativeArray<int> val = m_ResourceNeed;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val);
		NativeArray<int> val2 = m_EmployableByEducation;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val2);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.economyFix)
		{
			ref HouseholdData reference = ref m_LastHouseholdCountData;
			((IReader)reader/*cast due to .constrained prefix*/).Read<HouseholdData>(ref reference);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.countHouseholdDataFix)
		{
			context = ((IReader)reader).context;
			ContextFormat format = ((Context)(ref context)).format;
			if (((ContextFormat)(ref format)).Has<FormatTags>(FormatTags.FishResource))
			{
				NativeArray<int> val = m_ResourceNeed;
				((IReader)reader/*cast due to .constrained prefix*/).Read(val);
			}
			else
			{
				NativeArray<int> subArray = m_ResourceNeed.GetSubArray(0, 40);
				((IReader)reader/*cast due to .constrained prefix*/).Read(subArray);
				m_ResourceNeed[40] = 0;
			}
			NativeArray<int> val2 = m_EmployableByEducation;
			((IReader)reader/*cast due to .constrained prefix*/).Read(val2);
		}
		else
		{
			m_NeedForceCountData = true;
		}
	}

	public void SetDefaults(Context context)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		m_LastHouseholdCountData = default(HouseholdData);
		m_HouseholdCountData.Clear();
		m_HouseholdNeedCountData.Clear();
		CollectionUtils.Fill<int>(m_ResourceNeed, 0);
		CollectionUtils.Fill<int>(m_EmployableByEducation, 0);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		m_LastHouseholdCountData = m_HouseholdCountData.GetResult(0);
		m_HouseholdCountData.Clear();
		m_HouseholdNeedCountData.Clear();
		CountHouseholdJob countHouseholdJob = new CountHouseholdJob
		{
			m_HouseholdType = InternalCompilerInterface.GetComponentTypeHandle<Household>(ref __TypeHandle.__Game_Citizens_Household_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HomelessHouseholdType = InternalCompilerInterface.GetComponentTypeHandle<HomelessHousehold>(ref __TypeHandle.__Game_Citizens_HomelessHousehold_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdNeedType = InternalCompilerInterface.GetComponentTypeHandle<HouseholdNeed>(ref __TypeHandle.__Game_Citizens_HouseholdNeed_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyRenterType = InternalCompilerInterface.GetComponentTypeHandle<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResourcesType = InternalCompilerInterface.GetBufferTypeHandle<Resources>(ref __TypeHandle.__Game_Economy_Resources_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdCitizenType = InternalCompilerInterface.GetBufferTypeHandle<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HealthProblems = InternalCompilerInterface.GetComponentLookup<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Parks = InternalCompilerInterface.GetComponentLookup<Game.Buildings.Park>(ref __TypeHandle.__Game_Buildings_Park_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Abandoneds = InternalCompilerInterface.GetComponentLookup<Abandoned>(ref __TypeHandle.__Game_Buildings_Abandoned_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Citizens = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Students = InternalCompilerInterface.GetComponentLookup<Game.Citizens.Student>(ref __TypeHandle.__Game_Citizens_Student_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Workers = InternalCompilerInterface.GetComponentLookup<Worker>(ref __TypeHandle.__Game_Citizens_Worker_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OutsideConnections = InternalCompilerInterface.GetComponentLookup<Game.Objects.OutsideConnection>(ref __TypeHandle.__Game_Objects_OutsideConnection_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdCountData = m_HouseholdCountData.AsParallelWriter(),
			m_HouseholdNeedCountData = m_HouseholdNeedCountData.AsParallelWriter()
		};
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<CountHouseholdJob>(countHouseholdJob, m_HouseholdQuery, ((SystemBase)this).Dependency);
		ResultJob resultJob = new ResultJob
		{
			m_HouseholdData = m_HouseholdCountData,
			m_HouseholdNeedData = m_HouseholdNeedCountData,
			m_Populations = InternalCompilerInterface.GetComponentLookup<Population>(ref __TypeHandle.__Game_City_Population_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_City = m_CitySystem.City,
			m_ResourceNeed = m_ResourceNeed,
			m_EmployableByEducation = m_EmployableByEducation
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<ResultJob>(resultJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, m_HouseholdDataReadDependencies));
		m_HouseholdDataWriteDependencies = ((SystemBase)this).Dependency;
		if (m_NeedForceCountData)
		{
			JobHandle dependency = ((SystemBase)this).Dependency;
			((JobHandle)(ref dependency)).Complete();
			m_NeedForceCountData = false;
		}
		CitizenRequirementJob citizenRequirementJob = new CitizenRequirementJob
		{
			m_UnlockEventArchetype = m_UnlockEventArchetype
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		citizenRequirementJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		citizenRequirementJob.m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
		citizenRequirementJob.m_CitizenRequirementType = InternalCompilerInterface.GetComponentTypeHandle<CitizenRequirementData>(ref __TypeHandle.__Game_Prefabs_CitizenRequirementData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		citizenRequirementJob.m_UnlockRequirementType = InternalCompilerInterface.GetComponentTypeHandle<UnlockRequirementData>(ref __TypeHandle.__Game_Prefabs_UnlockRequirementData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		citizenRequirementJob.m_Populations = InternalCompilerInterface.GetComponentLookup<Population>(ref __TypeHandle.__Game_City_Population_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		citizenRequirementJob.m_City = m_CitySystem.City;
		CitizenRequirementJob citizenRequirementJob2 = citizenRequirementJob;
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<CitizenRequirementJob>(citizenRequirementJob2, m_RequirementQuery, ((SystemBase)this).Dependency);
		m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
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
	public CountHouseholdDataSystem()
	{
	}
}
