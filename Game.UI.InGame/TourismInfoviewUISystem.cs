using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Prefabs;
using Game.Simulation;
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
public class TourismInfoviewUISystem : InfoviewUISystemBase
{
	private enum Result
	{
		Price,
		HotelCount,
		ResultCount
	}

	[BurstCompile]
	private struct CalculateAverageHotelPriceJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<LodgingProvider> m_LodgingProviderHandle;

		public NativeArray<int> m_Results;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<LodgingProvider> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<LodgingProvider>(ref m_LodgingProviderHandle);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				LodgingProvider lodgingProvider = nativeArray[i];
				if (lodgingProvider.m_Price > 0)
				{
					ref NativeArray<int> reference = ref m_Results;
					reference[0] = reference[0] + lodgingProvider.m_Price;
					reference = ref m_Results;
					reference[1] = reference[1] + 1;
				}
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
		public ComponentTypeHandle<LodgingProvider> __Game_Companies_LodgingProvider_RO_ComponentTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			__Game_Companies_LodgingProvider_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<LodgingProvider>(true);
		}
	}

	private const string kGroup = "tourismInfo";

	private ClimateSystem m_ClimateSystem;

	private CityStatisticsSystem m_CityStatisticsSystem;

	private CitySystem m_CitySystem;

	private ValueBinding<IndicatorValue> m_Attractiveness;

	private ValueBinding<int> m_TourismRate;

	private ValueBinding<float> m_AverageHotelPrice;

	private ValueBinding<float> m_WeatherEffect;

	private EntityQuery m_HotelQuery;

	private EntityQuery m_HotelModifiedQuery;

	private NativeArray<int> m_Results;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_1647950437_0;

	protected override bool Active
	{
		get
		{
			if (!base.Active && !((EventBindingBase)m_Attractiveness).active && !((EventBindingBase)m_TourismRate).active && !((EventBindingBase)m_AverageHotelPrice).active)
			{
				return ((EventBindingBase)m_WeatherEffect).active;
			}
			return true;
		}
	}

	protected override bool Modified => !((EntityQuery)(ref m_HotelModifiedQuery)).IsEmptyIgnoreFilter;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Expected O, but got Unknown
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ClimateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ClimateSystem>();
		m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		AddBinding((IBinding)(object)(m_Attractiveness = new ValueBinding<IndicatorValue>("tourismInfo", "attractiveness", default(IndicatorValue), (IWriter<IndicatorValue>)(object)new ValueWriter<IndicatorValue>(), (EqualityComparer<IndicatorValue>)null)));
		AddBinding((IBinding)(object)(m_TourismRate = new ValueBinding<int>("tourismInfo", "tourismRate", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_AverageHotelPrice = new ValueBinding<float>("tourismInfo", "averageHotelPrice", 0f, (IWriter<float>)null, (EqualityComparer<float>)null)));
		AddBinding((IBinding)(object)(m_WeatherEffect = new ValueBinding<float>("tourismInfo", "weatherEffect", 0f, (IWriter<float>)null, (EqualityComparer<float>)null)));
		m_HotelQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<PropertyRenter>(),
			ComponentType.ReadOnly<LodgingProvider>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PropertyRenter>(),
			ComponentType.ReadOnly<LodgingProvider>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Created>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array[0] = val;
		m_HotelModifiedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		((ComponentSystemBase)this).RequireForUpdate<AttractivenessParameterData>();
		m_Results = new NativeArray<int>(2, (Allocator)4, (NativeArrayOptions)1);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_Results.Dispose();
		base.OnDestroy();
	}

	protected override void PerformUpdate()
	{
		UpdateAttractiveness();
		UpdateTourismRate();
		UpdateWeatherEffect();
		UpdateAverageHotelPrice();
	}

	private void UpdateAttractiveness()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Tourism tourism = default(Tourism);
		if (EntitiesExtensions.TryGetComponent<Tourism>(((ComponentSystemBase)this).EntityManager, m_CitySystem.City, ref tourism))
		{
			m_Attractiveness.Update(new IndicatorValue(0f, 100f, tourism.m_Attractiveness));
		}
	}

	private void UpdateTourismRate()
	{
		m_TourismRate.Update(m_CityStatisticsSystem.GetStatisticValue(StatisticType.TouristCount));
	}

	private void UpdateWeatherEffect()
	{
		m_WeatherEffect.Update(100f * (0f - (1f - TourismSystem.GetWeatherEffect(((EntityQuery)(ref __query_1647950437_0)).GetSingleton<AttractivenessParameterData>(), m_ClimateSystem.classification, m_ClimateSystem.temperature, m_ClimateSystem.precipitation, m_ClimateSystem.isRaining, m_ClimateSystem.isSnowing))));
	}

	private void UpdateAverageHotelPrice()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_Results.Length; i++)
		{
			m_Results[i] = 0;
		}
		JobHandle val = JobChunkExtensions.Schedule<CalculateAverageHotelPriceJob>(new CalculateAverageHotelPriceJob
		{
			m_LodgingProviderHandle = InternalCompilerInterface.GetComponentTypeHandle<LodgingProvider>(ref __TypeHandle.__Game_Companies_LodgingProvider_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Results = m_Results
		}, m_HotelQuery, ((SystemBase)this).Dependency);
		((JobHandle)(ref val)).Complete();
		int num = m_Results[1];
		float num2 = ((num > 0) ? (m_Results[0] / num) : 0);
		m_AverageHotelPrice.Update(num2);
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
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<AttractivenessParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_1647950437_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
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
	public TourismInfoviewUISystem()
	{
	}
}
