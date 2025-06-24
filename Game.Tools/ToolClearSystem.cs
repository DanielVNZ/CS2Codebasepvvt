using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Buildings;
using Game.Common;
using Game.Creatures;
using Game.Net;
using Game.Objects;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class ToolClearSystem : GameSystemBase
{
	[BurstCompile]
	private struct ClearEntitiesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Lane> m_LaneType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.Object> m_ObjectType;

		[ReadOnly]
		public ComponentTypeHandle<Vehicle> m_VehicleType;

		[ReadOnly]
		public ComponentTypeHandle<Creature> m_CreatureType;

		[ReadOnly]
		public ComponentTypeHandle<Building> m_BuildingType;

		[ReadOnly]
		public ComponentTypeHandle<ServiceUpgrade> m_ServiceUpgradeType;

		[ReadOnly]
		public BufferTypeHandle<AggregateElement> m_AggregateElementType;

		[ReadOnly]
		public ComponentLookup<Hidden> m_HiddenData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		public ParallelWriter m_CommandBuffer;

		public unsafe void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
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
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			NativeArray<Owner> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			BufferAccessor<AggregateElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<AggregateElement>(ref m_AggregateElementType);
			int count = ((ArchetypeChunk)(ref chunk)).Count;
			StackList<Entity> val = StackList<Entity>.op_Implicit(new Span<Entity>((void*)stackalloc Entity[count], count));
			if (nativeArray3.Length != 0 && IsHandledAsSubObject(chunk))
			{
				count = ((ArchetypeChunk)(ref chunk)).Count;
				StackList<Entity> val2 = StackList<Entity>.op_Implicit(new Span<Entity>((void*)stackalloc Entity[count], count));
				for (int i = 0; i < nativeArray3.Length; i++)
				{
					Owner owner = nativeArray3[i];
					Temp temp = nativeArray2[i];
					if ((temp.m_Flags & TempFlags.Essential) != 0 || !m_TempData.HasComponent(owner.m_Owner))
					{
						if (temp.m_Original != Entity.Null && m_HiddenData.HasComponent(temp.m_Original))
						{
							val.AddNoResize(temp.m_Original);
						}
						val2.AddNoResize(nativeArray[i]);
					}
				}
				if (val2.Length != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, val2.AsArray());
				}
			}
			else
			{
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					Temp temp2 = nativeArray2[j];
					if (temp2.m_Original != Entity.Null && m_HiddenData.HasComponent(temp2.m_Original))
					{
						val.AddNoResize(temp2.m_Original);
					}
				}
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, nativeArray);
			}
			if (val.Length != 0)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(unfilteredChunkIndex, val.AsArray());
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Hidden>(unfilteredChunkIndex, val.AsArray());
			}
			for (int k = 0; k < bufferAccessor.Length; k++)
			{
				NativeArray<Entity> val3 = bufferAccessor[k].AsNativeArray().Reinterpret<Entity>();
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(unfilteredChunkIndex, val3);
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Highlighted>(unfilteredChunkIndex, val3);
			}
		}

		private bool IsHandledAsSubObject(ArchetypeChunk chunk)
		{
			if (((ArchetypeChunk)(ref chunk)).Has<Lane>(ref m_LaneType))
			{
				return true;
			}
			if (((ArchetypeChunk)(ref chunk)).Has<Game.Objects.Object>(ref m_ObjectType))
			{
				if (((ArchetypeChunk)(ref chunk)).Has<Vehicle>(ref m_VehicleType))
				{
					return false;
				}
				if (((ArchetypeChunk)(ref chunk)).Has<Creature>(ref m_CreatureType))
				{
					return false;
				}
				if (((ArchetypeChunk)(ref chunk)).Has<Building>(ref m_BuildingType))
				{
					return false;
				}
				if (((ArchetypeChunk)(ref chunk)).Has<ServiceUpgrade>(ref m_ServiceUpgradeType))
				{
					return false;
				}
				return true;
			}
			return false;
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
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Lane> __Game_Net_Lane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.Object> __Game_Objects_Object_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Vehicle> __Game_Vehicles_Vehicle_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Creature> __Game_Creatures_Creature_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Building> __Game_Buildings_Building_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ServiceUpgrade> __Game_Buildings_ServiceUpgrade_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<AggregateElement> __Game_Net_AggregateElement_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Hidden> __Game_Tools_Hidden_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

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
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Net_Lane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Lane>(true);
			__Game_Objects_Object_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Objects.Object>(true);
			__Game_Vehicles_Vehicle_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Vehicle>(true);
			__Game_Creatures_Creature_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Creature>(true);
			__Game_Buildings_Building_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Building>(true);
			__Game_Buildings_ServiceUpgrade_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ServiceUpgrade>(true);
			__Game_Net_AggregateElement_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<AggregateElement>(true);
			__Game_Tools_Hidden_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Hidden>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
		}
	}

	private ToolOutputBarrier m_ToolOutputBarrier;

	private EntityQuery m_ClearQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolOutputBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolOutputBarrier>();
		m_ClearQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() });
		((ComponentSystemBase)this).RequireForUpdate(m_ClearQuery);
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
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		ClearEntitiesJob clearEntitiesJob = new ClearEntitiesJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LaneType = InternalCompilerInterface.GetComponentTypeHandle<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectType = InternalCompilerInterface.GetComponentTypeHandle<Game.Objects.Object>(ref __TypeHandle.__Game_Objects_Object_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_VehicleType = InternalCompilerInterface.GetComponentTypeHandle<Vehicle>(ref __TypeHandle.__Game_Vehicles_Vehicle_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CreatureType = InternalCompilerInterface.GetComponentTypeHandle<Creature>(ref __TypeHandle.__Game_Creatures_Creature_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingType = InternalCompilerInterface.GetComponentTypeHandle<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceUpgradeType = InternalCompilerInterface.GetComponentTypeHandle<ServiceUpgrade>(ref __TypeHandle.__Game_Buildings_ServiceUpgrade_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AggregateElementType = InternalCompilerInterface.GetBufferTypeHandle<AggregateElement>(ref __TypeHandle.__Game_Net_AggregateElement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HiddenData = InternalCompilerInterface.GetComponentLookup<Hidden>(ref __TypeHandle.__Game_Tools_Hidden_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		EntityCommandBuffer val = m_ToolOutputBarrier.CreateCommandBuffer();
		clearEntitiesJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<ClearEntitiesJob>(clearEntitiesJob, m_ClearQuery, ((SystemBase)this).Dependency);
		((EntityCommandBufferSystem)m_ToolOutputBarrier).AddJobHandleForProducer(val2);
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
	public ToolClearSystem()
	{
	}
}
