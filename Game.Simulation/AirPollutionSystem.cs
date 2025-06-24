using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Prefabs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class AirPollutionSystem : CellMapSystem<AirPollution>, IJobSerializable
{
	[BurstCompile]
	private struct AirPollutionMoveJob : IJob
	{
		public NativeArray<AirPollution> m_PollutionMap;

		[ReadOnly]
		public NativeArray<Wind> m_WindMap;

		public PollutionParameterData m_PollutionParameters;

		public RandomSeed m_Random;

		public uint m_Frame;

		public void Execute()
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<AirPollution> val = default(NativeArray<AirPollution>);
			val._002Ector(m_PollutionMap.Length, (Allocator)2, (NativeArrayOptions)1);
			Random random = m_Random.GetRandom((int)m_Frame);
			for (int i = 0; i < m_PollutionMap.Length; i++)
			{
				float3 cellCenter = GetCellCenter(i);
				Wind wind = WindSystem.GetWind(cellCenter, m_WindMap);
				short pollution = GetPollution(cellCenter - m_PollutionParameters.m_WindAdvectionSpeed * new float3(wind.m_Wind.x, 0f, wind.m_Wind.y), m_PollutionMap).m_Pollution;
				val[i] = new AirPollution
				{
					m_Pollution = pollution
				};
			}
			float num = (float)m_PollutionParameters.m_AirFade / (float)kUpdatesPerDay;
			for (int j = 0; j < kTextureSize; j++)
			{
				for (int k = 0; k < kTextureSize; k++)
				{
					int num2 = j * kTextureSize + k;
					int pollution2 = val[num2].m_Pollution;
					pollution2 += ((k > 0) ? (val[num2 - 1].m_Pollution >> kSpread) : 0);
					pollution2 += ((k < kTextureSize - 1) ? (val[num2 + 1].m_Pollution >> kSpread) : 0);
					pollution2 += ((j > 0) ? (val[num2 - kTextureSize].m_Pollution >> kSpread) : 0);
					pollution2 += ((j < kTextureSize - 1) ? (val[num2 + kTextureSize].m_Pollution >> kSpread) : 0);
					pollution2 -= (val[num2].m_Pollution >> kSpread - 2) + MathUtils.RoundToIntRandom(ref random, num);
					pollution2 = math.clamp(pollution2, 0, 32767);
					m_PollutionMap[num2] = new AirPollution
					{
						m_Pollution = (short)pollution2
					};
				}
			}
			val.Dispose();
		}
	}

	private static readonly int kSpread = 3;

	public static readonly int kTextureSize = 256;

	public static readonly int kUpdatesPerDay = 128;

	private WindSystem m_WindSystem;

	private SimulationSystem m_SimulationSystem;

	private EntityQuery m_PollutionParameterQuery;

	public int2 TextureSize => new int2(kTextureSize, kTextureSize);

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / kUpdatesPerDay;
	}

	public static float3 GetCellCenter(int index)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return CellMapSystem<AirPollution>.GetCellCenter(index, kTextureSize);
	}

	public static AirPollution GetPollution(float3 position, NativeArray<AirPollution> pollutionMap)
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
		AirPollution result = default(AirPollution);
		float num = (float)CellMapSystem<AirPollution>.kMapSize / (float)kTextureSize;
		int2 cell = CellMapSystem<AirPollution>.GetCell(position - new float3(num / 2f, 0f, num / 2f), CellMapSystem<AirPollution>.kMapSize, kTextureSize);
		float2 val = CellMapSystem<AirPollution>.GetCellCoords(position, CellMapSystem<AirPollution>.kMapSize, kTextureSize) - new float2(0.5f, 0.5f);
		cell = math.clamp(cell, int2.op_Implicit(0), int2.op_Implicit(kTextureSize - 2));
		short pollution = pollutionMap[cell.x + kTextureSize * cell.y].m_Pollution;
		short pollution2 = pollutionMap[cell.x + 1 + kTextureSize * cell.y].m_Pollution;
		short pollution3 = pollutionMap[cell.x + kTextureSize * (cell.y + 1)].m_Pollution;
		short pollution4 = pollutionMap[cell.x + 1 + kTextureSize * (cell.y + 1)].m_Pollution;
		result.m_Pollution = (short)math.round(math.lerp(math.lerp((float)pollution, (float)pollution2, val.x - (float)cell.x), math.lerp((float)pollution3, (float)pollution4, val.x - (float)cell.x), val.y - (float)cell.y));
		return result;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		CreateTextures(kTextureSize);
		m_WindSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WindSystem>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_PollutionParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PollutionParameterData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_PollutionParameterQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		AirPollutionMoveJob airPollutionMoveJob = new AirPollutionMoveJob
		{
			m_PollutionMap = m_Map,
			m_WindMap = m_WindSystem.GetMap(readOnly: true, out dependencies),
			m_PollutionParameters = ((EntityQuery)(ref m_PollutionParameterQuery)).GetSingleton<PollutionParameterData>(),
			m_Random = RandomSeed.Next(),
			m_Frame = m_SimulationSystem.frameIndex
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<AirPollutionMoveJob>(airPollutionMoveJob, JobUtils.CombineDependencies(dependencies, m_WriteDependencies, m_ReadDependencies, ((SystemBase)this).Dependency));
		m_WindSystem.AddReader(((SystemBase)this).Dependency);
		AddWriter(((SystemBase)this).Dependency);
		((SystemBase)this).Dependency = JobHandle.CombineDependencies(m_ReadDependencies, m_WriteDependencies, ((SystemBase)this).Dependency);
	}

	[Preserve]
	public AirPollutionSystem()
	{
	}
}
