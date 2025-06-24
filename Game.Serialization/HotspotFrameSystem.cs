using System.Runtime.CompilerServices;
using Game.Events;
using Game.Rendering;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine.Scripting;

namespace Game.Serialization;

[CompilerGenerated]
public class HotspotFrameSystem : GameSystemBase
{
	[BurstCompile]
	private struct HotspotFrameJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<WeatherPhenomenon> m_WeatherPhenomenonType;

		public ComponentTypeHandle<InterpolatedTransform> m_InterpolatedTransformType;

		public BufferTypeHandle<HotspotFrame> m_HotspotFrameType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<WeatherPhenomenon> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WeatherPhenomenon>(ref m_WeatherPhenomenonType);
			NativeArray<InterpolatedTransform> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<InterpolatedTransform>(ref m_InterpolatedTransformType);
			BufferAccessor<HotspotFrame> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<HotspotFrame>(ref m_HotspotFrameType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				WeatherPhenomenon weatherPhenomenon = nativeArray[i];
				nativeArray2[i] = new InterpolatedTransform(weatherPhenomenon);
				DynamicBuffer<HotspotFrame> val = bufferAccessor[i];
				val.ResizeUninitialized(4);
				for (int j = 0; j < val.Length; j++)
				{
					val[j] = new HotspotFrame(weatherPhenomenon);
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
		public ComponentTypeHandle<WeatherPhenomenon> __Game_Events_WeatherPhenomenon_RO_ComponentTypeHandle;

		public ComponentTypeHandle<InterpolatedTransform> __Game_Rendering_InterpolatedTransform_RW_ComponentTypeHandle;

		public BufferTypeHandle<HotspotFrame> __Game_Events_HotspotFrame_RW_BufferTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			__Game_Events_WeatherPhenomenon_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WeatherPhenomenon>(true);
			__Game_Rendering_InterpolatedTransform_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InterpolatedTransform>(false);
			__Game_Events_HotspotFrame_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<HotspotFrame>(false);
		}
	}

	private EntityQuery m_Query;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_Query = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<HotspotFrame>(),
			ComponentType.ReadWrite<InterpolatedTransform>(),
			ComponentType.ReadOnly<WeatherPhenomenon>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_Query);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		HotspotFrameJob hotspotFrameJob = new HotspotFrameJob
		{
			m_WeatherPhenomenonType = InternalCompilerInterface.GetComponentTypeHandle<WeatherPhenomenon>(ref __TypeHandle.__Game_Events_WeatherPhenomenon_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InterpolatedTransformType = InternalCompilerInterface.GetComponentTypeHandle<InterpolatedTransform>(ref __TypeHandle.__Game_Rendering_InterpolatedTransform_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HotspotFrameType = InternalCompilerInterface.GetBufferTypeHandle<HotspotFrame>(ref __TypeHandle.__Game_Events_HotspotFrame_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef)
		};
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<HotspotFrameJob>(hotspotFrameJob, m_Query, ((SystemBase)this).Dependency);
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
	public HotspotFrameSystem()
	{
	}
}
