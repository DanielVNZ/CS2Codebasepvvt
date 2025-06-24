using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Game.Buildings;
using Game.Citizens;
using Game.Common;
using Game.Events;
using Game.Net;
using Game.Notifications;
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
using UnityEngine.Assertions;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class AccidentSiteSystem : GameSystemBase
{
	[BurstCompile]
	private struct AccidentSiteJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Building> m_BuildingType;

		public ComponentTypeHandle<AccidentSite> m_AccidentSiteType;

		[ReadOnly]
		public ComponentLookup<InvolvedInAccident> m_InvolvedInAccidentData;

		[ReadOnly]
		public ComponentLookup<Criminal> m_CriminalData;

		[ReadOnly]
		public ComponentLookup<PoliceEmergencyRequest> m_PoliceEmergencyRequestData;

		[ReadOnly]
		public ComponentLookup<Moving> m_MovingData;

		[ReadOnly]
		public ComponentLookup<Vehicle> m_VehicleData;

		[ReadOnly]
		public ComponentLookup<Car> m_CarData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<TrafficAccidentData> m_PrefabTrafficAccidentData;

		[ReadOnly]
		public ComponentLookup<CrimeData> m_PrefabCrimeData;

		[ReadOnly]
		public BufferLookup<TargetElement> m_TargetElements;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		[ReadOnly]
		public BufferLookup<LaneObject> m_LaneObjects;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public uint m_SimulationFrame;

		[ReadOnly]
		public EntityArchetype m_PoliceRequestArchetype;

		[ReadOnly]
		public EntityArchetype m_EventImpactArchetype;

		[ReadOnly]
		public PoliceConfigurationData m_PoliceConfigurationData;

		public ParallelWriter m_CommandBuffer;

		public IconCommandBuffer m_IconCommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_041d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0430: Unknown result type (might be due to invalid IL or missing references)
			//IL_043c: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0509: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_0521: Unknown result type (might be due to invalid IL or missing references)
			//IL_0529: Unknown result type (might be due to invalid IL or missing references)
			//IL_0530: Unknown result type (might be due to invalid IL or missing references)
			//IL_0536: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<AccidentSite> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AccidentSite>(ref m_AccidentSiteType);
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<Building>(ref m_BuildingType);
			InvolvedInAccident involvedInAccident = default(InvolvedInAccident);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Entity val = nativeArray[i];
				AccidentSite accidentSite = nativeArray2[i];
				Random random = m_RandomSeed.GetRandom(val.Index);
				Entity val2 = Entity.Null;
				int num = 0;
				float num2 = 0f;
				if (m_SimulationFrame - accidentSite.m_CreationFrame >= 3600)
				{
					accidentSite.m_Flags &= ~AccidentSiteFlags.StageAccident;
				}
				accidentSite.m_Flags &= ~AccidentSiteFlags.MovingVehicles;
				if (m_TargetElements.HasBuffer(accidentSite.m_Event))
				{
					DynamicBuffer<TargetElement> val3 = m_TargetElements[accidentSite.m_Event];
					for (int j = 0; j < val3.Length; j++)
					{
						Entity entity = val3[j].m_Entity;
						if (m_InvolvedInAccidentData.TryGetComponent(entity, ref involvedInAccident))
						{
							if (involvedInAccident.m_Event == accidentSite.m_Event)
							{
								num++;
								bool flag2 = m_MovingData.HasComponent(entity);
								if (flag2 && (accidentSite.m_Flags & AccidentSiteFlags.MovingVehicles) == 0 && m_VehicleData.HasComponent(entity))
								{
									accidentSite.m_Flags |= AccidentSiteFlags.MovingVehicles;
								}
								if (involvedInAccident.m_Severity > num2)
								{
									val2 = (flag2 ? Entity.Null : entity);
									num2 = involvedInAccident.m_Severity;
									accidentSite.m_Flags &= ~AccidentSiteFlags.StageAccident;
								}
							}
						}
						else
						{
							if (!m_CriminalData.HasComponent(entity))
							{
								continue;
							}
							Criminal criminal = m_CriminalData[entity];
							if (criminal.m_Event == accidentSite.m_Event && (criminal.m_Flags & CriminalFlags.Arrested) == 0)
							{
								num++;
								if ((criminal.m_Flags & CriminalFlags.Monitored) != 0)
								{
									accidentSite.m_Flags |= AccidentSiteFlags.CrimeMonitored;
								}
							}
						}
					}
					if (num == 0 && (accidentSite.m_Flags & AccidentSiteFlags.StageAccident) != 0)
					{
						PrefabRef prefabRef = m_PrefabRefData[accidentSite.m_Event];
						if (m_PrefabTrafficAccidentData.HasComponent(prefabRef.m_Prefab))
						{
							TrafficAccidentData trafficAccidentData = m_PrefabTrafficAccidentData[prefabRef.m_Prefab];
							Entity val4 = TryFindSubject(val, ref random, trafficAccidentData);
							if (val4 != Entity.Null)
							{
								AddImpact(unfilteredChunkIndex, accidentSite.m_Event, ref random, val4, trafficAccidentData);
							}
						}
					}
				}
				if ((accidentSite.m_Flags & (AccidentSiteFlags.CrimeScene | AccidentSiteFlags.CrimeDetected)) == AccidentSiteFlags.CrimeScene)
				{
					PrefabRef prefabRef2 = m_PrefabRefData[accidentSite.m_Event];
					if (m_PrefabCrimeData.HasComponent(prefabRef2.m_Prefab))
					{
						CrimeData crimeData = m_PrefabCrimeData[prefabRef2.m_Prefab];
						float num3 = (float)(m_SimulationFrame - accidentSite.m_CreationFrame) / 60f;
						if ((accidentSite.m_Flags & AccidentSiteFlags.CrimeMonitored) != 0 || num3 >= crimeData.m_AlarmDelay.max)
						{
							accidentSite.m_Flags |= AccidentSiteFlags.CrimeDetected;
						}
						else if (num3 >= crimeData.m_AlarmDelay.min)
						{
							float num4 = 1.0666667f / (crimeData.m_AlarmDelay.max - crimeData.m_AlarmDelay.min);
							if (((Random)(ref random)).NextFloat(1f) <= num4)
							{
								accidentSite.m_Flags |= AccidentSiteFlags.CrimeDetected;
							}
						}
					}
					if ((accidentSite.m_Flags & AccidentSiteFlags.CrimeDetected) != 0)
					{
						m_IconCommandBuffer.Add(val, m_PoliceConfigurationData.m_CrimeSceneNotificationPrefab, IconPriority.MajorProblem, IconClusterLayer.Default, IconFlags.IgnoreTarget, accidentSite.m_Event);
					}
				}
				else if ((accidentSite.m_Flags & (AccidentSiteFlags.CrimeScene | AccidentSiteFlags.CrimeFinished)) == AccidentSiteFlags.CrimeScene)
				{
					PrefabRef prefabRef3 = m_PrefabRefData[accidentSite.m_Event];
					if (m_PrefabCrimeData.HasComponent(prefabRef3.m_Prefab))
					{
						CrimeData crimeData2 = m_PrefabCrimeData[prefabRef3.m_Prefab];
						float num5 = (float)(m_SimulationFrame - accidentSite.m_CreationFrame) / 60f;
						if (num5 >= crimeData2.m_CrimeDuration.max)
						{
							accidentSite.m_Flags |= AccidentSiteFlags.CrimeFinished;
						}
						else if (num5 >= crimeData2.m_CrimeDuration.min)
						{
							float num6 = 1.0666667f / (crimeData2.m_CrimeDuration.max - crimeData2.m_CrimeDuration.min);
							if (((Random)(ref random)).NextFloat(1f) <= num6)
							{
								accidentSite.m_Flags |= AccidentSiteFlags.CrimeFinished;
							}
						}
					}
				}
				accidentSite.m_Flags &= ~AccidentSiteFlags.RequirePolice;
				if (num2 > 0f || (accidentSite.m_Flags & (AccidentSiteFlags.Secured | AccidentSiteFlags.CrimeScene)) == AccidentSiteFlags.CrimeScene)
				{
					if (num2 > 0f || (accidentSite.m_Flags & AccidentSiteFlags.CrimeDetected) != 0)
					{
						if (flag)
						{
							val2 = val;
						}
						if (val2 != Entity.Null)
						{
							accidentSite.m_Flags |= AccidentSiteFlags.RequirePolice;
							RequestPoliceIfNeeded(unfilteredChunkIndex, val, ref accidentSite, val2, num2);
						}
					}
				}
				else if (num == 0 && ((accidentSite.m_Flags & (AccidentSiteFlags.Secured | AccidentSiteFlags.CrimeScene)) != (AccidentSiteFlags.Secured | AccidentSiteFlags.CrimeScene) || m_SimulationFrame >= accidentSite.m_SecuredFrame + 1024))
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<AccidentSite>(unfilteredChunkIndex, val);
					if ((accidentSite.m_Flags & AccidentSiteFlags.CrimeScene) != 0)
					{
						m_IconCommandBuffer.Remove(val, m_PoliceConfigurationData.m_CrimeSceneNotificationPrefab);
					}
				}
				nativeArray2[i] = accidentSite;
			}
		}

		private Entity TryFindSubject(Entity entity, ref Random random, TrafficAccidentData trafficAccidentData)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			Entity result = Entity.Null;
			int num = 0;
			if (m_SubLanes.HasBuffer(entity))
			{
				DynamicBuffer<Game.Net.SubLane> val = m_SubLanes[entity];
				for (int i = 0; i < val.Length; i++)
				{
					Entity subLane = val[i].m_SubLane;
					if (!m_LaneObjects.HasBuffer(subLane))
					{
						continue;
					}
					DynamicBuffer<LaneObject> val2 = m_LaneObjects[subLane];
					for (int j = 0; j < val2.Length; j++)
					{
						Entity laneObject = val2[j].m_LaneObject;
						if (trafficAccidentData.m_SubjectType == EventTargetType.MovingCar && m_CarData.HasComponent(laneObject) && m_MovingData.HasComponent(laneObject) && !m_InvolvedInAccidentData.HasComponent(laneObject))
						{
							num++;
							if (((Random)(ref random)).NextInt(num) == num - 1)
							{
								result = laneObject;
							}
						}
					}
				}
			}
			return result;
		}

		private void RequestPoliceIfNeeded(int jobIndex, Entity entity, ref AccidentSite accidentSite, Entity target, float severity)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			if (!m_PoliceEmergencyRequestData.HasComponent(accidentSite.m_PoliceRequest))
			{
				PolicePurpose purpose = (((accidentSite.m_Flags & AccidentSiteFlags.CrimeMonitored) == 0) ? PolicePurpose.Emergency : PolicePurpose.Intelligence);
				Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_PoliceRequestArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PoliceEmergencyRequest>(jobIndex, val, new PoliceEmergencyRequest(entity, target, severity, purpose));
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RequestGroup>(jobIndex, val, new RequestGroup(4u));
			}
		}

		private void AddImpact(int jobIndex, Entity eventEntity, ref Random random, Entity target, TrafficAccidentData trafficAccidentData)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			Impact impact = new Impact
			{
				m_Event = eventEntity,
				m_Target = target
			};
			if (trafficAccidentData.m_AccidentType == TrafficAccidentType.LoseControl && m_MovingData.HasComponent(target))
			{
				Moving moving = m_MovingData[target];
				impact.m_Severity = 5f;
				if (((Random)(ref random)).NextBool())
				{
					impact.m_AngularVelocityDelta.y = -2f;
					((float3)(ref impact.m_VelocityDelta)).xz = impact.m_Severity * MathUtils.Left(math.normalizesafe(((float3)(ref moving.m_Velocity)).xz, default(float2)));
				}
				else
				{
					impact.m_AngularVelocityDelta.y = 2f;
					((float3)(ref impact.m_VelocityDelta)).xz = impact.m_Severity * MathUtils.Right(math.normalizesafe(((float3)(ref moving.m_Velocity)).xz, default(float2)));
				}
			}
			Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_EventImpactArchetype);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Impact>(jobIndex, val, impact);
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
		public ComponentTypeHandle<Building> __Game_Buildings_Building_RO_ComponentTypeHandle;

		public ComponentTypeHandle<AccidentSite> __Game_Events_AccidentSite_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<InvolvedInAccident> __Game_Events_InvolvedInAccident_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Criminal> __Game_Citizens_Criminal_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PoliceEmergencyRequest> __Game_Simulation_PoliceEmergencyRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Moving> __Game_Objects_Moving_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Vehicle> __Game_Vehicles_Vehicle_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Car> __Game_Vehicles_Car_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrafficAccidentData> __Game_Prefabs_TrafficAccidentData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CrimeData> __Game_Prefabs_CrimeData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<TargetElement> __Game_Events_TargetElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LaneObject> __Game_Net_LaneObject_RO_BufferLookup;

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
			__Game_Buildings_Building_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Building>(true);
			__Game_Events_AccidentSite_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AccidentSite>(false);
			__Game_Events_InvolvedInAccident_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<InvolvedInAccident>(true);
			__Game_Citizens_Criminal_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Criminal>(true);
			__Game_Simulation_PoliceEmergencyRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PoliceEmergencyRequest>(true);
			__Game_Objects_Moving_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Moving>(true);
			__Game_Vehicles_Vehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Vehicle>(true);
			__Game_Vehicles_Car_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Car>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_TrafficAccidentData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrafficAccidentData>(true);
			__Game_Prefabs_CrimeData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CrimeData>(true);
			__Game_Events_TargetElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TargetElement>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Net_LaneObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneObject>(true);
		}
	}

	private const uint UPDATE_INTERVAL = 64u;

	private SimulationSystem m_SimulationSystem;

	private IconCommandSystem m_IconCommandSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_AccidentQuery;

	private EntityQuery m_ConfigQuery;

	private EntityArchetype m_PoliceRequestArchetype;

	private EntityArchetype m_EventImpactArchetype;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 64;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_IconCommandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IconCommandSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_AccidentQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<AccidentSite>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_ConfigQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PoliceConfigurationData>() });
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_PoliceRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<ServiceRequest>(),
			ComponentType.ReadWrite<PoliceEmergencyRequest>(),
			ComponentType.ReadWrite<RequestGroup>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_EventImpactArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Game.Common.Event>(),
			ComponentType.ReadWrite<Impact>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_AccidentQuery);
		Assert.IsTrue(true);
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
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		AccidentSiteJob accidentSiteJob = new AccidentSiteJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingType = InternalCompilerInterface.GetComponentTypeHandle<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AccidentSiteType = InternalCompilerInterface.GetComponentTypeHandle<AccidentSite>(ref __TypeHandle.__Game_Events_AccidentSite_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InvolvedInAccidentData = InternalCompilerInterface.GetComponentLookup<InvolvedInAccident>(ref __TypeHandle.__Game_Events_InvolvedInAccident_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CriminalData = InternalCompilerInterface.GetComponentLookup<Criminal>(ref __TypeHandle.__Game_Citizens_Criminal_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PoliceEmergencyRequestData = InternalCompilerInterface.GetComponentLookup<PoliceEmergencyRequest>(ref __TypeHandle.__Game_Simulation_PoliceEmergencyRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MovingData = InternalCompilerInterface.GetComponentLookup<Moving>(ref __TypeHandle.__Game_Objects_Moving_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_VehicleData = InternalCompilerInterface.GetComponentLookup<Vehicle>(ref __TypeHandle.__Game_Vehicles_Vehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarData = InternalCompilerInterface.GetComponentLookup<Car>(ref __TypeHandle.__Game_Vehicles_Car_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTrafficAccidentData = InternalCompilerInterface.GetComponentLookup<TrafficAccidentData>(ref __TypeHandle.__Game_Prefabs_TrafficAccidentData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCrimeData = InternalCompilerInterface.GetComponentLookup<CrimeData>(ref __TypeHandle.__Game_Prefabs_CrimeData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TargetElements = InternalCompilerInterface.GetBufferLookup<TargetElement>(ref __TypeHandle.__Game_Events_TargetElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubLanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneObjects = InternalCompilerInterface.GetBufferLookup<LaneObject>(ref __TypeHandle.__Game_Net_LaneObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RandomSeed = RandomSeed.Next(),
			m_SimulationFrame = m_SimulationSystem.frameIndex,
			m_PoliceRequestArchetype = m_PoliceRequestArchetype,
			m_EventImpactArchetype = m_EventImpactArchetype,
			m_PoliceConfigurationData = ((EntityQuery)(ref m_ConfigQuery)).GetSingleton<PoliceConfigurationData>()
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		accidentSiteJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		accidentSiteJob.m_IconCommandBuffer = m_IconCommandSystem.CreateCommandBuffer();
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<AccidentSiteJob>(accidentSiteJob, m_AccidentQuery, ((SystemBase)this).Dependency);
		m_EndFrameBarrier.AddJobHandleForProducer(val2);
		m_IconCommandSystem.AddCommandBufferWriter(val2);
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
	public AccidentSiteSystem()
	{
	}
}
