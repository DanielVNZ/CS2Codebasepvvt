using System.Runtime.CompilerServices;
using Game.Common;
using Game.Simulation;
using Game.Vehicles;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Objects;

[CompilerGenerated]
public class DamageSystem : GameSystemBase
{
	[BurstCompile]
	private struct DamageObjectsJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public ComponentTypeHandle<Damage> m_DamageType;

		[ReadOnly]
		public ComponentLookup<Vehicle> m_VehicleData;

		public ComponentLookup<Damaged> m_DamagedData;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			NativeParallelHashMap<Entity, Damaged> val = default(NativeParallelHashMap<Entity, Damaged>);
			val._002Ector(10, AllocatorHandle.op_Implicit((Allocator)2));
			Damaged damaged = default(Damaged);
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				ArchetypeChunk val2 = m_Chunks[i];
				NativeArray<Damage> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray<Damage>(ref m_DamageType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Damage damage = nativeArray[j];
					if (val.TryGetValue(damage.m_Object, ref damaged))
					{
						ref float3 damage2 = ref damaged.m_Damage;
						damage2 += damage.m_Delta;
						val[damage.m_Object] = damaged;
					}
					else if (m_DamagedData.HasComponent(damage.m_Object))
					{
						damaged = m_DamagedData[damage.m_Object];
						ref float3 damage3 = ref damaged.m_Damage;
						damage3 += damage.m_Delta;
						val.TryAdd(damage.m_Object, damaged);
					}
					else
					{
						val.TryAdd(damage.m_Object, new Damaged(damage.m_Delta));
					}
				}
			}
			if (val.Count() == 0)
			{
				return;
			}
			NativeArray<Entity> keyArray = val.GetKeyArray(AllocatorHandle.op_Implicit((Allocator)2));
			for (int k = 0; k < keyArray.Length; k++)
			{
				Entity val3 = keyArray[k];
				Damaged damaged2 = val[val3];
				if (m_DamagedData.HasComponent(val3))
				{
					if (math.any(damaged2.m_Damage > 0f))
					{
						m_DamagedData[val3] = damaged2;
						continue;
					}
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(val3, default(BatchesUpdated));
					((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Damaged>(val3);
					if (m_VehicleData.HasComponent(val3))
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<MaintenanceConsumer>(val3);
					}
				}
				else if (math.any(damaged2.m_Damage > 0f))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Damaged>(val3, damaged2);
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(val3, default(BatchesUpdated));
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<Damage> __Game_Objects_Damage_RO_ComponentTypeHandle;

		public ComponentLookup<Vehicle> __Game_Vehicles_Vehicle_RW_ComponentLookup;

		public ComponentLookup<Damaged> __Game_Objects_Damaged_RW_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			__Game_Objects_Damage_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Damage>(true);
			__Game_Vehicles_Vehicle_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Vehicle>(false);
			__Game_Objects_Damaged_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Damaged>(false);
		}
	}

	private ModificationBarrier2 m_ModificationBarrier;

	private EntityQuery m_EventQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier2>();
		m_EventQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Event>(),
			ComponentType.ReadOnly<Damage>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_EventQuery);
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
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		NativeList<ArchetypeChunk> chunks = ((EntityQuery)(ref m_EventQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		JobHandle val2 = IJobExtensions.Schedule<DamageObjectsJob>(new DamageObjectsJob
		{
			m_Chunks = chunks,
			m_DamageType = InternalCompilerInterface.GetComponentTypeHandle<Damage>(ref __TypeHandle.__Game_Objects_Damage_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_VehicleData = InternalCompilerInterface.GetComponentLookup<Vehicle>(ref __TypeHandle.__Game_Vehicles_Vehicle_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DamagedData = InternalCompilerInterface.GetComponentLookup<Damaged>(ref __TypeHandle.__Game_Objects_Damaged_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer()
		}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val));
		chunks.Dispose(val2);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val2);
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
	public DamageSystem()
	{
	}
}
