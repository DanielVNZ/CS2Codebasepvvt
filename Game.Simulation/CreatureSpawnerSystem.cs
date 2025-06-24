using System.Runtime.CompilerServices;
using Game.Common;
using Game.Creatures;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class CreatureSpawnerSystem : GameSystemBase
{
	[BurstCompile]
	private struct CreatureSpawnerJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.SpawnLocation> m_SpawnLocationType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		public BufferTypeHandle<OwnedCreature> m_OwnedCreatureType;

		[ReadOnly]
		public ComponentLookup<Creature> m_CreatureData;

		[ReadOnly]
		public ComponentLookup<GroupMember> m_GroupMemberData;

		[ReadOnly]
		public ComponentLookup<Game.Creatures.Domesticated> m_DomesticatedData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<CreatureSpawnData> m_PrefabCreatureSpawnData;

		[ReadOnly]
		public ComponentLookup<SpawnableObjectData> m_PrefabSpawnableObjectData;

		[ReadOnly]
		public ComponentLookup<ObjectData> m_PrefabObjectData;

		[ReadOnly]
		public ComponentLookup<AnimalData> m_PrefabAnimalData;

		[ReadOnly]
		public ComponentLookup<WildlifeData> m_PrefabWildlifeData;

		[ReadOnly]
		public ComponentLookup<DomesticatedData> m_PrefabDomesticatedData;

		[ReadOnly]
		public BufferLookup<PlaceholderObjectElement> m_PrefabPlaceholderObjects;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public ComponentTypeSet m_AnimalSpawnTypes;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Transform> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			NativeArray<Game.Objects.SpawnLocation> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Objects.SpawnLocation>(ref m_SpawnLocationType);
			NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<OwnedCreature> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<OwnedCreature>(ref m_OwnedCreatureType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			SpawnableObjectData spawnableObjectData = default(SpawnableObjectData);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				Entity spawner = nativeArray[i];
				Transform transform = nativeArray2[i];
				PrefabRef prefabRef = nativeArray4[i];
				DynamicBuffer<OwnedCreature> val = bufferAccessor[i];
				CreatureSpawnData creatureSpawnData = m_PrefabCreatureSpawnData[prefabRef.m_Prefab];
				int num = 0;
				Entity group = Entity.Null;
				for (int j = 0; j < val.Length; j++)
				{
					Entity creature = val[j].m_Creature;
					if (m_CreatureData.HasComponent(creature))
					{
						if (!m_GroupMemberData.HasComponent(creature))
						{
							num++;
						}
						if (group == Entity.Null && m_DomesticatedData.HasComponent(creature))
						{
							PrefabRef prefabRef2 = m_PrefabRefData[creature];
							group = ((!m_PrefabSpawnableObjectData.TryGetComponent(prefabRef2.m_Prefab, ref spawnableObjectData)) ? prefabRef2.m_Prefab : spawnableObjectData.m_RandomizationGroup);
						}
					}
					else
					{
						val.RemoveAtSwapBack(j--);
					}
				}
				if (num >= ((Random)(ref random)).NextInt(creatureSpawnData.m_MaxGroupCount + 1))
				{
					continue;
				}
				DynamicBuffer<PlaceholderObjectElement> placeholderObjects = m_PrefabPlaceholderObjects[prefabRef.m_Prefab];
				Game.Objects.SpawnLocation spawnLocation = default(Game.Objects.SpawnLocation);
				if (nativeArray3.Length != 0)
				{
					spawnLocation = nativeArray3[i];
				}
				Entity val2 = SelectPrefab(placeholderObjects, ref random, ref group);
				int num2 = 1;
				if (m_PrefabWildlifeData.HasComponent(val2))
				{
					WildlifeData wildlifeData = m_PrefabWildlifeData[val2];
					num2 = ((Random)(ref random)).NextInt(wildlifeData.m_GroupMemberCount.x, wildlifeData.m_GroupMemberCount.y + 1);
				}
				else if (m_PrefabDomesticatedData.HasComponent(val2))
				{
					DomesticatedData domesticatedData = m_PrefabDomesticatedData[val2];
					num2 = ((Random)(ref random)).NextInt(domesticatedData.m_GroupMemberCount.x, domesticatedData.m_GroupMemberCount.y + 1);
				}
				for (int k = 0; k < num2; k++)
				{
					if (k != 0)
					{
						val2 = SelectPrefab(placeholderObjects, ref random, ref group);
					}
					if (!(val2 == Entity.Null))
					{
						SpawnCreature(unfilteredChunkIndex, spawner, val2, transform, spawnLocation, new PseudoRandomSeed(ref random));
					}
				}
			}
		}

		private Entity SelectPrefab(DynamicBuffer<PlaceholderObjectElement> placeholderObjects, ref Random random, ref Entity group)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			Entity result = Entity.Null;
			Entity val = Entity.Null;
			for (int i = 0; i < placeholderObjects.Length; i++)
			{
				PlaceholderObjectElement placeholderObjectElement = placeholderObjects[i];
				if (!m_PrefabSpawnableObjectData.HasComponent(placeholderObjectElement.m_Object))
				{
					continue;
				}
				SpawnableObjectData spawnableObjectData = m_PrefabSpawnableObjectData[placeholderObjectElement.m_Object];
				Entity val2 = ((spawnableObjectData.m_RandomizationGroup != Entity.Null) ? spawnableObjectData.m_RandomizationGroup : placeholderObjectElement.m_Object);
				if (!(group != Entity.Null) || !(group != val2))
				{
					num += spawnableObjectData.m_Probability;
					if (((Random)(ref random)).NextInt(num) < spawnableObjectData.m_Probability)
					{
						result = placeholderObjectElement.m_Object;
						val = val2;
					}
				}
			}
			group = val;
			return result;
		}

		private void SpawnCreature(int jobIndex, Entity spawner, Entity prefab, Transform transform, Game.Objects.SpawnLocation spawnLocation, PseudoRandomSeed randomSeed)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			ObjectData objectData = m_PrefabObjectData[prefab];
			if (m_PrefabAnimalData.HasComponent(prefab))
			{
				Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, objectData.m_Archetype);
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent(jobIndex, val, ref m_AnimalSpawnTypes);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(jobIndex, val, new PrefabRef(prefab));
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Transform>(jobIndex, val, transform);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, val, randomSeed);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Owner>(jobIndex, val, new Owner(spawner));
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<TripSource>(jobIndex, val, new TripSource(spawner));
				if (spawnLocation.m_ConnectedLane1 == Entity.Null)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Animal>(jobIndex, val, new Animal(AnimalFlags.Roaming));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<AnimalNavigation>(jobIndex, val, new AnimalNavigation(transform.m_Position));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<AnimalCurrentLane>(jobIndex, val, default(AnimalCurrentLane));
				}
				else
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<AnimalCurrentLane>(jobIndex, val, new AnimalCurrentLane(spawnLocation.m_ConnectedLane1, spawnLocation.m_CurvePosition1, (CreatureLaneFlags)0u));
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
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.SpawnLocation> __Game_Objects_SpawnLocation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		public BufferTypeHandle<OwnedCreature> __Game_Creatures_OwnedCreature_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Creature> __Game_Creatures_Creature_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<GroupMember> __Game_Creatures_GroupMember_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Creatures.Domesticated> __Game_Creatures_Domesticated_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CreatureSpawnData> __Game_Prefabs_CreatureSpawnData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableObjectData> __Game_Prefabs_SpawnableObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectData> __Game_Prefabs_ObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AnimalData> __Game_Prefabs_AnimalData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WildlifeData> __Game_Prefabs_WildlifeData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<DomesticatedData> __Game_Prefabs_DomesticatedData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<PlaceholderObjectElement> __Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup;

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
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Objects_SpawnLocation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Objects.SpawnLocation>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Creatures_OwnedCreature_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<OwnedCreature>(false);
			__Game_Creatures_Creature_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Creature>(true);
			__Game_Creatures_GroupMember_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GroupMember>(true);
			__Game_Creatures_Domesticated_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Creatures.Domesticated>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_CreatureSpawnData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CreatureSpawnData>(true);
			__Game_Prefabs_SpawnableObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableObjectData>(true);
			__Game_Prefabs_ObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectData>(true);
			__Game_Prefabs_AnimalData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AnimalData>(true);
			__Game_Prefabs_WildlifeData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WildlifeData>(true);
			__Game_Prefabs_DomesticatedData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<DomesticatedData>(true);
			__Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PlaceholderObjectElement>(true);
		}
	}

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_SpawnerQuery;

	private ComponentTypeSet m_AnimalSpawnTypes;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 512;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_SpawnerQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Game.Creatures.CreatureSpawner>(),
			ComponentType.ReadWrite<OwnedCreature>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Destroyed>(),
			ComponentType.Exclude<Temp>()
		});
		m_AnimalSpawnTypes = new ComponentTypeSet(ComponentType.ReadWrite<AnimalCurrentLane>(), ComponentType.ReadWrite<Owner>(), ComponentType.ReadWrite<TripSource>(), ComponentType.ReadWrite<Unspawned>());
		((ComponentSystemBase)this).RequireForUpdate(m_SpawnerQuery);
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
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		CreatureSpawnerJob creatureSpawnerJob = new CreatureSpawnerJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnLocationType = InternalCompilerInterface.GetComponentTypeHandle<Game.Objects.SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnedCreatureType = InternalCompilerInterface.GetBufferTypeHandle<OwnedCreature>(ref __TypeHandle.__Game_Creatures_OwnedCreature_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CreatureData = InternalCompilerInterface.GetComponentLookup<Creature>(ref __TypeHandle.__Game_Creatures_Creature_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GroupMemberData = InternalCompilerInterface.GetComponentLookup<GroupMember>(ref __TypeHandle.__Game_Creatures_GroupMember_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DomesticatedData = InternalCompilerInterface.GetComponentLookup<Game.Creatures.Domesticated>(ref __TypeHandle.__Game_Creatures_Domesticated_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCreatureSpawnData = InternalCompilerInterface.GetComponentLookup<CreatureSpawnData>(ref __TypeHandle.__Game_Prefabs_CreatureSpawnData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSpawnableObjectData = InternalCompilerInterface.GetComponentLookup<SpawnableObjectData>(ref __TypeHandle.__Game_Prefabs_SpawnableObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectData = InternalCompilerInterface.GetComponentLookup<ObjectData>(ref __TypeHandle.__Game_Prefabs_ObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabAnimalData = InternalCompilerInterface.GetComponentLookup<AnimalData>(ref __TypeHandle.__Game_Prefabs_AnimalData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabWildlifeData = InternalCompilerInterface.GetComponentLookup<WildlifeData>(ref __TypeHandle.__Game_Prefabs_WildlifeData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabDomesticatedData = InternalCompilerInterface.GetComponentLookup<DomesticatedData>(ref __TypeHandle.__Game_Prefabs_DomesticatedData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPlaceholderObjects = InternalCompilerInterface.GetBufferLookup<PlaceholderObjectElement>(ref __TypeHandle.__Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RandomSeed = RandomSeed.Next(),
			m_AnimalSpawnTypes = m_AnimalSpawnTypes
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		creatureSpawnerJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<CreatureSpawnerJob>(creatureSpawnerJob, m_SpawnerQuery, ((SystemBase)this).Dependency);
		m_EndFrameBarrier.AddJobHandleForProducer(val2);
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
	public CreatureSpawnerSystem()
	{
	}
}
