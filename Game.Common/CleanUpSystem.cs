using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Common;

public class CleanUpSystem : GameSystemBase
{
	private NativeList<Entity> m_DeletedEntities;

	private NativeList<Entity> m_UpdatedEntities;

	private JobHandle m_DeletedDeps;

	private JobHandle m_UpdatedDeps;

	private ComponentTypeSet m_UpdateTypes;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_UpdateTypes = new ComponentTypeSet((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadWrite<Created>(),
			ComponentType.ReadWrite<Updated>(),
			ComponentType.ReadWrite<Applied>(),
			ComponentType.ReadWrite<EffectsUpdated>(),
			ComponentType.ReadWrite<BatchesUpdated>(),
			ComponentType.ReadWrite<PathfindUpdated>()
		});
	}

	public void AddDeleted(NativeList<Entity> deletedEntities, JobHandle deletedDeps)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		m_DeletedEntities = deletedEntities;
		m_DeletedDeps = deletedDeps;
	}

	public void AddUpdated(NativeList<Entity> updatedEntities, JobHandle updatedDeps)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		m_UpdatedEntities = updatedEntities;
		m_UpdatedDeps = updatedDeps;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_DeletedDeps)).Complete();
		((JobHandle)(ref m_UpdatedDeps)).Complete();
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).DestroyEntity(m_DeletedEntities.AsArray());
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).RemoveComponent(m_UpdatedEntities.AsArray(), ref m_UpdateTypes);
		m_DeletedEntities.Dispose();
		m_UpdatedEntities.Dispose();
	}

	[Preserve]
	public CleanUpSystem()
	{
	}
}
