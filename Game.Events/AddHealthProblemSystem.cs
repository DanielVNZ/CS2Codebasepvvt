using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Notifications;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Simulation;
using Game.Triggers;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Events;

[CompilerGenerated]
public class AddHealthProblemSystem : GameSystemBase
{
	[BurstCompile]
	private struct FindCitizensInBuildingJob : IJobChunk
	{
		[ReadOnly]
		public Entity m_Event;

		[ReadOnly]
		public Entity m_Building;

		[ReadOnly]
		public HealthProblemFlags m_Flags;

		[ReadOnly]
		public float m_DeathProbability;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentBuilding> m_CurrentBuildingType;

		[ReadOnly]
		public ComponentTypeHandle<HouseholdMember> m_HouseholdMemberType;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> m_HouseholdCitizens;

		public ParallelWriter<AddHealthProblem> m_AddQueue;

		public ParallelWriter<StatisticsEvent> m_StatisticsEventQueue;

		public ParallelWriter<TriggerAction> m_TriggerBuffer;

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
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<CurrentBuilding> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentBuilding>(ref m_CurrentBuildingType);
			NativeArray<HouseholdMember> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HouseholdMember>(ref m_HouseholdMemberType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				if (nativeArray2[i].m_CurrentBuilding == m_Building)
				{
					AddHealthProblem addHealthProblem = new AddHealthProblem
					{
						m_Event = m_Event,
						m_Target = nativeArray[i],
						m_Flags = m_Flags
					};
					if (m_DeathProbability > 0f && ((Random)(ref random)).NextFloat(1f) < m_DeathProbability)
					{
						addHealthProblem.m_Flags |= HealthProblemFlags.Dead | HealthProblemFlags.RequireTransport;
						Entity household = ((nativeArray3.Length != 0) ? nativeArray3[i].m_Household : Entity.Null);
						DeathCheckSystem.PerformAfterDeathActions(nativeArray[i], household, m_TriggerBuffer, m_StatisticsEventQueue, ref m_HouseholdCitizens);
					}
					m_AddQueue.Enqueue(addHealthProblem);
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct AddHealthProblemJob : IJob
	{
		[DeallocateOnJobCompletion]
		[ReadOnly]
		public NativeArray<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public ComponentTypeHandle<AddHealthProblem> m_AddHealthProblemType;

		[ReadOnly]
		public ComponentLookup<CurrentTransport> m_CurrentTransportData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public HealthcareParameterData m_HealthcareParameterData;

		public IconCommandBuffer m_IconCommandBuffer;

		public ComponentLookup<HealthProblem> m_HealthProblemData;

		public ComponentLookup<PathOwner> m_PathOwnerData;

		public ComponentLookup<Target> m_TargetData;

		public BufferLookup<TargetElement> m_TargetElements;

		public EntityArchetype m_JournalDataArchetype;

		public NativeQueue<AddHealthProblem> m_AddQueue;

		public EntityCommandBuffer m_CommandBuffer;

		public NativeQueue<TriggerAction> m_TriggerBuffer;

		public void Execute()
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_040d: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0413: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_043f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0441: Unknown result type (might be due to invalid IL or missing references)
			//IL_0445: Unknown result type (might be due to invalid IL or missing references)
			//IL_046f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0471: Unknown result type (might be due to invalid IL or missing references)
			//IL_0475: Unknown result type (might be due to invalid IL or missing references)
			//IL_049f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
			int count = m_AddQueue.Count;
			int num = count;
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				int num2 = num;
				ArchetypeChunk val = m_Chunks[i];
				num = num2 + ((ArchetypeChunk)(ref val)).Count;
			}
			NativeParallelHashMap<Entity, HealthProblem> val2 = default(NativeParallelHashMap<Entity, HealthProblem>);
			val2._002Ector(num, AllocatorHandle.op_Implicit((Allocator)2));
			HealthProblem problem = default(HealthProblem);
			for (int j = 0; j < m_Chunks.Length; j++)
			{
				ArchetypeChunk val3 = m_Chunks[j];
				NativeArray<AddHealthProblem> nativeArray = ((ArchetypeChunk)(ref val3)).GetNativeArray<AddHealthProblem>(ref m_AddHealthProblemType);
				for (int k = 0; k < nativeArray.Length; k++)
				{
					AddHealthProblem addHealthProblem = nativeArray[k];
					if (m_PrefabRefData.HasComponent(addHealthProblem.m_Target))
					{
						HealthProblem healthProblem = new HealthProblem(addHealthProblem.m_Event, addHealthProblem.m_Flags);
						if (val2.TryGetValue(addHealthProblem.m_Target, ref problem))
						{
							val2[addHealthProblem.m_Target] = MergeProblems(problem, healthProblem);
						}
						else if (m_HealthProblemData.HasComponent(addHealthProblem.m_Target))
						{
							problem = m_HealthProblemData[addHealthProblem.m_Target];
							val2.TryAdd(addHealthProblem.m_Target, MergeProblems(problem, healthProblem));
						}
						else
						{
							val2.TryAdd(addHealthProblem.m_Target, healthProblem);
						}
					}
				}
			}
			HealthProblem problem2 = default(HealthProblem);
			for (int l = 0; l < count; l++)
			{
				AddHealthProblem addHealthProblem2 = m_AddQueue.Dequeue();
				if (m_PrefabRefData.HasComponent(addHealthProblem2.m_Target))
				{
					HealthProblem healthProblem2 = new HealthProblem(addHealthProblem2.m_Event, addHealthProblem2.m_Flags);
					if (val2.TryGetValue(addHealthProblem2.m_Target, ref problem2))
					{
						val2[addHealthProblem2.m_Target] = MergeProblems(problem2, healthProblem2);
					}
					else if (m_HealthProblemData.HasComponent(addHealthProblem2.m_Target))
					{
						problem2 = m_HealthProblemData[addHealthProblem2.m_Target];
						val2.TryAdd(addHealthProblem2.m_Target, MergeProblems(problem2, healthProblem2));
					}
					else
					{
						val2.TryAdd(addHealthProblem2.m_Target, healthProblem2);
					}
				}
			}
			if (val2.Count() != 0)
			{
				NativeArray<Entity> keyArray = val2.GetKeyArray(AllocatorHandle.op_Implicit((Allocator)2));
				for (int m = 0; m < keyArray.Length; m++)
				{
					Entity val4 = keyArray[m];
					HealthProblem healthProblem3 = val2[val4];
					if (m_HealthProblemData.HasComponent(val4))
					{
						HealthProblem oldProblem = m_HealthProblemData[val4];
						if (oldProblem.m_Event != healthProblem3.m_Event && m_TargetElements.HasBuffer(healthProblem3.m_Event))
						{
							CollectionUtils.TryAddUniqueValue<TargetElement>(m_TargetElements[healthProblem3.m_Event], new TargetElement(val4));
						}
						if ((oldProblem.m_Flags & (HealthProblemFlags.Dead | HealthProblemFlags.RequireTransport)) == HealthProblemFlags.RequireTransport && (healthProblem3.m_Flags & HealthProblemFlags.Dead) != HealthProblemFlags.None)
						{
							m_IconCommandBuffer.Remove(val4, m_HealthcareParameterData.m_AmbulanceNotificationPrefab);
							healthProblem3.m_Timer = 0;
						}
						if ((healthProblem3.m_Flags & (HealthProblemFlags.Dead | HealthProblemFlags.Injured)) != HealthProblemFlags.None && (healthProblem3.m_Flags & HealthProblemFlags.RequireTransport) != HealthProblemFlags.None && ((oldProblem.m_Flags & (HealthProblemFlags.Dead | HealthProblemFlags.Injured)) == 0 || (oldProblem.m_Flags & HealthProblemFlags.RequireTransport) == 0))
						{
							StopMoving(val4);
						}
						AddJournalData(oldProblem, healthProblem3);
						m_HealthProblemData[val4] = healthProblem3;
					}
					else
					{
						if (m_TargetElements.HasBuffer(healthProblem3.m_Event))
						{
							CollectionUtils.TryAddUniqueValue<TargetElement>(m_TargetElements[healthProblem3.m_Event], new TargetElement(val4));
						}
						if ((healthProblem3.m_Flags & (HealthProblemFlags.Dead | HealthProblemFlags.Injured)) != HealthProblemFlags.None && (healthProblem3.m_Flags & HealthProblemFlags.RequireTransport) != HealthProblemFlags.None)
						{
							StopMoving(val4);
						}
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<HealthProblem>(val4, healthProblem3);
						AddJournalData(healthProblem3);
					}
					Entity triggerPrefab = Entity.Null;
					if (m_PrefabRefData.HasComponent(healthProblem3.m_Event))
					{
						triggerPrefab = m_PrefabRefData[healthProblem3.m_Event].m_Prefab;
					}
					if ((healthProblem3.m_Flags & HealthProblemFlags.Sick) != HealthProblemFlags.None)
					{
						m_TriggerBuffer.Enqueue(new TriggerAction(TriggerType.CitizenGotSick, triggerPrefab, val4, healthProblem3.m_Event));
					}
					else if ((healthProblem3.m_Flags & HealthProblemFlags.Injured) != HealthProblemFlags.None)
					{
						m_TriggerBuffer.Enqueue(new TriggerAction(TriggerType.CitizenGotInjured, triggerPrefab, val4, healthProblem3.m_Event));
					}
					else if ((healthProblem3.m_Flags & HealthProblemFlags.Trapped) != HealthProblemFlags.None)
					{
						m_TriggerBuffer.Enqueue(new TriggerAction(TriggerType.CitizenGotTrapped, triggerPrefab, val4, healthProblem3.m_Event));
					}
					else if ((healthProblem3.m_Flags & HealthProblemFlags.InDanger) != HealthProblemFlags.None)
					{
						m_TriggerBuffer.Enqueue(new TriggerAction(TriggerType.CitizenGotInDanger, triggerPrefab, val4, healthProblem3.m_Event));
					}
				}
			}
			val2.Dispose();
		}

		private void StopMoving(Entity citizen)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			if (m_CurrentTransportData.HasComponent(citizen))
			{
				CurrentTransport currentTransport = m_CurrentTransportData[citizen];
				if (m_PathOwnerData.HasComponent(currentTransport.m_CurrentTransport))
				{
					PathOwner pathOwner = m_PathOwnerData[currentTransport.m_CurrentTransport];
					pathOwner.m_State &= ~PathFlags.Failed;
					pathOwner.m_State |= PathFlags.Obsolete;
					m_PathOwnerData[currentTransport.m_CurrentTransport] = pathOwner;
				}
				if (m_TargetData.HasComponent(currentTransport.m_CurrentTransport))
				{
					m_TargetData[currentTransport.m_CurrentTransport] = default(Target);
				}
			}
		}

		private void AddJournalData(HealthProblem problem)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			if ((problem.m_Flags & (HealthProblemFlags.Sick | HealthProblemFlags.Dead | HealthProblemFlags.Injured)) != HealthProblemFlags.None)
			{
				Entity val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(m_JournalDataArchetype);
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<AddEventJournalData>(val, new AddEventJournalData(problem.m_Event, EventDataTrackingType.Casualties));
			}
		}

		private void AddJournalData(HealthProblem oldProblem, HealthProblem newProblem)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			if (oldProblem.m_Event != newProblem.m_Event)
			{
				AddJournalData(newProblem);
			}
			else if ((oldProblem.m_Flags & (HealthProblemFlags.Sick | HealthProblemFlags.Dead | HealthProblemFlags.Injured)) == 0)
			{
				AddJournalData(newProblem);
			}
		}

		private HealthProblem MergeProblems(HealthProblem problem1, HealthProblem problem2)
		{
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			HealthProblemFlags healthProblemFlags = problem1.m_Flags ^ problem2.m_Flags;
			if ((healthProblemFlags & HealthProblemFlags.Dead) != HealthProblemFlags.None)
			{
				if ((problem1.m_Flags & HealthProblemFlags.Dead) == 0)
				{
					return problem2;
				}
				return problem1;
			}
			HealthProblem result;
			if ((healthProblemFlags & HealthProblemFlags.RequireTransport) != HealthProblemFlags.None)
			{
				result = (((problem1.m_Flags & HealthProblemFlags.RequireTransport) != HealthProblemFlags.None) ? problem1 : problem2);
				result.m_Flags |= (((problem1.m_Flags & HealthProblemFlags.RequireTransport) != HealthProblemFlags.None) ? problem2.m_Flags : problem1.m_Flags);
			}
			else if (problem1.m_Event != Entity.Null != (problem2.m_Event != Entity.Null))
			{
				result = ((problem1.m_Event != Entity.Null) ? problem1 : problem2);
				result.m_Flags |= ((problem1.m_Event != Entity.Null) ? problem2.m_Flags : problem1.m_Flags);
			}
			else
			{
				result = problem1;
				result.m_Flags |= problem2.m_Flags;
			}
			return result;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<Ignite> __Game_Events_Ignite_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Destroy> __Game_Objects_Destroy_RO_ComponentTypeHandle;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentBuilding> __Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<HouseholdMember> __Game_Citizens_HouseholdMember_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferLookup;

		[ReadOnly]
		public ComponentTypeHandle<AddHealthProblem> __Game_Events_AddHealthProblem_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<CurrentTransport> __Game_Citizens_CurrentTransport_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		public ComponentLookup<HealthProblem> __Game_Citizens_HealthProblem_RW_ComponentLookup;

		public ComponentLookup<PathOwner> __Game_Pathfind_PathOwner_RW_ComponentLookup;

		public ComponentLookup<Target> __Game_Common_Target_RW_ComponentLookup;

		public BufferLookup<TargetElement> __Game_Events_TargetElement_RW_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
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
			__Game_Events_Ignite_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Ignite>(true);
			__Game_Objects_Destroy_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Destroy>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentBuilding>(true);
			__Game_Citizens_HouseholdMember_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HouseholdMember>(true);
			__Game_Citizens_HouseholdCitizen_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdCitizen>(true);
			__Game_Events_AddHealthProblem_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AddHealthProblem>(true);
			__Game_Citizens_CurrentTransport_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentTransport>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Citizens_HealthProblem_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HealthProblem>(false);
			__Game_Pathfind_PathOwner_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathOwner>(false);
			__Game_Common_Target_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Target>(false);
			__Game_Events_TargetElement_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TargetElement>(false);
		}
	}

	private IconCommandSystem m_IconCommandSystem;

	private ModificationBarrier4 m_ModificationBarrier;

	private EntityQuery m_AddHealthProblemQuery;

	private EntityQuery m_HealthcareSettingsQuery;

	private EntityQuery m_CitizenQuery;

	private EntityArchetype m_JournalDataArchetype;

	private TriggerSystem m_TriggerSystem;

	private CityStatisticsSystem m_CityStatisticsSystem;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier4>();
		m_IconCommandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IconCommandSystem>();
		m_TriggerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TriggerSystem>();
		m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Common.Event>() };
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<AddHealthProblem>(),
			ComponentType.ReadOnly<Ignite>(),
			ComponentType.ReadOnly<Destroy>()
		};
		array[0] = val;
		m_AddHealthProblemQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_HealthcareSettingsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<HealthcareParameterData>() });
		m_CitizenQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Citizen>(),
			ComponentType.ReadOnly<CurrentBuilding>(),
			ComponentType.Exclude<Deleted>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_JournalDataArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<AddEventJournalData>(),
			ComponentType.ReadWrite<Game.Common.Event>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_AddHealthProblemQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_HealthcareSettingsQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_0515: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<ArchetypeChunk> chunks = ((EntityQuery)(ref m_AddHealthProblemQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		NativeQueue<AddHealthProblem> addQueue = default(NativeQueue<AddHealthProblem>);
		addQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		ParallelWriter<AddHealthProblem> addQueue2 = addQueue.AsParallelWriter();
		ComponentTypeHandle<Ignite> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<Ignite>(ref __TypeHandle.__Game_Events_Ignite_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<Destroy> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<Destroy>(ref __TypeHandle.__Game_Objects_Destroy_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		for (int i = 0; i < chunks.Length; i++)
		{
			ArchetypeChunk val = chunks[i];
			NativeArray<Ignite> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<Ignite>(ref componentTypeHandle);
			NativeArray<Destroy> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<Destroy>(ref componentTypeHandle2);
			EntityManager entityManager;
			for (int j = 0; j < nativeArray.Length; j++)
			{
				Ignite ignite = nativeArray[j];
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<Building>(ignite.m_Target))
				{
					JobHandle deps;
					FindCitizensInBuildingJob findCitizensInBuildingJob = new FindCitizensInBuildingJob
					{
						m_Event = ignite.m_Event,
						m_Building = ignite.m_Target,
						m_Flags = HealthProblemFlags.InDanger,
						m_DeathProbability = 0f,
						m_RandomSeed = RandomSeed.Next(),
						m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
						m_CurrentBuildingType = InternalCompilerInterface.GetComponentTypeHandle<CurrentBuilding>(ref __TypeHandle.__Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
						m_HouseholdMemberType = InternalCompilerInterface.GetComponentTypeHandle<HouseholdMember>(ref __TypeHandle.__Game_Citizens_HouseholdMember_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
						m_HouseholdCitizens = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
						m_TriggerBuffer = m_TriggerSystem.CreateActionBuffer().AsParallelWriter(),
						m_StatisticsEventQueue = m_CityStatisticsSystem.GetStatisticsEventQueue(out deps).AsParallelWriter(),
						m_AddQueue = addQueue2
					};
					((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<FindCitizensInBuildingJob>(findCitizensInBuildingJob, m_CitizenQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, deps));
					m_CityStatisticsSystem.AddWriter(((SystemBase)this).Dependency);
					m_TriggerSystem.AddActionBufferWriter(((SystemBase)this).Dependency);
				}
			}
			for (int k = 0; k < nativeArray2.Length; k++)
			{
				Destroy destroy = nativeArray2[k];
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<Building>(destroy.m_Object))
				{
					JobHandle deps2;
					FindCitizensInBuildingJob findCitizensInBuildingJob2 = new FindCitizensInBuildingJob
					{
						m_Event = destroy.m_Event,
						m_Building = destroy.m_Object,
						m_Flags = HealthProblemFlags.Trapped,
						m_DeathProbability = ((EntityQuery)(ref m_HealthcareSettingsQuery)).GetSingleton<HealthcareParameterData>().m_BuildingDestoryDeathRate,
						m_RandomSeed = RandomSeed.Next(),
						m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
						m_CurrentBuildingType = InternalCompilerInterface.GetComponentTypeHandle<CurrentBuilding>(ref __TypeHandle.__Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
						m_HouseholdMemberType = InternalCompilerInterface.GetComponentTypeHandle<HouseholdMember>(ref __TypeHandle.__Game_Citizens_HouseholdMember_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
						m_HouseholdCitizens = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
						m_TriggerBuffer = m_TriggerSystem.CreateActionBuffer().AsParallelWriter(),
						m_StatisticsEventQueue = m_CityStatisticsSystem.GetStatisticsEventQueue(out deps2).AsParallelWriter(),
						m_AddQueue = addQueue2
					};
					((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<FindCitizensInBuildingJob>(findCitizensInBuildingJob2, m_CitizenQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, deps2));
					m_CityStatisticsSystem.AddWriter(((SystemBase)this).Dependency);
					m_TriggerSystem.AddActionBufferWriter(((SystemBase)this).Dependency);
				}
			}
		}
		AddHealthProblemJob addHealthProblemJob = new AddHealthProblemJob
		{
			m_Chunks = chunks,
			m_AddHealthProblemType = InternalCompilerInterface.GetComponentTypeHandle<AddHealthProblem>(ref __TypeHandle.__Game_Events_AddHealthProblem_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentTransportData = InternalCompilerInterface.GetComponentLookup<CurrentTransport>(ref __TypeHandle.__Game_Citizens_CurrentTransport_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HealthcareParameterData = ((EntityQuery)(ref m_HealthcareSettingsQuery)).GetSingleton<HealthcareParameterData>(),
			m_IconCommandBuffer = m_IconCommandSystem.CreateCommandBuffer(),
			m_HealthProblemData = InternalCompilerInterface.GetComponentLookup<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathOwnerData = InternalCompilerInterface.GetComponentLookup<PathOwner>(ref __TypeHandle.__Game_Pathfind_PathOwner_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TargetData = InternalCompilerInterface.GetComponentLookup<Target>(ref __TypeHandle.__Game_Common_Target_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TargetElements = InternalCompilerInterface.GetBufferLookup<TargetElement>(ref __TypeHandle.__Game_Events_TargetElement_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_JournalDataArchetype = m_JournalDataArchetype,
			m_AddQueue = addQueue,
			m_TriggerBuffer = m_TriggerSystem.CreateActionBuffer(),
			m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer()
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<AddHealthProblemJob>(addHealthProblemJob, ((SystemBase)this).Dependency);
		addQueue.Dispose(((SystemBase)this).Dependency);
		m_TriggerSystem.AddActionBufferWriter(((SystemBase)this).Dependency);
		m_IconCommandSystem.AddCommandBufferWriter(((SystemBase)this).Dependency);
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
	public AddHealthProblemSystem()
	{
	}
}
