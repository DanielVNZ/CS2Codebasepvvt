using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Serialization.Entities;
using Game.Areas;
using Game.City;
using Game.Common;
using Game.Prefabs;
using Game.Serialization;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Objects;

[CompilerGenerated]
public class PlaceholderSystem : GameSystemBase
{
	[BurstCompile]
	private struct PlaceholderJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<Node> m_AreaNodeType;

		[ReadOnly]
		public ComponentLookup<Placeholder> m_PlaceholderData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<SpawnableObjectData> m_PrefabSpawnableObjectData;

		[ReadOnly]
		public BufferLookup<PlaceholderObjectElement> m_PrefabPlaceholderElements;

		[ReadOnly]
		public BufferLookup<ObjectRequirementElement> m_PrefabRequirementElements;

		[ReadOnly]
		public Entity m_Theme;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Owner> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<Transform> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			if (nativeArray.Length != 0 && nativeArray2.Length != 0)
			{
				Owner owner2 = default(Owner);
				for (int i = 0; i < nativeArray.Length; i++)
				{
					Owner owner = nativeArray[i];
					while (m_OwnerData.TryGetComponent(owner.m_Owner, ref owner2))
					{
						owner = owner2;
					}
					if (!m_PlaceholderData.HasComponent(owner.m_Owner))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(unfilteredChunkIndex, owner.m_Owner, default(Updated));
					}
				}
				return;
			}
			NativeArray<Entity> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<Node> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Node>(ref m_AreaNodeType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			DynamicBuffer<PlaceholderObjectElement> val3 = default(DynamicBuffer<PlaceholderObjectElement>);
			Owner owner3 = default(Owner);
			Transform transform = default(Transform);
			DynamicBuffer<Node> val5 = default(DynamicBuffer<Node>);
			for (int j = 0; j < nativeArray4.Length; j++)
			{
				Entity val = nativeArray3[j];
				PrefabRef prefabRef = nativeArray4[j];
				Entity val2 = Entity.Null;
				if (m_PrefabPlaceholderElements.TryGetBuffer(prefabRef.m_Prefab, ref val3))
				{
					int num = 0;
					for (int k = 0; k < val3.Length; k++)
					{
						PlaceholderObjectElement placeholder = val3[k];
						if (GetVariationProbability(placeholder, out var probability))
						{
							num += probability;
							if (((Random)(ref random)).NextInt(num) < probability)
							{
								val2 = placeholder.m_Object;
							}
						}
					}
				}
				CreationDefinition creationDefinition;
				if (val2 != Entity.Null)
				{
					creationDefinition = new CreationDefinition
					{
						m_Prefab = val2
					};
					creationDefinition.m_Flags |= CreationFlags.Permanent | CreationFlags.Native;
					creationDefinition.m_RandomSeed = ((Random)(ref random)).NextInt();
					if (CollectionUtils.TryGet<Owner>(nativeArray, j, ref owner3))
					{
						creationDefinition.m_Owner = owner3.m_Owner;
						while (!m_PlaceholderData.HasComponent(owner3.m_Owner))
						{
							Entity owner4 = owner3.m_Owner;
							if (m_OwnerData.TryGetComponent(owner4, ref owner3))
							{
								continue;
							}
							goto IL_01dc;
						}
						continue;
					}
					goto IL_01dc;
				}
				goto IL_02ca;
				IL_01dc:
				Entity val4 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(unfilteredChunkIndex);
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(unfilteredChunkIndex, val4, creationDefinition);
				if (CollectionUtils.TryGet<Transform>(nativeArray2, j, ref transform))
				{
					ObjectDefinition objectDefinition = new ObjectDefinition
					{
						m_ParentMesh = -1,
						m_Position = transform.m_Position,
						m_Rotation = transform.m_Rotation,
						m_LocalPosition = transform.m_Position,
						m_LocalRotation = transform.m_Rotation
					};
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<ObjectDefinition>(unfilteredChunkIndex, val4, objectDefinition);
				}
				if (CollectionUtils.TryGet<Node>(bufferAccessor, j, ref val5))
				{
					DynamicBuffer<Node> val6 = ((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<Node>(unfilteredChunkIndex, val4);
					if (val5.Length != 0)
					{
						val6.Capacity = val5.Length + 1;
						val6.AddRange(val5.AsNativeArray());
						val6.Add(val5[0]);
					}
				}
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(unfilteredChunkIndex, val4, default(Updated));
				goto IL_02ca;
				IL_02ca:
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, val, default(Deleted));
			}
		}

		private bool GetVariationProbability(PlaceholderObjectElement placeholder, out int probability)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			probability = 100;
			DynamicBuffer<ObjectRequirementElement> val = default(DynamicBuffer<ObjectRequirementElement>);
			if (m_PrefabRequirementElements.TryGetBuffer(placeholder.m_Object, ref val))
			{
				int num = -1;
				bool flag = true;
				for (int i = 0; i < val.Length; i++)
				{
					ObjectRequirementElement objectRequirementElement = val[i];
					if (objectRequirementElement.m_Group != num)
					{
						if (!flag)
						{
							break;
						}
						num = objectRequirementElement.m_Group;
						flag = false;
					}
					flag |= m_Theme == objectRequirementElement.m_Requirement;
				}
				if (!flag)
				{
					return false;
				}
			}
			SpawnableObjectData spawnableObjectData = default(SpawnableObjectData);
			if (m_PrefabSpawnableObjectData.TryGetComponent(placeholder.m_Object, ref spawnableObjectData))
			{
				probability = spawnableObjectData.m_Probability;
			}
			return true;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Node> __Game_Areas_Node_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Placeholder> __Game_Objects_Placeholder_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableObjectData> __Game_Prefabs_SpawnableObjectData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<PlaceholderObjectElement> __Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ObjectRequirementElement> __Game_Prefabs_ObjectRequirementElement_RO_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Areas_Node_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Node>(true);
			__Game_Objects_Placeholder_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Placeholder>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Prefabs_SpawnableObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableObjectData>(true);
			__Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PlaceholderObjectElement>(true);
			__Game_Prefabs_ObjectRequirementElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ObjectRequirementElement>(true);
		}
	}

	private LoadGameSystem m_LoadGameSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private EntityQuery m_EntityQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_LoadGameSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LoadGameSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_EntityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Placeholder>() });
		((ComponentSystemBase)this).RequireForUpdate(m_EntityQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		Context context = m_LoadGameSystem.context;
		if ((int)((Context)(ref context)).purpose == 1)
		{
			EntityCommandBuffer val = default(EntityCommandBuffer);
			((EntityCommandBuffer)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			JobHandle val2 = JobChunkExtensions.ScheduleParallel<PlaceholderJob>(new PlaceholderJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_AreaNodeType = InternalCompilerInterface.GetBufferTypeHandle<Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PlaceholderData = InternalCompilerInterface.GetComponentLookup<Placeholder>(ref __TypeHandle.__Game_Objects_Placeholder_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabSpawnableObjectData = InternalCompilerInterface.GetComponentLookup<SpawnableObjectData>(ref __TypeHandle.__Game_Prefabs_SpawnableObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabPlaceholderElements = InternalCompilerInterface.GetBufferLookup<PlaceholderObjectElement>(ref __TypeHandle.__Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRequirementElements = InternalCompilerInterface.GetBufferLookup<ObjectRequirementElement>(ref __TypeHandle.__Game_Prefabs_ObjectRequirementElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Theme = m_CityConfigurationSystem.defaultTheme,
				m_RandomSeed = RandomSeed.Next(),
				m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter()
			}, m_EntityQuery, ((SystemBase)this).Dependency);
			((JobHandle)(ref val2)).Complete();
			((EntityCommandBuffer)(ref val)).Playback(((ComponentSystemBase)this).EntityManager);
			((EntityCommandBuffer)(ref val)).Dispose();
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
	public PlaceholderSystem()
	{
	}
}
