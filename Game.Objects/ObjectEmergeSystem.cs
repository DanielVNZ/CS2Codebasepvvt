using System.Runtime.CompilerServices;
using Game.Buildings;
using Game.Citizens;
using Game.Common;
using Game.Creatures;
using Game.Pathfind;
using Game.Prefabs;
using Game.Rendering;
using Game.Tools;
using Game.Vehicles;
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
public class ObjectEmergeSystem : GameSystemBase
{
	[BurstCompile]
	private struct FindObjectsInBuildingJob : IJobChunk
	{
		[ReadOnly]
		public Entity m_Building;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentBuilding> m_CurrentBuildingType;

		[ReadOnly]
		public ComponentTypeHandle<TripSource> m_TripSourceType;

		public ParallelWriter<Entity> m_EmergeQueue;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<CurrentBuilding> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentBuilding>(ref m_CurrentBuildingType);
			if (nativeArray2.Length != 0)
			{
				for (int i = 0; i < nativeArray2.Length; i++)
				{
					if (nativeArray2[i].m_CurrentBuilding == m_Building)
					{
						m_EmergeQueue.Enqueue(nativeArray[i]);
					}
				}
				return;
			}
			NativeArray<TripSource> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TripSource>(ref m_TripSourceType);
			if (nativeArray3.Length == 0)
			{
				return;
			}
			for (int j = 0; j < nativeArray3.Length; j++)
			{
				if (nativeArray3[j].m_Source == m_Building)
				{
					m_EmergeQueue.Enqueue(nativeArray[j]);
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct FindObjectsInVehiclesJob : IJobChunk
	{
		[ReadOnly]
		public BufferTypeHandle<Passenger> m_PassengerType;

		public ParallelWriter<Entity> m_EmergeQueue;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			BufferAccessor<Passenger> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Passenger>(ref m_PassengerType);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				DynamicBuffer<Passenger> val = bufferAccessor[i];
				for (int j = 0; j < val.Length; j++)
				{
					Passenger passenger = val[j];
					m_EmergeQueue.Enqueue(passenger.m_Passenger);
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct EmergeObjectsJob : IJob
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<CreatureData> m_CreatureDataType;

		[ReadOnly]
		public ComponentTypeHandle<PetData> m_PetDataType;

		[ReadOnly]
		public ComponentTypeHandle<ResidentData> m_ResidentDataType;

		[ReadOnly]
		public ComponentLookup<Updated> m_UpdatedData;

		[ReadOnly]
		public ComponentLookup<TripSource> m_TripSourceData;

		[ReadOnly]
		public ComponentLookup<Unspawned> m_UnspawnedData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Citizen> m_CitizenData;

		[ReadOnly]
		public ComponentLookup<HouseholdPet> m_HouseholdPetData;

		[ReadOnly]
		public ComponentLookup<HouseholdMember> m_HouseholdMembers;

		[ReadOnly]
		public ComponentLookup<CurrentBuilding> m_CurrentBuildingData;

		[ReadOnly]
		public ComponentLookup<CurrentTransport> m_CurrentTransportData;

		[ReadOnly]
		public ComponentLookup<HealthProblem> m_HealthProblemData;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> m_CurrentVehicleData;

		[ReadOnly]
		public ComponentLookup<Human> m_HumanData;

		[ReadOnly]
		public ComponentLookup<Animal> m_AnimalData;

		[ReadOnly]
		public ComponentLookup<Game.Creatures.Resident> m_ResidentData;

		[ReadOnly]
		public ComponentLookup<HumanCurrentLane> m_HumanCurrentLaneData;

		[ReadOnly]
		public ComponentLookup<AnimalCurrentLane> m_AnimalCurrentLane;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ObjectData> m_PrefabObjectData;

		[ReadOnly]
		public ComponentLookup<HouseholdPetData> m_PrefabHouseholdPetData;

		[ReadOnly]
		public ComponentLookup<HomelessHousehold> m_HomelessHouseholds;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenters;

		[ReadOnly]
		public ComponentLookup<Deleted> m_Deleteds;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public ComponentTypeSet m_TripSourceRemoveTypes;

		[ReadOnly]
		public ComponentTypeSet m_CurrentVehicleRemoveTypes;

		[ReadOnly]
		public ComponentTypeSet m_CurrentVehicleHumanAddTypes;

		[ReadOnly]
		public ComponentTypeSet m_CurrentVehicleAnimalAddTypes;

		[ReadOnly]
		public ComponentTypeSet m_HumanSpawnTypes;

		[ReadOnly]
		public ComponentTypeSet m_AnimalSpawnTypes;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_CreaturePrefabChunks;

		public NativeQueue<Entity> m_EmergeQueue;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			int count = m_EmergeQueue.Count;
			if (count == 0)
			{
				return;
			}
			for (int i = 0; i < count; i++)
			{
				Entity val = m_EmergeQueue.Dequeue();
				if (m_TripSourceData.HasComponent(val))
				{
					if (!m_UpdatedData.HasComponent(val))
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent(val, ref m_TripSourceRemoveTypes);
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(val, default(BatchesUpdated));
					}
				}
				else if (m_CurrentVehicleData.HasComponent(val))
				{
					ExitVehicle(val, m_CurrentVehicleData[val].m_Vehicle);
				}
				else if (m_CurrentBuildingData.HasComponent(val))
				{
					ExitBuilding(val, m_CurrentBuildingData[val].m_CurrentBuilding);
				}
			}
		}

		private void ExitVehicle(Entity creature, Entity vehicle)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = m_TransformData[creature];
			if (m_TransformData.HasComponent(vehicle))
			{
				transform = m_TransformData[vehicle];
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent(creature, ref m_CurrentVehicleRemoveTypes);
			bool num = m_HumanData.HasComponent(creature);
			bool flag = m_AnimalData.HasComponent(creature);
			CreatureLaneFlags creatureLaneFlags = CreatureLaneFlags.Obsolete;
			if (m_UnspawnedData.HasComponent(vehicle))
			{
				creatureLaneFlags |= CreatureLaneFlags.EmergeUnspawned;
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Unspawned>(creature, default(Unspawned));
			}
			if (num && !m_HumanCurrentLaneData.HasComponent(creature))
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent(creature, ref m_CurrentVehicleHumanAddTypes);
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Transform>(creature, transform);
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<HumanCurrentLane>(creature, new HumanCurrentLane(creatureLaneFlags));
			}
			else if (flag && !m_AnimalCurrentLane.HasComponent(creature))
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent(creature, ref m_CurrentVehicleAnimalAddTypes);
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Transform>(creature, transform);
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<AnimalCurrentLane>(creature, new AnimalCurrentLane(creatureLaneFlags));
			}
			if (m_ResidentData.HasComponent(creature))
			{
				Game.Creatures.Resident resident = m_ResidentData[creature];
				resident.m_Flags &= ~ResidentFlags.InVehicle;
				resident.m_Timer = 0;
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Game.Creatures.Resident>(creature, resident);
			}
		}

		private void ExitBuilding(Entity entity, Entity building)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<CurrentBuilding>(entity);
			if (m_CurrentTransportData.HasComponent(entity))
			{
				CurrentTransport currentTransport = m_CurrentTransportData[entity];
				if (m_TripSourceData.HasComponent(currentTransport.m_CurrentTransport) && !m_UpdatedData.HasComponent(currentTransport.m_CurrentTransport))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent(currentTransport.m_CurrentTransport, ref m_TripSourceRemoveTypes);
				}
			}
			else if (m_CitizenData.HasComponent(entity))
			{
				bool isDead = false;
				HealthProblem healthProblem = default(HealthProblem);
				if (m_HealthProblemData.TryGetComponent(entity, ref healthProblem))
				{
					isDead = (healthProblem.m_Flags & HealthProblemFlags.Dead) != 0;
				}
				Entity householdHomeBuilding = BuildingUtils.GetHouseholdHomeBuilding(m_HouseholdMembers[entity].m_Household, ref m_PropertyRenters, ref m_HomelessHouseholds);
				SpawnResident(entity, building, householdHomeBuilding, isDead);
			}
			else if (m_HouseholdPetData.HasComponent(entity))
			{
				SpawnPet(entity, building);
			}
		}

		private void SpawnResident(Entity citizenEntity, Entity building, Entity homeEntity, bool isDead)
		{
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			if (!isDead)
			{
				CreatureData creatureData;
				PseudoRandomSeed randomSeed;
				Entity val = SelectResidentPrefab(m_CitizenData[citizenEntity], m_CreaturePrefabChunks, m_EntityType, ref m_CreatureDataType, ref m_ResidentDataType, out creatureData, out randomSeed);
				ObjectData objectData = m_PrefabObjectData[val];
				PrefabRef prefabRef = new PrefabRef
				{
					m_Prefab = val
				};
				Transform transform = ((!m_TransformData.HasComponent(building)) ? new Transform(default(float3), quaternion.identity) : m_TransformData[building]);
				Game.Creatures.Resident resident = new Game.Creatures.Resident
				{
					m_Citizen = citizenEntity
				};
				PathOwner pathOwner = new PathOwner(PathFlags.Obsolete);
				TripSource tripSource = new TripSource(building);
				HumanCurrentLane humanCurrentLane = new HumanCurrentLane(CreatureLaneFlags.Obsolete);
				Divert divert = new Divert
				{
					m_Purpose = Purpose.Safety
				};
				Entity val2 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(objectData.m_Archetype);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent(val2, ref m_HumanSpawnTypes);
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Transform>(val2, transform);
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PrefabRef>(val2, prefabRef);
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Game.Creatures.Resident>(val2, resident);
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PathOwner>(val2, pathOwner);
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(val2, randomSeed);
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<HumanCurrentLane>(val2, humanCurrentLane);
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<TripSource>(val2, tripSource);
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Divert>(val2, divert);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CurrentTransport>(citizenEntity, new CurrentTransport(val2));
			}
			Purpose purpose = Purpose.GoingHome;
			if (homeEntity == Entity.Null || m_Deleteds.HasComponent(homeEntity))
			{
				purpose = Purpose.Leisure;
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<TravelPurpose>(citizenEntity, new TravelPurpose
			{
				m_Purpose = purpose
			});
		}

		private void SpawnPet(Entity householdPet, Entity building)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = m_PrefabRefData[householdPet];
			HouseholdPetData householdPetData = m_PrefabHouseholdPetData[prefabRef.m_Prefab];
			Random random = m_RandomSeed.GetRandom(householdPet.Index);
			PseudoRandomSeed randomSeed;
			Entity val = SelectAnimalPrefab(ref random, householdPetData.m_Type, m_CreaturePrefabChunks, m_EntityType, m_PetDataType, out randomSeed);
			ObjectData objectData = m_PrefabObjectData[val];
			Entity val2 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(objectData.m_Archetype);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent(val2, ref m_AnimalSpawnTypes);
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PrefabRef>(val2, new PrefabRef(val));
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Transform>(val2, m_TransformData[building]);
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Game.Creatures.Pet>(val2, new Game.Creatures.Pet(householdPet));
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(val2, randomSeed);
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<TripSource>(val2, new TripSource(building));
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<AnimalCurrentLane>(val2, new AnimalCurrentLane(CreatureLaneFlags.Obsolete));
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CurrentTransport>(householdPet, new CurrentTransport(val2));
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentBuilding> __Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TripSource> __Game_Objects_TripSource_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Passenger> __Game_Vehicles_Passenger_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CreatureData> __Game_Prefabs_CreatureData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PetData> __Game_Prefabs_PetData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ResidentData> __Game_Prefabs_ResidentData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Updated> __Game_Common_Updated_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TripSource> __Game_Objects_TripSource_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Unspawned> __Game_Objects_Unspawned_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HouseholdPet> __Game_Citizens_HouseholdPet_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HouseholdMember> __Game_Citizens_HouseholdMember_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentBuilding> __Game_Citizens_CurrentBuilding_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentTransport> __Game_Citizens_CurrentTransport_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HealthProblem> __Game_Citizens_HealthProblem_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> __Game_Creatures_CurrentVehicle_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Human> __Game_Creatures_Human_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Animal> __Game_Creatures_Animal_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Creatures.Resident> __Game_Creatures_Resident_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HumanCurrentLane> __Game_Creatures_HumanCurrentLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AnimalCurrentLane> __Game_Creatures_AnimalCurrentLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectData> __Game_Prefabs_ObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HouseholdPetData> __Game_Prefabs_HouseholdPetData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HomelessHousehold> __Game_Citizens_HomelessHousehold_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

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
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentBuilding>(true);
			__Game_Objects_TripSource_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TripSource>(true);
			__Game_Vehicles_Passenger_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Passenger>(true);
			__Game_Prefabs_CreatureData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CreatureData>(true);
			__Game_Prefabs_PetData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PetData>(true);
			__Game_Prefabs_ResidentData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ResidentData>(true);
			__Game_Common_Updated_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Updated>(true);
			__Game_Objects_TripSource_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TripSource>(true);
			__Game_Objects_Unspawned_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Unspawned>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
			__Game_Citizens_HouseholdPet_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HouseholdPet>(true);
			__Game_Citizens_HouseholdMember_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HouseholdMember>(true);
			__Game_Citizens_CurrentBuilding_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentBuilding>(true);
			__Game_Citizens_CurrentTransport_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentTransport>(true);
			__Game_Citizens_HealthProblem_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HealthProblem>(true);
			__Game_Creatures_CurrentVehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentVehicle>(true);
			__Game_Creatures_Human_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Human>(true);
			__Game_Creatures_Animal_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Animal>(true);
			__Game_Creatures_Resident_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Creatures.Resident>(true);
			__Game_Creatures_HumanCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HumanCurrentLane>(true);
			__Game_Creatures_AnimalCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AnimalCurrentLane>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectData>(true);
			__Game_Prefabs_HouseholdPetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HouseholdPetData>(true);
			__Game_Citizens_HomelessHousehold_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HomelessHousehold>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
		}
	}

	private ModificationBarrier4B m_ModificationBarrier;

	private EntityQuery m_DeletedBuildingQuery;

	private EntityQuery m_DeletedVehicleQuery;

	private EntityQuery m_EmergeObjectQuery;

	private EntityQuery m_CreaturePrefabQuery;

	private ComponentTypeSet m_TripSourceRemoveTypes;

	private ComponentTypeSet m_CurrentVehicleRemoveTypes;

	private ComponentTypeSet m_CurrentVehicleHumanAddTypes;

	private ComponentTypeSet m_CurrentVehicleAnimalAddTypes;

	private ComponentTypeSet m_HumanSpawnTypes;

	private ComponentTypeSet m_AnimalSpawnTypes;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Expected O, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier4B>();
		m_DeletedBuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Building>(),
			ComponentType.Exclude<Temp>()
		});
		m_DeletedVehicleQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Passenger>(),
			ComponentType.Exclude<Temp>()
		});
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<CurrentBuilding>(),
			ComponentType.ReadOnly<TripSource>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_EmergeObjectQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_CreaturePrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<CreatureData>(),
			ComponentType.ReadOnly<PrefabData>()
		});
		m_TripSourceRemoveTypes = new ComponentTypeSet(ComponentType.ReadWrite<TripSource>(), ComponentType.ReadWrite<Unspawned>());
		m_CurrentVehicleRemoveTypes = new ComponentTypeSet(ComponentType.ReadWrite<CurrentVehicle>(), ComponentType.ReadWrite<Relative>(), ComponentType.ReadWrite<Unspawned>());
		m_CurrentVehicleHumanAddTypes = new ComponentTypeSet((ComponentType[])(object)new ComponentType[7]
		{
			ComponentType.ReadWrite<Moving>(),
			ComponentType.ReadWrite<TransformFrame>(),
			ComponentType.ReadWrite<InterpolatedTransform>(),
			ComponentType.ReadWrite<HumanNavigation>(),
			ComponentType.ReadWrite<HumanCurrentLane>(),
			ComponentType.ReadWrite<Blocker>(),
			ComponentType.ReadWrite<Updated>()
		});
		m_CurrentVehicleAnimalAddTypes = new ComponentTypeSet((ComponentType[])(object)new ComponentType[7]
		{
			ComponentType.ReadWrite<Moving>(),
			ComponentType.ReadWrite<TransformFrame>(),
			ComponentType.ReadWrite<InterpolatedTransform>(),
			ComponentType.ReadWrite<AnimalNavigation>(),
			ComponentType.ReadWrite<AnimalCurrentLane>(),
			ComponentType.ReadWrite<Blocker>(),
			ComponentType.ReadWrite<Updated>()
		});
		m_HumanSpawnTypes = new ComponentTypeSet(ComponentType.ReadWrite<HumanCurrentLane>(), ComponentType.ReadWrite<TripSource>(), ComponentType.ReadWrite<Unspawned>(), ComponentType.ReadWrite<Divert>());
		m_AnimalSpawnTypes = new ComponentTypeSet(ComponentType.ReadWrite<AnimalCurrentLane>(), ComponentType.ReadWrite<TripSource>(), ComponentType.ReadWrite<Unspawned>());
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_0508: Unknown result type (might be due to invalid IL or missing references)
		//IL_050f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0511: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_0519: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_053a: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0544: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_054a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_0554: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		bool flag = !((EntityQuery)(ref m_DeletedBuildingQuery)).IsEmptyIgnoreFilter;
		bool flag2 = !((EntityQuery)(ref m_DeletedVehicleQuery)).IsEmptyIgnoreFilter;
		if (!flag && !flag2)
		{
			return;
		}
		NativeQueue<Entity> emergeQueue = default(NativeQueue<Entity>);
		emergeQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		ParallelWriter<Entity> emergeQueue2 = emergeQueue.AsParallelWriter();
		if (flag)
		{
			NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_DeletedBuildingQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
			try
			{
				EntityTypeHandle entityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
				for (int i = 0; i < val.Length; i++)
				{
					ArchetypeChunk val2 = val[i];
					NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray(entityTypeHandle);
					for (int j = 0; j < nativeArray.Length; j++)
					{
						FindObjectsInBuildingJob findObjectsInBuildingJob = new FindObjectsInBuildingJob
						{
							m_Building = nativeArray[j],
							m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
							m_CurrentBuildingType = InternalCompilerInterface.GetComponentTypeHandle<CurrentBuilding>(ref __TypeHandle.__Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
							m_TripSourceType = InternalCompilerInterface.GetComponentTypeHandle<TripSource>(ref __TypeHandle.__Game_Objects_TripSource_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
							m_EmergeQueue = emergeQueue2
						};
						((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<FindObjectsInBuildingJob>(findObjectsInBuildingJob, m_EmergeObjectQuery, ((SystemBase)this).Dependency);
					}
				}
			}
			finally
			{
				val.Dispose();
			}
		}
		if (flag2)
		{
			FindObjectsInVehiclesJob findObjectsInVehiclesJob = new FindObjectsInVehiclesJob
			{
				m_PassengerType = InternalCompilerInterface.GetBufferTypeHandle<Passenger>(ref __TypeHandle.__Game_Vehicles_Passenger_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EmergeQueue = emergeQueue2
			};
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<FindObjectsInVehiclesJob>(findObjectsInVehiclesJob, m_DeletedVehicleQuery, ((SystemBase)this).Dependency);
		}
		JobHandle val3 = default(JobHandle);
		NativeList<ArchetypeChunk> creaturePrefabChunks = ((EntityQuery)(ref m_CreaturePrefabQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val3);
		JobHandle val4 = IJobExtensions.Schedule<EmergeObjectsJob>(new EmergeObjectsJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CreatureDataType = InternalCompilerInterface.GetComponentTypeHandle<CreatureData>(ref __TypeHandle.__Game_Prefabs_CreatureData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PetDataType = InternalCompilerInterface.GetComponentTypeHandle<PetData>(ref __TypeHandle.__Game_Prefabs_PetData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResidentDataType = InternalCompilerInterface.GetComponentTypeHandle<ResidentData>(ref __TypeHandle.__Game_Prefabs_ResidentData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdatedData = InternalCompilerInterface.GetComponentLookup<Updated>(ref __TypeHandle.__Game_Common_Updated_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TripSourceData = InternalCompilerInterface.GetComponentLookup<TripSource>(ref __TypeHandle.__Game_Objects_TripSource_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UnspawnedData = InternalCompilerInterface.GetComponentLookup<Unspawned>(ref __TypeHandle.__Game_Objects_Unspawned_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CitizenData = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdPetData = InternalCompilerInterface.GetComponentLookup<HouseholdPet>(ref __TypeHandle.__Game_Citizens_HouseholdPet_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdMembers = InternalCompilerInterface.GetComponentLookup<HouseholdMember>(ref __TypeHandle.__Game_Citizens_HouseholdMember_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentBuildingData = InternalCompilerInterface.GetComponentLookup<CurrentBuilding>(ref __TypeHandle.__Game_Citizens_CurrentBuilding_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentTransportData = InternalCompilerInterface.GetComponentLookup<CurrentTransport>(ref __TypeHandle.__Game_Citizens_CurrentTransport_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HealthProblemData = InternalCompilerInterface.GetComponentLookup<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentVehicleData = InternalCompilerInterface.GetComponentLookup<CurrentVehicle>(ref __TypeHandle.__Game_Creatures_CurrentVehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HumanData = InternalCompilerInterface.GetComponentLookup<Human>(ref __TypeHandle.__Game_Creatures_Human_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AnimalData = InternalCompilerInterface.GetComponentLookup<Animal>(ref __TypeHandle.__Game_Creatures_Animal_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResidentData = InternalCompilerInterface.GetComponentLookup<Game.Creatures.Resident>(ref __TypeHandle.__Game_Creatures_Resident_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HumanCurrentLaneData = InternalCompilerInterface.GetComponentLookup<HumanCurrentLane>(ref __TypeHandle.__Game_Creatures_HumanCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AnimalCurrentLane = InternalCompilerInterface.GetComponentLookup<AnimalCurrentLane>(ref __TypeHandle.__Game_Creatures_AnimalCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectData = InternalCompilerInterface.GetComponentLookup<ObjectData>(ref __TypeHandle.__Game_Prefabs_ObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabHouseholdPetData = InternalCompilerInterface.GetComponentLookup<HouseholdPetData>(ref __TypeHandle.__Game_Prefabs_HouseholdPetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HomelessHouseholds = InternalCompilerInterface.GetComponentLookup<HomelessHousehold>(ref __TypeHandle.__Game_Citizens_HomelessHousehold_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyRenters = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Deleteds = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RandomSeed = RandomSeed.Next(),
			m_TripSourceRemoveTypes = m_TripSourceRemoveTypes,
			m_CurrentVehicleRemoveTypes = m_CurrentVehicleRemoveTypes,
			m_CurrentVehicleHumanAddTypes = m_CurrentVehicleHumanAddTypes,
			m_CurrentVehicleAnimalAddTypes = m_CurrentVehicleAnimalAddTypes,
			m_HumanSpawnTypes = m_HumanSpawnTypes,
			m_AnimalSpawnTypes = m_AnimalSpawnTypes,
			m_CreaturePrefabChunks = creaturePrefabChunks,
			m_EmergeQueue = emergeQueue,
			m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer()
		}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val3));
		emergeQueue.Dispose(val4);
		creaturePrefabChunks.Dispose(val4);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val4);
		((SystemBase)this).Dependency = val4;
	}

	public static Entity SelectResidentPrefab(Citizen citizenData, NativeList<ArchetypeChunk> chunks, EntityTypeHandle entityType, ref ComponentTypeHandle<CreatureData> creatureType, ref ComponentTypeHandle<ResidentData> residentType, out CreatureData creatureData, out PseudoRandomSeed randomSeed)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		Random random = citizenData.GetPseudoRandom(CitizenPseudoRandom.SpawnResident);
		GenderMask genderMask = (((citizenData.m_State & CitizenFlags.Male) == 0) ? GenderMask.Female : GenderMask.Male);
		Game.Prefabs.AgeMask ageMask = citizenData.GetAge() switch
		{
			CitizenAge.Child => Game.Prefabs.AgeMask.Child, 
			CitizenAge.Teen => Game.Prefabs.AgeMask.Teen, 
			CitizenAge.Adult => Game.Prefabs.AgeMask.Adult, 
			CitizenAge.Elderly => Game.Prefabs.AgeMask.Elderly, 
			_ => (Game.Prefabs.AgeMask)0, 
		};
		Entity result = Entity.Null;
		int totalProbability = 0;
		creatureData = default(CreatureData);
		randomSeed = new PseudoRandomSeed(ref random);
		for (int i = 0; i < chunks.Length; i++)
		{
			ArchetypeChunk val = chunks[i];
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(entityType);
			NativeArray<CreatureData> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<CreatureData>(ref creatureType);
			NativeArray<ResidentData> nativeArray3 = ((ArchetypeChunk)(ref val)).GetNativeArray<ResidentData>(ref residentType);
			for (int j = 0; j < nativeArray3.Length; j++)
			{
				CreatureData creatureData2 = nativeArray2[j];
				ResidentData residentData = nativeArray3[j];
				if ((creatureData2.m_Gender & genderMask) == genderMask && (residentData.m_Age & ageMask) == ageMask)
				{
					int probability = 100;
					if (SelectItem(ref random, probability, ref totalProbability))
					{
						result = nativeArray[j];
						creatureData = creatureData2;
					}
				}
			}
		}
		return result;
	}

	public static Entity SelectAnimalPrefab(ref Random random, PetType petType, NativeList<ArchetypeChunk> chunks, EntityTypeHandle entityType, ComponentTypeHandle<PetData> petDataType, out PseudoRandomSeed randomSeed)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		int totalProbability = 0;
		Entity result = Entity.Null;
		for (int i = 0; i < chunks.Length; i++)
		{
			ArchetypeChunk val = chunks[i];
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(entityType);
			NativeArray<PetData> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<PetData>(ref petDataType);
			for (int j = 0; j < nativeArray2.Length; j++)
			{
				if (nativeArray2[j].m_Type == petType && SelectItem(ref random, 100, ref totalProbability))
				{
					result = nativeArray[j];
				}
			}
		}
		randomSeed = new PseudoRandomSeed(ref random);
		return result;
	}

	private static bool SelectItem(ref Random random, int probability, ref int totalProbability)
	{
		totalProbability += probability;
		return ((Random)(ref random)).NextInt(totalProbability) < probability;
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
	public ObjectEmergeSystem()
	{
	}
}
