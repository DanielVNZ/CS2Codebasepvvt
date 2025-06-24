using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.Effects;
using Game.Objects;
using Game.Prefabs;
using Game.Rendering;
using Game.Simulation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Audio;

[CompilerGenerated]
public class WeatherAudioSystem : GameSystemBase
{
	[BurstCompile]
	private struct WeatherAudioJob : IJob
	{
		public ComponentLookup<EffectInstance> m_EffectInstances;

		public SourceUpdateData m_SourceUpdateData;

		[ReadOnly]
		public int2 m_WaterTextureSize;

		[ReadOnly]
		public float3 m_CameraPosition;

		[ReadOnly]
		public int m_WaterAudioNearDistance;

		[ReadOnly]
		public Entity m_WaterAudioEntity;

		[ReadOnly]
		public WeatherAudioData m_WeatherAudioData;

		[ReadOnly]
		public NativeArray<SurfaceWater> m_WaterDepths;

		[ReadOnly]
		public TerrainHeightData m_TerrainData;

		public void Execute()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			if (NearWater(m_CameraPosition, m_WaterTextureSize, m_WaterAudioNearDistance, ref m_WaterDepths))
			{
				EffectInstance effectInstance = m_EffectInstances[m_WaterAudioEntity];
				float num = TerrainUtils.SampleHeight(ref m_TerrainData, m_CameraPosition);
				float num2 = math.lerp(effectInstance.m_Intensity, m_WeatherAudioData.m_WaterAudioIntensity, m_WeatherAudioData.m_WaterFadeSpeed);
				effectInstance.m_Position = new float3(m_CameraPosition.x, num, m_CameraPosition.z);
				effectInstance.m_Rotation = quaternion.identity;
				effectInstance.m_Intensity = math.saturate(num2);
				m_EffectInstances[m_WaterAudioEntity] = effectInstance;
				m_SourceUpdateData.Add(m_WaterAudioEntity, new Transform
				{
					m_Position = m_CameraPosition,
					m_Rotation = quaternion.identity
				});
			}
			else if (m_EffectInstances.HasComponent(m_WaterAudioEntity))
			{
				EffectInstance effectInstance2 = m_EffectInstances[m_WaterAudioEntity];
				if (effectInstance2.m_Intensity <= 0.01f)
				{
					m_SourceUpdateData.Remove(m_WaterAudioEntity);
					return;
				}
				float num3 = math.lerp(effectInstance2.m_Intensity, 0f, m_WeatherAudioData.m_WaterFadeSpeed);
				effectInstance2.m_Intensity = math.saturate(num3);
				m_EffectInstances[m_WaterAudioEntity] = effectInstance2;
				m_SourceUpdateData.Add(m_WaterAudioEntity, new Transform
				{
					m_Position = m_CameraPosition,
					m_Rotation = quaternion.identity
				});
			}
		}

		private static bool NearWater(float3 position, int2 texSize, int distance, ref NativeArray<SurfaceWater> depthsCPU)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			float2 val = (float)WaterSystem.kMapSize / float2.op_Implicit(texSize);
			int2 cell = WaterSystem.GetCell(position - new float3(val.x / 2f, 0f, val.y / 2f), WaterSystem.kMapSize, texSize);
			int2 val2 = default(int2);
			for (int i = -distance; i <= distance; i++)
			{
				for (int j = -distance; j <= distance; j++)
				{
					val2.x = math.clamp(cell.x + i, 0, texSize.x - 2);
					val2.y = math.clamp(cell.y + j, 0, texSize.y - 2);
					if (depthsCPU[val2.x + 1 + texSize.x * val2.y].m_Depth > 0f)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	private struct TypeHandle
	{
		public ComponentLookup<EffectInstance> __Game_Effects_EffectInstance_RW_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			__Game_Effects_EffectInstance_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EffectInstance>(false);
		}
	}

	private AudioManager m_AudioManager;

	private TerrainSystem m_TerrainSystem;

	private WaterSystem m_WaterSystem;

	private CameraUpdateSystem m_CameraUpdateSystem;

	private EntityQuery m_WeatherAudioEntityQuery;

	private Entity m_SmallWaterAudioEntity;

	private int m_WaterAudioEnabledZoom;

	private int m_WaterAudioNearDistance;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_AudioManager = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AudioManager>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_CameraUpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CameraUpdateSystem>();
		m_WeatherAudioEntityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<WeatherAudioData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_WeatherAudioEntityQuery);
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		m_SmallWaterAudioEntity = Entity.Null;
	}

	private void Initialize()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		WeatherAudioData componentData = ((EntityManager)(ref entityManager)).GetComponentData<WeatherAudioData>(((EntityQuery)(ref m_WeatherAudioEntityQuery)).GetSingletonEntity());
		entityManager = ((ComponentSystemBase)this).EntityManager;
		Entity val = ((EntityManager)(ref entityManager)).CreateEntity();
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).AddComponentData<EffectInstance>(val, default(EffectInstance));
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).AddComponentData<PrefabRef>(val, new PrefabRef
		{
			m_Prefab = componentData.m_WaterAmbientAudio
		});
		m_SmallWaterAudioEntity = val;
		m_WaterAudioEnabledZoom = componentData.m_WaterAudioEnabledZoom;
		m_WaterAudioNearDistance = componentData.m_WaterAudioNearDistance;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		if (m_WaterSystem.Loaded && m_CameraUpdateSystem.activeViewer != null && m_CameraUpdateSystem.activeCameraController != null)
		{
			if (m_SmallWaterAudioEntity == Entity.Null)
			{
				Initialize();
			}
			IGameCameraController activeCameraController = m_CameraUpdateSystem.activeCameraController;
			float3 position = m_CameraUpdateSystem.activeViewer.position;
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<EffectInstance>(m_SmallWaterAudioEntity) && activeCameraController.zoom < (float)m_WaterAudioEnabledZoom)
			{
				WeatherAudioJob weatherAudioJob = new WeatherAudioJob
				{
					m_WaterTextureSize = m_WaterSystem.TextureSize,
					m_WaterAudioNearDistance = m_WaterAudioNearDistance,
					m_CameraPosition = position,
					m_WaterAudioEntity = m_SmallWaterAudioEntity
				};
				entityManager = ((ComponentSystemBase)this).EntityManager;
				weatherAudioJob.m_WeatherAudioData = ((EntityManager)(ref entityManager)).GetComponentData<WeatherAudioData>(((EntityQuery)(ref m_WeatherAudioEntityQuery)).GetSingletonEntity());
				weatherAudioJob.m_SourceUpdateData = m_AudioManager.GetSourceUpdateData(out var deps);
				weatherAudioJob.m_TerrainData = m_TerrainSystem.GetHeightData();
				weatherAudioJob.m_EffectInstances = InternalCompilerInterface.GetComponentLookup<EffectInstance>(ref __TypeHandle.__Game_Effects_EffectInstance_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
				weatherAudioJob.m_WaterDepths = m_WaterSystem.GetDepths(out var deps2);
				WeatherAudioJob weatherAudioJob2 = weatherAudioJob;
				((SystemBase)this).Dependency = IJobExtensions.Schedule<WeatherAudioJob>(weatherAudioJob2, JobHandle.CombineDependencies(deps, deps2, ((SystemBase)this).Dependency));
				m_TerrainSystem.AddCPUHeightReader(((SystemBase)this).Dependency);
				m_AudioManager.AddSourceUpdateWriter(((SystemBase)this).Dependency);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public WeatherAudioSystem()
	{
	}
}
