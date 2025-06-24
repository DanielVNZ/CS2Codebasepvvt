using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Agents;
using Game.Buildings;
using Game.Citizens;
using Game.Common;
using Game.Companies;
using Game.Debug;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Tools;
using Game.Triggers;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class FindJobSystem : GameSystemBase
{
	[BurstCompile]
	private struct CalculateFreeWorkplaceJob : IJob
	{
		[ReadOnly]
		public NativeList<FreeWorkplaces> m_FreeWorkplaces;

		public NativeArray<int> m_FreeCache;

		public void Execute()
		{
			for (int i = 0; i < m_FreeCache.Length; i++)
			{
				m_FreeCache[i] = 0;
			}
			for (int j = 0; j < m_FreeWorkplaces.Length; j++)
			{
				FreeWorkplaces freeWorkplaces = m_FreeWorkplaces[j];
				ref NativeArray<int> reference = ref m_FreeCache;
				reference[0] = reference[0] + freeWorkplaces.m_Uneducated;
				reference = ref m_FreeCache;
				reference[1] = reference[1] + freeWorkplaces.m_PoorlyEducated;
				reference = ref m_FreeCache;
				reference[2] = reference[2] + freeWorkplaces.m_Educated;
				reference = ref m_FreeCache;
				reference[3] = reference[3] + freeWorkplaces.m_WellEducated;
				reference = ref m_FreeCache;
				reference[4] = reference[4] + freeWorkplaces.m_HighlyEducated;
			}
		}
	}

	[BurstCompile]
	private struct FindJobJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<JobSeeker> m_JobSeekerType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentBuilding> m_CurrentBuildingType;

		[ReadOnly]
		public ComponentLookup<HouseholdMember> m_HouseholdMembers;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenters;

		[ReadOnly]
		public ComponentLookup<Citizen> m_CitizenDatas;

		[ReadOnly]
		public ComponentLookup<Worker> m_Workers;

		[ReadOnly]
		public ComponentLookup<Household> m_Households;

		[ReadOnly]
		public ComponentLookup<HomelessHousehold> m_HomelessHouseholds;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> m_OutsideConnections;

		[ReadOnly]
		public ComponentLookup<Deleted> m_Deleteds;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> m_HouseholdCitizens;

		[ReadOnly]
		public BufferLookup<OwnedVehicle> m_OwnedVehicles;

		public ParallelWriter<SetupQueueItem> m_PathfindQueue;

		public ParallelWriter m_CommandBuffer;

		[ReadOnly]
		public NativeArray<int> m_FreeCache;

		[ReadOnly]
		public NativeArray<int> m_EmployableByEducation;

		public RandomSeed m_RandomSeed;

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
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_041f: Unknown result type (might be due to invalid IL or missing references)
			//IL_042e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Owner> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<JobSeeker> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<JobSeeker>(ref m_JobSeekerType);
			NativeArray<CurrentBuilding> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentBuilding>(ref m_CurrentBuildingType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity owner = nativeArray2[i].m_Owner;
				if (m_Deleteds.HasComponent(owner) || !m_CitizenDatas.HasComponent(owner))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, nativeArray[i], default(Deleted));
					continue;
				}
				Entity household = m_HouseholdMembers[owner].m_Household;
				Citizen citizen = m_CitizenDatas[owner];
				Entity val = Entity.Null;
				if (m_PropertyRenters.HasComponent(household))
				{
					val = m_PropertyRenters[household].m_Property;
				}
				else if (((ArchetypeChunk)(ref chunk)).Has<CurrentBuilding>(ref m_CurrentBuildingType))
				{
					if ((citizen.m_State & CitizenFlags.Commuter) != CitizenFlags.None)
					{
						val = nativeArray4[i].m_CurrentBuilding;
					}
				}
				else if (m_HomelessHouseholds.HasComponent(household))
				{
					val = m_HomelessHouseholds[household].m_TempHome;
				}
				Entity val2 = nativeArray[i];
				if (val != Entity.Null)
				{
					int level = nativeArray3[i].m_Level;
					int num = level;
					int num2 = -1;
					bool flag = m_Workers.HasComponent(owner) && m_OutsideConnections.HasComponent(m_Workers[owner].m_Workplace);
					if (m_Workers.HasComponent(owner) && !flag)
					{
						num2 = m_Workers[owner].m_Level;
					}
					if (num2 >= 0 && num > level && num <= num2)
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponentEnabled<HasJobSeeker>(unfilteredChunkIndex, owner, false);
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, val2, default(Deleted));
						continue;
					}
					while (num > num2 && m_FreeCache[num] <= 0)
					{
						num--;
					}
					if (num == -1)
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponentEnabled<HasJobSeeker>(unfilteredChunkIndex, owner, false);
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, val2, default(Deleted));
						continue;
					}
					float num3 = m_FreeCache[num];
					float num4 = (float)m_EmployableByEducation[num] / num3;
					if (num2 >= 0 && ((Random)(ref random)).NextFloat(num4) > 2f)
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponentEnabled<HasJobSeeker>(unfilteredChunkIndex, owner, false);
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, val2, default(Deleted));
						continue;
					}
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathInformation>(unfilteredChunkIndex, val2, new PathInformation
					{
						m_State = PathFlags.Pending
					});
					Household household2 = m_Households[household];
					DynamicBuffer<HouseholdCitizen> val3 = m_HouseholdCitizens[household];
					PathfindParameters parameters = new PathfindParameters
					{
						m_MaxSpeed = float2.op_Implicit(111.111115f),
						m_WalkSpeed = float2.op_Implicit(1.6666667f),
						m_Weights = CitizenUtils.GetPathfindWeights(citizen, household2, val3.Length),
						m_Methods = (PathMethod.Pedestrian | PathMethod.PublicTransportDay | PathMethod.PublicTransportNight),
						m_MaxCost = CitizenBehaviorSystem.kMaxPathfindCost,
						m_PathfindFlags = (PathfindFlags.Simplified | PathfindFlags.IgnorePath)
					};
					SetupQueueTarget origin = new SetupQueueTarget
					{
						m_Type = SetupTargetType.CurrentLocation,
						m_Methods = PathMethod.Pedestrian
					};
					SetupQueueTarget destination = new SetupQueueTarget
					{
						m_Type = SetupTargetType.JobSeekerTo,
						m_Methods = PathMethod.Pedestrian,
						m_Value = level + 5 * (num + 1),
						m_Value2 = (flag ? 0f : num4)
					};
					if (nativeArray3[i].m_Outside > 0)
					{
						destination.m_Flags |= SetupTargetFlags.Export;
					}
					if (flag)
					{
						destination.m_Flags |= SetupTargetFlags.Import;
					}
					PathUtils.UpdateOwnedVehicleMethods(household, ref m_OwnedVehicles, ref parameters, ref origin, ref destination);
					SetupQueueItem setupQueueItem = new SetupQueueItem(val2, parameters, origin, destination);
					m_PathfindQueue.Enqueue(setupQueueItem);
				}
				else
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponentEnabled<HasJobSeeker>(unfilteredChunkIndex, owner, false);
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, val2, default(Deleted));
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct StartWorkingJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<JobSeeker> m_JobSeekerType;

		[ReadOnly]
		public ComponentTypeHandle<PathInformation> m_PathInfoType;

		[ReadOnly]
		public ComponentLookup<Citizen> m_Citizens;

		[ReadOnly]
		public ComponentLookup<Deleted> m_Deleteds;

		public BufferLookup<Employee> m_EmployeeBuffers;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<WorkplaceData> m_WorkplaceDatas;

		public ComponentLookup<FreeWorkplaces> m_FreeWorkplaces;

		[ReadOnly]
		public ComponentLookup<Worker> m_Workers;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> m_SpawnableBuildings;

		[ReadOnly]
		public ComponentLookup<WorkProvider> m_WorkProviders;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenters;

		public NativeQueue<TriggerAction> m_TriggerBuffer;

		public EntityCommandBuffer m_CommandBuffer;

		public uint m_SimulationFrame;

		public NativeValue<int> m_StartedWorking;

		public void Execute()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			Random val4 = default(Random);
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				ArchetypeChunk val = m_Chunks[i];
				NativeArray<Owner> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<Owner>(ref m_OwnerType);
				NativeArray<PathInformation> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<PathInformation>(ref m_PathInfoType);
				NativeArray<Entity> nativeArray3 = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				NativeArray<JobSeeker> nativeArray4 = ((ArchetypeChunk)(ref val)).GetNativeArray<JobSeeker>(ref m_JobSeekerType);
				for (int j = 0; j < nativeArray3.Length; j++)
				{
					if ((nativeArray2[j].m_State & PathFlags.Pending) != 0)
					{
						continue;
					}
					Entity val2 = nativeArray3[j];
					Entity owner = nativeArray[j].m_Owner;
					if (m_Citizens.HasComponent(owner) && !m_Deleteds.HasComponent(owner))
					{
						Entity destination = nativeArray2[j].m_Destination;
						if (m_Prefabs.HasComponent(destination) && m_EmployeeBuffers.HasBuffer(destination))
						{
							DynamicBuffer<Employee> employees = m_EmployeeBuffers[destination];
							WorkProvider workProvider = m_WorkProviders[destination];
							Entity val3 = (m_PropertyRenters.HasComponent(destination) ? m_PropertyRenters[destination].m_Property : destination);
							Entity prefab = m_Prefabs[val3].m_Prefab;
							int level = ((!m_SpawnableBuildings.HasComponent(prefab)) ? 1 : m_SpawnableBuildings[prefab].m_Level);
							if (m_Prefabs.HasComponent(destination) && (!m_Workers.HasComponent(owner) || destination != m_Workers[owner].m_Workplace))
							{
								Entity prefab2 = m_Prefabs[destination].m_Prefab;
								if (m_WorkplaceDatas.HasComponent(prefab2))
								{
									if (m_FreeWorkplaces.HasComponent(destination) && m_FreeWorkplaces[destination].Count > 0)
									{
										WorkplaceData workplaceData = m_WorkplaceDatas[prefab2];
										Citizen citizen = m_Citizens[owner];
										Workshift shift = Workshift.Day;
										FreeWorkplaces freeWorkplaces = m_FreeWorkplaces[destination];
										freeWorkplaces.Refresh(employees, workProvider.m_MaxWorkers, workplaceData.m_Complexity, level);
										byte level2 = nativeArray4[j].m_Level;
										int bestFor = freeWorkplaces.GetBestFor(level2);
										if (bestFor >= 0)
										{
											((Random)(ref val4))._002Ector(1 + (m_SimulationFrame ^ citizen.m_PseudoRandom));
											float num2 = ((Random)(ref val4)).NextFloat();
											if (num2 < workplaceData.m_EveningShiftProbability)
											{
												shift = Workshift.Evening;
											}
											else if (num2 < workplaceData.m_EveningShiftProbability + workplaceData.m_NightShiftProbability)
											{
												shift = Workshift.Night;
											}
											employees.Add(new Employee
											{
												m_Worker = owner,
												m_Level = (byte)bestFor
											});
											if (m_Workers.HasComponent(owner))
											{
												((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Worker>(owner);
											}
											((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Worker>(owner, new Worker
											{
												m_Workplace = destination,
												m_Level = (byte)bestFor,
												m_LastCommuteTime = nativeArray2[j].m_Duration,
												m_Shift = shift
											});
											num++;
											m_TriggerBuffer.Enqueue(new TriggerAction(TriggerType.CitizenStartedWorking, Entity.Null, owner, destination));
											freeWorkplaces.Refresh(employees, workProvider.m_MaxWorkers, workplaceData.m_Complexity, level);
											m_FreeWorkplaces[destination] = freeWorkplaces;
										}
									}
									else if (!m_Workers.HasComponent(owner))
									{
									}
								}
							}
						}
						else if (CitizenUtils.IsCommuter(owner, ref m_Citizens))
						{
							((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Deleted>(owner, default(Deleted));
							continue;
						}
						((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponentEnabled<HasJobSeeker>(owner, false);
					}
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Deleted>(val2, default(Deleted));
				}
			}
			ref NativeValue<int> reference = ref m_StartedWorking;
			reference.value += num;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		public ComponentTypeHandle<JobSeeker> __Game_Agents_JobSeeker_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Owner> __Game_Common_Owner_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentBuilding> __Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<HouseholdMember> __Game_Citizens_HouseholdMember_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Worker> __Game_Citizens_Worker_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Household> __Game_Citizens_Household_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HomelessHousehold> __Game_Citizens_HomelessHousehold_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> __Game_Objects_OutsideConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<OwnedVehicle> __Game_Vehicles_OwnedVehicle_RO_BufferLookup;

		[ReadOnly]
		public ComponentTypeHandle<JobSeeker> __Game_Agents_JobSeeker_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		public BufferLookup<Employee> __Game_Companies_Employee_RW_BufferLookup;

		public ComponentLookup<FreeWorkplaces> __Game_Companies_FreeWorkplaces_RW_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WorkplaceData> __Game_Prefabs_WorkplaceData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WorkProvider> __Game_Companies_WorkProvider_RO_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Agents_JobSeeker_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<JobSeeker>(false);
			__Game_Common_Owner_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(false);
			__Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentBuilding>(true);
			__Game_Citizens_HouseholdMember_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HouseholdMember>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
			__Game_Citizens_Worker_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Worker>(true);
			__Game_Citizens_Household_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Household>(true);
			__Game_Citizens_HomelessHousehold_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HomelessHousehold>(true);
			__Game_Objects_OutsideConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.OutsideConnection>(true);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Citizens_HouseholdCitizen_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdCitizen>(true);
			__Game_Vehicles_OwnedVehicle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<OwnedVehicle>(true);
			__Game_Agents_JobSeeker_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<JobSeeker>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Pathfind_PathInformation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathInformation>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Companies_Employee_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Employee>(false);
			__Game_Companies_FreeWorkplaces_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<FreeWorkplaces>(false);
			__Game_Prefabs_WorkplaceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WorkplaceData>(true);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableBuildingData>(true);
			__Game_Companies_WorkProvider_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WorkProvider>(true);
		}
	}

	private const int UPDATE_INTERVAL = 16;

	private EntityQuery m_JobSeekerQuery;

	private EntityQuery m_ResultsQuery;

	private EntityQuery m_FreeQuery;

	private SimulationSystem m_SimulationSystem;

	private TriggerSystem m_TriggerSystem;

	private CountHouseholdDataSystem m_CountHouseholdDataSystem;

	private PathfindSetupSystem m_PathfindSetupSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private NativeArray<int> m_FreeCache;

	[DebugWatchValue]
	private NativeValue<int> m_StartedWorking;

	[DebugWatchDeps]
	private JobHandle m_WriteDeps;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_PathfindSetupSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindSetupSystem>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_TriggerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TriggerSystem>();
		m_CountHouseholdDataSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CountHouseholdDataSystem>();
		m_FreeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<FreeWorkplaces>(),
			ComponentType.Exclude<Destroyed>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_StartedWorking = new NativeValue<int>((Allocator)4);
		m_JobSeekerQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadWrite<JobSeeker>(),
			ComponentType.ReadOnly<Owner>(),
			ComponentType.Exclude<PathInformation>(),
			ComponentType.Exclude<Deleted>()
		});
		m_ResultsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadWrite<JobSeeker>(),
			ComponentType.ReadOnly<Owner>(),
			ComponentType.ReadOnly<PathInformation>(),
			ComponentType.Exclude<Deleted>()
		});
		m_FreeCache = new NativeArray<int>(5, (Allocator)4, (NativeArrayOptions)1);
		((ComponentSystemBase)this).RequireAnyForUpdate((EntityQuery[])(object)new EntityQuery[2] { m_JobSeekerQuery, m_ResultsQuery });
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_StartedWorking.Dispose();
		m_FreeCache.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_049b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0507: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_0519: Unknown result type (might be due to invalid IL or missing references)
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_JobSeekerQuery)).IsEmptyIgnoreFilter && !m_CountHouseholdDataSystem.IsCountDataNotReady())
		{
			JobHandle val = IJobExtensions.Schedule<CalculateFreeWorkplaceJob>(new CalculateFreeWorkplaceJob
			{
				m_FreeWorkplaces = ((EntityQuery)(ref m_FreeQuery)).ToComponentDataListAsync<FreeWorkplaces>(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val),
				m_FreeCache = m_FreeCache
			}, JobHandle.CombineDependencies(val, ((SystemBase)this).Dependency));
			FindJobJob findJobJob = new FindJobJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_JobSeekerType = InternalCompilerInterface.GetComponentTypeHandle<JobSeeker>(ref __TypeHandle.__Game_Agents_JobSeeker_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CurrentBuildingType = InternalCompilerInterface.GetComponentTypeHandle<CurrentBuilding>(ref __TypeHandle.__Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_HouseholdMembers = InternalCompilerInterface.GetComponentLookup<HouseholdMember>(ref __TypeHandle.__Game_Citizens_HouseholdMember_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PropertyRenters = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CitizenDatas = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Workers = InternalCompilerInterface.GetComponentLookup<Worker>(ref __TypeHandle.__Game_Citizens_Worker_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Households = InternalCompilerInterface.GetComponentLookup<Household>(ref __TypeHandle.__Game_Citizens_Household_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HomelessHouseholds = InternalCompilerInterface.GetComponentLookup<HomelessHousehold>(ref __TypeHandle.__Game_Citizens_HomelessHousehold_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OutsideConnections = InternalCompilerInterface.GetComponentLookup<Game.Objects.OutsideConnection>(ref __TypeHandle.__Game_Objects_OutsideConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Deleteds = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HouseholdCitizens = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OwnedVehicles = InternalCompilerInterface.GetBufferLookup<OwnedVehicle>(ref __TypeHandle.__Game_Vehicles_OwnedVehicle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PathfindQueue = m_PathfindSetupSystem.GetQueue(this, 80, 16).AsParallelWriter()
			};
			EntityCommandBuffer val2 = m_EndFrameBarrier.CreateCommandBuffer();
			findJobJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
			findJobJob.m_FreeCache = m_FreeCache;
			findJobJob.m_EmployableByEducation = m_CountHouseholdDataSystem.GetEmployables();
			findJobJob.m_RandomSeed = RandomSeed.Next();
			FindJobJob findJobJob2 = findJobJob;
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<FindJobJob>(findJobJob2, m_JobSeekerQuery, JobHandle.CombineDependencies(val, ((SystemBase)this).Dependency));
			m_PathfindSetupSystem.AddQueueWriter(((SystemBase)this).Dependency);
			m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
		}
		if (!((EntityQuery)(ref m_ResultsQuery)).IsEmptyIgnoreFilter)
		{
			JobHandle val3 = default(JobHandle);
			StartWorkingJob startWorkingJob = new StartWorkingJob
			{
				m_Chunks = ((EntityQuery)(ref m_ResultsQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val3),
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_JobSeekerType = InternalCompilerInterface.GetComponentTypeHandle<JobSeeker>(ref __TypeHandle.__Game_Agents_JobSeeker_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PathInfoType = InternalCompilerInterface.GetComponentTypeHandle<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_Citizens = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EmployeeBuffers = InternalCompilerInterface.GetBufferLookup<Employee>(ref __TypeHandle.__Game_Companies_Employee_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_FreeWorkplaces = InternalCompilerInterface.GetComponentLookup<FreeWorkplaces>(ref __TypeHandle.__Game_Companies_FreeWorkplaces_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_WorkplaceDatas = InternalCompilerInterface.GetComponentLookup<WorkplaceData>(ref __TypeHandle.__Game_Prefabs_WorkplaceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Deleteds = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Workers = InternalCompilerInterface.GetComponentLookup<Worker>(ref __TypeHandle.__Game_Citizens_Worker_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PropertyRenters = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SpawnableBuildings = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_WorkProviders = InternalCompilerInterface.GetComponentLookup<WorkProvider>(ref __TypeHandle.__Game_Companies_WorkProvider_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TriggerBuffer = m_TriggerSystem.CreateActionBuffer(),
				m_SimulationFrame = m_SimulationSystem.frameIndex,
				m_CommandBuffer = m_EndFrameBarrier.CreateCommandBuffer(),
				m_StartedWorking = m_StartedWorking
			};
			((SystemBase)this).Dependency = IJobExtensions.Schedule<StartWorkingJob>(startWorkingJob, JobHandle.CombineDependencies(val3, ((SystemBase)this).Dependency));
			m_TriggerSystem.AddActionBufferWriter(((SystemBase)this).Dependency);
			m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
			m_WriteDeps = JobHandle.CombineDependencies(((SystemBase)this).Dependency, m_WriteDeps);
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
	public FindJobSystem()
	{
	}
}
