using System;
using System.Diagnostics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Game.Prefabs.Modes;

public abstract class EntityQueryModePrefab : ModePrefab
{
	public abstract EntityQueryDesc GetEntityQueryDesc();

	public virtual void StoreDefaultData(EntityManager entityManager, ref NativeArray<Entity> requestedQuery, PrefabSystem prefabSystem)
	{
	}

	public abstract void RestoreDefaultData(EntityManager entityManager, ref NativeArray<Entity> entities, PrefabSystem prefabSystem);

	public abstract JobHandle ApplyModeData(EntityManager entityManager, EntityQuery requestedQuery, JobHandle deps);

	[Conditional("UNITY_EDITOR")]
	[Conditional("DEVELOPMENT_BUILD")]
	public void RecordChanges(EntityManager entityManager, ref NativeArray<Entity> entities)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Enumerator<Entity> enumerator = entities.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				_ = enumerator.Current;
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
	}

	[Conditional("UNITY_EDITOR")]
	[Conditional("DEVELOPMENT_BUILD")]
	protected virtual void RecordChanges(EntityManager entityManager, Entity entity)
	{
	}
}
