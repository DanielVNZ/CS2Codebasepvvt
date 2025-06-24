using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

public class GroundWaterPollutionSystem : GameSystemBase
{
	[BurstCompile]
	private struct PolluteGroundWaterJob : IJob
	{
		public NativeArray<GroundWater> m_GroundWaterMap;

		[ReadOnly]
		public NativeArray<GroundPollution> m_PollutionMap;

		public void Execute()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_GroundWaterMap.Length; i++)
			{
				GroundWater groundWater = m_GroundWaterMap[i];
				GroundPollution pollution = GroundPollutionSystem.GetPollution(GroundWaterSystem.GetCellCenter(i), m_PollutionMap);
				if (pollution.m_Pollution > 0)
				{
					groundWater.m_Polluted = (short)math.min((int)groundWater.m_Amount, groundWater.m_Polluted + pollution.m_Pollution / 200);
					m_GroundWaterMap[i] = groundWater;
				}
			}
		}
	}

	private GroundWaterSystem m_GroundWaterSystem;

	private GroundPollutionSystem m_GroundPollutionSystem;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 128;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 64;
	}

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_GroundWaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GroundWaterSystem>();
		m_GroundPollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GroundPollutionSystem>();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		JobHandle dependencies2;
		PolluteGroundWaterJob polluteGroundWaterJob = new PolluteGroundWaterJob
		{
			m_GroundWaterMap = m_GroundWaterSystem.GetMap(readOnly: false, out dependencies),
			m_PollutionMap = m_GroundPollutionSystem.GetMap(readOnly: true, out dependencies2)
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<PolluteGroundWaterJob>(polluteGroundWaterJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies, dependencies2));
		m_GroundWaterSystem.AddWriter(((SystemBase)this).Dependency);
		m_GroundPollutionSystem.AddReader(((SystemBase)this).Dependency);
	}

	[Preserve]
	public GroundWaterPollutionSystem()
	{
	}
}
