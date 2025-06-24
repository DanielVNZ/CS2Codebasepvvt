using Colossal;
using Game.Simulation;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Debug;

public class SoilWaterDebugSystem : GameSystemBase
{
	private struct SoilWaterGizmoJob : IJob
	{
		[ReadOnly]
		public NativeArray<SoilWater> m_SoilWaterMap;

		public GizmoBatcher m_GizmoBatcher;

		public void Execute()
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 64; i < 128; i++)
			{
				for (int j = 64; j < 128; j++)
				{
					int num = i + j * 128;
					SoilWater soilWater = m_SoilWaterMap[num];
					if (soilWater.m_Max > 0)
					{
						float3 cellCenter = SoilWaterSystem.GetCellCenter(num);
						float3 val = cellCenter;
						cellCenter.y += (float)soilWater.m_Max / 400f;
						val.y += (float)soilWater.m_Amount / 400f;
						Color blue = Color.blue;
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCube(cellCenter, new float3(10f, (float)soilWater.m_Max / 200f, 10f), Color.grey);
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCube(val, new float3(10f, (float)soilWater.m_Amount / 200f, 10f), blue);
					}
				}
			}
		}
	}

	private SoilWaterSystem m_SoilWaterSystem;

	private GizmosSystem m_GizmosSystem;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_GizmosSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GizmosSystem>();
		m_SoilWaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SoilWaterSystem>();
		((ComponentSystemBase)this).Enabled = false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		JobHandle val = default(JobHandle);
		SoilWaterGizmoJob soilWaterGizmoJob = new SoilWaterGizmoJob
		{
			m_SoilWaterMap = m_SoilWaterSystem.GetMap(readOnly: true, out dependencies),
			m_GizmoBatcher = m_GizmosSystem.GetGizmosBatcher(ref val)
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<SoilWaterGizmoJob>(soilWaterGizmoJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val, dependencies));
		m_SoilWaterSystem.AddReader(((SystemBase)this).Dependency);
		m_GizmosSystem.AddGizmosBatcherWriter(((SystemBase)this).Dependency);
	}

	[Preserve]
	public SoilWaterDebugSystem()
	{
	}
}
