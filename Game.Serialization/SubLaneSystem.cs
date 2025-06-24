using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Common;
using Game.Net;
using Game.Pathfind;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine.Scripting;

namespace Game.Serialization;

[CompilerGenerated]
public class SubLaneSystem : GameSystemBase
{
	[BurstCompile]
	private struct SubLaneJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<PedestrianLane> m_PedestrianLaneType;

		[ReadOnly]
		public ComponentTypeHandle<CarLane> m_CarLaneType;

		[ReadOnly]
		public ComponentTypeHandle<TrackLane> m_TrackLaneType;

		[ReadOnly]
		public ComponentTypeHandle<ParkingLane> m_ParkingLaneType;

		[ReadOnly]
		public ComponentTypeHandle<ConnectionLane> m_ConnectionLaneType;

		public BufferLookup<SubLane> m_SubLanes;

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
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Owner> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<ParkingLane> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ParkingLane>(ref m_ParkingLaneType);
			NativeArray<ConnectionLane> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ConnectionLane>(ref m_ConnectionLaneType);
			PathMethod pathMethod = (PathMethod)0;
			if (((ArchetypeChunk)(ref chunk)).Has<PedestrianLane>(ref m_PedestrianLaneType))
			{
				pathMethod |= PathMethod.Pedestrian;
			}
			if (((ArchetypeChunk)(ref chunk)).Has<CarLane>(ref m_CarLaneType))
			{
				pathMethod |= PathMethod.Road;
			}
			if (((ArchetypeChunk)(ref chunk)).Has<TrackLane>(ref m_TrackLaneType))
			{
				pathMethod |= PathMethod.Track;
			}
			DynamicBuffer<SubLane> val = default(DynamicBuffer<SubLane>);
			ParkingLane parkingLane = default(ParkingLane);
			ConnectionLane connectionLane = default(ConnectionLane);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Entity lane = nativeArray[i];
				Owner owner = nativeArray2[i];
				if (!m_SubLanes.TryGetBuffer(owner.m_Owner, ref val))
				{
					continue;
				}
				PathMethod pathMethod2 = pathMethod;
				if (CollectionUtils.TryGet<ParkingLane>(nativeArray3, i, ref parkingLane))
				{
					pathMethod2 = (((parkingLane.m_Flags & ParkingLaneFlags.SpecialVehicles) == 0) ? (pathMethod2 | (PathMethod.Parking | PathMethod.Boarding)) : (pathMethod2 | (PathMethod.Boarding | PathMethod.SpecialParking)));
				}
				if (CollectionUtils.TryGet<ConnectionLane>(nativeArray4, i, ref connectionLane))
				{
					if ((connectionLane.m_Flags & ConnectionLaneFlags.Pedestrian) != 0)
					{
						pathMethod2 |= PathMethod.Pedestrian;
					}
					if ((connectionLane.m_Flags & ConnectionLaneFlags.Road) != 0)
					{
						pathMethod2 |= PathMethod.Road;
					}
					if ((connectionLane.m_Flags & ConnectionLaneFlags.Track) != 0)
					{
						pathMethod2 |= PathMethod.Track;
					}
					if ((connectionLane.m_Flags & ConnectionLaneFlags.Parking) != 0)
					{
						pathMethod2 |= PathMethod.Parking | PathMethod.Boarding;
					}
				}
				val.Add(new SubLane(lane, pathMethod2));
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
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PedestrianLane> __Game_Net_PedestrianLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CarLane> __Game_Net_CarLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TrackLane> __Game_Net_TrackLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ParkingLane> __Game_Net_ParkingLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ConnectionLane> __Game_Net_ConnectionLane_RO_ComponentTypeHandle;

		public BufferLookup<SubLane> __Game_Net_SubLane_RW_BufferLookup;

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
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Net_PedestrianLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PedestrianLane>(true);
			__Game_Net_CarLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CarLane>(true);
			__Game_Net_TrackLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TrackLane>(true);
			__Game_Net_ParkingLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ParkingLane>(true);
			__Game_Net_ConnectionLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ConnectionLane>(true);
			__Game_Net_SubLane_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubLane>(false);
		}
	}

	private EntityQuery m_Query;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_Query = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Lane>(),
			ComponentType.ReadOnly<Owner>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_Query);
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
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		SubLaneJob subLaneJob = new SubLaneJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PedestrianLaneType = InternalCompilerInterface.GetComponentTypeHandle<PedestrianLane>(ref __TypeHandle.__Game_Net_PedestrianLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CarLaneType = InternalCompilerInterface.GetComponentTypeHandle<CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TrackLaneType = InternalCompilerInterface.GetComponentTypeHandle<TrackLane>(ref __TypeHandle.__Game_Net_TrackLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ParkingLaneType = InternalCompilerInterface.GetComponentTypeHandle<ParkingLane>(ref __TypeHandle.__Game_Net_ParkingLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectionLaneType = InternalCompilerInterface.GetComponentTypeHandle<ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubLanes = InternalCompilerInterface.GetBufferLookup<SubLane>(ref __TypeHandle.__Game_Net_SubLane_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		((SystemBase)this).Dependency = JobChunkExtensions.Schedule<SubLaneJob>(subLaneJob, m_Query, ((SystemBase)this).Dependency);
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
	public SubLaneSystem()
	{
	}
}
