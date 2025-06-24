using System.Runtime.CompilerServices;
using Colossal.Entities;
using Game.Citizens;
using Game.Common;
using Game.Objects;
using Game.Pathfind;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Creatures;

[CompilerGenerated]
public class GroupSystem : GameSystemBase
{
	private struct GroupData
	{
		public Entity m_Creature1;

		public Entity m_Creature2;

		public GroupData(Entity creature1, Entity creature2)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			m_Creature1 = creature1;
			m_Creature2 = creature2;
		}
	}

	[BurstCompile]
	private struct ResetTripSetJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<ResetTrip> m_ResetTripType;

		public NativeParallelHashSet<Entity> m_ResetTripSet;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<ResetTrip> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ResetTrip>(ref m_ResetTripType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				m_ResetTripSet.Add(nativeArray[i].m_Creature);
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct FillGroupQueueJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Target> m_TargetType;

		[ReadOnly]
		public ComponentTypeHandle<TripSource> m_TripSourceType;

		[ReadOnly]
		public ComponentTypeHandle<Resident> m_ResidentType;

		[ReadOnly]
		public ComponentTypeHandle<Pet> m_PetType;

		[ReadOnly]
		public ComponentTypeHandle<Wildlife> m_WildlifeType;

		[ReadOnly]
		public ComponentTypeHandle<Domesticated> m_DomesticatedType;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedType;

		[ReadOnly]
		public ComponentTypeHandle<ResetTrip> m_ResetTripType;

		[ReadOnly]
		public ComponentTypeHandle<GroupMember> m_GroupMemberType;

		[ReadOnly]
		public BufferTypeHandle<GroupCreature> m_GroupCreatureType;

		[ReadOnly]
		public ComponentLookup<Target> m_TargetData;

		[ReadOnly]
		public ComponentLookup<TripSource> m_TripSourceData;

		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		[ReadOnly]
		public ComponentLookup<Updated> m_UpdatedData;

		[ReadOnly]
		public ComponentLookup<HouseholdMember> m_HouseholdMemberData;

		[ReadOnly]
		public ComponentLookup<CurrentTransport> m_CurrentTransportData;

		[ReadOnly]
		public ComponentLookup<HouseholdPet> m_HouseholdPetData;

		[ReadOnly]
		public ComponentLookup<GroupMember> m_GroupMemberData;

		[ReadOnly]
		public ComponentLookup<Resident> m_ResidentData;

		[ReadOnly]
		public ComponentLookup<Pet> m_PetData;

		[ReadOnly]
		public ComponentLookup<Wildlife> m_WildlifeData;

		[ReadOnly]
		public ComponentLookup<Domesticated> m_DomesticatedData;

		[ReadOnly]
		public BufferLookup<GroupCreature> m_GroupCreatures;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> m_HouseholdCitizens;

		[ReadOnly]
		public BufferLookup<HouseholdAnimal> m_HouseholdAnimals;

		[ReadOnly]
		public BufferLookup<OwnedCreature> m_OwnedCreatures;

		[ReadOnly]
		public NativeParallelHashSet<Entity> m_ResetTripSet;

		public ParallelWriter<GroupData> m_GroupQueue;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<ResetTrip> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ResetTrip>(ref m_ResetTripType);
			if (nativeArray.Length != 0)
			{
				GroupMember groupMember = default(GroupMember);
				DynamicBuffer<GroupCreature> groupCreatures = default(DynamicBuffer<GroupCreature>);
				TripSource tripSource = default(TripSource);
				Target target = default(Target);
				Resident resident = default(Resident);
				Pet pet = default(Pet);
				for (int i = 0; i < nativeArray.Length; i++)
				{
					ResetTrip resetTrip = nativeArray[i];
					if (m_DeletedData.HasComponent(resetTrip.m_Creature))
					{
						continue;
					}
					if (m_GroupMemberData.TryGetComponent(resetTrip.m_Creature, ref groupMember))
					{
						CheckDeletedGroupMember(groupMember);
					}
					if (m_GroupCreatures.TryGetBuffer(resetTrip.m_Creature, ref groupCreatures))
					{
						CheckDeletedGroupLeader(groupCreatures);
					}
					if (m_UpdatedData.HasComponent(resetTrip.m_Creature) && m_TripSourceData.HasComponent(resetTrip.m_Creature))
					{
						continue;
					}
					m_GroupQueue.Enqueue(new GroupData(resetTrip.m_Creature, Entity.Null));
					if (m_TripSourceData.TryGetComponent(resetTrip.m_Creature, ref tripSource) && m_TargetData.TryGetComponent(resetTrip.m_Creature, ref target))
					{
						if (m_ResidentData.TryGetComponent(resetTrip.m_Creature, ref resident))
						{
							CheckUpdatedResident(resetTrip.m_Creature, resident, tripSource, target);
						}
						if (m_PetData.TryGetComponent(resetTrip.m_Creature, ref pet))
						{
							CheckUpdatedPet(resetTrip.m_Creature, pet, tripSource, target);
						}
						if (m_WildlifeData.HasComponent(resetTrip.m_Creature))
						{
							CheckUpdatedWildlife(resetTrip.m_Creature, tripSource);
						}
						if (m_DomesticatedData.HasComponent(resetTrip.m_Creature))
						{
							CheckUpdatedDomesticated(resetTrip.m_Creature, tripSource);
						}
					}
				}
				return;
			}
			if (((ArchetypeChunk)(ref chunk)).Has<Deleted>(ref m_DeletedType))
			{
				NativeArray<GroupMember> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<GroupMember>(ref m_GroupMemberType);
				BufferAccessor<GroupCreature> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<GroupCreature>(ref m_GroupCreatureType);
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					CheckDeletedGroupMember(nativeArray2[j]);
				}
				for (int k = 0; k < bufferAccessor.Length; k++)
				{
					CheckDeletedGroupLeader(bufferAccessor[k]);
				}
				return;
			}
			NativeArray<TripSource> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TripSource>(ref m_TripSourceType);
			NativeArray<Target> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Target>(ref m_TargetType);
			if (nativeArray3.Length != 0 && nativeArray4.Length != 0)
			{
				NativeArray<Entity> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				NativeArray<Resident> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Resident>(ref m_ResidentType);
				NativeArray<Pet> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Pet>(ref m_PetType);
				NativeArray<Wildlife> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Wildlife>(ref m_WildlifeType);
				NativeArray<Domesticated> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Domesticated>(ref m_DomesticatedType);
				for (int l = 0; l < nativeArray6.Length; l++)
				{
					CheckUpdatedResident(nativeArray5[l], nativeArray6[l], nativeArray3[l], nativeArray4[l]);
				}
				for (int m = 0; m < nativeArray7.Length; m++)
				{
					CheckUpdatedPet(nativeArray5[m], nativeArray7[m], nativeArray3[m], nativeArray4[m]);
				}
				for (int n = 0; n < nativeArray8.Length; n++)
				{
					CheckUpdatedWildlife(nativeArray5[n], nativeArray3[n]);
				}
				for (int num = 0; num < nativeArray9.Length; num++)
				{
					CheckUpdatedDomesticated(nativeArray5[num], nativeArray3[num]);
				}
			}
		}

		private void CheckDeletedGroupMember(GroupMember groupMember)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			if (!m_DeletedData.HasComponent(groupMember.m_Leader) && !m_ResetTripSet.Contains(groupMember.m_Leader))
			{
				m_GroupQueue.Enqueue(new GroupData(groupMember.m_Leader, Entity.Null));
			}
		}

		private void CheckDeletedGroupLeader(DynamicBuffer<GroupCreature> groupCreatures)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			Entity val = Entity.Null;
			bool flag = false;
			for (int i = 0; i < groupCreatures.Length; i++)
			{
				GroupCreature groupCreature = groupCreatures[i];
				if (!m_DeletedData.HasComponent(groupCreature.m_Creature) && !m_ResetTripSet.Contains(groupCreature.m_Creature))
				{
					if (val != Entity.Null)
					{
						m_GroupQueue.Enqueue(new GroupData(val, groupCreature.m_Creature));
						flag = true;
					}
					val = groupCreature.m_Creature;
				}
			}
			if (val != Entity.Null && !flag)
			{
				m_GroupQueue.Enqueue(new GroupData(val, Entity.Null));
			}
		}

		private void CheckUpdatedResident(Entity creature, Resident resident, TripSource tripSource, Target target)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			if (m_HouseholdMemberData.HasComponent(resident.m_Citizen))
			{
				HouseholdMember householdMember = m_HouseholdMemberData[resident.m_Citizen];
				FindHumanPartners(creature, householdMember.m_Household, tripSource.m_Source, target.m_Target);
				FindAnimalPartners(creature, householdMember.m_Household, tripSource.m_Source, target.m_Target);
			}
		}

		private void CheckUpdatedPet(Entity creature, Pet pet, TripSource tripSource, Target target)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			if (m_HouseholdPetData.HasComponent(pet.m_HouseholdPet))
			{
				HouseholdPet householdPet = m_HouseholdPetData[pet.m_HouseholdPet];
				FindHumanPartners(creature, householdPet.m_Household, tripSource.m_Source, target.m_Target);
				FindAnimalPartners(creature, householdPet.m_Household, tripSource.m_Source, target.m_Target);
			}
		}

		private void CheckUpdatedWildlife(Entity creature, TripSource tripSource)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			FindCreaturePartners(creature, tripSource.m_Source);
		}

		private void CheckUpdatedDomesticated(Entity creature, TripSource tripSource)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			FindCreaturePartners(creature, tripSource.m_Source);
		}

		private void FindCreaturePartners(Entity creature, Entity source)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			if (!m_OwnedCreatures.HasBuffer(source))
			{
				return;
			}
			DynamicBuffer<OwnedCreature> val = m_OwnedCreatures[source];
			for (int i = 0; i < val.Length; i++)
			{
				OwnedCreature ownedCreature = val[i];
				if (!(ownedCreature.m_Creature == creature) && !m_DeletedData.HasComponent(ownedCreature.m_Creature) && m_TripSourceData.HasComponent(ownedCreature.m_Creature) && source == m_TripSourceData[ownedCreature.m_Creature].m_Source)
				{
					m_GroupQueue.Enqueue(new GroupData(creature, ownedCreature.m_Creature));
				}
			}
		}

		private void FindAnimalPartners(Entity creature, Entity household, Entity source, Entity target)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			if (!m_HouseholdAnimals.HasBuffer(household))
			{
				return;
			}
			DynamicBuffer<HouseholdAnimal> val = m_HouseholdAnimals[household];
			for (int i = 0; i < val.Length; i++)
			{
				HouseholdAnimal householdAnimal = val[i];
				if (!m_CurrentTransportData.HasComponent(householdAnimal.m_HouseholdPet))
				{
					continue;
				}
				CurrentTransport currentTransport = m_CurrentTransportData[householdAnimal.m_HouseholdPet];
				if (!(currentTransport.m_CurrentTransport == creature) && !m_DeletedData.HasComponent(currentTransport.m_CurrentTransport) && m_TripSourceData.HasComponent(currentTransport.m_CurrentTransport) && m_TargetData.HasComponent(currentTransport.m_CurrentTransport))
				{
					TripSource tripSource = m_TripSourceData[currentTransport.m_CurrentTransport];
					Target target2 = m_TargetData[currentTransport.m_CurrentTransport];
					if (source == tripSource.m_Source && target == target2.m_Target)
					{
						m_GroupQueue.Enqueue(new GroupData(creature, currentTransport.m_CurrentTransport));
					}
				}
			}
		}

		private void FindHumanPartners(Entity creature, Entity household, Entity source, Entity target)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			if (!m_HouseholdCitizens.HasBuffer(household))
			{
				return;
			}
			DynamicBuffer<HouseholdCitizen> val = m_HouseholdCitizens[household];
			for (int i = 0; i < val.Length; i++)
			{
				HouseholdCitizen householdCitizen = val[i];
				if (!m_CurrentTransportData.HasComponent(householdCitizen.m_Citizen))
				{
					continue;
				}
				CurrentTransport currentTransport = m_CurrentTransportData[householdCitizen.m_Citizen];
				if (!(currentTransport.m_CurrentTransport == creature) && !m_DeletedData.HasComponent(currentTransport.m_CurrentTransport) && m_TripSourceData.HasComponent(currentTransport.m_CurrentTransport) && m_TargetData.HasComponent(currentTransport.m_CurrentTransport))
				{
					TripSource tripSource = m_TripSourceData[currentTransport.m_CurrentTransport];
					Target target2 = m_TargetData[currentTransport.m_CurrentTransport];
					if (source == tripSource.m_Source && target == target2.m_Target)
					{
						m_GroupQueue.Enqueue(new GroupData(creature, currentTransport.m_CurrentTransport));
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct GroupCreaturesJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<Human> m_HumanData;

		[ReadOnly]
		public ComponentLookup<Resident> m_ResidentData;

		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		[ReadOnly]
		public ComponentLookup<CarKeeper> m_CarKeeperData;

		public ComponentLookup<GroupMember> m_GroupMemberData;

		public ComponentLookup<PathOwner> m_PathOwnerData;

		public BufferLookup<GroupCreature> m_GroupCreatures;

		public BufferLookup<PathElement> m_PathElements;

		[ReadOnly]
		public NativeParallelHashSet<Entity> m_ResetTripSet;

		public NativeQueue<GroupData> m_GroupQueue;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			if (m_GroupQueue.Count == 0)
			{
				return;
			}
			GroupBuilder<Entity> groupBuffer = new GroupBuilder<Entity>((Allocator)2);
			GroupData groupData = default(GroupData);
			while (m_GroupQueue.TryDequeue(ref groupData))
			{
				if (groupData.m_Creature1 != Entity.Null)
				{
					AddExistingGroupMembers(ref groupBuffer, groupData.m_Creature1);
					if (groupData.m_Creature2 != Entity.Null)
					{
						AddExistingGroupMembers(ref groupBuffer, groupData.m_Creature2);
						groupBuffer.AddPair(groupData.m_Creature1, groupData.m_Creature2);
					}
					else
					{
						groupBuffer.AddSingle(groupData.m_Creature1);
					}
				}
				else if (groupData.m_Creature2 != Entity.Null)
				{
					AddExistingGroupMembers(ref groupBuffer, groupData.m_Creature2);
					groupBuffer.AddSingle(groupData.m_Creature2);
				}
			}
			if (groupBuffer.TryGetFirstGroup(out var group, out var iterator))
			{
				do
				{
					ComposeGroup(NativeSlice<GroupBuilder<Entity>.Result>.op_Implicit(group));
				}
				while (groupBuffer.TryGetNextGroup(out group, ref iterator));
			}
		}

		private void AddExistingGroupMembers(ref GroupBuilder<Entity> groupBuffer, Entity creature)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			if (m_ResetTripSet.Contains(creature))
			{
				return;
			}
			if (m_GroupMemberData.HasComponent(creature))
			{
				creature = m_GroupMemberData[creature].m_Leader;
				if (m_DeletedData.HasComponent(creature) || m_ResetTripSet.Contains(creature))
				{
					return;
				}
			}
			if (!m_GroupCreatures.HasBuffer(creature))
			{
				return;
			}
			DynamicBuffer<GroupCreature> val = m_GroupCreatures[creature];
			for (int i = 0; i < val.Length; i++)
			{
				Entity creature2 = val[i].m_Creature;
				if (!m_DeletedData.HasComponent(creature2) && !m_ResetTripSet.Contains(creature2))
				{
					groupBuffer.AddPair(creature, creature2);
				}
			}
		}

		private void ComposeGroup(NativeSlice<GroupBuilder<Entity>.Result> group)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			if (group.Length == 1)
			{
				Entity item = group[0].m_Item;
				RemoveGroupMember(item);
				RemoveGroupCreatures(item);
				return;
			}
			Entity val = FindBestLeader(group);
			RemoveGroupMember(val);
			DynamicBuffer<GroupCreature> val2 = AddGroupCreatures(val);
			for (int i = 0; i < group.Length; i++)
			{
				Entity item2 = group[i].m_Item;
				if (item2 != val)
				{
					RemoveGroupCreatures(item2);
					AddGroupMember(item2, val);
					val2.Add(new GroupCreature(item2));
				}
			}
		}

		private void RemoveGroupMember(Entity creature)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			if (m_GroupMemberData.HasComponent(creature))
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<GroupMember>(creature);
				if (m_PathOwnerData.HasComponent(creature))
				{
					PathOwner pathOwner = m_PathOwnerData[creature];
					pathOwner.m_ElementIndex = 0;
					pathOwner.m_State |= PathFlags.Obsolete;
					m_PathOwnerData[creature] = pathOwner;
					m_PathElements[creature].Clear();
				}
			}
		}

		private void RemoveGroupCreatures(Entity creature)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			if (m_GroupCreatures.HasBuffer(creature))
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<GroupCreature>(creature);
			}
		}

		private void AddGroupMember(Entity creature, Entity leader)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			if (m_GroupMemberData.HasComponent(creature))
			{
				GroupMember groupMember = m_GroupMemberData[creature];
				groupMember.m_Leader = leader;
				m_GroupMemberData[creature] = groupMember;
				return;
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<GroupMember>(creature, new GroupMember(leader));
			((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Divert>(creature);
			if (m_PathOwnerData.HasComponent(creature))
			{
				PathOwner pathOwner = m_PathOwnerData[creature];
				pathOwner.m_ElementIndex = 0;
				pathOwner.m_State = (PathFlags)0;
				m_PathOwnerData[creature] = pathOwner;
				m_PathElements[creature].Clear();
			}
		}

		private DynamicBuffer<GroupCreature> AddGroupCreatures(Entity creature)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			if (m_GroupCreatures.HasBuffer(creature))
			{
				DynamicBuffer<GroupCreature> result = m_GroupCreatures[creature];
				result.Clear();
				return result;
			}
			return ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<GroupCreature>(creature);
		}

		private Entity FindBestLeader(NativeSlice<GroupBuilder<Entity>.Result> group)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			Entity result = Entity.Null;
			int num = -1;
			for (int i = 0; i < group.Length; i++)
			{
				Entity item = group[i].m_Item;
				int num2 = 0;
				if (m_HumanData.HasComponent(item))
				{
					num2 += 10;
				}
				if (m_ResidentData.HasComponent(item))
				{
					Resident resident = m_ResidentData[item];
					if ((resident.m_Flags & ResidentFlags.PreferredLeader) != ResidentFlags.None)
					{
						num2 += 2;
					}
					if (EntitiesExtensions.HasEnabledComponent<CarKeeper>(m_CarKeeperData, resident.m_Citizen))
					{
						num2++;
					}
				}
				if (num2 > num)
				{
					result = item;
					num = num2;
				}
			}
			return result;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<ResetTrip> __Game_Creatures_ResetTrip_RO_ComponentTypeHandle;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Target> __Game_Common_Target_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TripSource> __Game_Objects_TripSource_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Resident> __Game_Creatures_Resident_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Pet> __Game_Creatures_Pet_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Wildlife> __Game_Creatures_Wildlife_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Domesticated> __Game_Creatures_Domesticated_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> __Game_Common_Deleted_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<GroupMember> __Game_Creatures_GroupMember_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<GroupCreature> __Game_Creatures_GroupCreature_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Target> __Game_Common_Target_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TripSource> __Game_Objects_TripSource_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Updated> __Game_Common_Updated_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HouseholdMember> __Game_Citizens_HouseholdMember_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentTransport> __Game_Citizens_CurrentTransport_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HouseholdPet> __Game_Citizens_HouseholdPet_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<GroupMember> __Game_Creatures_GroupMember_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Resident> __Game_Creatures_Resident_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Pet> __Game_Creatures_Pet_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Wildlife> __Game_Creatures_Wildlife_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Domesticated> __Game_Creatures_Domesticated_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<GroupCreature> __Game_Creatures_GroupCreature_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<HouseholdAnimal> __Game_Citizens_HouseholdAnimal_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<OwnedCreature> __Game_Creatures_OwnedCreature_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Human> __Game_Creatures_Human_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarKeeper> __Game_Citizens_CarKeeper_RO_ComponentLookup;

		public ComponentLookup<GroupMember> __Game_Creatures_GroupMember_RW_ComponentLookup;

		public ComponentLookup<PathOwner> __Game_Pathfind_PathOwner_RW_ComponentLookup;

		public BufferLookup<GroupCreature> __Game_Creatures_GroupCreature_RW_BufferLookup;

		public BufferLookup<PathElement> __Game_Pathfind_PathElement_RW_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
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
			__Game_Creatures_ResetTrip_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ResetTrip>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Target_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Target>(true);
			__Game_Objects_TripSource_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TripSource>(true);
			__Game_Creatures_Resident_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Resident>(true);
			__Game_Creatures_Pet_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Pet>(true);
			__Game_Creatures_Wildlife_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Wildlife>(true);
			__Game_Creatures_Domesticated_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Domesticated>(true);
			__Game_Common_Deleted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Deleted>(true);
			__Game_Creatures_GroupMember_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<GroupMember>(true);
			__Game_Creatures_GroupCreature_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<GroupCreature>(true);
			__Game_Common_Target_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Target>(true);
			__Game_Objects_TripSource_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TripSource>(true);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Common_Updated_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Updated>(true);
			__Game_Citizens_HouseholdMember_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HouseholdMember>(true);
			__Game_Citizens_CurrentTransport_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentTransport>(true);
			__Game_Citizens_HouseholdPet_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HouseholdPet>(true);
			__Game_Creatures_GroupMember_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GroupMember>(true);
			__Game_Creatures_Resident_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Resident>(true);
			__Game_Creatures_Pet_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Pet>(true);
			__Game_Creatures_Wildlife_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Wildlife>(true);
			__Game_Creatures_Domesticated_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Domesticated>(true);
			__Game_Creatures_GroupCreature_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<GroupCreature>(true);
			__Game_Citizens_HouseholdCitizen_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdCitizen>(true);
			__Game_Citizens_HouseholdAnimal_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdAnimal>(true);
			__Game_Creatures_OwnedCreature_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<OwnedCreature>(true);
			__Game_Creatures_Human_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Human>(true);
			__Game_Citizens_CarKeeper_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarKeeper>(true);
			__Game_Creatures_GroupMember_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GroupMember>(false);
			__Game_Pathfind_PathOwner_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathOwner>(false);
			__Game_Creatures_GroupCreature_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<GroupCreature>(false);
			__Game_Pathfind_PathElement_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PathElement>(false);
		}
	}

	private ModificationBarrier5 m_ModificationBarrier;

	private EntityQuery m_CreatureQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier5>();
		EntityQueryDesc[] array = new EntityQueryDesc[2];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Creature>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ResetTrip>() };
		array[1] = val;
		m_CreatureQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		((ComponentSystemBase)this).RequireForUpdate(m_CreatureQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		NativeParallelHashSet<Entity> resetTripSet = default(NativeParallelHashSet<Entity>);
		resetTripSet._002Ector(100, AllocatorHandle.op_Implicit((Allocator)3));
		NativeQueue<GroupData> groupQueue = default(NativeQueue<GroupData>);
		groupQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		ResetTripSetJob resetTripSetJob = new ResetTripSetJob
		{
			m_ResetTripType = InternalCompilerInterface.GetComponentTypeHandle<ResetTrip>(ref __TypeHandle.__Game_Creatures_ResetTrip_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResetTripSet = resetTripSet
		};
		FillGroupQueueJob fillGroupQueueJob = new FillGroupQueueJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TargetType = InternalCompilerInterface.GetComponentTypeHandle<Target>(ref __TypeHandle.__Game_Common_Target_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TripSourceType = InternalCompilerInterface.GetComponentTypeHandle<TripSource>(ref __TypeHandle.__Game_Objects_TripSource_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResidentType = InternalCompilerInterface.GetComponentTypeHandle<Resident>(ref __TypeHandle.__Game_Creatures_Resident_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PetType = InternalCompilerInterface.GetComponentTypeHandle<Pet>(ref __TypeHandle.__Game_Creatures_Pet_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WildlifeType = InternalCompilerInterface.GetComponentTypeHandle<Wildlife>(ref __TypeHandle.__Game_Creatures_Wildlife_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DomesticatedType = InternalCompilerInterface.GetComponentTypeHandle<Domesticated>(ref __TypeHandle.__Game_Creatures_Domesticated_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DeletedType = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResetTripType = InternalCompilerInterface.GetComponentTypeHandle<ResetTrip>(ref __TypeHandle.__Game_Creatures_ResetTrip_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_GroupMemberType = InternalCompilerInterface.GetComponentTypeHandle<GroupMember>(ref __TypeHandle.__Game_Creatures_GroupMember_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_GroupCreatureType = InternalCompilerInterface.GetBufferTypeHandle<GroupCreature>(ref __TypeHandle.__Game_Creatures_GroupCreature_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TargetData = InternalCompilerInterface.GetComponentLookup<Target>(ref __TypeHandle.__Game_Common_Target_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TripSourceData = InternalCompilerInterface.GetComponentLookup<TripSource>(ref __TypeHandle.__Game_Objects_TripSource_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UpdatedData = InternalCompilerInterface.GetComponentLookup<Updated>(ref __TypeHandle.__Game_Common_Updated_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdMemberData = InternalCompilerInterface.GetComponentLookup<HouseholdMember>(ref __TypeHandle.__Game_Citizens_HouseholdMember_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentTransportData = InternalCompilerInterface.GetComponentLookup<CurrentTransport>(ref __TypeHandle.__Game_Citizens_CurrentTransport_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdPetData = InternalCompilerInterface.GetComponentLookup<HouseholdPet>(ref __TypeHandle.__Game_Citizens_HouseholdPet_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GroupMemberData = InternalCompilerInterface.GetComponentLookup<GroupMember>(ref __TypeHandle.__Game_Creatures_GroupMember_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResidentData = InternalCompilerInterface.GetComponentLookup<Resident>(ref __TypeHandle.__Game_Creatures_Resident_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PetData = InternalCompilerInterface.GetComponentLookup<Pet>(ref __TypeHandle.__Game_Creatures_Pet_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WildlifeData = InternalCompilerInterface.GetComponentLookup<Wildlife>(ref __TypeHandle.__Game_Creatures_Wildlife_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DomesticatedData = InternalCompilerInterface.GetComponentLookup<Domesticated>(ref __TypeHandle.__Game_Creatures_Domesticated_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GroupCreatures = InternalCompilerInterface.GetBufferLookup<GroupCreature>(ref __TypeHandle.__Game_Creatures_GroupCreature_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdCitizens = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdAnimals = InternalCompilerInterface.GetBufferLookup<HouseholdAnimal>(ref __TypeHandle.__Game_Citizens_HouseholdAnimal_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnedCreatures = InternalCompilerInterface.GetBufferLookup<OwnedCreature>(ref __TypeHandle.__Game_Creatures_OwnedCreature_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResetTripSet = resetTripSet,
			m_GroupQueue = groupQueue.AsParallelWriter()
		};
		GroupCreaturesJob obj = new GroupCreaturesJob
		{
			m_HumanData = InternalCompilerInterface.GetComponentLookup<Human>(ref __TypeHandle.__Game_Creatures_Human_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResidentData = InternalCompilerInterface.GetComponentLookup<Resident>(ref __TypeHandle.__Game_Creatures_Resident_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarKeeperData = InternalCompilerInterface.GetComponentLookup<CarKeeper>(ref __TypeHandle.__Game_Citizens_CarKeeper_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GroupMemberData = InternalCompilerInterface.GetComponentLookup<GroupMember>(ref __TypeHandle.__Game_Creatures_GroupMember_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathOwnerData = InternalCompilerInterface.GetComponentLookup<PathOwner>(ref __TypeHandle.__Game_Pathfind_PathOwner_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GroupCreatures = InternalCompilerInterface.GetBufferLookup<GroupCreature>(ref __TypeHandle.__Game_Creatures_GroupCreature_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathElements = InternalCompilerInterface.GetBufferLookup<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResetTripSet = resetTripSet,
			m_GroupQueue = groupQueue,
			m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer()
		};
		JobHandle val = JobChunkExtensions.Schedule<ResetTripSetJob>(resetTripSetJob, m_CreatureQuery, ((SystemBase)this).Dependency);
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<FillGroupQueueJob>(fillGroupQueueJob, m_CreatureQuery, val);
		JobHandle val3 = IJobExtensions.Schedule<GroupCreaturesJob>(obj, val2);
		resetTripSet.Dispose(val3);
		groupQueue.Dispose(val3);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val3);
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
	public GroupSystem()
	{
	}
}
