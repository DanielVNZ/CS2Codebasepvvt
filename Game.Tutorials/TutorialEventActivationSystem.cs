using Game.Common;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Tutorials;

public class TutorialEventActivationSystem : GameSystemBase
{
	protected EntityCommandBufferSystem m_BarrierSystem;

	private NativeQueue<Entity> m_ActivationQueue;

	private JobHandle m_InputDependencies;

	public NativeQueue<Entity> GetQueue(out JobHandle dependency)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		dependency = m_InputDependencies;
		return m_ActivationQueue;
	}

	public void AddQueueWriter(JobHandle dependency)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_InputDependencies = JobHandle.CombineDependencies(m_InputDependencies, dependency);
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_BarrierSystem = (EntityCommandBufferSystem)(object)((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier4>();
		m_ActivationQueue = new NativeQueue<Entity>(AllocatorHandle.op_Implicit((Allocator)4));
	}

	[Preserve]
	protected override void OnDestroy()
	{
		base.OnDestroy();
		m_ActivationQueue.Dispose();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_InputDependencies)).Complete();
		EntityCommandBuffer val = m_BarrierSystem.CreateCommandBuffer();
		Entity val2 = default(Entity);
		while (m_ActivationQueue.TryDequeue(ref val2))
		{
			((EntityCommandBuffer)(ref val)).AddComponent<TutorialActivated>(val2);
		}
	}

	[Preserve]
	public TutorialEventActivationSystem()
	{
	}
}
