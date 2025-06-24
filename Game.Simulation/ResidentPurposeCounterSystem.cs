using System.Runtime.CompilerServices;
using Game.Agents;
using Game.Citizens;
using Game.Common;
using Game.Creatures;
using Game.Debug;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Reflection;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Simulation;

[DebugWatchOnly]
[CompilerGenerated]
public class ResidentPurposeCounterSystem : GameSystemBase
{
	public enum CountPurpose
	{
		GoingHome,
		GoingToSchool,
		GoingToWork,
		Leisure,
		MovingAway,
		Shopping,
		Travel,
		None,
		Other,
		TouristLeaving,
		Mail,
		MovingIn,
		Count
	}

	[BurstCompile]
	private struct PurposeCountJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Creatures.Resident> m_ResidentType;

		[ReadOnly]
		public BufferTypeHandle<PathElement> m_PathElementType;

		[ReadOnly]
		public ComponentTypeHandle<Divert> m_DivertType;

		[ReadOnly]
		public ComponentLookup<TravelPurpose> m_TravelPurposeData;

		[ReadOnly]
		public ComponentLookup<CurrentBuilding> m_CurrentBuildings;

		[ReadOnly]
		public ComponentLookup<HouseholdMember> m_HouseholdMembers;

		[ReadOnly]
		public ComponentLookup<MovingAway> m_MovingAways;

		[ReadOnly]
		public ComponentLookup<Household> m_Households;

		[ReadOnly]
		public ComponentLookup<TouristHousehold> m_TouristHouseholds;

		public NativeArray<int> m_Results;

		public void Execute()
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_Results.Length; i++)
			{
				m_Results[i] = 0;
			}
			for (int j = 0; j < m_Chunks.Length; j++)
			{
				ArchetypeChunk val = m_Chunks[j];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				NativeArray<Game.Creatures.Resident> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<Game.Creatures.Resident>(ref m_ResidentType);
				BufferAccessor<PathElement> bufferAccessor = ((ArchetypeChunk)(ref val)).GetBufferAccessor<PathElement>(ref m_PathElementType);
				NativeArray<Divert> nativeArray3 = ((ArchetypeChunk)(ref val)).GetNativeArray<Divert>(ref m_DivertType);
				for (int k = 0; k < nativeArray.Length; k++)
				{
					if (bufferAccessor[k].Length == 0)
					{
						continue;
					}
					Entity citizen = nativeArray2[k].m_Citizen;
					Entity household = m_HouseholdMembers[citizen].m_Household;
					if (m_MovingAways.HasComponent(household))
					{
						if (m_TouristHouseholds.HasComponent(household))
						{
							ref NativeArray<int> reference = ref m_Results;
							int num = reference[9];
							reference[9] = num + 1;
						}
						else
						{
							ref NativeArray<int> reference2 = ref m_Results;
							int num = reference2[4];
							reference2[4] = num + 1;
						}
					}
					else if (m_Households.HasComponent(household) && m_TravelPurposeData.HasComponent(citizen) && !m_CurrentBuildings.HasComponent(citizen))
					{
						if ((m_Households[household].m_Flags & HouseholdFlags.MovedIn) == 0)
						{
							ref NativeArray<int> reference3 = ref m_Results;
							int num = reference3[11];
							reference3[11] = num + 1;
						}
						Purpose purpose = m_TravelPurposeData[citizen].m_Purpose;
						if (nativeArray3.IsCreated)
						{
							purpose = nativeArray3[k].m_Purpose;
						}
						switch (purpose)
						{
						case Purpose.GoingHome:
						{
							ref NativeArray<int> reference11 = ref m_Results;
							int num = reference11[0];
							reference11[0] = num + 1;
							break;
						}
						case Purpose.GoingToSchool:
						{
							ref NativeArray<int> reference10 = ref m_Results;
							int num = reference10[1];
							reference10[1] = num + 1;
							break;
						}
						case Purpose.GoingToWork:
						{
							ref NativeArray<int> reference9 = ref m_Results;
							int num = reference9[2];
							reference9[2] = num + 1;
							break;
						}
						case Purpose.Leisure:
						{
							ref NativeArray<int> reference8 = ref m_Results;
							int num = reference8[3];
							reference8[3] = num + 1;
							break;
						}
						case Purpose.Shopping:
						{
							ref NativeArray<int> reference7 = ref m_Results;
							int num = reference7[5];
							reference7[5] = num + 1;
							break;
						}
						case Purpose.Traveling:
						{
							ref NativeArray<int> reference6 = ref m_Results;
							int num = reference6[6];
							reference6[6] = num + 1;
							break;
						}
						case Purpose.SendMail:
						{
							ref NativeArray<int> reference5 = ref m_Results;
							int num = reference5[10];
							reference5[10] = num + 1;
							break;
						}
						default:
						{
							ref NativeArray<int> reference4 = ref m_Results;
							int num = reference4[8];
							reference4[8] = num + 1;
							break;
						}
						}
					}
					else
					{
						ref NativeArray<int> reference12 = ref m_Results;
						int num = reference12[7];
						reference12[7] = num + 1;
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
		public ComponentTypeHandle<Game.Creatures.Resident> __Game_Creatures_Resident_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<PathElement> __Game_Pathfind_PathElement_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Divert> __Game_Creatures_Divert_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<TravelPurpose> __Game_Citizens_TravelPurpose_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentBuilding> __Game_Citizens_CurrentBuilding_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HouseholdMember> __Game_Citizens_HouseholdMember_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MovingAway> __Game_Agents_MovingAway_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Household> __Game_Citizens_Household_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TouristHousehold> __Game_Citizens_TouristHousehold_RO_ComponentLookup;

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
			__Game_Creatures_Resident_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Creatures.Resident>(true);
			__Game_Pathfind_PathElement_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<PathElement>(true);
			__Game_Creatures_Divert_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Divert>(true);
			__Game_Citizens_TravelPurpose_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TravelPurpose>(true);
			__Game_Citizens_CurrentBuilding_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentBuilding>(true);
			__Game_Citizens_HouseholdMember_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HouseholdMember>(true);
			__Game_Agents_MovingAway_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MovingAway>(true);
			__Game_Citizens_Household_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Household>(true);
			__Game_Citizens_TouristHousehold_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TouristHousehold>(true);
		}
	}

	private EntityQuery m_CreatureQuery;

	[EnumArray(typeof(CountPurpose))]
	[DebugWatchValue(historyLength = 1024)]
	private NativeArray<int> m_Results;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 256;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CreatureQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[10]
		{
			ComponentType.ReadOnly<Game.Creatures.Resident>(),
			ComponentType.ReadOnly<Human>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.ReadOnly<PathOwner>(),
			ComponentType.ReadOnly<Target>(),
			ComponentType.Exclude<Unspawned>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Stumbling>()
		});
		m_Results = new NativeArray<int>(12, (Allocator)4, (NativeArrayOptions)1);
		((ComponentSystemBase)this).Enabled = false;
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_Results.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		PurposeCountJob purposeCountJob = new PurposeCountJob
		{
			m_Chunks = ((EntityQuery)(ref m_CreatureQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val),
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResidentType = InternalCompilerInterface.GetComponentTypeHandle<Game.Creatures.Resident>(ref __TypeHandle.__Game_Creatures_Resident_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathElementType = InternalCompilerInterface.GetBufferTypeHandle<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DivertType = InternalCompilerInterface.GetComponentTypeHandle<Divert>(ref __TypeHandle.__Game_Creatures_Divert_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TravelPurposeData = InternalCompilerInterface.GetComponentLookup<TravelPurpose>(ref __TypeHandle.__Game_Citizens_TravelPurpose_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentBuildings = InternalCompilerInterface.GetComponentLookup<CurrentBuilding>(ref __TypeHandle.__Game_Citizens_CurrentBuilding_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdMembers = InternalCompilerInterface.GetComponentLookup<HouseholdMember>(ref __TypeHandle.__Game_Citizens_HouseholdMember_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MovingAways = InternalCompilerInterface.GetComponentLookup<MovingAway>(ref __TypeHandle.__Game_Agents_MovingAway_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Households = InternalCompilerInterface.GetComponentLookup<Household>(ref __TypeHandle.__Game_Citizens_Household_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TouristHouseholds = InternalCompilerInterface.GetComponentLookup<TouristHousehold>(ref __TypeHandle.__Game_Citizens_TouristHousehold_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Results = m_Results
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<PurposeCountJob>(purposeCountJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val));
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
	public ResidentPurposeCounterSystem()
	{
	}
}
