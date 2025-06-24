using System.Runtime.CompilerServices;
using Colossal;
using Colossal.Entities;
using Game.Objects;
using Game.Simulation;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Debug;

[CompilerGenerated]
public class WaterDebugSystem : BaseDebugSystem
{
	private struct WaterGizmoJob : IJob
	{
		[ReadOnly]
		public NativeList<Entity> m_WaterSources;

		[ReadOnly]
		public ComponentLookup<WaterSourceData> m_SourceDatas;

		[ReadOnly]
		public ComponentLookup<Transform> m_Transforms;

		[ReadOnly]
		public NativeArray<int> m_WaterActive;

		[ReadOnly]
		public NativeArray<SurfaceWater> m_WaterDepths;

		public GizmoBatcher m_GizmoBatcher;

		public float2 m_MapSize;

		public bool m_ShowCulling;

		public bool m_showSurface;

		public float m_GridCellInMeters;

		public float m_CellInMeters;

		public float3 m_PositionOffset;

		public void Execute()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			float3 val3 = default(float3);
			float3 val4 = default(float3);
			for (int i = 0; i < m_WaterSources.Length; i++)
			{
				Entity val = m_WaterSources[i];
				WaterSourceData waterSourceData = m_SourceDatas[val];
				Transform transform = m_Transforms[val];
				if (waterSourceData.m_ConstantDepth > 0)
				{
					Color val2 = ((waterSourceData.m_ConstantDepth == 1) ? Color.cyan : Color.yellow);
					((float3)(ref val3))._002Ector(transform.m_Position.x, waterSourceData.m_Amount / 2f, transform.m_Position.z);
					val3.y += m_PositionOffset.y;
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCylinder(val3, waterSourceData.m_Radius, waterSourceData.m_Amount, val2, 36);
				}
				else
				{
					((float3)(ref val4))._002Ector(transform.m_Position.x, 100f * waterSourceData.m_Amount / 2f, transform.m_Position.z);
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCylinder(val4, waterSourceData.m_Radius, 10f * waterSourceData.m_Amount, Color.red, 36);
				}
			}
			int2 val5 = default(int2);
			float3 val6 = default(float3);
			int2 val8 = default(int2);
			for (int j = 0; j < m_WaterActive.Length; j++)
			{
				((int2)(ref val5))._002Ector(Mathf.RoundToInt(m_MapSize.x / m_GridCellInMeters), Mathf.RoundToInt(m_MapSize.x / m_GridCellInMeters));
				int num = j % val5.x;
				int num2 = j / val5.x;
				((float3)(ref val6))._002Ector(((float)num + 0.5f) * m_GridCellInMeters, 200f, ((float)num2 + 0.5f) * m_GridCellInMeters);
				val6 += m_PositionOffset;
				Color val7 = ((m_WaterActive[j] > 0) ? Color.white : Color.red);
				if (m_ShowCulling)
				{
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCube(val6, new float3(m_GridCellInMeters, 400f, m_GridCellInMeters), val7);
				}
				if (!m_showSurface || m_WaterActive[j] <= 0)
				{
					continue;
				}
				val6.y = 200f;
				((int2)(ref val8))._002Ector(Mathf.RoundToInt(m_GridCellInMeters / m_CellInMeters), Mathf.RoundToInt(m_GridCellInMeters / m_CellInMeters));
				for (int k = 0; k < val8.x; k += 16)
				{
					for (int l = 0; l < val8.y; l += 16)
					{
						int num3 = num * val8.x + k;
						int num4 = num2 * val8.y + l;
						int num5 = num3 + num4 * Mathf.RoundToInt(m_MapSize.x / m_CellInMeters);
						if (num5 < m_WaterDepths.Length)
						{
							SurfaceWater surfaceWater = m_WaterDepths[num5];
							if (surfaceWater.m_Depth > 0f)
							{
								Color val9 = Color.Lerp(Color.blue, new Color(0.54f, 0.27f, 0.07f), surfaceWater.m_Polluted);
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCube(val6 + new float3((float)k * m_CellInMeters - 0.5f * m_GridCellInMeters, 0f, (float)l * m_CellInMeters - 0.5f * m_GridCellInMeters), new float3(m_CellInMeters, surfaceWater.m_Depth, m_CellInMeters), val9);
							}
						}
					}
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<WaterSourceData> __Game_Simulation_WaterSourceData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			__Game_Simulation_WaterSourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterSourceData>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
		}
	}

	private WaterSystem m_WaterSystem;

	private TerrainSystem m_TerrainSystem;

	private GizmosSystem m_GizmosSystem;

	private EntityQuery m_WaterSourceGroup;

	private Option m_ShowCulling;

	private Option m_showSurface;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_GizmosSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GizmosSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_showSurface = AddOption("Show Surface Boxes", defaultEnabled: true);
		m_ShowCulling = AddOption("Show Culling Boxes", defaultEnabled: false);
		m_WaterSourceGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<WaterSourceData>() });
		((ComponentSystemBase)this).Enabled = false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		JobHandle deps;
		JobHandle val2 = default(JobHandle);
		WaterGizmoJob waterGizmoJob = new WaterGizmoJob
		{
			m_WaterSources = ((EntityQuery)(ref m_WaterSourceGroup)).ToEntityListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val),
			m_SourceDatas = InternalCompilerInterface.GetComponentLookup<WaterSourceData>(ref __TypeHandle.__Game_Simulation_WaterSourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Transforms = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WaterActive = m_WaterSystem.GetActive(),
			m_WaterDepths = m_WaterSystem.GetDepths(out deps),
			m_GizmoBatcher = m_GizmosSystem.GetGizmosBatcher(ref val2),
			m_GridCellInMeters = (float)m_WaterSystem.GridSize * m_WaterSystem.CellSize,
			m_CellInMeters = m_WaterSystem.CellSize,
			m_MapSize = m_WaterSystem.MapSize,
			m_PositionOffset = m_TerrainSystem.positionOffset,
			m_ShowCulling = m_ShowCulling.enabled,
			m_showSurface = m_showSurface.enabled
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<WaterGizmoJob>(waterGizmoJob, JobUtils.CombineDependencies(((SystemBase)this).Dependency, deps, val2, val));
		waterGizmoJob.m_WaterSources.Dispose(((SystemBase)this).Dependency);
		m_WaterSystem.AddSurfaceReader(((SystemBase)this).Dependency);
		m_WaterSystem.AddActiveReader(((SystemBase)this).Dependency);
		m_GizmosSystem.AddGizmosBatcherWriter(((SystemBase)this).Dependency);
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
	public WaterDebugSystem()
	{
	}
}
