using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Serialization.Entities;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Economy;
using Game.Net;
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
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class CityServiceBudgetSystem : GameSystemBase, ICityServiceBudgetSystem
{
	private struct UpdateDataJob : IJob
	{
		[ReadOnly]
		public BufferLookup<CityStatistic> m_Stats;

		[ReadOnly]
		public NativeList<Entity> m_CityServiceEntities;

		public Entity m_City;

		[ReadOnly]
		public NativeParallelHashMap<CityStatisticsSystem.StatisticsKey, Entity> m_Lookup;

		public NativeList<CollectedCityServiceUpkeepData> m_CityServiceUpkeeps;

		public NativeParallelHashMap<Entity, CollectedCityServiceBudgetData> m_CityServiceBudgets;

		[ReadOnly]
		public ComponentLookup<CollectedCityServiceBudgetData> m_CollectedBudgets;

		[ReadOnly]
		public ComponentLookup<Creditworthiness> m_Creditworthiness;

		[ReadOnly]
		public ComponentLookup<Population> m_Populations;

		[ReadOnly]
		public ComponentLookup<Game.City.City> m_CityData;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifiers;

		[ReadOnly]
		public BufferLookup<CollectedCityServiceUpkeepData> m_CollectedUpkeeps;

		public NativeParallelHashMap<Entity, int2> m_CityServiceUpkeepIndices;

		[ReadOnly]
		public NativeArray<int> m_TaxRates;

		public NativeArray<int> m_Income;

		public NativeArray<int> m_Expenses;

		public Entity m_BudgetEntity;

		public int m_MonthlySubsidy;

		[ReadOnly]
		public BufferLookup<ServiceBudgetData> m_BudgetDatas;

		[ReadOnly]
		public NativeList<CollectedCityServiceFeeData> m_CollectedFees;

		[ReadOnly]
		public BufferLookup<ServiceFee> m_ServiceFees;

		[ReadOnly]
		public ComponentLookup<Loan> m_Loans;

		[ReadOnly]
		public OutsideTradeParameterData m_OutsideTradeParameterData;

		[ReadOnly]
		public EconomyParameterData m_EconomyParametersData;

		[ReadOnly]
		public int4 m_ServiceFacilityBuildingCount;

		[ReadOnly]
		public int m_PoliceStationBuildingCount;

		[ReadOnly]
		public int m_MapTileUpkeepCost;

		public NativeReference<int> m_TotalTaxes;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_040c: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0732: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0637: Unknown result type (might be due to invalid IL or missing references)
			//IL_063d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0643: Unknown result type (might be due to invalid IL or missing references)
			//IL_0662: Unknown result type (might be due to invalid IL or missing references)
			//IL_0668: Unknown result type (might be due to invalid IL or missing references)
			//IL_066e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0580: Unknown result type (might be due to invalid IL or missing references)
			//IL_0587: Unknown result type (might be due to invalid IL or missing references)
			//IL_0611: Unknown result type (might be due to invalid IL or missing references)
			//IL_0618: Unknown result type (might be due to invalid IL or missing references)
			//IL_0533: Unknown result type (might be due to invalid IL or missing references)
			//IL_053a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0546: Unknown result type (might be due to invalid IL or missing references)
			//IL_054d: Unknown result type (might be due to invalid IL or missing references)
			//IL_055a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0561: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0510: Unknown result type (might be due to invalid IL or missing references)
			//IL_0515: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_068d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0693: Unknown result type (might be due to invalid IL or missing references)
			//IL_0699: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f7: Unknown result type (might be due to invalid IL or missing references)
			if (!m_ServiceFees.HasBuffer(m_City))
			{
				return;
			}
			DynamicBuffer<ServiceFee> fees = m_ServiceFees[m_City];
			m_CityServiceUpkeeps.Clear();
			for (int i = 0; i < m_CityServiceEntities.Length; i++)
			{
				Entity val = m_CityServiceEntities[i];
				m_CityServiceBudgets[val] = m_CollectedBudgets[val];
				if (m_CollectedUpkeeps.HasBuffer(val))
				{
					DynamicBuffer<CollectedCityServiceUpkeepData> val2 = m_CollectedUpkeeps[val];
					m_CityServiceUpkeepIndices[val] = new int2(m_CityServiceUpkeeps.Length, val2.Length);
					for (int j = 0; j < val2.Length; j++)
					{
						ref NativeList<CollectedCityServiceUpkeepData> reference = ref m_CityServiceUpkeeps;
						CollectedCityServiceUpkeepData collectedCityServiceUpkeepData = val2[j];
						reference.Add(ref collectedCityServiceUpkeepData);
					}
				}
			}
			for (int k = 0; k < 15; k++)
			{
				ExpenseSource expenseSource = (ExpenseSource)k;
				m_Expenses[k] = 0;
				switch (expenseSource)
				{
				case ExpenseSource.ServiceUpkeep:
				{
					for (int l = 0; l < m_CityServiceEntities.Length; l++)
					{
						int serviceBudget = GetServiceBudget(m_CityServiceEntities[l], m_CityServiceBudgets, m_BudgetEntity, m_BudgetDatas);
						ref NativeArray<int> reference2 = ref m_Expenses;
						int num = k;
						reference2[num] += GetEstimatedServiceUpkeep(m_CityServiceBudgets[m_CityServiceEntities[l]], m_CityServiceUpkeepIndices[m_CityServiceEntities[l]], serviceBudget, m_CityServiceUpkeeps);
					}
					break;
				}
				case ExpenseSource.LoanInterest:
				{
					LoanInfo loanInfo = LoanSystem.CalculateLoan(m_Loans[m_City].m_Amount, m_Creditworthiness[m_City].m_Amount, m_CityModifiers[m_City], m_EconomyParametersData.m_LoanMinMaxInterestRate);
					m_Expenses[k] = loanInfo.m_DailyPayment;
					break;
				}
				case ExpenseSource.ImportElectricity:
					m_Expenses[k] = ServiceFeeSystem.GetServiceFees(PlayerResource.Electricity, m_CollectedFees).z;
					break;
				case ExpenseSource.ImportWater:
					m_Expenses[k] = ServiceFeeSystem.GetServiceFees(PlayerResource.Water, m_CollectedFees).z;
					break;
				case ExpenseSource.ExportSewage:
					m_Expenses[k] = ServiceFeeSystem.GetServiceFees(PlayerResource.Sewage, m_CollectedFees).z;
					break;
				case ExpenseSource.SubsidyCommercial:
					m_Expenses[k] = -TaxSystem.GetEstimatedTaxAmount(TaxAreaType.Commercial, TaxResultType.Expense, m_Lookup, m_Stats, m_TaxRates);
					break;
				case ExpenseSource.SubsidyIndustrial:
					m_Expenses[k] = -TaxSystem.GetEstimatedTaxAmount(TaxAreaType.Industrial, TaxResultType.Expense, m_Lookup, m_Stats, m_TaxRates);
					break;
				case ExpenseSource.SubsidyOffice:
					m_Expenses[k] = -TaxSystem.GetEstimatedTaxAmount(TaxAreaType.Office, TaxResultType.Expense, m_Lookup, m_Stats, m_TaxRates);
					break;
				case ExpenseSource.SubsidyResidential:
					m_Expenses[k] = -TaxSystem.GetEstimatedTaxAmount(TaxAreaType.Residential, TaxResultType.Expense, m_Lookup, m_Stats, m_TaxRates);
					break;
				case ExpenseSource.ImportPoliceService:
					if (CityUtils.CheckOption(m_CityData[m_City], CityOption.ImportOutsideServices))
					{
						m_Expenses[k] = -GetImportedPoliceServiceFee();
					}
					break;
				case ExpenseSource.ImportAmbulanceService:
					if (CityUtils.CheckOption(m_CityData[m_City], CityOption.ImportOutsideServices))
					{
						m_Expenses[k] = -GetImportedAmbulanceServiceFee();
					}
					break;
				case ExpenseSource.ImportGarbageService:
					if (CityUtils.CheckOption(m_CityData[m_City], CityOption.ImportOutsideServices))
					{
						m_Expenses[k] = -GetImportedGarbageServiceFee();
					}
					break;
				case ExpenseSource.ImportHearseService:
					if (CityUtils.CheckOption(m_CityData[m_City], CityOption.ImportOutsideServices))
					{
						m_Expenses[k] = -GetImportedHearseServiceFee();
					}
					break;
				case ExpenseSource.ImportFireEngineService:
					if (CityUtils.CheckOption(m_CityData[m_City], CityOption.ImportOutsideServices))
					{
						m_Expenses[k] = -GetImportedFireEngineServiceFee();
					}
					break;
				case ExpenseSource.MapTileUpkeep:
					m_Expenses[k] = m_MapTileUpkeepCost;
					break;
				}
			}
			for (int m = 0; m < 14; m++)
			{
				IncomeSource incomeSource = (IncomeSource)m;
				m_Income[m] = 0;
				switch (incomeSource)
				{
				case IncomeSource.ExportElectricity:
					m_Income[m] = ServiceFeeSystem.GetServiceFees(PlayerResource.Electricity, m_CollectedFees).y;
					break;
				case IncomeSource.ExportWater:
					m_Income[m] = ServiceFeeSystem.GetServiceFees(PlayerResource.Water, m_CollectedFees).y;
					break;
				case IncomeSource.FeeEducation:
					m_Income[m] = ServiceFeeSystem.GetServiceFeeIncomeEstimate(PlayerResource.BasicEducation, ServiceFeeSystem.GetFee(PlayerResource.BasicEducation, fees), m_CollectedFees) + ServiceFeeSystem.GetServiceFeeIncomeEstimate(PlayerResource.SecondaryEducation, ServiceFeeSystem.GetFee(PlayerResource.SecondaryEducation, fees), m_CollectedFees) + ServiceFeeSystem.GetServiceFeeIncomeEstimate(PlayerResource.HigherEducation, ServiceFeeSystem.GetFee(PlayerResource.HigherEducation, fees), m_CollectedFees);
					break;
				case IncomeSource.FeeHealthcare:
					m_Income[m] = ServiceFeeSystem.GetServiceFeeIncomeEstimate(PlayerResource.Healthcare, ServiceFeeSystem.GetFee(PlayerResource.Healthcare, fees), m_CollectedFees);
					break;
				case IncomeSource.FeeParking:
					m_Income[m] = ServiceFeeSystem.GetServiceFees(PlayerResource.Parking, m_CollectedFees).x;
					break;
				case IncomeSource.FeePublicTransport:
					m_Income[m] = ServiceFeeSystem.GetServiceFees(PlayerResource.PublicTransport, m_CollectedFees).x;
					break;
				case IncomeSource.FeeGarbage:
					m_Income[m] = ServiceFeeSystem.GetServiceFeeIncomeEstimate(PlayerResource.Garbage, ServiceFeeSystem.GetFee(PlayerResource.Garbage, fees), m_CollectedFees);
					break;
				case IncomeSource.FeeElectricity:
					m_Income[m] = ServiceFeeSystem.GetServiceFeeIncomeEstimate(PlayerResource.Electricity, ServiceFeeSystem.GetFee(PlayerResource.Electricity, fees), m_CollectedFees);
					break;
				case IncomeSource.TaxCommercial:
					m_Income[m] = TaxSystem.GetEstimatedTaxAmount(TaxAreaType.Commercial, TaxResultType.Income, m_Lookup, m_Stats, m_TaxRates);
					break;
				case IncomeSource.TaxIndustrial:
					m_Income[m] = TaxSystem.GetEstimatedTaxAmount(TaxAreaType.Industrial, TaxResultType.Income, m_Lookup, m_Stats, m_TaxRates);
					break;
				case IncomeSource.TaxOffice:
					m_Income[m] = TaxSystem.GetEstimatedTaxAmount(TaxAreaType.Office, TaxResultType.Income, m_Lookup, m_Stats, m_TaxRates);
					break;
				case IncomeSource.TaxResidential:
					m_Income[m] = TaxSystem.GetEstimatedTaxAmount(TaxAreaType.Residential, TaxResultType.Income, m_Lookup, m_Stats, m_TaxRates);
					break;
				case IncomeSource.FeeWater:
					m_Income[m] = ServiceFeeSystem.GetServiceFeeIncomeEstimate(PlayerResource.Water, ServiceFeeSystem.GetFee(PlayerResource.Water, fees), m_CollectedFees) + ServiceFeeSystem.GetServiceFeeIncomeEstimate(PlayerResource.Sewage, ServiceFeeSystem.GetFee(PlayerResource.Water, fees), m_CollectedFees);
					break;
				case IncomeSource.GovernmentSubsidy:
					m_Income[m] = m_MonthlySubsidy;
					break;
				}
			}
			m_TotalTaxes.Value = GetTotalTaxIncome(m_Income);
		}

		private int GetImportedAmbulanceServiceFee()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			Population population = m_Populations[m_City];
			return -(int)(m_OutsideTradeParameterData.m_AmbulanceImportServiceFee * (float)(population.m_Population / m_OutsideTradeParameterData.m_OCServiceTradePopulationRange + 1) * (float)m_OutsideTradeParameterData.m_OCServiceTradePopulationRange);
		}

		private int GetImportedHearseServiceFee()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			Population population = m_Populations[m_City];
			return -(int)(m_OutsideTradeParameterData.m_HearseImportServiceFee * (float)(population.m_Population / m_OutsideTradeParameterData.m_OCServiceTradePopulationRange + 1) * (float)m_OutsideTradeParameterData.m_OCServiceTradePopulationRange);
		}

		private int GetImportedGarbageServiceFee()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			Population population = m_Populations[m_City];
			return -(int)(m_OutsideTradeParameterData.m_GarbageImportServiceFee * (float)(population.m_Population / m_OutsideTradeParameterData.m_OCServiceTradePopulationRange + 1) * (float)m_OutsideTradeParameterData.m_OCServiceTradePopulationRange);
		}

		private int GetImportedFireEngineServiceFee()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			Population population = m_Populations[m_City];
			return -(int)(m_OutsideTradeParameterData.m_FireEngineImportServiceFee * (float)(population.m_Population / m_OutsideTradeParameterData.m_OCServiceTradePopulationRange + 1) * (float)m_OutsideTradeParameterData.m_OCServiceTradePopulationRange);
		}

		private int GetImportedPoliceServiceFee()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			Population population = m_Populations[m_City];
			return -(int)(m_OutsideTradeParameterData.m_PoliceImportServiceFee * (float)(population.m_Population / m_OutsideTradeParameterData.m_OCServiceTradePopulationRange + 1) * (float)m_OutsideTradeParameterData.m_OCServiceTradePopulationRange);
		}
	}

	[BurstCompile]
	private struct ClearServiceDataJob : IJobChunk
	{
		public ComponentTypeHandle<CollectedCityServiceBudgetData> m_BudgetDataType;

		public BufferTypeHandle<CollectedCityServiceUpkeepData> m_UpkeepDataType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<CollectedCityServiceBudgetData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CollectedCityServiceBudgetData>(ref m_BudgetDataType);
			BufferAccessor<CollectedCityServiceUpkeepData> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<CollectedCityServiceUpkeepData>(ref m_UpkeepDataType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				nativeArray[i] = default(CollectedCityServiceBudgetData);
				bufferAccessor[i].Clear();
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct CityServiceBudgetJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		[ReadOnly]
		public ComponentTypeHandle<Building> m_BuildingType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<ResourceData> m_ResourceDatas;

		[ReadOnly]
		public ComponentLookup<ServiceObjectData> m_ServiceObjectDatas;

		[ReadOnly]
		public BufferLookup<ServiceUpkeepData> m_ServiceUpkeepDatas;

		[ReadOnly]
		public ComponentLookup<ServiceUsage> m_ServiceUsages;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgradeBufs;

		[ReadOnly]
		public BufferLookup<Employee> m_EmployeeBufs;

		public ComponentLookup<CollectedCityServiceBudgetData> m_BudgetDatas;

		public BufferLookup<CollectedCityServiceUpkeepData> m_UpkeepDatas;

		[ReadOnly]
		public DynamicBuffer<ServiceBudgetData> m_ServiceBudgets;

		[ReadOnly]
		public ResourcePrefabs m_ResourcePrefabs;

		[ReadOnly]
		public EconomyParameterData m_EconomyParameterData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabType);
			NativeArray<Building> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Building>(ref m_BuildingType);
			bool flag = nativeArray3.Length != 0;
			NativeList<ServiceUpkeepData> totalUpkeepDatas = default(NativeList<ServiceUpkeepData>);
			totalUpkeepDatas._002Ector(4, AllocatorHandle.op_Implicit((Allocator)2));
			ServiceObjectData serviceObjectData = default(ServiceObjectData);
			CollectedCityServiceBudgetData collectedCityServiceBudgetData = default(CollectedCityServiceBudgetData);
			DynamicBuffer<CollectedCityServiceUpkeepData> upkeepDatas = default(DynamicBuffer<CollectedCityServiceUpkeepData>);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity entity = nativeArray[i];
				Entity prefab = nativeArray2[i].m_Prefab;
				if (!m_ServiceObjectDatas.TryGetComponent(prefab, ref serviceObjectData))
				{
					continue;
				}
				Entity service = serviceObjectData.m_Service;
				if (!m_BudgetDatas.TryGetComponent(service, ref collectedCityServiceBudgetData) || !m_UpkeepDatas.TryGetBuffer(service, ref upkeepDatas))
				{
					continue;
				}
				collectedCityServiceBudgetData.m_Count++;
				if (m_ServiceUpkeepDatas.HasBuffer(prefab))
				{
					int serviceBudget = GetServiceBudget(service);
					totalUpkeepDatas.Clear();
					bool flag2 = flag && BuildingUtils.CheckOption(nativeArray3[i], BuildingOption.Inactive);
					CityServiceUpkeepSystem.GetUpkeepWithUsageScale(totalUpkeepDatas, m_ServiceUpkeepDatas, m_InstalledUpgradeBufs, m_Prefabs, m_ServiceUsages, entity, prefab, flag2);
					int upkeepOfEmployeeWage = CityServiceUpkeepSystem.GetUpkeepOfEmployeeWage(m_EmployeeBufs, entity, m_EconomyParameterData, flag2);
					for (int j = 0; j < totalUpkeepDatas.Length; j++)
					{
						ServiceUpkeepData serviceUpkeepData = totalUpkeepDatas[j];
						int amount = serviceUpkeepData.m_Upkeep.m_Amount;
						int num = Mathf.RoundToInt((float)amount * EconomyUtils.GetMarketPrice(serviceUpkeepData.m_Upkeep.m_Resource, m_ResourcePrefabs, ref m_ResourceDatas));
						int num2 = num;
						if (serviceUpkeepData.m_Upkeep.m_Resource == Resource.Money)
						{
							num += upkeepOfEmployeeWage;
							if (flag2)
							{
								num = (int)((float)num * 0.1f);
							}
							num2 = (int)math.round((float)num * ((float)serviceBudget / 100f));
						}
						if (amount > 0)
						{
							ref CollectedCityServiceUpkeepData orCreateUpkeepData = ref GetOrCreateUpkeepData(upkeepDatas, serviceUpkeepData.m_Upkeep.m_Resource);
							orCreateUpkeepData.m_Amount += amount;
							orCreateUpkeepData.m_Cost += num2;
							orCreateUpkeepData.m_FullCost += num;
						}
					}
				}
				m_BudgetDatas[service] = collectedCityServiceBudgetData;
			}
		}

		private int GetServiceBudget(Entity service)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_ServiceBudgets.Length; i++)
			{
				if (m_ServiceBudgets[i].m_Service == service)
				{
					return m_ServiceBudgets[i].m_Budget;
				}
			}
			return 100;
		}

		private ref CollectedCityServiceUpkeepData GetOrCreateUpkeepData(DynamicBuffer<CollectedCityServiceUpkeepData> upkeepDatas, Resource resource)
		{
			for (int i = 0; i < upkeepDatas.Length; i++)
			{
				if (upkeepDatas[i].m_Resource == resource)
				{
					return ref upkeepDatas.ElementAt(i);
				}
			}
			int num = upkeepDatas.Add(new CollectedCityServiceUpkeepData
			{
				m_Resource = resource,
				m_Amount = 0,
				m_Cost = 0,
				m_FullCost = 0
			});
			return ref upkeepDatas.ElementAt(num);
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct ClearBuildingDataJob : IJobChunk
	{
		public ComponentTypeHandle<CollectedServiceBuildingBudgetData> m_BudgetType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<CollectedServiceBuildingBudgetData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CollectedServiceBuildingBudgetData>(ref m_BudgetType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				nativeArray[i] = default(CollectedServiceBuildingBudgetData);
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct CollectServiceBuildingBudgetDatasJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		[ReadOnly]
		public ComponentTypeHandle<WorkProvider> m_WorkProviderType;

		[ReadOnly]
		public BufferTypeHandle<Employee> m_EmployeeType;

		public ComponentLookup<CollectedServiceBuildingBudgetData> m_Budgets;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabType);
			NativeArray<WorkProvider> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WorkProvider>(ref m_WorkProviderType);
			BufferAccessor<Employee> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Employee>(ref m_EmployeeType);
			if (nativeArray2.Length != 0 && bufferAccessor.Length != 0)
			{
				CollectedServiceBuildingBudgetData collectedServiceBuildingBudgetData = default(CollectedServiceBuildingBudgetData);
				for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
				{
					Entity prefab = nativeArray[i].m_Prefab;
					if (m_Budgets.TryGetComponent(prefab, ref collectedServiceBuildingBudgetData))
					{
						collectedServiceBuildingBudgetData.m_Count++;
						collectedServiceBuildingBudgetData.m_Workers += bufferAccessor[i].Length;
						collectedServiceBuildingBudgetData.m_Workplaces += nativeArray2[i].m_MaxWorkers;
						m_Budgets[prefab] = collectedServiceBuildingBudgetData;
					}
				}
				return;
			}
			CollectedServiceBuildingBudgetData collectedServiceBuildingBudgetData2 = default(CollectedServiceBuildingBudgetData);
			for (int j = 0; j < ((ArchetypeChunk)(ref chunk)).Count; j++)
			{
				Entity prefab2 = nativeArray[j].m_Prefab;
				if (m_Budgets.TryGetComponent(prefab2, ref collectedServiceBuildingBudgetData2))
				{
					collectedServiceBuildingBudgetData2.m_Count++;
					m_Budgets[prefab2] = collectedServiceBuildingBudgetData2;
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct NetServiceBudgetJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		[ReadOnly]
		public ComponentTypeHandle<Composition> m_CompositionType;

		[ReadOnly]
		public ComponentTypeHandle<Curve> m_CurveType;

		[ReadOnly]
		public ComponentLookup<ServiceObjectData> m_ServiceObjects;

		[ReadOnly]
		public ComponentLookup<PlaceableNetComposition> m_PlaceableNetCompositionData;

		public ComponentLookup<CollectedCityServiceBudgetData> m_BudgetDatas;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabType);
			NativeArray<Composition> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Composition>(ref m_CompositionType);
			NativeArray<Curve> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Curve>(ref m_CurveType);
			ServiceObjectData serviceObjectData = default(ServiceObjectData);
			PlaceableNetComposition placeableNetData = default(PlaceableNetComposition);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Entity prefab = nativeArray[i].m_Prefab;
				Composition composition = nativeArray2[i];
				Curve curve = nativeArray3[i];
				if (m_PlaceableNetCompositionData.HasComponent(composition.m_Edge) && m_ServiceObjects.TryGetComponent(prefab, ref serviceObjectData) && m_PlaceableNetCompositionData.TryGetComponent(composition.m_Edge, ref placeableNetData))
				{
					AddUpkeepCost(serviceObjectData.m_Service, NetUtils.GetUpkeepCost(curve, placeableNetData));
				}
			}
		}

		private void AddUpkeepCost(Entity service, int upkeep)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			if (m_BudgetDatas.HasComponent(service))
			{
				CollectedCityServiceBudgetData collectedCityServiceBudgetData = m_BudgetDatas[service];
				collectedCityServiceBudgetData.m_BaseCost += upkeep;
				m_BudgetDatas[service] = collectedCityServiceBudgetData;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public BufferLookup<CityStatistic> __Game_City_CityStatistic_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ServiceBudgetData> __Game_Simulation_ServiceBudgetData_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Loan> __Game_Simulation_Loan_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<CityModifier> __Game_City_CityModifier_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<CollectedCityServiceBudgetData> __Game_Simulation_CollectedCityServiceBudgetData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<CollectedCityServiceUpkeepData> __Game_Simulation_CollectedCityServiceUpkeepData_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Creditworthiness> __Game_Simulation_Creditworthiness_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ServiceFee> __Game_City_ServiceFee_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Population> __Game_City_Population_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.City.City> __Game_City_City_RO_ComponentLookup;

		public ComponentTypeHandle<CollectedCityServiceBudgetData> __Game_Simulation_CollectedCityServiceBudgetData_RW_ComponentTypeHandle;

		public BufferTypeHandle<CollectedCityServiceUpkeepData> __Game_Simulation_CollectedCityServiceUpkeepData_RW_BufferTypeHandle;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Building> __Game_Buildings_Building_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ResourceData> __Game_Prefabs_ResourceData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ServiceObjectData> __Game_Prefabs_ServiceObjectData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ServiceUpkeepData> __Game_Prefabs_ServiceUpkeepData_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<ServiceUsage> __Game_Buildings_ServiceUsage_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Employee> __Game_Companies_Employee_RO_BufferLookup;

		public ComponentLookup<CollectedCityServiceBudgetData> __Game_Simulation_CollectedCityServiceBudgetData_RW_ComponentLookup;

		public BufferLookup<CollectedCityServiceUpkeepData> __Game_Simulation_CollectedCityServiceUpkeepData_RW_BufferLookup;

		public ComponentTypeHandle<CollectedServiceBuildingBudgetData> __Game_Simulation_CollectedServiceBuildingBudgetData_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WorkProvider> __Game_Companies_WorkProvider_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Employee> __Game_Companies_Employee_RO_BufferTypeHandle;

		public ComponentLookup<CollectedServiceBuildingBudgetData> __Game_Simulation_CollectedServiceBuildingBudgetData_RW_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<Composition> __Game_Net_Composition_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Curve> __Game_Net_Curve_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<PlaceableNetComposition> __Game_Prefabs_PlaceableNetComposition_RO_ComponentLookup;

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
			__Game_City_CityStatistic_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityStatistic>(true);
			__Game_Simulation_ServiceBudgetData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceBudgetData>(true);
			__Game_Simulation_Loan_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Loan>(true);
			__Game_City_CityModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(true);
			__Game_Simulation_CollectedCityServiceBudgetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CollectedCityServiceBudgetData>(true);
			__Game_Simulation_CollectedCityServiceUpkeepData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CollectedCityServiceUpkeepData>(true);
			__Game_Simulation_Creditworthiness_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Creditworthiness>(true);
			__Game_City_ServiceFee_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceFee>(true);
			__Game_City_Population_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Population>(true);
			__Game_City_City_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.City.City>(true);
			__Game_Simulation_CollectedCityServiceBudgetData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CollectedCityServiceBudgetData>(false);
			__Game_Simulation_CollectedCityServiceUpkeepData_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<CollectedCityServiceUpkeepData>(false);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Buildings_Building_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Building>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ResourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceData>(true);
			__Game_Prefabs_ServiceObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceObjectData>(true);
			__Game_Prefabs_ServiceUpkeepData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceUpkeepData>(true);
			__Game_Buildings_ServiceUsage_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceUsage>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<InstalledUpgrade>(true);
			__Game_Companies_Employee_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Employee>(true);
			__Game_Simulation_CollectedCityServiceBudgetData_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CollectedCityServiceBudgetData>(false);
			__Game_Simulation_CollectedCityServiceUpkeepData_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CollectedCityServiceUpkeepData>(false);
			__Game_Simulation_CollectedServiceBuildingBudgetData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CollectedServiceBuildingBudgetData>(false);
			__Game_Companies_WorkProvider_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WorkProvider>(true);
			__Game_Companies_Employee_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Employee>(true);
			__Game_Simulation_CollectedServiceBuildingBudgetData_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CollectedServiceBuildingBudgetData>(false);
			__Game_Net_Composition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Composition>(true);
			__Game_Net_Curve_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Curve>(true);
			__Game_Prefabs_PlaceableNetComposition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceableNetComposition>(true);
		}
	}

	private CitySystem m_CitySystem;

	private CityStatisticsSystem m_CityStatisticsSystem;

	private ResourceSystem m_ResourceSystem;

	private ServiceFeeSystem m_ServiceFeeSystem;

	private TaxSystem m_TaxSystem;

	private MapTilePurchaseSystem m_MapTilePurchaseSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private GameModeGovernmentSubsidiesSystem m_GameModeGovernmentSubsidiesSystem;

	private EntityQuery m_BudgetDataQuery;

	private EntityQuery m_ServiceBuildingQuery;

	private EntityQuery m_ServiceQuery;

	private EntityQuery m_UpkeepGroup;

	private EntityQuery m_ServiceObjectQuery;

	private EntityQuery m_NetUpkeepQuery;

	private EntityQuery m_EconomyParameterQuery;

	private EntityQuery m_OutsideTradeParameterQuery;

	private EntityQuery m_HealthcareFacilityQuery;

	private EntityQuery m_DeathcareFacilityQuery;

	private EntityQuery m_GarbageFacilityQuery;

	private EntityQuery m_FireStationQuery;

	private EntityQuery m_PoliceStationQuery;

	protected NativeArray<int> m_Income;

	protected NativeArray<int> m_IncomeTemp;

	protected NativeArray<int> m_TotalIncome;

	protected NativeArray<int> m_Expenses;

	protected NativeArray<int> m_ExpensesTemp;

	private int m_TotalTaxIncome;

	private NativeReference<int> m_TotalTaxes;

	private NativeParallelHashMap<Entity, CollectedCityServiceBudgetData> m_CityServiceBudgets;

	private NativeParallelHashMap<Entity, int2> m_CityServiceUpkeepIndices;

	private NativeList<CollectedCityServiceUpkeepData> m_CityServiceUpkeeps;

	private JobHandle m_TempArrayDeps;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_844909884_0;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		m_ServiceFeeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ServiceFeeSystem>();
		m_TaxSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TaxSystem>();
		m_MapTilePurchaseSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<MapTilePurchaseSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_GameModeGovernmentSubsidiesSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GameModeGovernmentSubsidiesSystem>();
		m_TotalIncome = new NativeArray<int>(14, (Allocator)4, (NativeArrayOptions)1);
		m_Income = new NativeArray<int>(14, (Allocator)4, (NativeArrayOptions)1);
		m_IncomeTemp = new NativeArray<int>(14, (Allocator)4, (NativeArrayOptions)1);
		m_Expenses = new NativeArray<int>(15, (Allocator)4, (NativeArrayOptions)1);
		m_ExpensesTemp = new NativeArray<int>(15, (Allocator)4, (NativeArrayOptions)1);
		m_CityServiceUpkeepIndices = new NativeParallelHashMap<Entity, int2>(4, AllocatorHandle.op_Implicit((Allocator)4));
		m_CityServiceUpkeeps = new NativeList<CollectedCityServiceUpkeepData>(4, AllocatorHandle.op_Implicit((Allocator)4));
		m_CityServiceBudgets = new NativeParallelHashMap<Entity, CollectedCityServiceBudgetData>(4, AllocatorHandle.op_Implicit((Allocator)4));
		m_TotalTaxes = new NativeReference<int>(AllocatorHandle.op_Implicit((Allocator)4), (NativeArrayOptions)1);
		m_ServiceQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<CollectedCityServiceBudgetData>(),
			ComponentType.ReadWrite<CollectedCityServiceUpkeepData>()
		});
		m_ServiceObjectQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<CollectedServiceBuildingBudgetData>(),
			ComponentType.ReadOnly<ServiceObjectData>()
		});
		m_UpkeepGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<CityServiceUpkeep>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_BudgetDataQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ServiceBudgetData>() });
		m_ServiceBuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<CityServiceUpkeep>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_NetUpkeepQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[7]
		{
			ComponentType.ReadOnly<Composition>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.Exclude<Owner>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Destroyed>(),
			ComponentType.Exclude<Native>(),
			ComponentType.Exclude<Temp>()
		});
		m_EconomyParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
		m_HealthcareFacilityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Game.Buildings.Hospital>(),
			ComponentType.Exclude<Game.Objects.OutsideConnection>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_DeathcareFacilityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Game.Buildings.DeathcareFacility>(),
			ComponentType.Exclude<Game.Objects.OutsideConnection>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_GarbageFacilityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Game.Buildings.GarbageFacility>(),
			ComponentType.Exclude<Game.Objects.OutsideConnection>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_FireStationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Game.Buildings.FireStation>(),
			ComponentType.Exclude<Game.Objects.OutsideConnection>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_PoliceStationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Game.Buildings.PoliceStation>(),
			ComponentType.Exclude<Game.Objects.OutsideConnection>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_OutsideTradeParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<OutsideTradeParameterData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_BudgetDataQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_ServiceQuery);
	}

	protected override void OnGamePreload(Purpose purpose, GameMode mode)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGamePreload(purpose, mode);
		((ComponentSystemBase)this).Enabled = mode.IsGame();
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityQuery)(ref m_BudgetDataQuery)).CalculateEntityCount() == 0)
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).CreateEntity((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadWrite<ServiceBudgetData>() });
		}
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_Income.Dispose();
		m_IncomeTemp.Dispose();
		m_TotalIncome.Dispose();
		m_Expenses.Dispose();
		m_ExpensesTemp.Dispose();
		m_CityServiceUpkeeps.Dispose();
		m_CityServiceBudgets.Dispose();
		m_CityServiceUpkeepIndices.Dispose();
		m_TotalTaxes.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Unknown result type (might be due to invalid IL or missing references)
		//IL_0525: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0573: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_0595: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0600: Unknown result type (might be due to invalid IL or missing references)
		//IL_0605: Unknown result type (might be due to invalid IL or missing references)
		//IL_0622: Unknown result type (might be due to invalid IL or missing references)
		//IL_0627: Unknown result type (might be due to invalid IL or missing references)
		//IL_063f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0644: Unknown result type (might be due to invalid IL or missing references)
		//IL_065c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0661: Unknown result type (might be due to invalid IL or missing references)
		//IL_0679: Unknown result type (might be due to invalid IL or missing references)
		//IL_067e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0696: Unknown result type (might be due to invalid IL or missing references)
		//IL_069b: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d7: Unknown result type (might be due to invalid IL or missing references)
		m_TotalTaxIncome = m_TotalTaxes.Value;
		((JobHandle)(ref m_TempArrayDeps)).Complete();
		m_IncomeTemp.CopyTo(m_Income);
		m_ExpensesTemp.CopyTo(m_Expenses);
		JobHandle val = default(JobHandle);
		NativeList<Entity> cityServiceEntities = ((EntityQuery)(ref m_ServiceQuery)).ToEntityListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		UpdateDataJob updateDataJob = new UpdateDataJob
		{
			m_CityServiceEntities = cityServiceEntities,
			m_City = m_CitySystem.City,
			m_Lookup = m_CityStatisticsSystem.GetLookup(),
			m_Stats = InternalCompilerInterface.GetBufferLookup<CityStatistic>(ref __TypeHandle.__Game_City_CityStatistic_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CollectedFees = m_ServiceFeeSystem.GetServiceFees(),
			m_BudgetDatas = InternalCompilerInterface.GetBufferLookup<ServiceBudgetData>(ref __TypeHandle.__Game_Simulation_ServiceBudgetData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Loans = InternalCompilerInterface.GetComponentLookup<Loan>(ref __TypeHandle.__Game_Simulation_Loan_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BudgetEntity = ((EntityQuery)(ref m_BudgetDataQuery)).GetSingletonEntity(),
			m_CityModifiers = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CityServiceBudgets = m_CityServiceBudgets,
			m_CityServiceUpkeepIndices = m_CityServiceUpkeepIndices,
			m_CityServiceUpkeeps = m_CityServiceUpkeeps,
			m_CollectedBudgets = InternalCompilerInterface.GetComponentLookup<CollectedCityServiceBudgetData>(ref __TypeHandle.__Game_Simulation_CollectedCityServiceBudgetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CollectedUpkeeps = InternalCompilerInterface.GetBufferLookup<CollectedCityServiceUpkeepData>(ref __TypeHandle.__Game_Simulation_CollectedCityServiceUpkeepData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Creditworthiness = InternalCompilerInterface.GetComponentLookup<Creditworthiness>(ref __TypeHandle.__Game_Simulation_Creditworthiness_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Expenses = m_ExpensesTemp,
			m_ServiceFees = InternalCompilerInterface.GetBufferLookup<ServiceFee>(ref __TypeHandle.__Game_City_ServiceFee_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Income = m_IncomeTemp,
			m_TaxRates = m_TaxSystem.GetTaxRates(),
			m_Populations = InternalCompilerInterface.GetComponentLookup<Population>(ref __TypeHandle.__Game_City_Population_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CityData = InternalCompilerInterface.GetComponentLookup<Game.City.City>(ref __TypeHandle.__Game_City_City_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OutsideTradeParameterData = ((EntityQuery)(ref m_OutsideTradeParameterQuery)).GetSingleton<OutsideTradeParameterData>(),
			m_EconomyParametersData = ((EntityQuery)(ref m_EconomyParameterQuery)).GetSingleton<EconomyParameterData>(),
			m_MonthlySubsidy = m_GameModeGovernmentSubsidiesSystem.monthlySubsidy,
			m_TotalTaxes = m_TotalTaxes,
			m_ServiceFacilityBuildingCount = new int4(((EntityQuery)(ref m_HealthcareFacilityQuery)).CalculateEntityCount(), ((EntityQuery)(ref m_DeathcareFacilityQuery)).CalculateEntityCount(), ((EntityQuery)(ref m_GarbageFacilityQuery)).CalculateEntityCount(), ((EntityQuery)(ref m_FireStationQuery)).CalculateEntityCount()),
			m_PoliceStationBuildingCount = ((EntityQuery)(ref m_PoliceStationQuery)).CalculateEntityCount(),
			m_MapTileUpkeepCost = ((!m_CityConfigurationSystem.unlockMapTiles) ? m_MapTilePurchaseSystem.CalculateOwnedTilesUpkeep() : 0)
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<UpdateDataJob>(updateDataJob, JobHandle.CombineDependencies(val, m_TempArrayDeps, ((SystemBase)this).Dependency));
		m_TaxSystem.AddReader(((SystemBase)this).Dependency);
		m_TempArrayDeps = ((SystemBase)this).Dependency;
		cityServiceEntities.Dispose(((SystemBase)this).Dependency);
		ClearServiceDataJob clearServiceDataJob = new ClearServiceDataJob
		{
			m_BudgetDataType = InternalCompilerInterface.GetComponentTypeHandle<CollectedCityServiceBudgetData>(ref __TypeHandle.__Game_Simulation_CollectedCityServiceBudgetData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpkeepDataType = InternalCompilerInterface.GetBufferTypeHandle<CollectedCityServiceUpkeepData>(ref __TypeHandle.__Game_Simulation_CollectedCityServiceUpkeepData_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef)
		};
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<ClearServiceDataJob>(clearServiceDataJob, m_ServiceQuery, ((SystemBase)this).Dependency);
		CityServiceBudgetJob cityServiceBudgetJob = new CityServiceBudgetJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingType = InternalCompilerInterface.GetComponentTypeHandle<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceObjectDatas = InternalCompilerInterface.GetComponentLookup<ServiceObjectData>(ref __TypeHandle.__Game_Prefabs_ServiceObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceUpkeepDatas = InternalCompilerInterface.GetBufferLookup<ServiceUpkeepData>(ref __TypeHandle.__Game_Prefabs_ServiceUpkeepData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceUsages = InternalCompilerInterface.GetComponentLookup<ServiceUsage>(ref __TypeHandle.__Game_Buildings_ServiceUsage_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgradeBufs = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EmployeeBufs = InternalCompilerInterface.GetBufferLookup<Employee>(ref __TypeHandle.__Game_Companies_Employee_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BudgetDatas = InternalCompilerInterface.GetComponentLookup<CollectedCityServiceBudgetData>(ref __TypeHandle.__Game_Simulation_CollectedCityServiceBudgetData_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UpkeepDatas = InternalCompilerInterface.GetBufferLookup<CollectedCityServiceUpkeepData>(ref __TypeHandle.__Game_Simulation_CollectedCityServiceUpkeepData_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceBudgets = ((EntityQuery)(ref m_BudgetDataQuery)).GetSingletonBuffer<ServiceBudgetData>(true),
			m_ResourcePrefabs = m_ResourceSystem.GetPrefabs(),
			m_EconomyParameterData = ((EntityQuery)(ref m_EconomyParameterQuery)).GetSingleton<EconomyParameterData>()
		};
		((SystemBase)this).Dependency = JobChunkExtensions.Schedule<CityServiceBudgetJob>(cityServiceBudgetJob, m_UpkeepGroup, ((SystemBase)this).Dependency);
		m_ResourceSystem.AddPrefabsReader(((SystemBase)this).Dependency);
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<ClearBuildingDataJob>(new ClearBuildingDataJob
		{
			m_BudgetType = InternalCompilerInterface.GetComponentTypeHandle<CollectedServiceBuildingBudgetData>(ref __TypeHandle.__Game_Simulation_CollectedServiceBuildingBudgetData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef)
		}, m_ServiceObjectQuery, ((SystemBase)this).Dependency);
		JobHandle val3 = JobChunkExtensions.Schedule<CollectServiceBuildingBudgetDatasJob>(new CollectServiceBuildingBudgetDatasJob
		{
			m_WorkProviderType = InternalCompilerInterface.GetComponentTypeHandle<WorkProvider>(ref __TypeHandle.__Game_Companies_WorkProvider_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EmployeeType = InternalCompilerInterface.GetBufferTypeHandle<Employee>(ref __TypeHandle.__Game_Companies_Employee_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Budgets = InternalCompilerInterface.GetComponentLookup<CollectedServiceBuildingBudgetData>(ref __TypeHandle.__Game_Simulation_CollectedServiceBuildingBudgetData_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
		}, m_ServiceBuildingQuery, val2);
		JobHandle val4 = JobChunkExtensions.Schedule<NetServiceBudgetJob>(new NetServiceBudgetJob
		{
			m_CompositionType = InternalCompilerInterface.GetComponentTypeHandle<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurveType = InternalCompilerInterface.GetComponentTypeHandle<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceObjects = InternalCompilerInterface.GetComponentLookup<ServiceObjectData>(ref __TypeHandle.__Game_Prefabs_ServiceObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PlaceableNetCompositionData = InternalCompilerInterface.GetComponentLookup<PlaceableNetComposition>(ref __TypeHandle.__Game_Prefabs_PlaceableNetComposition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BudgetDatas = InternalCompilerInterface.GetComponentLookup<CollectedCityServiceBudgetData>(ref __TypeHandle.__Game_Simulation_CollectedCityServiceBudgetData_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
		}, m_NetUpkeepQuery, ((SystemBase)this).Dependency);
		((SystemBase)this).Dependency = JobHandle.CombineDependencies(val3, val4);
	}

	public NativeArray<int> GetIncomeArray(out JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		deps = m_TempArrayDeps;
		return m_IncomeTemp;
	}

	public NativeArray<int> GetExpenseArray(out JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		deps = m_TempArrayDeps;
		return m_ExpensesTemp;
	}

	public void AddArrayReader(JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_TempArrayDeps = JobHandle.CombineDependencies(m_TempArrayDeps, deps);
	}

	public int GetBalance()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return GetBalance(m_Income, m_Expenses);
	}

	public static int GetBalance(NativeArray<int> income, NativeArray<int> expenses)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return GetTotalIncome(income) + GetTotalExpenses(expenses);
	}

	public int GetTotalIncome()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetTotalIncome(m_Income);
	}

	public static int GetTotalIncome(NativeArray<int> income)
	{
		int num = 0;
		for (int i = 0; i < income.Length; i++)
		{
			num += income[i];
		}
		return num;
	}

	public int GetTotalExpenses()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetTotalExpenses(m_Expenses);
	}

	public static int GetTotalExpenses(NativeArray<int> expenses)
	{
		int num = 0;
		for (int i = 0; i < expenses.Length; i++)
		{
			num -= expenses[i];
		}
		return num;
	}

	public int GetTotalTaxIncome()
	{
		return m_TotalTaxIncome;
	}

	public int GetIncome(IncomeSource source)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return GetIncome(source, m_Income);
	}

	public static int GetIncome(IncomeSource source, NativeArray<int> income)
	{
		if ((int)source < income.Length)
		{
			return income[(int)source];
		}
		return 0;
	}

	public int GetTotalIncome(IncomeSource source)
	{
		if ((int)source < m_TotalIncome.Length)
		{
			return m_TotalIncome[(int)source];
		}
		return 0;
	}

	public int GetExpense(ExpenseSource source)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return GetExpense(source, m_Expenses);
	}

	public static int GetExpense(ExpenseSource source, NativeArray<int> expenses)
	{
		if ((int)source < expenses.Length)
		{
			return expenses[(int)source];
		}
		return 0;
	}

	public int GetMoneyDelta()
	{
		int num = 0;
		for (int i = 0; i < 15; i++)
		{
			num -= GetExpense((ExpenseSource)i);
		}
		for (int j = 0; j < 14; j++)
		{
			num += GetIncome((IncomeSource)j);
		}
		return num / 24;
	}

	public int GetServiceBudget(Entity servicePrefab)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_TempArrayDeps)).Complete();
		return GetServiceBudget(servicePrefab, m_CityServiceBudgets, ((EntityQuery)(ref m_BudgetDataQuery)).GetSingletonEntity(), InternalCompilerInterface.GetBufferLookup<ServiceBudgetData>(ref __TypeHandle.__Game_Simulation_ServiceBudgetData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef));
	}

	public static int GetServiceBudget(Entity servicePrefab, NativeParallelHashMap<Entity, CollectedCityServiceBudgetData> budgets, Entity budgetEntity, BufferLookup<ServiceBudgetData> budgetDatas)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (!budgets.ContainsKey(servicePrefab))
		{
			return 0;
		}
		DynamicBuffer<ServiceBudgetData> val = budgetDatas[budgetEntity];
		for (int i = 0; i < val.Length; i++)
		{
			ServiceBudgetData serviceBudgetData = val[i];
			if (serviceBudgetData.m_Service == servicePrefab)
			{
				return serviceBudgetData.m_Budget;
			}
		}
		return 100;
	}

	public void SetServiceBudget(Entity servicePrefab, int percentage)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_TempArrayDeps)).Complete();
		if (!m_CityServiceBudgets.ContainsKey(servicePrefab))
		{
			return;
		}
		Entity singletonEntity = ((EntityQuery)(ref m_BudgetDataQuery)).GetSingletonEntity();
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<ServiceBudgetData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<ServiceBudgetData>(singletonEntity, false);
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < buffer.Length; i++)
		{
			ServiceBudgetData serviceBudgetData = buffer[i];
			if (serviceBudgetData.m_Service == servicePrefab)
			{
				flag = serviceBudgetData.m_Budget != percentage;
				serviceBudgetData.m_Budget = percentage;
				buffer[i] = serviceBudgetData;
				flag2 = true;
				break;
			}
		}
		if (!flag2)
		{
			flag = true;
			buffer.Add(new ServiceBudgetData
			{
				m_Service = servicePrefab,
				m_Budget = percentage
			});
		}
		if (flag)
		{
			EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
			((EntityCommandBuffer)(ref val)).AddComponent<Updated>(singletonEntity);
		}
	}

	public int GetServiceEfficiency(Entity servicePrefab, int budget)
	{
		BuildingEfficiencyParameterData singleton = ((EntityQuery)(ref __query_844909884_0)).GetSingleton<BuildingEfficiencyParameterData>();
		return Mathf.RoundToInt(100f * ((AnimationCurve1)(ref singleton.m_ServiceBudgetEfficiencyFactor)).Evaluate((float)budget / 100f));
	}

	private static int GetEstimatedServiceUpkeep(CollectedCityServiceBudgetData data, int2 indices, int budget, NativeList<CollectedCityServiceUpkeepData> upkeeps)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		int num = data.m_BaseCost;
		for (int i = indices.x; i < indices.x + indices.y; i++)
		{
			CollectedCityServiceUpkeepData collectedCityServiceUpkeepData = upkeeps[i];
			int num2 = Mathf.RoundToInt((float)collectedCityServiceUpkeepData.m_FullCost);
			if (collectedCityServiceUpkeepData.m_Resource == Resource.Money)
			{
				num2 = Mathf.RoundToInt((float)num2 * ((float)budget / 100f));
			}
			num += num2;
		}
		return num;
	}

	public void GetEstimatedServiceBudget(Entity servicePrefab, out int upkeep)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_TempArrayDeps)).Complete();
		GetEstimatedServiceBudget(servicePrefab, out upkeep, m_CityServiceBudgets, m_CityServiceUpkeeps, m_CityServiceUpkeepIndices, ((EntityQuery)(ref m_BudgetDataQuery)).GetSingletonEntity(), ((SystemBase)this).GetBufferLookup<ServiceBudgetData>(true));
	}

	public static void GetEstimatedServiceBudget(Entity servicePrefab, out int upkeep, NativeParallelHashMap<Entity, CollectedCityServiceBudgetData> budgets, NativeList<CollectedCityServiceUpkeepData> upkeeps, NativeParallelHashMap<Entity, int2> upkeepIndices, Entity budgetEntity, BufferLookup<ServiceBudgetData> budgetDatas)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (!budgets.ContainsKey(servicePrefab))
		{
			upkeep = 0;
			return;
		}
		int serviceBudget = GetServiceBudget(servicePrefab, budgets, budgetEntity, budgetDatas);
		CollectedCityServiceBudgetData data = budgets[servicePrefab];
		int2 indices = upkeepIndices[servicePrefab];
		upkeep = GetEstimatedServiceUpkeep(data, indices, serviceBudget, upkeeps);
	}

	public int GetNumberOfServiceBuildings(Entity serviceBuildingPrefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<CollectedServiceBuildingBudgetData>(serviceBuildingPrefab))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).GetComponentData<CollectedServiceBuildingBudgetData>(serviceBuildingPrefab).m_Count;
		}
		return 0;
	}

	public int2 GetWorkersAndWorkplaces(Entity serviceBuildingPrefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<CollectedServiceBuildingBudgetData>(serviceBuildingPrefab))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			CollectedServiceBuildingBudgetData componentData = ((EntityManager)(ref entityManager)).GetComponentData<CollectedServiceBuildingBudgetData>(serviceBuildingPrefab);
			return new int2(componentData.m_Workers, componentData.m_Workplaces);
		}
		return new int2(0, 0);
	}

	public Entity[] GetServiceBuildings(Entity servicePrefab)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_ServiceObjectQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<ServiceObjectData> val2 = ((EntityQuery)(ref m_ServiceObjectQuery)).ToComponentDataArray<ServiceObjectData>(AllocatorHandle.op_Implicit((Allocator)3));
		List<Entity> list = new List<Entity>(4);
		for (int i = 0; i < val.Length; i++)
		{
			if (val2[i].m_Service == servicePrefab)
			{
				list.Add(val[i]);
			}
		}
		val.Dispose();
		val2.Dispose();
		return list.ToArray();
	}

	private static int GetTotalTaxIncome(NativeArray<int> income)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		return GetIncome(IncomeSource.TaxCommercial, income) + GetIncome(IncomeSource.TaxIndustrial, income) + GetIncome(IncomeSource.TaxResidential, income) + GetIncome(IncomeSource.TaxOffice, income);
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
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<BuildingEfficiencyParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_844909884_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
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
	public CityServiceBudgetSystem()
	{
	}
}
