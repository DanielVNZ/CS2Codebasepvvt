using System;
using System.Runtime.CompilerServices;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Tools;
using Game.Vehicles;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class VehicleSpawnSystem : GameSystemBase
{
	private struct SpawnData : IComparable<SpawnData>
	{
		public Entity m_Source;

		public Entity m_Vehicle;

		public int m_Priority;

		public int CompareTo(SpawnData other)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			return math.select(m_Priority - other.m_Priority, m_Source.Index - other.m_Source.Index, m_Source.Index != other.m_Source.Index);
		}
	}

	private struct SpawnRange
	{
		public int m_Start;

		public int m_End;
	}

	[BurstCompile]
	private struct GroupSpawnSourcesJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Controller> m_ControllerType;

		public ComponentTypeHandle<TripSource> m_SpawnSourceType;

		public NativeList<SpawnData> m_SpawnData;

		public NativeList<SpawnRange> m_SpawnGroups;

		public void Execute()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			SpawnData spawnData = default(SpawnData);
			SpawnData spawnData2 = default(SpawnData);
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				ArchetypeChunk val = m_Chunks[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				NativeArray<TripSource> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<TripSource>(ref m_SpawnSourceType);
				NativeArray<Controller> nativeArray3 = ((ArchetypeChunk)(ref val)).GetNativeArray<Controller>(ref m_ControllerType);
				if (nativeArray3.Length != 0)
				{
					for (int j = 0; j < nativeArray.Length; j++)
					{
						spawnData.m_Vehicle = nativeArray[j];
						Controller controller = nativeArray3[j];
						if (!(controller.m_Controller != Entity.Null) || !(controller.m_Controller != spawnData.m_Vehicle))
						{
							TripSource tripSource = nativeArray2[j];
							if (tripSource.m_Timer <= 0)
							{
								spawnData.m_Source = nativeArray2[j].m_Source;
								spawnData.m_Priority = tripSource.m_Timer;
								m_SpawnData.Add(ref spawnData);
							}
							tripSource.m_Timer -= 16;
							nativeArray2[j] = tripSource;
						}
					}
					continue;
				}
				for (int k = 0; k < nativeArray.Length; k++)
				{
					TripSource tripSource2 = nativeArray2[k];
					if (tripSource2.m_Timer <= 0)
					{
						spawnData2.m_Source = nativeArray2[k].m_Source;
						spawnData2.m_Vehicle = nativeArray[k];
						spawnData2.m_Priority = tripSource2.m_Timer;
						m_SpawnData.Add(ref spawnData2);
					}
					tripSource2.m_Timer -= 16;
					nativeArray2[k] = tripSource2;
				}
			}
			NativeSortExtension.Sort<SpawnData>(m_SpawnData);
			SpawnRange spawnRange = default(SpawnRange);
			spawnRange.m_Start = -1;
			Entity val2 = Entity.Null;
			for (int l = 0; l < m_SpawnData.Length; l++)
			{
				Entity val3 = m_SpawnData[l].m_Source;
				if (val3 != val2)
				{
					if (spawnRange.m_Start != -1)
					{
						spawnRange.m_End = l;
						m_SpawnGroups.Add(ref spawnRange);
					}
					spawnRange.m_Start = l;
					val2 = val3;
				}
			}
			if (spawnRange.m_Start != -1)
			{
				spawnRange.m_End = m_SpawnData.Length;
				m_SpawnGroups.Add(ref spawnRange);
			}
		}
	}

	private struct LaneBufferItem
	{
		public Entity m_Lane;

		public float2 m_Delta;

		public LaneBufferItem(Entity lane, float2 delta)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			m_Lane = lane;
			m_Delta = delta;
		}
	}

	[BurstCompile]
	private struct TrySpawnVehiclesJob : IJobParallelForDefer
	{
		[ReadOnly]
		public NativeArray<SpawnData> m_SpawnData;

		[ReadOnly]
		public NativeArray<SpawnRange> m_SpawnGroups;

		[ReadOnly]
		public ComponentLookup<TrainCurrentLane> m_TrainCurrentLaneData;

		[ReadOnly]
		public ComponentLookup<ParkedTrain> m_ParkedTrainData;

		[ReadOnly]
		public ComponentLookup<Controller> m_ControllerData;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_Layouts;

		[ReadOnly]
		public BufferLookup<LaneObject> m_LaneObjects;

		public ParallelWriter m_CommandBuffer;

		public void Execute(int index)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			SpawnRange spawnRange = m_SpawnGroups[index];
			_ = m_SpawnData[spawnRange.m_Start];
			Entity lastRemoved = Entity.Null;
			DynamicBuffer<LayoutElement> val2 = default(DynamicBuffer<LayoutElement>);
			for (int i = spawnRange.m_Start; i < spawnRange.m_End; i++)
			{
				Entity val = m_SpawnData[i].m_Vehicle;
				if (m_Layouts.TryGetBuffer(val, ref val2) && val2.Length != 0)
				{
					bool flag = true;
					for (int j = 0; j < val2.Length; j++)
					{
						flag &= CheckSpaceForVehicle(index, val, val2[j].m_Vehicle, ref lastRemoved);
					}
					if (flag)
					{
						for (int k = 0; k < val2.Length; k++)
						{
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TripSource>(index, val2[k].m_Vehicle);
						}
					}
				}
				else if (CheckSpaceForVehicle(index, val, val, ref lastRemoved))
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TripSource>(index, val);
				}
			}
		}

		private bool CheckSpaceForVehicle(int jobIndex, Entity vehicle, Entity vehicle2, ref Entity lastRemoved)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			bool flag = true;
			TrainCurrentLane trainCurrentLane = default(TrainCurrentLane);
			if (m_TrainCurrentLaneData.TryGetComponent(vehicle2, ref trainCurrentLane))
			{
				flag &= CheckSpaceForTrain(jobIndex, vehicle, trainCurrentLane.m_Front.m_Lane, ref lastRemoved);
				if (trainCurrentLane.m_Rear.m_Lane != trainCurrentLane.m_Front.m_Lane)
				{
					flag &= CheckSpaceForTrain(jobIndex, vehicle, trainCurrentLane.m_Rear.m_Lane, ref lastRemoved);
				}
			}
			return flag;
		}

		private bool CheckSpaceForTrain(int jobIndex, Entity vehicle, Entity lane, ref Entity lastRemoved)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			bool result = true;
			DynamicBuffer<LaneObject> val = default(DynamicBuffer<LaneObject>);
			if (m_LaneObjects.TryGetBuffer(lane, ref val))
			{
				Controller controller = default(Controller);
				DynamicBuffer<LayoutElement> layout = default(DynamicBuffer<LayoutElement>);
				for (int i = 0; i < val.Length; i++)
				{
					LaneObject laneObject = val[i];
					Entity val2 = Entity.Null;
					if (m_ParkedTrainData.HasComponent(laneObject.m_LaneObject))
					{
						val2 = laneObject.m_LaneObject;
						if (m_ControllerData.TryGetComponent(laneObject.m_LaneObject, ref controller))
						{
							val2 = controller.m_Controller;
						}
					}
					if (val2 != Entity.Null && val2 != vehicle && lastRemoved != val2)
					{
						m_Layouts.TryGetBuffer(val2, ref layout);
						VehicleUtils.DeleteVehicle(m_CommandBuffer, jobIndex, val2, layout);
						lastRemoved = val2;
					}
				}
			}
			return result;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Controller> __Game_Vehicles_Controller_RO_ComponentTypeHandle;

		public ComponentTypeHandle<TripSource> __Game_Objects_TripSource_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<TrainCurrentLane> __Game_Vehicles_TrainCurrentLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkedTrain> __Game_Vehicles_ParkedTrain_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Controller> __Game_Vehicles_Controller_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LaneObject> __Game_Net_LaneObject_RO_BufferLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Vehicles_Controller_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Controller>(true);
			__Game_Objects_TripSource_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TripSource>(false);
			__Game_Vehicles_TrainCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrainCurrentLane>(true);
			__Game_Vehicles_ParkedTrain_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedTrain>(true);
			__Game_Vehicles_Controller_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Controller>(true);
			__Game_Vehicles_LayoutElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(true);
			__Game_Net_LaneObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneObject>(true);
		}
	}

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_VehicleQuery;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_VehicleQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadWrite<TripSource>(),
			ComponentType.ReadOnly<Vehicle>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_VehicleQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		NativeList<SpawnData> spawnData = default(NativeList<SpawnData>);
		spawnData._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeList<SpawnRange> val = default(NativeList<SpawnRange>);
		val._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		JobHandle val2 = default(JobHandle);
		NativeList<ArchetypeChunk> chunks = ((EntityQuery)(ref m_VehicleQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val2);
		GroupSpawnSourcesJob groupSpawnSourcesJob = new GroupSpawnSourcesJob
		{
			m_Chunks = chunks,
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ControllerType = InternalCompilerInterface.GetComponentTypeHandle<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnSourceType = InternalCompilerInterface.GetComponentTypeHandle<TripSource>(ref __TypeHandle.__Game_Objects_TripSource_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnData = spawnData,
			m_SpawnGroups = val
		};
		TrySpawnVehiclesJob trySpawnVehiclesJob = new TrySpawnVehiclesJob
		{
			m_SpawnData = spawnData.AsDeferredJobArray(),
			m_SpawnGroups = val.AsDeferredJobArray(),
			m_TrainCurrentLaneData = InternalCompilerInterface.GetComponentLookup<TrainCurrentLane>(ref __TypeHandle.__Game_Vehicles_TrainCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedTrainData = InternalCompilerInterface.GetComponentLookup<ParkedTrain>(ref __TypeHandle.__Game_Vehicles_ParkedTrain_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ControllerData = InternalCompilerInterface.GetComponentLookup<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Layouts = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneObjects = InternalCompilerInterface.GetBufferLookup<LaneObject>(ref __TypeHandle.__Game_Net_LaneObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		EntityCommandBuffer val3 = m_EndFrameBarrier.CreateCommandBuffer();
		trySpawnVehiclesJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val3)).AsParallelWriter();
		TrySpawnVehiclesJob trySpawnVehiclesJob2 = trySpawnVehiclesJob;
		JobHandle val4 = IJobExtensions.Schedule<GroupSpawnSourcesJob>(groupSpawnSourcesJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val2));
		JobHandle val5 = IJobParallelForDeferExtensions.Schedule<TrySpawnVehiclesJob, SpawnRange>(trySpawnVehiclesJob2, val, 1, val4);
		spawnData.Dispose(val5);
		val.Dispose(val5);
		chunks.Dispose(val5);
		m_EndFrameBarrier.AddJobHandleForProducer(val5);
		((SystemBase)this).Dependency = val5;
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
	public VehicleSpawnSystem()
	{
	}
}
