using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Rendering;
using Colossal.Serialization.Entities;
using Game.Prefabs;
using Game.Serialization;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Rendering;

public class ProceduralSkeletonSystem : GameSystemBase, IPreDeserialize
{
	public struct AllocationInfo
	{
		public uint m_AllocationCount;
	}

	public struct AllocationRemove
	{
		public NativeHeapBlock m_Allocation;

		public int m_RemoveTime;
	}

	[BurstCompile]
	private struct RemoveAllocationsJob : IJob
	{
		public NativeHeapAllocator m_HeapAllocator;

		public NativeReference<AllocationInfo> m_AllocationInfo;

		public NativeQueue<AllocationRemove> m_AllocationRemoves;

		public int m_CurrentTime;

		public void Execute()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			ref AllocationInfo reference = ref CollectionUtils.ValueAsRef<AllocationInfo>(m_AllocationInfo);
			while (!m_AllocationRemoves.IsEmpty())
			{
				AllocationRemove allocationRemove = m_AllocationRemoves.Peek();
				int num = m_CurrentTime - allocationRemove.m_RemoveTime;
				num += math.select(0, 65536, num < 0);
				if (num >= 255)
				{
					m_AllocationRemoves.Dequeue();
					((NativeHeapAllocator)(ref m_HeapAllocator)).Release(allocationRemove.m_Allocation);
					reference.m_AllocationCount--;
					continue;
				}
				break;
			}
		}
	}

	public const uint SKELETON_MEMORY_DEFAULT = 4194304u;

	public const uint SKELETON_MEMORY_INCREMENT = 1048576u;

	public const uint UPLOADER_CHUNK_SIZE = 524288u;

	private RenderingSystem m_RenderingSystem;

	private NativeHeapAllocator m_HeapAllocator;

	private SparseUploader m_SparseUploader;

	private ThreadedSparseUploader m_ThreadedSparseUploader;

	private NativeReference<AllocationInfo> m_AllocationInfo;

	private NativeQueue<AllocationRemove> m_AllocationRemoves;

	private bool m_IsAllocating;

	private bool m_IsUploading;

	private GraphicsBuffer m_ComputeBuffer;

	private JobHandle m_HeapDeps;

	private JobHandle m_UploadDeps;

	private int m_HeapAllocatorByteSize;

	private int m_CurrentTime;

	private bool m_AreMotionVectorsEnabled;

	private bool m_ForceHistoryUpdate;

	public bool isMotionBlurEnabled => m_AreMotionVectorsEnabled;

	public bool forceHistoryUpdate => m_ForceHistoryUpdate;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_RenderingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RenderingSystem>();
		m_HeapAllocator = new NativeHeapAllocator(4194304u / (uint)System.Runtime.CompilerServices.Unsafe.SizeOf<float4x4>(), 1u, (Allocator)4);
		m_SparseUploader = new SparseUploader("Procedural skeleton uploader", (GraphicsBuffer)null, 524288);
		m_AllocationInfo = new NativeReference<AllocationInfo>(AllocatorHandle.op_Implicit((Allocator)4), (NativeArrayOptions)1);
		m_AllocationRemoves = new NativeQueue<AllocationRemove>(AllocatorHandle.op_Implicit((Allocator)4));
		AllocateIdentityEntry();
	}

	[Preserve]
	protected override void OnDestroy()
	{
		CompleteUpload();
		((JobHandle)(ref m_HeapDeps)).Complete();
		if (((NativeHeapAllocator)(ref m_HeapAllocator)).IsCreated)
		{
			((NativeHeapAllocator)(ref m_HeapAllocator)).Dispose();
			((SparseUploader)(ref m_SparseUploader)).Dispose();
			m_AllocationInfo.Dispose();
			m_AllocationRemoves.Dispose();
		}
		if (m_ComputeBuffer != null)
		{
			m_ComputeBuffer.Release();
		}
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Expected O, but got Unknown
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		bool motionVectors = m_RenderingSystem.motionVectors;
		int num = ((!motionVectors) ? 1 : 2);
		m_ForceHistoryUpdate = m_AreMotionVectorsEnabled != motionVectors;
		CompleteUpload();
		((JobHandle)(ref m_HeapDeps)).Complete();
		if (m_IsAllocating || m_ForceHistoryUpdate)
		{
			m_IsAllocating = false;
			m_AreMotionVectorsEnabled = motionVectors;
			m_HeapAllocatorByteSize = (int)((NativeHeapAllocator)(ref m_HeapAllocator)).Size * System.Runtime.CompilerServices.Unsafe.SizeOf<float4x4>();
			int num2 = m_HeapAllocatorByteSize * num;
			int num3 = ((m_ComputeBuffer != null) ? (m_ComputeBuffer.count * m_ComputeBuffer.stride) : 0);
			if (num2 != num3)
			{
				GraphicsBuffer val = new GraphicsBuffer((Target)32, num2 / 4, 4);
				val.name = "Procedural bone buffer";
				Shader.SetGlobalBuffer("_BoneTransforms", val);
				if (motionVectors && !m_ForceHistoryUpdate)
				{
					((SparseUploader)(ref m_SparseUploader)).ReplaceBuffer(val, true, num3 / 2);
				}
				else
				{
					((SparseUploader)(ref m_SparseUploader)).ReplaceBuffer(val, true, 0);
				}
				if (m_ComputeBuffer == null)
				{
					val.SetData<float4x4>(new List<float4x4> { float4x4.identity }, 0, 0, 1);
				}
				if (motionVectors && (m_ComputeBuffer == null || m_ForceHistoryUpdate))
				{
					val.SetData<float4x4>(new List<float4x4> { float4x4.identity }, 0, (int)((NativeHeapAllocator)(ref m_HeapAllocator)).Size, 1);
				}
				if (m_ComputeBuffer != null)
				{
					m_ComputeBuffer.Release();
				}
				m_ComputeBuffer = val;
			}
			Shader.SetGlobalInt("_BonePreviousTransformsByteOffset", m_HeapAllocatorByteSize);
		}
		if (!m_AllocationRemoves.IsEmpty())
		{
			m_CurrentTime = (m_CurrentTime + m_RenderingSystem.lodTimerDelta) & 0xFFFF;
			RemoveAllocationsJob removeAllocationsJob = new RemoveAllocationsJob
			{
				m_HeapAllocator = m_HeapAllocator,
				m_AllocationInfo = m_AllocationInfo,
				m_AllocationRemoves = m_AllocationRemoves,
				m_CurrentTime = m_CurrentTime
			};
			m_HeapDeps = IJobExtensions.Schedule<RemoveAllocationsJob>(removeAllocationsJob, default(JobHandle));
		}
	}

	public ThreadedSparseUploader BeginUpload(int opCount, uint dataSize, uint maxOpSize, out int historyByteOffset)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		m_ThreadedSparseUploader = ((SparseUploader)(ref m_SparseUploader)).Begin((int)dataSize, (int)maxOpSize, opCount);
		m_IsUploading = true;
		historyByteOffset = m_HeapAllocatorByteSize;
		return m_ThreadedSparseUploader;
	}

	public void AddUploadWriter(JobHandle handle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_UploadDeps = handle;
	}

	public void CompleteUpload()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (m_IsUploading)
		{
			((JobHandle)(ref m_UploadDeps)).Complete();
			m_IsUploading = false;
			((SparseUploader)(ref m_SparseUploader)).EndAndCommit(m_ThreadedSparseUploader);
		}
	}

	public void PreDeserialize(Context context)
	{
		((JobHandle)(ref m_HeapDeps)).Complete();
		((NativeHeapAllocator)(ref m_HeapAllocator)).Clear();
		m_AllocationRemoves.Clear();
		AllocateIdentityEntry();
	}

	public NativeHeapAllocator GetHeapAllocator(out NativeReference<AllocationInfo> allocationInfo, out NativeQueue<AllocationRemove> allocationRemoves, out int currentTime, out JobHandle dependencies)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		dependencies = m_HeapDeps;
		allocationInfo = m_AllocationInfo;
		allocationRemoves = m_AllocationRemoves;
		currentTime = m_CurrentTime;
		m_IsAllocating = true;
		return m_HeapAllocator;
	}

	public void AddHeapWriter(JobHandle handle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_HeapDeps = handle;
	}

	public void GetMemoryStats(out uint allocatedSize, out uint bufferSize, out uint currentUpload, out uint uploadSize, out int allocationCount)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_HeapDeps)).Complete();
		int num = ((!m_RenderingSystem.motionVectors) ? 1 : 2);
		allocatedSize = (uint)((int)((NativeHeapAllocator)(ref m_HeapAllocator)).UsedSpace * System.Runtime.CompilerServices.Unsafe.SizeOf<float4x4>() * num);
		bufferSize = (uint)((int)((NativeHeapAllocator)(ref m_HeapAllocator)).Size * System.Runtime.CompilerServices.Unsafe.SizeOf<float4x4>() * num);
		allocationCount = (int)m_AllocationInfo.Value.m_AllocationCount;
		SparseUploaderStats val = ((SparseUploader)(ref m_SparseUploader)).ComputeStats();
		currentUpload = (uint)val.BytesGPUMemoryUploadedCurr;
		uploadSize = (uint)val.BytesGPUMemoryUsed;
	}

	private void AllocateIdentityEntry()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		m_IsAllocating = true;
		((NativeHeapAllocator)(ref m_HeapAllocator)).Allocate(1u, 1u);
		m_AllocationInfo.Value = new AllocationInfo
		{
			m_AllocationCount = 0u
		};
	}

	public static void GetSkinMatrices(Skeleton skeleton, in DynamicBuffer<ProceduralBone> proceduralBones, in DynamicBuffer<Bone> bones, NativeList<float4x4> tempMatrices)
	{
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < proceduralBones.Length; i++)
		{
			ProceduralBone proceduralBone = proceduralBones[i];
			Bone bone = bones[skeleton.m_BoneOffset + i];
			float4x4 val = float4x4.TRS(bone.m_Position, bone.m_Rotation, bone.m_Scale);
			if (proceduralBone.m_ParentIndex >= 0)
			{
				val = math.mul(tempMatrices[proceduralBone.m_ParentIndex], val);
			}
			tempMatrices[i] = val;
			tempMatrices[proceduralBones.Length + proceduralBone.m_BindIndex] = math.mul(val, proceduralBone.m_BindPose);
		}
	}

	[Preserve]
	public ProceduralSkeletonSystem()
	{
	}
}
