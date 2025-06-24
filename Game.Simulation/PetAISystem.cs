using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Creatures;
using Game.Events;
using Game.Net;
using Game.Objects;
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

namespace Game.Simulation;

[CompilerGenerated]
public class PetAISystem : GameSystemBase
{
	private struct Boarding
	{
		public Entity m_Passenger;

		public Entity m_Vehicle;

		public AnimalCurrentLane m_CurrentLane;

		public CreatureVehicleFlags m_Flags;

		public float3 m_Position;

		public BoardingType m_Type;

		public static Boarding ExitVehicle(Entity passenger, Entity vehicle, AnimalCurrentLane newCurrentLane, float3 position)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			return new Boarding
			{
				m_Passenger = passenger,
				m_Vehicle = vehicle,
				m_CurrentLane = newCurrentLane,
				m_Position = position,
				m_Type = BoardingType.Exit
			};
		}

		public static Boarding TryEnterVehicle(Entity passenger, Entity vehicle, CreatureVehicleFlags flags)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			return new Boarding
			{
				m_Passenger = passenger,
				m_Vehicle = vehicle,
				m_Flags = flags,
				m_Type = BoardingType.TryEnter
			};
		}

		public static Boarding FinishEnterVehicle(Entity passenger, Entity vehicle, AnimalCurrentLane oldCurrentLane)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			return new Boarding
			{
				m_Passenger = passenger,
				m_Vehicle = vehicle,
				m_CurrentLane = oldCurrentLane,
				m_Type = BoardingType.FinishEnter
			};
		}

		public static Boarding CancelEnterVehicle(Entity passenger, Entity vehicle)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			return new Boarding
			{
				m_Passenger = passenger,
				m_Vehicle = vehicle,
				m_Type = BoardingType.CancelEnter
			};
		}
	}

	private enum BoardingType
	{
		Exit,
		TryEnter,
		FinishEnter,
		CancelEnter
	}

	[BurstCompile]
	private struct PetTickJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentVehicle> m_CurrentVehicleType;

		[ReadOnly]
		public ComponentTypeHandle<GroupMember> m_GroupMemberType;

		[ReadOnly]
		public ComponentTypeHandle<Unspawned> m_UnspawnedType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<GroupCreature> m_GroupCreatureType;

		public ComponentTypeHandle<Animal> m_AnimalType;

		public ComponentTypeHandle<Game.Creatures.Pet> m_PetType;

		public ComponentTypeHandle<Creature> m_CreatureType;

		public ComponentTypeHandle<AnimalCurrentLane> m_CurrentLaneType;

		public ComponentTypeHandle<Target> m_TargetType;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> m_CurrentVehicleData;

		[ReadOnly]
		public ComponentLookup<Destroyed> m_DestroyedData;

		[ReadOnly]
		public ComponentLookup<Unspawned> m_UnspawnedData;

		[ReadOnly]
		public ComponentLookup<Moving> m_MovingData;

		[ReadOnly]
		public ComponentLookup<Game.Creatures.Resident> m_ResidentData;

		[ReadOnly]
		public ComponentLookup<OnFire> m_OnFireData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PersonalCar> m_PersonalCarData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.Taxi> m_TaxiData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PublicTransport> m_PublicTransportData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PoliceCar> m_PoliceCarData;

		[ReadOnly]
		public ComponentLookup<Controller> m_ControllerData;

		[ReadOnly]
		public ComponentLookup<Vehicle> m_VehicleData;

		[ReadOnly]
		public ComponentLookup<Train> m_TrainData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenterData;

		[ReadOnly]
		public ComponentLookup<PathOwner> m_PathOwnerData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<CarData> m_PrefabCarData;

		[ReadOnly]
		public BufferLookup<ActivityLocationElement> m_PrefabActivityLocationElements;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public bool m_LefthandTraffic;

		[ReadOnly]
		public EntityArchetype m_ResetTripArchetype;

		public ParallelWriter<Boarding> m_BoardingQueue;

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
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<Game.Creatures.Pet> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Creatures.Pet>(ref m_PetType);
			NativeArray<Creature> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Creature>(ref m_CreatureType);
			NativeArray<Target> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Target>(ref m_TargetType);
			NativeArray<GroupMember> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<GroupMember>(ref m_GroupMemberType);
			NativeArray<CurrentVehicle> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentVehicle>(ref m_CurrentVehicleType);
			NativeArray<AnimalCurrentLane> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AnimalCurrentLane>(ref m_CurrentLaneType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			if (nativeArray7.Length != 0)
			{
				if (nativeArray6.Length != 0)
				{
					AnimalCurrentLane currentLane = default(AnimalCurrentLane);
					for (int i = 0; i < nativeArray.Length; i++)
					{
						Entity entity = nativeArray[i];
						PrefabRef prefabRef = nativeArray2[i];
						Game.Creatures.Pet pet = nativeArray3[i];
						Creature creature = nativeArray4[i];
						CurrentVehicle currentVehicle = nativeArray7[i];
						Target target = nativeArray5[i];
						GroupMember groupMember = nativeArray6[i];
						CollectionUtils.TryGet<AnimalCurrentLane>(nativeArray8, i, ref currentLane);
						TickGroupMemberInVehicle(ref random, unfilteredChunkIndex, entity, prefabRef, groupMember, currentVehicle, nativeArray8.Length != 0, ref pet, ref currentLane, ref target);
						TickQueue(ref creature, ref currentLane);
						nativeArray3[i] = pet;
						nativeArray4[i] = creature;
						nativeArray5[i] = target;
						CollectionUtils.TrySet<AnimalCurrentLane>(nativeArray8, i, currentLane);
					}
					return;
				}
				BufferAccessor<GroupCreature> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<GroupCreature>(ref m_GroupCreatureType);
				AnimalCurrentLane currentLane2 = default(AnimalCurrentLane);
				DynamicBuffer<GroupCreature> groupCreatures = default(DynamicBuffer<GroupCreature>);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Entity entity2 = nativeArray[j];
					PrefabRef prefabRef2 = nativeArray2[j];
					Game.Creatures.Pet pet2 = nativeArray3[j];
					Creature creature2 = nativeArray4[j];
					CurrentVehicle currentVehicle2 = nativeArray7[j];
					Target target2 = nativeArray5[j];
					CollectionUtils.TryGet<AnimalCurrentLane>(nativeArray8, j, ref currentLane2);
					CollectionUtils.TryGet<GroupCreature>(bufferAccessor, j, ref groupCreatures);
					TickInVehicle(ref random, unfilteredChunkIndex, entity2, prefabRef2, currentVehicle2, nativeArray8.Length != 0, ref pet2, ref currentLane2, ref target2, groupCreatures);
					TickQueue(ref creature2, ref currentLane2);
					nativeArray3[j] = pet2;
					nativeArray4[j] = creature2;
					nativeArray5[j] = target2;
					CollectionUtils.TrySet<AnimalCurrentLane>(nativeArray8, j, currentLane2);
				}
				return;
			}
			NativeArray<Animal> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Animal>(ref m_AnimalType);
			bool isUnspawned = ((ArchetypeChunk)(ref chunk)).Has<Unspawned>(ref m_UnspawnedType);
			if (nativeArray6.Length != 0)
			{
				AnimalCurrentLane currentLane3 = default(AnimalCurrentLane);
				for (int k = 0; k < nativeArray.Length; k++)
				{
					Entity entity3 = nativeArray[k];
					PrefabRef prefabRef3 = nativeArray2[k];
					Animal animal = nativeArray9[k];
					Game.Creatures.Pet pet3 = nativeArray3[k];
					Creature creature3 = nativeArray4[k];
					Target target3 = nativeArray5[k];
					GroupMember groupMember2 = nativeArray6[k];
					CollectionUtils.TryGet<AnimalCurrentLane>(nativeArray8, k, ref currentLane3);
					CreatureUtils.CheckUnspawned(unfilteredChunkIndex, entity3, currentLane3, animal, isUnspawned, m_CommandBuffer);
					TickGroupMemberWalking(unfilteredChunkIndex, entity3, prefabRef3, groupMember2, ref pet3, ref creature3, ref currentLane3, ref target3);
					TickQueue(ref creature3, ref currentLane3);
					nativeArray9[k] = animal;
					nativeArray3[k] = pet3;
					nativeArray4[k] = creature3;
					nativeArray5[k] = target3;
					CollectionUtils.TrySet<AnimalCurrentLane>(nativeArray8, k, currentLane3);
				}
			}
			else
			{
				AnimalCurrentLane currentLane4 = default(AnimalCurrentLane);
				for (int l = 0; l < nativeArray.Length; l++)
				{
					Entity entity4 = nativeArray[l];
					PrefabRef prefabRef4 = nativeArray2[l];
					Animal animal2 = nativeArray9[l];
					Game.Creatures.Pet pet4 = nativeArray3[l];
					Creature creature4 = nativeArray4[l];
					Target target4 = nativeArray5[l];
					CollectionUtils.TryGet<AnimalCurrentLane>(nativeArray8, l, ref currentLane4);
					CreatureUtils.CheckUnspawned(unfilteredChunkIndex, entity4, currentLane4, animal2, isUnspawned, m_CommandBuffer);
					TickWalking(unfilteredChunkIndex, entity4, prefabRef4, ref pet4, ref creature4, ref currentLane4, ref target4);
					TickQueue(ref creature4, ref currentLane4);
					nativeArray9[l] = animal2;
					nativeArray3[l] = pet4;
					nativeArray4[l] = creature4;
					nativeArray5[l] = target4;
					CollectionUtils.TrySet<AnimalCurrentLane>(nativeArray8, l, currentLane4);
				}
			}
		}

		private void TickGroupMemberInVehicle(ref Random random, int jobIndex, Entity entity, PrefabRef prefabRef, GroupMember groupMember, CurrentVehicle currentVehicle, bool hasCurrentLane, ref Game.Creatures.Pet pet, ref AnimalCurrentLane currentLane, ref Target target)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			if (!m_PrefabRefData.HasComponent(currentVehicle.m_Vehicle))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, entity, default(Deleted));
				return;
			}
			Entity val = currentVehicle.m_Vehicle;
			if (m_ControllerData.HasComponent(currentVehicle.m_Vehicle))
			{
				Controller controller = m_ControllerData[currentVehicle.m_Vehicle];
				if (controller.m_Controller != Entity.Null)
				{
					val = controller.m_Controller;
				}
			}
			if ((currentVehicle.m_Flags & CreatureVehicleFlags.Ready) == 0)
			{
				if (hasCurrentLane)
				{
					if (CreatureUtils.IsStuck(currentLane))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, entity, default(Deleted));
						return;
					}
					if (!m_CurrentVehicleData.HasComponent(groupMember.m_Leader))
					{
						CancelEnterVehicle(entity, currentVehicle.m_Vehicle, ref currentLane);
						return;
					}
					Game.Vehicles.PublicTransport publicTransport = default(Game.Vehicles.PublicTransport);
					if (m_PublicTransportData.TryGetComponent(val, ref publicTransport) && (publicTransport.m_State & PublicTransportFlags.Boarding) == 0 && currentLane.m_Lane == currentVehicle.m_Vehicle)
					{
						currentLane.m_Flags |= CreatureLaneFlags.EndOfPath | CreatureLaneFlags.EndReached;
					}
					Game.Creatures.Resident resident = default(Game.Creatures.Resident);
					m_ResidentData.TryGetComponent(groupMember.m_Leader, ref resident);
					if (CreatureUtils.PathEndReached(currentLane) || resident.m_Timer >= 250)
					{
						FinishEnterVehicle(entity, currentVehicle.m_Vehicle, ref currentLane);
						hasCurrentLane = false;
					}
				}
				if (!hasCurrentLane)
				{
					currentVehicle.m_Flags |= CreatureVehicleFlags.Ready;
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CurrentVehicle>(jobIndex, entity, currentVehicle);
				}
			}
			else
			{
				if ((pet.m_Flags & PetFlags.Disembarking) == 0 && !m_CurrentVehicleData.HasComponent(groupMember.m_Leader))
				{
					GroupLeaderDisembarking(entity, ref pet);
				}
				if ((pet.m_Flags & PetFlags.Disembarking) != PetFlags.None)
				{
					ExitVehicle(ref random, entity, val, prefabRef, currentVehicle);
				}
			}
		}

		private void TickInVehicle(ref Random random, int jobIndex, Entity entity, PrefabRef prefabRef, CurrentVehicle currentVehicle, bool hasCurrentLane, ref Game.Creatures.Pet pet, ref AnimalCurrentLane currentLane, ref Target target, DynamicBuffer<GroupCreature> groupCreatures)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			if (!m_PrefabRefData.HasComponent(currentVehicle.m_Vehicle))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, entity, default(Deleted));
				return;
			}
			Entity val = currentVehicle.m_Vehicle;
			if (m_ControllerData.HasComponent(currentVehicle.m_Vehicle))
			{
				Controller controller = m_ControllerData[currentVehicle.m_Vehicle];
				if (controller.m_Controller != Entity.Null)
				{
					val = controller.m_Controller;
				}
			}
			if ((currentVehicle.m_Flags & CreatureVehicleFlags.Ready) == 0)
			{
				if (hasCurrentLane)
				{
					if (CreatureUtils.IsStuck(currentLane))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, entity, default(Deleted));
						return;
					}
					Game.Vehicles.PublicTransport publicTransport = default(Game.Vehicles.PublicTransport);
					if (m_PublicTransportData.TryGetComponent(val, ref publicTransport) && (publicTransport.m_State & PublicTransportFlags.Boarding) == 0 && currentLane.m_Lane == currentVehicle.m_Vehicle)
					{
						currentLane.m_Flags |= CreatureLaneFlags.EndOfPath | CreatureLaneFlags.EndReached;
					}
					if (CreatureUtils.PathEndReached(currentLane))
					{
						FinishEnterVehicle(entity, currentVehicle.m_Vehicle, ref currentLane);
						hasCurrentLane = false;
					}
				}
				if (!hasCurrentLane && HasEveryoneBoarded(groupCreatures))
				{
					currentVehicle.m_Flags |= CreatureVehicleFlags.Ready;
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CurrentVehicle>(jobIndex, entity, currentVehicle);
				}
				return;
			}
			if ((pet.m_Flags & PetFlags.Disembarking) == 0)
			{
				if (m_DestroyedData.HasComponent(val))
				{
					if (!m_MovingData.HasComponent(val))
					{
						pet.m_Flags |= PetFlags.Disembarking;
					}
				}
				else if (m_PersonalCarData.HasComponent(val))
				{
					if ((m_PersonalCarData[val].m_State & PersonalCarFlags.Disembarking) != 0)
					{
						CurrentVehicleDisembarking(jobIndex, entity, val, ref pet, ref target);
					}
				}
				else if (m_PublicTransportData.HasComponent(val))
				{
					if ((m_PublicTransportData[val].m_State & PublicTransportFlags.Boarding) != 0)
					{
						CurrentVehicleBoarding(jobIndex, entity, val, ref pet, ref target);
					}
				}
				else if (m_TaxiData.HasComponent(val))
				{
					if ((m_TaxiData[val].m_State & TaxiFlags.Disembarking) != 0)
					{
						CurrentVehicleDisembarking(jobIndex, entity, val, ref pet, ref target);
					}
				}
				else if (m_PoliceCarData.HasComponent(val) && (m_PoliceCarData[val].m_State & PoliceCarFlags.Disembarking) != 0)
				{
					CurrentVehicleDisembarking(jobIndex, entity, val, ref pet, ref target);
				}
			}
			if ((pet.m_Flags & PetFlags.Disembarking) != PetFlags.None)
			{
				ExitVehicle(ref random, entity, val, prefabRef, currentVehicle);
			}
		}

		private void TickGroupMemberWalking(int jobIndex, Entity entity, PrefabRef prefabRef, GroupMember groupMember, ref Game.Creatures.Pet pet, ref Creature creature, ref AnimalCurrentLane currentLane, ref Target target)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			if ((pet.m_Flags & PetFlags.Disembarking) != PetFlags.None)
			{
				pet.m_Flags &= ~PetFlags.Disembarking;
			}
			else if (!m_PrefabRefData.HasComponent(target.m_Target) || CreatureUtils.IsStuck(currentLane))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, entity, default(Deleted));
				return;
			}
			if (m_CurrentVehicleData.HasComponent(groupMember.m_Leader) && (currentLane.m_Flags & CreatureLaneFlags.EndReached) != 0)
			{
				CurrentVehicle currentVehicle = m_CurrentVehicleData[groupMember.m_Leader];
				m_BoardingQueue.Enqueue(Boarding.TryEnterVehicle(entity, currentVehicle.m_Vehicle, (CreatureVehicleFlags)0u));
			}
		}

		private void TickWalking(int jobIndex, Entity entity, PrefabRef prefabRef, ref Game.Creatures.Pet pet, ref Creature creature, ref AnimalCurrentLane currentLane, ref Target target)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			if (!CheckTarget(entity, ref currentLane, ref target))
			{
				if ((pet.m_Flags & PetFlags.Disembarking) != PetFlags.None)
				{
					pet.m_Flags &= ~PetFlags.Disembarking;
				}
				else if (!m_PrefabRefData.HasComponent(target.m_Target) || CreatureUtils.IsStuck(currentLane))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, entity, default(Deleted));
				}
				else if (CreatureUtils.PathEndReached(currentLane))
				{
					PathEndReached(jobIndex, entity, ref pet, ref currentLane, ref target);
				}
			}
		}

		private void TickQueue(ref Creature creature, ref AnimalCurrentLane currentLane)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			creature.m_QueueEntity = currentLane.m_QueueEntity;
			creature.m_QueueArea = currentLane.m_QueueArea;
		}

		private void ExitVehicle(ref Random random, Entity entity, Entity controllerVehicle, PrefabRef prefabRef, CurrentVehicle currentVehicle)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			if (m_TransformData.HasComponent(currentVehicle.m_Vehicle))
			{
				Transform vehicleTransform = m_TransformData[currentVehicle.m_Vehicle];
				float3 position = m_TransformData[entity].m_Position;
				BufferLookup<SubMeshGroup> subMeshGroupBuffers = default(BufferLookup<SubMeshGroup>);
				BufferLookup<CharacterElement> characterElementBuffers = default(BufferLookup<CharacterElement>);
				BufferLookup<SubMesh> subMeshBuffers = default(BufferLookup<SubMesh>);
				BufferLookup<AnimationClip> animationClipBuffers = default(BufferLookup<AnimationClip>);
				BufferLookup<AnimationMotion> animationMotionBuffers = default(BufferLookup<AnimationMotion>);
				ActivityMask activityMask;
				AnimatedPropID propID;
				float3 position2 = CreatureUtils.GetVehicleDoorPosition(ref random, ActivityType.Exit, (ActivityCondition)0u, vehicleTransform, position, isDriver: false, m_LefthandTraffic, prefabRef.m_Prefab, currentVehicle.m_Vehicle, default(DynamicBuffer<MeshGroup>), ref m_PublicTransportData, ref m_TrainData, ref m_ControllerData, ref m_PrefabRefData, ref m_PrefabCarData, ref m_PrefabActivityLocationElements, ref subMeshGroupBuffers, ref characterElementBuffers, ref subMeshBuffers, ref animationClipBuffers, ref animationMotionBuffers, out activityMask, out propID).m_Position;
				AnimalCurrentLane newCurrentLane = default(AnimalCurrentLane);
				if (m_UnspawnedData.HasComponent(currentVehicle.m_Vehicle))
				{
					newCurrentLane.m_Flags |= CreatureLaneFlags.EmergeUnspawned;
				}
				PathOwner pathOwner = default(PathOwner);
				if (m_PathOwnerData.TryGetComponent(controllerVehicle, ref pathOwner) && VehicleUtils.PathfindFailed(pathOwner))
				{
					newCurrentLane.m_Flags |= CreatureLaneFlags.Stuck | CreatureLaneFlags.EmergeUnspawned;
				}
				m_BoardingQueue.Enqueue(Boarding.ExitVehicle(entity, currentVehicle.m_Vehicle, newCurrentLane, position2));
			}
			else
			{
				float3 position3 = m_TransformData[entity].m_Position;
				m_BoardingQueue.Enqueue(Boarding.ExitVehicle(entity, currentVehicle.m_Vehicle, default(AnimalCurrentLane), position3));
			}
		}

		private bool HasEveryoneBoarded(DynamicBuffer<GroupCreature> group)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			if (group.IsCreated)
			{
				for (int i = 0; i < group.Length; i++)
				{
					Entity creature = group[i].m_Creature;
					if (!m_CurrentVehicleData.HasComponent(creature))
					{
						return false;
					}
					if ((m_CurrentVehicleData[creature].m_Flags & CreatureVehicleFlags.Ready) == 0)
					{
						return false;
					}
				}
			}
			return true;
		}

		private bool CheckTarget(Entity entity, ref AnimalCurrentLane currentLane, ref Target target)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			if (m_VehicleData.HasComponent(target.m_Target))
			{
				Entity val = target.m_Target;
				if (m_ControllerData.HasComponent(target.m_Target))
				{
					Controller controller = m_ControllerData[target.m_Target];
					if (controller.m_Controller != Entity.Null)
					{
						val = controller.m_Controller;
					}
				}
				if (m_PublicTransportData.HasComponent(val))
				{
					if ((m_PublicTransportData[val].m_State & PublicTransportFlags.Boarding) != 0 && m_OwnerData.HasComponent(val))
					{
						Owner owner = m_OwnerData[val];
						if (m_BuildingData.HasComponent(owner.m_Owner))
						{
							TryEnterVehicle(entity, target.m_Target, ref currentLane);
							target.m_Target = owner.m_Owner;
							return true;
						}
					}
				}
				else if (m_PoliceCarData.HasComponent(val) && (m_PoliceCarData[val].m_State & PoliceCarFlags.AtTarget) != 0 && m_OwnerData.HasComponent(val))
				{
					Owner owner2 = m_OwnerData[val];
					if (m_BuildingData.HasComponent(owner2.m_Owner))
					{
						TryEnterVehicle(entity, target.m_Target, ref currentLane);
						target.m_Target = owner2.m_Owner;
						return true;
					}
				}
			}
			return false;
		}

		private void CurrentVehicleBoarding(int jobIndex, Entity entity, Entity controllerVehicle, ref Game.Creatures.Pet pet, ref Target target)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			Game.Vehicles.PublicTransport publicTransport = m_PublicTransportData[controllerVehicle];
			if ((publicTransport.m_State & (PublicTransportFlags.Evacuating | PublicTransportFlags.PrisonerTransport)) == 0 || (publicTransport.m_State & PublicTransportFlags.Returning) != 0)
			{
				pet.m_Flags |= PetFlags.Disembarking;
			}
		}

		private void CurrentVehicleDisembarking(int jobIndex, Entity entity, Entity controllerVehicle, ref Game.Creatures.Pet pet, ref Target target)
		{
			pet.m_Flags |= PetFlags.Disembarking;
		}

		private void GroupLeaderDisembarking(Entity entity, ref Game.Creatures.Pet pet)
		{
			pet.m_Flags |= PetFlags.Disembarking;
		}

		private bool PathEndReached(int jobIndex, Entity entity, ref Game.Creatures.Pet pet, ref AnimalCurrentLane currentLane, ref Target target)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			if (m_VehicleData.HasComponent(target.m_Target))
			{
				if (m_OwnerData.HasComponent(target.m_Target))
				{
					Owner owner = m_OwnerData[target.m_Target];
					if (m_BuildingData.HasComponent(owner.m_Owner))
					{
						target.m_Target = owner.m_Owner;
						return false;
					}
				}
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, entity, default(Deleted));
				return true;
			}
			if ((pet.m_Flags & (PetFlags.Arrived | PetFlags.LeaderArrived)) == 0)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, entity, default(Deleted));
				return true;
			}
			Entity val = target.m_Target;
			if (m_PropertyRenterData.HasComponent(val))
			{
				val = m_PropertyRenterData[val].m_Property;
			}
			if (m_OnFireData.HasComponent(val) || m_DestroyedData.HasComponent(val))
			{
				return false;
			}
			if ((currentLane.m_Flags & CreatureLaneFlags.Hangaround) != 0)
			{
				pet.m_Flags |= PetFlags.Hangaround;
			}
			else
			{
				pet.m_Flags &= ~PetFlags.Hangaround;
			}
			if (m_PrefabRefData.HasComponent(pet.m_HouseholdPet))
			{
				if ((pet.m_Flags & PetFlags.Hangaround) == 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<CurrentTransport>(jobIndex, pet.m_HouseholdPet);
				}
				if ((pet.m_Flags & PetFlags.Arrived) == 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<CurrentBuilding>(jobIndex, pet.m_HouseholdPet, new CurrentBuilding(val));
					Entity val2 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_ResetTripArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ResetTrip>(jobIndex, val2, new ResetTrip
					{
						m_Creature = entity,
						m_Target = target.m_Target
					});
					pet.m_Flags |= PetFlags.Arrived;
				}
			}
			if ((pet.m_Flags & PetFlags.Hangaround) != PetFlags.None)
			{
				return false;
			}
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, entity, default(Deleted));
			return true;
		}

		private void TryEnterVehicle(Entity entity, Entity vehicle, ref AnimalCurrentLane currentLane)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			m_BoardingQueue.Enqueue(Boarding.TryEnterVehicle(entity, vehicle, CreatureVehicleFlags.Leader));
		}

		private void FinishEnterVehicle(Entity entity, Entity vehicle, ref AnimalCurrentLane currentLane)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			m_BoardingQueue.Enqueue(Boarding.FinishEnterVehicle(entity, vehicle, currentLane));
		}

		private void CancelEnterVehicle(Entity entity, Entity vehicle, ref AnimalCurrentLane currentLane)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			m_BoardingQueue.Enqueue(Boarding.CancelEnterVehicle(entity, vehicle));
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct BoardingJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_ObjectGeometryData;

		public ComponentLookup<Creature> m_Creatures;

		public BufferLookup<Passenger> m_Passengers;

		public BufferLookup<LaneObject> m_LaneObjects;

		[ReadOnly]
		public ComponentTypeSet m_CurrentLaneTypes;

		public NativeQueue<Boarding> m_BoardingQueue;

		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_SearchTree;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			Boarding boarding = default(Boarding);
			while (m_BoardingQueue.TryDequeue(ref boarding))
			{
				switch (boarding.m_Type)
				{
				case BoardingType.Exit:
					ExitVehicle(boarding.m_Passenger, boarding.m_Vehicle, boarding.m_CurrentLane, boarding.m_Position);
					break;
				case BoardingType.TryEnter:
					TryEnterVehicle(boarding.m_Passenger, boarding.m_Vehicle, boarding.m_Flags);
					break;
				case BoardingType.FinishEnter:
					FinishEnterVehicle(boarding.m_Passenger, boarding.m_Vehicle, boarding.m_CurrentLane);
					break;
				case BoardingType.CancelEnter:
					CancelEnterVehicle(boarding.m_Passenger, boarding.m_Vehicle);
					break;
				}
			}
		}

		private void ExitVehicle(Entity passenger, Entity vehicle, AnimalCurrentLane newCurrentLane, float3 position)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			if (m_Passengers.HasBuffer(vehicle))
			{
				CollectionUtils.RemoveValue<Passenger>(m_Passengers[vehicle], new Passenger(passenger));
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<CurrentVehicle>(passenger);
			if (m_LaneObjects.HasBuffer(newCurrentLane.m_Lane))
			{
				NetUtils.AddLaneObject(m_LaneObjects[newCurrentLane.m_Lane], passenger, float2.op_Implicit(newCurrentLane.m_CurvePosition.x));
			}
			else
			{
				PrefabRef prefabRef = m_PrefabRefData[passenger];
				ObjectGeometryData geometryData = m_ObjectGeometryData[prefabRef.m_Prefab];
				Bounds3 bounds = ObjectUtils.CalculateBounds(position, quaternion.identity, geometryData);
				m_SearchTree.Add(passenger, new QuadTreeBoundsXZ(bounds));
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent(passenger, ref m_CurrentLaneTypes);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(passenger, default(Updated));
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<AnimalCurrentLane>(passenger, newCurrentLane);
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Transform>(passenger, new Transform(position, quaternion.identity));
		}

		private void TryEnterVehicle(Entity passenger, Entity vehicle, CreatureVehicleFlags flags)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			if (m_Passengers.HasBuffer(vehicle))
			{
				m_Passengers[vehicle].Add(new Passenger(passenger));
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CurrentVehicle>(passenger, new CurrentVehicle(vehicle, flags));
		}

		private void CancelEnterVehicle(Entity passenger, Entity vehicle)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (m_Passengers.HasBuffer(vehicle))
			{
				CollectionUtils.RemoveValue<Passenger>(m_Passengers[vehicle], new Passenger(passenger));
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<CurrentVehicle>(passenger);
		}

		private void FinishEnterVehicle(Entity passenger, Entity vehicle, AnimalCurrentLane oldCurrentLane)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			Creature creature = m_Creatures[passenger];
			creature.m_QueueEntity = Entity.Null;
			creature.m_QueueArea = default(Sphere3);
			m_Creatures[passenger] = creature;
			if (m_LaneObjects.HasBuffer(oldCurrentLane.m_Lane))
			{
				NetUtils.RemoveLaneObject(m_LaneObjects[oldCurrentLane.m_Lane], passenger);
			}
			else
			{
				m_SearchTree.TryRemove(passenger);
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent(passenger, ref m_CurrentLaneTypes);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Unspawned>(passenger, default(Unspawned));
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(passenger, default(Updated));
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentVehicle> __Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<GroupMember> __Game_Creatures_GroupMember_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Unspawned> __Game_Objects_Unspawned_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<GroupCreature> __Game_Creatures_GroupCreature_RO_BufferTypeHandle;

		public ComponentTypeHandle<Animal> __Game_Creatures_Animal_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Game.Creatures.Pet> __Game_Creatures_Pet_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Creature> __Game_Creatures_Creature_RW_ComponentTypeHandle;

		public ComponentTypeHandle<AnimalCurrentLane> __Game_Creatures_AnimalCurrentLane_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Target> __Game_Common_Target_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> __Game_Creatures_CurrentVehicle_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Destroyed> __Game_Common_Destroyed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Unspawned> __Game_Objects_Unspawned_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Moving> __Game_Objects_Moving_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Creatures.Resident> __Game_Creatures_Resident_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<OnFire> __Game_Events_OnFire_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PersonalCar> __Game_Vehicles_PersonalCar_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.Taxi> __Game_Vehicles_Taxi_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PublicTransport> __Game_Vehicles_PublicTransport_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PoliceCar> __Game_Vehicles_PoliceCar_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Controller> __Game_Vehicles_Controller_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Vehicle> __Game_Vehicles_Vehicle_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Train> __Game_Vehicles_Train_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathOwner> __Game_Pathfind_PathOwner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarData> __Game_Prefabs_CarData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ActivityLocationElement> __Game_Prefabs_ActivityLocationElement_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		public ComponentLookup<Creature> __Game_Creatures_Creature_RW_ComponentLookup;

		public BufferLookup<Passenger> __Game_Vehicles_Passenger_RW_BufferLookup;

		public BufferLookup<LaneObject> __Game_Net_LaneObject_RW_BufferLookup;

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
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentVehicle>(true);
			__Game_Creatures_GroupMember_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<GroupMember>(true);
			__Game_Objects_Unspawned_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Unspawned>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Creatures_GroupCreature_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<GroupCreature>(true);
			__Game_Creatures_Animal_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Animal>(false);
			__Game_Creatures_Pet_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Creatures.Pet>(false);
			__Game_Creatures_Creature_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Creature>(false);
			__Game_Creatures_AnimalCurrentLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AnimalCurrentLane>(false);
			__Game_Common_Target_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Target>(false);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Creatures_CurrentVehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentVehicle>(true);
			__Game_Common_Destroyed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Destroyed>(true);
			__Game_Objects_Unspawned_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Unspawned>(true);
			__Game_Objects_Moving_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Moving>(true);
			__Game_Creatures_Resident_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Creatures.Resident>(true);
			__Game_Events_OnFire_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<OnFire>(true);
			__Game_Vehicles_PersonalCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.PersonalCar>(true);
			__Game_Vehicles_Taxi_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.Taxi>(true);
			__Game_Vehicles_PublicTransport_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.PublicTransport>(true);
			__Game_Vehicles_PoliceCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.PoliceCar>(true);
			__Game_Vehicles_Controller_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Controller>(true);
			__Game_Vehicles_Vehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Vehicle>(true);
			__Game_Vehicles_Train_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Train>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
			__Game_Pathfind_PathOwner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathOwner>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_CarData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarData>(true);
			__Game_Prefabs_ActivityLocationElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ActivityLocationElement>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Creatures_Creature_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Creature>(false);
			__Game_Vehicles_Passenger_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Passenger>(false);
			__Game_Net_LaneObject_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneObject>(false);
		}
	}

	private EndFrameBarrier m_EndFrameBarrier;

	private Game.Objects.SearchSystem m_ObjectSearchSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private EntityQuery m_CreatureQuery;

	private EntityArchetype m_ResetTripArchetype;

	private ComponentTypeSet m_CurrentLaneTypes;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 5;
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
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_ObjectSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.SearchSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_CreatureQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[8]
		{
			ComponentType.ReadWrite<Game.Creatures.Pet>(),
			ComponentType.ReadOnly<Animal>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.ReadWrite<Target>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Stumbling>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_ResetTripArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Game.Common.Event>(),
			ComponentType.ReadWrite<ResetTrip>()
		});
		m_CurrentLaneTypes = new ComponentTypeSet((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadWrite<Moving>(),
			ComponentType.ReadWrite<TransformFrame>(),
			ComponentType.ReadWrite<InterpolatedTransform>(),
			ComponentType.ReadWrite<AnimalNavigation>(),
			ComponentType.ReadWrite<AnimalCurrentLane>(),
			ComponentType.ReadWrite<Blocker>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_CreatureQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_049b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0502: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Unknown result type (might be due to invalid IL or missing references)
		//IL_0508: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Unknown result type (might be due to invalid IL or missing references)
		NativeQueue<Boarding> boardingQueue = default(NativeQueue<Boarding>);
		boardingQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		PetTickJob petTickJob = new PetTickJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentVehicleType = InternalCompilerInterface.GetComponentTypeHandle<CurrentVehicle>(ref __TypeHandle.__Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_GroupMemberType = InternalCompilerInterface.GetComponentTypeHandle<GroupMember>(ref __TypeHandle.__Game_Creatures_GroupMember_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UnspawnedType = InternalCompilerInterface.GetComponentTypeHandle<Unspawned>(ref __TypeHandle.__Game_Objects_Unspawned_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_GroupCreatureType = InternalCompilerInterface.GetBufferTypeHandle<GroupCreature>(ref __TypeHandle.__Game_Creatures_GroupCreature_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AnimalType = InternalCompilerInterface.GetComponentTypeHandle<Animal>(ref __TypeHandle.__Game_Creatures_Animal_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PetType = InternalCompilerInterface.GetComponentTypeHandle<Game.Creatures.Pet>(ref __TypeHandle.__Game_Creatures_Pet_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CreatureType = InternalCompilerInterface.GetComponentTypeHandle<Creature>(ref __TypeHandle.__Game_Creatures_Creature_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<AnimalCurrentLane>(ref __TypeHandle.__Game_Creatures_AnimalCurrentLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TargetType = InternalCompilerInterface.GetComponentTypeHandle<Target>(ref __TypeHandle.__Game_Common_Target_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentVehicleData = InternalCompilerInterface.GetComponentLookup<CurrentVehicle>(ref __TypeHandle.__Game_Creatures_CurrentVehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DestroyedData = InternalCompilerInterface.GetComponentLookup<Destroyed>(ref __TypeHandle.__Game_Common_Destroyed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UnspawnedData = InternalCompilerInterface.GetComponentLookup<Unspawned>(ref __TypeHandle.__Game_Objects_Unspawned_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MovingData = InternalCompilerInterface.GetComponentLookup<Moving>(ref __TypeHandle.__Game_Objects_Moving_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResidentData = InternalCompilerInterface.GetComponentLookup<Game.Creatures.Resident>(ref __TypeHandle.__Game_Creatures_Resident_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OnFireData = InternalCompilerInterface.GetComponentLookup<OnFire>(ref __TypeHandle.__Game_Events_OnFire_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PersonalCarData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.PersonalCar>(ref __TypeHandle.__Game_Vehicles_PersonalCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TaxiData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.Taxi>(ref __TypeHandle.__Game_Vehicles_Taxi_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PublicTransportData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.PublicTransport>(ref __TypeHandle.__Game_Vehicles_PublicTransport_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PoliceCarData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.PoliceCar>(ref __TypeHandle.__Game_Vehicles_PoliceCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ControllerData = InternalCompilerInterface.GetComponentLookup<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_VehicleData = InternalCompilerInterface.GetComponentLookup<Vehicle>(ref __TypeHandle.__Game_Vehicles_Vehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrainData = InternalCompilerInterface.GetComponentLookup<Train>(ref __TypeHandle.__Game_Vehicles_Train_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyRenterData = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathOwnerData = InternalCompilerInterface.GetComponentLookup<PathOwner>(ref __TypeHandle.__Game_Pathfind_PathOwner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCarData = InternalCompilerInterface.GetComponentLookup<CarData>(ref __TypeHandle.__Game_Prefabs_CarData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabActivityLocationElements = InternalCompilerInterface.GetBufferLookup<ActivityLocationElement>(ref __TypeHandle.__Game_Prefabs_ActivityLocationElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RandomSeed = RandomSeed.Next(),
			m_LefthandTraffic = m_CityConfigurationSystem.leftHandTraffic,
			m_ResetTripArchetype = m_ResetTripArchetype,
			m_BoardingQueue = boardingQueue.AsParallelWriter()
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		petTickJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		PetTickJob petTickJob2 = petTickJob;
		JobHandle dependencies;
		BoardingJob obj = new BoardingJob
		{
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Creatures = InternalCompilerInterface.GetComponentLookup<Creature>(ref __TypeHandle.__Game_Creatures_Creature_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Passengers = InternalCompilerInterface.GetBufferLookup<Passenger>(ref __TypeHandle.__Game_Vehicles_Passenger_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneObjects = InternalCompilerInterface.GetBufferLookup<LaneObject>(ref __TypeHandle.__Game_Net_LaneObject_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentLaneTypes = m_CurrentLaneTypes,
			m_BoardingQueue = boardingQueue,
			m_SearchTree = m_ObjectSearchSystem.GetMovingSearchTree(readOnly: false, out dependencies),
			m_CommandBuffer = m_EndFrameBarrier.CreateCommandBuffer()
		};
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<PetTickJob>(petTickJob2, m_CreatureQuery, ((SystemBase)this).Dependency);
		JobHandle val3 = IJobExtensions.Schedule<BoardingJob>(obj, JobHandle.CombineDependencies(val2, dependencies));
		boardingQueue.Dispose(val3);
		m_ObjectSearchSystem.AddMovingSearchTreeWriter(val3);
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
	public PetAISystem()
	{
	}
}
