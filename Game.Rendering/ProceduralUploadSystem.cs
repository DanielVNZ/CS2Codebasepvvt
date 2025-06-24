using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Rendering;
using Game.Prefabs;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class ProceduralUploadSystem : GameSystemBase
{
	[CompilerGenerated]
	public class Prepare : GameSystemBase
	{
		private struct TypeHandle
		{
			[ReadOnly]
			public BufferLookup<Skeleton> __Game_Rendering_Skeleton_RO_BufferLookup;

			[ReadOnly]
			public BufferLookup<Emissive> __Game_Rendering_Emissive_RO_BufferLookup;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void __AssignHandles(ref SystemState state)
			{
				//IL_0003: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				__Game_Rendering_Skeleton_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Skeleton>(true);
				__Game_Rendering_Emissive_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Emissive>(true);
			}
		}

		private ProceduralUploadSystem m_ProceduralUploadSystem;

		private TypeHandle __TypeHandle;

		[Preserve]
		protected override void OnCreate()
		{
			base.OnCreate();
			m_ProceduralUploadSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ProceduralUploadSystem>();
		}

		[Preserve]
		protected override void OnUpdate()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			m_ProceduralUploadSystem.m_UploadData = new NativeAccumulator<UploadData>(2, AllocatorHandle.op_Implicit((Allocator)3));
			JobHandle dependencies;
			ProceduralPrepareJob obj = new ProceduralPrepareJob
			{
				m_Skeletons = InternalCompilerInterface.GetBufferLookup<Skeleton>(ref __TypeHandle.__Game_Rendering_Skeleton_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Emissives = InternalCompilerInterface.GetBufferLookup<Emissive>(ref __TypeHandle.__Game_Rendering_Emissive_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_MotionBlurEnabled = m_ProceduralUploadSystem.m_ProceduralSkeletonSystem.isMotionBlurEnabled,
				m_ForceHistoryUpdate = m_ProceduralUploadSystem.m_ProceduralSkeletonSystem.forceHistoryUpdate,
				m_CullingData = m_ProceduralUploadSystem.m_PreCullingSystem.GetCullingData(readOnly: true, out dependencies),
				m_UploadData = m_ProceduralUploadSystem.m_UploadData.AsParallelWriter()
			};
			JobHandle val = IJobParallelForDeferExtensions.Schedule<ProceduralPrepareJob, PreCullingData>(obj, obj.m_CullingData, 16, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies));
			m_ProceduralUploadSystem.m_PreCullingSystem.AddCullingDataReader(val);
			m_ProceduralUploadSystem.m_PrepareDeps = val;
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
		public Prepare()
		{
		}
	}

	internal struct UploadData : IAccumulable<UploadData>
	{
		public int m_OpCount;

		public uint m_DataSize;

		public uint m_MaxOpSize;

		public void Accumulate(UploadData other)
		{
			m_OpCount += other.m_OpCount;
			m_DataSize += other.m_DataSize;
			m_MaxOpSize = math.max(m_MaxOpSize, other.m_MaxOpSize);
		}
	}

	[BurstCompile]
	private struct ProceduralPrepareJob : IJobParallelForDefer
	{
		[ReadOnly]
		public BufferLookup<Skeleton> m_Skeletons;

		[ReadOnly]
		public BufferLookup<Emissive> m_Emissives;

		[ReadOnly]
		public bool m_MotionBlurEnabled;

		[ReadOnly]
		public bool m_ForceHistoryUpdate;

		[ReadOnly]
		public NativeList<PreCullingData> m_CullingData;

		public ParallelWriter<UploadData> m_UploadData;

		public void Execute(int index)
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			PreCullingData preCullingData = m_CullingData[index];
			if ((preCullingData.m_Flags & (PreCullingFlags.Skeleton | PreCullingFlags.Emissive)) == 0 || (preCullingData.m_Flags & PreCullingFlags.NearCamera) == 0)
			{
				return;
			}
			if ((preCullingData.m_Flags & PreCullingFlags.Skeleton) != 0)
			{
				DynamicBuffer<Skeleton> val = m_Skeletons[preCullingData.m_Entity];
				UploadData uploadData = default(UploadData);
				if (m_MotionBlurEnabled)
				{
					for (int i = 0; i < val.Length; i++)
					{
						Skeleton skeleton = val[i];
						if ((skeleton.m_CurrentUpdated || skeleton.m_HistoryUpdated || m_ForceHistoryUpdate) && !((NativeHeapBlock)(ref skeleton.m_BufferAllocation)).Empty)
						{
							uint num = ((NativeHeapBlock)(ref skeleton.m_BufferAllocation)).Length * (uint)System.Runtime.CompilerServices.Unsafe.SizeOf<float4x4>();
							if (skeleton.m_CurrentUpdated)
							{
								uploadData.Accumulate(new UploadData
								{
									m_OpCount = 1,
									m_DataSize = num,
									m_MaxOpSize = num
								});
							}
							if (skeleton.m_HistoryUpdated || m_ForceHistoryUpdate)
							{
								uploadData.Accumulate(new UploadData
								{
									m_OpCount = 1,
									m_DataSize = num,
									m_MaxOpSize = num
								});
							}
						}
					}
				}
				else
				{
					for (int j = 0; j < val.Length; j++)
					{
						Skeleton skeleton2 = val[j];
						if (skeleton2.m_CurrentUpdated && !((NativeHeapBlock)(ref skeleton2.m_BufferAllocation)).Empty)
						{
							uint num2 = ((NativeHeapBlock)(ref skeleton2.m_BufferAllocation)).Length * (uint)System.Runtime.CompilerServices.Unsafe.SizeOf<float4x4>();
							uploadData.Accumulate(new UploadData
							{
								m_OpCount = 1,
								m_DataSize = num2,
								m_MaxOpSize = num2
							});
						}
					}
				}
				m_UploadData.Accumulate(0, uploadData);
			}
			if ((preCullingData.m_Flags & PreCullingFlags.Emissive) == 0)
			{
				return;
			}
			DynamicBuffer<Emissive> val2 = m_Emissives[preCullingData.m_Entity];
			UploadData uploadData2 = default(UploadData);
			for (int k = 0; k < val2.Length; k++)
			{
				Emissive emissive = val2[k];
				if (emissive.m_Updated && !((NativeHeapBlock)(ref emissive.m_BufferAllocation)).Empty)
				{
					uint num3 = ((NativeHeapBlock)(ref emissive.m_BufferAllocation)).Length * (uint)System.Runtime.CompilerServices.Unsafe.SizeOf<float4>();
					uploadData2.Accumulate(new UploadData
					{
						m_OpCount = 1,
						m_DataSize = num3,
						m_MaxOpSize = num3
					});
				}
			}
			m_UploadData.Accumulate(1, uploadData2);
		}
	}

	[BurstCompile]
	private struct ProceduralUploadJob : IJobParallelForDefer
	{
		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public BufferLookup<Bone> m_Bones;

		[ReadOnly]
		public BufferLookup<LightState> m_Lights;

		[ReadOnly]
		public BufferLookup<ProceduralBone> m_ProceduralBones;

		[ReadOnly]
		public BufferLookup<ProceduralLight> m_ProceduralLights;

		[ReadOnly]
		public BufferLookup<SubMesh> m_SubMeshes;

		[NativeDisableParallelForRestriction]
		public BufferLookup<Skeleton> m_Skeletons;

		[NativeDisableParallelForRestriction]
		public BufferLookup<BoneHistory> m_BoneHistories;

		[NativeDisableParallelForRestriction]
		public BufferLookup<Emissive> m_Emissives;

		[ReadOnly]
		public ThreadedSparseUploader m_BoneUploader;

		[ReadOnly]
		public ThreadedSparseUploader m_LightUploader;

		[ReadOnly]
		public int m_HistoryByteOffset;

		[ReadOnly]
		public bool m_MotionBlurEnabled;

		[ReadOnly]
		public bool m_ForceHistoryUpdate;

		[ReadOnly]
		public NativeList<PreCullingData> m_CullingData;

		public unsafe void Execute(int index)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0589: Unknown result type (might be due to invalid IL or missing references)
			//IL_0596: Unknown result type (might be due to invalid IL or missing references)
			//IL_059b: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0466: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0470: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0633: Unknown result type (might be due to invalid IL or missing references)
			//IL_0640: Unknown result type (might be due to invalid IL or missing references)
			//IL_060e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0487: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0520: Unknown result type (might be due to invalid IL or missing references)
			//IL_0525: Unknown result type (might be due to invalid IL or missing references)
			//IL_053d: Unknown result type (might be due to invalid IL or missing references)
			//IL_053f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
			PreCullingData preCullingData = m_CullingData[index];
			if ((preCullingData.m_Flags & (PreCullingFlags.Skeleton | PreCullingFlags.Emissive)) == 0 || (preCullingData.m_Flags & PreCullingFlags.NearCamera) == 0)
			{
				return;
			}
			DynamicBuffer<Skeleton> val = default(DynamicBuffer<Skeleton>);
			DynamicBuffer<Emissive> val2 = default(DynamicBuffer<Emissive>);
			bool flag = false;
			bool flag2 = false;
			if ((preCullingData.m_Flags & PreCullingFlags.Skeleton) != 0)
			{
				val = m_Skeletons[preCullingData.m_Entity];
				if (m_MotionBlurEnabled)
				{
					for (int i = 0; i < val.Length; i++)
					{
						ref Skeleton reference = ref val.ElementAt(i);
						if ((reference.m_CurrentUpdated || reference.m_HistoryUpdated || m_ForceHistoryUpdate) && !((NativeHeapBlock)(ref reference.m_BufferAllocation)).Empty)
						{
							flag = true;
						}
					}
				}
				else
				{
					for (int j = 0; j < val.Length; j++)
					{
						ref Skeleton reference2 = ref val.ElementAt(j);
						if (reference2.m_CurrentUpdated && !((NativeHeapBlock)(ref reference2.m_BufferAllocation)).Empty)
						{
							flag = true;
						}
					}
				}
			}
			if ((preCullingData.m_Flags & PreCullingFlags.Emissive) != 0)
			{
				val2 = m_Emissives[preCullingData.m_Entity];
				for (int k = 0; k < val2.Length; k++)
				{
					ref Emissive reference3 = ref val2.ElementAt(k);
					if (reference3.m_Updated && !((NativeHeapBlock)(ref reference3.m_BufferAllocation)).Empty)
					{
						flag2 = true;
					}
				}
			}
			if (!flag && !flag2)
			{
				return;
			}
			PrefabRef prefabRef = m_PrefabRefData[preCullingData.m_Entity];
			DynamicBuffer<SubMesh> val3 = m_SubMeshes[prefabRef.m_Prefab];
			if (flag)
			{
				NativeList<float4x4> val4 = default(NativeList<float4x4>);
				DynamicBuffer<Bone> bones = m_Bones[preCullingData.m_Entity];
				DynamicBuffer<BoneHistory> val5 = m_BoneHistories[preCullingData.m_Entity];
				if (m_MotionBlurEnabled)
				{
					for (int l = 0; l < val.Length; l++)
					{
						ref Skeleton reference4 = ref val.ElementAt(l);
						if ((!reference4.m_CurrentUpdated && !reference4.m_HistoryUpdated && !m_ForceHistoryUpdate) || ((NativeHeapBlock)(ref reference4.m_BufferAllocation)).Empty)
						{
							continue;
						}
						SubMesh subMesh = val3[l];
						DynamicBuffer<ProceduralBone> proceduralBones = m_ProceduralBones[subMesh.m_SubMesh];
						if (!val4.IsCreated)
						{
							val4._002Ector(proceduralBones.Length * 3, AllocatorHandle.op_Implicit((Allocator)2));
						}
						val4.ResizeUninitialized(proceduralBones.Length * 3);
						ProceduralSkeletonSystem.GetSkinMatrices(reference4, in proceduralBones, in bones, val4);
						if (m_ForceHistoryUpdate)
						{
							for (int m = 0; m < proceduralBones.Length; m++)
							{
								ProceduralBone proceduralBone = proceduralBones[m];
								int num = reference4.m_BoneOffset + m;
								float4x4 val6 = val4[proceduralBones.Length + proceduralBone.m_BindIndex];
								val4[proceduralBones.Length * 2 + proceduralBone.m_BindIndex] = val6;
								val5[num] = new BoneHistory
								{
									m_Matrix = val6
								};
							}
						}
						else
						{
							for (int n = 0; n < proceduralBones.Length; n++)
							{
								ProceduralBone proceduralBone2 = proceduralBones[n];
								int num2 = reference4.m_BoneOffset + n;
								float4x4 matrix = val5[num2].m_Matrix;
								float4x4 matrix2 = val4[proceduralBones.Length + proceduralBone2.m_BindIndex];
								val4[proceduralBones.Length * 2 + proceduralBone2.m_BindIndex] = matrix;
								val5[num2] = new BoneHistory
								{
									m_Matrix = matrix2
								};
							}
						}
						int num3 = proceduralBones.Length * System.Runtime.CompilerServices.Unsafe.SizeOf<float4x4>();
						int num4 = (int)((NativeHeapBlock)(ref reference4.m_BufferAllocation)).Begin * System.Runtime.CompilerServices.Unsafe.SizeOf<float4x4>();
						if (reference4.m_CurrentUpdated)
						{
							((ThreadedSparseUploader)(ref m_BoneUploader)).AddUpload((void*)((byte*)NativeListUnsafeUtility.GetUnsafePtr<float4x4>(val4) + num3), num3, num4, 1);
						}
						if (reference4.m_HistoryUpdated || m_ForceHistoryUpdate)
						{
							((ThreadedSparseUploader)(ref m_BoneUploader)).AddUpload((void*)((byte*)NativeListUnsafeUtility.GetUnsafePtr<float4x4>(val4) + num3 * 2), num3, num4 + m_HistoryByteOffset, 1);
						}
						reference4.m_HistoryUpdated = reference4.m_CurrentUpdated;
						reference4.m_CurrentUpdated = false;
					}
				}
				else
				{
					for (int num5 = 0; num5 < val.Length; num5++)
					{
						ref Skeleton reference5 = ref val.ElementAt(num5);
						if (!reference5.m_CurrentUpdated || ((NativeHeapBlock)(ref reference5.m_BufferAllocation)).Empty)
						{
							continue;
						}
						reference5.m_CurrentUpdated = false;
						SubMesh subMesh2 = val3[num5];
						DynamicBuffer<ProceduralBone> proceduralBones2 = m_ProceduralBones[subMesh2.m_SubMesh];
						if (!val4.IsCreated)
						{
							val4._002Ector(proceduralBones2.Length * 2, AllocatorHandle.op_Implicit((Allocator)2));
						}
						val4.ResizeUninitialized(proceduralBones2.Length * 2);
						ProceduralSkeletonSystem.GetSkinMatrices(reference5, in proceduralBones2, in bones, val4);
						((ThreadedSparseUploader)(ref m_BoneUploader)).AddUpload((void*)((byte*)NativeListUnsafeUtility.GetUnsafePtr<float4x4>(val4) + proceduralBones2.Length * System.Runtime.CompilerServices.Unsafe.SizeOf<float4x4>()), proceduralBones2.Length * System.Runtime.CompilerServices.Unsafe.SizeOf<float4x4>(), (int)((NativeHeapBlock)(ref reference5.m_BufferAllocation)).Begin * System.Runtime.CompilerServices.Unsafe.SizeOf<float4x4>(), 1);
						if (reference5.m_RequireHistory)
						{
							for (int num6 = 0; num6 < proceduralBones2.Length; num6++)
							{
								ProceduralBone proceduralBone3 = proceduralBones2[num6];
								float4x4 matrix3 = val4[proceduralBones2.Length + proceduralBone3.m_BindIndex];
								val5[reference5.m_BoneOffset + num6] = new BoneHistory
								{
									m_Matrix = matrix3
								};
							}
						}
					}
				}
				if (val4.IsCreated)
				{
					val4.Dispose();
				}
			}
			if (!flag2)
			{
				return;
			}
			NativeList<float4> val7 = default(NativeList<float4>);
			DynamicBuffer<LightState> lights = m_Lights[preCullingData.m_Entity];
			for (int num7 = 0; num7 < val2.Length; num7++)
			{
				ref Emissive reference6 = ref val2.ElementAt(num7);
				if (reference6.m_Updated && !((NativeHeapBlock)(ref reference6.m_BufferAllocation)).Empty)
				{
					reference6.m_Updated = false;
					SubMesh subMesh3 = val3[num7];
					DynamicBuffer<ProceduralLight> proceduralLights = m_ProceduralLights[subMesh3.m_SubMesh];
					if (!val7.IsCreated)
					{
						val7._002Ector(proceduralLights.Length + 1, AllocatorHandle.op_Implicit((Allocator)2));
					}
					val7.ResizeUninitialized(proceduralLights.Length + 1);
					ProceduralEmissiveSystem.GetGpuLights(reference6, in proceduralLights, in lights, val7);
					((ThreadedSparseUploader)(ref m_LightUploader)).AddUpload((void*)NativeListUnsafeUtility.GetUnsafePtr<float4>(val7), val7.Length * System.Runtime.CompilerServices.Unsafe.SizeOf<float4>(), (int)((NativeHeapBlock)(ref reference6.m_BufferAllocation)).Begin * System.Runtime.CompilerServices.Unsafe.SizeOf<float4>(), 1);
				}
			}
			if (val7.IsCreated)
			{
				val7.Dispose();
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Bone> __Game_Rendering_Bone_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LightState> __Game_Rendering_LightState_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ProceduralBone> __Game_Prefabs_ProceduralBone_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ProceduralLight> __Game_Prefabs_ProceduralLight_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubMesh> __Game_Prefabs_SubMesh_RO_BufferLookup;

		public BufferLookup<Skeleton> __Game_Rendering_Skeleton_RW_BufferLookup;

		public BufferLookup<BoneHistory> __Game_Rendering_BoneHistory_RW_BufferLookup;

		public BufferLookup<Emissive> __Game_Rendering_Emissive_RW_BufferLookup;

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
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Rendering_Bone_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Bone>(true);
			__Game_Rendering_LightState_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LightState>(true);
			__Game_Prefabs_ProceduralBone_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ProceduralBone>(true);
			__Game_Prefabs_ProceduralLight_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ProceduralLight>(true);
			__Game_Prefabs_SubMesh_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubMesh>(true);
			__Game_Rendering_Skeleton_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Skeleton>(false);
			__Game_Rendering_BoneHistory_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<BoneHistory>(false);
			__Game_Rendering_Emissive_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Emissive>(false);
		}
	}

	private ProceduralSkeletonSystem m_ProceduralSkeletonSystem;

	private ProceduralEmissiveSystem m_ProceduralEmissiveSystem;

	private PreCullingSystem m_PreCullingSystem;

	private PrefabSystem m_PrefabSystem;

	private RenderPrefabBase m_OverridePrefab;

	private NativeAccumulator<UploadData> m_UploadData;

	private JobHandle m_PrepareDeps;

	private Entity m_OverrideEntity;

	private LightState m_OverrideLightState;

	private int m_OverrideSingleLightIndex;

	private int m_OverrideMultiLightIndex;

	private float m_OverrideTime;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_ProceduralSkeletonSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ProceduralSkeletonSystem>();
		m_ProceduralEmissiveSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ProceduralEmissiveSystem>();
		m_PreCullingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PreCullingSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
	}

	public void SetOverride(Entity entity, RenderPrefabBase prefab, int singleLightIndex, int multiLightIndex)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (entity != m_OverrideEntity || singleLightIndex != m_OverrideSingleLightIndex || multiLightIndex != m_OverrideMultiLightIndex)
		{
			m_OverrideLightState.m_Intensity = -1f;
			m_OverrideTime = 0f;
		}
		m_OverrideEntity = entity;
		m_OverridePrefab = prefab;
		m_OverrideSingleLightIndex = singleLightIndex;
		m_OverrideMultiLightIndex = multiLightIndex;
	}

	private void UpdateOverride(ref UploadData emissiveData)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<Emissive> val = default(DynamicBuffer<Emissive>);
		DynamicBuffer<LightState> val2 = default(DynamicBuffer<LightState>);
		PrefabRef prefabRef = default(PrefabRef);
		DynamicBuffer<SubMesh> val3 = default(DynamicBuffer<SubMesh>);
		if (!EntitiesExtensions.TryGetBuffer<Emissive>(((ComponentSystemBase)this).EntityManager, m_OverrideEntity, false, ref val) || !EntitiesExtensions.TryGetBuffer<LightState>(((ComponentSystemBase)this).EntityManager, m_OverrideEntity, false, ref val2) || !EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, m_OverrideEntity, ref prefabRef) || !EntitiesExtensions.TryGetBuffer<SubMesh>(((ComponentSystemBase)this).EntityManager, prefabRef.m_Prefab, true, ref val3) || !m_OverridePrefab.TryGet<EmissiveProperties>(out var component) || !m_PrefabSystem.TryGetEntity(m_OverridePrefab, out var entity))
		{
			return;
		}
		int num = -1;
		if (m_OverrideMultiLightIndex >= 0)
		{
			num = m_OverrideMultiLightIndex;
		}
		else
		{
			if (m_OverrideSingleLightIndex < 0)
			{
				return;
			}
			num = m_OverrideSingleLightIndex;
			if (component.hasMultiLights)
			{
				num += component.m_MultiLights.Count;
			}
		}
		float deltaTime = Time.deltaTime;
		DynamicBuffer<ProceduralLight> val4 = default(DynamicBuffer<ProceduralLight>);
		float2 target = default(float2);
		DynamicBuffer<LightAnimation> val5 = default(DynamicBuffer<LightAnimation>);
		for (int i = 0; i < val.Length; i++)
		{
			ref Emissive reference = ref val.ElementAt(i);
			if (((NativeHeapBlock)(ref reference.m_BufferAllocation)).Empty)
			{
				continue;
			}
			SubMesh subMesh = val3[i];
			if (!(subMesh.m_SubMesh != entity) && EntitiesExtensions.TryGetBuffer<ProceduralLight>(((ComponentSystemBase)this).EntityManager, subMesh.m_SubMesh, true, ref val4) && num < val4.Length)
			{
				if (!reference.m_Updated)
				{
					uint num2 = ((NativeHeapBlock)(ref reference.m_BufferAllocation)).Length * (uint)System.Runtime.CompilerServices.Unsafe.SizeOf<float4>();
					emissiveData.Accumulate(new UploadData
					{
						m_OpCount = 1,
						m_DataSize = num2,
						m_MaxOpSize = num2
					});
				}
				ProceduralLight proceduralLight = val4[num];
				ref LightState reference2 = ref val2.ElementAt(reference.m_LightOffset + num);
				if (m_OverrideLightState.m_Intensity < 0f)
				{
					m_OverrideLightState = reference2;
				}
				((float2)(ref target))._002Ector(1f, 0f);
				if (proceduralLight.m_AnimationIndex >= 0 && EntitiesExtensions.TryGetBuffer<LightAnimation>(((ComponentSystemBase)this).EntityManager, subMesh.m_SubMesh, true, ref val5))
				{
					LightAnimation lightAnimation = val5[proceduralLight.m_AnimationIndex];
					m_OverrideTime += deltaTime * 60f;
					m_OverrideTime %= lightAnimation.m_DurationFrames;
					target.x *= ((AnimationCurve1)(ref lightAnimation.m_AnimationCurve)).Evaluate(m_OverrideTime / (float)lightAnimation.m_DurationFrames);
				}
				ObjectInterpolateSystem.AnimateLight(proceduralLight, ref reference, ref m_OverrideLightState, deltaTime, target, instantReset: false);
				reference2 = m_OverrideLightState;
				reference.m_Updated = true;
			}
		}
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_PrepareDeps)).Complete();
		UploadData result = m_UploadData.GetResult(0);
		UploadData emissiveData = m_UploadData.GetResult(1);
		m_UploadData.Dispose();
		if (m_OverrideEntity != Entity.Null)
		{
			UpdateOverride(ref emissiveData);
		}
		int historyByteOffset;
		JobHandle dependencies;
		ProceduralUploadJob obj = new ProceduralUploadJob
		{
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Bones = InternalCompilerInterface.GetBufferLookup<Bone>(ref __TypeHandle.__Game_Rendering_Bone_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Lights = InternalCompilerInterface.GetBufferLookup<LightState>(ref __TypeHandle.__Game_Rendering_LightState_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ProceduralBones = InternalCompilerInterface.GetBufferLookup<ProceduralBone>(ref __TypeHandle.__Game_Prefabs_ProceduralBone_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ProceduralLights = InternalCompilerInterface.GetBufferLookup<ProceduralLight>(ref __TypeHandle.__Game_Prefabs_ProceduralLight_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubMeshes = InternalCompilerInterface.GetBufferLookup<SubMesh>(ref __TypeHandle.__Game_Prefabs_SubMesh_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Skeletons = InternalCompilerInterface.GetBufferLookup<Skeleton>(ref __TypeHandle.__Game_Rendering_Skeleton_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BoneHistories = InternalCompilerInterface.GetBufferLookup<BoneHistory>(ref __TypeHandle.__Game_Rendering_BoneHistory_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Emissives = InternalCompilerInterface.GetBufferLookup<Emissive>(ref __TypeHandle.__Game_Rendering_Emissive_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BoneUploader = m_ProceduralSkeletonSystem.BeginUpload(result.m_OpCount, result.m_DataSize, result.m_MaxOpSize, out historyByteOffset),
			m_LightUploader = m_ProceduralEmissiveSystem.BeginUpload(emissiveData.m_OpCount, emissiveData.m_DataSize, emissiveData.m_MaxOpSize),
			m_HistoryByteOffset = historyByteOffset,
			m_MotionBlurEnabled = m_ProceduralSkeletonSystem.isMotionBlurEnabled,
			m_ForceHistoryUpdate = m_ProceduralSkeletonSystem.forceHistoryUpdate,
			m_CullingData = m_PreCullingSystem.GetCullingData(readOnly: true, out dependencies)
		};
		JobHandle val = IJobParallelForDeferExtensions.Schedule<ProceduralUploadJob, PreCullingData>(obj, obj.m_CullingData, 16, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies));
		m_ProceduralSkeletonSystem.AddUploadWriter(val);
		m_ProceduralEmissiveSystem.AddUploadWriter(val);
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
	public ProceduralUploadSystem()
	{
	}
}
