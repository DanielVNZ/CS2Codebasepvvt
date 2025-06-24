using System.Runtime.CompilerServices;
using Game.Buildings;
using Game.Citizens;
using Game.Common;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Tutorials;

[CompilerGenerated]
public class TutorialHealthProblemActivationSystem : GameSystemBase
{
	[BurstCompile]
	private struct CheckProblemsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<HealthProblemActivationData> m_ActivationType;

		[ReadOnly]
		public ComponentTypeHandle<HealthProblem> m_HealthProblemType;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_HealthProblemChunks;

		public bool m_NoHospital;

		public bool m_NoCemetery;

		public ParallelWriter m_Writer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<HealthProblemActivationData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HealthProblemActivationData>(ref m_ActivationType);
			NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityTypeHandle);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				if (Execute(nativeArray[i]))
				{
					((ParallelWriter)(ref m_Writer)).AddComponent<TutorialActivated>(unfilteredChunkIndex, nativeArray2[i]);
				}
			}
		}

		private bool Execute(HealthProblemActivationData data)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			if ((data.m_Require & HealthProblemFlags.Dead) != HealthProblemFlags.None)
			{
				if (!m_NoCemetery)
				{
					return false;
				}
			}
			else if (!m_NoHospital)
			{
				return false;
			}
			int num = 0;
			for (int i = 0; i < m_HealthProblemChunks.Length; i++)
			{
				ArchetypeChunk val = m_HealthProblemChunks[i];
				NativeArray<HealthProblem> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<HealthProblem>(ref m_HealthProblemType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					if ((nativeArray[j].m_Flags & data.m_Require) != HealthProblemFlags.None)
					{
						num++;
					}
					if (num >= data.m_RequiredCount)
					{
						return true;
					}
				}
			}
			return false;
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
		public ComponentTypeHandle<HealthProblemActivationData> __Game_Tutorials_HealthProblemActivationData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<HealthProblem> __Game_Citizens_HealthProblem_RO_ComponentTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Tutorials_HealthProblemActivationData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HealthProblemActivationData>(true);
			__Game_Citizens_HealthProblem_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HealthProblem>(true);
		}
	}

	protected EntityCommandBufferSystem m_BarrierSystem;

	private EntityQuery m_TutorialQuery;

	private EntityQuery m_HealthProblemQuery;

	private EntityQuery m_MedicalClinicQuery;

	private EntityQuery m_MedicalClinicUnlockedQuery;

	private EntityQuery m_CemeteryQuery;

	private EntityQuery m_CemeteryUnlockedQuery;

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
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_BarrierSystem = (EntityCommandBufferSystem)(object)((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier4>();
		m_TutorialQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<HealthProblemActivationData>(),
			ComponentType.Exclude<TutorialActivated>(),
			ComponentType.Exclude<TutorialCompleted>()
		});
		m_HealthProblemQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Citizen>(),
			ComponentType.ReadOnly<HealthProblem>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_MedicalClinicQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Game.Buildings.Hospital>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_MedicalClinicUnlockedQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<HospitalData>(),
			ComponentType.ReadOnly<BuildingData>(),
			ComponentType.Exclude<Locked>()
		});
		m_CemeteryQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Game.Buildings.DeathcareFacility>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_CemeteryUnlockedQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<DeathcareFacilityData>(),
			ComponentType.ReadOnly<BuildingData>(),
			ComponentType.Exclude<Locked>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_HealthProblemQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_TutorialQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		bool flag = ((EntityQuery)(ref m_MedicalClinicQuery)).IsEmptyIgnoreFilter && !((EntityQuery)(ref m_MedicalClinicUnlockedQuery)).IsEmpty;
		bool flag2 = ((EntityQuery)(ref m_CemeteryQuery)).IsEmptyIgnoreFilter && !((EntityQuery)(ref m_CemeteryUnlockedQuery)).IsEmpty;
		if (flag || flag2)
		{
			JobHandle val = default(JobHandle);
			CheckProblemsJob checkProblemsJob = new CheckProblemsJob
			{
				m_EntityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ActivationType = InternalCompilerInterface.GetComponentTypeHandle<HealthProblemActivationData>(ref __TypeHandle.__Game_Tutorials_HealthProblemActivationData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_HealthProblemType = InternalCompilerInterface.GetComponentTypeHandle<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_HealthProblemChunks = ((EntityQuery)(ref m_HealthProblemQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val),
				m_NoHospital = flag,
				m_NoCemetery = flag2
			};
			EntityCommandBuffer val2 = m_BarrierSystem.CreateCommandBuffer();
			checkProblemsJob.m_Writer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
			CheckProblemsJob checkProblemsJob2 = checkProblemsJob;
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<CheckProblemsJob>(checkProblemsJob2, m_TutorialQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val));
			checkProblemsJob2.m_HealthProblemChunks.Dispose(((SystemBase)this).Dependency);
			m_BarrierSystem.AddJobHandleForProducer(((SystemBase)this).Dependency);
		}
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
	public TutorialHealthProblemActivationSystem()
	{
	}
}
