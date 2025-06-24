using Colossal.Serialization.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

public class TrafficAmbienceSystem : CellMapSystem<TrafficAmbienceCell>, IJobSerializable
{
	[BurstCompile]
	private struct TrafficAmbienceUpdateJob : IJobParallelFor
	{
		public NativeArray<TrafficAmbienceCell> m_TrafficMap;

		public void Execute(int index)
		{
			TrafficAmbienceCell trafficAmbienceCell = m_TrafficMap[index];
			m_TrafficMap[index] = new TrafficAmbienceCell
			{
				m_Traffic = trafficAmbienceCell.m_Accumulator,
				m_Accumulator = 0f
			};
		}
	}

	public static readonly int kTextureSize = 64;

	public static readonly int kUpdatesPerDay = 1024;

	public int2 TextureSize => new int2(kTextureSize, kTextureSize);

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / kUpdatesPerDay;
	}

	public static float3 GetCellCenter(int index)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return CellMapSystem<TrafficAmbienceCell>.GetCellCenter(index, kTextureSize);
	}

	public static TrafficAmbienceCell GetTrafficAmbience2(float3 position, NativeArray<TrafficAmbienceCell> trafficAmbienceMap, float maxPerCell)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		TrafficAmbienceCell result = default(TrafficAmbienceCell);
		int2 cell = CellMapSystem<TrafficAmbienceCell>.GetCell(position, CellMapSystem<TrafficAmbienceCell>.kMapSize, kTextureSize);
		float num = 0f;
		float num2 = 0f;
		for (int i = cell.x - 2; i <= cell.x + 2; i++)
		{
			for (int j = cell.y - 2; j <= cell.y + 2; j++)
			{
				if (i >= 0 && i < kTextureSize && j >= 0 && j < kTextureSize)
				{
					int num3 = i + kTextureSize * j;
					float num4 = math.max(1f, math.distancesq(GetCellCenter(num3), position));
					num += math.min(maxPerCell, trafficAmbienceMap[num3].m_Traffic) / num4;
					num2 += 1f / num4;
				}
			}
		}
		result.m_Traffic = num / num2;
		return result;
	}

	public static TrafficAmbienceCell GetTrafficAmbience(float3 position, NativeArray<TrafficAmbienceCell> trafficAmbienceMap)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		TrafficAmbienceCell result = default(TrafficAmbienceCell);
		int2 cell = CellMapSystem<TrafficAmbienceCell>.GetCell(position, CellMapSystem<TrafficAmbienceCell>.kMapSize, kTextureSize);
		if (cell.x < 0 || cell.x >= kTextureSize || cell.y < 0 || cell.y >= kTextureSize)
		{
			return new TrafficAmbienceCell
			{
				m_Accumulator = 0f,
				m_Traffic = 0f
			};
		}
		float2 cellCoords = CellMapSystem<TrafficAmbienceCell>.GetCellCoords(position, CellMapSystem<TrafficAmbienceCell>.kMapSize, kTextureSize);
		float traffic = trafficAmbienceMap[cell.x + kTextureSize * cell.y].m_Traffic;
		float num = ((cell.x < kTextureSize - 1) ? trafficAmbienceMap[cell.x + 1 + kTextureSize * cell.y].m_Traffic : 0f);
		float num2 = ((cell.y < kTextureSize - 1) ? trafficAmbienceMap[cell.x + kTextureSize * (cell.y + 1)].m_Traffic : 0f);
		float num3 = ((cell.x < kTextureSize - 1 && cell.y < kTextureSize - 1) ? trafficAmbienceMap[cell.x + 1 + kTextureSize * (cell.y + 1)].m_Traffic : 0f);
		result.m_Traffic = math.lerp(math.lerp(traffic, num, cellCoords.x - (float)cell.x), math.lerp(num2, num3, cellCoords.x - (float)cell.x), cellCoords.y - (float)cell.y);
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
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		TrafficAmbienceUpdateJob trafficAmbienceUpdateJob = new TrafficAmbienceUpdateJob
		{
			m_TrafficMap = m_Map
		};
		((SystemBase)this).Dependency = IJobParallelForExtensions.Schedule<TrafficAmbienceUpdateJob>(trafficAmbienceUpdateJob, kTextureSize * kTextureSize, kTextureSize, JobHandle.CombineDependencies(m_WriteDependencies, m_ReadDependencies, ((SystemBase)this).Dependency));
		AddWriter(((SystemBase)this).Dependency);
		((SystemBase)this).Dependency = JobHandle.CombineDependencies(m_ReadDependencies, m_WriteDependencies, ((SystemBase)this).Dependency);
	}

	[Preserve]
	public TrafficAmbienceSystem()
	{
	}
}
