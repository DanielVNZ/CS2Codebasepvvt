using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Prefabs;
using Game.Triggers;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class GraduationSystem : GameSystemBase
{
	[BurstCompile]
	public struct GraduationJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Citizens.Student> m_StudentType;

		public ComponentTypeHandle<Citizen> m_CitizenType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<SchoolData> m_SchoolDatas;

		[ReadOnly]
		public ComponentLookup<TravelPurpose> m_Purposes;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifiers;

		[ReadOnly]
		public BufferLookup<Efficiency> m_BuildingEfficiencies;

		[ReadOnly]
		public BufferLookup<ServiceFee> m_Fees;

		public ParallelWriter<TriggerAction> m_TriggerBuffer;

		public ParallelWriter m_CommandBuffer;

		public EconomyParameterData m_EconomyParameters;

		public TimeData m_TimeData;

		public RandomSeed m_RandomSeed;

		public uint m_SimulationFrame;

		public Entity m_City;

		public uint m_UpdateFrameIndex;

		public int m_DebugFastGraduationLevel;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			if (m_DebugFastGraduationLevel == 0 && ((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index != m_UpdateFrameIndex)
			{
				return;
			}
			NativeArray<Game.Citizens.Student> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Citizens.Student>(ref m_StudentType);
			NativeArray<Citizen> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Citizen>(ref m_CitizenType);
			NativeArray<Entity> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			DynamicBuffer<CityModifier> modifiers = m_CityModifiers[m_City];
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				if (m_DebugFastGraduationLevel == 0 && ((Random)(ref random)).NextInt(2) != 0)
				{
					continue;
				}
				Entity val = nativeArray3[i];
				Game.Citizens.Student student = nativeArray[i];
				ref Citizen reference = ref CollectionUtils.ElementAt<Citizen>(nativeArray2, i);
				Entity school = student.m_School;
				if (!m_Prefabs.HasComponent(school))
				{
					continue;
				}
				Entity prefab = m_Prefabs[school].m_Prefab;
				if (!m_SchoolDatas.HasComponent(prefab))
				{
					continue;
				}
				int num = student.m_Level;
				if (num == 255)
				{
					num = m_SchoolDatas[prefab].m_EducationLevel;
				}
				SchoolData data = m_SchoolDatas[prefab];
				if (m_InstalledUpgrades.HasBuffer(school))
				{
					UpgradeUtils.CombineStats<SchoolData>(ref data, m_InstalledUpgrades[school], ref m_Prefabs, ref m_SchoolDatas);
				}
				int wellBeing = reference.m_WellBeing;
				Random pseudoRandom = reference.GetPseudoRandom(CitizenPseudoRandom.StudyWillingness);
				float studyWillingness = ((Random)(ref pseudoRandom)).NextFloat();
				float efficiency = BuildingUtils.GetEfficiency(school, ref m_BuildingEfficiencies);
				float graduationProbability = GetGraduationProbability(num, wellBeing, data, modifiers, studyWillingness, efficiency);
				if (m_DebugFastGraduationLevel != 0 && m_DebugFastGraduationLevel != num)
				{
					continue;
				}
				if (m_DebugFastGraduationLevel == num || ((Random)(ref random)).NextFloat() < graduationProbability)
				{
					reference.SetEducationLevel(Mathf.Max(reference.GetEducationLevel(), num));
					if (m_DebugFastGraduationLevel != 0 || num > 1)
					{
						LeaveSchool(unfilteredChunkIndex, val, school);
						m_TriggerBuffer.Enqueue(new TriggerAction(TriggerType.CitizenGraduated, Entity.Null, val, school));
					}
				}
				else
				{
					if (num <= 2)
					{
						continue;
					}
					int failedEducationCount = reference.GetFailedEducationCount();
					if (failedEducationCount < 3)
					{
						reference.SetFailedEducationCount(failedEducationCount + 1);
						float fee = ServiceFeeSystem.GetFee(ServiceFeeSystem.GetEducationResource(student.m_Level), m_Fees[m_City]);
						float dropoutProbability = GetDropoutProbability(reference, student.m_Level, student.m_LastCommuteTime, fee, 0, m_SimulationFrame, ref m_EconomyParameters, data, modifiers, efficiency, m_TimeData);
						dropoutProbability = 1f - math.pow(math.saturate(1f - dropoutProbability), 32f);
						if (((Random)(ref random)).NextFloat() < dropoutProbability)
						{
							LeaveSchool(unfilteredChunkIndex, val, school);
							m_TriggerBuffer.Enqueue(new TriggerAction(TriggerType.CitizenDroppedOutSchool, Entity.Null, val, school));
						}
					}
					else
					{
						LeaveSchool(unfilteredChunkIndex, val, school);
						m_TriggerBuffer.Enqueue(new TriggerAction(TriggerType.CitizenFailedSchool, Entity.Null, val, school));
					}
				}
			}
		}

		private void LeaveSchool(int chunkIndex, Entity entity, Entity school)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<StudentsRemoved>(chunkIndex, school);
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Game.Citizens.Student>(chunkIndex, entity);
			TravelPurpose travelPurpose = default(TravelPurpose);
			if (m_Purposes.TryGetComponent(entity, ref travelPurpose))
			{
				Purpose purpose = travelPurpose.m_Purpose;
				if (purpose == Purpose.GoingToSchool || purpose == Purpose.Studying)
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TravelPurpose>(chunkIndex, entity);
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

		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		public ComponentTypeHandle<Citizen> __Game_Citizens_Citizen_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Citizens.Student> __Game_Citizens_Student_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SchoolData> __Game_Prefabs_SchoolData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<CityModifier> __Game_City_CityModifier_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<TravelPurpose> __Game_Citizens_TravelPurpose_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Efficiency> __Game_Buildings_Efficiency_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ServiceFee> __Game_City_ServiceFee_RO_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Citizens_Citizen_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Citizen>(false);
			__Game_Citizens_Student_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Citizens.Student>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_SchoolData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SchoolData>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<InstalledUpgrade>(true);
			__Game_City_CityModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(true);
			__Game_Citizens_TravelPurpose_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TravelPurpose>(true);
			__Game_Buildings_Efficiency_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Efficiency>(true);
			__Game_City_ServiceFee_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceFee>(true);
		}
	}

	public int debugFastGraduationLevel;

	public const int kUpdatesPerDay = 1;

	public const int kCheckSlowdown = 2;

	private SimulationSystem m_SimulationSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private CitySystem m_CitySystem;

	private TriggerSystem m_TriggerSystem;

	private EntityQuery m_StudentQuery;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_1855827631_0;

	private EntityQuery __query_1855827631_1;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16384;
	}

	public static float GetDropoutProbability(Citizen citizen, int level, float commute, float fee, int wealth, uint simulationFrame, ref EconomyParameterData economyParameters, SchoolData schoolData, DynamicBuffer<CityModifier> modifiers, float efficiency, TimeData timeData)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		float ageInDays = citizen.GetAgeInDays(simulationFrame, timeData);
		Random pseudoRandom = citizen.GetPseudoRandom(CitizenPseudoRandom.StudyWillingness);
		float studyWillingness = ((Random)(ref pseudoRandom)).NextFloat();
		int failedEducationCount = citizen.GetFailedEducationCount();
		float graduationProbability = GetGraduationProbability(level, citizen.m_WellBeing, schoolData, modifiers, studyWillingness, efficiency);
		return GetDropoutProbability(level, commute, fee, wealth, ageInDays, studyWillingness, failedEducationCount, graduationProbability, ref economyParameters);
	}

	public static float GetDropoutProbability(int level, float commute, float fee, int wealth, float age, float studyWillingness, int failedEducationCount, float graduationProbability, ref EconomyParameterData economyParameters)
	{
		int num = 4 - failedEducationCount;
		float num2 = math.pow(1f - graduationProbability, (float)num);
		float num3 = 1f / (graduationProbability * 2f * 1f);
		float num4 = num3 * fee;
		if (level > 2)
		{
			num4 -= num3 * (float)economyParameters.m_UnemploymentBenefit;
		}
		float num5 = math.max(0f, (float)AgingSystem.GetElderAgeLimitInDays() - age);
		float num6 = (float)economyParameters.GetWage(math.min(2, level - 1)) * num5;
		float num7 = math.lerp((float)economyParameters.GetWage(level), (float)economyParameters.GetWage(level - 1), num2) * (num5 - num3) - num4 + (0.5f + studyWillingness) * (float)economyParameters.m_UnemploymentBenefit * num3;
		if (num6 < num7)
		{
			float num8 = (num7 - num6) / num6;
			return math.saturate(-0.1f + (float)level / 4f - 10f * num8 - (float)wealth / (num7 - num6) + commute / 5000f);
		}
		return 1f;
	}

	public static float GetGraduationProbability(int level, int wellbeing, SchoolData schoolData, DynamicBuffer<CityModifier> modifiers, float studyWillingness, float efficiency)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		float2 modifier = CityUtils.GetModifier(modifiers, CityModifierType.CollegeGraduation);
		float2 modifier2 = CityUtils.GetModifier(modifiers, CityModifierType.UniversityGraduation);
		return GetGraduationProbability(level, wellbeing, schoolData.m_GraduationModifier, modifier, modifier2, studyWillingness, efficiency);
	}

	public static float GetGraduationProbability(int level, int wellbeing, float graduationModifier, float2 collegeModifier, float2 uniModifier, float studyWillingness, float efficiency)
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		if (efficiency <= 0.001f)
		{
			return 0f;
		}
		float num = math.saturate((0.5f + studyWillingness) * (float)wellbeing / 75f);
		float num2 = 0f;
		switch (level)
		{
		case 1:
			num2 = math.smoothstep(0f, 1f, 0.6f * num + 0.41f);
			break;
		case 2:
			num2 = 0.6f * math.log(2.6f * num + 1.1f);
			break;
		case 3:
			num2 = 90f * math.log(1.6f * num + 1f);
			num2 += collegeModifier.x;
			num2 += num2 * collegeModifier.y;
			num2 /= 100f;
			break;
		case 4:
			num2 = 70f * num;
			num2 += uniModifier.x;
			num2 += num2 * uniModifier.y;
			num2 /= 100f;
			break;
		default:
			num2 = 0f;
			break;
		}
		num2 = 1f - (1f - num2) / efficiency;
		return num2 + graduationModifier;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_TriggerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TriggerSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_StudentQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Citizens.Student>(),
			ComponentType.ReadWrite<Citizen>(),
			ComponentType.ReadOnly<UpdateFrame>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_StudentQuery);
		((ComponentSystemBase)this).RequireForUpdate<EconomyParameterData>();
		((ComponentSystemBase)this).RequireForUpdate<TimeData>();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		uint updateFrame = SimulationUtils.GetUpdateFrame(m_SimulationSystem.frameIndex, 1, 16);
		GraduationJob graduationJob = new GraduationJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CitizenType = InternalCompilerInterface.GetComponentTypeHandle<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_StudentType = InternalCompilerInterface.GetComponentTypeHandle<Game.Citizens.Student>(ref __TypeHandle.__Game_Citizens_Student_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SchoolDatas = InternalCompilerInterface.GetComponentLookup<SchoolData>(ref __TypeHandle.__Game_Prefabs_SchoolData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgrades = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CityModifiers = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Purposes = InternalCompilerInterface.GetComponentLookup<TravelPurpose>(ref __TypeHandle.__Game_Citizens_TravelPurpose_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingEfficiencies = InternalCompilerInterface.GetBufferLookup<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Fees = InternalCompilerInterface.GetBufferLookup<ServiceFee>(ref __TypeHandle.__Game_City_ServiceFee_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		graduationJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		graduationJob.m_TriggerBuffer = m_TriggerSystem.CreateActionBuffer().AsParallelWriter();
		graduationJob.m_EconomyParameters = ((EntityQuery)(ref __query_1855827631_0)).GetSingleton<EconomyParameterData>();
		graduationJob.m_TimeData = ((EntityQuery)(ref __query_1855827631_1)).GetSingleton<TimeData>();
		graduationJob.m_RandomSeed = RandomSeed.Next();
		graduationJob.m_City = m_CitySystem.City;
		graduationJob.m_SimulationFrame = m_SimulationSystem.frameIndex;
		graduationJob.m_UpdateFrameIndex = updateFrame;
		graduationJob.m_DebugFastGraduationLevel = debugFastGraduationLevel;
		GraduationJob graduationJob2 = graduationJob;
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<GraduationJob>(graduationJob2, m_StudentQuery, ((SystemBase)this).Dependency);
		m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
		m_TriggerSystem.AddActionBufferWriter(((SystemBase)this).Dependency);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<EconomyParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_1855827631_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		val2 = ((EntityQueryBuilder)(ref val)).WithAll<TimeData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_1855827631_1 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public GraduationSystem()
	{
	}
}
