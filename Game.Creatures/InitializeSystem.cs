using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Simulation;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Creatures;

[CompilerGenerated]
public class InitializeSystem : GameSystemBase
{
	[BurstCompile]
	private struct InitializeCreaturesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<TripSource> m_TripSourceType;

		[ReadOnly]
		public ComponentTypeHandle<Unspawned> m_UnspawnedType;

		[ReadOnly]
		public ComponentTypeHandle<PseudoRandomSeed> m_PseudoRandomSeedType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentVehicle> m_CurrentVehicleType;

		[ReadOnly]
		public ComponentTypeHandle<PathOwner> m_PathOwnerType;

		[ReadOnly]
		public BufferTypeHandle<PathElement> m_PathElementType;

		public ComponentTypeHandle<Human> m_HumanType;

		public ComponentTypeHandle<Animal> m_AnimalType;

		public ComponentTypeHandle<HumanCurrentLane> m_HumanCurrentLaneType;

		public ComponentTypeHandle<AnimalCurrentLane> m_AnimalCurrentLaneType;

		public ComponentTypeHandle<HumanNavigation> m_HumanNavigationType;

		public ComponentTypeHandle<AnimalNavigation> m_AnimalNavigationType;

		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		[ReadOnly]
		public ComponentLookup<Resident> m_ResidentData;

		[ReadOnly]
		public ComponentLookup<HouseholdMember> m_HouseholdMemberData;

		[ReadOnly]
		public ComponentLookup<HomelessHousehold> m_HomelessHousehold;

		[ReadOnly]
		public ComponentLookup<Worker> m_WorkerData;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenterData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> m_ConnectionLaneData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> m_SpawnLocationData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<AnimalData> m_AnimalData;

		[ReadOnly]
		public BufferLookup<SpawnLocationElement> m_SpawnLocations;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> m_AreaNodes;

		[ReadOnly]
		public BufferLookup<Triangle> m_AreaTriangles;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public ComponentTypeSet m_TripSourceRemoveTypes;

		[ReadOnly]
		public float m_Temperature;

		[ReadOnly]
		public bool m_LeftHandTraffic;

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
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0793: Unknown result type (might be due to invalid IL or missing references)
			//IL_0798: Unknown result type (might be due to invalid IL or missing references)
			//IL_04be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05da: Unknown result type (might be due to invalid IL or missing references)
			//IL_05df: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_060e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0622: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0644: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a11: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a26: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a2c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a2e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a33: Unknown result type (might be due to invalid IL or missing references)
			//IL_0658: Unknown result type (might be due to invalid IL or missing references)
			//IL_0449: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_045f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0809: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0913: Unknown result type (might be due to invalid IL or missing references)
			//IL_0820: Unknown result type (might be due to invalid IL or missing references)
			//IL_0825: Unknown result type (might be due to invalid IL or missing references)
			//IL_082a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0832: Unknown result type (might be due to invalid IL or missing references)
			//IL_083e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0845: Unknown result type (might be due to invalid IL or missing references)
			//IL_092a: Unknown result type (might be due to invalid IL or missing references)
			//IL_093c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0947: Unknown result type (might be due to invalid IL or missing references)
			//IL_094e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0953: Unknown result type (might be due to invalid IL or missing references)
			//IL_0958: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_085a: Unknown result type (might be due to invalid IL or missing references)
			//IL_086e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0882: Unknown result type (might be due to invalid IL or missing references)
			//IL_0967: Unknown result type (might be due to invalid IL or missing references)
			//IL_096c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0973: Unknown result type (might be due to invalid IL or missing references)
			//IL_0975: Unknown result type (might be due to invalid IL or missing references)
			//IL_097a: Unknown result type (might be due to invalid IL or missing references)
			//IL_097f: Unknown result type (might be due to invalid IL or missing references)
			//IL_08fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_08dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_08df: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0896: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d6: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Human> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Human>(ref m_HumanType);
			NativeArray<HumanCurrentLane> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HumanCurrentLane>(ref m_HumanCurrentLaneType);
			NativeArray<AnimalCurrentLane> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AnimalCurrentLane>(ref m_AnimalCurrentLaneType);
			NativeArray<HumanNavigation> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HumanNavigation>(ref m_HumanNavigationType);
			NativeArray<AnimalNavigation> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AnimalNavigation>(ref m_AnimalNavigationType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			float3 val4;
			if (nativeArray5.Length != 0)
			{
				NativeArray<CurrentVehicle> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentVehicle>(ref m_CurrentVehicleType);
				NativeArray<TripSource> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TripSource>(ref m_TripSourceType);
				NativeArray<PathOwner> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathOwner>(ref m_PathOwnerType);
				BufferAccessor<PathElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<PathElement>(ref m_PathElementType);
				bool flag = ((ArchetypeChunk)(ref chunk)).Has<Unspawned>(ref m_UnspawnedType);
				CurrentVehicle currentVehicle = default(CurrentVehicle);
				HumanCurrentLane humanCurrentLane = default(HumanCurrentLane);
				for (int i = 0; i < nativeArray5.Length; i++)
				{
					Entity val = nativeArray[i];
					if (flag && nativeArray8.Length != 0)
					{
						TripSource tripSource = nativeArray8[i];
						if (m_DeletedData.HasComponent(tripSource.m_Source))
						{
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(unfilteredChunkIndex, val, ref m_TripSourceRemoveTypes);
						}
						if (m_SpawnLocations.HasBuffer(tripSource.m_Source))
						{
							PathOwner pathOwner = nativeArray9[i];
							DynamicBuffer<PathElement> path = bufferAccessor[i];
							DynamicBuffer<SpawnLocationElement> spawnLocations = m_SpawnLocations[tripSource.m_Source];
							Transform transform = CalculatePathTransform(val, pathOwner, path);
							if (!FindClosestSpawnLocation(transform.m_Position, out var spawnPosition, spawnLocations, path.Length == 0, ref random, HasAuthorization(val, tripSource.m_Source)))
							{
								Transform transform2 = m_TransformData[tripSource.m_Source];
								PrefabRef prefabRef = m_PrefabRefData[tripSource.m_Source];
								if (m_BuildingData.HasComponent(prefabRef.m_Prefab))
								{
									BuildingData buildingData = m_BuildingData[prefabRef.m_Prefab];
									spawnPosition = BuildingUtils.CalculateFrontPosition(transform2, buildingData.m_LotSize.y);
								}
								else
								{
									spawnPosition = transform2.m_Position;
								}
							}
							float3 val2 = transform.m_Position - spawnPosition;
							if (MathUtils.TryNormalize(ref val2))
							{
								transform.m_Position = spawnPosition;
								transform.m_Rotation = quaternion.LookRotationSafe(val2, math.up());
							}
							m_TransformData[val] = transform;
						}
						else if (m_TransformData.HasComponent(tripSource.m_Source))
						{
							PathOwner pathOwner2 = nativeArray9[i];
							DynamicBuffer<PathElement> path2 = bufferAccessor[i];
							Transform transform3 = m_TransformData[tripSource.m_Source];
							Transform transform4 = CalculatePathTransform(val, pathOwner2, path2);
							float3 val3 = transform4.m_Position - transform3.m_Position;
							if (MathUtils.TryNormalize(ref val3))
							{
								transform4.m_Position = transform3.m_Position;
								transform4.m_Rotation = quaternion.LookRotationSafe(val3, math.up());
							}
							m_TransformData[val] = transform4;
						}
					}
					Transform transform5 = m_TransformData[val];
					HumanNavigation humanNavigation = new HumanNavigation
					{
						m_TargetPosition = transform5.m_Position
					};
					val4 = math.forward(transform5.m_Rotation);
					humanNavigation.m_TargetDirection = math.normalizesafe(((float3)(ref val4)).xz, default(float2));
					if (CollectionUtils.TryGet<CurrentVehicle>(nativeArray7, i, ref currentVehicle) && CollectionUtils.TryGet<HumanCurrentLane>(nativeArray3, i, ref humanCurrentLane) && (currentVehicle.m_Flags & CreatureVehicleFlags.Exiting) != 0 && (humanCurrentLane.m_Flags & CreatureLaneFlags.EndOfPath) != 0)
					{
						humanNavigation.m_TransformState = TransformState.Action;
						humanNavigation.m_LastActivity = 11;
						humanNavigation.m_TargetActivity = 11;
					}
					nativeArray5[i] = humanNavigation;
				}
			}
			if (nativeArray2.Length != 0)
			{
				NativeArray<PseudoRandomSeed> nativeArray10 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PseudoRandomSeed>(ref m_PseudoRandomSeedType);
				PseudoRandomSeed pseudoRandomSeed = default(PseudoRandomSeed);
				Resident resident = default(Resident);
				HouseholdMember householdMember = default(HouseholdMember);
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					Human human = nativeArray2[j];
					human.m_Flags &= ~(HumanFlags.Cold | HumanFlags.Homeless);
					float num;
					if (CollectionUtils.TryGet<PseudoRandomSeed>(nativeArray10, j, ref pseudoRandomSeed))
					{
						Random random2 = pseudoRandomSeed.GetRandom(PseudoRandomSeed.kTemperatureLimit);
						num = ((Random)(ref random2)).NextFloat(15f, 20f);
					}
					else
					{
						num = ((Random)(ref random)).NextFloat(15f, 20f);
					}
					if (m_Temperature < num)
					{
						human.m_Flags |= HumanFlags.Cold;
					}
					if (m_ResidentData.TryGetComponent(nativeArray[j], ref resident) && m_HouseholdMemberData.TryGetComponent(resident.m_Citizen, ref householdMember) && m_HomelessHousehold.HasComponent(householdMember.m_Household))
					{
						human.m_Flags |= HumanFlags.Homeless;
					}
					nativeArray2[j] = human;
				}
			}
			if (nativeArray3.Length != 0)
			{
				for (int k = 0; k < nativeArray3.Length; k++)
				{
					HumanCurrentLane humanCurrentLane2 = nativeArray3[k];
					if (m_TransformData.HasComponent(humanCurrentLane2.m_Lane))
					{
						humanCurrentLane2.m_Flags |= CreatureLaneFlags.TransformTarget;
					}
					else if (m_ConnectionLaneData.HasComponent(humanCurrentLane2.m_Lane))
					{
						if ((m_ConnectionLaneData[humanCurrentLane2.m_Lane].m_Flags & ConnectionLaneFlags.Area) != 0)
						{
							humanCurrentLane2.m_Flags |= CreatureLaneFlags.Area;
						}
						else
						{
							humanCurrentLane2.m_Flags |= CreatureLaneFlags.Connection;
						}
					}
					humanCurrentLane2.m_LanePosition = ((Random)(ref random)).NextFloat(0f, 1f);
					humanCurrentLane2.m_LanePosition *= humanCurrentLane2.m_LanePosition;
					humanCurrentLane2.m_LanePosition = math.select(0.5f - humanCurrentLane2.m_LanePosition, humanCurrentLane2.m_LanePosition - 0.5f, m_LeftHandTraffic != ((humanCurrentLane2.m_Flags & CreatureLaneFlags.Backward) != 0));
					nativeArray3[k] = humanCurrentLane2;
				}
			}
			if (nativeArray4.Length != 0)
			{
				NativeArray<Animal> nativeArray11 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Animal>(ref m_AnimalType);
				for (int l = 0; l < nativeArray4.Length; l++)
				{
					Entity val5 = nativeArray[l];
					Animal animal = nativeArray11[l];
					AnimalCurrentLane animalCurrentLane = nativeArray4[l];
					PrefabRef prefabRef2 = m_PrefabRefData[val5];
					AnimalData animalData = m_AnimalData[prefabRef2.m_Prefab];
					if (m_TransformData.HasComponent(animalCurrentLane.m_Lane))
					{
						animalCurrentLane.m_Flags |= CreatureLaneFlags.TransformTarget;
					}
					else if (m_ConnectionLaneData.HasComponent(animalCurrentLane.m_Lane))
					{
						if ((m_ConnectionLaneData[animalCurrentLane.m_Lane].m_Flags & ConnectionLaneFlags.Area) != 0)
						{
							animalCurrentLane.m_Flags |= CreatureLaneFlags.Area;
						}
						else
						{
							animalCurrentLane.m_Flags |= CreatureLaneFlags.Connection;
						}
					}
					if (animalData.m_MoveSpeed == 0f && animalData.m_SwimSpeed > 0f)
					{
						animal.m_Flags |= AnimalFlags.SwimmingTarget;
						animalCurrentLane.m_Flags |= CreatureLaneFlags.Swimming;
					}
					if ((animal.m_Flags & AnimalFlags.Roaming) != 0)
					{
						animalCurrentLane.m_LanePosition = ((Random)(ref random)).NextFloat(-0.5f, 0.5f);
					}
					else
					{
						animalCurrentLane.m_LanePosition = ((Random)(ref random)).NextFloat(0f, 1f);
						animalCurrentLane.m_LanePosition *= animalCurrentLane.m_LanePosition;
						animalCurrentLane.m_LanePosition = math.select(0.5f - animalCurrentLane.m_LanePosition, animalCurrentLane.m_LanePosition - 0.5f, m_LeftHandTraffic != ((animalCurrentLane.m_Flags & CreatureLaneFlags.Backward) != 0));
					}
					nativeArray11[l] = animal;
					nativeArray4[l] = animalCurrentLane;
				}
			}
			if (nativeArray6.Length == 0)
			{
				return;
			}
			NativeArray<TripSource> nativeArray12 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TripSource>(ref m_TripSourceType);
			bool flag2 = ((ArchetypeChunk)(ref chunk)).Has<Unspawned>(ref m_UnspawnedType);
			for (int m = 0; m < nativeArray6.Length; m++)
			{
				Entity val6 = nativeArray[m];
				if (flag2 && nativeArray12.Length != 0)
				{
					TripSource tripSource2 = nativeArray12[m];
					if (m_DeletedData.HasComponent(tripSource2.m_Source))
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(unfilteredChunkIndex, val6, ref m_TripSourceRemoveTypes);
					}
					if (m_SpawnLocations.HasBuffer(tripSource2.m_Source))
					{
						DynamicBuffer<SpawnLocationElement> spawnLocations2 = m_SpawnLocations[tripSource2.m_Source];
						Transform transform6 = m_TransformData[val6];
						if (!FindClosestSpawnLocation(transform6.m_Position, out var spawnPosition2, spawnLocations2, randomLocation: false, ref random, hasAuthorization: false))
						{
							Transform transform7 = m_TransformData[tripSource2.m_Source];
							PrefabRef prefabRef3 = m_PrefabRefData[tripSource2.m_Source];
							if (m_BuildingData.HasComponent(prefabRef3.m_Prefab))
							{
								BuildingData buildingData2 = m_BuildingData[prefabRef3.m_Prefab];
								spawnPosition2 = BuildingUtils.CalculateFrontPosition(transform7, buildingData2.m_LotSize.y);
							}
							else
							{
								spawnPosition2 = transform7.m_Position;
							}
						}
						float3 val7 = transform6.m_Position - spawnPosition2;
						if (MathUtils.TryNormalize(ref val7))
						{
							transform6.m_Position = spawnPosition2;
							transform6.m_Rotation = quaternion.LookRotationSafe(val7, math.up());
						}
						m_TransformData[val6] = transform6;
					}
					else if (m_TransformData.HasComponent(tripSource2.m_Source))
					{
						Transform transform8 = m_TransformData[tripSource2.m_Source];
						Transform transform9 = m_TransformData[val6];
						float3 val8 = transform9.m_Position - transform8.m_Position;
						if (MathUtils.TryNormalize(ref val8))
						{
							transform9.m_Position = transform8.m_Position;
							transform9.m_Rotation = quaternion.LookRotationSafe(val8, math.up());
						}
						if (nativeArray4.Length != 0 && (nativeArray4[m].m_Flags & CreatureLaneFlags.Swimming) != 0)
						{
							PrefabRef prefabRef4 = m_PrefabRefData[val6];
							AnimalData animalData2 = m_AnimalData[prefabRef4.m_Prefab];
							transform9.m_Position.y -= animalData2.m_SwimDepth.min;
						}
						m_TransformData[val6] = transform9;
					}
				}
				Transform transform10 = m_TransformData[val6];
				AnimalNavigation animalNavigation = new AnimalNavigation
				{
					m_TargetPosition = transform10.m_Position
				};
				float3 val9 = math.forward(transform10.m_Rotation);
				val4 = default(float3);
				animalNavigation.m_TargetDirection = math.normalizesafe(val9, val4);
				nativeArray6[m] = animalNavigation;
			}
		}

		private Transform CalculatePathTransform(Entity creature, PathOwner pathOwner, DynamicBuffer<PathElement> path)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			Transform result = m_TransformData[creature];
			if (path.Length > pathOwner.m_ElementIndex)
			{
				PathElement pathElement = path[pathOwner.m_ElementIndex];
				if (m_CurveData.HasComponent(pathElement.m_Target))
				{
					Curve curve = m_CurveData[pathElement.m_Target];
					result.m_Position = MathUtils.Position(curve.m_Bezier, pathElement.m_TargetDelta.x);
					float3 val = MathUtils.Tangent(curve.m_Bezier, pathElement.m_TargetDelta.x);
					if (MathUtils.TryNormalize(ref val))
					{
						result.m_Rotation = quaternion.LookRotationSafe(val, math.up());
					}
				}
			}
			return result;
		}

		private bool HasAuthorization(Entity entity, Entity building)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			Resident resident = default(Resident);
			if (m_ResidentData.TryGetComponent(entity, ref resident))
			{
				HouseholdMember householdMember = default(HouseholdMember);
				PropertyRenter propertyRenter = default(PropertyRenter);
				if (m_HouseholdMemberData.TryGetComponent(resident.m_Citizen, ref householdMember) && m_PropertyRenterData.TryGetComponent(householdMember.m_Household, ref propertyRenter) && propertyRenter.m_Property == building)
				{
					return true;
				}
				Worker worker = default(Worker);
				if (m_WorkerData.TryGetComponent(resident.m_Citizen, ref worker))
				{
					PropertyRenter propertyRenter2 = default(PropertyRenter);
					if (m_PropertyRenterData.TryGetComponent(worker.m_Workplace, ref propertyRenter2))
					{
						if (propertyRenter2.m_Property == building)
						{
							return true;
						}
					}
					else if (worker.m_Workplace == building)
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool FindClosestSpawnLocation(float3 comparePosition, out float3 spawnPosition, DynamicBuffer<SpawnLocationElement> spawnLocations, bool randomLocation, ref Random random, bool hasAuthorization)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			spawnPosition = comparePosition;
			float num = float.MaxValue;
			bool flag = true;
			bool result = false;
			float2 val2 = default(float2);
			for (int i = 0; i < spawnLocations.Length; i++)
			{
				if (spawnLocations[i].m_Type != SpawnLocationType.SpawnLocation && spawnLocations[i].m_Type != SpawnLocationType.HangaroundLocation)
				{
					continue;
				}
				Entity spawnLocation = spawnLocations[i].m_SpawnLocation;
				PrefabRef prefabRef = m_PrefabRefData[spawnLocation];
				SpawnLocationData spawnLocationData = m_SpawnLocationData[prefabRef.m_Prefab];
				if (spawnLocationData.m_ConnectionType != RouteConnectionType.Pedestrian)
				{
					continue;
				}
				bool flag2 = spawnLocationData.m_ActivityMask.m_Mask != 0;
				if (flag2 && !flag)
				{
					continue;
				}
				if (m_TransformData.HasComponent(spawnLocation))
				{
					Transform transform = m_TransformData[spawnLocation];
					float num2;
					if (randomLocation)
					{
						num2 = ((Random)(ref random)).NextFloat();
						num2 += math.select(0f, 1f, hasAuthorization != spawnLocationData.m_RequireAuthorization);
					}
					else
					{
						num2 = math.distance(transform.m_Position, comparePosition);
					}
					if ((!flag2 && flag) || num2 < num)
					{
						spawnPosition = transform.m_Position;
						num = num2;
						flag = flag2;
						result = true;
					}
				}
				else
				{
					if (!m_AreaNodes.HasBuffer(spawnLocation))
					{
						continue;
					}
					DynamicBuffer<Game.Areas.Node> nodes = m_AreaNodes[spawnLocation];
					DynamicBuffer<Triangle> val = m_AreaTriangles[spawnLocation];
					for (int j = 0; j < val.Length; j++)
					{
						Triangle3 triangle = AreaUtils.GetTriangle3(nodes, val[j]);
						float num3;
						if (randomLocation)
						{
							num3 = ((Random)(ref random)).NextFloat();
							num3 += math.select(0f, 1f, hasAuthorization != spawnLocationData.m_RequireAuthorization);
							val2 = ((Random)(ref random)).NextFloat2();
							val2 = math.select(val2, 1f - val2, math.csum(val2) > 1f);
						}
						else
						{
							num3 = MathUtils.Distance(triangle, comparePosition, ref val2);
						}
						if ((!flag2 && flag) || num3 < num)
						{
							spawnPosition = MathUtils.Position(triangle, val2);
							num = num3;
							flag = flag2;
							result = true;
						}
					}
				}
			}
			return result;
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
		public ComponentTypeHandle<TripSource> __Game_Objects_TripSource_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Unspawned> __Game_Objects_Unspawned_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PseudoRandomSeed> __Game_Common_PseudoRandomSeed_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentVehicle> __Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PathOwner> __Game_Pathfind_PathOwner_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<PathElement> __Game_Pathfind_PathElement_RO_BufferTypeHandle;

		public ComponentTypeHandle<Human> __Game_Creatures_Human_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Animal> __Game_Creatures_Animal_RW_ComponentTypeHandle;

		public ComponentTypeHandle<HumanCurrentLane> __Game_Creatures_HumanCurrentLane_RW_ComponentTypeHandle;

		public ComponentTypeHandle<AnimalCurrentLane> __Game_Creatures_AnimalCurrentLane_RW_ComponentTypeHandle;

		public ComponentTypeHandle<HumanNavigation> __Game_Creatures_HumanNavigation_RW_ComponentTypeHandle;

		public ComponentTypeHandle<AnimalNavigation> __Game_Creatures_AnimalNavigation_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Resident> __Game_Creatures_Resident_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HouseholdMember> __Game_Citizens_HouseholdMember_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HomelessHousehold> __Game_Citizens_HomelessHousehold_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Worker> __Game_Citizens_Worker_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ConnectionLane> __Game_Net_ConnectionLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> __Game_Prefabs_SpawnLocationData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AnimalData> __Game_Prefabs_AnimalData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<SpawnLocationElement> __Game_Buildings_SpawnLocationElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> __Game_Areas_Node_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Triangle> __Game_Areas_Triangle_RO_BufferLookup;

		public ComponentLookup<Transform> __Game_Objects_Transform_RW_ComponentLookup;

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
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Objects_TripSource_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TripSource>(true);
			__Game_Objects_Unspawned_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Unspawned>(true);
			__Game_Common_PseudoRandomSeed_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PseudoRandomSeed>(true);
			__Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentVehicle>(true);
			__Game_Pathfind_PathOwner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathOwner>(true);
			__Game_Pathfind_PathElement_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<PathElement>(true);
			__Game_Creatures_Human_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Human>(false);
			__Game_Creatures_Animal_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Animal>(false);
			__Game_Creatures_HumanCurrentLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HumanCurrentLane>(false);
			__Game_Creatures_AnimalCurrentLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AnimalCurrentLane>(false);
			__Game_Creatures_HumanNavigation_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HumanNavigation>(false);
			__Game_Creatures_AnimalNavigation_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AnimalNavigation>(false);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Creatures_Resident_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Resident>(true);
			__Game_Citizens_HouseholdMember_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HouseholdMember>(true);
			__Game_Citizens_HomelessHousehold_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HomelessHousehold>(true);
			__Game_Citizens_Worker_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Worker>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_ConnectionLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ConnectionLane>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_SpawnLocationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnLocationData>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_AnimalData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AnimalData>(true);
			__Game_Buildings_SpawnLocationElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SpawnLocationElement>(true);
			__Game_Areas_Node_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.Node>(true);
			__Game_Areas_Triangle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Triangle>(true);
			__Game_Objects_Transform_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(false);
		}
	}

	private ModificationBarrier5 m_ModificationBarrier;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private ClimateSystem m_ClimateSystem;

	private EntityQuery m_CreatureQuery;

	private ComponentTypeSet m_TripSourceRemoveTypes;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier5>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_ClimateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ClimateSystem>();
		m_CreatureQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Creature>(),
			ComponentType.ReadOnly<Updated>()
		});
		m_TripSourceRemoveTypes = new ComponentTypeSet(ComponentType.ReadWrite<TripSource>(), ComponentType.ReadWrite<Unspawned>());
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
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		InitializeCreaturesJob initializeCreaturesJob = new InitializeCreaturesJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TripSourceType = InternalCompilerInterface.GetComponentTypeHandle<TripSource>(ref __TypeHandle.__Game_Objects_TripSource_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UnspawnedType = InternalCompilerInterface.GetComponentTypeHandle<Unspawned>(ref __TypeHandle.__Game_Objects_Unspawned_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PseudoRandomSeedType = InternalCompilerInterface.GetComponentTypeHandle<PseudoRandomSeed>(ref __TypeHandle.__Game_Common_PseudoRandomSeed_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentVehicleType = InternalCompilerInterface.GetComponentTypeHandle<CurrentVehicle>(ref __TypeHandle.__Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathOwnerType = InternalCompilerInterface.GetComponentTypeHandle<PathOwner>(ref __TypeHandle.__Game_Pathfind_PathOwner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathElementType = InternalCompilerInterface.GetBufferTypeHandle<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HumanType = InternalCompilerInterface.GetComponentTypeHandle<Human>(ref __TypeHandle.__Game_Creatures_Human_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AnimalType = InternalCompilerInterface.GetComponentTypeHandle<Animal>(ref __TypeHandle.__Game_Creatures_Animal_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HumanCurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<HumanCurrentLane>(ref __TypeHandle.__Game_Creatures_HumanCurrentLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AnimalCurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<AnimalCurrentLane>(ref __TypeHandle.__Game_Creatures_AnimalCurrentLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HumanNavigationType = InternalCompilerInterface.GetComponentTypeHandle<HumanNavigation>(ref __TypeHandle.__Game_Creatures_HumanNavigation_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AnimalNavigationType = InternalCompilerInterface.GetComponentTypeHandle<AnimalNavigation>(ref __TypeHandle.__Game_Creatures_AnimalNavigation_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResidentData = InternalCompilerInterface.GetComponentLookup<Resident>(ref __TypeHandle.__Game_Creatures_Resident_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdMemberData = InternalCompilerInterface.GetComponentLookup<HouseholdMember>(ref __TypeHandle.__Game_Citizens_HouseholdMember_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HomelessHousehold = InternalCompilerInterface.GetComponentLookup<HomelessHousehold>(ref __TypeHandle.__Game_Citizens_HomelessHousehold_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WorkerData = InternalCompilerInterface.GetComponentLookup<Worker>(ref __TypeHandle.__Game_Citizens_Worker_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyRenterData = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectionLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnLocationData = InternalCompilerInterface.GetComponentLookup<SpawnLocationData>(ref __TypeHandle.__Game_Prefabs_SpawnLocationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AnimalData = InternalCompilerInterface.GetComponentLookup<AnimalData>(ref __TypeHandle.__Game_Prefabs_AnimalData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnLocations = InternalCompilerInterface.GetBufferLookup<SpawnLocationElement>(ref __TypeHandle.__Game_Buildings_SpawnLocationElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaNodes = InternalCompilerInterface.GetBufferLookup<Game.Areas.Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaTriangles = InternalCompilerInterface.GetBufferLookup<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RandomSeed = RandomSeed.Next(),
			m_TripSourceRemoveTypes = m_TripSourceRemoveTypes,
			m_Temperature = m_ClimateSystem.temperature,
			m_LeftHandTraffic = m_CityConfigurationSystem.leftHandTraffic
		};
		EntityCommandBuffer val = m_ModificationBarrier.CreateCommandBuffer();
		initializeCreaturesJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		InitializeCreaturesJob initializeCreaturesJob2 = initializeCreaturesJob;
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<InitializeCreaturesJob>(initializeCreaturesJob2, m_CreatureQuery, ((SystemBase)this).Dependency);
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
	public InitializeSystem()
	{
	}
}
