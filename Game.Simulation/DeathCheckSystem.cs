using System.Runtime.CompilerServices;
using Colossal;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Achievements;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Creatures;
using Game.Notifications;
using Game.Prefabs;
using Game.Tools;
using Game.Triggers;
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
public class DeathCheckSystem : GameSystemBase
{
	[BurstCompile]
	private struct DeathCheckJob : IJobChunk
	{
		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Citizen> m_CitizenType;

		[ReadOnly]
		public ComponentTypeHandle<Worker> m_WorkerType;

		[ReadOnly]
		public ComponentTypeHandle<HouseholdMember> m_HouseholdMemberType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Citizens.Student> m_StudentType;

		[ReadOnly]
		public ComponentTypeHandle<ResourceBuyer> m_ResourceBuyerType;

		[ReadOnly]
		public ComponentTypeHandle<Leisure> m_LeisureType;

		public ComponentTypeHandle<HealthProblem> m_HealthProblemType;

		[ReadOnly]
		public ComponentLookup<CurrentBuilding> m_CurrentBuildings;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Hospital> m_HospitalData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<Game.Creatures.Resident> m_ResidentData;

		[ReadOnly]
		public ComponentLookup<CurrentTransport> m_CurrentTransport;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifiers;

		[ReadOnly]
		public BufferLookup<Game.Buildings.Student> m_Students;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> m_HouseholdCitizens;

		[ReadOnly]
		public uint m_UpdateFrameIndex;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public HealthcareParameterData m_HealthcareParameterData;

		[ReadOnly]
		public Entity m_City;

		public ParallelWriter<StatisticsEvent> m_StatisticsEventQueue;

		public ParallelWriter<TriggerAction> m_TriggerBuffer;

		public IconCommandBuffer m_IconCommandBuffer;

		public ParallelWriter m_CommandBuffer;

		public Concurrent m_PatientsRecoveredCounter;

		public TimeSettingsData m_TimeSettings;

		public TimeData m_TimeData;

		public uint m_SimulationFrame;

		private void Die(ArchetypeChunk chunk, int chunkIndex, int i, Entity citizen, Entity household, NativeArray<Game.Citizens.Student> students, NativeArray<HealthProblem> healthProblems)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			if (!healthProblems.IsCreated)
			{
				HealthProblem healthProblem = new HealthProblem
				{
					m_Flags = (HealthProblemFlags.Dead | HealthProblemFlags.RequireTransport)
				};
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<HealthProblem>(chunkIndex, citizen, healthProblem);
			}
			else
			{
				HealthProblem healthProblem2 = healthProblems[i];
				if ((healthProblem2.m_Flags & HealthProblemFlags.RequireTransport) != HealthProblemFlags.None)
				{
					m_IconCommandBuffer.Remove(citizen, m_HealthcareParameterData.m_AmbulanceNotificationPrefab);
					healthProblem2.m_Timer = 0;
				}
				healthProblem2.m_Flags &= ~(HealthProblemFlags.Sick | HealthProblemFlags.Injured);
				healthProblem2.m_Flags |= HealthProblemFlags.Dead | HealthProblemFlags.RequireTransport;
				healthProblems[i] = healthProblem2;
			}
			PerformAfterDeathActions(citizen, household, m_TriggerBuffer, m_StatisticsEventQueue, ref m_HouseholdCitizens);
			if (((ArchetypeChunk)(ref chunk)).Has<Game.Citizens.Student>(ref m_StudentType))
			{
				Entity school = students[i].m_School;
				if (m_Students.HasBuffer(school))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<StudentsRemoved>(chunkIndex, school);
				}
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Game.Citizens.Student>(chunkIndex, citizen);
			}
			if (((ArchetypeChunk)(ref chunk)).Has<Worker>(ref m_WorkerType))
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Worker>(chunkIndex, citizen);
			}
			if (((ArchetypeChunk)(ref chunk)).Has<ResourceBuyer>(ref m_ResourceBuyerType))
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<ResourceBuyer>(chunkIndex, citizen);
			}
			if (((ArchetypeChunk)(ref chunk)).Has<Leisure>(ref m_LeisureType))
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Leisure>(chunkIndex, citizen);
			}
		}

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index != m_UpdateFrameIndex)
			{
				return;
			}
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			DynamicBuffer<CityModifier> modifiers = m_CityModifiers[m_City];
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<HealthProblem> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HealthProblem>(ref m_HealthProblemType);
			NativeArray<Citizen> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Citizen>(ref m_CitizenType);
			NativeArray<Game.Citizens.Student> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Citizens.Student>(ref m_StudentType);
			NativeArray<HouseholdMember> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HouseholdMember>(ref m_HouseholdMemberType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity val = nativeArray[i];
				Entity household = ((nativeArray5.Length != 0) ? nativeArray5[i].m_Household : Entity.Null);
				Citizen citizen = nativeArray3[i];
				if (m_CurrentTransport.HasComponent(val))
				{
					CurrentTransport currentTransport = m_CurrentTransport[val];
					if (m_ResidentData.HasComponent(currentTransport.m_CurrentTransport) && (m_ResidentData[currentTransport.m_CurrentTransport].m_Flags & ResidentFlags.InVehicle) != ResidentFlags.None)
					{
						continue;
					}
				}
				if (nativeArray2.IsCreated && (nativeArray2[i].m_Flags & HealthProblemFlags.Dead) != HealthProblemFlags.None)
				{
					continue;
				}
				float ageInDays = citizen.GetAgeInDays(m_SimulationFrame, m_TimeData);
				float num = 1f * ageInDays / (float)m_TimeSettings.m_DaysPerYear / (float)kMaxAgeInGameYear;
				Random pseudoRandom = citizen.GetPseudoRandom(CitizenPseudoRandom.Death);
				if (((Random)(ref pseudoRandom)).NextFloat() < ((AnimationCurve1)(ref m_HealthcareParameterData.m_DeathRate)).Evaluate(num))
				{
					Die(chunk, unfilteredChunkIndex, i, val, household, nativeArray4, nativeArray2);
				}
				else
				{
					if (!nativeArray2.IsCreated || (nativeArray2[i].m_Flags & (HealthProblemFlags.Sick | HealthProblemFlags.Injured)) == 0)
					{
						continue;
					}
					HealthProblem healthProblem = nativeArray2[i];
					int num2 = 10 - citizen.m_Health / 10;
					int num3 = num2 * num2;
					num3 += 8;
					if (((Random)(ref random)).NextInt(kUpdatesPerDay * 1000) <= num3)
					{
						Die(chunk, unfilteredChunkIndex, i, val, household, nativeArray4, nativeArray2);
						continue;
					}
					float num4 = MathUtils.Logistic(3f, 1000f, 6f, (float)num2 / 10f - 0.35f);
					int num5 = 0;
					if (m_CurrentBuildings.HasComponent(val))
					{
						Entity currentBuilding = m_CurrentBuildings[val].m_CurrentBuilding;
						if (m_BuildingData.HasComponent(currentBuilding) && !BuildingUtils.CheckOption(m_BuildingData[currentBuilding], BuildingOption.Inactive) && m_HospitalData.HasComponent(currentBuilding))
						{
							num5 = m_HospitalData[currentBuilding].m_TreatmentBonus;
						}
					}
					num4 -= (float)(10 * num5);
					CityUtils.ApplyModifier(ref num4, modifiers, CityModifierType.RecoveryFailChange);
					if (((Random)(ref random)).NextFloat(1000f) >= num4)
					{
						if ((healthProblem.m_Flags & HealthProblemFlags.RequireTransport) != HealthProblemFlags.None)
						{
							m_IconCommandBuffer.Remove(val, m_HealthcareParameterData.m_AmbulanceNotificationPrefab);
							healthProblem.m_Timer = 0;
						}
						healthProblem.m_Flags &= ~(HealthProblemFlags.Sick | HealthProblemFlags.Injured | HealthProblemFlags.RequireTransport);
						nativeArray2[i] = healthProblem;
						((Concurrent)(ref m_PatientsRecoveredCounter)).Increment();
					}
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
		public ComponentTypeHandle<Citizen> __Game_Citizens_Citizen_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Worker> __Game_Citizens_Worker_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<HouseholdMember> __Game_Citizens_HouseholdMember_RO_ComponentTypeHandle;

		public ComponentTypeHandle<HealthProblem> __Game_Citizens_HealthProblem_RW_ComponentTypeHandle;

		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Leisure> __Game_Citizens_Leisure_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ResourceBuyer> __Game_Companies_ResourceBuyer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Citizens.Student> __Game_Citizens_Student_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<CurrentBuilding> __Game_Citizens_CurrentBuilding_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Hospital> __Game_Buildings_Hospital_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentTransport> __Game_Citizens_CurrentTransport_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Creatures.Resident> __Game_Creatures_Resident_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<CityModifier> __Game_City_CityModifier_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Buildings.Student> __Game_Buildings_Student_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferLookup;

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
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Citizens_Citizen_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Citizen>(true);
			__Game_Citizens_Worker_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Worker>(true);
			__Game_Citizens_HouseholdMember_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HouseholdMember>(true);
			__Game_Citizens_HealthProblem_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HealthProblem>(false);
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Citizens_Leisure_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Leisure>(true);
			__Game_Companies_ResourceBuyer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ResourceBuyer>(true);
			__Game_Citizens_Student_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Citizens.Student>(true);
			__Game_Citizens_CurrentBuilding_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentBuilding>(true);
			__Game_Buildings_Hospital_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.Hospital>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Citizens_CurrentTransport_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentTransport>(true);
			__Game_Creatures_Resident_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Creatures.Resident>(true);
			__Game_City_CityModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(true);
			__Game_Buildings_Student_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Buildings.Student>(true);
			__Game_Citizens_HouseholdCitizen_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdCitizen>(true);
		}
	}

	public static readonly int kUpdatesPerDay = 4;

	public static readonly int kMaxAgeInGameYear = 9;

	private SimulationSystem m_SimulationSystem;

	private IconCommandSystem m_IconCommandSystem;

	private CitySystem m_CitySystem;

	private CityStatisticsSystem m_CityStatisticsSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_DeathCheckQuery;

	private EntityQuery m_HealthcareSettingsQuery;

	private EntityQuery m_TimeSettingsQuery;

	private EntityQuery m_TimeDataQuery;

	private TriggerSystem m_TriggerSystem;

	private AchievementTriggerSystem m_AchievementTriggerSystem;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / (kUpdatesPerDay * 16);
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_IconCommandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IconCommandSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_TriggerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TriggerSystem>();
		m_AchievementTriggerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AchievementTriggerSystem>();
		m_DeathCheckQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Citizen>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_HealthcareSettingsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<HealthcareParameterData>() });
		m_TimeSettingsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TimeSettingsData>() });
		m_TimeDataQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TimeData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_DeathCheckQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_HealthcareSettingsQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, ((EntityQuery)(ref m_HealthcareSettingsQuery)).GetSingleton<HealthcareParameterData>().m_HealthcareServicePrefab))
		{
			uint updateFrame = SimulationUtils.GetUpdateFrame(m_SimulationSystem.frameIndex, kUpdatesPerDay, 16);
			JobHandle deps;
			DeathCheckJob deathCheckJob = new DeathCheckJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CitizenType = InternalCompilerInterface.GetComponentTypeHandle<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_WorkerType = InternalCompilerInterface.GetComponentTypeHandle<Worker>(ref __TypeHandle.__Game_Citizens_Worker_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_HouseholdMemberType = InternalCompilerInterface.GetComponentTypeHandle<HouseholdMember>(ref __TypeHandle.__Game_Citizens_HouseholdMember_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_HealthProblemType = InternalCompilerInterface.GetComponentTypeHandle<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_LeisureType = InternalCompilerInterface.GetComponentTypeHandle<Leisure>(ref __TypeHandle.__Game_Citizens_Leisure_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ResourceBuyerType = InternalCompilerInterface.GetComponentTypeHandle<ResourceBuyer>(ref __TypeHandle.__Game_Companies_ResourceBuyer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_StudentType = InternalCompilerInterface.GetComponentTypeHandle<Game.Citizens.Student>(ref __TypeHandle.__Game_Citizens_Student_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CurrentBuildings = InternalCompilerInterface.GetComponentLookup<CurrentBuilding>(ref __TypeHandle.__Game_Citizens_CurrentBuilding_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HospitalData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.Hospital>(ref __TypeHandle.__Game_Buildings_Hospital_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CurrentTransport = InternalCompilerInterface.GetComponentLookup<CurrentTransport>(ref __TypeHandle.__Game_Citizens_CurrentTransport_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ResidentData = InternalCompilerInterface.GetComponentLookup<Game.Creatures.Resident>(ref __TypeHandle.__Game_Creatures_Resident_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CityModifiers = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Students = InternalCompilerInterface.GetBufferLookup<Game.Buildings.Student>(ref __TypeHandle.__Game_Buildings_Student_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HouseholdCitizens = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_UpdateFrameIndex = updateFrame,
				m_RandomSeed = RandomSeed.Next(),
				m_HealthcareParameterData = ((EntityQuery)(ref m_HealthcareSettingsQuery)).GetSingleton<HealthcareParameterData>(),
				m_City = m_CitySystem.City,
				m_TriggerBuffer = m_TriggerSystem.CreateActionBuffer().AsParallelWriter(),
				m_StatisticsEventQueue = m_CityStatisticsSystem.GetStatisticsEventQueue(out deps).AsParallelWriter(),
				m_IconCommandBuffer = m_IconCommandSystem.CreateCommandBuffer()
			};
			EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
			deathCheckJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
			deathCheckJob.m_PatientsRecoveredCounter = ((NativeCounter)(ref m_AchievementTriggerSystem.m_PatientsTreatedCounter)).ToConcurrent();
			deathCheckJob.m_SimulationFrame = m_SimulationSystem.frameIndex;
			deathCheckJob.m_TimeSettings = ((EntityQuery)(ref m_TimeSettingsQuery)).GetSingleton<TimeSettingsData>();
			deathCheckJob.m_TimeData = ((EntityQuery)(ref m_TimeDataQuery)).GetSingleton<TimeData>();
			DeathCheckJob deathCheckJob2 = deathCheckJob;
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<DeathCheckJob>(deathCheckJob2, m_DeathCheckQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, deps));
			m_IconCommandSystem.AddCommandBufferWriter(((SystemBase)this).Dependency);
			m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
			m_CityStatisticsSystem.AddWriter(((SystemBase)this).Dependency);
			m_TriggerSystem.AddActionBufferWriter(((SystemBase)this).Dependency);
		}
	}

	public static void PerformAfterDeathActions(Entity citizen, Entity household, ParallelWriter<TriggerAction> triggerBuffer, ParallelWriter<StatisticsEvent> statisticsEventQueue, ref BufferLookup<HouseholdCitizen> householdCitizens)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		triggerBuffer.Enqueue(new TriggerAction(TriggerType.CitizenDied, Entity.Null, citizen, Entity.Null));
		DynamicBuffer<HouseholdCitizen> val = default(DynamicBuffer<HouseholdCitizen>);
		if (household != Entity.Null && householdCitizens.TryGetBuffer(household, ref val))
		{
			for (int i = 0; i < val.Length; i++)
			{
				if (val[i].m_Citizen != citizen)
				{
					triggerBuffer.Enqueue(new TriggerAction(TriggerType.CitizensFamilyMemberDied, Entity.Null, val[i].m_Citizen, citizen));
				}
			}
		}
		statisticsEventQueue.Enqueue(new StatisticsEvent
		{
			m_Statistic = StatisticType.DeathRate,
			m_Change = 1f
		});
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
	public DeathCheckSystem()
	{
	}
}
