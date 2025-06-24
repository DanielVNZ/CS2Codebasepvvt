using System.Runtime.CompilerServices;
using Colossal.Entities;
using Game.Common;
using Game.Prefabs;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Serialization;

[CompilerGenerated]
public class SecondaryPrefabReferencesSystem : GameSystemBase
{
	[BurstCompile]
	private struct FixSpawnableBuildingJob : IJobChunk
	{
		public ComponentTypeHandle<SpawnableBuildingData> m_SpawnableBuildingType;

		public PrefabReferences m_PrefabReferences;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<SpawnableBuildingData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<SpawnableBuildingData>(ref m_SpawnableBuildingType);
			ChunkEntityEnumerator val = default(ChunkEntityEnumerator);
			((ChunkEntityEnumerator)(ref val))._002Ector(useEnabledMask, chunkEnabledMask, ((ArchetypeChunk)(ref chunk)).Count);
			int num = default(int);
			while (((ChunkEntityEnumerator)(ref val)).NextEntityIndex(ref num))
			{
				SpawnableBuildingData spawnableBuildingData = nativeArray[num];
				if (spawnableBuildingData.m_ZonePrefab != Entity.Null)
				{
					m_PrefabReferences.Check(ref spawnableBuildingData.m_ZonePrefab);
				}
				nativeArray[num] = spawnableBuildingData;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct FixPlaceholderBuildingJob : IJobChunk
	{
		public ComponentTypeHandle<PlaceholderBuildingData> m_PlaceholderBuildingType;

		public PrefabReferences m_PrefabReferences;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<PlaceholderBuildingData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PlaceholderBuildingData>(ref m_PlaceholderBuildingType);
			ChunkEntityEnumerator val = default(ChunkEntityEnumerator);
			((ChunkEntityEnumerator)(ref val))._002Ector(useEnabledMask, chunkEnabledMask, ((ArchetypeChunk)(ref chunk)).Count);
			int num = default(int);
			while (((ChunkEntityEnumerator)(ref val)).NextEntityIndex(ref num))
			{
				PlaceholderBuildingData placeholderBuildingData = nativeArray[num];
				if (placeholderBuildingData.m_ZonePrefab != Entity.Null)
				{
					m_PrefabReferences.Check(ref placeholderBuildingData.m_ZonePrefab);
				}
				nativeArray[num] = placeholderBuildingData;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct FixServiceObjectDataJob : IJobChunk
	{
		public ComponentTypeHandle<ServiceObjectData> m_ServiceObjectType;

		public PrefabReferences m_PrefabReferences;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<ServiceObjectData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ServiceObjectData>(ref m_ServiceObjectType);
			ChunkEntityEnumerator val = default(ChunkEntityEnumerator);
			((ChunkEntityEnumerator)(ref val))._002Ector(useEnabledMask, chunkEnabledMask, ((ArchetypeChunk)(ref chunk)).Count);
			int num = default(int);
			while (((ChunkEntityEnumerator)(ref val)).NextEntityIndex(ref num))
			{
				ServiceObjectData serviceObjectData = nativeArray[num];
				if (serviceObjectData.m_Service != Entity.Null)
				{
					m_PrefabReferences.Check(ref serviceObjectData.m_Service);
				}
				nativeArray[num] = serviceObjectData;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct FixNetLaneDataJob : IJobChunk
	{
		public ComponentTypeHandle<NetLaneData> m_NetLaneType;

		public PrefabReferences m_PrefabReferences;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<NetLaneData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<NetLaneData>(ref m_NetLaneType);
			ChunkEntityEnumerator val = default(ChunkEntityEnumerator);
			((ChunkEntityEnumerator)(ref val))._002Ector(useEnabledMask, chunkEnabledMask, ((ArchetypeChunk)(ref chunk)).Count);
			int num = default(int);
			while (((ChunkEntityEnumerator)(ref val)).NextEntityIndex(ref num))
			{
				NetLaneData netLaneData = nativeArray[num];
				if (netLaneData.m_PathfindPrefab != Entity.Null)
				{
					m_PrefabReferences.Check(ref netLaneData.m_PathfindPrefab);
				}
				nativeArray[num] = netLaneData;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct FixTransportLineDataJob : IJobChunk
	{
		public ComponentTypeHandle<TransportLineData> m_TransportLineType;

		public PrefabReferences m_PrefabReferences;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<TransportLineData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TransportLineData>(ref m_TransportLineType);
			ChunkEntityEnumerator val = default(ChunkEntityEnumerator);
			((ChunkEntityEnumerator)(ref val))._002Ector(useEnabledMask, chunkEnabledMask, ((ArchetypeChunk)(ref chunk)).Count);
			int num = default(int);
			while (((ChunkEntityEnumerator)(ref val)).NextEntityIndex(ref num))
			{
				TransportLineData transportLineData = nativeArray[num];
				if (transportLineData.m_PathfindPrefab != Entity.Null)
				{
					m_PrefabReferences.Check(ref transportLineData.m_PathfindPrefab);
				}
				nativeArray[num] = transportLineData;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct FixContentPrerequisiteDataJob : IJobChunk
	{
		public ComponentTypeHandle<ContentPrerequisiteData> m_ContentPrerequisiteType;

		public PrefabReferences m_PrefabReferences;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<ContentPrerequisiteData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ContentPrerequisiteData>(ref m_ContentPrerequisiteType);
			ChunkEntityEnumerator val = default(ChunkEntityEnumerator);
			((ChunkEntityEnumerator)(ref val))._002Ector(useEnabledMask, chunkEnabledMask, ((ArchetypeChunk)(ref chunk)).Count);
			int num = default(int);
			while (((ChunkEntityEnumerator)(ref val)).NextEntityIndex(ref num))
			{
				ContentPrerequisiteData contentPrerequisiteData = nativeArray[num];
				if (contentPrerequisiteData.m_ContentPrerequisite != Entity.Null)
				{
					m_PrefabReferences.Check(ref contentPrerequisiteData.m_ContentPrerequisite);
				}
				nativeArray[num] = contentPrerequisiteData;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		public ComponentTypeHandle<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<PlaceholderBuildingData> __Game_Prefabs_PlaceholderBuildingData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<ServiceObjectData> __Game_Prefabs_ServiceObjectData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<NetLaneData> __Game_Prefabs_NetLaneData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<TransportLineData> __Game_Prefabs_TransportLineData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<ContentPrerequisiteData> __Game_Prefabs_ContentPrerequisiteData_RW_ComponentTypeHandle;

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
			__Game_Prefabs_SpawnableBuildingData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<SpawnableBuildingData>(false);
			__Game_Prefabs_PlaceholderBuildingData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PlaceholderBuildingData>(false);
			__Game_Prefabs_ServiceObjectData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ServiceObjectData>(false);
			__Game_Prefabs_NetLaneData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NetLaneData>(false);
			__Game_Prefabs_TransportLineData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TransportLineData>(false);
			__Game_Prefabs_ContentPrerequisiteData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ContentPrerequisiteData>(false);
		}
	}

	private CheckPrefabReferencesSystem m_CheckPrefabReferencesSystem;

	private EntityQuery m_SpawnableBuildingQuery;

	private EntityQuery m_PlaceholderBuildingQuery;

	private EntityQuery m_ServiceObjectQuery;

	private EntityQuery m_NetLaneQuery;

	private EntityQuery m_TransportLineQuery;

	private EntityQuery m_ContentPrerequisiteQuery;

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
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CheckPrefabReferencesSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CheckPrefabReferencesSystem>();
		m_SpawnableBuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<SpawnableBuildingData>(),
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.Exclude<Deleted>()
		});
		m_PlaceholderBuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<PlaceholderBuildingData>(),
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.Exclude<Deleted>()
		});
		m_ServiceObjectQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<ServiceObjectData>(),
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.Exclude<Deleted>()
		});
		m_NetLaneQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<NetLaneData>(),
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.Exclude<Deleted>()
		});
		m_TransportLineQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<TransportLineData>(),
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.Exclude<Deleted>()
		});
		m_ContentPrerequisiteQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<ContentPrerequisiteData>(),
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.Exclude<Deleted>()
		});
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		PrefabReferences prefabReferences = m_CheckPrefabReferencesSystem.GetPrefabReferences((SystemBase)(object)this, out dependencies);
		dependencies = JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies);
		FixSpawnableBuildingJob fixSpawnableBuildingJob = new FixSpawnableBuildingJob
		{
			m_SpawnableBuildingType = InternalCompilerInterface.GetComponentTypeHandle<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabReferences = prefabReferences
		};
		FixPlaceholderBuildingJob fixPlaceholderBuildingJob = new FixPlaceholderBuildingJob
		{
			m_PlaceholderBuildingType = InternalCompilerInterface.GetComponentTypeHandle<PlaceholderBuildingData>(ref __TypeHandle.__Game_Prefabs_PlaceholderBuildingData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabReferences = prefabReferences
		};
		FixServiceObjectDataJob fixServiceObjectDataJob = new FixServiceObjectDataJob
		{
			m_ServiceObjectType = InternalCompilerInterface.GetComponentTypeHandle<ServiceObjectData>(ref __TypeHandle.__Game_Prefabs_ServiceObjectData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabReferences = prefabReferences
		};
		FixNetLaneDataJob fixNetLaneDataJob = new FixNetLaneDataJob
		{
			m_NetLaneType = InternalCompilerInterface.GetComponentTypeHandle<NetLaneData>(ref __TypeHandle.__Game_Prefabs_NetLaneData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabReferences = prefabReferences
		};
		FixTransportLineDataJob fixTransportLineDataJob = new FixTransportLineDataJob
		{
			m_TransportLineType = InternalCompilerInterface.GetComponentTypeHandle<TransportLineData>(ref __TypeHandle.__Game_Prefabs_TransportLineData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabReferences = prefabReferences
		};
		FixContentPrerequisiteDataJob obj = new FixContentPrerequisiteDataJob
		{
			m_ContentPrerequisiteType = InternalCompilerInterface.GetComponentTypeHandle<ContentPrerequisiteData>(ref __TypeHandle.__Game_Prefabs_ContentPrerequisiteData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabReferences = prefabReferences
		};
		JobHandle val = JobChunkExtensions.ScheduleParallel<FixSpawnableBuildingJob>(fixSpawnableBuildingJob, m_SpawnableBuildingQuery, dependencies);
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<FixPlaceholderBuildingJob>(fixPlaceholderBuildingJob, m_PlaceholderBuildingQuery, dependencies);
		JobHandle val3 = JobChunkExtensions.ScheduleParallel<FixServiceObjectDataJob>(fixServiceObjectDataJob, m_ServiceObjectQuery, dependencies);
		JobHandle val4 = JobChunkExtensions.ScheduleParallel<FixNetLaneDataJob>(fixNetLaneDataJob, m_NetLaneQuery, dependencies);
		JobHandle val5 = JobChunkExtensions.ScheduleParallel<FixTransportLineDataJob>(fixTransportLineDataJob, m_TransportLineQuery, dependencies);
		JobHandle val6 = JobChunkExtensions.ScheduleParallel<FixContentPrerequisiteDataJob>(obj, m_ContentPrerequisiteQuery, dependencies);
		dependencies = JobUtils.CombineDependencies(val, val2, val3, val4, val5, val6);
		m_CheckPrefabReferencesSystem.AddPrefabReferencesUser(dependencies);
		((SystemBase)this).Dependency = dependencies;
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
	public SecondaryPrefabReferencesSystem()
	{
	}
}
