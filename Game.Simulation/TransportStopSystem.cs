using System.Runtime.CompilerServices;
using Game.Buildings;
using Game.Common;
using Game.Prefabs;
using Game.Routes;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class TransportStopSystem : GameSystemBase
{
	[BurstCompile]
	private struct TransportStopTickJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<TaxiStand> m_TaxiStandType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<ConnectedRoute> m_ConnectedRouteType;

		public ComponentTypeHandle<Game.Routes.TransportStop> m_TransportStopType;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.TransportStation> m_TransportStationData;

		[ReadOnly]
		public ComponentLookup<TransportStopData> m_PrefabTransportStopData;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<Owner> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<Game.Routes.TransportStop> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Routes.TransportStop>(ref m_TransportStopType);
			BufferAccessor<ConnectedRoute> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ConnectedRoute>(ref m_ConnectedRouteType);
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<TaxiStand>(ref m_TaxiStandType);
			for (int i = 0; i < nativeArray4.Length; i++)
			{
				PrefabRef prefabRef = nativeArray2[i];
				Game.Routes.TransportStop transportStop = nativeArray4[i];
				TransportStopData transportStopData = default(TransportStopData);
				if (m_PrefabTransportStopData.HasComponent(prefabRef.m_Prefab))
				{
					transportStopData = m_PrefabTransportStopData[prefabRef.m_Prefab];
				}
				float num = math.saturate(transportStopData.m_ComfortFactor);
				float num2 = math.max(0f, 1f + transportStopData.m_LoadingFactor);
				bool flag2 = true;
				if (nativeArray3.Length != 0)
				{
					Entity transportStation = GetTransportStation(nativeArray3[i].m_Owner);
					if (transportStation != Entity.Null)
					{
						Game.Buildings.TransportStation transportStation2 = m_TransportStationData[transportStation];
						num = math.saturate(num + (1f - num) * transportStation2.m_ComfortFactor);
						num2 = math.max(0f, num2 * transportStation2.m_LoadingFactor);
						flag2 = (transportStation2.m_Flags & TransportStationFlags.TransportStopsActive) != 0;
					}
				}
				if (num != transportStop.m_ComfortFactor || num2 != transportStop.m_LoadingFactor || flag2 != ((transportStop.m_Flags & StopFlags.Active) != 0))
				{
					if (flag)
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(unfilteredChunkIndex, nativeArray[i], default(PathfindUpdated));
					}
					if (bufferAccessor.Length != 0)
					{
						DynamicBuffer<ConnectedRoute> val = bufferAccessor[i];
						for (int j = 0; j < val.Length; j++)
						{
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(unfilteredChunkIndex, val[j].m_Waypoint, default(PathfindUpdated));
						}
					}
				}
				transportStop.m_ComfortFactor = num;
				transportStop.m_LoadingFactor = num2;
				if (flag2)
				{
					transportStop.m_Flags |= StopFlags.Active;
				}
				else
				{
					transportStop.m_Flags &= ~StopFlags.Active;
				}
				nativeArray4[i] = transportStop;
			}
		}

		private Entity GetTransportStation(Entity owner)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			while (true)
			{
				if (m_TransportStationData.HasComponent(owner))
				{
					if (m_OwnerData.HasComponent(owner))
					{
						Entity owner2 = m_OwnerData[owner].m_Owner;
						if (m_TransportStationData.HasComponent(owner2))
						{
							return owner2;
						}
					}
					return owner;
				}
				if (!m_OwnerData.HasComponent(owner))
				{
					break;
				}
				owner = m_OwnerData[owner].m_Owner;
			}
			return Entity.Null;
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
		public ComponentTypeHandle<TaxiStand> __Game_Routes_TaxiStand_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<ConnectedRoute> __Game_Routes_ConnectedRoute_RO_BufferTypeHandle;

		public ComponentTypeHandle<Game.Routes.TransportStop> __Game_Routes_TransportStop_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.TransportStation> __Game_Buildings_TransportStation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TransportStopData> __Game_Prefabs_TransportStopData_RO_ComponentLookup;

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
			__Game_Routes_TaxiStand_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TaxiStand>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Routes_ConnectedRoute_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ConnectedRoute>(true);
			__Game_Routes_TransportStop_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Routes.TransportStop>(false);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Buildings_TransportStation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.TransportStation>(true);
			__Game_Prefabs_TransportStopData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TransportStopData>(true);
		}
	}

	private const uint UPDATE_INTERVAL = 256u;

	private EntityQuery m_StopQuery;

	private EndFrameBarrier m_EndFrameBarrier;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 256;
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
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_StopQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Routes.TransportStop>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_StopQuery);
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
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		TransportStopTickJob transportStopTickJob = new TransportStopTickJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TaxiStandType = InternalCompilerInterface.GetComponentTypeHandle<TaxiStand>(ref __TypeHandle.__Game_Routes_TaxiStand_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedRouteType = InternalCompilerInterface.GetBufferTypeHandle<ConnectedRoute>(ref __TypeHandle.__Game_Routes_ConnectedRoute_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransportStopType = InternalCompilerInterface.GetComponentTypeHandle<Game.Routes.TransportStop>(ref __TypeHandle.__Game_Routes_TransportStop_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransportStationData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.TransportStation>(ref __TypeHandle.__Game_Buildings_TransportStation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTransportStopData = InternalCompilerInterface.GetComponentLookup<TransportStopData>(ref __TypeHandle.__Game_Prefabs_TransportStopData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		transportStopTickJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<TransportStopTickJob>(transportStopTickJob, m_StopQuery, ((SystemBase)this).Dependency);
		m_EndFrameBarrier.AddJobHandleForProducer(val2);
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
	public TransportStopSystem()
	{
	}
}
