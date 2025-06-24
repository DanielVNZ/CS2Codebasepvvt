using System;
using System.Collections.Generic;
using Colossal.Mathematics;
using Colossal.UI.Binding;
using Game.Simulation;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

public class ClimateUISystem : UISystemBase
{
	public const string kGroup = "climate";

	private ClimateSystem m_ClimateSystem;

	private EntityQuery m_ClimateQuery;

	private EntityQuery m_ClimateSeasonQuery;

	private EntityQuery m_SeasonChangedQuery;

	private GetterValueBinding<float> m_TemperatureBinding;

	private GetterValueBinding<WeatherType> m_WeatherBinding;

	private GetterValueBinding<string> m_SeasonBinding;

	private Entity m_CurrentSeason;

	private float m_TemperatureBindingValue => MathUtils.Snap((float)m_ClimateSystem.temperature, 0.1f);

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		base.OnCreate();
		m_ClimateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ClimateSystem>();
		AddBinding((IBinding)(object)(m_TemperatureBinding = new GetterValueBinding<float>("climate", "temperature", (Func<float>)(() => m_TemperatureBindingValue), (IWriter<float>)null, (EqualityComparer<float>)null)));
		AddBinding((IBinding)(object)(m_WeatherBinding = new GetterValueBinding<WeatherType>("climate", "weather", (Func<WeatherType>)GetWeather, (IWriter<WeatherType>)(object)new DelegateWriter<WeatherType>((WriterDelegate<WeatherType>)WriteWeatherType), (EqualityComparer<WeatherType>)null)));
		AddBinding((IBinding)(object)(m_SeasonBinding = new GetterValueBinding<string>("climate", "seasonNameId", (Func<string>)GetCurrentSeasonNameID, (IWriter<string>)(object)ValueWriters.Nullable<string>((IWriter<string>)new StringWriter()), (EqualityComparer<string>)null)));
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		m_TemperatureBinding.Update();
		m_WeatherBinding.Update();
		if (!m_SeasonBinding.Update() && m_CurrentSeason != m_ClimateSystem.currentSeason)
		{
			m_SeasonBinding.TriggerUpdate();
		}
		m_CurrentSeason = m_ClimateSystem.currentSeason;
	}

	public WeatherType GetWeather()
	{
		if (m_ClimateSystem.isPrecipitating)
		{
			if (m_ClimateSystem.isRaining)
			{
				return WeatherType.Rain;
			}
			if (m_ClimateSystem.isSnowing)
			{
				return WeatherType.Snow;
			}
			return WeatherType.Clear;
		}
		return FromWeatherClassification(m_ClimateSystem.classification);
	}

	private static WeatherType FromWeatherClassification(ClimateSystem.WeatherClassification classification)
	{
		return classification switch
		{
			ClimateSystem.WeatherClassification.Clear => WeatherType.Clear, 
			ClimateSystem.WeatherClassification.Few => WeatherType.Few, 
			ClimateSystem.WeatherClassification.Scattered => WeatherType.Scattered, 
			ClimateSystem.WeatherClassification.Broken => WeatherType.Broken, 
			ClimateSystem.WeatherClassification.Overcast => WeatherType.Overcast, 
			ClimateSystem.WeatherClassification.Stormy => WeatherType.Storm, 
			_ => WeatherType.Clear, 
		};
	}

	private string GetCurrentSeasonNameID()
	{
		return m_ClimateSystem.currentSeasonNameID;
	}

	private static void WriteWeatherType(IJsonWriter writer, WeatherType type)
	{
		writer.Write((int)type);
	}

	[Preserve]
	public ClimateUISystem()
	{
	}
}
