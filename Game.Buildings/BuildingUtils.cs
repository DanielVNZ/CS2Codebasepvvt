using System;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Agents;
using Game.Citizens;
using Game.City;
using Game.Economy;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
using Game.Vehicles;
using Game.Zones;
using Unity.Assertions;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Buildings;

public static class BuildingUtils
{
	public struct LotInfo
	{
		public float3 m_Position;

		public float2 m_Extents;

		public float m_Radius;

		public float m_Circular;

		public quaternion m_Rotation;

		public float3 m_FrontHeights;

		public float3 m_RightHeights;

		public float3 m_BackHeights;

		public float3 m_LeftHeights;

		public float3 m_FlatX0;

		public float3 m_FlatZ0;

		public float3 m_FlatX1;

		public float3 m_FlatZ1;

		public float4 m_MinLimit;

		public float4 m_MaxLimit;
	}

	public const float MAX_ROAD_CONNECTION_DISTANCE = 8.4f;

	public const float GEOMETRY_SIZE_OFFSET = 0.4f;

	public const float MIN_BUILDING_HEIGHT = 5f;

	public const float MIN_CONSTRUCTION_HEIGHT = 15f;

	public const float RANDOM_CONSTRUCTION_HEIGHT = 5f;

	public const float COLLAPSE_ACCELERATION = 5f;

	public static Quad3 CalculateCorners(Transform transform, int2 lotSize)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		return CalculateCorners(transform.m_Position, transform.m_Rotation, float2.op_Implicit(lotSize) * 4f);
	}

	public static Quad3 CalculateCorners(float3 position, quaternion rotation, float2 halfLotSize)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		float3 val = math.mul(rotation, new float3(0f, 0f, -1f));
		float3 val2 = math.mul(rotation, new float3(-1f, 0f, 0f));
		float3 val3 = val * halfLotSize.y;
		float3 val4 = val2 * halfLotSize.x;
		float3 val5 = position + val3;
		float3 val6 = position - val3;
		return new Quad3(val5 - val4, val5 + val4, val6 + val4, val6 - val4);
	}

	public static float3 CalculateFrontPosition(Transform transform, int lotDepth)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		float3 position = default(float3);
		((float3)(ref position))._002Ector(0f, 0f, (float)lotDepth * 4f);
		return ObjectUtils.LocalToWorld(transform, position);
	}

	public static float GetEfficiency(BufferAccessor<Efficiency> bufferAccessor, int i)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (bufferAccessor.Length == 0)
		{
			return 1f;
		}
		return GetEfficiency(bufferAccessor[i]);
	}

	public static float GetImmediateEfficiency(BufferAccessor<Efficiency> bufferAccessor, int i)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (bufferAccessor.Length == 0)
		{
			return 1f;
		}
		return GetImmediateEfficiency(bufferAccessor[i]);
	}

	public static float GetEfficiency(Entity entity, ref BufferLookup<Efficiency> bufferLookup)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<Efficiency> buffer = default(DynamicBuffer<Efficiency>);
		if (!bufferLookup.TryGetBuffer(entity, ref buffer))
		{
			return 1f;
		}
		return GetEfficiency(buffer);
	}

	public static float GetEfficiency(DynamicBuffer<Efficiency> buffer)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		float num = 1f;
		Enumerator<Efficiency> enumerator = buffer.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				num *= math.max(0f, enumerator.Current.m_Efficiency);
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		if (!(num > 0f))
		{
			return 0f;
		}
		return math.max(0.01f, math.round(100f * num) / 100f);
	}

	public static float GetImmediateEfficiency(DynamicBuffer<Efficiency> buffer)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		float num = 1f;
		Enumerator<Efficiency> enumerator = buffer.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Efficiency current = enumerator.Current;
				EfficiencyFactor factor = current.m_Factor;
				if (factor <= EfficiencyFactor.Disabled || factor == EfficiencyFactor.ServiceBudget)
				{
					num *= math.max(0f, current.m_Efficiency);
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		if (!(num > 0f))
		{
			return 0f;
		}
		return math.max(0.01f, math.round(100f * num) / 100f);
	}

	public static float GetEfficiency(Span<float> factors)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		float num = 1f;
		Span<float> val = factors;
		for (int i = 0; i < val.Length; i++)
		{
			float num2 = val[i];
			num *= math.max(0f, num2);
		}
		if (!(num > 0f))
		{
			return 0f;
		}
		return math.max(0.01f, math.round(100f * num) / 100f);
	}

	public static void GetEfficiencyFactors(DynamicBuffer<Efficiency> buffer, Span<float> factors)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		factors.Fill(1f);
		Enumerator<Efficiency> enumerator = buffer.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Efficiency current = enumerator.Current;
				factors[(int)current.m_Factor] = current.m_Efficiency;
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
	}

	public static void SetEfficiencyFactors(DynamicBuffer<Efficiency> buffer, Span<float> factors)
	{
		buffer.Clear();
		for (int i = 0; i < factors.Length; i++)
		{
			if ((double)math.abs(factors[i] - 1f) > 0.001)
			{
				buffer.Add(new Efficiency((EfficiencyFactor)i, factors[i]));
			}
		}
	}

	public static void SetEfficiencyFactor(DynamicBuffer<Efficiency> buffer, EfficiencyFactor factor, float efficiency)
	{
		for (int i = 0; i < buffer.Length; i++)
		{
			if (buffer[i].m_Factor == factor)
			{
				if (math.abs(efficiency - 1f) > 0.001f)
				{
					buffer[i] = new Efficiency(factor, efficiency);
				}
				else
				{
					buffer.RemoveAt(i);
				}
				return;
			}
		}
		if (math.abs(efficiency - 1f) > 0.001f)
		{
			buffer.Add(new Efficiency(factor, efficiency));
		}
	}

	public static float2 ApproximateEfficiencyFactors(float targetEfficiency, float2 weights)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		Assert.IsTrue(targetEfficiency >= 0f && targetEfficiency <= 1f);
		Assert.IsTrue(math.cmin(weights) >= 0f);
		bool2 val = weights > 0.001f;
		if (targetEfficiency == 1f || !math.all(val))
		{
			return math.select(float2.op_Implicit(1f), float2.op_Implicit(targetEfficiency), val);
		}
		if (targetEfficiency == 0f)
		{
			return math.select(float2.op_Implicit(1f), float2.op_Implicit(0f), val);
		}
		float num = (weights.x + weights.y) / (2f * weights.x * weights.y);
		float num2 = (1f - targetEfficiency) / (weights.x * weights.y);
		float num3 = num - math.sqrt(num * num - num2);
		return 1f - num3 * weights;
	}

	public static float4 ApproximateEfficiencyFactors(float targetEfficiency, float4 weights)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		Assert.IsTrue(targetEfficiency >= 0f && targetEfficiency <= 1f);
		Assert.IsTrue(math.cmin(weights) >= 0f);
		float num = math.cmax(weights);
		if (targetEfficiency == 1f || num == 0f)
		{
			return float4.op_Implicit(1f);
		}
		if (targetEfficiency == 0f)
		{
			return math.select(float4.op_Implicit(1f), float4.op_Implicit(0f), weights > 1.1920929E-07f);
		}
		float num2 = -1f / num;
		float num3 = 0f;
		float4 val = default(float4);
		for (int i = 0; i < 16; i++)
		{
			float num4 = (num2 + num3) / 2f;
			val = num4 * weights + 1f;
			float num5 = val.x * val.y * val.z * val.w;
			num2 = math.select(num2, num4, num5 < targetEfficiency);
			num3 = math.select(num3, num4, num5 > targetEfficiency);
		}
		return val;
	}

	public static float GetEfficiency(byte rawValue)
	{
		return (float)(int)rawValue / 100f;
	}

	public static int GetLevelingCost(AreaType areaType, BuildingPropertyData propertyData, int currentlevel, DynamicBuffer<CityModifier> cityEffects)
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		int num = propertyData.CountProperties();
		float num2 = 0f;
		switch (areaType)
		{
		case AreaType.Residential:
			num2 = ((currentlevel <= 4) ? (num * Mathf.RoundToInt(math.pow(2f, (float)(2 * currentlevel)) * 40f)) : 1073741823);
			break;
		case AreaType.Commercial:
		case AreaType.Industrial:
			num2 = ((currentlevel <= 4) ? (num * Mathf.RoundToInt(math.pow(2f, (float)(2 * currentlevel)) * 160f)) : 1073741823);
			if (propertyData.m_AllowedStored != Resource.NoResource)
			{
				num2 *= 4f;
			}
			break;
		default:
			num2 = 1.0737418E+09f;
			break;
		}
		CityUtils.ApplyModifier(ref num2, cityEffects, CityModifierType.BuildingLevelingCost);
		return Mathf.RoundToInt(num2);
	}

	public static AreaType GetAreaType(Entity buildPrefab, ref ComponentLookup<SpawnableBuildingData> spawnableBuildingDatas, ref ComponentLookup<ZoneData> zoneDatas)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (spawnableBuildingDatas.HasComponent(buildPrefab) && zoneDatas.HasComponent(spawnableBuildingDatas[buildPrefab].m_ZonePrefab))
		{
			return zoneDatas[spawnableBuildingDatas[buildPrefab].m_ZonePrefab].m_AreaType;
		}
		return AreaType.None;
	}

	public static bool CheckOption(Building building, BuildingOption option)
	{
		return (building.m_OptionMask & (uint)(1 << (int)option)) != 0;
	}

	public static bool CheckOption(InstalledUpgrade installedUpgrade, BuildingOption option)
	{
		return (installedUpgrade.m_OptionMask & (uint)(1 << (int)option)) != 0;
	}

	public static void ApplyModifier(ref float value, DynamicBuffer<BuildingModifier> modifiers, BuildingModifierType type)
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

	public static bool HasOption(BuildingOptionData optionData, BuildingOption option)
	{
		return (optionData.m_OptionMask & (uint)(1 << (int)option)) != 0;
	}

	public static int GetVehicleCapacity(float efficiency, int capacity)
	{
		return math.select(0, math.clamp(Mathf.RoundToInt(efficiency * (float)capacity), 1, capacity), efficiency > 0.001f && capacity > 0);
	}

	public static bool GetAddress(EntityManager entityManager, Entity entity, out Entity road, out int number)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		Building building = default(Building);
		if (EntitiesExtensions.TryGetComponent<Building>(entityManager, entity, ref building))
		{
			return GetAddress(entityManager, entity, building.m_RoadEdge, building.m_CurvePosition, out road, out number);
		}
		Attached attached = default(Attached);
		if (EntitiesExtensions.TryGetComponent<Attached>(entityManager, entity, ref attached))
		{
			return GetAddress(entityManager, entity, attached.m_Parent, attached.m_CurvePosition, out road, out number);
		}
		road = Entity.Null;
		number = 0;
		return false;
	}

	public static bool GetRandomOutsideConnectionByParameters(ref NativeList<Entity> outsideConnections, ref ComponentLookup<OutsideConnectionData> outsideConnectionDatas, ref ComponentLookup<PrefabRef> prefabRefs, Random random, float4 outsideConnectionSpawnParameters, out Entity result)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		OutsideConnectionTransferType ocTransferType = OutsideConnectionTransferType.None;
		float num = ((Random)(ref random)).NextFloat(1f);
		if (num < outsideConnectionSpawnParameters.x)
		{
			ocTransferType = OutsideConnectionTransferType.Road;
		}
		else if (num < outsideConnectionSpawnParameters.x + outsideConnectionSpawnParameters.y)
		{
			ocTransferType = OutsideConnectionTransferType.Train;
		}
		else if (num < outsideConnectionSpawnParameters.x + outsideConnectionSpawnParameters.y + outsideConnectionSpawnParameters.z)
		{
			ocTransferType = OutsideConnectionTransferType.Air;
		}
		else if (num < outsideConnectionSpawnParameters.x + outsideConnectionSpawnParameters.y + outsideConnectionSpawnParameters.z + outsideConnectionSpawnParameters.w)
		{
			ocTransferType = OutsideConnectionTransferType.Ship;
		}
		return GetRandomOutsideConnectionByTransferType(ref outsideConnections, ref outsideConnectionDatas, ref prefabRefs, random, ocTransferType, out result);
	}

	public static bool GetRandomOutsideConnectionByTransferType(ref NativeList<Entity> outsideConnections, ref ComponentLookup<OutsideConnectionData> outsideConnectionDatas, ref ComponentLookup<PrefabRef> prefabRefs, Random random, OutsideConnectionTransferType ocTransferType, out Entity result)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		NativeList<Entity> val = default(NativeList<Entity>);
		val._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		if (ocTransferType != OutsideConnectionTransferType.None)
		{
			for (int i = 0; i < outsideConnections.Length; i++)
			{
				Entity prefab = prefabRefs[outsideConnections[i]].m_Prefab;
				if (outsideConnectionDatas.HasComponent(prefab) && (ocTransferType & outsideConnectionDatas[prefab].m_Type) != OutsideConnectionTransferType.None)
				{
					Entity val2 = outsideConnections[i];
					val.Add(ref val2);
				}
			}
		}
		result = Entity.Null;
		if (val.Length > 0)
		{
			result = val[((Random)(ref random)).NextInt(val.Length)];
			return true;
		}
		return false;
	}

	public static OutsideConnectionTransferType GetOutsideConnectionType(Entity building, ref ComponentLookup<PrefabRef> prefabRefs, ref ComponentLookup<OutsideConnectionData> outsideConnectionDatas)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (outsideConnectionDatas.HasComponent(prefabRefs[building].m_Prefab))
		{
			return outsideConnectionDatas[prefabRefs[building].m_Prefab].m_Type;
		}
		return OutsideConnectionTransferType.None;
	}

	public static bool GetAddress(EntityManager entityManager, Entity entity, Entity edge, float curvePos, out Entity road, out int number)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		Aggregated aggregated = default(Aggregated);
		DynamicBuffer<AggregateElement> val = default(DynamicBuffer<AggregateElement>);
		if (EntitiesExtensions.TryGetComponent<Aggregated>(entityManager, edge, ref aggregated) && EntitiesExtensions.TryGetBuffer<AggregateElement>(entityManager, aggregated.m_Aggregate, true, ref val))
		{
			float num = 0f;
			Curve curve = default(Curve);
			Composition composition = default(Composition);
			NetCompositionData netCompositionData = default(NetCompositionData);
			Edge edge2 = default(Edge);
			Edge edge3 = default(Edge);
			Edge edge4 = default(Edge);
			Edge edge5 = default(Edge);
			Edge edge6 = default(Edge);
			Roundabout roundabout = default(Roundabout);
			Bounds1 val5 = default(Bounds1);
			Transform transform = default(Transform);
			Edge edge7 = default(Edge);
			Roundabout roundabout2 = default(Roundabout);
			PrefabRef prefabRef = default(PrefabRef);
			BuildingData buildingData = default(BuildingData);
			Edge edge8 = default(Edge);
			Roundabout roundabout3 = default(Roundabout);
			PrefabRef prefabRef2 = default(PrefabRef);
			BuildingData buildingData2 = default(BuildingData);
			for (int i = 0; i < val.Length; i++)
			{
				AggregateElement aggregateElement = val[i];
				float num2 = num;
				float3 val2;
				if (EntitiesExtensions.TryGetComponent<Curve>(entityManager, aggregateElement.m_Edge, ref curve) && EntitiesExtensions.TryGetComponent<Composition>(entityManager, aggregateElement.m_Edge, ref composition) && EntitiesExtensions.TryGetComponent<NetCompositionData>(entityManager, composition.m_Edge, ref netCompositionData))
				{
					val2 = MathUtils.StartTangent(curve.m_Bezier);
					float2 val3 = math.normalizesafe(((float3)(ref val2)).xz, default(float2));
					val2 = MathUtils.EndTangent(curve.m_Bezier);
					float2 val4 = math.normalizesafe(((float3)(ref val2)).xz, default(float2));
					float num3 = ZoneUtils.GetCellWidth(netCompositionData.m_Width);
					float num4 = math.acos(math.clamp(math.dot(val3, val4), -1f, 1f));
					num2 += curve.m_Length + num3 * num4 * 0.5f;
				}
				bool flag = i == 0;
				bool flag2 = i == val.Length - 1;
				bool flag3 = aggregateElement.m_Edge == edge;
				bool flag4 = false;
				if (flag3 || flag || flag2)
				{
					if (!flag)
					{
						if (EntitiesExtensions.TryGetComponent<Edge>(entityManager, aggregateElement.m_Edge, ref edge2) && EntitiesExtensions.TryGetComponent<Edge>(entityManager, val[i - 1].m_Edge, ref edge3) && (edge2.m_End == edge3.m_Start || edge2.m_End == edge3.m_End))
						{
							flag4 = true;
						}
					}
					else if (!flag2 && EntitiesExtensions.TryGetComponent<Edge>(entityManager, aggregateElement.m_Edge, ref edge4) && EntitiesExtensions.TryGetComponent<Edge>(entityManager, val[i + 1].m_Edge, ref edge5) && (edge4.m_Start == edge5.m_Start || edge4.m_Start == edge5.m_End))
					{
						flag4 = true;
					}
					if (flag && EntitiesExtensions.TryGetComponent<Edge>(entityManager, aggregateElement.m_Edge, ref edge6) && EntitiesExtensions.TryGetComponent<Roundabout>(entityManager, flag4 ? edge6.m_End : edge6.m_Start, ref roundabout))
					{
						num += roundabout.m_Radius;
					}
					if (flag3)
					{
						((Bounds1)(ref val5))._002Ector(flag4 ? curvePos : 0f, flag4 ? 1f : curvePos);
						float num5 = math.saturate(MathUtils.Length(curve.m_Bezier, val5) / math.max(1f, curve.m_Length));
						float num6 = math.lerp(num, num2, num5);
						bool flag5 = false;
						if (EntitiesExtensions.TryGetComponent<Transform>(entityManager, entity, ref transform))
						{
							if (num5 < 0.01f && EntitiesExtensions.TryGetComponent<Edge>(entityManager, aggregateElement.m_Edge, ref edge7) && EntitiesExtensions.TryGetComponent<Roundabout>(entityManager, edge7.m_Start, ref roundabout2) && EntitiesExtensions.TryGetComponent<PrefabRef>(entityManager, entity, ref prefabRef) && EntitiesExtensions.TryGetComponent<BuildingData>(entityManager, prefabRef.m_Prefab, ref buildingData))
							{
								float3 val6 = CalculateFrontPosition(transform, buildingData.m_LotSize.y);
								val2 = MathUtils.StartTangent(curve.m_Bezier);
								float2 xz = ((float3)(ref val2)).xz;
								if (MathUtils.TryNormalize(ref xz))
								{
									float num7 = math.dot(xz, ((float3)(ref curve.m_Bezier.a)).xz - ((float3)(ref val6)).xz);
									num7 = math.clamp(num7, 0f, roundabout2.m_Radius);
									num6 += math.select(0f - num7, num7, flag4);
								}
							}
							if (num5 > 0.99f && EntitiesExtensions.TryGetComponent<Edge>(entityManager, aggregateElement.m_Edge, ref edge8) && EntitiesExtensions.TryGetComponent<Roundabout>(entityManager, edge8.m_End, ref roundabout3) && EntitiesExtensions.TryGetComponent<PrefabRef>(entityManager, entity, ref prefabRef2) && EntitiesExtensions.TryGetComponent<BuildingData>(entityManager, prefabRef2.m_Prefab, ref buildingData2))
							{
								float3 val7 = CalculateFrontPosition(transform, buildingData2.m_LotSize.y);
								val2 = MathUtils.EndTangent(curve.m_Bezier);
								float2 xz2 = ((float3)(ref val2)).xz;
								if (MathUtils.TryNormalize(ref xz2))
								{
									float num8 = math.dot(xz2, ((float3)(ref val7)).xz - ((float3)(ref curve.m_Bezier.d)).xz);
									num8 = math.clamp(num8, 0f, roundabout3.m_Radius);
									num6 += math.select(num8, 0f - num8, flag4);
								}
							}
							float2 xz3 = ((float3)(ref transform.m_Position)).xz;
							val2 = MathUtils.Position(curve.m_Bezier, curvePos);
							float2 val8 = xz3 - ((float3)(ref val2)).xz;
							val2 = MathUtils.Tangent(curve.m_Bezier, curvePos);
							float2 val9 = MathUtils.Right(((float3)(ref val2)).xz);
							flag5 = math.dot(val8, val9) > 0f != flag4;
						}
						road = aggregated.m_Aggregate;
						number = Mathf.RoundToInt(num6 / 8f) * 2 + ((!flag5) ? 1 : 2);
						return true;
					}
				}
				num = num2;
			}
		}
		road = Entity.Null;
		number = 0;
		return false;
	}

	public static LotInfo CalculateLotInfo(float2 extents, Transform transform, Game.Objects.Elevation elevation, Lot lot, PrefabRef prefabRef, DynamicBuffer<InstalledUpgrade> upgrades, ComponentLookup<Transform> transforms, ComponentLookup<PrefabRef> prefabRefs, ComponentLookup<ObjectGeometryData> objectGeometryDatas, ComponentLookup<BuildingTerraformData> buildingTerraformDatas, ComponentLookup<BuildingExtensionData> buildingExtensionDatas, bool defaultNoSmooth, out bool hasExtensionLots)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		LotInfo result = new LotInfo
		{
			m_Position = transform.m_Position,
			m_Extents = extents,
			m_Rotation = transform.m_Rotation,
			m_Radius = math.length(extents),
			m_Circular = 0f,
			m_FrontHeights = lot.m_FrontHeights,
			m_RightHeights = lot.m_RightHeights,
			m_BackHeights = lot.m_BackHeights,
			m_LeftHeights = lot.m_LeftHeights,
			m_FlatX0 = float3.op_Implicit(0f - extents.x),
			m_FlatZ0 = float3.op_Implicit(0f - extents.y),
			m_FlatX1 = float3.op_Implicit(extents.x),
			m_FlatZ1 = float3.op_Implicit(extents.y),
			m_MinLimit = new float4(-((float2)(ref extents)).xy, ((float2)(ref extents)).xy),
			m_MaxLimit = new float4(-((float2)(ref extents)).xy, ((float2)(ref extents)).xy)
		};
		ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
		if (objectGeometryDatas.TryGetComponent(prefabRef.m_Prefab, ref objectGeometryData))
		{
			bool flag = (objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Standing) != 0;
			bool flag2 = ((uint)objectGeometryData.m_Flags & (uint)((!flag) ? 1 : 256)) != 0;
			result.m_Circular = math.select(0f, 1f, flag2);
		}
		BuildingTerraformData buildingTerraformData = default(BuildingTerraformData);
		if (buildingTerraformDatas.TryGetComponent(prefabRef.m_Prefab, ref buildingTerraformData))
		{
			result.m_Position.y += buildingTerraformData.m_HeightOffset;
			result.m_FlatX0 = buildingTerraformData.m_FlatX0;
			result.m_FlatZ0 = buildingTerraformData.m_FlatZ0;
			result.m_FlatX1 = buildingTerraformData.m_FlatX1;
			result.m_FlatZ1 = buildingTerraformData.m_FlatZ1;
			result.m_MinLimit = buildingTerraformData.m_Smooth;
			result.m_MaxLimit = buildingTerraformData.m_Smooth;
		}
		else
		{
			buildingTerraformData.m_DontLower = defaultNoSmooth;
			buildingTerraformData.m_DontRaise = defaultNoSmooth;
		}
		hasExtensionLots = false;
		if (upgrades.IsCreated)
		{
			BuildingExtensionData buildingExtensionData = default(BuildingExtensionData);
			BuildingTerraformData buildingTerraformData2 = default(BuildingTerraformData);
			ObjectGeometryData objectGeometryData2 = default(ObjectGeometryData);
			for (int i = 0; i < upgrades.Length; i++)
			{
				Entity upgrade = upgrades[i].m_Upgrade;
				PrefabRef prefabRef2 = prefabRefs[upgrade];
				if (buildingExtensionDatas.TryGetComponent(prefabRef2.m_Prefab, ref buildingExtensionData) && !buildingExtensionData.m_External && buildingTerraformDatas.TryGetComponent(prefabRef2.m_Prefab, ref buildingTerraformData2))
				{
					float3 val = transforms[upgrade].m_Position - transform.m_Position;
					float num = 0f;
					if (objectGeometryDatas.TryGetComponent(prefabRef2.m_Prefab, ref objectGeometryData2))
					{
						bool flag3 = (objectGeometryData2.m_Flags & Game.Objects.GeometryFlags.Standing) != 0;
						bool flag4 = ((uint)objectGeometryData2.m_Flags & (uint)((!flag3) ? 1 : 256)) != 0;
						num = math.select(0f, 1f, flag4);
					}
					result.m_FlatX0 = math.min(result.m_FlatX0, buildingTerraformData2.m_FlatX0 + val.x);
					result.m_FlatZ0 = math.min(result.m_FlatZ0, buildingTerraformData2.m_FlatZ0 + val.z);
					result.m_FlatX1 = math.max(result.m_FlatX1, buildingTerraformData2.m_FlatX1 + val.x);
					result.m_FlatZ1 = math.max(result.m_FlatZ1, buildingTerraformData2.m_FlatZ1 + val.z);
					if (!math.all(buildingTerraformData2.m_Smooth + ((float3)(ref val)).xzxz == result.m_MaxLimit) || num != result.m_Circular)
					{
						hasExtensionLots = true;
					}
				}
			}
		}
		((float4)(ref result.m_MinLimit)).xy = math.min(new float2(result.m_FlatX0.y, result.m_FlatZ0.y), ((float4)(ref result.m_MinLimit)).xy);
		((float4)(ref result.m_MinLimit)).zw = math.max(new float2(result.m_FlatX1.y, result.m_FlatZ1.y), ((float4)(ref result.m_MinLimit)).zw);
		extents = math.max(extents, float2.op_Implicit(8f));
		if (buildingTerraformData.m_DontLower)
		{
			result.m_MinLimit = new float4(((float2)(ref extents)).xy, -((float2)(ref extents)).xy);
		}
		if (elevation.m_Elevation > 0f || buildingTerraformData.m_DontRaise)
		{
			result.m_MaxLimit = new float4(((float2)(ref extents)).xy, -((float2)(ref extents)).xy);
		}
		return result;
	}

	public static float SampleHeight(ref LotInfo lotInfo, float3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_050f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0516: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Unknown result type (might be due to invalid IL or missing references)
		//IL_052a: Unknown result type (might be due to invalid IL or missing references)
		//IL_052f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0542: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0550: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_055c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0561: Unknown result type (might be due to invalid IL or missing references)
		//IL_0566: Unknown result type (might be due to invalid IL or missing references)
		//IL_056b: Unknown result type (might be due to invalid IL or missing references)
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_056f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0571: Unknown result type (might be due to invalid IL or missing references)
		//IL_0576: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_057f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0581: Unknown result type (might be due to invalid IL or missing references)
		//IL_0586: Unknown result type (might be due to invalid IL or missing references)
		//IL_058a: Unknown result type (might be due to invalid IL or missing references)
		//IL_058f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0594: Unknown result type (might be due to invalid IL or missing references)
		//IL_059b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05db: Unknown result type (might be due to invalid IL or missing references)
		//IL_05df: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0603: Unknown result type (might be due to invalid IL or missing references)
		//IL_060d: Unknown result type (might be due to invalid IL or missing references)
		//IL_060f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0611: Unknown result type (might be due to invalid IL or missing references)
		//IL_0616: Unknown result type (might be due to invalid IL or missing references)
		//IL_061b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0622: Unknown result type (might be due to invalid IL or missing references)
		//IL_0627: Unknown result type (might be due to invalid IL or missing references)
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		//IL_063b: Unknown result type (might be due to invalid IL or missing references)
		//IL_063c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0643: Unknown result type (might be due to invalid IL or missing references)
		//IL_064f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0650: Unknown result type (might be due to invalid IL or missing references)
		//IL_0657: Unknown result type (might be due to invalid IL or missing references)
		//IL_0661: Unknown result type (might be due to invalid IL or missing references)
		//IL_0663: Unknown result type (might be due to invalid IL or missing references)
		//IL_0665: Unknown result type (might be due to invalid IL or missing references)
		//IL_066a: Unknown result type (might be due to invalid IL or missing references)
		//IL_066f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0676: Unknown result type (might be due to invalid IL or missing references)
		//IL_067b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0688: Unknown result type (might be due to invalid IL or missing references)
		//IL_068f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0696: Unknown result type (might be due to invalid IL or missing references)
		//IL_069b: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06de: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0708: Unknown result type (might be due to invalid IL or missing references)
		//IL_0714: Unknown result type (might be due to invalid IL or missing references)
		//IL_071c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0723: Unknown result type (might be due to invalid IL or missing references)
		//IL_0738: Unknown result type (might be due to invalid IL or missing references)
		//IL_073f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0746: Unknown result type (might be due to invalid IL or missing references)
		//IL_0759: Unknown result type (might be due to invalid IL or missing references)
		//IL_0770: Unknown result type (might be due to invalid IL or missing references)
		position = math.mul(math.inverse(lotInfo.m_Rotation), position - lotInfo.m_Position);
		Bezier4x2 val = default(Bezier4x2);
		((Bezier4x2)(ref val))._002Ector(new float2(lotInfo.m_RightHeights.x, lotInfo.m_FrontHeights.x), new float2(lotInfo.m_RightHeights.y, lotInfo.m_LeftHeights.z), new float2(lotInfo.m_RightHeights.z, lotInfo.m_LeftHeights.y), new float2(lotInfo.m_BackHeights.x, lotInfo.m_LeftHeights.x));
		Bezier4x2 val2 = default(Bezier4x2);
		((Bezier4x2)(ref val2))._002Ector(new float2(lotInfo.m_RightHeights.x, lotInfo.m_BackHeights.x), new float2(lotInfo.m_FrontHeights.z, lotInfo.m_BackHeights.y), new float2(lotInfo.m_FrontHeights.y, lotInfo.m_BackHeights.z), new float2(lotInfo.m_FrontHeights.x, lotInfo.m_LeftHeights.x));
		float2 val3 = math.clamp(((float3)(ref position)).xz, -lotInfo.m_Extents, lotInfo.m_Extents);
		float2 val4 = 0.5f / math.max(float2.op_Implicit(0.01f), lotInfo.m_Extents);
		float2 val5 = ((float3)(ref position)).xz * val4 + 0.5f;
		float2 val6 = math.saturate(val5);
		float2 val7 = ((float3)(ref position)).xz - val3;
		float2 val8 = val5 - math.sign(val7) * val4 * 2f;
		float2 val9 = (val8 - 0.5f) * (lotInfo.m_Extents * 2f);
		val7 = 8f - 8f / (1f + math.abs(val7) * 0.125f);
		float4 val10 = default(float4);
		((float4)(ref val10))._002Ector(((float3)(ref lotInfo.m_FlatX0)).xy, ((float3)(ref lotInfo.m_FlatZ0)).yx);
		float4 val11 = default(float4);
		((float4)(ref val11))._002Ector(((float3)(ref lotInfo.m_FlatZ0)).xy, ((float3)(ref lotInfo.m_FlatX0)).yx);
		float4 val12 = default(float4);
		((float4)(ref val12))._002Ector(((float3)(ref lotInfo.m_FlatX1)).xy, ((float3)(ref lotInfo.m_FlatZ0)).yz);
		float4 val13 = default(float4);
		((float4)(ref val13))._002Ector(((float3)(ref lotInfo.m_FlatZ1)).xy, ((float3)(ref lotInfo.m_FlatX0)).yz);
		val10 = math.select(val10, new float4(((float3)(ref lotInfo.m_FlatX0)).yy, lotInfo.m_FlatZ0.x, lotInfo.m_FlatZ1.x), val3.y > val10.w);
		val11 = math.select(val11, new float4(((float3)(ref lotInfo.m_FlatZ0)).yy, lotInfo.m_FlatX0.x, lotInfo.m_FlatX1.x), val3.x > val11.w);
		val12 = math.select(val12, new float4(((float3)(ref lotInfo.m_FlatX1)).yy, lotInfo.m_FlatZ0.z, lotInfo.m_FlatZ1.z), val3.y > val12.w);
		val13 = math.select(val13, new float4(((float3)(ref lotInfo.m_FlatZ1)).yy, lotInfo.m_FlatX0.z, lotInfo.m_FlatX1.z), val3.x > val13.w);
		val10 = math.select(val10, new float4(((float3)(ref lotInfo.m_FlatX0)).yz, ((float3)(ref lotInfo.m_FlatZ1)).xy), val3.y > val10.w);
		val11 = math.select(val11, new float4(((float3)(ref lotInfo.m_FlatZ0)).yz, ((float3)(ref lotInfo.m_FlatX1)).xy), val3.x > val11.w);
		val12 = math.select(val12, new float4(((float3)(ref lotInfo.m_FlatX1)).yz, ((float3)(ref lotInfo.m_FlatZ1)).zy), val3.y > val12.w);
		val13 = math.select(val13, new float4(((float3)(ref lotInfo.m_FlatZ1)).yz, ((float3)(ref lotInfo.m_FlatX1)).zy), val3.x > val13.w);
		float4 val14 = new float4(val10.x, val11.x, val12.x, val13.x);
		float4 val15 = default(float4);
		((float4)(ref val15))._002Ector(val10.y, val11.y, val12.y, val13.y);
		float4 val16 = default(float4);
		((float4)(ref val16))._002Ector(val10.z, val11.z, val12.z, val13.z);
		float4 val17 = default(float4);
		((float4)(ref val17))._002Ector(val10.w, val11.w, val12.w, val13.w);
		float4 val18 = (((float2)(ref val3)).yxyx - val16) / math.max(val17 - val16, float4.op_Implicit(0.1f));
		val18 = math.lerp(val14, val15, val18);
		val18 = math.saturate(new float4(((float4)(ref val18)).xy - val3, val3 - ((float4)(ref val18)).zw) / math.max(new float4(((float4)(ref val18)).xy + lotInfo.m_Extents, lotInfo.m_Extents - ((float4)(ref val18)).zw), float4.op_Implicit(0.1f)));
		float4 val19 = (((float2)(ref val9)).yxyx - val16) / math.max(val17 - val16, float4.op_Implicit(0.1f));
		val19 = math.lerp(val14, val15, val19);
		val19 = math.saturate(new float4(((float4)(ref val19)).xy - val9, val9 - ((float4)(ref val19)).zw) / math.max(new float4(((float4)(ref val19)).xy + lotInfo.m_Extents, lotInfo.m_Extents - ((float4)(ref val19)).zw), float4.op_Implicit(0.1f)));
		float4 val20 = default(float4);
		((float4)(ref val20)).xz = MathUtils.Position(val, val6.y);
		((float4)(ref val20)).yw = MathUtils.Position(val2, val6.x);
		val20 *= val18;
		((float4)(ref val20)).xy = ((float4)(ref val20)).xy + ((float4)(ref val20)).zw;
		float4 val21 = default(float4);
		((float4)(ref val21)).xz = MathUtils.Position(val, val8.y);
		((float4)(ref val21)).yw = MathUtils.Position(val2, val8.x);
		val21 *= val19;
		((float4)(ref val21)).xy = ((float4)(ref val21)).xy + ((float4)(ref val21)).zw;
		((float4)(ref val20)).xy = ((float4)(ref val20)).xy + (((float4)(ref val20)).xy - ((float4)(ref val21)).xy) * ((float2)(ref val7)).xy * 0.5f;
		((float4)(ref val18)).xy = math.max(((float4)(ref val18)).xy, ((float4)(ref val18)).zw);
		((float4)(ref val18)).xy = ((float4)(ref val18)).xy / math.max(1f, val18.x + val18.y);
		val18.x = math.select(val18.y, 1f - val18.x, val18.x > val18.y);
		val20.x = math.lerp(val20.x, val20.y, val18.x);
		position.y = val20.x;
		return lotInfo.m_Position.y + position.y;
	}

	public static float GetCollapseTime(float height)
	{
		return math.sqrt(math.max(0f, height) * 0.4f);
	}

	public static float GetCollapseHeight(float time)
	{
		return 2.5f * math.lengthsq(time);
	}

	public static MaintenanceType GetMaintenanceType(Entity entity, ref ComponentLookup<Park> parks, ref ComponentLookup<NetCondition> netConditions, ref ComponentLookup<Edge> edges, ref ComponentLookup<Surface> surfaces, ref ComponentLookup<Vehicle> vehicles)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (parks.HasComponent(entity))
		{
			return MaintenanceType.Park;
		}
		if (netConditions.HasComponent(entity))
		{
			Surface surface = default(Surface);
			Edge edge = default(Edge);
			Surface surface2 = default(Surface);
			Surface surface3 = default(Surface);
			if (!surfaces.TryGetComponent(entity, ref surface) && edges.TryGetComponent(entity, ref edge) && surfaces.TryGetComponent(edge.m_Start, ref surface2) && surfaces.TryGetComponent(edge.m_End, ref surface3))
			{
				surface.m_AccumulatedSnow = (byte)(surface2.m_AccumulatedSnow + surface3.m_AccumulatedSnow + 1 >> 1);
			}
			if (surface.m_AccumulatedSnow >= 15)
			{
				return MaintenanceType.Snow;
			}
			return MaintenanceType.Road;
		}
		if (vehicles.HasComponent(entity))
		{
			return MaintenanceType.Vehicle;
		}
		return MaintenanceType.None;
	}

	public static void CalculateUpgradeRangeValues(quaternion rotation, BuildingData ownerBuildingData, BuildingData buildingData, ServiceUpgradeData serviceUpgradeData, out float3 forward, out float width, out float length, out float roundness, out bool circular)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		forward = math.forward(rotation);
		if (ownerBuildingData.m_LotSize.y < ownerBuildingData.m_LotSize.x)
		{
			ownerBuildingData.m_LotSize = ((int2)(ref ownerBuildingData.m_LotSize)).yx;
			((float3)(ref forward)).xz = MathUtils.Right(((float3)(ref forward)).xz);
		}
		float num = serviceUpgradeData.m_MaxPlacementDistance + (float)buildingData.m_LotSize.y * 8f;
		width = (float)ownerBuildingData.m_LotSize.x * 8f + num * 2f;
		length = (float)ownerBuildingData.m_LotSize.y * 8f + num * 2f;
		roundness = math.max(0f, num - 40f) * 1.2f + 8f;
		width = math.min(length, math.max(width, roundness * 2f));
		roundness = math.min(roundness, width * 0.5f);
		circular = length * 0.5f - roundness < 1f;
	}

	public static bool IsHomelessShelterBuilding(Entity propertyEntity, ref ComponentLookup<Park> parks, ref ComponentLookup<Abandoned> abandoneds)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (!parks.HasComponent(propertyEntity))
		{
			return abandoneds.HasComponent(propertyEntity);
		}
		return true;
	}

	public static bool IsHomelessShelterBuilding(EntityManager entityManager, Entity propertyEntity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityManager)(ref entityManager)).HasComponent<Park>(propertyEntity))
		{
			return ((EntityManager)(ref entityManager)).HasComponent<Abandoned>(propertyEntity);
		}
		return true;
	}

	public static bool IsHomelessHousehold(Household household, Entity propertyEntity, ref ComponentLookup<Park> parks, ref ComponentLookup<Abandoned> abandoneds)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if ((household.m_Flags & HouseholdFlags.MovedIn) != HouseholdFlags.None)
		{
			if (!(propertyEntity == Entity.Null))
			{
				return IsHomelessShelterBuilding(propertyEntity, ref parks, ref abandoneds);
			}
			return true;
		}
		return false;
	}

	public static bool IsHomelessHousehold(EntityManager entityManager, Entity householdEntity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		Household household = default(Household);
		if (EntitiesExtensions.TryGetComponent<Household>(entityManager, householdEntity, ref household) && (household.m_Flags & HouseholdFlags.MovedIn) != HouseholdFlags.None && !((EntityManager)(ref entityManager)).HasComponent<MovingAway>(householdEntity))
		{
			PropertyRenter propertyRenter = default(PropertyRenter);
			if (EntitiesExtensions.TryGetComponent<PropertyRenter>(entityManager, householdEntity, ref propertyRenter) && !(propertyRenter.m_Property == Entity.Null) && !((EntityManager)(ref entityManager)).HasComponent<Park>(propertyRenter.m_Property))
			{
				return ((EntityManager)(ref entityManager)).HasComponent<Abandoned>(propertyRenter.m_Property);
			}
			return true;
		}
		return false;
	}

	public static Entity GetPropertyFromRenter(Entity renter, ref ComponentLookup<HomelessHousehold> homelessHouseholds, ref ComponentLookup<PropertyRenter> propertyRenters)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (homelessHouseholds.HasComponent(renter))
		{
			return homelessHouseholds[renter].m_TempHome;
		}
		if (propertyRenters.HasComponent(renter))
		{
			return propertyRenters[renter].m_Property;
		}
		return Entity.Null;
	}

	public static Entity GetHouseholdHomeBuilding(Entity householdEntity, ref ComponentLookup<PropertyRenter> propertyRenters, ref ComponentLookup<HomelessHousehold> homelessHouseholds)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		PropertyRenter propertyRenter = default(PropertyRenter);
		if (propertyRenters.TryGetComponent(householdEntity, ref propertyRenter))
		{
			return propertyRenter.m_Property;
		}
		HomelessHousehold homelessHousehold = default(HomelessHousehold);
		if (homelessHouseholds.TryGetComponent(householdEntity, ref homelessHousehold))
		{
			return homelessHousehold.m_TempHome;
		}
		return Entity.Null;
	}

	public static Entity GetHouseholdHomeBuilding(Entity householdEntity, ref ComponentLookup<PropertyRenter> propertyRenters, ref ComponentLookup<HomelessHousehold> homelessHouseholds, ref ComponentLookup<TouristHousehold> touristHouseholds)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		PropertyRenter propertyRenter = default(PropertyRenter);
		if (propertyRenters.TryGetComponent(householdEntity, ref propertyRenter))
		{
			return propertyRenter.m_Property;
		}
		TouristHousehold touristHousehold = default(TouristHousehold);
		if (touristHouseholds.TryGetComponent(householdEntity, ref touristHousehold) && propertyRenters.TryGetComponent(touristHousehold.m_Hotel, ref propertyRenter))
		{
			return propertyRenter.m_Property;
		}
		HomelessHousehold homelessHousehold = default(HomelessHousehold);
		if (homelessHouseholds.TryGetComponent(householdEntity, ref homelessHousehold))
		{
			return homelessHousehold.m_TempHome;
		}
		return Entity.Null;
	}

	public static int GetShelterHomelessCapacity(Entity buildingPrefabEntity, ref ComponentLookup<BuildingData> buildingDatas, ref ComponentLookup<BuildingPropertyData> buildingPropertyDatas)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (!buildingDatas.HasComponent(buildingPrefabEntity))
		{
			return 0;
		}
		BuildingData buildingData = buildingDatas[buildingPrefabEntity];
		int num = buildingData.m_LotSize.x * buildingData.m_LotSize.y;
		if (!buildingPropertyDatas.HasComponent(buildingPrefabEntity))
		{
			return num / 4;
		}
		BuildingPropertyData buildingPropertyData = buildingPropertyDatas[buildingPrefabEntity];
		float num2 = buildingPropertyData.m_ResidentialProperties;
		if (buildingPropertyData.m_AllowedSold != Resource.NoResource || buildingPropertyData.m_AllowedManufactured != Resource.NoResource || buildingPropertyData.m_AllowedStored != Resource.NoResource)
		{
			num2 += buildingPropertyData.m_SpaceMultiplier * (float)num;
		}
		return Mathf.CeilToInt(num2 / 2f);
	}
}
