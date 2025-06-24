using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Prefabs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class InitializeBonesSystem : GameSystemBase
{
	[BurstCompile]
	private struct InitializeBonesJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public BufferLookup<ProceduralBone> m_ProceduralBones;

		[ReadOnly]
		public BufferLookup<SubMesh> m_SubMeshes;

		public BufferLookup<Skeleton> m_Skeletons;

		public BufferLookup<Bone> m_Bones;

		public BufferLookup<Momentum> m_Momentums;

		[ReadOnly]
		public int m_CurrentTime;

		[ReadOnly]
		public NativeList<PreCullingData> m_CullingData;

		public NativeHeapAllocator m_HeapAllocator;

		public NativeReference<ProceduralSkeletonSystem.AllocationInfo> m_AllocationInfo;

		public NativeQueue<ProceduralSkeletonSystem.AllocationRemove> m_AllocationRemoves;

		public void Execute()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			ref ProceduralSkeletonSystem.AllocationInfo allocationInfo = ref CollectionUtils.ValueAsRef<ProceduralSkeletonSystem.AllocationInfo>(m_AllocationInfo);
			for (int i = 0; i < m_CullingData.Length; i++)
			{
				PreCullingData cullingData = m_CullingData[i];
				if ((cullingData.m_Flags & (PreCullingFlags.NearCameraUpdated | PreCullingFlags.Updated)) != 0 && (cullingData.m_Flags & PreCullingFlags.Skeleton) != 0)
				{
					if ((cullingData.m_Flags & PreCullingFlags.NearCamera) == 0)
					{
						Remove(cullingData);
					}
					else
					{
						Update(cullingData, ref allocationInfo);
					}
				}
			}
		}

		private void Remove(PreCullingData cullingData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<Skeleton> skeletons = m_Skeletons[cullingData.m_Entity];
			DynamicBuffer<Bone> val = m_Bones[cullingData.m_Entity];
			Deallocate(skeletons);
			skeletons.Clear();
			val.Clear();
			DynamicBuffer<Momentum> val2 = default(DynamicBuffer<Momentum>);
			if (m_Momentums.TryGetBuffer(cullingData.m_Entity, ref val2))
			{
				val2.Clear();
			}
		}

		private void Update(PreCullingData cullingData, ref ProceduralSkeletonSystem.AllocationInfo allocationInfo)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = m_PrefabRefData[cullingData.m_Entity];
			DynamicBuffer<SubMesh> val = default(DynamicBuffer<SubMesh>);
			if (m_SubMeshes.TryGetBuffer(prefabRef.m_Prefab, ref val))
			{
				DynamicBuffer<Skeleton> skeletons = m_Skeletons[cullingData.m_Entity];
				DynamicBuffer<Bone> val2 = m_Bones[cullingData.m_Entity];
				DynamicBuffer<Momentum> val3 = default(DynamicBuffer<Momentum>);
				m_Momentums.TryGetBuffer(cullingData.m_Entity, ref val3);
				int num = 0;
				for (int i = 0; i < val.Length; i++)
				{
					SubMesh subMesh = val[i];
					if (m_ProceduralBones.HasBuffer(subMesh.m_SubMesh))
					{
						num += m_ProceduralBones[subMesh.m_SubMesh].Length;
					}
				}
				if (skeletons.Length == val.Length && val2.Length == num)
				{
					return;
				}
				Deallocate(skeletons);
				skeletons.ResizeUninitialized(val.Length);
				val2.ResizeUninitialized(num);
				if (val3.IsCreated)
				{
					val3.ResizeUninitialized(num);
					for (int j = 0; j < val3.Length; j++)
					{
						val3[j] = default(Momentum);
					}
				}
				num = 0;
				DynamicBuffer<ProceduralBone> val4 = default(DynamicBuffer<ProceduralBone>);
				for (int k = 0; k < val.Length; k++)
				{
					SubMesh subMesh2 = val[k];
					if (m_ProceduralBones.TryGetBuffer(subMesh2.m_SubMesh, ref val4))
					{
						NativeHeapBlock bufferAllocation = ((NativeHeapAllocator)(ref m_HeapAllocator)).Allocate((uint)val4.Length, 1u);
						if (((NativeHeapBlock)(ref bufferAllocation)).Empty)
						{
							((NativeHeapAllocator)(ref m_HeapAllocator)).Resize(((NativeHeapAllocator)(ref m_HeapAllocator)).Size + 1048576u / (uint)System.Runtime.CompilerServices.Unsafe.SizeOf<float4x4>());
							bufferAllocation = ((NativeHeapAllocator)(ref m_HeapAllocator)).Allocate((uint)val4.Length, 1u);
						}
						allocationInfo.m_AllocationCount++;
						Skeleton skeleton = new Skeleton
						{
							m_BufferAllocation = bufferAllocation,
							m_BoneOffset = num,
							m_CurrentUpdated = true,
							m_HistoryUpdated = true
						};
						for (int l = 0; l < val4.Length; l++)
						{
							ProceduralBone proceduralBone = val4[l];
							skeleton.m_RequireHistory |= proceduralBone.m_ConnectionID != 0;
							val2[num++] = new Bone
							{
								m_Position = proceduralBone.m_Position,
								m_Rotation = proceduralBone.m_Rotation,
								m_Scale = proceduralBone.m_Scale
							};
						}
						skeletons[k] = skeleton;
					}
					else
					{
						skeletons[k] = new Skeleton
						{
							m_BoneOffset = -1
						};
					}
				}
			}
			else
			{
				Remove(cullingData);
			}
		}

		private void Deallocate(DynamicBuffer<Skeleton> skeletons)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < skeletons.Length; i++)
			{
				Skeleton skeleton = skeletons[i];
				if (!((NativeHeapBlock)(ref skeleton.m_BufferAllocation)).Empty)
				{
					m_AllocationRemoves.Enqueue(new ProceduralSkeletonSystem.AllocationRemove
					{
						m_Allocation = skeleton.m_BufferAllocation,
						m_RemoveTime = m_CurrentTime
					});
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ProceduralBone> __Game_Prefabs_ProceduralBone_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubMesh> __Game_Prefabs_SubMesh_RO_BufferLookup;

		public BufferLookup<Skeleton> __Game_Rendering_Skeleton_RW_BufferLookup;

		public BufferLookup<Bone> __Game_Rendering_Bone_RW_BufferLookup;

		public BufferLookup<Momentum> __Game_Rendering_Momentum_RW_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ProceduralBone_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ProceduralBone>(true);
			__Game_Prefabs_SubMesh_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubMesh>(true);
			__Game_Rendering_Skeleton_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Skeleton>(false);
			__Game_Rendering_Bone_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Bone>(false);
			__Game_Rendering_Momentum_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Momentum>(false);
		}
	}

	private ProceduralSkeletonSystem m_ProceduralSkeletonSystem;

	private PreCullingSystem m_PreCullingSystem;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_ProceduralSkeletonSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ProceduralSkeletonSystem>();
		m_PreCullingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PreCullingSystem>();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		NativeReference<ProceduralSkeletonSystem.AllocationInfo> allocationInfo;
		NativeQueue<ProceduralSkeletonSystem.AllocationRemove> allocationRemoves;
		int currentTime;
		JobHandle dependencies;
		NativeHeapAllocator heapAllocator = m_ProceduralSkeletonSystem.GetHeapAllocator(out allocationInfo, out allocationRemoves, out currentTime, out dependencies);
		JobHandle dependencies2;
		JobHandle val = IJobExtensions.Schedule<InitializeBonesJob>(new InitializeBonesJob
		{
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ProceduralBones = InternalCompilerInterface.GetBufferLookup<ProceduralBone>(ref __TypeHandle.__Game_Prefabs_ProceduralBone_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubMeshes = InternalCompilerInterface.GetBufferLookup<SubMesh>(ref __TypeHandle.__Game_Prefabs_SubMesh_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Skeletons = InternalCompilerInterface.GetBufferLookup<Skeleton>(ref __TypeHandle.__Game_Rendering_Skeleton_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Bones = InternalCompilerInterface.GetBufferLookup<Bone>(ref __TypeHandle.__Game_Rendering_Bone_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Momentums = InternalCompilerInterface.GetBufferLookup<Momentum>(ref __TypeHandle.__Game_Rendering_Momentum_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentTime = currentTime,
			m_CullingData = m_PreCullingSystem.GetUpdatedData(readOnly: true, out dependencies2),
			m_HeapAllocator = heapAllocator,
			m_AllocationInfo = allocationInfo,
			m_AllocationRemoves = allocationRemoves
		}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies2, dependencies));
		m_ProceduralSkeletonSystem.AddHeapWriter(val);
		m_PreCullingSystem.AddCullingDataReader(val);
		((SystemBase)this).Dependency = val;
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
	public InitializeBonesSystem()
	{
	}
}
