using System.Runtime.CompilerServices;
using Game.Agents;
using Game.Common;
using Game.Prefabs;
using Game.Simulation;
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

namespace Game.Citizens;

[CompilerGenerated]
public class CitizenInitializeSystem : GameSystemBase
{
	[BurstCompile]
	private struct InitializeCitizenJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public NativeList<Entity> m_CitizenPrefabs;

		[ReadOnly]
		public ComponentTypeHandle<HouseholdMember> m_HouseholdMemberType;

		public ComponentLookup<Arrived> m_Arriveds;

		public ComponentLookup<CarKeeper> m_CarKeepers;

		public ComponentLookup<HasJobSeeker> m_HasJobSeekers;

		public ComponentLookup<PropertySeeker> m_PropertySeekers;

		public ComponentLookup<MailSender> m_MailSenders;

		[ReadOnly]
		public ComponentLookup<CitizenData> m_CitizenDatas;

		public BufferLookup<HouseholdCitizen> m_HouseholdCitizens;

		public ComponentLookup<CrimeVictim> m_CrimeVictims;

		public ComponentLookup<Citizen> m_Citizens;

		public ParallelWriter<TriggerAction> m_TriggerBuffer;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public uint m_SimulationFrame;

		[ReadOnly]
		public TimeData m_TimeData;

		[ReadOnly]
		public DemandParameterData m_DemandParameters;

		[ReadOnly]
		public TimeSettingsData m_TimeSettings;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0418: Unknown result type (might be due to invalid IL or missing references)
			//IL_0468: Unknown result type (might be due to invalid IL or missing references)
			//IL_0493: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<HouseholdMember> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HouseholdMember>(ref m_HouseholdMemberType);
			int daysPerYear = m_TimeSettings.m_DaysPerYear;
			Random random = m_RandomSeed.GetRandom(0);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				m_Arriveds.SetComponentEnabled(val, false);
				m_MailSenders.SetComponentEnabled(val, false);
				m_CrimeVictims.SetComponentEnabled(val, false);
				m_CarKeepers.SetComponentEnabled(val, false);
				m_HasJobSeekers.SetComponentEnabled(val, false);
				Citizen citizen = m_Citizens[val];
				Entity household = nativeArray2[i].m_Household;
				bool flag = (citizen.m_State & CitizenFlags.Commuter) != 0;
				bool num = (citizen.m_State & CitizenFlags.Tourist) != 0;
				citizen.m_PseudoRandom = (ushort)(((Random)(ref random)).NextUInt() % 65536);
				citizen.m_Health = (byte)(40 + ((Random)(ref random)).NextInt(20));
				citizen.m_WellBeing = (byte)(40 + ((Random)(ref random)).NextInt(20));
				if (num)
				{
					citizen.m_LeisureCounter = (byte)((Random)(ref random)).NextInt(128);
				}
				else
				{
					citizen.m_LeisureCounter = (byte)(((Random)(ref random)).NextInt(92) + 128);
				}
				if (((Random)(ref random)).NextBool())
				{
					citizen.m_State |= CitizenFlags.Male;
				}
				Entity citizenPrefabFromCitizen = CitizenUtils.GetCitizenPrefabFromCitizen(m_CitizenPrefabs, citizen, m_CitizenDatas, random);
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PrefabRef>(unfilteredChunkIndex, val, new PrefabRef
				{
					m_Prefab = citizenPrefabFromCitizen
				});
				DynamicBuffer<HouseholdCitizen> val2 = m_HouseholdCitizens[household];
				val2.Add(new HouseholdCitizen
				{
					m_Citizen = val
				});
				int num2 = 0;
				int2 zero = int2.zero;
				if (citizen.m_BirthDay == 0)
				{
					citizen.SetAge(CitizenAge.Child);
					Entity val3 = Entity.Null;
					Entity val4 = Entity.Null;
					for (int j = 0; j < val2.Length; j++)
					{
						Entity citizen2 = val2[j].m_Citizen;
						if (m_Citizens.HasComponent(citizen2) && m_Citizens[citizen2].GetAge() == CitizenAge.Adult)
						{
							if (val3 == Entity.Null)
							{
								val3 = citizen2;
							}
							else
							{
								val4 = citizen2;
							}
						}
					}
					if (val3 != Entity.Null)
					{
						if (val4 != Entity.Null)
						{
							m_TriggerBuffer.Enqueue(new TriggerAction(TriggerType.CitizenCoupleMadeBaby, Entity.Null, val3, val));
						}
						else
						{
							m_TriggerBuffer.Enqueue(new TriggerAction(TriggerType.CitizenSingleMadeBaby, Entity.Null, val3, val));
						}
					}
				}
				else if (citizen.m_BirthDay == 1)
				{
					num2 = ((Random)(ref random)).NextInt(AgingSystem.GetAdultAgeLimitInDays(), AgingSystem.GetElderAgeLimitInDays());
					citizen.SetAge(CitizenAge.Adult);
					zero.x = 0;
					zero.y = (flag ? 4 : 3);
				}
				else if (citizen.m_BirthDay == 2)
				{
					if (((Random)(ref random)).NextFloat(1f) > m_DemandParameters.m_TeenSpawnPercentage)
					{
						num2 = ((Random)(ref random)).NextInt(AgingSystem.GetTeenAgeLimitInDays());
						citizen.SetAge(CitizenAge.Child);
					}
					else
					{
						num2 = ((Random)(ref random)).NextInt(AgingSystem.GetTeenAgeLimitInDays(), AgingSystem.GetAdultAgeLimitInDays());
						citizen.SetAge(CitizenAge.Teen);
						((int2)(ref zero))._002Ector(0, 1);
					}
				}
				else if (citizen.m_BirthDay == 3)
				{
					num2 = AgingSystem.GetElderAgeLimitInDays() + ((Random)(ref random)).NextInt(5);
					citizen.SetAge(CitizenAge.Elderly);
					((int2)(ref zero))._002Ector(0, 4);
				}
				else
				{
					num2 = AgingSystem.GetAdultAgeLimitInDays() + ((Random)(ref random)).NextInt(daysPerYear);
					citizen.SetAge(CitizenAge.Adult);
					((int2)(ref zero))._002Ector(2, 3);
				}
				float num3 = 0f;
				float num4 = 1f;
				for (int k = 0; k <= 3; k++)
				{
					if (k >= zero.x && k <= zero.y)
					{
						num3 += ((float4)(ref m_DemandParameters.m_NewCitizenEducationParameters))[k];
					}
					num4 -= ((float4)(ref m_DemandParameters.m_NewCitizenEducationParameters))[k];
				}
				if (zero.y == 4)
				{
					num3 += num4;
				}
				float num5 = ((Random)(ref random)).NextFloat(num3);
				for (int l = zero.x; l <= zero.y; l++)
				{
					if (l == 4 || num5 < ((float4)(ref m_DemandParameters.m_NewCitizenEducationParameters))[l])
					{
						citizen.SetEducationLevel(l);
						break;
					}
					num5 -= ((float4)(ref m_DemandParameters.m_NewCitizenEducationParameters))[l];
				}
				citizen.m_BirthDay = (short)(TimeSystem.GetDay(m_SimulationFrame, m_TimeData) - num2);
				m_Citizens[val] = citizen;
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

		public ComponentTypeHandle<HouseholdMember> __Game_Citizens_HouseholdMember_RW_ComponentTypeHandle;

		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RW_ComponentLookup;

		public BufferLookup<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RW_BufferLookup;

		[ReadOnly]
		public ComponentLookup<CitizenData> __Game_Prefabs_CitizenData_RO_ComponentLookup;

		public ComponentLookup<Arrived> __Game_Citizens_Arrived_RW_ComponentLookup;

		public ComponentLookup<CarKeeper> __Game_Citizens_CarKeeper_RW_ComponentLookup;

		public ComponentLookup<HasJobSeeker> __Game_Agents_HasJobSeeker_RW_ComponentLookup;

		public ComponentLookup<PropertySeeker> __Game_Agents_PropertySeeker_RW_ComponentLookup;

		public ComponentLookup<MailSender> __Game_Citizens_MailSender_RW_ComponentLookup;

		public ComponentLookup<CrimeVictim> __Game_Citizens_CrimeVictim_RW_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Citizens_HouseholdMember_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HouseholdMember>(false);
			__Game_Citizens_Citizen_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(false);
			__Game_Citizens_HouseholdCitizen_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdCitizen>(false);
			__Game_Prefabs_CitizenData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CitizenData>(true);
			__Game_Citizens_Arrived_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Arrived>(false);
			__Game_Citizens_CarKeeper_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarKeeper>(false);
			__Game_Agents_HasJobSeeker_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HasJobSeeker>(false);
			__Game_Agents_PropertySeeker_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertySeeker>(false);
			__Game_Citizens_MailSender_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MailSender>(false);
			__Game_Citizens_CrimeVictim_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CrimeVictim>(false);
		}
	}

	private EntityQuery m_NewCitizenQuery;

	private EntityQuery m_TimeSettingQuery;

	private EntityQuery m_CitizenPrefabQuery;

	private EntityQuery m_TimeDataQuery;

	private EntityQuery m_DemandParameterQuery;

	private SimulationSystem m_SimulationSystem;

	private TriggerSystem m_TriggerSystem;

	private ModificationBarrier5 m_EndFrameBarrier;

	private TypeHandle __TypeHandle;

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
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_TriggerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TriggerSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier5>();
		m_NewCitizenQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadWrite<Citizen>(),
			ComponentType.ReadWrite<HouseholdMember>(),
			ComponentType.ReadOnly<Created>(),
			ComponentType.Exclude<Temp>()
		});
		m_CitizenPrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<CitizenData>() });
		m_TimeSettingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TimeSettingsData>() });
		m_TimeDataQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TimeData>() });
		m_DemandParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<DemandParameterData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_NewCitizenQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_TimeDataQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_TimeSettingQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_DemandParameterQuery);
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
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		InitializeCitizenJob initializeCitizenJob = new InitializeCitizenJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdMemberType = InternalCompilerInterface.GetComponentTypeHandle<HouseholdMember>(ref __TypeHandle.__Game_Citizens_HouseholdMember_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CitizenPrefabs = ((EntityQuery)(ref m_CitizenPrefabQuery)).ToEntityListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val),
			m_Citizens = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdCitizens = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CitizenDatas = InternalCompilerInterface.GetComponentLookup<CitizenData>(ref __TypeHandle.__Game_Prefabs_CitizenData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Arriveds = InternalCompilerInterface.GetComponentLookup<Arrived>(ref __TypeHandle.__Game_Citizens_Arrived_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarKeepers = InternalCompilerInterface.GetComponentLookup<CarKeeper>(ref __TypeHandle.__Game_Citizens_CarKeeper_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HasJobSeekers = InternalCompilerInterface.GetComponentLookup<HasJobSeeker>(ref __TypeHandle.__Game_Agents_HasJobSeeker_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PropertySeekers = InternalCompilerInterface.GetComponentLookup<PropertySeeker>(ref __TypeHandle.__Game_Agents_PropertySeeker_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MailSenders = InternalCompilerInterface.GetComponentLookup<MailSender>(ref __TypeHandle.__Game_Citizens_MailSender_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CrimeVictims = InternalCompilerInterface.GetComponentLookup<CrimeVictim>(ref __TypeHandle.__Game_Citizens_CrimeVictim_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DemandParameters = ((EntityQuery)(ref m_DemandParameterQuery)).GetSingleton<DemandParameterData>(),
			m_TimeSettings = ((EntityQuery)(ref m_TimeSettingQuery)).GetSingleton<TimeSettingsData>(),
			m_TimeData = ((EntityQuery)(ref m_TimeDataQuery)).GetSingleton<TimeData>(),
			m_SimulationFrame = m_SimulationSystem.frameIndex,
			m_RandomSeed = RandomSeed.Next()
		};
		EntityCommandBuffer val2 = m_EndFrameBarrier.CreateCommandBuffer();
		initializeCitizenJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
		initializeCitizenJob.m_TriggerBuffer = m_TriggerSystem.CreateActionBuffer().AsParallelWriter();
		InitializeCitizenJob initializeCitizenJob2 = initializeCitizenJob;
		((SystemBase)this).Dependency = JobChunkExtensions.Schedule<InitializeCitizenJob>(initializeCitizenJob2, m_NewCitizenQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val));
		((EntityCommandBufferSystem)m_EndFrameBarrier).AddJobHandleForProducer(((SystemBase)this).Dependency);
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
	public CitizenInitializeSystem()
	{
	}
}
