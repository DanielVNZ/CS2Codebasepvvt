using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Buildings;
using Game.Common;
using Game.Creatures;
using Game.Net;
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
public class DomesticatedAISystem : GameSystemBase
{
	[BurstCompile]
	private struct DomesticatedGroupTickJob : IJobChunk
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
					animal.m_Flags ^= AnimalFlags.Roaming;
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct DomesticatedTickJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Unspawned> m_UnspawnedType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		public ComponentTypeHandle<Animal> m_AnimalType;

		public ComponentTypeHandle<Game.Creatures.Domesticated> m_DomesticatedType;

		public ComponentTypeHandle<AnimalCurrentLane> m_CurrentLaneType;

		public ComponentTypeHandle<AnimalNavigation> m_NavigationType;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<AnimalData> m_PrefabAnimalData;

		[ReadOnly]
		public ComponentLookup<DomesticatedData> m_PrefabDomesticatedData;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLanes;

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
			NativeArray<Game.Creatures.Domesticated> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Creatures.Domesticated>(ref m_DomesticatedType);
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
				Game.Creatures.Domesticated domesticated = nativeArray4[i];
				AnimalNavigation navigation = nativeArray6[i];
				if (!CollectionUtils.TryGet<AnimalCurrentLane>(nativeArray5, i, ref currentLane))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<AnimalCurrentLane>(unfilteredChunkIndex, val, default(AnimalCurrentLane));
				}
				CreatureUtils.CheckUnspawned(unfilteredChunkIndex, val, currentLane, animal, isUnspawned, m_CommandBuffer);
				TickWalking(unfilteredChunkIndex, val, prefabRef, ref random, ref animal, ref domesticated, ref currentLane, ref navigation);
				nativeArray3[i] = animal;
				nativeArray4[i] = domesticated;
				nativeArray6[i] = navigation;
				CollectionUtils.TrySet<AnimalCurrentLane>(nativeArray5, i, currentLane);
			}
		}

		private void TickWalking(int jobIndex, Entity entity, PrefabRef prefabRef, ref Random random, ref Animal animal, ref Game.Creatures.Domesticated domesticated, ref AnimalCurrentLane currentLane, ref AnimalNavigation navigation)
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
				PathEndReached(jobIndex, entity, prefabRef, ref random, ref animal, ref domesticated, ref currentLane, ref navigation);
			}
		}

		private bool PathEndReached(int jobIndex, Entity entity, PrefabRef prefabRef, ref Random random, ref Animal animal, ref Game.Creatures.Domesticated domesticated, ref AnimalCurrentLane currentLane, ref AnimalNavigation navigation)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_071b: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_041e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0435: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_043c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0441: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_045e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0465: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_058e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0590: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_059c: Unknown result type (might be due to invalid IL or missing references)
			//IL_059e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0695: Unknown result type (might be due to invalid IL or missing references)
			//IL_069a: Unknown result type (might be due to invalid IL or missing references)
			//IL_05de: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_052d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0532: Unknown result type (might be due to invalid IL or missing references)
			AnimalData animalData = m_PrefabAnimalData[prefabRef.m_Prefab];
			DomesticatedData domesticatedData = m_PrefabDomesticatedData[prefabRef.m_Prefab];
			if ((domesticated.m_Flags & DomesticatedFlags.Idling) != DomesticatedFlags.None)
			{
				if (--domesticated.m_StateTime > 0)
				{
					return false;
				}
				domesticated.m_Flags &= ~DomesticatedFlags.Idling;
				domesticated.m_Flags |= DomesticatedFlags.Wandering;
			}
			else if ((domesticated.m_Flags & DomesticatedFlags.Wandering) != DomesticatedFlags.None)
			{
				if ((animal.m_Flags & AnimalFlags.FlyingTarget) == 0)
				{
					float num = 3.75f;
					int num2 = Mathf.RoundToInt(domesticatedData.m_IdleTime.min * num);
					int num3 = Mathf.RoundToInt(domesticatedData.m_IdleTime.max * num);
					domesticated.m_StateTime = (ushort)math.clamp(((Random)(ref random)).NextInt(num2, num3 + 1), 0, 65535);
					if (domesticated.m_StateTime > 0)
					{
						domesticated.m_Flags &= ~DomesticatedFlags.Wandering;
						domesticated.m_Flags |= DomesticatedFlags.Idling;
						return false;
					}
				}
			}
			else
			{
				domesticated.m_Flags |= DomesticatedFlags.Wandering;
			}
			Owner owner = default(Owner);
			Transform transform = default(Transform);
			if (m_OwnerData.TryGetComponent(entity, ref owner) && m_TransformData.TryGetComponent(owner.m_Owner, ref transform))
			{
				if ((animal.m_Flags & AnimalFlags.Roaming) != 0)
				{
					Transform transform2 = m_TransformData[entity];
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
					Entity owner2 = owner.m_Owner;
					while (m_OwnerData.HasComponent(owner2) && !m_BuildingData.HasComponent(owner2))
					{
						owner2 = m_OwnerData[owner2].m_Owner;
					}
					float2 val = float2.op_Implicit(16f);
					if (owner2 != Entity.Null)
					{
						PrefabRef prefabRef2 = m_PrefabRefData[owner2];
						ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
						if (m_PrefabObjectGeometryData.TryGetComponent(prefabRef2.m_Prefab, ref objectGeometryData))
						{
							val = ((float3)(ref objectGeometryData.m_Size)).xz;
						}
					}
					float2 val2 = math.length(val) * new float2(0.2f, 0.9f);
					currentLane.m_Flags &= ~(CreatureLaneFlags.EndOfPath | CreatureLaneFlags.EndReached);
					navigation.m_TargetPosition = math.lerp(transform2.m_Position, transform.m_Position, 0.5f);
					ref float3 targetPosition = ref navigation.m_TargetPosition;
					((float3)(ref targetPosition)).xz = ((float3)(ref targetPosition)).xz + ((Random)(ref random)).NextFloat2Direction() * ((Random)(ref random)).NextFloat(val2.x, val2.y);
					if ((animal.m_Flags & AnimalFlags.SwimmingTarget) != 0)
					{
						Bounds1 val3 = WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, navigation.m_TargetPosition) - MathUtils.Invert(animalData.m_SwimDepth);
						navigation.m_TargetPosition.y = ((Random)(ref random)).NextFloat(val3.min, val3.max);
					}
					else if ((animal.m_Flags & AnimalFlags.FlyingTarget) != 0)
					{
						Bounds1 val4 = WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, navigation.m_TargetPosition) + animalData.m_FlyHeight;
						navigation.m_TargetPosition.y = ((Random)(ref random)).NextFloat(val4.min, val4.max);
					}
					else
					{
						if (animalData.m_FlySpeed > 0f)
						{
							float2 val5 = ((float3)(ref navigation.m_TargetPosition)).xz - ((float3)(ref transform2.m_Position)).xz;
							((float3)(ref navigation.m_TargetPosition)).xz = ((float3)(ref transform2.m_Position)).xz + val5 * (animalData.m_MoveSpeed / animalData.m_FlySpeed);
						}
						navigation.m_TargetPosition.y = TerrainUtils.SampleHeight(ref m_TerrainHeightData, navigation.m_TargetPosition);
					}
					return false;
				}
				Lane lane = default(Lane);
				Owner owner3 = default(Owner);
				DynamicBuffer<Game.Net.SubLane> val6 = default(DynamicBuffer<Game.Net.SubLane>);
				if ((currentLane.m_Flags & CreatureLaneFlags.Area) != 0 && m_LaneData.TryGetComponent(currentLane.m_Lane, ref lane) && m_OwnerData.TryGetComponent(currentLane.m_Lane, ref owner3) && m_SubLanes.TryGetBuffer(owner3.m_Owner, ref val6))
				{
					int num4 = 0;
					int num5 = 0;
					Entity a = Entity.Null;
					Entity b = Entity.Null;
					bool a2 = false;
					bool b2 = false;
					Lane lane2 = default(Lane);
					for (int i = 0; i < val6.Length; i++)
					{
						Game.Net.SubLane subLane = val6[i];
						if (subLane.m_SubLane == currentLane.m_Lane || !m_LaneData.TryGetComponent(subLane.m_SubLane, ref lane2))
						{
							continue;
						}
						int num6 = 100;
						if (lane2.m_StartNode.Equals(lane.m_EndNode) || lane2.m_EndNode.Equals(lane.m_EndNode))
						{
							num4 += num6;
							if (((Random)(ref random)).NextInt(num4) < num6)
							{
								a = subLane.m_SubLane;
								a2 = lane2.m_StartNode.Equals(lane.m_EndNode);
							}
						}
						if (lane2.m_StartNode.Equals(lane.m_StartNode) || lane2.m_EndNode.Equals(lane.m_StartNode))
						{
							num5 += num6;
							if (((Random)(ref random)).NextInt(num5) < num6)
							{
								b = subLane.m_SubLane;
								b2 = lane2.m_StartNode.Equals(lane.m_StartNode);
							}
						}
					}
					float num7;
					if ((currentLane.m_Flags & CreatureLaneFlags.Backward) != 0)
					{
						CommonUtils.Swap(ref a, ref b);
						CommonUtils.Swap(ref a2, ref b2);
						num7 = 0f;
					}
					else
					{
						num7 = 1f;
					}
					if (a == Entity.Null)
					{
						a = b;
						a2 = b2;
						num7 = math.select(0f, 1f, num7 == 0f);
					}
					currentLane.m_Flags &= ~(CreatureLaneFlags.EndOfPath | CreatureLaneFlags.EndReached);
					if (a != Entity.Null)
					{
						currentLane.m_NextLane = a;
						currentLane.m_NextPosition.x = math.select(1f, 0f, a2);
						currentLane.m_NextPosition.y = ((Random)(ref random)).NextFloat(0f, 1f);
						currentLane.m_NextFlags = currentLane.m_Flags;
						currentLane.m_CurvePosition.y = num7;
						if (currentLane.m_NextPosition.y > currentLane.m_NextPosition.x)
						{
							currentLane.m_NextFlags &= ~CreatureLaneFlags.Backward;
						}
						else if (currentLane.m_NextPosition.y < currentLane.m_NextPosition.x)
						{
							currentLane.m_NextFlags |= CreatureLaneFlags.Backward;
						}
					}
					else
					{
						currentLane.m_NextLane = Entity.Null;
						currentLane.m_CurvePosition.y = ((Random)(ref random)).NextFloat(0f, 1f);
					}
					if (currentLane.m_CurvePosition.y > currentLane.m_CurvePosition.x)
					{
						currentLane.m_Flags &= ~CreatureLaneFlags.Backward;
					}
					else if (currentLane.m_CurvePosition.y < currentLane.m_CurvePosition.x)
					{
						currentLane.m_Flags |= CreatureLaneFlags.Backward;
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

		public ComponentTypeHandle<Game.Creatures.Domesticated> __Game_Creatures_Domesticated_RW_ComponentTypeHandle;

		public ComponentTypeHandle<AnimalCurrentLane> __Game_Creatures_AnimalCurrentLane_RW_ComponentTypeHandle;

		public ComponentTypeHandle<AnimalNavigation> __Game_Creatures_AnimalNavigation_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Lane> __Game_Net_Lane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AnimalData> __Game_Prefabs_AnimalData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<DomesticatedData> __Game_Prefabs_DomesticatedData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

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
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Objects_Unspawned_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Unspawned>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Creatures_Animal_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Animal>(false);
			__Game_Creatures_Domesticated_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Creatures.Domesticated>(false);
			__Game_Creatures_AnimalCurrentLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AnimalCurrentLane>(false);
			__Game_Creatures_AnimalNavigation_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AnimalNavigation>(false);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Net_Lane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_AnimalData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AnimalData>(true);
			__Game_Prefabs_DomesticatedData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<DomesticatedData>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
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
		return 9;
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
			ComponentType.ReadWrite<Game.Creatures.Domesticated>(),
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
			ComponentType.ReadWrite<Game.Creatures.Domesticated>(),
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
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		JobHandle deps;
		DomesticatedTickJob domesticatedTickJob = new DomesticatedTickJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UnspawnedType = InternalCompilerInterface.GetComponentTypeHandle<Unspawned>(ref __TypeHandle.__Game_Objects_Unspawned_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AnimalType = InternalCompilerInterface.GetComponentTypeHandle<Animal>(ref __TypeHandle.__Game_Creatures_Animal_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DomesticatedType = InternalCompilerInterface.GetComponentTypeHandle<Game.Creatures.Domesticated>(ref __TypeHandle.__Game_Creatures_Domesticated_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<AnimalCurrentLane>(ref __TypeHandle.__Game_Creatures_AnimalCurrentLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NavigationType = InternalCompilerInterface.GetComponentTypeHandle<AnimalNavigation>(ref __TypeHandle.__Game_Creatures_AnimalNavigation_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabAnimalData = InternalCompilerInterface.GetComponentLookup<AnimalData>(ref __TypeHandle.__Game_Prefabs_AnimalData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabDomesticatedData = InternalCompilerInterface.GetComponentLookup<DomesticatedData>(ref __TypeHandle.__Game_Prefabs_DomesticatedData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubLanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RandomSeed = RandomSeed.Next(),
			m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
			m_WaterSurfaceData = m_WaterSystem.GetSurfaceData(out deps)
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		domesticatedTickJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		DomesticatedTickJob domesticatedTickJob2 = domesticatedTickJob;
		DomesticatedGroupTickJob obj = new DomesticatedGroupTickJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UnspawnedType = InternalCompilerInterface.GetComponentTypeHandle<Unspawned>(ref __TypeHandle.__Game_Objects_Unspawned_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_GroupMemberType = InternalCompilerInterface.GetComponentTypeHandle<GroupMember>(ref __TypeHandle.__Game_Creatures_GroupMember_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<AnimalCurrentLane>(ref __TypeHandle.__Game_Creatures_AnimalCurrentLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AnimalData = InternalCompilerInterface.GetComponentLookup<Animal>(ref __TypeHandle.__Game_Creatures_Animal_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CommandBuffer = domesticatedTickJob2.m_CommandBuffer
		};
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<DomesticatedTickJob>(domesticatedTickJob2, m_CreatureQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, deps));
		JobHandle val3 = JobChunkExtensions.ScheduleParallel<DomesticatedGroupTickJob>(obj, m_GroupCreatureQuery, val2);
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
	public DomesticatedAISystem()
	{
	}
}
