using Colossal.Serialization.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

public class ZoneAmbienceSystem : CellMapSystem<ZoneAmbienceCell>, IJobSerializable
{
	[BurstCompile]
	private struct ZoneAmbienceUpdateJob : IJobParallelFor
	{
		public NativeArray<ZoneAmbienceCell> m_ZoneMap;

		public void Execute(int index)
		{
			ZoneAmbienceCell zoneAmbienceCell = m_ZoneMap[index];
			m_ZoneMap[index] = new ZoneAmbienceCell
			{
				m_Value = zoneAmbienceCell.m_Accumulator,
				m_Accumulator = default(ZoneAmbiences)
			};
		}
	}

	public static readonly int kTextureSize = 64;

	public static readonly int kUpdatesPerDay = 128;

	public int2 TextureSize => new int2(kTextureSize, kTextureSize);

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / kUpdatesPerDay;
	}

	public static float3 GetCellCenter(int index)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return CellMapSystem<ZoneAmbienceCell>.GetCellCenter(index, kTextureSize);
	}

	public static float GetZoneAmbienceNear(GroupAmbienceType type, float3 position, NativeArray<ZoneAmbienceCell> zoneAmbienceMap, float nearWeight, float maxPerCell)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		int2 cell = CellMapSystem<ZoneAmbienceCell>.GetCell(position, CellMapSystem<ZoneAmbienceCell>.kMapSize, kTextureSize);
		float num = 0f;
		float num2 = 0f;
		for (int i = cell.x - 2; i <= cell.x + 2; i++)
		{
			for (int j = cell.y - 2; j <= cell.y + 2; j++)
			{
				if (i >= 0 && i < kTextureSize && j >= 0 && j < kTextureSize)
				{
					int num3 = i + kTextureSize * j;
					float num4 = math.max(1f, math.pow(math.distance(GetCellCenter(num3), position) / 10f, 1f + nearWeight));
					num += math.min(maxPerCell, zoneAmbienceMap[num3].m_Value.GetAmbience(type)) / num4;
					num2 += 1f / num4;
				}
			}
		}
		return num / num2;
	}

	public static float GetZoneAmbience(GroupAmbienceType type, float3 position, NativeArray<ZoneAmbienceCell> zoneAmbienceMap, float maxPerCell)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		int2 cell = CellMapSystem<ZoneAmbienceCell>.GetCell(position, CellMapSystem<ZoneAmbienceCell>.kMapSize, kTextureSize);
		float num = 0f;
		float num2 = 0f;
		for (int i = cell.x - 2; i <= cell.x + 2; i++)
		{
			for (int j = cell.y - 2; j <= cell.y + 2; j++)
			{
				if (i >= 0 && i < kTextureSize && j >= 0 && j < kTextureSize)
				{
					int num3 = i + kTextureSize * j;
					float num4 = math.max(1f, math.distancesq(GetCellCenter(num3), position) / 10f);
					num += math.min(maxPerCell, zoneAmbienceMap[num3].m_Value.GetAmbience(type)) / num4;
					num2 += 1f / num4;
				}
			}
		}
		return num / num2;
	}

	public static ZoneAmbienceCell GetZoneAmbience(float3 position, NativeArray<ZoneAmbienceCell> zoneAmbienceMap)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		ZoneAmbienceCell result = default(ZoneAmbienceCell);
		int2 cell = CellMapSystem<ZoneAmbienceCell>.GetCell(position, CellMapSystem<ZoneAmbienceCell>.kMapSize, kTextureSize);
		ZoneAmbiences zoneAmbiences = default(ZoneAmbiences);
		float num = 0f;
		for (int i = cell.x - 2; i <= cell.x + 2; i++)
		{
			for (int j = cell.y - 2; j <= cell.y + 2; j++)
			{
				if (i >= 0 && i < kTextureSize && j >= 0 && j < kTextureSize)
				{
					int num2 = i + kTextureSize * j;
					float num3 = math.max(1f, math.distancesq(GetCellCenter(num2), position) / 10f);
					zoneAmbiences += zoneAmbienceMap[num2].m_Value / num3;
					num += 1f / num3;
				}
			}
		}
		result.m_Value = zoneAmbiences / num;
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
		ZoneAmbienceUpdateJob zoneAmbienceUpdateJob = new ZoneAmbienceUpdateJob
		{
			m_ZoneMap = m_Map
		};
		((SystemBase)this).Dependency = IJobParallelForExtensions.Schedule<ZoneAmbienceUpdateJob>(zoneAmbienceUpdateJob, kTextureSize * kTextureSize, kTextureSize, JobHandle.CombineDependencies(m_WriteDependencies, m_ReadDependencies, ((SystemBase)this).Dependency));
		AddWriter(((SystemBase)this).Dependency);
		((SystemBase)this).Dependency = JobHandle.CombineDependencies(m_ReadDependencies, m_WriteDependencies, ((SystemBase)this).Dependency);
	}

	[Preserve]
	public ZoneAmbienceSystem()
	{
	}
}
