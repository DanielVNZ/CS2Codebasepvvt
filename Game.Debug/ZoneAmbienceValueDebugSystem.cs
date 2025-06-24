using System;
using System.Collections.Generic;
using Colossal;
using Game.Simulation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Debug;

public class ZoneAmbienceValueDebugSystem : BaseDebugSystem
{
	[BurstCompile]
	private struct ZoneAmbienceValueGizmoJob : IJob
	{
		public GroupAmbienceType m_Type;

		[ReadOnly]
		public NativeArray<ZoneAmbienceCell> m_AmbienceMap;

		[ReadOnly]
		public NativeArray<Color> m_DistinctColors;

		public GizmoBatcher m_GizmoBatcher;

		public void Execute()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			float3 val = default(float3);
			for (int i = 0; i < m_AmbienceMap.Length; i++)
			{
				ZoneAmbienceCell zoneAmbienceCell = m_AmbienceMap[i];
				float3 cellCenter = ZoneAmbienceSystem.GetCellCenter(i);
				cellCenter.y += 385.7151f;
				float ambience = zoneAmbienceCell.m_Value.GetAmbience(m_Type);
				float num = (int)m_Type * 5;
				((float3)(ref val))._002Ector(num, 0f, num);
				((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(cellCenter + val, cellCenter + new float3(0f, ambience, 0f) + val, m_DistinctColors[(int)m_Type]);
			}
		}
	}

	private ZoneAmbienceSystem m_ZoneAmbienceSystem;

	private GizmosSystem m_GizmosSystem;

	private Dictionary<GroupAmbienceType, Option> m_CoverageOptions;

	private NativeArray<Color> m_DistinctColors;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ZoneAmbienceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ZoneAmbienceSystem>();
		m_GizmosSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GizmosSystem>();
		m_CoverageOptions = new Dictionary<GroupAmbienceType, Option>();
		string[] names = Enum.GetNames(typeof(GroupAmbienceType));
		Array values = Enum.GetValues(typeof(GroupAmbienceType));
		for (int i = 0; i < names.Length; i++)
		{
			GroupAmbienceType groupAmbienceType = (GroupAmbienceType)values.GetValue(i);
			if (groupAmbienceType != GroupAmbienceType.Count)
			{
				m_CoverageOptions.Add(groupAmbienceType, AddOption(names[i], i == 0));
			}
		}
		m_DistinctColors = new NativeArray<Color>(22, (Allocator)4, (NativeArrayOptions)1);
		for (int j = 0; j < 22; j++)
		{
			float num = (float)j / 22f % 1f;
			m_DistinctColors[j] = Color.HSVToRGB(num, 1f, 1f);
		}
		((ComponentSystemBase)this).Enabled = false;
	}

	[Preserve]
	protected override void OnDestroy()
	{
		base.OnDestroy();
		m_DistinctColors.Dispose();
	}

	[Preserve]
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = inputDeps;
		JobHandle val3 = default(JobHandle);
		foreach (KeyValuePair<GroupAmbienceType, Option> coverageOption in m_CoverageOptions)
		{
			if (coverageOption.Value.enabled)
			{
				JobHandle dependencies;
				JobHandle val2 = IJobExtensions.Schedule<ZoneAmbienceValueGizmoJob>(new ZoneAmbienceValueGizmoJob
				{
					m_Type = coverageOption.Key,
					m_AmbienceMap = m_ZoneAmbienceSystem.GetMap(readOnly: true, out dependencies),
					m_DistinctColors = m_DistinctColors,
					m_GizmoBatcher = m_GizmosSystem.GetGizmosBatcher(ref val3)
				}, JobHandle.CombineDependencies(dependencies, val3, ((SystemBase)this).Dependency));
				m_GizmosSystem.AddGizmosBatcherWriter(val2);
				val = JobHandle.CombineDependencies(val, val2);
			}
		}
		return val;
	}

	[Preserve]
	public ZoneAmbienceValueDebugSystem()
	{
	}
}
