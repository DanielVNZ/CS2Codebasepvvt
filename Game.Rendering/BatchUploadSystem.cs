using Colossal.Rendering;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Rendering;

public class BatchUploadSystem : GameSystemBase
{
	[BurstCompile]
	private struct BatchUploadJob : IJobParallelFor
	{
		public ParallelUploadWriter<CullingData, GroupData, BatchData, InstanceData> m_NativeBatchInstances;

		public void Execute(int index)
		{
			m_NativeBatchInstances.UploadInstances(index);
		}
	}

	private BatchManagerSystem m_BatchManagerSystem;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_BatchManagerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<BatchManagerSystem>();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		NativeBatchInstances<CullingData, GroupData, BatchData, InstanceData> nativeBatchInstances = m_BatchManagerSystem.GetNativeBatchInstances(readOnly: false, out dependencies);
		JobHandle dependencies2;
		NativeSubBatches<CullingData, GroupData, BatchData, InstanceData> nativeSubBatches = m_BatchManagerSystem.GetNativeSubBatches(readOnly: true, out dependencies2);
		ManagedBatches<OptionalProperties> managedBatches = m_BatchManagerSystem.GetManagedBatches();
		((JobHandle)(ref dependencies)).Complete();
		((JobHandle)(ref dependencies2)).Complete();
		managedBatches.StartUpload<CullingData, GroupData, BatchData, InstanceData>(nativeBatchInstances, nativeSubBatches);
		int activeGroupCount = nativeBatchInstances.GetActiveGroupCount();
		BatchUploadJob batchUploadJob = new BatchUploadJob
		{
			m_NativeBatchInstances = nativeBatchInstances.BeginParallelUpload()
		};
		JobHandle val = IJobParallelForExtensions.Schedule<BatchUploadJob>(batchUploadJob, activeGroupCount, 1, default(JobHandle));
		JobHandle jobHandle = nativeBatchInstances.EndParallelUpload(batchUploadJob.m_NativeBatchInstances, val);
		m_BatchManagerSystem.AddNativeSubBatchesReader(val);
		m_BatchManagerSystem.AddNativeBatchInstancesWriter(jobHandle);
	}

	[Preserve]
	public BatchUploadSystem()
	{
	}
}
