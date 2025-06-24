using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Common;
using Game.Prefabs;
using Game.Routes;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class GenerateRoutesSystem : GameSystemBase
{
	[BurstCompile]
	private struct CreateRoutesJob : IJobChunk
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_SubElementChunks;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<CreationDefinition> m_CreationDefinitionType;

		[ReadOnly]
		public ComponentTypeHandle<Waypoint> m_WaypointType;

		[ReadOnly]
		public ComponentTypeHandle<Segment> m_SegmentType;

		[ReadOnly]
		public ComponentTypeHandle<ColorDefinition> m_ColorDefinitionType;

		[ReadOnly]
		public BufferTypeHandle<WaypointDefinition> m_WaypointDefinitionType;

		[ReadOnly]
		public ComponentLookup<Color> m_ColorData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<RouteData> m_RouteData;

		[ReadOnly]
		public ComponentLookup<TransportLineData> m_TransportLineData;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<CreationDefinition> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CreationDefinition>(ref m_CreationDefinitionType);
			NativeArray<ColorDefinition> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ColorDefinition>(ref m_ColorDefinitionType);
			BufferAccessor<WaypointDefinition> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<WaypointDefinition>(ref m_WaypointDefinitionType);
			for (int num = 0; num < nativeArray.Length; num++)
			{
				CreationDefinition creationDefinition = nativeArray[num];
				DynamicBuffer<WaypointDefinition> val = bufferAccessor[num];
				RouteFlags routeFlags = (RouteFlags)0;
				TempFlags tempFlags = (TempFlags)0u;
				RouteData routeData;
				if (creationDefinition.m_Original != Entity.Null)
				{
					routeFlags |= RouteFlags.Complete;
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Hidden>(unfilteredChunkIndex, creationDefinition.m_Original, default(Hidden));
					creationDefinition.m_Prefab = m_PrefabRefData[creationDefinition.m_Original].m_Prefab;
					routeData = m_RouteData[creationDefinition.m_Prefab];
					routeData.m_Color = m_ColorData[creationDefinition.m_Original].m_Color;
					tempFlags = (((creationDefinition.m_Flags & CreationFlags.Delete) != 0) ? (tempFlags | TempFlags.Delete) : (((creationDefinition.m_Flags & CreationFlags.Select) == 0) ? (tempFlags | TempFlags.Modify) : (tempFlags | TempFlags.Select)));
				}
				else
				{
					tempFlags |= TempFlags.Create;
					routeData = m_RouteData[creationDefinition.m_Prefab];
					if (nativeArray2.Length != 0)
					{
						routeData.m_Color = nativeArray2[num].m_Color;
					}
				}
				Entity val2 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(unfilteredChunkIndex, routeData.m_RouteArchetype);
				DynamicBuffer<RouteWaypoint> val3 = ((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<RouteWaypoint>(unfilteredChunkIndex, val2);
				DynamicBuffer<RouteSegment> val4 = ((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<RouteSegment>(unfilteredChunkIndex, val2);
				if ((routeFlags & RouteFlags.Complete) == 0 && val.Length >= 3)
				{
					WaypointDefinition waypointDefinition = val[0];
					if (((float3)(ref waypointDefinition.m_Position)).Equals(val[val.Length - 1].m_Position))
					{
						CollectionUtils.ResizeInitialized<RouteWaypoint>(val3, val.Length - 1, default(RouteWaypoint));
						CollectionUtils.ResizeInitialized<RouteSegment>(val4, val.Length - 1, default(RouteSegment));
						FindSubElements(val3, val4);
						routeFlags |= RouteFlags.Complete;
						goto IL_022a;
					}
				}
				CollectionUtils.ResizeInitialized<RouteWaypoint>(val3, val.Length, default(RouteWaypoint));
				CollectionUtils.ResizeInitialized<RouteSegment>(val4, val.Length, default(RouteSegment));
				FindSubElements(val3, val4);
				goto IL_022a;
				IL_022a:
				if (creationDefinition.m_Owner != Entity.Null)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(unfilteredChunkIndex, val2, new Owner(creationDefinition.m_Owner));
				}
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Route>(unfilteredChunkIndex, val2, new Route
				{
					m_Flags = routeFlags
				});
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Temp>(unfilteredChunkIndex, val2, new Temp(creationDefinition.m_Original, tempFlags));
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(unfilteredChunkIndex, val2, new PrefabRef(creationDefinition.m_Prefab));
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Color>(unfilteredChunkIndex, val2, new Color(routeData.m_Color));
				if (m_TransportLineData.HasComponent(creationDefinition.m_Prefab))
				{
					TransportLineData transportLineData = m_TransportLineData[creationDefinition.m_Prefab];
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<TransportLine>(unfilteredChunkIndex, val2, new TransportLine(transportLineData));
				}
			}
		}

		private void FindSubElements(DynamicBuffer<RouteWaypoint> routeWaypoints, DynamicBuffer<RouteSegment> routeSegments)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_SubElementChunks.Length; i++)
			{
				ArchetypeChunk val = m_SubElementChunks[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				NativeArray<Waypoint> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<Waypoint>(ref m_WaypointType);
				NativeArray<Segment> nativeArray3 = ((ArchetypeChunk)(ref val)).GetNativeArray<Segment>(ref m_SegmentType);
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					routeWaypoints[nativeArray2[j].m_Index] = new RouteWaypoint(nativeArray[j]);
				}
				for (int k = 0; k < nativeArray3.Length; k++)
				{
					routeSegments[nativeArray3[k].m_Index] = new RouteSegment(nativeArray[k]);
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

		[ReadOnly]
		public ComponentTypeHandle<CreationDefinition> __Game_Tools_CreationDefinition_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Waypoint> __Game_Routes_Waypoint_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Segment> __Game_Routes_Segment_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<WaypointDefinition> __Game_Routes_WaypointDefinition_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ColorDefinition> __Game_Tools_ColorDefinition_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Color> __Game_Routes_Color_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RouteData> __Game_Prefabs_RouteData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TransportLineData> __Game_Prefabs_TransportLineData_RO_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Tools_CreationDefinition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CreationDefinition>(true);
			__Game_Routes_Waypoint_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Waypoint>(true);
			__Game_Routes_Segment_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Segment>(true);
			__Game_Routes_WaypointDefinition_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<WaypointDefinition>(true);
			__Game_Tools_ColorDefinition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ColorDefinition>(true);
			__Game_Routes_Color_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Color>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_RouteData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RouteData>(true);
			__Game_Prefabs_TransportLineData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TransportLineData>(true);
		}
	}

	private ModificationBarrier2 m_ModificationBarrier;

	private EntityQuery m_DefinitionQuery;

	private EntityQuery m_SubElementQuery;

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
		//IL_005d: Expected O, but got Unknown
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier2>();
		m_DefinitionQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<CreationDefinition>(),
			ComponentType.ReadOnly<WaypointDefinition>(),
			ComponentType.ReadOnly<Updated>()
		});
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Updated>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Waypoint>(),
			ComponentType.ReadOnly<Segment>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Deleted>() };
		array[0] = val;
		m_SubElementQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		((ComponentSystemBase)this).RequireForUpdate(m_DefinitionQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		NativeList<ArchetypeChunk> subElementChunks = ((EntityQuery)(ref m_SubElementQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		CreateRoutesJob createRoutesJob = new CreateRoutesJob
		{
			m_SubElementChunks = subElementChunks,
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CreationDefinitionType = InternalCompilerInterface.GetComponentTypeHandle<CreationDefinition>(ref __TypeHandle.__Game_Tools_CreationDefinition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WaypointType = InternalCompilerInterface.GetComponentTypeHandle<Waypoint>(ref __TypeHandle.__Game_Routes_Waypoint_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SegmentType = InternalCompilerInterface.GetComponentTypeHandle<Segment>(ref __TypeHandle.__Game_Routes_Segment_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WaypointDefinitionType = InternalCompilerInterface.GetBufferTypeHandle<WaypointDefinition>(ref __TypeHandle.__Game_Routes_WaypointDefinition_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ColorDefinitionType = InternalCompilerInterface.GetComponentTypeHandle<ColorDefinition>(ref __TypeHandle.__Game_Tools_ColorDefinition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ColorData = InternalCompilerInterface.GetComponentLookup<Color>(ref __TypeHandle.__Game_Routes_Color_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteData = InternalCompilerInterface.GetComponentLookup<RouteData>(ref __TypeHandle.__Game_Prefabs_RouteData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransportLineData = InternalCompilerInterface.GetComponentLookup<TransportLineData>(ref __TypeHandle.__Game_Prefabs_TransportLineData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		EntityCommandBuffer val2 = m_ModificationBarrier.CreateCommandBuffer();
		createRoutesJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
		JobHandle val3 = JobChunkExtensions.ScheduleParallel<CreateRoutesJob>(createRoutesJob, m_DefinitionQuery, JobHandle.CombineDependencies(val, ((SystemBase)this).Dependency));
		subElementChunks.Dispose(val3);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val3);
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
	public GenerateRoutesSystem()
	{
	}
}
