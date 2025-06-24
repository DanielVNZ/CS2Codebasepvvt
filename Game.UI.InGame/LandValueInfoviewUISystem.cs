using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.Common;
using Game.Net;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class LandValueInfoviewUISystem : InfoviewUISystemBase
{
	[BurstCompile]
	private struct CalculateAverageLandValueJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<Building> m_BuildingTypeHandle;

		[ReadOnly]
		public ComponentLookup<LandValue> m_LandValues;

		public NativeArray<float> m_Results;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Building> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Building>(ref m_BuildingTypeHandle);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Building building = nativeArray[i];
				if (m_LandValues.HasComponent(building.m_RoadEdge))
				{
					LandValue landValue = m_LandValues[building.m_RoadEdge];
					ref NativeArray<float> reference = ref m_Results;
					reference[0] = reference[0] + landValue.m_LandValue;
					reference = ref m_Results;
					reference[1] = reference[1] + 1f;
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<Building> __Game_Buildings_Building_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<LandValue> __Game_Net_LandValue_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			__Game_Buildings_Building_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Building>(true);
			__Game_Net_LandValue_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LandValue>(true);
		}
	}

	private const string kGroup = "landValueInfo";

	private ValueBinding<float> m_AverageLandValue;

	private EntityQuery m_LandValueQuery;

	private NativeArray<float> m_Results;

	private TypeHandle __TypeHandle;

	protected override bool Active
	{
		get
		{
			if (!base.Active)
			{
				return ((EventBindingBase)m_AverageLandValue).active;
			}
			return true;
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_LandValueQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<BuildingCondition>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		AddBinding((IBinding)(object)(m_AverageLandValue = new ValueBinding<float>("landValueInfo", "averageLandValue", 0f, (IWriter<float>)null, (EqualityComparer<float>)null)));
		m_Results = new NativeArray<float>(2, (Allocator)4, (NativeArrayOptions)1);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		base.OnDestroy();
		m_Results.Dispose();
	}

	protected override void PerformUpdate()
	{
		UpdateLandValue();
	}

	private void UpdateLandValue()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_Results.Length; i++)
		{
			m_Results[i] = 0f;
		}
		JobHandle val = JobChunkExtensions.Schedule<CalculateAverageLandValueJob>(new CalculateAverageLandValueJob
		{
			m_BuildingTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LandValues = InternalCompilerInterface.GetComponentLookup<LandValue>(ref __TypeHandle.__Game_Net_LandValue_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Results = m_Results
		}, m_LandValueQuery, ((SystemBase)this).Dependency);
		((JobHandle)(ref val)).Complete();
		float num = m_Results[1];
		float num2 = ((num > 0f) ? (m_Results[0] / num) : 0f);
		m_AverageLandValue.Update(num2);
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
	public LandValueInfoviewUISystem()
	{
	}
}
