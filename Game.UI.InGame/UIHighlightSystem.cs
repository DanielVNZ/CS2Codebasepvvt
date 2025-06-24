using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.Prefabs;
using Game.Serialization;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class UIHighlightSystem : GameSystemBase, IPreDeserialize
{
	[BurstCompile]
	private struct HighlightJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<Unlock> m_UnlockType;

		[ReadOnly]
		public ComponentLookup<UIObjectData> m_ObjectDatas;

		[ReadOnly]
		public ComponentLookup<UIAssetCategoryData> m_AssetCategories;

		[ReadOnly]
		public ComponentLookup<UIToolbarGroupData> m_ToolbarGroups;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Unlock> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Unlock>(ref m_UnlockType);
			UIObjectData uIObjectData = default(UIObjectData);
			UIAssetCategoryData uIAssetCategoryData = default(UIAssetCategoryData);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Unlock unlock = nativeArray[i];
				if (m_ObjectDatas.TryGetComponent(unlock.m_Prefab, ref uIObjectData))
				{
					Entity val = uIObjectData.m_Group;
					if (m_AssetCategories.TryGetComponent(val, ref uIAssetCategoryData))
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<UIHighlight>(unlock.m_Prefab);
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<UIHighlight>(val);
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<UIHighlight>(uIAssetCategoryData.m_Menu);
					}
					else if (m_ToolbarGroups.HasComponent(val))
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<UIHighlight>(unlock.m_Prefab);
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<Unlock> __Game_Prefabs_Unlock_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<UIObjectData> __Game_Prefabs_UIObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<UIAssetCategoryData> __Game_Prefabs_UIAssetCategoryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<UIToolbarGroupData> __Game_Prefabs_UIToolbarGroupData_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_Unlock_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Unlock>(true);
			__Game_Prefabs_UIObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<UIObjectData>(true);
			__Game_Prefabs_UIAssetCategoryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<UIAssetCategoryData>(true);
			__Game_Prefabs_UIToolbarGroupData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<UIToolbarGroupData>(true);
		}
	}

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_UnlockedPrefabQuery;

	private bool m_SkipUpdate = true;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_UnlockedPrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Unlock>() });
		((ComponentSystemBase)this).RequireForUpdate(m_UnlockedPrefabQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		if (m_SkipUpdate)
		{
			m_SkipUpdate = false;
			return;
		}
		HighlightJob highlightJob = new HighlightJob
		{
			m_UnlockType = InternalCompilerInterface.GetComponentTypeHandle<Unlock>(ref __TypeHandle.__Game_Prefabs_Unlock_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectDatas = InternalCompilerInterface.GetComponentLookup<UIObjectData>(ref __TypeHandle.__Game_Prefabs_UIObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AssetCategories = InternalCompilerInterface.GetComponentLookup<UIAssetCategoryData>(ref __TypeHandle.__Game_Prefabs_UIAssetCategoryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ToolbarGroups = InternalCompilerInterface.GetComponentLookup<UIToolbarGroupData>(ref __TypeHandle.__Game_Prefabs_UIToolbarGroupData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CommandBuffer = m_EndFrameBarrier.CreateCommandBuffer()
		};
		((SystemBase)this).Dependency = JobChunkExtensions.Schedule<HighlightJob>(highlightJob, m_UnlockedPrefabQuery, ((SystemBase)this).Dependency);
		m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
	}

	public void PreDeserialize(Context context)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		EntityQuery val = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<UIHighlight>() });
		try
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).RemoveComponent<UIHighlight>(val);
		}
		finally
		{
			((EntityQuery)(ref val)).Dispose();
		}
		m_SkipUpdate = true;
	}

	public void SkipUpdate()
	{
		m_SkipUpdate = true;
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
	public UIHighlightSystem()
	{
	}
}
