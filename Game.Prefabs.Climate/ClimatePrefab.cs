using System;
using System.Collections.Generic;
using System.Linq;
using Colossal.Mathematics;
using Colossal.UI.Binding;
using Game.Simulation;
using Game.UI.Widgets;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Prefabs.Climate;

[ComponentMenu("Weather/", new Type[] { })]
public class ClimatePrefab : PrefabBase, IJsonWritable
{
	public struct SeasonTempCurves
	{
		public AnimationCurve nightMin;

		public AnimationCurve nightMax;

		public AnimationCurve dayMin;

		public AnimationCurve dayMax;
	}

	public struct SeasonPrecipCurves
	{
		public AnimationCurve cloudChance;

		public AnimationCurve cloudAmountMin;

		public AnimationCurve cloudAmountMax;

		public AnimationCurve precipChance;

		public AnimationCurve precipAmountMin;

		public AnimationCurve precipAmountMax;

		public AnimationCurve turbulence;
	}

	public struct SeasonAuroraCurves
	{
		public AnimationCurve amount;

		public AnimationCurve chance;
	}

	[Range(-90f, 90f)]
	[EditorName("Editor.CLIMATE_LATITUDE")]
	public float m_Latitude = 61.49772f;

	[Range(-180f, 180f)]
	[EditorName("Editor.CLIMATE_LONGITUDE")]
	public float m_Longitude = 23.767042f;

	[EditorName("Editor.CLIMATE_FREEZING_TEMPERATURE")]
	public float m_FreezingTemperature;

	public AnimationCurve m_Temperature;

	public AnimationCurve m_Precipitation;

	public AnimationCurve m_Cloudiness;

	public AnimationCurve m_Aurora;

	public AnimationCurve m_Fog;

	[EditorName("Editor.CLIMATE_DEFAULT_WEATHER")]
	public WeatherPrefab m_DefaultWeather;

	[EditorName("Editor.CLIMATE_DEFAULT_WEATHERS")]
	public WeatherPrefab[] m_DefaultWeathers;

	[EditorName("Editor.CLIMATE_SEASONS")]
	public ClimateSystem.SeasonInfo[] m_Seasons;

	[HideInInspector]
	public int m_RandomSeed = 1;

	public const int kYearDuration = 12;

	private int[] m_SeasonsOrder;

	private const float k90PercentileToStdDev = 0.78003126f;

	public Bounds1 temperatureRange
	{
		get
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			Bounds1 val = default(Bounds1);
			((Bounds1)(ref val))._002Ector(float.MaxValue, float.MinValue);
			for (int i = 0; i < 288; i++)
			{
				val |= m_Temperature.Evaluate((float)i / 288f * 12f);
			}
			return val;
		}
	}

	public float averageCloudiness
	{
		get
		{
			float num = 0f;
			for (int i = 0; i < 288; i++)
			{
				float num2 = (float)i / 288f * 12f;
				num += m_Cloudiness.Evaluate(num2);
			}
			return num / 288f;
		}
	}

	public float averagePrecipitation
	{
		get
		{
			float num = 0f;
			for (int i = 0; i < 288; i++)
			{
				float num2 = (float)i / 288f * 12f;
				num += m_Precipitation.Evaluate(num2);
			}
			return num / 288f;
		}
	}

	public void Write(IJsonWriter writer)
	{
		writer.TypeBegin(((object)this).GetType().Name);
		writer.PropertyName("latitude");
		writer.Write(m_Latitude);
		writer.PropertyName("longitude");
		writer.Write(m_Longitude);
		writer.PropertyName("freezingTemperature");
		writer.Write(m_FreezingTemperature);
		writer.PropertyName("seasons");
		JsonWriterExtensions.Write<ClimateSystem.SeasonInfo>(writer, (IList<ClimateSystem.SeasonInfo>)m_Seasons);
		writer.TypeEnd();
	}

	public void RebuildCurves()
	{
		EnsureSeasonsOrder(force: true);
		uint num = (uint)m_RandomSeed;
		if (num == 0)
		{
			num = (uint)(Time.realtimeSinceStartup * 10f);
		}
		RebuildTemperatureCurves(num);
		RebuildPrecipitationCurves(num);
		RebuildAuroraCurves(num);
		RebuildFogCurves(num);
	}

	internal void EnsureSeasonsOrder(bool force = false)
	{
		if (force || m_SeasonsOrder == null || m_SeasonsOrder.Length != m_Seasons.Length)
		{
			m_SeasonsOrder = (from v in Enumerable.Range(0, m_Seasons.Length)
				orderby m_Seasons[v].m_StartTime
				select v).ToArray();
		}
	}

	private static AnimationCurve GenCurveFromMinMax(int keyCount, AnimationCurve cmin, AnimationCurve cmax, uint seed, float minValue, float maxValue)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Expected O, but got Unknown
		//IL_008e: Expected O, but got Unknown
		Random rng = default(Random);
		((Random)(ref rng))._002Ector(seed);
		Keyframe[] array = (Keyframe[])(object)new Keyframe[keyCount];
		for (int i = 0; i < array.Length; i++)
		{
			float num = (float)i / (float)array.Length * 12f;
			float num2 = cmin.Evaluate(num);
			float num3 = cmax.Evaluate(num);
			float dev = (num3 - num2) / 2f * 0.78003126f;
			((Keyframe)(ref array[i])).time = num;
			((Keyframe)(ref array[i])).value = GaussianRandom((num2 + num3) / 2f, dev, ref rng);
		}
		AnimationCurve val = new AnimationCurve(array);
		LoopCurve(val, minValue, maxValue);
		return val;
	}

	private void RebuildTemperatureCurves(uint seed)
	{
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Expected O, but got Unknown
		SeasonTempCurves seasonTempCurves = CreateSeasonTemperatureCurves();
		AnimationCurve val = GenCurveFromMinMax(12, seasonTempCurves.nightMin, seasonTempCurves.nightMax, seed + 10000, -100f, 100f);
		AnimationCurve val2 = GenCurveFromMinMax(12, seasonTempCurves.dayMin, seasonTempCurves.dayMax, seed + 11000, -100f, 100f);
		Keyframe[] array = (Keyframe[])(object)new Keyframe[288];
		for (int i = 0; i < 288; i++)
		{
			float num = (float)i / 288f * 12f;
			((Keyframe)(ref array[i])).time = num;
			float num2 = val.Evaluate(num);
			float num3 = val2.Evaluate(num);
			float num4 = (float)(i % 24) / 24f;
			float num5 = noise.cnoise(new float2(num * 4f, 0f)) / 24f * 4f;
			float num6 = (0f - math.cos((num4 + num5) * (float)Math.PI * 2f)) * 0.5f + 0.5f;
			((Keyframe)(ref array[i])).value = math.lerp(num2, num3, num6);
		}
		m_Temperature = new AnimationCurve(array);
		LoopCurve(m_Temperature, -100f, 100f);
	}

	private void RebuildPrecipitationCurves(uint seed)
	{
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Expected O, but got Unknown
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Expected O, but got Unknown
		SeasonPrecipCurves seasonPrecipCurves = CreateSeasonPrecipCurves();
		AnimationCurve val = GenCurveFromMinMax(12, seasonPrecipCurves.cloudAmountMin, seasonPrecipCurves.cloudAmountMax, seed + 1000, 0f, 1f);
		Random val2 = default(Random);
		((Random)(ref val2))._002Ector(seed + 2000);
		Keyframe[] array = (Keyframe[])(object)new Keyframe[1728];
		float num = ((Random)(ref val2)).NextFloat(0f, 100f);
		float y = ((Random)(ref val2)).NextFloat(0f, 100f);
		for (int i = 0; i < array.Length; i++)
		{
			float num2 = (float)i / (float)array.Length * 12f;
			((Keyframe)(ref array[i])).time = num2;
			float num3 = math.saturate(val.Evaluate(num2));
			float num4 = math.saturate(seasonPrecipCurves.turbulence.Evaluate(num2));
			float num5 = SmoothNoise(num2 * 4f, y);
			num5 *= num4;
			num5 *= num3;
			num3 = math.saturate(num3 + num5);
			float num6 = math.saturate(seasonPrecipCurves.cloudChance.Evaluate(num2));
			float num7 = math.saturate((SmoothNoise(num2, num) + SmoothNoise(num2 * 2f, num + 7f) * 0.5f) * 0.5f + 0.5f);
			if (num7 > num6)
			{
				num3 *= 1f - math.saturate((num7 - num6) * 2f);
			}
			((Keyframe)(ref array[i])).value = num3;
		}
		m_Cloudiness = new AnimationCurve(array);
		LoopCurve(m_Cloudiness);
		AnimationCurve val3 = GenCurveFromMinMax(12, seasonPrecipCurves.precipAmountMin, seasonPrecipCurves.precipAmountMax, seed + 3000, 0f, 1f);
		((Random)(ref val2))._002Ector(seed + 4000);
		array = (Keyframe[])(object)new Keyframe[1728];
		num = ((Random)(ref val2)).NextFloat(0f, 100f);
		y = ((Random)(ref val2)).NextFloat(0f, 100f);
		for (int j = 0; j < array.Length; j++)
		{
			float num8 = (float)j / (float)array.Length * 12f;
			((Keyframe)(ref array[j])).time = num8;
			float num9 = math.saturate(val3.Evaluate(num8));
			float num10 = math.saturate(seasonPrecipCurves.turbulence.Evaluate(num8));
			float num11 = SmoothNoise(num8 * 4f, y);
			num11 *= num10;
			num11 *= num9;
			num9 = math.saturate(num9 + num11);
			float num12 = math.saturate(seasonPrecipCurves.precipChance.Evaluate(num8));
			float num13 = math.saturate((SmoothNoise(num8, num) + SmoothNoise(num8 * 2f, num + 7f) * 0.5f) * 0.5f + 0.5f);
			if (num13 > num12)
			{
				num9 *= 1f - math.saturate((num13 - num12) * 2f);
			}
			float num14 = m_Cloudiness.Evaluate(num8);
			if (num14 < 0.7f)
			{
				num9 *= num14 / 0.7f;
			}
			if (num14 < 0.4f)
			{
				num9 *= num14 / 0.4f;
			}
			if (num14 < 0.2f)
			{
				num9 = 0f;
			}
			((Keyframe)(ref array[j])).value = num9;
		}
		m_Precipitation = new AnimationCurve(array);
		LoopCurve(m_Precipitation);
	}

	private static float SmoothNoise(float x, float y = 0f)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return noise.snoise(new float2(x, y));
	}

	private void RebuildAuroraCurves(uint seed)
	{
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Expected O, but got Unknown
		SeasonAuroraCurves seasonAuroraCurves = CreateSeasonAuroraCurves();
		Random val = default(Random);
		((Random)(ref val))._002Ector(seed + 5000);
		Keyframe[] array = (Keyframe[])(object)new Keyframe[288];
		float num = ((Random)(ref val)).NextFloat(0f, 100f);
		float y = ((Random)(ref val)).NextFloat(0f, 100f);
		for (int i = 0; i < array.Length; i++)
		{
			float num2 = (float)i / (float)array.Length * 12f;
			((Keyframe)(ref array[i])).time = num2;
			float num3 = math.max(0f, seasonAuroraCurves.amount.Evaluate(num2));
			float num4 = 0.1f;
			float num5 = SmoothNoise(num2 * 4f, y);
			num5 *= num4;
			num5 *= num3;
			num3 = math.max(0f, num3 + num5);
			float num6 = math.saturate(seasonAuroraCurves.chance.Evaluate(num2));
			float num7 = math.saturate((SmoothNoise(num2, num) + SmoothNoise(num2 * 2f, num + 7f) * 0.5f) * 0.5f + 0.5f);
			if (num7 > num6)
			{
				num3 *= 1f - math.saturate((num7 - num6) * 8f);
			}
			((Keyframe)(ref array[i])).value = num3;
		}
		m_Aurora = new AnimationCurve(array);
		LoopCurve(m_Aurora, 0f, 10f);
	}

	private void RebuildFogCurves(uint seed)
	{
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Expected O, but got Unknown
		Keyframe[] array = (Keyframe[])(object)new Keyframe[288];
		float num = 1f / 24f;
		float num2 = 2f;
		float num3 = 0.15f;
		float num4 = -1f;
		float num5 = 25f;
		float num6 = 0.5f;
		for (int i = 0; i < array.Length; i++)
		{
			float num7 = (float)i / (float)array.Length * 12f;
			((Keyframe)(ref array[i])).time = num7;
			float num8 = m_Cloudiness.Evaluate(num7);
			float num9 = m_Precipitation.Evaluate(num7);
			float num10 = m_Temperature.Evaluate(num7);
			float num11 = 0f;
			if (num8 > num3 && num10 > num4 && num10 < num5 && num9 < num6)
			{
				float num12 = m_Temperature.Evaluate(num7 - 8f * num);
				float num13 = m_Temperature.Evaluate(num7 - 7f * num);
				float num14 = m_Temperature.Evaluate(num7 - 6f * num);
				float num15 = m_Temperature.Evaluate(num7 - 5f * num);
				float num16 = m_Temperature.Evaluate(num7 - 4f * num);
				float num17 = m_Temperature.Evaluate(num7 - 3f * num);
				float num18 = m_Temperature.Evaluate(num7 - 2f * num);
				float num19 = m_Temperature.Evaluate(num7 - 1f * num);
				if (num12 - num13 > num2)
				{
					num11 += 0.19f;
				}
				if (num13 - num14 > num2)
				{
					num11 += 0.17f;
				}
				if (num14 - num15 > num2)
				{
					num11 += 0.12f;
				}
				if (num15 - num16 > num2)
				{
					num11 += 0.09f;
				}
				if (num16 - num17 > num2)
				{
					num11 += 0.11f;
				}
				if (num17 - num18 > num2)
				{
					num11 += 0.13f;
				}
				if (num18 - num19 > num2)
				{
					num11 += 0.14f;
				}
				if (num19 - num10 > num2)
				{
					num11 += 0.15f;
				}
				if (num12 - num15 > num2)
				{
					num11 += 0.21f;
				}
				if (num13 - num16 > num2)
				{
					num11 += 0.18f;
				}
				if (num14 - num17 > num2)
				{
					num11 += 0.07f;
				}
			}
			((Keyframe)(ref array[i])).value = math.saturate(num11);
		}
		m_Fog = new AnimationCurve(array);
		LoopCurve(m_Aurora);
	}

	private static float GaussianRandom(float mean, float dev, ref Random rng)
	{
		int num = 0;
		float num5;
		do
		{
			float num2 = ((Random)(ref rng)).NextFloat();
			float num3 = ((Random)(ref rng)).NextFloat();
			float num4 = math.sqrt(-2f * math.log(num2)) * math.sin((float)Math.PI * 2f * num3);
			num5 = mean + dev * num4;
		}
		while (math.abs(num5 - mean) > 2f * dev && num++ < 20);
		return num5;
	}

	public SeasonTempCurves CreateSeasonTemperatureCurves()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		SeasonTempCurves result = new SeasonTempCurves
		{
			nightMin = new AnimationCurve(),
			nightMax = new AnimationCurve(),
			dayMin = new AnimationCurve(),
			dayMax = new AnimationCurve()
		};
		for (int i = 0; i < m_Seasons.Length; i++)
		{
			(ClimateSystem.SeasonInfo, float) seasonAndMidTime = GetSeasonAndMidTime(i);
			ClimateSystem.SeasonInfo item = seasonAndMidTime.Item1;
			float item2 = seasonAndMidTime.Item2;
			float2 tempNightDay = item.m_TempNightDay;
			float2 val = math.abs(item.m_TempDeviationNightDay);
			result.nightMin.AddKey(item2, tempNightDay.x - val.x);
			result.nightMax.AddKey(item2, tempNightDay.x + val.x);
			result.dayMin.AddKey(item2, tempNightDay.y - val.y);
			result.dayMax.AddKey(item2, tempNightDay.y + val.y);
		}
		LoopCurve(result.nightMin, -100f, 100f);
		LoopCurve(result.nightMax, -100f, 100f);
		LoopCurve(result.dayMin, -100f, 100f);
		LoopCurve(result.dayMax, -100f, 100f);
		return result;
	}

	public (ClimateSystem.SeasonInfo, float) GetSeasonAndMidTime(int index)
	{
		EnsureSeasonsOrder();
		return (m_Seasons[m_SeasonsOrder[index]], GetSeasonMidTime(index));
	}

	public int CountElapsedSeasons(float startTime, float elapsedTime)
	{
		if (m_Seasons == null || m_Seasons.Length == 0)
		{
			return 0;
		}
		if (m_Seasons.Length == 1)
		{
			return 1;
		}
		int num = 0;
		for (int i = 0; i < m_SeasonsOrder.Length; i++)
		{
			ClimateSystem.SeasonInfo seasonInfo = m_Seasons[m_SeasonsOrder[i]];
			ClimateSystem.SeasonInfo obj = m_Seasons[m_SeasonsOrder[(i + 1) % m_Seasons.Length]];
			float startTime2 = seasonInfo.m_StartTime;
			float startTime3 = obj.m_StartTime;
			if (Intersect(startTime, elapsedTime, startTime2, startTime3))
			{
				num++;
			}
		}
		return num;
	}

	private bool Intersect(float startTime, float elapsedTime, float seasonStart, float seasonEnd)
	{
		if (seasonEnd < seasonStart)
		{
			if (startTime < seasonEnd)
			{
				startTime += 1f;
			}
			seasonEnd += 1f;
		}
		if (startTime > seasonEnd)
		{
			startTime -= 1f;
		}
		if (startTime < seasonEnd)
		{
			return startTime + elapsedTime > seasonStart;
		}
		return false;
	}

	public (ClimateSystem.SeasonInfo, float, float) FindSeasonByTime(float time)
	{
		if (m_Seasons == null || m_Seasons.Length == 0)
		{
			return (null, 0f, 1f);
		}
		if (m_Seasons.Length == 1)
		{
			return (m_Seasons[0], 0f, 1f);
		}
		for (int i = 0; i < m_SeasonsOrder.Length; i++)
		{
			ClimateSystem.SeasonInfo seasonInfo = m_Seasons[m_SeasonsOrder[i]];
			ClimateSystem.SeasonInfo obj = m_Seasons[m_SeasonsOrder[(i + 1) % m_Seasons.Length]];
			float startTime = seasonInfo.m_StartTime;
			float num = obj.m_StartTime;
			if (num < startTime)
			{
				num += 1f;
			}
			if (time >= startTime && time < num)
			{
				return (seasonInfo, startTime, num);
			}
			if (num > 1f && time < num - 1f)
			{
				return (seasonInfo, startTime, num);
			}
		}
		return (m_Seasons[0], 0f, 1f);
	}

	private float GetSeasonMidTime(int index)
	{
		ClimateSystem.SeasonInfo seasonInfo = m_Seasons[m_SeasonsOrder[index]];
		ClimateSystem.SeasonInfo obj = m_Seasons[m_SeasonsOrder[(index + 1) % m_Seasons.Length]];
		float startTime = seasonInfo.m_StartTime;
		float num = obj.m_StartTime;
		if (num < startTime)
		{
			num += 1f;
		}
		return (startTime + num) * 0.5f * 12f % 12f;
	}

	public SeasonPrecipCurves CreateSeasonPrecipCurves()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected O, but got Unknown
		SeasonPrecipCurves result = new SeasonPrecipCurves
		{
			cloudChance = new AnimationCurve(),
			cloudAmountMin = new AnimationCurve(),
			cloudAmountMax = new AnimationCurve(),
			precipChance = new AnimationCurve(),
			precipAmountMin = new AnimationCurve(),
			precipAmountMax = new AnimationCurve(),
			turbulence = new AnimationCurve()
		};
		for (int i = 0; i < m_Seasons.Length; i++)
		{
			(ClimateSystem.SeasonInfo, float) seasonAndMidTime = GetSeasonAndMidTime(i);
			ClimateSystem.SeasonInfo item = seasonAndMidTime.Item1;
			float item2 = seasonAndMidTime.Item2;
			float num = item.m_CloudAmount * 0.01f;
			float num2 = math.abs(item.m_CloudAmountDeviation) * 0.01f;
			float num3 = item.m_CloudChance * 0.01f;
			float num4 = item.m_PrecipitationAmount * 0.01f;
			float num5 = math.abs(item.m_PrecipitationAmountDeviation) * 0.01f;
			float num6 = item.m_PrecipitationChance * 0.01f;
			result.cloudAmountMin.AddKey(item2, num - num2);
			result.cloudAmountMax.AddKey(item2, num + num2);
			result.cloudChance.AddKey(item2, num3);
			result.precipAmountMin.AddKey(item2, num4 - num5);
			result.precipAmountMax.AddKey(item2, num4 + num5);
			result.precipChance.AddKey(item2, num6);
			result.turbulence.AddKey(item2, item.m_Turbulence);
		}
		LoopCurve(result.cloudChance);
		LoopCurve(result.cloudAmountMin);
		LoopCurve(result.cloudAmountMax);
		LoopCurve(result.precipChance);
		LoopCurve(result.precipAmountMin);
		LoopCurve(result.precipAmountMax);
		LoopCurve(result.turbulence);
		return result;
	}

	private static void LoopCurve(AnimationCurve curve, float minValue = 0f, float maxValue = 1f)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		WrapMode preWrapMode = (WrapMode)2;
		curve.postWrapMode = (WrapMode)2;
		curve.preWrapMode = preWrapMode;
		for (int i = 0; i < curve.length; i++)
		{
			curve.SmoothTangents(i, 1f / 3f);
		}
		Keyframe[] keys = curve.keys;
		bool flag = false;
		for (int j = 0; j < keys.Length; j++)
		{
			Keyframe val = keys[j];
			if (((Keyframe)(ref val)).value <= minValue)
			{
				((Keyframe)(ref val)).value = minValue;
				float inTangent = (((Keyframe)(ref val)).outTangent = 0f);
				((Keyframe)(ref val)).inTangent = inTangent;
				keys[j] = val;
				flag = true;
			}
			if (((Keyframe)(ref val)).value >= maxValue)
			{
				((Keyframe)(ref val)).value = maxValue;
				float inTangent = (((Keyframe)(ref val)).outTangent = 0f);
				((Keyframe)(ref val)).inTangent = inTangent;
				keys[j] = val;
				flag = true;
			}
		}
		if (flag)
		{
			curve.keys = keys;
		}
		Keyframe val2 = keys[0];
		((Keyframe)(ref val2)).inTangent = 0f;
		((Keyframe)(ref val2)).outTangent = 0f;
		curve.MoveKey(0, val2);
		((Keyframe)(ref val2)).time = ((Keyframe)(ref val2)).time + 12f;
		curve.AddKey(val2);
	}

	public SeasonAuroraCurves CreateSeasonAuroraCurves()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		SeasonAuroraCurves result = new SeasonAuroraCurves
		{
			amount = new AnimationCurve(),
			chance = new AnimationCurve()
		};
		for (int i = 0; i < m_Seasons.Length; i++)
		{
			var (seasonInfo, num) = GetSeasonAndMidTime(i);
			result.amount.AddKey(num, seasonInfo.m_AuroraAmount);
			result.chance.AddKey(num, seasonInfo.m_AuroraChance * 0.01f);
		}
		LoopCurve(result.amount, 0f, 10f);
		LoopCurve(result.chance);
		return result;
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<ClimateData>());
	}

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		if (m_Seasons != null)
		{
			ClimateSystem.SeasonInfo[] seasons = m_Seasons;
			foreach (ClimateSystem.SeasonInfo seasonInfo in seasons)
			{
				prefabs.Add(seasonInfo.m_Prefab);
			}
		}
		if ((Object)(object)m_DefaultWeather != (Object)null)
		{
			prefabs.Add(m_DefaultWeather);
		}
		if (m_DefaultWeathers == null)
		{
			return;
		}
		WeatherPrefab[] defaultWeathers = m_DefaultWeathers;
		foreach (WeatherPrefab weatherPrefab in defaultWeathers)
		{
			if (weatherPrefab.active)
			{
				prefabs.Add(weatherPrefab);
			}
		}
	}
}
