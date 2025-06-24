using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Game.Common;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine.Scripting;

namespace Game.Prefabs;

[CompilerGenerated]
public class UIInitializeSystem : GameSystemBase
{
	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<UIObjectData> __Game_Prefabs_UIObjectData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<UIAssetCategoryData> __Game_Prefabs_UIAssetCategoryData_RO_ComponentTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_UIObjectData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<UIObjectData>(true);
			__Game_Prefabs_UIAssetCategoryData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<UIAssetCategoryData>(true);
		}
	}

	private EntityQuery m_PrefabQuery;

	private PrefabSystem m_PrefabSystem;

	private EntityQuery m_PolicyQuery;

	private TypeHandle __TypeHandle;

	public IEnumerable<PolicyPrefab> policies
	{
		get
		{
			if (!((EntityQuery)(ref m_PolicyQuery)).IsEmptyIgnoreFilter)
			{
				NativeArray<PrefabData> prefabs = ((EntityQuery)(ref m_PolicyQuery)).ToComponentDataArray<PrefabData>(AllocatorHandle.op_Implicit((Allocator)3));
				int i = 0;
				while (i < prefabs.Length)
				{
					yield return m_PrefabSystem.GetPrefab<PolicyPrefab>(prefabs[i]);
					int num = i + 1;
					i = num;
				}
				prefabs.Dispose();
			}
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<Deleted>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<UIObjectData>(),
			ComponentType.ReadOnly<UIAssetCategoryData>()
		};
		array[0] = val;
		m_PrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<PolicyData>()
		};
		array2[0] = val;
		m_PolicyQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		((ComponentSystemBase)this).RequireForUpdate(m_PrefabQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_PrefabQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			EntityTypeHandle entityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<UIObjectData> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<UIObjectData>(ref __TypeHandle.__Game_Prefabs_UIObjectData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<UIAssetCategoryData> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<UIAssetCategoryData>(ref __TypeHandle.__Game_Prefabs_UIAssetCategoryData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			((SystemBase)this).CompleteDependency();
			DynamicBuffer<UIGroupElement> uiGroupElements = default(DynamicBuffer<UIGroupElement>);
			DynamicBuffer<UnlockRequirement> unlockRequirements = default(DynamicBuffer<UnlockRequirement>);
			DynamicBuffer<UIGroupElement> uiGroupElements2 = default(DynamicBuffer<UIGroupElement>);
			DynamicBuffer<UnlockRequirement> unlockRequirements2 = default(DynamicBuffer<UnlockRequirement>);
			for (int i = 0; i < val.Length; i++)
			{
				ArchetypeChunk val2 = val[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray(entityTypeHandle);
				NativeArray<UIObjectData> nativeArray2 = ((ArchetypeChunk)(ref val2)).GetNativeArray<UIObjectData>(ref componentTypeHandle);
				NativeArray<UIAssetCategoryData> nativeArray3 = ((ArchetypeChunk)(ref val2)).GetNativeArray<UIAssetCategoryData>(ref componentTypeHandle2);
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					Entity entity = nativeArray[j];
					UIObjectData uIObjectData = nativeArray2[j];
					if (EntitiesExtensions.TryGetBuffer<UIGroupElement>(((ComponentSystemBase)this).EntityManager, uIObjectData.m_Group, false, ref uiGroupElements))
					{
						RemoveFrom(entity, uiGroupElements);
					}
					if (EntitiesExtensions.TryGetBuffer<UnlockRequirement>(((ComponentSystemBase)this).EntityManager, uIObjectData.m_Group, false, ref unlockRequirements))
					{
						RemoveFrom(entity, unlockRequirements);
					}
				}
				for (int k = 0; k < nativeArray3.Length; k++)
				{
					Entity entity2 = nativeArray[k];
					UIAssetCategoryData uIAssetCategoryData = nativeArray3[k];
					if (EntitiesExtensions.TryGetBuffer<UIGroupElement>(((ComponentSystemBase)this).EntityManager, uIAssetCategoryData.m_Menu, false, ref uiGroupElements2))
					{
						RemoveFrom(entity2, uiGroupElements2);
					}
					if (EntitiesExtensions.TryGetBuffer<UnlockRequirement>(((ComponentSystemBase)this).EntityManager, uIAssetCategoryData.m_Menu, false, ref unlockRequirements2))
					{
						RemoveFrom(entity2, unlockRequirements2);
					}
				}
			}
		}
		finally
		{
			val.Dispose(((SystemBase)this).Dependency);
		}
	}

	private void RemoveFrom(Entity entity, DynamicBuffer<UIGroupElement> uiGroupElements)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < uiGroupElements.Length; i++)
		{
			if (uiGroupElements[i].m_Prefab == entity)
			{
				uiGroupElements.RemoveAtSwapBack(i);
				break;
			}
		}
	}

	private void RemoveFrom(Entity entity, DynamicBuffer<UnlockRequirement> unlockRequirements)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < unlockRequirements.Length; i++)
		{
			if (unlockRequirements[i].m_Prefab == entity)
			{
				unlockRequirements.RemoveAtSwapBack(i);
				break;
			}
		}
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
	public UIInitializeSystem()
	{
	}
}
