using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.City;
using Game.Economy;
using Game.Prefabs;
using Game.Simulation;
using Game.Zones;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class TaxationUISystem : UISystemBase
{
	private struct TypeHandle
	{
		[ReadOnly]
		public BufferLookup<CityStatistic> __Game_City_CityStatistic_RO_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			__Game_City_CityStatistic_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityStatistic>(true);
		}
	}

	private static readonly string kGroup = "taxation";

	private ITaxSystem m_TaxSystem;

	private CityStatisticsSystem m_CityStatisticsSystem;

	private PrefabSystem m_PrefabSystem;

	private EntityQuery m_ResourceQuery;

	private EntityQuery m_UnlockedZoneQuery;

	private GetterValueBinding<int> m_TaxRate;

	private GetterValueBinding<int> m_TaxIncome;

	private GetterValueBinding<int> m_TaxEffect;

	private GetterValueBinding<int> m_MinTaxRate;

	private GetterValueBinding<int> m_MaxTaxRate;

	private RawValueBinding m_AreaTypes;

	private GetterMapBinding<int, int> m_AreaTaxRates;

	private GetterMapBinding<int, Bounds1> m_AreaResourceTaxRanges;

	private GetterMapBinding<int, int> m_AreaTaxIncomes;

	private GetterMapBinding<int, int> m_AreaTaxEffects;

	private GetterMapBinding<TaxResource, int> m_ResourceTaxRates;

	private GetterMapBinding<TaxResource, int> m_ResourceTaxIncomes;

	private TaxParameterData m_CachedTaxParameterData;

	private int m_CachedLockedOrderVersion = -1;

	private Dictionary<int, string> m_ResourceIcons = new Dictionary<int, string>();

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Expected O, but got Unknown
		//IL_018d: Expected O, but got Unknown
		base.OnCreate();
		m_ResourceQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<ResourceData>(),
			ComponentType.ReadOnly<TaxableResourceData>()
		});
		m_UnlockedZoneQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<ZoneData>(),
			ComponentType.Exclude<Locked>()
		});
		m_TaxSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TaxSystem>();
		m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		AddBinding((IBinding)(object)(m_TaxRate = new GetterValueBinding<int>(kGroup, "taxRate", (Func<int>)UpdateTaxRate, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_TaxIncome = new GetterValueBinding<int>(kGroup, "taxIncome", (Func<int>)UpdateTaxIncome, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_TaxEffect = new GetterValueBinding<int>(kGroup, "taxEffect", (Func<int>)UpdateTaxEffect, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_MinTaxRate = new GetterValueBinding<int>(kGroup, "minTaxRate", (Func<int>)UpdateMinTaxRate, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_MaxTaxRate = new GetterValueBinding<int>(kGroup, "maxTaxRate", (Func<int>)UpdateMaxTaxRate, (IWriter<int>)null, (EqualityComparer<int>)null)));
		RawValueBinding val = new RawValueBinding(kGroup, "areaTypes", (Action<IJsonWriter>)UpdateAreaTypes);
		RawValueBinding binding = val;
		m_AreaTypes = val;
		AddBinding((IBinding)(object)binding);
		AddBinding((IBinding)(object)(m_AreaTaxRates = new GetterMapBinding<int, int>(kGroup, "areaTaxRates", (Func<int, int>)UpdateAreaTaxRate, (IReader<int>)null, (IWriter<int>)null, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_AreaResourceTaxRanges = new GetterMapBinding<int, Bounds1>(kGroup, "areaResourceTaxRanges", (Func<int, Bounds1>)UpdateAreaResourceTaxRange, (IReader<int>)null, (IWriter<int>)null, (IWriter<Bounds1>)null, (EqualityComparer<Bounds1>)null)));
		AddBinding((IBinding)(object)(m_AreaTaxIncomes = new GetterMapBinding<int, int>(kGroup, "areaTaxIncomes", (Func<int, int>)UpdateAreaTaxIncome, (IReader<int>)null, (IWriter<int>)null, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_AreaTaxEffects = new GetterMapBinding<int, int>(kGroup, "areaTaxEffects", (Func<int, int>)UpdateAreaTaxEffect, (IReader<int>)null, (IWriter<int>)null, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)new RawMapBinding<int>(kGroup, "areaResources", (Action<IJsonWriter, int>)UpdateAreaResources, (IReader<int>)null, (IWriter<int>)null));
		AddBinding((IBinding)(object)(m_ResourceTaxRates = new GetterMapBinding<TaxResource, int>(kGroup, "resourceTaxRates", (Func<TaxResource, int>)UpdateResourceTaxRate, (IReader<TaxResource>)(object)new ValueReader<TaxResource>(), (IWriter<TaxResource>)(object)new ValueWriter<TaxResource>(), (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_ResourceTaxIncomes = new GetterMapBinding<TaxResource, int>(kGroup, "resourceTaxIncomes", (Func<TaxResource, int>)UpdateResourceTaxIncome, (IReader<TaxResource>)(object)new ValueReader<TaxResource>(), (IWriter<TaxResource>)(object)new ValueWriter<TaxResource>(), (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)new GetterMapBinding<TaxResource, TaxResourceInfo>(kGroup, "taxResourceInfos", (Func<TaxResource, TaxResourceInfo>)UpdateResourceInfo, (IReader<TaxResource>)(object)new ValueReader<TaxResource>(), (IWriter<TaxResource>)(object)new ValueWriter<TaxResource>(), (IWriter<TaxResourceInfo>)(object)new ValueWriter<TaxResourceInfo>(), (EqualityComparer<TaxResourceInfo>)null));
		AddBinding((IBinding)(object)new TriggerBinding<int>(kGroup, "setTaxRate", (Action<int>)SetTaxRate, (IReader<int>)null));
		AddBinding((IBinding)(object)new TriggerBinding<int, int>(kGroup, "setAreaTaxRate", (Action<int, int>)SetAreaTaxRate, (IReader<int>)null, (IReader<int>)null));
		AddBinding((IBinding)(object)new TriggerBinding<int, int, int>(kGroup, "setResourceTaxRate", (Action<int, int, int>)SetResourceTaxRate, (IReader<int>)null, (IReader<int>)null, (IReader<int>)null));
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		((MapBindingBase<int>)(object)m_AreaTaxIncomes).UpdateAll();
		((MapBindingBase<int>)(object)m_AreaTaxEffects).UpdateAll();
		m_TaxIncome.Update();
		m_TaxEffect.Update();
		((MapBindingBase<TaxResource>)(object)m_ResourceTaxIncomes).UpdateAll();
		TaxParameterData taxParameterData = m_TaxSystem.GetTaxParameterData();
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		int componentOrderVersion = ((EntityManager)(ref entityManager)).GetComponentOrderVersion<Locked>();
		bool flag = componentOrderVersion != m_CachedLockedOrderVersion;
		m_CachedLockedOrderVersion = componentOrderVersion;
		if (!m_CachedTaxParameterData.Equals(taxParameterData))
		{
			m_AreaTypes.Update();
			m_MinTaxRate.Update();
			m_MaxTaxRate.Update();
		}
		else if (flag)
		{
			m_AreaTypes.Update();
		}
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		base.OnGameLoaded(serializationContext);
		m_ResourceIcons.Clear();
		NativeArray<Entity> val = ((EntityQuery)(ref m_ResourceQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		for (int i = 0; i < val.Length; i++)
		{
			ResourcePrefab prefab = m_PrefabSystem.GetPrefab<ResourcePrefab>(val[i]);
			UIObject component = prefab.GetComponent<UIObject>();
			m_ResourceIcons[(int)(prefab.m_Resource - 1)] = (Object.op_Implicit((Object)(object)component) ? component.m_Icon : string.Empty);
		}
		val.Dispose();
	}

	private int2 GetLimits(TaxAreaType type, TaxParameterData limits)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		return (int2)(type switch
		{
			TaxAreaType.Residential => limits.m_ResidentialTaxLimits, 
			TaxAreaType.Commercial => limits.m_CommercialTaxLimits, 
			TaxAreaType.Industrial => limits.m_IndustrialTaxLimits, 
			TaxAreaType.Office => limits.m_OfficeTaxLimits, 
			_ => default(int2), 
		});
	}

	private int2 GetResourceLimits(TaxAreaType type, TaxParameterData limits)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		if (type == TaxAreaType.Residential)
		{
			return limits.m_JobLevelTaxLimits;
		}
		return limits.m_ResourceTaxLimits;
	}

	private int GetResourceTaxRate(TaxAreaType type, int resource)
	{
		return type switch
		{
			TaxAreaType.Residential => m_TaxSystem.GetResidentialTaxRate(resource), 
			TaxAreaType.Commercial => m_TaxSystem.GetCommercialTaxRate(EconomyUtils.GetResource(resource)), 
			TaxAreaType.Industrial => m_TaxSystem.GetIndustrialTaxRate(EconomyUtils.GetResource(resource)), 
			TaxAreaType.Office => m_TaxSystem.GetOfficeTaxRate(EconomyUtils.GetResource(resource)), 
			_ => 0, 
		};
	}

	private int GetEstimatedResourceTaxIncome(TaxAreaType type, int resource)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		NativeParallelHashMap<CityStatisticsSystem.StatisticsKey, Entity> lookup = m_CityStatisticsSystem.GetLookup();
		BufferLookup<CityStatistic> bufferLookup = InternalCompilerInterface.GetBufferLookup<CityStatistic>(ref __TypeHandle.__Game_City_CityStatistic_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		return type switch
		{
			TaxAreaType.Residential => m_TaxSystem.GetEstimatedResidentialTaxIncome(resource, lookup, bufferLookup), 
			TaxAreaType.Commercial => m_TaxSystem.GetEstimatedCommercialTaxIncome(EconomyUtils.GetResource(resource), lookup, bufferLookup), 
			TaxAreaType.Industrial => m_TaxSystem.GetEstimatedIndustrialTaxIncome(EconomyUtils.GetResource(resource), lookup, bufferLookup), 
			TaxAreaType.Office => m_TaxSystem.GetEstimatedOfficeTaxIncome(EconomyUtils.GetResource(resource), lookup, bufferLookup), 
			_ => 0, 
		};
	}

	private int UpdateTaxRate()
	{
		return m_TaxSystem.TaxRate;
	}

	private int UpdateTaxIncome()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		NativeParallelHashMap<CityStatisticsSystem.StatisticsKey, Entity> lookup = m_CityStatisticsSystem.GetLookup();
		BufferLookup<CityStatistic> bufferLookup = InternalCompilerInterface.GetBufferLookup<CityStatistic>(ref __TypeHandle.__Game_City_CityStatistic_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		return m_TaxSystem.GetEstimatedTaxAmount(TaxAreaType.Residential, TaxResultType.Any, lookup, bufferLookup) + m_TaxSystem.GetEstimatedTaxAmount(TaxAreaType.Commercial, TaxResultType.Any, lookup, bufferLookup) + m_TaxSystem.GetEstimatedTaxAmount(TaxAreaType.Industrial, TaxResultType.Any, lookup, bufferLookup) + m_TaxSystem.GetEstimatedTaxAmount(TaxAreaType.Office, TaxResultType.Any, lookup, bufferLookup);
	}

	private int UpdateTaxEffect()
	{
		return m_TaxSystem.GetTaxRateEffect(TaxAreaType.Residential, m_TaxSystem.GetTaxRate(TaxAreaType.Residential)) + m_TaxSystem.GetTaxRateEffect(TaxAreaType.Commercial, m_TaxSystem.GetTaxRate(TaxAreaType.Commercial)) + m_TaxSystem.GetTaxRateEffect(TaxAreaType.Industrial, m_TaxSystem.GetTaxRate(TaxAreaType.Industrial)) + m_TaxSystem.GetTaxRateEffect(TaxAreaType.Office, m_TaxSystem.GetTaxRate(TaxAreaType.Office));
	}

	private int UpdateMinTaxRate()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return m_TaxSystem.GetTaxParameterData().m_TotalTaxLimits.x;
	}

	private int UpdateMaxTaxRate()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return m_TaxSystem.GetTaxParameterData().m_TotalTaxLimits.y;
	}

	private void SetTaxRate(int rate)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		JobHandle readers = m_TaxSystem.Readers;
		((JobHandle)(ref readers)).Complete();
		m_TaxSystem.TaxRate = rate;
		m_TaxRate.Update();
		((MapBindingBase<int>)(object)m_AreaTaxRates).UpdateAll();
		((MapBindingBase<TaxResource>)(object)m_ResourceTaxRates).UpdateAll();
		((MapBindingBase<int>)(object)m_AreaResourceTaxRanges).UpdateAll();
	}

	private void SetAreaTaxRate(int areaType, int rate)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		JobHandle readers = m_TaxSystem.Readers;
		((JobHandle)(ref readers)).Complete();
		m_TaxSystem.SetTaxRate((TaxAreaType)areaType, rate);
		((MapBindingBase<int>)(object)m_AreaTaxRates).Update(areaType);
		((MapBindingBase<TaxResource>)(object)m_ResourceTaxRates).UpdateAll();
		((MapBindingBase<int>)(object)m_AreaResourceTaxRanges).UpdateAll();
	}

	private void SetResourceTaxRate(int resource, int areaType, int rate)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		JobHandle readers = m_TaxSystem.Readers;
		((JobHandle)(ref readers)).Complete();
		if ((byte)areaType == 1)
		{
			m_TaxSystem.SetResidentialTaxRate(resource, rate);
		}
		if ((byte)areaType == 2)
		{
			m_TaxSystem.SetCommercialTaxRate(EconomyUtils.GetResource(resource), rate);
		}
		else if ((byte)areaType == 3)
		{
			m_TaxSystem.SetIndustrialTaxRate(EconomyUtils.GetResource(resource), rate);
		}
		else if ((byte)areaType == 4)
		{
			m_TaxSystem.SetOfficeTaxRate(EconomyUtils.GetResource(resource), rate);
		}
		((MapBindingBase<int>)(object)m_AreaTaxRates).Update(areaType);
		((MapBindingBase<TaxResource>)(object)m_ResourceTaxRates).Update(new TaxResource
		{
			m_AreaType = areaType,
			m_Resource = resource
		});
		((MapBindingBase<int>)(object)m_AreaResourceTaxRanges).UpdateAll();
	}

	private int UpdateAreaTaxRate(int areaType)
	{
		return m_TaxSystem.GetTaxRate((TaxAreaType)areaType);
	}

	private Bounds1 UpdateAreaResourceTaxRange(int area)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return new Bounds1(float2.op_Implicit(m_TaxSystem.GetTaxRateRange((TaxAreaType)area)));
	}

	private int UpdateAreaTaxIncome(int areaType)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		NativeParallelHashMap<CityStatisticsSystem.StatisticsKey, Entity> lookup = m_CityStatisticsSystem.GetLookup();
		BufferLookup<CityStatistic> bufferLookup = InternalCompilerInterface.GetBufferLookup<CityStatistic>(ref __TypeHandle.__Game_City_CityStatistic_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		return m_TaxSystem.GetEstimatedTaxAmount((TaxAreaType)areaType, TaxResultType.Any, lookup, bufferLookup);
	}

	private int UpdateAreaTaxEffect(int areaType)
	{
		return m_TaxSystem.GetTaxRateEffect((TaxAreaType)areaType, m_TaxSystem.GetTaxRate((TaxAreaType)areaType));
	}

	private int UpdateResourceTaxRate(TaxResource taxResource)
	{
		return GetResourceTaxRate((TaxAreaType)taxResource.m_AreaType, taxResource.m_Resource);
	}

	private int UpdateResourceTaxIncome(TaxResource taxResource)
	{
		return GetEstimatedResourceTaxIncome((TaxAreaType)taxResource.m_AreaType, taxResource.m_Resource);
	}

	private void UpdateAreaTypes(IJsonWriter binder)
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		TaxParameterData limits = (m_CachedTaxParameterData = m_TaxSystem.GetTaxParameterData());
		binder.ArrayBegin(4u);
		TaxAreaType taxAreaType = TaxAreaType.Residential;
		while ((int)taxAreaType <= 4)
		{
			binder.TypeBegin("taxation.TaxAreaType");
			binder.PropertyName("index");
			binder.Write((int)taxAreaType);
			binder.PropertyName("id");
			binder.Write(taxAreaType.ToString());
			binder.PropertyName("icon");
			binder.Write(GetIcon(taxAreaType));
			int2 limits2 = GetLimits(taxAreaType, limits);
			binder.PropertyName("taxRateMin");
			binder.Write(limits2.x);
			binder.PropertyName("taxRateMax");
			binder.Write(limits2.y);
			int2 resourceLimits = GetResourceLimits(taxAreaType, limits);
			binder.PropertyName("resourceTaxRateMin");
			binder.Write(resourceLimits.x);
			binder.PropertyName("resourceTaxRateMax");
			binder.Write(resourceLimits.y);
			binder.PropertyName("locked");
			binder.Write(Locked(taxAreaType));
			binder.TypeEnd();
			taxAreaType++;
		}
		binder.ArrayEnd();
	}

	private bool Locked(TaxAreaType areaType)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<ZoneData> val = ((EntityQuery)(ref m_UnlockedZoneQuery)).ToComponentDataArray<ZoneData>(AllocatorHandle.op_Implicit((Allocator)3));
		for (int i = 0; i < val.Length; i++)
		{
			if ((areaType == TaxAreaType.Residential && val[i].m_AreaType == AreaType.Residential) || (areaType == TaxAreaType.Commercial && val[i].m_AreaType == AreaType.Commercial) || (areaType == TaxAreaType.Industrial && val[i].m_AreaType == AreaType.Industrial) || (areaType == TaxAreaType.Office && (val[i].m_ZoneFlags & ZoneFlags.Office) != 0))
			{
				val.Dispose();
				return false;
			}
		}
		val.Dispose();
		return true;
	}

	private void UpdateAreaResources(IJsonWriter binder, int area)
	{
		if ((byte)area == 1)
		{
			binder.ArrayBegin(5u);
			for (int i = 0; i < 5; i++)
			{
				JsonWriterExtensions.Write<TaxResource>(binder, new TaxResource
				{
					m_Resource = i,
					m_AreaType = 1
				});
			}
			binder.ArrayEnd();
			return;
		}
		int num = 0;
		foreach (ResourcePrefab resource in GetResources(area))
		{
			_ = resource;
			num++;
		}
		JsonWriterExtensions.ArrayBegin(binder, num);
		foreach (ResourcePrefab resource2 in GetResources(area))
		{
			JsonWriterExtensions.Write<TaxResource>(binder, new TaxResource
			{
				m_Resource = (int)(resource2.m_Resource - 1),
				m_AreaType = area
			});
		}
		binder.ArrayEnd();
	}

	private TaxResourceInfo UpdateResourceInfo(TaxResource resource)
	{
		if (resource.m_AreaType == 1)
		{
			return new TaxResourceInfo
			{
				m_ID = string.Empty,
				m_Icon = "Media/Game/Icons/ZoneResidential.svg"
			};
		}
		return new TaxResourceInfo
		{
			m_ID = EconomyUtils.GetResource(resource.m_Resource).ToString(),
			m_Icon = m_ResourceIcons[resource.m_Resource]
		};
	}

	private IEnumerable<ResourcePrefab> GetResources(int areaType)
	{
		NativeArray<Entity> entities = ((EntityQuery)(ref m_ResourceQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		int i = 0;
		while (i < entities.Length)
		{
			ResourcePrefab prefab = m_PrefabSystem.GetPrefab<ResourcePrefab>(entities[i]);
			TaxableResource component = prefab.GetComponent<TaxableResource>();
			if (MatchArea(component, areaType))
			{
				yield return prefab;
			}
			int num = i + 1;
			i = num;
		}
		entities.Dispose();
	}

	private bool MatchArea(TaxableResource data, int areaType)
	{
		if (data.m_TaxAreas == null || data.m_TaxAreas.Length == 0)
		{
			return true;
		}
		for (int i = 0; i < data.m_TaxAreas.Length; i++)
		{
			if ((int)data.m_TaxAreas[i] == areaType)
			{
				return true;
			}
		}
		return false;
	}

	private string GetIcon(TaxAreaType type)
	{
		return "Media/Game/Icons/Zone" + type.ToString() + ".svg";
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
	public TaxationUISystem()
	{
	}
}
