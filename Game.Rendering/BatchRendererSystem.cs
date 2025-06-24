using Colossal.Rendering;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace Game.Rendering;

public class BatchRendererSystem : GameSystemBase
{
	[BurstCompile]
	private struct ClearUpdatedMetaDatasJob : IJob
	{
		public NativeBatchGroups<CullingData, GroupData, BatchData, InstanceData> m_NativeBatchGroups;

		public void Execute()
		{
			m_NativeBatchGroups.ClearObsoleteManagedBatches();
			m_NativeBatchGroups.ClearUpdatedMetaDatas();
		}
	}

	private BatchManagerSystem m_BatchManagerSystem;

	private BatchMeshSystem m_BatchMeshSystem;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_BatchManagerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<BatchManagerSystem>();
		m_BatchMeshSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<BatchMeshSystem>();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		NativeBatchGroups<CullingData, GroupData, BatchData, InstanceData> nativeBatchGroups = m_BatchManagerSystem.GetNativeBatchGroups(readOnly: true, out dependencies);
		JobHandle dependencies2;
		NativeSubBatches<CullingData, GroupData, BatchData, InstanceData> nativeSubBatches = m_BatchManagerSystem.GetNativeSubBatches(readOnly: false, out dependencies2);
		ManagedBatches<OptionalProperties> managedBatches = m_BatchManagerSystem.GetManagedBatches();
		((JobHandle)(ref dependencies)).Complete();
		((JobHandle)(ref dependencies2)).Complete();
		ObsoleteManagedBatchEnumerator obsoleteManagedBatches = nativeBatchGroups.GetObsoleteManagedBatches();
		int num = default(int);
		while (((ObsoleteManagedBatchEnumerator)(ref obsoleteManagedBatches)).GetNextObsoleteBatch(ref num))
		{
			CustomBatch customBatch = (CustomBatch)(object)managedBatches.GetBatch(num);
			m_BatchMeshSystem.RemoveBatch(customBatch, num);
			managedBatches.RemoveBatch(num);
			((ManagedBatch)customBatch).Dispose();
		}
		UpdatedMetaDataEnumerator updatedMetaDatas = nativeBatchGroups.GetUpdatedMetaDatas();
		int num2 = default(int);
		while (((UpdatedMetaDataEnumerator)(ref updatedMetaDatas)).GetNextUpdatedGroup(ref num2))
		{
			nativeSubBatches.RecreateRenderers(num2);
		}
		ObsoleteBatchRendererEnumerator obsoleteBatchRenderers = nativeSubBatches.GetObsoleteBatchRenderers();
		BatchID val = default(BatchID);
		while (((ObsoleteBatchRendererEnumerator)(ref obsoleteBatchRenderers)).GetNextObsoleteRenderer(ref val))
		{
			managedBatches.RemoveRenderer(val);
		}
		nativeSubBatches.ClearObsoleteBatchRenderers();
		UpdatedBatchRendererEnumerator updatedBatchRenderers = nativeSubBatches.GetUpdatedBatchRenderers();
		int num3 = default(int);
		while (((UpdatedBatchRendererEnumerator)(ref updatedBatchRenderers)).GetNextUpdatedGroup(ref num3))
		{
			NativeSubBatchAccessor<BatchData> subBatchAccessor = nativeSubBatches.GetSubBatchAccessor(num3);
			for (int i = 0; i < subBatchAccessor.Length; i++)
			{
				NativeBatchPropertyAccessor batchPropertyAccessor = nativeBatchGroups.GetBatchPropertyAccessor(num3, i);
				if (subBatchAccessor.GetBatchID(i) == BatchID.Null)
				{
					BatchID val2 = managedBatches.AddBatchRenderer(batchPropertyAccessor);
					nativeSubBatches.SetBatchID(num3, i, val2);
				}
			}
		}
		nativeSubBatches.ClearUpdatedBatchRenderers();
		JobHandle jobHandle = IJobExtensions.Schedule<ClearUpdatedMetaDatasJob>(new ClearUpdatedMetaDatasJob
		{
			m_NativeBatchGroups = m_BatchManagerSystem.GetNativeBatchGroups(readOnly: false, out dependencies)
		}, dependencies);
		m_BatchManagerSystem.AddNativeBatchGroupsWriter(jobHandle);
	}

	[Preserve]
	public BatchRendererSystem()
	{
	}
}
