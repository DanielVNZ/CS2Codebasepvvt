using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Prefabs;
using Game.Serialization;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class NaturalResourceSystem : CellMapSystem<NaturalResourceCell>, IJobSerializable, IPostDeserialize
{
	[BurstCompile]
	private struct RegenerateNaturalResourcesJob : IJobParallelFor
	{
		[ReadOnly]
		public int m_FertilityRegenerationRate;

		[ReadOnly]
		public int m_FishRegenerationRate;

		[ReadOnly]
		public float m_PollutionRate;

		[ReadOnly]
		public float m_WaterCellFactor;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public int2 m_WaterResolutionFactor;

		[ReadOnly]
		public CellMapData<GroundPollution> m_GroundPollutionData;

		[ReadOnly]
		public CellMapData<NoisePollution> m_NoisePollutionData;

		[ReadOnly]
		public WaterSurfaceData m_WaterSurfaceData;

		[NativeDisableParallelForRestriction]
		public CellMapData<NaturalResourceCell> m_CellData;

		public void Execute(int zIndex)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			int num = zIndex * m_CellData.m_TextureSize.x;
			int num2 = zIndex * m_WaterResolutionFactor.y * m_WaterSurfaceData.resolution.x;
			for (int i = 0; i < m_CellData.m_TextureSize.x; i++)
			{
				NaturalResourceCell naturalResourceCell = m_CellData.m_Buffer[num];
				GroundPollution groundPollution = m_GroundPollutionData.m_Buffer[num];
				NoisePollution noisePollution = m_NoisePollutionData.m_Buffer[num];
				Random random = m_RandomSeed.GetRandom(1 + num);
				naturalResourceCell.m_Fertility.m_Used = (ushort)math.min((int)naturalResourceCell.m_Fertility.m_Base, math.max(0, naturalResourceCell.m_Fertility.m_Used - m_FertilityRegenerationRate + MathUtils.RoundToIntRandom(ref random, (float)groundPollution.m_Pollution * m_PollutionRate)));
				int num3 = num2;
				float num4 = 0f;
				float num5 = 0f;
				for (int j = 0; j < m_WaterResolutionFactor.y; j++)
				{
					int num6 = num3;
					for (int k = 0; k < m_WaterResolutionFactor.x; k++)
					{
						SurfaceWater surfaceWater = m_WaterSurfaceData.depths[num6++];
						float num7 = math.max(0f, surfaceWater.m_Depth - 2f);
						num4 += num7;
						num5 += num7 * surfaceWater.m_Polluted;
					}
					num3 += m_WaterSurfaceData.resolution.x;
				}
				num4 *= m_WaterCellFactor;
				num5 *= m_WaterCellFactor;
				num5 += num4 * (float)noisePollution.m_Pollution * 6.25E-05f;
				naturalResourceCell.m_Fish.m_Base = (ushort)math.min(10000f, num4);
				int num8 = (int)math.clamp(num5 * 50f, 0f, 10000f);
				if (naturalResourceCell.m_Fish.m_Used < num8)
				{
					naturalResourceCell.m_Fish.m_Used = (ushort)math.min(num8, naturalResourceCell.m_Fish.m_Used + MathUtils.RoundToIntRandom(ref random, num5 * 3.125f));
				}
				else
				{
					naturalResourceCell.m_Fish.m_Used = (ushort)math.max(num8, naturalResourceCell.m_Fish.m_Used - m_FishRegenerationRate);
				}
				m_CellData.m_Buffer[num++] = naturalResourceCell;
				num2 += m_WaterResolutionFactor.x;
			}
		}
	}

	public const int MAX_BASE_RESOURCES = 10000;

	public const int FERTILITY_REGENERATION_RATE = 800;

	public const int FISH_REGENERATION_RATE = 800;

	public const int UPDATES_PER_DAY = 32;

	public static readonly int kTextureSize = 256;

	public GroundPollutionSystem m_GroundPollutionSystem;

	public NoisePollutionSystem m_NoisePollutionSystem;

	public WaterSystem m_WaterSystem;

	private EntityQuery m_PollutionParameterQuery;

	public int2 TextureSize => new int2(kTextureSize, kTextureSize);

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 8192;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_GroundPollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GroundPollutionSystem>();
		m_NoisePollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NoisePollutionSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_PollutionParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PollutionParameterData>() });
		CreateTextures(kTextureSize);
	}

	public override JobHandle SetDefaults(Context context)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Invalid comparison between Unknown and I4
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		JobHandle result = base.SetDefaults(context);
		if ((int)((Context)(ref context)).purpose == 1)
		{
			((JobHandle)(ref result)).Complete();
			float3 val = default(float3);
			float3 val4 = default(float3);
			for (int i = 0; i < m_Map.Length; i++)
			{
				float num = (float)(i % kTextureSize) / (float)kTextureSize;
				float num2 = (float)(i / kTextureSize) / (float)kTextureSize;
				((float3)(ref val))._002Ector(6.1f, 13.9f, 10.7f);
				float3 val2 = num * val;
				float3 val3 = num2 * val;
				val4.x = Mathf.PerlinNoise(val2.x, val3.x);
				val4.y = Mathf.PerlinNoise(val2.y, val3.y);
				val4.z = Mathf.PerlinNoise(val2.z, val3.z);
				val4 = (val4 - new float3(0.4f, 0.7f, 0.7f)) * new float3(5f, 10f, 10f);
				val4 = 10000f * math.saturate(val4);
				NaturalResourceCell naturalResourceCell = new NaturalResourceCell
				{
					m_Fertility = 
					{
						m_Base = (ushort)val4.x
					},
					m_Ore = 
					{
						m_Base = (ushort)val4.y
					},
					m_Oil = 
					{
						m_Base = (ushort)val4.z
					}
				};
				m_Map[i] = naturalResourceCell;
			}
		}
		return result;
	}

	public void PostDeserialize(Context context)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		ContextFormat format = ((Context)(ref context)).format;
		if (!((ContextFormat)(ref format)).Has<FormatTags>(FormatTags.FishResource))
		{
			((ComponentSystemBase)this).Update();
		}
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		JobHandle deps;
		WaterSurfaceData surfaceData = m_WaterSystem.GetSurfaceData(out deps);
		int3 resolution = surfaceData.resolution;
		int2 val = ((int3)(ref resolution)).xz / kTextureSize;
		Assert.AreEqual(GroundPollutionSystem.kTextureSize, kTextureSize, "Ground pollution and Natural resources need to have the same resolution");
		Assert.AreEqual(NoisePollutionSystem.kTextureSize, kTextureSize, "Noise pollution and Natural resources need to have the same resolution");
		resolution = surfaceData.resolution;
		Assert.IsTrue(math.all(((int3)(ref resolution)).xz == val * kTextureSize), "Water resolution much be dividable with natural resources resolution");
		JobHandle dependencies;
		JobHandle dependencies2;
		JobHandle dependencies3;
		JobHandle val2 = IJobParallelForExtensions.Schedule<RegenerateNaturalResourcesJob>(new RegenerateNaturalResourcesJob
		{
			m_FertilityRegenerationRate = 25,
			m_FishRegenerationRate = 25,
			m_PollutionRate = ((EntityQuery)(ref m_PollutionParameterQuery)).GetSingleton<PollutionParameterData>().m_FertilityGroundMultiplier / 32f,
			m_WaterCellFactor = 300f / (float)(val.x * val.y),
			m_RandomSeed = RandomSeed.Next(),
			m_WaterResolutionFactor = val,
			m_GroundPollutionData = m_GroundPollutionSystem.GetData(readOnly: true, out dependencies),
			m_NoisePollutionData = m_NoisePollutionSystem.GetData(readOnly: true, out dependencies2),
			m_WaterSurfaceData = surfaceData,
			m_CellData = GetData(readOnly: false, out dependencies3)
		}, kTextureSize, 1, JobUtils.CombineDependencies(dependencies, dependencies2, dependencies3, deps));
		AddWriter(val2);
		m_GroundPollutionSystem.AddReader(val2);
		m_NoisePollutionSystem.AddReader(val2);
		m_WaterSystem.AddSurfaceReader(val2);
	}

	public float ResourceAmountToArea(float amount)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		float2 val = float2.op_Implicit(CellMapSystem<NaturalResourceCell>.kMapSize) / float2.op_Implicit(TextureSize);
		return amount * val.x * val.y / 10000f;
	}

	public static NaturalResourceAmount GetFertilityAmount(float3 position, NativeArray<NaturalResourceCell> map)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetResource(position, map, (NaturalResourceCell c) => c.m_Fertility);
	}

	public static NaturalResourceAmount GetOilAmount(float3 position, NativeArray<NaturalResourceCell> map)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetResource(position, map, (NaturalResourceCell c) => c.m_Oil);
	}

	public static NaturalResourceAmount GetOreAmount(float3 position, NativeArray<NaturalResourceCell> map)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetResource(position, map, (NaturalResourceCell c) => c.m_Ore);
	}

	public static NaturalResourceAmount GetFishAmount(float3 position, NativeArray<NaturalResourceCell> map)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetResource(position, map, (NaturalResourceCell c) => c.m_Fish);
	}

	private static NaturalResourceAmount GetResource(float3 position, NativeArray<NaturalResourceCell> map, Func<NaturalResourceCell, NaturalResourceAmount> getter)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)CellMapSystem<NaturalResourceCell>.kMapSize / (float)kTextureSize;
		int2 cell = CellMapSystem<NaturalResourceCell>.GetCell(position - new float3(num / 2f, 0f, num / 2f), CellMapSystem<NaturalResourceCell>.kMapSize, kTextureSize);
		float2 cellCoords = CellMapSystem<NaturalResourceCell>.GetCellCoords(position, CellMapSystem<NaturalResourceCell>.kMapSize, kTextureSize) - new float2(0.5f, 0.5f);
		cell = math.clamp(cell, int2.op_Implicit(0), int2.op_Implicit(kTextureSize - 2));
		NaturalResourceAmount naturalResourceAmount = getter(map[cell.x + kTextureSize * cell.y]);
		NaturalResourceAmount naturalResourceAmount2 = getter(map[cell.x + 1 + kTextureSize * cell.y]);
		NaturalResourceAmount naturalResourceAmount3 = getter(map[cell.x + kTextureSize * (cell.y + 1)]);
		NaturalResourceAmount naturalResourceAmount4 = getter(map[cell.x + 1 + kTextureSize * (cell.y + 1)]);
		return new NaturalResourceAmount
		{
			m_Base = FilteringValue(naturalResourceAmount.m_Base, naturalResourceAmount2.m_Base, naturalResourceAmount3.m_Base, naturalResourceAmount4.m_Base),
			m_Used = FilteringValue(naturalResourceAmount.m_Used, naturalResourceAmount2.m_Used, naturalResourceAmount3.m_Used, naturalResourceAmount4.m_Used)
		};
		ushort FilteringValue(ushort p1, ushort p2, ushort p3, ushort p4)
		{
			return (ushort)math.round(math.lerp(math.lerp((float)(int)p1, (float)(int)p2, cellCoords.x - (float)cell.x), math.lerp((float)(int)p3, (float)(int)p4, cellCoords.x - (float)cell.x), cellCoords.y - (float)cell.y));
		}
	}

	[Preserve]
	public NaturalResourceSystem()
	{
	}
}
