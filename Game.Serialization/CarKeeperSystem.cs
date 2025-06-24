using System.Runtime.CompilerServices;
using Colossal.Entities;
using Game.Citizens;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Serialization;

[CompilerGenerated]
public class CarKeeperSystem : GameSystemBase
{
	[BurstCompile]
	private struct CarKeeperJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PersonalCar> m_PersonalCarType;

		public ComponentLookup<CarKeeper> m_CarKeeperData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PersonalCar> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PersonalCar>(ref m_PersonalCarType);
			CarKeeper carKeeper = default(CarKeeper);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Entity car = nativeArray[i];
				PersonalCar personalCar = nativeArray2[i];
				if (EntitiesExtensions.TryGetEnabledComponent<CarKeeper>(m_CarKeeperData, personalCar.m_Keeper, ref carKeeper))
				{
					carKeeper.m_Car = car;
					m_CarKeeperData[personalCar.m_Keeper] = carKeeper;
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct NoCarJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		public ComponentTypeHandle<CarKeeper> m_CarKeeperType;

		[ReadOnly]
		public ComponentLookup<PersonalCar> m_PersonalCars;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<CarKeeper> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarKeeper>(ref m_CarKeeperType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				_ = nativeArray[i];
				if (((ArchetypeChunk)(ref chunk)).IsComponentEnabled<CarKeeper>(ref m_CarKeeperType, i))
				{
					CarKeeper carKeeper = nativeArray2[i];
					if (!m_PersonalCars.HasComponent(carKeeper.m_Car))
					{
						((ArchetypeChunk)(ref chunk)).SetComponentEnabled<CarKeeper>(ref m_CarKeeperType, i, false);
					}
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
		public ComponentTypeHandle<PersonalCar> __Game_Vehicles_PersonalCar_RO_ComponentTypeHandle;

		public ComponentLookup<CarKeeper> __Game_Citizens_CarKeeper_RW_ComponentLookup;

		public ComponentTypeHandle<CarKeeper> __Game_Citizens_CarKeeper_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<PersonalCar> __Game_Vehicles_PersonalCar_RO_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Vehicles_PersonalCar_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PersonalCar>(true);
			__Game_Citizens_CarKeeper_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarKeeper>(false);
			__Game_Citizens_CarKeeper_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CarKeeper>(false);
			__Game_Vehicles_PersonalCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PersonalCar>(true);
		}
	}

	private EntityQuery m_Query;

	private EntityQuery m_KeeperQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_Query = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PersonalCar>() });
		m_KeeperQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<CarKeeper>() });
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
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = JobChunkExtensions.Schedule<CarKeeperJob>(new CarKeeperJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PersonalCarType = InternalCompilerInterface.GetComponentTypeHandle<PersonalCar>(ref __TypeHandle.__Game_Vehicles_PersonalCar_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CarKeeperData = InternalCompilerInterface.GetComponentLookup<CarKeeper>(ref __TypeHandle.__Game_Citizens_CarKeeper_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
		}, m_Query, ((SystemBase)this).Dependency);
		NoCarJob noCarJob = new NoCarJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CarKeeperType = InternalCompilerInterface.GetComponentTypeHandle<CarKeeper>(ref __TypeHandle.__Game_Citizens_CarKeeper_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PersonalCars = InternalCompilerInterface.GetComponentLookup<PersonalCar>(ref __TypeHandle.__Game_Vehicles_PersonalCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		((SystemBase)this).Dependency = JobChunkExtensions.Schedule<NoCarJob>(noCarJob, m_KeeperQuery, val);
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
	public CarKeeperSystem()
	{
	}
}
