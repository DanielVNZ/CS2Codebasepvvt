using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class SchoolAISystem : GameSystemBase
{
	[BurstCompile]
	private struct SchoolTickJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> m_InstalledUpgradeType;

		[ReadOnly]
		public BufferTypeHandle<Efficiency> m_EfficiencyType;

		public ComponentTypeHandle<Game.Buildings.School> m_SchoolType;

		public ComponentTypeHandle<ServiceUsage> m_ServiceUsageType;

		public BufferTypeHandle<Game.Buildings.Student> m_StudentType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<SchoolData> m_SchoolDatas;

		[ReadOnly]
		public ComponentLookup<Game.Citizens.Student> m_Students;

		[ReadOnly]
		public ComponentLookup<Citizen> m_Citizens;

		[ReadOnly]
		public ComponentLookup<TravelPurpose> m_TravelPurposes;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifiers;

		[ReadOnly]
		public BufferLookup<ServiceFee> m_Fees;

		public ParallelWriter m_CommandBuffer;

		public EconomyParameterData m_EconomyParameters;

		public EducationParameterData m_EducationParameters;

		public TimeData m_TimeData;

		public RandomSeed m_RandomSeed;

		public Entity m_City;

		public uint m_SimulationFrame;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<CityModifier> modifiers = m_CityModifiers[m_City];
			NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<InstalledUpgrade> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<InstalledUpgrade>(ref m_InstalledUpgradeType);
			BufferAccessor<Efficiency> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Efficiency>(ref m_EfficiencyType);
			NativeArray<Game.Buildings.School> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.School>(ref m_SchoolType);
			BufferAccessor<Game.Buildings.Student> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Game.Buildings.Student>(ref m_StudentType);
			NativeArray<ServiceUsage> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ServiceUsage>(ref m_ServiceUsageType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			SchoolData data = default(SchoolData);
			Game.Citizens.Student student = default(Game.Citizens.Student);
			Citizen citizen = default(Citizen);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity prefab = nativeArray[i].m_Prefab;
				ref Game.Buildings.School reference = ref CollectionUtils.ElementAt<Game.Buildings.School>(nativeArray2, i);
				float efficiency = BuildingUtils.GetEfficiency(bufferAccessor2, i);
				m_SchoolDatas.TryGetComponent(prefab, ref data);
				if (bufferAccessor.Length != 0)
				{
					UpgradeUtils.CombineStats<SchoolData>(ref data, bufferAccessor[i], ref m_Prefabs, ref m_SchoolDatas);
				}
				float fee = ServiceFeeSystem.GetFee(ServiceFeeSystem.GetEducationResource(data.m_EducationLevel), m_Fees[m_City]);
				DynamicBuffer<Game.Buildings.Student> val = bufferAccessor3[i];
				float num = 0f;
				float num2 = 0f;
				int num3 = 0;
				for (int num4 = val.Length - 1; num4 >= 0; num4--)
				{
					if (m_Students.TryGetComponent((Entity)val[num4], ref student) && m_Citizens.TryGetComponent((Entity)val[num4], ref citizen))
					{
						if (efficiency <= 0.001f && ((Random)(ref random)).NextFloat() < m_EducationParameters.m_InoperableSchoolLeaveProbability)
						{
							LeaveSchool(unfilteredChunkIndex, val[num4]);
							val.RemoveAt(num4);
						}
						else
						{
							int failedEducationCount = citizen.GetFailedEducationCount();
							float ageInDays = citizen.GetAgeInDays(m_SimulationFrame, m_TimeData);
							float graduationProbability = GraduationSystem.GetGraduationProbability(student.m_Level, citizen.m_WellBeing, data, modifiers, 0.5f, efficiency);
							if (graduationProbability > 0.001f)
							{
								num += math.min(4f, (float)failedEducationCount + 0.5f - 1f / math.log2(1f - math.saturate(graduationProbability))) / 1f;
								float num5 = math.pow(1f - math.saturate(graduationProbability), 4f);
								float num6 = math.saturate(GraduationSystem.GetDropoutProbability(student.m_Level, student.m_LastCommuteTime, fee, 0, ageInDays, 0.5f, failedEducationCount, graduationProbability, ref m_EconomyParameters));
								num2 += math.saturate(num5 + num6);
							}
							else
							{
								num += 4f;
								num2 += 1f;
							}
							num3++;
						}
					}
					else
					{
						val.RemoveAt(num4);
					}
				}
				if (num3 > 0)
				{
					reference.m_AverageGraduationTime = num / (float)num3;
					reference.m_AverageFailProbability = num2 / (float)num3;
				}
				else
				{
					float graduationProbability2 = GraduationSystem.GetGraduationProbability(data.m_EducationLevel, 50, data, modifiers, 0.5f, efficiency);
					reference.m_AverageGraduationTime = 0.5f - 1f / math.log2(1f - math.saturate(graduationProbability2));
					reference.m_AverageFailProbability = 0f;
				}
				reference.m_StudentWellbeing = (sbyte)math.clamp((int)math.round(efficiency * (float)data.m_StudentWellbeing), -100, 100);
				reference.m_StudentHealth = (sbyte)math.clamp((int)math.round(efficiency * (float)data.m_StudentHealth), -100, 100);
				if (nativeArray3.Length != 0)
				{
					nativeArray3[i] = new ServiceUsage
					{
						m_Usage = 1f * (float)num3 / (float)data.m_StudentCapacity
					};
				}
			}
		}

		private void LeaveSchool(int chunkIndex, Entity entity)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Game.Citizens.Student>(chunkIndex, entity);
			TravelPurpose travelPurpose = default(TravelPurpose);
			if (m_TravelPurposes.TryGetComponent(entity, ref travelPurpose))
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
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Efficiency> __Game_Buildings_Efficiency_RO_BufferTypeHandle;

		public ComponentTypeHandle<Game.Buildings.School> __Game_Buildings_School_RW_ComponentTypeHandle;

		public ComponentTypeHandle<ServiceUsage> __Game_Buildings_ServiceUsage_RW_ComponentTypeHandle;

		public BufferTypeHandle<Game.Buildings.Student> __Game_Buildings_Student_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SchoolData> __Game_Prefabs_SchoolData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Citizens.Student> __Game_Citizens_Student_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TravelPurpose> __Game_Citizens_TravelPurpose_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<CityModifier> __Game_City_CityModifier_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ServiceFee> __Game_City_ServiceFee_RO_BufferLookup;

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
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<InstalledUpgrade>(true);
			__Game_Buildings_Efficiency_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Efficiency>(true);
			__Game_Buildings_School_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.School>(false);
			__Game_Buildings_ServiceUsage_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ServiceUsage>(false);
			__Game_Buildings_Student_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Game.Buildings.Student>(false);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_SchoolData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SchoolData>(true);
			__Game_Citizens_Student_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Citizens.Student>(true);
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
			__Game_Citizens_TravelPurpose_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TravelPurpose>(true);
			__Game_City_CityModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(true);
			__Game_City_ServiceFee_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceFee>(true);
		}
	}

	private SimulationSystem m_SimulationSystem;

	private CitySystem m_CitySystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_SchoolQuery;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_1235104412_0;

	private EntityQuery __query_1235104412_1;

	private EntityQuery __query_1235104412_2;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 256;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 96;
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
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_SchoolQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadWrite<Game.Buildings.School>(),
			ComponentType.ReadWrite<Game.Buildings.Student>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_SchoolQuery);
		((ComponentSystemBase)this).RequireForUpdate<EconomyParameterData>();
		((ComponentSystemBase)this).RequireForUpdate<EducationParameterData>();
		((ComponentSystemBase)this).RequireForUpdate<TimeData>();
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
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		SchoolTickJob schoolTickJob = new SchoolTickJob
		{
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgradeType = InternalCompilerInterface.GetBufferTypeHandle<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EfficiencyType = InternalCompilerInterface.GetBufferTypeHandle<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SchoolType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.School>(ref __TypeHandle.__Game_Buildings_School_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceUsageType = InternalCompilerInterface.GetComponentTypeHandle<ServiceUsage>(ref __TypeHandle.__Game_Buildings_ServiceUsage_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_StudentType = InternalCompilerInterface.GetBufferTypeHandle<Game.Buildings.Student>(ref __TypeHandle.__Game_Buildings_Student_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SchoolDatas = InternalCompilerInterface.GetComponentLookup<SchoolData>(ref __TypeHandle.__Game_Prefabs_SchoolData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Students = InternalCompilerInterface.GetComponentLookup<Game.Citizens.Student>(ref __TypeHandle.__Game_Citizens_Student_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Citizens = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TravelPurposes = InternalCompilerInterface.GetComponentLookup<TravelPurpose>(ref __TypeHandle.__Game_Citizens_TravelPurpose_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CityModifiers = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Fees = InternalCompilerInterface.GetBufferLookup<ServiceFee>(ref __TypeHandle.__Game_City_ServiceFee_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		schoolTickJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		schoolTickJob.m_EconomyParameters = ((EntityQuery)(ref __query_1235104412_0)).GetSingleton<EconomyParameterData>();
		schoolTickJob.m_EducationParameters = ((EntityQuery)(ref __query_1235104412_1)).GetSingleton<EducationParameterData>();
		schoolTickJob.m_TimeData = ((EntityQuery)(ref __query_1235104412_2)).GetSingleton<TimeData>();
		schoolTickJob.m_RandomSeed = RandomSeed.Next();
		schoolTickJob.m_City = m_CitySystem.City;
		schoolTickJob.m_SimulationFrame = m_SimulationSystem.frameIndex;
		SchoolTickJob schoolTickJob2 = schoolTickJob;
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<SchoolTickJob>(schoolTickJob2, m_SchoolQuery, ((SystemBase)this).Dependency);
		m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
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
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<EconomyParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_1235104412_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		val2 = ((EntityQueryBuilder)(ref val)).WithAll<EducationParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_1235104412_1 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		val2 = ((EntityQueryBuilder)(ref val)).WithAll<TimeData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_1235104412_2 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
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
	public SchoolAISystem()
	{
	}
}
