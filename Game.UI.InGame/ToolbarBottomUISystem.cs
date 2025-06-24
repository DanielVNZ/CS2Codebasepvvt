using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Colossal.UI.Binding;
using Game.City;
using Game.Prefabs;
using Game.Simulation;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class ToolbarBottomUISystem : UISystemBase
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct TypeHandle
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
		}
	}

	private const string kGroup = "toolbarBottom";

	private PrefabSystem m_PrefabSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private ICityStatisticsSystem m_CityStatisticsSystem;

	private CitySystem m_CitySystem;

	private ICityServiceBudgetSystem m_CityServiceBudgetSystem;

	private GetterValueBinding<string> m_CityNameBinding;

	private GetterValueBinding<int> m_MoneyBinding;

	private GetterValueBinding<int> m_MoneyDeltaBinding;

	private GetterValueBinding<int> m_PopulationBinding;

	private GetterValueBinding<int> m_PopulationDeltaBinding;

	private GetterValueBinding<bool> m_UnlimitedMoneyBinding;

	private UIToolbarBottomConfigurationPrefab m_ToolbarBottomConfigurationPrefab;

	private EntityQuery m_ToolbarBottomConfigurationQuery;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_2118611066_0;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_CityServiceBudgetSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityServiceBudgetSystem>();
		m_ToolbarBottomConfigurationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<UIToolbarBottomConfigurationData>() });
		AddBinding((IBinding)(object)(m_CityNameBinding = new GetterValueBinding<string>("toolbarBottom", "cityName", (Func<string>)(() => m_CityConfigurationSystem.cityName ?? ""), (IWriter<string>)null, (EqualityComparer<string>)null)));
		AddBinding((IBinding)(object)(m_MoneyBinding = new GetterValueBinding<int>("toolbarBottom", "money", (Func<int>)(() => m_CitySystem.moneyAmount), (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_MoneyDeltaBinding = new GetterValueBinding<int>("toolbarBottom", "moneyDelta", (Func<int>)m_CityServiceBudgetSystem.GetMoneyDelta, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_UnlimitedMoneyBinding = new GetterValueBinding<bool>("toolbarBottom", "unlimitedMoney", (Func<bool>)(() => m_CityConfigurationSystem.unlimitedMoney), (IWriter<bool>)null, (EqualityComparer<bool>)null)));
		AddBinding((IBinding)(object)(m_PopulationBinding = new GetterValueBinding<int>("toolbarBottom", "population", (Func<int>)GetPopulation, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_PopulationDeltaBinding = new GetterValueBinding<int>("toolbarBottom", "populationDelta", (Func<int>)GetPopulationDelta, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)new GetterValueBinding<float2>("toolbarBottom", "populationTrendThresholds", (Func<float2>)(() => new float2(m_ToolbarBottomConfigurationPrefab.m_PopulationTrendThresholds.m_Medium, m_ToolbarBottomConfigurationPrefab.m_PopulationTrendThresholds.m_High)), (IWriter<float2>)null, (EqualityComparer<float2>)null));
		AddBinding((IBinding)(object)new GetterValueBinding<float2>("toolbarBottom", "moneyTrendThresholds", (Func<float2>)(() => new float2(m_ToolbarBottomConfigurationPrefab.m_MoneyTrendThresholds.m_Medium, m_ToolbarBottomConfigurationPrefab.m_MoneyTrendThresholds.m_High)), (IWriter<float2>)null, (EqualityComparer<float2>)null));
		AddBinding((IBinding)(object)new TriggerBinding<string>("toolbarBottom", "setCityName", (Action<string>)SetCityName, (IReader<string>)null));
		((ComponentSystemBase)this).RequireForUpdate(m_ToolbarBottomConfigurationQuery);
	}

	private int GetPopulation()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Population>(m_CitySystem.City))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).GetComponentData<Population>(m_CitySystem.City).m_Population;
		}
		return 0;
	}

	private int GetPopulationDelta()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		Population population = default(Population);
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Population>(m_CitySystem.City))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			population = ((EntityManager)(ref entityManager)).GetComponentData<Population>(m_CitySystem.City);
		}
		NativeArray<int> statisticDataArray = m_CityStatisticsSystem.GetStatisticDataArray(StatisticType.Population);
		if (statisticDataArray.Length == 0)
		{
			return population.m_Population;
		}
		int num = ((statisticDataArray.Length >= 2) ? statisticDataArray[statisticDataArray.Length - 2] : 0);
		int num2 = statisticDataArray[statisticDataArray.Length - 1];
		float num3 = (float)(long)(m_CityStatisticsSystem.GetSampleFrameIndex(m_CityStatisticsSystem.sampleCount - 1) % 8192) / 8192f;
		return (population.m_Population - Mathf.RoundToInt(math.lerp((float)num, (float)num2, num3))) * 32 / 24;
	}

	private void SetCityName(string name)
	{
		m_CityConfigurationSystem.cityName = name;
		m_CityNameBinding.Update();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		m_CityNameBinding.Update();
		m_MoneyBinding.Update();
		m_PopulationBinding.Update();
		m_MoneyDeltaBinding.Update();
		m_PopulationDeltaBinding.Update();
		m_UnlimitedMoneyBinding.Update();
		if ((Object)(object)m_ToolbarBottomConfigurationPrefab == (Object)null)
		{
			Entity singletonEntity = ((EntityQuery)(ref __query_2118611066_0)).GetSingletonEntity();
			m_ToolbarBottomConfigurationPrefab = m_PrefabSystem.GetPrefab<UIToolbarBottomConfigurationPrefab>(singletonEntity);
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
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<UIToolbarBottomConfigurationData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_2118611066_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
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
	public ToolbarBottomUISystem()
	{
	}
}
