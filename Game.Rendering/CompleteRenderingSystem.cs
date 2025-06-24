using Colossal.Rendering;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Rendering;

public class CompleteRenderingSystem : GameSystemBase
{
	private BatchManagerSystem m_BatchManagerSystem;

	private ManagedBatchSystem m_ManagedBatchSystem;

	private ProceduralSkeletonSystem m_ProceduralSkeletonSystem;

	private ProceduralEmissiveSystem m_ProceduralEmissiveSystem;

	private WindTextureSystem m_WindTextureSystem;

	private BatchMeshSystem m_BatchMeshSystem;

	private UpdateSystem m_UpdateSystem;

	private OverlayInfomodeSystem m_OverlayInfomodeSystem;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_BatchManagerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<BatchManagerSystem>();
		m_ManagedBatchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ManagedBatchSystem>();
		m_ProceduralSkeletonSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ProceduralSkeletonSystem>();
		m_ProceduralEmissiveSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ProceduralEmissiveSystem>();
		m_WindTextureSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WindTextureSystem>();
		m_BatchMeshSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<BatchMeshSystem>();
		m_UpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UpdateSystem>();
		m_OverlayInfomodeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<OverlayInfomodeSystem>();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		NativeBatchInstances<CullingData, GroupData, BatchData, InstanceData> nativeBatchInstances = m_BatchManagerSystem.GetNativeBatchInstances(readOnly: false, out dependencies);
		ManagedBatches<OptionalProperties> managedBatches = m_BatchManagerSystem.GetManagedBatches();
		((JobHandle)(ref dependencies)).Complete();
		managedBatches.EndUpload<CullingData, GroupData, BatchData, InstanceData>(nativeBatchInstances);
		m_ProceduralSkeletonSystem.CompleteUpload();
		m_ProceduralEmissiveSystem.CompleteUpload();
		m_WindTextureSystem.CompleteUpdate();
		m_ManagedBatchSystem.CompleteVTRequests();
		m_BatchMeshSystem.CompleteMeshes();
		m_OverlayInfomodeSystem.ApplyOverlay();
		m_UpdateSystem.Update(SystemUpdatePhase.CompleteRendering);
	}

	[Preserve]
	public CompleteRenderingSystem()
	{
	}
}
