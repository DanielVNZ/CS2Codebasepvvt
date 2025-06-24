using Colossal;
using Game.Simulation;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Debug;

public class PollutionDebugSystem : BaseDebugSystem
{
	private struct PollutionGizmoJob : IJob
	{
		[ReadOnly]
		public NativeArray<GroundPollution> m_PollutionMap;

		[ReadOnly]
		public NativeArray<AirPollution> m_AirPollutionMap;

		[ReadOnly]
		public NativeArray<NoisePollution> m_NoisePollutionMap;

		public GizmoBatcher m_GizmoBatcher;

		public bool m_GroundOption;

		public bool m_AirOption;

		public bool m_NoiseOption;

		public float m_BaseHeight;

		public void Execute()
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			float3 val = default(float3);
			((float3)(ref val))._002Ector(0f, m_BaseHeight, 0f);
			if (m_GroundOption)
			{
				for (int i = 0; i < m_PollutionMap.Length; i++)
				{
					GroundPollution groundPollution = m_PollutionMap[i];
					if (groundPollution.m_Pollution > 0)
					{
						float3 cellCenter = GroundPollutionSystem.GetCellCenter(i);
						cellCenter.y += (float)groundPollution.m_Pollution / 400f;
						Color val2 = ((groundPollution.m_Pollution >= 8000) ? Color.Lerp(Color.yellow, Color.red, math.saturate((float)(groundPollution.m_Pollution - 8000) / 8000f)) : Color.Lerp(Color.green, Color.yellow, (float)groundPollution.m_Pollution / 8000f));
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCube(cellCenter + val, new float3(10f, (float)groundPollution.m_Pollution / 200f, 10f), val2);
					}
				}
			}
			if (m_AirOption)
			{
				for (int j = 0; j < m_AirPollutionMap.Length; j++)
				{
					AirPollution airPollution = m_AirPollutionMap[j];
					if (airPollution.m_Pollution > 0)
					{
						float3 cellCenter2 = AirPollutionSystem.GetCellCenter(j);
						cellCenter2.y += 200f;
						Color val3 = ((airPollution.m_Pollution >= 8000) ? Color.Lerp(Color.yellow, Color.red, math.saturate((float)(airPollution.m_Pollution - 8000) / 8000f)) : Color.Lerp(Color.green, Color.yellow, (float)airPollution.m_Pollution / 8000f));
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCone(cellCenter2 + val, 10f, cellCenter2 + val + new float3(0f, (float)airPollution.m_Pollution / 50f, 0f), 10f, val3, 36);
					}
				}
			}
			if (!m_NoiseOption)
			{
				return;
			}
			for (int k = 0; k < m_NoisePollutionMap.Length; k++)
			{
				NoisePollution noisePollution = m_NoisePollutionMap[k];
				if (noisePollution.m_Pollution > 0)
				{
					float3 cellCenter3 = NoisePollutionSystem.GetCellCenter(k);
					cellCenter3.y += 50f + (float)noisePollution.m_Pollution / 400f;
					Color val4 = ((noisePollution.m_Pollution >= 8000) ? Color.Lerp(Color.yellow, Color.red, math.saturate((float)(noisePollution.m_Pollution - 8000) / 8000f)) : Color.Lerp(Color.green, Color.yellow, (float)noisePollution.m_Pollution / 8000f));
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCube(cellCenter3 + val, new float3(10f, (float)noisePollution.m_Pollution / 200f, 10f), val4);
				}
			}
		}
	}

	private GroundPollutionSystem m_GroundPollutionSystem;

	private AirPollutionSystem m_AirPollutionSystem;

	private NoisePollutionSystem m_NoisePollutionSystem;

	private ClimateSystem m_ClimateSystem;

	private GizmosSystem m_GizmosSystem;

	private Option m_GroundOption;

	private Option m_AirOption;

	private Option m_NoiseOption;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_GizmosSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GizmosSystem>();
		m_GroundPollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GroundPollutionSystem>();
		m_AirPollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AirPollutionSystem>();
		m_NoisePollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NoisePollutionSystem>();
		m_ClimateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ClimateSystem>();
		m_GroundOption = AddOption("Ground pollution", defaultEnabled: true);
		m_AirOption = AddOption("Air pollution", defaultEnabled: true);
		m_NoiseOption = AddOption("Noise pollution", defaultEnabled: true);
		((ComponentSystemBase)this).Enabled = false;
	}

	[Preserve]
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		JobHandle dependencies2;
		JobHandle dependencies3;
		JobHandle val2 = default(JobHandle);
		JobHandle val = IJobExtensions.Schedule<PollutionGizmoJob>(new PollutionGizmoJob
		{
			m_PollutionMap = m_GroundPollutionSystem.GetMap(readOnly: true, out dependencies),
			m_AirPollutionMap = m_AirPollutionSystem.GetMap(readOnly: true, out dependencies2),
			m_NoisePollutionMap = m_NoisePollutionSystem.GetMap(readOnly: true, out dependencies3),
			m_GizmoBatcher = m_GizmosSystem.GetGizmosBatcher(ref val2),
			m_AirOption = m_AirOption.enabled,
			m_GroundOption = m_GroundOption.enabled,
			m_NoiseOption = m_NoiseOption.enabled,
			m_BaseHeight = m_ClimateSystem.temperatureBaseHeight
		}, JobHandle.CombineDependencies(dependencies2, dependencies3, JobHandle.CombineDependencies(inputDeps, val2, dependencies)));
		m_GizmosSystem.AddGizmosBatcherWriter(val);
		m_GroundPollutionSystem.AddReader(val);
		m_AirPollutionSystem.AddReader(val);
		m_NoisePollutionSystem.AddReader(val);
		return val;
	}

	[Preserve]
	public PollutionDebugSystem()
	{
	}
}
