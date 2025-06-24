using System.Runtime.CompilerServices;
using Game.Creatures;
using Game.Objects;
using Game.Rendering;
using Game.Simulation;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Serialization;

[CompilerGenerated]
public class TransformFrameSystem : GameSystemBase
{
	[BurstCompile]
	private struct TransformFrameJob : IJobChunk
	{
		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<Moving> m_MovingType;

		[ReadOnly]
		public ComponentTypeHandle<HumanNavigation> m_HumanNavigationType;

		[ReadOnly]
		public ComponentTypeHandle<AnimalNavigation> m_AnimalNavigationType;

		[ReadOnly]
		public ComponentTypeHandle<Train> m_TrainType;

		public ComponentTypeHandle<InterpolatedTransform> m_InterpolatedTransformType;

		public BufferTypeHandle<TransformFrame> m_TransformFrameType;

		[ReadOnly]
		public uint m_SimulationFrameIndex;

		private const float SECONDS_PER_FRAME = 4f / 15f;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Transform> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			NativeArray<Moving> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Moving>(ref m_MovingType);
			NativeArray<HumanNavigation> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HumanNavigation>(ref m_HumanNavigationType);
			NativeArray<AnimalNavigation> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AnimalNavigation>(ref m_AnimalNavigationType);
			NativeArray<InterpolatedTransform> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<InterpolatedTransform>(ref m_InterpolatedTransformType);
			NativeArray<Train> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Train>(ref m_TrainType);
			BufferAccessor<TransformFrame> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TransformFrame>(ref m_TransformFrameType);
			uint index = ((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index;
			uint updateFrameOffset = (m_SimulationFrameIndex - index) / 16;
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Transform transform = nativeArray[i];
				nativeArray5[i] = new InterpolatedTransform(transform);
				DynamicBuffer<TransformFrame> transformFrames = bufferAccessor[i];
				bool flag = transformFrames.Length != 4;
				if (flag)
				{
					transformFrames.ResizeUninitialized(4);
				}
				TransformFlags transformFlags = (TransformFlags)0u;
				if (nativeArray6.Length != 0 && (nativeArray6[i].m_Flags & TrainFlags.Pantograph) != 0)
				{
					transformFlags |= TransformFlags.Pantograph;
				}
				if (nativeArray2.Length != 0)
				{
					Moving moving = nativeArray2[i];
					if (nativeArray3.Length != 0)
					{
						HumanNavigation humanNavigation = nativeArray3[i];
						InitTransformFrames(transform, moving, humanNavigation.m_TransformState, transformFlags, humanNavigation.m_LastActivity, transformFrames, updateFrameOffset, flag);
					}
					else if (nativeArray4.Length != 0)
					{
						AnimalNavigation animalNavigation = nativeArray4[i];
						InitTransformFrames(transform, moving, animalNavigation.m_TransformState, transformFlags, animalNavigation.m_LastActivity, transformFrames, updateFrameOffset, flag);
					}
					else
					{
						InitTransformFrames(transform, moving, TransformState.Default, transformFlags, 0, transformFrames, updateFrameOffset, flag);
					}
				}
				else
				{
					for (int j = 0; j < transformFrames.Length; j++)
					{
						transformFrames[j] = new TransformFrame(transform)
						{
							m_Flags = transformFlags
						};
					}
				}
			}
		}

		private void InitTransformFrames(Transform transform, Moving moving, TransformState state, TransformFlags flags, byte activity, DynamicBuffer<TransformFrame> transformFrames, uint updateFrameOffset, bool fullReset)
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < transformFrames.Length; i++)
			{
				TransformFrame transformFrame;
				if (fullReset)
				{
					transformFrame = new TransformFrame(transform, moving)
					{
						m_State = state,
						m_Flags = flags,
						m_Activity = activity
					};
				}
				else
				{
					transformFrame = transformFrames[i];
					transformFrame.m_Position = transform.m_Position;
					transformFrame.m_Velocity = moving.m_Velocity;
					transformFrame.m_Rotation = transform.m_Rotation;
				}
				float num = (float)((uint)((int)updateFrameOffset - i) % 4u) * (4f / 15f) + 2f / 15f;
				ref float3 position = ref transformFrame.m_Position;
				position -= moving.m_Velocity * num;
				transformFrames[i] = transformFrame;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Moving> __Game_Objects_Moving_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<HumanNavigation> __Game_Creatures_HumanNavigation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<AnimalNavigation> __Game_Creatures_AnimalNavigation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Train> __Game_Vehicles_Train_RO_ComponentTypeHandle;

		public ComponentTypeHandle<InterpolatedTransform> __Game_Rendering_InterpolatedTransform_RW_ComponentTypeHandle;

		public BufferTypeHandle<TransformFrame> __Game_Objects_TransformFrame_RW_BufferTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Objects_Moving_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Moving>(true);
			__Game_Creatures_HumanNavigation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HumanNavigation>(true);
			__Game_Creatures_AnimalNavigation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AnimalNavigation>(true);
			__Game_Vehicles_Train_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Train>(true);
			__Game_Rendering_InterpolatedTransform_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InterpolatedTransform>(false);
			__Game_Objects_TransformFrame_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<TransformFrame>(false);
		}
	}

	private SimulationSystem m_SimulationSystem;

	private EntityQuery m_Query;

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
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_Query = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<TransformFrame>(),
			ComponentType.ReadWrite<InterpolatedTransform>(),
			ComponentType.ReadOnly<Transform>()
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
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		TransformFrameJob transformFrameJob = new TransformFrameJob
		{
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MovingType = InternalCompilerInterface.GetComponentTypeHandle<Moving>(ref __TypeHandle.__Game_Objects_Moving_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HumanNavigationType = InternalCompilerInterface.GetComponentTypeHandle<HumanNavigation>(ref __TypeHandle.__Game_Creatures_HumanNavigation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AnimalNavigationType = InternalCompilerInterface.GetComponentTypeHandle<AnimalNavigation>(ref __TypeHandle.__Game_Creatures_AnimalNavigation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TrainType = InternalCompilerInterface.GetComponentTypeHandle<Train>(ref __TypeHandle.__Game_Vehicles_Train_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InterpolatedTransformType = InternalCompilerInterface.GetComponentTypeHandle<InterpolatedTransform>(ref __TypeHandle.__Game_Rendering_InterpolatedTransform_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformFrameType = InternalCompilerInterface.GetBufferTypeHandle<TransformFrame>(ref __TypeHandle.__Game_Objects_TransformFrame_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SimulationFrameIndex = m_SimulationSystem.frameIndex
		};
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<TransformFrameJob>(transformFrameJob, m_Query, ((SystemBase)this).Dependency);
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
	public TransformFrameSystem()
	{
	}
}
