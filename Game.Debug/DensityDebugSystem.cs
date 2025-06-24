using System.Runtime.CompilerServices;
using Colossal;
using Colossal.Mathematics;
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
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Debug;

[CompilerGenerated]
public class DensityDebugSystem : BaseDebugSystem
{
	[BurstCompile]
	private struct DensityGizmoJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<EdgeGeometry> m_EdgeGeometryType;

		[ReadOnly]
		public ComponentTypeHandle<Density> m_DensityType;

		public GizmoBatcher m_GizmoBatcher;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<EdgeGeometry> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<EdgeGeometry>(ref m_EdgeGeometryType);
			NativeArray<Density> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Density>(ref m_DensityType);
			Segment val3 = default(Segment);
			Segment val5 = default(Segment);
			Segment val7 = default(Segment);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Density density = nativeArray2[i];
				EdgeGeometry edgeGeometry = nativeArray[i];
				Color val = Color.Lerp(Color.red, Color.green, math.saturate(5f * density.m_Density));
				int num = (int)math.ceil(edgeGeometry.m_Start.middleLength * 0.5f);
				int num2 = (int)math.ceil(edgeGeometry.m_End.middleLength * 0.5f);
				float3 val2 = math.lerp(edgeGeometry.m_Start.m_Left.a, edgeGeometry.m_Start.m_Right.a, 0.5f);
				((Segment)(ref val3))._002Ector(val2, val2 + new float3(0f, density.m_Density * 100f, 0f));
				((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val3.a, val3.b, val);
				for (int j = 1; j <= num; j++)
				{
					float2 val4 = (float)j / new float2((float)num, (float)(num + num2));
					val2 = math.lerp(MathUtils.Position(edgeGeometry.m_Start.m_Left, val4.x), MathUtils.Position(edgeGeometry.m_Start.m_Right, val4.x), 0.5f);
					((Segment)(ref val5))._002Ector(val2, val2 + new float3(0f, density.m_Density * 100f, 0f));
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val5.a, val5.b, val);
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val3.b, val5.b, val);
					val3 = val5;
				}
				for (int k = 1; k <= num2; k++)
				{
					float2 val6 = new float2((float)k, (float)(num + k)) / new float2((float)num2, (float)(num + num2));
					val2 = math.lerp(MathUtils.Position(edgeGeometry.m_End.m_Left, val6.x), MathUtils.Position(edgeGeometry.m_End.m_Right, val6.x), 0.5f);
					((Segment)(ref val7))._002Ector(val2, val2 + new float3(0f, density.m_Density * 100f, 0f));
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val7.a, val7.b, val);
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val3.b, val7.b, val);
					val3 = val7;
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
		public ComponentTypeHandle<EdgeGeometry> __Game_Net_EdgeGeometry_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Density> __Game_Net_Density_RO_ComponentTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			__Game_Net_EdgeGeometry_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EdgeGeometry>(true);
			__Game_Net_Density_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Density>(true);
		}
	}

	private EntityQuery m_EdgeGroup;

	private GizmosSystem m_GizmosSystem;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_GizmosSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GizmosSystem>();
		m_EdgeGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Density>(),
			ComponentType.ReadOnly<EdgeGeometry>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		((ComponentSystemBase)this).Enabled = false;
		((ComponentSystemBase)this).RequireForUpdate(m_EdgeGroup);
	}

	[Preserve]
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		inputDeps = JobChunkExtensions.ScheduleParallel<DensityGizmoJob>(new DensityGizmoJob
		{
			m_EdgeGeometryType = InternalCompilerInterface.GetComponentTypeHandle<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DensityType = InternalCompilerInterface.GetComponentTypeHandle<Density>(ref __TypeHandle.__Game_Net_Density_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_GizmoBatcher = m_GizmosSystem.GetGizmosBatcher(ref val)
		}, m_EdgeGroup, JobHandle.CombineDependencies(inputDeps, val));
		m_GizmosSystem.AddGizmosBatcherWriter(inputDeps);
		return inputDeps;
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
	public DensityDebugSystem()
	{
	}
}
