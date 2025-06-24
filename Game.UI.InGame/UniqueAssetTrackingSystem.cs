using System;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

public class UniqueAssetTrackingSystem : GameSystemBase, IUniqueAssetTrackingSystem
{
	private EntityQuery m_LoadedUniqueAssetQuery;

	private EntityQuery m_DeletedUniqueAssetQuery;

	private EntityQuery m_PlacedUniqueAssetQuery;

	private bool m_Loaded;

	public NativeParallelHashSet<Entity> placedUniqueAssets { get; private set; }

	public Action<Entity, bool> EventUniqueAssetStatusChanged { get; set; }

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_LoadedUniqueAssetQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Objects.UniqueObject>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Temp>()
		});
		m_DeletedUniqueAssetQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Game.Objects.UniqueObject>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_PlacedUniqueAssetQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Game.Objects.UniqueObject>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<Created>(),
			ComponentType.Exclude<Temp>()
		});
		placedUniqueAssets = new NativeParallelHashSet<Entity>(32, AllocatorHandle.op_Implicit((Allocator)4));
	}

	[Preserve]
	protected override void OnDestroy()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		placedUniqueAssets.Dispose();
		base.OnDestroy();
	}

	private bool GetLoaded()
	{
		if (m_Loaded)
		{
			m_Loaded = false;
			return true;
		}
		return false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		if (GetLoaded() && !((EntityQuery)(ref m_LoadedUniqueAssetQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<PrefabRef> val = ((EntityQuery)(ref m_LoadedUniqueAssetQuery)).ToComponentDataArray<PrefabRef>(AllocatorHandle.op_Implicit((Allocator)3));
			for (int i = 0; i < val.Length; i++)
			{
				placedUniqueAssets.Add(val[i].m_Prefab);
				EventUniqueAssetStatusChanged?.Invoke(val[i].m_Prefab, arg2: true);
			}
			val.Dispose();
		}
		if (!((EntityQuery)(ref m_PlacedUniqueAssetQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<PrefabRef> val2 = ((EntityQuery)(ref m_PlacedUniqueAssetQuery)).ToComponentDataArray<PrefabRef>(AllocatorHandle.op_Implicit((Allocator)3));
			for (int j = 0; j < val2.Length; j++)
			{
				placedUniqueAssets.Add(val2[j].m_Prefab);
				EventUniqueAssetStatusChanged?.Invoke(val2[j].m_Prefab, arg2: true);
			}
			val2.Dispose();
		}
		if (!((EntityQuery)(ref m_DeletedUniqueAssetQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<PrefabRef> val3 = ((EntityQuery)(ref m_DeletedUniqueAssetQuery)).ToComponentDataArray<PrefabRef>(AllocatorHandle.op_Implicit((Allocator)3));
			for (int k = 0; k < val3.Length; k++)
			{
				placedUniqueAssets.Remove(val3[k].m_Prefab);
				EventUniqueAssetStatusChanged?.Invoke(val3[k].m_Prefab, arg2: false);
			}
			val3.Dispose();
		}
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		placedUniqueAssets.Clear();
		m_Loaded = true;
	}

	public bool IsUniqueAsset(Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		PlaceableObjectData placeableObjectData = default(PlaceableObjectData);
		if (EntitiesExtensions.TryGetComponent<PlaceableObjectData>(((ComponentSystemBase)this).EntityManager, entity, ref placeableObjectData))
		{
			return (placeableObjectData.m_Flags & PlacementFlags.Unique) != 0;
		}
		return false;
	}

	public bool IsPlacedUniqueAsset(Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (IsUniqueAsset(entity))
		{
			return placedUniqueAssets.Contains(entity);
		}
		return false;
	}

	[Preserve]
	public UniqueAssetTrackingSystem()
	{
	}
}
