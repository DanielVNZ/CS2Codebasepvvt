using Colossal.Serialization.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

public class NoisePollutionSystem : CellMapSystem<NoisePollution>, IJobSerializable
{
	[BurstCompile]
	private struct NoisePollutionSwapJob : IJobParallelFor
	{
		[NativeDisableParallelForRestriction]
		public NativeArray<NoisePollution> m_PollutionMap;

		public void Execute(int index)
		{
			NoisePollution noisePollution = m_PollutionMap[index];
			int num = index % kTextureSize;
			int num2 = index / kTextureSize;
			short num3 = (short)((num > 0) ? m_PollutionMap[index - 1].m_PollutionTemp : 0);
			short num4 = (short)((num < kTextureSize - 1) ? m_PollutionMap[index + 1].m_PollutionTemp : 0);
			short num5 = (short)((num2 > 0) ? m_PollutionMap[index - kTextureSize].m_PollutionTemp : 0);
			short num6 = (short)((num2 < kTextureSize - 1) ? m_PollutionMap[index + kTextureSize].m_PollutionTemp : 0);
			short num7 = (short)((num > 0 && num2 > 0) ? m_PollutionMap[index - 1 - kTextureSize].m_PollutionTemp : 0);
			short num8 = (short)((num < kTextureSize - 1 && num2 > 0) ? m_PollutionMap[index + 1 - kTextureSize].m_PollutionTemp : 0);
			short num9 = (short)((num > 0 && num2 < kTextureSize - 1) ? m_PollutionMap[index - 1 + kTextureSize].m_PollutionTemp : 0);
			short num10 = (short)((num < kTextureSize - 1 && num2 < kTextureSize - 1) ? m_PollutionMap[index + 1 + kTextureSize].m_PollutionTemp : 0);
			noisePollution.m_Pollution = (short)(noisePollution.m_PollutionTemp / 4 + (num3 + num4 + num5 + num6) / 8 + (num7 + num8 + num9 + num10) / 16);
			m_PollutionMap[index] = noisePollution;
		}
	}

	[BurstCompile]
	private struct NoisePollutionClearJob : IJobParallelFor
	{
		public NativeArray<NoisePollution> m_PollutionMap;

		public void Execute(int index)
		{
			NoisePollution noisePollution = m_PollutionMap[index];
			noisePollution.m_PollutionTemp = 0;
			m_PollutionMap[index] = noisePollution;
		}
	}

	public static readonly int kTextureSize = 256;

	public static readonly int kUpdatesPerDay = 128;

	public int2 TextureSize => new int2(kTextureSize, kTextureSize);

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / kUpdatesPerDay;
	}

	public static float3 GetCellCenter(int index)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return CellMapSystem<NoisePollution>.GetCellCenter(index, kTextureSize);
	}

	public static NoisePollution GetPollution(float3 position, NativeArray<NoisePollution> pollutionMap)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		NoisePollution result = default(NoisePollution);
		float num = (float)CellMapSystem<NoisePollution>.kMapSize / (float)kTextureSize;
		int2 cell = CellMapSystem<NoisePollution>.GetCell(position - new float3(num / 2f, 0f, num / 2f), CellMapSystem<NoisePollution>.kMapSize, kTextureSize);
		float2 val = CellMapSystem<NoisePollution>.GetCellCoords(position, CellMapSystem<NoisePollution>.kMapSize, kTextureSize) - new float2(0.5f, 0.5f);
		cell = math.clamp(cell, int2.op_Implicit(0), int2.op_Implicit(kTextureSize - 2));
		short pollution = pollutionMap[cell.x + kTextureSize * cell.y].m_Pollution;
		short pollution2 = pollutionMap[cell.x + 1 + kTextureSize * cell.y].m_Pollution;
		short pollution3 = pollutionMap[cell.x + kTextureSize * (cell.y + 1)].m_Pollution;
		short pollution4 = pollutionMap[cell.x + 1 + kTextureSize * (cell.y + 1)].m_Pollution;
		result.m_Pollution = (short)Mathf.RoundToInt(math.lerp(math.lerp((float)pollution, (float)pollution2, val.x - (float)cell.x), math.lerp((float)pollution3, (float)pollution4, val.x - (float)cell.x), val.y - (float)cell.y));
		return result;
	}

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		CreateTextures(kTextureSize);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		NoisePollutionSwapJob noisePollutionSwapJob = new NoisePollutionSwapJob
		{
			m_PollutionMap = GetMap(readOnly: false, out dependencies)
		};
		NoisePollutionClearJob obj = new NoisePollutionClearJob
		{
			m_PollutionMap = noisePollutionSwapJob.m_PollutionMap
		};
		dependencies = IJobParallelForExtensions.Schedule<NoisePollutionSwapJob>(noisePollutionSwapJob, m_Map.Length, 4, dependencies);
		dependencies = IJobParallelForExtensions.Schedule<NoisePollutionClearJob>(obj, m_Map.Length, 64, dependencies);
		AddWriter(dependencies);
	}

	[Preserve]
	public NoisePollutionSystem()
	{
	}
}
