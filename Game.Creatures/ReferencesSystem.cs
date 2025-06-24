using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Citizens;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Creatures;

[CompilerGenerated]
public class ReferencesSystem : GameSystemBase
{
	[BurstCompile]
	private struct UpdateCreatureReferencesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Created> m_CreatedType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Resident> m_ResidentType;

		[ReadOnly]
		public ComponentTypeHandle<Pet> m_PetType;

		[ReadOnly]
		public ComponentTypeHandle<HumanCurrentLane> m_HumanCurrentLaneType;

		[ReadOnly]
		public ComponentTypeHandle<AnimalCurrentLane> m_AnimalCurrentLaneType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentVehicle> m_CurrentVehicleType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_ObjectGeometryData;

		public ComponentLookup<CurrentTransport> m_CurrentTransports;

		public BufferLookup<OwnedCreature> m_OwnedCreatures;

		public BufferLookup<LaneObject> m_LaneObjects;

		public BufferLookup<Passenger> m_Passengers;

		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_SearchTree;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_0447: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0452: Unknown result type (might be due to invalid IL or missing references)
			//IL_0457: Unknown result type (might be due to invalid IL or missing references)
			//IL_046c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0480: Unknown result type (might be due to invalid IL or missing references)
			//IL_048a: Unknown result type (might be due to invalid IL or missing references)
			//IL_048f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0413: Unknown result type (might be due to invalid IL or missing references)
			//IL_0418: Unknown result type (might be due to invalid IL or missing references)
			//IL_041d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0542: Unknown result type (might be due to invalid IL or missing references)
			//IL_0547: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0500: Unknown result type (might be due to invalid IL or missing references)
			//IL_050a: Unknown result type (might be due to invalid IL or missing references)
			//IL_050f: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0520: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_062f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0634: Unknown result type (might be due to invalid IL or missing references)
			//IL_055b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0560: Unknown result type (might be due to invalid IL or missing references)
			//IL_0575: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_060f: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0600: Unknown result type (might be due to invalid IL or missing references)
			//IL_0589: Unknown result type (might be due to invalid IL or missing references)
			//IL_058e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0593: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_063f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0644: Unknown result type (might be due to invalid IL or missing references)
			//IL_0659: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0686: Unknown result type (might be due to invalid IL or missing references)
			//IL_066d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0672: Unknown result type (might be due to invalid IL or missing references)
			//IL_0677: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			if (((ArchetypeChunk)(ref chunk)).Has<Created>(ref m_CreatedType))
			{
				NativeArray<Transform> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
				NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				NativeArray<Owner> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
				for (int i = 0; i < nativeArray4.Length; i++)
				{
					Entity creature = nativeArray[i];
					Owner owner = nativeArray4[i];
					if (m_OwnedCreatures.HasBuffer(owner.m_Owner))
					{
						m_OwnedCreatures[owner.m_Owner].Add(new OwnedCreature(creature));
					}
				}
				NativeArray<Resident> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Resident>(ref m_ResidentType);
				for (int j = 0; j < nativeArray5.Length; j++)
				{
					Entity currentTransport = nativeArray[j];
					Resident resident = nativeArray5[j];
					if (m_CurrentTransports.HasComponent(resident.m_Citizen))
					{
						CurrentTransport currentTransport2 = m_CurrentTransports[resident.m_Citizen];
						currentTransport2.m_CurrentTransport = currentTransport;
						m_CurrentTransports[resident.m_Citizen] = currentTransport2;
					}
				}
				NativeArray<Pet> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Pet>(ref m_PetType);
				for (int k = 0; k < nativeArray6.Length; k++)
				{
					Entity currentTransport3 = nativeArray[k];
					Pet pet = nativeArray6[k];
					if (m_CurrentTransports.HasComponent(pet.m_HouseholdPet))
					{
						CurrentTransport currentTransport4 = m_CurrentTransports[pet.m_HouseholdPet];
						currentTransport4.m_CurrentTransport = currentTransport3;
						m_CurrentTransports[pet.m_HouseholdPet] = currentTransport4;
					}
				}
				NativeArray<CurrentVehicle> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentVehicle>(ref m_CurrentVehicleType);
				for (int l = 0; l < nativeArray7.Length; l++)
				{
					Entity passenger = nativeArray[l];
					CurrentVehicle currentVehicle = nativeArray7[l];
					if (m_Passengers.HasBuffer(currentVehicle.m_Vehicle))
					{
						m_Passengers[currentVehicle.m_Vehicle].Add(new Passenger(passenger));
					}
				}
				NativeArray<HumanCurrentLane> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HumanCurrentLane>(ref m_HumanCurrentLaneType);
				for (int m = 0; m < nativeArray8.Length; m++)
				{
					Entity val = nativeArray[m];
					HumanCurrentLane humanCurrentLane = nativeArray8[m];
					if (m_LaneObjects.HasBuffer(humanCurrentLane.m_Lane))
					{
						NetUtils.AddLaneObject(m_LaneObjects[humanCurrentLane.m_Lane], val, ((float2)(ref humanCurrentLane.m_CurvePosition)).xx);
						continue;
					}
					Transform transform = nativeArray2[m];
					PrefabRef prefabRef = nativeArray3[m];
					ObjectGeometryData geometryData = m_ObjectGeometryData[prefabRef.m_Prefab];
					Bounds3 bounds = ObjectUtils.CalculateBounds(transform.m_Position, transform.m_Rotation, geometryData);
					m_SearchTree.Add(val, new QuadTreeBoundsXZ(bounds));
				}
				NativeArray<AnimalCurrentLane> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AnimalCurrentLane>(ref m_AnimalCurrentLaneType);
				for (int n = 0; n < nativeArray9.Length; n++)
				{
					Entity val2 = nativeArray[n];
					AnimalCurrentLane animalCurrentLane = nativeArray9[n];
					if (m_LaneObjects.HasBuffer(animalCurrentLane.m_Lane))
					{
						NetUtils.AddLaneObject(m_LaneObjects[animalCurrentLane.m_Lane], val2, ((float2)(ref animalCurrentLane.m_CurvePosition)).xx);
						continue;
					}
					Transform transform2 = nativeArray2[n];
					PrefabRef prefabRef2 = nativeArray3[n];
					ObjectGeometryData geometryData2 = m_ObjectGeometryData[prefabRef2.m_Prefab];
					Bounds3 bounds2 = ObjectUtils.CalculateBounds(transform2.m_Position, transform2.m_Rotation, geometryData2);
					m_SearchTree.Add(val2, new QuadTreeBoundsXZ(bounds2));
				}
				return;
			}
			NativeArray<Owner> nativeArray10 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			if (nativeArray10.Length != 0)
			{
				((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				for (int num = 0; num < nativeArray10.Length; num++)
				{
					Entity creature2 = nativeArray[num];
					Owner owner2 = nativeArray10[num];
					if (m_OwnedCreatures.HasBuffer(owner2.m_Owner))
					{
						CollectionUtils.RemoveValue<OwnedCreature>(m_OwnedCreatures[owner2.m_Owner], new OwnedCreature(creature2));
					}
				}
			}
			NativeArray<Resident> nativeArray11 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Resident>(ref m_ResidentType);
			for (int num2 = 0; num2 < nativeArray11.Length; num2++)
			{
				Entity val3 = nativeArray[num2];
				Resident resident2 = nativeArray11[num2];
				if (m_CurrentTransports.HasComponent(resident2.m_Citizen) && m_CurrentTransports[resident2.m_Citizen].m_CurrentTransport == val3)
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<CurrentTransport>(resident2.m_Citizen);
				}
			}
			NativeArray<Pet> nativeArray12 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Pet>(ref m_PetType);
			for (int num3 = 0; num3 < nativeArray12.Length; num3++)
			{
				Entity val4 = nativeArray[num3];
				Pet pet2 = nativeArray12[num3];
				if (m_CurrentTransports.HasComponent(pet2.m_HouseholdPet) && m_CurrentTransports[pet2.m_HouseholdPet].m_CurrentTransport == val4)
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<CurrentTransport>(pet2.m_HouseholdPet);
				}
			}
			NativeArray<CurrentVehicle> nativeArray13 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentVehicle>(ref m_CurrentVehicleType);
			if (nativeArray13.Length != 0)
			{
				for (int num4 = 0; num4 < nativeArray13.Length; num4++)
				{
					Entity passenger2 = nativeArray[num4];
					CurrentVehicle currentVehicle2 = nativeArray13[num4];
					if (m_Passengers.HasBuffer(currentVehicle2.m_Vehicle))
					{
						CollectionUtils.RemoveValue<Passenger>(m_Passengers[currentVehicle2.m_Vehicle], new Passenger(passenger2));
					}
				}
			}
			NativeArray<HumanCurrentLane> nativeArray14 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HumanCurrentLane>(ref m_HumanCurrentLaneType);
			for (int num5 = 0; num5 < nativeArray14.Length; num5++)
			{
				Entity val5 = nativeArray[num5];
				HumanCurrentLane humanCurrentLane2 = nativeArray14[num5];
				if (m_LaneObjects.HasBuffer(humanCurrentLane2.m_Lane))
				{
					NetUtils.RemoveLaneObject(m_LaneObjects[humanCurrentLane2.m_Lane], val5);
				}
				else
				{
					m_SearchTree.TryRemove(val5);
				}
			}
			NativeArray<AnimalCurrentLane> nativeArray15 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AnimalCurrentLane>(ref m_AnimalCurrentLaneType);
			for (int num6 = 0; num6 < nativeArray15.Length; num6++)
			{
				Entity val6 = nativeArray[num6];
				AnimalCurrentLane animalCurrentLane2 = nativeArray15[num6];
				if (m_LaneObjects.HasBuffer(animalCurrentLane2.m_Lane))
				{
					NetUtils.RemoveLaneObject(m_LaneObjects[animalCurrentLane2.m_Lane], val6);
				}
				else
				{
					m_SearchTree.TryRemove(val6);
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
		public ComponentTypeHandle<Created> __Game_Common_Created_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Resident> __Game_Creatures_Resident_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Pet> __Game_Creatures_Pet_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<HumanCurrentLane> __Game_Creatures_HumanCurrentLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<AnimalCurrentLane> __Game_Creatures_AnimalCurrentLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentVehicle> __Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		public ComponentLookup<CurrentTransport> __Game_Citizens_CurrentTransport_RW_ComponentLookup;

		public BufferLookup<OwnedCreature> __Game_Creatures_OwnedCreature_RW_BufferLookup;

		public BufferLookup<LaneObject> __Game_Net_LaneObject_RW_BufferLookup;

		public BufferLookup<Passenger> __Game_Vehicles_Passenger_RW_BufferLookup;

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
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Created_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Created>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Creatures_Resident_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Resident>(true);
			__Game_Creatures_Pet_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Pet>(true);
			__Game_Creatures_HumanCurrentLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HumanCurrentLane>(true);
			__Game_Creatures_AnimalCurrentLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AnimalCurrentLane>(true);
			__Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentVehicle>(true);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Citizens_CurrentTransport_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentTransport>(false);
			__Game_Creatures_OwnedCreature_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<OwnedCreature>(false);
			__Game_Net_LaneObject_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneObject>(false);
			__Game_Vehicles_Passenger_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Passenger>(false);
		}
	}

	private ModificationBarrier5 m_ModificationBarrier;

	private Game.Objects.SearchSystem m_SearchSystem;

	private EntityQuery m_CreatureQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier5>();
		m_SearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.SearchSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Creature>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Deleted>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array[0] = val;
		m_CreatureQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		((ComponentSystemBase)this).RequireForUpdate(m_CreatureQuery);
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
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		JobHandle val = JobChunkExtensions.Schedule<UpdateCreatureReferencesJob>(new UpdateCreatureReferencesJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CreatedType = InternalCompilerInterface.GetComponentTypeHandle<Created>(ref __TypeHandle.__Game_Common_Created_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResidentType = InternalCompilerInterface.GetComponentTypeHandle<Resident>(ref __TypeHandle.__Game_Creatures_Resident_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PetType = InternalCompilerInterface.GetComponentTypeHandle<Pet>(ref __TypeHandle.__Game_Creatures_Pet_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HumanCurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<HumanCurrentLane>(ref __TypeHandle.__Game_Creatures_HumanCurrentLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AnimalCurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<AnimalCurrentLane>(ref __TypeHandle.__Game_Creatures_AnimalCurrentLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentVehicleType = InternalCompilerInterface.GetComponentTypeHandle<CurrentVehicle>(ref __TypeHandle.__Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentTransports = InternalCompilerInterface.GetComponentLookup<CurrentTransport>(ref __TypeHandle.__Game_Citizens_CurrentTransport_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnedCreatures = InternalCompilerInterface.GetBufferLookup<OwnedCreature>(ref __TypeHandle.__Game_Creatures_OwnedCreature_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneObjects = InternalCompilerInterface.GetBufferLookup<LaneObject>(ref __TypeHandle.__Game_Net_LaneObject_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Passengers = InternalCompilerInterface.GetBufferLookup<Passenger>(ref __TypeHandle.__Game_Vehicles_Passenger_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SearchTree = m_SearchSystem.GetMovingSearchTree(readOnly: false, out dependencies),
			m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer()
		}, m_CreatureQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies));
		m_SearchSystem.AddMovingSearchTreeWriter(val);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val);
		((SystemBase)this).Dependency = val;
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
