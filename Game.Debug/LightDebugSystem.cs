using Colossal;
using Game.Prefabs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Scripting;

namespace Game.Debug;

public class LightDebugSystem : BaseDebugSystem
{
	[BurstCompile]
	private struct LightGizmoJob : IJob
	{
		[ReadOnly]
		public bool m_SpotOption;

		[ReadOnly]
		public bool m_PositionOption;

		[DeallocateOnJobCompletion]
		[ReadOnly]
		public NativeArray<PunctualLightData> m_punctualLights;

		public GizmoBatcher m_GizmoBatcher;

		public void Execute()
		{
		}
	}

	private EntityQuery m_LightEffectPrefabQuery;

	private GizmosSystem m_GizmosSystem;

	private Option m_SpotOption;

	private Option m_PositionOption;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_GizmosSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GizmosSystem>();
		m_LightEffectPrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<LightEffectData>(),
			ComponentType.ReadOnly<PrefabData>()
		});
		m_PositionOption = AddOption("Show positions", defaultEnabled: false);
		m_SpotOption = AddOption("Spot Lights Cones", defaultEnabled: false);
		((ComponentSystemBase)this).Enabled = false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		JobHandle punctualLightsJobHandle = HDRPDotsInputs.punctualLightsJobHandle;
		((JobHandle)(ref punctualLightsJobHandle)).Complete();
		if (HDRPDotsInputs.s_punctualLightdata.Length != 0)
		{
			JobHandle val = default(JobHandle);
			LightGizmoJob lightGizmoJob = new LightGizmoJob
			{
				m_SpotOption = m_SpotOption.enabled,
				m_PositionOption = m_PositionOption.enabled,
				m_GizmoBatcher = m_GizmosSystem.GetGizmosBatcher(ref val)
			};
			lightGizmoJob.m_punctualLights = new NativeArray<PunctualLightData>(HDRPDotsInputs.s_punctualLightdata.AsArray(), (Allocator)4);
			JobHandle val2 = IJobExtensions.Schedule<LightGizmoJob>(lightGizmoJob, val);
			m_GizmosSystem.AddGizmosBatcherWriter(val2);
			((SystemBase)this).Dependency = val2;
		}
	}

	[Preserve]
	public LightDebugSystem()
	{
	}
}
