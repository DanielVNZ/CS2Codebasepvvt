using System.Runtime.CompilerServices;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class InitializeBoneHistoriesSystem : GameSystemBase
{
	[BurstCompile]
	private struct InitializeBoneHistoriesJob : IJobParallelForDefer
	{
		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public BufferLookup<Skeleton> m_Skeletons;

		[ReadOnly]
		public BufferLookup<Bone> m_Bones;

		[ReadOnly]
		public BufferLookup<ProceduralBone> m_ProceduralBones;

		[ReadOnly]
		public BufferLookup<SubMesh> m_SubMeshes;

		[NativeDisableParallelForRestriction]
		public BufferLookup<BoneHistory> m_BoneHistories;

		[ReadOnly]
		public NativeList<PreCullingData> m_CullingData;

		public void Execute(int index)
		{
			PreCullingData cullingData = m_CullingData[index];
			if ((cullingData.m_Flags & (PreCullingFlags.NearCameraUpdated | PreCullingFlags.Updated)) != 0 && (cullingData.m_Flags & PreCullingFlags.Skeleton) != 0)
			{
				if ((cullingData.m_Flags & PreCullingFlags.NearCamera) == 0)
				{
					Remove(cullingData);
				}
				else
				{
					Update(cullingData);
				}
			}
		}

		private void Remove(PreCullingData cullingData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			m_BoneHistories[cullingData.m_Entity].Clear();
		}

		private void Update(PreCullingData cullingData)
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
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = m_PrefabRefData[cullingData.m_Entity];
			DynamicBuffer<SubMesh> val = default(DynamicBuffer<SubMesh>);
			if (m_SubMeshes.TryGetBuffer(prefabRef.m_Prefab, ref val))
			{
				DynamicBuffer<Skeleton> val2 = m_Skeletons[cullingData.m_Entity];
				DynamicBuffer<Bone> bones = m_Bones[cullingData.m_Entity];
				DynamicBuffer<BoneHistory> val3 = m_BoneHistories[cullingData.m_Entity];
				if (bones.Length == val3.Length)
				{
					return;
				}
				val3.ResizeUninitialized(bones.Length);
				DynamicBuffer<Skeleton> val4 = default(DynamicBuffer<Skeleton>);
				DynamicBuffer<Bone> bones2 = default(DynamicBuffer<Bone>);
				bool flag = false;
				if ((cullingData.m_Flags & PreCullingFlags.Temp) != 0)
				{
					Temp temp = m_TempData[cullingData.m_Entity];
					PrefabRef prefabRef2 = default(PrefabRef);
					flag = m_PrefabRefData.TryGetComponent(temp.m_Original, ref prefabRef2) && m_Skeletons.TryGetBuffer(temp.m_Original, ref val4) && m_Bones.TryGetBuffer(temp.m_Original, ref bones2) && prefabRef2.m_Prefab == prefabRef.m_Prefab && val4.Length == val2.Length && bones2.Length == bones.Length;
				}
				NativeList<float4x4> tempMatrices = default(NativeList<float4x4>);
				for (int i = 0; i < val2.Length; i++)
				{
					Skeleton skeleton = val2[i];
					if (skeleton.m_BoneOffset >= 0)
					{
						SubMesh subMesh = val[i];
						DynamicBuffer<ProceduralBone> proceduralBones = m_ProceduralBones[subMesh.m_SubMesh];
						if (!tempMatrices.IsCreated)
						{
							tempMatrices._002Ector(proceduralBones.Length * 2, AllocatorHandle.op_Implicit((Allocator)2));
						}
						tempMatrices.ResizeUninitialized(proceduralBones.Length * 2);
						if (flag)
						{
							ProceduralSkeletonSystem.GetSkinMatrices(val4[i], in proceduralBones, in bones2, tempMatrices);
						}
						else
						{
							ProceduralSkeletonSystem.GetSkinMatrices(skeleton, in proceduralBones, in bones, tempMatrices);
						}
						for (int j = 0; j < proceduralBones.Length; j++)
						{
							ProceduralBone proceduralBone = proceduralBones[j];
							val3[skeleton.m_BoneOffset + j] = new BoneHistory
							{
								m_Matrix = tempMatrices[proceduralBones.Length + proceduralBone.m_BindIndex]
							};
						}
					}
				}
				if (tempMatrices.IsCreated)
				{
					tempMatrices.Dispose();
				}
			}
			else
			{
				Remove(cullingData);
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Skeleton> __Game_Rendering_Skeleton_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Bone> __Game_Rendering_Bone_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ProceduralBone> __Game_Prefabs_ProceduralBone_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubMesh> __Game_Prefabs_SubMesh_RO_BufferLookup;

		public BufferLookup<BoneHistory> __Game_Rendering_BoneHistory_RW_BufferLookup;

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
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Rendering_Skeleton_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Skeleton>(true);
			__Game_Rendering_Bone_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Bone>(true);
			__Game_Prefabs_ProceduralBone_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ProceduralBone>(true);
			__Game_Prefabs_SubMesh_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubMesh>(true);
			__Game_Rendering_BoneHistory_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<BoneHistory>(false);
		}
	}

	private PreCullingSystem m_PreCullingSystem;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_PreCullingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PreCullingSystem>();
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
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		InitializeBoneHistoriesJob obj = new InitializeBoneHistoriesJob
		{
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Skeletons = InternalCompilerInterface.GetBufferLookup<Skeleton>(ref __TypeHandle.__Game_Rendering_Skeleton_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Bones = InternalCompilerInterface.GetBufferLookup<Bone>(ref __TypeHandle.__Game_Rendering_Bone_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ProceduralBones = InternalCompilerInterface.GetBufferLookup<ProceduralBone>(ref __TypeHandle.__Game_Prefabs_ProceduralBone_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubMeshes = InternalCompilerInterface.GetBufferLookup<SubMesh>(ref __TypeHandle.__Game_Prefabs_SubMesh_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BoneHistories = InternalCompilerInterface.GetBufferLookup<BoneHistory>(ref __TypeHandle.__Game_Rendering_BoneHistory_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CullingData = m_PreCullingSystem.GetUpdatedData(readOnly: true, out dependencies)
		};
		JobHandle val = IJobParallelForDeferExtensions.Schedule<InitializeBoneHistoriesJob, PreCullingData>(obj, obj.m_CullingData, 4, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies));
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
	public InitializeBoneHistoriesSystem()
	{
	}
}
