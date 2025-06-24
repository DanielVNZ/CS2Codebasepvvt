using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.IO.AssetDatabase;
using Game.Common;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Prefabs;

[CompilerGenerated]
public class PrefabInitializeSystem : GameSystemBase
{
	private struct ListItem
	{
		public readonly Entity m_Entity;

		public readonly PrefabBase m_Prefab;

		public ListItem(Entity entity, PrefabBase prefab)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_Entity = entity;
			m_Prefab = prefab;
		}
	}

	private struct QueueItem
	{
		public readonly PrefabBase m_Prefab;

		public readonly PrefabBase m_ParentPrefab;

		public readonly ComponentBase m_ParentComponent;

		public QueueItem(PrefabBase prefab, PrefabBase parentPrefab, ComponentBase parentComponent)
		{
			m_Prefab = prefab;
			m_ParentPrefab = parentPrefab;
			m_ParentComponent = parentComponent;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabData> __Game_Prefabs_PrefabData_RO_ComponentTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_PrefabData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabData>(true);
		}
	}

	private EntityQuery m_PrefabQuery;

	private PrefabSystem m_PrefabSystem;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_PrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<PrefabData>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_PrefabQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_PrefabQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			EntityTypeHandle entityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<PrefabData> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			((SystemBase)this).CompleteDependency();
			List<ListItem> list = new List<ListItem>();
			Queue<QueueItem> queue = new Queue<QueueItem>();
			HashSet<PrefabBase> hashSet = new HashSet<PrefabBase>();
			List<PrefabBase> dependencies = new List<PrefabBase>();
			List<ComponentBase> components = new List<ComponentBase>();
			for (int i = 0; i < val.Length; i++)
			{
				ArchetypeChunk val2 = val[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray(entityTypeHandle);
				NativeArray<PrefabData> nativeArray2 = ((ArchetypeChunk)(ref val2)).GetNativeArray<PrefabData>(ref componentTypeHandle);
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					PrefabBase prefab = m_PrefabSystem.GetPrefab<PrefabBase>(nativeArray2[j]);
					list.Add(new ListItem(nativeArray[j], prefab));
					hashSet.Add(prefab);
				}
			}
			foreach (ListItem item in list)
			{
				InitializePrefab(item.m_Entity, item.m_Prefab, queue, hashSet, dependencies, components);
			}
			QueueItem queueItem = default(QueueItem);
			while (queue.TryDequeue(ref queueItem))
			{
				if (m_PrefabSystem.AddPrefab(queueItem.m_Prefab, null, queueItem.m_ParentPrefab, queueItem.m_ParentComponent))
				{
					Entity entity = m_PrefabSystem.GetEntity(queueItem.m_Prefab);
					InitializePrefab(entity, queueItem.m_Prefab, queue, hashSet, dependencies, components);
					list.Add(new ListItem(entity, queueItem.m_Prefab));
				}
			}
			foreach (ListItem item2 in list)
			{
				LateInitializePrefab(item2.m_Entity, item2.m_Prefab, dependencies, components);
			}
		}
		finally
		{
			val.Dispose();
		}
	}

	private void InitializePrefab(Entity entity, PrefabBase prefab, Queue<QueueItem> queue, HashSet<PrefabBase> prefabSet, List<PrefabBase> dependencies, List<ComponentBase> components)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		prefab.GetComponents(components);
		for (int i = 0; i < components.Count; i++)
		{
			ComponentBase componentBase = components[i];
			try
			{
				componentBase.Initialize(((ComponentSystemBase)this).EntityManager, entity);
				componentBase.GetDependencies(dependencies);
			}
			catch (Exception ex)
			{
				COSystemBase.baseLog.ErrorFormat((Object)(object)prefab, ex, "Error when initializing prefab: {0} ({1})", (object)((Object)prefab).name, (object)(((AssetData)(object)prefab.asset != (IAssetData)null) ? ((string)(object)prefab.asset) : "No asset"));
			}
			finally
			{
				foreach (PrefabBase dependency in dependencies)
				{
					if ((Object)(object)dependency == (Object)null || prefabSet.Add(dependency))
					{
						queue.Enqueue(new QueueItem(dependency, prefab, componentBase));
					}
				}
				dependencies.Clear();
			}
		}
		components.Clear();
	}

	private void LateInitializePrefab(Entity entity, PrefabBase prefab, List<PrefabBase> dependencies, List<ComponentBase> components)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		bool flag = m_PrefabSystem.IsUnlockable(prefab);
		bool canIgnoreUnlockDependencies = prefab.canIgnoreUnlockDependencies;
		prefab.GetComponents(components);
		UnlockableBase unlockableBase = null;
		if (flag)
		{
			unlockableBase = prefab.GetComponent<UnlockableBase>();
		}
		for (int i = 0; i < components.Count; i++)
		{
			ComponentBase componentBase = components[i];
			if (!((Object)(object)componentBase != (Object)(object)unlockableBase))
			{
				continue;
			}
			try
			{
				componentBase.LateInitialize(((ComponentSystemBase)this).EntityManager, entity);
				if (!canIgnoreUnlockDependencies || !componentBase.ignoreUnlockDependencies)
				{
					componentBase.GetDependencies(dependencies);
				}
			}
			catch (Exception ex)
			{
				COSystemBase.baseLog.ErrorFormat((Object)(object)prefab, ex, "Error when initializing prefab: {0} ({1})", (object)((Object)prefab).name, (object)(((AssetData)(object)prefab.asset != (IAssetData)null) ? ((string)(object)prefab.asset) : "No asset"));
			}
		}
		if (flag)
		{
			try
			{
				if ((Object)(object)unlockableBase != (Object)null)
				{
					unlockableBase.LateInitialize(((ComponentSystemBase)this).EntityManager, entity, dependencies);
				}
				else
				{
					UnlockableBase.DefaultLateInitialize(((ComponentSystemBase)this).EntityManager, entity, dependencies);
				}
			}
			catch (Exception ex2)
			{
				COSystemBase.baseLog.ErrorFormat((Object)(object)prefab, ex2, "Error when initializing prefab: {0} ({1})", (object)((Object)prefab).name, (object)(((AssetData)(object)prefab.asset != (IAssetData)null) ? ((string)(object)prefab.asset) : "No asset"));
			}
		}
		components.Clear();
		dependencies.Clear();
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
	public PrefabInitializeSystem()
	{
	}
}
