using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.UI.Binding;
using Game.Prefabs;
using Game.Simulation;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class WaterInfoviewUISystem : InfoviewUISystemBase
{
	private const string kGroup = "waterInfo";

	private WaterStatisticsSystem m_WaterStatisticsSystem;

	private GetterValueBinding<int> m_WaterCapacity;

	private WaterTradeSystem m_WaterTradeSystem;

	private EntityQuery m_OutsideTradeParameterGroup;

	private GetterValueBinding<int> m_WaterConsumption;

	private GetterValueBinding<int> m_SewageCapacity;

	private GetterValueBinding<int> m_SewageConsumption;

	private GetterValueBinding<int> m_WaterExport;

	private GetterValueBinding<IndicatorValue> m_WaterAvailability;

	private GetterValueBinding<int> m_WaterImport;

	private GetterValueBinding<IndicatorValue> m_SewageAvailability;

	private GetterValueBinding<int> m_SewageExport;

	private GetterValueBinding<IndicatorValue> m_WaterTrade;

	protected override bool Active
	{
		get
		{
			if (!base.Active && !((EventBindingBase)m_WaterCapacity).active && !((EventBindingBase)m_WaterConsumption).active && !((EventBindingBase)m_SewageCapacity).active && !((EventBindingBase)m_SewageConsumption).active && !((EventBindingBase)m_WaterExport).active && !((EventBindingBase)m_WaterImport).active && !((EventBindingBase)m_SewageExport).active && !((EventBindingBase)m_WaterAvailability).active && !((EventBindingBase)m_SewageAvailability).active)
			{
				return ((EventBindingBase)m_WaterTrade).active;
			}
			return true;
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_WaterStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterStatisticsSystem>();
		m_WaterTradeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterTradeSystem>();
		m_OutsideTradeParameterGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<OutsideTradeParameterData>() });
		AddBinding((IBinding)(object)(m_WaterCapacity = new GetterValueBinding<int>("waterInfo", "waterCapacity", (Func<int>)(() => m_WaterStatisticsSystem.freshCapacity), (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_WaterConsumption = new GetterValueBinding<int>("waterInfo", "waterConsumption", (Func<int>)(() => m_WaterStatisticsSystem.freshConsumption), (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_SewageCapacity = new GetterValueBinding<int>("waterInfo", "sewageCapacity", (Func<int>)(() => m_WaterStatisticsSystem.sewageCapacity), (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_SewageConsumption = new GetterValueBinding<int>("waterInfo", "sewageConsumption", (Func<int>)(() => m_WaterStatisticsSystem.sewageConsumption), (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_WaterExport = new GetterValueBinding<int>("waterInfo", "waterExport", (Func<int>)(() => m_WaterTradeSystem.freshExport), (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_WaterImport = new GetterValueBinding<int>("waterInfo", "waterImport", (Func<int>)(() => m_WaterTradeSystem.freshImport), (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_SewageExport = new GetterValueBinding<int>("waterInfo", "sewageExport", (Func<int>)(() => m_WaterTradeSystem.sewageExport), (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_WaterAvailability = new GetterValueBinding<IndicatorValue>("waterInfo", "waterAvailability", (Func<IndicatorValue>)GetWaterAvailability, (IWriter<IndicatorValue>)(object)new ValueWriter<IndicatorValue>(), (EqualityComparer<IndicatorValue>)null)));
		AddBinding((IBinding)(object)(m_SewageAvailability = new GetterValueBinding<IndicatorValue>("waterInfo", "sewageAvailability", (Func<IndicatorValue>)GetSewageAvailability, (IWriter<IndicatorValue>)(object)new ValueWriter<IndicatorValue>(), (EqualityComparer<IndicatorValue>)null)));
		AddBinding((IBinding)(object)(m_WaterTrade = new GetterValueBinding<IndicatorValue>("waterInfo", "waterTrade", (Func<IndicatorValue>)GetWaterTrade, (IWriter<IndicatorValue>)(object)new ValueWriter<IndicatorValue>(), (EqualityComparer<IndicatorValue>)null)));
	}

	protected override void PerformUpdate()
	{
		m_WaterCapacity.Update();
		m_WaterConsumption.Update();
		m_SewageCapacity.Update();
		m_SewageConsumption.Update();
		m_WaterExport.Update();
		m_WaterImport.Update();
		m_SewageExport.Update();
		m_WaterAvailability.Update();
		m_SewageAvailability.Update();
		m_WaterTrade.Update();
	}

	private IndicatorValue GetWaterTrade()
	{
		if (!((EntityQuery)(ref m_OutsideTradeParameterGroup)).IsEmptyIgnoreFilter)
		{
			OutsideTradeParameterData singleton = ((EntityQuery)(ref m_OutsideTradeParameterGroup)).GetSingleton<OutsideTradeParameterData>();
			float num = (float)m_WaterTradeSystem.freshExport * singleton.m_WaterExportPrice - (float)m_WaterTradeSystem.freshImport * singleton.m_WaterImportPrice;
			float num2 = math.max(0.01f, (float)m_WaterStatisticsSystem.freshConsumption * singleton.m_WaterExportPrice);
			return new IndicatorValue(-1f, 1f, math.clamp(num / num2, -1f, 1f));
		}
		return new IndicatorValue(-1f, 1f, 0f);
	}

	private IndicatorValue GetWaterAvailability()
	{
		return IndicatorValue.Calculate(m_WaterStatisticsSystem.freshCapacity, m_WaterStatisticsSystem.freshConsumption);
	}

	private IndicatorValue GetSewageAvailability()
	{
		return IndicatorValue.Calculate(m_WaterStatisticsSystem.sewageCapacity, m_WaterStatisticsSystem.sewageConsumption);
	}

	[Preserve]
	public WaterInfoviewUISystem()
	{
	}
}
