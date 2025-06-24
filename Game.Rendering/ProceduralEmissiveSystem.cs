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

public class ProceduralEmissiveSystem : GameSystemBase, IPreDeserialize
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

	public const uint EMISSIVE_MEMORY_DEFAULT = 2097152u;

	public const uint EMISSIVE_MEMORY_INCREMENT = 1048576u;

	public const uint UPLOADER_CHUNK_SIZE = 131072u;

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
		m_HeapAllocator = new NativeHeapAllocator(2097152u / (uint)System.Runtime.CompilerServices.Unsafe.SizeOf<float4>(), 1u, (Allocator)4);
		m_SparseUploader = new SparseUploader("Procedural emissive uploader", (GraphicsBuffer)null, 131072);
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
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		CompleteUpload();
		((JobHandle)(ref m_HeapDeps)).Complete();
		if (m_IsAllocating)
		{
			m_IsAllocating = false;
			m_HeapAllocatorByteSize = (int)((NativeHeapAllocator)(ref m_HeapAllocator)).Size * System.Runtime.CompilerServices.Unsafe.SizeOf<float4>();
			int heapAllocatorByteSize = m_HeapAllocatorByteSize;
			int num = ((m_ComputeBuffer != null) ? (m_ComputeBuffer.count * m_ComputeBuffer.stride) : 0);
			if (heapAllocatorByteSize != num)
			{
				GraphicsBuffer val = new GraphicsBuffer((Target)32, heapAllocatorByteSize / 4, 4);
				val.name = "Procedural emissive buffer";
				Shader.SetGlobalBuffer("_LightInfo", val);
				((SparseUploader)(ref m_SparseUploader)).ReplaceBuffer(val, true, 0);
				if (m_ComputeBuffer != null)
				{
					m_ComputeBuffer.Release();
				}
				else
				{
					val.SetData<float4>(new List<float4> { float4.zero }, 0, 0, 1);
				}
				m_ComputeBuffer = val;
			}
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

	public ThreadedSparseUploader BeginUpload(int opCount, uint dataSize, uint maxOpSize)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		m_ThreadedSparseUploader = ((SparseUploader)(ref m_SparseUploader)).Begin((int)dataSize, (int)maxOpSize, opCount);
		m_IsUploading = true;
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
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_HeapDeps)).Complete();
		allocatedSize = ((NativeHeapAllocator)(ref m_HeapAllocator)).UsedSpace * (uint)System.Runtime.CompilerServices.Unsafe.SizeOf<float4>();
		bufferSize = ((NativeHeapAllocator)(ref m_HeapAllocator)).Size * (uint)System.Runtime.CompilerServices.Unsafe.SizeOf<float4>();
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

	public static void GetGpuLights(Emissive emissive, in DynamicBuffer<ProceduralLight> proceduralLights, in DynamicBuffer<LightState> lights, NativeList<float4> gpuLights)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		gpuLights[0] = default(float4);
		for (int i = 0; i < proceduralLights.Length; i++)
		{
			ProceduralLight proceduralLight = proceduralLights[i];
			LightState lightState = lights[emissive.m_LightOffset + i];
			float4 val = math.lerp(proceduralLight.m_Color, proceduralLight.m_Color2, lightState.m_Color);
			val.w *= lightState.m_Intensity;
			gpuLights[i + 1] = val;
		}
	}

	[Preserve]
	public ProceduralEmissiveSystem()
	{
	}
}
