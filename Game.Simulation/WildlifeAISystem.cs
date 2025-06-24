using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
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
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class WildlifeAISystem : GameSystemBase
{
	[BurstCompile]
	private struct WildlifeGroupTickJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Unspawned> m_UnspawnedType;

		[ReadOnly]
		public ComponentTypeHandle<GroupMember> m_GroupMemberType;

		public ComponentTypeHandle<AnimalCurrentLane> m_CurrentLaneType;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Animal> m_AnimalData;

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
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<GroupMember> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<GroupMember>(ref m_GroupMemberType);
			NativeArray<AnimalCurrentLane> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AnimalCurrentLane>(ref m_CurrentLaneType);
			bool isUnspawned = ((ArchetypeChunk)(ref chunk)).Has<Unspawned>(ref m_UnspawnedType);
			AnimalCurrentLane currentLane = default(AnimalCurrentLane);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				GroupMember groupMember = nativeArray2[i];
				if (!CollectionUtils.TryGet<AnimalCurrentLane>(nativeArray3, i, ref currentLane))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<AnimalCurrentLane>(unfilteredChunkIndex, val, default(AnimalCurrentLane));
				}
				Animal animal = m_AnimalData[val];
				CreatureUtils.CheckUnspawned(unfilteredChunkIndex, val, currentLane, animal, isUnspawned, m_CommandBuffer);
				TickGroupMemberWalking(unfilteredChunkIndex, val, groupMember, ref animal, ref currentLane);
				m_AnimalData[val] = animal;
				CollectionUtils.TrySet<AnimalCurrentLane>(nativeArray3, i, currentLane);
			}
		}

		private void TickGroupMemberWalking(int jobIndex, Entity entity, GroupMember groupMember, ref Animal animal, ref AnimalCurrentLane currentLane)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			if (CreatureUtils.IsStuck(currentLane))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, entity, default(Deleted));
			}
			else if (m_AnimalData.HasComponent(groupMember.m_Leader))
			{
				Animal animal2 = m_AnimalData[groupMember.m_Leader];
				animal.m_Flags = (AnimalFlags)(((uint)animal.m_Flags & 0xFFFFFFF9u) | (uint)(animal2.m_Flags & (AnimalFlags.SwimmingTarget | AnimalFlags.FlyingTarget)));
				if (((animal.m_Flags ^ animal2.m_Flags) & AnimalFlags.Roaming) != 0 && (currentLane.m_Flags & CreatureLaneFlags.EndReached) != 0)
				{
					animal.m_Flags = (AnimalFlags)(((uint)animal.m_Flags & 0xFFFFFFFEu) | (uint)(animal2.m_Flags & AnimalFlags.Roaming));
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct WildlifeTickJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Unspawned> m_UnspawnedType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		public ComponentTypeHandle<Animal> m_AnimalType;

		public ComponentTypeHandle<Game.Creatures.Wildlife> m_WildlifeType;

		public ComponentTypeHandle<AnimalCurrentLane> m_CurrentLaneType;

		public ComponentTypeHandle<AnimalNavigation> m_NavigationType;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<AnimalData> m_PrefabAnimalData;

		[ReadOnly]
		public ComponentLookup<WildlifeData> m_PrefabWildlifeData;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[ReadOnly]
		public WaterSurfaceData m_WaterSurfaceData;

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
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<Animal> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Animal>(ref m_AnimalType);
			NativeArray<Game.Creatures.Wildlife> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Creatures.Wildlife>(ref m_WildlifeType);
			NativeArray<AnimalCurrentLane> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AnimalCurrentLane>(ref m_CurrentLaneType);
			NativeArray<AnimalNavigation> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AnimalNavigation>(ref m_NavigationType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			bool isUnspawned = ((ArchetypeChunk)(ref chunk)).Has<Unspawned>(ref m_UnspawnedType);
			AnimalCurrentLane currentLane = default(AnimalCurrentLane);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				PrefabRef prefabRef = nativeArray2[i];
				Animal animal = nativeArray3[i];
				Game.Creatures.Wildlife wildlife = nativeArray4[i];
				AnimalNavigation navigation = nativeArray6[i];
				if (!CollectionUtils.TryGet<AnimalCurrentLane>(nativeArray5, i, ref currentLane))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<AnimalCurrentLane>(unfilteredChunkIndex, val, default(AnimalCurrentLane));
				}
				CreatureUtils.CheckUnspawned(unfilteredChunkIndex, val, currentLane, animal, isUnspawned, m_CommandBuffer);
				TickWalking(unfilteredChunkIndex, val, prefabRef, ref random, ref animal, ref wildlife, ref currentLane, ref navigation);
				nativeArray3[i] = animal;
				nativeArray4[i] = wildlife;
				nativeArray6[i] = navigation;
				CollectionUtils.TrySet<AnimalCurrentLane>(nativeArray5, i, currentLane);
			}
		}

		private void TickWalking(int jobIndex, Entity entity, PrefabRef prefabRef, ref Random random, ref Animal animal, ref Game.Creatures.Wildlife wildlife, ref AnimalCurrentLane currentLane, ref AnimalNavigation navigation)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			if (CreatureUtils.IsStuck(currentLane))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, entity, default(Deleted));
				return;
			}
			if (currentLane.m_Lane != Entity.Null)
			{
				animal.m_Flags &= ~AnimalFlags.Roaming;
			}
			else
			{
				animal.m_Flags |= AnimalFlags.Roaming;
			}
			if (CreatureUtils.PathEndReached(currentLane))
			{
				PathEndReached(jobIndex, entity, prefabRef, ref random, ref animal, ref wildlife, ref currentLane, ref navigation);
			}
		}

		private bool PathEndReached(int jobIndex, Entity entity, PrefabRef prefabRef, ref Random random, ref Animal animal, ref Game.Creatures.Wildlife wildlife, ref AnimalCurrentLane currentLane, ref AnimalNavigation navigation)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			AnimalData animalData = m_PrefabAnimalData[prefabRef.m_Prefab];
			WildlifeData wildlifeData = m_PrefabWildlifeData[prefabRef.m_Prefab];
			if ((wildlife.m_Flags & WildlifeFlags.Idling) != WildlifeFlags.None)
			{
				if (--wildlife.m_StateTime > 0)
				{
					return false;
				}
				wildlife.m_Flags &= ~WildlifeFlags.Idling;
				wildlife.m_Flags |= WildlifeFlags.Wandering;
			}
			else if ((wildlife.m_Flags & WildlifeFlags.Wandering) != WildlifeFlags.None)
			{
				if ((animal.m_Flags & AnimalFlags.FlyingTarget) == 0)
				{
					float num = 3.75f;
					int num2 = Mathf.RoundToInt(wildlifeData.m_IdleTime.min * num);
					int num3 = Mathf.RoundToInt(wildlifeData.m_IdleTime.max * num);
					wildlife.m_StateTime = (ushort)math.clamp(((Random)(ref random)).NextInt(num2, num3 + 1), 0, 65535);
					if (wildlife.m_StateTime > 0)
					{
						wildlife.m_Flags &= ~WildlifeFlags.Wandering;
						wildlife.m_Flags |= WildlifeFlags.Idling;
						return false;
					}
				}
			}
			else
			{
				wildlife.m_Flags |= WildlifeFlags.Wandering;
			}
			if (m_OwnerData.HasComponent(entity))
			{
				Owner owner = m_OwnerData[entity];
				if (m_TransformData.HasComponent(owner.m_Owner))
				{
					Transform transform = m_TransformData[entity];
					Transform transform2 = m_TransformData[owner.m_Owner];
					if ((animal.m_Flags & AnimalFlags.FlyingTarget) != 0)
					{
						if (((Random)(ref random)).NextInt(5) == 0)
						{
							animal.m_Flags &= ~AnimalFlags.FlyingTarget;
						}
					}
					else if (animalData.m_FlySpeed > 0f && (animalData.m_MoveSpeed == 0f || ((Random)(ref random)).NextInt(3) == 0))
					{
						animal.m_Flags |= AnimalFlags.FlyingTarget;
					}
					currentLane.m_Flags &= ~(CreatureLaneFlags.EndOfPath | CreatureLaneFlags.EndReached);
					navigation.m_TargetPosition = math.lerp(transform.m_Position, transform2.m_Position, 0.25f);
					ref float3 targetPosition = ref navigation.m_TargetPosition;
					((float3)(ref targetPosition)).xz = ((float3)(ref targetPosition)).xz + ((Random)(ref random)).NextFloat2Direction() * ((Random)(ref random)).NextFloat(wildlifeData.m_TripLength.min, wildlifeData.m_TripLength.max);
					if ((animal.m_Flags & AnimalFlags.SwimmingTarget) != 0)
					{
						Bounds1 val = WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, navigation.m_TargetPosition) - MathUtils.Invert(animalData.m_SwimDepth);
						navigation.m_TargetPosition.y = ((Random)(ref random)).NextFloat(val.min, val.max);
					}
					else if ((animal.m_Flags & AnimalFlags.FlyingTarget) != 0)
					{
						Bounds1 val2 = WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, navigation.m_TargetPosition) + animalData.m_FlyHeight;
						navigation.m_TargetPosition.y = ((Random)(ref random)).NextFloat(val2.min, val2.max);
					}
					else
					{
						if (animalData.m_FlySpeed > 0f)
						{
							float2 val3 = ((float3)(ref navigation.m_TargetPosition)).xz - ((float3)(ref transform.m_Position)).xz;
							((float3)(ref navigation.m_TargetPosition)).xz = ((float3)(ref transform.m_Position)).xz + val3 * (animalData.m_MoveSpeed / animalData.m_FlySpeed);
						}
						navigation.m_TargetPosition.y = TerrainUtils.SampleHeight(ref m_TerrainHeightData, navigation.m_TargetPosition);
					}
					return false;
				}
			}
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, entity, default(Deleted));
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
		public ComponentTypeHandle<Unspawned> __Game_Objects_Unspawned_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		public ComponentTypeHandle<Animal> __Game_Creatures_Animal_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Game.Creatures.Wildlife> __Game_Creatures_Wildlife_RW_ComponentTypeHandle;

		public ComponentTypeHandle<AnimalCurrentLane> __Game_Creatures_AnimalCurrentLane_RW_ComponentTypeHandle;

		public ComponentTypeHandle<AnimalNavigation> __Game_Creatures_AnimalNavigation_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AnimalData> __Game_Prefabs_AnimalData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WildlifeData> __Game_Prefabs_WildlifeData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<GroupMember> __Game_Creatures_GroupMember_RO_ComponentTypeHandle;

		public ComponentLookup<Animal> __Game_Creatures_Animal_RW_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Objects_Unspawned_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Unspawned>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Creatures_Animal_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Animal>(false);
			__Game_Creatures_Wildlife_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Creatures.Wildlife>(false);
			__Game_Creatures_AnimalCurrentLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AnimalCurrentLane>(false);
			__Game_Creatures_AnimalNavigation_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AnimalNavigation>(false);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Prefabs_AnimalData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AnimalData>(true);
			__Game_Prefabs_WildlifeData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WildlifeData>(true);
			__Game_Creatures_GroupMember_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<GroupMember>(true);
			__Game_Creatures_Animal_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Animal>(false);
		}
	}

	private EndFrameBarrier m_EndFrameBarrier;

	private TerrainSystem m_TerrainSystem;

	private WaterSystem m_WaterSystem;

	private EntityQuery m_CreatureQuery;

	private EntityQuery m_GroupCreatureQuery;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 13;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_CreatureQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[8]
		{
			ComponentType.ReadWrite<Game.Creatures.Wildlife>(),
			ComponentType.ReadWrite<Animal>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.Exclude<GroupMember>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Stumbling>()
		});
		m_GroupCreatureQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[8]
		{
			ComponentType.ReadWrite<Game.Creatures.Wildlife>(),
			ComponentType.ReadWrite<Animal>(),
			ComponentType.ReadOnly<GroupMember>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Stumbling>()
		});
		((ComponentSystemBase)this).RequireAnyForUpdate((EntityQuery[])(object)new EntityQuery[2] { m_CreatureQuery, m_GroupCreatureQuery });
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
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		JobHandle deps;
		WildlifeTickJob wildlifeTickJob = new WildlifeTickJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UnspawnedType = InternalCompilerInterface.GetComponentTypeHandle<Unspawned>(ref __TypeHandle.__Game_Objects_Unspawned_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AnimalType = InternalCompilerInterface.GetComponentTypeHandle<Animal>(ref __TypeHandle.__Game_Creatures_Animal_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WildlifeType = InternalCompilerInterface.GetComponentTypeHandle<Game.Creatures.Wildlife>(ref __TypeHandle.__Game_Creatures_Wildlife_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<AnimalCurrentLane>(ref __TypeHandle.__Game_Creatures_AnimalCurrentLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NavigationType = InternalCompilerInterface.GetComponentTypeHandle<AnimalNavigation>(ref __TypeHandle.__Game_Creatures_AnimalNavigation_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabAnimalData = InternalCompilerInterface.GetComponentLookup<AnimalData>(ref __TypeHandle.__Game_Prefabs_AnimalData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabWildlifeData = InternalCompilerInterface.GetComponentLookup<WildlifeData>(ref __TypeHandle.__Game_Prefabs_WildlifeData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RandomSeed = RandomSeed.Next(),
			m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
			m_WaterSurfaceData = m_WaterSystem.GetSurfaceData(out deps)
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		wildlifeTickJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		WildlifeTickJob wildlifeTickJob2 = wildlifeTickJob;
		WildlifeGroupTickJob obj = new WildlifeGroupTickJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UnspawnedType = InternalCompilerInterface.GetComponentTypeHandle<Unspawned>(ref __TypeHandle.__Game_Objects_Unspawned_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_GroupMemberType = InternalCompilerInterface.GetComponentTypeHandle<GroupMember>(ref __TypeHandle.__Game_Creatures_GroupMember_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<AnimalCurrentLane>(ref __TypeHandle.__Game_Creatures_AnimalCurrentLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AnimalData = InternalCompilerInterface.GetComponentLookup<Animal>(ref __TypeHandle.__Game_Creatures_Animal_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CommandBuffer = wildlifeTickJob2.m_CommandBuffer
		};
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<WildlifeTickJob>(wildlifeTickJob2, m_CreatureQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, deps));
		JobHandle val3 = JobChunkExtensions.ScheduleParallel<WildlifeGroupTickJob>(obj, m_GroupCreatureQuery, val2);
		m_TerrainSystem.AddCPUHeightReader(val2);
		m_WaterSystem.AddSurfaceReader(val2);
		m_EndFrameBarrier.AddJobHandleForProducer(val3);
		((SystemBase)this).Dependency = val3;
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
	public WildlifeAISystem()
	{
	}
}
