using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Game.Common;
using Game.Events;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class EventInterpolateSystem : GameSystemBase
{
	[BurstCompile]
	private struct UpdateTransformDataJob : IJobChunk
	{
		[ReadOnly]
		public uint m_FrameIndex;

		[ReadOnly]
		public float m_FrameTime;

		[ReadOnly]
		public BufferTypeHandle<HotspotFrame> m_HotspotFrameType;

		public ComponentTypeHandle<InterpolatedTransform> m_InterpolatedTransformType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			uint num = 0u;
			uint num2 = m_FrameIndex - num - 32;
			uint num3 = num2 / 16 % 4;
			uint num4 = (num3 + 1) % 4;
			float num5 = ((float)(num2 % 16) + m_FrameTime) / 16f;
			NativeArray<InterpolatedTransform> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<InterpolatedTransform>(ref m_InterpolatedTransformType);
			BufferAccessor<HotspotFrame> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<HotspotFrame>(ref m_HotspotFrameType);
			Bezier4x3 val2 = default(Bezier4x3);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				InterpolatedTransform interpolatedTransform = nativeArray[i];
				DynamicBuffer<HotspotFrame> val = bufferAccessor[i];
				HotspotFrame hotspotFrame = val[(int)num3];
				HotspotFrame hotspotFrame2 = val[(int)num4];
				float num6 = 4f / 45f;
				((Bezier4x3)(ref val2))._002Ector(hotspotFrame.m_Position, hotspotFrame.m_Position + hotspotFrame.m_Velocity * num6, hotspotFrame2.m_Position - hotspotFrame2.m_Velocity * num6, hotspotFrame2.m_Position);
				interpolatedTransform.m_Position = MathUtils.Position(val2, num5);
				interpolatedTransform.m_Rotation = quaternion.identity;
				nativeArray[i] = interpolatedTransform;
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
		public BufferTypeHandle<HotspotFrame> __Game_Events_HotspotFrame_RO_BufferTypeHandle;

		public ComponentTypeHandle<InterpolatedTransform> __Game_Rendering_InterpolatedTransform_RW_ComponentTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			__Game_Events_HotspotFrame_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<HotspotFrame>(true);
			__Game_Rendering_InterpolatedTransform_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InterpolatedTransform>(false);
		}
	}

	private RenderingSystem m_RenderingSystem;

	private EntityQuery m_EventQuery;

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
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_RenderingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RenderingSystem>();
		m_EventQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<WeatherPhenomenon>(),
			ComponentType.ReadOnly<HotspotFrame>(),
			ComponentType.ReadWrite<InterpolatedTransform>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_EventQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		UpdateTransformDataJob updateTransformDataJob = new UpdateTransformDataJob
		{
			m_FrameIndex = m_RenderingSystem.frameIndex,
			m_FrameTime = m_RenderingSystem.frameTime,
			m_HotspotFrameType = InternalCompilerInterface.GetBufferTypeHandle<HotspotFrame>(ref __TypeHandle.__Game_Events_HotspotFrame_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InterpolatedTransformType = InternalCompilerInterface.GetComponentTypeHandle<InterpolatedTransform>(ref __TypeHandle.__Game_Rendering_InterpolatedTransform_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef)
		};
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<UpdateTransformDataJob>(updateTransformDataJob, m_EventQuery, ((SystemBase)this).Dependency);
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
	public EventInterpolateSystem()
	{
	}
}
