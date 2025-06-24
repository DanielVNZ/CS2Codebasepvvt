using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Serialization.Entities;
using Game.Prefabs;
using Unity.Assertions;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class GroundWaterSystem : CellMapSystem<GroundWater>, IJobSerializable
{
	[BurstCompile]
	private struct GroundWaterTickJob : IJob
	{
		public NativeArray<GroundWater> m_GroundWaterMap;

		public WaterPipeParameterData m_Parameters;

		private void HandlePollution(int index, int otherIndex, NativeArray<int2> tmp)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			GroundWater groundWater = m_GroundWaterMap[index];
			GroundWater groundWater2 = m_GroundWaterMap[otherIndex];
			ref int2 reference = ref CollectionUtils.ElementAt<int2>(tmp, index);
			ref int2 reference2 = ref CollectionUtils.ElementAt<int2>(tmp, otherIndex);
			int num = groundWater.m_Polluted + groundWater2.m_Polluted;
			int num2 = groundWater.m_Amount + groundWater2.m_Amount;
			int num3 = math.clamp((((num2 > 0) ? (groundWater.m_Amount * num / num2) : 0) - groundWater.m_Polluted) / 4, -(groundWater2.m_Amount - groundWater2.m_Polluted) / 4, (groundWater.m_Amount - groundWater.m_Polluted) / 4);
			reference.y += num3;
			reference2.y -= num3;
			Assert.IsTrue(0 <= groundWater.m_Polluted + reference.y);
			Assert.IsTrue(groundWater.m_Polluted + reference.y <= groundWater.m_Amount);
			Assert.IsTrue(0 <= groundWater2.m_Polluted + reference2.y);
			Assert.IsTrue(groundWater2.m_Polluted + reference2.y <= groundWater2.m_Amount);
		}

		private void HandleFlow(int index, int otherIndex, NativeArray<int2> tmp)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			GroundWater groundWater = m_GroundWaterMap[index];
			GroundWater groundWater2 = m_GroundWaterMap[otherIndex];
			ref int2 reference = ref CollectionUtils.ElementAt<int2>(tmp, index);
			ref int2 reference2 = ref CollectionUtils.ElementAt<int2>(tmp, otherIndex);
			Assert.IsTrue(groundWater2.m_Polluted + reference2.y <= groundWater2.m_Amount + reference2.x);
			Assert.IsTrue(groundWater.m_Polluted + reference.y <= groundWater.m_Amount + reference.x);
			float num = ((groundWater.m_Amount + reference.x != 0) ? (1f * (float)(groundWater.m_Polluted + reference.y) / (float)(groundWater.m_Amount + reference.x)) : 0f);
			float num2 = ((groundWater2.m_Amount + reference2.x != 0) ? (1f * (float)(groundWater2.m_Polluted + reference2.y) / (float)(groundWater2.m_Amount + reference2.x)) : 0f);
			int num3 = groundWater.m_Amount - groundWater.m_Max;
			int num4 = math.clamp((groundWater2.m_Amount - groundWater2.m_Max - num3) / 4, -groundWater.m_Amount / 4, groundWater2.m_Amount / 4);
			reference.x += num4;
			reference2.x -= num4;
			int num5 = 0;
			if (num4 > 0)
			{
				num5 = (int)((float)num4 * num2);
			}
			else if (num4 < 0)
			{
				num5 = (int)((float)num4 * num);
			}
			reference.y += num5;
			reference2.y -= num5;
			Assert.IsTrue(0 <= groundWater.m_Amount + reference.x);
			Assert.IsTrue(groundWater.m_Amount + reference.x <= groundWater.m_Max);
			Assert.IsTrue(0 <= groundWater2.m_Amount + reference2.x);
			Assert.IsTrue(groundWater2.m_Amount + reference2.x <= groundWater2.m_Max);
			Assert.IsTrue(0 <= groundWater.m_Polluted + reference.y);
			Assert.IsTrue(groundWater.m_Polluted + reference.y <= groundWater.m_Amount + reference.x);
			Assert.IsTrue(0 <= groundWater2.m_Polluted + reference2.y);
			Assert.IsTrue(groundWater2.m_Polluted + reference2.y <= groundWater2.m_Amount + reference2.x);
		}

		public void Execute()
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<int2> tmp = default(NativeArray<int2>);
			tmp._002Ector(m_GroundWaterMap.Length, (Allocator)2, (NativeArrayOptions)1);
			for (int i = 0; i < m_GroundWaterMap.Length; i++)
			{
				int num = i % kTextureSize;
				int num2 = i / kTextureSize;
				if (num < kTextureSize - 1)
				{
					HandlePollution(i, i + 1, tmp);
				}
				if (num2 < kTextureSize - 1)
				{
					HandlePollution(i, i + kTextureSize, tmp);
				}
			}
			for (int j = 0; j < m_GroundWaterMap.Length; j++)
			{
				int num3 = j % kTextureSize;
				int num4 = j / kTextureSize;
				if (num3 < kTextureSize - 1)
				{
					HandleFlow(j, j + 1, tmp);
				}
				if (num4 < kTextureSize - 1)
				{
					HandleFlow(j, j + kTextureSize, tmp);
				}
			}
			for (int k = 0; k < m_GroundWaterMap.Length; k++)
			{
				GroundWater groundWater = m_GroundWaterMap[k];
				groundWater.m_Amount = (short)math.min(groundWater.m_Amount + tmp[k].x + Mathf.CeilToInt(m_Parameters.m_GroundwaterReplenish * (float)groundWater.m_Max), (int)groundWater.m_Max);
				groundWater.m_Polluted = (short)math.clamp(groundWater.m_Polluted + tmp[k].y - m_Parameters.m_GroundwaterPurification, 0, (int)groundWater.m_Amount);
				m_GroundWaterMap[k] = groundWater;
			}
			tmp.Dispose();
		}
	}

	public const int kMaxGroundWater = 10000;

	public const int kMinGroundWaterThreshold = 500;

	public static readonly int kTextureSize = 256;

	private EntityQuery m_ParameterQuery;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 128;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 64;
	}

	public static float3 GetCellCenter(int index)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return CellMapSystem<GroundWater>.GetCellCenter(index, kTextureSize);
	}

	public static bool TryGetCell(float3 position, out int2 cell)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		cell = CellMapSystem<GroundWater>.GetCell(position, CellMapSystem<GroundWater>.kMapSize, kTextureSize);
		return IsValidCell(cell);
	}

	public static bool IsValidCell(int2 cell)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (cell.x >= 0 && cell.y >= 0 && cell.x < kTextureSize)
		{
			return cell.y < kTextureSize;
		}
		return false;
	}

	public static GroundWater GetGroundWater(float3 position, NativeArray<GroundWater> groundWaterMap)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		float2 val = CellMapSystem<GroundWater>.GetCellCoords(position, CellMapSystem<GroundWater>.kMapSize, kTextureSize) - new float2(0.5f, 0.5f);
		int2 val2 = default(int2);
		((int2)(ref val2))._002Ector(Mathf.FloorToInt(val.x), Mathf.FloorToInt(val.y));
		int2 cell = default(int2);
		((int2)(ref cell))._002Ector(val2.x + 1, val2.y);
		int2 cell2 = default(int2);
		((int2)(ref cell2))._002Ector(val2.x, val2.y + 1);
		int2 cell3 = default(int2);
		((int2)(ref cell3))._002Ector(val2.x + 1, val2.y + 1);
		GroundWater groundWater = GetGroundWater(groundWaterMap, val2);
		GroundWater groundWater2 = GetGroundWater(groundWaterMap, cell);
		GroundWater groundWater3 = GetGroundWater(groundWaterMap, cell2);
		GroundWater groundWater4 = GetGroundWater(groundWaterMap, cell3);
		float sx = val.x - (float)val2.x;
		float sy = val.y - (float)val2.y;
		return new GroundWater
		{
			m_Amount = (short)math.round(Bilinear(groundWater.m_Amount, groundWater2.m_Amount, groundWater3.m_Amount, groundWater4.m_Amount, sx, sy)),
			m_Polluted = (short)math.round(Bilinear(groundWater.m_Polluted, groundWater2.m_Polluted, groundWater3.m_Polluted, groundWater4.m_Polluted, sx, sy)),
			m_Max = (short)math.round(Bilinear(groundWater.m_Max, groundWater2.m_Max, groundWater3.m_Max, groundWater4.m_Max, sx, sy))
		};
	}

	public static void ConsumeGroundWater(float3 position, NativeArray<GroundWater> groundWaterMap, int amount)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		Assert.IsTrue(amount >= 0);
		float2 val = CellMapSystem<GroundWater>.GetCellCoords(position, CellMapSystem<GroundWater>.kMapSize, kTextureSize) - new float2(0.5f, 0.5f);
		int2 val2 = default(int2);
		((int2)(ref val2))._002Ector(Mathf.FloorToInt(val.x), Mathf.FloorToInt(val.y));
		int2 cell = default(int2);
		((int2)(ref cell))._002Ector(val2.x + 1, val2.y);
		int2 cell2 = default(int2);
		((int2)(ref cell2))._002Ector(val2.x, val2.y + 1);
		int2 cell3 = default(int2);
		((int2)(ref cell3))._002Ector(val2.x + 1, val2.y + 1);
		GroundWater gw = GetGroundWater(groundWaterMap, val2);
		GroundWater gw2 = GetGroundWater(groundWaterMap, cell);
		GroundWater gw3 = GetGroundWater(groundWaterMap, cell2);
		GroundWater gw4 = GetGroundWater(groundWaterMap, cell3);
		float sx = val.x - (float)val2.x;
		float sy = val.y - (float)val2.y;
		float num = math.ceil(Bilinear(gw.m_Amount, 0, 0, 0, sx, sy));
		float num2 = math.ceil(Bilinear(0, gw2.m_Amount, 0, 0, sx, sy));
		float num3 = math.ceil(Bilinear(0, 0, gw3.m_Amount, 0, sx, sy));
		float num4 = math.ceil(Bilinear(0, 0, 0, gw4.m_Amount, sx, sy));
		float totalAvailable = num + num2 + num3 + num4;
		float totalConsumed = math.min((float)amount, totalAvailable);
		if (totalAvailable < (float)amount)
		{
			Debug.LogWarning((object)$"Trying to consume more groundwater than available! amount: {amount}, available: {totalAvailable}");
		}
		ConsumeFraction(ref gw, num);
		ConsumeFraction(ref gw2, num2);
		ConsumeFraction(ref gw3, num3);
		ConsumeFraction(ref gw4, num4);
		Assert.IsTrue(Mathf.Approximately(totalAvailable, 0f));
		Assert.IsTrue(Mathf.Approximately(totalConsumed, 0f));
		SetGroundWater(groundWaterMap, val2, gw);
		SetGroundWater(groundWaterMap, cell, gw2);
		SetGroundWater(groundWaterMap, cell2, gw3);
		SetGroundWater(groundWaterMap, cell3, gw4);
		void ConsumeFraction(ref GroundWater reference, float cellAvailable)
		{
			if (!(totalAvailable < 0.5f))
			{
				float num5 = cellAvailable / totalAvailable;
				totalAvailable -= cellAvailable;
				float num6 = math.max(0f, totalConsumed - totalAvailable);
				float num7 = math.max(math.round(num5 * totalConsumed), num6);
				Assert.IsTrue(num7 <= (float)reference.m_Amount);
				reference.Consume((int)num7);
				totalConsumed -= num7;
			}
		}
	}

	private static GroundWater GetGroundWater(NativeArray<GroundWater> groundWaterMap, int2 cell)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (!IsValidCell(cell))
		{
			return default(GroundWater);
		}
		return groundWaterMap[cell.x + kTextureSize * cell.y];
	}

	private static void SetGroundWater(NativeArray<GroundWater> groundWaterMap, int2 cell, GroundWater gw)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (IsValidCell(cell))
		{
			groundWaterMap[cell.x + kTextureSize * cell.y] = gw;
		}
	}

	private static float Bilinear(short v00, short v10, short v01, short v11, float sx, float sy)
	{
		return math.lerp(math.lerp((float)v00, (float)v10, sx), math.lerp((float)v01, (float)v11, sx), sy);
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		CreateTextures(kTextureSize);
		m_ParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<WaterPipeParameterData>() });
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		GroundWaterTickJob groundWaterTickJob = new GroundWaterTickJob
		{
			m_GroundWaterMap = m_Map,
			m_Parameters = ((EntityQuery)(ref m_ParameterQuery)).GetSingleton<WaterPipeParameterData>()
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<GroundWaterTickJob>(groundWaterTickJob, JobHandle.CombineDependencies(m_WriteDependencies, m_ReadDependencies, ((SystemBase)this).Dependency));
		AddWriter(((SystemBase)this).Dependency);
		((SystemBase)this).Dependency = JobHandle.CombineDependencies(m_ReadDependencies, m_WriteDependencies, ((SystemBase)this).Dependency);
	}

	public override JobHandle SetDefaults(Context context)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		if ((int)((Context)(ref context)).purpose == 1 && ((Context)(ref context)).version < Version.timoSerializationFlow)
		{
			for (int i = 0; i < m_Map.Length; i++)
			{
				float num = (float)(i % kTextureSize) / (float)kTextureSize;
				float num2 = (float)(i / kTextureSize) / (float)kTextureSize;
				short num3 = (short)Mathf.RoundToInt(10000f * math.saturate((Mathf.PerlinNoise(32f * num, 32f * num2) - 0.6f) / 0.4f));
				GroundWater groundWater = new GroundWater
				{
					m_Amount = num3,
					m_Max = num3
				};
				m_Map[i] = groundWater;
			}
			return default(JobHandle);
		}
		return base.SetDefaults(context);
	}

	[Preserve]
	public GroundWaterSystem()
	{
	}
}
