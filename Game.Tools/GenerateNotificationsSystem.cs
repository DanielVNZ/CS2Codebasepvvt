using System.Runtime.CompilerServices;
using Game.Common;
using Game.Notifications;
using Game.Prefabs;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class GenerateNotificationsSystem : GameSystemBase
{
	[BurstCompile]
	private struct GenerateIconsJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<CreationDefinition> m_CreationDefinitionType;

		[ReadOnly]
		public ComponentTypeHandle<IconDefinition> m_IconDefinitionType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NotificationIconData> m_NotificationIconData;

		[ReadOnly]
		public EntityArchetype m_DefaultArchetype;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<CreationDefinition> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CreationDefinition>(ref m_CreationDefinitionType);
			NativeArray<IconDefinition> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<IconDefinition>(ref m_IconDefinitionType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				CreationDefinition creationDefinition = nativeArray[i];
				IconDefinition iconDefinition = nativeArray2[i];
				Icon icon = new Icon
				{
					m_Location = iconDefinition.m_Location,
					m_Priority = iconDefinition.m_Priority,
					m_ClusterLayer = iconDefinition.m_ClusterLayer,
					m_Flags = iconDefinition.m_Flags
				};
				PrefabRef prefabRef = new PrefabRef
				{
					m_Prefab = creationDefinition.m_Prefab
				};
				if (creationDefinition.m_Original != Entity.Null && m_PrefabRefData.HasComponent(creationDefinition.m_Original))
				{
					prefabRef.m_Prefab = m_PrefabRefData[creationDefinition.m_Original].m_Prefab;
				}
				if (prefabRef.m_Prefab == Entity.Null || !m_NotificationIconData.HasComponent(prefabRef.m_Prefab))
				{
					continue;
				}
				NotificationIconData notificationIconData = m_NotificationIconData[prefabRef.m_Prefab];
				if (!((EntityArchetype)(ref notificationIconData.m_Archetype)).Valid)
				{
					if ((creationDefinition.m_Flags & CreationFlags.Permanent) != 0)
					{
						continue;
					}
					notificationIconData.m_Archetype = m_DefaultArchetype;
				}
				if (creationDefinition.m_Original != Entity.Null)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Hidden>(unfilteredChunkIndex, creationDefinition.m_Original, default(Hidden));
				}
				Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(unfilteredChunkIndex, notificationIconData.m_Archetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(unfilteredChunkIndex, val, prefabRef);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Icon>(unfilteredChunkIndex, val, icon);
				if ((creationDefinition.m_Flags & CreationFlags.Permanent) == 0)
				{
					Temp temp = new Temp
					{
						m_Original = creationDefinition.m_Original
					};
					temp.m_Flags |= TempFlags.Essential;
					if ((creationDefinition.m_Flags & CreationFlags.Delete) != 0)
					{
						temp.m_Flags |= TempFlags.Delete;
					}
					else if ((creationDefinition.m_Flags & CreationFlags.Select) != 0)
					{
						temp.m_Flags |= TempFlags.Select;
					}
					else
					{
						temp.m_Flags |= TempFlags.Create;
					}
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Temp>(unfilteredChunkIndex, val, temp);
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<DisallowCluster>(unfilteredChunkIndex, val, default(DisallowCluster));
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
		public ComponentTypeHandle<CreationDefinition> __Game_Tools_CreationDefinition_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<IconDefinition> __Game_Tools_IconDefinition_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NotificationIconData> __Game_Prefabs_NotificationIconData_RO_ComponentLookup;

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
			__Game_Tools_CreationDefinition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CreationDefinition>(true);
			__Game_Tools_IconDefinition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<IconDefinition>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_NotificationIconData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NotificationIconData>(true);
		}
	}

	private ModificationBarrier1 m_ModificationBarrier;

	private EntityQuery m_DefinitionQuery;

	private EntityArchetype m_DefaultArchetype;

	private TypeHandle __TypeHandle;

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
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier1>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<CreationDefinition>(),
			ComponentType.ReadOnly<Updated>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<IconDefinition>() };
		array[0] = val;
		m_DefinitionQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_DefaultArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<PrefabRef>(),
			ComponentType.ReadWrite<Icon>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_DefinitionQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		GenerateIconsJob generateIconsJob = new GenerateIconsJob
		{
			m_CreationDefinitionType = InternalCompilerInterface.GetComponentTypeHandle<CreationDefinition>(ref __TypeHandle.__Game_Tools_CreationDefinition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_IconDefinitionType = InternalCompilerInterface.GetComponentTypeHandle<IconDefinition>(ref __TypeHandle.__Game_Tools_IconDefinition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NotificationIconData = InternalCompilerInterface.GetComponentLookup<NotificationIconData>(ref __TypeHandle.__Game_Prefabs_NotificationIconData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DefaultArchetype = m_DefaultArchetype
		};
		EntityCommandBuffer val = m_ModificationBarrier.CreateCommandBuffer();
		generateIconsJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<GenerateIconsJob>(generateIconsJob, m_DefinitionQuery, ((SystemBase)this).Dependency);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val2);
		((SystemBase)this).Dependency = val2;
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
	public GenerateNotificationsSystem()
	{
	}
}
