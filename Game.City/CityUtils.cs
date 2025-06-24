using System;
using Game.Buildings;
using Game.Common;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.City;

public static class CityUtils
{
	public static bool CheckOption(City city, CityOption option)
	{
		return (city.m_OptionMask & (uint)(1 << (int)option)) != 0;
	}

	public static void ApplyModifier(ref float value, DynamicBuffer<CityModifier> modifiers, CityModifierType type)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (modifiers.Length > (int)type)
		{
			float2 delta = modifiers[(int)type].m_Delta;
			value += delta.x;
			value += value * delta.y;
		}
	}

	public static float2 GetModifier(DynamicBuffer<CityModifier> modifiers, CityModifierType type)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (modifiers.Length > (int)type)
		{
			return modifiers[(int)type].m_Delta;
		}
		return default(float2);
	}

	public static bool HasOption(CityOptionData optionData, CityOption option)
	{
		return (optionData.m_OptionMask & (uint)(1 << (int)option)) != 0;
	}

	public static int GetCityServiceWorkplaceMaxWorkers(Entity ownerEntity, ref ComponentLookup<PrefabRef> prefabRefs, ref BufferLookup<InstalledUpgrade> installedUpgrades, ref ComponentLookup<Deleted> deleteds, ref ComponentLookup<WorkplaceData> workplaceDatas, ref ComponentLookup<SchoolData> schoolDatas, ref BufferLookup<Student> studentBufs)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		int result = 0;
		if (deleteds.HasComponent(ownerEntity))
		{
			return result;
		}
		Entity val = prefabRefs[ownerEntity];
		if (!workplaceDatas.HasComponent(val))
		{
			return result;
		}
		result = workplaceDatas[val].m_MaxWorkers;
		if (!installedUpgrades.HasBuffer(ownerEntity))
		{
			return result;
		}
		int num = ((workplaceDatas[val].m_MinimumWorkersLimit == 0) ? result : workplaceDatas[val].m_MinimumWorkersLimit);
		Enumerator<InstalledUpgrade> enumerator = installedUpgrades[ownerEntity].GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				InstalledUpgrade current = enumerator.Current;
				if (prefabRefs.HasComponent(current.m_Upgrade) && !deleteds.HasComponent(current.m_Upgrade))
				{
					Entity val2 = prefabRefs[current.m_Upgrade];
					if (workplaceDatas.HasComponent(val2))
					{
						num += workplaceDatas[val2].m_MinimumWorkersLimit;
						result += workplaceDatas[val2].m_MaxWorkers;
					}
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		if (schoolDatas.HasComponent(val))
		{
			int studentCapacity = schoolDatas[val].m_StudentCapacity;
			int length = studentBufs[ownerEntity].Length;
			result = math.max(num, (int)Mathf.Lerp(0f, (float)result, 1f * (float)length / (float)studentCapacity));
		}
		return result;
	}
}
