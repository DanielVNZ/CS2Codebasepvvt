using System.Runtime.CompilerServices;
using Game.Citizens;
using Game.Common;
using Game.Objects;
using Game.Pathfind;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine.Scripting;

namespace Game.Creatures;

[CompilerGenerated]
public class TripResetSystem : GameSystemBase
{
	[BurstCompile]
	private struct CreatureTripResetJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<ResetTrip> m_ResetTripType;

		[ReadOnly]
		public ComponentLookup<Deleted> m_Deleted;

		[ReadOnly]
		public ComponentLookup<TravelPurpose> m_TravelPurpose;

		[ReadOnly]
		public BufferLookup<TripNeeded> m_TripNeeded;

		[ReadOnly]
		public BufferLookup<GroupCreature> m_GroupCreatures;

		public ComponentLookup<HumanCurrentLane> m_HumanCurrentLane;

		public ComponentLookup<AnimalCurrentLane> m_AnimalCurrentLane;

		public ComponentLookup<Resident> m_Resident;

		public ComponentLookup<Pet> m_Pet;

		public ComponentLookup<Target> m_Target;

		public ComponentLookup<Divert> m_Divert;

		public ComponentLookup<PathOwner> m_PathOwner;

		public BufferLookup<PathElement> m_PathElements;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_047e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0728: Unknown result type (might be due to invalid IL or missing references)
			//IL_072d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0491: Unknown result type (might be due to invalid IL or missing references)
			//IL_0496: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0742: Unknown result type (might be due to invalid IL or missing references)
			//IL_0749: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0578: Unknown result type (might be due to invalid IL or missing references)
			//IL_057d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_064e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0653: Unknown result type (might be due to invalid IL or missing references)
			//IL_0594: Unknown result type (might be due to invalid IL or missing references)
			//IL_050a: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04df: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_066a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0634: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_0681: Unknown result type (might be due to invalid IL or missing references)
			//IL_0686: Unknown result type (might be due to invalid IL or missing references)
			//IL_068b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0696: Unknown result type (might be due to invalid IL or missing references)
			//IL_069b: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0603: Unknown result type (might be due to invalid IL or missing references)
			//IL_0460: Unknown result type (might be due to invalid IL or missing references)
			//IL_0467: Unknown result type (might be due to invalid IL or missing references)
			//IL_0526: Unknown result type (might be due to invalid IL or missing references)
			//IL_052b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0533: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_0550: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_044c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<ResetTrip> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ResetTrip>(ref m_ResetTripType);
			HumanCurrentLane humanCurrentLane = default(HumanCurrentLane);
			AnimalCurrentLane animalCurrentLane = default(AnimalCurrentLane);
			Target target = default(Target);
			Divert divert = default(Divert);
			PathOwner pathOwner = default(PathOwner);
			DynamicBuffer<PathElement> sourceElements = default(DynamicBuffer<PathElement>);
			DynamicBuffer<PathElement> targetElements = default(DynamicBuffer<PathElement>);
			Resident resident = default(Resident);
			Pet pet = default(Pet);
			PathOwner pathOwner2 = default(PathOwner);
			DynamicBuffer<PathElement> sourceElements2 = default(DynamicBuffer<PathElement>);
			DynamicBuffer<PathElement> targetElements2 = default(DynamicBuffer<PathElement>);
			Resident resident2 = default(Resident);
			DynamicBuffer<GroupCreature> val2 = default(DynamicBuffer<GroupCreature>);
			Pet pet2 = default(Pet);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Entity val = nativeArray[i];
				ResetTrip resetTrip = nativeArray2[i];
				if (m_Deleted.HasComponent(resetTrip.m_Creature))
				{
					continue;
				}
				if (m_HumanCurrentLane.TryGetComponent(resetTrip.m_Creature, ref humanCurrentLane))
				{
					humanCurrentLane.m_Flags &= ~CreatureLaneFlags.EndOfPath;
					m_HumanCurrentLane[resetTrip.m_Creature] = humanCurrentLane;
				}
				if (m_AnimalCurrentLane.TryGetComponent(resetTrip.m_Creature, ref animalCurrentLane))
				{
					animalCurrentLane.m_Flags &= ~CreatureLaneFlags.EndOfPath;
					m_AnimalCurrentLane[resetTrip.m_Creature] = animalCurrentLane;
				}
				if (m_Target.TryGetComponent(resetTrip.m_Creature, ref target))
				{
					bool flag = false;
					bool flag2 = false;
					if (resetTrip.m_DivertPurpose != Purpose.None)
					{
						if (m_Divert.TryGetComponent(resetTrip.m_Creature, ref divert))
						{
							if (divert.m_Purpose != resetTrip.m_DivertPurpose || divert.m_Target != resetTrip.m_DivertTarget)
							{
								divert.m_Purpose = resetTrip.m_DivertPurpose;
								divert.m_Target = resetTrip.m_DivertTarget;
								divert.m_Data = resetTrip.m_DivertData;
								divert.m_Resource = resetTrip.m_DivertResource;
								m_Divert[resetTrip.m_Creature] = divert;
								flag = true;
							}
							else
							{
								flag2 = true;
							}
						}
						else
						{
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Divert>(unfilteredChunkIndex, resetTrip.m_Creature, new Divert
							{
								m_Target = resetTrip.m_DivertTarget,
								m_Purpose = resetTrip.m_DivertPurpose,
								m_Data = resetTrip.m_DivertData,
								m_Resource = resetTrip.m_DivertResource
							});
							flag = true;
						}
						if (flag && m_PathOwner.TryGetComponent(resetTrip.m_Creature, ref pathOwner))
						{
							pathOwner.m_State &= ~PathFlags.Failed;
							if (resetTrip.m_HasDivertPath && (pathOwner.m_State & PathFlags.Pending) == 0 && m_PathElements.TryGetBuffer(val, ref sourceElements) && m_PathElements.TryGetBuffer(resetTrip.m_Creature, ref targetElements))
							{
								PathUtils.CopyPath(sourceElements, default(PathOwner), 0, targetElements);
								pathOwner.m_ElementIndex = 0;
								pathOwner.m_State &= ~PathFlags.DivertObsolete;
								pathOwner.m_State |= PathFlags.Updated | PathFlags.CachedObsolete;
							}
							else
							{
								pathOwner.m_State |= PathFlags.DivertObsolete;
							}
							m_PathOwner[resetTrip.m_Creature] = pathOwner;
						}
					}
					else if (m_Divert.HasComponent(resetTrip.m_Creature))
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Divert>(unfilteredChunkIndex, resetTrip.m_Creature);
					}
					if (resetTrip.m_Target != target.m_Target)
					{
						if (m_Resident.TryGetComponent(resetTrip.m_Creature, ref resident))
						{
							resident.m_Flags &= ~(ResidentFlags.Arrived | ResidentFlags.Hangaround | ResidentFlags.PreferredLeader | ResidentFlags.IgnoreBenches | ResidentFlags.IgnoreAreas | ResidentFlags.CannotIgnore);
							resident.m_Flags |= resetTrip.m_ResidentFlags;
							resident.m_Timer = 0;
							m_Resident[resetTrip.m_Creature] = resident;
						}
						if (m_Pet.TryGetComponent(resetTrip.m_Creature, ref pet))
						{
							pet.m_Flags &= ~(PetFlags.Hangaround | PetFlags.Arrived | PetFlags.LeaderArrived);
							m_Pet[resetTrip.m_Creature] = pet;
						}
						if (m_PathOwner.TryGetComponent(resetTrip.m_Creature, ref pathOwner2))
						{
							pathOwner2.m_State &= ~PathFlags.Failed;
							if (!resetTrip.m_HasDivertPath && (pathOwner2.m_State & PathFlags.Pending) == 0 && m_PathElements.TryGetBuffer(val, ref sourceElements2) && m_PathElements.TryGetBuffer(resetTrip.m_Creature, ref targetElements2))
							{
								PathUtils.CopyPath(sourceElements2, default(PathOwner), 0, targetElements2);
								pathOwner2.m_ElementIndex = 0;
								if (flag || flag2)
								{
									pathOwner2.m_State &= ~PathFlags.CachedObsolete;
								}
								else
								{
									pathOwner2.m_State &= ~PathFlags.Obsolete;
								}
								pathOwner2.m_State |= PathFlags.Updated;
							}
							else if (flag || flag2)
							{
								pathOwner2.m_State |= PathFlags.CachedObsolete;
							}
							else
							{
								pathOwner2.m_State |= PathFlags.Obsolete;
							}
							m_PathOwner[resetTrip.m_Creature] = pathOwner2;
						}
						m_Target[resetTrip.m_Creature] = new Target(resetTrip.m_Target);
					}
				}
				if (m_Resident.TryGetComponent(resetTrip.m_Creature, ref resident2))
				{
					if (resetTrip.m_Arrived != Entity.Null && resident2.m_Citizen != Entity.Null)
					{
						if (m_TripNeeded.HasBuffer(resident2.m_Citizen))
						{
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<CurrentBuilding>(unfilteredChunkIndex, resident2.m_Citizen, new CurrentBuilding(resetTrip.m_Arrived));
							((ParallelWriter)(ref m_CommandBuffer)).SetComponentEnabled<Arrived>(unfilteredChunkIndex, resident2.m_Citizen, true);
						}
						if (m_GroupCreatures.TryGetBuffer(resetTrip.m_Creature, ref val2))
						{
							for (int j = 0; j < val2.Length; j++)
							{
								Entity creature = val2[j].m_Creature;
								if (m_Pet.TryGetComponent(creature, ref pet2))
								{
									pet2.m_Flags |= PetFlags.LeaderArrived;
									m_Pet[creature] = pet2;
								}
							}
						}
					}
					if (resetTrip.m_TravelPurpose != Purpose.None && resident2.m_Citizen != Entity.Null)
					{
						if (m_TravelPurpose.HasComponent(resident2.m_Citizen))
						{
							if (m_TravelPurpose[resident2.m_Citizen].m_Purpose != resetTrip.m_TravelPurpose)
							{
								TravelPurpose travelPurpose = new TravelPurpose
								{
									m_Purpose = resetTrip.m_TravelPurpose,
									m_Data = resetTrip.m_TravelData,
									m_Resource = resetTrip.m_TravelResource
								};
								((ParallelWriter)(ref m_CommandBuffer)).SetComponent<TravelPurpose>(unfilteredChunkIndex, resident2.m_Citizen, travelPurpose);
							}
						}
						else
						{
							TravelPurpose travelPurpose2 = new TravelPurpose
							{
								m_Purpose = resetTrip.m_TravelPurpose
							};
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<TravelPurpose>(unfilteredChunkIndex, resident2.m_Citizen, travelPurpose2);
						}
					}
					if (resetTrip.m_NextPurpose != Purpose.None && resident2.m_Citizen != Entity.Null && m_TripNeeded.HasBuffer(resident2.m_Citizen))
					{
						DynamicBuffer<TripNeeded> val3 = m_TripNeeded[resident2.m_Citizen];
						DynamicBuffer<TripNeeded> val4 = ((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<TripNeeded>(unfilteredChunkIndex, resident2.m_Citizen);
						val4.ResizeUninitialized(1 + val3.Length);
						val4[0] = new TripNeeded
						{
							m_Purpose = resetTrip.m_NextPurpose,
							m_TargetAgent = resetTrip.m_NextTarget,
							m_Data = resetTrip.m_NextData,
							m_Resource = resetTrip.m_NextResource
						};
						for (int k = 0; k < val3.Length; k++)
						{
							val4[k + 1] = val3[k];
						}
					}
				}
				if (resetTrip.m_Source != Entity.Null)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<TripSource>(unfilteredChunkIndex, resetTrip.m_Creature, new TripSource(resetTrip.m_Source, resetTrip.m_Delay));
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
		public ComponentTypeHandle<ResetTrip> __Game_Creatures_ResetTrip_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TravelPurpose> __Game_Citizens_TravelPurpose_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<TripNeeded> __Game_Citizens_TripNeeded_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<GroupCreature> __Game_Creatures_GroupCreature_RO_BufferLookup;

		public ComponentLookup<HumanCurrentLane> __Game_Creatures_HumanCurrentLane_RW_ComponentLookup;

		public ComponentLookup<AnimalCurrentLane> __Game_Creatures_AnimalCurrentLane_RW_ComponentLookup;

		public ComponentLookup<Resident> __Game_Creatures_Resident_RW_ComponentLookup;

		public ComponentLookup<Pet> __Game_Creatures_Pet_RW_ComponentLookup;

		public ComponentLookup<Target> __Game_Common_Target_RW_ComponentLookup;

		public ComponentLookup<Divert> __Game_Creatures_Divert_RW_ComponentLookup;

		public ComponentLookup<PathOwner> __Game_Pathfind_PathOwner_RW_ComponentLookup;

		public BufferLookup<PathElement> __Game_Pathfind_PathElement_RW_BufferLookup;

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
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Creatures_ResetTrip_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ResetTrip>(true);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Citizens_TravelPurpose_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TravelPurpose>(true);
			__Game_Citizens_TripNeeded_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TripNeeded>(true);
			__Game_Creatures_GroupCreature_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<GroupCreature>(true);
			__Game_Creatures_HumanCurrentLane_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HumanCurrentLane>(false);
			__Game_Creatures_AnimalCurrentLane_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AnimalCurrentLane>(false);
			__Game_Creatures_Resident_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Resident>(false);
			__Game_Creatures_Pet_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Pet>(false);
			__Game_Common_Target_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Target>(false);
			__Game_Creatures_Divert_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Divert>(false);
			__Game_Pathfind_PathOwner_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathOwner>(false);
			__Game_Pathfind_PathElement_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PathElement>(false);
		}
	}

	private ModificationBarrier4 m_ModificationBarrier;

	private EntityQuery m_ResetQuery;

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
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier4>();
		m_ResetQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ResetTrip>() });
		((ComponentSystemBase)this).RequireForUpdate(m_ResetQuery);
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
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		CreatureTripResetJob creatureTripResetJob = new CreatureTripResetJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResetTripType = InternalCompilerInterface.GetComponentTypeHandle<ResetTrip>(ref __TypeHandle.__Game_Creatures_ResetTrip_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Deleted = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TravelPurpose = InternalCompilerInterface.GetComponentLookup<TravelPurpose>(ref __TypeHandle.__Game_Citizens_TravelPurpose_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TripNeeded = InternalCompilerInterface.GetBufferLookup<TripNeeded>(ref __TypeHandle.__Game_Citizens_TripNeeded_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GroupCreatures = InternalCompilerInterface.GetBufferLookup<GroupCreature>(ref __TypeHandle.__Game_Creatures_GroupCreature_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HumanCurrentLane = InternalCompilerInterface.GetComponentLookup<HumanCurrentLane>(ref __TypeHandle.__Game_Creatures_HumanCurrentLane_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AnimalCurrentLane = InternalCompilerInterface.GetComponentLookup<AnimalCurrentLane>(ref __TypeHandle.__Game_Creatures_AnimalCurrentLane_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Resident = InternalCompilerInterface.GetComponentLookup<Resident>(ref __TypeHandle.__Game_Creatures_Resident_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Pet = InternalCompilerInterface.GetComponentLookup<Pet>(ref __TypeHandle.__Game_Creatures_Pet_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Target = InternalCompilerInterface.GetComponentLookup<Target>(ref __TypeHandle.__Game_Common_Target_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Divert = InternalCompilerInterface.GetComponentLookup<Divert>(ref __TypeHandle.__Game_Creatures_Divert_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathOwner = InternalCompilerInterface.GetComponentLookup<PathOwner>(ref __TypeHandle.__Game_Pathfind_PathOwner_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathElements = InternalCompilerInterface.GetBufferLookup<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		EntityCommandBuffer val = m_ModificationBarrier.CreateCommandBuffer();
		creatureTripResetJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		CreatureTripResetJob creatureTripResetJob2 = creatureTripResetJob;
		((SystemBase)this).Dependency = JobChunkExtensions.Schedule<CreatureTripResetJob>(creatureTripResetJob2, m_ResetQuery, ((SystemBase)this).Dependency);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(((SystemBase)this).Dependency);
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
	public TripResetSystem()
	{
	}
}
