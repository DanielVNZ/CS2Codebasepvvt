using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.Prefabs;
using Game.Simulation;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Serialization;

[CompilerGenerated]
public class BeginPrefabSerializationSystem : GameSystemBase
{
	[BurstCompile]
	private struct BeginPrefabSerializationJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		public ComponentTypeHandle<PrefabData> m_PrefabDataType;

		[NativeDisableParallelForRestriction]
		public NativeArray<Entity> m_PrefabArray;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabData> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabData>(ref m_PrefabDataType);
			EnabledMask enabledMask = ((ArchetypeChunk)(ref chunk)).GetEnabledMask<PrefabData>(ref m_PrefabDataType);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				PrefabData prefabData = nativeArray2[i];
				((EnabledMask)(ref enabledMask))[i] = false;
				m_PrefabArray[math.select(prefabData.m_Index, m_PrefabArray.Length + prefabData.m_Index, prefabData.m_Index < 0)] = nativeArray[i];
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct CheckSavedPrefabsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Locked> m_LockedType;

		[ReadOnly]
		public ComponentTypeHandle<SignatureBuildingData> m_SignatureBuildingType;

		[ReadOnly]
		public ComponentTypeHandle<PlacedSignatureBuildingData> m_PlacedSignatureBuildingType;

		[ReadOnly]
		public ComponentTypeHandle<CollectedCityServiceBudgetData> m_CollectedCityServiceBudgetType;

		[ReadOnly]
		public BufferTypeHandle<CollectedCityServiceFeeData> m_CollectedCityServiceFeeType;

		[ReadOnly]
		public BufferTypeHandle<CollectedCityServiceUpkeepData> m_CollectedCityServiceUpkeepType;

		public PrefabReferences m_PrefabReferences;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			PrefabComponents prefabComponents = (PrefabComponents)0u;
			if (((ArchetypeChunk)(ref chunk)).Has<SignatureBuildingData>(ref m_SignatureBuildingType))
			{
				prefabComponents |= PrefabComponents.PlacedSignatureBuilding;
			}
			PrefabComponents prefabComponents2 = (PrefabComponents)0u;
			EnabledMask enabledMask = ((ArchetypeChunk)(ref chunk)).GetEnabledMask<Locked>(ref m_LockedType);
			if (((ArchetypeChunk)(ref chunk)).Has<PlacedSignatureBuildingData>(ref m_PlacedSignatureBuildingType))
			{
				prefabComponents2 |= PrefabComponents.PlacedSignatureBuilding;
			}
			bool flag = prefabComponents != prefabComponents2 || ((ArchetypeChunk)(ref chunk)).Has<CollectedCityServiceBudgetData>(ref m_CollectedCityServiceBudgetType) || ((ArchetypeChunk)(ref chunk)).Has<CollectedCityServiceFeeData>(ref m_CollectedCityServiceFeeType) || ((ArchetypeChunk)(ref chunk)).Has<CollectedCityServiceUpkeepData>(ref m_CollectedCityServiceUpkeepType);
			SafeBitRef enableBit;
			if (!flag)
			{
				enableBit = ((EnabledMask)(ref enabledMask)).EnableBit;
				if (!((SafeBitRef)(ref enableBit)).IsValid)
				{
					return;
				}
			}
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				if (!flag)
				{
					enableBit = ((EnabledMask)(ref enabledMask)).EnableBit;
					if (!((SafeBitRef)(ref enableBit)).IsValid || ((EnabledMask)(ref enabledMask))[i])
					{
						continue;
					}
				}
				m_PrefabReferences.SetDirty(nativeArray[i]);
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct SetPrefabDataIndexJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_PrefabChunks;

		public ComponentTypeHandle<PrefabData> m_PrefabDataType;

		public BufferTypeHandle<LoadedIndex> m_LoadedIndexType;

		public void Execute()
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < m_PrefabChunks.Length; i++)
			{
				ArchetypeChunk val = m_PrefabChunks[i];
				EnabledMask enabledMask = ((ArchetypeChunk)(ref val)).GetEnabledMask<PrefabData>(ref m_PrefabDataType);
				for (int j = 0; j < ((ArchetypeChunk)(ref val)).Count; j++)
				{
					num += math.select(0, 1, ((EnabledMask)(ref enabledMask))[j]);
				}
			}
			NativeArray<int> val2 = default(NativeArray<int>);
			val2._002Ector(num, (Allocator)2, (NativeArrayOptions)1);
			num = 0;
			for (int k = 0; k < m_PrefabChunks.Length; k++)
			{
				ArchetypeChunk val3 = m_PrefabChunks[k];
				NativeArray<PrefabData> nativeArray = ((ArchetypeChunk)(ref val3)).GetNativeArray<PrefabData>(ref m_PrefabDataType);
				BufferAccessor<LoadedIndex> bufferAccessor = ((ArchetypeChunk)(ref val3)).GetBufferAccessor<LoadedIndex>(ref m_LoadedIndexType);
				EnabledMask enabledMask2 = ((ArchetypeChunk)(ref val3)).GetEnabledMask<PrefabData>(ref m_PrefabDataType);
				for (int l = 0; l < ((ArchetypeChunk)(ref val3)).Count; l++)
				{
					if (((EnabledMask)(ref enabledMask2))[l])
					{
						PrefabData prefabData = nativeArray[l];
						DynamicBuffer<LoadedIndex> val4 = bufferAccessor[l];
						val4.ResizeUninitialized(1);
						val4[0] = new LoadedIndex
						{
							m_Index = prefabData.m_Index
						};
						val2[num++] = prefabData.m_Index;
						num2 += math.select(0, 1, prefabData.m_Index < 0);
					}
				}
			}
			NativeSortExtension.Sort<int>(val2);
			for (int m = 0; m < m_PrefabChunks.Length; m++)
			{
				ArchetypeChunk val5 = m_PrefabChunks[m];
				NativeArray<PrefabData> nativeArray2 = ((ArchetypeChunk)(ref val5)).GetNativeArray<PrefabData>(ref m_PrefabDataType);
				EnabledMask enabledMask3 = ((ArchetypeChunk)(ref val5)).GetEnabledMask<PrefabData>(ref m_PrefabDataType);
				for (int n = 0; n < ((ArchetypeChunk)(ref val5)).Count; n++)
				{
					if (!((EnabledMask)(ref enabledMask3))[n])
					{
						continue;
					}
					PrefabData prefabData2 = nativeArray2[n];
					int num3 = 0;
					int num4 = num;
					while (num3 < num4)
					{
						int num5 = num3 + num4 >> 1;
						int num6 = val2[num5];
						if (num6 < prefabData2.m_Index)
						{
							num3 = num5 + 1;
							continue;
						}
						if (num6 > prefabData2.m_Index)
						{
							num4 = num5;
							continue;
						}
						num3 = num5;
						break;
					}
					prefabData2.m_Index = num3 - num2;
					nativeArray2[n] = prefabData2;
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		public ComponentTypeHandle<PrefabData> __Game_Prefabs_PrefabData_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Locked> __Game_Prefabs_Locked_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<SignatureBuildingData> __Game_Prefabs_SignatureBuildingData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PlacedSignatureBuildingData> __Game_Prefabs_PlacedSignatureBuildingData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CollectedCityServiceBudgetData> __Game_Simulation_CollectedCityServiceBudgetData_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<CollectedCityServiceFeeData> __Game_Simulation_CollectedCityServiceFeeData_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<CollectedCityServiceUpkeepData> __Game_Simulation_CollectedCityServiceUpkeepData_RO_BufferTypeHandle;

		public BufferTypeHandle<LoadedIndex> __Game_Prefabs_LoadedIndex_RW_BufferTypeHandle;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_PrefabData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabData>(false);
			__Game_Prefabs_Locked_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Locked>(true);
			__Game_Prefabs_SignatureBuildingData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<SignatureBuildingData>(true);
			__Game_Prefabs_PlacedSignatureBuildingData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PlacedSignatureBuildingData>(true);
			__Game_Simulation_CollectedCityServiceBudgetData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CollectedCityServiceBudgetData>(true);
			__Game_Simulation_CollectedCityServiceFeeData_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<CollectedCityServiceFeeData>(true);
			__Game_Simulation_CollectedCityServiceUpkeepData_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<CollectedCityServiceUpkeepData>(true);
			__Game_Prefabs_LoadedIndex_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<LoadedIndex>(false);
		}
	}

	private SaveGameSystem m_SaveGameSystem;

	private CheckPrefabReferencesSystem m_CheckPrefabReferencesSystem;

	private UpdateSystem m_UpdateSystem;

	private EntityQuery m_EnabledPrefabsQuery;

	private EntityQuery m_LoadedPrefabsQuery;

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
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SaveGameSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SaveGameSystem>();
		m_CheckPrefabReferencesSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CheckPrefabReferencesSystem>();
		m_UpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UpdateSystem>();
		m_EnabledPrefabsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PrefabData>() });
		m_LoadedPrefabsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<LoadedIndex>() });
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		int num = ((EntityQuery)(ref m_LoadedPrefabsQuery)).CalculateEntityCountWithoutFiltering();
		NativeArray<Entity> val = default(NativeArray<Entity>);
		val._002Ector(num, (Allocator)3, (NativeArrayOptions)1);
		JobHandle dependencies = JobChunkExtensions.ScheduleParallel<BeginPrefabSerializationJob>(new BeginPrefabSerializationJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabDataType = InternalCompilerInterface.GetComponentTypeHandle<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabArray = val
		}, m_LoadedPrefabsQuery, ((SystemBase)this).Dependency);
		m_CheckPrefabReferencesSystem.BeginPrefabCheck(val, isLoading: false, dependencies);
		m_UpdateSystem.Update(SystemUpdatePhase.PrefabReferences);
		Context context = m_SaveGameSystem.context;
		if ((int)((Context)(ref context)).purpose == 0)
		{
			JobHandle dependencies3;
			JobHandle dependencies2 = JobChunkExtensions.ScheduleParallel<CheckSavedPrefabsJob>(new CheckSavedPrefabsJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_LockedType = InternalCompilerInterface.GetComponentTypeHandle<Locked>(ref __TypeHandle.__Game_Prefabs_Locked_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_SignatureBuildingType = InternalCompilerInterface.GetComponentTypeHandle<SignatureBuildingData>(ref __TypeHandle.__Game_Prefabs_SignatureBuildingData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PlacedSignatureBuildingType = InternalCompilerInterface.GetComponentTypeHandle<PlacedSignatureBuildingData>(ref __TypeHandle.__Game_Prefabs_PlacedSignatureBuildingData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CollectedCityServiceBudgetType = InternalCompilerInterface.GetComponentTypeHandle<CollectedCityServiceBudgetData>(ref __TypeHandle.__Game_Simulation_CollectedCityServiceBudgetData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CollectedCityServiceFeeType = InternalCompilerInterface.GetBufferTypeHandle<CollectedCityServiceFeeData>(ref __TypeHandle.__Game_Simulation_CollectedCityServiceFeeData_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CollectedCityServiceUpkeepType = InternalCompilerInterface.GetBufferTypeHandle<CollectedCityServiceUpkeepData>(ref __TypeHandle.__Game_Simulation_CollectedCityServiceUpkeepData_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabReferences = m_CheckPrefabReferencesSystem.GetPrefabReferences((SystemBase)(object)this, out dependencies3)
			}, m_LoadedPrefabsQuery, dependencies3);
			m_CheckPrefabReferencesSystem.AddPrefabReferencesUser(dependencies2);
			((ComponentSystemBase)m_CheckPrefabReferencesSystem).Update();
		}
		m_CheckPrefabReferencesSystem.EndPrefabCheck(out var dependencies4);
		val.Dispose(dependencies4);
		((JobHandle)(ref dependencies4)).Complete();
		JobHandle val2 = default(JobHandle);
		NativeList<ArchetypeChunk> prefabChunks = ((EntityQuery)(ref m_EnabledPrefabsQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), dependencies4, ref val2);
		JobHandle val3 = IJobExtensions.Schedule<SetPrefabDataIndexJob>(new SetPrefabDataIndexJob
		{
			m_PrefabChunks = prefabChunks,
			m_PrefabDataType = InternalCompilerInterface.GetComponentTypeHandle<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LoadedIndexType = InternalCompilerInterface.GetBufferTypeHandle<LoadedIndex>(ref __TypeHandle.__Game_Prefabs_LoadedIndex_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef)
		}, val2);
		prefabChunks.Dispose(val3);
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
	public BeginPrefabSerializationSystem()
	{
	}
}
