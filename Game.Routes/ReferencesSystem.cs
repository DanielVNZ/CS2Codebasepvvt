using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Common;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Routes;

[CompilerGenerated]
public class ReferencesSystem : GameSystemBase
{
	[BurstCompile]
	private struct UpdateRouteReferencesJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_RouteChunks;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public BufferTypeHandle<RouteWaypoint> m_RouteWaypointType;

		[ReadOnly]
		public BufferTypeHandle<RouteSegment> m_RouteSegmentType;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		public ComponentLookup<Owner> m_OwnerData;

		public BufferLookup<SubRoute> m_SubRoutes;

		public void Execute()
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
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			Owner owner = default(Owner);
			DynamicBuffer<SubRoute> val3 = default(DynamicBuffer<SubRoute>);
			Owner owner4 = default(Owner);
			DynamicBuffer<SubRoute> val7 = default(DynamicBuffer<SubRoute>);
			for (int i = 0; i < m_RouteChunks.Length; i++)
			{
				ArchetypeChunk val = m_RouteChunks[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				BufferAccessor<RouteWaypoint> bufferAccessor = ((ArchetypeChunk)(ref val)).GetBufferAccessor<RouteWaypoint>(ref m_RouteWaypointType);
				BufferAccessor<RouteSegment> bufferAccessor2 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<RouteSegment>(ref m_RouteSegmentType);
				if (((ArchetypeChunk)(ref val)).Has<Deleted>(ref m_DeletedType))
				{
					for (int j = 0; j < nativeArray.Length; j++)
					{
						Entity val2 = nativeArray[j];
						if (m_OwnerData.TryGetComponent(val2, ref owner) && m_SubRoutes.TryGetBuffer(owner.m_Owner, ref val3))
						{
							CollectionUtils.RemoveValue<SubRoute>(val3, new SubRoute(val2));
						}
					}
					continue;
				}
				bool flag = ((ArchetypeChunk)(ref val)).Has<Temp>(ref m_TempType);
				for (int k = 0; k < nativeArray.Length; k++)
				{
					Entity val4 = nativeArray[k];
					DynamicBuffer<RouteWaypoint> val5 = bufferAccessor[k];
					DynamicBuffer<RouteSegment> val6 = bufferAccessor2[k];
					for (int l = 0; l < val5.Length; l++)
					{
						Entity waypoint = val5[l].m_Waypoint;
						Owner owner2 = m_OwnerData[waypoint];
						owner2.m_Owner = val4;
						m_OwnerData[waypoint] = owner2;
					}
					for (int m = 0; m < val6.Length; m++)
					{
						Entity segment = val6[m].m_Segment;
						if (segment != Entity.Null)
						{
							Owner owner3 = m_OwnerData[segment];
							owner3.m_Owner = val4;
							m_OwnerData[segment] = owner3;
						}
					}
					if (m_OwnerData.TryGetComponent(val4, ref owner4) && m_TempData.HasComponent(owner4.m_Owner) == flag && m_SubRoutes.TryGetBuffer(owner4.m_Owner, ref val7))
					{
						CollectionUtils.TryAddUniqueValue<SubRoute>(val7, new SubRoute(val4));
					}
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public BufferTypeHandle<RouteWaypoint> __Game_Routes_RouteWaypoint_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<RouteSegment> __Game_Routes_RouteSegment_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> __Game_Common_Deleted_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		public ComponentLookup<Owner> __Game_Common_Owner_RW_ComponentLookup;

		public BufferLookup<SubRoute> __Game_Routes_SubRoute_RW_BufferLookup;

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
			__Game_Routes_RouteWaypoint_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<RouteWaypoint>(true);
			__Game_Routes_RouteSegment_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<RouteSegment>(true);
			__Game_Common_Deleted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Deleted>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Common_Owner_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(false);
			__Game_Routes_SubRoute_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubRoute>(false);
		}
	}

	private EntityQuery m_RouteQuery;

	private TypeHandle __TypeHandle;

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
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Route>(),
			ComponentType.ReadOnly<RouteWaypoint>(),
			ComponentType.ReadOnly<RouteSegment>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array[0] = val;
		m_RouteQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		((ComponentSystemBase)this).RequireForUpdate(m_RouteQuery);
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
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		NativeList<ArchetypeChunk> routeChunks = ((EntityQuery)(ref m_RouteQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		JobHandle val2 = IJobExtensions.Schedule<UpdateRouteReferencesJob>(new UpdateRouteReferencesJob
		{
			m_RouteChunks = routeChunks,
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RouteWaypointType = InternalCompilerInterface.GetBufferTypeHandle<RouteWaypoint>(ref __TypeHandle.__Game_Routes_RouteWaypoint_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RouteSegmentType = InternalCompilerInterface.GetBufferTypeHandle<RouteSegment>(ref __TypeHandle.__Game_Routes_RouteSegment_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DeletedType = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubRoutes = InternalCompilerInterface.GetBufferLookup<SubRoute>(ref __TypeHandle.__Game_Routes_SubRoute_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		}, JobHandle.CombineDependencies(val, ((SystemBase)this).Dependency));
		routeChunks.Dispose(val2);
		((SystemBase)this).Dependency = val2;
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
	public ReferencesSystem()
	{
	}
}
