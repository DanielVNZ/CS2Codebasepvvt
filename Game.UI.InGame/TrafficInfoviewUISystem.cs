using System;
using System.Runtime.CompilerServices;
using Colossal.UI.Binding;
using Game.Common;
using Game.Net;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class TrafficInfoviewUISystem : InfoviewUISystemBase
{
	[BurstCompile]
	private struct UpdateFlowJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<Road> m_RoadHandle;

		public NativeArray<float> m_Results;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Road> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Road>(ref m_RoadHandle);
			ref NativeArray<float> reference;
			for (int i = 0; i < nativeArray.Length; i++)
			{
				float4 val = NetUtils.GetTrafficFlowSpeed(nativeArray[i]) * 100f;
				reference = ref m_Results;
				reference[0] = reference[0] + val.x;
				reference = ref m_Results;
				reference[1] = reference[1] + val.y;
				reference = ref m_Results;
				reference[2] = reference[2] + val.z;
				reference = ref m_Results;
				reference[3] = reference[3] + val.w;
			}
			reference = ref m_Results;
			reference[4] = reference[4] + (float)nativeArray.Length;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<Road> __Game_Net_Road_RO_ComponentTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			__Game_Net_Road_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Road>(true);
		}
	}

	private const string kGroup = "trafficInfo";

	private EntityQuery m_AggregateQuery;

	private RawValueBinding m_TrafficFlow;

	private NativeArray<float> m_Results;

	private float[] m_Flow;

	private TypeHandle __TypeHandle;

	protected override bool Active
	{
		get
		{
			if (!base.Active)
			{
				return ((EventBindingBase)m_TrafficFlow).active;
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
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_0084: Expected O, but got Unknown
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_AggregateQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<Aggregated>(),
			ComponentType.ReadOnly<Edge>(),
			ComponentType.ReadOnly<Road>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Native>()
		});
		RawValueBinding val = new RawValueBinding("trafficInfo", "trafficFlow", (Action<IJsonWriter>)UpdateTrafficFlowBinding);
		RawValueBinding binding = val;
		m_TrafficFlow = val;
		AddBinding((IBinding)(object)binding);
		m_Flow = new float[5];
		m_Results = new NativeArray<float>(5, (Allocator)4, (NativeArrayOptions)1);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_Results.Dispose();
		base.OnDestroy();
	}

	protected override void PerformUpdate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		Reset();
		JobHandle val = JobChunkExtensions.Schedule<UpdateFlowJob>(new UpdateFlowJob
		{
			m_RoadHandle = InternalCompilerInterface.GetComponentTypeHandle<Road>(ref __TypeHandle.__Game_Net_Road_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Results = m_Results
		}, m_AggregateQuery, ((SystemBase)this).Dependency);
		((JobHandle)(ref val)).Complete();
		m_TrafficFlow.Update();
	}

	private void Reset()
	{
		for (int i = 0; i < m_Results.Length; i++)
		{
			m_Results[i] = 0f;
		}
		for (int j = 0; j < m_Flow.Length; j++)
		{
			m_Flow[j] = 0f;
		}
	}

	private void UpdateTrafficFlowBinding(IJsonWriter writer)
	{
		int num = math.select((int)m_Results[4], 1, (int)m_Results[4] == 0);
		m_Flow[0] = m_Results[0] / (float)num;
		m_Flow[1] = m_Results[1] / (float)num;
		m_Flow[2] = m_Results[2] / (float)num;
		m_Flow[3] = m_Results[3] / (float)num;
		m_Flow[4] = m_Flow[0];
		JsonWriterExtensions.ArrayBegin(writer, m_Flow.Length);
		for (int i = 0; i < m_Flow.Length; i++)
		{
			writer.Write(m_Flow[i]);
		}
		writer.ArrayEnd();
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
	public TrafficInfoviewUISystem()
	{
	}
}
