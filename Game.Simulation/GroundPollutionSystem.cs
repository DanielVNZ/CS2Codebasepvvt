using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Prefabs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class GroundPollutionSystem : CellMapSystem<GroundPollution>, IJobSerializable
{
	[BurstCompile]
	private struct PollutionFadeJob : IJob
	{
		public NativeArray<GroundPollution> m_PollutionMap;

		public PollutionParameterData m_PollutionParameters;

		public RandomSeed m_Random;

		public uint m_Frame;

		public void Execute()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			Random random = m_Random.GetRandom((int)m_Frame);
			for (int i = 0; i < m_PollutionMap.Length; i++)
			{
				GroundPollution groundPollution = m_PollutionMap[i];
				if (groundPollution.m_Pollution > 0)
				{
					groundPollution.m_Pollution = (short)math.max(0, m_PollutionMap[i].m_Pollution - MathUtils.RoundToIntRandom(ref random, (float)m_PollutionParameters.m_GroundFade / (float)kUpdatesPerDay));
				}
				m_PollutionMap[i] = groundPollution;
			}
		}
	}

	public static readonly int kTextureSize = 256;

	public static readonly int kUpdatesPerDay = 128;

	private SimulationSystem m_SimulationSystem;

	private EntityQuery m_PollutionParameterGroup;

	public int2 TextureSize => new int2(kTextureSize, kTextureSize);

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / kUpdatesPerDay;
	}

	public static float3 GetCellCenter(int index)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return CellMapSystem<GroundPollution>.GetCellCenter(index, kTextureSize);
	}

	public static GroundPollution GetPollution(float3 position, NativeArray<GroundPollution> pollutionMap)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		GroundPollution result = default(GroundPollution);
		int2 cell = CellMapSystem<GroundPollution>.GetCell(position, CellMapSystem<GroundPollution>.kMapSize, kTextureSize);
		float2 cellCoords = CellMapSystem<GroundPollution>.GetCellCoords(position, CellMapSystem<GroundPollution>.kMapSize, kTextureSize);
		if (cell.x < 0 || cell.x >= kTextureSize || cell.y < 0 || cell.y >= kTextureSize)
		{
			return result;
		}
		GroundPollution groundPollution = pollutionMap[cell.x + kTextureSize * cell.y];
		GroundPollution groundPollution2 = ((cell.x < kTextureSize - 1) ? pollutionMap[cell.x + 1 + kTextureSize * cell.y] : default(GroundPollution));
		GroundPollution groundPollution3 = ((cell.y < kTextureSize - 1) ? pollutionMap[cell.x + kTextureSize * (cell.y + 1)] : default(GroundPollution));
		GroundPollution groundPollution4 = ((cell.x < kTextureSize - 1 && cell.y < kTextureSize - 1) ? pollutionMap[cell.x + 1 + kTextureSize * (cell.y + 1)] : default(GroundPollution));
		result.m_Pollution = (short)Mathf.RoundToInt(math.lerp(math.lerp((float)groundPollution.m_Pollution, (float)groundPollution2.m_Pollution, cellCoords.x - (float)cell.x), math.lerp((float)groundPollution3.m_Pollution, (float)groundPollution4.m_Pollution, cellCoords.x - (float)cell.x), cellCoords.y - (float)cell.y));
		return result;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		CreateTextures(kTextureSize);
		m_PollutionParameterGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PollutionParameterData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_PollutionParameterGroup);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		PollutionFadeJob pollutionFadeJob = new PollutionFadeJob
		{
			m_PollutionMap = m_Map,
			m_PollutionParameters = ((EntityQuery)(ref m_PollutionParameterGroup)).GetSingleton<PollutionParameterData>(),
			m_Random = RandomSeed.Next(),
			m_Frame = m_SimulationSystem.frameIndex
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<PollutionFadeJob>(pollutionFadeJob, JobHandle.CombineDependencies(m_WriteDependencies, m_ReadDependencies, ((SystemBase)this).Dependency));
		AddWriter(((SystemBase)this).Dependency);
		((SystemBase)this).Dependency = JobHandle.CombineDependencies(m_ReadDependencies, m_WriteDependencies, ((SystemBase)this).Dependency);
	}

	[Preserve]
	public GroundPollutionSystem()
	{
	}
}
