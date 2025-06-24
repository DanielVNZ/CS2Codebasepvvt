using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Serialization.Entities;
using Game.Citizens;
using Game.Common;
using Game.Creatures;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Serialization.DataMigration;

[CompilerGenerated]
public class ResidentPseudoRandomSystem : GameSystemBase
{
	[BurstCompile]
	private struct ResidentPseudoRandomJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<Resident> m_ResidentType;

		public ComponentTypeHandle<PseudoRandomSeed> m_PseudoRandomSeedType;

		[ReadOnly]
		public ComponentLookup<Citizen> m_CitizenData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Resident> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Resident>(ref m_ResidentType);
			NativeArray<PseudoRandomSeed> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PseudoRandomSeed>(ref m_PseudoRandomSeedType);
			Citizen citizen = default(Citizen);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Resident resident = nativeArray[i];
				ref PseudoRandomSeed reference = ref CollectionUtils.ElementAt<PseudoRandomSeed>(nativeArray2, i);
				if (m_CitizenData.TryGetComponent(resident.m_Citizen, ref citizen))
				{
					Random random = citizen.GetPseudoRandom(CitizenPseudoRandom.SpawnResident);
					reference = new PseudoRandomSeed(ref random);
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
		public ComponentTypeHandle<Resident> __Game_Creatures_Resident_RO_ComponentTypeHandle;

		public ComponentTypeHandle<PseudoRandomSeed> __Game_Common_PseudoRandomSeed_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			__Game_Creatures_Resident_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Resident>(true);
			__Game_Common_PseudoRandomSeed_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PseudoRandomSeed>(false);
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
		}
	}

	private LoadGameSystem m_LoadGameSystem;

	private EntityQuery m_Query;

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
		base.OnCreate();
		m_LoadGameSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LoadGameSystem>();
		m_Query = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Resident>(),
			ComponentType.ReadWrite<PseudoRandomSeed>()
		});
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		Context context = m_LoadGameSystem.context;
		if (!(((Context)(ref context)).version >= Version.residentPseudoRandomFix) && !((EntityQuery)(ref m_Query)).IsEmptyIgnoreFilter)
		{
			ResidentPseudoRandomJob residentPseudoRandomJob = new ResidentPseudoRandomJob
			{
				m_ResidentType = InternalCompilerInterface.GetComponentTypeHandle<Resident>(ref __TypeHandle.__Game_Creatures_Resident_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PseudoRandomSeedType = InternalCompilerInterface.GetComponentTypeHandle<PseudoRandomSeed>(ref __TypeHandle.__Game_Common_PseudoRandomSeed_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CitizenData = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<ResidentPseudoRandomJob>(residentPseudoRandomJob, m_Query, ((SystemBase)this).Dependency);
		}
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
	public ResidentPseudoRandomSystem()
	{
	}
}
