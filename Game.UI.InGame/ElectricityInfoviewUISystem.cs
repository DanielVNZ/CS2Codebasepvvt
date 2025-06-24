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
public class ElectricityInfoviewUISystem : InfoviewUISystemBase
{
	private const string kGroup = "electricityInfo";

	private ElectricityStatisticsSystem m_ElectricityStatisticsSystem;

	private ElectricityTradeSystem m_ElectricityTradeSystem;

	private EntityQuery m_OutsideTradeParameterGroup;

	private GetterValueBinding<int> m_ElectricityProduction;

	private GetterValueBinding<int> m_ElectricityConsumption;

	private GetterValueBinding<int> m_ElectricityTransmitted;

	private GetterValueBinding<int> m_ElectricityExport;

	private GetterValueBinding<int> m_ElectricityImport;

	private GetterValueBinding<IndicatorValue> m_ElectricityAvailability;

	private GetterValueBinding<IndicatorValue> m_ElectricityTransmission;

	private GetterValueBinding<IndicatorValue> m_ElectricityTrade;

	private GetterValueBinding<IndicatorValue> m_BatteryCharge;

	protected override bool Active
	{
		get
		{
			if (!base.Active && !((EventBindingBase)m_ElectricityProduction).active && !((EventBindingBase)m_ElectricityConsumption).active && !((EventBindingBase)m_ElectricityTransmitted).active && !((EventBindingBase)m_ElectricityExport).active && !((EventBindingBase)m_ElectricityImport).active && !((EventBindingBase)m_ElectricityAvailability).active && !((EventBindingBase)m_ElectricityTransmission).active && !((EventBindingBase)m_ElectricityTrade).active)
			{
				return ((EventBindingBase)m_BatteryCharge).active;
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
		m_ElectricityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ElectricityStatisticsSystem>();
		m_ElectricityTradeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ElectricityTradeSystem>();
		m_OutsideTradeParameterGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<OutsideTradeParameterData>() });
		AddBinding((IBinding)(object)(m_ElectricityProduction = new GetterValueBinding<int>("electricityInfo", "electricityProduction", (Func<int>)(() => m_ElectricityStatisticsSystem.production), (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_ElectricityConsumption = new GetterValueBinding<int>("electricityInfo", "electricityConsumption", (Func<int>)(() => m_ElectricityStatisticsSystem.consumption), (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_ElectricityTransmitted = new GetterValueBinding<int>("electricityInfo", "electricityTransmitted", (Func<int>)(() => m_ElectricityStatisticsSystem.fulfilledConsumption), (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_ElectricityExport = new GetterValueBinding<int>("electricityInfo", "electricityExport", (Func<int>)(() => m_ElectricityTradeSystem.export), (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_ElectricityImport = new GetterValueBinding<int>("electricityInfo", "electricityImport", (Func<int>)(() => m_ElectricityTradeSystem.import), (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_ElectricityAvailability = new GetterValueBinding<IndicatorValue>("electricityInfo", "electricityAvailability", (Func<IndicatorValue>)GetElectricityAvailability, (IWriter<IndicatorValue>)(object)new ValueWriter<IndicatorValue>(), (EqualityComparer<IndicatorValue>)null)));
		AddBinding((IBinding)(object)(m_ElectricityTransmission = new GetterValueBinding<IndicatorValue>("electricityInfo", "electricityTransmission", (Func<IndicatorValue>)GetElectricityTransmission, (IWriter<IndicatorValue>)(object)new ValueWriter<IndicatorValue>(), (EqualityComparer<IndicatorValue>)null)));
		AddBinding((IBinding)(object)(m_ElectricityTrade = new GetterValueBinding<IndicatorValue>("electricityInfo", "electricityTrade", (Func<IndicatorValue>)GetElectricityTrade, (IWriter<IndicatorValue>)(object)new ValueWriter<IndicatorValue>(), (EqualityComparer<IndicatorValue>)null)));
		AddBinding((IBinding)(object)(m_BatteryCharge = new GetterValueBinding<IndicatorValue>("electricityInfo", "batteryCharge", (Func<IndicatorValue>)GetBatteryCharge, (IWriter<IndicatorValue>)(object)new ValueWriter<IndicatorValue>(), (EqualityComparer<IndicatorValue>)null)));
	}

	protected override void PerformUpdate()
	{
		m_ElectricityProduction.Update();
		m_ElectricityConsumption.Update();
		m_ElectricityTransmitted.Update();
		m_ElectricityExport.Update();
		m_ElectricityImport.Update();
		m_ElectricityAvailability.Update();
		m_ElectricityTransmission.Update();
		m_ElectricityTrade.Update();
		m_BatteryCharge.Update();
	}

	private IndicatorValue GetElectricityTransmission()
	{
		float max = m_ElectricityStatisticsSystem.consumption;
		float current = m_ElectricityStatisticsSystem.fulfilledConsumption;
		return new IndicatorValue(0f, max, current);
	}

	private IndicatorValue GetElectricityAvailability()
	{
		return IndicatorValue.Calculate(m_ElectricityStatisticsSystem.production, m_ElectricityStatisticsSystem.consumption);
	}

	private IndicatorValue GetElectricityTrade()
	{
		if (!((EntityQuery)(ref m_OutsideTradeParameterGroup)).IsEmptyIgnoreFilter)
		{
			OutsideTradeParameterData singleton = ((EntityQuery)(ref m_OutsideTradeParameterGroup)).GetSingleton<OutsideTradeParameterData>();
			float num = (float)m_ElectricityTradeSystem.export * singleton.m_ElectricityExportPrice - (float)m_ElectricityTradeSystem.import * singleton.m_ElectricityImportPrice;
			float num2 = math.max(0.01f, (float)m_ElectricityStatisticsSystem.consumption * singleton.m_ElectricityExportPrice);
			return new IndicatorValue(-1f, 1f, math.clamp(num / num2, -1f, 1f));
		}
		return new IndicatorValue(-1f, 1f, 0f);
	}

	private IndicatorValue GetBatteryCharge()
	{
		return new IndicatorValue(0f, m_ElectricityStatisticsSystem.batteryCapacity, m_ElectricityStatisticsSystem.batteryCharge);
	}

	[Preserve]
	public ElectricityInfoviewUISystem()
	{
	}
}
