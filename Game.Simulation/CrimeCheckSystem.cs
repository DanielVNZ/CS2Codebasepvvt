using System.Runtime.CompilerServices;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Events;
using Game.Net;
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
public class CrimeCheckSystem : GameSystemBase
{
	[BurstCompile]
	private struct CrimeCheckJob : IJobChunk
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_EventPrefabChunks;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Citizen> m_CitizenType;

		[ReadOnly]
		public ComponentTypeHandle<Criminal> m_CriminalType;

		[ReadOnly]
		public ComponentTypeHandle<HouseholdMember> m_HouseholdMemberType;

		[ReadOnly]
		public ComponentTypeHandle<EventData> m_PrefabEventType;

		[ReadOnly]
		public ComponentTypeHandle<CrimeData> m_CrimeDataType;

		[ReadOnly]
		public ComponentTypeHandle<Locked> m_LockedType;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenterData;

		[ReadOnly]
		public BufferLookup<Game.Net.ServiceCoverage> m_ServiceCoverages;

		[ReadOnly]
		public ComponentLookup<Population> m_Populations;

		public ParallelWriter<TriggerAction> m_TriggerBuffer;

		public ParallelWriter<StatisticsEvent> m_StatisticsEventQueue;

		[ReadOnly]
		public uint m_UpdateFrameIndex;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public PoliceConfigurationData m_PoliceConfigurationData;

		[ReadOnly]
		public Entity m_City;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifiers;

		[ReadOnly]
		public bool m_DebugFullCrimeMode;

		public ParallelWriter m_CommandBuffer;

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
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index != m_UpdateFrameIndex)
			{
				return;
			}
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			DynamicBuffer<CityModifier> cityModifiers = m_CityModifiers[m_City];
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Citizen> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Citizen>(ref m_CitizenType);
			NativeArray<Criminal> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Criminal>(ref m_CriminalType);
			NativeArray<HouseholdMember> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HouseholdMember>(ref m_HouseholdMemberType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity entity = nativeArray[i];
				Citizen citizen = nativeArray2[i];
				CitizenAge age = citizen.GetAge();
				if (age == CitizenAge.Child || age == CitizenAge.Elderly)
				{
					continue;
				}
				bool flag = nativeArray3.Length != 0;
				if (!flag || !(nativeArray3[i].m_Event != Entity.Null))
				{
					Entity val = Entity.Null;
					if (nativeArray4.Length != 0)
					{
						val = nativeArray4[i].m_Household;
					}
					Entity property = Entity.Null;
					if (m_PropertyRenterData.HasComponent(val))
					{
						property = m_PropertyRenterData[val].m_Property;
					}
					TryAddCrime(unfilteredChunkIndex, ref random, entity, citizen, flag, val, property, cityModifiers);
				}
			}
		}

		private void TryAddCrime(int jobIndex, ref Random random, Entity entity, Citizen citizen, bool isCriminal, Entity household, Entity property, DynamicBuffer<CityModifier> cityModifiers)
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			float num;
			if (citizen.m_WellBeing <= 25)
			{
				num = (float)(int)citizen.m_WellBeing / 25f;
			}
			else
			{
				num = (float)(100 - citizen.m_WellBeing) / 75f;
				num *= num;
			}
			for (int i = 0; i < m_EventPrefabChunks.Length; i++)
			{
				ArchetypeChunk val = m_EventPrefabChunks[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				NativeArray<EventData> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<EventData>(ref m_PrefabEventType);
				NativeArray<CrimeData> nativeArray3 = ((ArchetypeChunk)(ref val)).GetNativeArray<CrimeData>(ref m_CrimeDataType);
				EnabledMask enabledMask = ((ArchetypeChunk)(ref val)).GetEnabledMask<Locked>(ref m_LockedType);
				for (int j = 0; j < nativeArray3.Length; j++)
				{
					CrimeData crimeData = nativeArray3[j];
					if (crimeData.m_RandomTargetType != EventTargetType.Citizen)
					{
						continue;
					}
					SafeBitRef enableBit = ((EnabledMask)(ref enabledMask)).EnableBit;
					if (((SafeBitRef)(ref enableBit)).IsValid && ((EnabledMask)(ref enabledMask))[j])
					{
						continue;
					}
					float num2 = 0f;
					num2 = ((!isCriminal) ? math.lerp(crimeData.m_OccurenceProbability.min, crimeData.m_OccurenceProbability.max, num) : math.lerp(crimeData.m_RecurrenceProbability.min, crimeData.m_RecurrenceProbability.max, num));
					CityUtils.ApplyModifier(ref num2, cityModifiers, CityModifierType.CrimeProbability);
					float num3 = math.max((float)m_Populations[m_City].m_Population / m_PoliceConfigurationData.m_CrimePopulationReduction * 100f, 100f);
					if (!m_DebugFullCrimeMode && !(((Random)(ref random)).NextFloat(num3) < num2))
					{
						continue;
					}
					if (isCriminal && property != Entity.Null && m_BuildingData.HasComponent(property))
					{
						Building building = m_BuildingData[property];
						if (m_ServiceCoverages.HasBuffer(building.m_RoadEdge))
						{
							float serviceCoverage = NetUtils.GetServiceCoverage(m_ServiceCoverages[building.m_RoadEdge], CoverageService.Welfare, building.m_CurvePosition);
							if (((Random)(ref random)).NextFloat(num3) < serviceCoverage * m_PoliceConfigurationData.m_WelfareCrimeRecurrenceFactor)
							{
								continue;
							}
						}
					}
					m_StatisticsEventQueue.Enqueue(new StatisticsEvent
					{
						m_Statistic = StatisticType.CrimeCount,
						m_Change = 1f
					});
					CreateCrimeEvent(jobIndex, entity, nativeArray[j], nativeArray2[j]);
					return;
				}
			}
		}

		private void CreateCrimeEvent(int jobIndex, Entity targetEntity, Entity eventPrefab, EventData eventData)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, eventData.m_Archetype);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(jobIndex, val, new PrefabRef(eventPrefab));
			((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<TargetElement>(jobIndex, val).Add(new TargetElement(targetEntity));
			m_TriggerBuffer.Enqueue(new TriggerAction(TriggerType.CitizenCommitedCrime, eventPrefab, targetEntity, Entity.Null));
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<Citizen> __Game_Citizens_Citizen_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Criminal> __Game_Citizens_Criminal_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<HouseholdMember> __Game_Citizens_HouseholdMember_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<EventData> __Game_Prefabs_EventData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CrimeData> __Game_Prefabs_CrimeData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Locked> __Game_Prefabs_Locked_RO_ComponentTypeHandle;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.ServiceCoverage> __Game_Net_ServiceCoverage_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Population> __Game_City_Population_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<CityModifier> __Game_City_CityModifier_RO_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
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
			__Game_Citizens_Citizen_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Citizen>(true);
			__Game_Citizens_Criminal_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Criminal>(true);
			__Game_Citizens_HouseholdMember_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HouseholdMember>(true);
			__Game_Prefabs_EventData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EventData>(true);
			__Game_Prefabs_CrimeData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CrimeData>(true);
			__Game_Prefabs_Locked_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Locked>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
			__Game_Net_ServiceCoverage_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.ServiceCoverage>(true);
			__Game_City_Population_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Population>(true);
			__Game_City_CityModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(true);
		}
	}

	public readonly int kUpdatesPerDay = 1;

	public bool debugFullCrimeMode;

	private EndFrameBarrier m_EndFrameBarrier;

	private SimulationSystem m_SimulationSystem;

	private CitySystem m_CitySystem;

	private CityStatisticsSystem m_CityStatisticsSystem;

	private EntityQuery m_CitizenQuery;

	private EntityQuery m_EventQuery;

	private EntityQuery m_PoliceConfigurationQuery;

	private TriggerSystem m_TriggerSystem;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / (kUpdatesPerDay * 16);
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_TriggerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TriggerSystem>();
		m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Citizen>(),
			ComponentType.ReadOnly<UpdateFrame>()
		};
		val.None = (ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<HealthProblem>(),
			ComponentType.ReadOnly<Worker>(),
			ComponentType.ReadOnly<Game.Citizens.Student>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_CitizenQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_EventQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<CrimeData>(),
			ComponentType.Exclude<Locked>()
		});
		m_PoliceConfigurationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PoliceConfigurationData>(),
			ComponentType.Exclude<Locked>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_CitizenQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_EventQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_PoliceConfigurationQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		uint updateFrame = SimulationUtils.GetUpdateFrame(m_SimulationSystem.frameIndex, kUpdatesPerDay, 16);
		JobHandle val = default(JobHandle);
		JobHandle deps;
		CrimeCheckJob crimeCheckJob = new CrimeCheckJob
		{
			m_EventPrefabChunks = ((EntityQuery)(ref m_EventQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val),
			m_CitizenType = InternalCompilerInterface.GetComponentTypeHandle<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CriminalType = InternalCompilerInterface.GetComponentTypeHandle<Criminal>(ref __TypeHandle.__Game_Citizens_Criminal_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdMemberType = InternalCompilerInterface.GetComponentTypeHandle<HouseholdMember>(ref __TypeHandle.__Game_Citizens_HouseholdMember_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabEventType = InternalCompilerInterface.GetComponentTypeHandle<EventData>(ref __TypeHandle.__Game_Prefabs_EventData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CrimeDataType = InternalCompilerInterface.GetComponentTypeHandle<CrimeData>(ref __TypeHandle.__Game_Prefabs_CrimeData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LockedType = InternalCompilerInterface.GetComponentTypeHandle<Locked>(ref __TypeHandle.__Game_Prefabs_Locked_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyRenterData = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceCoverages = InternalCompilerInterface.GetBufferLookup<Game.Net.ServiceCoverage>(ref __TypeHandle.__Game_Net_ServiceCoverage_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Populations = InternalCompilerInterface.GetComponentLookup<Population>(ref __TypeHandle.__Game_City_Population_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TriggerBuffer = m_TriggerSystem.CreateActionBuffer().AsParallelWriter(),
			m_StatisticsEventQueue = m_CityStatisticsSystem.GetStatisticsEventQueue(out deps).AsParallelWriter(),
			m_UpdateFrameIndex = updateFrame,
			m_RandomSeed = RandomSeed.Next(),
			m_PoliceConfigurationData = ((EntityQuery)(ref m_PoliceConfigurationQuery)).GetSingleton<PoliceConfigurationData>(),
			m_City = m_CitySystem.City,
			m_CityModifiers = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DebugFullCrimeMode = debugFullCrimeMode
		};
		EntityCommandBuffer val2 = m_EndFrameBarrier.CreateCommandBuffer();
		crimeCheckJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
		JobHandle val3 = JobChunkExtensions.ScheduleParallel<CrimeCheckJob>(crimeCheckJob, m_CitizenQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val, deps));
		m_EndFrameBarrier.AddJobHandleForProducer(val3);
		m_TriggerSystem.AddActionBufferWriter(val3);
		m_CityStatisticsSystem.AddWriter(val3);
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
	public CrimeCheckSystem()
	{
	}
}
