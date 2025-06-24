using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.Buildings;
using Game.Common;
using Game.Debug;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class CountStudyPositionsSystem : GameSystemBase, IDefaultSerializable, ISerializable
{
	[BurstCompile]
	private struct CountStudyPositionsJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_SchoolChunks;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		[ReadOnly]
		public BufferTypeHandle<Student> m_StudentType;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> m_UpgradeType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabDatas;

		[ReadOnly]
		public ComponentLookup<SchoolData> m_SchoolDatas;

		[ReadOnly]
		public ComponentLookup<OutsideConnectionData> m_OutsideConnectionDatas;

		public NativeArray<int> m_StudyPositionByEducation;

		public void Execute()
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < 5; i++)
			{
				m_StudyPositionByEducation[i] = 0;
			}
			for (int j = 0; j < m_SchoolChunks.Length; j++)
			{
				ArchetypeChunk val = m_SchoolChunks[j];
				BufferAccessor<Student> bufferAccessor = ((ArchetypeChunk)(ref val)).GetBufferAccessor<Student>(ref m_StudentType);
				NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabRef>(ref m_PrefabType);
				BufferAccessor<InstalledUpgrade> bufferAccessor2 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<InstalledUpgrade>(ref m_UpgradeType);
				bool flag = bufferAccessor2.Length != 0;
				for (int k = 0; k < bufferAccessor.Length; k++)
				{
					Entity prefab = nativeArray[k].m_Prefab;
					if (!m_OutsideConnectionDatas.HasComponent(prefab) && m_SchoolDatas.HasComponent(prefab))
					{
						SchoolData data = m_SchoolDatas[prefab];
						if (flag)
						{
							UpgradeUtils.CombineStats<SchoolData>(ref data, bufferAccessor2[k], ref m_PrefabDatas, ref m_SchoolDatas);
						}
						DynamicBuffer<Student> val2 = bufferAccessor[k];
						ref NativeArray<int> reference = ref m_StudyPositionByEducation;
						int educationLevel = m_SchoolDatas[prefab].m_EducationLevel;
						reference[educationLevel] += math.max(0, data.m_StudentCapacity / 2 - val2.Length);
					}
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Student> __Game_Buildings_Student_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<OutsideConnectionData> __Game_Prefabs_OutsideConnectionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SchoolData> __Game_Prefabs_SchoolData_RO_ComponentLookup;

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
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<InstalledUpgrade>(true);
			__Game_Buildings_Student_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Student>(true);
			__Game_Prefabs_OutsideConnectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<OutsideConnectionData>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_SchoolData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SchoolData>(true);
		}
	}

	private EntityQuery m_SchoolQuery;

	[DebugWatchValue]
	private NativeArray<int> m_StudyPositionByEducation;

	private JobHandle m_WriteDependencies;

	private JobHandle m_ReadDependencies;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	public NativeArray<int> GetStudyPositionsByEducation(out JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		deps = m_WriteDependencies;
		return m_StudyPositionByEducation;
	}

	public void AddReader(JobHandle reader)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_ReadDependencies = JobHandle.CombineDependencies(m_ReadDependencies, reader);
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SchoolQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[8]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Game.Buildings.School>(),
			ComponentType.ReadOnly<Student>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Game.Objects.OutsideConnection>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Destroyed>(),
			ComponentType.Exclude<Deleted>()
		});
		m_StudyPositionByEducation = new NativeArray<int>(5, (Allocator)4, (NativeArrayOptions)1);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_StudyPositionByEducation.Dispose();
		base.OnDestroy();
	}

	public void SetDefaults(Context context)
	{
		for (int i = 0; i < 5; i++)
		{
			m_StudyPositionByEducation[i] = 0;
		}
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_StudyPositionByEducation);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.economyFix)
		{
			int num = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
			return;
		}
		NativeArray<int> val = m_StudyPositionByEducation;
		((IReader)reader/*cast due to .constrained prefix*/).Read(val);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		CountStudyPositionsJob countStudyPositionsJob = new CountStudyPositionsJob
		{
			m_SchoolChunks = ((EntityQuery)(ref m_SchoolQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val),
			m_PrefabType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpgradeType = InternalCompilerInterface.GetBufferTypeHandle<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_StudentType = InternalCompilerInterface.GetBufferTypeHandle<Student>(ref __TypeHandle.__Game_Buildings_Student_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OutsideConnectionDatas = InternalCompilerInterface.GetComponentLookup<OutsideConnectionData>(ref __TypeHandle.__Game_Prefabs_OutsideConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabDatas = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SchoolDatas = InternalCompilerInterface.GetComponentLookup<SchoolData>(ref __TypeHandle.__Game_Prefabs_SchoolData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StudyPositionByEducation = m_StudyPositionByEducation
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<CountStudyPositionsJob>(countStudyPositionsJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, m_ReadDependencies, val));
		m_WriteDependencies = ((SystemBase)this).Dependency;
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
	public CountStudyPositionsSystem()
	{
	}
}
