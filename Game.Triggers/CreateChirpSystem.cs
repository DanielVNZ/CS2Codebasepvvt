using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Unity.Assertions;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Triggers;

[CompilerGenerated]
public class CreateChirpSystem : GameSystemBase
{
	[BurstCompile]
	public struct CollectRecentChirpsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		[ReadOnly]
		public ComponentTypeHandle<Chirp> m_ChirpType;

		[ReadOnly]
		public ComponentLookup<ChirpData> m_ChirpDatas;

		public ParallelWriter<Entity, Entity> m_RecentChirps;

		public uint m_SimulationFrame;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabType);
			NativeArray<Chirp> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Chirp>(ref m_ChirpType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				if (nativeArray3[i].m_CreationFrame + 18000 >= m_SimulationFrame)
				{
					Entity prefab = nativeArray2[i].m_Prefab;
					if (m_ChirpDatas.HasComponent(prefab))
					{
						m_RecentChirps.TryAdd(prefab, nativeArray[i]);
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
	private struct CreateChirpJob : IJob
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public BufferLookup<TriggerChirpData> m_TriggerChirpData;

		[ReadOnly]
		public ComponentLookup<ChirpData> m_ChirpData;

		[ReadOnly]
		public ComponentLookup<LifePathEventData> m_LifepathEventData;

		[ReadOnly]
		public ComponentLookup<BrandChirpData> m_BrandChirpData;

		[ReadOnly]
		public ComponentLookup<RandomLikeCountData> m_RandomLikeCountData;

		[ReadOnly]
		public ComponentLookup<ServiceChirpData> m_ServiceChirpDatas;

		[ReadOnly]
		public ComponentLookup<Citizen> m_Citizens;

		[ReadOnly]
		public ComponentLookup<HealthProblem> m_HealthProblems;

		[ReadOnly]
		public ComponentLookup<HouseholdMember> m_HouseholdMembers;

		[ReadOnly]
		public ComponentLookup<Household> m_Households;

		[ReadOnly]
		public ComponentLookup<HomelessHousehold> m_HomelessHouseholds;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> m_HouseholdCitizens;

		[ReadOnly]
		public BufferLookup<Employee> m_Employees;

		[ReadOnly]
		public BufferLookup<LifePathEntry> m_LifepathEntries;

		public EntityCommandBuffer m_CommandBuffer;

		public NativeQueue<ChirpCreationData> m_Queue;

		public NativeParallelHashMap<Entity, Entity> m_RecentChirps;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_RandomCitizenChunks;

		public RandomSeed m_RandomSeed;

		public int m_UneducatedPopulation;

		public int m_EducatedPopulation;

		public uint m_SimulationFrame;

		public void Execute()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			if (m_Queue.IsEmpty())
			{
				return;
			}
			Random random = m_RandomSeed.GetRandom(0);
			ChirpCreationData chirpCreationData = default(ChirpCreationData);
			RandomLikeCountData randomLikeCountData = default(RandomLikeCountData);
			LifePathEventData lifePathEventData = default(LifePathEventData);
			HouseholdMember householdMember = default(HouseholdMember);
			DynamicBuffer<HouseholdCitizen> val4 = default(DynamicBuffer<HouseholdCitizen>);
			while (m_Queue.TryDequeue(ref chirpCreationData))
			{
				bool isChirperChirp;
				Entity chirpPrefab = GetChirpPrefab(chirpCreationData.m_TriggerPrefab, ref random, out isChirperChirp);
				if (chirpPrefab == Entity.Null || (isChirperChirp && m_RecentChirps.ContainsKey(chirpPrefab)))
				{
					continue;
				}
				EntityArchetype archetype = GetArchetype(chirpPrefab);
				if (!((EntityArchetype)(ref archetype)).Valid)
				{
					continue;
				}
				Entity val = (isChirperChirp ? FindSender(chirpCreationData.m_Sender, chirpCreationData.m_Target, chirpPrefab, ref random) : chirpCreationData.m_Sender);
				if (val == Entity.Null)
				{
					continue;
				}
				float num = ((Random)(ref random)).NextFloat(0.2f, 1f);
				float num2 = ((Random)(ref random)).NextFloat(0.001f, 0.03f);
				int viralFactor = ((Random)(ref random)).NextInt(5, 100);
				int num3 = m_EducatedPopulation + m_UneducatedPopulation;
				float continuousFactor = 0.2f;
				if (m_RandomLikeCountData.TryGetComponent(chirpPrefab, ref randomLikeCountData))
				{
					num2 = ((Random)(ref random)).NextFloat(randomLikeCountData.m_RandomAmountFactor.x, randomLikeCountData.m_RandomAmountFactor.y);
					num3 = (int)((float)m_EducatedPopulation * randomLikeCountData.m_EducatedPercentage + (float)m_UneducatedPopulation * randomLikeCountData.m_UneducatedPercentage);
					viralFactor = ((Random)(ref random)).NextInt(randomLikeCountData.m_GoViralFactor.x, randomLikeCountData.m_GoViralFactor.y + 1);
					num = ((Random)(ref random)).NextFloat(m_RandomLikeCountData[chirpPrefab].m_ActiveDays.x, m_RandomLikeCountData[chirpPrefab].m_ActiveDays.y);
					continuousFactor = randomLikeCountData.m_ContinuousFactor;
				}
				int targetLikes = (int)((float)num3 * num2);
				Entity val2 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(archetype);
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Chirp>(val2, new Chirp(val, m_SimulationFrame)
				{
					m_TargetLikes = (uint)targetLikes,
					m_InactiveFrame = (uint)((float)m_SimulationFrame + num * 262144f),
					m_ViralFactor = viralFactor,
					m_ContinuousFactor = continuousFactor,
					m_Likes = (uint)math.min(num3, ((Random)(ref random)).NextInt(5))
				});
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PrefabRef>(val2, new PrefabRef(chirpPrefab));
				DynamicBuffer<ChirpEntity> val3 = ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<ChirpEntity>(val2);
				if (val != Entity.Null)
				{
					val3.Add(new ChirpEntity(val));
				}
				if (m_LifepathEventData.TryGetComponent(chirpPrefab, ref lifePathEventData) && lifePathEventData.m_EventType == LifePathEventType.CitizenCoupleMadeBaby && m_HouseholdMembers.TryGetComponent(chirpCreationData.m_Sender, ref householdMember) && m_HouseholdCitizens.TryGetBuffer(householdMember.m_Household, ref val4))
				{
					for (int i = 0; i < val4.Length; i++)
					{
						Entity citizen = val4[i].m_Citizen;
						if (m_Citizens.HasComponent(citizen) && m_Citizens[citizen].GetAge() == CitizenAge.Adult && val4[i].m_Citizen != val)
						{
							val3.Add(new ChirpEntity(val4[i].m_Citizen));
							break;
						}
					}
				}
				if (chirpCreationData.m_Target != Entity.Null)
				{
					val3.Add(new ChirpEntity(chirpCreationData.m_Target));
				}
				if (isChirperChirp)
				{
					m_RecentChirps.Add(chirpPrefab, val2);
				}
				else if (m_LifepathEntries.HasBuffer(val))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AppendToBuffer<LifePathEntry>(val, new LifePathEntry(val2));
				}
			}
		}

		private Entity GetChirpPrefab(Entity triggerPrefab, ref Random random, out bool isChirperChirp)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<TriggerChirpData> val = default(DynamicBuffer<TriggerChirpData>);
			if (m_TriggerChirpData.TryGetBuffer(triggerPrefab, ref val))
			{
				isChirperChirp = true;
				if (val.Length <= 0)
				{
					return Entity.Null;
				}
				return val[((Random)(ref random)).NextInt(val.Length)].m_Chirp;
			}
			isChirperChirp = false;
			return triggerPrefab;
		}

		private EntityArchetype GetArchetype(Entity prefab)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			ChirpData chirpData = default(ChirpData);
			if (m_ChirpData.TryGetComponent(prefab, ref chirpData))
			{
				return chirpData.m_Archetype;
			}
			LifePathEventData lifePathEventData = default(LifePathEventData);
			if (m_LifepathEventData.TryGetComponent(prefab, ref lifePathEventData))
			{
				return lifePathEventData.m_ChirpArchetype;
			}
			return default(EntityArchetype);
		}

		private Entity FindSender(Entity sender, Entity target, Entity prefab, ref Random random)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			ServiceChirpData serviceChirpData = default(ServiceChirpData);
			if (m_ServiceChirpDatas.TryGetComponent(prefab, ref serviceChirpData))
			{
				return serviceChirpData.m_Account;
			}
			if (m_BrandChirpData.HasComponent(prefab))
			{
				return sender;
			}
			DynamicBuffer<Employee> employees = default(DynamicBuffer<Employee>);
			if (m_Employees.TryGetBuffer(sender, ref employees) && employees.Length > 0)
			{
				return SelectRandomSender(employees, ref random);
			}
			DynamicBuffer<HouseholdCitizen> citizens = default(DynamicBuffer<HouseholdCitizen>);
			if (m_HouseholdCitizens.TryGetBuffer(sender, ref citizens) && citizens.Length > 0)
			{
				return SelectRandomSender(citizens, ref random);
			}
			return SelectRandomSender(ref random);
		}

		private Entity SelectRandomSender(DynamicBuffer<Employee> employees, ref Random random)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			Entity result = Entity.Null;
			int num = 0;
			for (int i = 0; i < employees.Length; i++)
			{
				Entity worker = employees[i].m_Worker;
				if (m_Citizens.HasComponent(worker) && !CitizenUtils.IsDead(worker, ref m_HealthProblems) && ((Random)(ref random)).NextInt(++num) == 0)
				{
					result = worker;
				}
			}
			return result;
		}

		private Entity SelectRandomSender(DynamicBuffer<HouseholdCitizen> citizens, ref Random random)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			Entity result = Entity.Null;
			int num = 0;
			for (int i = 0; i < citizens.Length; i++)
			{
				Entity citizen = citizens[i].m_Citizen;
				if (m_Citizens.HasComponent(citizen) && !CitizenUtils.IsDead(citizen, ref m_HealthProblems) && ((Random)(ref random)).NextInt(++num) == 0)
				{
					result = citizen;
				}
			}
			return result;
		}

		private Entity SelectRandomSender(ref Random random)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<ArchetypeChunk> val = m_RandomCitizenChunks.AsArray();
			int length = val.Length;
			if (length != 0)
			{
				Citizen citizen = default(Citizen);
				for (int i = 0; i < 100; i++)
				{
					ArchetypeChunk val2 = val[((Random)(ref random)).NextInt(length)];
					NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray(m_EntityType);
					Entity val3 = nativeArray[((Random)(ref random)).NextInt(nativeArray.Length)];
					if (!CitizenUtils.IsDead(val3, ref m_HealthProblems) && CitizenUtils.HasMovedIn(val3, ref m_HouseholdMembers, ref m_Households, ref m_HomelessHouseholds) && m_Citizens.TryGetComponent(val3, ref citizen) && citizen.GetAge() == CitizenAge.Adult)
					{
						return val3;
					}
				}
				int num = ((Random)(ref random)).NextInt(length);
				Citizen citizen2 = default(Citizen);
				for (int j = 0; j < length; j++)
				{
					ArchetypeChunk val4 = val[num++];
					num = math.select(num, 0, num == length);
					NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref val4)).GetNativeArray(m_EntityType);
					int length2 = nativeArray2.Length;
					int num2 = ((Random)(ref random)).NextInt(length2);
					for (int k = 0; k < length2; k++)
					{
						Entity val5 = nativeArray2[num2++];
						num2 = math.select(num2, 0, num2 == length2);
						if (!CitizenUtils.IsDead(val5, ref m_HealthProblems) && CitizenUtils.HasMovedIn(val5, ref m_HouseholdMembers, ref m_Households, ref m_HomelessHouseholds) && m_Citizens.TryGetComponent(val5, ref citizen2) && citizen2.GetAge() == CitizenAge.Adult)
						{
							return val5;
						}
					}
				}
			}
			return Entity.Null;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Chirp> __Game_Triggers_Chirp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<ChirpData> __Game_Prefabs_ChirpData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<TriggerChirpData> __Game_Prefabs_TriggerChirpData_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<LifePathEventData> __Game_Prefabs_LifePathEventData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BrandChirpData> __Game_Prefabs_BrandChirpData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RandomLikeCountData> __Game_Prefabs_RandomLikeCountData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ServiceChirpData> __Game_Prefabs_ServiceChirpData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HealthProblem> __Game_Citizens_HealthProblem_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HouseholdMember> __Game_Citizens_HouseholdMember_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Household> __Game_Citizens_Household_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HomelessHousehold> __Game_Citizens_HomelessHousehold_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Employee> __Game_Companies_Employee_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LifePathEntry> __Game_Triggers_LifePathEntry_RO_BufferLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Triggers_Chirp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Chirp>(true);
			__Game_Prefabs_ChirpData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ChirpData>(true);
			__Game_Prefabs_TriggerChirpData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TriggerChirpData>(true);
			__Game_Prefabs_LifePathEventData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LifePathEventData>(true);
			__Game_Prefabs_BrandChirpData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BrandChirpData>(true);
			__Game_Prefabs_RandomLikeCountData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RandomLikeCountData>(true);
			__Game_Prefabs_ServiceChirpData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceChirpData>(true);
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
			__Game_Citizens_HealthProblem_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HealthProblem>(true);
			__Game_Citizens_HouseholdMember_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HouseholdMember>(true);
			__Game_Citizens_Household_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Household>(true);
			__Game_Citizens_HomelessHousehold_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HomelessHousehold>(true);
			__Game_Citizens_HouseholdCitizen_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdCitizen>(true);
			__Game_Companies_Employee_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Employee>(true);
			__Game_Triggers_LifePathEntry_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LifePathEntry>(true);
		}
	}

	private SimulationSystem m_SimulationSystem;

	private CityStatisticsSystem m_CityStatisticsSystem;

	private ModificationEndBarrier m_ModificationBarrier;

	private JobHandle m_WriteDependencies;

	private EntityQuery m_PrefabQuery;

	private EntityQuery m_ChirpQuery;

	private EntityQuery m_CitizenQuery;

	private NativeQueue<ChirpCreationData> m_Queue;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationEndBarrier>();
		m_PrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ChirpData>() });
		m_ChirpQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Chirp>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_CitizenQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Citizen>(),
			ComponentType.ReadOnly<HouseholdMember>(),
			ComponentType.Exclude<Deleted>()
		});
		m_Queue = new NativeQueue<ChirpCreationData>(AllocatorHandle.op_Implicit((Allocator)4));
		((ComponentSystemBase)this).Enabled = false;
	}

	protected override void OnGamePreload(Purpose purpose, GameMode mode)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGamePreload(purpose, mode);
		((ComponentSystemBase)this).Enabled = mode.IsGame();
	}

	[Preserve]
	protected override void OnDestroy()
	{
		((JobHandle)(ref m_WriteDependencies)).Complete();
		m_Queue.Dispose();
		base.OnDestroy();
	}

	public NativeQueue<ChirpCreationData> GetQueue(out JobHandle deps)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		Assert.IsTrue(((ComponentSystemBase)this).Enabled, "Can not write to queue when system isn't running");
		deps = m_WriteDependencies;
		return m_Queue;
	}

	public void AddQueueWriter(JobHandle handle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_WriteDependencies = JobHandle.CombineDependencies(m_WriteDependencies, handle);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		int num = ((EntityQuery)(ref m_PrefabQuery)).CalculateEntityCount();
		NativeParallelHashMap<Entity, Entity> recentChirps = default(NativeParallelHashMap<Entity, Entity>);
		recentChirps._002Ector(num, AllocatorHandle.op_Implicit((Allocator)3));
		JobHandle val = default(JobHandle);
		if (!((EntityQuery)(ref m_ChirpQuery)).IsEmptyIgnoreFilter)
		{
			val = JobChunkExtensions.ScheduleParallel<CollectRecentChirpsJob>(new CollectRecentChirpsJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ChirpType = InternalCompilerInterface.GetComponentTypeHandle<Chirp>(ref __TypeHandle.__Game_Triggers_Chirp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ChirpDatas = InternalCompilerInterface.GetComponentLookup<ChirpData>(ref __TypeHandle.__Game_Prefabs_ChirpData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_RecentChirps = recentChirps.AsParallelWriter(),
				m_SimulationFrame = m_SimulationSystem.frameIndex
			}, m_ChirpQuery, ((SystemBase)this).Dependency);
		}
		JobHandle val2 = default(JobHandle);
		NativeList<ArchetypeChunk> randomCitizenChunks = ((EntityQuery)(ref m_CitizenQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val2);
		CreateChirpJob createChirpJob = new CreateChirpJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TriggerChirpData = InternalCompilerInterface.GetBufferLookup<TriggerChirpData>(ref __TypeHandle.__Game_Prefabs_TriggerChirpData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ChirpData = InternalCompilerInterface.GetComponentLookup<ChirpData>(ref __TypeHandle.__Game_Prefabs_ChirpData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LifepathEventData = InternalCompilerInterface.GetComponentLookup<LifePathEventData>(ref __TypeHandle.__Game_Prefabs_LifePathEventData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BrandChirpData = InternalCompilerInterface.GetComponentLookup<BrandChirpData>(ref __TypeHandle.__Game_Prefabs_BrandChirpData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RandomLikeCountData = InternalCompilerInterface.GetComponentLookup<RandomLikeCountData>(ref __TypeHandle.__Game_Prefabs_RandomLikeCountData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceChirpDatas = InternalCompilerInterface.GetComponentLookup<ServiceChirpData>(ref __TypeHandle.__Game_Prefabs_ServiceChirpData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Citizens = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HealthProblems = InternalCompilerInterface.GetComponentLookup<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdMembers = InternalCompilerInterface.GetComponentLookup<HouseholdMember>(ref __TypeHandle.__Game_Citizens_HouseholdMember_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Households = InternalCompilerInterface.GetComponentLookup<Household>(ref __TypeHandle.__Game_Citizens_Household_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HomelessHouseholds = InternalCompilerInterface.GetComponentLookup<HomelessHousehold>(ref __TypeHandle.__Game_Citizens_HomelessHousehold_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdCitizens = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Employees = InternalCompilerInterface.GetBufferLookup<Employee>(ref __TypeHandle.__Game_Companies_Employee_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LifepathEntries = InternalCompilerInterface.GetBufferLookup<LifePathEntry>(ref __TypeHandle.__Game_Triggers_LifePathEntry_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer(),
			m_Queue = m_Queue,
			m_RecentChirps = recentChirps,
			m_RandomCitizenChunks = randomCitizenChunks,
			m_RandomSeed = RandomSeed.Next(),
			m_UneducatedPopulation = m_CityStatisticsSystem.GetStatisticValue(StatisticType.EducationCount) + m_CityStatisticsSystem.GetStatisticValue(StatisticType.EducationCount, 1),
			m_EducatedPopulation = m_CityStatisticsSystem.GetStatisticValue(StatisticType.EducationCount, 2) + m_CityStatisticsSystem.GetStatisticValue(StatisticType.EducationCount, 3) + m_CityStatisticsSystem.GetStatisticValue(StatisticType.EducationCount, 4),
			m_SimulationFrame = m_SimulationSystem.frameIndex
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<CreateChirpJob>(createChirpJob, JobUtils.CombineDependencies(((SystemBase)this).Dependency, m_WriteDependencies, val2, val));
		recentChirps.Dispose(((SystemBase)this).Dependency);
		randomCitizenChunks.Dispose(((SystemBase)this).Dependency);
		m_WriteDependencies = ((SystemBase)this).Dependency;
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(((SystemBase)this).Dependency);
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
	public CreateChirpSystem()
	{
	}
}
