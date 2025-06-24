using System.Runtime.CompilerServices;
using Game.Buildings;
using Game.City;
using Game.Companies;
using Game.Economy;
using Game.Prefabs;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class CompanyStatisticsSystem : GameSystemBase
{
	[BurstCompile]
	private struct ProcessCompanyStatisticsJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<WorkProvider> m_WorkProviderType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		[ReadOnly]
		public BufferTypeHandle<Employee> m_EmployeeType;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		[ReadOnly]
		public ComponentTypeHandle<CommercialCompany> m_CommercialCompanyType;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> m_IndustrialProcessDatas;

		public uint m_UpdateFrameIndex;

		public ParallelWriter<StatisticsEvent> m_StatisticsEventQueue;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index != m_UpdateFrameIndex)
			{
				return;
			}
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<CommercialCompany>(ref m_CommercialCompanyType);
			NativeArray<WorkProvider> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WorkProvider>(ref m_WorkProviderType);
			BufferAccessor<Employee> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Employee>(ref m_EmployeeType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabType);
			NativeArray<int> val = default(NativeArray<int>);
			val._002Ector(EconomyUtils.ResourceCount, (Allocator)2, (NativeArrayOptions)1);
			NativeArray<int> val2 = default(NativeArray<int>);
			val2._002Ector(EconomyUtils.ResourceCount, (Allocator)2, (NativeArrayOptions)1);
			NativeArray<int> val3 = default(NativeArray<int>);
			val3._002Ector(EconomyUtils.ResourceCount, (Allocator)2, (NativeArrayOptions)1);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity prefab = nativeArray2[i].m_Prefab;
				WorkProvider workProvider = nativeArray[i];
				DynamicBuffer<Employee> val4 = bufferAccessor[i];
				int resourceIndex = EconomyUtils.GetResourceIndex(m_IndustrialProcessDatas[prefab].m_Output.m_Resource);
				int num = resourceIndex;
				int num2 = val3[num];
				val3[num] = num2 + 1;
				ref NativeArray<int> reference = ref val2;
				num2 = resourceIndex;
				reference[num2] += workProvider.m_MaxWorkers;
				reference = ref val;
				num2 = resourceIndex;
				reference[num2] += val4.Length;
			}
			ResourceIterator iterator = ResourceIterator.GetIterator();
			while (iterator.Next())
			{
				int resourceIndex2 = EconomyUtils.GetResourceIndex(iterator.resource);
				if (val3[resourceIndex2] > 0)
				{
					bool flag2 = EconomyUtils.IsOfficeResource(iterator.resource);
					m_StatisticsEventQueue.Enqueue(new StatisticsEvent
					{
						m_Statistic = (flag ? StatisticType.ServiceCount : (flag2 ? StatisticType.OfficeCount : StatisticType.ProcessingCount)),
						m_Change = val3[resourceIndex2],
						m_Parameter = resourceIndex2
					});
					m_StatisticsEventQueue.Enqueue(new StatisticsEvent
					{
						m_Statistic = (flag ? StatisticType.ServiceWorkers : (flag2 ? StatisticType.OfficeWorkers : StatisticType.ProcessingWorkers)),
						m_Change = val[resourceIndex2],
						m_Parameter = resourceIndex2
					});
					m_StatisticsEventQueue.Enqueue(new StatisticsEvent
					{
						m_Statistic = (flag ? StatisticType.ServiceMaxWorkers : (flag2 ? StatisticType.OfficeMaxWorkers : StatisticType.ProcessingMaxWorkers)),
						m_Change = val2[resourceIndex2],
						m_Parameter = resourceIndex2
					});
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
		public ComponentTypeHandle<WorkProvider> __Game_Companies_WorkProvider_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Employee> __Game_Companies_Employee_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CommercialCompany> __Game_Companies_CommercialCompany_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> __Game_Prefabs_IndustrialProcessData_RO_ComponentLookup;

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
			__Game_Companies_WorkProvider_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WorkProvider>(true);
			__Game_Companies_Employee_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Employee>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Companies_CommercialCompany_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CommercialCompany>(true);
			__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<IndustrialProcessData>(true);
		}
	}

	private EntityQuery m_CompanyGroup;

	private SimulationSystem m_SimulationSystem;

	private CityStatisticsSystem m_CityStatisticsSystem;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 512;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<WorkProvider>(),
			ComponentType.ReadOnly<PropertyRenter>(),
			ComponentType.ReadOnly<UpdateFrame>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<IndustrialCompany>(),
			ComponentType.ReadOnly<CommercialCompany>()
		};
		array[0] = val;
		m_CompanyGroup = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		uint updateFrame = SimulationUtils.GetUpdateFrame(m_SimulationSystem.frameIndex, 32, 16);
		JobHandle deps;
		ProcessCompanyStatisticsJob processCompanyStatisticsJob = new ProcessCompanyStatisticsJob
		{
			m_WorkProviderType = InternalCompilerInterface.GetComponentTypeHandle<WorkProvider>(ref __TypeHandle.__Game_Companies_WorkProvider_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EmployeeType = InternalCompilerInterface.GetBufferTypeHandle<Employee>(ref __TypeHandle.__Game_Companies_Employee_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = ((ComponentSystemBase)this).GetSharedComponentTypeHandle<UpdateFrame>(),
			m_CommercialCompanyType = InternalCompilerInterface.GetComponentTypeHandle<CommercialCompany>(ref __TypeHandle.__Game_Companies_CommercialCompany_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_IndustrialProcessDatas = InternalCompilerInterface.GetComponentLookup<IndustrialProcessData>(ref __TypeHandle.__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StatisticsEventQueue = m_CityStatisticsSystem.GetStatisticsEventQueue(out deps).AsParallelWriter(),
			m_UpdateFrameIndex = updateFrame
		};
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<ProcessCompanyStatisticsJob>(processCompanyStatisticsJob, m_CompanyGroup, JobHandle.CombineDependencies(((SystemBase)this).Dependency, deps));
		m_CityStatisticsSystem.AddWriter(((SystemBase)this).Dependency);
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
	public CompanyStatisticsSystem()
	{
	}
}
