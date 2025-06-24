using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Creatures;
using Game.Economy;
using Game.Events;
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
public class CriminalSystem : GameSystemBase
{
	private struct CrimeData
	{
		public Entity m_Source;

		public Entity m_Target;

		public int m_StealAmount;

		public int m_EffectAmount;
	}

	[BurstCompile]
	private struct CriminalJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<TravelPurpose> m_TravelPurposeType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentBuilding> m_CurrentBuildingType;

		[ReadOnly]
		public ComponentTypeHandle<HealthProblem> m_HealthProblemType;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		public ComponentTypeHandle<Criminal> m_CriminalType;

		public BufferTypeHandle<TripNeeded> m_TripNeededType;

		[ReadOnly]
		public ComponentLookup<AccidentSite> m_AccidentSiteData;

		[ReadOnly]
		public ComponentLookup<HouseholdMember> m_HouseholdMemberData;

		[ReadOnly]
		public ComponentLookup<CurrentTransport> m_CurrentTransports;

		[ReadOnly]
		public ComponentLookup<Game.Creatures.Resident> m_Residents;

		[ReadOnly]
		public ComponentLookup<Dispatched> m_DispatchedData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.PoliceStation> m_PoliceStationData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Prison> m_PrisonData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PublicTransport> m_PublicTransportData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PoliceCar> m_PoliceCarData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<Game.Prefabs.CrimeData> m_PrefabCrimeData;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> m_HouseholdCitizens;

		[ReadOnly]
		public BufferLookup<ServiceDispatch> m_ServiceDispatches;

		[ReadOnly]
		public BufferLookup<Employee> m_Employees;

		[ReadOnly]
		public BufferLookup<Renter> m_Renters;

		[ReadOnly]
		public BufferLookup<Resources> m_Resources;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifiers;

		public BufferLookup<Occupant> m_Occupants;

		[ReadOnly]
		public uint m_UpdateFrameIndex;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public PoliceConfigurationData m_PoliceConfigurationData;

		[ReadOnly]
		public EntityArchetype m_AddAccidentSiteArchetype;

		[ReadOnly]
		public Entity m_City;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<CrimeData> m_CrimeQueue;

		public ParallelWriter<TriggerAction> m_TriggerBuffer;

		public ParallelWriter<StatisticsEvent> m_StatisticsEventQueue;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_059f: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_05af: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05be: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0689: Unknown result type (might be due to invalid IL or missing references)
			//IL_068e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0653: Unknown result type (might be due to invalid IL or missing references)
			//IL_0658: Unknown result type (might be due to invalid IL or missing references)
			//IL_065b: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_054e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0553: Unknown result type (might be due to invalid IL or missing references)
			//IL_0567: Unknown result type (might be due to invalid IL or missing references)
			//IL_056c: Unknown result type (might be due to invalid IL or missing references)
			//IL_057e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0580: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0404: Unknown result type (might be due to invalid IL or missing references)
			//IL_040e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0425: Unknown result type (might be due to invalid IL or missing references)
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_039e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_034b: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0709: Unknown result type (might be due to invalid IL or missing references)
			//IL_0713: Unknown result type (might be due to invalid IL or missing references)
			//IL_0524: Unknown result type (might be due to invalid IL or missing references)
			//IL_0529: Unknown result type (might be due to invalid IL or missing references)
			//IL_0530: Unknown result type (might be due to invalid IL or missing references)
			//IL_0532: Unknown result type (might be due to invalid IL or missing references)
			//IL_0450: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a6a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a6f: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0782: Unknown result type (might be due to invalid IL or missing references)
			//IL_0787: Unknown result type (might be due to invalid IL or missing references)
			//IL_072f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0739: Unknown result type (might be due to invalid IL or missing references)
			//IL_0476: Unknown result type (might be due to invalid IL or missing references)
			//IL_0482: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04de: Unknown result type (might be due to invalid IL or missing references)
			//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a86: Unknown result type (might be due to invalid IL or missing references)
			//IL_0800: Unknown result type (might be due to invalid IL or missing references)
			//IL_0805: Unknown result type (might be due to invalid IL or missing references)
			//IL_080b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0810: Unknown result type (might be due to invalid IL or missing references)
			//IL_0818: Unknown result type (might be due to invalid IL or missing references)
			//IL_081f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0824: Unknown result type (might be due to invalid IL or missing references)
			//IL_0827: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_094a: Unknown result type (might be due to invalid IL or missing references)
			//IL_094f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0954: Unknown result type (might be due to invalid IL or missing references)
			//IL_0957: Unknown result type (might be due to invalid IL or missing references)
			//IL_0959: Unknown result type (might be due to invalid IL or missing references)
			//IL_095e: Unknown result type (might be due to invalid IL or missing references)
			//IL_096b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a03: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a11: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a33: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a35: Unknown result type (might be due to invalid IL or missing references)
			//IL_0982: Unknown result type (might be due to invalid IL or missing references)
			//IL_0996: Unknown result type (might be due to invalid IL or missing references)
			//IL_0860: Unknown result type (might be due to invalid IL or missing references)
			//IL_09aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0910: Unknown result type (might be due to invalid IL or missing references)
			//IL_0912: Unknown result type (might be due to invalid IL or missing references)
			//IL_0914: Unknown result type (might be due to invalid IL or missing references)
			//IL_0923: Unknown result type (might be due to invalid IL or missing references)
			//IL_0928: Unknown result type (might be due to invalid IL or missing references)
			//IL_092c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0874: Unknown result type (might be due to invalid IL or missing references)
			//IL_0888: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_089c: Unknown result type (might be due to invalid IL or missing references)
			//IL_08aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ed: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index != m_UpdateFrameIndex)
			{
				return;
			}
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<TravelPurpose> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TravelPurpose>(ref m_TravelPurposeType);
			NativeArray<CurrentBuilding> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentBuilding>(ref m_CurrentBuildingType);
			NativeArray<HealthProblem> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HealthProblem>(ref m_HealthProblemType);
			NativeArray<Criminal> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Criminal>(ref m_CriminalType);
			BufferAccessor<TripNeeded> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TripNeeded>(ref m_TripNeededType);
			DynamicBuffer<CityModifier> modifiers = m_CityModifiers[m_City];
			for (int i = 0; i < nativeArray5.Length; i++)
			{
				Criminal criminal = nativeArray5[i];
				if (criminal.m_Flags == (CriminalFlags)0)
				{
					Entity val = nativeArray[i];
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Criminal>(unfilteredChunkIndex, val);
				}
				else if ((criminal.m_Flags & CriminalFlags.Prisoner) != 0)
				{
					if (nativeArray3.Length == 0)
					{
						continue;
					}
					CurrentBuilding currentBuilding = nativeArray3[i];
					if (m_PrisonData.HasComponent(currentBuilding.m_CurrentBuilding))
					{
						if (m_BuildingData.HasComponent(currentBuilding.m_CurrentBuilding) && BuildingUtils.CheckOption(m_BuildingData[currentBuilding.m_CurrentBuilding], BuildingOption.Inactive))
						{
							Entity val2 = nativeArray[i];
							RemoveTravelPurpose(unfilteredChunkIndex, val2, nativeArray2, i);
							if (m_Occupants.HasBuffer(currentBuilding.m_CurrentBuilding))
							{
								CollectionUtils.RemoveValue<Occupant>(m_Occupants[currentBuilding.m_CurrentBuilding], new Occupant(val2));
								continue;
							}
						}
						criminal.m_JailTime = (ushort)math.max(0, criminal.m_JailTime - 1);
						if (criminal.m_JailTime == 0)
						{
							Entity val3 = nativeArray[i];
							criminal.m_Flags = (CriminalFlags)0;
							criminal.m_Event = Entity.Null;
							RemoveTravelPurpose(unfilteredChunkIndex, val3, nativeArray2, i);
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Criminal>(unfilteredChunkIndex, val3);
						}
						nativeArray5[i] = criminal;
					}
					else
					{
						Entity entity = nativeArray[i];
						criminal.m_Flags &= ~(CriminalFlags.Prisoner | CriminalFlags.Arrested | CriminalFlags.Sentenced);
						criminal.m_Event = Entity.Null;
						nativeArray5[i] = criminal;
						RemoveTravelPurpose(unfilteredChunkIndex, entity, nativeArray2, i);
					}
				}
				else if ((criminal.m_Flags & CriminalFlags.Arrested) != 0)
				{
					if (nativeArray3.Length == 0)
					{
						continue;
					}
					CurrentBuilding currentBuilding2 = nativeArray3[i];
					if (m_PoliceStationData.HasComponent(currentBuilding2.m_CurrentBuilding))
					{
						if (m_BuildingData.HasComponent(currentBuilding2.m_CurrentBuilding) && BuildingUtils.CheckOption(m_BuildingData[currentBuilding2.m_CurrentBuilding], BuildingOption.Inactive))
						{
							Entity val4 = nativeArray[i];
							RemoveTravelPurpose(unfilteredChunkIndex, val4, nativeArray2, i);
							if (m_Occupants.HasBuffer(currentBuilding2.m_CurrentBuilding))
							{
								CollectionUtils.RemoveValue<Occupant>(m_Occupants[currentBuilding2.m_CurrentBuilding], new Occupant(val4));
								continue;
							}
						}
						if ((criminal.m_Flags & CriminalFlags.Sentenced) != 0)
						{
							Game.Buildings.PoliceStation policeStation = m_PoliceStationData[currentBuilding2.m_CurrentBuilding];
							if (GetTransportVehicle(policeStation, out var vehicle) && CheckHealth(nativeArray4, i))
							{
								Entity entity2 = nativeArray[i];
								DynamicBuffer<TripNeeded> tripNeededs = bufferAccessor[i];
								criminal.m_Flags |= CriminalFlags.Prisoner;
								criminal.m_Event = Entity.Null;
								nativeArray5[i] = criminal;
								GoToPrison(unfilteredChunkIndex, entity2, tripNeededs, vehicle);
								continue;
							}
							criminal.m_JailTime = (ushort)math.max(0, criminal.m_JailTime - 1);
							if (criminal.m_JailTime == 0)
							{
								Entity val5 = nativeArray[i];
								criminal.m_Flags = (CriminalFlags)0;
								criminal.m_Event = Entity.Null;
								RemoveTravelPurpose(unfilteredChunkIndex, val5, nativeArray2, i);
								((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Criminal>(unfilteredChunkIndex, val5);
							}
							nativeArray5[i] = criminal;
							continue;
						}
						criminal.m_JailTime = (ushort)math.max(0, criminal.m_JailTime - 1);
						if (criminal.m_JailTime == 0)
						{
							Entity val6 = nativeArray[i];
							Random random = m_RandomSeed.GetRandom(val6.Index);
							if (m_PrefabRefData.HasComponent(criminal.m_Event))
							{
								PrefabRef prefabRef = m_PrefabRefData[criminal.m_Event];
								if (m_PrefabCrimeData.HasComponent(prefabRef.m_Prefab))
								{
									Game.Prefabs.CrimeData crimeData = m_PrefabCrimeData[prefabRef.m_Prefab];
									if (((Random)(ref random)).NextFloat(100f) < crimeData.m_PrisonProbability)
									{
										float value = math.lerp(crimeData.m_PrisonTimeRange.min, crimeData.m_PrisonTimeRange.max, ((Random)(ref random)).NextFloat(1f));
										CityUtils.ApplyModifier(ref value, modifiers, CityModifierType.PrisonTime);
										criminal.m_Flags |= CriminalFlags.Sentenced;
										criminal.m_JailTime = (ushort)math.min(65535f, value * 262144f / 256f);
										criminal.m_Event = Entity.Null;
										m_TriggerBuffer.Enqueue(new TriggerAction(TriggerType.CitizenGotSentencedToPrison, Entity.Null, val6, Entity.Null));
									}
								}
							}
							if ((criminal.m_Flags & CriminalFlags.Sentenced) == 0)
							{
								criminal.m_Flags &= ~CriminalFlags.Arrested;
								criminal.m_Event = Entity.Null;
								RemoveTravelPurpose(unfilteredChunkIndex, val6, nativeArray2, i);
							}
						}
						nativeArray5[i] = criminal;
					}
					else
					{
						Entity entity3 = nativeArray[i];
						criminal.m_Flags &= ~(CriminalFlags.Arrested | CriminalFlags.Sentenced);
						criminal.m_Event = Entity.Null;
						nativeArray5[i] = criminal;
						RemoveTravelPurpose(unfilteredChunkIndex, entity3, nativeArray2, i);
					}
				}
				else if ((criminal.m_Flags & CriminalFlags.Planning) != 0)
				{
					Entity val7 = nativeArray[i];
					DynamicBuffer<TripNeeded> tripNeededs2 = bufferAccessor[i];
					Random random2 = m_RandomSeed.GetRandom(val7.Index);
					if (!IsPreparingCrime(tripNeededs2))
					{
						tripNeededs2.Add(new TripNeeded
						{
							m_Purpose = Purpose.Crime
						});
					}
					float value2 = 0f;
					CityUtils.ApplyModifier(ref value2, modifiers, CityModifierType.CriminalMonitorProbability);
					if (((Random)(ref random2)).NextFloat(100f) < value2)
					{
						criminal.m_Flags |= CriminalFlags.Monitored;
					}
					criminal.m_Flags &= ~CriminalFlags.Planning;
					criminal.m_Flags |= CriminalFlags.Preparing;
					nativeArray5[i] = criminal;
				}
				else if ((criminal.m_Flags & CriminalFlags.Preparing) != 0)
				{
					DynamicBuffer<TripNeeded> tripNeededs3 = bufferAccessor[i];
					if (!IsPreparingCrime(tripNeededs3))
					{
						criminal.m_Flags &= ~CriminalFlags.Preparing;
						nativeArray5[i] = criminal;
					}
				}
				else
				{
					if (!(criminal.m_Event != Entity.Null))
					{
						continue;
					}
					TravelPurpose travelPurpose = default(TravelPurpose);
					CurrentBuilding currentBuilding3 = default(CurrentBuilding);
					if (nativeArray2.Length != 0)
					{
						travelPurpose = nativeArray2[i];
					}
					if (nativeArray3.Length != 0)
					{
						currentBuilding3 = nativeArray3[i];
					}
					if (travelPurpose.m_Purpose == Purpose.GoingToJail && m_CurrentTransports.HasComponent(nativeArray[i]) && m_Residents.HasComponent(m_CurrentTransports[nativeArray[i]].m_CurrentTransport) && (m_Residents[m_CurrentTransports[nativeArray[i]].m_CurrentTransport].m_Flags & ResidentFlags.InVehicle) != ResidentFlags.None)
					{
						criminal.m_Flags |= CriminalFlags.Arrested;
						nativeArray5[i] = criminal;
					}
					else if (travelPurpose.m_Purpose != Purpose.Crime && travelPurpose.m_Purpose != Purpose.GoingToJail)
					{
						criminal.m_Event = Entity.Null;
						criminal.m_Flags &= ~CriminalFlags.Monitored;
						nativeArray5[i] = criminal;
					}
					else if (m_AccidentSiteData.HasComponent(currentBuilding3.m_CurrentBuilding))
					{
						AccidentSite accidentSite = m_AccidentSiteData[currentBuilding3.m_CurrentBuilding];
						if ((accidentSite.m_Flags & (AccidentSiteFlags.Secured | AccidentSiteFlags.CrimeScene | AccidentSiteFlags.CrimeFinished)) == AccidentSiteFlags.CrimeScene && !(accidentSite.m_Event != criminal.m_Event))
						{
							continue;
						}
						Entity val8 = nativeArray[i];
						DynamicBuffer<TripNeeded> tripNeededs4 = bufferAccessor[i];
						Random random3 = m_RandomSeed.GetRandom(val8.Index);
						if (!CheckHealth(nativeArray4, i))
						{
							continue;
						}
						if ((accidentSite.m_Flags & AccidentSiteFlags.Secured) != 0 && GetPoliceCar(accidentSite, out var vehicle2))
						{
							float num = 0f;
							if (m_PrefabRefData.HasComponent(criminal.m_Event))
							{
								PrefabRef prefabRef2 = m_PrefabRefData[criminal.m_Event];
								if (m_PrefabCrimeData.HasComponent(prefabRef2.m_Prefab))
								{
									Game.Prefabs.CrimeData crimeData2 = m_PrefabCrimeData[prefabRef2.m_Prefab];
									num = math.lerp(crimeData2.m_JailTimeRange.min, crimeData2.m_JailTimeRange.max, ((Random)(ref random3)).NextFloat(1f));
								}
							}
							criminal.m_Flags &= ~CriminalFlags.Monitored;
							criminal.m_JailTime = (ushort)math.min(65535f, num * 262144f / 256f);
							nativeArray5[i] = criminal;
							GoToJail(unfilteredChunkIndex, val8, tripNeededs4, vehicle2);
							m_TriggerBuffer.Enqueue(new TriggerAction(TriggerType.CitizenGotArrested, Entity.Null, val8, criminal.m_Event));
							continue;
						}
						Entity crimeSource = GetCrimeSource(ref random3, currentBuilding3.m_CurrentBuilding);
						Entity crimeTarget = GetCrimeTarget(val8);
						int num2 = 0;
						if (m_PrefabRefData.HasComponent(criminal.m_Event))
						{
							PrefabRef prefabRef3 = m_PrefabRefData[criminal.m_Event];
							if (m_PrefabCrimeData.HasComponent(prefabRef3.m_Prefab))
							{
								Game.Prefabs.CrimeData crimeData3 = m_PrefabCrimeData[prefabRef3.m_Prefab];
								if (crimeData3.m_CrimeType == CrimeType.Robbery)
								{
									num2 = GetStealAmount(ref random3, crimeSource, crimeData3);
									if (num2 > 0)
									{
										m_CrimeQueue.Enqueue(new CrimeData
										{
											m_Source = crimeSource,
											m_Target = crimeTarget,
											m_StealAmount = num2
										});
									}
								}
							}
						}
						AddCrimeEffects(crimeSource);
						criminal.m_Event = Entity.Null;
						criminal.m_Flags &= ~CriminalFlags.Monitored;
						nativeArray5[i] = criminal;
						TryEscape(unfilteredChunkIndex, val8, tripNeededs4);
						m_StatisticsEventQueue.Enqueue(new StatisticsEvent
						{
							m_Statistic = StatisticType.EscapedArrestCount,
							m_Change = 1f
						});
					}
					else if (currentBuilding3.m_CurrentBuilding != Entity.Null)
					{
						AddCrimeScene(unfilteredChunkIndex, criminal.m_Event, currentBuilding3.m_CurrentBuilding);
					}
				}
			}
		}

		private void RemoveTravelPurpose(int jobIndex, Entity entity, NativeArray<TravelPurpose> travelPurposes, int index)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			TravelPurpose travelPurpose = default(TravelPurpose);
			if (CollectionUtils.TryGet<TravelPurpose>(travelPurposes, index, ref travelPurpose) && (travelPurpose.m_Purpose == Purpose.GoingToPrison || travelPurpose.m_Purpose == Purpose.InPrison || travelPurpose.m_Purpose == Purpose.GoingToJail || travelPurpose.m_Purpose == Purpose.InJail))
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TravelPurpose>(jobIndex, entity);
			}
		}

		private bool CheckHealth(NativeArray<HealthProblem> healthProblems, int index)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			HealthProblem healthProblem = default(HealthProblem);
			if (CollectionUtils.TryGet<HealthProblem>(healthProblems, index, ref healthProblem) && (healthProblem.m_Flags & HealthProblemFlags.RequireTransport) != HealthProblemFlags.None)
			{
				return false;
			}
			return true;
		}

		private void GoToPrison(int jobIndex, Entity entity, DynamicBuffer<TripNeeded> tripNeededs, Entity vehicle)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < tripNeededs.Length; i++)
			{
				if (tripNeededs[i].m_Purpose == Purpose.GoingToPrison)
				{
					return;
				}
			}
			tripNeededs.Add(new TripNeeded
			{
				m_Purpose = Purpose.GoingToPrison,
				m_TargetAgent = vehicle
			});
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TravelPurpose>(jobIndex, entity);
		}

		private void GoToJail(int jobIndex, Entity entity, DynamicBuffer<TripNeeded> tripNeededs, Entity vehicle)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < tripNeededs.Length; i++)
			{
				if (tripNeededs[i].m_Purpose == Purpose.GoingToJail)
				{
					return;
				}
			}
			tripNeededs.Add(new TripNeeded
			{
				m_Purpose = Purpose.GoingToJail,
				m_TargetAgent = vehicle
			});
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TravelPurpose>(jobIndex, entity);
		}

		private bool GetTransportVehicle(Game.Buildings.PoliceStation policeStation, out Entity vehicle)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			vehicle = Entity.Null;
			if (!m_DispatchedData.HasComponent(policeStation.m_PrisonerTransportRequest))
			{
				return false;
			}
			Dispatched dispatched = m_DispatchedData[policeStation.m_PrisonerTransportRequest];
			if (!m_PublicTransportData.HasComponent(dispatched.m_Handler))
			{
				return false;
			}
			if ((m_PublicTransportData[dispatched.m_Handler].m_State & PublicTransportFlags.Boarding) == 0)
			{
				return false;
			}
			if (!m_ServiceDispatches.HasBuffer(dispatched.m_Handler))
			{
				return false;
			}
			DynamicBuffer<ServiceDispatch> val = m_ServiceDispatches[dispatched.m_Handler];
			if (val.Length == 0 || val[0].m_Request != policeStation.m_PrisonerTransportRequest)
			{
				return false;
			}
			vehicle = dispatched.m_Handler;
			return true;
		}

		private bool GetPoliceCar(AccidentSite accidentSite, out Entity vehicle)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			vehicle = Entity.Null;
			if (!m_DispatchedData.HasComponent(accidentSite.m_PoliceRequest))
			{
				return false;
			}
			Dispatched dispatched = m_DispatchedData[accidentSite.m_PoliceRequest];
			if (!m_PoliceCarData.HasComponent(dispatched.m_Handler))
			{
				return false;
			}
			if ((m_PoliceCarData[dispatched.m_Handler].m_State & PoliceCarFlags.AtTarget) == 0)
			{
				return false;
			}
			if (!m_ServiceDispatches.HasBuffer(dispatched.m_Handler))
			{
				return false;
			}
			DynamicBuffer<ServiceDispatch> val = m_ServiceDispatches[dispatched.m_Handler];
			if (val.Length == 0 || val[0].m_Request != accidentSite.m_PoliceRequest)
			{
				return false;
			}
			vehicle = dispatched.m_Handler;
			return true;
		}

		private void AddCrimeEffects(Entity source)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			if (m_HouseholdCitizens.HasBuffer(source))
			{
				DynamicBuffer<HouseholdCitizen> val = m_HouseholdCitizens[source];
				for (int i = 0; i < val.Length; i++)
				{
					Entity citizen = val[i].m_Citizen;
					if (m_PrefabRefData.HasComponent(citizen))
					{
						m_CrimeQueue.Enqueue(new CrimeData
						{
							m_Source = citizen,
							m_Target = Entity.Null,
							m_EffectAmount = m_PoliceConfigurationData.m_HomeCrimeEffect
						});
					}
				}
			}
			if (!m_Employees.HasBuffer(source))
			{
				return;
			}
			DynamicBuffer<Employee> val2 = m_Employees[source];
			for (int j = 0; j < val2.Length; j++)
			{
				Entity worker = val2[j].m_Worker;
				if (m_PrefabRefData.HasComponent(worker))
				{
					m_CrimeQueue.Enqueue(new CrimeData
					{
						m_Source = worker,
						m_Target = Entity.Null,
						m_EffectAmount = m_PoliceConfigurationData.m_WorkplaceCrimeEffect
					});
				}
			}
		}

		private Entity GetCrimeSource(ref Random random, Entity building)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			if (m_Renters.HasBuffer(building))
			{
				DynamicBuffer<Renter> val = m_Renters[building];
				if (val.Length > 0)
				{
					return val[((Random)(ref random)).NextInt(val.Length)].m_Renter;
				}
			}
			return building;
		}

		private Entity GetCrimeTarget(Entity criminal)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (m_HouseholdMemberData.HasComponent(criminal))
			{
				return m_HouseholdMemberData[criminal].m_Household;
			}
			return criminal;
		}

		private int GetStealAmount(ref Random random, Entity source, Game.Prefabs.CrimeData crimeData)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			float num = 0f;
			if (m_Resources.HasBuffer(source))
			{
				DynamicBuffer<Resources> resources = m_Resources[source];
				int resources2 = EconomyUtils.GetResources(Resource.Money, resources);
				if (resources2 > 0)
				{
					num += math.lerp(crimeData.m_CrimeIncomeRelative.min, crimeData.m_CrimeIncomeRelative.max, ((Random)(ref random)).NextFloat(1f)) * (float)resources2;
				}
				num += math.lerp(crimeData.m_CrimeIncomeAbsolute.min, crimeData.m_CrimeIncomeAbsolute.max, ((Random)(ref random)).NextFloat(1f));
			}
			return (int)num;
		}

		private void TryEscape(int jobIndex, Entity entity, DynamicBuffer<TripNeeded> tripNeededs)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			tripNeededs.Clear();
			tripNeededs.Add(new TripNeeded
			{
				m_Purpose = Purpose.Escape
			});
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<ResourceBuyer>(jobIndex, entity);
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TravelPurpose>(jobIndex, entity);
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<AttendingMeeting>(jobIndex, entity);
		}

		private bool IsPreparingCrime(DynamicBuffer<TripNeeded> tripNeededs)
		{
			for (int i = 0; i < tripNeededs.Length; i++)
			{
				if (tripNeededs[i].m_Purpose == Purpose.Crime)
				{
					return true;
				}
			}
			return false;
		}

		private void AddCrimeScene(int jobIndex, Entity _event, Entity building)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			AddAccidentSite addAccidentSite = new AddAccidentSite
			{
				m_Event = _event,
				m_Target = building,
				m_Flags = AccidentSiteFlags.CrimeScene
			};
			Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_AddAccidentSiteArchetype);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<AddAccidentSite>(jobIndex, val, addAccidentSite);
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct CrimeJob : IJob
	{
		public ComponentLookup<CrimeVictim> m_CrimeVictimData;

		public BufferLookup<Resources> m_Resources;

		public NativeQueue<CrimeData> m_CrimeQueue;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			int count = m_CrimeQueue.Count;
			if (count == 0)
			{
				return;
			}
			NativeParallelHashMap<Entity, CrimeVictim> val = default(NativeParallelHashMap<Entity, CrimeVictim>);
			val._002Ector(count, AllocatorHandle.op_Implicit((Allocator)2));
			CrimeVictim crimeVictim = default(CrimeVictim);
			for (int i = 0; i < count; i++)
			{
				CrimeData crimeData = m_CrimeQueue.Dequeue();
				if (crimeData.m_StealAmount > 0)
				{
					if (!m_Resources.HasBuffer(crimeData.m_Source) || !m_Resources.HasBuffer(crimeData.m_Target))
					{
						continue;
					}
					DynamicBuffer<Resources> resources = m_Resources[crimeData.m_Source];
					DynamicBuffer<Resources> resources2 = m_Resources[crimeData.m_Target];
					EconomyUtils.AddResources(Resource.Money, -crimeData.m_StealAmount, resources);
					EconomyUtils.AddResources(Resource.Money, crimeData.m_StealAmount, resources2);
				}
				if (crimeData.m_EffectAmount > 0)
				{
					if (val.TryGetValue(crimeData.m_Source, ref crimeVictim))
					{
						crimeVictim.m_Effect = (byte)math.min(crimeVictim.m_Effect + crimeData.m_EffectAmount, 255);
						val[crimeData.m_Source] = crimeVictim;
					}
					else if (m_CrimeVictimData.HasComponent(crimeData.m_Source) && m_CrimeVictimData.IsComponentEnabled(crimeData.m_Source))
					{
						crimeVictim = m_CrimeVictimData[crimeData.m_Source];
						crimeVictim.m_Effect = (byte)math.min(crimeVictim.m_Effect + crimeData.m_EffectAmount, 255);
						val.Add(crimeData.m_Source, crimeVictim);
					}
					else
					{
						crimeVictim.m_Effect = (byte)math.min(crimeData.m_EffectAmount, 255);
						val.Add(crimeData.m_Source, crimeVictim);
					}
				}
			}
			if (val.Count() <= 0)
			{
				return;
			}
			NativeArray<Entity> keyArray = val.GetKeyArray(AllocatorHandle.op_Implicit((Allocator)2));
			for (int j = 0; j < keyArray.Length; j++)
			{
				Entity val2 = keyArray[j];
				CrimeVictim crimeVictim2 = val[val2];
				if (m_CrimeVictimData.HasComponent(val2) && !m_CrimeVictimData.IsComponentEnabled(val2))
				{
					m_CrimeVictimData.SetComponentEnabled(val2, true);
				}
				m_CrimeVictimData[val2] = crimeVictim2;
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TravelPurpose> __Game_Citizens_TravelPurpose_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentBuilding> __Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<HealthProblem> __Game_Citizens_HealthProblem_RO_ComponentTypeHandle;

		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		public ComponentTypeHandle<Criminal> __Game_Citizens_Criminal_RW_ComponentTypeHandle;

		public BufferTypeHandle<TripNeeded> __Game_Citizens_TripNeeded_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<AccidentSite> __Game_Events_AccidentSite_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HouseholdMember> __Game_Citizens_HouseholdMember_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentTransport> __Game_Citizens_CurrentTransport_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Creatures.Resident> __Game_Creatures_Resident_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Dispatched> __Game_Simulation_Dispatched_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.PoliceStation> __Game_Buildings_PoliceStation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Prison> __Game_Buildings_Prison_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PublicTransport> __Game_Vehicles_PublicTransport_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PoliceCar> __Game_Vehicles_PoliceCar_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Prefabs.CrimeData> __Game_Prefabs_CrimeData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ServiceDispatch> __Game_Simulation_ServiceDispatch_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Employee> __Game_Companies_Employee_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Renter> __Game_Buildings_Renter_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Resources> __Game_Economy_Resources_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<CityModifier> __Game_City_CityModifier_RO_BufferLookup;

		public BufferLookup<Occupant> __Game_Buildings_Occupant_RW_BufferLookup;

		public ComponentLookup<CrimeVictim> __Game_Citizens_CrimeVictim_RW_ComponentLookup;

		public BufferLookup<Resources> __Game_Economy_Resources_RW_BufferLookup;

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
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Citizens_TravelPurpose_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TravelPurpose>(true);
			__Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentBuilding>(true);
			__Game_Citizens_HealthProblem_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HealthProblem>(true);
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Citizens_Criminal_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Criminal>(false);
			__Game_Citizens_TripNeeded_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<TripNeeded>(false);
			__Game_Events_AccidentSite_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AccidentSite>(true);
			__Game_Citizens_HouseholdMember_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HouseholdMember>(true);
			__Game_Citizens_CurrentTransport_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentTransport>(true);
			__Game_Creatures_Resident_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Creatures.Resident>(true);
			__Game_Simulation_Dispatched_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Dispatched>(true);
			__Game_Buildings_PoliceStation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.PoliceStation>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Buildings_Prison_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.Prison>(true);
			__Game_Vehicles_PublicTransport_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.PublicTransport>(true);
			__Game_Vehicles_PoliceCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.PoliceCar>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_CrimeData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Prefabs.CrimeData>(true);
			__Game_Citizens_HouseholdCitizen_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdCitizen>(true);
			__Game_Simulation_ServiceDispatch_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceDispatch>(true);
			__Game_Companies_Employee_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Employee>(true);
			__Game_Buildings_Renter_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Renter>(true);
			__Game_Economy_Resources_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Resources>(true);
			__Game_City_CityModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(true);
			__Game_Buildings_Occupant_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Occupant>(false);
			__Game_Citizens_CrimeVictim_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CrimeVictim>(false);
			__Game_Economy_Resources_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Resources>(false);
		}
	}

	public const uint SYSTEM_UPDATE_INTERVAL = 16u;

	private EndFrameBarrier m_EndFrameBarrier;

	private SimulationSystem m_SimulationSystem;

	private CityStatisticsSystem m_CityStatisticsSystem;

	private CitySystem m_CitySystem;

	private EntityQuery m_CriminalQuery;

	private EntityQuery m_PoliceConfigQuery;

	private EntityArchetype m_AddAccidentSiteArchetype;

	private TriggerSystem m_TriggerSystem;

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
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_TriggerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TriggerSystem>();
		m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
		m_CriminalQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Citizen>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.ReadWrite<Criminal>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_PoliceConfigQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PoliceConfigurationData>() });
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_AddAccidentSiteArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Game.Common.Event>(),
			ComponentType.ReadWrite<AddAccidentSite>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_CriminalQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_PoliceConfigQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		uint updateFrameIndex = (m_SimulationSystem.frameIndex / 16) & 0xF;
		NativeQueue<CrimeData> crimeQueue = default(NativeQueue<CrimeData>);
		crimeQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		JobHandle deps;
		CriminalJob criminalJob = new CriminalJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TravelPurposeType = InternalCompilerInterface.GetComponentTypeHandle<TravelPurpose>(ref __TypeHandle.__Game_Citizens_TravelPurpose_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentBuildingType = InternalCompilerInterface.GetComponentTypeHandle<CurrentBuilding>(ref __TypeHandle.__Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HealthProblemType = InternalCompilerInterface.GetComponentTypeHandle<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CriminalType = InternalCompilerInterface.GetComponentTypeHandle<Criminal>(ref __TypeHandle.__Game_Citizens_Criminal_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TripNeededType = InternalCompilerInterface.GetBufferTypeHandle<TripNeeded>(ref __TypeHandle.__Game_Citizens_TripNeeded_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AccidentSiteData = InternalCompilerInterface.GetComponentLookup<AccidentSite>(ref __TypeHandle.__Game_Events_AccidentSite_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdMemberData = InternalCompilerInterface.GetComponentLookup<HouseholdMember>(ref __TypeHandle.__Game_Citizens_HouseholdMember_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentTransports = InternalCompilerInterface.GetComponentLookup<CurrentTransport>(ref __TypeHandle.__Game_Citizens_CurrentTransport_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Residents = InternalCompilerInterface.GetComponentLookup<Game.Creatures.Resident>(ref __TypeHandle.__Game_Creatures_Resident_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DispatchedData = InternalCompilerInterface.GetComponentLookup<Dispatched>(ref __TypeHandle.__Game_Simulation_Dispatched_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PoliceStationData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.PoliceStation>(ref __TypeHandle.__Game_Buildings_PoliceStation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrisonData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.Prison>(ref __TypeHandle.__Game_Buildings_Prison_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PublicTransportData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.PublicTransport>(ref __TypeHandle.__Game_Vehicles_PublicTransport_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PoliceCarData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.PoliceCar>(ref __TypeHandle.__Game_Vehicles_PoliceCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCrimeData = InternalCompilerInterface.GetComponentLookup<Game.Prefabs.CrimeData>(ref __TypeHandle.__Game_Prefabs_CrimeData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdCitizens = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceDispatches = InternalCompilerInterface.GetBufferLookup<ServiceDispatch>(ref __TypeHandle.__Game_Simulation_ServiceDispatch_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Employees = InternalCompilerInterface.GetBufferLookup<Employee>(ref __TypeHandle.__Game_Companies_Employee_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Renters = InternalCompilerInterface.GetBufferLookup<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Resources = InternalCompilerInterface.GetBufferLookup<Resources>(ref __TypeHandle.__Game_Economy_Resources_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CityModifiers = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Occupants = InternalCompilerInterface.GetBufferLookup<Occupant>(ref __TypeHandle.__Game_Buildings_Occupant_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TriggerBuffer = m_TriggerSystem.CreateActionBuffer().AsParallelWriter(),
			m_StatisticsEventQueue = m_CityStatisticsSystem.GetStatisticsEventQueue(out deps).AsParallelWriter(),
			m_UpdateFrameIndex = updateFrameIndex,
			m_RandomSeed = RandomSeed.Next(),
			m_PoliceConfigurationData = ((EntityQuery)(ref m_PoliceConfigQuery)).GetSingleton<PoliceConfigurationData>(),
			m_AddAccidentSiteArchetype = m_AddAccidentSiteArchetype,
			m_City = m_CitySystem.City
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		criminalJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		criminalJob.m_CrimeQueue = crimeQueue.AsParallelWriter();
		CriminalJob criminalJob2 = criminalJob;
		CrimeJob obj = new CrimeJob
		{
			m_CrimeVictimData = InternalCompilerInterface.GetComponentLookup<CrimeVictim>(ref __TypeHandle.__Game_Citizens_CrimeVictim_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Resources = InternalCompilerInterface.GetBufferLookup<Resources>(ref __TypeHandle.__Game_Economy_Resources_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CrimeQueue = crimeQueue,
			m_CommandBuffer = m_EndFrameBarrier.CreateCommandBuffer()
		};
		JobHandle val2 = JobChunkExtensions.Schedule<CriminalJob>(criminalJob2, m_CriminalQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, deps));
		JobHandle val3 = IJobExtensions.Schedule<CrimeJob>(obj, val2);
		crimeQueue.Dispose(val3);
		m_TriggerSystem.AddActionBufferWriter(val3);
		m_EndFrameBarrier.AddJobHandleForProducer(val3);
		m_CityStatisticsSystem.AddWriter(val2);
		((SystemBase)this).Dependency = val3;
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
	public CriminalSystem()
	{
	}
}
