using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.UI.Binding;
using Game.Agents;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
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
public class PollutionInfoviewUISystem : InfoviewUISystemBase
{
	private enum Result
	{
		GroundPollution,
		GroundPollutionCount,
		AirPollution,
		AirPollutionCount,
		NoisePollution,
		NoisePollutionCount,
		ConsumedWater,
		PollutedWater,
		ResultCount
	}

	[BurstCompile]
	private struct CalculateAveragePollutionJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<PropertyRenter> m_PropertyRenterType;

		[ReadOnly]
		public ComponentLookup<WaterConsumer> m_WaterConsumerFromEntity;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformFromEntity;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifiers;

		[ReadOnly]
		public NativeArray<AirPollution> m_AirPollutionMap;

		[ReadOnly]
		public NativeArray<NoisePollution> m_NoisePollutionMap;

		[ReadOnly]
		public NativeArray<GroundPollution> m_GroundPollutionMap;

		[ReadOnly]
		public Entity m_City;

		public CitizenHappinessParameterData m_HappinessParameters;

		public NativeArray<int> m_Results;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<PropertyRenter> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PropertyRenter>(ref m_PropertyRenterType);
			DynamicBuffer<CityModifier> cityModifiers = m_CityModifiers[m_City];
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			float num8 = 0f;
			WaterConsumer waterConsumer = default(WaterConsumer);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity property = nativeArray[i].m_Property;
				int2 airPollutionBonuses = CitizenHappinessSystem.GetAirPollutionBonuses(property, ref m_TransformFromEntity, m_AirPollutionMap, cityModifiers, in m_HappinessParameters);
				num3 += airPollutionBonuses.x + airPollutionBonuses.y;
				num6++;
				int2 groundPollutionBonuses = CitizenHappinessSystem.GetGroundPollutionBonuses(property, ref m_TransformFromEntity, m_GroundPollutionMap, cityModifiers, in m_HappinessParameters);
				num += groundPollutionBonuses.x + groundPollutionBonuses.y;
				num4++;
				int2 noiseBonuses = CitizenHappinessSystem.GetNoiseBonuses(property, ref m_TransformFromEntity, m_NoisePollutionMap, in m_HappinessParameters);
				num2 += noiseBonuses.x + noiseBonuses.y;
				num5++;
				if (m_WaterConsumerFromEntity.TryGetComponent(property, ref waterConsumer))
				{
					num7 += waterConsumer.m_FulfilledFresh;
					num8 += waterConsumer.m_Pollution * (float)waterConsumer.m_FulfilledFresh;
				}
			}
			ref NativeArray<int> reference = ref m_Results;
			reference[0] = reference[0] + num;
			reference = ref m_Results;
			reference[1] = reference[1] + num4;
			reference = ref m_Results;
			reference[2] = reference[2] + num3;
			reference = ref m_Results;
			reference[3] = reference[3] + num6;
			reference = ref m_Results;
			reference[4] = reference[4] + num2;
			reference = ref m_Results;
			reference[5] = reference[5] + num5;
			reference = ref m_Results;
			reference[6] = reference[6] + num7;
			reference = ref m_Results;
			reference[7] = reference[7] + Mathf.RoundToInt(num8);
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<WaterConsumer> __Game_Buildings_WaterConsumer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

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
			__Game_Buildings_PropertyRenter_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PropertyRenter>(true);
			__Game_Buildings_WaterConsumer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterConsumer>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_City_CityModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(true);
		}
	}

	private const string kGroup = "pollutionInfo";

	private AirPollutionSystem m_AirPollutionSystem;

	private GroundPollutionSystem m_GroundPollutionSystem;

	private NoisePollutionSystem m_NoisePollutionSystem;

	protected CitySystem m_CitySystem;

	private ValueBinding<IndicatorValue> m_AverageGroundPollution;

	private ValueBinding<IndicatorValue> m_AverageWaterPollution;

	private ValueBinding<IndicatorValue> m_AverageAirPollution;

	private ValueBinding<IndicatorValue> m_AverageNoisePollution;

	private EntityQuery m_HouseholdQuery;

	private NativeArray<int> m_Results;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_374463591_0;

	public override GameMode gameMode => GameMode.GameOrEditor;

	protected override bool Active
	{
		get
		{
			if (!base.Active && !((EventBindingBase)m_AverageAirPollution).active && !((EventBindingBase)m_AverageGroundPollution).active && !((EventBindingBase)m_AverageNoisePollution).active)
			{
				return ((EventBindingBase)m_AverageWaterPollution).active;
			}
			return true;
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_AirPollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AirPollutionSystem>();
		m_GroundPollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GroundPollutionSystem>();
		m_NoisePollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NoisePollutionSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_Results = new NativeArray<int>(8, (Allocator)4, (NativeArrayOptions)1);
		m_HouseholdQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[8]
		{
			ComponentType.ReadOnly<PropertyRenter>(),
			ComponentType.ReadOnly<Household>(),
			ComponentType.ReadOnly<HouseholdCitizen>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.Exclude<PropertySeeker>(),
			ComponentType.Exclude<TouristHousehold>(),
			ComponentType.Exclude<CommuterHousehold>(),
			ComponentType.Exclude<MovingAway>()
		});
		AddBinding((IBinding)(object)(m_AverageGroundPollution = new ValueBinding<IndicatorValue>("pollutionInfo", "averageGroundPollution", default(IndicatorValue), (IWriter<IndicatorValue>)(object)new ValueWriter<IndicatorValue>(), (EqualityComparer<IndicatorValue>)null)));
		AddBinding((IBinding)(object)(m_AverageWaterPollution = new ValueBinding<IndicatorValue>("pollutionInfo", "averageWaterPollution", default(IndicatorValue), (IWriter<IndicatorValue>)(object)new ValueWriter<IndicatorValue>(), (EqualityComparer<IndicatorValue>)null)));
		AddBinding((IBinding)(object)(m_AverageAirPollution = new ValueBinding<IndicatorValue>("pollutionInfo", "averageAirPollution", default(IndicatorValue), (IWriter<IndicatorValue>)(object)new ValueWriter<IndicatorValue>(), (EqualityComparer<IndicatorValue>)null)));
		AddBinding((IBinding)(object)(m_AverageNoisePollution = new ValueBinding<IndicatorValue>("pollutionInfo", "averageNoisePollution", default(IndicatorValue), (IWriter<IndicatorValue>)(object)new ValueWriter<IndicatorValue>(), (EqualityComparer<IndicatorValue>)null)));
		((ComponentSystemBase)this).RequireForUpdate<CitizenHappinessParameterData>();
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_Results.Dispose();
		base.OnDestroy();
	}

	protected override void PerformUpdate()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		NativeArray<GroundPollution> map = m_GroundPollutionSystem.GetMap(readOnly: true, out dependencies);
		JobHandle dependencies2;
		NativeArray<AirPollution> map2 = m_AirPollutionSystem.GetMap(readOnly: true, out dependencies2);
		JobHandle dependencies3;
		NativeArray<NoisePollution> map3 = m_NoisePollutionSystem.GetMap(readOnly: true, out dependencies3);
		JobHandle val = JobHandle.CombineDependencies(dependencies, dependencies2, dependencies3);
		CitizenHappinessParameterData singleton = ((EntityQuery)(ref __query_374463591_0)).GetSingleton<CitizenHappinessParameterData>();
		ResetResults();
		JobHandle val2 = JobChunkExtensions.Schedule<CalculateAveragePollutionJob>(new CalculateAveragePollutionJob
		{
			m_PropertyRenterType = InternalCompilerInterface.GetComponentTypeHandle<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WaterConsumerFromEntity = InternalCompilerInterface.GetComponentLookup<WaterConsumer>(ref __TypeHandle.__Game_Buildings_WaterConsumer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformFromEntity = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CityModifiers = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AirPollutionMap = map2,
			m_NoisePollutionMap = map3,
			m_GroundPollutionMap = map,
			m_HappinessParameters = singleton,
			m_City = m_CitySystem.City,
			m_Results = m_Results
		}, m_HouseholdQuery, JobHandle.CombineDependencies(val, ((SystemBase)this).Dependency));
		((JobHandle)(ref val2)).Complete();
		int num = m_Results[0];
		int num2 = m_Results[4];
		int num3 = m_Results[2];
		int num4 = m_Results[1];
		int num5 = m_Results[5];
		int num6 = m_Results[3];
		int num7 = m_Results[6];
		int num8 = m_Results[7];
		int num9 = ((num4 > 0) ? (num / num4) : 0);
		int num10 = ((num6 > 0) ? (num3 / num6) : 0);
		int num11 = ((num5 > 0) ? (num2 / num5) : 0);
		m_AverageGroundPollution.Update(new IndicatorValue(0f, singleton.m_MaxAirAndGroundPollutionBonus, -num9));
		m_AverageAirPollution.Update(new IndicatorValue(0f, singleton.m_MaxAirAndGroundPollutionBonus, -num10));
		m_AverageNoisePollution.Update(new IndicatorValue(0f, singleton.m_MaxNoisePollutionBonus, -num11));
		m_AverageWaterPollution.Update(new IndicatorValue(0f, num7, num8));
	}

	private void ResetResults()
	{
		for (int i = 0; i < m_Results.Length; i++)
		{
			m_Results[i] = 0;
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
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<CitizenHappinessParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_374463591_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
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
	public PollutionInfoviewUISystem()
	{
	}
}
