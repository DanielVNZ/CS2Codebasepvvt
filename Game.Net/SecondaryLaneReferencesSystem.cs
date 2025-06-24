using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Common;
using Game.Pathfind;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine.Scripting;

namespace Game.Net;

[CompilerGenerated]
public class SecondaryLaneReferencesSystem : GameSystemBase
{
	[BurstCompile]
	private struct UpdateLaneReferencesJob : IJobChunk
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

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedType;

		public BufferLookup<SubLane> m_Lanes;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Owner> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			if (((ArchetypeChunk)(ref chunk)).Has<Deleted>(ref m_DeletedType))
			{
				DynamicBuffer<SubLane> val = default(DynamicBuffer<SubLane>);
				for (int i = 0; i < nativeArray.Length; i++)
				{
					if (m_Lanes.TryGetBuffer(nativeArray2[i].m_Owner, ref val))
					{
						CollectionUtils.RemoveValue<SubLane>(val, new SubLane(nativeArray[i], (PathMethod)0));
					}
				}
				return;
			}
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
			ParkingLane parkingLane = default(ParkingLane);
			ConnectionLane connectionLane = default(ConnectionLane);
			for (int j = 0; j < nativeArray.Length; j++)
			{
				DynamicBuffer<SubLane> val2 = m_Lanes[nativeArray2[j].m_Owner];
				PathMethod pathMethod2 = pathMethod;
				if (CollectionUtils.TryGet<ParkingLane>(nativeArray3, j, ref parkingLane))
				{
					pathMethod2 = (((parkingLane.m_Flags & ParkingLaneFlags.SpecialVehicles) == 0) ? (pathMethod2 | (PathMethod.Parking | PathMethod.Boarding)) : (pathMethod2 | (PathMethod.Boarding | PathMethod.SpecialParking)));
				}
				if (CollectionUtils.TryGet<ConnectionLane>(nativeArray4, j, ref connectionLane))
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
				CollectionUtils.TryAddUniqueValue<SubLane>(val2, new SubLane(nativeArray[j], pathMethod2));
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

		[ReadOnly]
		public ComponentTypeHandle<Deleted> __Game_Common_Deleted_RO_ComponentTypeHandle;

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
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Net_PedestrianLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PedestrianLane>(true);
			__Game_Net_CarLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CarLane>(true);
			__Game_Net_TrackLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TrackLane>(true);
			__Game_Net_ParkingLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ParkingLane>(true);
			__Game_Net_ConnectionLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ConnectionLane>(true);
			__Game_Common_Deleted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Deleted>(true);
			__Game_Net_SubLane_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubLane>(false);
		}
	}

	private EntityQuery m_LanesQuery;

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
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Lane>(),
			ComponentType.ReadOnly<SecondaryLane>(),
			ComponentType.ReadOnly<Owner>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Deleted>()
		};
		val.None = (ComponentType[])(object)new ComponentType[0];
		array[0] = val;
		m_LanesQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		((ComponentSystemBase)this).RequireForUpdate(m_LanesQuery);
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
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		UpdateLaneReferencesJob updateLaneReferencesJob = new UpdateLaneReferencesJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PedestrianLaneType = InternalCompilerInterface.GetComponentTypeHandle<PedestrianLane>(ref __TypeHandle.__Game_Net_PedestrianLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CarLaneType = InternalCompilerInterface.GetComponentTypeHandle<CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TrackLaneType = InternalCompilerInterface.GetComponentTypeHandle<TrackLane>(ref __TypeHandle.__Game_Net_TrackLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ParkingLaneType = InternalCompilerInterface.GetComponentTypeHandle<ParkingLane>(ref __TypeHandle.__Game_Net_ParkingLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectionLaneType = InternalCompilerInterface.GetComponentTypeHandle<ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DeletedType = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Lanes = InternalCompilerInterface.GetBufferLookup<SubLane>(ref __TypeHandle.__Game_Net_SubLane_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		((SystemBase)this).Dependency = JobChunkExtensions.Schedule<UpdateLaneReferencesJob>(updateLaneReferencesJob, m_LanesQuery, ((SystemBase)this).Dependency);
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
	public SecondaryLaneReferencesSystem()
	{
	}
}
