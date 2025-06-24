using System.Runtime.CompilerServices;
using Colossal;
using Game.Prefabs;
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
public class AudioGroupingDebugSystem : GameSystemBase
{
	private struct AudioGroupingGizmoJob : IJob
	{
		public Entity m_SettingsEntity;

		[ReadOnly]
		public NativeArray<TrafficAmbienceCell> m_TrafficMap;

		[ReadOnly]
		public NativeArray<ZoneAmbienceCell> m_ZoneMap;

		[ReadOnly]
		public TerrainHeightData m_HeightData;

		[ReadOnly]
		public BufferLookup<AudioGroupingSettingsData> m_Settings;

		public GizmoBatcher m_GizmoBatcher;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<AudioGroupingSettingsData> val = m_Settings[m_SettingsEntity];
			for (int i = 0; i < m_TrafficMap.Length; i++)
			{
				TrafficAmbienceCell trafficAmbienceCell = m_TrafficMap[i];
				float num = math.saturate(val[14].m_Scale * trafficAmbienceCell.m_Traffic);
				if (trafficAmbienceCell.m_Traffic > 0f)
				{
					float3 val2 = TrafficAmbienceSystem.GetCellCenter(i) + new float3(-60f, 0f, 0f);
					float num2 = TerrainUtils.SampleHeight(ref m_HeightData, val2);
					val2.y += 100f * num / 2f + num2;
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCube(val2, new float3(30f, 100f * num, 30f), Color.white);
				}
			}
			Color val4 = default(Color);
			for (int j = 0; j < m_ZoneMap.Length; j++)
			{
				ZoneAmbienceCell zoneAmbienceCell = m_ZoneMap[j];
				for (int k = 0; k < 21; k++)
				{
					float num3 = math.saturate(val[k].m_Scale * zoneAmbienceCell.m_Value.GetAmbience(val[k].m_Type));
					if (num3 > 0f)
					{
						float3 val3 = ZoneAmbienceSystem.GetCellCenter(j) + new float3(15f * (float)(k % 5) - 40f, 0f, 15f * (float)(k / 5));
						float num4 = TerrainUtils.SampleHeight(ref m_HeightData, val3);
						val3.y += 100f * num3 / 2f + num4;
						switch (val[k].m_Type)
						{
						case GroupAmbienceType.ResidentialLow:
						case GroupAmbienceType.ResidentialMedium:
						case GroupAmbienceType.ResidentialHigh:
						case GroupAmbienceType.ResidentialMixed:
						case GroupAmbienceType.ResidentialLowRent:
							val4 = Color.green;
							break;
						case GroupAmbienceType.CommercialLow:
						case GroupAmbienceType.CommercialHigh:
							val4 = Color.blue;
							break;
						case GroupAmbienceType.Industrial:
							val4 = Color.yellow;
							break;
						case GroupAmbienceType.OfficeLow:
						case GroupAmbienceType.OfficeHigh:
							val4 = Color.cyan;
							break;
						case GroupAmbienceType.Forest:
							((Color)(ref val4))._002Ector(34f, 139f, 34f);
							break;
						default:
							val4 = Color.grey;
							break;
						}
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCube(val3, new float3(15f, 100f * num3, 15f), val4);
					}
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public BufferLookup<AudioGroupingSettingsData> __Game_Prefabs_AudioGroupingSettingsData_RO_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_AudioGroupingSettingsData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<AudioGroupingSettingsData>(true);
		}
	}

	private TrafficAmbienceSystem m_TrafficAmbienceSystem;

	private ZoneAmbienceSystem m_ZoneAmbienceSystem;

	private TerrainSystem m_TerrainSystem;

	private GizmosSystem m_GizmosSystem;

	private EntityQuery m_AudioGroupingConfigurationQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_GizmosSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GizmosSystem>();
		m_TrafficAmbienceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TrafficAmbienceSystem>();
		m_ZoneAmbienceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ZoneAmbienceSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		((ComponentSystemBase)this).Enabled = false;
		m_AudioGroupingConfigurationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<AudioGroupingSettingsData>() });
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		JobHandle dependencies2;
		JobHandle val = default(JobHandle);
		AudioGroupingGizmoJob audioGroupingGizmoJob = new AudioGroupingGizmoJob
		{
			m_SettingsEntity = ((EntityQuery)(ref m_AudioGroupingConfigurationQuery)).GetSingletonEntity(),
			m_Settings = InternalCompilerInterface.GetBufferLookup<AudioGroupingSettingsData>(ref __TypeHandle.__Game_Prefabs_AudioGroupingSettingsData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrafficMap = m_TrafficAmbienceSystem.GetMap(readOnly: true, out dependencies),
			m_ZoneMap = m_ZoneAmbienceSystem.GetMap(readOnly: true, out dependencies2),
			m_HeightData = m_TerrainSystem.GetHeightData(),
			m_GizmoBatcher = m_GizmosSystem.GetGizmosBatcher(ref val)
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<AudioGroupingGizmoJob>(audioGroupingGizmoJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val, JobHandle.CombineDependencies(dependencies2, dependencies)));
		m_TrafficAmbienceSystem.AddReader(((SystemBase)this).Dependency);
		m_ZoneAmbienceSystem.AddReader(((SystemBase)this).Dependency);
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
	public AudioGroupingDebugSystem()
	{
	}
}
