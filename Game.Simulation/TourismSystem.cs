using System.Runtime.CompilerServices;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class TourismSystem : GameSystemBase
{
	[BurstCompile]
	private struct TourismJob : IJob
	{
		[DeallocateOnJobCompletion]
		[ReadOnly]
		public NativeArray<ArchetypeChunk> m_HotelChunks;

		[DeallocateOnJobCompletion]
		[ReadOnly]
		public NativeArray<ArchetypeChunk> m_m_AttractivenessProviderChunks;

		[ReadOnly]
		public ComponentTypeHandle<AttractivenessProvider> m_ProviderType;

		[ReadOnly]
		public ComponentTypeHandle<LodgingProvider> m_LodgingProviderType;

		[ReadOnly]
		public BufferTypeHandle<Renter> m_RenterType;

		[ReadOnly]
		public AttractivenessParameterData m_Parameters;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifiers;

		public ComponentLookup<Tourism> m_Tourisms;

		[ReadOnly]
		public Entity m_City;

		[ReadOnly]
		public bool m_IsRaining;

		[ReadOnly]
		public bool m_IsSnowing;

		[ReadOnly]
		public float m_Temperature;

		[ReadOnly]
		public float m_Precipitation;

		[ReadOnly]
		public int m_TouristCitizenCount;

		[ReadOnly]
		public ClimateSystem.WeatherClassification m_WeatherClassification;

		public void Execute()
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			Tourism tourism = new Tourism
			{
				m_CurrentTourists = m_TouristCitizenCount
			};
			int2 val = default(int2);
			for (int i = 0; i < m_HotelChunks.Length; i++)
			{
				ArchetypeChunk val2 = m_HotelChunks[i];
				NativeArray<LodgingProvider> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray<LodgingProvider>(ref m_LodgingProviderType);
				BufferAccessor<Renter> bufferAccessor = ((ArchetypeChunk)(ref val2)).GetBufferAccessor<Renter>(ref m_RenterType);
				for (int j = 0; j < ((ArchetypeChunk)(ref val2)).Count; j++)
				{
					LodgingProvider lodgingProvider = nativeArray[j];
					DynamicBuffer<Renter> val3 = bufferAccessor[j];
					val += new int2(val3.Length, val3.Length + lodgingProvider.m_FreeRooms);
				}
			}
			tourism.m_Lodging = val;
			float num = 0f;
			for (int k = 0; k < m_m_AttractivenessProviderChunks.Length; k++)
			{
				ArchetypeChunk val4 = m_m_AttractivenessProviderChunks[k];
				NativeArray<AttractivenessProvider> nativeArray2 = ((ArchetypeChunk)(ref val4)).GetNativeArray<AttractivenessProvider>(ref m_ProviderType);
				for (int l = 0; l < nativeArray2.Length; l++)
				{
					AttractivenessProvider attractivenessProvider = nativeArray2[l];
					num += (float)(attractivenessProvider.m_Attractiveness * attractivenessProvider.m_Attractiveness) / 10000f;
				}
			}
			num = 200f / (1f + math.exp(-0.3f * num)) - 100f;
			DynamicBuffer<CityModifier> modifiers = m_CityModifiers[m_City];
			CityUtils.ApplyModifier(ref num, modifiers, CityModifierType.Attractiveness);
			tourism.m_Attractiveness = Mathf.RoundToInt(num);
			tourism.m_AverageTourists = Mathf.RoundToInt(2f * GetTouristProbability(m_Parameters, tourism.m_Attractiveness, m_WeatherClassification, m_Temperature, m_Precipitation, m_IsRaining, m_IsSnowing) * 100000f / 16f);
			m_Tourisms[m_City] = tourism;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<LodgingProvider> __Game_Companies_LodgingProvider_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Renter> __Game_Buildings_Renter_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferLookup<CityModifier> __Game_City_CityModifier_RO_BufferLookup;

		public ComponentLookup<Tourism> __Game_City_Tourism_RW_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<AttractivenessProvider> __Game_Buildings_AttractivenessProvider_RO_ComponentTypeHandle;

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
			__Game_Companies_LodgingProvider_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<LodgingProvider>(true);
			__Game_Buildings_Renter_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Renter>(true);
			__Game_City_CityModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(true);
			__Game_City_Tourism_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Tourism>(false);
			__Game_Buildings_AttractivenessProvider_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AttractivenessProvider>(true);
		}
	}

	private int2 m_CachedLodging;

	private CitySystem m_CitySystem;

	private ClimateSystem m_ClimateSystem;

	private CountHouseholdDataSystem m_CountHouseholdDataSystem;

	private EntityQuery m_AttractivenessProviderGroup;

	private EntityQuery m_HotelGroup;

	private EntityQuery m_ParameterQuery;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 32768;
	}

	public static int GetTouristRandomStay()
	{
		return 262144;
	}

	public static float GetRawTouristProbability(int attractiveness)
	{
		return (float)attractiveness / 1000f;
	}

	public static float GetTouristProbability(AttractivenessParameterData parameterData, int attractiveness, ClimateSystem.WeatherClassification weatherClassification, float temperature, float precipitation, bool isRaining, bool isSnowing)
	{
		return GetRawTouristProbability(attractiveness) * GetWeatherEffect(parameterData, weatherClassification, temperature, precipitation, isRaining, isSnowing);
	}

	public static float GetWeatherEffect(AttractivenessParameterData parameterData, ClimateSystem.WeatherClassification weatherClassification, float temperature, float precipitation, bool isRaining, bool isSnowing)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		float num = 1f;
		if (temperature > parameterData.m_AttractiveTemperature.x && temperature < parameterData.m_AttractiveTemperature.y)
		{
			num += Mathf.Lerp(parameterData.m_TemperatureAffect.x, 0f, math.abs(temperature - (parameterData.m_AttractiveTemperature.x + parameterData.m_AttractiveTemperature.y) / 2f) / ((parameterData.m_AttractiveTemperature.y - parameterData.m_AttractiveTemperature.x) / 2f));
		}
		else if (temperature > parameterData.m_ExtremeTemperature.y)
		{
			num += Mathf.Lerp(0f, parameterData.m_TemperatureAffect.y, (temperature - parameterData.m_ExtremeTemperature.y) / 10f);
		}
		else if (temperature < parameterData.m_ExtremeTemperature.x)
		{
			num += Mathf.Lerp(0f, parameterData.m_TemperatureAffect.y, (parameterData.m_ExtremeTemperature.x - temperature) / 10f);
		}
		if (isSnowing && precipitation > parameterData.m_SnowEffectRange.x && precipitation < parameterData.m_SnowEffectRange.y)
		{
			num += Mathf.Lerp(0f, parameterData.m_SnowRainExtremeAffect.x, (precipitation - parameterData.m_SnowEffectRange.x) / (parameterData.m_SnowEffectRange.y - parameterData.m_SnowEffectRange.x));
		}
		else if (isRaining && precipitation > parameterData.m_RainEffectRange.x && precipitation < parameterData.m_RainEffectRange.y)
		{
			num += Mathf.Lerp(0f, parameterData.m_SnowRainExtremeAffect.y, (precipitation - parameterData.m_RainEffectRange.x) / (parameterData.m_RainEffectRange.y - parameterData.m_RainEffectRange.x));
		}
		if (weatherClassification == ClimateSystem.WeatherClassification.Stormy)
		{
			num += parameterData.m_SnowRainExtremeAffect.z;
		}
		return math.clamp(num, 0.5f, 1.5f);
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_ClimateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ClimateSystem>();
		m_CountHouseholdDataSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CountHouseholdDataSystem>();
		m_AttractivenessProviderGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<AttractivenessProvider>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_HotelGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<LodgingProvider>(),
			ComponentType.ReadOnly<PropertyRenter>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_ParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<AttractivenessParameterData>() });
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		TourismJob tourismJob = new TourismJob
		{
			m_m_AttractivenessProviderChunks = ((EntityQuery)(ref m_AttractivenessProviderGroup)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3)),
			m_HotelChunks = ((EntityQuery)(ref m_HotelGroup)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3)),
			m_LodgingProviderType = InternalCompilerInterface.GetComponentTypeHandle<LodgingProvider>(ref __TypeHandle.__Game_Companies_LodgingProvider_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RenterType = InternalCompilerInterface.GetBufferTypeHandle<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CityModifiers = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Tourisms = InternalCompilerInterface.GetComponentLookup<Tourism>(ref __TypeHandle.__Game_City_Tourism_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Parameters = ((EntityQuery)(ref m_ParameterQuery)).GetSingleton<AttractivenessParameterData>(),
			m_ProviderType = InternalCompilerInterface.GetComponentTypeHandle<AttractivenessProvider>(ref __TypeHandle.__Game_Buildings_AttractivenessProvider_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_City = m_CitySystem.City,
			m_IsRaining = m_ClimateSystem.isRaining,
			m_IsSnowing = m_ClimateSystem.isSnowing,
			m_Temperature = m_ClimateSystem.temperature,
			m_Precipitation = m_ClimateSystem.precipitation,
			m_TouristCitizenCount = m_CountHouseholdDataSystem.TouristCitizenCount,
			m_WeatherClassification = m_ClimateSystem.classification
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<TourismJob>(tourismJob, ((SystemBase)this).Dependency);
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
	public TourismSystem()
	{
	}
}
